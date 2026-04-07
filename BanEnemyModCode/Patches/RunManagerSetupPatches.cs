using BanEnemyMod.BanEnemyModCode.Config;
using HarmonyLib;
using MegaCrit.Sts2.Core.Multiplayer.Game.Lobby;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves;

namespace BanEnemyMod.BanEnemyModCode.Patches;

[HarmonyPatch(typeof(RunManager), nameof(RunManager.SetUpNewSinglePlayer))]
internal static class RunManagerSetUpNewSinglePlayerPatch
{
    private static void Postfix(RunState state)
    {
        BanEnemyConfigStore.TryActivateRunSnapshot(state);
    }
}

[HarmonyPatch(typeof(RunManager), nameof(RunManager.SetUpNewMultiPlayer))]
internal static class RunManagerSetUpNewMultiPlayerPatch
{
    private static void Postfix(RunState state, StartRunLobby lobby)
    {
        BanEnemyConfigStore.TryActivateRunSnapshot(state);
    }
}

[HarmonyPatch(typeof(RunManager), nameof(RunManager.SetUpSavedSinglePlayer))]
internal static class RunManagerSetUpSavedSinglePlayerPatch
{
    private static void Postfix(RunState state, SerializableRun save)
    {
        BanEnemyConfigStore.TryActivateRunSnapshot(state);
    }
}

[HarmonyPatch(typeof(RunManager), nameof(RunManager.SetUpSavedMultiPlayer))]
internal static class RunManagerSetUpSavedMultiPlayerPatch
{
    private static void Postfix(RunState state, LoadRunLobby lobby)
    {
        BanEnemyConfigStore.TryActivateRunSnapshot(state);
    }
}
