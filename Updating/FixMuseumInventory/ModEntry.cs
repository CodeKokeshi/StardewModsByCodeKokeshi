using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;

namespace FixMuseumInventory;

/// <summary>
/// Museum inventory improvements:
/// 1. Movable inventory (drag to reposition)
/// 2. Compact mode (only shows donatable items in a smaller grid)
/// </summary>
public class ModEntry : Mod
{
    private const int MoveButtonComponentId = 19512001;
    private const int StationaryTicksBeforeShowingMoveTooltip = 6;

    // Button bounds
    private Rectangle _moveButtonBounds;

    private ClickableComponent? _moveButtonComponent;

    // Move mode state
    private bool _isMoving;
    private Point _mouseOffset;

    // Compact inventory state (always active in donation mode)
    private List<int> _donatableSlotIndices = new();
    private List<Rectangle> _compactSlotBounds = new();
    private int _compactColumns = 6;
    private const int ExtraSlotIndex = 999; // Virtual slot for holding items

    // Compact panel dimensions (calculated each frame)
    private Rectangle _compactPanelBounds;

    // Tooltip timing
    private Point? _lastMenuPosition;
    private int _ticksSinceMenuMoved = int.MaxValue;

    // Saved position for session
    private Point? _savedMenuOffset;

    // Original menu dimensions
    private int _originalMenuWidth;
    private int _originalMenuHeight;
    private bool _dimensionsSaved;

    // Original OK button position (to restore when exiting compact mode)
    private Point _originalOkButtonPosition;
    
    // Compact mode independent positioning
    private Point _compactPanelOffset = Point.Zero;
    private Point? _compactPanelBasePosition;

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
        _moveButtonComponent = null;
        _lastMenuPosition = null;
        _ticksSinceMenuMoved = int.MaxValue;
        _dimensionsSaved = false;
        _donatableSlotIndices.Clear();
        _compactSlotBounds.Clear();
        _originalOkButtonPosition = Point.Zero;
        _compactPanelOffset = Point.Zero;
        _compactPanelBasePosition = null;

