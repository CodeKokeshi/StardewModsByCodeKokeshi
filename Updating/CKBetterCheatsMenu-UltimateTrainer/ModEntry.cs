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
using StardewValley.Characters;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Monsters;
using StardewValley.Network;
using StardewValley.Quests;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using SObject = StardewValley.Object;

namespace CKBetterCheatsMenu
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

            // Read config from disk but only preserve ModEnabled and OpenMenuKey.
            // All cheat values reset to defaults every game launch (runtime trainer).
            var diskConfig = helper.ReadConfig<ModConfig>();
            Config = new ModConfig
            {
                ModEnabled = diskConfig.ModEnabled,
                OpenMenuKey = diskConfig.OpenMenuKey,
                EnableNotifications = diskConfig.EnableNotifications
            };

            // Restore saved cheats from config file (persistent across game restarts)
            ApplySavedCheats(diskConfig.Saved);

            harmony = new Harmony(this.ModManifest.UniqueID);
            ApplyPatches();

            // Register events
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
            helper.Events.GameLoop.OneSecondUpdateTicked += OnOneSecondUpdateTicked;
            helper.Events.GameLoop.DayStarted += OnDayStarted;
            helper.Events.GameLoop.DayEnding += OnDayEnding;
            helper.Events.GameLoop.TimeChanged += OnTimeChanged;
            helper.Events.Input.ButtonPressed += OnButtonPressed;
            helper.Events.World.BuildingListChanged += OnBuildingListChanged;

            Monitor.Log("[CKBetterCheatsMenu] Loaded! Press K to open cheats menu (StardewUI).", LogLevel.Info);
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
                    Monitor.Log($"[CKBetterCheatsMenu] WARNING: Could not find method to patch: {description}", LogLevel.Warn);
                    return;
                }
                try
                {
                    harmony.Patch(original, prefix: prefix, postfix: postfix);
                    Monitor.Log($"[CKBetterCheatsMenu] Patched: {description}", LogLevel.Trace);
                }
                catch (Exception ex)
                {
                    Monitor.Log($"[CKBetterCheatsMenu] ERROR patching {description}: {ex.Message}", LogLevel.Error);
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

            // === Prevent Debris Spawn (weeds, stones, twigs) ===
            TryPatch(
                original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.spawnWeedsAndStones)),
                prefix: new HarmonyMethod(typeof(WorldPatches), nameof(WorldPatches.GameLocation_SpawnWeedsAndStones_Prefix)),
                description: "GameLocation.spawnWeedsAndStones"
            );

            // === Tilled Soil Don't Decay ===
            TryPatch(
                original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.GetDirtDecayChance)),
                postfix: new HarmonyMethod(typeof(WorldPatches), nameof(WorldPatches.GameLocation_GetDirtDecayChance_Postfix)),
                description: "GameLocation.GetDirtDecayChance"
            );

            // === Buy Animals Fully Matured ===
            TryPatch(
                original: AccessTools.Method(typeof(PurchaseAnimalsMenu), nameof(PurchaseAnimalsMenu.receiveLeftClick)),
                prefix: new HarmonyMethod(typeof(WorldPatches), nameof(WorldPatches.PurchaseAnimalsMenu_ReceiveLeftClick_Prefix)),
                postfix: new HarmonyMethod(typeof(WorldPatches), nameof(WorldPatches.PurchaseAnimalsMenu_ReceiveLeftClick_Postfix)),
                description: "PurchaseAnimalsMenu.receiveLeftClick"
            );

            // ==================== PHASE 7: WORLD UPDATES ====================

            // === Bypass All Doors — performAction ===
            TryPatch(
                original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.performAction), new Type[] { typeof(string[]), typeof(Farmer), typeof(xTile.Dimensions.Location) }),
                prefix: new HarmonyMethod(typeof(WorldPatches), nameof(WorldPatches.GameLocation_PerformAction_Prefix)),
                description: "GameLocation.performAction (BypassAllDoors)"
            );

            // === Bypass All Doors — lockedDoorWarp ===
            TryPatch(
                original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.lockedDoorWarp)),
                prefix: new HarmonyMethod(typeof(WorldPatches), nameof(WorldPatches.GameLocation_LockedDoorWarp_Prefix)),
                description: "GameLocation.lockedDoorWarp (BypassAllDoors)"
            );

            // === Bypass All Doors — checkAction ===
            TryPatch(
                original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.checkAction)),
                prefix: new HarmonyMethod(typeof(WorldPatches), nameof(WorldPatches.GameLocation_CheckAction_Prefix)),
                description: "GameLocation.checkAction (BypassAllDoors)"
            );

            // === Freeze Time in Mines (separate prefix from base FreezeTime) ===
            TryPatch(
                original: AccessTools.Method(typeof(Game1), nameof(Game1.performTenMinuteClockUpdate)),
                prefix: new HarmonyMethod(typeof(WorldPatches), nameof(WorldPatches.Game1_PerformTenMinuteClockUpdate_MinesPrefix)),
                description: "Game1.performTenMinuteClockUpdate (MinesOnly)"
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
            viewEngine.RegisterViews("Mods/CodeKokeshi.CKBetterCheatsMenu/Views", "assets/views");

            Monitor.Log("StardewUI integration complete!", LogLevel.Debug);
        }

        private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
        {
            // Re-read config from disk every time a save is loaded.
            // This ensures returning to title and loading behaves identically to relaunching the game.
            var diskConfig = Helper.ReadConfig<ModConfig>();
            Config = new ModConfig
            {
                ModEnabled = diskConfig.ModEnabled,
                OpenMenuKey = diskConfig.OpenMenuKey,
                EnableNotifications = diskConfig.EnableNotifications
            };
            ApplySavedCheats(diskConfig.Saved);
            Monitor.Log("[CKBetterCheatsMenu] Config reloaded from disk on save load.", LogLevel.Debug);

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

            // Max animal happiness (only happiness, not friendship)
            if (Config.MaxAnimalHappiness)
            {
                var farm = Game1.getFarm();
                if (farm != null)
                {
                    foreach (var animal in farm.getAllFarmAnimals())
                    {
                        animal.happiness.Value = 255;
                    }
                }
            }

            // Infinite hay - keep all silos full
            if (Config.InfiniteHay)
            {
                foreach (var location in Game1.locations)
                {
                    int capacity = location.GetHayCapacity();
                    if (capacity > 0 && location.piecesOfHay.Value < capacity)
                    {
                        location.piecesOfHay.Value = capacity;
                    }
                }
            }

            // Farm animal hearts override
            if (Config.FarmAnimalHeartsOverride >= 0)
            {
                int targetFriendship = Config.FarmAnimalHeartsOverride * 100;
                var farm = Game1.getFarm();
                if (farm != null)
                {
                    foreach (var animal in farm.getAllFarmAnimals())
                    {
                        animal.friendshipTowardFarmer.Value = targetFriendship;
                    }
                }
            }

            // Pet hearts override
            if (Config.PetHeartsOverride >= 0)
            {
                int targetFriendship = Config.PetHeartsOverride * 100;
                foreach (var location in Game1.locations)
                {
                    foreach (var character in location.characters)
                    {
                        if (character is Pet pet)
                        {
                            pet.friendshipTowardFarmer.Value = targetFriendship;
                        }
                    }
                }
            }

            // Real-time luck setting
            if (Config.AlwaysMaxLuck)
            {
                Game1.player.team.sharedDailyLuck.Value = 0.12;
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

            // Auto-pet all farm animals immediately (runs every second while enabled)
            if (Config.AutoPetAnimals)
            {
                var farm = Game1.getFarm();
                if (farm != null)
                {
                    foreach (var animal in farm.getAllFarmAnimals())
                    {
                        if (!animal.wasPet.Value)
                            animal.wasPet.Value = true;
                    }
                }
            }

            // Auto-feed all barn/coop animals immediately (fill troughs every second while enabled)
            if (Config.AutoFeedAnimals)
            {
                var farm = Game1.getFarm();
                if (farm != null)
                {
                    foreach (var building in farm.buildings)
                    {
                        var indoors = building.GetIndoors();
                        if (indoors is AnimalHouse animalHouse)
                        {
                            animalHouse.feedAllAnimals();
                        }
                    }
                }
            }

            // Animals produce daily — set daysSinceLastLay high so they produce on next check
            if (Config.AnimalsProduceDaily)
            {
                var farm = Game1.getFarm();
                if (farm != null)
                {
                    foreach (var animal in farm.getAllFarmAnimals())
                    {
                        if (animal.daysSinceLastLay.Value < 99)
                            animal.daysSinceLastLay.Value = 99;
                    }
                }
            }

            // Auto-accept daily quest immediately when enabled
            if (Config.AutoAcceptQuests)
            {
                if (Game1.CanAcceptDailyQuest() && !Game1.player.acceptedDailyQuest.Value)
                {
                    Game1.player.questLog.Add(Game1.questOfTheDay);
                    Game1.player.acceptedDailyQuest.Set(true);
                    Monitor.Log("[World] Auto-accepted daily quest (immediate).", LogLevel.Trace);
                }
            }

            // Infinite quest time — keep daysLeft high for all active timed quests
            if (Config.InfiniteQuestTime)
            {
                foreach (var quest in Game1.player.questLog)
                {
                    if (quest.IsTimedQuest() && !quest.completed.Value && quest.daysLeft.Value < 90)
                    {
                        quest.daysLeft.Value = 99;
                    }
                }
            }
        }

        private void OnDayStarted(object? sender, DayStartedEventArgs e)
        {
            if (!Config.ModEnabled)
                return;

            // Weather override (explicitly "next day" — applied at day start)
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

        private void OnDayEnding(object? sender, DayEndingEventArgs e)
        {
            if (!Config.ModEnabled)
                return;

            // Infinite quest time - set daysLeft high BEFORE Farmer.dayupdate() decrements/removes them
            if (Config.InfiniteQuestTime)
            {
                foreach (var quest in Game1.player.questLog)
                {
                    if (quest.IsTimedQuest() && !quest.completed.Value)
                    {
                        quest.daysLeft.Value = 99;
                    }
                }
            }
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

            // Freeze time in mines (backup for when Harmony prefix might not fire)
            if (Config.FreezeTimeMines && (Game1.currentLocation is MineShaft || Game1.currentLocation is VolcanoDungeon))
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

        /// <summary>Apply saved cheats from config file to the runtime config.</summary>
        private static void ApplySavedCheats(SavedCheats saved)
        {
            if (saved == null) return;

            // Player: Movement & Speed (no NoClip)
            Config.SpeedMultiplier = saved.SpeedMultiplier;
            Config.AddedSpeedBonus = saved.AddedSpeedBonus;

            // Player: Health & Stamina
            Config.InfiniteStamina = saved.InfiniteStamina;
            Config.InfiniteHealth = saved.InfiniteHealth;
            Config.MaxStaminaOverride = saved.MaxStaminaOverride;
            Config.MaxHealthOverride = saved.MaxHealthOverride;
            Config.StaminaRegenPerSecond = saved.StaminaRegenPerSecond;
            Config.HealthRegenPerSecond = saved.HealthRegenPerSecond;

            // Combat: All
            Config.DamageMultiplier = saved.DamageMultiplier;
            Config.OneHitKill = saved.OneHitKill;
            Config.AlwaysCrit = saved.AlwaysCrit;
            Config.CritDamageMultiplier = saved.CritDamageMultiplier;
            Config.AddedDefense = saved.AddedDefense;
            Config.AddedAttack = saved.AddedAttack;
            Config.AddedImmunity = saved.AddedImmunity;
            Config.NoMonsterSpawns = saved.NoMonsterSpawns;

            // Skills: XP Multiplier only (no level overrides)
            Config.XPMultiplier = saved.XPMultiplier;

            // Tools: All
            Config.ToolAreaMultiplier = saved.ToolAreaMultiplier;
            Config.NoToolStaminaCost = saved.NoToolStaminaCost;
            Config.InfiniteWater = saved.InfiniteWater;
            Config.OneHitTools = saved.OneHitTools;
            Config.InstantToolUpgrade = saved.InstantToolUpgrade;
            Config.FreeCrafting = saved.FreeCrafting;

            // Farming: Crop Settings + Field Protection
            Config.CropsNeverDie = saved.CropsNeverDie;
            Config.ForceForageQuality = saved.ForceForageQuality;
            Config.PreventDebrisSpawn = saved.PreventDebrisSpawn;
            Config.TilledSoilDontDecay = saved.TilledSoilDontDecay;

            // Animals: All except hearts overrides
            Config.MaxAnimalHappiness = saved.MaxAnimalHappiness;
            Config.BuyAnimalsFullyMatured = saved.BuyAnimalsFullyMatured;
            Config.AutoPetAnimals = saved.AutoPetAnimals;
            Config.AutoFeedAnimals = saved.AutoFeedAnimals;
            Config.InfiniteHay = saved.InfiniteHay;
            Config.AnimalsProduceDaily = saved.AnimalsProduceDaily;

            // Fishing: All
            Config.InstantFishBite = saved.InstantFishBite;
            Config.InstantCatch = saved.InstantCatch;
            Config.MaxFishQuality = saved.MaxFishQuality;
            Config.AlwaysFindTreasure = saved.AlwaysFindTreasure;

            // Items: Items and Inventory only
            Config.MagneticRadiusMultiplier = saved.MagneticRadiusMultiplier;
            Config.AddedMagneticRadius = saved.AddedMagneticRadius;
            Config.InfiniteItems = saved.InfiniteItems;

            // Economy: Prices and Shopping only
            Config.SellPriceMultiplier = saved.SellPriceMultiplier;
            Config.BuyPriceMultiplier = saved.BuyPriceMultiplier;
            Config.FreeShopPurchases = saved.FreeShopPurchases;
            Config.FreeGeodeProcessing = saved.FreeGeodeProcessing;

            // Buildings: All
            Config.InstantBuildConstruction = saved.InstantBuildConstruction;
            Config.InstantBuildUpgrade = saved.InstantBuildUpgrade;
            Config.InstantHouseUpgrade = saved.InstantHouseUpgrade;
            Config.InstantCommunityUpgrade = saved.InstantCommunityUpgrade;
            Config.FreeBuildingConstruction = saved.FreeBuildingConstruction;
            Config.InstantMachineProcessing = saved.InstantMachineProcessing;

            // World: Specific ones only
            Config.NeverPassOut = saved.NeverPassOut;
            Config.AlwaysMaxLuck = saved.AlwaysMaxLuck;
            Config.BypassFriendshipDoors = saved.BypassFriendshipDoors;
            Config.BypassTimeRestrictions = saved.BypassTimeRestrictions;
            Config.BypassFestivalClosures = saved.BypassFestivalClosures;
            Config.BypassConditionalDoors = saved.BypassConditionalDoors;
            Config.BypassSpecialClosures = saved.BypassSpecialClosures;
            Config.AutoAcceptQuests = saved.AutoAcceptQuests;
            Config.InfiniteQuestTime = saved.InfiniteQuestTime;

            // Relationships: All
            Config.FriendshipMultiplier = saved.FriendshipMultiplier;
            Config.NoFriendshipDecay = saved.NoFriendshipDecay;
            Config.GiveGiftsAnytime = saved.GiveGiftsAnytime;

            // Mining
            Config.ForceLadderChance = saved.ForceLadderChance;

            ModMonitor.Log("[CKBetterCheatsMenu] Restored saved cheats from config.", LogLevel.Debug);
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
                    "Mods/CodeKokeshi.CKBetterCheatsMenu/Views/CheatsMenu",
                    viewModel
                );

                controller.EnableCloseButton();
                controller.DimmingAmount = 0.75f;

                // When menu closes, sync to in-memory config (runtime trainer — no disk persistence)
                controller.Closing += () =>
                {
                    viewModel.SaveToConfig(Config);
                    Monitor.Log("Cheats synced to runtime config.", LogLevel.Trace);
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
