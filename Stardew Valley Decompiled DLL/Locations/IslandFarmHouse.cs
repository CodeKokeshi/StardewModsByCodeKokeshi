// Decompiled with JetBrains decompiler
// Type: StardewValley.Locations.IslandFarmHouse
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Extensions;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using xTile.Dimensions;
using xTile.Layers;

#nullable disable
namespace StardewValley.Locations;

public class IslandFarmHouse : DecoratableLocation
{
  [XmlElement("fridge")]
  public readonly NetRef<Chest> fridge;
  public Point fridgePosition;
  public NetBool visited;
  private Color nightLightingColor;
  private Color rainLightingColor;

  public IslandFarmHouse()
  {
    NetBool netBool = new NetBool(false);
    netBool.InterpolationEnabled = false;
    this.visited = netBool;
    this.nightLightingColor = new Color(180, 180, 0);
    this.rainLightingColor = new Color(90, 90, 0);
    // ISSUE: explicit constructor call
    base.\u002Ector();
    this.fridge.Value.Location = (GameLocation) this;
  }

  public IslandFarmHouse(string map, string name)
  {
    NetBool netBool = new NetBool(false);
    netBool.InterpolationEnabled = false;
    this.visited = netBool;
    this.nightLightingColor = new Color(180, 180, 0);
    this.rainLightingColor = new Color(90, 90, 0);
    // ISSUE: explicit constructor call
    base.\u002Ector(map, name);
    this.fridge.Value.Location = (GameLocation) this;
    this.furniture.Add(ItemRegistry.Create<Furniture>("(F)1798").SetPlacement(12, 8));
    this.furniture.Add(ItemRegistry.Create<Furniture>("(F)1614").SetPlacement(3, 1));
    this.furniture.Add(ItemRegistry.Create<Furniture>("(F)1614").SetPlacement(8, 1));
    this.furniture.Add(ItemRegistry.Create<Furniture>("(F)1614").SetPlacement(20, 1));
    this.furniture.Add(ItemRegistry.Create<Furniture>("(F)1614").SetPlacement(25, 1));
    this.furniture.Add(ItemRegistry.Create<Furniture>("(F)1294").SetPlacement(1, 4));
    this.furniture.Add(ItemRegistry.Create<Furniture>("(F)1294").SetPlacement(10, 4));
    this.furniture.Add(ItemRegistry.Create<Furniture>("(F)1294").SetPlacement(18, 4));
    this.furniture.Add(ItemRegistry.Create<Furniture>("(F)1294").SetPlacement(28, 4));
    this.furniture.Add(ItemRegistry.Create<Furniture>("(F)1742").SetPlacement(20, 4));
    this.furniture.Add(ItemRegistry.Create<Furniture>("(F)1755").SetPlacement(14, 9));
    this.ReadWallpaperAndFloorTileData();
    this.SetWallpaper("88", "UpperLeft");
    this.SetFloor("23", "UpperLeft");
    this.SetWallpaper("88", "UpperRight");
    this.SetFloor("48", "Kitchen");
    this.SetWallpaper("87", "Kitchen");
    this.SetFloor("52", "UpperRight");
    this.SetWallpaper("87", "BottomRight_Left");
    this.SetFloor("23", "BottomRight");
    this.SetWallpaper("87", "BottomRight_Right");
    this.fridgePosition = new Point();
  }

  public override void TransferDataFromSavedLocation(GameLocation l)
  {
    IslandFarmHouse islandFarmHouse = (IslandFarmHouse) l;
    this.fridge.Value = islandFarmHouse.fridge.Value;
    this.visited.Value = islandFarmHouse.visited.Value;
    base.TransferDataFromSavedLocation(l);
  }

  public override void UpdateWhenCurrentLocation(GameTime time)
  {
    base.UpdateWhenCurrentLocation(time);
    this.fridge.Value.updateWhenCurrentLocation(time);
  }

  public override List<Microsoft.Xna.Framework.Rectangle> getWalls()
  {
    return new List<Microsoft.Xna.Framework.Rectangle>()
    {
      new Microsoft.Xna.Framework.Rectangle(1, 1, 10, 3),
      new Microsoft.Xna.Framework.Rectangle(18, 1, 11, 3),
      new Microsoft.Xna.Framework.Rectangle(12, 5, 5, 2),
      new Microsoft.Xna.Framework.Rectangle(17, 9, 2, 2),
      new Microsoft.Xna.Framework.Rectangle(21, 9, 8, 2)
    };
  }

  protected override void resetLocalState()
  {
    base.resetLocalState();
    if (!this.visited.Value)
      this.visited.Value = true;
    this.fridgePosition = this.GetFridgePositionFromMap() ?? Point.Zero;
  }

