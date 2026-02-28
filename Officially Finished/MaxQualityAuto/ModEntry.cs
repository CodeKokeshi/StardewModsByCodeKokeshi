using System;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace MaxQualityAuto;

public class ModEntry : Mod
{
    internal static ModConfig Config { get; private set; } = new();

    /// <summary>Whether we are currently subscribed to the tick event (only while a chest is open).</summary>
    private bool isTickHooked;

    public override void Entry(IModHelper helper)
    {
        // Incompatibility check — only one Max Quality variant should be active.
        if (helper.ModRegistry.IsLoaded("CodeKokeshi.MaxQuality")
            || helper.ModRegistry.IsLoaded("CodeKokeshi.MaxQualityManual"))
        {
            this.Monitor.Log(
                "[Max Quality - Auto] Incompatible mod detected (MaxQuality or MaxQualityManual). "
                + "Disable the other Max Quality mod to use Max Quality - Auto.",
                LogLevel.Error
            );
            return;
        }

        Config = helper.ReadConfig<ModConfig>();

        helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
        helper.Events.Player.InventoryChanged += this.OnInventoryChanged;
        helper.Events.Display.MenuChanged += this.OnMenuChanged;
        // NOTE: UpdateTicked is NOT subscribed here — it is hooked/unhooked
        //       dynamically so there is ZERO overhead when no chest is open.
    }

    // ────────────────────────────────────────────
    //  GMCM registration
    // ────────────────────────────────────────────
    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
        if (configMenu is null)
        {
            this.Monitor.Log("Generic Mod Config Menu not found. Options can only be edited via config.json", LogLevel.Info);
            return;
        }

        configMenu.Register(
            mod: this.ModManifest,
            reset: () => Config = new ModConfig(),
            save: () => this.Helper.WriteConfig(Config),
            titleScreenOnly: false
        );

        configMenu.AddSectionTitle(
            mod: this.ModManifest,
            text: () => "Auto Iridium"
        );

        configMenu.AddParagraph(
            mod: this.ModManifest,
            text: () => "Automatically upgrades eligible items to iridium quality. No hotkey needed — perfect for mobile!"
        );

        configMenu.AddBoolOption(
            mod: this.ModManifest,
            name: () => "Enabled",
            tooltip: () => "Master toggle. Turn this off to completely disable the mod without removing it.",
            getValue: () => Config.Enabled,
            setValue: value => Config.Enabled = value
        );

        configMenu.AddBoolOption(
            mod: this.ModManifest,
            name: () => "Auto-Upgrade Inventory",
            tooltip: () => "Automatically upgrade items whenever they enter your inventory (pick-up, craft, buy, etc.).",
            getValue: () => Config.AutoUpgradeInventory,
            setValue: value => Config.AutoUpgradeInventory = value
        );

        configMenu.AddBoolOption(
            mod: this.ModManifest,
            name: () => "Auto-Upgrade Chests",
            tooltip: () => "Automatically upgrade chest contents when you open a chest and while moving items in/out.",
            getValue: () => Config.AutoUpgradeChests,
            setValue: value => Config.AutoUpgradeChests = value
        );
    }

    // ────────────────────────────────────────────
    //  Inventory changed → auto-upgrade player inventory
    //  ALSO upgrades the open chest (covers item transfers both directions)
    // ────────────────────────────────────────────
    private void OnInventoryChanged(object? sender, InventoryChangedEventArgs e)
    {
        if (!Config.Enabled)
            return;
        if (!e.IsLocalPlayer || !Context.IsWorldReady)
            return;

        if (Config.AutoUpgradeInventory)
            ApplyToItemList(Game1.player?.Items);

        // If a chest is open, an inventory change almost certainly means the
        // player moved something in/out → also scan the chest right now.
        if (Config.AutoUpgradeChests && this.isTickHooked
            && Game1.activeClickableMenu is ItemGrabMenu grabMenu)
        {
            ApplyToItemList(grabMenu.ItemsToGrabMenu?.actualInventory);
        }
    }

    // ────────────────────────────────────────────
    //  Menu opened/closed → auto-upgrade chest
    //  Dynamically hooks/unhooks the tick event
    // ────────────────────────────────────────────
    private void OnMenuChanged(object? sender, MenuChangedEventArgs e)
    {
        // --- Chest opened ---
        if (e.NewMenu is ItemGrabMenu grabMenu)
        {
            if (!this.isTickHooked)
            {
                this.Helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
                this.isTickHooked = true;
            }

            if (!Config.Enabled)
                return;

            // Upgrade chest contents immediately on open.
            if (Config.AutoUpgradeChests)
                ApplyToItemList(grabMenu.ItemsToGrabMenu?.actualInventory);

            // Also upgrade player inventory if that option is on.
            if (Config.AutoUpgradeInventory)
                ApplyToItemList(Game1.player?.Items);
        }
        // --- Chest closed ---
        else if (e.OldMenu is ItemGrabMenu && e.NewMenu is not ItemGrabMenu)
        {
            if (this.isTickHooked)
            {
                this.Helper.Events.GameLoop.UpdateTicked -= this.OnUpdateTicked;
                this.isTickHooked = false;
            }
        }
    }

    // ────────────────────────────────────────────
    //  Fallback tick — ONLY subscribed while a chest is open.
    //  Runs once per second as a safety net for edge cases
    //  (e.g. another mod drops items directly into the chest).
    //  The primary chest scanning happens in OnInventoryChanged.
    // ────────────────────────────────────────────
    private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
    {
        if (!Config.Enabled)
            return;

        // Once per second (60 ticks) — pure safety net, not the main path.
        if (!e.IsMultipleOf(60))
            return;

        if (Game1.activeClickableMenu is ItemGrabMenu grabMenu)
        {
            if (Config.AutoUpgradeChests)
                ApplyToItemList(grabMenu.ItemsToGrabMenu?.actualInventory);

            if (Config.AutoUpgradeInventory)
                ApplyToItemList(Game1.player?.Items);
        }
        else
        {
            // Menu closed without triggering MenuChanged (edge case) → unhook.
            this.Helper.Events.GameLoop.UpdateTicked -= this.OnUpdateTicked;
            this.isTickHooked = false;
        }
    }

    // ────────────────────────────────────────────
    //  Core quality-upgrade logic (same as MaxQuality)
    // ────────────────────────────────────────────
    private static int ApplyToItemList(System.Collections.Generic.IList<Item?>? items)
    {
        if (items == null)
            return 0;

        int changed = 0;

        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] is not StardewValley.Object obj)
                continue;
            if (obj.bigCraftable.Value)
                continue;
            if (obj.QualifiedItemId == "(BC)126")
                continue;
            if (!ShouldForceQuality(obj))
                continue;

            if (obj.Quality != StardewValley.Object.bestQuality)
            {
                obj.Quality = StardewValley.Object.bestQuality;
                changed++;
            }
        }

        return changed;
    }

    private static bool ShouldForceQuality(StardewValley.Object obj)
    {
        return obj.isForage()
            || obj.Category == StardewValley.Object.FishCategory
            || obj.Category == StardewValley.Object.EggCategory
            || obj.Category == StardewValley.Object.MilkCategory
            || obj.Category == StardewValley.Object.artisanGoodsCategory
            || obj.Category == StardewValley.Object.meatCategory
            || obj.Category == StardewValley.Object.sellAtPierres
            || obj.Category == StardewValley.Object.sellAtPierresAndMarnies;
    }
}
