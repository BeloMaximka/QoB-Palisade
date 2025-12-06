using QobPalisade.Source.Utils;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.API.Util;

namespace QobPalisade.Source.Blocks;

public class PalisadeStakesBlock : PalisadeBlock
{
    public override bool CanPlaceBlock(IWorldAccessor world, IPlayer byPlayer, BlockSelection blockSel, ref string failureCode)
    {
        if (CanAttachSpikes(world, byPlayer, blockSel))
        {
            return true;
        }

        return base.CanPlaceBlock(world, byPlayer, blockSel, ref failureCode);
    }

    public override bool DoPlaceBlock(IWorldAccessor world, IPlayer byPlayer, BlockSelection blockSel, ItemStack byItemStack)
    {
        if (CanAttachSpikes(world, byPlayer, blockSel))
        {
            blockSel.Position.OffsetOpposite(blockSel.Face);
            return ReplacePalisadeLowerWithSpikedVersion(world, blockSel.Position);
        }

        return base.DoPlaceBlock(world, byPlayer, blockSel, byItemStack);
    }

    public override bool TryPlaceBlock(
        IWorldAccessor world,
        IPlayer byPlayer,
        ItemStack itemstack,
        BlockSelection blockSel,
        ref string failureCode
    )
    {
        if (CanAttachSpikes(world, byPlayer, blockSel))
        {
            blockSel.Position.OffsetOpposite(blockSel.Face);
            return ReplacePalisadeLowerWithSpikedVersion(world, blockSel.Position);
        }

        return base.TryPlaceBlock(world, byPlayer, itemstack, blockSel, ref failureCode);
    }

    private bool ReplacePalisadeLowerWithSpikedVersion(IWorldAccessor world, BlockPos pos)
    {
        string orientation = world.BlockAccessor.GetBlock(pos).Code.EndVariant();
        string palisadeLowerSpikedCode = $"palisadewall-four-lowerspiked-wall-{orientation}";
        Block? block = world.GetBlock(palisadeLowerSpikedCode);
        if (block is not null)
        {
            world.BlockAccessor.SetBlock(block.Id, pos);
            return true;
        }
        else
        {
            api.Logger.Warning("[qobpalisade] Could not find block {0} in {1} class", palisadeLowerSpikedCode, nameof(PalisadeStakesBlock));
            return true;
        }
    }

    private static bool CanAttachSpikes(IWorldAccessor world, IPlayer byPlayer, BlockSelection blockSel)
    {
        if (byPlayer.Entity.Controls.ShiftKey)
        {
            return false;
        }

        blockSel.Position.OffsetOpposite(blockSel.Face);
        bool canAttachSpikes =
            !world.BlockAccessor.GetBlockBelow(blockSel.Position).Code.Path.StartsWithOrdinal("palisadewall")
            && world.BlockAccessor.GetBlock(blockSel.Position).Code.Path.StartsWithOrdinal("palisadewall-four-lower-wall");
        blockSel.Position.Offset(blockSel.Face);
        return canAttachSpikes;
    }
}
