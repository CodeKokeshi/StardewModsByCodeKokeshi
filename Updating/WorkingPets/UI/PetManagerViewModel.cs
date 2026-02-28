#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PropertyChanged.SourceGenerator;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Menus;
using WorkingPets.Behaviors;

namespace WorkingPets.UI;

// ═══════════════════════════════════════════════════════════════════
//  Tab data – one instance per tab icon in the tab bar
// ═══════════════════════════════════════════════════════════════════
internal partial class TabData : INotifyPropertyChanged
{
    public string Name { get; }
    public string DisplayName { get; }
    public Tuple<Texture2D, Rectangle> Sprite { get; }
    [Notify] private bool active;

    public TabData(string name, string displayName, Texture2D texture, Rectangle sourceRect)
    {
        Name = name;
        DisplayName = displayName;
        Sprite = Tuple.Create(texture, sourceRect);
    }
}

// ═══════════════════════════════════════════════════════════════════
//  Per-pet row on the "Manage" tab (state buttons)
// ═══════════════════════════════════════════════════════════════════
internal partial class PetRowViewModel : INotifyPropertyChanged
{
    private readonly Pet _pet;
    private readonly PetWorkManager _manager;
    private readonly PetManagerViewModel _parent;

    // ── Pet info ──
    [Notify] private string displayName = "";
    [Notify] private Tuple<Texture2D, Rectangle>? petSprite;
    [Notify] private string statusText = "";
    [Notify] private string locationText = "";

    // ── Active state flags (true = this mode is active → button disabled) ──
    [Notify] private bool isIdle = true;
    [Notify] private bool isFarmWork;
    [Notify] private bool isValleyWork;
    [Notify] private bool isExploration;
    [Notify] private bool isFollowing;

    // ── Inverse flags (true = button is clickable) ──
    [Notify] private bool canSetIdle;
    [Notify] private bool canSetFarmWork = true;
    [Notify] private bool canSetValleyWork = true;
    [Notify] private bool canSetExploration = true;
    [Notify] private bool canSetFollow = true;

    // ── Button icon sprites ──
    [Notify] private Tuple<Texture2D, Rectangle>? idleIcon;
    [Notify] private Tuple<Texture2D, Rectangle>? farmWorkIcon;
    [Notify] private Tuple<Texture2D, Rectangle>? valleyWorkIcon;
    [Notify] private Tuple<Texture2D, Rectangle>? explorationIcon;
    [Notify] private Tuple<Texture2D, Rectangle>? followIcon;

    // ── Tooltips (clickable buttons) ──
    public string IdleTooltip => T("petManager.tooltip.idle");
    public string FarmWorkTooltip => T("petManager.tooltip.farmWork");
    public string ValleyWorkTooltip => T("petManager.tooltip.valleyWork");
    public string ExplorationTooltip => T("petManager.tooltip.exploration");
    public string FollowTooltip => T("petManager.tooltip.follow");

    // ── Tooltips (active / lit-up state) ──
    public string IdleActiveTooltip => T("petManager.tooltip.idle.active");
    public string FarmWorkActiveTooltip => T("petManager.tooltip.farmWork.active");
    public string ValleyWorkActiveTooltip => T("petManager.tooltip.valleyWork.active");
    public string ExplorationActiveTooltip => T("petManager.tooltip.exploration.active");
    public string FollowActiveTooltip => T("petManager.tooltip.follow.active");

    public PetRowViewModel(Pet pet, PetWorkManager manager, PetManagerViewModel parent)
    {
        _pet = pet;
        _manager = manager;
        _parent = parent;

        DisplayName = string.IsNullOrWhiteSpace(pet.displayName) ? pet.Name : pet.displayName;

        var texture = pet.Sprite.Texture;
        int w = pet.Sprite.SpriteWidth;
        int h = pet.Sprite.SpriteHeight;
        PetSprite = Tuple.Create(texture, new Rectangle(0, 0, w, h));

        LoadIcons();
        RefreshFromManager();
    }

    // ── Button click handlers ──

    public void OnSetIdle()
    {
        if (IsIdle) return;
        _manager.SetState(PetState.Idle);
        _parent.QueueStateNotification(_pet, PetState.Idle);
        _parent.OnPetStateChanged(this);
        Game1.playSound("breathin");
    }

    public void OnSetFarmWork()
    {
        if (IsFarmWork) return;
        _manager.SetState(PetState.FarmWork);
        _parent.QueueStateNotification(_pet, PetState.FarmWork);
        _parent.OnPetStateChanged(this);
        Game1.playSound("breathin");
    }

    public void OnSetValleyWork()
    {
        if (IsValleyWork) return;
        _manager.SetState(PetState.ValleyWork);
        _parent.QueueStateNotification(_pet, PetState.ValleyWork);
        _parent.OnPetStateChanged(this);
        Game1.playSound("breathin");
    }

