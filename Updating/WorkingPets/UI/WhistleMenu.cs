using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Menus;
using System;
using System.Collections.Generic;

namespace WorkingPets.UI;

/// <summary>
/// Menu for selecting which pet to whistle/call.
/// Styled like the Animals page with full-body sprites and scrolling.
/// </summary>
public class WhistleMenu : IClickableMenu
{
    // Layout constants (match AnimalPage)
    private const int SlotsPerPage = 5;
    private const int SlotHeight = 112;
    
    // Pet entries and components
    private readonly List<PetEntry> PetEntries = new();
    private readonly List<ClickableTextureComponent> PetSprites = new();
    private readonly List<ClickableTextureComponent> PetSlots = new();
    
    // Scrolling
    private int SlotPosition = 0;
    private ClickableTextureComponent? UpButton;
    private ClickableTextureComponent? DownButton;
    private ClickableTextureComponent? ScrollBar;
    private Rectangle ScrollBarRunner;
    private bool Scrolling = false;
    
    private string HoverText = "";
    
    public WhistleMenu() : base(
        Game1.uiViewport.Width / 2 - 400,
        Game1.uiViewport.Height / 2 - 300,
        800 + IClickableMenu.borderWidth * 2,
        600 + IClickableMenu.borderWidth * 2
    )
    {
        // Get all pets
        var allPets = new List<Pet>();
        Utility.ForEachLocation(location =>
        {
            foreach (var character in location.characters)
            {
                if (character is Pet pet)
                    allPets.Add(pet);
            }
            return true;
        });

        // Create entries for each pet
        foreach (var pet in allPets)
        {
            bool isInSameLocation = pet.currentLocation == Game1.player.currentLocation;
            string displayName = string.IsNullOrWhiteSpace(pet.displayName) ? pet.Name : pet.displayName;
            
            var manager = ModEntry.PetManager?.GetManagerForPet(pet);
            bool isAlreadyFollowing = manager?.IsFollowing == true;
            bool isExploring = manager?.IsExploring == true;
            
            PetEntries.Add(new PetEntry
            {
                Pet = pet,
                DisplayName = displayName,
                IsEnabled = (isInSameLocation && !isAlreadyFollowing) || isExploring, // Can whistle exploring pets
                Sprite = pet.Sprite.Texture,
                SpriteSourceRect = new Rectangle(0, pet.Sprite.SourceRect.Height * 2 - 24, pet.Sprite.SourceRect.Width, 24)
            });
        }

        CreateComponents();
    }
    
    private void CreateComponents()
    {
        PetSprites.Clear();
        PetSlots.Clear();
        
        // Create sprite and slot components for each pet
        for (int i = 0; i < PetEntries.Count; i++)
        {
            var entry = PetEntries[i];
            
            // Sprite component (full-body pet sprite like Animals page)
            Rectangle bounds = new Rectangle(
                xPositionOnScreen + IClickableMenu.borderWidth + 4,
                0, // Y position set in UpdateSlots
                width - IClickableMenu.borderWidth * 2,
                64
            );
            
            var spriteComp = new ClickableTextureComponent(
                i.ToString(),
                bounds,
                null,
                "",
                entry.Sprite,
                entry.SpriteSourceRect,
                4f
            );
            PetSprites.Add(spriteComp);
            
            // Clickable slot
            var slot = new ClickableTextureComponent(
                new Rectangle(
                    xPositionOnScreen + IClickableMenu.borderWidth,
                    0, // Y position set in UpdateSlots
                    width - IClickableMenu.borderWidth * 2,
                    SlotHeight
                ),
                null,
                new Rectangle(0, 0, 0, 0),
                4f
            )
            {
                myID = i,
                downNeighborID = i + 1,
                upNeighborID = i - 1
            };
            
            if (slot.upNeighborID < 0)
                slot.upNeighborID = 12342;
            
            PetSlots.Add(slot);
        }
        
        // Create scroll buttons
        UpButton = new ClickableTextureComponent(
            new Rectangle(xPositionOnScreen + width + 16, yPositionOnScreen + 64, 44, 48),
            Game1.mouseCursors,
            new Rectangle(421, 459, 11, 12),
            4f
        );
        
        DownButton = new ClickableTextureComponent(
            new Rectangle(xPositionOnScreen + width + 16, yPositionOnScreen + height - 64, 44, 48),
            Game1.mouseCursors,
            new Rectangle(421, 472, 11, 12),
            4f
        );
        
        ScrollBar = new ClickableTextureComponent(
            new Rectangle(
                UpButton.bounds.X + 12,
                UpButton.bounds.Y + UpButton.bounds.Height + 4,
                24,
                40
            ),
            Game1.mouseCursors,
            new Rectangle(435, 463, 6, 10),
            4f
        );
        
        ScrollBarRunner = new Rectangle(
            ScrollBar.bounds.X,
            UpButton.bounds.Y + UpButton.bounds.Height + 4,
            ScrollBar.bounds.Width,
            height - 128 - UpButton.bounds.Height - 8
        );
        
        UpdateSlots();
    }
    
