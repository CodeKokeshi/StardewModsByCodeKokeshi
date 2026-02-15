using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PropertyChanged.SourceGenerator;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;

namespace CKBetterCheatsMenu
{
    /// <summary>Data for a single warp location.</summary>
    internal class WarpLocationData
    {
        /// <summary>Internal location name used for warping.</summary>
        public string LocationName { get; }

        /// <summary>Display name shown to the user.</summary>
        public string DisplayName { get; }

        /// <summary>Default X tile position for warping.</summary>
        public int TileX { get; }

        /// <summary>Default Y tile position for warping.</summary>
        public int TileY { get; }

        /// <summary>Category for grouping locations in the UI.</summary>
        public string Category { get; }

        public WarpLocationData(string locationName, string displayName, int tileX, int tileY, string category)
        {
            LocationName = locationName;
            DisplayName = displayName;
            TileX = tileX;
            TileY = tileY;
            Category = category;
        }
    }

    /// <summary>Tab data for the main menu tabs.</summary>
    internal partial class TabData : INotifyPropertyChanged
    {
        public string Name { get; }
        public Tuple<Texture2D, Rectangle> Sprite { get; }

        [Notify]
        private bool active;

        public TabData(string name, Texture2D texture, Rectangle sourceRect)
        {
            Name = name;
            Sprite = Tuple.Create(texture, sourceRect);
        }
    }

    /// <summary>Main ViewModel for the cheats menu. Binds all config values to the StardewUI view.</summary>
    internal partial class CheatsMenuViewModel : INotifyPropertyChanged
    {
        /*********
        ** Tab Management
        *********/
        public IReadOnlyList<TabData> Tabs { get; set; } = Array.Empty<TabData>();

        [Notify]
        private string selectedTab = "General";

        public void OnTabActivated(string name)
        {
            foreach (var tab in Tabs)
            {
                if (tab.Name != name)
                    tab.Active = false;
            }
            SelectedTab = name;
        }

        /*********
        ** General
        *********/
        [Notify] private bool modEnabled;

        /*********
        ** Movement & Speed
        *********/
        [Notify] private float speedMultiplier;
        [Notify] private float addedSpeedBonus;
        [Notify] private bool noClip;

        /*********
        ** Health & Stamina
        *********/
        [Notify] private bool infiniteStamina;
        [Notify] private bool infiniteHealth;
        [Notify] private float maxStaminaOverride;
        [Notify] private float maxHealthOverride;
        [Notify] private float staminaRegenPerSecond;
        [Notify] private float healthRegenPerSecond;

        /*********
        ** Combat
        *********/
        [Notify] private float damageMultiplier;
        [Notify] private bool oneHitKill;
        [Notify] private bool alwaysCrit;
        [Notify] private float critDamageMultiplier;
        [Notify] private float addedDefense;
        [Notify] private float addedAttack;
        [Notify] private float addedImmunity;
        [Notify] private bool noMonsterSpawns;

        /*********
        ** Tools & Farming
        *********/
        [Notify] private float toolAreaMultiplier;
        [Notify] private bool noToolStaminaCost;
        [Notify] private bool infiniteWater;
        [Notify] private bool oneHitTools;

        /*********
        ** Items & Inventory
        *********/
        [Notify] private float magneticRadiusMultiplier;
        [Notify] private float addedMagneticRadius;
        [Notify] private bool infiniteItems;

        /*********
        ** Skills & Levels
        *********/
        [Notify] private float farmingLevelOverride;
        [Notify] private float miningLevelOverride;
        [Notify] private float foragingLevelOverride;
        [Notify] private float fishingLevelOverride;
        [Notify] private float combatLevelOverride;
        [Notify] private float xpMultiplier;

        /*********
        ** Luck
        *********/
        [Notify] private bool alwaysMaxLuck;

        /*********
        ** Fishing
        *********/
        [Notify] private bool instantFishBite;
        [Notify] private bool instantCatch;
        [Notify] private bool maxFishQuality;
        [Notify] private bool alwaysFindTreasure;

        /*********
        ** Quality & Prices
        *********/
        [Notify] private float forceForageQuality;
        [Notify] private float sellPriceMultiplier;
        [Notify] private float buyPriceMultiplier;

        /*********
        ** Economy Actions
        *********/
        [Notify] private float addMoneyAmount = 1000;
        [Notify] private float addCasinoCoinsAmount = 100;

        /*********
        ** Relationships
        *********/
        [Notify] private float friendshipMultiplier;
        [Notify] private bool noFriendshipDecay;
        [Notify] private bool giveGiftsAnytime;

        /*********
        ** Time
        *********/
        [Notify] private bool freezeTime;
        [Notify] private bool freezeTimeIndoors;
        [Notify] private bool freezeTimeMines;
        [Notify] private bool neverPassOut;

        /*********
        ** Bypass All Doors
        *********/
        [Notify] private bool bypassFriendshipDoors;
        [Notify] private bool bypassTimeRestrictions;
        [Notify] private bool bypassFestivalClosures;
        [Notify] private bool bypassConditionalDoors;
        [Notify] private bool bypassSpecialClosures;

        /*********
        ** Quests
        *********/
        [Notify] private bool autoAcceptQuests;
        [Notify] private bool infiniteQuestTime;

        /*********
        ** Time Control
        *********/
        [Notify] private float setTimeTarget = 600;

        /*********
        ** Animals & Pets
        *********/
        [Notify] private bool maxAnimalHappiness;
        [Notify] private bool buyAnimalsFullyMatured;
        [Notify] private bool autoPetAnimals;
        [Notify] private bool autoFeedAnimals;
        [Notify] private bool infiniteHay;
        [Notify] private bool animalsProduceDaily;
        [Notify] private int farmAnimalHeartsOverride;
        [Notify] private int petHeartsOverride;

        /*********
        ** Farming
        *********/
        [Notify] private bool cropsNeverDie;
        [Notify] private bool preventDebrisSpawn;
        [Notify] private bool tilledSoilDontDecay;

