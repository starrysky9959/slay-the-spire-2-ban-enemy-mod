using BanEnemyMod.BanEnemyModCode.Infrastructure;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;

namespace BanEnemyMod.BanEnemyModCode.Patches;

[HarmonyPatch(typeof(CombatManager), nameof(CombatManager.SetUpCombat))]
internal static class CombatManagerSetUpCombatPatch
{
    private static void Prefix(CombatState state)
    {
        string encounterId = state.Encounter?.Id.ToString() ?? "<null>";
        string encounterType = state.Encounter?.GetType().FullName ?? "<null>";

        HookTrace.Write(
            $"SetUpCombat hook fired. encounterId={encounterId}, encounterType={encounterType}, enemyCount={state.Enemies.Count}, round={state.RoundNumber}");
    }
}
