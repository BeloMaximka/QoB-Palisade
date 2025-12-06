using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Util;

namespace QobPalisade.Source.Blocks;

internal class PalisadeTopRopedBlock : PalisadeBlock
{
    private Block? top;
    private Item? rope;

    public const string RopeCode = "flaxfibers";

    public override void OnLoaded(ICoreAPI api)
    {
        base.OnLoaded(api);

        string topBlockCode = Code.Path.Replace("toproped", "top");
        top = api.World.GetBlock(Code.Path.Replace("toproped", "top"));
        if (top is null)
        {
            api.Logger.Warning("[qobpalisade] Could not find block {0} in {1} class", topBlockCode, nameof(PalisadeTopRopedBlock));
        }

        rope = api.World.GetItem(RopeCode);
        if (rope is null)
        {
            api.Logger.Warning("[qobpalisade] Could not find item {0} in {1} class", RopeCode, nameof(PalisadeTopRopedBlock));
        }
    }

    public override bool OnBlockInteractStart(IWorldAccessor world, IPlayer byPlayer, BlockSelection blockSel)
    {
        if (
            rope is not null
            && top is not null
            && byPlayer.Entity.Controls.ShiftKey
            && world.Claims.TryAccess(byPlayer, blockSel.Position, EnumBlockAccessFlags.BuildOrBreak)
        )
        {
            world.BlockAccessor.SetBlock(top.Id, blockSel.Position);
            byPlayer.InventoryManager.TryGiveItemstack(new(rope), true);
            return true;
        }

        return false;
    }

    public override WorldInteraction[] GetPlacedBlockInteractionHelp(IWorldAccessor world, BlockSelection selection, IPlayer forPlayer)
    {
        string interactionCode = "interactionhelp-palisade-remove-rope";
        return ObjectCacheUtil.GetOrCreate<WorldInteraction[]>(
            world.Api,
            interactionCode,
            () =>
            {
                return
                [
                    new()
                    {
                        ActionLangCode = interactionCode,
                        MouseButton = EnumMouseButton.Right,
                        HotKeyCode = "shift",
                    },
                ];
            }
        );
    }
}
