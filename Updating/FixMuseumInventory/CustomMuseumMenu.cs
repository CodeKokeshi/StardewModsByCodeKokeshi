using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Locations;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using xTile.Dimensions;

using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace FixMuseumInventory;

/// <summary>
/// Custom museum menu built from scratch - NO inheritance from MenuWithInventory.
/// Two distinct modes:
/// 1. Donation Mode: Shows compact inventory of only donatable items
/// 2. Arrange Mode: NO inventory at all, just pick up and move existing pieces
/// </summary>
public class CustomMuseumMenu : IClickableMenu
{
    // === Core References ===
    public readonly LibraryMuseum Museum;
    public readonly bool IsArrangeMode;

    // === State ===
    public Item? HeldItem;
    private bool _holdingMuseumPiece; // True when holding a piece FROM the museum
    private int _fadeTimer;
    private bool _fadeIntoBlack;
    private int _state; // 0=starting, 1=active, 2=exiting fade, 3=done
    
    // === UI Components ===
    private ClickableTextureComponent? _okButton;
    private string _hoverText = "";
    
    // === Visual Effects ===
    public SparklingText? SparkleText;
    public Vector2 GlobalLocationOfSparklingArtifact;
    
    // === Donation Mode: Compact Inventory ===
    private List<Item> _donatableItems = new();
    private List<ClickableComponent> _inventorySlots = new();
    private Rectangle _inventoryBounds;
    private const int InventoryColumns = 6;
    private const int InventoryHeaderHeight = 44;
    private const int SlotSize = 64;
    private const int SlotPadding = 8;
    private const int PanelExtraPadding = 12;
    private Point _inventoryOffset = Point.Zero;
    
    // === Arrange Mode: No Inventory ===
    // (Nothing needed - that's the point!)
    
    // === Draggable Inventory ===
    private bool _followModeEnabled; // Toggle mode where panel follows cursor
    private Point _dragMouseOffset;
    private ClickableComponent? _dragHandle;

    // === Controller navigation ===
    private readonly List<ClickableComponent> _museumSlotComponents = new();
    private readonly Dictionary<ClickableComponent, Vector2> _museumComponentToTile = new();
    private ClickableComponent? _controllerSelected;
    private int _navRepeatTimerMs;
    private bool _navWasNeutral = true;
    private GamePadState _prevPadState;
    private bool _suppressNextLeftClick;
    private const int NavInitialDelayMs = 200;
    private const int NavRepeatDelayMs = 90;
    private const float NavStickDeadzone = 0.55f;
    private const float FollowStickDeadzone = 0.2f;
    private const float FollowPanelSpeedPxPerSec = 900f;
    
    public CustomMuseumMenu(LibraryMuseum museum, bool arrangeMode = false)
        : base(0, 0, Game1.uiViewport.Width, Game1.uiViewport.Height, showUpperRightCloseButton: false)
    {
        Museum = museum ?? throw new ArgumentNullException(nameof(museum));
        IsArrangeMode = arrangeMode;
        
        // Setup fade transition
        _fadeTimer = 800;
        _fadeIntoBlack = true;
        _state = 0;
        
        // Make player movable during menu
        Game1.player.forceCanMove();
        
        // Create OK button
        _okButton = new ClickableTextureComponent(
            bounds: new Rectangle(
                xPositionOnScreen + width - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder - 64,
                yPositionOnScreen + height - IClickableMenu.borderWidth - IClickableMenu.spaceToClearTopBorder + 16,
                64,
                64
            ),
            texture: Game1.mouseCursors,
            sourceRect: Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46),
            scale: 1f
        );
        
        // Initialize mode-specific UI
        if (!IsArrangeMode)
        {
            InitializeDonationInventory();
        }
        
