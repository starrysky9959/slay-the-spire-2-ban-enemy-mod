using System;
using System.Collections.Generic;
using System.Linq;
using BanEnemyMod.BanEnemyModCode.Infrastructure;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace BanEnemyMod.BanEnemyModCode.Models;

internal sealed class BanEncounterSnapshotModifier : ModifierModel
{
    private const char Delimiter = '\n';

    [SavedProperty]
    public string SnapshotData { get; set; } = string.Empty;

    public override LocString Title => new("modifiers", "flight.title");

    public override LocString Description => new("modifiers", "flight.description");

    public static BanEncounterSnapshotModifier CreateFromEncounterIds(IEnumerable<string> bannedEncounterIds)
    {
        BanEncounterSnapshotModifier modifier = ModelDb.GetById<BanEncounterSnapshotModifier>(ModelDb.GetId<BanEncounterSnapshotModifier>())
            .ToMutable() as BanEncounterSnapshotModifier
            ?? throw new InvalidOperationException("Failed to create BanEncounterSnapshotModifier.");
        modifier.SnapshotData = string.Join(
            Delimiter,
            bannedEncounterIds
                .Where(id => !string.IsNullOrWhiteSpace(id))
                .Distinct(StringComparer.Ordinal)
                .OrderBy(id => id, StringComparer.Ordinal));
        HookTrace.Write($"Built ban snapshot modifier. bannedCount={modifier.GetBannedEncounterIds().Count}");
        return modifier;
    }

    public IReadOnlyCollection<string> GetBannedEncounterIds()
    {
        if (string.IsNullOrWhiteSpace(SnapshotData))
        {
            return Array.Empty<string>();
        }

        return SnapshotData
            .Split(Delimiter, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Distinct(StringComparer.Ordinal)
            .ToArray();
    }

    public override bool IsEquivalent(ModifierModel other)
    {
        return other is BanEncounterSnapshotModifier snapshot
               && string.Equals(snapshot.SnapshotData, SnapshotData, StringComparison.Ordinal);
    }
}
