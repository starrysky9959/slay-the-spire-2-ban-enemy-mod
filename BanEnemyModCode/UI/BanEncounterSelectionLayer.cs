using System;
using System.Collections.Generic;
using System.Linq;
using BanEnemyMod.BanEnemyModCode.Config;
using BanEnemyMod.BanEnemyModCode.Infrastructure;
using BanEnemyMod.BanEnemyModCode.Localization;
using Godot;
using MegaCrit.Sts2.Core.Localization.Fonts;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.addons.mega_text;

namespace BanEnemyMod.BanEnemyModCode.UI;

internal sealed partial class BanEncounterSelectionLayer : CanvasLayer
{
    private IReadOnlyList<EncounterCatalog.EncounterEntry> _entries = Array.Empty<EncounterCatalog.EncounterEntry>();
    private Dictionary<string, EncounterCatalog.EncounterEntry> _entryById = new(StringComparer.Ordinal);
    private bool _initialized;
    private int _encounterColumnWidth;

    private Control _root = null!;
    private LineEdit _searchInput = null!;
    private Label _countLabel = null!;
    private Label _modeLabel = null!;
    private Label _warningLabel = null!;
    private Label _ruleHintLabel = null!;
    private Label _titleLabel = null!;
    private Button _selectAllButton = null!;
    private Button _deselectAllButton = null!;
    private Button _closeButton = null!;
    private Tree _tree = null!;
    private Label _previewTitle = null!;
    private Label _previewSubtitle = null!;
    private VBoxContainer _previewMonsterList = null!;
    private string? _currentPreviewEncounterId;

    public static BanEncounterSelectionLayer Create()
    {
        BanEncounterSelectionLayer layer = new();
        layer.Name = "BanEncounterSelectionLayer";
        layer.InitializeUi();
        return layer;
    }

