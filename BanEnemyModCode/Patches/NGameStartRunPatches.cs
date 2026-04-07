using System.Collections.Generic;
using BanEnemyMod.BanEnemyModCode.Config;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Multiplayer.Game.Lobby;
using MegaCrit.Sts2.Core.Nodes;

namespace BanEnemyMod.BanEnemyModCode.Patches;

[HarmonyPatch(typeof(NGame), nameof(NGame.StartNewSingleplayerRun))]
internal static class NGameStartNewSingleplayerRunPatch
{
    private static void Prefix(ref IReadOnlyList<ModifierModel> modifiers)
    {
        modifiers = BanEnemyConfigStore.GetModifiersForRunCreation(modifiers);
    }
}

[HarmonyPatch(typeof(NGame), nameof(NGame.StartNewMultiplayerRun))]
internal static class NGameStartNewMultiplayerRunPatch
{
    private static void Prefix(StartRunLobby lobby, ref IReadOnlyList<ModifierModel> modifiers)
    {
        modifiers = BanEnemyConfigStore.GetModifiersForRunCreation(modifiers);
    }
}