    private void UpdateSlots()
    {
        // Update slot Y positions based on scroll
        for (int i = 0; i < PetSlots.Count; i++)
        {
            PetSlots[i].bounds.Y = GetRowPosition(i - 1);
        }
        
        // Update sprite Y positions (only visible ones)
        int index = 0;
        for (int i = SlotPosition; i < SlotPosition + SlotsPerPage && i < PetSprites.Count; i++)
        {
            int y = yPositionOnScreen + IClickableMenu.borderWidth + 32 + SlotHeight * index + 16;
            PetSprites[i].bounds.Y = y + 8; // Offset for centering
            index++;
        }
    }
    
    private int GetRowPosition(int slotIndex)
    {
        int adjustedIndex = slotIndex - SlotPosition;
        return yPositionOnScreen + IClickableMenu.borderWidth + 160 + 4 + adjustedIndex * SlotHeight;
    }
    
    private void SetScrollBarToCurrentIndex()
    {
        if (PetSprites.Count > 0)
        {
            ScrollBar!.bounds.Y = ScrollBarRunner.Height / Math.Max(1, PetSprites.Count - SlotsPerPage + 1) * SlotPosition + UpButton!.bounds.Bottom + 4;
            
            if (SlotPosition == PetSprites.Count - SlotsPerPage)
                ScrollBar.bounds.Y = DownButton!.bounds.Y - ScrollBar.bounds.Height - 4;
        }
        
        UpdateSlots();
    }
    
    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
        base.receiveLeftClick(x, y, playSound);
        
        // Check if clicked a pet slot
        for (int i = SlotPosition; i < SlotPosition + SlotsPerPage && i < PetSlots.Count; i++)
        {
            if (PetSlots[i].bounds.Contains(x, y) && PetEntries[i].IsEnabled)
            {
                WhistlePet(PetEntries[i].Pet);
                Game1.playSound("breathin");
                exitThisMenu();
                return;
            }
        }
        
        // Scroll up
        if (UpButton?.bounds.Contains(x, y) == true && SlotPosition > 0)
        {
            UpArrowPressed();
            Game1.playSound("shiny4");
        }
        
        // Scroll down
        else if (DownButton?.bounds.Contains(x, y) == true && SlotPosition < Math.Max(0, PetSprites.Count - SlotsPerPage))
        {
            DownArrowPressed();
            Game1.playSound("shiny4");
        }
        
