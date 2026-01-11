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

    public override void Entry(IModHelper helper)
    {
        helper.Events.Display.MenuChanged += OnMenuChanged;
        helper.Events.Display.RenderingActiveMenu += OnRenderingActiveMenu; // Changed from RenderedActiveMenu
        helper.Events.Input.ButtonPressed += OnButtonPressed;
        helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
    }

    private void OnMenuChanged(object? sender, MenuChangedEventArgs e)
    {
        _toggleButton = null;
        _isInventoryOnTop = false;
        _shouldRender = false;

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

        // --- Optimized Checks (Direct Access) ---
        // 1. okButton is public in MenuWithInventory
        ClickableTextureComponent? okButton = menu.okButton;

        // 2. heldItem is public in MenuWithInventory
        Item? heldItem = menu.heldItem;
        
        // Also check if player is holding something in their actual cursor (outside of menu logic)
        bool playerHoldingItem = Game1.player?.CursorSlotItem != null || Game1.player?.itemToEat != null;
        bool cursorHasItem = heldItem != null || playerHoldingItem;

        // 3. Fade/Transition Check (public fields in MuseumMenu)
        // Note: blackFadeAlpha replaces old reflection check for "fadeToBlackAlpha"
        bool isFading = menu.fadeTimer > 0 || menu.blackFadeAlpha > 0f;

        // --- Inventory Visibility Check ---
        bool inventoryReady = false;
        bool inventoryVisible = true;
        if (menu.inventory != null)
        {
            // Position Checks
            bool verticalVisible = menu.inventory.yPositionOnScreen < Game1.viewport.Height - 32;
            inventoryReady = menu.inventory.xPositionOnScreen > 0 && 
                           menu.inventory.yPositionOnScreen > 0 &&
                           menu.inventory.xPositionOnScreen < Game1.viewport.Width &&
                           verticalVisible;
            
            // Content Checks
            if (menu.inventory.actualInventory != null && menu.inventory.actualInventory.Count == 0) inventoryVisible = false;
            if (menu.inventory.width <= 0 || menu.inventory.height <= 0) inventoryVisible = false;
            
            // "visible" field check - direct access inside InventoryMenu would be better usually public
            // Assuming inventory.visible is a mismatch or we keep safe. 
            // InventoryMenu usually doesn't have a 'visible' field, usually it's just implicit.
            // But let's check if we can remove this reflection too.
            // Actually, InventoryMenu does not have a 'visible' field in standard 1.6 logic usually.
            // It relies on being drawn or not.
            // So we can probably skip this reflection entirely as 'verticalVisible' covers offscreen.
        }
        else
        {
            inventoryVisible = false;
        }

        // --- OK Button Screen Checks ---
        // 1. Must exist and be 'visible' flag wise
        // 2. Must be physically on screen (yPosition check) - often UI slides offscreen
        bool okButtonOnScreen = false;
        if (okButton != null)
        {
            okButtonOnScreen = okButton.visible && 
                               okButton.bounds.Y < Game1.viewport.Height && 
                               okButton.bounds.Y > 0 &&
                               okButton.bounds.X > 0 && 
                               okButton.bounds.X < Game1.viewport.Width;
        }

        // --- Final Decision ---
        // readyToClose is a method usually, not a field.
        bool menuReady = menu.readyToClose();

        _shouldRender = okButtonOnScreen && 
                       !cursorHasItem && 
                       inventoryReady &&
                       inventoryVisible &&
                       !isFading &&
                       menuReady;

        if (_shouldRender && okButton != null)
        {
            _toggleButton.bounds.X = okButton.bounds.X;
            _toggleButton.bounds.Y = okButton.bounds.Y - 80;
        }
    }

    private void OnRenderingActiveMenu(object? sender, RenderingActiveMenuEventArgs e)
    {
        if (!_shouldRender || _toggleButton == null) return;

        Rectangle arrowSource = !_isInventoryOnTop
            ? new Rectangle(421, 459, 11, 12) // UP
            : new Rectangle(421, 472, 11, 12); // DOWN

        // Draw button background - using simple draw without sprite batch parameters
        Game1.spriteBatch.Draw(
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

        Game1.spriteBatch.Draw(
            Game1.mouseCursors,
            arrowPos,
            arrowSource,
            Color.White,
            0f,
            Vector2.Zero,
            scale,
            SpriteEffects.None,
            1f  // Maximum layer depth - draws first, cursor draws last
        );
    }

    private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
    {
        if (e.Button != SButton.MouseLeft || !_shouldRender || _toggleButton == null) return;
        if (!(Game1.activeClickableMenu is MuseumMenu menu)) return;

        if (_toggleButton.containsPoint((int)e.Cursor.ScreenPixels.X, (int)e.Cursor.ScreenPixels.Y))
        {
            ToggleInventoryPosition(menu);
            Helper.Input.Suppress(e.Button);
        }
    }

    private void ToggleInventoryPosition(MuseumMenu menu)
    {
        int viewportHeight = Game1.viewport.Height;
        _isInventoryOnTop = !_isInventoryOnTop;

        int currentY = menu.yPositionOnScreen;
        int targetY = _isInventoryOnTop ? 0 : viewportHeight - menu.height;

        if (targetY > viewportHeight - 100) targetY = viewportHeight - 300;

        int dy = targetY - currentY;

        menu.yPositionOnScreen += dy;

        if (menu.inventory != null)
        {
            menu.inventory.movePosition(0, dy);
        }

        if (menu.okButton != null)
        {
            menu.okButton.bounds.Y += dy;
        }

        Game1.playSound("drumkit6");
    }
}
