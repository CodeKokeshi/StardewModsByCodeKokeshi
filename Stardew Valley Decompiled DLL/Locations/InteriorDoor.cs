// Decompiled with JetBrains decompiler
// Type: StardewValley.InteriorDoor
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Extensions;
using System;
using System.IO;
using xTile;
using xTile.Layers;
using xTile.ObjectModel;
using xTile.Tiles;

#nullable disable
namespace StardewValley;

public class InteriorDoor : NetField<bool, InteriorDoor>
{
  public GameLocation Location;
  public Point Position;
  public TemporaryAnimatedSprite Sprite;
  public Tile Tile;

  public InteriorDoor()
  {
  }

  public InteriorDoor(GameLocation location, Point position)
    : this()
  {
    this.Location = location;
    this.Position = position;
  }

  public override void Set(bool newValue)
  {
    if (newValue == this.value)
      return;
    this.cleanSet(newValue);
    this.MarkDirty();
  }

  protected override void ReadDelta(BinaryReader reader, NetVersion version)
  {
    bool newValue = reader.ReadBoolean();
    if (!version.IsPriorityOver(this.ChangeVersion))
      return;
    this.setInterpolationTarget(newValue);
  }

  protected override void WriteDelta(BinaryWriter writer) => writer.Write(this.targetValue);

  public void ResetLocalState()
  {
    int x = this.Position.X;
    int y = this.Position.Y;
    xTile.Dimensions.Location location = new xTile.Dimensions.Location(x, y);
    Layer layer1 = this.Location.Map.RequireLayer("Buildings");
    Layer layer2 = this.Location.Map.RequireLayer("Back");
    if (this.Tile == null)
      this.Tile = layer1.Tiles[location];
    if (this.Tile == null)
      return;
    string str;
    if (this.Tile.Properties.TryGetValue("Action", out str) && str.Contains("Door"))
    {
      string[] strArray = ArgUtility.SplitBySpace(str, 2);
      if (strArray.Length > 1)
      {
        Tile tile = layer2.Tiles[location];
        if (tile != null && !tile.Properties.ContainsKey("TouchAction"))
          tile.Properties.Add("TouchAction", (PropertyValue) ("Door " + strArray[1]));
      }
    }
    Microsoft.Xna.Framework.Rectangle sourceRect = new Microsoft.Xna.Framework.Rectangle();
    bool flipped = false;
    switch (this.Tile.TileIndex)
    {
      case 120:
        sourceRect = new Microsoft.Xna.Framework.Rectangle(512 /*0x0200*/, 144 /*0x90*/, 16 /*0x10*/, 48 /*0x30*/);
        break;
      case 824:
        sourceRect = new Microsoft.Xna.Framework.Rectangle(640, 144 /*0x90*/, 16 /*0x10*/, 48 /*0x30*/);
        break;
      case 825:
        sourceRect = new Microsoft.Xna.Framework.Rectangle(640, 144 /*0x90*/, 16 /*0x10*/, 48 /*0x30*/);
        flipped = true;
        break;
      case 838:
        sourceRect = new Microsoft.Xna.Framework.Rectangle(576, 144 /*0x90*/, 16 /*0x10*/, 48 /*0x30*/);
        if (x == 10 && y == 5)
        {
          flipped = true;
          break;
        }
        break;
    }
    this.Sprite = new TemporaryAnimatedSprite("LooseSprites\\Cursors", sourceRect, 100f, 4, 1, new Vector2((float) x, (float) (y - 2)) * 64f, false, flipped, (float) ((y + 1) * 64 /*0x40*/ - 12) / 10000f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
    {
      holdLastFrame = true,
      paused = true
    };
    if (!this.Value)
      return;
    this.Sprite.paused = false;
    this.Sprite.resetEnd();
  }

  public virtual void ApplyMapModifications()
  {
    if (this.Value)
      this.openDoorTiles();
    else
      this.closeDoorTiles();
  }

  public void CleanUpLocalState() => this.closeDoorTiles();

  private void closeDoorSprite()
  {
    this.Sprite.reset();
    this.Sprite.paused = true;
  }

  private void openDoorSprite() => this.Sprite.paused = false;

  private void openDoorTiles()
  {
    this.Location.setTileProperty(this.Position.X, this.Position.Y, "Back", "TemporaryBarrier", "T");
    this.Location.removeTile(this.Position.X, this.Position.Y, "Buildings");
    DelayedAction.functionAfterDelay((Action) (() => this.Location.removeTileProperty(this.Position.X, this.Position.Y, "Back", "TemporaryBarrier")), 400);
    this.Location.removeTile(this.Position.X, this.Position.Y - 1, "Front");
    this.Location.removeTile(this.Position.X, this.Position.Y - 2, "Front");
  }

  private void closeDoorTiles()
  {
    xTile.Dimensions.Location location = new xTile.Dimensions.Location(this.Position.X, this.Position.Y);
    Map map = this.Location.Map;
    if (map == null || this.Tile == null)
      return;
    map.RequireLayer("Buildings").Tiles[location] = this.Tile;
    this.Location.removeTileProperty(this.Position.X, this.Position.Y, "Back", "TemporaryBarrier");
    --location.Y;
    map.RequireLayer("Front").Tiles[location] = (Tile) new StaticTile(map.RequireLayer("Front"), this.Tile.TileSheet, BlendMode.Alpha, this.Tile.TileIndex - this.Tile.TileSheet.SheetWidth);
    --location.Y;
    map.RequireLayer("Front").Tiles[location] = (Tile) new StaticTile(map.RequireLayer("Front"), this.Tile.TileSheet, BlendMode.Alpha, this.Tile.TileIndex - this.Tile.TileSheet.SheetWidth * 2);
  }

  public void Update(GameTime time)
  {
    if (this.Sprite == null)
      return;
    if (this.Value && this.Sprite.paused)
    {
      this.openDoorSprite();
      this.openDoorTiles();
    }
    else if (!this.Value && !this.Sprite.paused)
    {
      this.closeDoorSprite();
      this.closeDoorTiles();
    }
    this.Sprite.update(time);
  }

  public void Draw(SpriteBatch b) => this.Sprite?.draw(b);
}
