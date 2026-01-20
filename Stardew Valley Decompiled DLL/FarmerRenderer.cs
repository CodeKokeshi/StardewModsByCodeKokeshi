// Decompiled with JetBrains decompiler
// Type: StardewValley.FarmerRenderer
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Extensions;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Objects;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley;

[InstanceStatics]
public class FarmerRenderer : INetObject<NetFields>
{
  public const int sleeveDarkestColorIndex = 256 /*0x0100*/;
  public const int skinDarkestColorIndex = 260;
  public const int shoeDarkestColorIndex = 268;
  public const int eyeLightestColorIndex = 276;
  public const int accessoryDrawBelowHairThreshold = 8;
  public const int accessoryFacialHairThreshold = 6;
  protected bool _sickFrame;
  public static bool isDrawingForUI = false;
  public const int TransparentSkin = -12345;
  public const int pantsOffset = 288;
  public const int armOffset = 96 /*0x60*/;
  public const int shirtXOffset = 16 /*0x10*/;
  public const int shirtYOffset = 56;
  public static int[] featureYOffsetPerFrame = new int[126]
  {
    1,
    2,
    2,
    0,
    5,
    6,
    1,
    2,
    2,
    1,
    0,
    2,
    0,
    1,
    1,
    0,
    2,
    2,
    3,
    3,
    2,
    2,
    1,
    1,
    0,
    0,
    2,
    2,
    4,
    4,
    0,
    0,
    1,
    2,
    1,
    1,
    1,
    1,
    0,
    0,
    1,
    1,
    1,
    0,
    0,
    -2,
    -1,
    1,
    1,
    0,
    -1,
    -2,
    -1,
    -1,
    5,
    4,
    0,
    0,
    3,
    2,
    -1,
    0,
    4,
    2,
    0,
    0,
    2,
    1,
    0,
    -1,
    1,
    -2,
    0,
    0,
    1,
    1,
    1,
    1,
    1,
    1,
    0,
    0,
    0,
    0,
    1,
    -1,
    -1,
    -1,
    -1,
    1,
    1,
    0,
    0,
    0,
    0,
    4,
    1,
    0,
    1,
    2,
    1,
    0,
    1,
    0,
    1,
    2,
    -3,
    -4,
    -1,
    0,
    0,
    2,
    1,
    -4,
    -1,
    0,
    0,
    -3,
    0,
    0,
    -1,
    0,
    0,
    2,
    1,
    1
  };
  public static int[] featureXOffsetPerFrame = new int[126]
  {
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    -1,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    -1,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    -1,
    -1,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    4,
    0,
    0,
    0,
    0,
    -1,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    -1,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0
  };
  public static int[] hairstyleHatOffset = new int[16 /*0x10*/]
  {
    0,
    0,
    0,
    4,
    0,
    0,
    3,
    0,
    4,
    0,
    0,
    0,
    0,
    0,
    0,
    0
  };
  public static Texture2D hairStylesTexture;
  public static Texture2D shirtsTexture;
  public static Texture2D hatsTexture;
  public static Texture2D accessoriesTexture;
  public static Texture2D pantsTexture;
  public static Dictionary<string, Dictionary<int, List<int>>> recolorOffsets;
  [XmlElement("textureName")]
  public readonly NetString textureName = new NetString();
  [XmlIgnore]
  private LocalizedContentManager farmerTextureManager;
  [XmlIgnore]
  internal Texture2D baseTexture;
  [XmlElement("heightOffset")]
  public readonly NetInt heightOffset = new NetInt(0);
  [XmlIgnore]
  public readonly NetColor eyes = new NetColor();
  [XmlIgnore]
  public readonly NetInt skin = new NetInt();
  [XmlIgnore]
  public readonly NetString shoes = new NetString();
  [XmlIgnore]
  public readonly NetString shirt = new NetString();
  [XmlIgnore]
  public readonly NetString pants = new NetString();
  protected bool _spriteDirty;
  protected bool _baseTextureDirty;
  protected bool _eyesDirty;
  protected bool _skinDirty;
  protected bool _shoesDirty;
  protected bool _shirtDirty;
  protected bool _pantsDirty;
  public Rectangle shirtSourceRect;
  public Rectangle hairstyleSourceRect;
  public Rectangle hatSourceRect;
  public Rectangle accessorySourceRect;
  public Vector2 rotationAdjustment;
  public Vector2 positionOffset;

  [XmlIgnore]
  public NetFields NetFields { get; } = new NetFields(nameof (FarmerRenderer));

  public FarmerRenderer()
  {
    this.NetFields.SetOwner((INetObject<NetFields>) this).AddField((INetSerializable) this.textureName, nameof (textureName)).AddField((INetSerializable) this.heightOffset, nameof (heightOffset)).AddField((INetSerializable) this.eyes, nameof (eyes)).AddField((INetSerializable) this.skin, nameof (skin)).AddField((INetSerializable) this.shoes, nameof (shoes)).AddField((INetSerializable) this.shirt, nameof (shirt)).AddField((INetSerializable) this.pants, nameof (pants));
    this.farmerTextureManager = Game1.content.CreateTemporary();
    this.textureName.fieldChangeVisibleEvent += (FieldChange<NetString, string>) ((_param1, _param2, _param3) =>
    {
      this._spriteDirty = true;
      this._baseTextureDirty = true;
    });
    this.eyes.fieldChangeVisibleEvent += (FieldChange<NetColor, Color>) ((_param1, _param2, _param3) =>
    {
      this._spriteDirty = true;
      this._eyesDirty = true;
    });
    this.skin.fieldChangeVisibleEvent += (FieldChange<NetInt, int>) ((_param1, _param2, _param3) =>
    {
      this._spriteDirty = true;
      this._skinDirty = true;
      this._shirtDirty = true;
    });
    this.shoes.fieldChangeVisibleEvent += (FieldChange<NetString, string>) ((_param1, _param2, _param3) =>
    {
      this._spriteDirty = true;
      this._shoesDirty = true;
    });
    this.shirt.fieldChangeVisibleEvent += (FieldChange<NetString, string>) ((_param1, _param2, _param3) =>
    {
      this._spriteDirty = true;
      this._shirtDirty = true;
    });
    this.pants.fieldChangeVisibleEvent += (FieldChange<NetString, string>) ((_param1, _param2, _param3) =>
    {
      this._spriteDirty = true;
      this._pantsDirty = true;
    });
    this._spriteDirty = true;
    this._baseTextureDirty = true;
  }

  public FarmerRenderer(string textureName, Farmer farmer)
    : this()
  {
    this.eyes.Set(farmer.newEyeColor.Value);
    this.textureName.Set(textureName);
    this._spriteDirty = true;
    this._baseTextureDirty = true;
  }

  public bool isAccessoryFacialHair(int which)
  {
    if (which < 6)
      return true;
    return which >= 19 && which <= 22;
  }

