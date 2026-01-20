// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.ShippingMenu
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley.BellsAndWhistles;
using StardewValley.Extensions;
using System;
using System.Collections.Generic;

#nullable disable
namespace StardewValley.Menus;

public class ShippingMenu : IClickableMenu
{
  public const int region_okbutton = 101;
  public const int region_forwardButton = 102;
  public const int region_backButton = 103;
  public const int farming_category = 0;
  public const int foraging_category = 1;
  public const int fishing_category = 2;
  public const int mining_category = 3;
  public const int other_category = 4;
  public const int total_category = 5;
  public const int timePerIntroCategory = 500;
  public const int outroFadeTime = 800;
  public const int smokeRate = 100;
  public const int categorylabelHeight = 25;
  public int itemsPerCategoryPage = 9;
  public int currentPage = -1;
  public int currentTab;
  public List<ClickableTextureComponent> categories = new List<ClickableTextureComponent>();
  public ClickableTextureComponent okButton;
  public ClickableTextureComponent forwardButton;
  public ClickableTextureComponent backButton;
  private List<int> categoryTotals = new List<int>();
  private List<MoneyDial> categoryDials = new List<MoneyDial>();
  private Dictionary<Item, int> itemValues = new Dictionary<Item, int>();
  private Dictionary<Item, int> singleItemValues = new Dictionary<Item, int>();
  private List<List<Item>> categoryItems = new List<List<Item>>();
  private int categoryLabelsWidth;
  private int plusButtonWidth;
  private int itemSlotWidth;
  private int itemAndPlusButtonWidth;
  private int totalWidth;
  private int centerX;
  private int centerY;
  private int introTimer = 3500;
  private int outroFadeTimer;
  private int outroPauseBeforeDateChange;
  private int finalOutroTimer;
  private int smokeTimer;
  private int dayPlaqueY;
  private int moonShake = -1;
  private int timesPokedMoon;
  private float weatherX;
  private bool outro;
  private bool newDayPlaque;
  private bool savedYet;
  public TemporaryAnimatedSpriteList animations = new TemporaryAnimatedSpriteList();
  private SaveGameMenu saveGameMenu;
  protected bool _hasFinished;
  public bool _activated;
  private bool wasGreenRain;

