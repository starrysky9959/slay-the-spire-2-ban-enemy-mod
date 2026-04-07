using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BanEnemyMod.BanEnemyModCode.Config;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;

namespace BanEnemyMod.BanEnemyModCode.Infrastructure;

internal static class ActEncounterFilter
{
    private static readonly FieldInfo RoomsField =
        typeof(ActModel).GetField("_rooms", BindingFlags.Instance | BindingFlags.NonPublic)
        ?? throw new InvalidOperationException("Could not access ActModel._rooms");

    public static void ApplyToAct(ActModel act)
    {
        RoomSet? rooms = RoomsField.GetValue(act) as RoomSet;
        if (rooms == null)
        {
            return;
        }

        FilterEncounterList(rooms.normalEncounters, act.AllEncounters.Where(e => e.RoomType == RoomType.Monster).ToList());
        FilterEncounterList(rooms.eliteEncounters, act.AllEliteEncounters.ToList());
        rooms.Boss = FilterBossEncounter(rooms.Boss, act.AllBossEncounters.ToList());

        if (rooms.SecondBoss != null)
        {
            rooms.SecondBoss = FilterSecondBossEncounter(rooms.SecondBoss, rooms.Boss, act.AllBossEncounters.ToList());
        }
    }

    private static void FilterEncounterList(List<EncounterModel> scheduledEncounters, IReadOnlyList<EncounterModel> candidates)
    {
        if (scheduledEncounters.Count == 0 || candidates.Count == 0)
        {
            return;
        }

        List<EncounterModel> allowedCandidates = candidates
            .Where(encounter => BanEnemyConfigStore.IsEncounterAllowed(encounter.Id.ToString()))
            .ToList();

        if (allowedCandidates.Count == 0)
        {
            return;
        }

        for (int i = 0; i < scheduledEncounters.Count; i++)
        {
            EncounterModel current = scheduledEncounters[i];
            if (BanEnemyConfigStore.IsEncounterAllowed(current.Id.ToString()))
            {
                continue;
            }

            scheduledEncounters[i] = SelectReplacement(candidates, allowedCandidates, current);
        }
    }

    private static EncounterModel FilterBossEncounter(EncounterModel currentBoss, IReadOnlyList<EncounterModel> candidates)
    {
        if (BanEnemyConfigStore.IsEncounterAllowed(currentBoss.Id.ToString()))
        {
            return currentBoss;
        }

        List<EncounterModel> allowedCandidates = candidates
            .Where(encounter => BanEnemyConfigStore.IsEncounterAllowed(encounter.Id.ToString()))
            .ToList();

        if (allowedCandidates.Count == 0)
        {
            return currentBoss;
        }

        return SelectReplacement(candidates, allowedCandidates, currentBoss);
    }

    private static EncounterModel FilterSecondBossEncounter(
        EncounterModel currentSecondBoss,
        EncounterModel currentBoss,
        IReadOnlyList<EncounterModel> allBossCandidates)
    {
        if (BanEnemyConfigStore.IsEncounterAllowed(currentSecondBoss.Id.ToString()) && currentSecondBoss.Id != currentBoss.Id)
        {
            return currentSecondBoss;
        }

        List<EncounterModel> secondaryCandidates = allBossCandidates
            .Where(encounter => encounter.Id != currentBoss.Id)
            .ToList();
        List<EncounterModel> allowedSecondaryCandidates = secondaryCandidates
            .Where(encounter => BanEnemyConfigStore.IsEncounterAllowed(encounter.Id.ToString()))
            .ToList();

        if (allowedSecondaryCandidates.Count > 0)
        {
            return SelectReplacement(secondaryCandidates, allowedSecondaryCandidates, currentSecondBoss);
        }

        return currentBoss;
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
            for (int offset = 1; offset <= candidates.Count; offset++)
            {
                EncounterModel candidate = candidates[(startIndex + offset) % candidates.Count];
                if (BanEnemyConfigStore.IsEncounterAllowed(candidate.Id.ToString()))
                {
                    return candidate;
                }
            }
        }

        return allowedCandidates[0];
    }
}