        if (e.NewMenu is MuseumMenu menu)
        {
            // Initial update of donatable items
            UpdateDonatableItems(menu);
            
            if (_savedMenuOffset.HasValue)
            {
                UpdateAllBounds(menu);

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

        bool isFading = menu.fadeTimer > 0 || menu.blackFadeAlpha > 0f;
        if (isFading)
        {
            _isMoving = false;
            return;
        }

        // Save original dimensions once
        if (!_dimensionsSaved)
        {
            _originalMenuWidth = menu.width;
            _originalMenuHeight = menu.height;
            _dimensionsSaved = true;
        }

        Point positionBefore = new(menu.xPositionOnScreen, menu.yPositionOnScreen);

        // Update donatable items periodically
        if (e.IsMultipleOf(30))
        {
            UpdateDonatableItems(menu);
        }

        // Update ALL bounds every frame (fixes lag issue)
        UpdateAllBounds(menu);
        TryAddButtonsToSnappyMenu(menu);

        // Handle move mode - always moves compact panel
        if (_isMoving && !menu.menuMovingDown)
        {
            if (Game1.options.gamepadControls)
            {
                MoveCompactPanelWithController();
            }
            else
            {
                int mouseX = Game1.getMouseX();
                int mouseY = Game1.getMouseY();

                int targetX = mouseX - _mouseOffset.X;
                int targetY = mouseY - _mouseOffset.Y;

                Point clamped = ClampCompactPanelPosition(new Point(targetX, targetY));
                _compactPanelOffset = new Point(
                    clamped.X - (_compactPanelBasePosition?.X ?? 0),
                    clamped.Y - (_compactPanelBasePosition?.Y ?? 0)
                );
            }
        }

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
    /// Finds all inventory slots that contain donatable items.
    /// </summary>
    private void UpdateDonatableItems(MuseumMenu menu)
    {
        _donatableSlotIndices.Clear();

        var museum = menu.Museum;
        if (museum == null) return;

        var inventory = Game1.player?.Items;
        if (inventory == null) return;

        for (int i = 0; i < inventory.Count && i < 36; i++)
        {
            Item? item = inventory[i];
            if (item != null && museum.isItemSuitableForDonation(item))
            {
                _donatableSlotIndices.Add(i);
            }
        }
        
        // Always add one extra slot at the end for holding/returning items
        _donatableSlotIndices.Add(ExtraSlotIndex);
    }

    /// <summary>
    /// Updates all bounds (compact slots, panel, buttons) based on current menu position.
    /// Called every frame to keep everything in sync.
    /// </summary>
    private void UpdateAllBounds(MuseumMenu menu)
    {
        // Calculate compact panel dimensions
        int padding = 16;
        int slotSize = 64;
        int slotPadding = 4;

        int itemCount = _donatableSlotIndices.Count;
        // Always have at least 1 item (the extra slot)
        _compactColumns = itemCount switch
        {
            1 => 1, // Just the extra slot
            <= 3 => itemCount,
            <= 6 => 3,
            <= 12 => 4,
            <= 20 => 5,
            _ => 6
        };

        int rows = itemCount > 0 ? (int)Math.Ceiling((double)itemCount / _compactColumns) : 1;

        int panelWidth = _compactColumns * (slotSize + slotPadding) + padding * 2;
        int panelHeight = rows * (slotSize + slotPadding) + padding * 2;

        // Save base position on first calculation
        if (!_compactPanelBasePosition.HasValue)
        {
            _compactPanelBasePosition = new Point(
                menu.inventory.xPositionOnScreen,
                menu.inventory.yPositionOnScreen
            );
        }
        
        // Compact panel position = base + offset (independent positioning)
        int invX = _compactPanelBasePosition.Value.X + _compactPanelOffset.X;
        int invY = _compactPanelBasePosition.Value.Y + _compactPanelOffset.Y;
        
        int panelX = invX - padding;
        int panelY = invY - padding;

        _compactPanelBounds = new Rectangle(panelX, panelY, panelWidth, panelHeight);

        // Update compact slot bounds
        _compactSlotBounds.Clear();
        int startX = invX;
        int startY = invY;

        for (int i = 0; i < itemCount; i++)
        {
            int col = i % _compactColumns;
            int row = i / _compactColumns;

            Rectangle bounds = new(
                startX + col * (slotSize + slotPadding),
                startY + row * (slotSize + slotPadding),
                slotSize,
                slotSize
            );

            _compactSlotBounds.Add(bounds);
        }

        // Update button positions based on mode
        UpdateButtonBounds(menu);
    }

    private void UpdateButtonBounds(MuseumMenu menu)
    {
        if (menu.okButton == null)
            return;

        // Store original OK button position before moving it
        if (_originalOkButtonPosition == Point.Zero)
        {
            _originalOkButtonPosition = new Point(menu.okButton.bounds.X, menu.okButton.bounds.Y);
        }

        // Always position buttons below the compact panel
        int buttonStartY = _compactPanelBounds.Bottom + 8;
        int buttonX = _compactPanelBounds.X;

        // Move button at left
        _moveButtonBounds = new Rectangle(buttonX, buttonStartY, 64, 64);

        // OK button next to move button
        menu.okButton.bounds.X = buttonX + 72;
        menu.okButton.bounds.Y = buttonStartY;
    }

    private void TryAddButtonsToSnappyMenu(MuseumMenu menu)
    {
        if (!Game1.options.SnappyMenus)
            return;

        bool holdingItem = menu.heldItem != null || Game1.player?.CursorSlotItem != null;
        bool shouldShowButtons = !holdingItem;

        menu.allClickableComponents ??= new();
        if (menu.allClickableComponents.Count == 0)
            menu.populateClickableComponentList();

        // === MOVE BUTTON ===
        if (_moveButtonComponent == null)
        {
            _moveButtonComponent = new ClickableComponent(_moveButtonBounds, "Move UI")
            {
                myID = MoveButtonComponentId,
                upNeighborID = ClickableComponent.SNAP_AUTOMATIC,
                downNeighborID = menu.okButton?.myID ?? 106,
                leftNeighborID = ClickableComponent.SNAP_AUTOMATIC,
                rightNeighborID = ClickableComponent.SNAP_AUTOMATIC,
                region = menu.okButton?.region ?? 0
            };
        }
        else
        {
            _moveButtonComponent.bounds = _moveButtonBounds;
            _moveButtonComponent.visible = shouldShowButtons;
        }

        AddOrUpdateComponent(menu, _moveButtonComponent, MoveButtonComponentId);

        if (menu.okButton != null)
            menu.okButton.upNeighborID = MoveButtonComponentId;
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

        int speed = state.IsButtonDown(Buttons.LeftShoulder) || state.IsButtonDown(Buttons.RightShoulder)
            ? 12
            : 6;

        int dx = 0;
        int dy = 0;

        Vector2 stick = state.ThumbSticks.Left;
        if (Math.Abs(stick.X) > 0.2f)
            dx += (int)Math.Round(stick.X * speed);
        if (Math.Abs(stick.Y) > 0.2f)
            dy += (int)Math.Round(-stick.Y * speed);

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
        Rectangle union = new(menu.xPositionOnScreen, menu.yPositionOnScreen, menu.width, menu.height);

        if (menu.okButton != null)
            union = Rectangle.Union(union, menu.okButton.bounds);

        if (menu.allClickableComponents != null)
        {
            foreach (var c in menu.allClickableComponents)
            {
                if (c != null && c.myID == MoveButtonComponentId)
                {
                    union = Rectangle.Union(union, c.bounds);
                }
            }
        }

        int leftOffset = union.Left - menu.xPositionOnScreen;
        int rightOffset = union.Right - menu.xPositionOnScreen;
        int topOffset = union.Top - menu.yPositionOnScreen;
        int bottomOffset = union.Bottom - menu.yPositionOnScreen;

        int minX = -leftOffset;
        int maxX = Game1.uiViewport.Width - rightOffset;
        int minY = -topOffset;
        int maxY = Game1.uiViewport.Height - bottomOffset;

        int x = Math.Clamp(desiredTopLeft.X, minX, maxX);
        int y = Math.Clamp(desiredTopLeft.Y, minY, maxY);
        return new Point(x, y);
    }

    private Point ClampCompactPanelPosition(Point desiredTopLeft)
    {
        // Calculate the union of compact panel and its button
        Rectangle union = _compactPanelBounds;
        union = Rectangle.Union(union, _moveButtonBounds);

        // Ensure entire compact UI stays on screen
        int minX = -union.Left + desiredTopLeft.X;
        int maxX = Game1.uiViewport.Width - union.Right + desiredTopLeft.X;
        int minY = -union.Top + desiredTopLeft.Y;
        int maxY = Game1.uiViewport.Height - union.Bottom + desiredTopLeft.Y;

        int x = Math.Clamp(desiredTopLeft.X, minX, maxX);
        int y = Math.Clamp(desiredTopLeft.Y, minY, maxY);
        return new Point(x, y);
    }

    private void MoveCompactPanelWithController()
    {
        GamePadState state = GamePad.GetState(PlayerIndex.One);

        int speed = state.IsButtonDown(Buttons.LeftShoulder) || state.IsButtonDown(Buttons.RightShoulder)
            ? 12
            : 6;

        int dx = 0;
        int dy = 0;

        Vector2 stick = state.ThumbSticks.Left;
        if (Math.Abs(stick.X) > 0.2f)
            dx += (int)Math.Round(stick.X * speed);
        if (Math.Abs(stick.Y) > 0.2f)
            dy += (int)Math.Round(-stick.Y * speed);

        if (state.IsButtonDown(Buttons.DPadLeft)) dx -= speed;
        if (state.IsButtonDown(Buttons.DPadRight)) dx += speed;
        if (state.IsButtonDown(Buttons.DPadUp)) dy -= speed;
        if (state.IsButtonDown(Buttons.DPadDown)) dy += speed;

        if (dx == 0 && dy == 0)
            return;

        Point currentPos = new Point(
            _compactPanelBounds.X + dx,
            _compactPanelBounds.Y + dy
        );
        
        Point clamped = ClampCompactPanelPosition(currentPos);
        _compactPanelOffset = new Point(
            clamped.X - (_compactPanelBasePosition?.X ?? 0) + 16,
            clamped.Y - (_compactPanelBasePosition?.Y ?? 0) + 16
        );
    }

    private void OnRenderedActiveMenu(object? sender, RenderedActiveMenuEventArgs e)
    {
        if (Game1.activeClickableMenu is not MuseumMenu menu)
            return;

        if (menu.fadeTimer > 0 || menu.blackFadeAlpha > 0f)
            return;

        SpriteBatch b = e.SpriteBatch;
        int mx = Game1.getMouseX();
        int my = Game1.getMouseY();

        bool holdingItem = menu.heldItem != null || Game1.player?.CursorSlotItem != null;
        bool inArrangeMode = menu.menuMovingDown;

        // === HIDE VANILLA INVENTORY ALWAYS ===
        // Cover the entire vanilla inventory area
        Rectangle inventoryArea = new(
            menu.inventory.xPositionOnScreen - 32,
            menu.inventory.yPositionOnScreen - 32,
            menu.inventory.width + 64,
            menu.inventory.height + 64
        );
        b.Draw(Game1.fadeToBlackRect, inventoryArea, Color.Black * 1.0f);

        // === DRAW COMPACT INVENTORY (unless in arrange mode) ===
        if (!inArrangeMode)
        {
            DrawCompactInventoryOverlay(b, menu, mx, my);
        }

        // === DRAW BUTTONS (only in donation mode) ===
        if (!holdingItem && !inArrangeMode)
        {
            DrawMoveButton(b, mx, my);
            
            // Redraw OK button since we repositioned it
            if (menu.okButton != null)
            {
                menu.okButton.draw(b);
            }
        }

        menu.drawMouse(b);
    }

    private void DrawCompactInventoryOverlay(SpriteBatch b, MuseumMenu menu, int mx, int my)
    {
        // Draw compact panel background
        IClickableMenu.drawTextureBox(
            b,
            Game1.menuTexture,
            new Rectangle(0, 256, 60, 60),
            _compactPanelBounds.X,
            _compactPanelBounds.Y,
            _compactPanelBounds.Width,
            _compactPanelBounds.Height,
            Color.White,
            1f,
            drawShadow: true
        );

        if (_donatableSlotIndices.Count == 0)
            return;

        // Draw each donatable item
        Item? hoveredItem = null;
        for (int i = 0; i < _donatableSlotIndices.Count && i < _compactSlotBounds.Count; i++)
        {
            int slotIndex = _donatableSlotIndices[i];
            Rectangle bounds = _compactSlotBounds[i];

            var playerItems = Game1.player?.Items;
            Item? item = playerItems != null && slotIndex < playerItems.Count ? playerItems[slotIndex] : null;
            if (item == null) continue;

            bool isHovering = bounds.Contains(mx, my);

            // Draw slot background
            Color slotColor = isHovering ? Color.Yellow : Color.White;
            b.Draw(
                Game1.menuTexture,
                bounds,
                new Rectangle(128, 128, 64, 64),
                slotColor
            );

            // Draw item
            item.drawInMenu(b, new Vector2(bounds.X, bounds.Y), 1f);

            // Draw stack count if > 1
            if (item.Stack > 1)
            {
                string stackText = item.Stack.ToString();
                Vector2 stackPos = new(bounds.Right - Game1.tinyFont.MeasureString(stackText).X - 4, bounds.Bottom - 24);
                b.DrawString(Game1.tinyFont, stackText, stackPos + new Vector2(1, 1), Color.Black * 0.5f);
                b.DrawString(Game1.tinyFont, stackText, stackPos, Color.White);
            }

            if (isHovering)
                hoveredItem = item;
        }

        // Draw tooltip last (on top of everything)
        if (hoveredItem != null)
        {
            IClickableMenu.drawToolTip(b, hoveredItem.getDescription(), hoveredItem.DisplayName, hoveredItem);
        }
    }

    private void DrawMoveButton(SpriteBatch b, int mx, int my)
    {
        bool isHovering = _moveButtonBounds.Contains(mx, my);

        Color bgColor = _isMoving
            ? new Color(180, 255, 180)
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

        Rectangle iconSource = new(162, 440, 16, 16);
        float iconScale = isHovering ? 4.25f : 4f;
        Color iconColor = _isMoving ? Color.DarkGreen : Color.White;
        Vector2 iconPos = new(
            center.X - iconSource.Width * iconScale / 2f,
            center.Y - iconSource.Height * iconScale / 2f
        );

        Utility.drawWithShadow(b, Game1.mouseCursors, iconPos, iconSource, iconColor, 0f, Vector2.Zero, iconScale);

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

        if (menu.fadeTimer > 0 || menu.blackFadeAlpha > 0f)
            return;

        if (menu.heldItem != null || Game1.player?.CursorSlotItem != null)
            return;

        int mouseX = Game1.getMouseX();
        int mouseY = Game1.getMouseY();

        // Cancel move mode
        if (_isMoving && isCancel)
        {
            _isMoving = false;
            Game1.playSound("cancel");
            Helper.Input.Suppress(e.Button);
            return;
        }

        if (!isConfirm)
            return;

        // === COMPACT INVENTORY ITEM CLICK ===
        if (isConfirm && !menu.menuMovingDown)
        {
            for (int i = 0; i < _compactSlotBounds.Count && i < _donatableSlotIndices.Count; i++)
            {
                if (_compactSlotBounds[i].Contains(mouseX, mouseY))
                {
                    int slotIndex = _donatableSlotIndices[i];
                    var playerItems = Game1.player?.Items;
                    
                    // Handle extra slot for holding/returning items
                    if (slotIndex == ExtraSlotIndex)
                    {
                        if (menu.heldItem != null)
                        {
                            // Return held item to player inventory
                            Game1.player?.addItemToInventoryBool(menu.heldItem);
                            menu.heldItem = null;
                            Game1.playSound("coin");
                        }
                        else if (Game1.player?.CursorSlotItem != null)
                        {
                            // Drop cursor item
                            Game1.player.addItemToInventoryBool(Game1.player.CursorSlotItem);
                            Game1.player.CursorSlotItem = null;
                            Game1.playSound("coin");
                        }
                        Helper.Input.Suppress(e.Button);
                        return;
                    }
                    
                    // Handle regular item slots
                    if (playerItems != null && slotIndex < playerItems.Count)
                    {
                        Item? item = playerItems[slotIndex];
                        if (item != null)
                        {
                            // Pick up the item (same as clicking in normal inventory)
                            menu.heldItem = item.getOne();
                            Item? remaining = item.ConsumeStack(1);
                            playerItems[slotIndex] = remaining;
                            Game1.playSound("dwop");

                            Helper.Input.Suppress(e.Button);
                            return;
                        }
                    }
                }
            }
        }

        // === MOVE BUTTON ===
        if (_isMoving)
        {
            _isMoving = false;
            _savedMenuOffset = new Point(menu.xPositionOnScreen, menu.yPositionOnScreen);
            Game1.playSound("stoneStep");
            Helper.Input.Suppress(e.Button);
        }
        else if (_moveButtonBounds.Contains(mouseX, mouseY))
        {
            _isMoving = true;
            _ticksSinceMenuMoved = 0;
            
            // Always moving compact panel
            _mouseOffset = new Point(
                mouseX - _compactPanelBounds.X,
                mouseY - _compactPanelBounds.Y
            );
            
            Game1.playSound("bigSelect");
            Helper.Input.Suppress(e.Button);
        }
    }
}