        // Start scrollbar dragging
        else if (ScrollBar?.bounds.Contains(x, y) == true)
        {
            Scrolling = true;
        }
    }
    
    public override void leftClickHeld(int x, int y)
    {
        base.leftClickHeld(x, y);
        
        if (Scrolling)
        {
            int oldY = ScrollBar!.bounds.Y;
            ScrollBar.bounds.Y = Math.Min(
                yPositionOnScreen + height - 64 - 12 - ScrollBar.bounds.Height,
                Math.Max(y, yPositionOnScreen + UpButton!.bounds.Height + 20)
            );
            
            float percentage = (float)(y - ScrollBarRunner.Y) / ScrollBarRunner.Height;
            SlotPosition = Math.Min(
                PetSprites.Count - SlotsPerPage,
                Math.Max(0, (int)(PetSprites.Count * percentage))
            );
            
            SetScrollBarToCurrentIndex();
            
            if (oldY != ScrollBar.bounds.Y)
                Game1.playSound("shiny4");
        }
    }
    
    public override void releaseLeftClick(int x, int y)
    {
        base.releaseLeftClick(x, y);
        Scrolling = false;
    }
    
    public override void receiveScrollWheelAction(int direction)
    {
        base.receiveScrollWheelAction(direction);
        
        if (direction > 0 && SlotPosition > 0)
        {
            UpArrowPressed();
            Game1.playSound("shiny4");
        }
        else if (direction < 0 && SlotPosition < Math.Max(0, PetSprites.Count - SlotsPerPage))
        {
            DownArrowPressed();
            Game1.playSound("shiny4");
        }
    }
    
    private void UpArrowPressed()
    {
        SlotPosition--;
        SetScrollBarToCurrentIndex();
    }
    
    private void DownArrowPressed()
    {
        SlotPosition++;
        SetScrollBarToCurrentIndex();
    }
    
    public override void performHoverAction(int x, int y)
    {
        base.performHoverAction(x, y);
        
        HoverText = "";
        UpButton?.tryHover(x, y);
        DownButton?.tryHover(x, y);
        
        // Check hover on pet slots
        for (int i = SlotPosition; i < SlotPosition + SlotsPerPage && i < PetSlots.Count; i++)
        {
            if (PetSlots[i].bounds.Contains(x, y))
            {
                var entry = PetEntries[i];
                if (entry.IsEnabled)
                {
                    HoverText = $"Whistle {entry.DisplayName} to follow you";
                }
                else
                {
                    var manager = ModEntry.PetManager?.GetManagerForPet(entry.Pet);
                    if (manager?.IsExploring == true)
                        HoverText = $"{entry.DisplayName} is exploring the valley.";
                    else if (manager?.IsFollowing == true)
                        HoverText = $"{entry.DisplayName} is already following you!";
                    else
                        HoverText = $"{entry.DisplayName} is not on the same area.";
                }
                break;
            }
        }
    }
    
    private void WhistlePet(Pet pet)
    {
        var manager = ModEntry.PetManager?.GetManagerForPet(pet);
        if (manager == null)
            return;
        
        // Stop exploring if needed
        if (manager.IsExploring)
            manager.StopExploring();
        
        // Stop working if needed
        if (manager.IsWorking)
            manager.ToggleWork();
        
        // Start following
        if (!manager.IsFollowing)
            manager.ToggleFollow();
        
        Game1.showGlobalMessage($"{pet.displayName} is coming to you!");
        Game1.playSound("whistle");
    }
    
    public override void draw(SpriteBatch b)
    {
        // Dim background
        b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.4f);
        
        // Draw main dialog box
        Game1.drawDialogueBox(
            xPositionOnScreen,
            yPositionOnScreen,
            width,
            height,
            false,
            true
        );
        
        // Title with background container
        string title = "Whistle Pet";
        Vector2 titleSize = Game1.dialogueFont.MeasureString(title);
        int titleX = xPositionOnScreen + width / 2 - (int)titleSize.X / 2;
        int titleY = yPositionOnScreen + IClickableMenu.borderWidth + 12;
        
        // Draw title background box
        drawTextureBox(
            b,
            Game1.menuTexture,
            new Rectangle(0, 256, 60, 60),
            titleX - 24,
            titleY - 8,
            (int)titleSize.X + 48,
            (int)titleSize.Y + 16,
            Color.White,
            1f,
            false
        );
        
        b.DrawString(
            Game1.dialogueFont,
            title,
            new Vector2(titleX, titleY),
            Game1.textColor
        );
        
        // Draw horizontal dividers (like Animals page)
        int dividerY = yPositionOnScreen + IClickableMenu.borderWidth + 128 + 4;
        for (int i = 0; i < Math.Min(SlotsPerPage - 1, PetEntries.Count - 1); i++)
        {
            drawHorizontalPartition(b, dividerY + i * SlotHeight, true);
        }
        
        // Draw pet slots
        for (int i = SlotPosition; i < SlotPosition + SlotsPerPage && i < PetEntries.Count; i++)
        {
            DrawPetSlot(b, i);
        }
        
        // Draw scroll buttons if needed
        if (PetEntries.Count > SlotsPerPage)
        {
            UpButton?.draw(b);
            DownButton?.draw(b);
            
            // Draw scrollbar track
            drawTextureBox(
                b,
                Game1.mouseCursors,
                new Rectangle(403, 383, 6, 6),
                ScrollBarRunner.X,
                ScrollBarRunner.Y,
                ScrollBarRunner.Width,
                ScrollBarRunner.Height,
                Color.White,
                4f,
                false
            );
            
            ScrollBar?.draw(b);
        }
        
        // Hover text
        if (!string.IsNullOrEmpty(HoverText))
        {
            drawHoverText(b, HoverText, Game1.smallFont);
        }
        
        // Draw cursor
        drawMouse(b);
    }
    
    private void DrawPetSlot(SpriteBatch b, int index)
    {
        var entry = PetEntries[index];
        var sprite = PetSprites[index];
        var slot = PetSlots[index];
        
        // Highlight if hovering and enabled
        if (entry.IsEnabled && slot.bounds.Contains(Game1.getMouseX(), Game1.getMouseY()))
        {
            b.Draw(
                Game1.staminaRect,
                new Rectangle(
                    xPositionOnScreen + IClickableMenu.borderWidth - 4,
                    sprite.bounds.Y - 4,
                    slot.bounds.Width,
                    slot.bounds.Height - 12
                ),
                Color.White * 0.25f
            );
        }
        
        // Draw pet sprite (full-body like Animals page)
        Color spriteColor = entry.IsEnabled ? Color.White : Color.Gray * 0.5f;
        sprite.draw(b, spriteColor, 0.89f);
        
        // Draw pet name (centered)
        Color textColor = entry.IsEnabled ? Game1.textColor : Game1.textColor * 0.5f;
        Vector2 nameSize = Game1.dialogueFont.MeasureString(entry.DisplayName);
        b.DrawString(
            Game1.dialogueFont,
            entry.DisplayName,
            new Vector2(
                xPositionOnScreen + IClickableMenu.borderWidth * 3 / 2 + 192 + 96 - nameSize.X / 2f,
                sprite.bounds.Y + 48 - 20f
            ),
            textColor
        );
        
        // Draw status text instead of icons
        var manager = ModEntry.PetManager?.GetManagerForPet(entry.Pet);
        string statusText = "Idle";
        Color statusColor = Game1.textColor;
        
        if (manager != null)
        {
            if (manager.IsExploring)
            {
                // Show current explore location in status
                string locationName = manager.CurrentExploreLocation ?? "Valley";
                statusText = $"Exploring ({locationName})";
                statusColor = Game1.textColor; // No gold, just normal text color
            }
            else if (manager.IsFollowing)
            {
                statusText = "Following";
                statusColor = Game1.textColor;
            }
            else if (manager.IsWorking)
            {
                statusText = "Working";
                statusColor = Game1.textColor;
            }
        }
        
        Vector2 statusSize = Game1.smallFont.MeasureString(statusText);
        b.DrawString(
            Game1.smallFont,
            statusText,
            new Vector2(
                xPositionOnScreen + width - IClickableMenu.borderWidth - statusSize.X - 16,
                sprite.bounds.Y + 40
            ),
            entry.IsEnabled ? statusColor : statusColor * 0.5f
        );
    }
    
    private class PetEntry
    {
        public Pet Pet = null!;
        public string DisplayName = "";
        public bool IsEnabled;
        public Texture2D? Sprite;
        public Rectangle SpriteSourceRect;
    }
}
