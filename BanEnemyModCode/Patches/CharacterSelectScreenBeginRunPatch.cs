using System.Collections.Generic;
using System.Linq;
using BanEnemyMod.BanEnemyModCode.Config;
using BanEnemyMod.BanEnemyModCode.Models;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Screens.CharacterSelect;

namespace BanEnemyMod.BanEnemyModCode.Patches;

[HarmonyPatch(typeof(NCharacterSelectScreen), nameof(NCharacterSelectScreen.BeginRun))]
internal static class CharacterSelectScreenBeginRunPatch
{
    private static void Prefix(ref IReadOnlyList<ModifierModel> modifiers)
    {
        BanEnemyConfigStore.CapturePendingRunModifiers(modifiers.ToList());
        modifiers = modifiers.Where(modifier => modifier is not BanEncounterSnapshotModifier).ToList();
    }
}
