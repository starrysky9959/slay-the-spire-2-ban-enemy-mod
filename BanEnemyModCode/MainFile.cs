using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Modding;
using BanEnemyMod.BanEnemyModCode.Infrastructure;
using BanEnemyMod.BanEnemyModCode.Models;
using MegaCrit.Sts2.Core.Saves.Runs;
using System;
using System.Reflection;

namespace BanEnemyMod.BanEnemyModCode;

[ModInitializer(nameof(Initialize))]
public partial class MainFile : Node
{
    public const string ModId = "BanEnemyMod";

    public static MegaCrit.Sts2.Core.Logging.Logger Logger { get; } =
        new(ModId, MegaCrit.Sts2.Core.Logging.LogType.Generic);

    public static void Initialize()
    {
        HookTrace.Write("Initializing mod and applying Harmony patches.");

        RegisterSavedPropertyTypes();

        Harmony harmony = new(ModId);
        harmony.PatchAll();

        HookTrace.Write("Harmony patches applied.");
    }

    private static void RegisterSavedPropertyTypes()
    {
        SavedPropertiesTypeCache.InjectTypeIntoCache(typeof(BanEncounterSnapshotModifier));

        PropertyInfo? netIdBitSizeProperty = typeof(SavedPropertiesTypeCache).GetProperty(
            nameof(SavedPropertiesTypeCache.NetIdBitSize),
            BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        FieldInfo? propertyNameMapField = typeof(SavedPropertiesTypeCache).GetField(
            "_netIdToPropertyNameMap",
            BindingFlags.Static | BindingFlags.NonPublic);

        if (netIdBitSizeProperty?.SetMethod != null && propertyNameMapField?.GetValue(null) is System.Collections.ICollection propertyNames)
        {
            int netIdBitSize = Mathf.CeilToInt(Mathf.Log(propertyNames.Count) / Mathf.Log(2f));
            netIdBitSizeProperty.SetValue(null, Math.Max(1, netIdBitSize));
        }

        HookTrace.Write("Registered custom SavedProperty types for multiplayer serialization.");
    }
}
