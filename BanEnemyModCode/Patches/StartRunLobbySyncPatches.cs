using System.Collections.Generic;
using System.Linq;
using BanEnemyMod.BanEnemyModCode.Config;
using BanEnemyMod.BanEnemyModCode.Infrastructure;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Multiplayer.Game.Lobby;
using MegaCrit.Sts2.Core.Multiplayer.Messages.Lobby;

namespace BanEnemyMod.BanEnemyModCode.Patches;

[HarmonyPatch(typeof(StartRunLobby), "BeginRun")]
internal static class StartRunLobbyBeginRunCapturePatch
{
    private static void Prefix(string seed, List<ModifierModel> modifiers)
    {
        IReadOnlyList<ModifierModel> snapshot = modifiers?.ToList() ?? new List<ModifierModel>();
        BanEnemyConfigStore.CapturePendingRunModifiers(snapshot);
        HookTrace.Write($"StartRunLobby.BeginRun captured modifiers. seed={seed}, modifierCount={snapshot.Count}");
    }
}

[HarmonyPatch(typeof(StartRunLobby), "HandleLobbyBeginRunMessage")]
internal static class StartRunLobbyHandleBeginRunMessagePatch
{
    private static void Prefix(LobbyBeginRunMessage message, ulong senderId)
    {
        List<ModifierModel> modifiers = message.modifiers.Select(ModifierModel.FromSerializable).ToList();
        BanEnemyConfigStore.CapturePendingRunModifiers(modifiers);
        HookTrace.Write(
            $"HandleLobbyBeginRunMessage captured modifiers. sender={senderId}, modifierCount={modifiers.Count}");
    }
}
