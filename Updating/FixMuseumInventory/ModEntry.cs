using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace FixMuseumInventory;

/// <summary>
/// Complete overhaul of the museum inventory UI:
/// 1. Removes the annoying sliding animation when opening/interacting
/// 2. Adds a Hide/Show button to toggle inventory visibility
/// 3. Retains the Move button to reposition the inventory
/// 4. Full controller support for all buttons
/// 5. Better overall UX for donating artifacts
/// </summary>
public class ModEntry : Mod
{
    // Component IDs for snappy menu navigation
    private const int MoveButtonComponentId = 19512001;
    private const int HideButtonComponentId = 19512002;
    private const int StationaryTicksBeforeShowingMoveTooltip = 6;

    // Button bounds - Hide button stays visible, Move button hides with inventory
    private Rectangle _moveButtonBounds;
    private Rectangle _hideButtonBounds;

    // Clickable components for controller navigation
    private ClickableComponent? _moveButtonComponent;
    private ClickableComponent? _hideButtonComponent;

    // Move mode state
    private bool _isMoving;
    private Point _mouseOffset;

    // Hide state - inventory is hidden but Hide button remains visible
    private bool _isInventoryHidden;

    // Track menu position changes for tooltip timing
    private Point? _lastMenuPosition;
    private int _ticksSinceMenuMoved = int.MaxValue;

    // Saved position persists across menu opens within the same game session
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
        // Reset all state when menu changes
        _isMoving = false;
        _isInventoryHidden = false;
        _moveButtonComponent = null;
        _hideButtonComponent = null;
        _lastMenuPosition = null;
        _ticksSinceMenuMoved = int.MaxValue;

        if (e.NewMenu is MuseumMenu menu)
        {
            // === REMOVE THE FADE-IN ANIMATION ===
            // Skip directly to the "placing" state with no fade
            menu.fadeTimer = 0;
            menu.fadeIntoBlack = false;
            menu.blackFadeAlpha = 0f;

            // Set state to 1 (placingInMuseumState) - this is what happens after fade completes
            var stateField = typeof(MuseumMenu).GetField("state", 
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            stateField?.SetValue(menu, 1);

            // Set up the viewport as the game would after the fade
            Game1.viewportFreeze = true;
            Game1.viewport.Location = new xTile.Dimensions.Location(1152, 128);
            Game1.clampViewportToGameMap();

            // Pre-calculate button bounds
            UpdateButtonBounds(menu);

            // Apply saved position if available
            if (_savedMenuOffset.HasValue)
            {
                Point clamped = ClampMenuPosition(menu, _savedMenuOffset.Value);
                int dx = clamped.X - menu.xPositionOnScreen;
                int dy = clamped.Y - menu.yPositionOnScreen;

                if (dx != 0 || dy != 0)
                    menu.movePosition(dx, dy);
            }
        }
    }

    private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
    {
        if (Game1.activeClickableMenu is not MuseumMenu menu)
        {
            _isMoving = false;
            _lastMenuPosition = null;
            _ticksSinceMenuMoved = int.MaxValue;
            return;
        }

        // Allow interaction during normal exit fade (state 2/3) but not during it
        bool isExiting = menu.fadeTimer > 0 && menu.fadeIntoBlack;
        if (isExiting)
        {
            _isMoving = false;
            return;
        }

        // === PREVENT SLIDING ANIMATION ===
        // The vanilla game slides the menu down when menuMovingDown is true
        // We forcefully counteract this every tick
        PreventSlidingAnimation(menu);

        Point positionBefore = new(menu.xPositionOnScreen, menu.yPositionOnScreen);

        // Update button positions
        UpdateButtonBounds(menu);

        // Register buttons for controller navigation
        TryAddButtonsToSnappyMenu(menu);

        // Handle move mode - menu follows input
        if (_isMoving)
        {
            if (Game1.options.gamepadControls)
            {
                MoveMenuWithController(menu);
            }
            else
            {
                int mouseX = Game1.getMouseX();
                int mouseY = Game1.getMouseY();

                int targetX = mouseX - _mouseOffset.X;
                int targetY = mouseY - _mouseOffset.Y;

                Point clamped = ClampMenuPosition(menu, new Point(targetX, targetY));
                int dx = clamped.X - menu.xPositionOnScreen;
                int dy = clamped.Y - menu.yPositionOnScreen;

                if (dx != 0 || dy != 0)
                    menu.movePosition(dx, dy);
            }
        }

        // Track position changes for tooltip timing
        Point positionAfter = new(menu.xPositionOnScreen, menu.yPositionOnScreen);
        if (_lastMenuPosition == null)
        {
            _lastMenuPosition = positionAfter;
            _ticksSinceMenuMoved = int.MaxValue;
        }
        else if (positionAfter != positionBefore)
        {
            _lastMenuPosition = positionAfter;
            _ticksSinceMenuMoved = 0;
        }
        else
        {
            if (_ticksSinceMenuMoved < int.MaxValue)
                _ticksSinceMenuMoved++;
        }
    }

