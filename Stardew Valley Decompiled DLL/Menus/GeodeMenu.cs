// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.GeodeMenu
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace StardewValley.Menus;

public class GeodeMenu : MenuWithInventory
{
  public const int region_geodeSpot = 998;
  public ClickableComponent geodeSpot;
  public AnimatedSprite clint;
  public TemporaryAnimatedSprite geodeDestructionAnimation;
  public TemporaryAnimatedSprite sparkle;
  public int geodeAnimationTimer;
  public int yPositionOfGem;
  public int alertTimer;
  public float delayBeforeShowArtifactTimer;
  public Item geodeTreasure;
  public Item geodeTreasureOverride;
  public bool waitingForServerResponse;
  private TemporaryAnimatedSpriteList fluffSprites = new TemporaryAnimatedSpriteList();

  public GeodeMenu()
    : base(okButton: true, trashCan: true, inventoryXOffset: 12, inventoryYOffset: 132)
  {
    if (this.yPositionOnScreen == IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder)
      this.movePosition(0, -IClickableMenu.spaceToClearTopBorder);
    this.inventory.highlightMethod = new InventoryMenu.highlightThisItem(this.highlightGeodes);
    this.geodeSpot = new ClickableComponent(new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth / 2, this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + 4, 560, 308), "")
    {
      myID = 998,
      downNeighborID = 0
    };
    this.clint = new AnimatedSprite("Characters\\Clint", 8, 32 /*0x20*/, 48 /*0x30*/);
    List<ClickableComponent> inventory = this.inventory.inventory;
    // ISSUE: explicit non-virtual call
    if ((inventory != null ? (__nonvirtual (inventory.Count) >= 12 ? 1 : 0) : 0) != 0)
    {
      for (int index = 0; index < 12; ++index)
      {
        if (this.inventory.inventory[index] != null)
          this.inventory.inventory[index].upNeighborID = 998;
      }
    }
    if (this.trashCan != null)
      this.trashCan.myID = 106;
    if (this.okButton != null)
      this.okButton.leftNeighborID = 11;
    if (!Game1.options.SnappyMenus)
      return;
    this.populateClickableComponentList();
    this.snapToDefaultClickableComponent();
  }

  public override void snapToDefaultClickableComponent()
  {
    this.currentlySnappedComponent = this.getComponentWithID(0);
    this.snapCursorToCurrentSnappedComponent();
  }

  public override bool readyToClose()
  {
    return base.readyToClose() && this.geodeAnimationTimer <= 0 && this.heldItem == null && !this.waitingForServerResponse;
  }

  public bool highlightGeodes(Item i) => this.heldItem != null || Utility.IsGeode(i);

  public virtual void startGeodeCrack()
  {
    this.geodeSpot.item = this.heldItem.getOne();
    this.heldItem = this.heldItem.ConsumeStack(1);
    this.geodeAnimationTimer = 2700;
    Game1.player.Money -= 25;
    Game1.playSound("stoneStep");
    this.clint.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
    {
      new FarmerSprite.AnimationFrame(8, 300),
      new FarmerSprite.AnimationFrame(9, 200),
      new FarmerSprite.AnimationFrame(10, 80 /*0x50*/),
      new FarmerSprite.AnimationFrame(11, 200),
      new FarmerSprite.AnimationFrame(12, 100),
      new FarmerSprite.AnimationFrame(8, 300)
    });
    this.clint.loop = false;
  }

  /// <inheritdoc />
  public override void receiveLeftClick(int x, int y, bool playSound = true)
  {
    if (this.waitingForServerResponse)
      return;
    base.receiveLeftClick(x, y, true);
    if (!this.geodeSpot.containsPoint(x, y))
      return;
    if (this.heldItem != null && Utility.IsGeode(this.heldItem) && Game1.player.Money >= 25 && this.geodeAnimationTimer <= 0)
    {
      int num = Game1.player.freeSpotsInInventory();
      if (num > 1 || num == 1 && this.heldItem.Stack == 1)
      {
        if (this.heldItem.QualifiedItemId == "(O)791" && !Game1.netWorldState.Value.GoldenCoconutCracked)
        {
          this.waitingForServerResponse = true;
          Game1.player.team.goldenCoconutMutex.RequestLock((Action) (() =>
          {
            this.waitingForServerResponse = false;
            this.geodeTreasureOverride = ItemRegistry.Create("(O)73");
            this.startGeodeCrack();
          }), (Action) (() =>
          {
            this.waitingForServerResponse = false;
            this.startGeodeCrack();
          }));
        }
        else
          this.startGeodeCrack();
      }
      else
      {
        this.descriptionText = Game1.content.LoadString("Strings\\UI:GeodeMenu_InventoryFull");
        this.wiggleWordsTimer = 500;
        this.alertTimer = 1500;
      }
    }
    else
    {
      if (Game1.player.Money >= 25)
        return;
      this.wiggleWordsTimer = 500;
      Game1.dayTimeMoneyBox.moneyShakeTimer = 1000;
    }
  }

  /// <inheritdoc />
  public override void receiveRightClick(int x, int y, bool playSound = true)
  {
    base.receiveRightClick(x, y, true);
  }

  /// <inheritdoc />
  public override void performHoverAction(int x, int y)
  {
    if (this.alertTimer > 0)
      return;
    base.performHoverAction(x, y);
    if (!this.descriptionText.Equals(""))
      return;
    if (Game1.player.Money < 25)
      this.descriptionText = Game1.content.LoadString("Strings\\UI:GeodeMenu_Description_NotEnoughMoney");
    else
      this.descriptionText = Game1.content.LoadString("Strings\\UI:GeodeMenu_Description");
  }

  public override void emergencyShutDown()
  {
    base.emergencyShutDown();
    if (this.heldItem == null)
      return;
    Game1.player.addItemToInventoryBool(this.heldItem);
  }

  /// <inheritdoc />
  public override void update(GameTime time)
  {
    base.update(time);
    this.fluffSprites.RemoveWhere<TemporaryAnimatedSprite>((Predicate<TemporaryAnimatedSprite>) (sprite => sprite.update(time)));
    if (this.alertTimer > 0)
      this.alertTimer -= time.ElapsedGameTime.Milliseconds;
    if (this.geodeAnimationTimer <= 0)
      return;
    Game1.MusicDuckTimer = 1500f;
    this.geodeAnimationTimer -= time.ElapsedGameTime.Milliseconds;
    if (this.geodeAnimationTimer <= 0)
    {
      this.geodeDestructionAnimation = (TemporaryAnimatedSprite) null;
      this.geodeSpot.item = (Item) null;
      if (this.geodeTreasure?.QualifiedItemId == "(O)73")
        Game1.netWorldState.Value.GoldenCoconutCracked = true;
      Game1.player.addItemToInventoryBool(this.geodeTreasure);
      this.geodeTreasure = (Item) null;
      this.yPositionOfGem = 0;
      this.fluffSprites.Clear();
      this.delayBeforeShowArtifactTimer = 0.0f;
    }
    else
    {
      int currentFrame = this.clint.currentFrame;
      this.clint.animateOnce(time);
      if (this.clint.currentFrame == 11 && currentFrame != 11)
      {
        if (this.geodeSpot.item?.QualifiedItemId == "(O)275" || this.geodeSpot.item?.QualifiedItemId == "(O)MysteryBox" || this.geodeSpot.item?.QualifiedItemId == "(O)GoldenMysteryBox")
        {
          Game1.playSound("hammer");
          Game1.playSound("woodWhack");
        }
        else
        {
          Game1.playSound("hammer");
          Game1.playSound("stoneCrack");
        }
        ++Game1.stats.GeodesCracked;
        if (this.geodeSpot.item?.QualifiedItemId == "(O)MysteryBox" || this.geodeSpot.item?.QualifiedItemId == "(O)GoldenMysteryBox")
        {
          int num1 = (int) Game1.stats.Increment("MysteryBoxesOpened");
        }
        int y = 448;
        if (this.geodeSpot.item != null)
        {
          switch (this.geodeSpot.item.QualifiedItemId)
          {
            case "(O)536":
              y += 64 /*0x40*/;
              break;
            case "(O)537":
              y += 128 /*0x80*/;
              break;
          }
          this.geodeDestructionAnimation = new TemporaryAnimatedSprite("TileSheets\\animations", new Rectangle(0, y, 64 /*0x40*/, 64 /*0x40*/), 100f, 8, 0, new Vector2((float) (this.geodeSpot.bounds.X + 392 - 32 /*0x20*/), (float) (this.geodeSpot.bounds.Y + 192 /*0xC0*/ - 32 /*0x20*/)), false, false);
          switch (this.geodeSpot.item?.QualifiedItemId)
          {
            case "(O)275":
              this.geodeDestructionAnimation = new TemporaryAnimatedSprite()
              {
                texture = Game1.temporaryContent.Load<Texture2D>("LooseSprites//temporary_sprites_1"),
                sourceRect = new Rectangle(388, 123, 18, 21),
                sourceRectStartingPos = new Vector2(388f, 123f),
                animationLength = 6,
                position = new Vector2((float) (this.geodeSpot.bounds.X + 380 - 32 /*0x20*/), (float) (this.geodeSpot.bounds.Y + 192 /*0xC0*/ - 32 /*0x20*/)),
                holdLastFrame = true,
                interval = 100f,
                id = 777,
                scale = 4f
              };
              for (int index = 0; index < 6; ++index)
              {
                this.fluffSprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(372, 1956, 10, 10), new Vector2((float) (this.geodeSpot.bounds.X + 392 - 32 /*0x20*/ + Game1.random.Next(21)), (float) (this.geodeSpot.bounds.Y + 192 /*0xC0*/ - 16 /*0x10*/)), false, 1f / 500f, new Color((int) byte.MaxValue, 222, 198))
                {
                  alphaFade = 0.02f,
                  motion = new Vector2((float) Game1.random.Next(-20, 21) / 10f, (float) Game1.random.Next(5, 20) / 10f),
                  interval = 99999f,
                  layerDepth = 0.9f,
                  scale = 3f,
                  scaleChange = 0.01f,
                  rotationChange = (float) ((double) Game1.random.Next(-5, 6) * 3.1415927410125732 / 256.0),
                  delayBeforeAnimationStart = index * 20
                });
                this.fluffSprites.Add(new TemporaryAnimatedSprite()
                {
                  texture = Game1.temporaryContent.Load<Texture2D>("LooseSprites//temporary_sprites_1"),
                  sourceRect = new Rectangle(499, 132, 5, 5),
                  sourceRectStartingPos = new Vector2(499f, 132f),
                  motion = new Vector2((float) Game1.random.Next(-30, 31 /*0x1F*/) / 10f, (float) Game1.random.Next(-7, -4)),
                  acceleration = new Vector2(0.0f, 0.25f),
                  totalNumberOfLoops = 1,
                  interval = 1000f,
                  alphaFade = 0.015f,
                  animationLength = 1,
                  layerDepth = 1f,
                  scale = 4f,
                  rotationChange = (float) ((double) Game1.random.Next(-5, 6) * 3.1415927410125732 / 256.0),
                  delayBeforeAnimationStart = index * 10,
                  position = new Vector2((float) (this.geodeSpot.bounds.X + 392 - 32 /*0x20*/ + Game1.random.Next(21)), (float) (this.geodeSpot.bounds.Y + 192 /*0xC0*/ - 16 /*0x10*/))
                });
                this.delayBeforeShowArtifactTimer = 500f;
              }
              break;
            case "(O)MysteryBox":
            case "(O)GoldenMysteryBox":
              this.geodeDestructionAnimation = new TemporaryAnimatedSprite()
              {
                texture = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\Cursors_1_6"),
                sourceRect = new Rectangle(this.geodeSpot.item?.QualifiedItemId == "(O)GoldenMysteryBox" ? 256 /*0x0100*/ : 0, 27, 24, 24),
                sourceRectStartingPos = new Vector2(this.geodeSpot.item?.QualifiedItemId == "(O)GoldenMysteryBox" ? 256f : 0.0f, 27f),
                animationLength = 8,
                position = new Vector2((float) (this.geodeSpot.bounds.X + 380 - 48 /*0x30*/), (float) (this.geodeSpot.bounds.Y + 192 /*0xC0*/ - 48 /*0x30*/)),
                holdLastFrame = true,
                interval = 100f,
                id = 777,
                scale = 4f
              };
              for (int index = 0; index < 6; ++index)
              {
                this.fluffSprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(372, 1956, 10, 10), new Vector2((float) (this.geodeSpot.bounds.X + 392 - 48 /*0x30*/ + Game1.random.Next(32 /*0x20*/)), (float) (this.geodeSpot.bounds.Y + 192 /*0xC0*/ - 24)), false, 1f / 500f, new Color((int) byte.MaxValue, 222, 198))
                {
                  alphaFade = 0.02f,
                  motion = new Vector2((float) Game1.random.Next(-20, 21) / 10f, (float) Game1.random.Next(5, 20) / 10f),
                  interval = 99999f,
                  layerDepth = 0.9f,
                  scale = 3f,
                  scaleChange = 0.01f,
                  rotationChange = (float) ((double) Game1.random.Next(-5, 6) * 3.1415927410125732 / 256.0),
                  delayBeforeAnimationStart = index * 20
                });
                int num2 = Game1.random.Next(3);
                this.fluffSprites.Add(new TemporaryAnimatedSprite()
                {
                  texture = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\Cursors_1_6"),
                  sourceRect = new Rectangle((this.geodeSpot.item?.QualifiedItemId == "(O)GoldenMysteryBox" ? 15 : 0) + num2 * 5, 52, 5, 5),
                  sourceRectStartingPos = new Vector2((float) (num2 * 5), 75f),
                  motion = new Vector2((float) Game1.random.Next(-30, 31 /*0x1F*/) / 10f, (float) Game1.random.Next(-7, -4)),
                  acceleration = new Vector2(0.0f, 0.25f),
                  totalNumberOfLoops = 1,
                  interval = 1000f,
                  alphaFade = 0.015f,
                  animationLength = 1,
                  layerDepth = 1f,
                  scale = 4f,
                  rotationChange = (float) ((double) Game1.random.Next(-5, 6) * 3.1415927410125732 / 256.0),
                  delayBeforeAnimationStart = index * 10,
                  position = new Vector2((float) (this.geodeSpot.bounds.X + 392 - 48 /*0x30*/ + Game1.random.Next(32 /*0x20*/)), (float) (this.geodeSpot.bounds.Y + 192 /*0xC0*/ - 24))
                });
                this.delayBeforeShowArtifactTimer = 500f;
              }
              break;
          }
          if (this.geodeTreasureOverride != null)
          {
            this.geodeTreasure = this.geodeTreasureOverride;
            this.geodeTreasureOverride = (Item) null;
          }
          else
            this.geodeTreasure = Utility.getTreasureFromGeode(this.geodeSpot.item);
          if (!(this.geodeSpot.item.QualifiedItemId == "(O)275") && (!(this.geodeTreasure is StardewValley.Object geodeTreasure1) || !(geodeTreasure1.Type == "Minerals")) && this.geodeTreasure is StardewValley.Object geodeTreasure2 && geodeTreasure2.Type == "Arch" && !Game1.player.hasOrWillReceiveMail("artifactFound"))
            this.geodeTreasure = ItemRegistry.Create("(O)390", 5);
        }
      }
      if (this.geodeDestructionAnimation != null && (this.geodeDestructionAnimation.id != 777 && this.geodeDestructionAnimation.currentParentTileIndex < 7 || this.geodeDestructionAnimation.id == 777 && this.geodeDestructionAnimation.currentParentTileIndex < 5))
      {
        this.geodeDestructionAnimation.update(time);
        if ((double) this.delayBeforeShowArtifactTimer > 0.0)
        {
          this.delayBeforeShowArtifactTimer -= (float) time.ElapsedGameTime.TotalMilliseconds;
          if ((double) this.delayBeforeShowArtifactTimer <= 0.0)
          {
            this.fluffSprites.Add(this.geodeDestructionAnimation);
            this.fluffSprites.Reverse<TemporaryAnimatedSprite>();
            this.geodeDestructionAnimation = new TemporaryAnimatedSprite()
            {
              interval = 100f,
              animationLength = 6,
              alpha = 1f / 1000f,
              id = 777
            };
          }
        }
        else
        {
          if (this.geodeDestructionAnimation.currentParentTileIndex < 3)
            --this.yPositionOfGem;
          --this.yPositionOfGem;
          if (this.geodeDestructionAnimation.currentParentTileIndex == 7 || this.geodeDestructionAnimation.id == 777 && this.geodeDestructionAnimation.currentParentTileIndex == 5)
          {
            if (!(this.geodeTreasure is StardewValley.Object geodeTreasure) || geodeTreasure.price.Value > 75 || this.geodeSpot.item?.QualifiedItemId == "(O)MysteryBox" || this.geodeSpot.item?.QualifiedItemId == "(O)GoldenMysteryBox")
            {
              if (this.geodeSpot.item != null)
                this.sparkle = new TemporaryAnimatedSprite("TileSheets\\animations", new Rectangle(0, 640, 64 /*0x40*/, 64 /*0x40*/), 100f, 8, 0, new Vector2((float) (this.geodeSpot.bounds.X + (this.geodeSpot.item.itemId.Value == "MysteryBox" ? 94 : 98) * 4 - 32 /*0x20*/), (float) (this.geodeSpot.bounds.Y + 192 /*0xC0*/ + this.yPositionOfGem - 32 /*0x20*/)), false, false);
              Game1.playSound("discoverMineral");
            }
            else
              Game1.playSound("newArtifact");
          }
        }
      }
      if (this.sparkle == null || !this.sparkle.update(time))
        return;
      this.sparkle = (TemporaryAnimatedSprite) null;
    }
  }

  /// <inheritdoc />
  public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
  {
    base.gameWindowSizeChanged(oldBounds, newBounds);
    Vector2 centeringOnScreen = Utility.getTopLeftPositionForCenteringOnScreen(this.width, this.height);
    this.xPositionOnScreen = (int) centeringOnScreen.X;
    this.yPositionOnScreen = (int) centeringOnScreen.Y;
    Item obj = this.geodeSpot.item;
    this.geodeSpot = new ClickableComponent(new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth / 2, this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + 4, 560, 308), "Anvil");
    this.geodeSpot.item = obj;
    int yPosition = this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + IClickableMenu.borderWidth + 192 /*0xC0*/ - 16 /*0x10*/ + 128 /*0x80*/ + 4;
    if (this.okButton != null)
    {
      ClickableTextureComponent textureComponent = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + this.width + 4, this.yPositionOnScreen + this.height - 192 /*0xC0*/ - IClickableMenu.borderWidth, 64 /*0x40*/, 64 /*0x40*/), Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46), 1f);
      textureComponent.myID = 4857;
      textureComponent.upNeighborID = 5948;
      textureComponent.leftNeighborID = 12;
      this.okButton = textureComponent;
    }
    if (this.trashCan != null)
    {
      ClickableTextureComponent textureComponent = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + this.width + 4, this.yPositionOnScreen + this.height - 192 /*0xC0*/ - 32 /*0x20*/ - IClickableMenu.borderWidth - 104, 64 /*0x40*/, 104), Game1.mouseCursors, new Rectangle(564 + Game1.player.trashCanLevel * 18, 102, 18, 26), 4f);
      textureComponent.myID = 5948;
      textureComponent.downNeighborID = 4857;
      textureComponent.leftNeighborID = 12;
      textureComponent.upNeighborID = 106;
      this.trashCan = textureComponent;
    }
    this.inventory = new InventoryMenu(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth / 2 + 12, yPosition, false, highlightMethod: this.inventory.highlightMethod);
  }

  /// <inheritdoc />
  public override void draw(SpriteBatch b)
  {
    if (!Game1.options.showClearBackgrounds)
      b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.4f);
    this.draw(b, true, true, -1, -1, -1);
    Game1.dayTimeMoneyBox.drawMoneyBox(b);
    b.Draw(Game1.mouseCursors, new Vector2((float) this.geodeSpot.bounds.X, (float) this.geodeSpot.bounds.Y), new Rectangle?(new Rectangle(0, 512 /*0x0200*/, 140, 78)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.87f);
    if (this.geodeSpot.item != null)
    {
      if (this.geodeDestructionAnimation == null)
      {
        Vector2 vector2 = Vector2.Zero;
        if (this.geodeSpot.item.QualifiedItemId == "(O)275")
          vector2 = new Vector2(-2f, 2f);
        else if (this.geodeSpot.item.QualifiedItemId == "(O)MysteryBox" || this.geodeSpot.item?.QualifiedItemId == "(O)GoldenMysteryBox")
          vector2 = new Vector2(-7f, 4f);
        int num = this.geodeSpot.item.QualifiedItemId == "(O)275" ? 1 : 0;
        this.geodeSpot.item.drawInMenu(b, new Vector2((float) (this.geodeSpot.bounds.X + 360), (float) (this.geodeSpot.bounds.Y + 160 /*0xA0*/)) + vector2, 1f);
      }
      else
        this.geodeDestructionAnimation.draw(b, true);
      foreach (TemporaryAnimatedSprite fluffSprite in this.fluffSprites)
        fluffSprite.draw(b, true);
      if (this.geodeTreasure != null && (double) this.delayBeforeShowArtifactTimer <= 0.0)
        this.geodeTreasure.drawInMenu(b, new Vector2((float) (this.geodeSpot.bounds.X + (this.geodeSpot.item.QualifiedItemId.Contains("MysteryBox") ? 86 : 90) * 4), (float) (this.geodeSpot.bounds.Y + 160 /*0xA0*/ + this.yPositionOfGem)), 1f);
      this.sparkle?.draw(b, true);
    }
    this.clint.draw(b, new Vector2((float) (this.geodeSpot.bounds.X + 384), (float) (this.geodeSpot.bounds.Y + 64 /*0x40*/)), 0.877f);
    if (!this.hoverText.Equals(""))
      IClickableMenu.drawHoverText(b, this.hoverText, Game1.smallFont);
    this.heldItem?.drawInMenu(b, new Vector2((float) (Game1.getOldMouseX() + 8), (float) (Game1.getOldMouseY() + 8)), 1f);
    if (Game1.options.hardwareCursor)
      return;
    this.drawMouse(b);
  }
}
