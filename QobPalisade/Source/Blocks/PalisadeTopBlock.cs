using System.Linq;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Util;

namespace QobPalisade.Source.Blocks;

public class PalisadeTopBlock : Block
{
    private Block? halfTop;
    private Block? topRoped;

    public override void OnLoaded(ICoreAPI api)
    {
        halfTop = api.World.GetBlock(Code.Path.Replace("top", "halftop"));
        topRoped = api.World.GetBlock(Code.Path.Replace("top", "toproped"));
    }

    public override bool OnBlockInteractStart(IWorldAccessor world, IPlayer byPlayer, BlockSelection blockSel)
    {
        if (!byPlayer.Entity.Controls.ShiftKey)
        {
            return false;
        }

        if (halfTop is not null && byPlayer.InventoryManager.ActiveTool == EnumTool.Axe)
        {
            world.PlaySoundAt(PalisadeLowerBlock.SharpenSound, blockSel.Position, 0, byPlayer);
            world.BlockAccessor.SetBlock(halfTop.Id, blockSel.Position);
            return true;
        }

        if (
            topRoped is not null
            && byPlayer.InventoryManager.ActiveHotbarSlot?.Itemstack?.Item?.Code?.Path == PalisadeTopRopedBlock.RopeCode
        )
        {
            byPlayer.InventoryManager.ActiveHotbarSlot.TakeOut(1);
            world.BlockAccessor.SetBlock(topRoped.Id, blockSel.Position);
            return true;
        }

        return false;
    }

    public override WorldInteraction[] GetPlacedBlockInteractionHelp(IWorldAccessor world, BlockSelection selection, IPlayer forPlayer)
    {
        return ObjectCacheUtil.GetOrCreate<WorldInteraction[]>(
            world.Api,
            PalisadeLowerBlock.SharpenInteractionCode,
            () =>
            {
                ItemStack[] axes = [.. world.SearchItems("axe*").Select(item => new ItemStack(item))];
                return
                [
                    new()
                    {
                        Itemstacks = axes,
                        ActionLangCode = PalisadeLowerBlock.SharpenInteractionCode,
                        MouseButton = EnumMouseButton.Right,
                        HotKeyCode = "shift",
                    },
                ];
            }
        );
    }
}