  public ShippingMenu(IList<Item> items)
    : base(0, 0, Game1.uiViewport.Width, Game1.uiViewport.Height)
  {
    this._activated = false;
    this.parseItems(items);
    if (!Game1.wasRainingYesterday)
      Game1.changeMusicTrack(Game1.IsSummer ? "nightTime" : "none");
    this.wasGreenRain = Utility.isGreenRainDay(Game1.dayOfMonth - 1, Game1.season);
    this.categoryLabelsWidth = 512 /*0x0200*/;
    this.plusButtonWidth = 40;
    this.itemSlotWidth = 96 /*0x60*/;
    this.itemAndPlusButtonWidth = this.plusButtonWidth + this.itemSlotWidth + 8;
    this.totalWidth = this.categoryLabelsWidth + this.itemAndPlusButtonWidth;
    this.centerX = Game1.uiViewport.Width / 2;
    this.centerY = Game1.uiViewport.Height / 2;
    this._hasFinished = false;
    int num1 = Game1.content.GetCurrentLanguage() == LocalizedContentManager.LanguageCode.ru ? 64 /*0x40*/ : 0;
    int num2 = -1;
    for (int index = 0; index < 6; ++index)
    {
      List<ClickableTextureComponent> categories = this.categories;
      ClickableTextureComponent textureComponent = new ClickableTextureComponent("", new Rectangle(this.centerX + num1 + this.totalWidth / 2 - this.plusButtonWidth, this.centerY - 300 + index * 27 * 4, this.plusButtonWidth, 44), "", this.getCategoryName(index), Game1.mouseCursors, new Rectangle(392, 361, 10, 11), 4f);
      textureComponent.visible = index < 5 && this.categoryItems[index].Count > 0;
      textureComponent.myID = index;
      textureComponent.downNeighborID = index < 4 ? index + 1 : 101;
      textureComponent.upNeighborID = index > 0 ? num2 : -1;
      textureComponent.upNeighborImmutable = true;
      categories.Add(textureComponent);
      num2 = index >= 5 || this.categoryItems[index].Count <= 0 ? num2 : index;
    }
    this.dayPlaqueY = this.categories[0].bounds.Y - 128 /*0x80*/;
    Rectangle bounds = new Rectangle(this.centerX + num1 + this.totalWidth / 2 - this.itemAndPlusButtonWidth + 32 /*0x20*/, this.centerY + 300 - 64 /*0x40*/, 64 /*0x40*/, 64 /*0x40*/);
    ClickableTextureComponent textureComponent1 = new ClickableTextureComponent(Game1.content.LoadString("Strings\\StringsFromCSFiles:ShippingMenu.cs.11382"), bounds, (string) null, Game1.content.LoadString("Strings\\StringsFromCSFiles:ShippingMenu.cs.11382"), Game1.mouseCursors, new Rectangle(128 /*0x80*/, 256 /*0x0100*/, 64 /*0x40*/, 64 /*0x40*/), 1f);
    textureComponent1.myID = 101;
    textureComponent1.upNeighborID = num2;
    this.okButton = textureComponent1;
    ClickableTextureComponent textureComponent2 = new ClickableTextureComponent("", new Rectangle(this.xPositionOnScreen + 32 /*0x20*/, this.yPositionOnScreen + this.height - 64 /*0x40*/, 48 /*0x30*/, 44), (string) null, "", Game1.mouseCursors, new Rectangle(352, 495, 12, 11), 4f);
    textureComponent2.myID = 103;
    textureComponent2.rightNeighborID = -7777;
    this.backButton = textureComponent2;
    ClickableTextureComponent textureComponent3 = new ClickableTextureComponent("", new Rectangle(this.xPositionOnScreen + this.width - 32 /*0x20*/ - 48 /*0x30*/, this.yPositionOnScreen + this.height - 64 /*0x40*/, 48 /*0x30*/, 44), (string) null, "", Game1.mouseCursors, new Rectangle(365, 495, 12, 11), 4f);
    textureComponent3.myID = 102;
    textureComponent3.leftNeighborID = 103;
    this.forwardButton = textureComponent3;
    if (Game1.dayOfMonth == 25 && Game1.season == Season.Winter)
      this.animations.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(640, 800, 32 /*0x20*/, 16 /*0x10*/), 80f, 2, 1000, new Vector2((float) Game1.uiViewport.Width, (float) Game1.random.Next(0, 200)), false, false, 0.01f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
      {
        motion = new Vector2(-4f, 0.0f),
        delayBeforeAnimationStart = 3000
      });
    Game1.stats.checkForShippingAchievements();
    this.RepositionItems();
    this.populateClickableComponentList();
    if (!Game1.options.SnappyMenus)
      return;
    this.snapToDefaultClickableComponent();
  }

  public void RepositionItems()
  {
    this.centerX = Game1.uiViewport.Width / 2;
    this.centerY = Game1.uiViewport.Height / 2;
    int width = Game1.uiViewport.Width;
    int height = Game1.uiViewport.Height;
    int num1 = Math.Min(this.width, 1280 /*0x0500*/);
    int num2 = Math.Min(this.height, 920);
    int num3 = Game1.content.GetCurrentLanguage() == LocalizedContentManager.LanguageCode.ru ? 64 /*0x40*/ : 0;
    for (int index = 0; index < 6; ++index)
      this.categories[index].bounds = new Rectangle(this.centerX + num3 + this.totalWidth / 2 - this.plusButtonWidth, this.centerY - 300 + index * 27 * 4, this.plusButtonWidth, 44);
    this.dayPlaqueY = this.categories[0].bounds.Y - 128 /*0x80*/;
    if (this.dayPlaqueY < 0)
      this.dayPlaqueY = -64;
    this.backButton.bounds.X = this.centerX - num1 / 2 - 64 /*0x40*/;
    this.backButton.bounds.Y = this.centerY + num2 / 2 - 48 /*0x30*/;
    if (this.backButton.bounds.X < 0)
      this.backButton.bounds.X = this.xPositionOnScreen + 32 /*0x20*/;
    if (this.backButton.bounds.Y > Game1.uiViewport.Height - 32 /*0x20*/)
      this.backButton.bounds.Y = Game1.uiViewport.Height - 80 /*0x50*/;
    this.forwardButton.bounds.X = this.centerX + num1 / 2 + 8;
    this.forwardButton.bounds.Y = this.centerY + num2 / 2 - 48 /*0x30*/;
    if (this.forwardButton.bounds.X > Game1.uiViewport.Width - 32 /*0x20*/)
      this.forwardButton.bounds.X = this.xPositionOnScreen + this.width - 32 /*0x20*/ - 48 /*0x30*/;
    if (this.forwardButton.bounds.Y > Game1.uiViewport.Height - 32 /*0x20*/)
      this.forwardButton.bounds.Y = Game1.uiViewport.Height - 80 /*0x50*/;
    this.okButton.bounds = new Rectangle(this.centerX + num3 + this.totalWidth / 2 - this.itemAndPlusButtonWidth + 32 /*0x20*/, this.centerY + 300 - 64 /*0x40*/, 64 /*0x40*/, 64 /*0x40*/);
    this.itemsPerCategoryPage = (int) ((double) (this.yPositionOnScreen + Math.Min(this.height, 920) - 64 /*0x40*/ - (this.yPositionOnScreen + 32 /*0x20*/)) / 68.0);
    if (this.currentPage < 0)
      return;
    this.currentTab = Utility.Clamp(this.currentTab, 0, (this.categoryItems[this.currentPage].Count - 1) / this.itemsPerCategoryPage);
  }

  protected override void customSnapBehavior(int direction, int oldRegion, int oldID)
  {
    if (oldID != 103 || direction != 1 || !this.showForwardButton())
      return;
    this.currentlySnappedComponent = this.getComponentWithID(102);
    this.snapCursorToCurrentSnappedComponent();
  }

  public override void snapToDefaultClickableComponent()
  {
    if (this.currentPage != -1)
      this.currentlySnappedComponent = this.getComponentWithID(103);
    else
      this.currentlySnappedComponent = this.getComponentWithID(101);
    this.snapCursorToCurrentSnappedComponent();
  }

  public void parseItems(IList<Item> items)
  {
    Utility.consolidateStacks(items);
    for (int index = 0; index < 6; ++index)
    {
      this.categoryItems.Add(new List<Item>());
      this.categoryTotals.Add(0);
      this.categoryDials.Add(new MoneyDial(7, index == 5));
    }
    foreach (Item key in (IEnumerable<Item>) items)
    {
      if (key != null)
      {
        int categoryIndexForObject = this.getCategoryIndexForObject(key);
        this.categoryItems[categoryIndexForObject].Add(key);
        int storePrice = key.sellToStorePrice(-1L);
        int num = storePrice * key.Stack;
        this.categoryTotals[categoryIndexForObject] += num;
        this.itemValues[key] = num;
        this.singleItemValues[key] = storePrice;
        Game1.stats.ItemsShipped += (uint) key.Stack;
        if (key.Category == -75 || key.Category == -79)
          Game1.stats.CropsShipped += (uint) key.Stack;
        if (key is StardewValley.Object @object && @object.countsForShippedCollection())
          Game1.player.shippedBasic(@object.ItemId, @object.stack.Value);
      }
    }
    for (int index = 0; index < 5; ++index)
    {
      this.categoryTotals[5] += this.categoryTotals[index];
      this.categoryItems[5].AddRange((IEnumerable<Item>) this.categoryItems[index]);
      this.categoryDials[index].currentValue = this.categoryTotals[index];
      this.categoryDials[index].previousTargetValue = this.categoryDials[index].currentValue;
    }
    this.categoryDials[5].currentValue = this.categoryTotals[5];
    Game1.setRichPresence("earnings", (object) this.categoryTotals[5]);
  }

  public int getCategoryIndexForObject(Item item)
  {
    string qualifiedItemId = item.QualifiedItemId;
    if (qualifiedItemId != null && qualifiedItemId.Length == 6)
    {
      switch (qualifiedItemId[5])
      {
        case '0':
          if (!(qualifiedItemId == "(O)410"))
            goto label_8;
          break;
        case '2':
          if (qualifiedItemId == "(O)402")
            break;
          goto label_8;
        case '4':
          if (qualifiedItemId == "(O)414")
            break;
          goto label_8;
        case '6':
          if (qualifiedItemId == "(O)396" || qualifiedItemId == "(O)406" || qualifiedItemId == "(O)296")
            break;
          goto label_8;
        case '8':
          if (qualifiedItemId == "(O)418")
            break;
          goto label_8;
        default:
          goto label_8;
      }
      return 1;
    }
label_8:
    if (item is StardewValley.Object @object)
    {
      StardewValley.Object.PreserveType? nullable = @object.preserve.Value;
      if (nullable.GetValueOrDefault() != StardewValley.Object.PreserveType.SmokedFish)
      {
        nullable = @object.preserve.Value;
        if (nullable.GetValueOrDefault() != StardewValley.Object.PreserveType.AgedRoe)
        {
          nullable = @object.preserve.Value;
          if (nullable.GetValueOrDefault() != StardewValley.Object.PreserveType.Roe)
            goto label_13;
        }
      }
      return 2;
    }
label_13:
    switch (item.Category)
    {
      case -81:
      case -27:
      case -23:
        return 1;
      case -80:
      case -79:
      case -75:
      case -26:
      case -14:
      case -6:
      case -5:
        return 0;
      case -21:
      case -20:
      case -4:
        return 2;
      case -15:
      case -12:
      case -2:
        return 3;
      default:
        return 4;
    }
  }

  public string getCategoryName(int index)
  {
    switch (index)
    {
      case 0:
        return Game1.content.LoadString("Strings\\StringsFromCSFiles:ShippingMenu.cs.11389");
      case 1:
        return Game1.content.LoadString("Strings\\StringsFromCSFiles:ShippingMenu.cs.11390");
      case 2:
        return Game1.content.LoadString("Strings\\StringsFromCSFiles:ShippingMenu.cs.11391");
      case 3:
        return Game1.content.LoadString("Strings\\StringsFromCSFiles:ShippingMenu.cs.11392");
      case 4:
        return Game1.content.LoadString("Strings\\StringsFromCSFiles:ShippingMenu.cs.11393");
      case 5:
        return Game1.content.LoadString("Strings\\StringsFromCSFiles:ShippingMenu.cs.11394");
      default:
        return "";
    }
  }

  /// <inheritdoc />
  public override void update(GameTime time)
  {
    base.update(time);
    if (!this._activated)
    {
      this._activated = true;
      Game1.player.team.endOfNightStatus.UpdateState("shipment");
    }
    if (this._hasFinished)
    {
      if (!Game1.PollForEndOfNewDaySync())
        return;
      this.exitThisMenu(false);
    }
    else
    {
      if (this.saveGameMenu != null)
      {
        this.saveGameMenu.update(time);
        if (this.saveGameMenu.quit)
        {
          this.saveGameMenu = (SaveGameMenu) null;
          this.savedYet = true;
        }
      }
      this.weatherX += (float) time.ElapsedGameTime.Milliseconds * 0.03f;
      this.animations.RemoveWhere<TemporaryAnimatedSprite>((Predicate<TemporaryAnimatedSprite>) (animation => animation.update(time)));
      if (this.outro)
      {
        if (this.outroFadeTimer > 0)
          this.outroFadeTimer -= time.ElapsedGameTime.Milliseconds;
        else if (this.outroFadeTimer <= 0 && this.dayPlaqueY < this.centerY - 64 /*0x40*/)
        {
          if (this.animations.Count > 0)
            this.animations.Clear();
          this.dayPlaqueY += (int) Math.Ceiling((double) time.ElapsedGameTime.Milliseconds * 0.34999999403953552);
          if (this.dayPlaqueY >= this.centerY - 64 /*0x40*/)
            this.outroPauseBeforeDateChange = 700;
        }
        else if (this.outroPauseBeforeDateChange > 0)
        {
          this.outroPauseBeforeDateChange -= time.ElapsedGameTime.Milliseconds;
          if (this.outroPauseBeforeDateChange <= 0)
          {
            this.newDayPlaque = true;
            Game1.playSound("newRecipe");
            if (Game1.season != Season.Winter && Game1.game1.IsMainInstance)
              DelayedAction.playSoundAfterDelay(Game1.IsRainingHere() ? "rainsound" : "rooster", 1500);
            this.finalOutroTimer = 2000;
            this.animations.Clear();
            if (!this.savedYet)
            {
              if (this.saveGameMenu != null)
                return;
              this.saveGameMenu = new SaveGameMenu();
              return;
            }
          }
        }
        else if (this.finalOutroTimer > 0 && this.savedYet)
        {
          this.finalOutroTimer -= time.ElapsedGameTime.Milliseconds;
          if (this.finalOutroTimer <= 0)
            this._hasFinished = true;
        }
      }
      if (this.introTimer >= 0)
      {
        int introTimer = this.introTimer;
        this.introTimer -= time.ElapsedGameTime.Milliseconds * (Game1.oldMouseState.LeftButton == ButtonState.Pressed ? 3 : 1);
        if (introTimer % 500 < this.introTimer % 500 && this.introTimer <= 3000)
        {
          int num = 4 - this.introTimer / 500;
          if (num < 6 && num > -1)
          {
            if (this.categoryItems[num].Count > 0)
            {
              Game1.playSound(this.getCategorySound(num));
              this.categoryDials[num].currentValue = 0;
              this.categoryDials[num].previousTargetValue = 0;
            }
            else
              Game1.playSound("stoneStep");
          }
        }
        if (this.introTimer < 0)
        {
          if (Game1.options.SnappyMenus)
            this.snapToDefaultClickableComponent();
          Game1.playSound("money");
          this.categoryDials[5].currentValue = 0;
          this.categoryDials[5].previousTargetValue = 0;
        }
      }
      else if (Game1.dayOfMonth != 28 && !this.outro)
      {
        if (!Game1.wasRainingYesterday)
        {
          Vector2 position = new Vector2((float) Game1.uiViewport.Width, (float) Game1.random.Next(200));
          Rectangle sourceRect = new Rectangle(640, 752, 16 /*0x10*/, 16 /*0x10*/);
          int num = Game1.random.Next(1, 4);
          if (Game1.random.NextDouble() < 0.001)
          {
            bool flipped = Game1.random.NextBool();
            if (Game1.random.NextBool())
              this.animations.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(640, 826, 16 /*0x10*/, 8), 40f, 4, 0, new Vector2((float) Game1.random.Next(this.centerX * 2), (float) Game1.random.Next(this.centerY)), false, flipped)
              {
                rotation = 3.14159274f,
                scale = 4f,
                motion = new Vector2(flipped ? -8f : 8f, 8f),
                local = true
              });
            else
              this.animations.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(258, 1680, 16 /*0x10*/, 16 /*0x10*/), 40f, 4, 0, new Vector2((float) Game1.random.Next(this.centerX * 2), (float) Game1.random.Next(this.centerY)), false, flipped)
              {
                scale = 4f,
                motion = new Vector2(flipped ? -8f : 8f, 8f),
                local = true
              });
          }
          else if (Game1.random.NextDouble() < 0.0002)
          {
            position = new Vector2((float) Game1.uiViewport.Width, (float) Game1.random.Next(4, 256 /*0x0100*/));
            this.animations.Add(new TemporaryAnimatedSprite("", new Rectangle(0, 0, 1, 1), 9999f, 1, 10000, position, false, false, 0.01f, 0.0f, Color.White * (0.25f + (float) Game1.random.NextDouble()), 4f, 0.0f, 0.0f, 0.0f, true)
            {
              motion = new Vector2(-0.25f, 0.0f)
            });
          }
          else if (Game1.random.NextDouble() < 5E-05)
          {
            position = new Vector2((float) Game1.uiViewport.Width, (float) (Game1.uiViewport.Height - 192 /*0xC0*/));
            for (int index = 0; index < num; ++index)
            {
              this.animations.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", sourceRect, (float) Game1.random.Next(60, 101), 4, 100, position + new Vector2((float) ((index + 1) * Game1.random.Next(15, 18)), (float) ((index + 1) * -20)), false, false, 0.01f, 0.0f, Color.Black, 4f, 0.0f, 0.0f, 0.0f, true)
              {
                motion = new Vector2(-1f, 0.0f)
              });
              this.animations.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", sourceRect, (float) Game1.random.Next(60, 101), 4, 100, position + new Vector2((float) ((index + 1) * Game1.random.Next(15, 18)), (float) ((index + 1) * 20)), false, false, 0.01f, 0.0f, Color.Black, 4f, 0.0f, 0.0f, 0.0f, true)
              {
                motion = new Vector2(-1f, 0.0f)
              });
            }
          }
          else if (Game1.random.NextDouble() < 1E-05)
          {
            sourceRect = new Rectangle(640, 784, 16 /*0x10*/, 16 /*0x10*/);
            this.animations.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", sourceRect, 75f, 4, 1000, position, false, false, 0.01f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
            {
              motion = new Vector2(-3f, 0.0f),
              yPeriodic = true,
              yPeriodicLoopTime = 1000f,
              yPeriodicRange = 8f,
              shakeIntensity = 0.5f
            });
          }
        }
        this.smokeTimer -= time.ElapsedGameTime.Milliseconds;
        if (this.smokeTimer <= 0)
        {
          this.smokeTimer = 50;
          this.animations.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(684, 1075, 1, 1), 1000f, 1, 1000, new Vector2(188f, (float) (Game1.uiViewport.Height - 128 /*0x80*/ + 20)), false, false)
          {
            color = Game1.wasRainingYesterday ? Color.SlateGray : Color.White,
            scale = 4f,
            scaleChange = 0.0f,
            alphaFade = 1f / 400f,
            motion = new Vector2(0.0f, (float) ((double) -Game1.random.Next(25, 75) / 100.0 / 4.0)),
            acceleration = new Vector2(-1f / 1000f, 0.0f)
          });
        }
      }
      if (this.moonShake <= 0)
        return;
      this.moonShake -= time.ElapsedGameTime.Milliseconds;
    }
  }

  public string getCategorySound(int which)
  {
    switch (which)
    {
      case 0:
        bool? nullable = this.categoryItems[0][0] is StardewValley.Object @object ? new bool?(@object.isAnimalProduct()) : new bool?();
        return !nullable.HasValue || !nullable.GetValueOrDefault() ? "harvest" : "cluck";
      case 1:
        return "leafrustle";
      case 2:
        return "button1";
      case 3:
        return "hammer";
      case 4:
        return "coin";
      case 5:
        return "money";
      default:
        return "stoneStep";
    }
  }

  public override void applyMovementKey(int direction)
  {
    if (!this.CanReceiveInput())
      return;
    base.applyMovementKey(direction);
  }

  /// <inheritdoc />
  public override void performHoverAction(int x, int y)
  {
    if (!this.CanReceiveInput())
      return;
    base.performHoverAction(x, y);
    if (this.currentPage == -1)
    {
      this.okButton.tryHover(x, y);
      foreach (ClickableTextureComponent category in this.categories)
        category.sourceRect.X = !category.containsPoint(x, y) ? 392 : 402;
    }
    else
    {
      this.backButton.tryHover(x, y, 0.5f);
      this.forwardButton.tryHover(x, y, 0.5f);
    }
  }

  public bool CanReceiveInput() => this.introTimer <= 0 && this.saveGameMenu == null && !this.outro;

  /// <inheritdoc />
  public override void receiveKeyPress(Keys key)
  {
    if (!this.CanReceiveInput())
      return;
    if (this.introTimer <= 0 && !Game1.options.gamepadControls && (key.Equals((object) Keys.Escape) || Game1.options.doesInputListContain(Game1.options.menuButton, key)))
    {
      if (this.currentPage == -1)
        this.receiveLeftClick(this.okButton.bounds.Center.X, this.okButton.bounds.Center.Y, true);
      else
        this.receiveLeftClick(this.backButton.bounds.Center.X, this.backButton.bounds.Center.Y, true);
    }
    else
    {
      if (this.introTimer > 0 || Game1.options.gamepadControls && Game1.options.doesInputListContain(Game1.options.menuButton, key))
        return;
      base.receiveKeyPress(key);
    }
  }

  /// <inheritdoc />
  public override void receiveGamePadButton(Buttons button)
  {
    if (!this.CanReceiveInput())
      return;
    base.receiveGamePadButton(button);
    if (button != Buttons.Start && button != Buttons.B)
      return;
    if (button == Buttons.B && this.currentPage != -1)
    {
      if (this.currentTab == 0)
      {
        if (Game1.options.SnappyMenus)
        {
          this.currentlySnappedComponent = this.getComponentWithID(this.currentPage);
          this.snapCursorToCurrentSnappedComponent();
        }
        this.currentPage = -1;
      }
      else
        --this.currentTab;
      Game1.playSound("shwip");
    }
    else
    {
      if (this.currentPage != -1 || this.outro)
        return;
      if (this.introTimer <= 0)
        this.okClicked();
      else
        this.introTimer -= Game1.currentGameTime.ElapsedGameTime.Milliseconds * 2;
    }
  }

  private void okClicked()
  {
    this.outro = true;
    this.outroFadeTimer = 800;
    Game1.playSound("bigDeSelect");
    Game1.changeMusicTrack("none");
  }

  /// <inheritdoc />
  public override void receiveLeftClick(int x, int y, bool playSound = true)
  {
    if (!this.CanReceiveInput() || this.outro && !this.savedYet || this.savedYet)
      return;
    base.receiveLeftClick(x, y, playSound);
    if (this.currentPage == -1 && this.introTimer <= 0 && this.okButton.containsPoint(x, y))
      this.okClicked();
    if (this.currentPage == -1)
    {
      for (int index = 0; index < this.categories.Count; ++index)
      {
        if (this.categories[index].visible && this.categories[index].containsPoint(x, y))
        {
          this.currentPage = index;
          Game1.playSound("shwip");
          if (Game1.options.SnappyMenus)
          {
            this.currentlySnappedComponent = this.getComponentWithID(103);
            this.snapCursorToCurrentSnappedComponent();
            break;
          }
          break;
        }
      }
      if (Game1.dayOfMonth != 28 || this.timesPokedMoon > 10 || !new Rectangle(Game1.uiViewport.Width - 176 /*0xB0*/, 4, 172, 172).Contains(x, y))
        return;
      this.moonShake = 100;
      ++this.timesPokedMoon;
      if (this.timesPokedMoon > 10)
        Game1.playSound("shadowDie");
      else
        Game1.playSound("thudStep");
    }
    else if (this.backButton.containsPoint(x, y))
    {
      if (this.currentTab == 0)
      {
        if (Game1.options.SnappyMenus)
        {
          this.currentlySnappedComponent = this.getComponentWithID(this.currentPage);
          this.snapCursorToCurrentSnappedComponent();
        }
        this.currentPage = -1;
      }
      else
        --this.currentTab;
      Game1.playSound("shwip");
    }
    else
    {
      if (!this.showForwardButton() || !this.forwardButton.containsPoint(x, y))
        return;
      ++this.currentTab;
      Game1.playSound("shwip");
    }
  }

  public bool showForwardButton()
  {
    return this.categoryItems[this.currentPage].Count > this.itemsPerCategoryPage * (this.currentTab + 1);
  }

  /// <inheritdoc />
  public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
  {
    this.initialize(0, 0, Game1.uiViewport.Width, Game1.uiViewport.Height);
    this.RepositionItems();
  }

  /// <inheritdoc />
  public override void draw(SpriteBatch b)
  {
    bool flag = Game1.season == Season.Winter;
    if (Game1.wasRainingYesterday)
    {
      b.Draw(Game1.mouseCursors, new Rectangle(0, 0, Game1.uiViewport.Width, Game1.uiViewport.Height), new Rectangle?(new Rectangle(this.wasGreenRain ? 640 : 639, 858, 1, 184)), (flag ? Color.LightSlateGray : (this.wasGreenRain ? Color.LightGreen : Color.SlateGray)) * (float) (1.0 - (double) this.introTimer / 3500.0));
      if (this.wasGreenRain)
        b.Draw(Game1.mouseCursors, new Rectangle(0, 0, Game1.uiViewport.Width, Game1.uiViewport.Height), new Rectangle?(new Rectangle(this.wasGreenRain ? 640 : 639, 858, 1, 184)), Color.DimGray * 0.8f * (float) (1.0 - (double) this.introTimer / 3500.0));
      for (int index = -244; index < Game1.uiViewport.Width + 244; index += 244)
        b.Draw(Game1.mouseCursors, new Vector2((float) index + (float) ((double) this.weatherX / 2.0 % 244.0), 32f), new Rectangle?(new Rectangle(643, 1142, 61, 53)), Color.DarkSlateGray * 1f * (float) (1.0 - (double) this.introTimer / 3500.0), 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
      for (int index = 0; index < this.width; index += 639)
      {
        b.Draw(Game1.mouseCursors, new Vector2((float) (index * 4), (float) (Game1.uiViewport.Height - 192 /*0xC0*/)), new Rectangle?(new Rectangle(0, flag ? 1034 : 737, 639, 48 /*0x30*/)), (flag ? Color.White * 0.25f : new Color(30, 62, 50)) * (float) (0.5 - (double) this.introTimer / 3500.0), 0.0f, Vector2.Zero, 4f, SpriteEffects.FlipHorizontally, 1f);
        b.Draw(Game1.mouseCursors, new Vector2((float) (index * 4), (float) (Game1.uiViewport.Height - 128 /*0x80*/)), new Rectangle?(new Rectangle(0, flag ? 1034 : 737, 639, 32 /*0x20*/)), (flag ? Color.White * 0.5f : new Color(30, 62, 50)) * (float) (1.0 - (double) this.introTimer / 3500.0), 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
      }
      b.Draw(Game1.mouseCursors, new Vector2(160f, (float) (Game1.uiViewport.Height - 128 /*0x80*/ + 16 /*0x10*/ + 8)), new Rectangle?(new Rectangle(653, 880, 10, 10)), Color.White * (float) (1.0 - (double) this.introTimer / 3500.0), 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
      for (int index = -244; index < Game1.uiViewport.Width + 244; index += 244)
        b.Draw(Game1.mouseCursors, new Vector2((float) index + this.weatherX % 244f, -32f), new Rectangle?(new Rectangle(643, 1142, 61, 53)), Color.SlateGray * 0.85f * (float) (1.0 - (double) this.introTimer / 3500.0), 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.9f);
      foreach (TemporaryAnimatedSprite animation in this.animations)
        animation.draw(b, true);
      for (int index = -244; index < Game1.uiViewport.Width + 244; index += 244)
        b.Draw(Game1.mouseCursors, new Vector2((float) index + (float) ((double) this.weatherX * 1.5 % 244.0), (float) sbyte.MinValue), new Rectangle?(new Rectangle(643, 1142, 61, 53)), Color.LightSlateGray * (float) (1.0 - (double) this.introTimer / 3500.0), 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.9f);
    }
    else
    {
      b.Draw(Game1.mouseCursors, new Rectangle(0, 0, Game1.uiViewport.Width, Game1.uiViewport.Height), new Rectangle?(new Rectangle(639, 858, 1, 184)), Color.White * (float) (1.0 - (double) this.introTimer / 3500.0));
      for (int index = 0; index < this.width; index += 639)
        b.Draw(Game1.mouseCursors, new Vector2((float) (index * 4), 0.0f), new Rectangle?(new Rectangle(0, 1453, 639, 195)), Color.White * (float) (1.0 - (double) this.introTimer / 3500.0), 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
      if (Game1.dayOfMonth == 28)
      {
        b.Draw(Game1.mouseCursors, new Vector2((float) (Game1.uiViewport.Width - 176 /*0xB0*/), 4f) + (this.moonShake > 0 ? new Vector2((float) Game1.random.Next(-1, 2), (float) Game1.random.Next(-1, 2)) : Vector2.Zero), new Rectangle?(new Rectangle(642, 835, 43, 43)), Color.White * (float) (1.0 - (double) this.introTimer / 3500.0), 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
        if (this.timesPokedMoon > 10)
        {
          SpriteBatch spriteBatch = b;
          Texture2D mouseCursors = Game1.mouseCursors;
          Vector2 position = new Vector2((float) (Game1.uiViewport.Width - 136), 48f) + (this.moonShake > 0 ? new Vector2((float) Game1.random.Next(-1, 2), (float) Game1.random.Next(-1, 2)) : Vector2.Zero);
          TimeSpan totalGameTime = Game1.currentGameTime.TotalGameTime;
          int num;
          if (totalGameTime.TotalMilliseconds % 4000.0 >= 200.0)
          {
            totalGameTime = Game1.currentGameTime.TotalGameTime;
            if (totalGameTime.TotalMilliseconds % 8000.0 > 7600.0)
            {
              totalGameTime = Game1.currentGameTime.TotalGameTime;
              if (totalGameTime.TotalMilliseconds % 8000.0 < 7800.0)
                goto label_31;
            }
            num = 0;
            goto label_32;
          }
label_31:
          num = 21;
label_32:
          Rectangle? sourceRectangle = new Rectangle?(new Rectangle(685, 844 + num, 19, 21));
          Color color = Color.White * (float) (1.0 - (double) this.introTimer / 3500.0);
          Vector2 zero = Vector2.Zero;
          spriteBatch.Draw(mouseCursors, position, sourceRectangle, color, 0.0f, zero, 4f, SpriteEffects.None, 1f);
        }
      }
      b.Draw(Game1.mouseCursors, new Vector2(0.0f, (float) (Game1.uiViewport.Height - 192 /*0xC0*/)), new Rectangle?(new Rectangle(0, flag ? 1034 : 737, 639, 48 /*0x30*/)), (flag ? Color.White * 0.25f : new Color(0, 20, 40)) * (float) (0.64999997615814209 - (double) this.introTimer / 3500.0), 0.0f, Vector2.Zero, 4f, SpriteEffects.FlipHorizontally, 1f);
      b.Draw(Game1.mouseCursors, new Vector2(2556f, (float) (Game1.uiViewport.Height - 192 /*0xC0*/)), new Rectangle?(new Rectangle(0, flag ? 1034 : 737, 639, 48 /*0x30*/)), (flag ? Color.White * 0.25f : new Color(0, 20, 40)) * (float) (0.64999997615814209 - (double) this.introTimer / 3500.0), 0.0f, Vector2.Zero, 4f, SpriteEffects.FlipHorizontally, 1f);
      b.Draw(Game1.mouseCursors, new Vector2(0.0f, (float) (Game1.uiViewport.Height - 128 /*0x80*/)), new Rectangle?(new Rectangle(0, flag ? 1034 : 737, 639, 32 /*0x20*/)), (flag ? Color.White * 0.5f : new Color(0, 32 /*0x20*/, 20)) * (float) (1.0 - (double) this.introTimer / 3500.0), 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
      b.Draw(Game1.mouseCursors, new Vector2(2556f, (float) (Game1.uiViewport.Height - 128 /*0x80*/)), new Rectangle?(new Rectangle(0, flag ? 1034 : 737, 639, 32 /*0x20*/)), (flag ? Color.White * 0.5f : new Color(0, 32 /*0x20*/, 20)) * (float) (1.0 - (double) this.introTimer / 3500.0), 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
      b.Draw(Game1.mouseCursors, new Vector2(160f, (float) (Game1.uiViewport.Height - 128 /*0x80*/ + 16 /*0x10*/ + 8)), new Rectangle?(new Rectangle(653, 880, 10, 10)), Color.White * (float) (1.0 - (double) this.introTimer / 3500.0), 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
    }
    if (!this.outro && !Game1.wasRainingYesterday)
    {
      foreach (TemporaryAnimatedSprite animation in this.animations)
        animation.draw(b, true);
    }
    if (this.wasGreenRain)
      b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Green * 0.1f);
    if (this.currentPage == -1)
    {
      int y = this.categories[0].bounds.Y - 128 /*0x80*/;
      if (y >= 0)
        SpriteText.drawStringWithScrollCenteredAt(b, Utility.getYesterdaysDate(), Game1.uiViewport.Width / 2, y);
      int num1 = Game1.content.GetCurrentLanguage() == LocalizedContentManager.LanguageCode.ru ? 64 /*0x40*/ : 0;
      int num2 = -20;
      int index1 = 0;
      foreach (ClickableTextureComponent category in this.categories)
      {
        if (this.introTimer < 2500 - index1 * 500)
        {
          Vector2 vector2 = category.getVector2() + new Vector2((float) (12 - num1), -8f);
          if (category.visible)
          {
            category.draw(b);
            b.Draw(Game1.mouseCursors, vector2 + new Vector2((float) (num1 - 104), (float) (num2 + 4)), new Rectangle?(new Rectangle(293, 360, 24, 24)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.88f);
            this.categoryItems[index1][0].drawInMenu(b, vector2 + new Vector2((float) (num1 - 88), (float) (num2 + 16 /*0x10*/)), 1f, 1f, 0.9f, StackDrawType.Hide);
          }
          IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(384, 373, 18, 18), (int) ((double) vector2.X + (double) -this.itemSlotWidth - (double) this.categoryLabelsWidth - 12.0), (int) ((double) vector2.Y + (double) num2), this.categoryLabelsWidth + num1, 104, Color.White, 4f, false);
          SpriteText.drawString(b, category.hoverText, (int) vector2.X - this.itemSlotWidth - this.categoryLabelsWidth + 8, (int) vector2.Y + 4);
          for (int index2 = 0; index2 < 6; ++index2)
            b.Draw(Game1.mouseCursors, vector2 + new Vector2((float) (-this.itemSlotWidth + num1 - 192 /*0xC0*/ - 24 + index2 * 6 * 4), 12f), new Rectangle?(new Rectangle(355, 476, 7, 11)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.88f);
          this.categoryDials[index1].draw(b, vector2 + new Vector2((float) (-this.itemSlotWidth + num1 - 192 /*0xC0*/ - 48 /*0x30*/ + 4), 20f), this.categoryTotals[index1]);
          b.Draw(Game1.mouseCursors, vector2 + new Vector2((float) (-this.itemSlotWidth + num1 - 64 /*0x40*/ - 4), 12f), new Rectangle?(new Rectangle(408, 476, 9, 11)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.88f);
        }
        ++index1;
      }
      if (this.introTimer <= 0)
        this.okButton.draw(b);
    }
    else
    {
      int width1 = Game1.uiViewport.Width;
      int height1 = Game1.uiViewport.Height;
      int width2 = Math.Min(this.width, 1280 /*0x0500*/);
      int height2 = Math.Min(this.height, 920);
      int x1 = Game1.uiViewport.Width / 2 - width2 / 2;
      int y = Game1.uiViewport.Height / 2 - height2 / 2;
      IClickableMenu.drawTextureBox(b, x1, y, width2, height2, Color.White);
      Vector2 location = new Vector2((float) (x1 + 32 /*0x20*/), (float) (y + 32 /*0x20*/));
      for (int index = this.currentTab * this.itemsPerCategoryPage; index < this.currentTab * this.itemsPerCategoryPage + this.itemsPerCategoryPage; ++index)
      {
        if (this.categoryItems[this.currentPage].Count > index)
        {
          Item key = this.categoryItems[this.currentPage][index];
          key.drawInMenu(b, location, 1f, 1f, 1f, StackDrawType.Draw);
          string str = $"{key.DisplayName} x{Game1.content.LoadString("Strings\\StringsFromCSFiles:LoadGameMenu.cs.11020", (object) this.singleItemValues[key])}";
          string s1 = Game1.content.LoadString("Strings\\StringsFromCSFiles:LoadGameMenu.cs.11020", (object) Utility.getNumberWithCommas(this.itemValues[key]));
          string s2 = str;
          int x2 = (int) location.X + width2 - 64 /*0x40*/ - SpriteText.getWidthOfString(s1);
          while (SpriteText.getWidthOfString(s2 + s1) < width2 - 192 /*0xC0*/)
            s2 += " .";
          if (SpriteText.getWidthOfString(s2 + s1) >= width2)
            s2 = s2.Remove(s2.Length - 1);
          SpriteText.drawString(b, s2, (int) location.X + 64 /*0x40*/ + 12, (int) location.Y + 12);
          SpriteText.drawString(b, s1, x2, (int) location.Y + 12);
          location.Y += 68f;
        }
      }
      this.backButton.draw(b);
      if (this.showForwardButton())
        this.forwardButton.draw(b);
    }
    if (this.outro)
    {
      b.Draw(Game1.mouseCursors, new Rectangle(0, 0, Game1.uiViewport.Width, Game1.uiViewport.Height), new Rectangle?(new Rectangle(639, 858, 1, 184)), Color.Black * (float) (1.0 - (double) this.outroFadeTimer / 800.0));
      SpriteText.drawStringWithScrollCenteredAt(b, this.newDayPlaque ? Utility.getDateString() : Utility.getYesterdaysDate(), Game1.uiViewport.Width / 2, this.dayPlaqueY);
      foreach (TemporaryAnimatedSprite animation in this.animations)
        animation.draw(b, true);
      if (this.finalOutroTimer > 0 || this._hasFinished)
        b.Draw(Game1.staminaRect, new Rectangle(0, 0, Game1.uiViewport.Width, Game1.uiViewport.Height), new Rectangle?(new Rectangle(0, 0, 1, 1)), Color.Black * (float) (1.0 - (double) this.finalOutroTimer / 2000.0));
    }
    this.saveGameMenu?.draw(b);
    if (Game1.options.SnappyMenus && (this.introTimer > 0 || this.outro))
      return;
    Game1.mouseCursorTransparency = 1f;
    this.drawMouse(b);
  }
}
