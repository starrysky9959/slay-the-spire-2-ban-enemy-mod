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

[HarmonyPatch(typeof(NCharacterSelectScreen))]
internal static class CharacterSelectScreenReadyPatch
{
    internal const string ButtonName = "BanEncountersButton";
    internal const string LayerName = "BanEncounterSelectionLayer";
    private const string TitleLabelName = "BanEncountersButtonTitle";
    private const string HotkeyLabelName = "BanEncountersButtonHotkey";
    private const string ConfirmButtonPath = "ConfirmButton";
    private static readonly Color AccentColor = new(0.97f, 0.86f, 0.38f, 1f);
    private static readonly Color AccentHoverColor = new(1f, 0.91f, 0.52f, 1f);
    private static readonly Color AccentPressedColor = new(0.92f, 0.74f, 0.25f, 1f);
    private static readonly Color ButtonFillColor = new(0.12f, 0.14f, 0.18f, 0.96f);
    private static readonly Color ButtonHoverFillColor = new(0.18f, 0.2f, 0.25f, 0.98f);
    private static readonly Color ButtonPressedFillColor = new(0.09f, 0.1f, 0.14f, 1f);
    private static readonly Color ButtonBorderColor = new(0.92f, 0.74f, 0.25f, 1f);

    [HarmonyPostfix]
    [HarmonyPatch(nameof(NCharacterSelectScreen._Ready))]
    [HarmonyPatch(nameof(NCharacterSelectScreen.InitializeSingleplayer))]
    [HarmonyPatch(nameof(NCharacterSelectScreen.InitializeMultiplayerAsHost))]
    [HarmonyPatch(nameof(NCharacterSelectScreen.InitializeMultiplayerAsClient))]
    [HarmonyPatch("AfterInitialized")]
    private static void Postfix(NCharacterSelectScreen __instance)
    {
        EnsureButton(__instance, clearRunSnapshot: true);
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(NCharacterSelectScreen._Process))]
    private static void ProcessPostfix(NCharacterSelectScreen __instance)
    {
        EnsureButton(__instance, clearRunSnapshot: false);
    }

    internal static void EnsureButton(NCharacterSelectScreen screen, bool clearRunSnapshot)
    {
        if (clearRunSnapshot)
        {
            BanEnemyConfigStore.ClearRunSnapshot();
        }

        Node overlayParent = GetOverlayParent(screen);
        if (overlayParent.GetNodeOrNull<BanEncounterSelectionLayer>(LayerName) == null)
        {
            BanEncounterSelectionLayer layer = BanEncounterSelectionLayer.Create();
            overlayParent.AddChild(layer);
            HookTrace.Write("Ban encounter layer node added to scene tree.");
        }

        if (screen.GetNodeOrNull<Button>(ButtonName) is { } existingButton)
        {
            existingButton.Text = string.Empty;
            existingButton.CustomMinimumSize = new Vector2(280, 60);
            ApplyButtonAccent(existingButton);
            ApplyButtonFont(existingButton);
            EnsureButtonLabels(existingButton);
            PositionButton(screen, existingButton);
            return;
        }

        Button openButton = new()
        {
            Name = ButtonName,
            Text = string.Empty,
            CustomMinimumSize = new Vector2(280, 60),
            Alignment = HorizontalAlignment.Center,
            ZIndex = 10_001
        };
        ApplyButtonAccent(openButton);
        ApplyButtonFont(openButton);
        EnsureButtonLabels(openButton);
        PositionButton(screen, openButton);
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

        screen.AddChild(openButton);
        screen.MoveChild(openButton, screen.GetChildCount() - 1);
    }

    internal static Node GetOverlayParent(Node screen)
    {
        return NGame.Instance != null ? (Node)NGame.Instance : screen.GetTree().Root;
    }

    internal static BanEncounterSelectionLayer EnsureLayer(Node screen)
    {
        Node overlayParent = GetOverlayParent(screen);
        if (overlayParent.GetNodeOrNull<BanEncounterSelectionLayer>(LayerName) is { } existingLayer)
        {
            return existingLayer;
        }

        BanEncounterSelectionLayer layer = BanEncounterSelectionLayer.Create();
        overlayParent.AddChild(layer);
        HookTrace.Write("Ban encounter layer node added to scene tree from helper.");
        return layer;
    }

    private static void PositionButton(NCharacterSelectScreen screen, Button button)
    {
        button.TopLevel = false;
        button.SetAnchorsPreset(Control.LayoutPreset.TopLeft);

        if (screen.GetNodeOrNull<Control>(ConfirmButtonPath) is { } confirmButton)
        {
            Vector2 basePosition = confirmButton.Position;
            float y = Mathf.Max(20f, basePosition.Y - 72f);
            button.Position = new Vector2(basePosition.X - 64f, y);
            return;
        }

        button.TopLevel = true;
        button.SetAnchorsPreset(Control.LayoutPreset.TopRight);
        button.Position = new Vector2(-360, 20);
    }

    private static void ApplyButtonAccent(Button button)
    {
        StyleBoxFlat normalStyle = CreateButtonStyle(ButtonFillColor, ButtonBorderColor);
        StyleBoxFlat hoverStyle = CreateButtonStyle(ButtonHoverFillColor, AccentHoverColor);
        StyleBoxFlat pressedStyle = CreateButtonStyle(ButtonPressedFillColor, AccentPressedColor);

        button.AddThemeStyleboxOverride("normal", normalStyle);
        button.AddThemeStyleboxOverride("hover", hoverStyle);
        button.AddThemeStyleboxOverride("pressed", pressedStyle);
        button.AddThemeStyleboxOverride("focus", hoverStyle);
        button.AddThemeColorOverride("font_color", AccentColor);
        button.AddThemeColorOverride("font_hover_color", AccentHoverColor);
        button.AddThemeColorOverride("font_pressed_color", AccentPressedColor);
        button.AddThemeColorOverride("font_focus_color", AccentHoverColor);
        button.AddThemeConstantOverride("outline_size", 6);
        button.AddThemeConstantOverride("h_separation", 0);
        button.AddThemeColorOverride("font_outline_color", new Color(0f, 0f, 0f, 0.85f));
    }

    private static void ApplyButtonFont(Button button)
    {
        button.ApplyLocaleFontSubstitution(FontType.Regular, ThemeConstants.Label.Font);
    }

    private static void EnsureButtonLabels(Button button)
    {
        Label titleLabel = button.GetNodeOrNull<Label>(TitleLabelName) ?? CreateTitleLabel(button);
        Label hotkeyLabel = button.GetNodeOrNull<Label>(HotkeyLabelName) ?? CreateHotkeyLabel(button);

        titleLabel.Text = BanEnemyText.Get("button.open");
        titleLabel.ApplyLocaleFontSubstitution(FontType.Regular, ThemeConstants.Label.Font);
        hotkeyLabel.Text = "[B]";
        hotkeyLabel.ApplyLocaleFontSubstitution(FontType.Regular, ThemeConstants.Label.Font);
    }

    private static Label CreateTitleLabel(Button button)
    {
        Label label = new()
        {
            Name = TitleLabelName,
            MouseFilter = Control.MouseFilterEnum.Ignore,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };
        label.SetAnchorsPreset(Control.LayoutPreset.FullRect);
        label.OffsetLeft = 18f;
        label.OffsetTop = 0f;
        label.OffsetRight = -54f;
        label.OffsetBottom = 0f;
        label.AddThemeColorOverride("font_color", AccentColor);
        label.AddThemeColorOverride("font_outline_color", new Color(0f, 0f, 0f, 0.85f));
        label.AddThemeConstantOverride("outline_size", 6);
        button.AddChild(label);
        return label;
    }

    private static Label CreateHotkeyLabel(Button button)
    {
        Label label = new()
        {
            Name = HotkeyLabelName,
            MouseFilter = Control.MouseFilterEnum.Ignore,
            HorizontalAlignment = HorizontalAlignment.Right,
            VerticalAlignment = VerticalAlignment.Center
        };
        label.SetAnchorsPreset(Control.LayoutPreset.FullRect);
        label.OffsetLeft = 0f;
        label.OffsetTop = 0f;
        label.OffsetRight = -16f;
        label.OffsetBottom = 0f;
        label.AddThemeColorOverride("font_color", AccentHoverColor);
        label.AddThemeColorOverride("font_outline_color", new Color(0f, 0f, 0f, 0.85f));
        label.AddThemeConstantOverride("outline_size", 6);
        button.AddChild(label);
        return label;
    }

    private static StyleBoxFlat CreateButtonStyle(Color fillColor, Color borderColor)
    {
        return new StyleBoxFlat
        {
            BgColor = fillColor,
            BorderColor = borderColor,
            BorderWidthLeft = 3,
            BorderWidthTop = 3,
            BorderWidthRight = 3,
            BorderWidthBottom = 3,
            CornerRadiusTopLeft = 12,
            CornerRadiusTopRight = 12,
            CornerRadiusBottomRight = 12,
            CornerRadiusBottomLeft = 12,
            ContentMarginLeft = 10,
            ContentMarginTop = 6,
            ContentMarginRight = 10,
            ContentMarginBottom = 6,
            ShadowColor = new Color(0f, 0f, 0f, 0.35f),
            ShadowSize = 6,
            ShadowOffset = new Vector2(0, 3)
        };
    }
}