    private void InitializeUi()
    {
        if (_initialized)
        {
            return;
        }

        _initialized = true;
        Layer = 100;

        _root = new Control
        {
            Visible = false,
            MouseFilter = Control.MouseFilterEnum.Stop
        };
        _root.SetAnchorsPreset(Control.LayoutPreset.FullRect);
        AddChild(_root);

        ColorRect backdrop = new()
        {
            Color = new Color(0f, 0f, 0f, 0.78f),
            MouseFilter = Control.MouseFilterEnum.Stop
        };
        backdrop.SetAnchorsPreset(Control.LayoutPreset.FullRect);
        _root.AddChild(backdrop);

        MarginContainer outer = new();
        outer.SetAnchorsPreset(Control.LayoutPreset.FullRect);
        outer.AddThemeConstantOverride("margin_left", 90);
        outer.AddThemeConstantOverride("margin_top", 50);
        outer.AddThemeConstantOverride("margin_right", 90);
        outer.AddThemeConstantOverride("margin_bottom", 50);
        _root.AddChild(outer);

        PanelContainer panel = new();
        outer.AddChild(panel);

        MarginContainer panelMargin = new();
        panelMargin.AddThemeConstantOverride("margin_left", 18);
        panelMargin.AddThemeConstantOverride("margin_top", 18);
        panelMargin.AddThemeConstantOverride("margin_right", 18);
        panelMargin.AddThemeConstantOverride("margin_bottom", 18);
        panel.AddChild(panelMargin);

        HBoxContainer split = new();
        split.AddThemeConstantOverride("separation", 16);
        panelMargin.AddChild(split);

        VBoxContainer root = new();
        root.AddThemeConstantOverride("separation", 10);
        root.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
        root.CustomMinimumSize = new Vector2(560, 0);
        split.AddChild(root);

        HBoxContainer header = new();
        header.AddThemeConstantOverride("separation", 8);
        root.AddChild(header);

        _titleLabel = new Label
        {
            Text = BanEnemyText.Get("title"),
            SizeFlagsHorizontal = Control.SizeFlags.ExpandFill
        };
        _titleLabel.ApplyLocaleFontSubstitution(FontType.Regular, ThemeConstants.Label.font);
        header.AddChild(_titleLabel);

        _countLabel = new Label();
        _countLabel.ApplyLocaleFontSubstitution(FontType.Regular, ThemeConstants.Label.font);
        root.AddChild(_countLabel);

        _modeLabel = new Label
        {
            AutowrapMode = TextServer.AutowrapMode.WordSmart
        };
        _modeLabel.ApplyLocaleFontSubstitution(FontType.Regular, ThemeConstants.Label.font);
        root.AddChild(_modeLabel);

        _warningLabel = new Label
        {
            Visible = false,
            AutowrapMode = TextServer.AutowrapMode.WordSmart
        };
        _warningLabel.ApplyLocaleFontSubstitution(FontType.Regular, ThemeConstants.Label.font);
        _warningLabel.Modulate = new Color(1f, 0.72f, 0.3f, 1f);
        root.AddChild(_warningLabel);

        _ruleHintLabel = new Label
        {
            AutowrapMode = TextServer.AutowrapMode.WordSmart
        };
        _ruleHintLabel.ApplyLocaleFontSubstitution(FontType.Regular, ThemeConstants.Label.font);
        _ruleHintLabel.Modulate = new Color(0.85f, 0.85f, 0.85f, 0.95f);
        root.AddChild(_ruleHintLabel);

        _searchInput = new LineEdit
        {
            PlaceholderText = BanEnemyText.Get("search.placeholder")
        };
        _searchInput.ApplyLocaleFontSubstitution(FontType.Regular, ThemeConstants.LineEdit.font);
        _searchInput.CustomMinimumSize = new Vector2(0, 44);
        _searchInput.TextChanged += _ => RefreshTree();
        root.AddChild(_searchInput);

        HBoxContainer actions = new();
        actions.AddThemeConstantOverride("separation", 8);
        root.AddChild(actions);

        _selectAllButton = new Button
        {
            Text = BanEnemyText.Get("select_all"),
            CustomMinimumSize = new Vector2(120, 52)
        };
        _selectAllButton.ApplyLocaleFontSubstitution(FontType.Regular, ThemeConstants.Label.font);
        _selectAllButton.AddThemeConstantOverride("h_separation", 6);
        _selectAllButton.Pressed += SelectAll;
        actions.AddChild(_selectAllButton);

        _deselectAllButton = new Button
        {
            Text = BanEnemyText.Get("deselect_all"),
            CustomMinimumSize = new Vector2(140, 52)
        };
        _deselectAllButton.ApplyLocaleFontSubstitution(FontType.Regular, ThemeConstants.Label.font);
        _deselectAllButton.AddThemeConstantOverride("h_separation", 6);
        _deselectAllButton.Pressed += DeselectAllSafely;
        actions.AddChild(_deselectAllButton);

        _closeButton = new Button
        {
            Text = BanEnemyText.Get("close"),
            CustomMinimumSize = new Vector2(160, 52)
        };
        _closeButton.ApplyLocaleFontSubstitution(FontType.Regular, ThemeConstants.Label.font);
        _closeButton.AddThemeConstantOverride("h_separation", 6);
        _closeButton.Pressed += HideLayer;
        actions.AddChild(_closeButton);

        _tree = new Tree
        {
            HideRoot = true,
            Columns = 2,
            SizeFlagsVertical = Control.SizeFlags.ExpandFill
        };
        _tree.SetColumnTitlesVisible(true);
        _tree.SetColumnTitle(0, BanEnemyText.Get("column.encounter"));
        _tree.SetColumnTitle(1, BanEnemyText.Get("column.monsters"));
        _tree.SetColumnExpand(0, false);
        _tree.SetColumnCustomMinimumWidth(0, _encounterColumnWidth);
        _tree.SetColumnExpand(1, true);
        _tree.SetColumnClipContent(0, true);
        _tree.SetColumnClipContent(1, true);
        _tree.ApplyLocaleFontSubstitution(FontType.Regular, new StringName("font"));
        _tree.ApplyLocaleFontSubstitution(FontType.Regular, new StringName("title_button_font"));
        _tree.AddThemeConstantOverride("v_separation", 4);
        _tree.AddThemeConstantOverride("inner_item_margin_left", 4);
        _tree.AddThemeConstantOverride("inner_item_margin_right", 4);
        _tree.AddThemeConstantOverride("inner_item_margin_top", 3);
        _tree.AddThemeConstantOverride("inner_item_margin_bottom", 3);
        _tree.ItemEdited += OnTreeItemEdited;
        _tree.GuiInput += OnTreeGuiInput;
        root.AddChild(_tree);

        VBoxContainer previewPanel = new();
        previewPanel.CustomMinimumSize = new Vector2(560, 0);
        previewPanel.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
        previewPanel.SizeFlagsVertical = Control.SizeFlags.ExpandFill;
        previewPanel.AddThemeConstantOverride("separation", 8);
        split.AddChild(previewPanel);

        _previewTitle = new Label
        {
            Text = BanEnemyText.Get("preview.title")
        };
        _previewTitle.ApplyLocaleFontSubstitution(FontType.Regular, ThemeConstants.Label.font);
        previewPanel.AddChild(_previewTitle);

        _previewSubtitle = new Label
        {
            Text = BanEnemyText.Get("preview.empty"),
            AutowrapMode = TextServer.AutowrapMode.WordSmart
        };
        _previewSubtitle.ApplyLocaleFontSubstitution(FontType.Regular, ThemeConstants.Label.font);
        previewPanel.AddChild(_previewSubtitle);

        ScrollContainer previewScroll = new()
        {
            SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
            SizeFlagsVertical = Control.SizeFlags.ExpandFill
        };
        previewPanel.AddChild(previewScroll);

        _previewMonsterList = new VBoxContainer
        {
            SizeFlagsHorizontal = Control.SizeFlags.ExpandFill
        };
        _previewMonsterList.AddThemeConstantOverride("separation", 12);
        previewScroll.AddChild(_previewMonsterList);

        RefreshLocalizedState();
        RefreshTree();
        HideLayer();
        HookTrace.Write("Ban encounter layer initialized.");
    }

