using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Characters;
using System.Collections.Generic;

namespace WorkingPets.UI;

/// <summary>
/// Draws pet icons on the map/HUD to show their locations.
/// </summary>
public class PetMapTracker
{
    /// <summary>
    /// Draw pet icons on the HUD showing their locations.
    /// Called during RenderingHud event.
    /// </summary>
    public static void DrawPetIcons(SpriteBatch spriteBatch)
    {
        if (!Context.IsWorldReady || Game1.eventUp || Game1.isFestival())
            return;

        var player = Game1.player;
        var currentLocation = player.currentLocation;
        
        // Get all pets in the game
        List<Pet> allPets = new List<Pet>();
        Utility.ForEachLocation(location =>
        {
            foreach (var character in location.characters)
            {
                if (character is Pet pet)
                {
                    allPets.Add(pet);
                }
            }
            return true;
        });

        // Draw icons for pets in the current location
        foreach (var pet in allPets)
        {
            if (pet.currentLocation != currentLocation)
                continue;

            // Get pet position in world
            Vector2 petWorldPos = pet.Position + new Vector2(32, 32); // center of pet
            
            // Only draw off-screen indicator (no floating icons above pets - looks buggy)
            if (!Utility.isOnScreen(petWorldPos, 64))
            {
                // Draw off-screen indicator (arrow at edge of screen pointing to pet)
                DrawOffScreenIndicator(spriteBatch, petWorldPos, pet);
            }
        }
    }

    /// <summary>
    /// Draw an arrow at the edge of the screen pointing toward off-screen pets, with pet icon.
    /// </summary>
    private static void DrawOffScreenIndicator(SpriteBatch spriteBatch, Vector2 petWorldPos, Pet pet)
    {
        var viewport = Game1.viewport;
        var screenBounds = Game1.graphics.GraphicsDevice.Viewport.Bounds;

        Vector2 onScreenPosition = Vector2.Zero;
        float rotation = 0f;

        // Calculate where arrow should appear
        if (petWorldPos.X > viewport.MaxCorner.X - 64)
        {
            onScreenPosition.X = screenBounds.Right - 16;
            rotation = MathHelper.PiOver2;
        }
        else if (petWorldPos.X < viewport.X)
        {
            onScreenPosition.X = 16;
            rotation = -MathHelper.PiOver2;
        }
        else
        {
            onScreenPosition.X = petWorldPos.X - viewport.X;
        }

        if (petWorldPos.Y > viewport.MaxCorner.Y - 64)
        {
            onScreenPosition.Y = screenBounds.Bottom - 16;
            rotation = MathHelper.Pi;
        }
        else if (petWorldPos.Y < viewport.Y)
        {
            onScreenPosition.Y = 16;
            rotation = 0f;
        }
        else
        {
            onScreenPosition.Y = petWorldPos.Y - viewport.Y;
        }

        // Adjust rotation for corners
        if (onScreenPosition.X == screenBounds.Right - 16 && onScreenPosition.Y == screenBounds.Bottom - 16)
        {
            rotation = MathHelper.PiOver2 + MathHelper.PiOver4;
        }
        else if (onScreenPosition.X == 16 && onScreenPosition.Y == 16)
        {
            rotation = -MathHelper.PiOver2 - MathHelper.PiOver4;
        }
        else if (onScreenPosition.X == screenBounds.Right - 16 && onScreenPosition.Y == 16)
        {
            rotation = MathHelper.PiOver4;
        }
        else if (onScreenPosition.X == 16 && onScreenPosition.Y == screenBounds.Bottom - 16)
        {
            rotation = MathHelper.Pi - MathHelper.PiOver4;
        }

        // Get pet icon
        pet.GetPetIcon(out string iconTexture, out Rectangle iconSourceRect);
        Texture2D petIconTexture = Game1.content.Load<Texture2D>(iconTexture);

        // Get color based on work status
        Color arrowColor = Color.White;
        var manager = ModEntry.PetManager?.GetManagerForPet(pet);
        if (manager != null && manager.IsWorking)
        {
            arrowColor = Color.LightGreen;
        }

        // Draw pet icon first (behind arrow)
        spriteBatch.Draw(
            petIconTexture,
            onScreenPosition - new Vector2(iconSourceRect.Width, iconSourceRect.Height),
            iconSourceRect,
            Color.White,
            0f,
            Vector2.Zero,
            2f, // icon scale
            SpriteEffects.None,
            0.94f
        );

        // Draw arrow pointing to pet
        spriteBatch.Draw(
            Game1.mouseCursors,
            onScreenPosition,
            new Rectangle(412, 495, 5, 4), // arrow sprite
            arrowColor,
            rotation,
            new Vector2(2f, 2f),
            4f,
            SpriteEffects.None,
            0.95f
        );
    }
}