  public bool drawAccessoryBelowHair(int which) => which < 8 || this.isAccessoryFacialHair(which);

  private void executeRecolorActions(Farmer farmer)
  {
    if (!this._spriteDirty)
      return;
    this._spriteDirty = false;
    if (this._baseTextureDirty)
    {
      this._baseTextureDirty = false;
      this.textureChanged();
      this._eyesDirty = true;
      this._shoesDirty = true;
      this._pantsDirty = true;
      this._skinDirty = true;
      this._shirtDirty = true;
    }
    if (FarmerRenderer.recolorOffsets == null)
      FarmerRenderer.recolorOffsets = new Dictionary<string, Dictionary<int, List<int>>>();
    if (!FarmerRenderer.recolorOffsets.ContainsKey(this.textureName.Value))
    {
      FarmerRenderer.recolorOffsets[this.textureName.Value] = new Dictionary<int, List<int>>();
      Texture2D texture2D = this.farmerTextureManager.Load<Texture2D>(this.textureName.Value);
      Color[] colorArray = new Color[texture2D.Width * texture2D.Height];
      texture2D.GetData<Color>(colorArray);
      this._GeneratePixelIndices(256 /*0x0100*/, this.textureName.Value, colorArray);
      this._GeneratePixelIndices(257, this.textureName.Value, colorArray);
      this._GeneratePixelIndices(258, this.textureName.Value, colorArray);
      this._GeneratePixelIndices(268, this.textureName.Value, colorArray);
      this._GeneratePixelIndices(269, this.textureName.Value, colorArray);
      this._GeneratePixelIndices(270, this.textureName.Value, colorArray);
      this._GeneratePixelIndices(271, this.textureName.Value, colorArray);
      this._GeneratePixelIndices(260, this.textureName.Value, colorArray);
      this._GeneratePixelIndices(261, this.textureName.Value, colorArray);
      this._GeneratePixelIndices(262, this.textureName.Value, colorArray);
      this._GeneratePixelIndices(276, this.textureName.Value, colorArray);
      this._GeneratePixelIndices(277, this.textureName.Value, colorArray);
    }
    Color[] colorArray1 = new Color[this.baseTexture.Width * this.baseTexture.Height];
    this.baseTexture.GetData<Color>(colorArray1);
    if (this._eyesDirty)
    {
      this._eyesDirty = false;
      this.ApplyEyeColor(this.textureName.Value, colorArray1);
    }
    if (this._skinDirty)
    {
      this._skinDirty = false;
      this.ApplySkinColor(this.textureName.Value, colorArray1);
    }
    if (this._shoesDirty)
    {
      this._shoesDirty = false;
      this.ApplyShoeColor(this.textureName.Value, colorArray1);
    }
    if (this._shirtDirty)
    {
      this._shirtDirty = false;
      this.ApplySleeveColor(this.textureName.Value, colorArray1, farmer);
    }
    if (this._pantsDirty)
      this._pantsDirty = false;
    this.baseTexture.SetData<Color>(colorArray1);
  }

  protected void _GeneratePixelIndices(int source_color_index, string texture_name, Color[] pixels)
  {
    Color pixel = pixels[source_color_index];
    List<int> intList = new List<int>();
    for (int index = 0; index < pixels.Length; ++index)
    {
      if ((int) pixels[index].PackedValue == (int) pixel.PackedValue)
        intList.Add(index);
    }
    FarmerRenderer.recolorOffsets[texture_name][source_color_index] = intList;
  }

  public void unload()
  {
    this.farmerTextureManager.Unload();
    this.farmerTextureManager.Dispose();
  }

  public void textureChanged()
  {
    if (this.baseTexture != null)
    {
      this.baseTexture.Dispose();
      this.baseTexture = (Texture2D) null;
    }
    Texture2D texture = this.farmerTextureManager.Load<Texture2D>(this.textureName.Value);
    Texture2D texture2D = new Texture2D(Game1.graphics.GraphicsDevice, texture.GetActualWidth(), texture.GetActualHeight());
    texture2D.Name = "@FarmerRenderer.baseTexture";
    this.baseTexture = texture2D;
    Color[] data = new Color[texture.GetElementCount()];
    texture.GetData<Color>(data, 0, data.Length);
    this.baseTexture.SetData<Color>(data);
  }

  public void recolorEyes(Color lightestColor) => this.eyes.Set(lightestColor);

  public void ApplyEyeColor(string texture_name, Color[] pixels)
  {
    Color color1 = this.eyes.Value;
    Color color2 = FarmerRenderer.changeBrightness(color1, -75);
    if (color1.Equals(color2))
      color1.B += (byte) 10;
    this._SwapColor(texture_name, pixels, 276, color1);
    this._SwapColor(texture_name, pixels, 277, color2);
  }

  private void _SwapColor(string texture_name, Color[] pixels, int color_index, Color color)
  {
    foreach (int index in FarmerRenderer.recolorOffsets[texture_name][color_index])
      pixels[index] = color;
  }

  public void recolorShoes(string which) => this.shoes.Set(which);

  private void ApplyShoeColor(string texture_name, Color[] pixels)
  {
    int result = 12;
    Texture2D texture2D1 = (Texture2D) null;
    int length = this.shoes.Value.LastIndexOf(':');
    if (length > -1)
    {
      string assetName = this.shoes.Value.Substring(0, length);
      string s = this.shoes.Value.Substring(length + 1);
      try
      {
        texture2D1 = this.farmerTextureManager.Load<Texture2D>(assetName);
        if (!int.TryParse(s, out result))
          result = 12;
      }
      catch (Exception ex)
      {
        texture2D1 = this.farmerTextureManager.Load<Texture2D>("Characters\\Farmer\\shoeColors");
      }
    }
    else if (!int.TryParse(this.shoes.Value, out result))
      result = 12;
    if (texture2D1 == null)
      texture2D1 = this.farmerTextureManager.Load<Texture2D>("Characters\\Farmer\\shoeColors");
    Texture2D texture2D2 = texture2D1;
    if (result >= texture2D2.Height)
      result = texture2D2.Height - 1;
    if (texture2D2.Width < 4)
      return;
    Color[] data = new Color[texture2D2.Width * texture2D2.Height];
    texture2D2.GetData<Color>(data);
    Color color1 = data[result * 4 % (texture2D2.Height * 4)];
    Color color2 = data[result * 4 % (texture2D2.Height * 4) + 1];
    Color color3 = data[result * 4 % (texture2D2.Height * 4) + 2];
    Color color4 = data[result * 4 % (texture2D2.Height * 4) + 3];
    this._SwapColor(texture_name, pixels, 268, color1);
    this._SwapColor(texture_name, pixels, 269, color2);
    this._SwapColor(texture_name, pixels, 270, color3);
    this._SwapColor(texture_name, pixels, 271, color4);
  }

