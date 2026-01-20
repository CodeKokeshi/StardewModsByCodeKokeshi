// Decompiled with JetBrains decompiler
// Type: StardewValley.Locations.Mountain
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Events;
using StardewValley.Extensions;
using StardewValley.Network;
using StardewValley.TerrainFeatures;
using System;
using System.Xml.Serialization;
using xTile;
using xTile.Dimensions;
using xTile.Layers;
using xTile.ObjectModel;
using xTile.Tiles;

#nullable disable
namespace StardewValley.Locations;

public class Mountain : GameLocation
{
  public const int daysBeforeLandslide = 31 /*0x1F*/;
  private TemporaryAnimatedSprite minecartSteam;
  private bool bridgeRestored;
  [XmlIgnore]
  public bool treehouseBuilt;
  [XmlIgnore]
  public bool treehouseDoorDirty;
  private readonly NetBool oreBoulderPresent = new NetBool();
  private readonly NetBool railroadAreaBlocked = new NetBool(Game1.stats.DaysPlayed < 31U /*0x1F*/);
  private readonly NetBool landslide = new NetBool(Game1.stats.DaysPlayed < 5U);
  private Microsoft.Xna.Framework.Rectangle landSlideRect = new Microsoft.Xna.Framework.Rectangle(3200, 256 /*0x0100*/, 192 /*0xC0*/, 320);
  private Microsoft.Xna.Framework.Rectangle railroadBlockRect = new Microsoft.Xna.Framework.Rectangle(512 /*0x0200*/, 0, 256 /*0x0100*/, 320);
  private int oldTime;
  private Microsoft.Xna.Framework.Rectangle boulderSourceRect = new Microsoft.Xna.Framework.Rectangle(439, 1385, 39, 48 /*0x30*/);
  private Microsoft.Xna.Framework.Rectangle raildroadBlocksourceRect = new Microsoft.Xna.Framework.Rectangle(640, 2176, 64 /*0x40*/, 80 /*0x50*/);
  private Microsoft.Xna.Framework.Rectangle landSlideSourceRect = new Microsoft.Xna.Framework.Rectangle(646, 1218, 48 /*0x30*/, 80 /*0x50*/);
  private Vector2 boulderPosition = new Vector2(47f, 3f) * 64f - new Vector2(4f, 3f) * 4f;

  public Mountain()
  {
  }

  public Mountain(string map, string name)
    : base(map, name)
  {
    for (int index = 0; index < 10; ++index)
      this.quarryDayUpdate();
  }

