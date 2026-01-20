// Decompiled with JetBrains decompiler
// Type: StardewValley.Buildings.GreenhouseBuilding
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Extensions;
using System;
using xTile;
using xTile.Dimensions;
using xTile.Layers;
using xTile.Tiles;

#nullable disable
namespace StardewValley.Buildings;

public class GreenhouseBuilding(Vector2 tileLocation) : Building("Greenhouse", tileLocation)
{
  protected Farm _farm;

  public GreenhouseBuilding()
    : this(Vector2.Zero)
  {
  }

  public override void drawBackground(SpriteBatch b)
  {
    base.drawBackground(b);
    if (this.isMoving)
      return;
    this.DrawEntranceTiles(b);
    this.drawShadow(b, -1, -1);
  }

  public Farm GetFarm()
  {
    if (this._farm == null)
      this._farm = Game1.getFarm();
    return this._farm;
  }

  public override bool OnUseHumanDoor(Farmer who)
  {
    if (Game1.MasterPlayer.mailReceived.Contains("ccPantry"))
      return true;
    Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Farm_GreenhouseRuins"));
    return false;
  }

  public override string isThereAnythingtoPreventConstruction(
    GameLocation location,
    Vector2 tile_position)
  {
    return (string) null;
  }

  public override bool doesTileHaveProperty(
    int tile_x,
    int tile_y,
    string property_name,
    string layer_name,
    ref string property_value)
  {
    if (this.isMoving)
      return false;
    if (layer_name == "Back" && (tile_x >= this.tileX.Value - 1 && tile_x <= this.tileX.Value + this.tilesWide.Value - 1 && tile_y <= this.tileY.Value + this.tilesHigh.Value && tile_y >= this.tileY.Value || this.CanDrawEntranceTiles() && tile_x >= this.tileX.Value + 1 && tile_x <= this.tileX.Value + this.tilesWide.Value - 2 && tile_y == this.tileY.Value + this.tilesHigh.Value + 1))
    {
      if (this.CanDrawEntranceTiles() && tile_x >= this.tileX.Value + this.humanDoor.X - 1 && tile_x <= this.tileX.Value + this.humanDoor.X + 1 && tile_y <= this.tileY.Value + this.tilesHigh.Value + 1 && tile_y >= this.tileY.Value + this.humanDoor.Y + 1)
      {
        if (!(property_name == "Type"))
        {
          if (!(property_name == "NoSpawn"))
          {
            if (property_name == "Buildable")
            {
              property_value = (string) null;
              return true;
            }
          }
          else
          {
            property_value = "All";
            return true;
          }
        }
        else
        {
          property_value = "Stone";
          return true;
        }
      }
      if (!(property_name == "Buildable"))
      {
        if (!(property_name == "NoSpawn"))
        {
          if (property_name == "Diggable")
          {
            property_value = (string) null;
            return true;
          }
        }
        else
        {
          property_value = "Tree";
          return true;
        }
      }
      else
      {
        property_value = "T";
        return true;
      }
    }
    return base.doesTileHaveProperty(tile_x, tile_y, property_name, layer_name, ref property_value);
  }

  public virtual bool CanDrawEntranceTiles() => true;

  public virtual void DrawEntranceTiles(SpriteBatch b)
  {
    Map map = this.GetFarm().Map;
    Layer layer = map.RequireLayer("Back");
    TileSheet tileSheet = map.GetTileSheet("untitled tile sheet") ?? map.TileSheets[Math.Min(1, map.TileSheets.Count - 1)];
    if (tileSheet == null)
      return;
    StaticTile staticTile1 = new StaticTile(layer, tileSheet, BlendMode.Alpha, 812);
    if (!this.CanDrawEntranceTiles())
      return;
    float layerDepth = 0.0f;
    Vector2 local1 = Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (this.tileX.Value + this.humanDoor.Value.X - 1), (float) (this.tileY.Value + this.humanDoor.Value.Y + 1)) * 64f);
    Location location = new Location((int) local1.X, (int) local1.Y);
    Game1.mapDisplayDevice.DrawTile((Tile) staticTile1, location, layerDepth);
    location.X += 64 /*0x40*/;
    Game1.mapDisplayDevice.DrawTile((Tile) staticTile1, location, layerDepth);
    location.X += 64 /*0x40*/;
    Game1.mapDisplayDevice.DrawTile((Tile) staticTile1, location, layerDepth);
    StaticTile staticTile2 = new StaticTile(layer, tileSheet, BlendMode.Alpha, 838);
    Vector2 local2 = Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (this.tileX.Value + this.humanDoor.Value.X - 1), (float) (this.tileY.Value + this.humanDoor.Value.Y + 2)) * 64f);
    location.X = (int) local2.X;
    location.Y = (int) local2.Y;
    Game1.mapDisplayDevice.DrawTile((Tile) staticTile2, location, layerDepth);
    location.X += 64 /*0x40*/;
    Game1.mapDisplayDevice.DrawTile((Tile) staticTile2, location, layerDepth);
    location.X += 64 /*0x40*/;
    Game1.mapDisplayDevice.DrawTile((Tile) staticTile2, location, layerDepth);
  }

  public override void drawShadow(SpriteBatch b, int localX = -1, int localY = -1)
  {
    Microsoft.Xna.Framework.Rectangle rectangle = new Microsoft.Xna.Framework.Rectangle(112 /*0x70*/, 0, 128 /*0x80*/, 144 /*0x90*/);
    if (this.CanDrawEntranceTiles())
      rectangle.Y = 144 /*0x90*/;
    b.Draw(this.texture.Value, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) ((this.tileX.Value - 1) * 64 /*0x40*/), (float) (this.tileY.Value * 64 /*0x40*/))), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.White * (localX == -1 ? this.alpha : 1f), 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.0f);
  }
}