    /// <summary>
    /// Prevents the vanilla sliding animation by resetting menuPositionOffset
    /// and undoing any movement the game applied.
    /// </summary>
    private void PreventSlidingAnimation(MuseumMenu menu)
    {
        // The game's update() does: if (menuMovingDown && menuPositionOffset < height/3) 
        // { menuPositionOffset += 8; movePosition(0, 8); }
        // We counteract by moving back and resetting the offset

        if (menu.menuPositionOffset != 0)
        {
            int undoY = -menu.menuPositionOffset;
            menu.menuPositionOffset = 0;

            // Only undo if we're not actively moving (avoid fighting user input)
            if (!_isMoving && undoY != 0)
            {
                menu.movePosition(0, undoY);
            }
        }

        // Also prevent the flag from triggering more movement
        // We need reflection since menuMovingDown is public but we want to be careful
        menu.menuMovingDown = false;
    }

    /// <summary>
    /// Updates button bounds based on current menu position.
    /// Hide button is positioned to the left of Move button.
    /// Both are above the OK button.
    /// </summary>
    private void UpdateButtonBounds(MuseumMenu menu)
    {
        if (menu.okButton == null)
            return;

        // Move button: directly above OK button
        _moveButtonBounds = new Rectangle(
            menu.okButton.bounds.X,
            menu.okButton.bounds.Y - 80,
            64,
            64
        );

        // Hide button: to the left of Move button
        _hideButtonBounds = new Rectangle(
            _moveButtonBounds.X - 72,
            _moveButtonBounds.Y,
            64,
            64
        );
    }

    /// <summary>
    /// Registers both buttons as clickable components for controller/snappy navigation.
    /// </summary>
    private void TryAddButtonsToSnappyMenu(MuseumMenu menu)
    {
        if (!Game1.options.SnappyMenus)
            return;

        bool holdingItem = menu.heldItem != null || Game1.player?.CursorSlotItem != null;

        // Move button only visible when inventory is visible and not holding item
        bool showMoveButton = !_isInventoryHidden && !holdingItem;

        // Hide button is ALWAYS visible (except when holding item) - that's the whole point!
        bool showHideButton = !holdingItem;

        menu.allClickableComponents ??= new();
        if (menu.allClickableComponents.Count == 0)
            menu.populateClickableComponentList();

        // === HIDE BUTTON ===
        if (_hideButtonComponent == null)
        {
            _hideButtonComponent = new ClickableComponent(_hideButtonBounds, "Hide/Show Inventory")
            {
                myID = HideButtonComponentId,
                rightNeighborID = showMoveButton ? MoveButtonComponentId : (menu.okButton?.myID ?? 106),
                downNeighborID = menu.okButton?.myID ?? 106,
                leftNeighborID = ClickableComponent.SNAP_AUTOMATIC,
                upNeighborID = ClickableComponent.SNAP_AUTOMATIC,
                region = menu.okButton?.region ?? 0
            };
        }
        else
        {
            _hideButtonComponent.bounds = _hideButtonBounds;
            _hideButtonComponent.visible = showHideButton;
            _hideButtonComponent.rightNeighborID = showMoveButton ? MoveButtonComponentId : (menu.okButton?.myID ?? 106);
        }

        // === MOVE BUTTON ===
        if (_moveButtonComponent == null)
        {
            _moveButtonComponent = new ClickableComponent(_moveButtonBounds, "Move Inventory")
            {
                myID = MoveButtonComponentId,
                leftNeighborID = HideButtonComponentId,
                downNeighborID = menu.okButton?.myID ?? 106,
                rightNeighborID = ClickableComponent.SNAP_AUTOMATIC,
                upNeighborID = ClickableComponent.SNAP_AUTOMATIC,
                region = menu.okButton?.region ?? 0
            };
        }
        else
        {
            _moveButtonComponent.bounds = _moveButtonBounds;
            _moveButtonComponent.visible = showMoveButton;
        }

        // Add or update in component list
        AddOrUpdateComponent(menu, _hideButtonComponent, HideButtonComponentId);
        AddOrUpdateComponent(menu, _moveButtonComponent, MoveButtonComponentId);

        // Wire up navigation from OK button to our buttons
        if (menu.okButton != null)
        {
            if (showMoveButton)
                menu.okButton.upNeighborID = MoveButtonComponentId;
            else if (showHideButton)
                menu.okButton.upNeighborID = HideButtonComponentId;
        }
    }