  protected override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.oreBoulderPresent, "oreBoulderPresent").AddField((INetSerializable) this.railroadAreaBlocked, "railroadAreaBlocked").AddField((INetSerializable) this.landslide, "landslide");
  }

  public override bool checkAction(Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who)
  {
    switch (this.getTileIndexAt(tileLocation, "Buildings", "outdoors"))
    {
      case 958:
      case 1080:
      case 1081:
        this.ShowMineCartMenu("Default", "Quarry");
        return true;
      case 1136:
        if (!who.mailReceived.Contains("guildMember") && !who.hasQuest("16"))
        {
          Game1.drawLetterMessage(Game1.content.LoadString("Strings\\Locations:Mountain_AdventurersGuildNote").Replace('\n', '^'));
          return true;
        }
        break;
    }
    return base.checkAction(tileLocation, viewport, who);
  }

  public void ApplyTreehouseIfNecessary()
  {
    if ((Game1.farmEvent is WorldChangeEvent farmEvent ? (farmEvent.whichEvent.Value == 14 ? 1 : 0) : 0) == 0 && !Game1.MasterPlayer.mailReceived.Contains("leoMoved") && !Game1.MasterPlayer.mailReceived.Contains("leoMoved%&NL&%") || this.treehouseBuilt)
      return;
    TileSheet tileSheet = this.map.RequireTileSheet("untitled tile sheet2");
    Layer layer1 = this.map.RequireLayer("Buildings");
    Layer layer2 = this.map.RequireLayer("Back");
    layer1.Tiles[16 /*0x10*/, 6] = (Tile) new StaticTile(layer1, tileSheet, BlendMode.Alpha, 197);
    layer1.Tiles[16 /*0x10*/, 7] = (Tile) new StaticTile(layer1, tileSheet, BlendMode.Alpha, 213);
    layer2.Tiles[16 /*0x10*/, 8] = (Tile) new StaticTile(layer2, tileSheet, BlendMode.Alpha, 229);
    layer1.Tiles[16 /*0x10*/, 7].Properties["Action"] = (PropertyValue) "LockedDoorWarp 3 8 LeoTreeHouse 600 2300";
    this.treehouseBuilt = true;
    if (!Game1.IsMasterGame)
      return;
    this.updateDoors();
    this.treehouseDoorDirty = true;
  }

  private void restoreBridge()
  {
    LocalizedContentManager temporary = Game1.content.CreateTemporary();
    Map map = temporary.Load<Map>("Maps\\Mountain-BridgeFixed");
    int num1 = 92;
    int num2 = 24;
    Layer layer1 = this.map.RequireLayer("Back");
    Layer layer2 = this.map.RequireLayer("Buildings");
    Layer layer3 = this.map.RequireLayer("Front");
    Layer layer4 = map.RequireLayer("Back");
    Layer layer5 = map.RequireLayer("Buildings");
    Layer layer6 = map.RequireLayer("Front");
    TileSheet tileSheet = this.map.RequireTileSheet(0, "outdoors");
    for (int x = 0; x < layer4.LayerWidth; ++x)
    {
      for (int y = 0; y < layer4.LayerHeight; ++y)
      {
        layer1.Tiles[x + num1, y + num2] = layer4.Tiles[x, y] == null ? (Tile) null : (Tile) new StaticTile(layer1, tileSheet, BlendMode.Alpha, layer4.Tiles[x, y].TileIndex);
        layer2.Tiles[x + num1, y + num2] = layer5.Tiles[x, y] == null ? (Tile) null : (Tile) new StaticTile(layer2, tileSheet, BlendMode.Alpha, layer5.Tiles[x, y].TileIndex);
        layer3.Tiles[x + num1, y + num2] = layer6.Tiles[x, y] == null ? (Tile) null : (Tile) new StaticTile(layer3, tileSheet, BlendMode.Alpha, layer6.Tiles[x, y].TileIndex);
      }
    }
    this.bridgeRestored = true;
    temporary.Unload();
  }

  protected override void resetSharedState()
  {
    base.resetSharedState();
    this.oreBoulderPresent.Value = !Game1.MasterPlayer.mailReceived.Contains("ccFishTank") || Game1.farmEvent != null;
    Vector2 key1 = new Vector2(29f, 9f);
    if (!this.objects.ContainsKey(key1))
    {
      OverlaidDictionary objects = this.objects;
      Vector2 key2 = key1;
      Torch torch = new Torch("146", true);
      torch.IsOn = false;
      torch.Fragility = 2;
      objects.Add(key2, (StardewValley.Object) torch);
      this.objects[key1].checkForAction((Farmer) null);
    }
    if (Game1.stats.DaysPlayed >= 5U)
      this.landslide.Value = false;
    if (Game1.stats.DaysPlayed < 31U /*0x1F*/)
      return;
    this.railroadAreaBlocked.Value = false;
  }

  public override void MakeMapModifications(bool force = false)
  {
    base.MakeMapModifications(force);
    if (force)
    {
      this.treehouseBuilt = false;
      this.bridgeRestored = false;
    }
    if (!this.bridgeRestored && Utility.doesMasterPlayerHaveMailReceivedButNotMailForTomorrow("ccCraftsRoom"))
      this.restoreBridge();
    if ((Game1.farmEvent is WorldChangeEvent farmEvent ? (farmEvent.whichEvent.Value == 14 ? 1 : 0) : 0) == 0)
      this.ApplyTreehouseIfNecessary();
    if (!Game1.MasterPlayer.mailReceived.Contains("communityUpgradeShortcuts"))
      return;
    this.ApplyMapOverride("Mountain_Shortcuts");
    this.waterTiles[81, 37] = false;
    this.waterTiles[82, 37] = false;
    this.waterTiles[83, 37] = false;
    this.waterTiles[84, 37] = false;
    this.waterTiles[85, 37] = false;
    this.waterTiles[85, 38] = false;
    this.waterTiles[85, 39] = false;
    this.waterTiles[85, 40] = false;
  }

  protected override void resetLocalState()
  {
    base.resetLocalState();
    if (Game1.MasterPlayer.mailReceived.Contains("ccBoilerRoom"))
      this.minecartSteam = new TemporaryAnimatedSprite(27, new Vector2(8072f, 656f), Color.White)
      {
        totalNumberOfLoops = 999999,
        interval = 60f,
        flipped = true
      };
    Season season = this.GetSeason();
    this.boulderSourceRect = new Microsoft.Xna.Framework.Rectangle(439 + (season == Season.Winter ? 39 : 0), 1385, 39, 48 /*0x30*/);
    this.raildroadBlocksourceRect = new Microsoft.Xna.Framework.Rectangle(640, season == Season.Spring ? 2176 : 1453, 64 /*0x40*/, 80 /*0x50*/);
    this.addFrog();
    if (!Game1.IsWinter)
      return;
    Game1.currentLightSources.Add(new LightSource("Mountain_1", 4, new Vector2(800f, 1366f), 0.5f, onlyLocation: this.NameOrUniqueName));
    Game1.currentLightSources.Add(new LightSource("Mountain_2", 4, new Vector2(544f, 1155f), 0.5f, onlyLocation: this.NameOrUniqueName));
    Game1.currentLightSources.Add(new LightSource("Mountain_3", 4, new Vector2(924f, 1563f), 0.5f, onlyLocation: this.NameOrUniqueName));
    Game1.currentLightSources.Add(new LightSource("Mountain_4", 4, new Vector2(673f, 1567f), 0.5f, onlyLocation: this.NameOrUniqueName));
  }

  public override void DayUpdate(int dayOfMonth)
  {
    base.DayUpdate(dayOfMonth);
    this.quarryDayUpdate();
    if (Game1.stats.DaysPlayed >= 31U /*0x1F*/)
      this.railroadAreaBlocked.Value = false;
    if (Game1.stats.DaysPlayed >= 5U)
    {
      this.landslide.Value = false;
      if (!Game1.player.hasOrWillReceiveMail("landslideDone"))
        Game1.addMail("landslideDone", sendToEveryone: true);
    }
    if (!Game1.IsFall || Game1.dayOfMonth != 17)
      return;
    this.tryPlaceObject(new Vector2(11f, 26f), ItemRegistry.Create<StardewValley.Object>("(O)746"));
  }

  private void quarryDayUpdate()
  {
    Microsoft.Xna.Framework.Rectangle r = new Microsoft.Xna.Framework.Rectangle(106, 13, 22, 22);
    int num1 = Math.Min(16 /*0x10*/, 5 + Game1.year * 2);
    for (int index = 0; index < num1; ++index)
    {
      Vector2 positionInThisRectangle = Utility.getRandomPositionInThisRectangle(r, Game1.random);
      int num2;
      if (this.isTileOpenForQuarryStone((int) positionInThisRectangle.X, (int) positionInThisRectangle.Y))
      {
        if (Game1.random.NextDouble() < 0.06)
        {
          NetVector2Dictionary<TerrainFeature, NetRef<TerrainFeature>> terrainFeatures = this.terrainFeatures;
          Vector2 key = positionInThisRectangle;
          num2 = 1 + Game1.random.Next(2);
          Tree tree = new Tree(num2.ToString(), 1);
          terrainFeatures.Add(key, (TerrainFeature) tree);
        }
        else if (Game1.random.NextDouble() < 0.02)
        {
          if (Game1.random.NextDouble() < 0.1)
          {
            OverlaidDictionary objects = this.objects;
            Vector2 key = positionInThisRectangle;
            num2 = 46;
            objects.Add(key, new StardewValley.Object(num2.ToString(), 1)
            {
              MinutesUntilReady = 12
            });
          }
          else
          {
            OverlaidDictionary objects = this.objects;
            Vector2 key = positionInThisRectangle;
            num2 = (Game1.random.Next(7) + 1) * 2;
            objects.Add(key, new StardewValley.Object(num2.ToString(), 1)
            {
              MinutesUntilReady = 5
            });
          }
        }
        else if (Game1.random.NextDouble() < 0.04)
          this.objects.Add(positionInThisRectangle, ItemRegistry.Create<StardewValley.Object>(Game1.random.NextBool(0.15) ? "(O)SeedSpot" : "(O)590"));
        else if (Game1.random.NextDouble() < 0.15)
        {
          if (Game1.random.NextDouble() < 0.001)
            this.objects.Add(positionInThisRectangle, new StardewValley.Object("765", 1)
            {
              MinutesUntilReady = 16 /*0x10*/
            });
          else if (Game1.random.NextDouble() < 0.1)
            this.objects.Add(positionInThisRectangle, new StardewValley.Object("764", 1)
            {
              MinutesUntilReady = 8
            });
          else if (Game1.random.NextDouble() < 0.33)
            this.objects.Add(positionInThisRectangle, new StardewValley.Object("290", 1)
            {
              MinutesUntilReady = 5
            });
          else
            this.objects.Add(positionInThisRectangle, new StardewValley.Object("751", 1)
            {
              MinutesUntilReady = 3
            });
        }
        else if (Game1.random.NextDouble() < 0.1)
        {
          this.objects.Add(positionInThisRectangle, new StardewValley.Object(Game1.random.Choose<string>("BasicCoalNode0", "BasicCoalNode1"), 1)
          {
            MinutesUntilReady = 5
          });
        }
        else
        {
          string itemId = Game1.random.Choose<string>("32", "38", "40", "42", "668", "670");
          this.objects.Add(positionInThisRectangle, new StardewValley.Object(itemId, 1)
          {
            MinutesUntilReady = 2
          });
        }
      }
    }
  }

  private bool isTileOpenForQuarryStone(int tileX, int tileY)
  {
    return this.doesTileHaveProperty(tileX, tileY, "Diggable", "Back") != null && this.CanItemBePlacedHere(new Vector2((float) tileX, (float) tileY), ignorePassables: CollisionMask.None);
  }

  public override void cleanupBeforePlayerExit()
  {
    base.cleanupBeforePlayerExit();
    this.minecartSteam = (TemporaryAnimatedSprite) null;
  }

  public override void UpdateWhenCurrentLocation(GameTime time)
  {
    base.UpdateWhenCurrentLocation(time);
    this.minecartSteam?.update(time);
    if (!this.landslide.Value || (int) ((Game1.currentGameTime.TotalGameTime.TotalMilliseconds - 400.0) / 1600.0) % 2 == 0 || !Utility.isOnScreen(new Point(this.landSlideRect.X / 64 /*0x40*/, this.landSlideRect.Y / 64 /*0x40*/), 128 /*0x80*/))
      return;
    if (Game1.currentGameTime.TotalGameTime.TotalMilliseconds % 400.0 < (double) (this.oldTime % 400))
      this.localSound("hammer");
    this.oldTime = (int) time.TotalGameTime.TotalMilliseconds;
  }

  public override bool isCollidingPosition(
    Microsoft.Xna.Framework.Rectangle position,
    xTile.Dimensions.Rectangle viewport,
    bool isFarmer,
    int damagesFarmer,
    bool glider,
    Character character)
  {
    return this.landslide.Value && position.Intersects(this.landSlideRect) || this.railroadAreaBlocked.Value && position.Intersects(this.railroadBlockRect) || base.isCollidingPosition(position, viewport, isFarmer, damagesFarmer, glider, character);
  }

  public override bool isTilePlaceable(Vector2 tileLocation, bool itemIsPassable = false)
  {
    Point point = Utility.Vector2ToPoint((tileLocation + new Vector2(0.5f, 0.5f)) * 64f);
    return (!this.landslide.Value || !this.landSlideRect.Contains(point)) && (!this.railroadAreaBlocked.Value || !this.railroadBlockRect.Contains(point)) && base.isTilePlaceable(tileLocation, itemIsPassable);
  }

  public override void draw(SpriteBatch spriteBatch)
  {
    base.draw(spriteBatch);
    this.minecartSteam?.draw(spriteBatch);
    if (this.oreBoulderPresent.Value)
      spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, this.boulderPosition), new Microsoft.Xna.Framework.Rectangle?(this.boulderSourceRect), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.0001f);
    if (this.railroadAreaBlocked.Value)
      spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, this.railroadBlockRect), new Microsoft.Xna.Framework.Rectangle?(this.raildroadBlocksourceRect), Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 0.0193f);
    if (!this.landslide.Value)
      return;
    spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, this.landSlideRect), new Microsoft.Xna.Framework.Rectangle?(this.landSlideSourceRect), Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 0.0192f);
    SpriteBatch spriteBatch1 = spriteBatch;
    Texture2D shadowTexture = Game1.shadowTexture;
    Vector2 local = Game1.GlobalToLocal(new Vector2((float) (this.landSlideRect.X + 192 /*0xC0*/ - 20), (float) (this.landSlideRect.Y + 192 /*0xC0*/ + 20)) + new Vector2(32f, 24f));
    Microsoft.Xna.Framework.Rectangle? sourceRectangle = new Microsoft.Xna.Framework.Rectangle?(Game1.shadowTexture.Bounds);
    Color white = Color.White;
    Microsoft.Xna.Framework.Rectangle bounds = Game1.shadowTexture.Bounds;
    double x = (double) bounds.Center.X;
    bounds = Game1.shadowTexture.Bounds;
    double y = (double) bounds.Center.Y;
    Vector2 origin = new Vector2((float) x, (float) y);
    spriteBatch1.Draw(shadowTexture, local, sourceRectangle, white, 0.0f, origin, 4f, SpriteEffects.None, 0.0224f);
    spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(new Vector2((float) (this.landSlideRect.X + 192 /*0xC0*/ - 20), (float) (this.landSlideRect.Y + 128 /*0x80*/))), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(288 + ((int) (Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 1600.0 % 2.0) == 0 ? 0 : (int) (Game1.currentGameTime.TotalGameTime.TotalMilliseconds % 400.0 / 100.0) * 19), 1349, 19, 28)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.0256f);
    spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(new Vector2((float) (this.landSlideRect.X + 256 /*0x0100*/ - 20), (float) (this.landSlideRect.Y + 128 /*0x80*/))), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(335, 1410, 21, 21)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.0128f);
  }
}
