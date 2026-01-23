namespace BountifulForaging
{
    /// <summary>Configuration settings for Bountiful Foraging mod.</summary>
    public class ModConfig
    {
        // ==========================================
        // FORAGE SPAWN SETTINGS (vanilla: LocationData)
        // ==========================================

        /// <summary>
        /// Minimum forage items to try spawning per day.
        /// Vanilla default varies by location (typically 1-2).
        /// </summary>
        public int MinDailyForageSpawn { get; set; } = 10;

        /// <summary>
        /// Maximum forage items to try spawning per day.
        /// Vanilla default varies by location (typically 4-6).
        /// </summary>
        public int MaxDailyForageSpawn { get; set; } = 25;

        /// <summary>
        /// Maximum forage items that can exist in a location at once.
        /// Vanilla default is typically 6.
        /// </summary>
        public int MaxSpawnedForageAtOnce { get; set; } = 50;

        /// <summary>
        /// Multiplier for individual forage item spawn chance.
        /// Vanilla chances are typically 0.5-1.0. This multiplies that.
        /// 100 = vanilla, 200 = double chance, etc.
        /// </summary>
        public int ForageChancePercent { get; set; } = 150;

        // ==========================================
        // ARTIFACT SPOT (DIGGING SPOT) SETTINGS
        // ==========================================

        /// <summary>
        /// Whether to boost artifact spot spawning.
        /// </summary>
        public bool BoostArtifactSpots { get; set; } = true;

        /// <summary>
        /// Number of extra artifact spot spawn attempts per day.
        /// Vanilla spawns ~1-3 on average. This adds extra attempts.
        /// </summary>
        public int ExtraArtifactSpotAttempts { get; set; } = 15;

        /// <summary>
        /// Chance that an artifact spot is a Seed Spot (4-leaf clover) instead of regular artifact spot.
        /// Vanilla is ~16.6%. Range: 0-100.
        /// </summary>
        public int SeedSpotChancePercent { get; set; } = 17;

        // ==========================================
        // BEACH SPECIAL ITEMS (hardcoded in vanilla)
        // ==========================================

        /// <summary>
        /// Whether to boost beach-specific items (coral, sea urchins).
        /// These use special hardcoded spawning in vanilla, not SpawnForageData.
        /// </summary>
        public bool BoostBeachItems { get; set; } = true;

        /// <summary>
        /// Extra coral and nautilus shell spawns on the beach tidepool area.
        /// Vanilla spawns ~1-2 on average.
        /// </summary>
        public int ExtraBeachCoralSpawns { get; set; } = 10;

        /// <summary>
        /// Extra sea urchin spawns on the beach shore.
        /// Vanilla spawns ~0-1 on average (very rare).
        /// </summary>
        public int ExtraBeachUrchinSpawns { get; set; } = 5;
    }
}
