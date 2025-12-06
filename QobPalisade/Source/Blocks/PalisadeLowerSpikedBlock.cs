using System.Linq;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Util;

namespace QobPalisade.Source.Blocks;

public class PalisadeLowerSpikedBlock : PalisadeBlock
{
    private Block? lowerBlock;
    private Item? firewood;

    public override void OnLoaded(ICoreAPI api)
    {
        base.OnLoaded(api);

        firewood = api.World.GetItem("firewood");
        lowerBlock = api.World.GetBlock(Code.Path.Replace("lowerspiked", "lower"));
    }

    public override bool OnBlockInteractStart(IWorldAccessor world, IPlayer byPlayer, BlockSelection blockSel)
    {
        if (
            firewood is not null
            && lowerBlock is not null
            && byPlayer.Entity.Controls.ShiftKey
            && byPlayer.InventoryManager.ActiveTool == EnumTool.Axe
        )
        {
            world.PlaySoundAt(PalisadeLowerBlock.SharpenSound, blockSel.Position, 0, byPlayer);
            ItemStack firewoodStack = new(firewood, 4);
            world.SpawnItemEntity(firewoodStack, blockSel.Position);
            world.SpawnItemEntity(firewoodStack, blockSel.Position);
            world.SpawnItemEntity(firewoodStack, blockSel.Position);
            world.SpawnItemEntity(firewoodStack, blockSel.Position);
            world.BlockAccessor.SetBlock(lowerBlock.Id, blockSel.Position);
            return true;
        }

        return false;
    }

    public override WorldInteraction[] GetPlacedBlockInteractionHelp(IWorldAccessor world, BlockSelection selection, IPlayer forPlayer)
    {
        string interactionCode = "interactionhelp-palisade-remove-stakes";
        return ObjectCacheUtil.GetOrCreate<WorldInteraction[]>(
            world.Api,
            interactionCode,
            () =>
            {
                ItemStack[] axes = [.. world.SearchItems("axe*").Select(item => new ItemStack(item))];
                return
                [
                    new()
                    {
                        Itemstacks = axes,
                        ActionLangCode = interactionCode,
                        MouseButton = EnumMouseButton.Right,
                        HotKeyCode = "shift",
                    },
                ];
            }
        );
    }

}
