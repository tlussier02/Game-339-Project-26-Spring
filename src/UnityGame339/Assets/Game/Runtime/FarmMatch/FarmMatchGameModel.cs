using System;
using System.Collections.Generic;
using Game339.Shared.Services;

namespace Game.Runtime.FarmMatch
{
    public sealed class FarmMatchGameModel
    {
        private readonly IFarmMatchBoard _board;
        private readonly IFarmMatchScoreService _scoreService;
        private readonly FarmMatchRules _rules;
        private readonly Timer _timer;
        private readonly List<GridPosition> _selection = new List<GridPosition>();

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
                TargetScore = _rules.TargetScore
            };
        }

        public event Action StateChanged;

        public event Action<FarmMatchResolution> MatchResolved;

        public event Action<FarmMatchRoundResult> RoundEnded;

        public FarmMatchGameState State { get; }

        public IReadOnlyList<GridPosition> CurrentSelection => _selection;

        public void StartNewRound()
        {
            _board.ResetBoard();
            ClearSelection(FarmMatchSelectionClearReason.RoundRestarted);

            State.RoundState = FarmMatchRoundState.Playing;
            State.EndReason = FarmMatchRoundEndReason.None;
            State.CurrentScore = 0;
            State.FinalScore = 0;
            State.TimeRemainingSeconds = _rules.RoundDurationSeconds;
            State.TargetScore = _rules.TargetScore;
            State.LastSelectionFailureReason = FarmMatchSelectionFailureReason.None;
            State.LastSelectionClearReason = FarmMatchSelectionClearReason.RoundRestarted;
            State.LastAwardedScore = 0;
            State.LastMatchedCropCount = 0;

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
            State.TimeRemainingSeconds = _rules.RoundDurationSeconds;
            State.LastSelectionFailureReason = FarmMatchSelectionFailureReason.None;
            State.LastSelectionClearReason = FarmMatchSelectionClearReason.RoundEnded;
            State.LastAwardedScore = 0;
            State.LastMatchedCropCount = 0;

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
                State.HighScore = State.CurrentScore;
            }
            State.LastSelectionFailureReason = FarmMatchSelectionFailureReason.None;
            State.LastSelectionClearReason = FarmMatchSelectionClearReason.SelectionResolved;
            State.LastAwardedScore = awardedScore;
            State.LastMatchedCropCount = matchedPositions.Length;

            resolution = new FarmMatchResolution(
                State.SelectedCropType,
                matchedPositions,
                awardedScore,
                State.CurrentScore);

            ClearSelection(FarmMatchSelectionClearReason.SelectionResolved);
            MatchResolved?.Invoke(resolution);

            if (State.TargetScore.HasValue && State.CurrentScore >= State.TargetScore.Value)
            {
                EndRound(FarmMatchRoundEndReason.TargetScoreReached);
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
                State.HighScore = State.FinalScore;
            }

            RoundEnded?.Invoke(new FarmMatchRoundResult(
                State.EndReason,
                State.FinalScore,
                State.HighScore,
                State.DidWin,
                State.DidLose));
            RaiseStateChanged();
        }

        private void HandleFailedSelection(FarmMatchSelectionFailureReason failureReason)
        {
            State.LastSelectionFailureReason = failureReason;
            CancelSelection(FarmMatchSelectionClearReason.InvalidSelection);
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
