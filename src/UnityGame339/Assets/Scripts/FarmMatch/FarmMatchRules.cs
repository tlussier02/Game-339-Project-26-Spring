using System;

namespace Game.Runtime.FarmMatch
{
    public sealed class FarmMatchRules
    {
        public int GridSize { get; set; } = 9;

        public int MinimumMatchCount { get; set; } = 3;

        public int BaseMatchScore { get; set; } = 100;

        public int ExtraCropMultiplierStep { get; set; } = 1;

        public float RoundDurationSeconds { get; set; } = 180f;

        public int? TargetScore { get; set; }

        public int TargetScoreIncreasePerRound { get; set; } = 500;

        public void Validate()
        {
            if (GridSize <= 0)
            {
                throw new InvalidOperationException("GridSize must be greater than zero.");
            }

            if (MinimumMatchCount < 3)
            {
                throw new InvalidOperationException("MinimumMatchCount must be at least 3.");
            }

            if (BaseMatchScore <= 0)
            {
                throw new InvalidOperationException("BaseMatchScore must be greater than zero.");
            }

            if (ExtraCropMultiplierStep < 0)
            {
                throw new InvalidOperationException("ExtraCropMultiplierStep cannot be negative.");
            }

            if (RoundDurationSeconds <= 0f)
            {
                throw new InvalidOperationException("RoundDurationSeconds must be greater than zero.");
            }

            if (TargetScore.HasValue && TargetScore.Value <= 0)
            {
                throw new InvalidOperationException("TargetScore must be greater than zero when it is set.");
            }

            if (TargetScoreIncreasePerRound < 0)
            {
                throw new InvalidOperationException("TargetScoreIncreasePerRound cannot be negative.");
            }
        }
    }
}
