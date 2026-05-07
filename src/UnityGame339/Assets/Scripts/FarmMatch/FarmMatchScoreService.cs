namespace Game.Runtime.FarmMatch
{
    public sealed class FarmMatchScoreService : IFarmMatchScoreService
    {
        public int CalculateScore(int matchedCropCount, FarmMatchRules rules)
        {
            if (rules == null)
            {
                throw new System.ArgumentNullException(nameof(rules));
            }

            if (matchedCropCount < rules.MinimumMatchCount)
            {
                return 0;
            }

            var extraCropCount = matchedCropCount - rules.MinimumMatchCount;
            var multiplier = 1 + (extraCropCount * rules.ExtraCropMultiplierStep);

            return rules.BaseMatchScore * multiplier;
        }
    }
}
