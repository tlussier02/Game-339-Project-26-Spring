using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Game339.Shared.Services;
using UnityEngine;

namespace Game.Runtime.FarmMatch
{
    public sealed class FarmMatchGameModel
    {
        private readonly IFarmMatchBoard _board;
        private readonly IFarmMatchScoreService _scoreService;
        private readonly FarmMatchRules _rules;
        private readonly Timer _timer;
        private readonly List<GridPosition> _selection = new List<GridPosition>();
        private int _savedHighScore;

        public FarmMatchGameModel(
            IFarmMatchBoard board,
            ITimeProvider timeProvider,
            IFarmMatchScoreService scoreService = null,
            FarmMatchRules rules = null)
        {
            _board = board ?? throw new ArgumentNullException(nameof(board));
            _timer = new Timer(timeProvider ?? throw new ArgumentNullException(nameof(timeProvider)));
            _scoreService = scoreService ?? new FarmMatchScoreService();
            _rules = rules ?? new FarmMatchRules();
            _rules.Validate();

            State = new FarmMatchGameState
            {
                TimeRemainingSeconds = _rules.RoundDurationSeconds,
                TargetScore = GetTargetScoreForRound(1)
            };
        }

        public event Action StateChanged;

        public event Action<FarmMatchResolution> MatchResolved;

        public event Action<FarmMatchRoundProgress> RoundAdvanced;

        public event Action<FarmMatchRoundResult> RoundEnded;

        public event Action<int> HighScoreChanged;

        public FarmMatchGameState State { get; }

        public IReadOnlyList<GridPosition> CurrentSelection => _selection;

        public void SetHighScore(int highScore)
        {
            _savedHighScore = Math.Max(0, highScore);
            State.HighScore = _savedHighScore;
            RaiseStateChanged();
        }

        public void StartNewRound()
        {
            _board.ResetBoard();
            ClearSelection(FarmMatchSelectionClearReason.RoundRestarted);

            State.RoundState = FarmMatchRoundState.Playing;
            State.EndReason = FarmMatchRoundEndReason.None;
            State.CurrentScore = 0;
            State.FinalScore = 0;
            State.HighScore = _savedHighScore;
            State.RoundNumber = 1;
            State.TimeRemainingSeconds = _rules.RoundDurationSeconds;
            State.TargetScore = GetTargetScoreForRound(State.RoundNumber);
            State.LastSelectionFailureReason = FarmMatchSelectionFailureReason.None;
            State.LastSelectionClearReason = FarmMatchSelectionClearReason.RoundRestarted;
            State.LastAwardedScore = 0;
            State.LastMatchedCropCount = 0;
            State.LastMatchedCropType = FarmCropType.None;

            _timer.Start(_rules.RoundDurationSeconds);
            RaiseStateChanged();
        }

        public void ReturnToTitle()
        {
            ClearSelection(FarmMatchSelectionClearReason.RoundEnded);

            State.RoundState = FarmMatchRoundState.Title;
            State.EndReason = FarmMatchRoundEndReason.None;
            State.CurrentScore = 0;
            State.FinalScore = 0;
            State.RoundNumber = 1;
            State.TimeRemainingSeconds = _rules.RoundDurationSeconds;
            State.TargetScore = GetTargetScoreForRound(State.RoundNumber);
            State.LastSelectionFailureReason = FarmMatchSelectionFailureReason.None;
            State.LastSelectionClearReason = FarmMatchSelectionClearReason.RoundEnded;
            State.LastAwardedScore = 0;
            State.LastMatchedCropCount = 0;
            State.LastMatchedCropType = FarmCropType.None;

            RaiseStateChanged();
        }

        public void Tick()
        {
            if (State.RoundState != FarmMatchRoundState.Playing)
            {
                return;
            }

            _timer.Tick();
            State.TimeRemainingSeconds = _timer.Current;

            if (State.TimeRemainingSeconds <= 0f)
            {
                EndRound(FarmMatchRoundEndReason.TimerExpired);
                return;
            }

            RaiseStateChanged();
        }

        public FarmMatchSelectionResult TrySelectCell(int row, int column)
        {
            return TrySelect(new GridPosition(row, column));
        }

