using HarmonyLib;
using QobPalisade.Source.Blocks;
using System.Linq;
using Vintagestory.API.Common;

namespace QobPalisade.Source.Systems;

public class MainSystem : ModSystem
{
    private Harmony HarmonyInstance => new(Mod.Info.ModID);

    public override void StartPre(ICoreAPI api)
    {
        if (!HarmonyInstance.GetPatchedMethods().Any())
        {
            HarmonyInstance.PatchAll();
        }
    }

    public override void Start(ICoreAPI api)
    {
        api.RegisterBlockClass(nameof(PalisadeBlock), typeof(PalisadeBlock));
        api.RegisterBlockClass(nameof(PalisadeLowerBlock), typeof(PalisadeLowerBlock));
        api.RegisterBlockClass(nameof(PalisadeLowerSpikedBlock), typeof(PalisadeLowerSpikedBlock));
        api.RegisterBlockClass(nameof(PalisadeTopBlock), typeof(PalisadeTopBlock));
        api.RegisterBlockClass(nameof(PalisadeTopRopedBlock), typeof(PalisadeTopRopedBlock));
        api.RegisterBlockClass(nameof(PalisadeStakesBlock), typeof(PalisadeStakesBlock));
    }

    public override void Dispose()
    {
        HarmonyInstance.UnpatchAll(HarmonyInstance.Id);
    }
}
