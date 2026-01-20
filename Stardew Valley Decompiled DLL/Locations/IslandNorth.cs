// Decompiled with JetBrains decompiler
// Type: StardewValley.Locations.IslandNorth
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.BellsAndWhistles;
using StardewValley.Extensions;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using xTile.Dimensions;

#nullable disable
namespace StardewValley.Locations;

public class IslandNorth : IslandLocation
{
  [XmlElement("bridgeFixed")]
  public readonly NetBool bridgeFixed;
  [XmlElement("traderActivated")]
  public readonly NetBool traderActivated;
  [XmlElement("caveOpened")]
  public readonly NetBool caveOpened;
  [XmlElement("treeNutShot")]
  public readonly NetBool treeNutShot;
  [XmlIgnore]
  public List<SuspensionBridge> suspensionBridges;
  [XmlIgnore]
  protected bool _sawFlameSpriteSouth;
  [XmlIgnore]
  protected bool _sawFlameSpriteNorth;
  [XmlIgnore]
  protected bool hasTriedFirstEntryDigSiteLoad;
  private float boulderKnockTimer;
  private float boulderTextTimer;
  private string boulderTextString;
  private int boulderKnocksLeft;
  private Microsoft.Xna.Framework.Rectangle boulderPosition;
  private float doneHittingBoulderWithToolTimer;

