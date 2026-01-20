// Decompiled with JetBrains decompiler
// Type: StardewValley.TerrainFeatures.Flooring
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.GameData.FloorsAndPaths;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Locations;
using StardewValley.Network;
using StardewValley.Tools;
using System.Collections.Generic;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley.TerrainFeatures;

public class Flooring : TerrainFeature
{
  public const byte N = 1;
  public const byte E = 2;
  public const byte S = 4;
  public const byte W = 8;
  public const byte NE = 16 /*0x10*/;
  public const byte NW = 32 /*0x20*/;
  public const byte SE = 64 /*0x40*/;
  public const byte SW = 128 /*0x80*/;
  public const byte Cardinals = 15;
  public static readonly Vector2 N_Offset = new Vector2(0.0f, -1f);
  public static readonly Vector2 E_Offset = new Vector2(1f, 0.0f);
  public static readonly Vector2 S_Offset = new Vector2(0.0f, 1f);
  public static readonly Vector2 W_Offset = new Vector2(-1f, 0.0f);
  public static readonly Vector2 NE_Offset = new Vector2(1f, -1f);
  public static readonly Vector2 NW_Offset = new Vector2(-1f, -1f);
  public static readonly Vector2 SE_Offset = new Vector2(1f, 1f);
  public static readonly Vector2 SW_Offset = new Vector2(-1f, 1f);
  public const string wood = "0";
  public const string stone = "1";
  public const string ghost = "2";
  public const string iceTile = "3";
  public const string straw = "4";
  public const string gravel = "5";
  public const string boardwalk = "6";
  public const string colored_cobblestone = "7";
  public const string cobblestone = "8";
  public const string steppingStone = "9";
  public const string brick = "10";
  public const string plankFlooring = "11";
  public const string townFlooring = "12";
  [XmlIgnore]
  public Texture2D floorTexture;
  [XmlIgnore]
  public Texture2D floorTextureWinter;
  [InstancedStatic]
  public static Dictionary<byte, int> drawGuide;
  [InstancedStatic]
  public static List<int> drawGuideList;
  [XmlElement("whichFloor")]
  public readonly NetString whichFloor = new NetString();
  [XmlElement("whichView")]
  public readonly NetInt whichView = new NetInt();
  private byte neighborMask;
  protected static Dictionary<string, string> _FloorPathItemLookup;
  private static readonly Flooring.NeighborLoc[] _offsets = new Flooring.NeighborLoc[8]
  {
    new Flooring.NeighborLoc(Flooring.N_Offset, (byte) 1, (byte) 4),
    new Flooring.NeighborLoc(Flooring.S_Offset, (byte) 4, (byte) 1),
    new Flooring.NeighborLoc(Flooring.E_Offset, (byte) 2, (byte) 8),
    new Flooring.NeighborLoc(Flooring.W_Offset, (byte) 8, (byte) 2),
    new Flooring.NeighborLoc(Flooring.NE_Offset, (byte) 16 /*0x10*/, (byte) 128 /*0x80*/),
    new Flooring.NeighborLoc(Flooring.NW_Offset, (byte) 32 /*0x20*/, (byte) 64 /*0x40*/),
    new Flooring.NeighborLoc(Flooring.SE_Offset, (byte) 64 /*0x40*/, (byte) 32 /*0x20*/),
    new Flooring.NeighborLoc(Flooring.SW_Offset, (byte) 128 /*0x80*/, (byte) 16 /*0x10*/)
  };
  private List<Flooring.Neighbor> _neighbors = new List<Flooring.Neighbor>();

  public Flooring()
    : base(false)
  {
    this.loadSprite();
    if (Flooring.drawGuide != null)
      return;
    Flooring.populateDrawGuide();
  }

  public Flooring(string which)
    : this()
  {
    this.whichFloor.Value = which;
    this.ApplyFlooringFlags();
  }

