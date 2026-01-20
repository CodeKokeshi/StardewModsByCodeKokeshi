using System;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Tools;

namespace InstantUpgradeTools
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
            helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
            helper.Events.GameLoop.DayStarted += this.OnDayStarted;
        }

        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the game state is updated (≈60 times per second).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            // Check if player has a tool being upgraded and instantly complete it
            if (!Context.IsWorldReady)
                return;

            if (Game1.player.toolBeingUpgraded.Value != null && Game1.player.daysLeftForToolUpgrade.Value > 0)
            {
                this.Monitor.Log($"Tool upgrade detected: {Game1.player.toolBeingUpgraded.Value.DisplayName}. Setting days to 0 for instant completion.", LogLevel.Debug);
                Game1.player.daysLeftForToolUpgrade.Value = 0;
            }
        }

        /// <summary>Raised after the player loads a save.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            // Check if there's a tool being upgraded when loading the game
            if (Game1.player.toolBeingUpgraded.Value != null && Game1.player.daysLeftForToolUpgrade.Value > 0)
            {
                this.Monitor.Log($"Found tool being upgraded on save load: {Game1.player.toolBeingUpgraded.Value.DisplayName}. Completing instantly.", LogLevel.Info);
                Game1.player.daysLeftForToolUpgrade.Value = 0;
            }
        }

        /// <summary>Raised after the game begins a new day (including when the player loads a save).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            // Additional check at day start to ensure any pending upgrades are completed
            if (Game1.player.toolBeingUpgraded.Value != null && Game1.player.daysLeftForToolUpgrade.Value > 0)
            {
                this.Monitor.Log($"Tool upgrade pending at day start: {Game1.player.toolBeingUpgraded.Value.DisplayName}. Completing instantly.", LogLevel.Info);
                Game1.player.daysLeftForToolUpgrade.Value = 0;
            }
        }
    }
}
