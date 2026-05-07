using System.Collections.Generic;

namespace Game.Runtime.FarmMatch
{
    public interface IFarmMatchBoard
    {
        bool TryGetCrop(GridPosition position, out FarmCropType cropType);

        void ReplaceMatchedCrops(IReadOnlyList<GridPosition> matchedPositions);

        void ResetBoard();
    }
}