    public void OnSetExploration()
    {
        if (IsExploration) return;
        _manager.SetState(PetState.Exploration);
        _parent.QueueStateNotification(_pet, PetState.Exploration);
        _parent.OnPetStateChanged(this);
        Game1.playSound("breathin");
    }

    public void OnSetFollow()
    {
        if (IsFollowing) return;
        _manager.SetState(PetState.Following);
        _parent.QueueStateNotification(_pet, PetState.Following);
        _parent.OnPetStateChanged(this);
        Game1.playSound("breathin");
    }

    // ── Helpers ──

    public void RefreshFromManager()
    {
        var state = _manager.CurrentState;

        IsIdle = state == PetState.Idle;
        CanSetIdle = !IsIdle;

        IsFarmWork = state == PetState.FarmWork;
        CanSetFarmWork = !IsFarmWork;

        IsValleyWork = state == PetState.ValleyWork;
        CanSetValleyWork = !IsValleyWork;

        IsExploration = state == PetState.Exploration;
        CanSetExploration = !IsExploration;

        IsFollowing = state == PetState.Following;
        CanSetFollow = !IsFollowing;

        StatusText = state switch
        {
            PetState.Idle => T("petManager.status.idle"),
            PetState.FarmWork => T("petManager.status.farmWork"),
            PetState.ValleyWork => T("petManager.status.valleyWork"),
            PetState.Exploration => T("petManager.status.exploration"),
            PetState.Following => T("petManager.status.following"),
            _ => T("petManager.status.idle")
        };

        string locName = _pet.currentLocation?.DisplayName
            ?? _pet.currentLocation?.Name
            ?? T("petManager.location.unknown");
        LocationText = T("petManager.location", new { location = locName });
    }

    private void LoadIcons()
    {
        var helper = ModEntry.Instance.Helper;
        IdleIcon = LoadIcon(helper, "assets/views/idle_mode.png");
        FarmWorkIcon = LoadIcon(helper, "assets/views/farm_work_mode.png");
        ValleyWorkIcon = LoadIcon(helper, "assets/views/valley_work_mode.png");
        ExplorationIcon = LoadIcon(helper, "assets/views/exploration_mode.png");
        FollowIcon = LoadIcon(helper, "assets/views/follow_mode.png");
    }

    private static Tuple<Texture2D, Rectangle> LoadIcon(IModHelper helper, string path)
    {
        var tex = helper.ModContent.Load<Texture2D>(path);
        return Tuple.Create(tex, new Rectangle(0, 0, tex.Width, tex.Height));
    }

    internal static string T(string key, object? tokens = null)
        => ModEntry.I18n.Get(key, tokens).ToString();
}

// ═══════════════════════════════════════════════════════════════════
//  Per-pet row on the "Pets" tab (rename button)
// ═══════════════════════════════════════════════════════════════════
internal partial class PetRenameRowViewModel : INotifyPropertyChanged
{
    private readonly Pet _pet;
    private readonly PetManagerViewModel _parent;

    [Notify] private string displayName = "";
    [Notify] private Tuple<Texture2D, Rectangle>? petSprite;
    [Notify] private string locationText = "";

    public string RenameTooltip => PetRowViewModel.T("petManager.tooltip.rename");

    public PetRenameRowViewModel(Pet pet, PetManagerViewModel parent)
    {
        _pet = pet;
        _parent = parent;

        RefreshInfo();

        var texture = pet.Sprite.Texture;
        int w = pet.Sprite.SpriteWidth;
        int h = pet.Sprite.SpriteHeight;
        PetSprite = Tuple.Create(texture, new Rectangle(0, 0, w, h));
    }

    public void OnRename()
    {
        string oldName = _pet.Name ?? "Pet";

        Game1.activeClickableMenu = new NamingMenu(
            (string newName) =>
            {
                if (!string.IsNullOrWhiteSpace(newName))
                {
                    _pet.Name = newName;
                    _pet.displayName = newName;
                    DisplayName = newName;

                    Game1.playSound("newArtifact");
                    Game1.addHUDMessage(new HUDMessage(
                        PetRowViewModel.T("hud.rename.success", new { oldName, newName }),
                        HUDMessage.newQuest_type));

                    // Refresh the manage tab rows too so names stay in sync
                    _parent.RefreshAllDisplayNames();
                }
                Game1.exitActiveMenu();
                Game1.player.canMove = true;
                Game1.dialogueUp = false;

                // Re-open the Pet Manager after closing the naming menu
                _parent.ReopenAfterRename();
            },
            Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1236"),
            _pet.Name
        );
        Game1.playSound("bigSelect");
    }

