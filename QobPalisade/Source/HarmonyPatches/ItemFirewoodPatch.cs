using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Vintagestory.API.Common;
using Vintagestory.GameContent;

namespace QualityOfBuilding.Source.HarmonyPatches;

[HarmonyPatch]
public static class ItemFirewoodPatch
{
    [HarmonyTranspiler]
    [HarmonyPatch(typeof(ItemFirewood), nameof(ItemFirewood.OnHeldInteractStart))]
    private static IEnumerable<CodeInstruction> SkipCheckForHoldingShift(IEnumerable<CodeInstruction> instructions)
    {
        MethodInfo controlsGetter = AccessTools.PropertyGetter(typeof(EntityAgent), nameof(EntityAgent.Controls));
        return new CodeMatcher(instructions)
            .MatchStartForward(CodeMatch.Calls(controlsGetter))
            .ThrowIfNotMatchForward("Could not find byEntity.Controls")
            .Advance(-1)
            .RemoveInstructions(3)
            .Insert(new CodeInstruction(OpCodes.Ldc_I4_1))
            .InstructionEnumeration();
    }
}
