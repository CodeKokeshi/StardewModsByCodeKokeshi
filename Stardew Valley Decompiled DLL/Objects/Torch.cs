// Decompiled with JetBrains decompiler
// Type: StardewValley.Torch
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.BellsAndWhistles;
using StardewValley.Extensions;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Menus;
using System;

#nullable disable
namespace StardewValley;

public class Torch : Object
{
  public const float yVelocity = 1f;
  public const float yDissapearLevel = -100f;
  public const double ashChance = 0.015;
  private float color;
  private Vector2[] ashes = new Vector2[3];
  private float smokePuffTimer;

  public Torch()
    : this(1)
  {
  }

  public Torch(int initialStack)
    : base("93", initialStack)
  {
  }

  public Torch(int initialStack, string itemId)
    : base(itemId, initialStack)
  {
  }

  public Torch(string index, bool bigCraftable)
    : base(Vector2.Zero, index)
  {
  }

  /// <inheritdoc />
  public override void RecalculateBoundingBox()
  {
    this.boundingBox.Value = new Rectangle((int) this.tileLocation.X * 64 /*0x40*/, (int) this.tileLocation.Y * 64 /*0x40*/, 64 /*0x40*/, 64 /*0x40*/);
  }

  /// <inheritdoc />
  protected override void MigrateLegacyItemId()
  {
    this.ItemId = this.parentSheetIndex.Value.ToString();
  }

  /// <inheritdoc />
  protected override Item GetOneNew()
  {
    return !this.bigCraftable.Value ? (Item) new Torch(1, this.ItemId) : (Item) new Torch(this.ItemId, true);
  }

  /// <inheritdoc />
  public override void actionOnPlayerEntry()
  {
    base.actionOnPlayerEntry();
    if (!this.bigCraftable.Value || !this.isOn.Value)
      return;
    AmbientLocationSounds.addSound(this.tileLocation.Value, 1);
  }

  /// <inheritdoc />
  public override bool checkForAction(Farmer who, bool justCheckingForActivity = false)
  {
    if (!this.bigCraftable.Value)
      return base.checkForAction(who, justCheckingForActivity);
    if (justCheckingForActivity)
      return true;
    if (this.QualifiedItemId == "(BC)278")
    {
      Vector2 centeringOnScreen = Utility.getTopLeftPositionForCenteringOnScreen(800 + IClickableMenu.borderWidth * 2, 600 + IClickableMenu.borderWidth * 2);
      Game1.activeClickableMenu = (IClickableMenu) new CraftingPage((int) centeringOnScreen.X, (int) centeringOnScreen.Y, 800 + IClickableMenu.borderWidth * 2, 600 + IClickableMenu.borderWidth * 2, true, true);
      return true;
    }
    this.isOn.Value = !this.isOn.Value;
    if (this.isOn.Value)
    {
      if (this.bigCraftable.Value)
      {
        if (who != null)
          Game1.playSound("fireball");
        this.initializeLightSource(this.tileLocation.Value);
        AmbientLocationSounds.addSound(this.tileLocation.Value, 1);
      }
    }
    else if (this.bigCraftable.Value)
    {
      this.performRemoveAction();
      if (who != null)
        Game1.playSound("woodyHit");
    }
    return true;
  }

  /// <inheritdoc />
  public override bool placementAction(GameLocation location, int x, int y, Farmer who)
  {
    Vector2 vector2 = new Vector2((float) (x / 64 /*0x40*/), (float) (y / 64 /*0x40*/));
    Torch torch = this.bigCraftable.Value ? new Torch(this.ItemId, true) : new Torch(1, this.ItemId);
    if (this.bigCraftable.Value)
      torch.isOn.Value = false;
    location.objects.Add(vector2, (Object) torch);
    torch.initializeLightSource(vector2);
    if (who != null)
      Game1.playSound("woodyStep");
    return true;
  }

  public override bool isPassable() => !this.bigCraftable.Value;