        /*********
        ** Buildings & Construction (World Cheats)
        *********/
        [Notify] private bool instantBuildConstruction;
        [Notify] private bool instantBuildUpgrade;
        [Notify] private bool instantHouseUpgrade;
        [Notify] private bool instantCommunityUpgrade;
        [Notify] private bool freeBuildingConstruction;

        /*********
        ** Tool Upgrades (World Cheats)
        *********/
        [Notify] private bool instantToolUpgrade;

        /*********
        ** Machines & Processing (World Cheats)
        *********/
        [Notify] private bool instantMachineProcessing;
        [Notify] private bool freeGeodeProcessing;

        /*********
        ** Shopping & Economy (World Cheats)
        *********/
        [Notify] private bool freeShopPurchases;
        [Notify] private bool freeCrafting;

        /*********
        ** Weather Control (World Cheats)
        *********/
        [Notify] private float weatherIndex;

        /*********
        ** Mining
        *********/
        [Notify] private float forceLadderChance;

        /*********
        ** Warp Locations
        *********/
        /// <summary>All available warp locations grouped by category.</summary>
        public IReadOnlyList<WarpLocationData> WarpLocations { get; private set; } = Array.Empty<WarpLocationData>();

        /// <summary>Warp the player to the specified location.</summary>
        public void WarpTo(WarpLocationData location)
        {
            if (location == null || !Context.IsWorldReady)
                return;

            // Close the menu first
            Game1.activeClickableMenu?.exitThisMenu();

            // Warp the player to the location
            Game1.warpFarmer(location.LocationName, location.TileX, location.TileY, false);
            ModEntry.ModMonitor.Log($"[CKBetterCheatsMenu] Warped to {location.DisplayName} ({location.LocationName}) at ({location.TileX}, {location.TileY})", LogLevel.Info);
        }

        /// <summary>Add money to the player's wallet and profit tracking.</summary>
        public void AddMoney()
        {
            if (!Context.IsWorldReady || AddMoneyAmount <= 0)
                return;

            int amount = (int)AddMoneyAmount;
            Game1.player.Money += amount;
            Game1.player.totalMoneyEarned += (uint)amount;
            Game1.addHUDMessage(new HUDMessage(ModEntry.ModHelper.Translation.Get("hud.added-gold", new { amount = amount.ToString("N0") }), HUDMessage.achievement_type));
            ModEntry.ModMonitor.Log($"[CKBetterCheatsMenu] Added {amount}g to player. New total: {Game1.player.Money}g", LogLevel.Info);
        }

        /// <summary>Add casino coins (Qi coins) to the player.</summary>
        public void AddCasinoCoins()
        {
            if (!Context.IsWorldReady || AddCasinoCoinsAmount <= 0)
                return;

            int amount = (int)AddCasinoCoinsAmount;
            Game1.player.clubCoins += amount;
            Game1.addHUDMessage(new HUDMessage(ModEntry.ModHelper.Translation.Get("hud.added-qi-coins", new { amount = amount.ToString("N0") }), HUDMessage.achievement_type));
            ModEntry.ModMonitor.Log($"[CKBetterCheatsMenu] Added {amount} Qi Coins. New total: {Game1.player.clubCoins}", LogLevel.Info);
        }

        /*********
        ** Farming Actions
        *********/

        /// <summary>Instantly grow all crops to harvestable state.</summary>
        public void GrowAllCrops()
        {
            if (!Context.IsWorldReady)
                return;

            int count = 0;
            foreach (var location in Game1.locations)
            {
                foreach (var pair in location.terrainFeatures.Pairs)
                {
                    if (pair.Value is HoeDirt dirt && dirt.crop != null && !dirt.crop.fullyGrown.Value)
                    {
                        dirt.crop.growCompletely();
                        count++;
                    }
                }
            }

            Game1.addHUDMessage(new HUDMessage(ModEntry.ModHelper.Translation.Get("hud.grew-crops", new { count }), HUDMessage.achievement_type));
            ModEntry.ModMonitor.Log($"[CKBetterCheatsMenu] Grew {count} crops to full maturity.", LogLevel.Info);
        }

        /// <summary>Instantly grow all regular trees to full size.</summary>
        public void GrowAllTrees()
        {
            if (!Context.IsWorldReady)
                return;

            int count = 0;
            foreach (var location in Game1.locations)
            {
                foreach (var pair in location.terrainFeatures.Pairs)
                {
                    if (pair.Value is Tree tree && tree.growthStage.Value < Tree.treeStage)
                    {
                        tree.growthStage.Value = Tree.treeStage;
                        count++;
                    }
                }
            }

            Game1.addHUDMessage(new HUDMessage(ModEntry.ModHelper.Translation.Get("hud.grew-trees", new { count }), HUDMessage.achievement_type));
            ModEntry.ModMonitor.Log($"[CKBetterCheatsMenu] Grew {count} regular trees to full size.", LogLevel.Info);
        }

        /// <summary>Instantly grow all fruit trees to full maturity.</summary>
        public void GrowAllFruitTrees()
        {
            if (!Context.IsWorldReady)
                return;

            int count = 0;
            foreach (var location in Game1.locations)
            {
                foreach (var pair in location.terrainFeatures.Pairs)
                {
                    if (pair.Value is FruitTree fruitTree && fruitTree.growthStage.Value < FruitTree.treeStage)
                    {
                        fruitTree.growthStage.Value = FruitTree.treeStage;
                        fruitTree.daysUntilMature.Value = 0;
                        count++;
                    }
                }
            }

            Game1.addHUDMessage(new HUDMessage(ModEntry.ModHelper.Translation.Get("hud.grew-fruit-trees", new { count }), HUDMessage.achievement_type));
            ModEntry.ModMonitor.Log($"[CKBetterCheatsMenu] Grew {count} fruit trees to full maturity.", LogLevel.Info);
        }

