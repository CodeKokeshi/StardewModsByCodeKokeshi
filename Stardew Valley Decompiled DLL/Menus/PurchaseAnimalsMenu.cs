// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.PurchaseAnimalsMenu
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley.BellsAndWhistles;
using StardewValley.Buildings;
using StardewValley.Extensions;
using StardewValley.GameData.FarmAnimals;
using System;
using System.Collections.Generic;
using xTile.Dimensions;

#nullable disable
namespace StardewValley.Menus;

public class PurchaseAnimalsMenu : IClickableMenu
{
  public const int region_okButton = 101;
  public const int region_doneNamingButton = 102;
  public const int region_randomButton = 103;
  public const int region_namingBox = 104;
  public const int region_upArrow = 105;
  public const int region_downArrow = 106;
  public static int menuHeight = 320;
  public static int menuWidth = 384;
  public int clickedAnimalButton = -1;
  public List<ClickableTextureComponent> animalsToPurchase = new List<ClickableTextureComponent>();
  public ClickableTextureComponent okButton;
  public ClickableTextureComponent doneNamingButton;
  public ClickableTextureComponent randomButton;
  public ClickableTextureComponent upArrow;
  public ClickableTextureComponent downArrow;
  public ClickableTextureComponent hovered;
  public ClickableComponent textBoxCC;
  /// <summary>Whether the menu is currently showing the target location (regardless of whether it's the farm), so the player can choose a building to put animals in.</summary>
  public bool onFarm;
  public bool namingAnimal;
  public bool freeze;
  public FarmAnimal animalBeingPurchased;
  public TextBox textBox;
  public TextBoxEvent textBoxEvent;
  public Building newAnimalHome;
  public int priceOfAnimal;
  public bool readOnly;
  /// <summary>The index of the row shown at the top of the shop menu.</summary>
  public int currentScroll;
  /// <summary>The number of shop rows that are off-screen.</summary>
  public int scrollRows;
  /// <summary>The location in which to construct or manage buildings.</summary>
  public GameLocation TargetLocation;

