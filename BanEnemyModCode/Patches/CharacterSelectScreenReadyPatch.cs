using BanEnemyMod.BanEnemyModCode.Config;
using BanEnemyMod.BanEnemyModCode.Infrastructure;
using BanEnemyMod.BanEnemyModCode.Localization;
using BanEnemyMod.BanEnemyModCode.UI;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Screens.CharacterSelect;
using MegaCrit.Sts2.Core.Localization.Fonts;
using MegaCrit.Sts2.addons.mega_text;
using Godot;

namespace BanEnemyMod.BanEnemyModCode.Patches;

[HarmonyPatch(typeof(NCharacterSelectScreen), nameof(NCharacterSelectScreen._Ready))]
internal static class CharacterSelectScreenReadyPatch
{
    private const string ButtonName = "BanEncountersButton";
    private const string LayerName = "BanEncounterSelectionLayer";
    private static readonly Color AccentColor = new(0.97f, 0.86f, 0.38f, 1f);
    private static readonly Color AccentHoverColor = new(1f, 0.91f, 0.52f, 1f);
    private static readonly Color AccentPressedColor = new(0.92f, 0.74f, 0.25f, 1f);

    private static void Postfix(NCharacterSelectScreen __instance)
    {
        BanEnemyConfigStore.ClearRunSnapshot();

        if (__instance.GetNodeOrNull<Button>(ButtonName) is { } existingButton)
        {
            existingButton.Text = BanEnemyText.Get("button.open");
            ApplyButtonAccent(existingButton);
            ApplyButtonFont(existingButton);
            return;
        }

        Node overlayParent = NGame.Instance != null ? (Node)NGame.Instance : __instance.GetTree().Root;
        if (overlayParent.GetNodeOrNull<BanEncounterSelectionLayer>(LayerName) == null)
        {
            BanEncounterSelectionLayer layer = BanEncounterSelectionLayer.Create();
            overlayParent.AddChild(layer);
            HookTrace.Write("Ban encounter layer node added to scene tree.");
        }

        Button openButton = new()
        {
            Name = ButtonName,
            Text = BanEnemyText.Get("button.open"),
            CustomMinimumSize = new Vector2(190, 44),
            TopLevel = true,
            ZIndex = 10_001
        };
        ApplyButtonAccent(openButton);
        ApplyButtonFont(openButton);
        openButton.SetAnchorsPreset(Control.LayoutPreset.TopRight);
        openButton.Position = new Vector2(-220, 24);
        openButton.Pressed += () =>
        {
            HookTrace.Write("Ban Encounters button pressed.");
            BanEncounterSelectionLayer? currentLayer = overlayParent.GetNodeOrNull<BanEncounterSelectionLayer>(LayerName);
            if (currentLayer == null)
            {
                HookTrace.Write("Ban encounter layer node was not found.");
                return;
            }

            currentLayer.ToggleLayer();
        };

        __instance.AddChild(openButton);
        __instance.MoveChild(openButton, __instance.GetChildCount() - 1);
    }

    private static void ApplyButtonAccent(Button button)
    {
        button.AddThemeColorOverride("font_color", AccentColor);
        button.AddThemeColorOverride("font_hover_color", AccentHoverColor);
        button.AddThemeColorOverride("font_pressed_color", AccentPressedColor);
        button.AddThemeColorOverride("font_focus_color", AccentHoverColor);
    }

    private static void ApplyButtonFont(Button button)
    {
        button.ApplyLocaleFontSubstitution(FontType.Regular, ThemeConstants.Label.font);
    }
}
