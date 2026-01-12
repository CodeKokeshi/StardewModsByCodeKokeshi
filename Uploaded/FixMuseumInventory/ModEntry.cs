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

        int mx = Game1.getMouseX();
        int my = Game1.getMouseY();
        bool isHoveringMoveButton = _moveButtonBounds.Contains(mx, my);

        SpriteBatch b = e.SpriteBatch;

        // Draw button background - highlight if moving
        Color bgColor = _isMoving
            ? new Color(180, 255, 180)
            : (isHoveringMoveButton ? new Color(255, 255, 220) : Color.White);
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

        // Draw a single, clear icon (vanilla hand cursor) to indicate "drag/move"
        Vector2 center = new Vector2(
            _moveButtonBounds.X + _moveButtonBounds.Width / 2f,
            _moveButtonBounds.Y + _moveButtonBounds.Height / 2f
        );

        Rectangle handSource = Game1.getSourceRectForStandardTileSheet(
            Game1.mouseCursors,
            StardewValley.Object.HandCursorIndex,
            16,
            16
        );

        float iconScale = isHoveringMoveButton ? 3.25f : 3f; // 16 * 3 = 48px, fits nicely in a 64px button
        Color iconColor = _isMoving ? Color.DarkGreen : Color.White;
        Vector2 iconPos = new Vector2(
            center.X - handSource.Width * iconScale / 2f,
            center.Y - handSource.Height * iconScale / 2f
        );
        Utility.drawWithShadow(b, Game1.mouseCursors, iconPos, handSource, iconColor, 0f, Vector2.Zero, iconScale);

        // Tooltip
        if (isHoveringMoveButton && !_isMoving)
        {
            IClickableMenu.drawHoverText(b, "Move this UI (drag)", Game1.smallFont);
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
