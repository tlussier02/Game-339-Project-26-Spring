using System;
using System.Collections.Generic;

namespace Game.Runtime.FarmMatch
{
    public enum FarmCropType
    {
        None = 0,
        Apple = 1,
        Grape = 2,
        Cherry = 3,
        Kiwi = 4,
        Orange = 5,
        Watermelon = 6
    }

    public enum FarmMatchRoundState
    {
        Title,
        Playing,
        Results
    }

    public enum FarmMatchRoundEndReason
    {
        None,
        TimerExpired,
        TargetScoreReached,
        StoppedEarly
    }

    public enum FarmMatchSelectionFailureReason
    {
        None,
        NotPlaying,
        InvalidPosition,
        EmptyCell,
        DifferentCrop,
        NotAdjacent,
        DuplicateSelection,
        TooFewCrops
    }

    public enum FarmMatchSelectionClearReason
    {
        None,
        InvalidSelection,
        ClickedOutsideGrid,
        RoundRestarted,
        RoundEnded,
        SelectionResolved
    }

    public readonly struct GridPosition : IEquatable<GridPosition>
    {
        public GridPosition(int row, int column)
        {
            Row = row;
            Column = column;
        }

        public int Row { get; }

        public int Column { get; }

        public bool IsOrthogonallyAdjacentTo(GridPosition other)
        {
            var rowDistance = Math.Abs(Row - other.Row);
            var columnDistance = Math.Abs(Column - other.Column);

            return rowDistance + columnDistance == 1;
        }

        public bool Equals(GridPosition other)
        {
            return Row == other.Row && Column == other.Column;
        }

        public override bool Equals(object obj)
        {
            return obj is GridPosition other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Row * 397) ^ Column;
            }
        }

        public override string ToString()
        {
            return "(" + Row + ", " + Column + ")";
        }
    }

    public readonly struct FarmMatchSelectionResult
    {
        private FarmMatchSelectionResult(
            bool succeeded,
            FarmMatchSelectionFailureReason failureReason,
            FarmCropType cropType,
            int selectionCount)
        {
            Succeeded = succeeded;
            FailureReason = failureReason;
            CropType = cropType;
            SelectionCount = selectionCount;
        }

        public bool Succeeded { get; }

        public FarmMatchSelectionFailureReason FailureReason { get; }

        public FarmCropType CropType { get; }

        public int SelectionCount { get; }

        public static FarmMatchSelectionResult Success(FarmCropType cropType, int selectionCount)
        {
            return new FarmMatchSelectionResult(true, FarmMatchSelectionFailureReason.None, cropType, selectionCount);
        }

        public static FarmMatchSelectionResult Failure(FarmMatchSelectionFailureReason failureReason)
        {
            return new FarmMatchSelectionResult(false, failureReason, FarmCropType.None, 0);
        }
    }

    public sealed class FarmMatchResolution
    {
        public FarmMatchResolution(
            FarmCropType cropType,
            IReadOnlyList<GridPosition> matchedPositions,
            int awardedScore,
            int totalScore)
        {
            CropType = cropType;
            MatchedPositions = matchedPositions ?? throw new ArgumentNullException(nameof(matchedPositions));
            AwardedScore = awardedScore;
            TotalScore = totalScore;
        }

        public FarmCropType CropType { get; }

        public IReadOnlyList<GridPosition> MatchedPositions { get; }

        public int AwardedScore { get; }

        public int TotalScore { get; }

        public int MatchCount => MatchedPositions.Count;
    }

    public sealed class FarmMatchRoundResult
    {
        public FarmMatchRoundResult(
            FarmMatchRoundEndReason endReason,
            int finalScore,
            int highScore,
            bool didWin,
            bool didLose)
        {
            EndReason = endReason;
            FinalScore = finalScore;
            HighScore = highScore;
            DidWin = didWin;
            DidLose = didLose;
        }

        public FarmMatchRoundEndReason EndReason { get; }

        public int FinalScore { get; }

        public int HighScore { get; }

        public bool DidWin { get; }

        public bool DidLose { get; }
    }
}