    public void ToggleLayer()
    {
        InitializeUi();
        RefreshLocalizedState();
        bool nextVisible = !_root.Visible;
        _root.Visible = nextVisible;
        HookTrace.Write($"Ban encounter layer toggled. visible={nextVisible}");
        if (nextVisible)
        {
            RebuildTree();
            RefreshTree();
            ShowPreview(null);
            _searchInput.GrabFocus();
        }
    }

    private void HideLayer()
    {
        if (!_initialized)
        {
            return;
        }

        _root.Visible = false;
        HookTrace.Write("Ban encounter layer hidden.");
    }

    private void RebuildTree()
    {
        RefreshCatalog();
        _tree.Clear();
        TreeItem root = _tree.CreateItem();
        Dictionary<string, TreeItem> mapGroups = new(StringComparer.Ordinal);
        Dictionary<(string MapId, string Category), TreeItem> categoryGroups = new();
        bool isEditable = BanEnemyConfigStore.IsEditingEnabled;

        foreach (EncounterCatalog.EncounterEntry entry in _entries)
        {
            if (!mapGroups.TryGetValue(entry.MapId, out TreeItem? mapGroup))
            {
                mapGroup = CreateGroup(root, entry.MapLabel);
                mapGroups.Add(entry.MapId, mapGroup);
            }

            (string, string) categoryKey = (entry.MapId, entry.Category);
            if (!categoryGroups.TryGetValue(categoryKey, out TreeItem? categoryGroup))
            {
                categoryGroup = CreateGroup(mapGroup, entry.Category);
                categoryGroups.Add(categoryKey, categoryGroup);
            }

            TreeItem item = _tree.CreateItem(categoryGroup);
            item.SetCellMode(0, TreeItem.TreeCellMode.Check);
            item.SetEditable(0, isEditable);
            item.SetChecked(0, BanEnemyConfigStore.IsEncounterAllowed(entry.EncounterId));
            item.SetText(0, Shorten(entry.EncounterTitle, 22));
            item.SetText(1, Shorten(entry.MonsterSummary, 28));
            item.SetTooltipText(0, entry.EncounterTitle);
            item.SetTooltipText(1, entry.MonsterSummary);
            item.SetMetadata(0, entry.EncounterId);
        }
    }

    private TreeItem CreateGroup(TreeItem root, string label)
    {
        TreeItem group = _tree.CreateItem(root);
        group.SetText(0, label);
        group.SetSelectable(0, false);
        group.SetSelectable(1, false);
        group.Collapsed = false;
        return group;
    }

