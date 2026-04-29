using System;

namespace Game.Runtime.FarmMatch
{
    public sealed class FarmMatchScreenViewModel
    {
        private readonly FarmMatchGameModel _model;

        public FarmMatchScreenViewModel(FarmMatchGameModel model)
        {
            _model = model ?? throw new ArgumentNullException(nameof(model));
        }

        public FarmMatchRoundState RoundState => _model.State.RoundState;

        public bool ShowTitle => RoundState == FarmMatchRoundState.Title;

        public bool ShowGameplay => RoundState == FarmMatchRoundState.Playing;

        public bool ShowGameOverPanel => _model.State.ShowGameOverPanel;

        public bool CanSelectCells => RoundState == FarmMatchRoundState.Playing;

        public bool CanRestart => RoundState == FarmMatchRoundState.Results;

        public string ScoreLabel => "Score: " + _model.State.CurrentScore;

        public string HighScoreLabel => "High Score: " + _model.State.HighScore;

        public string TimerLabel => "Time: " + Math.Max(0, (int)Math.Ceiling(_model.State.TimeRemainingSeconds));

        public string SelectionCountLabel => "Selected: " + _model.State.SelectionCount;

        public string GameOverHeaderLabel
        {
            get
            {
                if (_model.State.DidWin)
                {
                    return "You Win";
                }

                if (_model.State.DidLose)
                {
                    return "Game Over";
                }

                return "Round Complete";
            }
        }

        public string GameOverCurrentScoreLabel => "Current Score: " + _model.State.FinalScore;

        public string GameOverHighScoreLabel => "High Score: " + _model.State.HighScore;

        public string RestartButtonLabel => "Restart";

        public string StatusLabel
        {
            get
            {
                if (_model.State.ShowGameOverPanel)
                {
                    if (_model.State.DidWin)
                    {
                        return "Target score reached before time expired.";
                    }

                    if (_model.State.DidLose)
                    {
                        return _model.State.HasWinCondition
                            ? "Time ran out before the target score was reached."
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
                    return "+" + _model.State.LastAwardedScore + " points for matching " + _model.State.LastMatchedCropCount + ".";
                }

                if (_model.State.LastSelectionClearReason == FarmMatchSelectionClearReason.ClickedOutsideGrid)
                {
                    return "Selection canceled.";
                }

                if (_model.State.SelectionCount > 0)
                {
                    return "Select neighboring matching crops.";
                }

                return "Build a chain of 3 or more matching neighboring crops.";
            }
        }
    }
}
