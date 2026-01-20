// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.ProfileMenu
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley.BellsAndWhistles;
using StardewValley.Extensions;
using StardewValley.GameData.Characters;
using StardewValley.ItemTypeDefinitions;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace StardewValley.Menus;

public class ProfileMenu : IClickableMenu
{
  public const int region_characterSelectors = 500;
  public const int region_categorySelector = 501;
  public const int region_itemButtons = 502;
  public const int region_backButton = 101;
  public const int region_forwardButton = 102;
  public const int region_upArrow = 105;
  public const int region_downArrow = 106;
  public const int letterWidth = 320;
  public const int letterHeight = 180;
  public Texture2D letterTexture;
  protected string hoverText = "";
  protected List<ProfileItem> _profileItems;
  public Item hoveredItem;
  public ClickableTextureComponent backButton;
  public ClickableTextureComponent forwardButton;
  public ClickableTextureComponent nextCharacterButton;
  public ClickableTextureComponent previousCharacterButton;
  protected Rectangle characterSpriteBox;
  protected int _currentCategory;
  protected AnimatedSprite _animatedSprite;
  protected float _directionChangeTimer;
  protected float _hiddenEmoteTimer = -1f;
  protected int _currentDirection;
  protected int _hideTooltipTime;
  protected SocialPage _socialPage;
  protected string _status = "";
  protected string _printedName = "";
  protected Vector2 _characterEntrancePosition = new Vector2(0.0f, 0.0f);
  public ClickableTextureComponent upArrow;
  public ClickableTextureComponent downArrow;
  protected ClickableTextureComponent scrollBar;
  protected Rectangle scrollBarRunner;
  public List<ClickableComponent> clickableProfileItems;
  /// <summary>The current character being shown in the menu.</summary>
  public SocialPage.SocialEntry Current;
  /// <summary>The social entries for characters that can be viewed in the profile menu.</summary>
  public readonly List<SocialPage.SocialEntry> SocialEntries = new List<SocialPage.SocialEntry>();
  protected Vector2 _characterNamePosition;
  protected Vector2 _heartDisplayPosition;
  protected Vector2 _birthdayHeadingDisplayPosition;
  protected Vector2 _birthdayDisplayPosition;
  protected Vector2 _statusHeadingDisplayPosition;
  protected Vector2 _statusDisplayPosition;
  protected Vector2 _giftLogHeadingDisplayPosition;
  protected Vector2 _giftLogCategoryDisplayPosition;
  protected Vector2 _errorMessagePosition;
  protected Vector2 _characterSpriteDrawPosition;
  protected Rectangle _characterStatusDisplayBox;
  protected List<ClickableTextureComponent> _clickableTextureComponents;
  public Rectangle _itemDisplayRect;
  protected int scrollPosition;
  protected int scrollStep = 36;
  protected int scrollSize;
  public static ProfileMenu.ProfileItemCategory[] itemCategories = new ProfileMenu.ProfileItemCategory[10]
  {
    new ProfileMenu.ProfileItemCategory("Profile_Gift_Category_LikedGifts", (int[]) null),
    new ProfileMenu.ProfileItemCategory("Profile_Gift_Category_FruitsAndVegetables", new int[2]
    {
      -75,
      -79
    }),
    new ProfileMenu.ProfileItemCategory("Profile_Gift_Category_AnimalProduce", new int[4]
    {
      -6,
      -5,
      -14,
      -18
    }),
    new ProfileMenu.ProfileItemCategory("Profile_Gift_Category_ArtisanItems", new int[1]
    {
      -26
    }),
    new ProfileMenu.ProfileItemCategory("Profile_Gift_Category_CookedItems", new int[1]
    {
      -7
    }),
    new ProfileMenu.ProfileItemCategory("Profile_Gift_Category_ForagedItems", new int[4]
    {
      -80,
      -81,
      -23,
      -17
    }),
    new ProfileMenu.ProfileItemCategory("Profile_Gift_Category_Fish", new int[1]
    {
      -4
    }),
    new ProfileMenu.ProfileItemCategory("Profile_Gift_Category_Ingredients", new int[2]
    {
      -27,
      -25
    }),
    new ProfileMenu.ProfileItemCategory("Profile_Gift_Category_MineralsAndGems", new int[3]
    {
      -15,
      -12,
      -2
    }),
    new ProfileMenu.ProfileItemCategory("Profile_Gift_Category_Misc", (int[]) null)
  };
  protected Dictionary<int, List<Item>> _sortedItems;
  public bool scrolling;
  private int _characterSpriteRandomInt;

  public ProfileMenu(SocialPage.SocialEntry subject, List<SocialPage.SocialEntry> allSocialEntries)
    : base((int) Utility.getTopLeftPositionForCenteringOnScreen(1280 /*0x0500*/, 720).X, (int) Utility.getTopLeftPositionForCenteringOnScreen(1280 /*0x0500*/, 720).Y, 1280 /*0x0500*/, 720, true)
  {
    this._printedName = "";
    this._characterEntrancePosition = new Vector2(0.0f, 4f);
    foreach (SocialPage.SocialEntry allSocialEntry in allSocialEntries)
    {
      if (allSocialEntry.Character is NPC && allSocialEntry.IsMet)
        this.SocialEntries.Add(allSocialEntry);
    }
    this._profileItems = new List<ProfileItem>();
    this.clickableProfileItems = new List<ClickableComponent>();
    this.UpdateButtons();
    this.letterTexture = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\letterBG");
    this._SetCharacter(subject);
  }

