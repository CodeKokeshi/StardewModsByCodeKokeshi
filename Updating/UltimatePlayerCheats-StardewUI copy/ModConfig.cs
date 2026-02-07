using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace PlayerCheats
{
    /// <summary>Configuration settings for Player Cheats mod.</summary>
    public class ModConfig
    {
        /*********
        ** Master Toggle
        *********/

        /// <summary>Enable/disable the entire mod.</summary>
        public bool ModEnabled { get; set; } = true;

        /// <summary>Hotkey to open the cheats menu.</summary>
        public KeybindList OpenMenuKey { get; set; } = new KeybindList(SButton.K);

        /*********
        ** Movement & Speed
        *********/

        /// <summary>Speed multiplier (1.0 = normal, 2.0 = 2x speed, etc.).</summary>
        public float SpeedMultiplier { get; set; } = 1.0f;

        /// <summary>Additional flat speed bonus added to movement.</summary>
        public float AddedSpeedBonus { get; set; } = 0f;

        /// <summary>Whether player can walk through walls/obstacles (noclip).</summary>
        public bool NoClip { get; set; } = false;

        /// <summary>Whether running is always enabled (no walking).</summary>
        public bool AlwaysRun { get; set; } = false;

        /*********
        ** Health & Stamina
        *********/

        /// <summary>Infinite stamina - never get tired.</summary>
        public bool InfiniteStamina { get; set; } = false;

        /// <summary>Infinite health - never die (invincibility).</summary>
        public bool InfiniteHealth { get; set; } = false;

        /// <summary>Max stamina override (0 = use default).</summary>
        public int MaxStaminaOverride { get; set; } = 0;

        /// <summary>Max health override (0 = use default).</summary>
        public int MaxHealthOverride { get; set; } = 0;

        /// <summary>Stamina regen rate per second (0 = normal).</summary>
        public float StaminaRegenPerSecond { get; set; } = 0f;

        /// <summary>Health regen rate per second (0 = normal).</summary>
        public float HealthRegenPerSecond { get; set; } = 0f;

        /*********
        ** Combat
        *********/

        /// <summary>Damage multiplier for weapons (1.0 = normal).</summary>
        public float DamageMultiplier { get; set; } = 1.0f;

        /// <summary>One-hit kill all enemies.</summary>
        public bool OneHitKill { get; set; } = false;

        /// <summary>100% critical hit chance.</summary>
        public bool AlwaysCrit { get; set; } = false;

        /// <summary>Critical damage multiplier (1.0 = normal).</summary>
        public float CritDamageMultiplier { get; set; } = 1.0f;

        /// <summary>Additional defense points.</summary>
        public int AddedDefense { get; set; } = 0;

        /// <summary>Additional attack points.</summary>
        public int AddedAttack { get; set; } = 0;

        /// <summary>Additional immunity points.</summary>
        public int AddedImmunity { get; set; } = 0;

        /// <summary>No monster spawns.</summary>
        public bool NoMonsterSpawns { get; set; } = false;

        /*********
        ** Tools & Farming
        *********/

        /// <summary>Tool area multiplier for hoe/watering can (1 = normal, 3 = 3x3, 5 = 5x5).</summary>
        public int ToolAreaMultiplier { get; set; } = 1;

        /// <summary>No stamina cost for tools.</summary>
        public bool NoToolStaminaCost { get; set; } = false;

        /// <summary>Watering can never runs out of water.</summary>
        public bool InfiniteWater { get; set; } = false;

        /// <summary>Axe power bonus (0 = normal).</summary>
        public int AxePowerBonus { get; set; } = 0;

        /// <summary>Pickaxe power bonus (0 = normal).</summary>
        public int PickaxePowerBonus { get; set; } = 0;

        /*********
        ** Item Pickup & Inventory
        *********/

        /// <summary>Magnetic radius multiplier (1.0 = normal, 2.0 = 2x range).</summary>
        public float MagneticRadiusMultiplier { get; set; } = 1.0f;

        /// <summary>Additional magnetic radius in pixels (128 = 2 tiles).</summary>
        public int AddedMagneticRadius { get; set; } = 0;

        /// <summary>Items in inventory never reduce (infinite items).</summary>
        public bool InfiniteItems { get; set; } = false;

        /*********
        ** Skills & Levels
        *********/

        /// <summary>Farming level override (-1 = normal).</summary>
        public int FarmingLevelOverride { get; set; } = -1;

        /// <summary>Mining level override (-1 = normal).</summary>
        public int MiningLevelOverride { get; set; } = -1;

        /// <summary>Foraging level override (-1 = normal).</summary>
        public int ForagingLevelOverride { get; set; } = -1;

        /// <summary>Fishing level override (-1 = normal).</summary>
        public int FishingLevelOverride { get; set; } = -1;

        /// <summary>Combat level override (-1 = normal).</summary>
        public int CombatLevelOverride { get; set; } = -1;

        /// <summary>XP multiplier for all skills (1.0 = normal).</summary>
        public float XPMultiplier { get; set; } = 1.0f;

        /*********
        ** Luck & Daily
        *********/

        /// <summary>Daily luck override (-1.0 = normal, range -0.1 to 0.12).</summary>
        public float DailyLuckOverride { get; set; } = -1.0f;

        /// <summary>Always max luck day.</summary>
        public bool AlwaysMaxLuck { get; set; } = false;

        /*********
        ** Fishing
        *********/

        /// <summary>Instant fish bite (fish bite immediately).</summary>
        public bool InstantFishBite { get; set; } = false;

        /// <summary>Always perfect catch (easy fishing).</summary>
        public bool AlwaysPerfectCatch { get; set; } = false;

        /// <summary>Max fish quality (always iridium).</summary>
        public bool MaxFishQuality { get; set; } = false;

        /// <summary>No fishing minigame (auto-catch).</summary>
        public bool InstantCatch { get; set; } = false;

        /// <summary>Increased treasure chance.</summary>
        public bool AlwaysFindTreasure { get; set; } = false;

        /*********
        ** Quality & Prices
        *********/

        /// <summary>Force quality for foraged items (-1=disabled, 0=normal, 1=silver, 2=gold, 4=iridium).</summary>
        public int ForceForageQuality { get; set; } = -1;

        /// <summary>Sell price multiplier (1.0 = normal).</summary>
        public float SellPriceMultiplier { get; set; } = 1.0f;

        /// <summary>Buy price multiplier (1.0 = normal, 0.5 = half price).</summary>
        public float BuyPriceMultiplier { get; set; } = 1.0f;

        /*********
        ** Relationships
        *********/

        /// <summary>Friendship gain multiplier (1.0 = normal).</summary>
        public float FriendshipMultiplier { get; set; } = 1.0f;

        /// <summary>No friendship decay.</summary>
        public bool NoFriendshipDecay { get; set; } = false;

        /*********
        ** Time & Energy
        *********/

        /// <summary>Freeze time (time never passes).</summary>
        public bool FreezeTime { get; set; } = false;

        /// <summary>Freeze time indoors only.</summary>
        public bool FreezeTimeIndoors { get; set; } = false;

        /// <summary>Never pass out at 2am.</summary>
        public bool NeverPassOut { get; set; } = false;

        /*********
        ** Misc Cheats
        *********/

        /// <summary>No monster spawns in current location.</summary>
        public bool MaxAnimalHappiness { get; set; } = false;

        /// <summary>Crops never die from season change or lack of water.</summary>
        public bool CropsNeverDie { get; set; } = false;

        /// <summary>Instant crop growth.</summary>
        public bool InstantCropGrowth { get; set; } = false;

        /// <summary>Instant tree growth — regular trees grow to full size immediately.</summary>
        public bool InstantTreeGrowth { get; set; } = false;

        /// <summary>Instant fruit tree growth — fruit trees grow to full maturity immediately.</summary>
        public bool InstantFruitTreeGrowth { get; set; } = false;

        /*********
        ** Buildings & Construction (from World Cheats)
        *********/

        /// <summary>Buildings finish constructing instantly when placed.</summary>
        public bool InstantBuildConstruction { get; set; } = false;

        /// <summary>Building upgrades complete instantly.</summary>
        public bool InstantBuildUpgrade { get; set; } = false;

        /// <summary>Farmhouse upgrades complete instantly.</summary>
        public bool InstantHouseUpgrade { get; set; } = false;

        /// <summary>Community upgrades (Pam's house, shortcuts) complete instantly.</summary>
        public bool InstantCommunityUpgrade { get; set; } = false;

        /// <summary>Buildings cost no gold or materials to construct.</summary>
        public bool FreeBuildingConstruction { get; set; } = false;

        /*********
        ** Tool Upgrades (from World Cheats)
        *********/

        /// <summary>Tool upgrades at the blacksmith complete instantly.</summary>
        public bool InstantToolUpgrade { get; set; } = false;

        /*********
        ** Machines & Processing (from World Cheats)
        *********/

        /// <summary>All machines produce output instantly (0 processing time).</summary>
        public bool InstantMachineProcessing { get; set; } = false;

        /// <summary>Geode processing at Clint's is free (no 25g cost).</summary>
        public bool FreeGeodeProcessing { get; set; } = false;

        /*********
        ** Shopping & Economy (from World Cheats)
        *********/

        /// <summary>All shop purchases are free (no gold cost).</summary>
        public bool FreeShopPurchases { get; set; } = false;

        /// <summary>Crafting does not consume ingredients.</summary>
        public bool FreeCrafting { get; set; } = false;

        /*********
        ** Weather Control (from World Cheats)
        *********/

        /// <summary>Override tomorrow's weather. "Default" = no override.</summary>
        public string WeatherForTomorrow { get; set; } = "Default";
    }
}
