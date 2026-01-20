// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.MuseumMenu
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley.BellsAndWhistles;
using StardewValley.Locations;
using StardewValley.TokenizableStrings;
using System;
using xTile.Dimensions;

#nullable disable
namespace StardewValley.Menus;

public class MuseumMenu : MenuWithInventory
{
  public const int startingState = 0;
  public const int placingInMuseumState = 1;
  public const int exitingState = 2;
  public int fadeTimer;
  public int state;
  public int menuPositionOffset;
  public bool fadeIntoBlack;
  public bool menuMovingDown;
  public float blackFadeAlpha;
  public SparklingText sparkleText;
  public Vector2 globalLocationOfSparklingArtifact;
  /// <summary>The museum for which the menu was opened.</summary>
  public LibraryMuseum Museum;
  private bool holdingMuseumPiece;
  public bool reOrganizing;

  public MuseumMenu(InventoryMenu.highlightThisItem highlighterMethod)
    : base(highlighterMethod, true)
  {
    this.fadeTimer = 800;
    this.fadeIntoBlack = true;
    this.movePosition(0, Game1.uiViewport.Height - this.yPositionOnScreen - this.height);
    Game1.player.forceCanMove();
    this.Museum = Game1.currentLocation is LibraryMuseum currentLocation ? currentLocation : throw new InvalidOperationException("The museum donation menu must be used from within the museum.");
    if (Game1.options.SnappyMenus)
    {
      if (this.okButton != null)
        this.okButton.myID = 106;
      this.populateClickableComponentList();
      this.currentlySnappedComponent = this.getComponentWithID(0);
      this.snapCursorToCurrentSnappedComponent();
    }
    Game1.displayHUD = false;
  }

  public override bool shouldClampGamePadCursor() => true;

