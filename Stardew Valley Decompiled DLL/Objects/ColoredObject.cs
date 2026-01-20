// Decompiled with JetBrains decompiler
// Type: StardewValley.Objects.ColoredObject
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Extensions;
using StardewValley.GameData.Objects;
using StardewValley.ItemTypeDefinitions;
using System;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley.Objects;

public class ColoredObject : StardewValley.Object
{
  [XmlElement("color")]
  public readonly NetColor color = new NetColor();
  [XmlElement("colorSameIndexAsParentSheetIndex")]
  public readonly NetBool colorSameIndexAsParentSheetIndex = new NetBool();

  public bool ColorSameIndexAsParentSheetIndex
  {
    get => this.colorSameIndexAsParentSheetIndex.Value;
    set => this.colorSameIndexAsParentSheetIndex.Value = value;
  }

  /// <inheritdoc />
  protected override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.color, "color").AddField((INetSerializable) this.colorSameIndexAsParentSheetIndex, "colorSameIndexAsParentSheetIndex");
  }

  public ColoredObject()
  {
  }

  public ColoredObject(string itemId, int stack, Color color)
    : base(itemId, stack)
  {
    this.color.Value = color;
    ObjectData objectData;
    if (!Game1.objectData.TryGetValue(this.ItemId, out objectData))
      return;
    this.ColorSameIndexAsParentSheetIndex = !objectData.ColorOverlayFromNextIndex;
  }

  public override void drawInMenu(
    SpriteBatch spriteBatch,
    Vector2 location,
    float scaleSize,
    float transparency,
    float layerDepth,
    StackDrawType drawStackNumber,
    Color colorOverride,
    bool drawShadow)
  {
    this.AdjustMenuDrawForRecipes(ref transparency, ref scaleSize);
    if (drawShadow && !this.bigCraftable.Value && this.QualifiedItemId != "(O)590" && this.QualifiedItemId != "(O)SeedSpot")
      this.DrawShadow(spriteBatch, location, colorOverride, layerDepth);
    ParsedItemData dataOrErrorItem = ItemRegistry.GetDataOrErrorItem(this.ItemId);
    Texture2D texture = dataOrErrorItem.GetTexture();
    Vector2 vector2 = this.bigCraftable.Value ? new Vector2(32f, 64f) : new Vector2(8f, 8f);
    float scale = this.bigCraftable.Value ? ((double) scaleSize < 0.20000000298023224 ? scaleSize : scaleSize / 2f) : 4f * scaleSize;
    if (this.ItemId == "SmokedFish")
      this.drawSmokedFish(spriteBatch, location, scaleSize, layerDepth, (double) transparency != 1.0 || colorOverride.A >= byte.MaxValue ? transparency : (float) colorOverride.A / (float) byte.MaxValue);
    else if (!this.ColorSameIndexAsParentSheetIndex)
    {
      Rectangle sourceRect = dataOrErrorItem.GetSourceRect(1, new int?(this.ParentSheetIndex));
      transparency = (double) transparency != 1.0 || colorOverride.A >= byte.MaxValue ? transparency : (float) colorOverride.A / (float) byte.MaxValue;
      spriteBatch.Draw(texture, location + new Vector2(32f, 32f) * scaleSize, new Rectangle?(dataOrErrorItem.GetSourceRect(spriteIndex: new int?(this.ParentSheetIndex))), Color.White * transparency, 0.0f, vector2 * scaleSize, scale, SpriteEffects.None, layerDepth);
      spriteBatch.Draw(texture, location + new Vector2(32f, 32f) * scaleSize, new Rectangle?(sourceRect), this.color.Value * transparency, 0.0f, vector2 * scaleSize, scale, SpriteEffects.None, Math.Min(1f, layerDepth + 2E-05f));
    }
    else
      spriteBatch.Draw(texture, location + new Vector2(32f, 32f) * scaleSize, new Rectangle?(dataOrErrorItem.GetSourceRect(spriteIndex: new int?(this.ParentSheetIndex))), this.color.Value * transparency, 0.0f, vector2 * scaleSize, scale, SpriteEffects.None, Math.Min(1f, layerDepth + 2E-05f));
    this.DrawMenuIcons(spriteBatch, location, scaleSize, transparency, layerDepth + 3E-05f, drawStackNumber, colorOverride);
  }

  private void drawSmokedFish(
    SpriteBatch spriteBatch,
    Vector2 location,
    float scaleSize,
    float layerDepth,
    float transparency = 1f)
  {
    Vector2 vector2 = new Vector2(8f, 8f);
    float scale = 4f * scaleSize;
    ParsedItemData dataOrErrorItem = ItemRegistry.GetDataOrErrorItem(this.preservedParentSheetIndex.Value);
    Texture2D texture = dataOrErrorItem.GetTexture();
    Rectangle sourceRect = dataOrErrorItem.GetSourceRect();
    spriteBatch.Draw(texture, location + new Vector2(32f, 32f) * scaleSize, new Rectangle?(sourceRect), Color.White * transparency, 0.0f, vector2 * scaleSize, scale, SpriteEffects.None, Math.Min(1f, layerDepth + 1E-05f));
    spriteBatch.Draw(texture, location + new Vector2(32f, 32f) * scaleSize, new Rectangle?(sourceRect), new Color(80 /*0x50*/, 30, 10) * 0.6f * transparency, 0.0f, vector2 * scaleSize, scale, SpriteEffects.None, Math.Min(1f, layerDepth + 1.5E-05f));
    int num = 700 + (this.price.Value + 17) * 7777 % 200;
    spriteBatch.Draw(Game1.mouseCursors, location + new Vector2(32f, 32f) * scaleSize + new Vector2(0.0f, (float) (-Game1.currentGameTime.TotalGameTime.TotalMilliseconds % 2000.0 * 0.029999999329447746)), new Rectangle?(new Rectangle(372, 1956, 10, 10)), new Color(80 /*0x50*/, 80 /*0x50*/, 80 /*0x50*/) * transparency * 0.53f * (float) (1.0 - Game1.currentGameTime.TotalGameTime.TotalMilliseconds % 2000.0 / 2000.0), (float) (-Game1.currentGameTime.TotalGameTime.TotalMilliseconds % 2000.0 * (1.0 / 1000.0)), vector2 * scaleSize, scale / 2f, SpriteEffects.None, Math.Min(1f, layerDepth + 2E-05f));
    spriteBatch.Draw(Game1.mouseCursors, location + new Vector2(24f, 40f) * scaleSize + new Vector2(0.0f, (float) (-(Game1.currentGameTime.TotalGameTime.TotalMilliseconds + (double) num) % 2000.0 * 0.029999999329447746)), new Rectangle?(new Rectangle(372, 1956, 10, 10)), new Color(80 /*0x50*/, 80 /*0x50*/, 80 /*0x50*/) * transparency * 0.53f * (float) (1.0 - (Game1.currentGameTime.TotalGameTime.TotalMilliseconds + (double) num) % 2000.0 / 2000.0), (float) (-(Game1.currentGameTime.TotalGameTime.TotalMilliseconds + (double) num) % 2000.0 * (1.0 / 1000.0)), vector2 * scaleSize, scale / 2f, SpriteEffects.None, Math.Min(1f, layerDepth + 2E-05f));
    spriteBatch.Draw(Game1.mouseCursors, location + new Vector2(48f, 21f) * scaleSize + new Vector2(0.0f, (float) (-(Game1.currentGameTime.TotalGameTime.TotalMilliseconds + (double) (num * 2)) % 2000.0 * 0.029999999329447746)), new Rectangle?(new Rectangle(372, 1956, 10, 10)), new Color(80 /*0x50*/, 80 /*0x50*/, 80 /*0x50*/) * transparency * 0.53f * (float) (1.0 - (Game1.currentGameTime.TotalGameTime.TotalMilliseconds + (double) (num * 2)) % 2000.0 / 2000.0), (float) (-(Game1.currentGameTime.TotalGameTime.TotalMilliseconds + (double) (num * 2)) % 2000.0 * (1.0 / 1000.0)), vector2 * scaleSize, scale / 2f, SpriteEffects.None, Math.Min(1f, layerDepth + 2E-05f));
  }

  public override void drawWhenHeld(SpriteBatch spriteBatch, Vector2 objectPosition, Farmer f)
  {
    if (this.ItemId == "SmokedFish")
      this.drawSmokedFish(spriteBatch, objectPosition, 1f, f.getDrawLayer() + 1E-05f);
    else if (!this.ColorSameIndexAsParentSheetIndex)
    {
      base.drawWhenHeld(spriteBatch, objectPosition, f);
      ParsedItemData dataOrErrorItem = ItemRegistry.GetDataOrErrorItem(this.QualifiedItemId);
      spriteBatch.Draw(dataOrErrorItem.GetTexture(), objectPosition, new Rectangle?(dataOrErrorItem.GetSourceRect(1, new int?(this.ParentSheetIndex))), this.color.Value, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, Math.Max(0.0f, (float) (f.StandingPixel.Y + 4) / 10000f));
    }
    else
    {
      ParsedItemData dataOrErrorItem = ItemRegistry.GetDataOrErrorItem(this.QualifiedItemId);
      spriteBatch.Draw(dataOrErrorItem.GetTexture(), objectPosition, new Rectangle?(dataOrErrorItem.GetSourceRect(spriteIndex: new int?(this.ParentSheetIndex))), this.color.Value, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, Math.Max(0.0f, (float) (f.StandingPixel.Y + 4) / 10000f));
    }
  }

  /// <summary>Get the hue value for the current <see cref="F:StardewValley.Objects.ColoredObject.color" />.</summary>
  public double GetHue()
  {
    Color color = this.color.Value;
    double h;
    Utility.RGBtoHSL((int) color.R, (int) color.G, (int) color.B, out h, out double _, out double _);
    return h;
  }

  /// <inheritdoc />
  protected override Item GetOneNew() => (Item) new ColoredObject(this.ItemId, 1, this.color.Value);

  /// <inheritdoc />
  protected override void GetOneCopyFrom(Item source)
  {
    base.GetOneCopyFrom(source);
    if (!(source is ColoredObject coloredObject))
      return;
    this.preserve.Value = coloredObject.preserve.Value;
    this.preservedParentSheetIndex.Value = coloredObject.preservedParentSheetIndex.Value;
    this.Name = coloredObject.Name;
    this.colorSameIndexAsParentSheetIndex.Value = coloredObject.colorSameIndexAsParentSheetIndex.Value;
  }

  public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1f)
  {
    if (this.bigCraftable.Value)
    {
      Vector2 scale = this.getScale();
      Vector2 local = Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (x * 64 /*0x40*/), (float) (y * 64 /*0x40*/ - 64 /*0x40*/)));
      Rectangle destinationRectangle = new Rectangle((int) ((double) local.X - (double) scale.X / 2.0), (int) ((double) local.Y - (double) scale.Y / 2.0), (int) (64.0 + (double) scale.X), (int) (128.0 + (double) scale.Y / 2.0));
      int offset = 0;
      if (this.showNextIndex.Value)
        offset = 1;
      ParsedItemData dataOrErrorItem = ItemRegistry.GetDataOrErrorItem(this.QualifiedItemId);
      Texture2D texture = dataOrErrorItem.GetTexture();
      if (!this.ColorSameIndexAsParentSheetIndex)
      {
        Rectangle sourceRect = dataOrErrorItem.GetSourceRect(offset + 1, new int?(this.ParentSheetIndex));
        spriteBatch.Draw(texture, destinationRectangle, new Rectangle?(dataOrErrorItem.GetSourceRect(offset, new int?(this.ParentSheetIndex))), Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, Math.Max(0.0f, (float) ((y + 1) * 64 /*0x40*/ - 1) / 10000f));
        spriteBatch.Draw(texture, destinationRectangle, new Rectangle?(sourceRect), this.color.Value, 0.0f, Vector2.Zero, SpriteEffects.None, Math.Max(0.0f, (float) ((y + 1) * 64 /*0x40*/ - 1) / 10000f));
      }
      else
        spriteBatch.Draw(texture, destinationRectangle, new Rectangle?(dataOrErrorItem.GetSourceRect(spriteIndex: new int?(this.ParentSheetIndex))), this.color.Value, 0.0f, Vector2.Zero, SpriteEffects.None, Math.Max(0.0f, (float) ((y + 1) * 64 /*0x40*/ - 1) / 10000f));
      if (!(this.QualifiedItemId == "(BC)17") || this.MinutesUntilReady <= 0)
        return;
      spriteBatch.Draw(Game1.objectSpriteSheet, this.getLocalPosition(Game1.viewport) + new Vector2(32f, 0.0f), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, 435, 16 /*0x10*/, 16 /*0x10*/)), Color.White, this.scale.X, new Vector2(32f, 32f), 1f, SpriteEffects.None, Math.Max(0.0f, (float) ((y + 1) * 64 /*0x40*/ - 1) / 10000f));
    }
    else
    {
      if (Game1.eventUp && !this.Location.IsFarm)
        return;
      if (this.QualifiedItemId != "(O)590")
        spriteBatch.Draw(Game1.shadowTexture, this.getLocalPosition(Game1.viewport) + new Vector2(32f, 53f), new Rectangle?(Game1.shadowTexture.Bounds), Color.White, 0.0f, new Vector2((float) Game1.shadowTexture.Bounds.Center.X, (float) Game1.shadowTexture.Bounds.Center.Y), 4f, SpriteEffects.None, 1E-07f);
      ParsedItemData dataOrErrorItem = ItemRegistry.GetDataOrErrorItem(this.QualifiedItemId);
      Texture2D texture = dataOrErrorItem.GetTexture();
      Rectangle boundingBoxAt = this.GetBoundingBoxAt(x, y);
      if (!this.ColorSameIndexAsParentSheetIndex)
      {
        Rectangle sourceRect = dataOrErrorItem.GetSourceRect(1, new int?(this.ParentSheetIndex));
        spriteBatch.Draw(texture, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (x * 64 /*0x40*/ + 32 /*0x20*/), (float) (y * 64 /*0x40*/ + 32 /*0x20*/))), new Rectangle?(dataOrErrorItem.GetSourceRect(spriteIndex: new int?(this.ParentSheetIndex))), Color.White, 0.0f, new Vector2(8f, 8f), (double) this.scale.Y > 1.0 ? this.getScale().Y : 4f, this.flipped.Value ? SpriteEffects.FlipHorizontally : SpriteEffects.None, (float) boundingBoxAt.Bottom / 10000f);
        spriteBatch.Draw(texture, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (x * 64 /*0x40*/ + 32 /*0x20*/ + (this.shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0)), (float) (y * 64 /*0x40*/ + 32 /*0x20*/ + (this.shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0)))), new Rectangle?(sourceRect), this.color.Value, 0.0f, new Vector2(8f, 8f), (double) this.scale.Y > 1.0 ? this.getScale().Y : 4f, this.flipped.Value ? SpriteEffects.FlipHorizontally : SpriteEffects.None, (float) boundingBoxAt.Bottom / 10000f);
      }
      else
        spriteBatch.Draw(texture, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (x * 64 /*0x40*/ + 32 /*0x20*/ + (this.shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0)), (float) (y * 64 /*0x40*/ + 32 /*0x20*/ + (this.shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0)))), new Rectangle?(dataOrErrorItem.GetSourceRect(spriteIndex: new int?(this.ParentSheetIndex))), this.color.Value, 0.0f, new Vector2(8f, 8f), (double) this.scale.Y > 1.0 ? this.getScale().Y : 4f, this.flipped.Value ? SpriteEffects.FlipHorizontally : SpriteEffects.None, (float) boundingBoxAt.Bottom / 10000f);
    }
  }

  /// <summary>Set the tint color for an item, if it's a <see cref="T:StardewValley.Objects.ColoredObject" /> or can be converted to one.</summary>
  /// <param name="input">The input item whose color to set.</param>
  /// <param name="color">The tint color to apply.</param>
  /// <param name="coloredItem">The resulting colored item. This may be <paramref name="input" /> (if it was already a <see cref="T:StardewValley.Objects.ColoredObject" />), a new item (if the <paramref name="input" /> can be converted to a <see cref="T:StardewValley.Objects.ColoredObject" />), else null.</param>
  /// <returns>Returns whether the <paramref name="coloredItem" /> was successfully set.</returns>
  public static bool TrySetColor(Item input, Color color, out ColoredObject coloredItem)
  {
    if (input == null)
    {
      coloredItem = (ColoredObject) null;
      return false;
    }
    coloredItem = input as ColoredObject;
    if (coloredItem != null)
    {
      coloredItem.color.Value = color;
      return true;
    }
    if (input.HasTypeObject())
    {
      coloredItem = new ColoredObject(input.ItemId, input.Stack, color);
      coloredItem.CopyFieldsFrom(input);
      coloredItem.color.Value = color;
      return true;
    }
    coloredItem = (ColoredObject) null;
    return false;
  }
}
