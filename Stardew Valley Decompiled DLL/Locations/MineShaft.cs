// Decompiled with JetBrains decompiler
// Type: StardewValley.Locations.MineShaft
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Netcode;
using StardewValley.BellsAndWhistles;
using StardewValley.Constants;
using StardewValley.Enchantments;
using StardewValley.Extensions;
using StardewValley.Menus;
using StardewValley.Monsters;
using StardewValley.Network;
using StardewValley.Objects;
using StardewValley.Objects.Trinkets;
using StardewValley.Pathfinding;
using StardewValley.SpecialOrders;
using StardewValley.TerrainFeatures;
using StardewValley.TokenizableStrings;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using xTile;
using xTile.Dimensions;
using xTile.Layers;
using xTile.Tiles;

#nullable disable
namespace StardewValley.Locations;

public class MineShaft : GameLocation
{
  public const int mineFrostLevel = 40;
  public const int mineLavaLevel = 80 /*0x50*/;
  public const int upperArea = 0;
  public const int jungleArea = 10;
  public const int frostArea = 40;
  public const int lavaArea = 80 /*0x50*/;
  public const int desertArea = 121;
  public const int bottomOfMineLevel = 120;
  public const int quarryMineShaft = 77377;
  public const int numberOfLevelsPerArea = 40;
  public const int mineFeature_barrels = 0;
  public const int mineFeature_chests = 1;
  public const int mineFeature_coalCart = 2;
  public const int mineFeature_elevator = 3;
  public const double chanceForColoredGemstone = 0.008;
  public const double chanceForDiamond = 0.0005;
  public const double chanceForPrismaticShard = 0.0005;
  public const int monsterLimit = 30;
  /// <summary>The main tile sheet ID for mine tiles.</summary>
  public const string MineTileSheetId = "mine";
  public static SerializableDictionary<int, MineInfo> permanentMineChanges = new SerializableDictionary<int, MineInfo>();
  public static int numberOfCraftedStairsUsedThisRun;
  public Random mineRandom = new Random();
  private LocalizedContentManager mineLoader = Game1.content.CreateTemporary();
  private int timeUntilElevatorLightUp;
  [XmlIgnore]
  public int loadedMapNumber;
  public int fogTime;
  public NetBool isFogUp = new NetBool();
  public static int timeSinceLastMusic = 200000;
  public bool ladderHasSpawned;
  public bool ghostAdded;
  public bool loadedDarkArea;
  public bool isFallingDownShaft;
  public Vector2 fogPos;
  private readonly NetBool elevatorShouldDing = new NetBool();
  public readonly NetString mapImageSource = new NetString();
  private readonly NetInt netMineLevel = new NetInt();
  private readonly NetIntDelta netStonesLeftOnThisLevel = new NetIntDelta();
  private readonly NetVector2 netTileBeneathLadder = new NetVector2();
  private readonly NetVector2 netTileBeneathElevator = new NetVector2();
  public readonly NetPoint calicoStatueSpot = new NetPoint();
  public readonly NetPoint recentlyActivatedCalicoStatue = new NetPoint();
  private readonly NetPoint netElevatorLightSpot = new NetPoint();
  private readonly NetBool netIsSlimeArea = new NetBool();
  private readonly NetBool netIsMonsterArea = new NetBool();
  private readonly NetBool netIsTreasureRoom = new NetBool();
  private readonly NetBool netIsDinoArea = new NetBool();
  private readonly NetBool netIsQuarryArea = new NetBool();
  private readonly NetBool netAmbientFog = new NetBool();
  private readonly NetColor netLighting = new NetColor(Color.White);
  private readonly NetColor netFogColor = new NetColor();
  private readonly NetVector2Dictionary<bool, NetBool> createLadderAtEvent = new NetVector2Dictionary<bool, NetBool>();
  private readonly NetPointDictionary<bool, NetBool> createLadderDownEvent = new NetPointDictionary<bool, NetBool>();
  private float fogAlpha;
  [XmlIgnore]
  public static ICue bugLevelLoop;
  public readonly NetBool rainbowLights = new NetBool(false);
  public readonly NetBool isLightingDark = new NetBool(false);
  /// <summary>The layout in <c>Maps/Mines</c> to use, or <c>null</c> to choose a random one based on the level.</summary>
  private readonly int? forceLayout;
  private LocalizedContentManager mapContent;
  /// <summary>The mine levels which are currently loaded and ready.</summary>
  /// <remarks>When removing a location from this list, code should call <see cref="M:StardewValley.Locations.MineShaft.OnRemoved" /> since it won't be called automatically.</remarks>
  public static List<MineShaft> activeMines = new List<MineShaft>();
  public static HashSet<int> mushroomLevelsGeneratedToday = new HashSet<int>();
  public static int totalCalicoStatuesActivatedToday;
  private int recentCalicoStatueEffect;
  private bool forceFirstTime;
  private static int deepestLevelOnCurrentDesertFestivalRun;
  private int lastLevelsDownFallen;
  private Microsoft.Xna.Framework.Rectangle fogSource = new Microsoft.Xna.Framework.Rectangle(640, 0, 64 /*0x40*/, 64 /*0x40*/);
  private List<Vector2> brownSpots = new List<Vector2>();
  private int lifespan;
  private bool hasAddedDesertFestivalStatue;
  public float calicoEggIconTimerShake;

  public static int lowestLevelReached
  {
    get
    {
      if (Game1.netWorldState.Value.LowestMineLevelForOrder < 0)
        return Game1.netWorldState.Value.LowestMineLevel;
      return Game1.netWorldState.Value.LowestMineLevelForOrder == 120 ? Math.Max(Game1.netWorldState.Value.LowestMineLevelForOrder, Game1.netWorldState.Value.LowestMineLevelForOrder) : Game1.netWorldState.Value.LowestMineLevelForOrder;
    }
    set
    {
      if (Game1.netWorldState.Value.LowestMineLevelForOrder >= 0 && value <= 120)
      {
        Game1.netWorldState.Value.LowestMineLevelForOrder = value;
      }
      else
      {
        if (!Game1.player.hasSkullKey && value > 120)
          return;
        Game1.netWorldState.Value.LowestMineLevel = value;
      }
    }
  }

  public int mineLevel
  {
    get => this.netMineLevel.Value;
    set => this.netMineLevel.Value = value;
  }

  public int stonesLeftOnThisLevel
  {
    get => this.netStonesLeftOnThisLevel.Value;
    set => this.netStonesLeftOnThisLevel.Value = value;
  }

  public Vector2 tileBeneathLadder
  {
    get => this.netTileBeneathLadder.Value;
    set => this.netTileBeneathLadder.Value = value;
  }

  public Vector2 tileBeneathElevator
  {
    get => this.netTileBeneathElevator.Value;
    set => this.netTileBeneathElevator.Value = value;
  }

  public Point ElevatorLightSpot
  {
    get => this.netElevatorLightSpot.Value;
    set => this.netElevatorLightSpot.Value = value;
  }

  public bool isSlimeArea
  {
    get => this.netIsSlimeArea.Value;
    set => this.netIsSlimeArea.Value = value;
  }

  public bool isDinoArea
  {
    get => this.netIsDinoArea.Value;
    set => this.netIsDinoArea.Value = value;
  }

  public bool isMonsterArea
  {
    get => this.netIsMonsterArea.Value;
    set => this.netIsMonsterArea.Value = value;
  }

  public bool isQuarryArea
  {
    get => this.netIsQuarryArea.Value;
    set => this.netIsQuarryArea.Value = value;
  }

  public bool ambientFog
  {
    get => this.netAmbientFog.Value;
    set => this.netAmbientFog.Value = value;
  }

  public Color lighting
  {
    get => this.netLighting.Value;
    set => this.netLighting.Value = value;
  }

  public Color fogColor
  {
    get => this.netFogColor.Value;
    set => this.netFogColor.Value = value;
  }

  public MineShaft()
    : this(0)
  {
  }

  /// <summary>Construct an instance.</summary>
  /// <param name="level">The mine level.</param>
  /// <param name="forceLayout">The layout in <c>Maps/Mines</c> to use, or <c>null</c> to choose a random one based on the level.</param>
  public MineShaft(int level, int? forceLayout = null)
  {
    this.mineLevel = level;
    this.name.Value = MineShaft.GetLevelName(level);
    this.mapContent = Game1.game1.xTileContent.CreateTemporary();
    this.forceLayout = forceLayout;
    if (Game1.IsMultiplayer || this.getMineArea() != 121)
      return;
    this.ExtraMillisecondsPerInGameMinute = 200;
  }

  public override string GetLocationContextId()
  {
    if (this.locationContextId == null)
      this.locationContextId = this.mineLevel >= 121 ? "Desert" : "Default";
    return base.GetLocationContextId();
  }

  public override bool CanPlaceThisFurnitureHere(Furniture furniture) => false;

