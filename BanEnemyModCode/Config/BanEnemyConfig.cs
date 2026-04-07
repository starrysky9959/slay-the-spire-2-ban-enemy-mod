using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using BanEnemyMod.BanEnemyModCode.Infrastructure;

namespace BanEnemyMod.BanEnemyModCode.Config;

internal sealed class BanEnemyConfig
{
    public HashSet<string> BannedEncounterIds { get; set; } = new(StringComparer.Ordinal);

    public static BanEnemyConfig Load(string path)
    {
        try
        {
            if (!File.Exists(path))
            {
                BanEnemyConfig config = new();
                Save(path, config);
                return config;
            }

            string json = File.ReadAllText(path);
            BanEnemyConfig? configFromDisk = JsonSerializer.Deserialize<BanEnemyConfig>(json);
            if (configFromDisk == null)
            {
                return new BanEnemyConfig();
            }

            configFromDisk.BannedEncounterIds = new HashSet<string>(
                configFromDisk.BannedEncounterIds,
                StringComparer.Ordinal);
            return configFromDisk;
        }
        catch (Exception ex)
        {
            HookTrace.Write($"Failed to load config: {ex}");
            return new BanEnemyConfig();
        }
    }

    public static void Save(string path, BanEnemyConfig config)
    {
        try
        {
            string? directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            JsonSerializerOptions options = new()
            {
                WriteIndented = true
            };
            string json = JsonSerializer.Serialize(config, options);
            File.WriteAllText(path, json);
        }
        catch (Exception ex)
        {
            HookTrace.Write($"Failed to save config: {ex}");
        }
    }
}
