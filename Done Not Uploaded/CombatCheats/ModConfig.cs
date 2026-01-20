namespace CombatCheats
{
    /// <summary>Configuration settings for Combat Cheats mod.</summary>
    public class ModConfig
    {
        /// <summary>One-hit kill all enemies (sets their health to 1 when damaged).</summary>
        public bool OneHitKill { get; set; } = false;

        /// <summary>Infinite HP - player takes no damage.</summary>
        public bool InfiniteHP { get; set; } = false;

        /// <summary>Max drop rate - all monsters drop all their possible items.</summary>
        public bool MaxDropRate { get; set; } = false;

        /// <summary>100% crit chance on all weapon attacks.</summary>
        public bool HundredPercentCrit { get; set; } = false;
    }
}