  protected override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.netMineLevel, "netMineLevel").AddField((INetSerializable) this.netStonesLeftOnThisLevel, "netStonesLeftOnThisLevel").AddField((INetSerializable) this.netTileBeneathLadder, "netTileBeneathLadder").AddField((INetSerializable) this.netTileBeneathElevator, "netTileBeneathElevator").AddField((INetSerializable) this.netElevatorLightSpot, "netElevatorLightSpot").AddField((INetSerializable) this.netIsSlimeArea, "netIsSlimeArea").AddField((INetSerializable) this.netIsMonsterArea, "netIsMonsterArea").AddField((INetSerializable) this.netIsTreasureRoom, "netIsTreasureRoom").AddField((INetSerializable) this.netIsDinoArea, "netIsDinoArea").AddField((INetSerializable) this.netIsQuarryArea, "netIsQuarryArea").AddField((INetSerializable) this.netAmbientFog, "netAmbientFog").AddField((INetSerializable) this.netLighting, "netLighting").AddField((INetSerializable) this.netFogColor, "netFogColor").AddField((INetSerializable) this.createLadderAtEvent, "createLadderAtEvent").AddField((INetSerializable) this.createLadderDownEvent, "createLadderDownEvent").AddField((INetSerializable) this.mapImageSource, "mapImageSource").AddField((INetSerializable) this.rainbowLights, "rainbowLights").AddField((INetSerializable) this.isLightingDark, "isLightingDark").AddField((INetSerializable) this.elevatorShouldDing, "elevatorShouldDing").AddField((INetSerializable) this.isFogUp, "isFogUp").AddField((INetSerializable) this.calicoStatueSpot, "calicoStatueSpot").AddField((INetSerializable) this.recentlyActivatedCalicoStatue, "recentlyActivatedCalicoStatue");
    this.isFogUp.fieldChangeEvent += (FieldChange<NetBool, bool>) ((field, oldValue, newValue) =>
    {
      if (!oldValue & newValue)
      {
        if (Game1.currentLocation == this)
          Game1.changeMusicTrack("none");
        if (!Game1.IsClient)
          return;
        this.fogTime = 35000;
      }
      else
      {
        if (newValue)
          return;
        this.fogTime = 0;
      }
    });
    this.createLadderAtEvent.OnValueAdded += (NetDictionary<Vector2, bool, NetBool, SerializableDictionary<Vector2, bool>, NetVector2Dictionary<bool, NetBool>>.ContentsChangeEvent) ((v, b) => this.doCreateLadderAt(v));
    this.createLadderDownEvent.OnValueAdded += new NetDictionary<Point, bool, NetBool, SerializableDictionary<Point, bool>, NetPointDictionary<bool, NetBool>>.ContentsChangeEvent(this.doCreateLadderDown);
    this.mapImageSource.fieldChangeEvent += (FieldChange<NetString, string>) ((field, oldValue, newValue) =>
    {
      if (newValue == null || !(newValue != oldValue))
        return;
      this.Map.RequireTileSheet(0, "mine").ImageSource = newValue;
      this.Map.LoadTileSheets(Game1.mapDisplayDevice);
    });
    this.recentlyActivatedCalicoStatue.fieldChangeEvent += new FieldChange<NetPoint, Point>(this.calicoStatueActivated);
  }

  public void calicoStatueActivated(NetPoint field, Point oldVector, Point newVector)
  {
    if (newVector == Point.Zero)
      return;
    if (Game1.currentLocation != null && Game1.currentLocation.Equals((GameLocation) this))
    {
      Game1.playSound("openBox");
      this.temporarySprites.AddRange((IEnumerable<TemporaryAnimatedSprite>) Utility.sparkleWithinArea(new Microsoft.Xna.Framework.Rectangle((newVector.X - 1) * 64 /*0x40*/, (newVector.Y - 3) * 64 /*0x40*/, 192 /*0xC0*/, 192 /*0xC0*/), 20, Color.White, 50, 500));
      this.calicoEggIconTimerShake = 1500f;
      this.setMapTile(newVector.X, newVector.Y, 285, "Buildings", "mine");
      this.setMapTile(newVector.X, newVector.Y - 1, 269, "Front", "mine");
      this.setMapTile(newVector.X, newVector.Y - 2, 253, "Front", "mine");
      this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors_1_6", new Microsoft.Xna.Framework.Rectangle(20, 0, 19, 21), new Vector2((float) (newVector.X * 64 /*0x40*/ - 4), (float) (((double) newVector.Y - 2.5) * 64.0)), false, 0.0f, Color.White)
      {
        motion = new Vector2(0.0f, -1f),
        yStopCoordinate = (int) (((double) newVector.Y - 3.25) * 64.0),
        scale = 4f,
        animationLength = 1,
        delayBeforeAnimationStart = 1500,
        totalNumberOfLoops = 10,
        interval = 300f,
        drawAboveAlwaysFront = true
      });
    }
    if (!Game1.IsMasterGame)
      return;
    ++Game1.player.team.calicoEggSkullCavernRating.Value;
    ++MineShaft.totalCalicoStatuesActivatedToday;
    Random daySaveRandom = Utility.CreateDaySaveRandom((double) MineShaft.totalCalicoStatuesActivatedToday);
    if (daySaveRandom.NextBool(0.51 + Game1.player.team.AverageDailyLuck((GameLocation) this)))
    {
      if (this.tryToAddCalicoStatueEffect(daySaveRandom, 0.15, 10) || this.tryToAddCalicoStatueEffect(daySaveRandom, 0.01, 17, true) || this.tryToAddCalicoStatueEffect(daySaveRandom, 0.05, 12, true) || this.tryToAddCalicoStatueEffect(daySaveRandom, 0.1, 15, true) || this.tryToAddCalicoStatueEffect(daySaveRandom, 0.2, 16 /*0x10*/, true) || this.tryToAddCalicoStatueEffect(daySaveRandom, 0.1, 14, true) || this.tryToAddCalicoStatueEffect(daySaveRandom, 0.5, 11, true))
        return;
      Game1.player.team.AddCalicoStatueEffect(13);
      this.signalCalicoStatueActivation(13);
    }
    else
    {
      if (daySaveRandom.NextBool(0.2))
      {
        for (int index = 0; index < 30; ++index)
        {
          int num = daySaveRandom.Next(4);
          if (!Game1.player.team.calicoStatueEffects.ContainsKey(num))
          {
            Game1.player.team.AddCalicoStatueEffect(num);
            this.signalCalicoStatueActivation(num);
            return;
          }
        }
      }
      if (this.tryToAddCalicoStatueEffect(daySaveRandom, 0.1, 4) || this.tryToAddCalicoStatueEffect(daySaveRandom, 0.1, 9) || this.tryToAddCalicoStatueEffect(daySaveRandom, 0.1, 5) || this.tryToAddCalicoStatueEffect(daySaveRandom, 0.1, 6) || this.tryToAddCalicoStatueEffect(daySaveRandom, 0.2, 7, true) || this.tryToAddCalicoStatueEffect(daySaveRandom, 0.2, 8, true))
        return;
      Game1.player.team.AddCalicoStatueEffect(13);
      this.signalCalicoStatueActivation(13);
    }
  }

  private void signalCalicoStatueActivation(int whichEffect)
  {
    this.recentCalicoStatueEffect = whichEffect;
    if (!Game1.IsMultiplayer)
      return;
    Game1.multiplayer.globalChatInfoMessage("CalicoStatue_Activated", TokenStringBuilder.LocalizedText("Strings\\1_6_Strings:DF_Mine_CalicoStatue_Description_" + whichEffect.ToString()));
  }

  private bool tryToAddCalicoStatueEffect(Random r, double chance, int which, bool effectCanStack = false)
  {
    if (!r.NextBool(chance) || !effectCanStack && Game1.player.team.calicoStatueEffects.ContainsKey(which))
      return false;
    Game1.player.team.AddCalicoStatueEffect(which);
    this.signalCalicoStatueActivation(which);
    return true;
  }

  public override bool AllowMapModificationsInResetState() => true;

  protected override LocalizedContentManager getMapLoader() => this.mapContent;

  private void setElevatorLit()
  {
    if (this.ElevatorLightSpot.X == -1 || this.ElevatorLightSpot.Y == -1)
      return;
    this.setMapTile(this.ElevatorLightSpot.X, this.ElevatorLightSpot.Y, 48 /*0x30*/, "Buildings", "mine");
    Game1.currentLightSources.Add(new LightSource($"Mine_{this.mineLevel}_Elevator", 4, new Vector2((float) this.ElevatorLightSpot.X, (float) this.ElevatorLightSpot.Y) * 64f, 2f, Color.Black, onlyLocation: this.NameOrUniqueName));
    this.elevatorShouldDing.Value = false;
  }

  public override void UpdateWhenCurrentLocation(GameTime time)
  {
    int num1 = Game1.currentLocation == this ? 1 : 0;
    int? pitch1;
    if ((Game1.isMusicContextActiveButNotPlaying() || Game1.getMusicTrackName().Contains("Ambient")) && Game1.random.NextDouble() < 0.00195)
    {
      Vector2? position = new Vector2?();
      pitch1 = new int?();
      int? pitch2 = pitch1;
      this.localSound("cavedrip", position, pitch2);
    }
    if (this.timeUntilElevatorLightUp > 0)
    {
      this.timeUntilElevatorLightUp -= time.ElapsedGameTime.Milliseconds;
      if (this.timeUntilElevatorLightUp <= 0)
      {
        pitch1 = new int?(0);
        this.localSound("crystal", pitch: pitch1);
        this.setElevatorLit();
      }
    }
    if ((double) this.calicoEggIconTimerShake > 0.0)
      this.calicoEggIconTimerShake -= (float) time.ElapsedGameTime.TotalMilliseconds;
    if (num1 != 0)
    {
      if (this.isFogUp.Value && Game1.shouldTimePass())
      {
        if (MineShaft.bugLevelLoop == null || MineShaft.bugLevelLoop.IsStopped)
          Game1.playSound("bugLevelLoop", out MineShaft.bugLevelLoop);
        if ((double) this.fogAlpha < 1.0)
        {
          if (Game1.shouldTimePass())
            this.fogAlpha += 0.01f;
          if (MineShaft.bugLevelLoop != null)
          {
            MineShaft.bugLevelLoop.SetVariable("Volume", this.fogAlpha * 100f);
            MineShaft.bugLevelLoop.SetVariable("Frequency", this.fogAlpha * 25f);
          }
        }
        else if (MineShaft.bugLevelLoop != null)
        {
          float num2 = (float) Math.Max(0.0, Math.Min(100.0, Math.Sin((double) this.fogTime / 10000.0 % (200.0 * Math.PI))));
          MineShaft.bugLevelLoop.SetVariable("Frequency", Math.Max(0.0f, Math.Min(100f, (float) ((double) this.fogAlpha * 25.0 + (double) num2 * 10.0))));
        }
      }
      else if ((double) this.fogAlpha > 0.0)
      {
        if (Game1.shouldTimePass())
          this.fogAlpha -= 0.01f;
        if (MineShaft.bugLevelLoop != null)
        {
          MineShaft.bugLevelLoop.SetVariable("Volume", this.fogAlpha * 100f);
          MineShaft.bugLevelLoop.SetVariable("Frequency", Math.Max(0.0f, MineShaft.bugLevelLoop.GetVariable("Frequency") - 0.01f));
          if ((double) this.fogAlpha <= 0.0)
          {
            MineShaft.bugLevelLoop.Stop(AudioStopOptions.Immediate);
            MineShaft.bugLevelLoop = (ICue) null;
          }
        }
      }
      if ((double) this.fogAlpha > 0.0 || this.ambientFog)
      {
        this.fogPos = Game1.updateFloatingObjectPositionForMovement(this.fogPos, new Vector2((float) Game1.viewport.X, (float) Game1.viewport.Y), Game1.previousViewportPosition, -1f);
        this.fogPos.X = (float) (((double) this.fogPos.X + 0.5) % 256.0);
        this.fogPos.Y = (float) (((double) this.fogPos.Y + 0.5) % 256.0);
      }
    }
    base.UpdateWhenCurrentLocation(time);
  }

  public override void cleanupBeforePlayerExit()
  {
    base.cleanupBeforePlayerExit();
    if (MineShaft.bugLevelLoop != null)
    {
      MineShaft.bugLevelLoop.Stop(AudioStopOptions.Immediate);
      MineShaft.bugLevelLoop = (ICue) null;
    }
    if (Game1.IsMultiplayer || this.mineLevel != 20)
      return;
    Game1.changeMusicTrack("none");
  }

  public Vector2 mineEntrancePosition(Farmer who)
  {
    return !who.ridingMineElevator || this.tileBeneathElevator.Equals(Vector2.Zero) ? this.tileBeneathLadder : this.tileBeneathElevator;
  }

  private void generateContents()
  {
    this.ladderHasSpawned = false;
    this.loadLevel(this.mineLevel);
    this.chooseLevelType();
    this.findLadder();
    this.populateLevel();
  }

  public void chooseLevelType()
  {
    this.fogTime = 0;
    if (MineShaft.bugLevelLoop != null)
    {
      MineShaft.bugLevelLoop.Stop(AudioStopOptions.Immediate);
      MineShaft.bugLevelLoop = (ICue) null;
    }
    this.ambientFog = false;
    this.rainbowLights.Value = false;
    this.isLightingDark.Value = false;
    Random daySaveRandom = Utility.CreateDaySaveRandom((double) Game1.stats.DaysPlayed, (double) this.mineLevel, (double) (4 * this.mineLevel));
    this.lighting = new Color(80 /*0x50*/, 80 /*0x50*/, 40);
    if (this.getMineArea() == 80 /*0x50*/)
      this.lighting = new Color(100, 100, 50);
    if (this.GetAdditionalDifficulty() > 0)
    {
      if (this.getMineArea() == 40)
      {
        this.lighting = new Color(230, 200, 90);
        this.ambientFog = true;
        this.fogColor = new Color(0, 80 /*0x50*/, (int) byte.MaxValue) * 0.55f;
        if (this.mineLevel < 50)
        {
          this.lighting = new Color(100, 80 /*0x50*/, 40);
          this.ambientFog = false;
        }
      }
    }
    else if (daySaveRandom.NextDouble() < 0.3 && this.mineLevel > 2)
    {
      this.isLightingDark.Value = true;
      this.lighting = new Color(120, 120, 40);
      if (daySaveRandom.NextDouble() < 0.3)
        this.lighting = new Color(150, 150, 60);
    }
    if (daySaveRandom.NextDouble() < 0.15 && this.mineLevel > 5 && this.mineLevel != 120)
    {
      this.isLightingDark.Value = true;
      switch (this.getMineArea())
      {
        case 0:
        case 10:
          this.lighting = new Color(110, 110, 70);
          break;
        case 40:
          this.lighting = Color.Black;
          if (this.GetAdditionalDifficulty() > 0)
          {
            this.lighting = new Color(237, 212, 185);
            break;
          }
          break;
        case 80 /*0x50*/:
          this.lighting = new Color(90, 130, 70);
          break;
      }
    }
    if (daySaveRandom.NextDouble() < 0.035 && this.getMineArea() == 80 /*0x50*/ && this.mineLevel % 5 != 0 && !MineShaft.mushroomLevelsGeneratedToday.Contains(this.mineLevel))
    {
      this.rainbowLights.Value = true;
      MineShaft.mushroomLevelsGeneratedToday.Add(this.mineLevel);
    }
    if (this.isDarkArea() && this.mineLevel < 120)
    {
      this.isLightingDark.Value = true;
      this.lighting = this.getMineArea() == 80 /*0x50*/ ? new Color(70, 100, 100) : new Color(150, 150, 120);
      if (this.getMineArea() == 0)
      {
        this.ambientFog = true;
        this.fogColor = Color.Black;
      }
    }
    if (this.mineLevel == 100)
      this.lighting = new Color(140, 140, 80 /*0x50*/);
    if (this.getMineArea() == 121)
    {
      this.lighting = new Color(110, 110, 40);
      if (daySaveRandom.NextDouble() < 0.05)
        this.lighting = daySaveRandom.NextBool() ? new Color(30, 30, 0) : new Color(150, 150, 50);
    }
    if (this.getMineArea() != 77377)
      return;
    this.isLightingDark.Value = false;
    this.rainbowLights.Value = false;
    this.ambientFog = true;
    this.fogColor = Color.White * 0.4f;
    this.lighting = new Color(80 /*0x50*/, 80 /*0x50*/, 30);
  }

  public static void yearUpdate()
  {
    MineShaft.permanentMineChanges.RemoveWhere<int, MineInfo>((Func<KeyValuePair<int, MineInfo>, bool>) (p => p.Key > 120 || p.Key % 5 != 0));
    MineInfo mineInfo;
    if (MineShaft.permanentMineChanges.TryGetValue(5, out mineInfo))
      mineInfo.platformContainersLeft = 6;
    if (MineShaft.permanentMineChanges.TryGetValue(45, out mineInfo))
      mineInfo.platformContainersLeft = 6;
    if (!MineShaft.permanentMineChanges.TryGetValue(85, out mineInfo))
      return;
    mineInfo.platformContainersLeft = 6;
  }

  private bool canAdd(int typeOfFeature, int numberSoFar)
  {
    MineInfo mineInfo;
    if (MineShaft.permanentMineChanges.TryGetValue(this.mineLevel, out mineInfo))
    {
      switch (typeOfFeature)
      {
        case 0:
          return mineInfo.platformContainersLeft > numberSoFar;
        case 1:
          return mineInfo.chestsLeft > numberSoFar;
        case 2:
          return mineInfo.coalCartsLeft > numberSoFar;
        case 3:
          return mineInfo.elevator == 0;
      }
    }
    return true;
  }

  public void updateMineLevelData(int feature, int amount = 1)
  {
    MineInfo mineInfo;
    if (!MineShaft.permanentMineChanges.TryGetValue(this.mineLevel, out mineInfo))
    {
      MineShaft.permanentMineChanges[this.mineLevel] = mineInfo = new MineInfo();
      if (this.mineLevel == 5 || this.mineLevel == 45 || this.mineLevel == 85)
        this.forceFirstTime = true;
    }
    switch (feature)
    {
      case 0:
        mineInfo.platformContainersLeft += amount;
        break;
      case 1:
        mineInfo.chestsLeft += amount;
        break;
      case 2:
        mineInfo.coalCartsLeft += amount;
        break;
      case 3:
        mineInfo.elevator += amount;
        break;
    }
  }

  public void chestConsumed() => Game1.player.chestConsumedMineLevels[this.mineLevel] = true;

  public bool isLevelSlimeArea() => this.isSlimeArea;

  public void checkForMapAlterations(int x, int y)
  {
    if (this.getTileIndexAt(x, y, "Buildings", "mine") != 194 || this.canAdd(2, 0))
      return;
    this.setMapTile(x, y, 195, "Buildings", "mine");
    this.setMapTile(x, y - 1, 179, "Front", "mine");
  }

  public void findLadder()
  {
    int num = 0;
    this.tileBeneathElevator = Vector2.Zero;
    bool flag = this.mineLevel % 20 == 0;
    this.lightGlows.Clear();
    Layer layer = this.map.RequireLayer("Buildings");
    for (int index1 = 0; index1 < layer.LayerHeight; ++index1)
    {
      for (int index2 = 0; index2 < layer.LayerWidth; ++index2)
      {
        int tileIndexAt = layer.GetTileIndexAt(index2, index1, "mine");
        switch (tileIndexAt)
        {
          case -1:
label_13:
            if (Game1.IsMasterGame && this.isWaterTile(index2, index1) && this.getMineArea() == 80 /*0x50*/ && Game1.random.NextDouble() < 0.1)
            {
              this.sharedLights.AddLight(new LightSource($"Mines_{this.mineLevel}_{index2}_{index1}_Lava", 4, new Vector2((float) index2, (float) index1) * 64f, 2f, new Color(0, 220, 220), onlyLocation: this.NameOrUniqueName));
              continue;
            }
            continue;
          case 112 /*0x70*/:
            this.tileBeneathElevator = new Vector2((float) index2, (float) (index1 + 1));
            ++num;
            goto default;
          case 115:
            string str = $"Mines_{this.mineLevel}_{index2}_{index1}";
            this.tileBeneathLadder = new Vector2((float) index2, (float) (index1 + 1));
            this.sharedLights.AddLight(new LightSource(str + "_1", 4, new Vector2((float) index2, (float) (index1 - 2)) * 64f + new Vector2(32f, 0.0f), 0.25f, new Color(0, 20, 50), onlyLocation: this.NameOrUniqueName));
            this.sharedLights.AddLight(new LightSource(str + "_2", 4, new Vector2((float) index2, (float) (index1 - 1)) * 64f + new Vector2(32f, 0.0f), 0.5f, new Color(0, 20, 50), onlyLocation: this.NameOrUniqueName));
            this.sharedLights.AddLight(new LightSource(str + "_3", 4, new Vector2((float) index2, (float) index1) * 64f + new Vector2(32f, 0.0f), 0.75f, new Color(0, 20, 50), onlyLocation: this.NameOrUniqueName));
            this.sharedLights.AddLight(new LightSource(str + "_4", 4, new Vector2((float) index2, (float) (index1 + 1)) * 64f + new Vector2(32f, 0.0f), 1f, new Color(0, 20, 50), onlyLocation: this.NameOrUniqueName));
            ++num;
            goto default;
          default:
            if (this.lighting.Equals(Color.White) && num == 2 && !flag)
              return;
            if (!this.lighting.Equals(Color.White))
            {
              switch (tileIndexAt)
              {
                case 48 /*0x30*/:
                case 65:
                case 66:
                case 81:
                case 82:
                case 97:
                case 113:
                  this.sharedLights.AddLight(new LightSource($"Mines_{this.mineLevel}_{index2}_{index1}_5", 4, new Vector2((float) index2, (float) index1) * 64f, 2.5f, new Color(0, 50, 100), onlyLocation: this.NameOrUniqueName));
                  if (tileIndexAt == 66)
                  {
                    this.lightGlows.Add(new Vector2((float) index2, (float) index1) * 64f + new Vector2(0.0f, 64f));
                    goto label_13;
                  }
                  if (tileIndexAt == 97 || tileIndexAt == 113)
                  {
                    this.lightGlows.Add(new Vector2((float) index2, (float) index1) * 64f + new Vector2(32f, 32f));
                    goto label_13;
                  }
                  goto label_13;
                default:
                  goto label_13;
              }
            }
            else
              goto case -1;
        }
      }
    }
    if (this.isFallingDownShaft)
    {
      Vector2 v = new Vector2();
      while (!this.isTileClearForMineObjects(v))
      {
        v.X = (float) Game1.random.Next(1, this.map.Layers[0].LayerWidth);
        v.Y = (float) Game1.random.Next(1, this.map.Layers[0].LayerHeight);
      }
      this.tileBeneathLadder = v;
      Game1.player.showFrame(5);
    }
    this.isFallingDownShaft = false;
  }

  public int EnemyCount => this.characters.Count<NPC>((Func<NPC, bool>) (p => p is Monster));

  public override void performTenMinuteUpdate(int timeOfDay)
  {
    base.performTenMinuteUpdate(timeOfDay);
    if (this.mustKillAllMonstersToAdvance() && this.EnemyCount == 0)
    {
      Vector2 p = new Vector2((float) (int) this.tileBeneathLadder.X, (float) (int) this.tileBeneathLadder.Y);
      if (!this.hasTileAt((int) p.X, (int) p.Y, "Buildings"))
      {
        this.createLadderAt(p, "newArtifact");
        if (this.mustKillAllMonstersToAdvance() && Game1.player.currentLocation == this)
          Game1.showGlobalMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:MineShaft.cs.9484"));
      }
    }
    if (this.isFogUp.Value || this.map == null || this.mineLevel % 5 == 0 || Game1.random.NextDouble() >= 0.1 || this.AnyOnlineFarmerHasBuff("23"))
      return;
    if (this.mineLevel > 10 && !this.mustKillAllMonstersToAdvance() && Game1.random.NextDouble() < 0.11 && this.getMineArea() != 77377)
    {
      this.isFogUp.Value = true;
      this.fogTime = 35000 + Game1.random.Next(-5, 6) * 1000;
      switch (this.getMineArea())
      {
        case 0:
        case 10:
          if (this.GetAdditionalDifficulty() > 0)
          {
            this.fogColor = this.isDarkArea() ? new Color((int) byte.MaxValue, 150, 0) : Color.Cyan * 0.75f;
            break;
          }
          this.fogColor = this.isDarkArea() ? Color.Khaki : Color.Green * 0.75f;
          break;
        case 40:
          this.fogColor = Color.Blue * 0.75f;
          break;
        case 80 /*0x50*/:
          this.fogColor = Color.Red * 0.5f;
          break;
        case 121:
          this.fogColor = Color.BlueViolet * 1f;
          break;
      }
    }
    else
      this.spawnFlyingMonsterOffScreen();
  }

  public void spawnFlyingMonsterOffScreen()
  {
    Vector2 zero = Vector2.Zero;
    switch (Game1.random.Next(4))
    {
      case 0:
        zero.X = (float) Game1.random.Next(this.map.Layers[0].LayerWidth);
        break;
      case 1:
        zero.X = (float) (this.map.Layers[0].LayerWidth - 1);
        zero.Y = (float) Game1.random.Next(this.map.Layers[0].LayerHeight);
        break;
      case 2:
        zero.Y = (float) (this.map.Layers[0].LayerHeight - 1);
        zero.X = (float) Game1.random.Next(this.map.Layers[0].LayerWidth);
        break;
      case 3:
        zero.Y = (float) Game1.random.Next(this.map.Layers[0].LayerHeight);
        break;
    }
    if (Utility.isOnScreen(zero * 64f, 64 /*0x40*/))
      zero.X -= (float) (Game1.viewport.Width / 64 /*0x40*/);
    switch (this.getMineArea())
    {
      case 0:
        if (this.mineLevel <= 10 || !this.isDarkArea())
          break;
        NetCollection<NPC> characters1 = this.characters;
        Bat bat1 = new Bat(zero * 64f, this.mineLevel);
        bat1.focusedOnFarmers = true;
        Monster monster1 = this.BuffMonsterIfNecessary((Monster) bat1);
        characters1.Add((NPC) monster1);
        this.playSound("batScreech");
        break;
      case 10:
        if (this.GetAdditionalDifficulty() > 0)
        {
          NetCollection<NPC> characters2 = this.characters;
          BlueSquid blueSquid = new BlueSquid(zero * 64f);
          blueSquid.focusedOnFarmers = true;
          Monster monster2 = this.BuffMonsterIfNecessary((Monster) blueSquid);
          characters2.Add((NPC) monster2);
          break;
        }
        NetCollection<NPC> characters3 = this.characters;
        Fly fly = new Fly(zero * 64f);
        fly.focusedOnFarmers = true;
        Monster monster3 = this.BuffMonsterIfNecessary((Monster) fly);
        characters3.Add((NPC) monster3);
        break;
      case 40:
        NetCollection<NPC> characters4 = this.characters;
        Bat bat2 = new Bat(zero * 64f, this.mineLevel);
        bat2.focusedOnFarmers = true;
        Monster monster4 = this.BuffMonsterIfNecessary((Monster) bat2);
        characters4.Add((NPC) monster4);
        this.playSound("batScreech");
        break;
      case 80 /*0x50*/:
        NetCollection<NPC> characters5 = this.characters;
        Bat bat3 = new Bat(zero * 64f, this.mineLevel);
        bat3.focusedOnFarmers = true;
        Monster monster5 = this.BuffMonsterIfNecessary((Monster) bat3);
        characters5.Add((NPC) monster5);
        this.playSound("batScreech");
        break;
      case 121:
        if (this.mineLevel < 171 || Game1.random.NextBool())
        {
          NetCollection<NPC> characters6 = this.characters;
          Serpent serpent1;
          if (this.GetAdditionalDifficulty() <= 0)
          {
            Serpent serpent2 = new Serpent(zero * 64f);
            serpent2.focusedOnFarmers = true;
            serpent1 = serpent2;
          }
          else
          {
            serpent1 = new Serpent(zero * 64f, "Royal Serpent");
            serpent1.focusedOnFarmers = true;
          }
          Monster monster6 = this.BuffMonsterIfNecessary((Monster) serpent1);
          characters6.Add((NPC) monster6);
          this.playSound("serpentDie");
          break;
        }
        NetCollection<NPC> characters7 = this.characters;
        Bat bat4 = new Bat(zero * 64f, this.mineLevel);
        bat4.focusedOnFarmers = true;
        Monster monster7 = this.BuffMonsterIfNecessary((Monster) bat4);
        characters7.Add((NPC) monster7);
        this.playSound("batScreech");
        break;
      case 77377:
        NetCollection<NPC> characters8 = this.characters;
        Bat bat5 = new Bat(zero * 64f, 77377);
        bat5.focusedOnFarmers = true;
        characters8.Add((NPC) bat5);
        this.playSound("rockGolemHit");
        break;
    }
  }

  public override void drawLightGlows(SpriteBatch b)
  {
    Color color;
    switch (this.getMineArea())
    {
      case 0:
        color = this.isDarkArea() ? Color.PaleGoldenrod * 0.5f : Color.PaleGoldenrod * 0.33f;
        break;
      case 40:
        color = Color.White * 0.65f;
        if (this.GetAdditionalDifficulty() > 0)
        {
          color = this.mineLevel % 40 >= 30 ? new Color(220, 240 /*0xF0*/, (int) byte.MaxValue) * 0.8f : new Color(230, 225, 100) * 0.8f;
          break;
        }
        break;
      case 80 /*0x50*/:
        color = this.isDarkArea() ? Color.Pink * 0.4f : Color.Red * 0.33f;
        break;
      case 121:
        color = Color.White * 0.8f;
        if (this.isDinoArea)
        {
          color = Color.Orange * 0.5f;
          break;
        }
        break;
      default:
        color = Color.PaleGoldenrod * 0.33f;
        break;
    }
    foreach (Vector2 lightGlow in (NetHashSet<Vector2>) this.lightGlows)
    {
      if (this.rainbowLights.Value)
      {
        switch ((int) ((double) lightGlow.X / 64.0 + (double) lightGlow.Y / 64.0) % 4)
        {
          case 0:
            color = Color.Red * 0.5f;
            break;
          case 1:
            color = Color.Yellow * 0.5f;
            break;
          case 2:
            color = Color.Cyan * 0.33f;
            break;
          case 3:
            color = Color.Lime * 0.45f;
            break;
        }
      }
      b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, lightGlow), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(88, 1779, 30, 30)), color, 0.0f, new Vector2(15f, 15f), (float) (8.0 + 96.0 * Math.Sin((Game1.currentGameTime.TotalGameTime.TotalMilliseconds + (double) lightGlow.X * 777.0 + (double) lightGlow.Y * 9746.0) % 3140.0 / 1000.0) / 50.0), SpriteEffects.None, 1f);
    }
  }

  public Monster BuffMonsterIfNecessary(Monster monster)
  {
    if (monster != null && monster.GetBaseDifficultyLevel() < this.GetAdditionalDifficulty())
    {
      monster.BuffForAdditionalDifficulty(this.GetAdditionalDifficulty() - monster.GetBaseDifficultyLevel());
      if (monster is GreenSlime greenSlime)
      {
        if (this.mineLevel < 40)
          greenSlime.color.Value = new Color(Game1.random.Next(40, 70), Game1.random.Next(100, 190), (int) byte.MaxValue);
        else if (this.mineLevel < 80 /*0x50*/)
          greenSlime.color.Value = new Color(0, 180, 120);
        else if (this.mineLevel < 120)
          greenSlime.color.Value = new Color(Game1.random.Next(180, 250), 20, 120);
        else
          greenSlime.color.Value = new Color(Game1.random.Next(120, 180), 20, (int) byte.MaxValue);
      }
      this.setMonsterTextureToDangerousVersion(monster);
    }
    return monster;
  }

  private void setMonsterTextureToDangerousVersion(Monster monster)
  {
    string str = monster.Sprite.textureName.Value + "_dangerous";
    if (!Game1.content.DoesAssetExist<Texture2D>(str))
      return;
    try
    {
      monster.Sprite.LoadTexture(str);
    }
    catch (Exception ex)
    {
      Game1.log.Error($"Failed loading '{str}' texture for dangerous {monster.Name}.", ex);
    }
  }

  public override Item getFish(
    float millisecondsAfterNibble,
    string bait,
    int waterDepth,
    Farmer who,
    double baitPotency,
    Vector2 bobberTile,
    string locationName = null)
  {
    if (who?.CurrentTool is FishingRod currentTool1 && currentTool1.QualifiedItemId.Contains("TrainingRod"))
      return ItemRegistry.Create("(O)" + Game1.random.Next(167, 173).ToString());
    string itemId = (string) null;
    double num1 = 1.0 + 0.4 * (double) who.FishingLevel + (double) waterDepth * 0.1;
    string str = "";
    if (who?.CurrentTool is FishingRod currentTool2)
    {
      if (currentTool2.HasCuriosityLure())
        num1 += 5.0;
      str = currentTool2.GetBait()?.Name ?? "";
    }
    switch (this.getMineArea())
    {
      case 0:
      case 10:
        double num2 = num1 + (str.Contains("Stonefish") ? 10.0 : 0.0);
        if (Game1.random.NextDouble() < 0.02 + 0.01 * num2)
        {
          itemId = "(O)158";
          break;
        }
        break;
      case 40:
        double num3 = num1 + (str.Contains("Ice Pip") ? 10.0 : 0.0);
        if (Game1.random.NextDouble() < 0.015 + 0.009 * num3)
        {
          itemId = "(O)161";
          break;
        }
        break;
      case 80 /*0x50*/:
        double num4 = num1 + (str.Contains("Lava Eel") ? 10.0 : 0.0);
        if (Game1.random.NextDouble() < 0.01 + 0.008 * num4)
        {
          itemId = "(O)162";
          break;
        }
        break;
    }
    int quality = 0;
    if (Game1.random.NextDouble() < (double) who.FishingLevel / 10.0)
      quality = 1;
    if (Game1.random.NextDouble() < (double) who.FishingLevel / 50.0 + (double) who.LuckLevel / 100.0)
      quality = 2;
    if (itemId != null)
      return ItemRegistry.Create(itemId, quality: quality);
    if (this.getMineArea() != 80 /*0x50*/)
      return base.getFish(millisecondsAfterNibble, bait, waterDepth, who, baitPotency, bobberTile, "UndergroundMine");
    return Game1.random.NextDouble() < 0.05 + (double) who.LuckLevel * 0.05 ? ItemRegistry.Create("(O)CaveJelly") : ItemRegistry.Create("(O)" + Game1.random.Next(167, 173).ToString());
  }

  private void adjustLevelChances(
    ref double stoneChance,
    ref double monsterChance,
    ref double itemChance,
    ref double gemStoneChance)
  {
    if (this.mineLevel == 1)
    {
      monsterChance = 0.0;
      itemChance = 0.0;
      gemStoneChance = 0.0;
    }
    else if (this.mineLevel % 5 == 0 && this.getMineArea() != 121)
    {
      itemChance = 0.0;
      gemStoneChance = 0.0;
      if (this.mineLevel % 10 == 0)
        monsterChance = 0.0;
    }
    if (this.mustKillAllMonstersToAdvance())
    {
      monsterChance = 0.025;
      itemChance = 0.001;
      stoneChance = 0.0;
      gemStoneChance = 0.0;
      if (this.isDinoArea)
        itemChance *= 4.0;
    }
    monsterChance += 0.02 * (double) this.GetAdditionalDifficulty();
    int num1 = this.AnyOnlineFarmerHasBuff("23") ? 1 : 0;
    bool flag = this.AnyOnlineFarmerHasBuff("24");
    if (num1 != 0 && this.getMineArea() != 121)
    {
      if (!flag)
        monsterChance = 0.0;
    }
    else if (flag)
      monsterChance *= 2.0;
    gemStoneChance /= 2.0;
    if (this.isQuarryArea || this.getMineArea() == 77377)
    {
      gemStoneChance = 0.001;
      itemChance = 0.0001;
      stoneChance *= 2.0;
      monsterChance = 0.02;
    }
    if (this.GetAdditionalDifficulty() > 0 && this.getMineArea() == 40)
      monsterChance *= 0.6600000262260437;
    if (Utility.GetDayOfPassiveFestival("DesertFestival") <= 0 || this.getMineArea() != 121)
      return;
    double num2 = 1.0;
    foreach (int statueInvasionId in DesertFestival.CalicoStatueInvasionIds)
    {
      int num3;
      if (Game1.player.team.calicoStatueEffects.TryGetValue(statueInvasionId, out num3))
        monsterChance += (double) num3 * 0.01;
    }
    int num4;
    if (Game1.player.team.calicoStatueEffects.TryGetValue(7, out num4))
      num2 += (double) num4 * 0.2;
    monsterChance *= num2;
  }

  public bool AnyOnlineFarmerHasBuff(string which_buff)
  {
    if (which_buff == "23" && this.GetAdditionalDifficulty() > 0)
      return false;
    foreach (Farmer onlineFarmer in Game1.getOnlineFarmers())
    {
      if (onlineFarmer.hasBuff(which_buff))
        return true;
    }
    return false;
  }

  private void populateLevel()
  {
    this.objects.Clear();
    this.terrainFeatures.Clear();
    this.resourceClumps.Clear();
    this.debris.Clear();
    this.characters.Clear();
    this.ghostAdded = false;
    this.stonesLeftOnThisLevel = 0;
    if (this.mineLevel == 77377)
    {
      this.resourceClumps.Add(new ResourceClump(148, 2, 2, new Vector2(47f, 37f), textureName: "TileSheets\\Objects_2"));
      this.resourceClumps.Add(new ResourceClump(148, 2, 2, new Vector2(36f, 12f), textureName: "TileSheets\\Objects_2"));
    }
    double stoneChance = (double) this.mineRandom.Next(10, 30) / 100.0;
    double monsterChance = 0.002 + (double) this.mineRandom.Next(200) / 10000.0;
    double itemChance = 1.0 / 400.0;
    double gemStoneChance = 0.003;
    this.adjustLevelChances(ref stoneChance, ref monsterChance, ref itemChance, ref gemStoneChance);
    int numberSoFar = 0;
    bool flag1 = !MineShaft.permanentMineChanges.ContainsKey(this.mineLevel) || this.forceFirstTime;
    float num1 = 0.0f;
    if (Utility.GetDayOfPassiveFestival("DesertFestival") > 0 && this.mineLevel > 131)
      num1 += (float) (1.0 - 130.0 / (double) this.mineLevel);
    if (this.mineLevel > 1 && (this.mineLevel % 5 != 0 || this.mineLevel >= 121) && (this.mineRandom.NextBool() || this.isDinoArea))
    {
      Layer layer = this.map.RequireLayer("Back");
      int num2 = this.mineRandom.Next(5) + (int) (Game1.player.team.AverageDailyLuck((GameLocation) this) * 20.0);
      if (this.isDinoArea)
        num2 += this.map.Layers[0].LayerWidth * this.map.Layers[0].LayerHeight / 40;
      for (int index = 0; index < num2; ++index)
      {
        Point point1;
        Point point2;
        if (this.mineRandom.NextDouble() < 0.33 + (double) num1 / 2.0)
        {
          point1 = new Point(this.mineRandom.Next(layer.LayerWidth), 0);
          point2 = new Point(0, 1);
        }
        else if (this.mineRandom.NextBool())
        {
          point1 = new Point(0, this.mineRandom.Next(layer.LayerHeight));
          point2 = new Point(1, 0);
        }
        else
        {
          point1 = new Point(layer.LayerWidth - 1, this.mineRandom.Next(layer.LayerHeight));
          point2 = new Point(-1, 0);
        }
        while (this.isTileOnMap(point1.X, point1.Y))
        {
          point1.X += point2.X;
          point1.Y += point2.Y;
          if (this.isTileClearForMineObjects(point1.X, point1.Y))
          {
            Vector2 vector2 = new Vector2((float) point1.X, (float) point1.Y);
            if (this.isDinoArea)
            {
              this.terrainFeatures.Add(vector2, (TerrainFeature) new CosmeticPlant(this.mineRandom.Next(3)));
              break;
            }
            if (!this.mustKillAllMonstersToAdvance())
            {
              if (Utility.GetDayOfPassiveFestival("DesertFestival") > 0 && this.getMineArea() == 121 && !this.hasAddedDesertFestivalStatue && this.hasTileAt((int) vector2.X, (int) vector2.Y - 1, "Buildings"))
              {
                this.calicoStatueSpot.Value = point1;
                this.hasAddedDesertFestivalStatue = true;
                break;
              }
              this.objects.Add(vector2, (StardewValley.Object) BreakableContainer.GetBarrelForMines(vector2, this));
              break;
            }
            break;
          }
        }
      }
    }
    bool flag2 = false;
    if (this.mineLevel % 10 != 0 || this.getMineArea() == 121 && !this.isForcedChestLevel(this.mineLevel) && !this.netIsTreasureRoom.Value)
    {
      Layer layer = this.map.RequireLayer("Back");
      for (int index1 = 0; index1 < layer.LayerWidth; ++index1)
      {
        for (int index2 = 0; index2 < layer.LayerHeight; ++index2)
        {
          this.checkForMapAlterations(index1, index2);
          if (this.isTileClearForMineObjects(index1, index2))
          {
            if (this.mineRandom.NextDouble() <= stoneChance)
            {
              Vector2 vector2 = new Vector2((float) index1, (float) index2);
              if (!this.Objects.ContainsKey(vector2))
              {
                if (this.getMineArea() == 40 && this.mineRandom.NextDouble() < 0.15)
                {
                  int num3 = this.mineRandom.Next(319, 322);
                  if (this.GetAdditionalDifficulty() > 0 && this.mineLevel % 40 < 30)
                    num3 = this.mineRandom.Next(313, 316);
                  this.Objects.Add(vector2, new StardewValley.Object(num3.ToString(), 1)
                  {
                    Fragility = 2,
                    CanBeGrabbed = true
                  });
                }
                else if (this.rainbowLights.Value && this.mineRandom.NextDouble() < 0.55)
                {
                  if (this.mineRandom.NextDouble() < 0.25)
                  {
                    StardewValley.Object @object = ItemRegistry.Create<StardewValley.Object>(this.mineRandom.Next(5) != 0 ? "(O)420" : "(O)422");
                    @object.IsSpawnedObject = true;
                    this.Objects.Add(vector2, @object);
                  }
                }
                else
                {
                  StardewValley.Object litterObject = this.createLitterObject(0.001, 5E-05, gemStoneChance, vector2);
                  if (litterObject != null)
                  {
                    this.Objects.Add(vector2, litterObject);
                    if (litterObject.IsBreakableStone())
                      ++this.stonesLeftOnThisLevel;
                  }
                }
              }
            }
            else if (this.mineRandom.NextDouble() <= monsterChance && (double) this.getDistanceFromStart(index1, index2) > 5.0)
            {
              Monster monster = (Monster) null;
              if (Utility.GetDayOfPassiveFestival("DesertFestival") > 0 && this.getMineArea() == 121)
              {
                int[] statueInvasionIds = DesertFestival.CalicoStatueInvasionIds;
label_53:
                for (int index3 = 0; index3 < statueInvasionIds.Length; ++index3)
                {
                  int key = statueInvasionIds[index3];
                  int num4;
                  if (Game1.player.team.calicoStatueEffects.TryGetValue(key, out num4))
                  {
                    for (int index4 = 0; index4 < num4; ++index4)
                    {
                      if (this.mineRandom.NextBool(0.15))
                      {
                        Vector2 position = new Vector2((float) index1, (float) index2) * 64f;
                        switch (key)
                        {
                          case 0:
                            monster = (Monster) new Ghost(position, "Carbon Ghost");
                            goto label_53;
                          case 1:
                            monster = (Monster) new Serpent(position);
                            goto label_53;
                          case 2:
                            monster = this.mineRandom.NextDouble() >= 0.33 ? (Monster) new Skeleton(position, this.mineRandom.NextBool()) : (Monster) new Bat(position, 77377);
                            monster.BuffForAdditionalDifficulty(1);
                            goto label_53;
                          case 3:
                            monster = (Monster) new Bat(position, this.mineLevel);
                            goto label_53;
                          default:
                            goto label_53;
                        }
                      }
                    }
                  }
                }
              }
              if (monster == null)
                monster = this.BuffMonsterIfNecessary(this.getMonsterForThisLevel(this.mineLevel, index1, index2));
              if (!(monster is GreenSlime greenSlime))
              {
                if (!(monster is Leaper))
                {
                  if (!(monster is Grub))
                  {
                    if (monster is DustSpirit)
                    {
                      if (this.mineRandom.NextDouble() < 0.6)
                        this.tryToAddMonster(this.BuffMonsterIfNecessary((Monster) new DustSpirit(Vector2.Zero)), index1 - 1, index2);
                      if (this.mineRandom.NextDouble() < 0.6)
                        this.tryToAddMonster(this.BuffMonsterIfNecessary((Monster) new DustSpirit(Vector2.Zero)), index1 + 1, index2);
                      if (this.mineRandom.NextDouble() < 0.6)
                        this.tryToAddMonster(this.BuffMonsterIfNecessary((Monster) new DustSpirit(Vector2.Zero)), index1, index2 - 1);
                      if (this.mineRandom.NextDouble() < 0.6)
                        this.tryToAddMonster(this.BuffMonsterIfNecessary((Monster) new DustSpirit(Vector2.Zero)), index1, index2 + 1);
                    }
                  }
                  else
                  {
                    if (this.mineRandom.NextDouble() < 0.4)
                      this.tryToAddMonster(this.BuffMonsterIfNecessary((Monster) new Grub(Vector2.Zero)), index1 - 1, index2);
                    if (this.mineRandom.NextDouble() < 0.4)
                      this.tryToAddMonster(this.BuffMonsterIfNecessary((Monster) new Grub(Vector2.Zero)), index1 + 1, index2);
                    if (this.mineRandom.NextDouble() < 0.4)
                      this.tryToAddMonster(this.BuffMonsterIfNecessary((Monster) new Grub(Vector2.Zero)), index1, index2 - 1);
                    if (this.mineRandom.NextDouble() < 0.4)
                      this.tryToAddMonster(this.BuffMonsterIfNecessary((Monster) new Grub(Vector2.Zero)), index1, index2 + 1);
                  }
                }
                else
                {
                  float num5 = (float) (this.GetAdditionalDifficulty() + 1) * 0.3f;
                  if (this.mineRandom.NextDouble() < (double) num5)
                    this.tryToAddMonster(this.BuffMonsterIfNecessary((Monster) new Leaper(Vector2.Zero)), index1 - 1, index2);
                  if (this.mineRandom.NextDouble() < (double) num5)
                    this.tryToAddMonster(this.BuffMonsterIfNecessary((Monster) new Leaper(Vector2.Zero)), index1 + 1, index2);
                  if (this.mineRandom.NextDouble() < (double) num5)
                    this.tryToAddMonster(this.BuffMonsterIfNecessary((Monster) new Leaper(Vector2.Zero)), index1, index2 - 1);
                  if (this.mineRandom.NextDouble() < (double) num5)
                    this.tryToAddMonster(this.BuffMonsterIfNecessary((Monster) new Leaper(Vector2.Zero)), index1, index2 + 1);
                }
              }
              else
              {
                if (!flag2 && Game1.random.NextDouble() <= Math.Max(0.01, 0.012 + Game1.player.team.AverageDailyLuck((GameLocation) this) / 10.0) && Game1.player.team.SpecialOrderActive("Wizard2"))
                {
                  greenSlime.makePrismatic();
                  flag2 = true;
                }
                if (this.GetAdditionalDifficulty() > 0 && this.mineRandom.NextDouble() < (double) Math.Min((float) this.GetAdditionalDifficulty() * 0.1f, 0.5f))
                {
                  if (this.mineRandom.NextDouble() < 0.0099999997764825821)
                    greenSlime.stackedSlimes.Value = 4;
                  else
                    greenSlime.stackedSlimes.Value = 2;
                }
              }
              if (this.mineRandom.NextDouble() < 0.00175)
                monster.hasSpecialItem.Value = true;
              if (monster.GetBoundingBox().Width <= 64 /*0x40*/ || this.isTileClearForMineObjects(index1 + 1, index2))
                this.characters.Add((NPC) monster);
            }
            else if (this.mineRandom.NextDouble() <= itemChance)
            {
              Vector2 vector2 = new Vector2((float) index1, (float) index2);
              this.Objects.Add(vector2, this.getRandomItemForThisLevel(this.mineLevel, vector2));
            }
            else if (this.mineRandom.NextDouble() <= 0.005 && !this.isDarkArea() && !this.mustKillAllMonstersToAdvance() && (this.GetAdditionalDifficulty() <= 0 || this.getMineArea() == 40 && this.mineLevel % 40 < 30))
            {
              if (this.isTileClearForMineObjects(index1 + 1, index2) && this.isTileClearForMineObjects(index1, index2 + 1) && this.isTileClearForMineObjects(index1 + 1, index2 + 1))
              {
                Vector2 tile = new Vector2((float) index1, (float) index2);
                int parentSheetIndex = this.mineRandom.Choose<int>(752, 754);
                if (this.getMineArea() == 40)
                {
                  if (this.GetAdditionalDifficulty() > 0)
                  {
                    parentSheetIndex = 600;
                    if (this.mineRandom.NextDouble() < 0.1)
                      parentSheetIndex = 602;
                  }
                  else
                    parentSheetIndex = this.mineRandom.Choose<int>(756, 758);
                }
                this.resourceClumps.Add(new ResourceClump(parentSheetIndex, 2, 2, tile));
              }
            }
            else if (this.GetAdditionalDifficulty() > 0)
            {
              if (this.getMineArea() == 40 && this.mineLevel % 40 < 30 && this.mineRandom.NextDouble() < 0.01 && this.hasTileAt(index1, index2 - 1, "Buildings"))
                this.terrainFeatures.Add(new Vector2((float) index1, (float) index2), (TerrainFeature) new Tree("8", 5));
              else if (this.getMineArea() == 40 && this.mineLevel % 40 < 30 && this.mineRandom.NextDouble() < 0.1 && (this.hasTileAt(index1, index2 - 1, "Buildings") || this.hasTileAt(index1 - 1, index2, "Buildings") || this.hasTileAt(index1, index2 + 1, "Buildings") || this.hasTileAt(index1 + 1, index2, "Buildings") || this.terrainFeatures.ContainsKey(new Vector2((float) (index1 - 1), (float) index2)) || this.terrainFeatures.ContainsKey(new Vector2((float) (index1 + 1), (float) index2)) || this.terrainFeatures.ContainsKey(new Vector2((float) index1, (float) (index2 - 1))) || this.terrainFeatures.ContainsKey(new Vector2((float) index1, (float) (index2 + 1)))))
                this.terrainFeatures.Add(new Vector2((float) index1, (float) index2), (TerrainFeature) new Grass(this.mineLevel >= 50 ? 6 : 5, this.mineLevel >= 50 ? 1 : this.mineRandom.Next(1, 5)));
              else if (this.getMineArea() == 80 /*0x50*/ && !this.isDarkArea() && this.mineRandom.NextDouble() < 0.1 && (this.hasTileAt(index1, index2 - 1, "Buildings") || this.hasTileAt(index1 - 1, index2, "Buildings") || this.hasTileAt(index1, index2 + 1, "Buildings") || this.hasTileAt(index1 + 1, index2, "Buildings") || this.terrainFeatures.ContainsKey(new Vector2((float) (index1 - 1), (float) index2)) || this.terrainFeatures.ContainsKey(new Vector2((float) (index1 + 1), (float) index2)) || this.terrainFeatures.ContainsKey(new Vector2((float) index1, (float) (index2 - 1))) || this.terrainFeatures.ContainsKey(new Vector2((float) index1, (float) (index2 + 1)))))
                this.terrainFeatures.Add(new Vector2((float) index1, (float) index2), (TerrainFeature) new Grass(4, this.mineRandom.Next(1, 5)));
            }
          }
          else if (this.isContainerPlatform(index1, index2) && this.CanItemBePlacedHere(new Vector2((float) index1, (float) index2)) && this.mineRandom.NextDouble() < 0.4 && (flag1 || this.canAdd(0, numberSoFar)))
          {
            Vector2 vector2 = new Vector2((float) index1, (float) index2);
            this.objects.Add(vector2, (StardewValley.Object) BreakableContainer.GetBarrelForMines(vector2, this));
            ++numberSoFar;
            if (flag1)
              this.updateMineLevelData(0);
          }
          else if (this.mineRandom.NextDouble() <= monsterChance && this.CanSpawnCharacterHere(new Vector2((float) index1, (float) index2)) && this.isTileOnClearAndSolidGround(index1, index2) && (double) this.getDistanceFromStart(index1, index2) > 5.0 && (!this.AnyOnlineFarmerHasBuff("23") || this.getMineArea() == 121))
          {
            Monster monster = this.BuffMonsterIfNecessary(this.getMonsterForThisLevel(this.mineLevel, index1, index2));
            if (monster.GetBoundingBox().Width <= 64 /*0x40*/ || this.isTileClearForMineObjects(index1 + 1, index2))
            {
              if (this.mineRandom.NextDouble() < 0.01)
                monster.hasSpecialItem.Value = true;
              this.characters.Add((NPC) monster);
            }
          }
        }
      }
      if (this.stonesLeftOnThisLevel > 35)
      {
        int num6 = this.stonesLeftOnThisLevel / 35;
        for (int index5 = 0; index5 < num6; ++index5)
        {
          Vector2 key1;
          StardewValley.Object object1;
          if (Utility.TryGetRandom(this.objects, out key1, out object1) && object1.IsBreakableStone())
          {
            int num7 = this.mineRandom.Next(3, 8);
            bool flag3 = this.mineRandom.NextDouble() < 0.1;
            for (int index6 = (int) key1.X - num7 / 2; (double) index6 < (double) key1.X + (double) (num7 / 2); ++index6)
            {
              for (int index7 = (int) key1.Y - num7 / 2; (double) index7 < (double) key1.Y + (double) (num7 / 2); ++index7)
              {
                Vector2 key2 = new Vector2((float) index6, (float) index7);
                StardewValley.Object object2;
                if (this.objects.TryGetValue(key2, out object2) && object2.IsBreakableStone())
                {
                  this.objects.Remove(key2);
                  --this.stonesLeftOnThisLevel;
                  if ((double) this.getDistanceFromStart(index6, index7) > 5.0 && flag3 && this.mineRandom.NextDouble() < 0.12)
                  {
                    Monster monster = this.BuffMonsterIfNecessary(this.getMonsterForThisLevel(this.mineLevel, index6, index7));
                    if (monster.GetBoundingBox().Width <= 64 /*0x40*/ || this.isTileClearForMineObjects(index6 + 1, index7))
                      this.characters.Add((NPC) monster);
                  }
                }
              }
            }
          }
        }
      }
      this.tryToAddAreaUniques();
      if (this.mineRandom.NextDouble() < 0.95 && !this.mustKillAllMonstersToAdvance() && this.mineLevel > 1 && this.mineLevel % 5 != 0 && this.shouldCreateLadderOnThisLevel())
      {
        Vector2 v = new Vector2((float) this.mineRandom.Next(layer.LayerWidth), (float) this.mineRandom.Next(layer.LayerHeight));
        if (this.isTileClearForMineObjects(v))
          this.createLadderDown((int) v.X, (int) v.Y);
      }
      if (this.mustKillAllMonstersToAdvance() && this.EnemyCount <= 1)
        this.characters.Add((NPC) new Bat(this.tileBeneathLadder * 64f + new Vector2(256f, 256f)));
    }
    if (this.mustKillAllMonstersToAdvance() && !this.isDinoArea || this.mineLevel % 5 == 0 || this.mineLevel <= 2 || this.isForcedChestLevel(this.mineLevel) || this.netIsTreasureRoom.Value)
      return;
    this.tryToAddOreClumps();
    if (!this.isLightingDark.Value)
      return;
    this.tryToAddOldMinerPath();
  }

  public void placeAppropriateOreAt(Vector2 tile)
  {
    if (!this.CanItemBePlacedHere(tile, ignorePassables: CollisionMask.None))
      return;
    this.objects.Add(tile, this.getAppropriateOre(tile));
  }

  public StardewValley.Object getAppropriateOre(Vector2 tile)
  {
    StardewValley.Object appropriateOre = new StardewValley.Object("751", 1)
    {
      MinutesUntilReady = 3
    };
    switch (this.getMineArea())
    {
      case 0:
      case 10:
        if (this.GetAdditionalDifficulty() > 0)
        {
          appropriateOre = new StardewValley.Object("849", 1)
          {
            MinutesUntilReady = 6
          };
          break;
        }
        break;
      case 40:
        if (this.GetAdditionalDifficulty() > 0)
        {
          ColoredObject coloredObject = new ColoredObject("290", 1, new Color(150, 225, 160 /*0xA0*/));
          coloredObject.MinutesUntilReady = 6;
          coloredObject.TileLocation = tile;
          coloredObject.Flipped = this.mineRandom.NextBool();
          appropriateOre = (StardewValley.Object) coloredObject;
          break;
        }
        if (this.mineRandom.NextDouble() < 0.8)
        {
          appropriateOre = new StardewValley.Object("290", 1)
          {
            MinutesUntilReady = 4
          };
          break;
        }
        break;
      case 80 /*0x50*/:
        if (this.mineRandom.NextDouble() < 0.8)
        {
          appropriateOre = new StardewValley.Object("764", 1)
          {
            MinutesUntilReady = 8
          };
          break;
        }
        break;
      case 121:
        if (Utility.GetDayOfPassiveFestival("DesertFestival") > 0 && this.mineRandom.NextBool(0.25 + (double) (Game1.player.team.calicoEggSkullCavernRating.Value * 5) / 100.0))
        {
          appropriateOre = new StardewValley.Object("CalicoEggStone_" + this.mineRandom.Next(3).ToString(), 1)
          {
            MinutesUntilReady = 8
          };
          break;
        }
        appropriateOre = new StardewValley.Object("764", 1)
        {
          MinutesUntilReady = 8
        };
        if (this.mineRandom.NextDouble() < 0.02)
        {
          appropriateOre = new StardewValley.Object("765", 1)
          {
            MinutesUntilReady = 16 /*0x10*/
          };
          break;
        }
        break;
    }
    if (this.mineRandom.NextDouble() < 0.25 && this.getMineArea() != 40 && this.GetAdditionalDifficulty() <= 0)
      appropriateOre = new StardewValley.Object(this.mineRandom.Choose<string>("668", "670"), 1)
      {
        MinutesUntilReady = 2
      };
    return appropriateOre;
  }

  public void tryToAddOreClumps()
  {
    if (this.mineRandom.NextDouble() >= 0.55 + Game1.player.team.AverageDailyLuck((GameLocation) this))
      return;
    Vector2 randomTile = this.getRandomTile();
    for (int index = 0; index < 1 || this.mineRandom.NextDouble() < 0.25 + Game1.player.team.AverageDailyLuck((GameLocation) this); ++index)
    {
      if (this.CanItemBePlacedHere(randomTile, ignorePassables: CollisionMask.None) && this.isTileOnClearAndSolidGround(randomTile) && this.doesTileHaveProperty((int) randomTile.X, (int) randomTile.Y, "Diggable", "Back") == null)
      {
        StardewValley.Object o = this.getAppropriateOre(randomTile);
        if (o.QualifiedItemId == "(O)670")
          o = new StardewValley.Object("668", 1);
        bool flag = o.QualifiedItemId == "(O)668";
        if (o.QualifiedItemId.Contains("CalicoEgg"))
          Utility.recursiveObjectPlacement(o, (int) randomTile.X, (int) randomTile.Y, 0.949999988079071, 0.30000001192092896, (GameLocation) this, "Dirt", failChance: 0.05000000074505806, itemIDVariations: new List<string>()
          {
            "CalicoEggStone_0",
            "CalicoEggStone_1",
            "CalicoEggStone_2"
          });
        else
          Utility.recursiveObjectPlacement(o, (int) randomTile.X, (int) randomTile.Y, 0.949999988079071, 0.30000001192092896, (GameLocation) this, "Dirt", flag ? 1 : 0, 0.05000000074505806, flag ? 2 : 1);
      }
      randomTile = this.getRandomTile();
    }
  }

  public void tryToAddOldMinerPath()
  {
    Vector2 randomTile = this.getRandomTile();
    for (int index = 0; !this.isTileOnClearAndSolidGround(randomTile) && index < 8; ++index)
      randomTile = this.getRandomTile();
    if (!this.isTileOnClearAndSolidGround(randomTile))
      return;
    Stack<Point> path = PathFindController.findPath(Utility.Vector2ToPoint(this.tileBeneathLadder), Utility.Vector2ToPoint(randomTile), new PathFindController.isAtEnd(PathFindController.isAtEndPoint), (GameLocation) this, (Character) Game1.player, 500);
    if (path == null)
      return;
    while (path.Count > 0)
    {
      Point point = path.Pop();
      this.removeObjectsAndSpawned(point.X, point.Y, 1, 1);
      if (path.Count > 0 && this.mineRandom.NextDouble() < 0.2)
      {
        Vector2 vector2 = path.Peek().X == point.X ? new Vector2((float) (point.X + this.mineRandom.Choose<int>(-1, 1)), (float) point.Y) : new Vector2((float) point.X, (float) (point.Y + this.mineRandom.Choose<int>(-1, 1)));
        if (!vector2.Equals(Vector2.Zero) && this.CanItemBePlacedHere(vector2) && this.isTileOnClearAndSolidGround(vector2))
        {
          if (this.mineRandom.NextBool())
            new Torch().placementAction((GameLocation) this, (int) vector2.X * 64 /*0x40*/, (int) vector2.Y * 64 /*0x40*/);
          else
            this.placeAppropriateOreAt(vector2);
        }
      }
    }
  }

  public void tryToAddAreaUniques()
  {
    if (this.getMineArea() != 10 && this.getMineArea() != 80 /*0x50*/ && (this.getMineArea() != 40 || this.mineRandom.NextDouble() >= 0.1) || this.isDarkArea() || this.mustKillAllMonstersToAdvance())
      return;
    int num1 = this.mineRandom.Next(7, 24);
    int num2 = this.getMineArea() == 80 /*0x50*/ ? 316 : (this.getMineArea() == 40 ? 319 : 313);
    Color color = Color.White;
    int objectIndexAddRange = 2;
    if (this.GetAdditionalDifficulty() > 0)
    {
      if (this.getMineArea() == 10)
      {
        num2 = 674;
        color = new Color(30, 120, (int) byte.MaxValue);
      }
      else if (this.getMineArea() == 40)
      {
        if (this.mineLevel % 40 >= 30)
        {
          num2 = 319;
        }
        else
        {
          num2 = 882;
          color = new Color(100, 180, 220);
        }
      }
      else if (this.getMineArea() == 80 /*0x50*/)
        return;
    }
    Layer layer = this.map.RequireLayer("Back");
    for (int index = 0; index < num1; ++index)
    {
      Vector2 vector2 = new Vector2((float) this.mineRandom.Next(layer.LayerWidth), (float) this.mineRandom.Next(layer.LayerHeight));
      if (color.Equals(Color.White))
      {
        Utility.recursiveObjectPlacement(new StardewValley.Object(num2.ToString(), 1)
        {
          Fragility = 2,
          CanBeGrabbed = true
        }, (int) vector2.X, (int) vector2.Y, 1.0, (double) this.mineRandom.Next(10, 40) / 100.0, (GameLocation) this, "Dirt", objectIndexAddRange, 0.29);
      }
      else
      {
        ColoredObject o = new ColoredObject(num2.ToString(), 1, color);
        o.Fragility = 2;
        o.CanBeGrabbed = true;
        o.CanBeSetDown = true;
        o.TileLocation = vector2;
        Utility.recursiveObjectPlacement((StardewValley.Object) o, (int) vector2.X, (int) vector2.Y, 1.0, (double) this.mineRandom.Next(10, 40) / 100.0, (GameLocation) this, "Dirt", objectIndexAddRange, 0.29);
      }
    }
  }

  public bool tryToAddMonster(Monster m, int tileX, int tileY)
  {
    if (!this.isTileClearForMineObjects(tileX, tileY) || this.IsTileOccupiedBy(new Vector2((float) tileX, (float) tileY)))
      return false;
    m.setTilePosition(tileX, tileY);
    this.characters.Add((NPC) m);
    return true;
  }

  public bool isContainerPlatform(int x, int y) => this.getTileIndexAt(x, y, "Back", "mine") == 257;

  public bool mustKillAllMonstersToAdvance()
  {
    return this.isSlimeArea || this.isMonsterArea || this.isDinoArea;
  }

  public void createLadderAt(Vector2 p, string sound = "hoeHit")
  {
    if (!this.shouldCreateLadderOnThisLevel())
      return;
    this.playSound(sound);
    this.createLadderAtEvent[p] = true;
  }

  public bool shouldCreateLadderOnThisLevel() => this.mineLevel != 77377 && this.mineLevel != 120;

  private void doCreateLadderAt(Vector2 p)
  {
    string str = Game1.currentLocation == this ? "sandyStep" : (string) null;
    this.updateMap();
    this.setMapTile((int) p.X, (int) p.Y, 173, "Buildings", "mine");
    this.temporarySprites.Add(new TemporaryAnimatedSprite(5, p * 64f, Color.White * 0.5f)
    {
      interval = 80f
    });
    this.temporarySprites.Add(new TemporaryAnimatedSprite(5, p * 64f - new Vector2(16f, 16f), Color.White * 0.5f)
    {
      delayBeforeAnimationStart = 150,
      interval = 80f,
      scale = 0.75f,
      startSound = str
    });
    this.temporarySprites.Add(new TemporaryAnimatedSprite(5, p * 64f + new Vector2(32f, 16f), Color.White * 0.5f)
    {
      delayBeforeAnimationStart = 300,
      interval = 80f,
      scale = 0.75f,
      startSound = str
    });
    this.temporarySprites.Add(new TemporaryAnimatedSprite(5, p * 64f - new Vector2(32f, -16f), Color.White * 0.5f)
    {
      delayBeforeAnimationStart = 450,
      interval = 80f,
      scale = 0.75f,
      startSound = str
    });
    this.temporarySprites.Add(new TemporaryAnimatedSprite(5, p * 64f - new Vector2(-16f, 16f), Color.White * 0.5f)
    {
      delayBeforeAnimationStart = 600,
      interval = 80f,
      scale = 0.75f,
      startSound = str
    });
    if (Game1.player.currentLocation != this)
      return;
    Game1.player.TemporaryPassableTiles.Add(new Microsoft.Xna.Framework.Rectangle((int) p.X * 64 /*0x40*/, (int) p.Y * 64 /*0x40*/, 64 /*0x40*/, 64 /*0x40*/));
  }

  public bool recursiveTryToCreateLadderDown(Vector2 centerTile, string sound = "hoeHit", int maxIterations = 16 /*0x10*/)
  {
    int num = 0;
    Queue<Vector2> vector2Queue = new Queue<Vector2>();
    vector2Queue.Enqueue(centerTile);
    List<Vector2> vector2List = new List<Vector2>();
    for (; num < maxIterations && vector2Queue.Count > 0; ++num)
    {
      Vector2 vector2 = vector2Queue.Dequeue();
      vector2List.Add(vector2);
      if (!this.IsTileOccupiedBy(vector2) && this.isTileOnClearAndSolidGround(vector2) && this.doesTileHaveProperty((int) vector2.X, (int) vector2.Y, "Type", "Back") != null && this.doesTileHaveProperty((int) vector2.X, (int) vector2.Y, "Type", "Back").Equals("Stone"))
      {
        this.createLadderAt(vector2);
        return true;
      }
      foreach (Vector2 directionsTileVector in Utility.DirectionsTileVectors)
      {
        if (!vector2List.Contains(vector2 + directionsTileVector))
          vector2Queue.Enqueue(vector2 + directionsTileVector);
      }
    }
    return false;
  }

  public override void monsterDrop(Monster monster, int x, int y, Farmer who)
  {
    if (monster.hasSpecialItem.Value)
      Game1.createItemDebris(MineShaft.getSpecialItemForThisMineLevel(this.mineLevel, x / 64 /*0x40*/, y / 64 /*0x40*/), monster.Position, Game1.random.Next(4), monster.currentLocation);
    else if (this.mineLevel > 121 && who != null && who.getFriendshipHeartLevelForNPC("Krobus") >= 10 && who.houseUpgradeLevel.Value >= 1 && !who.isMarriedOrRoommates() && !who.isEngaged() && Game1.random.NextDouble() < 0.001)
      Game1.createItemDebris(ItemRegistry.Create("(O)808"), monster.Position, Game1.random.Next(4), monster.currentLocation);
    else
      base.monsterDrop(monster, x, y, who);
    double num = who == null || !who.hasBuff("dwarfStatue_1") ? 0.0 : 0.07;
    if ((this.mustKillAllMonstersToAdvance() || Game1.random.NextDouble() >= 0.15 + num) && (!this.mustKillAllMonstersToAdvance() || this.EnemyCount > 1))
      return;
    Vector2 vector2 = new Vector2((float) x, (float) y) / 64f;
    vector2.X = (float) (int) vector2.X;
    vector2.Y = (float) (int) vector2.Y;
    monster.IsInvisible = true;
    if (!this.IsTileOccupiedBy(vector2) && this.isTileOnClearAndSolidGround(vector2) && this.doesTileHaveProperty((int) vector2.X, (int) vector2.Y, "Type", "Back") != null && this.doesTileHaveProperty((int) vector2.X, (int) vector2.Y, "Type", "Back").Equals("Stone"))
    {
      this.createLadderAt(vector2);
    }
    else
    {
      if (!this.mustKillAllMonstersToAdvance() || this.EnemyCount > 1)
        return;
      vector2 = new Vector2((float) (int) this.tileBeneathLadder.X, (float) (int) this.tileBeneathLadder.Y);
      this.createLadderAt(vector2, "newArtifact");
      if (!this.mustKillAllMonstersToAdvance() || !who.IsLocalPlayer || who.currentLocation != this)
        return;
      Game1.showGlobalMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:MineShaft.cs.9484"));
    }
  }

  public Item GetReplacementChestItem(int floor)
  {
    List<Item> options = (List<Item>) null;
    if (Game1.netWorldState.Value.ShuffleMineChests == Game1.MineChestType.Remixed)
    {
      options = new List<Item>();
      switch (floor)
      {
        case 10:
          options.Add(ItemRegistry.Create("(B)506"));
          options.Add(ItemRegistry.Create("(B)507"));
          options.Add(ItemRegistry.Create("(W)12"));
          options.Add(ItemRegistry.Create("(W)17"));
          options.Add(ItemRegistry.Create("(W)22"));
          options.Add(ItemRegistry.Create("(W)31"));
          break;
        case 20:
          options.Add(ItemRegistry.Create("(W)11"));
          options.Add(ItemRegistry.Create("(W)24"));
          options.Add(ItemRegistry.Create("(W)20"));
          options.Add((Item) new Ring("517"));
          options.Add((Item) new Ring("519"));
          break;
        case 50:
          options.Add(ItemRegistry.Create("(B)509"));
          options.Add(ItemRegistry.Create("(B)510"));
          options.Add(ItemRegistry.Create("(B)508"));
          options.Add(ItemRegistry.Create("(W)1"));
          options.Add(ItemRegistry.Create("(W)43"));
          break;
        case 60:
          options.Add(ItemRegistry.Create("(W)21"));
          options.Add(ItemRegistry.Create("(W)44"));
          options.Add(ItemRegistry.Create("(W)6"));
          options.Add(ItemRegistry.Create("(W)18"));
          options.Add(ItemRegistry.Create("(W)27"));
          break;
        case 80 /*0x50*/:
          options.Add(ItemRegistry.Create("(B)512"));
          options.Add(ItemRegistry.Create("(B)511"));
          options.Add(ItemRegistry.Create("(W)10"));
          options.Add(ItemRegistry.Create("(W)7"));
          options.Add(ItemRegistry.Create("(W)46"));
          options.Add(ItemRegistry.Create("(W)19"));
          break;
        case 90:
          options.Add(ItemRegistry.Create("(W)8"));
          options.Add(ItemRegistry.Create("(W)52"));
          options.Add(ItemRegistry.Create("(W)45"));
          options.Add(ItemRegistry.Create("(W)5"));
          options.Add(ItemRegistry.Create("(W)60"));
          break;
        case 110:
          options.Add(ItemRegistry.Create("(B)514"));
          options.Add(ItemRegistry.Create("(B)878"));
          options.Add(ItemRegistry.Create("(W)50"));
          options.Add(ItemRegistry.Create("(W)28"));
          break;
      }
    }
    // ISSUE: explicit non-virtual call
    return options != null && __nonvirtual (options.Count) > 0 ? Utility.CreateRandom((double) Game1.uniqueIDForThisGame * 512.0, (double) floor).ChooseFrom<Item>((IList<Item>) options) : (Item) null;
  }

  private void addLevelChests()
  {
    List<Item> items = new List<Item>();
    Vector2 vector2 = new Vector2(9f, 9f);
    Color color = Color.White;
    if (this.mineLevel < 121 && this.mineLevel % 20 == 0 && this.mineLevel % 40 != 0)
      vector2.Y += 4f;
    Item replacementChestItem = this.GetReplacementChestItem(this.mineLevel);
    bool flag = false;
    if (replacementChestItem != null)
    {
      items.Add(replacementChestItem);
    }
    else
    {
      switch (this.mineLevel)
      {
        case 5:
          Game1.player.completeQuest("14");
          if (!Game1.player.hasOrWillReceiveMail("guildQuest"))
          {
            Game1.addMailForTomorrow("guildQuest");
            break;
          }
          break;
        case 10:
          items.Add(ItemRegistry.Create("(B)506"));
          break;
        case 20:
          items.Add(ItemRegistry.Create("(W)11"));
          break;
        case 40:
          Game1.player.completeQuest("17");
          items.Add(ItemRegistry.Create("(W)32"));
          break;
        case 50:
          items.Add(ItemRegistry.Create("(B)509"));
          break;
        case 60:
          items.Add(ItemRegistry.Create("(W)21"));
          break;
        case 70:
          items.Add(ItemRegistry.Create("(W)33"));
          break;
        case 80 /*0x50*/:
          items.Add(ItemRegistry.Create("(B)512"));
          break;
        case 90:
          items.Add(ItemRegistry.Create("(W)8"));
          break;
        case 100:
          items.Add((Item) new StardewValley.Object("434", 1));
          break;
        case 110:
          items.Add(ItemRegistry.Create("(B)514"));
          break;
        case 120:
          Game1.player.completeQuest("18");
          Game1.player.stats.checkForMineAchievement(true, true);
          if (!Game1.player.hasSkullKey)
          {
            Game1.player.chestConsumedMineLevels.Remove(120);
            items.Add((Item) new SpecialItem(4));
            color = Color.Pink;
            break;
          }
          break;
        case 220:
          if (Game1.player.secretNotesSeen.Contains(10) && !Game1.player.mailReceived.Contains("qiCave"))
          {
            Game1.eventUp = true;
            Game1.displayHUD = false;
            Game1.player.CanMove = false;
            Game1.player.showNotCarrying();
            this.currentEvent = new StardewValley.Event(Game1.content.LoadString(MineShaft.numberOfCraftedStairsUsedThisRun <= 10 ? "Data\\ExtraDialogue:SkullCavern_100_event_honorable" : "Data\\ExtraDialogue:SkullCavern_100_event"));
            this.currentEvent.exitLocation = new LocationRequest(this.Name, false, (GameLocation) this);
            Game1.player.chestConsumedMineLevels[this.mineLevel] = true;
            break;
          }
          flag = true;
          break;
        case 320:
        case 420:
          flag = true;
          break;
      }
    }
    if (this.netIsTreasureRoom.Value | flag)
      items.Add(MineShaft.getTreasureRoomItem());
    if (this.mineLevel == 320)
      ++vector2.X;
    if (items.Count > 0 && !Game1.player.chestConsumedMineLevels.ContainsKey(this.mineLevel))
    {
      this.overlayObjects[vector2] = (StardewValley.Object) new Chest(items, vector2)
      {
        Tint = color
      };
      if (this.getMineArea() == 121 & flag)
        (this.overlayObjects[vector2] as Chest).SetBigCraftableSpriteIndex(344);
    }
    if (this.mineLevel == 320 || this.mineLevel == 420)
    {
      this.overlayObjects[vector2 + new Vector2(-2f, 0.0f)] = (StardewValley.Object) new Chest(new List<Item>()
      {
        MineShaft.getTreasureRoomItem()
      }, vector2 + new Vector2(-2f, 0.0f))
      {
        Tint = new Color((int) byte.MaxValue, 210, 200)
      };
      (this.overlayObjects[vector2 + new Vector2(-2f, 0.0f)] as Chest).SetBigCraftableSpriteIndex(344);
    }
    if (this.mineLevel != 420)
      return;
    this.overlayObjects[vector2 + new Vector2(2f, 0.0f)] = (StardewValley.Object) new Chest(new List<Item>()
    {
      MineShaft.getTreasureRoomItem()
    }, vector2 + new Vector2(2f, 0.0f))
    {
      Tint = new Color(216, (int) byte.MaxValue, 240 /*0xF0*/)
    };
    (this.overlayObjects[vector2 + new Vector2(2f, 0.0f)] as Chest).SetBigCraftableSpriteIndex(344);
  }

  private bool isForcedChestLevel(int level) => level == 220 || level == 320 || level == 420;

  public static Item getTreasureRoomItem()
  {
    if (Game1.player.stats.Get(StatKeys.Mastery(0)) > 0U && Game1.random.NextDouble() < 0.02)
      return ItemRegistry.Create("(O)GoldenAnimalCracker");
    if (Trinket.CanSpawnTrinket(Game1.player) && Game1.random.NextDouble() < 0.045)
      return (Item) Trinket.GetRandomTrinket();
    switch (Game1.random.Next(26))
    {
      case 0:
        return ItemRegistry.Create("(O)288", 5);
      case 1:
        return ItemRegistry.Create("(O)287", 10);
      case 2:
        return !Game1.MasterPlayer.hasOrWillReceiveMail("volcanoShortcutUnlocked") || Game1.random.NextDouble() >= 0.66 ? ItemRegistry.Create("(O)275", 5) : ItemRegistry.Create("(O)848", 5 + Game1.random.Next(1, 4) * 5);
      case 3:
        return ItemRegistry.Create("(O)773", Game1.random.Next(2, 5));
      case 4:
        return ItemRegistry.Create("(O)749", 5 + (Game1.random.NextDouble() < 0.25 ? 5 : 0));
      case 5:
        return ItemRegistry.Create("(O)688", 5);
      case 6:
        return ItemRegistry.Create("(O)681", Game1.random.Next(1, 4));
      case 7:
        return ItemRegistry.Create("(O)" + Game1.random.Next(628, 634).ToString());
      case 8:
        return ItemRegistry.Create("(O)645", Game1.random.Next(1, 3));
      case 9:
        return ItemRegistry.Create("(O)621", 4);
      case 10:
        return Game1.random.NextDouble() >= 0.33 ? ItemRegistry.Create("(O)" + Game1.random.Next(472, 499).ToString(), Game1.random.Next(1, 5) * 5) : ItemRegistry.Create("(O)802", 15);
      case 11:
        return ItemRegistry.Create("(O)286", 15);
      case 12:
        return Game1.random.NextDouble() >= 0.5 ? ItemRegistry.Create("(O)437") : ItemRegistry.Create("(O)265");
      case 13:
        return ItemRegistry.Create("(O)439");
      case 14:
        return Game1.random.NextDouble() >= 0.33 ? ItemRegistry.Create("(O)349", Game1.random.Next(2, 5)) : ItemRegistry.Create("(O)" + (Game1.random.NextDouble() < 0.5 ? 226 : 732).ToString(), 5);
      case 15:
        return ItemRegistry.Create("(O)337", Game1.random.Next(2, 4));
      case 16 /*0x10*/:
        return Game1.random.NextDouble() >= 0.33 ? ItemRegistry.Create("(O)" + Game1.random.Next(235, 245).ToString(), 5) : ItemRegistry.Create("(O)" + (Game1.random.NextDouble() < 0.5 ? 226 : 732).ToString(), 5);
      case 17:
        return ItemRegistry.Create("(O)74");
      case 18:
        return ItemRegistry.Create("(BC)21");
      case 19:
        return ItemRegistry.Create("(BC)25");
      case 20:
        return ItemRegistry.Create("(BC)165");
      case 21:
        return ItemRegistry.Create(Game1.random.NextBool() ? "(H)38" : "(H)37");
      case 22:
        return Game1.player.mailReceived.Contains("sawQiPlane") ? ItemRegistry.Create(Game1.player.stats.Get(StatKeys.Mastery(2)) > 0U ? "(O)GoldenMysteryBox" : "(O)MysteryBox", 5) : ItemRegistry.Create("(O)749", 5 + (Game1.random.NextDouble() < 0.25 ? 5 : 0));
      case 23:
        return ItemRegistry.Create("(H)65");
      case 24:
        return ItemRegistry.Create("(BC)272");
      case 25:
        return ItemRegistry.Create("(H)83");
      default:
        return ItemRegistry.Create("(O)288", 5);
    }
  }

  public static Item getSpecialItemForThisMineLevel(int level, int x, int y)
  {
    Random random = Utility.CreateRandom((double) level, (double) Game1.stats.DaysPlayed, (double) x, (double) y * 9999.0);
    if (Game1.mine == null)
      return ItemRegistry.Create("(O)388");
    if (Game1.mine.GetAdditionalDifficulty() > 0)
    {
      if (random.NextDouble() < 0.02)
        return ItemRegistry.Create("(BC)272");
      switch (random.Next(7))
      {
        case 0:
          return MeleeWeapon.attemptAddRandomInnateEnchantment(ItemRegistry.Create("(W)61"), random);
        case 1:
          return ItemRegistry.Create("(O)910");
        case 2:
          return ItemRegistry.Create("(O)913");
        case 3:
          return ItemRegistry.Create("(O)915");
        case 4:
          return (Item) new Ring("527");
        case 5:
          return ItemRegistry.Create("(O)858");
        case 6:
          Item treasureRoomItem = MineShaft.getTreasureRoomItem();
          treasureRoomItem.Stack = 1;
          return treasureRoomItem;
      }
    }
    if (level < 20)
    {
      switch (random.Next(6))
      {
        case 0:
          return MeleeWeapon.attemptAddRandomInnateEnchantment(ItemRegistry.Create("(W)16"), random);
        case 1:
          return MeleeWeapon.attemptAddRandomInnateEnchantment(ItemRegistry.Create("(W)24"), random);
        case 2:
          return ItemRegistry.Create("(B)504");
        case 3:
          return ItemRegistry.Create("(B)505");
        case 4:
          return (Item) new Ring("516");
        case 5:
          return (Item) new Ring("518");
      }
    }
    else if (level < 40)
    {
      switch (random.Next(7))
      {
        case 0:
          return MeleeWeapon.attemptAddRandomInnateEnchantment(ItemRegistry.Create("(W)22"), random);
        case 1:
          return MeleeWeapon.attemptAddRandomInnateEnchantment(ItemRegistry.Create("(W)24"), random);
        case 2:
          return ItemRegistry.Create("(B)504");
        case 3:
          return ItemRegistry.Create("(B)505");
        case 4:
          return (Item) new Ring("516");
        case 5:
          return (Item) new Ring("518");
        case 6:
          return ItemRegistry.Create("(W)15");
      }
    }
    else if (level < 60)
    {
      switch (random.Next(7))
      {
        case 0:
          return MeleeWeapon.attemptAddRandomInnateEnchantment(ItemRegistry.Create("(W)6"), random);
        case 1:
          return MeleeWeapon.attemptAddRandomInnateEnchantment(ItemRegistry.Create("(W)26"), random);
        case 2:
          return MeleeWeapon.attemptAddRandomInnateEnchantment(ItemRegistry.Create("(W)15"), random);
        case 3:
          return ItemRegistry.Create("(B)510");
        case 4:
          return (Item) new Ring("517");
        case 5:
          return (Item) new Ring("519");
        case 6:
          return MeleeWeapon.attemptAddRandomInnateEnchantment(ItemRegistry.Create("(W)27"), random);
      }
    }
    else if (level < 80 /*0x50*/)
    {
      switch (random.Next(7))
      {
        case 0:
          return MeleeWeapon.attemptAddRandomInnateEnchantment(ItemRegistry.Create("(W)26"), random);
        case 1:
          return MeleeWeapon.attemptAddRandomInnateEnchantment(ItemRegistry.Create("(W)27"), random);
        case 2:
          return ItemRegistry.Create("(B)508");
        case 3:
          return ItemRegistry.Create("(B)510");
        case 4:
          return (Item) new Ring("517");
        case 5:
          return (Item) new Ring("519");
        case 6:
          return MeleeWeapon.attemptAddRandomInnateEnchantment(ItemRegistry.Create("(W)19"), random);
      }
    }
    else if (level < 100)
    {
      switch (random.Next(8))
      {
        case 0:
          return MeleeWeapon.attemptAddRandomInnateEnchantment(ItemRegistry.Create("(W)48"), random);
        case 1:
          return MeleeWeapon.attemptAddRandomInnateEnchantment(ItemRegistry.Create("(W)48"), random);
        case 2:
          return ItemRegistry.Create("(B)511");
        case 3:
          return ItemRegistry.Create("(B)513");
        case 4:
          return MeleeWeapon.attemptAddRandomInnateEnchantment(ItemRegistry.Create("(W)18"), random);
        case 5:
          return MeleeWeapon.attemptAddRandomInnateEnchantment(ItemRegistry.Create("(W)28"), random);
        case 6:
          return MeleeWeapon.attemptAddRandomInnateEnchantment(ItemRegistry.Create("(W)52"), random);
        case 7:
          MeleeWeapon forThisMineLevel1 = (MeleeWeapon) MeleeWeapon.attemptAddRandomInnateEnchantment(ItemRegistry.Create("(W)3"), random);
          forThisMineLevel1.AddEnchantment((BaseEnchantment) new CrusaderEnchantment());
          return (Item) forThisMineLevel1;
      }
    }
    else if (level < 120)
    {
      switch (random.Next(8))
      {
        case 0:
          return MeleeWeapon.attemptAddRandomInnateEnchantment(ItemRegistry.Create("(W)19"), random);
        case 1:
          return MeleeWeapon.attemptAddRandomInnateEnchantment(ItemRegistry.Create("(W)50"), random);
        case 2:
          return ItemRegistry.Create("(B)511");
        case 3:
          return ItemRegistry.Create("(B)513");
        case 4:
          return MeleeWeapon.attemptAddRandomInnateEnchantment(ItemRegistry.Create("(W)18"), random);
        case 5:
          return MeleeWeapon.attemptAddRandomInnateEnchantment(ItemRegistry.Create("(W)46"), random);
        case 6:
          return (Item) new Ring("887");
        case 7:
          MeleeWeapon forThisMineLevel2 = (MeleeWeapon) MeleeWeapon.attemptAddRandomInnateEnchantment(ItemRegistry.Create("(W)3"), random);
          forThisMineLevel2.AddEnchantment((BaseEnchantment) new CrusaderEnchantment());
          return (Item) forThisMineLevel2;
      }
    }
    else
    {
      switch (random.Next(12))
      {
        case 0:
          return MeleeWeapon.attemptAddRandomInnateEnchantment(ItemRegistry.Create("(W)45"), random);
        case 1:
          return MeleeWeapon.attemptAddRandomInnateEnchantment(ItemRegistry.Create("(W)50"), random);
        case 2:
          return ItemRegistry.Create("(B)511");
        case 3:
          return ItemRegistry.Create("(B)513");
        case 4:
          return MeleeWeapon.attemptAddRandomInnateEnchantment(ItemRegistry.Create("(W)18"), random);
        case 5:
          return MeleeWeapon.attemptAddRandomInnateEnchantment(ItemRegistry.Create("(W)28"), random);
        case 6:
          return MeleeWeapon.attemptAddRandomInnateEnchantment(ItemRegistry.Create("(W)52"), random);
        case 7:
          return ItemRegistry.Create("(O)787");
        case 8:
          return ItemRegistry.Create("(B)878");
        case 9:
          return ItemRegistry.Create("(O)856");
        case 10:
          return (Item) new Ring("859");
        case 11:
          return (Item) new Ring("887");
      }
    }
    return (Item) new StardewValley.Object("78", 1);
  }

  public override bool IsLocationSpecificOccupantOnTile(Vector2 tileLocation)
  {
    return this.tileBeneathLadder.Equals(tileLocation) || this.tileBeneathElevator != Vector2.Zero && this.tileBeneathElevator.Equals(tileLocation) || base.IsLocationSpecificOccupantOnTile(tileLocation);
  }

  public bool isDarkArea()
  {
    return (this.loadedDarkArea || this.mineLevel % 40 > 30) && this.getMineArea() != 40;
  }

  public bool isTileClearForMineObjects(Vector2 v)
  {
    if (this.tileBeneathLadder.Equals(v) || this.tileBeneathElevator.Equals(v) || !this.CanItemBePlacedHere(v, ignorePassables: CollisionMask.None) || this.IsTileOccupiedBy(v, CollisionMask.Characters) || this.IsTileOccupiedBy(v, CollisionMask.Flooring | CollisionMask.TerrainFeatures))
      return false;
    switch (this.doesTileHaveProperty((int) v.X, (int) v.Y, "Type", "Back"))
    {
      case "Stone":
        return this.isTileOnClearAndSolidGround(v) && !this.objects.ContainsKey(v) && !Utility.PointToVector2(this.calicoStatueSpot.Value).Equals(v);
      default:
        return false;
    }
  }

  public override string getFootstepSoundReplacement(string footstep)
  {
    return this.GetAdditionalDifficulty() > 0 && this.getMineArea() == 40 && this.mineLevel % 40 < 30 && footstep == "stoneStep" ? "grassyStep" : base.getFootstepSoundReplacement(footstep);
  }

  public bool isTileOnClearAndSolidGround(Vector2 v)
  {
    return this.hasTileAt((int) v.X, (int) v.Y, "Back") && !this.hasTileAt((int) v.X, (int) v.Y, "Front") && !this.hasTileAt((int) v.X, (int) v.Y, "Buildings") && this.getTileIndexAt((int) v.X, (int) v.Y, "Back", "mine") != 77;
  }

  public bool isTileOnClearAndSolidGround(int x, int y)
  {
    return this.hasTileAt(x, y, "Back") && !this.hasTileAt(x, y, "Front") && this.getTileIndexAt(x, y, "Back", "mine") != 77;
  }

  public bool isTileClearForMineObjects(int x, int y)
  {
    return this.isTileClearForMineObjects(new Vector2((float) x, (float) y));
  }

  public void loadLevel(int level)
  {
    this.forceFirstTime = false;
    this.hasAddedDesertFestivalStatue = false;
    this.isMonsterArea = false;
    this.isSlimeArea = false;
    this.loadedDarkArea = false;
    this.isQuarryArea = false;
    this.isDinoArea = false;
    this.mineLoader.Unload();
    this.mineLoader.Dispose();
    this.mineLoader = Game1.content.CreateTemporary();
    if (Utility.GetDayOfPassiveFestival("DesertFestival") > 0 && Game1.IsMasterGame && level > MineShaft.deepestLevelOnCurrentDesertFestivalRun && this.getMineArea() == 121)
    {
      if (level % 5 == 0)
        ++Game1.player.team.calicoEggSkullCavernRating.Value;
      MineShaft.deepestLevelOnCurrentDesertFestivalRun = level;
    }
    bool flag = false;
    int num1 = -1;
    if (this.forceLayout.HasValue)
    {
      num1 = this.forceLayout.Value;
      string assetName = "Maps\\Mines\\" + num1.ToString();
      if (!this.mapContent.DoesAssetExist<Map>(assetName))
      {
        Game1.log.Warn($"Can't force mine layout to {num1} because there's no '{assetName}' asset, falling back to default logic.");
        num1 = -1;
      }
    }
    if (num1 < 0)
    {
      num1 = level % 40 % 20 != 0 || level % 40 == 0 ? (level % 10 == 0 ? 10 : level) : 20;
      num1 %= 40;
      if (level == 120)
        num1 = 120;
      if (this.getMineArea(level) == 121)
      {
        MineShaft mineShaft = (MineShaft) null;
        foreach (MineShaft activeMine in MineShaft.activeMines)
        {
          if (activeMine != null && activeMine.mineLevel > 120 && activeMine.mineLevel < level && (mineShaft == null || activeMine.mineLevel > mineShaft.mineLevel))
            mineShaft = activeMine;
        }
        num1 = this.mineRandom.Next(40);
        while (true)
        {
          int num2 = num1;
          int? loadedMapNumber = mineShaft?.loadedMapNumber;
          int valueOrDefault = loadedMapNumber.GetValueOrDefault();
          if (num2 == valueOrDefault & loadedMapNumber.HasValue)
            num1 = this.mineRandom.Next(40);
          else
            break;
        }
        while (num1 % 5 == 0)
          num1 = this.mineRandom.Next(40);
        if (this.isForcedChestLevel(level))
          num1 = 10;
        else if (level >= 130)
        {
          double num3 = 0.01 + (Game1.player.team.AverageDailyLuck((GameLocation) this) / 10.0 + Game1.player.team.AverageLuckLevel((GameLocation) this) / 100.0);
          if (Game1.random.NextDouble() < num3)
          {
            this.netIsTreasureRoom.Value = true;
            num1 = 10;
          }
        }
      }
      else if (this.getMineArea() == 77377 && this.mineLevel == 77377)
        num1 = 77377;
      if (MineShaft.lowestLevelReached >= 120 && num1 != 10 && num1 % 5 != 0 && this.mineLevel > 1 && this.mineLevel != 77377)
      {
        Random daySaveRandom = Utility.CreateDaySaveRandom((double) (1293857 + this.mineLevel * 400));
        double num4 = 0.06;
        if (this.mineLevel > 120)
          num4 += Math.Min(0.06, (double) this.mineLevel / 10000.0);
        if (daySaveRandom.NextDouble() < num4)
        {
          int[] source = new int[4]{ 40, 47, 50, 51 };
          num1 = daySaveRandom.Next(40, 61);
          if (((IEnumerable<int>) source).Contains<int>(num1) && daySaveRandom.NextDouble() < 0.75)
            num1 = daySaveRandom.Next(40, 61);
          if (num1 == 53 && this.getMineArea() != 121)
            num1 = daySaveRandom.Next(52, 61);
          if (num1 == 40 && this.getMineArea() != 0 && this.getMineArea() != 80 /*0x50*/)
            num1 = daySaveRandom.Next(52, 61);
          if (((IEnumerable<int>) source).Contains<int>(num1))
            flag = true;
        }
      }
    }
    this.mapPath.Value = "Maps\\Mines\\" + num1.ToString();
    this.loadedMapNumber = num1;
    this.updateMap();
    Random daySaveRandom1 = Utility.CreateDaySaveRandom((double) (level * 100));
    if ((!this.AnyOnlineFarmerHasBuff("23") || this.getMineArea() == 121) && daySaveRandom1.NextDouble() < 0.044 && num1 % 5 != 0 && num1 % 40 > 5 && num1 % 40 < 30 && num1 % 40 != 19 && !flag)
    {
      if (daySaveRandom1.NextBool())
        this.isMonsterArea = true;
      else
        this.isSlimeArea = true;
      if (this.getMineArea() == 121 && this.mineLevel > 126 && daySaveRandom1.NextBool())
      {
        this.isDinoArea = true;
        this.isSlimeArea = false;
        this.isMonsterArea = false;
      }
    }
    else if (this.mineLevel < 121 && daySaveRandom1.NextDouble() < 0.044 && Utility.doesMasterPlayerHaveMailReceivedButNotMailForTomorrow("ccCraftsRoom") && Game1.MasterPlayer.hasOrWillReceiveMail("VisitedQuarryMine") && num1 % 40 > 1 && num1 % 5 != 0)
    {
      this.isQuarryArea = true;
      if (daySaveRandom1.NextDouble() < 0.25 && !flag)
        this.isMonsterArea = true;
    }
    if (this.isQuarryArea || this.getMineArea(level) == 77377)
    {
      this.mapImageSource.Value = "Maps\\Mines\\mine_quarryshaft";
      int num5 = this.map.Layers[0].LayerWidth * this.map.Layers[0].LayerHeight / 100;
      this.isQuarryArea = true;
      this.isSlimeArea = false;
      this.isMonsterArea = false;
      this.isDinoArea = false;
      for (int index = 0; index < num5; ++index)
        this.brownSpots.Add(new Vector2((float) this.mineRandom.Next(0, this.map.Layers[0].LayerWidth), (float) this.mineRandom.Next(0, this.map.Layers[0].LayerHeight)));
    }
    else if (this.isDinoArea)
      this.mapImageSource.Value = "Maps\\Mines\\mine_dino";
    else if (this.isSlimeArea)
      this.mapImageSource.Value = "Maps\\Mines\\mine_slime";
    else if (this.getMineArea() == 0 || this.getMineArea() == 10 || this.getMineArea(level) != 0 && this.getMineArea(level) != 10)
    {
      if (this.getMineArea(level) == 40)
      {
        this.mapImageSource.Value = "Maps\\Mines\\mine_frost";
        if (level >= 70)
        {
          this.mapImageSource.Value += "_dark";
          this.loadedDarkArea = true;
        }
      }
      else if (this.getMineArea(level) == 80 /*0x50*/)
      {
        this.mapImageSource.Value = "Maps\\Mines\\mine_lava";
        if (level >= 110 && level != 120)
        {
          this.mapImageSource.Value += "_dark";
          this.loadedDarkArea = true;
        }
      }
      else if (this.getMineArea(level) == 121)
      {
        this.mapImageSource.Value = "Maps\\Mines\\mine_desert";
        if (num1 % 40 >= 30)
        {
          this.mapImageSource.Value += "_dark";
          this.loadedDarkArea = true;
        }
      }
    }
    if (num1 == 45)
    {
      this.loadedDarkArea = true;
      if (this.mapImageSource.Value == null)
        this.mapImageSource.Value = "Maps\\Mines\\mine_dark";
      else if (!this.mapImageSource.Value.EndsWith("dark"))
        this.mapImageSource.Value += "_dark";
    }
    if (this.GetAdditionalDifficulty() > 0)
    {
      string str1 = "Maps\\Mines\\mine";
      if (this.mapImageSource.Value != null)
        str1 = this.mapImageSource.Value;
      if (str1.EndsWith("_dark"))
        str1 = str1.Remove(str1.Length - "_dark".Length);
      string str2 = str1;
      if (level % 40 >= 30)
        this.loadedDarkArea = true;
      if (this.loadedDarkArea)
        str1 += "_dark";
      string str3 = str1 + "_dangerous";
      try
      {
        this.mapImageSource.Value = str3;
        Game1.temporaryContent.Load<Texture2D>(this.mapImageSource.Value);
      }
      catch (ContentLoadException ex1)
      {
        string str4 = str2 + "_dangerous";
        try
        {
          this.mapImageSource.Value = str4;
          Game1.temporaryContent.Load<Texture2D>(this.mapImageSource.Value);
        }
        catch (ContentLoadException ex2)
        {
          string str5 = str2;
          if (this.loadedDarkArea)
            str5 += "_dark";
          try
          {
            this.mapImageSource.Value = str5;
            Game1.temporaryContent.Load<Texture2D>(this.mapImageSource.Value);
          }
          catch (ContentLoadException ex3)
          {
            this.mapImageSource.Value = str2;
          }
        }
      }
    }
    this.ApplyDiggableTileFixes();
    if (this.isSideBranch())
      return;
    MineShaft.lowestLevelReached = Math.Max(MineShaft.lowestLevelReached, level);
    if (this.mineLevel % 5 != 0 || this.getMineArea() == 121)
      return;
    this.prepareElevator();
  }

  private void addBlueFlamesToChallengeShrine()
  {
    TemporaryAnimatedSpriteList temporarySprites1 = this.temporarySprites;
    TemporaryAnimatedSprite temporaryAnimatedSprite1 = new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(536, 1945, 8, 8), new Vector2(8.75f, 5.8f) * 64f + new Vector2(32f, -32f), false, 0.0f, Color.White);
    temporaryAnimatedSprite1.interval = 50f;
    temporaryAnimatedSprite1.totalNumberOfLoops = 99999;
    temporaryAnimatedSprite1.animationLength = 4;
    temporaryAnimatedSprite1.lightId = $"Mines_{this.mineLevel}_ChallengeShrineFlames_1";
    temporaryAnimatedSprite1.id = 888;
    temporaryAnimatedSprite1.lightRadius = 2f;
    temporaryAnimatedSprite1.scale = 4f;
    temporaryAnimatedSprite1.yPeriodic = true;
    temporaryAnimatedSprite1.lightcolor = new Color(100, 0, 0);
    temporaryAnimatedSprite1.yPeriodicLoopTime = 1000f;
    temporaryAnimatedSprite1.yPeriodicRange = 4f;
    temporaryAnimatedSprite1.layerDepth = 0.04544f;
    temporarySprites1.Add(temporaryAnimatedSprite1);
    TemporaryAnimatedSpriteList temporarySprites2 = this.temporarySprites;
    TemporaryAnimatedSprite temporaryAnimatedSprite2 = new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(536, 1945, 8, 8), new Vector2(10.75f, 5.8f) * 64f + new Vector2(32f, -32f), false, 0.0f, Color.White);
    temporaryAnimatedSprite2.interval = 50f;
    temporaryAnimatedSprite2.totalNumberOfLoops = 99999;
    temporaryAnimatedSprite2.animationLength = 4;
    temporaryAnimatedSprite2.lightId = $"Mines_{this.mineLevel}_ChallengeShrineFlames_2";
    temporaryAnimatedSprite2.id = 889;
    temporaryAnimatedSprite2.lightRadius = 2f;
    temporaryAnimatedSprite2.scale = 4f;
    temporaryAnimatedSprite2.lightcolor = new Color(100, 0, 0);
    temporaryAnimatedSprite2.yPeriodic = true;
    temporaryAnimatedSprite2.yPeriodicLoopTime = 1100f;
    temporaryAnimatedSprite2.yPeriodicRange = 4f;
    temporaryAnimatedSprite2.layerDepth = 0.04544f;
    temporarySprites2.Add(temporaryAnimatedSprite2);
    Game1.playSound("fireball");
  }

  public static void CheckForQiChallengeCompletion()
  {
    if (Game1.player.deepestMineLevel < 145 || !Game1.player.hasQuest("20") || Game1.player.hasOrWillReceiveMail("QiChallengeComplete"))
      return;
    Game1.player.completeQuest("20");
    Game1.addMailForTomorrow("QiChallengeComplete");
  }

  private void prepareElevator()
  {
    Point tile = Utility.findTile((GameLocation) this, 80 /*0x50*/, "Buildings", "mine");
    this.ElevatorLightSpot = tile;
    if (tile.X < 0)
      return;
    if (this.canAdd(3, 0))
    {
      this.elevatorShouldDing.Value = true;
      this.updateMineLevelData(3);
    }
    else
      this.setMapTile(tile.X, tile.Y, 48 /*0x30*/, "Buildings", "mine");
  }

  public void enterMineShaft()
  {
    DelayedAction.playSoundAfterDelay("fallDown", 800, (GameLocation) this);
    DelayedAction.playSoundAfterDelay("clubSmash", 1800);
    Random random = Utility.CreateRandom((double) this.mineLevel, (double) Game1.uniqueIDForThisGame, (double) Game1.Date.TotalDays);
    int num = random.Next(3, 9);
    if (random.NextDouble() < 0.1)
      num = num * 2 - 1;
    if (this.mineLevel < 220 && this.mineLevel + num > 220)
      num = 220 - this.mineLevel;
    this.lastLevelsDownFallen = num;
    Game1.player.health = Math.Max(1, Game1.player.health - num * 3);
    this.isFallingDownShaft = true;
    Game1.globalFadeToBlack(new Game1.afterFadeFunction(this.afterFall), 0.045f);
    Game1.player.CanMove = false;
    Game1.player.jump();
    Game1.player.temporarilyInvincible = true;
    Game1.player.temporaryInvincibilityTimer = 0;
    Game1.player.flashDuringThisTemporaryInvincibility = false;
    Game1.player.currentTemporaryInvincibilityDuration = 700;
    if (Utility.GetDayOfPassiveFestival("DesertFestival") <= 0 || !Game1.IsMasterGame || this.lastLevelsDownFallen + this.mineLevel <= MineShaft.deepestLevelOnCurrentDesertFestivalRun || !this.isFallingDownShaft || (this.lastLevelsDownFallen + this.mineLevel) / 5 <= this.mineLevel / 5)
      return;
    Game1.player.team.calicoEggSkullCavernRating.Value += (this.lastLevelsDownFallen + this.mineLevel) / 5 - this.mineLevel / 5;
  }

  private void afterFall()
  {
    Game1.drawObjectDialogue(Game1.content.LoadString(this.lastLevelsDownFallen > 7 ? "Strings\\Locations:Mines_FallenFar" : "Strings\\Locations:Mines_Fallen", (object) this.lastLevelsDownFallen));
    Game1.messagePause = true;
    Game1.enterMine(this.mineLevel + this.lastLevelsDownFallen);
    Game1.fadeToBlackAlpha = 1f;
    Game1.player.faceDirection(2);
    Game1.player.showFrame(5);
  }

  /// <inheritdoc />
  public override bool ShouldExcludeFromNpcPathfinding() => true;

  public override bool checkAction(Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who)
  {
    if (who.IsLocalPlayer)
    {
      switch (this.getTileIndexAt(tileLocation, "Buildings", "mine"))
      {
        case 112 /*0x70*/:
          if (this.mineLevel <= 120)
          {
            Game1.activeClickableMenu = (IClickableMenu) new MineElevatorMenu();
            return true;
          }
          break;
        case 115:
          this.createQuestionDialogue(" ", new Response[2]
          {
            new Response("Leave", Game1.content.LoadString("Strings\\Locations:Mines_LeaveMine")).SetHotKey(Keys.Y),
            new Response("Do", Game1.content.LoadString("Strings\\Locations:Mines_DoNothing")).SetHotKey(Keys.Escape)
          }, "ExitMine");
          return true;
        case 173:
          Game1.enterMine(this.mineLevel + 1);
          this.playSound("stairsdown");
          return true;
        case 174:
          Response[] answerChoices = new Response[2]
          {
            new Response("Jump", Game1.content.LoadString("Strings\\Locations:Mines_ShaftJumpIn")).SetHotKey(Keys.Y),
            new Response("Do", Game1.content.LoadString("Strings\\Locations:Mines_DoNothing")).SetHotKey(Keys.Escape)
          };
          this.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:Mines_Shaft"), answerChoices, "Shaft");
          return true;
        case 194:
          this.playSound("openBox");
          this.playSound("Ship");
          ++this.map.RequireLayer("Buildings").Tiles[tileLocation].TileIndex;
          ++this.map.RequireLayer("Front").Tiles[tileLocation.X, tileLocation.Y - 1].TileIndex;
          Game1.createRadialDebris((GameLocation) this, 382, tileLocation.X, tileLocation.Y, 6, false, item: true);
          this.updateMineLevelData(2, -1);
          return true;
        case 284:
          if (this.mineLevel > 120 && this.mineLevel != 77377)
          {
            this.recentlyActivatedCalicoStatue.Value = new Point(tileLocation.X, tileLocation.Y);
            return true;
          }
          break;
        case 315:
        case 316:
        case 317:
          if (Game1.player.team.SpecialOrderRuleActive("MINE_HARD") || Game1.player.team.specialRulesRemovedToday.Contains("MINE_HARD"))
          {
            Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:ChallengeShrine_OnQiChallenge"));
            break;
          }
          if (Game1.player.team.toggleMineShrineOvernight.Value)
          {
            Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:ChallengeShrine_AlreadyActive"));
            break;
          }
          this.createQuestionDialogue(Game1.player.team.mineShrineActivated.Value ? Game1.content.LoadString("Strings\\Locations:ChallengeShrine_AlreadyHard") : Game1.content.LoadString("Strings\\Locations:ChallengeShrine_NotYetHard"), this.createYesNoResponses(), "ShrineOfChallenge");
          break;
      }
    }
    return base.checkAction(tileLocation, viewport, who);
  }

  public override string checkForBuriedItem(
    int xLocation,
    int yLocation,
    bool explosion,
    bool detectOnly,
    Farmer who)
  {
    if (this.isQuarryArea || Game1.random.NextDouble() >= 0.15)
      return "";
    string id = "(O)330";
    if (Game1.random.NextDouble() < 0.07)
    {
      if (Game1.random.NextDouble() < 0.75)
      {
        switch (Game1.random.Next(5))
        {
          case 0:
            id = "(O)96";
            break;
          case 1:
            id = who.hasOrWillReceiveMail("lostBookFound") ? (Game1.netWorldState.Value.LostBooksFound < 21 ? "(O)102" : "(O)770") : "(O)770";
            break;
          case 2:
            id = "(O)110";
            break;
          case 3:
            id = "(O)112";
            break;
          case 4:
            id = "(O)585";
            break;
        }
      }
      else if (Game1.random.NextDouble() < 0.75)
      {
        switch (this.getMineArea())
        {
          case 0:
          case 10:
            id = Game1.random.Choose<string>("(O)121", "(O)97");
            break;
          case 40:
            id = Game1.random.Choose<string>("(O)122", "(O)336");
            break;
          case 80 /*0x50*/:
            id = "(O)99";
            break;
        }
      }
      else
        id = Game1.random.Choose<string>("(O)126", "(O)127");
    }
    else if (Game1.random.NextDouble() < 0.19)
      id = Game1.random.NextBool() ? "(O)390" : this.getOreIdForLevel(this.mineLevel, Game1.random);
    else if (Game1.random.NextDouble() < 0.45)
      id = "(O)330";
    else if (Game1.random.NextDouble() < 0.12)
    {
      if (Game1.random.NextDouble() < 0.25)
      {
        id = "(O)749";
      }
      else
      {
        switch (this.getMineArea())
        {
          case 0:
          case 10:
            id = "(O)535";
            break;
          case 40:
            id = "(O)536";
            break;
          case 80 /*0x50*/:
            id = "(O)537";
            break;
        }
      }
    }
    else
      id = "(O)78";
    Game1.createObjectDebris(id, xLocation, yLocation, who.UniqueMultiplayerID, (GameLocation) this);
    int num1 = !(who?.CurrentTool is Hoe) ? 0 : (who.CurrentTool.hasEnchantmentOfType<GenerousEnchantment>() ? 1 : 0);
    float num2 = 0.25f;
    if (num1 != 0 && Game1.random.NextDouble() < (double) num2)
      Game1.createObjectDebris(id, xLocation, yLocation, who.UniqueMultiplayerID, (GameLocation) this);
    return "";
  }

  public override void drawAboveAlwaysFrontLayer(SpriteBatch b)
  {
    base.drawAboveAlwaysFrontLayer(b);
    b.End();
    b.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp);
    foreach (NPC character in this.characters)
    {
      if (character is Monster monster)
        monster.drawAboveAllLayers(b);
    }
    b.End();
    b.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
    if ((double) this.fogAlpha > 0.0 || this.ambientFog)
    {
      Vector2 position = new Vector2();
      float num1 = (float) ((int) ((double) this.fogPos.X % 256.0) - 256 /*0x0100*/);
      while (true)
      {
        double num2 = (double) num1;
        Viewport viewport = Game1.graphics.GraphicsDevice.Viewport;
        double width = (double) viewport.Width;
        if (num2 < width)
        {
          float num3 = (float) ((int) ((double) this.fogPos.Y % 256.0) - 256 /*0x0100*/);
          while (true)
          {
            double num4 = (double) num3;
            viewport = Game1.graphics.GraphicsDevice.Viewport;
            double height = (double) viewport.Height;
            if (num4 < height)
            {
              position.X = (float) (int) num1;
              position.Y = (float) (int) num3;
              b.Draw(Game1.mouseCursors, position, new Microsoft.Xna.Framework.Rectangle?(this.fogSource), (double) this.fogAlpha > 0.0 ? this.fogColor * this.fogAlpha : this.fogColor, 0.0f, Vector2.Zero, 4.001f, SpriteEffects.None, 1f);
              num3 += 256f;
            }
            else
              break;
          }
          num1 += 256f;
        }
        else
          break;
      }
    }
    if (Game1.game1.takingMapScreenshot || this.isSideBranch())
      return;
    Color color1 = this.getMineArea() == 0 || this.isDarkArea() && this.getMineArea() != 121 ? SpriteText.color_White : (this.getMineArea() == 10 ? SpriteText.color_Green : (this.getMineArea() == 40 ? SpriteText.color_Cyan : (this.getMineArea() == 80 /*0x50*/ ? SpriteText.color_Red : SpriteText.color_Purple)));
    string s1 = (this.mineLevel + (this.getMineArea() == 121 ? -120 : 0)).ToString() ?? "";
    Microsoft.Xna.Framework.Rectangle titleSafeArea = Game1.game1.GraphicsDevice.Viewport.GetTitleSafeArea();
    int heightOfString = SpriteText.getHeightOfString(s1);
    SpriteText.drawString(b, s1, titleSafeArea.Left + 16 /*0x10*/, titleSafeArea.Top + 16 /*0x10*/, height: heightOfString, layerDepth: 1f, drawBGScroll: 2, color: new Color?(color1));
    int widthOfString = SpriteText.getWidthOfString(s1);
    if (this.mustKillAllMonstersToAdvance())
      b.Draw(Game1.mouseCursors, new Vector2((float) (titleSafeArea.Left + 16 /*0x10*/ + widthOfString + 16 /*0x10*/), (float) (titleSafeArea.Top + 16 /*0x10*/)) + new Vector2(4f, 6f) * 4f, new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(192 /*0xC0*/, 324, 7, 10)), Color.White, 0.0f, new Vector2(3f, 5f), (float) (4.0 + (double) Game1.dialogueButtonScale / 25.0), SpriteEffects.None, 1f);
    if (Utility.GetDayOfPassiveFestival("DesertFestival") <= 0)
      return;
    int num5 = 0;
    foreach (IClickableMenu onScreenMenu in (IEnumerable<IClickableMenu>) Game1.onScreenMenus)
    {
      if (onScreenMenu is BuffsDisplay buffsDisplay)
        num5 = buffsDisplay.getNumBuffs();
    }
    Viewport viewport1 = Game1.graphics.GraphicsDevice.Viewport;
    double width1 = (double) viewport1.Width;
    viewport1 = Game1.graphics.GraphicsDevice.Viewport;
    double num6 = 300.0 * ((double) viewport1.Width / (double) Game1.uiViewport.Width);
    Vector2 position1 = new Vector2((float) (width1 - num6 - 100.0), (float) (titleSafeArea.Top + 64 /*0x40*/ + 16 /*0x10*/ + (num5 - 1) / 5 * 16 /*0x10*/ * 4)) + new Vector2(4f, 6f) * 4f;
    if ((double) this.calicoEggIconTimerShake > 0.0)
    {
      position1 += new Vector2((float) Game1.random.Next(-4, 5), (float) Game1.random.Next(-4, 5));
      b.DrawString(Game1.dialogueFont, "+1", position1 + new Vector2(position1.X - 32f, position1.Y + 32f), Color.White);
    }
    b.Draw(Game1.mouseCursors_1_6, position1, new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, 0, 19, 21)), Color.White, 0.0f, new Vector2(3f, 5f), 4f, SpriteEffects.None, 1f);
    SpriteBatch b1 = b;
    int num7 = Game1.player.team.calicoEggSkullCavernRating.Value + 1;
    string s2 = num7.ToString() ?? "";
    int num8 = (int) position1.X + 28;
    num7 = Game1.player.team.calicoEggSkullCavernRating.Value + 1;
    int num9 = SpriteText.getWidthOfString(num7.ToString() ?? "") / 2;
    int x = num8 - num9;
    int y = (int) position1.Y + 4;
    Color? color2 = new Color?();
    SpriteText.drawString(b1, s2, x, y, color: color2);
  }

  /// <inheritdoc />
  public override void checkForMusic(GameTime time)
  {
    if (Game1.player.freezePause > 0 || this.isFogUp.Value || this.mineLevel == 120)
      return;
    string newTrackName = (string) null;
    switch (this.getMineArea())
    {
      case 0:
      case 10:
      case 121:
      case 77377:
        newTrackName = "Upper_Ambient";
        break;
      case 40:
        newTrackName = "Frost_Ambient";
        break;
      case 80 /*0x50*/:
        newTrackName = "Lava_Ambient";
        break;
    }
    if (this.GetAdditionalDifficulty() > 0 && this.getMineArea() == 40 && this.mineLevel < 70)
      newTrackName = "jungle_ambience";
    if (Game1.getMusicTrackName() == "none" || Game1.isMusicContextActiveButNotPlaying() || Game1.getMusicTrackName().EndsWith("_Ambient") && Game1.getMusicTrackName() != newTrackName)
      Game1.changeMusicTrack(newTrackName);
    MineShaft.timeSinceLastMusic = Math.Min(335000, MineShaft.timeSinceLastMusic + time.ElapsedGameTime.Milliseconds);
  }

  public string getMineSong()
  {
    if (this.mineLevel < 40)
      return "EarthMine";
    if (this.mineLevel < 80 /*0x50*/)
      return "FrostMine";
    return this.getMineArea() == 121 && Game1.random.NextDouble() >= 0.75 ? "EarthMine" : "LavaMine";
  }

  public int GetAdditionalDifficulty()
  {
    if (this.mineLevel == 77377)
      return 0;
    return this.mineLevel > 120 ? Game1.netWorldState.Value.SkullCavesDifficulty : Game1.netWorldState.Value.MinesDifficulty;
  }

  public bool isPlayingSongFromDifferentArea()
  {
    return Game1.getMusicTrackName() != this.getMineSong() && Game1.getMusicTrackName().EndsWith("Mine");
  }

  public void playMineSong()
  {
    string mineSong = this.getMineSong();
    if (!(Game1.getMusicTrackName() == "none") && !Game1.isMusicContextActiveButNotPlaying() && !Game1.getMusicTrackName().Contains("Ambient") || this.isDarkArea() || this.mineLevel == 77377)
      return;
    Game1.changeMusicTrack(mineSong);
    MineShaft.timeSinceLastMusic = 0;
  }

  protected override void resetLocalState()
  {
    this.addLevelChests();
    base.resetLocalState();
    if (Game1.IsPlayingBackgroundMusic)
      Game1.changeMusicTrack("none");
    if (this.elevatorShouldDing.Value)
      this.timeUntilElevatorLightUp = 1500;
    else if (this.mineLevel % 5 == 0 && this.getMineArea() != 121)
      this.setElevatorLit();
    if (!this.isSideBranch(this.mineLevel))
    {
      Game1.player.deepestMineLevel = Math.Max(Game1.player.deepestMineLevel, this.mineLevel);
      if (Game1.player.team.specialOrders != null)
      {
        foreach (SpecialOrder specialOrder in Game1.player.team.specialOrders)
        {
          Action<Farmer, int> mineFloorReached = specialOrder.onMineFloorReached;
          if (mineFloorReached != null)
            mineFloorReached(Game1.player, this.mineLevel);
        }
      }
      Game1.player.autoGenerateActiveDialogueEvent("mineArea_" + this.getMineArea().ToString());
    }
    if (this.mineLevel == 77377)
      Game1.addMailForTomorrow("VisitedQuarryMine", true, true);
    if (this.getMineArea() == 121 && Game1.player.team.calicoStatueEffects.ContainsKey(10) && !Game1.player.hasBuff("CalicoStatueSpeed"))
      DesertFestival.addCalicoStatueSpeedBuff();
    MineShaft.CheckForQiChallengeCompletion();
    if (this.mineLevel == 120)
      ++Game1.player.timesReachedMineBottom;
    Vector2 vector2 = this.mineEntrancePosition(Game1.player);
    Game1.xLocationAfterWarp = (int) vector2.X;
    Game1.yLocationAfterWarp = (int) vector2.Y;
    if (Game1.IsClient)
      Game1.player.Position = new Vector2((float) (Game1.xLocationAfterWarp * 64 /*0x40*/), (float) (Game1.yLocationAfterWarp * 64 /*0x40*/ - (Game1.player.Sprite.getHeight() - 32 /*0x20*/) + 16 /*0x10*/));
    this.forceViewportPlayerFollow = true;
    switch (this.mineLevel)
    {
      case 20:
        if (!Game1.IsMultiplayer && this.IsRainingHere() && Game1.player.eventsSeen.Contains("901756"))
        {
          this.characters.Clear();
          NPC npc1 = new NPC(new AnimatedSprite("Characters\\Abigail", 0, 16 /*0x10*/, 32 /*0x20*/), new Vector2(896f, 644f), "SeedShop", 3, "AbigailMine", true, Game1.content.Load<Texture2D>("Portraits\\Abigail"));
          npc1.displayName = NPC.GetDisplayName("Abigail");
          NPC npc2 = npc1;
          Random random = Utility.CreateRandom((double) Game1.stats.DaysPlayed);
          if (Game1.player.mailReceived.Add("AbigailInMineFirst"))
          {
            npc2.setNewDialogue("Strings\\Characters:AbigailInMineFirst");
            npc2.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
            {
              new FarmerSprite.AnimationFrame(0, 300),
              new FarmerSprite.AnimationFrame(1, 300),
              new FarmerSprite.AnimationFrame(2, 300),
              new FarmerSprite.AnimationFrame(3, 300)
            });
          }
          else if (random.NextDouble() < 0.15)
          {
            npc2.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
            {
              new FarmerSprite.AnimationFrame(16 /*0x10*/, 500),
              new FarmerSprite.AnimationFrame(17, 500),
              new FarmerSprite.AnimationFrame(18, 500),
              new FarmerSprite.AnimationFrame(19, 500)
            });
            npc2.setNewDialogue("Strings\\Characters:AbigailInMineFlute");
            Game1.changeMusicTrack("AbigailFlute");
          }
          else
          {
            npc2.setNewDialogue("Strings\\Characters:AbigailInMine" + random.Next(5).ToString());
            npc2.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
            {
              new FarmerSprite.AnimationFrame(0, 300),
              new FarmerSprite.AnimationFrame(1, 300),
              new FarmerSprite.AnimationFrame(2, 300),
              new FarmerSprite.AnimationFrame(3, 300)
            });
          }
          this.characters.Add(npc2);
          break;
        }
        break;
      case 120:
        if (this.GetAdditionalDifficulty() > 0 && !Game1.player.hasOrWillReceiveMail("reachedBottomOfHardMines"))
          Game1.addMailForTomorrow("reachedBottomOfHardMines", true, true);
        if (this.GetAdditionalDifficulty() > 0)
          Game1.getAchievement(41);
        if (Game1.player.hasOrWillReceiveMail("reachedBottomOfHardMines"))
        {
          this.setMapTile(9, 6, 315, "Buildings", "mine", "None");
          this.setMapTile(10, 6, 316, "Buildings", "mine", "None");
          this.setMapTile(11, 6, 317, "Buildings", "mine", "None");
          this.setMapTile(9, 5, 299, "Front", "mine");
          this.setMapTile(10, 5, 300, "Front", "mine");
          this.setMapTile(11, 5, 301, "Front", "mine");
          if (Game1.player.team.mineShrineActivated.Value && !Game1.player.team.toggleMineShrineOvernight.Value || !Game1.player.team.mineShrineActivated.Value && Game1.player.team.toggleMineShrineOvernight.Value)
          {
            DelayedAction.functionAfterDelay(new Action(this.addBlueFlamesToChallengeShrine), 1000);
            break;
          }
          break;
        }
        break;
    }
    this.ApplyDiggableTileFixes();
    if (this.isMonsterArea || this.isSlimeArea)
    {
      Random random = Utility.CreateRandom((double) Game1.stats.DaysPlayed);
      Game1.showGlobalMessage(Game1.content.LoadString("Strings\\Locations:Mines_" + random.Choose<string>("Infested", "Overrun")));
    }
    int num = this.mineLevel % 20 == 0 ? 1 : 0;
    bool flag = false;
    if (num != 0)
    {
      this.waterTiles = new WaterTiles(this.map.Layers[0].LayerWidth, this.map.Layers[0].LayerHeight);
      this.waterColor.Value = this.getMineArea() == 80 /*0x50*/ ? Color.Red * 0.8f : new Color(50, 100, 200) * 0.5f;
      for (int index1 = 0; index1 < this.map.RequireLayer("Buildings").LayerHeight; ++index1)
      {
        for (int index2 = 0; index2 < this.map.RequireLayer("Buildings").LayerWidth; ++index2)
        {
          string str = this.doesTileHaveProperty(index2, index1, "Water", "Back");
          if (str != null)
          {
            flag = true;
            if (str == "I")
              this.waterTiles.waterTiles[index2, index1] = new WaterTiles.WaterTileData(true, false);
            else
              this.waterTiles[index2, index1] = true;
            if (this.getMineArea() == 80 /*0x50*/ && Game1.random.NextDouble() < 0.1)
              this.sharedLights.AddLight(new LightSource($"Mines_{this.mineLevel}_{index2}_{index1}_Lava", 4, new Vector2((float) index2, (float) index1) * 64f, 2f, new Color(0, 220, 220), onlyLocation: this.NameOrUniqueName));
          }
        }
      }
    }
    if (!flag)
      this.waterTiles = (WaterTiles) null;
    if (this.getMineArea(this.mineLevel) != this.getMineArea(this.mineLevel - 1) || this.mineLevel == 120 || this.isPlayingSongFromDifferentArea())
      Game1.changeMusicTrack("none");
    if (this.GetAdditionalDifficulty() > 0 && this.mineLevel == 70)
      Game1.changeMusicTrack("none");
    if (this.mineLevel == 77377 && Game1.player.mailReceived.Contains("gotGoldenScythe"))
    {
      this.setMapTile(29, 4, 245, "Front", "mine");
      this.setMapTile(30, 4, 246, "Front", "mine");
      this.setMapTile(29, 5, 261, "Front", "mine");
      this.setMapTile(30, 5, 262, "Front", "mine");
      this.setMapTile(29, 6, 277, "Buildings", "mine");
      this.setMapTile(30, 56, 278, "Buildings", "mine");
    }
    if (this.calicoStatueSpot.Value != Point.Zero)
    {
      if (this.recentlyActivatedCalicoStatue.Value != Point.Zero)
      {
        this.setMapTile(this.calicoStatueSpot.X, this.calicoStatueSpot.Y, 285, "Buildings", "mine");
        this.setMapTile(this.calicoStatueSpot.X, this.calicoStatueSpot.Y - 1, 269, "Front", "mine");
        this.setMapTile(this.calicoStatueSpot.X, this.calicoStatueSpot.Y - 2, 253, "Front", "mine");
      }
      else
      {
        this.setMapTile(this.calicoStatueSpot.X, this.calicoStatueSpot.Y, 284, "Buildings", "mine");
        this.setMapTile(this.calicoStatueSpot.X, this.calicoStatueSpot.Y - 1, 268, "Front", "mine");
        this.setMapTile(this.calicoStatueSpot.X, this.calicoStatueSpot.Y - 2, 252, "Front", "mine");
      }
    }
    if (this.mineLevel <= 1 || this.mineLevel != 2 && (this.mineLevel % 5 == 0 || MineShaft.timeSinceLastMusic <= 150000 || !Game1.random.NextBool()))
      return;
    this.playMineSong();
  }

  public virtual void ApplyDiggableTileFixes()
  {
    if (this.map == null || this.GetAdditionalDifficulty() > 0 && this.getMineArea() != 40 && this.isDarkArea())
      return;
    TileSheet tileSheet = this.map.RequireTileSheet(0, "mine");
    tileSheet.TileIndexProperties[165].TryAdd("Diggable", "true");
    tileSheet.TileIndexProperties[181].TryAdd("Diggable", "true");
    tileSheet.TileIndexProperties[183].TryAdd("Diggable", "true");
  }

  public void createLadderDown(int x, int y, bool forceShaft = false)
  {
    this.createLadderDownEvent[new Point(x, y)] = forceShaft || this.getMineArea() == 121 && !this.mustKillAllMonstersToAdvance() && this.mineRandom.NextDouble() < 0.2;
  }

  private void doCreateLadderDown(Point point, bool shaft)
  {
    this.updateMap();
    int x = point.X;
    int y = point.Y;
    Layer layer = this.map.RequireLayer("Buildings");
    TileSheet tileSheet = this.map.RequireTileSheet(0, "mine");
    if (shaft)
    {
      layer.Tiles[x, y] = (Tile) new StaticTile(layer, tileSheet, BlendMode.Alpha, 174);
    }
    else
    {
      this.ladderHasSpawned = true;
      layer.Tiles[x, y] = (Tile) new StaticTile(layer, tileSheet, BlendMode.Alpha, 173);
    }
    if (Game1.player.currentLocation != this)
      return;
    Game1.player.TemporaryPassableTiles.Add(new Microsoft.Xna.Framework.Rectangle(x * 64 /*0x40*/, y * 64 /*0x40*/, 64 /*0x40*/, 64 /*0x40*/));
  }

  public void checkStoneForItems(string stoneId, int x, int y, Farmer who)
  {
    long uniqueMultiplayerId = who != null ? who.UniqueMultiplayerID : 0L;
    int luckLevel = who != null ? who.LuckLevel : 0;
    double num1 = (who != null ? who.DailyLuck : 0.0) / 2.0 + (double) (who != null ? who.MiningLevel : 0) * 0.005 + (double) luckLevel * 0.001;
    Random daySaveRandom = Utility.CreateDaySaveRandom((double) (x * 1000), (double) y, (double) this.mineLevel);
    daySaveRandom.NextDouble();
    int num2;
    double num3;
    if (!(stoneId == 40.ToString()))
    {
      string str1 = stoneId;
      num2 = 42;
      string str2 = num2.ToString();
      if (!(str1 == str2))
      {
        num3 = 0.8;
        goto label_4;
      }
    }
    num3 = 1.2;
label_4:
    double num4 = num3;
    num2 = this.stonesLeftOnThisLevel--;
    double num5 = 0.02 + 1.0 / (double) Math.Max(1, this.stonesLeftOnThisLevel) + (double) luckLevel / 100.0 + Game1.player.DailyLuck / 5.0;
    if (this.EnemyCount == 0)
      num5 += 0.04;
    if (who != null && who.hasBuff("dwarfStatue_1"))
      num5 *= 1.25;
    if (!this.ladderHasSpawned && !this.mustKillAllMonstersToAdvance() && (this.stonesLeftOnThisLevel == 0 || daySaveRandom.NextDouble() < num5) && this.shouldCreateLadderOnThisLevel())
      this.createLadderDown(x, y);
    if (this.breakStone(stoneId, x, y, who, daySaveRandom))
      return;
    string str3 = stoneId;
    num2 = 44;
    string str4 = num2.ToString();
    if (str3 == str4)
    {
      int num6 = daySaveRandom.Next(59, 70);
      int num7 = num6 + num6 % 2;
      bool flag = false;
      foreach (Farmer allFarmer in Game1.getAllFarmers())
      {
        if (allFarmer.timesReachedMineBottom > 0)
        {
          flag = true;
          break;
        }
      }
      if (!flag)
      {
        if (this.mineLevel < 40 && num7 != 66 && num7 != 68)
          num7 = daySaveRandom.Choose<int>(66, 68);
        else if (this.mineLevel < 80 /*0x50*/ && (num7 == 64 /*0x40*/ || num7 == 60))
          num7 = daySaveRandom.Choose<int>(66, 70, 68, 62);
      }
      Game1.createObjectDebris("(O)" + num7.ToString(), x, y, uniqueMultiplayerId, (GameLocation) this);
      ++Game1.stats.OtherPreciousGemsFound;
    }
    else
    {
      int num8 = who == null || !who.professions.Contains(22) ? 1 : 2;
      double num9 = who == null || !who.hasBuff("dwarfStatue_4") ? 1.0 : 1.25;
      if (daySaveRandom.NextDouble() < 0.022 * (1.0 + num1) * (double) num8 * num9)
      {
        num2 = 535 + (this.getMineArea() == 40 ? 1 : (this.getMineArea() == 80 /*0x50*/ ? 2 : 0));
        string id = "(O)" + num2.ToString();
        if (this.getMineArea() == 121)
          id = "(O)749";
        if (who != null && who.professions.Contains(19) && daySaveRandom.NextBool())
          Game1.createObjectDebris(id, x, y, uniqueMultiplayerId, (GameLocation) this);
        Game1.createObjectDebris(id, x, y, uniqueMultiplayerId, (GameLocation) this);
        who?.gainExperience(5, 20 * this.getMineArea());
      }
      if (this.mineLevel > 20 && daySaveRandom.NextDouble() < 0.005 * (1.0 + num1) * (double) num8 * num9)
      {
        if (who != null && who.professions.Contains(19) && daySaveRandom.NextBool())
          Game1.createObjectDebris("(O)749", x, y, uniqueMultiplayerId, (GameLocation) this);
        Game1.createObjectDebris("(O)749", x, y, uniqueMultiplayerId, (GameLocation) this);
        who?.gainExperience(5, 40 * this.getMineArea());
      }
      if (daySaveRandom.NextDouble() < 0.05 * (1.0 + num1) * num4)
      {
        int num10 = who == null || !who.professions.Contains(21) ? 1 : 2;
        double num11 = who == null || !who.hasBuff("dwarfStatue_2") ? 0.0 : 0.1;
        if (daySaveRandom.NextDouble() < 0.25 * (double) num10 + num11)
        {
          Game1.createObjectDebris("(O)382", x, y, uniqueMultiplayerId, (GameLocation) this);
          Game1.multiplayer.broadcastSprites((GameLocation) this, new TemporaryAnimatedSprite(25, new Vector2((float) (64 /*0x40*/ * x), (float) (64 /*0x40*/ * y)), Color.White, flipped: Game1.random.NextBool(), animationInterval: 80f, sourceRectHeight: 128 /*0x80*/));
        }
        Game1.createObjectDebris(this.getOreIdForLevel(this.mineLevel, daySaveRandom), x, y, uniqueMultiplayerId, (GameLocation) this);
        who?.gainExperience(3, 5);
      }
      else
      {
        if (!daySaveRandom.NextBool())
          return;
        Game1.createDebris(14, x, y, 1, (GameLocation) this);
      }
    }
  }

  public string getOreIdForLevel(int mineLevel, Random r)
  {
    if (this.getMineArea(mineLevel) == 77377)
      return "(O)380";
    if (mineLevel < 40)
      return mineLevel >= 20 && r.NextDouble() < 0.1 ? "(O)380" : "(O)378";
    if (mineLevel < 80 /*0x50*/)
    {
      if (mineLevel >= 60 && r.NextDouble() < 0.1)
        return "(O)384";
      return r.NextDouble() >= 0.75 ? "(O)378" : "(O)380";
    }
    if (mineLevel < 120)
    {
      if (r.NextDouble() < 0.75)
        return "(O)384";
      return r.NextDouble() >= 0.75 ? "(O)378" : "(O)380";
    }
    if (Utility.GetDayOfPassiveFestival("DesertFestival") > 0 && r.NextDouble() < 0.13 + (double) (Game1.player.team.calicoEggSkullCavernRating.Value * 5) / 1000.0)
      return "CalicoEgg";
    if (r.NextDouble() < 0.01 + (double) (mineLevel - 120) / 2000.0)
      return "(O)386";
    if (r.NextDouble() < 0.75)
      return "(O)384";
    return r.NextDouble() >= 0.75 ? "(O)378" : "(O)380";
  }

  public bool shouldUseSnowTextureHoeDirt()
  {
    return !this.isSlimeArea && (this.GetAdditionalDifficulty() > 0 && (this.mineLevel < 40 || this.mineLevel >= 70 && this.mineLevel < 80 /*0x50*/) || this.GetAdditionalDifficulty() <= 0 && this.getMineArea() == 40);
  }

  public int getMineArea(int level = -1)
  {
    if (level == -1)
      level = this.mineLevel;
    if (this.isQuarryArea || level == 77377)
      return 77377;
    if (level >= 80 /*0x50*/ && level <= 120)
      return 80 /*0x50*/;
    if (level > 120)
      return 121;
    if (level >= 40)
      return 40;
    return level > 10 && this.mineLevel < 30 ? 10 : 0;
  }

  public bool isSideBranch(int level = -1)
  {
    if (level == -1)
      level = this.mineLevel;
    return level == 77377;
  }

  public byte getWallAt(int x, int y) => byte.MaxValue;

  public Color getLightingColor(GameTime time) => this.lighting;

  public StardewValley.Object getRandomItemForThisLevel(int level, Vector2 tile)
  {
    string itemId = "80";
    if (this.mineRandom.NextDouble() < 0.05 && level > 80 /*0x50*/)
      itemId = "422";
    else if (this.mineRandom.NextDouble() < 0.1 && level > 20 && this.getMineArea() != 40)
      itemId = "420";
    else if (this.mineRandom.NextDouble() < 0.25 || this.GetAdditionalDifficulty() > 0)
    {
      switch (this.getMineArea())
      {
        case 0:
        case 10:
          if (this.GetAdditionalDifficulty() > 0 && !this.isDarkArea())
          {
            switch (this.mineRandom.Next(6))
            {
              case 0:
              case 6:
                itemId = "152";
                break;
              case 1:
                itemId = "393";
                break;
              case 2:
                itemId = "397";
                break;
              case 3:
                itemId = "372";
                break;
              case 4:
                itemId = "392";
                break;
            }
            if (this.mineRandom.NextDouble() < 0.005)
            {
              itemId = "797";
              break;
            }
            if (this.mineRandom.NextDouble() < 0.08)
            {
              itemId = "394";
              break;
            }
            break;
          }
          itemId = "86";
          break;
        case 40:
          if (this.GetAdditionalDifficulty() > 0 && this.mineLevel % 40 < 30)
          {
            switch (this.mineRandom.Next(4))
            {
              case 0:
              case 3:
                itemId = "259";
                break;
              case 1:
                itemId = "404";
                break;
              case 2:
                itemId = "420";
                break;
            }
            if (this.mineRandom.NextDouble() < 0.08)
            {
              itemId = "422";
              break;
            }
            break;
          }
          itemId = "84";
          break;
        case 80 /*0x50*/:
          itemId = "82";
          break;
        case 121:
          itemId = this.mineRandom.NextDouble() < 0.3 ? "86" : (this.mineRandom.NextDouble() < 0.3 ? "84" : "82");
          break;
      }
    }
    else
      itemId = "80";
    if (this.isDinoArea)
    {
      itemId = "259";
      if (this.mineRandom.NextDouble() < 0.06)
        itemId = "107";
    }
    return new StardewValley.Object(itemId, 1) { IsSpawnedObject = true };
  }

  public bool shouldShowDarkHoeDirt() => this.getMineArea() != 121 || this.isDinoArea;

  public string getRandomGemRichStoneForThisLevel(int level)
  {
    int num1 = this.mineRandom.Next(59, 70);
    int num2 = num1 + num1 % 2;
    if (Game1.player.timesReachedMineBottom == 0)
    {
      if (level < 40 && num2 != 66 && num2 != 68)
        num2 = this.mineRandom.Choose<int>(66, 68);
      else if (level < 80 /*0x50*/ && (num2 == 64 /*0x40*/ || num2 == 60))
        num2 = this.mineRandom.Choose<int>(66, 70, 68, 62);
    }
    switch (num2)
    {
      case 60:
        return "12";
      case 62:
        return "14";
      case 64 /*0x40*/:
        return "4";
      case 66:
        return "8";
      case 68:
        return "10";
      case 70:
        return "6";
      default:
        return 40.ToString();
    }
  }

  public float getDistanceFromStart(int xTile, int yTile)
  {
    float val1 = Utility.distance((float) xTile, this.tileBeneathLadder.X, (float) yTile, this.tileBeneathLadder.Y);
    if (this.tileBeneathElevator != Vector2.Zero)
      val1 = Math.Min(val1, Utility.distance((float) xTile, this.tileBeneathElevator.X, (float) yTile, this.tileBeneathElevator.Y));
    return val1;
  }

  public Monster getMonsterForThisLevel(int level, int xTile, int yTile)
  {
    Vector2 vector2_1 = new Vector2((float) xTile, (float) yTile) * 64f;
    float distanceFromStart = this.getDistanceFromStart(xTile, yTile);
    if (this.isSlimeArea)
    {
      if (this.GetAdditionalDifficulty() > 0)
      {
        if (this.mineLevel < 20)
          return (Monster) new GreenSlime(vector2_1, this.mineLevel);
        if (this.mineLevel < 30)
          return (Monster) new BlueSquid(vector2_1);
        if (this.mineLevel < 40)
          return (Monster) new RockGolem(vector2_1, this);
        if (this.mineLevel < 50)
          return this.mineRandom.NextDouble() < 0.15 && (double) distanceFromStart >= 10.0 ? (Monster) new Fly(vector2_1) : (Monster) new Grub(vector2_1);
        if (this.mineLevel < 70)
          return (Monster) new Leaper(vector2_1);
      }
      else
        return this.mineRandom.NextDouble() < 0.2 ? (Monster) new BigSlime(vector2_1, this.getMineArea()) : (Monster) new GreenSlime(vector2_1, this.mineLevel);
    }
    else if (this.isDinoArea)
    {
      if (this.mineRandom.NextDouble() < 0.1)
        return (Monster) new Bat(vector2_1, 999);
      return this.mineRandom.NextDouble() < 0.1 ? (Monster) new Fly(vector2_1, true) : (Monster) new DinoMonster(vector2_1);
    }
    if (this.getMineArea() == 0 || this.getMineArea() == 10)
    {
      if (this.mineRandom.NextDouble() < 0.25 && !this.mustKillAllMonstersToAdvance())
        return (Monster) new Bug(vector2_1, this.mineRandom.Next(4), this);
      if (level < 15)
      {
        if (this.doesTileHaveProperty(xTile, yTile, "Diggable", "Back") != null)
          return (Monster) new Duggy(vector2_1);
        return this.mineRandom.NextDouble() < 0.15 ? (Monster) new RockCrab(vector2_1) : (Monster) new GreenSlime(vector2_1, level);
      }
      if (level <= 30)
      {
        if (this.doesTileHaveProperty(xTile, yTile, "Diggable", "Back") != null)
          return (Monster) new Duggy(vector2_1);
        if (this.mineRandom.NextDouble() < 0.15)
          return (Monster) new RockCrab(vector2_1);
        if (this.mineRandom.NextDouble() < 0.05 && (double) distanceFromStart > 10.0 && this.GetAdditionalDifficulty() <= 0)
          return (Monster) new Fly(vector2_1);
        if (this.mineRandom.NextDouble() < 0.45)
          return (Monster) new GreenSlime(vector2_1, level);
        if (this.GetAdditionalDifficulty() <= 0)
          return (Monster) new Grub(vector2_1);
        if ((double) distanceFromStart > 9.0)
          return (Monster) new BlueSquid(vector2_1);
        return this.mineRandom.NextDouble() < 0.01 ? (Monster) new RockGolem(vector2_1, this) : (Monster) new GreenSlime(vector2_1, level);
      }
      if (level <= 40)
      {
        if (this.mineRandom.NextDouble() < 0.1 && (double) distanceFromStart > 10.0)
          return (Monster) new Bat(vector2_1, level);
        return this.GetAdditionalDifficulty() > 0 && this.mineRandom.NextDouble() < 0.1 ? (Monster) new Ghost(vector2_1, "Carbon Ghost") : (Monster) new RockGolem(vector2_1, this);
      }
    }
    else if (this.getMineArea() == 40)
    {
      if (this.mineLevel >= 70 && (this.mineRandom.NextDouble() < 0.75 || this.GetAdditionalDifficulty() > 0))
        return this.mineRandom.NextDouble() < 0.75 || this.GetAdditionalDifficulty() <= 0 ? (Monster) new Skeleton(vector2_1, this.GetAdditionalDifficulty() > 0 && this.mineRandom.NextBool()) : (Monster) new Bat(vector2_1, 77377);
      if (this.mineRandom.NextDouble() < 0.3)
        return (Monster) new DustSpirit(vector2_1, this.mineRandom.NextDouble() < 0.8);
      if (this.mineRandom.NextDouble() < 0.3 && (double) distanceFromStart > 10.0)
        return (Monster) new Bat(vector2_1, this.mineLevel);
      if (!this.ghostAdded && this.mineLevel > 50 && this.mineRandom.NextDouble() < 0.3 && (double) distanceFromStart > 10.0)
      {
        this.ghostAdded = true;
        return this.GetAdditionalDifficulty() > 0 ? (Monster) new Ghost(vector2_1, "Putrid Ghost") : (Monster) new Ghost(vector2_1);
      }
      if (this.GetAdditionalDifficulty() > 0)
      {
        if (this.mineRandom.NextDouble() < 0.01)
        {
          RockCrab monsterForThisLevel = new RockCrab(vector2_1);
          monsterForThisLevel.makeStickBug();
          return (Monster) monsterForThisLevel;
        }
        if (this.mineLevel >= 50)
          return (Monster) new Leaper(vector2_1);
        return this.mineRandom.NextDouble() < 0.7 ? (Monster) new Grub(vector2_1) : (Monster) new GreenSlime(vector2_1, this.mineLevel);
      }
    }
    else if (this.getMineArea() == 80 /*0x50*/)
    {
      if (this.isDarkArea() && this.mineRandom.NextDouble() < 0.25)
        return (Monster) new Bat(vector2_1, this.mineLevel);
      if (this.mineRandom.NextDouble() < (this.GetAdditionalDifficulty() > 0 ? 0.05 : 0.15))
        return (Monster) new GreenSlime(vector2_1, this.getMineArea());
      if (this.mineRandom.NextDouble() < 0.15)
        return (Monster) new MetalHead(vector2_1, this.getMineArea());
      if (this.mineRandom.NextDouble() < 0.25)
        return (Monster) new ShadowBrute(vector2_1);
      if (this.GetAdditionalDifficulty() > 0 && this.mineRandom.NextDouble() < 0.25)
        return (Monster) new Shooter(vector2_1, "Shadow Sniper");
      if (this.mineRandom.NextDouble() < 0.25)
        return (Monster) new ShadowShaman(vector2_1);
      if (this.mineRandom.NextDouble() < 0.25)
        return (Monster) new RockCrab(vector2_1, "Lava Crab");
      if (this.mineRandom.NextDouble() < 0.2 && (double) distanceFromStart > 8.0 && this.mineLevel >= 90 && this.hasTileAt(xTile, yTile, "Back") && !this.hasTileAt(xTile, yTile, "Front"))
        return (Monster) new SquidKid(vector2_1);
    }
    else
    {
      if (this.getMineArea() == 121)
      {
        if (this.loadedDarkArea)
        {
          if (this.mineRandom.NextDouble() < 0.18 && (double) distanceFromStart > 8.0)
            return (Monster) new Ghost(vector2_1, "Carbon Ghost");
          Mummy monsterForThisLevel = new Mummy(vector2_1);
          if (Utility.GetDayOfPassiveFestival("DesertFestival") > 0 && this.getMineArea() == 121 && Game1.player.team.calicoStatueEffects.ContainsKey(9))
          {
            monsterForThisLevel.BuffForAdditionalDifficulty(2);
            monsterForThisLevel.speed *= 2;
            this.setMonsterTextureToDangerousVersion((Monster) monsterForThisLevel);
          }
          return (Monster) monsterForThisLevel;
        }
        if (this.mineLevel % 20 == 0 && (double) distanceFromStart > 10.0)
          return (Monster) new Bat(vector2_1, this.mineLevel);
        if (this.mineLevel % 16 /*0x10*/ == 0 && !this.mustKillAllMonstersToAdvance())
          return Utility.GetDayOfPassiveFestival("DesertFestival") > 0 && this.getMineArea() == 121 && Game1.player.team.calicoStatueEffects.ContainsKey(4) ? (Monster) new Bug(vector2_1, this.mineRandom.Next(4), "Assassin Bug") : (Monster) new Bug(vector2_1, this.mineRandom.Next(4), this);
        if (this.mineRandom.NextDouble() < 0.33 && (double) distanceFromStart > 10.0)
          return this.GetAdditionalDifficulty() <= 0 ? (Monster) new Serpent(vector2_1) : (Monster) new Serpent(vector2_1, "Royal Serpent");
        if (this.mineRandom.NextDouble() < 0.33 && (double) distanceFromStart > 10.0 && this.mineLevel >= 171)
          return (Monster) new Bat(vector2_1, this.mineLevel);
        if (this.mineLevel >= 126 && (double) distanceFromStart > 10.0 && this.mineRandom.NextDouble() < 0.04 && !this.mustKillAllMonstersToAdvance())
          return (Monster) new DinoMonster(vector2_1);
        if (this.mineRandom.NextDouble() < 0.33 && !this.mustKillAllMonstersToAdvance())
          return Utility.GetDayOfPassiveFestival("DesertFestival") > 0 && this.getMineArea() == 121 && Game1.player.team.calicoStatueEffects.ContainsKey(4) ? (Monster) new Bug(vector2_1, this.mineRandom.Next(4), "Assassin Bug") : (Monster) new Bug(vector2_1, this.mineRandom.Next(4), this);
        if (this.mineRandom.NextDouble() < 0.25)
          return (Monster) new GreenSlime(vector2_1, level);
        if (this.mineLevel >= 146 && this.mineRandom.NextDouble() < 0.25)
          return (Monster) new RockCrab(vector2_1, "Iridium Crab");
        return this.GetAdditionalDifficulty() > 0 && this.mineRandom.NextDouble() < 0.2 && (double) distanceFromStart > 8.0 && this.hasTileAt(xTile, yTile, "Back") && !this.hasTileAt(xTile, yTile, "Front") ? (Monster) new SquidKid(vector2_1) : (Monster) new BigSlime(vector2_1, this);
      }
      if (this.getMineArea() == 77377)
      {
        if (this.mineLevel == 77377 && yTile > 59 || this.mineLevel != 77377 && this.mineLevel % 2 == 0)
        {
          GreenSlime monsterForThisLevel = new GreenSlime(vector2_1, 77377);
          Vector2 vector2_2 = new Vector2((float) xTile, (float) yTile);
          bool flag = false;
          for (int index = 0; index < this.brownSpots.Count; ++index)
          {
            if ((double) Vector2.Distance(vector2_2, this.brownSpots[index]) < 4.0)
            {
              flag = true;
              break;
            }
          }
          if (flag)
          {
            int r = Game1.random.Next(120, 200);
            monsterForThisLevel.color.Value = new Color(r, r / 2, r / 4);
            while (Game1.random.NextDouble() < 0.33)
              monsterForThisLevel.objectsToDrop.Add("378");
            monsterForThisLevel.Health = (int) ((double) monsterForThisLevel.Health * 0.5);
            monsterForThisLevel.Speed += 2;
          }
          else
          {
            int num = Game1.random.Next(120, 200);
            monsterForThisLevel.color.Value = new Color(num, num, num);
            while (Game1.random.NextDouble() < 0.33)
              monsterForThisLevel.objectsToDrop.Add("380");
            monsterForThisLevel.Speed = 1;
          }
          return (Monster) monsterForThisLevel;
        }
        if (yTile < 51 || this.mineLevel != 77377)
        {
          if (xTile < 70)
            return (Monster) new Bat(vector2_1, 77377);
          Monster monster = (Monster) new Skeleton(vector2_1, Game1.random.NextBool());
          monster.BuffForAdditionalDifficulty(this.mineRandom.Next(1, 3));
          this.setMonsterTextureToDangerousVersion(monster);
          return monster;
        }
        Bat monsterForThisLevel1 = new Bat(vector2_1, 77377);
        monsterForThisLevel1.focusedOnFarmers = true;
        return (Monster) monsterForThisLevel1;
      }
    }
    return (Monster) new GreenSlime(vector2_1, level);
  }

  private StardewValley.Object createLitterObject(
    double chanceForPurpleStone,
    double chanceForMysticStone,
    double gemStoneChance,
    Vector2 tile)
  {
    Color color = Color.White;
    int num1 = 1;
    if (this.GetAdditionalDifficulty() > 0 && this.mineLevel % 5 != 0 && this.mineRandom.NextDouble() < (double) this.GetAdditionalDifficulty() * 0.001 + (double) this.mineLevel / 100000.0 + Game1.player.team.AverageDailyLuck((GameLocation) this) / 13.0 + Game1.player.team.AverageLuckLevel((GameLocation) this) * 0.0001500000071246177)
      return new StardewValley.Object("95", 1) { MinutesUntilReady = 25 };
    int num2;
    if (this.getMineArea() == 0 || this.getMineArea() == 10)
    {
      num2 = this.mineRandom.Next(31 /*0x1F*/, 42);
      if (this.mineLevel % 40 < 30 && num2 >= 33 && num2 < 38)
        num2 = this.mineRandom.Choose<int>(32 /*0x20*/, 38);
      else if (this.mineLevel % 40 >= 30)
        num2 = this.mineRandom.Choose<int>(34, 36);
      if (this.GetAdditionalDifficulty() > 0)
      {
        num2 = this.mineRandom.Next(33, 37);
        num1 = 5;
        if (Game1.random.NextDouble() < 0.33)
          num2 = 846;
        else
          color = new Color(Game1.random.Next(60, 90), Game1.random.Next(150, 200), Game1.random.Next(190, 240 /*0xF0*/));
        if (this.isDarkArea())
        {
          num2 = this.mineRandom.Next(32 /*0x20*/, 39);
          int num3 = Game1.random.Next(130, 160 /*0xA0*/);
          color = new Color(num3, num3, num3);
        }
        if (this.mineLevel != 1 && this.mineLevel % 5 != 0 && this.mineRandom.NextDouble() < 0.029)
          return new StardewValley.Object("849", 1)
          {
            MinutesUntilReady = 6
          };
        if (color.Equals(Color.White))
          return new StardewValley.Object(num2.ToString(), 1)
          {
            MinutesUntilReady = num1
          };
      }
      else if (this.mineLevel != 1 && this.mineLevel % 5 != 0 && this.mineRandom.NextDouble() < 0.029)
        return new StardewValley.Object("751", 1)
        {
          MinutesUntilReady = 3
        };
    }
    else if (this.getMineArea() == 40)
    {
      num2 = this.mineRandom.Next(47, 54);
      num1 = 3;
      if (this.GetAdditionalDifficulty() > 0 && this.mineLevel % 40 < 30)
      {
        num2 = this.mineRandom.Next(39, 42);
        num1 = 5;
        color = new Color(170, (int) byte.MaxValue, 160 /*0xA0*/);
        if (this.isDarkArea())
        {
          num2 = this.mineRandom.Next(32 /*0x20*/, 39);
          int num4 = Game1.random.Next(130, 160 /*0xA0*/);
          color = new Color(num4, num4, num4);
        }
        if (this.mineRandom.NextDouble() < 0.15)
        {
          ColoredObject litterObject = new ColoredObject((294 + this.mineRandom.Choose<int>(1, 0)).ToString(), 1, new Color(170, 140, 155));
          litterObject.MinutesUntilReady = 6;
          litterObject.CanBeSetDown = true;
          litterObject.Flipped = this.mineRandom.NextBool();
          return (StardewValley.Object) litterObject;
        }
        if (this.mineLevel != 1 && this.mineLevel % 5 != 0 && this.mineRandom.NextDouble() < 0.029)
        {
          ColoredObject litterObject = new ColoredObject("290", 1, new Color(150, 225, 160 /*0xA0*/));
          litterObject.MinutesUntilReady = 6;
          litterObject.CanBeSetDown = true;
          litterObject.Flipped = this.mineRandom.NextBool();
          return (StardewValley.Object) litterObject;
        }
        if (color.Equals(Color.White))
          return new StardewValley.Object(num2.ToString(), 1)
          {
            MinutesUntilReady = num1
          };
      }
      else if (this.mineLevel % 5 != 0 && this.mineRandom.NextDouble() < 0.029)
        return new StardewValley.Object("290", 1)
        {
          MinutesUntilReady = 4
        };
    }
    else if (this.getMineArea() == 80 /*0x50*/)
    {
      num1 = 4;
      num2 = this.mineRandom.NextDouble() >= 0.3 || this.isDarkArea() ? (this.mineRandom.NextDouble() >= 0.3 ? (!this.mineRandom.NextBool() ? 762 : 760) : this.mineRandom.Next(55, 58)) : (!this.mineRandom.NextBool() ? 32 /*0x20*/ : 38);
      if (this.GetAdditionalDifficulty() > 0)
      {
        num2 = !this.mineRandom.NextBool() ? 32 /*0x20*/ : 38;
        num1 = 5;
        color = new Color(Game1.random.Next(140, 190), Game1.random.Next(90, 120), Game1.random.Next(210, (int) byte.MaxValue));
        if (this.isDarkArea())
        {
          num2 = this.mineRandom.Next(32 /*0x20*/, 39);
          int num5 = Game1.random.Next(130, 160 /*0xA0*/);
          color = new Color(num5, num5, num5);
        }
        if (this.mineLevel != 1 && this.mineLevel % 5 != 0 && this.mineRandom.NextDouble() < 0.029)
          return new StardewValley.Object("764", 1)
          {
            MinutesUntilReady = 7
          };
        if (color.Equals(Color.White))
          return new StardewValley.Object(num2.ToString(), 1)
          {
            MinutesUntilReady = num1
          };
      }
      else if (this.mineLevel % 5 != 0 && this.mineRandom.NextDouble() < 0.029)
        return new StardewValley.Object("764", 1)
        {
          MinutesUntilReady = 8
        };
    }
    else
    {
      if (this.getMineArea() == 77377)
      {
        int num6 = 5;
        bool flag1 = false;
        foreach (Vector2 adjacentTileLocation in Utility.getAdjacentTileLocations(tile))
        {
          if (this.objects.ContainsKey(adjacentTileLocation))
          {
            flag1 = true;
            break;
          }
        }
        if (!flag1 && this.mineRandom.NextDouble() < 0.45)
          return (StardewValley.Object) null;
        bool flag2 = false;
        for (int index = 0; index < this.brownSpots.Count; ++index)
        {
          if ((double) Vector2.Distance(tile, this.brownSpots[index]) < 4.0)
          {
            flag2 = true;
            break;
          }
          if ((double) Vector2.Distance(tile, this.brownSpots[index]) < 6.0)
            return (StardewValley.Object) null;
        }
        int num7;
        if ((double) tile.X > 50.0)
        {
          num7 = Game1.random.Choose<int>(668, 670);
          if (this.mineRandom.NextDouble() < 0.09 + Game1.player.team.AverageDailyLuck((GameLocation) this) / 2.0)
            return new StardewValley.Object(Game1.random.Choose<string>("BasicCoalNode0", "BasicCoalNode1"), 1)
            {
              MinutesUntilReady = 5
            };
          if (this.mineRandom.NextDouble() < 0.25)
            return (StardewValley.Object) null;
        }
        else if (flag2)
        {
          num7 = this.mineRandom.Choose<int>(32 /*0x20*/, 38);
          if (this.mineRandom.NextDouble() < 0.01)
            return new StardewValley.Object("751", 1)
            {
              MinutesUntilReady = 3
            };
        }
        else
        {
          num7 = this.mineRandom.Choose<int>(34, 36);
          if (this.mineRandom.NextDouble() < 0.01)
            return new StardewValley.Object("290", 1)
            {
              MinutesUntilReady = 3
            };
        }
        return new StardewValley.Object(num7.ToString(), 1)
        {
          MinutesUntilReady = num6
        };
      }
      num1 = 5;
      num2 = !this.mineRandom.NextBool() ? (!this.mineRandom.NextBool() ? 42 : 40) : (!this.mineRandom.NextBool() ? 32 /*0x20*/ : 38);
      int val2 = this.mineLevel - 120;
      double num8 = 0.02 + (double) val2 * 0.0005;
      if (this.mineLevel >= 130)
        num8 += 0.01 * ((double) (Math.Min(100, val2) - 10) / 10.0);
      double val1 = 0.0;
      if (this.mineLevel >= 130)
        val1 += 0.001 * ((double) (val2 - 10) / 10.0);
      double num9 = Math.Min(val1, 0.004);
      if (val2 > 100)
        num9 += (double) val2 / 1000000.0;
      if (!this.netIsTreasureRoom.Value && this.mineRandom.NextDouble() < num8)
      {
        double num10 = (double) Math.Min(100, val2) * (0.0003 + num9);
        double num11 = 0.01 + (double) (this.mineLevel - Math.Min(150, val2)) * 0.0005;
        double num12 = Math.Min(0.5, 0.1 + (double) (this.mineLevel - Math.Min(200, val2)) * 0.005);
        if (Utility.GetDayOfPassiveFestival("DesertFestival") > 0 && this.mineRandom.NextBool(0.13 + (double) (Game1.player.team.calicoEggSkullCavernRating.Value * 5) / 1000.0))
          return new StardewValley.Object("CalicoEggStone_" + this.mineRandom.Next(3).ToString(), 1)
          {
            MinutesUntilReady = 8
          };
        if (this.mineRandom.NextDouble() < num10)
          return new StardewValley.Object("765", 1)
          {
            MinutesUntilReady = 16 /*0x10*/
          };
        if (this.mineRandom.NextDouble() < num11)
          return new StardewValley.Object("764", 1)
          {
            MinutesUntilReady = 8
          };
        if (this.mineRandom.NextDouble() < num12)
          return new StardewValley.Object("290", 1)
          {
            MinutesUntilReady = 4
          };
        return new StardewValley.Object("751", 1)
        {
          MinutesUntilReady = 2
        };
      }
    }
    double num13 = Game1.player.team.AverageDailyLuck((GameLocation) this);
    double num14 = Game1.player.team.AverageSkillLevel(3, Game1.currentLocation);
    double num15 = num13 + num14 * 0.005;
    if (this.mineLevel > 50 && this.mineRandom.NextDouble() < 0.00025 + (double) this.mineLevel / 120000.0 + 0.0005 * num15 / 2.0)
    {
      num2 = 2;
      num1 = 10;
    }
    else if (gemStoneChance != 0.0 && this.mineRandom.NextDouble() < gemStoneChance + gemStoneChance * num15 + (double) this.mineLevel / 24000.0)
      return new StardewValley.Object(this.getRandomGemRichStoneForThisLevel(this.mineLevel), 1)
      {
        MinutesUntilReady = 5
      };
    if (this.mineRandom.NextDouble() < chanceForPurpleStone / 2.0 + chanceForPurpleStone * num14 * 0.008 + chanceForPurpleStone * (num13 / 2.0))
      num2 = 44;
    if (this.mineLevel > 100 && this.mineRandom.NextDouble() < chanceForMysticStone + chanceForMysticStone * num14 * 0.008 + chanceForMysticStone * (num13 / 2.0))
      num2 = 46;
    int num16 = num2 + num2 % 2;
    if (this.mineRandom.NextDouble() < 0.1 && this.getMineArea() != 40)
    {
      if (!color.Equals(Color.White))
      {
        ColoredObject litterObject = new ColoredObject(this.mineRandom.Choose<string>("668", "670"), 1, color);
        litterObject.MinutesUntilReady = 2;
        litterObject.Flipped = this.mineRandom.NextBool();
        return (StardewValley.Object) litterObject;
      }
      return new StardewValley.Object(this.mineRandom.Choose<string>("668", "670"), 1)
      {
        MinutesUntilReady = 2,
        Flipped = this.mineRandom.NextBool()
      };
    }
    if (!color.Equals(Color.White))
    {
      ColoredObject litterObject = new ColoredObject(num16.ToString(), 1, color);
      litterObject.MinutesUntilReady = num1;
      litterObject.Flipped = this.mineRandom.NextBool();
      return (StardewValley.Object) litterObject;
    }
    return new StardewValley.Object(num16.ToString(), 1)
    {
      MinutesUntilReady = num1
    };
  }

  public static void OnLeftMines()
  {
    if (!Game1.IsClient && !Game1.IsMultiplayer)
      MineShaft.clearInactiveMines(false);
    Game1.player.buffs.Remove("CalicoStatueSpeed");
  }

  public static void clearActiveMines()
  {
    MineShaft.activeMines.RemoveAll((Predicate<MineShaft>) (mine =>
    {
      mine.OnRemoved();
      return true;
    }));
  }

  private static void clearInactiveMines(bool keepUntickedLevels = true)
  {
    int maxMineLevel = -1;
    int maxSkullLevel = -1;
    string[] disconnectLevels = Game1.getAllFarmhands().Select<Farmer, string>((Func<Farmer, string>) (fh => (long) fh.disconnectDay.Value != (long) Game1.MasterPlayer.stats.DaysPlayed ? (string) null : fh.disconnectLocation.Value)).ToArray<string>();
    foreach (Farmer allFarmer in Game1.getAllFarmers())
    {
      int level;
      if (allFarmer.locationBeforeForcedEvent.Value != null && MineShaft.IsGeneratedLevel(allFarmer.locationBeforeForcedEvent.Value, out level))
      {
        if (level > 120)
        {
          if (level < 77377)
            maxSkullLevel = Math.Max(maxSkullLevel, level);
        }
        else
          maxMineLevel = Math.Max(maxMineLevel, level);
      }
    }
    foreach (MineShaft activeMine in MineShaft.activeMines)
    {
      if (activeMine.farmers.Any() || ((IEnumerable<string>) disconnectLevels).Contains<string>(activeMine.NameOrUniqueName))
      {
        if (activeMine.mineLevel > 120)
        {
          if (activeMine.mineLevel < 77377)
            maxSkullLevel = Math.Max(maxSkullLevel, activeMine.mineLevel);
        }
        else
          maxMineLevel = Math.Max(maxMineLevel, activeMine.mineLevel);
      }
    }
    MineShaft.activeMines.RemoveAll((Predicate<MineShaft>) (mine =>
    {
      if (mine.mineLevel == 77377 || ((IEnumerable<string>) disconnectLevels).Contains<string>(mine.NameOrUniqueName))
        return false;
      if (mine.mineLevel > 120)
      {
        if (mine.mineLevel <= maxSkullLevel)
          return false;
      }
      else if (mine.mineLevel <= maxMineLevel)
        return false;
      if (mine.lifespan == 0 & keepUntickedLevels || Game1.IsServer && Game1.locationRequest?.Location is MineShaft location2 && mine.NameOrUniqueName == location2.NameOrUniqueName)
        return false;
      mine.OnRemoved();
      return true;
    }));
    if (MineShaft.activeMines.Count != 0)
      return;
    Game1.player.team.calicoEggSkullCavernRating.Value = 0;
    Game1.player.team.calicoStatueEffects.Clear();
    MineShaft.deepestLevelOnCurrentDesertFestivalRun = 0;
  }

  public static void UpdateMines10Minutes(int timeOfDay)
  {
    MineShaft.clearInactiveMines();
    if (Game1.IsClient)
      return;
    foreach (MineShaft activeMine in MineShaft.activeMines)
    {
      if (activeMine.farmers.Any())
        activeMine.performTenMinuteUpdate(timeOfDay);
      ++activeMine.lifespan;
    }
  }

  protected override void updateCharacters(GameTime time)
  {
    if (!this.farmers.Any())
      return;
    base.updateCharacters(time);
  }

  public override void updateEvenIfFarmerIsntHere(GameTime time, bool ignoreWasUpdatedFlush = false)
  {
    base.updateEvenIfFarmerIsntHere(time, ignoreWasUpdatedFlush);
    if (!Game1.shouldTimePass() || !this.isFogUp.Value)
      return;
    int fogTime = this.fogTime;
    this.fogTime -= (int) time.ElapsedGameTime.TotalMilliseconds;
    if (!Game1.IsMasterGame)
      return;
    if (this.fogTime > 5000 && fogTime % 4000 < this.fogTime % 4000)
      this.spawnFlyingMonsterOffScreen();
    if (this.fogTime > 0)
      return;
    this.isFogUp.Value = false;
    if (this.isDarkArea())
    {
      this.netFogColor.Value = Color.Black;
    }
    else
    {
      if (this.GetAdditionalDifficulty() <= 0 || this.getMineArea() != 40 || this.isDarkArea())
        return;
      this.netFogColor.Value = new Color();
    }
  }

  public static void UpdateMines(GameTime time)
  {
    foreach (MineShaft activeMine in MineShaft.activeMines)
    {
      if (activeMine.farmers.Any())
        activeMine.UpdateWhenCurrentLocation(time);
      activeMine.updateEvenIfFarmerIsntHere(time, false);
    }
  }

  /// <inheritdoc />
  public override void OnRemoved()
  {
    base.OnRemoved();
    this.mapContent.Dispose();
  }

  /// <summary>Get the location name for a generated mine level.</summary>
  /// <param name="level">The mine level.</param>
  /// <param name="forceLayout">The layout in <c>Maps/Mines</c> to use, or <c>null</c> to choose a random one based on the level.</param>
  public static string GetLevelName(int level, int? forceLayout = null)
  {
    if (!forceLayout.HasValue)
      return $"UndergroundMine{level}";
    return $"UndergroundMine{level}:{forceLayout}";
  }

  /// <summary>Get whether a location is a generated mine level.</summary>
  /// <param name="location">The location to check.</param>
  public static bool IsGeneratedLevel(GameLocation location)
  {
    return MineShaft.IsGeneratedLevel(location, out int _, out int? _);
  }

  /// <summary>Get whether a location is a generated mine level.</summary>
  /// <param name="location">The location to check.</param>
  /// <param name="level">The parsed mine level, if applicable.</param>
  public static bool IsGeneratedLevel(GameLocation location, out int level)
  {
    return MineShaft.IsGeneratedLevel(location, out level, out int? _);
  }

  /// <summary>Get whether a location is a generated mine level.</summary>
  /// <param name="location">The location to check.</param>
  /// <param name="level">The parsed mine level, if applicable.</param>
  /// <param name="forceLayout">The layout in <c>Maps/Mines</c> to use, or <c>null</c> to choose a random one based on the level.</param>
  public static bool IsGeneratedLevel(GameLocation location, out int level, out int? forceLayout)
  {
    if (location is MineShaft mineShaft)
    {
      level = mineShaft.mineLevel;
      forceLayout = mineShaft.forceLayout;
      return true;
    }
    level = 0;
    forceLayout = new int?();
    return false;
  }

  /// <summary>Get whether a location name is a generated mine level.</summary>
  /// <param name="locationName">The location name to check.</param>
  public static bool IsGeneratedLevel(string locationName)
  {
    return MineShaft.IsGeneratedLevel(locationName, out int _, out int? _);
  }

  /// <summary>Get whether a location name is a generated mine level.</summary>
  /// <param name="locationName">The location name to check.</param>
  /// <param name="level">The parsed mine level, if applicable.</param>
  public static bool IsGeneratedLevel(string locationName, out int level)
  {
    return MineShaft.IsGeneratedLevel(locationName, out level, out int? _);
  }

  /// <summary>Get whether a location name is a generated mine level.</summary>
  /// <param name="locationName">The location name to check.</param>
  /// <param name="level">The parsed mine level, if applicable.</param>
  /// <param name="forceLayout">The layout in <c>Maps/Mines</c> to use, or <c>null</c> to choose a random one based on the level.</param>
  public static bool IsGeneratedLevel(string locationName, out int level, out int? forceLayout)
  {
    if (locationName == null || !locationName.StartsWithIgnoreCase("UndergroundMine"))
    {
      level = 0;
      forceLayout = new int?();
      return false;
    }
    string s = locationName.Substring("UndergroundMine".Length);
    int length = s.IndexOf(':');
    if (length > 0)
    {
      int result;
      if (int.TryParse(s.Substring(0, length), out level) && int.TryParse(s.Substring(length + 1), out result))
      {
        forceLayout = new int?(result);
        return true;
      }
      level = 0;
      forceLayout = new int?();
      return false;
    }
    forceLayout = new int?();
    return int.TryParse(s, out level);
  }

  public static MineShaft GetMine(string name)
  {
    int level;
    int? forceLayout;
    if (!MineShaft.IsGeneratedLevel(name, out level, out forceLayout))
    {
      Game1.log.Warn($"Failed parsing mine level from location name '{name}', defaulting to level 0.");
      level = 0;
    }
    if (forceLayout.HasValue)
      name = MineShaft.GetLevelName(level);
    foreach (MineShaft activeMine in MineShaft.activeMines)
    {
      if (activeMine.Name == name)
      {
        if (forceLayout.HasValue)
        {
          int loadedMapNumber = activeMine.loadedMapNumber;
          int? nullable = forceLayout;
          int valueOrDefault = nullable.GetValueOrDefault();
          if (!(loadedMapNumber == valueOrDefault & nullable.HasValue))
            Game1.log.Warn($"Can't set mine level {level} to layout {forceLayout} because it's already active with layout {activeMine.loadedMapNumber}.");
        }
        return activeMine;
      }
    }
    MineShaft mine = new MineShaft(level, forceLayout);
    MineShaft.activeMines.Add(mine);
    mine.generateContents();
    return mine;
  }

  public static void ForEach(Action<MineShaft> action)
  {
    foreach (MineShaft activeMine in MineShaft.activeMines)
      action(activeMine);
  }
}
