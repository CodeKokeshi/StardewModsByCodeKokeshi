namespace BypassAllDoors
{
    /// <summary>Configuration settings for Bypass All Doors mod.</summary>
    public class ModConfig
    {
        /// <summary>Whether to bypass friendship-locked doors (NPC bedrooms requiring 2+ hearts).</summary>
        public bool BypassFriendshipDoors { get; set; } = true;

        /// <summary>Whether to bypass time-locked doors (shop hours).</summary>
        public bool BypassTimeRestrictions { get; set; } = true;

        /// <summary>Whether to bypass festival closures (all shops closed during festivals).</summary>
        public bool BypassFestivalClosures { get; set; } = true;

        /// <summary>Whether to bypass conditional doors (GSQ-based locks).</summary>
        public bool BypassConditionalDoors { get; set; } = true;

        /// <summary>Whether to bypass special closures (Pierre's Wednesday, etc.).</summary>
        public bool BypassSpecialClosures { get; set; } = true;
    }
}
