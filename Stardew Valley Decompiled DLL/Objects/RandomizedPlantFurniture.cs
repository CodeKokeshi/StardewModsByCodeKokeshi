// Decompiled with JetBrains decompiler
// Type: StardewValley.Objects.RandomizedPlantFurniture
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using System;

#nullable disable
namespace StardewValley.Objects;

public class RandomizedPlantFurniture : Furniture
{
  public NetInt topIndex = new NetInt();
  public NetInt middleIndex = new NetInt();
  public NetInt bottomIndex = new NetInt();

  protected override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.topIndex, "topIndex").AddField((INetSerializable) this.middleIndex, "middleIndex").AddField((INetSerializable) this.bottomIndex, "bottomIndex");
  }

  /// <inheritdoc />
  protected override Item GetOneNew()
  {
    return (Item) new RandomizedPlantFurniture(this.ItemId, this.tileLocation.Value);
  }

  /// <inheritdoc />
  protected override void GetOneCopyFrom(Item source)
  {
    base.GetOneCopyFrom(source);
    if (!(source is RandomizedPlantFurniture randomizedPlantFurniture))
      return;
    this.topIndex.Value = randomizedPlantFurniture.topIndex.Value;
    this.middleIndex.Value = randomizedPlantFurniture.middleIndex.Value;
    this.bottomIndex.Value = randomizedPlantFurniture.bottomIndex.Value;
  }

  public RandomizedPlantFurniture(string which, Vector2 tile)
    : this(which, tile, Game1.random.Next())
  {
  }

  public RandomizedPlantFurniture(string which, Vector2 tile, int random_seed)
    : base(which, tile)
  {
    Random random = Utility.CreateRandom((double) random_seed);
    this.topIndex.Value = random.Next(24);
    this.middleIndex.Value = random.Next(24);
    this.bottomIndex.Value = random.Next(16 /*0x10*/);
  }

  public RandomizedPlantFurniture()
  {
  }

  protected override float getScaleSize() => 1.5f;

  public override void drawInMenu(
    SpriteBatch spriteBatch,
    Vector2 location,
    float scaleSize,
    float transparency,
    float layerDepth,
    StackDrawType drawStackNumber,
    Color color,
    bool drawShadow)
  {
    location += new Vector2(32f, 32f);
    this.DrawFurniture(spriteBatch, location, transparency, new Vector2(8f, 0.0f), this.getScaleSize() * scaleSize, layerDepth);
    if (((drawStackNumber != StackDrawType.Draw || this.maximumStackSize() <= 1 || this.Stack <= 1) && drawStackNumber != StackDrawType.Draw_OneInclusive || (double) scaleSize <= 0.3 ? 0 : (this.Stack != int.MaxValue ? 1 : 0)) == 0)
      return;
    Utility.drawTinyDigits(this.stack.Value, spriteBatch, location + new Vector2((float) (64 /*0x40*/ - Utility.getWidthOfTinyDigitString(this.stack.Value, 3f * scaleSize)) + 3f * scaleSize, (float) (64.0 - 18.0 * (double) scaleSize + 2.0)), 3f * scaleSize, 1f, color);
  }

  public override bool IsHeldOverHead() => true;

  public override void drawWhenHeld(SpriteBatch spriteBatch, Vector2 objectPosition, Farmer f)
  {
    this.DrawFurniture(spriteBatch, objectPosition, 4f, Vector2.Zero, 4f, (float) (f.StandingPixel.Y + 3) / 10000f);
  }

  public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1f)
  {
    if (this.isTemporarilyInvisible)
      return;
    if (Furniture.isDrawingLocationFurniture)
    {
      x = (int) this.drawPosition.X;
      y = (int) this.drawPosition.Y;
    }
    else
    {
      x *= 64 /*0x40*/;
      y *= 64 /*0x40*/;
    }
    if (this.shakeTimer > 0)
    {
      x += Game1.random.Next(-1, 2);
      y += Game1.random.Next(-1, 2);
    }
    this.DrawFurniture(spriteBatch, Game1.GlobalToLocal(new Vector2((float) x, (float) y)), alpha, Vector2.Zero, 4f, (float) (this.boundingBox.Value.Bottom - 8) / 10000f);
  }

  public override void drawAtNonTileSpot(
    SpriteBatch spriteBatch,
    Vector2 location,
    float layerDepth,
    float alpha = 1f)
  {
    this.DrawFurniture(spriteBatch, location, 1f, Vector2.Zero, 4f, layerDepth);
  }

  public virtual void DrawFurniture(
    SpriteBatch sb,
    Vector2 location,
    float alpha,
    Vector2 origin,
    float scale,
    float base_sort_y)
  {
    Texture2D texture = ItemRegistry.GetDataOrErrorItem(this.QualifiedItemId).GetTexture();
    Rectangle rectangle = new Rectangle(0, 96 /*0x60*/, 16 /*0x10*/, 16 /*0x10*/);
    rectangle.X += this.bottomIndex.Value % 8 * 16 /*0x10*/;
    rectangle.Y += this.bottomIndex.Value / 8 * 16 /*0x10*/;
    sb.Draw(texture, location, new Rectangle?(rectangle), Color.White * alpha, 0.0f, origin, scale, this.flipped.Value ? SpriteEffects.FlipHorizontally : SpriteEffects.None, base_sort_y);
    float x = -1f * scale;
    rectangle = new Rectangle(0, 48 /*0x30*/, 16 /*0x10*/, 16 /*0x10*/);
    rectangle.X += this.middleIndex.Value % 8 * 16 /*0x10*/;
    rectangle.Y += this.middleIndex.Value / 8 * 16 /*0x10*/;
    sb.Draw(texture, location + new Vector2(x, -8f * scale), new Rectangle?(rectangle), Color.White * alpha, 0.0f, origin, scale, this.flipped.Value ? SpriteEffects.FlipHorizontally : SpriteEffects.None, base_sort_y + 1E-05f);
    rectangle = new Rectangle(0, 0, 16 /*0x10*/, 16 /*0x10*/);
    rectangle.X += this.topIndex.Value % 8 * 16 /*0x10*/;
    rectangle.Y += this.topIndex.Value / 8 * 16 /*0x10*/;
    sb.Draw(texture, location + new Vector2(x, -24f * scale), new Rectangle?(rectangle), Color.White * alpha, 0.0f, origin, scale, this.flipped.Value ? SpriteEffects.FlipHorizontally : SpriteEffects.None, base_sort_y + 1E-05f);
  }
}