  public int recolorSkin(int which, bool force = false)
  {
    if (force)
      this.skin.Value = -1;
    this.skin.Set(which);
    return which;
  }

  private void ApplySkinColor(string texture_name, Color[] pixels)
  {
    int num = this.skin.Value;
    Texture2D texture2D = this.farmerTextureManager.Load<Texture2D>("Characters\\Farmer\\skinColors");
    Color[] data = new Color[texture2D.Width * texture2D.Height];
    if (num < 0)
      num = texture2D.Height - 1;
    if (num > texture2D.Height - 1)
      num = 0;
    texture2D.GetData<Color>(data);
    Color color1 = data[num * 3 % (texture2D.Height * 3)];
    Color color2 = data[num * 3 % (texture2D.Height * 3) + 1];
    Color color3 = data[num * 3 % (texture2D.Height * 3) + 2];
    if (this.skin.Value == -12345)
    {
      Color transparent;
      color3 = transparent = Color.Transparent;
      color2 = transparent;
      color1 = transparent;
    }
    this._SwapColor(texture_name, pixels, 260, color1);
    this._SwapColor(texture_name, pixels, 261, color2);
    this._SwapColor(texture_name, pixels, 262, color3);
  }

  public void changeShirt(string whichShirt) => this.shirt.Set(whichShirt);

  public void changePants(string whichPants) => this.pants.Set(whichPants);

  public void MarkSpriteDirty()
  {
    this._spriteDirty = true;
    this._shirtDirty = true;
    this._pantsDirty = true;
    this._eyesDirty = true;
    this._shoesDirty = true;
    this._baseTextureDirty = true;
  }

  public void ApplySleeveColor(string texture_name, Color[] pixels, Farmer who)
  {
    Texture2D texture;
    int spriteIndex;
    who.GetDisplayShirt(out texture, out spriteIndex);
    Color[] data1 = new Color[texture.Bounds.Width * texture.Bounds.Height];
    texture.GetData<Color>(data1);
    int index1 = spriteIndex * 8 / 128 /*0x80*/ * 32 /*0x20*/ * texture.Bounds.Width + spriteIndex * 8 % 128 /*0x80*/ + texture.Width * 4;
    int index2 = index1 + 128 /*0x80*/;
    if (!who.ShirtHasSleeves() || index1 >= data1.Length || this.skin.Value == -12345 && who.shirtItem.Value == null)
    {
      Texture2D texture2D = this.farmerTextureManager.Load<Texture2D>("Characters\\Farmer\\skinColors");
      Color[] data2 = new Color[texture2D.Width * texture2D.Height];
      int num = this.skin.Value;
      if (num < 0)
        num = texture2D.Height - 1;
      if (num > texture2D.Height - 1)
        num = 0;
      texture2D.GetData<Color>(data2);
      Color pixel1 = data2[num * 3 % (texture2D.Height * 3)];
      Color pixel2 = data2[num * 3 % (texture2D.Height * 3) + 1];
      Color pixel3 = data2[num * 3 % (texture2D.Height * 3) + 2];
      if (this.skin.Value == -12345)
      {
        pixel1 = pixels[260 + this.baseTexture.Width * 2];
        pixel2 = pixels[261 + this.baseTexture.Width * 2];
        pixel3 = pixels[262 + this.baseTexture.Width * 2];
      }
      if (this._sickFrame)
      {
        pixel1 = pixels[260 + this.baseTexture.Width];
        pixel2 = pixels[261 + this.baseTexture.Width];
        pixel3 = pixels[262 + this.baseTexture.Width];
      }
      this._SwapColor(texture_name, pixels, 256 /*0x0100*/, pixel1);
      this._SwapColor(texture_name, pixels, 257, pixel2);
      this._SwapColor(texture_name, pixels, 258, pixel3);
    }
    else
    {
      Color color1 = Utility.MakeCompletelyOpaque(who.GetShirtColor());
      Color a1 = data1[index2];
      Color b = color1;
      if (a1.A < byte.MaxValue)
      {
        a1 = data1[index1];
        b = Color.White;
      }
      Color color2 = Utility.MultiplyColor(a1, b);
      this._SwapColor(texture_name, pixels, 256 /*0x0100*/, color2);
      Color a2 = data1[index2 - texture.Width];
      if (a2.A < byte.MaxValue)
      {
        a2 = data1[index1 - texture.Width];
        b = Color.White;
      }
      Color color3 = Utility.MultiplyColor(a2, b);
      this._SwapColor(texture_name, pixels, 257, color3);
      Color a3 = data1[index2 - texture.Width * 2];
      if (a3.A < byte.MaxValue)
      {
        a3 = data1[index1 - texture.Width * 2];
        b = Color.White;
      }
      Color color4 = Utility.MultiplyColor(a3, b);
      this._SwapColor(texture_name, pixels, 258, color4);
    }
  }

  public static Color changeBrightness(Color c, int brightness)
  {
    c.R = (byte) Math.Min((int) byte.MaxValue, Math.Max(0, (int) c.R + brightness));
    c.G = (byte) Math.Min((int) byte.MaxValue, Math.Max(0, (int) c.G + brightness));
    c.B = (byte) Math.Min((int) byte.MaxValue, Math.Max(0, (int) c.B + (brightness > 0 ? brightness * 5 / 6 : brightness * 8 / 7)));
    return c;
  }

  public void draw(
    SpriteBatch b,
    Farmer who,
    int whichFrame,
    Vector2 position,
    float layerDepth = 1f,
    bool flip = false)
  {
    who.FarmerSprite.setCurrentSingleFrame(whichFrame, flip: flip);
    this.draw(b, who.FarmerSprite, who.FarmerSprite.SourceRect, position, Vector2.Zero, layerDepth, Color.White, 0.0f, who);
  }

  public void draw(
    SpriteBatch b,
    FarmerSprite farmerSprite,
    Rectangle sourceRect,
    Vector2 position,
    Vector2 origin,
    float layerDepth,
    Color overrideColor,
    float rotation,
    Farmer who)
  {
    this.draw(b, farmerSprite.CurrentAnimationFrame, farmerSprite.CurrentFrame, sourceRect, position, origin, layerDepth, overrideColor, rotation, 1f, who);
  }