  /// <summary>Get the fridge position by scanning the map tiles for the sprite index.</summary>
  /// <remarks>This is relatively expensive. Most code should use the cached <see cref="F:StardewValley.Locations.IslandFarmHouse.fridgePosition" /> instead.</remarks>
  public Point? GetFridgePositionFromMap()
  {
    Layer layer = this.map.RequireLayer("Buildings");
    for (int y = 0; y < layer.LayerHeight; ++y)
    {
      for (int x = 0; x < layer.LayerWidth; ++x)
      {
        if (layer.GetTileIndexAt(x, y, "untitled tile sheet") == 258)
          return new Point?(new Point(x, y));
      }
    }
    return new Point?();
  }

  public override List<Microsoft.Xna.Framework.Rectangle> getFloors()
  {
    return new List<Microsoft.Xna.Framework.Rectangle>()
    {
      new Microsoft.Xna.Framework.Rectangle(1, 3, 11, 12),
      new Microsoft.Xna.Framework.Rectangle(11, 7, 6, 9),
      new Microsoft.Xna.Framework.Rectangle(18, 3, 11, 6),
      new Microsoft.Xna.Framework.Rectangle(17, 11, 12, 6)
    };
  }

  protected override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.fridge, "fridge").AddField((INetSerializable) this.visited, "visited");
    this.visited.fieldChangeVisibleEvent += (FieldChange<NetBool, bool>) ((a, b, c) => this.InitializeBeds());
    this.fridge.fieldChangeEvent += (FieldChange<NetRef<Chest>, Chest>) ((field, oldValue, newValue) => newValue.Location = (GameLocation) this);
  }

  public virtual void InitializeBeds()
  {
    if (!Game1.IsMasterGame || Game1.gameMode == (byte) 6 || !this.visited.Value)
      return;
    int num1 = 0;
    foreach (Farmer allFarmer in Game1.getAllFarmers())
      ++num1;
    string itemId = "2176";
    this.furniture.Add((Furniture) new BedFurniture(itemId, new Vector2(22f, 3f)));
    int val2 = num1 - 1;
    if (val2 > 0)
    {
      this.furniture.Add((Furniture) new BedFurniture(itemId, new Vector2(26f, 3f)));
      --val2;
    }
    for (int index = 0; index < Math.Min(6, val2); ++index)
    {
      int x = 3;
      int num2 = 3;
      if (index % 2 == 0)
        x += 4;
      int y = num2 + index / 2 * 4;
      this.furniture.Add((Furniture) new BedFurniture(itemId, new Vector2((float) x, (float) y)));
    }
  }

  protected override void _updateAmbientLighting()
  {
    if (Game1.isStartingToGetDarkOut((GameLocation) this) || (double) this.lightLevel.Value > 0.0)
    {
      float t = 1f - Utility.Clamp((float) Utility.CalculateMinutesBetweenTimes(Game1.timeOfDay + Game1.gameTimeInterval / (Game1.realMilliSecondsPerGameMinute + this.ExtraMillisecondsPerInGameMinute), Game1.getTrulyDarkTime((GameLocation) this)) / 120f, 0.0f, 1f);
      Game1.ambientLight = new Color((int) (byte) Utility.Lerp(Game1.isRaining ? (float) this.rainLightingColor.R : 0.0f, (float) this.nightLightingColor.R, t), (int) (byte) Utility.Lerp(Game1.isRaining ? (float) this.rainLightingColor.G : 0.0f, (float) this.nightLightingColor.G, t), (int) (byte) Utility.Lerp(0.0f, (float) this.nightLightingColor.B, t));
    }
    else
      Game1.ambientLight = Game1.isRaining ? this.rainLightingColor : Color.White;
  }

  public override void drawAboveFrontLayer(SpriteBatch b)
  {
    base.drawAboveFrontLayer(b);
    if (!this.fridge.Value.mutex.IsLocked())
      return;
    b.Draw(Game1.mouseCursors2, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) this.fridgePosition.X, (float) (this.fridgePosition.Y - 1)) * 64f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, 192 /*0xC0*/, 16 /*0x10*/, 32 /*0x20*/)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) ((this.fridgePosition.Y + 1) * 64 /*0x40*/ + 1) / 10000f);
  }

  public override bool checkAction(Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who)
  {
    if (this.getTileIndexAt(tileLocation, "Buildings", "untitled tile sheet") != 258)
      return base.checkAction(tileLocation, viewport, who);
    this.fridge.Value.fridge.Value = true;
    this.fridge.Value.checkForAction(who, false);
    return true;
  }
}
