using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace FixMuseumInventory;

public class ModEntry : Mod
{
    // Drag handle button
    private Rectangle _dragHandleBounds;
    private bool _isDragging;
    private Point _dragOffset;
    
    // Saved position for the session
    private Point? _savedMenuOffset;

    public override void Entry(IModHelper helper)
    {
        helper.Events.Display.MenuChanged += OnMenuChanged;
        helper.Events.Display.RenderedActiveMenu += OnRenderedActiveMenu;
        helper.Events.Input.ButtonPressed += OnButtonPressed;
        helper.Events.Input.ButtonReleased += OnButtonReleased;
        helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
    }

    private void OnMenuChanged(object? sender, MenuChangedEventArgs e)
    {
        _isDragging = false;

        // When museum menu opens, apply saved offset if we have one
        if (e.NewMenu is MuseumMenu menu && _savedMenuOffset.HasValue)
        {
            // Calculate safe position
            int targetX = _savedMenuOffset.Value.X;
            int targetY = _savedMenuOffset.Value.Y;
            
            targetX = Math.Clamp(targetX, 0, Game1.uiViewport.Width - menu.width);
            targetY = Math.Clamp(targetY, 0, Game1.uiViewport.Height - menu.height);

            int dx = targetX - menu.xPositionOnScreen;
            int dy = targetY - menu.yPositionOnScreen;
            
            if (dx != 0 || dy != 0)
            {
                menu.movePosition(dx, dy);
            }
        }
    }

    private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
    {
        if (Game1.activeClickableMenu is not MuseumMenu menu)
            return;

        // Don't allow interaction during fades
        bool isFading = menu.fadeTimer > 0 || menu.blackFadeAlpha > 0f;
        if (isFading)
        {
            _isDragging = false;
            return;
        }

        // Position the drag handle near the top-right of the inventory area
        // Using okButton as reference since it's always positioned relative to the menu
        if (menu.okButton != null)
        {
            _dragHandleBounds = new Rectangle(
                menu.okButton.bounds.X,
                menu.okButton.bounds.Y - 80,
                64,
                64
            );
        }

        // Handle active dragging
        if (_isDragging)
        {
            int mouseX = Game1.getMouseX();
            int mouseY = Game1.getMouseY();

            // Calculate where the menu should be based on mouse + offset
            int targetX = mouseX - _dragOffset.X;
            int targetY = mouseY - _dragOffset.Y;

            // Clamp to keep the entire menu on screen
            targetX = Math.Clamp(targetX, 0, Game1.uiViewport.Width - menu.width);
            targetY = Math.Clamp(targetY, 0, Game1.uiViewport.Height - menu.height);

            int dx = targetX - menu.xPositionOnScreen;
            int dy = targetY - menu.yPositionOnScreen;

            if (dx != 0 || dy != 0)
            {
                menu.movePosition(dx, dy);
            }
        }
    }

    private void OnRenderedActiveMenu(object? sender, RenderedActiveMenuEventArgs e)
    {
        if (Game1.activeClickableMenu is not MuseumMenu menu)
            return;

        // Don't render during fades
        bool isFading = menu.fadeTimer > 0 || menu.blackFadeAlpha > 0f;
        if (isFading)
            return;

        // Don't show handle while holding an item (to not interfere with donations)
        Item? heldItem = menu.heldItem;
        bool playerHoldingItem = Game1.player?.CursorSlotItem != null;
        if (heldItem != null || playerHoldingItem)
            return;

        SpriteBatch b = e.SpriteBatch;

        // Draw button background (wooden button style)
        IClickableMenu.drawTextureBox(
            b,
            Game1.mouseCursors,
            new Rectangle(432, 439, 9, 9),
            _dragHandleBounds.X,
            _dragHandleBounds.Y,
            _dragHandleBounds.Width,
            _dragHandleBounds.Height,
            _isDragging ? Color.Gray : Color.White,
            4f,
            drawShadow: false
        );

        // Draw a move/drag icon (using the hand cursor icon area, or we can use arrows)
        // Let's draw 4 small arrows pointing outward to indicate "move"
        Rectangle arrowUp = new Rectangle(421, 459, 11, 12);
        Rectangle arrowDown = new Rectangle(421, 472, 11, 12);
        
        Vector2 center = new Vector2(
            _dragHandleBounds.X + _dragHandleBounds.Width / 2f,
            _dragHandleBounds.Y + _dragHandleBounds.Height / 2f
        );

        float iconScale = 2f;
        
        // Up arrow
        b.Draw(
            Game1.mouseCursors,
            center + new Vector2(-5.5f * iconScale, -16f),
            arrowUp,
            Color.White,
            0f,
            Vector2.Zero,
            iconScale,
            SpriteEffects.None,
            1f
        );
        
        // Down arrow
        b.Draw(
            Game1.mouseCursors,
            center + new Vector2(-5.5f * iconScale, 4f),
            arrowDown,
            Color.White,
            0f,
            Vector2.Zero,
            iconScale,
            SpriteEffects.None,
            1f
        );

        // Left arrow (rotated up arrow)
        b.Draw(
            Game1.mouseCursors,
            center + new Vector2(-16f, -6f * iconScale),
            arrowUp,
            Color.White,
            -MathHelper.PiOver2,
            new Vector2(5.5f, 6f),
            iconScale,
            SpriteEffects.None,
            1f
        );

        // Right arrow (rotated down arrow)  
        b.Draw(
            Game1.mouseCursors,
            center + new Vector2(16f, -6f * iconScale),
            arrowDown,
            Color.White,
            MathHelper.PiOver2,
            new Vector2(5.5f, 6f),
            iconScale,
            SpriteEffects.None,
            1f
        );

        // Draw hover tooltip
        int mouseX = Game1.getMouseX();
        int mouseY = Game1.getMouseY();
        if (_dragHandleBounds.Contains(mouseX, mouseY))
        {
            IClickableMenu.drawHoverText(b, "Drag to move inventory", Game1.smallFont);
        }

        // Redraw mouse cursor on top so it's never hidden
        menu.drawMouse(b);
    }

    private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
    {
        if (e.Button != SButton.MouseLeft)
            return;

        if (Game1.activeClickableMenu is not MuseumMenu menu)
            return;

        // Don't allow dragging during fades
        bool isFading = menu.fadeTimer > 0 || menu.blackFadeAlpha > 0f;
        if (isFading)
            return;

        // Don't allow dragging while holding an item
        Item? heldItem = menu.heldItem;
        bool playerHoldingItem = Game1.player?.CursorSlotItem != null;
        if (heldItem != null || playerHoldingItem)
            return;

        int mouseX = (int)e.Cursor.ScreenPixels.X;
        int mouseY = (int)e.Cursor.ScreenPixels.Y;

        if (_dragHandleBounds.Contains(mouseX, mouseY))
        {
            _isDragging = true;
            // Store offset from mouse to menu top-left corner
            _dragOffset = new Point(
                mouseX - menu.xPositionOnScreen,
                mouseY - menu.yPositionOnScreen
            );
            
            Game1.playSound("bigSelect");
            Helper.Input.Suppress(e.Button);
        }
    }

    private void OnButtonReleased(object? sender, ButtonReleasedEventArgs e)
    {
        if (e.Button != SButton.MouseLeft)
            return;

        if (_isDragging)
        {
            _isDragging = false;
            Game1.playSound("bigDeSelect");

            // Save the current position for next time the menu opens
            if (Game1.activeClickableMenu is MuseumMenu menu)
            {
                _savedMenuOffset = new Point(menu.xPositionOnScreen, menu.yPositionOnScreen);
            }
        }
    }
}
