#nullable enable
using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewUI.Framework;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Monsters;
using StardewValley.Network;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using SObject = StardewValley.Object;

namespace PlayerCheats
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        /*********
        ** Public Fields
        *********/
        /// <summary>Static config instance accessible by the whole mod.</summary>
        internal static ModConfig Config { get; private set; } = null!;

        /// <summary>Static monitor for logging from static methods.</summary>
        internal static IMonitor ModMonitor { get; private set; } = null!;

        /// <summary>Static helper for accessing mod APIs.</summary>
        internal static IModHelper ModHelper { get; private set; } = null!;

        /// <summary>Harmony instance for patching.</summary>
        private Harmony? harmony;

        /// <summary>StardewUI view engine for creating menus.</summary>
        private IViewEngine? viewEngine;

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
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
            helper.Events.GameLoop.OneSecondUpdateTicked += OnOneSecondUpdateTicked;
            helper.Events.GameLoop.DayStarted += OnDayStarted;
            helper.Events.GameLoop.TimeChanged += OnTimeChanged;
            helper.Events.Input.ButtonPressed += OnButtonPressed;
            helper.Events.World.BuildingListChanged += OnBuildingListChanged;

            Monitor.Log("[PlayerCheats] Loaded! Press K to open cheats menu (StardewUI).", LogLevel.Info);
        }

        /*********
        ** Private Methods — Harmony Patches
        *********/
        private void ApplyPatches()
        {
            if (harmony == null) return;

            void TryPatch(MethodBase? original, HarmonyMethod? prefix = null, HarmonyMethod? postfix = null, string description = "")
            {
                if (original == null)
                {
                    Monitor.Log($"[PlayerCheats] WARNING: Could not find method to patch: {description}", LogLevel.Warn);
                    return;
                }
                try
                {
                    harmony.Patch(original, prefix: prefix, postfix: postfix);
                    Monitor.Log($"[PlayerCheats] Patched: {description}", LogLevel.Trace);
                }
                catch (Exception ex)
                {
                    Monitor.Log($"[PlayerCheats] ERROR patching {description}: {ex.Message}", LogLevel.Error);
                }
            }

            // === Player Damage (Invincibility) ===
            TryPatch(
                original: AccessTools.Method(typeof(Farmer), nameof(Farmer.takeDamage)),
                prefix: new HarmonyMethod(typeof(PlayerPatches), nameof(PlayerPatches.Farmer_TakeDamage_Prefix)),
                description: "Farmer.takeDamage"
            );

            // === Movement Speed ===
            TryPatch(
                original: AccessTools.Method(typeof(Farmer), nameof(Farmer.getMovementSpeed)),
                postfix: new HarmonyMethod(typeof(PlayerPatches), nameof(PlayerPatches.Farmer_GetMovementSpeed_Postfix)),
                description: "Farmer.getMovementSpeed"
            );

            // === Magnetic Radius ===
            TryPatch(
                original: AccessTools.PropertyGetter(typeof(Farmer), nameof(Farmer.MagneticRadius)),
                postfix: new HarmonyMethod(typeof(PlayerPatches), nameof(PlayerPatches.Farmer_MagneticRadius_Postfix)),
                description: "Farmer.MagneticRadius getter"
            );

            // === Stamina Consumption ===
            TryPatch(
                original: AccessTools.PropertySetter(typeof(Farmer), nameof(Farmer.stamina)),
                prefix: new HarmonyMethod(typeof(PlayerPatches), nameof(PlayerPatches.Farmer_Stamina_Prefix)),
                description: "Farmer.stamina setter"
            );

            // === Monster One Hit Kill ===
            TryPatch(
                original: AccessTools.Method(typeof(Monster), nameof(Monster.takeDamage), new Type[] { typeof(int), typeof(int), typeof(int), typeof(bool), typeof(double), typeof(Farmer) }),
                prefix: new HarmonyMethod(typeof(PlayerPatches), nameof(PlayerPatches.Monster_TakeDamage_Prefix)),
                description: "Monster.takeDamage"
            );

            // === Weapon Critical Chance ===
            TryPatch(
                original: AccessTools.Method(typeof(MeleeWeapon), nameof(MeleeWeapon.DoDamage)),
                prefix: new HarmonyMethod(typeof(PlayerPatches), nameof(PlayerPatches.MeleeWeapon_DoDamage_Prefix)),
                description: "MeleeWeapon.DoDamage"
            );

            // === Tool Stamina Cost ===
            TryPatch(
                original: AccessTools.Method(typeof(Tool), nameof(Tool.DoFunction)),
                prefix: new HarmonyMethod(typeof(PlayerPatches), nameof(PlayerPatches.Tool_DoFunction_Prefix)),
                description: "Tool.DoFunction"
            );

            // === Hoe/Watering Can Area ===
            TryPatch(
                original: AccessTools.Method(typeof(Tool), "tilesAffected", new Type[] { typeof(Vector2), typeof(int), typeof(Farmer) }),
                postfix: new HarmonyMethod(typeof(PlayerPatches), nameof(PlayerPatches.Tool_TilesAffected_Postfix)),
                description: "Tool.tilesAffected"
            );

            // === Watering Can Infinite Water ===
            TryPatch(
                original: AccessTools.PropertyGetter(typeof(WateringCan), nameof(WateringCan.WaterLeft)),
                postfix: new HarmonyMethod(typeof(PlayerPatches), nameof(PlayerPatches.WateringCan_WaterLeft_Postfix)),
                description: "WateringCan.WaterLeft getter"
            );

            // === Skill Levels ===
            TryPatch(
                original: AccessTools.PropertyGetter(typeof(Farmer), nameof(Farmer.FarmingLevel)),
                postfix: new HarmonyMethod(typeof(PlayerPatches), nameof(PlayerPatches.Farmer_FarmingLevel_Postfix)),
                description: "Farmer.FarmingLevel getter"
            );
            TryPatch(
                original: AccessTools.PropertyGetter(typeof(Farmer), nameof(Farmer.MiningLevel)),
                postfix: new HarmonyMethod(typeof(PlayerPatches), nameof(PlayerPatches.Farmer_MiningLevel_Postfix)),
                description: "Farmer.MiningLevel getter"
            );
            TryPatch(
                original: AccessTools.PropertyGetter(typeof(Farmer), nameof(Farmer.ForagingLevel)),
                postfix: new HarmonyMethod(typeof(PlayerPatches), nameof(PlayerPatches.Farmer_ForagingLevel_Postfix)),
                description: "Farmer.ForagingLevel getter"
            );
            TryPatch(
                original: AccessTools.PropertyGetter(typeof(Farmer), nameof(Farmer.FishingLevel)),
                postfix: new HarmonyMethod(typeof(PlayerPatches), nameof(PlayerPatches.Farmer_FishingLevel_Postfix)),
                description: "Farmer.FishingLevel getter"
            );
            TryPatch(
                original: AccessTools.PropertyGetter(typeof(Farmer), nameof(Farmer.CombatLevel)),
                postfix: new HarmonyMethod(typeof(PlayerPatches), nameof(PlayerPatches.Farmer_CombatLevel_Postfix)),
                description: "Farmer.CombatLevel getter"
            );

            // === XP Gain ===
            TryPatch(
                original: AccessTools.Method(typeof(Farmer), nameof(Farmer.gainExperience)),
                prefix: new HarmonyMethod(typeof(PlayerPatches), nameof(PlayerPatches.Farmer_GainExperience_Prefix)),
                description: "Farmer.gainExperience"
            );

            // === Time Freeze ===
            TryPatch(
                original: AccessTools.Method(typeof(Game1), nameof(Game1.performTenMinuteClockUpdate)),
                prefix: new HarmonyMethod(typeof(PlayerPatches), nameof(PlayerPatches.Game1_PerformTenMinuteClockUpdate_Prefix)),
                description: "Game1.performTenMinuteClockUpdate"
            );

            // === Passout Prevention ===
            TryPatch(
                original: AccessTools.Method(typeof(Farmer), nameof(Farmer.passOutFromTired)),
                prefix: new HarmonyMethod(typeof(PlayerPatches), nameof(PlayerPatches.Farmer_PassOutFromTired_Prefix)),
                description: "Farmer.passOutFromTired"
            );

            // === Friendship Gain ===
            TryPatch(
                original: AccessTools.Method(typeof(Farmer), nameof(Farmer.changeFriendship)),
                prefix: new HarmonyMethod(typeof(PlayerPatches), nameof(PlayerPatches.Farmer_ChangeFriendship_Prefix)),
                description: "Farmer.changeFriendship"
            );

            // === Max Fish Quality ===
            TryPatch(
                original: AccessTools.Method(typeof(FishingRod), nameof(FishingRod.pullFishFromWater)),
                prefix: new HarmonyMethod(typeof(PlayerPatches), nameof(PlayerPatches.FishingRod_PullFishFromWater_Prefix)),
                description: "FishingRod.pullFishFromWater"
            );

            // === Infinite Items ===
            TryPatch(
                original: AccessTools.Method(typeof(Farmer), nameof(Farmer.reduceActiveItemByOne)),
                prefix: new HarmonyMethod(typeof(PlayerPatches), nameof(PlayerPatches.Farmer_ReduceActiveItemByOne_Prefix)),
                description: "Farmer.reduceActiveItemByOne"
            );

            TryPatch(
                original: AccessTools.Method(typeof(Item), nameof(Item.ConsumeStack)),
                prefix: new HarmonyMethod(typeof(PlayerPatches), nameof(PlayerPatches.Item_ConsumeStack_Prefix)),
                description: "Item.ConsumeStack"
            );

            // === Fishing Rod (Instant Bite) ===
            TryPatch(
                original: AccessTools.Method(typeof(FishingRod), nameof(FishingRod.DoFunction)),
                postfix: new HarmonyMethod(typeof(PlayerPatches), nameof(PlayerPatches.FishingRod_DoFunction_Postfix)),
                description: "FishingRod.DoFunction"
            );

            // === Instant Catch & Always Find Treasure ===
            TryPatch(
                original: AccessTools.Method(typeof(FishingRod), nameof(FishingRod.startMinigameEndFunction)),
                prefix: new HarmonyMethod(typeof(PlayerPatches), nameof(PlayerPatches.FishingRod_StartMinigameEndFunction_Prefix)),
                description: "FishingRod.startMinigameEndFunction"
            );

            // === BobberBar Treasure Override ===
            TryPatch(
                original: AccessTools.Constructor(typeof(BobberBar), new Type[] { typeof(string), typeof(float), typeof(bool), typeof(List<string>), typeof(string), typeof(bool), typeof(string), typeof(bool) }),
                postfix: new HarmonyMethod(typeof(PlayerPatches), nameof(PlayerPatches.BobberBar_Constructor_Postfix)),
                description: "BobberBar constructor"
            );

            // === Max Stamina Override ===
            TryPatch(
                original: AccessTools.PropertyGetter(typeof(Farmer), nameof(Farmer.MaxStamina)),
                postfix: new HarmonyMethod(typeof(PlayerPatches), nameof(PlayerPatches.Farmer_MaxStamina_Postfix)),
                description: "Farmer.MaxStamina getter"
            );

            // === Attack ===
            TryPatch(
                original: AccessTools.PropertyGetter(typeof(Farmer), nameof(Farmer.Attack)),
                postfix: new HarmonyMethod(typeof(PlayerPatches), nameof(PlayerPatches.Farmer_Attack_Postfix)),
                description: "Farmer.Attack getter"
            );

            // === Immunity ===
            TryPatch(
                original: AccessTools.PropertyGetter(typeof(Farmer), nameof(Farmer.Immunity)),
                postfix: new HarmonyMethod(typeof(PlayerPatches), nameof(PlayerPatches.Farmer_Immunity_Postfix)),
                description: "Farmer.Immunity getter"
            );

            // === Forage Quality Override ===
            TryPatch(
                original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.GetHarvestSpawnedObjectQuality)),
                postfix: new HarmonyMethod(typeof(PlayerPatches), nameof(PlayerPatches.GameLocation_GetHarvestSpawnedObjectQuality_Postfix)),
                description: "GameLocation.GetHarvestSpawnedObjectQuality"
            );

            // === Sell Price Multiplier ===
            TryPatch(
                original: AccessTools.Method(typeof(StardewValley.Object), nameof(StardewValley.Object.sellToStorePrice)),
                postfix: new HarmonyMethod(typeof(PlayerPatches), nameof(PlayerPatches.Object_SellToStorePrice_Postfix)),
                description: "Object.sellToStorePrice"
            );

            // === Buy Price Multiplier ===
            TryPatch(
                original: AccessTools.Method(typeof(StardewValley.Object), nameof(StardewValley.Object.salePrice)),
                postfix: new HarmonyMethod(typeof(PlayerPatches), nameof(PlayerPatches.Object_SalePrice_Postfix)),
                description: "Object.salePrice"
            );

            // ==================== WORLD CHEAT PATCHES ====================

            // === Instant Building Construction ===
            TryPatch(
                original: AccessTools.Method(typeof(NetWorldState), "MarkUnderConstruction"),
                prefix: new HarmonyMethod(typeof(WorldPatches), nameof(WorldPatches.MarkUnderConstruction_Prefix)),
                description: "NetWorldState.MarkUnderConstruction"
            );

            // === Free Building Construction (skip resources) ===
            TryPatch(
                original: AccessTools.Method(typeof(CarpenterMenu), "ConsumeResources"),
                prefix: new HarmonyMethod(typeof(WorldPatches), nameof(WorldPatches.CarpenterMenu_ConsumeResources_Prefix)),
                description: "CarpenterMenu.ConsumeResources"
            );

            // === Free Building Construction (always have enough) ===
            TryPatch(
                original: AccessTools.Method(typeof(CarpenterMenu), "DoesFarmerHaveEnoughResourcesToBuild"),
                postfix: new HarmonyMethod(typeof(WorldPatches), nameof(WorldPatches.CarpenterMenu_DoesFarmerHaveEnoughResources_Postfix)),
                description: "CarpenterMenu.DoesFarmerHaveEnoughResourcesToBuild"
            );

            // === Instant Machine Processing ===
            TryPatch(
                original: AccessTools.Method(typeof(SObject), nameof(SObject.OutputMachine)),
                postfix: new HarmonyMethod(typeof(WorldPatches), nameof(WorldPatches.Object_OutputMachine_Postfix)),
                description: "Object.OutputMachine"
            );

            // === Free Shopping (charge 0) ===
            TryPatch(
                original: AccessTools.Method(typeof(ShopMenu), "chargePlayer"),
                prefix: new HarmonyMethod(typeof(WorldPatches), nameof(WorldPatches.ShopMenu_ChargePlayer_Prefix)),
                description: "ShopMenu.chargePlayer"
            );

            // === Free Shopping (infinite currency) ===
            TryPatch(
                original: AccessTools.Method(typeof(ShopMenu), "getPlayerCurrencyAmount"),
                postfix: new HarmonyMethod(typeof(WorldPatches), nameof(WorldPatches.ShopMenu_GetPlayerCurrencyAmount_Postfix)),
                description: "ShopMenu.getPlayerCurrencyAmount"
            );

            // === Free Crafting (skip ingredient consumption) ===
            TryPatch(
                original: AccessTools.Method(typeof(CraftingRecipe), nameof(CraftingRecipe.consumeIngredients)),
                prefix: new HarmonyMethod(typeof(WorldPatches), nameof(WorldPatches.CraftingRecipe_ConsumeIngredients_Prefix)),
                description: "CraftingRecipe.consumeIngredients"
            );

            // === Free Crafting (always have ingredients) ===
            TryPatch(
                original: AccessTools.Method(typeof(CraftingRecipe), nameof(CraftingRecipe.doesFarmerHaveIngredientsInInventory)),
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

            // === Force Ladder Spawn in Mines ===
            TryPatch(
                original: AccessTools.Method(typeof(MineShaft), nameof(MineShaft.checkStoneForItems)),
                postfix: new HarmonyMethod(typeof(WorldPatches), nameof(WorldPatches.MineShaft_CheckStoneForItems_Postfix)),
                description: "MineShaft.checkStoneForItems"
            );

            // === Give Gifts Anytime (bypass daily/weekly limits) ===
            TryPatch(
                original: AccessTools.PropertyGetter(typeof(Friendship), nameof(Friendship.GiftsToday)),
                postfix: new HarmonyMethod(typeof(PlayerPatches), nameof(PlayerPatches.Friendship_GiftsToday_Postfix)),
                description: "Friendship.GiftsToday getter"
            );

            TryPatch(
                original: AccessTools.PropertyGetter(typeof(Friendship), nameof(Friendship.GiftsThisWeek)),
                postfix: new HarmonyMethod(typeof(PlayerPatches), nameof(PlayerPatches.Friendship_GiftsThisWeek_Postfix)),
                description: "Friendship.GiftsThisWeek getter"
            );

            Monitor.Log("All Harmony patches applied!", LogLevel.Debug);
        }

        /*********
        ** Private Methods — Event Handlers
        *********/
        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            // Initialize StardewUI
            viewEngine = Helper.ModRegistry.GetApi<IViewEngine>("focustense.StardewUI");
            if (viewEngine == null)
            {
                Monitor.Log("StardewUI Framework not found! Cannot create cheats menu.", LogLevel.Error);
                return;
            }

            // Register our view assets
            viewEngine.RegisterViews("Mods/CodeKokeshi.UltimatePlayerCheats/Views", "assets/views");

            Monitor.Log("StardewUI integration complete!", LogLevel.Debug);
        }

        private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
        {
            if (!Config.ModEnabled)
                return;

            // Instant building/upgrade completion on save load
            if (Config.InstantBuildConstruction || Config.InstantBuildUpgrade)
                WorldPatches.CompleteAllBuildings();
            if (Config.InstantHouseUpgrade)
                WorldPatches.CompleteHouseUpgrades();
            if (Config.InstantCommunityUpgrade)
                WorldPatches.CompleteCommunityUpgrades();

            // Instant tool upgrade
            if (Config.InstantToolUpgrade)
                CompleteToolUpgrade();
        }

        private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady || !Config.ModEnabled)
                return;

            var player = Game1.player;
            if (player == null) return;

            // Infinite stamina
            if (Config.InfiniteStamina)
            {
                player.stamina = player.MaxStamina;
            }

            // Infinite health
            if (Config.InfiniteHealth)
            {
                player.health = player.maxHealth;
            }

            // Max health override
            if (Config.MaxHealthOverride > 0)
            {
                player.maxHealth = Config.MaxHealthOverride;
                // Clamp current health to new max
                if (player.health > player.maxHealth)
                    player.health = player.maxHealth;
            }

            // NoClip
            if (Config.NoClip)
            {
                player.ignoreCollisions = true;
            }
            else
            {
                player.ignoreCollisions = false;
            }

            // No monster spawns - remove all monsters
            if (Config.NoMonsterSpawns && e.IsMultipleOf(60))
            {
                var location = player.currentLocation;
                if (location != null)
                {
                    for (int i = location.characters.Count - 1; i >= 0; i--)
                    {
                        if (location.characters[i] is Monster)
                        {
                            location.characters.RemoveAt(i);
                        }
                    }
                }
            }

            // === World Cheat Tick Checks (every 15 ticks) ===
            if (e.IsMultipleOf(15))
            {
                // Instant tool upgrade check
                if (Config.InstantToolUpgrade)
                    CompleteToolUpgrade();

                // Instant house upgrade check
                if (Config.InstantHouseUpgrade)
                    WorldPatches.CompleteHouseUpgrades();

                // Instant community upgrade check
                if (Config.InstantCommunityUpgrade)
                    WorldPatches.CompleteCommunityUpgrades();
            }
        }

        private void OnOneSecondUpdateTicked(object? sender, OneSecondUpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady || !Config.ModEnabled)
                return;

            var player = Game1.player;
            if (player == null) return;

            // Stamina regen
            if (Config.StaminaRegenPerSecond > 0 && player.stamina < player.MaxStamina)
            {
                player.stamina = Math.Min(player.MaxStamina, player.stamina + Config.StaminaRegenPerSecond);
            }

            // Health regen
            if (Config.HealthRegenPerSecond > 0 && player.health < player.maxHealth)
            {
                player.health = Math.Min(player.maxHealth, player.health + (int)Config.HealthRegenPerSecond);
            }

            // Max animal happiness
            if (Config.MaxAnimalHappiness)
            {
                var farm = Game1.getFarm();
                if (farm != null)
                {
                    foreach (var animal in farm.getAllFarmAnimals())
                    {
                        animal.happiness.Value = 255;
                        animal.friendshipTowardFarmer.Value = 1000;
                    }
                }
            }

            // Real-time luck setting
            if (Config.AlwaysMaxLuck)
            {
                Game1.player.team.sharedDailyLuck.Value = 0.12;
            }

            // Real-time instant crop growth (all locations)
            if (Config.InstantCropGrowth)
            {
                foreach (var location in Game1.locations)
                {
                    foreach (var pair in location.terrainFeatures.Pairs)
                    {
                        if (pair.Value is HoeDirt dirt && dirt.crop != null && !dirt.crop.fullyGrown.Value)
                        {
                            dirt.crop.growCompletely();
                        }
                    }
                }
            }

            // Real-time instant tree growth (regular trees)
            if (Config.InstantTreeGrowth)
            {
                foreach (var location in Game1.locations)
                {
                    foreach (var pair in location.terrainFeatures.Pairs)
                    {
                        if (pair.Value is Tree tree && tree.growthStage.Value < Tree.treeStage)
                        {
                            tree.growthStage.Value = Tree.treeStage;
                        }
                    }
                }
            }

            // Real-time instant fruit tree growth
            if (Config.InstantFruitTreeGrowth)
            {
                foreach (var location in Game1.locations)
                {
                    foreach (var pair in location.terrainFeatures.Pairs)
                    {
                        if (pair.Value is FruitTree fruitTree && fruitTree.growthStage.Value < FruitTree.treeStage)
                        {
                            fruitTree.growthStage.Value = FruitTree.treeStage;
                            fruitTree.daysUntilMature.Value = 0;
                        }
                    }
                }
            }

            // Force forage quality on world objects
            if (Config.ForceForageQuality >= 0)
            {
                foreach (var location in Game1.locations)
                {
                    foreach (var obj in location.objects.Values)
                    {
                        if (obj.IsSpawnedObject && obj.isForage() && obj.Quality != Config.ForceForageQuality)
                        {
                            obj.Quality = Config.ForceForageQuality;
                        }
                    }
                }
            }

            // Instant machine processing — complete all machines every second
            if (Config.InstantMachineProcessing)
            {
                WorldPatches.CompleteAllMachines();
            }
        }

        private void OnDayStarted(object? sender, DayStartedEventArgs e)
        {
            if (!Config.ModEnabled)
                return;

            // Weather override
            WorldPatches.ApplyWeatherOverride();

            // Complete any pending buildings/upgrades
            if (Config.InstantBuildConstruction || Config.InstantBuildUpgrade)
                WorldPatches.CompleteAllBuildings();
            if (Config.InstantHouseUpgrade)
                WorldPatches.CompleteHouseUpgrades();
            if (Config.InstantCommunityUpgrade)
                WorldPatches.CompleteCommunityUpgrades();
            if (Config.InstantToolUpgrade)
                CompleteToolUpgrade();
        }

        private void OnTimeChanged(object? sender, TimeChangedEventArgs e)
        {
            if (!Context.IsWorldReady || !Config.ModEnabled)
                return;

            // Freeze time indoors
            if (Config.FreezeTimeIndoors && !Game1.currentLocation.IsOutdoors)
            {
                Game1.timeOfDay = e.OldTime;
            }
        }

        private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            // Open StardewUI cheats menu when hotkey is pressed
            if (Config.OpenMenuKey.JustPressed())
            {
                if (!Context.IsWorldReady)
                    return;

                OpenCheatsMenu();
            }
        }

        private void OnBuildingListChanged(object? sender, BuildingListChangedEventArgs e)
        {
            if (!Context.IsWorldReady || !Config.ModEnabled)
                return;

            if (!Config.InstantBuildConstruction)
                return;

            // When a new building is added, complete it instantly
            foreach (var building in e.Added)
            {
                if (building.isUnderConstruction(ignoreUpgrades: false))
                {
                    building.FinishConstruction();
                    Monitor.Log($"Instant completed new building: {building.buildingType.Value}", LogLevel.Info);
                }
            }
        }

        /// <summary>Complete any pending tool upgrades at the blacksmith.</summary>
        private void CompleteToolUpgrade()
        {
            var player = Game1.player;
            if (player == null) return;

            if (player.toolBeingUpgraded.Value != null && player.daysLeftForToolUpgrade.Value > 0)
            {
                player.daysLeftForToolUpgrade.Value = 0;
                Monitor.Log($"Instant completed tool upgrade: {player.toolBeingUpgraded.Value.DisplayName}", LogLevel.Info);
            }
        }

        /// <summary>Opens the StardewUI cheats menu.</summary>
        private void OpenCheatsMenu()
        {
            if (viewEngine == null)
            {
                Monitor.Log("Cannot open cheats menu — StardewUI not loaded.", LogLevel.Warn);
                return;
            }

            try
            {
                var viewModel = new CheatsMenuViewModel();
                var controller = viewEngine.CreateMenuControllerFromAsset(
                    "Mods/CodeKokeshi.UltimatePlayerCheats/Views/CheatsMenu",
                    viewModel
                );

                controller.EnableCloseButton();
                controller.DimmingAmount = 0.75f;

                // When menu closes, auto-save config
                controller.Closing += () =>
                {
                    viewModel.SaveToConfig(Config);
                    Helper.WriteConfig(Config);
                    Monitor.Log("Cheats config saved.", LogLevel.Trace);
                };

                Game1.activeClickableMenu = controller.Menu;
            }
            catch (Exception ex)
            {
                Monitor.Log($"Error opening cheats menu: {ex.Message}", LogLevel.Error);
            }
        }
    }
}