    public void RefreshInfo()
    {
        DisplayName = string.IsNullOrWhiteSpace(_pet.displayName) ? _pet.Name : _pet.displayName;
        string locName = _pet.currentLocation?.DisplayName
            ?? _pet.currentLocation?.Name
            ?? PetRowViewModel.T("petManager.location.unknown");
        LocationText = PetRowViewModel.T("petManager.location", new { location = locName });
    }
}

// ═══════════════════════════════════════════════════════════════════
//  Settings tab ViewModel
// ═══════════════════════════════════════════════════════════════════
internal partial class SettingsViewModel : INotifyPropertyChanged
{
    [Notify] private bool clearDebris;
    [Notify] private bool chopTrees;
    [Notify] private bool clearStumpsAndLogs;
    [Notify] private bool breakBoulders;
    [Notify] private bool showWorkingMessages;
    [Notify] private bool showStateNotifications;
    [Notify] private bool forageWhileFollowing;
    [Notify] private bool followOutsideFarm;
    [Notify] private bool autoDepositToChests;

    // Labels (read-only)
    public string ClearDebrisLabel => PetRowViewModel.T("petManager.settings.clearDebris");
    public string ChopTreesLabel => PetRowViewModel.T("petManager.settings.chopTrees");
    public string ClearStumpsLabel => PetRowViewModel.T("petManager.settings.clearStumps");
    public string BreakBouldersLabel => PetRowViewModel.T("petManager.settings.breakBoulders");
    public string ShowWorkingMsgLabel => PetRowViewModel.T("petManager.settings.showWorkingMsg");
    public string ShowStateNotifLabel => PetRowViewModel.T("petManager.settings.showStateNotif");
    public string ForageFollowLabel => PetRowViewModel.T("petManager.settings.forageFollow");
    public string FollowOutsideLabel => PetRowViewModel.T("petManager.settings.followOutside");
    public string AutoDepositLabel => PetRowViewModel.T("petManager.settings.autoDeposit");

    public SettingsViewModel()
    {
        LoadFromConfig();
    }

    private void LoadFromConfig()
    {
        var c = ModEntry.Config;
        ClearDebris = c.ClearDebris;
        ChopTrees = c.ChopTrees;
        ClearStumpsAndLogs = c.ClearStumpsAndLogs;
        BreakBoulders = c.BreakBoulders;
        ShowWorkingMessages = c.ShowWorkingMessages;
        ShowStateNotifications = c.ShowStateNotifications;
        ForageWhileFollowing = c.ForageWhileFollowing;
        FollowOutsideFarm = c.FollowOutsideFarm;
        AutoDepositToChests = c.AutoDepositToChests;
    }

    /// <summary>Push current toggle values back into ModConfig and save.</summary>
    public void SaveToConfig()
    {
        var c = ModEntry.Config;
        c.ClearDebris = ClearDebris;
        c.ChopTrees = ChopTrees;
        c.ClearStumpsAndLogs = ClearStumpsAndLogs;
        c.BreakBoulders = BreakBoulders;
        c.ShowWorkingMessages = ShowWorkingMessages;
        c.ShowStateNotifications = ShowStateNotifications;
        c.ForageWhileFollowing = ForageWhileFollowing;
        c.FollowOutsideFarm = FollowOutsideFarm;
        c.AutoDepositToChests = AutoDepositToChests;

        ModEntry.Instance.Helper.WriteConfig(c);
    }
}

// ═══════════════════════════════════════════════════════════════════
//  Top-level Pet Manager ViewModel (owns tabs + all sub-models)
// ═══════════════════════════════════════════════════════════════════
internal partial class PetManagerViewModel : INotifyPropertyChanged
{
    // ── Tab bar ──
    public IReadOnlyList<TabData> Tabs { get; set; } = Array.Empty<TabData>();
    [Notify] private string selectedTab = "Manage";

    // ── Shared ──
    [Notify] private string menuTitle = "";
    [Notify] private string noPetsText = "";
    [Notify] private bool hasPets;
    [Notify] private bool hasNoPets = true;

    // ── Manage tab ──
    [Notify] private List<PetRowViewModel> pets = new();

    // ── Pets tab (rename) ──
    [Notify] private List<PetRenameRowViewModel> renamePets = new();

    // ── Settings tab ──
    public SettingsViewModel Settings { get; }

    // ── Queued HUD notifications (shown when menu closes) ──
    private readonly List<(string petName, PetState state)> _pendingNotifications = new();

    public PetManagerViewModel()
    {
        MenuTitle = ModEntry.I18n.Get("petManager.title").ToString();
        NoPetsText = ModEntry.I18n.Get("petManager.noPets").ToString();
        Settings = new SettingsViewModel();

        InitTabs();
        LoadPets();
    }

