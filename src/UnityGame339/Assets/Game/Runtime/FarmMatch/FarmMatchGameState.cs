namespace Game.Runtime.FarmMatch
{
    public sealed class FarmMatchGameState
    {
        public FarmMatchRoundState RoundState { get; internal set; } = FarmMatchRoundState.Title;

        public FarmMatchRoundEndReason EndReason { get; internal set; } = FarmMatchRoundEndReason.None;

        public int CurrentScore { get; internal set; }

        public int FinalScore { get; internal set; }

        public int HighScore { get; internal set; }

        public float TimeRemainingSeconds { get; internal set; }

        public FarmCropType SelectedCropType { get; internal set; } = FarmCropType.None;

        public int SelectionCount { get; internal set; }

        public int? TargetScore { get; internal set; }

        public FarmMatchSelectionFailureReason LastSelectionFailureReason { get; internal set; }

        public FarmMatchSelectionClearReason LastSelectionClearReason { get; internal set; }

        public int LastAwardedScore { get; internal set; }

        public int LastMatchedCropCount { get; internal set; }

        public bool HasSelection => SelectionCount > 0;

        public bool HasWinCondition => TargetScore.HasValue;

        public bool DidWin => RoundState == FarmMatchRoundState.Results
            && EndReason == FarmMatchRoundEndReason.TargetScoreReached;

        public bool DidLose => RoundState == FarmMatchRoundState.Results
            && EndReason == FarmMatchRoundEndReason.TimerExpired;

        public bool ShowGameOverPanel => RoundState == FarmMatchRoundState.Results;
    }
}
