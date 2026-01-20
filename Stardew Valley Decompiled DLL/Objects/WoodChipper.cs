// Decompiled with JetBrains decompiler
// Type: StardewValley.Objects.WoodChipper
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.ItemTypeDefinitions;
using System;

#nullable disable
namespace StardewValley.Objects;

public class WoodChipper : StardewValley.Object
{
  public const int CHIP_TIME = 1000;
  public readonly NetRef<StardewValley.Object> depositedItem = new NetRef<StardewValley.Object>();
  protected bool _isAnimatingChip;
  public int nextSmokeTime;
  public int nextShakeTime;

  /// <inheritdoc />
  public override string TypeDefinitionId => "(BC)";

  /// <inheritdoc />
  protected override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.depositedItem, "depositedItem");
    this.depositedItem.fieldChangeVisibleEvent += new FieldChange<NetRef<StardewValley.Object>, StardewValley.Object>(this.OnDepositedItemChange);
  }

  public void OnDepositedItemChange(NetRef<StardewValley.Object> field, StardewValley.Object old_value, StardewValley.Object new_value)
  {
    if (Game1.gameMode == (byte) 6 || new_value == null)
      return;
    this.shakeTimer = 1000;
    this._isAnimatingChip = true;
  }

  public WoodChipper()
  {
  }

  public WoodChipper(Vector2 position)
    : base(position, "211")
  {
    this.Name = "Wood Chipper";
    this.type.Value = "Crafting";
    this.bigCraftable.Value = true;
    this.canBeSetDown.Value = true;
  }

  public override void addWorkingAnimation()
  {
    GameLocation location = this.Location;
    if (location == null || !location.farmers.Any() || Game1.random.NextDouble() >= 0.35)
      return;
    for (int index = 0; index < 8; ++index)
      location.temporarySprites.Add(new TemporaryAnimatedSprite(47, this.tileLocation.Value * 64f + new Vector2(0.0f, (float) (Game1.random.Next(-48, 0) - 76)), new Color(200, 110, 17), animationInterval: 50f, layerDepth: (float) (3.0 / 1000.0 + (double) Math.Max(0.0f, (float) ((((double) this.tileLocation.Y + 1.0) * 64.0 - 24.0) / 10000.0)) + (double) this.tileLocation.X * 9.9999997473787516E-06))
      {
        delayBeforeAnimationStart = index * 100
      });
    location.playSound("woodchipper_occasional");
    this.shakeTimer = 1500;
  }

  /// <inheritdoc />
  public override bool performObjectDropInAction(
    Item dropInItem,
    bool probe,
    Farmer who,
    bool returnFalseIfItemConsumed = false)
  {
    GameLocation location = this.Location;
    if (location == null)
      return false;
    StardewValley.Object inputItem = dropInItem as StardewValley.Object;
    if (this.heldObject.Value != null || this.depositedItem.Value != null)
      return base.performObjectDropInAction(dropInItem, probe, who, returnFalseIfItemConsumed);
    if (inputItem == null)
      return false;
    if (!this.PlaceInMachine(this.GetMachineData(), (Item) inputItem, probe, who))
      return base.performObjectDropInAction(dropInItem, probe, who, returnFalseIfItemConsumed);
    if (!probe)
    {
      this.depositedItem.Value = dropInItem.getOne() as StardewValley.Object;
      this.shakeTimer = 1800;
      for (int index = 0; index < 12; ++index)
        location.temporarySprites.Add(new TemporaryAnimatedSprite(47, this.tileLocation.Value * 64f + new Vector2(0.0f, (float) (Game1.random.Next(-48, 0) - 76)), new Color(200, 110, 17), animationInterval: 50f, layerDepth: (float) (3.0 / 1000.0 + (double) Math.Max(0.0f, (float) ((((double) this.tileLocation.Y + 1.0) * 64.0 - 24.0) / 10000.0)) + (double) this.tileLocation.X * 9.9999997473787516E-06))
        {
          delayBeforeAnimationStart = index * 100
        });
      if (returnFalseIfItemConsumed)
        return false;
    }
    return true;
  }

  public override bool placementAction(GameLocation location, int x, int y, Farmer who = null)
  {
    this.TileLocation = new Vector2((float) (x / 64 /*0x40*/), (float) (y / 64 /*0x40*/));
    return true;
  }

  /// <inheritdoc />
  public override bool checkForAction(Farmer who, bool justCheckingForActivity = false)
  {
    if (!who.IsLocalPlayer || this.heldObject.Value == null || !this.readyForHarvest.Value)
      return base.checkForAction(who, justCheckingForActivity);
    if (!justCheckingForActivity)
    {
      StardewValley.Object @object = this.heldObject.Value;
      this.heldObject.Value = (StardewValley.Object) null;
      if (who.isMoving())
        Game1.haltAfterCheck = false;
      if (!who.addItemToInventoryBool((Item) @object))
      {
        this.heldObject.Value = @object;
        Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Crop.cs.588"));
        return false;
      }
      Game1.playSound("coin");
      this.readyForHarvest.Value = false;
      this.depositedItem.Value = (StardewValley.Object) null;
      this.heldObject.Value = (StardewValley.Object) null;
      this.AttemptAutoLoad(who);
    }
    return true;
  }

  public override void updateWhenCurrentLocation(GameTime time)
  {
    if (this.Location != null && this.depositedItem.Value != null && this.MinutesUntilReady > 0)
    {
      int nextShakeTime = this.nextShakeTime;
      TimeSpan elapsedGameTime = time.ElapsedGameTime;
      int milliseconds1 = elapsedGameTime.Milliseconds;
      this.nextShakeTime = nextShakeTime - milliseconds1;
      int nextSmokeTime = this.nextSmokeTime;
      elapsedGameTime = time.ElapsedGameTime;
      int milliseconds2 = elapsedGameTime.Milliseconds;
      this.nextSmokeTime = nextSmokeTime - milliseconds2;
      if (this.nextSmokeTime <= 0)
        this.nextSmokeTime = Game1.random.Next(3000, 6000);
      if (this.nextShakeTime <= 0)
      {
        this.nextShakeTime = Game1.random.Next(1000, 2000);
        if (this.shakeTimer <= 0)
        {
          this._isAnimatingChip = false;
          this.shakeTimer = 0;
        }
      }
    }
    base.updateWhenCurrentLocation(time);
  }

  public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1f)
  {
    if (this.isTemporarilyInvisible)
      return;
    Vector2 vector2_1 = Vector2.One * 4f;
    Vector2 local = Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (x * 64 /*0x40*/), (float) (y * 64 /*0x40*/ - 64 /*0x40*/)));
    Rectangle destinationRectangle = new Rectangle((int) ((double) local.X - (double) vector2_1.X / 2.0) + (this.shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0), (int) ((double) local.Y - (double) vector2_1.Y / 2.0) + (this.shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0), (int) (64.0 + (double) vector2_1.X), (int) (128.0 + (double) vector2_1.Y / 2.0));
    float layerDepth = Math.Max(0.0f, (float) ((y + 1) * 64 /*0x40*/ - 24) / 10000f) + (float) x * 1E-05f;
    ParsedItemData dataOrErrorItem1 = ItemRegistry.GetDataOrErrorItem(this.QualifiedItemId);
    Texture2D texture1 = dataOrErrorItem1.GetTexture();
    spriteBatch.Draw(texture1, destinationRectangle, new Rectangle?(dataOrErrorItem1.GetSourceRect(this.readyForHarvest.Value ? 1 : 0)), Color.White * alpha, 0.0f, Vector2.Zero, SpriteEffects.None, layerDepth);
    if (this.shakeTimer > 0)
      spriteBatch.Draw(texture1, new Rectangle(destinationRectangle.X, destinationRectangle.Y + 4, destinationRectangle.Width, 60), new Rectangle?(new Rectangle(80 /*0x50*/, 833, 16 /*0x10*/, 15)), Color.White * alpha, 0.0f, Vector2.Zero, SpriteEffects.None, layerDepth + 0.0035f);
    if (this.depositedItem.Value != null && this.shakeTimer > 0 && this._isAnimatingChip)
    {
      float t = (float) (1.0 - (double) this.shakeTimer / 1000.0);
      Vector2 vector2_2 = local + new Vector2(32f, 32f);
      Vector2 vector2_3 = vector2_2 + new Vector2(0.0f, -16f);
      Vector2 position = new Vector2();
      position.X = Utility.Lerp(vector2_3.X, vector2_2.X, t);
      position.Y = Utility.Lerp(vector2_3.Y, vector2_2.Y, t);
      position.X += (float) (Game1.random.Next(-1, 2) * 2);
      position.Y += (float) (Game1.random.Next(-1, 2) * 2);
      float num = Utility.Lerp(1f, 0.75f, t);
      ParsedItemData dataOrErrorItem2 = ItemRegistry.GetDataOrErrorItem(this.depositedItem.Value.QualifiedItemId);
      Texture2D texture2 = dataOrErrorItem2.GetTexture();
      spriteBatch.Draw(texture2, position, new Rectangle?(dataOrErrorItem2.GetSourceRect()), Color.White * alpha, 0.0f, new Vector2(8f, 8f), 4f * num, this.depositedItem.Value.flipped.Value ? SpriteEffects.FlipHorizontally : SpriteEffects.None, layerDepth + 0.00175f);
    }
    TimeSpan totalGameTime;
    if (this.depositedItem.Value != null && this.MinutesUntilReady > 0)
    {
      totalGameTime = Game1.currentGameTime.TotalGameTime;
      int num = (int) (totalGameTime.TotalMilliseconds % 200.0) / 50;
      spriteBatch.Draw(texture1, local + new Vector2(6f, 17f) * 4f, new Rectangle?(new Rectangle(80 /*0x50*/ + num % 2 * 8, 848 + num / 2 * 7, 8, 7)), Color.White * alpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth + 1E-05f);
      spriteBatch.Draw(texture1, local + new Vector2(3f, 9f) * 4f + new Vector2((float) Game1.random.Next(-1, 2), (float) Game1.random.Next(-1, 2)), new Rectangle?(new Rectangle(51, 841, 10, 6)), Color.White * alpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth + 1E-05f);
    }
    if (!this.readyForHarvest.Value)
      return;
    totalGameTime = Game1.currentGameTime.TotalGameTime;
    float num1 = (float) (4.0 * Math.Round(Math.Sin(totalGameTime.TotalMilliseconds / 250.0), 2));
    spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (x * 64 /*0x40*/ - 8), (float) (y * 64 /*0x40*/ - 96 /*0x60*/ - 16 /*0x10*/) + num1)), new Rectangle?(new Rectangle(141, 465, 20, 24)), Color.White * 0.75f, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) ((double) ((y + 1) * 64 /*0x40*/) / 10000.0 + 9.9999999747524271E-07 + (double) this.tileLocation.X / 10000.0));
    if (this.heldObject.Value == null)
      return;
    ParsedItemData dataOrErrorItem3 = ItemRegistry.GetDataOrErrorItem(this.heldObject.Value.QualifiedItemId);
    Texture2D texture3 = dataOrErrorItem3.GetTexture();
    spriteBatch.Draw(texture3, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (x * 64 /*0x40*/ + 32 /*0x20*/), (float) (y * 64 /*0x40*/ - 64 /*0x40*/ - 8) + num1)), new Rectangle?(dataOrErrorItem3.GetSourceRect()), Color.White * 0.75f, 0.0f, new Vector2(8f, 8f), 4f, SpriteEffects.None, (float) ((double) ((y + 1) * 64 /*0x40*/) / 10000.0 + 9.9999997473787516E-06 + (double) this.tileLocation.X / 10000.0));
    if (!(this.heldObject.Value is ColoredObject coloredObject))
      return;
    Rectangle sourceRect = dataOrErrorItem3.GetSourceRect(1, new int?(this.ParentSheetIndex));
    spriteBatch.Draw(texture3, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (x * 64 /*0x40*/ + 32 /*0x20*/), (float) (y * 64 /*0x40*/ - 64 /*0x40*/ - 8) + num1)), new Rectangle?(sourceRect), coloredObject.color.Value * 0.75f, 0.0f, new Vector2(8f, 8f), 4f, SpriteEffects.None, (float) ((double) ((y + 1) * 64 /*0x40*/) / 10000.0 + 9.9999997473787516E-06 + (double) this.tileLocation.X / 10000.0));
  }
}