  public override void updateWhenCurrentLocation(GameTime time)
  {
    base.updateWhenCurrentLocation(time);
    GameLocation location = this.Location;
    if (location == null)
      return;
    this.updateAshes((int) ((double) this.tileLocation.X * 2000.0 + (double) this.tileLocation.Y));
    this.smokePuffTimer -= (float) time.ElapsedGameTime.TotalMilliseconds;
    if ((double) this.smokePuffTimer > 0.0)
      return;
    this.smokePuffTimer = 1000f;
    if (!(this.QualifiedItemId == "(BC)278"))
      return;
    Utility.addSmokePuff(location, this.tileLocation.Value * 64f + new Vector2(32f, -32f));
  }

  private void updateAshes(int identifier)
  {
    if (!Utility.isOnScreen(this.tileLocation.Value * 64f, 256 /*0x0100*/))
      return;
    for (int index = this.ashes.Length - 1; index >= 0; --index)
    {
      Vector2 ash = this.ashes[index];
      ash.Y -= (float) (1.0 * ((double) (index + 1) * 0.25));
      if (index % 2 != 0)
        ash.X += (float) Math.Sin((double) this.ashes[index].Y / (2.0 * Math.PI)) / 2f;
      this.ashes[index] = ash;
      if (Game1.random.NextDouble() < 3.0 / 400.0 && (double) this.ashes[index].Y < -100.0)
        this.ashes[index] = new Vector2((float) (Game1.random.Next(-1, 3) * 4) * 0.75f, 0.0f);
    }
    this.color = Math.Max(-0.8f, Math.Min(0.7f, this.color + this.ashes[0].Y / 1200f));
  }

  public override void performRemoveAction()
  {
    AmbientLocationSounds.removeSound(this.TileLocation);
    if (this.bigCraftable.Value)
      this.isOn.Value = false;
    base.performRemoveAction();
  }