  public void drawMiniPortrat(
    SpriteBatch b,
    Vector2 position,
    float layerDepth,
    float scale,
    int facingDirection,
    Farmer who,
    float alpha = 1f)
  {
    int hair = who.getHair(true);
    this.executeRecolorActions(who);
    facingDirection = 2;
    bool flag = false;
    int y = 0;
    int num = 0;
    HairStyleMetadata hairStyleMetadata = Farmer.GetHairStyleMetadata(who.hair.Value);
    Texture2D texture = hairStyleMetadata?.texture ?? FarmerRenderer.hairStylesTexture;
    this.hairstyleSourceRect = hairStyleMetadata != null ? new Rectangle(hairStyleMetadata.tileX * 16 /*0x10*/, hairStyleMetadata.tileY * 16 /*0x10*/, 16 /*0x10*/, 15) : new Rectangle(hair * 16 /*0x10*/ % FarmerRenderer.hairStylesTexture.Width, hair * 16 /*0x10*/ / FarmerRenderer.hairStylesTexture.Width * 96 /*0x60*/, 16 /*0x10*/, 15);
    if (facingDirection == 2)
    {
      y = 0;
      this.hairstyleSourceRect.Offset(0, 0);
      num = FarmerRenderer.featureYOffsetPerFrame[0];
    }
    b.Draw(this.baseTexture, position, new Rectangle?(new Rectangle(0, y, 16 /*0x10*/, who.IsMale ? 15 : 16 /*0x10*/)), Color.White * alpha, 0.0f, Vector2.Zero, scale, flag ? SpriteEffects.FlipHorizontally : SpriteEffects.None, FarmerRenderer.GetLayerDepth(layerDepth, FarmerRenderer.FarmerSpriteLayers.Base));
    Color color = who.prismaticHair.Value ? Utility.GetPrismaticColor() : who.hairstyleColor.Value;
    b.Draw(texture, position + new Vector2(0.0f, (float) (num * 4 + (!who.IsMale || who.hair.Value < 16 /*0x10*/ ? (who.IsMale || who.hair.Value >= 16 /*0x10*/ ? 0 : 4) : -4))) * scale / 4f, new Rectangle?(this.hairstyleSourceRect), color * alpha, 0.0f, Vector2.Zero, scale, flag ? SpriteEffects.FlipHorizontally : SpriteEffects.None, FarmerRenderer.GetLayerDepth(layerDepth, FarmerRenderer.FarmerSpriteLayers.Hair));
  }

  public void draw(
    SpriteBatch b,
    FarmerSprite.AnimationFrame animationFrame,
    int currentFrame,
    Rectangle sourceRect,
    Vector2 position,
    Vector2 origin,
    float layerDepth,
    Color overrideColor,
    float rotation,
    float scale,
    Farmer who)
  {
    this.draw(b, animationFrame, currentFrame, sourceRect, position, origin, layerDepth, who.FacingDirection, overrideColor, rotation, scale, who);
  }

