using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PropertyChanged.SourceGenerator;
using StardewModdingAPI;
using StardewValley;

namespace PlayerCheats
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
        [Notify] private bool neverPassOut;

        /*********
        ** Miscellaneous
        *********/
        [Notify] private bool maxAnimalHappiness;
        [Notify] private bool cropsNeverDie;
        [Notify] private bool instantCropGrowth;
        [Notify] private bool instantTreeGrowth;
        [Notify] private bool instantFruitTreeGrowth;

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
            ModEntry.ModMonitor.Log($"[PlayerCheats] Warped to {location.DisplayName} ({location.LocationName}) at ({location.TileX}, {location.TileY})", LogLevel.Info);
        }

        /// <summary>Add money to the player's wallet and profit tracking.</summary>
        public void AddMoney()
        {
            if (!Context.IsWorldReady || AddMoneyAmount <= 0)
                return;

            int amount = (int)AddMoneyAmount;
            Game1.player.Money += amount;
            Game1.player.totalMoneyEarned += (uint)amount;
            Game1.addHUDMessage(new HUDMessage($"Added {amount:N0}g to wallet!", HUDMessage.achievement_type));
            ModEntry.ModMonitor.Log($"[PlayerCheats] Added {amount}g to player. New total: {Game1.player.Money}g", LogLevel.Info);
        }

        /// <summary>Add casino coins (Qi coins) to the player.</summary>
        public void AddCasinoCoins()
        {
            if (!Context.IsWorldReady || AddCasinoCoinsAmount <= 0)
                return;

            int amount = (int)AddCasinoCoinsAmount;
            Game1.player.clubCoins += amount;
            Game1.addHUDMessage(new HUDMessage($"Added {amount:N0} Qi Coins!", HUDMessage.achievement_type));
            ModEntry.ModMonitor.Log($"[PlayerCheats] Added {amount} Qi Coins. New total: {Game1.player.clubCoins}", LogLevel.Info);
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

            ModEntry.ModMonitor.Log($"[PlayerCheats] Loaded {WarpLocations.Count} warp locations.", LogLevel.Trace);
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
            NeverPassOut = config.NeverPassOut;

            MaxAnimalHappiness = config.MaxAnimalHappiness;
            CropsNeverDie = config.CropsNeverDie;
            InstantCropGrowth = config.InstantCropGrowth;
            InstantTreeGrowth = config.InstantTreeGrowth;
            InstantFruitTreeGrowth = config.InstantFruitTreeGrowth;

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
            config.NeverPassOut = NeverPassOut;

            config.MaxAnimalHappiness = MaxAnimalHappiness;
            config.CropsNeverDie = CropsNeverDie;
            config.InstantCropGrowth = InstantCropGrowth;
            config.InstantTreeGrowth = InstantTreeGrowth;
            config.InstantFruitTreeGrowth = InstantFruitTreeGrowth;

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

        /// <summary>Reset all values to defaults and immediately apply to the live config.</summary>
        public void ResetToDefaults()
        {
            var defaults = new ModConfig();
            LoadFromConfig(defaults);

            // Immediately apply to the live config so game logic sees defaults right away
            SaveToConfig(ModEntry.Config);
            ModEntry.ModHelper.WriteConfig(ModEntry.Config);

            // Reset game state for things that need immediate reversal
            if (Context.IsWorldReady && Game1.player != null)
            {
                Game1.player.ignoreCollisions = false; // NoClip off
            }
        }

        /// <summary>Save to memory config (not persisted to file) and close.</summary>
        public void SaveAndClose()
        {
            SaveToConfig(ModEntry.Config);
            // NOTE: We intentionally do NOT call ModEntry.ModHelper.WriteConfig()
            // This ensures cheats reset when the game restarts or player returns to title.

            // Apply game state for things that need immediate effect
            if (Context.IsWorldReady && Game1.player != null)
            {
                Game1.player.ignoreCollisions = ModEntry.Config.NoClip;
            }

            Game1.activeClickableMenu?.exitThisMenu();
        }

        /// <summary>Close without saving.</summary>
        public void CloseMenu()
        {
            Game1.activeClickableMenu?.exitThisMenu();
        }

        /*********
        ** Value Formatters for Sliders (must be Func<float, string> properties for StardewUI binding)
        *********/
        public Func<float, string> FormatMultiplier { get; } = value => $"{value:F1}x";

        public Func<float, string> FormatFlat { get; } = value => $"+{value:F1}";

        public Func<float, string> FormatInt { get; } = value => $"{(int)value}";

        public Func<float, string> FormatLevel { get; } = value =>
        {
            int v = (int)value;
            return v < 0 ? "Default" : $"{v}";
        };

        public Func<float, string> FormatQuality { get; } = value =>
        {
            int v = (int)value;
            return v switch
            {
                -1 => "Disabled",
                0 => "Normal",
                1 => "Silver",
                2 => "Gold",
                4 => "Iridium",
                _ => $"{v}"
            };
        };

        public Func<float, string> FormatPercent { get; } = value => $"{value * 100:F0}%";

        public Func<float, string> FormatLuck { get; } = value =>
        {
            if (value <= -0.5f) return "Disabled";
            return $"{value:F2}";
        };

        public Func<float, string> FormatToolArea { get; } = value =>
        {
            int v = (int)value;
            return v <= 1 ? "Normal" : $"{v}x{v}";
        };

        public Func<float, string> FormatRadius { get; } = value =>
        {
            int v = (int)value;
            return $"{v}px ({v / 64f:F1} tiles)";
        };

        public Func<float, string> FormatBuyPrice { get; } = value =>
        {
            if (value <= 0.01f) return "Free!";
            return $"{value:F1}x";
        };

        public Func<float, string> FormatWeather { get; } = value =>
        {
            return ((int)value) switch
            {
                1 => "Sunny",
                2 => "Rain",
                3 => "Storm",
                4 => "Snow",
                5 => "Wind",
                _ => "No Override"
            };
        };

        public Func<float, string> FormatLadderChance { get; } = value =>
        {
            int v = (int)value;
            if (v <= 0) return "Disabled";
            if (v >= 100) return "Always";
            return $"{v}%";
        };

        public Func<float, string> FormatMoney { get; } = value => $"{(int)value:N0}g";

        public Func<float, string> FormatQiCoins { get; } = value => $"{(int)value:N0}";
    }
}
