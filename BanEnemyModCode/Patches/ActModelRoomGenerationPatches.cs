using BanEnemyMod.BanEnemyModCode.Infrastructure;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.Unlocks;

namespace BanEnemyMod.BanEnemyModCode.Patches;

[HarmonyPatch(typeof(ActModel), nameof(ActModel.GenerateRooms))]
internal static class ActModelGenerateRoomsPatch
{
    private static void Postfix(ActModel __instance, Rng rng, UnlockState unlockState, bool isMultiplayer)
    {
        ActEncounterFilter.ApplyToAct(__instance);
    }
}

[HarmonyPatch(typeof(ActModel), nameof(ActModel.ValidateRoomsAfterLoad))]
internal static class ActModelValidateRoomsAfterLoadPatch
{
    private static void Postfix(ActModel __instance, Rng rng)
    {
        ActEncounterFilter.ApplyToAct(__instance);
    }
}

[HarmonyPatch(typeof(ActModel), nameof(ActModel.ApplyDiscoveryOrderModifications))]
internal static class ActModelApplyDiscoveryOrderModificationsPatch
{
    private static void Postfix(ActModel __instance, UnlockState unlockState)
    {
        ActEncounterFilter.ApplyToAct(__instance);
    }
}
