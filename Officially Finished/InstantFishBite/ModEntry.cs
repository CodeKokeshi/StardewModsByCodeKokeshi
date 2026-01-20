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

        // Optimization: If player isn't using a tool, they can't be fishing.
        // This avoids unnecessary casting and logic when just walking around.
        if (!Game1.player.UsingTool)
            return;

        // Check if player is holding a fishing rod
        if (Game1.player.CurrentTool is FishingRod rod)
        {
            // If the rod is in the water (isFishing) but not yet nibbling/biting
            // We want to force the bite timer to 0 so it happens instantly.
            if (rod.isFishing && !rod.isNibbling && !rod.hit && !rod.pullingOutOfWater && rod.timeUntilFishingBite > 0)
            {
                rod.timeUntilFishingBite = 0;
            }
        }
    }
}
