namespace Game.Runtime.FarmMatch
{
    public interface IFarmMatchScoreService
    {
        int CalculateScore(int matchedCropCount, FarmMatchRules rules);
    }
}