  public override void draw(
    SpriteBatch spriteBatch,
    int xNonTile,
    int yNonTile,
    float layerDepth,
    float alpha = 1f)
  {
    ParsedItemData dataOrErrorItem = ItemRegistry.GetDataOrErrorItem(this.QualifiedItemId);
    Rectangle rectangle = dataOrErrorItem.GetSourceRect(spriteIndex: new int?(this.ParentSheetIndex)).Clone();
    rectangle.Y += 8;
    rectangle.Height /= 2;
    spriteBatch.Draw(dataOrErrorItem.GetTexture(), Game1.GlobalToLocal(Game1.viewport, new Vector2((float) xNonTile, (float) (yNonTile + 32 /*0x20*/))), new Rectangle?(rectangle), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth);
    rectangle.X = 276 + (int) ((Game1.currentGameTime.TotalGameTime.TotalMilliseconds + (double) (xNonTile * 320) + (double) (yNonTile * 49)) % 700.0 / 100.0) * 8;
    rectangle.Y = 1965;
    rectangle.Width = 8;
    rectangle.Height = 8;
    spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (xNonTile + 32 /*0x20*/ + 4), (float) (yNonTile + 16 /*0x10*/ + 4))), new Rectangle?(rectangle), Color.White * 0.75f, 0.0f, new Vector2(4f, 4f), 3f, SpriteEffects.None, layerDepth + 1E-05f);
    spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (xNonTile + 32 /*0x20*/ + 4), (float) (yNonTile + 16 /*0x10*/ + 4))), new Rectangle?(new Rectangle(88, 1779, 30, 30)), Color.PaleGoldenrod * (Game1.currentLocation.IsOutdoors ? 0.35f : 0.43f), 0.0f, new Vector2(15f, 15f), (float) (8.0 + 32.0 * Math.Sin((Game1.currentGameTime.TotalGameTime.TotalMilliseconds + (double) (xNonTile * 777) + (double) (yNonTile * 9746)) % 3140.0 / 1000.0) / 50.0), SpriteEffects.None, 1f);
  }

  public static void drawBasicTorch(
    SpriteBatch spriteBatch,
    float x,
    float y,
    float layerDepth,
    float alpha = 1f)
  {
    Rectangle rectangle = new Rectangle(336, 48 /*0x30*/, 16 /*0x10*/, 16 /*0x10*/);
    rectangle.Y += 8;
    rectangle.Height /= 2;
    spriteBatch.Draw(Game1.objectSpriteSheet, Game1.GlobalToLocal(Game1.viewport, new Vector2(x, y + 32f)), new Rectangle?(rectangle), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth);
    spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) ((double) x + 32.0 + 2.0), y + 16f)), new Rectangle?(new Rectangle(88, 1779, 30, 30)), Color.PaleGoldenrod * (Game1.currentLocation.IsOutdoors ? 0.35f : 0.43f), 0.0f, new Vector2(15f, 15f), (float) (4.0 + 64.0 * Math.Sin((Game1.currentGameTime.TotalGameTime.TotalMilliseconds + (double) x * 777.0 + (double) y * 9746.0) % 3140.0 / 1000.0) / 50.0), SpriteEffects.None, 1f);
    rectangle.X = 276 + (int) ((Game1.currentGameTime.TotalGameTime.TotalMilliseconds + (double) x * 3204.0 + (double) y * 49.0) % 700.0 / 100.0) * 8;
    rectangle.Y = 1965;
    rectangle.Width = 8;
    rectangle.Height = 8;
    spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) ((double) x + 32.0 + 4.0), (float) ((double) y + 16.0 + 4.0))), new Rectangle?(rectangle), Color.White * 0.75f, 0.0f, new Vector2(4f, 4f), 3f, SpriteEffects.None, layerDepth + 0.0001f);
  }

  public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1f)
  {
    if (Game1.eventUp)
    {
      GameLocation currentLocation = Game1.currentLocation;
      if ((currentLocation != null ? (currentLocation.currentEvent?.showGroundObjects.GetValueOrDefault() ? 1 : 0) : 0) == 0 && !Game1.currentLocation.IsFarm)
        return;
    }
    if (!this.bigCraftable.Value)
    {
      ParsedItemData dataOrErrorItem = ItemRegistry.GetDataOrErrorItem(this.QualifiedItemId);
      Rectangle rectangle = dataOrErrorItem.GetSourceRect(spriteIndex: new int?(this.ParentSheetIndex)).Clone();
      Rectangle boundingBoxAt = this.GetBoundingBoxAt(x, y);
      rectangle.Y += 8;
      rectangle.Height /= 2;
      spriteBatch.Draw(dataOrErrorItem.GetTexture(), Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (x * 64 /*0x40*/), (float) (y * 64 /*0x40*/ + 32 /*0x20*/))), new Rectangle?(rectangle), Color.White, 0.0f, Vector2.Zero, (double) this.scale.Y > 1.0 ? this.getScale().Y : 4f, SpriteEffects.None, (float) (boundingBoxAt.Center.Y - 16 /*0x10*/) / 10000f);
      spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (x * 64 /*0x40*/ + 32 /*0x20*/ + 2), (float) (y * 64 /*0x40*/ + 16 /*0x10*/))), new Rectangle?(new Rectangle(88, 1779, 30, 30)), Color.PaleGoldenrod * (Game1.currentLocation.IsOutdoors ? 0.35f : 0.43f), 0.0f, new Vector2(15f, 15f), (float) (4.0 + 64.0 * Math.Sin((Game1.currentGameTime.TotalGameTime.TotalMilliseconds + (double) (x * 64 /*0x40*/ * 777) + (double) (y * 64 /*0x40*/ * 9746)) % 3140.0 / 1000.0) / 50.0), SpriteEffects.None, (float) (boundingBoxAt.Center.Y - 15) / 10000f);
      rectangle.X = 276 + (int) ((Game1.currentGameTime.TotalGameTime.TotalMilliseconds + (double) (x * 3204) + (double) (y * 49)) % 700.0 / 100.0) * 8;
      rectangle.Y = 1965;
      rectangle.Width = 8;
      rectangle.Height = 8;
      spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (x * 64 /*0x40*/ + 32 /*0x20*/ + 4), (float) (y * 64 /*0x40*/ + 16 /*0x10*/ + 4))), new Rectangle?(rectangle), Color.White * 0.75f, 0.0f, new Vector2(4f, 4f), 3f, SpriteEffects.None, (float) (boundingBoxAt.Center.Y - 16 /*0x10*/) / 10000f);
      for (int index = 0; index < this.ashes.Length; ++index)
        spriteBatch.Draw(Game1.objectSpriteSheet, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (x * 64 /*0x40*/ + 32 /*0x20*/) + this.ashes[index].X, (float) (y * 64 /*0x40*/ + 32 /*0x20*/) + this.ashes[index].Y)), new Rectangle?(new Rectangle(344 + index % 3, 53, 1, 1)), Color.White * 0.5f * (float) ((-100.0 - (double) this.ashes[index].Y / 2.0) / -100.0), 0.0f, Vector2.Zero, 3f, SpriteEffects.None, (float) (boundingBoxAt.Center.Y - 16 /*0x10*/) / 10000f);
    }
    else
    {
      base.draw(spriteBatch, x, y, alpha);
      float num = Math.Max(0.0f, (float) ((y + 1) * 64 /*0x40*/ - 24) / 10000f) + (float) x * 1E-05f;
      if (!this.isOn.Value)
        return;
      if (ItemContextTagManager.HasBaseTag(this.QualifiedItemId, "campfire_item"))
      {
        spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (x * 64 /*0x40*/ + 16 /*0x10*/ - 4), (float) (y * 64 /*0x40*/ - 8))), new Rectangle?(new Rectangle(276 + (int) ((Game1.currentGameTime.TotalGameTime.TotalMilliseconds + (double) (x * 3047) + (double) (y * 88)) % 400.0 / 100.0) * 12, 1985, 12, 11)), Color.White, 0.0f, Vector2.Zero, 3f, SpriteEffects.None, num + 0.0008f);
        spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (x * 64 /*0x40*/ + 32 /*0x20*/ - 12), (float) (y * 64 /*0x40*/))), new Rectangle?(new Rectangle(276 + (int) ((Game1.currentGameTime.TotalGameTime.TotalMilliseconds + (double) (x * 2047 /*0x07FF*/) + (double) (y * 98)) % 400.0 / 100.0) * 12, 1985, 12, 11)), Color.White, 0.0f, Vector2.Zero, 3f, SpriteEffects.None, num + 0.0009f);
        spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (x * 64 /*0x40*/ + 32 /*0x20*/ - 20), (float) (y * 64 /*0x40*/ + 12))), new Rectangle?(new Rectangle(276 + (int) ((Game1.currentGameTime.TotalGameTime.TotalMilliseconds + (double) (x * 2077) + (double) (y * 98)) % 400.0 / 100.0) * 12, 1985, 12, 11)), Color.White, 0.0f, Vector2.Zero, 3f, SpriteEffects.None, num + 1f / 1000f);
        if (!(this.QualifiedItemId == "(BC)278"))
          return;
        ParsedItemData dataOrErrorItem = ItemRegistry.GetDataOrErrorItem(this.QualifiedItemId);
        Rectangle rectangle = dataOrErrorItem.GetSourceRect(1, new int?(this.ParentSheetIndex)).Clone();
        rectangle.Height -= 16 /*0x10*/;
        Vector2 vector2 = this.getScale() * 4f;
        Vector2 local = Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (x * 64 /*0x40*/), (float) (y * 64 /*0x40*/ - 64 /*0x40*/ + 12)));
        Rectangle destinationRectangle = new Rectangle((int) ((double) local.X - (double) vector2.X / 2.0) + (this.shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0), (int) ((double) local.Y - (double) vector2.Y / 2.0) + (this.shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0), (int) (64.0 + (double) vector2.X), (int) (64.0 + (double) vector2.Y / 2.0));
        spriteBatch.Draw(dataOrErrorItem.GetTexture(), destinationRectangle, new Rectangle?(rectangle), Color.White * alpha, 0.0f, Vector2.Zero, SpriteEffects.None, num + 0.0028f);
      }
      else
        spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (x * 64 /*0x40*/ + 16 /*0x10*/ - 8), (float) (y * 64 /*0x40*/ - 64 /*0x40*/ + 8))), new Rectangle?(new Rectangle(276 + (int) ((Game1.currentGameTime.TotalGameTime.TotalMilliseconds + (double) (x * 3047) + (double) (y * 88)) % 400.0 / 100.0) * 12, 1985, 12, 11)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, num + 0.0008f);
    }
  }
}