    private void RefreshTree()
    {
        TreeItem? root = _tree.GetRoot();
        if (root == null)
        {
            UpdateCountLabel(0);
            return;
        }

        string query = _searchInput.Text?.Trim() ?? string.Empty;
        int visibleCount = 0;

        for (TreeItem? actGroup = root.GetFirstChild(); actGroup != null; actGroup = actGroup.GetNext())
        {
            int actVisibleCount = 0;
            for (TreeItem? categoryGroup = actGroup.GetFirstChild(); categoryGroup != null; categoryGroup = categoryGroup.GetNext())
            {
                int categoryVisibleCount = 0;
                for (TreeItem? item = categoryGroup.GetFirstChild(); item != null; item = item.GetNext())
                {
                    string encounterId = item.GetMetadata(0).AsString();
                    bool allowed = BanEnemyConfigStore.IsEncounterAllowed(encounterId);
                    if (item.IsChecked(0) != allowed)
                    {
                        item.SetChecked(0, allowed);
                    }

                    bool matches = string.IsNullOrEmpty(query)
                        || item.GetText(0).Contains(query, StringComparison.OrdinalIgnoreCase)
                        || item.GetText(1).Contains(query, StringComparison.OrdinalIgnoreCase)
                        || categoryGroup.GetText(0).Contains(query, StringComparison.OrdinalIgnoreCase)
                        || actGroup.GetText(0).Contains(query, StringComparison.OrdinalIgnoreCase)
                        || encounterId.Contains(query, StringComparison.OrdinalIgnoreCase);

                    item.Visible = matches;
                    if (matches)
                    {
                        visibleCount++;
                        categoryVisibleCount++;
                        actVisibleCount++;
                    }
                }

                categoryGroup.Visible = categoryVisibleCount > 0;
            }

            actGroup.Visible = actVisibleCount > 0;
        }

        UpdateCountLabel(visibleCount);
        _modeLabel.Text = BanEnemyConfigStore.GetStatusText();
        _selectAllButton.Disabled = !BanEnemyConfigStore.IsEditingEnabled;
        _deselectAllButton.Disabled = !BanEnemyConfigStore.IsEditingEnabled;
    }

    private void UpdateCountLabel(int visibleCount)
    {
        _countLabel.Text =
            BanEnemyText.Get("count", visibleCount, BanEnemyConfigStore.BannedEncounterIds.Count);
    }

    private void SelectAll()
    {
        if (!BanEnemyConfigStore.IsEditingEnabled)
        {
            return;
        }

        ClearWarning();
        foreach (EncounterCatalog.EncounterEntry entry in _entries)
        {
            BanEnemyConfigStore.SetEncounterAllowed(entry.EncounterId, allowed: true);
        }

        RefreshTree();
    }

    private void DeselectAllSafely()
    {
        if (!BanEnemyConfigStore.IsEditingEnabled)
        {
            return;
        }

        ClearWarning();
        HashSet<string> keepEncounterIds = _entries
            .GroupBy(entry => (entry.MapId, entry.Category))
            .Select(group => group.OrderBy(entry => entry.EncounterTitle, StringComparer.Ordinal).First().EncounterId)
            .ToHashSet(StringComparer.Ordinal);

        foreach (EncounterCatalog.EncounterEntry entry in _entries)
        {
            bool shouldKeep = keepEncounterIds.Contains(entry.EncounterId);
            BanEnemyConfigStore.SetEncounterAllowed(entry.EncounterId, shouldKeep);
        }

        ShowWarning(BanEnemyText.Get("warn.group_generic"));
        RefreshTree();
    }

    private void OnTreeItemEdited()
    {
        TreeItem? item = _tree.GetEdited();
        if (item == null)
        {
            return;
        }

        Variant metadata = item.GetMetadata(0);
        if (metadata.VariantType == Variant.Type.Nil)
        {
            return;
        }

        string encounterId = metadata.AsString();
        if (!BanEnemyConfigStore.IsEditingEnabled)
        {
            RefreshTree();
            return;
        }

        bool allowed = item.IsChecked(0);
        if (!allowed && !CanDisableEncounter(encounterId))
        {
            item.SetChecked(0, true);
            if (_entryById.TryGetValue(encounterId, out EncounterCatalog.EncounterEntry? entry))
            {
                ShowWarning(BanEnemyText.Get("warn.group_min", entry.MapLabel, entry.Category));
            }
            else
            {
                ShowWarning(BanEnemyText.Get("warn.group_generic"));
            }

            HookTrace.Write($"Encounter selection blocked. encounterId={encounterId}, reason=last_allowed_in_group");
            RefreshTree();
            return;
        }

        ClearWarning();
        BanEnemyConfigStore.SetEncounterAllowed(encounterId, allowed);
        HookTrace.Write($"Encounter selection changed. encounterId={encounterId}, allowed={allowed}");
        RefreshTree();
    }

    private void OnTreeGuiInput(InputEvent inputEvent)
    {
        if (inputEvent is not InputEventMouseMotion mouseMotion)
        {
            return;
        }

        TreeItem? hoveredItem = _tree.GetItemAtPosition(mouseMotion.Position);
        if (hoveredItem == null)
        {
            ShowPreview(null);
            return;
        }

        Variant metadata = hoveredItem.GetMetadata(0);
        if (metadata.VariantType == Variant.Type.Nil)
        {
            ShowPreview(null);
            return;
        }

        string encounterId = metadata.AsString();
        EncounterCatalog.EncounterEntry? entry = _entries.FirstOrDefault(e => e.EncounterId == encounterId);
        ShowPreview(entry);
    }