  /// <inheritdoc />
  public override void receiveKeyPress(Keys key)
  {
    if (this.fadeTimer > 0)
      return;
    if (Game1.options.doesInputListContain(Game1.options.menuButton, key) && !Game1.isOneOfTheseKeysDown(Game1.oldKBState, Game1.options.menuButton) && this.readyToClose())
    {
      this.state = 2;
      this.fadeTimer = 500;
      this.fadeIntoBlack = true;
    }
    else if (Game1.options.doesInputListContain(Game1.options.menuButton, key) && !Game1.isOneOfTheseKeysDown(Game1.oldKBState, Game1.options.menuButton) && !this.holdingMuseumPiece && this.menuMovingDown)
    {
      if (this.heldItem != null)
      {
        Game1.playSound("bigDeSelect");
        Utility.CollectOrDrop(this.heldItem);
        this.heldItem = (Item) null;
      }
      this.ReturnToDonatableItems();
    }
    else if (Game1.options.SnappyMenus && this.heldItem == null && !this.reOrganizing)
      base.receiveKeyPress(key);
    if (!Game1.options.SnappyMenus)
    {
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
    else
    {
      if (this.heldItem == null && !this.reOrganizing)
        return;
      LibraryMuseum museum = this.Museum;
      Vector2 vector2 = new Vector2((float) (int) (((double) Utility.ModifyCoordinateFromUIScale((float) Game1.getMouseX()) + (double) Game1.viewport.X) / 64.0), (float) (int) (((double) Utility.ModifyCoordinateFromUIScale((float) Game1.getMouseY()) + (double) Game1.viewport.Y) / 64.0));
      if (!museum.isTileSuitableForMuseumPiece((int) vector2.X, (int) vector2.Y) && (!this.reOrganizing || !LibraryMuseum.HasDonatedArtifactAt(vector2)))
      {
        vector2 = museum.getFreeDonationSpot();
        Game1.setMousePosition((int) Utility.ModifyCoordinateForUIScale((float) ((double) vector2.X * 64.0 - (double) Game1.viewport.X + 32.0)), (int) Utility.ModifyCoordinateForUIScale((float) ((double) vector2.Y * 64.0 - (double) Game1.viewport.Y + 32.0)));
      }
      else
      {
        if (key == Game1.options.getFirstKeyboardKeyFromInputButtonList(Game1.options.moveUpButton))
          vector2 = museum.findMuseumPieceLocationInDirection(vector2, 0, 21, !this.reOrganizing);
        else if (key == Game1.options.getFirstKeyboardKeyFromInputButtonList(Game1.options.moveRightButton))
          vector2 = museum.findMuseumPieceLocationInDirection(vector2, 1, 21, !this.reOrganizing);
        else if (key == Game1.options.getFirstKeyboardKeyFromInputButtonList(Game1.options.moveDownButton))
          vector2 = museum.findMuseumPieceLocationInDirection(vector2, 2, 21, !this.reOrganizing);
        else if (key == Game1.options.getFirstKeyboardKeyFromInputButtonList(Game1.options.moveLeftButton))
          vector2 = museum.findMuseumPieceLocationInDirection(vector2, 3, 21, !this.reOrganizing);
        if (!Game1.viewport.Contains(new Location((int) ((double) vector2.X * 64.0 + 32.0), Game1.viewport.Y + 1)))
          Game1.panScreen((int) ((double) vector2.X * 64.0 - (double) Game1.viewport.X), 0);
        else if (!Game1.viewport.Contains(new Location(Game1.viewport.X + 1, (int) ((double) vector2.Y * 64.0 + 32.0))))
          Game1.panScreen(0, (int) ((double) vector2.Y * 64.0 - (double) Game1.viewport.Y));
        Game1.setMousePosition((int) Utility.ModifyCoordinateForUIScale((float) ((int) vector2.X * 64 /*0x40*/ - Game1.viewport.X + 32 /*0x20*/)), (int) Utility.ModifyCoordinateForUIScale((float) ((int) vector2.Y * 64 /*0x40*/ - Game1.viewport.Y + 32 /*0x20*/)));
      }
    }
  }

  /// <inheritdoc />
  public override void receiveGamePadButton(Buttons button)
  {
    if (button != Buttons.DPadUp && button != Buttons.LeftThumbstickUp || this.menuMovingDown || !Game1.options.SnappyMenus || this.currentlySnappedComponent == null || this.currentlySnappedComponent.myID >= 12)
      return;
    this.reOrganizing = true;
    this.menuMovingDown = true;
    this.receiveKeyPress(Game1.options.moveUpButton[0].key);
  }

  /// <inheritdoc />
  public override void receiveLeftClick(int x, int y, bool playSound = true)
  {
    if (this.fadeTimer > 0)
      return;
    if (this.okButton != null && this.okButton.containsPoint(x, y) && this.readyToClose())
    {
      if (this.fadeTimer <= 0)
        Game1.playSound("bigDeSelect");
      this.state = 2;
      this.fadeTimer = 800;
      this.fadeIntoBlack = true;
    }
    else
    {
      Item heldItem1 = this.heldItem;
      if (!this.holdingMuseumPiece)
      {
        if (this.heldItem == null)
        {
          int inventoryPositionOfClick = this.inventory.getInventoryPositionOfClick(x, y);
          Item i = inventoryPositionOfClick < 0 || inventoryPositionOfClick >= this.inventory.actualInventory.Count ? (Item) null : this.inventory.actualInventory[inventoryPositionOfClick];
          if (i != null && this.inventory.highlightMethod(i))
          {
            this.heldItem = i.getOne();
            this.inventory.actualInventory[inventoryPositionOfClick] = i.ConsumeStack(1);
          }
        }
        else
          this.heldItem = this.inventory.leftClick(x, y, this.heldItem);
      }
      if (heldItem1 == null && this.heldItem != null && Game1.isAnyGamePadButtonBeingPressed())
        this.receiveGamePadButton(Buttons.DPadUp);
      if (heldItem1 != null && this.heldItem != null && (y < Game1.viewport.Height - (this.height - (IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + 192 /*0xC0*/)) || this.menuMovingDown))
      {
        Item heldItem2 = this.heldItem;
        LibraryMuseum museum = this.Museum;
        int x1 = (int) ((double) Utility.ModifyCoordinateFromUIScale((float) x) + (double) Game1.viewport.X) / 64 /*0x40*/;
        int y1 = (int) ((double) Utility.ModifyCoordinateFromUIScale((float) y) + (double) Game1.viewport.Y) / 64 /*0x40*/;
        if (museum.isTileSuitableForMuseumPiece(x1, y1) && museum.isItemSuitableForDonation(heldItem2))
        {
          int count = museum.getRewardsForPlayer(Game1.player).Count;
          museum.museumPieces.Add(new Vector2((float) x1, (float) y1), heldItem2.ItemId);
          Game1.playSound("stoneStep");
          if (museum.getRewardsForPlayer(Game1.player).Count > count && !this.holdingMuseumPiece)
          {
            this.sparkleText = new SparklingText(Game1.dialogueFont, Game1.content.LoadString("Strings\\StringsFromCSFiles:NewReward"), Color.MediumSpringGreen, Color.White);
            Game1.playSound("reward");
            this.globalLocationOfSparklingArtifact = new Vector2((float) (x1 * 64 /*0x40*/ + 32 /*0x20*/) - this.sparkleText.textWidth / 2f, (float) (y1 * 64 /*0x40*/ - 48 /*0x30*/));
          }
          else
            Game1.playSound("newArtifact");
          Game1.player.completeQuest("24");
          this.heldItem = heldItem2.ConsumeStack(1);
          int length = museum.museumPieces.Length;
          if (!this.holdingMuseumPiece)
          {
            Game1.stats.checkForArchaeologyAchievements();
            if (length == LibraryMuseum.totalArtifacts)
              Game1.multiplayer.globalChatInfoMessage("MuseumComplete", Game1.player.farmName.Value);
            else if (length == 40)
              Game1.multiplayer.globalChatInfoMessage("Museum40", Game1.player.farmName.Value);
            else
              Game1.multiplayer.globalChatInfoMessage("donation", Game1.player.name.Value, TokenStringBuilder.ItemNameFor(heldItem2));
          }
          this.ReturnToDonatableItems();
        }
      }
      else if (this.heldItem == null && !this.inventory.isWithinBounds(x, y))
      {
        Vector2 key = new Vector2((float) ((int) ((double) Utility.ModifyCoordinateFromUIScale((float) x) + (double) Game1.viewport.X) / 64 /*0x40*/), (float) ((int) ((double) Utility.ModifyCoordinateFromUIScale((float) y) + (double) Game1.viewport.Y) / 64 /*0x40*/));
        LibraryMuseum museum = this.Museum;
        string itemId;
        if (museum.museumPieces.TryGetValue(key, out itemId))
        {
          this.heldItem = ItemRegistry.Create(itemId, allowNull: true);
          museum.museumPieces.Remove(key);
          if (this.heldItem != null)
            this.holdingMuseumPiece = !LibraryMuseum.HasDonatedArtifact(this.heldItem.QualifiedItemId);
        }
      }
      if (this.heldItem == null || heldItem1 != null)
        return;
      this.menuMovingDown = true;
      this.reOrganizing = false;
    }
  }

  public virtual void ReturnToDonatableItems()
  {
    this.menuMovingDown = false;
    this.holdingMuseumPiece = false;
    this.reOrganizing = false;
    if (!Game1.options.SnappyMenus)
      return;
    this.movePosition(0, -this.menuPositionOffset);
    this.menuPositionOffset = 0;
    this.snapCursorToCurrentSnappedComponent();
  }

  public override void emergencyShutDown()
  {
    if (this.heldItem != null && this.holdingMuseumPiece && this.Museum.museumPieces.TryAdd(this.Museum.getFreeDonationSpot(), this.heldItem.ItemId))
    {
      this.heldItem = (Item) null;
      this.holdingMuseumPiece = false;
    }
    base.emergencyShutDown();
  }

  public override bool readyToClose()
  {
    return !this.holdingMuseumPiece && this.heldItem == null && !this.menuMovingDown;
  }

  /// <inheritdoc />
  protected override void cleanupBeforeExit()
  {
    if (this.heldItem != null)
    {
      this.heldItem = Game1.player.addItemToInventory(this.heldItem);
      if (this.heldItem != null)
      {
        Game1.createItemDebris(this.heldItem, Game1.player.Position, -1);
        this.heldItem = (Item) null;
      }
    }
    Game1.displayHUD = true;
  }

  /// <inheritdoc />
  public override void receiveRightClick(int x, int y, bool playSound = true)
  {
    Item heldItem = this.heldItem;
    if (this.fadeTimer <= 0)
      base.receiveRightClick(x, y, true);
    if (this.heldItem == null || heldItem != null)
      return;
    this.menuMovingDown = true;
  }

  /// <inheritdoc />
  public override void update(GameTime time)
  {
    base.update(time);
    if (this.sparkleText != null && this.sparkleText.update(time))
      this.sparkleText = (SparklingText) null;
    if (this.fadeTimer > 0)
    {
      this.fadeTimer -= time.ElapsedGameTime.Milliseconds;
      this.blackFadeAlpha = !this.fadeIntoBlack ? (float) (1.0 - (1500.0 - (double) this.fadeTimer) / 1500.0) : (float) (0.0 + (1500.0 - (double) this.fadeTimer) / 1500.0);
      if (this.fadeTimer <= 0)
      {
        switch (this.state)
        {
          case 0:
            this.state = 1;
            Game1.viewportFreeze = true;
            Game1.viewport.Location = new Location(1152, 128 /*0x80*/);
            Game1.clampViewportToGameMap();
            this.fadeTimer = 800;
            this.fadeIntoBlack = false;
            break;
          case 2:
            Game1.viewportFreeze = false;
            this.fadeIntoBlack = false;
            this.fadeTimer = 800;
            this.state = 3;
            break;
          case 3:
            this.exitThisMenuNoSound();
            break;
        }
      }
    }
    if (this.menuMovingDown && this.menuPositionOffset < this.height / 3)
    {
      this.menuPositionOffset += 8;
      this.movePosition(0, 8);
    }
    else if (!this.menuMovingDown && this.menuPositionOffset > 0)
    {
      this.menuPositionOffset -= 8;
      this.movePosition(0, -8);
    }
    int num1 = Game1.getOldMouseX(false) + Game1.viewport.X;
    int num2 = Game1.getOldMouseY(false) + Game1.viewport.Y;
    if (!Game1.options.SnappyMenus && Game1.lastCursorMotionWasMouse && num1 - Game1.viewport.X < 64 /*0x40*/ || (double) Game1.input.GetGamePadState().ThumbSticks.Right.X < 0.0)
    {
      Game1.panScreen(-4, 0);
      if ((double) Game1.input.GetGamePadState().ThumbSticks.Right.X < 0.0)
        this.snapCursorToCurrentMuseumSpot();
    }
    else if (!Game1.options.SnappyMenus && Game1.lastCursorMotionWasMouse && num1 - (Game1.viewport.X + Game1.viewport.Width) >= -64 || (double) Game1.input.GetGamePadState().ThumbSticks.Right.X > 0.0)
    {
      Game1.panScreen(4, 0);
      if ((double) Game1.input.GetGamePadState().ThumbSticks.Right.X > 0.0)
        this.snapCursorToCurrentMuseumSpot();
    }
    if (!Game1.options.SnappyMenus && Game1.lastCursorMotionWasMouse && num2 - Game1.viewport.Y < 64 /*0x40*/ || (double) Game1.input.GetGamePadState().ThumbSticks.Right.Y > 0.0)
    {
      Game1.panScreen(0, -4);
      if ((double) Game1.input.GetGamePadState().ThumbSticks.Right.Y > 0.0)
        this.snapCursorToCurrentMuseumSpot();
    }
    else if (!Game1.options.SnappyMenus && Game1.lastCursorMotionWasMouse && num2 - (Game1.viewport.Y + Game1.viewport.Height) >= -64 || (double) Game1.input.GetGamePadState().ThumbSticks.Right.Y < 0.0)
    {
      Game1.panScreen(0, 4);
      if ((double) Game1.input.GetGamePadState().ThumbSticks.Right.Y < 0.0)
        this.snapCursorToCurrentMuseumSpot();
    }
    foreach (Keys pressedKey in Game1.oldKBState.GetPressedKeys())
      this.receiveKeyPress(pressedKey);
  }

  private void snapCursorToCurrentMuseumSpot()
  {
    if (!this.menuMovingDown)
      return;
    Vector2 vector2 = new Vector2((float) ((Game1.getMouseX(false) + Game1.viewport.X) / 64 /*0x40*/), (float) ((Game1.getMouseY(false) + Game1.viewport.Y) / 64 /*0x40*/));
    Game1.setMousePosition((int) vector2.X * 64 /*0x40*/ - Game1.viewport.X + 32 /*0x20*/, (int) vector2.Y * 64 /*0x40*/ - Game1.viewport.Y + 32 /*0x20*/, false);
  }

  /// <inheritdoc />
  public override void gameWindowSizeChanged(Microsoft.Xna.Framework.Rectangle oldBounds, Microsoft.Xna.Framework.Rectangle newBounds)
  {
    base.gameWindowSizeChanged(oldBounds, newBounds);
    this.movePosition(0, Game1.viewport.Height - this.yPositionOnScreen - this.height);
    Game1.player.forceCanMove();
  }

  /// <inheritdoc />
  public override void draw(SpriteBatch b)
  {
    if ((this.fadeTimer <= 0 || !this.fadeIntoBlack) && this.state != 3)
    {
      if (this.heldItem != null)
      {
        Game1.StartWorldDrawInUI(b);
        for (int y = Game1.viewport.Y / 64 /*0x40*/ - 1; y < (Game1.viewport.Y + Game1.viewport.Height) / 64 /*0x40*/ + 2; ++y)
        {
          for (int x = Game1.viewport.X / 64 /*0x40*/ - 1; x < (Game1.viewport.X + Game1.viewport.Width) / 64 /*0x40*/ + 1; ++x)
          {
            if (this.Museum.isTileSuitableForMuseumPiece(x, y))
              b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) x, (float) y) * 64f), new Microsoft.Xna.Framework.Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 29)), Color.LightGreen);
          }
        }
        Game1.EndWorldDrawInUI(b);
      }
      if (!this.holdingMuseumPiece)
        this.draw(b, false, false);
      if (!this.hoverText.Equals(""))
        IClickableMenu.drawHoverText(b, this.hoverText, Game1.smallFont);
      this.heldItem?.drawInMenu(b, new Vector2((float) (Game1.getOldMouseX() + 8), (float) (Game1.getOldMouseY() + 8)), 1f);
      this.drawMouse(b);
      this.sparkleText?.draw(b, Utility.ModifyCoordinatesForUIScale(Game1.GlobalToLocal(Game1.viewport, this.globalLocationOfSparklingArtifact)));
    }
    b.Draw(Game1.fadeToBlackRect, new Microsoft.Xna.Framework.Rectangle(0, 0, Game1.uiViewport.Width, Game1.uiViewport.Height), Color.Black * this.blackFadeAlpha);
  }
}
