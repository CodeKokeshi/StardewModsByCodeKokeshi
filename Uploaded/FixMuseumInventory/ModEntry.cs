using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace FixMuseumInventory;

public class ModEntry : Mod
{
    // Move button bounds
    private Rectangle _moveButtonBounds;
    
    // Toggle move mode: click once to start moving, click again to place
    private bool _isMoving;
    private Point _mouseOffset; // Offset from mouse to menu top-left
    
    // Saved position for the session
    private Point? _savedMenuOffset;

    public override void Entry(IModHelper helper)
    {
        helper.Events.Display.MenuChanged += OnMenuChanged;
        helper.Events.Display.RenderedActiveMenu += OnRenderedActiveMenu;
        helper.Events.Input.ButtonPressed += OnButtonPressed;
        helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
    }

    private void OnMenuChanged(object? sender, MenuChangedEventArgs e)
    {
        _isMoving = false;

        // When museum menu opens, apply saved offset if we have one
        if (e.NewMenu is MuseumMenu menu && _savedMenuOffset.HasValue)
        {
            int targetX = Math.Clamp(_savedMenuOffset.Value.X, 0, Game1.uiViewport.Width - menu.width);
            int targetY = Math.Clamp(_savedMenuOffset.Value.Y, 0, Game1.uiViewport.Height - menu.height);

            int dx = targetX - menu.xPositionOnScreen;
            int dy = targetY - menu.yPositionOnScreen;
            
            if (dx != 0 || dy != 0)
                menu.movePosition(dx, dy);
        }
    }

    private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
    {
        if (Game1.activeClickableMenu is not MuseumMenu menu)
        {
            _isMoving = false;
            return;
        }

        // Don't allow interaction during fades
        bool isFading = menu.fadeTimer > 0 || menu.blackFadeAlpha > 0f;
        if (isFading)
        {
            _isMoving = false;
            return;
        }

        // Position the move button relative to okButton
        if (menu.okButton != null)
        {
            _moveButtonBounds = new Rectangle(
                menu.okButton.bounds.X,
                menu.okButton.bounds.Y - 80,
                64,
                64
            );
        }

        // If in move mode, menu follows the mouse
        if (_isMoving)
        {
            int mouseX = Game1.getMouseX();
            int mouseY = Game1.getMouseY();

            // Calculate target menu position (mouse pos minus offset)
            int targetX = mouseX - _mouseOffset.X;
            int targetY = mouseY - _mouseOffset.Y;

            // Clamp to keep menu fully on screen
            targetX = Math.Clamp(targetX, 0, Game1.uiViewport.Width - menu.width);
            targetY = Math.Clamp(targetY, 0, Game1.uiViewport.Height - menu.height);

            int dx = targetX - menu.xPositionOnScreen;
            int dy = targetY - menu.yPositionOnScreen;

            if (dx != 0 || dy != 0)
                menu.movePosition(dx, dy);
        }
    }

    private void OnRenderedActiveMenu(object? sender, RenderedActiveMenuEventArgs e)
    {
        if (Game1.activeClickableMenu is not MuseumMenu menu)
            return;

        // Don't render during fades
        if (menu.fadeTimer > 0 || menu.blackFadeAlpha > 0f)
            return;

        // Don't show button while holding an item
        if (menu.heldItem != null || Game1.player?.CursorSlotItem != null)
            return;

        SpriteBatch b = e.SpriteBatch;

        // Draw button background - highlight if moving
        Color bgColor = _isMoving ? Color.LightGreen : Color.White;
        IClickableMenu.drawTextureBox(
            b,
            Game1.mouseCursors,
            new Rectangle(432, 439, 9, 9),
            _moveButtonBounds.X,
            _moveButtonBounds.Y,
            _moveButtonBounds.Width,
            _moveButtonBounds.Height,
            bgColor,
            4f,
            drawShadow: false
        );

        // Draw 4-direction arrow icon
        Rectangle arrowUp = new Rectangle(421, 459, 11, 12);
        Rectangle arrowDown = new Rectangle(421, 472, 11, 12);
        
        Vector2 center = new Vector2(
            _moveButtonBounds.X + _moveButtonBounds.Width / 2f,
            _moveButtonBounds.Y + _moveButtonBounds.Height / 2f
        );

        float scale = 2f;
        Color iconColor = _isMoving ? Color.DarkGreen : Color.White;
        
        // Up
        b.Draw(Game1.mouseCursors, center + new Vector2(-11f, -18f), arrowUp, iconColor, 0f, Vector2.Zero, scale, SpriteEffects.None, 1f);
        // Down
        b.Draw(Game1.mouseCursors, center + new Vector2(-11f, 6f), arrowDown, iconColor, 0f, Vector2.Zero, scale, SpriteEffects.None, 1f);
        // Left
        b.Draw(Game1.mouseCursors, center + new Vector2(-20f, -12f), arrowUp, iconColor, -MathHelper.PiOver2, new Vector2(5.5f, 6f), scale, SpriteEffects.None, 1f);
        // Right
        b.Draw(Game1.mouseCursors, center + new Vector2(20f, -12f), arrowDown, iconColor, MathHelper.PiOver2, new Vector2(5.5f, 6f), scale, SpriteEffects.None, 1f);

        // Tooltip
        int mx = Game1.getMouseX();
        int my = Game1.getMouseY();
        if (_moveButtonBounds.Contains(mx, my) && !_isMoving)
        {
            IClickableMenu.drawHoverText(b, "Click to move inventory", Game1.smallFont);
        }
        else if (_isMoving)
        {
            IClickableMenu.drawHoverText(b, "Click anywhere to place", Game1.smallFont);
        }

        // Always redraw cursor on top
        menu.drawMouse(b);
    }

    private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
    {
        if (e.Button != SButton.MouseLeft)
            return;

        if (Game1.activeClickableMenu is not MuseumMenu menu)
            return;

        // Don't allow during fades
        if (menu.fadeTimer > 0 || menu.blackFadeAlpha > 0f)
            return;

        // Don't allow while holding an item
        if (menu.heldItem != null || Game1.player?.CursorSlotItem != null)
            return;

        int mouseX = Game1.getMouseX();
        int mouseY = Game1.getMouseY();

        if (_isMoving)
        {
            // Currently moving → click anywhere to place it
            _isMoving = false;
            _savedMenuOffset = new Point(menu.xPositionOnScreen, menu.yPositionOnScreen);
            Game1.playSound("stoneStep");
            Helper.Input.Suppress(e.Button);
        }
        else if (_moveButtonBounds.Width > 0 && _moveButtonBounds.Contains(mouseX, mouseY))
        {
            // Click on button → start moving
            _isMoving = true;
            // Store offset: where on the menu the mouse currently is
            _mouseOffset = new Point(
                mouseX - menu.xPositionOnScreen,
                mouseY - menu.yPositionOnScreen
            );
            Game1.playSound("bigSelect");
            Helper.Input.Suppress(e.Button);
        }
    }
}
