using System;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace NoStamina;

public class ModEntry : Mod
{
    public override void Entry(IModHelper helper)
    {
        helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
    }

    private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
    {
        if (Context.IsWorldReady && Game1.player != null)
        {
            // basically every stamina value is max value
            Game1.player.Stamina = Game1.player.MaxStamina;
        }
    }
}
