using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace WorkingPets.UI;

/// <summary>
/// Draws a whistle button on the Animals page for calling pets.
/// </summary>
public static class WhistleButton
{
    private static Rectangle ButtonBounds;
    private static bool IsHovering;
    
    /// <summary>
    /// Draw the whistle button on the Animals page.
    /// </summary>
    public static void Draw(SpriteBatch b, IClickableMenu menu)
    {
        // Position at bottom-right of the animals page
        int x = menu.xPositionOnScreen + menu.width - 80;
        int y = menu.yPositionOnScreen + menu.height - 80;
        
        ButtonBounds = new Rectangle(x, y, 64, 64);
        
        // Check if hovering
        int mouseX = Game1.getMouseX();
        int mouseY = Game1.getMouseY();
        IsHovering = ButtonBounds.Contains(mouseX, mouseY);
        
        // Draw button background
        IClickableMenu.drawTextureBox(
            b,
            Game1.mouseCursors,
            new Rectangle(432, 439, 9, 9),
            x, y, 64, 64,
            IsHovering ? Color.Wheat : Color.White,
            4f,
            false
        );
        
        // Draw whistle icon (use the dog/bone icon from game cursors)
        float iconScale = IsHovering ? 3.2f : 3f;
        Vector2 iconPos = new Vector2(
            x + 32 - (10 * iconScale / 2),
            y + 32 - (10 * iconScale / 2)
        );
        
        b.Draw(
            Game1.mouseCursors,
            iconPos,
            new Rectangle(96, 1936, 16, 16), // whistle/horn icon
            Color.White,
            0f,
            Vector2.Zero,
            iconScale / 2f,
            SpriteEffects.None,
            0.9f
        );
        
        // Draw tooltip if hovering
        if (IsHovering)
        {
            string tooltip = "Click pets to whistle them\n(switches to follow mode)";
            IClickableMenu.drawHoverText(b, tooltip, Game1.smallFont);
        }
    }
    
    /// <summary>
    /// Check if the button was clicked.
    /// </summary>
    public static bool IsClicked(int x, int y)
    {
        return ButtonBounds.Contains(x, y);
    }
}
