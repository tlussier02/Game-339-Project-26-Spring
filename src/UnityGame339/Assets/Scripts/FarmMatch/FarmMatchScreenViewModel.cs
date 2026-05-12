using System;

namespace Game.Runtime.FarmMatch
{
    public sealed class FarmMatchScreenViewModel
    {
        private readonly FarmMatchGameModel _model;

        public FarmMatchScreenViewModel(FarmMatchGameModel model)
        {
            _model = model ?? throw new ArgumentNullException(nameof(model));
            _model.StateChanged += HandleStateChanged;
            _model.MatchResolved += HandleMatchResolved;
            _model.RoundAdvanced += HandleRoundAdvanced;
            _model.RoundEnded += HandleRoundEnded;
        }

        public event Action ViewChanged;

        public event Action<FarmMatchResolution> MatchResolved;

        public event Action<FarmMatchRoundProgress> RoundAdvanced;

        public event Action<FarmMatchRoundResult> RoundEnded;

        public FarmMatchRoundState RoundState => _model.State.RoundState;

        public bool ShowTitle => RoundState == FarmMatchRoundState.Title;

        public bool ShowGameplay => RoundState == FarmMatchRoundState.Playing;

        public bool ShowGameOverPanel => _model.State.ShowGameOverPanel;

        public bool CanSelectCells => RoundState == FarmMatchRoundState.Playing;

        public bool CanRestart => RoundState == FarmMatchRoundState.Results;

        public string ScoreLabel => "Score: " + _model.State.CurrentScore;

        public string ScoreHudLabel => "SCORE: " + _model.State.CurrentScore.ToString("0000");

        public string HighScoreLabel => "High Score: " + _model.State.HighScore;

        public string RoundLabel => "Round: " + _model.State.RoundNumber;

        public string TimerLabel => "Time: " + Math.Max(0, (int)Math.Ceiling(_model.State.TimeRemainingSeconds));

        public string TimerHudLabel => "TIME: " + Math.Max(0, (int)Math.Ceiling(_model.State.TimeRemainingSeconds)).ToString("000");

        public string GoalHudLabel
        {
            get
            {
                if (_model.State.RoundState == FarmMatchRoundState.Results)
                {
                    return "FINAL: " + _model.State.FinalScore.ToString("0000");
                }

                if (_model.State.TargetScore.HasValue)
                {
                    return "GOAL: " + _model.State.TargetScore.Value.ToString("0000");
                }

                return "HIGH: " + _model.State.HighScore.ToString("0000");
            }
        }

        public string SelectionCountLabel => "Selected: " + _model.State.SelectionCount;

        public string GameOverHeaderLabel
        {
            get
            {
                if (_model.State.DidLose)
                {
                    return "Game Over";
                }

                return "Round Complete";
            }
        }

        public string GameOverCurrentScoreLabel => "Current Score: " + _model.State.FinalScore;

        public string GameOverHighScoreLabel => "High Score: " + _model.State.HighScore;

        public string GameOverRoundLabel => "Rounds Cleared: " + Math.Max(0, _model.State.RoundNumber - 1);

        public string RestartButtonLabel => "Restart";

        public string StatusLabel
        {
            get
            {
                if (_model.State.ShowGameOverPanel)
                {
                    if (_model.State.DidLose)
                    {
                        return _model.State.HasWinCondition
                            ? "Time ran out before the round goal was reached."
                            : "Time ran out.";
                    }

                    return "Round ended.";
                }

                switch (_model.State.LastSelectionFailureReason)
                {
                    case FarmMatchSelectionFailureReason.DifferentCrop:
                        return "Selection canceled: crops must match.";
                    case FarmMatchSelectionFailureReason.NotAdjacent:
                        return "Selection canceled: crops must be neighboring cells.";
                    case FarmMatchSelectionFailureReason.DuplicateSelection:
                        return "Selection canceled: clicking the same crop clears the selection.";
                    case FarmMatchSelectionFailureReason.TooFewCrops:
                        return "Need at least 3 matching crops.";
                    case FarmMatchSelectionFailureReason.EmptyCell:
                    case FarmMatchSelectionFailureReason.InvalidPosition:
                        return "Selection canceled.";
                    case FarmMatchSelectionFailureReason.NotPlaying:
                        return "The round is not currently active.";
                }

                if (_model.State.LastAwardedScore > 0 && _model.State.LastMatchedCropCount > 0)
                {
                    return "+" + _model.State.LastAwardedScore
                        + " points for matching "
                        + _model.State.LastMatchedCropCount
                        + " "
                        + FormatCropName(_model.State.LastMatchedCropType)
                        + ".";
                }

                if (_model.State.LastSelectionClearReason == FarmMatchSelectionClearReason.ClickedOutsideGrid)
                {
                    return "Selection canceled.";
                }

                if (_model.State.LastSelectionClearReason == FarmMatchSelectionClearReason.RoundRestarted
                    && _model.State.RoundState == FarmMatchRoundState.Playing
                    && _model.State.RoundNumber > 1)
                {
                    return GoalHudLabel + " for " + RoundLabel + ".";
                }

                if (_model.State.SelectionCount > 0)
                {
                    return "Select neighboring matching crops.";
                }

                return "Build a chain of 3 or more matching neighboring crops.";
            }
        }

        public void Dispose()
        {
            _model.StateChanged -= HandleStateChanged;
            _model.MatchResolved -= HandleMatchResolved;
            _model.RoundAdvanced -= HandleRoundAdvanced;
            _model.RoundEnded -= HandleRoundEnded;
        }

        private void HandleStateChanged()
        {
            ViewChanged?.Invoke();
        }

        private void HandleMatchResolved(FarmMatchResolution resolution)
        {
            MatchResolved?.Invoke(resolution);
        }

        private void HandleRoundAdvanced(FarmMatchRoundProgress progress)
        {
            RoundAdvanced?.Invoke(progress);
        }

        private void HandleRoundEnded(FarmMatchRoundResult result)
        {
            RoundEnded?.Invoke(result);
        }

        private static string FormatCropName(FarmCropType cropType)
        {
            return cropType == FarmCropType.None ? "crops" : cropType.ToString().ToLowerInvariant() + "s";
        }
    }
}
