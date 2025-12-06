using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.API.Util;

namespace QobPalisade.Source.Blocks;

public class PalisadeBlock : Block
{
    public override void OnLoaded(ICoreAPI api)
    {
        foreach (var drop in Drops)
        {
            drop.ResolvedItemstack.StackSize = (int)drop.Quantity.avg; // the game does not respect our avg
        }
    }
    public override void OnNeighbourBlockChange(IWorldAccessor world, BlockPos pos, BlockPos neibpos)
    {
        if (world.BlockAccessor.GetBlockBelow(pos).Id == 0)
        {
            world.BlockAccessor.BreakBlock(pos, null);
        }
    }
}
