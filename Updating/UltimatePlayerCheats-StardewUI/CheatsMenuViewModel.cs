using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PropertyChanged.SourceGenerator;
using StardewModdingAPI;
using StardewValley;

namespace PlayerCheats
{
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
        [Notify] private bool alwaysRun;

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
        [Notify] private float axePowerBonus;
        [Notify] private float pickaxePowerBonus;

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
        [Notify] private float dailyLuckOverride;

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
        ** Relationships
        *********/
        [Notify] private float friendshipMultiplier;
        [Notify] private bool noFriendshipDecay;

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
        ** Constructor — load from config
        *********/
        public CheatsMenuViewModel()
        {
            LoadFromConfig(ModEntry.Config);
            InitTabs();
        }

        private void InitTabs()
        {
            var icons = ModEntry.ModHelper.ModContent.Load<Texture2D>("assets/Icons.png");
            // Custom 16x16 icons in a horizontal strip: [0]=General, [1]=Movement, ... [9]=World
            Tabs = new List<TabData>
            {
                new TabData("General",       icons, new Rectangle(0,   0, 16, 16)),
                new TabData("Movement",      icons, new Rectangle(16,  0, 16, 16)),
                new TabData("Health",        icons, new Rectangle(32,  0, 16, 16)),
                new TabData("Combat",        icons, new Rectangle(48,  0, 16, 16)),
                new TabData("Tools",         icons, new Rectangle(64,  0, 16, 16)),
                new TabData("Items",         icons, new Rectangle(80,  0, 16, 16)),
                new TabData("Skills",        icons, new Rectangle(96,  0, 16, 16)),
                new TabData("Fishing",       icons, new Rectangle(112, 0, 16, 16)),
                new TabData("Economy",       icons, new Rectangle(128, 0, 16, 16)),
                new TabData("World",         icons, new Rectangle(144, 0, 16, 16)),
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
            AlwaysRun = config.AlwaysRun;

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
            AxePowerBonus = config.AxePowerBonus;
            PickaxePowerBonus = config.PickaxePowerBonus;

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
            DailyLuckOverride = config.DailyLuckOverride;

            InstantFishBite = config.InstantFishBite;
            InstantCatch = config.InstantCatch;
            MaxFishQuality = config.MaxFishQuality;
            AlwaysFindTreasure = config.AlwaysFindTreasure;

            ForceForageQuality = config.ForceForageQuality;
            SellPriceMultiplier = config.SellPriceMultiplier;
            BuyPriceMultiplier = config.BuyPriceMultiplier;

            FriendshipMultiplier = config.FriendshipMultiplier;
            NoFriendshipDecay = config.NoFriendshipDecay;

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
        }

        /// <summary>Save all ViewModel values back to the config.</summary>
        public void SaveToConfig(ModConfig config)
        {
            config.ModEnabled = ModEnabled;

            config.SpeedMultiplier = SpeedMultiplier;
            config.AddedSpeedBonus = AddedSpeedBonus;
            config.NoClip = NoClip;
            config.AlwaysRun = AlwaysRun;

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
            config.AxePowerBonus = (int)AxePowerBonus;
            config.PickaxePowerBonus = (int)PickaxePowerBonus;

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
            config.DailyLuckOverride = DailyLuckOverride;

            config.InstantFishBite = InstantFishBite;
            config.InstantCatch = InstantCatch;
            config.MaxFishQuality = MaxFishQuality;
            config.AlwaysFindTreasure = AlwaysFindTreasure;

            config.ForceForageQuality = (int)ForceForageQuality;
            config.SellPriceMultiplier = SellPriceMultiplier;
            config.BuyPriceMultiplier = BuyPriceMultiplier;

            config.FriendshipMultiplier = FriendshipMultiplier;
            config.NoFriendshipDecay = NoFriendshipDecay;

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

        /// <summary>Save and close.</summary>
        public void SaveAndClose()
        {
            SaveToConfig(ModEntry.Config);
            ModEntry.ModHelper.WriteConfig(ModEntry.Config);

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
    }
}