        /// <summary>Water all tilled fields on the farm.</summary>
        public void WaterAllFields()
        {
            if (!Context.IsWorldReady)
                return;

            int count = 0;
            foreach (var location in Game1.locations)
            {
                foreach (var pair in location.terrainFeatures.Pairs)
                {
                    if (pair.Value is HoeDirt dirt && dirt.state.Value != HoeDirt.watered)
                    {
                        dirt.state.Value = HoeDirt.watered;
                        count++;
                    }
                }
            }

            Game1.addHUDMessage(new HUDMessage(ModEntry.ModHelper.Translation.Get("hud.watered-tiles", new { count }), HUDMessage.achievement_type));
            ModEntry.ModMonitor.Log($"[CKBetterCheatsMenu] Watered {count} tilled tiles.", LogLevel.Info);
        }

        /// <summary>Unlock all crafting and cooking recipes for the player.</summary>
        public void UnlockAllRecipes()
        {
            if (!Context.IsWorldReady)
                return;

            int craftingCount = UnlockAllCraftingRecipesInternal();
            int cookingCount = UnlockAllCookingRecipesInternal();

            Game1.addHUDMessage(new HUDMessage(ModEntry.ModHelper.Translation.Get("hud.unlocked-all-recipes", new { crafting = craftingCount, cooking = cookingCount }), HUDMessage.achievement_type));
            ModEntry.ModMonitor.Log($"[CKBetterCheatsMenu] Unlocked {craftingCount} crafting and {cookingCount} cooking recipes.", LogLevel.Info);
        }

        /// <summary>Unlock all crafting recipes for the player.</summary>
        public void UnlockAllCraftingRecipes()
        {
            if (!Context.IsWorldReady)
                return;

            int count = UnlockAllCraftingRecipesInternal();
            Game1.addHUDMessage(new HUDMessage(ModEntry.ModHelper.Translation.Get("hud.unlocked-crafting", new { count }), HUDMessage.achievement_type));
            ModEntry.ModMonitor.Log($"[CKBetterCheatsMenu] Unlocked {count} crafting recipes.", LogLevel.Info);
        }

        /// <summary>Unlock all cooking recipes for the player.</summary>
        public void UnlockAllCookingRecipes()
        {
            if (!Context.IsWorldReady)
                return;

            int count = UnlockAllCookingRecipesInternal();
            Game1.addHUDMessage(new HUDMessage(ModEntry.ModHelper.Translation.Get("hud.unlocked-cooking", new { count }), HUDMessage.achievement_type));
            ModEntry.ModMonitor.Log($"[CKBetterCheatsMenu] Unlocked {count} cooking recipes.", LogLevel.Info);
        }

        /// <summary>Unlock all inventory slots (maximize backpack size).</summary>
        public void UnlockAllInventorySlots()
        {
            if (!Context.IsWorldReady)
                return;

            var player = Game1.player;
            int maxSlots = 36; // Maximum backpack size (3 rows of 12)

            if (player.MaxItems >= maxSlots)
            {
                Game1.addHUDMessage(new HUDMessage(ModEntry.ModHelper.Translation.Get("hud.inventory-max"), HUDMessage.error_type));
                return;
            }

            int added = maxSlots - player.MaxItems;
            player.increaseBackpackSize(added);

            Game1.addHUDMessage(new HUDMessage(ModEntry.ModHelper.Translation.Get("hud.unlocked-slots", new { count = added }), HUDMessage.achievement_type));
            ModEntry.ModMonitor.Log($"[CKBetterCheatsMenu] Increased backpack from {maxSlots - added} to {maxSlots} slots.", LogLevel.Info);
        }

        /// <summary>Internal helper to unlock all crafting recipes.</summary>
        private int UnlockAllCraftingRecipesInternal()
        {
            int count = 0;
            var player = Game1.player;

            // Ensure crafting recipes are loaded
            if (CraftingRecipe.craftingRecipes == null)
                CraftingRecipe.InitShared();

            foreach (string recipeName in CraftingRecipe.craftingRecipes.Keys)
            {
                if (player.craftingRecipes.TryAdd(recipeName, 0))
                    count++;
            }

            return count;
        }

        /// <summary>Internal helper to unlock all cooking recipes.</summary>
        private int UnlockAllCookingRecipesInternal()
        {
            int count = 0;
            var player = Game1.player;

            // Ensure cooking recipes are loaded
            if (CraftingRecipe.cookingRecipes == null)
                CraftingRecipe.InitShared();

            foreach (string recipeName in CraftingRecipe.cookingRecipes.Keys)
            {
                if (player.cookingRecipes.TryAdd(recipeName, 0))
                    count++;
            }

            return count;
        }

        /// <summary>Refresh the list of available warp locations from the current game.</summary>
        public void RefreshWarpLocations()
        {
            if (!Context.IsWorldReady)
            {
                WarpLocations = Array.Empty<WarpLocationData>();
                return;
            }

            var locations = new List<WarpLocationData>();

            foreach (var gameLocation in Game1.locations)
            {
                if (gameLocation == null || string.IsNullOrEmpty(gameLocation.Name))
                    continue;

                // Skip temporary locations (festivals, events)
                if (gameLocation.IsTemporary)
                    continue;

                // Get display name for tooltip (falls back to Name if DisplayName is not set)
                string displayName = gameLocation.DisplayName ?? gameLocation.Name;

                // If display name is same as location name, show category instead
                if (displayName == gameLocation.Name)
                    displayName = CategorizeLocation(gameLocation);

                // Get default warp position
                int x = 0, y = 0;
                Utility.getDefaultWarpLocation(gameLocation.Name, ref x, ref y);

                // If no default position found, try center of the map
                if (x == 0 && y == 0 && gameLocation.Map != null)
                {
                    x = gameLocation.Map.Layers[0].LayerWidth / 2;
                    y = gameLocation.Map.Layers[0].LayerHeight / 2;
                }

                // Categorize locations
                string category = CategorizeLocation(gameLocation);

                locations.Add(new WarpLocationData(gameLocation.Name, displayName, x, y, category));
            }

            // Sort by category, then by location name
            WarpLocations = locations
                .OrderBy(l => GetCategoryOrder(l.Category))
                .ThenBy(l => l.LocationName)
                .ToList();

            ModEntry.ModMonitor.Log($"[CKBetterCheatsMenu] Loaded {WarpLocations.Count} warp locations.", LogLevel.Trace);
        }

