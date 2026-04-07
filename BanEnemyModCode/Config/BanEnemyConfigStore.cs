using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using BanEnemyMod.BanEnemyModCode.Infrastructure;
using BanEnemyMod.BanEnemyModCode.Localization;
using BanEnemyMod.BanEnemyModCode.Models;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Multiplayer.Game;
using MegaCrit.Sts2.Core.Runs;

namespace BanEnemyMod.BanEnemyModCode.Config;

internal static class BanEnemyConfigStore
{
    private static readonly object Sync = new();
    private static BanEnemyConfig? _config;
    private static HashSet<string>? _activeBannedEncounterIds;
    private static List<ModifierModel>? _pendingRunModifiers;

    public static string ConfigPath
    {
        get
        {
            string assemblyLocation = Assembly.GetExecutingAssembly().Location;
            string? directory = Path.GetDirectoryName(assemblyLocation);
            return Path.Combine(directory ?? AppContext.BaseDirectory, "ban_enemy_config.json");
        }
    }

    public static IReadOnlyCollection<string> BannedEncounterIds
    {
        get
        {
            lock (Sync)
            {
                return GetCurrentBannedEncounterIdsLocked().ToArray();
            }
        }
    }

    public static bool HasActiveRunSnapshot
    {
        get
        {
            lock (Sync)
            {
                return _activeBannedEncounterIds != null;
            }
        }
    }

    public static bool IsEditingEnabled => !IsRunLocked;

    public static bool IsRunLocked
    {
        get
        {
            lock (Sync)
            {
                return _activeBannedEncounterIds != null || RunManager.Instance?.IsInProgress == true;
            }
        }
    }

    public static bool IsEncounterAllowed(string encounterId)
    {
        lock (Sync)
        {
            return !GetCurrentBannedEncounterIdsLocked().Contains(encounterId);
        }
    }

    public static void SetEncounterAllowed(string encounterId, bool allowed)
    {
        lock (Sync)
        {
            if (IsRunLocked)
            {
                return;
            }

            BanEnemyConfig config = EnsureLoaded();
            if (allowed)
            {
                config.BannedEncounterIds.Remove(encounterId);
            }
            else
            {
                config.BannedEncounterIds.Add(encounterId);
            }

            BanEnemyConfig.Save(ConfigPath, config);
        }
    }

    public static void CapturePendingRunModifiers(IReadOnlyList<ModifierModel> modifiers)
    {
        lock (Sync)
        {
            _pendingRunModifiers = modifiers.Select(CloneModifier).ToList();
            int snapshotCount = _pendingRunModifiers.OfType<BanEncounterSnapshotModifier>().Count();
            HookTrace.Write(
                $"Captured pending run modifiers. total={_pendingRunModifiers.Count}, snapshotModifiers={snapshotCount}, netType={RunManager.Instance?.NetService?.Type}");
        }
    }

    public static IReadOnlyList<ModifierModel> MergeSnapshotModifier(IReadOnlyList<ModifierModel> existingModifiers)
    {
        lock (Sync)
        {
            List<ModifierModel> source = existingModifiers.Select(CloneModifier).ToList();

            if (source.OfType<BanEncounterSnapshotModifier>().Any())
            {
                return source;
            }

            source.Add(BanEncounterSnapshotModifier.CreateFromEncounterIds(EnsureLoaded().BannedEncounterIds));
            HookTrace.Write($"Merged snapshot modifier into modifier list. total={source.Count}");
            return source;
        }
    }

    public static IReadOnlyList<ModifierModel> GetModifiersForRunCreation(IReadOnlyList<ModifierModel> existingModifiers)
    {
        lock (Sync)
        {
            List<ModifierModel> source = _pendingRunModifiers?.Select(CloneModifier).ToList()
                ?? existingModifiers.Select(CloneModifier).ToList();
            BanEncounterSnapshotModifier? snapshotModifier = source.OfType<BanEncounterSnapshotModifier>().FirstOrDefault();
            _activeBannedEncounterIds = snapshotModifier != null
                ? new HashSet<string>(snapshotModifier.GetBannedEncounterIds(), StringComparer.Ordinal)
                : new HashSet<string>(EnsureLoaded().BannedEncounterIds, StringComparer.Ordinal);
            HookTrace.Write(
                $"Activating run snapshot from modifiers. sourceCount={source.Count}, snapshotPresent={snapshotModifier != null}, bannedCount={_activeBannedEncounterIds.Count}, netType={RunManager.Instance?.NetService?.Type}");
            _pendingRunModifiers = null;
            return source.Where(modifier => modifier is not BanEncounterSnapshotModifier).ToList();
        }
    }

    public static void TryActivateRunSnapshot(RunState runState)
    {
        lock (Sync)
        {
            BanEncounterSnapshotModifier? snapshotModifier = runState.Modifiers.OfType<BanEncounterSnapshotModifier>().FirstOrDefault();
            if (snapshotModifier == null)
            {
                HookTrace.Write("TryActivateRunSnapshot found no snapshot modifier on RunState.");
                return;
            }

            _activeBannedEncounterIds = new HashSet<string>(snapshotModifier.GetBannedEncounterIds(), StringComparer.Ordinal);
            _pendingRunModifiers = null;
            HookTrace.Write($"Activated run snapshot from RunState. bannedCount={_activeBannedEncounterIds.Count}");
        }
    }

    public static void ClearRunSnapshot()
    {
        lock (Sync)
        {
            _activeBannedEncounterIds = null;
            _pendingRunModifiers = null;
        }
    }

    public static string GetStatusText()
    {
        lock (Sync)
        {
            if (IsRunLocked)
            {
                return BanEnemyText.Get("status.readonly");
            }

            NetGameType? netGameType = RunManager.Instance?.NetService?.Type;
            if (netGameType == NetGameType.Client)
            {
                return BanEnemyText.Get("status.client");
            }

            return BanEnemyText.Get("status.editable");
        }
    }

    private static BanEnemyConfig EnsureLoaded()
    {
        _config ??= BanEnemyConfig.Load(ConfigPath);
        return _config;
    }

    private static HashSet<string> GetCurrentBannedEncounterIdsLocked()
    {
        return _activeBannedEncounterIds ?? EnsureLoaded().BannedEncounterIds;
    }

    private static ModifierModel CloneModifier(ModifierModel modifier)
    {
        return modifier.ClonePreservingMutability() as ModifierModel
               ?? throw new InvalidOperationException($"Failed to clone modifier {modifier.Id}.");
    }
}
