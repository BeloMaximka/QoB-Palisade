using System.Linq;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Util;

namespace QobPalisade.Source.Blocks;

public class PalisadeLowerBlock : PalisadeBlock
{
    private const int MaxHeight = 2;
    public const string SharpenSound = "sounds/block/chop";
    public const string SharpenInteractionCode = "interactionhelp-palisade-sharpen";
    private Block? topBlock;

    public override void OnLoaded(ICoreAPI api)
    {
        base.OnLoaded(api);

        topBlock = api.World.GetBlock(Code.Path.Replace("lower", "top"));
    }

    public override bool OnBlockInteractStart(IWorldAccessor world, IPlayer byPlayer, BlockSelection blockSel)
    {
        if (
            topBlock is not null
            && byPlayer.Entity.Controls.ShiftKey
            && byPlayer.InventoryManager.ActiveTool == EnumTool.Axe
            && !world.BlockAccessor.GetBlockAbove(blockSel.Position).Code.Path.StartsWithOrdinal("palisade")
        )
        {
            world.PlaySoundAt(SharpenSound, blockSel.Position, 0, byPlayer);
            world.BlockAccessor.SetBlock(topBlock.Id, blockSel.Position);
            return true;
        }

        return false;
    }

    public override WorldInteraction[] GetPlacedBlockInteractionHelp(IWorldAccessor world, BlockSelection selection, IPlayer forPlayer)
    {
        if (world.BlockAccessor.GetBlockAbove(selection.Position).Code.Path.StartsWithOrdinal("palisade"))
        {
            return [];
        }

        return ObjectCacheUtil.GetOrCreate<WorldInteraction[]>(
            world.Api,
            SharpenInteractionCode,
            () =>
            {
                ItemStack[] axes = [.. world.SearchItems("axe*").Select(item => new ItemStack(item))];
                return
                [
                    new()
                    {
                        Itemstacks = axes,
                        ActionLangCode = SharpenInteractionCode,
                        MouseButton = EnumMouseButton.Right,
                        HotKeyCode = "shift",
                    },
                ];
            }
        );
    }

    public override bool CanPlaceBlock(IWorldAccessor world, IPlayer byPlayer, BlockSelection blockSel, ref string failureCode)
    {
        BlockSelection? newSel = GetOffsetSel(world, byPlayer, blockSel, ref failureCode);
        if (newSel is null)
        {
            return false;
        }

        return base.CanPlaceBlock(world, byPlayer, newSel, ref failureCode);
    }

    public override bool DoPlaceBlock(IWorldAccessor world, IPlayer byPlayer, BlockSelection blockSel, ItemStack byItemStack)
    {
        Block blockBelow = world.BlockAccessor.GetBlockBelow(blockSel.Position);
        if (blockBelow.Class == Class)
        {
            world.BlockAccessor.SetBlock(blockBelow.Id, blockSel.Position);
            return true;
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
        BlockSelection? newSel = GetOffsetSel(world, byPlayer, blockSel, ref failureCode);
        if (newSel is null)
        {
            return false;
        }

        return base.TryPlaceBlock(world, byPlayer, itemstack, newSel, ref failureCode);
    }

    private BlockSelection? GetOffsetSel(IWorldAccessor world, IPlayer byPlayer, BlockSelection blockSel, ref string failureCode)
    {
        if (byPlayer.Entity.Controls.ShiftKey)
        {
            return blockSel;
        }

        BlockSelection selCopy = blockSel.Clone();
        selCopy.Position.X -= selCopy.Face.Normali.X;
        selCopy.Position.Y -= selCopy.Face.Normali.Y;
        selCopy.Position.Z -= selCopy.Face.Normali.Z;
        if (world.BlockAccessor.GetBlock(selCopy.Position).Class != Class)
        {
            return blockSel;
        }

        for (int i = 0; i < MaxHeight; i++)
        {
            selCopy.Position.Up();
            if (world.BlockAccessor.GetBlock(selCopy.Position).Id == 0)
            {
                return selCopy;
            }
        }

        failureCode = "cannot-stack-more";
        return null;
    }
}