        Game1.displayHUD = false;
    }
    
    /// <summary>
    /// Creates compact inventory showing ONLY donatable items
    /// </summary>
    private void InitializeDonationInventory()
    {
        _donatableItems.Clear();
        _inventorySlots.Clear();
        
        // Collect all donatable items from player inventory
        foreach (var item in Game1.player.Items)
        {
            if (item != null && Museum.isItemSuitableForDonation(item))
            {
                _donatableItems.Add(item);
            }
        }
        
        // Calculate inventory panel size (header + grid fit + extra padding)
        int rows = (int)Math.Ceiling(_donatableItems.Count / (double)InventoryColumns);
        int horizontalGaps = Math.Max(0, InventoryColumns - 1);
        int verticalGaps = Math.Max(0, rows - 1);

        int innerPad = SlotPadding + PanelExtraPadding;

        int panelWidth = (innerPad * 2) + (InventoryColumns * SlotSize) + (horizontalGaps * SlotPadding);
        int panelHeight = InventoryHeaderHeight + (innerPad * 2) + (rows * SlotSize) + (verticalGaps * SlotPadding);
        
        // Position at bottom center (default)
        int panelX = (Game1.uiViewport.Width - panelWidth) / 2;
        int panelY = Game1.uiViewport.Height - panelHeight - 100;
        
        _inventoryBounds = new Rectangle(panelX, panelY, panelWidth, panelHeight);
        
        // Create drag handle button in the header (top-right corner)
        int dragButtonSize = 32;
        int dragButtonX = panelX + panelWidth - dragButtonSize - innerPad;
        int dragButtonY = panelY + (InventoryHeaderHeight - dragButtonSize) / 2;
        _dragHandle = new ClickableComponent(
            bounds: new Rectangle(dragButtonX, dragButtonY, dragButtonSize, dragButtonSize),
            name: "dragHandle"
        );
        
        // Create clickable slots
        for (int i = 0; i < _donatableItems.Count; i++)
        {
            int col = i % InventoryColumns;
            int row = i / InventoryColumns;
            
            // Position with consistent spacing
            int slotX = panelX + innerPad + (col * (SlotSize + SlotPadding));
            int slotY = panelY + InventoryHeaderHeight + innerPad + (row * (SlotSize + SlotPadding));
            
            _inventorySlots.Add(new ClickableComponent(
                bounds: new Rectangle(slotX, slotY, SlotSize, SlotSize),
                name: i.ToString()
            ));
        }
    }
    
    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
        base.receiveLeftClick(x, y, playSound);
        
        // Block input during fade
        if (_fadeTimer > 0)
            return;

        // When using gamepad, the engine may also translate button presses into clicks.
        // If we already handled that press deterministically, ignore the next click.
        if (Game1.options.gamepadControls && _suppressNextLeftClick)
        {
            _suppressNextLeftClick = false;
            return;
        }

        // Follow-mode UX: any click cancels follow-mode. The cancel click should not also pick/place items.
        if (!IsArrangeMode && _followModeEnabled && _dragHandle != null)
        {
            Rectangle dragBounds = new Rectangle(
                _dragHandle.bounds.X + _inventoryOffset.X,
                _dragHandle.bounds.Y + _inventoryOffset.Y,
                _dragHandle.bounds.Width,
                _dragHandle.bounds.Height
            );

            if (!dragBounds.Contains(x, y))
            {
                _followModeEnabled = false;
                Game1.playSound("bigDeSelect");
                return;
            }
        }
        
        // OK button
        if (_okButton != null && _okButton.containsPoint(x, y) && ReadyToClose())
        {
            // Start exit fade sequence
            _state = 2;
            _fadeTimer = 500;
            _fadeIntoBlack = true;
            Game1.playSound("bigDeSelect");
            return;
        }
        
        // Toggle follow mode for inventory panel (drag handle hit-test must include offset)
        if (!IsArrangeMode && _dragHandle != null)
        {
            Rectangle dragBounds = new Rectangle(
                _dragHandle.bounds.X + _inventoryOffset.X,
                _dragHandle.bounds.Y + _inventoryOffset.Y,
                _dragHandle.bounds.Width,
                _dragHandle.bounds.Height
            );

            if (dragBounds.Contains(x, y))
            {
                _followModeEnabled = !_followModeEnabled;
                if (_followModeEnabled)
                {
                    _dragMouseOffset = new Point(x - (_inventoryBounds.X + _inventoryOffset.X), y - (_inventoryBounds.Y + _inventoryOffset.Y));
                    Game1.playSound("bigSelect");
                }
                else
                {
                    Game1.playSound("bigDeSelect");
                }
                return;
            }
        }
        
        // Handle inventory clicks (donation mode only)
        if (!IsArrangeMode && HeldItem == null && _donatableItems.Count > 0)
        {
            // Adjust for offset
            int adjustedX = x - _inventoryOffset.X;
            int adjustedY = y - _inventoryOffset.Y;
            
            for (int i = 0; i < _inventorySlots.Count; i++)
            {
                if (_inventorySlots[i].containsPoint(adjustedX, adjustedY) && i < _donatableItems.Count)
                {
                    Item item = _donatableItems[i];
                    
                    // Remove from PLAYER inventory (prevents duplication)
                    for (int j = 0; j < Game1.player.Items.Count; j++)
                    {
                        if (Game1.player.Items[j] == item)
                        {
                            HeldItem = item.getOne();
                            Game1.player.Items[j] = item.ConsumeStack(1);
                            if (Game1.player.Items[j] == null || Game1.player.Items[j].Stack <= 0)
                            {
                                Game1.player.Items[j] = null;
                            }
                            break;
                        }
                    }
                    
                    // Rebuild inventory display
                    InitializeDonationInventory();
                    _controllerSelected = null;
                    Game1.playSound("dwop");
                    return;
                }
            }
        }
        
        // Handle museum tile clicks
        HandleMuseumTileClick(x, y);
    }
    
    private void HandleMuseumTileClick(int x, int y)
    {
        // In donation mode, don't handle tile clicks if clicking inventory area
        if (!IsArrangeMode && _donatableItems.Count > 0)
        {
            Rectangle inventoryArea = new Rectangle(
                _inventoryBounds.X + _inventoryOffset.X,
                _inventoryBounds.Y + _inventoryOffset.Y,
                _inventoryBounds.Width,
                _inventoryBounds.Height
            );
            if (inventoryArea.Contains(x, y))
                return;
        }
        
        // Convert screen coords to tile coords
        int tileX = (int)(Utility.ModifyCoordinateFromUIScale((float)x) + Game1.viewport.X) / 64;
        int tileY = (int)(Utility.ModifyCoordinateFromUIScale((float)y) + Game1.viewport.Y) / 64;
        HandleMuseumTileClickTile(tileX, tileY);
    }

    private void HandleMuseumTileClickTile(int tileX, int tileY)
    {
        Vector2 tilePos = new Vector2(tileX, tileY);

        // IMPORTANT: vanilla allows picking up from occupied tiles even though
        // isTileSuitableForMuseumPiece() returns false for occupied tiles.
        bool tileOccupied = Museum.museumPieces.ContainsKey(tilePos);
        bool tilePlaceable = tileOccupied || Museum.isTileSuitableForMuseumPiece(tileX, tileY);
        
        if (HeldItem != null)
        {
            // === PLACING/SWAPPING ===
            if (tileOccupied)
            {
                // Swap
                string existingId = Museum.museumPieces[tilePos];
                Museum.museumPieces[tilePos] = HeldItem.ItemId;
                
                Item? swappedItem = ItemRegistry.Create(existingId, allowNull: true);
                if (swappedItem == null)
                {
                    swappedItem = ItemRegistry.Create("(O)" + existingId, allowNull: true);
                }
                
                HeldItem = swappedItem;
                _holdingMuseumPiece = true;
                Game1.playSound("coin");
            }
            else
            {
                // Place
                if (!tilePlaceable)
                    return;

                if (!IsArrangeMode && !Museum.isItemSuitableForDonation(HeldItem))
                    return; // Only allow donations in donation mode
                
                int rewardsBefore = Museum.getRewardsForPlayer(Game1.player).Count;
                Museum.museumPieces.Add(tilePos, HeldItem.ItemId);
                int rewardsAfter = Museum.getRewardsForPlayer(Game1.player).Count;
                
                // Check for rewards
                if (rewardsAfter > rewardsBefore)
                {
                    SparkleText = new SparklingText(
                        Game1.dialogueFont,
                        Game1.content.LoadString("Strings\\StringsFromCSFiles:NewReward"),
                        Color.MediumSpringGreen,
                        Color.White
                    );
                    GlobalLocationOfSparklingArtifact = new Vector2(tileX, tileY - 1) * 64f;
                    Game1.playSound("reward");
                }
                else
                {
                    Game1.playSound("stoneStep");
                }
                
                HeldItem = null;
                _holdingMuseumPiece = false;

                // Donation flow: after placing one item, return to inventory navigation
                // if and only if there are still donatable items remaining.
                if (!IsArrangeMode)
                {
                    InitializeDonationInventory();
                    if (_donatableItems.Count > 0 && _inventorySlots.Count > 0)
                    {
                        _controllerSelected = _inventorySlots[0];
                    }
                }
            }
        }
        else if (tileOccupied)
        {
            // === PICKING UP ===
            string itemId = Museum.museumPieces[tilePos];
            Item? pickedItem = ItemRegistry.Create(itemId, allowNull: true);
            
            if (pickedItem == null)
            {
                pickedItem = ItemRegistry.Create("(O)" + itemId, allowNull: true);
            }
            
            if (pickedItem != null)
            {
                Museum.museumPieces.Remove(tilePos);
                HeldItem = pickedItem;
                _holdingMuseumPiece = true;
                Game1.playSound("coin");
            }
        }
    }

    
    public override void receiveRightClick(int x, int y, bool playSound = true)
    {
        // Right-click returns held item
        if (HeldItem != null)
        {
            if (_holdingMuseumPiece)
            {
                // Return to museum at free spot
                Vector2 freeSpot = Museum.getFreeDonationSpot();
                if (freeSpot != Vector2.Zero)
                {
                    Museum.museumPieces.TryAdd(freeSpot, HeldItem.ItemId);
                }
            }
            else
            {
                // Return to player inventory
                HeldItem = Game1.player.addItemToInventory(HeldItem);
                if (HeldItem != null)
                {
                    // Couldn't return, drop it
                    Game1.createItemDebris(HeldItem, Game1.player.Position, -1);
                }
            }
            
            HeldItem = null;
            _holdingMuseumPiece = false;
            Game1.playSound("dwop");
        }
    }
    
    public override void releaseLeftClick(int x, int y)
    {
        base.releaseLeftClick(x, y);
        // Follow mode is toggle-based, not drag-based
    }
    
    public override void receiveKeyPress(Keys key)
    {
        // Block input during fade
        if (_fadeTimer > 0)
            return;
        
        // Handle ESC key
        if (key == Keys.Escape && ReadyToClose())
        {
            // Start exit fade sequence
            _state = 2;
            _fadeTimer = 500;
            _fadeIntoBlack = true;
            Game1.playSound("bigDeSelect");
        }
    }
    
    public override void update(GameTime time)
    {
        base.update(time);
        
        // Handle fade transitions
        if (_fadeTimer > 0)
        {
            _fadeTimer -= time.ElapsedGameTime.Milliseconds;
            
            if (_fadeTimer <= 0)
            {
                switch (_state)
                {
                    case 0: // Starting fade done - move camera to museum
                        _state = 1;
                        Game1.viewportFreeze = true;
                        Game1.viewport.Location = new Location(1152, 128);
                        Game1.clampViewportToGameMap();
                        _fadeTimer = 800;
                        _fadeIntoBlack = false;
                        break;
                    case 2: // Exit fade started - return camera to player
                        Game1.viewportFreeze = false;
                        Game1.viewport.X = (int)Game1.player.Position.X - Game1.viewport.Width / 2;
                        Game1.viewport.Y = (int)Game1.player.Position.Y - Game1.viewport.Height / 2;
                        _fadeIntoBlack = false;
                        _fadeTimer = 800;
                        _state = 3;
                        break;
                    case 3: // Exit fade done
                        exitThisMenuNoSound();
                        break;
                }
            }
        }
        
        SparkleText?.update(time);

        UpdateControllerNavigation(time);
        
        // Update inventory panel position in follow mode (mouse or gamepad)
        if (_followModeEnabled && !IsArrangeMode)
        {
            if (Game1.options.gamepadControls)
            {
                var state = GamePad.GetState(Game1.playerOneIndex);
                Vector2 stick = state.ThumbSticks.Left;
                stick.Y *= -1f;

                if (Math.Abs(stick.X) < FollowStickDeadzone)
                    stick.X = 0f;
                if (Math.Abs(stick.Y) < FollowStickDeadzone)
                    stick.Y = 0f;

                if (stick != Vector2.Zero)
                {
                    float dt = (float)time.ElapsedGameTime.TotalSeconds;
                    Point delta = new Point(
                        (int)(stick.X * FollowPanelSpeedPxPerSec * dt),
                        (int)(stick.Y * FollowPanelSpeedPxPerSec * dt)
                    );

                    _inventoryOffset = new Point(_inventoryOffset.X + delta.X, _inventoryOffset.Y + delta.Y);
                    ClampInventoryOffsetToScreen();
                }
            }
            else
            {
                int mouseX = Game1.getMouseX();
                int mouseY = Game1.getMouseY();
                int newX = mouseX - _dragMouseOffset.X;
                int newY = mouseY - _dragMouseOffset.Y;

                _inventoryOffset = new Point(
                    newX - _inventoryBounds.X,
                    newY - _inventoryBounds.Y
                );
                ClampInventoryOffsetToScreen();
            }
        }
    }
    
    public override void draw(SpriteBatch b)
    {
        _hoverText = "";

        // Draw fade overlay during transitions
        if (_fadeTimer > 0)
        {
            float alpha = _fadeIntoBlack ? (1f - _fadeTimer / 800f) : (_fadeTimer / 800f);
            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * alpha);
            return; // Don't draw anything else during fade
        }
        
        // No dim overlay (requested)
        
        // Draw valid placement tiles when holding item
        if (HeldItem != null)
        {
            Game1.StartWorldDrawInUI(b);
            for (int y = Game1.viewport.Y / 64 - 1; y < (Game1.viewport.Y + Game1.viewport.Height) / 64 + 2; ++y)
            {
                for (int x = Game1.viewport.X / 64 - 1; x < (Game1.viewport.X + Game1.viewport.Width) / 64 + 1; ++x)
                {
                    if (Museum.isTileSuitableForMuseumPiece(x, y))
                    {
                        b.Draw(
                            Game1.mouseCursors,
                            Game1.GlobalToLocal(Game1.viewport, new Vector2(x, y) * 64f),
                            Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 29),
                            Color.LightGreen
                        );
                    }
                }
            }
            Game1.EndWorldDrawInUI(b);
        }
        
        // Draw instruction text at top center
        string instructionText = IsArrangeMode 
            ? (HeldItem != null ? "Place item on a shelf" : "Click a museum piece to rearrange it")
            : (_donatableItems.Count > 0 ? "Donate artifacts and minerals to the museum" : "No donatable items in inventory");
        
        Vector2 textSize = Game1.dialogueFont.MeasureString(instructionText);
        Vector2 textPos = new Vector2(
            (Game1.uiViewport.Width - textSize.X) / 2,
            32
        );
        
        // Draw text with shadow
        b.DrawString(Game1.dialogueFont, instructionText, textPos + new Vector2(2, 2), Color.Black);
        b.DrawString(Game1.dialogueFont, instructionText, textPos, Color.White);
        
        // Draw inventory panel (donation mode only, and only if has items)
        if (!IsArrangeMode && _donatableItems.Count > 0)
        {
            DrawDonationInventory(b);
        }
        
        // Draw OK button
        _okButton?.draw(b);
        
        // Draw held item
        if (HeldItem != null)
        {
            HeldItem.drawInMenu(b, new Vector2(Game1.getOldMouseX() + 8, Game1.getOldMouseY() + 8), 1f);
        }
        
        // Draw hover text
        if (!string.IsNullOrEmpty(_hoverText))
        {
            drawHoverText(b, _hoverText, Game1.smallFont);
        }
        
        // Draw sparkle text (rewards)
        SparkleText?.draw(b, Utility.ModifyCoordinatesForUIScale(
            Game1.GlobalToLocal(Game1.viewport, GlobalLocationOfSparklingArtifact)));
        
        // Draw cursor
        drawMouse(b);
    }

    public override void receiveGamePadButton(Buttons b)
    {
        base.receiveGamePadButton(b);

        if (_fadeTimer > 0)
            return;

        // D-pad navigation
        switch (b)
        {
            case Buttons.DPadUp:
                if (!IsArrangeMode && _followModeEnabled)
                    return;
                MoveControllerSelection(0, -1);
                return;
            case Buttons.DPadDown:
                if (!IsArrangeMode && _followModeEnabled)
                    return;
                MoveControllerSelection(0, 1);
                return;
            case Buttons.DPadLeft:
                if (!IsArrangeMode && _followModeEnabled)
                    return;
                MoveControllerSelection(-1, 0);
                return;
            case Buttons.DPadRight:
                if (!IsArrangeMode && _followModeEnabled)
                    return;
                MoveControllerSelection(1, 0);
                return;
        }

        // A/B are handled deterministically in UpdateControllerNavigation() via edge detection.
    }

    private void ActivateControllerSelection()
    {
        EnsureControllerSelection();
        if (_controllerSelected == null)
            return;

        // OK button
        if (_okButton != null && _controllerSelected == _okButton)
        {
            receiveLeftClick(_okButton.bounds.Center.X, _okButton.bounds.Center.Y);
            return;
        }

        // Drag handle / move toggle
        if (!IsArrangeMode && _dragHandle != null && _controllerSelected == _dragHandle)
        {
            // Toggle follow-mode directly; for gamepad this is a modal state.
            _followModeEnabled = !_followModeEnabled;
            Game1.playSound(_followModeEnabled ? "bigSelect" : "bigDeSelect");
            return;
        }

        // Inventory slot
        if (!IsArrangeMode && _inventorySlots.Contains(_controllerSelected))
        {
            Point click = GetClickPointForComponent(_controllerSelected);
            receiveLeftClick(click.X, click.Y);
            return;
        }

        // Museum slot (arrange OR donation while holding)
        if (_museumComponentToTile.TryGetValue(_controllerSelected, out Vector2 tile))
        {
            HandleMuseumTileClickTile((int)tile.X, (int)tile.Y);
        }
    }

    private void UpdateControllerNavigation(GameTime time)
    {
        if (!Game1.options.gamepadControls)
            return;

        if (_fadeTimer > 0)
            return;

        EnsureMuseumSlotComponentsBuilt();
        EnsureControllerSelection();

        GamePadState state = Game1.input.GetGamePadState();

        // A/B edge handling (do NOT rely on engine-generated clicks)
        bool aPressed = state.IsButtonDown(Buttons.A) && !_prevPadState.IsButtonDown(Buttons.A);
        bool bPressed = state.IsButtonDown(Buttons.B) && !_prevPadState.IsButtonDown(Buttons.B);

        if (bPressed)
        {
            _suppressNextLeftClick = true;
            if (HeldItem != null)
            {
                receiveRightClick(Game1.getMouseX(), Game1.getMouseY());
            }
            else if (ReadyToClose())
            {
                _state = 2;
                _fadeTimer = 500;
                _fadeIntoBlack = true;
                Game1.playSound("bigDeSelect");
            }
        }

        if (aPressed)
        {
            _suppressNextLeftClick = true;

            // Follow-mode toggle off
            if (!IsArrangeMode && _followModeEnabled)
            {
                _followModeEnabled = false;
                Game1.playSound("bigDeSelect");
            }
            else
            {
                ActivateControllerSelection();
            }
        }

        // Follow-mode is modal for gamepad: stick moves the panel, no navigation.
        if (!IsArrangeMode && _followModeEnabled)
        {
            _prevPadState = state;
            return;
        }

        // Donation flow: once you're holding an item, switch navigation to museum slots.
        if (!IsArrangeMode && HeldItem != null)
        {
            if (_controllerSelected != null && (_controllerSelected == _dragHandle || _inventorySlots.Contains(_controllerSelected)))
            {
                if (_museumSlotComponents.Count > 0)
                {
                    _controllerSelected = _museumSlotComponents[0];
                    SnapMouseToComponent(_controllerSelected);
                }
            }
        }

        // Left-stick navigation (repeat)
        Vector2 stick = state.ThumbSticks.Left;
        stick.Y *= -1f;

        int dirX = 0;
        int dirY = 0;
        if (Math.Abs(stick.X) >= NavStickDeadzone)
            dirX = stick.X > 0 ? 1 : -1;
        if (Math.Abs(stick.Y) >= NavStickDeadzone)
            dirY = stick.Y > 0 ? 1 : -1;

        if (dirX == 0 && dirY == 0)
        {
            _navRepeatTimerMs = 0;
            _navWasNeutral = true;
            _prevPadState = state;
            return;
        }

        if (_navWasNeutral)
        {
            MoveControllerSelection(dirX, dirY);
            _navRepeatTimerMs = NavInitialDelayMs;
            _navWasNeutral = false;
            _prevPadState = state;
            return;
        }

        _navRepeatTimerMs -= time.ElapsedGameTime.Milliseconds;
        if (_navRepeatTimerMs > 0)
            return;

        MoveControllerSelection(dirX, dirY);
        _navRepeatTimerMs = NavRepeatDelayMs;

        _prevPadState = state;
    }

    private void EnsureControllerSelection()
    {
        if (_controllerSelected != null)
            return;

        // Donation: if holding an item, museum navigation takes priority.
        if (!IsArrangeMode && HeldItem != null && _museumSlotComponents.Count > 0)
        {
            _controllerSelected = _museumSlotComponents[0];
            return;
        }

        if (!IsArrangeMode && _donatableItems.Count > 0 && _inventorySlots.Count > 0)
        {
            _controllerSelected = _inventorySlots[0];
            return;
        }

        if (_museumSlotComponents.Count > 0)
        {
            _controllerSelected = _museumSlotComponents[0];
            return;
        }

        if (_okButton != null)
        {
            _controllerSelected = _okButton;
        }
    }

    private void EnsureMuseumSlotComponentsBuilt()
    {
        if (_museumSlotComponents.Count > 0)
            return;

        _museumSlotComponents.Clear();
        _museumComponentToTile.Clear();

        int mapWidth = Museum.Map.Layers[0].LayerWidth;
        int mapHeight = Museum.Map.Layers[0].LayerHeight;

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                Vector2 tile = new Vector2(x, y);
                bool occupied = Museum.museumPieces.ContainsKey(tile);
                bool suitable = Museum.isTileSuitableForMuseumPiece(x, y);
                if (!occupied && !suitable)
                    continue;

                // Bounds aren't used for correctness (tile is); keep something reasonable for UI highlight/debug.
                Point uiCenter = GetUiPointForTile(tile);
                int size = 64;
                Rectangle bounds = new Rectangle(uiCenter.X - size / 2, uiCenter.Y - size / 2, size, size);

                var comp = new ClickableComponent(bounds, $"{x},{y}");
                _museumSlotComponents.Add(comp);
                _museumComponentToTile[comp] = tile;
            }
        }

        // Stable ordering: top-to-bottom, then left-to-right
        _museumSlotComponents.Sort((a, b) =>
        {
            Vector2 ta = _museumComponentToTile[a];
            Vector2 tb = _museumComponentToTile[b];
            int byY = ta.Y.CompareTo(tb.Y);
            return byY != 0 ? byY : ta.X.CompareTo(tb.X);
        });
    }

    private void MoveControllerSelection(int dx, int dy)
    {
        EnsureControllerSelection();
        EnsureMuseumSlotComponentsBuilt();

        if (_controllerSelected == null)
            return;

        // Prefer museum navigation while holding an item in donation mode.
        if (!IsArrangeMode && HeldItem != null)
        {
            if (_museumComponentToTile.TryGetValue(_controllerSelected, out Vector2 curTileHolding))
            {
                ClickableComponent? nextHolding = FindNextMuseumSlot(curTileHolding, dx, dy);
                if (nextHolding != null)
                {
                    _controllerSelected = nextHolding;
                    SnapMouseToComponent(_controllerSelected);
                    return;
                }
            }

            if (_museumSlotComponents.Count > 0)
            {
                _controllerSelected = _museumSlotComponents[0];
                SnapMouseToComponent(_controllerSelected);
                return;
            }
        }

        // Prefer inventory navigation in donation mode when selecting an inventory slot
        if (!IsArrangeMode && _inventorySlots.Contains(_controllerSelected))
        {
            int index = _inventorySlots.IndexOf(_controllerSelected);
            int rows = (int)Math.Ceiling(_donatableItems.Count / (double)InventoryColumns);
            int col = index % InventoryColumns;
            int row = index / InventoryColumns;

            if (dx != 0)
                col = Math.Clamp(col + dx, 0, InventoryColumns - 1);
            if (dy != 0)
                row = Math.Clamp(row + dy, 0, Math.Max(0, rows - 1));

            int newIndex = row * InventoryColumns + col;
            if (newIndex >= 0 && newIndex < _inventorySlots.Count && newIndex < _donatableItems.Count)
            {
                _controllerSelected = _inventorySlots[newIndex];
                SnapMouseToComponent(_controllerSelected);
                return;
            }

            // Fallthrough: reaching beyond inventory rows goes to OK
            if (dy > 0 && _okButton != null)
            {
                _controllerSelected = _okButton;
                SnapMouseToComponent(_controllerSelected);
                return;
            }

            // Up from first row goes to move button
            if (dy < 0 && _dragHandle != null)
            {
                _controllerSelected = _dragHandle;
                SnapMouseToComponent(_controllerSelected);
                return;
            }
        }

        // Drag handle navigation
        if (!IsArrangeMode && _dragHandle != null && _controllerSelected == _dragHandle)
        {
            if (dy > 0 && _inventorySlots.Count > 0)
            {
                _controllerSelected = _inventorySlots[0];
                SnapMouseToComponent(_controllerSelected);
                return;
            }
            if (dy > 0 && _okButton != null)
            {
                _controllerSelected = _okButton;
                SnapMouseToComponent(_controllerSelected);
                return;
            }
        }

        // OK button navigation
        if (_okButton != null && _controllerSelected == _okButton)
        {
            if (dy < 0)
            {
                if (!IsArrangeMode && _donatableItems.Count > 0 && _inventorySlots.Count > 0)
                {
                    _controllerSelected = _inventorySlots[Math.Min(_inventorySlots.Count - 1, _donatableItems.Count - 1)];
                    SnapMouseToComponent(_controllerSelected);
                    return;
                }

                if (_museumSlotComponents.Count > 0)
                {
                    _controllerSelected = _museumSlotComponents[^1];
                    SnapMouseToComponent(_controllerSelected);
                    return;
                }
            }
        }

        // Museum slot navigation
        if (_museumComponentToTile.TryGetValue(_controllerSelected, out Vector2 curTile))
        {
            ClickableComponent? next = FindNextMuseumSlot(curTile, dx, dy);
            if (next != null)
            {
                _controllerSelected = next;
                SnapMouseToComponent(_controllerSelected);
                return;
            }

            // If there's no slot in that direction, allow dropping to OK
            if (dy > 0 && _okButton != null)
            {
                _controllerSelected = _okButton;
                SnapMouseToComponent(_controllerSelected);
            }
        }
    }

    private ClickableComponent? FindNextMuseumSlot(Vector2 curTile, int dx, int dy)
    {
        if (_museumSlotComponents.Count == 0)
            return null;

        ClickableComponent? best = null;
        int bestPrimary = int.MaxValue;
        int bestSecondary = int.MaxValue;

        foreach (var comp in _museumSlotComponents)
        {
            Vector2 tile = _museumComponentToTile[comp];
            int tx = (int)tile.X;
            int ty = (int)tile.Y;
            int cx = (int)curTile.X;
            int cy = (int)curTile.Y;

            if (dx > 0 && tx <= cx) continue;
            if (dx < 0 && tx >= cx) continue;
            if (dy > 0 && ty <= cy) continue;
            if (dy < 0 && ty >= cy) continue;

            int primary = dx != 0 ? Math.Abs(tx - cx) : Math.Abs(ty - cy);
            int secondary = dx != 0 ? Math.Abs(ty - cy) : Math.Abs(tx - cx);

            if (primary < bestPrimary || (primary == bestPrimary && secondary < bestSecondary))
            {
                bestPrimary = primary;
                bestSecondary = secondary;
                best = comp;
            }
        }

        return best;
    }

    private void SnapMouseToComponent(ClickableComponent component)
    {
        Point click = GetClickPointForComponent(component);
        int x = click.X;
        int y = click.Y;

        // If snapping to a museum tile, pan camera so it stays in view (vanilla-like).
        if (_museumComponentToTile.TryGetValue(component, out Vector2 tile))
        {
            PanViewportToTile(tile);
            click = GetClickPointForComponent(component);
            x = click.X;
            y = click.Y;
        }

        Game1.setMousePosition(x, y);

        // Keep hover text consistent with controller selection
        if (!IsArrangeMode && _inventorySlots.Contains(component))
        {
            int index = _inventorySlots.IndexOf(component);
            if (index >= 0 && index < _donatableItems.Count)
                _hoverText = _donatableItems[index].DisplayName;
        }
    }

    private Point GetClickPointForComponent(ClickableComponent component)
    {
        if (_museumComponentToTile.TryGetValue(component, out Vector2 tile))
            return GetUiPointForTile(tile);

        Rectangle bounds = component.bounds;
        if (!IsArrangeMode)
        {
            if (component == _dragHandle || _inventorySlots.Contains(component))
            {
                bounds = new Rectangle(
                    bounds.X + _inventoryOffset.X,
                    bounds.Y + _inventoryOffset.Y,
                    bounds.Width,
                    bounds.Height
                );
            }
        }

        return new Point(bounds.Center.X, bounds.Center.Y);
    }

    private Point GetUiPointForTile(Vector2 tile)
    {
        Vector2 worldCenter = tile * 64f + new Vector2(32f, 32f);
        Vector2 local = Game1.GlobalToLocal(Game1.viewport, worldCenter);
        Vector2 ui = Utility.ModifyCoordinatesForUIScale(local);
        return new Point((int)ui.X, (int)ui.Y);
    }

    private void PanViewportToTile(Vector2 tile)
    {
        int targetX = (int)tile.X * 64 + 32;
        int targetY = (int)tile.Y * 64 + 32;

        // Match vanilla: pan in the axis that's out of bounds.
        if (!Game1.viewport.Contains(new Location(targetX, Game1.viewport.Y + 1)))
            Game1.panScreen((int)tile.X * 64 - Game1.viewport.X, 0);
        else if (!Game1.viewport.Contains(new Location(Game1.viewport.X + 1, targetY)))
            Game1.panScreen(0, (int)tile.Y * 64 - Game1.viewport.Y);

        Game1.clampViewportToGameMap();
    }

    private void ClampInventoryOffsetToScreen()
    {
        int panelX = _inventoryBounds.X + _inventoryOffset.X;
        int panelY = _inventoryBounds.Y + _inventoryOffset.Y;
        int minX = 0;
        int minY = 0;
        int maxX = Game1.uiViewport.Width - _inventoryBounds.Width;
        int maxY = Game1.uiViewport.Height - _inventoryBounds.Height;
        panelX = Math.Clamp(panelX, minX, maxX);
        panelY = Math.Clamp(panelY, minY, maxY);
        _inventoryOffset = new Point(panelX - _inventoryBounds.X, panelY - _inventoryBounds.Y);
    }
    
    private void DrawDonationInventory(SpriteBatch b)
    {
        int panelX = _inventoryBounds.X + _inventoryOffset.X;
        int panelY = _inventoryBounds.Y + _inventoryOffset.Y;
        int panelWidth = _inventoryBounds.Width;
        int panelHeight = _inventoryBounds.Height;
        
        // Draw panel background
        IClickableMenu.drawTextureBox(
            b,
            Game1.menuTexture,
            new Rectangle(0, 256, 60, 60),
            panelX,
            panelY,
            panelWidth,
            panelHeight,
            Color.White,
            1f,
            false
        );
        
        // Draw drag button with follow mode indicator
        int mouseX = Game1.getMouseX();
        int mouseY = Game1.getMouseY();
        string dragTooltip = "";
        
        if (_dragHandle != null)
        {
            Rectangle dragBounds = new Rectangle(
                _dragHandle.bounds.X + _inventoryOffset.X,
                _dragHandle.bounds.Y + _inventoryOffset.Y,
                _dragHandle.bounds.Width,
                _dragHandle.bounds.Height
            );
            
            bool hovering = dragBounds.Contains(mouseX, mouseY);
            
            // Visual feedback (no yellow guide box)
            Color dragColor = hovering ? Color.White : (Color.White * 0.7f);

            // Draw as a real button: small texture box container + icon inside.
            Rectangle buttonBounds = dragBounds;
            int inset = 4;
            Rectangle innerBounds = new Rectangle(
                buttonBounds.X + inset,
                buttonBounds.Y + inset,
                Math.Max(8, buttonBounds.Width - inset * 2),
                Math.Max(8, buttonBounds.Height - inset * 2)
            );

            IClickableMenu.drawTextureBox(
                b,
                Game1.menuTexture,
                new Rectangle(0, 256, 60, 60),
                buttonBounds.X,
                buttonBounds.Y,
                buttonBounds.Width,
                buttonBounds.Height,
                hovering ? Color.White : (Color.White * 0.85f),
                1f,
                false
            );

            DrawMoveIcon(b, innerBounds, dragColor);
            
            // Tooltip for drag button
            if (hovering)
            {
                dragTooltip = _followModeEnabled ? "Click to lock position" : "Click to follow cursor";
            }
        }
        
        // Draw inventory slots
        _hoverText = dragTooltip; // Start with drag tooltip, can be overwritten by item hover
        for (int i = 0; i < _inventorySlots.Count && i < _donatableItems.Count; i++)
        {
            var slot = _inventorySlots[i];
            var item = _donatableItems[i];
            
            if (item == null || item.Stack <= 0)
                continue;
            
            Rectangle slotBounds = new Rectangle(
                slot.bounds.X + _inventoryOffset.X,
                slot.bounds.Y + _inventoryOffset.Y,
                slot.bounds.Width,
                slot.bounds.Height
            );
            
            // Draw slot background
            b.Draw(
                Game1.menuTexture,
                slotBounds,
                new Rectangle(128, 128, 64, 64),
                Color.White
            );
            
            // Draw item
            item.drawInMenu(
                b,
                new Vector2(slotBounds.X, slotBounds.Y),
                1f,
                1f,
                1f,
                StackDrawType.Draw,
                Color.White,
                true
            );
            
            // Hover tooltip
            if (slotBounds.Contains(mouseX, mouseY))
            {
                _hoverText = item.DisplayName;
            }
        }
        
        // No item count text needed
    }

    private static void DrawMoveIcon(SpriteBatch b, Rectangle bounds, Color color)
    {
        // Simple cross/4-way move glyph so it can't be mistaken for zoom.
        int thickness = Math.Max(2, bounds.Width / 10);
        int padding = Math.Max(3, bounds.Width / 6);
        int cx = bounds.Center.X;
        int cy = bounds.Center.Y;

        int left = bounds.Left + padding;
        int right = bounds.Right - padding;
        int top = bounds.Top + padding;
        int bottom = bounds.Bottom - padding;

        // Horizontal bar
        b.Draw(Game1.staminaRect, new Rectangle(left, cy - thickness / 2, right - left, thickness), color);
        // Vertical bar
        b.Draw(Game1.staminaRect, new Rectangle(cx - thickness / 2, top, thickness, bottom - top), color);

        // End caps (suggest direction)
        int cap = thickness + 2;
        b.Draw(Game1.staminaRect, new Rectangle(left - 1, cy - cap / 2, cap, cap), color);
        b.Draw(Game1.staminaRect, new Rectangle(right - cap + 1, cy - cap / 2, cap, cap), color);
        b.Draw(Game1.staminaRect, new Rectangle(cx - cap / 2, top - 1, cap, cap), color);
        b.Draw(Game1.staminaRect, new Rectangle(cx - cap / 2, bottom - cap + 1, cap, cap), color);
    }
    
    public bool ReadyToClose()
    {
        return HeldItem == null && !_holdingMuseumPiece;
    }
    
    public override void emergencyShutDown()
    {
        base.emergencyShutDown();
        
        // Return held item
        if (HeldItem != null)
        {
            if (_holdingMuseumPiece)
            {
                Vector2 freeSpot = Museum.getFreeDonationSpot();
                if (freeSpot != Vector2.Zero)
                {
                    Museum.museumPieces.TryAdd(freeSpot, HeldItem.ItemId);
                }
            }
            else
            {
                HeldItem = Game1.player.addItemToInventory(HeldItem);
                if (HeldItem != null)
                {
                    Game1.createItemDebris(HeldItem, Game1.player.Position, -1);
                }
            }
        }
        
        Game1.displayHUD = true;
    }
    
    protected override void cleanupBeforeExit()
    {
        base.cleanupBeforeExit();
        
        // Return held item
        if (HeldItem != null)
        {
            if (_holdingMuseumPiece)
            {
                Vector2 freeSpot = Museum.getFreeDonationSpot();
                if (freeSpot != Vector2.Zero)
                {
                    Museum.museumPieces.TryAdd(freeSpot, HeldItem.ItemId);
                }
            }
            else
            {
                HeldItem = Game1.player.addItemToInventory(HeldItem);
                if (HeldItem != null)
                {
                    Game1.createItemDebris(HeldItem, Game1.player.Position, -1);
                }
            }
        }
        
        Game1.displayHUD = true;
    }
}