        /// <summary>Categorize a location for UI grouping.</summary>
        private static string CategorizeLocation(GameLocation location)
        {
            string name = location.Name ?? "";

            // Farm and farm buildings
            if (name == "Farm" || name == "FarmHouse" || name == "FarmCave" || name == "Greenhouse" || name == "Cellar")
                return "Farm";
            if (name.StartsWith("Barn") || name.StartsWith("Coop") || name.StartsWith("Shed") || name.StartsWith("SlimeHutch"))
                return "Farm Buildings";

            // Town
            if (name == "Town" || name == "SeedShop" || name == "JoshHouse" || name == "HaleyHouse" ||
                name == "SamHouse" || name == "Blacksmith" || name == "ManorHouse" || name == "ArchaeologyHouse" ||
                name == "AlexHouse" || name == "JojaMart" || name == "Saloon" || name == "Hospital" ||
                name == "HarveyRoom" || name == "Trailer" || name == "Trailer_Big" || name == "CommunityCenter" ||
                name == "MovieTheater")
                return "Town";

            // Beach
            if (name == "Beach" || name == "FishShop" || name == "ElliottHouse")
                return "Beach";

            // Mountain & Forest
            if (name == "Mountain" || name == "ScienceHouse" || name == "SebastianRoom" || name == "Tent" ||
                name == "AdventureGuild" || name == "Backwoods")
                return "Mountain";
            if (name == "Forest" || name == "Woods" || name == "WizardHouse" || name == "WizardHouseBasement" ||
                name == "AnimalShop" || name == "LeahHouse")
                return "Forest";

            // Mines & Caves
            if (name.StartsWith("Mine") || name.StartsWith("UndergroundMine") || name == "SkullCave" ||
                name.StartsWith("VolcanoDungeon") || name == "Caldera")
                return "Mines & Caves";

            // Desert
            if (name == "Desert" || name == "SandyHouse" || name == "SkullCave" || name == "Club")
                return "Desert";

            // Island
            if (name.StartsWith("Island"))
                return "Ginger Island";

            // Railroad & Spa
            if (name == "Railroad" || name == "BathHouse_Entry" || name == "BathHouse_MensLocker" ||
                name == "BathHouse_WomensLocker" || name == "BathHouse_Pool" || name == "Summit")
                return "Railroad & Spa";

            // Sewer & Special
            if (name == "Sewer" || name == "BugLand" || name == "WitchSwamp" || name == "WitchHut" ||
                name == "WitchWarpCave" || name == "Sunroom")
                return "Special";

            // Other
            return "Other";
        }

        /// <summary>Get sort order for categories.</summary>
        private static int GetCategoryOrder(string category)
        {
            return category switch
            {
                "Farm" => 0,
                "Farm Buildings" => 1,
                "Town" => 2,
                "Beach" => 3,
                "Forest" => 4,
                "Mountain" => 5,
                "Mines & Caves" => 6,
                "Desert" => 7,
                "Railroad & Spa" => 8,
                "Ginger Island" => 9,
                "Special" => 10,
                _ => 99
            };
        }

        /*********
        ** World Actions — Phase 7
        *********/

        /// <summary>Complete all community center bundles.</summary>
        public void CompleteCommunityBundle()
        {
            if (!Context.IsWorldReady)
                return;

            var cc = Game1.getLocationFromName("CommunityCenter") as StardewValley.Locations.CommunityCenter;
            if (cc == null)
            {
                Game1.addHUDMessage(new HUDMessage(ModEntry.ModHelper.Translation.Get("hud.cc-not-found"), HUDMessage.error_type));
                return;
            }

            // Check if already completed via JojaMart route
            if (Game1.MasterPlayer.mailReceived.Contains("JojaMember"))
            {
                Game1.addHUDMessage(new HUDMessage(ModEntry.ModHelper.Translation.Get("hud.joja-active"), HUDMessage.error_type));
                return;
            }

            int completedBundles = 0;
            var bundleData = Game1.netWorldState.Value.Bundles;

            foreach (var key in bundleData.Keys)
            {
                bool[] slots = bundleData[key];
                bool anyChanged = false;
                for (int i = 0; i < slots.Length; i++)
                {
                    if (!slots[i])
                    {
                        slots[i] = true;
                        anyChanged = true;
                    }
                }
                if (anyChanged)
                {
                    bundleData[key] = slots;
                    completedBundles++;
                }

                // Mark bundle reward as collected
                if (!Game1.netWorldState.Value.BundleRewards.ContainsKey(key) || !Game1.netWorldState.Value.BundleRewards[key])
                {
                    Game1.netWorldState.Value.BundleRewards[key] = true;
                }
            }

            // Mark all areas as complete
            string[] areaMailFlags = { "ccPantry", "ccCraftsRoom", "ccFishTank", "ccBoilerRoom", "ccVault", "ccBulletin" };
            for (int area = 0; area < 6; area++)
            {
                if (!cc.areasComplete[area])
                {
                    cc.markAreaAsComplete(area);
                }
                if (!Game1.MasterPlayer.mailReceived.Contains(areaMailFlags[area]))
                {
                    Game1.MasterPlayer.mailReceived.Add(areaMailFlags[area]);
                }
            }

            // Mark community center as complete
            if (!Game1.MasterPlayer.mailReceived.Contains("ccIsComplete"))
            {
                Game1.MasterPlayer.mailReceived.Add("ccIsComplete");
            }

            Game1.addHUDMessage(new HUDMessage(ModEntry.ModHelper.Translation.Get("hud.completed-bundles", new { count = completedBundles }), HUDMessage.achievement_type));
            ModEntry.ModMonitor.Log($"[World] Completed all community center bundles ({completedBundles} updated).", LogLevel.Info);
        }

