namespace WorkingPets
{
    /// <summary>Configuration settings for Working Pets mod.</summary>
    public class ModConfig
    {
        /// <summary>Whether the mod is enabled.</summary>
        public bool ModEnabled { get; set; } = true;

        /// <summary>Number of game ticks between each work action (~60 ticks = 1 second).</summary>
        public int TicksBetweenActions { get; set; } = 60;

        /// <summary>Whether to clear weeds.</summary>
        public bool ClearWeeds { get; set; } = true;

        /// <summary>Whether to clear small stones.</summary>
        public bool ClearStones { get; set; } = true;

        /// <summary>Whether to clear sticks/twigs.</summary>
        public bool ClearSticks { get; set; } = true;

        /// <summary>Whether to chop down fully grown trees.</summary>
        public bool ChopTrees { get; set; } = true;

        /// <summary>Whether to remove tree stumps.</summary>
        public bool ChopStumps { get; set; } = true;

        /// <summary>Maximum distance (in tiles) the pet will work from its current position.</summary>
        public int WorkRadius { get; set; } = 30;

        /// <summary>Number of inventory slots for the pet (max 36).</summary>
        public int InventorySize { get; set; } = 36;

        /// <summary>Whether to show floating text when pet clears something.</summary>
        public bool ShowWorkingMessages { get; set; } = true;
    }
}
