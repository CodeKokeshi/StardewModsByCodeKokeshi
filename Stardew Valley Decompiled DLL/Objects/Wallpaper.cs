// Decompiled with JetBrains decompiler
// Type: StardewValley.Objects.Wallpaper
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.GameData;
using StardewValley.Locations;
using System;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley.Objects;

public class Wallpaper : StardewValley.Object
{
  [XmlElement("sourceRect")]
  public readonly NetRectangle sourceRect = new NetRectangle();
  /// <summary>Whether this is a flooring item; else it's a wallpaper item.</summary>
  [XmlElement("isFloor")]
  public readonly NetBool isFloor = new NetBool(false);
  /// <summary>The <c>Data/AdditionalWallpaperFlooring</c> set which contains this flooring or wallpaper, or <c>null</c> for a pre-1.6 vanilla wallpaper.</summary>
  [XmlElement("sourceTexture")]
  public readonly NetString setId = new NetString((string) null);
  /// <summary>The cached data for the flooring or wallpaper set.</summary>
  protected ModWallpaperOrFlooring setData;
  private static readonly Rectangle wallpaperContainerRect = new Rectangle(39, 31 /*0x1F*/, 16 /*0x10*/, 16 /*0x10*/);
  private static readonly Rectangle floorContainerRect = new Rectangle(55, 31 /*0x1F*/, 16 /*0x10*/, 16 /*0x10*/);

  /// <inheritdoc />
  public override string TypeDefinitionId => !this.isFloor.Value ? "(WP)" : "(FL)";

  public Wallpaper()
  {
  }

  public Wallpaper(int which, bool isFloor = false)
    : this()
  {
    this.ItemId = which.ToString();
    this.isFloor.Value = isFloor;
    this.ParentSheetIndex = which;
    this.name = isFloor ? "Flooring" : nameof (Wallpaper);
    this.sourceRect.Value = isFloor ? new Rectangle(which % 8 * 32 /*0x20*/, 336 + which / 8 * 32 /*0x20*/, 28, 26) : new Rectangle(which % 16 /*0x10*/ * 16 /*0x10*/, which / 16 /*0x10*/ * 48 /*0x30*/ + 8, 16 /*0x10*/, 28);
    this.price.Value = 100;
  }

  public Wallpaper(string setId, int which)
    : this()
  {
    this.ItemId = $"{setId}:{which}";
    this.setId.Value = setId;
    this.ParentSheetIndex = which;
    ModWallpaperOrFlooring setData = this.GetSetData();
    if (setData == null)
      this.setId.Value = (string) null;
    this.isFloor.Value = setData != null && setData.IsFlooring;
    this.sourceRect.Value = this.isFloor.Value ? new Rectangle(which % 8 * 32 /*0x20*/, 336 + which / 8 * 32 /*0x20*/, 28, 26) : new Rectangle(which % 16 /*0x10*/ * 16 /*0x10*/, which / 16 /*0x10*/ * 48 /*0x30*/ + 8, 16 /*0x10*/, 28);
    if (setData != null && this.isFloor.Value)
      this.sourceRect.Y = which / 8 * 32 /*0x20*/;
    this.name = this.isFloor.Value ? "Flooring" : nameof (Wallpaper);
    this.price.Value = 100;
  }

