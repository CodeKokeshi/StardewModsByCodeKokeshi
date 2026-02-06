#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.GameData.Machines;
using StardewValley.Locations;
using StardewValley.Inventories;
using StardewValley.Menus;
using StardewValley.Network;
using SObject = StardewValley.Object;

namespace WorldCheats
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        /*********
        ** Public Fields
        *********/
        internal static ModConfig Config { get; private set; } = null!;
        internal static IMonitor ModMonitor { get; private set; } = null!;
        internal static IModHelper ModHelper { get; private set; } = null!;

        private Harmony? harmony;
        private IGenericModConfigMenuApi? gmcmApi;

        /*********
        ** Public Methods
        *********/
        public override void Entry(IModHelper helper)
        {
            ModMonitor = this.Monitor;
            ModHelper = helper;
            Config = helper.ReadConfig<ModConfig>();

            harmony = new Harmony(this.ModManifest.UniqueID);
            ApplyPatches();

            // Register events
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.DayStarted += OnDayStarted;
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
            helper.Events.GameLoop.OneSecondUpdateTicked += OnOneSecondUpdateTicked;
            helper.Events.World.BuildingListChanged += OnBuildingListChanged;
            helper.Events.Input.ButtonPressed += OnButtonPressed;

            Monitor.Log("[WorldCheats] Loaded! Configure in GMCM.", LogLevel.Info);
        }

        /*********
        ** Private Methods — Harmony Patching
        *********/
        private void ApplyPatches()
        {
            if (harmony == null) return;

            void TryPatch(MethodBase? original, HarmonyMethod? prefix = null, HarmonyMethod? postfix = null, string description = "")
            {
                if (original == null)
                {
                    Monitor.Log($"[WorldCheats] WARNING: Could not find method to patch: {description}", LogLevel.Warn);
                    return;
                }
                try
                {
                    harmony.Patch(original, prefix: prefix, postfix: postfix);
                    Monitor.Log($"[WorldCheats] Patched: {description}", LogLevel.Trace);
                }
                catch (Exception ex)
                {
                    Monitor.Log($"[WorldCheats] ERROR patching {description}: {ex.Message}", LogLevel.Error);
                }
            }

            // === Instant Building Construction (intercept MarkUnderConstruction) ===
            TryPatch(
                original: AccessTools.Method(typeof(NetWorldState), nameof(NetWorldState.MarkUnderConstruction)),
                prefix: new HarmonyMethod(typeof(WorldPatches), nameof(WorldPatches.MarkUnderConstruction_Prefix)),
                description: "NetWorldState.MarkUnderConstruction"
            );

            // === Free Building Construction (skip resource consumption) ===
            TryPatch(
                original: AccessTools.Method(typeof(CarpenterMenu), "ConsumeResources"),
                prefix: new HarmonyMethod(typeof(WorldPatches), nameof(WorldPatches.CarpenterMenu_ConsumeResources_Prefix)),
                description: "CarpenterMenu.ConsumeResources"
            );

            TryPatch(
                original: AccessTools.Method(typeof(CarpenterMenu), "DoesFarmerHaveEnoughResourcesToBuild"),
                postfix: new HarmonyMethod(typeof(WorldPatches), nameof(WorldPatches.CarpenterMenu_DoesFarmerHaveEnoughResources_Postfix)),
                description: "CarpenterMenu.DoesFarmerHaveEnoughResourcesToBuild"
            );

            // === Instant Machine Processing ===
            TryPatch(
                original: AccessTools.Method(typeof(SObject), nameof(SObject.OutputMachine),
                    new Type[] { typeof(MachineData), typeof(MachineOutputRule), typeof(Item), typeof(Farmer), typeof(GameLocation), typeof(bool), typeof(bool) }),
                postfix: new HarmonyMethod(typeof(WorldPatches), nameof(WorldPatches.Object_OutputMachine_Postfix)),
                description: "Object.OutputMachine"
            );

            // === Free Shop Purchases ===
            TryPatch(
                original: AccessTools.Method(typeof(ShopMenu), "chargePlayer",
                    new Type[] { typeof(Farmer), typeof(int), typeof(int) }),
                prefix: new HarmonyMethod(typeof(WorldPatches), nameof(WorldPatches.ShopMenu_ChargePlayer_Prefix)),
                description: "ShopMenu.chargePlayer"
            );

            TryPatch(
                original: AccessTools.Method(typeof(ShopMenu), "getPlayerCurrencyAmount",
                    new Type[] { typeof(Farmer), typeof(int) }),
                postfix: new HarmonyMethod(typeof(WorldPatches), nameof(WorldPatches.ShopMenu_GetPlayerCurrencyAmount_Postfix)),
                description: "ShopMenu.getPlayerCurrencyAmount"
            );

            // === Free Crafting ===
            TryPatch(
                original: AccessTools.Method(typeof(CraftingRecipe), nameof(CraftingRecipe.consumeIngredients),
                    new Type[] { typeof(List<IInventory>) }),
                prefix: new HarmonyMethod(typeof(WorldPatches), nameof(WorldPatches.CraftingRecipe_ConsumeIngredients_Prefix)),
                description: "CraftingRecipe.consumeIngredients"
            );

            TryPatch(
                original: AccessTools.Method(typeof(CraftingRecipe), nameof(CraftingRecipe.doesFarmerHaveIngredientsInInventory),
                    new Type[] { typeof(IList<Item>) }),
                postfix: new HarmonyMethod(typeof(WorldPatches), nameof(WorldPatches.CraftingRecipe_DoesHaveIngredients_Postfix)),
                description: "CraftingRecipe.doesFarmerHaveIngredientsInInventory"
            );

            // === Free Geode Processing ===
            TryPatch(
                original: AccessTools.Method(typeof(GeodeMenu), "startGeodeCrack"),
                prefix: new HarmonyMethod(typeof(WorldPatches), nameof(WorldPatches.GeodeMenu_StartGeodeCrack_Prefix)),
                postfix: new HarmonyMethod(typeof(WorldPatches), nameof(WorldPatches.GeodeMenu_StartGeodeCrack_Postfix)),
                description: "GeodeMenu.startGeodeCrack"
            );

            Monitor.Log("[WorldCheats] All Harmony patches applied!", LogLevel.Debug);
        }

        /*********
        ** Private Methods — Event Handlers
        *********/
        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            SetupGMCM();
        }

        private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
        {
            if (!Config.ModEnabled)
                return;

            // Instant tool upgrade on save load
            if (Config.InstantToolUpgrade)
            {
                if (Game1.player.toolBeingUpgraded.Value != null && Game1.player.daysLeftForToolUpgrade.Value > 0)
                {
                    Monitor.Log($"Save loaded: Completing tool upgrade for {Game1.player.toolBeingUpgraded.Value.DisplayName}.", LogLevel.Info);
                    Game1.player.daysLeftForToolUpgrade.Value = 0;
                }
            }

            // Instant building construction on save load
            if (Config.InstantBuildConstruction || Config.InstantBuildUpgrade)
            {
                WorldPatches.CompleteAllBuildings();
            }

            // Instant house upgrade on save load
            if (Config.InstantHouseUpgrade)
            {
                WorldPatches.CompleteHouseUpgrades();
            }

            // Instant community upgrade on save load
            if (Config.InstantCommunityUpgrade)
            {
                WorldPatches.CompleteCommunityUpgrades();
            }
        }

        private void OnDayStarted(object? sender, DayStartedEventArgs e)
        {
            if (!Config.ModEnabled)
                return;

            // Instant tool upgrade
            if (Config.InstantToolUpgrade)
            {
                if (Game1.player.toolBeingUpgraded.Value != null && Game1.player.daysLeftForToolUpgrade.Value > 0)
                {
                    Monitor.Log($"Day start: Completing tool upgrade for {Game1.player.toolBeingUpgraded.Value.DisplayName}.", LogLevel.Info);
                    Game1.player.daysLeftForToolUpgrade.Value = 0;
                }
            }

            // Instant building construction
            if (Config.InstantBuildConstruction || Config.InstantBuildUpgrade)
            {
                WorldPatches.CompleteAllBuildings();
            }

            // Instant house upgrade
            if (Config.InstantHouseUpgrade)
            {
                WorldPatches.CompleteHouseUpgrades();
            }

            // Instant community upgrade
            if (Config.InstantCommunityUpgrade)
            {
                WorldPatches.CompleteCommunityUpgrades();
            }

            // Weather control
            WorldPatches.ApplyWeatherOverride();
        }

        private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady || !Config.ModEnabled)
                return;

            // Check tool upgrades every quarter second
            if (Config.InstantToolUpgrade && e.IsMultipleOf(15))
            {
                if (Game1.player.toolBeingUpgraded.Value != null && Game1.player.daysLeftForToolUpgrade.Value > 0)
                {
                    Monitor.Log($"Tick: Completing tool upgrade for {Game1.player.toolBeingUpgraded.Value.DisplayName}.", LogLevel.Debug);
                    Game1.player.daysLeftForToolUpgrade.Value = 0;
                }
            }

            // Check house and community upgrades every quarter second
            if (e.IsMultipleOf(15))
            {
                if (Config.InstantHouseUpgrade)
                {
                    WorldPatches.CompleteHouseUpgrades();
                }

                if (Config.InstantCommunityUpgrade)
                {
                    WorldPatches.CompleteCommunityUpgrades();
                }
            }
        }

        private void OnOneSecondUpdateTicked(object? sender, OneSecondUpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady || !Config.ModEnabled)
                return;

            // Complete all machines every second if instant machines is enabled
            if (Config.InstantMachineProcessing)
            {
                WorldPatches.CompleteAllMachines();
            }
        }

        private void OnBuildingListChanged(object? sender, BuildingListChangedEventArgs e)
        {
            if (!Config.ModEnabled)
                return;

            if ((Config.InstantBuildConstruction || Config.InstantBuildUpgrade) && e.Added.Any())
            {
                WorldPatches.CompleteAllBuildings();
            }
        }

        private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            if (Config.OpenMenuKey.JustPressed())
            {
                if (gmcmApi != null)
                {
                    gmcmApi.OpenModMenu(this.ModManifest);
                }
                else
                {
                    Monitor.Log("Cannot open config menu - GMCM is not installed.", LogLevel.Warn);
                }
            }
        }

        /*********
        ** Private Methods — GMCM Setup
        *********/
        private void SetupGMCM()
        {
            var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
            {
                Monitor.Log("Generic Mod Config Menu not found. Config can only be edited via config.json.", LogLevel.Info);
                return;
            }

            gmcmApi = configMenu;

            var i18n = this.Helper.Translation;

            // Register mod
            configMenu.Register(
                mod: this.ModManifest,
                reset: () => Config = new ModConfig(),
                save: () => this.Helper.WriteConfig(Config),
                titleScreenOnly: false
            );

            // === Master Toggle ===
            configMenu.AddSectionTitle(
                mod: this.ModManifest,
                text: () => i18n.Get("section.master")
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => i18n.Get("option.modEnabled.name"),
                tooltip: () => i18n.Get("option.modEnabled.tooltip"),
                getValue: () => Config.ModEnabled,
                setValue: value => Config.ModEnabled = value
            );

            configMenu.AddKeybindList(
                mod: this.ModManifest,
                name: () => i18n.Get("option.openMenuKey.name"),
                tooltip: () => i18n.Get("option.openMenuKey.tooltip"),
                getValue: () => Config.OpenMenuKey,
                setValue: value => Config.OpenMenuKey = value
            );

            // === Buildings & Construction ===
            configMenu.AddSectionTitle(
                mod: this.ModManifest,
                text: () => i18n.Get("section.buildings")
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => i18n.Get("option.instantBuildConstruction.name"),
                tooltip: () => i18n.Get("option.instantBuildConstruction.tooltip"),
                getValue: () => Config.InstantBuildConstruction,
                setValue: value => Config.InstantBuildConstruction = value
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => i18n.Get("option.instantBuildUpgrade.name"),
                tooltip: () => i18n.Get("option.instantBuildUpgrade.tooltip"),
                getValue: () => Config.InstantBuildUpgrade,
                setValue: value => Config.InstantBuildUpgrade = value
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => i18n.Get("option.instantHouseUpgrade.name"),
                tooltip: () => i18n.Get("option.instantHouseUpgrade.tooltip"),
                getValue: () => Config.InstantHouseUpgrade,
                setValue: value => Config.InstantHouseUpgrade = value
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => i18n.Get("option.instantCommunityUpgrade.name"),
                tooltip: () => i18n.Get("option.instantCommunityUpgrade.tooltip"),
                getValue: () => Config.InstantCommunityUpgrade,
                setValue: value => Config.InstantCommunityUpgrade = value
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => i18n.Get("option.freeBuildingConstruction.name"),
                tooltip: () => i18n.Get("option.freeBuildingConstruction.tooltip"),
                getValue: () => Config.FreeBuildingConstruction,
                setValue: value => Config.FreeBuildingConstruction = value
            );

            // === Tool Upgrades ===
            configMenu.AddSectionTitle(
                mod: this.ModManifest,
                text: () => i18n.Get("section.toolUpgrades")
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => i18n.Get("option.instantToolUpgrade.name"),
                tooltip: () => i18n.Get("option.instantToolUpgrade.tooltip"),
                getValue: () => Config.InstantToolUpgrade,
                setValue: value => Config.InstantToolUpgrade = value
            );

            // === Machines & Processing ===
            configMenu.AddSectionTitle(
                mod: this.ModManifest,
                text: () => i18n.Get("section.machines")
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => i18n.Get("option.instantMachineProcessing.name"),
                tooltip: () => i18n.Get("option.instantMachineProcessing.tooltip"),
                getValue: () => Config.InstantMachineProcessing,
                setValue: value => Config.InstantMachineProcessing = value
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => i18n.Get("option.freeGeodeProcessing.name"),
                tooltip: () => i18n.Get("option.freeGeodeProcessing.tooltip"),
                getValue: () => Config.FreeGeodeProcessing,
                setValue: value => Config.FreeGeodeProcessing = value
            );

            // === Shopping & Economy ===
            configMenu.AddSectionTitle(
                mod: this.ModManifest,
                text: () => i18n.Get("section.shopping")
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => i18n.Get("option.freeShopPurchases.name"),
                tooltip: () => i18n.Get("option.freeShopPurchases.tooltip"),
                getValue: () => Config.FreeShopPurchases,
                setValue: value => Config.FreeShopPurchases = value
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => i18n.Get("option.freeCrafting.name"),
                tooltip: () => i18n.Get("option.freeCrafting.tooltip"),
                getValue: () => Config.FreeCrafting,
                setValue: value => Config.FreeCrafting = value
            );

            // === Weather Control ===
            configMenu.AddSectionTitle(
                mod: this.ModManifest,
                text: () => i18n.Get("section.weather")
            );

            configMenu.AddTextOption(
                mod: this.ModManifest,
                name: () => i18n.Get("option.weatherForTomorrow.name"),
                tooltip: () => i18n.Get("option.weatherForTomorrow.tooltip"),
                getValue: () => Config.WeatherForTomorrow,
                setValue: value => Config.WeatherForTomorrow = value,
                allowedValues: new string[] { "Default", "Sun", "Rain", "Storm", "Snow", "Wind" },
                formatAllowedValue: value => value switch
                {
                    "Default" => "No Override",
                    "Sun" => "☀ Sunny",
                    "Rain" => "🌧 Rain",
                    "Storm" => "⛈ Thunderstorm",
                    "Snow" => "❄ Snow",
                    "Wind" => "🍃 Windy/Debris",
                    _ => value
                }
            );

            Monitor.Log("[WorldCheats] Generic Mod Config Menu integration complete!", LogLevel.Debug);
        }
    }
}