        /// <summary>Complete all active special orders.</summary>
        public void CompleteSpecialOrders()
        {
            if (!Context.IsWorldReady)
                return;

            int count = 0;
            foreach (var order in Game1.player.team.specialOrders)
            {
                if (order.questState.Value == StardewValley.SpecialOrders.SpecialOrderStatus.InProgress)
                {
                    // Complete all objectives
                    foreach (var objective in order.objectives)
                    {
                        objective.SetCount(objective.GetMaxCount());
                    }
                    order.CheckCompletion();
                    count++;
                }
            }

            if (count > 0)
            {
                Game1.addHUDMessage(new HUDMessage(ModEntry.ModHelper.Translation.Get("hud.completed-orders", new { count }), HUDMessage.achievement_type));
                ModEntry.ModMonitor.Log($"[World] Completed {count} active special orders.", LogLevel.Info);
            }
            else
            {
                Game1.addHUDMessage(new HUDMessage(ModEntry.ModHelper.Translation.Get("hud.no-active-orders"), HUDMessage.error_type));
            }
        }

        /// <summary>Subtract 10 minutes from current time.</summary>
        public void SubtractTime()
        {
            if (!Context.IsWorldReady)
                return;

            int newTime = Game1.timeOfDay - 10;
            // Handle minutes rollover: if minutes become negative (e.g., 1200 - 10 = 1190 → 1150)
            if (newTime % 100 >= 60)
                newTime = newTime - newTime % 100 + 50;
            if (newTime < 600)
                newTime = 600;

            Game1.timeOfDay = newTime;
            Game1.gameTimeInterval = 0;
            ModEntry.ModMonitor.Log($"[World] Time set to {newTime}.", LogLevel.Trace);
        }

        /// <summary>Add 10 minutes to current time.</summary>
        public void AddTime()
        {
            if (!Context.IsWorldReady)
                return;

            int newTime = Game1.timeOfDay + 10;
            // Handle minutes rollover: if minutes reach 60 (e.g., 1250 + 10 = 1260 → 1300)
            if (newTime % 100 >= 60)
                newTime = newTime - newTime % 100 + 100;
            if (newTime > 2600)
                newTime = 2600;

            Game1.timeOfDay = newTime;
            Game1.gameTimeInterval = 0;
            ModEntry.ModMonitor.Log($"[World] Time set to {newTime}.", LogLevel.Trace);
        }

        /// <summary>Set current time to the slider value.</summary>
        public void SetCurrentTime()
        {
            if (!Context.IsWorldReady)
                return;

            int target = (int)SetTimeTarget;
            // Clamp and round to nearest 10-minute increment
            target = Math.Clamp(target, 600, 2600);
            // Round to nearest 10
            target = (target / 10) * 10;
            // Fix rollover
            if (target % 100 >= 60)
                target = target - target % 100 + 100;

            Game1.timeOfDay = target;
            Game1.gameTimeInterval = 0;
            Game1.addHUDMessage(new HUDMessage(ModEntry.ModHelper.Translation.Get("hud.time-set", new { time = Game1.getTimeOfDayString(target) }), HUDMessage.achievement_type));
            ModEntry.ModMonitor.Log($"[World] Time set to {target}.", LogLevel.Info);
        }

        /*********
        ** Constructor — load from config
        *********/
        public CheatsMenuViewModel()
        {
            LoadFromConfig(ModEntry.Config);
            InitTabs();
            RefreshWarpLocations();

            // Subscribe to property changes to sync values immediately to config
            PropertyChanged += HandlePropertyChanged;
        }

        /// <summary>Sync changed property values to config immediately so cheats take effect in real-time.</summary>
        private void HandlePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            // Sync current ViewModel state to config so UpdateTicked can use it
            SaveToConfig(ModEntry.Config);
        }

        private void InitTabs()
        {
            var icons = ModEntry.ModHelper.ModContent.Load<Texture2D>("assets/Icons.png");
            // Custom 16x16 icons in a horizontal strip (240x16 image, 15 tabs)
            Tabs = new List<TabData>
            {
                new TabData("General",       icons, new Rectangle(0,   0, 16, 16)),
                new TabData("Player",        icons, new Rectangle(16,  0, 16, 16)),
                new TabData("Combat",        icons, new Rectangle(32,  0, 16, 16)),
                new TabData("Skills",        icons, new Rectangle(48,  0, 16, 16)),
                new TabData("Tools",         icons, new Rectangle(64,  0, 16, 16)),
                new TabData("Farming",       icons, new Rectangle(80,  0, 16, 16)),
                new TabData("Animals",       icons, new Rectangle(96,  0, 16, 16)),
                new TabData("Fishing",       icons, new Rectangle(112, 0, 16, 16)),
                new TabData("Items",         icons, new Rectangle(128, 0, 16, 16)),
                new TabData("Economy",       icons, new Rectangle(144, 0, 16, 16)),
                new TabData("Buildings",     icons, new Rectangle(160, 0, 16, 16)),
                new TabData("World",         icons, new Rectangle(176, 0, 16, 16)),
                new TabData("Relationships", icons, new Rectangle(192, 0, 16, 16)),
                new TabData("Warp",          icons, new Rectangle(208, 0, 16, 16)),
                new TabData("Mining",        icons, new Rectangle(224, 0, 16, 16)),
            };
            Tabs[0].Active = true;
        }