  protected override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.bridgeFixed, "bridgeFixed").AddField((INetSerializable) this.traderActivated, "traderActivated").AddField((INetSerializable) this.caveOpened, "caveOpened").AddField((INetSerializable) this.treeNutShot, "treeNutShot");
    this.bridgeFixed.fieldChangeEvent += (FieldChange<NetBool, bool>) ((f, oldValue, newValue) =>
    {
      if (!newValue || this.mapPath.Value == null)
        return;
      this.ApplyFixedBridge();
    });
    this.traderActivated.fieldChangeEvent += (FieldChange<NetBool, bool>) ((f, oldValue, newValue) =>
    {
      if (Utility.ShouldIgnoreValueChangeCallback())
        return;
      this.ApplyIslandTraderHut();
    });
    this.caveOpened.fieldChangeEvent += (FieldChange<NetBool, bool>) ((f, oldValue, newValue) =>
    {
      if (Utility.ShouldIgnoreValueChangeCallback())
        return;
      this.ApplyCaveOpened();
    });
  }

  public override void SetBuriedNutLocations()
  {
    this.buriedNutPoints.Add(new Point(57, 79));
    this.buriedNutPoints.Add(new Point(19, 39));
    this.buriedNutPoints.Add(new Point(19, 13));
    this.buriedNutPoints.Add(new Point(54, 21));
    this.buriedNutPoints.Add(new Point(42, 77));
    this.buriedNutPoints.Add(new Point(62, 54));
    this.buriedNutPoints.Add(new Point(26, 81));
    base.SetBuriedNutLocations();
  }

  public virtual void ApplyFixedBridge()
  {
    if (this.map == null)
      return;
    this.ApplyMapOverride("Island_Bridge_Repaired", destination_rect: new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(31 /*0x1F*/, 52, 4, 3)));
  }

  public virtual void ApplyIslandTraderHut()
  {
    if (this.map == null)
      return;
    this.ApplyMapOverride("Island_N_Trader", destination_rect: new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(32 /*0x20*/, 64 /*0x40*/, 9, 10)));
    this.removeTemporarySpritesWithIDLocal(8989);
    this.removeTemporarySpritesWithIDLocal(8988);
    this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(276, 1985, 12, 11), new Vector2(33.45f, 70.33f) * 64f + new Vector2(-16f, -32f), false, 0.0f, Color.White)
    {
      delayBeforeAnimationStart = 10,
      interval = 50f,
      totalNumberOfLoops = 99999,
      animationLength = 4,
      lightId = "IslandNorth_Trader_1",
      id = 8989,
      lightRadius = 2f,
      scale = 4f,
      layerDepth = 0.46144f
    });
    this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(276, 1985, 12, 11), new Vector2(39.45f, 70.33f) * 64f + new Vector2(-16f, -32f), false, 0.0f, Color.White)
    {
      delayBeforeAnimationStart = 10,
      interval = 50f,
      totalNumberOfLoops = 99999,
      animationLength = 4,
      lightId = "IslandNorth_Trader_2",
      id = 8988,
      lightRadius = 2f,
      scale = 4f,
      layerDepth = 0.46144f
    });
  }

  public virtual void ApplyCaveOpened()
  {
    if (Game1.player.currentLocation == null || !Game1.player.currentLocation.Equals((GameLocation) this))
      return;
    for (int index = 0; index < 12; ++index)
    {
      this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors2", new Microsoft.Xna.Framework.Rectangle(146, 229 + Game1.random.Next(3) * 9, 9, 9), Utility.getRandomPositionInThisRectangle(this.boulderPosition, Game1.random), Game1.random.NextBool(), 0.0f, Color.White)
      {
        scale = 4f,
        motion = new Vector2((float) Game1.random.Next(-3, 1), (float) Game1.random.Next(-15, -9)),
        acceleration = new Vector2(0.0f, 0.4f),
        rotationChange = (float) Game1.random.Next(-2, 3) * 0.01f,
        drawAboveAlwaysFront = true,
        yStopCoordinate = this.boulderPosition.Bottom + 1 + Game1.random.Next(64 /*0x40*/),
        delayBeforeAnimationStart = index * 15
      });
      this.temporarySprites[this.temporarySprites.Count - 1].initialPosition.Y = (float) this.temporarySprites[this.temporarySprites.Count - 1].yStopCoordinate;
      this.temporarySprites[this.temporarySprites.Count - 1].reachedStopCoordinate = new TemporaryAnimatedSprite.endBehavior(this.temporarySprites[this.temporarySprites.Count - 1].bounce);
    }
    for (int index = 0; index < 8; ++index)
      this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(372, 1956, 10, 10), Utility.getRandomPositionInThisRectangle(this.boulderPosition, Game1.random) + new Vector2(-32f, -32f), false, 0.007f, Color.White)
      {
        alpha = 0.75f,
        motion = new Vector2(0.0f, -1f),
        acceleration = new Vector2(1f / 500f, 0.0f),
        interval = 99999f,
        layerDepth = 1f,
        scale = 4f,
        scaleChange = 0.02f,
        rotationChange = (float) ((double) Game1.random.Next(-5, 6) * 3.1415927410125732 / 256.0),
        delayBeforeAnimationStart = index * 40
      });
    Game1.playSound("boulderBreak");
    Game1.player.freezePause = 3000;
    DelayedAction.functionAfterDelay((Action) (() => Game1.globalFadeToBlack((Game1.afterFadeFunction) (() => this.startEvent(new StardewValley.Event(Game1.content.LoadString("Strings\\Locations:IslandNorth_Event_SafariManAppear")))))), 1000);
  }

  public override string checkForBuriedItem(
    int xLocation,
    int yLocation,
    bool explosion,
    bool detectOnly,
    Farmer who)
  {
    if (xLocation == 27 && yLocation == 28 && who.secretNotesSeen.Contains(1010))
    {
      Game1.player.team.RequestLimitedNutDrops("Island_N_BuriedTreasureNut", (GameLocation) this, xLocation * 64 /*0x40*/, yLocation * 64 /*0x40*/, 1);
      if (!Game1.player.hasOrWillReceiveMail("Island_N_BuriedTreasure"))
      {
        Game1.createItemDebris(ItemRegistry.Create("(O)289"), new Vector2((float) xLocation, (float) yLocation) * 64f, 1);
        Game1.addMailForTomorrow("Island_N_BuriedTreasure", true);
      }
    }
    if (xLocation == 26 && yLocation == 81 && !Game1.player.team.collectedNutTracker.Contains("Buried_IslandNorth_26_81"))
      DelayedAction.functionAfterDelay((Action) (() =>
      {
        TemporaryAnimatedSprite t = this.getTemporarySpriteByID(79797);
        if (t == null)
          return;
        t.sourceRectStartingPos.X += 40f;
        t.sourceRect.X = 181;
        t.interval = 100f;
        t.shakeIntensity = 1f;
        this.playSound("monkey1");
        t.motion = new Vector2(-3f, -10f);
        t.acceleration = new Vector2(0.0f, 0.3f);
        t.yStopCoordinate = (int) t.position.Y + 1;
        t.reachedStopCoordinate = (TemporaryAnimatedSprite.endBehavior) (x =>
        {
          this.temporarySprites.Add(new TemporaryAnimatedSprite(50, t.position, Color.Green)
          {
            drawAboveAlwaysFront = true
          });
          this.removeTemporarySpritesWithID(79797);
          this.playSound("leafrustle");
        });
      }), 700);
    return base.checkForBuriedItem(xLocation, yLocation, explosion, detectOnly, who);
  }

  public IslandNorth()
  {
    NetBool netBool1 = new NetBool();
    netBool1.InterpolationWait = false;
    this.bridgeFixed = netBool1;
    NetBool netBool2 = new NetBool();
    netBool2.InterpolationWait = false;
    this.traderActivated = netBool2;
    NetBool netBool3 = new NetBool();
    netBool3.InterpolationWait = false;
    this.caveOpened = netBool3;
    NetBool netBool4 = new NetBool();
    netBool4.InterpolationWait = false;
    this.treeNutShot = netBool4;
    this.suspensionBridges = new List<SuspensionBridge>();
    this.boulderPosition = new Microsoft.Xna.Framework.Rectangle(1344, 3008, 128 /*0x80*/, 64 /*0x40*/);
    // ISSUE: explicit constructor call
    base.\u002Ector();
  }

  public override bool isCollidingPosition(
    Microsoft.Xna.Framework.Rectangle position,
    xTile.Dimensions.Rectangle viewport,
    bool isFarmer,
    int damagesFarmer,
    bool glider,
    Character character,
    bool pathfinding,
    bool projectile = false,
    bool ignoreCharacterRequirement = false,
    bool skipCollisionEffects = false)
  {
    if (!projectile || damagesFarmer != 0 || position.Bottom >= 832)
      return base.isCollidingPosition(position, viewport, isFarmer, damagesFarmer, glider, character, pathfinding, projectile, ignoreCharacterRequirement, false);
    if (!position.Intersects(new Microsoft.Xna.Framework.Rectangle(3648, 576, 256 /*0x0100*/, 64 /*0x40*/)))
      return false;
    if (Game1.IsMasterGame && !this.treeNutShot.Value)
    {
      Game1.player.team.MarkCollectedNut("TreeNutShot");
      this.treeNutShot.Value = true;
      Game1.createItemDebris(ItemRegistry.Create("(O)73"), new Vector2(58.5f, 11f) * 64f, 0, (GameLocation) this, 0);
    }
    return true;
  }

  public IslandNorth(string map, string name)
  {
    NetBool netBool1 = new NetBool();
    netBool1.InterpolationWait = false;
    this.bridgeFixed = netBool1;
    NetBool netBool2 = new NetBool();
    netBool2.InterpolationWait = false;
    this.traderActivated = netBool2;
    NetBool netBool3 = new NetBool();
    netBool3.InterpolationWait = false;
    this.caveOpened = netBool3;
    NetBool netBool4 = new NetBool();
    netBool4.InterpolationWait = false;
    this.treeNutShot = netBool4;
    this.suspensionBridges = new List<SuspensionBridge>();
    this.boulderPosition = new Microsoft.Xna.Framework.Rectangle(1344, 3008, 128 /*0x80*/, 64 /*0x40*/);
    // ISSUE: explicit constructor call
    base.\u002Ector(map, name);
    this.parrotUpgradePerches.Clear();
    this.parrotUpgradePerches.Add(new ParrotUpgradePerch((GameLocation) this, new Point(35, 52), new Microsoft.Xna.Framework.Rectangle(31 /*0x1F*/, 52, 4, 4), 10, (Action) (() =>
    {
      Game1.addMailForTomorrow("Island_UpgradeBridge", true, true);
      this.bridgeFixed.Value = true;
    }), (Func<bool>) (() => this.bridgeFixed.Value), "Bridge", "Island_Turtle"));
    this.parrotUpgradePerches.Add(new ParrotUpgradePerch((GameLocation) this, new Point(32 /*0x20*/, 72), new Microsoft.Xna.Framework.Rectangle(33, 68, 5, 5), 10, (Action) (() =>
    {
      Game1.addMailForTomorrow("Island_UpgradeTrader", true, true);
      this.traderActivated.Value = true;
    }), (Func<bool>) (() => this.traderActivated.Value), "Trader", "Island_UpgradeHouse"));
    if (!Game1.netWorldState.Value.ActivatedGoldenParrot && Game1.netWorldState.Value.GoldenWalnutsFound < 130)
      this.parrotUpgradePerches.Add(new ParrotUpgradePerch((GameLocation) this, new Point(14, 14), new Microsoft.Xna.Framework.Rectangle(2, 2, this.Map.Layers[0].LayerWidth - 4, this.Map.Layers[0].LayerHeight - 4), -1, (Action) (() => { }), (Func<bool>) (() => false), "GoldenParrot"));
    this.largeTerrainFeatures.Add((LargeTerrainFeature) new Bush(new Vector2(45f, 38f), 4, (GameLocation) this));
    this.largeTerrainFeatures.Add((LargeTerrainFeature) new Bush(new Vector2(47f, 40f), 4, (GameLocation) this));
    this.largeTerrainFeatures.Add((LargeTerrainFeature) new Bush(new Vector2(13f, 33f), 4, (GameLocation) this));
    this.largeTerrainFeatures.Add((LargeTerrainFeature) new Bush(new Vector2(5f, 30f), 4, (GameLocation) this));
  }

  public override void TransferDataFromSavedLocation(GameLocation l)
  {
    if (l is IslandNorth islandNorth)
    {
      this.bridgeFixed.Value = islandNorth.bridgeFixed.Value;
      this.treeNutShot.Value = islandNorth.treeNutShot.Value;
      this.caveOpened.Value = islandNorth.caveOpened.Value;
      this.traderActivated.Value = islandNorth.traderActivated.Value;
    }
    base.TransferDataFromSavedLocation(l);
  }

  public override bool checkAction(Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who)
  {
    switch (this.getTileIndexAt(tileLocation.X, tileLocation.Y, "Buildings", "untitled tile sheet"))
    {
      case 2074:
      case 2075:
      case 2076:
      case 2077:
      case 2078:
        Utility.TryOpenShopMenu("IslandTrade", (string) null);
        return true;
      default:
        return base.checkAction(tileLocation, viewport, who);
    }
  }

  public override List<Vector2> GetAdditionalWalnutBushes()
  {
    return new List<Vector2>() { new Vector2(56f, 27f) };
  }

  /// <inheritdoc />
  public override bool catchOceanCrabPotFishFromThisSpot(int x, int y) => false;

  public override void UpdateWhenCurrentLocation(GameTime time)
  {
    base.UpdateWhenCurrentLocation(time);
    foreach (SuspensionBridge suspensionBridge in this.suspensionBridges)
      suspensionBridge.Update(time);
    if (!this.caveOpened.Value && Utility.isOnScreen(Utility.PointToVector2(this.boulderPosition.Location), 1))
    {
      double boulderKnockTimer = (double) this.boulderKnockTimer;
      TimeSpan elapsedGameTime = time.ElapsedGameTime;
      double totalMilliseconds1 = elapsedGameTime.TotalMilliseconds;
      this.boulderKnockTimer = (float) (boulderKnockTimer - totalMilliseconds1);
      double boulderTextTimer = (double) this.boulderTextTimer;
      elapsedGameTime = time.ElapsedGameTime;
      double totalMilliseconds2 = elapsedGameTime.TotalMilliseconds;
      this.boulderTextTimer = (float) (boulderTextTimer - totalMilliseconds2);
      if ((double) this.doneHittingBoulderWithToolTimer > 0.0)
      {
        double boulderWithToolTimer = (double) this.doneHittingBoulderWithToolTimer;
        elapsedGameTime = time.ElapsedGameTime;
        double totalMilliseconds3 = elapsedGameTime.TotalMilliseconds;
        this.doneHittingBoulderWithToolTimer = (float) (boulderWithToolTimer - totalMilliseconds3);
        if ((double) this.doneHittingBoulderWithToolTimer <= 0.0)
        {
          this.boulderTextTimer = 2000f;
          this.boulderTextString = Game1.content.LoadString("Strings\\Locations:IslandNorth_CaveTool_" + Game1.random.Next(4).ToString());
        }
      }
      if (this.boulderKnocksLeft > 0)
      {
        if ((double) this.boulderKnockTimer < 0.0)
        {
          Game1.playSound("hammer");
          --this.boulderKnocksLeft;
          this.boulderKnockTimer = 500f;
          if (this.boulderKnocksLeft == 0 && Game1.random.NextBool())
            DelayedAction.functionAfterDelay((Action) (() =>
            {
              this.boulderTextTimer = 2000f;
              this.boulderTextString = Game1.content.LoadString("Strings\\Locations:IslandNorth_CaveHelp_" + Game1.random.Next(4).ToString());
            }), 1000);
        }
      }
      else if (Game1.random.NextDouble() < 0.002 && (double) this.boulderTextTimer < -500.0)
        this.boulderKnocksLeft = Game1.random.Next(3, 6);
    }
    if (!this._sawFlameSpriteSouth && Utility.isThereAFarmerWithinDistance(new Vector2(36f, 79f), 5, (GameLocation) this) == Game1.player)
    {
      Game1.addMailForTomorrow("Saw_Flame_Sprite_North_South", true);
      TemporaryAnimatedSprite temporarySpriteById1 = this.getTemporarySpriteByID(999);
      if (temporarySpriteById1 != null)
      {
        temporarySpriteById1.yPeriodic = false;
        temporarySpriteById1.xPeriodic = false;
        temporarySpriteById1.sourceRect.Y = 0;
        temporarySpriteById1.sourceRectStartingPos.Y = 0.0f;
        temporarySpriteById1.motion = new Vector2(1f, -4f);
        temporarySpriteById1.acceleration = new Vector2(0.0f, -0.04f);
        temporarySpriteById1.drawAboveAlwaysFront = true;
      }
      this.localSound("magma_sprite_spot");
      TemporaryAnimatedSprite temporarySpriteById2 = this.getTemporarySpriteByID(998);
      if (temporarySpriteById2 != null)
      {
        temporarySpriteById2.yPeriodic = false;
        temporarySpriteById2.xPeriodic = false;
        temporarySpriteById2.motion = new Vector2(1f, -4f);
        temporarySpriteById2.acceleration = new Vector2(0.0f, -0.04f);
      }
      this._sawFlameSpriteSouth = true;
    }
    if (!this._sawFlameSpriteNorth && Utility.isThereAFarmerWithinDistance(new Vector2(41f, 30f), 5, (GameLocation) this) == Game1.player)
    {
      Game1.addMailForTomorrow("Saw_Flame_Sprite_North_North", true);
      TemporaryAnimatedSprite temporarySpriteById3 = this.getTemporarySpriteByID(9999);
      if (temporarySpriteById3 != null)
      {
        temporarySpriteById3.yPeriodic = false;
        temporarySpriteById3.xPeriodic = false;
        temporarySpriteById3.sourceRect.Y = 0;
        temporarySpriteById3.sourceRectStartingPos.Y = 0.0f;
        temporarySpriteById3.motion = new Vector2(0.0f, -4f);
        temporarySpriteById3.acceleration = new Vector2(0.0f, -0.04f);
        temporarySpriteById3.yStopCoordinate = 1216;
        temporarySpriteById3.reachedStopCoordinate = (TemporaryAnimatedSprite.endBehavior) (x => this.removeTemporarySpritesWithID(9999));
      }
      this.localSound("magma_sprite_spot");
      TemporaryAnimatedSprite temporarySpriteById4 = this.getTemporarySpriteByID(9998);
      if (temporarySpriteById4 != null)
      {
        temporarySpriteById4.yPeriodic = false;
        temporarySpriteById4.xPeriodic = false;
        temporarySpriteById4.motion = new Vector2(0.0f, -4f);
        temporarySpriteById4.acceleration = new Vector2(0.0f, -0.04f);
        temporarySpriteById4.yStopCoordinate = 1280 /*0x0500*/;
        temporarySpriteById4.reachedStopCoordinate = (TemporaryAnimatedSprite.endBehavior) (x => this.removeTemporarySpritesWithID(9998));
      }
      this._sawFlameSpriteNorth = true;
    }
    if (this.hasTriedFirstEntryDigSiteLoad)
      return;
    if (Game1.IsMasterGame && !Game1.player.hasOrWillReceiveMail("ISLAND_NORTH_DIGSITE_LOAD"))
    {
      Game1.addMail("ISLAND_NORTH_DIGSITE_LOAD", true);
      for (int index = 0; index < 40; ++index)
        this.digSiteUpdate();
    }
    this.hasTriedFirstEntryDigSiteLoad = true;
  }

  public override bool isCollidingPosition(
    Microsoft.Xna.Framework.Rectangle position,
    xTile.Dimensions.Rectangle viewport,
    bool isFarmer,
    int damagesFarmer,
    bool glider,
    Character character)
  {
    return !this.caveOpened.Value && this.boulderPosition.Intersects(position) || base.isCollidingPosition(position, viewport, isFarmer, damagesFarmer, glider, character);
  }

  public override bool isTilePlaceable(Vector2 tile_location, bool itemIsPassable = false)
  {
    Point point = Utility.Vector2ToPoint((tile_location + new Vector2(0.5f, 0.5f)) * 64f);
    return (this.caveOpened.Value || !this.boulderPosition.Contains(point)) && base.isTilePlaceable(tile_location, itemIsPassable);
  }

  public override void DayUpdate(int dayOfMonth)
  {
    base.DayUpdate(dayOfMonth);
    this.digSiteUpdate();
    this.terrainFeatures.RemoveWhere((Func<KeyValuePair<Vector2, TerrainFeature>, bool>) (pair => pair.Value is HoeDirt hoeDirt1 && hoeDirt1.crop != null && hoeDirt1.crop.forageCrop.Value));
    Microsoft.Xna.Framework.Rectangle[] rectangleArray = new Microsoft.Xna.Framework.Rectangle[4]
    {
      new Microsoft.Xna.Framework.Rectangle(10, 51, 1, 8),
      new Microsoft.Xna.Framework.Rectangle(15, 59, 1, 4),
      new Microsoft.Xna.Framework.Rectangle(18, 34, 1, 1),
      new Microsoft.Xna.Framework.Rectangle(40, 48 /*0x30*/, 6, 6)
    };
    for (int index = 0; index < 1; ++index)
    {
      Microsoft.Xna.Framework.Rectangle rectangle = rectangleArray[Game1.random.Next(rectangleArray.Length)];
      Vector2 tileLocation = new Vector2((float) Game1.random.Next(rectangle.X, rectangle.Right), (float) Game1.random.Next(rectangle.Y, rectangle.Bottom));
      foreach (Vector2 openTile in Utility.recursiveFindOpenTiles((GameLocation) this, tileLocation, 16 /*0x10*/))
      {
        string str = this.doesTileHaveProperty((int) openTile.X, (int) openTile.Y, "Diggable", "Back", false);
        if (!this.terrainFeatures.ContainsKey(openTile) && str != null && Game1.random.NextDouble() < 1.0 - (double) Vector2.Distance(tileLocation, openTile) * 0.34999999403953552)
        {
          HoeDirt hoeDirt2 = new HoeDirt(0, new Crop(true, "2", (int) openTile.X, (int) openTile.Y, (GameLocation) this));
          hoeDirt2.state.Value = 2;
          this.terrainFeatures.Add(openTile, (TerrainFeature) hoeDirt2);
        }
      }
    }
  }

  private bool isTileOpenForDigSiteStone(int tileX, int tileY)
  {
    return this.doesTileHaveProperty(tileX, tileY, "Diggable", "Back", false) != null && this.doesTileHaveProperty(tileX, tileY, "Diggable", "Back", false) == "T" && this.CanItemBePlacedHere(new Vector2((float) tileX, (float) tileY), ignorePassables: CollisionMask.None);
  }

  public void digSiteUpdate()
  {
    bool flag = false;
    Random daySaveRandom = Utility.CreateDaySaveRandom(78.0);
    Microsoft.Xna.Framework.Rectangle r = new Microsoft.Xna.Framework.Rectangle(4, 47, 22, 20);
    int num1 = 20;
    Vector2[] vector2Array = new Vector2[8]
    {
      new Vector2(18f, 49f),
      new Vector2(15f, 54f),
      new Vector2(21f, 52f),
      new Vector2(18f, 61f),
      new Vector2(23f, 57f),
      new Vector2(9f, 63f),
      new Vector2(7f, 51f),
      new Vector2(7f, 57f)
    };
    if (Utility.getNumObjectsOfIndexWithinRectangle(r, new string[9]
    {
      "(O)816",
      "(O)817",
      "(O)818",
      "(O)819",
      "(O)32",
      "(O)38",
      "(O)40",
      "(O)42",
      "(O)590"
    }, (GameLocation) this) < 60)
    {
      for (int index1 = 0; index1 < num1; ++index1)
      {
        Vector2 positionInThisRectangle = Utility.getRandomPositionInThisRectangle(r, Game1.random);
        Vector2 tileLocation = daySaveRandom.Choose<Vector2>(vector2Array);
        if (this.isTileOpenForDigSiteStone((int) positionInThisRectangle.X, (int) positionInThisRectangle.Y))
        {
          if (!flag || Game1.random.NextDouble() < 0.3)
          {
            flag = true;
            StardewValley.Object @object = ItemRegistry.Create<StardewValley.Object>("(O)" + (816 + Game1.random.Next(2)).ToString());
            @object.MinutesUntilReady = 4;
            this.objects.Add(positionInThisRectangle, @object);
          }
          else if (Game1.random.NextDouble() < 0.1)
          {
            int x = (int) positionInThisRectangle.X;
            int y = (int) positionInThisRectangle.Y;
            if (this.CanItemBePlacedHere(positionInThisRectangle, ignorePassables: CollisionMask.None) && !this.IsTileOccupiedBy(positionInThisRectangle) && !this.hasTileAt(x, y, "AlwaysFront") && !this.hasTileAt(x, y, "Front") && !this.isBehindBush(positionInThisRectangle) && this.doesTileHaveProperty(x, y, "Diggable", "Back", false) != null && this.doesTileHaveProperty(x, y, "Diggable", "Back", false) == "T")
              this.objects.Add(positionInThisRectangle, ItemRegistry.Create<StardewValley.Object>("(O)590"));
          }
          else if (Game1.random.NextDouble() < 0.06)
            this.terrainFeatures.Add(positionInThisRectangle, (TerrainFeature) new Tree("8", 1));
          else if (Game1.random.NextDouble() < 0.2)
          {
            if (this.isTileOpenForDigSiteStone((int) tileLocation.X, (int) tileLocation.Y))
            {
              int num2 = Game1.random.Next(2, 5);
              for (int index2 = 0; index2 < num2; ++index2)
              {
                StardewValley.Object o1 = ItemRegistry.Create<StardewValley.Object>("(O)818");
                o1.MinutesUntilReady = 4;
                Utility.spawnObjectAround(tileLocation, o1, (GameLocation) this, false, (Action<StardewValley.Object>) (o =>
                {
                  o.CanBeGrabbed = false;
                  o.IsSpawnedObject = false;
                }));
              }
            }
          }
          else if (Game1.random.NextDouble() < 0.25)
          {
            this.objects.Add(positionInThisRectangle, new StardewValley.Object(daySaveRandom.Choose<string>("785", "676", "677"), 1));
          }
          else
          {
            string itemId = daySaveRandom.Choose<string>("32", "38", "40", "42");
            this.objects.Add(positionInThisRectangle, new StardewValley.Object(itemId, 1)
            {
              MinutesUntilReady = 2
            });
          }
        }
      }
    }
    else
    {
      if (Utility.getNumObjectsOfIndexWithinRectangle(r, new string[3]
      {
        "(O)785",
        "(O)676",
        "(O)677"
      }, (GameLocation) this) >= 100)
        return;
      int num3 = daySaveRandom.Next(4);
      for (int index = 0; index < num3; ++index)
      {
        Vector2 positionInThisRectangle = Utility.getRandomPositionInThisRectangle(r, Game1.random);
        if (this.isTileOpenForDigSiteStone((int) positionInThisRectangle.X, (int) positionInThisRectangle.Y))
          this.objects.Add(positionInThisRectangle, ItemRegistry.Create<StardewValley.Object>(daySaveRandom.Choose<string>("(O)785", "(O)676", "(O)677")));
      }
    }
  }

  public override bool performOrePanTenMinuteUpdate(Random r)
  {
    Point point;
    if (Game1.MasterPlayer.mailReceived.Contains("ccFishTank"))
    {
      point = this.orePanPoint.Value;
      if (point.Equals(Point.Zero) && r.NextBool())
      {
        for (int index = 0; index < 3; ++index)
        {
          Point tile = new Point(r.Next(4, 15), r.Next(45, 70));
          if (this.isOpenWater(tile.X, tile.Y) && FishingRod.distanceToLand(tile.X, tile.Y, (GameLocation) this) <= 1 && !this.hasTileAt(tile, "Buildings"))
          {
            if (Game1.player.currentLocation.Equals((GameLocation) this))
              this.playSound("slosh");
            this.orePanPoint.Value = tile;
            return true;
          }
        }
        goto label_11;
      }
    }
    point = this.orePanPoint.Value;
    if (!point.Equals(Point.Zero) && r.NextDouble() < 0.2)
      this.orePanPoint.Value = Point.Zero;
label_11:
    return false;
  }

  public override bool performToolAction(Tool t, int tileX, int tileY)
  {
    if (!this.caveOpened.Value && tileY == 47 && (tileX == 21 || tileX == 22))
    {
      this.boulderKnockTimer = 500f;
      Game1.playSound("hammer");
      this.boulderKnocksLeft = 0;
      this.doneHittingBoulderWithToolTimer = 1200f;
    }
    return base.performToolAction(t, tileX, tileY);
  }

  public override void explosionAt(float x, float y)
  {
    base.explosionAt(x, y);
    if (this.caveOpened.Value || (double) y != 47.0 || (double) x != 21.0 && (double) x != 22.0)
      return;
    this.caveOpened.Value = true;
    Game1.addMailForTomorrow("islandNorthCaveOpened", true, true);
  }

  public override void drawBackground(SpriteBatch b)
  {
    base.drawBackground(b);
    this.DrawParallaxHorizon(b);
    if (this.treeNutShot.Value)
      return;
    b.Draw(Game1.objectSpriteSheet, Game1.GlobalToLocal(Game1.viewport, new Vector2(58.25f, 10f) * 64f), new Microsoft.Xna.Framework.Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, 73, 16 /*0x10*/, 16 /*0x10*/)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.1f);
  }

  public override void draw(SpriteBatch b)
  {
    base.draw(b);
    foreach (SuspensionBridge suspensionBridge in this.suspensionBridges)
      suspensionBridge.Draw(b);
    if (this.caveOpened.Value)
      return;
    b.Draw(Game1.mouseCursors2, Game1.GlobalToLocal(Utility.PointToVector2(this.boulderPosition.Location) + new Vector2((double) this.boulderKnockTimer > 250.0 ? (float) Game1.random.Next(-1, 2) : 0.0f, (float) (((double) this.boulderKnockTimer > 250.0 ? Game1.random.Next(-1, 2) : 0) - 64 /*0x40*/))), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(155, 224 /*0xE0*/, 32 /*0x20*/, 32 /*0x20*/)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) this.boulderPosition.Y / 10000f);
  }

  public override void drawAboveAlwaysFrontLayer(SpriteBatch b)
  {
    base.drawAboveAlwaysFrontLayer(b);
    if (this.caveOpened.Value || (double) this.boulderTextTimer <= 0.0)
      return;
    SpriteText.drawStringWithScrollCenteredAt(b, this.boulderTextString, (int) Game1.GlobalToLocal(Utility.PointToVector2(this.boulderPosition.Location)).X + 64 /*0x40*/, (int) Game1.GlobalToLocal(Utility.PointToVector2(this.boulderPosition.Location)).Y - 128 /*0x80*/ - 32 /*0x20*/, "", 1f, new Color?(), 1, 1f, false);
  }

  public override bool IsLocationSpecificPlacementRestriction(Vector2 tileLocation)
  {
    foreach (SuspensionBridge suspensionBridge in this.suspensionBridges)
    {
      if (suspensionBridge.CheckPlacementPrevention(tileLocation))
        return true;
    }
    return base.IsLocationSpecificPlacementRestriction(tileLocation);
  }

  public override void MakeMapModifications(bool force = false)
  {
    base.MakeMapModifications(force);
    if (this.bridgeFixed.Value)
      this.ApplyFixedBridge();
    else
      this.ApplyMapOverride("Island_Bridge_Broken", destination_rect: new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(31 /*0x1F*/, 52, 4, 3)));
    if (!this.traderActivated.Value)
      return;
    this.ApplyIslandTraderHut();
  }

  protected override void resetLocalState()
  {
    base.resetLocalState();
    if (this.traderActivated.Value)
    {
      this.removeTemporarySpritesWithIDLocal(8989);
      this.removeTemporarySpritesWithIDLocal(8988);
      this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(276, 1985, 12, 11), new Vector2(33.45f, 70.33f) * 64f + new Vector2(-16f, -32f), false, 0.0f, Color.White)
      {
        delayBeforeAnimationStart = 10,
        interval = 50f,
        totalNumberOfLoops = 99999,
        animationLength = 4,
        lightId = "IslandNorth_Trader_1",
        id = 8989,
        lightRadius = 2f,
        scale = 4f,
        layerDepth = 0.46144f
      });
      this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(276, 1985, 12, 11), new Vector2(39.45f, 70.33f) * 64f + new Vector2(-16f, -32f), false, 0.0f, Color.White)
      {
        delayBeforeAnimationStart = 10,
        interval = 50f,
        totalNumberOfLoops = 99999,
        animationLength = 4,
        lightId = "IslandNorth_Trader_2",
        id = 8988,
        lightRadius = 2f,
        scale = 4f,
        layerDepth = 0.46144f
      });
    }
    if (this.caveOpened.Value && !Game1.player.hasOrWillReceiveMail("islandNorthCaveOpened"))
      Game1.addMailForTomorrow("islandNorthCaveOpened", true);
    this.suspensionBridges.Clear();
    this.suspensionBridges.Add(new SuspensionBridge(38, 39));
    if (Game1.player.hasOrWillReceiveMail("Saw_Flame_Sprite_North_South"))
      this._sawFlameSpriteSouth = true;
    if (Game1.player.hasOrWillReceiveMail("Saw_Flame_Sprite_North_North"))
      this._sawFlameSpriteNorth = true;
    if (!this._sawFlameSpriteSouth)
    {
      this.temporarySprites.Add(new TemporaryAnimatedSprite("Characters\\Monsters\\Magma Sprite", new Microsoft.Xna.Framework.Rectangle(0, 32 /*0x20*/, 16 /*0x10*/, 16 /*0x10*/), new Vector2(36f, 79f) * 64f, false, 0.0f, Color.White)
      {
        id = 999,
        scale = 4f,
        totalNumberOfLoops = 99999,
        interval = 70f,
        lightId = "IslandNorth_FlameSpirit_South",
        lightRadius = 1f,
        animationLength = 7,
        layerDepth = 1f,
        yPeriodic = true,
        yPeriodicRange = 12f,
        yPeriodicLoopTime = 1000f,
        xPeriodic = true,
        xPeriodicRange = 16f,
        xPeriodicLoopTime = 1800f
      });
      this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\shadow", new Microsoft.Xna.Framework.Rectangle(0, 0, 12, 7), new Vector2(36.2f, 80.4f) * 64f, false, 0.0f, Color.White)
      {
        id = 998,
        scale = 4f,
        totalNumberOfLoops = 99999,
        interval = 1000f,
        animationLength = 1,
        layerDepth = 1f / 1000f,
        yPeriodic = true,
        yPeriodicRange = 1f,
        yPeriodicLoopTime = 1000f,
        xPeriodic = true,
        xPeriodicRange = 16f,
        xPeriodicLoopTime = 1800f
      });
    }
    if (!this._sawFlameSpriteNorth)
    {
      this.temporarySprites.Add(new TemporaryAnimatedSprite("Characters\\Monsters\\Magma Sprite", new Microsoft.Xna.Framework.Rectangle(0, 32 /*0x20*/, 16 /*0x10*/, 16 /*0x10*/), new Vector2(41f, 30f) * 64f, false, 0.0f, Color.White)
      {
        id = 9999,
        scale = 4f,
        totalNumberOfLoops = 99999,
        interval = 70f,
        lightId = "IslandNorth_FlameSpirit_North",
        lightRadius = 1f,
        animationLength = 7,
        layerDepth = 1f,
        yPeriodic = true,
        yPeriodicRange = 12f,
        yPeriodicLoopTime = 1000f,
        xPeriodic = true,
        xPeriodicRange = 16f,
        xPeriodicLoopTime = 1800f
      });
      this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\shadow", new Microsoft.Xna.Framework.Rectangle(0, 0, 12, 7), new Vector2(41.2f, 31.4f) * 64f, false, 0.0f, Color.White)
      {
        id = 9998,
        scale = 4f,
        totalNumberOfLoops = 99999,
        interval = 1000f,
        animationLength = 1,
        layerDepth = 1f / 1000f,
        yPeriodic = true,
        yPeriodicRange = 1f,
        yPeriodicLoopTime = 1000f,
        xPeriodic = true,
        xPeriodicRange = 16f,
        xPeriodicLoopTime = 1800f
      });
    }
    Random random = Utility.CreateRandom((double) Game1.uniqueIDForThisGame, (double) Game1.stats.DaysPlayed, 978.0);
    if (!Game1.player.team.collectedNutTracker.Contains("Buried_IslandNorth_26_81"))
    {
      this.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\critters", new Microsoft.Xna.Framework.Rectangle(141, 310, 20, 23), new Vector2(23.75f, 77.15f) * 64f, false, 0.0f, Color.White)
      {
        totalNumberOfLoops = 999999,
        animationLength = 2,
        interval = 200f,
        id = 79797,
        layerDepth = 1f,
        scale = 4f,
        drawAboveAlwaysFront = true
      });
    }
    else
    {
      if (this.IsRainingHere() || random.NextDouble() >= 0.1)
        return;
      this.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\critters", new Microsoft.Xna.Framework.Rectangle(141, 310, 20, 23), new Vector2(23.75f, 77.15f) * 64f, false, 0.0f, Color.White)
      {
        totalNumberOfLoops = 999999,
        animationLength = 2,
        interval = 200f,
        layerDepth = 1f,
        scale = 4f,
        drawAboveAlwaysFront = true
      });
    }
  }
}
