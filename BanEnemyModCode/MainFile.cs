using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Modding;
using BanEnemyMod.BanEnemyModCode.Infrastructure;

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

        Harmony harmony = new(ModId);
        harmony.PatchAll();

        HookTrace.Write("Harmony patches applied.");
    }
}
