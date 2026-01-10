using System;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Tools;

namespace InstantFishBite;

public class ModEntry : Mod
{
    public override void Entry(IModHelper helper)
    {
        helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
    }

    private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
    {
        // Check if world is ready and player exists
        if (!Context.IsWorldReady || Game1.player == null)
            return;

        // Check if player is holding a fishing rod
        if (Game1.player.CurrentTool is FishingRod rod)
        {
            // If the rod is in the water (isFishing) but not yet nibbling/biting
            // We want to force the bite timer to 0 so it happens instantly.
            // We check !rod.isNibbling to assume it's in the waiting phase.
            // also check !rod.hit and !rod.pullingOutOfWater etc to be safe, 
            // though simply setting timeUntilFishingBite to 0 when it > 0 is usually enough.
            
            if (rod.isFishing && !rod.isNibbling && !rod.hit && !rod.pullingOutOfWater && rod.timeUntilFishingBite > 0)
            {
                rod.timeUntilFishingBite = 0;
            }
        }
    }
}
