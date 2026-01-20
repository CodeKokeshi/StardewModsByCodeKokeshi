// Decompiled with JetBrains decompiler
// Type: StardewValley.Buildings.ShippingBin
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Menus;
using StardewValley.Objects;
using System.Collections.Generic;

#nullable disable
namespace StardewValley.Buildings;

public class ShippingBin : Building
{
  private TemporaryAnimatedSprite shippingBinLid;
  private Farm farm;
  private Rectangle shippingBinLidOpenArea;
  protected Vector2 _lidGenerationPosition;

  public ShippingBin(Vector2 tileLocation)
    : base("Shipping Bin", tileLocation)
  {
    this.initLid();
  }

  public ShippingBin()
    : this(Vector2.Zero)
  {
  }

  public void initLid()
  {
    this.shippingBinLid = new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(134, 226, 30, 25), new Vector2((float) this.tileX.Value, (float) (this.tileY.Value - 1)) * 64f + new Vector2(1f, -7f) * 4f, false, 0.0f, Color.White)
    {
      holdLastFrame = true,
      destroyable = false,
      interval = 20f,
      animationLength = 13,
      paused = true,
      scale = 4f,
      layerDepth = (float) ((double) ((this.tileY.Value + 1) * 64 /*0x40*/) / 10000.0 + 9.9999997473787516E-05),
      pingPong = true,
      pingPongMotion = 0
    };
    this.shippingBinLidOpenArea = new Rectangle((this.tileX.Value - 1) * 64 /*0x40*/, (this.tileY.Value - 1) * 64 /*0x40*/, 256 /*0x0100*/, 192 /*0xC0*/);
    this._lidGenerationPosition = new Vector2((float) this.tileX.Value, (float) this.tileY.Value);
  }

  public override Rectangle? getSourceRectForMenu()
  {
    return new Rectangle?(new Rectangle(0, 0, this.texture.Value.Bounds.Width, this.texture.Value.Bounds.Height));
  }

  public override void resetLocalState()
  {
    base.resetLocalState();
    if (this.shippingBinLid != null)
    {
      Rectangle shippingBinLidOpenArea = this.shippingBinLidOpenArea;
    }
    else
      this.initLid();
  }

  public override void Update(GameTime time)
  {
    base.Update(time);
    if (this.farm == null)
      this.farm = Game1.getFarm();
    if (this.shippingBinLid != null)
    {
      Rectangle shippingBinLidOpenArea = this.shippingBinLidOpenArea;
      if ((double) this._lidGenerationPosition.X == (double) this.tileX.Value && (double) this._lidGenerationPosition.Y == (double) this.tileY.Value)
      {
        bool flag = false;
        foreach (Character farmer in this.GetParentLocation().farmers)
        {
          if (farmer.GetBoundingBox().Intersects(this.shippingBinLidOpenArea))
          {
            this.openShippingBinLid();
            flag = true;
          }
        }
        if (!flag)
          this.closeShippingBinLid();
        this.updateShippingBinLid(time);
        return;
      }
    }
    this.initLid();
  }

  /// <inheritdoc />
  public override void performActionOnBuildingPlacement()
  {
    base.performActionOnBuildingPlacement();
    this.initLid();
  }

  private void openShippingBinLid()
  {
    if (this.shippingBinLid == null)
      return;
    if (this.shippingBinLid.pingPongMotion != 1 && this.IsInCurrentLocation())
      Game1.currentLocation.localSound("doorCreak");
    this.shippingBinLid.pingPongMotion = 1;
    this.shippingBinLid.paused = false;
  }

  private void closeShippingBinLid()
  {
    TemporaryAnimatedSprite shippingBinLid = this.shippingBinLid;
    if ((shippingBinLid != null ? (shippingBinLid.currentParentTileIndex > 0 ? 1 : 0) : 0) == 0)
      return;
    if (this.shippingBinLid.pingPongMotion != -1 && this.IsInCurrentLocation())
      Game1.currentLocation.localSound("doorCreakReverse");
    this.shippingBinLid.pingPongMotion = -1;
    this.shippingBinLid.paused = false;
  }

  private void updateShippingBinLid(GameTime time)
  {
    if (this.isShippingBinLidOpen(true) && this.shippingBinLid.pingPongMotion == 1)
      this.shippingBinLid.paused = true;
    else if (this.shippingBinLid.currentParentTileIndex == 0 && this.shippingBinLid.pingPongMotion == -1)
    {
      if (!this.shippingBinLid.paused && this.IsInCurrentLocation())
        Game1.currentLocation.localSound("woodyStep");
      this.shippingBinLid.paused = true;
    }
    this.shippingBinLid.update(time);
  }

  private bool isShippingBinLidOpen(bool requiredToBeFullyOpen = false)
  {
    return this.shippingBinLid != null && this.shippingBinLid.currentParentTileIndex >= (requiredToBeFullyOpen ? this.shippingBinLid.animationLength - 1 : 1);
  }

  private void shipItem(Item i, Farmer who)
  {
    if (i == null)
      return;
    who.removeItemFromInventory(i);
    this.farm?.getShippingBin(who).Add(i);
    this.showShipment(i, false);
    this.farm.lastItemShipped = i;
    if (Game1.player.ActiveItem != null)
      return;
    Game1.player.showNotCarrying();
    Game1.player.Halt();
  }

  public override bool CanLeftClick(int x, int y)
  {
    Rectangle rectangle = new Rectangle(this.tileX.Value * 64 /*0x40*/, this.tileY.Value * 64 /*0x40*/, this.tilesWide.Value * 64 /*0x40*/, this.tilesHigh.Value * 64 /*0x40*/);
    rectangle.Y -= 64 /*0x40*/;
    rectangle.Height += 64 /*0x40*/;
    return rectangle.Contains(x, y);
  }

  public override bool leftClicked()
  {
    Item activeItem = Game1.player.ActiveItem;
    bool? nullable = activeItem?.canBeShipped();
    if (!nullable.HasValue || !nullable.GetValueOrDefault() || this.farm == null || (double) Vector2.Distance(Game1.player.Tile, new Vector2((float) this.tileX.Value + 0.5f, (float) this.tileY.Value)) > 2.0)
      return base.leftClicked();
    Game1.player.ActiveItem = (Item) null;
    Game1.player.showNotCarrying();
    this.farm.getShippingBin(Game1.player).Add(activeItem);
    this.farm.lastItemShipped = activeItem;
    this.showShipment(activeItem);
    return true;
  }

  public void showShipment(Item item, bool playThrowSound = true)
  {
    if (this.farm == null)
      return;
    GameLocation parentLocation = this.GetParentLocation();
    if (playThrowSound)
      parentLocation.localSound("backpackIN");
    DelayedAction.playSoundAfterDelay("Ship", playThrowSound ? 250 : 0);
    int num = Game1.random.Next();
    parentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(524, 218, 34, 22), new Vector2((float) this.tileX.Value, (float) (this.tileY.Value - 1)) * 64f + new Vector2(-1f, 5f) * 4f, false, 0.0f, Color.White)
    {
      interval = 100f,
      totalNumberOfLoops = 1,
      animationLength = 3,
      pingPong = true,
      alpha = this.alpha,
      scale = 4f,
      layerDepth = (float) ((double) ((this.tileY.Value + 1) * 64 /*0x40*/) / 10000.0 + 0.00019999999494757503),
      id = num,
      extraInfoForEndBehavior = num,
      endFunction = new TemporaryAnimatedSprite.endBehavior(parentLocation.removeTemporarySpritesWithID)
    });
    parentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(524, 230, 34, 10), new Vector2((float) this.tileX.Value, (float) (this.tileY.Value - 1)) * 64f + new Vector2(-1f, 17f) * 4f, false, 0.0f, Color.White)
    {
      interval = 100f,
      totalNumberOfLoops = 1,
      animationLength = 3,
      pingPong = true,
      alpha = this.alpha,
      scale = 4f,
      layerDepth = (float) ((double) ((this.tileY.Value + 1) * 64 /*0x40*/) / 10000.0 + 0.00030000001424923539),
      id = num,
      extraInfoForEndBehavior = num
    });
    ParsedItemData dataOrErrorItem = ItemRegistry.GetDataOrErrorItem(item.QualifiedItemId);
    ColoredObject coloredObject = item as ColoredObject;
    Vector2 position = new Vector2((float) this.tileX.Value, (float) (this.tileY.Value - 1)) * 64f + new Vector2((float) (7 + Game1.random.Next(6)), 2f) * 4f;
    bool[] flagArray = new bool[2]{ false, true };
    foreach (bool flag in flagArray)
    {
      if (!flag || coloredObject != null && !coloredObject.ColorSameIndexAsParentSheetIndex)
        parentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(dataOrErrorItem.TextureName, dataOrErrorItem.GetSourceRect(flag ? 1 : 0), position, false, 0.0f, Color.White)
        {
          interval = 9999f,
          scale = 4f,
          alphaFade = 0.045f,
          layerDepth = (float) ((double) ((this.tileY.Value + 1) * 64 /*0x40*/) / 10000.0 + 0.00022499999613501132),
          motion = new Vector2(0.0f, 0.3f),
          acceleration = new Vector2(0.0f, 0.2f),
          scaleChange = -0.05f,
          color = coloredObject != null ? coloredObject.color.Value : Color.White
        });
    }
  }

  public override bool doAction(Vector2 tileLocation, Farmer who)
  {
    if (this.daysOfConstructionLeft.Value > 0 || (double) tileLocation.X < (double) this.tileX.Value || (double) tileLocation.X > (double) (this.tileX.Value + 1) || (double) tileLocation.Y != (double) this.tileY.Value)
      return base.doAction(tileLocation, who);
    if (!Game1.didPlayerJustRightClick(true))
      return false;
    ItemGrabMenu itemGrabMenu = new ItemGrabMenu((IList<Item>) null, true, false, new InventoryMenu.highlightThisItem(Utility.highlightShippableObjects), new ItemGrabMenu.behaviorOnItemSelect(this.shipItem), "", snapToBottom: true, canBeExitedWithKey: true, playRightClickSound: false, context: (object) this);
    itemGrabMenu.initializeUpperRightCloseButton();
    itemGrabMenu.setBackgroundTransparency(false);
    itemGrabMenu.setDestroyItemOnClick(true);
    itemGrabMenu.initializeShippingBin();
    Game1.activeClickableMenu = (IClickableMenu) itemGrabMenu;
    if (who.IsLocalPlayer)
      Game1.playSound("shwip");
    if (Game1.player.FacingDirection == 1)
      Game1.player.Halt();
    Game1.player.showCarrying();
    return true;
  }

  public override void drawInMenu(SpriteBatch b, int x, int y)
  {
    base.drawInMenu(b, x, y);
    b.Draw(Game1.mouseCursors, new Vector2((float) (x + 4), (float) (y - 20)), new Rectangle?(new Rectangle(134, 226, 30, 25)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
  }

  public override void draw(SpriteBatch b)
  {
    if (this.isMoving)
      return;
    base.draw(b);
    if (this.shippingBinLid == null || this.daysOfConstructionLeft.Value > 0)
      return;
    this.shippingBinLid.color = this.color;
    this.shippingBinLid.draw(b, extraAlpha: this.alpha * (this.newConstructionTimer.Value > 0 ? (float) ((1000.0 - (double) this.newConstructionTimer.Value) / 1000.0) : 1f));
  }
}