        /// <summary>Load all values from the config into this ViewModel.</summary>
        public void LoadFromConfig(ModConfig config)
        {
            ModEnabled = config.ModEnabled;

            SpeedMultiplier = config.SpeedMultiplier;
            AddedSpeedBonus = config.AddedSpeedBonus;
            NoClip = config.NoClip;

            InfiniteStamina = config.InfiniteStamina;
            InfiniteHealth = config.InfiniteHealth;
            MaxStaminaOverride = config.MaxStaminaOverride;
            MaxHealthOverride = config.MaxHealthOverride;
            StaminaRegenPerSecond = config.StaminaRegenPerSecond;
            HealthRegenPerSecond = config.HealthRegenPerSecond;

            DamageMultiplier = config.DamageMultiplier;
            OneHitKill = config.OneHitKill;
            AlwaysCrit = config.AlwaysCrit;
            CritDamageMultiplier = config.CritDamageMultiplier;
            AddedDefense = config.AddedDefense;
            AddedAttack = config.AddedAttack;
            AddedImmunity = config.AddedImmunity;
            NoMonsterSpawns = config.NoMonsterSpawns;

            ToolAreaMultiplier = config.ToolAreaMultiplier;
            NoToolStaminaCost = config.NoToolStaminaCost;
            InfiniteWater = config.InfiniteWater;
            OneHitTools = config.OneHitTools;

            MagneticRadiusMultiplier = config.MagneticRadiusMultiplier;
            AddedMagneticRadius = config.AddedMagneticRadius;
            InfiniteItems = config.InfiniteItems;

            FarmingLevelOverride = config.FarmingLevelOverride;
            MiningLevelOverride = config.MiningLevelOverride;
            ForagingLevelOverride = config.ForagingLevelOverride;
            FishingLevelOverride = config.FishingLevelOverride;
            CombatLevelOverride = config.CombatLevelOverride;
            XpMultiplier = config.XPMultiplier;

            AlwaysMaxLuck = config.AlwaysMaxLuck;

            InstantFishBite = config.InstantFishBite;
            InstantCatch = config.InstantCatch;
            MaxFishQuality = config.MaxFishQuality;
            AlwaysFindTreasure = config.AlwaysFindTreasure;

            ForceForageQuality = config.ForceForageQuality;
            SellPriceMultiplier = config.SellPriceMultiplier;
            BuyPriceMultiplier = config.BuyPriceMultiplier;

            FriendshipMultiplier = config.FriendshipMultiplier;
            NoFriendshipDecay = config.NoFriendshipDecay;
            GiveGiftsAnytime = config.GiveGiftsAnytime;

            FreezeTime = config.FreezeTime;
            FreezeTimeIndoors = config.FreezeTimeIndoors;
            FreezeTimeMines = config.FreezeTimeMines;
            NeverPassOut = config.NeverPassOut;

            // Bypass All Doors
            BypassFriendshipDoors = config.BypassFriendshipDoors;
            BypassTimeRestrictions = config.BypassTimeRestrictions;
            BypassFestivalClosures = config.BypassFestivalClosures;
            BypassConditionalDoors = config.BypassConditionalDoors;
            BypassSpecialClosures = config.BypassSpecialClosures;

            // Quests
            AutoAcceptQuests = config.AutoAcceptQuests;
            InfiniteQuestTime = config.InfiniteQuestTime;

            // Time Control
            SetTimeTarget = config.SetTimeTarget > 0 ? config.SetTimeTarget : 600;

            MaxAnimalHappiness = config.MaxAnimalHappiness;
            BuyAnimalsFullyMatured = config.BuyAnimalsFullyMatured;
            AutoPetAnimals = config.AutoPetAnimals;
            AutoFeedAnimals = config.AutoFeedAnimals;
            InfiniteHay = config.InfiniteHay;
            AnimalsProduceDaily = config.AnimalsProduceDaily;
            FarmAnimalHeartsOverride = config.FarmAnimalHeartsOverride;
            PetHeartsOverride = config.PetHeartsOverride;
            CropsNeverDie = config.CropsNeverDie;
            PreventDebrisSpawn = config.PreventDebrisSpawn;
            TilledSoilDontDecay = config.TilledSoilDontDecay;

            // World Cheats
            InstantBuildConstruction = config.InstantBuildConstruction;
            InstantBuildUpgrade = config.InstantBuildUpgrade;
            InstantHouseUpgrade = config.InstantHouseUpgrade;
            InstantCommunityUpgrade = config.InstantCommunityUpgrade;
            FreeBuildingConstruction = config.FreeBuildingConstruction;
            InstantToolUpgrade = config.InstantToolUpgrade;
            InstantMachineProcessing = config.InstantMachineProcessing;
            FreeGeodeProcessing = config.FreeGeodeProcessing;
            FreeShopPurchases = config.FreeShopPurchases;
            FreeCrafting = config.FreeCrafting;
            WeatherIndex = config.WeatherForTomorrow switch
            {
                "Sun" => 1, "Rain" => 2, "Storm" => 3, "Snow" => 4, "Wind" => 5, _ => 0
            };

            // Mining
            ForceLadderChance = config.ForceLadderChance;
        }

