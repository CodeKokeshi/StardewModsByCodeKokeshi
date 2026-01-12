using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace FixMuseumInventory;

public class ModEntry : Mod
{
    private ClickableTextureComponent? _toggleButton;
    private bool _isInventoryOnTop = false;
    private bool _shouldRender = false;

    // Dragging state (drag the button to reposition the menu)
    private bool _isDragging;
    private Vector2 _dragStartMouse;
    private Point _dragStartMenuPos;
    private bool _dragMoved;

    public override void Entry(IModHelper helper)
    {
        helper.Events.Display.MenuChanged += OnMenuChanged;
        // Draw after the menu so our button isn't covered by it.
        helper.Events.Display.RenderedActiveMenu += OnRenderedActiveMenu;
        helper.Events.Input.ButtonPressed += OnButtonPressed;
        helper.Events.Input.ButtonReleased += OnButtonReleased;
        helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
    }

    private void OnMenuChanged(object? sender, MenuChangedEventArgs e)
    {
        _toggleButton = null;
        _isInventoryOnTop = false;
        _shouldRender = false;

        _isDragging = false;
        _dragMoved = false;

        if (e.NewMenu is MuseumMenu)
        {
            _toggleButton = new ClickableTextureComponent(
                new Rectangle(0, 0, 64, 64),
                Game1.mouseCursors,
                Rectangle.Empty,
                1f
            );
        }
    }

    private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
    {
        if (!(Game1.activeClickableMenu is MuseumMenu menu) || _toggleButton == null)
        {
            _shouldRender = false;
            return;
        }

        // Keep this simple so the button doesn't "disappear" due to over-strict checks.
        // If the museum menu is open and not fading, show it (unless the cursor is holding an item).
        ClickableTextureComponent? okButton = menu.okButton;
        Item? heldItem = menu.heldItem;
        bool playerHoldingItem = Game1.player?.CursorSlotItem != null || Game1.player?.itemToEat != null;
        bool cursorHasItem = heldItem != null || playerHoldingItem;
        bool isFading = menu.fadeTimer > 0 || menu.blackFadeAlpha > 0f;

        _shouldRender = okButton != null && okButton.visible && !cursorHasItem && !isFading;

        if (_shouldRender)
        {
            // Position the button near the OK button.
            // (If the menu is moved, okButton bounds already reflect the new position.)
            _toggleButton.bounds.X = okButton!.bounds.X;
            _toggleButton.bounds.Y = okButton.bounds.Y - 80;
        }

        // Handle dragging to reposition the menu.
        if (_isDragging)
        {
            Vector2 mouse = new Vector2(Game1.getMouseX(), Game1.getMouseY());
            Vector2 delta = mouse - _dragStartMouse;
            if (!_dragMoved && delta.LengthSquared() > 16f)
                _dragMoved = true;

            int targetX = _dragStartMenuPos.X + (int)delta.X;
            int targetY = _dragStartMenuPos.Y + (int)delta.Y;

            // Clamp to keep the menu on screen.
            targetX = Math.Clamp(targetX, 0, Game1.viewport.Width - menu.width);
            targetY = Math.Clamp(targetY, 0, Game1.viewport.Height - menu.height);

            int dx = targetX - menu.xPositionOnScreen;
            int dy = targetY - menu.yPositionOnScreen;
            if (dx != 0 || dy != 0)
            {
                // Use the menu's own movePosition so internal state stays consistent.
                menu.movePosition(dx, dy);
            }
        }
    }

    private void OnRenderedActiveMenu(object? sender, RenderedActiveMenuEventArgs e)
    {
        if (!_shouldRender || _toggleButton == null) return;

        Rectangle arrowSource = !_isInventoryOnTop
            ? new Rectangle(421, 459, 11, 12) // UP
            : new Rectangle(421, 472, 11, 12); // DOWN

        // Draw button background
        e.SpriteBatch.Draw(
            Game1.mouseCursors,
            _toggleButton.bounds,
            new Rectangle(128, 256, 64, 64),
            Color.White
        );

        // Draw arrow
        float scale = 4f;
        Vector2 arrowPos = new Vector2(
            _toggleButton.bounds.X + (_toggleButton.bounds.Width - arrowSource.Width * scale) / 2,
            _toggleButton.bounds.Y + (_toggleButton.bounds.Height - arrowSource.Height * scale) / 2
        );

        e.SpriteBatch.Draw(
            Game1.mouseCursors,
            arrowPos,
            arrowSource,
            Color.White,
            0f,
            Vector2.Zero,
            scale,
            SpriteEffects.None,
            0.9f
        );
    }

    private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
    {
        if (e.Button != SButton.MouseLeft || !_shouldRender || _toggleButton == null) return;
        if (!(Game1.activeClickableMenu is MuseumMenu menu)) return;

        if (_toggleButton.containsPoint((int)e.Cursor.ScreenPixels.X, (int)e.Cursor.ScreenPixels.Y))
        {
            // Start drag (release will toggle if it wasn't actually dragged).
            _isDragging = true;
            _dragMoved = false;
            _dragStartMouse = new Vector2(Game1.getMouseX(), Game1.getMouseY());
            _dragStartMenuPos = new Point(menu.xPositionOnScreen, menu.yPositionOnScreen);
            Helper.Input.Suppress(e.Button);
        }
    }

    private void OnButtonReleased(object? sender, ButtonReleasedEventArgs e)
    {
        if (e.Button != SButton.MouseLeft)
            return;

        if (!_isDragging)
            return;

        _isDragging = false;

        // If it was a click (not a drag), keep the old up/down toggle behavior.
        if (!_dragMoved && Game1.activeClickableMenu is MuseumMenu menu)
        {
            ToggleInventoryPosition(menu);
        }

        _dragMoved = false;
    }

    private void ToggleInventoryPosition(MuseumMenu menu)
    {
        _isInventoryOnTop = !_isInventoryOnTop;

        int targetY = _isInventoryOnTop ? 0 : Game1.viewport.Height - menu.height;
        targetY = Math.Clamp(targetY, 0, Game1.viewport.Height - menu.height);

        int dy = targetY - menu.yPositionOnScreen;
        if (dy != 0)
            menu.movePosition(0, dy);

        Game1.playSound("drumkit6");
    }
}