        public FarmMatchSelectionResult TrySelect(GridPosition position)
        {
            if (State.RoundState != FarmMatchRoundState.Playing)
            {
                State.LastSelectionFailureReason = FarmMatchSelectionFailureReason.NotPlaying;
                return FarmMatchSelectionResult.Failure(FarmMatchSelectionFailureReason.NotPlaying);
            }

            if (!_board.TryGetCrop(position, out var cropType))
            {
                HandleFailedSelection(FarmMatchSelectionFailureReason.InvalidPosition);
                return FarmMatchSelectionResult.Failure(FarmMatchSelectionFailureReason.InvalidPosition);
            }

            if (cropType == FarmCropType.None)
            {
                HandleFailedSelection(FarmMatchSelectionFailureReason.EmptyCell);
                return FarmMatchSelectionResult.Failure(FarmMatchSelectionFailureReason.EmptyCell);
            }

            if (_selection.Contains(position))
            {
                HandleFailedSelection(FarmMatchSelectionFailureReason.DuplicateSelection);
                return FarmMatchSelectionResult.Failure(FarmMatchSelectionFailureReason.DuplicateSelection);
            }

            if (_selection.Count == 0)
            {
                _selection.Add(position);
                State.LastSelectionFailureReason = FarmMatchSelectionFailureReason.None;
                State.LastSelectionClearReason = FarmMatchSelectionClearReason.None;
                SyncSelectionState(cropType);
                RaiseStateChanged();
                return FarmMatchSelectionResult.Success(cropType, _selection.Count);
            }

            if (cropType != State.SelectedCropType)
            {
                HandleFailedSelection(FarmMatchSelectionFailureReason.DifferentCrop);
                return FarmMatchSelectionResult.Failure(FarmMatchSelectionFailureReason.DifferentCrop);
            }

            if (!TouchesLastSelection(position))
            {
                HandleFailedSelection(FarmMatchSelectionFailureReason.NotAdjacent);
                return FarmMatchSelectionResult.Failure(FarmMatchSelectionFailureReason.NotAdjacent);
            }

            _selection.Add(position);
            State.LastSelectionFailureReason = FarmMatchSelectionFailureReason.None;
            State.LastSelectionClearReason = FarmMatchSelectionClearReason.None;
            SyncSelectionState(cropType);
            RaiseStateChanged();

            return FarmMatchSelectionResult.Success(cropType, _selection.Count);
        }

        public void CancelSelection()
        {
            CancelSelection(FarmMatchSelectionClearReason.ClickedOutsideGrid);
        }

        public void CancelSelection(FarmMatchSelectionClearReason clearReason)
        {
            if (_selection.Count == 0)
            {
                State.LastSelectionClearReason = clearReason;
                return;
            }

            ClearSelection(clearReason);
            RaiseStateChanged();
        }

        public bool TryResolveSelection(out FarmMatchResolution resolution, out FarmMatchSelectionFailureReason failureReason)
        {
            resolution = null;
            failureReason = FarmMatchSelectionFailureReason.None;

            if (State.RoundState != FarmMatchRoundState.Playing)
            {
                failureReason = FarmMatchSelectionFailureReason.NotPlaying;
                State.LastSelectionFailureReason = failureReason;
                return false;
            }

            if (_selection.Count < _rules.MinimumMatchCount)
            {
                failureReason = FarmMatchSelectionFailureReason.TooFewCrops;
                State.LastSelectionFailureReason = failureReason;
                CancelSelection(FarmMatchSelectionClearReason.InvalidSelection);
                return false;
            }

            var matchedPositions = _selection.ToArray();
            var awardedScore = _scoreService.CalculateScore(matchedPositions.Length, _rules);

            _board.ReplaceMatchedCrops(matchedPositions);

            State.CurrentScore += awardedScore;
            if (State.CurrentScore > State.HighScore)
            {
                UpdateHighScore(State.CurrentScore);
            }
            State.LastSelectionFailureReason = FarmMatchSelectionFailureReason.None;
            State.LastSelectionClearReason = FarmMatchSelectionClearReason.SelectionResolved;
            State.LastAwardedScore = awardedScore;
            State.LastMatchedCropCount = matchedPositions.Length;
            State.LastMatchedCropType = State.SelectedCropType;

            resolution = new FarmMatchResolution(
                State.SelectedCropType,
                matchedPositions,
                awardedScore,
                State.CurrentScore);

            ClearSelection(FarmMatchSelectionClearReason.SelectionResolved);
            MatchResolved?.Invoke(resolution);

            if (State.TargetScore.HasValue && State.CurrentScore >= State.TargetScore.Value)
            {
                AdvanceRound();
                return true;
            }

            RaiseStateChanged();
            return true;
        }