        /// <summary>Save all ViewModel values back to the config.</summary>
        public void SaveToConfig(ModConfig config)
        {
            config.ModEnabled = ModEnabled;

            config.SpeedMultiplier = SpeedMultiplier;
            config.AddedSpeedBonus = AddedSpeedBonus;
            config.NoClip = NoClip;

            config.InfiniteStamina = InfiniteStamina;
            config.InfiniteHealth = InfiniteHealth;
            config.MaxStaminaOverride = (int)MaxStaminaOverride;
            config.MaxHealthOverride = (int)MaxHealthOverride;
            config.StaminaRegenPerSecond = StaminaRegenPerSecond;
            config.HealthRegenPerSecond = HealthRegenPerSecond;

            config.DamageMultiplier = DamageMultiplier;
            config.OneHitKill = OneHitKill;
            config.AlwaysCrit = AlwaysCrit;
            config.CritDamageMultiplier = CritDamageMultiplier;
            config.AddedDefense = (int)AddedDefense;
            config.AddedAttack = (int)AddedAttack;
            config.AddedImmunity = (int)AddedImmunity;
            config.NoMonsterSpawns = NoMonsterSpawns;

            config.ToolAreaMultiplier = (int)ToolAreaMultiplier;
            config.NoToolStaminaCost = NoToolStaminaCost;
            config.InfiniteWater = InfiniteWater;
            config.OneHitTools = OneHitTools;

            config.MagneticRadiusMultiplier = MagneticRadiusMultiplier;
            config.AddedMagneticRadius = (int)AddedMagneticRadius;
            config.InfiniteItems = InfiniteItems;

            config.FarmingLevelOverride = (int)FarmingLevelOverride;
            config.MiningLevelOverride = (int)MiningLevelOverride;
            config.ForagingLevelOverride = (int)ForagingLevelOverride;
            config.FishingLevelOverride = (int)FishingLevelOverride;
            config.CombatLevelOverride = (int)CombatLevelOverride;
            config.XPMultiplier = XpMultiplier;

            config.AlwaysMaxLuck = AlwaysMaxLuck;

            config.InstantFishBite = InstantFishBite;
            config.InstantCatch = InstantCatch;
            config.MaxFishQuality = MaxFishQuality;
            config.AlwaysFindTreasure = AlwaysFindTreasure;

            config.ForceForageQuality = (int)ForceForageQuality;
            config.SellPriceMultiplier = SellPriceMultiplier;
            config.BuyPriceMultiplier = BuyPriceMultiplier;

            config.FriendshipMultiplier = FriendshipMultiplier;
            config.NoFriendshipDecay = NoFriendshipDecay;
            config.GiveGiftsAnytime = GiveGiftsAnytime;

            config.FreezeTime = FreezeTime;
            config.FreezeTimeIndoors = FreezeTimeIndoors;
            config.FreezeTimeMines = FreezeTimeMines;
            config.NeverPassOut = NeverPassOut;

            // Bypass All Doors
            config.BypassFriendshipDoors = BypassFriendshipDoors;
            config.BypassTimeRestrictions = BypassTimeRestrictions;
            config.BypassFestivalClosures = BypassFestivalClosures;
            config.BypassConditionalDoors = BypassConditionalDoors;
            config.BypassSpecialClosures = BypassSpecialClosures;

            // Quests
            config.AutoAcceptQuests = AutoAcceptQuests;
            config.InfiniteQuestTime = InfiniteQuestTime;

            // Time Control
            config.SetTimeTarget = (int)SetTimeTarget;

            config.MaxAnimalHappiness = MaxAnimalHappiness;
            config.BuyAnimalsFullyMatured = BuyAnimalsFullyMatured;
            config.AutoPetAnimals = AutoPetAnimals;
            config.AutoFeedAnimals = AutoFeedAnimals;
            config.InfiniteHay = InfiniteHay;
            config.AnimalsProduceDaily = AnimalsProduceDaily;
            config.FarmAnimalHeartsOverride = FarmAnimalHeartsOverride;
            config.PetHeartsOverride = PetHeartsOverride;
            config.CropsNeverDie = CropsNeverDie;
            config.PreventDebrisSpawn = PreventDebrisSpawn;
            config.TilledSoilDontDecay = TilledSoilDontDecay;

            // World Cheats
            config.InstantBuildConstruction = InstantBuildConstruction;
            config.InstantBuildUpgrade = InstantBuildUpgrade;
            config.InstantHouseUpgrade = InstantHouseUpgrade;
            config.InstantCommunityUpgrade = InstantCommunityUpgrade;
            config.FreeBuildingConstruction = FreeBuildingConstruction;
            config.InstantToolUpgrade = InstantToolUpgrade;
            config.InstantMachineProcessing = InstantMachineProcessing;
            config.FreeGeodeProcessing = FreeGeodeProcessing;
            config.FreeShopPurchases = FreeShopPurchases;
            config.FreeCrafting = FreeCrafting;
            config.WeatherForTomorrow = ((int)WeatherIndex) switch
            {
                1 => "Sun", 2 => "Rain", 3 => "Storm", 4 => "Snow", 5 => "Wind", _ => "Default"
            };

            // Mining
            config.ForceLadderChance = (int)ForceLadderChance;
        }

        /// <summary>Reset all cheat values to defaults (preserves ModEnabled and hotkey).</summary>
        public void ResetToDefaults()
        {
            // Preserve non-cheat settings
            bool savedEnabled = ModEntry.Config.ModEnabled;
            var savedKey = ModEntry.Config.OpenMenuKey;

            var defaults = new ModConfig
            {
                ModEnabled = savedEnabled,
                OpenMenuKey = savedKey
            };
            LoadFromConfig(defaults);

            // Immediately apply to the live config so game logic sees defaults right away
            SaveToConfig(ModEntry.Config);

            // Reset game state for things that need immediate reversal
            if (Context.IsWorldReady && Game1.player != null)
            {
                Game1.player.ignoreCollisions = false; // NoClip off
            }
        }

