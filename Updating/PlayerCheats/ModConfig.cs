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

        /// <summary>Attack speed multiplier (1.0 = normal).</summary>
        public float AttackSpeedMultiplier { get; set; } = 1.0f;

        /// <summary>Additional defense points.</summary>
        public int AddedDefense { get; set; } = 0;

        /// <summary>Additional attack points.</summary>
        public int AddedAttack { get; set; } = 0;

        /// <summary>Additional immunity points.</summary>
        public int AddedImmunity { get; set; } = 0;

        /// <summary>Invincibility duration after getting hit (ms, default 1200).</summary>
        public int InvincibilityDuration { get; set; } = 1200;

        /*********
        ** Tools & Farming
        *********/

        /// <summary>Tool power level override (-1 = normal, 0-5 = power level).</summary>
        public int ToolPowerOverride { get; set; } = -1;

        /// <summary>Tool area multiplier for hoe/watering can (1 = normal, 2 = 2x area, etc.).</summary>
        public int ToolAreaMultiplier { get; set; } = 1;

        /// <summary>No stamina cost for tools.</summary>
        public bool NoToolStaminaCost { get; set; } = false;

        /// <summary>Instant tool charge (no holding required).</summary>
        public bool InstantToolCharge { get; set; } = false;

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

        /// <summary>Max inventory slots override (0 = use default, max 36).</summary>
        public int MaxInventoryOverride { get; set; } = 0;

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

        /// <summary>Daily luck override (-1.0 = normal, range -0.1 to 0.1).</summary>
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

        /// <summary>Force quality for all harvested crops (0=normal, 1=silver, 2=gold, 4=iridium, -1=disabled).</summary>
        public int ForceHarvestQuality { get; set; } = -1;

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

        /// <summary>Gifts never rejected.</summary>
        public bool GiftsNeverRejected { get; set; } = false;

        /// <summary>No gift limit per day/week.</summary>
        public bool NoGiftLimit { get; set; } = false;

        /*********
        ** Time & Energy
        *********/

        /// <summary>Freeze time (time never passes).</summary>
        public bool FreezeTime { get; set; } = false;

        /// <summary>Freeze time indoors only.</summary>
        public bool FreezeTimeIndoors { get; set; } = false;

        /// <summary>Time speed multiplier (1.0 = normal, 0.5 = half speed).</summary>
        public float TimeSpeedMultiplier { get; set; } = 1.0f;

        /// <summary>Never pass out at 2am.</summary>
        public bool NeverPassOut { get; set; } = false;

        /*********
        ** Misc Cheats
        *********/

        /// <summary>Walk on water.</summary>
        public bool WalkOnWater { get; set; } = false;

        /// <summary>See in caves/mines without lantern.</summary>
        public bool AlwaysBright { get; set; } = false;

        /// <summary>No monster spawns.</summary>
        public bool NoMonsterSpawns { get; set; } = false;

        /// <summary>Animals always max friendship/happiness.</summary>
        public bool MaxAnimalHappiness { get; set; } = false;

        /// <summary>Crops never die from season change or lack of water.</summary>
        public bool CropsNeverDie { get; set; } = false;

        /// <summary>Instant crop growth (grows to harvest in one day).</summary>
        public bool InstantCropGrowth { get; set; } = false;
    }
}