    private static void AddOrUpdateComponent(MuseumMenu menu, ClickableComponent component, int componentId)
    {
        for (int i = 0; i < menu.allClickableComponents!.Count; i++)
        {
            if (menu.allClickableComponents[i]?.myID == componentId)
            {
                menu.allClickableComponents[i] = component;
                return;
            }
        }
        menu.allClickableComponents.Add(component);
    }

    private static void MoveMenuWithController(MuseumMenu menu)
    {
        GamePadState state = GamePad.GetState(PlayerIndex.One);

        // Hold shoulder button for faster movement
        int speed = state.IsButtonDown(Buttons.LeftShoulder) || state.IsButtonDown(Buttons.RightShoulder)
            ? 12
            : 6;

        int dx = 0;
        int dy = 0;

        // Left stick
        Vector2 stick = state.ThumbSticks.Left;
        if (Math.Abs(stick.X) > 0.2f)
            dx += (int)Math.Round(stick.X * speed);
        if (Math.Abs(stick.Y) > 0.2f)
            dy += (int)Math.Round(-stick.Y * speed);

        // D-pad
        if (state.IsButtonDown(Buttons.DPadLeft)) dx -= speed;
        if (state.IsButtonDown(Buttons.DPadRight)) dx += speed;
        if (state.IsButtonDown(Buttons.DPadUp)) dy -= speed;
        if (state.IsButtonDown(Buttons.DPadDown)) dy += speed;

        if (dx == 0 && dy == 0)
            return;

        Point clamped = ClampMenuPosition(menu, new Point(menu.xPositionOnScreen + dx, menu.yPositionOnScreen + dy));
        int actualDx = clamped.X - menu.xPositionOnScreen;
        int actualDy = clamped.Y - menu.yPositionOnScreen;

        if (actualDx != 0 || actualDy != 0)
            menu.movePosition(actualDx, actualDy);
    }

    private static Point ClampMenuPosition(MuseumMenu menu, Point desiredTopLeft)
    {
        // Build a union rectangle of all elements that must stay on screen
        Rectangle union = new(menu.xPositionOnScreen, menu.yPositionOnScreen, menu.width, menu.height);

        if (menu.okButton != null)
            union = Rectangle.Union(union, menu.okButton.bounds);

        // Find our custom buttons in the component list
        if (menu.allClickableComponents != null)
        {
            foreach (var c in menu.allClickableComponents)
            {
                if (c != null && (c.myID == MoveButtonComponentId || c.myID == HideButtonComponentId))
                {
                    union = Rectangle.Union(union, c.bounds);
                }
            }
        }

        // Calculate offsets from menu position to union edges
        int leftOffset = union.Left - menu.xPositionOnScreen;
        int rightOffset = union.Right - menu.xPositionOnScreen;
        int topOffset = union.Top - menu.yPositionOnScreen;
        int bottomOffset = union.Bottom - menu.yPositionOnScreen;

        // Clamp so union stays within viewport
        int minX = -leftOffset;
        int maxX = Game1.uiViewport.Width - rightOffset;
        int minY = -topOffset;
        int maxY = Game1.uiViewport.Height - bottomOffset;

        int x = Math.Clamp(desiredTopLeft.X, minX, maxX);
        int y = Math.Clamp(desiredTopLeft.Y, minY, maxY);
        return new Point(x, y);
    }

