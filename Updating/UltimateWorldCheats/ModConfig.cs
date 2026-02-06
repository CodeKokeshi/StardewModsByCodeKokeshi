using StardewModdingAPI.Utilities;

namespace WorldCheats
{
    /// <summary>Configuration settings for Ultimate World Cheats mod.</summary>
    public class ModConfig
    {
        /*********
        ** Master Toggle
        *********/

        /// <summary>Enable/disable the entire mod.</summary>
        public bool ModEnabled { get; set; } = true;

        /// <summary>Hotkey to open this mod's config menu directly.</summary>
        public KeybindList OpenMenuKey { get; set; } = new KeybindList(StardewModdingAPI.SButton.L);

        /*********
        ** Buildings & Construction
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
        ** Tool Upgrades
        *********/

        /// <summary>Tool upgrades at the blacksmith complete instantly.</summary>
        public bool InstantToolUpgrade { get; set; } = false;

        /*********
        ** Machines & Processing
        *********/

        /// <summary>All machines produce output instantly (0 processing time).</summary>
        public bool InstantMachineProcessing { get; set; } = false;

        /// <summary>Geode processing at Clint's is free (no 25g cost).</summary>
        public bool FreeGeodeProcessing { get; set; } = false;

        /*********
        ** Shopping & Economy
        *********/

        /// <summary>All shop purchases are free (no gold cost).</summary>
        public bool FreeShopPurchases { get; set; } = false;

        /// <summary>Crafting does not consume ingredients.</summary>
        public bool FreeCrafting { get; set; } = false;

        /*********
        ** Weather Control
        *********/

        /// <summary>Override tomorrow's weather. "Default" = no override.</summary>
        public string WeatherForTomorrow { get; set; } = "Default";
    }
}
