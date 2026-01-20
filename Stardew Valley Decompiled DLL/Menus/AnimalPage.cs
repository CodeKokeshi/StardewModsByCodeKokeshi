// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.AnimalPage
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Characters;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace StardewValley.Menus;

public class AnimalPage(int x, int y, int width, int height) : IClickableMenu(x, y, width, height)
{
  public const int slotsOnPage = 5;
  public string hoverText = "";
  public ClickableTextureComponent upButton;
  public ClickableTextureComponent downButton;
  public ClickableTextureComponent scrollBar;
  public Rectangle scrollBarRunner;
  /// <summary>The players and social NPCs shown in the list.</summary>
  public List<AnimalPage.AnimalEntry> AnimalEntries;
  /// <summary>The character portrait components.</summary>
  public readonly List<ClickableTextureComponent> sprites = new List<ClickableTextureComponent>();
  /// <summary>The index of the <see cref="F:StardewValley.Menus.AnimalPage.AnimalEntries" /> entry shown at the top of the scrolled view.</summary>
  public int slotPosition;
  /// <summary>The clickable slots over which character info is drawn.</summary>
  public readonly List<ClickableTextureComponent> characterSlots = new List<ClickableTextureComponent>();
  public bool scrolling;

  public void init()
  {
    this.AnimalEntries = this.FindAnimals();
    this.CreateComponents();
    this.slotPosition = 0;
    this.setScrollBarToCurrentIndex();
    this.updateSlots();
  }

  public override void populateClickableComponentList()
  {
    this.init();
    base.populateClickableComponentList();
  }

  /// <summary>Find all social NPCs which should be shown on the social page.</summary>
  public List<AnimalPage.AnimalEntry> FindAnimals()
  {
    List<AnimalPage.AnimalEntry> collection1 = new List<AnimalPage.AnimalEntry>();
    List<AnimalPage.AnimalEntry> source = new List<AnimalPage.AnimalEntry>();
    List<AnimalPage.AnimalEntry> collection2 = new List<AnimalPage.AnimalEntry>();
    foreach (Character allAnimal in this.GetAllAnimals())
    {
      switch (allAnimal)
      {
        case Pet _:
          collection1.Add(new AnimalPage.AnimalEntry(allAnimal));
          continue;
        case Horse _:
          collection2.Add(new AnimalPage.AnimalEntry(allAnimal));
          continue;
        default:
          source.Add(new AnimalPage.AnimalEntry(allAnimal));
          continue;
      }
    }
    foreach (Farmer allFarmer in Game1.getAllFarmers())
    {
      if (allFarmer.mount != null)
        collection2.Add(new AnimalPage.AnimalEntry((Character) allFarmer.mount));
    }
    List<AnimalPage.AnimalEntry> animals = new List<AnimalPage.AnimalEntry>();
    animals.AddRange((IEnumerable<AnimalPage.AnimalEntry>) collection1);
    animals.AddRange((IEnumerable<AnimalPage.AnimalEntry>) collection2);
    animals.AddRange((IEnumerable<AnimalPage.AnimalEntry>) source.OrderBy<AnimalPage.AnimalEntry, string>((Func<AnimalPage.AnimalEntry, string>) (entry => entry.AnimalBaseType)).ThenBy<AnimalPage.AnimalEntry, string>((Func<AnimalPage.AnimalEntry, string>) (entry => entry.AnimalType)).ThenByDescending<AnimalPage.AnimalEntry, int>((Func<AnimalPage.AnimalEntry, int>) (entry => entry.FriendshipLevel)));
    return animals;
  }

  /// <summary>Get all animals from the world and friendship data.</summary>
  public IEnumerable<Character> GetAllAnimals()
  {
    List<Character> animals = new List<Character>();
    Utility.ForEachLocation((Func<GameLocation, bool>) (location =>
    {
      foreach (NPC character in location.characters)
      {
        if ((character is Pet || character is Horse) && !character.hideFromAnimalSocialMenu.Value)
          animals.Add((Character) character);
      }
      foreach (FarmAnimal farmAnimal in location.animals.Values)
      {
        if (!farmAnimal.hideFromAnimalSocialMenu.Value)
          animals.Add((Character) farmAnimal);
      }
      return true;
    }));
    return (IEnumerable<Character>) animals;
  }