    private void OnRenderedActiveMenu(object? sender, RenderedActiveMenuEventArgs e)
    {
        if (Game1.activeClickableMenu is not MuseumMenu menu)
            return;

        // Don't render during exit fade
        if (menu.fadeTimer > 0 && menu.fadeIntoBlack)
            return;

        SpriteBatch b = e.SpriteBatch;
        int mx = Game1.getMouseX();
        int my = Game1.getMouseY();

        bool holdingItem = menu.heldItem != null || Game1.player?.CursorSlotItem != null;

        // === DRAW HIDE/SHOW BUTTON (Always visible when not holding item) ===
        if (!holdingItem)
        {
            DrawHideButton(b, mx, my);
        }

        // === DRAW MOVE BUTTON (Only when inventory visible and not holding item) ===
        if (!_isInventoryHidden && !holdingItem)
        {
            DrawMoveButton(b, mx, my);
        }

        // === COVER INVENTORY IF HIDDEN ===
        // Draw a semi-transparent overlay over the inventory area when hidden
        if (_isInventoryHidden && !holdingItem)
        {
            // The inventory area bounds
            Rectangle inventoryBounds = new(
                menu.xPositionOnScreen,
                menu.yPositionOnScreen,
                menu.width,
                menu.height
            );

            // Draw dark overlay
            b.Draw(
                Game1.fadeToBlackRect,
                inventoryBounds,
                Color.Black * 0.75f
            );

            // Draw "Inventory Hidden" text in center
            string hiddenText = "Inventory Hidden";
            Vector2 textSize = Game1.smallFont.MeasureString(hiddenText);
            Vector2 textPos = new(
                inventoryBounds.X + (inventoryBounds.Width - textSize.X) / 2,
                inventoryBounds.Y + (inventoryBounds.Height - textSize.Y) / 2
            );
            b.DrawString(Game1.smallFont, hiddenText, textPos + new Vector2(2, 2), Color.Black * 0.5f);
            b.DrawString(Game1.smallFont, hiddenText, textPos, Color.White);
        }

        // Always redraw cursor on top
        menu.drawMouse(b);
    }

    private void DrawHideButton(SpriteBatch b, int mx, int my)
    {
        bool isHovering = _hideButtonBounds.Contains(mx, my);

        // Background color indicates state
        Color bgColor = _isInventoryHidden
            ? new Color(255, 180, 180) // Reddish when hidden
            : (isHovering ? new Color(255, 255, 220) : Color.White);

        IClickableMenu.drawTextureBox(
            b,
            Game1.menuTexture,
            new Rectangle(0, 256, 60, 60),
            _hideButtonBounds.X,
            _hideButtonBounds.Y,
            _hideButtonBounds.Width,
            _hideButtonBounds.Height,
            bgColor,
            1f,
            drawShadow: false
        );

        // Draw icon - eye-like symbol from cursors
        Vector2 center = new(
            _hideButtonBounds.X + _hideButtonBounds.Width / 2f,
            _hideButtonBounds.Y + _hideButtonBounds.Height / 2f
        );

        // Different icons for shown/hidden state
        // Using magnifying glass for "visible" and X for "hidden"
        Rectangle iconSource = _isInventoryHidden
            ? new Rectangle(322, 498, 12, 12)  // Small circle/dot
            : new Rectangle(175, 425, 12, 12); // Eye-like icon

        float iconScale = isHovering ? 4f : 3.5f;
        Color iconColor = _isInventoryHidden ? new Color(180, 60, 60) : Color.White;
        Vector2 iconPos = new(
            center.X - iconSource.Width * iconScale / 2f,
            center.Y - iconSource.Height * iconScale / 2f
        );

        Utility.drawWithShadow(b, Game1.mouseCursors, iconPos, iconSource, iconColor, 0f, Vector2.Zero, iconScale);

        // Tooltip
        if (isHovering)
        {
            string tooltip = _isInventoryHidden ? "Show Inventory (click to show)" : "Hide Inventory";
            IClickableMenu.drawHoverText(b, tooltip, Game1.smallFont);
        }
    }

