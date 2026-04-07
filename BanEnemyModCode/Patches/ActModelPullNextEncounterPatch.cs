using System;
using System.Collections.Generic;
using System.Linq;
using BanEnemyMod.BanEnemyModCode.Config;
using BanEnemyMod.BanEnemyModCode.Infrastructure;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;

namespace BanEnemyMod.BanEnemyModCode.Patches;

[HarmonyPatch(typeof(ActModel), nameof(ActModel.PullNextEncounter))]
internal static class ActModelPullNextEncounterPatch
{
    private static void Postfix(ActModel __instance, RoomType roomType, ref EncounterModel __result)
    {
        string encounterId = __result.Id.ToString();
        if (BanEnemyConfigStore.IsEncounterAllowed(encounterId))
        {
            HookTrace.Write(
                $"PullNextEncounter hook fired. roomType={roomType}, encounterId={encounterId}, allowed=true");
            return;
        }

        IReadOnlyList<EncounterModel> candidates = GetCandidates(__instance, roomType);
        List<EncounterModel> allowedCandidates = candidates
            .Where(encounter => BanEnemyConfigStore.IsEncounterAllowed(encounter.Id.ToString()))
            .ToList();

        if (allowedCandidates.Count == 0)
        {
            HookTrace.Write(
                $"PullNextEncounter ban fallback. roomType={roomType}, encounterId={encounterId}, reason=no_allowed_candidates");
            return;
        }

        EncounterModel replacement = SelectReplacement(candidates, allowedCandidates, __result);
        __result = replacement;
        HookTrace.Write(
            $"PullNextEncounter replaced. roomType={roomType}, bannedEncounterId={encounterId}, replacementEncounterId={replacement.Id}");
    }

    private static IReadOnlyList<EncounterModel> GetCandidates(ActModel act, RoomType roomType)
    {
        IEnumerable<EncounterModel> query = roomType switch
        {
            RoomType.Monster => act.AllEncounters.Where(encounter => encounter.RoomType == RoomType.Monster),
            RoomType.Elite => act.AllEliteEncounters,
            RoomType.Boss => act.AllBossEncounters,
            _ => Array.Empty<EncounterModel>()
        };

        return query.ToList();
    }

    private static EncounterModel SelectReplacement(
        IReadOnlyList<EncounterModel> candidates,
        IReadOnlyList<EncounterModel> allowedCandidates,
        EncounterModel bannedEncounter)
    {
        int startIndex = candidates
            .Select((encounter, index) => new { encounter, index })
            .FirstOrDefault(item => item.encounter.Id == bannedEncounter.Id)
            ?.index ?? -1;

        if (startIndex >= 0)
        {
            for (int offset = 1; offset < candidates.Count; offset++)
            {
                int index = (startIndex + offset) % candidates.Count;
                EncounterModel candidate = candidates[index];
                if (BanEnemyConfigStore.IsEncounterAllowed(candidate.Id.ToString()))
                {
                    return candidate;
                }
            }
        }

        return allowedCandidates[0];
    }
}
