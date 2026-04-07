using System.Collections.Generic;
using BanEnemyMod.BanEnemyModCode.Config;
using BanEnemyMod.BanEnemyModCode.Models;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Multiplayer.Game.Lobby;

namespace BanEnemyMod.BanEnemyModCode.Patches;

[HarmonyPatch(typeof(StartRunLobby), "BeginRun")]
internal static class StartRunLobbyBeginRunPatch
{
    private static void Prefix(ref List<ModifierModel> modifiers)
    {
        modifiers ??= new List<ModifierModel>();
        if (modifiers.Exists(modifier => modifier is BanEncounterSnapshotModifier))
        {
            return;
        }

        IReadOnlyList<ModifierModel> launchModifiers = BanEnemyConfigStore.MergeSnapshotModifier(modifiers);
        modifiers = new List<ModifierModel>(launchModifiers);
    }
}
