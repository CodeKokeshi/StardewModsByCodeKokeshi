using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace CKBetterCheatsMenu
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

        /// <summary>Whether the mod should show HUD notification messages. Disabled by default.</summary>
        public bool EnableNotifications { get; set; } = false;

        /// <summary>Saved cheats that persist across game restarts.</summary>
        public SavedCheats Saved { get; set; } = new SavedCheats();

        /*********
        ** Movement & Speed
        *********/

        /// <summary>Speed multiplier (1.0 = normal, 2.0 = 2x speed, etc.).</summary>
        public float SpeedMultiplier { get; set; } = 1.0f;

        /// <summary>Additional flat speed bonus added to movement.</summary>
        public float AddedSpeedBonus { get; set; } = 0f;

        /// <summary>Whether player can walk through walls/obstacles (noclip).</summary>
        public bool NoClip { get; set; } = false;

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

        /// <summary>One hit destroy trees (axe) and rocks/boulders (pickaxe).</summary>
        public bool OneHitTools { get; set; } = false;

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

        /// <summary>Give unlimited gifts to NPCs (bypasses daily/weekly limits).</summary>
        public bool GiveGiftsAnytime { get; set; } = false;

        /*********
        ** Time & Energy
        *********/

        /// <summary>Freeze time (time never passes).</summary>
        public bool FreezeTime { get; set; } = false;

        /// <summary>Freeze time indoors only.</summary>
        public bool FreezeTimeIndoors { get; set; } = false;

        /// <summary>Freeze time only within mines/skull cavern/volcano.</summary>
        public bool FreezeTimeMines { get; set; } = false;

        /// <summary>Never pass out at 2am.</summary>
        public bool NeverPassOut { get; set; } = false;

        /*********
        ** Bypass All Doors
        *********/

        /// <summary>Bypass friendship-locked doors (NPC bedrooms requiring 2+ hearts).</summary>
        public bool BypassFriendshipDoors { get; set; } = false;

        /// <summary>Bypass time-locked doors (shop hours).</summary>
        public bool BypassTimeRestrictions { get; set; } = false;

        /// <summary>Bypass festival closures (all shops closed during festivals).</summary>
        public bool BypassFestivalClosures { get; set; } = false;

        /// <summary>Bypass conditional doors (GSQ-based locks).</summary>
        public bool BypassConditionalDoors { get; set; } = false;

        /// <summary>Bypass special closures (Pierre's Wednesday, etc.).</summary>
        public bool BypassSpecialClosures { get; set; } = false;

        /*********
        ** Quests
        *********/

        /// <summary>Automatically accept the daily quest from the Help Wanted board.</summary>
        public bool AutoAcceptQuests { get; set; } = false;

        /// <summary>Quest timers never decrease (infinite quest time).</summary>
        public bool InfiniteQuestTime { get; set; } = false;

        /*********
        ** Animals & Pets
        *********/

        /// <summary>All farm animals are always at max happiness and friendship.</summary>
        public bool MaxAnimalHappiness { get; set; } = false;

        /// <summary>Purchased animals are fully grown immediately.</summary>
        public bool BuyAnimalsFullyMatured { get; set; } = false;

        /// <summary>Automatically pet all farm animals each day.</summary>
        public bool AutoPetAnimals { get; set; } = false;

        /// <summary>Automatically feed all farm animals (fill troughs).</summary>
        public bool AutoFeedAnimals { get; set; } = false;

        /// <summary>Silos always have infinite hay.</summary>
        public bool InfiniteHay { get; set; } = false;

        /// <summary>All animals produce every day regardless of their normal schedule.</summary>
        public bool AnimalsProduceDaily { get; set; } = false;

        /// <summary>Override farm animal hearts (-1 = disabled, 0-10 = hearts).</summary>
        public int FarmAnimalHeartsOverride { get; set; } = -1;

        /// <summary>Override pet hearts (-1 = disabled, 0-10 = hearts).</summary>
        public int PetHeartsOverride { get; set; } = -1;

        /// <summary>Crops never die from season change or lack of water.</summary>
        public bool CropsNeverDie { get; set; } = false;

        /// <summary>Prevent debris (weeds, stones, twigs) from spawning on the farm.</summary>
        public bool PreventDebrisSpawn { get; set; } = false;

        /// <summary>Tilled soil stays tilled indefinitely.</summary>
        public bool TilledSoilDontDecay { get; set; } = false;

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

        /*********
        ** Mining
        *********/

        /// <summary>Force ladder spawn chance when breaking rocks in mines (0 = disabled, 100 = always spawn).</summary>
        public int ForceLadderChance { get; set; } = 0;

        /*********
        ** Time Override (not persisted, action-only)
        *********/

        /// <summary>Current time override target for the Set Time slider (600-2600). Not persisted.</summary>
        public int SetTimeTarget { get; set; } = 600;
    }

    /// <summary>Cheats that persist across game restarts when saved via "Save to Config".</summary>
    public class SavedCheats
    {
        /*********
        ** Player: Movement & Speed (no NoClip)
        *********/
        public float SpeedMultiplier { get; set; } = 1.0f;
        public float AddedSpeedBonus { get; set; } = 0f;

        /*********
        ** Player: Health & Stamina
        *********/
        public bool InfiniteStamina { get; set; } = false;
        public bool InfiniteHealth { get; set; } = false;
        public int MaxStaminaOverride { get; set; } = 0;
        public int MaxHealthOverride { get; set; } = 0;
        public float StaminaRegenPerSecond { get; set; } = 0f;
        public float HealthRegenPerSecond { get; set; } = 0f;

        /*********
        ** Combat: All
        *********/
        public float DamageMultiplier { get; set; } = 1.0f;
        public bool OneHitKill { get; set; } = false;
        public bool AlwaysCrit { get; set; } = false;
        public float CritDamageMultiplier { get; set; } = 1.0f;
        public int AddedDefense { get; set; } = 0;
        public int AddedAttack { get; set; } = 0;
        public int AddedImmunity { get; set; } = 0;
        public bool NoMonsterSpawns { get; set; } = false;

        /*********
        ** Skills: XP Multiplier only (no level overrides)
        *********/
        public float XPMultiplier { get; set; } = 1.0f;

        /*********
        ** Tools: All
        *********/
        public int ToolAreaMultiplier { get; set; } = 1;
        public bool NoToolStaminaCost { get; set; } = false;
        public bool InfiniteWater { get; set; } = false;
        public bool OneHitTools { get; set; } = false;
        public bool InstantToolUpgrade { get; set; } = false;
        public bool FreeCrafting { get; set; } = false;

        /*********
        ** Farming: Crop Settings + Field Protection (no instant growth)
        *********/
        public bool CropsNeverDie { get; set; } = false;
        public int ForceForageQuality { get; set; } = -1;
        public bool PreventDebrisSpawn { get; set; } = false;
        public bool TilledSoilDontDecay { get; set; } = false;

        /*********
        ** Animals: All except hearts overrides
        *********/
        public bool MaxAnimalHappiness { get; set; } = false;
        public bool BuyAnimalsFullyMatured { get; set; } = false;
        public bool AutoPetAnimals { get; set; } = false;
        public bool AutoFeedAnimals { get; set; } = false;
        public bool InfiniteHay { get; set; } = false;
        public bool AnimalsProduceDaily { get; set; } = false;

        /*********
        ** Fishing: All
        *********/
        public bool InstantFishBite { get; set; } = false;
        public bool InstantCatch { get; set; } = false;
        public bool MaxFishQuality { get; set; } = false;
        public bool AlwaysFindTreasure { get; set; } = false;

        /*********
        ** Items: Items and Inventory only (no backpack, no recipes)
        *********/
        public float MagneticRadiusMultiplier { get; set; } = 1.0f;
        public int AddedMagneticRadius { get; set; } = 0;
        public bool InfiniteItems { get; set; } = false;

        /*********
        ** Economy: Prices and Shopping only (no currency adding)
        *********/
        public float SellPriceMultiplier { get; set; } = 1.0f;
        public float BuyPriceMultiplier { get; set; } = 1.0f;
        public bool FreeShopPurchases { get; set; } = false;
        public bool FreeGeodeProcessing { get; set; } = false;

        /*********
        ** Buildings: All
        *********/
        public bool InstantBuildConstruction { get; set; } = false;
        public bool InstantBuildUpgrade { get; set; } = false;
        public bool InstantHouseUpgrade { get; set; } = false;
        public bool InstantCommunityUpgrade { get; set; } = false;
        public bool FreeBuildingConstruction { get; set; } = false;
        public bool InstantMachineProcessing { get; set; } = false;

        /*********
        ** World: Specific ones only
        *********/
        public bool NeverPassOut { get; set; } = false;
        public bool AlwaysMaxLuck { get; set; } = false;
        public bool BypassFriendshipDoors { get; set; } = false;
        public bool BypassTimeRestrictions { get; set; } = false;
        public bool BypassFestivalClosures { get; set; } = false;
        public bool BypassConditionalDoors { get; set; } = false;
        public bool BypassSpecialClosures { get; set; } = false;
        public bool AutoAcceptQuests { get; set; } = false;
        public bool InfiniteQuestTime { get; set; } = false;

        /*********
        ** Relationships: All
        *********/
        public float FriendshipMultiplier { get; set; } = 1.0f;
        public bool NoFriendshipDecay { get; set; } = false;
        public bool GiveGiftsAnytime { get; set; } = false;

        /*********
        ** Mining: All
        *********/
        public int ForceLadderChance { get; set; } = 0;
    }
}