  /// <inheritdoc />
  protected override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.sourceRect, "sourceRect").AddField((INetSerializable) this.isFloor, "isFloor").AddField((INetSerializable) this.setId, "setId");
  }

  /// <summary>Get the data for the flooring or wallpaper set which contains this item, if any.</summary>
  public virtual ModWallpaperOrFlooring GetSetData()
  {
    if (this.setId.Value == null)
      return (ModWallpaperOrFlooring) null;
    if (this.setData != null)
      return this.setData;
    foreach (ModWallpaperOrFlooring setData in DataLoader.AdditionalWallpaperFlooring(Game1.content))
    {
      if (setData.Id == this.setId.Value)
      {
        this.setData = setData;
        return setData;
      }
    }
    return (ModWallpaperOrFlooring) null;
  }

  /// <inheritdoc />
  protected override string loadDisplayName()
  {
    return !this.isFloor.Value ? Game1.content.LoadString("Strings\\StringsFromCSFiles:Wallpaper.cs.13204") : Game1.content.LoadString("Strings\\StringsFromCSFiles:Wallpaper.cs.13203");
  }

  public override string getDescription()
  {
    return !this.isFloor.Value ? Game1.content.LoadString("Strings\\StringsFromCSFiles:Wallpaper.cs.13206") : Game1.content.LoadString("Strings\\StringsFromCSFiles:Wallpaper.cs.13205");
  }

  /// <inheritdoc />
  public override bool performDropDownAction(Farmer who) => true;

  /// <inheritdoc />
  public override bool performObjectDropInAction(
    Item dropInItem,
    bool probe,
    Farmer who,
    bool returnFalseIfItemConsumed = false)
  {
    return false;
  }

  public override bool canBePlacedHere(
    GameLocation l,
    Vector2 tile,
    CollisionMask collisionMask = CollisionMask.All,
    bool showError = false)
  {
    Vector2 vector2 = tile * 64f;
    vector2.X += 32f;
    vector2.Y += 32f;
    foreach (Furniture furniture in l.furniture)
    {
      if (furniture.furniture_type.Value != 12 && furniture.GetBoundingBox().Contains((int) vector2.X, (int) vector2.Y))
        return false;
    }
    return true;
  }

  public override bool placementAction(GameLocation location, int x, int y, Farmer who = null)
  {
    if (who == null)
      who = Game1.player;
    if (location is DecoratableLocation decoratableLocation)
    {
      Point point = new Point(x / 64 /*0x40*/, y / 64 /*0x40*/);
      if (this.isFloor.Value)
      {
        string floorId = decoratableLocation.GetFloorID(point.X, point.Y);
        if (floorId != null)
        {
          if (this.GetSetData() != null)
            decoratableLocation.SetFloor($"{this.GetSetData().Id}:{this.parentSheetIndex.Value.ToString()}", floorId);
          else
            decoratableLocation.SetFloor(this.parentSheetIndex.Value.ToString(), floorId);
          location.playSound("coin");
          return true;
        }
      }
      else
      {
        string wallpaperId = decoratableLocation.GetWallpaperID(point.X, point.Y);
        if (wallpaperId != null)
        {
          if (this.GetSetData() != null)
            decoratableLocation.SetWallpaper($"{this.GetSetData().Id}:{this.parentSheetIndex.Value.ToString()}", wallpaperId);
          else
            decoratableLocation.SetWallpaper(this.parentSheetIndex.Value.ToString(), wallpaperId);
          location.playSound("coin");
          return true;
        }
      }
    }
    return false;
  }

  public override bool isPlaceable() => true;

  /// <inheritdoc />
  public override int salePrice(bool ignoreProfitMargins = false) => this.price.Value;

  public override int maximumStackSize() => 1;

  /// <inheritdoc />
  [XmlIgnore]
  public override string Name => this.name;

  public override void drawWhenHeld(SpriteBatch spriteBatch, Vector2 objectPosition, Farmer f)
  {
    this.drawInMenu(spriteBatch, objectPosition, 1f);
  }

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
    this.AdjustMenuDrawForRecipes(ref transparency, ref scaleSize);
    Texture2D texture;
    if (this.GetSetData() != null)
    {
      try
      {
        texture = Game1.content.Load<Texture2D>(this.GetSetData().Texture);
      }
      catch (Exception ex)
      {
        texture = Game1.content.Load<Texture2D>("Maps\\walls_and_floors");
      }
    }
    else
      texture = Game1.content.Load<Texture2D>("Maps\\walls_and_floors");
    if (this.isFloor.Value)
    {
      spriteBatch.Draw(Game1.mouseCursors2, location + new Vector2(32f, 32f), new Rectangle?(Wallpaper.floorContainerRect), color * transparency, 0.0f, new Vector2(8f, 8f), 4f * scaleSize, SpriteEffects.None, layerDepth);
      spriteBatch.Draw(texture, location + new Vector2(32f, 30f), new Rectangle?(this.sourceRect.Value), color * transparency, 0.0f, new Vector2(14f, 13f), 2f * scaleSize, SpriteEffects.None, layerDepth + 1f / 1000f);
    }
    else
    {
      spriteBatch.Draw(Game1.mouseCursors2, location + new Vector2(32f, 32f), new Rectangle?(Wallpaper.wallpaperContainerRect), color * transparency, 0.0f, new Vector2(8f, 8f), 4f * scaleSize, SpriteEffects.None, layerDepth);
      spriteBatch.Draw(texture, location + new Vector2(32f, 32f), new Rectangle?(this.sourceRect.Value), color * transparency, 0.0f, new Vector2(8f, 14f), 2f * scaleSize, SpriteEffects.None, layerDepth + 1f / 1000f);
    }
    this.DrawMenuIcons(spriteBatch, location, scaleSize, transparency, layerDepth, drawStackNumber, color);
  }

  /// <inheritdoc />
  protected override Item GetOneNew()
  {
    ModWallpaperOrFlooring setData = this.GetSetData();
    return setData == null ? (Item) new Wallpaper(this.parentSheetIndex.Value, this.isFloor.Value) : (Item) new Wallpaper(setData.Id, this.parentSheetIndex.Value);
  }
}