        /// <summary>Save specified cheats to config file for persistence across game restarts.</summary>
        public void SaveToConfigFile()
        {
            var saved = new SavedCheats
            {
                // Player: Movement & Speed (no NoClip)
                SpeedMultiplier = SpeedMultiplier,
                AddedSpeedBonus = AddedSpeedBonus,

                // Player: Health & Stamina
                InfiniteStamina = InfiniteStamina,
                InfiniteHealth = InfiniteHealth,
                MaxStaminaOverride = (int)MaxStaminaOverride,
                MaxHealthOverride = (int)MaxHealthOverride,
                StaminaRegenPerSecond = StaminaRegenPerSecond,
                HealthRegenPerSecond = HealthRegenPerSecond,

                // Combat: All
                DamageMultiplier = DamageMultiplier,
                OneHitKill = OneHitKill,
                AlwaysCrit = AlwaysCrit,
                CritDamageMultiplier = CritDamageMultiplier,
                AddedDefense = (int)AddedDefense,
                AddedAttack = (int)AddedAttack,
                AddedImmunity = (int)AddedImmunity,
                NoMonsterSpawns = NoMonsterSpawns,

                // Skills: XP Multiplier only
                XPMultiplier = XpMultiplier,

                // Tools: All
                ToolAreaMultiplier = (int)ToolAreaMultiplier,
                NoToolStaminaCost = NoToolStaminaCost,
                InfiniteWater = InfiniteWater,
                OneHitTools = OneHitTools,
                InstantToolUpgrade = InstantToolUpgrade,
                FreeCrafting = FreeCrafting,

                // Farming: Crop Settings + Field Protection
                CropsNeverDie = CropsNeverDie,
                ForceForageQuality = (int)ForceForageQuality,
                PreventDebrisSpawn = PreventDebrisSpawn,
                TilledSoilDontDecay = TilledSoilDontDecay,

                // Animals: All except hearts overrides
                MaxAnimalHappiness = MaxAnimalHappiness,
                BuyAnimalsFullyMatured = BuyAnimalsFullyMatured,
                AutoPetAnimals = AutoPetAnimals,
                AutoFeedAnimals = AutoFeedAnimals,
                InfiniteHay = InfiniteHay,
                AnimalsProduceDaily = AnimalsProduceDaily,

                // Fishing: All
                InstantFishBite = InstantFishBite,
                InstantCatch = InstantCatch,
                MaxFishQuality = MaxFishQuality,
                AlwaysFindTreasure = AlwaysFindTreasure,

                // Items: Items and Inventory only
                MagneticRadiusMultiplier = MagneticRadiusMultiplier,
                AddedMagneticRadius = (int)AddedMagneticRadius,
                InfiniteItems = InfiniteItems,

                // Economy: Prices and Shopping only
                SellPriceMultiplier = SellPriceMultiplier,
                BuyPriceMultiplier = BuyPriceMultiplier,
                FreeShopPurchases = FreeShopPurchases,
                FreeGeodeProcessing = FreeGeodeProcessing,

                // Buildings: All
                InstantBuildConstruction = InstantBuildConstruction,
                InstantBuildUpgrade = InstantBuildUpgrade,
                InstantHouseUpgrade = InstantHouseUpgrade,
                InstantCommunityUpgrade = InstantCommunityUpgrade,
                FreeBuildingConstruction = FreeBuildingConstruction,
                InstantMachineProcessing = InstantMachineProcessing,

                // World: Specific ones only
                NeverPassOut = NeverPassOut,
                AlwaysMaxLuck = AlwaysMaxLuck,
                BypassFriendshipDoors = BypassFriendshipDoors,
                BypassTimeRestrictions = BypassTimeRestrictions,
                BypassFestivalClosures = BypassFestivalClosures,
                BypassConditionalDoors = BypassConditionalDoors,
                BypassSpecialClosures = BypassSpecialClosures,
                AutoAcceptQuests = AutoAcceptQuests,
                InfiniteQuestTime = InfiniteQuestTime,

                // Relationships: All
                FriendshipMultiplier = FriendshipMultiplier,
                NoFriendshipDecay = NoFriendshipDecay,
                GiveGiftsAnytime = GiveGiftsAnytime,

                // Mining
                ForceLadderChance = (int)ForceLadderChance
            };

            // Update in-memory config
            ModEntry.Config.Saved = saved;

            // Write clean config to disk (only persistent values + saved cheats)
            var diskConfig = new ModConfig
            {
                ModEnabled = ModEntry.Config.ModEnabled,
                OpenMenuKey = ModEntry.Config.OpenMenuKey,
                Saved = saved
            };
            ModEntry.ModHelper.WriteConfig(diskConfig);

            Game1.addHUDMessage(new HUDMessage(
                ModEntry.ModHelper.Translation.Get("hud.config-saved"),
                HUDMessage.achievement_type));
            ModEntry.ModMonitor.Log("[CKBetterCheatsMenu] Cheats saved to config file.", LogLevel.Info);
        }

        /// <summary>Close the menu. Runtime config stays synced via property change handlers.</summary>
        public void CloseMenu()
        {
            Game1.activeClickableMenu?.exitThisMenu();
        }

        /*********
        ** Value Formatters for Sliders (must be Func<float, string> properties for StardewUI binding)
        *********/
        private static string T(string key) => ModEntry.ModHelper.Translation.Get(key);

        public Func<float, string> FormatMultiplier { get; } = value => $"{value:F1}x";

        public Func<float, string> FormatFlat { get; } = value => $"+{value:F1}";

        public Func<float, string> FormatInt { get; } = value => $"{(int)value}";

        public Func<float, string> FormatLevel { get; } = value =>
        {
            int v = (int)value;
            return v < 0 ? T("format.default") : $"{v}";
        };

        public Func<float, string> FormatQuality { get; } = value =>
        {
            int v = (int)value;
            return v switch
            {
                -1 => T("format.disabled"),
                0 => T("format.normal-quality"),
                1 => T("format.silver"),
                2 => T("format.gold"),
                4 => T("format.iridium"),
                _ => $"{v}"
            };
        };

        public Func<float, string> FormatHearts { get; } = value =>
        {
            int v = (int)value;
            return v < 0 ? T("format.disabled") : $"{v} ♥";
        };

        public Func<float, string> FormatPercent { get; } = value => $"{value * 100:F0}%";

        public Func<float, string> FormatLuck { get; } = value =>
        {
            if (value <= -0.5f) return T("format.disabled");
            return $"{value:F2}";
        };

        public Func<float, string> FormatToolArea { get; } = value =>
        {
            int v = (int)value;
            return v <= 1 ? T("format.tool-area-normal") : $"{v}x{v}";
        };

        public Func<float, string> FormatRadius { get; } = value =>
        {
            int v = (int)value;
            return $"{v}px ({v / 64f:F1} tiles)";
        };

        public Func<float, string> FormatBuyPrice { get; } = value =>
        {
            if (value <= 0.01f) return T("format.buy-free");
            return $"{value:F1}x";
        };

        public Func<float, string> FormatWeather { get; } = value =>
        {
            return ((int)value) switch
            {
                1 => T("format.weather-sunny"),
                2 => T("format.weather-rain"),
                3 => T("format.weather-storm"),
                4 => T("format.weather-snow"),
                5 => T("format.weather-wind"),
                _ => T("format.weather-none")
            };
        };

        public Func<float, string> FormatLadderChance { get; } = value =>
        {
            int v = (int)value;
            if (v <= 0) return T("format.ladder-disabled");
            if (v >= 100) return T("format.ladder-always");
            return $"{v}%";
        };

        public Func<float, string> FormatMoney { get; } = value => $"{(int)value:N0}g";

        public Func<float, string> FormatQiCoins { get; } = value => $"{(int)value:N0}";

        public Func<float, string> FormatTime { get; } = value =>
        {
            int v = (int)value;
            int hours = v / 100;
            int minutes = v % 100;
            // Convert to 12-hour format
            string period = hours >= 12 && hours < 24 ? "PM" : "AM";
            if (hours > 24) { hours -= 24; period = "AM"; }
            int displayHour = hours % 12;
            if (displayHour == 0) displayHour = 12;
            return $"{displayHour}:{minutes:D2} {period}";
        };
    }
}