  protected void _SetCharacter(SocialPage.SocialEntry entry)
  {
    this.Current = entry;
    this._sortedItems = new Dictionary<int, List<Item>>();
    if (this.Current.Character is NPC character)
    {
      CharacterData data = character.GetData();
      string textureName = "Characters/" + character.getTextureName();
      try
      {
        this._animatedSprite = new AnimatedSprite(textureName, 0, data != null ? data.Size.X : 16 /*0x10*/, data != null ? data.Size.Y : 32 /*0x20*/);
      }
      catch (Exception ex)
      {
        Game1.log.Error($"Profile menu couldn't load sprite '{textureName}' for NPC '{character.Name}', defaulting to their current sprite.", ex);
        this._animatedSprite = character.Sprite.Clone();
        this._animatedSprite.tempSpriteHeight = -1;
        this._animatedSprite.SpriteWidth = data != null ? data.Size.X : this._animatedSprite.SpriteWidth;
        this._animatedSprite.SpriteHeight = data != null ? data.Size.Y : this._animatedSprite.SpriteHeight;
      }
      this._animatedSprite.faceDirection(2);
      foreach (ParsedItemData parsedItemData in ItemRegistry.GetObjectTypeDefinition().GetAllData())
      {
        if (Game1.player.hasGiftTasteBeenRevealed(character, parsedItemData.ItemId))
        {
          StardewValley.Object @object = ItemRegistry.Create<StardewValley.Object>(parsedItemData.QualifiedItemId);
          if (!@object.IsBreakableStone())
          {
            for (int key = 0; key < ProfileMenu.itemCategories.Length; ++key)
            {
              switch (ProfileMenu.itemCategories[key].categoryName)
              {
                case "Profile_Gift_Category_LikedGifts":
                  switch (character.getGiftTasteForThisItem((Item) @object))
                  {
                    case 0:
                    case 2:
                      List<Item> objList1;
                      if (!this._sortedItems.TryGetValue(key, out objList1))
                        this._sortedItems[key] = objList1 = new List<Item>();
                      objList1.Add((Item) @object);
                      continue;
                    default:
                      continue;
                  }
                case "Profile_Gift_Category_Misc":
                  bool flag = false;
                  foreach (ProfileMenu.ProfileItemCategory itemCategory in ProfileMenu.itemCategories)
                  {
                    if (itemCategory.validCategories != null && ((IEnumerable<int>) itemCategory.validCategories).Contains<int>(@object.Category))
                    {
                      flag = true;
                      break;
                    }
                  }
                  if (!flag)
                  {
                    List<Item> objList2;
                    if (!this._sortedItems.TryGetValue(key, out objList2))
                      this._sortedItems[key] = objList2 = new List<Item>();
                    objList2.Add((Item) @object);
                    break;
                  }
                  break;
                default:
                  if (((IEnumerable<int>) ProfileMenu.itemCategories[key].validCategories).Contains<int>(@object.Category))
                  {
                    List<Item> objList3;
                    if (!this._sortedItems.TryGetValue(key, out objList3))
                      this._sortedItems[key] = objList3 = new List<Item>();
                    objList3.Add((Item) @object);
                    break;
                  }
                  break;
              }
            }
          }
        }
      }
      Gender gender = this.Current.Gender;
      int num1 = this.Current.IsDatable ? 1 : 0;
      bool flag1 = this.Current.IsRoommateForCurrentPlayer();
      this._status = "";
      int num2 = flag1 ? 1 : 0;
      if ((num1 | num2) != 0)
        this._status = Utility.capitalizeFirstLetter(Game1.parseText(!flag1 ? (!this.Current.IsMarriedToCurrentPlayer() ? (!this.Current.IsMarriedToAnyone() ? (Game1.player.isMarriedOrRoommates() || !this.Current.IsDatingCurrentPlayer() ? (!this.Current.IsDivorcedFromCurrentPlayer() ? (gender == Gender.Female ? Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage_Relationship_Single_Female") : Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage_Relationship_Single_Male")) : (gender == Gender.Female ? Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage_Relationship_ExWife") : Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage_Relationship_ExHusband"))) : (gender == Gender.Female ? Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage_Relationship_Girlfriend") : Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage_Relationship_Boyfriend"))) : (gender == Gender.Female ? Game1.content.LoadString("Strings\\UI:SocialPage_Relationship_MarriedToOtherPlayer_FemaleNpc") : Game1.content.LoadString("Strings\\UI:SocialPage_Relationship_MarriedToOtherPlayer_MaleNpc"))) : (gender == Gender.Female ? Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage_Relationship_Wife") : Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage_Relationship_Husband"))) : (gender == Gender.Female ? Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage_Relationship_Housemate_Female") : Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage_Relationship_Housemate_Male")), Game1.smallFont, this.width).Replace("(", "").Replace(")", "").Replace("（", "").Replace("）", ""));
      this._UpdateList();
    }
    this._directionChangeTimer = 2000f;
    this._currentDirection = 2;
    this._hiddenEmoteTimer = -1f;
  }

  public void ChangeCharacter(int offset)
  {
    int num = this.SocialEntries.IndexOf(this.Current);
    if (num == -1)
    {
      if (this.SocialEntries.Count <= 0)
        return;
      this._SetCharacter(this.SocialEntries[0]);
    }
    else
    {
      int index = num + offset;
      while (index < 0)
        index += this.SocialEntries.Count;
      while (index >= this.SocialEntries.Count)
        index -= this.SocialEntries.Count;
      this._SetCharacter(this.SocialEntries[index]);
      Game1.playSound("smallSelect");
      this._printedName = "";
      this._characterEntrancePosition = new Vector2((float) (Math.Sign(offset) * -4), 0.0f);
      if (!Game1.options.SnappyMenus || this.currentlySnappedComponent != null && this.currentlySnappedComponent.visible)
        return;
      this.snapToDefaultClickableComponent();
    }
  }

  protected void _UpdateList()
  {
    for (int index = 0; index < this._profileItems.Count; ++index)
      this._profileItems[index].Unload();
    this._profileItems.Clear();
    if (!(this.Current.Character is NPC character))
      return;
    List<Item> values1 = new List<Item>();
    List<Item> values2 = new List<Item>();
    List<Item> values3 = new List<Item>();
    List<Item> values4 = new List<Item>();
    List<Item> values5 = new List<Item>();
    List<Item> objList;
    if (this._sortedItems.TryGetValue(this._currentCategory, out objList))
    {
      foreach (Item obj in objList)
      {
        switch (character.getGiftTasteForThisItem(obj))
        {
          case 0:
            values1.Add(obj);
            continue;
          case 2:
            values2.Add(obj);
            continue;
          case 4:
            values4.Add(obj);
            continue;
          case 6:
            values5.Add(obj);
            continue;
          case 8:
            values3.Add(obj);
            continue;
          default:
            continue;
        }
      }
    }
    this._profileItems.Add((ProfileItem) new PI_ItemList(this, Game1.content.LoadString("Strings\\UI:Profile_Gift_Loved"), values1));
    this._profileItems.Add((ProfileItem) new PI_ItemList(this, Game1.content.LoadString("Strings\\UI:Profile_Gift_Liked"), values2));
    this._profileItems.Add((ProfileItem) new PI_ItemList(this, Game1.content.LoadString("Strings\\UI:Profile_Gift_Neutral"), values3));
    this._profileItems.Add((ProfileItem) new PI_ItemList(this, Game1.content.LoadString("Strings\\UI:Profile_Gift_Disliked"), values4));
    this._profileItems.Add((ProfileItem) new PI_ItemList(this, Game1.content.LoadString("Strings\\UI:Profile_Gift_Hated"), values5));
    this.SetupLayout();
    this.populateClickableComponentList();
    if (!Game1.options.snappyMenus || !Game1.options.gamepadControls || this.currentlySnappedComponent != null && this.allClickableComponents.Contains(this.currentlySnappedComponent))
      return;
    this.snapToDefaultClickableComponent();
  }

  public override bool IsAutomaticSnapValid(
    int direction,
    ClickableComponent a,
    ClickableComponent b)
  {
    return (direction != 2 || a.region != 501 || b.region != 500) && base.IsAutomaticSnapValid(direction, a, b);
  }

  public override void snapToDefaultClickableComponent()
  {
    if (this.clickableProfileItems.Count > 0)
      this.currentlySnappedComponent = this.clickableProfileItems[0];
    else
      this.currentlySnappedComponent = (ClickableComponent) this.backButton;
    this.snapCursorToCurrentSnappedComponent();
  }

  public void UpdateButtons()
  {
    this._clickableTextureComponents = new List<ClickableTextureComponent>();
    ClickableTextureComponent textureComponent1 = new ClickableTextureComponent(new Rectangle(0, 0, 44, 48 /*0x30*/), Game1.mouseCursors, new Rectangle(421, 459, 11, 12), 4f);
    textureComponent1.myID = 105;
    textureComponent1.upNeighborID = 102;
    textureComponent1.upNeighborImmutable = true;
    textureComponent1.downNeighborID = 106;
    textureComponent1.downNeighborImmutable = true;
    textureComponent1.leftNeighborID = -99998;
    textureComponent1.leftNeighborImmutable = true;
    this.upArrow = textureComponent1;
    ClickableTextureComponent textureComponent2 = new ClickableTextureComponent(new Rectangle(0, 0, 44, 48 /*0x30*/), Game1.mouseCursors, new Rectangle(421, 472, 11, 12), 4f);
    textureComponent2.myID = 106;
    textureComponent2.upNeighborID = 105;
    textureComponent2.upNeighborImmutable = true;
    textureComponent2.leftNeighborID = -99998;
    textureComponent2.leftNeighborImmutable = true;
    this.downArrow = textureComponent2;
    this.scrollBar = new ClickableTextureComponent(new Rectangle(0, 0, 24, 40), Game1.mouseCursors, new Rectangle(435, 463, 6, 10), 4f);
    ClickableTextureComponent textureComponent3 = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + 32 /*0x20*/, this.yPositionOnScreen + this.height - 32 /*0x20*/ - 64 /*0x40*/, 48 /*0x30*/, 44), Game1.mouseCursors, new Rectangle(352, 495, 12, 11), 4f);
    textureComponent3.myID = 101;
    textureComponent3.name = "Back Button";
    textureComponent3.upNeighborID = -99998;
    textureComponent3.downNeighborID = -99998;
    textureComponent3.downNeighborImmutable = true;
    textureComponent3.leftNeighborID = -99998;
    textureComponent3.rightNeighborID = -99998;
    textureComponent3.region = 501;
    this.backButton = textureComponent3;
    this._clickableTextureComponents.Add(this.backButton);
    ClickableTextureComponent textureComponent4 = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + this.width - 32 /*0x20*/ - 48 /*0x30*/, this.yPositionOnScreen + this.height - 32 /*0x20*/ - 64 /*0x40*/, 48 /*0x30*/, 44), Game1.mouseCursors, new Rectangle(365, 495, 12, 11), 4f);
    textureComponent4.myID = 102;
    textureComponent4.name = "Forward Button";
    textureComponent4.upNeighborID = -99998;
    textureComponent4.downNeighborID = -99998;
    textureComponent4.downNeighborImmutable = true;
    textureComponent4.leftNeighborID = -99998;
    textureComponent4.rightNeighborID = -99998;
    textureComponent4.region = 501;
    this.forwardButton = textureComponent4;
    this._clickableTextureComponents.Add(this.forwardButton);
    ClickableTextureComponent textureComponent5 = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + 32 /*0x20*/, this.yPositionOnScreen + this.height - 32 /*0x20*/ - 64 /*0x40*/, 48 /*0x30*/, 44), Game1.mouseCursors, new Rectangle(352, 495, 12, 11), 4f);
    textureComponent5.myID = 0;
    textureComponent5.name = "Previous Char";
    textureComponent5.upNeighborID = -99998;
    textureComponent5.downNeighborID = -99998;
    textureComponent5.leftNeighborID = -99998;
    textureComponent5.rightNeighborID = -99998;
    textureComponent5.region = 500;
    this.previousCharacterButton = textureComponent5;
    this._clickableTextureComponents.Add(this.previousCharacterButton);
    ClickableTextureComponent textureComponent6 = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + this.width - 32 /*0x20*/ - 48 /*0x30*/, this.yPositionOnScreen + this.height - 32 /*0x20*/ - 64 /*0x40*/, 48 /*0x30*/, 44), Game1.mouseCursors, new Rectangle(365, 495, 12, 11), 4f);
    textureComponent6.myID = 0;
    textureComponent6.name = "Next Char";
    textureComponent6.upNeighborID = -99998;
    textureComponent6.downNeighborID = -99998;
    textureComponent6.leftNeighborID = -99998;
    textureComponent6.rightNeighborID = -99998;
    textureComponent6.region = 500;
    this.nextCharacterButton = textureComponent6;
    this._clickableTextureComponents.Add(this.nextCharacterButton);
    this._clickableTextureComponents.Add(this.upArrow);
    this._clickableTextureComponents.Add(this.downArrow);
  }

  /// <inheritdoc />
  public override void receiveScrollWheelAction(int direction)
  {
    base.receiveScrollWheelAction(direction);
    if (direction > 0)
    {
      this.Scroll(-this.scrollStep);
    }
    else
    {
      if (direction >= 0)
        return;
      this.Scroll(this.scrollStep);
    }
  }

  public void ChangePage(int offset)
  {
    this.scrollPosition = 0;
    this._currentCategory += offset;
    while (this._currentCategory < 0)
      this._currentCategory += ProfileMenu.itemCategories.Length;
    while (this._currentCategory >= ProfileMenu.itemCategories.Length)
      this._currentCategory -= ProfileMenu.itemCategories.Length;
    Game1.playSound("shwip");
    this._UpdateList();
    if (!Game1.options.SnappyMenus || this.currentlySnappedComponent != null && this.currentlySnappedComponent.visible)
      return;
    this.snapToDefaultClickableComponent();
  }

  public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
  {
    this.xPositionOnScreen = (int) Utility.getTopLeftPositionForCenteringOnScreen(1280 /*0x0500*/, 720).X;
    this.yPositionOnScreen = (int) Utility.getTopLeftPositionForCenteringOnScreen(1280 /*0x0500*/, 720).Y;
    this.UpdateButtons();
    this.SetupLayout();
    this.initializeUpperRightCloseButton();
    this.populateClickableComponentList();
  }

  /// <inheritdoc />
  public override void receiveGamePadButton(Buttons button)
  {
    base.receiveGamePadButton(button);
    switch (button)
    {
      case Buttons.Back:
        this.PlayHiddenEmote();
        break;
      case Buttons.LeftShoulder:
        this.ChangeCharacter(-1);
        break;
      case Buttons.RightShoulder:
        this.ChangeCharacter(1);
        break;
      case Buttons.RightTrigger:
        this.ChangePage(1);
        break;
      case Buttons.LeftTrigger:
        this.ChangePage(-1);
        break;
    }
  }

  /// <inheritdoc />
  public override void receiveKeyPress(Keys key)
  {
    if (key == Keys.None)
      return;
    if (Game1.options.doesInputListContain(Game1.options.menuButton, key) && this.readyToClose())
    {
      this.exitThisMenu();
    }
    else
    {
      if (!Game1.options.snappyMenus || !Game1.options.gamepadControls || this.overrideSnappyMenuCursorMovementBan())
        return;
      this.applyMovementKey(key);
    }
  }

  public override void applyMovementKey(int direction)
  {
    base.applyMovementKey(direction);
    this.ConstrainSelectionToView();
  }

  /// <inheritdoc />
  public override void releaseLeftClick(int x, int y)
  {
    base.releaseLeftClick(x, y);
    this.scrolling = false;
  }

  /// <inheritdoc />
  public override void receiveLeftClick(int x, int y, bool playSound = true)
  {
    if (this.scrollBar.containsPoint(x, y))
      this.scrolling = true;
    else if (this.scrollBarRunner.Contains(x, y))
    {
      this.scrolling = true;
      this.leftClickHeld(x, y);
      this.releaseLeftClick(x, y);
    }
    if (this.upperRightCloseButton != null && this.readyToClose() && this.upperRightCloseButton.containsPoint(x, y))
    {
      this.exitThisMenu();
    }
    else
    {
      if (Game1.activeClickableMenu == null && Game1.currentMinigame == null)
        return;
      if (this.backButton.containsPoint(x, y))
        this.ChangePage(-1);
      else if (this.forwardButton.containsPoint(x, y))
        this.ChangePage(1);
      else if (this.previousCharacterButton.containsPoint(x, y))
        this.ChangeCharacter(-1);
      else if (this.nextCharacterButton.containsPoint(x, y))
      {
        this.ChangeCharacter(1);
      }
      else
      {
        if (this.downArrow.containsPoint(x, y))
          this.Scroll(this.scrollStep);
        if (this.upArrow.containsPoint(x, y))
          this.Scroll(-this.scrollStep);
        if (!this.characterSpriteBox.Contains(x, y))
          return;
        this.PlayHiddenEmote();
      }
    }
  }

  public void PlayHiddenEmote()
  {
    if (this.Current.HeartLevel >= 4)
    {
      this._currentDirection = 2;
      this._characterSpriteRandomInt = Game1.random.Next(4);
      CharacterData data = this.Current.Data;
      Game1.playSound(data?.HiddenProfileEmoteSound ?? "drumkit6");
      this._hiddenEmoteTimer = data == null || data.HiddenProfileEmoteDuration < 0 ? 4000f : (float) data.HiddenProfileEmoteDuration;
    }
    else
    {
      this._currentDirection = 2;
      this._directionChangeTimer = 5000f;
      Game1.playSound("Cowboy_Footstep");
    }
  }

  /// <inheritdoc />
  public override void performHoverAction(int x, int y)
  {
    base.performHoverAction(x, y);
    this.hoveredItem = (Item) null;
    if (this._itemDisplayRect.Contains(x, y))
    {
      foreach (ProfileItem profileItem in this._profileItems)
        profileItem.performHover(x, y);
    }
    this.upArrow.tryHover(x, y);
    this.downArrow.tryHover(x, y);
    this.backButton.tryHover(x, y, 0.6f);
    this.forwardButton.tryHover(x, y, 0.6f);
    this.nextCharacterButton.tryHover(x, y, 0.6f);
    this.previousCharacterButton.tryHover(x, y, 0.6f);
  }

  public void ConstrainSelectionToView()
  {
    if (!Game1.options.snappyMenus)
      return;
    ClickableComponent snappedComponent = this.currentlySnappedComponent;
    if ((snappedComponent != null ? (snappedComponent.region == 502 ? 1 : 0) : 0) != 0 && !this._itemDisplayRect.Contains(this.currentlySnappedComponent.bounds))
    {
      if (this.currentlySnappedComponent.bounds.Bottom > this._itemDisplayRect.Bottom)
        this.Scroll((int) Math.Ceiling(((double) this.currentlySnappedComponent.bounds.Bottom - (double) this._itemDisplayRect.Bottom) / (double) this.scrollStep) * this.scrollStep);
      else if (this.currentlySnappedComponent.bounds.Top < this._itemDisplayRect.Top)
        this.Scroll((int) Math.Floor(((double) this.currentlySnappedComponent.bounds.Top - (double) this._itemDisplayRect.Top) / (double) this.scrollStep) * this.scrollStep);
    }
    if (this.scrollPosition > this.scrollStep)
      return;
    this.scrollPosition = 0;
    this.UpdateScroll();
  }

  /// <inheritdoc />
  public override void update(GameTime time)
  {
    base.update(time);
    if (this.Current.DisplayName != null && this._printedName.Length < this.Current.DisplayName.Length)
      this._printedName += this.Current.DisplayName[this._printedName.Length].ToString();
    TimeSpan elapsedGameTime;
    if (this._hideTooltipTime > 0)
    {
      int hideTooltipTime = this._hideTooltipTime;
      elapsedGameTime = time.ElapsedGameTime;
      int milliseconds = elapsedGameTime.Milliseconds;
      this._hideTooltipTime = hideTooltipTime - milliseconds;
      if (this._hideTooltipTime < 0)
        this._hideTooltipTime = 0;
    }
    if ((double) this._characterEntrancePosition.X != 0.0)
      this._characterEntrancePosition.X -= (float) Math.Sign(this._characterEntrancePosition.X) * 0.25f;
    if ((double) this._characterEntrancePosition.Y != 0.0)
      this._characterEntrancePosition.Y -= (float) Math.Sign(this._characterEntrancePosition.Y) * 0.25f;
    if (this._animatedSprite == null)
      return;
    if ((double) this._hiddenEmoteTimer > 0.0)
    {
      double hiddenEmoteTimer = (double) this._hiddenEmoteTimer;
      elapsedGameTime = time.ElapsedGameTime;
      double milliseconds = (double) elapsedGameTime.Milliseconds;
      this._hiddenEmoteTimer = (float) (hiddenEmoteTimer - milliseconds);
      if ((double) this._hiddenEmoteTimer <= 0.0)
      {
        this._hiddenEmoteTimer = -1f;
        this._currentDirection = 2;
        this._directionChangeTimer = 2000f;
        if (this.Current.InternalName == "Leo")
          this.Current.Character.Sprite.AnimateDown(time);
      }
    }
    else if ((double) this._directionChangeTimer > 0.0)
    {
      double directionChangeTimer = (double) this._directionChangeTimer;
      elapsedGameTime = time.ElapsedGameTime;
      double milliseconds = (double) elapsedGameTime.Milliseconds;
      this._directionChangeTimer = (float) (directionChangeTimer - milliseconds);
      if ((double) this._directionChangeTimer <= 0.0)
      {
        this._directionChangeTimer = 2000f;
        this._currentDirection = (this._currentDirection + 1) % 4;
      }
    }
    if (this._characterEntrancePosition != Vector2.Zero)
    {
      if ((double) this._characterEntrancePosition.X < 0.0)
        this._animatedSprite.AnimateRight(time, 2);
      else if ((double) this._characterEntrancePosition.X > 0.0)
        this._animatedSprite.AnimateLeft(time, 2);
      else if ((double) this._characterEntrancePosition.Y > 0.0)
      {
        this._animatedSprite.AnimateUp(time, 2);
      }
      else
      {
        if ((double) this._characterEntrancePosition.Y >= 0.0)
          return;
        this._animatedSprite.AnimateDown(time, 2);
      }
    }
    else if ((double) this._hiddenEmoteTimer > 0.0)
    {
      CharacterData data = this.Current.Data;
      if (data != null && data.HiddenProfileEmoteStartFrame >= 0)
      {
        int startFrame = !(this.Current.InternalName == "Emily") || data.HiddenProfileEmoteStartFrame != 16 /*0x10*/ ? data.HiddenProfileEmoteStartFrame : data.HiddenProfileEmoteStartFrame + this._characterSpriteRandomInt * 2;
        this._animatedSprite.Animate(time, startFrame, data.HiddenProfileEmoteFrameCount, data.HiddenProfileEmoteFrameDuration);
      }
      else
        this._animatedSprite.AnimateDown(time, 2);
    }
    else
    {
      switch (this._currentDirection)
      {
        case 0:
          this._animatedSprite.AnimateUp(time, 2);
          break;
        case 1:
          this._animatedSprite.AnimateRight(time, 2);
          break;
        case 2:
          this._animatedSprite.AnimateDown(time, 2);
          break;
        case 3:
          this._animatedSprite.AnimateLeft(time, 2);
          break;
      }
    }
  }

  public void SetupLayout()
  {
    int x = this.xPositionOnScreen + 64 /*0x40*/ - 12;
    int y = this.yPositionOnScreen + IClickableMenu.borderWidth;
    Rectangle rectangle1 = new Rectangle(x, y, 400, 720 - IClickableMenu.borderWidth * 2);
    Rectangle rectangle2 = new Rectangle(x, y, 1204, 720 - IClickableMenu.borderWidth * 2);
    rectangle2.X += rectangle1.Width;
    rectangle2.Width -= rectangle1.Width;
    this._characterStatusDisplayBox = new Rectangle(rectangle1.X, rectangle1.Y, rectangle1.Width, rectangle1.Height);
    rectangle1.Y += 32 /*0x20*/;
    rectangle1.Height -= 32 /*0x20*/;
    this._characterSpriteDrawPosition = new Vector2((float) (rectangle1.X + (rectangle1.Width - Game1.nightbg.Width) / 2), (float) rectangle1.Y);
    this.characterSpriteBox = new Rectangle(this.xPositionOnScreen + 64 /*0x40*/ - 12 + (400 - Game1.nightbg.Width) / 2, this.yPositionOnScreen + IClickableMenu.borderWidth, Game1.nightbg.Width, Game1.nightbg.Height);
    this.previousCharacterButton.bounds.X = (int) this._characterSpriteDrawPosition.X - 64 /*0x40*/ - this.previousCharacterButton.bounds.Width / 2;
    this.previousCharacterButton.bounds.Y = (int) this._characterSpriteDrawPosition.Y + Game1.nightbg.Height / 2 - this.previousCharacterButton.bounds.Height / 2;
    this.nextCharacterButton.bounds.X = (int) this._characterSpriteDrawPosition.X + Game1.nightbg.Width + 64 /*0x40*/ - this.nextCharacterButton.bounds.Width / 2;
    this.nextCharacterButton.bounds.Y = (int) this._characterSpriteDrawPosition.Y + Game1.nightbg.Height / 2 - this.nextCharacterButton.bounds.Height / 2;
    rectangle1.Y += Game1.daybg.Height + 32 /*0x20*/;
    rectangle1.Height -= Game1.daybg.Height + 32 /*0x20*/;
    this._characterNamePosition = new Vector2((float) rectangle1.Center.X, (float) rectangle1.Top);
    rectangle1.Y += 96 /*0x60*/;
    rectangle1.Height -= 96 /*0x60*/;
    this._heartDisplayPosition = new Vector2((float) rectangle1.Center.X, (float) rectangle1.Top);
    if (this.Current.Character is NPC character)
    {
      rectangle1.Y += 56;
      rectangle1.Height -= 48 /*0x30*/;
      this._birthdayHeadingDisplayPosition = new Vector2((float) rectangle1.Center.X, (float) rectangle1.Top);
      if (character.birthday_Season.Value != null && Utility.getSeasonNumber(character.birthday_Season.Value) >= 0)
      {
        rectangle1.Y += 48 /*0x30*/;
        rectangle1.Height -= 48 /*0x30*/;
        this._birthdayDisplayPosition = new Vector2((float) rectangle1.Center.X, (float) rectangle1.Top);
        rectangle1.Y += 64 /*0x40*/;
        rectangle1.Height -= 64 /*0x40*/;
      }
      if (this._status != "")
      {
        this._statusHeadingDisplayPosition = new Vector2((float) rectangle1.Center.X, (float) rectangle1.Top);
        rectangle1.Y += 48 /*0x30*/;
        rectangle1.Height -= 48 /*0x30*/;
        this._statusDisplayPosition = new Vector2((float) rectangle1.Center.X, (float) rectangle1.Top);
        rectangle1.Y += 64 /*0x40*/;
        rectangle1.Height -= 64 /*0x40*/;
      }
    }
    rectangle2.Height -= 96 /*0x60*/;
    rectangle2.Y -= 8;
    this._giftLogHeadingDisplayPosition = new Vector2((float) rectangle2.Center.X, (float) rectangle2.Top);
    rectangle2.Y += 80 /*0x50*/;
    rectangle2.Height -= 70;
    this.backButton.bounds.X = rectangle2.Left + 64 /*0x40*/ - this.forwardButton.bounds.Width / 2;
    this.backButton.bounds.Y = rectangle2.Top;
    this.forwardButton.bounds.X = rectangle2.Right - 64 /*0x40*/ - this.forwardButton.bounds.Width / 2;
    this.forwardButton.bounds.Y = rectangle2.Top;
    rectangle2.Width -= 250;
    rectangle2.X += 125;
    this._giftLogCategoryDisplayPosition = new Vector2((float) rectangle2.Center.X, (float) rectangle2.Top);
    rectangle2.Y += 64 /*0x40*/;
    rectangle2.Y += 32 /*0x20*/;
    rectangle2.Height -= 32 /*0x20*/;
    this._itemDisplayRect = rectangle2;
    int num = 64 /*0x40*/;
    this.scrollBarRunner = new Rectangle(rectangle2.Right + 48 /*0x30*/, rectangle2.Top + num, this.scrollBar.bounds.Width, rectangle2.Height - num * 2);
    this.downArrow.bounds.Y = this.scrollBarRunner.Bottom + 16 /*0x10*/;
    this.downArrow.bounds.X = this.scrollBarRunner.Center.X - this.downArrow.bounds.Width / 2;
    this.upArrow.bounds.Y = this.scrollBarRunner.Top - 16 /*0x10*/ - this.upArrow.bounds.Height;
    this.upArrow.bounds.X = this.scrollBarRunner.Center.X - this.upArrow.bounds.Width / 2;
    float draw_y = 0.0f;
    if (this._profileItems.Count > 0)
    {
      int index1 = 0;
      for (int index2 = 0; index2 < this._profileItems.Count; ++index2)
      {
        ProfileItem profileItem = this._profileItems[index2];
        if (profileItem.ShouldDraw())
        {
          draw_y = profileItem.HandleLayout(draw_y, this._itemDisplayRect, index1);
          ++index1;
        }
      }
    }
    this.scrollSize = (int) draw_y - this._itemDisplayRect.Height;
    if (this.NeedsScrollBar())
    {
      this.upArrow.visible = true;
      this.downArrow.visible = true;
    }
    else
    {
      this.upArrow.visible = false;
      this.downArrow.visible = false;
    }
    this.UpdateScroll();
  }

  /// <inheritdoc />
  public override void leftClickHeld(int x, int y)
  {
    if (GameMenu.forcePreventClose)
      return;
    base.leftClickHeld(x, y);
    if (!this.scrolling)
      return;
    int scrollPosition1 = this.scrollPosition;
    this.scrollPosition = (int) Math.Round((double) (y - this.scrollBarRunner.Top) / (double) this.scrollBarRunner.Height * (double) this.scrollSize / (double) this.scrollStep) * this.scrollStep;
    this.UpdateScroll();
    int scrollPosition2 = this.scrollPosition;
    if (scrollPosition1 == scrollPosition2)
      return;
    Game1.playSound("shiny4");
  }

  public bool NeedsScrollBar() => this.scrollSize > 0;

  public void Scroll(int offset)
  {
    if (!this.NeedsScrollBar())
      return;
    int scrollPosition1 = this.scrollPosition;
    this.scrollPosition += offset;
    this.UpdateScroll();
    int scrollPosition2 = this.scrollPosition;
    if (scrollPosition1 == scrollPosition2)
      return;
    Game1.playSound("shwip");
  }

  public virtual void UpdateScroll()
  {
    this.scrollPosition = Utility.Clamp(this.scrollPosition, 0, this.scrollSize);
    float draw_y = (float) (this._itemDisplayRect.Top - this.scrollPosition);
    this._errorMessagePosition = new Vector2((float) this._itemDisplayRect.Center.X, (float) this._itemDisplayRect.Center.Y);
    if (this._profileItems.Count > 0)
    {
      int index1 = 0;
      for (int index2 = 0; index2 < this._profileItems.Count; ++index2)
      {
        ProfileItem profileItem = this._profileItems[index2];
        if (profileItem.ShouldDraw())
        {
          draw_y = profileItem.HandleLayout(draw_y, this._itemDisplayRect, index1);
          ++index1;
        }
      }
    }
    if (this.scrollSize <= 0)
      return;
    this.scrollBar.bounds.X = this.scrollBarRunner.Center.X - this.scrollBar.bounds.Width / 2;
    this.scrollBar.bounds.Y = (int) Utility.Lerp((float) this.scrollBarRunner.Top, (float) (this.scrollBarRunner.Bottom - this.scrollBar.bounds.Height), (float) this.scrollPosition / (float) this.scrollSize);
    if (!Game1.options.SnappyMenus)
      return;
    this.snapCursorToCurrentSnappedComponent();
  }

  /// <inheritdoc />
  public override void draw(SpriteBatch b)
  {
    if (!Game1.options.showClearBackgrounds)
      b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.4f);
    b.Draw(this.letterTexture, new Vector2((float) (this.xPositionOnScreen + this.width / 2), (float) (this.yPositionOnScreen + this.height / 2)), new Rectangle?(new Rectangle(0, 0, 320, 180)), Color.White, 0.0f, new Vector2(160f, 90f), 4f, SpriteEffects.None, 0.86f);
    Game1.DrawBox(this._characterStatusDisplayBox.X, this._characterStatusDisplayBox.Y, this._characterStatusDisplayBox.Width, this._characterStatusDisplayBox.Height);
    b.Draw(Game1.timeOfDay >= 1900 ? Game1.nightbg : Game1.daybg, this._characterSpriteDrawPosition, Color.White);
    Vector2 screenPosition = new Vector2(this._characterSpriteDrawPosition.X + (float) ((Game1.daybg.Width - this._animatedSprite.SpriteWidth * 4) / 2), this._characterSpriteDrawPosition.Y + 32f + (float) ((32 /*0x20*/ - this._animatedSprite.SpriteHeight) * 4));
    if (this.Current.Character is NPC character)
    {
      this._animatedSprite.draw(b, screenPosition, 0.8f);
      bool currentPlayer = this.Current.IsMarriedToCurrentPlayer();
      int val2 = Math.Max(10, Utility.GetMaximumHeartsForCharacter((Character) character));
      float heartDrawStartX = this._heartDisplayPosition.X - (float) (Math.Min(10, val2) * 32 /*0x20*/ / 2);
      float heartDrawStartY = val2 > 10 ? -16f : 0.0f;
      for (int hearts = 0; hearts < val2; ++hearts)
        this.drawNPCSlotHeart(b, heartDrawStartX, heartDrawStartY, this.Current, hearts, this.Current.IsDatingCurrentPlayer(), currentPlayer);
    }
    if (this._printedName.Length < this.Current.DisplayName.Length)
      SpriteText.drawStringWithScrollCenteredAt(b, "", (int) this._characterNamePosition.X, (int) this._characterNamePosition.Y, this._printedName);
    else
      SpriteText.drawStringWithScrollCenteredAt(b, this.Current.DisplayName, (int) this._characterNamePosition.X, (int) this._characterNamePosition.Y);
    if (character != null && character.birthday_Season.Value != null)
    {
      int seasonNumber = Utility.getSeasonNumber(character.birthday_Season.Value);
      if (seasonNumber >= 0)
      {
        SpriteText.drawStringHorizontallyCenteredAt(b, Game1.content.LoadString("Strings\\UI:Profile_Birthday"), (int) this._birthdayHeadingDisplayPosition.X, (int) this._birthdayHeadingDisplayPosition.Y);
        string text = Game1.content.LoadString("Strings\\UI:BirthdayOrder", (object) character.Birthday_Day, (object) Utility.getSeasonNameFromNumber(seasonNumber));
        b.DrawString(Game1.dialogueFont, text, new Vector2((float) (-(double) Game1.dialogueFont.MeasureString(text).X / 2.0) + this._birthdayDisplayPosition.X, this._birthdayDisplayPosition.Y), Game1.textColor);
      }
      if (this._status != "")
      {
        SpriteText.drawStringHorizontallyCenteredAt(b, Game1.content.LoadString("Strings\\UI:Profile_Status"), (int) this._statusHeadingDisplayPosition.X, (int) this._statusHeadingDisplayPosition.Y);
        b.DrawString(Game1.dialogueFont, this._status, new Vector2((float) (-(double) Game1.dialogueFont.MeasureString(this._status).X / 2.0) + this._statusDisplayPosition.X, this._statusDisplayPosition.Y), Game1.textColor);
      }
    }
    SpriteText.drawStringWithScrollCenteredAt(b, Game1.content.LoadString("Strings\\UI:Profile_GiftLog"), (int) this._giftLogHeadingDisplayPosition.X, (int) this._giftLogHeadingDisplayPosition.Y);
    SpriteText.drawStringHorizontallyCenteredAt(b, Game1.content.LoadString("Strings\\UI:" + ProfileMenu.itemCategories[this._currentCategory].categoryName, (object) this.Current.DisplayName), (int) this._giftLogCategoryDisplayPosition.X, (int) this._giftLogCategoryDisplayPosition.Y);
    bool flag1 = false;
    b.End();
    Rectangle scissorRectangle = b.GraphicsDevice.ScissorRectangle;
    b.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp, rasterizerState: Utility.ScissorEnabled);
    b.GraphicsDevice.ScissorRectangle = this._itemDisplayRect;
    if (this._profileItems.Count > 0)
    {
      for (int index = 0; index < this._profileItems.Count; ++index)
      {
        ProfileItem profileItem = this._profileItems[index];
        if (profileItem.ShouldDraw())
        {
          flag1 = true;
          profileItem.Draw(b);
        }
      }
    }
    b.End();
    b.GraphicsDevice.ScissorRectangle = scissorRectangle;
    b.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
    if (this.NeedsScrollBar())
    {
      IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(403, 383, 6, 6), this.scrollBarRunner.X, this.scrollBarRunner.Y, this.scrollBarRunner.Width, this.scrollBarRunner.Height, Color.White, 4f, false);
      this.scrollBar.draw(b);
    }
    if (!flag1)
    {
      string text = Game1.content.LoadString("Strings\\UI:Profile_GiftLog_NoGiftsGiven");
      b.DrawString(Game1.smallFont, text, new Vector2((float) (-(double) Game1.smallFont.MeasureString(text).X / 2.0) + this._errorMessagePosition.X, this._errorMessagePosition.Y), Game1.textColor);
    }
    foreach (ClickableTextureComponent textureComponent in this._clickableTextureComponents)
      textureComponent.draw(b);
    base.draw(b);
    this.drawMouse(b, true);
    if (this.hoveredItem == null)
      return;
    bool flag2 = true;
    if (Game1.options.snappyMenus && Game1.options.gamepadControls && !Game1.lastCursorMotionWasMouse && this._hideTooltipTime > 0)
      flag2 = false;
    if (!flag2)
      return;
    string hoverTitle = this.hoveredItem.DisplayName;
    string hoverText = this.hoveredItem.getDescription();
    if (hoverText.Contains("{0}") || this.hoveredItem.ItemId == "DriedMushrooms")
    {
      hoverTitle = Game1.content.LoadStringReturnNullIfNotFound($"Strings\\Objects:{this.hoveredItem.ItemId}_CollectionsTabName") ?? hoverTitle;
      hoverText = Game1.content.LoadStringReturnNullIfNotFound($"Strings\\Objects:{this.hoveredItem.ItemId}_CollectionsTabDescription") ?? hoverText;
    }
    IClickableMenu.drawToolTip(b, hoverText, hoverTitle, this.hoveredItem);
  }

  /// <summary>Draw the heart sprite for an NPC's entry in the social page.</summary>
  /// <param name="b">The sprite batch being drawn.</param>
  /// <param name="heartDrawStartX">The left X position at which to draw the first heart.</param>
  /// <param name="heartDrawStartY">The top Y position at which to draw hearts.</param>
  /// <param name="entry">The NPC's cached social data.</param>
  /// <param name="hearts">The current heart index being drawn (starting at 0 for the first heart).</param>
  /// <param name="isDating">Whether the player is currently dating this NPC.</param>
  /// <param name="isCurrentSpouse">Whether the player is currently married to this NPC.</param>
  private void drawNPCSlotHeart(
    SpriteBatch b,
    float heartDrawStartX,
    float heartDrawStartY,
    SocialPage.SocialEntry entry,
    int hearts,
    bool isDating,
    bool isCurrentSpouse)
  {
    bool flag = entry.IsDatable && !isDating && !isCurrentSpouse && hearts >= 8;
    int x = hearts < entry.HeartLevel | flag ? 211 : 218;
    Color color = hearts < 10 & flag ? Color.Black * 0.35f : Color.White;
    if (hearts < 10)
      b.Draw(Game1.mouseCursors, new Vector2(heartDrawStartX + (float) (hearts * 32 /*0x20*/), this._heartDisplayPosition.Y + heartDrawStartY), new Rectangle?(new Rectangle(x, 428, 7, 6)), color, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.88f);
    else
      b.Draw(Game1.mouseCursors, new Vector2(heartDrawStartX + (float) ((hearts - 10) * 32 /*0x20*/), (float) ((double) this._heartDisplayPosition.Y + (double) heartDrawStartY + 32.0)), new Rectangle?(new Rectangle(x, 428, 7, 6)), color, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.88f);
  }

  /// <inheritdoc />
  public override void receiveRightClick(int x, int y, bool playSound = true)
  {
    this.receiveLeftClick(x, y, playSound);
  }

  public void RegisterClickable(ClickableComponent clickable)
  {
    this.clickableProfileItems.Add(clickable);
  }

  public void UnregisterClickable(ClickableComponent clickable)
  {
    this.clickableProfileItems.Remove(clickable);
  }

  public class ProfileItemCategory
  {
    public string categoryName;
    public int[] validCategories;

    public ProfileItemCategory(string name, int[] valid_categories)
    {
      this.categoryName = name;
      this.validCategories = valid_categories;
    }
  }
}
