using Vintagestory.API.MathTools;

namespace QobPalisade.Source.Utils;

public static class BlockPosExtension
{
    public static BlockPos OffsetOpposite(this BlockPos pos, BlockFacing face)
    {
        pos.X -= face.Normali.X;
        pos.Y -= face.Normali.Y;
        pos.Z -= face.Normali.Z;
        return pos;
    }
}