        public void StopRoundEarly()
        {
            if (State.RoundState != FarmMatchRoundState.Playing)
            {
                return;
            }

            EndRound(FarmMatchRoundEndReason.StoppedEarly);
        }

        private bool TouchesLastSelection(GridPosition position)
        {
            if (_selection.Count == 0)
            {
                return false;
            }

            return _selection[_selection.Count - 1].IsOrthogonallyAdjacentTo(position);
        }

        private void EndRound(FarmMatchRoundEndReason endReason)
        {
            ClearSelection(FarmMatchSelectionClearReason.RoundEnded);

            State.RoundState = FarmMatchRoundState.Results;
            State.EndReason = endReason;
            State.FinalScore = State.CurrentScore;
            State.TimeRemainingSeconds = Math.Max(_timer.Current, 0f);
            if (State.FinalScore > State.HighScore)
            {
                UpdateHighScore(State.FinalScore);
            }

            RoundEnded?.Invoke(new FarmMatchRoundResult(
                State.EndReason,
                State.FinalScore,
                State.HighScore,
                State.RoundNumber,
                State.DidWin,
                State.DidLose));
            RaiseStateChanged();
        }

        private void AdvanceRound()
        {
            State.RoundNumber++;
            State.TargetScore = GetTargetScoreForRound(State.RoundNumber);
            State.TimeRemainingSeconds = _rules.RoundDurationSeconds;
            State.EndReason = FarmMatchRoundEndReason.None;
            State.LastSelectionFailureReason = FarmMatchSelectionFailureReason.None;
            State.LastSelectionClearReason = FarmMatchSelectionClearReason.RoundRestarted;
            State.LastAwardedScore = 0;
            State.LastMatchedCropCount = 0;
            State.LastMatchedCropType = FarmCropType.None;

            _board.ResetBoard();
            ClearSelection(FarmMatchSelectionClearReason.RoundRestarted);
            _timer.Start(_rules.RoundDurationSeconds);

            RoundAdvanced?.Invoke(new FarmMatchRoundProgress(State.RoundNumber, State.TargetScore));
            RaiseStateChanged();
        }

        private int? GetTargetScoreForRound(int roundNumber)
        {
            
            
            if (!_rules.TargetScore.HasValue)
            {
                return null;
            }

            var completedRounds = Math.Max(0, roundNumber - 1);
            MonoBehaviour.print("GetTargetScoreForRound: " + completedRounds);
            int? baseAmount = _rules.TargetScore.Value;
            
            if (State != null)
            {
                MonoBehaviour.print("State.TargetScore: " + State.TargetScore);
                baseAmount = State.TargetScore;
            }

            int? result = baseAmount + (completedRounds * _rules.TargetScoreIncreasePerRound);
            MonoBehaviour.print("result: " + result);
            
            return result;
        }

        private void HandleFailedSelection(FarmMatchSelectionFailureReason failureReason)
        {
            State.LastSelectionFailureReason = failureReason;
            CancelSelection(FarmMatchSelectionClearReason.InvalidSelection);
        }

        private void UpdateHighScore(int highScore)
        {
            _savedHighScore = highScore;
            State.HighScore = highScore;
            HighScoreChanged?.Invoke(highScore);
        }

        private void ClearSelection(FarmMatchSelectionClearReason clearReason)
        {
            _selection.Clear();
            State.LastSelectionClearReason = clearReason;
            SyncSelectionState(FarmCropType.None);
        }

        private void SyncSelectionState(FarmCropType cropType)
        {
            State.SelectedCropType = _selection.Count == 0 ? FarmCropType.None : cropType;
            State.SelectionCount = _selection.Count;
        }

        private void RaiseStateChanged()
        {
            StateChanged?.Invoke();
        }
    }
}