  /// <summary>Load the clickable components to display.</summary>
  public void CreateComponents()
  {
    this.sprites.Clear();
    this.characterSlots.Clear();
    for (int index = 0; index < this.AnimalEntries.Count; ++index)
    {
      this.sprites.Add(this.CreateSpriteComponent(this.AnimalEntries[index], index));
      ClickableTextureComponent textureComponent1 = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + IClickableMenu.borderWidth, 0, this.width - IClickableMenu.borderWidth * 2, this.rowPosition(1) - this.rowPosition(0)), (Texture2D) null, new Rectangle(0, 0, 0, 0), 4f);
      textureComponent1.myID = index;
      textureComponent1.downNeighborID = index + 1;
      textureComponent1.upNeighborID = index - 1;
      ClickableTextureComponent textureComponent2 = textureComponent1;
      if (textureComponent2.upNeighborID < 0)
        textureComponent2.upNeighborID = 12342;
      this.characterSlots.Add(textureComponent2);
    }
    this.upButton = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + this.width + 16 /*0x10*/, this.yPositionOnScreen + 64 /*0x40*/, 44, 48 /*0x30*/), Game1.mouseCursors, new Rectangle(421, 459, 11, 12), 4f);
    this.downButton = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + this.width + 16 /*0x10*/, this.yPositionOnScreen + this.height - 64 /*0x40*/, 44, 48 /*0x30*/), Game1.mouseCursors, new Rectangle(421, 472, 11, 12), 4f);
    this.scrollBar = new ClickableTextureComponent(new Rectangle(this.upButton.bounds.X + 12, this.upButton.bounds.Y + this.upButton.bounds.Height + 4, 24, 40), Game1.mouseCursors, new Rectangle(435, 463, 6, 10), 4f);
    this.scrollBarRunner = new Rectangle(this.scrollBar.bounds.X, this.upButton.bounds.Y + this.upButton.bounds.Height + 4, this.scrollBar.bounds.Width, this.height - 128 /*0x80*/ - this.upButton.bounds.Height - 8);
  }

  /// <summary>Create the clickable texture component for a character's portrait.</summary>
  /// <param name="entry">The social character to render.</param>
  /// <param name="index">The index in the list of entries.</param>
  public ClickableTextureComponent CreateSpriteComponent(AnimalPage.AnimalEntry entry, int index)
  {
    Rectangle bounds = new Rectangle(this.xPositionOnScreen + IClickableMenu.borderWidth + 4, 0, this.width, 64 /*0x40*/);
    Rectangle textureSourceRect = entry.TextureSourceRect;
    if (textureSourceRect.Height <= 16 /*0x10*/)
    {
      --bounds.Height;
      bounds.X += 24;
    }
    return new ClickableTextureComponent(index.ToString(), bounds, (string) null, "", entry.Texture, textureSourceRect, 4f);
  }

  /// <summary>Get the social entry from its index in the list.</summary>
  /// <param name="index">The index in the social list.</param>
  public AnimalPage.AnimalEntry GetSocialEntry(int index)
  {
    if (index < 0 || index >= this.AnimalEntries.Count)
      index = 0;
    return this.AnimalEntries.Count == 0 ? (AnimalPage.AnimalEntry) null : this.AnimalEntries[index];
  }

  public override void snapToDefaultClickableComponent()
  {
    if (this.slotPosition < this.characterSlots.Count)
      this.currentlySnappedComponent = (ClickableComponent) this.characterSlots[this.slotPosition];
    this.snapCursorToCurrentSnappedComponent();
  }

  public void updateSlots()
  {
    for (int index = 0; index < this.characterSlots.Count; ++index)
      this.characterSlots[index].bounds.Y = this.rowPosition(index - 1);
    int num1 = 0;
    for (int slotPosition = this.slotPosition; slotPosition < this.slotPosition + 5; ++slotPosition)
    {
      if (this.slotPosition >= 0 && this.sprites.Count > slotPosition)
      {
        int num2 = this.yPositionOnScreen + IClickableMenu.borderWidth + 32 /*0x20*/ + 112 /*0x70*/ * num1 + 16 /*0x10*/;
        if (this.sprites[slotPosition].bounds.Height < 64 /*0x40*/)
          num2 += 48 /*0x30*/;
        this.sprites[slotPosition].bounds.Y = num2;
      }
      ++num1;
    }
    base.populateClickableComponentList();
    this.addTabsToClickableComponents();
  }

  public void addTabsToClickableComponents()
  {
    if (!(Game1.activeClickableMenu is GameMenu activeClickableMenu) || this.allClickableComponents.Contains(activeClickableMenu.tabs[0]))
      return;
    this.allClickableComponents.AddRange((IEnumerable<ClickableComponent>) activeClickableMenu.tabs);
  }

  protected void _SelectSlot(AnimalPage.AnimalEntry entry)
  {
    bool flag = false;
    for (int index = 0; index < this.AnimalEntries.Count; ++index)
    {
      if (this.AnimalEntries[index].InternalName == entry.InternalName)
      {
        this._SelectSlot((ClickableComponent) this.characterSlots[index]);
        flag = true;
        break;
      }
    }
    if (flag)
      return;
    this._SelectSlot((ClickableComponent) this.characterSlots[0]);
  }

  protected void _SelectSlot(ClickableComponent slot_component)
  {
    if (slot_component == null || !((IEnumerable<ClickableComponent>) this.characterSlots).Contains<ClickableComponent>(slot_component))
      return;
    int num = this.characterSlots.IndexOf(slot_component as ClickableTextureComponent);
    this.currentlySnappedComponent = slot_component;
    if (num < this.slotPosition)
      this.slotPosition = num;
    else if (num >= this.slotPosition + 5)
      this.slotPosition = num - 5 + 1;
    this.setScrollBarToCurrentIndex();
    this.updateSlots();
    if (!Game1.options.snappyMenus || !Game1.options.gamepadControls)
      return;
    this.snapCursorToCurrentSnappedComponent();
  }

  public void ConstrainSelectionToVisibleSlots()
  {
    if (!((IEnumerable<ClickableComponent>) this.characterSlots).Contains<ClickableComponent>(this.currentlySnappedComponent))
      return;
    int index = this.characterSlots.IndexOf(this.currentlySnappedComponent as ClickableTextureComponent);
    if (index < this.slotPosition)
      index = this.slotPosition;
    else if (index >= this.slotPosition + 5)
      index = this.slotPosition + 5 - 1;
    this.currentlySnappedComponent = (ClickableComponent) this.characterSlots[index];
    if (!Game1.options.snappyMenus || !Game1.options.gamepadControls)
      return;
    this.snapCursorToCurrentSnappedComponent();
  }

  public override void snapCursorToCurrentSnappedComponent()
  {
    if (this.currentlySnappedComponent != null && ((IEnumerable<ClickableComponent>) this.characterSlots).Contains<ClickableComponent>(this.currentlySnappedComponent))
      Game1.setMousePosition(this.currentlySnappedComponent.bounds.Left + 64 /*0x40*/, this.currentlySnappedComponent.bounds.Center.Y);
    else
      base.snapCursorToCurrentSnappedComponent();
  }

  public override void applyMovementKey(int direction)
  {
    base.applyMovementKey(direction);
    if (!((IEnumerable<ClickableComponent>) this.characterSlots).Contains<ClickableComponent>(this.currentlySnappedComponent))
      return;
    this._SelectSlot(this.currentlySnappedComponent);
  }

  /// <inheritdoc />
  public override void leftClickHeld(int x, int y)
  {
    base.leftClickHeld(x, y);
    if (!this.scrolling)
      return;
    int y1 = this.scrollBar.bounds.Y;
    this.scrollBar.bounds.Y = Math.Min(this.yPositionOnScreen + this.height - 64 /*0x40*/ - 12 - this.scrollBar.bounds.Height, Math.Max(y, this.yPositionOnScreen + this.upButton.bounds.Height + 20));
    this.slotPosition = Math.Min(this.sprites.Count - 5, Math.Max(0, (int) ((double) this.sprites.Count * (double) ((float) (y - this.scrollBarRunner.Y) / (float) this.scrollBarRunner.Height))));
    this.setScrollBarToCurrentIndex();
    int y2 = this.scrollBar.bounds.Y;
    if (y1 == y2)
      return;
    Game1.playSound("shiny4");
  }

  /// <inheritdoc />
  public override void releaseLeftClick(int x, int y)
  {
    base.releaseLeftClick(x, y);
    this.scrolling = false;
  }

  private void setScrollBarToCurrentIndex()
  {
    if (this.sprites.Count > 0)
    {
      this.scrollBar.bounds.Y = this.scrollBarRunner.Height / Math.Max(1, this.sprites.Count - 5 + 1) * this.slotPosition + this.upButton.bounds.Bottom + 4;
      if (this.slotPosition == this.sprites.Count - 5)
        this.scrollBar.bounds.Y = this.downButton.bounds.Y - this.scrollBar.bounds.Height - 4;
    }
    this.updateSlots();
  }

  /// <inheritdoc />
  public override void receiveScrollWheelAction(int direction)
  {
    base.receiveScrollWheelAction(direction);
    if (direction > 0 && this.slotPosition > 0)
    {
      this.upArrowPressed();
      this.ConstrainSelectionToVisibleSlots();
      Game1.playSound("shiny4");
    }
    else
    {
      if (direction >= 0 || this.slotPosition >= Math.Max(0, this.sprites.Count - 5))
        return;
      this.downArrowPressed();
      this.ConstrainSelectionToVisibleSlots();
      Game1.playSound("shiny4");
    }
  }

  public void upArrowPressed()
  {
    --this.slotPosition;
    this.updateSlots();
    this.upButton.scale = 3.5f;
    this.setScrollBarToCurrentIndex();
  }

  public void downArrowPressed()
  {
    ++this.slotPosition;
    this.updateSlots();
    this.downButton.scale = 3.5f;
    this.setScrollBarToCurrentIndex();
  }

  /// <inheritdoc />
  public override void receiveLeftClick(int x, int y, bool playSound = true)
  {
    if (this.upButton.containsPoint(x, y) && this.slotPosition > 0)
    {
      this.upArrowPressed();
      Game1.playSound("shwip");
    }
    else if (this.downButton.containsPoint(x, y) && this.slotPosition < this.sprites.Count - 5)
    {
      this.downArrowPressed();
      Game1.playSound("shwip");
    }
    else if (this.scrollBar.containsPoint(x, y))
      this.scrolling = true;
    else if (!this.downButton.containsPoint(x, y) && x > this.xPositionOnScreen + this.width && x < this.xPositionOnScreen + this.width + 128 /*0x80*/ && y > this.yPositionOnScreen && y < this.yPositionOnScreen + this.height)
    {
      this.scrolling = true;
      this.leftClickHeld(x, y);
      this.releaseLeftClick(x, y);
    }
    else
    {
      for (int index = 0; index < this.characterSlots.Count; ++index)
      {
        if (index >= this.slotPosition)
        {
          int num = this.slotPosition + 5;
        }
      }
      this.slotPosition = Math.Max(0, Math.Min(this.sprites.Count - 5, this.slotPosition));
    }
  }

  /// <inheritdoc />
  public override void performHoverAction(int x, int y)
  {
    this.hoverText = "";
    this.upButton.tryHover(x, y);
    this.downButton.tryHover(x, y);
  }

  private bool isCharacterSlotClickable(int i)
  {
    this.GetSocialEntry(i);
    return false;
  }

  private void drawNPCSlot(SpriteBatch b, int i)
  {
    AnimalPage.AnimalEntry socialEntry = this.GetSocialEntry(i);
    if (socialEntry == null || i < 0)
      return;
    if ((!this.isCharacterSlotClickable(i) ? 0 : (this.characterSlots[i].bounds.Contains(Game1.getMouseX(), Game1.getMouseY()) ? 1 : 0)) != 0)
      b.Draw(Game1.staminaRect, new Rectangle(this.xPositionOnScreen + IClickableMenu.borderWidth - 4, this.sprites[i].bounds.Y - 4, this.characterSlots[i].bounds.Width, this.characterSlots[i].bounds.Height - 12), Color.White * 0.25f);
    this.sprites[i].draw(b);
    string internalName = socialEntry.InternalName;
    int friendshipLevel = socialEntry.FriendshipLevel;
    float y = Game1.smallFont.MeasureString("W").Y;
    float num1 = LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ru || LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ko ? (float) (-(double) y / 2.0) : 0.0f;
    int num2 = socialEntry.TextureSourceRect.Height <= 16 /*0x10*/ ? -40 : 8;
    b.DrawString(Game1.dialogueFont, socialEntry.DisplayName, new Vector2((float) (this.xPositionOnScreen + IClickableMenu.borderWidth * 3 / 2 + 192 /*0xC0*/ - 20 + 96 /*0x60*/ - (int) ((double) Game1.dialogueFont.MeasureString(socialEntry.DisplayName).X / 2.0)), (float) ((double) (this.sprites[i].bounds.Y + 48 /*0x30*/ + num2) + (double) num1 - 20.0)), Game1.textColor);
    if (socialEntry.FriendshipLevel != -1)
    {
      double num3 = (double) socialEntry.FriendshipLevel / 1000.0;
      int num4 = num3 * 1000.0 % 200.0 >= 100.0 ? (int) (num3 * 1000.0 / 200.0) : -100;
      int num5 = socialEntry.ReceivedAnimalCracker ? -24 : 0;
      for (int index = 0; index < 5; ++index)
      {
        b.Draw(Game1.mouseCursors, new Vector2((float) (this.xPositionOnScreen + 512 /*0x0200*/ - 4 + index * 32 /*0x20*/), (float) (this.sprites[i].bounds.Y + num5 + num2 + 64 /*0x40*/ - 24)), new Rectangle?(new Rectangle(211 + (num3 * 1000.0 <= (double) ((index + 1) * 195) ? 7 : 0), 428, 7, 6)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.89f);
        if (num4 == index)
          b.Draw(Game1.mouseCursors, new Vector2((float) (this.xPositionOnScreen + 512 /*0x0200*/ - 4 + index * 32 /*0x20*/), (float) (this.sprites[i].bounds.Y + num5 + num2 + 64 /*0x40*/ - 24)), new Rectangle?(new Rectangle(211, 428, 4, 6)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.891f);
      }
    }
    if (socialEntry.WasPetYet != -1)
    {
      b.Draw(Game1.mouseCursors, new Vector2((float) (this.xPositionOnScreen + 704 - 4), (float) (this.sprites[i].bounds.Y + num2 + 64 /*0x40*/ - 52)), new Rectangle?(new Rectangle(32 /*0x20*/, 0, 10, 10)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.8f);
      b.Draw(Game1.mouseCursors_1_6, new Vector2((float) (this.xPositionOnScreen + 704 - 4), (float) (this.sprites[i].bounds.Y + num2 + 64 /*0x40*/ - 8)), new Rectangle?(new Rectangle(273 + socialEntry.WasPetYet * 9, 253, 9, 9)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.8f);
    }
    if (socialEntry.special == 1)
      Utility.drawWithShadow(b, Game1.objectSpriteSheet_2, new Vector2((float) (this.xPositionOnScreen + 704 - 16 /*0x10*/), (float) (this.sprites[i].bounds.Y + num2 + 64 /*0x40*/ - 52)), new Rectangle(0, 160 /*0xA0*/, 16 /*0x10*/, 16 /*0x10*/), Color.White, 0.0f, Vector2.Zero, 4f, layerDepth: 0.8f, horizontalShadowOffset: 0, verticalShadowOffset: 8);
    if (!socialEntry.ReceivedAnimalCracker)
      return;
    Utility.drawWithShadow(b, Game1.objectSpriteSheet_2, new Vector2((float) (this.xPositionOnScreen + 576 - 20), (float) (this.sprites[i].bounds.Y + num2 + 64 /*0x40*/ - 16 /*0x10*/)), new Rectangle(16 /*0x10*/, 242, 15, 11), Color.White, 0.0f, Vector2.Zero, 4f, layerDepth: 0.8f);
  }

  private int rowPosition(int i)
  {
    int num1 = i - this.slotPosition;
    int num2 = 112 /*0x70*/;
    return this.yPositionOnScreen + IClickableMenu.borderWidth + 160 /*0xA0*/ + 4 + num1 * num2;
  }

  /// <inheritdoc />
  public override void draw(SpriteBatch b)
  {
    b.End();
    b.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, rasterizerState: Utility.ScissorEnabled);
    if (this.sprites.Count > 0)
      this.drawHorizontalPartition(b, this.yPositionOnScreen + IClickableMenu.borderWidth + 128 /*0x80*/ + 4, true);
    if (this.sprites.Count > 1)
      this.drawHorizontalPartition(b, this.yPositionOnScreen + IClickableMenu.borderWidth + 192 /*0xC0*/ + 32 /*0x20*/ + 20, true);
    if (this.sprites.Count > 2)
      this.drawHorizontalPartition(b, this.yPositionOnScreen + IClickableMenu.borderWidth + 320 + 36, true);
    if (this.sprites.Count > 3)
      this.drawHorizontalPartition(b, this.yPositionOnScreen + IClickableMenu.borderWidth + 384 + 32 /*0x20*/ + 52, true);
    for (int slotPosition = this.slotPosition; slotPosition < this.slotPosition + 5 && slotPosition < this.sprites.Count; ++slotPosition)
    {
      if (this.GetSocialEntry(slotPosition) != null)
        this.drawNPCSlot(b, slotPosition);
    }
    Rectangle scissorRectangle = b.GraphicsDevice.ScissorRectangle with
    {
      Y = Math.Max(0, this.rowPosition(4 - this.sprites.Count))
    };
    scissorRectangle.Height -= scissorRectangle.Y;
    if (scissorRectangle.Height > 0)
    {
      int heightOverride = this.sprites.Count >= 5 ? -1 : (108 + this.sprites.Count) * this.sprites.Count;
      this.drawVerticalPartition(b, this.xPositionOnScreen + 448 + 12, true, heightOverride: heightOverride);
      this.drawVerticalPartition(b, this.xPositionOnScreen + 256 /*0x0100*/ + 12 + 376, true, heightOverride: heightOverride);
    }
    this.upButton.draw(b);
    this.downButton.draw(b);
    IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(403, 383, 6, 6), this.scrollBarRunner.X, this.scrollBarRunner.Y, this.scrollBarRunner.Width, this.scrollBarRunner.Height, Color.White, 4f);
    this.scrollBar.draw(b);
    if (!this.hoverText.Equals(""))
      IClickableMenu.drawHoverText(b, this.hoverText, Game1.smallFont);
    b.End();
    b.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
  }

  /// <summary>An entry on the social page.</summary>
  public class AnimalEntry
  {
    /// <summary>The character instance.</summary>
    public Character Animal;
    /// <summary>The unique multiplayer ID for a player, or the internal name for an NPC.</summary>
    public readonly string InternalName;
    /// <summary>The translated display name.</summary>
    public readonly string DisplayName;
    public readonly string AnimalType;
    public readonly string AnimalBaseType;
    /// <summary>The current player's heart level with this animal. -1 means friendship is not tracked.</summary>
    public readonly int FriendshipLevel = -1;
    public readonly bool ReceivedAnimalCracker;
    /// <summary>0 is no, 1 is auto-pet, 2 is hand pet</summary>
    public readonly int WasPetYet;
    public readonly int special;
    public Texture2D Texture;
    public Rectangle TextureSourceRect;

    /// <summary>Construct an instance.</summary>
    /// <param name="player">The player for which to create an entry.</param>
    /// <param name="friendship">The current player's friendship with this character.</param>
    public AnimalEntry(Character animal)
    {
      this.Animal = animal;
      this.DisplayName = animal.displayName;
      switch (animal)
      {
        case FarmAnimal farmAnimal:
          this.InternalName = farmAnimal.myID?.ToString() ?? "";
          this.FriendshipLevel = farmAnimal.friendshipTowardFarmer.Value;
          this.Texture = farmAnimal.Sprite.Texture;
          this.TextureSourceRect = farmAnimal.Sprite.SourceRect.Height <= 16 /*0x10*/ ? new Rectangle(0, 16 /*0x10*/, 16 /*0x10*/, 16 /*0x10*/) : (!farmAnimal.type.Equals((object) "Ostrich") ? new Rectangle(0, farmAnimal.Sprite.SourceRect.Height * 2 - 28, farmAnimal.Sprite.SourceRect.Width, 28) : new Rectangle(0, farmAnimal.Sprite.SourceRect.Height * 2 - 32 /*0x20*/, farmAnimal.Sprite.SourceRect.Width, 28));
          this.AnimalType = farmAnimal.type.Value;
          this.AnimalBaseType = !this.AnimalType.Contains(' ') ? this.AnimalType : this.AnimalType.Split(' ')[1];
          this.WasPetYet = farmAnimal.wasPet.Value ? 2 : (farmAnimal.wasAutoPet.Value ? 1 : 0);
          this.ReceivedAnimalCracker = farmAnimal.hasEatenAnimalCracker.Value;
          break;
        case Pet pet:
          this.InternalName = pet.petId?.ToString() ?? "";
          this.FriendshipLevel = pet.friendshipTowardFarmer.Value;
          this.Texture = pet.Sprite.Texture;
          this.TextureSourceRect = new Rectangle(0, pet.Sprite.SourceRect.Height * 2 - 24, pet.Sprite.SourceRect.Width, 24);
          this.AnimalType = pet.petType.Value;
          this.WasPetYet = pet.grantedFriendshipForPet.Value ? 2 : 0;
          break;
        case Horse horse:
          this.InternalName = horse.HorseId.ToString();
          this.Texture = horse.Sprite.Texture;
          this.TextureSourceRect = new Rectangle(0, horse.Sprite.SourceRect.Height * 2 - 26, horse.Sprite.SourceRect.Width, 24);
          this.AnimalType = "Horse";
          this.WasPetYet = -1;
          this.special = horse.ateCarrotToday ? 1 : 0;
          break;
      }
    }
  }
}
