#nullable enable
namespace CustomAnimalHearts
{
    /// <summary>Configuration settings for Custom Animal Hearts mod.</summary>
    public class ModConfig
    {
        /// <summary>Whether the mod is enabled.</summary>
        public bool Enabled { get; set; } = true;

        /// <summary>Whether to override pet hearts/friendship.</summary>
        public bool OverridePetHearts { get; set; } = false;

        /// <summary>The number of hearts pets will have (0-10). Hearts are calculated as friendshipValue / 100.</summary>
        public int PetHearts { get; set; } = 5;

        /// <summary>Whether to override farm animal hearts/friendship.</summary>
        public bool OverrideFarmAnimalHearts { get; set; } = false;

        /// <summary>The number of hearts farm animals will have (0-10). Hearts are calculated as friendshipValue / 100.</summary>
        public int FarmAnimalHearts { get; set; } = 5;
    }
}