    private void ShowPreview(EncounterCatalog.EncounterEntry? entry)
    {
        if (entry?.EncounterId == _currentPreviewEncounterId)
        {
            return;
        }

        _currentPreviewEncounterId = entry?.EncounterId;
        foreach (Node child in _previewMonsterList.GetChildren())
        {
            child.QueueFree();
        }

        if (entry == null)
        {
            _previewTitle.Text = BanEnemyText.Get("preview.title");
            _previewSubtitle.Text = BanEnemyText.Get("preview.empty");
            _previewTitle.ApplyLocaleFontSubstitution(FontType.Regular, ThemeConstants.Label.font);
            _previewSubtitle.ApplyLocaleFontSubstitution(FontType.Regular, ThemeConstants.Label.font);
            return;
        }

        _previewTitle.Text = entry.EncounterTitle;
        _previewSubtitle.Text = $"{entry.MapLabel} | {entry.Category}";

        for (int i = 0; i < entry.MonsterIds.Count; i++)
        {
            string monsterId = entry.MonsterIds[i];
            string monsterTitle = entry.MonsterTitles.ElementAtOrDefault(i) ?? monsterId;
            AddMonsterPreview(monsterId, monsterTitle);
        }
    }

    private void AddMonsterPreview(string monsterId, string monsterTitle)
    {
        PanelContainer panel = new();
        panel.ClipContents = true;
        panel.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
        _previewMonsterList.AddChild(panel);

        MarginContainer margin = new();
        margin.AddThemeConstantOverride("margin_left", 10);
        margin.AddThemeConstantOverride("margin_top", 10);
        margin.AddThemeConstantOverride("margin_right", 10);
        margin.AddThemeConstantOverride("margin_bottom", 10);
        panel.AddChild(margin);

        VBoxContainer content = new()
        {
            SizeFlagsHorizontal = Control.SizeFlags.ExpandFill
        };
        content.AddThemeConstantOverride("separation", 8);
        margin.AddChild(content);

        Label title = new()
        {
            Text = monsterTitle,
            AutowrapMode = TextServer.AutowrapMode.WordSmart
        };
        title.ApplyLocaleFontSubstitution(FontType.Regular, ThemeConstants.Label.font);
        content.AddChild(title);

        Control visualHolder = new()
        {
            CustomMinimumSize = new Vector2(0, 360),
            SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
            ClipContents = true
        };
        content.AddChild(visualHolder);

        try
        {
            MonsterModel monster = ModelDb.GetById<MonsterModel>(ModelId.Deserialize(monsterId));
            NCreatureVisuals visuals = monster.CreateVisuals();
            visualHolder.AddChild(visuals);

            visualHolder.Resized += () => LayoutMonsterPreview(visualHolder, visuals);
            visuals.Ready += () => LayoutMonsterPreview(visualHolder, visuals);
            if (visuals.HasSpineAnimation && visuals.SpineBody != null)
            {
                monster.GenerateAnimator(visuals.SpineBody);
                visuals.SetUpSkin(monster);
            }

            Callable.From(() => LayoutMonsterPreview(visualHolder, visuals)).CallDeferred();
        }
        catch (Exception ex)
        {
            Label fallback = new()
            {
                Text = $"Preview unavailable for {monsterTitle}\n{ex.Message}",
                AutowrapMode = TextServer.AutowrapMode.WordSmart
            };
            fallback.ApplyLocaleFontSubstitution(FontType.Regular, ThemeConstants.Label.font);
            content.AddChild(fallback);
            HookTrace.Write($"Failed to create monster preview. monsterId={monsterId}, error={ex}");
        }
    }

    private static void LayoutMonsterPreview(Control visualHolder, NCreatureVisuals visuals)
    {
        if (!GodotObject.IsInstanceValid(visualHolder) || !GodotObject.IsInstanceValid(visuals))
        {
            return;
        }

        Control bounds = visuals.Bounds;
        if (!GodotObject.IsInstanceValid(bounds) || bounds.Size.X <= 0 || bounds.Size.Y <= 0)
        {
            return;
        }

        Vector2 available = visualHolder.Size - new Vector2(48f, 48f);
        if (available.X <= 0 || available.Y <= 0)
        {
            return;
        }

        float scale = Mathf.Min(Mathf.Min(available.X / bounds.Size.X, available.Y / bounds.Size.Y), 1f) * 0.95f;
        visuals.Scale = Vector2.One * scale;
        Vector2 boundsCenter = bounds.Position + (bounds.Size * 0.5f);
        visuals.Position = (visualHolder.Size * 0.5f) - (boundsCenter * scale);
    }

