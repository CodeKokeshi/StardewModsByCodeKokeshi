namespace BountifulForaging
{
    /// <summary>Configuration settings for Bountiful Foraging mod.</summary>
    public class ModConfig
    {
        /// <summary>
        /// Multiplier for how much forage to spawn beyond the game's default max.
        /// Range: 1-15, Default: 4
        /// </summary>
        public int ForageMultiplier { get; set; } = 4;

        /// <summary>
        /// Minimum forage to spawn per location even if the game says it should have none.
        /// Range: 1-20, Default: 4
        /// </summary>
        public int MinimumForagePerLocation { get; set; } = 4;

        /// <summary>
        /// Maximum forage per location to prevent performance issues.
        /// Range: 10-50, Default: 50
        /// </summary>
        public int MaxForagePerLocation { get; set; } = 50;
    }
}
