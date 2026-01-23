using System;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace MaxQuality;

public class ModEntry : Mod
{
    internal static ModConfig Config { get; private set; } = new();

    public override void Entry(IModHelper helper)
    {
        Config = helper.ReadConfig<ModConfig>();

        helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
        helper.Events.Input.ButtonPressed += this.OnButtonPressed;

        helper.ConsoleCommands.Add(
            name: "maxquality_apply",
            documentation: "Apply max quality to player inventory and open chest menu (if any).",
            callback: (_, _) => this.ApplyNow(showHudMessage: true)
        );
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
        if (configMenu is null)
        {
            this.Monitor.Log("Generic Mod Config Menu not found. Hotkey can only be edited via config.json", LogLevel.Info);
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
            text: () => "Max Quality"
        );

        configMenu.AddParagraph(
            mod: this.ModManifest,
            text: () => "This mod does not auto-change quality. Use the hotkey to apply iridium quality to your inventory and any open chest menu."
        );

        configMenu.AddKeybindList(
            mod: this.ModManifest,
            name: () => "Apply Hotkey",
            tooltip: () => "Applies iridium quality to items in your inventory + open chest.",
            getValue: () => Config.ApplyKey,
            setValue: value => Config.ApplyKey = value
        );
    }

    private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
    {
        if (!Context.IsWorldReady)
            return;

        // Manual apply (no automatic quality forcing).
        if (Config.ApplyKey.JustPressed())
        {
            this.ApplyNow(showHudMessage: true);
            return;
        }
    }

    private void ApplyNow(bool showHudMessage)
    {
        if (!Context.IsWorldReady)
        {
            if (showHudMessage)
                this.Monitor.Log("Max Quality: world not ready.", LogLevel.Info);
            return;
        }

        int changedCount = 0;

        changedCount += ApplyToItemList(Game1.player?.Items);

        if (Game1.activeClickableMenu is ItemGrabMenu grabMenu)
            changedCount += ApplyToItemList(grabMenu.ItemsToGrabMenu?.actualInventory);

        if (showHudMessage)
            this.ShowHudMessage(changedCount > 0 ? $"Max Quality applied to {changedCount} item(s)." : "Max Quality: nothing to change.");
    }

    private void ShowHudMessage(string message)
    {
        // Guard: some calls can happen during transitions.
        if (!Context.IsWorldReady)
        {
            this.Monitor.Log(message, LogLevel.Info);
            return;
        }

        Game1.addHUDMessage(new HUDMessage(message));
    }

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
        // Only force quality for items which can reasonably have quality in vanilla.
        // This avoids affecting junk/resources and other non-quality objects.
        return obj.isForage()
            || obj.Category == StardewValley.Object.FishCategory
            || obj.Category == StardewValley.Object.EggCategory
            || obj.Category == StardewValley.Object.MilkCategory
            || obj.Category == StardewValley.Object.artisanGoodsCategory
            || obj.Category == StardewValley.Object.meatCategory
            // Rabbit's Foot + Wool, etc.
            || obj.Category == StardewValley.Object.sellAtPierres
            || obj.Category == StardewValley.Object.sellAtPierresAndMarnies;
    }
}
