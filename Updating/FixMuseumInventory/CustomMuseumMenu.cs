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
    private const int SlotPadding = 4;
    private Point _inventoryOffset = Point.Zero;
    
    // === Arrange Mode: No Inventory ===
    // (Nothing needed - that's the point!)
    
    // === Draggable Inventory ===
    private bool _isDraggingInventory;
    private Point _dragMouseOffset;
    private ClickableComponent? _dragHandle;
    
    public CustomMuseumMenu(LibraryMuseum museum, bool arrangeMode = false)
        : base(0, 0, Game1.uiViewport.Width, Game1.uiViewport.Height, showUpperRightCloseButton: false)
    {
        Museum = museum ?? throw new ArgumentNullException(nameof(museum));
        IsArrangeMode = arrangeMode;
        
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
        
        // Calculate inventory panel size
        int rows = (int)Math.Ceiling(_donatableItems.Count / (double)InventoryColumns);
        int panelWidth = (InventoryColumns * SlotSize) + ((InventoryColumns + 1) * SlotPadding);
        int panelHeight = (rows * SlotSize) + ((rows + 1) * SlotPadding) + 80; // +80 for title bar
        
        // Position at bottom center (default)
        int panelX = (Game1.uiViewport.Width - panelWidth) / 2;
        int panelY = Game1.uiViewport.Height - panelHeight - 100;
        
        _inventoryBounds = new Rectangle(panelX, panelY, panelWidth, panelHeight);
        
        // Create drag handle at top of panel
        _dragHandle = new ClickableComponent(
            bounds: new Rectangle(panelX, panelY, panelWidth, 40),
            name: "dragHandle"
        );
        
        // Create clickable slots
        for (int i = 0; i < _donatableItems.Count; i++)
        {
            int col = i % InventoryColumns;
            int row = i / InventoryColumns;
            
            int slotX = panelX + SlotPadding + (col * (SlotSize + SlotPadding));
            int slotY = panelY + 50 + SlotPadding + (row * (SlotSize + SlotPadding));
            
            _inventorySlots.Add(new ClickableComponent(
                bounds: new Rectangle(slotX, slotY, SlotSize, SlotSize),
                name: i.ToString()
            ));
        }
    }
    
    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
        base.receiveLeftClick(x, y, playSound);
        
        // OK button
        if (_okButton != null && _okButton.containsPoint(x, y) && ReadyToClose())
        {
            exitThisMenu();
            Game1.playSound("bigDeSelect");
            return;
        }
        
        // Start dragging inventory panel
        if (!IsArrangeMode && _dragHandle != null && _dragHandle.containsPoint(x, y))
        {
            _isDraggingInventory = true;
            _dragMouseOffset = new Point(x - _inventoryBounds.X, y - _inventoryBounds.Y);
            return;
        }
        
        // Handle inventory clicks (donation mode only)
        if (!IsArrangeMode && HeldItem == null)
        {
            for (int i = 0; i < _inventorySlots.Count; i++)
            {
                if (_inventorySlots[i].containsPoint(x, y) && i < _donatableItems.Count)
                {
                    HeldItem = _donatableItems[i].getOne();
                    var consumedItem = _donatableItems[i].ConsumeStack(1);
                    
                    // Remove from list if stack depleted
                    if (consumedItem == null || consumedItem.Stack <= 0)
                    {
                        _donatableItems.RemoveAt(i);
                        InitializeDonationInventory(); // Rebuild layout
                    }
                    
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
        _isDraggingInventory = false;
    }
    
    public override void update(GameTime time)
    {
        base.update(time);
        
        SparkleText?.update(time);
        
        // Update inventory panel position while dragging
        if (_isDraggingInventory)
        {
            int newX = Game1.getMouseX() - _dragMouseOffset.X;
            int newY = Game1.getMouseY() - _dragMouseOffset.Y;
            
            _inventoryOffset = new Point(
                newX - (_inventoryBounds.X - _inventoryOffset.X),
                newY - (_inventoryBounds.Y - _inventoryOffset.Y)
            );
        }
    }
    
    public override void draw(SpriteBatch b)
    {
        // Draw fade overlay
        b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.5f);
        
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
        
        // Draw inventory panel (donation mode only)
        if (!IsArrangeMode)
        {
            DrawDonationInventory(b);
        }
        else
        {
            // Arrange mode: Draw helpful text
            string message = HeldItem != null 
                ? "Place item on a shelf or press ESC to cancel" 
                : "Click a museum piece to move it";
            
            Vector2 textSize = Game1.dialogueFont.MeasureString(message);
            Vector2 textPos = new Vector2(
                (Game1.uiViewport.Width - textSize.X) / 2,
                Game1.uiViewport.Height - 120
            );
            
            // Draw shadow
            b.DrawString(Game1.dialogueFont, message, textPos + new Vector2(2, 2), Color.Black);
            b.DrawString(Game1.dialogueFont, message, textPos, Color.White);
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
        
        // Draw title
        string title = "Donatable Items";
        Vector2 titleSize = Game1.dialogueFont.MeasureString(title);
        b.DrawString(
            Game1.dialogueFont,
            title,
            new Vector2(panelX + (panelWidth - titleSize.X) / 2, panelY + 8),
            Game1.textColor
        );
        
        // Draw drag handle hint
        int mouseX = Game1.getMouseX();
        int mouseY = Game1.getMouseY();
        if (_dragHandle != null && _dragHandle.containsPoint(mouseX, mouseY))
        {
            b.Draw(
                Game1.mouseCursors,
                new Rectangle(panelX + panelWidth / 2 - 16, panelY + 4, 32, 32),
                new Rectangle(80, 0, 16, 16),
                Color.White * 0.5f
            );
        }
        
        // Draw inventory slots
        _hoverText = "";
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
        
        // Draw count
        string countText = $"{_donatableItems.Count} item{(_donatableItems.Count != 1 ? "s" : "")}";
        Vector2 countSize = Game1.smallFont.MeasureString(countText);
        b.DrawString(
            Game1.smallFont,
            countText,
            new Vector2(panelX + panelWidth - countSize.X - 12, panelY + panelHeight - countSize.Y - 8),
            Color.DarkGray
        );
    }
    
    public bool ReadyToClose()
    {
        return HeldItem == null && !_holdingMuseumPiece && !_isDraggingInventory;
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
