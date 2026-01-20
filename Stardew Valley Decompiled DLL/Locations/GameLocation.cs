// Decompiled with JetBrains decompiler
// Type: StardewValley.GameLocation
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Netcode;
using Netcode.Validation;
using StardewValley.Audio;
using StardewValley.BellsAndWhistles;
using StardewValley.Buffs;
using StardewValley.Buildings;
using StardewValley.Characters;
using StardewValley.Constants;
using StardewValley.Enchantments;
using StardewValley.Extensions;
using StardewValley.GameData;
using StardewValley.GameData.Buildings;
using StardewValley.GameData.Characters;
using StardewValley.GameData.Crops;
using StardewValley.GameData.FruitTrees;
using StardewValley.GameData.GarbageCans;
using StardewValley.GameData.LocationContexts;
using StardewValley.GameData.Locations;
using StardewValley.GameData.Minecarts;
using StardewValley.GameData.Movies;
using StardewValley.GameData.Pets;
using StardewValley.GameData.WildTrees;
using StardewValley.Internal;
using StardewValley.Inventories;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Minigames;
using StardewValley.Mods;
using StardewValley.Monsters;
using StardewValley.Network;
using StardewValley.Network.NetEvents;
using StardewValley.Objects;
using StardewValley.Objects.Trinkets;
using StardewValley.Pathfinding;
using StardewValley.Projectiles;
using StardewValley.Quests;
using StardewValley.SpecialOrders;
using StardewValley.SpecialOrders.Objectives;
using StardewValley.TerrainFeatures;
using StardewValley.TokenizableStrings;
using StardewValley.Tools;
using StardewValley.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using xTile;
using xTile.Dimensions;
using xTile.Layers;
using xTile.ObjectModel;
using xTile.Tiles;

#nullable disable
namespace StardewValley;

[XmlInclude(typeof (AbandonedJojaMart))]
[XmlInclude(typeof (AdventureGuild))]
[XmlInclude(typeof (AnimalHouse))]
[XmlInclude(typeof (BathHousePool))]
[XmlInclude(typeof (Beach))]
[XmlInclude(typeof (BeachNightMarket))]
[XmlInclude(typeof (BoatTunnel))]
[XmlInclude(typeof (BugLand))]
[XmlInclude(typeof (BusStop))]
[XmlInclude(typeof (Cabin))]
[XmlInclude(typeof (Caldera))]
[XmlInclude(typeof (Cellar))]
[XmlInclude(typeof (Club))]
[XmlInclude(typeof (CommunityCenter))]
[XmlInclude(typeof (DecoratableLocation))]
[XmlInclude(typeof (Desert))]
[XmlInclude(typeof (DesertFestival))]
[XmlInclude(typeof (Farm))]
[XmlInclude(typeof (FarmCave))]
[XmlInclude(typeof (FarmHouse))]
[XmlInclude(typeof (FishShop))]
[XmlInclude(typeof (Forest))]
[XmlInclude(typeof (IslandEast))]
[XmlInclude(typeof (IslandFarmCave))]
[XmlInclude(typeof (IslandFarmHouse))]
[XmlInclude(typeof (IslandFieldOffice))]
[XmlInclude(typeof (IslandForestLocation))]
[XmlInclude(typeof (IslandHut))]
[XmlInclude(typeof (IslandLocation))]
[XmlInclude(typeof (IslandNorth))]
[XmlInclude(typeof (IslandSecret))]
[XmlInclude(typeof (IslandShrine))]
[XmlInclude(typeof (IslandSouth))]
[XmlInclude(typeof (IslandSouthEast))]
[XmlInclude(typeof (IslandSouthEastCave))]
[XmlInclude(typeof (IslandWest))]
[XmlInclude(typeof (IslandWestCave1))]
[XmlInclude(typeof (JojaMart))]
[XmlInclude(typeof (LibraryMuseum))]
[XmlInclude(typeof (ManorHouse))]
[XmlInclude(typeof (MermaidHouse))]
[XmlInclude(typeof (Mine))]
[XmlInclude(typeof (MineShaft))]
[XmlInclude(typeof (Mountain))]
[XmlInclude(typeof (MovieTheater))]
[XmlInclude(typeof (Railroad))]
[XmlInclude(typeof (SeedShop))]
[XmlInclude(typeof (Sewer))]
[XmlInclude(typeof (Shed))]
[XmlInclude(typeof (ShopLocation))]
[XmlInclude(typeof (SlimeHutch))]
[XmlInclude(typeof (Submarine))]
[XmlInclude(typeof (Summit))]
[XmlInclude(typeof (Town))]
[XmlInclude(typeof (WizardHouse))]
[XmlInclude(typeof (Woods))]
[InstanceStatics]
[NotImplicitNetField]
public class GameLocation : 
  INetObject<NetFields>,
  IEquatable<GameLocation>,
  IAnimalLocation,
  IHaveModData
{
  public const int maxTriesForDebrisPlacement = 3;
  /// <summary>The default ID for a map tile sheet. This often (but not always) matches the main tile sheet for Stardew Valley maps.</summary>
  public const string DefaultTileSheetId = "untitled tile sheet";
  public const string OVERRIDE_MAP_TILESHEET_PREFIX = "zzzzz";
  public const string PHONE_DIAL_SOUND = "telephone_buttonPush";
  public const int PHONE_RING_DURATION = 4950;
  public const string PHONE_PICKUP_SOUND = "bigSelect";
  public const string PHONE_HANGUP_SOUND = "openBox";
  /// <summary>The ocean fish types.</summary>
  public static readonly IList<string> OceanCrabPotFishTypes = (IList<string>) new string[1]
  {
    "ocean"
  };
  /// <summary>The default fish types caught by crab pots in all locations which don't have a specific value in <c>Data/Locations</c>.</summary>
  public static readonly IList<string> DefaultCrabPotFishTypes = (IList<string>) new string[1]
  {
    "freshwater"
  };
  /// <summary>The cached value for <see cref="M:StardewValley.GameLocation.GetSeason" />.</summary>
  /// <remarks>Most code should use <see cref="M:StardewValley.GameLocation.GetSeason" /> instead.</remarks>
  [XmlIgnore]
  private Lazy<Season?> seasonOverride;
  [XmlIgnore]
  public bool? isMusicTownMusic;
  /// <summary>The cached location context ID for <see cref="M:StardewValley.GameLocation.GetLocationContextId" />.</summary>
  /// <remarks>Most code should use <see cref="M:StardewValley.GameLocation.GetLocationContextId" /> or <see cref="M:StardewValley.GameLocation.GetLocationContext" /> instead.</remarks>
  [XmlIgnore]
  public string locationContextId;
  public readonly NetCollection<Building> buildings = new NetCollection<Building>()
  {
    InterpolationWait = false
  };
  [XmlElement("animals")]
  public readonly NetLongDictionary<FarmAnimal, NetRef<FarmAnimal>> animals = new NetLongDictionary<FarmAnimal, NetRef<FarmAnimal>>();
  [XmlElement("piecesOfHay")]
  public readonly NetInt piecesOfHay = new NetInt(0);
  private readonly List<KeyValuePair<long, FarmAnimal>> tempAnimals = new List<KeyValuePair<long, FarmAnimal>>();
  /// <summary>The unique name of the parent location, if applicable.</summary>
  [XmlIgnore]
  public readonly NetString parentLocationName = new NetString();
  /// <summary>The building which contains this location, if applicable.</summary>
  [XmlIgnore]
  public Building ParentBuilding;
  [XmlIgnore]
  public List<KeyValuePair<Layer, int>> backgroundLayers = new List<KeyValuePair<Layer, int>>();
  [XmlIgnore]
  public List<KeyValuePair<Layer, int>> buildingLayers = new List<KeyValuePair<Layer, int>>();
  [XmlIgnore]
  public List<KeyValuePair<Layer, int>> frontLayers = new List<KeyValuePair<Layer, int>>();
  [XmlIgnore]
  public List<KeyValuePair<Layer, int>> alwaysFrontLayers = new List<KeyValuePair<Layer, int>>();
  [NonInstancedStatic]
  [XmlIgnore]
  protected static Dictionary<string, Action<GameLocation, string[], Farmer, Vector2>> registeredTouchActions = new Dictionary<string, Action<GameLocation, string[], Farmer, Vector2>>();
  [NonInstancedStatic]
  [XmlIgnore]
  protected static Dictionary<string, Func<GameLocation, string[], Farmer, Point, bool>> registeredTileActions = new Dictionary<string, Func<GameLocation, string[], Farmer, Point, bool>>();
  /// <summary>Whether this location should always be synchronized in multiplayer. </summary>
  /// <remarks>
  ///   <para>This value should only be set when the location is instantiated, it shouldn't be modified during gameplay.</para>
  /// 
  ///   <para>Most code should call <see cref="M:StardewValley.Multiplayer.isAlwaysActiveLocation(StardewValley.GameLocation)" /> instead.</para>
  /// </remarks>
  [XmlIgnore]
  public NetBool isAlwaysActive = new NetBool();
  [XmlIgnore]
  public GameLocation.afterQuestionBehavior afterQuestion;
  [XmlIgnore]
  public Map map;
  [XmlIgnore]
  public readonly NetString mapPath = new NetString().Interpolated(false, false);
  [XmlIgnore]
  protected string loadedMapPath;
  public readonly NetCollection<NPC> characters = new NetCollection<NPC>();
  [XmlIgnore]
  public readonly NetVector2Dictionary<Object, NetRef<Object>> netObjects = new NetVector2Dictionary<Object, NetRef<Object>>();
  [XmlIgnore]
  public readonly OverlayDictionary<Vector2, Object> overlayObjects = new OverlayDictionary<Vector2, Object>((IEqualityComparer<Vector2>) GameLocation.tilePositionComparer);
  [XmlElement("objects")]
  public readonly OverlaidDictionary objects;
  [XmlIgnore]
  public NetList<MapSeat, NetRef<MapSeat>> mapSeats = new NetList<MapSeat, NetRef<MapSeat>>();
  protected bool _mapSeatsDirty;
  [XmlIgnore]
  public TemporaryAnimatedSpriteList temporarySprites = new TemporaryAnimatedSpriteList();
  [XmlIgnore]
  public List<Action> postFarmEventOvernightActions = new List<Action>();
  [XmlIgnore]
  public readonly NetObjectList<Warp> warps = new NetObjectList<Warp>();
  [XmlIgnore]
  public readonly NetPointDictionary<string, NetString> doors = new NetPointDictionary<string, NetString>();
  [XmlIgnore]
  public readonly InteriorDoorDictionary interiorDoors;
  [XmlIgnore]
  public readonly FarmerCollection farmers;
  [XmlIgnore]
  public readonly NetCollection<Projectile> projectiles = new NetCollection<Projectile>();
  public readonly NetCollection<ResourceClump> resourceClumps = new NetCollection<ResourceClump>();
  public readonly NetCollection<LargeTerrainFeature> largeTerrainFeatures = new NetCollection<LargeTerrainFeature>();
  /// <summary>The terrain features whose <see cref="M:StardewValley.TerrainFeatures.TerrainFeature.tickUpdate(Microsoft.Xna.Framework.GameTime)" /> method should be called on each tick.</summary>
  [XmlIgnore]
  public List<TerrainFeature> _activeTerrainFeatures = new List<TerrainFeature>();
  [XmlIgnore]
  public List<Critter> critters;
  [XmlElement("terrainFeatures")]
  public readonly NetVector2Dictionary<TerrainFeature, NetRef<TerrainFeature>> terrainFeatures = new NetVector2Dictionary<TerrainFeature, NetRef<TerrainFeature>>();
  [XmlIgnore]
  public readonly NetCollection<Debris> debris = new NetCollection<Debris>();
  [XmlIgnore]
  public readonly NetPoint fishSplashPoint = new NetPoint(Point.Zero);
  private int fishSplashPointTime;
  [XmlIgnore]
  public readonly NetString fishFrenzyFish = new NetString();
  [XmlIgnore]
  public readonly NetPoint orePanPoint = new NetPoint(Point.Zero);
  [XmlIgnore]
  public TemporaryAnimatedSprite fishSplashAnimation;
  [XmlIgnore]
  public TemporaryAnimatedSprite orePanAnimation;
  [XmlIgnore]
  public WaterTiles waterTiles;
  [XmlIgnore]
  protected HashSet<string> _appliedMapOverrides;
  [XmlElement("uniqueName")]
  public readonly NetString uniqueName = new NetString();
  [XmlIgnore]
  protected string _displayName;
  [XmlElement("name")]
  public readonly NetString name = new NetString();
  [XmlElement("waterColor")]
  public readonly NetColor waterColor = new NetColor(Color.White * 0.33f);
  [XmlIgnore]
  public string lastQuestionKey;
  [XmlIgnore]
  public Vector2 lastTouchActionLocation = Vector2.Zero;
  [XmlElement("lightLevel")]
  protected readonly NetFloat lightLevel = new NetFloat(0.0f);
  [XmlElement("isFarm")]
  public readonly NetBool isFarm = new NetBool();
  [XmlElement("isOutdoors")]
  public readonly NetBool isOutdoors = new NetBool();
  [XmlIgnore]
  public readonly NetBool isGreenhouse = new NetBool();
  [XmlElement("isStructure")]
  public readonly NetBool isStructure = new NetBool();
  [XmlElement("ignoreDebrisWeather")]
  public readonly NetBool ignoreDebrisWeather = new NetBool();
  [XmlElement("ignoreOutdoorLighting")]
  public readonly NetBool ignoreOutdoorLighting = new NetBool();
  [XmlElement("ignoreLights")]
  public readonly NetBool ignoreLights = new NetBool();
  [XmlElement("treatAsOutdoors")]
  public readonly NetBool treatAsOutdoors = new NetBool();
  [XmlIgnore]
  public bool wasUpdated;
  public int numberOfSpawnedObjectsOnMap;
  [XmlIgnore]
  public bool showDropboxIndicator;
  [XmlIgnore]
  public Vector2 dropBoxIndicatorLocation;
  [XmlElement("miniJukeboxCount")]
  public readonly NetInt miniJukeboxCount = new NetInt();
  [XmlElement("miniJukeboxTrack")]
  public readonly NetString miniJukeboxTrack = new NetString("");
  [XmlIgnore]
  public readonly NetString randomMiniJukeboxTrack = new NetString();
  [XmlIgnore]
  public Event currentEvent;
  [XmlIgnore]
  public Object actionObjectForQuestionDialogue;
  [XmlIgnore]
  public int waterAnimationIndex;
  [XmlIgnore]
  public int waterAnimationTimer;
  [XmlIgnore]
  public bool waterTileFlip;
  [XmlIgnore]
  public bool forceViewportPlayerFollow;
  [XmlIgnore]
  public bool forceLoadPathLayerLights;
  [XmlIgnore]
  public float waterPosition;
  [XmlIgnore]
  public readonly NetAudio netAudio;
  /// <summary>The light sources to draw for players in this location.</summary>
  [XmlIgnore]
  public readonly NetStringDictionary<LightSource, NetRef<LightSource>> sharedLights = new NetStringDictionary<LightSource, NetRef<LightSource>>();
  private readonly NetEvent1Field<int, NetInt> removeTemporarySpritesWithIDEvent = new NetEvent1Field<int, NetInt>();
  private readonly NetEvent1Field<int, NetInt> rumbleAndFadeEvent = new NetEvent1Field<int, NetInt>();
  /// <summary>An event raised to damage players within the current location.</summary>
  private readonly NetEvent1<GameLocation.DamagePlayersEventArg> damagePlayersEvent = new NetEvent1<GameLocation.DamagePlayersEventArg>();
  [XmlIgnore]
  public NetVector2HashSet lightGlows = new NetVector2HashSet();
  public static readonly int JOURNAL_INDEX = 1000;
  public static readonly float FIRST_SECRET_NOTE_CHANCE = 0.8f;
  public static readonly float LAST_SECRET_NOTE_CHANCE = 0.12f;
  public static readonly int NECKLACE_SECRET_NOTE_INDEX = 25;
  public static readonly string CAROLINES_NECKLACE_ITEM_QID = "(O)191";
  public static readonly string CAROLINES_NECKLACE_MAIL = "carolinesNecklace";
  public static TilePositionComparer tilePositionComparer = new TilePositionComparer();
  protected List<Vector2> _startingCabinLocations = new List<Vector2>();
  [XmlIgnore]
  public bool wasInhabited;
  [XmlIgnore]
  protected bool _madeMapModifications;
  public readonly NetCollection<Furniture> furniture = new NetCollection<Furniture>()
  {
    InterpolationWait = false
  };
  protected readonly NetMutexQueue<Guid> furnitureToRemove = new NetMutexQueue<Guid>();
  protected bool _mapPathDirty = true;
  protected LocalizedContentManager _structureMapLoader;
  protected bool ignoreWarps;
  protected HashSet<Vector2> _visitedCollisionTiles = new HashSet<Vector2>();
  protected bool _looserBuildRestrictions;
  protected Microsoft.Xna.Framework.Rectangle? _buildableTileRect;
  private bool showedBuildableButNotAlwaysActiveWarning;
  public static bool PlayedNewLocationContextMusic = false;
  private const int fireIDBase = 944468;
  protected Color indoorLightingColor = new Color(100, 120, 30);
  protected Color indoorLightingNightColor = new Color(150, 150, 30);
  protected static List<KeyValuePair<string, string>> _PagedResponses = new List<KeyValuePair<string, string>>();
  protected static int _PagedResponsePage = 0;
  protected static int _PagedResponseItemsPerPage;
  public static bool _PagedResponseAddCancel;
  protected static string _PagedResponsePrompt;
  protected static Action<string> _OnPagedResponse;
  protected string _constructLocationBuilderName;
  protected List<Farmer> _currentLocationFarmersForDisambiguating = new List<Farmer>();
  [XmlIgnore]
  public Dictionary<Vector2, float> lightGlowLayerCache = new Dictionary<Vector2, float>();

  public NetLongDictionary<FarmAnimal, NetRef<FarmAnimal>> Animals => this.animals;

  [XmlIgnore]
  public NetFields NetFields { get; }

  [XmlIgnore]
  public NetRoot<GameLocation> Root => this.NetFields.Root as NetRoot<GameLocation>;

  /// <summary>The number of milliseconds to add to <see cref="F:StardewValley.Game1.realMilliSecondsPerGameMinute" /> when calculating the flow of time within this location.</summary>
  [XmlIgnore]
  public int ExtraMillisecondsPerInGameMinute { get; set; }

  [XmlIgnore]
  public string DisplayName
  {
    get
    {
      if (this._displayName == null)
        this._displayName = this.GetDisplayName();
      return this._displayName == null ? this.GetParentLocation()?.DisplayName ?? this.Name : this._displayName;
    }
    set => this._displayName = value;
  }

  public virtual string GetDisplayName()
  {
    string displayName = this.GetData()?.DisplayName;
    return displayName == null ? (string) null : TokenParser.ParseText(displayName);
  }

  [XmlIgnore]
  public string NameOrUniqueName
  {
    get => this.uniqueName.Value != null ? this.uniqueName.Value : this.name.Value;
  }

  /// <summary>Whether this is a temporary location for a festival or event.</summary>
  /// <remarks>This is set automatically based on <see cref="M:StardewValley.GameLocation.IsTemporaryName(System.String)" />.</remarks>
  [XmlIgnore]
  public bool IsTemporary { get; protected set; }

  [XmlIgnore]
  public float LightLevel
  {
    get => this.lightLevel.Value;
    set => this.lightLevel.Value = value;
  }

  [XmlIgnore]
  public Map Map
  {
    get
    {
      this.updateMap();
      return this.map;
    }
    set => this.map = value;
  }

  [XmlIgnore]
  public OverlaidDictionary Objects => this.objects;

  [XmlIgnore]
  public TemporaryAnimatedSpriteList TemporarySprites => this.temporarySprites;

  public string Name => this.name.Value;

  [XmlIgnore]
  public bool IsFarm
  {
    get => this.isFarm.Value;
    set => this.isFarm.Value = value;
  }

  [XmlIgnore]
  public bool IsOutdoors
  {
    get => this.isOutdoors.Value;
    set => this.isOutdoors.Value = value;
  }

  public bool IsGreenhouse
  {
    get => this.isGreenhouse.Value;
    set => this.isGreenhouse.Value = value;
  }

  /// <summary>Whether seeds and sapling can be planted and grown in any season here.</summary>
  public virtual bool SeedsIgnoreSeasonsHere() => this.IsGreenhouse;

  /// <summary>Get whether crop seeds can be planted in this location.</summary>
  /// <param name="itemId">The qualified or unqualified item ID for the seed being planted.</param>
  /// <param name="tileX">The X tile position for which to apply location-specific overrides.</param>
  /// <param name="tileY">The Y tile position for which to apply location-specific overrides.</param>
  /// <param name="isGardenPot">Whether the item is being planted in a garden pot.</param>
  /// <param name="deniedMessage">The translated message to show to the user indicating why it can't be planted, if applicable.</param>
  public virtual bool CanPlantSeedsHere(
    string itemId,
    int tileX,
    int tileY,
    bool isGardenPot,
    out string deniedMessage)
  {
    return this.CheckItemPlantRules(itemId, isGardenPot, ((int) this.GetData()?.CanPlantHere ?? (this.IsFarm ? 1 : 0)) != 0, out deniedMessage);
  }

  /// <summary>Get whether tree saplings can be planted in this location.</summary>
  /// <param name="itemId">The qualified or unqualified item ID for the sapling being planted.</param>
  /// <param name="tileX">The X tile position for which to apply location-specific overrides.</param>
  /// <param name="tileY">The Y tile position for which to apply location-specific overrides.</param>
  /// <param name="deniedMessage">The translated message to show to the user indicating why it can't be planted, if applicable.</param>
  public virtual bool CanPlantTreesHere(
    string itemId,
    int tileX,
    int tileY,
    out string deniedMessage)
  {
    string itemId1 = itemId;
    int num;
    if (!this.IsGreenhouse && !this.IsFarm)
    {
      bool? nullable = (bool?) this.GetData()?.CanPlantHere;
      if ((!nullable.HasValue || !nullable.GetValueOrDefault()) && (!Object.isWildTreeSeed(itemId) || !this.IsOutdoors || !(this.doesTileHavePropertyNoNull(tileX, tileY, "Type", "Back") == "Dirt")))
      {
        nullable = this.map?.Properties.ContainsKey("ForceAllowTreePlanting");
        num = !nullable.HasValue ? 0 : (nullable.GetValueOrDefault() ? 1 : 0);
        goto label_4;
      }
    }
    num = 1;
label_4:
    ref string local = ref deniedMessage;
    return this.CheckItemPlantRules(itemId1, false, num != 0, out local);
  }

  /// <summary>Get whether a crop or tree can be planted here according to the planting rules in its data.</summary>
  /// <param name="itemId">The qualified or unqualified item ID for the seed or sapling being planted.</param>
  /// <param name="isGardenPot">Whether the item is being planted in a garden pot.</param>
  /// <param name="defaultAllowed">The result to return when no rules apply, or the selected rule uses <see cref="F:StardewValley.GameData.PlantableResult.Default" />.</param>
  /// <param name="deniedMessage">The translated message to show to the user indicating why it can't be planted, if applicable.</param>
  /// <remarks>This is a low-level method which doesn't check higher-level requirements. Most code should call <see cref="M:StardewValley.GameLocation.CanPlantSeedsHere(System.String,System.Int32,System.Int32,System.Boolean,System.String@)" /> or <see cref="M:StardewValley.GameLocation.CanPlantTreesHere(System.String,System.Int32,System.Int32,System.String@)" /> instead.</remarks>
  public bool CheckItemPlantRules(
    string itemId,
    bool isGardenPot,
    bool defaultAllowed,
    out string deniedMessage)
  {
    ItemMetadata metadata = ItemRegistry.GetMetadata(itemId);
    if (metadata != null && metadata.TypeIdentifier == "(O)")
    {
      itemId = metadata.LocalItemId;
      CropData data1;
      if (Crop.TryGetData(itemId, out data1))
        return this.CheckItemPlantRules(data1.PlantableLocationRules, isGardenPot, defaultAllowed, out deniedMessage);
      string id = Tree.ResolveTreeTypeFromSeed(metadata.QualifiedItemId);
      WildTreeData data2;
      if (id != null && Tree.TryGetData(id, out data2))
        return this.CheckItemPlantRules(data2.PlantableLocationRules, isGardenPot, defaultAllowed, out deniedMessage);
      FruitTreeData data3;
      if (FruitTree.TryGetData(itemId, out data3))
        return this.CheckItemPlantRules(data3.PlantableLocationRules, isGardenPot, defaultAllowed, out deniedMessage);
    }
    deniedMessage = (string) null;
    return defaultAllowed;
  }

  /// <summary>Get whether a crop or tree can be planted here according to the planting rules in its data.</summary>
  /// <param name="rules">The plantable rules to check.</param>
  /// <param name="isGardenPot">Whether the item is being planted in a garden pot.</param>
  /// <param name="defaultAllowed">The result to return when no rules apply, or the selected rule uses <see cref="F:StardewValley.GameData.PlantableResult.Default" />.</param>
  /// <param name="deniedMessage">The translated message to show to the user indicating why it can't be planted, if applicable.</param>
  /// <remarks>This is a low-level method which doesn't check higher-level requirements. Most code should call <see cref="M:StardewValley.GameLocation.CanPlantSeedsHere(System.String,System.Int32,System.Int32,System.Boolean,System.String@)" /> or <see cref="M:StardewValley.GameLocation.CanPlantTreesHere(System.String,System.Int32,System.Int32,System.String@)" /> instead.</remarks>
  private bool CheckItemPlantRules(
    List<PlantableRule> rules,
    bool isGardenPot,
    bool defaultAllowed,
    out string deniedMessage)
  {
    // ISSUE: explicit non-virtual call
    if (rules != null && __nonvirtual (rules.Count) > 0)
    {
      foreach (PlantableRule rule in rules)
      {
        if (rule.ShouldApplyWhen(isGardenPot) && GameStateQuery.CheckConditions(rule.Condition, this))
        {
          switch (rule.Result)
          {
            case PlantableResult.Allow:
              deniedMessage = (string) null;
              return true;
            case PlantableResult.Deny:
              deniedMessage = TokenParser.ParseText(rule.DeniedMessage);
              return false;
            default:
              deniedMessage = !defaultAllowed ? TokenParser.ParseText(rule.DeniedMessage) : (string) null;
              return defaultAllowed;
          }
        }
      }
    }
    deniedMessage = (string) null;
    return defaultAllowed;
  }

  /// <inheritdoc />
  [XmlIgnore]
  public ModDataDictionary modData { get; } = new ModDataDictionary();

  /// <inheritdoc />
  [XmlElement("modData")]
  public ModDataDictionary modDataForSerialization
  {
    get => this.modData.GetForSerialization();
    set => this.modData.SetFromSerialization(value);
  }

  protected virtual void initNetFields()
  {
    this.NetFields.SetOwner((INetObject<NetFields>) this).AddField((INetSerializable) this.mapPath, "mapPath").AddField((INetSerializable) this.uniqueName, "uniqueName").AddField((INetSerializable) this.name, "name").AddField((INetSerializable) this.lightLevel, "lightLevel").AddField((INetSerializable) this.sharedLights, "sharedLights").AddField((INetSerializable) this.isFarm, "isFarm").AddField((INetSerializable) this.isOutdoors, "isOutdoors").AddField((INetSerializable) this.isStructure, "isStructure").AddField((INetSerializable) this.ignoreDebrisWeather, "ignoreDebrisWeather").AddField((INetSerializable) this.ignoreOutdoorLighting, "ignoreOutdoorLighting").AddField((INetSerializable) this.ignoreLights, "ignoreLights").AddField((INetSerializable) this.treatAsOutdoors, "treatAsOutdoors").AddField((INetSerializable) this.warps, "warps").AddField((INetSerializable) this.doors, "doors").AddField((INetSerializable) this.interiorDoors, "interiorDoors").AddField((INetSerializable) this.waterColor, "waterColor").AddField((INetSerializable) this.netObjects, "netObjects").AddField((INetSerializable) this.projectiles, "projectiles").AddField((INetSerializable) this.largeTerrainFeatures, "largeTerrainFeatures").AddField((INetSerializable) this.terrainFeatures, "terrainFeatures").AddField((INetSerializable) this.characters, "characters").AddField((INetSerializable) this.debris, "debris").AddField((INetSerializable) this.netAudio.NetFields, "netAudio.NetFields").AddField((INetSerializable) this.removeTemporarySpritesWithIDEvent, "removeTemporarySpritesWithIDEvent").AddField((INetSerializable) this.rumbleAndFadeEvent, "rumbleAndFadeEvent").AddField((INetSerializable) this.damagePlayersEvent, "damagePlayersEvent").AddField((INetSerializable) this.lightGlows, "lightGlows").AddField((INetSerializable) this.fishSplashPoint, "fishSplashPoint").AddField((INetSerializable) this.fishFrenzyFish, "fishFrenzyFish").AddField((INetSerializable) this.orePanPoint, "orePanPoint").AddField((INetSerializable) this.isGreenhouse, "isGreenhouse").AddField((INetSerializable) this.miniJukeboxCount, "miniJukeboxCount").AddField((INetSerializable) this.miniJukeboxTrack, "miniJukeboxTrack").AddField((INetSerializable) this.randomMiniJukeboxTrack, "randomMiniJukeboxTrack").AddField((INetSerializable) this.resourceClumps, "resourceClumps").AddField((INetSerializable) this.isAlwaysActive, "isAlwaysActive").AddField((INetSerializable) this.furniture, "furniture").AddField((INetSerializable) this.furnitureToRemove.NetFields, "furnitureToRemove.NetFields").AddField((INetSerializable) this.parentLocationName, "parentLocationName").AddField((INetSerializable) this.buildings, "buildings").AddField((INetSerializable) this.animals, "animals").AddField((INetSerializable) this.piecesOfHay, "piecesOfHay").AddField((INetSerializable) this.mapSeats, "mapSeats").AddField((INetSerializable) this.modData, "modData");
    this.mapPath.fieldChangeVisibleEvent += (FieldChange<NetString, string>) ((_param1, _param2, _param3) => this._mapPathDirty = true);
    this.name.fieldChangeVisibleEvent += (FieldChange<NetString, string>) ((_param1, _param2, _param3) => this.OnNameChanged());
    this.uniqueName.fieldChangeVisibleEvent += (FieldChange<NetString, string>) ((_param1, _param2, _param3) => this.OnNameChanged());
    this.parentLocationName.fieldChangeVisibleEvent += (FieldChange<NetString, string>) ((_param1, _param2, _param3) => this.OnParentLocationChanged());
    this.buildings.OnValueAdded += (NetCollection<Building>.ContentsChangeEvent) (b =>
    {
      if (b != null)
      {
        b.parentLocationName.Value = this.NameOrUniqueName;
        b.updateInteriorWarps();
      }
      if (!Game1.IsMasterGame)
        return;
      Game1.netWorldState.Value.UpdateBuildingCache(this);
    });
    this.buildings.OnValueRemoved += (NetCollection<Building>.ContentsChangeEvent) (b =>
    {
      if (b != null)
        b.parentLocationName.Value = (string) null;
      if (!Game1.IsMasterGame)
        return;
      Game1.netWorldState.Value.UpdateBuildingCache(this);
    });
    this.isStructure.fieldChangeVisibleEvent += (FieldChange<NetBool, bool>) ((_param1, _param2, _param3) =>
    {
      if (this.mapPath.Value == null)
        return;
      this.InvalidateCachedMultiplayerMap(Game1.multiplayer.cachedMultiplayerMaps);
      this.reloadMap();
    });
    this.sharedLights.OnValueAdded += (NetDictionary<string, LightSource, NetRef<LightSource>, SerializableDictionary<string, LightSource>, NetStringDictionary<LightSource, NetRef<LightSource>>>.ContentsChangeEvent) ((identifier, light) =>
    {
      if (Game1.currentLocation != this)
        return;
      Game1.currentLightSources.Add(light);
    });
    this.sharedLights.OnValueRemoved += (NetDictionary<string, LightSource, NetRef<LightSource>, SerializableDictionary<string, LightSource>, NetStringDictionary<LightSource, NetRef<LightSource>>>.ContentsChangeEvent) ((identifier, light) =>
    {
      if (Game1.currentLocation != this)
        return;
      Game1.currentLightSources.Remove(light?.Id);
    });
    this.netObjects.OnConflictResolve += (NetDictionary<Vector2, Object, NetRef<Object>, SerializableDictionary<Vector2, Object>, NetVector2Dictionary<Object, NetRef<Object>>>.ConflictResolveEvent) ((pos, rejected, accepted) =>
    {
      if (!Game1.IsMasterGame)
        return;
      Object @object = rejected.Value;
      if (@object == null)
        return;
      @object.onDetachedFromParent();
      @object.dropItem(this, pos * 64f, pos * 64f);
    });
    this.netObjects.OnValueAdded += new NetDictionary<Vector2, Object, NetRef<Object>, SerializableDictionary<Vector2, Object>, NetVector2Dictionary<Object, NetRef<Object>>>.ContentsChangeEvent(this.OnObjectAdded);
    this.overlayObjects.onValueAdded += new Action<Vector2, Object>(this.OnObjectAdded);
    this.removeTemporarySpritesWithIDEvent.onEvent += new AbstractNetEvent1<int>.Event(this.removeTemporarySpritesWithIDLocal);
    this.rumbleAndFadeEvent.onEvent += new AbstractNetEvent1<int>.Event(this.performRumbleAndFade);
    this.damagePlayersEvent.onEvent += new AbstractNetEvent1<GameLocation.DamagePlayersEventArg>.Event(this.performDamagePlayers);
    this.fishSplashPoint.fieldChangeVisibleEvent += (FieldChange<NetPoint, Point>) ((_param1, _param2, _param3) => this.updateFishSplashAnimation());
    this.orePanPoint.fieldChangeVisibleEvent += (FieldChange<NetPoint, Point>) ((_param1, _param2, _param3) => this.updateOrePanAnimation());
    this.characters.OnValueRemoved += (NetCollection<NPC>.ContentsChangeEvent) (npc => npc.Removed());
    this.terrainFeatures.OnValueAdded += (NetDictionary<Vector2, TerrainFeature, NetRef<TerrainFeature>, SerializableDictionary<Vector2, TerrainFeature>, NetVector2Dictionary<TerrainFeature, NetRef<TerrainFeature>>>.ContentsChangeEvent) ((tile, feature) => this.OnTerrainFeatureAdded(feature, tile));
    this.terrainFeatures.OnValueRemoved += (NetDictionary<Vector2, TerrainFeature, NetRef<TerrainFeature>, SerializableDictionary<Vector2, TerrainFeature>, NetVector2Dictionary<TerrainFeature, NetRef<TerrainFeature>>>.ContentsChangeEvent) ((tile, feature) => this.OnTerrainFeatureRemoved(feature));
    this.largeTerrainFeatures.OnValueAdded += (NetCollection<LargeTerrainFeature>.ContentsChangeEvent) (feature => this.OnTerrainFeatureAdded((TerrainFeature) feature, feature.Tile));
    this.largeTerrainFeatures.OnValueRemoved += new NetCollection<LargeTerrainFeature>.ContentsChangeEvent(this.OnTerrainFeatureRemoved);
    this.resourceClumps.OnValueAdded += new NetCollection<ResourceClump>.ContentsChangeEvent(this.OnResourceClumpAdded);
    this.resourceClumps.OnValueRemoved += new NetCollection<ResourceClump>.ContentsChangeEvent(this.OnResourceClumpRemoved);
    this.furniture.OnValueAdded += (NetCollection<Furniture>.ContentsChangeEvent) (f =>
    {
      f.Location = this;
      f.OnAdded(this, f.TileLocation);
    });
    this.furniture.OnValueRemoved += (NetCollection<Furniture>.ContentsChangeEvent) (f => f.OnRemoved(this, f.TileLocation));
    this.furnitureToRemove.Processor = new Action<Guid>(this.removeQueuedFurniture);
  }

  public virtual void InvalidateCachedMultiplayerMap(
    Dictionary<string, CachedMultiplayerMap> cached_data)
  {
    if (Game1.IsMasterGame)
      return;
    cached_data.Remove(this.NameOrUniqueName);
  }

  public virtual void MakeMapModifications(bool force = false)
  {
    if (force)
      this._appliedMapOverrides.Clear();
    this.interiorDoors.MakeMapModifications();
    string str = this.name.Value;
    if (str == null)
      return;
    switch (str.Length)
    {
      case 6:
        if (!(str == "Saloon") || !NetWorldState.checkAnywhereForWorldStateID("saloonSportsRoom"))
          break;
        this.ApplyMapOverride("RefurbishedSaloonRoom", destination_rect: new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(32 /*0x20*/, 1, 6, 8)));
        Game1.currentLightSources.Add(new LightSource("Saloon_1", 1, new Vector2(33f, 7f) * 64f, 4f, onlyLocation: this.NameOrUniqueName));
        Game1.currentLightSources.Add(new LightSource("Saloon_2", 1, new Vector2(36f, 7f) * 64f, 4f, onlyLocation: this.NameOrUniqueName));
        Game1.currentLightSources.Add(new LightSource("Saloon_3", 1, new Vector2(34f, 5f) * 64f, 4f, onlyLocation: this.NameOrUniqueName));
        break;
      case 7:
        if (!(str == "Sunroom"))
          break;
        TileSheet tileSheet = this.map.RequireTileSheet(1, "2");
        string path1 = Path.GetDirectoryName(tileSheet.ImageSource);
        if (string.IsNullOrWhiteSpace(path1))
          path1 = "Maps";
        tileSheet.ImageSource = Path.Combine(path1, "CarolineGreenhouseTiles" + (this.IsRainingHere() || Game1.timeOfDay > Game1.getTrulyDarkTime(this) ? "_rainy" : ""));
        this.map.DisposeTileSheets(Game1.mapDisplayDevice);
        this.map.LoadTileSheets(Game1.mapDisplayDevice);
        break;
      case 8:
        if (!(str == "WitchHut") || !Game1.player.mailReceived.Contains("hasPickedUpMagicInk"))
          break;
        ((IDictionary<string, PropertyValue>) this.setMapTile(4, 11, 113, "Buildings", "untitled tile sheet").Properties).Remove("Action");
        break;
      case 9:
        switch (str[0])
        {
          case 'B':
            if (!(str == "Backwoods"))
              return;
            if (Game1.netWorldState.Value.hasWorldStateID("golemGrave"))
              this.ApplyMapOverride("Backwoods_GraveSite");
            if (Game1.MasterPlayer.mailReceived.Contains("communityUpgradeShortcuts") && !this._appliedMapOverrides.Contains("Backwoods_Staircase"))
            {
              this.ApplyMapOverride("Backwoods_Staircase");
              LargeTerrainFeature largeTerrainFeature1 = (LargeTerrainFeature) null;
              foreach (LargeTerrainFeature largeTerrainFeature2 in this.largeTerrainFeatures)
              {
                if (largeTerrainFeature2.Tile == new Vector2(37f, 16f))
                {
                  largeTerrainFeature1 = largeTerrainFeature2;
                  break;
                }
              }
              if (largeTerrainFeature1 != null)
                this.largeTerrainFeatures.Remove(largeTerrainFeature1);
            }
            if (!Game1.player.mailReceived.Contains("asdlkjfg1") || Game1.random.NextDouble() < 0.01)
            {
              this.setTileProperty(13, 29, "Back", "TouchAction", "asdlfkjg");
              this.setTileProperty(14, 29, "Back", "TouchAction", "asdlfkjg");
              this.setTileProperty(15, 29, "Back", "TouchAction", "asdlfkjg");
            }
            else if (Utility.doesAnyFarmerHaveMail("asdlkjfg1") && Utility.CreateDaySaveRandom(1244.0).NextDouble() < 0.02)
            {
              if (!this.IsTileOccupiedBy(new Vector2(13f, 26f)))
                this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors_1_6", new Microsoft.Xna.Framework.Rectangle(495, 412, 16 /*0x10*/, 16 /*0x10*/), new Vector2(13f, 26f) * 64f, false, 3f / 1000f, Color.White)
                {
                  scale = 4f,
                  layerDepth = 0.0f
                });
              if (!this.IsTileOccupiedBy(new Vector2(12f, 25f)))
                this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors_1_6", new Microsoft.Xna.Framework.Rectangle(495, 412, 16 /*0x10*/, 16 /*0x10*/), new Vector2(12f, 25f) * 64f, true, 3f / 1000f, Color.White)
                {
                  scale = 4f,
                  layerDepth = 0.0f
                });
              if (!this.IsTileOccupiedBy(new Vector2(13f, 24f)))
                this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors_1_6", new Microsoft.Xna.Framework.Rectangle(495, 412, 16 /*0x10*/, 16 /*0x10*/), new Vector2(13f, 24f) * 64f, false, 3f / 1000f, Color.White)
                {
                  scale = 4f,
                  layerDepth = 0.0f
                });
              if (!this.IsTileOccupiedBy(new Vector2(13f, 23f)))
                this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors_1_6", new Microsoft.Xna.Framework.Rectangle(495, 412, 16 /*0x10*/, 16 /*0x10*/), new Vector2(12f, 23f) * 64f, true, 3f / 1000f, Color.White * 0.66f)
                {
                  scale = 4f,
                  layerDepth = 0.0f
                });
              if (!this.IsTileOccupiedBy(new Vector2(13f, 22f)))
                this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors_1_6", new Microsoft.Xna.Framework.Rectangle(495, 412, 16 /*0x10*/, 16 /*0x10*/), new Vector2(13f, 22f) * 64f, false, 3f / 1000f, Color.White * 0.33f)
                {
                  scale = 4f,
                  layerDepth = 0.0f
                });
            }
            if (Game1.timeOfDay < 2400)
              return;
            Random daySaveRandom = Utility.CreateDaySaveRandom(124.0);
            int num1 = Utility.ModifyTime(2400, daySaveRandom.Next(12) * 10);
            if (Game1.timeOfDay != num1 || daySaveRandom.NextDouble() >= 0.33)
              return;
            this.temporarySprites.Add(new TemporaryAnimatedSprite("Characters\\asldkfjsquaskutanfsldk", new Microsoft.Xna.Framework.Rectangle(0, 48 /*0x30*/, 32 /*0x20*/, 48 /*0x30*/), new Vector2(60f, -260f), true, 0.0f, Color.White)
            {
              animationLength = 8,
              totalNumberOfLoops = 99,
              interval = 120f,
              scale = 4f,
              motion = new Vector2(0.5f, 1f),
              yStopCoordinate = 256 /*0x0100*/,
              xStopCoordinate = 256 /*0x0100*/,
              delayBeforeAnimationStart = 1000
            });
            return;
          case 'S':
            if (!(str == "SkullCave"))
              return;
            bool flag = Game1.player.team.skullShrineActivated.Value || Game1.player.team.SpecialOrderRuleActive("SC_HARD");
            if (Game1.player.team.toggleSkullShrineOvernight.Value)
              flag = !flag;
            if (flag)
            {
              this._appliedMapOverrides.Remove("SkullCaveAltarDeactivated");
              this.ApplyMapOverride("SkullCaveAltar", new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, 0, 5, 4)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(10, 1, 5, 4)));
              Game1.currentLightSources.Add(new LightSource("SkullCaveAltar", 4, new Vector2(12f, 3f) * 64f, 1f, LightSource.LightContext.MapLight, onlyLocation: this.NameOrUniqueName));
              AmbientLocationSounds.addSound(new Vector2(12f, 3f), 1);
              return;
            }
            this._appliedMapOverrides.Remove("SkullCaveAltar");
            this.ApplyMapOverride(Game1.temporaryContent.Load<Map>("Maps\\SkullCave"), "SkullCaveAltarDeactivated", new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(10, 1, 5, 4)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(10, 1, 5, 4)));
            Game1.currentLightSources.Remove("SkullCaveAltar");
            AmbientLocationSounds.removeSound(new Vector2(12f, 3f));
            return;
          default:
            return;
        }
      case 10:
        switch (str[0])
        {
          case 'H':
            if (!(str == "HaleyHouse") || !Game1.player.eventsSeen.Contains("463391") || !(Game1.player.spouse != "Emily"))
              return;
            this.setMapTile(14, 4, 2173, "Buildings", "1");
            this.setMapTile(14, 3, 2141, "Buildings", "1");
            this.setMapTile(14, 3, 219, "Back", "1");
            return;
          case 'W':
            if (!(str == "WitchSwamp"))
              return;
            if (Game1.MasterPlayer.mailReceived.Contains("henchmanGone"))
            {
              this.removeTile(20, 29, "Buildings");
              return;
            }
            this.setMapTile(20, 29, 10, "Buildings", "wt");
            return;
          default:
            return;
        }
      case 11:
        if (!(str == "MasteryCave"))
          break;
        int num2 = (int) Game1.stats.Get("MasteryExp");
        int levelsNotSpent = MasteryTrackerMenu.getCurrentMasteryLevel() - (int) Game1.stats.Get("masteryLevelsSpent");
        ShowSkillMastery(4, new Vector2(54f, 98f));
        ShowSkillMastery(2, new Vector2(84f, 82f));
        ShowSkillMastery(0, new Vector2(116f, 82f));
        ShowSkillMastery(0, new Vector2(116f, 82f));
        ShowSkillMastery(1, new Vector2(148f, 82f));
        ShowSkillMastery(3, new Vector2(179f, 98f));
        if (!MasteryTrackerMenu.hasCompletedAllMasteryPlaques())
          break;
        MasteryTrackerMenu.addSpiritCandles(true);
        Game1.changeMusicTrack("grandpas_theme");
        break;

        void ShowSkillMastery(int skill, Vector2 spritePosition)
        {
          uint num = Game1.player.stats.Get(StatKeys.Mastery(skill));
          if (levelsNotSpent > 0 && num == 0U)
            this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors_1_6", new Microsoft.Xna.Framework.Rectangle(16 /*0x10*/, 110, 7, 9), spritePosition * 4f, false, 0.0f, Color.White)
            {
              animationLength = 15,
              interval = 50f,
              totalNumberOfLoops = 999999,
              scale = 4f,
              id = 8765 + skill
            });
          else if (num > 0U)
            MasteryTrackerMenu.addSkillFlairPlaque(skill);
          Game1.changeMusicTrack("Upper_Ambient");
        }
      case 16 /*0x10*/:
        if (!(str == "IslandNorthCave1") || !Game1.player.mailReceived.Contains("FizzIntro"))
          break;
        if (this.getCharacterFromName("Fizz") == null)
        {
          NetCollection<NPC> characters = this.characters;
          NPC npc = new NPC(new AnimatedSprite("Characters\\Fizz", 0, 16 /*0x10*/, 32 /*0x20*/), new Vector2(6f, 3f) * 64f, 2, "Fizz");
          npc.SimpleNonVillagerNPC = true;
          npc.Portrait = Game1.content.Load<Texture2D>("Portraits\\Fizz");
          npc.displayName = Game1.content.LoadString("Strings\\NPCNames:Fizz");
          characters.Add(npc);
          this.removeObjectsAndSpawned(6, 3, 1, 1);
        }
        else
        {
          this.getCharacterFromName("Fizz").SimpleNonVillagerNPC = true;
          this.getCharacterFromName("Fizz").Sprite.SpriteHeight = 32 /*0x20*/;
          this.getCharacterFromName("Fizz").Sprite.UpdateSourceRect();
        }
        Game1.currentLightSources.Add(new LightSource("IslandNorthCave1", 1, new Vector2(6f, 3f) * 64f + new Vector2(32f), 2f, onlyLocation: this.NameOrUniqueName));
        break;
      case 17:
        if (!(str == "AbandonedJojaMart"))
          break;
        if (!Game1.MasterPlayer.hasOrWillReceiveMail("ccMovieTheater"))
        {
          StaticTile[] junimoNoteTileFrames = CommunityCenter.getJunimoNoteTileFrames(0, this.map);
          string layerId = "Buildings";
          Point point = new Point(8, 8);
          this.map.RequireLayer(layerId).Tiles[point.X, point.Y] = (Tile) new AnimatedTile(this.map.RequireLayer(layerId), junimoNoteTileFrames, 70L);
          break;
        }
        this.removeTile(8, 8, "Buildings");
        break;
      case 19:
        if (!(str == "WizardHouseBasement") || !Game1.player.mailReceived.Contains("hasActivatedForestPylon"))
          break;
        this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\terraria_cat", new Microsoft.Xna.Framework.Rectangle(0, 106, 14, 22), new Vector2(16.6f, 2.5f) * 64f, false, 0.0f, Color.White)
        {
          animationLength = 8,
          interval = 100f,
          totalNumberOfLoops = 9999,
          scale = 4f
        });
        break;
    }
  }

  public virtual bool ApplyCachedMultiplayerMap(
    Dictionary<string, CachedMultiplayerMap> cached_data,
    string requested_map_path)
  {
    CachedMultiplayerMap cachedMultiplayerMap;
    if (Game1.IsMasterGame || !cached_data.TryGetValue(this.NameOrUniqueName, out cachedMultiplayerMap))
      return false;
    if (cachedMultiplayerMap.mapPath == requested_map_path)
    {
      this._appliedMapOverrides = cachedMultiplayerMap.appliedMapOverrides;
      this.map = cachedMultiplayerMap.map;
      this.loadedMapPath = cachedMultiplayerMap.loadedMapPath;
      return true;
    }
    cached_data.Remove(this.NameOrUniqueName);
    return false;
  }

  public virtual void StoreCachedMultiplayerMap(
    Dictionary<string, CachedMultiplayerMap> cached_data)
  {
    if (Game1.IsMasterGame)
      return;
    switch (this)
    {
      case VolcanoDungeon _:
        break;
      case MineShaft _:
        break;
      default:
        cached_data[this.NameOrUniqueName] = new CachedMultiplayerMap()
        {
          map = this.map,
          appliedMapOverrides = this._appliedMapOverrides,
          mapPath = this.mapPath.Value,
          loadedMapPath = this.loadedMapPath
        };
        break;
    }
  }

  public virtual void TransferDataFromSavedLocation(GameLocation l)
  {
    this.modData.Clear();
    if (l.modData != null)
    {
      foreach (string key in l.modData.Keys)
        this.modData[key] = l.modData[key];
    }
    this.miniJukeboxCount.Value = l.miniJukeboxCount.Value;
    this.miniJukeboxTrack.Value = l.miniJukeboxTrack.Value;
    this.SelectRandomMiniJukeboxTrack();
    this.UpdateMapSeats();
  }

  /// <summary>Reset cached data when the name or unique name changes.</summary>
  private void OnNameChanged() => this.IsTemporary = GameLocation.IsTemporaryName(this.Name);

  /// <summary>Reset cached data when the parent location changes.</summary>
  private void OnParentLocationChanged()
  {
    this.locationContextId = (string) null;
    if (this.seasonOverride != null && !this.seasonOverride.IsValueCreated)
      return;
    this.seasonOverride = new Lazy<Season?>(new Func<Season?>(this.LoadSeasonOverride));
  }

  /// <summary>Update when the building containing this location is upgraded, if applicable.</summary>
  /// <param name="building">The building containing this location.</param>
  public virtual void OnParentBuildingUpgraded(Building building)
  {
  }

  /// <summary>Update when this location is removed from the game (e.g. a mine level that was unloaded).</summary>
  public virtual void OnRemoved()
  {
    for (int index = this.characters.Count - 1; index >= 0; --index)
      this.characters[index].OnLocationRemoved();
  }

  /// <summary>Handle an object added to the location.</summary>
  /// <param name="tile">The tile position.</param>
  /// <param name="obj">The object that was added.</param>
  protected virtual void OnObjectAdded(Vector2 tile, Object obj)
  {
    obj.Location = this;
    obj.TileLocation = tile;
  }

  /// <summary>Handle a resource clump added to the location.</summary>
  /// <param name="obj">The resource clump that was added.</param>
  public virtual void OnResourceClumpAdded(ResourceClump resourceClump)
  {
    resourceClump.Location = this;
    resourceClump.OnAddedToLocation(this, resourceClump.Tile);
  }

  /// <summary>Handle a resource clump removed from the location.</summary>
  /// <param name="tile">The tile position.</param>
  /// <param name="obj">The resource clump that was removed.</param>
  public virtual void OnResourceClumpRemoved(ResourceClump resourceClump)
  {
    resourceClump.Location = (GameLocation) null;
  }

  /// <summary>Handle a terrain feature added to the location.</summary>
  /// <param name="tile">The tile position.</param>
  /// <param name="obj">The terrain feature that was added.</param>
  public virtual void OnTerrainFeatureAdded(TerrainFeature feature, Vector2 location)
  {
    switch (feature)
    {
      case null:
        return;
      case Flooring flooring:
        flooring.OnAdded(this, location);
        break;
      case HoeDirt hoeDirt:
        hoeDirt.OnAdded(this, location);
        break;
    }
    feature.Location = this;
    feature.Tile = location;
    feature.OnAddedToLocation(this, location);
    this.UpdateTerrainFeatureUpdateSubscription(feature);
  }

  /// <summary>Handle a terrain feature removed from the location.</summary>
  /// <param name="tile">The tile position.</param>
  /// <param name="obj">The terrain feature that was removed.</param>
  public virtual void OnTerrainFeatureRemoved(TerrainFeature feature)
  {
    switch (feature)
    {
      case null:
        return;
      case Flooring flooring:
        flooring.OnRemoved();
        break;
      case HoeDirt hoeDirt:
        hoeDirt.OnRemoved();
        break;
      case LargeTerrainFeature largeTerrainFeature:
        largeTerrainFeature.onDestroy();
        break;
    }
    if (feature.NeedsUpdate)
      this._activeTerrainFeatures.Remove(feature);
    feature.Location = (GameLocation) null;
  }

  public virtual void UpdateTerrainFeatureUpdateSubscription(TerrainFeature feature)
  {
    if (feature.NeedsUpdate)
      this._activeTerrainFeatures.Add(feature);
    else
      this._activeTerrainFeatures.Remove(feature);
  }

  /// <summary>Get the season which currently applies to this location as a numeric index.</summary>
  /// <remarks>Most code should use <see cref="M:StardewValley.GameLocation.GetSeason" /> instead.</remarks>
  public int GetSeasonIndex() => (int) this.GetSeason();

  /// <summary>Read the override season from the map or location context.</summary>
  private Season? LoadSeasonOverride()
  {
    if (this.map == null && this.mapPath.Value != null)
      this.reloadMap();
    string str;
    if (this.map != null && this.map.Properties.TryGetValue("SeasonOverride", out str) && !string.IsNullOrWhiteSpace(str))
    {
      Season parsed;
      if (Utility.TryParseEnum<Season>(str, out parsed))
        return new Season?(parsed);
      Game1.log.Error($"Unable to read SeasonOverride map property value '{str}' for location '{this.NameOrUniqueName}', not a valid season name.");
    }
    return this.GetLocationContext()?.SeasonOverride;
  }

  /// <summary>Get the season which currently applies to this location.</summary>
  public Season GetSeason()
  {
    Season? nullable = this.seasonOverride.Value;
    if (nullable.HasValue)
      return nullable.GetValueOrDefault();
    GameLocation parentLocation = this.GetParentLocation();
    return parentLocation == null ? Game1.season : parentLocation.GetSeason();
  }

  /// <summary>Get the season which currently applies to this location as a string.</summary>
  /// <remarks>Most code should use <see cref="M:StardewValley.GameLocation.GetSeason" /> instead.</remarks>
  public string GetSeasonKey() => Utility.getSeasonKey(this.GetSeason());

  /// <summary>Get whether it's spring in this location's context.</summary>
  /// <remarks>This is a shortcut for convenience. When checking multiple season, consider caching the result from <see cref="M:StardewValley.GameLocation.GetSeason" /> instead.</remarks>
  public bool IsSpringHere() => this.GetSeason() == Season.Spring;

  /// <summary>Get whether it's summer in this location's context.</summary>
  /// <inheritdoc cref="M:StardewValley.GameLocation.IsSpringHere" path="/remarks" />
  public bool IsSummerHere() => this.GetSeason() == Season.Summer;

  /// <summary>Get whether it's fall in this location's context.</summary>
  /// <inheritdoc cref="M:StardewValley.GameLocation.IsSpringHere" path="/remarks" />
  public bool IsFallHere() => this.GetSeason() == Season.Fall;

  /// <summary>Get whether it's winter in this location's context.</summary>
  /// <inheritdoc cref="M:StardewValley.GameLocation.IsSpringHere" path="/remarks" />
  public bool IsWinterHere() => this.GetSeason() == Season.Winter;

  /// <summary>Get the weather which applies in this location's context.</summary>
  public LocationWeather GetWeather()
  {
    return Game1.netWorldState.Value.GetWeatherForLocation(this.GetLocationContextId());
  }

  /// <summary>Get whether it's raining in this location's context (regardless of whether the player is currently indoors and sheltered from the rain).</summary>
  public bool IsRainingHere() => this.GetWeather().IsRaining;

  /// <summary>Get whether it's green raining in this location's context (regardless of whether the player is currently indoors and sheltered from the green rain).</summary>
  public bool IsGreenRainingHere() => this.IsRainingHere() && this.GetWeather().IsGreenRain;

  /// <summary>Get whether it's storming in this location's context (regardless of whether the player is currently indoors and sheltered from the storm).</summary>
  public bool IsLightningHere() => this.GetWeather().IsLightning;

  /// <summary>Get whether it's snowing in this location's context (regardless of whether the player is currently indoors and sheltered from the snow).</summary>
  public bool IsSnowingHere() => this.GetWeather().IsSnowing;

  /// <summary>Get whether it's blowing debris like leaves in this location's context (regardless of whether the player is currently indoors and sheltered from the wind).</summary>
  public bool IsDebrisWeatherHere() => this.GetWeather().IsDebrisWeather;

  /// <summary>Get whether a location name matches the pattern used by temporary locations for events or minigames.</summary>
  /// <param name="name">The location name to check.</param>
  public static bool IsTemporaryName(string name)
  {
    if (string.IsNullOrEmpty(name))
      return false;
    return name.StartsWith("Temp", StringComparison.Ordinal) || name == "fishingGame" || name == "tent";
  }

  private void updateFishSplashAnimation()
  {
    if (this.fishSplashPoint.Value == Point.Zero)
      this.fishSplashAnimation = (TemporaryAnimatedSprite) null;
    else
      this.fishSplashAnimation = new TemporaryAnimatedSprite(51, new Vector2((float) (this.fishSplashPoint.X * 64 /*0x40*/), (float) (this.fishSplashPoint.Y * 64 /*0x40*/)), Color.White, 10, animationInterval: 80f, numberOfLoops: 999999)
      {
        layerDepth = (float) (this.fishSplashPoint.Y * 64 /*0x40*/ - 64 /*0x40*/ - 1) / 10000f
      };
  }

  private void updateOrePanAnimation()
  {
    if (this.orePanPoint.Value == Point.Zero)
      this.orePanAnimation = (TemporaryAnimatedSprite) null;
    else
      this.orePanAnimation = new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(432, 1435, 16 /*0x10*/, 16 /*0x10*/), new Vector2((float) (this.orePanPoint.X * 64 /*0x40*/ + 32 /*0x20*/), (float) (this.orePanPoint.Y * 64 /*0x40*/ + 32 /*0x20*/)), false, 0.0f, Color.White)
      {
        totalNumberOfLoops = 9999999,
        interval = 100f,
        scale = 3f,
        animationLength = 6
      };
  }

  public GameLocation()
  {
    this.NetFields = new NetFields(NetFields.GetNameForInstance<GameLocation>(this));
    this.farmers = new FarmerCollection(this);
    this.interiorDoors = new InteriorDoorDictionary(this);
    this.netAudio = new NetAudio(this);
    this.objects = new OverlaidDictionary(this.netObjects, this.overlayObjects);
    this._appliedMapOverrides = new HashSet<string>();
    this.terrainFeatures.SetEqualityComparer((IEqualityComparer<Vector2>) GameLocation.tilePositionComparer);
    this.netObjects.SetEqualityComparer((IEqualityComparer<Vector2>) GameLocation.tilePositionComparer);
    this.objects.SetEqualityComparer((IEqualityComparer<Vector2>) GameLocation.tilePositionComparer, ref this.netObjects, ref this.overlayObjects);
    this.seasonOverride = new Lazy<Season?>(new Func<Season?>(this.LoadSeasonOverride));
    this.initNetFields();
  }

  public GameLocation(string mapPath, string name)
    : this()
  {
    this.mapPath.Set(mapPath);
    this.name.Value = name;
    if (name.Contains("Farm") || name.Contains("Coop") || name.Contains("Barn") || name.Equals("SlimeHutch"))
      this.isFarm.Value = true;
    if (name == "Greenhouse")
      this.IsGreenhouse = true;
    this.reloadMap();
    this.loadObjects();
  }

  /// <summary>Add the default buildings which should always exist on the farm, if missing.</summary>
  /// <param name="load">Whether to call <see cref="M:StardewValley.Buildings.Building.load" />. This should be true unless you'll be calling it separately.</param>
  public virtual void AddDefaultBuildings(bool load = true)
  {
  }

  /// <summary>Add a default building which should always exist on the farm, if it's missing.</summary>
  /// <param name="id">The building ID in <c>Data/Buildings</c>.</param>
  /// <param name="tile">The tile position at which to construct it.</param>
  /// <param name="load">Whether to call <see cref="M:StardewValley.Buildings.Building.load" />. This should be true unless you'll be calling it separately.</param>
  public virtual void AddDefaultBuilding(string id, Vector2 tile, bool load = true)
  {
    foreach (Building building in this.buildings)
    {
      if (building.buildingType.Value == id)
        return;
    }
    Building instanceFromId = Building.CreateInstanceFromId(id, tile);
    if (load)
      instanceFromId.load();
    this.buildings.Add(instanceFromId);
  }

  /// <summary>Play a sound for each online player in the location if they can hear it.</summary>
  /// <param name="audioName">The sound ID to play.</param>
  /// <param name="position">The tile position from which to play the sound, or <c>null</c> if it should be played throughout the location.</param>
  /// <param name="pitch">The pitch modifier to apply, or <c>null</c> to keep it as-is.</param>
  /// <param name="context">The source which triggered the sound.</param>
  public void playSound(string audioName, Vector2? position = null, int? pitch = null, SoundContext context = SoundContext.Default)
  {
    Game1.sounds.PlayAll(audioName, this, position, pitch, context);
  }

  /// <summary>Play a sound for the current player only, if they can hear it.</summary>
  /// <param name="audioName">The sound ID to play.</param>
  /// <param name="position">The tile position from which to play the sound, or <c>null</c> if not applicable.</param>
  /// <param name="pitch">The pitch modifier to apply, or <c>null</c> to keep it as-is.</param>
  /// <param name="context">The source which triggered the sound.</param>
  public void localSound(string audioName, Vector2? position = null, int? pitch = null, SoundContext context = SoundContext.Default)
  {
    Game1.sounds.PlayLocal(audioName, this, position, pitch, context, out ICue _);
  }

  protected virtual LocalizedContentManager getMapLoader()
  {
    if (!this.isStructure.Value)
      return Game1.game1.xTileContent;
    if (this._structureMapLoader == null)
      this._structureMapLoader = Game1.game1.xTileContent.CreateTemporary();
    return this._structureMapLoader;
  }

  /// <summary>Destroy any organic material like weeds or twigs, and send any player items to the lost and found. Used to clean up areas before map overrides.</summary>
  /// <param name="tile">The tile position to clean up.</param>
  public void cleanUpTileForMapOverride(Point tile)
  {
    this.cleanUpTileForMapOverride(tile, (string) null);
  }

  /// <summary>Destroy any organic material like weeds or twigs, and send any player items to the lost and found. Used to clean up areas before map overrides.</summary>
  /// <param name="tile">The tile position to clean up.</param>
  /// <param name="exceptItemId">If set, an item on this spot won't be moved if its item ID matches this one.</param>
  public void cleanUpTileForMapOverride(Point tile, string exceptItemId)
  {
    Vector2 vector2 = Utility.PointToVector2(tile);
    Point tileCenterPoint = Utility.Vector2ToPoint(vector2 * new Vector2(64f) + new Vector2(32f, 32f));
    NetCollection<Item> lostAndFound = Game1.player.team.returnedDonations;
    Object @object;
    if (this.Objects.TryGetValue(vector2, out @object) && (exceptItemId == null || !ItemRegistry.HasItemId((Item) @object, exceptItemId)))
    {
      if ((@object == null ? 0 : (@object.HasBeenInInventory ? 1 : (@object.isDebrisOrForage() || !(@object.QualifiedItemId != "(O)590") ? 0 : (@object.QualifiedItemId != "(O)SeedSpot" ? 1 : 0)))) != 0)
      {
        if (@object is Chest chest)
        {
          foreach (Item obj in chest.Items)
            lostAndFound.Add(obj);
          chest.Items.Clear();
        }
        else if (@object.readyForHarvest.Value && (NetFieldBase<Object, NetRef<Object>>) @object.heldObject != (NetRef<Object>) null)
        {
          lostAndFound.Add((Item) @object.heldObject.Value);
          @object.heldObject.Value = (Object) null;
        }
        lostAndFound.Add((Item) @object);
        Game1.player.team.newLostAndFoundItems.Value = true;
      }
      this.objects.Remove(vector2);
    }
    this.furniture.RemoveWhere((Func<Furniture, bool>) (item =>
    {
      if (!item.GetBoundingBox().Contains(tileCenterPoint) || exceptItemId != null && ItemRegistry.HasItemId((Item) item, exceptItemId))
        return false;
      if (item.heldObject.Value != null)
      {
        lostAndFound.Add((Item) item.heldObject.Value);
        item.heldObject.Value = (Object) null;
      }
      lostAndFound.Add((Item) item);
      return true;
    }));
    this.terrainFeatures.Remove(vector2);
    this.largeTerrainFeatures.RemoveWhere((Func<LargeTerrainFeature, bool>) (feature => feature.getBoundingBox().Contains(tileCenterPoint)));
    this.resourceClumps.RemoveWhere((Func<ResourceClump, bool>) (clump => clump.getBoundingBox().Contains(tileCenterPoint)));
  }

  public void ApplyMapOverride(
    Map override_map,
    string override_key,
    Microsoft.Xna.Framework.Rectangle? source_rect = null,
    Microsoft.Xna.Framework.Rectangle? dest_rect = null,
    Action<Point> perTileCustomAction = null)
  {
    if (this._appliedMapOverrides.Contains(override_key))
      return;
    this._appliedMapOverrides.Add(override_key);
    this.updateSeasonalTileSheets(override_map);
    Dictionary<TileSheet, TileSheet> dictionary1 = new Dictionary<TileSheet, TileSheet>();
    foreach (TileSheet tileSheet1 in override_map.TileSheets)
    {
      TileSheet tileSheet2 = this.map.GetTileSheet(tileSheet1.Id);
      string str1 = "";
      string str2 = "";
      if (tileSheet2 != null)
        str1 = tileSheet2.ImageSource;
      if (str2 != null)
        str2 = tileSheet1.ImageSource;
      if (tileSheet2 == null || str2 != str1)
      {
        tileSheet2 = new TileSheet(GameLocation.GetAddedMapOverrideTilesheetId(override_key, tileSheet1.Id), this.map, tileSheet1.ImageSource, tileSheet1.SheetSize, tileSheet1.TileSize);
        for (int tileIndex = 0; tileIndex < tileSheet1.TileCount; ++tileIndex)
          tileSheet2.TileIndexProperties[tileIndex].CopyFrom(tileSheet1.TileIndexProperties[tileIndex]);
        this.map.AddTileSheet(tileSheet2);
      }
      else if (tileSheet2.TileCount < tileSheet1.TileCount)
      {
        int tileCount = tileSheet2.TileCount;
        tileSheet2.SheetWidth = tileSheet1.SheetWidth;
        tileSheet2.SheetHeight = tileSheet1.SheetHeight;
        for (int tileIndex = tileCount; tileIndex < tileSheet1.TileCount; ++tileIndex)
          tileSheet2.TileIndexProperties[tileIndex].CopyFrom(tileSheet1.TileIndexProperties[tileIndex]);
      }
      dictionary1[tileSheet1] = tileSheet2;
    }
    Dictionary<Layer, Layer> dictionary2 = new Dictionary<Layer, Layer>();
    int num1 = 0;
    int num2 = 0;
    for (int index = 0; index < override_map.Layers.Count; ++index)
    {
      num1 = Math.Max(num1, override_map.Layers[index].LayerWidth);
      num2 = Math.Max(num2, override_map.Layers[index].LayerHeight);
    }
    if (!source_rect.HasValue)
      source_rect = new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, 0, num1, num2));
    int num3 = 0;
    int num4 = 0;
    for (int index = 0; index < this.map.Layers.Count; ++index)
    {
      num3 = Math.Max(num3, this.map.Layers[index].LayerWidth);
      num4 = Math.Max(num4, this.map.Layers[index].LayerHeight);
    }
    bool flag1 = false;
    for (int index = 0; index < override_map.Layers.Count; ++index)
    {
      Layer layer = this.map.GetLayer(override_map.Layers[index].Id);
      if (layer == null)
      {
        layer = new Layer(override_map.Layers[index].Id, this.map, new Size(num3, num4), override_map.Layers[index].TileSize);
        this.map.AddLayer(layer);
        flag1 = true;
      }
      dictionary2[override_map.Layers[index]] = layer;
    }
    if (flag1)
      this.SortLayers();
    if (!dest_rect.HasValue)
      dest_rect = new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, 0, num3, num4));
    int x1 = source_rect.Value.X;
    int y1 = source_rect.Value.Y;
    int x2 = dest_rect.Value.X;
    int y2 = dest_rect.Value.Y;
    for (int index1 = 0; index1 < source_rect.Value.Width; ++index1)
    {
      for (int index2 = 0; index2 < source_rect.Value.Height; ++index2)
      {
        Point point1 = new Point(x1 + index1, y1 + index2);
        Point point2 = new Point(x2 + index1, y2 + index2);
        if (perTileCustomAction != null)
          perTileCustomAction(point2);
        bool flag2 = false;
        for (int index3 = 0; index3 < override_map.Layers.Count; ++index3)
        {
          Layer layer1 = override_map.Layers[index3];
          Layer layer2 = dictionary2[layer1];
          if (layer2 != null && point2.X < layer2.LayerWidth && point2.Y < layer2.LayerHeight && (flag2 || override_map.Layers[index3].Tiles[point1.X, point1.Y] != null))
          {
            flag2 = true;
            if (point1.X < layer1.LayerWidth && point1.Y < layer1.LayerHeight)
            {
              if (layer1.Tiles[point1.X, point1.Y] == null)
              {
                layer2.Tiles[point2.X, point2.Y] = (Tile) null;
              }
              else
              {
                Tile tile1 = layer1.Tiles[point1.X, point1.Y];
                Tile tile2 = (Tile) null;
                if (!(tile1 is StaticTile))
                {
                  if (tile1 is AnimatedTile animatedTile)
                  {
                    StaticTile[] tileFrames = new StaticTile[animatedTile.TileFrames.Length];
                    for (int index4 = 0; index4 < animatedTile.TileFrames.Length; ++index4)
                    {
                      StaticTile tileFrame = animatedTile.TileFrames[index4];
                      tileFrames[index4] = new StaticTile(layer2, dictionary1[tileFrame.TileSheet], tileFrame.BlendMode, tileFrame.TileIndex);
                    }
                    tile2 = (Tile) new AnimatedTile(layer2, tileFrames, animatedTile.FrameInterval);
                  }
                }
                else
                  tile2 = (Tile) new StaticTile(layer2, dictionary1[tile1.TileSheet], tile1.BlendMode, tile1.TileIndex);
                tile2?.Properties.CopyFrom(tile1.Properties);
                layer2.Tiles[point2.X, point2.Y] = tile2;
              }
            }
          }
        }
      }
    }
    this.map.LoadTileSheets(Game1.mapDisplayDevice);
    if (!Game1.IsMasterGame && !this.IsTemporary)
      return;
    this._mapSeatsDirty = true;
  }

  /// <summary>Get the generated tilesheet ID for a tilesheet added to the map via <see cref="M:StardewValley.GameLocation.ApplyMapOverride(xTile.Map,System.String,System.Nullable{Microsoft.Xna.Framework.Rectangle},System.Nullable{Microsoft.Xna.Framework.Rectangle},System.Action{Microsoft.Xna.Framework.Point})" />.</summary>
  /// <param name="overrideKey">The map override ID.</param>
  /// <param name="tilesheetId">The tilesheet ID in the applied override map.</param>
  /// <remarks>Note that this tilesheet ID is only used when adding a new tilesheet to the map. If the tilesheet already exists in the base map with the same asset path, it's reused as-is.</remarks>
  public static string GetAddedMapOverrideTilesheetId(string overrideKey, string tilesheetId)
  {
    return $"{"zzzzz"}_{overrideKey}_{tilesheetId}";
  }

  public virtual bool RunLocationSpecificEventCommand(
    Event current_event,
    string command_string,
    bool first_run,
    params string[] args)
  {
    return true;
  }

  public bool hasActiveFireplace()
  {
    for (int index = 0; index < this.furniture.Count; ++index)
    {
      if (this.furniture[index].furniture_type.Value == 14 && this.furniture[index].isOn.Value)
        return true;
    }
    return false;
  }

  public void ApplyMapOverride(
    string map_name,
    Microsoft.Xna.Framework.Rectangle? source_rect = null,
    Microsoft.Xna.Framework.Rectangle? destination_rect = null)
  {
    if (this._appliedMapOverrides.Contains(map_name))
      return;
    this.ApplyMapOverride(Game1.game1.xTileContent.Load<Map>("Maps\\" + map_name), map_name, source_rect, destination_rect);
  }

  public void ApplyMapOverride(
    string map_name,
    string override_key_name,
    Microsoft.Xna.Framework.Rectangle? source_rect = null,
    Microsoft.Xna.Framework.Rectangle? destination_rect = null)
  {
    if (this._appliedMapOverrides.Contains(override_key_name))
      return;
    this.ApplyMapOverride(Game1.game1.xTileContent.Load<Map>("Maps\\" + map_name), override_key_name, source_rect, destination_rect);
  }

  public virtual void UpdateMapSeats()
  {
    this._mapSeatsDirty = false;
    if (!Game1.IsMasterGame && !this.IsTemporary)
      return;
    Dictionary<string, string> dictionary1 = new Dictionary<string, string>();
    Dictionary<string, string> dictionary2 = DataLoader.ChairTiles(Game1.content);
    this.mapSeats.Clear();
    Layer layer = this.map.GetLayer("Buildings");
    if (layer == null)
      return;
    for (int x = 0; x < layer.LayerWidth; ++x)
    {
      for (int y = 0; y < layer.LayerHeight; ++y)
      {
        Tile tile = layer.Tiles[x, y];
        if (tile != null)
        {
          string key1 = Path.GetFileNameWithoutExtension(tile.TileSheet.ImageSource);
          string str;
          if (dictionary1.TryGetValue(key1, out str))
          {
            key1 = str;
          }
          else
          {
            if (key1.StartsWith("summer_") || key1.StartsWith("winter_") || key1.StartsWith("fall_"))
              key1 = "spring_" + key1.Substring(key1.IndexOf('_') + 1);
            dictionary1[key1] = key1;
          }
          int sheetWidth = tile.TileSheet.SheetWidth;
          int num1 = tile.TileIndex % sheetWidth;
          int num2 = tile.TileIndex / sheetWidth;
          string key2 = $"{key1}/{num1.ToString()}/{num2.ToString()}";
          string data;
          if (dictionary2.TryGetValue(key2, out data))
          {
            MapSeat mapSeat = MapSeat.FromData(data, x, y);
            if (mapSeat != null)
              this.mapSeats.Add(mapSeat);
          }
        }
      }
    }
  }

  public virtual void SortLayers()
  {
    this.backgroundLayers.Clear();
    this.buildingLayers.Clear();
    this.frontLayers.Clear();
    this.alwaysFrontLayers.Clear();
    Dictionary<string, List<KeyValuePair<Layer, int>>> dictionary = new Dictionary<string, List<KeyValuePair<Layer, int>>>();
    dictionary["Back"] = this.backgroundLayers;
    dictionary["Buildings"] = this.buildingLayers;
    dictionary["Front"] = this.frontLayers;
    dictionary["AlwaysFront"] = this.alwaysFrontLayers;
    foreach (Layer layer in this.map.Layers)
    {
      foreach (string key in dictionary.Keys)
      {
        if (layer.Id.StartsWith(key))
        {
          int result = 0;
          string s = layer.Id.Substring(key.Length);
          if (s.Length <= 0 || int.TryParse(s, out result))
          {
            dictionary[key].Add(new KeyValuePair<Layer, int>(layer, result));
            break;
          }
        }
      }
    }
    foreach (List<KeyValuePair<Layer, int>> keyValuePairList in dictionary.Values)
      keyValuePairList.Sort((Comparison<KeyValuePair<Layer, int>>) ((a, b) => a.Value.CompareTo(b.Value)));
  }

  public virtual void OnMapLoad(Map map)
  {
  }

  public void loadMap(string mapPath, bool force_reload = false)
  {
    if (force_reload)
    {
      LocalizedContentManager contentManager = Program.gamePtr.CreateContentManager(Game1.content.ServiceProvider, Game1.content.RootDirectory);
      this.map = contentManager.Load<Map>(mapPath);
      contentManager.Unload();
      this.InvalidateCachedMultiplayerMap(Game1.multiplayer.cachedMultiplayerMaps);
    }
    else if (!this.ApplyCachedMultiplayerMap(Game1.multiplayer.cachedMultiplayerMaps, mapPath))
      this.map = this.getMapLoader().Load<Map>(mapPath);
    this.loadedMapPath = mapPath;
    this.OnMapLoad(this.map);
    this.SortLayers();
    if (this.map.Properties.ContainsKey("Outdoors"))
      this.isOutdoors.Value = true;
    if (this.map.Properties.ContainsKey("IsFarm"))
      this.isFarm.Value = true;
    if (this.map.Properties.ContainsKey("IsGreenhouse"))
      this.isGreenhouse.Value = true;
    if (this.HasMapPropertyWithValue("forceLoadPathLayerLights"))
      this.forceLoadPathLayerLights = true;
    if (this.HasMapPropertyWithValue("TreatAsOutdoors"))
      this.treatAsOutdoors.Value = true;
    this.updateSeasonalTileSheets(this.map);
    this.map.LoadTileSheets(Game1.mapDisplayDevice);
    if (Game1.IsMasterGame || this.IsTemporary)
      this._mapSeatsDirty = true;
    if ((this.isOutdoors.Value || this.HasMapPropertyWithValue("indoorWater") || this is Sewer || this is Submarine) && !(this is Desert))
    {
      this.waterTiles = new WaterTiles(this.map.Layers[0].LayerWidth, this.map.Layers[0].LayerHeight);
      bool flag = false;
      for (int index1 = 0; index1 < this.map.Layers[0].LayerWidth; ++index1)
      {
        for (int index2 = 0; index2 < this.map.Layers[0].LayerHeight; ++index2)
        {
          string str = this.doesTileHaveProperty(index1, index2, "Water", "Back");
          if (str != null)
          {
            flag = true;
            if (str == "I")
              this.waterTiles.waterTiles[index1, index2] = new WaterTiles.WaterTileData(true, false);
            else
              this.waterTiles[index1, index2] = true;
          }
        }
      }
      if (!flag)
        this.waterTiles = (WaterTiles) null;
    }
    if (this.isOutdoors.Value)
      this.critters = new List<Critter>();
    this.loadLights();
  }

  public virtual void HandleGrassGrowth(int dayOfMonth)
  {
    if (dayOfMonth == 1)
    {
      if (this is Farm || this.HasMapPropertyWithValue("ClearEmptyDirtOnNewMonth"))
        this.terrainFeatures.RemoveWhere((Func<KeyValuePair<Vector2, TerrainFeature>, bool>) (pair => pair.Value is HoeDirt hoeDirt && hoeDirt.crop == null && Game1.random.NextDouble() < 0.8));
      if (this is Farm || this.HasMapPropertyWithValue("SpawnDebrisOnNewMonth"))
        this.spawnWeedsAndStones(20, spawnFromOldWeeds: false);
      if (Game1.IsSpring && Game1.stats.DaysPlayed > 1U)
      {
        if (this is Farm || this.HasMapPropertyWithValue("SpawnDebrisOnNewYear"))
        {
          this.spawnWeedsAndStones(40, spawnFromOldWeeds: false);
          this.spawnWeedsAndStones(40, true, false);
        }
        if (this is Farm || this.HasMapPropertyWithValue("SpawnRandomGrassOnNewYear"))
        {
          for (int index = 0; index < 15; ++index)
          {
            int num1 = Game1.random.Next(this.map.DisplayWidth / 64 /*0x40*/);
            int num2 = Game1.random.Next(this.map.DisplayHeight / 64 /*0x40*/);
            Vector2 vector2 = new Vector2((float) num1, (float) num2);
            Object @object;
            this.objects.TryGetValue(vector2, out @object);
            if (@object == null && this.doesTileHaveProperty(num1, num2, "Diggable", "Back") != null && !this.IsNoSpawnTile(vector2) && this.isTileLocationOpen(new Location(num1, num2)) && !this.IsTileOccupiedBy(vector2) && !this.isWaterTile(num1, num2))
            {
              int which = 1;
              if (Game1.GetFarmTypeID() == "MeadowlandsFarm" && Game1.random.NextDouble() < 0.2)
                which = 7;
              this.terrainFeatures.Add(vector2, (TerrainFeature) new Grass(which, 4));
            }
          }
          this.growWeedGrass(40);
        }
        if (this.HasMapPropertyWithValue("SpawnGrassFromPathsOnNewYear"))
        {
          Layer layer = this.map.GetLayer("Paths");
          if (layer != null)
          {
            for (int x = 0; x < layer.LayerWidth; ++x)
            {
              for (int y = 0; y < layer.LayerHeight; ++y)
              {
                Vector2 vector2 = new Vector2((float) x, (float) y);
                Object @object;
                this.objects.TryGetValue(vector2, out @object);
                if (@object == null && this.getTileIndexAt(x, y, "Paths") == 22 && this.isTileLocationOpen(vector2) && !this.IsTileOccupiedBy(vector2))
                  this.terrainFeatures.Add(vector2, (TerrainFeature) new Grass(1, 4));
              }
            }
          }
        }
      }
    }
    if (!(this is Farm) && !this.HasMapPropertyWithValue("EnableGrassSpread") || this.IsWinterHere() && !this.HasMapPropertyWithValue("AllowGrassGrowInWinter"))
      return;
    this.growWeedGrass(1);
  }

  public void reloadMap()
  {
    if (this.mapPath.Value != null)
      this.loadMap(this.mapPath.Value);
    else
      this.map = (Map) null;
    this.loadedMapPath = this.mapPath.Value;
  }

  public virtual bool canSlimeMateHere() => true;

  public virtual bool canSlimeHatchHere() => true;

  public void addCharacter(NPC character) => this.characters.Add(character);

  public static Microsoft.Xna.Framework.Rectangle getSourceRectForObject(int tileIndex)
  {
    return new Microsoft.Xna.Framework.Rectangle(tileIndex * 16 /*0x10*/ % Game1.objectSpriteSheet.Width, tileIndex * 16 /*0x10*/ / Game1.objectSpriteSheet.Width * 16 /*0x10*/, 16 /*0x10*/, 16 /*0x10*/);
  }

  public Warp isCollidingWithWarp(Microsoft.Xna.Framework.Rectangle position, Character character)
  {
    if (this.ignoreWarps)
      return (Warp) null;
    foreach (Warp warp in (NetList<Warp, NetRef<Warp>>) this.warps)
    {
      if ((character is NPC || !warp.npcOnly.Value) && (warp.X == (int) Math.Floor((double) position.Left / 64.0) || warp.X == (int) Math.Floor((double) position.Right / 64.0)) && (warp.Y == (int) Math.Floor((double) position.Top / 64.0) || warp.Y == (int) Math.Floor((double) position.Bottom / 64.0)))
      {
        switch (warp.TargetName)
        {
          case "BoatTunnel":
            if (character is NPC)
              return new Warp(warp.X, warp.Y, "IslandSouth", 17, 43, false);
            break;
          case "VolcanoEntrance":
            return new Warp(warp.X, warp.Y, VolcanoDungeon.GetLevelName(0), warp.TargetX, warp.TargetY, false);
        }
        return warp;
      }
    }
    return (Warp) null;
  }

  public Warp isCollidingWithWarpOrDoor(Microsoft.Xna.Framework.Rectangle position, Character character = null)
  {
    return this.isCollidingWithWarp(position, character) ?? this.isCollidingWithDoors(position, character);
  }

  public virtual Warp isCollidingWithDoors(Microsoft.Xna.Framework.Rectangle position, Character character = null)
  {
    for (int corner = 0; corner < 4; ++corner)
    {
      Vector2 cornersOfThisRectangle = Utility.getCornersOfThisRectangle(ref position, corner);
      Point point = new Point((int) cornersOfThisRectangle.X / 64 /*0x40*/, (int) cornersOfThisRectangle.Y / 64 /*0x40*/);
      foreach (KeyValuePair<Point, string> pair in this.doors.Pairs)
      {
        Point key = pair.Key;
        if (point == key)
        {
          Warp warpFromDoor = this.getWarpFromDoor(key, character);
          if (warpFromDoor != null)
            return warpFromDoor;
        }
      }
      foreach (Building building in this.buildings)
      {
        if (building.HasIndoors())
        {
          Point pointForHumanDoor = building.getPointForHumanDoor();
          if (point == pointForHumanDoor)
          {
            Warp warpFromDoor = this.getWarpFromDoor(pointForHumanDoor, character);
            if (warpFromDoor != null)
              return warpFromDoor;
          }
        }
      }
    }
    return (Warp) null;
  }

  public virtual Warp getWarpFromDoor(Point door, Character character = null)
  {
    foreach (Building building in this.buildings)
    {
      if (door == building.getPointForHumanDoor())
      {
        GameLocation indoors = building.GetIndoors();
        if (indoors != null)
          return new Warp(door.X, door.Y, indoors.NameOrUniqueName, indoors.warps[0].X, indoors.warps[0].Y - 1, false);
      }
    }
    string[] propertySplitBySpaces = this.GetTilePropertySplitBySpaces("Action", "Buildings", door.X, door.Y);
    string str = ArgUtility.Get(propertySplitBySpaces, 0, "");
    if (str != null)
    {
      switch (str.Length)
      {
        case 4:
          if (str == "Warp")
            break;
          goto label_27;
        case 14:
          switch (str[4])
          {
            case 'B':
              if (str == "WarpBoatTunnel")
                return !(character is NPC) ? new Warp(door.X, door.Y, "BoatTunnel", 6, 11, false) : new Warp(door.X, door.Y, "IslandSouth", 17, 43, false);
              goto label_27;
            case 'M':
              if (str == "WarpMensLocker")
                break;
              goto label_27;
            case 'e':
              if (str == "LockedDoorWarp")
                break;
              goto label_27;
            default:
              goto label_27;
          }
          break;
        case 16 /*0x10*/:
          if (str == "WarpWomensLocker")
            break;
          goto label_27;
        case 17:
          if (str == "Warp_Sunroom_Door")
            return new Warp(door.X, door.Y, "Sunroom", 5, 13, false);
          goto label_27;
        case 19:
          if (str == "WarpCommunityCenter")
            return new Warp(door.X, door.Y, "CommunityCenter", 32 /*0x20*/, 23, false);
          goto label_27;
        default:
          goto label_27;
      }
    }
    else
      goto label_27;
label_22:
    Point point;
    string error;
    string targetName;
    if (!ArgUtility.TryGetPoint(propertySplitBySpaces, 1, out point, out error, "Point tile") || !ArgUtility.TryGet(propertySplitBySpaces, 3, out targetName, out error, name: "string locationName"))
    {
      this.LogTileActionError(propertySplitBySpaces, door.X, door.Y, error);
      return (Warp) null;
    }
    return !(targetName == "BoatTunnel") || !(character is NPC) ? new Warp(door.X, door.Y, targetName, point.X, point.Y, false) : new Warp(door.X, door.Y, "IslandSouth", 17, 43, false);
label_27:
    if (!str.Contains("Warp"))
      return (Warp) null;
    Game1.log.Warn($"Door in {this.NameOrUniqueName} ({door}) has unknown warp property '{string.Join(" ", propertySplitBySpaces)}', parsing with legacy logic.");
    goto label_22;
  }

  /// <summary>Get the first warp which the player can use to leave the location, accounting for any gender restrictions and NPC-only flags if possible.</summary>
  public Warp GetFirstPlayerWarp()
  {
    Warp warp1 = (Warp) null;
    foreach (Warp warp2 in (NetList<Warp, NetRef<Warp>>) this.warps)
    {
      if (!warp2.npcOnly.Value)
      {
        Gender gender;
        if (!WarpPathfindingCache.GenderRestrictions.TryGetValue(warp2.TargetName, out gender) || gender == Game1.player.Gender)
          return warp2;
        if (warp1 == null)
          warp1 = warp2;
      }
    }
    return warp1 ?? this.warps.FirstOrDefault<Warp>();
  }

  public void addResourceClumpAndRemoveUnderlyingTerrain(
    int resourceClumpIndex,
    int width,
    int height,
    Vector2 tile)
  {
    this.removeObjectsAndSpawned((int) tile.X, (int) tile.Y, width, height);
    this.resourceClumps.Add(new ResourceClump(resourceClumpIndex, width, height, tile));
  }

  public virtual bool canFishHere() => true;

  /// <summary>Get whether a player can resume in this location after waking up, instead of being warped home.</summary>
  /// <param name="who">The player who's waking up here.</param>
  /// <param name="tile">The tile at which they're waking up, or <c>null</c> to use <see cref="F:StardewValley.Farmer.lastSleepPoint" />.</param>
  public virtual bool CanWakeUpHere(Farmer who, Point? tile = null)
  {
    Point point = tile ?? who.lastSleepPoint.Value;
    bool parsed;
    return BedFurniture.IsBedHere(this, point.X, point.Y) || who.sleptInTemporaryBed.Value || this is IslandFarmHouse || this.TryGetMapPropertyAs("AllowWakeUpWithoutBed", out parsed) & parsed;
  }

  public virtual bool CanRefillWateringCanOnTile(int tileX, int tileY)
  {
    Building buildingAt = this.getBuildingAt(new Vector2((float) tileX, (float) tileY));
    if ((buildingAt != null ? (buildingAt.CanRefillWateringCan() ? 1 : 0) : 0) != 0 || this.isWaterTile(tileX, tileY) || this.doesTileHaveProperty(tileX, tileY, "WaterSource", "Back") != null)
      return true;
    if (this.isOutdoors.Value || !(this.doesTileHaveProperty(tileX, tileY, "Action", "Buildings") == "kitchen"))
      return false;
    return this.getTileIndexAt(tileX, tileY, "Buildings", "untitled tile sheet") == 172 || this.getTileIndexAt(tileX, tileY, "Buildings", "untitled tile sheet") == 257;
  }

  public virtual bool isTileBuildingFishable(int tileX, int tileY)
  {
    Vector2 tile = new Vector2((float) tileX, (float) tileY);
    foreach (Building building in this.buildings)
    {
      if (building.isTileFishable(tile))
        return true;
    }
    return false;
  }

  public virtual bool isTileFishable(int tileX, int tileY)
  {
    return this.isTileBuildingFishable(tileX, tileY) || this.isWaterTile(tileX, tileY) && this.doesTileHaveProperty(tileX, tileY, "NoFishing", "Back") == null && !this.hasTileAt(tileX, tileY, "Buildings") || this.doesTileHaveProperty(tileX, tileY, "Water", "Buildings") != null;
  }

  public bool isFarmerCollidingWithAnyCharacter()
  {
    if (this.characters.Count > 0)
    {
      Microsoft.Xna.Framework.Rectangle boundingBox = Game1.player.GetBoundingBox();
      foreach (NPC character in this.characters)
      {
        if (character != null && boundingBox.Intersects(character.GetBoundingBox()))
          return true;
      }
    }
    return false;
  }

  public bool isCollidingPosition(Microsoft.Xna.Framework.Rectangle position, xTile.Dimensions.Rectangle viewport, Character character)
  {
    return this.isCollidingPosition(position, viewport, character is Farmer, 0, false, character, false);
  }

  public virtual bool isCollidingPosition(
    Microsoft.Xna.Framework.Rectangle position,
    xTile.Dimensions.Rectangle viewport,
    bool isFarmer,
    int damagesFarmer,
    bool glider,
    Character character)
  {
    return this.isCollidingPosition(position, viewport, character is Farmer, damagesFarmer, glider, character, false);
  }

  protected bool _TestCornersWorld(
    int top,
    int bottom,
    int left,
    int right,
    Func<int, int, bool> action)
  {
    return action(right, top) || action(right, bottom) || action(left, top) || action(left, bottom);
  }

  protected bool _TestCornersTiles(
    Vector2 top_right,
    Vector2 top_left,
    Vector2 bottom_right,
    Vector2 bottom_left,
    Vector2 top_mid,
    Vector2 bottom_mid,
    Vector2? player_top_right,
    Vector2? player_top_left,
    Vector2? player_bottom_right,
    Vector2? player_bottom_left,
    Vector2? player_top_mid,
    Vector2? player_bottom_mid,
    bool bigger_than_tile,
    Func<Vector2, bool> action)
  {
    this._visitedCollisionTiles.Clear();
    Vector2? nullable1 = player_top_right;
    Vector2 vector2_1 = top_right;
    if ((nullable1.HasValue ? (nullable1.GetValueOrDefault() != vector2_1 ? 1 : 0) : 1) != 0 && this._visitedCollisionTiles.Add(top_right) && action(top_right))
      return true;
    Vector2? nullable2 = player_top_left;
    Vector2 vector2_2 = top_left;
    if ((nullable2.HasValue ? (nullable2.GetValueOrDefault() != vector2_2 ? 1 : 0) : 1) != 0 && this._visitedCollisionTiles.Add(top_left) && action(top_left))
      return true;
    Vector2 vector2_3 = bottom_left;
    nullable2 = player_bottom_left;
    if ((nullable2.HasValue ? (vector2_3 != nullable2.GetValueOrDefault() ? 1 : 0) : 1) != 0 && this._visitedCollisionTiles.Add(bottom_left) && action(bottom_left))
      return true;
    Vector2 vector2_4 = bottom_right;
    nullable2 = player_bottom_right;
    if ((nullable2.HasValue ? (vector2_4 != nullable2.GetValueOrDefault() ? 1 : 0) : 1) != 0 && this._visitedCollisionTiles.Add(bottom_right) && action(bottom_right))
      return true;
    if (bigger_than_tile)
    {
      nullable2 = player_top_mid;
      Vector2 vector2_5 = top_mid;
      if ((nullable2.HasValue ? (nullable2.GetValueOrDefault() != vector2_5 ? 1 : 0) : 1) != 0 && this._visitedCollisionTiles.Add(top_mid) && action(top_mid))
        return true;
      nullable2 = player_bottom_mid;
      Vector2 vector2_6 = bottom_mid;
      if ((nullable2.HasValue ? (nullable2.GetValueOrDefault() != vector2_6 ? 1 : 0) : 1) != 0 && this._visitedCollisionTiles.Add(bottom_mid) && action(bottom_mid))
        return true;
    }
    return false;
  }

  public Furniture GetFurnitureAt(Vector2 tile_position)
  {
    Point point = new Point();
    point.X = (int) ((double) (int) tile_position.X + 0.5) * 64 /*0x40*/;
    point.Y = (int) ((double) (int) tile_position.Y + 0.5) * 64 /*0x40*/;
    foreach (Furniture furnitureAt in this.furniture)
    {
      if (!furnitureAt.isPassable() && furnitureAt.GetBoundingBox().Contains(point))
        return furnitureAt;
    }
    foreach (Furniture furnitureAt in this.furniture)
    {
      if (furnitureAt.isPassable() && furnitureAt.GetBoundingBox().Contains(point))
        return furnitureAt;
    }
    return (Furniture) null;
  }

  public virtual Microsoft.Xna.Framework.Rectangle GetBuildableRectangle()
  {
    if (!this._buildableTileRect.HasValue)
    {
      Microsoft.Xna.Framework.Rectangle parsed;
      this._buildableTileRect = new Microsoft.Xna.Framework.Rectangle?(this.TryGetMapPropertyAs("ValidBuildRect", out parsed) ? parsed : Microsoft.Xna.Framework.Rectangle.Empty);
      this._looserBuildRestrictions = this.HasMapPropertyWithValue("LooserBuildRestrictions");
    }
    return this._buildableTileRect.Value;
  }

  public virtual bool IsBuildableLocation()
  {
    if (this.HasMapPropertyWithValue("CanBuildHere"))
    {
      if (!Game1.multiplayer.isAlwaysActiveLocation(this))
      {
        if (!this.showedBuildableButNotAlwaysActiveWarning)
        {
          Game1.log.Warn($"Location {this.NameOrUniqueName} has the CanBuildHere map property set, but its {"AlwaysActive"} option is disabled, so building is disabled here.");
          this.showedBuildableButNotAlwaysActiveWarning = true;
        }
        return false;
      }
      string mapProperty = this.getMapProperty("BuildConditions");
      if (string.IsNullOrEmpty(mapProperty) || GameStateQuery.CheckConditions(mapProperty, this))
        return true;
    }
    return false;
  }

  /// <summary>Get whether a pixel area is fully outside the bounds of the map.</summary>
  /// <param name="pixelPosition">The pixel position.</param>
  public virtual bool IsOutOfBounds(Microsoft.Xna.Framework.Rectangle pixelPosition)
  {
    if (pixelPosition.Right < 0 || pixelPosition.Bottom < 0)
      return true;
    Layer layer = this.map.Layers[0];
    return pixelPosition.X > layer.DisplayWidth || pixelPosition.Top > layer.DisplayHeight;
  }

  public virtual bool isCollidingPosition(
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
    bool flag1 = Game1.eventUp;
    if (flag1 && Game1.CurrentEvent != null && !Game1.CurrentEvent.ignoreObjectCollisions)
      flag1 = false;
    this.updateMap();
    if (this.IsOutOfBounds(position))
    {
      if (isFarmer && Game1.eventUp)
      {
        bool? isFestival = this.currentEvent?.isFestival;
        if (isFestival.HasValue && isFestival.GetValueOrDefault())
        {
          Event currentEvent = this.currentEvent;
          Microsoft.Xna.Framework.Rectangle position1 = position;
          if (!(character is Farmer who))
            who = Game1.player;
          if (currentEvent.checkForCollision(position1, who))
            return true;
        }
      }
      return false;
    }
    if (character == null && !ignoreCharacterRequirement)
      return true;
    Vector2 vector2_1 = new Vector2((float) (position.Right / 64 /*0x40*/), (float) (position.Top / 64 /*0x40*/));
    Vector2 vector2_2 = new Vector2((float) (position.Left / 64 /*0x40*/), (float) (position.Top / 64 /*0x40*/));
    Vector2 vector2_3 = new Vector2((float) (position.Right / 64 /*0x40*/), (float) (position.Bottom / 64 /*0x40*/));
    Vector2 vector2_4 = new Vector2((float) (position.Left / 64 /*0x40*/), (float) (position.Bottom / 64 /*0x40*/));
    bool bigger_than_tile = position.Width > 64 /*0x40*/;
    Vector2 bottom_mid = new Vector2((float) (position.Center.X / 64 /*0x40*/), (float) (position.Bottom / 64 /*0x40*/));
    Vector2 top_mid = new Vector2((float) (position.Center.X / 64 /*0x40*/), (float) (position.Top / 64 /*0x40*/));
    BoundingBoxGroup passableTiles = (BoundingBoxGroup) null;
    Farmer farmer = character as Farmer;
    Microsoft.Xna.Framework.Rectangle? nullable1;
    if (farmer != null)
    {
      isFarmer = true;
      nullable1 = new Microsoft.Xna.Framework.Rectangle?(farmer.GetBoundingBox());
      passableTiles = farmer.TemporaryPassableTiles;
    }
    else
    {
      farmer = (Farmer) null;
      isFarmer = false;
      nullable1 = new Microsoft.Xna.Framework.Rectangle?();
    }
    Vector2? player_top_right = new Vector2?();
    Vector2? player_top_left = new Vector2?();
    Vector2? player_bottom_right = new Vector2?();
    Vector2? player_bottom_left = new Vector2?();
    Vector2? player_bottom_mid = new Vector2?();
    Vector2? player_top_mid = new Vector2?();
    Microsoft.Xna.Framework.Rectangle boundingBox1;
    if (nullable1.HasValue)
    {
      ref Vector2? local1 = ref player_top_right;
      boundingBox1 = nullable1.Value;
      double x1 = (double) ((boundingBox1.Right - 1) / 64 /*0x40*/);
      boundingBox1 = nullable1.Value;
      double y1 = (double) (boundingBox1.Top / 64 /*0x40*/);
      Vector2 vector2_5 = new Vector2((float) x1, (float) y1);
      local1 = new Vector2?(vector2_5);
      ref Vector2? local2 = ref player_top_left;
      boundingBox1 = nullable1.Value;
      double x2 = (double) (boundingBox1.Left / 64 /*0x40*/);
      boundingBox1 = nullable1.Value;
      double y2 = (double) (boundingBox1.Top / 64 /*0x40*/);
      Vector2 vector2_6 = new Vector2((float) x2, (float) y2);
      local2 = new Vector2?(vector2_6);
      ref Vector2? local3 = ref player_bottom_right;
      boundingBox1 = nullable1.Value;
      double x3 = (double) ((boundingBox1.Right - 1) / 64 /*0x40*/);
      boundingBox1 = nullable1.Value;
      double y3 = (double) ((boundingBox1.Bottom - 1) / 64 /*0x40*/);
      Vector2 vector2_7 = new Vector2((float) x3, (float) y3);
      local3 = new Vector2?(vector2_7);
      ref Vector2? local4 = ref player_bottom_left;
      boundingBox1 = nullable1.Value;
      double x4 = (double) (boundingBox1.Left / 64 /*0x40*/);
      boundingBox1 = nullable1.Value;
      double y4 = (double) ((boundingBox1.Bottom - 1) / 64 /*0x40*/);
      Vector2 vector2_8 = new Vector2((float) x4, (float) y4);
      local4 = new Vector2?(vector2_8);
      ref Vector2? local5 = ref player_bottom_mid;
      boundingBox1 = nullable1.Value;
      double x5 = (double) (boundingBox1.Center.X / 64 /*0x40*/);
      boundingBox1 = nullable1.Value;
      double y5 = (double) ((boundingBox1.Bottom - 1) / 64 /*0x40*/);
      Vector2 vector2_9 = new Vector2((float) x5, (float) y5);
      local5 = new Vector2?(vector2_9);
      ref Vector2? local6 = ref player_top_mid;
      boundingBox1 = nullable1.Value;
      double x6 = (double) (boundingBox1.Center.X / 64 /*0x40*/);
      boundingBox1 = nullable1.Value;
      double y6 = (double) (boundingBox1.Top / 64 /*0x40*/);
      Vector2 vector2_10 = new Vector2((float) x6, (float) y6);
      local6 = new Vector2?(vector2_10);
    }
    if (farmer?.bridge != null && farmer.onBridge.Value && position.Right >= farmer.bridge.bridgeBounds.X && position.Left <= farmer.bridge.bridgeBounds.Right)
      return this._TestCornersWorld(position.Top, position.Bottom, position.Left, position.Right, (Func<int, int, bool>) ((x, y) => y > farmer.bridge.bridgeBounds.Bottom || y < farmer.bridge.bridgeBounds.Top));
    if (!glider)
    {
      if (character != null && this.animals.FieldDict.Count > 0 && !(character is FarmAnimal))
      {
        foreach (FarmAnimal farmAnimal in this.animals.Values)
        {
          Microsoft.Xna.Framework.Rectangle boundingBox2 = farmAnimal.GetBoundingBox();
          if (position.Intersects(boundingBox2))
          {
            if (nullable1.HasValue)
            {
              boundingBox1 = nullable1.Value;
              if (boundingBox1.Intersects(boundingBox2))
                continue;
            }
            if (passableTiles == null || !passableTiles.Intersects(position))
            {
              if (!skipCollisionEffects)
                farmAnimal.farmerPushing();
              return true;
            }
          }
        }
      }
      if (this.buildings.Count > 0)
      {
        foreach (Building building in this.buildings)
        {
          if (building.intersects(position) && (!nullable1.HasValue || !building.intersects(nullable1.Value)))
          {
            if (!(character is FarmAnimal) && !(character is JunimoHarvester))
            {
              if (!(character is NPC))
                return true;
              Microsoft.Xna.Framework.Rectangle rectForHumanDoor = building.getRectForHumanDoor();
              rectForHumanDoor.Height += 64 /*0x40*/;
              if (!rectForHumanDoor.Contains(position))
                return true;
            }
            else
            {
              Microsoft.Xna.Framework.Rectangle rectForAnimalDoor = building.getRectForAnimalDoor();
              rectForAnimalDoor.Height += 64 /*0x40*/;
              if (!rectForAnimalDoor.Contains(position) || character is FarmAnimal farmAnimal && !farmAnimal.CanLiveIn(building))
                return true;
            }
          }
        }
      }
      if (this.resourceClumps.Count > 0)
      {
        foreach (TerrainFeature resourceClump in this.resourceClumps)
        {
          Microsoft.Xna.Framework.Rectangle boundingBox3 = resourceClump.getBoundingBox();
          if (boundingBox3.Intersects(position) && (!nullable1.HasValue || !boundingBox3.Intersects(nullable1.Value)))
            return true;
        }
      }
      if (!flag1 && this.furniture.Count > 0)
      {
        foreach (Furniture furniture in this.furniture)
        {
          if (furniture.furniture_type.Value != 12 && furniture.IntersectsForCollision(position) && (!nullable1.HasValue || !furniture.IntersectsForCollision(nullable1.Value)))
            return true;
        }
      }
      NetCollection<LargeTerrainFeature> largeTerrainFeatures = this.largeTerrainFeatures;
      if ((largeTerrainFeatures != null ? (largeTerrainFeatures.Count > 0 ? 1 : 0) : 0) != 0)
      {
        foreach (TerrainFeature largeTerrainFeature in this.largeTerrainFeatures)
        {
          Microsoft.Xna.Framework.Rectangle boundingBox4 = largeTerrainFeature.getBoundingBox();
          if (boundingBox4.Intersects(position) && (!nullable1.HasValue || !boundingBox4.Intersects(nullable1.Value)))
            return true;
        }
      }
    }
    if (!glider)
    {
      if ((!flag1 || character != null && !isFarmer && (!pathfinding || !character.willDestroyObjectsUnderfoot)) && this._TestCornersTiles(vector2_1, vector2_2, vector2_3, vector2_4, top_mid, bottom_mid, player_top_right, player_top_left, player_bottom_right, player_bottom_left, player_top_mid, player_bottom_mid, bigger_than_tile, (Func<Vector2, bool>) (corner =>
      {
        Object o;
        if (this.objects.TryGetValue(corner, out o) && o != null && !o.isPassable())
        {
          Microsoft.Xna.Framework.Rectangle boundingBox5 = o.GetBoundingBox();
          if (boundingBox5.Intersects(position) && (character == null || character.collideWith(o)) && (!(character is FarmAnimal) || !o.isAnimalProduct()) && (passableTiles == null || !passableTiles.Intersects(boundingBox5)))
            return true;
        }
        return false;
      })))
        return true;
      this._TestCornersTiles(vector2_1, vector2_2, vector2_3, vector2_4, top_mid, bottom_mid, new Vector2?(), new Vector2?(), new Vector2?(), new Vector2?(), new Vector2?(), new Vector2?(), bigger_than_tile, (Func<Vector2, bool>) (corner =>
      {
        TerrainFeature terrainFeature;
        if (this.terrainFeatures.TryGetValue(corner, out terrainFeature) && terrainFeature != null && terrainFeature.getBoundingBox().Intersects(position) && !pathfinding && character != null && !skipCollisionEffects)
          terrainFeature.doCollisionAction(position, (int) ((double) character.speed + (double) character.addedSpeed), corner, character);
        return false;
      }));
      TerrainFeature terrainFeature1;
      if (this._TestCornersTiles(vector2_1, vector2_2, vector2_3, vector2_4, top_mid, bottom_mid, player_top_right, player_top_left, player_bottom_right, player_bottom_left, player_top_mid, player_bottom_mid, bigger_than_tile, (Func<Vector2, bool>) (corner => this.terrainFeatures.TryGetValue(corner, out terrainFeature1) && terrainFeature1 != null && terrainFeature1.getBoundingBox().Intersects(position) && !terrainFeature1.isPassable(character))))
        return true;
    }
    if (character != null && character.hasSpecialCollisionRules() && (character.isColliding(this, vector2_1) || character.isColliding(this, vector2_2) || character.isColliding(this, vector2_3) || character.isColliding(this, vector2_4)))
      return true;
    if ((isFarmer && (this.currentEvent == null || this.currentEvent.playerControlSequence) || character != null && character.collidesWithOtherCharacters.Value) && !pathfinding)
    {
      for (int index = this.characters.Count - 1; index >= 0; --index)
      {
        NPC character1 = this.characters[index];
        if (character1 != null && (character == null || !character.Equals((object) character1)))
        {
          Microsoft.Xna.Framework.Rectangle boundingBox6 = character1.GetBoundingBox();
          if (character1.layingDown)
          {
            boundingBox6.Y -= 64 /*0x40*/;
            boundingBox6.Height += 64 /*0x40*/;
          }
          if (boundingBox6.Intersects(position) && !Game1.player.temporarilyInvincible && !skipCollisionEffects)
            character1.behaviorOnFarmerPushing();
          if (isFarmer)
          {
            if (!flag1 && !character1.farmerPassesThrough && boundingBox6.Intersects(position) && !Game1.player.temporarilyInvincible && Game1.player.TemporaryPassableTiles.IsEmpty())
            {
              if (character1.IsMonster)
              {
                if (!((Monster) character1).isGlider.Value)
                {
                  boundingBox1 = Game1.player.GetBoundingBox();
                  if (boundingBox1.Intersects(character1.GetBoundingBox()))
                    continue;
                }
                else
                  continue;
              }
              if (!character1.IsInvisible)
              {
                boundingBox1 = Game1.player.GetBoundingBox();
                if (!boundingBox1.Intersects(boundingBox6))
                  return true;
              }
            }
          }
          else if (boundingBox6.Intersects(position))
            return true;
        }
      }
    }
    Layer back_layer = this.map.RequireLayer("Back");
    Layer buildings_layer = this.map.RequireLayer("Buildings");
    if (isFarmer)
    {
      Event currentEvent = this.currentEvent;
      int num;
      if (currentEvent == null)
      {
        num = 0;
      }
      else
      {
        Microsoft.Xna.Framework.Rectangle position2 = position;
        if (!(character is Farmer who))
          who = Game1.player;
        num = currentEvent.checkForCollision(position2, who) ? 1 : 0;
      }
      if (num != 0)
        return true;
    }
    else
    {
      if (!pathfinding && !(character is Monster) && damagesFarmer == 0 && !glider)
      {
        foreach (Farmer farmer1 in this.farmers)
        {
          if (position.Intersects(farmer1.GetBoundingBox()))
            return true;
        }
      }
      if ((this.isFarm.Value || MineShaft.IsGeneratedLevel(this) || this is IslandLocation) && character != null && !character.Name.Contains("NPC") && !character.EventActor && !glider)
      {
        Tile t;
        if (this._TestCornersTiles(vector2_1, vector2_2, vector2_3, vector2_4, top_mid, bottom_mid, player_top_right, player_top_left, player_bottom_right, player_bottom_left, player_top_mid, player_bottom_mid, bigger_than_tile, (Func<Vector2, bool>) (tile =>
        {
          t = back_layer.Tiles[(int) tile.X, (int) tile.Y];
          return t != null && t.Properties.ContainsKey("NPCBarrier");
        })))
          return true;
      }
      if (glider && !projectile)
        return false;
    }
    if (!isFarmer || !Game1.player.isRafting)
    {
      Tile t;
      if (this._TestCornersTiles(vector2_1, vector2_2, vector2_3, vector2_4, top_mid, bottom_mid, player_top_right, player_top_left, player_bottom_right, player_bottom_left, player_top_mid, player_bottom_mid, bigger_than_tile, (Func<Vector2, bool>) (tile =>
      {
        t = back_layer.Tiles[(int) tile.X, (int) tile.Y];
        return t != null && t.Properties.ContainsKey("TemporaryBarrier");
      })))
        return true;
    }
    if (!isFarmer || !Game1.player.isRafting)
    {
      if ((!(character is FarmAnimal farmAnimal) || !farmAnimal.IsActuallySwimming()) && this._TestCornersTiles(vector2_1, vector2_2, vector2_3, vector2_4, top_mid, bottom_mid, player_top_right, player_top_left, player_bottom_right, player_bottom_left, player_top_mid, player_bottom_mid, bigger_than_tile, (Func<Vector2, bool>) (tile =>
      {
        Tile tile1 = back_layer.Tiles[(int) tile.X, (int) tile.Y];
        if (tile1 != null)
        {
          bool flag2 = tile1.TileIndexProperties.ContainsKey("Passable");
          if (!flag2)
            flag2 = tile1.Properties.ContainsKey("Passable");
          if (flag2 && (passableTiles == null || !passableTiles.Contains((int) tile.X, (int) tile.Y)))
            return true;
        }
        return false;
      })))
        return true;
      if ((character == null ? 1 : (character.shouldCollideWithBuildingLayer(this) ? 1 : 0)) != 0)
      {
        Tile tmp;
        if (this._TestCornersTiles(vector2_1, vector2_2, vector2_3, vector2_4, top_mid, bottom_mid, player_top_right, player_top_left, player_bottom_right, player_bottom_left, player_top_mid, player_bottom_mid, bigger_than_tile, (Func<Vector2, bool>) (tile =>
        {
          tmp = buildings_layer.Tiles[(int) tile.X, (int) tile.Y];
          if (tmp != null)
          {
            if (projectile && this is VolcanoDungeon)
            {
              Tile tile2 = back_layer.Tiles[(int) tile.X, (int) tile.Y];
              if (tile2 != null && (tile2.TileIndexProperties.ContainsKey("Water") || tile2.Properties.ContainsKey("Water")))
                return false;
            }
            int num;
            if (!tmp.TileIndexProperties.ContainsKey("Shadow") && !tmp.TileIndexProperties.ContainsKey("Passable") && !tmp.Properties.ContainsKey("Passable") && (!projectile || !tmp.TileIndexProperties.ContainsKey("ProjectilePassable") && !tmp.Properties.ContainsKey("ProjectilePassable")))
            {
              if (!isFarmer)
              {
                if (!tmp.TileIndexProperties.ContainsKey("NPCPassable") && !tmp.Properties.ContainsKey("NPCPassable"))
                {
                  bool? nullable2 = character?.canPassThroughActionTiles();
                  num = !nullable2.HasValue || !nullable2.GetValueOrDefault() ? 0 : (tmp.Properties.ContainsKey("Action") ? 1 : 0);
                }
                else
                  num = 1;
              }
              else
                num = 0;
            }
            else
              num = 1;
            if (num == 0)
              return passableTiles == null || !passableTiles.Contains((int) tile.X, (int) tile.Y);
          }
          return false;
        })))
          return true;
      }
      if (!isFarmer && character?.controller != null && !skipCollisionEffects)
      {
        Point point = new Point(position.Center.X / 64 /*0x40*/, position.Bottom / 64 /*0x40*/);
        Tile tile3 = buildings_layer.Tiles[point.X, point.Y];
        if (tile3 != null && tile3.Properties.ContainsKey("Action"))
        {
          this.openDoor(new Location(point.X, point.Y), Game1.currentLocation.Equals(this));
        }
        else
        {
          point = new Point(position.Center.X / 64 /*0x40*/, position.Top / 64 /*0x40*/);
          Tile tile4 = buildings_layer.Tiles[point.X, point.Y];
          if (tile4 != null && tile4.Properties.ContainsKey("Action"))
            this.openDoor(new Location(point.X, point.Y), Game1.currentLocation.Equals(this));
        }
      }
      return false;
    }
    Tile t1;
    return this._TestCornersTiles(vector2_1, vector2_2, vector2_3, vector2_4, top_mid, bottom_mid, player_top_right, player_top_left, player_bottom_right, player_bottom_left, player_top_mid, player_bottom_mid, bigger_than_tile, (Func<Vector2, bool>) (tile =>
    {
      t1 = back_layer.Tiles[(int) tile.X, (int) tile.Y];
      bool? nullable3 = t1?.TileIndexProperties.ContainsKey("Water");
      if ((!nullable3.HasValue ? 0 : (nullable3.GetValueOrDefault() ? 1 : 0)) != 0)
        return false;
      int x = (int) tile.X;
      int y = (int) tile.Y;
      if (this.IsTileBlockedBy(new Vector2((float) x, (float) y)))
      {
        Game1.player.isRafting = false;
        Game1.player.Position = new Vector2((float) (x * 64 /*0x40*/), (float) (y * 64 /*0x40*/ - 32 /*0x20*/));
        Game1.player.setTrajectory(0, 0);
      }
      return true;
    }));
  }

  public bool isTilePassable(Vector2 tileLocation)
  {
    Tile tile1 = this.map.RequireLayer("Back").Tiles[(int) tileLocation.X, (int) tileLocation.Y];
    if (tile1 != null && tile1.TileIndexProperties.ContainsKey("Passable"))
      return false;
    Tile tile2 = this.map.RequireLayer("Buildings").Tiles[(int) tileLocation.X, (int) tileLocation.Y];
    return tile2 == null || tile2.TileIndexProperties.ContainsKey("Shadow") || tile2.TileIndexProperties.ContainsKey("Passable");
  }

  public bool isTilePassable(Location tileLocation, xTile.Dimensions.Rectangle viewport)
  {
    return this.isTilePassable(new Vector2((float) tileLocation.X, (float) tileLocation.Y));
  }

  public bool isPointPassable(Location location, xTile.Dimensions.Rectangle viewport)
  {
    return this.isTilePassable(new Location(location.X / 64 /*0x40*/, location.Y / 64 /*0x40*/), viewport);
  }

  public bool isTilePassable(Microsoft.Xna.Framework.Rectangle nextPosition, xTile.Dimensions.Rectangle viewport)
  {
    return this.isPointPassable(new Location(nextPosition.Left, nextPosition.Top), viewport) && this.isPointPassable(new Location(nextPosition.Right, nextPosition.Bottom), viewport) && this.isPointPassable(new Location(nextPosition.Left, nextPosition.Bottom), viewport) && this.isPointPassable(new Location(nextPosition.Right, nextPosition.Top), viewport);
  }

  public bool isTileOnMap(Vector2 position)
  {
    return (double) position.X >= 0.0 && (double) position.X < (double) this.map.Layers[0].LayerWidth && (double) position.Y >= 0.0 && (double) position.Y < (double) this.map.Layers[0].LayerHeight;
  }

  public bool isTileOnMap(Point tile) => this.isTileOnMap(tile.X, tile.Y);

  public bool isTileOnMap(int x, int y)
  {
    return x >= 0 && x < this.map.Layers[0].LayerWidth && y >= 0 && y < this.map.Layers[0].LayerHeight;
  }

  public int numberOfObjectsWithName(string name)
  {
    int num = 0;
    foreach (Item obj in this.objects.Values)
    {
      if (obj.Name.Equals(name))
        ++num;
    }
    return num;
  }

  public virtual Point getWarpPointTo(string location, Character character = null)
  {
    foreach (Building building in this.buildings)
    {
      if (building.HasIndoorsName(location))
        return building.getPointForHumanDoor();
    }
    foreach (Warp warp in (NetList<Warp, NetRef<Warp>>) this.warps)
    {
      if (warp.TargetName.Equals(location))
        return new Point(warp.X, warp.Y);
      if (warp.TargetName.Equals("BoatTunnel") && location == "IslandSouth")
        return new Point(warp.X, warp.Y);
    }
    foreach (KeyValuePair<Point, string> pair in this.doors.Pairs)
    {
      if (pair.Value.Equals("BoatTunnel") && location == "IslandSouth" || pair.Value.Equals(location))
        return pair.Key;
    }
    return Point.Zero;
  }

  public Point getWarpPointTarget(Point warpPointLocation, Character character = null)
  {
    foreach (Warp warp in (NetList<Warp, NetRef<Warp>>) this.warps)
    {
      if (warp.X == warpPointLocation.X && warp.Y == warpPointLocation.Y)
        return new Point(warp.TargetX, warp.TargetY);
    }
    foreach (KeyValuePair<Point, string> pair in this.doors.Pairs)
    {
      if (pair.Key.Equals(warpPointLocation))
      {
        string[] propertySplitBySpaces = this.GetTilePropertySplitBySpaces("Action", "Buildings", warpPointLocation.X, warpPointLocation.Y);
        string str1 = ArgUtility.Get(propertySplitBySpaces, 0, "");
        if (str1 != null)
        {
          switch (str1.Length)
          {
            case 4:
              if (str1 == "Warp")
                break;
              goto label_29;
            case 14:
              switch (str1[4])
              {
                case 'B':
                  if (str1 == "WarpBoatTunnel")
                    return new Point(17, 43);
                  goto label_29;
                case 'M':
                  if (str1 == "WarpMensLocker")
                    break;
                  goto label_29;
                case 'e':
                  if (str1 == "LockedDoorWarp")
                    break;
                  goto label_29;
                default:
                  goto label_29;
              }
              break;
            case 16 /*0x10*/:
              if (str1 == "WarpWomensLocker")
                break;
              goto label_29;
            case 17:
              if (str1 == "Warp_Sunroom_Door")
                return new Point(5, 13);
              goto label_29;
            case 19:
              if (str1 == "WarpCommunityCenter")
                return new Point(32 /*0x20*/, 23);
              goto label_29;
            default:
              goto label_29;
          }
        }
        else
          goto label_29;
label_22:
        Point point;
        string error;
        string str2;
        if (!ArgUtility.TryGetPoint(propertySplitBySpaces, 1, out point, out error, "Point tile") || !ArgUtility.TryGet(propertySplitBySpaces, 3, out str2, out error, name: "string locationName"))
        {
          this.LogTileActionError(propertySplitBySpaces, warpPointLocation.X, warpPointLocation.Y, error);
          continue;
        }
        switch (str2)
        {
          case "BoatTunnel":
            return new Point(17, 43);
          case "Trailer":
            if (Game1.MasterPlayer.mailReceived.Contains("pamHouseUpgrade"))
              return new Point(13, 24);
            break;
        }
        return new Point(point.X, point.Y);
label_29:
        if (str1.Contains("Warp"))
        {
          Game1.log.Warn($"Door in {this.NameOrUniqueName} ({pair.Key}) has unknown warp property '{string.Join(" ", propertySplitBySpaces)}', parsing with legacy logic.");
          goto label_22;
        }
      }
    }
    return Point.Zero;
  }

  public virtual bool HasLocationOverrideDialogue(NPC character) => false;

  public virtual string GetLocationOverrideDialogue(NPC character)
  {
    return !this.HasLocationOverrideDialogue(character) ? (string) null : "";
  }

  public NPC doesPositionCollideWithCharacter(Microsoft.Xna.Framework.Rectangle r, bool ignoreMonsters = false)
  {
    foreach (NPC character in this.characters)
    {
      if (character.GetBoundingBox().Intersects(r) && (!character.IsMonster || !ignoreMonsters))
        return character;
    }
    return (NPC) null;
  }

  public void switchOutNightTiles()
  {
    string[] propertySplitBySpaces = this.GetMapPropertySplitBySpaces("NightTiles");
    for (int index = 0; index < propertySplitBySpaces.Length; index += 4)
    {
      string layerId;
      string error;
      Point point;
      int num;
      if (!ArgUtility.TryGet(propertySplitBySpaces, index, out layerId, out error, name: "string layerId") || !ArgUtility.TryGetPoint(propertySplitBySpaces, index + 1, out point, out error, "Point position") || !ArgUtility.TryGetInt(propertySplitBySpaces, index + 3, out num, out error, "int tileIndex"))
        this.LogMapPropertyError("NightTiles", propertySplitBySpaces, error);
      else if (num != 726 && num != 720 || !Game1.MasterPlayer.mailReceived.Contains("pamHouseUpgrade"))
      {
        Tile tile = this.map.RequireLayer(layerId).Tiles[point.X, point.Y];
        if (tile == null)
          this.LogMapPropertyError("NightTiles", propertySplitBySpaces, $"there's no tile at position ({point})");
        else
          tile.TileIndex = num;
      }
    }
    switch (this)
    {
      case MineShaft _:
        break;
      case Woods _:
        break;
      default:
        this.lightGlows.Clear();
        break;
    }
  }

  public string GetMorningSong()
  {
    LocationWeather weather = this.GetWeather();
    if (weather.IsRaining)
      return "rain";
    List<string> stringList = new List<string>();
    List<LocationMusicData> locationMusicDataList = this.GetLocationContext().Music;
    // ISSUE: explicit non-virtual call
    if (locationMusicDataList == null || __nonvirtual (locationMusicDataList.Count) <= 0)
      locationMusicDataList = StardewValley.LocationContexts.Default.Music ?? new List<LocationMusicData>();
    foreach (LocationMusicData locationMusicData in locationMusicDataList)
    {
      if (GameStateQuery.CheckConditions(locationMusicData.Condition, this))
        stringList.Add(locationMusicData.Track);
    }
    if (stringList.Count == 0)
      return "none";
    int num = weather.monthlyNonRainyDayCount.Value - 1;
    if (num < 0)
      num = 0;
    return stringList[num % stringList.Count];
  }

  /// <summary>Update the music when the player changes location.</summary>
  /// <param name="oldLocation">The location the player just left.</param>
  /// <param name="newLocation">The location the player just arrived in.</param>
  /// <remarks>For changes to music while a location is active, see <see cref="M:StardewValley.GameLocation.checkForMusic(Microsoft.Xna.Framework.GameTime)" />.</remarks>
  public static void HandleMusicChange(GameLocation oldLocation, GameLocation newLocation)
  {
    string musicTrackName = Game1.getMusicTrackName();
    if (!newLocation.IsOutdoors && Game1.IsPlayingOutdoorsAmbience)
      Game1.changeMusicTrack("none", true);
    if (musicTrackName == "rain")
    {
      if (!Game1.IsRainingHere(newLocation))
        Game1.stopMusicTrack(MusicContext.Default);
      else if (newLocation is MineShaft && !(oldLocation is MineShaft))
        Game1.stopMusicTrack(MusicContext.Default);
    }
    if (Game1.getMusicTrackName() == "sam_acoustic1")
      Game1.stopMusicTrack(MusicContext.Default);
    if (newLocation is MineShaft)
      return;
    string locationContextId1 = oldLocation?.GetLocationContextId();
    LocationContextData locationContext1 = oldLocation?.GetLocationContext();
    LocationData data = newLocation?.GetData();
    string locationContextId2 = newLocation?.GetLocationContextId();
    LocationContextData locationContext2 = newLocation?.GetLocationContext();
    string newTrackName = newLocation?.GetLocationSpecificMusic();
    MusicContext music_context = data != null ? data.MusicContext : MusicContext.Default;
    bool flag = false;
    if (newLocation != null)
    {
      if (newTrackName != null)
      {
        flag = data != null && data.MusicIsTownTheme;
        newLocation.isMusicTownMusic = new bool?(flag);
      }
      else
        newLocation.isMusicTownMusic = new bool?(false);
    }
    if (newTrackName == null || music_context == MusicContext.Default)
      Game1.stopMusicTrack(MusicContext.SubLocation);
    if (newTrackName == null && Game1.IsRainingHere(newLocation))
      newTrackName = "rain";
    else if (Game1.IsPlayingMorningSong && oldLocation != null && oldLocation.GetMorningSong() != newLocation.GetMorningSong() && Game1.shouldPlayMorningSong(true))
    {
      Game1.playMorningSong(true);
      return;
    }
    if (newTrackName == null && !Game1.IsPlayingBackgroundMusic && newLocation.isOutdoors.Value && Game1.shouldPlayMorningSong())
    {
      Game1.playMorningSong();
    }
    else
    {
      if (locationContextId1 != locationContextId2)
        GameLocation.PlayedNewLocationContextMusic = false;
      if (!locationContext2.DefaultMusicDelayOneScreen)
        GameLocation.PlayedNewLocationContextMusic = false;
      if (Game1.IsPlayingTownMusic && newLocation.IsOutdoors && (!flag || newTrackName != musicTrackName))
      {
        Game1.IsPlayingTownMusic = false;
        Game1.changeMusicTrack("none", true);
      }
      if (flag)
      {
        if (newTrackName == musicTrackName)
          return;
        newTrackName = (string) null;
      }
      if (newTrackName == null)
      {
        if (locationContext1 != null && locationContext2.DefaultMusic != locationContext1.DefaultMusic)
          Game1.stopMusicTrack(MusicContext.Default);
        if (!GameLocation.PlayedNewLocationContextMusic)
        {
          if (locationContext2.DefaultMusic != null)
          {
            if (Game1.isDarkOut(newLocation) || Game1.isStartingToGetDarkOut(newLocation) || Game1.IsRainingHere(newLocation))
              GameLocation.PlayedNewLocationContextMusic = true;
            else if (locationContext2.DefaultMusicCondition == null || GameStateQuery.CheckConditions(locationContext2.DefaultMusicCondition, newLocation))
            {
              Game1.changeMusicTrack(locationContext2.DefaultMusic, true);
              Game1.IsPlayingBackgroundMusic = true;
              GameLocation.PlayedNewLocationContextMusic = true;
            }
          }
          else
          {
            GameLocation.PlayedNewLocationContextMusic = true;
            if (!flag && Game1.shouldPlayMorningSong(true))
            {
              Game1.playMorningSong();
              return;
            }
          }
        }
      }
      if (!(musicTrackName != newTrackName))
        return;
      if (newTrackName == null)
      {
        if (Game1.IsPlayingBackgroundMusic || Game1.IsPlayingOutdoorsAmbience)
          return;
        Game1.stopMusicTrack(MusicContext.Default);
      }
      else
        Game1.changeMusicTrack(newTrackName, true, music_context);
    }
  }

  /// <summary>Check for music changes while the level is active.</summary>
  /// <param name="time">The current game time.</param>
  /// <remarks>This should only be used for music changes while a location is active. Other music changes should be in <see cref="M:StardewValley.GameLocation.HandleMusicChange(StardewValley.GameLocation,StardewValley.GameLocation)" />.</remarks>
  public virtual void checkForMusic(GameTime time)
  {
    if (Game1.getMusicTrackName() == "sam_acoustic1" && Game1.isMusicContextActiveButNotPlaying())
      Game1.changeMusicTrack("none", true);
    if (this.isMusicTownMusic.HasValue && this.isMusicTownMusic.Value && !Game1.eventUp && Game1.timeOfDay < 1800 && (Game1.isMusicContextActiveButNotPlaying() || Game1.IsPlayingOutdoorsAmbience))
    {
      string locationSpecificMusic = this.GetLocationSpecificMusic();
      if (locationSpecificMusic != null)
      {
        LocationData data = this.GetData();
        MusicContext music_context = data != null ? data.MusicContext : MusicContext.Default;
        Game1.changeMusicTrack(locationSpecificMusic, music_context: music_context);
        Game1.IsPlayingBackgroundMusic = true;
        Game1.IsPlayingTownMusic = true;
      }
    }
    if (this.IsOutdoors && !this.IsRainingHere() && !Game1.eventUp)
    {
      bool flag = Game1.isDarkOut(this);
      if (flag && Game1.IsPlayingOutdoorsAmbience && !Game1.IsPlayingNightAmbience)
        Game1.changeMusicTrack("none", true);
      if (!Game1.isMusicContextActiveButNotPlaying())
        return;
      if (!flag)
      {
        LocationContextData locationContext = this.GetLocationContext();
        if (locationContext.DayAmbience != null)
        {
          Game1.changeMusicTrack(locationContext.DayAmbience, true);
        }
        else
        {
          switch (this.GetSeason())
          {
            case Season.Spring:
              Game1.changeMusicTrack("spring_day_ambient", true);
              break;
            case Season.Summer:
              Game1.changeMusicTrack("summer_day_ambient", true);
              break;
            case Season.Fall:
              Game1.changeMusicTrack("fall_day_ambient", true);
              break;
            case Season.Winter:
              Game1.changeMusicTrack("winter_day_ambient", true);
              break;
          }
        }
        Game1.IsPlayingOutdoorsAmbience = true;
      }
      else
      {
        if (Game1.timeOfDay >= 2500)
          return;
        LocationContextData locationContext = this.GetLocationContext();
        if (locationContext.NightAmbience != null)
        {
          Game1.changeMusicTrack(locationContext.NightAmbience, true);
        }
        else
        {
          switch (this.GetSeason())
          {
            case Season.Spring:
              Game1.changeMusicTrack("spring_night_ambient", true);
              break;
            case Season.Summer:
              Game1.changeMusicTrack("spring_night_ambient", true);
              break;
            case Season.Fall:
              Game1.changeMusicTrack("spring_night_ambient", true);
              break;
            case Season.Winter:
              Game1.changeMusicTrack("none", true);
              break;
          }
        }
        Game1.IsPlayingNightAmbience = true;
        Game1.IsPlayingOutdoorsAmbience = true;
      }
    }
    else
    {
      if (!this.IsRainingHere() || Game1.showingEndOfNightStuff || !Game1.isMusicContextActiveButNotPlaying())
        return;
      Game1.changeMusicTrack("rain", true);
    }
  }

  public virtual string GetLocationSpecificMusic()
  {
    LocationData data = this.GetData();
    if (data != null)
    {
      if (data.MusicIgnoredInRain && this.IsRainingHere())
        return (string) null;
      Season season = this.GetSeason();
      bool flag = false;
      switch (season)
      {
        case Season.Spring:
          flag = data.MusicIgnoredInSpring;
          break;
        case Season.Summer:
          flag = data.MusicIgnoredInSummer;
          break;
        case Season.Fall:
          flag = data.MusicIgnoredInFall;
          break;
        case Season.Winter:
          flag = data.MusicIgnoredInWinter;
          break;
      }
      if (flag)
        return (string) null;
      if (season == Season.Fall && this.IsDebrisWeatherHere() && data.MusicIgnoredInFallDebris)
        return (string) null;
      List<LocationMusicData> music = data.Music;
      // ISSUE: explicit non-virtual call
      if ((music != null ? (__nonvirtual (music.Count) > 0 ? 1 : 0) : 0) != 0)
      {
        foreach (LocationMusicData locationMusicData in data.Music)
        {
          if (GameStateQuery.CheckConditions(locationMusicData.Condition, this))
            return locationMusicData.Track;
        }
      }
      if (data.MusicDefault != null)
        return data.MusicDefault;
    }
    string[] propertySplitBySpaces = this.GetMapPropertySplitBySpaces("Music");
    if (propertySplitBySpaces.Length == 0)
      return (string) null;
    if (propertySplitBySpaces.Length <= 1)
      return propertySplitBySpaces[0];
    int num1;
    string error;
    int num2;
    string str;
    if (!ArgUtility.TryGetInt(propertySplitBySpaces, 0, out num1, out error, "int startTime") || !ArgUtility.TryGetInt(propertySplitBySpaces, 1, out num2, out error, "int endTime") || !ArgUtility.TryGet(propertySplitBySpaces, 2, out str, out error, name: "string musicId"))
    {
      this.LogMapPropertyError("Music", propertySplitBySpaces, error);
      return (string) null;
    }
    return Game1.timeOfDay < num1 || num2 != 0 && Game1.timeOfDay >= num2 ? (string) null : str;
  }

  public NPC isCollidingWithCharacter(Microsoft.Xna.Framework.Rectangle box)
  {
    if (Game1.isFestival() && this.currentEvent != null)
    {
      foreach (NPC actor in this.currentEvent.actors)
      {
        if (actor.GetBoundingBox().Intersects(box))
          return actor;
      }
    }
    foreach (NPC character in this.characters)
    {
      if (character.GetBoundingBox().Intersects(box))
        return character;
    }
    return (NPC) null;
  }

  public virtual void drawAboveAlwaysFrontLayer(SpriteBatch b)
  {
    if (this.critters != null && Game1.farmEvent == null)
    {
      for (int index = 0; index < this.critters.Count; ++index)
        this.critters[index].drawAboveFrontLayer(b);
    }
    foreach (Character character in this.characters)
      character.drawAboveAlwaysFrontLayer(b);
    if (!(this is MineShaft))
    {
      foreach (NPC character in this.characters)
      {
        if (character is Monster monster)
          monster.drawAboveAllLayers(b);
      }
    }
    if (this.TemporarySprites.Count > 0)
    {
      foreach (TemporaryAnimatedSprite temporarySprite in this.TemporarySprites)
      {
        if (temporarySprite.drawAboveAlwaysFront)
          temporarySprite.draw(b);
      }
    }
    if (this.projectiles.Count <= 0)
      return;
    foreach (Projectile projectile in this.projectiles)
      projectile.draw(b);
  }

  /// <summary>Move objects and furniture covering a tile to another position.</summary>
  /// <param name="oldX">The X tile coordinate from which to move items.</param>
  /// <param name="oldY">The Y tile coordinate from which to move items.</param>
  /// <param name="newX">The X tile coordinate at which to place moved items.</param>
  /// <param name="newY">The Y tile coordinate at which to place moved items.</param>
  /// <param name="unlessItemId">If set, an item won't be moved if its item ID matches this one.</param>
  /// <returns>Returns whether any items were moved.</returns>
  /// <remarks>Multi-tile furniture which cover the old tile will be placed at an equivalent position relative to the new tile.</remarks>
  public bool moveContents(int oldX, int oldY, int newX, int newY, string unlessItemId)
  {
    Vector2 key1 = new Vector2((float) oldX, (float) oldY);
    Vector2 key2 = new Vector2((float) newX, (float) newY);
    Microsoft.Xna.Framework.Rectangle rectangle = new Microsoft.Xna.Framework.Rectangle(oldX * 64 /*0x40*/, oldY * 64 /*0x40*/, 64 /*0x40*/, 64 /*0x40*/);
    Microsoft.Xna.Framework.Rectangle newPixelArea = new Microsoft.Xna.Framework.Rectangle(newX * 64 /*0x40*/, newY * 64 /*0x40*/, 64 /*0x40*/, 64 /*0x40*/);
    bool flag = false;
    Object @object;
    if (this.objects.TryGetValue(key1, out @object) && !this.objects.ContainsKey(key2) && (unlessItemId == null || !ItemRegistry.HasItemId((Item) @object, unlessItemId)))
    {
      this.objects.Remove(key1);
      this.objects.Add(key2, @object);
      flag = true;
    }
    for (int index = this.furniture.Count - 1; index >= 0; --index)
    {
      Furniture furniture = this.furniture[index];
      if (furniture.boundingBox.Value.Intersects(rectangle) && (unlessItemId == null || !ItemRegistry.HasItemId((Item) furniture, unlessItemId)) && !this.furniture.Any<Furniture>((Func<Furniture, bool>) (p => p.boundingBox.Value.Intersects(newPixelArea))))
      {
        Vector2 vector2 = furniture.TileLocation - key1;
        this.furniture.RemoveAt(index);
        furniture.TileLocation = key2 + vector2;
        this.furniture.Add(furniture);
        flag = true;
      }
    }
    return flag;
  }

  private void getGalaxySword()
  {
    Item obj = ItemRegistry.Create("(W)4");
    Game1.flashAlpha = 1f;
    Game1.player.holdUpItemThenMessage(obj);
    Game1.player.reduceActiveItemByOne();
    if (!Game1.player.addItemToInventoryBool(obj))
      Game1.createItemDebris(obj, Game1.player.getStandingPosition(), 1);
    Game1.player.mailReceived.Add("galaxySword");
    Game1.player.jitterStrength = 0.0f;
    Game1.screenGlowHold = false;
    Game1.multiplayer.globalChatInfoMessage("GalaxySword", Game1.player.Name);
  }

  public static void RegisterTouchAction(
    string key,
    Action<GameLocation, string[], Farmer, Vector2> action)
  {
    if (action == null)
      GameLocation.registeredTouchActions.Remove(key);
    else
      GameLocation.registeredTouchActions[key] = action;
  }

  public static void RegisterTileAction(
    string key,
    Func<GameLocation, string[], Farmer, Point, bool> action)
  {
    if (action == null)
      GameLocation.registeredTileActions.Remove(key);
    else
      GameLocation.registeredTileActions[key] = action;
  }

  /// <summary>Whether to ignore any touch actions the player walks over.</summary>
  public virtual bool IgnoreTouchActions() => Game1.eventUp;

  /// <summary>Handle a <c>TouchAction</c> property from a <c>Back</c> map tile in the location when a player steps on the tile.</summary>
  /// <param name="fullActionString">The full action string to parse, including the <c>TouchAction</c> prefix.</param>
  /// <param name="playerStandingPosition">The tile coordinate containing the tile which was stepped on.</param>
  public virtual void performTouchAction(string fullActionString, Vector2 playerStandingPosition)
  {
    this.performTouchAction(ArgUtility.SplitBySpace(fullActionString), playerStandingPosition);
  }

  /// <summary>Handle a <c>TouchAction</c> property from a <c>Back</c> map tile in the location when a player steps on the tile.</summary>
  /// <param name="action">The action arguments to parse, including the <c>TouchAction</c> prefix.</param>
  /// <param name="playerStandingPosition">The tile coordinate containing the tile which was stepped on.</param>
  public virtual void performTouchAction(string[] action, Vector2 playerStandingPosition)
  {
    if (this.IgnoreTouchActions())
      return;
    try
    {
      string key;
      string error;
      if (!ArgUtility.TryGet(action, 0, out key, out error, name: "string actionType"))
      {
        LogError(error);
      }
      else
      {
        Action<GameLocation, string[], Farmer, Vector2> action1;
        if (GameLocation.registeredTouchActions.TryGetValue(key, out action1))
        {
          action1(this, action, Game1.player, playerStandingPosition);
        }
        else
        {
          if (key == null)
            return;
          switch (key.Length)
          {
            case 4:
              switch (key[0])
              {
                case 'D':
                  if (!(key == "Door"))
                    return;
                  for (int index = 1; index < action.Length && (!(action[index] == "Sebastian") || !this.IsGreenRainingHere() || Game1.year != 1); ++index)
                  {
                    if (Game1.player.getFriendshipHeartLevelForNPC(action[index]) < 2 && index == action.Length - 1)
                    {
                      Farmer player = Game1.player;
                      player.Position = player.Position - Game1.player.getMostRecentMovementVector() * 2f;
                      Game1.player.yVelocity = 0.0f;
                      Game1.player.Halt();
                      Game1.player.TemporaryPassableTiles.Clear();
                      if (Game1.player.Tile == this.lastTouchActionLocation)
                      {
                        if ((double) Game1.player.Position.Y > (double) this.lastTouchActionLocation.Y * 64.0 + 32.0)
                          Game1.player.position.Y += 4f;
                        else
                          Game1.player.position.Y -= 4f;
                        this.lastTouchActionLocation = Vector2.Zero;
                      }
                      if (Game1.player.mailReceived.Contains("doorUnlock" + action[1]) && (action.Length == 2 || Game1.player.mailReceived.Contains("doorUnlock" + action[2])) || action.Length == 3 && Game1.player.mailReceived.Contains("doorUnlock" + action[2]))
                        break;
                      this.ShowLockedDoorMessage(action);
                      break;
                    }
                    if (index != action.Length - 1 && Game1.player.getFriendshipHeartLevelForNPC(action[index]) >= 2)
                    {
                      Game1.player.mailReceived.Add("doorUnlock" + action[index]);
                      break;
                    }
                    if (index == action.Length - 1 && Game1.player.getFriendshipHeartLevelForNPC(action[index]) >= 2)
                    {
                      Game1.player.mailReceived.Add("doorUnlock" + action[index]);
                      break;
                    }
                  }
                  return;
                case 'W':
                  if (!(key == "Warp"))
                    return;
                  string locationName;
                  Point point;
                  string str1;
                  if (!ArgUtility.TryGet(action, 1, out locationName, out error, name: "string locationToWarp") || !ArgUtility.TryGetPoint(action, 2, out point, out error, "Point tile") || !ArgUtility.TryGetOptional(action, 4, out str1, out error, name: "string mailRequired"))
                  {
                    LogError(error);
                    return;
                  }
                  if (str1 != null && !Game1.player.mailReceived.Contains(str1))
                    return;
                  Game1.warpFarmer(locationName, point.X, point.Y, false);
                  return;
                default:
                  return;
              }
            case 5:
              switch (key[0])
              {
                case 'E':
                  if (!(key == "Emote"))
                    return;
                  string name1;
                  int whichEmote;
                  if (!ArgUtility.TryGet(action, 1, out name1, out error, name: "string npcName") || !ArgUtility.TryGetInt(action, 2, out whichEmote, out error, "int emote"))
                  {
                    LogError(error);
                    return;
                  }
                  this.getCharacterFromName(name1)?.doEmote(whichEmote);
                  return;
                case 'S':
                  if (!(key == "Sleep") || Game1.newDay || !Game1.shouldTimePass() || !Game1.player.hasMoved || Game1.player.passedOut)
                    return;
                  this.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:FarmHouse_Bed_GoToSleep"), this.createYesNoResponses(), "Sleep", (Object) null);
                  return;
                default:
                  return;
              }
            case 8:
              if (!(key == "asdlfkjg"))
                break;
              this.removeTileProperty(13, 29, "Back", "TouchAction");
              this.removeTileProperty(14, 29, "Back", "TouchAction");
              this.removeTileProperty(15, 29, "Back", "TouchAction");
              if (Game1.timeOfDay < 1920 || Game1.timeOfDay >= 2020 || this.farmers.Count != 1 || Game1.stats.DaysPlayed <= 3U || Game1.isRaining || Game1.random.NextDouble() >= 0.025)
                break;
              Game1.player.mailReceived.Add("asdlkjfg1");
              this.playSound("shadowDie");
              DelayedAction.playSoundAfterDelay("grassyStep", 500, this);
              DelayedAction.playSoundAfterDelay("grassyStep", 1000, this);
              DelayedAction.playSoundAfterDelay("grassyStep", 1500, this);
              this.temporarySprites.Add(new TemporaryAnimatedSprite("Characters\\asldkfjsquaskutanfsldk", new Microsoft.Xna.Framework.Rectangle(0, 0, 32 /*0x20*/, 48 /*0x30*/), new Vector2(390f, 1980f), true, 0.0f, Color.White)
              {
                animationLength = 8,
                totalNumberOfLoops = 99,
                interval = 100f,
                motion = new Vector2(-5f, -1f),
                scale = 5.5f
              });
              break;
            case 9:
              switch (key[0])
              {
                case 'M':
                  if (!(key == "MagicWarp"))
                    return;
                  string str2;
                  string locationToWarp;
                  Point tile;
                  if (!ArgUtility.TryGet(action, 1, out locationToWarp, out error, name: "string locationToWarp") || !ArgUtility.TryGetPoint(action, 2, out tile, out error, "Point tile") || !ArgUtility.TryGetOptional(action, 4, out str2, out error, name: "string mailRequired"))
                  {
                    LogError(error);
                    return;
                  }
                  if (str2 != null && !Game1.player.mailReceived.Contains(str2))
                    return;
                  for (int index = 0; index < 12; ++index)
                    Game1.multiplayer.broadcastSprites(this, new TemporaryAnimatedSprite(354, (float) Game1.random.Next(25, 75), 6, 1, new Vector2((float) Game1.random.Next((int) Game1.player.position.X - 256 /*0x0100*/, (int) Game1.player.position.X + 192 /*0xC0*/), (float) Game1.random.Next((int) Game1.player.position.Y - 256 /*0x0100*/, (int) Game1.player.position.Y + 192 /*0xC0*/)), false, Game1.random.NextBool()));
                  this.playSound("wand");
                  Game1.freezeControls = true;
                  Game1.displayFarmer = false;
                  Game1.player.CanMove = false;
                  Game1.flashAlpha = 1f;
                  DelayedAction.fadeAfterDelay((Game1.afterFadeFunction) (() =>
                  {
                    Game1.warpFarmer(locationToWarp, tile.X, tile.Y, false);
                    Game1.fadeToBlackAlpha = 0.99f;
                    Game1.screenGlow = false;
                    Game1.displayFarmer = true;
                    Game1.player.CanMove = true;
                    Game1.freezeControls = false;
                  }), 1000);
                  Microsoft.Xna.Framework.Rectangle boundingBox = Game1.player.GetBoundingBox();
                  new Microsoft.Xna.Framework.Rectangle(boundingBox.X, boundingBox.Y, 64 /*0x40*/, 64 /*0x40*/).Inflate(192 /*0xC0*/, 192 /*0xC0*/);
                  int num = 0;
                  Point tilePoint = Game1.player.TilePoint;
                  for (int x = tilePoint.X + 8; x >= tilePoint.X - 8; --x)
                  {
                    Game1.multiplayer.broadcastSprites(this, new TemporaryAnimatedSprite(6, new Vector2((float) x, (float) tilePoint.Y) * 64f, Color.White, animationInterval: 50f)
                    {
                      layerDepth = 1f,
                      delayBeforeAnimationStart = num * 25,
                      motion = new Vector2(-0.25f, 0.0f)
                    });
                    ++num;
                  }
                  return;
                case 'P':
                  if (!(key == "PlayEvent"))
                    return;
                  string eventId;
                  bool checkPreconditions;
                  bool checkSeen;
                  string fullActionString;
                  if (!ArgUtility.TryGet(action, 1, out eventId, out error, name: "string eventId") || !ArgUtility.TryGetOptionalBool(action, 2, out checkPreconditions, out error, true, "bool checkPreconditions") || !ArgUtility.TryGetOptionalBool(action, 3, out checkSeen, out error, true, "bool checkSeen") || !ArgUtility.TryGetOptionalRemainder(action, 4, out fullActionString))
                  {
                    LogError(error);
                    return;
                  }
                  if (Game1.PlayEvent(eventId, checkPreconditions, checkSeen) || fullActionString == null)
                    return;
                  this.performAction(fullActionString, Game1.player, new Location((int) playerStandingPosition.X, (int) playerStandingPosition.Y));
                  return;
                default:
                  return;
              }
            case 10:
              if (!(key == "MensLocker") || Game1.player.IsMale)
                break;
              Game1.player.position.Y += (float) (((double) Game1.player.Speed + (double) Game1.player.addedSpeed) * 2.0);
              Game1.player.Halt();
              Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:MensLocker_WrongGender"));
              break;
            case 11:
              if (!(key == "MagicalSeal") || Game1.player.mailReceived.Contains("krobusUnseal"))
                break;
              Farmer player1 = Game1.player;
              player1.Position = player1.Position - Game1.player.getMostRecentMovementVector() * 2f;
              Game1.player.yVelocity = 0.0f;
              Game1.player.Halt();
              Game1.player.TemporaryPassableTiles.Clear();
              if (Game1.player.Tile == this.lastTouchActionLocation)
              {
                if ((double) Game1.player.position.Y > (double) this.lastTouchActionLocation.Y * 64.0 + 32.0)
                  Game1.player.position.Y += 4f;
                else
                  Game1.player.position.Y -= 4f;
                this.lastTouchActionLocation = Vector2.Zero;
              }
              Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Sewer_MagicSeal"));
              for (int index = 0; index < 40; ++index)
              {
                Game1.multiplayer.broadcastSprites(this, new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(666, 1851, 8, 8), 25f, 4, 2, new Vector2(3f, 19f) * 64f + new Vector2((float) (index % 4 * 16 /*0x10*/ - 8), (float) (-(index / 4) * 64 /*0x40*/ / 4)), false, false)
                {
                  layerDepth = (float) (0.11519999802112579 + (double) index / 10000.0),
                  color = new Color(100 + index * 4, index * 5, 120 + index * 4),
                  pingPong = true,
                  delayBeforeAnimationStart = index * 10,
                  scale = 4f,
                  alphaFade = 0.01f
                });
                Game1.multiplayer.broadcastSprites(this, new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(666, 1851, 8, 8), 25f, 4, 2, new Vector2(3f, 17f) * 64f + new Vector2((float) (index % 4 * 16 /*0x10*/ - 8), (float) (index / 4 * 64 /*0x40*/ / 4)), false, false)
                {
                  layerDepth = (float) (0.11519999802112579 + (double) index / 10000.0),
                  color = new Color(232 - index * 4, 192 /*0xC0*/ - index * 6, (int) byte.MaxValue - index * 4),
                  pingPong = true,
                  delayBeforeAnimationStart = 320 + index * 10,
                  scale = 4f,
                  alphaFade = 0.01f
                });
                Game1.multiplayer.broadcastSprites(this, new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(666, 1851, 8, 8), 25f, 4, 2, new Vector2(3f, 19f) * 64f + new Vector2((float) (index % 4 * 16 /*0x10*/ - 8), (float) (-(index / 4) * 64 /*0x40*/ / 4)), false, false)
                {
                  layerDepth = (float) (0.11519999802112579 + (double) index / 10000.0),
                  color = new Color(100 + index * 4, index * 6, 120 + index * 4),
                  pingPong = true,
                  delayBeforeAnimationStart = 640 + index * 10,
                  scale = 4f,
                  alphaFade = 0.01f
                });
              }
              Game1.player.jitterStrength = 2f;
              Game1.player.freezePause = 500;
              this.playSound("debuffHit");
              break;
            case 12:
              switch (key[0])
              {
                case 'P':
                  if (!(key == "PoolEntrance"))
                    return;
                  if (!Game1.player.swimming.Value)
                  {
                    Game1.player.swimTimer = 800;
                    Game1.player.swimming.Value = true;
                    Game1.player.position.Y += 16f;
                    Game1.player.yVelocity = -8f;
                    this.playSound("pullItemFromWater");
                    Game1.multiplayer.broadcastSprites(this, new TemporaryAnimatedSprite(27, 100f, 4, 0, new Vector2(Game1.player.Position.X, (float) (Game1.player.StandingPixel.Y - 40)), false, false)
                    {
                      layerDepth = 1f,
                      motion = new Vector2(0.0f, 2f)
                    });
                  }
                  else
                  {
                    Game1.player.jump();
                    Game1.player.swimTimer = 800;
                    Game1.player.position.X = playerStandingPosition.X * 64f;
                    this.playSound("pullItemFromWater");
                    Game1.player.yVelocity = 8f;
                    Game1.player.swimming.Value = false;
                  }
                  Game1.player.noMovementPause = 500;
                  return;
                case 'W':
                  if (!(key == "WomensLocker") || !Game1.player.IsMale)
                    return;
                  Game1.player.position.Y += (float) (((double) Game1.player.Speed + (double) Game1.player.addedSpeed) * 2.0);
                  Game1.player.Halt();
                  Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:WomensLocker_WrongGender"));
                  return;
                default:
                  return;
              }
            case 13:
              if (!(key == "FaceDirection"))
                break;
              string name2;
              int direction;
              if (!ArgUtility.TryGet(action, 1, out name2, out error, name: "string npcName") || !ArgUtility.TryGetInt(action, 2, out direction, out error, "int direction"))
              {
                LogError(error);
                break;
              }
              this.getCharacterFromName(name2)?.faceDirection(direction);
              break;
            case 14:
              if (!(key == "legendarySword"))
                break;
              if (Game1.player.ActiveObject?.QualifiedItemId == "(O)74" && !Game1.player.mailReceived.Contains("galaxySword"))
              {
                Game1.player.Halt();
                Game1.player.faceDirection(2);
                Game1.player.showCarrying();
                Game1.player.jitterStrength = 1f;
                Game1.pauseThenDoFunction(7000, new Game1.afterFadeFunction(this.getGalaxySword));
                Game1.changeMusicTrack("none", music_context: MusicContext.Event);
                this.playSound("crit");
                Game1.screenGlowOnce(new Color(30, 0, 150), true, 0.01f, 0.999f);
                DelayedAction.playSoundAfterDelay("stardrop", 1500);
                Game1.screenOverlayTempSprites.AddRange((IEnumerable<TemporaryAnimatedSprite>) Utility.sparkleWithinArea(new Microsoft.Xna.Framework.Rectangle(0, 0, Game1.viewport.Width, Game1.viewport.Height), 500, Color.White, 10, 2000));
                Game1.afterDialogues += (Game1.afterFadeFunction) (() => Game1.stopMusicTrack(MusicContext.Event));
                break;
              }
              if (Game1.player.mailReceived.Contains("galaxySword"))
                break;
              this.localSound("SpringBirds");
              break;
            case 15:
              if (!(key == "ConditionalDoor") || action.Length <= 1 || Game1.eventUp || GameStateQuery.CheckConditions(ArgUtility.UnsplitQuoteAware(action, ' ', 1)))
                break;
              Farmer player2 = Game1.player;
              player2.Position = player2.Position - Game1.player.getMostRecentMovementVector() * 2f;
              Game1.player.yVelocity = 0.0f;
              Game1.player.Halt();
              Game1.player.TemporaryPassableTiles.Clear();
              if (Game1.player.Tile == this.lastTouchActionLocation)
              {
                if ((double) Game1.player.Position.Y > (double) this.lastTouchActionLocation.Y * 64.0 + 32.0)
                  Game1.player.position.Y += 4f;
                else
                  Game1.player.position.Y -= 4f;
                this.lastTouchActionLocation = Vector2.Zero;
              }
              string text = this.doesTileHaveProperty((int) playerStandingPosition.X / 64 /*0x40*/, (int) playerStandingPosition.Y / 64 /*0x40*/, "LockedDoorMessage", "Back");
              if (text != null)
              {
                Game1.drawObjectDialogue(Game1.content.LoadString(TokenParser.ParseText(text)));
                break;
              }
              Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:LockedDoor"));
              break;
            case 18:
              if (!(key == "ChangeIntoSwimsuit"))
                break;
              Game1.player.changeIntoSwimsuit();
              break;
            case 19:
              if (!(key == "ChangeOutOfSwimsuit"))
                break;
              Game1.player.changeOutOfSwimSuit();
              break;
          }
        }
      }
    }
    catch (Exception ex)
    {
    }

    void LogError(string errorPhrase)
    {
      this.LogTileTouchActionError(action, playerStandingPosition, errorPhrase);
    }
  }

  public virtual void updateMap()
  {
    if (!this._mapPathDirty)
      return;
    this._mapPathDirty = false;
    if (string.Equals(this.mapPath.Value, this.loadedMapPath, StringComparison.Ordinal))
      return;
    this.reloadMap();
    this.updateLayout();
  }

  public virtual void updateLayout()
  {
    if (!Game1.IsMasterGame)
      return;
    this.updateDoors();
    this.updateWarps();
  }

  public LargeTerrainFeature getLargeTerrainFeatureAt(int tileX, int tileY)
  {
    foreach (LargeTerrainFeature largeTerrainFeature in this.largeTerrainFeatures)
    {
      if (largeTerrainFeature.getBoundingBox().Contains(tileX * 64 /*0x40*/ + 32 /*0x20*/, tileY * 64 /*0x40*/ + 32 /*0x20*/))
        return largeTerrainFeature;
    }
    return (LargeTerrainFeature) null;
  }

  public virtual void UpdateWhenCurrentLocation(GameTime time)
  {
    this.updateMap();
    if (this.wasUpdated)
      return;
    this.wasUpdated = true;
    if (this._mapSeatsDirty)
      this.UpdateMapSeats();
    this.furnitureToRemove.Update(this);
    if (Game1.player.currentLocation.Equals(this))
      this._updateAmbientLighting();
    for (int index = 0; index < this.furniture.Count; ++index)
      this.furniture[index].updateWhenCurrentLocation(time);
    AmbientLocationSounds.update(time);
    this.critters?.RemoveAll((Predicate<Critter>) (critter => critter.update(time, this)));
    if (this.fishSplashAnimation != null)
    {
      this.fishSplashAnimation.update(time);
      bool flag = this.fishFrenzyFish.Value != null && !this.fishFrenzyFish.Value.Equals("");
      double num1 = flag ? 0.1 : 0.02;
      ICue cue;
      if (Game1.random.NextDouble() < num1)
      {
        this.temporarySprites.Add(new TemporaryAnimatedSprite(0, this.fishSplashAnimation.position + new Vector2((float) Game1.random.Next(-32, 32 /*0x20*/), (float) Game1.random.Next(-32, 32 /*0x20*/)), Color.White * 0.3f)
        {
          layerDepth = (float) (((double) this.fishSplashAnimation.position.Y - 64.0) / 10000.0)
        });
        if (flag)
        {
          this.temporarySprites.Add(new TemporaryAnimatedSprite(0, this.fishSplashAnimation.position + new Vector2((float) Game1.random.Next(-64, 64 /*0x40*/), (float) Game1.random.Next(-64, 64 /*0x40*/)), Color.White * 0.3f)
          {
            layerDepth = (float) (((double) this.fishSplashAnimation.position.Y - 64.0) / 10000.0)
          });
          if (Game1.random.NextDouble() < 0.1)
            Game1.sounds.PlayLocal("slosh", this, new Vector2?(this.fishSplashAnimation.Position / 64f), new int?(), SoundContext.Default, out cue);
        }
      }
      if (flag && Game1.random.NextDouble() < 0.005)
      {
        Vector2 position = this.fishSplashAnimation.position + new Vector2((float) Game1.random.Next(-32, 32 /*0x20*/), (float) Game1.random.Next(-32, 32 /*0x20*/));
        Action<Vector2> splashAnimation = (Action<Vector2>) (pos => this.TemporarySprites.Add(new TemporaryAnimatedSprite(28, 100f, 2, 1, pos, false, false)
        {
          delayBeforeAnimationStart = 0,
          layerDepth = (float) (((double) pos.Y + 1.0) / 10000.0)
        }));
        Game1.sounds.PlayLocal("slosh", this, new Vector2?(this.fishSplashAnimation.Position / 64f), new int?(), SoundContext.Default, out cue);
        splashAnimation(position);
        ParsedItemData data = ItemRegistry.GetData(this.fishFrenzyFish.Value);
        int spriteID = 982648 + Game1.random.Next(99999);
        bool flipped = Game1.random.NextDouble() < 0.5;
        float num2 = (float) Game1.random.Next(10, 20) / 10f;
        if (Game1.random.NextDouble() < 0.9)
          num2 *= 0.75f;
        this.TemporarySprites.Add(new TemporaryAnimatedSprite(data.GetTextureName(), data.GetSourceRect(), position, flipped, 0.0f, Color.White)
        {
          scale = 4f,
          motion = new Vector2((float) ((flipped ? -1.0 : 1.0) * ((double) Game1.random.Next(11) * (double) num2 + (double) num2 * 5.0) / 20.0), (float) (-((double) Game1.random.Next(30, 41) * (double) num2) / 10.0)),
          acceleration = new Vector2(0.0f, 0.1f),
          rotationChange = (float) ((flipped ? -1.0 : 1.0) * ((double) Game1.random.Next(5, 10) * (double) num2) / 800.0),
          yStopCoordinate = (int) position.Y + 1,
          id = spriteID,
          layerDepth = position.Y / 10000f,
          reachedStopCoordinateSprite = (Action<TemporaryAnimatedSprite>) (x =>
          {
            this.removeTemporarySpritesWithID(spriteID);
            Game1.sounds.PlayLocal("dropItemInWater", this, new Vector2?(position / 64f), new int?(), SoundContext.Default, out ICue _);
            splashAnimation(x.Position);
          })
        });
      }
    }
    if (this.orePanAnimation != null)
    {
      this.orePanAnimation.update(time);
      if (Game1.random.NextDouble() < 0.05)
        this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(432, 1435, 16 /*0x10*/, 16 /*0x10*/), this.orePanAnimation.position + new Vector2((float) Game1.random.Next(-32, 32 /*0x20*/), (float) Game1.random.Next(-32, 32 /*0x20*/)), false, 0.02f, Color.White * 0.8f)
        {
          scale = 2f,
          animationLength = 6,
          interval = 100f
        });
    }
    this.interiorDoors.Update(time);
    this.updateWater(time);
    this.Map.Update((long) time.ElapsedGameTime.Milliseconds);
    this.debris.RemoveWhere((Func<Debris, bool>) (d => d.updateChunks(time, this)));
    if (Game1.shouldTimePass() || Game1.isFestival())
      this.projectiles.RemoveWhere((Func<Projectile, bool>) (projectile => projectile.update(time, this)));
    for (int index = this._activeTerrainFeatures.Count - 1; index >= 0; --index)
    {
      TerrainFeature activeTerrainFeature = this._activeTerrainFeatures[index];
      if (activeTerrainFeature.tickUpdate(time))
        this.terrainFeatures.Remove(activeTerrainFeature.Tile);
    }
    this.largeTerrainFeatures?.RemoveWhere((Func<LargeTerrainFeature, bool>) (feature => feature.tickUpdate(time)));
    foreach (TerrainFeature resourceClump in this.resourceClumps)
      resourceClump.tickUpdate(time);
    if (this.currentEvent != null)
    {
      bool flag;
      do
      {
        int currentCommand = this.currentEvent.CurrentCommand;
        this.currentEvent.Update(this, time);
        if (this.currentEvent != null)
        {
          flag = this.currentEvent.simultaneousCommand;
          if (currentCommand == this.currentEvent.CurrentCommand)
            flag = false;
        }
        else
          flag = false;
      }
      while (flag);
    }
    this.objects.Lock();
    foreach (Object @object in this.objects.Values)
      @object.updateWhenCurrentLocation(time);
    this.objects.Unlock();
    if (Game1.gameMode == (byte) 3 && this == Game1.currentLocation)
    {
      if (Game1.currentLocation.GetLocationContext().PlayRandomAmbientSounds && this.isOutdoors.Value)
      {
        if (!this.IsRainingHere())
        {
          if (Game1.timeOfDay < 2000)
          {
            if (Game1.isMusicContextActiveButNotPlaying() && !this.IsWinterHere() && Game1.random.NextDouble() < 0.002)
              this.localSound("SpringBirds");
          }
          else if (Game1.timeOfDay > 2100 && !(this is Beach) && this.IsSummerHere() && !this.IsTemporary && Game1.random.NextDouble() < 0.0005)
            this.localSound("crickets");
        }
        else if (!Game1.eventUp && (double) Game1.options.musicVolumeLevel > 0.0 && Game1.random.NextDouble() < 0.00015 && !this.name.Equals((object) "Town"))
          this.localSound("rainsound");
      }
      Vector2 tile1 = Game1.player.Tile;
      if (this.lastTouchActionLocation.Equals(Vector2.Zero))
      {
        string fullActionString = this.doesTileHaveProperty((int) tile1.X, (int) tile1.Y, "TouchAction", "Back");
        this.lastTouchActionLocation = tile1;
        if (fullActionString != null)
          this.performTouchAction(fullActionString, tile1);
      }
      else if (!this.lastTouchActionLocation.Equals(tile1))
        this.lastTouchActionLocation = Vector2.Zero;
      foreach (Farmer farmer in this.farmers)
      {
        Vector2 tile2 = farmer.Tile;
        foreach (Vector2 vectorsWithDiagonal in Utility.DirectionsTileVectorsWithDiagonals)
        {
          Object @object;
          if (this.objects.TryGetValue(tile2 + vectorsWithDiagonal, out @object))
            @object.farmerAdjacentAction(farmer, (double) vectorsWithDiagonal.X != 0.0 && (double) vectorsWithDiagonal.Y != 0.0);
        }
      }
      if (Game1.player != null)
      {
        int index = Game1.player.facingDirection.Value;
        Vector2 player_position = Game1.player.Tile;
        Object @object = (Object) null;
        if (index >= 0 && index < 4)
        {
          Vector2 directionsTileVector = Utility.DirectionsTileVectors[index];
          @object = CheckForSign((int) directionsTileVector.X, (int) directionsTileVector.Y);
        }
        if (@object == null)
          @object = CheckForSign(0, -1) ?? CheckForSign(0, 1) ?? CheckForSign(-1, 0) ?? CheckForSign(1, 0) ?? CheckForSign(-1, -1) ?? CheckForSign(1, -1) ?? CheckForSign(-1, 1) ?? CheckForSign(1, 1);
        if (@object != null)
          @object.shouldShowSign = true;

        Object CheckForSign(int offsetX, int offsetY)
        {
          Object @object;
          // ISSUE: reference to a compiler-generated field
          return !this.\u003C\u003E4__this.objects.TryGetValue(player_position + new Vector2((float) offsetX, (float) offsetY), out @object) || !@object.IsTextSign() ? (Object) null : @object;
        }
      }
    }
    foreach (KeyValuePair<long, FarmAnimal> pair in this.animals.Pairs)
      this.tempAnimals.Add(pair);
    foreach (KeyValuePair<long, FarmAnimal> tempAnimal in this.tempAnimals)
    {
      if (tempAnimal.Value.updateWhenCurrentLocation(time, this))
        this.animals.Remove(tempAnimal.Key);
    }
    this.tempAnimals.Clear();
    foreach (Building building in this.buildings)
      building.Update(time);
  }

  public void updateWater(GameTime time)
  {
    this.waterAnimationTimer -= time.ElapsedGameTime.Milliseconds;
    if (this.waterAnimationTimer <= 0)
    {
      this.waterAnimationIndex = (this.waterAnimationIndex + 1) % 10;
      this.waterAnimationTimer = 200;
    }
    this.waterPosition += !this.isFarm.Value ? (float) ((Math.Sin((double) time.TotalGameTime.Milliseconds / 1000.0) + 1.0) * 0.15000000596046448) : 0.1f;
    if ((double) this.waterPosition < 64.0)
      return;
    this.waterPosition -= 64f;
    this.waterTileFlip = !this.waterTileFlip;
  }

  public NPC getCharacterFromName(string name)
  {
    NPC characterFromName = (NPC) null;
    foreach (NPC character in this.characters)
    {
      if (character.Name.Equals(name))
        return character;
    }
    return characterFromName;
  }

  protected virtual void updateCharacters(GameTime time)
  {
    bool flag = Game1.shouldTimePass();
    for (int index = this.characters.Count - 1; index >= 0; --index)
    {
      NPC character = this.characters[index];
      if (character != null && (flag || character is Horse || character.forceUpdateTimer > 0))
      {
        character.currentLocation = this;
        character.update(time, this);
        if (index < this.characters.Count && character is Monster monster && monster.ShouldMonsterBeRemoved())
          this.characters.RemoveAt(index);
      }
      else if (character != null)
      {
        if (character.hasJustStartedFacingPlayer)
          character.updateFaceTowardsFarmer(time, this);
        character.updateEmote(time);
      }
    }
  }

  public Projectile getProjectileFromID(int uniqueID)
  {
    foreach (Projectile projectile in this.projectiles)
    {
      if (projectile.uniqueID.Value == uniqueID)
        return projectile;
    }
    return (Projectile) null;
  }

  public virtual void updateEvenIfFarmerIsntHere(GameTime time, bool ignoreWasUpdatedFlush = false)
  {
    this.netAudio.Update();
    this.removeTemporarySpritesWithIDEvent.Poll();
    this.rumbleAndFadeEvent.Poll();
    this.damagePlayersEvent.Poll();
    if (!ignoreWasUpdatedFlush)
      this.wasUpdated = false;
    this.updateCharacters(time);
    for (int index = this.temporarySprites.Count - 1; index >= 0; --index)
    {
      TemporaryAnimatedSprite temporarySprite = index < this.temporarySprites.Count ? this.temporarySprites[index] : (TemporaryAnimatedSprite) null;
      if (index < this.temporarySprites.Count && temporarySprite != null && temporarySprite.update(time) && index < this.temporarySprites.Count)
        this.temporarySprites.RemoveAt(index);
    }
    foreach (Building building in this.buildings)
      building.updateWhenFarmNotCurrentLocation(time);
    if (Game1.currentLocation.Equals(this) || this.animals.Length <= 0)
      return;
    Building parentBuilding = this.ParentBuilding;
    foreach (FarmAnimal farmAnimal in this.animals.Values.ToArray<FarmAnimal>())
      farmAnimal.updateWhenNotCurrentLocation(parentBuilding, time, this);
  }

  /// <summary>Get the location which contains this one, if applicable.</summary>
  /// <remarks>
  ///   <para>For example, the interior for a farm building will have the farm as its root location.</para>
  ///   <para>See also <see cref="M:StardewValley.GameLocation.GetRootLocation" />.</para>
  /// </remarks>
  public GameLocation GetParentLocation()
  {
    return this.parentLocationName.Value == null ? (GameLocation) null : Game1.getLocationFromName(this.parentLocationName.Value);
  }

  /// <summary>Get the parent location which contains this one, or the current location if it has no parent.</summary>
  /// <remarks>See also <see cref="M:StardewValley.GameLocation.GetParentLocation" />.</remarks>
  public GameLocation GetRootLocation() => this.GetParentLocation() ?? this;

  public Response[] createYesNoResponses()
  {
    return new Response[2]
    {
      new Response("Yes", Game1.content.LoadString("Strings\\Lexicon:QuestionDialogue_Yes")).SetHotKey(Keys.Y),
      new Response("No", Game1.content.LoadString("Strings\\Lexicon:QuestionDialogue_No")).SetHotKey(Keys.Escape)
    };
  }

  public virtual void customQuestCompleteBehavior(string questId)
  {
  }

  public void createQuestionDialogue(string question, Response[] answerChoices, string dialogKey)
  {
    this.lastQuestionKey = dialogKey;
    Game1.drawObjectQuestionDialogue(question, answerChoices);
  }

  public void createQuestionDialogueWithCustomWidth(
    string question,
    Response[] answerChoices,
    string dialogKey)
  {
    int width = SpriteText.getWidthOfString(question) + 64 /*0x40*/;
    this.lastQuestionKey = dialogKey;
    Game1.drawObjectQuestionDialogue(question, answerChoices, width);
  }

  public void createQuestionDialogue(
    string question,
    Response[] answerChoices,
    GameLocation.afterQuestionBehavior afterDialogueBehavior,
    NPC speaker = null)
  {
    this.lastQuestionKey = (string) null;
    this.afterQuestion = afterDialogueBehavior;
    Game1.drawObjectQuestionDialogue(question, answerChoices);
    if (speaker == null)
      return;
    Game1.objectDialoguePortraitPerson = speaker;
  }

  public void createQuestionDialogue(
    string question,
    Response[] answerChoices,
    string dialogKey,
    Object actionObject)
  {
    this.lastQuestionKey = dialogKey;
    Game1.drawObjectQuestionDialogue(question, answerChoices);
    this.actionObjectForQuestionDialogue = actionObject;
  }

  public virtual void monsterDrop(Monster monster, int x, int y, Farmer who)
  {
    IList<string> objectsToDrop = (IList<string>) monster.objectsToDrop;
    Vector2 vector2 = Utility.PointToVector2(who.StandingPixel);
    List<Item> extraDropItems1 = monster.getExtraDropItems();
    string str1;
    if (who.isWearingRing("526") && DataLoader.Monsters(Game1.content).TryGetValue(monster.Name, out str1))
    {
      string[] strArray = ArgUtility.SplitBySpace(str1.Split('/')[6]);
      for (int index = 0; index < strArray.Length; index += 2)
      {
        if (Game1.random.NextDouble() < Convert.ToDouble(strArray[index + 1]))
          objectsToDrop.Add(strArray[index]);
      }
    }
    List<Debris> debrisList = new List<Debris>();
    for (int index = 0; index < objectsToDrop.Count; ++index)
    {
      string str2 = objectsToDrop[index];
      int result;
      if (str2 != null && str2.StartsWith('-') && int.TryParse(str2, out result))
        debrisList.Add(monster.ModifyMonsterLoot(new Debris(Math.Abs(result), Game1.random.Next(1, 4), new Vector2((float) x, (float) y), vector2)));
      else
        debrisList.Add(monster.ModifyMonsterLoot(new Debris(str2, new Vector2((float) x, (float) y), vector2)));
    }
    for (int index = 0; index < extraDropItems1.Count; ++index)
      debrisList.Add(monster.ModifyMonsterLoot(new Debris(extraDropItems1[index], new Vector2((float) x, (float) y), vector2)));
    Trinket.TrySpawnTrinket(this, monster, monster.getStandingPosition());
    if (who.isWearingRing("526"))
    {
      List<Item> extraDropItems2 = monster.getExtraDropItems();
      for (int index = 0; index < extraDropItems2.Count; ++index)
      {
        Item one = extraDropItems2[index].getOne();
        one.Stack = extraDropItems2[index].Stack;
        one.HasBeenInInventory = false;
        debrisList.Add(monster.ModifyMonsterLoot(new Debris(one, new Vector2((float) x, (float) y), vector2)));
      }
    }
    foreach (Debris debris in debrisList)
      this.debris.Add(debris);
    if (who.stats.Get("Book_Void") > 0U && Game1.random.NextDouble() < 0.03 && debrisList != null && monster != null)
    {
      foreach (Debris debris in debrisList)
      {
        if (debris.item != null)
        {
          Item one = debris.item.getOne();
          if (one != null)
          {
            one.Stack = debris.item.Stack;
            one.HasBeenInInventory = false;
            this.debris.Add(monster.ModifyMonsterLoot(new Debris(one, new Vector2((float) x, (float) y), vector2)));
          }
        }
        else if (debris.itemId.Value != null && debris.itemId.Value.Length > 0)
        {
          Item obj = ItemRegistry.Create(debris.itemId.Value);
          obj.HasBeenInInventory = false;
          this.debris.Add(monster.ModifyMonsterLoot(new Debris(obj, new Vector2((float) x, (float) y), vector2)));
        }
      }
    }
    if (this.HasUnlockedAreaSecretNotes(who) && Game1.random.NextDouble() < 0.033)
    {
      Object unseenSecretNote = this.tryToCreateUnseenSecretNote(who);
      if (unseenSecretNote != null)
        monster.ModifyMonsterLoot(Game1.createItemDebris((Item) unseenSecretNote, new Vector2((float) x, (float) y), -1, this));
    }
    Utility.trySpawnRareObject(who, new Vector2((float) x, (float) y), this, 1.5);
    if (Utility.tryRollMysteryBox(0.01 + who.team.AverageDailyLuck() / 10.0 + (double) who.LuckLevel * 0.008))
      monster.ModifyMonsterLoot(Game1.createItemDebris(ItemRegistry.Create(who.stats.Get(StatKeys.Mastery(2)) > 0U ? "(O)GoldenMysteryBox" : "(O)MysteryBox"), new Vector2((float) x, (float) y), -1, this));
    if (who.stats.MonstersKilled > 10U && Game1.random.NextDouble() < 0.0001 + (!who.mailReceived.Contains("voidBookDropped") ? (double) who.stats.MonstersKilled * 1.5E-05 : 0.0004))
    {
      monster.ModifyMonsterLoot(Game1.createItemDebris(ItemRegistry.Create("(O)Book_Void"), new Vector2((float) x, (float) y), -1, this));
      who.mailReceived.Add("voidBookDropped");
    }
    if (this is Woods && Game1.random.NextDouble() < 0.1)
      monster.ModifyMonsterLoot(Game1.createItemDebris(ItemRegistry.Create("(O)292"), new Vector2((float) x, (float) y), -1, this));
    if (Game1.netWorldState.Value.GoldenWalnutsFound < 100)
      return;
    if (monster.isHardModeMonster.Value && Game1.stats.Get("hardModeMonstersKilled") > 50U && Game1.random.NextDouble() < 0.001 + (double) who.LuckLevel * 0.00019999999494757503)
    {
      monster.ModifyMonsterLoot(Game1.createItemDebris(ItemRegistry.Create("(O)896"), new Vector2((float) x, (float) y), -1, this));
    }
    else
    {
      if (!monster.isHardModeMonster.Value || Game1.random.NextDouble() >= 0.008 + (double) who.LuckLevel * (1.0 / 500.0))
        return;
      monster.ModifyMonsterLoot(Game1.createItemDebris(ItemRegistry.Create("(O)858"), new Vector2((float) x, (float) y), -1, this));
    }
  }

  public virtual bool HasUnlockedAreaSecretNotes(Farmer who)
  {
    return this.InIslandContext() || who.hasMagnifyingGlass;
  }

  public bool damageMonster(
    Microsoft.Xna.Framework.Rectangle areaOfEffect,
    int minDamage,
    int maxDamage,
    bool isBomb,
    Farmer who,
    bool isProjectile = false)
  {
    return this.damageMonster(areaOfEffect, minDamage, maxDamage, isBomb, 1f, 0, 0.0f, 1f, false, who, isProjectile);
  }

  private bool isMonsterDamageApplicable(Farmer who, Monster monster, bool horizontalBias = true)
  {
    if (!monster.isGlider.Value && !(who.CurrentTool is Slingshot) && !monster.ignoreDamageLOS.Value)
    {
      Point tilePoint1 = who.TilePoint;
      Point tilePoint2 = monster.TilePoint;
      if (Math.Abs(tilePoint1.X - tilePoint2.X) + Math.Abs(tilePoint1.Y - tilePoint2.Y) > 1)
      {
        int num1 = tilePoint2.X - tilePoint1.X;
        int num2 = tilePoint2.Y - tilePoint1.Y;
        Vector2 key = new Vector2((float) tilePoint1.X, (float) tilePoint1.Y);
        while (num1 != 0 || num2 != 0)
        {
          if (horizontalBias)
          {
            if (Math.Abs(num1) >= Math.Abs(num2))
            {
              key.X += (float) Math.Sign(num1);
              num1 -= Math.Sign(num1);
            }
            else
            {
              key.Y += (float) Math.Sign(num2);
              num2 -= Math.Sign(num2);
            }
          }
          else if (Math.Abs(num2) >= Math.Abs(num1))
          {
            key.Y += (float) Math.Sign(num2);
            num2 -= Math.Sign(num2);
          }
          else
          {
            key.X += (float) Math.Sign(num1);
            num1 -= Math.Sign(num1);
          }
          Object @object;
          if (this.objects.TryGetValue(key, out @object) && !@object.isPassable() || this.BlocksDamageLOS((int) key.X, (int) key.Y))
            return false;
        }
      }
    }
    return true;
  }

  public virtual bool BlocksDamageLOS(int x, int y)
  {
    return this.hasTileAt(x, y, "Buildings") && this.doesTileHaveProperty(x, y, "Passable", "Buildings") == null;
  }

  public bool damageMonster(
    Microsoft.Xna.Framework.Rectangle areaOfEffect,
    int minDamage,
    int maxDamage,
    bool isBomb,
    float knockBackModifier,
    int addedPrecision,
    float critChance,
    float critMultiplier,
    bool triggerMonsterInvincibleTimer,
    Farmer who,
    bool isProjectile = false)
  {
    bool flag1 = false;
    for (int index = this.characters.Count - 1; index >= 0; --index)
    {
      if (index < this.characters.Count && this.characters[index] is Monster character && character.IsMonster && character.Health > 0 && character.TakesDamageFromHitbox(areaOfEffect))
      {
        if (character.currentLocation == null)
          character.currentLocation = this;
        if (!character.IsInvisible && !character.isInvincible() && (isBomb || isProjectile || this.isMonsterDamageApplicable(who, character) || this.isMonsterDamageApplicable(who, character, false)))
        {
          bool flag2 = !isBomb && who?.CurrentTool is MeleeWeapon currentTool && currentTool.type.Value == 1;
          bool flag3 = false;
          if (flag2 && MeleeWeapon.daggerHitsLeft > 1)
            flag3 = true;
          if (flag3)
            triggerMonsterInvincibleTimer = false;
          flag1 = true;
          if (Game1.currentLocation == this)
            Rumble.rumble(0.1f + (float) (Game1.random.NextDouble() / 8.0), (float) (200 + Game1.random.Next(-50, 50)));
          Microsoft.Xna.Framework.Rectangle boundingBox = character.GetBoundingBox();
          Vector2 trajectory = Utility.getAwayFromPlayerTrajectory(boundingBox, who);
          if ((double) knockBackModifier > 0.0)
            trajectory *= knockBackModifier;
          else
            trajectory = new Vector2(character.xVelocity, character.yVelocity);
          if (character.Slipperiness == -1)
            trajectory = Vector2.Zero;
          bool isCriticalHit = false;
          if (who?.CurrentTool != null && character.hitWithTool(who.CurrentTool))
            return false;
          if (who.hasBuff("statue_of_blessings_5"))
            critChance += 0.1f;
          if (who.professions.Contains(25))
            critChance += critChance * 0.5f;
          int num1;
          if (maxDamage >= 0)
          {
            int num2 = Game1.random.Next(minDamage, maxDamage + 1);
            if (who != null && Game1.random.NextDouble() < (double) critChance + (double) who.LuckLevel * ((double) critChance / 40.0))
            {
              isCriticalHit = true;
              this.playSound("crit");
              if (who.hasTrinketWithID("IridiumSpur"))
              {
                BuffEffects effects = new BuffEffects();
                effects.Speed.Value = 1f;
                who.applyBuff(new Buff("iridiumspur", displaySource: Game1.content.LoadString("Strings\\1_6_Strings:IridiumSpur_Name"), duration: who.getFirstTrinketWithID("IridiumSpur").GetEffect().GeneralStat * 1000, iconTexture: Game1.objectSpriteSheet_2, iconSheetIndex: 76, effects: effects, isDebuff: new bool?(false)));
              }
            }
            int amount = Math.Max(1, (isCriticalHit ? (int) ((double) num2 * (double) critMultiplier) : num2) + (who != null ? who.Attack * 3 : 0));
            if (who != null && who.professions.Contains(24))
              amount = (int) Math.Ceiling((double) amount * 1.1000000238418579);
            if (who != null && who.professions.Contains(26))
              amount = (int) Math.Ceiling((double) amount * 1.1499999761581421);
            if (who != null & isCriticalHit && who.professions.Contains(29))
              amount = (int) ((double) amount * 2.0);
            if (who != null)
            {
              foreach (BaseEnchantment enchantment in who.enchantments)
                enchantment.OnCalculateDamage(character, this, who, isBomb, ref amount);
            }
            num1 = character.takeDamage(amount, (int) trajectory.X, (int) trajectory.Y, isBomb, (double) addedPrecision / 10.0, who);
            if (flag3)
            {
              if (character.stunTime.Value < 50)
                character.stunTime.Value = 50;
            }
            else if (character.stunTime.Value < 50)
              character.stunTime.Value = 0;
            if (num1 == -1)
            {
              this.debris.Add(new Debris(Game1.content.LoadString("Strings\\StringsFromCSFiles:Attack_Miss"), 1, new Vector2((float) boundingBox.Center.X, (float) boundingBox.Center.Y), Color.LightGray, 1f, 0.0f));
            }
            else
            {
              this.removeDamageDebris(character);
              this.debris.Add(new Debris(num1, new Vector2((float) (boundingBox.Center.X + 16 /*0x10*/), (float) boundingBox.Center.Y), isCriticalHit ? Color.Yellow : new Color((int) byte.MaxValue, 130, 0), isCriticalHit ? (float) (1.0 + (double) num1 / 300.0) : 1f, (Character) character));
              if (who != null)
              {
                foreach (BaseEnchantment enchantment in who.enchantments)
                  enchantment.OnDealtDamage(character, this, who, isBomb, num1);
              }
            }
            if (triggerMonsterInvincibleTimer)
              character.setInvincibleCountdown(450 / (flag2 ? 3 : 2));
            if (who != null)
            {
              foreach (Trinket trinketItem in who.trinketItems)
                trinketItem?.OnDamageMonster(who, character, num1, isBomb, isCriticalHit);
            }
          }
          else
          {
            num1 = -2;
            character.setTrajectory(trajectory);
            if (character.Slipperiness > 10)
            {
              character.xVelocity /= 2f;
              character.yVelocity /= 2f;
            }
          }
          if (who?.CurrentTool?.QualifiedItemId == "(W)4")
            Game1.multiplayer.broadcastSprites(this, new TemporaryAnimatedSprite(362, (float) Game1.random.Next(50, 120), 6, 1, new Vector2((float) (boundingBox.Center.X - 32 /*0x20*/), (float) (boundingBox.Center.Y - 32 /*0x20*/)), false, false));
          if (character.Health <= 0)
            this.onMonsterKilled(who, character, boundingBox, isBomb);
          else if (num1 > 0)
          {
            character.shedChunks(Game1.random.Next(1, 3));
            if (isCriticalHit)
            {
              Vector2 standingPosition = character.getStandingPosition();
              Game1.multiplayer.broadcastSprites(this, new TemporaryAnimatedSprite(362, (float) Game1.random.Next(15, 50), 6, 1, standingPosition - new Vector2(32f, 32f), false, Game1.random.NextBool())
              {
                scale = 0.75f,
                alpha = isCriticalHit ? 0.75f : 0.5f
              });
              Game1.multiplayer.broadcastSprites(this, new TemporaryAnimatedSprite(362, (float) Game1.random.Next(15, 50), 6, 1, standingPosition - new Vector2((float) (32 /*0x20*/ + Game1.random.Next(-21, 21) + 32 /*0x20*/), (float) (32 /*0x20*/ + Game1.random.Next(-21, 21))), false, Game1.random.NextBool())
              {
                scale = 0.5f,
                delayBeforeAnimationStart = 50,
                alpha = isCriticalHit ? 0.75f : 0.5f
              });
              Game1.multiplayer.broadcastSprites(this, new TemporaryAnimatedSprite(362, (float) Game1.random.Next(15, 50), 6, 1, standingPosition - new Vector2((float) (32 /*0x20*/ + Game1.random.Next(-21, 21) - 32 /*0x20*/), (float) (32 /*0x20*/ + Game1.random.Next(-21, 21))), false, Game1.random.NextBool())
              {
                scale = 0.5f,
                delayBeforeAnimationStart = 100,
                alpha = isCriticalHit ? 0.75f : 0.5f
              });
              Game1.multiplayer.broadcastSprites(this, new TemporaryAnimatedSprite(362, (float) Game1.random.Next(15, 50), 6, 1, standingPosition - new Vector2((float) (32 /*0x20*/ + Game1.random.Next(-21, 21) + 32 /*0x20*/), (float) (32 /*0x20*/ + Game1.random.Next(-21, 21))), false, Game1.random.NextBool())
              {
                scale = 0.5f,
                delayBeforeAnimationStart = 150,
                alpha = isCriticalHit ? 0.75f : 0.5f
              });
              Game1.multiplayer.broadcastSprites(this, new TemporaryAnimatedSprite(362, (float) Game1.random.Next(15, 50), 6, 1, standingPosition - new Vector2((float) (32 /*0x20*/ + Game1.random.Next(-21, 21) - 32 /*0x20*/), (float) (32 /*0x20*/ + Game1.random.Next(-21, 21))), false, Game1.random.NextBool())
              {
                scale = 0.5f,
                delayBeforeAnimationStart = 200,
                alpha = isCriticalHit ? 0.75f : 0.5f
              });
            }
          }
        }
      }
    }
    return flag1;
  }

  /// <summary>Handle a monster reaching zero health after being hit by the player.</summary>
  /// <param name="who">The player who damaged the monster.</param>
  /// <param name="monster">The monster whose health reached zero.</param>
  /// <param name="monsterBox">The monster's pixel hitbox.</param>
  /// <param name="killedByBomb">Whether the monster was killed by a bomb placed by the player.</param>
  private void onMonsterKilled(
    Farmer who,
    Monster monster,
    Microsoft.Xna.Framework.Rectangle monsterBox,
    bool killedByBomb)
  {
    bool isHutchSlime = false;
    bool flag = false;
    if (monster is GreenSlime greenSlime)
    {
      isHutchSlime = this is SlimeHutch;
      flag = !greenSlime.firstGeneration.Value;
    }
    who.NotifyQuests((Func<Quest, bool>) (quest => quest.OnMonsterSlain(this, monster, killedByBomb, isHutchSlime)));
    if (!isHutchSlime && Game1.player.team.specialOrders != null)
    {
      foreach (SpecialOrder specialOrder in Game1.player.team.specialOrders)
      {
        Action<Farmer, Monster> onMonsterSlain = specialOrder.onMonsterSlain;
        if (onMonsterSlain != null)
          onMonsterSlain(Game1.player, monster);
      }
    }
    if (who != null)
    {
      foreach (BaseEnchantment enchantment in who.enchantments)
        enchantment.OnMonsterSlay(monster, this, who, killedByBomb);
    }
    who?.leftRing.Value?.onMonsterSlay(monster, this, who);
    who?.rightRing.Value?.onMonsterSlay(monster, this, who);
    if (who != null && !isHutchSlime && !flag)
    {
      if (who.IsLocalPlayer)
        Game1.stats.monsterKilled(monster.Name);
      else if (Game1.IsMasterGame)
        who.queueMessage((byte) 25, Game1.player, (object) monster.Name);
    }
    if (monster.isHardModeMonster.Value)
    {
      int num = (int) Game1.stats.Increment("hardModeMonstersKilled");
    }
    ++Game1.stats.MonstersKilled;
    this.monsterDrop(monster, monsterBox.Center.X, monsterBox.Center.Y, who);
    if (!isHutchSlime && who != null)
      who.gainExperience(4, this.isFarm.Value ? Math.Max(1, monster.ExperienceGained / 3) : monster.ExperienceGained);
    if (monster.ShouldMonsterBeRemoved())
      this.characters.Remove((NPC) monster);
    this.removeTemporarySpritesWithID((int) ((double) monster.position.X * 777.0 + (double) monster.position.Y * 77777.0));
    if (!(who?.CurrentTool is MeleeWeapon currentTool) || !(currentTool.QualifiedItemId == "(W)65") && (currentTool.appearance.Value == null || !currentTool.appearance.Value.Equals("(W)65")))
      return;
    Utility.addRainbowStarExplosion(this, new Vector2((float) (monsterBox.Center.X - 32 /*0x20*/), (float) (monsterBox.Center.Y - 32 /*0x20*/)), Game1.random.Next(6, 9));
  }

  public void growWeedGrass(int iterations)
  {
    for (int index = 0; index < iterations; ++index)
    {
      foreach (KeyValuePair<Vector2, TerrainFeature> keyValuePair in this.terrainFeatures.Pairs.ToArray<KeyValuePair<Vector2, TerrainFeature>>())
      {
        if (keyValuePair.Value is Grass grass && Game1.random.NextDouble() < 0.65)
        {
          if (grass.numberOfWeeds.Value < 4)
            grass.numberOfWeeds.Value = Math.Max(0, Math.Min(4, grass.numberOfWeeds.Value + Game1.random.Next(3)));
          else if (grass.numberOfWeeds.Value >= 4)
          {
            int x = (int) keyValuePair.Key.X;
            int y = (int) keyValuePair.Key.Y;
            foreach (Vector2 adjacentTileLocations in Utility.getAdjacentTileLocationsArray(keyValuePair.Key))
            {
              if (this.isTileOnMap(x, y) && !this.IsTileBlockedBy(adjacentTileLocations) && this.doesTileHaveProperty((int) adjacentTileLocations.X, (int) adjacentTileLocations.Y, "Diggable", "Back") != null && !this.IsNoSpawnTile(adjacentTileLocations) && Game1.random.NextDouble() < 0.25)
                this.terrainFeatures.Add(adjacentTileLocations, (TerrainFeature) new Grass((int) grass.grassType.Value, Game1.random.Next(1, 3)));
            }
          }
        }
      }
    }
  }

  public bool tryPlaceObject(Vector2 tile, Object o)
  {
    if (!this.CanItemBePlacedHere(tile))
      return false;
    o.initializeLightSource(tile);
    this.objects.Add(tile, o);
    return true;
  }

  public void removeDamageDebris(Monster monster)
  {
    this.debris.RemoveWhere((Func<Debris, bool>) (d => d.toHover != null && d.toHover.Equals((object) monster) && !d.nonSpriteChunkColor.Equals(Color.Yellow) && (double) d.timeSinceDoneBouncing > 900.0));
  }

  public void spawnWeeds(bool weedsOnly)
  {
    LocationData data = this.GetData();
    int num1 = Game1.random.Next(data != null ? data.MinDailyWeeds : 1, (data != null ? data.MaxDailyWeeds : 5) + 1);
    if (Game1.dayOfMonth == 1 && Game1.IsSpring)
      num1 *= data != null ? data.FirstDayWeedMultiplier : 15;
    for (int index = 0; index < num1; ++index)
    {
      int num2 = 0;
      while (num2 < 3)
      {
        int num3 = Game1.random.Next(this.map.DisplayWidth / 64 /*0x40*/);
        int num4 = Game1.random.Next(this.map.DisplayHeight / 64 /*0x40*/);
        Vector2 vector2 = new Vector2((float) num3, (float) num4);
        Object @object;
        this.objects.TryGetValue(vector2, out @object);
        int which = -1;
        int num5 = -1;
        if (Game1.random.NextDouble() < 0.15 + (weedsOnly ? 0.05 : 0.0))
          which = 1;
        else if (!weedsOnly)
        {
          if (Game1.random.NextDouble() < 0.35)
            num5 = 1;
          else if (!this.isFarm.Value && Game1.random.NextDouble() < 0.35)
            num5 = 2;
        }
        if (num5 != -1)
        {
          if (this is Farm && Game1.random.NextDouble() < 0.25)
            return;
        }
        else if (@object == null && this.doesTileHaveProperty(num3, num4, "Diggable", "Back") != null && this.isTileLocationOpen(new Location(num3, num4)) && !this.IsTileOccupiedBy(vector2) && !this.isWaterTile(num3, num4))
        {
          if (!this.IsNoSpawnTile(vector2, "Grass"))
          {
            if (which != -1 && this.GetSeason() != Season.Winter && this.name.Value == "Farm")
            {
              if (Game1.GetFarmTypeID() == "MeadowlandsFarm" && Game1.random.NextDouble() < 0.1)
                which = 7;
              int numberOfWeeds = Game1.random.Next(1, 3);
              this.terrainFeatures.Add(vector2, (TerrainFeature) new Grass(which, numberOfWeeds));
            }
          }
          else
            continue;
        }
        ++num2;
      }
    }
  }

  public virtual void OnMiniJukeboxAdded()
  {
    ++this.miniJukeboxCount.Value;
    this.UpdateMiniJukebox();
  }

  public virtual void OnMiniJukeboxRemoved()
  {
    --this.miniJukeboxCount.Value;
    this.UpdateMiniJukebox();
  }

  public virtual void UpdateMiniJukebox()
  {
    if (this.miniJukeboxCount.Value > 0)
      return;
    this.miniJukeboxCount.Set(0);
    this.miniJukeboxTrack.Set("");
  }

  public virtual bool IsMiniJukeboxPlaying()
  {
    return this.miniJukeboxCount.Value > 0 && this.miniJukeboxTrack.Value != "" && (!this.IsOutdoors || !this.IsRainingHere()) && !Game1.isGreenRain;
  }

  /// <summary>Update the location state when setting up the new day, before the game saves overnight.</summary>
  /// <param name="dayOfMonth">The current day of month.</param>
  /// <remarks>See also <see cref="M:StardewValley.GameLocation.OnDayStarted" />, which happens after saving when the day has started.</remarks>
  public virtual void DayUpdate(int dayOfMonth)
  {
    this.isMusicTownMusic = new bool?();
    this.netAudio.StopPlaying("fuse");
    this.SelectRandomMiniJukeboxTrack();
    this.critters?.Clear();
    this.characters.RemoveWhere((Func<NPC, bool>) (npc =>
    {
      switch (npc)
      {
        case JunimoHarvester _:
          return true;
        case Monster monster2:
          return monster2.wildernessFarmMonster;
        default:
          return false;
      }
    }));
    foreach (FarmAnimal farmAnimal in this.animals.Values.ToArray<FarmAnimal>())
      farmAnimal.dayUpdate(this);
    for (int index = this.debris.Count - 1; index >= 0; --index)
    {
      Debris debri = this.debris[index];
      if (debri.isEssentialItem() && Game1.IsMasterGame)
      {
        if (debri.item?.QualifiedItemId == "(O)73")
        {
          debri.collect(Game1.player);
        }
        else
        {
          Item obj = debri.item;
          debri.item = (Item) null;
          Game1.player.team.returnedDonations.Add(obj);
          Game1.player.team.newLostAndFoundItems.Value = true;
        }
        this.debris.RemoveAt(index);
      }
    }
    this.updateMap();
    this.temporarySprites.Clear();
    KeyValuePair<Vector2, TerrainFeature>[] array = this.terrainFeatures.Pairs.ToArray<KeyValuePair<Vector2, TerrainFeature>>();
    foreach (KeyValuePair<Vector2, TerrainFeature> keyValuePair in array)
    {
      if (!this.isTileOnMap(keyValuePair.Key))
        this.terrainFeatures.Remove(keyValuePair.Key);
      else
        keyValuePair.Value.dayUpdate();
    }
    foreach (KeyValuePair<Vector2, TerrainFeature> keyValuePair in array)
    {
      if (keyValuePair.Value is HoeDirt hoeDirt)
        hoeDirt.updateNeighbors();
    }
    if (this.largeTerrainFeatures != null)
    {
      foreach (TerrainFeature terrainFeature in this.largeTerrainFeatures.ToArray<LargeTerrainFeature>())
        terrainFeature.dayUpdate();
    }
    this.objects.Lock();
    foreach (KeyValuePair<Vector2, Object> pair in this.objects.Pairs)
    {
      pair.Value.DayUpdate();
      if (pair.Value.destroyOvernight)
      {
        pair.Value.performRemoveAction();
        this.objects.Remove(pair.Key);
      }
    }
    this.objects.Unlock();
    this.RespawnStumpsFromMapProperty();
    if (!(this is FarmHouse))
      this.debris.RemoveWhere((Func<Debris, bool>) (d => d.item == null && d.itemId.Value == null));
    if (this.map != null && (this.isOutdoors.Value || this.map.Properties.ContainsKey("ForceSpawnForageables")) && !this.map.Properties.ContainsKey("skipWeedGrowth"))
    {
      if (Game1.dayOfMonth % 7 == 0 && !(this is Farm))
      {
        Microsoft.Xna.Framework.Rectangle rectangle = new Microsoft.Xna.Framework.Rectangle(0, 0, 0, 0);
        if (this is IslandWest)
          rectangle = new Microsoft.Xna.Framework.Rectangle(31 /*0x1F*/, 3, 77, 70);
        foreach (KeyValuePair<Vector2, Object> keyValuePair in this.objects.Pairs.ToArray<KeyValuePair<Vector2, Object>>())
        {
          if (keyValuePair.Value.isSpawnedObject.Value && keyValuePair.Value.SpecialVariable != 724519 && !rectangle.Contains(Utility.Vector2ToPoint(keyValuePair.Key)))
            this.objects.Remove(keyValuePair.Key);
        }
        this.numberOfSpawnedObjectsOnMap = 0;
        this.spawnObjects();
        this.spawnObjects();
      }
      this.spawnObjects();
      if (Game1.dayOfMonth == 1)
        this.spawnObjects();
      if (Game1.stats.DaysPlayed < 4U)
        this.spawnObjects();
      Layer layer = this.map.GetLayer("Paths");
      if (layer != null && !(this is Farm))
      {
        for (int x = 0; x < this.map.Layers[0].LayerWidth; ++x)
        {
          for (int y = 0; y < this.map.Layers[0].LayerHeight; ++y)
          {
            string treeId;
            int? growthStageOnRegrow;
            bool isFruitTree;
            if (this.TryGetTreeIdForTile(layer.Tiles[x, y], out treeId, out int? _, out growthStageOnRegrow, out isFruitTree) && Game1.random.NextBool())
            {
              Vector2 vector2 = new Vector2((float) x, (float) y);
              if (this.GetFurnitureAt(vector2) == null && !this.terrainFeatures.ContainsKey(vector2) && !this.objects.ContainsKey(vector2) && this.getBuildingAt(vector2) == null)
              {
                if (isFruitTree)
                  this.terrainFeatures.Add(vector2, (TerrainFeature) new FruitTree(treeId, growthStageOnRegrow ?? 2));
                else
                  this.terrainFeatures.Add(vector2, (TerrainFeature) new Tree(treeId, growthStageOnRegrow ?? 2));
              }
            }
          }
        }
      }
    }
    Object @object;
    this.terrainFeatures.RemoveWhere((Func<KeyValuePair<Vector2, TerrainFeature>, bool>) (pair => pair.Value is HoeDirt hoeDirt1 && (hoeDirt1.crop == null || hoeDirt1.crop.forageCrop.Value) && (!this.objects.TryGetValue(pair.Key, out @object) || @object == null || !@object.IsSpawnedObject || !@object.isForage()) && Game1.random.NextBool(this.GetDirtDecayChance(pair.Key))));
    this.lightLevel.Value = 0.0f;
    foreach (Furniture furniture in this.furniture)
    {
      furniture.minutesElapsed(Utility.CalculateMinutesUntilMorning(Game1.timeOfDay));
      furniture.DayUpdate();
    }
    this.addLightGlows();
    if (!(this is Farm))
      this.HandleGrassGrowth(dayOfMonth);
    foreach (Building building in this.buildings)
      building.dayUpdate(dayOfMonth);
    foreach (string str in new List<string>((IEnumerable<string>) Game1.netWorldState.Value.Builders.Keys))
    {
      BuilderData builder = Game1.netWorldState.Value.Builders[str];
      if (builder.buildingLocation.Value == this.NameOrUniqueName)
      {
        Building buildingAt = this.getBuildingAt(Utility.PointToVector2(builder.buildingTile.Value));
        if (buildingAt == null || buildingAt.daysUntilUpgrade.Value == 0 && buildingAt.daysOfConstructionLeft.Value == 0)
          Game1.netWorldState.Value.Builders.Remove(str);
        else
          Game1.netWorldState.Value.MarkUnderConstruction(str, buildingAt);
      }
    }
    if (dayOfMonth == 9 && this.Name.Equals("Backwoods"))
    {
      if (this.terrainFeatures.GetValueOrDefault(new Vector2(18f, 18f)) is HoeDirt)
        this.terrainFeatures.Remove(new Vector2(18f, 18f));
      this.tryPlaceObject(new Vector2(18f, 18f), ItemRegistry.Create<Object>("(O)SeedSpot"));
    }
    this.fishSplashPointTime = 0;
    this.fishFrenzyFish.Value = "";
    this.fishSplashPoint.Value = Point.Zero;
    this.orePanPoint.Value = Point.Zero;
  }

  /// <summary>Get the probability that a hoed dirt tile decays overnight, as a value between 0 (never) and 1 (always).</summary>
  /// <param name="tile">The dirt tile position.</param>
  public virtual double GetDirtDecayChance(Vector2 tile)
  {
    double parsed;
    if (this.TryGetMapPropertyAs("DirtDecayChance", out parsed))
      return parsed;
    if (this.IsGreenhouse)
      return 0.0;
    return this is Farm || this is IslandWest || this.isFarm.Value ? 0.1 : 1.0;
  }

  /// <summary>If the location's map has the <c>Stumps</c> map property, respawn any missing stumps. This will destroy any objects placed on the same tile.</summary>
  public void RespawnStumpsFromMapProperty()
  {
    string[] propertySplitBySpaces = this.GetMapPropertySplitBySpaces("Stumps");
    for (int index = 0; index < propertySplitBySpaces.Length; index += 3)
    {
      Vector2 vector2;
      string error;
      if (!ArgUtility.TryGetVector2(propertySplitBySpaces, index, out vector2, out error, name: "Vector2 tile"))
      {
        this.LogMapPropertyError("Stumps", propertySplitBySpaces, error);
      }
      else
      {
        bool flag = false;
        foreach (TerrainFeature resourceClump in this.resourceClumps)
        {
          if (resourceClump.Tile == vector2)
          {
            flag = true;
            break;
          }
        }
        if (!flag)
        {
          this.resourceClumps.Add(new ResourceClump(600, 2, 2, vector2));
          this.removeObject(vector2, false);
          this.removeObject(vector2 + new Vector2(1f, 0.0f), false);
          this.removeObject(vector2 + new Vector2(1f, 1f), false);
          this.removeObject(vector2 + new Vector2(0.0f, 1f), false);
        }
      }
    }
  }

  public void addLightGlows()
  {
    int num1 = Game1.getTrulyDarkTime(this) - 100;
    if (this.isOutdoors.Value || Game1.timeOfDay >= num1 && !Game1.newDay)
      return;
    this.lightGlows.Clear();
    string[] propertySplitBySpaces = this.GetMapPropertySplitBySpaces("DayTiles");
    for (int index = 0; index < propertySplitBySpaces.Length; index += 4)
    {
      string layerId;
      string error;
      Vector2 vector2;
      int num2;
      if (!ArgUtility.TryGet(propertySplitBySpaces, index, out layerId, out error, name: "string layerId") || !ArgUtility.TryGetVector2(propertySplitBySpaces, index + 1, out vector2, out error, name: "Vector2 position") || !ArgUtility.TryGetInt(propertySplitBySpaces, index + 3, out num2, out error, "int tileIndex"))
      {
        this.LogMapPropertyError("DayTiles", propertySplitBySpaces, error);
      }
      else
      {
        Tile tile = this.map.RequireLayer(layerId).Tiles[(int) vector2.X, (int) vector2.Y];
        if (tile != null)
        {
          tile.TileIndex = num2;
          switch (num2)
          {
            case 256 /*0x0100*/:
              this.lightGlows.Add(vector2 * 64f + new Vector2(32f, 64f));
              continue;
            case 257:
              this.lightGlows.Add(vector2 * 64f + new Vector2(32f, -4f));
              continue;
            case 405:
              this.lightGlows.Add(vector2 * 64f + new Vector2(32f, 32f));
              this.lightGlows.Add(vector2 * 64f + new Vector2(96f, 32f));
              continue;
            case 469:
              this.lightGlows.Add(vector2 * 64f + new Vector2(32f, 36f));
              continue;
            case 1224:
              this.lightGlows.Add(vector2 * 64f + new Vector2(32f, 32f));
              continue;
            default:
              continue;
          }
        }
      }
    }
  }

  public NPC isCharacterAtTile(Vector2 tileLocation)
  {
    NPC npc = (NPC) null;
    tileLocation.X = (float) (int) tileLocation.X;
    tileLocation.Y = (float) (int) tileLocation.Y;
    Microsoft.Xna.Framework.Rectangle rectangle = new Microsoft.Xna.Framework.Rectangle((int) tileLocation.X * 64 /*0x40*/, (int) tileLocation.Y * 64 /*0x40*/, 64 /*0x40*/, 64 /*0x40*/);
    if (this.currentEvent == null)
    {
      foreach (NPC character in this.characters)
      {
        if (character.GetBoundingBox().Intersects(rectangle))
          return character;
      }
    }
    else
    {
      foreach (NPC actor in this.currentEvent.actors)
      {
        if (actor.GetBoundingBox().Intersects(rectangle))
          return actor;
      }
    }
    return npc;
  }

  public void ResetCharacterDialogues()
  {
    for (int index = this.characters.Count - 1; index >= 0; --index)
      this.characters[index].resetCurrentDialogue();
  }

  /// <summary>Get the value of a map property, if it's defined.</summary>
  /// <param name="propertyName">The property name to check.</param>
  /// <returns>Returns the map property value, or null if it's not set.</returns>
  public string getMapProperty(string propertyName)
  {
    string propertyValue;
    return !this.TryGetMapProperty(propertyName, out propertyValue) ? (string) null : propertyValue;
  }

  /// <summary>Get the value of a map property, if it's defined.</summary>
  /// <param name="propertyName">The property name to check.</param>
  /// <param name="propertyValue">The map property value, if it's set.</param>
  /// <returns>Returns whether the map property is set.</returns>
  public bool TryGetMapProperty(string propertyName, out string propertyValue)
  {
    Map map = this.Map;
    if (map == null)
    {
      Game1.log.Warn($"Can't read map property '{propertyName}' for location '{this.NameOrUniqueName}' because the map is null.");
      propertyValue = (string) null;
      return false;
    }
    return map.Properties.TryGetValue(propertyName, out propertyValue) && propertyValue != null;
  }

  /// <summary>Get the space-delimited values defined by a map property.</summary>
  /// <param name="propertyName">The property name to read.</param>
  /// <returns>Returns the map property value, or an empty array if it's empty or unset.</returns>
  public string[] GetMapPropertySplitBySpaces(string propertyName)
  {
    string propertyValue;
    return !this.TryGetMapProperty(propertyName, out propertyValue) || propertyValue == null ? LegacyShims.EmptyArray<string>() : ArgUtility.SplitBySpace(propertyValue);
  }

  /// <summary>Get a map property which defines a boolean value.</summary>
  /// <param name="key">The property name to read.</param>
  /// <param name="parsed">The parsed boolean value, if the map property was present and valid.</param>
  /// <param name="required">Whether to log an error if the map property isn't defined.</param>
  public bool TryGetMapPropertyAs(string key, out bool parsed, bool required = false)
  {
    string propertyValue;
    if (!this.TryGetMapProperty(key, out propertyValue))
    {
      if (required)
        this.LogMapPropertyError(key, "", "required map property isn't defined");
      parsed = false;
      return false;
    }
    switch (propertyValue)
    {
      case "T":
      case "t":
        parsed = true;
        return true;
      case "F":
      case "f":
        parsed = false;
        return true;
      default:
        if (bool.TryParse(propertyValue, out parsed))
          return true;
        this.LogMapPropertyError(key, propertyValue, "not a valid boolean value");
        return false;
    }
  }

  /// <summary>Get a map property which defines a space-delimited <see cref="T:System.Double" /> value.</summary>
  /// <param name="key">The property name to read.</param>
  /// <param name="parsed">The parsed value, if the map property was present and valid.</param>
  /// <param name="required">Whether to log an error if the map property isn't defined.</param>
  public bool TryGetMapPropertyAs(string key, out double parsed, bool required = false)
  {
    string propertyValue;
    if (!this.TryGetMapProperty(key, out propertyValue))
    {
      if (required)
        this.LogMapPropertyError(key, "", "required map property isn't defined");
      parsed = 0.0;
      return false;
    }
    if (double.TryParse(propertyValue, out parsed))
      return true;
    this.LogMapPropertyError(key, propertyValue, $"value '{propertyValue}' can't be parsed as a decimal value");
    return false;
  }

  /// <summary>Get a map property which defines a space-delimited <see cref="T:Microsoft.Xna.Framework.Point" /> position.</summary>
  /// <param name="key">The property name to read.</param>
  /// <param name="parsed">The parsed position value, if the map property was present and valid.</param>
  /// <param name="required">Whether to log an error if the map property isn't defined.</param>
  public bool TryGetMapPropertyAs(string key, out Point parsed, bool required = false)
  {
    string[] propertySplitBySpaces = this.GetMapPropertySplitBySpaces(key);
    if (propertySplitBySpaces.Length == 0)
    {
      if (required)
        this.LogMapPropertyError(key, "", "required map property isn't defined");
      parsed = Point.Zero;
      return false;
    }
    string error;
    if (ArgUtility.TryGetPoint(propertySplitBySpaces, 0, out parsed, out error, nameof (parsed)))
      return true;
    this.LogMapPropertyError(key, propertySplitBySpaces, error);
    parsed = Point.Zero;
    return false;
  }

  /// <summary>Get a map property which defines a space-delimited <see cref="T:Microsoft.Xna.Framework.Vector2" /> position.</summary>
  /// <param name="key">The property name to read.</param>
  /// <param name="parsed">The parsed position value, if the map property was present and valid.</param>
  /// <param name="required">Whether to log an error if the map property isn't defined.</param>
  public bool TryGetMapPropertyAs(string key, out Vector2 parsed, bool required = false)
  {
    string[] propertySplitBySpaces = this.GetMapPropertySplitBySpaces(key);
    if (propertySplitBySpaces.Length == 0)
    {
      if (required)
        this.LogMapPropertyError(key, "", "required map property isn't defined");
      parsed = Vector2.Zero;
      return false;
    }
    string error;
    if (ArgUtility.TryGetVector2(propertySplitBySpaces, 0, out parsed, out error, name: nameof (parsed)))
      return true;
    this.LogMapPropertyError(key, propertySplitBySpaces, error);
    parsed = Vector2.Zero;
    return false;
  }

  /// <summary>Get a map property which defines a space-delimited position and size.</summary>
  /// <param name="key">The property name to read.</param>
  /// <param name="parsed">The parsed position value, if the map property was present and valid.</param>
  /// <param name="required">Whether to log an error if the map property isn't defined.</param>
  public bool TryGetMapPropertyAs(string key, out Microsoft.Xna.Framework.Rectangle parsed, bool required = false)
  {
    string[] propertySplitBySpaces = this.GetMapPropertySplitBySpaces(key);
    if (propertySplitBySpaces.Length == 0)
    {
      if (required)
        this.LogMapPropertyError(key, "", "required map property isn't defined");
      parsed = Microsoft.Xna.Framework.Rectangle.Empty;
      return false;
    }
    string error;
    if (ArgUtility.TryGetRectangle(propertySplitBySpaces, 0, out parsed, out error, nameof (parsed)))
      return true;
    this.LogMapPropertyError(key, propertySplitBySpaces, error);
    parsed = Microsoft.Xna.Framework.Rectangle.Empty;
    return false;
  }

  /// <summary>Get whether a map property is defined and has a non-empty value.</summary>
  /// <param name="propertyName">The property name to check.</param>
  public bool HasMapPropertyWithValue(string propertyName)
  {
    string str;
    return this.map != null && this.Map.Properties.TryGetValue(propertyName, out str) && str != null && str.Length > 0;
  }

  public virtual void tryToAddCritters(bool onlyIfOnScreen = false)
  {
    if (Game1.CurrentEvent != null)
      return;
    double num1 = Math.Max(0.15, Math.Min(0.5, (double) (this.map.Layers[0].LayerWidth * this.map.Layers[0].LayerHeight) / 15000.0));
    double chance1 = num1;
    double chance2 = num1;
    double chance3 = num1 / 2.0;
    double chance4 = num1 / 2.0;
    double chance5 = num1 / 8.0;
    double num2 = num1 * 2.0;
    if (this.IsRainingHere())
      return;
    this.addClouds(num2 / (onlyIfOnScreen ? 2.0 : 1.0), onlyIfOnScreen);
    if (this is Beach || this.critters == null || this.critters.Count > (this.IsSummerHere() ? 20 : 10))
      return;
    this.addBirdies(chance1, onlyIfOnScreen);
    this.addButterflies(chance2, onlyIfOnScreen);
    this.addBunnies(chance3, onlyIfOnScreen);
    this.addSquirrels(chance4, onlyIfOnScreen);
    this.addWoodpecker(chance5, onlyIfOnScreen);
    if (Game1.isDarkOut(this) && Game1.random.NextDouble() < 0.01)
      this.addOwl();
    if (!Game1.isDarkOut(this))
      return;
    this.addOpossums(num1 / 10.0, onlyIfOnScreen);
  }

  public void addClouds(double chance, bool onlyIfOnScreen = false)
  {
    if (!this.IsSummerHere() || this.IsRainingHere() || Game1.weatherIcon == 4 || Game1.timeOfDay >= Game1.getStartingToGetDarkTime(this) - 100)
      return;
    while (Game1.random.NextDouble() < Math.Min(0.9, chance))
    {
      Vector2 position = this.getRandomTile();
      if (onlyIfOnScreen)
        position = Game1.random.NextBool() ? new Vector2((float) this.map.Layers[0].LayerWidth, (float) Game1.random.Next(this.map.Layers[0].LayerHeight)) : new Vector2((float) Game1.random.Next(this.map.Layers[0].LayerWidth), (float) this.map.Layers[0].LayerHeight);
      if (onlyIfOnScreen || !Utility.isOnScreen(position * 64f, 1280 /*0x0500*/))
      {
        Cloud c = new Cloud(position);
        bool flag = true;
        if (this.critters != null)
        {
          foreach (Critter critter in this.critters)
          {
            if (critter is Cloud && critter.getBoundingBox(0, 0).Intersects(c.getBoundingBox(0, 0)))
            {
              flag = false;
              break;
            }
          }
        }
        if (flag)
          this.addCritter((Critter) c);
      }
    }
  }

  public void addOwl()
  {
    this.critters.Add((Critter) new Owl(new Vector2((float) Game1.random.Next(64 /*0x40*/, this.map.Layers[0].LayerWidth * 64 /*0x40*/ - 64 /*0x40*/), (float) sbyte.MinValue)));
  }

  public void setFireplace(
    bool on,
    int tileLocationX,
    int tileLocationY,
    bool playSound = true,
    int xOffset = 0,
    int yOffset = 0)
  {
    int id = 944468 + tileLocationX * 1000 + tileLocationY;
    string str = $"{this.NameOrUniqueName}_Fireplace_{tileLocationX}_{tileLocationY}";
    if (on)
    {
      if (this.getTemporarySpriteByID(id) != null)
        return;
      this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(276, 1985, 12, 11), new Vector2((float) tileLocationX, (float) tileLocationY) * 64f + new Vector2(32f, -32f) + new Vector2((float) xOffset, (float) yOffset), false, 0.0f, Color.White)
      {
        interval = 50f,
        totalNumberOfLoops = 99999,
        animationLength = 4,
        lightId = str + "_1",
        id = id,
        lightRadius = 2f,
        scale = 4f,
        layerDepth = (float) (((double) tileLocationY + 1.1000000238418579) * 64.0 / 10000.0)
      });
      this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(276, 1985, 12, 11), new Vector2((float) (tileLocationX + 1), (float) tileLocationY) * 64f + new Vector2(-16f, -32f) + new Vector2((float) xOffset, (float) yOffset), false, 0.0f, Color.White)
      {
        delayBeforeAnimationStart = 10,
        interval = 50f,
        totalNumberOfLoops = 99999,
        animationLength = 4,
        lightId = str + "_2",
        id = id,
        lightRadius = 2f,
        scale = 4f,
        layerDepth = (float) (((double) tileLocationY + 1.1000000238418579) * 64.0 / 10000.0)
      });
      if (playSound && Game1.gameMode != (byte) 6)
        this.localSound("fireball");
      AmbientLocationSounds.addSound(new Vector2((float) tileLocationX, (float) tileLocationY), 1);
    }
    else
    {
      this.removeTemporarySpritesWithID(id);
      Game1.currentLightSources.Remove(str + "_1");
      Game1.currentLightSources.Remove(str + "_2");
      if (playSound)
        this.localSound("fireball");
      AmbientLocationSounds.removeSound(new Vector2((float) tileLocationX, (float) tileLocationY));
    }
  }

  public void addWoodpecker(double chance, bool onlyIfOnScreen = false)
  {
    if (Game1.isStartingToGetDarkOut(this) || onlyIfOnScreen || this is Town || this is Desert || Game1.random.NextDouble() >= chance || this.terrainFeatures.Length <= 0)
      return;
    for (int index = 0; index < 3; ++index)
    {
      Vector2 key;
      TerrainFeature terrainFeature;
      if (Utility.TryGetRandom<Vector2, TerrainFeature, NetRef<TerrainFeature>, SerializableDictionary<Vector2, TerrainFeature>, NetVector2Dictionary<TerrainFeature, NetRef<TerrainFeature>>>((NetDictionary<Vector2, TerrainFeature, NetRef<TerrainFeature>, SerializableDictionary<Vector2, TerrainFeature>, NetVector2Dictionary<TerrainFeature, NetRef<TerrainFeature>>>) this.terrainFeatures, out key, out terrainFeature) && terrainFeature is Tree tree)
      {
        WildTreeData data = tree.GetData();
        if ((data != null ? (data.AllowWoodpeckers ? 1 : 0) : 0) != 0 && tree.growthStage.Value >= 5)
        {
          this.critters.Add((Critter) new Woodpecker(tree, key));
          break;
        }
      }
    }
  }

  public void addSquirrels(double chance, bool onlyIfOnScreen = false)
  {
    if (Game1.isStartingToGetDarkOut(this) || onlyIfOnScreen)
      return;
    switch (this)
    {
      case Farm _:
        break;
      case Town _:
        break;
      case Desert _:
        break;
      default:
        if (Game1.random.NextDouble() >= chance || this.terrainFeatures.Length <= 0)
          break;
        for (int index1 = 0; index1 < 3; ++index1)
        {
          Vector2 key;
          TerrainFeature terrainFeature;
          if (Utility.TryGetRandom<Vector2, TerrainFeature, NetRef<TerrainFeature>, SerializableDictionary<Vector2, TerrainFeature>, NetVector2Dictionary<TerrainFeature, NetRef<TerrainFeature>>>((NetDictionary<Vector2, TerrainFeature, NetRef<TerrainFeature>, SerializableDictionary<Vector2, TerrainFeature>, NetVector2Dictionary<TerrainFeature, NetRef<TerrainFeature>>>) this.terrainFeatures, out key, out terrainFeature) && terrainFeature is Tree tree && tree.growthStage.Value >= 5 && !tree.stump.Value)
          {
            int num = Game1.random.Next(4, 7);
            bool flip = Game1.random.NextBool();
            bool flag = true;
            for (int index2 = 0; index2 < num; ++index2)
            {
              key.X += flip ? 1f : -1f;
              if (!this.CanSpawnCharacterHere(key))
              {
                flag = false;
                break;
              }
            }
            if (flag)
            {
              this.critters.Add((Critter) new Squirrel(key, flip));
              break;
            }
          }
        }
        break;
    }
  }

  public void addBunnies(double chance, bool onlyIfOnScreen = false)
  {
    if (onlyIfOnScreen || this is Farm || this is Desert || Game1.random.NextDouble() >= chance || this.largeTerrainFeatures == null)
      return;
    for (int index1 = 0; index1 < 3; ++index1)
    {
      int index2 = Game1.random.Next(this.largeTerrainFeatures.Count);
      if (this.largeTerrainFeatures.Count > 0 && this.largeTerrainFeatures[index2] is StardewValley.TerrainFeatures.Bush)
      {
        Vector2 tile = this.largeTerrainFeatures[index2].Tile;
        int num = Game1.random.Next(5, 12);
        bool flip = Game1.random.NextBool();
        bool flag = true;
        for (int index3 = 0; index3 < num; ++index3)
        {
          tile.X += flip ? 1f : -1f;
          if (!this.largeTerrainFeatures[index2].getBoundingBox().Intersects(new Microsoft.Xna.Framework.Rectangle((int) tile.X * 64 /*0x40*/, (int) tile.Y * 64 /*0x40*/, 64 /*0x40*/, 64 /*0x40*/)) && !this.CanSpawnCharacterHere(tile))
          {
            flag = false;
            break;
          }
        }
        if (flag)
        {
          this.critters.Add((Critter) new Rabbit(this, tile, flip));
          break;
        }
      }
    }
  }

  public void addOpossums(double chance, bool onlyIfOnScreen = false)
  {
    if (onlyIfOnScreen || this is Farm || this is Desert || Game1.random.NextDouble() >= chance || this.largeTerrainFeatures == null)
      return;
    for (int index1 = 0; index1 < 3; ++index1)
    {
      int index2 = Game1.random.Next(this.largeTerrainFeatures.Count);
      if (this.largeTerrainFeatures.Count > 0 && this.largeTerrainFeatures[index2] is StardewValley.TerrainFeatures.Bush)
      {
        Vector2 vector2 = this.largeTerrainFeatures[index2].Tile;
        int num = Game1.random.Next(5, 12);
        bool flip = (double) Game1.player.Position.X > (this is BusStop ? 704.0 : 64.0);
        bool flag = true;
        for (int index3 = 0; index3 < num; ++index3)
        {
          vector2.X += flip ? 1f : -1f;
          if (!this.largeTerrainFeatures[index2].getBoundingBox().Intersects(new Microsoft.Xna.Framework.Rectangle((int) vector2.X * 64 /*0x40*/, (int) vector2.Y * 64 /*0x40*/, 64 /*0x40*/, 64 /*0x40*/)) && !this.CanSpawnCharacterHere(vector2))
          {
            flag = false;
            break;
          }
        }
        if (flag)
        {
          if (this is BusStop && Game1.random.NextDouble() < 0.5)
            vector2 = new Vector2((double) Game1.player.Tile.X < 26.0 ? 36f : 16f, (float) (23 + Game1.random.Next(2)));
          this.critters.Add((Critter) new Opossum(this, vector2, flip));
          break;
        }
      }
    }
  }

  public void instantiateCrittersList()
  {
    if (this.critters != null)
      return;
    this.critters = new List<Critter>();
  }

  public void addCritter(Critter c) => this.critters?.Add(c);

  public void addButterflies(double chance, bool onlyIfOnScreen = false)
  {
    Season season = this.GetSeason();
    bool islandButterfly = this.InIslandContext();
    bool flag = season == Season.Summer && Game1.isDarkOut(this);
    if (Game1.timeOfDay >= 1500 && !flag && season != Season.Winter)
      return;
    if (season == Season.Spring || season == Season.Summer || season == Season.Winter && Game1.dayOfMonth % 7 == 0 && Game1.isDarkOut(this))
    {
      chance = Math.Min(0.8, chance * 1.5);
      while (Game1.random.NextDouble() < chance)
      {
        Vector2 randomTile = this.getRandomTile();
        if (!onlyIfOnScreen || !Utility.isOnScreen(randomTile * 64f, 64 /*0x40*/))
        {
          if (flag)
            this.critters.Add((Critter) new Firefly(randomTile));
          else
            this.critters.Add((Critter) new Butterfly(this, randomTile, islandButterfly));
          while (Game1.random.NextDouble() < 0.4)
          {
            if (flag)
              this.critters.Add((Critter) new Firefly(randomTile + new Vector2((float) Game1.random.Next(-2, 3), (float) Game1.random.Next(-2, 3))));
            else
              this.critters.Add((Critter) new Butterfly(this, randomTile + new Vector2((float) Game1.random.Next(-2, 3), (float) Game1.random.Next(-2, 3)), islandButterfly));
          }
        }
      }
    }
    if (Game1.timeOfDay >= 1700)
      return;
    this.tryAddPrismaticButterfly();
  }

  public void tryAddPrismaticButterfly()
  {
    if (!Game1.player.hasBuff("statue_of_blessings_6"))
      return;
    foreach (Critter critter in this.critters)
    {
      if (critter is Butterfly butterfly && butterfly.isPrismatic)
        return;
    }
    Random daySaveRandom = Utility.CreateDaySaveRandom((double) (Game1.player.UniqueMultiplayerID % 10000L));
    string[] strArray = new string[7]
    {
      "Forest",
      "Town",
      "Beach",
      "Mountain",
      "Woods",
      "BusStop",
      "Backwoods"
    };
    string str = strArray[daySaveRandom.Next(strArray.Length)];
    if (str.Equals("Beach") && this.Name.Equals("BeachNightMarket"))
      str = "BeachNightMarket";
    if (!this.Name.Equals(str))
      return;
    Vector2 randomTile = this.getRandomTile(daySaveRandom);
    for (int index = 0; index < 32 /*0x20*/ && !this.isTileLocationOpen(randomTile); ++index)
      randomTile = this.getRandomTile(daySaveRandom);
    this.critters.Add((Critter) new Butterfly(this, randomTile, baseFrameOverride: 394, prismatic: true)
    {
      stayInbounds = true
    });
  }

  public void addBirdies(double chance, bool onlyIfOnScreen = false)
  {
    if (Game1.timeOfDay >= 1500)
      return;
    switch (this)
    {
      case Desert _:
        break;
      case Railroad _:
        break;
      case Farm _:
        break;
      default:
        Season season = this.GetSeason();
        if (season == Season.Summer)
          break;
label_17:
        while (Game1.random.NextDouble() < chance)
        {
          int num1 = Game1.random.Next(1, 4);
          bool flag = false;
          int num2 = 0;
          while (true)
          {
            if (!flag && num2 < 5)
            {
              Vector2 randomTile = this.getRandomTile();
              if ((!onlyIfOnScreen || !Utility.isOnScreen(randomTile * 64f, 64 /*0x40*/)) && this.isAreaClear(new Microsoft.Xna.Framework.Rectangle((int) randomTile.X - 2, (int) randomTile.Y - 2, 5, 5)))
              {
                List<Critter> crittersToAdd = new List<Critter>();
                int startingIndex = season == Season.Fall ? 45 : 25;
                if (Game1.random.NextBool() && Game1.MasterPlayer.mailReceived.Contains("Farm_Eternal"))
                  startingIndex = season == Season.Fall ? 135 : 125;
                if (startingIndex == 25 && Game1.random.NextDouble() < 0.05)
                  startingIndex = 165;
                for (int index = 0; index < num1; ++index)
                  crittersToAdd.Add((Critter) new Birdie(-100, -100, startingIndex));
                this.addCrittersStartingAtTile(randomTile, crittersToAdd);
                flag = true;
              }
              ++num2;
            }
            else
              goto label_17;
          }
        }
        break;
    }
  }

  public void addJumperFrog(Vector2 tileLocation)
  {
    this.critters?.Add((Critter) new Frog(tileLocation));
  }

  public void addFrog()
  {
    if (!this.IsRainingHere() || this.IsWinterHere())
      return;
    for (int index1 = 0; index1 < 3; ++index1)
    {
      Vector2 randomTile = this.getRandomTile();
      if (this.isWaterTile((int) randomTile.X, (int) randomTile.Y) && this.isWaterTile((int) randomTile.X, (int) randomTile.Y - 1) && this.doesTileHaveProperty((int) randomTile.X, (int) randomTile.Y, "Passable", "Buildings") == null)
      {
        int num = 10;
        bool forceFlip = Game1.random.NextBool();
        for (int index2 = 0; index2 < num; ++index2)
        {
          randomTile.X += forceFlip ? 1f : -1f;
          if (this.isTileOnMap((int) randomTile.X, (int) randomTile.Y) && !this.isWaterTile((int) randomTile.X, (int) randomTile.Y))
          {
            this.critters.Add((Critter) new Frog(randomTile, true, forceFlip));
            return;
          }
        }
      }
    }
  }

  public void checkForSpecialCharacterIconAtThisTile(Vector2 tileLocation)
  {
    this.currentEvent?.checkForSpecialCharacterIconAtThisTile(tileLocation);
  }

  private void addCrittersStartingAtTile(Vector2 tile, List<Critter> crittersToAdd)
  {
    if (crittersToAdd == null)
      return;
    int num = 0;
    HashSet<Vector2> vector2Set = new HashSet<Vector2>();
    for (; crittersToAdd.Count > 0 && num < 20; ++num)
    {
      if (vector2Set.Contains(tile))
      {
        tile = Utility.getTranslatedVector2(tile, Game1.random.Next(4), 1f);
      }
      else
      {
        if (this.CanItemBePlacedHere(tile))
        {
          Critter critter = crittersToAdd.Last<Critter>();
          critter.position = tile * 64f;
          critter.startingPosition = tile * 64f;
          this.critters.Add(critter);
          crittersToAdd.RemoveAt(crittersToAdd.Count - 1);
        }
        tile = Utility.getTranslatedVector2(tile, Game1.random.Next(4), 1f);
        vector2Set.Add(tile);
      }
    }
  }

  public bool isAreaClear(Microsoft.Xna.Framework.Rectangle area)
  {
    foreach (Vector2 vector in area.GetVectors())
    {
      if (!this.CanItemBePlacedHere(vector))
        return false;
    }
    return true;
  }

  public void performGreenRainUpdate()
  {
    if (!this.IsGreenRainingHere() || !this.IsOutdoors)
      return;
    bool? haveGreenRainSpawns = this.GetData()?.CanHaveGreenRainSpawns;
    if (haveGreenRainSpawns.HasValue && !haveGreenRainSpawns.GetValueOrDefault())
      return;
    Layer layer = this.map.GetLayer("Paths");
    int index1;
    if (layer != null)
    {
      for (int x = 0; x < layer.LayerWidth; ++x)
      {
        for (int y = 0; y < layer.LayerHeight; ++y)
        {
          Tile tile1 = layer.Tiles[x, y];
          if (tile1 != null && tile1.TileIndexProperties.ContainsKey("GreenRain"))
          {
            Vector2 tile2 = new Vector2((float) x, (float) y);
            if (!this.IsTileOccupiedBy(tile2))
            {
              NetVector2Dictionary<TerrainFeature, NetRef<TerrainFeature>> terrainFeatures = this.terrainFeatures;
              Vector2 key = tile2;
              Tree tree;
              if (!(this is Forest))
              {
                index1 = 10 + (Game1.random.NextBool(0.1) ? 2 : Game1.random.Choose<int>(1, 0));
                tree = new Tree(index1.ToString(), 5, true);
              }
              else
                tree = new Tree("12", 5, true);
              terrainFeatures.Add(key, (TerrainFeature) tree);
            }
          }
        }
      }
    }
    if (this is Town)
      return;
    string[] propertySplitBySpaces = this.GetMapPropertySplitBySpaces("Trees");
    for (int index2 = 0; index2 < propertySplitBySpaces.Length; index2 += 3)
    {
      Vector2 tile;
      string error;
      int num;
      if (!ArgUtility.TryGetVector2(propertySplitBySpaces, index2, out tile, out error, name: "Vector2 position") || !ArgUtility.TryGetInt(propertySplitBySpaces, index2 + 2, out num, out error, "int treeType"))
      {
        this.LogMapPropertyError("Trees", propertySplitBySpaces, error);
      }
      else
      {
        float chance = this.IsFarm ? 0.5f : 1f;
        if (Game1.random.NextBool(chance) && !this.IsTileOccupiedBy(tile))
        {
          NetVector2Dictionary<TerrainFeature, NetRef<TerrainFeature>> terrainFeatures = this.terrainFeatures;
          Vector2 key = tile;
          index1 = num + 1;
          Tree tree = new Tree(index1.ToString(), 5);
          terrainFeatures.Add(key, (TerrainFeature) tree);
        }
      }
    }
    TerrainFeature[] array = this.terrainFeatures.Values.ToArray<TerrainFeature>();
    for (index1 = 0; index1 < array.Length; ++index1)
    {
      if (array[index1] is Tree tree)
        tree.onGreenRainDay();
    }
    int num1 = this.map.Layers[0].LayerWidth * this.map.Layers[0].LayerHeight;
    this.spawnWeedsAndStones(num1 / 16 /*0x10*/, true, false);
    this.spawnWeedsAndStones(num1 / 8, true);
    for (int index3 = 0; index3 < num1 / 4; ++index3)
    {
      Vector2 randomTile = this.getRandomTile();
      Object object1;
      Object object2;
      Object object3;
      Object object4;
      if (this.objects.TryGetValue(randomTile, out object1) && object1.IsWeeds() && this.objects.TryGetValue(randomTile + new Vector2(1f, 0.0f), out object2) && object2.IsWeeds() && this.objects.TryGetValue(randomTile + new Vector2(1f, 1f), out object3) && object3.IsWeeds() && this.objects.TryGetValue(randomTile + new Vector2(0.0f, 1f), out object4) && object4.IsWeeds())
      {
        this.objects.Remove(randomTile);
        this.objects.Remove(randomTile + new Vector2(1f, 0.0f));
        this.objects.Remove(randomTile + new Vector2(1f, 1f));
        this.objects.Remove(randomTile + new Vector2(0.0f, 1f));
        this.resourceClumps.Add(new ResourceClump(44 + Game1.random.Choose<int>(2, 0), 2, 2, randomTile, new int?(4), "TileSheets\\Objects_2"));
      }
    }
  }

  public void performDayAfterGreenRainUpdate()
  {
    foreach (KeyValuePair<Vector2, Object> keyValuePair in this.objects.Pairs.ToArray<KeyValuePair<Vector2, Object>>())
    {
      if (keyValuePair.Value.Name.Contains("GreenRainWeeds"))
        this.objects.Remove(keyValuePair.Key);
    }
    this.resourceClumps.RemoveWhere((Func<ResourceClump, bool>) (clump => clump.IsGreenRainBush()));
    foreach (KeyValuePair<Vector2, TerrainFeature> keyValuePair in this.terrainFeatures.Pairs.ToArray<KeyValuePair<Vector2, TerrainFeature>>())
    {
      if (keyValuePair.Value is Tree tree)
      {
        if (this is Town)
        {
          if (tree.isTemporaryGreenRainTree.Value)
            this.terrainFeatures.Remove(keyValuePair.Key);
        }
        else
          tree.onGreenRainDay(true);
      }
    }
  }

  public Vector2 getRandomTile(Random r = null)
  {
    if (r == null)
      r = Game1.random;
    return new Vector2((float) r.Next(this.Map.Layers[0].LayerWidth), (float) r.Next(this.Map.Layers[0].LayerHeight));
  }

  public void setUpLocationSpecificFlair()
  {
    this.indoorLightingColor = new Color(100, 120, 30);
    this.indoorLightingNightColor = new Color(150, 150, 30);
    Color color1;
    if (this.TryGetAmbientLightFromMap(out color1))
    {
      if (color1 == Color.White)
        color1 = Color.Black;
      this.indoorLightingColor = color1;
      Color color2;
      this.indoorLightingNightColor = !this.TryGetAmbientLightFromMap(out color2, "AmbientNightLight") ? this.indoorLightingColor : color2;
    }
    if (!this.isOutdoors.Value)
    {
      switch (this)
      {
        case FarmHouse _:
        case IslandFarmHouse _:
          break;
        default:
          Game1.ambientLight = this.indoorLightingColor;
          break;
      }
    }
    Game1.screenGlow = false;
    if (!this.IsOutdoors && this.IsGreenRainingHere() && !this.InIslandContext() && this.IsRainingHere())
    {
      this.indoorLightingColor = new Color(123, 0, 96 /*0x60*/);
      this.indoorLightingNightColor = new Color(185, 40, 119);
      Game1.screenGlowOnce(new Color(0, (int) byte.MaxValue, 50) * 0.5f, true, 1f);
    }
    string str1 = this.name.Value;
    if (str1 == null)
      return;
    switch (str1.Length)
    {
      case 6:
        switch (str1[1])
        {
          case 'a':
            if (!(str1 == "Saloon"))
              return;
            if (Game1.timeOfDay >= 1700 || this.IsGreenRainingHere())
              this.setFireplace(true, 22, 17, false);
            if (Game1.random.NextDouble() < 0.25)
            {
              NPC characterFromName = Game1.getCharacterFromName("Gus");
              if (characterFromName != null && characterFromName.TilePoint.Y == 18 && characterFromName.currentLocation == this)
              {
                string str2;
                switch (Game1.random.Next(5))
                {
                  case 0:
                    str2 = "Greeting";
                    break;
                  case 1:
                    str2 = this.IsSummerHere() ? "Summer" : "NotSummer";
                    break;
                  case 2:
                    str2 = this.IsSnowingHere() ? "Snowing1" : "NotSnowing1";
                    break;
                  case 3:
                    str2 = this.IsRainingHere() ? "Raining" : "NotRaining";
                    break;
                  default:
                    str2 = this.IsSnowingHere() ? "Snowing2" : "NotSnowing2";
                    break;
                }
                if (Game1.random.NextDouble() < 0.001)
                  str2 = "RareGreeting";
                characterFromName.showTextAboveHead(Game1.content.LoadString("Strings\\SpeechBubbles:Saloon_Gus_" + str2));
              }
            }
            if (this.getCharacterFromName("Gus") == null && Game1.IsVisitingIslandToday("Gus"))
              this.temporarySprites.Add(new TemporaryAnimatedSprite()
              {
                texture = Game1.mouseCursors2,
                sourceRect = new Microsoft.Xna.Framework.Rectangle(129, 210, 13, 16 /*0x10*/),
                animationLength = 1,
                sourceRectStartingPos = new Vector2(129f, 210f),
                interval = 50000f,
                totalNumberOfLoops = 9999,
                position = new Vector2(11f, 18f) * 64f + new Vector2(3f, 0.0f) * 4f,
                scale = 4f,
                layerDepth = 0.1281f,
                id = 777
              });
            if (Game1.dayOfMonth % 7 != 0 || !NetWorldState.checkAnywhereForWorldStateID("saloonSportsRoom") || Game1.timeOfDay >= 1500)
              return;
            Texture2D texture2D = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\temporary_sprites_1");
            this.TemporarySprites.Add(new TemporaryAnimatedSprite()
            {
              texture = texture2D,
              sourceRect = new Microsoft.Xna.Framework.Rectangle(368, 336, 19, 14),
              animationLength = 7,
              sourceRectStartingPos = new Vector2(368f, 336f),
              interval = 5000f,
              totalNumberOfLoops = 99999,
              position = new Vector2(34f, 3f) * 64f + new Vector2(7f, 13f) * 4f,
              scale = 4f,
              layerDepth = 0.0401f,
              id = 2400
            });
            return;
          case 'u':
            if (!(str1 == "Summit"))
              return;
            Game1.ambientLight = Color.Black;
            return;
          default:
            return;
        }
      case 7:
        switch (str1[0])
        {
          case 'B':
            if (!(str1 == "BugLand"))
              return;
            if (!Game1.player.hasDarkTalisman && this.CanItemBePlacedHere(new Vector2(31f, 5f)))
              this.overlayObjects.Add(new Vector2(31f, 5f), (Object) new Chest(new List<Item>()
              {
                (Item) new SpecialItem(6)
              }, new Vector2(31f, 5f))
              {
                Tint = Color.Gray
              });
            using (List<NPC>.Enumerator enumerator = this.characters.GetEnumerator())
            {
              while (enumerator.MoveNext())
              {
                NPC current = enumerator.Current;
                if (!(current is Grub grub))
                {
                  if (current is Fly fly)
                    fly.setHard();
                }
                else
                  grub.setHard();
              }
              return;
            }
          case 'S':
            if (!(str1 == "Sunroom"))
              return;
            this.indoorLightingColor = new Color(0, 0, 0);
            AmbientLocationSounds.addSound(new Vector2(3f, 4f), 0);
            if (this.largeTerrainFeatures.Count == 0)
            {
              StardewValley.TerrainFeatures.Bush bush = new StardewValley.TerrainFeatures.Bush(new Vector2(6f, 7f), 3, this, -999);
              bush.loadSprite();
              bush.health = 99f;
              this.largeTerrainFeatures.Add((LargeTerrainFeature) bush);
            }
            if (this.IsRainingHere())
              return;
            this.critters = new List<Critter>();
            this.critters.Add((Critter) new Butterfly(this, this.getRandomTile()).setStayInbounds(true));
            while (Game1.random.NextBool())
              this.critters.Add((Critter) new Butterfly(this, this.getRandomTile()).setStayInbounds(true));
            return;
          default:
            return;
        }
      case 8:
        switch (str1[0])
        {
          case 'H':
            if (!(str1 == "Hospital"))
              return;
            this.indoorLightingColor = new Color(100, 100, 60);
            if (!Game1.random.NextBool())
              return;
            NPC characterFromName1 = Game1.getCharacterFromName("Maru");
            if (characterFromName1 == null || characterFromName1.currentLocation != this || characterFromName1.isDivorcedFrom(Game1.player))
              return;
            string path1;
            switch (Game1.random.Next(5))
            {
              case 0:
                path1 = "Strings\\SpeechBubbles:Hospital_Maru_Greeting1";
                break;
              case 1:
                path1 = "Strings\\SpeechBubbles:Hospital_Maru_Greeting2";
                break;
              case 2:
                path1 = "Strings\\SpeechBubbles:Hospital_Maru_Greeting3";
                break;
              case 3:
                path1 = "Strings\\SpeechBubbles:Hospital_Maru_Greeting4";
                break;
              default:
                path1 = "Strings\\SpeechBubbles:Hospital_Maru_Greeting5";
                break;
            }
            if (Game1.player.spouse == "Maru")
            {
              string path2 = "Strings\\SpeechBubbles:Hospital_Maru_Spouse";
              characterFromName1.showTextAboveHead(Game1.content.LoadString(path2), new Color?(SpriteText.color_Red));
              return;
            }
            characterFromName1.showTextAboveHead(Game1.content.LoadString(path1));
            return;
          case 'J':
            if (!(str1 == "JojaMart"))
              return;
            this.indoorLightingColor = new Color(0, 0, 0);
            if (!Game1.random.NextBool())
              return;
            NPC characterFromName2 = Game1.getCharacterFromName("Morris");
            if (characterFromName2 == null || characterFromName2.currentLocation != this)
              return;
            string path3 = "Strings\\SpeechBubbles:JojaMart_Morris_Greeting";
            characterFromName2.showTextAboveHead(Game1.content.LoadString(path3));
            return;
          case 'S':
            if (!(str1 == "SeedShop"))
              return;
            this.setFireplace(true, 25, 13, false);
            if (Game1.random.NextBool() && Game1.player.TilePoint.Y > 10)
            {
              NPC characterFromName3 = Game1.getCharacterFromName("Pierre");
              if (characterFromName3 != null && characterFromName3.TilePoint.Y == 17 && characterFromName3.currentLocation == this)
              {
                string str3;
                switch (Game1.random.Next(5))
                {
                  case 0:
                    str3 = this.IsWinterHere() ? "Winter" : "NotWinter";
                    break;
                  case 1:
                    str3 = this.IsSummerHere() ? "Summer" : "NotSummer";
                    break;
                  case 2:
                    str3 = "Greeting1";
                    break;
                  case 3:
                    str3 = "Greeting2";
                    break;
                  default:
                    str3 = this.IsRainingHere() ? "Raining" : "NotRaining";
                    break;
                }
                if (Game1.random.NextDouble() < 0.001)
                  str3 = "RareGreeting";
                string format = Game1.content.LoadString("Strings\\SpeechBubbles:SeedShop_Pierre_" + str3);
                characterFromName3.showTextAboveHead(string.Format(format, (object) Game1.player.Name));
              }
            }
            if (this.getCharacterFromName("Pierre") == null && Game1.IsVisitingIslandToday("Pierre"))
              this.temporarySprites.Add(new TemporaryAnimatedSprite()
              {
                texture = Game1.mouseCursors2,
                sourceRect = new Microsoft.Xna.Framework.Rectangle(129, 210, 13, 16 /*0x10*/),
                animationLength = 1,
                sourceRectStartingPos = new Vector2(129f, 210f),
                interval = 50000f,
                totalNumberOfLoops = 9999,
                position = new Vector2(5f, 17f) * 64f + new Vector2(3f, 0.0f) * 4f,
                scale = 4f,
                layerDepth = 0.1217f,
                id = 777
              });
            if (this.getCharacterFromName("Abigail") == null || !this.getCharacterFromName("Abigail").TilePoint.Equals(new Point(3, 6)))
              return;
            this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(167, 1714, 19, 14), 100f, 3, 999999, new Vector2(2f, 3f) * 64f + new Vector2(7f, 12f) * 4f, false, false, 0.0002f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
            {
              id = 688
            });
            return;
          case 'W':
            if (!(str1 == "WitchHut") || !Game1.player.mailReceived.Contains("cursed_doll") || this.farmers.Any())
              return;
            this.characters.Clear();
            uint num = Game1.stats.Get("childrenTurnedToDoves");
            this.addCharacter((NPC) new Bat(new Vector2(7f, 6f) * 64f, -666));
            if (num > 1U)
              this.addCharacter((NPC) new Bat(new Vector2(4f, 7f) * 64f, -666));
            if (num > 2U)
              this.addCharacter((NPC) new Bat(new Vector2(10f, 7f) * 64f, -666));
            for (int index = 4; (long) index <= (long) num; ++index)
              this.addCharacter((NPC) new Bat(Utility.getRandomPositionInThisRectangle(new Microsoft.Xna.Framework.Rectangle(1, 4, 13, 4), Game1.random) * 64f + new Vector2((float) Game1.random.Next(-32, 32 /*0x20*/), (float) Game1.random.Next(-32, 32 /*0x20*/)), -666));
            return;
          default:
            return;
        }
      case 9:
        switch (str1[0])
        {
          case 'J':
            if (!(str1 == "JoshHouse") || !Game1.isGreenRain)
              return;
            this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors_1_6", new Microsoft.Xna.Framework.Rectangle(386, 334, 36, 28), 40f, 3, 999999, new Vector2(246.5f, 317f) * 4f, false, false, 0.136001f, 0.0f, Color.White, 2f, 0.0f, 0.0f, 0.0f));
            return;
          case 'L':
            if (!(str1 == "LeahHouse"))
              return;
            NPC characterFromName4 = Game1.getCharacterFromName("Leah");
            if (this.IsFallHere() || this.IsWinterHere() || this.IsRainingHere())
              this.setFireplace(true, 11, 4, false);
            if (characterFromName4 == null || characterFromName4.currentLocation != this || characterFromName4.isDivorcedFrom(Game1.player))
              return;
            string path4;
            switch (Game1.random.Next(3))
            {
              case 0:
                path4 = "Strings\\SpeechBubbles:LeahHouse_Leah_Greeting1";
                break;
              case 1:
                path4 = "Strings\\SpeechBubbles:LeahHouse_Leah_Greeting2";
                break;
              default:
                path4 = "Strings\\SpeechBubbles:LeahHouse_Leah_Greeting3";
                break;
            }
            characterFromName4.faceTowardFarmerForPeriod(3000, 15, false, Game1.player);
            characterFromName4.showTextAboveHead(Game1.content.LoadString(path4, (object) Game1.player.Name));
            return;
          case 'Q':
            if (!(str1 == "QiNutRoom"))
              return;
            Game1.ambientLight = this.indoorLightingColor;
            return;
          default:
            return;
        }
      case 10:
        switch (str1[0])
        {
          case 'A':
            if (!(str1 == "AnimalShop"))
              return;
            this.setFireplace(true, 3, 14, false);
            if (Game1.random.NextBool())
            {
              NPC characterFromName5 = Game1.getCharacterFromName("Marnie");
              if (characterFromName5 != null && characterFromName5.TilePoint.Y == 14)
              {
                string path5;
                switch (Game1.random.Next(5))
                {
                  case 0:
                    path5 = "Strings\\SpeechBubbles:AnimalShop_Marnie_Greeting1";
                    break;
                  case 1:
                    path5 = "Strings\\SpeechBubbles:AnimalShop_Marnie_Greeting2";
                    break;
                  case 2:
                    path5 = Game1.player.getFriendshipHeartLevelForNPC("Marnie") > 4 ? "Strings\\SpeechBubbles:AnimalShop_Marnie_CloseFriends" : "Strings\\SpeechBubbles:AnimalShop_Marnie_NotCloseFriends";
                    break;
                  case 3:
                    path5 = this.IsRainingHere() ? "Strings\\SpeechBubbles:AnimalShop_Marnie_Raining" : "Strings\\SpeechBubbles:AnimalShop_Marnie_NotRaining";
                    break;
                  default:
                    path5 = "Strings\\SpeechBubbles:AnimalShop_Marnie_Greeting3";
                    break;
                }
                if (Game1.random.NextDouble() < 0.001)
                  path5 = "Strings\\SpeechBubbles:AnimalShop_Marnie_RareGreeting";
                characterFromName5.showTextAboveHead(Game1.content.LoadString(path5, (object) Game1.player.Name, (object) Game1.player.farmName));
              }
            }
            if (this.getCharacterFromName("Marnie") == null && Game1.IsVisitingIslandToday("Marnie"))
              this.temporarySprites.Add(new TemporaryAnimatedSprite()
              {
                texture = Game1.mouseCursors2,
                sourceRect = new Microsoft.Xna.Framework.Rectangle(129, 210, 13, 16 /*0x10*/),
                animationLength = 1,
                sourceRectStartingPos = new Vector2(129f, 210f),
                interval = 50000f,
                totalNumberOfLoops = 9999,
                position = new Vector2(13f, 14f) * 64f + new Vector2(3f, 0.0f) * 4f,
                scale = 4f,
                layerDepth = 0.1025f,
                id = 777
              });
            if (Game1.netWorldState.Value.hasWorldStateID("m_painting0"))
            {
              this.temporarySprites.Add(new TemporaryAnimatedSprite()
              {
                texture = Game1.mouseCursors,
                sourceRect = new Microsoft.Xna.Framework.Rectangle(25, 1925, 25, 23),
                animationLength = 1,
                sourceRectStartingPos = new Vector2(25f, 1925f),
                interval = 5000f,
                totalNumberOfLoops = 9999,
                position = new Vector2(16f, 1f) * 64f + new Vector2(3f, 1f) * 4f,
                scale = 4f,
                layerDepth = 0.1f,
                id = 777
              });
              return;
            }
            if (Game1.netWorldState.Value.hasWorldStateID("m_painting1"))
            {
              this.temporarySprites.Add(new TemporaryAnimatedSprite()
              {
                texture = Game1.mouseCursors,
                sourceRect = new Microsoft.Xna.Framework.Rectangle(0, 1925, 25, 23),
                animationLength = 1,
                sourceRectStartingPos = new Vector2(0.0f, 1925f),
                interval = 5000f,
                totalNumberOfLoops = 9999,
                position = new Vector2(16f, 1f) * 64f + new Vector2(3f, 1f) * 4f,
                scale = 4f,
                layerDepth = 0.1f,
                id = 777
              });
              return;
            }
            if (!Game1.netWorldState.Value.hasWorldStateID("m_painting2"))
              return;
            this.temporarySprites.Add(new TemporaryAnimatedSprite()
            {
              texture = Game1.mouseCursors,
              sourceRect = new Microsoft.Xna.Framework.Rectangle(0, 1948, 25, 24),
              animationLength = 1,
              sourceRectStartingPos = new Vector2(0.0f, 1948f),
              interval = 5000f,
              totalNumberOfLoops = 9999,
              position = new Vector2(16f, 1f) * 64f + new Vector2(3f, 1f) * 4f,
              scale = 4f,
              layerDepth = 0.1f,
              id = 777
            });
            return;
          case 'B':
            if (!(str1 == "Blacksmith"))
              return;
            AmbientLocationSounds.addSound(new Vector2(9f, 10f), 2);
            AmbientLocationSounds.changeSpecificVariable("Frequency", 2f, 2);
            return;
          case 'G':
            if (!(str1 == "Greenhouse") || !Game1.isDarkOut(this))
              return;
            Game1.ambientLight = Game1.outdoorLight;
            return;
          case 'H':
            if (!(str1 == "HaleyHouse") || !Game1.player.eventsSeen.Contains("463391") || !(Game1.player.spouse != "Emily"))
              return;
            this.temporarySprites.Add((TemporaryAnimatedSprite) new EmilysParrot(new Vector2(912f, 160f)));
            return;
          case 'M':
            if (!(str1 == "ManorHouse"))
              return;
            this.indoorLightingColor = new Color(150, 120, 50);
            NPC characterFromName6 = Game1.getCharacterFromName("Lewis");
            if (characterFromName6 == null || characterFromName6.currentLocation != this)
              return;
            string str4 = Game1.timeOfDay < 1200 ? "Morning" : (Game1.timeOfDay < 1700 ? "Afternoon" : "Evening");
            characterFromName6.faceTowardFarmerForPeriod(3000, 15, false, Game1.player);
            characterFromName6.showTextAboveHead(Game1.content.LoadString("Strings\\SpeechBubbles:ManorHouse_Lewis_" + str4));
            return;
          case 'S':
            if (!(str1 == "SandyHouse"))
              return;
            this.indoorLightingColor = new Color(0, 0, 0);
            if (!Game1.random.NextBool())
              return;
            NPC characterFromName7 = Game1.getCharacterFromName("Sandy");
            if (characterFromName7 == null || characterFromName7.currentLocation != this)
              return;
            string path6;
            switch (Game1.random.Next(5))
            {
              case 0:
                path6 = "Strings\\SpeechBubbles:SandyHouse_Sandy_Greeting1";
                break;
              case 1:
                path6 = "Strings\\SpeechBubbles:SandyHouse_Sandy_Greeting2";
                break;
              case 2:
                path6 = "Strings\\SpeechBubbles:SandyHouse_Sandy_Greeting3";
                break;
              case 3:
                path6 = "Strings\\SpeechBubbles:SandyHouse_Sandy_Greeting4";
                break;
              default:
                path6 = "Strings\\SpeechBubbles:SandyHouse_Sandy_Greeting5";
                break;
            }
            characterFromName7.showTextAboveHead(Game1.content.LoadString(path6));
            return;
          default:
            return;
        }
      case 12:
        switch (str1[0])
        {
          case 'E':
            if (!(str1 == "ElliottHouse"))
              return;
            NPC characterFromName8 = Game1.getCharacterFromName("Elliott");
            if (characterFromName8 == null || characterFromName8.currentLocation != this || characterFromName8.isDivorcedFrom(Game1.player))
              return;
            string path7;
            switch (Game1.random.Next(3))
            {
              case 0:
                path7 = "Strings\\SpeechBubbles:ElliottHouse_Elliott_Greeting1";
                break;
              case 1:
                path7 = "Strings\\SpeechBubbles:ElliottHouse_Elliott_Greeting2";
                break;
              default:
                path7 = "Strings\\SpeechBubbles:ElliottHouse_Elliott_Greeting3";
                break;
            }
            characterFromName8.faceTowardFarmerForPeriod(3000, 15, false, Game1.player);
            characterFromName8.showTextAboveHead(Game1.content.LoadString(path7, (object) Game1.player.Name));
            return;
          case 'L':
            if (!(str1 == "LeoTreeHouse"))
              return;
            TemporaryAnimatedSpriteList temporarySprites = this.temporarySprites;
            EmilysParrot emilysParrot = new EmilysParrot(new Vector2(88f, 224f));
            emilysParrot.layerDepth = 1f;
            emilysParrot.id = 5858585;
            temporarySprites.Add((TemporaryAnimatedSprite) emilysParrot);
            this.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\critters", new Microsoft.Xna.Framework.Rectangle(71, 334, 12, 11), new Vector2(304f, 32f), false, 0.0f, Color.White)
            {
              layerDepth = 1f / 1000f,
              interval = 700f,
              animationLength = 3,
              totalNumberOfLoops = 999999,
              scale = 4f
            });
            this.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\critters", new Microsoft.Xna.Framework.Rectangle(47, 334, 12, 11), new Vector2(112f, -25.6f), true, 0.0f, Color.White)
            {
              layerDepth = 1f / 1000f,
              interval = 300f,
              animationLength = 3,
              totalNumberOfLoops = 999999,
              scale = 4f
            });
            this.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\critters", new Microsoft.Xna.Framework.Rectangle(71, 334, 12, 11), new Vector2(224f, -25.6f), false, 0.0f, Color.White)
            {
              layerDepth = 1f / 1000f,
              interval = 800f,
              animationLength = 3,
              totalNumberOfLoops = 999999,
              scale = 4f
            });
            return;
          case 'S':
            if (!(str1 == "ScienceHouse"))
              return;
            if (Game1.random.NextBool() && Game1.player.currentLocation != null && Game1.player.currentLocation.isOutdoors.Value)
            {
              NPC characterFromName9 = Game1.getCharacterFromName("Robin");
              if (characterFromName9 != null && characterFromName9.TilePoint.Y == 18)
              {
                string path8;
                switch (Game1.random.Next(5))
                {
                  case 0:
                    path8 = this.IsRainingHere() ? "Strings\\SpeechBubbles:ScienceHouse_Robin_Raining1" : "Strings\\SpeechBubbles:ScienceHouse_Robin_NotRaining1";
                    break;
                  case 1:
                    path8 = this.IsSnowingHere() ? "Strings\\SpeechBubbles:ScienceHouse_Robin_Snowing" : "Strings\\SpeechBubbles:ScienceHouse_Robin_NotSnowing";
                    break;
                  case 2:
                    path8 = Game1.player.getFriendshipHeartLevelForNPC("Robin") > 4 ? "Strings\\SpeechBubbles:ScienceHouse_Robin_CloseFriends" : "Strings\\SpeechBubbles:ScienceHouse_Robin_NotCloseFriends";
                    break;
                  case 3:
                    path8 = this.IsRainingHere() ? "Strings\\SpeechBubbles:ScienceHouse_Robin_Raining2" : "Strings\\SpeechBubbles:ScienceHouse_Robin_NotRaining2";
                    break;
                  default:
                    path8 = "Strings\\SpeechBubbles:ScienceHouse_Robin_Greeting";
                    break;
                }
                if (Game1.random.NextDouble() < 0.001)
                  path8 = "Strings\\SpeechBubbles:ScienceHouse_Robin_RareGreeting";
                characterFromName9.showTextAboveHead(Game1.content.LoadString(path8, (object) Game1.player.Name));
              }
            }
            if (this.getCharacterFromName("Robin") != null || !Game1.IsVisitingIslandToday("Robin"))
              return;
            this.temporarySprites.Add(new TemporaryAnimatedSprite()
            {
              texture = Game1.mouseCursors2,
              sourceRect = new Microsoft.Xna.Framework.Rectangle(129, 210, 13, 16 /*0x10*/),
              animationLength = 1,
              sourceRectStartingPos = new Vector2(129f, 210f),
              interval = 50000f,
              totalNumberOfLoops = 9999,
              position = new Vector2(7f, 18f) * 64f + new Vector2(3f, 0.0f) * 4f,
              scale = 4f,
              layerDepth = 0.1281f,
              id = 777
            });
            return;
          default:
            return;
        }
      case 13:
        if (!(str1 == "LewisBasement"))
          break;
        if (this.farmers.Count == 0)
          this.characters.Clear();
        Vector2 key = new Vector2(17f, 15f);
        this.overlayObjects.Remove(key);
        Object @object = ItemRegistry.Create<Object>("(O)789");
        @object.questItem.Value = true;
        @object.TileLocation = key;
        @object.IsSpawnedObject = true;
        this.overlayObjects.Add(key, @object);
        break;
      case 14:
        if (!(str1 == "AdventureGuild"))
          break;
        this.setFireplace(true, 9, 11, false);
        if (!Game1.random.NextBool())
          break;
        NPC characterFromName10 = Game1.getCharacterFromName("Marlon");
        if (characterFromName10 == null)
          break;
        string path9;
        switch (Game1.random.Next(5))
        {
          case 0:
            path9 = "Strings\\SpeechBubbles:AdventureGuild_Marlon_Greeting_" + (Game1.player.IsMale ? "Male" : "Female");
            break;
          case 1:
            path9 = "Strings\\SpeechBubbles:AdventureGuild_Marlon_Greeting1";
            break;
          case 2:
            path9 = "Strings\\SpeechBubbles:AdventureGuild_Marlon_Greeting2";
            break;
          case 3:
            path9 = "Strings\\SpeechBubbles:AdventureGuild_Marlon_Greeting3";
            break;
          default:
            path9 = "Strings\\SpeechBubbles:AdventureGuild_Marlon_Greeting4";
            break;
        }
        characterFromName10.showTextAboveHead(Game1.content.LoadString(path9));
        break;
      case 15:
        if (!(str1 == "CommunityCenter") || !(this is CommunityCenter) || !Game1.isLocationAccessible("CommunityCenter") && !(this.currentEvent?.id == "191393"))
          break;
        this.setFireplace(true, 31 /*0x1F*/, 8, false);
        this.setFireplace(true, 32 /*0x20*/, 8, false);
        this.setFireplace(true, 33, 8, false);
        break;
      case 16 /*0x10*/:
        if (!(str1 == "ArchaeologyHouse"))
          break;
        this.setFireplace(true, 43, 4, false);
        if (!Game1.random.NextBool() || !Game1.player.hasOrWillReceiveMail("artifactFound"))
          break;
        NPC characterFromName11 = Game1.getCharacterFromName("Gunther");
        if (characterFromName11 == null || characterFromName11.currentLocation != this)
          break;
        string str5;
        switch (Game1.random.Next(5))
        {
          case 0:
            str5 = "Greeting1";
            break;
          case 1:
            str5 = "Greeting2";
            break;
          case 2:
            str5 = "Greeting3";
            break;
          case 3:
            str5 = "Greeting4";
            break;
          default:
            str5 = "Greeting5";
            break;
        }
        if (Game1.random.NextDouble() < 0.001)
          str5 = "RareGreeting";
        characterFromName11.showTextAboveHead(Game1.content.LoadString("Strings\\SpeechBubbles:ArchaeologyHouse_Gunther_" + str5));
        break;
      case 17:
        if (!(str1 == "AbandonedJojaMart") || Game1.MasterPlayer.hasOrWillReceiveMail("ccMovieTheater"))
          break;
        Point point = new Point(8, 8);
        Game1.currentLightSources.Add(new LightSource("AbandonedJojaMart", 4, new Vector2((float) (point.X * 64 /*0x40*/), (float) (point.Y * 64 /*0x40*/)), 1f, onlyLocation: this.NameOrUniqueName));
        this.temporarySprites.Add(new TemporaryAnimatedSprite(6, new Vector2((float) (point.X * 64 /*0x40*/), (float) (point.Y * 64 /*0x40*/)), Color.White)
        {
          layerDepth = 1f,
          interval = 50f,
          motion = new Vector2(1f, 0.0f),
          acceleration = new Vector2(-0.005f, 0.0f)
        });
        this.temporarySprites.Add(new TemporaryAnimatedSprite(6, new Vector2((float) (point.X * 64 /*0x40*/ - 12), (float) (point.Y * 64 /*0x40*/ - 12)), Color.White)
        {
          scale = 0.75f,
          layerDepth = 1f,
          interval = 50f,
          motion = new Vector2(1f, 0.0f),
          acceleration = new Vector2(-0.005f, 0.0f),
          delayBeforeAnimationStart = 50
        });
        this.temporarySprites.Add(new TemporaryAnimatedSprite(6, new Vector2((float) (point.X * 64 /*0x40*/ - 12), (float) (point.Y * 64 /*0x40*/ + 12)), Color.White)
        {
          layerDepth = 1f,
          interval = 50f,
          motion = new Vector2(1f, 0.0f),
          acceleration = new Vector2(-0.005f, 0.0f),
          delayBeforeAnimationStart = 100
        });
        this.temporarySprites.Add(new TemporaryAnimatedSprite(6, new Vector2((float) (point.X * 64 /*0x40*/), (float) (point.Y * 64 /*0x40*/)), Color.White)
        {
          layerDepth = 1f,
          scale = 0.75f,
          interval = 50f,
          motion = new Vector2(1f, 0.0f),
          acceleration = new Vector2(-0.005f, 0.0f),
          delayBeforeAnimationStart = 150
        });
        if (this.characters.Count != 0)
          break;
        this.characters.Add((NPC) new Junimo(new Vector2(8f, 7f) * 64f, 6));
        break;
    }
  }

  public virtual void hostSetup()
  {
    if (!Game1.IsMasterGame || this.farmers.Any() || this.HasFarmerWatchingBroadcastEventReturningHere())
      return;
    this.interiorDoors.ResetSharedState();
  }

  public virtual void ResetForEvent(Event ev)
  {
    ev.eventPositionTileOffset = Vector2.Zero;
    if (!this.IsOutdoors)
      return;
    Game1.ambientLight = this.IsRainingHere() ? new Color((int) byte.MaxValue, 200, 80 /*0x50*/) : Color.White;
  }

  public virtual bool HasFarmerWatchingBroadcastEventReturningHere()
  {
    foreach (Farmer allFarmer in Game1.getAllFarmers())
    {
      if (allFarmer.locationBeforeForcedEvent.Value != null && allFarmer.locationBeforeForcedEvent.Value == this.NameOrUniqueName)
        return true;
    }
    return false;
  }

  public void resetForPlayerEntry()
  {
    Game1.updateWeatherIcon();
    Game1.hooks.OnGameLocation_ResetForPlayerEntry(this, (Action) (() =>
    {
      this._madeMapModifications = false;
      if (!this.farmers.Any() && !this.HasFarmerWatchingBroadcastEventReturningHere() || Game1.player.sleptInTemporaryBed.Value)
        this.resetSharedState();
      this.resetLocalState();
      if (this._madeMapModifications)
        return;
      this._madeMapModifications = true;
      this.MakeMapModifications();
    }));
    Microsoft.Xna.Framework.Rectangle boundingBox1 = Game1.player.GetBoundingBox();
    foreach (Furniture furniture in this.furniture)
    {
      Microsoft.Xna.Framework.Rectangle boundingBox2 = furniture.GetBoundingBox();
      if (boundingBox2.Intersects(boundingBox1) && furniture.IntersectsForCollision(boundingBox1) && !furniture.isPassable())
        Game1.player.TemporaryPassableTiles.Add(boundingBox2);
    }
  }

  protected virtual void resetLocalState()
  {
    bool flag = Game1.newDaySync.hasInstance();
    string propertyValue;
    if (this.TryGetMapProperty("ViewportClamp", out propertyValue))
    {
      try
      {
        int[] stringToIntArray = Utility.parseStringToIntArray(propertyValue);
        Game1.viewportClampArea = new Microsoft.Xna.Framework.Rectangle(stringToIntArray[0] * 64 /*0x40*/, stringToIntArray[1] * 64 /*0x40*/, stringToIntArray[2] * 64 /*0x40*/, stringToIntArray[3] * 64 /*0x40*/);
      }
      catch (Exception ex)
      {
        Game1.viewportClampArea = Microsoft.Xna.Framework.Rectangle.Empty;
      }
    }
    else
      Game1.viewportClampArea = Microsoft.Xna.Framework.Rectangle.Empty;
    Game1.elliottPiano = 0;
    Game1.crabPotOverlayTiles.Clear();
    Utility.killAllStaticLoopingSoundCues();
    Game1.player.bridge = (SuspensionBridge) null;
    Game1.player.SetOnBridge(false);
    if (Game1.CurrentEvent == null && !this.Name.ContainsIgnoreCase("bath"))
      Game1.player.canOnlyWalk = false;
    if (!(this is Farm))
      this.temporarySprites.RemoveWhere<TemporaryAnimatedSprite>((Predicate<TemporaryAnimatedSprite>) (sprite => sprite.clearOnAreaEntry()));
    if (Game1.options != null)
    {
      if (Game1.isOneOfTheseKeysDown(Game1.GetKeyboardState(), Game1.options.runButton))
        Game1.player.setRunning(!Game1.options.autoRun, true);
      else
        Game1.player.setRunning(Game1.options.autoRun, true);
    }
    Game1.player.mount?.SyncPositionToRider();
    Game1.UpdateViewPort(false, Game1.player.StandingPixel);
    Game1.previousViewportPosition = new Vector2((float) Game1.viewport.X, (float) Game1.viewport.Y);
    Game1.PushUIMode();
    foreach (IClickableMenu onScreenMenu in (IEnumerable<IClickableMenu>) Game1.onScreenMenus)
      onScreenMenu.gameWindowSizeChanged(new Microsoft.Xna.Framework.Rectangle(Game1.uiViewport.X, Game1.uiViewport.Y, Game1.uiViewport.Width, Game1.uiViewport.Height), new Microsoft.Xna.Framework.Rectangle(Game1.uiViewport.X, Game1.uiViewport.Y, Game1.uiViewport.Width, Game1.uiViewport.Height));
    Game1.PopUIMode();
    this.ignoreWarps = false;
    if (!flag || Game1.newDaySync.hasFinished())
    {
      if (Game1.player.rightRing.Value != null)
        Game1.player.rightRing.Value.onNewLocation(Game1.player, this);
      if (Game1.player.leftRing.Value != null)
        Game1.player.leftRing.Value.onNewLocation(Game1.player, this);
    }
    this.forceViewportPlayerFollow = this.Map.Properties.ContainsKey("ViewportFollowPlayer");
    this.lastTouchActionLocation = Game1.player.Tile;
    Game1.player.NotifyQuests((Func<Quest, bool>) (quest => quest.OnWarped(this)));
    if (!this.isOutdoors.Value)
      Game1.player.FarmerSprite.currentStep = "thudStep";
    this.setUpLocationSpecificFlair();
    this._updateAmbientLighting();
    if (!this.ignoreLights.Value)
    {
      string lightIdPrefix = this.NameOrUniqueName + "_MapLight_";
      Game1.currentLightSources.RemoveWhere<string, LightSource>((Func<KeyValuePair<string, LightSource>, bool>) (p => p.Key.StartsWith(lightIdPrefix)));
      string[] propertySplitBySpaces1 = this.GetMapPropertySplitBySpaces("Light");
      for (int index = 0; index < propertySplitBySpaces1.Length; index += 3)
      {
        Point point;
        string error;
        int textureIndex;
        if (!ArgUtility.TryGetPoint(propertySplitBySpaces1, index, out point, out error, "Point tile") || !ArgUtility.TryGetInt(propertySplitBySpaces1, index + 2, out textureIndex, out error, "int textureIndex"))
          this.LogMapPropertyError("Light", propertySplitBySpaces1, error);
        else
          Game1.currentLightSources.Add(new LightSource($"{lightIdPrefix}_{point.X}_{point.Y}", textureIndex, new Vector2((float) (point.X * 64 /*0x40*/ + 32 /*0x20*/), (float) (point.Y * 64 /*0x40*/ + 32 /*0x20*/)), 1f, LightSource.LightContext.MapLight, onlyLocation: this.NameOrUniqueName));
      }
      if (!Game1.isTimeToTurnOffLighting(this) && !Game1.isRaining)
      {
        string[] propertySplitBySpaces2 = this.GetMapPropertySplitBySpaces("WindowLight");
        for (int index = 0; index < propertySplitBySpaces2.Length; index += 3)
        {
          Point point;
          string error;
          int textureIndex;
          if (!ArgUtility.TryGetPoint(propertySplitBySpaces2, index, out point, out error, "Point tile") || !ArgUtility.TryGetInt(propertySplitBySpaces2, index + 2, out textureIndex, out error, "int textureIndex"))
            this.LogMapPropertyError("WindowLight", propertySplitBySpaces2, error);
          else
            Game1.currentLightSources.Add(new LightSource($"{lightIdPrefix}_{point.X}_{point.Y}_Window", textureIndex, new Vector2((float) (point.X * 64 /*0x40*/ + 32 /*0x20*/), (float) (point.Y * 64 /*0x40*/ + 32 /*0x20*/)), 1f, LightSource.LightContext.WindowLight, onlyLocation: this.NameOrUniqueName));
        }
        foreach (Vector2 lightGlow in (NetHashSet<Vector2>) this.lightGlows)
          Game1.currentLightSources.Add(new LightSource($"{lightIdPrefix}_{lightGlow.X}_{lightGlow.Y}_Glow", 6, lightGlow, 1f, LightSource.LightContext.WindowLight, onlyLocation: this.NameOrUniqueName));
      }
    }
    if (this.isOutdoors.Value || this.treatAsOutdoors.Value)
    {
      string[] propertySplitBySpaces = this.GetMapPropertySplitBySpaces("BrookSounds");
      for (int index = 0; index < propertySplitBySpaces.Length; index += 3)
      {
        Vector2 tileLocation;
        string error;
        int whichSound;
        if (!ArgUtility.TryGetVector2(propertySplitBySpaces, index, out tileLocation, out error, name: "Vector2 tile") || !ArgUtility.TryGetInt(propertySplitBySpaces, index + 2, out whichSound, out error, "int soundId"))
          this.LogMapPropertyError("BrookSounds", propertySplitBySpaces, error);
        else
          AmbientLocationSounds.addSound(tileLocation, whichSound);
      }
      Game1.randomizeRainPositions();
      Game1.randomizeDebrisWeatherPositions(Game1.debrisWeather);
    }
    foreach (KeyValuePair<Vector2, TerrainFeature> pair in this.terrainFeatures.Pairs)
      pair.Value.performPlayerEntryAction();
    if (this.largeTerrainFeatures != null)
    {
      foreach (TerrainFeature largeTerrainFeature in this.largeTerrainFeatures)
        largeTerrainFeature.performPlayerEntryAction();
    }
    foreach (KeyValuePair<Vector2, Object> pair in this.objects.Pairs)
      pair.Value.actionOnPlayerEntry();
    if (this.isOutdoors.Value)
    {
      ((FarmerSprite) Game1.player.Sprite).currentStep = "sandyStep";
      this.tryToAddCritters();
    }
    this.interiorDoors.ResetLocalState();
    int num1 = Game1.getTrulyDarkTime(this) - 100;
    if (Game1.timeOfDay < num1 && (!this.IsRainingHere() || this.name.Equals((object) "SandyHouse")))
    {
      string[] propertySplitBySpaces = this.GetMapPropertySplitBySpaces("DayTiles");
      for (int index = 0; index < propertySplitBySpaces.Length; index += 4)
      {
        string layerId;
        string error;
        Point point;
        int num2;
        if (!ArgUtility.TryGet(propertySplitBySpaces, index, out layerId, out error, name: "string layerId") || !ArgUtility.TryGetPoint(propertySplitBySpaces, index + 1, out point, out error, "Point position") || !ArgUtility.TryGetInt(propertySplitBySpaces, index + 3, out num2, out error, "int tileIndex"))
          this.LogMapPropertyError("DayTiles", propertySplitBySpaces, error);
        else if (num2 != 720 || !Game1.MasterPlayer.mailReceived.Contains("pamHouseUpgrade"))
        {
          Tile tile = this.map.RequireLayer(layerId).Tiles[point.X, point.Y];
          if (tile != null)
            tile.TileIndex = num2;
        }
      }
    }
    else if (Game1.timeOfDay >= num1 || this.IsRainingHere() && !this.name.Equals((object) "SandyHouse"))
      this.switchOutNightTiles();
    if (Game1.killScreen && Game1.activeClickableMenu != null && !Game1.dialogueUp)
    {
      Game1.activeClickableMenu.emergencyShutDown();
      Game1.exitActiveMenu();
    }
    if (Game1.activeClickableMenu == null && !Game1.warpingForForcedRemoteEvent && !flag)
      this.checkForEvents();
    foreach (KeyValuePair<string, LightSource> pair in this.sharedLights.Pairs)
      Game1.currentLightSources[pair.Key] = pair.Value;
    foreach (NPC character in this.characters)
      character.behaviorOnLocalFarmerLocationEntry(this);
    foreach (Object @object in this.furniture)
      @object.actionOnPlayerEntry();
    this.updateFishSplashAnimation();
    this.updateOrePanAnimation();
    this.showDropboxIndicator = false;
    foreach (SpecialOrder specialOrder in Game1.player.team.specialOrders)
    {
      if (!specialOrder.ShouldDisplayAsComplete())
      {
        foreach (OrderObjective objective in specialOrder.objectives)
        {
          if (objective is DonateObjective donateObjective && !string.IsNullOrEmpty(donateObjective.dropBoxGameLocation.Value) && donateObjective.GetDropboxLocationName() == this.Name)
          {
            this.showDropboxIndicator = true;
            this.dropBoxIndicatorLocation = donateObjective.dropBoxTileLocation.Value * 64f + new Vector2(7f, 0.0f) * 4f;
          }
        }
      }
    }
    if (Game1.timeOfDay >= 1830)
    {
      foreach (FarmAnimal farmAnimal in this.animals.Values.ToArray<FarmAnimal>())
        farmAnimal.warpHome();
    }
    foreach (Building building in this.buildings)
      building.resetLocalState();
    if (this.isThereABuildingUnderConstruction())
    {
      foreach (string key in Game1.netWorldState.Value.Builders.Keys)
      {
        BuilderData builder = Game1.netWorldState.Value.Builders[key];
        if (builder.buildingLocation.Value == this.NameOrUniqueName && builder.daysUntilBuilt.Value > 0)
        {
          NPC characterFromName = Game1.getCharacterFromName(key);
          if (characterFromName != null && characterFromName.currentLocation.Equals(this))
          {
            Building buildingAt = this.getBuildingAt(Utility.PointToVector2(builder.buildingTile.Value));
            if (buildingAt != null)
              this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(399, 262, buildingAt.daysOfConstructionLeft.Value == 1 ? 29 : 9, 43), new Vector2((float) (buildingAt.tileX.Value + buildingAt.tilesWide.Value / 2), (float) (buildingAt.tileY.Value + buildingAt.tilesHigh.Value / 2)) * 64f + new Vector2(-16f, -144f), false, 0.0f, Color.White)
              {
                id = 16846,
                scale = 4f,
                interval = 999999f,
                animationLength = 1,
                totalNumberOfLoops = 99999,
                layerDepth = (float) ((buildingAt.tileY.Value + buildingAt.tilesHigh.Value / 2) * 64 /*0x40*/ + 32 /*0x20*/) / 10000f
              });
          }
        }
      }
    }
    else
      this.removeTemporarySpritesWithIDLocal(16846);
  }

  protected virtual void _updateAmbientLighting()
  {
    if (Game1.eventUp || Game1.player.viewingLocation.Value != null && !Game1.player.viewingLocation.Value.Equals(this.Name))
      return;
    if (!this.isOutdoors.Value || this.ignoreOutdoorLighting.Value)
    {
      if (Game1.isStartingToGetDarkOut(this) || (double) this.lightLevel.Value > 0.0)
      {
        float t = 1f - Utility.Clamp((float) Utility.CalculateMinutesBetweenTimes(Game1.timeOfDay + Game1.gameTimeInterval / (Game1.realMilliSecondsPerGameMinute + this.ExtraMillisecondsPerInGameMinute), Game1.getTrulyDarkTime(this)) / 120f, 0.0f, 1f);
        Game1.ambientLight = new Color((int) (byte) Utility.Lerp((float) this.indoorLightingColor.R, (float) this.indoorLightingNightColor.R, t), (int) (byte) Utility.Lerp((float) this.indoorLightingColor.G, (float) this.indoorLightingNightColor.G, t), (int) (byte) Utility.Lerp((float) this.indoorLightingColor.B, (float) this.indoorLightingNightColor.B, t));
      }
      else
        Game1.ambientLight = this.indoorLightingColor;
    }
    else
      Game1.ambientLight = this.IsRainingHere() ? new Color((int) byte.MaxValue, 200, 80 /*0x50*/) : Color.White;
  }

  private bool TryGetAmbientLightFromMap(out Color color, string propertyName = "AmbientLight")
  {
    string[] propertySplitBySpaces = this.GetMapPropertySplitBySpaces(propertyName);
    if (propertySplitBySpaces.Length != 0)
    {
      int r;
      string error;
      int g;
      int b;
      if (!ArgUtility.TryGetInt(propertySplitBySpaces, 0, out r, out error, "int r") || !ArgUtility.TryGetInt(propertySplitBySpaces, 1, out g, out error, "int g") || !ArgUtility.TryGetInt(propertySplitBySpaces, 2, out b, out error, "int b"))
      {
        this.LogMapPropertyError(propertyName, propertySplitBySpaces, error);
      }
      else
      {
        color = new Color(r, g, b);
        return true;
      }
    }
    color = Color.White;
    return false;
  }

  public void SelectRandomMiniJukeboxTrack()
  {
    if (this.miniJukeboxTrack.Value != "random")
      return;
    Farmer player = Game1.player;
    if (this is FarmHouse farmHouse && farmHouse.HasOwner)
      player = farmHouse.owner;
    List<string> jukeboxTracks = Utility.GetJukeboxTracks(player, this);
    this.randomMiniJukeboxTrack.Value = Game1.random.ChooseFrom<string>((IList<string>) jukeboxTracks);
  }

  protected virtual void resetSharedState()
  {
    this.SelectRandomMiniJukeboxTrack();
    for (int index = this.characters.Count - 1; index >= 0; --index)
      this.characters[index].behaviorOnFarmerLocationEntry(this, Game1.player);
    if (this is MineShaft)
      return;
    switch (this.GetSeason())
    {
      case Season.Spring:
        this.waterColor.Value = new Color(120, 200, (int) byte.MaxValue) * 0.5f;
        break;
      case Season.Summer:
        this.waterColor.Value = new Color(60, 240 /*0xF0*/, (int) byte.MaxValue) * 0.5f;
        break;
      case Season.Fall:
        this.waterColor.Value = new Color((int) byte.MaxValue, 130, 200) * 0.5f;
        break;
      case Season.Winter:
        this.waterColor.Value = new Color(130, 80 /*0x50*/, (int) byte.MaxValue) * 0.5f;
        break;
    }
  }

  public LightSource getLightSource([NotNullWhen(true)] string identifier)
  {
    LightSource lightSource;
    return identifier == null || !this.sharedLights.TryGetValue(identifier, out lightSource) ? (LightSource) null : lightSource;
  }

  public bool hasLightSource([NotNullWhen(true)] string identifier)
  {
    return identifier != null && this.sharedLights.ContainsKey(identifier);
  }

  public void removeLightSource([NotNullWhen(true)] string identifier)
  {
    if (identifier == null)
      return;
    this.sharedLights.Remove(identifier);
  }

  public void repositionLightSource([NotNullWhen(true)] string identifier, Vector2 position)
  {
    LightSource lightSource;
    if (identifier == null || !this.sharedLights.TryGetValue(identifier, out lightSource))
      return;
    lightSource.position.Value = position;
  }

  public virtual bool CanSpawnCharacterHere(Vector2 tileLocation)
  {
    return this.isTileOnMap(tileLocation) && this.isTilePlaceable(tileLocation) && !this.IsTileBlockedBy(tileLocation);
  }

  /// <summary>Get whether items in general can be placed on a tile.</summary>
  /// <param name="tile">The tile position within the location.</param>
  /// <param name="itemIsPassable">Whether the item being placed can be walked over by players and characters.</param>
  /// <param name="collisionMask">The collision types to look for. This should usually be kept default.</param>
  /// <param name="ignorePassables">The collision types to ignore when they don't block movement (e.g. tilled dirt).</param>
  /// <param name="useFarmerTile">When checking collisions with farmers, whether to check their tile position instead of their bounding box.</param>
  /// <param name="ignorePassablesExactly">Whether to use the exact <paramref name="ignorePassables" /> value provided, without adjusting it for <paramref name="itemIsPassable" />. This should only be true in specialized cases.</param>
  public virtual bool CanItemBePlacedHere(
    Vector2 tile,
    bool itemIsPassable = false,
    CollisionMask collisionMask = CollisionMask.All,
    CollisionMask ignorePassables = CollisionMask.Buildings | CollisionMask.Characters | CollisionMask.Farmers | CollisionMask.Flooring | CollisionMask.Furniture | CollisionMask.TerrainFeatures | CollisionMask.LocationSpecific,
    bool useFarmerTile = false,
    bool ignorePassablesExactly = false)
  {
    if (!ignorePassablesExactly)
    {
      ignorePassables &= CollisionMask.Buildings | CollisionMask.Characters | CollisionMask.Farmers | CollisionMask.Flooring | CollisionMask.Furniture | CollisionMask.TerrainFeatures | CollisionMask.LocationSpecific;
      if (!itemIsPassable)
        ignorePassables &= CollisionMask.Buildings | CollisionMask.Flooring | CollisionMask.Furniture | CollisionMask.Objects | CollisionMask.TerrainFeatures | CollisionMask.LocationSpecific;
    }
    return this.isTileOnMap(tile) && this.isTilePlaceable(tile, itemIsPassable) && this.GetHoeDirtAtTile(tile)?.crop == null && !this.IsTileBlockedBy(tile, collisionMask, ignorePassables, useFarmerTile) && (!itemIsPassable || this.getBuildingAt(tile) == null || this.getBuildingAt(tile).GetData() == null || this.getBuildingAt(tile).GetData().AllowsFlooringUnderneath);
  }

  /// <summary>Get whether a tile is either occupied by an object or is a non-passable tile.</summary>
  /// <param name="tile">The tile position within the location.</param>
  /// <param name="collisionMask">The collision types to look for. This should usually be kept default.</param>
  /// <param name="ignorePassables">The collision types to ignore when they don't block movement (e.g. tilled dirt).</param>
  /// <param name="useFarmerTile">When checking collisions with farmers, whether to check their tile position instead of their bounding box.</param>
  public virtual bool IsTileBlockedBy(
    Vector2 tile,
    CollisionMask collisionMask = CollisionMask.All,
    CollisionMask ignorePassables = CollisionMask.None,
    bool useFarmerTile = false)
  {
    return this.IsTileOccupiedBy(tile, collisionMask, ignorePassables, useFarmerTile) || !this.isTilePassable(tile);
  }

  /// <summary>Get whether a tile is occupied.</summary>
  /// <param name="tile">The tile position within the location.</param>
  /// <param name="collisionMask">The collision types to look for. This should usually be kept default.</param>
  /// <param name="ignorePassables">The collision types to ignore when they don't block movement (e.g. tilled dirt).</param>
  /// <param name="useFarmerTile">When checking collisions with farmers, whether to check their tile position instead of their bounding box.</param>
  public virtual bool IsTileOccupiedBy(
    Vector2 tile,
    CollisionMask collisionMask = CollisionMask.All,
    CollisionMask ignorePassables = CollisionMask.None,
    bool useFarmerTile = false)
  {
    Microsoft.Xna.Framework.Rectangle rectangle = new Microsoft.Xna.Framework.Rectangle((int) tile.X * 64 /*0x40*/, (int) tile.Y * 64 /*0x40*/, 64 /*0x40*/, 64 /*0x40*/);
    if (collisionMask.HasFlag((Enum) CollisionMask.Farmers) && !ignorePassables.HasFlag((Enum) CollisionMask.Farmers))
    {
      foreach (Farmer farmer in this.farmers)
      {
        if ((useFarmerTile ? (farmer.Tile == tile ? 1 : 0) : (farmer.GetBoundingBox().Intersects(rectangle) ? 1 : 0)) != 0)
          return true;
      }
    }
    Object @object;
    if (collisionMask.HasFlag((Enum) CollisionMask.Objects) && this.objects.TryGetValue(tile, out @object) && (!ignorePassables.HasFlag((Enum) CollisionMask.Objects) || !@object.isPassable()))
      return true;
    if (collisionMask.HasFlag((Enum) CollisionMask.Furniture))
    {
      Furniture furnitureAt = this.GetFurnitureAt(tile);
      if (furnitureAt != null && (!ignorePassables.HasFlag((Enum) CollisionMask.Furniture) || !furnitureAt.isPassable()))
        return true;
    }
    if (collisionMask.HasFlag((Enum) CollisionMask.Characters))
    {
      foreach (NPC character in this.characters)
      {
        if (character != null && character.GetBoundingBox().Intersects(rectangle) && !character.IsInvisible && (!ignorePassables.HasFlag((Enum) CollisionMask.Characters) || !character.farmerPassesThrough))
          return true;
      }
      if (this.animals.Length > 0)
      {
        foreach (FarmAnimal farmAnimal in this.animals.Values)
        {
          if (farmAnimal.Tile == tile && (!ignorePassables.HasFlag((Enum) CollisionMask.Characters) || !farmAnimal.farmerPassesThrough))
            return true;
        }
      }
    }
    if (collisionMask.HasFlag((Enum) CollisionMask.TerrainFeatures))
    {
      foreach (ResourceClump resourceClump in this.resourceClumps)
      {
        if (resourceClump.occupiesTile((int) tile.X, (int) tile.Y) && (!ignorePassables.HasFlag((Enum) CollisionMask.TerrainFeatures) || !resourceClump.isPassable((Character) null)))
          return true;
      }
      if (this.largeTerrainFeatures != null)
      {
        foreach (LargeTerrainFeature largeTerrainFeature in this.largeTerrainFeatures)
        {
          if (largeTerrainFeature.getBoundingBox().Intersects(rectangle) && (!ignorePassables.HasFlag((Enum) CollisionMask.TerrainFeatures) || !largeTerrainFeature.isPassable()))
            return true;
        }
      }
    }
    TerrainFeature terrainFeature;
    if ((collisionMask.HasFlag((Enum) CollisionMask.TerrainFeatures) || collisionMask.HasFlag((Enum) CollisionMask.Flooring)) && this.terrainFeatures.TryGetValue(tile, out terrainFeature) && terrainFeature.getBoundingBox().Intersects(rectangle))
    {
      CollisionMask flag = terrainFeature is Flooring ? CollisionMask.Flooring : CollisionMask.TerrainFeatures;
      if (collisionMask.HasFlag((Enum) flag) && (!ignorePassables.HasFlag((Enum) flag) || !terrainFeature.isPassable()))
        return true;
    }
    if (collisionMask.HasFlag((Enum) CollisionMask.LocationSpecific) && this.IsLocationSpecificOccupantOnTile(tile))
      return true;
    if (collisionMask.HasFlag((Enum) CollisionMask.Buildings))
    {
      foreach (Building building in this.buildings)
      {
        if (!building.isMoving && (ignorePassables.HasFlag((Enum) CollisionMask.Buildings) ? (!building.isTilePassable(tile) ? 1 : 0) : (building.occupiesTile(tile) ? 1 : 0)) != 0)
          return true;
      }
    }
    return false;
  }

  public virtual bool IsLocationSpecificOccupantOnTile(Vector2 tileLocation) => false;

  public virtual bool IsLocationSpecificPlacementRestriction(Vector2 tileLocation) => false;

  public Farmer isTileOccupiedByFarmer(Vector2 tileLocation)
  {
    foreach (Farmer farmer in this.farmers)
    {
      if (farmer.Tile == tileLocation)
        return farmer;
    }
    return (Farmer) null;
  }

  /// <summary>Get any tilled dirt at a tile position, whether it's on the ground or in a garden pot.</summary>
  /// <param name="tile">The tile position to check.</param>
  /// <returns>Returns the tilled dirt found, else <c>null</c>.</returns>
  public HoeDirt GetHoeDirtAtTile(Vector2 tile)
  {
    Object @object;
    if (this.objects.TryGetValue(tile, out @object) && @object is IndoorPot indoorPot)
      return indoorPot.hoeDirt.Value;
    TerrainFeature terrainFeature;
    return this.terrainFeatures.TryGetValue(tile, out terrainFeature) && terrainFeature is HoeDirt hoeDirt ? hoeDirt : (HoeDirt) null;
  }

  /// <summary>Get whether a tile contains a hoe dirt, or an object that should behave like a hoe dirt, such as a Garden Pot.</summary>
  public bool isTileHoeDirt(Vector2 tile) => this.GetHoeDirtAtTile(tile) != null;

  /// <summary>Get whether a tile is not on the water, and is unobstructed by a tile on the Buildings layer or higher. This can be used to ensure items don't spawn behind high walls, etc.</summary>
  public bool isTileLocationOpen(Location location)
  {
    return this.isTileLocationOpen(new Vector2((float) location.X, (float) location.Y));
  }

  /// <summary>Get whether a tile is not on the water, and is unobstructed by a tile on the Buildings layer or higher. This can be used to ensure items don't spawn behind high walls, etc.</summary>
  public bool isTileLocationOpen(Vector2 location)
  {
    return this.map.RequireLayer("Buildings").Tiles[(int) location.X, (int) location.Y] == null && !this.isWaterTile((int) location.X, (int) location.Y) && this.map.RequireLayer("Front").Tiles[(int) location.X, (int) location.Y] == null && this.map.GetLayer("AlwaysFront")?.Tiles[(int) location.X, (int) location.Y] == null;
  }

  public virtual bool CanPlaceThisFurnitureHere(Furniture furniture)
  {
    if (furniture == null)
      return false;
    bool flag = this is DecoratableLocation || !this.IsOutdoors;
    if (furniture.furniture_type.Value == 15)
    {
      bool parsed;
      if (!this.TryGetMapPropertyAs("AllowBeds", out parsed))
        parsed = this is FarmHouse || this is IslandFarmHouse || flag && this.ParentBuilding != null;
      if (!parsed)
        return false;
    }
    switch (furniture.placementRestriction)
    {
      case 0:
        return flag;
      case 1:
        return !flag;
      case 2:
        return flag || !flag;
      default:
        return false;
    }
  }

  /// <summary>Get whether a tile is allowed to have an object placed on it. Note that this function does not factor in the tile's current occupancy.</summary>
  public virtual bool isTilePlaceable(Vector2 v, bool itemIsPassable = false)
  {
    if (this.IsLocationSpecificPlacementRestriction(v) || !this.hasTileAt((int) v.X, (int) v.Y, "Back") || this.isWaterTile((int) v.X, (int) v.Y))
      return false;
    switch (this.doesTileHaveProperty((int) v.X, (int) v.Y, "NoFurniture", "Back"))
    {
      case null:
        return true;
      case "total":
        return false;
      default:
        if (!itemIsPassable || !Game1.currentLocation.IsOutdoors)
          return false;
        goto case null;
    }
  }

  public void playTerrainSound(
    Vector2 tileLocation,
    Character who = null,
    bool showTerrainDisturbAnimation = true)
  {
    string audioName = "thudStep";
    if (this.IsOutdoors || this.treatAsOutdoors.Value || this.Name.ContainsIgnoreCase("mine"))
    {
      switch (this.doesTileHaveProperty((int) tileLocation.X, (int) tileLocation.Y, "Type", "Back"))
      {
        case null:
          if (this.isWaterTile((int) tileLocation.X, (int) tileLocation.Y))
          {
            audioName = "waterSlosh";
            break;
          }
          break;
        case "Dirt":
          audioName = "sandyStep";
          break;
        case "Stone":
          audioName = "stoneStep";
          break;
        case "Grass":
          audioName = this.GetSeason() == Season.Winter ? "snowyStep" : "grassyStep";
          break;
        case "Wood":
          audioName = "woodyStep";
          break;
      }
    }
    TerrainFeature terrainFeature;
    if (this.terrainFeatures.TryGetValue(tileLocation, out terrainFeature) && terrainFeature is Flooring)
      audioName = ((Flooring) this.terrainFeatures[tileLocation]).getFootstepSound();
    if (who != null & showTerrainDisturbAnimation && audioName == "sandyStep")
    {
      Vector2 vector2 = Vector2.Zero;
      if (who.shouldShadowBeOffset)
        vector2 = who.drawOffset;
      this.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(0, 64 /*0x40*/, 64 /*0x40*/, 64 /*0x40*/), 50f, 4, 1, new Vector2(who.Position.X + (float) Game1.random.Next(-8, 8), who.Position.Y + (float) Game1.random.Next(-16, 0)) + vector2, false, Game1.random.NextBool(), 0.0001f, 0.0f, Color.White, 1f, 0.01f, 0.0f, (float) ((double) Game1.random.Next(-5, 6) * 3.1415927410125732 / 128.0)));
    }
    else if (who != null & showTerrainDisturbAnimation && this.GetSeason() == Season.Winter && audioName == "grassyStep")
    {
      Vector2 vector2 = Vector2.Zero;
      if (who.shouldShadowBeOffset)
        vector2 = who.drawOffset;
      this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(247, 407, 6, 6), 2000f, 1, 10000, new Vector2(who.Position.X, who.Position.Y) + vector2, false, false, 0.0001f, 1f / 1000f, Color.White, 1f, 0.01f, 0.0f, 0.0f));
    }
    if ((who is Farmer farmer ? farmer.boots.Value?.ItemId : (string) null) == "853")
      this.localSound("jingleBell");
    if (audioName.Length <= 0)
      return;
    this.localSound(audioName);
  }

  public bool checkTileIndexAction(int tileIndex)
  {
    switch (tileIndex)
    {
      case 1799:
      case 1824:
      case 1825:
      case 1826:
      case 1827:
      case 1828:
      case 1829:
      case 1830:
      case 1831:
      case 1832:
      case 1833:
        if (this.Name.Equals("AbandonedJojaMart"))
        {
          Game1.RequireLocation<AbandonedJojaMart>("AbandonedJojaMart").checkBundle();
          return true;
        }
        break;
    }
    return false;
  }

  public bool checkForTerrainFeaturesAndObjectsButDestroyNonPlayerItems(int x, int y)
  {
    Vector2 key = new Vector2((float) x, (float) y);
    Object @object;
    if (this.objects.TryGetValue(key, out @object))
    {
      if (!@object.IsSpawnedObject || @object is Chest || @object.Type == "Crafting")
        return false;
      this.objects.Remove(key);
    }
    this.terrainFeatures.Remove(key);
    return true;
  }

  public virtual bool checkAction(Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who)
  {
    who.ignoreItemConsumptionThisFrame = false;
    Microsoft.Xna.Framework.Rectangle rect = new Microsoft.Xna.Framework.Rectangle(tileLocation.X * 64 /*0x40*/, tileLocation.Y * 64 /*0x40*/, 64 /*0x40*/, 64 /*0x40*/);
    if (!this.objects.ContainsKey(new Vector2((float) tileLocation.X, (float) tileLocation.Y)) && this.CheckPetAnimal(rect, who))
      return true;
    foreach (Building building in this.buildings)
    {
      if (building.doAction(new Vector2((float) tileLocation.X, (float) tileLocation.Y), who))
        return true;
    }
    if (who.IsSitting())
    {
      who.StopSitting();
      return true;
    }
    foreach (Farmer farmer in this.farmers)
    {
      if (farmer != Game1.player && farmer.GetBoundingBox().Intersects(rect) && farmer.checkAction(who, this))
        return true;
    }
    if (this.currentEvent != null && this.currentEvent.isFestival)
      return this.currentEvent.checkAction(tileLocation, viewport, who);
    foreach (NPC character in this.characters)
    {
      if (character != null && !character.IsMonster && (!who.isRidingHorse() || !(character is Horse)) && character.GetBoundingBox().Intersects(rect) && character.checkAction(who, this))
      {
        if (who.FarmerSprite.IsPlayingBasicAnimation(who.FacingDirection, false) || who.FarmerSprite.IsPlayingBasicAnimation(who.FacingDirection, true))
          who.faceGeneralDirection(character.getStandingPosition(), 0, false, false);
        return true;
      }
    }
    int tileIndexAt = this.getTileIndexAt(tileLocation, "Buildings", "untitled tile sheet");
    if (this.NameOrUniqueName == "SkullCave" && (tileIndexAt == 344 || tileIndexAt == 349))
    {
      if (Game1.player.team.SpecialOrderActive("QiChallenge10"))
      {
        who.doEmote(40);
        return false;
      }
      if (!Game1.player.team.completedSpecialOrders.Contains("QiChallenge10"))
      {
        who.doEmote(8);
        return false;
      }
      if (!Game1.player.team.toggleSkullShrineOvernight.Value)
      {
        if (!Game1.player.team.skullShrineActivated.Value)
        {
          this.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:ChallengeShrine_NotYetHard"), this.createYesNoResponses(), "ShrineOfSkullChallenge");
        }
        else
        {
          Game1.player.team.toggleSkullShrineOvernight.Value = true;
          Game1.showGlobalMessage(Game1.content.LoadString("Strings\\Locations:ChallengeShrine_Activated"));
          Game1.multiplayer.globalChatInfoMessage(Game1.player.team.skullShrineActivated.Value ? "HardModeSkullCaveDeactivated" : "HardModeSkullCaveActivated", who.name.Value);
          this.playSound(Game1.player.team.skullShrineActivated.Value ? "skeletonStep" : "serpentDie");
        }
      }
      else if (Game1.player.team.toggleSkullShrineOvernight.Value && Game1.player.team.skullShrineActivated.Value)
      {
        Game1.player.team.toggleSkullShrineOvernight.Value = false;
        Game1.showGlobalMessage(Game1.content.LoadString("Strings\\UI:PendingProposal_Canceling"));
        this.playSound("skeletonStep");
      }
      return true;
    }
    foreach (ResourceClump resourceClump in this.resourceClumps)
    {
      if (resourceClump.getBoundingBox().Intersects(rect) && resourceClump.performUseAction(new Vector2((float) tileLocation.X, (float) tileLocation.Y)))
        return true;
    }
    Vector2 key = new Vector2((float) tileLocation.X, (float) tileLocation.Y);
    Object forage;
    if (this.objects.TryGetValue(key, out forage))
    {
      bool isErrorItem = ItemRegistry.GetDataOrErrorItem(forage.QualifiedItemId).IsErrorItem;
      if (forage.Type != null | isErrorItem)
      {
        if (who.isRidingHorse() && !(forage is Fence))
          return false;
        if (key == who.Tile && !forage.isPassable() && (!(forage is Fence fence) || !fence.isGate.Value))
        {
          Tool t1 = ItemRegistry.Create<Tool>("(T)Pickaxe");
          t1.DoFunction(Game1.currentLocation, -1, -1, 0, who);
          if (forage.performToolAction(t1))
          {
            forage.performRemoveAction();
            forage.dropItem(this, who.GetToolLocation(), Utility.PointToVector2(who.StandingPixel));
            Game1.currentLocation.Objects.Remove(key);
            return true;
          }
          Tool t2 = ItemRegistry.Create<Tool>("(T)Axe");
          t2.DoFunction(Game1.currentLocation, -1, -1, 0, who);
          if (this.objects.TryGetValue(key, out forage) && forage.performToolAction(t2))
          {
            forage.performRemoveAction();
            forage.dropItem(this, who.GetToolLocation(), Utility.PointToVector2(who.StandingPixel));
            Game1.currentLocation.Objects.Remove(key);
            return true;
          }
          if (!this.objects.TryGetValue(key, out forage))
            return true;
        }
        if (this.objects.TryGetValue(key, out forage) && (forage.Type == "Crafting" || forage.Type == "interactive"))
        {
          if (who.ActiveObject == null && forage.checkForAction(who))
            return true;
          if (this.objects.TryGetValue(key, out forage))
          {
            if (who.CurrentItem == null)
              return forage.checkForAction(who);
            Object @object = forage.heldObject.Value;
            forage.heldObject.Value = (Object) null;
            bool flag1 = forage.performObjectDropInAction(who.CurrentItem, true, who);
            forage.heldObject.Value = @object;
            bool flag2 = forage.performObjectDropInAction(who.CurrentItem, false, who, true);
            if (flag1 | flag2 && who.isMoving())
              Game1.haltAfterCheck = false;
            if (who.ignoreItemConsumptionThisFrame)
              return true;
            if (!flag2)
              return forage.checkForAction(who) | flag1;
            who.reduceActiveItemByOne();
            return true;
          }
        }
        else if (this.objects.TryGetValue(key, out forage) && forage.isSpawnedObject.Value | isErrorItem)
        {
          int num = forage.quality.Value;
          Random daySaveRandom = Utility.CreateDaySaveRandom((double) key.X, (double) key.Y * 777.0);
          if (forage.isForage())
            forage.Quality = this.GetHarvestSpawnedObjectQuality(who, forage.isForage(), forage.TileLocation, daySaveRandom);
          if (forage.questItem.Value && forage.questId.Value != null && forage.questId.Value != "0" && !who.hasQuest(forage.questId.Value))
            return false;
          if (who.couldInventoryAcceptThisItem((Item) forage))
          {
            if (who.IsLocalPlayer)
            {
              this.localSound("pickUpItem");
              DelayedAction.playSoundAfterDelay("coin", 300);
            }
            who.animateOnce(279 + who.FacingDirection);
            if (!this.isFarmBuildingInterior())
            {
              if (forage.isForage())
                this.OnHarvestedForage(who, forage);
              if (forage.ItemId.Equals("789") && this.Name.Equals("LewisBasement"))
              {
                Bat bat = new Bat(Vector2.Zero, -789);
                bat.focusedOnFarmers = true;
                Game1.changeMusicTrack("none");
                this.playSound("cursed_mannequin");
                this.characters.Add((NPC) bat);
              }
            }
            else
              who.gainExperience(0, 5);
            who.addItemToInventoryBool(forage.getOne());
            ++Game1.stats.ItemsForaged;
            if (who.professions.Contains(13) && daySaveRandom.NextDouble() < 0.2 && !forage.questItem.Value && who.couldInventoryAcceptThisItem((Item) forage) && !this.isFarmBuildingInterior())
            {
              who.addItemToInventoryBool(forage.getOne());
              who.gainExperience(2, 7);
            }
            this.objects.Remove(key);
            return true;
          }
          forage.Quality = num;
        }
      }
    }
    if (who.isRidingHorse())
    {
      who.mount.checkAction(who, this);
      return true;
    }
    foreach (KeyValuePair<Vector2, TerrainFeature> pair in this.terrainFeatures.Pairs)
    {
      if (pair.Value.getBoundingBox().Intersects(rect) && pair.Value.performUseAction(pair.Key))
      {
        Game1.haltAfterCheck = false;
        return true;
      }
    }
    if (this.largeTerrainFeatures != null)
    {
      foreach (LargeTerrainFeature largeTerrainFeature in this.largeTerrainFeatures)
      {
        if (largeTerrainFeature.getBoundingBox().Intersects(rect) && largeTerrainFeature.performUseAction(largeTerrainFeature.Tile))
        {
          Game1.haltAfterCheck = false;
          return true;
        }
      }
    }
    Tile tile = this.map.RequireLayer("Buildings").PickTile(new Location(tileLocation.X * 64 /*0x40*/, tileLocation.Y * 64 /*0x40*/), viewport.Size);
    string fullActionString;
    if (tile == null || !tile.Properties.TryGetValue("Action", out fullActionString))
      fullActionString = this.doesTileHaveProperty(tileLocation.X, tileLocation.Y, "Action", "Buildings");
    if (fullActionString != null)
    {
      NPC npc = this.isCharacterAtTile(key + new Vector2(0.0f, 1f));
      if (this.currentEvent == null && npc != null && !npc.IsInvisible && !npc.IsMonster && (!who.isRidingHorse() || !(npc is Horse)))
      {
        Point standingPixel = npc.StandingPixel;
        if (Utility.withinRadiusOfPlayer(standingPixel.X, standingPixel.Y, 1, who) && npc.checkAction(who, this))
        {
          if (who.FarmerSprite.IsPlayingBasicAnimation(who.FacingDirection, who.IsCarrying()))
            who.faceGeneralDirection(Utility.PointToVector2(standingPixel), 0, false, false);
          return true;
        }
      }
      return this.performAction(fullActionString, who, tileLocation);
    }
    if (tile != null && this.checkTileIndexAction(tile.TileIndex))
      return true;
    foreach (MapSeat mapSeat in this.mapSeats)
    {
      if (mapSeat.OccupiesTile(tileLocation.X, tileLocation.Y) && !mapSeat.IsBlocked(this))
      {
        who.BeginSitting((ISittable) mapSeat);
        return true;
      }
    }
    Point point = new Point(tileLocation.X * 64 /*0x40*/, (tileLocation.Y - 1) * 64 /*0x40*/);
    bool flag = Game1.didPlayerJustRightClick();
    Furniture furniture1 = (Furniture) null;
    foreach (Furniture furniture2 in this.furniture)
    {
      Microsoft.Xna.Framework.Rectangle rectangle = furniture2.boundingBox.Value;
      if (rectangle.Contains((int) ((double) key.X * 64.0), (int) ((double) key.Y * 64.0)) && furniture2.furniture_type.Value != 12)
      {
        if (!flag)
          return furniture2.clicked(who);
        return who.ActiveObject != null && furniture2.performObjectDropInAction((Item) who.ActiveObject, false, who, false) || furniture2.checkForAction(who, false);
      }
      if (furniture2.furniture_type.Value == 6)
      {
        rectangle = furniture2.boundingBox.Value;
        if (rectangle.Contains(point))
          furniture1 = furniture2;
      }
    }
    if (furniture1 != null)
    {
      if (!flag)
        return furniture1.clicked(who);
      return who.ActiveObject != null && furniture1.performObjectDropInAction((Item) who.ActiveObject, false, who, false) || furniture1.checkForAction(who, false);
    }
    return Game1.didPlayerJustRightClick(true) && this.animals.Length > 0 && this.CheckInspectAnimal(rect, who);
  }

  /// <summary>Get the quality for a newly harvested spawned object.</summary>
  /// <param name="who">The player harvesting the object.</param>
  /// <param name="isForage">Whether the object is a forage item.</param>
  /// <param name="tile">The tile position.</param>
  /// <param name="random">The RNG to use if needed, or <c>null</c> to create a new one.</param>
  public int GetHarvestSpawnedObjectQuality(
    Farmer who,
    bool isForage,
    Vector2 tile,
    Random random = null)
  {
    if (who.professions.Contains(16 /*0x10*/) & isForage)
      return 4;
    if (isForage)
    {
      if (random == null)
        random = Utility.CreateDaySaveRandom((double) tile.X, (double) tile.Y * 777.0);
      if (random.NextBool((float) who.ForagingLevel / 30f))
        return 2;
      if (random.NextBool((float) who.ForagingLevel / 15f))
        return 1;
    }
    return 0;
  }

  /// <summary>Handle a player harvesting a spawned forage object.</summary>
  /// <param name="who">The player who harvested the forage.</param>
  /// <param name="forage">The forage object that was harvested.</param>
  public void OnHarvestedForage(Farmer who, Object forage)
  {
    if (forage.SpecialVariable == 724519)
    {
      who.gainExperience(2, 2);
      who.gainExperience(0, 3);
    }
    else
      who.gainExperience(2, 7);
  }

  public virtual bool CanFreePlaceFurniture() => false;

  public virtual bool LowPriorityLeftClick(int x, int y, Farmer who)
  {
    if (Game1.activeClickableMenu != null)
      return false;
    Microsoft.Xna.Framework.Rectangle rectangle;
    for (int index = this.furniture.Count - 1; index >= 0; --index)
    {
      Furniture furniture = this.furniture[index];
      if (this.CanFreePlaceFurniture() || furniture.IsCloseEnoughToFarmer(who))
      {
        if (!furniture.isPassable())
        {
          rectangle = furniture.boundingBox.Value;
          if (rectangle.Contains(x, y) && furniture.canBeRemoved(who))
          {
            furniture.AttemptRemoval((Action<Furniture>) (f =>
            {
              Guid job = this.furniture.GuidOf(f);
              if (this.furnitureToRemove.Contains(job))
                return;
              this.furnitureToRemove.Add(job);
            }));
            return true;
          }
        }
        rectangle = furniture.boundingBox.Value;
        if (rectangle.Contains(x, y) && furniture.heldObject.Value != null)
        {
          furniture.clicked(who);
          return true;
        }
        if (!furniture.isGroundFurniture() && furniture.canBeRemoved(who))
        {
          int y1 = y;
          if (this is DecoratableLocation decoratableLocation)
          {
            int wallTopY = decoratableLocation.GetWallTopY(x / 64 /*0x40*/, y / 64 /*0x40*/);
            y1 = wallTopY != -1 ? wallTopY * 64 /*0x40*/ : y * 64 /*0x40*/;
          }
          rectangle = furniture.boundingBox.Value;
          if (rectangle.Contains(x, y1))
          {
            furniture.AttemptRemoval((Action<Furniture>) (f =>
            {
              Guid job = this.furniture.GuidOf(f);
              if (this.furnitureToRemove.Contains(job))
                return;
              this.furnitureToRemove.Add(job);
            }));
            return true;
          }
        }
      }
    }
    for (int index = this.furniture.Count - 1; index >= 0; --index)
    {
      Furniture furniture = this.furniture[index];
      if ((this.CanFreePlaceFurniture() || furniture.IsCloseEnoughToFarmer(who)) && furniture.isPassable())
      {
        rectangle = furniture.boundingBox.Value;
        if (rectangle.Contains(x, y) && furniture.canBeRemoved(who))
        {
          furniture.AttemptRemoval((Action<Furniture>) (f =>
          {
            Guid job = this.furniture.GuidOf(f);
            if (this.furnitureToRemove.Contains(job))
              return;
            this.furnitureToRemove.Add(job);
          }));
          return true;
        }
      }
    }
    Microsoft.Xna.Framework.Rectangle rect = new Microsoft.Xna.Framework.Rectangle(x * 64 /*0x40*/, y * 64 /*0x40*/, 64 /*0x40*/, 64 /*0x40*/);
    return Game1.didPlayerJustRightClick(true) && this.CheckInspectAnimal(rect, who);
  }

  [Obsolete("These values returned by this function are no longer used by the game (except for rare, backwards compatibility related cases.) Check DecoratableLocation's wallpaper/flooring related functionality instead.")]
  public virtual List<Microsoft.Xna.Framework.Rectangle> getWalls() => new List<Microsoft.Xna.Framework.Rectangle>();

  protected virtual void removeQueuedFurniture(Guid guid)
  {
    Farmer player = Game1.player;
    Furniture furniture;
    if (!this.furniture.TryGetValue(guid, out furniture) || !player.couldInventoryAcceptThisItem((Item) furniture))
      return;
    furniture.performRemoveAction();
    this.furniture.Remove(guid);
    bool flag = false;
    for (int index = 0; index < 12; ++index)
    {
      if (player.Items[index] == null)
      {
        player.Items[index] = (Item) furniture;
        player.CurrentToolIndex = index;
        flag = true;
        break;
      }
    }
    if (!flag)
    {
      Item inventory = player.addItemToInventory((Item) furniture, 11);
      player.addItemToInventory(inventory);
      player.CurrentToolIndex = 11;
    }
    this.localSound("coin");
  }

  public virtual bool leftClick(int x, int y, Farmer who)
  {
    Vector2 key = new Vector2((float) (x / 64 /*0x40*/), (float) (y / 64 /*0x40*/));
    foreach (Building building in this.buildings)
    {
      if (building.CanLeftClick(x, y) && building.leftClicked())
        return true;
    }
    Object @object;
    if (!this.objects.TryGetValue(key, out @object) || !@object.clicked(who))
      return false;
    this.objects.Remove(key);
    return true;
  }

  public virtual bool shouldShadowBeDrawnAboveBuildingsLayer(Vector2 p)
  {
    TerrainFeature terrainFeature;
    if (this.doesTileHaveProperty((int) p.X, (int) p.Y, "Passable", "Buildings") != null || this.terrainFeatures.TryGetValue(p, out terrainFeature) && terrainFeature is HoeDirt)
      return true;
    if (this.isWaterTile((int) p.X, (int) p.Y))
    {
      int tileIndexAt = this.getTileIndexAt((int) p.X, (int) p.Y, "Buildings", "Town");
      if (tileIndexAt < 1004 || tileIndexAt > 1013)
        return true;
    }
    foreach (Building building in this.buildings)
    {
      if (building.occupiesTile(p) && building.isTilePassable(p))
        return true;
    }
    return false;
  }

  /// <summary>Get the fridge that's part of this map, if it has one.</summary>
  /// <param name="onlyUnlocked">Whether to only return the fridge if it's available to the player (e.g. they've unlocked the required house upgrade).</param>
  public virtual Chest GetFridge(bool onlyUnlocked = true)
  {
    switch (this)
    {
      case FarmHouse farmHouse:
        if (!onlyUnlocked || farmHouse.fridgePosition != Point.Zero)
          return farmHouse.fridge.Value;
        break;
      case IslandFarmHouse islandFarmHouse:
        if (!onlyUnlocked || islandFarmHouse.fridgePosition != Point.Zero)
          return islandFarmHouse.fridge.Value;
        break;
    }
    return (Chest) null;
  }

  /// <summary>Get the tile position of the fridge that's part of this map, if it has one and it's available to the player (e.g. they've unlocked the required house upgrade).</summary>
  public virtual Point? GetFridgePosition()
  {
    switch (this)
    {
      case FarmHouse farmHouse:
        if (farmHouse.fridgePosition != Point.Zero)
          return new Point?(farmHouse.fridgePosition);
        break;
      case IslandFarmHouse islandFarmHouse:
        if (islandFarmHouse.fridgePosition != Point.Zero)
          return new Point?(islandFarmHouse.fridgePosition);
        break;
    }
    return new Point?();
  }

  /// <summary>Open the cooking menu, with ingredients available from any <see cref="M:StardewValley.GameLocation.GetFridge(System.Boolean)" /> or mini-fridges in the location.</summary>
  public void ActivateKitchen()
  {
    List<NetMutex> mutexes = new List<NetMutex>();
    List<Chest> mini_fridges = new List<Chest>();
    foreach (Object @object in this.objects.Values)
    {
      if (@object != null && @object.bigCraftable.Value && @object is Chest chest && chest.fridge.Value)
      {
        mini_fridges.Add(chest);
        mutexes.Add(chest.mutex);
      }
    }
    Chest fridge = this.GetFridge();
    if (fridge != null)
      mutexes.Add(fridge.mutex);
    MultipleMutexRequest multipleMutexRequest = new MultipleMutexRequest(mutexes, (Action<MultipleMutexRequest>) (request =>
    {
      List<IInventory> materialContainers = new List<IInventory>();
      if (fridge != null)
        materialContainers.Add((IInventory) fridge.Items);
      foreach (Chest chest in mini_fridges)
        materialContainers.Add((IInventory) chest.Items);
      Vector2 centeringOnScreen = Utility.getTopLeftPositionForCenteringOnScreen(800 + IClickableMenu.borderWidth * 2, 600 + IClickableMenu.borderWidth * 2);
      Game1.activeClickableMenu = (IClickableMenu) new CraftingPage((int) centeringOnScreen.X, (int) centeringOnScreen.Y, 800 + IClickableMenu.borderWidth * 2, 600 + IClickableMenu.borderWidth * 2, true, true, materialContainers);
      Game1.activeClickableMenu.exitFunction = new IClickableMenu.onExit(request.ReleaseLocks);
    }), (Action<MultipleMutexRequest>) (request => Game1.showRedMessage(Game1.content.LoadString("Strings\\UI:Kitchen_InUse"))));
  }

  public void openDoor(Location tileLocation, bool playSound)
  {
    try
    {
      int tileIndexAt = this.getTileIndexAt(tileLocation.X, tileLocation.Y, "Buildings", "1");
      Point key = new Point(tileLocation.X, tileLocation.Y);
      if (!this.interiorDoors.ContainsKey(key))
        return;
      this.interiorDoors[key] = true;
      if (!playSound)
        return;
      Vector2 vector2 = new Vector2((float) tileLocation.X, (float) tileLocation.Y);
      if (tileIndexAt == 120)
        this.playSound("doorOpen", new Vector2?(vector2));
      else
        this.playSound("doorCreak", new Vector2?(vector2));
    }
    catch (Exception ex)
    {
    }
  }

  public void doStarpoint(string which)
  {
    switch (which)
    {
      case "3":
        if (Game1.player.ActiveObject == null || !(Game1.player.ActiveObject.QualifiedItemId == "(O)307"))
          break;
        Object o1 = ItemRegistry.Create<Object>("(BC)161");
        if (!Game1.player.couldInventoryAcceptThisItem((Item) o1) && Game1.player.ActiveObject.stack.Value > 1)
        {
          Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Crop.cs.588"));
          break;
        }
        Game1.player.reduceActiveItemByOne();
        Game1.player.makeThisTheActiveObject(o1);
        this.localSound("discoverMineral");
        Game1.flashAlpha = 1f;
        break;
      case "4":
        if (Game1.player.ActiveObject == null || !(Game1.player.ActiveObject.QualifiedItemId == "(O)203"))
          break;
        Object o2 = ItemRegistry.Create<Object>("(BC)162");
        if (!Game1.player.couldInventoryAcceptThisItem((Item) o2) && Game1.player.ActiveObject.stack.Value > 1)
        {
          Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Crop.cs.588"));
          break;
        }
        Game1.player.reduceActiveItemByOne();
        Game1.player.makeThisTheActiveObject(o2);
        this.localSound("croak");
        Game1.flashAlpha = 1f;
        break;
    }
  }

  public virtual string FormatCompletionLine(Func<Farmer, float> check)
  {
    KeyValuePair<Farmer, float> farmCompletion = Utility.GetFarmCompletion(check);
    return farmCompletion.Key == Game1.player ? farmCompletion.Value.ToString() : $"({farmCompletion.Key.Name}) {farmCompletion.Value.ToString()}";
  }

  public virtual string FormatCompletionLine(
    Func<Farmer, bool> check,
    string true_value,
    string false_value)
  {
    KeyValuePair<Farmer, bool> farmCompletion = Utility.GetFarmCompletion(check);
    if (farmCompletion.Key != Game1.player)
      return $"({farmCompletion.Key.Name}) {(farmCompletion.Value ? true_value : false_value)}";
    return !farmCompletion.Value ? false_value : true_value;
  }

  public virtual void ShowQiCat()
  {
    if (Game1.MasterPlayer.mailReceived.Contains("Farm_Eternal") && !Game1.MasterPlayer.mailReceived.Contains("GotPerfectionStatue"))
    {
      Game1.player.team.RequestSetMail(PlayerActionTarget.Host, "GotPerfectionStatue", MailType.Received, true);
      Game1.player.addItemByMenuIfNecessaryElseHoldUp(ItemRegistry.Create("(BC)280"));
    }
    else
    {
      if (!Game1.player.hasOrWillReceiveMail("FizzIntro"))
        Game1.addMailForTomorrow("FizzIntro", sendToEveryone: true);
      Game1.playSound("qi_shop");
      int perfectionWaivers = Game1.netWorldState.Value.PerfectionWaivers;
      double sub1 = Math.Floor((double) Utility.percentGameComplete() * 100.0);
      if (Game1.content.GetCurrentLanguage() == LocalizedContentManager.LanguageCode.ja || Game1.content.GetCurrentLanguage() == LocalizedContentManager.LanguageCode.ko || Game1.content.GetCurrentLanguage() == LocalizedContentManager.LanguageCode.zh)
        sub1 += (double) perfectionWaivers;
      string str;
      switch (perfectionWaivers)
      {
        case 0:
          str = Game1.content.LoadString("Strings\\UI:PT_Total_Value", (object) sub1);
          break;
        case 1:
          str = Game1.content.LoadString("Strings\\UI:PT_Total_ValueWithWaiver", (object) sub1);
          break;
        default:
          str = Game1.content.LoadString("Strings\\UI:PT_Total_ValueWithWaivers", (object) sub1, (object) perfectionWaivers);
          break;
      }
      string[] strArray1 = new string[15]
      {
        Utility.loadStringShort("UI", "PT_Title") + "^",
        "----------------^",
        $"{Utility.loadStringShort("UI", "PT_Shipped")}: {this.FormatCompletionLine((Func<Farmer, float>) (farmer => (float) Math.Floor((double) Utility.getFarmerItemsShippedPercent(farmer) * 100.0)))}%^",
        $"{Utility.loadStringShort("UI", "PT_Obelisks")}: {Math.Min(Utility.GetObeliskTypesBuilt(), 4).ToString()}/4^",
        $"{Utility.loadStringShort("UI", "PT_GoldClock")}: {(Game1.IsBuildingConstructed("Gold Clock") ? Game1.content.LoadString("Strings\\Lexicon:QuestionDialogue_Yes") : Game1.content.LoadString("Strings\\Lexicon:QuestionDialogue_No"))}^",
        $"{Utility.loadStringShort("UI", "PT_MonsterSlayer")}: {this.FormatCompletionLine((Func<Farmer, bool>) (farmer => farmer.hasCompletedAllMonsterSlayerQuests.Value), Game1.content.LoadString("Strings\\Lexicon:QuestionDialogue_Yes"), Game1.content.LoadString("Strings\\Lexicon:QuestionDialogue_No"))}^",
        $"{Utility.loadStringShort("UI", "PT_GreatFriends")}: {this.FormatCompletionLine((Func<Farmer, float>) (farmer => (float) Math.Floor((double) Utility.getMaxedFriendshipPercent(farmer) * 100.0)))}%^",
        $"{Utility.loadStringShort("UI", "PT_FarmerLevel")}: {this.FormatCompletionLine((Func<Farmer, float>) (farmer => (float) Math.Min(farmer.Level, 25)))}/25^",
        $"{Utility.loadStringShort("UI", "PT_Stardrops")}: {this.FormatCompletionLine((Func<Farmer, bool>) (farmer => Utility.foundAllStardrops(farmer)), Game1.content.LoadString("Strings\\Lexicon:QuestionDialogue_Yes"), Game1.content.LoadString("Strings\\Lexicon:QuestionDialogue_No"))}^",
        $"{Utility.loadStringShort("UI", "PT_Cooking")}: {this.FormatCompletionLine((Func<Farmer, float>) (farmer => (float) Math.Floor((double) Utility.getCookedRecipesPercent(farmer) * 100.0)))}%^",
        $"{Utility.loadStringShort("UI", "PT_Crafting")}: {this.FormatCompletionLine((Func<Farmer, float>) (farmer => (float) Math.Floor((double) Utility.getCraftedRecipesPercent(farmer) * 100.0)))}%^",
        $"{Utility.loadStringShort("UI", "PT_Fish")}: {this.FormatCompletionLine((Func<Farmer, float>) (farmer => (float) Math.Floor((double) Utility.getFishCaughtPercent(farmer) * 100.0)))}%^",
        null,
        null,
        null
      };
      string[] strArray2 = new string[6]
      {
        Utility.loadStringShort("UI", "PT_GoldenWalnut"),
        ": ",
        Math.Min(Game1.netWorldState.Value.GoldenWalnutsFound, 130).ToString(),
        "/",
        null,
        null
      };
      int index1 = 130;
      strArray2[4] = index1.ToString();
      strArray2[5] = "^";
      strArray1[12] = string.Concat(strArray2);
      strArray1[13] = "----------------^";
      strArray1[14] = $"{Utility.loadStringShort("UI", "PT_Total")}: {str}";
      List<string> sectionsOfHeight = SpriteText.getStringBrokenIntoSectionsOfHeight(string.Concat(strArray1), 9999, Game1.uiViewport.Height - 100);
      for (int index2 = 0; index2 < sectionsOfHeight.Count - 1; ++index2)
      {
        List<string> stringList = sectionsOfHeight;
        index1 = index2;
        stringList[index1] += "...\n";
      }
      Game1.drawDialogueNoTyping(sectionsOfHeight);
    }
  }

  /// <summary>Search a garbage can for a player if they haven't searched it today, and give or drop the resulting item (if any).</summary>
  /// <param name="id">The unique ID for the garbage can to search.</param>
  /// <param name="tile">The tile position for the garbage can being searched.</param>
  /// <param name="who">The player performing the search.</param>
  /// <param name="playAnimations">Whether to play animations and sounds.</param>
  /// <param name="reactNpcs">Whether nearby NPCs should react to the search (e.g. friendship point impact or dialogue).</param>
  /// <param name="logError">Log an error if the search fails due to invalid data, or <c>null</c> to fail silently.</param>
  /// <returns>Returns whether the garbage can was searched successfully, regardless of whether an item was found.</returns>
  public virtual bool CheckGarbage(
    string id,
    Vector2 tile,
    Farmer who,
    bool playAnimations = true,
    bool reactNpcs = true,
    Action<string> logError = null)
  {
    if (string.IsNullOrWhiteSpace(id))
    {
      if (logError != null)
        logError("must specify a garbage can ID");
      return false;
    }
    if (id != null && id.Length == 1)
    {
      switch (id[0])
      {
        case '0':
          id = "JodiAndKent";
          break;
        case '1':
          id = "EmilyAndHaley";
          break;
        case '2':
          id = "Mayor";
          break;
        case '3':
          id = "Museum";
          break;
        case '4':
          id = "Blacksmith";
          break;
        case '5':
          id = "Saloon";
          break;
        case '6':
          id = "Evelyn";
          break;
        case '7':
          id = "JojaMart";
          break;
      }
    }
    if (!Game1.netWorldState.Value.CheckedGarbage.Add(id))
    {
      Game1.haltAfterCheck = false;
      return true;
    }
    Item obj;
    GarbageCanItemData selected;
    Random garbageRandom;
    this.TryGetGarbageItem(id, who.DailyLuck, out obj, out selected, out garbageRandom, logError);
    if (playAnimations)
    {
      bool flag1 = selected != null && selected.IsDoubleMegaSuccess;
      bool flag2 = !flag1 && selected != null && selected.IsMegaSuccess;
      if (flag1)
        this.playSound("explosion");
      else if (flag2)
        this.playSound("crit");
      this.playSound("trashcan");
      int tileY = (int) tile.Y;
      int num = this.GetSeasonIndex() * 17;
      TemporaryAnimatedSprite temporaryAnimatedSprite1 = new TemporaryAnimatedSprite("LooseSprites\\Cursors2", new Microsoft.Xna.Framework.Rectangle(22 + num, 0, 16 /*0x10*/, 10), tile * 64f + new Vector2(0.0f, -6f) * 4f, false, 0.0f, Color.White)
      {
        interval = flag1 ? 4000f : 1000f,
        motion = flag1 ? new Vector2(4f, -20f) : new Vector2(0.0f, (float) ((flag2 ? -7.0 : (double) (garbageRandom.Next(-1, 3) + (garbageRandom.NextDouble() < 0.1 ? -2 : 0))) - 8.0)),
        rotationChange = flag1 ? 0.4f : 0.0f,
        acceleration = new Vector2(0.0f, 0.7f),
        yStopCoordinate = tileY * 64 /*0x40*/ - 24,
        layerDepth = flag1 ? 1f : (float) ((tileY + 1) * 64 /*0x40*/ + 2) / 10000f,
        scale = 4f,
        Parent = this,
        shakeIntensity = flag1 ? 0.0f : 1f,
        reachedStopCoordinate = (TemporaryAnimatedSprite.endBehavior) (x =>
        {
          this.removeTemporarySpritesWithID(97654);
          this.playSound("thudStep");
          for (int index = 0; index < 3; ++index)
            this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(372, 1956, 10, 10), tile * 64f + new Vector2((float) (index * 6), (float) (garbageRandom.Next(3) - 3)) * 4f, false, 0.02f, Color.DimGray)
            {
              alpha = 0.85f,
              motion = new Vector2((float) ((double) index * 0.30000001192092896 - 0.60000002384185791), -1f),
              acceleration = new Vector2(1f / 500f, 0.0f),
              interval = 99999f,
              layerDepth = (float) ((tileY + 1) * 64 /*0x40*/ + 3) / 10000f,
              scale = 3f,
              scaleChange = 0.02f,
              rotationChange = (float) ((double) garbageRandom.Next(-5, 6) * 3.1415927410125732 / 256.0),
              delayBeforeAnimationStart = 50
            });
        }),
        id = 97654
      };
      TemporaryAnimatedSprite temporaryAnimatedSprite2 = new TemporaryAnimatedSprite("LooseSprites\\Cursors2", new Microsoft.Xna.Framework.Rectangle(22 + num, 11, 16 /*0x10*/, 16 /*0x10*/), tile * 64f + new Vector2(0.0f, -5f) * 4f, false, 0.0f, Color.White)
      {
        interval = flag1 ? 999999f : 1000f,
        layerDepth = (float) ((tileY + 1) * 64 /*0x40*/ + 1) / 10000f,
        scale = 4f,
        id = 97654
      };
      if (flag1)
        temporaryAnimatedSprite1.reachedStopCoordinate = new TemporaryAnimatedSprite.endBehavior(temporaryAnimatedSprite1.bounce);
      TemporaryAnimatedSpriteList sprites = new TemporaryAnimatedSpriteList()
      {
        temporaryAnimatedSprite1,
        temporaryAnimatedSprite2
      };
      for (int index = 0; index < 5; ++index)
      {
        TemporaryAnimatedSprite temporaryAnimatedSprite3 = new TemporaryAnimatedSprite("LooseSprites\\Cursors2", new Microsoft.Xna.Framework.Rectangle(22 + garbageRandom.Next(4) * 4, 32 /*0x20*/, 4, 4), tile * 64f + new Vector2((float) Game1.random.Next(13), (float) (Game1.random.Next(3) - 3)) * 4f, false, 0.0f, Color.White)
        {
          interval = 500f,
          motion = new Vector2((float) garbageRandom.Next(-2, 3), -5f),
          acceleration = new Vector2(0.0f, 0.4f),
          layerDepth = (float) ((tileY + 1) * 64 /*0x40*/ + 3) / 10000f,
          scale = 4f,
          color = Utility.getRandomRainbowColor(garbageRandom),
          delayBeforeAnimationStart = garbageRandom.Next(100)
        };
        sprites.Add(temporaryAnimatedSprite3);
      }
      Game1.multiplayer.broadcastSprites(this, sprites);
    }
    if (reactNpcs)
    {
      foreach (NPC npc in Utility.GetNpcsWithinDistance(tile, 7, this))
      {
        if (!(npc is Horse))
        {
          Game1.multiplayer.globalChatInfoMessage("TrashCan", who.Name, npc.GetTokenizedDisplayName());
          if (npc.Name == "Linus")
            Game1.multiplayer.globalChatInfoMessage("LinusTrashCan");
          CharacterData data = npc.GetData();
          int amount = data != null ? data.DumpsterDiveFriendshipEffect : -25;
          int? nullable = (int?) data?.DumpsterDiveEmote;
          Dialogue dialogue1 = npc.TryGetDialogue("DumpsterDiveComment");
          Dialogue dialogue2;
          switch (npc.Age)
          {
            case 1:
              nullable = new int?(nullable ?? 8);
              dialogue2 = dialogue1 ?? new Dialogue(npc, "Data\\ExtraDialogue:Town_DumpsterDiveComment_Teen");
              break;
            case 2:
              nullable = new int?(nullable ?? 28);
              dialogue2 = dialogue1 ?? new Dialogue(npc, "Data\\ExtraDialogue:Town_DumpsterDiveComment_Child");
              break;
            default:
              nullable = new int?(nullable ?? 12);
              dialogue2 = dialogue1 ?? new Dialogue(npc, "Data\\ExtraDialogue:Town_DumpsterDiveComment_Adult");
              break;
          }
          npc.doEmote(nullable.Value);
          who.changeFriendship(amount, npc);
          npc.setNewDialogue(dialogue2, true, true);
          Game1.drawDialogue(npc);
          break;
        }
      }
    }
    int num1 = (int) Game1.stats.Increment("trashCansChecked");
    if (selected != null)
    {
      if (selected.AddToInventoryDirectly)
      {
        who.addItemByMenuIfNecessary(obj);
      }
      else
      {
        Vector2 pixelOrigin = new Vector2(tile.X + 0.5f, tile.Y - 1f) * 64f;
        if (selected.CreateMultipleDebris)
          Game1.createMultipleItemDebris(obj, pixelOrigin, 2, this, (int) pixelOrigin.Y + 64 /*0x40*/);
        else
          Game1.createItemDebris(obj, pixelOrigin, 2, this, (int) pixelOrigin.Y + 64 /*0x40*/);
      }
    }
    return true;
  }

  /// <summary>Try to get the item that would be produced by checking a garbage can in the location, without marking it checked or playing animations or sounds.</summary>
  /// <param name="id">The garbage can ID in <c>Data/GarbageCans</c>.</param>
  /// <param name="dailyLuck">The daily luck of the player checking the garbage can.</param>
  /// <param name="item">The item produced by the garbage can, if any.</param>
  /// <param name="selected">The data entry which produced the <paramref name="item" />, if applicable.</param>
  /// <param name="garbageRandom">The RNG used to select the item, and which would normally be used for subsequent effects like animations.</param>
  /// <param name="logError">Log an error if the search fails due to invalid data, or <c>null</c> to fail silently.</param>
  /// <returns>Returns whether an item was produced.</returns>
  public virtual bool TryGetGarbageItem(
    string id,
    double dailyLuck,
    out Item item,
    out GarbageCanItemData selected,
    out Random garbageRandom,
    Action<string> logError = null)
  {
    GarbageCanData garbageCanData = DataLoader.GarbageCans(Game1.content);
    GarbageCanEntryData valueOrDefault = garbageCanData.GarbageCans.GetValueOrDefault<string, GarbageCanEntryData>(id);
    float num1 = (valueOrDefault == null || (double) valueOrDefault.BaseChance <= 0.0 ? garbageCanData.DefaultBaseChance : valueOrDefault.BaseChance) + (float) dailyLuck;
    if (Game1.player.stats.Get("Book_Trash") > 0U)
      num1 += 0.2f;
    garbageRandom = Utility.CreateDaySaveRandom((double) (777 + Game1.hash.GetDeterministicHashCode(id)));
    int num2 = garbageRandom.Next(0, 100);
    for (int index = 0; index < num2; ++index)
      garbageRandom.NextDouble();
    int num3 = garbageRandom.Next(0, 100);
    for (int index = 0; index < num3; ++index)
      garbageRandom.NextDouble();
    selected = (GarbageCanItemData) null;
    item = (Item) null;
    bool flag = garbageRandom.NextDouble() < (double) num1;
    ItemQueryContext context = new ItemQueryContext(this, Game1.player, garbageRandom, $"garbage data '{id}'");
    List<GarbageCanItemData>[] garbageCanItemDataListArray = new List<GarbageCanItemData>[3]
    {
      garbageCanData.BeforeAll,
      valueOrDefault?.Items,
      garbageCanData.AfterAll
    };
    foreach (List<GarbageCanItemData> garbageCanItemDataList in garbageCanItemDataListArray)
    {
      if (garbageCanItemDataList != null)
      {
        foreach (GarbageCanItemData data in garbageCanItemDataList)
        {
          if (string.IsNullOrWhiteSpace(data.Id))
            logError("ignored item entry with no Id field.");
          else if ((flag || data.IgnoreBaseChance) && GameStateQuery.CheckConditions(data.Condition, this, random: garbageRandom))
          {
            bool error = false;
            Item obj = ItemQueryResolver.TryResolveRandomItem((ISpawnItemData) data, context, logError: (Action<string, string>) ((query, message) =>
            {
              error = true;
              logError($"failed parsing item query '{query}': {message}");
            }));
            if (!error)
            {
              selected = data;
              item = obj;
              break;
            }
          }
        }
        if (selected != null)
          break;
      }
    }
    return item != null;
  }

  /// <summary>Handle an <c>Action</c> property from a <c>Buildings</c> map tile in the location when the player interacts with the tile.</summary>
  /// <param name="fullActionString">The full action string to parse, <strong>excluding</strong> the <c>Action</c> prefix.</param>
  /// <param name="who">The player performing the action.</param>
  /// <param name="tileLocation">The tile coordinate of the action to handle.</param>
  public virtual bool performAction(string fullActionString, Farmer who, Location tileLocation)
  {
    return fullActionString != null && this.performAction(ArgUtility.SplitBySpace(fullActionString), who, tileLocation);
  }

  /// <summary>Get whether an <c>Action</c> property from a <c>Buildings</c> map tile in the location should be ignored, so it doesn't show an action cursor and isn't triggered on click.</summary>
  /// <param name="action">The action arguments to parse, including the <c>Action</c> prefix.</param>
  /// <param name="who">The player performing the action.</param>
  /// <param name="tileLocation">The tile coordinate of the action to handle.</param>
  public virtual bool ShouldIgnoreAction(string[] action, Farmer who, Location tileLocation)
  {
    string str = ArgUtility.Get(action, 0);
    if (string.IsNullOrWhiteSpace(str))
      return true;
    switch (str)
    {
      case "DropBox":
        if (Game1.player.team.specialOrders != null)
        {
          string box_id = ArgUtility.Get(action, 1);
          if (box_id != null)
          {
            foreach (SpecialOrder specialOrder in Game1.player.team.specialOrders)
            {
              if (specialOrder.UsesDropBox(box_id))
                return false;
            }
          }
        }
        return true;
      case "MonsterGrave":
        return !who.eventsSeen.Contains("6963327");
      default:
        return false;
    }
  }

  /// <summary>Displays a message when a player is not good enough friends with a villager to enter their room.</summary>
  /// <param name="action">The action arguments from <see cref="M:StardewValley.GameLocation.performAction(System.String,StardewValley.Farmer,xTile.Dimensions.Location)" />.</param>
  public virtual void ShowLockedDoorMessage(string[] action)
  {
    Gender gender = Gender.Female;
    string str1 = (string) null;
    string[] strArray = new string[action.Length == 2 ? 1 : 2];
    for (int index = 0; index < strArray.Length; ++index)
    {
      string name = action[index + 1];
      NPC characterFromName = Game1.getCharacterFromName(name);
      if (characterFromName != null)
      {
        str1 = characterFromName.Name;
        gender = characterFromName.Gender;
        strArray[index] = characterFromName.displayName;
      }
      else
      {
        CharacterData data;
        if (!NPC.TryGetData(name, out data))
          return;
        str1 = name;
        gender = data.Gender;
        strArray[index] = TokenParser.ParseText(data.DisplayName);
      }
    }
    string dialogue;
    if (strArray.Length > 1)
    {
      dialogue = Game1.content.LoadString("Strings\\Locations:DoorUnlock_NotFriend_Couple", (object) strArray[0], (object) strArray[1]);
    }
    else
    {
      string sub1 = strArray[0];
      string str2 = Game1.content.LoadStringReturnNullIfNotFound("Strings\\Locations:DoorUnlock_NotFriend_" + str1);
      if (str2 == null)
        str2 = Game1.content.LoadStringReturnNullIfNotFound($"Strings\\Locations:DoorUnlock_NotFriend_{gender}", sub1) ?? Game1.content.LoadString("Strings\\Locations:DoorUnlock_NotFriend_Female", (object) sub1);
      dialogue = str2;
    }
    Game1.drawObjectDialogue(dialogue);
  }

  /// <summary>Handle an <c>Action</c> property from a <c>Buildings</c> map tile in the location when the player interacts with the tile.</summary>
  /// <param name="action">The action arguments to parse, <strong>excluding</strong> the <c>Action</c> prefix.</param>
  /// <param name="who">The player performing the action.</param>
  /// <param name="tileLocation">The tile coordinate of the action to handle.</param>
  public virtual bool performAction(string[] action, Farmer who, Location tileLocation)
  {
    if (this.ShouldIgnoreAction(action, who, tileLocation))
      return false;
    string key1;
    string error;
    if (!ArgUtility.TryGet(action, 0, out key1, out error, name: "string actionType"))
      return LogError(error);
    if (who.IsLocalPlayer)
    {
      Func<GameLocation, string[], Farmer, Point, bool> func;
      if (GameLocation.registeredTileActions.TryGetValue(key1, out func))
        return func(this, action, who, new Point(tileLocation.X, tileLocation.Y));
      if (key1 != null)
      {
        switch (key1.Length)
        {
          case 3:
            if (key1 == "Buy")
            {
              string which;
              if (!ArgUtility.TryGet(action, 1, out which, out error, name: "string which"))
                return LogError(error);
              return who.TilePoint.Y >= tileLocation.Y && this.HandleBuyAction(which);
            }
            goto label_536;
          case 4:
            switch (key1[0])
            {
              case 'C':
                if (key1 == "Crib")
                {
                  foreach (NPC character in this.characters)
                  {
                    if (character is Child child)
                    {
                      switch (child.Age)
                      {
                        case 0:
                          Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\Locations:FarmHouse_Crib_NewbornSleeping", (object) character.displayName)));
                          return true;
                        case 1:
                          child.toss(who);
                          return true;
                        case 2:
                          if (child.isInCrib())
                            return character.checkAction(who, this);
                          continue;
                        default:
                          continue;
                      }
                    }
                  }
                  return false;
                }
                goto label_536;
              case 'D':
                if (key1 == "Door")
                {
                  if (action.Length > 1 && !Game1.eventUp)
                  {
                    for (int index = 1; index < action.Length; ++index)
                    {
                      string name = action[index];
                      string str = "doorUnlock" + name;
                      if (who.getFriendshipHeartLevelForNPC(name) >= 2 || Game1.player.mailReceived.Contains(str))
                      {
                        Rumble.rumble(0.1f, 100f);
                        Game1.player.mailReceived.Add(str);
                        this.openDoor(tileLocation, true);
                        return true;
                      }
                      if (name == "Sebastian" && this.IsGreenRainingHere() && Game1.year == 1)
                      {
                        Rumble.rumble(0.1f, 100f);
                        this.openDoor(tileLocation, true);
                        return true;
                      }
                    }
                    this.ShowLockedDoorMessage(action);
                    goto label_537;
                  }
                  this.openDoor(tileLocation, true);
                  return true;
                }
                goto label_536;
              case 'L':
                if (key1 == "Lamp")
                {
                  if ((double) this.lightLevel.Value == 0.0)
                    this.lightLevel.Value = 0.6f;
                  else
                    this.lightLevel.Value = 0.0f;
                  this.playSound("openBox");
                  goto label_537;
                }
                goto label_536;
              case 'M':
                if (key1 == "Mine")
                  goto label_434;
                goto label_536;
              case 'N':
                if (key1 == "None")
                  return true;
                goto label_536;
              case 'W':
                if (key1 == "Warp")
                {
                  Point point;
                  string locationName;
                  if (!ArgUtility.TryGetPoint(action, 1, out point, out error, "Point tile") || !ArgUtility.TryGet(action, 3, out locationName, out error, name: "string locationName"))
                    return LogError(error);
                  int num = action.Length < 5 ? 1 : 0;
                  who.faceGeneralDirection(new Vector2((float) tileLocation.X, (float) tileLocation.Y) * 64f);
                  Rumble.rumble(0.15f, 200f);
                  if (num != 0)
                    this.playSound("doorClose", new Vector2?(new Vector2((float) tileLocation.X, (float) tileLocation.Y)));
                  Game1.warpFarmer(locationName, point.X, point.Y, false);
                  goto label_537;
                }
                goto label_536;
              case 'Y':
                if (key1 == "Yoba")
                {
                  Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:SeedShop_Yoba"));
                  goto label_537;
                }
                goto label_536;
              default:
                goto label_536;
            }
          case 5:
            switch (key1[0])
            {
              case 'C':
                if (key1 == "Craft")
                {
                  GameLocation.openCraftingMenu();
                  goto label_537;
                }
                goto label_536;
              case 'F':
                if (key1 == "Forge")
                {
                  Game1.activeClickableMenu = (IClickableMenu) new ForgeMenu();
                  return true;
                }
                goto label_536;
              case 'H':
                if (key1 == "HMTGF")
                {
                  if (who.ActiveObject != null && who.ActiveObject.QualifiedItemId == "(O)155")
                  {
                    Object o = ItemRegistry.Create<Object>("(BC)155");
                    if (!Game1.player.couldInventoryAcceptThisItem((Item) o) && Game1.player.ActiveObject.stack.Value > 1)
                    {
                      Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Crop.cs.588"));
                      goto label_537;
                    }
                    Game1.player.reduceActiveItemByOne();
                    Game1.player.makeThisTheActiveObject(o);
                    this.localSound("discoverMineral");
                    Game1.flashAlpha = 1f;
                    goto label_537;
                  }
                  goto label_537;
                }
                goto label_536;
              case 'N':
                if (key1 == "Notes")
                {
                  int which;
                  if (!ArgUtility.TryGetInt(action, 1, out which, out error, "int noteId"))
                    return LogError(error);
                  this.readNote(which);
                  goto label_537;
                }
                goto label_536;
              case 'Q':
                if (key1 == "QiCat")
                {
                  this.ShowQiCat();
                  goto label_537;
                }
                goto label_536;
              default:
                goto label_536;
            }
          case 6:
            switch (key1[0])
            {
              case 'D':
                if (key1 == "DyePot")
                {
                  if (who.eventsSeen.Contains("992559"))
                  {
                    if (!DyeMenu.IsWearingDyeable())
                    {
                      Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\UI:DyePot_NoDyeable"));
                      goto label_537;
                    }
                    Game1.activeClickableMenu = (IClickableMenu) new DyeMenu();
                    goto label_537;
                  }
                  Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:HaleyHouse_DyePot"));
                  goto label_537;
                }
                goto label_536;
              case 'L':
                if (key1 == "Letter")
                {
                  string str;
                  if (!ArgUtility.TryGet(action, 1, out str, out error, name: "string translationKey"))
                    return LogError(error);
                  Game1.drawLetterMessage(Game1.content.LoadString("Strings\\StringsFromMaps:" + str.Replace("\"", "")));
                  goto label_537;
                }
                goto label_536;
              case 'S':
                if (key1 == "Saloon" && who.TilePoint.Y > tileLocation.Y)
                  return this.saloon(tileLocation);
                goto label_536;
              default:
                goto label_536;
            }
          case 7:
            switch (key1[0])
            {
              case 'B':
                if (key1 == "Bobbers")
                {
                  Game1.activeClickableMenu = (IClickableMenu) new ChooseFromIconsMenu("bobbers");
                  goto label_537;
                }
                goto label_536;
              case 'D':
                if (key1 == "DropBox")
                {
                  string box_id;
                  if (!ArgUtility.TryGet(action, 1, out box_id, out error, name: "string box_id"))
                    return LogError(error);
                  int minimum_capacity = 0;
                  foreach (SpecialOrder specialOrder in Game1.player.team.specialOrders)
                  {
                    if (specialOrder.UsesDropBox(box_id))
                      minimum_capacity = Math.Max(minimum_capacity, specialOrder.GetMinimumDropBoxCapacity(box_id));
                  }
                  foreach (SpecialOrder specialOrder in Game1.player.team.specialOrders)
                  {
                    SpecialOrder order = specialOrder;
                    if (order.UsesDropBox(box_id))
                    {
                      order.donateMutex.RequestLock((Action) (() =>
                      {
                        while (order.donatedItems.Count < minimum_capacity)
                          order.donatedItems.Add((Item) null);
                        Game1.activeClickableMenu = (IClickableMenu) new QuestContainerMenu((IList<Item>) order.donatedItems, highlight_method: new InventoryMenu.highlightThisItem(order.HighlightAcceptableItems), stack_capacity_check: new Func<Item, int>(order.GetAcceptCount), on_item_changed: new Action(order.UpdateDonationCounts), on_confirm: new Action(order.ConfirmCompleteDonations));
                      }));
                      return true;
                    }
                  }
                  return false;
                }
                goto label_536;
              case 'G':
                if (key1 == "Garbage")
                {
                  string id;
                  if (!ArgUtility.TryGet(action, 1, out id, out error, name: "string id"))
                    return LogError(error);
                  this.CheckGarbage(id, new Vector2((float) tileLocation.X, (float) tileLocation.Y), who, logError: (Action<string>) (garbageError => Game1.log.Warn($"Ignored invalid 'Action Garbage {id}' property: {garbageError}.")));
                  Game1.haltAfterCheck = false;
                  return true;
                }
                goto label_536;
              case 'J':
                if (key1 == "Jukebox")
                {
                  Game1.activeClickableMenu = (IClickableMenu) new ChooseFromListMenu(Utility.GetJukeboxTracks(Game1.player, Game1.player.currentLocation), new ChooseFromListMenu.actionOnChoosingListOption(ChooseFromListMenu.playSongAction), true);
                  goto label_537;
                }
                goto label_536;
              case 'K':
                if (key1 == "Kitchen")
                  break;
                goto label_536;
              case 'M':
                switch (key1)
                {
                  case "Message":
                    goto label_367;
                  case "Mailbox":
                    if (this is Farm && this.getBuildingAt(new Vector2((float) tileLocation.X, (float) tileLocation.Y))?.GetIndoors() is FarmHouse indoors && !indoors.IsOwnedByCurrentPlayer)
                    {
                      Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Farm_OtherPlayerMailbox"));
                      goto label_537;
                    }
                    this.mailbox();
                    goto label_537;
                  default:
                    goto label_536;
                }
              case 'Q':
                if (key1 == "QiCoins")
                {
                  if (who.clubCoins > 0)
                  {
                    Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Club_QiCoins", (object) who.clubCoins));
                    goto label_537;
                  }
                  this.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:Club_QiCoins_BuyStarter"), this.createYesNoResponses(), "BuyClubCoins");
                  goto label_537;
                }
                goto label_536;
              case 'k':
                if (key1 == "kitchen")
                  break;
                goto label_536;
              default:
                goto label_536;
            }
            this.ActivateKitchen();
            return true;
          case 8:
            switch (key1[2])
            {
              case 'a':
                if (key1 == "Dialogue")
                {
                  string text;
                  if (!ArgUtility.TryGetRemainder(action, 1, out text, out error, name: "string dialogue"))
                    return LogError(error);
                  Game1.drawDialogueNoTyping(TokenParser.ParseText(text));
                  goto label_537;
                }
                goto label_536;
              case 'e':
                if (key1 == "OpenShop")
                {
                  string str1;
                  string str2;
                  int num1;
                  int num2;
                  int x;
                  int y;
                  int width;
                  int height;
                  if (!ArgUtility.TryGet(action, 1, out str1, out error, name: "string shopId") || !ArgUtility.TryGetOptional(action, 2, out str2, out error, name: "string direction") || !ArgUtility.TryGetOptionalInt(action, 3, out num1, out error, -1, "int openTime") || !ArgUtility.TryGetOptionalInt(action, 4, out num2, out error, -1, "int closeTime") || !ArgUtility.TryGetOptionalInt(action, 5, out x, out error, -1, "int shopAreaX") || !ArgUtility.TryGetOptionalInt(action, 6, out y, out error, -1, "int shopAreaY") || !ArgUtility.TryGetOptionalInt(action, 7, out width, out error, -1, "int shopAreaWidth") || !ArgUtility.TryGetOptionalInt(action, 8, out height, out error, -1, "int shopAreaHeight"))
                    return LogError(error);
                  Microsoft.Xna.Framework.Rectangle? nullable = new Microsoft.Xna.Framework.Rectangle?();
                  if (x != -1 || y != -1 || width != -1 || height != -1)
                  {
                    if (x == -1 || y == -1 || width == -1 || height == -1)
                      return LogError("when specifying any of the shop area 'x y width height' arguments (indexes 5-8), all four must be specified");
                    nullable = new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(x, y, width, height));
                  }
                  switch (str2)
                  {
                    case "down":
                      if (who.TilePoint.Y < tileLocation.Y)
                        return false;
                      break;
                    case "up":
                      if (who.TilePoint.Y > tileLocation.Y)
                        return false;
                      break;
                    case "left":
                      if (who.TilePoint.X > tileLocation.X)
                        return false;
                      break;
                    case "right":
                      if (who.TilePoint.X < tileLocation.X)
                        return false;
                      break;
                  }
                  if (num1 >= 0 && Game1.timeOfDay < num1 || num2 >= 0 && Game1.timeOfDay >= num2)
                    return false;
                  string shopId = str1;
                  Microsoft.Xna.Framework.Rectangle? ownerArea = nullable;
                  bool flag = !nullable.HasValue;
                  int? maxOwnerY = new int?();
                  int num3 = flag ? 1 : 0;
                  return Utility.TryOpenShopMenu(shopId, this, ownerArea, maxOwnerY, num3 != 0);
                }
                goto label_536;
              case 'g':
                if (key1 == "MagicInk")
                {
                  if (who.mailReceived.Add("hasPickedUpMagicInk"))
                  {
                    who.hasMagicInk = true;
                    this.setMapTile(4, 11, 113, "Buildings", "untitled tile sheet");
                    who.addItemByMenuIfNecessaryElseHoldUp((Item) new SpecialItem(7));
                    goto label_537;
                  }
                  goto label_537;
                }
                goto label_536;
              case 'i':
                if (key1 == "ExitMine")
                {
                  this.createQuestionDialogue(" ", new Response[3]
                  {
                    new Response("Leave", Game1.content.LoadString("Strings\\Locations:Mines_LeaveMine")),
                    new Response("Go", Game1.content.LoadString("Strings\\Locations:Mines_GoUp")),
                    new Response("Do", Game1.content.LoadString("Strings\\Locations:Mines_DoNothing"))
                  }, "ExitMine");
                  goto label_537;
                }
                goto label_536;
              case 'j':
                if (key1 == "JojaShop")
                {
                  Utility.TryOpenShopMenu("Joja", (string) null);
                  goto label_537;
                }
                goto label_536;
              case 'n':
                if (key1 == "MineSign")
                {
                  string text;
                  if (!ArgUtility.TryGetRemainder(action, 1, out text, out error, name: "string dialogue"))
                    return LogError(error);
                  Game1.drawObjectDialogue(Game1.parseText(text));
                  goto label_537;
                }
                goto label_536;
              case 't':
                if (key1 == "Tutorial")
                {
                  Game1.activeClickableMenu = (IClickableMenu) new TutorialMenu();
                  goto label_537;
                }
                goto label_536;
              case 'u':
                if (key1 == "ClubShop")
                {
                  Utility.TryOpenShopMenu("Casino", (string) null);
                  goto label_537;
                }
                goto label_536;
              default:
                goto label_536;
            }
          case 9:
            switch (key1[4])
            {
              case 'C':
                if (key1 == "ClubCards")
                  break;
                goto label_536;
              case 'E':
                if (key1 == "PlayEvent")
                {
                  string eventId;
                  bool checkPreconditions;
                  bool checkSeen;
                  string fullActionString;
                  if (!ArgUtility.TryGet(action, 1, out eventId, out error, name: "string eventId") || !ArgUtility.TryGetOptionalBool(action, 2, out checkPreconditions, out error, true, "bool checkPreconditions") || !ArgUtility.TryGetOptionalBool(action, 3, out checkSeen, out error, true, "bool checkSeen") || !ArgUtility.TryGetOptionalRemainder(action, 4, out fullActionString))
                    return LogError(error);
                  if (Game1.PlayEvent(eventId, checkPreconditions, checkSeen))
                    return true;
                  return fullActionString != null && this.performAction(fullActionString, who, tileLocation);
                }
                goto label_536;
              case 'S':
                switch (key1)
                {
                  case "playSound":
                    string audioName;
                    if (!ArgUtility.TryGet(action, 1, out audioName, out error, name: "string audioName"))
                      return LogError(error);
                    this.localSound(audioName);
                    goto label_537;
                  case "ClubSlots":
                    Game1.currentMinigame = (IMinigame) new Slots();
                    goto label_537;
                  default:
                    goto label_536;
                }
              case 'a':
                if (key1 == "LeoParrot")
                {
                  if (this.getTemporarySpriteByID(5858585) is EmilysParrot temporarySpriteById)
                  {
                    temporarySpriteById.doAction();
                    goto label_537;
                  }
                  goto label_537;
                }
                goto label_536;
              case 'b':
                if (key1 == "Billboard")
                {
                  Game1.activeClickableMenu = (IClickableMenu) new Billboard(ArgUtility.Get(action, 1) == "3");
                  goto label_537;
                }
                goto label_536;
              case 'e':
                if (key1 == "Carpenter" && who.TilePoint.Y > tileLocation.Y)
                  return this.carpenters(tileLocation);
                goto label_536;
              case 'k':
                if (key1 == "BlackJack")
                  break;
                goto label_536;
              case 'l':
                if (key1 == "SkullDoor")
                {
                  if (who.hasSkullKey || Utility.IsPassiveFestivalDay("DesertFestival"))
                  {
                    if (!who.hasUnlockedSkullDoor && !Utility.IsPassiveFestivalDay("DesertFestival"))
                    {
                      Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\Locations:SkullCave_SkullDoor_Unlock")));
                      DelayedAction.playSoundAfterDelay("openBox", 500);
                      DelayedAction.playSoundAfterDelay("openBox", 700);
                      Game1.addMailForTomorrow("skullCave");
                      who.hasUnlockedSkullDoor = true;
                      who.completeQuest("19");
                      goto label_537;
                    }
                    who.completelyStopAnimatingOrDoingAction();
                    this.playSound("doorClose");
                    DelayedAction.playSoundAfterDelay("stairsdown", 500, this);
                    Game1.enterMine(121);
                    MineShaft.numberOfCraftedStairsUsedThisRun = 0;
                    goto label_537;
                  }
                  Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:SkullCave_SkullDoor_Locked"));
                  goto label_537;
                }
                goto label_536;
              case 'm':
                if (key1 == "QiGemShop")
                  return Utility.TryOpenShopMenu("QiGemShop", (string) null);
                goto label_536;
              case 'o':
                if (key1 == "Tailoring")
                {
                  if (who.eventsSeen.Contains("992559"))
                  {
                    Game1.activeClickableMenu = (IClickableMenu) new TailoringMenu();
                    goto label_537;
                  }
                  Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:HaleyHouse_SewingMachine"));
                  goto label_537;
                }
                goto label_536;
              case 'p':
                if (key1 == "Starpoint")
                {
                  string which;
                  if (!ArgUtility.TryGet(action, 1, out which, out error, name: "string which"))
                    return LogError(error);
                  this.doStarpoint(which);
                  goto label_537;
                }
                goto label_536;
              case 't':
                if (key1 == "DogStatue")
                {
                  if (GameLocation.canRespec(0) || GameLocation.canRespec(3) || GameLocation.canRespec(2) || GameLocation.canRespec(4) || GameLocation.canRespec(1))
                  {
                    this.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:Sewer_DogStatue"), this.createYesNoResponses(), "dogStatue");
                    goto label_537;
                  }
                  string str = Game1.content.LoadString("Strings\\Locations:Sewer_DogStatue");
                  Game1.drawObjectDialogue(str.Substring(0, str.LastIndexOf('^')));
                  goto label_537;
                }
                goto label_536;
              default:
                goto label_536;
            }
            if (ArgUtility.Get(action, 1) == "1000")
            {
              this.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:Club_CalicoJack_HS"), new Response[2]
              {
                new Response("Play", Game1.content.LoadString("Strings\\Locations:Club_CalicoJack_Play")),
                new Response("Leave", Game1.content.LoadString("Strings\\Locations:Club_CalicoJack_Leave"))
              }, "CalicoJackHS");
              goto label_537;
            }
            this.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:Club_CalicoJack"), new Response[3]
            {
              new Response("Play", Game1.content.LoadString("Strings\\Locations:Club_CalicoJack_Play")),
              new Response("Leave", Game1.content.LoadString("Strings\\Locations:Club_CalicoJack_Leave")),
              new Response("Rules", Game1.content.LoadString("Strings\\Locations:Club_CalicoJack_Rules"))
            }, "CalicoJack");
            goto label_537;
          case 10:
            switch (key1[0])
            {
              case 'A':
                if (key1 == "AnimalShop" && who.TilePoint.Y > tileLocation.Y)
                  return this.animalShop(tileLocation);
                goto label_536;
              case 'B':
                switch (key1)
                {
                  case "Bookseller":
                    if (Utility.getDaysOfBooksellerThisSeason().Contains(Game1.dayOfMonth))
                    {
                      if (Game1.player.mailReceived.Contains("read_a_book"))
                      {
                        this.createQuestionDialogue(Game1.content.LoadString("Strings\\1_6_Strings:books_welcome"), new Response[3]
                        {
                          new Response("Buy", Game1.content.LoadString("Strings\\1_6_Strings:buy_books")),
                          new Response("Trade", Game1.content.LoadString("Strings\\1_6_Strings:trade_books")),
                          new Response("Leave", Game1.content.LoadString("Strings\\1_6_Strings:Leave"))
                        }, "Bookseller");
                        goto label_537;
                      }
                      Utility.TryOpenShopMenu("Bookseller", (string) null);
                      goto label_537;
                    }
                    goto label_537;
                  case "BuyQiCoins":
                    this.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:Club_Buy100Coins"), this.createYesNoResponses(), "BuyQiCoins");
                    goto label_537;
                  case "Blacksmith":
                    return who.TilePoint.Y > tileLocation.Y && this.blacksmith(tileLocation);
                  default:
                    goto label_536;
                }
              case 'C':
                if (key1 == "ClubSeller")
                {
                  this.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:Club_ClubSeller"), new Response[2]
                  {
                    new Response("I'll", Game1.content.LoadString("Strings\\Locations:Club_ClubSeller_Yes")),
                    new Response("No", Game1.content.LoadString("Strings\\Locations:Club_ClubSeller_No"))
                  }, "ClubSeller");
                  goto label_537;
                }
                goto label_536;
              case 'D':
                if (key1 == "DwarfGrave")
                {
                  if (who.canUnderstandDwarves)
                  {
                    Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Town_DwarfGrave_Translated").Replace('\n', '^'));
                    goto label_537;
                  }
                  Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:GameLocation.cs.8214"));
                  goto label_537;
                }
                goto label_536;
              case 'E':
                if (key1 == "EnterSewer")
                {
                  if (who.mailReceived.Contains("OpenedSewer"))
                  {
                    this.playSound("stairsdown", new Vector2?(new Vector2((float) tileLocation.X, (float) tileLocation.Y)));
                    Game1.warpFarmer("Sewer", 16 /*0x10*/, 11, 2);
                    goto label_537;
                  }
                  if (who.hasRustyKey)
                  {
                    this.playSound("openBox");
                    Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\Locations:Forest_OpenedSewer")));
                    who.mailReceived.Add("OpenedSewer");
                    goto label_537;
                  }
                  Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:LockedDoor"));
                  goto label_537;
                }
                goto label_536;
              case 'F':
                if (key1 == "FarmerFile")
                  goto label_467;
                goto label_536;
              case 'L':
                if (key1 == "LumberPile")
                {
                  if (!who.hasOrWillReceiveMail("TH_LumberPile") && who.hasOrWillReceiveMail("TH_SandDragon"))
                  {
                    Game1.player.hasClubCard = true;
                    Game1.player.CanMove = false;
                    Game1.player.mailReceived.Add("TH_LumberPile");
                    Game1.player.addItemByMenuIfNecessaryElseHoldUp((Item) new SpecialItem(2));
                    Game1.player.removeQuest("5");
                    goto label_537;
                  }
                  goto label_537;
                }
                goto label_536;
              case 'N':
                if (key1 == "NPCMessage")
                {
                  string name;
                  string str3;
                  if (!ArgUtility.TryGet(action, 1, out name, out error, name: "string npcName") || !ArgUtility.TryGetRemainder(action, 2, out str3, out error, name: "string rawMessage"))
                    return LogError(error);
                  string str4 = str3.Replace("\"", "");
                  NPC characterFromName = Game1.getCharacterFromName(name);
                  if (characterFromName != null && characterFromName.currentLocation == who.currentLocation)
                  {
                    if (Utility.tileWithinRadiusOfPlayer(characterFromName.TilePoint.X, characterFromName.TilePoint.Y, 14, who))
                    {
                      try
                      {
                        string translationKey = str4.Split('/')[0];
                        string str5 = translationKey.Substring(translationKey.IndexOf(':') + 1);
                        characterFromName.setNewDialogue(translationKey, true);
                        Game1.drawDialogue(characterFromName);
                        if ((str5 == "AnimalShop.20" || str5 == "JoshHouse_Alex_Trash" || str5 == "SamHouse_Sam_Trash" || str5 == "SeedShop_Abigail_Drawers") && who != null)
                          Game1.multiplayer.globalChatInfoMessage("Caught_Snooping", who.name.Value, characterFromName.GetTokenizedDisplayName());
                        return true;
                      }
                      catch (Exception ex)
                      {
                        return false;
                      }
                    }
                  }
                  try
                  {
                    Game1.drawDialogueNoTyping(Game1.content.LoadString(str4.Split('/')[1]));
                    return false;
                  }
                  catch (Exception ex)
                  {
                    return false;
                  }
                }
                else
                  goto label_536;
              case 'S':
                if (key1 == "SandDragon")
                {
                  if (who.ActiveObject?.QualifiedItemId == "(O)768" && !who.hasOrWillReceiveMail("TH_SandDragon") && who.hasOrWillReceiveMail("TH_MayorFridge"))
                  {
                    who.reduceActiveItemByOne();
                    Game1.player.CanMove = false;
                    this.localSound("eat");
                    Game1.player.mailReceived.Add("TH_SandDragon");
                    Game1.multipleDialogues(new string[2]
                    {
                      Game1.content.LoadString("Strings\\Locations:Desert_SandDragon_ConsumeEssence"),
                      Game1.content.LoadString("Strings\\Locations:Desert_SandDragon_MrQiNote")
                    });
                    Game1.player.removeQuest("4");
                    Game1.player.addQuest("5");
                    goto label_537;
                  }
                  if (who.hasOrWillReceiveMail("TH_SandDragon"))
                  {
                    Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Desert_SandDragon_MrQiNote"));
                    goto label_537;
                  }
                  Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Desert_SandDragon_Initial"));
                  goto label_537;
                }
                goto label_536;
              case 'T':
                if (key1 == "TunnelSafe")
                {
                  if (who.ActiveObject?.QualifiedItemId == "(O)787" && !who.hasOrWillReceiveMail("TH_Tunnel"))
                  {
                    who.reduceActiveItemByOne();
                    Game1.player.CanMove = false;
                    this.playSound("openBox");
                    DelayedAction.playSoundAfterDelay("doorCreakReverse", 500);
                    Game1.player.mailReceived.Add("TH_Tunnel");
                    Game1.multipleDialogues(new string[2]
                    {
                      Game1.content.LoadString("Strings\\Locations:Tunnel_TunnelSafe_ConsumeBattery"),
                      Game1.content.LoadString("Strings\\Locations:Tunnel_TunnelSafe_MrQiNote")
                    });
                    Game1.player.addQuest("2");
                    goto label_537;
                  }
                  if (who.hasOrWillReceiveMail("TH_Tunnel"))
                  {
                    Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Tunnel_TunnelSafe_MrQiNote"));
                    goto label_537;
                  }
                  Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Tunnel_TunnelSafe_Initial"));
                  goto label_537;
                }
                goto label_536;
              case 'W':
                if (key1 == "WizardBook")
                {
                  if (who.mailReceived.Contains("hasPickedUpMagicInk") || who.hasMagicInk)
                  {
                    this.ShowConstructOptions("Wizard");
                    goto label_537;
                  }
                  goto label_537;
                }
                goto label_536;
              default:
                goto label_536;
            }
          case 11:
            switch (key1[7])
            {
              case 'B':
                if (key1 == "ElliottBook")
                {
                  Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:ElliottHouse_ElliottBook_Blank"));
                  goto label_537;
                }
                goto label_536;
              case 'O':
                if (key1 == "MessageOnce")
                {
                  string str;
                  string text;
                  if (!ArgUtility.TryGet(action, 1, out str, out error, name: "string eventFlag") || !ArgUtility.TryGetRemainder(action, 2, out text, out error, name: "string dialogue"))
                    return LogError(error);
                  if (who.eventsSeen.Add(str))
                  {
                    Game1.drawObjectDialogue(Game1.parseText(text));
                    goto label_537;
                  }
                  goto label_537;
                }
                goto label_536;
              case 'R':
                if (key1 == "MasteryRoom")
                {
                  int sub1 = Game1.player.farmingLevel.Value / 10 + Game1.player.fishingLevel.Value / 10 + Game1.player.foragingLevel.Value / 10 + Game1.player.miningLevel.Value / 10 + Game1.player.combatLevel.Value / 10;
                  if (sub1 >= 5)
                  {
                    Game1.playSound("doorClose");
                    Game1.warpFarmer("MasteryCave", 7, 11, 0);
                    goto label_537;
                  }
                  Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\1_6_Strings:MasteryCave", (object) sub1));
                  goto label_537;
                }
                goto label_536;
              case 'W':
                if (key1 == "ObeliskWarp")
                {
                  string destination;
                  Point point;
                  bool force_dismount;
                  if (!ArgUtility.TryGet(action, 1, out destination, out error, name: "string targetLocation") || !ArgUtility.TryGetPoint(action, 2, out point, out error, "Point targetTile") || !ArgUtility.TryGetOptionalBool(action, 4, out force_dismount, out error, name: "bool forceDismount"))
                    return LogError(error);
                  Building.PerformObeliskWarp(destination, point.X, point.Y, force_dismount, who);
                  return true;
                }
                goto label_536;
              case 'a':
                if (key1 == "WizardHatch")
                {
                  Friendship friendship;
                  if (who.friendshipData.TryGetValue("Wizard", out friendship) && friendship.Points >= 1000)
                  {
                    this.playSound("doorClose", new Vector2?(new Vector2((float) tileLocation.X, (float) tileLocation.Y)));
                    Game1.warpFarmer("WizardHouseBasement", 4, 4, true);
                    goto label_537;
                  }
                  NPC character = this.characters[0];
                  character.CurrentDialogue.Push(new Dialogue(character, "Data\\ExtraDialogue:Wizard_Hatch"));
                  Game1.drawDialogue(character);
                  goto label_537;
                }
                goto label_536;
              case 'd':
                if (key1 == "RailroadBox")
                {
                  if (who.ActiveObject?.QualifiedItemId == "(O)394" && !who.hasOrWillReceiveMail("TH_Railroad") && who.hasOrWillReceiveMail("TH_Tunnel"))
                  {
                    who.reduceActiveItemByOne();
                    Game1.player.CanMove = false;
                    this.localSound("Ship");
                    Game1.player.mailReceived.Add("TH_Railroad");
                    Game1.multipleDialogues(new string[2]
                    {
                      Game1.content.LoadString("Strings\\Locations:Railroad_Box_ConsumeShell"),
                      Game1.content.LoadString("Strings\\Locations:Railroad_Box_MrQiNote")
                    });
                    Game1.player.removeQuest("2");
                    Game1.player.addQuest("3");
                    goto label_537;
                  }
                  if (who.hasOrWillReceiveMail("TH_Railroad"))
                  {
                    Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Railroad_Box_MrQiNote"));
                    goto label_537;
                  }
                  Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Railroad_Box_Initial"));
                  goto label_537;
                }
                goto label_536;
              case 'h':
                if (key1 == "ColaMachine")
                {
                  this.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:Saloon_ColaMachine_Question"), this.createYesNoResponses(), "buyJojaCola");
                  goto label_537;
                }
                goto label_536;
              case 'l':
                if (key1 == "SpiritAltar")
                {
                  if (who.ActiveObject != null && Game1.player.team.sharedDailyLuck.Value != -0.12 && Game1.player.team.sharedDailyLuck.Value != 0.12)
                  {
                    if (who.ActiveObject.Price >= 60)
                    {
                      this.temporarySprites.Add(new TemporaryAnimatedSprite(352, 70f, 2, 2, new Vector2((float) (tileLocation.X * 64 /*0x40*/), (float) (tileLocation.Y * 64 /*0x40*/)), false, false));
                      Game1.player.team.sharedDailyLuck.Value = 0.12;
                      this.playSound("money");
                    }
                    else
                    {
                      this.temporarySprites.Add(new TemporaryAnimatedSprite(362, 50f, 6, 1, new Vector2((float) (tileLocation.X * 64 /*0x40*/), (float) (tileLocation.Y * 64 /*0x40*/)), false, false));
                      Game1.player.team.sharedDailyLuck.Value = -0.12;
                      this.playSound("thunder");
                    }
                    who.ActiveObject = (Object) null;
                    who.showNotCarrying();
                    goto label_537;
                  }
                  goto label_537;
                }
                goto label_536;
              case 'p':
                if (key1 == "BuyBackpack")
                {
                  Response response1 = new Response("Purchase", Game1.content.LoadString("Strings\\Locations:SeedShop_BuyBackpack_Response2000"));
                  Response response2 = new Response("Purchase", Game1.content.LoadString("Strings\\Locations:SeedShop_BuyBackpack_Response10000"));
                  Response response3 = new Response("Not", Game1.content.LoadString("Strings\\Locations:SeedShop_BuyBackpack_ResponseNo"));
                  if (Game1.player.maxItems.Value == 12)
                  {
                    this.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:SeedShop_BuyBackpack_Question24"), new Response[2]
                    {
                      response1,
                      response3
                    }, "Backpack");
                    goto label_537;
                  }
                  if (Game1.player.maxItems.Value < 36)
                  {
                    this.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:SeedShop_BuyBackpack_Question36"), new Response[2]
                    {
                      response2,
                      response3
                    }, "Backpack");
                    goto label_537;
                  }
                  goto label_537;
                }
                goto label_536;
              case 'y':
                if (key1 == "ForestPylon")
                {
                  if (who?.ActiveObject?.QualifiedItemId == "(O)FarAwayStone")
                  {
                    who.reduceActiveItemByOne();
                    Game1.playSound("openBox");
                    Game1.player.mailReceived.Add("hasActivatedForestPylon");
                    this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\terraria_cat", new Microsoft.Xna.Framework.Rectangle(0, 106, 14, 22), new Vector2(16.6f, 2.5f) * 64f, false, 0.0f, Color.White)
                    {
                      animationLength = 8,
                      interval = 100f,
                      totalNumberOfLoops = 9999,
                      scale = 4f
                    });
                    Game1.player.freezePause = 3000;
                    DelayedAction.functionAfterDelay((Action) (() => Game1.globalFadeToBlack((Game1.afterFadeFunction) (() => this.startEvent(new Event(Game1.content.LoadString("Strings\\1_6_Strings:ForestPylonEvent")))))), 1000);
                    goto label_537;
                  }
                  if (who.mailReceived.Contains("hasActivatedForestPylon"))
                  {
                    Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\1_6_Strings:ForestPylonActivated"));
                    goto label_537;
                  }
                  Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\1_6_Strings:ForestPylon"));
                  goto label_537;
                }
                goto label_536;
              default:
                goto label_536;
            }
          case 12:
            switch (key1[3])
            {
              case 'a':
                if (key1 == "WizardShrine")
                {
                  this.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:WizardTower_WizardShrine").Replace('\n', '^'), this.createYesNoResponses(), "WizardShrine");
                  goto label_537;
                }
                goto label_536;
              case 'b':
                if (key1 == "ClubComputer")
                  goto label_467;
                goto label_536;
              case 'd':
                if (key1 == "GoldenScythe")
                {
                  if (!Game1.player.mailReceived.Contains("gotGoldenScythe"))
                  {
                    if (!Game1.player.isInventoryFull())
                    {
                      Game1.playSound("parry");
                      Game1.player.mailReceived.Add("gotGoldenScythe");
                      this.setMapTile(29, 4, 245, "Front", "mine");
                      this.setMapTile(30, 4, 246, "Front", "mine");
                      this.setMapTile(29, 5, 261, "Front", "mine");
                      this.setMapTile(30, 5, 262, "Front", "mine");
                      this.setMapTile(29, 6, 277, "Buildings", "mine");
                      this.setMapTile(30, 56, 278, "Buildings", "mine");
                      Game1.player.addItemByMenuIfNecessaryElseHoldUp(ItemRegistry.Create("(W)53"));
                      goto label_537;
                    }
                    Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:Crop.cs.588"));
                    goto label_537;
                  }
                  Game1.changeMusicTrack("silence");
                  this.performTouchAction("MagicWarp Mine 67 10", Game1.player.getStandingPosition());
                  goto label_537;
                }
                goto label_536;
              case 'e':
                if (key1 == "MineElevator")
                {
                  if (MineShaft.lowestLevelReached < 5)
                  {
                    Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\Locations:Mines_MineElevator_NotWorking")));
                    goto label_537;
                  }
                  Game1.activeClickableMenu = (IClickableMenu) new MineElevatorMenu();
                  goto label_537;
                }
                goto label_536;
              case 'i':
                if (key1 == "ElliottPiano")
                {
                  int key2;
                  if (!ArgUtility.TryGetInt(action, 1, out key2, out error, "int key"))
                    return LogError(error);
                  this.playElliottPiano(key2);
                  goto label_537;
                }
                goto label_536;
              case 'l':
                if (key1 == "BuildingSilo")
                {
                  if (who.IsLocalPlayer)
                  {
                    Object activeObject = who.ActiveObject;
                    if (activeObject?.QualifiedItemId == "(O)178")
                    {
                      activeObject.FixStackSize();
                      int num = activeObject.Stack - this.tryToAddHay(activeObject.Stack);
                      if (num > 0)
                      {
                        if (activeObject.ConsumeStack(num) == null)
                          who.ActiveObject = (Object) null;
                        Game1.playSound("Ship");
                        DelayedAction.playSoundAfterDelay("grassyStep", 100);
                        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Buildings:AddedHay", (object) num));
                        goto label_537;
                      }
                      goto label_537;
                    }
                    Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Buildings:PiecesOfHay", (object) this.piecesOfHay, (object) this.GetHayCapacity()));
                    goto label_537;
                  }
                  goto label_537;
                }
                goto label_536;
              case 'p':
                if (key1 == "HospitalShop")
                {
                  Point tilePoint = who.TilePoint;
                  Utility.TryOpenShopMenu("Hospital", this, new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(tilePoint.X - 1, tilePoint.Y - 2, 2, 1)));
                  goto label_537;
                }
                goto label_536;
              case 's':
                if (key1 == "MonsterGrave")
                {
                  Game1.multipleDialogues(Game1.content.LoadString("Strings\\Locations:Backwoods_MonsterGrave").Split('#'));
                  goto label_537;
                }
                goto label_536;
              case 'z':
                if (key1 == "PrizeMachine")
                {
                  Game1.activeClickableMenu = (IClickableMenu) new PrizeTicketMenu();
                  goto label_537;
                }
                goto label_536;
              default:
                goto label_536;
            }
          case 13:
            switch (key1[3])
            {
              case 'C':
                if (key1 == "IceCreamStand")
                {
                  Utility.TryOpenShopMenu("IceCreamStand", this, new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(tileLocation.X, tileLocation.Y - 3, 1, 3)));
                  goto label_537;
                }
                goto label_536;
              case 'c':
                if (key1 == "SpecialOrders")
                {
                  Game1.player.team.ordersBoardMutex.RequestLock((Action) (() =>
                  {
                    Game1.activeClickableMenu = (IClickableMenu) new SpecialOrdersBoard()
                    {
                      behaviorBeforeCleanup = (Action<IClickableMenu>) (menu => Game1.player.team.ordersBoardMutex.ReleaseLock())
                    };
                  }));
                  goto label_537;
                }
                goto label_536;
              case 'e':
                if (key1 == "AdventureShop")
                {
                  this.adventureShop();
                  goto label_537;
                }
                goto label_536;
              case 'l':
                if (key1 == "BuildingChest")
                {
                  string name;
                  if (!ArgUtility.TryGet(action, 1, out name, out error, name: "string buildingAction"))
                    return LogError(error);
                  Building buildingAt = this.getBuildingAt(new Vector2((float) tileLocation.X, (float) tileLocation.Y));
                  int num = buildingAt != null ? (buildingAt.PerformBuildingChestAction(name, who) ? 1 : 0) : 0;
                  return true;
                }
                goto label_536;
              case 'm':
                if (key1 == "SummitBoulder")
                {
                  Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:SummitBoulder"));
                  goto label_537;
                }
                goto label_536;
              case 's':
                if (key1 == "MessageSpeech")
                  break;
                goto label_536;
              case 't':
                if (key1 == "NextMineLevel")
                  goto label_434;
                goto label_536;
              default:
                goto label_536;
            }
            break;
          case 14:
            switch (key1[11])
            {
              case 'a':
                if (key1 == "LockedDoorWarp")
                {
                  Point tile;
                  string locationName;
                  int openTime;
                  int closeTime;
                  string npcName;
                  int minFriendship;
                  if (!ArgUtility.TryGetPoint(action, 1, out tile, out error, "Point tile") || !ArgUtility.TryGet(action, 3, out locationName, out error, name: "string locationName") || !ArgUtility.TryGetInt(action, 4, out openTime, out error, "int openTime") || !ArgUtility.TryGetInt(action, 5, out closeTime, out error, "int closeTime") || !ArgUtility.TryGetOptional(action, 6, out npcName, out error, name: "string npcName") || !ArgUtility.TryGetOptionalInt(action, 7, out minFriendship, out error, name: "int minFriendship"))
                    return LogError(error);
                  who.faceGeneralDirection(new Vector2((float) tileLocation.X, (float) tileLocation.Y) * 64f);
                  this.lockedDoorWarp(tile, locationName, openTime, closeTime, npcName, minFriendship);
                  goto label_537;
                }
                goto label_536;
              case 'e':
                if (key1 == "EvilShrineLeft")
                {
                  if (who.getChildrenCount() == 0)
                  {
                    Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:WitchHut_EvilShrineLeftInactive"));
                    goto label_537;
                  }
                  this.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:WitchHut_EvilShrineLeft"), this.createYesNoResponses(), "evilShrineLeft");
                  goto label_537;
                }
                goto label_536;
              case 'k':
                if (key1 == "WarpMensLocker")
                {
                  Point point;
                  string locationName;
                  if (!ArgUtility.TryGetPoint(action, 1, out point, out error, "Point tile") || !ArgUtility.TryGet(action, 3, out locationName, out error, name: "string locationName"))
                    return LogError(error);
                  bool flag = action.Length < 5;
                  if (!who.IsMale)
                  {
                    if (who.IsLocalPlayer)
                      Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:MensLocker_WrongGender"));
                    return true;
                  }
                  who.faceGeneralDirection(new Vector2((float) tileLocation.X, (float) tileLocation.Y) * 64f);
                  if (flag)
                    this.playSound("doorClose", new Vector2?(new Vector2((float) tileLocation.X, (float) tileLocation.Y)));
                  Game1.warpFarmer(locationName, point.X, point.Y, false);
                  goto label_537;
                }
                goto label_536;
              case 'o':
                if (key1 == "SquidFestBooth")
                {
                  this.createQuestionDialogue(Game1.content.LoadString("Strings\\1_6_Strings:SquidFestBooth_Intro"), new Response[3]
                  {
                    new Response("Rewards", Game1.content.LoadString("Strings\\1_6_Strings:GetRewards")),
                    new Response("Explanation", Game1.content.LoadString("Strings\\1_6_Strings:Explanation")),
                    new Response("Leave", Game1.content.LoadString("Strings\\1_6_Strings:Leave"))
                  }, "SquidFestBooth");
                  goto label_537;
                }
                goto label_536;
              case 'r':
                if (key1 == "Arcade_Prairie")
                {
                  this.showPrairieKingMenu();
                  goto label_537;
                }
                goto label_536;
              case 't':
                if (key1 == "Theater_Poster")
                {
                  if (Game1.MasterPlayer.hasOrWillReceiveMail("ccMovieTheater"))
                  {
                    MovieData movieToday = MovieTheater.GetMovieToday();
                    if (movieToday != null)
                    {
                      Game1.multipleDialogues(new string[2]
                      {
                        Game1.content.LoadString("Strings\\Locations:Theater_Poster_0", (object) TokenParser.ParseText(movieToday.Title)),
                        Game1.content.LoadString("Strings\\Locations:Theater_Poster_1", (object) TokenParser.ParseText(movieToday.Description))
                      });
                      goto label_537;
                    }
                    goto label_537;
                  }
                  goto label_537;
                }
                goto label_536;
              case 'u':
                if (key1 == "WarpGreenhouse")
                {
                  if (Game1.MasterPlayer.mailReceived.Contains("ccPantry"))
                  {
                    who.faceGeneralDirection(new Vector2((float) tileLocation.X, (float) tileLocation.Y) * 64f);
                    this.playSound("doorClose", new Vector2?(new Vector2((float) tileLocation.X, (float) tileLocation.Y)));
                    GameLocation locationFromName = Game1.getLocationFromName("Greenhouse");
                    int tileX = 10;
                    int tileY = 23;
                    if (locationFromName != null)
                    {
                      foreach (Warp warp in (NetList<Warp, NetRef<Warp>>) locationFromName.warps)
                      {
                        if (warp.TargetName == "Farm")
                        {
                          tileX = warp.X;
                          tileY = warp.Y - 1;
                          break;
                        }
                      }
                    }
                    Game1.warpFarmer("Greenhouse", tileX, tileY, false);
                    goto label_537;
                  }
                  Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Farm_GreenhouseRuins"));
                  goto label_537;
                }
                goto label_536;
              default:
                goto label_536;
            }
          case 15:
            switch (key1[4])
            {
              case 'S':
                if (key1 == "EvilShrineRight")
                {
                  if (Game1.spawnMonstersAtNight)
                  {
                    this.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:WitchHut_EvilShrineRightDeActivate"), this.createYesNoResponses(), "evilShrineRightDeActivate");
                    goto label_537;
                  }
                  this.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:WitchHut_EvilShrineRightActivate"), this.createYesNoResponses(), "evilShrineRightActivate");
                  goto label_537;
                }
                goto label_536;
              case 'd':
                if (key1 == "Arcade_Minecart")
                {
                  if (who.hasSkullKey)
                  {
                    Response[] answerChoices = new Response[3]
                    {
                      new Response("Progress", Game1.content.LoadString("Strings\\Locations:Saloon_Arcade_Minecart_ProgressMode")),
                      new Response("Endless", Game1.content.LoadString("Strings\\Locations:Saloon_Arcade_Minecart_EndlessMode")),
                      new Response("Exit", Game1.content.LoadString("Strings\\Locations:Saloon_Arcade_Minecart_Exit"))
                    };
                    this.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:Saloon_Arcade_Minecart_Menu"), answerChoices, "MinecartGame");
                    goto label_537;
                  }
                  Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Saloon_Arcade_Minecart_Inactive"));
                  goto label_537;
                }
                goto label_536;
              case 'i':
                if (key1 == "ConditionalDoor")
                {
                  if (action.Length > 1 && !Game1.eventUp)
                  {
                    if (GameStateQuery.CheckConditions(ArgUtility.UnsplitQuoteAware(action, ' ', 1)))
                    {
                      this.openDoor(tileLocation, true);
                      return true;
                    }
                    string path = this.doesTileHaveProperty(tileLocation.X, tileLocation.Y, "LockedDoorMessage", "Buildings");
                    if (path != null)
                    {
                      Game1.drawObjectDialogue(TokenParser.ParseText(Game1.content.LoadString(path)));
                      goto label_537;
                    }
                    Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:LockedDoor"));
                    goto label_537;
                  }
                  goto label_537;
                }
                goto label_536;
              case 't':
                if (key1 == "TroutDerbyBooth")
                {
                  this.createQuestionDialogue(Game1.content.LoadString("Strings\\1_6_Strings:FishingDerbyBooth_Intro"), new Response[3]
                  {
                    new Response("Rewards", Game1.content.LoadString("Strings\\1_6_Strings:GetRewards")),
                    new Response("Explanation", Game1.content.LoadString("Strings\\1_6_Strings:Explanation")),
                    new Response("Leave", Game1.content.LoadString("Strings\\1_6_Strings:Leave"))
                  }, "TroutDerbyBooth");
                  goto label_537;
                }
                goto label_536;
              case 'y':
                if (key1 == "EmilyRoomObject")
                {
                  if (Game1.player.eventsSeen.Contains("463391") && Game1.player.spouse != "Emily")
                  {
                    if (this.getTemporarySpriteByID(5858585) is EmilysParrot temporarySpriteById)
                    {
                      temporarySpriteById.doAction();
                      goto label_537;
                    }
                    goto label_537;
                  }
                  Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:HaleyHouse_EmilyRoomObject"));
                  goto label_537;
                }
                goto label_536;
              default:
                goto label_536;
            }
          case 16 /*0x10*/:
            switch (key1[0])
            {
              case 'E':
                if (key1 == "EvilShrineCenter")
                {
                  if (who.isDivorced())
                  {
                    this.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:WitchHut_EvilShrineCenter"), this.createYesNoResponses(), "evilShrineCenter");
                    goto label_537;
                  }
                  Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:WitchHut_EvilShrineCenterInactive"));
                  goto label_537;
                }
                goto label_536;
              case 'F':
                if (key1 == "FishingDerbySign")
                {
                  Game1.activeClickableMenu = (IClickableMenu) new LetterViewerMenu(Game1.content.LoadString(Game1.IsSummer ? "Strings\\1_6_Strings:FishingDerbySign" : "Strings\\1_6_Strings:SquidFestSign"));
                  goto label_537;
                }
                goto label_536;
              case 'Q':
                if (key1 == "QiChallengeBoard")
                {
                  Game1.player.team.qiChallengeBoardMutex.RequestLock((Action) (() =>
                  {
                    Game1.activeClickableMenu = (IClickableMenu) new SpecialOrdersBoard("Qi")
                    {
                      behaviorBeforeCleanup = (Action<IClickableMenu>) (menu => Game1.player.team.qiChallengeBoardMutex.ReleaseLock())
                    };
                  }));
                  goto label_537;
                }
                goto label_536;
              case 'T':
                if (key1 == "Theater_Entrance")
                {
                  if (Game1.MasterPlayer.hasOrWillReceiveMail("ccMovieTheater"))
                  {
                    if (Game1.player.team.movieMutex.IsLocked())
                    {
                      Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\Characters:MovieTheater_CurrentlyShowing")));
                      goto label_537;
                    }
                    if (Game1.isFestival())
                    {
                      Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\Characters:MovieTheater_ClosedFestival")));
                      goto label_537;
                    }
                    if (Game1.timeOfDay > 2100 || Game1.timeOfDay < 900)
                    {
                      string sub1 = Game1.getTimeOfDayString(900).Replace(" ", "");
                      string sub2 = Game1.getTimeOfDayString(2100).Replace(" ", "");
                      Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:LockedDoor_OpenRange", (object) sub1, (object) sub2));
                      goto label_537;
                    }
                    if (Game1.player.lastSeenMovieWeek.Value >= Game1.Date.TotalWeeks)
                    {
                      Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Characters:MovieTheater_AlreadySeen"));
                      goto label_537;
                    }
                    NPC npc = (NPC) null;
                    foreach (MovieInvitation movieInvitation in Game1.player.team.movieInvitations)
                    {
                      if (movieInvitation.farmer == Game1.player && !movieInvitation.fulfilled && MovieTheater.GetFirstInvitedPlayer(movieInvitation.invitedNPC) == Game1.player)
                      {
                        npc = movieInvitation.invitedNPC;
                        break;
                      }
                    }
                    if (Game1.player.Items.ContainsId("(O)809"))
                    {
                      Game1.currentLocation.createQuestionDialogue(npc != null ? Game1.content.LoadString("Strings\\Characters:MovieTheater_WatchWithFriendPrompt", (object) npc.displayName) : Game1.content.LoadString("Strings\\Characters:MovieTheater_WatchAlonePrompt"), Game1.currentLocation.createYesNoResponses(), "EnterTheaterSpendTicket");
                      goto label_537;
                    }
                    Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\Characters:MovieTheater_NoTicket")));
                    goto label_537;
                  }
                  goto label_537;
                }
                goto label_536;
              case 'W':
                if (key1 == "WarpWomensLocker")
                {
                  Point point;
                  string locationName;
                  if (!ArgUtility.TryGetPoint(action, 1, out point, out error, "Point tile") || !ArgUtility.TryGet(action, 3, out locationName, out error, name: "string locationName"))
                    return LogError(error);
                  bool flag = action.Length < 5;
                  if (who.IsMale)
                  {
                    if (who.IsLocalPlayer)
                      Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:WomensLocker_WrongGender"));
                    return true;
                  }
                  who.faceGeneralDirection(new Vector2((float) tileLocation.X, (float) tileLocation.Y) * 64f);
                  if (flag)
                    this.playSound("doorClose", new Vector2?(new Vector2((float) tileLocation.X, (float) tileLocation.Y)));
                  Game1.warpFarmer(locationName, point.X, point.Y, false);
                  goto label_537;
                }
                goto label_536;
              default:
                goto label_536;
            }
          case 17:
            switch (key1[0])
            {
              case 'B':
                if (key1 == "BuildingGoldClock")
                {
                  bool flag = !Game1.netWorldState.Value.goldenClocksTurnedOff.Value;
                  who.currentLocation.createQuestionDialogue(Game1.content.LoadString("Strings\\1_6_Strings:GoldClock_" + (flag ? "Off" : "On")), who.currentLocation.createYesNoResponses(), "GoldClock");
                  goto label_537;
                }
                goto label_536;
              case 'M':
                if (key1 == "MinecartTransport")
                {
                  this.ShowMineCartMenu(ArgUtility.Get(action, 1) ?? "Default", ArgUtility.Get(action, 2));
                  return true;
                }
                goto label_536;
              case 'T':
                if (key1 == "Theater_BoxOffice")
                {
                  if (Game1.MasterPlayer.hasOrWillReceiveMail("ccMovieTheater"))
                  {
                    if (Game1.isFestival())
                    {
                      Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\Characters:MovieTheater_ClosedFestival")));
                      goto label_537;
                    }
                    if (Game1.timeOfDay > 2100)
                    {
                      Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Characters:MovieTheater_BoxOfficeClosed"));
                      goto label_537;
                    }
                    if (MovieTheater.GetMovieToday() != null)
                    {
                      Utility.TryOpenShopMenu("BoxOffice", (string) null);
                      goto label_537;
                    }
                    goto label_537;
                  }
                  goto label_537;
                }
                goto label_536;
              case 'W':
                if (key1 == "Warp_Sunroom_Door")
                {
                  if (who.getFriendshipHeartLevelForNPC("Caroline") >= 2)
                  {
                    this.playSound("doorClose", new Vector2?(new Vector2((float) tileLocation.X, (float) tileLocation.Y)));
                    Game1.warpFarmer("Sunroom", 5, 13, false);
                    goto label_537;
                  }
                  Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Caroline_Sunroom_Door"));
                  goto label_537;
                }
                goto label_536;
              default:
                goto label_536;
            }
          case 18:
            switch (key1[12])
            {
              case 'C':
                if (key1 == "MasteryCave_Combat")
                {
                  if (Game1.player.stats.Get(StatKeys.Mastery(4)) >= 0U)
                  {
                    Game1.activeClickableMenu = (IClickableMenu) new MasteryTrackerMenu(4);
                    goto label_537;
                  }
                  goto label_537;
                }
                goto label_536;
              case 'M':
                if (key1 == "MasteryCave_Mining")
                {
                  if (Game1.player.stats.Get(StatKeys.Mastery(3)) >= 0U)
                  {
                    Game1.activeClickableMenu = (IClickableMenu) new MasteryTrackerMenu(3);
                    goto label_537;
                  }
                  goto label_537;
                }
                goto label_536;
              case 'r':
                if (key1 == "GrandpaMasteryNote")
                {
                  Game1.activeClickableMenu = (IClickableMenu) new LetterViewerMenu(Game1.content.LoadString("Strings\\1_6_Strings:GrandpaMasteryNote", (object) Game1.player.Name, (object) Game1.player.farmName));
                  goto label_537;
                }
                goto label_536;
              default:
                goto label_536;
            }
          case 19:
            switch (key1[13])
            {
              case 'C':
                if (key1 == "WarpCommunityCenter")
                {
                  if (Game1.MasterPlayer.mailReceived.Contains("ccDoorUnlock") || Game1.MasterPlayer.mailReceived.Contains("JojaMember"))
                  {
                    this.playSound("doorClose", new Vector2?(new Vector2((float) tileLocation.X, (float) tileLocation.Y)));
                    Game1.warpFarmer("CommunityCenter", 32 /*0x20*/, 23, false);
                    goto label_537;
                  }
                  Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:GameLocation.cs.8175"));
                  goto label_537;
                }
                goto label_536;
              case 'a':
                if (key1 == "MasteryCave_Farming")
                {
                  if (Game1.player.stats.Get(StatKeys.Mastery(0)) >= 0U)
                  {
                    Game1.activeClickableMenu = (IClickableMenu) new MasteryTrackerMenu(0);
                    goto label_537;
                  }
                  goto label_537;
                }
                goto label_536;
              case 'i':
                if (key1 == "MasteryCave_Fishing")
                {
                  if (Game1.player.stats.Get(StatKeys.Mastery(1)) >= 0U)
                  {
                    Game1.activeClickableMenu = (IClickableMenu) new MasteryTrackerMenu(1);
                    goto label_537;
                  }
                  goto label_537;
                }
                goto label_536;
              default:
                goto label_536;
            }
          case 20:
            switch (key1[12])
            {
              case 'F':
                if (key1 == "MasteryCave_Foraging")
                {
                  if (Game1.player.stats.Get(StatKeys.Mastery(2)) >= 0U)
                  {
                    Game1.activeClickableMenu = (IClickableMenu) new MasteryTrackerMenu(2);
                    goto label_537;
                  }
                  goto label_537;
                }
                goto label_536;
              case 'P':
                if (key1 == "MasteryCave_Pedestal")
                {
                  Game1.activeClickableMenu = (IClickableMenu) new MasteryTrackerMenu();
                  goto label_537;
                }
                goto label_536;
              default:
                goto label_536;
            }
          case 21:
            if (key1 == "SpecialWaterDroppable" && (!(this is MineShaft) || (this as MineShaft).mineLevel == 100))
            {
              if (who?.ActiveObject?.QualifiedItemId == "(O)103")
              {
                this.localSound("throwDownITem");
                who.reduceActiveItemByOne();
                TemporaryAnimatedSprite tempSprite = new TemporaryAnimatedSprite(103, 9999f, 1, 1, who.position.Value + new Vector2(0.0f, (float) sbyte.MinValue), false, false, false)
                {
                  motion = new Vector2(4f, -4f),
                  acceleration = new Vector2(0.0f, 0.3f),
                  yStopCoordinate = (int) who.position.Y,
                  id = 777
                };
                who.freezePause = 4000;
                tempSprite.reachedStopCoordinate = (TemporaryAnimatedSprite.endBehavior) (x =>
                {
                  this.removeTemporarySpritesWithID(777);
                  this.temporarySprites.Add(new TemporaryAnimatedSprite(28, 300f, 2, 1, tempSprite.position, false, false)
                  {
                    color = Color.OrangeRed
                  });
                  this.localSound("dropItemInWater");
                  DelayedAction.functionAfterDelay((Action) (() =>
                  {
                    this.localSound("terraria_boneSerpent");
                    this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\terraria_cat", new Microsoft.Xna.Framework.Rectangle(128 /*0x80*/, 96 /*0x60*/, 32 /*0x20*/, 32 /*0x20*/), 70f, 4, 5, tempSprite.position + new Vector2(-5f, -3f) * 4f, false, true, 0.99f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f));
                    this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\terraria_cat", new Microsoft.Xna.Framework.Rectangle(128 /*0x80*/, 96 /*0x60*/, 32 /*0x20*/, 32 /*0x20*/), 60f, 4, 5, tempSprite.position + new Vector2(-5f, 7f) * 4f, false, false, 1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f));
                    this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\terraria_cat", new Microsoft.Xna.Framework.Rectangle(134, 2, 21, 38), 9999f, 1, 1, tempSprite.position, false, false, 0.98f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
                    {
                      xPeriodic = true,
                      xPeriodicLoopTime = 500f,
                      xPeriodicRange = 2f,
                      motion = new Vector2(0.0f, -8f)
                    });
                    for (int index = 0; index < 13; ++index)
                      this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\terraria_cat", new Microsoft.Xna.Framework.Rectangle(134, index == 12 ? 54 : 41, 21, 12), 9999f, 1, 1, tempSprite.position, false, false, (float) (0.97000002861022949 - (double) index * 0.0099999997764825821), 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
                      {
                        xPeriodic = true,
                        xPeriodicLoopTime = (float) (500 + Game1.random.Next(-50, 50)),
                        xPeriodicRange = 2f,
                        motion = new Vector2(0.0f, -8f),
                        delayBeforeAnimationStart = 220 + 80 /*0x50*/ * index
                      });
                    TemporaryAnimatedSprite temporaryAnimatedSprite = new TemporaryAnimatedSprite(935, 9999f, 1, 1, tempSprite.position + new Vector2(0.0f, (float) sbyte.MinValue), false, false, false)
                    {
                      motion = new Vector2(-4f, -4f),
                      acceleration = new Vector2(0.0f, 0.3f),
                      yStopCoordinate = (int) ((double) who.position.Y - 128.0 + 12.0),
                      id = 888
                    };
                    temporaryAnimatedSprite.reachedStopCoordinate = (TemporaryAnimatedSprite.endBehavior) (y =>
                    {
                      who.addItemByMenuIfNecessary((Item) new Object("FarAwayStone", 1));
                      who.currentLocation.removeTemporarySpritesWithID(888);
                      this.localSound("coin");
                    });
                    who.currentLocation.temporarySprites.Add(temporaryAnimatedSprite);
                  }), 1000);
                });
                this.temporarySprites.Add(tempSprite);
                return true;
              }
              if (who?.ActiveObject == null || who.ActiveObject.questItem.Value || !(who.ActiveObject.QualifiedItemId != "(O)FarAwayStone") || who.ActiveObject.Edibility > 0 || who.ActiveObject.Name.Contains("Totem"))
                return false;
              ParsedItemData dataOrErrorItem = ItemRegistry.GetDataOrErrorItem(who?.ActiveObject.QualifiedItemId);
              if (dataOrErrorItem != null)
              {
                this.localSound("throwDownITem");
                int _id = Game1.random.Next();
                TemporaryAnimatedSprite tempSprite = new TemporaryAnimatedSprite(dataOrErrorItem.GetTextureName(), dataOrErrorItem.GetSourceRect(), 9999f, 1, 1, who.position.Value + new Vector2(0.0f, (float) sbyte.MinValue), false, false)
                {
                  motion = new Vector2(4f, -4f),
                  acceleration = new Vector2(0.0f, 0.3f),
                  yStopCoordinate = (int) who.position.Y,
                  id = _id,
                  scale = (float) (4.0 * (dataOrErrorItem.GetSourceRect().Height > 32 /*0x20*/ ? 0.5 : 1.0))
                };
                who.reduceActiveItemByOne();
                tempSprite.reachedStopCoordinate = (TemporaryAnimatedSprite.endBehavior) (x =>
                {
                  this.removeTemporarySpritesWithID(_id);
                  this.temporarySprites.Add(new TemporaryAnimatedSprite(28, 300f, 2, 1, tempSprite.position, false, false)
                  {
                    color = Color.OrangeRed
                  });
                  this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(276, 1985, 12, 11), tempSprite.position + new Vector2(2f, 0.0f) * 4f, false, 0.0f, Color.White)
                  {
                    interval = 50f,
                    totalNumberOfLoops = 99999,
                    animationLength = 4,
                    scale = 4f,
                    layerDepth = 0.99f,
                    alphaFade = 0.02f
                  });
                  for (int index = 0; index < 4; ++index)
                    this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(276, 1965, 8, 8), tempSprite.position + new Vector2(2f, 0.0f) * 4f, false, 0.0f, Color.White)
                    {
                      motion = new Vector2((float) Game1.random.Next(-15, 26) / 10f, -4f),
                      acceleration = new Vector2(0.0f, (float) Game1.random.Next(3, 7) / 30f),
                      interval = 50f,
                      totalNumberOfLoops = 99999,
                      animationLength = 7,
                      scale = 4f,
                      layerDepth = 0.99f,
                      alphaFade = 0.02f,
                      delayBeforeAnimationStart = index * 30
                    });
                  this.localSound("dropItemInWater");
                  this.localSound("fireball");
                });
                this.temporarySprites.Add(tempSprite);
              }
              return true;
            }
            goto label_536;
          case 24:
            switch (key1[0])
            {
              case 'B':
                if (key1 == "BuildingToggleAnimalDoor")
                {
                  Building buildingAt = this.getBuildingAt(new Vector2((float) tileLocation.X, (float) tileLocation.Y));
                  if (buildingAt != null && Game1.didPlayerJustRightClick(true))
                  {
                    buildingAt.ToggleAnimalDoor(who);
                    return true;
                  }
                  goto label_537;
                }
                goto label_536;
              case 'N':
                if (key1 == "NPCSpeechMessageNoRadius")
                {
                  string name;
                  string str;
                  if (!ArgUtility.TryGet(action, 1, out name, out error, name: "string npcName") || !ArgUtility.TryGet(action, 2, out str, out error, name: "string translationKey"))
                    return LogError(error);
                  NPC speaker = Game1.getCharacterFromName(name);
                  if (speaker == null)
                  {
                    try
                    {
                      speaker = new NPC((AnimatedSprite) null, Vector2.Zero, "", 0, name, false, Game1.temporaryContent.Load<Texture2D>("Portraits\\" + name));
                    }
                    catch (Exception ex)
                    {
                      return LogError("couldn't find or create a matching NPC");
                    }
                  }
                  try
                  {
                    speaker.setNewDialogue("Strings\\StringsFromMaps:" + str, true);
                    Game1.drawDialogue(speaker);
                    return true;
                  }
                  catch (Exception ex)
                  {
                    return LogError($"unhandled exception drawing dialogue: {ex}");
                  }
                }
                else
                  goto label_536;
              case 'T':
                if (key1 == "Theater_PosterComingSoon")
                {
                  if (Game1.MasterPlayer.hasOrWillReceiveMail("ccMovieTheater"))
                  {
                    MovieData upcomingMovie = MovieTheater.GetUpcomingMovie();
                    if (upcomingMovie != null)
                    {
                      Game1.multipleDialogues(new string[1]
                      {
                        Game1.content.LoadString("Strings\\Locations:Theater_Poster_Coming_Soon", (object) TokenParser.ParseText(upcomingMovie.Title))
                      });
                      goto label_537;
                    }
                    goto label_537;
                  }
                  goto label_537;
                }
                goto label_536;
              default:
                goto label_536;
            }
          case 25:
            if (key1 == "SpecialOrdersPrizeTickets")
            {
              if (Game1.player.stats.Get("specialOrderPrizeTickets") > 0U)
              {
                if (Game1.player.couldInventoryAcceptThisItem(ItemRegistry.Create("(O)PrizeTicket")))
                {
                  Game1.player.addItemToInventoryBool(ItemRegistry.Create("(O)PrizeTicket"));
                  int num = (int) Game1.player.stats.Decrement("specialOrderPrizeTickets");
                  Game1.playSound("coin");
                  goto label_537;
                }
                Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Crop.cs.588"));
                goto label_537;
              }
              goto label_537;
            }
            goto label_536;
          default:
            goto label_536;
        }
label_367:
        string path1;
        if (!ArgUtility.TryGet(action, 1, out path1, out error, name: "string translationKey"))
          return LogError(error);
        string dialogue;
        try
        {
          dialogue = Game1.content.LoadStringReturnNullIfNotFound(path1);
        }
        catch (Exception ex)
        {
          dialogue = (string) null;
        }
        if (dialogue != null)
        {
          Game1.drawDialogueNoTyping(dialogue);
          goto label_537;
        }
        Game1.drawDialogueNoTyping(Game1.content.LoadString("Strings\\StringsFromMaps:" + path1.Replace("\"", "")));
        goto label_537;
label_434:
        int whatLevel;
        if (!ArgUtility.TryGetOptionalInt(action, 1, out whatLevel, out error, 1, "int mineLevel"))
          return LogError(error);
        this.playSound("stairsdown");
        Game1.enterMine(whatLevel);
        goto label_537;
label_467:
        this.farmerFile();
label_537:
        return true;
      }
label_536:
      return false;
    }
    if (key1 == "Door")
      this.openDoor(tileLocation, true);
    return false;

    bool LogError(string errorPhrase)
    {
      this.LogTileActionError(action, tileLocation.X, tileLocation.Y, errorPhrase);
      return false;
    }
  }

  public void showPrairieKingMenu()
  {
    if (Game1.player.jotpkProgress.Value == null)
    {
      Game1.currentMinigame = (IMinigame) new AbigailGame();
    }
    else
    {
      Response[] answerChoices = new Response[3]
      {
        new Response("Continue", Game1.content.LoadString("Strings\\Locations:Saloon_Arcade_Cowboy_Continue")),
        new Response("NewGame", Game1.content.LoadString("Strings\\Locations:Saloon_Arcade_Cowboy_NewGame")),
        new Response("Exit", Game1.content.LoadString("Strings\\Locations:Saloon_Arcade_Minecart_Exit"))
      };
      this.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:Saloon_Arcade_Cowboy_Menu"), answerChoices, "CowboyGame");
    }
  }

  /// <summary>Show a minecart destination menu if the network is unlocked.</summary>
  /// <param name="networkId">The network whose destinations to show.</param>
  /// <param name="excludeDestinationId">The destination to hide from the list (usually the ID of the minecart we're using), or <c>null</c> to show all of them.</param>
  public void ShowMineCartMenu(string networkId, string excludeDestinationId)
  {
    if (Game1.player.mount != null)
      return;
    Dictionary<string, MinecartNetworkData> dictionary = DataLoader.Minecarts(Game1.content);
    MinecartNetworkData network;
    if (networkId == null || !dictionary.TryGetValue(networkId, out network))
      Game1.log.Warn($"Can't show minecart menu for unknown network ID '{networkId}'.");
    else if (!GameStateQuery.CheckConditions(network.UnlockCondition, this))
    {
      Game1.drawObjectDialogue(TokenParser.ParseText(network.LockedMessage) ?? Game1.content.LoadString("Strings\\Locations:MineCart_OutOfOrder"));
    }
    else
    {
      MinecartNetworkData minecartNetworkData = network;
      int num1;
      if (minecartNetworkData == null)
      {
        num1 = 0;
      }
      else
      {
        int? count = minecartNetworkData.Destinations?.Count;
        int num2 = 0;
        num1 = count.GetValueOrDefault() > num2 & count.HasValue ? 1 : 0;
      }
      if (num1 == 0)
      {
        Game1.log.Warn($"Can't show minecart menu for network ID '{networkId}' with missing destination data.");
      }
      else
      {
        List<KeyValuePair<string, string>> responses = new List<KeyValuePair<string, string>>();
        Dictionary<string, MinecartDestinationData> destinationLookup = new Dictionary<string, MinecartDestinationData>();
        foreach (MinecartDestinationData destination in network.Destinations)
        {
          if (string.IsNullOrWhiteSpace(destination?.Id) || string.IsNullOrWhiteSpace(destination?.TargetLocation))
            Game1.log.Warn($"Ignored invalid minecart destination '{destination?.Id}' in network '{networkId}' because its ID or location isn't specified.");
          else if (!destination.Id.EqualsIgnoreCase(excludeDestinationId) && GameStateQuery.CheckConditions(destination.Condition, this))
          {
            if (destinationLookup.TryAdd(destination.Id, destination))
            {
              string sub1 = TokenParser.ParseText(destination.DisplayName) ?? destination.TargetLocation;
              if (destination.Price > 0)
                sub1 = Game1.content.LoadString("Strings\\Locations:MineCart_DestinationWithPrice", (object) sub1, (object) destination.Price);
              responses.Add(new KeyValuePair<string, string>(destination.Id, sub1));
            }
            else
              Game1.log.Warn($"Ignored minecart destination with duplicate ID '{destination.Id}' in network '{networkId}'.");
          }
        }
        this.ShowPagedResponses(TokenParser.ParseText(network.ChooseDestinationMessage) ?? Game1.content.LoadString("Strings\\Locations:MineCart_ChooseDestination"), responses, (Action<string>) (destinationId =>
        {
          MinecartDestinationData destination;
          if (!destinationLookup.TryGetValue(destinationId, out destination))
            return;
          int price = destination.Price;
          if (price < 1)
          {
            this.MinecartWarp(destination);
          }
          else
          {
            string numberWithCommas = Utility.getNumberWithCommas(price);
            this.createQuestionDialogue((destination.BuyTicketMessage ?? network.BuyTicketMessage) != null ? string.Format(TokenParser.ParseText(network.BuyTicketMessage), (object) numberWithCommas) : Game1.content.LoadString("Strings\\Locations:BuyTicket", (object) numberWithCommas), this.createYesNoResponses(), (GameLocation.afterQuestionBehavior) ((who, whichAnswer) =>
            {
              if (!(whichAnswer == "Yes"))
                return;
              if (who.Money >= price)
              {
                who.Money -= price;
                this.MinecartWarp(destination);
              }
              else
                Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\UI:NotEnoughMoney1"));
            }));
          }
        }));
      }
    }
  }

  /// <summary>Warp to a minecart destination.</summary>
  /// <param name="destination">The minecart destination data.</param>
  public void MinecartWarp(MinecartDestinationData destination)
  {
    GameLocation gameLocation = Game1.RequireLocation(destination.TargetLocation);
    Point targetTile = destination.TargetTile;
    int parsed;
    if (!Utility.TryParseDirection(destination.TargetDirection, out parsed))
      parsed = 2;
    Game1.player.Halt();
    Game1.player.freezePause = 700;
    Game1.warpFarmer(gameLocation.NameOrUniqueName, targetTile.X, targetTile.Y, parsed);
    if (!Game1.IsPlayingTownMusic || gameLocation.IsOutdoors)
      return;
    Game1.changeMusicTrack("none");
  }

  public void lockedDoorWarp(
    Point tile,
    string locationName,
    int openTime,
    int closeTime,
    string npcName,
    int minFriendship)
  {
    bool flag1 = Game1.player.HasTownKey;
    if (GameLocation.AreStoresClosedForFestival() && this.InValleyContext())
      Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\Locations:FestivalDay_DoorLocked")));
    else if (locationName == "SeedShop" && Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth).Equals("Wed") && !Utility.HasAnyPlayerSeenEvent("191393") && !flag1)
    {
      Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\Locations:SeedShop_LockedWed")));
    }
    else
    {
      if (locationName == "FishShop" && Game1.player.mailReceived.Contains("willyHours"))
        openTime = 800;
      if (flag1)
      {
        if (flag1 && !this.InValleyContext())
          flag1 = false;
        if (flag1 && this is BeachNightMarket && locationName != "FishShop")
          flag1 = false;
      }
      Friendship friendship;
      bool flag2 = (flag1 || Game1.timeOfDay >= openTime && Game1.timeOfDay < closeTime) && (minFriendship <= 0 || this.IsWinterHere() || Game1.player.friendshipData.TryGetValue(npcName, out friendship) && friendship.Points >= minFriendship);
      if (this.IsGreenRainingHere() && Game1.year == 1 && !(this is Beach) && !(this is Forest) && !locationName.Equals("AdventureGuild"))
        flag2 = true;
      if (flag2)
      {
        Rumble.rumble(0.15f, 200f);
        Game1.player.completelyStopAnimatingOrDoingAction();
        this.playSound("doorClose", new Vector2?(Game1.player.Tile));
        Game1.warpFarmer(locationName, tile.X, tile.Y, false);
      }
      else if (minFriendship <= 0)
      {
        string sub1 = Game1.getTimeOfDayString(openTime).Replace(" ", "");
        if (locationName == "FishShop" && Game1.player.mailReceived.Contains("willyHours"))
          sub1 = Game1.getTimeOfDayString(800).Replace(" ", "");
        string sub2 = Game1.getTimeOfDayString(closeTime).Replace(" ", "");
        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:LockedDoor_OpenRange", (object) sub1, (object) sub2));
      }
      else if (Game1.timeOfDay < openTime || Game1.timeOfDay >= closeTime)
      {
        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:LockedDoor"));
      }
      else
      {
        NPC characterFromName = Game1.getCharacterFromName(npcName);
        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:LockedDoor_FriendsOnly", (object) characterFromName.displayName));
      }
    }
  }

  public void playElliottPiano(int key)
  {
    if (Game1.IsMultiplayer && Game1.player.UniqueMultiplayerID % 111L == 0L)
    {
      switch (key)
      {
        case 1:
          this.playSound("toyPiano", pitch: new int?(500));
          break;
        case 2:
          this.playSound("toyPiano", pitch: new int?(1200));
          break;
        case 3:
          this.playSound("toyPiano", pitch: new int?(1400));
          break;
        case 4:
          this.playSound("toyPiano", pitch: new int?(2000));
          break;
      }
    }
    else
    {
      switch (key)
      {
        case 1:
          this.playSound("toyPiano", pitch: new int?(1100));
          break;
        case 2:
          this.playSound("toyPiano", pitch: new int?(1500));
          break;
        case 3:
          this.playSound("toyPiano", pitch: new int?(1600));
          break;
        case 4:
          this.playSound("toyPiano", pitch: new int?(1800));
          break;
      }
      switch (Game1.elliottPiano)
      {
        case 0:
          if (key == 2)
          {
            ++Game1.elliottPiano;
            break;
          }
          Game1.elliottPiano = 0;
          break;
        case 1:
          if (key == 4)
          {
            ++Game1.elliottPiano;
            break;
          }
          Game1.elliottPiano = 0;
          break;
        case 2:
          if (key == 3)
          {
            ++Game1.elliottPiano;
            break;
          }
          Game1.elliottPiano = 0;
          break;
        case 3:
          if (key == 2)
          {
            ++Game1.elliottPiano;
            break;
          }
          Game1.elliottPiano = 0;
          break;
        case 4:
          if (key == 3)
          {
            ++Game1.elliottPiano;
            break;
          }
          Game1.elliottPiano = 0;
          break;
        case 5:
          if (key == 4)
          {
            ++Game1.elliottPiano;
            break;
          }
          Game1.elliottPiano = 0;
          break;
        case 6:
          if (key == 2)
          {
            ++Game1.elliottPiano;
            break;
          }
          Game1.elliottPiano = 0;
          break;
        case 7:
          if (key == 1)
          {
            Game1.elliottPiano = 0;
            NPC characterFromName = this.getCharacterFromName("Elliott");
            if (Game1.eventUp || characterFromName == null || characterFromName.isMoving())
              break;
            characterFromName.faceTowardFarmerForPeriod(1000, 100, false, Game1.player);
            characterFromName.doEmote(20);
            break;
          }
          Game1.elliottPiano = 0;
          break;
      }
    }
  }

  public void readNote(int which)
  {
    if (Game1.netWorldState.Value.LostBooksFound >= which)
    {
      string message = Game1.content.LoadString("Strings\\Notes:" + which.ToString()).Replace('\n', '^');
      Game1.player.mailReceived.Add("lb_" + which.ToString());
      this.removeTemporarySpritesWithIDLocal(which);
      Game1.drawLetterMessage(message);
    }
    else
      Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\Notes:Missing")));
  }

  public void mailbox()
  {
    if (Game1.mailbox.Count > 0)
    {
      string str = Game1.mailbox[0];
      if (!str.Contains("passedOut") && !str.Contains("Cooking"))
        Game1.player.mailReceived.Add(str);
      Game1.mailbox.RemoveAt(0);
      Dictionary<string, string> dictionary = DataLoader.Mail(Game1.content);
      string mail = dictionary.GetValueOrDefault<string, string>(str, "");
      if (str.StartsWith("passedOut"))
      {
        if (str.StartsWith("passedOut "))
        {
          string[] strArray = ArgUtility.SplitBySpace(str);
          int int32 = strArray.Length > 1 ? Convert.ToInt32(strArray[1]) : 0;
          string key;
          switch (Utility.CreateDaySaveRandom((double) int32).Next(Game1.player.getSpouse() == null || !Game1.player.getSpouse().Name.Equals("Harvey") ? 3 : 2))
          {
            case 0:
              key = !Game1.MasterPlayer.hasCompletedCommunityCenter() || Game1.MasterPlayer.mailReceived.Contains("JojaMember") ? $"passedOut1_{(int32 > 0 ? "Billed" : "NotBilled")}_{(Game1.player.IsMale ? "Male" : "Female")}" : "passedOut4";
              break;
            case 1:
              key = "passedOut2";
              break;
            default:
              key = "passedOut3_" + (int32 > 0 ? "Billed" : "NotBilled");
              break;
          }
          mail = string.Format(Dialogue.applyGenderSwitchBlocks(Game1.player.Gender, dictionary[key]), (object) int32);
        }
        else
        {
          string[] strArray = ArgUtility.SplitBySpace(str);
          if (strArray.Length > 1)
          {
            int int32 = Convert.ToInt32(strArray[1]);
            mail = string.Format(Dialogue.applyGenderSwitchBlocks(Game1.player.Gender, dictionary[strArray[0]]), (object) int32);
          }
        }
      }
      if (mail.Length <= 0)
        return;
      Game1.activeClickableMenu = (IClickableMenu) new LetterViewerMenu(mail, str);
    }
    else
    {
      if (Game1.mailbox.Count != 0)
        return;
      Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:GameLocation.cs.8429"));
    }
  }

  public void farmerFile()
  {
    Game1.multipleDialogues(new string[2]
    {
      Game1.content.LoadString("Strings\\UI:FarmerFile_1", (object) Game1.player.Name, (object) Game1.stats.StepsTaken, (object) Game1.stats.GiftsGiven, (object) Game1.stats.DaysPlayed, (object) Game1.stats.DirtHoed, (object) Game1.stats.ItemsCrafted, (object) Game1.stats.ItemsCooked, (object) Game1.stats.PiecesOfTrashRecycled).Replace('\n', '^'),
      Game1.content.LoadString("Strings\\UI:FarmerFile_2", (object) Game1.stats.MonstersKilled, (object) Game1.stats.FishCaught, (object) Game1.stats.TimesFished, (object) Game1.stats.SeedsSown, (object) Game1.stats.ItemsShipped).Replace('\n', '^')
    });
  }

  /// <summary>Get the number of crops currently planted in this location.</summary>
  public int getTotalCrops()
  {
    int totalCrops = 0;
    foreach (TerrainFeature terrainFeature in this.terrainFeatures.Values)
    {
      if (terrainFeature is HoeDirt hoeDirt && hoeDirt.crop != null && !hoeDirt.crop.dead.Value)
        ++totalCrops;
    }
    return totalCrops;
  }

  /// <summary>Get the number of crops currently planted in this location which are ready to harvest.</summary>
  public int getTotalCropsReadyForHarvest()
  {
    int cropsReadyForHarvest = 0;
    foreach (TerrainFeature terrainFeature in this.terrainFeatures.Values)
    {
      if (terrainFeature is HoeDirt hoeDirt && hoeDirt.readyForHarvest())
        ++cropsReadyForHarvest;
    }
    return cropsReadyForHarvest;
  }

  /// <summary>Get the number of crops currently planted in this location which need to be watered.</summary>
  public int getTotalUnwateredCrops()
  {
    int totalUnwateredCrops = 0;
    foreach (TerrainFeature terrainFeature in this.terrainFeatures.Values)
    {
      if (terrainFeature is HoeDirt hoeDirt && hoeDirt.crop != null && hoeDirt.needsWatering() && !hoeDirt.isWatered())
        ++totalUnwateredCrops;
    }
    return totalUnwateredCrops;
  }

  /// <summary>Get the number of crops currently planted in a greenhouse within this location.</summary>
  public int? getTotalGreenhouseCropsReadyForHarvest()
  {
    if (!Game1.MasterPlayer.mailReceived.Contains("ccPantry"))
      return new int?();
    int num = 0;
    foreach (TerrainFeature terrainFeature in Game1.RequireLocation("Greenhouse").terrainFeatures.Values)
    {
      if (terrainFeature is HoeDirt hoeDirt && hoeDirt.readyForHarvest())
        ++num;
    }
    return new int?(num);
  }

  /// <summary>Get the number of tiles currently tilled in this location which don't contain a crop.</summary>
  public int getTotalOpenHoeDirt()
  {
    int totalOpenHoeDirt = 0;
    foreach (TerrainFeature terrainFeature in this.terrainFeatures.Values)
    {
      if (terrainFeature is HoeDirt hoeDirt && hoeDirt.crop == null && !this.objects.ContainsKey(terrainFeature.Tile))
        ++totalOpenHoeDirt;
    }
    return totalOpenHoeDirt;
  }

  /// <summary>Get the number of forage items currently in this location.</summary>
  public int getTotalForageItems()
  {
    int totalForageItems = 0;
    foreach (Object @object in this.objects.Values)
    {
      if (@object.isSpawnedObject.Value)
        ++totalForageItems;
    }
    return totalForageItems;
  }

  /// <summary>Get the number of machines within this location with output ready to collect.</summary>
  public int getNumberOfMachinesReadyForHarvest()
  {
    int machinesReadyForHarvest = 0;
    foreach (Object @object in this.objects.Values)
    {
      if (@object.IsConsideredReadyMachineForComputer())
        ++machinesReadyForHarvest;
    }
    string name = (string) null;
    GameLocation gameLocation = this;
    if (!(gameLocation is Farm))
    {
      if (gameLocation is IslandWest islandWest && islandWest.farmhouseRestored.Value)
        name = "IslandFarmHouse";
    }
    else
      name = "FarmHouse";
    if (name != null)
    {
      foreach (Object @object in Game1.RequireLocation(name).objects.Values)
      {
        if (@object.IsConsideredReadyMachineForComputer())
          ++machinesReadyForHarvest;
      }
    }
    foreach (Building building in this.buildings)
    {
      GameLocation indoors = building.GetIndoors();
      if (indoors != null)
      {
        foreach (Object @object in indoors.objects.Values)
        {
          if (@object.IsConsideredReadyMachineForComputer())
            ++machinesReadyForHarvest;
        }
      }
    }
    return machinesReadyForHarvest;
  }

  public static void openCraftingMenu()
  {
    Game1.activeClickableMenu = (IClickableMenu) new GameMenu(GameMenu.craftingTab);
  }

  /// <summary>Handle an <c>Action Buy</c> tile property in this location.</summary>
  /// <param name="which">The legacy shop ID. This is not necessarily the same ID used in <c>Data/ShopData</c>.</param>
  /// <remarks>This is used to apply hardcoded game logic (like showing a message when Pierre is visiting the island). Most code should use <c>Action OpenShop</c> or <see cref="M:StardewValley.Utility.TryOpenShopMenu(System.String,System.String,System.Boolean)" /> instead.</remarks>
  public virtual bool HandleBuyAction(string which)
  {
    if (which.Equals("Fish"))
      return Utility.TryOpenShopMenu("FishShop", this, maxOwnerY: new int?(Game1.player.TilePoint.Y - 1));
    if (this is SeedShop)
    {
      if (this.getCharacterFromName("Pierre") == null && Game1.IsVisitingIslandToday("Pierre"))
      {
        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:SeedShop_MoneyBox"));
        Game1.afterDialogues = (Game1.afterFadeFunction) (() => Utility.TryOpenShopMenu("SeedShop", (string) null));
      }
      else
        Utility.TryOpenShopMenu("SeedShop", this, new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(4, 17, 1, 1)), new int?(Game1.player.TilePoint.Y - 1));
      return true;
    }
    if (!this.name.Equals((object) "SandyHouse"))
      return false;
    Utility.TryOpenShopMenu("Sandy", this);
    return true;
  }

  public virtual bool isObjectAt(int x, int y)
  {
    Vector2 key = new Vector2((float) (x / 64 /*0x40*/), (float) (y / 64 /*0x40*/));
    foreach (Object @object in this.furniture)
    {
      if (@object.boundingBox.Value.Contains(x, y))
        return true;
    }
    return this.objects.ContainsKey(key);
  }

  public virtual bool isObjectAtTile(int tileX, int tileY)
  {
    Vector2 key = new Vector2((float) tileX, (float) tileY);
    foreach (Object @object in this.furniture)
    {
      if (@object.boundingBox.Value.Contains(tileX * 64 /*0x40*/, tileY * 64 /*0x40*/))
        return true;
    }
    return this.objects.ContainsKey(key);
  }

  public virtual Object getObjectAt(int x, int y, bool ignorePassables = false)
  {
    Vector2 key = new Vector2((float) (x / 64 /*0x40*/), (float) (y / 64 /*0x40*/));
    foreach (Furniture objectAt in this.furniture)
    {
      if (objectAt.boundingBox.Value.Contains(x, y) && (!ignorePassables || !objectAt.isPassable()))
        return (Object) objectAt;
    }
    Object objectAt1 = (Object) null;
    this.objects.TryGetValue(key, out objectAt1);
    if (ignorePassables && objectAt1 != null && objectAt1.isPassable())
      objectAt1 = (Object) null;
    return objectAt1;
  }

  public Object getObjectAtTile(int x, int y, bool ignorePassables = false)
  {
    return this.getObjectAt(x * 64 /*0x40*/, y * 64 /*0x40*/, ignorePassables);
  }

  public virtual bool saloon(Location tileLocation)
  {
    NPC characterFromName = this.getCharacterFromName("Gus");
    if (Utility.TryOpenShopMenu("Saloon", this, new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(9, 17, 10, 2))))
    {
      characterFromName?.facePlayer(Game1.player);
      return true;
    }
    if (characterFromName != null || !Game1.IsVisitingIslandToday("Gus"))
      return false;
    Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Saloon_MoneyBox"));
    Game1.afterDialogues = (Game1.afterFadeFunction) (() => Utility.TryOpenShopMenu("Saloon", (string) null));
    return true;
  }

  private void adventureShop()
  {
    if (Game1.player.itemsLostLastDeath.Count > 0)
    {
      List<Response> responseList = new List<Response>()
      {
        new Response("Shop", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_Shop")),
        new Response("Recovery", Game1.content.LoadString("Strings\\Locations:AdventureGuild_ItemRecovery")),
        new Response("Leave", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_Leave"))
      };
      this.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:AdventureGuild_Greeting"), responseList.ToArray(), "adventureGuild");
    }
    else
      Utility.TryOpenShopMenu("AdventureShop", "Marlon");
  }

  public virtual bool carpenters(Location tileLocation)
  {
    foreach (NPC character in this.characters)
    {
      if (character.Name.Equals("Robin"))
      {
        if ((double) Vector2.Distance(character.Tile, new Vector2((float) tileLocation.X, (float) tileLocation.Y)) > 3.0)
          return false;
        character.faceDirection(2);
        if (Game1.player.daysUntilHouseUpgrade.Value < 0 && !Game1.IsThereABuildingUnderConstruction())
        {
          List<Response> responseList = new List<Response>();
          responseList.Add(new Response("Shop", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_Shop")));
          if (Game1.IsMasterGame)
          {
            if (Game1.player.houseUpgradeLevel.Value < 3)
              responseList.Add(new Response("Upgrade", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_UpgradeHouse")));
            else if ((Game1.MasterPlayer.mailReceived.Contains("ccIsComplete") || Game1.MasterPlayer.mailReceived.Contains("JojaMember") || Game1.MasterPlayer.hasCompletedCommunityCenter()) && Game1.RequireLocation<Town>("Town").daysUntilCommunityUpgrade.Value <= 0)
            {
              if (!Game1.MasterPlayer.mailReceived.Contains("pamHouseUpgrade"))
                responseList.Add(new Response("CommunityUpgrade", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_CommunityUpgrade")));
              else if (!Game1.MasterPlayer.mailReceived.Contains("communityUpgradeShortcuts"))
                responseList.Add(new Response("CommunityUpgrade", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_CommunityUpgrade")));
            }
          }
          else if (Game1.player.houseUpgradeLevel.Value < 3)
            responseList.Add(new Response("Upgrade", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_UpgradeCabin")));
          if (Game1.player.houseUpgradeLevel.Value >= 2)
          {
            if (Game1.IsMasterGame)
              responseList.Add(new Response("Renovate", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_RenovateHouse")));
            else
              responseList.Add(new Response("Renovate", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_RenovateCabin")));
          }
          responseList.Add(new Response("Construct", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_Construct")));
          responseList.Add(new Response("Leave", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_Leave")));
          this.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu"), responseList.ToArray(), "carpenter");
        }
        else
          Utility.TryOpenShopMenu("Carpenter", "Robin");
        return true;
      }
    }
    if (this.getCharacterFromName("Robin") == null && Game1.IsVisitingIslandToday("Robin"))
    {
      Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:ScienceHouse_MoneyBox"));
      Game1.afterDialogues = (Game1.afterFadeFunction) (() => Utility.TryOpenShopMenu("Carpenter", (string) null));
      return true;
    }
    if (!Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth).Equals("Tue"))
      return false;
    Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:ScienceHouse_RobinAbsent").Replace('\n', '^'));
    return true;
  }

  public virtual bool blacksmith(Location tileLocation)
  {
    foreach (NPC character in this.characters)
    {
      if (character.Name.Equals("Clint"))
      {
        if (character.Tile != new Vector2((float) tileLocation.X, (float) (tileLocation.Y - 1)))
        {
          int num = character.Tile != new Vector2((float) (tileLocation.X - 1), (float) (tileLocation.Y - 1)) ? 1 : 0;
        }
        character.faceDirection(2);
        if (Game1.player.toolBeingUpgraded.Value != null && Game1.player.daysLeftForToolUpgrade.Value <= 0)
        {
          if (Game1.player.freeSpotsInInventory() > 0 || Game1.player.toolBeingUpgraded.Value is GenericTool)
          {
            Tool tool = Game1.player.toolBeingUpgraded.Value;
            Game1.player.toolBeingUpgraded.Value = (Tool) null;
            Game1.player.hasReceivedToolUpgradeMessageYet = false;
            Game1.player.holdUpItemThenMessage((Item) tool);
            if (tool is GenericTool)
              tool.actionWhenClaimed();
            else
              Game1.player.addItemToInventoryBool((Item) tool);
            if (Game1.player.team.useSeparateWallets.Value && tool.UpgradeLevel == 4)
              Game1.multiplayer.globalChatInfoMessage("IridiumToolUpgrade", Game1.player.Name, TokenStringBuilder.ToolName(tool.QualifiedItemId, tool.UpgradeLevel));
          }
          else
            Game1.DrawDialogue(character, "Data\\ExtraDialogue:Clint_NoInventorySpace");
        }
        else
        {
          bool flag = false;
          foreach (Item obj in Game1.player.Items)
          {
            if (Utility.IsGeode(obj))
            {
              flag = true;
              break;
            }
          }
          Response[] answerChoices;
          if (flag)
            answerChoices = new Response[4]
            {
              new Response("Shop", Game1.content.LoadString("Strings\\Locations:Blacksmith_Clint_Shop")),
              new Response("Upgrade", Game1.content.LoadString("Strings\\Locations:Blacksmith_Clint_Upgrade")),
              new Response("Process", Game1.content.LoadString("Strings\\Locations:Blacksmith_Clint_Geodes")),
              new Response("Leave", Game1.content.LoadString("Strings\\Locations:Blacksmith_Clint_Leave"))
            };
          else
            answerChoices = new Response[3]
            {
              new Response("Shop", Game1.content.LoadString("Strings\\Locations:Blacksmith_Clint_Shop")),
              new Response("Upgrade", Game1.content.LoadString("Strings\\Locations:Blacksmith_Clint_Upgrade")),
              new Response("Leave", Game1.content.LoadString("Strings\\Locations:Blacksmith_Clint_Leave"))
            };
          this.createQuestionDialogue("", answerChoices, "Blacksmith");
        }
        return true;
      }
    }
    return false;
  }

  public virtual bool animalShop(Location tileLocation)
  {
    foreach (NPC character in this.characters)
    {
      if (character.Name.Equals("Marnie"))
      {
        if (character.Tile != new Vector2((float) tileLocation.X, (float) (tileLocation.Y - 1)) && character.Tile != new Vector2((float) (tileLocation.X - 1), (float) (tileLocation.Y - 1)))
        {
          if (Game1.player.stats.Get("Book_AnimalCatalogue") <= 0U)
            return false;
          break;
        }
        character.faceDirection(2);
        List<Response> responseList = new List<Response>()
        {
          new Response("Supplies", Game1.content.LoadString("Strings\\Locations:AnimalShop_Marnie_Supplies")),
          new Response("Purchase", Game1.content.LoadString("Strings\\Locations:AnimalShop_Marnie_Animals")),
          new Response("Leave", Game1.content.LoadString("Strings\\Locations:AnimalShop_Marnie_Leave"))
        };
        if ((Utility.getAllPets().Count != 0 ? 0 : (Game1.year >= 2 ? 1 : 0)) != 0 || Game1.player.mailReceived.Contains("MarniePetAdoption") || Game1.player.mailReceived.Contains("MarniePetRejectedAdoption"))
          responseList.Insert(2, new Response("Adopt", Game1.content.LoadString("Strings\\1_6_Strings:AdoptPets")));
        this.createQuestionDialogue("", responseList.ToArray(), "Marnie");
        return true;
      }
    }
    if (this.getCharacterFromName("Marnie") == null && Game1.IsVisitingIslandToday("Marnie"))
    {
      Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:AnimalShop_MoneyBox"));
      Game1.afterDialogues = (Game1.afterFadeFunction) (() => Utility.TryOpenShopMenu("AnimalShop", (string) null));
      return true;
    }
    if (Game1.player.stats.Get("Book_AnimalCatalogue") > 0U)
    {
      Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\1_6_Strings:Marnie_Counter"));
      Game1.afterDialogues = (Game1.afterFadeFunction) (() =>
      {
        List<Response> responseList = new List<Response>()
        {
          new Response("Supplies", Game1.content.LoadString("Strings\\Locations:AnimalShop_Marnie_Supplies")),
          new Response("Purchase", Game1.content.LoadString("Strings\\Locations:AnimalShop_Marnie_Animals")),
          new Response("Leave", Game1.content.LoadString("Strings\\Locations:AnimalShop_Marnie_Leave"))
        };
        if ((Utility.getAllPets().Count != 0 ? 0 : (Game1.year >= 2 ? 1 : 0)) != 0 || Game1.player.mailReceived.Contains("MarniePetAdoption") || Game1.player.mailReceived.Contains("MarniePetRejectedAdoption"))
          responseList.Insert(2, new Response("Adopt", Game1.content.LoadString("Strings\\1_6_Strings:AdoptPets")));
        this.createQuestionDialogue("", responseList.ToArray(), "Marnie");
      });
      return true;
    }
    if (!Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth).Equals("Tue"))
      return false;
    Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:AnimalShop_Marnie_Absent").Replace('\n', '^'));
    return true;
  }

  public void removeTile(Location tileLocation, string layer)
  {
    this.Map.RequireLayer(layer).Tiles[tileLocation.X, tileLocation.Y] = (Tile) null;
  }

  public void removeTile(int x, int y, string layer)
  {
    this.Map.RequireLayer(layer).Tiles[x, y] = (Tile) null;
  }

  public void characterTrampleTile(Vector2 tile)
  {
    switch (this)
    {
      case FarmHouse _:
        break;
      case IslandFarmHouse _:
        break;
      case Farm _:
        break;
      default:
        TerrainFeature terrainFeature;
        this.terrainFeatures.TryGetValue(tile, out terrainFeature);
        if (!(terrainFeature is Tree tree) || tree.growthStage.Value >= 1 || !tree.instantDestroy(tile))
          break;
        this.terrainFeatures.Remove(tile);
        break;
    }
  }

  public bool characterDestroyObjectWithinRectangle(Microsoft.Xna.Framework.Rectangle rect, bool showDestroyedObject)
  {
    switch (this)
    {
      case FarmHouse _:
      case IslandFarmHouse _:
        return false;
      default:
        foreach (Farmer farmer in this.farmers)
        {
          if (rect.Intersects(farmer.GetBoundingBox()))
            return false;
        }
        Vector2 vector2 = new Vector2((float) (rect.X / 64 /*0x40*/), (float) (rect.Y / 64 /*0x40*/));
        Object o;
        this.objects.TryGetValue(vector2, out o);
        if (this.checkDestroyItem(o, vector2, showDestroyedObject))
          return true;
        TerrainFeature tf;
        this.terrainFeatures.TryGetValue(vector2, out tf);
        if (this.checkDestroyTerrainFeature(tf, vector2))
          return true;
        vector2.X = (float) (rect.Right / 64 /*0x40*/);
        this.objects.TryGetValue(vector2, out o);
        if (this.checkDestroyItem(o, vector2, showDestroyedObject))
          return true;
        this.terrainFeatures.TryGetValue(vector2, out tf);
        if (this.checkDestroyTerrainFeature(tf, vector2))
          return true;
        vector2.X = (float) (rect.X / 64 /*0x40*/);
        vector2.Y = (float) (rect.Bottom / 64 /*0x40*/);
        this.objects.TryGetValue(vector2, out o);
        if (this.checkDestroyItem(o, vector2, showDestroyedObject))
          return true;
        this.terrainFeatures.TryGetValue(vector2, out tf);
        if (this.checkDestroyTerrainFeature(tf, vector2))
          return true;
        vector2.X = (float) (rect.Right / 64 /*0x40*/);
        this.objects.TryGetValue(vector2, out o);
        if (this.checkDestroyItem(o, vector2, showDestroyedObject))
          return true;
        this.terrainFeatures.TryGetValue(vector2, out tf);
        if (this.checkDestroyTerrainFeature(tf, vector2))
          return true;
        for (int index = this.largeTerrainFeatures.Count - 1; index >= 0; --index)
        {
          LargeTerrainFeature largeTerrainFeature = this.largeTerrainFeatures[index];
          if (largeTerrainFeature.isDestroyedByNPCTrample && largeTerrainFeature.getBoundingBox().Intersects(rect))
          {
            largeTerrainFeature.onDestroy();
            this.largeTerrainFeatures.RemoveAt(index);
            return true;
          }
        }
        for (int index = this.resourceClumps.Count - 1; index >= 0; --index)
        {
          ResourceClump resourceClump = this.resourceClumps[index];
          if (resourceClump.IsGreenRainBush() && resourceClump.getBoundingBox().Intersects(rect) && resourceClump.destroy((Tool) null, this, resourceClump.Tile))
            this.resourceClumps.RemoveAt(index);
        }
        return false;
    }
  }

  private bool checkDestroyTerrainFeature(TerrainFeature tf, Vector2 tilePositionToTry)
  {
    if (tf is Tree tree && tree.instantDestroy(tilePositionToTry))
      this.terrainFeatures.Remove(tilePositionToTry);
    return false;
  }

  private bool checkDestroyItem(Object o, Vector2 tilePositionToTry, bool showDestroyedObject)
  {
    if (o == null || o.isPassable() || this.map.RequireLayer("Back").Tiles[(int) tilePositionToTry.X, (int) tilePositionToTry.Y].Properties.ContainsKey("NPCBarrier"))
      return false;
    if (o.IsSpawnedObject)
      --this.numberOfSpawnedObjectsOnMap;
    if (showDestroyedObject && !o.bigCraftable.Value)
    {
      TemporaryAnimatedSprite temporaryAnimatedSprite = new TemporaryAnimatedSprite(0, 150f, 1, 3, new Vector2(tilePositionToTry.X * 64f, tilePositionToTry.Y * 64f), false, o.flipped.Value)
      {
        alphaFade = 0.01f
      };
      temporaryAnimatedSprite.CopyAppearanceFromItemId(o.QualifiedItemId);
      Game1.multiplayer.broadcastSprites(this, temporaryAnimatedSprite);
    }
    o.performToolAction((Tool) null);
    if (this.objects.ContainsKey(tilePositionToTry))
    {
      if (o is Chest chest)
      {
        if (chest.TryMoveToSafePosition())
          return true;
        chest.destroyAndDropContents(tilePositionToTry * 64f);
      }
      this.objects.Remove(tilePositionToTry);
    }
    return true;
  }

  public Object removeObject(Vector2 location, bool showDestroyedObject)
  {
    Object object1;
    this.objects.TryGetValue(location, out object1);
    if (object1 == null || !(object1.CanBeGrabbed | showDestroyedObject))
      return (Object) null;
    if (object1.IsSpawnedObject)
      --this.numberOfSpawnedObjectsOnMap;
    Object object2 = this.objects[location];
    this.objects.Remove(location);
    if (showDestroyedObject)
    {
      TemporaryAnimatedSprite temporaryAnimatedSprite = new TemporaryAnimatedSprite(0, 150f, 1, 3, new Vector2(location.X * 64f, location.Y * 64f), true, object2.bigCraftable.Value, object2.flipped.Value);
      temporaryAnimatedSprite.CopyAppearanceFromItemId(object2.QualifiedItemId, !(object2.Type == "Crafting") ? 1 : 0);
      Game1.multiplayer.broadcastSprites(this, temporaryAnimatedSprite);
    }
    if (object1.IsWeeds())
      ++Game1.stats.WeedsEliminated;
    return object2;
  }

  public void removeTileProperty(int tileX, int tileY, string layer, string key)
  {
    try
    {
      ((IDictionary<string, PropertyValue>) this.map?.GetLayer(layer)?.Tiles[tileX, tileY]?.Properties).Remove(key);
    }
    catch (Exception ex)
    {
    }
  }

  public void setTileProperty(int tileX, int tileY, string layer, string key, string value)
  {
    try
    {
      Tile tile = this.map?.GetLayer(layer)?.Tiles[tileX, tileY];
      if (tile == null)
        return;
      tile.Properties[key] = (PropertyValue) value;
    }
    catch (Exception ex)
    {
    }
  }

  public void setObjectAt(float x, float y, Object o) => this.objects[new Vector2(x, y)] = o;

  public virtual void cleanupBeforeSave()
  {
    this.characters.RemoveWhere((Func<NPC, bool>) (npc => npc is Junimo));
    if (this.name.Equals((object) "WitchHut"))
      this.characters.Clear();
    this.largeTerrainFeatures.RemoveWhere((Func<LargeTerrainFeature, bool>) (feature => feature is Tent));
    foreach (Building building in this.buildings)
      building.indoors.Value?.cleanupBeforeSave();
  }

  public virtual void cleanupForVacancy()
  {
    if (!Game1.IsMasterGame)
      return;
    this.debris.RemoveWhere((Func<Debris, bool>) (d => d.isEssentialItem() && d.collect(Game1.player)));
  }

  public virtual void cleanupBeforePlayerExit()
  {
    this.debris.RemoveWhere((Func<Debris, bool>) (d => d.isEssentialItem() && d.player.Value != null && d.player.Value == Game1.player && d.collect(d.player.Value)));
    Game1.currentLightSources.Clear();
    this.critters?.Clear();
    Game1.onScreenMenus.RemoveWhere<IClickableMenu>((Predicate<IClickableMenu>) (menu =>
    {
      if (!menu.destroy)
        return false;
      if (menu is IDisposable disposable2)
        disposable2.Dispose();
      return true;
    }));
    AmbientLocationSounds.onLocationLeave();
    Game1.player.rightRing.Value?.onLeaveLocation(Game1.player, this);
    Game1.player.leftRing.Value?.onLeaveLocation(Game1.player, this);
    if (this.name.Equals((object) "AbandonedJojaMart") && this.farmers.Count <= 1)
      this.characters.RemoveWhere((Func<NPC, bool>) (npc => npc is Junimo));
    this.furnitureToRemove.Clear();
    this.interiorDoors.CleanUpLocalState();
    Game1.temporaryContent.Unload();
    Utility.CollectGarbage();
  }

  public static string getWeedForSeason(Random r, Season season)
  {
    switch (season)
    {
      case Season.Spring:
        return r.Choose<string>("(O)784", "(O)674", "(O)675");
      case Season.Summer:
        return r.Choose<string>("(O)785", "(O)676", "(O)677");
      case Season.Fall:
        return r.Choose<string>("(O)786", "(O)678", "(O)679");
      default:
        return "(O)674";
    }
  }

  private void startSleep()
  {
    Game1.player.timeWentToBed.Value = Game1.timeOfDay;
    if (Game1.IsMultiplayer)
    {
      Game1.netReady.SetLocalReady("sleep", true);
      Game1.dialogueUp = false;
      Game1.activeClickableMenu = (IClickableMenu) new ReadyCheckDialog("sleep", true, (ConfirmationDialog.behavior) (who => this.doSleep()), (ConfirmationDialog.behavior) (who =>
      {
        if (Game1.activeClickableMenu is ReadyCheckDialog activeClickableMenu2)
          activeClickableMenu2.closeDialog(who);
        who.timeWentToBed.Value = 0;
      }));
    }
    else
      this.doSleep();
    if (Game1.IsDedicatedHost || Game1.player.team.announcedSleepingFarmers.Contains(Game1.player))
      return;
    Game1.player.team.announcedSleepingFarmers.Add(Game1.player);
    if (!Game1.IsMultiplayer || Game1.player.team.sleepAnnounceMode.Value != FarmerTeam.SleepAnnounceModes.All && (Game1.player.team.sleepAnnounceMode.Value != FarmerTeam.SleepAnnounceModes.First || Game1.player.team.announcedSleepingFarmers.Count != 1))
      return;
    string str = "GoneToBed";
    if (Game1.random.NextDouble() < 0.75)
    {
      if (Game1.timeOfDay < 1800)
        str += "Early";
      else if (Game1.timeOfDay > 2530)
        str += "Late";
    }
    int num = 0;
    for (int index = 0; index < 2; ++index)
    {
      if (Game1.random.NextDouble() < 0.25)
        ++num;
    }
    Game1.multiplayer.globalChatInfoMessage(str + num.ToString(), Game1.player.displayName);
  }

  protected virtual void _CleanupPagedResponses()
  {
    GameLocation._PagedResponses.Clear();
    GameLocation._OnPagedResponse = (Action<string>) null;
    GameLocation._PagedResponsePrompt = (string) null;
  }

  public virtual void ShowPagedResponses(
    string prompt,
    List<KeyValuePair<string, string>> responses,
    Action<string> on_response,
    bool auto_select_single_choice = false,
    bool addCancel = true,
    int itemsPerPage = 5)
  {
    GameLocation._PagedResponses.Clear();
    GameLocation._PagedResponses.AddRange((IEnumerable<KeyValuePair<string, string>>) responses);
    GameLocation._PagedResponsePage = 0;
    GameLocation._PagedResponseAddCancel = addCancel;
    GameLocation._PagedResponseItemsPerPage = itemsPerPage;
    GameLocation._PagedResponsePrompt = prompt;
    GameLocation._OnPagedResponse = on_response;
    if (GameLocation._PagedResponses.Count == 1 & auto_select_single_choice)
    {
      on_response(GameLocation._PagedResponses[0].Key);
    }
    else
    {
      if (GameLocation._PagedResponses.Count <= 0)
        return;
      this._ShowPagedResponses(GameLocation._PagedResponsePage);
    }
  }

  protected virtual void _ShowPagedResponses(int page = -1)
  {
    GameLocation._PagedResponsePage = page;
    int responseItemsPerPage = GameLocation._PagedResponseItemsPerPage;
    int num1 = (GameLocation._PagedResponses.Count - 1) / responseItemsPerPage;
    int num2 = responseItemsPerPage;
    if (GameLocation._PagedResponsePage == num1 - 1 && GameLocation._PagedResponses.Count % responseItemsPerPage == 1)
    {
      ++num2;
      --num1;
    }
    List<Response> responseList = new List<Response>();
    for (int index1 = 0; index1 < num2; ++index1)
    {
      int index2 = index1 + GameLocation._PagedResponsePage * responseItemsPerPage;
      if (index2 < GameLocation._PagedResponses.Count)
      {
        KeyValuePair<string, string> pagedResponse = GameLocation._PagedResponses[index2];
        responseList.Add(new Response(pagedResponse.Key, pagedResponse.Value));
      }
    }
    if (GameLocation._PagedResponsePage < num1)
      responseList.Add(new Response("nextPage", Game1.content.LoadString("Strings\\UI:NextPage")));
    if (GameLocation._PagedResponsePage > 0)
      responseList.Add(new Response("previousPage", Game1.content.LoadString("Strings\\UI:PreviousPage")));
    if (GameLocation._PagedResponseAddCancel)
      responseList.Add(new Response("cancel", Game1.content.LoadString("Strings\\Locations:MineCart_Destination_Cancel")));
    this.createQuestionDialogue(GameLocation._PagedResponsePrompt, responseList.ToArray(), "pagedResponse");
  }

  /// <summary>Show a dialogue menu to choose where to construct buildings.</summary>
  /// <param name="builder">The name of the NPC whose building menu is being shown (the vanilla values are <see cref="F:StardewValley.Game1.builder_robin" /> and <see cref="F:StardewValley.Game1.builder_wizard" />).</param>
  /// <param name="page">The page of location names to show, if there are multiple pages.</param>
  public virtual void ShowConstructOptions(string builder, int page = -1)
  {
    if (builder != null)
      this._constructLocationBuilderName = builder;
    List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
    foreach (GameLocation location in (IEnumerable<GameLocation>) Game1.locations)
    {
      if (location.IsBuildableLocation())
        keyValuePairList.Add(new KeyValuePair<string, string>(location.NameOrUniqueName, location.DisplayName));
    }
    if (!keyValuePairList.Any<KeyValuePair<string, string>>())
    {
      Farm farm = Game1.getFarm();
      keyValuePairList.Add(new KeyValuePair<string, string>(farm.NameOrUniqueName, farm.DisplayName));
    }
    this.ShowPagedResponses(Game1.content.LoadString("Strings\\Buildings:Construction_ChooseLocation"), keyValuePairList, (Action<string>) (value =>
    {
      GameLocation locationFromName = Game1.getLocationFromName(value);
      if (locationFromName != null)
        Game1.activeClickableMenu = (IClickableMenu) new CarpenterMenu(this._constructLocationBuilderName, locationFromName);
      else
        Game1.log.Error($"Can't find location '{value}' for construct menu.");
    }), true);
  }

  /// <summary>Show a shop menu to select a location (if multiple have animal buildings) and purchase animals.</summary>
  /// <param name="onMenuOpened">An callback to invoke when the purchase menu is opened.</param>
  public void ShowAnimalShopMenu(Action<PurchaseAnimalsMenu> onMenuOpened = null)
  {
    List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
    foreach (GameLocation location in (IEnumerable<GameLocation>) Game1.locations)
    {
      if (location.buildings.Any<Building>((Func<Building, bool>) (p => p.GetIndoors() is AnimalHouse)) && (!Game1.IsClient || location.CanBeRemotedlyViewed()))
        keyValuePairList.Add(new KeyValuePair<string, string>(location.NameOrUniqueName, location.DisplayName));
    }
    if (!keyValuePairList.Any<KeyValuePair<string, string>>())
    {
      Farm farm = Game1.getFarm();
      keyValuePairList.Add(new KeyValuePair<string, string>(farm.NameOrUniqueName, farm.DisplayName));
    }
    Game1.currentLocation.ShowPagedResponses(Game1.content.LoadString("Strings\\StringsFromCSFiles:PurchaseAnimalsMenu.ChooseLocation"), keyValuePairList, (Action<string>) (value =>
    {
      GameLocation locationFromName = Game1.getLocationFromName(value);
      if (locationFromName != null)
      {
        PurchaseAnimalsMenu purchaseAnimalsMenu = new PurchaseAnimalsMenu(Utility.getPurchaseAnimalStock(locationFromName), locationFromName);
        Action<PurchaseAnimalsMenu> action = onMenuOpened;
        if (action != null)
          action(purchaseAnimalsMenu);
        Game1.activeClickableMenu = (IClickableMenu) purchaseAnimalsMenu;
      }
      else
        Game1.log.Error($"Can't find location '{value}' for animal purchase menu.");
    }), true);
  }

  private void doSleep()
  {
    if ((double) this.lightLevel.Value == 0.0 && Game1.timeOfDay < 2000)
    {
      if (!this.isOutdoors.Value)
      {
        this.lightLevel.Value = 0.6f;
        this.localSound("openBox");
      }
      if (Game1.IsMasterGame)
        Game1.NewDay(600f);
    }
    else if ((double) this.lightLevel.Value > 0.0 && Game1.timeOfDay >= 2000)
    {
      if (!this.isOutdoors.Value)
      {
        this.lightLevel.Value = 0.0f;
        this.localSound("openBox");
      }
      if (Game1.IsMasterGame)
        Game1.NewDay(600f);
    }
    else if (Game1.IsMasterGame)
      Game1.NewDay(0.0f);
    Game1.player.lastSleepLocation.Value = Game1.currentLocation.NameOrUniqueName;
    Game1.player.lastSleepPoint.Value = Game1.player.TilePoint;
    Game1.player.mostRecentBed = Game1.player.Position;
    Game1.player.doEmote(24);
    Game1.player.freezePause = 2000;
  }

  public virtual bool answerDialogueAction(string questionAndAnswer, string[] questionParams)
  {
    if (questionAndAnswer == null)
      return false;
    if (questionAndAnswer != null)
    {
      switch (questionAndAnswer.Length)
      {
        case 6:
          if (questionAndAnswer == "Eat_No")
          {
            Game1.player.isEating = false;
            Game1.player.completelyStopAnimatingOrDoingAction();
            goto label_390;
          }
          goto label_387;
        case 7:
          switch (questionAndAnswer[0])
          {
            case 'E':
              if (questionAndAnswer == "Eat_Yes")
              {
                Game1.player.isEating = false;
                Game1.player.eatHeldObject();
                goto label_390;
              }
              goto label_387;
            case 'M':
              if (questionAndAnswer == "Mine_No")
              {
                Response[] answerChoices = new Response[2]
                {
                  new Response("No", Game1.content.LoadString("Strings\\Lexicon:QuestionDialogue_No")),
                  new Response("Yes", Game1.content.LoadString("Strings\\Lexicon:QuestionDialogue_Yes"))
                };
                this.createQuestionDialogue(Game1.parseText(Game1.content.LoadString("Strings\\Locations:Mines_ResetMine")), answerChoices, "ResetMine");
                goto label_390;
              }
              goto label_387;
            default:
              goto label_387;
          }
        case 8:
          switch (questionAndAnswer[0])
          {
            case 'F':
              if (questionAndAnswer == "Fizz_Yes")
              {
                if (Game1.player.Money >= 500000)
                {
                  Game1.player.Money -= 500000;
                  ++Game1.netWorldState.Value.PerfectionWaivers;
                  DelayedAction.playSoundAfterDelay("qi_shop_purchase", 500);
                  this.getCharacterFromName("Fizz")?.showTextAboveHead(Game1.content.LoadString("Strings\\1_6_Strings:Fizz_Sweet"));
                  this.getCharacterFromName("Fizz")?.shake(500);
                  if (Game1.IsMultiplayer)
                  {
                    Game1.Multiplayer.broadcastGlobalMessage("Strings\\1_6_Strings:Waiver_Note_Multiplayer", false, (GameLocation) null, Game1.player.Name);
                    goto label_390;
                  }
                  Game1.showGlobalMessage(string.Format(Game1.content.LoadString("Strings\\1_6_Strings:Waiver_Note", (object) (Game1.netWorldState.Value.PerfectionWaivers.ToString() ?? ""))));
                  goto label_390;
                }
                Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\UI:NotEnoughMoney1"));
                goto label_390;
              }
              goto label_387;
            case 'M':
              if (questionAndAnswer == "Mine_Yes")
              {
                if (Game1.CurrentMineLevel > 120)
                {
                  Game1.warpFarmer("SkullCave", 3, 4, 2);
                  goto label_390;
                }
                Game1.warpFarmer("UndergroundMine", 16 /*0x10*/, 16 /*0x10*/, false);
                goto label_390;
              }
              goto label_387;
            default:
              goto label_387;
          }
        case 9:
          if (questionAndAnswer == "Sleep_Yes")
          {
            this.startSleep();
            goto label_390;
          }
          goto label_387;
        case 10:
          switch (questionAndAnswer[0])
          {
            case 'D':
              if (questionAndAnswer == "Dungeon_Go")
              {
                Game1.enterMine(Game1.CurrentMineLevel + 1);
                goto label_390;
              }
              goto label_387;
            case 'M':
              if (questionAndAnswer == "Mine_Enter")
              {
                Game1.enterMine(1);
                goto label_390;
              }
              goto label_387;
            case 'S':
              if (questionAndAnswer == "Shaft_Jump")
              {
                if (this is MineShaft mineShaft)
                {
                  mineShaft.enterMineShaft();
                  goto label_390;
                }
                goto label_390;
              }
              goto label_387;
            default:
              goto label_387;
          }
        case 11:
          switch (questionAndAnswer[0])
          {
            case 'B':
              if (questionAndAnswer == "Bouquet_Yes")
              {
                if (Game1.player.Money >= 500)
                {
                  if (Game1.player.ActiveObject == null)
                  {
                    Game1.player.Money -= 500;
                    Object @object = ItemRegistry.Create<Object>("(O)458");
                    @object.CanBeSetDown = false;
                    Game1.player.grabObject(@object);
                    return true;
                  }
                  goto label_390;
                }
                Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\UI:NotEnoughMoney1"));
                goto label_390;
              }
              goto label_387;
            case 'E':
              if (questionAndAnswer == "ExitMine_Go")
              {
                Game1.enterMine(Game1.CurrentMineLevel - 1);
                goto label_390;
              }
              goto label_387;
            case 'M':
              switch (questionAndAnswer)
              {
                case "Mine_Return":
                  Game1.enterMine(Game1.player.deepestMineLevel);
                  goto label_390;
                case "Mariner_Buy":
                  if (Game1.player.Money >= 5000)
                  {
                    Game1.player.Money -= 5000;
                    Object @object = ItemRegistry.Create<Object>("(O)460");
                    @object.CanBeSetDown = false;
                    Game1.player.grabObject(@object);
                    return true;
                  }
                  Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\UI:NotEnoughMoney1"));
                  goto label_390;
                default:
                  goto label_387;
              }
            case 'm':
              if (questionAndAnswer == "mariner_Buy")
              {
                if (Game1.player.Money >= 5000)
                {
                  Game1.player.Money -= 5000;
                  Item obj = ItemRegistry.Create("(O)460");
                  obj.specialItem = true;
                  Game1.player.addItemByMenuIfNecessary(obj);
                  if (Game1.activeClickableMenu == null)
                  {
                    Game1.player.holdUpItemThenMessage(ItemRegistry.Create("(O)460"));
                    goto label_390;
                  }
                  goto label_390;
                }
                Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\UI:NotEnoughMoney1"));
                goto label_390;
              }
              goto label_387;
            case 'u':
              if (questionAndAnswer == "upgrade_Yes")
              {
                this.houseUpgradeAccept();
                goto label_390;
              }
              goto label_387;
            default:
              goto label_387;
          }
        case 12:
          switch (questionAndAnswer[0])
          {
            case 'E':
              if (questionAndAnswer == "ExitMine_Yes")
                break;
              goto label_387;
            case 'M':
              if (questionAndAnswer == "Marnie_Adopt")
              {
                Utility.TryOpenShopMenu("PetAdoption", "Marnie");
                goto label_390;
              }
              goto label_387;
            default:
              goto label_387;
          }
          break;
        case 13:
          switch (questionAndAnswer[0])
          {
            case 'C':
              if (questionAndAnswer == "ClubCard_Yes.")
                goto label_342;
              goto label_387;
            case 'G':
              if (questionAndAnswer == "GoldClock_Yes")
              {
                Game1.netWorldState.Value.goldenClocksTurnedOff.Value = !Game1.netWorldState.Value.goldenClocksTurnedOff.Value;
                Game1.playSound("yoba");
                goto label_390;
              }
              goto label_387;
            case 'S':
              if (questionAndAnswer == "SleepTent_Yes")
              {
                Game1.player.isInBed.Value = true;
                Game1.player.sleptInTemporaryBed.Value = true;
                Game1.displayFarmer = false;
                Game1.playSound("sandyStep");
                DelayedAction.playSoundAfterDelay("sandyStep", 500);
                this.startSleep();
                goto label_390;
              }
              goto label_387;
            case 'd':
              if (questionAndAnswer == "dogStatue_Yes")
              {
                if (Game1.player.Money < 10000)
                {
                  Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:BusStop_NotEnoughMoneyForTicket"));
                  goto label_390;
                }
                List<Response> responseList = new List<Response>();
                if (GameLocation.canRespec(0))
                  responseList.Add(new Response("farming", Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11604")));
                if (GameLocation.canRespec(3))
                  responseList.Add(new Response("mining", Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11605")));
                if (GameLocation.canRespec(2))
                  responseList.Add(new Response("foraging", Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11606")));
                if (GameLocation.canRespec(1))
                  responseList.Add(new Response("fishing", Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11607")));
                if (GameLocation.canRespec(4))
                  responseList.Add(new Response("combat", Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11608")));
                responseList.Add(new Response("cancel", Game1.content.LoadString("Strings\\Locations:Sewer_DogStatueCancel")));
                this.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:Sewer_DogStatueQuestion"), responseList.ToArray(), "professionForget");
                goto label_390;
              }
              goto label_387;
            default:
              goto label_387;
          }
        case 14:
          switch (questionAndAnswer[1])
          {
            case 'a':
              if (questionAndAnswer == "carpenter_Shop")
              {
                Game1.player.forceCanMove();
                Utility.TryOpenShopMenu("Carpenter", "Robin");
                goto label_390;
              }
              goto label_387;
            case 'l':
              if (questionAndAnswer == "ClearHouse_Yes")
              {
                Vector2 tile = Game1.player.Tile;
                foreach (Vector2 adjacentTilesOffset in Character.AdjacentTilesOffsets)
                  this.objects.Remove(tile + adjacentTilesOffset);
                goto label_390;
              }
              goto label_387;
            case 'o':
              if (questionAndAnswer == "Bookseller_Buy")
              {
                Utility.TryOpenShopMenu("Bookseller", (string) null);
                goto label_390;
              }
              goto label_387;
            case 'u':
              if (questionAndAnswer == "BuyQiCoins_Yes")
              {
                if (Game1.player.Money >= 1000)
                {
                  Game1.player.Money -= 1000;
                  this.localSound("Pickup_Coin15");
                  Game1.player.clubCoins += 100;
                  goto label_390;
                }
                Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:GameLocation.cs.8715"));
                goto label_390;
              }
              goto label_387;
            case 'x':
              if (questionAndAnswer == "ExitMine_Leave")
                break;
              goto label_387;
            default:
              goto label_387;
          }
          break;
        case 15:
          switch (questionAndAnswer[4])
          {
            case 'C':
              if (questionAndAnswer == "ClubCard_That's")
                goto label_342;
              goto label_387;
            case 'S':
              if (questionAndAnswer == "ClubSeller_I'll")
              {
                if (Game1.player.Money >= 1000000)
                {
                  Game1.player.Money -= 1000000;
                  Game1.exitActiveMenu();
                  Game1.player.forceCanMove();
                  Game1.player.addItemByMenuIfNecessaryElseHoldUp(ItemRegistry.Create("(BC)127"));
                  goto label_390;
                }
                Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Club_ClubSeller_NotEnoughMoney"));
                goto label_390;
              }
              goto label_387;
            case 'T':
              if (questionAndAnswer == "ExitToTitle_Yes")
              {
                Game1.fadeScreenToBlack();
                Game1.exitToTitle = true;
                goto label_390;
              }
              goto label_387;
            case 'c':
              if (questionAndAnswer == "CalicoJack_Play")
              {
                if (Game1.player.clubCoins >= 100)
                {
                  Game1.currentMinigame = (IMinigame) new CalicoJack();
                  goto label_390;
                }
                Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Club_CalicoJack_NotEnoughCoins"));
                goto label_390;
              }
              goto label_387;
            case 'i':
              switch (questionAndAnswer)
              {
                case "Marnie_Supplies":
                  Utility.TryOpenShopMenu("AnimalShop", "Marnie");
                  goto label_390;
                case "Marnie_Purchase":
                  Game1.player.forceCanMove();
                  Game1.currentLocation.ShowAnimalShopMenu();
                  goto label_390;
                default:
                  goto label_387;
              }
            case 'k':
              if (questionAndAnswer == "Blacksmith_Shop")
              {
                Utility.TryOpenShopMenu("Blacksmith", "Clint");
                goto label_390;
              }
              goto label_387;
            case 'o':
              if (questionAndAnswer == "buyJojaCola_Yes")
              {
                if (Game1.player.Money >= 75)
                {
                  Game1.player.Money -= 75;
                  Game1.player.addItemByMenuIfNecessary(ItemRegistry.Create("(O)167"));
                  goto label_390;
                }
                Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\UI:NotEnoughMoney1"));
                goto label_390;
              }
              goto label_387;
            default:
              goto label_387;
          }
        case 16 /*0x10*/:
          switch (questionAndAnswer[1])
          {
            case 'a':
              if (questionAndAnswer == "CalicoJack_Rules")
              {
                Game1.multipleDialogues(new string[2]
                {
                  Game1.content.LoadString("Strings\\Locations:Club_CalicoJack_Rules1"),
                  Game1.content.LoadString("Strings\\Locations:Club_CalicoJack_Rules2")
                });
                goto label_390;
              }
              goto label_387;
            case 'i':
              if (questionAndAnswer == "WizardShrine_Yes")
              {
                if (Game1.player.Money >= 500)
                {
                  Game1.activeClickableMenu = (IClickableMenu) new CharacterCustomization(CharacterCustomization.Source.Wizard);
                  Game1.player.Money -= 500;
                  goto label_390;
                }
                Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\UI:NotEnoughMoney2"));
                goto label_390;
              }
              goto label_387;
            case 'n':
              if (questionAndAnswer == "EnterTheater_Yes")
              {
                Rumble.rumble(0.15f, 200f);
                Game1.player.completelyStopAnimatingOrDoingAction();
                this.playSound("doorClose", new Vector2?(Game1.player.Tile));
                Game1.warpFarmer("MovieTheater", 13, 15, 0);
                goto label_390;
              }
              goto label_387;
            case 'o':
              if (questionAndAnswer == "Bookseller_Trade")
              {
                Utility.TryOpenShopMenu("BooksellerTrade", (string) null);
                goto label_390;
              }
              goto label_387;
            case 'u':
              if (questionAndAnswer == "BuyClubCoins_Yes")
              {
                if (Game1.player.Money >= 1000)
                {
                  Game1.player.Money -= 1000;
                  Game1.player.clubCoins += 10;
                  goto label_390;
                }
                Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\UI:NotEnoughMoney1"));
                goto label_390;
              }
              goto label_387;
            default:
              goto label_387;
          }
        case 17:
          switch (questionAndAnswer[0])
          {
            case 'B':
              if (questionAndAnswer == "Backpack_Purchase")
              {
                if (Game1.player.maxItems.Value == 12 && Game1.player.Money >= 2000)
                {
                  Game1.player.Money -= 2000;
                  Game1.player.increaseBackpackSize(12);
                  Game1.player.holdUpItemThenMessage((Item) new SpecialItem(99, Game1.content.LoadString("Strings\\StringsFromCSFiles:GameLocation.cs.8708")));
                  Game1.multiplayer.globalChatInfoMessage("BackpackLarge", Game1.player.Name);
                  goto label_390;
                }
                if (Game1.player.maxItems.Value < 36 && Game1.player.Money >= 10000)
                {
                  Game1.player.Money -= 10000;
                  Game1.player.maxItems.Value += 12;
                  Game1.player.holdUpItemThenMessage((Item) new SpecialItem(99, Game1.content.LoadString("Strings\\StringsFromCSFiles:GameLocation.cs.8709")));
                  for (int index = 0; index < Game1.player.maxItems.Value; ++index)
                  {
                    if (Game1.player.Items.Count <= index)
                      Game1.player.Items.Add((Item) null);
                  }
                  Game1.multiplayer.globalChatInfoMessage("BackpackDeluxe", Game1.player.Name);
                  goto label_390;
                }
                if (Game1.player.maxItems.Value != 36)
                {
                  Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\UI:NotEnoughMoney2"));
                  goto label_390;
                }
                goto label_390;
              }
              goto label_387;
            case 'C':
              if (questionAndAnswer == "CalicoJackHS_Play")
              {
                if (Game1.player.clubCoins >= 1000)
                {
                  Game1.currentMinigame = (IMinigame) new CalicoJack(highStakes: true);
                  goto label_390;
                }
                Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Club_CalicoJackHS_NotEnoughCoins"));
                goto label_390;
              }
              goto label_387;
            case 'c':
              if (questionAndAnswer == "carpenter_Upgrade")
              {
                this.houseUpgradeOffer();
                goto label_390;
              }
              goto label_387;
            default:
              goto label_387;
          }
        case 18:
          switch (questionAndAnswer[12])
          {
            case 'e':
              if (questionAndAnswer == "CowboyGame_NewGame")
              {
                Game1.player.jotpkProgress.Value = (AbigailGame.JOTPKProgress) null;
                Game1.currentMinigame = (IMinigame) new AbigailGame();
                goto label_390;
              }
              goto label_387;
            case 'f':
              if (questionAndAnswer == "evilShrineLeft_Yes")
              {
                if (Game1.player.Items.ReduceId("(O)74", 1) > 0)
                {
                  Game1.multiplayer.broadcastSprites(this, new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(536, 1945, 8, 8), new Vector2(156f, 388f), false, 0.0f, Color.White)
                  {
                    interval = 50f,
                    totalNumberOfLoops = 99999,
                    animationLength = 7,
                    layerDepth = 0.0385000035f,
                    scale = 4f
                  });
                  for (int index = 0; index < 20; ++index)
                    Game1.multiplayer.broadcastSprites(this, new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(372, 1956, 10, 10), new Vector2(2f, 6f) * 64f + new Vector2((float) Game1.random.Next(-32, 64 /*0x40*/), (float) Game1.random.Next(16 /*0x10*/)), false, 1f / 500f, Color.LightGray)
                    {
                      alpha = 0.75f,
                      motion = new Vector2(1f, -0.5f),
                      acceleration = new Vector2(-1f / 500f, 0.0f),
                      interval = 99999f,
                      layerDepth = (float) (0.03840000182390213 + (double) Game1.random.Next(100) / 10000.0),
                      scale = 3f,
                      scaleChange = 0.01f,
                      rotationChange = (float) ((double) Game1.random.Next(-5, 6) * 3.1415927410125732 / 256.0),
                      delayBeforeAnimationStart = index * 25
                    });
                  this.playSound("fireball");
                  Game1.multiplayer.broadcastSprites(this, new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(388, 1894, 24, 22), 100f, 6, 9999, new Vector2(2f, 5f) * 64f, false, true, 1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
                  {
                    motion = new Vector2(4f, -2f)
                  });
                  if (Game1.player.getChildrenCount() > 1)
                    Game1.multiplayer.broadcastSprites(this, new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(388, 1894, 24, 22), 100f, 6, 9999, new Vector2(2f, 5f) * 64f, false, true, 1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
                    {
                      motion = new Vector2(4f, -1.5f),
                      delayBeforeAnimationStart = 50
                    });
                  string message = "";
                  foreach (NPC child in Game1.player.getChildren())
                    message += Game1.content.LoadString("Strings\\Locations:WitchHut_Goodbye", (object) child.getName());
                  Game1.showGlobalMessage(message);
                  Game1.player.getRidOfChildren();
                  Game1.multiplayer.globalChatInfoMessage("EvilShrine", Game1.player.name.Value);
                  goto label_390;
                }
                Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:WitchHut_NoOffering"));
                goto label_390;
              }
              goto label_387;
            case 'n':
              if (questionAndAnswer == "carpenter_Renovate")
              {
                Game1.player.forceCanMove();
                HouseRenovation.ShowRenovationMenu();
                goto label_390;
              }
              goto label_387;
            case 'p':
              if (questionAndAnswer == "Blacksmith_Upgrade")
              {
                if (Game1.player.daysLeftForToolUpgrade.Value > 0)
                {
                  NPC characterFromName = this.getCharacterFromName("Clint");
                  if (characterFromName != null)
                  {
                    Game1.DrawDialogue(characterFromName, "Data\\ExtraDialogue:Clint_StillWorking", (object) Game1.player.toolBeingUpgraded.Value.DisplayName);
                    goto label_390;
                  }
                  goto label_390;
                }
                Utility.TryOpenShopMenu("ClintUpgrade", "Clint");
                goto label_390;
              }
              goto label_387;
            case 'r':
              if (questionAndAnswer == "Blacksmith_Process")
              {
                Game1.activeClickableMenu = (IClickableMenu) new GeodeMenu();
                goto label_390;
              }
              goto label_387;
            default:
              goto label_387;
          }
        case 19:
          switch (questionAndAnswer[0])
          {
            case 'C':
              if (questionAndAnswer == "CowboyGame_Continue")
              {
                Game1.currentMinigame = (IMinigame) new AbigailGame();
                goto label_390;
              }
              goto label_387;
            case 'a':
              if (questionAndAnswer == "adventureGuild_Shop")
              {
                Game1.player.forceCanMove();
                Utility.TryOpenShopMenu("AdventureShop", "Marlon");
                goto label_390;
              }
              goto label_387;
            case 'c':
              if (questionAndAnswer == "carpenter_Construct")
              {
                this.ShowConstructOptions("Robin");
                goto label_390;
              }
              goto label_387;
            default:
              goto label_387;
          }
        case 20:
          switch (questionAndAnswer[0])
          {
            case 'M':
              if (questionAndAnswer == "MinecartGame_Endless")
              {
                Game1.currentMinigame = (IMinigame) new MineCart(0, 2);
                goto label_390;
              }
              goto label_387;
            case 'c':
              if (questionAndAnswer == "communityUpgrade_Yes")
              {
                this.communityUpgradeAccept();
                goto label_390;
              }
              goto label_387;
            case 'e':
              if (questionAndAnswer == "evilShrineCenter_Yes")
              {
                if (Game1.player.Money >= 30000)
                {
                  Game1.player.Money -= 30000;
                  Game1.player.wipeExMemories();
                  Game1.multiplayer.globalChatInfoMessage("EvilShrine", Game1.player.name.Value);
                  Game1.multiplayer.broadcastSprites(this, new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(536, 1945, 8, 8), new Vector2(468f, 328f), false, 0.0f, Color.White)
                  {
                    interval = 50f,
                    totalNumberOfLoops = 99999,
                    animationLength = 7,
                    layerDepth = 0.0385000035f,
                    scale = 4f
                  });
                  this.playSound("fireball");
                  DelayedAction.playSoundAfterDelay("debuffHit", 500, this);
                  int num = 0;
                  Game1.player.faceDirection(2);
                  Game1.player.FarmerSprite.animateOnce(new FarmerSprite.AnimationFrame[2]
                  {
                    new FarmerSprite.AnimationFrame(94, 1500),
                    new FarmerSprite.AnimationFrame(0, 1)
                  });
                  Game1.player.freezePause = 1500;
                  Game1.player.jitterStrength = 1f;
                  for (int index = 0; index < 20; ++index)
                    Game1.multiplayer.broadcastSprites(this, new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(372, 1956, 10, 10), new Vector2(7f, 5f) * 64f + new Vector2((float) Game1.random.Next(-32, 64 /*0x40*/), (float) Game1.random.Next(16 /*0x10*/)), false, 1f / 500f, Color.SlateGray)
                    {
                      alpha = 0.75f,
                      motion = new Vector2(0.0f, -0.5f),
                      acceleration = new Vector2(-1f / 500f, 0.0f),
                      interval = 99999f,
                      layerDepth = (float) (0.032000001519918442 + (double) Game1.random.Next(100) / 10000.0),
                      scale = 3f,
                      scaleChange = 0.01f,
                      rotationChange = (float) ((double) Game1.random.Next(-5, 6) * 3.1415927410125732 / 256.0),
                      delayBeforeAnimationStart = index * 25
                    });
                  for (int index = 0; index < 16 /*0x10*/; ++index)
                  {
                    foreach (Vector2 vector2 in Utility.getBorderOfThisRectangle(Utility.getRectangleCenteredAt(new Vector2(7f, 5f), 2 + index * 2)))
                    {
                      if (num % 2 == 0)
                      {
                        Multiplayer multiplayer = Game1.multiplayer;
                        TemporaryAnimatedSprite[] temporaryAnimatedSpriteArray = new TemporaryAnimatedSprite[1];
                        TemporaryAnimatedSprite temporaryAnimatedSprite = new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(692, 1853, 4, 4), 25f, 1, 16 /*0x10*/, vector2 * 64f + new Vector2(32f, 32f), false, false);
                        temporaryAnimatedSprite.layerDepth = 1f;
                        temporaryAnimatedSprite.delayBeforeAnimationStart = index * 50;
                        temporaryAnimatedSprite.scale = 4f;
                        temporaryAnimatedSprite.scaleChange = 1f;
                        Color toGreenLerpColor = Utility.getRedToGreenLerpColor(1f / (float) (index + 1));
                        int r = (int) byte.MaxValue - (int) toGreenLerpColor.R;
                        toGreenLerpColor = Utility.getRedToGreenLerpColor(1f / (float) (index + 1));
                        int g = (int) byte.MaxValue - (int) toGreenLerpColor.G;
                        toGreenLerpColor = Utility.getRedToGreenLerpColor(1f / (float) (index + 1));
                        int b = (int) byte.MaxValue - (int) toGreenLerpColor.B;
                        temporaryAnimatedSprite.color = new Color(r, g, b);
                        temporaryAnimatedSprite.acceleration = new Vector2(-0.1f, 0.0f);
                        temporaryAnimatedSpriteArray[0] = temporaryAnimatedSprite;
                        multiplayer.broadcastSprites(this, temporaryAnimatedSpriteArray);
                      }
                      ++num;
                    }
                  }
                  goto label_390;
                }
                Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:WitchHut_NoOffering"));
                goto label_390;
              }
              goto label_387;
            case 'p':
              if (questionAndAnswer == "pagedResponse_cancel")
              {
                this._CleanupPagedResponses();
                goto label_390;
              }
              goto label_387;
            default:
              goto label_387;
          }
        case 21:
          switch (questionAndAnswer[0])
          {
            case 'M':
              if (questionAndAnswer == "MinecartGame_Progress")
              {
                Game1.currentMinigame = (IMinigame) new MineCart(0, 3);
                goto label_390;
              }
              goto label_387;
            case 'S':
              if (questionAndAnswer == "ShrineOfChallenge_Yes")
              {
                Game1.player.team.toggleMineShrineOvernight.Value = true;
                Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:ChallengeShrine_Activated"));
                Game1.multiplayer.globalChatInfoMessage(!Game1.player.team.mineShrineActivated.Value ? "HardModeMinesActivated" : "HardModeMinesDeactivated", Game1.player.Name);
                DelayedAction.functionAfterDelay((Action) (() =>
                {
                  if (!Game1.player.team.mineShrineActivated.Value)
                  {
                    Game1.playSound("fireball");
                    this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(536, 1945, 8, 8), new Vector2(8.75f, 5.8f) * 64f + new Vector2(32f, -32f), false, 0.0f, Color.White)
                    {
                      interval = 50f,
                      totalNumberOfLoops = 99999,
                      animationLength = 4,
                      lightId = "ShrineOfChallenge_Activation_1",
                      id = 888,
                      lightRadius = 2f,
                      scale = 4f,
                      yPeriodic = true,
                      lightcolor = new Color(100, 0, 0),
                      yPeriodicLoopTime = 1000f,
                      yPeriodicRange = 4f,
                      layerDepth = 0.04544f
                    });
                    this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(536, 1945, 8, 8), new Vector2(10.75f, 5.8f) * 64f + new Vector2(32f, -32f), false, 0.0f, Color.White)
                    {
                      interval = 50f,
                      totalNumberOfLoops = 99999,
                      animationLength = 4,
                      lightId = "ShrineOfChallenge_Activation_2",
                      id = 889,
                      lightRadius = 2f,
                      scale = 4f,
                      lightcolor = new Color(100, 0, 0),
                      yPeriodic = true,
                      yPeriodicLoopTime = 1100f,
                      yPeriodicRange = 4f,
                      layerDepth = 0.04544f
                    });
                  }
                  else
                  {
                    this.removeTemporarySpritesWithID(888);
                    this.removeTemporarySpritesWithID(889);
                    Game1.playSound("fireball");
                  }
                }), 500);
                goto label_390;
              }
              goto label_387;
            default:
              goto label_387;
          }
        case 22:
          switch (questionAndAnswer[0])
          {
            case 'S':
              if (questionAndAnswer == "SquidFestBooth_Rewards")
              {
                if (!Game1.player.mailReceived.Contains($"GotSquidFestReward_{Game1.year.ToString()}_{Game1.dayOfMonth.ToString()}_3"))
                {
                  if (!Game1.player.mailReceived.Contains($"GotSquidFestReward_{Game1.year.ToString()}_{Game1.dayOfMonth.ToString()}_3"))
                  {
                    List<string> stringList = new List<string>();
                    int[] numArray1;
                    if (Game1.dayOfMonth != 12)
                      numArray1 = new int[4]{ 2, 5, 7, 10 };
                    else
                      numArray1 = new int[4]{ 1, 3, 5, 8 };
                    int[] numArray2 = numArray1;
                    int num = (int) Game1.stats.Get(StatKeys.SquidFestScore(Game1.dayOfMonth, Game1.year));
                    bool flag1 = false;
                    bool flag2 = Game1.player.mailReceived.Contains("GotCrabbingBook");
                    for (int index = 0; index < numArray2.Length; ++index)
                    {
                      if (num >= numArray2[index])
                      {
                        if (!Game1.player.mailReceived.Contains($"GotSquidFestReward_{Game1.year.ToString()}_{Game1.dayOfMonth.ToString()}_{index.ToString()}"))
                        {
                          stringList.Add($"{Game1.dayOfMonth.ToString()}_{index.ToString()}");
                          Game1.player.mailReceived.Add($"GotSquidFestReward_{Game1.year.ToString()}_{Game1.dayOfMonth.ToString()}_{index.ToString()}");
                          flag1 = false;
                          if (!flag2 && index >= 3)
                            Game1.player.mailReceived.Add("GotCrabbingBook");
                        }
                        else
                          flag1 = true;
                      }
                    }
                    if (stringList.Count > 0)
                    {
                      List<Item> inventory = new List<Item>();
                      Random daySaveRandom = Utility.CreateDaySaveRandom((double) (Game1.year * 2000), (double) (Game1.dayOfMonth * 10));
                      foreach (string str in stringList)
                      {
                        if (str != null && str.Length == 4)
                        {
                          switch (str[3])
                          {
                            case '0':
                              switch (str)
                              {
                                case "12_0":
                                  inventory.Add(ItemRegistry.Create("(O)DeluxeBait", 20));
                                  continue;
                                case "13_0":
                                  inventory.Add(ItemRegistry.Create("(O)694"));
                                  continue;
                                default:
                                  continue;
                              }
                            case '1':
                              switch (str)
                              {
                                case "12_1":
                                  inventory.Add(daySaveRandom.NextDouble() < 0.5 ? ItemRegistry.Create("(O)498", 10) : ItemRegistry.Create("(O)MysteryBox", 2));
                                  inventory.Add(ItemRegistry.Create("(O)242"));
                                  continue;
                                case "13_1":
                                  inventory.Add(daySaveRandom.NextDouble() < 0.5 ? ItemRegistry.Create("(O)498", 15) : ItemRegistry.Create("(O)MysteryBox", 3));
                                  inventory.Add(ItemRegistry.Create("(O)242"));
                                  continue;
                                default:
                                  continue;
                              }
                            case '2':
                              switch (str)
                              {
                                case "12_2":
                                  inventory.Add(ItemRegistry.Create("(O)797"));
                                  inventory.Add(ItemRegistry.Create("(O)395", 3));
                                  continue;
                                case "13_2":
                                  inventory.Add(ItemRegistry.Create("(O)166"));
                                  inventory.Add(ItemRegistry.Create("(O)253", 3));
                                  continue;
                                default:
                                  continue;
                              }
                            case '3':
                              switch (str)
                              {
                                case "12_3":
                                  inventory.Add((Item) new Furniture("SquidKid_Painting", Vector2.Zero));
                                  if (!flag2)
                                  {
                                    inventory.Add(ItemRegistry.Create("(O)Book_Crabbing"));
                                    continue;
                                  }
                                  inventory.Add(ItemRegistry.Create("(O)MysteryBox", 3));
                                  inventory.Add(ItemRegistry.Create("(O)265"));
                                  continue;
                                case "13_3":
                                  inventory.Add((Item) new Hat("SquidHat"));
                                  if (!flag2)
                                  {
                                    inventory.Add(ItemRegistry.Create("(O)Book_Crabbing"));
                                    continue;
                                  }
                                  inventory.Add(ItemRegistry.Create("(O)MysteryBox", 3));
                                  inventory.Add(ItemRegistry.Create("(O)265"));
                                  continue;
                                default:
                                  continue;
                              }
                            default:
                              continue;
                          }
                        }
                      }
                      if (inventory.Count > 0)
                      {
                        ItemGrabMenu itemGrabMenu = new ItemGrabMenu((IList<Item>) inventory).setEssential(true, true);
                        itemGrabMenu.inventory.showGrayedOutSlots = true;
                        itemGrabMenu.source = 2;
                        Game1.activeClickableMenu = (IClickableMenu) itemGrabMenu;
                        goto label_390;
                      }
                      goto label_390;
                    }
                    Game1.drawObjectDialogue(Game1.content.LoadString(flag1 ? "Strings\\1_6_Strings:SquidFest_AlreadyGotAvailableRewards" : "Strings\\1_6_Strings:SquidFestBooth_NoRewards"));
                    goto label_390;
                  }
                }
                Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\1_6_Strings:SquidFest_GotAllRewardsToday"));
                goto label_390;
              }
              goto label_387;
            case 'p':
              if (questionAndAnswer == "pagedResponse_nextPage")
              {
                this._ShowPagedResponses(GameLocation._PagedResponsePage + 1);
                goto label_390;
              }
              goto label_387;
            default:
              goto label_387;
          }
        case 23:
          switch (questionAndAnswer[19])
          {
            case 'a':
              if (questionAndAnswer == "TroutDerbyBooth_Rewards")
              {
                if (Game1.player.Items.CountId("TroutDerbyTag") > 0)
                {
                  Item obj = (Item) null;
                  int num1 = (int) ((long) Utility.CreateRandom((double) Game1.uniqueIDForThisGame).Next(10) + (long) Game1.stats.Get("GoldenTagsTurnedIn")) % 10;
                  if (Game1.stats.Get("GoldenTagsTurnedIn") == 0U)
                  {
                    obj = ItemRegistry.Create("(O)TentKit");
                  }
                  else
                  {
                    switch (num1)
                    {
                      case 0:
                        obj = ItemRegistry.Create("(H)BucketHat");
                        break;
                      case 1:
                        obj = ItemRegistry.Create("(O)710");
                        break;
                      case 2:
                        obj = ItemRegistry.Create("(O)MysteryBox", 3);
                        break;
                      case 3:
                        obj = ItemRegistry.Create("(O)72");
                        break;
                      case 4:
                        obj = ItemRegistry.Create("(F)MountedTrout_Painting");
                        break;
                      case 5:
                        obj = ItemRegistry.Create("(O)DeluxeBait", 20);
                        break;
                      case 6:
                        obj = ItemRegistry.Create("(O)253", 2);
                        break;
                      case 7:
                        obj = ItemRegistry.Create("(O)621");
                        break;
                      case 8:
                        obj = ItemRegistry.Create("(O)688", 3);
                        break;
                      case 9:
                        obj = ItemRegistry.Create("(O)749", 3);
                        break;
                    }
                  }
                  if (obj != null && (Game1.player.couldInventoryAcceptThisItem(obj) || Game1.player.Items.CountId("TroutDerbyTag") == 1))
                  {
                    int num2 = (int) Game1.stats.Increment("GoldenTagsTurnedIn");
                    Game1.player.Items.ReduceId("TroutDerbyTag", 1);
                    Game1.player.holdUpItemThenMessage(obj);
                    Game1.player.addItemToInventoryBool(obj);
                    goto label_390;
                  }
                  Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\1_6_Strings:FishingDerbyBooth_BagFull"));
                  goto label_390;
                }
                Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\1_6_Strings:FishingDerbyBooth_NoTags"));
                goto label_390;
              }
              goto label_387;
            case 'm':
              if (questionAndAnswer == "professionForget_combat")
              {
                if (Game1.player.newLevels.Contains(new Point(4, 5)) || Game1.player.newLevels.Contains(new Point(4, 10)))
                {
                  Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Sewer_DogStatueAlready"));
                  goto label_390;
                }
                Game1.player.Money = Math.Max(0, Game1.player.Money - 10000);
                GameLocation.RemoveProfession(26);
                GameLocation.RemoveProfession(27);
                GameLocation.RemoveProfession(29);
                GameLocation.RemoveProfession(25);
                GameLocation.RemoveProfession(28);
                GameLocation.RemoveProfession(24);
                Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Sewer_DogStatueFinished"));
                int num = Farmer.checkForLevelGain(0, Game1.player.experiencePoints[4]);
                if (num >= 5)
                  Game1.player.newLevels.Add(new Point(4, 5));
                if (num >= 10)
                  Game1.player.newLevels.Add(new Point(4, 10));
                DelayedAction.playSoundAfterDelay("dog_bark", 300);
                DelayedAction.playSoundAfterDelay("dog_bark", 900);
                goto label_390;
              }
              goto label_387;
            case 'n':
              if (questionAndAnswer == "professionForget_mining")
              {
                if (Game1.player.newLevels.Contains(new Point(3, 5)) || Game1.player.newLevels.Contains(new Point(3, 10)))
                {
                  Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Sewer_DogStatueAlready"));
                  goto label_390;
                }
                Game1.player.Money = Math.Max(0, Game1.player.Money - 10000);
                GameLocation.RemoveProfession(23);
                GameLocation.RemoveProfession(21);
                GameLocation.RemoveProfession(18);
                GameLocation.RemoveProfession(19);
                GameLocation.RemoveProfession(22);
                GameLocation.RemoveProfession(20);
                Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Sewer_DogStatueFinished"));
                int num = Farmer.checkForLevelGain(0, Game1.player.experiencePoints[3]);
                if (num >= 5)
                  Game1.player.newLevels.Add(new Point(3, 5));
                if (num >= 10)
                  Game1.player.newLevels.Add(new Point(3, 10));
                DelayedAction.playSoundAfterDelay("dog_bark", 300);
                DelayedAction.playSoundAfterDelay("dog_bark", 900);
                goto label_390;
              }
              goto label_387;
            case 'v':
              if (questionAndAnswer == "adventureGuild_Recovery")
              {
                Game1.player.forceCanMove();
                Utility.TryOpenShopMenu("AdventureGuildRecovery", "Marlon");
                goto label_390;
              }
              goto label_387;
            default:
              goto label_387;
          }
        case 24:
          switch (questionAndAnswer[18])
          {
            case 'a':
              if (questionAndAnswer == "professionForget_farming")
              {
                if (Game1.player.newLevels.Contains(new Point(0, 5)) || Game1.player.newLevels.Contains(new Point(0, 10)))
                {
                  Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Sewer_DogStatueAlready"));
                  goto label_390;
                }
                Game1.player.Money = Math.Max(0, Game1.player.Money - 10000);
                GameLocation.RemoveProfession(0);
                GameLocation.RemoveProfession(1);
                GameLocation.RemoveProfession(3);
                GameLocation.RemoveProfession(5);
                GameLocation.RemoveProfession(2);
                GameLocation.RemoveProfession(4);
                Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Sewer_DogStatueFinished"));
                int num = Farmer.checkForLevelGain(0, Game1.player.experiencePoints[0]);
                if (num >= 5)
                  Game1.player.newLevels.Add(new Point(0, 5));
                if (num >= 10)
                  Game1.player.newLevels.Add(new Point(0, 10));
                DelayedAction.playSoundAfterDelay("dog_bark", 300);
                DelayedAction.playSoundAfterDelay("dog_bark", 900);
                goto label_390;
              }
              goto label_387;
            case 'i':
              if (questionAndAnswer == "professionForget_fishing")
              {
                if (Game1.player.newLevels.Contains(new Point(1, 5)) || Game1.player.newLevels.Contains(new Point(1, 10)))
                {
                  Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Sewer_DogStatueAlready"));
                  goto label_390;
                }
                Game1.player.Money = Math.Max(0, Game1.player.Money - 10000);
                GameLocation.RemoveProfession(8);
                GameLocation.RemoveProfession(11);
                GameLocation.RemoveProfession(10);
                GameLocation.RemoveProfession(6);
                GameLocation.RemoveProfession(9);
                GameLocation.RemoveProfession(7);
                Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Sewer_DogStatueFinished"));
                int num = Farmer.checkForLevelGain(0, Game1.player.experiencePoints[1]);
                if (num >= 5)
                  Game1.player.newLevels.Add(new Point(1, 5));
                if (num >= 10)
                  Game1.player.newLevels.Add(new Point(1, 10));
                DelayedAction.playSoundAfterDelay("dog_bark", 300);
                DelayedAction.playSoundAfterDelay("dog_bark", 900);
                goto label_390;
              }
              goto label_387;
            case 'o':
              if (questionAndAnswer == "specialCharmQuestion_Yes")
              {
                if (Game1.player.Items.ContainsId("(O)446"))
                {
                  Game1.player.holdUpItemThenMessage((Item) new SpecialItem(3));
                  Game1.player.removeFirstOfThisItemFromInventory("446");
                  Game1.player.hasSpecialCharm = true;
                  Game1.player.mailReceived.Add("SecretNote20_done");
                  goto label_390;
                }
                Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Town_specialCharmNoFoot"));
                goto label_390;
              }
              goto label_387;
            default:
              goto label_387;
          }
        case 25:
          if (questionAndAnswer == "professionForget_foraging")
          {
            if (Game1.player.newLevels.Contains(new Point(2, 5)) || Game1.player.newLevels.Contains(new Point(2, 10)))
            {
              Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Sewer_DogStatueAlready"));
              goto label_390;
            }
            Game1.player.Money = Math.Max(0, Game1.player.Money - 10000);
            GameLocation.RemoveProfession(16 /*0x10*/);
            GameLocation.RemoveProfession(14);
            GameLocation.RemoveProfession(17);
            GameLocation.RemoveProfession(12);
            GameLocation.RemoveProfession(13);
            GameLocation.RemoveProfession(15);
            Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Sewer_DogStatueFinished"));
            int num = Farmer.checkForLevelGain(0, Game1.player.experiencePoints[2]);
            if (num >= 5)
              Game1.player.newLevels.Add(new Point(2, 5));
            if (num >= 10)
              Game1.player.newLevels.Add(new Point(2, 10));
            DelayedAction.playSoundAfterDelay("dog_bark", 300);
            DelayedAction.playSoundAfterDelay("dog_bark", 900);
            goto label_390;
          }
          goto label_387;
        case 26:
          switch (questionAndAnswer[5])
          {
            case 'F':
              if (questionAndAnswer == "SquidFestBooth_Explanation")
              {
                Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\1_6_Strings:SquidFestBooth_Explanation"));
                goto label_390;
              }
              goto label_387;
            case 'R':
              if (questionAndAnswer == "pagedResponse_previousPage")
              {
                this._ShowPagedResponses(GameLocation._PagedResponsePage - 1);
                goto label_390;
              }
              goto label_387;
            case 'e':
              if (questionAndAnswer == "ShrineOfSkullChallenge_Yes")
              {
                Game1.player.team.toggleSkullShrineOvernight.Value = true;
                Game1.showGlobalMessage(Game1.content.LoadString("Strings\\Locations:ChallengeShrine_Activated"));
                Game1.multiplayer.globalChatInfoMessage(Game1.player.team.skullShrineActivated.Value ? "HardModeSkullCaveDeactivated" : "HardModeSkullCaveActivated", Game1.player.Name);
                this.playSound(Game1.player.team.skullShrineActivated.Value ? "skeletonStep" : "serpentDie");
                goto label_390;
              }
              goto label_387;
            case 'n':
              if (questionAndAnswer == "carpenter_CommunityUpgrade")
              {
                this.communityUpgradeOffer();
                goto label_390;
              }
              goto label_387;
            default:
              goto label_387;
          }
        case 27:
          switch (questionAndAnswer[0])
          {
            case 'E':
              if (questionAndAnswer == "EnterTheaterSpendTicket_Yes")
              {
                Game1.player.Items.ReduceId("(O)809", 1);
                Rumble.rumble(0.15f, 200f);
                Game1.player.completelyStopAnimatingOrDoingAction();
                this.playSound("doorClose", new Vector2?(Game1.player.Tile));
                Game1.warpFarmer("MovieTheater", 13, 15, 0);
                goto label_390;
              }
              goto label_387;
            case 'T':
              if (questionAndAnswer == "TroutDerbyBooth_Explanation")
              {
                Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\1_6_Strings:FishingDerbyBooth_Explanation"));
                goto label_390;
              }
              goto label_387;
            case 'e':
              if (questionAndAnswer == "evilShrineRightActivate_Yes")
              {
                if (Game1.player.Items.ReduceId("(O)203", 1) > 0)
                {
                  Game1.multiplayer.broadcastSprites(this, new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(536, 1945, 8, 8), new Vector2(780f, 388f), false, 0.0f, Color.White)
                  {
                    interval = 50f,
                    totalNumberOfLoops = 99999,
                    animationLength = 7,
                    layerDepth = 0.0385000035f,
                    scale = 4f
                  });
                  this.playSound("fireball");
                  DelayedAction.playSoundAfterDelay("batScreech", 500, this);
                  for (int index = 0; index < 20; ++index)
                    Game1.multiplayer.broadcastSprites(this, new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(372, 1956, 10, 10), new Vector2(12f, 6f) * 64f + new Vector2((float) Game1.random.Next(-32, 64 /*0x40*/), (float) Game1.random.Next(16 /*0x10*/)), false, 1f / 500f, Color.DarkSlateBlue)
                    {
                      alpha = 0.75f,
                      motion = new Vector2(-0.1f, -0.5f),
                      acceleration = new Vector2(-1f / 500f, 0.0f),
                      interval = 99999f,
                      layerDepth = (float) (0.03840000182390213 + (double) Game1.random.Next(100) / 10000.0),
                      scale = 3f,
                      scaleChange = 0.01f,
                      rotationChange = (float) ((double) Game1.random.Next(-5, 6) * 3.1415927410125732 / 256.0),
                      delayBeforeAnimationStart = index * 60
                    });
                  Game1.player.freezePause = 1501;
                  for (int index = 0; index < 28; ++index)
                    Game1.multiplayer.broadcastSprites(this, new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(540, 347, 13, 13), 50f, 4, 9999, new Vector2(12f, 5f) * 64f, false, true, 1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
                    {
                      delayBeforeAnimationStart = 500 + index * 25,
                      motion = new Vector2((float) (Game1.random.Next(1, 5) * Game1.random.Choose<int>(-1, 1)), (float) (Game1.random.Next(1, 5) * Game1.random.Choose<int>(-1, 1)))
                    });
                  Game1.spawnMonstersAtNight = true;
                  Game1.multiplayer.globalChatInfoMessage("MonstersActivated", Game1.player.name.Value);
                  goto label_390;
                }
                Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:WitchHut_NoOffering"));
                goto label_390;
              }
              goto label_387;
            default:
              goto label_387;
          }
        case 29:
          switch (questionAndAnswer[20])
          {
            case 'H':
              if (questionAndAnswer == "telephone_Carpenter_HouseCost")
              {
                NPC characterFromName = Game1.getCharacterFromName("Robin");
                string path = "Strings\\Locations:ScienceHouse_Carpenter_UpgradeHouse" + (Game1.player.houseUpgradeLevel.Value + 1).ToString();
                string str = Game1.content.LoadString(path, (object) "65,000", (object) "100");
                if (str.Contains('.'))
                  str = str.Substring(0, str.LastIndexOf('.') + 1);
                else if (str.Contains('。'))
                  str = str.Substring(0, str.LastIndexOf('。') + 1);
                string translationKey = path;
                string dialogueText = str;
                Game1.DrawDialogue(new Dialogue(characterFromName, translationKey, dialogueText)
                {
                  overridePortrait = Game1.temporaryContent.Load<Texture2D>("Portraits\\AnsweringMachine")
                });
                Game1.afterDialogues += (Game1.afterFadeFunction) (() => this.answerDialogueAction("HangUp", LegacyShims.EmptyArray<string>()));
                goto label_390;
              }
              goto label_387;
            case 'S':
              if (questionAndAnswer == "telephone_Carpenter_ShopStock")
              {
                Utility.TryOpenShopMenu("Carpenter", (string) null);
                if (Game1.activeClickableMenu is ShopMenu activeClickableMenu)
                {
                  activeClickableMenu.readOnly = true;
                  ShopMenu shopMenu = activeClickableMenu;
                  shopMenu.behaviorBeforeCleanup = shopMenu.behaviorBeforeCleanup + (Action<IClickableMenu>) (closed_menu => this.answerDialogueAction("HangUp", LegacyShims.EmptyArray<string>()));
                  goto label_390;
                }
                goto label_390;
              }
              goto label_387;
            case 'i':
              if (questionAndAnswer == "evilShrineRightDeActivate_Yes")
              {
                if (Game1.player.Items.ReduceId("(O)203", 1) > 0)
                {
                  Game1.multiplayer.broadcastSprites(this, new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(536, 1945, 8, 8), new Vector2(780f, 388f), false, 0.0f, Color.White)
                  {
                    interval = 50f,
                    totalNumberOfLoops = 99999,
                    animationLength = 7,
                    layerDepth = 0.0385000035f,
                    scale = 4f
                  });
                  this.playSound("fireball");
                  for (int index = 0; index < 20; ++index)
                    Game1.multiplayer.broadcastSprites(this, new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(372, 1956, 10, 10), new Vector2(12f, 6f) * 64f + new Vector2((float) Game1.random.Next(-32, 64 /*0x40*/), (float) Game1.random.Next(16 /*0x10*/)), false, 1f / 500f, Color.DarkSlateBlue)
                    {
                      alpha = 0.75f,
                      motion = new Vector2(0.0f, -0.5f),
                      acceleration = new Vector2(-1f / 500f, 0.0f),
                      interval = 99999f,
                      layerDepth = (float) (0.03840000182390213 + (double) Game1.random.Next(100) / 10000.0),
                      scale = 3f,
                      scaleChange = 0.01f,
                      rotationChange = (float) ((double) Game1.random.Next(-5, 6) * 3.1415927410125732 / 256.0),
                      delayBeforeAnimationStart = index * 25
                    });
                  Game1.spawnMonstersAtNight = false;
                  Game1.multiplayer.globalChatInfoMessage("MonstersDeActivated", Game1.player.name.Value);
                  goto label_390;
                }
                Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:WitchHut_NoOffering"));
                goto label_390;
              }
              goto label_387;
            default:
              goto label_387;
          }
        case 32 /*0x20*/:
          switch (questionAndAnswer[10])
          {
            case 'B':
              if (questionAndAnswer == "telephone_Blacksmith_UpgradeCost")
              {
                this.answerDialogueAction("Blacksmith_Upgrade", LegacyShims.EmptyArray<string>());
                if (Game1.activeClickableMenu is ShopMenu activeClickableMenu)
                {
                  activeClickableMenu.readOnly = true;
                  ShopMenu shopMenu = activeClickableMenu;
                  shopMenu.behaviorBeforeCleanup = shopMenu.behaviorBeforeCleanup + (Action<IClickableMenu>) (closed_menu => this.answerDialogueAction("HangUp", LegacyShims.EmptyArray<string>()));
                  goto label_390;
                }
                goto label_390;
              }
              goto label_387;
            case 'C':
              if (questionAndAnswer == "telephone_Carpenter_BuildingCost")
              {
                GameLocation targetLocation = (GameLocation) Game1.getFarm();
                if (Game1.currentLocation.IsBuildableLocation())
                  targetLocation = Game1.currentLocation;
                Game1.activeClickableMenu = (IClickableMenu) new CarpenterMenu("Robin", targetLocation);
                if (Game1.activeClickableMenu is CarpenterMenu activeClickableMenu)
                {
                  activeClickableMenu.readOnly = true;
                  CarpenterMenu carpenterMenu = activeClickableMenu;
                  carpenterMenu.behaviorBeforeCleanup = carpenterMenu.behaviorBeforeCleanup + (Action<IClickableMenu>) (closed_menu => this.answerDialogueAction("HangUp", LegacyShims.EmptyArray<string>()));
                  goto label_390;
                }
                goto label_390;
              }
              goto label_387;
            default:
              goto label_387;
          }
        case 33:
          if (questionAndAnswer == "telephone_SeedShop_CheckSeedStock")
          {
            if (Game1.getLocationFromName("SeedShop") is SeedShop)
            {
              if (Utility.TryOpenShopMenu("SeedShop", (string) null) && Game1.activeClickableMenu is ShopMenu activeClickableMenu)
              {
                activeClickableMenu.readOnly = true;
                ShopMenu shopMenu = activeClickableMenu;
                shopMenu.behaviorBeforeCleanup = shopMenu.behaviorBeforeCleanup + (Action<IClickableMenu>) (closed_menu => this.answerDialogueAction("HangUp", LegacyShims.EmptyArray<string>()));
                goto label_390;
              }
              goto label_390;
            }
            this.answerDialogueAction("HangUp", LegacyShims.EmptyArray<string>());
            goto label_390;
          }
          goto label_387;
        case 38:
          if (questionAndAnswer == "telephone_AnimalShop_CheckAnimalPrices")
          {
            Game1.currentLocation.ShowAnimalShopMenu((Action<PurchaseAnimalsMenu>) (menu =>
            {
              menu.readOnly = true;
              PurchaseAnimalsMenu purchaseAnimalsMenu = menu;
              purchaseAnimalsMenu.behaviorBeforeCleanup = purchaseAnimalsMenu.behaviorBeforeCleanup + (Action<IClickableMenu>) (closed_menu => this.answerDialogueAction("HangUp", LegacyShims.EmptyArray<string>()));
            }));
            goto label_390;
          }
          goto label_387;
        default:
          goto label_387;
      }
      if (Game1.CurrentMineLevel == 77377)
      {
        Game1.warpFarmer("Mine", 67, 10, true);
        goto label_390;
      }
      if (Game1.CurrentMineLevel > 120)
      {
        Game1.warpFarmer("SkullCave", 3, 4, 2);
        goto label_390;
      }
      Game1.warpFarmer("Mine", 23, 8, false);
      goto label_390;
label_342:
      Game1.addMail("bouncerGone", true, true);
      this.playSound("explosion");
      Game1.flashAlpha = 5f;
      this.characters.Remove(this.getCharacterFromName("Bouncer"));
      NPC characterFromName1 = this.getCharacterFromName("Sandy");
      if (characterFromName1 != null)
      {
        characterFromName1.faceDirection(1);
        characterFromName1.setNewDialogue("Data\\ExtraDialogue:Sandy_PlayerClubMember");
        characterFromName1.doEmote(16 /*0x10*/);
      }
      Game1.pauseThenMessage(500, Game1.content.LoadString("Strings\\Locations:Club_Bouncer_PlayerClubMember"));
      Game1.player.Halt();
      NPC characterFromName2 = Game1.getCharacterFromName("Mister Qi");
      if (characterFromName2 != null)
      {
        characterFromName2.setNewDialogue("Data\\ExtraDialogue:MisterQi_PlayerClubMember");
        goto label_390;
      }
      goto label_390;
    }
label_387:
    if (questionAndAnswer.StartsWith("pagedResponse"))
    {
      string str = questionAndAnswer.Substring("pagedResponse".Length + 1);
      Action<string> onPagedResponse = GameLocation._OnPagedResponse;
      this._CleanupPagedResponses();
      if (onPagedResponse != null)
        onPagedResponse(str);
    }
label_390:
    return true;
  }

  public void playShopPhoneNumberSounds(string whichShop)
  {
    Random random = Utility.CreateRandom((double) whichShop.GetHashCode());
    DelayedAction.playSoundAfterDelay("telephone_dialtone", 495, pitch: 1200);
    DelayedAction.playSoundAfterDelay("telephone_buttonPush", 1200, pitch: 1200 + random.Next(-4, 5) * 100);
    DelayedAction.playSoundAfterDelay("telephone_buttonPush", 1370, pitch: 1200 + random.Next(-4, 5) * 100);
    DelayedAction.playSoundAfterDelay("telephone_buttonPush", 1600, pitch: 1200 + random.Next(-4, 5) * 100);
    DelayedAction.playSoundAfterDelay("telephone_buttonPush", 1850, pitch: 1200 + random.Next(-4, 5) * 100);
    DelayedAction.playSoundAfterDelay("telephone_buttonPush", 2030, pitch: 1200 + random.Next(-4, 5) * 100);
    DelayedAction.playSoundAfterDelay("telephone_buttonPush", 2250, pitch: 1200 + random.Next(-4, 5) * 100);
    DelayedAction.playSoundAfterDelay("telephone_buttonPush", 2410, pitch: 1200 + random.Next(-4, 5) * 100);
    DelayedAction.playSoundAfterDelay("telephone_ringingInEar", 3150);
  }

  public virtual bool answerDialogue(Response answer)
  {
    string[] questionParams = this.lastQuestionKey != null ? ArgUtility.SplitBySpace(this.lastQuestionKey) : (string[]) null;
    string questionAndAnswer = questionParams != null ? $"{questionParams[0]}_{answer.responseKey}" : (string) null;
    if (answer.responseKey.Equals("Move"))
    {
      Game1.player.grabObject(this.actionObjectForQuestionDialogue);
      this.removeObject(this.actionObjectForQuestionDialogue.TileLocation, false);
      this.actionObjectForQuestionDialogue = (Object) null;
      return true;
    }
    if (this.afterQuestion != null)
    {
      this.afterQuestion(Game1.player, answer.responseKey);
      this.afterQuestion = (GameLocation.afterQuestionBehavior) null;
      Game1.objectDialoguePortraitPerson = (NPC) null;
      return true;
    }
    return questionAndAnswer != null && this.answerDialogueAction(questionAndAnswer, questionParams);
  }

  public static bool AreStoresClosedForFestival()
  {
    return Utility.isFestivalDay() && Utility.getStartTimeOfFestival() < 1900;
  }

  public static void RemoveProfession(int profession)
  {
    if (!Game1.player.professions.Remove(profession))
      return;
    LevelUpMenu.removeImmediateProfessionPerk(profession);
  }

  public static bool canRespec(int skill_index)
  {
    return Game1.player.GetUnmodifiedSkillLevel(skill_index) >= 5 && !Game1.player.newLevels.Contains(new Point(skill_index, 5)) && !Game1.player.newLevels.Contains(new Point(skill_index, 10));
  }

  public void setObject(Vector2 v, Object o) => this.objects[v] = o;

  private void houseUpgradeOffer()
  {
    switch (Game1.player.houseUpgradeLevel.Value)
    {
      case 0:
        this.createQuestionDialogue(Game1.parseText(Game1.content.LoadString("Strings\\Locations:ScienceHouse_Carpenter_UpgradeHouse1")), this.createYesNoResponses(), "upgrade");
        break;
      case 1:
        this.createQuestionDialogue(Game1.parseText(Game1.content.LoadString("Strings\\Locations:ScienceHouse_Carpenter_UpgradeHouse2", (object) "65,000", (object) "100")), this.createYesNoResponses(), "upgrade");
        break;
      case 2:
        this.createQuestionDialogue(Game1.parseText(Game1.content.LoadString("Strings\\Locations:ScienceHouse_Carpenter_UpgradeHouse3")), this.createYesNoResponses(), "upgrade");
        break;
    }
  }

  private void communityUpgradeOffer()
  {
    if (!Game1.MasterPlayer.mailReceived.Contains("pamHouseUpgrade"))
    {
      this.createQuestionDialogue(Game1.parseText(Game1.content.LoadString("Strings\\Locations:ScienceHouse_Carpenter_CommunityUpgrade1")), this.createYesNoResponses(), "communityUpgrade");
      Game1.player.team.RequestSetMail(PlayerActionTarget.Host, "pamHouseUpgradeAsked", MailType.Received, true);
    }
    else
    {
      if (Game1.MasterPlayer.mailReceived.Contains("communityUpgradeShortcuts"))
        return;
      this.createQuestionDialogue(Game1.parseText(Game1.content.LoadString("Strings\\Locations:ScienceHouse_Carpenter_CommunityUpgrade2")), this.createYesNoResponses(), "communityUpgrade");
    }
  }

  /// <summary>Whether crab pots on a given tile can only catch ocean fish, regardless of the location's crab pot fish areas.</summary>
  /// <param name="x">The X tile position to check.</param>
  /// <param name="y">The Y tile position to check.</param>
  /// <returns>Returns true to only catch ocean fish, or false to apply the normal crab pot behavior based on <c>Data/Locations</c> or <see cref="F:StardewValley.GameLocation.DefaultCrabPotFishTypes" />.</returns>
  public virtual bool catchOceanCrabPotFishFromThisSpot(int x, int y) => false;

  private void communityUpgradeAccept()
  {
    if (!Game1.MasterPlayer.mailReceived.Contains("pamHouseUpgrade"))
    {
      if (Game1.player.Money >= 500000 && Game1.player.Items.ContainsId("(O)388", 950))
      {
        Game1.player.Money -= 500000;
        Game1.player.Items.ReduceId("(O)388", 950);
        Game1.RequireCharacter("Robin").setNewDialogue("Data\\ExtraDialogue:Robin_PamUpgrade_Accepted");
        Game1.drawDialogue(Game1.getCharacterFromName("Robin"));
        Game1.RequireLocation<Town>("Town").daysUntilCommunityUpgrade.Value = 3;
        Game1.multiplayer.globalChatInfoMessage("CommunityUpgrade", Game1.player.Name);
      }
      else if (Game1.player.Money < 500000)
        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\UI:NotEnoughMoney3"));
      else
        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:ScienceHouse_Carpenter_NotEnoughWood", (object) 950));
    }
    else
    {
      if (Game1.MasterPlayer.mailReceived.Contains("communityUpgradeShortcuts"))
        return;
      if (Game1.player.Money >= 300000)
      {
        Game1.player.Money -= 300000;
        Game1.RequireCharacter("Robin").setNewDialogue("Data\\ExtraDialogue:Robin_HouseUpgrade_Accepted", true);
        Game1.drawDialogue(Game1.getCharacterFromName("Robin"));
        Game1.RequireLocation<Town>("Town").daysUntilCommunityUpgrade.Value = 3;
        Game1.multiplayer.globalChatInfoMessage("CommunityUpgrade", Game1.player.Name);
      }
      else
      {
        if (Game1.player.Money >= 300000)
          return;
        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\UI:NotEnoughMoney3"));
      }
    }
  }

  private void houseUpgradeAccept()
  {
    switch (Game1.player.houseUpgradeLevel.Value)
    {
      case 0:
        if (Game1.player.Money >= 10000 && Game1.player.Items.ContainsId("(O)388", 450))
        {
          Game1.player.daysUntilHouseUpgrade.Value = 3;
          Game1.player.Money -= 10000;
          Game1.player.Items.ReduceId("(O)388", 450);
          Game1.RequireCharacter("Robin").setNewDialogue("Data\\ExtraDialogue:Robin_HouseUpgrade_Accepted", true);
          Game1.drawDialogue(Game1.getCharacterFromName("Robin"));
          Game1.multiplayer.globalChatInfoMessage("HouseUpgrade", Game1.player.Name, Lexicon.getTokenizedPossessivePronoun(Game1.player.IsMale));
          break;
        }
        if (Game1.player.Money < 10000)
        {
          Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\UI:NotEnoughMoney3"));
          break;
        }
        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:ScienceHouse_Carpenter_NotEnoughWood", (object) 450));
        break;
      case 1:
        if (Game1.player.Money >= 65000 && Game1.player.Items.ContainsId("(O)709", 100))
        {
          Game1.player.daysUntilHouseUpgrade.Value = 3;
          Game1.player.Money -= 65000;
          Game1.player.Items.ReduceId("(O)709", 100);
          Game1.RequireCharacter("Robin").setNewDialogue("Data\\ExtraDialogue:Robin_HouseUpgrade_Accepted", true);
          Game1.drawDialogue(Game1.getCharacterFromName("Robin"));
          Game1.multiplayer.globalChatInfoMessage("HouseUpgrade", Game1.player.Name, Lexicon.getTokenizedPossessivePronoun(Game1.player.IsMale));
          break;
        }
        if (Game1.player.Money < 65000)
        {
          Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\UI:NotEnoughMoney3"));
          break;
        }
        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:ScienceHouse_Carpenter_NotEnoughHardwood", (object) 100));
        break;
      case 2:
        if (Game1.player.Money >= 100000)
        {
          Game1.player.daysUntilHouseUpgrade.Value = 3;
          Game1.player.Money -= 100000;
          Game1.RequireCharacter("Robin").setNewDialogue("Data\\ExtraDialogue:Robin_HouseUpgrade_Accepted", true);
          Game1.drawDialogue(Game1.getCharacterFromName("Robin"));
          Game1.multiplayer.globalChatInfoMessage("HouseUpgrade", Game1.player.Name, Lexicon.getTokenizedPossessivePronoun(Game1.player.IsMale));
          break;
        }
        if (Game1.player.Money >= 100000)
          break;
        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\UI:NotEnoughMoney3"));
        break;
    }
  }

  public void destroyObject(Vector2 tileLocation, Farmer who)
  {
    this.destroyObject(tileLocation, false, who);
  }

  public void destroyObject(Vector2 tileLocation, bool hardDestroy, Farmer who)
  {
    Object @object;
    if (!this.objects.TryGetValue(tileLocation, out @object) || @object.fragility.Value == 2 || @object is Chest || !(@object.QualifiedItemId != "(BC)165"))
      return;
    bool flag = false;
    if (@object.Type == "Fish" || @object.Type == "Cooking" || @object.Type == "Crafting")
    {
      if (!(@object is BreakableContainer))
      {
        TemporaryAnimatedSprite temporaryAnimatedSprite = new TemporaryAnimatedSprite(0, 150f, 1, 3, new Vector2(tileLocation.X * 64f, tileLocation.Y * 64f), true, @object.bigCraftable.Value, @object.flipped.Value);
        temporaryAnimatedSprite.CopyAppearanceFromItemId(@object.QualifiedItemId, @object.showNextIndex.Value ? 1 : 0);
        temporaryAnimatedSprite.scale = 4f;
        Game1.multiplayer.broadcastSprites(this, temporaryAnimatedSprite);
      }
      flag = true;
    }
    else if (@object.CanBeGrabbed | hardDestroy)
      flag = true;
    if (@object.IsBreakableStone())
    {
      flag = true;
      this.OnStoneDestroyed(@object.ItemId, (int) tileLocation.X, (int) tileLocation.Y, who);
    }
    if (!flag)
      return;
    this.objects.Remove(tileLocation);
  }

  public void addOneTimeGiftBox(Item i, int x, int y, int whichGiftBox = 2)
  {
    string str = $"{this.Name}_giftbox_{x.ToString()}_{y.ToString()}";
    if (Game1.player.mailReceived.Contains(str))
      return;
    Vector2 vector2 = new Vector2((float) x, (float) y);
    if (!(this.overlayObjects.GetValueOrDefault(vector2) is Chest valueOrDefault) || !(valueOrDefault.mailToAddOnItemDump == str))
      this.cleanUpTileForMapOverride(new Point(x, y));
    if (this.overlayObjects.ContainsKey(vector2))
      return;
    Chest chest = new Chest(new List<Item>() { i }, vector2, true, whichGiftBox)
    {
      mailToAddOnItemDump = str
    };
    this.overlayObjects.Add(vector2, (Object) chest);
  }

  /// <summary>Get the unique ID of the location context in <c>Data/LocationContexts</c> which includes this location.</summary>
  public virtual string GetLocationContextId()
  {
    if (this.locationContextId == null)
    {
      if (this.map == null)
        this.reloadMap();
      string key;
      if (this.map != null && this.map.Properties.TryGetValue("LocationContext", out key))
      {
        if (Game1.locationContextData.ContainsKey(key))
          this.locationContextId = key;
        else
          Game1.log.Error($"Location {this.NameOrUniqueName} has invalid LocationContext map property '{key}', ignoring value.");
      }
      if (this.locationContextId == null)
        this.locationContextId = this.GetParentLocation()?.GetLocationContextId() ?? "Default";
    }
    return this.locationContextId;
  }

  /// <summary>Get the data for the location context in <c>Data/LocationContexts</c> which includes this location.</summary>
  public virtual LocationContextData GetLocationContext()
  {
    return StardewValley.LocationContexts.Require(this.GetLocationContextId());
  }

  /// <summary>Get whether this location is in the desert context.</summary>
  public bool InDesertContext() => this.GetLocationContextId() == "Desert";

  /// <summary>Get whether this location is in the Ginger Island context.</summary>
  public bool InIslandContext() => this.GetLocationContextId() == "Island";

  /// <summary>Get whether this location is in the default valley context.</summary>
  public bool InValleyContext() => this.GetLocationContextId() == "Default";

  public virtual bool sinkDebris(Debris debris, Vector2 chunkTile, Vector2 chunkPosition)
  {
    if (debris.isEssentialItem() || debris.item != null && debris.item.HasContextTag("book_item") || debris.debrisType.Value == Debris.DebrisType.OBJECT && debris.chunkType.Value == 74)
      return false;
    if (debris.floppingFish.Value)
    {
      foreach (Building building in this.buildings)
      {
        if (building.isTileFishable(chunkTile))
          return false;
      }
    }
    if (!debris.isSinking.Value)
    {
      debris.isSinking.Value = true;
      switch (debris.debrisType.Value)
      {
        case Debris.DebrisType.OBJECT:
        case Debris.DebrisType.RESOURCE:
          if (Game1.random.NextBool())
            this.TemporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(0, 0, 64 /*0x40*/, 64 /*0x40*/), 150f, 8, 0, chunkPosition + new Vector2(-8f), false, Game1.random.NextBool(), 1f / 1000f, 0.02f, Color.White, 1f, 3f / 1000f, 0.0f, 0.0f)
            {
              delayBeforeAnimationStart = Game1.random.Next(300),
              startSound = "quickSlosh"
            });
          return false;
      }
    }
    else
    {
      bool flag = false;
      foreach (Chunk chunk in debris.Chunks)
      {
        if (chunk.sinkTimer.Value <= 0)
        {
          flag = true;
          break;
        }
      }
      if (!flag)
        return false;
    }
    if (debris.debrisType.Value == Debris.DebrisType.CHUNKS)
    {
      this.localSound("quickSlosh");
      this.TemporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(0, 0, 64 /*0x40*/, 64 /*0x40*/), 150f, 8, 0, chunkPosition + new Vector2(-8f), false, Game1.random.NextBool(), 1f / 1000f, 0.02f, Color.White, 1f, 3f / 1000f, 0.0f, 0.0f));
      return true;
    }
    this.TemporarySprites.Add(new TemporaryAnimatedSprite(28, 300f, 2, 1, chunkPosition + new Vector2(-8f), false, false));
    this.localSound("dropItemInWater");
    return true;
  }

  public virtual bool doesTileSinkDebris(int xTile, int yTile, Debris.DebrisType type)
  {
    if (this.isTileBuildingFishable(xTile, yTile))
      return true;
    return type == Debris.DebrisType.CHUNKS ? this.isWaterTile(xTile, yTile) && !this.hasTileAt(xTile, yTile, "Buildings") : this.isWaterTile(xTile, yTile) && !this.isTileUpperWaterBorder(this.getTileIndexAt(xTile, yTile, "Buildings", "untitled tile sheet")) && this.doesTileHaveProperty(xTile, yTile, "Passable", "Buildings") == null;
  }

  private bool isTileUpperWaterBorder(int index)
  {
    switch (index)
    {
      case 183:
      case 184:
      case 185:
      case 211:
      case 1182:
      case 1183:
      case 1184:
      case 1210:
        return true;
      default:
        return false;
    }
  }

  public virtual bool doesEitherTileOrTileIndexPropertyEqual(
    int xTile,
    int yTile,
    string propertyName,
    string layerName,
    string propertyValue)
  {
    Layer layer = this.map?.GetLayer(layerName);
    if (layer != null)
    {
      Tile tile = layer.PickTile(new Location(xTile * 64 /*0x40*/, yTile * 64 /*0x40*/), Game1.viewport.Size);
      string str1;
      string str2;
      if (tile != null && tile.TileIndexProperties.TryGetValue(propertyName, out str1) && str1 == propertyValue || tile != null && layer.PickTile(new Location(xTile * 64 /*0x40*/, yTile * 64 /*0x40*/), Game1.viewport.Size).Properties.TryGetValue(propertyName, out str2) && str2 == propertyValue)
        return true;
    }
    return propertyValue == null;
  }

  /// <summary>Get whether the given tile prohibits spawned items.</summary>
  /// <param name="tile">The tile position to check.</param>
  /// <param name="type">The spawn type. This can be <c>Grass</c> (weeds, stones, and other debris), <c>Tree</c> (trees), or <c>All</c> (any other type).</param>
  public virtual bool IsNoSpawnTile(Vector2 tile, string type = "All", bool ignoreTileSheetProperties = false)
  {
    string str = this.doesTileHaveProperty((int) tile.X, (int) tile.Y, "NoSpawn", "Back", ignoreTileSheetProperties);
    switch (str)
    {
      case null:
        return this.getBuildingAt(tile) != null;
      case "Grass":
      case "Tree":
        if (type == str)
          return true;
        goto case null;
      default:
        bool result;
        if (!bool.TryParse(str, out result) | result)
          return true;
        goto case null;
    }
  }

  public virtual string doesTileHaveProperty(
    int xTile,
    int yTile,
    string propertyName,
    string layerName,
    bool ignoreTileSheetProperties = false)
  {
    Vector2 tile1 = new Vector2((float) xTile, (float) yTile);
    bool flag = false;
    foreach (Building building in this.buildings)
    {
      if (!building.isMoving && building.occupiesTile(tile1, true))
      {
        string property_value = (string) null;
        if (building.doesTileHaveProperty(xTile, yTile, propertyName, layerName, ref property_value))
          return property_value;
        flag = flag || building.occupiesTile(tile1);
      }
    }
    foreach (Furniture furniture in this.furniture)
    {
      if ((double) xTile >= (double) furniture.tileLocation.X - (double) furniture.GetAdditionalTilePropertyRadius() && (double) xTile < (double) furniture.tileLocation.X + (double) furniture.getTilesWide() + (double) furniture.GetAdditionalTilePropertyRadius() && (double) yTile >= (double) furniture.tileLocation.Y - (double) furniture.GetAdditionalTilePropertyRadius() && (double) yTile < (double) furniture.tileLocation.Y + (double) furniture.getTilesHigh() + (double) furniture.GetAdditionalTilePropertyRadius())
      {
        string property_value = (string) null;
        if (furniture.DoesTileHaveProperty(xTile, yTile, propertyName, layerName, ref property_value))
          return property_value;
      }
    }
    if (!flag && this.map != null)
    {
      Tile tile2 = this.map.GetLayer(layerName)?.Tiles[xTile, yTile];
      string str;
      if (tile2 != null && (tile2.Properties.TryGetValue(propertyName, out str) || !ignoreTileSheetProperties && tile2.TileIndexProperties.TryGetValue(propertyName, out str)))
        return str;
    }
    return (string) null;
  }

  public virtual string doesTileHavePropertyNoNull(
    int xTile,
    int yTile,
    string propertyName,
    string layerName)
  {
    return this.doesTileHaveProperty(xTile, yTile, propertyName, layerName) ?? "";
  }

  /// <summary>Get the space-delimited values defined by a map property.</summary>
  /// <param name="propertyName">The property name to read.</param>
  /// <param name="layerId">The ID for the layer whose tile to check.</param>
  /// <param name="tileX">The X tile position for the map tile to check.</param>
  /// <param name="tileY">The Y tile position for the map tile to check.</param>
  /// <returns>Returns the map property value, or an empty array if it's empty or unset.</returns>
  /// <remarks>See <see cref="M:StardewValley.GameLocation.doesTileHaveProperty(System.Int32,System.Int32,System.String,System.String,System.Boolean)" /> or <see cref="M:StardewValley.GameLocation.doesTileHavePropertyNoNull(System.Int32,System.Int32,System.String,System.String)" /> to get a tile property without splitting it.</remarks>
  public string[] GetTilePropertySplitBySpaces(
    string propertyName,
    string layerId,
    int tileX,
    int tileY)
  {
    string str = this.doesTileHaveProperty(tileX, tileY, propertyName, layerId);
    return str == null ? LegacyShims.EmptyArray<string>() : ArgUtility.SplitBySpace(str);
  }

  /// <summary>Whether a tile coordinate matches a map water tile.</summary>
  /// <param name="xTile">The X tile position.</param>
  /// <param name="yTile">The Y tile position.</param>
  public bool isWaterTile(int xTile, int yTile)
  {
    return this.doesTileHaveProperty(xTile, yTile, "Water", "Back") != null;
  }

  public bool isOpenWater(int xTile, int yTile)
  {
    if (!this.isWaterTile(xTile, yTile))
      return false;
    switch (this.getTileIndexAt(xTile, yTile, "Buildings", "outdoors"))
    {
      case 628:
      case 629:
      case 734:
      case 759:
        return false;
      default:
        return !this.objects.ContainsKey(new Vector2((float) xTile, (float) yTile));
    }
  }

  public bool isCropAtTile(int tileX, int tileY)
  {
    TerrainFeature terrainFeature;
    return this.terrainFeatures.TryGetValue(new Vector2((float) tileX, (float) tileY), out terrainFeature) && terrainFeature is HoeDirt hoeDirt && hoeDirt.crop != null;
  }

  /// <summary>Try to add an object to the location.</summary>
  /// <param name="obj">The object to place. This must be a new instance or <see cref="M:StardewValley.Item.getOne" /> copy; passing a stack that's stored in an inventory will link their state and cause unexpected behaviors.</param>
  /// <param name="dropLocation">The pixel position at which to place the item.</param>
  /// <param name="viewport">Unused.</param>
  /// <param name="initialPlacement">Whether to place the item regardless of the <see cref="F:StardewValley.Object.canBeSetDown" /> field.</param>
  /// <param name="who">The player placing the object, if applicable.</param>
  /// <returns>Returns whether the object was added to the location.</returns>
  public virtual bool dropObject(
    Object obj,
    Vector2 dropLocation,
    xTile.Dimensions.Rectangle viewport,
    bool initialPlacement,
    Farmer who = null)
  {
    Vector2 vector2 = new Vector2((float) ((int) dropLocation.X / 64 /*0x40*/), (float) ((int) dropLocation.Y / 64 /*0x40*/));
    obj.Location = this;
    obj.TileLocation = vector2;
    obj.isSpawnedObject.Value = true;
    if (!this.isTileOnMap(vector2) || this.map.RequireLayer("Back").PickTile(new Location((int) dropLocation.X, (int) dropLocation.Y), Game1.viewport.Size) == null || this.map.RequireLayer("Back").Tiles[(int) vector2.X, (int) vector2.Y].TileIndexProperties.ContainsKey("Unplaceable"))
      return false;
    if (obj.bigCraftable.Value)
    {
      if (!this.isFarm.Value || !obj.setOutdoors.Value && this.isOutdoors.Value || !obj.setIndoors.Value && !this.isOutdoors.Value || obj.performDropDownAction(who))
        return false;
    }
    else if (obj.Type == "Crafting" && obj.performDropDownAction(who))
      obj.CanBeSetDown = false;
    bool flag = this.isTilePassable(new Location((int) vector2.X, (int) vector2.Y), viewport) && this.CanItemBePlacedHere(vector2);
    if ((obj.CanBeSetDown | initialPlacement) & flag && !this.isTileHoeDirt(vector2))
    {
      if (!this.objects.TryAdd(vector2, obj))
        return false;
    }
    else if (this.isWaterTile((int) vector2.X, (int) vector2.Y))
    {
      Game1.multiplayer.broadcastSprites(this, new TemporaryAnimatedSprite(28, 300f, 2, 1, dropLocation, false, obj.flipped.Value));
      this.playSound("dropItemInWater");
    }
    else
    {
      if (obj.CanBeSetDown && !flag)
        return false;
      if (obj.ParentSheetIndex >= 0 && obj.Type != null)
      {
        if (obj.Type == "Fish" || obj.Type == "Cooking" || obj.Type == "Crafting")
        {
          TemporaryAnimatedSprite temporaryAnimatedSprite = new TemporaryAnimatedSprite(0, 150f, 1, 3, dropLocation, true, obj.flipped.Value);
          temporaryAnimatedSprite.CopyAppearanceFromItemId(obj.QualifiedItemId);
          Game1.multiplayer.broadcastSprites(this, temporaryAnimatedSprite);
        }
        else
        {
          TemporaryAnimatedSprite temporaryAnimatedSprite = new TemporaryAnimatedSprite(0, 150f, 1, 3, dropLocation, true, obj.flipped.Value);
          temporaryAnimatedSprite.CopyAppearanceFromItemId(obj.QualifiedItemId, 1);
          Game1.multiplayer.broadcastSprites(this, temporaryAnimatedSprite);
        }
      }
    }
    return true;
  }

  private void rumbleAndFade(int milliseconds) => this.rumbleAndFadeEvent.Fire(milliseconds);

  private void performRumbleAndFade(int milliseconds)
  {
    if (Game1.currentLocation != this)
      return;
    Rumble.rumbleAndFade(1f, (float) milliseconds);
  }

  /// <summary>Sends a request to damage players within the current location.</summary>
  /// <param name="area">The location pixel area where players will take damage.</param>
  /// <param name="damage">The amount of damage the player should take.</param>
  /// <param name="isBomb">Whether the damage source was a bomb.</param>
  private void damagePlayers(Microsoft.Xna.Framework.Rectangle area, int damage, bool isBomb = false)
  {
    this.damagePlayersEvent.Fire(new GameLocation.DamagePlayersEventArg()
    {
      Area = area,
      Damage = damage,
      IsBomb = isBomb
    });
  }

  private void performDamagePlayers(GameLocation.DamagePlayersEventArg arg)
  {
    if (Game1.player.currentLocation != this || arg.IsBomb && Game1.player.hasBuff("dwarfStatue_3"))
      return;
    int damage = arg.Damage;
    if (Game1.player.stats.Get("Book_Bombs") > 0U)
      damage = (int) ((double) damage * 0.75);
    if (!Game1.player.GetBoundingBox().Intersects(arg.Area) || Game1.player.onBridge.Value)
      return;
    Game1.player.takeDamage(damage, true, (Monster) null);
  }

  public void explode(
    Vector2 tileLocation,
    int radius,
    Farmer who,
    bool damageFarmers = true,
    int damage_amount = -1,
    bool destroyObjects = true)
  {
    int num1 = 0;
    this.updateMap();
    Vector2 vector2 = new Vector2(Math.Min((float) (this.map.Layers[0].LayerWidth - 1), Math.Max(0.0f, tileLocation.X - (float) radius)), Math.Min((float) (this.map.Layers[0].LayerHeight - 1), Math.Max(0.0f, tileLocation.Y - (float) radius)));
    bool[,] circleOutlineGrid1 = Game1.getCircleOutlineGrid(radius);
    Microsoft.Xna.Framework.Rectangle rectangle = new Microsoft.Xna.Framework.Rectangle((int) ((double) tileLocation.X - (double) radius) * 64 /*0x40*/, (int) ((double) tileLocation.Y - (double) radius) * 64 /*0x40*/, (radius * 2 + 1) * 64 /*0x40*/, (radius * 2 + 1) * 64 /*0x40*/);
    if (damage_amount > 0)
      this.damageMonster(rectangle, damage_amount, damage_amount, true, who);
    else
      this.damageMonster(rectangle, radius * 6, radius * 8, true, who);
    TemporaryAnimatedSpriteList animatedSpriteList = new TemporaryAnimatedSpriteList();
    TemporaryAnimatedSprite temporaryAnimatedSprite = new TemporaryAnimatedSprite(23, 9999f, 6, 1, new Vector2(vector2.X * 64f, vector2.Y * 64f), false, Game1.random.NextBool());
    temporaryAnimatedSprite.lightId = $"{this.NameOrUniqueName}_{nameof (explode)}_{tileLocation.X}_{tileLocation.Y}_{Game1.random.Next()}";
    temporaryAnimatedSprite.lightRadius = (float) radius;
    temporaryAnimatedSprite.lightcolor = Color.Black;
    temporaryAnimatedSprite.alphaFade = (float) (0.029999999329447746 - (double) radius * (3.0 / 1000.0));
    temporaryAnimatedSprite.Parent = this;
    animatedSpriteList.Add(temporaryAnimatedSprite);
    TemporaryAnimatedSpriteList sprites = animatedSpriteList;
    this.rumbleAndFade(300 + radius * 100);
    if (damageFarmers)
    {
      int damage = damage_amount > 0 ? damage_amount : radius * 3;
      this.damagePlayers(rectangle, damage, true);
    }
    for (int index1 = 0; index1 < radius * 2 + 1; ++index1)
    {
      for (int index2 = 0; index2 < radius * 2 + 1; ++index2)
      {
        if (index1 == 0 || index2 == 0 || index1 == radius * 2 || index2 == radius * 2)
          num1 = circleOutlineGrid1[index1, index2] ? 1 : 0;
        else if (circleOutlineGrid1[index1, index2])
        {
          num1 += index2 <= radius ? 1 : -1;
          if (num1 <= 0)
          {
            if (destroyObjects)
            {
              Object @object;
              if (this.objects.TryGetValue(vector2, out @object) && @object.onExplosion(who))
                this.destroyObject(vector2, who);
              TerrainFeature terrainFeature;
              if (this.terrainFeatures.TryGetValue(vector2, out terrainFeature) && terrainFeature.performToolAction((Tool) null, radius / 2, vector2))
                this.terrainFeatures.Remove(vector2);
            }
            if (Game1.random.NextDouble() < 0.45)
            {
              if (Game1.random.NextBool())
                sprites.Add(new TemporaryAnimatedSprite(362, (float) Game1.random.Next(30, 90), 6, 1, new Vector2(vector2.X * 64f, vector2.Y * 64f), false, Game1.random.NextBool())
                {
                  delayBeforeAnimationStart = Game1.random.Next(700)
                });
              else
                sprites.Add(new TemporaryAnimatedSprite(5, new Vector2(vector2.X * 64f, vector2.Y * 64f), Color.White, animationInterval: 50f)
                {
                  delayBeforeAnimationStart = Game1.random.Next(200),
                  scale = (float) Game1.random.Next(5, 15) / 10f
                });
            }
          }
        }
        if (num1 >= 1)
        {
          this.explosionAt(vector2.X, vector2.Y);
          if (destroyObjects)
          {
            Object @object;
            if (this.objects.TryGetValue(vector2, out @object) && @object.onExplosion(who))
              this.destroyObject(vector2, who);
            TerrainFeature terrainFeature;
            if (this.terrainFeatures.TryGetValue(vector2, out terrainFeature) && terrainFeature.performToolAction((Tool) null, radius / 2, vector2))
              this.terrainFeatures.Remove(vector2);
          }
          if (Game1.random.NextDouble() < 0.45)
          {
            if (Game1.random.NextBool())
              sprites.Add(new TemporaryAnimatedSprite(362, (float) Game1.random.Next(30, 90), 6, 1, new Vector2(vector2.X * 64f, vector2.Y * 64f), false, Game1.random.NextBool())
              {
                delayBeforeAnimationStart = Game1.random.Next(700)
              });
            else
              sprites.Add(new TemporaryAnimatedSprite(5, new Vector2(vector2.X * 64f, vector2.Y * 64f), Color.White, animationInterval: 50f)
              {
                delayBeforeAnimationStart = Game1.random.Next(200),
                scale = (float) Game1.random.Next(5, 15) / 10f
              });
          }
          sprites.Add(new TemporaryAnimatedSprite(6, new Vector2(vector2.X * 64f, vector2.Y * 64f), Color.White, flipped: Game1.random.NextBool(), animationInterval: Vector2.Distance(vector2, tileLocation) * 20f));
        }
        ++vector2.Y;
        vector2.Y = Math.Min((float) (this.map.Layers[0].LayerHeight - 1), Math.Max(0.0f, vector2.Y));
      }
      ++vector2.X;
      vector2.Y = Math.Min((float) (this.map.Layers[0].LayerWidth - 1), Math.Max(0.0f, vector2.X));
      vector2.Y = tileLocation.Y - (float) radius;
      vector2.Y = Math.Min((float) (this.map.Layers[0].LayerHeight - 1), Math.Max(0.0f, vector2.Y));
    }
    Game1.multiplayer.broadcastSprites(this, sprites);
    radius /= 2;
    bool[,] circleOutlineGrid2 = Game1.getCircleOutlineGrid(radius);
    vector2 = new Vector2((float) (int) ((double) tileLocation.X - (double) radius), (float) (int) ((double) tileLocation.Y - (double) radius));
    int num2 = 0;
    for (int index3 = 0; index3 < radius * 2 + 1; ++index3)
    {
      for (int index4 = 0; index4 < radius * 2 + 1; ++index4)
      {
        if (index3 == 0 || index4 == 0 || index3 == radius * 2 || index4 == radius * 2)
          num2 = circleOutlineGrid2[index3, index4] ? 1 : 0;
        else if (circleOutlineGrid2[index3, index4])
        {
          num2 += index4 <= radius ? 1 : -1;
          if (num2 <= 0 && !this.objects.ContainsKey(vector2) && Game1.random.NextDouble() < 0.9 && !this.isTileHoeDirt(vector2) && this.makeHoeDirt(vector2))
            this.checkForBuriedItem((int) vector2.X, (int) vector2.Y, true, false, who);
        }
        if (num2 >= 1 && !this.objects.ContainsKey(vector2) && Game1.random.NextDouble() < 0.9 && !this.isTileHoeDirt(vector2) && this.makeHoeDirt(vector2))
          this.checkForBuriedItem((int) vector2.X, (int) vector2.Y, true, false, who);
        ++vector2.Y;
        vector2.Y = Math.Min((float) (this.map.Layers[0].LayerHeight - 1), Math.Max(0.0f, vector2.Y));
      }
      ++vector2.X;
      vector2.Y = Math.Min((float) (this.map.Layers[0].LayerWidth - 1), Math.Max(0.0f, vector2.X));
      vector2.Y = tileLocation.Y - (float) radius;
      vector2.Y = Math.Min((float) (this.map.Layers[0].LayerHeight - 1), Math.Max(0.0f, vector2.Y));
    }
  }

  public virtual void explosionAt(float x, float y)
  {
  }

  public void removeTemporarySpritesWithID(int id)
  {
    this.removeTemporarySpritesWithIDEvent.Fire(id);
  }

  public void removeTemporarySpritesWithIDLocal(int id)
  {
    this.temporarySprites.RemoveWhere<TemporaryAnimatedSprite>((Predicate<TemporaryAnimatedSprite>) (sprite =>
    {
      if (sprite.id != id)
        return false;
      if (sprite.hasLit)
        Utility.removeLightSource(sprite.lightId);
      return true;
    }));
  }

  /// <summary>Till a tile into a <see cref="T:StardewValley.TerrainFeatures.HoeDirt" /> if it's a valid diggable position, and there isn't already a tilled dirt there.</summary>
  /// <param name="tileLocation">The tile position to till.</param>
  /// <param name="ignoreChecks">Whether to till the tile even if it's occupied or non-diggable.</param>
  /// <returns>Returns whether a <see cref="T:StardewValley.TerrainFeatures.HoeDirt" /> instance was successfully added.</returns>
  public bool makeHoeDirt(Vector2 tileLocation, bool ignoreChecks = false)
  {
    return (ignoreChecks || this.doesTileHaveProperty((int) tileLocation.X, (int) tileLocation.Y, "Diggable", "Back") != null && !this.IsTileBlockedBy(tileLocation, CollisionMask.Buildings | CollisionMask.Flooring | CollisionMask.Furniture | CollisionMask.Objects | CollisionMask.TerrainFeatures | CollisionMask.LocationSpecific) ? (this is MineShaft mineShaft ? (mineShaft.getMineArea() != 77377 ? 1 : 0) : 1) : 0) != 0 && this.terrainFeatures.TryAdd(tileLocation, (TerrainFeature) new HoeDirt(!this.IsRainingHere() || !this.isOutdoors.Value ? 0 : 1, this));
  }

  public int numberOfObjectsOfType(string itemId, bool bigCraftable)
  {
    int num = 0;
    string typeId = bigCraftable ? "(BC)" : "(O)";
    foreach (Object @object in this.Objects.Values)
    {
      if (@object.HasTypeId(typeId) && @object.ItemId == itemId)
        ++num;
    }
    return num;
  }

  public virtual void timeUpdate(int timeElapsed)
  {
    if (Game1.IsMasterGame)
    {
      foreach (FarmAnimal farmAnimal in this.animals.Values)
        farmAnimal.updatePerTenMinutes(Game1.timeOfDay, this);
    }
    foreach (Building building in this.buildings)
    {
      if (building.daysOfConstructionLeft.Value <= 0)
      {
        building.performTenMinuteAction(timeElapsed);
        if (building.GetIndoorsType() == IndoorsType.Instanced)
        {
          GameLocation indoors = building.GetIndoors();
          if (indoors != null)
          {
            foreach (FarmAnimal farmAnimal in indoors.animals.Values)
              farmAnimal.updatePerTenMinutes(Game1.timeOfDay, indoors);
            if (timeElapsed >= 10)
            {
              indoors.performTenMinuteUpdate(Game1.timeOfDay);
              if (timeElapsed > 10)
                indoors.passTimeForObjects(timeElapsed - 10);
            }
          }
        }
      }
    }
  }

  /// <summary>Update all object when the time of day changes.</summary>
  /// <param name="timeElapsed">The number of minutes that passed.</param>
  public void passTimeForObjects(int timeElapsed)
  {
    this.objects.Lock();
    foreach (KeyValuePair<Vector2, Object> pair in this.objects.Pairs)
    {
      if (pair.Value.minutesElapsed(timeElapsed))
        this.objects.Remove(pair.Key);
    }
    this.objects.Unlock();
  }

  public virtual void performTenMinuteUpdate(int timeOfDay)
  {
    for (int index = 0; index < this.furniture.Count; ++index)
      this.furniture[index].minutesElapsed(10);
    for (int index = 0; index < this.characters.Count; ++index)
    {
      NPC character = this.characters[index];
      if (!character.IsInvisible)
      {
        character.checkSchedule(timeOfDay);
        character.performTenMinuteUpdate(timeOfDay, this);
      }
    }
    this.passTimeForObjects(10);
    if (this.isOutdoors.Value)
    {
      Random daySaveRandom = Utility.CreateDaySaveRandom((double) timeOfDay, (double) this.Map.Layers[0].LayerWidth);
      if (this.Equals(Game1.currentLocation))
        this.tryToAddCritters(true);
      if (Game1.IsMasterGame)
      {
        int minutesBetweenTimes = Utility.CalculateMinutesBetweenTimes(this.fishSplashPointTime, Game1.timeOfDay);
        bool flag = this.fishFrenzyFish.Value != null && !this.fishFrenzyFish.Value.Equals("");
        if (this.fishSplashPoint.Value.Equals(Point.Zero) && daySaveRandom.NextBool() && (!(this is Farm) || Game1.whichFarm == 1))
        {
          for (int index = 0; index < 2; ++index)
          {
            Point p = new Point(daySaveRandom.Next(0, this.map.RequireLayer("Back").LayerWidth), daySaveRandom.Next(0, this.map.RequireLayer("Back").LayerHeight));
            if (this.isOpenWater(p.X, p.Y) && this.doesTileHaveProperty(p.X, p.Y, "NoFishing", "Back") == null)
            {
              int land = FishingRod.distanceToLand(p.X, p.Y, this);
              if (land > 1 && land < 5)
              {
                if (Game1.player.currentLocation.Equals(this))
                  this.playSound("waterSlosh");
                if (daySaveRandom.NextDouble() < (this is Beach ? 0.008 : 0.01) && Game1.Date.TotalDays > 3)
                {
                  switch (this)
                  {
                    case Town _:
                    case Mountain _:
                    case Forest _:
                    case Beach _:
                      if (Game1.timeOfDay < 2300 && (Game1.player.fishCaught.Count() > 2 || Game1.Date.TotalDays > 14) && !Utility.isFestivalDay())
                      {
                        Item fish = this.getFish((float) daySaveRandom.Next(500), "", land, Game1.player, 0.0, Utility.PointToVector2(p));
                        if (fish.Category == -4 && !fish.HasContextTag("fish_legendary"))
                        {
                          this.fishFrenzyFish.Value = fish.QualifiedItemId;
                          string str1;
                          switch (this)
                          {
                            case Mountain _:
                              str1 = "mountain";
                              break;
                            case Forest _:
                              str1 = "forest";
                              break;
                            case Town _:
                              str1 = "town";
                              break;
                            default:
                              str1 = "beach";
                              break;
                          }
                          string word = TokenStringBuilder.ItemNameFor(fish);
                          string str2 = TokenStringBuilder.CapitalizeFirstLetter(TokenStringBuilder.ArticleFor(word));
                          Game1.multiplayer.broadcastGlobalMessage("Strings\\1_6_Strings:FishFrenzy_" + str1, false, (GameLocation) null, word, str2);
                          break;
                        }
                        break;
                      }
                      break;
                  }
                }
                this.fishSplashPointTime = Game1.timeOfDay;
                this.fishSplashPoint.Value = p;
                break;
              }
            }
          }
        }
        else if (!this.fishSplashPoint.Value.Equals(Point.Zero) && daySaveRandom.NextDouble() < 0.1 + (double) minutesBetweenTimes / 1800.0 && minutesBetweenTimes > (flag ? 120 : 60))
        {
          this.fishSplashPointTime = 0;
          this.fishFrenzyFish.Value = "";
          this.fishSplashPoint.Value = Point.Zero;
        }
        this.performOrePanTenMinuteUpdate(daySaveRandom);
      }
    }
    if (Game1.dayOfMonth % 7 == 0 && Game1.timeOfDay >= 1200 && Game1.timeOfDay <= 1500 && this.name.Equals((object) "Saloon") && NetWorldState.checkAnywhereForWorldStateID("saloonSportsRoom"))
    {
      if (Game1.timeOfDay == 1500)
      {
        this.removeTemporarySpritesWithID(2400);
      }
      else
      {
        bool flag1 = Game1.random.NextDouble() < 0.25;
        bool flag2 = Game1.random.NextDouble() < 0.25;
        List<NPC> npcList = new List<NPC>();
        foreach (NPC character in this.characters)
        {
          if (character.TilePoint.Y < 12 && character.TilePoint.X > 26 && Game1.random.NextDouble() < (flag1 | flag2 ? 0.66 : 0.25))
            npcList.Add(character);
        }
        foreach (NPC npc in npcList)
        {
          npc.showTextAboveHead(Game1.content.LoadString($"Strings\\Characters:Saloon_{(flag1 ? "goodEvent" : (flag2 ? "badEvent" : "neutralEvent"))}_{Game1.random.Next(5).ToString()}"));
          if (flag1 && Game1.random.NextDouble() < 0.55)
            npc.jump();
        }
      }
    }
    if (!Game1.currentLocation.Equals(this) || !this.name.Equals((object) "BugLand") || Game1.random.NextDouble() > 0.2)
      return;
    this.characters.Add((NPC) new Fly(this.getRandomTile() * 64f, true));
  }

  public virtual bool performOrePanTenMinuteUpdate(Random r)
  {
    Point point;
    if (Game1.MasterPlayer.mailReceived.Contains("ccFishTank") && !(this is Beach))
    {
      point = this.orePanPoint.Value;
      if (point.Equals(Point.Zero) && r.NextBool())
      {
        for (int index = 0; index < 8; ++index)
        {
          Point tile = new Point(r.Next(0, this.Map.RequireLayer("Back").LayerWidth), r.Next(0, this.Map.RequireLayer("Back").LayerHeight));
          if (this.isOpenWater(tile.X, tile.Y) && FishingRod.distanceToLand(tile.X, tile.Y, this, true) <= 1 && !this.hasTileAt(tile, "Buildings"))
          {
            if (Game1.player.currentLocation.Equals(this))
              this.playSound("slosh");
            this.orePanPoint.Value = tile;
            return true;
          }
        }
        goto label_11;
      }
    }
    point = this.orePanPoint.Value;
    if (!point.Equals(Point.Zero) && r.NextDouble() < 0.1)
      this.orePanPoint.Value = Point.Zero;
label_11:
    return false;
  }

  /// <summary>Get the fish types that can be caught by crab pots on a given tile.</summary>
  /// <param name="tile">The tile position containing the crab pot.</param>
  public virtual IList<string> GetCrabPotFishForTile(Vector2 tile)
  {
    if (this.catchOceanCrabPotFishFromThisSpot((int) tile.X, (int) tile.Y))
      return GameLocation.OceanCrabPotFishTypes;
    FishAreaData data;
    if (this.TryGetFishAreaForTile(tile, out string _, out data))
    {
      List<string> crabPotFishTypes = data.CrabPotFishTypes;
      // ISSUE: explicit non-virtual call
      if ((crabPotFishTypes != null ? (__nonvirtual (crabPotFishTypes.Count) > 0 ? 1 : 0) : 0) != 0)
        return (IList<string>) data.CrabPotFishTypes;
    }
    return GameLocation.DefaultCrabPotFishTypes;
  }

  /// <summary>Get the fish area that applies to the given tile, if any.</summary>
  /// <param name="tile">The tile to check.</param>
  /// <param name="id">The fish area ID which applies, if any.</param>
  /// <param name="data">The fish area data which applies, if any.</param>
  public virtual bool TryGetFishAreaForTile(Vector2 tile, out string id, out FishAreaData data)
  {
    LocationData data1 = this.GetData();
    if (data1?.FishAreas != null)
    {
      string str = (string) null;
      FishAreaData fishAreaData1 = (FishAreaData) null;
      foreach (KeyValuePair<string, FishAreaData> fishArea in data1.FishAreas)
      {
        FishAreaData fishAreaData2 = fishArea.Value;
        Microsoft.Xna.Framework.Rectangle? position = fishAreaData2.Position;
        ref Microsoft.Xna.Framework.Rectangle? local = ref position;
        bool? nullable = local.HasValue ? new bool?(local.GetValueOrDefault().Contains((int) tile.X, (int) tile.Y)) : new bool?();
        if (nullable.HasValue)
        {
          if (nullable.GetValueOrDefault())
          {
            id = fishArea.Key;
            data = fishAreaData2;
            return true;
          }
        }
        else if (str == null)
        {
          str = fishArea.Key;
          fishAreaData1 = fishArea.Value;
        }
      }
      if (str != null)
      {
        id = str;
        data = fishAreaData1;
        return true;
      }
    }
    id = (string) null;
    data = (FishAreaData) null;
    return false;
  }

  /// <summary>Get the display name for a fishing area, if it has one.</summary>
  /// <param name="id">The fishing area ID, as returned by <see cref="M:StardewValley.GameLocation.TryGetFishAreaForTile(Microsoft.Xna.Framework.Vector2,System.String@,StardewValley.GameData.Locations.FishAreaData@)" />.</param>
  public virtual string GetFishingAreaDisplayName(string id)
  {
    LocationData data = this.GetData();
    FishAreaData fishAreaData;
    return data?.FishAreas == null || !data.FishAreas.TryGetValue(id, out fishAreaData) || fishAreaData.DisplayName == null ? (string) null : TokenParser.ParseText(fishAreaData.DisplayName);
  }

  /// <summary>Get a random fish that can be caught in this location.</summary>
  /// <param name="millisecondsAfterNibble">The number of milliseconds after the fish starting biting before the player reacted and pressed the tool button.</param>
  /// <param name="bait">The qualified item ID for the bait attached to the fishing rod, if any.</param>
  /// <param name="waterDepth">The tile distance from the nearest shore.</param>
  /// <param name="who">The player who's fishing.</param>
  /// <param name="baitPotency">Unused.</param>
  /// <param name="bobberTile">The tile position where the fishing rod's bobber is floating.</param>
  /// <param name="locationName">The name of the location whose fish to get, or <c>null</c> for the current location.</param>
  public virtual Item getFish(
    float millisecondsAfterNibble,
    string bait,
    int waterDepth,
    Farmer who,
    double baitPotency,
    Vector2 bobberTile,
    string locationName = null)
  {
    if (locationName != null && locationName != this.Name && (!(locationName == "UndergroundMine") || !(this is MineShaft)))
    {
      GameLocation locationFromName = Game1.getLocationFromName(locationName);
      if (locationFromName != null && locationFromName != this)
        return locationFromName.getFish(millisecondsAfterNibble, bait, waterDepth, who, baitPotency, bobberTile);
    }
    if (bobberTile != Vector2.Zero && who.currentLocation?.NameOrUniqueName == this.NameOrUniqueName)
    {
      foreach (Building building in this.buildings)
      {
        if (building is FishPond fishPond && fishPond.isTileFishable(bobberTile))
          return (Item) fishPond.CatchFish();
      }
    }
    if (this.fishFrenzyFish.Value != null && !this.fishFrenzyFish.Value.Equals("") && (double) Vector2.Distance(bobberTile, Utility.PointToVector2(this.fishSplashPoint.Value)) <= 2.0)
      return ItemRegistry.Create(this.fishFrenzyFish.Value);
    bool isTutorialCatch = who.fishCaught.Length == 0;
    return GameLocation.GetFishFromLocationData(this.Name, bobberTile, waterDepth, who, isTutorialCatch, false, this) ?? ItemRegistry.Create("(O)168");
  }

  /// <summary>Get a random fish that can be caught for a given location based on its <c>Data\Locations</c> entry. This doesn't include global default fish and special cases; most code should call <see cref="M:StardewValley.GameLocation.getFish(System.Single,System.String,System.Int32,StardewValley.Farmer,System.Double,Microsoft.Xna.Framework.Vector2,System.String)" /> instead.</summary>
  /// <param name="locationName">The name of the location whose fish to get.</param>
  /// <param name="bobberTile">The tile position where the fishing rod's bobber is floating.</param>
  /// <param name="waterDepth">The tile distance from the nearest shore.</param>
  /// <param name="player">The player who's fishing.</param>
  /// <param name="isTutorialCatch">Whether this is the player's first catch, so it should be an easy fish for the tutorial.</param>
  /// <param name="isInherited">Whether we're loading fish indirectly (e.g. via the <c>LOCATION_FISH</c> item query), rather than for the actual location.</param>
  /// <param name="location">The location instance from which to get context data. If this is <c>null</c>, it'll be loaded based on the <paramref name="locationName" />; if that fails, generic context info (e.g. current location's weather) will be used instead.</param>
  /// <returns>Returns the fish to catch, or <c>null</c> if no match was found.</returns>
  public static Item GetFishFromLocationData(
    string locationName,
    Vector2 bobberTile,
    int waterDepth,
    Farmer player,
    bool isTutorialCatch,
    bool isInherited,
    GameLocation location = null)
  {
    return GameLocation.GetFishFromLocationData(locationName, bobberTile, waterDepth, player, isTutorialCatch, isInherited, location, (ItemQueryContext) null);
  }

  /// <summary>Get a random fish that can be caught for a given location based on its <c>Data\Locations</c> entry. This doesn't include global default fish and special cases; most code should call <see cref="M:StardewValley.GameLocation.getFish(System.Single,System.String,System.Int32,StardewValley.Farmer,System.Double,Microsoft.Xna.Framework.Vector2,System.String)" /> instead.</summary>
  /// <param name="locationName">The name of the location whose fish to get.</param>
  /// <param name="bobberTile">The tile position where the fishing rod's bobber is floating.</param>
  /// <param name="waterDepth">The tile distance from the nearest shore.</param>
  /// <param name="player">The player who's fishing.</param>
  /// <param name="isTutorialCatch">Whether this is the player's first catch, so it should be an easy fish for the tutorial.</param>
  /// <param name="isInherited">Whether we're loading fish indirectly (e.g. via the <c>LOCATION_FISH</c> item query), rather than for the actual location.</param>
  /// <param name="location">The location instance from which to get context data. If this is <c>null</c>, it'll be loaded based on the <paramref name="locationName" />; if that fails, generic context info (e.g. current location's weather) will be used instead.</param>
  /// <param name="itemQueryContext">The context for the item query which led to this call, if applicable. This is used internally to prevent circular loops.</param>
  /// <returns>Returns the fish to catch, or <c>null</c> if no match was found.</returns>
  internal static Item GetFishFromLocationData(
    string locationName,
    Vector2 bobberTile,
    int waterDepth,
    Farmer player,
    bool isTutorialCatch,
    bool isInherited,
    GameLocation location,
    ItemQueryContext itemQueryContext)
  {
    if (location == null)
      location = Game1.getLocationFromName(locationName);
    LocationData locationData = location != null ? location.GetData() : GameLocation.GetData(locationName);
    Dictionary<string, string> allFishData = DataLoader.Fish(Game1.content);
    Season seasonForLocation = Game1.GetSeasonForLocation(location);
    string id;
    if (location == null || !location.TryGetFishAreaForTile(bobberTile, out id, out FishAreaData _))
      id = (string) null;
    bool usingMagicBait = false;
    bool hasCuriosityLure = false;
    string str = (string) null;
    bool flag = false;
    if (player?.CurrentTool is FishingRod currentTool && currentTool.isFishing)
    {
      usingMagicBait = currentTool.HasMagicBait();
      hasCuriosityLure = currentTool.HasCuriosityLure();
      Object bait = currentTool.GetBait();
      if (bait != null)
      {
        if (bait.QualifiedItemId == "(O)SpecificBait" && bait.preservedParentSheetIndex.Value != null)
          str = "(O)" + bait.preservedParentSheetIndex.Value;
        if (bait.QualifiedItemId != "(O)685")
          flag = true;
      }
    }
    Point tilePoint = player.TilePoint;
    if (itemQueryContext == null)
      itemQueryContext = new ItemQueryContext(location, (Farmer) null, Game1.random, $"location '{locationName}' > fish data");
    IEnumerable<SpawnFishData> spawnFishDatas1 = (IEnumerable<SpawnFishData>) Game1.locationData["Default"].Fish;
    if (locationData != null)
    {
      int? count = locationData.Fish?.Count;
      int num = 0;
      if (count.GetValueOrDefault() > num & count.HasValue)
        spawnFishDatas1 = spawnFishDatas1.Concat<SpawnFishData>((IEnumerable<SpawnFishData>) locationData.Fish);
    }
    IEnumerable<SpawnFishData> spawnFishDatas2 = (IEnumerable<SpawnFishData>) spawnFishDatas1.OrderBy<SpawnFishData, int>((Func<SpawnFishData, int>) (p => p.Precedence)).ThenBy<SpawnFishData, int>((Func<SpawnFishData, int>) (p => Game1.random.Next()));
    int num1 = 0;
    HashSet<string> baitIgnoreQueryKeys = usingMagicBait ? GameStateQuery.MagicBaitIgnoreQueryKeys : (HashSet<string>) null;
    Item fromLocationData = (Item) null;
    for (int index = 0; index < 2; ++index)
    {
      foreach (SpawnFishData spawnFishData in spawnFishDatas2)
      {
        SpawnFishData spawn = spawnFishData;
        if ((!isInherited || spawn.CanBeInherited) && (spawn.FishAreaId == null || !(id != spawn.FishAreaId)))
        {
          Season? season1 = spawn.Season;
          if (season1.HasValue && !usingMagicBait)
          {
            season1 = spawn.Season;
            Season season2 = seasonForLocation;
            if (!(season1.GetValueOrDefault() == season2 & season1.HasValue))
              continue;
          }
          Microsoft.Xna.Framework.Rectangle? nullable = spawn.PlayerPosition;
          ref Microsoft.Xna.Framework.Rectangle? local1 = ref nullable;
          if ((local1.HasValue ? (!local1.GetValueOrDefault().Contains(tilePoint.X, tilePoint.Y) ? 1 : 0) : 0) == 0)
          {
            nullable = spawn.BobberPosition;
            ref Microsoft.Xna.Framework.Rectangle? local2 = ref nullable;
            if ((local2.HasValue ? (!local2.GetValueOrDefault().Contains((int) bobberTile.X, (int) bobberTile.Y) ? 1 : 0) : 0) == 0 && player.FishingLevel >= spawn.MinFishingLevel && waterDepth >= spawn.MinDistanceFromShore && (spawn.MaxDistanceFromShore <= -1 || waterDepth <= spawn.MaxDistanceFromShore) && (!spawn.RequireMagicBait || usingMagicBait))
            {
              float chance = spawn.GetChance(hasCuriosityLure, player.DailyLuck, player.LuckLevel, (Func<float, IList<QuantityModifier>, QuantityModifier.QuantityModifierMode, float>) ((value, modifiers, mode) => Utility.ApplyQuantityModifiers(value, modifiers, mode, location)), spawn.ItemId == str);
              if (spawn.UseFishCaughtSeededRandom)
              {
                if (!Utility.CreateRandom((double) Game1.uniqueIDForThisGame, (double) (player.stats.Get("PreciseFishCaught") * 859U)).NextBool(chance))
                  continue;
              }
              else if (!Game1.random.NextBool(chance))
                continue;
              if (spawn.Condition == null || GameStateQuery.CheckConditions(spawn.Condition, location, ignoreQueryKeys: baitIgnoreQueryKeys))
              {
                Item obj = ItemQueryResolver.TryResolveRandomItem((ISpawnItemData) spawn, itemQueryContext, formatItemId: (Func<string, string>) (query => query.Replace("BOBBER_X", ((int) bobberTile.X).ToString()).Replace("BOBBER_Y", ((int) bobberTile.Y).ToString()).Replace("WATER_DEPTH", waterDepth.ToString())), logError: (Action<string, string>) ((query, error) => Game1.log.Error($"Location '{location.NameOrUniqueName}' failed parsing item query '{query}' for fish '{spawn.Id}': {error}")));
                if (obj != null)
                {
                  if (!string.IsNullOrWhiteSpace(spawn.SetFlagOnCatch))
                    obj.SetFlagOnPickup = spawn.SetFlagOnCatch;
                  if (spawn.IsBossFish)
                    obj.SetTempData<bool>("IsBossFish", true);
                  Item fish = obj;
                  int[] numArray;
                  if ((spawn.CatchLimit <= -1 || !player.fishCaught.TryGetValue(fish.QualifiedItemId, out numArray) || numArray[0] < spawn.CatchLimit) && GameLocation.CheckGenericFishRequirements(fish, allFishData, location, player, spawn, waterDepth, usingMagicBait, hasCuriosityLure, spawn.ItemId == str, isTutorialCatch))
                  {
                    if (str == null || !(fish.QualifiedItemId != str) || num1 >= 2)
                      return fish;
                    if (fromLocationData == null)
                      fromLocationData = fish;
                    ++num1;
                  }
                }
              }
            }
          }
        }
      }
      if (!flag)
        ++index;
    }
    if (fromLocationData != null)
      return fromLocationData;
    return !isTutorialCatch ? (Item) null : ItemRegistry.Create("(O)145");
  }

  /// <summary>Get whether a fish can be spawned based on its requirements in Data/Fish, if applicable.</summary>
  /// <param name="fish">The fish being checked.</param>
  /// <param name="allFishData">The Data/Fish data to check.</param>
  /// <param name="location">The location for which fish are being caught.</param>
  /// <param name="player">The player catching fish.</param>
  /// <param name="spawn">The fish spawn rule for which a fish is being checked.</param>
  /// <param name="waterDepth">The current water depth for the fishing bobber.</param>
  /// <param name="usingMagicBait">Whether the player has the magic bait equipped.</param>
  /// <param name="hasCuriosityLure">Whether the player has the curiosity lure equipped.</param>
  /// <param name="usingTargetBait">Whether the player has the target bait equipped.</param>
  /// <param name="isTutorialCatch">Whether this is the player's first catch, so it should be an easy fish for the tutorial.</param>
  internal static bool CheckGenericFishRequirements(
    Item fish,
    Dictionary<string, string> allFishData,
    GameLocation location,
    Farmer player,
    SpawnFishData spawn,
    int waterDepth,
    bool usingMagicBait,
    bool hasCuriosityLure,
    bool usingTargetBait,
    bool isTutorialCatch)
  {
    string str1;
    if (!fish.HasTypeObject() || !allFishData.TryGetValue(fish.ItemId, out str1))
      return !isTutorialCatch;
    string[] array1 = str1.Split('/');
    if (ArgUtility.Get(array1, 1) == "trap")
      return !isTutorialCatch;
    bool flag1 = player?.CurrentTool?.QualifiedItemId == "(T)TrainingRod";
    if (flag1)
    {
      bool? canUseTrainingRod = spawn.CanUseTrainingRod;
      if (canUseTrainingRod.HasValue)
      {
        if (!canUseTrainingRod.GetValueOrDefault())
          return false;
      }
      else
      {
        int num;
        string error;
        if (!ArgUtility.TryGetInt(array1, 1, out num, out error, "int difficulty"))
          return LogFormatError(error);
        if (num >= 50)
          return false;
      }
    }
    if (isTutorialCatch)
    {
      bool flag2;
      string error;
      if (!ArgUtility.TryGetOptionalBool(array1, 13, out flag2, out error, name: "bool isTutorialFish"))
        return LogFormatError(error);
      if (!flag2)
        return false;
    }
    if (!spawn.IgnoreFishDataRequirements)
    {
      if (!usingMagicBait)
      {
        string str2;
        string error;
        if (!ArgUtility.TryGet(array1, 5, out str2, out error, name: "string rawTimeSpans"))
          return LogFormatError(error);
        string[] array2 = ArgUtility.SplitBySpace(str2);
        bool flag3 = false;
        for (int index = 0; index < array2.Length; index += 2)
        {
          int num1;
          int num2;
          if (!ArgUtility.TryGetInt(array2, index, out num1, out error, "int startTime") || !ArgUtility.TryGetInt(array2, index + 1, out num2, out error, "int endTime"))
            return LogFormatError($"invalid time spans '{str2}': {error}");
          if (Game1.timeOfDay >= num1 && Game1.timeOfDay < num2)
          {
            flag3 = true;
            break;
          }
        }
        if (!flag3)
          return false;
      }
      if (!usingMagicBait)
      {
        string str3;
        string error;
        if (!ArgUtility.TryGet(array1, 7, out str3, out error, name: "string weather"))
          return LogFormatError(error);
        switch (str3)
        {
          case "rainy":
            if (!location.IsRainingHere())
              return false;
            break;
          case "sunny":
            if (location.IsRainingHere())
              return false;
            break;
        }
      }
      int num3;
      string error1;
      if (!ArgUtility.TryGetInt(array1, 12, out num3, out error1, "int minFishingLevel"))
        return LogFormatError(error1);
      if (player.FishingLevel < num3)
        return false;
      int num4;
      string error2;
      float num5;
      float num6;
      if (!ArgUtility.TryGetInt(array1, 9, out num4, out error2, "int maxDepth") || !ArgUtility.TryGetFloat(array1, 10, out num5, out error2, "float chance") || !ArgUtility.TryGetFloat(array1, 11, out num6, out error2, "float depthMultiplier"))
        return LogFormatError(error2);
      float num7 = num6 * num5;
      num5 -= (float) Math.Max(0, num4 - waterDepth) * num7;
      num5 += (float) player.FishingLevel / 50f;
      if (flag1)
        num5 *= 1.1f;
      num5 = Math.Min(num5, 0.9f);
      if ((double) num5 < 0.25 & hasCuriosityLure)
      {
        if ((double) spawn.CuriosityLureBuff > -1.0)
        {
          num5 += spawn.CuriosityLureBuff;
        }
        else
        {
          float num8 = 0.25f;
          float num9 = 0.08f;
          num5 = (float) (((double) num8 - (double) num9) / (double) num8 * (double) num5 + ((double) num8 - (double) num9) / 2.0);
        }
      }
      if (usingTargetBait)
        num5 *= 1.66f;
      if (spawn.ApplyDailyLuck)
        num5 += (float) player.DailyLuck;
      List<QuantityModifier> chanceModifiers = spawn.ChanceModifiers;
      // ISSUE: explicit non-virtual call
      if ((chanceModifiers != null ? (__nonvirtual (chanceModifiers.Count) > 0 ? 1 : 0) : 0) != 0)
        num5 = Utility.ApplyQuantityModifiers(num5, (IList<QuantityModifier>) spawn.ChanceModifiers, spawn.ChanceModifierMode, location);
      if (!Game1.random.NextBool(num5))
        return false;
    }
    return true;

    bool LogFormatError(string error)
    {
      Game1.log.Warn($"Skipped fish '{fish.ItemId}' due to invalid requirements in Data/Fish: {error}");
      return false;
    }
  }

  public virtual bool isActionableTile(int xTile, int yTile, Farmer who)
  {
    foreach (Building building in this.buildings)
    {
      if (building.isActionableTile(xTile, yTile, who))
        return true;
    }
    bool flag = false;
    string[] action = ArgUtility.SplitBySpace(this.doesTileHaveProperty(xTile, yTile, "Action", "Buildings"));
    if (!this.ShouldIgnoreAction(action, who, new Location(xTile, yTile)))
    {
      switch (action[0])
      {
        case "Dialogue":
        case "Message":
        case "MessageOnce":
        case "NPCMessage":
          flag = true;
          Game1.isInspectionAtCurrentCursorTile = true;
          break;
        case "MessageSpeech":
          flag = true;
          Game1.isSpeechAtCurrentCursorTile = true;
          break;
        default:
          flag = true;
          break;
      }
    }
    if (!flag)
    {
      Object @object;
      if (this.objects.TryGetValue(new Vector2((float) xTile, (float) yTile), out @object) && @object.isActionable(who))
        flag = true;
      TerrainFeature terrainFeature;
      if (!Game1.isFestival() && this.terrainFeatures.TryGetValue(new Vector2((float) xTile, (float) yTile), out terrainFeature) && terrainFeature.isActionable())
        flag = true;
    }
    if (flag && !Utility.tileWithinRadiusOfPlayer(xTile, yTile, 1, who))
      Game1.mouseCursorTransparency = 0.5f;
    return flag;
  }

  public Item tryGetRandomArtifactFromThisLocation(Farmer who, Random r, double chanceMultipler = 1.0)
  {
    LocationData data = this.GetData();
    ItemQueryContext context = new ItemQueryContext(this, who, r, $"location '{this.NameOrUniqueName}' > artifact spots");
    IEnumerable<ArtifactSpotDropData> artifactSpotDropDatas = (IEnumerable<ArtifactSpotDropData>) Game1.locationData["Default"].ArtifactSpots;
    if (data != null)
    {
      int? count = data.ArtifactSpots?.Count;
      int num = 0;
      if (count.GetValueOrDefault() > num & count.HasValue)
        artifactSpotDropDatas = artifactSpotDropDatas.Concat<ArtifactSpotDropData>((IEnumerable<ArtifactSpotDropData>) data.ArtifactSpots);
    }
    foreach (ArtifactSpotDropData artifactSpotDropData in (IEnumerable<ArtifactSpotDropData>) artifactSpotDropDatas.OrderBy<ArtifactSpotDropData, int>((Func<ArtifactSpotDropData, int>) (p => p.Precedence)))
    {
      ArtifactSpotDropData drop = artifactSpotDropData;
      if (r.NextBool(drop.Chance * chanceMultipler) && (drop.Condition == null || GameStateQuery.CheckConditions(drop.Condition, this, who, random: r)))
      {
        Item fromThisLocation = ItemQueryResolver.TryResolveRandomItem((ISpawnItemData) drop, context, logError: (Action<string, string>) ((query, error) => Game1.log.Error($"Location '{this.NameOrUniqueName}' failed parsing item query '{query}' for artifact spot '{drop.Id}': {error}")));
        if (fromThisLocation != null)
          return fromThisLocation;
      }
    }
    return (Item) null;
  }

  public virtual void digUpArtifactSpot(int xLocation, int yLocation, Farmer who)
  {
    Random daySaveRandom = Utility.CreateDaySaveRandom((double) (xLocation * 2000), (double) yLocation, (double) (Game1.netWorldState.Value.TreasureTotemsUsed * 777));
    Vector2 vector2 = new Vector2((float) (xLocation * 64 /*0x40*/), (float) (yLocation * 64 /*0x40*/));
    bool flag = who?.CurrentTool is Hoe currentTool && currentTool.hasEnchantmentOfType<GenerousEnchantment>();
    LocationData data = this.GetData();
    ItemQueryContext context = new ItemQueryContext(this, who, daySaveRandom, $"location '{this.NameOrUniqueName}' > artifact spots");
    IEnumerable<ArtifactSpotDropData> artifactSpotDropDatas1 = (IEnumerable<ArtifactSpotDropData>) Game1.locationData["Default"].ArtifactSpots;
    if (data != null)
    {
      int? count = data.ArtifactSpots?.Count;
      int num = 0;
      if (count.GetValueOrDefault() > num & count.HasValue)
        artifactSpotDropDatas1 = artifactSpotDropDatas1.Concat<ArtifactSpotDropData>((IEnumerable<ArtifactSpotDropData>) data.ArtifactSpots);
    }
    IEnumerable<ArtifactSpotDropData> artifactSpotDropDatas2 = (IEnumerable<ArtifactSpotDropData>) artifactSpotDropDatas1.OrderBy<ArtifactSpotDropData, int>((Func<ArtifactSpotDropData, int>) (p => p.Precedence));
    if (Game1.player.mailReceived.Contains("sawQiPlane") && daySaveRandom.NextDouble() < 0.05 + Game1.player.team.AverageDailyLuck() / 2.0)
      Game1.createMultipleItemDebris(ItemRegistry.Create("(O)MysteryBox", daySaveRandom.Next(1, 3)), vector2, -1, this);
    Utility.trySpawnRareObject(who, vector2, this, 9.0, random: daySaveRandom);
    foreach (ArtifactSpotDropData artifactSpotDropData in artifactSpotDropDatas2)
    {
      ArtifactSpotDropData drop = artifactSpotDropData;
      if (daySaveRandom.NextBool(drop.Chance) && (drop.Condition == null || GameStateQuery.CheckConditions(drop.Condition, this, who, random: daySaveRandom)))
      {
        Item obj1 = ItemQueryResolver.TryResolveRandomItem((ISpawnItemData) drop, context, logError: (Action<string, string>) ((query, error) => Game1.log.Error($"Location '{this.NameOrUniqueName}' failed parsing item query '{query}' for artifact spot '{drop.Id}': {error}")));
        if (obj1 != null)
        {
          if (drop.OneDebrisPerDrop && obj1.Stack > 1)
            Game1.createMultipleItemDebris(obj1, vector2, -1, this);
          else
            Game1.createItemDebris(obj1, vector2, Game1.random.Next(4), this);
          if (flag && drop.ApplyGenerousEnchantment && daySaveRandom.NextBool())
          {
            Item obj2 = (Item) ItemQueryResolver.ApplyItemFields((ISalable) obj1.getOne(), (ISpawnItemData) drop, context);
            if (drop.OneDebrisPerDrop && obj2.Stack > 1)
              Game1.createMultipleItemDebris(obj2, vector2, -1, this);
            else
              Game1.createItemDebris(obj2, vector2, -1, this);
          }
          if (!drop.ContinueOnDrop)
            break;
        }
      }
    }
  }

  /// <summary>Get the underlying data from <c>Data/Locations</c> for this location, if available.</summary>
  /// <remarks>If this is a passive festival location and doesn't have its own data, this will return the data matching its <see cref="F:StardewValley.GameData.PassiveFestivalData.MapReplacements" /> field.</remarks>
  public LocationData GetData()
  {
    string name = this.Name;
    switch (this)
    {
      case MineShaft _:
        name = "UndergroundMine";
        break;
      case Cellar _:
        if (name.StartsWith("Cellar"))
        {
          name = "Cellar";
          break;
        }
        break;
    }
    return GameLocation.GetData(name);
  }

  /// <summary>Get the underlying data from <c>Data/Locations</c> for this location, if available.</summary>
  /// <param name="name">The location name to match.</param>
  /// <remarks>If this is a passive festival location and doesn't have its own data, this will return the data matching its <see cref="F:StardewValley.GameData.PassiveFestivalData.MapReplacements" /> field.</remarks>
  public static LocationData GetData(string name)
  {
    IDictionary<string, LocationData> rawData = Game1.locationData;
    return name == "Farm" ? GetImpl("Farm_" + Game1.GetFarmTypeKey()) ?? GetImpl("Farm_Standard") : GetImpl(name);

    LocationData GetImpl(string entryName)
    {
      LocationData impl;
      if (rawData.TryGetValue(entryName, out impl))
        return impl;
      foreach (string activePassiveFestival in (IEnumerable<string>) Game1.netWorldState.Value.ActivePassiveFestivals)
      {
        PassiveFestivalData data;
        if (Utility.TryGetPassiveFestivalData(activePassiveFestival, out data) && data.MapReplacements != null)
        {
          foreach (KeyValuePair<string, string> mapReplacement in data.MapReplacements)
          {
            if (mapReplacement.Value == entryName)
            {
              if (rawData.TryGetValue(mapReplacement.Key, out impl))
                return impl;
              break;
            }
          }
        }
      }
      return (LocationData) null;
    }
  }

  /// <summary>Get whether NPCs should ignore this location when pathfinding between locations.</summary>
  public virtual bool ShouldExcludeFromNpcPathfinding()
  {
    LocationData data = this.GetData();
    return data != null && data.ExcludeFromNpcPathfinding;
  }

  public virtual string checkForBuriedItem(
    int xLocation,
    int yLocation,
    bool explosion,
    bool detectOnly,
    Farmer who)
  {
    Random daySaveRandom = Utility.CreateDaySaveRandom((double) (xLocation * 2000), (double) (yLocation * 77), (double) Game1.stats.DirtHoed);
    string str = this.HandleTreasureTileProperty(xLocation, yLocation, detectOnly);
    if (str != null)
      return str;
    bool flag = who?.CurrentTool is Hoe && who.CurrentTool.hasEnchantmentOfType<GenerousEnchantment>();
    float num = 0.5f;
    if (!this.isFarm.Value && this.isOutdoors.Value && this.GetSeason() == Season.Winter && daySaveRandom.NextDouble() < 0.08 && !explosion && !detectOnly && !(this is Desert))
    {
      Game1.createObjectDebris(daySaveRandom.Choose<string>("(O)412", "(O)416"), xLocation, yLocation);
      if (flag && daySaveRandom.NextDouble() < (double) num)
        Game1.createObjectDebris(daySaveRandom.Choose<string>("(O)412", "(O)416"), xLocation, yLocation);
      return "";
    }
    LocationData data = this.GetData();
    if (!this.isOutdoors.Value || !daySaveRandom.NextBool(data != null ? data.ChanceForClay : 0.03) || explosion)
      return "";
    if (detectOnly)
    {
      this.map.RequireLayer("Back").Tiles[xLocation, yLocation].Properties.Add("Treasure", (PropertyValue) "Item (O)330");
      return "Item";
    }
    Game1.createObjectDebris("(O)330", xLocation, yLocation);
    if (flag && daySaveRandom.NextDouble() < (double) num)
      Game1.createObjectDebris("(O)330", xLocation, yLocation);
    return "";
  }

  private string HandleTreasureTileProperty(int xLocation, int yLocation, bool detectOnly)
  {
    string str1 = this.doesTileHaveProperty(xLocation, yLocation, "Treasure", "Back");
    if (str1 == null)
      return (string) null;
    string[] array = ArgUtility.SplitBySpace(str1);
    string str2;
    string error;
    if (!ArgUtility.TryGet(array, 0, out str2, out error, name: "string type"))
    {
      LogError(str1, error);
      return (string) null;
    }
    if (detectOnly)
      return str2;
    if (str2 != null)
    {
      switch (str2.Length)
      {
        case 4:
          switch (str2[2])
          {
            case 'a':
              if (str2 == "Coal")
                break;
              goto label_36;
            case 'c':
              if (str2 == "Arch")
              {
                string id;
                if (ArgUtility.TryGet(array, 1, out id, out error, name: "string itemId"))
                {
                  Game1.createObjectDebris(id, xLocation, yLocation);
                  goto label_37;
                }
                LogError(str1, error);
                goto label_37;
              }
              goto label_36;
            case 'e':
              if (str2 == "Item")
              {
                string itemId;
                if (ArgUtility.TryGet(array, 1, out itemId, out error, name: "string itemId"))
                {
                  Item obj = ItemRegistry.Create(itemId);
                  Game1.createItemDebris(obj, new Vector2((float) xLocation, (float) yLocation), -1, this);
                  if (obj.QualifiedItemId == "(O)78")
                  {
                    ++Game1.stats.CaveCarrotsFound;
                    goto label_37;
                  }
                  goto label_37;
                }
                LogError(str1, error);
                goto label_37;
              }
              goto label_36;
            case 'l':
              if (str2 == "Gold")
                break;
              goto label_36;
            case 'o':
              if (str2 == "Iron")
                break;
              goto label_36;
            default:
              goto label_36;
          }
          break;
        case 5:
          if (str2 == "Coins")
          {
            Game1.createObjectDebris("(O)330", xLocation, yLocation);
            goto label_37;
          }
          goto label_36;
        case 6:
          switch (str2[0])
          {
            case 'C':
              if (str2 == "Copper")
                break;
              goto label_36;
            case 'O':
              if (str2 == "Object")
              {
                string id;
                if (ArgUtility.TryGet(array, 1, out id, out error, name: "string itemId"))
                {
                  Game1.createObjectDebris(id, xLocation, yLocation);
                  if (id == "78" || id == "(O)79")
                  {
                    ++Game1.stats.CaveCarrotsFound;
                    goto label_37;
                  }
                  goto label_37;
                }
                LogError(str1, error);
                goto label_37;
              }
              goto label_36;
            default:
              goto label_36;
          }
          break;
        case 7:
          if (str2 == "Iridium")
            break;
          goto label_36;
        case 10:
          if (str2 == "CaveCarrot")
          {
            Game1.createObjectDebris("(O)78", xLocation, yLocation);
            goto label_37;
          }
          goto label_36;
        default:
          goto label_36;
      }
      int debrisType = str2 == "Coal" ? 4 : (str2 == "Copper" ? 0 : (str2 == "Gold" ? 6 : (str2 == "Iridium" ? 10 : 2)));
      int numberOfChunks;
      if (ArgUtility.TryGetInt(array, 1, out numberOfChunks, out error, "int itemId"))
      {
        Game1.createDebris(debrisType, xLocation, yLocation, numberOfChunks);
        goto label_37;
      }
      LogError(str1, error);
      goto label_37;
    }
label_36:
    str2 = (string) null;
    LogError(str1, $"invalid treasure type '{str2}'");
label_37:
    this.map.RequireLayer("Back").Tiles[xLocation, yLocation].Properties["Treasure"] = (PropertyValue) null;
    return str2;

    void LogError(string value, string errorPhrase)
    {
      this.LogTilePropertyError("Treasure", "Back", xLocation, yLocation, value, errorPhrase);
    }
  }

  public virtual bool AllowMapModificationsInResetState() => false;

  /// <summary>Remove a tile from the location's map.</summary>
  /// <param name="tileX">The X tile position to set.</param>
  /// <param name="tileY">The Y tile position to set.</param>
  /// <param name="layer">The layer whose tile to set.</param>
  public void removeMapTile(int tileX, int tileY, string layer)
  {
    Map map = this.map;
    Layer layer1 = map != null ? map.RequireLayer(layer) : (Layer) null;
    if (layer1?.Tiles[tileX, tileY] == null)
      return;
    layer1.Tiles[tileX, tileY] = (Tile) null;
  }

  /// <summary>Change the tile at a given map position to use the given tilesheet and tile index, recreating it if needed.</summary>
  /// <param name="tileX">The X tile position to set.</param>
  /// <param name="tileY">The Y tile position to set.</param>
  /// <param name="index">The tile index in the tilesheet to show.</param>
  /// <param name="layer">The layer whose tile to set.</param>
  /// <param name="tileSheetId">The tilesheet ID from which to get the <paramref name="index" />.</param>
  /// <param name="action">The <c>Action</c> tile property to set, or <c>null</c> for none. This is ignored if <paramref name="layer" /> is not <c>Buildings</c>.</param>
  /// <param name="copyProperties">If the tile is recreated, whether to copy any tile properties that were on the previous tile.</param>
  /// <returns>Returns the new or updated tile at the tile position.</returns>
  public StaticTile setMapTile(
    int tileX,
    int tileY,
    int index,
    string layer,
    string tileSheetId,
    string action = null,
    bool copyProperties = true)
  {
    Layer layer1 = this.map.RequireLayer(layer);
    Tile tile = layer1.Tiles[tileX, tileY];
    if (tile is StaticTile staticTile && staticTile.TileSheet.Id == tileSheetId)
    {
      staticTile.TileIndex = index;
    }
    else
    {
      layer1.Tiles[tileX, tileY] = (Tile) (staticTile = new StaticTile(layer1, this.map.RequireTileSheet(tileSheetId), BlendMode.Alpha, index));
      if (copyProperties && tile != null)
      {
        foreach (KeyValuePair<string, PropertyValue> property in (IEnumerable<KeyValuePair<string, PropertyValue>>) tile.Properties)
          staticTile.Properties[property.Key] = property.Value;
      }
    }
    if (action != null && layer == "Buildings")
      staticTile.Properties["Action"] = (PropertyValue) action;
    return staticTile;
  }

  /// <summary>Replace a map tile with an animated tile.</summary>
  /// <param name="tileX">The X tile position to set.</param>
  /// <param name="tileY">The Y tile position to set.</param>
  /// <param name="animationTileIndexes">The tile index in the tilesheet to show for each frame in the animation.</param>
  /// <param name="interval">The number of milliseconds for which to show each frame of the animation.</param>
  /// <param name="layer">The layer whose tile to set.</param>
  /// <param name="tileSheetId">The tilesheet ID from which to get the <paramref name="animationTileIndexes" />.</param>
  /// <param name="action">The <c>Action</c> tile property to set, or <c>null</c> for none. This is ignored if <paramref name="layer" /> is not <c>Buildings</c>.</param>
  /// <param name="copyProperties">Whether to copy any tile properties that were on the previous tile.</param>
  /// <returns>Returns the new tile at the tile position.</returns>
  public AnimatedTile setAnimatedMapTile(
    int tileX,
    int tileY,
    int[] animationTileIndexes,
    long interval,
    string layer,
    string tileSheetId,
    string action = null,
    bool copyProperties = true)
  {
    Layer layer1 = this.map.RequireLayer(layer);
    TileSheet tileSheet = this.map.RequireTileSheet(tileSheetId);
    StaticTile[] tileFrames = new StaticTile[animationTileIndexes.Length];
    for (int index = 0; index < animationTileIndexes.Length; ++index)
      tileFrames[index] = new StaticTile(layer1, tileSheet, BlendMode.Alpha, animationTileIndexes[index]);
    AnimatedTile animatedTile = new AnimatedTile(layer1, tileFrames, interval);
    if (copyProperties)
    {
      Tile tile = layer1.Tiles[tileX, tileY];
      if (tile != null)
      {
        foreach (KeyValuePair<string, PropertyValue> property in (IEnumerable<KeyValuePair<string, PropertyValue>>) tile.Properties)
          animatedTile.Properties[property.Key] = property.Value;
      }
    }
    if (action != null && layer == "Buildings")
      animatedTile.Properties["Action"] = (PropertyValue) action;
    layer1.Tiles[tileX, tileY] = (Tile) animatedTile;
    return animatedTile;
  }

  /// <summary>Move all objects, furniture, terrain features, and large terrain features within the location.</summary>
  /// <param name="dx">The X tile offset to apply.</param>
  /// <param name="dy">The Y tile offset to apply.</param>
  /// <param name="where">If set, a filter which indicates whether something should be moved.</param>
  public virtual void shiftContents(int dx, int dy, Func<Vector2, object, bool> where = null)
  {
    Vector2 vector2_1 = new Vector2((float) dx, (float) dy);
    List<KeyValuePair<Vector2, Object>> keyValuePairList1 = new List<KeyValuePair<Vector2, Object>>(this.objects.Pairs);
    this.objects.Clear();
    foreach (KeyValuePair<Vector2, Object> keyValuePair in keyValuePairList1)
    {
      if ((where != null ? (where(keyValuePair.Key, (object) keyValuePair.Value) ? 1 : 0) : 1) != 0)
      {
        this.removeLightSource(keyValuePair.Value.lightSource?.Id);
        Vector2 vector2_2 = keyValuePair.Key + vector2_1;
        this.objects.Add(vector2_2, keyValuePair.Value);
        keyValuePair.Value.initializeLightSource(vector2_2);
      }
      else
        this.objects.Add(keyValuePair.Key, keyValuePair.Value);
    }
    List<KeyValuePair<Vector2, TerrainFeature>> keyValuePairList2 = new List<KeyValuePair<Vector2, TerrainFeature>>((IEnumerable<KeyValuePair<Vector2, TerrainFeature>>) this.terrainFeatures.Pairs);
    this.terrainFeatures.Clear();
    foreach (KeyValuePair<Vector2, TerrainFeature> keyValuePair in keyValuePairList2)
      this.terrainFeatures.Add((where != null ? (where(keyValuePair.Key, (object) keyValuePair.Value) ? 1 : 0) : 1) != 0 ? keyValuePair.Key + vector2_1 : keyValuePair.Key, keyValuePair.Value);
    foreach (LargeTerrainFeature largeTerrainFeature1 in this.largeTerrainFeatures)
    {
      if ((where != null ? (where(largeTerrainFeature1.Tile, (object) largeTerrainFeature1) ? 1 : 0) : 1) != 0)
      {
        LargeTerrainFeature largeTerrainFeature2 = largeTerrainFeature1;
        largeTerrainFeature2.Tile = largeTerrainFeature2.Tile + vector2_1;
      }
    }
    foreach (Furniture furniture in this.furniture)
    {
      if ((where != null ? (where(furniture.TileLocation, (object) furniture) ? 1 : 0) : 1) != 0)
      {
        furniture.removeLights();
        furniture.TileLocation = new Vector2(furniture.TileLocation.X + (float) dx, furniture.TileLocation.Y + (float) dy);
        furniture.updateDrawPosition();
        if (Game1.isDarkOut(this))
          furniture.addLights();
      }
    }
  }

  public void moveFurniture(int oldX, int oldY, int newX, int newY)
  {
    Vector2 key = new Vector2((float) oldX, (float) oldY);
    foreach (Furniture furniture in this.furniture)
    {
      if (furniture.tileLocation.Equals((object) key))
      {
        furniture.removeLights();
        furniture.TileLocation = new Vector2((float) newX, (float) newY);
        if (!Game1.isDarkOut(this))
          return;
        furniture.addLights();
        return;
      }
    }
    Object @object;
    if (!this.objects.TryGetValue(key, out @object))
      return;
    this.objects.Remove(key);
    this.objects.Add(new Vector2((float) newX, (float) newY), @object);
  }

  /// <summary>Get whether a tile exists at the given coordinate.</summary>
  /// <param name="x">The tile X coordinate.</param>
  /// <param name="y">The tile Y coordinate.</param>
  /// <param name="layer">The layer whose tiles to check.</param>
  /// <param name="tilesheetId">The tilesheet ID to check, or <c>null</c> for any tilesheet. If the tile doesn't use this tilesheet, it'll be ignored.</param>
  public bool hasTileAt(int x, int y, string layer, string tilesheetId = null)
  {
    Map map = this.map;
    return map != null && map.HasTileAt(x, y, layer, tilesheetId);
  }

  /// <summary>Get whether a tile exists at the given coordinate.</summary>
  /// <param name="tile">The tile coordinate.</param>
  /// <param name="layer">The layer whose tiles to check.</param>
  /// <param name="tilesheetId">The tilesheet ID to check, or <c>null</c> for any tilesheet. If the tile doesn't use this tilesheet, it'll be ignored.</param>
  public bool hasTileAt(Location tile, string layer, string tilesheetId = null)
  {
    Map map = this.map;
    return map != null && map.HasTileAt(tile.X, tile.Y, layer, tilesheetId);
  }

  /// <summary>Get whether a tile exists at the given coordinate.</summary>
  /// <param name="tile">The tile coordinate.</param>
  /// <param name="layer">The layer whose tiles to check.</param>
  /// <param name="tilesheetId">The tilesheet ID to check, or <c>null</c> for any tilesheet. If the tile doesn't use this tilesheet, it'll be ignored.</param>
  public bool hasTileAt(Point tile, string layer, string tilesheetId = null)
  {
    Map map = this.map;
    return map != null && map.HasTileAt(tile.X, tile.Y, layer, tilesheetId);
  }

  /// <summary>Get the tile index at the given map coordinate.</summary>
  /// <param name="p">The tile coordinate.</param>
  /// <param name="layer">The layer whose tiles to check.</param>
  /// <param name="tilesheetId">The tilesheet ID for which to get a tile index, or <c>null</c> for any tilesheet. If the tile doesn't use this tilesheet, it'll be ignored.</param>
  /// <returns>Returns the matching tile's index, or <c>-1</c> if no tile was found.</returns>
  public int getTileIndexAt(Location p, string layer, string tilesheetId = null)
  {
    Map map = this.map;
    return map == null ? -1 : map.GetTileIndexAt(p.X, p.Y, layer, tilesheetId);
  }

  /// <summary>Get the tile index at the given map coordinate.</summary>
  /// <param name="p">The tile coordinate.</param>
  /// <param name="layer">The layer whose tiles to check.</param>
  /// <param name="tilesheetId">The tilesheet ID for which to get a tile index, or <c>null</c> for any tilesheet. If the tile doesn't use this tilesheet, it'll be ignored.</param>
  /// <returns>Returns the matching tile's index, or <c>-1</c> if no tile was found.</returns>
  public int getTileIndexAt(Point p, string layer, string tilesheetId = null)
  {
    Map map = this.map;
    return map == null ? -1 : map.GetTileIndexAt(p.X, p.Y, layer, tilesheetId);
  }

  /// <summary>Get the tile index at the given layer coordinate.</summary>
  /// <param name="x">The tile X coordinate.</param>
  /// <param name="y">The tile Y coordinate.</param>
  /// <param name="layer">The layer whose tiles to check.</param>
  /// <param name="tilesheetId">The tilesheet ID for which to get a tile index, or <c>null</c> for any tilesheet. If the tile doesn't use this tilesheet, it'll be ignored.</param>
  /// <returns>Returns the matching tile's index, or <c>-1</c> if no tile was found.</returns>
  public int getTileIndexAt(int x, int y, string layer, string tilesheetId = null)
  {
    Map map = this.map;
    return map == null ? -1 : map.GetTileIndexAt(x, y, layer, tilesheetId);
  }

  public string getTileSheetIDAt(int x, int y, string layer)
  {
    return this.map.GetLayer(layer)?.Tiles[x, y]?.TileSheet.Id ?? "";
  }

  /// <summary>Handle a building in this location being constructed by any player.</summary>
  /// <param name="building">The building that was constructed.</param>
  /// <param name="who">The player that constructed the building.</param>
  public virtual void OnBuildingConstructed(Building building, Farmer who)
  {
    building.performActionOnConstruction(this, who);
  }

  /// <summary>Handle a building in this location being moved by any player.</summary>
  /// <param name="building">The building that was moved.</param>
  public virtual void OnBuildingMoved(Building building)
  {
    building.performActionOnBuildingPlacement();
  }

  /// <summary>Handle a building in this location being demolished by the current player.</summary>
  /// <param name="building">The building type that was demolished.</param>
  /// <param name="id">The unique building ID.</param>
  public virtual void OnBuildingDemolished(string type, Guid id)
  {
    if (!(type == "Stable"))
      return;
    Horse mount = Game1.player.mount;
    if ((mount != null ? (mount.HorseId == id ? 1 : 0) : 0) == 0)
      return;
    Game1.player.mount.dismount(true);
  }

  /// <summary>Handle the new day starting after the player saves, loads, or connects.</summary>
  /// <remarks>See also <see cref="M:StardewValley.GameLocation.DayUpdate(System.Int32)" />, which happens while setting up the day before saving.</remarks>
  public virtual void OnDayStarted()
  {
  }

  /// <summary>Handle a breakable mine stone being destroyed.</summary>
  /// <param name="stoneId">The unqualified item ID for the stone object.</param>
  /// <param name="x">The stone's X tile position.</param>
  /// <param name="y">The stone's Y tile position.</param>
  /// <param name="who">The player who broke the stone.</param>
  /// <remarks>This is the entry point for creating item drops when breaking stone.</remarks>
  public void OnStoneDestroyed(string stoneId, int x, int y, Farmer who)
  {
    long uniqueMultiplayerId = who != null ? who.UniqueMultiplayerID : 0L;
    if (who?.currentLocation is MineShaft currentLocation && currentLocation.mineLevel > 120 && !currentLocation.isSideBranch())
    {
      int num = currentLocation.mineLevel - 121;
      if (Utility.GetDayOfPassiveFestival("DesertFestival") > 0)
      {
        float chance = 0.01f + (float) num * 0.0005f;
        if ((double) chance > 0.5)
          chance = 0.5f;
        if (Game1.random.NextBool(chance))
          Game1.createMultipleObjectDebris("CalicoEgg", x, y, Game1.random.Next(1, 4), who.UniqueMultiplayerID, this);
      }
    }
    if (who != null && Game1.random.NextDouble() <= 0.02 && Game1.player.team.SpecialOrderRuleActive("DROP_QI_BEANS"))
      Game1.createMultipleObjectDebris("(O)890", x, y, 1, who.UniqueMultiplayerID, this);
    if (!MineShaft.IsGeneratedLevel(this))
    {
      if (stoneId == "343" || stoneId == "450")
      {
        Random daySaveRandom = Utility.CreateDaySaveRandom((double) (x * 2000), (double) y);
        double num1 = who == null || !who.hasBuff("dwarfStatue_4") ? 1.0 : 1.25;
        if (daySaveRandom.NextDouble() < 0.035 * num1 && Game1.stats.DaysPlayed > 1U)
          Game1.createObjectDebris("(O)" + (535 + (Game1.stats.DaysPlayed <= 60U || daySaveRandom.NextDouble() >= 0.2 ? (Game1.stats.DaysPlayed <= 120U || daySaveRandom.NextDouble() >= 0.2 ? 0 : 2) : 1)).ToString(), x, y, uniqueMultiplayerId, this);
        int num2 = who == null || !who.professions.Contains(21) ? 1 : 2;
        double num3 = who == null || !who.hasBuff("dwarfStatue_2") ? 0.0 : 0.03;
        if (daySaveRandom.NextDouble() < 0.035 * (double) num2 + num3 && Game1.stats.DaysPlayed > 1U)
          Game1.createObjectDebris("(O)382", x, y, uniqueMultiplayerId, this);
        if (daySaveRandom.NextDouble() < 0.01 && Game1.stats.DaysPlayed > 1U)
          Game1.createObjectDebris("(O)390", x, y, uniqueMultiplayerId, this);
      }
      this.breakStone(stoneId, x, y, who, Utility.CreateDaySaveRandom((double) (x * 4000), (double) y));
    }
    else
      (this as MineShaft).checkStoneForItems(stoneId, x, y, who);
  }

  protected virtual bool breakStone(string stoneId, int x, int y, Farmer who, Random r)
  {
    int howMuch = 0;
    int num1 = who == null || !who.professions.Contains(18) ? 0 : 1;
    if (who != null && who.hasBuff("dwarfStatue_0"))
      ++num1;
    string str1 = stoneId;
    int num2 = 44;
    string str2 = num2.ToString();
    if (str1 == str2)
    {
      num2 = r.Next(1, 8) * 2;
      stoneId = num2.ToString();
    }
    long uniqueMultiplayerId = who != null ? who.UniqueMultiplayerID : 0L;
    int luckLevel = who != null ? who.LuckLevel : 0;
    double num3 = who != null ? who.DailyLuck : 0.0;
    int miningLevel = who != null ? who.MiningLevel : 0;
    if (stoneId != null)
    {
      num2 = stoneId.Length;
      switch (num2)
      {
        case 1:
          switch (stoneId[0])
          {
            case '2':
              Game1.createMultipleObjectDebris("(O)72", x, y, who == null || who.stats.Get(StatKeys.Mastery(3)) <= 0U ? 1 : 2, uniqueMultiplayerId, this);
              howMuch = 150;
              goto label_65;
            case '4':
              Game1.createMultipleObjectDebris("(O)64", x, y, who == null || who.stats.Get(StatKeys.Mastery(3)) <= 0U ? 1 : 2, uniqueMultiplayerId, this);
              howMuch = 80 /*0x50*/;
              goto label_65;
            case '6':
              Game1.createMultipleObjectDebris("(O)70", x, y, who == null || who.stats.Get(StatKeys.Mastery(3)) <= 0U ? 1 : 2, uniqueMultiplayerId, this);
              howMuch = 40;
              goto label_65;
            case '8':
              Game1.createMultipleObjectDebris("(O)66", x, y, who == null || who.stats.Get(StatKeys.Mastery(3)) <= 0U ? 1 : 2, uniqueMultiplayerId, this);
              howMuch = 16 /*0x10*/;
              goto label_65;
            default:
              goto label_65;
          }
        case 2:
          switch (stoneId[1])
          {
            case '0':
              if (stoneId == "10")
              {
                Game1.createMultipleObjectDebris("(O)68", x, y, who == null || who.stats.Get(StatKeys.Mastery(3)) <= 0U ? 1 : 2, uniqueMultiplayerId, this);
                howMuch = 16 /*0x10*/;
                goto label_65;
              }
              goto label_65;
            case '2':
              if (stoneId == "12")
              {
                Game1.createMultipleObjectDebris("(O)60", x, y, who == null || who.stats.Get(StatKeys.Mastery(3)) <= 0U ? 1 : 2, uniqueMultiplayerId, this);
                howMuch = 80 /*0x50*/;
                goto label_65;
              }
              goto label_65;
            case '4':
              if (stoneId == "14")
              {
                Game1.createMultipleObjectDebris("(O)62", x, y, who == null || who.stats.Get(StatKeys.Mastery(3)) <= 0U ? 1 : 2, uniqueMultiplayerId, this);
                howMuch = 40;
                goto label_65;
              }
              goto label_65;
            case '5':
              switch (stoneId)
              {
                case "95":
                  Game1.createMultipleObjectDebris("(O)909", x, y, num1 + r.Next(1, 3) + (r.NextDouble() < (double) luckLevel / 100.0 ? 1 : 0) + (r.NextDouble() < (double) miningLevel / 200.0 ? 1 : 0), uniqueMultiplayerId, this);
                  howMuch = 18;
                  goto label_65;
                case "25":
                  Game1.createMultipleObjectDebris("(O)719", x, y, r.Next(2, 5), uniqueMultiplayerId, this);
                  howMuch = 5;
                  if (this is IslandLocation && r.NextDouble() < 0.1)
                  {
                    Game1.player.team.RequestLimitedNutDrops("MusselStone", this, x * 64 /*0x40*/, y * 64 /*0x40*/, 5);
                    goto label_65;
                  }
                  goto label_65;
                case "75":
                  Game1.createObjectDebris("(O)535", x, y, uniqueMultiplayerId, this);
                  howMuch = 8;
                  goto label_65;
                default:
                  goto label_65;
              }
            case '6':
              if (stoneId == "76")
              {
                Game1.createObjectDebris("(O)536", x, y, uniqueMultiplayerId, this);
                howMuch = 16 /*0x10*/;
                goto label_65;
              }
              goto label_65;
            case '7':
              if (stoneId == "77")
              {
                Game1.createObjectDebris("(O)537", x, y, uniqueMultiplayerId, this);
                howMuch = 32 /*0x20*/;
                goto label_65;
              }
              goto label_65;
            default:
              goto label_65;
          }
        case 3:
          switch (stoneId[2])
          {
            case '0':
              switch (stoneId)
              {
                case "670":
                  goto label_55;
                case "850":
                case "290":
                  Game1.createMultipleObjectDebris("(O)380", x, y, num1 + r.Next(1, 4) + (r.NextDouble() < (double) luckLevel / 100.0 ? 1 : 0) + (r.NextDouble() < (double) miningLevel / 100.0 ? 1 : 0), uniqueMultiplayerId, this);
                  howMuch = 12;
                  Game1.multiplayer.broadcastSprites(this, Utility.sparkleWithinArea(new Microsoft.Xna.Framework.Rectangle(x * 64 /*0x40*/, (y - 1) * 64 /*0x40*/, 32 /*0x20*/, 96 /*0x60*/), 3, Color.White * 0.5f, 175, 100));
                  goto label_65;
                default:
                  goto label_65;
              }
            case '1':
              if (stoneId == "751")
                goto label_57;
              goto label_65;
            case '3':
              if (stoneId == "843")
                break;
              goto label_65;
            case '4':
              switch (stoneId)
              {
                case "844":
                  break;
                case "764":
                  goto label_60;
                default:
                  goto label_65;
              }
              break;
            case '5':
              switch (stoneId)
              {
                case "845":
                  goto label_55;
                case "765":
                  Game1.createMultipleObjectDebris("(O)386", x, y, num1 + r.Next(1, 4) + (r.NextDouble() < (double) luckLevel / 100.0 ? 1 : 0) + (r.NextDouble() < (double) miningLevel / 100.0 ? 1 : 0), uniqueMultiplayerId, this);
                  Game1.multiplayer.broadcastSprites(this, Utility.sparkleWithinArea(new Microsoft.Xna.Framework.Rectangle(x * 64 /*0x40*/, (y - 1) * 64 /*0x40*/, 32 /*0x20*/, 96 /*0x60*/), 6, Color.BlueViolet * 0.5f, 175, 100));
                  if (r.NextDouble() < 0.035)
                    Game1.createMultipleObjectDebris("(O)74", x, y, 1, uniqueMultiplayerId, this);
                  howMuch = 50;
                  goto label_65;
                default:
                  goto label_65;
              }
            case '6':
              switch (stoneId)
              {
                case "816":
                  goto label_39;
                case "846":
                  goto label_55;
                default:
                  goto label_65;
              }
            case '7':
              switch (stoneId)
              {
                case "817":
                  goto label_39;
                case "847":
                  goto label_55;
                default:
                  goto label_65;
              }
            case '8':
              switch (stoneId)
              {
                case "818":
                  Game1.createMultipleObjectDebris("(O)330", x, y, num1 + r.Next(1, 3) + (r.NextDouble() < (double) luckLevel / 100.0 ? 1 : 0) + (r.NextDouble() < (double) miningLevel / 100.0 ? 1 : 0), uniqueMultiplayerId, this);
                  howMuch = 6;
                  goto label_65;
                case "668":
                  goto label_55;
                default:
                  goto label_65;
              }
            case '9':
              switch (stoneId)
              {
                case "819":
                  Game1.createObjectDebris("(O)749", x, y, uniqueMultiplayerId, this);
                  howMuch = 64 /*0x40*/;
                  goto label_65;
                case "849":
                  goto label_57;
                default:
                  goto label_65;
              }
            default:
              goto label_65;
          }
          Game1.createMultipleObjectDebris("(O)848", x, y, num1 + r.Next(1, 3) + (r.NextDouble() < (double) luckLevel / 100.0 ? 1 : 0) + (r.NextDouble() < (double) miningLevel / 200.0 ? 1 : 0), uniqueMultiplayerId, this);
          howMuch = 12;
          goto label_65;
label_39:
          if (r.NextDouble() < 0.1)
            Game1.createObjectDebris("(O)823", x, y, uniqueMultiplayerId, this);
          else if (r.NextDouble() < 0.015)
            Game1.createObjectDebris("(O)824", x, y, uniqueMultiplayerId, this);
          else if (r.NextDouble() < 0.1)
          {
            num2 = 579 + r.Next(11);
            Game1.createObjectDebris("(O)" + num2.ToString(), x, y, uniqueMultiplayerId, this);
          }
          Game1.createMultipleObjectDebris("(O)881", x, y, num1 + r.Next(1, 3) + (r.NextDouble() < (double) luckLevel / 100.0 ? 1 : 0) + (r.NextDouble() < (double) miningLevel / 100.0 ? 1 : 0), uniqueMultiplayerId, this);
          howMuch = 6;
          goto label_65;
label_55:
          Game1.createMultipleObjectDebris("(O)390", x, y, num1 + r.Next(1, 3) + (r.NextDouble() < (double) luckLevel / 100.0 ? 1 : 0) + (r.NextDouble() < (double) miningLevel / 100.0 ? 1 : 0), uniqueMultiplayerId, this);
          howMuch = 3;
          if (r.NextDouble() < 0.08)
          {
            Game1.createMultipleObjectDebris("(O)382", x, y, 1 + num1, uniqueMultiplayerId, this);
            howMuch = 4;
            goto label_65;
          }
          goto label_65;
label_57:
          Game1.createMultipleObjectDebris("(O)378", x, y, num1 + r.Next(1, 4) + (r.NextDouble() < (double) luckLevel / 100.0 ? 1 : 0) + (r.NextDouble() < (double) miningLevel / 100.0 ? 1 : 0), uniqueMultiplayerId, this);
          howMuch = 5;
          Game1.multiplayer.broadcastSprites(this, Utility.sparkleWithinArea(new Microsoft.Xna.Framework.Rectangle(x * 64 /*0x40*/, (y - 1) * 64 /*0x40*/, 32 /*0x20*/, 96 /*0x60*/), 3, Color.Orange * 0.5f, 175, 100));
          goto label_65;
        case 14:
          switch (stoneId[13])
          {
            case '0':
              if (stoneId == "BasicCoalNode0")
                break;
              goto label_65;
            case '1':
              if (stoneId == "BasicCoalNode1")
                break;
              goto label_65;
            default:
              goto label_65;
          }
          break;
        case 15:
          if (stoneId == "VolcanoGoldNode")
            goto label_60;
          goto label_65;
        case 16 /*0x10*/:
          switch (stoneId[15])
          {
            case '0':
              switch (stoneId)
              {
                case "VolcanoCoalNode0":
                  goto label_59;
                case "CalicoEggStone_0":
                  break;
                default:
                  goto label_65;
              }
              break;
            case '1':
              switch (stoneId)
              {
                case "VolcanoCoalNode1":
                  goto label_59;
                case "CalicoEggStone_1":
                  break;
                default:
                  goto label_65;
              }
              break;
            case '2':
              if (stoneId == "CalicoEggStone_2")
                break;
              goto label_65;
            default:
              goto label_65;
          }
          Game1.createMultipleObjectDebris("CalicoEgg", x, y, r.Next(1, 4) + (r.NextBool((float) luckLevel / 100f) ? 1 : 0) + (r.NextBool((float) miningLevel / 100f) ? 1 : 0), uniqueMultiplayerId, this);
          howMuch = 50;
          Game1.multiplayer.broadcastSprites(this, Utility.sparkleWithinArea(new Microsoft.Xna.Framework.Rectangle(x * 64 /*0x40*/, (y - 1) * 64 /*0x40*/, 32 /*0x20*/, 96 /*0x60*/), 6, new Color((int) byte.MaxValue, 120, 0) * 0.5f, 175, 100));
          goto label_65;
        default:
          goto label_65;
      }
label_59:
      Game1.createMultipleObjectDebris("(O)382", x, y, num1 + r.Next(1, 4) + (r.NextDouble() < (double) luckLevel / 100.0 ? 1 : 0) + (r.NextDouble() < (double) miningLevel / 100.0 ? 1 : 0), uniqueMultiplayerId, this);
      howMuch = 10;
      Game1.multiplayer.broadcastSprites(this, Utility.sparkleWithinArea(new Microsoft.Xna.Framework.Rectangle(x * 64 /*0x40*/, (y - 1) * 64 /*0x40*/, 32 /*0x20*/, 96 /*0x60*/), 3, Color.Black * 0.5f, 175, 100));
      goto label_65;
label_60:
      Game1.createMultipleObjectDebris("(O)384", x, y, num1 + r.Next(1, 4) + (r.NextDouble() < (double) luckLevel / 100.0 ? 1 : 0) + (r.NextDouble() < (double) miningLevel / 100.0 ? 1 : 0), uniqueMultiplayerId, this);
      howMuch = 18;
      Game1.multiplayer.broadcastSprites(this, Utility.sparkleWithinArea(new Microsoft.Xna.Framework.Rectangle(x * 64 /*0x40*/, (y - 1) * 64 /*0x40*/, 32 /*0x20*/, 96 /*0x60*/), 3, Color.Yellow * 0.5f, 175, 100));
    }
label_65:
    if (who != null && who.professions.Contains(19) && r.NextBool())
    {
      int number = who.stats.Get(StatKeys.Mastery(3)) > 0U ? 2 : 1;
      if (stoneId != null)
      {
        num2 = stoneId.Length;
        switch (num2)
        {
          case 1:
            switch (stoneId[0])
            {
              case '2':
                Game1.createMultipleObjectDebris("(O)72", x, y, number, who.UniqueMultiplayerID, this);
                howMuch = 100;
                break;
              case '4':
                Game1.createMultipleObjectDebris("(O)64", x, y, number, who.UniqueMultiplayerID, this);
                howMuch = 50;
                break;
              case '6':
                Game1.createMultipleObjectDebris("(O)70", x, y, number, who.UniqueMultiplayerID, this);
                howMuch = 20;
                break;
              case '8':
                Game1.createMultipleObjectDebris("(O)66", x, y, number, who.UniqueMultiplayerID, this);
                howMuch = 8;
                break;
            }
            break;
          case 2:
            switch (stoneId[1])
            {
              case '0':
                if (stoneId == "10")
                {
                  Game1.createMultipleObjectDebris("(O)68", x, y, number, who.UniqueMultiplayerID, this);
                  howMuch = 8;
                  break;
                }
                break;
              case '2':
                if (stoneId == "12")
                {
                  Game1.createMultipleObjectDebris("(O)60", x, y, number, who.UniqueMultiplayerID, this);
                  howMuch = 50;
                  break;
                }
                break;
              case '4':
                if (stoneId == "14")
                {
                  Game1.createMultipleObjectDebris("(O)62", x, y, number, who.UniqueMultiplayerID, this);
                  howMuch = 20;
                  break;
                }
                break;
            }
            break;
        }
      }
    }
    string str3 = stoneId;
    num2 = 46;
    string str4 = num2.ToString();
    if (str3 == str4)
    {
      Game1.createDebris(10, x, y, r.Next(1, 4), this);
      Game1.createDebris(6, x, y, r.Next(1, 5), this);
      if (r.NextDouble() < 0.25)
        Game1.createMultipleObjectDebris("(O)74", x, y, 1, uniqueMultiplayerId, this);
      howMuch = 150;
      ++Game1.stats.MysticStonesCrushed;
    }
    if ((this.isOutdoors.Value || this.treatAsOutdoors.Value) && howMuch == 0)
    {
      double num4 = num3 / 2.0 + (double) miningLevel * 0.005 + (double) luckLevel * 0.001;
      Random daySaveRandom = Utility.CreateDaySaveRandom((double) (x * 1000), (double) y);
      Game1.createDebris(14, x, y, 1, this);
      if (who != null)
      {
        who.gainExperience(3, 1);
        double num5 = 0.0;
        if (who.professions.Contains(21))
          num5 += 0.05 * (1.0 + num4);
        if (who.hasBuff("dwarfStatue_2"))
          num5 += 0.025;
        if (daySaveRandom.NextDouble() < num5)
          Game1.createObjectDebris("(O)382", x, y, who.UniqueMultiplayerID, this);
      }
      if (daySaveRandom.NextDouble() < 0.05 * (1.0 + num4))
      {
        Game1.createObjectDebris("(O)382", x, y, uniqueMultiplayerId, this);
        Game1.multiplayer.broadcastSprites(this, new TemporaryAnimatedSprite(25, new Vector2((float) (64 /*0x40*/ * x), (float) (64 /*0x40*/ * y)), Color.White, flipped: Game1.random.NextBool(), animationInterval: 80f, sourceRectHeight: 128 /*0x80*/));
        who?.gainExperience(3, 5);
      }
    }
    if (who != null && this.HasUnlockedAreaSecretNotes(who) && r.NextDouble() < 3.0 / 400.0)
    {
      Object unseenSecretNote = this.tryToCreateUnseenSecretNote(who);
      if (unseenSecretNote != null)
        Game1.createItemDebris((Item) unseenSecretNote, new Vector2((float) x + 0.5f, (float) y + 0.75f) * 64f, Game1.player.FacingDirection, this);
    }
    who?.gainExperience(3, howMuch);
    return howMuch > 0;
  }

  public bool isBehindBush(Vector2 Tile)
  {
    if (this.largeTerrainFeatures != null)
    {
      Microsoft.Xna.Framework.Rectangle rectangle = new Microsoft.Xna.Framework.Rectangle((int) Tile.X * 64 /*0x40*/, (int) ((double) Tile.Y + 1.0) * 64 /*0x40*/, 64 /*0x40*/, 128 /*0x80*/);
      foreach (TerrainFeature largeTerrainFeature in this.largeTerrainFeatures)
      {
        if (largeTerrainFeature.getBoundingBox().Intersects(rectangle))
          return true;
      }
    }
    return false;
  }

  public bool isBehindTree(Vector2 Tile)
  {
    if (this.terrainFeatures != null)
    {
      Microsoft.Xna.Framework.Rectangle rectangle = new Microsoft.Xna.Framework.Rectangle((int) ((double) Tile.X - 1.0) * 64 /*0x40*/, (int) Tile.Y * 64 /*0x40*/, 192 /*0xC0*/, 256 /*0x0100*/);
      foreach (KeyValuePair<Vector2, TerrainFeature> pair in this.terrainFeatures.Pairs)
      {
        if (pair.Value is Tree && pair.Value.getBoundingBox().Intersects(rectangle))
          return true;
      }
    }
    return false;
  }

  public virtual void spawnObjects()
  {
    Random daySaveRandom = Utility.CreateDaySaveRandom();
    LocationData data = this.GetData();
    if (data != null && this.numberOfSpawnedObjectsOnMap < data.MaxSpawnedForageAtOnce)
    {
      Season season1 = this.GetSeason();
      List<SpawnForageData> spawnForageDataList = new List<SpawnForageData>();
      foreach (SpawnForageData spawnForageData in GameLocation.GetData("Default").Forage.Concat<SpawnForageData>((IEnumerable<SpawnForageData>) data.Forage))
      {
        if (spawnForageData.Condition == null || GameStateQuery.CheckConditions(spawnForageData.Condition, this, random: daySaveRandom))
        {
          Season? season2 = spawnForageData.Season;
          if (season2.HasValue)
          {
            season2 = spawnForageData.Season;
            Season season3 = season1;
            if (!(season2.GetValueOrDefault() == season3 & season2.HasValue))
              continue;
          }
          spawnForageDataList.Add(spawnForageData);
        }
      }
      if (spawnForageDataList.Any<SpawnForageData>())
      {
        int num1 = Math.Min(daySaveRandom.Next(data.MinDailyForageSpawn, data.MaxDailyForageSpawn + 1), data.MaxSpawnedForageAtOnce - this.numberOfSpawnedObjectsOnMap);
        ItemQueryContext context = new ItemQueryContext(this, (Farmer) null, daySaveRandom, $"location '{this.NameOrUniqueName}' > forage");
        for (int index1 = 0; index1 < num1; ++index1)
        {
          for (int index2 = 0; index2 < 11; ++index2)
          {
            int num2 = daySaveRandom.Next(this.map.DisplayWidth / 64 /*0x40*/);
            int num3 = daySaveRandom.Next(this.map.DisplayHeight / 64 /*0x40*/);
            Vector2 vector2 = new Vector2((float) num2, (float) num3);
            if ((this.objects.ContainsKey(vector2) || this.IsNoSpawnTile(vector2) || this.doesTileHaveProperty(num2, num3, "Spawnable", "Back") == null || this.doesEitherTileOrTileIndexPropertyEqual(num2, num3, "Spawnable", "Back", "F") || !this.CanItemBePlacedHere(vector2) || this.hasTileAt(num2, num3, "AlwaysFront") || this.hasTileAt(num2, num3, "AlwaysFront2") || this.hasTileAt(num2, num3, "AlwaysFront3") || this.hasTileAt(num2, num3, "Front") || this.isBehindBush(vector2) ? 0 : (daySaveRandom.NextBool(0.1) ? 1 : (!this.isBehindTree(vector2) ? 1 : 0))) != 0)
            {
              SpawnForageData forage = daySaveRandom.ChooseFrom<SpawnForageData>((IList<SpawnForageData>) spawnForageDataList);
              if (daySaveRandom.NextBool(forage.Chance))
              {
                Item obj = ItemQueryResolver.TryResolveRandomItem((ISpawnItemData) forage, context, logError: (Action<string, string>) ((query, error) => Game1.log.Error($"Location '{this.NameOrUniqueName}' failed parsing item query '{query}' for forage '{forage.Id}': {error}")));
                if (obj != null)
                {
                  if (!(obj is Object @object))
                  {
                    Game1.log.Warn($"Location '{this.Name}' ignored invalid forage data '{forage.Id}': the resulting item '{obj.QualifiedItemId}' isn't an {"Object"}-type item.");
                  }
                  else
                  {
                    @object.IsSpawnedObject = true;
                    if (this.dropObject(@object, vector2 * 64f, Game1.viewport, true))
                    {
                      ++this.numberOfSpawnedObjectsOnMap;
                      break;
                    }
                  }
                }
              }
            }
          }
        }
      }
    }
    List<Vector2> vector2List = new List<Vector2>();
    foreach (KeyValuePair<Vector2, Object> pair in this.objects.Pairs)
    {
      if (pair.Value.QualifiedItemId == "(O)590" || pair.Value.QualifiedItemId == "(O)SeedSpot")
        vector2List.Add(pair.Key);
    }
    switch (this)
    {
      case Farm _:
      case IslandWest _:
label_36:
        for (int index = vector2List.Count - 1; index >= 0; --index)
        {
          if ((!(this is IslandNorth) || (double) vector2List[index].X >= 26.0) && daySaveRandom.NextBool(0.15))
          {
            this.objects.Remove(vector2List[index]);
            vector2List.RemoveAt(index);
          }
        }
        if (vector2List.Count > (!(this is Farm) ? 1 : 0) && (this.GetSeason() != Season.Winter || vector2List.Count > 4))
          break;
        double num4 = 1.0;
        while (daySaveRandom.NextDouble() < num4)
        {
          int num5 = daySaveRandom.Next(this.map.DisplayWidth / 64 /*0x40*/);
          int num6 = daySaveRandom.Next(this.map.DisplayHeight / 64 /*0x40*/);
          Vector2 vector2 = new Vector2((float) num5, (float) num6);
          if (this.CanItemBePlacedHere(vector2) && !this.IsTileOccupiedBy(vector2) && !this.hasTileAt(num5, num6, "AlwaysFront") && !this.hasTileAt(num5, num6, "Front") && !this.isBehindBush(vector2) && (this.doesTileHaveProperty(num5, num6, "Diggable", "Back") != null || this.GetSeason() == Season.Winter && this.doesTileHaveProperty(num5, num6, "Type", "Back") != null && this.doesTileHaveProperty(num5, num6, "Type", "Back").Equals("Grass")))
          {
            if (!this.name.Equals((object) "Forest") || num5 < 93 || num6 > 22)
              this.objects.Add(vector2, ItemRegistry.Create<Object>(daySaveRandom.NextBool(0.166) ? "(O)SeedSpot" : "(O)590"));
            else
              continue;
          }
          num4 *= 0.75;
          if (this.GetSeason() == Season.Winter)
            num4 += 0.10000000149011612;
        }
        break;
      default:
        this.spawnWeedsAndStones();
        goto label_36;
    }
  }

  public void spawnWeedsAndStones(int numDebris = -1, bool weedsOnly = false, bool spawnFromOldWeeds = true)
  {
    switch (this)
    {
      case Farm _:
      case IslandWest _:
        if (Game1.IsBuildingConstructed("Gold Clock") && !Game1.netWorldState.Value.goldenClocksTurnedOff.Value)
          return;
        break;
    }
    bool flag1 = false;
    if (this is Beach || this.GetSeason() == Season.Winter || this is Desert)
      return;
    int num1 = numDebris != -1 ? numDebris : (Game1.random.NextDouble() < 0.95 ? (Game1.random.NextDouble() < 0.25 ? Game1.random.Next(10, 21) : Game1.random.Next(5, 11)) : 0);
    if (this.IsRainingHere())
      num1 *= 2;
    if (Game1.dayOfMonth == 1)
      num1 *= 5;
    if (this.objects.Length <= 0 & spawnFromOldWeeds)
      return;
    if (!(this is Farm))
      num1 /= 2;
    bool flag2 = this.IsGreenRainingHere();
    for (int index = 0; index < num1; ++index)
    {
      Vector2 vector2_1 = spawnFromOldWeeds ? new Vector2((float) Game1.random.Next(-1, 2), (float) Game1.random.Next(-1, 2)) : new Vector2((float) Game1.random.Next(this.map.Layers[0].LayerWidth), (float) Game1.random.Next(this.map.Layers[0].LayerHeight));
      if (!spawnFromOldWeeds && this is IslandWest)
        vector2_1 = new Vector2((float) Game1.random.Next(57, 97), (float) Game1.random.Next(44, 68));
      while (spawnFromOldWeeds && vector2_1.Equals(Vector2.Zero))
        vector2_1 = new Vector2((float) Game1.random.Next(-1, 2), (float) Game1.random.Next(-1, 2));
      Vector2 key = Vector2.Zero;
      Object object1 = (Object) null;
      if (spawnFromOldWeeds)
        Utility.TryGetRandom(this.objects, out key, out object1);
      Vector2 vector2_2 = spawnFromOldWeeds ? key : Vector2.Zero;
      int num2;
      switch (this)
      {
        case Mountain _ when (double) vector2_1.X + (double) vector2_2.X > 100.0:
        case IslandNorth _:
          continue;
        case Farm _:
          num2 = 1;
          break;
        default:
          num2 = this is IslandWest ? 1 : 0;
          break;
      }
      int num3 = (int) ((double) vector2_1.X + (double) vector2_2.X);
      int num4 = (int) ((double) vector2_1.Y + (double) vector2_2.Y);
      Vector2 vector2_3 = vector2_1 + vector2_2;
      int num5 = 1;
      bool flag3 = false;
      int num6 = this.doesTileHaveProperty(num3, num4, "Diggable", "Back") != null ? 1 : 0;
      if (num2 == num6 && !this.IsNoSpawnTile(vector2_3) && this.doesTileHaveProperty(num3, num4, "Type", "Back") != "Wood")
      {
        bool flag4 = false;
        if (this.CanItemBePlacedHere(vector2_3) && !this.terrainFeatures.ContainsKey(vector2_3))
          flag4 = true;
        else if (spawnFromOldWeeds)
        {
          Object object2;
          if (this.objects.TryGetValue(vector2_3, out object2))
          {
            if (flag2)
              flag4 = false;
            else if (!object2.IsTapper())
              flag4 = true;
          }
          TerrainFeature terrainFeature;
          if (!flag4 && this.terrainFeatures.TryGetValue(vector2_3, out terrainFeature) && (terrainFeature is HoeDirt || terrainFeature is Flooring))
            flag4 = !flag2 && this.getLargeTerrainFeatureAt(num3, num4) == null;
        }
        if (flag4)
        {
          if (spawnFromOldWeeds)
            flag3 = true;
          else if (!this.objects.ContainsKey(vector2_3))
            flag3 = true;
        }
      }
      if (flag3)
      {
        string itemId = (string) null;
        if (this is Desert)
        {
          itemId = "(O)750";
        }
        else
        {
          if (Game1.random.NextBool() && !weedsOnly && (!spawnFromOldWeeds || object1.IsBreakableStone() || object1.IsTwig()))
            itemId = Game1.random.Choose<string>("(O)294", "(O)295", "(O)343", "(O)450");
          else if (!spawnFromOldWeeds || object1.IsWeeds())
          {
            itemId = GameLocation.getWeedForSeason(Game1.random, this.GetSeason());
            if (this.IsGreenRainingHere())
            {
              if (this.doesTileHavePropertyNoNull((int) ((double) vector2_1.X + (double) vector2_2.X), (int) ((double) vector2_1.Y + (double) vector2_2.Y), "Type", "Back") == (this.IsFarm ? "Dirt" : "Grass"))
              {
                int num7 = Game1.random.Next(8);
                itemId = "(O)GreenRainWeeds" + num7.ToString();
                if (num7 == 2 || num7 == 3 || num7 == 7)
                  num5 = 2;
              }
              else
                itemId = (string) null;
            }
          }
          if (this is Farm && !spawnFromOldWeeds && Game1.random.NextDouble() < 0.05 && !this.terrainFeatures.ContainsKey(vector2_3))
          {
            this.terrainFeatures.Add(vector2_3, (TerrainFeature) new Tree((Game1.random.Next(3) + 1).ToString(), Game1.random.Next(3)));
            continue;
          }
        }
        if (itemId != null)
        {
          bool flag5 = false;
          Object object3;
          if (this.objects.TryGetValue(vector2_1 + vector2_2, out object3))
          {
            if (!flag2 && !(object3 is Fence) && !(object3 is Chest) && !(object3.QualifiedItemId == "(O)590") && !(object3.QualifiedItemId == "(BC)MushroomLog"))
            {
              if (object3.name.Length > 0 && object3.Category != -999)
              {
                flag5 = true;
                Game1.debugOutput = object3.Name + " was destroyed";
              }
              this.objects.Remove(vector2_1 + vector2_2);
            }
            else
              continue;
          }
          TerrainFeature terrainFeature;
          if (this.terrainFeatures.TryGetValue(vector2_1 + vector2_2, out terrainFeature))
          {
            try
            {
              flag5 = terrainFeature is HoeDirt || terrainFeature is Flooring;
            }
            catch (Exception ex)
            {
            }
            if (!flag5 || this.IsGreenRainingHere())
              break;
            this.terrainFeatures.Remove(vector2_1 + vector2_2);
          }
          if (flag5 && this is Farm && Game1.stats.DaysPlayed > 1U && !flag1)
          {
            flag1 = true;
            Game1.multiplayer.broadcastGlobalMessage("Strings\\Locations:Farm_WeedsDestruction");
          }
          Object object4 = ItemRegistry.Create<Object>(itemId);
          object4.minutesUntilReady.Value = num5;
          this.objects.TryAdd(vector2_1 + vector2_2, object4);
        }
      }
    }
  }

  [Obsolete("Use removeObjectsAndSpawned instead.")]
  public virtual void removeEverythingExceptCharactersFromThisTile(int x, int y)
  {
    this.removeObjectsAndSpawned(x, y, 1, 1);
  }

  /// <summary>Remove all objects, bushes, resource clumps, and terrain features within an area.</summary>
  /// <param name="x">The top-left X position of the area to clear.</param>
  /// <param name="y">The top-right X position of the area to clear.</param>
  /// <param name="width">The width of the area to clear.</param>
  /// <param name="height">The height of the area to clear.</param>
  public virtual void removeObjectsAndSpawned(int x, int y, int width, int height)
  {
    Microsoft.Xna.Framework.Rectangle pixelArea = new Microsoft.Xna.Framework.Rectangle(x * 64 /*0x40*/, y * 64 /*0x40*/, width * 64 /*0x40*/, height * 64 /*0x40*/);
    int num1 = x + width - 1;
    int num2 = y + height - 1;
    for (int y1 = y; y1 <= num2; ++y1)
    {
      for (int x1 = x; x1 <= num1; ++x1)
      {
        Vector2 key = new Vector2((float) x1, (float) y1);
        this.terrainFeatures.Remove(key);
        this.objects.Remove(key);
      }
    }
    this.largeTerrainFeatures.RemoveWhere((Func<LargeTerrainFeature, bool>) (feature => feature.getBoundingBox().Intersects(pixelArea)));
    this.resourceClumps.RemoveWhere((Func<ResourceClump, bool>) (clump => clump.getBoundingBox().Intersects(pixelArea)));
  }

  public virtual string getFootstepSoundReplacement(string footstep) => footstep;

  public virtual void removeEverythingFromThisTile(int x, int y)
  {
    Vector2 tile = new Vector2((float) x, (float) y);
    Point pixel = Utility.Vector2ToPoint(tile * 64f + new Vector2(32f));
    this.resourceClumps.RemoveWhere((Func<ResourceClump, bool>) (clump => clump.Tile == tile));
    this.terrainFeatures.Remove(tile);
    this.objects.Remove(tile);
    this.furniture.RemoveWhere((Func<Furniture, bool>) (f => f.GetBoundingBox().Contains(pixel)));
    this.characters.RemoveWhere((Func<NPC, bool>) (npc => npc.Tile == tile && npc is Monster));
  }

  public virtual bool TryGetLocationEvents(
    out string assetName,
    out Dictionary<string, string> events)
  {
    events = (Dictionary<string, string>) null;
    assetName = this.NameOrUniqueName == Game1.player.homeLocation.Value ? "Data\\Events\\FarmHouse" : "Data\\Events\\" + this.name.Value;
    try
    {
      if (Game1.content.DoesAssetExist<Dictionary<string, string>>(assetName))
        events = Game1.content.Load<Dictionary<string, string>>(assetName);
    }
    catch (Exception ex)
    {
      Game1.log.Error($"Failed loading events for location '{this.NameOrUniqueName}' from asset '{assetName}'.", ex);
    }
    if (events == null)
      events = new Dictionary<string, string>();
    if (assetName != "Data\\Events\\FarmHouse")
    {
      foreach (KeyValuePair<string, string> keyValuePair in Game1.content.Load<Dictionary<string, string>>("Data\\Events\\FarmHouse"))
      {
        if (keyValuePair.Key.StartsWith("558291/") || keyValuePair.Key.StartsWith("558292/"))
          events.TryAdd(keyValuePair.Key, keyValuePair.Value);
      }
    }
    if (this.Name == "Trailer_Big")
    {
      events = new Dictionary<string, string>((IDictionary<string, string>) events);
      Dictionary<string, string> dictionary = Game1.content.Load<Dictionary<string, string>>("Data\\Events\\Trailer");
      if (dictionary != null)
      {
        foreach (string key in dictionary.Keys)
        {
          string str = dictionary[key];
          if (!(this.name.Value == "Trailer_Big") || !events.ContainsKey(key))
          {
            if (key.StartsWith("36/"))
              str = str.Replace("/farmer -30 30 0", "/farmer 12 19 0").Replace("/playSound doorClose/warp farmer 12 9", "/move farmer 0 -10 0");
            else if (key.StartsWith("35/"))
              str = str.Replace("/farmer -30 30 0", "/farmer 12 19 0").Replace("/warp farmer 12 9/playSound doorClose", "/move farmer 0 -10 0").Replace("/warp farmer -40 -40/playSound doorClose", "/move farmer 0 10 0/warp farmer -40 -40");
            events[key] = str;
          }
        }
      }
    }
    return events.Count > 0;
  }

  public static bool IsValidLocationEvent(string key, string eventScript)
  {
    if (!key.Contains('/') && !int.TryParse(key, out int _))
      return false;
    string[] commands = Event.ParseCommands(eventScript);
    if (commands.Length < 3)
      return false;
    string str = commands[1];
    return str.Length != 0 && (!(str != "follow") || char.IsDigit(str[0]) || str[0] == '-');
  }

  public virtual void checkForEvents()
  {
    if (Game1.killScreen && !Game1.eventUp)
    {
      if (Game1.player.bathingClothes.Value)
        Game1.player.changeOutOfSwimSuit();
      if (this.name.Equals((object) "Mine"))
      {
        string sub1;
        string sub2;
        switch (Game1.random.Next(7))
        {
          case 0:
            sub1 = "Robin";
            sub2 = "Data\\ExtraDialogue:Mines_PlayerKilled_Robin";
            break;
          case 1:
            sub1 = "Clint";
            sub2 = "Data\\ExtraDialogue:Mines_PlayerKilled_Clint";
            break;
          case 2:
            sub1 = "Maru";
            sub2 = Game1.player.spouse == "Maru" ? "Data\\ExtraDialogue:Mines_PlayerKilled_Maru_Spouse" : "Data\\ExtraDialogue:Mines_PlayerKilled_Maru_NotSpouse";
            break;
          default:
            sub1 = "Linus";
            sub2 = "Data\\ExtraDialogue:Mines_PlayerKilled_Linus";
            break;
        }
        if (Game1.random.NextDouble() < 0.1 && Game1.player.spouse != null && !Game1.player.isEngaged() && Game1.player.spouse.Length > 1)
        {
          sub1 = Game1.player.spouse;
          sub2 = Game1.player.IsMale ? "Data\\ExtraDialogue:Mines_PlayerKilled_Spouse_PlayerMale" : "Data\\ExtraDialogue:Mines_PlayerKilled_Spouse_PlayerFemale";
        }
        this.currentEvent = new Event(Game1.content.LoadString("Data\\Events\\Mine:PlayerKilled", (object) sub1, (object) sub2, (object) ArgUtility.EscapeQuotes(Game1.player.Name)));
      }
      else if (this is IslandLocation)
      {
        string sub1 = "Willy";
        string sub2 = "Data\\ExtraDialogue:Island_willy_rescue";
        if (Game1.player.friendshipData.ContainsKey("Leo") && Game1.random.NextBool())
        {
          sub1 = "Leo";
          sub2 = "Data\\ExtraDialogue:Island_leo_rescue";
        }
        this.currentEvent = new Event(Game1.content.LoadString("Data\\Events\\IslandSouth:PlayerKilled", (object) sub1, (object) sub2, (object) ArgUtility.EscapeQuotes(Game1.player.Name)));
      }
      else if (this.name.Equals((object) "Hospital"))
      {
        this.currentEvent = new Event(Game1.content.LoadString("Data\\Events\\Hospital:PlayerKilled", (object) ArgUtility.EscapeQuotes(Game1.player.Name)));
      }
      else
      {
        try
        {
          string assetName;
          Dictionary<string, string> events;
          if (this.TryGetLocationEvents(out assetName, out events))
          {
            string eventString;
            if (events.TryGetValue("PlayerKilled", out eventString))
              this.currentEvent = new Event(eventString, assetName, "PlayerKilled");
          }
        }
        catch (Exception ex)
        {
        }
      }
      if (this.currentEvent != null)
        Game1.eventUp = true;
      Game1.changeMusicTrack("none", true);
      Game1.killScreen = false;
      Game1.player.health = 10;
    }
    else if (!Game1.eventUp && Game1.weddingsToday.Count > 0 && (Game1.CurrentEvent == null || Game1.CurrentEvent.id != "-2") && Game1.currentLocation != null && !Game1.currentLocation.IsTemporary)
    {
      this.currentEvent = Game1.getAvailableWeddingEvent();
      if (this.currentEvent == null)
        return;
      this.startEvent(this.currentEvent);
    }
    else
    {
      if (Game1.eventUp || Game1.farmEvent != null)
        return;
      string festival = $"{Game1.currentSeason}{Game1.dayOfMonth}";
      try
      {
        Event ev;
        if (Event.tryToLoadFestival(festival, out ev))
          this.currentEvent = ev;
      }
      catch (Exception ex)
      {
      }
      if (!Game1.eventUp && this.currentEvent == null && Game1.farmEvent == null)
      {
        if (!this.IsGreenRainingHere())
        {
          string assetName;
          Dictionary<string, string> events;
          try
          {
            if (!this.TryGetLocationEvents(out assetName, out events))
              return;
          }
          catch
          {
            return;
          }
          if (events != null)
          {
            foreach (string key in events.Keys)
            {
              string eventID = this.checkEventPrecondition(key);
              if (!string.IsNullOrEmpty(eventID) && eventID != "-1" && GameLocation.IsValidLocationEvent(key, events[key]))
              {
                this.currentEvent = new Event(events[key], assetName, eventID);
                break;
              }
            }
            PetData data;
            if (this.currentEvent == null && Game1.IsMasterGame && Game1.stats.DaysPlayed >= 20U && !Game1.player.mailReceived.Contains("rejectedPet") && !Game1.player.hasPet() && Pet.TryGetData(Game1.player.whichPetType, out data) && this.Name == data.AdoptionEventLocation && !string.IsNullOrWhiteSpace(data.AdoptionEventId) && !Game1.player.eventsSeen.Contains(data.AdoptionEventId))
              Game1.PlayEvent(data.AdoptionEventId, false, false);
          }
        }
      }
      if (this.currentEvent == null)
        return;
      this.startEvent(this.currentEvent);
    }
  }

  public Event findEventById(string id, Farmer farmerActor = null)
  {
    if (id == "-2")
    {
      long? spouse = Game1.player.team.GetSpouse(farmerActor.UniqueMultiplayerID);
      if (farmerActor == null || !spouse.HasValue || Game1.otherFarmers.ContainsKey(spouse.Value))
        return Utility.getWeddingEvent(farmerActor);
    }
    string assetName;
    Dictionary<string, string> events;
    try
    {
      if (!this.TryGetLocationEvents(out assetName, out events))
        return (Event) null;
    }
    catch
    {
      return (Event) null;
    }
    foreach (KeyValuePair<string, string> keyValuePair in events)
    {
      if (Event.SplitPreconditions(keyValuePair.Key)[0] == id)
        return new Event(keyValuePair.Value, assetName, id, farmerActor);
    }
    return (Event) null;
  }

  public virtual void startEvent(Event evt)
  {
    if (Game1.eventUp || Game1.eventOver)
      return;
    this.currentEvent = evt;
    this.ResetForEvent(evt);
    if (evt.exitLocation == null)
      evt.exitLocation = Game1.getLocationRequest(this.NameOrUniqueName, this.isStructure.Value);
    if (Game1.player.mount != null)
    {
      Horse mount = Game1.player.mount;
      mount.currentLocation = this;
      mount.dismount();
      Microsoft.Xna.Framework.Rectangle boundingBox = mount.GetBoundingBox();
      Vector2 position = mount.Position;
      if (mount.currentLocation != null && mount.currentLocation.isCollidingPosition(boundingBox, Game1.viewport, false, 0, false, (Character) mount, true))
      {
        boundingBox.X -= 64 /*0x40*/;
        if (!mount.currentLocation.isCollidingPosition(boundingBox, Game1.viewport, false, 0, false, (Character) mount, true))
        {
          position.X -= 64f;
          mount.Position = position;
        }
        else
        {
          boundingBox.X += 128 /*0x80*/;
          if (!mount.currentLocation.isCollidingPosition(boundingBox, Game1.viewport, false, 0, false, (Character) mount, true))
          {
            position.X += 64f;
            mount.Position = position;
          }
        }
      }
    }
    foreach (NPC character in this.characters)
      character.clearTextAboveHead();
    Game1.eventUp = true;
    Game1.displayHUD = false;
    Game1.player.CanMove = false;
    Game1.player.showNotCarrying();
    this.critters?.Clear();
    if (this.currentEvent == null)
      return;
    Game1.player.autoGenerateActiveDialogueEvent("eventSeen_" + this.currentEvent.id);
  }

  public virtual void drawBackground(SpriteBatch b)
  {
  }

  public virtual void drawWater(SpriteBatch b)
  {
    this.currentEvent?.drawUnderWater(b);
    if (this.waterTiles == null)
      return;
    for (int y = Math.Max(0, Game1.viewport.Y / 64 /*0x40*/ - 1); y < Math.Min(this.map.Layers[0].LayerHeight, (Game1.viewport.Y + Game1.viewport.Height) / 64 /*0x40*/ + 2); ++y)
    {
      for (int x = Math.Max(0, Game1.viewport.X / 64 /*0x40*/ - 1); x < Math.Min(this.map.Layers[0].LayerWidth, (Game1.viewport.X + Game1.viewport.Width) / 64 /*0x40*/ + 1); ++x)
      {
        if (this.waterTiles.waterTiles[x, y].isWater && this.waterTiles.waterTiles[x, y].isVisible)
          this.drawWaterTile(b, x, y);
      }
    }
  }

  public virtual void drawWaterTile(SpriteBatch b, int x, int y)
  {
    this.drawWaterTile(b, x, y, this.waterColor.Value);
  }

  public void drawWaterTile(SpriteBatch b, int x, int y, Color color)
  {
    int num = y == this.map.Layers[0].LayerHeight - 1 ? 1 : (!this.waterTiles[x, y + 1] ? 1 : 0);
    bool flag = y == 0 || !this.waterTiles[x, y - 1];
    b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (x * 64 /*0x40*/), (float) (y * 64 /*0x40*/ - (!flag ? (int) this.waterPosition : 0)))), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(this.waterAnimationIndex * 64 /*0x40*/, 2064 + ((x + y) % 2 == 0 ? (this.waterTileFlip ? 128 /*0x80*/ : 0) : (this.waterTileFlip ? 0 : 128 /*0x80*/)) + (flag ? (int) this.waterPosition : 0), 64 /*0x40*/, 64 /*0x40*/ + (flag ? (int) -(double) this.waterPosition : 0))), color, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.56f);
    if (num == 0)
      return;
    b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (x * 64 /*0x40*/), (float) ((y + 1) * 64 /*0x40*/ - (int) this.waterPosition))), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(this.waterAnimationIndex * 64 /*0x40*/, 2064 + ((x + (y + 1)) % 2 == 0 ? (this.waterTileFlip ? 128 /*0x80*/ : 0) : (this.waterTileFlip ? 0 : 128 /*0x80*/)), 64 /*0x40*/, 64 /*0x40*/ - (int) (64.0 - (double) this.waterPosition) - 1)), color, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.56f);
  }

  public virtual void drawFloorDecorations(SpriteBatch b)
  {
    int num1 = 1;
    Microsoft.Xna.Framework.Rectangle rectangle1 = new Microsoft.Xna.Framework.Rectangle(Game1.viewport.X / 64 /*0x40*/ - num1, Game1.viewport.Y / 64 /*0x40*/ - num1, (int) Math.Ceiling((double) Game1.viewport.Width / 64.0) + 2 * num1, (int) Math.Ceiling((double) Game1.viewport.Height / 64.0) + 3 + 2 * num1);
    Microsoft.Xna.Framework.Rectangle rectangle2 = new Microsoft.Xna.Framework.Rectangle();
    if (this.buildings.Count > 0)
    {
      foreach (Building building in this.buildings)
      {
        int tilePropertyRadius = building.GetAdditionalTilePropertyRadius();
        Microsoft.Xna.Framework.Rectangle sourceRect = building.getSourceRect();
        rectangle2.X = building.tileX.Value - tilePropertyRadius;
        rectangle2.Width = building.tilesWide.Value + tilePropertyRadius * 2;
        int num2 = building.tileY.Value + building.tilesHigh.Value + tilePropertyRadius;
        int num3 = num2 - (int) Math.Ceiling((double) sourceRect.Height * 4.0 / 64.0) - tilePropertyRadius;
        rectangle2.Y = num3;
        rectangle2.Height = num2 - num3;
        if (rectangle2.Intersects(rectangle1))
          building.drawBackground(b);
      }
    }
    if (!Game1.isFestival() && this.terrainFeatures.Length > 0)
    {
      Vector2 key = new Vector2();
      for (int index1 = Game1.viewport.Y / 64 /*0x40*/ - 1; index1 < (Game1.viewport.Y + Game1.viewport.Height) / 64 /*0x40*/ + 7; ++index1)
      {
        for (int index2 = Game1.viewport.X / 64 /*0x40*/ - 1; index2 < (Game1.viewport.X + Game1.viewport.Width) / 64 /*0x40*/ + 3; ++index2)
        {
          key.X = (float) index2;
          key.Y = (float) index1;
          TerrainFeature terrainFeature;
          if (this.terrainFeatures.TryGetValue(key, out terrainFeature) && terrainFeature is Flooring)
            terrainFeature.draw(b);
        }
      }
    }
    if (Game1.eventUp)
    {
      switch (this)
      {
        case Farm _:
        case FarmHouse _:
          break;
        default:
          return;
      }
    }
    Furniture.isDrawingLocationFurniture = true;
    foreach (Furniture furniture in this.furniture)
    {
      if (furniture.furniture_type.Value == 12)
        furniture.draw(b, -1, -1, 1f);
    }
    Furniture.isDrawingLocationFurniture = false;
  }

  public TemporaryAnimatedSprite getTemporarySpriteByID(int id)
  {
    for (int index = 0; index < this.temporarySprites.Count; ++index)
    {
      if (this.temporarySprites[index].id == id)
        return this.temporarySprites[index];
    }
    return (TemporaryAnimatedSprite) null;
  }

  protected void drawDebris(SpriteBatch b)
  {
    int num = 0;
    foreach (Debris debri in this.debris)
    {
      ++num;
      if (debri.item != null)
      {
        Vector2 visualPosition = debri.Chunks[0].GetVisualPosition();
        if (debri.item is Object @object && @object.bigCraftable.Value)
          @object.drawInMenu(b, Utility.snapDrawPosition(Game1.GlobalToLocal(Game1.viewport, visualPosition + new Vector2(32f, 32f))), 1.6f, 1f, (float) (((double) (debri.chunkFinalYLevel + 64 /*0x40*/ + 8) + (double) visualPosition.X / 10000.0) / 10000.0), StackDrawType.Hide, Color.White, true);
        else
          debri.item.drawInMenu(b, Utility.snapDrawPosition(Game1.GlobalToLocal(Game1.viewport, visualPosition + new Vector2(32f, 32f))), (float) (0.800000011920929 + (double) debri.itemQuality * 0.10000000149011612), 1f, (float) (((double) (debri.chunkFinalYLevel + 64 /*0x40*/ + 8) + (double) visualPosition.X / 10000.0) / 10000.0), StackDrawType.Hide, Color.White, true);
      }
      else
      {
        switch (debri.debrisType.Value)
        {
          case Debris.DebrisType.LETTERS:
            Chunk chunk1 = debri.Chunks[0];
            Vector2 visualPosition1 = chunk1.GetVisualPosition();
            Game1.drawWithBorder(debri.debrisMessage.Value, Color.Black, debri.nonSpriteChunkColor.Value, Utility.snapDrawPosition(Game1.GlobalToLocal(Game1.viewport, visualPosition1)), chunk1.rotation, chunk1.scale, (float) (((double) visualPosition1.Y + 64.0) / 10000.0));
            continue;
          case Debris.DebrisType.SPRITECHUNKS:
            for (int index = 0; index < debri.Chunks.Count; ++index)
            {
              Chunk chunk2 = debri.Chunks[0];
              Vector2 visualPosition2 = chunk2.GetVisualPosition();
              b.Draw(debri.spriteChunkSheet, Utility.snapDrawPosition(Game1.GlobalToLocal(Game1.viewport, visualPosition2)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(chunk2.xSpriteSheet.Value, chunk2.ySpriteSheet.Value, Math.Min(debri.sizeOfSourceRectSquares.Value, debri.spriteChunkSheet.Bounds.Width), Math.Min(debri.sizeOfSourceRectSquares.Value, debri.spriteChunkSheet.Bounds.Height))), debri.nonSpriteChunkColor.Value * chunk2.alpha, chunk2.rotation, new Vector2((float) (debri.sizeOfSourceRectSquares.Value / 2), (float) (debri.sizeOfSourceRectSquares.Value / 2)), chunk2.scale, SpriteEffects.None, (float) (((double) (debri.chunkFinalYLevel + 16 /*0x10*/) + (double) visualPosition2.X / 10000.0) / 10000.0));
            }
            continue;
          case Debris.DebrisType.NUMBERS:
            Chunk chunk3 = debri.Chunks[0];
            Vector2 visualPosition3 = chunk3.GetVisualPosition();
            NumberSprite.draw(debri.chunkType.Value, b, Game1.GlobalToLocal(Game1.viewport, Utility.snapDrawPosition(new Vector2(visualPosition3.X, (float) debri.chunkFinalYLevel - ((float) debri.chunkFinalYLevel - visualPosition3.Y)))), debri.nonSpriteChunkColor.Value, chunk3.scale * 0.75f, (float) (0.98000001907348633 + 9.9999997473787516E-05 * (double) num), chunk3.alpha, -1 * (int) ((double) debri.chunkFinalYLevel - (double) visualPosition3.Y) / 2);
            continue;
          default:
            if (debri.itemId.Value != null)
            {
              ParsedItemData dataOrErrorItem = ItemRegistry.GetDataOrErrorItem(debri.itemId.Value);
              Texture2D texture = dataOrErrorItem.GetTexture();
              float scale1 = debri.debrisType.Value == Debris.DebrisType.RESOURCE || debri.floppingFish.Value ? 4f : (float) (4.0 * (0.800000011920929 + (double) debri.itemQuality * 0.10000000149011612));
              for (int index = 0; index < debri.Chunks.Count; ++index)
              {
                Chunk chunk4 = debri.Chunks[index];
                Vector2 visualPosition4 = chunk4.GetVisualPosition();
                Microsoft.Xna.Framework.Rectangle rectangle = debri.debrisType.Value == Debris.DebrisType.RESOURCE ? dataOrErrorItem.GetSourceRect(chunk4.randomOffset) : dataOrErrorItem.GetSourceRect();
                SpriteEffects effects = !debri.floppingFish.Value || chunk4.bounces % 2 != 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                b.Draw(texture, Utility.snapDrawPosition(Game1.GlobalToLocal(Game1.viewport, visualPosition4)), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.White, 0.0f, Vector2.Zero, scale1, effects, (float) (((double) (debri.chunkFinalYLevel + 32 /*0x20*/) + (double) visualPosition4.X / 10000.0) / 10000.0));
                SpriteBatch spriteBatch = b;
                Texture2D shadowTexture = Game1.shadowTexture;
                Vector2 position = Utility.snapDrawPosition(Game1.GlobalToLocal(Game1.viewport, new Vector2(visualPosition4.X + 25.6f, (float) ((debri.chunksMoveTowardPlayer ? (double) visualPosition4.Y + 8.0 : (double) debri.chunkFinalYLevel) + 32.0) + (float) (12 * debri.itemQuality))));
                Microsoft.Xna.Framework.Rectangle? sourceRectangle = new Microsoft.Xna.Framework.Rectangle?(Game1.shadowTexture.Bounds);
                Color color = Color.White * 0.75f;
                Microsoft.Xna.Framework.Rectangle bounds = Game1.shadowTexture.Bounds;
                double x = (double) bounds.Center.X;
                bounds = Game1.shadowTexture.Bounds;
                double y = (double) bounds.Center.Y;
                Vector2 origin = new Vector2((float) x, (float) y);
                double scale2 = (double) Math.Min(3f, (float) (3.0 - (debri.chunksMoveTowardPlayer ? 0.0 : ((double) debri.chunkFinalYLevel - (double) visualPosition4.Y) / 96.0)));
                double layerDepth = (double) debri.chunkFinalYLevel / 10000.0;
                spriteBatch.Draw(shadowTexture, position, sourceRectangle, color, 0.0f, origin, (float) scale2, SpriteEffects.None, (float) layerDepth);
              }
              continue;
            }
            for (int index = 0; index < debri.Chunks.Count; ++index)
            {
              Vector2 position = Utility.snapDrawPosition(Game1.GlobalToLocal(Game1.viewport, debri.Chunks[index].position.Value));
              Microsoft.Xna.Framework.Rectangle standardTileSheet = Game1.getSourceRectForStandardTileSheet(Game1.debrisSpriteSheet, debri.chunkType.Value + debri.Chunks[index].randomOffset, 16 /*0x10*/, 16 /*0x10*/);
              float layerDepth = (float) (((double) debri.Chunks[index].position.Y + 128.0 + (double) debri.Chunks[index].position.X / 10000.0) / 10000.0);
              b.Draw(Game1.debrisSpriteSheet, position, new Microsoft.Xna.Framework.Rectangle?(standardTileSheet), debri.chunksColor.Value, 0.0f, Vector2.Zero, 4f * debri.scale.Value, SpriteEffects.None, layerDepth);
            }
            continue;
        }
      }
    }
  }

  public virtual bool shouldHideCharacters() => false;

  protected virtual void drawCharacters(SpriteBatch b)
  {
    if (this.shouldHideCharacters() || Game1.eventUp && (Game1.CurrentEvent == null || !Game1.CurrentEvent.showWorldCharacters))
      return;
    for (int index = 0; index < this.characters.Count; ++index)
    {
      if (this.characters[index] != null)
        this.characters[index].draw(b);
    }
  }

  protected virtual void drawFarmers(SpriteBatch b)
  {
    if (this.shouldHideCharacters() || Game1.currentMinigame != null)
      return;
    if (this.currentEvent == null || this.currentEvent.isFestival || this.currentEvent.farmerActors.Count == 0)
    {
      foreach (Farmer farmer in this.farmers)
      {
        if (!Game1.multiplayer.isDisconnecting(farmer.UniqueMultiplayerID))
          farmer.draw(b);
      }
    }
    else
      this.currentEvent.drawFarmers(b);
  }

  public virtual void DrawFarmerUsernames(SpriteBatch b)
  {
    if (this.shouldHideCharacters() || Game1.currentMinigame != null || this.currentEvent != null && !this.currentEvent.isFestival && this.currentEvent.farmerActors.Count != 0)
      return;
    foreach (Farmer farmer in this.farmers)
    {
      if (!Game1.multiplayer.isDisconnecting(farmer.UniqueMultiplayerID))
        farmer.DrawUsername(b);
    }
  }

  public virtual void draw(SpriteBatch b)
  {
    if (this.animals.Length > 0)
    {
      foreach (Character character in this.animals.Values)
        character.draw(b);
    }
    if (this.mapSeats.Count > 0)
    {
      foreach (MapSeat mapSeat in this.mapSeats)
        mapSeat.Draw(b);
    }
    Microsoft.Xna.Framework.Rectangle rectangle1 = new Microsoft.Xna.Framework.Rectangle(Game1.viewport.X, Game1.viewport.Y, Game1.viewport.Width, Game1.viewport.Height);
    rectangle1.Inflate(128 /*0x80*/, 128 /*0x80*/);
    if (this is Woods && Game1.eventUp)
    {
      Event currentEvent = this.currentEvent;
      if ((currentEvent != null ? (!currentEvent.showGroundObjects ? 1 : 0) : 1) != 0)
        goto label_21;
    }
    if (this.resourceClumps.Count > 0)
    {
      foreach (ResourceClump resourceClump in this.resourceClumps)
      {
        if (resourceClump.getRenderBounds().Intersects(rectangle1))
          resourceClump.draw(b);
      }
    }
label_21:
    this._currentLocationFarmersForDisambiguating.Clear();
    foreach (Farmer farmer in this.farmers)
    {
      farmer.drawLayerDisambiguator = 0.0f;
      this._currentLocationFarmersForDisambiguating.Add(farmer);
    }
    if (this._currentLocationFarmersForDisambiguating.Contains(Game1.player))
    {
      this._currentLocationFarmersForDisambiguating.Remove(Game1.player);
      this._currentLocationFarmersForDisambiguating.Insert(0, Game1.player);
    }
    float num1 = 0.0001f;
    for (int index1 = 0; index1 < this._currentLocationFarmersForDisambiguating.Count; ++index1)
    {
      for (int index2 = index1 + 1; index2 < this._currentLocationFarmersForDisambiguating.Count; ++index2)
      {
        Farmer farmer1 = this._currentLocationFarmersForDisambiguating[index1];
        Farmer farmer2 = this._currentLocationFarmersForDisambiguating[index2];
        if (!farmer2.IsSitting() && (double) Math.Abs(farmer1.getDrawLayer() - farmer2.getDrawLayer()) < (double) num1 && (double) Math.Abs(farmer1.position.X - farmer2.position.X) < 64.0)
          farmer2.drawLayerDisambiguator += farmer1.getDrawLayer() - num1 - farmer2.getDrawLayer();
      }
    }
    this.drawCharacters(b);
    this.drawFarmers(b);
    if (this.critters != null && Game1.farmEvent == null)
    {
      for (int index = 0; index < this.critters.Count; ++index)
        this.critters[index].draw(b);
    }
    this.drawDebris(b);
    if ((!Game1.eventUp || this.currentEvent != null && this.currentEvent.showGroundObjects) && this.objects.Length > 0)
    {
      Vector2 key = new Vector2();
      for (int index3 = Game1.viewport.Y / 64 /*0x40*/ - 1; index3 < (Game1.viewport.Y + Game1.viewport.Height) / 64 /*0x40*/ + 3; ++index3)
      {
        for (int index4 = Game1.viewport.X / 64 /*0x40*/ - 1; index4 < (Game1.viewport.X + Game1.viewport.Width) / 64 /*0x40*/ + 1; ++index4)
        {
          key.X = (float) index4;
          key.Y = (float) index3;
          Object @object;
          if (this.objects.TryGetValue(key, out @object))
            @object.draw(b, (int) key.X, (int) key.Y);
        }
      }
    }
    if (this.TemporarySprites.Count > 0)
    {
      foreach (TemporaryAnimatedSprite temporarySprite in this.TemporarySprites)
      {
        if (!temporarySprite.drawAboveAlwaysFront)
          temporarySprite.draw(b);
      }
    }
    this.interiorDoors.Draw(b);
    NetCollection<LargeTerrainFeature> largeTerrainFeatures = this.largeTerrainFeatures;
    if ((largeTerrainFeatures != null ? (largeTerrainFeatures.Count > 0 ? 1 : 0) : 0) != 0)
    {
      foreach (LargeTerrainFeature largeTerrainFeature in this.largeTerrainFeatures)
      {
        if (largeTerrainFeature.getRenderBounds().Intersects(rectangle1))
          largeTerrainFeature.draw(b);
      }
    }
    if (this.buildings.Count > 0)
    {
      int num2 = 1;
      rectangle1 = new Microsoft.Xna.Framework.Rectangle(Game1.viewport.X / 64 /*0x40*/ - num2, Game1.viewport.Y / 64 /*0x40*/ - num2, (int) Math.Ceiling((double) Game1.viewport.Width / 64.0) + 2 * num2, (int) Math.Ceiling((double) Game1.viewport.Height / 64.0) + 3 + 2 * num2);
      Microsoft.Xna.Framework.Rectangle rectangle2 = new Microsoft.Xna.Framework.Rectangle();
      foreach (Building building in this.buildings)
      {
        int tilePropertyRadius = building.GetAdditionalTilePropertyRadius();
        Microsoft.Xna.Framework.Rectangle sourceRect = building.getSourceRect();
        rectangle2.X = building.tileX.Value - tilePropertyRadius;
        rectangle2.Width = building.tilesWide.Value + tilePropertyRadius * 2;
        int num3 = building.tileY.Value + building.tilesHigh.Value + tilePropertyRadius;
        int num4 = num3 - (int) Math.Ceiling((double) sourceRect.Height * 4.0 / 64.0) - tilePropertyRadius;
        rectangle2.Y = num4;
        rectangle2.Height = num3 - num4;
        if (rectangle2.Intersects(rectangle1))
          building.draw(b);
      }
    }
    this.fishSplashAnimation?.draw(b);
    this.orePanAnimation?.draw(b);
    if (!Game1.eventUp || this is Farm || this is FarmHouse)
    {
      Furniture.isDrawingLocationFurniture = true;
      foreach (Furniture furniture in this.furniture)
      {
        if (furniture.furniture_type.Value != 12)
          furniture.draw(b, -1, -1, 1f);
      }
      Furniture.isDrawingLocationFurniture = false;
    }
    if (this.showDropboxIndicator && !Game1.eventUp)
    {
      float num5 = (float) (4.0 * Math.Round(Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 250.0), 2));
      b.Draw(Game1.mouseCursors2, Game1.GlobalToLocal(Game1.viewport, new Vector2(this.dropBoxIndicatorLocation.X, this.dropBoxIndicatorLocation.Y + num5)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(114, 53, 6, 10)), Color.White, 0.0f, new Vector2(1f, 4f), 4f, SpriteEffects.None, 1f);
    }
    if (this.lightGlows.Count <= 0)
      return;
    this.drawLightGlows(b);
  }

  public virtual void drawOverlays(SpriteBatch b)
  {
  }

  public virtual void drawAboveFrontLayer(SpriteBatch b)
  {
    Vector2 key = new Vector2();
    for (int index1 = Game1.viewport.Y / 64 /*0x40*/ - 1; index1 < (Game1.viewport.Y + Game1.viewport.Height) / 64 /*0x40*/ + 7; ++index1)
    {
      for (int index2 = Game1.viewport.X / 64 /*0x40*/ - 1; index2 < (Game1.viewport.X + Game1.viewport.Width) / 64 /*0x40*/ + 3; ++index2)
      {
        key.X = (float) index2;
        key.Y = (float) index1;
        TerrainFeature terrainFeature;
        if (this.terrainFeatures.TryGetValue(key, out terrainFeature) && !(terrainFeature is Flooring))
          terrainFeature.draw(b);
      }
    }
  }

  public virtual void drawLightGlows(SpriteBatch b)
  {
    foreach (Vector2 lightGlow in (NetHashSet<Vector2>) this.lightGlows)
    {
      if (!this.lightGlowLayerCache.ContainsKey(lightGlow))
      {
        Furniture furnitureAt = this.GetFurnitureAt(new Vector2((float) (int) ((double) lightGlow.X / 64.0), (float) ((int) ((double) lightGlow.Y / 64.0) + 2)));
        if (furnitureAt != null && furnitureAt.sourceRect.Height / 16 /*0x10*/ - furnitureAt.getTilesHigh() > 1)
          this.lightGlowLayerCache.Add(lightGlow, 2.5f);
        else if (this is FarmHouse farmHouse && farmHouse.upgradeLevel > 0)
        {
          Vector2 vector2_1 = new Vector2((float) (int) ((double) lightGlow.X / 64.0), (float) (int) ((double) lightGlow.Y / 64.0));
          Vector2 vector2_2 = Utility.PointToVector2(farmHouse.getKitchenStandingSpot()) - vector2_1;
          if ((double) vector2_2.Y == 3.0 && ((double) vector2_2.X == 2.0 || (double) vector2_2.X == 3.0 || (double) vector2_2.X == -1.0 || (double) vector2_2.X == -2.0))
            this.lightGlowLayerCache.Add(lightGlow, 1.5f);
          else
            this.lightGlowLayerCache.Add(lightGlow, 10f);
        }
        else
          this.lightGlowLayerCache.Add(lightGlow, 10f);
      }
      b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, lightGlow), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(21, 1695, 41, 67)), Color.White, 0.0f, new Vector2(19f, 22f), 4f, SpriteEffects.None, (float) (((double) lightGlow.Y + 64.0 * (double) this.lightGlowLayerCache[lightGlow]) / 10000.0));
    }
  }

  /// <summary>Try to create a secret note or journal scrap that hasn't been seen by a player, based on the random spawn chance.</summary>
  /// <param name="who">The farmer for whom to create a secret note.</param>
  /// <returns>Returns an unseen secret note/journal scrap, or <see langworld="null" /> if there are none left or the random spawn chance fails.</returns>
  public Object tryToCreateUnseenSecretNote(Farmer who)
  {
    if (this.currentEvent != null && this.currentEvent.isFestival)
      return (Object) null;
    bool journal = this.InIslandContext();
    if (!journal && (who == null || !who.hasMagnifyingGlass))
      return (Object) null;
    string itemId = journal ? "(O)842" : "(O)79";
    int totalNotes;
    int num1 = Utility.GetUnseenSecretNotes(who, journal, out totalNotes).Length - who.Items.CountId(itemId);
    if (num1 <= 0)
      return (Object) null;
    float num2 = (float) (num1 - 1) / (float) Math.Max(1, totalNotes - 1);
    float chance = GameLocation.LAST_SECRET_NOTE_CHANCE + (GameLocation.FIRST_SECRET_NOTE_CHANCE - GameLocation.LAST_SECRET_NOTE_CHANCE) * num2;
    return !Game1.random.NextBool(chance) ? (Object) null : ItemRegistry.Create<Object>(itemId);
  }

  public virtual bool performToolAction(Tool t, int tileX, int tileY)
  {
    if (t is MeleeWeapon t1)
    {
      foreach (FarmAnimal farmAnimal in this.animals.Values)
      {
        if (farmAnimal.GetBoundingBox().Intersects(t1.mostRecentArea))
          farmAnimal.hitWithWeapon(t1);
      }
    }
    foreach (Building building in this.buildings)
    {
      if (building.occupiesTile(new Vector2((float) tileX, (float) tileY)))
        building.performToolAction(t, tileX, tileY);
    }
    for (int index = this.resourceClumps.Count - 1; index >= 0; --index)
    {
      if (this.resourceClumps[index] != null && this.resourceClumps[index].getBoundingBox().Contains(tileX * 64 /*0x40*/, tileY * 64 /*0x40*/) && this.resourceClumps[index].performToolAction(t, 1, this.resourceClumps[index].Tile))
      {
        this.resourceClumps.RemoveAt(index);
        return true;
      }
    }
    Microsoft.Xna.Framework.Rectangle rectangle = new Microsoft.Xna.Framework.Rectangle(tileX * 64 /*0x40*/, tileY * 64 /*0x40*/, 64 /*0x40*/, 64 /*0x40*/);
    foreach (LargeTerrainFeature largeTerrainFeature in this.largeTerrainFeatures)
    {
      if (largeTerrainFeature.getBoundingBox().Intersects(rectangle))
        largeTerrainFeature.performToolAction(t, 1, new Vector2((float) tileX, (float) tileY));
    }
    return false;
  }

  /// <summary>Update the location when the season changes.</summary>
  /// <param name="onLoad">Whether the season is being initialized as part of loading the save, instead of an actual in-game season change.</param>
  public virtual void seasonUpdate(bool onLoad = false)
  {
    Season season = this.GetSeason();
    this.terrainFeatures.RemoveWhere((Func<KeyValuePair<Vector2, TerrainFeature>, bool>) (pair => pair.Value.seasonUpdate(onLoad)));
    this.largeTerrainFeatures?.RemoveWhere((Func<LargeTerrainFeature, bool>) (feature => feature.seasonUpdate(onLoad)));
    foreach (NPC character in this.characters)
    {
      if (!character.IsMonster)
        character.resetSeasonalDialogue();
    }
    if (this.IsOutdoors && !onLoad)
    {
      foreach (KeyValuePair<Vector2, Object> keyValuePair in this.objects.Pairs.ToArray<KeyValuePair<Vector2, Object>>())
      {
        Vector2 key = keyValuePair.Key;
        Object @object = keyValuePair.Value;
        if (@object.IsSpawnedObject && !@object.IsBreakableStone())
          this.objects.Remove(key);
        else if (@object.QualifiedItemId == "(O)590" && this.doesTileHavePropertyNoNull((int) key.X, (int) key.Y, "Diggable", "Back") == "")
          this.objects.Remove(key);
      }
      this.numberOfSpawnedObjectsOnMap = 0;
    }
    switch (season)
    {
      case Season.Spring:
        this.waterColor.Value = new Color(120, 200, (int) byte.MaxValue) * 0.5f;
        break;
      case Season.Summer:
        this.waterColor.Value = new Color(60, 240 /*0xF0*/, (int) byte.MaxValue) * 0.5f;
        break;
      case Season.Fall:
        this.waterColor.Value = new Color((int) byte.MaxValue, 130, 200) * 0.5f;
        break;
      case Season.Winter:
        this.waterColor.Value = new Color(130, 80 /*0x50*/, (int) byte.MaxValue) * 0.5f;
        break;
    }
    if (onLoad || season != Season.Spring || Game1.stats.DaysPlayed <= 1U || this is Farm)
      return;
    this.loadWeeds();
  }

  public List<FarmAnimal> getAllFarmAnimals()
  {
    List<FarmAnimal> list = this.animals.Values.ToList<FarmAnimal>();
    foreach (Building building in this.buildings)
    {
      GameLocation indoors = building.GetIndoors();
      if (indoors != null)
        list.AddRange((IEnumerable<FarmAnimal>) indoors.animals.Values);
    }
    return list;
  }

  public virtual int GetHayCapacity()
  {
    int hayCapacity = 0;
    foreach (Building building in this.buildings)
    {
      if (building.hayCapacity.Value > 0 && building.daysOfConstructionLeft.Value <= 0)
        hayCapacity += building.hayCapacity.Value;
    }
    return hayCapacity;
  }

  public bool CheckPetAnimal(Vector2 position, Farmer who)
  {
    foreach (FarmAnimal farmAnimal in this.animals.Values)
    {
      if (!farmAnimal.wasPet.Value && farmAnimal.GetCursorPetBoundingBox().Contains((int) position.X, (int) position.Y))
      {
        farmAnimal.pet(who);
        return true;
      }
    }
    return false;
  }

  public bool CheckPetAnimal(Microsoft.Xna.Framework.Rectangle rect, Farmer who)
  {
    foreach (FarmAnimal farmAnimal in this.animals.Values)
    {
      if (!farmAnimal.wasPet.Value && farmAnimal.GetBoundingBox().Intersects(rect))
      {
        farmAnimal.pet(who);
        return true;
      }
    }
    return false;
  }

  public bool CheckInspectAnimal(Vector2 position, Farmer who)
  {
    foreach (FarmAnimal farmAnimal in this.animals.Values)
    {
      if (farmAnimal.wasPet.Value && farmAnimal.GetCursorPetBoundingBox().Contains((int) position.X, (int) position.Y))
      {
        farmAnimal.pet(who);
        return true;
      }
    }
    return false;
  }

  public bool CheckInspectAnimal(Microsoft.Xna.Framework.Rectangle rect, Farmer who)
  {
    foreach (FarmAnimal farmAnimal in this.animals.Values)
    {
      if (farmAnimal.wasPet.Value && farmAnimal.GetBoundingBox().Intersects(rect))
      {
        farmAnimal.pet(who);
        return true;
      }
    }
    return false;
  }

  public virtual void updateSeasonalTileSheets(Map map = null)
  {
    if (map == null)
      map = this.Map;
    if (!(this is Summit) && (!this.IsOutdoors || this.Name.Equals("Desert")))
      return;
    map.DisposeTileSheets(Game1.mapDisplayDevice);
    foreach (TileSheet tileSheet in map.TileSheets)
    {
      string imageSource = tileSheet.ImageSource;
      try
      {
        tileSheet.ImageSource = GameLocation.GetSeasonalTilesheetName(tileSheet.ImageSource, this.GetSeasonKey());
        Game1.mapDisplayDevice.LoadTileSheet(tileSheet);
      }
      catch (Exception ex)
      {
        Game1.log.Error($"Location '{this.NameOrUniqueName}' failed to load seasonal asset name '{tileSheet.ImageSource}' for tilesheet ID '{tileSheet.Id}'.", ex);
        tileSheet.ImageSource = imageSource;
      }
    }
    map.LoadTileSheets(Game1.mapDisplayDevice);
  }

  public static string GetSeasonalTilesheetName(string sheet_path, string current_season)
  {
    string fileName = Path.GetFileName(sheet_path);
    if (fileName.StartsWith("spring_") || fileName.StartsWith("summer_") || fileName.StartsWith("fall_") || fileName.StartsWith("winter_"))
      sheet_path = Path.Combine(Path.GetDirectoryName(sheet_path), current_season + fileName.Substring(fileName.IndexOf('_')));
    return sheet_path;
  }

  public virtual string checkEventPrecondition(string precondition)
  {
    return this.checkEventPrecondition(precondition, true);
  }

  public virtual string checkEventPrecondition(string precondition, bool check_seen)
  {
    string[] strArray = Event.SplitPreconditions(precondition);
    string str = strArray[0];
    if (string.IsNullOrEmpty(str) || str == "-1" || check_seen && (Game1.player.eventsSeen.Contains(str) || Game1.eventsSeenSinceLastLocationChange.Contains(str)))
      return "-1";
    for (int index = 1; index < strArray.Length; ++index)
    {
      if (!string.IsNullOrEmpty(strArray[index]) && !Event.CheckPrecondition(this, strArray[0], strArray[index]))
        return "-1";
    }
    return str;
  }

  /// <summary>Get hay from any non-empty silos.</summary>
  /// <param name="currentLocation">The location in which the hay was found.</param>
  public static Object GetHayFromAnySilo(GameLocation currentLocation)
  {
    Object hay;
    if (TryGetHayFrom(currentLocation, out hay) || currentLocation.Name != "Farm" && TryGetHayFrom((GameLocation) Game1.getFarm(), out hay))
      return hay;
    Utility.ForEachLocation((Func<GameLocation, bool>) (location => !TryGetHayFrom(location, out hay)), false);
    return hay;

    static bool TryGetHayFrom(GameLocation location, out Object foundHay)
    {
      if (location.piecesOfHay.Value < 1)
      {
        foundHay = (Object) null;
        return false;
      }
      foundHay = ItemRegistry.Create<Object>("(O)178");
      --location.piecesOfHay.Value;
      return true;
    }
  }

  /// <summary>Store hay in any silos that have available space.</summary>
  /// <param name="count">The number of hay items to store.</param>
  /// <param name="currentLocation">The location in which the hay was found.</param>
  /// <returns>Returns the number of hay that couldn't be stored.</returns>
  public static int StoreHayInAnySilo(int count, GameLocation currentLocation)
  {
    count = currentLocation.tryToAddHay(count);
    if (count > 0 && currentLocation.Name != "Farm")
    {
      count = Game1.getFarm().tryToAddHay(count);
      if (count <= 0)
        return 0;
    }
    if (count > 0)
      Utility.ForEachLocation((Func<GameLocation, bool>) (location =>
      {
        if (location.buildings.Count <= 0)
          return true;
        count = location.tryToAddHay(count);
        return count > 0;
      }), false);
    return count <= 0 ? 0 : count;
  }

  /// <summary>Store hay in the current location's silos, if they have space available.</summary>
  /// <param name="num">The number of hay items to store.</param>
  /// <returns>Returns the number of hay that couldn't be stored.</returns>
  public int tryToAddHay(int num)
  {
    int num1 = Math.Min(this.GetHayCapacity() - this.piecesOfHay.Value, num);
    this.piecesOfHay.Value += num1;
    return num - num1;
  }

  public Building getBuildingAt(Vector2 tile)
  {
    foreach (Building building in this.buildings)
    {
      if (building.occupiesTile(tile) || !building.isTilePassable(tile))
        return building;
    }
    return (Building) null;
  }

  /// <summary>Get a building by its <see cref="F:StardewValley.Buildings.Building.buildingType" /> value.</summary>
  /// <param name="id">The building type key.</param>
  public Building getBuildingByType(string type)
  {
    if (type != null)
    {
      foreach (Building building in this.buildings)
      {
        if (string.Equals(building.buildingType.Value, type, StringComparison.Ordinal))
          return building;
      }
    }
    return (Building) null;
  }

  /// <summary>Get a building by its <see cref="F:StardewValley.Buildings.Building.id" /> value.</summary>
  /// <param name="id">The unique building ID.</param>
  public Building getBuildingById(Guid id)
  {
    if (id != Guid.Empty)
    {
      foreach (Building building in this.buildings)
      {
        if (building.id.Value == id)
          return building;
      }
    }
    return (Building) null;
  }

  /// <summary>Get a building by the unique name of its interior location.</summary>
  /// <param name="id">The building interior location's unique name.</param>
  public Building getBuildingByName(string name)
  {
    if (name != null)
    {
      foreach (Building building in this.buildings)
      {
        if (building.HasIndoorsName(name))
          return building;
      }
    }
    return (Building) null;
  }

  public bool destroyStructure(Vector2 tile)
  {
    Building buildingAt = this.getBuildingAt(tile);
    return buildingAt != null && this.destroyStructure(buildingAt);
  }

  public bool destroyStructure(Building building)
  {
    if (!this.buildings.Remove(building))
      return false;
    building.performActionOnDemolition(this);
    Game1.player.team.SendBuildingDemolishedEvent(this, building);
    return true;
  }

  public bool buildStructure(
    Building building,
    Vector2 tileLocation,
    Farmer who,
    bool skipSafetyChecks = false)
  {
    if (!skipSafetyChecks)
    {
      for (int index1 = 0; index1 < building.tilesHigh.Value; ++index1)
      {
        for (int index2 = 0; index2 < building.tilesWide.Value; ++index2)
          this.pokeTileForConstruction(new Vector2(tileLocation.X + (float) index2, tileLocation.Y + (float) index1));
      }
      foreach (BuildingPlacementTile additionalPlacementTile in building.GetAdditionalPlacementTiles())
      {
        foreach (Point point in additionalPlacementTile.TileArea.GetPoints())
          this.pokeTileForConstruction(new Vector2(tileLocation.X + (float) point.X, tileLocation.Y + (float) point.Y));
      }
      for (int index3 = 0; index3 < building.tilesHigh.Value; ++index3)
      {
        for (int index4 = 0; index4 < building.tilesWide.Value; ++index4)
        {
          Vector2 vector2 = new Vector2(tileLocation.X + (float) index4, tileLocation.Y + (float) index3);
          if (!this.buildings.Contains(building) || !building.occupiesTile(vector2))
          {
            if (!this.isBuildable(vector2))
              return false;
            foreach (Character farmer in this.farmers)
            {
              if (farmer.GetBoundingBox().Intersects(new Microsoft.Xna.Framework.Rectangle(index4 * 64 /*0x40*/, index3 * 64 /*0x40*/, 64 /*0x40*/, 64 /*0x40*/)))
                return false;
            }
          }
        }
      }
      foreach (BuildingPlacementTile additionalPlacementTile in building.GetAdditionalPlacementTiles())
      {
        bool needsToBePassable = additionalPlacementTile.OnlyNeedsToBePassable;
        foreach (Point point in additionalPlacementTile.TileArea.GetPoints())
        {
          int x = point.X;
          int y = point.Y;
          Vector2 vector2 = new Vector2(tileLocation.X + (float) x, tileLocation.Y + (float) y);
          if (!this.buildings.Contains(building) || !building.occupiesTile(vector2))
          {
            if (!this.isBuildable(vector2, needsToBePassable))
              return false;
            if (!needsToBePassable)
            {
              foreach (Character farmer in this.farmers)
              {
                if (farmer.GetBoundingBox().Intersects(new Microsoft.Xna.Framework.Rectangle(x * 64 /*0x40*/, y * 64 /*0x40*/, 64 /*0x40*/, 64 /*0x40*/)))
                  return false;
              }
            }
          }
        }
      }
      if (building.humanDoor.Value != new Point(-1, -1))
      {
        Vector2 vector2 = tileLocation + new Vector2((float) building.humanDoor.X, (float) (building.humanDoor.Y + 1));
        if ((!this.buildings.Contains(building) || !building.occupiesTile(vector2)) && !this.isBuildable(vector2) && !this.isPath(vector2))
          return false;
      }
      string message = building.isThereAnythingtoPreventConstruction(this, tileLocation);
      if (message != null)
      {
        Game1.addHUDMessage(new HUDMessage(message, 3));
        return false;
      }
    }
    building.tileX.Value = (int) tileLocation.X;
    building.tileY.Value = (int) tileLocation.Y;
    for (int index5 = 0; index5 < building.tilesHigh.Value; ++index5)
    {
      for (int index6 = 0; index6 < building.tilesWide.Value; ++index6)
      {
        Vector2 key = new Vector2(tileLocation.X + (float) index6, tileLocation.Y + (float) index5);
        if (this.terrainFeatures.GetValueOrDefault(key) is Flooring)
        {
          bool? flooringUnderneath = building.GetData()?.AllowsFlooringUnderneath;
          if (flooringUnderneath.HasValue && flooringUnderneath.GetValueOrDefault())
            continue;
        }
        this.terrainFeatures.Remove(key);
      }
    }
    if (!this.buildings.Contains(building))
    {
      this.buildings.Add(building);
      who.team.SendBuildingConstructedEvent(this, building, who);
    }
    GameLocation indoors = building.GetIndoors();
    if (indoors is AnimalHouse animalHouse)
    {
      foreach (long num in (NetList<long, NetLong>) animalHouse.animalsThatLiveHere)
      {
        FarmAnimal animal = Utility.getAnimal(num);
        if (animal != null)
          animal.homeInterior = indoors;
        else if (animalHouse.animals.TryGetValue(num, out animal))
          animal.homeInterior = indoors;
      }
    }
    if (indoors != null)
    {
      foreach (Warp warp in (NetList<Warp, NetRef<Warp>>) indoors.warps)
      {
        if (warp.TargetName == this.NameOrUniqueName)
        {
          warp.TargetX = building.humanDoor.X + building.tileX.Value;
          warp.TargetY = building.humanDoor.Y + building.tileY.Value + 1;
        }
      }
    }
    for (int index7 = 0; index7 < building.tilesHigh.Value; ++index7)
    {
      for (int index8 = 0; index8 < building.tilesWide.Value; ++index8)
        RemoveArtifactSpots(new Vector2(tileLocation.X + (float) index8, tileLocation.Y + (float) index7));
    }
    foreach (BuildingPlacementTile additionalPlacementTile in building.GetAdditionalPlacementTiles())
    {
      if (!additionalPlacementTile.OnlyNeedsToBePassable)
      {
        foreach (Point point in additionalPlacementTile.TileArea.GetPoints())
          RemoveArtifactSpots(new Vector2(tileLocation.X + (float) point.X, tileLocation.Y + (float) point.Y));
      }
    }
    return true;

    void RemoveArtifactSpots(Vector2 tile_location)
    {
      if (!(this.getObjectAtTile((int) tile_location.X, (int) tile_location.Y)?.QualifiedItemId == "(O)590"))
        return;
      this.removeObject(tile_location, false);
    }
  }

  /// <summary>Construct a building in the location.</summary>
  /// <param name="typeId">The building type ID in <c>Data/Buildings</c>.</param>
  /// <param name="data">The building data from <c>Data/Buildings</c>.</param>
  /// <param name="tileLocation">The top-left tile position of the building.</param>
  /// <param name="who">The player constructing the building.</param>
  /// <param name="magicalConstruction">Whether construction should complete instantly.</param>
  /// <param name="skipSafetyChecks">Whether to ignore safety checks (e.g. making sure the area is clear).</param>
  /// <returns>Returns whether the building was successfully placed.</returns>
  public bool buildStructure(
    string typeId,
    BuildingData data,
    Vector2 tileLocation,
    Farmer who,
    out Building constructed,
    bool magicalConstruction = false,
    bool skipSafetyChecks = false)
  {
    if (data == null || !skipSafetyChecks && !this.IsBuildableLocation())
    {
      constructed = (Building) null;
      return false;
    }
    int x1 = data.Size.X;
    int y1 = data.Size.Y;
    List<BuildingPlacementTile> buildingPlacementTileList = data.AdditionalPlacementTiles ?? new List<BuildingPlacementTile>(0);
    if (!skipSafetyChecks)
    {
      for (int index1 = 0; index1 < y1; ++index1)
      {
        for (int index2 = 0; index2 < x1; ++index2)
          this.pokeTileForConstruction(new Vector2(tileLocation.X + (float) index2, tileLocation.Y + (float) index1));
      }
      foreach (BuildingPlacementTile buildingPlacementTile in buildingPlacementTileList)
      {
        foreach (Point point in buildingPlacementTile.TileArea.GetPoints())
          this.pokeTileForConstruction(new Vector2(tileLocation.X + (float) point.X, tileLocation.Y + (float) point.Y));
      }
      for (int index3 = 0; index3 < y1; ++index3)
      {
        for (int index4 = 0; index4 < x1; ++index4)
        {
          if (!this.isBuildable(new Vector2(tileLocation.X + (float) index4, tileLocation.Y + (float) index3)))
          {
            constructed = (Building) null;
            return false;
          }
          foreach (Character farmer in this.farmers)
          {
            if (farmer.GetBoundingBox().Intersects(new Microsoft.Xna.Framework.Rectangle(index4 * 64 /*0x40*/, index3 * 64 /*0x40*/, 64 /*0x40*/, 64 /*0x40*/)))
            {
              constructed = (Building) null;
              return false;
            }
          }
        }
      }
      foreach (BuildingPlacementTile buildingPlacementTile in buildingPlacementTileList)
      {
        bool needsToBePassable = buildingPlacementTile.OnlyNeedsToBePassable;
        foreach (Point point in buildingPlacementTile.TileArea.GetPoints())
        {
          int x2 = point.X;
          int y2 = point.Y;
          if (!this.isBuildable(new Vector2(tileLocation.X + (float) x2, tileLocation.Y + (float) y2), needsToBePassable))
          {
            constructed = (Building) null;
            return false;
          }
          if (!needsToBePassable)
          {
            foreach (Character farmer in this.farmers)
            {
              if (farmer.GetBoundingBox().Intersects(new Microsoft.Xna.Framework.Rectangle(x2 * 64 /*0x40*/, y2 * 64 /*0x40*/, 64 /*0x40*/, 64 /*0x40*/)))
              {
                constructed = (Building) null;
                return false;
              }
            }
          }
        }
      }
      if (data.HumanDoor != new Point(-1, -1))
      {
        Vector2 tileLocation1 = tileLocation + new Vector2((float) data.HumanDoor.X, (float) (data.HumanDoor.Y + 1));
        if (!this.isBuildable(tileLocation1, true) && !this.isPath(tileLocation1))
        {
          constructed = (Building) null;
          return false;
        }
      }
    }
    Building instanceFromId = Building.CreateInstanceFromId(typeId, tileLocation);
    if (magicalConstruction)
    {
      instanceFromId.magical.Value = true;
      instanceFromId.daysOfConstructionLeft.Value = 0;
    }
    instanceFromId.owner.Value = who.UniqueMultiplayerID;
    if (!skipSafetyChecks)
    {
      string message = instanceFromId.isThereAnythingtoPreventConstruction(this, tileLocation);
      if (message != null)
      {
        Game1.addHUDMessage(new HUDMessage(message, 3));
        constructed = (Building) null;
        return false;
      }
    }
    for (int index5 = 0; index5 < instanceFromId.tilesHigh.Value; ++index5)
    {
      for (int index6 = 0; index6 < instanceFromId.tilesWide.Value; ++index6)
      {
        Vector2 key = new Vector2(tileLocation.X + (float) index6, tileLocation.Y + (float) index5);
        if (this.terrainFeatures.GetValueOrDefault(key) is Flooring)
        {
          bool? flooringUnderneath = instanceFromId.GetData()?.AllowsFlooringUnderneath;
          if (flooringUnderneath.HasValue && flooringUnderneath.GetValueOrDefault())
            continue;
        }
        this.terrainFeatures.Remove(key);
      }
    }
    this.buildings.Add(instanceFromId);
    who.team.SendBuildingConstructedEvent(this, instanceFromId, who);
    string messageKey = magicalConstruction ? "BuildingMagicBuild" : "BuildingBuild";
    Game1.multiplayer.globalChatInfoMessage(messageKey, Game1.player.Name, "aOrAn:" + data.Name, data.Name, Game1.player.farmName.Value);
    constructed = instanceFromId;
    return true;
  }

  /// <summary>Construct a building in the location.</summary>
  /// <param name="typeId">The building type ID in <c>Data/Buildings</c>.</param>
  /// <param name="tileLocation">The top-left tile position of the building.</param>
  /// <param name="who">The player constructing the building.</param>
  /// <param name="magicalConstruction">Whether construction should complete instantly.</param>
  /// <param name="skipSafetyChecks">Whether to ignore safety checks (e.g. making sure the area is clear).</param>
  /// <returns>Returns whether the building was successfully placed.</returns>
  public bool buildStructure(
    string typeId,
    Vector2 tileLocation,
    Farmer who,
    out Building constructed,
    bool magicalConstruction = false,
    bool skipSafetyChecks = false)
  {
    BuildingData data;
    if (typeId != null && Game1.buildingData.TryGetValue(typeId, out data))
      return this.buildStructure(typeId, data, tileLocation, who, out constructed, magicalConstruction, skipSafetyChecks);
    Game1.log.Error($"Can't construct building '{typeId}', no data found matching that ID.");
    constructed = (Building) null;
    return false;
  }

  /// <summary>Get whether the location contains any buildings of the given type.</summary>
  /// <param name="name">The building type's ID in <c>Data/Buildings</c>.</param>
  public bool isBuildingConstructed(string name) => this.getNumberBuildingsConstructed(name) > 0;

  /// <summary>Get whether the location has a minimum number of matching buildings.</summary>
  /// <param name="buildingType">The building type to count.</param>
  /// <param name="minCount">The minimum number needed.</param>
  public bool HasMinBuildings(string buildingType, int minCount)
  {
    return this.getNumberBuildingsConstructed(buildingType) >= minCount;
  }

  /// <summary>Get whether the location has a minimum number of matching buildings.</summary>
  /// <param name="match">A filter which matches buildings to count.</param>
  /// <param name="minCount">The minimum number needed.</param>
  public bool HasMinBuildings(Func<Building, bool> match, int minCount)
  {
    if (minCount <= 0)
      return true;
    int num = 0;
    foreach (Building building in this.buildings)
    {
      if (match(building))
        ++num;
      if (num >= minCount)
        return true;
    }
    return false;
  }

  public int getNumberBuildingsConstructed(bool includeUnderConstruction = false)
  {
    if (includeUnderConstruction || this.buildings.Count == 0)
      return this.buildings.Count;
    int buildingsConstructed = 0;
    foreach (Building building in this.buildings)
    {
      if (!building.isUnderConstruction())
        ++buildingsConstructed;
    }
    return buildingsConstructed;
  }

  public int getNumberBuildingsConstructed(string name, bool includeUnderConstruction = false)
  {
    int buildingsConstructed = 0;
    if (this.buildings.Count > 0)
    {
      foreach (Building building in this.buildings)
      {
        if (building.buildingType.Value == name && (includeUnderConstruction || !building.isUnderConstruction()))
          ++buildingsConstructed;
      }
    }
    return buildingsConstructed;
  }

  public bool isThereABuildingUnderConstruction()
  {
    if (this.buildings.Count > 0)
    {
      foreach (Building building in this.buildings)
      {
        if (building.isUnderConstruction())
          return true;
      }
    }
    return false;
  }

  /// <summary>Get all building interiors within this location which are instanced to the building (i.e. not in <see cref="P:StardewValley.Game1.locations" /> separately).</summary>
  public IEnumerable<GameLocation> GetInstancedBuildingInteriors()
  {
    List<GameLocation> interiors = (List<GameLocation>) null;
    this.ForEachInstancedInterior((Func<GameLocation, bool>) (location =>
    {
      if (interiors == null)
        interiors = new List<GameLocation>();
      interiors.Add(location);
      return true;
    }));
    return interiors == null ? (IEnumerable<GameLocation>) LegacyShims.EmptyArray<GameLocation>() : (IEnumerable<GameLocation>) interiors;
  }

  /// <summary>Perform an action for each building interior within this location which is instanced to the building (i.e. not in <see cref="P:StardewValley.Game1.locations" /> separately).</summary>
  /// <param name="action">The action to perform for each interior. This should return true (continue iterating) or false (stop).</param>
  public void ForEachInstancedInterior(Func<GameLocation, bool> action)
  {
    foreach (Building building in this.buildings)
    {
      if (building.GetIndoorsType() == IndoorsType.Instanced)
      {
        GameLocation indoors = building.GetIndoors();
        if (indoors != null && !action(indoors))
          break;
      }
    }
  }

  /// <summary>Perform an action for each tilled dirt in the location.</summary>
  /// <param name="action">The action to perform.</param>
  /// <param name="includeGardenPots">Whether to apply the action to dirt in garden pots.</param>
  public void ForEachDirt(Func<HoeDirt, bool> action, bool includeGardenPots = true)
  {
    foreach (TerrainFeature terrainFeature in this.terrainFeatures.Values)
    {
      if (terrainFeature is HoeDirt hoeDirt && !action(hoeDirt))
        return;
    }
    if (!includeGardenPots)
      return;
    using (Dictionary<Vector2, Object>.ValueCollection.Enumerator enumerator = this.objects.Values.GetEnumerator())
    {
      do
        ;
      while (enumerator.MoveNext() && (!(enumerator.Current is IndoorPot current) || current.bush.Value != null || action(current.hoeDirt.Value)));
    }
  }

  public bool isPath(Vector2 tileLocation)
  {
    TerrainFeature terrainFeature;
    if (!this.terrainFeatures.TryGetValue(tileLocation, out terrainFeature) || terrainFeature == null || !terrainFeature.isPassable())
      return false;
    Object @object;
    return !this.objects.TryGetValue(tileLocation, out @object) || @object == null || @object.isPassable();
  }

  public bool isBuildable(Vector2 tileLocation, bool onlyNeedsToBePassable = false)
  {
    Microsoft.Xna.Framework.Rectangle buildableRectangle = this.GetBuildableRectangle();
    if (buildableRectangle != Microsoft.Xna.Framework.Rectangle.Empty && !buildableRectangle.Contains((int) tileLocation.X, (int) tileLocation.Y))
      return false;
    if (onlyNeedsToBePassable)
      return this.isTilePassable(tileLocation) && !this.IsTileOccupiedBy(tileLocation, ignorePassables: CollisionMask.All);
    Building buildingAt = this.getBuildingAt(tileLocation);
    if (buildingAt != null && !buildingAt.isMoving || !this.CanItemBePlacedHere(tileLocation, useFarmerTile: true) && !(this.getObjectAtTile((int) tileLocation.X, (int) tileLocation.Y)?.QualifiedItemId == "(O)590"))
      return false;
    if (this._looserBuildRestrictions)
      return !Game1.currentLocation.doesTileHavePropertyNoNull((int) tileLocation.X, (int) tileLocation.Y, "Buildable", "Back").EqualsIgnoreCase("f");
    return Game1.currentLocation.doesTileHavePropertyNoNull((int) tileLocation.X, (int) tileLocation.Y, "Buildable", "Back").EqualsIgnoreCase("t") || Game1.currentLocation.doesTileHavePropertyNoNull((int) tileLocation.X, (int) tileLocation.Y, "Buildable", "Back").ToLower().Equals("true") || Game1.currentLocation.doesTileHaveProperty((int) tileLocation.X, (int) tileLocation.Y, "Diggable", "Back") != null && !Game1.currentLocation.doesTileHavePropertyNoNull((int) tileLocation.X, (int) tileLocation.Y, "Buildable", "Back").EqualsIgnoreCase("f");
  }

  public virtual void pokeTileForConstruction(Vector2 tile)
  {
    foreach (FarmAnimal farmAnimal in this.animals.Values)
    {
      if (farmAnimal.Tile == tile)
        farmAnimal.Poke();
    }
  }

  public virtual void updateWarps()
  {
    if (Game1.IsClient)
      return;
    this.warps.Clear();
    string[] strArray = new string[2]{ "NPCWarp", "Warp" };
    foreach (string key in strArray)
    {
      string str;
      if (this.map.Properties.TryGetValue(key, out str) && str != null)
      {
        bool npcOnly = key == "NPCWarp";
        string[] source = ArgUtility.SplitBySpace(str);
        for (int count = 0; count < source.Length; count += 5)
        {
          bool flag = source.Length >= count + 5;
          int result1;
          int result2;
          int result3;
          int result4;
          if (!flag || !int.TryParse(source[count], out result1) || !int.TryParse(source[count + 1], out result2) || !int.TryParse(source[count + 3], out result3) || !int.TryParse(source[count + 4], out result4))
            Game1.log.Warn($"Failed parsing {(npcOnly ? "NPC warp" : "warp")} '{string.Join(" ", ((IEnumerable<string>) source).Skip<string>(count))}' for location '{this.NameOrUniqueName}'. Warps must have five fields in the form 'fromX fromY toLocationName toX toY', but " + (!flag ? "got insufficient fields." : "got a non-numeric value for one of the X/Y position fields."));
          else
            this.warps.Add(new Warp(result1, result2, source[count + 2], result3, result4, false, npcOnly));
        }
      }
    }
    if (this.warps.Count <= 0)
      return;
    this.ParentBuilding?.updateInteriorWarps(this);
  }

  public void loadWeeds()
  {
    if (!this.isOutdoors.Value && !this.treatAsOutdoors.Value)
      return;
    Layer layer = this.map?.GetLayer("Paths");
    if (layer == null)
      return;
    for (int x = 0; x < this.map.Layers[0].LayerWidth; ++x)
    {
      for (int y = 0; y < this.map.Layers[0].LayerHeight; ++y)
      {
        int tileIndexAt = layer.GetTileIndexAt(x, y);
        if (tileIndexAt != -1)
        {
          Vector2 vector2 = new Vector2((float) x, (float) y);
          switch (tileIndexAt - 13)
          {
            case 0:
            case 1:
            case 2:
              if (this.CanLoadPathObjectHere(vector2))
              {
                this.objects.Add(vector2, ItemRegistry.Create<Object>(GameLocation.getWeedForSeason(Game1.random, this.GetSeason())));
                continue;
              }
              continue;
            case 3:
              if (this.CanLoadPathObjectHere(vector2))
              {
                this.objects.Add(vector2, ItemRegistry.Create<Object>(Game1.random.Choose<string>("(O)343", "(O)450")));
                continue;
              }
              continue;
            case 4:
              if (this.CanLoadPathObjectHere(vector2))
              {
                this.objects.Add(vector2, ItemRegistry.Create<Object>(Game1.random.Choose<string>("(O)343", "(O)450")));
                continue;
              }
              continue;
            case 5:
              if (this.CanLoadPathObjectHere(vector2))
              {
                this.objects.Add(vector2, ItemRegistry.Create<Object>(Game1.random.Choose<string>("(O)294", "(O)295")));
                continue;
              }
              continue;
            default:
              continue;
          }
        }
      }
    }
  }

  /// <summary>Get whether a tile is unoccupied for the purposes of spawning weed/stone debris from the <c>Paths</c> layer.</summary>
  /// <param name="tile">The tile position.</param>
  public bool CanLoadPathObjectHere(Vector2 tile)
  {
    if (this.IsTileOccupiedBy(tile, CollisionMask.Buildings | CollisionMask.Flooring | CollisionMask.Objects | CollisionMask.TerrainFeatures))
      return false;
    Vector2 vector2 = tile * 64f;
    vector2.X += 32f;
    vector2.Y += 32f;
    foreach (Furniture furniture in this.furniture)
    {
      if (furniture.furniture_type.Value != 12 && !furniture.isPassable() && furniture.GetBoundingBox().Contains((int) vector2.X, (int) vector2.Y) && !furniture.AllowPlacementOnThisTile((int) tile.X, (int) tile.Y))
        return false;
    }
    return true;
  }

  public void loadObjects()
  {
    this._startingCabinLocations.Clear();
    if (this.map == null)
      return;
    this.updateWarps();
    Layer layer = this.map.GetLayer("Paths");
    string[] propertySplitBySpaces = this.GetMapPropertySplitBySpaces("Trees");
    for (int index = 0; index < propertySplitBySpaces.Length; index += 3)
    {
      Vector2 key;
      string error;
      int num;
      if (!ArgUtility.TryGetVector2(propertySplitBySpaces, index, out key, out error, name: "Vector2 position") || !ArgUtility.TryGetInt(propertySplitBySpaces, index + 2, out num, out error, "int treeType"))
        this.LogMapPropertyError("Trees", propertySplitBySpaces, error);
      else
        this.terrainFeatures.Add(key, (TerrainFeature) new Tree((num + 1).ToString(), 5));
    }
    string propertyValue;
    if (layer != null && this.TryGetMapProperty("LoadTreesFrom", out propertyValue))
    {
      GameLocation locationFromName = Game1.getLocationFromName(propertyValue);
      if (locationFromName != null)
      {
        foreach (KeyValuePair<Vector2, TerrainFeature> pair in locationFromName.terrainFeatures.Pairs)
        {
          if (pair.Value is Tree tree)
          {
            Point point = new Point((int) pair.Key.X, (int) pair.Key.Y);
            if (layer.HasTileAt(point.X, point.Y) && this.TryGetTreeIdForTile(layer.Tiles[point.X, point.Y], out string _, out int? _, out int? _, out bool _))
              this.terrainFeatures.Add(pair.Key, (TerrainFeature) new Tree(tree.treeType.Value, tree.growthStage.Value));
          }
        }
      }
    }
    if ((this.isOutdoors.Value || this.name.Equals((object) "BathHouse_Entry") || this.treatAsOutdoors.Value || this.map.Properties.ContainsKey("forceLoadObjects")) && layer != null)
    {
      this.loadPathsLayerObjectsInArea(0, 0, this.map.Layers[0].LayerWidth, this.map.Layers[0].LayerHeight);
      if (!Game1.eventUp && this.HasMapPropertyWithValue(this.GetSeason().ToString() + "_Objects"))
        this.spawnObjects();
    }
    this.updateDoors();
  }

  public void loadPathsLayerObjectsInArea(int startingX, int startingY, int width, int height)
  {
    Layer layer = this.map.GetLayer("Paths");
    for (int x = startingX; x < startingX + width; ++x)
    {
      for (int y = startingY; y < startingY + height; ++y)
      {
        Tile tile = layer.Tiles[x, y];
        if (tile != null)
        {
          Vector2 vector2 = new Vector2((float) x, (float) y);
          string treeId;
          int? growthStageOnLoad;
          bool isFruitTree;
          if (this.TryGetTreeIdForTile(tile, out treeId, out growthStageOnLoad, out int? _, out isFruitTree))
          {
            if (this.GetFurnitureAt(vector2) == null && !this.terrainFeatures.ContainsKey(vector2) && !this.objects.ContainsKey(vector2))
            {
              if (isFruitTree)
                this.terrainFeatures.Add(vector2, (TerrainFeature) new FruitTree(treeId, growthStageOnLoad ?? 4));
              else
                this.terrainFeatures.Add(vector2, (TerrainFeature) new Tree(treeId, growthStageOnLoad ?? 5));
            }
          }
          else
          {
            switch (tile.TileIndex)
            {
              case 13:
              case 14:
              case 15:
                if (!this.objects.ContainsKey(vector2) && (!this.IsOutdoors || !Game1.IsWinter))
                {
                  this.objects.Add(vector2, ItemRegistry.Create<Object>(GameLocation.getWeedForSeason(Game1.random, this.GetSeason())));
                  continue;
                }
                continue;
              case 16 /*0x10*/:
                if (!this.objects.ContainsKey(vector2))
                {
                  this.objects.Add(vector2, ItemRegistry.Create<Object>(Game1.random.Choose<string>("(O)343", "(O)450")));
                  continue;
                }
                continue;
              case 17:
                if (!this.objects.ContainsKey(vector2))
                {
                  this.objects.Add(vector2, ItemRegistry.Create<Object>(Game1.random.Choose<string>("(O)343", "(O)450")));
                  continue;
                }
                continue;
              case 18:
                if (!this.objects.ContainsKey(vector2))
                {
                  this.objects.Add(vector2, ItemRegistry.Create<Object>(Game1.random.Choose<string>("(O)294", "(O)295")));
                  continue;
                }
                continue;
              case 19:
                this.addResourceClumpAndRemoveUnderlyingTerrain(602, 2, 2, vector2);
                continue;
              case 20:
                this.addResourceClumpAndRemoveUnderlyingTerrain(672, 2, 2, vector2);
                continue;
              case 21:
                this.addResourceClumpAndRemoveUnderlyingTerrain(600, 2, 2, vector2);
                continue;
              case 22:
              case 36:
                if (!this.terrainFeatures.ContainsKey(vector2))
                {
                  Microsoft.Xna.Framework.Rectangle rectangle = new Microsoft.Xna.Framework.Rectangle((int) vector2.X * 64 /*0x40*/, (int) vector2.Y * 64 /*0x40*/, 64 /*0x40*/, 64 /*0x40*/);
                  rectangle.Inflate(-1, -1);
                  bool flag = false;
                  foreach (TerrainFeature resourceClump in this.resourceClumps)
                  {
                    if (resourceClump.getBoundingBox().Intersects(rectangle))
                    {
                      flag = true;
                      break;
                    }
                  }
                  if (!flag)
                  {
                    this.terrainFeatures.Add(vector2, (TerrainFeature) new Grass(tile.TileIndex == 36 ? 7 : 1, 3));
                    continue;
                  }
                  continue;
                }
                continue;
              case 23:
                if (!this.terrainFeatures.ContainsKey(vector2))
                {
                  this.terrainFeatures.Add(vector2, (TerrainFeature) new Tree(Game1.random.Next(1, 4).ToString(), Game1.random.Next(2, 4)));
                  continue;
                }
                continue;
              case 24:
                if (!this.terrainFeatures.ContainsKey(vector2))
                {
                  this.largeTerrainFeatures.Add((LargeTerrainFeature) new StardewValley.TerrainFeatures.Bush(vector2, 2, this));
                  continue;
                }
                continue;
              case 25:
                if (!this.terrainFeatures.ContainsKey(vector2))
                {
                  this.largeTerrainFeatures.Add((LargeTerrainFeature) new StardewValley.TerrainFeatures.Bush(vector2, 1, this));
                  continue;
                }
                continue;
              case 26:
                if (!this.terrainFeatures.ContainsKey(vector2))
                {
                  this.largeTerrainFeatures.Add((LargeTerrainFeature) new StardewValley.TerrainFeatures.Bush(vector2, 0, this));
                  continue;
                }
                continue;
              case 27:
                this.changeMapProperties("BrookSounds", $"{vector2.X.ToString()} {vector2.Y.ToString()} 0");
                continue;
              case 29:
              case 30:
                string s;
                if (Game1.startingCabins > 0 && tile.Properties.TryGetValue("Order", out s) && int.Parse(s) <= Game1.startingCabins && (tile.TileIndex == 29 && !Game1.cabinsSeparate || tile.TileIndex == 30 && Game1.cabinsSeparate))
                {
                  this._startingCabinLocations.Add(vector2);
                  continue;
                }
                continue;
              case 33:
                if (!this.terrainFeatures.ContainsKey(vector2))
                {
                  this.largeTerrainFeatures.Add((LargeTerrainFeature) new StardewValley.TerrainFeatures.Bush(vector2, 4, this));
                  continue;
                }
                continue;
              default:
                continue;
            }
          }
        }
      }
    }
  }

  /// <summary>Get the tree to spawn on a tile based on its tile index on the <c>Paths</c> layer, if any.</summary>
  /// <param name="tileIndex">The tile index on the <c>Paths</c> layer.</param>
  /// <param name="treeId">The tree ID in <c>Data/FruitTrees</c> or <c>Data/WildTrees</c> that should spawn.</param>
  /// <param name="growthStageOnLoad">The preferred tree growth stage when first populating the save, if applicable.</param>
  /// <param name="growthStageOnRegrow">The preferred tree growth stage when regrowing trees on day update, if applicable.</param>
  /// <param name="isFruitTree">Whether to spawn a fruit tree (<c>true</c>) or wild tree (<c>false</c>).</param>
  /// <returns>Returns whether a tree should spawn here.</returns>
  public bool TryGetTreeIdForTile(
    Tile tile,
    out string treeId,
    out int? growthStageOnLoad,
    out int? growthStageOnRegrow,
    out bool isFruitTree)
  {
    isFruitTree = false;
    growthStageOnLoad = new int?();
    growthStageOnRegrow = new int?();
    if (tile == null)
    {
      treeId = (string) null;
      return false;
    }
    switch (tile.TileIndex)
    {
      case 9:
        treeId = this.IsWinterHere() ? "4" : "1";
        return true;
      case 10:
        treeId = this.IsWinterHere() ? "5" : "2";
        return true;
      case 11:
        treeId = "3";
        return true;
      case 12:
        treeId = "6";
        return true;
      case 31 /*0x1F*/:
        treeId = "9";
        return true;
      case 32 /*0x20*/:
        treeId = "8";
        return true;
      case 34:
        string str1;
        if (!tile.Properties.TryGetValue("SpawnTree", out str1))
        {
          Game1.log.Warn($"Location '{this.NameOrUniqueName}' ignored path tile index 34 (spawn tree) at position {tile} because the tile has no '{"SpawnTree"}' tile property.");
          break;
        }
        string[] array = ArgUtility.SplitBySpace(str1);
        string str2;
        string error;
        string str3;
        int num1;
        int num2;
        if (!ArgUtility.TryGet(array, 0, out str2, out error, name: "string rawType") || !ArgUtility.TryGet(array, 1, out str3, out error, name: "string rawId") || !ArgUtility.TryGetOptionalInt(array, 2, out num1, out error, -1, "int rawGrowthStageOnLoad") || !ArgUtility.TryGetOptionalInt(array, 3, out num2, out error, -1, "int rawGrowthStageOnRegrow"))
        {
          Game1.log.Warn($"Location '{this.NameOrUniqueName}' ignored path tile index 34 (spawn tree) at position {tile} because the '{"SpawnTree"}' tile property is invalid: {error}.");
          break;
        }
        if (num1 > -1)
          growthStageOnLoad = new int?(num1);
        if (num2 > -1)
          growthStageOnRegrow = new int?(num2);
        if (str2.EqualsIgnoreCase("wild"))
        {
          treeId = str3;
          return true;
        }
        if (str2.EqualsIgnoreCase("fruit"))
        {
          treeId = str3;
          isFruitTree = true;
          return true;
        }
        Game1.log.Warn($"Location '{this.NameOrUniqueName}' ignored path tile index 34 (spawn tree) at position {tile} because the '{"SpawnTree"}' tile property has invalid type '{str2}' (expected 'fruit' or 'wild').");
        break;
    }
    growthStageOnLoad = new int?();
    growthStageOnRegrow = new int?();
    treeId = (string) null;
    return false;
  }

  public void BuildStartingCabins()
  {
    if (this._startingCabinLocations.Count > 0)
    {
      List<string> stringList = new List<string>();
      switch (Game1.whichFarm)
      {
        case 1:
          stringList.Add("Beach Cabin");
          stringList.Add("Plank Cabin");
          stringList.Add("Log Cabin");
          stringList.Add("Neighbor Cabin");
          stringList.Add("Trailer Cabin");
          stringList.Add("Stone Cabin");
          stringList.Add("Rustic Cabin");
          break;
        case 3:
        case 4:
          stringList.Add("Stone Cabin");
          stringList.Add("Log Cabin");
          stringList.Add("Plank Cabin");
          stringList.Add("Rustic Cabin");
          stringList.Add("Trailer Cabin");
          stringList.Add("Neighbor Cabin");
          stringList.Add("Beach Cabin");
          break;
        default:
          bool flag = Game1.random.NextBool();
          stringList.Add(flag ? "Log Cabin" : "Plank Cabin");
          stringList.Add("Stone Cabin");
          stringList.Add(flag ? "Plank Cabin" : "Log Cabin");
          stringList.Add("Trailer Cabin");
          stringList.Add("Neighbor Cabin");
          stringList.Add("Rustic Cabin");
          stringList.Add("Beach Cabin");
          break;
      }
      List<Vector2> vector2List = new List<Vector2>();
      for (int index1 = 0; index1 < this._startingCabinLocations.Count; ++index1)
      {
        for (int index2 = 0; index2 < this._startingCabinLocations.Count; ++index2)
        {
          if (this.doesTileHavePropertyNoNull((int) this._startingCabinLocations[index2].X, (int) this._startingCabinLocations[index2].Y, "Order", "Paths").Equals((index1 + 1).ToString() ?? ""))
            vector2List.Add(this._startingCabinLocations[index2]);
        }
      }
      for (int index = 0; index < vector2List.Count; ++index)
      {
        this.removeObjectsAndSpawned((int) vector2List[index].X, (int) vector2List[index].Y, 5, 3);
        this.removeObjectsAndSpawned((int) vector2List[index].X + 2, (int) vector2List[index].Y + 3, 1, 1);
        Building building = new Building("Cabin", vector2List[index]);
        building.magical.Value = true;
        building.skinId.Value = stringList[index % stringList.Count];
        building.daysOfConstructionLeft.Value = 0;
        building.load();
        this.buildStructure(building, vector2List[index], Game1.player, true);
        building.removeOverlappingBushes(this);
      }
    }
    this._startingCabinLocations.Clear();
  }

  public void updateDoors()
  {
    if (Game1.IsClient)
      return;
    this.doors.Clear();
    Layer layer = this.map.RequireLayer("Buildings");
    int y = 0;
    for (int layerHeight = layer.LayerHeight; y < layerHeight; ++y)
    {
      int x = 0;
      for (int layerWidth = layer.LayerWidth; x < layerWidth; ++x)
      {
        Tile tile = layer.Tiles[x, y];
        string str1;
        if (tile != null && tile.Properties.TryGetValue("Action", out str1) && str1.Contains("Warp"))
        {
          string[] array = ArgUtility.SplitBySpace(str1);
          string str2 = ArgUtility.Get(array, 0);
          if (str2 != null)
          {
            switch (str2.Length)
            {
              case 4:
                if (str2 == "Warp")
                  break;
                goto label_22;
              case 14:
                switch (str2[4])
                {
                  case 'B':
                    if (str2 == "WarpBoatTunnel")
                    {
                      this.doors.Add(new Point(x, y), new NetString("BoatTunnel"));
                      continue;
                    }
                    goto label_22;
                  case 'M':
                    if (str2 == "WarpMensLocker")
                      break;
                    goto label_22;
                  case 'e':
                    if (str2 == "LockedDoorWarp")
                      break;
                    goto label_22;
                  default:
                    goto label_22;
                }
                break;
              case 16 /*0x10*/:
                if (str2 == "WarpWomensLocker")
                  break;
                goto label_22;
              case 17:
                if (str2 == "Warp_Sunroom_Door")
                {
                  this.doors.Add(new Point(x, y), new NetString("Sunroom"));
                  continue;
                }
                goto label_22;
              case 19:
                if (str2 == "WarpCommunityCenter")
                {
                  this.doors.Add(new Point(x, y), new NetString("CommunityCenter"));
                  continue;
                }
                goto label_22;
              default:
                goto label_22;
            }
          }
          else
            goto label_22;
label_19:
          if (!(this.name.Value == "Mountain") || x != 8 || y != 20)
          {
            string str3 = ArgUtility.Get(array, 3);
            if (str3 != null)
            {
              this.doors.Add(new Point(x, y), new NetString(str3));
              continue;
            }
            continue;
          }
          continue;
label_22:
          if (str2.Contains("Warp"))
          {
            Game1.log.Warn($"{this.NameOrUniqueName} ({x}, {y}) has unknown warp property '{str1}', parsing with legacy logic.");
            goto label_19;
          }
        }
      }
    }
  }

  [Obsolete("Use removeObjectsAndSpawned instead.")]
  private void clearArea(int startingX, int startingY, int width, int height)
  {
    this.removeObjectsAndSpawned(startingX, startingY, width, height);
  }

  public bool isTerrainFeatureAt(int x, int y)
  {
    TerrainFeature terrainFeature;
    if (this.terrainFeatures.TryGetValue(new Vector2((float) x, (float) y), out terrainFeature) && !terrainFeature.isPassable())
      return true;
    if (this.largeTerrainFeatures != null)
    {
      Microsoft.Xna.Framework.Rectangle rectangle = new Microsoft.Xna.Framework.Rectangle(x * 64 /*0x40*/, y * 64 /*0x40*/, 64 /*0x40*/, 64 /*0x40*/);
      foreach (TerrainFeature largeTerrainFeature in this.largeTerrainFeatures)
      {
        if (largeTerrainFeature.getBoundingBox().Intersects(rectangle))
          return true;
      }
    }
    return false;
  }

  public void loadLights()
  {
    if (this.isOutdoors.Value && !Game1.isFestival() && !this.forceLoadPathLayerLights)
      return;
    switch (this)
    {
      case FarmHouse _:
        break;
      case IslandFarmHouse _:
        break;
      default:
        Layer layer1 = this.map.GetLayer("Paths");
        Layer layer2 = this.map.RequireLayer("Front");
        Layer layer3 = this.map.RequireLayer("Buildings");
        for (int x = 0; x < this.map.Layers[0].LayerWidth; ++x)
        {
          for (int y = 0; y < this.map.Layers[0].LayerHeight; ++y)
          {
            if (!this.isOutdoors.Value && !this.map.Properties.ContainsKey("IgnoreLightingTiles"))
            {
              int tileIndexAt1 = layer2.GetTileIndexAt(x, y);
              if (tileIndexAt1 != -1)
                this.adjustMapLightPropertiesForLamp(tileIndexAt1, x, y, "Front");
              int tileIndexAt2 = layer3.GetTileIndexAt(x, y);
              if (tileIndexAt2 != -1)
                this.adjustMapLightPropertiesForLamp(tileIndexAt2, x, y, "Buildings");
            }
            int tile = layer1 != null ? layer1.GetTileIndexAt(x, y) : -1;
            if (tile != -1)
              this.adjustMapLightPropertiesForLamp(tile, x, y, "Paths");
          }
        }
        break;
    }
  }

  public bool isFarmBuildingInterior() => this is AnimalHouse;

  /// <summary>Get whether this location is actively synced to the current player.</summary>
  /// <remarks>This is always true for the main player, and based on <see cref="M:StardewValley.Multiplayer.isActiveLocation(StardewValley.GameLocation)" /> for farmhands.</remarks>
  public bool IsActiveLocation()
  {
    if (Game1.IsMasterGame)
      return true;
    return this.Root?.Value != null && Game1.multiplayer.isActiveLocation(this);
  }

  public virtual bool CanBeRemotedlyViewed() => Game1.multiplayer.isAlwaysActiveLocation(this);

  protected void adjustMapLightPropertiesForLamp(int tile, int x, int y, string layer)
  {
    string tileSheetIdAt = this.getTileSheetIDAt(x, y, layer);
    if (this.isFarmBuildingInterior())
    {
      if (!(tileSheetIdAt == "Coop") && !(tileSheetIdAt == "barn"))
        return;
      switch (tile)
      {
        case 24:
          this.changeMapProperties("DayTiles", $"{layer} {x.ToString()} {y.ToString()} {tile.ToString()}");
          string[] strArray = new string[7]
          {
            layer,
            " ",
            x.ToString(),
            " ",
            y.ToString(),
            " ",
            null
          };
          int num = 26;
          strArray[6] = num.ToString();
          this.changeMapProperties("NightTiles", string.Concat(strArray));
          string str1 = x.ToString();
          num = y + 1;
          string str2 = num.ToString();
          this.changeMapProperties("WindowLight", $"{str1} {str2} 4");
          string str3 = x.ToString();
          num = y + 3;
          string str4 = num.ToString();
          this.changeMapProperties("WindowLight", $"{str3} {str4} 4");
          break;
        case 25:
          this.changeMapProperties("DayTiles", $"{layer} {x.ToString()} {y.ToString()} {tile.ToString()}");
          this.changeMapProperties("NightTiles", $"{layer} {x.ToString()} {y.ToString()} {12.ToString()}");
          break;
        case 46:
          this.changeMapProperties("DayTiles", $"{layer} {x.ToString()} {y.ToString()} {tile.ToString()}");
          this.changeMapProperties("NightTiles", $"{layer} {x.ToString()} {y.ToString()} {53.ToString()}");
          break;
      }
    }
    else if (tile == 8 && layer == "Paths")
    {
      this.changeMapProperties("Light", $"{x.ToString()} {y.ToString()} 4");
    }
    else
    {
      if (!(tileSheetIdAt == "indoor"))
        return;
      switch (tile)
      {
        case 225:
          if (this.name.Value.Contains("BathHouse") || this.name.Value.Contains("Club") || this.name.Equals((object) "SeedShop") && (x == 36 || x == 37))
            break;
          this.changeMapProperties("DayTiles", $"{layer} {x.ToString()} {y.ToString()} {tile.ToString()}");
          string[] strArray1 = new string[7]
          {
            layer,
            " ",
            x.ToString(),
            " ",
            y.ToString(),
            " ",
            null
          };
          int num1 = 1222;
          strArray1[6] = num1.ToString();
          this.changeMapProperties("NightTiles", string.Concat(strArray1));
          string[] strArray2 = new string[7]
          {
            layer,
            " ",
            x.ToString(),
            " ",
            null,
            null,
            null
          };
          num1 = y + 1;
          strArray2[4] = num1.ToString();
          strArray2[5] = " ";
          num1 = 257;
          strArray2[6] = num1.ToString();
          this.changeMapProperties("DayTiles", string.Concat(strArray2));
          this.changeMapProperties("NightTiles", $"{layer} {x.ToString()} {(y + 1).ToString()} {1254.ToString()}");
          this.changeMapProperties("WindowLight", $"{x.ToString()} {y.ToString()} 4");
          this.changeMapProperties("WindowLight", $"{x.ToString()} {(y + 1).ToString()} 4");
          break;
        case 256 /*0x0100*/:
          this.changeMapProperties("DayTiles", $"{layer} {x.ToString()} {y.ToString()} {tile.ToString()}");
          string[] strArray3 = new string[7]
          {
            layer,
            " ",
            x.ToString(),
            " ",
            y.ToString(),
            " ",
            null
          };
          int num2 = 1253;
          strArray3[6] = num2.ToString();
          this.changeMapProperties("NightTiles", string.Concat(strArray3));
          string[] strArray4 = new string[7]
          {
            layer,
            " ",
            x.ToString(),
            " ",
            null,
            null,
            null
          };
          num2 = y + 1;
          strArray4[4] = num2.ToString();
          strArray4[5] = " ";
          num2 = 288;
          strArray4[6] = num2.ToString();
          this.changeMapProperties("DayTiles", string.Concat(strArray4));
          this.changeMapProperties("NightTiles", $"{layer} {x.ToString()} {(y + 1).ToString()} {1285.ToString()}");
          this.changeMapProperties("WindowLight", $"{x.ToString()} {y.ToString()} 4");
          this.changeMapProperties("WindowLight", $"{x.ToString()} {(y + 1).ToString()} 4");
          break;
        case 480:
          this.changeMapProperties("DayTiles", $"{layer} {x.ToString()} {y.ToString()} {tile.ToString()}");
          this.changeMapProperties("NightTiles", $"{layer} {x.ToString()} {y.ToString()} {809.ToString()}");
          this.changeMapProperties("Light", $"{x.ToString()} {y.ToString()} 4");
          break;
        case 826:
          this.changeMapProperties("DayTiles", $"{layer} {x.ToString()} {y.ToString()} {tile.ToString()}");
          this.changeMapProperties("NightTiles", $"{layer} {x.ToString()} {y.ToString()} {827.ToString()}");
          this.changeMapProperties("Light", $"{x.ToString()} {y.ToString()} 4");
          break;
        case 1344:
          this.changeMapProperties("DayTiles", $"{layer} {x.ToString()} {y.ToString()} {tile.ToString()}");
          this.changeMapProperties("NightTiles", $"{layer} {x.ToString()} {y.ToString()} {1345.ToString()}");
          this.changeMapProperties("Light", $"{x.ToString()} {y.ToString()} 4");
          break;
        case 1346:
          this.changeMapProperties("DayTiles", $"Front {x.ToString()} {y.ToString()} {tile.ToString()}");
          string[] strArray5 = new string[6]
          {
            "Front ",
            x.ToString(),
            " ",
            y.ToString(),
            " ",
            null
          };
          int num3 = 1347;
          strArray5[5] = num3.ToString();
          this.changeMapProperties("NightTiles", string.Concat(strArray5));
          string[] strArray6 = new string[6]
          {
            "Buildings ",
            x.ToString(),
            " ",
            null,
            null,
            null
          };
          num3 = y + 1;
          strArray6[3] = num3.ToString();
          strArray6[4] = " ";
          num3 = 452;
          strArray6[5] = num3.ToString();
          this.changeMapProperties("DayTiles", string.Concat(strArray6));
          string[] strArray7 = new string[6]
          {
            "Buildings ",
            x.ToString(),
            " ",
            null,
            null,
            null
          };
          num3 = y + 1;
          strArray7[3] = num3.ToString();
          strArray7[4] = " ";
          num3 = 453;
          strArray7[5] = num3.ToString();
          this.changeMapProperties("NightTiles", string.Concat(strArray7));
          this.changeMapProperties("Light", $"{x.ToString()} {y.ToString()} 4");
          break;
      }
    }
  }

  private void changeMapProperties(string propertyName, string toAdd)
  {
    try
    {
      string str1;
      if (!this.map.Properties.TryGetValue(propertyName, out str1))
      {
        this.map.Properties[propertyName] = new PropertyValue(toAdd);
      }
      else
      {
        if (str1.Contains(toAdd))
          return;
        string str2 = $"{str1} {toAdd}";
        this.map.Properties[propertyName] = new PropertyValue(str2);
      }
    }
    catch
    {
    }
  }

  /// <summary>Log an error indicating that a map property could not be parsed.</summary>
  /// <param name="name">The name of the property that failed to parse.</param>
  /// <param name="value">The property value that failed to parse.</param>
  /// <param name="error">The error phrase indicating why it failed.</param>
  public void LogMapPropertyError(string name, string value, string error)
  {
    Game1.log.Error($"Can't parse map property '{name}' with value '{value}' in location '{this.NameOrUniqueName}': {error}.");
  }

  /// <summary>Log an error indicating that a map property could not be parsed.</summary>
  /// <param name="name">The name of the property that failed to parse.</param>
  /// <param name="value">The property value that failed to parse.</param>
  /// <param name="error">The error phrase indicating why it failed.</param>
  /// <param name="delimiter">The character used to delimit values in the property.</param>
  public void LogMapPropertyError(string name, string[] value, string error, char delimiter = ' ')
  {
    this.LogMapPropertyError(name, string.Join(delimiter, value), error);
  }

  /// <summary>Log an error indicating that a tile property could not be parsed.</summary>
  /// <param name="name">The name of the property that failed to parse.</param>
  /// <param name="layerId">The layer containing the tile.</param>
  /// <param name="x">The X tile position of the tile.</param>
  /// <param name="y">The Y tile position of the tile.</param>
  /// <param name="value">The property value that failed to parse.</param>
  /// <param name="error">The error phrase indicating why it failed.</param>
  public void LogTilePropertyError(
    string name,
    string layerId,
    int x,
    int y,
    string value,
    string error)
  {
    Game1.log.Error($"Can't parse tile property '{name}' at {layerId}:{x},{y} with value '{value}' in location '{this.NameOrUniqueName}': {error}.");
  }

  /// <summary>Log an error indicating that a tile property could not be parsed.</summary>
  /// <param name="name">The name of the property that failed to parse.</param>
  /// <param name="layerId">The layer containing the tile.</param>
  /// <param name="x">The X tile position of the tile.</param>
  /// <param name="y">The Y tile position of the tile.</param>
  /// <param name="value">The property value that failed to parse.</param>
  /// <param name="error">The error phrase indicating why it failed.</param>
  /// <param name="delimiter">The character used to delimit values in the property.</param>
  public void LogTilePropertyError(
    string name,
    string layerId,
    int x,
    int y,
    string[] value,
    string error,
    char delimiter = ' ')
  {
    this.LogTilePropertyError(name, layerId, x, y, string.Join(delimiter, value), error);
  }

  /// <summary>Log an error indicating that a tile <c>Action</c> property could not be parsed.</summary>
  /// <param name="action">The action arguments, including the <c>Action</c> prefix.</param>
  /// <param name="x">The tile X position containing the action.</param>
  /// <param name="y">The tile Y position containing the action.</param>
  /// <param name="error">The error phrase indicating why it failed.</param>
  public void LogTileActionError(string[] action, int x, int y, string error)
  {
    this.LogTilePropertyError("Action", "Buildings", x, y, action, error);
  }

  /// <summary>Log an error indicating that a tile <c>TouchAction</c> property could not be parsed.</summary>
  /// <param name="action">The action arguments, including the <c>TouchAction</c> prefix.</param>
  /// <param name="tile">The tile position containing the action.</param>
  /// <param name="error">The error phrase indicating why it failed.</param>
  public void LogTileTouchActionError(string[] action, Vector2 tile, string error)
  {
    this.LogTilePropertyError("TouchAction", "Back", (int) tile.X, (int) tile.Y, action, error);
  }

  public override bool Equals(object obj) => obj is GameLocation other && this.Equals(other);

  public bool Equals(GameLocation other)
  {
    return other != null && this.isStructure.Get() == other.isStructure.Get() && string.Equals(this.NameOrUniqueName, other.NameOrUniqueName, StringComparison.Ordinal);
  }

  public delegate void afterQuestionBehavior(Farmer who, string whichAnswer);

  /// <summary>A request to damage players who overlap a bounding box within the current location.</summary>
  private struct DamagePlayersEventArg : NetEventArg
  {
    /// <summary>The location pixel area where players will take damage.</summary>
    public Microsoft.Xna.Framework.Rectangle Area;
    /// <summary>The amount of damage the player should take.</summary>
    public int Damage;
    /// <summary>Whether the damage source was a bomb.</summary>
    public bool IsBomb;

    /// <summary>Reads the request data from a net-sync stream.</summary>
    /// <param name="reader">The binary stream to read.</param>
    public void Read(BinaryReader reader)
    {
      this.Area = reader.ReadRectangle();
      this.Damage = reader.ReadInt32();
      this.IsBomb = reader.ReadBoolean();
    }

    /// <summary>Writes the request data to a net-sync stream.</summary>
    /// <param name="writer">The binary stream to write to.</param>
    public void Write(BinaryWriter writer)
    {
      writer.WriteRectangle(this.Area);
      writer.Write(this.Damage);
      writer.Write(this.IsBomb);
    }
  }
}
