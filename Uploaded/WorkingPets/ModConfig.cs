namespace WorkingPets
{
    /// <summary>Configuration settings for Working Pets mod.</summary>
    public class ModConfig
    {
        /*********
        ** General Settings
        *********/

        /// <summary>Whether the mod is enabled.</summary>
        public bool ModEnabled { get; set; } = true;

        /// <summary>Number of game ticks between each work action (~60 ticks = 1 second).</summary>
        public int TicksBetweenActions { get; set; } = 60;

        /// <summary>Maximum distance (in tiles) the pet will work from its current position.</summary>
        public int WorkRadius { get; set; } = 30;

        /// <summary>Number of inventory slots for the pet (max 36).</summary>
        public int InventorySize { get; set; } = 36;

        /// <summary>Whether to show floating text when pet clears something.</summary>
        public bool ShowWorkingMessages { get; set; } = true;

        /// <summary>Whether the pet can follow you outside the farm.</summary>
        public bool FollowOutsideFarm { get; set; } = false;

        /*********
        ** Work Type Toggles
        *********/

        /// <summary>Whether to clear debris (weeds, small stones, twigs).</summary>
        public bool ClearDebris { get; set; } = true;

        /// <summary>Whether to clear stumps and logs.</summary>
        public bool ClearStumpsAndLogs { get; set; } = false;

        /// <summary>Whether to chop down fully grown trees.</summary>
        public bool ChopTrees { get; set; } = false;

        /// <summary>Whether to break large boulders.</summary>
        public bool BreakBoulders { get; set; } = false;

        /*********
        ** Priority Settings (lower number = higher priority, 1-99)
        *********/

        /// <summary>Priority for clearing debris (1 = highest priority).</summary>
        public int DebrisPriority { get; set; } = 1;

        /// <summary>Priority for clearing stumps and logs.</summary>
        public int StumpsAndLogsPriority { get; set; } = 2;

        /// <summary>Priority for chopping trees.</summary>
        public int TreesPriority { get; set; } = 3;

        /// <summary>Priority for breaking boulders.</summary>
        public int BouldersPriority { get; set; } = 4;

        /// <summary>If true, ignores priority and attacks nearest target regardless of type.</summary>
        public bool IgnorePriority { get; set; } = false;
    }
}