    // ── Tab switching ──

    public void OnTabActivated(string name)
    {
        foreach (var tab in Tabs)
            if (tab.Name != name)
                tab.Active = false;
        SelectedTab = name;

        // Leaving settings tab → auto-save
        if (name != "Settings")
            Settings.SaveToConfig();

        // Refresh data when switching to a tab
        if (name == "Manage")
            foreach (var row in Pets) row.RefreshFromManager();
        else if (name == "Pets")
            foreach (var row in RenamePets) row.RefreshInfo();
    }

    // ── Public helpers ──

    internal void OnPetStateChanged(PetRowViewModel _)
    {
        foreach (var row in Pets)
            row.RefreshFromManager();
    }

    /// <summary>Queue a state-change notification to show when the menu closes.</summary>
    internal void QueueStateNotification(Pet pet, PetState state)
    {
        string petName = pet?.Name ?? ModEntry.I18n.Get("pet.genericName").ToString();
        // Replace any previous notification for the same pet (only latest matters)
        _pendingNotifications.RemoveAll(n => n.petName == petName);
        _pendingNotifications.Add((petName, state));
    }

    /// <summary>Show all queued HUD messages. Called when the menu closes.</summary>
    internal void FlushNotifications()
    {
        if (!ModEntry.Config.ShowStateNotifications) return;

        foreach (var (petName, state) in _pendingNotifications)
        {
            string key = state switch
            {
                PetState.Idle => "hud.state.idle",
                PetState.FarmWork => "hud.work.start",
                PetState.ValleyWork => "hud.valleyWork.start",
                PetState.Exploration => "hud.explore.start",
                PetState.Following => "hud.follow.start",
                _ => "hud.state.idle"
            };
            Game1.addHUDMessage(new HUDMessage(
                ModEntry.I18n.Get(key, new { petName }).ToString(),
                HUDMessage.newQuest_type));
        }
        _pendingNotifications.Clear();
    }

    internal void RefreshAllDisplayNames()
    {
        foreach (var row in Pets)
            row.RefreshFromManager();
        foreach (var row in RenamePets)
            row.RefreshInfo();
    }

    internal void ReopenAfterRename()
    {
        // Re-open the Pet Manager so we return to the menu after the naming screen
        try
        {
            var engine = ModEntry.Instance.GetViewEngine();
            if (engine == null) return;

            // Build a fresh VM so all names are up-to-date
            var vm = new PetManagerViewModel();
            vm.SelectedTab = "Pets"; // go back to the Pets tab
            foreach (var tab in vm.Tabs)
                tab.Active = tab.Name == "Pets";

            var controller = engine.CreateMenuControllerFromAsset(
                "Mods/CodeKokeshi.WorkingPets/Views/PetManagerMenu", vm);
            controller.EnableCloseButton();
            controller.DimmingAmount = 0.75f;
            Game1.activeClickableMenu = controller.Menu;
        }
        catch { /* swallow – worst case the menu just closes */ }
    }

    // ── Initialization ──

    private void InitTabs()
    {
        var helper = ModEntry.Instance.Helper;

        var petTabTex = helper.ModContent.Load<Texture2D>("assets/views/pet_tab.png");
        var renameTabTex = helper.ModContent.Load<Texture2D>("assets/views/rename_tab.png");
        var settingsTabTex = helper.ModContent.Load<Texture2D>("assets/views/settings_tab.png");

        Tabs = new List<TabData>
        {
            new TabData("Manage",
                PetRowViewModel.T("petManager.tab.manage"),
                petTabTex, new Rectangle(0, 0, petTabTex.Width, petTabTex.Height)),
            new TabData("Pets",
                PetRowViewModel.T("petManager.tab.pets"),
                renameTabTex, new Rectangle(0, 0, renameTabTex.Width, renameTabTex.Height)),
            new TabData("Settings",
                PetRowViewModel.T("petManager.tab.settings"),
                settingsTabTex, new Rectangle(0, 0, settingsTabTex.Width, settingsTabTex.Height)),
        };
        Tabs[0].Active = true;
    }

    private void LoadPets()
    {
        var allPets = MultiPetManager.GetAllPets();
        var manageRows = new List<PetRowViewModel>();
        var renameRows = new List<PetRenameRowViewModel>();

        foreach (var pet in allPets)
        {
            var manager = ModEntry.PetManager?.GetManagerForPet(pet);
            if (manager != null)
                manageRows.Add(new PetRowViewModel(pet, manager, this));
            renameRows.Add(new PetRenameRowViewModel(pet, this));
        }

        Pets = manageRows;
        RenamePets = renameRows;
        HasPets = manageRows.Count > 0;
        HasNoPets = !HasPets;
    }
}
