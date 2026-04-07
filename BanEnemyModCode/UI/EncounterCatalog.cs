using System;
using System.Collections.Generic;
using System.Linq;
using BanEnemyMod.BanEnemyModCode.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;

namespace BanEnemyMod.BanEnemyModCode.UI;

internal static class EncounterCatalog
{
    internal sealed record EncounterEntry(
        int MapOrder,
        string MapId,
        string MapLabel,
        string Category,
        string EncounterId,
        string EncounterTitle,
        string MonsterSummary,
        IReadOnlyList<string> MonsterIds,
        IReadOnlyList<string> MonsterTitles);

    public static IReadOnlyList<EncounterEntry> Build()
    {
        List<EncounterEntry> entries = new();
        int mapOrder = 0;

        foreach (ActModel act in ModelDb.Acts)
        {
            mapOrder++;

            foreach (EncounterModel encounter in act.AllEncounters)
            {
                string category = encounter.RoomType switch
                {
                    RoomType.Monster => BanEnemyText.Get("category.normal"),
                    RoomType.Elite => BanEnemyText.Get("category.elite"),
                    RoomType.Boss => BanEnemyText.Get("category.boss"),
                    _ => string.Empty
                };

                if (string.IsNullOrEmpty(category))
                {
                    continue;
                }

                string monsterSummary = string.Join(
                    ", ",
                    encounter.AllPossibleMonsters
                        .Select(m => m.Title.GetFormattedText())
                        .Distinct(StringComparer.Ordinal)
                        .OrderBy(name => name, StringComparer.Ordinal));
                List<string> monsterIds = encounter.AllPossibleMonsters
                    .Select(m => m.Id.ToString())
                    .Distinct(StringComparer.Ordinal)
                    .ToList();
                List<string> monsterTitles = encounter.AllPossibleMonsters
                    .Select(m => m.Title.GetFormattedText())
                    .Distinct(StringComparer.Ordinal)
                    .OrderBy(name => name, StringComparer.Ordinal)
                    .ToList();

                entries.Add(new EncounterEntry(
                    mapOrder,
                    act.Id.ToString(),
                    act.Title.GetFormattedText(),
                    category,
                    encounter.Id.ToString(),
                    encounter.Title.GetFormattedText(),
                    monsterSummary,
                    monsterIds,
                    monsterTitles));
            }
        }

        return entries
            .OrderBy(e => e.MapOrder)
            .ThenBy(e => CategoryOrder(e.Category))
            .ThenBy(e => e.EncounterTitle, StringComparer.Ordinal)
            .ToList();
    }

    private static int CategoryOrder(string category)
    {
        if (category == BanEnemyText.Get("category.normal"))
        {
            return 0;
        }

        if (category == BanEnemyText.Get("category.elite"))
        {
            return 1;
        }

        return category == BanEnemyText.Get("category.boss") ? 2 : 3;
    }
}