  /// <summary>Construct an instance.</summary>
  /// <param name="stock">The animals available to purchase.</param>
  /// <param name="targetLocation">The location for which to purchase animals, or <c>null</c> for the farm.</param>
  public PurchaseAnimalsMenu(List<StardewValley.Object> stock, GameLocation targetLocation = null)
    : base(Game1.uiViewport.Width / 2 - PurchaseAnimalsMenu.menuWidth / 2 - IClickableMenu.borderWidth * 2, (Game1.uiViewport.Height - PurchaseAnimalsMenu.menuHeight - IClickableMenu.borderWidth * 2) / 4, PurchaseAnimalsMenu.menuWidth + IClickableMenu.borderWidth * 2 + (PurchaseAnimalsMenu.GetOffScreenRows(stock.Count) > 0 ? 44 : 0), PurchaseAnimalsMenu.menuHeight + IClickableMenu.borderWidth)
  {
    this.height += 64 /*0x40*/;
    this.TargetLocation = targetLocation ?? (GameLocation) Game1.getFarm();
    for (int index = 0; index < stock.Count; ++index)
    {
      FarmAnimalData farmAnimalData;
      Texture2D texture;
      Microsoft.Xna.Framework.Rectangle sourceRect;
      if (Game1.farmAnimalData.TryGetValue(stock[index].Name, out farmAnimalData) && farmAnimalData.ShopTexture != null)
      {
        texture = Game1.content.Load<Texture2D>(farmAnimalData.ShopTexture);
        sourceRect = farmAnimalData.ShopSourceRect;
      }
      else if (index >= 9)
      {
        texture = Game1.mouseCursors2;
        sourceRect = new Microsoft.Xna.Framework.Rectangle(128 /*0x80*/ + index % 3 * 16 /*0x10*/ * 2, index / 3 * 16 /*0x10*/, 32 /*0x20*/, 16 /*0x10*/);
      }
      else
      {
        texture = Game1.mouseCursors;
        sourceRect = new Microsoft.Xna.Framework.Rectangle(index % 3 * 16 /*0x10*/ * 2, 448 + index / 3 * 16 /*0x10*/, 32 /*0x20*/, 16 /*0x10*/);
      }
      ClickableTextureComponent textureComponent = new ClickableTextureComponent(stock[index].salePrice(false).ToString() ?? "", new Microsoft.Xna.Framework.Rectangle(this.xPositionOnScreen + IClickableMenu.borderWidth + index % 3 * 64 /*0x40*/ * 2, this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + IClickableMenu.borderWidth / 2 + index / 3 * 85, 128 /*0x80*/, 64 /*0x40*/), (string) null, stock[index].Name, texture, sourceRect, 4f, stock[index].Type == null);
      textureComponent.item = (Item) stock[index];
      textureComponent.myID = index;
      textureComponent.rightNeighborID = -99998;
      textureComponent.leftNeighborID = -99998;
      textureComponent.downNeighborID = -99998;
      textureComponent.upNeighborID = -99998;
      this.animalsToPurchase.Add(textureComponent);
    }
    this.scrollRows = PurchaseAnimalsMenu.GetOffScreenRows(this.animalsToPurchase.Count);
    if (this.scrollRows < 0)
      this.scrollRows = 0;
    this.RepositionAnimalButtons();
    ClickableTextureComponent textureComponent1 = new ClickableTextureComponent(new Microsoft.Xna.Framework.Rectangle(this.xPositionOnScreen + this.width + 4, this.yPositionOnScreen + this.height - 64 /*0x40*/ - IClickableMenu.borderWidth, 64 /*0x40*/, 64 /*0x40*/), Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 47), 1f);
    textureComponent1.myID = 101;
    textureComponent1.rightNeighborID = -99998;
    textureComponent1.leftNeighborID = -99998;
    textureComponent1.downNeighborID = -99998;
    textureComponent1.upNeighborID = -99998;
    this.okButton = textureComponent1;
    ClickableTextureComponent textureComponent2 = new ClickableTextureComponent(new Microsoft.Xna.Framework.Rectangle(this.xPositionOnScreen + this.width + 51 + 64 /*0x40*/, Game1.uiViewport.Height / 2, 64 /*0x40*/, 64 /*0x40*/), Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(381, 361, 10, 10), 4f);
    textureComponent2.myID = 103;
    textureComponent2.rightNeighborID = -99998;
    textureComponent2.leftNeighborID = -99998;
    textureComponent2.downNeighborID = -99998;
    textureComponent2.upNeighborID = -99998;
    this.randomButton = textureComponent2;
    PurchaseAnimalsMenu.menuHeight = 320;
    PurchaseAnimalsMenu.menuWidth = 384;
    this.textBox = new TextBox((Texture2D) null, (Texture2D) null, Game1.dialogueFont, Game1.textColor);
    this.textBox.X = Game1.uiViewport.Width / 2 - 192 /*0xC0*/;
    this.textBox.Y = Game1.uiViewport.Height / 2;
    this.textBox.Width = 256 /*0x0100*/;
    this.textBox.Height = 192 /*0xC0*/;
    this.textBoxEvent = new TextBoxEvent(this.textBoxEnter);
    this.textBoxCC = new ClickableComponent(new Microsoft.Xna.Framework.Rectangle(this.textBox.X, this.textBox.Y, 192 /*0xC0*/, 48 /*0x30*/), "")
    {
      myID = 104,
      rightNeighborID = -99998,
      leftNeighborID = -99998,
      downNeighborID = -99998,
      upNeighborID = -99998
    };
    ClickableTextureComponent textureComponent3 = new ClickableTextureComponent(new Microsoft.Xna.Framework.Rectangle(this.textBox.X + this.textBox.Width + 64 /*0x40*/ + 48 /*0x30*/ - 8, Game1.uiViewport.Height / 2 + 4, 64 /*0x40*/, 64 /*0x40*/), Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(381, 361, 10, 10), 4f);
    textureComponent3.myID = 103;
    textureComponent3.rightNeighborID = -99998;
    textureComponent3.leftNeighborID = -99998;
    textureComponent3.downNeighborID = -99998;
    textureComponent3.upNeighborID = -99998;
    this.randomButton = textureComponent3;
    ClickableTextureComponent textureComponent4 = new ClickableTextureComponent(new Microsoft.Xna.Framework.Rectangle(this.textBox.X + this.textBox.Width + 32 /*0x20*/ + 4, Game1.uiViewport.Height / 2 - 8, 64 /*0x40*/, 64 /*0x40*/), Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46), 1f);
    textureComponent4.myID = 102;
    textureComponent4.rightNeighborID = -99998;
    textureComponent4.leftNeighborID = -99998;
    textureComponent4.downNeighborID = -99998;
    textureComponent4.upNeighborID = -99998;
    this.doneNamingButton = textureComponent4;
    int x = this.xPositionOnScreen + this.width - 64 /*0x40*/ - 24;
    ClickableTextureComponent textureComponent5 = new ClickableTextureComponent(new Microsoft.Xna.Framework.Rectangle(x, this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + 16 /*0x10*/, 44, 48 /*0x30*/), Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(421, 459, 11, 12), 4f);
    textureComponent5.myID = 105;
    textureComponent5.rightNeighborID = -99998;
    textureComponent5.leftNeighborID = -99998;
    textureComponent5.downNeighborID = -99998;
    textureComponent5.upNeighborID = -99998;
    this.upArrow = textureComponent5;
    ClickableTextureComponent textureComponent6 = new ClickableTextureComponent(new Microsoft.Xna.Framework.Rectangle(x, this.yPositionOnScreen + this.height - 64 /*0x40*/ - 24, 44, 48 /*0x30*/), Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(421, 472, 11, 12), 4f);
    textureComponent6.myID = 106;
    textureComponent6.rightNeighborID = -99998;
    textureComponent6.leftNeighborID = -99998;
    textureComponent6.downNeighborID = -99998;
    textureComponent6.upNeighborID = -99998;
    this.downArrow = textureComponent6;
    this.doneNamingButton.visible = false;
    this.randomButton.visible = false;
    this.textBoxCC.visible = false;
    if (this.scrollRows <= 0)
    {
      this.upArrow.visible = false;
      this.downArrow.visible = false;
    }
    if (!Game1.options.SnappyMenus)
      return;
    this.populateClickableComponentList();
    this.snapToDefaultClickableComponent();
  }

  /// <summary>Get the number of shop rows that are off-screen.</summary>
  /// <param name="animalsToPurchase">The number of animals available to purchase.</param>
  public static int GetOffScreenRows(int animalsToPurchase) => (animalsToPurchase - 1) / 3 + 1 - 3;

  public override bool shouldClampGamePadCursor() => this.onFarm;

  public override void snapToDefaultClickableComponent()
  {
    this.currentlySnappedComponent = this.getComponentWithID(0);
    this.snapCursorToCurrentSnappedComponent();
  }

  public void textBoxEnter(TextBox sender)
  {
    if (!this.namingAnimal)
      return;
    if (Game1.activeClickableMenu == null || !(Game1.activeClickableMenu is PurchaseAnimalsMenu))
    {
      this.textBox.OnEnterPressed -= this.textBoxEvent;
    }
    else
    {
      if (sender.Text.Length < 1)
        return;
      if (Utility.areThereAnyOtherAnimalsWithThisName(sender.Text))
      {
        Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:PurchaseAnimalsMenu.cs.11308"));
      }
      else
      {
        this.textBox.OnEnterPressed -= this.textBoxEvent;
        this.animalBeingPurchased.Name = sender.Text;
        this.animalBeingPurchased.displayName = sender.Text;
        ((AnimalHouse) this.newAnimalHome.GetIndoors()).adoptAnimal(this.animalBeingPurchased);
        this.newAnimalHome = (Building) null;
        this.namingAnimal = false;
        Game1.player.Money -= this.priceOfAnimal;
        this.setUpForReturnAfterPurchasingAnimal();
      }
    }
  }

  public void setUpForReturnAfterPurchasingAnimal()
  {
    LocationRequest locationRequest = Game1.getLocationRequest("AnimalShop");
    locationRequest.OnWarp += (LocationRequest.Callback) (() =>
    {
      this.onFarm = false;
      Game1.player.viewingLocation.Value = (string) null;
      this.okButton.bounds.X = this.xPositionOnScreen + this.width + 4;
      Game1.displayHUD = true;
      Game1.displayFarmer = true;
      this.freeze = false;
      this.textBox.OnEnterPressed -= this.textBoxEvent;
      this.textBox.Selected = false;
      Game1.viewportFreeze = false;
      this.marnieAnimalPurchaseMessage();
    });
    Game1.warpFarmer(locationRequest, Game1.player.TilePoint.X, Game1.player.TilePoint.Y, Game1.player.FacingDirection);
  }

  public void marnieAnimalPurchaseMessage()
  {
    this.exitThisMenu();
    Game1.player.forceCanMove();
    this.freeze = false;
    Game1.DrawDialogue(Game1.getCharacterFromName("Marnie"), this.animalBeingPurchased.isMale() ? "Strings\\StringsFromCSFiles:PurchaseAnimalsMenu.cs.11311" : "Strings\\StringsFromCSFiles:PurchaseAnimalsMenu.cs.11314", (object) this.animalBeingPurchased.displayName);
  }

  public void setUpForAnimalPlacement()
  {
    this.upArrow.visible = false;
    this.downArrow.visible = false;
    Game1.currentLocation.cleanupBeforePlayerExit();
    Game1.displayFarmer = false;
    Game1.currentLocation = this.TargetLocation;
    Game1.player.viewingLocation.Value = this.TargetLocation.NameOrUniqueName;
    Game1.currentLocation.resetForPlayerEntry();
    Game1.globalFadeToClear();
    this.onFarm = true;
    this.freeze = false;
    this.okButton.bounds.X = Game1.uiViewport.Width - 128 /*0x80*/;
    this.okButton.bounds.Y = Game1.uiViewport.Height - 128 /*0x80*/;
    Game1.displayHUD = false;
    Game1.viewportFreeze = true;
    Game1.viewport.Location = new Location(3136, 320);
    Building suggestedBuilding = this.GetSuggestedBuilding(this.animalBeingPurchased);
    if (suggestedBuilding != null)
      Game1.viewport.Location = this.GetTopLeftPixelToCenterBuilding(suggestedBuilding);
    Game1.panScreen(0, 0);
  }

  public void setUpForReturnToShopMenu()
  {
    this.freeze = false;
    if (this.scrollRows > 0)
    {
      this.upArrow.visible = true;
      this.downArrow.visible = true;
    }
    this.doneNamingButton.visible = false;
    this.randomButton.visible = false;
    Game1.displayFarmer = true;
    LocationRequest locationRequest = Game1.getLocationRequest("AnimalShop");
    locationRequest.OnWarp += (LocationRequest.Callback) (() =>
    {
      this.onFarm = false;
      Game1.player.viewingLocation.Value = (string) null;
      this.okButton.bounds.X = this.xPositionOnScreen + this.width + 4;
      this.okButton.bounds.Y = this.yPositionOnScreen + this.height - 64 /*0x40*/ - IClickableMenu.borderWidth;
      Game1.displayHUD = true;
      Game1.viewportFreeze = false;
      this.namingAnimal = false;
      this.textBox.OnEnterPressed -= this.textBoxEvent;
      this.textBox.Selected = false;
      if (!Game1.options.SnappyMenus)
        return;
      this.setCurrentlySnappedComponentTo(this.clickedAnimalButton);
      this.snapCursorToCurrentSnappedComponent();
    });
    Game1.warpFarmer(locationRequest, Game1.player.TilePoint.X, Game1.player.TilePoint.Y, Game1.player.FacingDirection);
  }

  public virtual void Scroll(int offset)
  {
    this.currentScroll += offset;
    if (this.currentScroll < 0)
      this.currentScroll = 0;
    if (this.currentScroll > this.scrollRows)
      this.currentScroll = this.scrollRows;
    this.RepositionAnimalButtons();
  }

  public virtual void RepositionAnimalButtons()
  {
    foreach (ClickableComponent clickableComponent in this.animalsToPurchase)
      clickableComponent.visible = false;
    for (int index1 = 0; index1 < 3; ++index1)
    {
      for (int index2 = 0; index2 < 3; ++index2)
      {
        int index3 = (index1 + this.currentScroll) * 3 + index2;
        if (index3 < this.animalsToPurchase.Count && index3 >= 0)
        {
          ClickableTextureComponent textureComponent = this.animalsToPurchase[index3];
          textureComponent.bounds.X = this.xPositionOnScreen + IClickableMenu.borderWidth + index2 * 64 /*0x40*/ * 2;
          textureComponent.bounds.Y = this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + IClickableMenu.borderWidth / 2 + index1 * 85;
          textureComponent.visible = true;
        }
        else
          break;
      }
    }
  }

  /// <inheritdoc />
  public override void receiveLeftClick(int x, int y, bool playSound = true)
  {
    if (Game1.IsFading() || this.freeze)
      return;
    if (this.upArrow.containsPoint(x, y))
    {
      Game1.playSound("shwip");
      this.Scroll(-1);
    }
    else if (this.downArrow.containsPoint(x, y))
    {
      Game1.playSound("shwip");
      this.Scroll(1);
    }
    if (this.okButton != null && this.okButton.containsPoint(x, y) && this.readyToClose())
    {
      if (this.onFarm)
      {
        this.setUpForReturnToShopMenu();
        Game1.playSound("smallSelect");
      }
      else
      {
        Game1.exitActiveMenu();
        Game1.playSound("bigDeSelect");
      }
    }
    if (this.onFarm)
    {
      Building buildingAt = this.TargetLocation.getBuildingAt(new Vector2((float) (int) (((double) Utility.ModifyCoordinateFromUIScale((float) x) + (double) Game1.viewport.X) / 64.0), (float) (int) (((double) Utility.ModifyCoordinateFromUIScale((float) y) + (double) Game1.viewport.Y) / 64.0)));
      if (!this.namingAnimal && buildingAt?.GetIndoors() is AnimalHouse indoors && !buildingAt.isUnderConstruction())
      {
        if (this.animalBeingPurchased.CanLiveIn(buildingAt))
        {
          if (indoors.isFull())
          {
            Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:PurchaseAnimalsMenu.cs.11321"));
          }
          else
          {
            this.namingAnimal = true;
            this.doneNamingButton.visible = true;
            this.randomButton.visible = true;
            this.textBoxCC.visible = true;
            this.newAnimalHome = buildingAt;
            FarmAnimalData animalData = this.animalBeingPurchased.GetAnimalData();
            if (animalData != null)
            {
              if (animalData.BabySound != null)
                Game1.playSound(animalData.BabySound, new int?(1200 + Game1.random.Next(-200, 201)));
              else if (animalData.Sound != null)
                Game1.playSound(animalData.Sound, new int?(1200 + Game1.random.Next(-200, 201)));
            }
            this.textBox.OnEnterPressed += this.textBoxEvent;
            this.textBox.Text = this.animalBeingPurchased.displayName;
            Game1.keyboardDispatcher.Subscriber = (IKeyboardSubscriber) this.textBox;
            if (Game1.options.SnappyMenus)
            {
              this.currentlySnappedComponent = this.getComponentWithID(104);
              this.snapCursorToCurrentSnappedComponent();
            }
          }
        }
        else
          Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:PurchaseAnimalsMenu.cs.11326", (object) this.animalBeingPurchased.displayType));
      }
      if (!this.namingAnimal)
        return;
      if (this.doneNamingButton.containsPoint(x, y))
      {
        this.textBoxEnter(this.textBox);
        Game1.playSound("smallSelect");
      }
      else if (this.namingAnimal && this.randomButton.containsPoint(x, y))
      {
        this.animalBeingPurchased.Name = Dialogue.randomName();
        this.animalBeingPurchased.displayName = this.animalBeingPurchased.Name;
        this.textBox.Text = this.animalBeingPurchased.displayName;
        this.randomButton.scale = this.randomButton.baseScale;
        Game1.playSound("drumkit6");
      }
      this.textBox.Update();
    }
    else
    {
      foreach (ClickableTextureComponent textureComponent in this.animalsToPurchase)
      {
        if (!this.readOnly && textureComponent.containsPoint(x, y) && (textureComponent.item as StardewValley.Object).Type == null)
        {
          int num = textureComponent.item.salePrice(false);
          if (Game1.player.Money >= num)
          {
            this.clickedAnimalButton = textureComponent.myID;
            Game1.globalFadeToBlack(new Game1.afterFadeFunction(this.setUpForAnimalPlacement));
            Game1.playSound("smallSelect");
            this.onFarm = true;
            string str = textureComponent.hoverText;
            FarmAnimalData farmAnimalData;
            if (Game1.farmAnimalData.TryGetValue(str, out farmAnimalData) && farmAnimalData.AlternatePurchaseTypes != null)
            {
              foreach (AlternatePurchaseAnimals alternatePurchaseType in farmAnimalData.AlternatePurchaseTypes)
              {
                if (GameStateQuery.CheckConditions(alternatePurchaseType.Condition))
                {
                  str = Game1.random.ChooseFrom<string>((IList<string>) alternatePurchaseType.AnimalIds);
                  break;
                }
              }
            }
            this.animalBeingPurchased = new FarmAnimal(str, Game1.multiplayer.getNewID(), Game1.player.UniqueMultiplayerID);
            this.priceOfAnimal = num;
          }
          else
            Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:PurchaseAnimalsMenu.cs.11325"), 3));
        }
      }
    }
  }

  /// <inheritdoc />
  public override bool overrideSnappyMenuCursorMovementBan() => this.onFarm && !this.namingAnimal;

  /// <inheritdoc />
  public override void receiveGamePadButton(Buttons button)
  {
    base.receiveGamePadButton(button);
    if (button != Buttons.B || Game1.globalFade || !this.onFarm || !this.namingAnimal)
      return;
    this.setUpForReturnToShopMenu();
    Game1.playSound("smallSelect");
  }

  public override void gamePadButtonHeld(Buttons b)
  {
    base.gamePadButtonHeld(b);
    switch (b)
    {
      case Buttons.DPadUp:
      case Buttons.DPadDown:
      case Buttons.DPadLeft:
      case Buttons.DPadRight:
        if (!this.onFarm || this.namingAnimal)
          break;
        GamePadState gamePadState = Game1.input.GetGamePadState();
        MouseState mouseState = Game1.input.GetMouseState();
        int num1 = 12 + (gamePadState.IsButtonDown(Buttons.RightTrigger) || gamePadState.IsButtonDown(Buttons.RightShoulder) ? 8 : 0);
        int num2;
        switch (b)
        {
          case Buttons.DPadLeft:
            num2 = -num1;
            break;
          case Buttons.DPadRight:
            num2 = num1;
            break;
          default:
            num2 = 0;
            break;
        }
        int num3 = num2;
        int num4;
        switch (b)
        {
          case Buttons.DPadUp:
            num4 = -num1;
            break;
          case Buttons.DPadDown:
            num4 = num1;
            break;
          default:
            num4 = 0;
            break;
        }
        int num5 = num4;
        Game1.setMousePositionRaw(mouseState.X + num3, mouseState.Y + num5);
        break;
    }
  }

  /// <inheritdoc />
  public override void receiveKeyPress(Keys key)
  {
    if (Game1.globalFade || this.freeze)
      return;
    if (!Game1.globalFade && this.onFarm)
    {
      if (!this.namingAnimal)
      {
        if (Game1.options.doesInputListContain(Game1.options.menuButton, key) && this.readyToClose() && !Game1.IsFading())
        {
          this.setUpForReturnToShopMenu();
        }
        else
        {
          if (Game1.options.SnappyMenus)
            return;
          if (Game1.options.doesInputListContain(Game1.options.moveDownButton, key))
            Game1.panScreen(0, 4);
          else if (Game1.options.doesInputListContain(Game1.options.moveRightButton, key))
            Game1.panScreen(4, 0);
          else if (Game1.options.doesInputListContain(Game1.options.moveUpButton, key))
          {
            Game1.panScreen(0, -4);
          }
          else
          {
            if (!Game1.options.doesInputListContain(Game1.options.moveLeftButton, key))
              return;
            Game1.panScreen(-4, 0);
          }
        }
      }
      else
      {
        if (!Game1.options.SnappyMenus)
          return;
        if (!this.textBox.Selected && Game1.options.doesInputListContain(Game1.options.menuButton, key))
        {
          this.setUpForReturnToShopMenu();
          Game1.playSound("smallSelect");
        }
        else
        {
          if (this.textBox.Selected && Game1.options.doesInputListContain(Game1.options.menuButton, key))
            return;
          base.receiveKeyPress(key);
        }
      }
    }
    else if (Game1.options.doesInputListContain(Game1.options.menuButton, key) && !Game1.IsFading())
    {
      if (!this.readyToClose())
        return;
      Game1.player.forceCanMove();
      Game1.exitActiveMenu();
      Game1.playSound("bigDeSelect");
    }
    else
    {
      if (!Game1.options.SnappyMenus)
        return;
      base.receiveKeyPress(key);
    }
  }

  /// <inheritdoc />
  public override void update(GameTime time)
  {
    base.update(time);
    if (!this.onFarm)
    {
      this.upArrow.visible = this.currentScroll > 0;
      this.downArrow.visible = this.currentScroll < this.scrollRows;
    }
    else
    {
      if (this.namingAnimal)
        return;
      int num1 = Game1.getOldMouseX(false) + Game1.viewport.X;
      int num2 = Game1.getOldMouseY(false) + Game1.viewport.Y;
      if (num1 - Game1.viewport.X < 64 /*0x40*/)
        Game1.panScreen(-8, 0);
      else if (num1 - (Game1.viewport.X + Game1.viewport.Width) >= -64)
        Game1.panScreen(8, 0);
      if (num2 - Game1.viewport.Y < 64 /*0x40*/)
        Game1.panScreen(0, -8);
      else if (num2 - (Game1.viewport.Y + Game1.viewport.Height) >= -64)
        Game1.panScreen(0, 8);
      foreach (Keys pressedKey in Game1.oldKBState.GetPressedKeys())
        this.receiveKeyPress(pressedKey);
    }
  }

  /// <inheritdoc />
  public override void performHoverAction(int x, int y)
  {
    this.hovered = (ClickableTextureComponent) null;
    if (Game1.IsFading() || this.freeze)
      return;
    this.upArrow.tryHover(x, y);
    this.downArrow.tryHover(x, y);
    if (this.okButton != null)
    {
      if (this.okButton.containsPoint(x, y))
        this.okButton.scale = Math.Min(1.1f, this.okButton.scale + 0.05f);
      else
        this.okButton.scale = Math.Max(1f, this.okButton.scale - 0.05f);
    }
    if (this.onFarm)
    {
      if (!this.namingAnimal)
      {
        Vector2 tile = new Vector2((float) (int) (((double) Utility.ModifyCoordinateFromUIScale((float) x) + (double) Game1.viewport.X) / 64.0), (float) (int) (((double) Utility.ModifyCoordinateFromUIScale((float) y) + (double) Game1.viewport.Y) / 64.0));
        GameLocation targetLocation = this.TargetLocation;
        foreach (Building building in targetLocation.buildings)
          building.color = Color.White;
        Building buildingAt = targetLocation.getBuildingAt(tile);
        if (buildingAt?.GetIndoors() is AnimalHouse indoors)
          buildingAt.color = !this.animalBeingPurchased.CanLiveIn(buildingAt) || indoors.isFull() ? Color.Red * 0.8f : Color.LightGreen * 0.8f;
      }
      if (this.doneNamingButton != null)
      {
        if (this.doneNamingButton.containsPoint(x, y))
          this.doneNamingButton.scale = Math.Min(1.1f, this.doneNamingButton.scale + 0.05f);
        else
          this.doneNamingButton.scale = Math.Max(1f, this.doneNamingButton.scale - 0.05f);
      }
      this.randomButton.tryHover(x, y, 0.5f);
    }
    else
    {
      foreach (ClickableTextureComponent textureComponent in this.animalsToPurchase)
      {
        if (textureComponent.containsPoint(x, y))
        {
          textureComponent.scale = Math.Min(textureComponent.scale + 0.05f, 4.1f);
          this.hovered = textureComponent;
        }
        else
          textureComponent.scale = Math.Max(4f, textureComponent.scale - 0.025f);
      }
    }
  }

  /// <inheritdoc />
  public override void draw(SpriteBatch b)
  {
    if (!this.onFarm && !Game1.dialogueUp && !Game1.IsFading())
    {
      if (!Game1.options.showClearBackgrounds)
        b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
      SpriteText.drawStringWithScrollBackground(b, Game1.content.LoadString("Strings\\StringsFromCSFiles:PurchaseAnimalsMenu.cs.11354"), this.xPositionOnScreen + 96 /*0x60*/, this.yPositionOnScreen);
      Game1.drawDialogueBox(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height, false, true);
      Game1.dayTimeMoneyBox.drawMoneyBox(b);
      this.upArrow.draw(b);
      this.downArrow.draw(b);
      foreach (ClickableTextureComponent textureComponent in this.animalsToPurchase)
        textureComponent.draw(b, (textureComponent.item as StardewValley.Object).Type != null ? Color.Black * 0.4f : Color.White, 0.87f);
    }
    else if (!Game1.IsFading() && this.onFarm)
    {
      string s = Game1.content.LoadString("Strings\\StringsFromCSFiles:PurchaseAnimalsMenu.cs.11355", (object) this.animalBeingPurchased.displayHouse, (object) this.animalBeingPurchased.displayType);
      SpriteText.drawStringWithScrollBackground(b, s, Game1.uiViewport.Width / 2 - SpriteText.getWidthOfString(s) / 2, 16 /*0x10*/);
      if (this.namingAnimal)
      {
        if (!Game1.options.showClearBackgrounds)
          b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
        Game1.drawDialogueBox(Game1.uiViewport.Width / 2 - 256 /*0x0100*/, Game1.uiViewport.Height / 2 - 192 /*0xC0*/ - 32 /*0x20*/, 512 /*0x0200*/, 192 /*0xC0*/, false, true);
        Utility.drawTextWithShadow(b, Game1.content.LoadString("Strings\\StringsFromCSFiles:PurchaseAnimalsMenu.cs.11357"), Game1.dialogueFont, new Vector2((float) (Game1.uiViewport.Width / 2 - 256 /*0x0100*/ + 32 /*0x20*/ + 8), (float) (Game1.uiViewport.Height / 2 - 128 /*0x80*/ + 8)), Game1.textColor);
        this.textBox.Draw(b);
        this.doneNamingButton.draw(b);
        this.randomButton.draw(b);
      }
    }
    if (!Game1.IsFading() && this.okButton != null)
      this.okButton.draw(b);
    if (this.hovered != null)
    {
      if ((this.hovered.item as StardewValley.Object).Type != null)
      {
        IClickableMenu.drawHoverText(b, Game1.parseText((this.hovered.item as StardewValley.Object).Type, Game1.dialogueFont, 320), Game1.dialogueFont);
      }
      else
      {
        string displayName = FarmAnimal.GetDisplayName(this.hovered.hoverText, true);
        SpriteText.drawStringWithScrollBackground(b, displayName, this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + 64 /*0x40*/, this.yPositionOnScreen + this.height - 32 /*0x20*/ + IClickableMenu.spaceToClearTopBorder / 2 + 8, "Truffle Pig");
        SpriteText.drawStringWithScrollBackground(b, "$" + Game1.content.LoadString("Strings\\StringsFromCSFiles:LoadGameMenu.cs.11020", (object) this.hovered.item.salePrice(false)), this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + 128 /*0x80*/, this.yPositionOnScreen + this.height + 64 /*0x40*/ + IClickableMenu.spaceToClearTopBorder / 2 + 8, "$99999999g", Game1.player.Money >= this.hovered.item.salePrice(false) ? 1f : 0.5f);
        string shopDescription = FarmAnimal.GetShopDescription(this.hovered.hoverText);
        IClickableMenu.drawHoverText(b, Game1.parseText(shopDescription, Game1.smallFont, 320), Game1.smallFont, moneyAmountToDisplayAtBottom: this.hovered.item.salePrice(false), boldTitleText: displayName);
      }
    }
    Game1.mouseCursorTransparency = Game1.IsFading() ? 0.0f : 1f;
    this.drawMouse(b);
  }

  /// <summary>Get a suggested building to preselect when opening the menu.</summary>
  /// <param name="animal">The farm animal being placed.</param>
  /// <returns>Returns a building which has room for the animal, else a building which could accept the animal if it wasn't full, else null.</returns>
  public Building GetSuggestedBuilding(FarmAnimal animal)
  {
    Building suggestedBuilding = (Building) null;
    foreach (Building building in this.TargetLocation.buildings)
    {
      if (this.animalBeingPurchased.CanLiveIn(building))
      {
        suggestedBuilding = building;
        if (building.GetIndoors() is AnimalHouse indoors && !indoors.isFull())
          return suggestedBuilding;
      }
    }
    return suggestedBuilding;
  }

  /// <summary>Get the pixel position relative to the top-left corner of the map at which to set the viewpoint so a given building is centered on screen.</summary>
  /// <param name="building">The building to center on screen.</param>
  public Location GetTopLeftPixelToCenterBuilding(Building building)
  {
    Vector2 centeringOnScreen = Utility.getTopLeftPositionForCenteringOnScreen(Game1.viewport, building.tilesWide.Value * 64 /*0x40*/, building.tilesHigh.Value * 64 /*0x40*/);
    return new Location(building.tileX.Value * 64 /*0x40*/ - (int) centeringOnScreen.X, building.tileY.Value * 64 /*0x40*/ - (int) centeringOnScreen.Y);
  }
}