  public void drawHairAndAccesories(
    SpriteBatch b,
    int facingDirection,
    Farmer who,
    Vector2 position,
    Vector2 origin,
    float scale,
    int currentFrame,
    float rotation,
    Color overrideColor,
    float layerDepth)
  {
    int hair_index = who.getHair();
    float scale1 = 4f * scale;
    int num1 = FarmerRenderer.featureXOffsetPerFrame[currentFrame];
    int num2 = FarmerRenderer.featureYOffsetPerFrame[currentFrame];
    HairStyleMetadata hairStyleMetadata = Farmer.GetHairStyleMetadata(hair_index);
    Hat hat = who.hat.Value;
    if ((hat != null ? (hat.hairDrawType.Value == 1 ? 1 : 0) : 0) != 0 && hairStyleMetadata != null && hairStyleMetadata.coveredIndex != -1)
    {
      hair_index = hairStyleMetadata.coveredIndex;
      hairStyleMetadata = Farmer.GetHairStyleMetadata(hair_index);
    }
    this.executeRecolorActions(who);
    Texture2D texture1;
    int spriteIndex1;
    who.GetDisplayShirt(out texture1, out spriteIndex1);
    Color color1 = who.prismaticHair.Value ? Utility.GetPrismaticColor() : who.hairstyleColor.Value;
    this.shirtSourceRect = new Rectangle(spriteIndex1 * 8 % 128 /*0x80*/, spriteIndex1 * 8 / 128 /*0x80*/ * 32 /*0x20*/, 8, 8);
    Texture2D texture2 = hairStyleMetadata?.texture ?? FarmerRenderer.hairStylesTexture;
    this.hairstyleSourceRect = hairStyleMetadata != null ? new Rectangle(hairStyleMetadata.tileX * 16 /*0x10*/, hairStyleMetadata.tileY * 16 /*0x10*/, 16 /*0x10*/, 32 /*0x20*/) : new Rectangle(hair_index * 16 /*0x10*/ % FarmerRenderer.hairStylesTexture.Width, hair_index * 16 /*0x10*/ / FarmerRenderer.hairStylesTexture.Width * 96 /*0x60*/, 16 /*0x10*/, 32 /*0x20*/);
    if (who.accessory.Value >= 0)
      this.accessorySourceRect = new Rectangle(who.accessory.Value * 16 /*0x10*/ % FarmerRenderer.accessoriesTexture.Width, who.accessory.Value * 16 /*0x10*/ / FarmerRenderer.accessoriesTexture.Width * 32 /*0x20*/, 16 /*0x10*/, 16 /*0x10*/);
    Texture2D texture3 = FarmerRenderer.hatsTexture;
    bool flag1 = false;
    if (who.hat.Value != null)
    {
      ParsedItemData dataOrErrorItem = ItemRegistry.GetDataOrErrorItem(who.hat.Value.QualifiedItemId);
      int spriteIndex2 = dataOrErrorItem.SpriteIndex;
      texture3 = dataOrErrorItem.GetTexture();
      this.hatSourceRect = new Rectangle(20 * spriteIndex2 % texture3.Width, 20 * spriteIndex2 / texture3.Width * 20 * 4, 20, 20);
      if (dataOrErrorItem.IsErrorItem)
      {
        this.hatSourceRect = dataOrErrorItem.GetSourceRect();
        flag1 = true;
      }
    }
    FarmerRenderer.FarmerSpriteLayers layer = FarmerRenderer.FarmerSpriteLayers.Accessory;
    if (who.accessory.Value >= 0 && this.drawAccessoryBelowHair(who.accessory.Value))
      layer = FarmerRenderer.FarmerSpriteLayers.AccessoryUnderHair;
    switch (facingDirection)
    {
      case 0:
        this.shirtSourceRect.Offset(0, 24);
        this.hairstyleSourceRect.Offset(0, 64 /*0x40*/);
        Rectangle shirtSourceRect1 = this.shirtSourceRect;
        shirtSourceRect1.Offset(128 /*0x80*/, 0);
        if (!flag1 && who.hat.Value != null)
          this.hatSourceRect.Offset(0, 60);
        if (!who.bathingClothes.Value && (this.skin.Value != -12345 || who.shirtItem.Value != null))
        {
          Vector2 position1 = position + origin + this.positionOffset + new Vector2(16f * scale + (float) (num1 * 4), (float) (56 + num2 * 4) + (float) this.heightOffset.Value * scale);
          b.Draw(texture1, position1, new Rectangle?(this.shirtSourceRect), overrideColor.Equals(Color.White) ? Color.White : overrideColor, rotation, origin, scale1, SpriteEffects.None, FarmerRenderer.GetLayerDepth(layerDepth, FarmerRenderer.FarmerSpriteLayers.Shirt));
          b.Draw(texture1, position1, new Rectangle?(shirtSourceRect1), overrideColor.Equals(Color.White) ? Utility.MakeCompletelyOpaque(who.GetShirtColor()) : overrideColor, rotation, origin, scale1, SpriteEffects.None, FarmerRenderer.GetLayerDepth(layerDepth, FarmerRenderer.FarmerSpriteLayers.Shirt, true));
        }
        b.Draw(texture2, position + origin + this.positionOffset + new Vector2((float) (num1 * 4), (float) (num2 * 4 + 4 + (!who.IsMale || hair_index < 16 /*0x10*/ ? (who.IsMale || hair_index >= 16 /*0x10*/ ? 0 : 4) : -4))), new Rectangle?(this.hairstyleSourceRect), overrideColor.Equals(Color.White) ? color1 : overrideColor, rotation, origin, scale1, SpriteEffects.None, FarmerRenderer.GetLayerDepth(layerDepth, FarmerRenderer.FarmerSpriteLayers.Hair));
        break;
      case 1:
        this.shirtSourceRect.Offset(0, 8);
        this.hairstyleSourceRect.Offset(0, 32 /*0x20*/);
        Rectangle shirtSourceRect2 = this.shirtSourceRect;
        shirtSourceRect2.Offset(128 /*0x80*/, 0);
        if (!flag1 && who.hat.Value != null)
          this.hatSourceRect.Offset(0, 20);
        if ((double) rotation != -0.098174773156642914)
        {
          if ((double) rotation == 0.098174773156642914)
          {
            this.rotationAdjustment.X = -6f;
            this.rotationAdjustment.Y = 1f;
          }
        }
        else
        {
          this.rotationAdjustment.X = 6f;
          this.rotationAdjustment.Y = -2f;
        }
        if (!who.bathingClothes.Value && (this.skin.Value != -12345 || who.shirtItem.Value != null))
        {
          Vector2 position2 = position + origin + this.positionOffset + this.rotationAdjustment + new Vector2(16f * scale + (float) (num1 * 4), (float) (56.0 * (double) scale + (double) (num2 * 4) + (double) this.heightOffset.Value * (double) scale));
          b.Draw(texture1, position2, new Rectangle?(this.shirtSourceRect), overrideColor.Equals(Color.White) ? Color.White : overrideColor, rotation, origin, scale1, SpriteEffects.None, FarmerRenderer.GetLayerDepth(layerDepth, FarmerRenderer.FarmerSpriteLayers.Shirt));
          b.Draw(texture1, position2, new Rectangle?(shirtSourceRect2), overrideColor.Equals(Color.White) ? Utility.MakeCompletelyOpaque(who.GetShirtColor()) : overrideColor, rotation, origin, scale1, SpriteEffects.None, FarmerRenderer.GetLayerDepth(layerDepth, FarmerRenderer.FarmerSpriteLayers.Shirt, true));
        }
        if (who.accessory.Value >= 0)
        {
          this.accessorySourceRect.Offset(0, 16 /*0x10*/);
          b.Draw(FarmerRenderer.accessoriesTexture, position + origin + this.positionOffset + this.rotationAdjustment + new Vector2((float) (num1 * 4), (float) (4 + num2 * 4 + this.heightOffset.Value)), new Rectangle?(this.accessorySourceRect), !overrideColor.Equals(Color.White) || !this.isAccessoryFacialHair(who.accessory.Value) ? overrideColor : color1, rotation, origin, scale1, SpriteEffects.None, FarmerRenderer.GetLayerDepth(layerDepth, layer));
        }
        b.Draw(texture2, position + origin + this.positionOffset + new Vector2((float) (num1 * 4), (float) (num2 * 4 + (!who.IsMale || who.hair.Value < 16 /*0x10*/ ? (who.IsMale || who.hair.Value >= 16 /*0x10*/ ? 0 : 4) : -4))), new Rectangle?(this.hairstyleSourceRect), overrideColor.Equals(Color.White) ? color1 : overrideColor, rotation, origin, scale1, SpriteEffects.None, FarmerRenderer.GetLayerDepth(layerDepth, FarmerRenderer.FarmerSpriteLayers.Hair));
        break;
      case 2:
        Rectangle shirtSourceRect3 = this.shirtSourceRect;
        shirtSourceRect3.Offset(128 /*0x80*/, 0);
        if (!who.bathingClothes.Value && (this.skin.Value != -12345 || who.shirtItem.Value != null))
        {
          Vector2 position3 = position + origin + this.positionOffset + new Vector2((float) (16 /*0x10*/ + num1 * 4), (float) (56 + num2 * 4) + (float) this.heightOffset.Value * scale);
          b.Draw(texture1, position3, new Rectangle?(this.shirtSourceRect), overrideColor.Equals(Color.White) ? Color.White : overrideColor, rotation, origin, scale1, SpriteEffects.None, FarmerRenderer.GetLayerDepth(layerDepth, FarmerRenderer.FarmerSpriteLayers.Shirt));
          b.Draw(texture1, position3, new Rectangle?(shirtSourceRect3), overrideColor.Equals(Color.White) ? Utility.MakeCompletelyOpaque(who.GetShirtColor()) : overrideColor, rotation, origin, scale1, SpriteEffects.None, FarmerRenderer.GetLayerDepth(layerDepth, FarmerRenderer.FarmerSpriteLayers.Shirt, true));
        }
        if (who.accessory.Value >= 0)
        {
          if (who.accessory.Value == 26 && (currentFrame == 70 || currentFrame > 23 && currentFrame < 27))
            this.positionOffset.Y += 4f;
          b.Draw(FarmerRenderer.accessoriesTexture, position + origin + this.positionOffset + this.rotationAdjustment + new Vector2((float) (num1 * 4), (float) (8 + num2 * 4 + this.heightOffset.Value - 4)), new Rectangle?(this.accessorySourceRect), !overrideColor.Equals(Color.White) || !this.isAccessoryFacialHair(who.accessory.Value) ? overrideColor : color1, rotation, origin, scale1, SpriteEffects.None, FarmerRenderer.GetLayerDepth(layerDepth, layer));
        }
        b.Draw(texture2, position + origin + this.positionOffset + new Vector2((float) (num1 * 4), (float) (num2 * 4 + (!who.IsMale || who.hair.Value < 16 /*0x10*/ ? (who.IsMale || who.hair.Value >= 16 /*0x10*/ ? 0 : 4) : -4))), new Rectangle?(this.hairstyleSourceRect), overrideColor.Equals(Color.White) ? color1 : overrideColor, rotation, origin, scale1, SpriteEffects.None, FarmerRenderer.GetLayerDepth(layerDepth, FarmerRenderer.FarmerSpriteLayers.Hair));
        break;
      case 3:
        bool flag2 = true;
        this.shirtSourceRect.Offset(0, 16 /*0x10*/);
        Rectangle shirtSourceRect4 = this.shirtSourceRect;
        shirtSourceRect4.Offset(128 /*0x80*/, 0);
        if (hairStyleMetadata != null && hairStyleMetadata.usesUniqueLeftSprite)
        {
          flag2 = false;
          this.hairstyleSourceRect.Offset(0, 96 /*0x60*/);
        }
        else
          this.hairstyleSourceRect.Offset(0, 32 /*0x20*/);
        if (!flag1 && who.hat.Value != null)
          this.hatSourceRect.Offset(0, 40);
        if ((double) rotation != -0.098174773156642914)
        {
          if ((double) rotation == 0.098174773156642914)
          {
            this.rotationAdjustment.X = -5f;
            this.rotationAdjustment.Y = 1f;
          }
        }
        else
        {
          this.rotationAdjustment.X = 6f;
          this.rotationAdjustment.Y = -2f;
        }
        if (!who.bathingClothes.Value && (this.skin.Value != -12345 || who.shirtItem.Value != null))
        {
          Vector2 position4 = position + origin + this.positionOffset + this.rotationAdjustment + new Vector2(16f * scale - (float) (num1 * 4), (float) (56.0 * (double) scale + (double) (num2 * 4) + (double) this.heightOffset.Value * (double) scale));
          b.Draw(texture1, position4, new Rectangle?(this.shirtSourceRect), overrideColor.Equals(Color.White) ? Color.White : overrideColor, rotation, origin, scale1, SpriteEffects.None, FarmerRenderer.GetLayerDepth(layerDepth, FarmerRenderer.FarmerSpriteLayers.Shirt));
          b.Draw(texture1, position4, new Rectangle?(shirtSourceRect4), overrideColor.Equals(Color.White) ? Utility.MakeCompletelyOpaque(who.GetShirtColor()) : overrideColor, rotation, origin, scale1, SpriteEffects.None, FarmerRenderer.GetLayerDepth(layerDepth, FarmerRenderer.FarmerSpriteLayers.Shirt, true));
        }
        if (who.accessory.Value >= 0)
        {
          this.accessorySourceRect.Offset(0, 16 /*0x10*/);
          b.Draw(FarmerRenderer.accessoriesTexture, position + origin + this.positionOffset + this.rotationAdjustment + new Vector2((float) (-num1 * 4), (float) (4 + num2 * 4 + this.heightOffset.Value)), new Rectangle?(this.accessorySourceRect), !overrideColor.Equals(Color.White) || !this.isAccessoryFacialHair(who.accessory.Value) ? overrideColor : color1, rotation, origin, scale1, SpriteEffects.FlipHorizontally, FarmerRenderer.GetLayerDepth(layerDepth, layer));
        }
        b.Draw(texture2, position + origin + this.positionOffset + new Vector2((float) (-num1 * 4), (float) (num2 * 4 + (!who.IsMale || who.hair.Value < 16 /*0x10*/ ? (who.IsMale || who.hair.Value >= 16 /*0x10*/ ? 0 : 4) : -4))), new Rectangle?(this.hairstyleSourceRect), overrideColor.Equals(Color.White) ? color1 : overrideColor, rotation, origin, scale1, flag2 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, FarmerRenderer.GetLayerDepth(layerDepth, FarmerRenderer.FarmerSpriteLayers.Hair));
        break;
    }
    if (who.hat.Value == null || who.bathingClothes.Value)
      return;
    bool flip = who.FarmerSprite.CurrentAnimationFrame.flip;
    int num3 = who.hat.Value.ignoreHairstyleOffset.Value ? 0 : FarmerRenderer.hairstyleHatOffset[who.hair.Value % 16 /*0x10*/];
    Vector2 position5 = position + origin + this.positionOffset + new Vector2((float) (-(double) scale1 * 2.0 + (double) ((flip ? -1 : 1) * num1) * (double) scale1), (float) (-(double) scale1 * 4.0 + (double) (num2 * 4) + (double) num3 + 4.0) + (float) this.heightOffset.Value);
    Color color2 = who.hat.Value.isPrismatic.Value ? Utility.GetPrismaticColor() : overrideColor;
    if (!flag1 && who.hat.Value.isMask && facingDirection == 0)
    {
      Rectangle hatSourceRect = this.hatSourceRect;
      hatSourceRect.Height -= 11;
      hatSourceRect.Y += 11;
      b.Draw(texture3, position + origin + this.positionOffset + new Vector2(0.0f, 11f * scale1) + new Vector2((float) (-(double) scale1 * 2.0) + (float) ((flip ? -1 : 1) * num1 * 4), (float) (num2 * 4 - 16 /*0x10*/ + num3 + 4 + this.heightOffset.Value)), new Rectangle?(hatSourceRect), overrideColor, rotation, origin, scale1, SpriteEffects.None, FarmerRenderer.GetLayerDepth(layerDepth, FarmerRenderer.FarmerSpriteLayers.Hat));
      hatSourceRect = this.hatSourceRect with
      {
        Height = 11
      };
      b.Draw(texture3, position5, new Rectangle?(hatSourceRect), color2, rotation, origin, scale1, SpriteEffects.None, FarmerRenderer.GetLayerDepth(layerDepth, FarmerRenderer.FarmerSpriteLayers.HatMaskUp));
    }
    else
      b.Draw(texture3, position5, new Rectangle?(this.hatSourceRect), color2, rotation, origin, scale1, SpriteEffects.None, FarmerRenderer.GetLayerDepth(layerDepth, FarmerRenderer.FarmerSpriteLayers.Hat));
  }

