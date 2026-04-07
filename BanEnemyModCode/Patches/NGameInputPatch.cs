using BanEnemyMod.BanEnemyModCode.Infrastructure;
using BanEnemyMod.BanEnemyModCode.UI;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes;

namespace BanEnemyMod.BanEnemyModCode.Patches;

[HarmonyPatch(typeof(NGame), nameof(NGame._Input))]
internal static class NGameInputPatch
{
    private const string LayerName = "BanEncounterSelectionLayer";

    private static void Postfix(NGame __instance, InputEvent inputEvent)
    {
        if (inputEvent is not InputEventKey keyEvent || !keyEvent.Pressed || keyEvent.Echo || keyEvent.Keycode != Key.B)
        {
            return;
        }

        BanEncounterSelectionLayer layer = EnsureLayer(__instance);
        HookTrace.Write("Global B hotkey pressed.");
        layer.ToggleLayer();
        __instance.GetViewport().SetInputAsHandled();
    }

    private static BanEncounterSelectionLayer EnsureLayer(Node parent)
    {
        BanEncounterSelectionLayer? layer = parent.GetNodeOrNull<BanEncounterSelectionLayer>(LayerName);
        if (layer != null)
        {
            return layer;
        }

        layer = BanEncounterSelectionLayer.Create();
        parent.AddChild(layer);
        HookTrace.Write("Ban encounter layer node added to scene tree from NGame.");
        return layer;
    }
}