  public override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.whichFloor, "whichFloor").AddField((INetSerializable) this.whichView, "whichView");
  }

  public virtual void ApplyFlooringFlags()
  {
    FloorPathData data = this.GetData();
    if ((data != null ? (data.ConnectType == FloorPathConnectType.Random ? 1 : 0) : 0) == 0)
      return;
    this.whichView.Value = Game1.random.Next(16 /*0x10*/);
  }

  public static Dictionary<string, string> GetFloorPathItemLookup()
  {
    if (Flooring._FloorPathItemLookup == null)
      Flooring.LoadFloorPathItemLookup();
    return Flooring._FloorPathItemLookup;
  }

  /// <summary>Get the flooring or path's data from <see cref="F:StardewValley.Game1.floorPathData" />, if found.</summary>
  public FloorPathData GetData()
  {
    FloorPathData data;
    return !Flooring.TryGetData(this.whichFloor.Value, out data) ? (FloorPathData) null : data;
  }

  /// <summary>Try to get a flooring or path's data from <see cref="F:StardewValley.Game1.floorPathData" />.</summary>
  /// <param name="id">The flooring or path type ID (i.e. the key in <see cref="F:StardewValley.Game1.floorPathData" />).</param>
  /// <param name="data">The flooring or path data, if found.</param>
  /// <returns>Returns whether the flooring or path data was found.</returns>
  public static bool TryGetData(string id, out FloorPathData data)
  {
    if (id != null)
      return Game1.floorPathData.TryGetValue(id, out data);
    data = (FloorPathData) null;
    return false;
  }

  protected static void LoadFloorPathItemLookup()
  {
    Flooring._FloorPathItemLookup = new Dictionary<string, string>();
    foreach (KeyValuePair<string, FloorPathData> keyValuePair in (IEnumerable<KeyValuePair<string, FloorPathData>>) Game1.floorPathData)
    {
      string key = keyValuePair.Key;
      string itemId = keyValuePair.Value.ItemId;
      if (!string.IsNullOrEmpty(itemId))
        Flooring._FloorPathItemLookup[itemId] = key;
    }
  }

  public override Rectangle getBoundingBox()
  {
    Vector2 tile = this.Tile;
    return new Rectangle((int) ((double) tile.X * 64.0), (int) ((double) tile.Y * 64.0), 64 /*0x40*/, 64 /*0x40*/);
  }

  public static void populateDrawGuide()
  {
    Flooring.drawGuide = new Dictionary<byte, int>()
    {
      [(byte) 0] = 0,
      [(byte) 6] = 1,
      [(byte) 14] = 2,
      [(byte) 12] = 3,
      [(byte) 4] = 16 /*0x10*/,
      [(byte) 7] = 17,
      [(byte) 15] = 18,
      [(byte) 13] = 19,
      [(byte) 5] = 32 /*0x20*/,
      [(byte) 3] = 33,
      [(byte) 11] = 34,
      [(byte) 9] = 35,
      [(byte) 1] = 48 /*0x30*/,
      [(byte) 2] = 49,
      [(byte) 10] = 50,
      [(byte) 8] = 51
    };
    Flooring.drawGuideList = new List<int>(Flooring.drawGuide.Count);
    foreach (KeyValuePair<byte, int> keyValuePair in Flooring.drawGuide)
      Flooring.drawGuideList.Add(keyValuePair.Value);
  }

  public override void loadSprite()
  {
  }

  public override void doCollisionAction(
    Rectangle positionOfCollider,
    int speedOfCollision,
    Vector2 tileLocation,
    Character who)
  {
    base.doCollisionAction(positionOfCollider, speedOfCollision, tileLocation, who);
    FloorPathData data = this.GetData();
    GameLocation location = this.Location;
    if (!(who is Farmer farmer))
      return;
    switch (location)
    {
      case Farm _:
      case IslandWest _:
        float num = 0.1f;
        if (data != null && (double) data.FarmSpeedBuff >= 0.0)
          num = data.FarmSpeedBuff;
        farmer.temporarySpeedBuff = num;
        break;
    }
  }

  /// <inheritdoc />
  public override bool isPassable(Character c = null) => true;

  public string getFootstepSound() => this.GetData()?.FootstepSound ?? "stoneStep";

  public Point GetTextureCorner(bool useSeasonalVariants = true)
  {
    return !useSeasonalVariants || !this.ShouldDrawWinterVersion() ? this.GetData().Corner : this.GetData().WinterCorner;
  }

  public Texture2D GetTexture(bool useSeasonalVariants = true)
  {
    if (useSeasonalVariants && this.ShouldDrawWinterVersion())
    {
      if (this.floorTextureWinter == null)
        this.floorTextureWinter = Game1.content.Load<Texture2D>(this.GetData().WinterTexture);
      return this.floorTextureWinter;
    }
    if (this.floorTexture == null)
      this.floorTexture = Game1.content.Load<Texture2D>(this.GetData().Texture);
    return this.floorTexture;
  }

  public bool ShouldDrawWinterVersion()
  {
    return this.Location != null && !this.Location.isGreenhouse.Value && this.GetData().WinterTexture != null && this.Location.IsWinterHere();
  }

  public override bool performToolAction(Tool t, int damage, Vector2 tileLocation)
  {
    GameLocation location = this.Location ?? Game1.currentLocation;
    if (t != null || damage > 0)
    {
      if (damage <= 0)
      {
        switch (t)
        {
          case Pickaxe _:
          case Axe _:
            break;
          default:
            goto label_8;
        }
      }
      FloorPathData data = this.GetData();
      if (data != null)
      {
        location.playSound(data.RemovalSound ?? data.PlacementSound, new Vector2?(tileLocation));
        Game1.createRadialDebris(location, data.RemovalDebrisType, (int) tileLocation.X, (int) tileLocation.Y, 4, false);
        if (data.ItemId != null)
        {
          Item obj = ItemRegistry.Create(data.ItemId);
          if (obj != null)
            location.debris.Add(new Debris(obj, tileLocation * 64f + new Vector2(32f, 32f)));
        }
      }
      return true;
    }
label_8:
    return false;
  }

  public override void drawInMenu(
    SpriteBatch spriteBatch,
    Vector2 positionOnScreen,
    Vector2 tileLocation,
    float scale,
    float layerDepth)
  {
  }

  public override void draw(SpriteBatch spriteBatch)
  {
    Vector2 tile = this.Tile;
    FloorPathData data = this.GetData();
    if (data == null)
    {
      IItemDataDefinition itemDataDefinition = ItemRegistry.RequireTypeDefinition("(O)");
      spriteBatch.Draw(itemDataDefinition.GetErrorTexture(), Game1.GlobalToLocal(Game1.viewport, new Vector2(tile.X * 64f, tile.Y * 64f)), new Rectangle?(itemDataDefinition.GetErrorSourceRect()), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1E-09f);
    }
    else
    {
      Texture2D texture = this.GetTexture();
      Point textureCorner = this.GetTextureCorner();
      float num = 1f;
      switch (data.ConnectType)
      {
        case FloorPathConnectType.Default:
          int cornerSize1 = data.CornerSize;
          if (((int) this.neighborMask & 9) == 9 && ((int) this.neighborMask & 32 /*0x20*/) == 0)
            spriteBatch.Draw(texture, Game1.GlobalToLocal(Game1.viewport, new Vector2(tile.X * 64f, tile.Y * 64f)), new Rectangle?(new Rectangle(64 /*0x40*/ - cornerSize1 + textureCorner.X, 48 /*0x30*/ - cornerSize1 + textureCorner.Y, cornerSize1, cornerSize1)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) (((double) tile.Y * 64.0 + 2.0 + (double) tile.X / 10000.0) / 20000.0));
          if (((int) this.neighborMask & 3) == 3 && ((int) this.neighborMask & 16 /*0x10*/) == 0)
            spriteBatch.Draw(texture, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) ((double) tile.X * 64.0 + 64.0) - (float) (cornerSize1 * 4), tile.Y * 64f)), new Rectangle?(new Rectangle(16 /*0x10*/ + textureCorner.X, 48 /*0x30*/ - cornerSize1 + textureCorner.Y, cornerSize1, cornerSize1)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) (((double) tile.Y * 64.0 + 2.0 + (double) tile.X / 10000.0 + (double) num) / 20000.0));
          if (((int) this.neighborMask & 6) == 6 && ((int) this.neighborMask & 64 /*0x40*/) == 0)
            spriteBatch.Draw(texture, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) ((double) tile.X * 64.0 + 64.0) - (float) (cornerSize1 * 4), (float) ((double) tile.Y * 64.0 + 48.0))), new Rectangle?(new Rectangle(16 /*0x10*/ + textureCorner.X, textureCorner.Y, cornerSize1, cornerSize1)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) (((double) tile.Y * 64.0 + 2.0 + (double) tile.X / 10000.0) / 20000.0));
          if (((int) this.neighborMask & 12) == 12 && ((int) this.neighborMask & 128 /*0x80*/) == 0)
          {
            spriteBatch.Draw(texture, Game1.GlobalToLocal(Game1.viewport, new Vector2(tile.X * 64f, (float) ((double) tile.Y * 64.0 + 64.0) - (float) (cornerSize1 * 4))), new Rectangle?(new Rectangle(64 /*0x40*/ - cornerSize1 + textureCorner.X, textureCorner.Y, cornerSize1, cornerSize1)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) (((double) tile.Y * 64.0 + 2.0 + (double) tile.X / 10000.0) / 20000.0));
            break;
          }
          break;
        case FloorPathConnectType.CornerDecorated:
          int cornerSize2 = data.CornerSize;
          if (((int) this.neighborMask & 9) == 9 && ((int) this.neighborMask & 32 /*0x20*/) == 0)
            spriteBatch.Draw(texture, Game1.GlobalToLocal(Game1.viewport, new Vector2(tile.X * 64f, tile.Y * 64f)), new Rectangle?(new Rectangle(64 /*0x40*/ - cornerSize2 + textureCorner.X, 48 /*0x30*/ - cornerSize2 + textureCorner.Y, cornerSize2, cornerSize2)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) (((double) tile.Y * 64.0 + 2.0 + (double) tile.X / 10000.0) / 20000.0));
          if (((int) this.neighborMask & 3) == 3 && ((int) this.neighborMask & 16 /*0x10*/) == 0)
            spriteBatch.Draw(texture, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) ((double) tile.X * 64.0 + 64.0) - (float) (cornerSize2 * 4), tile.Y * 64f)), new Rectangle?(new Rectangle(16 /*0x10*/ + textureCorner.X, 48 /*0x30*/ - cornerSize2 + textureCorner.Y, cornerSize2, cornerSize2)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) (((double) tile.Y * 64.0 + 2.0 + (double) tile.X / 10000.0 + (double) num) / 20000.0));
          if (((int) this.neighborMask & 6) == 6 && ((int) this.neighborMask & 64 /*0x40*/) == 0)
            spriteBatch.Draw(texture, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) ((double) tile.X * 64.0 + 64.0) - (float) (cornerSize2 * 4), (float) ((double) tile.Y * 64.0 + 64.0) - (float) (cornerSize2 * 4))), new Rectangle?(new Rectangle(16 /*0x10*/ + textureCorner.X, textureCorner.Y, cornerSize2, cornerSize2)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) (((double) tile.Y * 64.0 + 2.0 + (double) tile.X / 10000.0) / 20000.0));
          if (((int) this.neighborMask & 12) == 12 && ((int) this.neighborMask & 128 /*0x80*/) == 0)
          {
            spriteBatch.Draw(texture, Game1.GlobalToLocal(Game1.viewport, new Vector2(tile.X * 64f, (float) ((double) tile.Y * 64.0 + 64.0) - (float) (cornerSize2 * 4))), new Rectangle?(new Rectangle(64 /*0x40*/ - cornerSize2 + textureCorner.X, textureCorner.Y, cornerSize2, cornerSize2)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) (((double) tile.Y * 64.0 + 2.0 + (double) tile.X / 10000.0) / 20000.0));
            break;
          }
          break;
      }
      byte key = (byte) ((uint) this.neighborMask & 15U);
      int drawGuide = Flooring.drawGuide[key];
      if (data.ConnectType == FloorPathConnectType.Random)
        drawGuide = Flooring.drawGuideList[this.whichView.Value];
      switch (data.ShadowType)
      {
        case FloorPathShadowType.Square:
          spriteBatch.Draw(Game1.staminaRect, new Rectangle((int) ((double) tile.X * 64.0) - 4 - Game1.viewport.X, (int) ((double) tile.Y * 64.0) + 4 - Game1.viewport.Y, 64 /*0x40*/, 64 /*0x40*/), Color.Black * 0.33f);
          break;
        case FloorPathShadowType.Contoured:
          Color black = Color.Black;
          black.A = (byte) ((double) black.A * 0.33000001311302185);
          spriteBatch.Draw(texture, Game1.GlobalToLocal(Game1.viewport, new Vector2(tile.X * 64f, tile.Y * 64f)) + new Vector2(-4f, 4f), new Rectangle?(new Rectangle(textureCorner.X + drawGuide * 16 /*0x10*/ % 256 /*0x0100*/, drawGuide / 16 /*0x10*/ * 16 /*0x10*/ + textureCorner.Y, 16 /*0x10*/, 16 /*0x10*/)), black, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1E-10f);
          break;
      }
      spriteBatch.Draw(texture, Game1.GlobalToLocal(Game1.viewport, new Vector2(tile.X * 64f, tile.Y * 64f)), new Rectangle?(new Rectangle(textureCorner.X + drawGuide * 16 /*0x10*/ % 256 /*0x0100*/, drawGuide / 16 /*0x10*/ * 16 /*0x10*/ + textureCorner.Y, 16 /*0x10*/, 16 /*0x10*/)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1E-09f);
    }
  }

  public override bool tickUpdate(GameTime time)
  {
    this.NeedsUpdate = false;
    return false;
  }

  private List<Flooring.Neighbor> gatherNeighbors()
  {
    List<Flooring.Neighbor> neighbors = this._neighbors;
    neighbors.Clear();
    GameLocation location = this.Location;
    Vector2 tile = this.Tile;
    NetVector2Dictionary<TerrainFeature, NetRef<TerrainFeature>> terrainFeatures = location.terrainFeatures;
    foreach (Flooring.NeighborLoc offset in Flooring._offsets)
    {
      Vector2 vector2 = tile + offset.Offset;
      if (location.map != null && !location.isTileOnMap(vector2))
      {
        Flooring.Neighbor neighbor = new Flooring.Neighbor((Flooring) null, offset.Direction, offset.InvDirection);
        neighbors.Add(neighbor);
      }
      else
      {
        TerrainFeature terrainFeature;
        if (terrainFeatures.TryGetValue(vector2, out terrainFeature) && terrainFeature is Flooring a && a.whichFloor.Value == this.whichFloor.Value)
        {
          Flooring.Neighbor neighbor = new Flooring.Neighbor(a, offset.Direction, offset.InvDirection);
          neighbors.Add(neighbor);
        }
      }
    }
    return neighbors;
  }

  public void OnAdded(GameLocation loc, Vector2 tilePos)
  {
    this.Location = loc;
    this.Tile = tilePos;
    List<Flooring.Neighbor> neighborList = this.gatherNeighbors();
    this.neighborMask = (byte) 0;
    foreach (Flooring.Neighbor neighbor in neighborList)
    {
      this.neighborMask |= neighbor.direction;
      neighbor.feature?.OnNeighborAdded(neighbor.invDirection);
    }
  }

  public void OnRemoved()
  {
    List<Flooring.Neighbor> neighborList = this.gatherNeighbors();
    this.neighborMask = (byte) 0;
    foreach (Flooring.Neighbor neighbor in neighborList)
      neighbor.feature?.OnNeighborRemoved(neighbor.invDirection);
  }

  public void OnNeighborAdded(byte direction) => this.neighborMask |= direction;

  public void OnNeighborRemoved(byte direction) => this.neighborMask &= ~direction;

  private struct NeighborLoc(Vector2 a, byte b, byte c)
  {
    public readonly Vector2 Offset = a;
    public readonly byte Direction = b;
    public readonly byte InvDirection = c;
  }

  private struct Neighbor(Flooring a, byte b, byte c)
  {
    public readonly Flooring feature = a;
    public readonly byte direction = b;
    public readonly byte invDirection = c;
  }
}