    private void DrawMoveButton(SpriteBatch b, int mx, int my)
    {
        bool isHovering = _moveButtonBounds.Contains(mx, my);

        Color bgColor = _isMoving
            ? new Color(180, 255, 180) // Green when moving
            : (isHovering ? new Color(255, 255, 220) : Color.White);

        IClickableMenu.drawTextureBox(
            b,
            Game1.menuTexture,
            new Rectangle(0, 256, 60, 60),
            _moveButtonBounds.X,
            _moveButtonBounds.Y,
            _moveButtonBounds.Width,
            _moveButtonBounds.Height,
            bgColor,
            1f,
            drawShadow: false
        );

        Vector2 center = new(
            _moveButtonBounds.X + _moveButtonBounds.Width / 2f,
            _moveButtonBounds.Y + _moveButtonBounds.Height / 2f
        );

        // Four-way arrow icon
        Rectangle iconSource = new(162, 440, 16, 16);
        float iconScale = isHovering ? 4.25f : 4f;
        Color iconColor = _isMoving ? Color.DarkGreen : Color.White;
        Vector2 iconPos = new(
            center.X - iconSource.Width * iconScale / 2f,
            center.Y - iconSource.Height * iconScale / 2f
        );

        Utility.drawWithShadow(b, Game1.mouseCursors, iconPos, iconSource, iconColor, 0f, Vector2.Zero, iconScale);

        // Tooltip
        if (isHovering && !_isMoving)
        {
            IClickableMenu.drawHoverText(b, "Move Inventory", Game1.smallFont);
        }
        else if (_isMoving && _ticksSinceMenuMoved >= StationaryTicksBeforeShowingMoveTooltip)
        {
            IClickableMenu.drawHoverText(b, "Click anywhere (or press A) to place", Game1.smallFont);
        }
    }

    private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
    {
        bool isConfirm = e.Button == SButton.MouseLeft || e.Button == SButton.ControllerA;
        bool isCancel = e.Button == SButton.ControllerB;

        if (!isConfirm && !isCancel)
            return;

        if (Game1.activeClickableMenu is not MuseumMenu menu)
            return;

        // Don't allow during exit fade
        if (menu.fadeTimer > 0 && menu.fadeIntoBlack)
            return;

        // Don't allow while holding an item
        if (menu.heldItem != null || Game1.player?.CursorSlotItem != null)
            return;

        int mouseX = Game1.getMouseX();
        int mouseY = Game1.getMouseY();

        // Handle cancel while moving
        if (_isMoving && isCancel)
        {
            _isMoving = false;
            Game1.playSound("cancel");
            Helper.Input.Suppress(e.Button);
            return;
        }

        if (!isConfirm)
            return;

        // === HIDE BUTTON (always clickable when visible) ===
        if (_hideButtonBounds.Contains(mouseX, mouseY))
        {
            _isInventoryHidden = !_isInventoryHidden;
            Game1.playSound(_isInventoryHidden ? "breathout" : "breathin");

            // Cancel move mode if hiding
            if (_isInventoryHidden)
                _isMoving = false;

            Helper.Input.Suppress(e.Button);
            return;
        }

        // === MOVE BUTTON (only when inventory visible) ===
        if (!_isInventoryHidden)
        {
            if (_isMoving)
            {
                // Currently moving → click anywhere to place
                _isMoving = false;
                _savedMenuOffset = new Point(menu.xPositionOnScreen, menu.yPositionOnScreen);
                Game1.playSound("stoneStep");
                Helper.Input.Suppress(e.Button);
            }
            else if (_moveButtonBounds.Contains(mouseX, mouseY))
            {
                // Click on move button → start moving
                _isMoving = true;
                _ticksSinceMenuMoved = 0;
                _mouseOffset = new Point(mouseX - menu.xPositionOnScreen, mouseY - menu.yPositionOnScreen);
                Game1.playSound("bigSelect");
                Helper.Input.Suppress(e.Button);
            }
        }
    }
}
