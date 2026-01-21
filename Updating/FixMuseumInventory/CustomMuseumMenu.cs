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
    private const int SlotSize = 64;
    private const int SlotPadding = 8;
    private Point _inventoryOffset = Point.Zero;
    
    // === Arrange Mode: No Inventory ===
    // (Nothing needed - that's the point!)
    
    // === Draggable Inventory ===
    private bool _followModeEnabled; // Toggle mode where panel follows cursor
    private Point _dragMouseOffset;
    private ClickableComponent? _dragHandle;
    
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
        
        // Calculate inventory panel size - PROPERLY this time
        int rows = (int)Math.Ceiling(_donatableItems.Count / (double)InventoryColumns);
        // Width: padding + (columns * (slotSize + padding between))
        int panelWidth = SlotPadding + (InventoryColumns * SlotSize) + (InventoryColumns * SlotPadding);
        // Height: padding + (rows * (slotSize + padding between)) + button space
        int panelHeight = SlotPadding + (rows * SlotSize) + (rows * SlotPadding) + 40; // +40 for drag button
        
        // Position at bottom center (default)
        int panelX = (Game1.uiViewport.Width - panelWidth) / 2;
        int panelY = Game1.uiViewport.Height - panelHeight - 100;
        
        _inventoryBounds = new Rectangle(panelX, panelY, panelWidth, panelHeight);
        
        // Create drag handle button at top-right corner of panel
        int dragButtonSize = 32;
        int dragButtonX = panelX + panelWidth - dragButtonSize - SlotPadding;
        int dragButtonY = panelY + SlotPadding;
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
            int slotX = panelX + SlotPadding + (col * (SlotSize + SlotPadding));
            int slotY = panelY + SlotPadding + (row * (SlotSize + SlotPadding));
            
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
        
        // Toggle follow mode for inventory panel
        if (!IsArrangeMode && _dragHandle != null && _dragHandle.containsPoint(x, y))
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
        Vector2 tilePos = new Vector2(tileX, tileY);
        
        if (!Museum.isTileSuitableForMuseumPiece(tileX, tileY))
            return;
        
        bool tileOccupied = Museum.museumPieces.ContainsKey(tilePos);
        
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
        
        // Update inventory panel position in follow mode
        if (_followModeEnabled && !IsArrangeMode)
        {
            int mouseX = Game1.getMouseX();
            int mouseY = Game1.getMouseY();
            int newX = mouseX - _dragMouseOffset.X;
            int newY = mouseY - _dragMouseOffset.Y;
            
            _inventoryOffset = new Point(
                newX - _inventoryBounds.X,
                newY - _inventoryBounds.Y
            );
        }
    }
    
    public override void draw(SpriteBatch b)
    {
        // Draw fade overlay during transitions
        if (_fadeTimer > 0)
        {
            float alpha = _fadeIntoBlack ? (1f - _fadeTimer / 800f) : (_fadeTimer / 800f);
            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * alpha);
            return; // Don't draw anything else during fade
        }
        
        // Draw semi-transparent background overlay (less dim)
        b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.25f);
        
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
            
            // Visual feedback: Yellow when active, white when hovering, gray otherwise
            Color dragColor = _followModeEnabled ? Color.Yellow : (hovering ? Color.White : (Color.White * 0.7f));
            
            b.Draw(
                Game1.mouseCursors,
                dragBounds,
                new Rectangle(80, 0, 16, 16),
                dragColor
            );
            
            // Tooltip for drag button
            if (hovering)
            {
                dragTooltip = _followModeEnabled ? "Click to lock position" : "Click to follow cursor";
            }
            
            // Active indicator - draw pulsing border when following
            if (_followModeEnabled)
            {
                float pulse = (float)Math.Abs(Math.Sin(Game1.currentGameTime.TotalGameTime.TotalSeconds * 3));
                Color borderColor = Color.Yellow * (0.5f + pulse * 0.5f);
                
                // Draw border around entire panel
                int borderThickness = 3;
                Rectangle panelRect = new Rectangle(
                    panelX + _inventoryOffset.X,
                    panelY + _inventoryOffset.Y,
                    panelWidth,
                    panelHeight
                );
                
                // Top
                b.Draw(Game1.staminaRect, new Rectangle(panelRect.X, panelRect.Y, panelRect.Width, borderThickness), borderColor);
                // Bottom
                b.Draw(Game1.staminaRect, new Rectangle(panelRect.X, panelRect.Bottom - borderThickness, panelRect.Width, borderThickness), borderColor);
                // Left
                b.Draw(Game1.staminaRect, new Rectangle(panelRect.X, panelRect.Y, borderThickness, panelRect.Height), borderColor);
                // Right
                b.Draw(Game1.staminaRect, new Rectangle(panelRect.Right - borderThickness, panelRect.Y, borderThickness, panelRect.Height), borderColor);
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
    
    public bool ReadyToClose()
    {
        return HeldItem == null && !_holdingMuseumPiece && !_followModeEnabled;
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