    private int CalculateEncounterColumnWidth()
    {
        int maxChars = _entries.Count == 0 ? 18 : _entries.Max(entry => entry.EncounterTitle.Length);
        return Mathf.Clamp((maxChars * 12) + 28, 180, 320);
    }

    private void RefreshLocalizedState()
    {
        RefreshCatalog();
        _titleLabel.Text = BanEnemyText.Get("title");
        _titleLabel.ApplyLocaleFontSubstitution(FontType.Regular, ThemeConstants.Label.font);
        _countLabel.ApplyLocaleFontSubstitution(FontType.Regular, ThemeConstants.Label.font);
        _modeLabel.ApplyLocaleFontSubstitution(FontType.Regular, ThemeConstants.Label.font);
        _warningLabel.ApplyLocaleFontSubstitution(FontType.Regular, ThemeConstants.Label.font);
        _ruleHintLabel.ApplyLocaleFontSubstitution(FontType.Regular, ThemeConstants.Label.font);
        _searchInput.PlaceholderText = BanEnemyText.Get("search.placeholder");
        _searchInput.ApplyLocaleFontSubstitution(FontType.Regular, ThemeConstants.LineEdit.font);
        _selectAllButton.Text = BanEnemyText.Get("select_all");
        _selectAllButton.ApplyLocaleFontSubstitution(FontType.Regular, ThemeConstants.Label.font);
        _deselectAllButton.Text = BanEnemyText.Get("deselect_all");
        _deselectAllButton.ApplyLocaleFontSubstitution(FontType.Regular, ThemeConstants.Label.font);
        _closeButton.Text = BanEnemyText.Get("close");
        _closeButton.ApplyLocaleFontSubstitution(FontType.Regular, ThemeConstants.Label.font);
        _ruleHintLabel.Text = BanEnemyText.Get("rule_hint");
        _tree.SetColumnTitle(0, BanEnemyText.Get("column.encounter"));
        _tree.SetColumnTitle(1, BanEnemyText.Get("column.monsters"));
        _tree.ApplyLocaleFontSubstitution(FontType.Regular, new StringName("font"));
        _tree.ApplyLocaleFontSubstitution(FontType.Regular, new StringName("title_button_font"));

        if (_currentPreviewEncounterId == null)
        {
            _previewTitle.Text = BanEnemyText.Get("preview.title");
            _previewSubtitle.Text = BanEnemyText.Get("preview.empty");
        }

        _previewTitle.ApplyLocaleFontSubstitution(FontType.Regular, ThemeConstants.Label.font);
        _previewSubtitle.ApplyLocaleFontSubstitution(FontType.Regular, ThemeConstants.Label.font);
    }

    private void RefreshCatalog()
    {
        _entries = EncounterCatalog.Build();
        _entryById = _entries.ToDictionary(entry => entry.EncounterId, StringComparer.Ordinal);
        _encounterColumnWidth = CalculateEncounterColumnWidth();
        if (_tree != null)
        {
            _tree.SetColumnCustomMinimumWidth(0, _encounterColumnWidth);
        }
    }

    private bool CanDisableEncounter(string encounterId)
    {
        if (!_entryById.TryGetValue(encounterId, out EncounterCatalog.EncounterEntry? entry))
        {
            return true;
        }

        int remainingAllowed = _entries.Count(candidate =>
            candidate.MapId == entry.MapId
            && string.Equals(candidate.Category, entry.Category, StringComparison.Ordinal)
            && candidate.EncounterId != encounterId
            && BanEnemyConfigStore.IsEncounterAllowed(candidate.EncounterId));

        return remainingAllowed > 0;
    }

    private void ShowWarning(string message)
    {
        _warningLabel.Text = message;
        _warningLabel.Visible = true;
    }

    private void ClearWarning()
    {
        _warningLabel.Text = string.Empty;
        _warningLabel.Visible = false;
    }

    private static string Shorten(string text, int maxChars)
    {
        if (string.IsNullOrWhiteSpace(text) || text.Length <= maxChars)
        {
            return text;
        }

        return text[..(maxChars - 1)] + "…";
    }
}