  public static float GetLayerDepth(
    float baseLayerDepth,
    FarmerRenderer.FarmerSpriteLayers layer,
    bool dyeLayer = false)
  {
    if (layer == FarmerRenderer.FarmerSpriteLayers.TOOL_IN_USE_SIDE)
      return baseLayerDepth + 2f / 625f;
    int num = Game1.isUsingBackToFrontSorting ? -1 : 1;
    if (dyeLayer)
      baseLayerDepth += 1E-07f * (float) num;
    return baseLayerDepth + (float) layer * 1E-06f * (float) num;
  }

  public void draw(
    SpriteBatch b,
    FarmerSprite.AnimationFrame animationFrame,
    int currentFrame,
    Rectangle sourceRect,
    Vector2 position,
    Vector2 origin,
    float layerDepth,
    int facingDirection,
    Color overrideColor,
    float rotation,
    float scale,
    Farmer who)
  {
    float scale1 = 4f * scale;
    int num1 = FarmerRenderer.featureXOffsetPerFrame[currentFrame];
    int num2 = FarmerRenderer.featureYOffsetPerFrame[currentFrame];
    bool flag = currentFrame == 104 || currentFrame == 105;
    if (this._sickFrame != flag)
    {
      this._sickFrame = flag;
      this._shirtDirty = true;
      this._spriteDirty = true;
    }
    this.executeRecolorActions(who);
    position = new Vector2((float) Math.Floor((double) position.X), (float) Math.Floor((double) position.Y));
    this.rotationAdjustment = Vector2.Zero;
    this.positionOffset.Y = (float) (animationFrame.positionOffset * 4);
    this.positionOffset.X = (float) (animationFrame.xOffset * 4);
    if (!FarmerRenderer.isDrawingForUI && who.swimming.Value)
    {
      sourceRect.Height /= 2;
      sourceRect.Height -= (int) who.yOffset / 4;
      position.Y += 64f;
    }
    if (facingDirection == 3 || facingDirection == 1)
      facingDirection = animationFrame.flip ? 3 : 1;
    b.Draw(this.baseTexture, position + origin + this.positionOffset, new Rectangle?(sourceRect), overrideColor, rotation, origin, scale1, animationFrame.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, FarmerRenderer.GetLayerDepth(layerDepth, FarmerRenderer.FarmerSpriteLayers.Base));
    if (!FarmerRenderer.isDrawingForUI && who.swimming.Value)
    {
      if (who.currentEyes != 0 && who.FacingDirection != 0 && (Game1.timeOfDay < 2600 || who.isInBed.Value && who.timeWentToBed.Value != 0) && (!who.FarmerSprite.PauseForSingleAnimation && !who.UsingTool || who.UsingTool && who.CurrentTool is FishingRod))
      {
        Vector2 position1 = position + origin + this.positionOffset + new Vector2((float) (num1 * 4 + 20 + (who.FacingDirection == 1 ? 12 : (who.FacingDirection == 3 ? 4 : 0))), (float) (num2 * 4 + 40));
        b.Draw(this.baseTexture, position1, new Rectangle?(new Rectangle(5, 16 /*0x10*/, who.FacingDirection == 2 ? 6 : 2, 2)), overrideColor, 0.0f, origin, scale1, SpriteEffects.None, FarmerRenderer.GetLayerDepth(layerDepth, FarmerRenderer.FarmerSpriteLayers.FaceSkin));
        b.Draw(this.baseTexture, position1, new Rectangle?(new Rectangle(264 + (who.FacingDirection == 3 ? 4 : 0), 2 + (who.currentEyes - 1) * 2, who.FacingDirection == 2 ? 6 : 2, 2)), overrideColor, 0.0f, origin, scale1, SpriteEffects.None, FarmerRenderer.GetLayerDepth(layerDepth, FarmerRenderer.FarmerSpriteLayers.Eyes));
      }
      this.drawHairAndAccesories(b, facingDirection, who, position, origin, scale, currentFrame, rotation, overrideColor, layerDepth);
      b.Draw(Game1.staminaRect, new Rectangle((int) position.X + (int) who.yOffset + 8, (int) position.Y - 128 /*0x80*/ + sourceRect.Height * 4 + (int) origin.Y - (int) who.yOffset, sourceRect.Width * 4 - (int) who.yOffset * 2 - 16 /*0x10*/, 4), new Rectangle?(Game1.staminaRect.Bounds), Color.White * 0.75f, 0.0f, Vector2.Zero, SpriteEffects.None, FarmerRenderer.GetLayerDepth(layerDepth, FarmerRenderer.FarmerSpriteLayers.SwimWaterRing));
    }
    else
    {
      Texture2D texture;
      int spriteIndex;
      who.GetDisplayPants(out texture, out spriteIndex);
      Rectangle rectangle = new Rectangle(sourceRect.X, sourceRect.Y, sourceRect.Width, sourceRect.Height);
      rectangle.X += spriteIndex % 10 * 192 /*0xC0*/;
      rectangle.Y += spriteIndex / 10 * 688;
      if (!who.IsMale)
        rectangle.X += 96 /*0x60*/;
      if (this.skin.Value != -12345 || who.pantsItem.Value != null)
        b.Draw(texture, position + origin + this.positionOffset, new Rectangle?(rectangle), overrideColor == Color.White ? Utility.MakeCompletelyOpaque(who.GetPantsColor()) : overrideColor, rotation, origin, scale1, animationFrame.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, FarmerRenderer.GetLayerDepth(layerDepth, who.FarmerSprite.CurrentAnimationFrame.frame == 5 ? FarmerRenderer.FarmerSpriteLayers.PantsPassedOut : FarmerRenderer.FarmerSpriteLayers.Pants));
      sourceRect.Offset(288, 0);
      if (who.currentEyes != 0 && facingDirection != 0 && (Game1.timeOfDay < 2600 || who.isInBed.Value && who.timeWentToBed.Value != 0) && (!who.FarmerSprite.PauseForSingleAnimation && !who.UsingTool || who.UsingTool && who.CurrentTool is FishingRod) && (!who.UsingTool || !(who.CurrentTool is FishingRod currentTool1) || currentTool1.isFishing))
      {
        int num3 = 5;
        int num4 = animationFrame.flip ? num3 - num1 : num3 + num1;
        switch (facingDirection)
        {
          case 1:
            num4 += 3;
            break;
          case 3:
            ++num4;
            break;
        }
        int x = num4 * 4;
        b.Draw(this.baseTexture, position + origin + this.positionOffset + new Vector2((float) x, (float) (num2 * 4 + (!who.IsMale || who.FacingDirection == 2 ? 40 : 36))), new Rectangle?(new Rectangle(5, 16 /*0x10*/, facingDirection == 2 ? 6 : 2, 2)), overrideColor, 0.0f, origin, scale1, SpriteEffects.None, FarmerRenderer.GetLayerDepth(layerDepth, FarmerRenderer.FarmerSpriteLayers.FaceSkin));
        b.Draw(this.baseTexture, position + origin + this.positionOffset + new Vector2((float) x, (float) (num2 * 4 + (who.FacingDirection == 1 || who.FacingDirection == 3 ? 40 : 44))), new Rectangle?(new Rectangle(264 + (facingDirection == 3 ? 4 : 0), 2 + (who.currentEyes - 1) * 2, facingDirection == 2 ? 6 : 2, 2)), overrideColor, 0.0f, origin, scale1, SpriteEffects.None, FarmerRenderer.GetLayerDepth(layerDepth, FarmerRenderer.FarmerSpriteLayers.Eyes));
      }
      this.drawHairAndAccesories(b, facingDirection, who, position, origin, scale, currentFrame, rotation, overrideColor, layerDepth);
      FarmerRenderer.FarmerSpriteLayers layer = FarmerRenderer.FarmerSpriteLayers.Arms;
      if (facingDirection == 0)
        layer = FarmerRenderer.FarmerSpriteLayers.ArmsUp;
      if (animationFrame.armOffset > 0)
      {
        sourceRect.Offset(animationFrame.armOffset * 16 /*0x10*/ - 288, 0);
        b.Draw(this.baseTexture, position + origin + this.positionOffset + who.armOffset, new Rectangle?(sourceRect), overrideColor, rotation, origin, scale1, animationFrame.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, FarmerRenderer.GetLayerDepth(layerDepth, layer));
      }
      if (!who.usingSlingshot || !(who.CurrentTool is Slingshot currentTool2))
        return;
      Point point = Utility.Vector2ToPoint(currentTool2.AdjustForHeight(Utility.PointToVector2(currentTool2.aimPos.Value)));
      int x1 = point.X;
      int y = point.Y;
      int backArmDistance = currentTool2.GetBackArmDistance(who);
      Vector2 shootOrigin = currentTool2.GetShootOrigin(who);
      float rotation1 = (float) Math.Atan2((double) y - (double) shootOrigin.Y, (double) x1 - (double) shootOrigin.X) + 3.14159274f;
      if (!Game1.options.useLegacySlingshotFiring)
      {
        rotation1 -= 3.14159274f;
        if ((double) rotation1 < 0.0)
          rotation1 += 6.28318548f;
      }
      switch (facingDirection)
      {
        case 0:
          b.Draw(this.baseTexture, position + new Vector2((float) (4.0 + (double) rotation1 * 8.0), -44f), new Rectangle?(new Rectangle(173, 238, 9, 14)), Color.White, 0.0f, new Vector2(4f, 11f), scale1, SpriteEffects.None, FarmerRenderer.GetLayerDepth(layerDepth, FarmerRenderer.FarmerSpriteLayers.SlingshotUp));
          break;
        case 1:
          b.Draw(this.baseTexture, position + new Vector2((float) (52 - backArmDistance), -32f), new Rectangle?(new Rectangle(147, 237, 10, 4)), Color.White, 0.0f, new Vector2(8f, 3f), scale1, SpriteEffects.None, FarmerRenderer.GetLayerDepth(layerDepth, FarmerRenderer.FarmerSpriteLayers.Slingshot));
          b.Draw(this.baseTexture, position + new Vector2(36f, -44f), new Rectangle?(new Rectangle(156, 244, 9, 10)), Color.White, rotation1, new Vector2(0.0f, 3f), scale1, SpriteEffects.None, FarmerRenderer.GetLayerDepth(layerDepth, FarmerRenderer.FarmerSpriteLayers.SlingshotUp));
          int num5 = (int) (Math.Cos((double) rotation1 + 1.5707963705062866) * (double) (20 - backArmDistance - 8) - Math.Sin((double) rotation1 + 1.5707963705062866) * -68.0);
          int num6 = (int) (Math.Sin((double) rotation1 + 1.5707963705062866) * (double) (20 - backArmDistance - 8) + Math.Cos((double) rotation1 + 1.5707963705062866) * -68.0);
          Utility.drawLineWithScreenCoordinates((int) ((double) position.X + 52.0 - (double) backArmDistance), (int) ((double) position.Y - 32.0 - 4.0), (int) ((double) position.X + 32.0 + (double) (num5 / 2)), (int) ((double) position.Y - 32.0 - 12.0 + (double) (num6 / 2)), b, Color.White);
          break;
        case 2:
          b.Draw(this.baseTexture, position + new Vector2(4f, (float) (-32 - backArmDistance / 2)), new Rectangle?(new Rectangle(148, 244, 4, 4)), Color.White, 0.0f, Vector2.Zero, scale1, SpriteEffects.None, FarmerRenderer.GetLayerDepth(layerDepth, FarmerRenderer.FarmerSpriteLayers.Arms));
          Utility.drawLineWithScreenCoordinates((int) ((double) position.X + 16.0), (int) ((double) position.Y - 28.0 - (double) (backArmDistance / 2)), (int) ((double) position.X + 44.0 - (double) rotation1 * 10.0), (int) ((double) position.Y - 16.0 - 8.0), b, Color.White);
          Utility.drawLineWithScreenCoordinates((int) ((double) position.X + 16.0), (int) ((double) position.Y - 28.0 - (double) (backArmDistance / 2)), (int) ((double) position.X + 56.0 - (double) rotation1 * 10.0), (int) ((double) position.Y - 16.0 - 8.0), b, Color.White);
          b.Draw(this.baseTexture, position + new Vector2((float) (44.0 - (double) rotation1 * 10.0), -16f), new Rectangle?(new Rectangle(167, 235, 7, 9)), Color.White, 0.0f, new Vector2(3f, 5f), scale1, SpriteEffects.None, FarmerRenderer.GetLayerDepth(layerDepth, FarmerRenderer.FarmerSpriteLayers.Slingshot, true));
          break;
        case 3:
          b.Draw(this.baseTexture, position + new Vector2((float) (40 + backArmDistance), -32f), new Rectangle?(new Rectangle(147, 237, 10, 4)), Color.White, 0.0f, new Vector2(9f, 4f), scale1, SpriteEffects.FlipHorizontally, FarmerRenderer.GetLayerDepth(layerDepth, FarmerRenderer.FarmerSpriteLayers.Slingshot));
          b.Draw(this.baseTexture, position + new Vector2(24f, -40f), new Rectangle?(new Rectangle(156, 244, 9, 10)), Color.White, rotation1 + 3.14159274f, new Vector2(8f, 3f), scale1, SpriteEffects.FlipHorizontally, FarmerRenderer.GetLayerDepth(layerDepth, FarmerRenderer.FarmerSpriteLayers.SlingshotUp));
          int num7 = (int) (Math.Cos((double) rotation1 + 1.2566370964050293) * (double) (20 + backArmDistance - 8) - Math.Sin((double) rotation1 + 1.2566370964050293) * -68.0);
          int num8 = (int) (Math.Sin((double) rotation1 + 1.2566370964050293) * (double) (20 + backArmDistance - 8) + Math.Cos((double) rotation1 + 1.2566370964050293) * -68.0);
          Utility.drawLineWithScreenCoordinates((int) ((double) position.X + 4.0 + (double) backArmDistance), (int) ((double) position.Y - 32.0 - 8.0), (int) ((double) position.X + 26.0 + (double) num7 * 4.0 / 10.0), (int) ((double) position.Y - 32.0 - 8.0 + (double) num8 * 4.0 / 10.0), b, Color.White);
          break;
      }
    }
  }

  public enum FarmerSpriteLayers
  {
    SlingshotUp,
    ToolUp,
    Base,
    Pants,
    FaceSkin,
    Eyes,
    Shirt,
    AccessoryUnderHair,
    ArmsUp,
    HatMaskUp,
    Hair,
    Accessory,
    Hat,
    Tool,
    Arms,
    ToolDown,
    Slingshot,
    PantsPassedOut,
    SwimWaterRing,
    MAX,
    TOOL_IN_USE_SIDE,
  }
}
