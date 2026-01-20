// Decompiled with JetBrains decompiler
// Type: StardewValley.Game1
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
using SkiaSharp;
using StardewValley.Audio;
using StardewValley.BellsAndWhistles;
using StardewValley.Buildings;
using StardewValley.Characters;
using StardewValley.Delegates;
using StardewValley.Enchantments;
using StardewValley.Events;
using StardewValley.Extensions;
using StardewValley.GameData;
using StardewValley.GameData.BigCraftables;
using StardewValley.GameData.Buildings;
using StardewValley.GameData.Characters;
using StardewValley.GameData.Crops;
using StardewValley.GameData.FarmAnimals;
using StardewValley.GameData.FloorsAndPaths;
using StardewValley.GameData.FruitTrees;
using StardewValley.GameData.LocationContexts;
using StardewValley.GameData.Locations;
using StardewValley.GameData.Objects;
using StardewValley.GameData.Pants;
using StardewValley.GameData.Pets;
using StardewValley.GameData.Shirts;
using StardewValley.GameData.Tools;
using StardewValley.GameData.Weapons;
using StardewValley.Hashing;
using StardewValley.Internal;
using StardewValley.Inventories;
using StardewValley.Locations;
using StardewValley.Logging;
using StardewValley.Menus;
using StardewValley.Minigames;
using StardewValley.Mods;
using StardewValley.Monsters;
using StardewValley.Network;
using StardewValley.Network.Dedicated;
using StardewValley.Network.NetReady;
using StardewValley.Objects;
using StardewValley.Pathfinding;
using StardewValley.Projectiles;
using StardewValley.Quests;
using StardewValley.SaveMigrations;
using StardewValley.SaveSerialization;
using StardewValley.SDKs.Steam;
using StardewValley.SpecialOrders;
using StardewValley.TerrainFeatures;
using StardewValley.TokenizableStrings;
using StardewValley.Tools;
using StardewValley.Triggers;
using StardewValley.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using xTile.Dimensions;
using xTile.Display;
using xTile.Layers;
using xTile.Tiles;

#nullable enable
namespace StardewValley;

/// <summary>This is the main type for your game</summary>
[InstanceStatics]
public class Game1 : InstanceGame
{
  public const bool IncrementalLoadEnabled = false;
  public const int defaultResolutionX = 1280 /*0x0500*/;
  public const int defaultResolutionY = 720;
  public const int pixelZoom = 4;
  public const int tileSize = 64 /*0x40*/;
  public const int smallestTileSize = 16 /*0x10*/;
  public const int up = 0;
  public const int right = 1;
  public const int down = 2;
  public const int left = 3;
  public const int dialogueBoxTileHeight = 5;
  public static int realMilliSecondsPerGameMinute = 700;
  public static int realMilliSecondsPerGameTenMinutes = Game1.realMilliSecondsPerGameMinute * 10;
  public const int rainDensity = 70;
  public const int rainLoopLength = 70;
  /// <summary>For <see cref="F:StardewValley.Game1.mouseCursor" />, a value indicating the cursor should be hidden.</summary>
  public static readonly int cursor_none = -1;
  /// <summary>For <see cref="F:StardewValley.Game1.mouseCursor" />, a default pointer icon.</summary>
  public static readonly int cursor_default = 0;
  /// <summary>For <see cref="F:StardewValley.Game1.mouseCursor" />, a wait icon.</summary>
  public static readonly int cursor_wait = 1;
  /// <summary>For <see cref="F:StardewValley.Game1.mouseCursor" />, a hand icon indicating that an item can be picked up.</summary>
  public static readonly int cursor_grab = 2;
  /// <summary>For <see cref="F:StardewValley.Game1.mouseCursor" />, a gift box icon indicating that an NPC on this tile can accept a gift.</summary>
  public static readonly int cursor_gift = 3;
  /// <summary>For <see cref="F:StardewValley.Game1.mouseCursor" />, a speech bubble icon indicating that an NPC can be talked to.</summary>
  public static readonly int cursor_talk = 4;
  /// <summary>For <see cref="F:StardewValley.Game1.mouseCursor" />, a magnifying glass icon indicating that something can be examined.</summary>
  public static readonly int cursor_look = 5;
  /// <summary>For <see cref="F:StardewValley.Game1.mouseCursor" />, an icon indicating that something can be harvested.</summary>
  public static readonly int cursor_harvest = 6;
  /// <summary>For <see cref="F:StardewValley.Game1.mouseCursor" />, a pointer icon used when hovering elements with gamepad controls.</summary>
  public static readonly int cursor_gamepad_pointer = 44;
  public static readonly 
  #nullable disable
  string asianSpacingRegexString = "\\s|[（《“‘「『(](?:[\\w,%％]+|[^…—])[々ぁぃぅぇぉっゃゅょゎゕゖァィゥェォッャュョヶー]*[）》”’」』)，。、？！：；·～,.?!:;~…]*|.[々ぁぃぅぇぉっゃゅょゎゕゖァィゥェォッャュョヶー]*[·・].[々ぁぃぅぇぉっゃゅょゎゕゖァィゥェォッャュョヶー]*|(?:[\\w,%％]+|[^…—])[々ぁぃぅぇぉっゃゅょゎゕゖァィゥェォッャュョヶー]*[）》”’」』)]?(?:[，。、？！：；·～,.?!:;~…]{1,2}[）》”’」』)]?)?|[\\w,%％]+|.[々ぁぃぅぇぉっゃゅょゎゕゖァィゥェォッャュョヶー]+|……|——|.";
  public const int legacy_weather_sunny = 0;
  public const int legacy_weather_rain = 1;
  public const int legacy_weather_debris = 2;
  public const int legacy_weather_lightning = 3;
  public const int legacy_weather_festival = 4;
  public const int legacy_weather_snow = 5;
  public const int legacy_weather_wedding = 6;
  public const string weather_sunny = "Sun";
  public const string weather_rain = "Rain";
  public const string weather_green_rain = "GreenRain";
  public const string weather_debris = "Wind";
  public const string weather_lightning = "Storm";
  public const string weather_festival = "Festival";
  public const string weather_snow = "Snow";
  public const string weather_wedding = "Wedding";
  /// <summary>The builder name for Robin's carpenter shop.</summary>
  public const string builder_robin = "Robin";
  /// <summary>The builder name for Wizard's magical construction shop.</summary>
  public const string builder_wizard = "Wizard";
  /// <summary>The shop ID for the Adventurer's Guild shop.</summary>
  public const string shop_adventurersGuild = "AdventureShop";
  /// <summary>The shop ID for the Adventurer's Guild item recovery shop.</summary>
  public const string shop_adventurersGuildItemRecovery = "AdventureGuildRecovery";
  /// <summary>The shop ID for Marnie's animal supply shop.</summary>
  public const string shop_animalSupplies = "AnimalShop";
  /// <summary>The shop ID for Clint's blacksmithery.</summary>
  public const string shop_blacksmith = "Blacksmith";
  /// <summary>The shop ID for Clint's tool upgrade shop.</summary>
  public const string shop_blacksmithUpgrades = "ClintUpgrade";
  /// <summary>The shop ID for the movie theater box office.</summary>
  public const string shop_boxOffice = "BoxOffice";
  /// <summary>The 'shop' ID for the floorpaper/wallpaper catalogue.</summary>
  public const string shop_catalogue = "Catalogue";
  /// <summary>The shop ID for Robin's carpenter supplies.</summary>
  public const string shop_carpenter = "Carpenter";
  /// <summary>The shop ID for the casino club shop.</summary>
  public const string shop_casino = "Casino";
  /// <summary>The shop ID for the desert trader.</summary>
  public const string shop_desertTrader = "DesertTrade";
  /// <summary>The shop ID for Dwarf's shop.</summary>
  public const string shop_dwarf = "Dwarf";
  /// <summary>The shop ID for Willy's fish shop.</summary>
  public const string shop_fish = "FishShop";
  /// <summary>The 'shop' ID for the furniture catalogue.</summary>
  public const string shop_furnitureCatalogue = "Furniture Catalogue";
  /// <summary>The shop ID for Pierre's General Store.</summary>
  public const string shop_generalStore = "SeedShop";
  /// <summary>The shop ID for the Hat Mouse shop.</summary>
  public const string shop_hatMouse = "HatMouse";
  /// <summary>The shop ID for Harvey's clinic.</summary>
  public const string shop_hospital = "Hospital";
  /// <summary>The shop ID for the ice-cream stand.</summary>
  public const string shop_iceCreamStand = "IceCreamStand";
  /// <summary>The shop ID for the island trader.</summary>
  public const string shop_islandTrader = "IslandTrade";
  /// <summary>The shop ID for Joja Mart.</summary>
  public const string shop_jojaMart = "Joja";
  /// <summary>The shop ID for Krobus' shop.</summary>
  public const string shop_krobus = "ShadowShop";
  /// <summary>The shop ID for Qi's gem shop.</summary>
  public const string shop_qiGemShop = "QiGemShop";
  /// <summary>The shop ID for the Ginger Island resort bar.</summary>
  public const string shop_resortBar = "ResortBar";
  /// <summary>The shop ID for Sandy's Oasis.</summary>
  public const string shop_sandy = "Sandy";
  /// <summary>The shop ID for the Stardrop Saloon.</summary>
  public const string shop_saloon = "Saloon";
  /// <summary>The shop ID for the traveling cart shop.</summary>
  public const string shop_travelingCart = "Traveler";
  /// <summary>The shop ID for the Volcano Dungeon shop.</summary>
  public const string shop_volcanoShop = "VolcanoShop";
  /// <summary>The shop ID for the bookseller.</summary>
  public const string shop_bookseller = "Bookseller";
  /// <summary>The shop ID for the bookseller trade-ins.</summary>
  public const string shop_bookseller_trade = "BooksellerTrade";
  /// <summary>The 'shop' ID for the joja furniture catalogue.</summary>
  public const string shop_jojaCatalogue = "JojaFurnitureCatalogue";
  /// <summary>The 'shop' ID for the wizard furniture catalogue.</summary>
  public const string shop_wizardCatalogue = "WizardFurnitureCatalogue";
  /// <summary>The 'shop' ID for the wizard furniture catalogue.</summary>
  public const string shop_junimoCatalogue = "JunimoFurnitureCatalogue";
  /// <summary>The 'shop' ID for the wizard furniture catalogue.</summary>
  public const string shop_retroCatalogue = "RetroFurnitureCatalogue";
  /// <summary>The 'shop' ID for the wizard furniture catalogue.</summary>
  public const string shop_trashCatalogue = "TrashFurnitureCatalogue";
  /// <summary>The shop ID for Marnie's pet adoption shop.</summary>
  public const string shop_petAdoption = "PetAdoption";
  public const byte singlePlayer = 0;
  public const byte multiplayerClient = 1;
  public const byte multiplayerServer = 2;
  public const byte logoScreenGameMode = 4;
  public const byte titleScreenGameMode = 0;
  public const byte loadScreenGameMode = 1;
  public const byte newGameMode = 2;
  public const byte playingGameMode = 3;
  public const byte loadingMode = 6;
  public const byte saveMode = 7;
  public const byte saveCompleteMode = 8;
  public const byte selectGameScreen = 9;
  public const byte creditsMode = 10;
  public const byte errorLogMode = 11;
  /// <summary>The name of the game's main assembly.</summary>
  public static readonly string GameAssemblyName;
  /// <summary>The semantic game version, like <c>1.6.0</c>.</summary>
  /// <remarks>
  ///   <para>
  ///     This mostly follows semantic versioning format with three or four numbers (without leading zeros), so
  ///     1.6.7 comes before 1.6.10. The first three numbers are consistent across all platforms, while some
  ///     platforms may add a fourth number for the port version. This doesn't include tags like <c>-alpha</c>
  ///     or <c>-beta</c>; see <see cref="F:StardewValley.Game1.versionLabel" /> or <see cref="M:StardewValley.Game1.GetVersionString" /> for that.
  ///   </para>
  /// 
  ///   <para>Game versions can be compared using <see cref="M:StardewValley.Utility.CompareGameVersions(System.String,System.String,System.Boolean)" />.</para>
  /// </remarks>
  public static readonly string version;
  /// <summary>A human-readable label for the update, like 'modding update' or 'hotfix #3', if any.</summary>
  public static readonly string versionLabel;
  /// <summary>The game build number used to distinguish different builds with the same version number, like <c>26055</c>.</summary>
  /// <remarks>This value is platform-dependent.</remarks>
  public static readonly int versionBuildNumber;
  public const float keyPollingThreshold = 650f;
  public const float toolHoldPerPowerupLevel = 600f;
  public const float startingMusicVolume = 1f;
  /// <summary>
  /// ContentManager specifically for loading xTile.Map(s).
  /// Will be unloaded when returning to title.
  /// </summary>
  public LocalizedContentManager xTileContent;
  public static DelayedAction morningSongPlayAction;
  private static LocalizedContentManager _temporaryContent;
  [NonInstancedStatic]
  private static bool FinishedIncrementalLoad = false;
  [NonInstancedStatic]
  private static bool FinishedFirstLoadContent = false;
  [NonInstancedStatic]
  private static volatile bool FinishedFirstInitSounds = false;
  [NonInstancedStatic]
  private static volatile bool FinishedFirstInitSerializers = false;
  [NonInstancedStatic]
  private static IEnumerator<int> LoadContentEnumerator;
  [NonInstancedStatic]
  public static GraphicsDeviceManager graphics;
  [NonInstancedStatic]
  public static LocalizedContentManager content;
  public static SpriteBatch spriteBatch;
  public static float MusicDuckTimer = 0.0f;
  public static GamePadState oldPadState;
  public static float thumbStickSensitivity = 0.1f;
  public static float runThreshold = 0.5f;
  public static int rightStickHoldTime = 0;
  public static int emoteMenuShowTime = 250;
  public static int nextFarmerWarpOffsetX = 0;
  public static int nextFarmerWarpOffsetY = 0;
  public static KeyboardState oldKBState;
  public static MouseState oldMouseState;
  [NonInstancedStatic]
  public static Game1 keyboardFocusInstance = (Game1) null;
  private static Farmer _player;
  public static NetFarmerRoot serverHost;
  protected static bool _isWarping = false;
  [NonInstancedStatic]
  public static bool hasLocalClientsOnly = false;
  protected bool _instanceIsPlayingBackgroundMusic;
  protected bool _instanceIsPlayingOutdoorsAmbience;
  protected bool _instanceIsPlayingNightAmbience;
  protected bool _instanceIsPlayingTownMusic;
  protected bool _instanceIsPlayingMorningSong;
  public static bool isUsingBackToFrontSorting = false;
  protected static StringBuilder _debugStringBuilder = new StringBuilder();
  [NonInstancedStatic]
  internal static readonly DebugTimings debugTimings = new DebugTimings();
  public static Dictionary<string, GameLocation> _locationLookup = new Dictionary<string, GameLocation>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
  public IList<GameLocation> _locations = (IList<GameLocation>) new List<GameLocation>();
  public static Regex asianSpacingRegex = new Regex(Game1.asianSpacingRegexString, RegexOptions.ECMAScript);
  public static Viewport defaultDeviceViewport;
  public static LocationRequest locationRequest;
  public static bool warpingForForcedRemoteEvent = false;
  protected static GameLocation _PreviousNonNullLocation = (GameLocation) null;
  public GameLocation instanceGameLocation;
  public static IDisplayDevice mapDisplayDevice;
  public static xTile.Dimensions.Rectangle viewport;
  public static xTile.Dimensions.Rectangle uiViewport;
  public static Texture2D objectSpriteSheet;
  public static Texture2D cropSpriteSheet;
  public static Texture2D emoteSpriteSheet;
  public static Texture2D debrisSpriteSheet;
  public static Texture2D rainTexture;
  public static Texture2D bigCraftableSpriteSheet;
  public static Texture2D buffsIcons;
  public static Texture2D daybg;
  public static Texture2D nightbg;
  public static Texture2D menuTexture;
  public static Texture2D uncoloredMenuTexture;
  public static Texture2D lantern;
  public static Texture2D windowLight;
  public static Texture2D sconceLight;
  public static Texture2D cauldronLight;
  public static Texture2D shadowTexture;
  public static Texture2D mouseCursors;
  public static Texture2D mouseCursors2;
  public static Texture2D mouseCursors_1_6;
  public static Texture2D giftboxTexture;
  public static Texture2D controllerMaps;
  public static Texture2D indoorWindowLight;
  public static Texture2D animations;
  public static Texture2D concessionsSpriteSheet;
  public static Texture2D birdsSpriteSheet;
  public static Texture2D objectSpriteSheet_2;
  public static Texture2D bobbersTexture;
  public static Dictionary<string, Stack<Dialogue>> npcDialogues = new Dictionary<string, Stack<Dialogue>>();
  protected readonly List<Farmer> _farmerShadows = new List<Farmer>();
  /// <summary>Actions that are called after waking up in the morning. These aren't saved, so they're only use for "fluff".</summary>
  public static Queue<Action> morningQueue = new Queue<Action>();
  [NonInstancedStatic]
  protected internal static ModHooks hooks = new ModHooks();
  public static InputState input = new InputState();
  protected internal static IInputSimulator inputSimulator = (IInputSimulator) null;
  public const string concessionsSpriteSheetName = "LooseSprites\\Concessions";
  public const string cropSpriteSheetName = "TileSheets\\crops";
  public const string objectSpriteSheetName = "Maps\\springobjects";
  public const string animationsName = "TileSheets\\animations";
  public const string mouseCursorsName = "LooseSprites\\Cursors";
  public const string mouseCursors2Name = "LooseSprites\\Cursors2";
  public const string mouseCursors1_6Name = "LooseSprites\\Cursors_1_6";
  public const string giftboxName = "LooseSprites\\Giftbox";
  public const string toolSpriteSheetName = "TileSheets\\tools";
  public const string bigCraftableSpriteSheetName = "TileSheets\\Craftables";
  public const string debrisSpriteSheetName = "TileSheets\\debris";
  public const string parrotSheetName = "LooseSprites\\parrots";
  public const string hatsSheetName = "Characters\\Farmer\\hats";
  public const string bobbersTextureName = "TileSheets\\bobbers";
  private static Texture2D _toolSpriteSheet = (Texture2D) null;
  public static Dictionary<Vector2, int> crabPotOverlayTiles = new Dictionary<Vector2, int>();
  protected static bool _setSaveName = false;
  protected static string _currentSaveName = "";
  public static List<string> mailDeliveredFromMailForTomorrow = new List<string>();
  private static RenderTarget2D _lightmap;
  public static Texture2D[] dynamicPixelRects = new Texture2D[3];
  public static Texture2D fadeToBlackRect;
  public static Texture2D staminaRect;
  public static Texture2D lightingRect;
  public static SpriteFont dialogueFont;
  public static SpriteFont smallFont;
  public static SpriteFont tinyFont;
  public static float screenGlowAlpha = 0.0f;
  public static float flashAlpha = 0.0f;
  public static float noteBlockTimer;
  public static int currentGemBirdIndex = 0;
  public Dictionary<string, object> newGameSetupOptions = new Dictionary<string, object>();
  public static bool dialogueUp = false;
  public static bool dialogueTyping = false;
  public static bool isQuestion = false;
  public static bool newDay = false;
  public static bool eventUp = false;
  public static bool viewportFreeze = false;
  public static bool eventOver = false;
  public static bool screenGlow = false;
  public static bool screenGlowHold = false;
  public static bool screenGlowUp;
  public static bool killScreen = false;
  public static bool messagePause;
  public static bool weddingToday;
  public static bool exitToTitle;
  public static bool debugMode;
  public static bool displayHUD = true;
  public static bool displayFarmer = true;
  public static bool dialogueButtonShrinking;
  public static bool drawLighting;
  public static bool quit;
  public static bool drawGrid;
  public static bool freezeControls;
  public static bool saveOnNewDay;
  public static bool panMode;
  public static bool showingEndOfNightStuff;
  public static bool wasRainingYesterday;
  public static bool hasLoadedGame;
  public static bool isActionAtCurrentCursorTile;
  public static bool isInspectionAtCurrentCursorTile;
  public static bool isSpeechAtCurrentCursorTile;
  public static bool paused;
  public static bool isTimePaused;
  public static bool frameByFrame;
  public static bool lastCursorMotionWasMouse;
  public static bool showingHealth = false;
  public static bool cabinsSeparate = false;
  public static bool showingHealthBar = false;
  /// <summary>Whether <see cref="M:StardewValley.Game1.OnDayStarted" /> has been called at least once since this save was loaded or joined.</summary>
  public static bool hasStartedDay = false;
  /// <summary>The event IDs which the current player has seen since entering the location.</summary>
  public static HashSet<string> eventsSeenSinceLastLocationChange = new HashSet<string>();
  internal static bool hasApplied1_3_UpdateChanges = false;
  internal static bool hasApplied1_4_UpdateChanges = false;
  private static Action postExitToTitleCallback = (Action) null;
  protected int _lastUsedDisplay = -1;
  public bool wasAskedLeoMemory;
  public float controllerSlingshotSafeTime;
  public static Game1.BundleType bundleType = Game1.BundleType.Default;
  public static bool isRaining = false;
  public static bool isSnowing = false;
  public static bool isLightning = false;
  public static bool isDebrisWeather = false;
  /// <summary>Internal state that tracks whether today's weather state is a green rain day.</summary>
  private static bool _isGreenRain = false;
  /// <summary>Whether today's weather state was green rain at any point.</summary>
  internal static bool wasGreenRain = false;
  /// <summary>Whether the locations affected by green rain still need cleanup. This should only be set by <see cref="M:StardewValley.Game1._newDayAfterFade" />.</summary>
  internal static bool greenRainNeedsCleanup = false;
  /// <summary>The season for which the debris weather fields like <see cref="F:StardewValley.Game1.debrisWeather" /> were last generated.</summary>
  public static Season? debrisWeatherSeason;
  public static string weatherForTomorrow;
  public float zoomModifier = 1f;
  private static ScreenFade screenFade;
  /// <summary>The current season of the year.</summary>
  public static Season season = Season.Spring;
  public static SerializableDictionary<string, string> bannedUsers = new SerializableDictionary<string, string>();
  private static object _debugOutputLock = new object();
  private static string _debugOutput;
  public static string requestedMusicTrack = "";
  public static string messageAfterPause = "";
  public static string samBandName = "The Alfalfas";
  public static string loadingMessage = "";
  public static string errorMessage = "";
  protected Dictionary<MusicContext, KeyValuePair<string, bool>> _instanceRequestedMusicTracks = new Dictionary<MusicContext, KeyValuePair<string, bool>>();
  protected MusicContext _instanceActiveMusicContext;
  public static bool requestedMusicTrackOverrideable;
  public static bool currentTrackOverrideable;
  public static bool requestedMusicDirty = false;
  protected bool _useUnscaledLighting;
  protected bool _didInitiateItemStow;
  public bool instanceIsOverridingTrack;
  private static string[] _shortDayDisplayName = new string[7];
  public static Queue<string> currentObjectDialogue = new Queue<string>();
  public static HashSet<string> worldStateIDs = new HashSet<string>();
  public static List<Response> questionChoices = new List<Response>();
  public static int xLocationAfterWarp;
  public static int yLocationAfterWarp;
  public static int gameTimeInterval;
  public static int currentQuestionChoice;
  public static int currentDialogueCharacterIndex;
  public static int dialogueTypingInterval;
  /// <summary>The calendar day of month.</summary>
  public static int dayOfMonth = 0;
  /// <summary>The calendar year.</summary>
  public static int year = 1;
  public static int timeOfDay = 600;
  public static int timeOfDayAfterFade = -1;
  public static int dialogueWidth;
  public static int facingDirectionAfterWarp;
  public static int mouseClickPolling;
  public static int gamePadXButtonPolling;
  public static int gamePadAButtonPolling;
  public static int weatherIcon;
  public static int hitShakeTimer;
  public static int staminaShakeTimer;
  public static int pauseThenDoFunctionTimer;
  public static int cursorTileHintCheckTimer;
  public static int timerUntilMouseFade;
  public static int whichFarm;
  public static int startingCabins;
  public static ModFarmType whichModFarm = (ModFarmType) null;
  public static ulong? startingGameSeed = new ulong?();
  public static int elliottPiano = 0;
  public static Microsoft.Xna.Framework.Rectangle viewportClampArea = Microsoft.Xna.Framework.Rectangle.Empty;
  public static SaveFixes lastAppliedSaveFix;
  public static Color eveningColor = new Color((int) byte.MaxValue, (int) byte.MaxValue, 0);
  public static Color unselectedOptionColor = new Color(100, 100, 100);
  public static Color screenGlowColor;
  public static NPC currentSpeaker;
  /// <summary>A default random number generator used for a wide variety of randomization in the game. This provides non-repeatable randomization (e.g. reloading the save will produce different results).</summary>
  public static Random random = new Random();
  public static Random recentMultiplayerRandom = new Random();
  /// <summary>The cached data for achievements from <c>Data/Achievements</c>.</summary>
  public static Dictionary<int, string> achievements;
  /// <summary>The cached data for <see cref="F:StardewValley.ItemRegistry.type_bigCraftable" />-type items from <c>Data/BigCraftables</c>.</summary>
  public static IDictionary<string, BigCraftableData> bigCraftableData;
  /// <summary>The cached data for buildings from <c>Data/Buildings</c>.</summary>
  public static IDictionary<string, BuildingData> buildingData;
  /// <summary>The cached data for NPCs from <c>Data/Characters</c>.</summary>
  public static IDictionary<string, CharacterData> characterData;
  /// <summary>The cached data for crops from <c>Data/Crops</c>.</summary>
  public static IDictionary<string, CropData> cropData;
  /// <summary>The cached data for farm animals from <c>Data/FarmAnimals</c>.</summary>
  public static IDictionary<string, FarmAnimalData> farmAnimalData;
  /// <summary>The cached data for flooring and path items from <c>Data/FloorsAndPaths</c>.</summary>
  public static IDictionary<string, FloorPathData> floorPathData;
  /// <summary>The cached data for fruit trees from <c>Data/FruitTrees</c>.</summary>
  public static IDictionary<string, FruitTreeData> fruitTreeData;
  /// <summary>The cached data for jukebox tracks from <c>Data/JukeboxTracks</c>.</summary>
  public static IDictionary<string, JukeboxTrackData> jukeboxTrackData;
  /// <summary>The cached data for locations from <c>Data/Locations</c>.</summary>
  public static IDictionary<string, LocationData> locationData;
  /// <summary>The cached data for location contexts from <c>Data/LocationContexts</c>.</summary>
  public static IDictionary<string, LocationContextData> locationContextData;
  /// <summary>The cached data for <c>Data/NPCGiftTastes</c>.</summary>
  public static IDictionary<string, string> NPCGiftTastes;
  /// <summary>The cached data for <see cref="F:StardewValley.ItemRegistry.type_object" />-type items from <c>Data/Objects</c>.</summary>
  public static IDictionary<string, ObjectData> objectData;
  /// <summary>The cached data for <see cref="F:StardewValley.ItemRegistry.type_pants" />-type items from <c>Data/Pants</c>.</summary>
  public static IDictionary<string, PantsData> pantsData;
  /// <summary>The cached data for pets from <c>Data/Pets</c>.</summary>
  public static IDictionary<string, PetData> petData;
  /// <summary>The cached data for <see cref="F:StardewValley.ItemRegistry.type_shirt" />-type items from <c>Data/Shirts</c>.</summary>
  public static IDictionary<string, ShirtData> shirtData;
  /// <summary>The cached data for <see cref="F:StardewValley.ItemRegistry.type_tool" />-type items from <c>Data/Tools</c>.</summary>
  public static IDictionary<string, ToolData> toolData;
  /// <summary>The cached data for <see cref="F:StardewValley.ItemRegistry.type_weapon" />-type items from <c>Data/Weapons</c>.</summary>
  public static IDictionary<string, WeaponData> weaponData;
  public static List<HUDMessage> hudMessages = new List<HUDMessage>();
  public static float musicPlayerVolume;
  public static float ambientPlayerVolume;
  public static float pauseAccumulator;
  public static float pauseTime;
  public static float upPolling;
  public static float downPolling;
  public static float rightPolling;
  public static float leftPolling;
  public static float debrisSoundInterval;
  public static float windGust;
  public static float dialogueButtonScale = 1f;
  public ICue instanceCurrentSong;
  public static IAudioCategory musicCategory;
  public static IAudioCategory soundCategory;
  public static IAudioCategory ambientCategory;
  public static IAudioCategory footstepCategory;
  public PlayerIndex instancePlayerOneIndex;
  [NonInstancedStatic]
  public static IAudioEngine audioEngine;
  [NonInstancedStatic]
  public static WaveBank waveBank;
  [NonInstancedStatic]
  public static WaveBank waveBank1_4;
  [NonInstancedStatic]
  public static ISoundBank soundBank;
  public static Vector2 previousViewportPosition;
  public static Vector2 currentCursorTile;
  public static Vector2 lastCursorTile = Vector2.Zero;
  public static Vector2 snowPos;
  public Microsoft.Xna.Framework.Rectangle localMultiplayerWindow;
  public static RainDrop[] rainDrops = new RainDrop[70];
  public static ICue chargeUpSound;
  public static ICue wind;
  /// <summary>The audio cues for the current location which are continuously looping until they're stopped.</summary>
  public static LoopingCueManager loopingLocationCues = new LoopingCueManager();
  /// <summary>Encapsulates the game logic for playing sound effects (excluding music and background ambience).</summary>
  public static ISoundsHelper sounds = (ISoundsHelper) new SoundsHelper();
  [NonInstancedStatic]
  public static AudioCueModificationManager CueModification = new AudioCueModificationManager();
  public static List<WeatherDebris> debrisWeather = new List<WeatherDebris>();
  public static TemporaryAnimatedSpriteList screenOverlayTempSprites = new TemporaryAnimatedSpriteList();
  public static TemporaryAnimatedSpriteList uiOverlayTempSprites = new TemporaryAnimatedSpriteList();
  private static byte _gameMode;
  private bool _isSaving;
  /// <summary>Handles writing game messages to the log output.</summary>
  [NonInstancedStatic]
  protected internal static IGameLogger log = (IGameLogger) new DefaultLogger(!Program.releaseBuild, false);
  /// <summary>Combines hash codes in a deterministic way that's consistent between both sessions and players.</summary>
  [NonInstancedStatic]
  public static IHashUtility hash = (IHashUtility) new HashUtility();
  protected internal static Multiplayer multiplayer = new Multiplayer();
  public static byte multiplayerMode;
  public static IEnumerator<int> currentLoader;
  public static ulong uniqueIDForThisGame = Utility.NewUniqueIdForThisGame();
  public static int[] directionKeyPolling = new int[4];
  /// <summary>The light sources to draw in the current location.</summary>
  public static Dictionary<string, LightSource> currentLightSources = new Dictionary<string, LightSource>();
  public static Color ambientLight;
  public static Color outdoorLight = new Color((int) byte.MaxValue, (int) byte.MaxValue, 0);
  public static Color textColor = new Color(34, 17, 34);
  /// <summary>The default color for shadows drawn under text.</summary>
  public static Color textShadowColor = new Color(206, 156, 95);
  /// <summary>A darker version of <see cref="F:StardewValley.Game1.textShadowColor" /> used in some cases.</summary>
  public static Color textShadowDarkerColor = new Color(221, 148, 84);
  public static IClickableMenu overlayMenu;
  private static IClickableMenu _activeClickableMenu;
  /// <summary>The queue of menus to open when the <see cref="P:StardewValley.Game1.activeClickableMenu" /> is closed.</summary>
  /// <remarks>See also <see cref="P:StardewValley.Game1.activeClickableMenu" />, <see cref="F:StardewValley.Game1.onScreenMenus" />, and <see cref="F:StardewValley.Game1.overlayMenu" />.</remarks>
  public static List<IClickableMenu> nextClickableMenu = new List<IClickableMenu>();
  /// <summary>A queue of actions to perform when <see cref="M:StardewValley.Farmer.IsBusyDoingSomething" /> is false.</summary>
  /// <remarks>Most code should call <see cref="M:StardewValley.Game1.PerformActionWhenPlayerFree(System.Action)" /> instead of using this field directly.</remarks>
  public static List<Action> actionsWhenPlayerFree = new List<Action>();
  public static bool isCheckingNonMousePlacement = false;
  private static IMinigame _currentMinigame = (IMinigame) null;
  public static IList<IClickableMenu> onScreenMenus = (IList<IClickableMenu>) new List<IClickableMenu>();
  public static BuffsDisplay buffsDisplay;
  public static DayTimeMoneyBox dayTimeMoneyBox;
  public static NetRootDictionary<long, Farmer> otherFarmers;
  private static readonly FarmerCollection _onlineFarmers = new FarmerCollection();
  public static IGameServer server;
  public static Client client;
  public KeyboardDispatcher instanceKeyboardDispatcher;
  public static Background background;
  public static FarmEvent farmEvent;
  /// <summary>The farm event to play next, if a regular farm event doesn't play via <see cref="F:StardewValley.Game1.farmEvent" /> instead.</summary>
  /// <remarks>This is set via the <see cref="M:StardewValley.DebugCommands.DefaultHandlers.SetFarmEvent(System.String[],StardewValley.Logging.IGameLogger)" /> debug command.</remarks>
  public static FarmEvent farmEventOverride;
  public static Game1.afterFadeFunction afterFade;
  public static Game1.afterFadeFunction afterDialogues;
  public static Game1.afterFadeFunction afterViewport;
  public static Game1.afterFadeFunction viewportReachedTarget;
  public static Game1.afterFadeFunction afterPause;
  public static GameTime currentGameTime;
  public static IList<DelayedAction> delayedActions = (IList<DelayedAction>) new List<DelayedAction>();
  public static Stack<IClickableMenu> endOfNightMenus = new Stack<IClickableMenu>();
  public Options instanceOptions;
  [NonInstancedStatic]
  public static SerializableDictionary<long, Options> splitscreenOptions = new SerializableDictionary<long, Options>();
  public static Game1 game1;
  public static Point lastMousePositionBeforeFade;
  public static int ticks;
  public static EmoteMenu emoteMenu;
  [NonInstancedStatic]
  public static SerializableDictionary<string, string> CustomData = new SerializableDictionary<string, string>();
  /// <summary>Manages and synchronizes ready checks, which ensure all players are ready before proceeding (e.g. before sleeping).</summary>
  public static ReadySynchronizer netReady = new ReadySynchronizer();
  /// <summary>Manages updating a fake host player when <see cref="!:Game1.IsDedicatedServer" /> is true.</summary>
  public static DedicatedServer dedicatedServer = new DedicatedServer();
  public static NetRoot<NetWorldState> netWorldState;
  public static ChatBox chatBox;
  public TextEntryMenu instanceTextEntry;
  public static SpecialCurrencyDisplay specialCurrencyDisplay = (SpecialCurrencyDisplay) null;
  private static string debugPresenceString;
  public static List<Action> remoteEventQueue = new List<Action>();
  public static List<long> weddingsToday = new List<long>();
  public int instanceIndex;
  public int instanceId;
  public static bool overrideGameMenuReset;
  protected bool _windowResizing;
  protected Point _oldMousePosition;
  protected bool _oldGamepadConnectedState;
  protected int _oldScrollWheelValue;
  public static Point viewportCenter;
  public static Vector2 viewportTarget = new Vector2((float) int.MinValue, (float) int.MinValue);
  public static float viewportSpeed = 2f;
  public static int viewportHold;
  private static bool _cursorDragEnabled = false;
  private static bool _cursorDragPrevEnabled = false;
  private static bool _cursorSpeedDirty = true;
  private const float CursorBaseSpeed = 16f;
  private static float _cursorSpeed = 16f;
  private static float _cursorSpeedScale = 1f;
  private static float _cursorUpdateElapsedSec = 0.0f;
  private static int thumbstickPollingTimer;
  public static bool toggleFullScreen;
  public static string whereIsTodaysFest;
  public const string NO_LETTER_MAIL = "%&NL&%";
  public const string BROADCAST_MAIL_FOR_TOMORROW_PREFIX = "%&MFT&%";
  public const string BROADCAST_SEEN_MAIL_PREFIX = "%&SM&%";
  public const string BROADCAST_MAILBOX_PREFIX = "%&MB&%";
  public bool isLocalMultiplayerNewDayActive;
  protected static Task _newDayTask;
  private static Action _afterNewDayAction;
  public static NewDaySynchronizer newDaySync = new NewDaySynchronizer();
  public static bool forceSnapOnNextViewportUpdate = false;
  public static Vector2 currentViewportTarget;
  public static Vector2 viewportPositionLerp;
  public static float screenGlowRate = 0.005f;
  public static float screenGlowMax;
  public static bool haltAfterCheck = false;
  public static bool uiMode = false;
  public static RenderTarget2D nonUIRenderTarget = (RenderTarget2D) null;
  public static int uiModeCount = 0;
  protected static int _oldUIModeCount = 0;
  internal string panModeString;
  public static bool conventionMode = false;
  internal static EventTest eventTest;
  internal bool panFacingDirectionWait;
  public static bool isRunningMacro = false;
  public static int thumbstickMotionMargin;
  public static float thumbstickMotionAccell = 1f;
  public static int triggerPolling;
  public static int rightClickPolling;
  private RenderTarget2D _screen;
  private RenderTarget2D _uiScreen;
  public static Color bgColor = new Color(5, 3, 4);
  protected readonly BlendState lightingBlend = new BlendState()
  {
    ColorBlendFunction = BlendFunction.ReverseSubtract,
    ColorDestinationBlend = Blend.One,
    ColorSourceBlend = Blend.SourceColor
  };
  public bool isDrawing;
  [NonInstancedStatic]
  public static bool isRenderingScreenBuffer = false;
  protected bool _lastDrewMouseCursor;
  protected static int _activatedTick = 0;
  /// <summary>The cursor icon to show, usually matching a constant like <see cref="F:StardewValley.Game1.cursor_default" />.</summary>
  public static int mouseCursor = Game1.cursor_default;
  private static float _mouseCursorTransparency = 1f;
  public static bool wasMouseVisibleThisFrame = true;
  public static NPC objectDialoguePortraitPerson;
  protected static StringBuilder _ParseTextStringBuilder = new StringBuilder(2408);
  protected static StringBuilder _ParseTextStringBuilderLine = new StringBuilder(1024 /*0x0400*/);
  protected static StringBuilder _ParseTextStringBuilderWord = new StringBuilder(256 /*0x0100*/);
  public bool ScreenshotBusy;
  public bool takingMapScreenshot;

  public bool IsActiveNoOverlay => this.IsActive && !Program.sdk.HasOverlay;

  public static void GetHasRoomAnotherFarmAsync(ReportHasRoomAnotherFarmDelegate callback)
  {
    if (LocalMultiplayer.IsLocalMultiplayer())
    {
      bool hasRoomAnotherFarm = Game1.GetHasRoomAnotherFarm();
      callback(hasRoomAnotherFarm);
    }
    else
    {
      Task task = new Task((Action) (() =>
      {
        Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
        callback(Game1.GetHasRoomAnotherFarm());
      }));
      Game1.hooks.StartTask(task, "Farm_SpaceCheck");
    }
  }

  private static string GameModeToString(byte mode)
  {
    switch (mode)
    {
      case 0:
        return $"titleScreenGameMode ({mode})";
      case 1:
        return $"loadScreenGameMode ({mode})";
      case 2:
        return $"newGameMode ({mode})";
      case 3:
        return $"playingGameMode ({mode})";
      case 4:
        return $"logoScreenGameMode ({mode})";
      case 6:
        return $"loadingMode ({mode})";
      case 7:
        return $"saveMode ({mode})";
      case 8:
        return $"saveCompleteMode ({mode})";
      case 9:
        return $"selectGameScreen ({mode})";
      case 10:
        return $"creditsMode ({mode})";
      case 11:
        return $"errorLogMode ({mode})";
      default:
        return $"unknown ({mode})";
    }
  }

  /// <summary>Get a human-readable game version which includes the <see cref="F:StardewValley.Game1.version" />, <see cref="F:StardewValley.Game1.versionLabel" />, and <see cref="F:StardewValley.Game1.versionBuildNumber" />.</summary>
  public static string GetVersionString()
  {
    string versionString = Game1.version;
    if (!string.IsNullOrEmpty(Game1.versionLabel))
      versionString = $"{versionString} '{Game1.versionLabel}'";
    if (Game1.versionBuildNumber > 0)
      versionString = $"{versionString} build {Game1.versionBuildNumber.ToString()}";
    return versionString;
  }

  public static LocalizedContentManager temporaryContent
  {
    get
    {
      if (Game1._temporaryContent == null)
        Game1._temporaryContent = Game1.content.CreateTemporary();
      return Game1._temporaryContent;
    }
  }

  private bool ShouldLoadIncrementally => false;

  /// <summary>The local character controlled by the current player.</summary>
  /// <remarks>Setting this field will dispose the previous value, if any. The new value must be a completely new instance which doesn't share any texture instances with the previous one to avoid errors.</remarks>
  public static Farmer player
  {
    get => Game1._player;
    internal set
    {
      Game1._player?.unload();
      Game1._player = value;
      Game1._player.Items.IsLocalPlayerInventory = true;
    }
  }

  public static bool IsPlayingBackgroundMusic
  {
    get => Game1.game1._instanceIsPlayingBackgroundMusic;
    set => Game1.game1._instanceIsPlayingBackgroundMusic = value;
  }

  public static bool IsPlayingOutdoorsAmbience
  {
    get => Game1.game1._instanceIsPlayingOutdoorsAmbience;
    set => Game1.game1._instanceIsPlayingOutdoorsAmbience = value;
  }

  public static bool IsPlayingNightAmbience
  {
    get => Game1.game1._instanceIsPlayingNightAmbience;
    set => Game1.game1._instanceIsPlayingNightAmbience = value;
  }

  public static bool IsPlayingTownMusic
  {
    get => Game1.game1._instanceIsPlayingTownMusic;
    set => Game1.game1._instanceIsPlayingTownMusic = value;
  }

  public static bool IsPlayingMorningSong
  {
    get => Game1.game1._instanceIsPlayingMorningSong;
    set => Game1.game1._instanceIsPlayingMorningSong = value;
  }

  public static bool isWarping => Game1._isWarping;

  public static IList<GameLocation> locations => Game1.game1._locations;

  public static GameLocation currentLocation
  {
    get => Game1.game1.instanceGameLocation;
    set
    {
      if (Game1.game1.instanceGameLocation == value)
        return;
      if (Game1._PreviousNonNullLocation == null)
        Game1._PreviousNonNullLocation = Game1.game1.instanceGameLocation;
      Game1.game1.instanceGameLocation = value;
      if (Game1.game1.instanceGameLocation == null)
        return;
      GameLocation previousNonNullLocation = Game1._PreviousNonNullLocation;
      Game1._PreviousNonNullLocation = (GameLocation) null;
      GameLocation instanceGameLocation = Game1.game1.instanceGameLocation;
      Game1.OnLocationChanged(previousNonNullLocation, instanceGameLocation);
    }
  }

  public static Texture2D toolSpriteSheet
  {
    get
    {
      if (Game1._toolSpriteSheet == null)
        Game1.ResetToolSpriteSheet();
      return Game1._toolSpriteSheet;
    }
  }

  public static void ResetToolSpriteSheet()
  {
    if (Game1._toolSpriteSheet != null)
    {
      Game1._toolSpriteSheet.Dispose();
      Game1._toolSpriteSheet = (Texture2D) null;
    }
    Texture2D texture2D1 = Game1.content.Load<Texture2D>("TileSheets\\tools");
    int width = texture2D1.Width;
    int height = texture2D1.Height;
    Texture2D texture2D2 = new Texture2D(Game1.game1.GraphicsDevice, width, height, false, SurfaceFormat.Color);
    texture2D2.Name = texture2D1.Name;
    Color[] data = new Color[width * height];
    texture2D1.GetData<Color>(data);
    texture2D2.SetData<Color>(data);
    Game1._toolSpriteSheet = texture2D2;
  }

  public static RenderTarget2D lightmap => Game1._lightmap;

  public static void SetSaveName(string new_save_name)
  {
    if (new_save_name == null)
      new_save_name = "";
    Game1._currentSaveName = new_save_name;
    Game1._setSaveName = true;
  }

  public static string GetSaveGameName(bool set_value = true)
  {
    if (!Game1._setSaveName & set_value)
    {
      string str1 = Game1.MasterPlayer.farmName.Value;
      string str2 = str1;
      int num = 2;
      while (SaveGame.IsNewGameSaveNameCollision(str2))
      {
        str2 = str1 + num.ToString();
        ++num;
      }
      Game1.SetSaveName(str2);
    }
    return Game1._currentSaveName;
  }

  private static void allocateLightmap(int width, int height)
  {
    int num1 = 8;
    float num2 = 1f;
    if (Game1.options != null)
    {
      num1 = Game1.options.lightingQuality;
      num2 = !Game1.game1.useUnscaledLighting ? Game1.options.zoomLevel : 1f;
    }
    int width1 = (int) ((double) width * (1.0 / (double) num2) + 64.0) / (num1 / 2);
    int height1 = (int) ((double) height * (1.0 / (double) num2) + 64.0) / (num1 / 2);
    RenderTarget2D lightmap = Game1.lightmap;
    // ISSUE: explicit non-virtual call
    if ((lightmap != null ? (__nonvirtual (lightmap.Width) != width1 ? 1 : 0) : 1) == 0 && Game1.lightmap.Height == height1)
      return;
    Game1._lightmap?.Dispose();
    Game1._lightmap = new RenderTarget2D(Game1.graphics.GraphicsDevice, width1, height1, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
  }

  /// <summary>Get whether HUD menus like the toolbar are currently being drawn.</summary>
  public static bool IsHudDrawn
  {
    get
    {
      return (Game1.displayHUD || Game1.eventUp) && Game1.gameMode == (byte) 3 && !Game1.freezeControls && !Game1.panMode && !Game1.HostPaused && !Game1.game1.takingMapScreenshot;
    }
  }

  /// <summary>Whether today's weather state is a green rain day.</summary>
  public static bool isGreenRain
  {
    get => Game1._isGreenRain;
    set
    {
      Game1._isGreenRain = value;
      Game1.wasGreenRain |= value;
    }
  }

  public static bool spawnMonstersAtNight
  {
    get => Game1.player.team.spawnMonstersAtNight.Value;
    set => Game1.player.team.spawnMonstersAtNight.Value = value;
  }

  /// <summary>When the game makes a random choice, whether to use a simpler method that's prone to repeating patterns.</summary>
  /// <remarks>This is mainly intended for speedrunning, where full randomization might be undesirable.</remarks>
  public static bool UseLegacyRandom
  {
    get => Game1.player.team.useLegacyRandom.Value;
    set => Game1.player.team.useLegacyRandom.Value = value;
  }

  public static bool fadeToBlack
  {
    get => Game1.screenFade.fadeToBlack;
    set => Game1.screenFade.fadeToBlack = value;
  }

  public static bool fadeIn
  {
    get => Game1.screenFade.fadeIn;
    set => Game1.screenFade.fadeIn = value;
  }

  public static bool globalFade
  {
    get => Game1.screenFade.globalFade;
    set => Game1.screenFade.globalFade = value;
  }

  public static bool nonWarpFade
  {
    get => Game1.screenFade.nonWarpFade;
    set => Game1.screenFade.nonWarpFade = value;
  }

  public static float fadeToBlackAlpha
  {
    get => Game1.screenFade.fadeToBlackAlpha;
    set => Game1.screenFade.fadeToBlackAlpha = value;
  }

  public static float globalFadeSpeed
  {
    get => Game1.screenFade.globalFadeSpeed;
    set => Game1.screenFade.globalFadeSpeed = value;
  }

  public static string CurrentSeasonDisplayName
  {
    get => Game1.content.LoadString("Strings\\StringsFromCSFiles:" + Game1.currentSeason);
  }

  /// <summary>The current season of the year as a string (one of <c>spring</c>, <c>summer</c>, <c>fall</c>, or <c>winter</c>).</summary>
  /// <remarks>Most code should use <see cref="F:StardewValley.Game1.season" /> instead.</remarks>
  public static string currentSeason
  {
    get => Utility.getSeasonKey(Game1.season);
    set
    {
      Season parsed;
      if (!Utility.TryParseEnum<Season>(value, out parsed))
        throw new ArgumentException($"Can't parse value '{value}' as a season name.");
      Game1.season = parsed;
    }
  }

  /// <summary>The current season of the year as a numeric index.</summary>
  /// <remarks>Most code should use <see cref="F:StardewValley.Game1.season" /> instead.</remarks>
  public static int seasonIndex => (int) Game1.season;

  public static string debugOutput
  {
    get => Game1._debugOutput;
    set
    {
      lock (Game1._debugOutputLock)
      {
        if (!(Game1._debugOutput != value))
          return;
        Game1._debugOutput = value;
        if (string.IsNullOrEmpty(Game1._debugOutput))
          return;
        Game1.log.Debug("DebugOutput: " + Game1._debugOutput);
      }
    }
  }

  public static string elliottBookName
  {
    get
    {
      if (Game1.player != null && Game1.player.DialogueQuestionsAnswered.Contains("958699"))
        return Game1.content.LoadString("Strings\\Events:ElliottBook_mystery");
      return Game1.player != null && Game1.player.DialogueQuestionsAnswered.Contains("958700") ? Game1.content.LoadString("Strings\\Events:ElliottBook_romance") : Game1.content.LoadString("Strings\\Events:ElliottBook_default");
    }
    set
    {
    }
  }

  protected static Dictionary<MusicContext, KeyValuePair<string, bool>> _requestedMusicTracks
  {
    get => Game1.game1._instanceRequestedMusicTracks;
    set => Game1.game1._instanceRequestedMusicTracks = value;
  }

  protected static MusicContext _activeMusicContext
  {
    get => Game1.game1._instanceActiveMusicContext;
    set => Game1.game1._instanceActiveMusicContext = value;
  }

  public static bool isOverridingTrack
  {
    get => Game1.game1.instanceIsOverridingTrack;
    set => Game1.game1.instanceIsOverridingTrack = value;
  }

  public bool useUnscaledLighting
  {
    get => this._useUnscaledLighting;
    set
    {
      if (this._useUnscaledLighting == value)
        return;
      this._useUnscaledLighting = value;
      Game1.allocateLightmap(this.localMultiplayerWindow.Width, this.localMultiplayerWindow.Height);
    }
  }

  /// <inheritdoc cref="F:StardewValley.Farmer.mailbox" />
  public static IList<string> mailbox => (IList<string>) Game1.player.mailbox;

  public static ICue currentSong
  {
    get => Game1.game1.instanceCurrentSong;
    set => Game1.game1.instanceCurrentSong = value;
  }

  public static PlayerIndex playerOneIndex
  {
    get => Game1.game1.instancePlayerOneIndex;
    set => Game1.game1.instancePlayerOneIndex = value;
  }

  /// <summary>The number of ticks since <see cref="P:StardewValley.Game1.gameMode" /> changed.</summary>
  public static int gameModeTicks { get; private set; }

  public static byte gameMode
  {
    get => Game1._gameMode;
    set
    {
      if ((int) Game1._gameMode == (int) value)
        return;
      Game1.log.Verbose($"gameMode was '{Game1.GameModeToString(Game1._gameMode)}', set to '{Game1.GameModeToString(value)}'.");
      Game1._gameMode = value;
      Game1.gameModeTicks = 0;
    }
  }

  public bool IsSaving
  {
    get => this._isSaving;
    set => this._isSaving = value;
  }

  public static Multiplayer Multiplayer => Game1.multiplayer;

  public static Stats stats => Game1.player.stats;

  /// <summary>The daily quest that's shown on the billboard, if any.</summary>
  public static Quest questOfTheDay => Game1.netWorldState.Value.QuestOfTheDay;

  /// <summary>The menu which is currently handling player interactions (e.g. a letter viewer, dialogue box, inventory, etc).</summary>
  /// <remarks>See also <see cref="F:StardewValley.Game1.nextClickableMenu" />, <see cref="F:StardewValley.Game1.onScreenMenus" />, and <see cref="F:StardewValley.Game1.overlayMenu" />.</remarks>
  public static IClickableMenu activeClickableMenu
  {
    get => Game1._activeClickableMenu;
    set
    {
      int num;
      switch (Game1.activeClickableMenu)
      {
        case SaveGameMenu _:
        case ShippingMenu _:
          num = value is SaveGameMenu ? 0 : (!(value is ShippingMenu) ? 1 : 0);
          break;
        default:
          num = 0;
          break;
      }
      if (Game1._activeClickableMenu is IDisposable activeClickableMenu && !Game1._activeClickableMenu.HasDependencies())
        activeClickableMenu.Dispose();
      if (Game1.textEntry != null && Game1._activeClickableMenu != value)
        Game1.closeTextEntry();
      if (Game1._activeClickableMenu != null && value == null)
        Game1.timerUntilMouseFade = 0;
      Game1._activeClickableMenu = value;
      if (num != 0)
        Game1.OnDayStarted();
      if (Game1._activeClickableMenu != null)
      {
        if (Game1.eventUp && (Game1.CurrentEvent == null || !Game1.CurrentEvent.playerControlSequence || Game1.player.UsingTool))
          return;
        Game1.player.Halt();
      }
      else
      {
        if (Game1.nextClickableMenu.Count <= 0)
          return;
        Game1.activeClickableMenu = Game1.nextClickableMenu[0];
        Game1.nextClickableMenu.RemoveAt(0);
      }
    }
  }

  public static IMinigame currentMinigame
  {
    get => Game1._currentMinigame;
    set
    {
      Game1._currentMinigame = value;
      if (value == null)
      {
        if (Game1.currentLocation != null)
          Game1.setRichPresence("location", (object) Game1.currentLocation.Name);
        Game1.randomizeDebrisWeatherPositions(Game1.debrisWeather);
        Game1.randomizeRainPositions();
      }
      else
      {
        if (value.minigameId() == null)
          return;
        Game1.setRichPresence("minigame", (object) value.minigameId());
      }
    }
  }

  public static bool canHaveWeddingOnDay(int day, Season season)
  {
    return !Utility.isFestivalDay(day, season) && !Utility.isGreenRainDay(day, season);
  }

  /// <summary>Reset the <see cref="P:StardewValley.Game1.questOfTheDay" /> for today and synchronize it to other player. In multiplayer, this can only be called on the host instance.</summary>
  public static void RefreshQuestOfTheDay()
  {
    Quest questOfTheDay = Utility.isFestivalDay() || Utility.isFestivalDay(Game1.dayOfMonth + 1, Game1.season) ? (Quest) null : Utility.getQuestOfTheDay();
    questOfTheDay?.dailyQuest.Set(true);
    questOfTheDay?.reloadObjective();
    questOfTheDay?.reloadDescription();
    Game1.netWorldState.Value.SetQuestOfTheDay(questOfTheDay);
  }

  public static void ExitToTitle(Action postExitCallback = null)
  {
    Game1.currentMinigame?.unload();
    Game1._requestedMusicTracks.Clear();
    Game1.UpdateRequestedMusicTrack();
    Game1.changeMusicTrack("none");
    Game1.setGameMode((byte) 0);
    Game1.exitToTitle = true;
    Game1.postExitToTitleCallback = postExitCallback;
  }

  public static Object dishOfTheDay
  {
    get => Game1.netWorldState.Value.DishOfTheDay;
    set => Game1.netWorldState.Value.DishOfTheDay = value;
  }

  public static KeyboardDispatcher keyboardDispatcher
  {
    get => Game1.game1.instanceKeyboardDispatcher;
    set => Game1.game1.instanceKeyboardDispatcher = value;
  }

  public static Options options
  {
    get => Game1.game1.instanceOptions;
    set => Game1.game1.instanceOptions = value;
  }

  public static TextEntryMenu textEntry
  {
    get => Game1.game1.instanceTextEntry;
    set => Game1.game1.instanceTextEntry = value;
  }

  public static WorldDate Date => Game1.netWorldState.Value.Date;

  public static bool NetTimePaused => Game1.netWorldState.Get().IsTimePaused;

  public static bool HostPaused => Game1.netWorldState.Get().IsPaused;

  /// <summary>Whether the game is currently in multiplayer mode with at least one other player connected.</summary>
  public static bool IsMultiplayer => Game1.otherFarmers.Count > 0;

  /// <summary>Whether this game instance is a farmhand connected to a remote host in multiplayer.</summary>
  public static bool IsClient => Game1.multiplayerMode == (byte) 1;

  /// <summary>Whether this game instance is the host in multiplayer.</summary>
  public static bool IsServer => Game1.multiplayerMode == (byte) 2;

  /// <summary>Whether this game instance is the main or host player.</summary>
  public static bool IsMasterGame
  {
    get => Game1.multiplayerMode == (byte) 0 || Game1.multiplayerMode == (byte) 2;
  }

  /// <summary>Whether this game is hosted by an automated dedicated host</summary>
  public static bool HasDedicatedHost
  {
    get
    {
      return Game1.multiplayerMode != (byte) 0 && Game1.player?.team?.hasDedicatedHost.Value.GetValueOrDefault();
    }
  }

  /// <summary>Whether this game instance is acting as the host of a dedicated server.</summary>
  public static bool IsDedicatedHost => Game1.IsServer && Game1.HasDedicatedHost;

  /// <summary>The main or host player instance.</summary>
  public static Farmer MasterPlayer => !Game1.IsMasterGame ? Game1.serverHost.Value : Game1.player;

  public static bool IsChatting
  {
    get => Game1.chatBox != null && Game1.chatBox.isActive();
    set
    {
      if (value == Game1.chatBox.isActive())
        return;
      if (value)
        Game1.chatBox.activate();
      else
        Game1.chatBox.clickAway();
    }
  }

  public static Event CurrentEvent
  {
    get => Game1.currentLocation == null ? (Event) null : Game1.currentLocation.currentEvent;
  }

  public static MineShaft mine
  {
    get
    {
      return Game1.locationRequest?.Location is MineShaft location ? location : Game1.currentLocation as MineShaft;
    }
  }

  public static int CurrentMineLevel
  {
    get => !(Game1.currentLocation is MineShaft currentLocation) ? 0 : currentLocation.mineLevel;
  }

  static Game1()
  {
    Game1.GameAssemblyName = typeof (Game1).Assembly.GetName().Name;
    AssemblyInformationalVersionAttribute customAttribute = typeof (Game1).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
    if (!string.IsNullOrWhiteSpace(customAttribute?.InformationalVersion))
    {
      string[] strArray = customAttribute.InformationalVersion.Split(',');
      if (strArray.Length == 4)
      {
        Game1.version = strArray[0].Trim();
        if (!string.IsNullOrWhiteSpace(strArray[1]))
          Game1.versionLabel = strArray[1].Trim();
        if (!string.IsNullOrWhiteSpace(strArray[2]))
        {
          int result;
          if (!int.TryParse(strArray[2], out result))
            throw new InvalidOperationException($"Can't parse game build number value '{strArray[2]}' as a number.");
          Game1.versionBuildNumber = result;
        }
        if (!string.IsNullOrWhiteSpace(strArray[3]))
          Multiplayer.protocolVersionOverride = strArray[3].Trim();
      }
    }
    if (string.IsNullOrWhiteSpace(Game1.version))
      throw new InvalidOperationException("No game version found in assembly info.");
  }

  public Game1(PlayerIndex player_index, int index)
    : this()
  {
    this.instancePlayerOneIndex = player_index;
    this.instanceIndex = index;
  }

  public Game1()
  {
    this.instanceId = GameRunner.instance.GetNewInstanceID();
    if (Program.gamePtr == null)
      Program.gamePtr = this;
    Game1._temporaryContent = this.CreateContentManager(this.Content.ServiceProvider, this.Content.RootDirectory);
  }

  public void TranslateFields()
  {
    LocalizedContentManager.localizedAssetNames.Clear();
    BaseEnchantment.ResetEnchantments();
    Game1.samBandName = Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.2156");
    Game1.elliottBookName = Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.2157");
    Game1.objectSpriteSheet = Game1.content.Load<Texture2D>("Maps\\springobjects");
    Game1.objectSpriteSheet_2 = Game1.content.Load<Texture2D>("TileSheets\\Objects_2");
    Game1.bobbersTexture = Game1.content.Load<Texture2D>("TileSheets\\bobbers");
    Game1.dialogueFont = Game1.content.Load<SpriteFont>("Fonts\\SpriteFont1");
    Game1.smallFont = Game1.content.Load<SpriteFont>("Fonts\\SmallFont");
    Game1.smallFont.LineSpacing = 28;
    switch (LocalizedContentManager.CurrentLanguageCode)
    {
      case LocalizedContentManager.LanguageCode.ko:
        Game1.smallFont.LineSpacing += 16 /*0x10*/;
        break;
      case LocalizedContentManager.LanguageCode.tr:
        Game1.smallFont.LineSpacing += 4;
        break;
      case LocalizedContentManager.LanguageCode.mod:
        Game1.smallFont.LineSpacing = LocalizedContentManager.CurrentModLanguage.SmallFontLineSpacing;
        break;
    }
    Game1.tinyFont = Game1.content.Load<SpriteFont>("Fonts\\tinyFont");
    Game1.objectData = (IDictionary<string, ObjectData>) DataLoader.Objects(Game1.content);
    Game1.bigCraftableData = (IDictionary<string, BigCraftableData>) DataLoader.BigCraftables(Game1.content);
    Game1.achievements = DataLoader.Achievements(Game1.content);
    CraftingRecipe.craftingRecipes = DataLoader.CraftingRecipes(Game1.content);
    CraftingRecipe.cookingRecipes = DataLoader.CookingRecipes(Game1.content);
    ItemRegistry.ResetCache();
    MovieTheater.ClearCachedLocalizedData();
    Game1.mouseCursors = Game1.content.Load<Texture2D>("LooseSprites\\Cursors");
    Game1.mouseCursors2 = Game1.content.Load<Texture2D>("LooseSprites\\Cursors2");
    Game1.mouseCursors_1_6 = Game1.content.Load<Texture2D>("LooseSprites\\Cursors_1_6");
    Game1.giftboxTexture = Game1.content.Load<Texture2D>("LooseSprites\\Giftbox");
    Game1.controllerMaps = Game1.content.Load<Texture2D>("LooseSprites\\ControllerMaps");
    Game1.NPCGiftTastes = (IDictionary<string, string>) DataLoader.NpcGiftTastes(Game1.content);
    Game1._shortDayDisplayName[0] = Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3042");
    Game1._shortDayDisplayName[1] = Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3043");
    Game1._shortDayDisplayName[2] = Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3044");
    Game1._shortDayDisplayName[3] = Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3045");
    Game1._shortDayDisplayName[4] = Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3046");
    Game1._shortDayDisplayName[5] = Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3047");
    Game1._shortDayDisplayName[6] = Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3048");
  }

  public void exitEvent(object sender, EventArgs e)
  {
    Game1.multiplayer.Disconnect(Multiplayer.DisconnectType.ClosedGame);
    Game1.keyboardDispatcher.Cleanup();
  }

  public void refreshWindowSettings()
  {
    GameRunner.instance.OnWindowSizeChange((object) null, (EventArgs) null);
  }

  public void Window_ClientSizeChanged(object sender, EventArgs e)
  {
    if (this._windowResizing)
      return;
    Game1.log.Verbose("Window_ClientSizeChanged(); Window.ClientBounds=" + this.Window.ClientBounds.ToString());
    if (Game1.options == null)
    {
      Game1.log.Verbose("Window_ClientSizeChanged(); options is null, returning.");
    }
    else
    {
      this._windowResizing = true;
      int w = Game1.graphics.IsFullScreen ? Game1.graphics.PreferredBackBufferWidth : this.Window.ClientBounds.Width;
      int h = Game1.graphics.IsFullScreen ? Game1.graphics.PreferredBackBufferHeight : this.Window.ClientBounds.Height;
      GameRunner.instance.ExecuteForInstances((Action<Game1>) (instance => instance.SetWindowSize(w, h)));
      this._windowResizing = false;
    }
  }

  public virtual void SetWindowSize(int w, int h)
  {
    Microsoft.Xna.Framework.Rectangle oldBounds = new Microsoft.Xna.Framework.Rectangle(Game1.viewport.X, Game1.viewport.Y, Game1.viewport.Width, Game1.viewport.Height);
    if (Environment.OSVersion.Platform == PlatformID.Win32NT)
    {
      if (w < 1280 /*0x0500*/ && !Game1.graphics.IsFullScreen)
        w = 1280 /*0x0500*/;
      if (h < 720 && !Game1.graphics.IsFullScreen)
        h = 720;
    }
    if (!Game1.graphics.IsFullScreen && this.Window.AllowUserResizing)
    {
      Game1.graphics.PreferredBackBufferWidth = w;
      Game1.graphics.PreferredBackBufferHeight = h;
    }
    if (this.IsMainInstance && Game1.graphics.SynchronizeWithVerticalRetrace != Game1.options.vsyncEnabled)
    {
      Game1.graphics.SynchronizeWithVerticalRetrace = Game1.options.vsyncEnabled;
      Game1.log.Verbose("Vsync toggled: " + Game1.graphics.SynchronizeWithVerticalRetrace.ToString());
    }
    Game1.graphics.ApplyChanges();
    try
    {
      this.localMultiplayerWindow = !Game1.graphics.IsFullScreen ? new Microsoft.Xna.Framework.Rectangle(0, 0, w, h) : new Microsoft.Xna.Framework.Rectangle(0, 0, Game1.graphics.PreferredBackBufferWidth, Game1.graphics.PreferredBackBufferHeight);
    }
    catch (Exception ex)
    {
    }
    Game1.defaultDeviceViewport = new Viewport(this.localMultiplayerWindow);
    List<Vector4> vector4List = new List<Vector4>();
    if (GameRunner.instance.gameInstances.Count <= 1)
    {
      vector4List.Add(new Vector4(0.0f, 0.0f, 1f, 1f));
    }
    else
    {
      switch (GameRunner.instance.gameInstances.Count)
      {
        case 2:
          vector4List.Add(new Vector4(0.0f, 0.0f, 0.5f, 1f));
          vector4List.Add(new Vector4(0.5f, 0.0f, 0.5f, 1f));
          break;
        case 3:
          vector4List.Add(new Vector4(0.0f, 0.0f, 1f, 0.5f));
          vector4List.Add(new Vector4(0.0f, 0.5f, 0.5f, 0.5f));
          vector4List.Add(new Vector4(0.5f, 0.5f, 0.5f, 0.5f));
          break;
        case 4:
          vector4List.Add(new Vector4(0.0f, 0.0f, 0.5f, 0.5f));
          vector4List.Add(new Vector4(0.5f, 0.0f, 0.5f, 0.5f));
          vector4List.Add(new Vector4(0.0f, 0.5f, 0.5f, 0.5f));
          vector4List.Add(new Vector4(0.5f, 0.5f, 0.5f, 0.5f));
          break;
      }
    }
    this.zoomModifier = GameRunner.instance.gameInstances.Count > 1 ? 0.5f : 1f;
    Vector4 vector4 = vector4List[Game1.game1.instanceIndex];
    Vector2? nullable = new Vector2?();
    if (this.uiScreen != null)
      nullable = new Vector2?(new Vector2((float) this.uiScreen.Width, (float) this.uiScreen.Height));
    this.localMultiplayerWindow.X = (int) ((double) w * (double) vector4.X);
    this.localMultiplayerWindow.Y = (int) ((double) h * (double) vector4.Y);
    this.localMultiplayerWindow.Width = (int) Math.Ceiling((double) w * (double) vector4.Z);
    this.localMultiplayerWindow.Height = (int) Math.Ceiling((double) h * (double) vector4.W);
    try
    {
      int width1 = (int) Math.Ceiling((double) this.localMultiplayerWindow.Width * (1.0 / (double) Game1.options.zoomLevel));
      int height1 = (int) Math.Ceiling((double) this.localMultiplayerWindow.Height * (1.0 / (double) Game1.options.zoomLevel));
      RenderTarget2D renderTarget2D1 = new RenderTarget2D(Game1.graphics.GraphicsDevice, width1, height1, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
      renderTarget2D1.Name = "@Game1.screen";
      this.screen = renderTarget2D1;
      int width2 = (int) Math.Ceiling((double) this.localMultiplayerWindow.Width / (double) Game1.options.uiScale);
      int height2 = (int) Math.Ceiling((double) this.localMultiplayerWindow.Height / (double) Game1.options.uiScale);
      RenderTarget2D renderTarget2D2 = new RenderTarget2D(Game1.graphics.GraphicsDevice, width2, height2, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
      renderTarget2D2.Name = "@Game1.uiScreen";
      this.uiScreen = renderTarget2D2;
    }
    catch (Exception ex)
    {
    }
    Game1.updateViewportForScreenSizeChange(false, this.localMultiplayerWindow.Width, this.localMultiplayerWindow.Height);
    if (nullable.HasValue && (double) nullable.Value.X == (double) this.uiScreen.Width && (double) nullable.Value.Y == (double) this.uiScreen.Height)
      return;
    Game1.PushUIMode();
    Game1.textEntry?.gameWindowSizeChanged(oldBounds, new Microsoft.Xna.Framework.Rectangle(Game1.viewport.X, Game1.viewport.Y, Game1.viewport.Width, Game1.viewport.Height));
    foreach (IClickableMenu onScreenMenu in (IEnumerable<IClickableMenu>) Game1.onScreenMenus)
      onScreenMenu.gameWindowSizeChanged(oldBounds, new Microsoft.Xna.Framework.Rectangle(Game1.viewport.X, Game1.viewport.Y, Game1.viewport.Width, Game1.viewport.Height));
    Game1.currentMinigame?.changeScreenSize();
    Game1.activeClickableMenu?.gameWindowSizeChanged(oldBounds, new Microsoft.Xna.Framework.Rectangle(Game1.viewport.X, Game1.viewport.Y, Game1.viewport.Width, Game1.viewport.Height));
    if (Game1.activeClickableMenu?.GetType() == typeof (GameMenu))
    {
      GameMenu activeClickableMenu = Game1.activeClickableMenu as GameMenu;
      IClickableMenu currentPage = activeClickableMenu.GetCurrentPage();
      switch (((GameMenu) (Game1.activeClickableMenu = (IClickableMenu) new GameMenu(activeClickableMenu.currentTab))).GetCurrentPage())
      {
        case CollectionsPage collectionsPage:
          collectionsPage.postWindowSizeChange(currentPage);
          break;
        case OptionsPage optionsPage:
          optionsPage.postWindowSizeChange(currentPage);
          break;
        case SocialPage socialPage:
          socialPage.postWindowSizeChange(currentPage);
          break;
      }
    }
    Game1.PopUIMode();
  }

  private void Game1_Exiting(object sender, EventArgs e) => Program.sdk.Shutdown();

  public static void setGameMode(byte mode)
  {
    Game1.log.Verbose($"setGameMode( '{Game1.GameModeToString(mode)}' )");
    Game1._gameMode = mode;
    Game1.temporaryContent?.Unload();
    if (mode != (byte) 0)
      return;
    bool flag = false;
    if (Game1.activeClickableMenu != null)
    {
      GameTime currentGameTime = Game1.currentGameTime;
      if ((currentGameTime != null ? (currentGameTime.TotalGameTime.TotalSeconds > 10.0 ? 1 : 0) : 0) != 0)
        flag = true;
    }
    if (Game1.game1.instanceIndex > 0)
      return;
    TitleMenu titleMenu = new TitleMenu();
    Game1.activeClickableMenu = (IClickableMenu) titleMenu;
    if (!flag)
      return;
    titleMenu.skipToTitleButtons();
  }

  public static void updateViewportForScreenSizeChange(
    bool fullscreenChange,
    int width,
    int height)
  {
    Game1.forceSnapOnNextViewportUpdate = true;
    if (Game1.graphics.GraphicsDevice != null)
      Game1.allocateLightmap(width, height);
    width = (int) Math.Ceiling((double) width / (double) Game1.options.zoomLevel);
    height = (int) Math.Ceiling((double) height / (double) Game1.options.zoomLevel);
    Point centerPoint = new Point(Game1.viewport.X + Game1.viewport.Width / 2, Game1.viewport.Y + Game1.viewport.Height / 2);
    bool flag = Game1.viewport.Width != width || Game1.viewport.Height != height;
    Game1.viewport = new xTile.Dimensions.Rectangle(centerPoint.X - width / 2, centerPoint.Y - height / 2, width, height);
    if (Game1.currentLocation == null)
      return;
    if (Game1.eventUp)
    {
      if (Game1.IsFakedBlackScreen() || !Game1.currentLocation.IsOutdoors)
        return;
      Game1.clampViewportToGameMap();
    }
    else
    {
      if (((Game1.viewport.X >= 0 ? 1 : (!Game1.currentLocation.IsOutdoors ? 1 : 0)) | (fullscreenChange ? 1 : 0)) != 0)
      {
        centerPoint = new Point(Game1.viewport.X + Game1.viewport.Width / 2, Game1.viewport.Y + Game1.viewport.Height / 2);
        Game1.viewport = new xTile.Dimensions.Rectangle(centerPoint.X - width / 2, centerPoint.Y - height / 2, width, height);
        Game1.UpdateViewPort(true, centerPoint);
      }
      if (!flag)
        return;
      Game1.forceSnapOnNextViewportUpdate = true;
      Game1.randomizeRainPositions();
      Game1.randomizeDebrisWeatherPositions(Game1.debrisWeather);
    }
  }

  public void Instance_Initialize()
  {
    Stopwatch stopwatch = Stopwatch.StartNew();
    this.Initialize();
    stopwatch.Stop();
    Game1.log.Verbose($"Instance_Initialize() finished, elapsed = '{stopwatch.Elapsed}'");
  }

  public static bool IsFading()
  {
    if (Game1.globalFade || Game1.fadeIn && (double) Game1.fadeToBlackAlpha > 0.0)
      return true;
    return Game1.fadeToBlack && (double) Game1.fadeToBlackAlpha < 1.0;
  }

  public static bool IsFakedBlackScreen()
  {
    return Game1.currentMinigame == null && (Game1.CurrentEvent == null || Game1.CurrentEvent.currentCustomEventScript == null) && Game1.eventUp && (double) (int) Math.Floor((double) new Point(Game1.viewport.X + Game1.viewport.Width / 2, Game1.viewport.Y + Game1.viewport.Height / 2).X / 64.0) <= -200.0;
  }

  private void DoThreadedInitTask(ThreadStart initTask)
  {
    if (this.ShouldLoadIncrementally)
      new Thread(initTask)
      {
        CurrentCulture = CultureInfo.InvariantCulture,
        Priority = ThreadPriority.Highest
      }.Start();
    else
      initTask();
  }

  /// <summary>
  /// Allows the game to perform any initialization it needs to before starting to run.
  /// This is where it can query for any required services and load any non-graphic
  /// related content.  Calling base.Initialize will enumerate through any components
  /// and initialize them as well.
  /// </summary>
  protected override void Initialize()
  {
    Game1.keyboardDispatcher = new KeyboardDispatcher(this.Window);
    Game1.screenFade = new ScreenFade(new Func<bool>(this.onFadeToBlackComplete), new Action(Game1.onFadedBackInComplete));
    Game1.options = new Options();
    Game1.options.musicVolumeLevel = 1f;
    Game1.options.soundVolumeLevel = 1f;
    Game1.otherFarmers = new NetRootDictionary<long, Farmer>();
    this.DoThreadedInitTask(new ThreadStart(this.InitializeSerializers));
    Game1.viewport = new xTile.Dimensions.Rectangle(new Size(Game1.graphics.PreferredBackBufferWidth, Game1.graphics.PreferredBackBufferHeight));
    Game1.currentSong = (ICue) null;
    this.DoThreadedInitTask(new ThreadStart(this.InitializeSounds));
    int width = Game1.graphics.GraphicsDevice.Viewport.Width;
    int height = Game1.graphics.GraphicsDevice.Viewport.Height;
    this.screen = new RenderTarget2D(Game1.graphics.GraphicsDevice, width, height, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
    Game1.allocateLightmap(width, height);
    Game1.previousViewportPosition = Vector2.Zero;
    Game1.PushUIMode();
    Game1.PopUIMode();
    Game1.setRichPresence("menus");
  }

  private void InitializeSounds()
  {
    if (this.IsMainInstance)
    {
      try
      {
        string rootDirectory = this.Content.RootDirectory;
        AudioEngine engine = new AudioEngine(Path.Combine(rootDirectory, "XACT", "FarmerSounds.xgs"));
        engine.GetReverbSettings()[18] = 4f;
        engine.GetReverbSettings()[17] = -12f;
        Game1.audioEngine = (IAudioEngine) new AudioEngineWrapper(engine);
        Game1.waveBank = new WaveBank(Game1.audioEngine.Engine, Path.Combine(rootDirectory, "XACT", "Wave Bank.xwb"));
        Game1.waveBank1_4 = new WaveBank(Game1.audioEngine.Engine, Path.Combine(rootDirectory, "XACT", "Wave Bank(1.4).xwb"));
        Game1.soundBank = (ISoundBank) new SoundBankWrapper(new SoundBank(Game1.audioEngine.Engine, Path.Combine(rootDirectory, "XACT", "Sound Bank.xsb")));
      }
      catch (Exception ex)
      {
        Game1.log.Error("Game.Initialize() caught exception initializing XACT.", ex);
        Game1.audioEngine = (IAudioEngine) new DummyAudioEngine();
        Game1.soundBank = (ISoundBank) new DummySoundBank();
      }
    }
    Game1.audioEngine.Update();
    Game1.musicCategory = Game1.audioEngine.GetCategory("Music");
    Game1.soundCategory = Game1.audioEngine.GetCategory("Sound");
    Game1.ambientCategory = Game1.audioEngine.GetCategory("Ambient");
    Game1.footstepCategory = Game1.audioEngine.GetCategory("Footsteps");
    Game1.wind = Game1.soundBank.GetCue("wind");
    Game1.chargeUpSound = Game1.soundBank.GetCue("toolCharge");
    AmbientLocationSounds.InitShared();
    Game1.FinishedFirstInitSounds = true;
  }

  private void InitializeSerializers()
  {
    Game1.otherFarmers.Serializer = SaveSerializer.GetSerializer(typeof (Farmer));
    if (StartupPreferences.serializer == null)
      StartupPreferences.serializer = SaveSerializer.GetSerializer(typeof (StartupPreferences));
    Game1.FinishedFirstInitSerializers = true;
  }

  public static void pauseThenDoFunction(int pauseTime, Game1.afterFadeFunction function)
  {
    Game1.afterPause = function;
    Game1.pauseThenDoFunctionTimer = pauseTime;
  }

  /// <summary>Construct a content manager to read game content files.</summary>
  /// <param name="serviceProvider">The service provider to use to locate services.</param>
  /// <param name="rootDirectory">The root directory to search for content.</param>
  protected internal virtual LocalizedContentManager CreateContentManager(
    IServiceProvider serviceProvider,
    string rootDirectory)
  {
    return new LocalizedContentManager(serviceProvider, rootDirectory);
  }

  /// <summary>Create an xTile map display device.</summary>
  /// <param name="content">The content manager through which it should load tilesheet textures.</param>
  /// <param name="graphicsDevice">The XNA graphics device.</param>
  protected internal virtual IDisplayDevice CreateDisplayDevice(
    ContentManager content,
    GraphicsDevice graphicsDevice)
  {
    return (IDisplayDevice) new XnaDisplayDevice(content, graphicsDevice);
  }

  public void Instance_LoadContent()
  {
    Stopwatch stopwatch = Stopwatch.StartNew();
    this.LoadContent();
    stopwatch.Stop();
    Game1.log.Verbose($"Instance_LoadContent() finished, elapsed = '{stopwatch.Elapsed}'");
  }

  /// <summary>LoadContent will be called once per game and is the place to load all of your content.</summary>
  protected override void LoadContent()
  {
    Game1.content = this.CreateContentManager(this.Content.ServiceProvider, this.Content.RootDirectory);
    this.xTileContent = this.CreateContentManager(Game1.content.ServiceProvider, Game1.content.RootDirectory);
    Game1.mapDisplayDevice = this.CreateDisplayDevice((ContentManager) Game1.content, this.GraphicsDevice);
    Game1.spriteBatch = new SpriteBatch(this.GraphicsDevice);
    Game1.netWorldState = new NetRoot<NetWorldState>(new NetWorldState());
    Game1.LoadContentEnumerator = this.GetLoadContentEnumerator();
    if (this.ShouldLoadIncrementally)
      return;
    do
      ;
    while (Game1.LoadContentEnumerator.MoveNext());
    Game1.LoadContentEnumerator = (IEnumerator<int>) null;
    this.AfterLoadContent();
  }

  private void AfterLoadContent()
  {
    Game1.saveOnNewDay = true;
    if (Game1.gameMode == (byte) 4)
    {
      Game1.fadeToBlackAlpha = -0.5f;
      Game1.fadeIn = true;
    }
    if (Game1.random.NextDouble() < 0.7)
    {
      Game1.isDebrisWeather = true;
      Game1.populateDebrisWeatherArray();
    }
    Game1.resetPlayer();
    Game1.CueModification.OnStartup();
    Game1.setGameMode((byte) 0);
  }

  private IEnumerator<int> GetLoadContentEnumerator()
  {
    Game1 game1 = this;
    int step = 0;
    Game1.bigCraftableData = (IDictionary<string, BigCraftableData>) DataLoader.BigCraftables(Game1.content);
    yield return ++step;
    Game1.objectData = (IDictionary<string, ObjectData>) DataLoader.Objects(Game1.content);
    yield return ++step;
    Game1.cropData = (IDictionary<string, CropData>) DataLoader.Crops(Game1.content);
    yield return ++step;
    Game1.characterData = (IDictionary<string, CharacterData>) DataLoader.Characters(Game1.content);
    yield return ++step;
    Game1.pantsData = (IDictionary<string, PantsData>) DataLoader.Pants(Game1.content);
    yield return ++step;
    Game1.shirtData = (IDictionary<string, ShirtData>) DataLoader.Shirts(Game1.content);
    yield return ++step;
    Game1.toolData = (IDictionary<string, ToolData>) DataLoader.Tools(Game1.content);
    yield return ++step;
    Game1.weaponData = (IDictionary<string, WeaponData>) DataLoader.Weapons(Game1.content);
    yield return ++step;
    Game1.achievements = DataLoader.Achievements(Game1.content);
    yield return ++step;
    Game1.buildingData = (IDictionary<string, BuildingData>) DataLoader.Buildings(Game1.content);
    yield return ++step;
    Game1.farmAnimalData = (IDictionary<string, FarmAnimalData>) DataLoader.FarmAnimals(Game1.content);
    yield return ++step;
    Game1.floorPathData = (IDictionary<string, FloorPathData>) DataLoader.FloorsAndPaths(Game1.content);
    yield return ++step;
    Game1.fruitTreeData = (IDictionary<string, FruitTreeData>) DataLoader.FruitTrees(Game1.content);
    yield return ++step;
    Game1.locationData = (IDictionary<string, LocationData>) DataLoader.Locations(Game1.content);
    yield return ++step;
    Game1.locationContextData = (IDictionary<string, LocationContextData>) DataLoader.LocationContexts(Game1.content);
    yield return ++step;
    Game1.petData = (IDictionary<string, PetData>) DataLoader.Pets(Game1.content);
    yield return ++step;
    Game1.NPCGiftTastes = (IDictionary<string, string>) DataLoader.NpcGiftTastes(Game1.content);
    yield return ++step;
    CraftingRecipe.InitShared();
    yield return ++step;
    ItemRegistry.ResetCache();
    yield return ++step;
    Game1.jukeboxTrackData = (IDictionary<string, JukeboxTrackData>) new Dictionary<string, JukeboxTrackData>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    foreach (KeyValuePair<string, JukeboxTrackData> jukeboxTrack in DataLoader.JukeboxTracks(Game1.content))
    {
      if (!Game1.jukeboxTrackData.TryAdd<string, JukeboxTrackData>(jukeboxTrack.Key, jukeboxTrack.Value))
        Game1.log.Warn($"Ignored duplicate ID '{jukeboxTrack.Key}' in Data/JukeboxTracks.");
    }
    yield return ++step;
    Game1.concessionsSpriteSheet = Game1.content.Load<Texture2D>("LooseSprites\\Concessions");
    yield return ++step;
    Game1.birdsSpriteSheet = Game1.content.Load<Texture2D>("LooseSprites\\birds");
    yield return ++step;
    Game1.daybg = Game1.content.Load<Texture2D>("LooseSprites\\daybg");
    yield return ++step;
    Game1.nightbg = Game1.content.Load<Texture2D>("LooseSprites\\nightbg");
    yield return ++step;
    Game1.menuTexture = Game1.content.Load<Texture2D>("Maps\\MenuTiles");
    yield return ++step;
    Game1.uncoloredMenuTexture = Game1.content.Load<Texture2D>("Maps\\MenuTilesUncolored");
    yield return ++step;
    Game1.lantern = Game1.content.Load<Texture2D>("LooseSprites\\Lighting\\lantern");
    yield return ++step;
    Game1.windowLight = Game1.content.Load<Texture2D>("LooseSprites\\Lighting\\windowLight");
    yield return ++step;
    Game1.sconceLight = Game1.content.Load<Texture2D>("LooseSprites\\Lighting\\sconceLight");
    yield return ++step;
    Game1.cauldronLight = Game1.content.Load<Texture2D>("LooseSprites\\Lighting\\greenLight");
    yield return ++step;
    Game1.indoorWindowLight = Game1.content.Load<Texture2D>("LooseSprites\\Lighting\\indoorWindowLight");
    yield return ++step;
    Game1.shadowTexture = Game1.content.Load<Texture2D>("LooseSprites\\shadow");
    yield return ++step;
    Game1.mouseCursors = Game1.content.Load<Texture2D>("LooseSprites\\Cursors");
    yield return ++step;
    Game1.mouseCursors2 = Game1.content.Load<Texture2D>("LooseSprites\\Cursors2");
    yield return ++step;
    Game1.mouseCursors_1_6 = Game1.content.Load<Texture2D>("LooseSprites\\Cursors_1_6");
    yield return ++step;
    Game1.giftboxTexture = Game1.content.Load<Texture2D>("LooseSprites\\Giftbox");
    yield return ++step;
    Game1.controllerMaps = Game1.content.Load<Texture2D>("LooseSprites\\ControllerMaps");
    yield return ++step;
    Game1.animations = Game1.content.Load<Texture2D>("TileSheets\\animations");
    yield return ++step;
    Game1.objectSpriteSheet = Game1.content.Load<Texture2D>("Maps\\springobjects");
    yield return ++step;
    Game1.objectSpriteSheet_2 = Game1.content.Load<Texture2D>("TileSheets\\Objects_2");
    yield return ++step;
    Game1.bobbersTexture = Game1.content.Load<Texture2D>("TileSheets\\bobbers");
    yield return ++step;
    Game1.cropSpriteSheet = Game1.content.Load<Texture2D>("TileSheets\\crops");
    yield return ++step;
    Game1.emoteSpriteSheet = Game1.content.Load<Texture2D>("TileSheets\\emotes");
    yield return ++step;
    Game1.debrisSpriteSheet = Game1.content.Load<Texture2D>("TileSheets\\debris");
    yield return ++step;
    Game1.bigCraftableSpriteSheet = Game1.content.Load<Texture2D>("TileSheets\\Craftables");
    yield return ++step;
    Game1.rainTexture = Game1.content.Load<Texture2D>("TileSheets\\rain");
    yield return ++step;
    Game1.buffsIcons = Game1.content.Load<Texture2D>("TileSheets\\BuffsIcons");
    yield return ++step;
    Tool.weaponsTexture = Game1.content.Load<Texture2D>("TileSheets\\weapons");
    yield return ++step;
    FarmerRenderer.hairStylesTexture = Game1.content.Load<Texture2D>("Characters\\Farmer\\hairstyles");
    yield return ++step;
    FarmerRenderer.shirtsTexture = Game1.content.Load<Texture2D>("Characters\\Farmer\\shirts");
    yield return ++step;
    FarmerRenderer.pantsTexture = Game1.content.Load<Texture2D>("Characters\\Farmer\\pants");
    yield return ++step;
    FarmerRenderer.hatsTexture = Game1.content.Load<Texture2D>("Characters\\Farmer\\hats");
    yield return ++step;
    FarmerRenderer.accessoriesTexture = Game1.content.Load<Texture2D>("Characters\\Farmer\\accessories");
    yield return ++step;
    MapSeat.mapChairTexture = Game1.content.Load<Texture2D>("TileSheets\\ChairTiles");
    yield return ++step;
    SpriteText.spriteTexture = Game1.content.Load<Texture2D>("LooseSprites\\font_bold");
    yield return ++step;
    SpriteText.coloredTexture = Game1.content.Load<Texture2D>("LooseSprites\\font_colored");
    yield return ++step;
    Projectile.projectileSheet = Game1.content.Load<Texture2D>("TileSheets\\Projectiles");
    yield return ++step;
    Color[] data = new Color[1]{ Color.White };
    for (int index1 = 0; index1 < Game1.dynamicPixelRects.Length; ++index1)
    {
      Texture2D[] dynamicPixelRects = Game1.dynamicPixelRects;
      int index2 = index1;
      Texture2D texture2D = new Texture2D(game1.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
      texture2D.Name = $"@{nameof (Game1)}.{"dynamicPixelRects"}[{index1}]";
      dynamicPixelRects[index2] = texture2D;
      Game1.dynamicPixelRects[index1].SetData<Color>(data);
    }
    Game1.fadeToBlackRect = Game1.dynamicPixelRects[0];
    Game1.staminaRect = Game1.dynamicPixelRects[1];
    Game1.lightingRect = Game1.dynamicPixelRects[2];
    yield return ++step;
    Game1.onScreenMenus.Clear();
    Game1.onScreenMenus.Add((IClickableMenu) (Game1.dayTimeMoneyBox = new DayTimeMoneyBox()));
    Game1.onScreenMenus.Add((IClickableMenu) new Toolbar());
    Game1.onScreenMenus.Add((IClickableMenu) (Game1.buffsDisplay = new BuffsDisplay()));
    yield return ++step;
    for (int index = 0; index < 70; ++index)
      Game1.rainDrops[index] = new RainDrop(Game1.random.Next(Game1.viewport.Width), Game1.random.Next(Game1.viewport.Height), Game1.random.Next(4), Game1.random.Next(70));
    yield return ++step;
    Game1.dialogueWidth = Math.Min(1024 /*0x0400*/, Game1.graphics.GraphicsDevice.Viewport.GetTitleSafeArea().Width - 256 /*0x0100*/);
    Game1.dialogueFont = Game1.content.Load<SpriteFont>("Fonts\\SpriteFont1");
    Game1.dialogueFont.LineSpacing = 42;
    yield return ++step;
    Game1.smallFont = Game1.content.Load<SpriteFont>("Fonts\\SmallFont");
    Game1.smallFont.LineSpacing = 28;
    yield return ++step;
    Game1.tinyFont = Game1.content.Load<SpriteFont>("Fonts\\tinyFont");
    yield return ++step;
    Game1._shortDayDisplayName[0] = Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3042");
    yield return ++step;
    Game1._shortDayDisplayName[1] = Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3043");
    yield return ++step;
    Game1._shortDayDisplayName[2] = Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3044");
    yield return ++step;
    Game1._shortDayDisplayName[3] = Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3045");
    yield return ++step;
    Game1._shortDayDisplayName[4] = Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3046");
    yield return ++step;
    Game1._shortDayDisplayName[5] = Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3047");
    yield return ++step;
    Game1._shortDayDisplayName[6] = Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3048");
    yield return ++step;
  }

  public static void resetPlayer()
  {
    Game1.player = new Farmer(new FarmerSprite((string) null), new Vector2(192f, 192f), 1, "", Farmer.initialTools(), true);
  }

  public static void resetVariables()
  {
    Game1.xLocationAfterWarp = 0;
    Game1.yLocationAfterWarp = 0;
    Game1.gameTimeInterval = 0;
    Game1.currentQuestionChoice = 0;
    Game1.currentDialogueCharacterIndex = 0;
    Game1.dialogueTypingInterval = 0;
    Game1.dayOfMonth = 0;
    Game1.year = 1;
    Game1.timeOfDay = 600;
    Game1.timeOfDayAfterFade = -1;
    Game1.facingDirectionAfterWarp = 0;
    Game1.dialogueWidth = 0;
    Game1.facingDirectionAfterWarp = 0;
    Game1.mouseClickPolling = 0;
    Game1.weatherIcon = 0;
    Game1.hitShakeTimer = 0;
    Game1.staminaShakeTimer = 0;
    Game1.pauseThenDoFunctionTimer = 0;
    Game1.weatherForTomorrow = "Sun";
  }

  /// <summary>Play a game sound for the local player.</summary>
  /// <param name="cueName">The sound ID to play.</param>
  /// <param name="pitch">The pitch modifier to apply, or <c>null</c> for the default pitch.</param>
  /// <returns>Returns whether the cue exists and was started successfully.</returns>
  /// <remarks>To play audio in a specific location, see <see cref="M:StardewValley.GameLocation.playSound(System.String,System.Nullable{Microsoft.Xna.Framework.Vector2},System.Nullable{System.Int32},StardewValley.Audio.SoundContext)" /> or <see cref="M:StardewValley.GameLocation.localSound(System.String,System.Nullable{Microsoft.Xna.Framework.Vector2},System.Nullable{System.Int32},StardewValley.Audio.SoundContext)" /> instead.</remarks>
  public static bool playSound(string cueName, int? pitch = null)
  {
    return Game1.sounds.PlayLocal(cueName, (GameLocation) null, new Vector2?(), pitch, SoundContext.Default, out ICue _);
  }

  /// <summary>Play a game sound for the local player.</summary>
  /// <param name="cueName">The sound ID to play.</param>
  /// <param name="cue">The cue instance that was started, or a no-op cue if it failed.</param>
  /// <returns>Returns whether the cue exists and was started successfully.</returns>
  /// <remarks>To play audio in a specific location, see <see cref="M:StardewValley.GameLocation.playSound(System.String,System.Nullable{Microsoft.Xna.Framework.Vector2},System.Nullable{System.Int32},StardewValley.Audio.SoundContext)" /> or <see cref="M:StardewValley.GameLocation.localSound(System.String,System.Nullable{Microsoft.Xna.Framework.Vector2},System.Nullable{System.Int32},StardewValley.Audio.SoundContext)" /> instead.</remarks>
  public static bool playSound(string cueName, out ICue cue)
  {
    return Game1.sounds.PlayLocal(cueName, (GameLocation) null, new Vector2?(), new int?(), SoundContext.Default, out cue);
  }

  /// <summary>Play a game sound for the local player.</summary>
  /// <param name="cueName">The sound ID to play.</param>
  /// <param name="pitch">The pitch modifier to apply.</param>
  /// <param name="cue">The cue instance that was started, or a no-op cue if it failed.</param>
  /// <returns>Returns whether the cue exists and was started successfully.</returns>
  /// <remarks>To play audio in a specific location, see <see cref="M:StardewValley.GameLocation.playSound(System.String,System.Nullable{Microsoft.Xna.Framework.Vector2},System.Nullable{System.Int32},StardewValley.Audio.SoundContext)" /> or <see cref="M:StardewValley.GameLocation.localSound(System.String,System.Nullable{Microsoft.Xna.Framework.Vector2},System.Nullable{System.Int32},StardewValley.Audio.SoundContext)" /> instead.</remarks>
  public static bool playSound(string cueName, int pitch, out ICue cue)
  {
    return Game1.sounds.PlayLocal(cueName, (GameLocation) null, new Vector2?(), new int?(pitch), SoundContext.Default, out cue);
  }

  public static void setRichPresence(string friendlyName, object argument = null)
  {
    if (friendlyName == null)
      return;
    switch (friendlyName.Length)
    {
      case 5:
        if (!(friendlyName == "menus"))
          break;
        Game1.debugPresenceString = "In menus";
        break;
      case 7:
        switch (friendlyName[0])
        {
          case 'f':
            if (!(friendlyName == "fishing"))
              return;
            Game1.debugPresenceString = $"Fishing at {argument}";
            return;
          case 'w':
            if (!(friendlyName == "wedding"))
              return;
            Game1.debugPresenceString = $"Getting married to {argument}";
            return;
          default:
            return;
        }
      case 8:
        switch (friendlyName[0])
        {
          case 'e':
            if (!(friendlyName == "earnings"))
              return;
            Game1.debugPresenceString = $"Made {argument}g last night";
            return;
          case 'f':
            if (!(friendlyName == "festival"))
              return;
            Game1.debugPresenceString = $"At {argument}";
            return;
          case 'l':
            if (!(friendlyName == "location"))
              return;
            Game1.debugPresenceString = $"At {argument}";
            return;
          case 'm':
            if (!(friendlyName == "minigame"))
              return;
            Game1.debugPresenceString = $"Playing {argument}";
            return;
          default:
            return;
        }
      case 9:
        if (!(friendlyName == "giantcrop"))
          break;
        Game1.debugPresenceString = $"Just harvested a Giant {argument}";
        break;
    }
  }

  public static void GenerateBundles(Game1.BundleType bundle_type, bool use_seed = true)
  {
    if (bundle_type == Game1.BundleType.Remixed)
    {
      Random rng = use_seed ? Utility.CreateRandom((double) Game1.uniqueIDForThisGame * 9.0) : new Random();
      Dictionary<string, string> data = new BundleGenerator().Generate(DataLoader.RandomBundles(Game1.content), rng);
      Game1.netWorldState.Value.SetBundleData(data);
    }
    else
      Game1.netWorldState.Value.SetBundleData(DataLoader.Bundles(Game1.content));
  }

  public void SetNewGameOption<T>(string key, T val)
  {
    this.newGameSetupOptions[key] = (object) val;
  }

  public T GetNewGameOption<T>(string key)
  {
    object obj;
    return !this.newGameSetupOptions.TryGetValue(key, out obj) ? default (T) : (T) obj;
  }

  public virtual void loadForNewGame(bool loadedGame = false)
  {
    if (Game1.startingGameSeed.HasValue)
      Game1.uniqueIDForThisGame = Game1.startingGameSeed.Value;
    Game1.specialCurrencyDisplay = new SpecialCurrencyDisplay();
    Game1.flushLocationLookup();
    Game1.locations.Clear();
    Game1.mailbox.Clear();
    Game1.currentLightSources.Clear();
    Game1.questionChoices.Clear();
    Game1.hudMessages.Clear();
    Game1.weddingToday = false;
    Game1.timeOfDay = 600;
    Game1.season = Season.Spring;
    if (!loadedGame)
      Game1.year = 1;
    Game1.dayOfMonth = 0;
    Game1.isQuestion = false;
    Game1.nonWarpFade = false;
    Game1.newDay = false;
    Game1.eventUp = false;
    Game1.viewportFreeze = false;
    Game1.eventOver = false;
    Game1.screenGlow = false;
    Game1.screenGlowHold = false;
    Game1.screenGlowUp = false;
    Game1.isRaining = false;
    Game1.wasGreenRain = false;
    Game1.killScreen = false;
    Game1.messagePause = false;
    Game1.isDebrisWeather = false;
    Game1.weddingToday = false;
    Game1.exitToTitle = false;
    Game1.dialogueUp = false;
    Game1.postExitToTitleCallback = (Action) null;
    Game1.displayHUD = true;
    Game1.messageAfterPause = "";
    Game1.samBandName = Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.2156");
    Game1.background = (Background) null;
    Game1.currentCursorTile = Vector2.Zero;
    if (!loadedGame)
      Game1.lastAppliedSaveFix = SaveMigrator.LatestSaveFix;
    Game1.resetVariables();
    Game1.player.team.sharedDailyLuck.Value = 0.001;
    if (!loadedGame)
    {
      Game1.options = new Options();
      Game1.options.LoadDefaultOptions();
      Game1.initializeVolumeLevels();
    }
    Game1.game1.CheckGamepadMode();
    Game1.onScreenMenus.Add((IClickableMenu) (Game1.chatBox = new ChatBox()));
    Game1.outdoorLight = Color.White;
    Game1.ambientLight = Color.White;
    Game1.UpdateDishOfTheDay();
    Game1.locations.Clear();
    Farm farm = new Farm("Maps\\" + Farm.getMapNameFromTypeInt(Game1.whichFarm), "Farm");
    Game1.locations.Add((GameLocation) farm);
    Game1.AddLocations();
    foreach (GameLocation location in (IEnumerable<GameLocation>) Game1.locations)
      location.AddDefaultBuildings();
    Game1.forceSnapOnNextViewportUpdate = true;
    farm.onNewGame();
    if (!loadedGame)
    {
      foreach (GameLocation location in (IEnumerable<GameLocation>) Game1.locations)
      {
        if (location is IslandLocation islandLocation)
          islandLocation.AddAdditionalWalnutBushes();
      }
    }
    if (!loadedGame)
      Game1.hooks.CreatedInitialLocations();
    else
      Game1.hooks.SaveAddedLocations();
    if (!loadedGame)
      Game1.AddNPCs();
    WarpPathfindingCache.PopulateCache();
    if (!loadedGame)
    {
      Game1.GenerateBundles(Game1.bundleType);
      foreach (string str in Game1.netWorldState.Value.BundleData.Values)
      {
        string[] strArray = ArgUtility.SplitBySpace(str.Split('/')[2]);
        if (Game1.game1.GetNewGameOption<bool>("YearOneCompletable"))
        {
          for (int index = 0; index < strArray.Length; index += 3)
          {
            if (strArray[index] == "266")
            {
              int maxValue = (16 /*0x10*/ - 2) * 2 + 3;
              Random random = Utility.CreateRandom((double) Game1.uniqueIDForThisGame * 12.0);
              Game1.netWorldState.Value.VisitsUntilY1Guarantee = random.Next(2, maxValue);
            }
          }
        }
      }
      Game1.netWorldState.Value.ShuffleMineChests = Game1.game1.GetNewGameOption<Game1.MineChestType>("MineChests");
      if (Game1.game1.newGameSetupOptions.ContainsKey("SpawnMonstersAtNight"))
        Game1.spawnMonstersAtNight = Game1.game1.GetNewGameOption<bool>("SpawnMonstersAtNight");
    }
    Game1.player.ConvertClothingOverrideToClothesItems();
    Game1.player.addQuest("9");
    Game1.RefreshQuestOfTheDay();
    Game1.player.currentLocation = Game1.RequireLocation("FarmHouse");
    Game1.player.gameVersion = Game1.version;
    Game1.hudMessages.Clear();
    Game1.hasLoadedGame = true;
    Game1.setGraphicsForSeason(true);
    if (!loadedGame)
      Game1._setSaveName = false;
    Game1.game1.newGameSetupOptions.Clear();
    Game1.updateCellarAssignments();
    if (loadedGame || !((NetFieldBase<NetWorldState, NetRef<NetWorldState>>) Game1.netWorldState != (NetRef<NetWorldState>) null) || Game1.netWorldState.Value == null)
      return;
    Game1.netWorldState.Value.RegisterSpecialCurrencies();
  }

  public bool IsLocalCoopJoinable()
  {
    return GameRunner.instance.gameInstances.Count < GameRunner.instance.GetMaxSimultaneousPlayers() && !Game1.IsClient;
  }

  public static void StartLocalMultiplayerIfNecessary()
  {
    if (Game1.multiplayerMode != (byte) 0)
      return;
    Game1.log.Verbose("Starting multiplayer server for local multiplayer...");
    Game1.multiplayerMode = (byte) 2;
    if (Game1.server != null)
      return;
    Game1.multiplayer.StartLocalMultiplayerServer();
  }

  public static void EndLocalMultiplayer()
  {
  }

  public static void UpdatePassiveFestivalStates()
  {
    Game1.netWorldState.Value.ActivePassiveFestivals.Clear();
    foreach (KeyValuePair<string, PassiveFestivalData> passiveFestival in DataLoader.PassiveFestivals(Game1.content))
    {
      string key = passiveFestival.Key;
      PassiveFestivalData passiveFestivalData = passiveFestival.Value;
      if (Game1.dayOfMonth >= passiveFestivalData.StartDay && Game1.dayOfMonth <= passiveFestivalData.EndDay && Game1.season == passiveFestivalData.Season && GameStateQuery.CheckConditions(passiveFestivalData.Condition))
        Game1.netWorldState.Value.ActivePassiveFestivals.Add(key);
    }
  }

  public void Instance_UnloadContent() => this.UnloadContent();

  /// <summary>
  /// UnloadContent will be called once per game and is the place to unload
  /// all content.
  /// </summary>
  protected override void UnloadContent()
  {
    base.UnloadContent();
    Game1.spriteBatch.Dispose();
    Game1.content.Unload();
    this.xTileContent.Unload();
    Game1.server?.stopServer();
  }

  public static void showRedMessage(string message, bool playSound = true)
  {
    Game1.addHUDMessage(new HUDMessage(message, 3));
    if (!message.Contains("Inventory") & playSound)
    {
      Game1.playSound("cancel");
    }
    else
    {
      if (!Game1.player.mailReceived.Add("BackpackTip"))
        return;
      Game1.addMailForTomorrow("pierreBackpack");
    }
  }

  public static void showRedMessageUsingLoadString(string loadString, bool playSound = true)
  {
    Game1.showRedMessage(Game1.content.LoadString(loadString), playSound);
  }

  public static bool didPlayerJustLeftClick(bool ignoreNonMouseHeldInput = false)
  {
    return Game1.input.GetMouseState().LeftButton == ButtonState.Pressed && Game1.oldMouseState.LeftButton != ButtonState.Pressed || Game1.input.GetGamePadState().Buttons.X == ButtonState.Pressed && (!ignoreNonMouseHeldInput || !Game1.oldPadState.IsButtonDown(Buttons.X)) || Game1.isOneOfTheseKeysDown(Game1.input.GetKeyboardState(), Game1.options.useToolButton) && (!ignoreNonMouseHeldInput || Game1.areAllOfTheseKeysUp(Game1.oldKBState, Game1.options.useToolButton));
  }

  public static bool didPlayerJustRightClick(bool ignoreNonMouseHeldInput = false)
  {
    return Game1.input.GetMouseState().RightButton == ButtonState.Pressed && Game1.oldMouseState.RightButton != ButtonState.Pressed || Game1.input.GetGamePadState().Buttons.A == ButtonState.Pressed && (!ignoreNonMouseHeldInput || !Game1.oldPadState.IsButtonDown(Buttons.A)) || Game1.isOneOfTheseKeysDown(Game1.input.GetKeyboardState(), Game1.options.actionButton) && (!ignoreNonMouseHeldInput || !Game1.isOneOfTheseKeysDown(Game1.oldKBState, Game1.options.actionButton));
  }

  public static bool didPlayerJustClickAtAll(bool ignoreNonMouseHeldInput = false)
  {
    return Game1.didPlayerJustLeftClick(ignoreNonMouseHeldInput) || Game1.didPlayerJustRightClick(ignoreNonMouseHeldInput);
  }

  public static void showGlobalMessage(string message)
  {
    Game1.addHUDMessage(HUDMessage.ForCornerTextbox(message));
  }

  public static void globalFadeToBlack(Game1.afterFadeFunction afterFade = null, float fadeSpeed = 0.02f)
  {
    Game1.screenFade.GlobalFadeToBlack(afterFade, fadeSpeed);
  }

  public static void globalFadeToClear(Game1.afterFadeFunction afterFade = null, float fadeSpeed = 0.02f)
  {
    Game1.screenFade.GlobalFadeToClear(afterFade, fadeSpeed);
  }

  public void CheckGamepadMode()
  {
    bool gamepadControls = Game1.options.gamepadControls;
    switch (Game1.options.gamepadMode)
    {
      case Options.GamepadModes.ForceOn:
        Game1.options.gamepadControls = true;
        break;
      case Options.GamepadModes.ForceOff:
        Game1.options.gamepadControls = false;
        break;
      default:
        MouseState mouseState = Game1.input.GetMouseState();
        KeyboardState keyboardState = Game1.GetKeyboardState();
        GamePadState gamePadState = Game1.input.GetGamePadState();
        bool flag1 = false;
        if ((mouseState.LeftButton == ButtonState.Pressed || mouseState.MiddleButton == ButtonState.Pressed || mouseState.RightButton == ButtonState.Pressed || mouseState.ScrollWheelValue != this._oldScrollWheelValue || (mouseState.X != this._oldMousePosition.X || mouseState.Y != this._oldMousePosition.Y) && Game1.lastCursorMotionWasMouse || keyboardState.GetPressedKeys().Length != 0) && (keyboardState.GetPressedKeys().Length != 1 || keyboardState.GetPressedKeys()[0] != Keys.Pause))
        {
          flag1 = true;
          if (Program.sdk is SteamHelper sdk && sdk.IsRunningOnSteamDeck())
            flag1 = false;
        }
        this._oldScrollWheelValue = mouseState.ScrollWheelValue;
        this._oldMousePosition.X = mouseState.X;
        this._oldMousePosition.Y = mouseState.Y;
        bool flag2 = Game1.isAnyGamePadButtonBeingPressed() || Game1.isDPadPressed() || Game1.isGamePadThumbstickInMotion() || (double) gamePadState.Triggers.Left != 0.0 || (double) gamePadState.Triggers.Right != 0.0;
        if (this._oldGamepadConnectedState != gamePadState.IsConnected)
        {
          this._oldGamepadConnectedState = gamePadState.IsConnected;
          if (this._oldGamepadConnectedState)
          {
            Game1.options.gamepadControls = true;
            Game1.showGlobalMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.2574"));
          }
          else
          {
            Game1.options.gamepadControls = false;
            if (this.instancePlayerOneIndex != ~PlayerIndex.One)
            {
              Game1.showGlobalMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.2575"));
              if (Game1.CanShowPauseMenu() && Game1.activeClickableMenu == null)
                Game1.activeClickableMenu = (IClickableMenu) new GameMenu();
            }
          }
        }
        if (flag1 && Game1.options.gamepadControls)
          Game1.options.gamepadControls = false;
        if (!Game1.options.gamepadControls & flag2)
          Game1.options.gamepadControls = true;
        if (gamepadControls == Game1.options.gamepadControls || !Game1.options.gamepadControls)
          break;
        Game1.lastMousePositionBeforeFade = new Point(this.localMultiplayerWindow.Width / 2, this.localMultiplayerWindow.Height / 2);
        if (Game1.activeClickableMenu != null)
        {
          Game1.activeClickableMenu.setUpForGamePadMode();
          if (Game1.options.SnappyMenus)
          {
            Game1.activeClickableMenu.populateClickableComponentList();
            Game1.activeClickableMenu.snapToDefaultClickableComponent();
          }
        }
        Game1.timerUntilMouseFade = 0;
        break;
    }
  }

  public void Instance_Update(GameTime gameTime) => this.Update(gameTime);

  protected override void Update(GameTime gameTime)
  {
    GameTime gameTime1 = gameTime;
    DebugTools.BeforeGameUpdate(this, ref gameTime1);
    Game1.input.UpdateStates();
    TimeSpan elapsedGameTime;
    if (Game1.input.GetGamePadState().IsButtonDown(Buttons.RightStick))
    {
      int rightStickHoldTime = Game1.rightStickHoldTime;
      elapsedGameTime = gameTime.ElapsedGameTime;
      int milliseconds = elapsedGameTime.Milliseconds;
      Game1.rightStickHoldTime = rightStickHoldTime + milliseconds;
    }
    GameMenu.bundleItemHovered = false;
    this._update(gameTime1);
    if (Game1.IsMultiplayer && Game1.player != null)
    {
      Game1.player.requestingTimePause.Value = !Game1.shouldTimePass(LocalMultiplayer.IsLocalMultiplayer(true));
      if (Game1.IsMasterGame)
      {
        bool flag = false;
        if (LocalMultiplayer.IsLocalMultiplayer(true))
        {
          flag = true;
          foreach (Farmer onlineFarmer in Game1.getOnlineFarmers())
          {
            if (!onlineFarmer.requestingTimePause.Value)
            {
              flag = false;
              break;
            }
          }
        }
        Game1.netWorldState.Value.IsTimePaused = flag;
      }
    }
    elapsedGameTime = gameTime.ElapsedGameTime;
    Rumble.update((float) elapsedGameTime.Milliseconds);
    if (Game1.options.gamepadControls && Game1.thumbstickMotionMargin > 0)
    {
      int thumbstickMotionMargin = Game1.thumbstickMotionMargin;
      elapsedGameTime = gameTime.ElapsedGameTime;
      int milliseconds = elapsedGameTime.Milliseconds;
      Game1.thumbstickMotionMargin = thumbstickMotionMargin - milliseconds;
    }
    if (!Game1.input.GetGamePadState().IsButtonDown(Buttons.RightStick))
      Game1.rightStickHoldTime = 0;
    base.Update(gameTime);
  }

  public void Instance_OnActivated(object sender, EventArgs args) => this.OnActivated(sender, args);

  protected override void OnActivated(object sender, EventArgs args)
  {
    base.OnActivated(sender, args);
    Game1._activatedTick = Game1.ticks + 1;
    Game1.input.IgnoreKeys(Game1.GetKeyboardState().GetPressedKeys());
  }

  public bool HasKeyboardFocus()
  {
    return Game1.keyboardFocusInstance == null ? this.IsMainInstance : Game1.keyboardFocusInstance == this;
  }

  /// <summary>
  /// Allows the game to run logic such as updating the world,
  /// checking for collisions, gathering input, and playing audio.
  /// </summary>
  /// <param name="gameTime">Provides a snapshot of timing values.</param>
  private void _update(GameTime gameTime)
  {
    if (Game1.graphics.GraphicsDevice == null)
      return;
    bool flag1 = false;
    ++Game1.gameModeTicks;
    if (Game1.options != null && !this.takingMapScreenshot)
    {
      if ((double) Game1.options.baseUIScale != (double) Game1.options.desiredUIScale)
      {
        if ((double) Game1.options.desiredUIScale < 0.0)
          Game1.options.desiredUIScale = Game1.options.desiredBaseZoomLevel;
        Game1.options.baseUIScale = Game1.options.desiredUIScale;
        flag1 = true;
      }
      if ((double) Game1.options.desiredBaseZoomLevel != (double) Game1.options.baseZoomLevel)
      {
        Game1.options.baseZoomLevel = Game1.options.desiredBaseZoomLevel;
        Game1.forceSnapOnNextViewportUpdate = true;
        flag1 = true;
      }
    }
    if (flag1)
      this.refreshWindowSettings();
    if (!this.ShouldLoadIncrementally)
      this.CheckGamepadMode();
    FarmAnimal.NumPathfindingThisTick = 0;
    Game1.options.reApplySetOptions();
    if (Game1.toggleFullScreen)
    {
      Game1.toggleFullscreen();
      Game1.toggleFullScreen = false;
    }
    Game1.input.Update();
    if (Game1.frameByFrame)
    {
      KeyboardState keyboardState = Game1.GetKeyboardState();
      if (keyboardState.IsKeyDown(Keys.Escape) && Game1.oldKBState.IsKeyUp(Keys.Escape))
        Game1.frameByFrame = false;
      keyboardState = Game1.GetKeyboardState();
      if ((!keyboardState.IsKeyDown(Keys.G) ? 0 : (Game1.oldKBState.IsKeyUp(Keys.G) ? 1 : 0)) == 0)
      {
        Game1.oldKBState = Game1.GetKeyboardState();
        return;
      }
    }
    if (Game1.client != null && Game1.client.timedOut)
      Game1.multiplayer.clientRemotelyDisconnected(Game1.client.pendingDisconnect);
    if (Game1._newDayTask != null)
    {
      if (Game1._newDayTask.Status == TaskStatus.Created)
        Game1.hooks.StartTask(Game1._newDayTask, "NewDay");
      if (Game1._newDayTask.Status >= TaskStatus.RanToCompletion)
      {
        if (Game1._newDayTask.IsFaulted)
        {
          Exception baseException = Game1._newDayTask.Exception.GetBaseException();
          if (!Game1.IsMasterGame)
          {
            if (baseException is AbortNetSynchronizerException)
              Game1.log.Verbose("_newDayTask failed: client lost connection to the server");
            else
              Game1.log.Error("Client _newDayTask failed with an exception:", baseException);
            Game1.multiplayer.clientRemotelyDisconnected(Multiplayer.DisconnectType.ClientTimeout);
            Game1._newDayTask = (Task) null;
            Utility.CollectGarbage();
            return;
          }
          Game1.log.Error("_newDayTask failed with an exception:", baseException);
          throw new Exception($"Error on new day: \n---------------\n{baseException}\n---------------\n");
        }
        Game1._newDayTask = (Task) null;
        Utility.CollectGarbage();
      }
      Game1.UpdateChatBox();
    }
    else if (this.isLocalMultiplayerNewDayActive)
      Game1.UpdateChatBox();
    else if (this.IsSaving)
    {
      Game1.PushUIMode();
      Game1.activeClickableMenu?.update(gameTime);
      if (Game1.overlayMenu != null)
      {
        Game1.overlayMenu.update(gameTime);
        if (Game1.overlayMenu == null)
        {
          Game1.PopUIMode();
          return;
        }
      }
      Game1.PopUIMode();
      Game1.UpdateChatBox();
    }
    else
    {
      if (Game1.exitToTitle)
      {
        Game1.exitToTitle = false;
        this.CleanupReturningToTitle();
        Utility.CollectGarbage();
        Action exitToTitleCallback = Game1.postExitToTitleCallback;
        if (exitToTitleCallback != null)
          exitToTitleCallback();
      }
      TimeSpan timeSpan = gameTime.ElapsedGameTime;
      Game1.SetFreeCursorElapsed((float) timeSpan.TotalSeconds);
      Program.sdk.Update();
      if (Game1.game1.IsMainInstance)
      {
        Game1.keyboardFocusInstance = Game1.game1;
        foreach (Game1 gameInstance in GameRunner.instance.gameInstances)
        {
          if (gameInstance.instanceKeyboardDispatcher.Subscriber != null && gameInstance.instanceTextEntry != null)
          {
            Game1.keyboardFocusInstance = gameInstance;
            break;
          }
        }
      }
      if (this.IsMainInstance)
      {
        int displayIndex = this.Window.GetDisplayIndex();
        if (this._lastUsedDisplay != -1 && this._lastUsedDisplay != displayIndex)
        {
          StartupPreferences startupPreferences = new StartupPreferences();
          startupPreferences.loadPreferences(false, false);
          startupPreferences.displayIndex = displayIndex;
          startupPreferences.savePreferences(false);
        }
        this._lastUsedDisplay = displayIndex;
      }
      if (this.HasKeyboardFocus())
        Game1.keyboardDispatcher.Poll();
      else
        Game1.keyboardDispatcher.Discard();
      if (Game1.gameMode == (byte) 6)
        Game1.multiplayer.UpdateLoading();
      if (Game1.gameMode == (byte) 3)
      {
        Game1.multiplayer.UpdateEarly();
        Game1.dedicatedServer.Tick();
        if (Game1.player?.team != null)
          Game1.player.team.Update();
      }
      if ((Game1.paused || !this.IsActiveNoOverlay && Program.releaseBuild) && (Game1.options == null || Game1.options.pauseWhenOutOfFocus || Game1.paused) && Game1.multiplayerMode == (byte) 0)
      {
        Game1.UpdateChatBox();
      }
      else
      {
        if (Game1.quit)
          this.Exit();
        Game1.currentGameTime = gameTime;
        if (Game1.gameMode != (byte) 11 && !this.ShouldLoadIncrementally)
        {
          ++Game1.ticks;
          if (this.IsActiveNoOverlay)
            this.checkForEscapeKeys();
          Game1.updateMusic();
          Game1.updateRaindropPosition();
          if (Game1.globalFade)
            Game1.screenFade.UpdateGlobalFade();
          else if (Game1.pauseThenDoFunctionTimer > 0)
          {
            Game1.freezeControls = true;
            int thenDoFunctionTimer = Game1.pauseThenDoFunctionTimer;
            timeSpan = gameTime.ElapsedGameTime;
            int milliseconds = timeSpan.Milliseconds;
            Game1.pauseThenDoFunctionTimer = thenDoFunctionTimer - milliseconds;
            if (Game1.pauseThenDoFunctionTimer <= 0)
            {
              Game1.freezeControls = false;
              Game1.afterFadeFunction afterPause = Game1.afterPause;
              if (afterPause != null)
                afterPause();
            }
          }
          int num1;
          if (Game1.options.gamepadControls)
          {
            bool? nullable = Game1.activeClickableMenu?.shouldClampGamePadCursor();
            num1 = !nullable.HasValue ? 0 : (nullable.GetValueOrDefault() ? 1 : 0);
          }
          else
            num1 = 0;
          if (num1 != 0)
          {
            Point mousePositionRaw = Game1.getMousePositionRaw();
            Microsoft.Xna.Framework.Rectangle rectangle = new Microsoft.Xna.Framework.Rectangle(0, 0, this.localMultiplayerWindow.Width, this.localMultiplayerWindow.Height);
            if (mousePositionRaw.X < rectangle.X)
              mousePositionRaw.X = rectangle.X;
            else if (mousePositionRaw.X > rectangle.Right)
              mousePositionRaw.X = rectangle.Right;
            if (mousePositionRaw.Y < rectangle.Y)
              mousePositionRaw.Y = rectangle.Y;
            else if (mousePositionRaw.Y > rectangle.Bottom)
              mousePositionRaw.Y = rectangle.Bottom;
            Game1.setMousePositionRaw(mousePositionRaw.X, mousePositionRaw.Y);
          }
          if (Game1.gameMode == (byte) 3 || Game1.gameMode == (byte) 2)
          {
            if (!Game1.warpingForForcedRemoteEvent && !Game1.eventUp && !Game1.dialogueUp && Game1.remoteEventQueue.Count > 0 && Game1.player != null && Game1.player.isCustomized.Value && (!Game1.fadeIn || (double) Game1.fadeToBlackAlpha <= 0.0))
            {
              if (Game1.activeClickableMenu != null)
              {
                Game1.activeClickableMenu.emergencyShutDown();
                Game1.exitActiveMenu();
              }
              else if (Game1.currentMinigame != null && Game1.currentMinigame.forceQuit())
                Game1.currentMinigame = (IMinigame) null;
              if (Game1.activeClickableMenu == null && Game1.currentMinigame == null && Game1.player.freezePause <= 0)
              {
                Action remoteEvent = Game1.remoteEventQueue[0];
                Game1.remoteEventQueue.RemoveAt(0);
                remoteEvent();
              }
            }
            Farmer player = Game1.player;
            long millisecondsPlayed = (long) player.millisecondsPlayed;
            timeSpan = gameTime.ElapsedGameTime;
            long milliseconds1 = (long) (uint) timeSpan.Milliseconds;
            player.millisecondsPlayed = (ulong) (millisecondsPlayed + milliseconds1);
            bool flag2 = true;
            if (Game1.currentMinigame != null && !Game1.HostPaused)
            {
              if ((double) Game1.pauseTime > 0.0)
                Game1.updatePause(gameTime);
              if (Game1.fadeToBlack)
              {
                Game1.screenFade.UpdateFadeAlpha(gameTime);
                if ((double) Game1.fadeToBlackAlpha >= 1.0)
                  Game1.fadeToBlack = false;
              }
              else
              {
                if (Game1.thumbstickMotionMargin > 0)
                {
                  int thumbstickMotionMargin = Game1.thumbstickMotionMargin;
                  timeSpan = gameTime.ElapsedGameTime;
                  int milliseconds2 = timeSpan.Milliseconds;
                  Game1.thumbstickMotionMargin = thumbstickMotionMargin - milliseconds2;
                }
                KeyboardState keyboardState = new KeyboardState();
                MouseState mouseState = new MouseState();
                GamePadState gamePadState = new GamePadState();
                if (this.IsActive)
                {
                  keyboardState = Game1.GetKeyboardState();
                  mouseState = Game1.input.GetMouseState();
                  gamePadState = Game1.input.GetGamePadState();
                  bool? nullable = Game1.chatBox?.isActive();
                  if ((!nullable.HasValue || !nullable.GetValueOrDefault() ? (Game1.textEntry != null ? 1 : 0) : 1) != 0)
                  {
                    keyboardState = new KeyboardState();
                    gamePadState = new GamePadState();
                  }
                  else
                  {
                    foreach (Keys pressedKey in keyboardState.GetPressedKeys())
                    {
                      if (!Game1.oldKBState.IsKeyDown(pressedKey) && Game1.currentMinigame != null)
                        Game1.currentMinigame.receiveKeyPress(pressedKey);
                    }
                    if (Game1.options.gamepadControls)
                    {
                      if (Game1.currentMinigame == null)
                      {
                        Game1.oldMouseState = mouseState;
                        Game1.oldKBState = keyboardState;
                        Game1.oldPadState = gamePadState;
                        Game1.UpdateChatBox();
                        return;
                      }
                      foreach (Buttons pressedButton in Utility.getPressedButtons(gamePadState, Game1.oldPadState))
                        Game1.currentMinigame?.receiveKeyPress(Utility.mapGamePadButtonToKey(pressedButton));
                      if (Game1.currentMinigame == null)
                      {
                        Game1.oldMouseState = mouseState;
                        Game1.oldKBState = keyboardState;
                        Game1.oldPadState = gamePadState;
                        Game1.UpdateChatBox();
                        return;
                      }
                      GamePadThumbSticks thumbSticks = gamePadState.ThumbSticks;
                      if ((double) thumbSticks.Right.Y < -0.20000000298023224)
                      {
                        thumbSticks = Game1.oldPadState.ThumbSticks;
                        if ((double) thumbSticks.Right.Y >= -0.20000000298023224)
                          Game1.currentMinigame.receiveKeyPress(Keys.Down);
                      }
                      thumbSticks = gamePadState.ThumbSticks;
                      if ((double) thumbSticks.Right.Y > 0.20000000298023224)
                      {
                        thumbSticks = Game1.oldPadState.ThumbSticks;
                        if ((double) thumbSticks.Right.Y <= 0.20000000298023224)
                          Game1.currentMinigame.receiveKeyPress(Keys.Up);
                      }
                      thumbSticks = gamePadState.ThumbSticks;
                      if ((double) thumbSticks.Right.X < -0.20000000298023224)
                      {
                        thumbSticks = Game1.oldPadState.ThumbSticks;
                        if ((double) thumbSticks.Right.X >= -0.20000000298023224)
                          Game1.currentMinigame.receiveKeyPress(Keys.Left);
                      }
                      thumbSticks = gamePadState.ThumbSticks;
                      if ((double) thumbSticks.Right.X > 0.20000000298023224)
                      {
                        thumbSticks = Game1.oldPadState.ThumbSticks;
                        if ((double) thumbSticks.Right.X <= 0.20000000298023224)
                          Game1.currentMinigame.receiveKeyPress(Keys.Right);
                      }
                      thumbSticks = Game1.oldPadState.ThumbSticks;
                      if ((double) thumbSticks.Right.Y < -0.20000000298023224)
                      {
                        thumbSticks = gamePadState.ThumbSticks;
                        if ((double) thumbSticks.Right.Y >= -0.20000000298023224)
                          Game1.currentMinigame.receiveKeyRelease(Keys.Down);
                      }
                      thumbSticks = Game1.oldPadState.ThumbSticks;
                      if ((double) thumbSticks.Right.Y > 0.20000000298023224)
                      {
                        thumbSticks = gamePadState.ThumbSticks;
                        if ((double) thumbSticks.Right.Y <= 0.20000000298023224)
                          Game1.currentMinigame.receiveKeyRelease(Keys.Up);
                      }
                      thumbSticks = Game1.oldPadState.ThumbSticks;
                      if ((double) thumbSticks.Right.X < -0.20000000298023224)
                      {
                        thumbSticks = gamePadState.ThumbSticks;
                        if ((double) thumbSticks.Right.X >= -0.20000000298023224)
                          Game1.currentMinigame.receiveKeyRelease(Keys.Left);
                      }
                      thumbSticks = Game1.oldPadState.ThumbSticks;
                      if ((double) thumbSticks.Right.X > 0.20000000298023224)
                      {
                        thumbSticks = gamePadState.ThumbSticks;
                        if ((double) thumbSticks.Right.X <= 0.20000000298023224)
                          Game1.currentMinigame.receiveKeyRelease(Keys.Right);
                      }
                      if (Game1.isGamePadThumbstickInMotion() && Game1.currentMinigame != null && !Game1.currentMinigame.overrideFreeMouseMovement())
                      {
                        int mouseX = Game1.getMouseX();
                        thumbSticks = gamePadState.ThumbSticks;
                        int num2 = (int) ((double) thumbSticks.Left.X * (double) Game1.thumbstickToMouseModifier);
                        int x = mouseX + num2;
                        int mouseY = Game1.getMouseY();
                        thumbSticks = gamePadState.ThumbSticks;
                        int num3 = (int) ((double) thumbSticks.Left.Y * (double) Game1.thumbstickToMouseModifier);
                        int y = mouseY - num3;
                        Game1.setMousePosition(x, y);
                      }
                      else if (Game1.getMouseX() != Game1.getOldMouseX() || Game1.getMouseY() != Game1.getOldMouseY())
                        Game1.lastCursorMotionWasMouse = true;
                    }
                    foreach (Keys pressedKey in Game1.oldKBState.GetPressedKeys())
                    {
                      if (!keyboardState.IsKeyDown(pressedKey) && Game1.currentMinigame != null)
                        Game1.currentMinigame.receiveKeyRelease(pressedKey);
                    }
                    if (Game1.options.gamepadControls)
                    {
                      if (Game1.currentMinigame == null)
                      {
                        Game1.oldMouseState = mouseState;
                        Game1.oldKBState = keyboardState;
                        Game1.oldPadState = gamePadState;
                        Game1.UpdateChatBox();
                        return;
                      }
                      if (gamePadState.IsConnected)
                      {
                        if (gamePadState.IsButtonDown(Buttons.X) && !Game1.oldPadState.IsButtonDown(Buttons.X))
                          Game1.currentMinigame.receiveRightClick(Game1.getMouseX(), Game1.getMouseY());
                        else if (gamePadState.IsButtonDown(Buttons.A) && !Game1.oldPadState.IsButtonDown(Buttons.A))
                          Game1.currentMinigame.receiveLeftClick(Game1.getMouseX(), Game1.getMouseY());
                        else if (!gamePadState.IsButtonDown(Buttons.X) && Game1.oldPadState.IsButtonDown(Buttons.X))
                          Game1.currentMinigame.releaseRightClick(Game1.getMouseX(), Game1.getMouseY());
                        else if (!gamePadState.IsButtonDown(Buttons.A) && Game1.oldPadState.IsButtonDown(Buttons.A))
                          Game1.currentMinigame.releaseLeftClick(Game1.getMouseX(), Game1.getMouseY());
                      }
                      foreach (Buttons pressedButton in Utility.getPressedButtons(Game1.oldPadState, gamePadState))
                        Game1.currentMinigame?.receiveKeyRelease(Utility.mapGamePadButtonToKey(pressedButton));
                      if (gamePadState.IsConnected && gamePadState.IsButtonDown(Buttons.A) && Game1.currentMinigame != null)
                        Game1.currentMinigame.leftClickHeld(0, 0);
                    }
                    if (Game1.currentMinigame == null)
                    {
                      Game1.oldMouseState = mouseState;
                      Game1.oldKBState = keyboardState;
                      Game1.oldPadState = gamePadState;
                      Game1.UpdateChatBox();
                      return;
                    }
                    if (Game1.currentMinigame != null && mouseState.LeftButton == ButtonState.Pressed && Game1.oldMouseState.LeftButton != ButtonState.Pressed)
                      Game1.currentMinigame.receiveLeftClick(Game1.getMouseX(), Game1.getMouseY());
                    if (Game1.currentMinigame != null && mouseState.RightButton == ButtonState.Pressed && Game1.oldMouseState.RightButton != ButtonState.Pressed)
                      Game1.currentMinigame.receiveRightClick(Game1.getMouseX(), Game1.getMouseY());
                    if (Game1.currentMinigame != null && mouseState.LeftButton == ButtonState.Released && Game1.oldMouseState.LeftButton == ButtonState.Pressed)
                      Game1.currentMinigame.releaseLeftClick(Game1.getMouseX(), Game1.getMouseY());
                    if (Game1.currentMinigame != null && mouseState.RightButton == ButtonState.Released && Game1.oldMouseState.RightButton == ButtonState.Pressed)
                      Game1.currentMinigame.releaseLeftClick(Game1.getMouseX(), Game1.getMouseY());
                    if (Game1.currentMinigame != null && mouseState.LeftButton == ButtonState.Pressed && Game1.oldMouseState.LeftButton == ButtonState.Pressed)
                      Game1.currentMinigame.leftClickHeld(Game1.getMouseX(), Game1.getMouseY());
                  }
                }
                if (Game1.currentMinigame != null && Game1.currentMinigame.tick(gameTime))
                {
                  Game1.oldMouseState = mouseState;
                  Game1.oldKBState = keyboardState;
                  Game1.oldPadState = gamePadState;
                  Game1.currentMinigame?.unload();
                  Game1.currentMinigame = (IMinigame) null;
                  Game1.fadeIn = true;
                  Game1.fadeToBlackAlpha = 1f;
                  Game1.UpdateChatBox();
                  return;
                }
                if (Game1.currentMinigame == null && Game1.IsMusicContextActive(MusicContext.MiniGame))
                  Game1.stopMusicTrack(MusicContext.MiniGame);
                Game1.oldMouseState = mouseState;
                Game1.oldKBState = keyboardState;
                Game1.oldPadState = gamePadState;
              }
              flag2 = Game1.IsMultiplayer || Game1.currentMinigame == null || Game1.currentMinigame.doMainGameUpdates();
            }
            else if (Game1.farmEvent != null && !Game1.HostPaused && Game1.farmEvent.tickUpdate(gameTime))
            {
              Game1.farmEvent.makeChangesToLocation();
              Game1.timeOfDay = 600;
              Game1.outdoorLight = Color.White;
              Game1.displayHUD = true;
              Game1.farmEvent = (FarmEvent) null;
              Game1.netWorldState.Value.WriteToGame1();
              Game1.currentLocation = Game1.player.currentLocation;
              LocationRequest locationRequest = Game1.getLocationRequest(Game1.currentLocation.Name);
              locationRequest.OnWarp += (LocationRequest.Callback) (() =>
              {
                if (Game1.currentLocation is FarmHouse currentLocation2)
                {
                  Game1.player.Position = Utility.PointToVector2(currentLocation2.GetPlayerBedSpot()) * 64f;
                  BedFurniture.ShiftPositionForBed(Game1.player);
                }
                else
                  BedFurniture.ApplyWakeUpPosition(Game1.player);
                if (Game1.player.IsSitting())
                  Game1.player.StopSitting(false);
                Game1.changeMusicTrack("none", true);
                Game1.player.forceCanMove();
                Game1.freezeControls = false;
                Game1.displayFarmer = true;
                Game1.viewportFreeze = false;
                Game1.fadeToBlackAlpha = 0.0f;
                Game1.fadeToBlack = false;
                Game1.globalFadeToClear();
                Game1.RemoveDeliveredMailForTomorrow();
                Game1.handlePostFarmEventActions();
                Game1.showEndOfNightStuff();
              });
              Game1.warpFarmer(locationRequest, 5, 9, Game1.player.FacingDirection);
              Game1.fadeToBlackAlpha = 1.1f;
              Game1.fadeToBlack = true;
              Game1.nonWarpFade = false;
              Game1.UpdateOther(gameTime);
            }
            if (flag2)
            {
              if (Game1.endOfNightMenus.Count > 0 && Game1.activeClickableMenu == null)
              {
                Game1.activeClickableMenu = Game1.endOfNightMenus.Pop();
                if (Game1.activeClickableMenu != null && Game1.options.SnappyMenus)
                  Game1.activeClickableMenu.snapToDefaultClickableComponent();
              }
              Game1.specialCurrencyDisplay?.Update(gameTime);
              if (Game1.currentLocation != null && Game1.currentMinigame == null)
              {
                if (Game1.emoteMenu != null)
                {
                  Game1.emoteMenu.update(gameTime);
                  if (Game1.emoteMenu != null)
                  {
                    Game1.PushUIMode();
                    Game1.emoteMenu.performHoverAction(Game1.getMouseX(), Game1.getMouseY());
                    KeyboardState keyboardState = Game1.GetKeyboardState();
                    if (Game1.input.GetMouseState().LeftButton == ButtonState.Pressed && Game1.oldMouseState.LeftButton == ButtonState.Released)
                      Game1.emoteMenu.receiveLeftClick(Game1.getMouseX(), Game1.getMouseY(), true);
                    else if (Game1.input.GetMouseState().RightButton == ButtonState.Pressed && Game1.oldMouseState.RightButton == ButtonState.Released)
                      Game1.emoteMenu.receiveRightClick(Game1.getMouseX(), Game1.getMouseY(), true);
                    else if (Game1.isOneOfTheseKeysDown(keyboardState, Game1.options.menuButton) || Game1.isOneOfTheseKeysDown(keyboardState, Game1.options.emoteButton) && Game1.areAllOfTheseKeysUp(Game1.oldKBState, Game1.options.emoteButton))
                      Game1.emoteMenu.exitThisMenu(false);
                    Game1.PopUIMode();
                    Game1.oldKBState = keyboardState;
                    Game1.oldMouseState = Game1.input.GetMouseState();
                  }
                }
                else if (Game1.textEntry != null)
                {
                  Game1.PushUIMode();
                  Game1.updateTextEntry(gameTime);
                  Game1.PopUIMode();
                }
                else if (Game1.activeClickableMenu != null)
                {
                  Game1.PushUIMode();
                  Game1.updateActiveMenu(gameTime);
                  Game1.PopUIMode();
                }
                else
                {
                  if ((double) Game1.pauseTime > 0.0)
                    Game1.updatePause(gameTime);
                  if (!Game1.globalFade && !Game1.freezeControls && Game1.activeClickableMenu == null && (this.IsActiveNoOverlay || Game1.inputSimulator != null))
                    this.UpdateControlInput(gameTime);
                }
              }
              if (Game1.showingEndOfNightStuff && Game1.endOfNightMenus.Count == 0 && Game1.activeClickableMenu == null)
              {
                Game1.newDaySync.destroy();
                Game1.player.team.endOfNightStatus.WithdrawState();
                Game1.showingEndOfNightStuff = false;
                Action afterNewDayAction = Game1._afterNewDayAction;
                if (afterNewDayAction != null)
                {
                  Game1._afterNewDayAction = (Action) null;
                  afterNewDayAction();
                }
                Game1.player.ReequipEnchantments();
                Game1.globalFadeToClear(new Game1.afterFadeFunction(Game1.doMorningStuff));
              }
              if (Game1.currentLocation != null)
              {
                if (!Game1.HostPaused && !Game1.showingEndOfNightStuff)
                {
                  if (Game1.IsMultiplayer || Game1.activeClickableMenu == null && Game1.currentMinigame == null || Game1.player.viewingLocation.Value != null)
                    Game1.UpdateGameClock(gameTime);
                  this.UpdateCharacters(gameTime);
                  this.UpdateLocations(gameTime);
                  if (Game1.currentMinigame == null)
                  {
                    Game1.UpdateViewPort(false, this.getViewportCenter());
                  }
                  else
                  {
                    Game1.previousViewportPosition.X = (float) Game1.viewport.X;
                    Game1.previousViewportPosition.Y = (float) Game1.viewport.Y;
                  }
                  Game1.UpdateOther(gameTime);
                }
                if (Game1.messagePause)
                {
                  KeyboardState keyboardState = Game1.GetKeyboardState();
                  MouseState mouseState = Game1.input.GetMouseState();
                  GamePadState gamePadState = Game1.input.GetGamePadState();
                  if (Game1.isOneOfTheseKeysDown(keyboardState, Game1.options.actionButton) && !Game1.isOneOfTheseKeysDown(Game1.oldKBState, Game1.options.actionButton))
                    Game1.pressActionButton(keyboardState, mouseState, gamePadState);
                  Game1.oldKBState = keyboardState;
                  Game1.oldPadState = gamePadState;
                }
              }
            }
            else if (Game1.textEntry != null)
            {
              Game1.PushUIMode();
              Game1.updateTextEntry(gameTime);
              Game1.PopUIMode();
            }
          }
          else
          {
            this.UpdateTitleScreen(gameTime);
            if (Game1.textEntry != null)
            {
              Game1.PushUIMode();
              Game1.updateTextEntry(gameTime);
              Game1.PopUIMode();
            }
            else if (Game1.activeClickableMenu != null)
            {
              Game1.PushUIMode();
              Game1.updateActiveMenu(gameTime);
              Game1.PopUIMode();
            }
            if (Game1.gameMode == (byte) 10)
              Game1.UpdateOther(gameTime);
          }
          Game1.audioEngine?.Update();
          Game1.UpdateChatBox();
          if (Game1.gameMode != (byte) 6)
            Game1.multiplayer.UpdateLate();
        }
        else if (this.ShouldLoadIncrementally)
        {
          Stopwatch stopwatch = Stopwatch.StartNew();
          while (Game1.LoadContentEnumerator.MoveNext())
          {
            timeSpan = stopwatch.Elapsed;
            if (timeSpan.TotalMilliseconds >= 25.0)
              goto label_256;
          }
          Game1.FinishedFirstLoadContent = true;
label_256:
          if (Game1.FinishedFirstLoadContent && Game1.FinishedFirstInitSounds && Game1.FinishedFirstInitSerializers)
          {
            Game1.FinishedIncrementalLoad = true;
            this.AfterLoadContent();
          }
        }
        if (Game1.gameMode != (byte) 3 || Game1.gameModeTicks != 1)
          return;
        Game1.OnDayStarted();
      }
    }
  }

  /// <summary>Handle the new day starting after the player saves, loads, or connects.</summary>
  public static void OnDayStarted()
  {
    TriggerActionManager.Raise("DayStarted");
    Utility.ForEachLocation((Func<GameLocation, bool>) (location =>
    {
      location.OnDayStarted();
      return true;
    }));
    Utility.fixAllAnimals();
    foreach (NPC allCharacter in Utility.getAllCharacters())
      allCharacter.OnDayStarted();
    Utility.ForEachLocation((Func<GameLocation, bool>) (location =>
    {
      foreach (FarmAnimal farmAnimal in location.animals.Values)
        farmAnimal.OnDayStarted();
      return true;
    }));
    Game1.player.currentLocation.resetForPlayerEntry();
    if (!Game1.hasStartedDay)
    {
      foreach (string constructedBuilding in (NetHashSet<string>) Game1.player.team.constructedBuildings)
      {
        string buildingType = constructedBuilding;
        Game1.player.NotifyQuests((Func<Quest, bool>) (quest => quest.OnBuildingExists(buildingType)));
      }
      if (Stats.AllowRetroactiveAchievements)
      {
        foreach (int achievement in (NetHashSet<int>) Game1.player.achievements)
          Game1.getPlatformAchievement(achievement.ToString());
      }
      Game1.hasStartedDay = true;
    }
    if (!Game1.IsMasterGame)
      return;
    Woods.ResetLostItemsShop();
  }

  public static void PerformPassiveFestivalSetup()
  {
    foreach (string activePassiveFestival in (IEnumerable<string>) Game1.netWorldState.Value.ActivePassiveFestivals)
    {
      PassiveFestivalData data;
      if (Utility.TryGetPassiveFestivalData(activePassiveFestival, out data) && data.DailySetupMethod != null)
      {
        FestivalDailySetupDelegate createdDelegate;
        string error;
        if (StaticDelegateBuilder.TryCreateDelegate<FestivalDailySetupDelegate>(data.DailySetupMethod, out createdDelegate, out error))
          createdDelegate();
        else
          Game1.log.Warn($"Passive festival '{activePassiveFestival}' has invalid daily setup method '{data.DailySetupMethod}': {error}");
      }
    }
  }

  public static int CurrentPlayerLimit
  {
    get
    {
      if (Game1.netWorldState?.Value == null)
        return Game1.multiplayer.playerLimit;
      int currentPlayerLimit = Game1.netWorldState.Value.CurrentPlayerLimit;
      return Game1.netWorldState.Value.CurrentPlayerLimit;
    }
  }

  public static void showTextEntry(TextBox text_box)
  {
    Game1.timerUntilMouseFade = 0;
    Game1.PushUIMode();
    Game1.textEntry = new TextEntryMenu(text_box);
    Game1.PopUIMode();
  }

  public static void closeTextEntry()
  {
    if (Game1.textEntry != null)
      Game1.textEntry = (TextEntryMenu) null;
    if (Game1.activeClickableMenu == null || !Game1.options.SnappyMenus)
      return;
    if (Game1.activeClickableMenu is TitleMenu && TitleMenu.subMenu != null)
      TitleMenu.subMenu.snapCursorToCurrentSnappedComponent();
    else
      Game1.activeClickableMenu.snapCursorToCurrentSnappedComponent();
  }

  public static bool isDarkOut(GameLocation location)
  {
    return Game1.timeOfDay >= Game1.getTrulyDarkTime(location);
  }

  public static bool isTimeToTurnOffLighting(GameLocation location)
  {
    return Game1.timeOfDay >= Game1.getTrulyDarkTime(location) - 100;
  }

  public static bool isStartingToGetDarkOut(GameLocation location)
  {
    return Game1.timeOfDay >= Game1.getStartingToGetDarkTime(location);
  }

  public static int getStartingToGetDarkTime(GameLocation location)
  {
    if (location != null && location.InIslandContext())
      return 1800;
    switch (Game1.season)
    {
      case Season.Fall:
        return 1700;
      case Season.Winter:
        return 1500;
      default:
        return 1800;
    }
  }

  public static void updateCellarAssignments()
  {
    if (!Game1.IsMasterGame)
      return;
    Game1.player.team.cellarAssignments[1] = Game1.MasterPlayer.UniqueMultiplayerID;
    for (int key = 2; key <= Game1.netWorldState.Value.HighestPlayerLimit; ++key)
    {
      string name = "Cellar" + key.ToString();
      if (key != 1 && Game1.getLocationFromName(name) != null)
      {
        long id;
        if (Game1.player.team.cellarAssignments.TryGetValue(key, out id))
        {
          if (Game1.GetPlayer(id) == null)
            Game1.player.team.cellarAssignments.Remove(key);
          else
            continue;
        }
        foreach (Farmer allFarmer in Game1.getAllFarmers())
        {
          if (!Game1.player.team.cellarAssignments.Values.Contains<long>(allFarmer.UniqueMultiplayerID))
          {
            Game1.player.team.cellarAssignments[key] = allFarmer.UniqueMultiplayerID;
            break;
          }
        }
      }
    }
  }

  public static int getModeratelyDarkTime(GameLocation location)
  {
    return (Game1.getTrulyDarkTime(location) + Game1.getStartingToGetDarkTime(location)) / 2;
  }

  public static int getTrulyDarkTime(GameLocation location)
  {
    return Game1.getStartingToGetDarkTime(location) + 200;
  }

  public static void playMorningSong(bool ignoreDelay = false)
  {
    if (!Game1.eventUp && Game1.dayOfMonth > 0)
    {
      LocationData data = Game1.currentLocation.GetData();
      if (Game1.currentLocation.GetLocationSpecificMusic() != null && (data != null ? (!data.MusicIsTownTheme ? 1 : 0) : 1) != 0)
      {
        Game1.changeMusicTrack("none", true);
        GameLocation.HandleMusicChange((GameLocation) null, Game1.currentLocation);
      }
      else if (Game1.IsRainingHere())
      {
        if (ignoreDelay)
          PlayRain();
        else
          Game1.morningSongPlayAction = DelayedAction.functionAfterDelay(new Action(PlayRain), 500);
      }
      else
      {
        LocationContextData context = Game1.currentLocation?.GetLocationContext();
        if (context?.DefaultMusic != null)
        {
          if (context.DefaultMusicCondition != null && !GameStateQuery.CheckConditions(context.DefaultMusicCondition))
            return;
          if (ignoreDelay)
            PlayLocationSong();
          else
            Game1.morningSongPlayAction = DelayedAction.functionAfterDelay(new Action(PlayLocationSong), 500);
        }
        else if (ignoreDelay)
          PlayDefault();
        else
          Game1.morningSongPlayAction = DelayedAction.functionAfterDelay(new Action(PlayDefault), 500);

        void PlayLocationSong()
        {
          if (Game1.currentLocation == null)
          {
            Game1.changeMusicTrack("none", true);
          }
          else
          {
            Game1.changeMusicTrack(context.DefaultMusic, true);
            Game1.IsPlayingBackgroundMusic = true;
          }
        }
      }
    }
    else
    {
      if (!(Game1.getMusicTrackName() == "silence"))
        return;
      Game1.changeMusicTrack("none", true);
    }

    static void PlayRain() => Game1.changeMusicTrack("rain", true);

    static void PlayDefault()
    {
      Game1.changeMusicTrack(Game1.currentLocation.GetMorningSong(), true);
      Game1.IsPlayingBackgroundMusic = true;
      Game1.IsPlayingMorningSong = true;
    }
  }

  public static void doMorningStuff()
  {
    Game1.playMorningSong();
    DelayedAction.functionAfterDelay((Action) (() =>
    {
      while (Game1.morningQueue.Count > 0)
        Game1.morningQueue.Dequeue()();
    }), 1000);
    if (!Game1.player.hasPendingCompletedQuests)
      return;
    Game1.dayTimeMoneyBox.PingQuestLog();
  }

  /// <summary>Add an action that will be called one second after fully waking up in the morning. This won't be saved, so it should only be used for "fluff" functions like sending multiplayer chat messages, etc.</summary>
  /// <param name="action">The action to perform.</param>
  public static void addMorningFluffFunction(Action action) => Game1.morningQueue.Enqueue(action);

  private Point getViewportCenter()
  {
    if ((double) Game1.viewportTarget.X != (double) int.MinValue)
    {
      if ((double) Math.Abs((float) Game1.viewportCenter.X - Game1.viewportTarget.X) > (double) Game1.viewportSpeed || (double) Math.Abs((float) Game1.viewportCenter.Y - Game1.viewportTarget.Y) > (double) Game1.viewportSpeed)
      {
        Vector2 velocityTowardPoint = Utility.getVelocityTowardPoint(Game1.viewportCenter, Game1.viewportTarget, Game1.viewportSpeed);
        Game1.viewportCenter.X += (int) Math.Round((double) velocityTowardPoint.X);
        Game1.viewportCenter.Y += (int) Math.Round((double) velocityTowardPoint.Y);
      }
      else
      {
        if (Game1.viewportReachedTarget != null)
        {
          Game1.viewportReachedTarget();
          Game1.viewportReachedTarget = (Game1.afterFadeFunction) null;
        }
        Game1.viewportHold -= Game1.currentGameTime.ElapsedGameTime.Milliseconds;
        if (Game1.viewportHold <= 0)
        {
          Game1.viewportTarget = new Vector2((float) int.MinValue, (float) int.MinValue);
          Game1.afterFadeFunction afterViewport = Game1.afterViewport;
          if (afterViewport != null)
            afterViewport();
        }
      }
    }
    else
      Game1.viewportCenter = Game1.getPlayerOrEventFarmer().StandingPixel;
    return Game1.viewportCenter;
  }

  public static void afterFadeReturnViewportToPlayer()
  {
    Game1.viewportTarget = new Vector2((float) int.MinValue, (float) int.MinValue);
    Game1.viewportHold = 0;
    Game1.viewportFreeze = false;
    Game1.viewportCenter = Game1.player.StandingPixel;
    Game1.globalFadeToClear();
  }

  public static bool isViewportOnCustomPath()
  {
    return (double) Game1.viewportTarget.X != (double) int.MinValue;
  }

  public static void moveViewportTo(
    Vector2 target,
    float speed,
    int holdTimer = 0,
    Game1.afterFadeFunction reachedTarget = null,
    Game1.afterFadeFunction endFunction = null)
  {
    Game1.viewportTarget = target;
    Game1.viewportSpeed = speed;
    Game1.viewportHold = holdTimer;
    Game1.afterViewport = endFunction;
    Game1.viewportReachedTarget = reachedTarget;
  }

  public static Farm getFarm() => Game1.RequireLocation<Farm>("Farm");

  public static void setMousePosition(int x, int y, bool ui_scale)
  {
    if (ui_scale)
      Game1.setMousePositionRaw((int) ((double) x * (double) Game1.options.uiScale), (int) ((double) y * (double) Game1.options.uiScale));
    else
      Game1.setMousePositionRaw((int) ((double) x * (double) Game1.options.zoomLevel), (int) ((double) y * (double) Game1.options.zoomLevel));
  }

  public static void setMousePosition(int x, int y) => Game1.setMousePosition(x, y, Game1.uiMode);

  public static void setMousePosition(Point position, bool ui_scale)
  {
    Game1.setMousePosition(position.X, position.Y, ui_scale);
  }

  public static void setMousePosition(Point position)
  {
    Game1.setMousePosition(position, Game1.uiMode);
  }

  public static void setMousePositionRaw(int x, int y)
  {
    Game1.input.SetMousePosition(x, y);
    Game1.InvalidateOldMouseMovement();
    Game1.lastCursorMotionWasMouse = false;
  }

  public static Point getMousePositionRaw()
  {
    return new Point(Game1.getMouseXRaw(), Game1.getMouseYRaw());
  }

  public static Point getMousePosition(bool ui_scale)
  {
    return new Point(Game1.getMouseX(ui_scale), Game1.getMouseY(ui_scale));
  }

  public static Point getMousePosition() => Game1.getMousePosition(Game1.uiMode);

  private static float thumbstickToMouseModifier
  {
    get
    {
      if (Game1._cursorSpeedDirty)
        Game1.ComputeCursorSpeed();
      return (float) ((double) Game1._cursorSpeed / 720.0 * (double) Game1.viewport.Height * Game1.currentGameTime.ElapsedGameTime.TotalSeconds);
    }
  }

  private static void ComputeCursorSpeed()
  {
    Game1._cursorSpeedDirty = false;
    GamePadState gamePadState = Game1.input.GetGamePadState();
    float num1 = 0.9f;
    bool flag = false;
    Vector2 vector2 = gamePadState.ThumbSticks.Left;
    double num2 = (double) vector2.Length();
    vector2 = gamePadState.ThumbSticks.Right;
    float num3 = vector2.Length();
    double num4 = (double) num1;
    if (num2 > num4 || (double) num3 > (double) num1)
      flag = true;
    float min = 0.7f;
    float max = 2f;
    float num5 = 1f;
    if (Game1._cursorDragEnabled)
    {
      min = 0.5f;
      max = 2f;
      num5 = 1f;
    }
    if (!flag)
      num5 = -5f;
    if (Game1._cursorDragPrevEnabled != Game1._cursorDragEnabled)
      Game1._cursorSpeedScale *= 0.5f;
    Game1._cursorDragPrevEnabled = Game1._cursorDragEnabled;
    Game1._cursorSpeedScale += Game1._cursorUpdateElapsedSec * num5;
    Game1._cursorSpeedScale = MathHelper.Clamp(Game1._cursorSpeedScale, min, max);
    double num6 = 16.0 / Game1.game1.TargetElapsedTime.TotalSeconds * (double) Game1._cursorSpeedScale;
    float num7 = (float) num6 - Game1._cursorSpeed;
    Game1._cursorSpeed = (float) num6;
    Game1._cursorUpdateElapsedSec = 0.0f;
    if (!Game1.debugMode)
      return;
    Game1.log.Verbose($"_cursorSpeed={Game1._cursorSpeed.ToString("0.0")}, _cursorSpeedScale={Game1._cursorSpeedScale.ToString("0.0")}, deltaSpeed={num7.ToString("0.0")}");
  }

  private static void SetFreeCursorElapsed(float elapsedSec)
  {
    if ((double) elapsedSec == (double) Game1._cursorUpdateElapsedSec)
      return;
    Game1._cursorUpdateElapsedSec = elapsedSec;
    Game1._cursorSpeedDirty = true;
  }

  public static void ResetFreeCursorDrag()
  {
    if (Game1._cursorDragEnabled)
      Game1._cursorSpeedDirty = true;
    Game1._cursorDragEnabled = false;
  }

  public static void SetFreeCursorDrag()
  {
    if (!Game1._cursorDragEnabled)
      Game1._cursorSpeedDirty = true;
    Game1._cursorDragEnabled = true;
  }

  public static void updateActiveMenu(GameTime gameTime)
  {
    IClickableMenu iclickableMenu = Game1.activeClickableMenu;
    while (iclickableMenu.GetChildMenu() != null)
      iclickableMenu = iclickableMenu.GetChildMenu();
    if (!Program.gamePtr.IsActiveNoOverlay && Program.releaseBuild)
    {
      if (iclickableMenu == null || !iclickableMenu.IsActive())
        return;
      iclickableMenu.update(gameTime);
    }
    else
    {
      MouseState mouseState = Game1.input.GetMouseState();
      KeyboardState keyboardState = Game1.GetKeyboardState();
      GamePadState gamePadState = Game1.input.GetGamePadState();
      if (Game1.CurrentEvent != null)
      {
        if (mouseState.LeftButton == ButtonState.Pressed && Game1.oldMouseState.LeftButton == ButtonState.Released || Game1.options.gamepadControls && gamePadState.IsButtonDown(Buttons.A) && Game1.oldPadState.IsButtonUp(Buttons.A))
          Game1.CurrentEvent.receiveMouseClick(Game1.getMouseX(), Game1.getMouseY());
        else if (Game1.options.gamepadControls && gamePadState.IsButtonDown(Buttons.Back) && Game1.oldPadState.IsButtonUp(Buttons.Back) && !Game1.CurrentEvent.skipped && Game1.CurrentEvent.skippable)
        {
          Game1.CurrentEvent.skipped = true;
          Game1.CurrentEvent.skipEvent();
          Game1.freezeControls = false;
        }
        if (Game1.CurrentEvent != null && Game1.CurrentEvent.skipped)
        {
          Game1.oldMouseState = Game1.input.GetMouseState();
          Game1.oldKBState = keyboardState;
          Game1.oldPadState = gamePadState;
          return;
        }
      }
      if (Game1.options.gamepadControls && iclickableMenu != null && iclickableMenu.IsActive())
      {
        if (Game1.isGamePadThumbstickInMotion() && (!Game1.options.snappyMenus || iclickableMenu.overrideSnappyMenuCursorMovementBan()))
          Game1.setMousePositionRaw((int) ((double) mouseState.X + (double) gamePadState.ThumbSticks.Left.X * (double) Game1.thumbstickToMouseModifier), (int) ((double) mouseState.Y - (double) gamePadState.ThumbSticks.Left.Y * (double) Game1.thumbstickToMouseModifier));
        if (iclickableMenu != null && iclickableMenu.IsActive() && (Game1.chatBox == null || !Game1.chatBox.isActive()))
        {
          foreach (Buttons pressedButton in Utility.getPressedButtons(gamePadState, Game1.oldPadState))
          {
            iclickableMenu.receiveGamePadButton(pressedButton);
            if (iclickableMenu == null || !iclickableMenu.IsActive())
              break;
          }
          foreach (Buttons heldButton in Utility.getHeldButtons(gamePadState))
          {
            if (iclickableMenu != null && iclickableMenu.IsActive())
              iclickableMenu.gamePadButtonHeld(heldButton);
            if (iclickableMenu == null || !iclickableMenu.IsActive())
              break;
          }
        }
      }
      if ((Game1.getMouseX() != Game1.getOldMouseX() || Game1.getMouseY() != Game1.getOldMouseY()) && !Game1.isGamePadThumbstickInMotion() && !Game1.isDPadPressed())
        Game1.lastCursorMotionWasMouse = true;
      Game1.ResetFreeCursorDrag();
      if (iclickableMenu != null && iclickableMenu.IsActive())
        iclickableMenu.performHoverAction(Game1.getMouseX(), Game1.getMouseY());
      if (iclickableMenu != null && iclickableMenu.IsActive())
        iclickableMenu.update(gameTime);
      if (iclickableMenu != null && iclickableMenu.IsActive() && mouseState.LeftButton == ButtonState.Pressed && Game1.oldMouseState.LeftButton == ButtonState.Released)
      {
        if (Game1.chatBox != null && Game1.chatBox.isActive() && Game1.chatBox.isWithinBounds(Game1.getMouseX(), Game1.getMouseY()))
          Game1.chatBox.receiveLeftClick(Game1.getMouseX(), Game1.getMouseY(), true);
        else
          iclickableMenu.receiveLeftClick(Game1.getMouseX(), Game1.getMouseY());
      }
      else if (iclickableMenu != null && iclickableMenu.IsActive() && mouseState.RightButton == ButtonState.Pressed && (Game1.oldMouseState.RightButton == ButtonState.Released || (double) Game1.mouseClickPolling > 650.0 && !(iclickableMenu is DialogueBox)))
      {
        iclickableMenu.receiveRightClick(Game1.getMouseX(), Game1.getMouseY());
        if ((double) Game1.mouseClickPolling > 650.0)
          Game1.mouseClickPolling = 600;
        if ((iclickableMenu == null || !iclickableMenu.IsActive()) && Game1.activeClickableMenu == null)
        {
          Game1.rightClickPolling = 500;
          Game1.mouseClickPolling = 0;
        }
      }
      if (mouseState.ScrollWheelValue != Game1.oldMouseState.ScrollWheelValue && iclickableMenu != null && iclickableMenu.IsActive())
      {
        if (Game1.chatBox != null && Game1.chatBox.choosingEmoji && Game1.chatBox.emojiMenu.isWithinBounds(Game1.getOldMouseX(), Game1.getOldMouseY()))
          Game1.chatBox.receiveScrollWheelAction(mouseState.ScrollWheelValue - Game1.oldMouseState.ScrollWheelValue);
        else
          iclickableMenu.receiveScrollWheelAction(mouseState.ScrollWheelValue - Game1.oldMouseState.ScrollWheelValue);
      }
      if (Game1.options.gamepadControls && iclickableMenu != null && iclickableMenu.IsActive())
      {
        Game1.thumbstickPollingTimer -= Game1.currentGameTime.ElapsedGameTime.Milliseconds;
        if (Game1.thumbstickPollingTimer <= 0)
        {
          if ((double) gamePadState.ThumbSticks.Right.Y > 0.20000000298023224)
            iclickableMenu.receiveScrollWheelAction(1);
          else if ((double) gamePadState.ThumbSticks.Right.Y < -0.20000000298023224)
            iclickableMenu.receiveScrollWheelAction(-1);
        }
        if (Game1.thumbstickPollingTimer <= 0)
          Game1.thumbstickPollingTimer = 220 - (int) ((double) Math.Abs(gamePadState.ThumbSticks.Right.Y) * 170.0);
        if ((double) Math.Abs(gamePadState.ThumbSticks.Right.Y) < 0.20000000298023224)
          Game1.thumbstickPollingTimer = 0;
      }
      if (iclickableMenu != null && iclickableMenu.IsActive() && mouseState.LeftButton == ButtonState.Released && Game1.oldMouseState.LeftButton == ButtonState.Pressed)
        iclickableMenu.releaseLeftClick(Game1.getMouseX(), Game1.getMouseY());
      else if (iclickableMenu != null && iclickableMenu.IsActive() && mouseState.LeftButton == ButtonState.Pressed && Game1.oldMouseState.LeftButton == ButtonState.Pressed)
        iclickableMenu.leftClickHeld(Game1.getMouseX(), Game1.getMouseY());
      foreach (Keys pressedKey in keyboardState.GetPressedKeys())
      {
        if (iclickableMenu != null && iclickableMenu.IsActive() && !((IEnumerable<Keys>) Game1.oldKBState.GetPressedKeys()).Contains<Keys>(pressedKey))
          iclickableMenu.receiveKeyPress(pressedKey);
      }
      TimeSpan elapsedGameTime;
      if (Game1.chatBox == null || !Game1.chatBox.isActive())
      {
        if (Game1.isOneOfTheseKeysDown(Game1.oldKBState, Game1.options.moveUpButton) || Game1.options.snappyMenus && Game1.options.gamepadControls && ((double) Math.Abs(gamePadState.ThumbSticks.Left.X) < (double) gamePadState.ThumbSticks.Left.Y || gamePadState.IsButtonDown(Buttons.DPadUp)))
        {
          ref int local = ref Game1.directionKeyPolling[0];
          int num = local;
          elapsedGameTime = Game1.currentGameTime.ElapsedGameTime;
          int milliseconds = elapsedGameTime.Milliseconds;
          local = num - milliseconds;
        }
        else if (Game1.isOneOfTheseKeysDown(Game1.oldKBState, Game1.options.moveRightButton) || Game1.options.snappyMenus && Game1.options.gamepadControls && ((double) gamePadState.ThumbSticks.Left.X > (double) Math.Abs(gamePadState.ThumbSticks.Left.Y) || gamePadState.IsButtonDown(Buttons.DPadRight)))
        {
          ref int local = ref Game1.directionKeyPolling[1];
          int num = local;
          elapsedGameTime = Game1.currentGameTime.ElapsedGameTime;
          int milliseconds = elapsedGameTime.Milliseconds;
          local = num - milliseconds;
        }
        else if (Game1.isOneOfTheseKeysDown(Game1.oldKBState, Game1.options.moveDownButton) || Game1.options.snappyMenus && Game1.options.gamepadControls && ((double) Math.Abs(gamePadState.ThumbSticks.Left.X) < (double) Math.Abs(gamePadState.ThumbSticks.Left.Y) || gamePadState.IsButtonDown(Buttons.DPadDown)))
        {
          ref int local = ref Game1.directionKeyPolling[2];
          int num = local;
          elapsedGameTime = Game1.currentGameTime.ElapsedGameTime;
          int milliseconds = elapsedGameTime.Milliseconds;
          local = num - milliseconds;
        }
        else if (Game1.isOneOfTheseKeysDown(Game1.oldKBState, Game1.options.moveLeftButton) || Game1.options.snappyMenus && Game1.options.gamepadControls && ((double) Math.Abs(gamePadState.ThumbSticks.Left.X) > (double) Math.Abs(gamePadState.ThumbSticks.Left.Y) || gamePadState.IsButtonDown(Buttons.DPadLeft)))
        {
          ref int local = ref Game1.directionKeyPolling[3];
          int num = local;
          elapsedGameTime = Game1.currentGameTime.ElapsedGameTime;
          int milliseconds = elapsedGameTime.Milliseconds;
          local = num - milliseconds;
        }
        if (Game1.areAllOfTheseKeysUp(Game1.oldKBState, Game1.options.moveUpButton) && (!Game1.options.snappyMenus || !Game1.options.gamepadControls || (double) gamePadState.ThumbSticks.Left.Y < 0.1 && gamePadState.IsButtonUp(Buttons.DPadUp)))
          Game1.directionKeyPolling[0] = 250;
        if (Game1.areAllOfTheseKeysUp(Game1.oldKBState, Game1.options.moveRightButton) && (!Game1.options.snappyMenus || !Game1.options.gamepadControls || (double) gamePadState.ThumbSticks.Left.X < 0.1 && gamePadState.IsButtonUp(Buttons.DPadRight)))
          Game1.directionKeyPolling[1] = 250;
        if (Game1.areAllOfTheseKeysUp(Game1.oldKBState, Game1.options.moveDownButton) && (!Game1.options.snappyMenus || !Game1.options.gamepadControls || (double) gamePadState.ThumbSticks.Left.Y > -0.1 && gamePadState.IsButtonUp(Buttons.DPadDown)))
          Game1.directionKeyPolling[2] = 250;
        if (Game1.areAllOfTheseKeysUp(Game1.oldKBState, Game1.options.moveLeftButton) && (!Game1.options.snappyMenus || !Game1.options.gamepadControls || (double) gamePadState.ThumbSticks.Left.X > -0.1 && gamePadState.IsButtonUp(Buttons.DPadLeft)))
          Game1.directionKeyPolling[3] = 250;
        if (Game1.directionKeyPolling[0] <= 0 && iclickableMenu != null && iclickableMenu.IsActive())
        {
          iclickableMenu.receiveKeyPress(Game1.options.getFirstKeyboardKeyFromInputButtonList(Game1.options.moveUpButton));
          Game1.directionKeyPolling[0] = 70;
        }
        if (Game1.directionKeyPolling[1] <= 0 && iclickableMenu != null && iclickableMenu.IsActive())
        {
          iclickableMenu.receiveKeyPress(Game1.options.getFirstKeyboardKeyFromInputButtonList(Game1.options.moveRightButton));
          Game1.directionKeyPolling[1] = 70;
        }
        if (Game1.directionKeyPolling[2] <= 0 && iclickableMenu != null && iclickableMenu.IsActive())
        {
          iclickableMenu.receiveKeyPress(Game1.options.getFirstKeyboardKeyFromInputButtonList(Game1.options.moveDownButton));
          Game1.directionKeyPolling[2] = 70;
        }
        if (Game1.directionKeyPolling[3] <= 0 && iclickableMenu != null && iclickableMenu.IsActive())
        {
          iclickableMenu.receiveKeyPress(Game1.options.getFirstKeyboardKeyFromInputButtonList(Game1.options.moveLeftButton));
          Game1.directionKeyPolling[3] = 70;
        }
        if (Game1.options.gamepadControls && iclickableMenu != null && iclickableMenu.IsActive())
        {
          if (!iclickableMenu.areGamePadControlsImplemented() && gamePadState.IsButtonDown(Buttons.A) && (!Game1.oldPadState.IsButtonDown(Buttons.A) || (double) Game1.gamePadAButtonPolling > 650.0 && !(iclickableMenu is DialogueBox)))
          {
            iclickableMenu.receiveLeftClick(Game1.getMousePosition().X, Game1.getMousePosition().Y);
            if ((double) Game1.gamePadAButtonPolling > 650.0)
              Game1.gamePadAButtonPolling = 600;
          }
          else if (!iclickableMenu.areGamePadControlsImplemented() && !gamePadState.IsButtonDown(Buttons.A) && Game1.oldPadState.IsButtonDown(Buttons.A))
            iclickableMenu.releaseLeftClick(Game1.getMousePosition().X, Game1.getMousePosition().Y);
          else if (!iclickableMenu.areGamePadControlsImplemented() && gamePadState.IsButtonDown(Buttons.X) && (!Game1.oldPadState.IsButtonDown(Buttons.X) || (double) Game1.gamePadXButtonPolling > 650.0 && !(iclickableMenu is DialogueBox)))
          {
            iclickableMenu.receiveRightClick(Game1.getMousePosition().X, Game1.getMousePosition().Y);
            if ((double) Game1.gamePadXButtonPolling > 650.0)
              Game1.gamePadXButtonPolling = 600;
          }
          foreach (Buttons pressedButton in Utility.getPressedButtons(gamePadState, Game1.oldPadState))
          {
            if (iclickableMenu != null && iclickableMenu.IsActive())
            {
              Keys key = Utility.mapGamePadButtonToKey(pressedButton);
              if (!(iclickableMenu is FarmhandMenu) || Game1.game1.IsMainInstance || !Game1.options.doesInputListContain(Game1.options.menuButton, key))
                iclickableMenu.receiveKeyPress(key);
            }
            else
              break;
          }
          if (iclickableMenu != null && iclickableMenu.IsActive() && !iclickableMenu.areGamePadControlsImplemented() && gamePadState.IsButtonDown(Buttons.A) && Game1.oldPadState.IsButtonDown(Buttons.A))
            iclickableMenu.leftClickHeld(Game1.getMousePosition().X, Game1.getMousePosition().Y);
          if (gamePadState.IsButtonDown(Buttons.X))
          {
            int padXbuttonPolling = Game1.gamePadXButtonPolling;
            elapsedGameTime = gameTime.ElapsedGameTime;
            int milliseconds = elapsedGameTime.Milliseconds;
            Game1.gamePadXButtonPolling = padXbuttonPolling + milliseconds;
          }
          else
            Game1.gamePadXButtonPolling = 0;
          if (gamePadState.IsButtonDown(Buttons.A))
          {
            int padAbuttonPolling = Game1.gamePadAButtonPolling;
            elapsedGameTime = gameTime.ElapsedGameTime;
            int milliseconds = elapsedGameTime.Milliseconds;
            Game1.gamePadAButtonPolling = padAbuttonPolling + milliseconds;
          }
          else
            Game1.gamePadAButtonPolling = 0;
          if (!iclickableMenu.IsActive() && Game1.activeClickableMenu == null)
          {
            Game1.rightClickPolling = 500;
            Game1.gamePadXButtonPolling = 0;
            Game1.gamePadAButtonPolling = 0;
          }
        }
      }
      if (mouseState.RightButton == ButtonState.Pressed)
      {
        int mouseClickPolling = Game1.mouseClickPolling;
        elapsedGameTime = gameTime.ElapsedGameTime;
        int milliseconds = elapsedGameTime.Milliseconds;
        Game1.mouseClickPolling = mouseClickPolling + milliseconds;
      }
      else
        Game1.mouseClickPolling = 0;
      Game1.oldMouseState = Game1.input.GetMouseState();
      Game1.oldKBState = keyboardState;
      Game1.oldPadState = gamePadState;
    }
  }

  public bool ShowLocalCoopJoinMenu()
  {
    if (!this.IsMainInstance || Game1.gameMode != (byte) 3)
      return false;
    int free_farmhands = 0;
    Utility.ForEachLocation((Func<GameLocation, bool>) (location =>
    {
      if (location is Cabin cabin2 && (!cabin2.HasOwner || !cabin2.IsOwnerActivated))
        ++free_farmhands;
      return true;
    }));
    if (free_farmhands == 0)
    {
      Game1.showRedMessage(Game1.content.LoadString("Strings\\UI:CoopMenu_NoSlots"));
      return false;
    }
    if (Game1.currentMinigame != null || Game1.activeClickableMenu != null || !this.IsLocalCoopJoinable())
      return false;
    Game1.playSound("bigSelect");
    Game1.activeClickableMenu = (IClickableMenu) new LocalCoopJoinMenu();
    return true;
  }

  public static void updateTextEntry(GameTime gameTime)
  {
    MouseState mouseState = Game1.input.GetMouseState();
    KeyboardState keyboardState = Game1.GetKeyboardState();
    GamePadState gamePadState = Game1.input.GetGamePadState();
    ButtonCollection buttonCollection;
    if (Game1.options.gamepadControls)
    {
      switch (Game1.textEntry)
      {
        case null:
        case null:
          break;
        default:
          buttonCollection = Utility.getPressedButtons(gamePadState, Game1.oldPadState);
          foreach (Buttons button in buttonCollection)
          {
            Game1.textEntry.receiveGamePadButton(button);
            if (Game1.textEntry == null)
              break;
          }
          buttonCollection = Utility.getHeldButtons(gamePadState);
          foreach (Buttons b in buttonCollection)
          {
            Game1.textEntry?.gamePadButtonHeld(b);
            if (Game1.textEntry == null)
              break;
          }
          break;
      }
    }
    Game1.textEntry?.performHoverAction(Game1.getMouseX(), Game1.getMouseY());
    Game1.textEntry?.update(gameTime);
    if (Game1.textEntry != null && mouseState.LeftButton == ButtonState.Pressed && Game1.oldMouseState.LeftButton == ButtonState.Released)
      Game1.textEntry.receiveLeftClick(Game1.getMouseX(), Game1.getMouseY(), true);
    else if (Game1.textEntry != null && mouseState.RightButton == ButtonState.Pressed && (Game1.oldMouseState.RightButton == ButtonState.Released || (double) Game1.mouseClickPolling > 650.0))
    {
      Game1.textEntry.receiveRightClick(Game1.getMouseX(), Game1.getMouseY());
      if ((double) Game1.mouseClickPolling > 650.0)
        Game1.mouseClickPolling = 600;
      if (Game1.textEntry == null)
      {
        Game1.rightClickPolling = 500;
        Game1.mouseClickPolling = 0;
      }
    }
    if (mouseState.ScrollWheelValue != Game1.oldMouseState.ScrollWheelValue && Game1.textEntry != null)
    {
      if (Game1.chatBox != null && Game1.chatBox.choosingEmoji && Game1.chatBox.emojiMenu.isWithinBounds(Game1.getOldMouseX(), Game1.getOldMouseY()))
        Game1.chatBox.receiveScrollWheelAction(mouseState.ScrollWheelValue - Game1.oldMouseState.ScrollWheelValue);
      else
        Game1.textEntry.receiveScrollWheelAction(mouseState.ScrollWheelValue - Game1.oldMouseState.ScrollWheelValue);
    }
    TimeSpan elapsedGameTime;
    GamePadThumbSticks thumbSticks;
    if (Game1.options.gamepadControls && Game1.textEntry != null)
    {
      int thumbstickPollingTimer = Game1.thumbstickPollingTimer;
      elapsedGameTime = Game1.currentGameTime.ElapsedGameTime;
      int milliseconds = elapsedGameTime.Milliseconds;
      Game1.thumbstickPollingTimer = thumbstickPollingTimer - milliseconds;
      if (Game1.thumbstickPollingTimer <= 0)
      {
        if ((double) gamePadState.ThumbSticks.Right.Y > 0.20000000298023224)
          Game1.textEntry.receiveScrollWheelAction(1);
        else if ((double) gamePadState.ThumbSticks.Right.Y < -0.20000000298023224)
          Game1.textEntry.receiveScrollWheelAction(-1);
      }
      if (Game1.thumbstickPollingTimer <= 0)
      {
        thumbSticks = gamePadState.ThumbSticks;
        Game1.thumbstickPollingTimer = 220 - (int) ((double) Math.Abs(thumbSticks.Right.Y) * 170.0);
      }
      thumbSticks = gamePadState.ThumbSticks;
      if ((double) Math.Abs(thumbSticks.Right.Y) < 0.20000000298023224)
        Game1.thumbstickPollingTimer = 0;
    }
    if (Game1.textEntry != null && mouseState.LeftButton == ButtonState.Released && Game1.oldMouseState.LeftButton == ButtonState.Pressed)
      Game1.textEntry.releaseLeftClick(Game1.getMouseX(), Game1.getMouseY());
    else if (Game1.textEntry != null && mouseState.LeftButton == ButtonState.Pressed && Game1.oldMouseState.LeftButton == ButtonState.Pressed)
      Game1.textEntry.leftClickHeld(Game1.getMouseX(), Game1.getMouseY());
    foreach (Keys pressedKey in keyboardState.GetPressedKeys())
    {
      if (Game1.textEntry != null && !((IEnumerable<Keys>) Game1.oldKBState.GetPressedKeys()).Contains<Keys>(pressedKey))
        Game1.textEntry.receiveKeyPress(pressedKey);
    }
    if (!Game1.isOneOfTheseKeysDown(Game1.oldKBState, Game1.options.moveUpButton))
    {
      if (Game1.options.snappyMenus && Game1.options.gamepadControls)
      {
        thumbSticks = gamePadState.ThumbSticks;
        double num = (double) Math.Abs(thumbSticks.Left.X);
        thumbSticks = gamePadState.ThumbSticks;
        double y = (double) thumbSticks.Left.Y;
        if (num < y || gamePadState.IsButtonDown(Buttons.DPadUp))
          goto label_47;
      }
      if (!Game1.isOneOfTheseKeysDown(Game1.oldKBState, Game1.options.moveRightButton))
      {
        if (Game1.options.snappyMenus && Game1.options.gamepadControls)
        {
          thumbSticks = gamePadState.ThumbSticks;
          double x = (double) thumbSticks.Left.X;
          thumbSticks = gamePadState.ThumbSticks;
          double num = (double) Math.Abs(thumbSticks.Left.Y);
          if (x > num || gamePadState.IsButtonDown(Buttons.DPadRight))
            goto label_51;
        }
        if (!Game1.isOneOfTheseKeysDown(Game1.oldKBState, Game1.options.moveDownButton))
        {
          if (Game1.options.snappyMenus && Game1.options.gamepadControls)
          {
            thumbSticks = gamePadState.ThumbSticks;
            double num1 = (double) Math.Abs(thumbSticks.Left.X);
            thumbSticks = gamePadState.ThumbSticks;
            double num2 = (double) Math.Abs(thumbSticks.Left.Y);
            if (num1 < num2 || gamePadState.IsButtonDown(Buttons.DPadDown))
              goto label_55;
          }
          if (!Game1.isOneOfTheseKeysDown(Game1.oldKBState, Game1.options.moveLeftButton))
          {
            if (Game1.options.snappyMenus && Game1.options.gamepadControls)
            {
              thumbSticks = gamePadState.ThumbSticks;
              double num3 = (double) Math.Abs(thumbSticks.Left.X);
              thumbSticks = gamePadState.ThumbSticks;
              double num4 = (double) Math.Abs(thumbSticks.Left.Y);
              if (num3 <= num4 && !gamePadState.IsButtonDown(Buttons.DPadLeft))
                goto label_60;
            }
            else
              goto label_60;
          }
          ref int local = ref Game1.directionKeyPolling[3];
          int num = local;
          elapsedGameTime = Game1.currentGameTime.ElapsedGameTime;
          int milliseconds = elapsedGameTime.Milliseconds;
          local = num - milliseconds;
          goto label_60;
        }
label_55:
        ref int local1 = ref Game1.directionKeyPolling[2];
        int num5 = local1;
        elapsedGameTime = Game1.currentGameTime.ElapsedGameTime;
        int milliseconds1 = elapsedGameTime.Milliseconds;
        local1 = num5 - milliseconds1;
        goto label_60;
      }
label_51:
      ref int local2 = ref Game1.directionKeyPolling[1];
      int num6 = local2;
      elapsedGameTime = Game1.currentGameTime.ElapsedGameTime;
      int milliseconds2 = elapsedGameTime.Milliseconds;
      local2 = num6 - milliseconds2;
      goto label_60;
    }
label_47:
    ref int local3 = ref Game1.directionKeyPolling[0];
    int num7 = local3;
    elapsedGameTime = Game1.currentGameTime.ElapsedGameTime;
    int milliseconds3 = elapsedGameTime.Milliseconds;
    local3 = num7 - milliseconds3;
label_60:
    if (Game1.areAllOfTheseKeysUp(Game1.oldKBState, Game1.options.moveUpButton))
    {
      if (Game1.options.snappyMenus && Game1.options.gamepadControls)
      {
        thumbSticks = gamePadState.ThumbSticks;
        if ((double) thumbSticks.Left.Y >= 0.1 || !gamePadState.IsButtonUp(Buttons.DPadUp))
          goto label_64;
      }
      Game1.directionKeyPolling[0] = 250;
    }
label_64:
    if (Game1.areAllOfTheseKeysUp(Game1.oldKBState, Game1.options.moveRightButton))
    {
      if (Game1.options.snappyMenus && Game1.options.gamepadControls)
      {
        thumbSticks = gamePadState.ThumbSticks;
        if ((double) thumbSticks.Left.X >= 0.1 || !gamePadState.IsButtonUp(Buttons.DPadRight))
          goto label_68;
      }
      Game1.directionKeyPolling[1] = 250;
    }
label_68:
    if (Game1.areAllOfTheseKeysUp(Game1.oldKBState, Game1.options.moveDownButton))
    {
      if (Game1.options.snappyMenus && Game1.options.gamepadControls)
      {
        thumbSticks = gamePadState.ThumbSticks;
        if ((double) thumbSticks.Left.Y <= -0.1 || !gamePadState.IsButtonUp(Buttons.DPadDown))
          goto label_72;
      }
      Game1.directionKeyPolling[2] = 250;
    }
label_72:
    if (Game1.areAllOfTheseKeysUp(Game1.oldKBState, Game1.options.moveLeftButton))
    {
      if (Game1.options.snappyMenus && Game1.options.gamepadControls)
      {
        thumbSticks = gamePadState.ThumbSticks;
        if ((double) thumbSticks.Left.X <= -0.1 || !gamePadState.IsButtonUp(Buttons.DPadLeft))
          goto label_76;
      }
      Game1.directionKeyPolling[3] = 250;
    }
label_76:
    if (Game1.directionKeyPolling[0] <= 0 && Game1.textEntry != null)
    {
      Game1.textEntry.receiveKeyPress(Game1.options.getFirstKeyboardKeyFromInputButtonList(Game1.options.moveUpButton));
      Game1.directionKeyPolling[0] = 70;
    }
    if (Game1.directionKeyPolling[1] <= 0 && Game1.textEntry != null)
    {
      Game1.textEntry.receiveKeyPress(Game1.options.getFirstKeyboardKeyFromInputButtonList(Game1.options.moveRightButton));
      Game1.directionKeyPolling[1] = 70;
    }
    if (Game1.directionKeyPolling[2] <= 0 && Game1.textEntry != null)
    {
      Game1.textEntry.receiveKeyPress(Game1.options.getFirstKeyboardKeyFromInputButtonList(Game1.options.moveDownButton));
      Game1.directionKeyPolling[2] = 70;
    }
    if (Game1.directionKeyPolling[3] <= 0 && Game1.textEntry != null)
    {
      Game1.textEntry.receiveKeyPress(Game1.options.getFirstKeyboardKeyFromInputButtonList(Game1.options.moveLeftButton));
      Game1.directionKeyPolling[3] = 70;
    }
    if (Game1.options.gamepadControls && Game1.textEntry != null)
    {
      if (!Game1.textEntry.areGamePadControlsImplemented() && gamePadState.IsButtonDown(Buttons.A) && (!Game1.oldPadState.IsButtonDown(Buttons.A) || (double) Game1.gamePadAButtonPolling > 650.0))
      {
        Game1.textEntry.receiveLeftClick(Game1.getMousePosition().X, Game1.getMousePosition().Y, true);
        if ((double) Game1.gamePadAButtonPolling > 650.0)
          Game1.gamePadAButtonPolling = 600;
      }
      else if (!Game1.textEntry.areGamePadControlsImplemented() && !gamePadState.IsButtonDown(Buttons.A) && Game1.oldPadState.IsButtonDown(Buttons.A))
        Game1.textEntry.releaseLeftClick(Game1.getMousePosition().X, Game1.getMousePosition().Y);
      else if (!Game1.textEntry.areGamePadControlsImplemented() && gamePadState.IsButtonDown(Buttons.X) && (!Game1.oldPadState.IsButtonDown(Buttons.X) || (double) Game1.gamePadXButtonPolling > 650.0))
      {
        Game1.textEntry.receiveRightClick(Game1.getMousePosition().X, Game1.getMousePosition().Y);
        if ((double) Game1.gamePadXButtonPolling > 650.0)
          Game1.gamePadXButtonPolling = 600;
      }
      buttonCollection = Utility.getPressedButtons(gamePadState, Game1.oldPadState);
      foreach (Buttons b in buttonCollection)
      {
        if (Game1.textEntry != null)
          Game1.textEntry.receiveKeyPress(Utility.mapGamePadButtonToKey(b));
        else
          break;
      }
      if (Game1.textEntry != null && !Game1.textEntry.areGamePadControlsImplemented() && gamePadState.IsButtonDown(Buttons.A) && Game1.oldPadState.IsButtonDown(Buttons.A))
        Game1.textEntry.leftClickHeld(Game1.getMousePosition().X, Game1.getMousePosition().Y);
      if (gamePadState.IsButtonDown(Buttons.X))
      {
        int padXbuttonPolling = Game1.gamePadXButtonPolling;
        elapsedGameTime = gameTime.ElapsedGameTime;
        int milliseconds4 = elapsedGameTime.Milliseconds;
        Game1.gamePadXButtonPolling = padXbuttonPolling + milliseconds4;
      }
      else
        Game1.gamePadXButtonPolling = 0;
      if (gamePadState.IsButtonDown(Buttons.A))
      {
        int padAbuttonPolling = Game1.gamePadAButtonPolling;
        elapsedGameTime = gameTime.ElapsedGameTime;
        int milliseconds5 = elapsedGameTime.Milliseconds;
        Game1.gamePadAButtonPolling = padAbuttonPolling + milliseconds5;
      }
      else
        Game1.gamePadAButtonPolling = 0;
      if (Game1.textEntry == null)
      {
        Game1.rightClickPolling = 500;
        Game1.gamePadAButtonPolling = 0;
        Game1.gamePadXButtonPolling = 0;
      }
    }
    if (mouseState.RightButton == ButtonState.Pressed)
    {
      int mouseClickPolling = Game1.mouseClickPolling;
      elapsedGameTime = gameTime.ElapsedGameTime;
      int milliseconds6 = elapsedGameTime.Milliseconds;
      Game1.mouseClickPolling = mouseClickPolling + milliseconds6;
    }
    else
      Game1.mouseClickPolling = 0;
    Game1.oldMouseState = Game1.input.GetMouseState();
    Game1.oldKBState = keyboardState;
    Game1.oldPadState = gamePadState;
  }

  public static string DateCompiled()
  {
    Version version = Assembly.GetExecutingAssembly().GetName().Version;
    return $"{version.Major.ToString()}.{version.Minor.ToString()}.{version.Build.ToString()}.{version.Revision.ToString()}";
  }

  public static void updatePause(GameTime gameTime)
  {
    if (Game1.IsDedicatedHost)
      Game1.pauseTime = 0.0f;
    Game1.pauseTime -= (float) gameTime.ElapsedGameTime.Milliseconds;
    if (Game1.player.isCrafting && Game1.random.NextDouble() < 0.007)
      Game1.playSound("crafting");
    if ((double) Game1.pauseTime > 0.0)
      return;
    if (Game1.currentObjectDialogue.Count == 0)
      Game1.messagePause = false;
    Game1.pauseTime = 0.0f;
    if (!string.IsNullOrEmpty(Game1.messageAfterPause))
    {
      Game1.player.isCrafting = false;
      Game1.drawObjectDialogue(Game1.messageAfterPause);
      Game1.messageAfterPause = "";
      if (Game1.killScreen)
      {
        Game1.killScreen = false;
        Game1.player.health = 10;
      }
    }
    else if (Game1.killScreen)
    {
      Game1.multiplayer.globalChatInfoMessage("PlayerDeath", Game1.player.Name);
      Game1.screenGlow = false;
      bool flag = false;
      if (Game1.currentLocation.GetLocationContext().ReviveLocations != null)
      {
        foreach (ReviveLocation reviveLocation in Game1.currentLocation.GetLocationContext().ReviveLocations)
        {
          if (GameStateQuery.CheckConditions(reviveLocation.Condition, player: Game1.player))
          {
            Game1.warpFarmer(reviveLocation.Location, reviveLocation.Position.X, reviveLocation.Position.Y, false);
            flag = true;
            break;
          }
        }
      }
      else
      {
        foreach (ReviveLocation reviveLocation in StardewValley.LocationContexts.Default.ReviveLocations)
        {
          if (GameStateQuery.CheckConditions(reviveLocation.Condition, player: Game1.player))
          {
            Game1.warpFarmer(reviveLocation.Location, reviveLocation.Position.X, reviveLocation.Position.Y, false);
            flag = true;
            break;
          }
        }
      }
      if (!flag)
        Game1.warpFarmer("Hospital", 20, 12, false);
    }
    if (Game1.currentLocation.currentEvent == null)
      return;
    ++Game1.currentLocation.currentEvent.CurrentCommand;
  }

  public static void CheckValidFullscreenResolution(ref int width, ref int height)
  {
    int num1 = width;
    int num2 = height;
    foreach (Microsoft.Xna.Framework.Graphics.DisplayMode supportedDisplayMode in Game1.graphics.GraphicsDevice.Adapter.SupportedDisplayModes)
    {
      if (supportedDisplayMode.Width >= 1280 /*0x0500*/ && supportedDisplayMode.Width == num1 && supportedDisplayMode.Height == num2)
      {
        width = num1;
        height = num2;
        return;
      }
    }
    foreach (Microsoft.Xna.Framework.Graphics.DisplayMode supportedDisplayMode in Game1.graphics.GraphicsDevice.Adapter.SupportedDisplayModes)
    {
      if (supportedDisplayMode.Width >= 1280 /*0x0500*/ && supportedDisplayMode.Width == Game1.graphics.GraphicsDevice.Adapter.CurrentDisplayMode.Width && supportedDisplayMode.Height == Game1.graphics.GraphicsDevice.Adapter.CurrentDisplayMode.Height)
      {
        width = Game1.graphics.GraphicsDevice.Adapter.CurrentDisplayMode.Width;
        height = Game1.graphics.GraphicsDevice.Adapter.CurrentDisplayMode.Height;
        return;
      }
    }
    bool flag = false;
    foreach (Microsoft.Xna.Framework.Graphics.DisplayMode supportedDisplayMode in Game1.graphics.GraphicsDevice.Adapter.SupportedDisplayModes)
    {
      if (supportedDisplayMode.Width >= 1280 /*0x0500*/ && num1 > supportedDisplayMode.Width)
      {
        width = supportedDisplayMode.Width;
        height = supportedDisplayMode.Height;
        flag = true;
      }
    }
    if (flag)
      return;
    Game1.log.Warn("Requested fullscreen resolution not valid, switching to windowed.");
    width = 1280 /*0x0500*/;
    height = 720;
    Game1.options.fullscreen = false;
  }

  public static void toggleNonBorderlessWindowedFullscreen()
  {
    int width = Game1.options.preferredResolutionX;
    int height = Game1.options.preferredResolutionY;
    Game1.graphics.HardwareModeSwitch = Game1.options.fullscreen && !Game1.options.windowedBorderlessFullscreen;
    if (Game1.options.fullscreen && !Game1.options.windowedBorderlessFullscreen)
      Game1.CheckValidFullscreenResolution(ref width, ref height);
    if (!Game1.options.fullscreen && !Game1.options.windowedBorderlessFullscreen)
    {
      width = 1280 /*0x0500*/;
      height = 720;
    }
    Game1.graphics.PreferredBackBufferWidth = width;
    Game1.graphics.PreferredBackBufferHeight = height;
    if (Game1.options.fullscreen != Game1.graphics.IsFullScreen)
      Game1.graphics.ToggleFullScreen();
    Game1.graphics.ApplyChanges();
    Game1.updateViewportForScreenSizeChange(true, Game1.graphics.PreferredBackBufferWidth, Game1.graphics.PreferredBackBufferHeight);
    GameRunner.instance.OnWindowSizeChange((object) null, (EventArgs) null);
  }

  public static void toggleFullscreen()
  {
    if (Game1.options.windowedBorderlessFullscreen)
    {
      Game1.graphics.HardwareModeSwitch = false;
      Game1.graphics.IsFullScreen = true;
      Game1.graphics.ApplyChanges();
      Game1.graphics.PreferredBackBufferWidth = Program.gamePtr.Window.ClientBounds.Width;
      Game1.graphics.PreferredBackBufferHeight = Program.gamePtr.Window.ClientBounds.Height;
    }
    else
      Game1.toggleNonBorderlessWindowedFullscreen();
    GameRunner.instance.OnWindowSizeChange((object) null, (EventArgs) null);
  }

  public static bool isFullscreen => Game1.graphics.IsFullScreen;

  private void checkForEscapeKeys()
  {
    KeyboardState keyboardState = Game1.input.GetKeyboardState();
    if (!this.IsMainInstance)
      return;
    if (keyboardState.IsKeyDown(Keys.LeftAlt) && keyboardState.IsKeyDown(Keys.Enter) && (Game1.oldKBState.IsKeyUp(Keys.LeftAlt) || Game1.oldKBState.IsKeyUp(Keys.Enter)))
    {
      if (Game1.options.isCurrentlyFullscreen() || Game1.options.isCurrentlyWindowedBorderless())
        Game1.options.setWindowedOption(1);
      else
        Game1.options.setWindowedOption(0);
    }
    if (!Game1.player.UsingTool && !Game1.freezeControls || !keyboardState.IsKeyDown(Keys.RightShift) || !keyboardState.IsKeyDown(Keys.R) || !keyboardState.IsKeyDown(Keys.Delete))
      return;
    Game1.freezeControls = false;
    Game1.player.forceCanMove();
    Game1.player.completelyStopAnimatingOrDoingAction();
    Game1.player.UsingTool = false;
  }

  public static bool IsPressEvent(ref KeyboardState state, Keys key)
  {
    if (!state.IsKeyDown(key) || Game1.oldKBState.IsKeyDown(key))
      return false;
    Game1.oldKBState = state;
    return true;
  }

  public static bool IsPressEvent(ref GamePadState state, Buttons btn)
  {
    if (!state.IsConnected || !state.IsButtonDown(btn) || Game1.oldPadState.IsButtonDown(btn))
      return false;
    Game1.oldPadState = state;
    return true;
  }

  public static bool isOneOfTheseKeysDown(KeyboardState state, InputButton[] keys)
  {
    foreach (InputButton key in keys)
    {
      if (key.key != Keys.None && state.IsKeyDown(key.key))
        return true;
    }
    return false;
  }

  public static bool areAllOfTheseKeysUp(KeyboardState state, InputButton[] keys)
  {
    foreach (InputButton key in keys)
    {
      if (key.key != Keys.None && !state.IsKeyUp(key.key))
        return false;
    }
    return true;
  }

  internal void UpdateTitleScreen(GameTime time)
  {
    if (Game1.quit)
    {
      this.Exit();
      Game1.changeMusicTrack("none");
    }
    switch (Game1.gameMode)
    {
      case 6:
        this.UpdateTitleScreenDuringLoadingMode();
        break;
      case 7:
        Game1.currentLoader.MoveNext();
        break;
      case 8:
        Game1.pauseAccumulator -= (float) time.ElapsedGameTime.Milliseconds;
        if ((double) Game1.pauseAccumulator > 0.0)
          break;
        Game1.pauseAccumulator = 0.0f;
        Game1.setGameMode((byte) 3);
        if (Game1.currentObjectDialogue.Count <= 0)
          break;
        Game1.messagePause = true;
        Game1.pauseTime = 1E+10f;
        Game1.fadeToBlackAlpha = 1f;
        Game1.player.CanMove = false;
        break;
      default:
        if (Game1.game1.instanceIndex > 0)
        {
          if (Game1.activeClickableMenu != null || Game1.ticks <= 1)
            break;
          Game1.activeClickableMenu = (IClickableMenu) new FarmhandMenu(Game1.multiplayer.InitClient((Client) new LidgrenClient("localhost")));
          Game1.activeClickableMenu.populateClickableComponentList();
          if (!Game1.options.SnappyMenus)
            break;
          Game1.activeClickableMenu.snapToDefaultClickableComponent();
          break;
        }
        if ((double) Game1.fadeToBlackAlpha < 1.0 && Game1.fadeIn)
          Game1.fadeToBlackAlpha += 0.02f;
        else if ((double) Game1.fadeToBlackAlpha > 0.0 && Game1.fadeToBlack)
          Game1.fadeToBlackAlpha -= 0.02f;
        if ((double) Game1.pauseTime > 0.0)
          Game1.pauseTime = Math.Max(0.0f, Game1.pauseTime - (float) time.ElapsedGameTime.Milliseconds);
        if ((double) Game1.fadeToBlackAlpha >= 1.0)
        {
          switch (Game1.gameMode)
          {
            case 0:
              if (Game1.currentSong == null && (double) Game1.pauseTime <= 0.0 && this.IsMainInstance)
              {
                ICue cue;
                Game1.playSound("spring_day_ambient", out cue);
                Game1.currentSong = cue;
              }
              if (Game1.activeClickableMenu != null || Game1.quit)
                return;
              Game1.activeClickableMenu = (IClickableMenu) new TitleMenu();
              return;
            case 4:
              if (Game1.fadeToBlack)
                return;
              Game1.fadeIn = false;
              Game1.fadeToBlack = true;
              Game1.fadeToBlackAlpha = 2.5f;
              return;
            default:
              return;
          }
        }
        else
        {
          if ((double) Game1.fadeToBlackAlpha > 0.0)
            break;
          switch (Game1.gameMode)
          {
            case 0:
              if (!Game1.fadeToBlack)
                return;
              Game1.currentLoader = Utility.generateNewFarm(Game1.IsClient);
              Game1.setGameMode((byte) 6);
              Game1.loadingMessage = Game1.IsClient ? Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.2574", (object) Game1.client.serverName) : Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.2575");
              Game1.exitActiveMenu();
              return;
            case 4:
              if (!Game1.fadeToBlack)
                return;
              Game1.fadeIn = true;
              Game1.fadeToBlack = false;
              Game1.setGameMode((byte) 0);
              Game1.pauseTime = 2000f;
              return;
            default:
              return;
          }
        }
    }
  }

  /// <summary>Update while the <see cref="P:StardewValley.Game1.gameMode" /> is <see cref="F:StardewValley.Game1.loadingMode" /> and the <see cref="F:StardewValley.Game1.currentLoader" /> is running.</summary>
  /// <remarks>This is a low-level method; most code should call <see cref="M:StardewValley.Game1.UpdateTitleScreen(Microsoft.Xna.Framework.GameTime)" /> instead.</remarks>
  internal void UpdateTitleScreenDuringLoadingMode()
  {
    if (Game1._requestedMusicTracks.Count > 0)
      Game1._requestedMusicTracks = new Dictionary<MusicContext, KeyValuePair<string, bool>>();
    Game1.requestedMusicTrack = "none";
    Game1.requestedMusicTrackOverrideable = false;
    Game1.requestedMusicDirty = true;
    if (Game1.currentLoader == null || Game1.currentLoader.MoveNext())
      return;
    Game1.currentLoader = (IEnumerator<int>) null;
    if (Game1.gameMode == (byte) 3)
    {
      Game1.setGameMode((byte) 3);
      Game1.fadeIn = true;
      Game1.fadeToBlackAlpha = 0.99f;
    }
    else
      Game1.ExitToTitle();
  }

  /// <summary>Get whether the given NPC is currently constructing a building anywhere in the world.</summary>
  /// <param name="builder">The NPC constructing the building, usually <see cref="F:StardewValley.Game1.builder_robin" /> or <see cref="F:StardewValley.Game1.builder_wizard" />.</param>
  public static bool IsThereABuildingUnderConstruction(string builder = "Robin")
  {
    return Game1.netWorldState.Value.GetBuilderData(builder) != null;
  }

  /// <summary>Get the building currently being constructed by a given builder.</summary>
  /// <param name="builder">The NPC constructing the building, usually <see cref="F:StardewValley.Game1.builder_robin" /> or <see cref="F:StardewValley.Game1.builder_wizard" />.</param>
  public static Building GetBuildingUnderConstruction(string builder = "Robin")
  {
    BuilderData builderData = Game1.netWorldState.Value.GetBuilderData(builder);
    if (builderData == null)
      return (Building) null;
    GameLocation locationFromName = Game1.getLocationFromName(builderData.buildingLocation.Value);
    if (locationFromName == null)
      return (Building) null;
    return Game1.client != null && !Game1.multiplayer.isActiveLocation(locationFromName) ? (Building) null : locationFromName.getBuildingAt(Utility.PointToVector2(builderData.buildingTile.Value));
  }

  /// <summary>Get whether a building type was constructed anywhere in the world.</summary>
  /// <param name="name">The building type's ID in <c>Data/Buildings</c>.</param>
  public static bool IsBuildingConstructed(string name)
  {
    return Game1.GetNumberBuildingsConstructed(name) > 0;
  }

  /// <summary>Get the number of buildings of all types constructed anywhere in the world.</summary>
  /// <param name="includeUnderConstruction">Whether to count buildings that haven't finished construction yet.</param>
  public static int GetNumberBuildingsConstructed(bool includeUnderConstruction = false)
  {
    int buildingsConstructed1 = 0;
    foreach (string locationsWithBuilding in (IEnumerable<string>) Game1.netWorldState.Value.LocationsWithBuildings)
    {
      int num = buildingsConstructed1;
      GameLocation locationFromName = Game1.getLocationFromName(locationsWithBuilding);
      int buildingsConstructed2 = locationFromName != null ? locationFromName.getNumberBuildingsConstructed(includeUnderConstruction) : 0;
      buildingsConstructed1 = num + buildingsConstructed2;
    }
    return buildingsConstructed1;
  }

  /// <summary>Get the number of buildings of a given type constructed anywhere in the world.</summary>
  /// <param name="name">The building type's ID in <c>Data/Buildings</c>.</param>
  /// <param name="includeUnderConstruction">Whether to count buildings that haven't finished construction yet.</param>
  public static int GetNumberBuildingsConstructed(string name, bool includeUnderConstruction = false)
  {
    int buildingsConstructed1 = 0;
    foreach (string locationsWithBuilding in (IEnumerable<string>) Game1.netWorldState.Value.LocationsWithBuildings)
    {
      int num = buildingsConstructed1;
      GameLocation locationFromName = Game1.getLocationFromName(locationsWithBuilding);
      int buildingsConstructed2 = locationFromName != null ? locationFromName.getNumberBuildingsConstructed(name, includeUnderConstruction) : 0;
      buildingsConstructed1 = num + buildingsConstructed2;
    }
    return buildingsConstructed1;
  }

  private void UpdateLocations(GameTime time)
  {
    Game1.loopingLocationCues.Update(Game1.currentLocation);
    if (Game1.IsClient)
    {
      Game1.currentLocation.UpdateWhenCurrentLocation(time);
      foreach (GameLocation activeLocation in Game1.multiplayer.activeLocations())
        activeLocation.updateEvenIfFarmerIsntHere(time);
    }
    else
    {
      Utility.ForEachLocation((Func<GameLocation, bool>) (location =>
      {
        this._UpdateLocation(location, time);
        return true;
      }));
      if (Game1.currentLocation.IsTemporary)
        this._UpdateLocation(Game1.currentLocation, time);
      MineShaft.UpdateMines(time);
      VolcanoDungeon.UpdateLevels(time);
    }
  }

  protected void _UpdateLocation(GameLocation location, GameTime time)
  {
    bool flag = location.farmers.Any();
    if (!flag && location.CanBeRemotedlyViewed())
    {
      if (Game1.player.currentLocation == location)
      {
        flag = true;
      }
      else
      {
        foreach (Farmer farmer in (IEnumerable<Farmer>) Game1.otherFarmers.Values)
        {
          if (farmer.viewingLocation.Value != null && farmer.viewingLocation.Value.Equals(location.NameOrUniqueName))
          {
            flag = true;
            break;
          }
        }
      }
    }
    if (flag)
      location.UpdateWhenCurrentLocation(time);
    location.updateEvenIfFarmerIsntHere(time);
    if (location.wasInhabited == flag)
      return;
    location.wasInhabited = flag;
    if (!Game1.IsMasterGame)
      return;
    location.cleanupForVacancy();
  }

  public static void performTenMinuteClockUpdate()
  {
    Game1.hooks.OnGame1_PerformTenMinuteClockUpdate((Action) (() =>
    {
      int num = Game1.getTrulyDarkTime(Game1.currentLocation) - 100;
      Game1.gameTimeInterval = 0;
      if (Game1.IsMasterGame)
        Game1.timeOfDay += 10;
      if (Game1.timeOfDay % 100 >= 60)
        Game1.timeOfDay = Game1.timeOfDay - Game1.timeOfDay % 100 + 100;
      Game1.timeOfDay = Math.Min(Game1.timeOfDay, 2600);
      if (Game1.isLightning && Game1.timeOfDay < 2400 && Game1.IsMasterGame)
        Utility.performLightningUpdate(Game1.timeOfDay);
      if (Game1.timeOfDay == num)
        Game1.currentLocation.switchOutNightTiles();
      else if (Game1.timeOfDay == Game1.getModeratelyDarkTime(Game1.currentLocation) && Game1.currentLocation.IsOutdoors && !Game1.currentLocation.IsRainingHere())
        Game1.ambientLight = Color.White;
      if (!Game1.eventUp && Game1.isDarkOut(Game1.currentLocation) && Game1.IsPlayingBackgroundMusic)
        Game1.changeMusicTrack("none", true);
      if (Game1.weatherIcon == 1)
      {
        Dictionary<string, string> dictionary = Game1.temporaryContent.Load<Dictionary<string, string>>($"Data\\Festivals\\{Game1.currentSeason}{Game1.dayOfMonth.ToString()}");
        string[] strArray = dictionary["conditions"].Split('/');
        int int32 = Convert.ToInt32(ArgUtility.SplitBySpaceAndGet(strArray[1], 0));
        if (Game1.whereIsTodaysFest == null)
          Game1.whereIsTodaysFest = strArray[0];
        if (Game1.timeOfDay == int32)
        {
          string text;
          if (dictionary.TryGetValue("startedMessage", out text))
          {
            Game1.showGlobalMessage(TokenParser.ParseText(text));
          }
          else
          {
            string str;
            if (!dictionary.TryGetValue("locationDisplayName", out str))
            {
              string name = strArray[0];
              switch (name)
              {
                case "Forest":
                  str = Game1.IsWinter ? Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.2634") : Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.2635");
                  break;
                case "Town":
                  str = Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.2637");
                  break;
                case "Beach":
                  str = Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.2639");
                  break;
                default:
                  str = TokenParser.ParseText(GameLocation.GetData(name)?.DisplayName) ?? name;
                  break;
              }
            }
            Game1.showGlobalMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.2640", (object) dictionary["name"]) + str);
          }
        }
      }
      Game1.player.performTenMinuteUpdate();
      switch (Game1.timeOfDay)
      {
        case 1200:
          if (Game1.currentLocation.isOutdoors.Value && !Game1.currentLocation.IsRainingHere() && (Game1.IsPlayingOutdoorsAmbience || Game1.currentSong == null || Game1.isMusicContextActiveButNotPlaying()))
          {
            Game1.playMorningSong();
            break;
          }
          break;
        case 2000:
          if (Game1.IsPlayingTownMusic)
          {
            Game1.changeMusicTrack("none", true);
            break;
          }
          break;
        case 2400:
          Game1.dayTimeMoneyBox.timeShakeTimer = 2000;
          Game1.player.doEmote(24);
          Game1.showGlobalMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.2652"));
          break;
        case 2500:
          Game1.dayTimeMoneyBox.timeShakeTimer = 2000;
          Game1.player.doEmote(24);
          break;
        case 2600:
          Game1.dayTimeMoneyBox.timeShakeTimer = 2000;
          Game1.player.mount?.dismount();
          if (Game1.player.IsSitting())
            Game1.player.StopSitting(false);
          if (Game1.player.UsingTool && (!(Game1.player.CurrentTool is FishingRod currentTool4) || !currentTool4.isReeling && !currentTool4.pullingOutOfWater))
          {
            if (Game1.player.UsingTool && Game1.player.CurrentTool != null && Game1.player.CurrentTool is FishingRod currentTool3 && currentTool3.fishCaught)
            {
              currentTool3.doneHoldingFish(Game1.player, true);
              break;
            }
            Game1.player.completelyStopAnimatingOrDoingAction();
            break;
          }
          break;
        case 2800:
          if (Game1.activeClickableMenu != null)
          {
            Game1.activeClickableMenu.emergencyShutDown();
            Game1.exitActiveMenu();
          }
          Game1.player.startToPassOut();
          Horse mount = Game1.player.mount;
          if (mount != null)
          {
            mount.dismount();
            break;
          }
          break;
      }
      foreach (string activePassiveFestival in (IEnumerable<string>) Game1.netWorldState.Value.ActivePassiveFestivals)
      {
        PassiveFestivalData data;
        if (Utility.TryGetPassiveFestivalData(activePassiveFestival, out data) && Game1.timeOfDay == data.StartTime && (!data.OnlyShowMessageOnFirstDay || Utility.GetDayOfPassiveFestival(activePassiveFestival) == 1))
          Game1.showGlobalMessage(TokenParser.ParseText(data.StartMessage));
      }
      foreach (GameLocation location in (IEnumerable<GameLocation>) Game1.locations)
      {
        GameLocation gameLocation = location;
        if (gameLocation.NameOrUniqueName == Game1.currentLocation.NameOrUniqueName)
          gameLocation = Game1.currentLocation;
        gameLocation.performTenMinuteUpdate(Game1.timeOfDay);
        gameLocation.timeUpdate(10);
      }
      MineShaft.UpdateMines10Minutes(Game1.timeOfDay);
      VolcanoDungeon.UpdateLevels10Minutes(Game1.timeOfDay);
      if (Game1.IsMasterGame && Game1.farmEvent == null)
        Game1.netWorldState.Value.UpdateFromGame1();
      Game1.currentLightSources.RemoveWhere<string, LightSource>((Func<KeyValuePair<string, LightSource>, bool>) (p => p.Value.color.A <= (byte) 0));
    }));
  }

  public static bool shouldPlayMorningSong(bool loading_game = false)
  {
    if (Game1.eventUp || (double) Game1.options.musicVolumeLevel <= 0.025 || Game1.timeOfDay >= 1200)
      return false;
    if (loading_game)
      return true;
    return Game1.currentSong != null && Game1.IsPlayingOutdoorsAmbience;
  }

  public static void UpdateGameClock(GameTime time)
  {
    if (Game1.shouldTimePass() && !Game1.IsClient)
      Game1.gameTimeInterval += time.ElapsedGameTime.Milliseconds;
    if (Game1.timeOfDay >= Game1.getTrulyDarkTime(Game1.currentLocation))
    {
      float num = Math.Min(0.93f, (float) (0.75 + ((double) ((int) ((double) (Game1.timeOfDay - Game1.timeOfDay % 100) + (double) (Game1.timeOfDay % 100 / 10) * 16.659999847412109) - Game1.getTrulyDarkTime(Game1.currentLocation)) + (double) Game1.gameTimeInterval / (double) Game1.realMilliSecondsPerGameTenMinutes * 16.600000381469727) * 0.00062499998603016138));
      Game1.outdoorLight = (Game1.IsRainingHere() ? Game1.ambientLight : Game1.eveningColor) * num;
    }
    else if (Game1.timeOfDay >= Game1.getStartingToGetDarkTime(Game1.currentLocation))
    {
      float num = Math.Min(0.93f, (float) (0.30000001192092896 + ((double) ((int) ((double) (Game1.timeOfDay - Game1.timeOfDay % 100) + (double) (Game1.timeOfDay % 100 / 10) * 16.659999847412109) - Game1.getStartingToGetDarkTime(Game1.currentLocation)) + (double) Game1.gameTimeInterval / (double) Game1.realMilliSecondsPerGameTenMinutes * 16.600000381469727) * 0.0022499999031424522));
      Game1.outdoorLight = (Game1.IsRainingHere() ? Game1.ambientLight : Game1.eveningColor) * num;
    }
    else
      Game1.outdoorLight = !Game1.IsRainingHere() ? Game1.ambientLight : Game1.ambientLight * 0.3f;
    int gameTimeInterval = Game1.gameTimeInterval;
    int perGameTenMinutes = Game1.realMilliSecondsPerGameTenMinutes;
    GameLocation currentLocation = Game1.currentLocation;
    int? nullable1 = currentLocation != null ? new int?(currentLocation.ExtraMillisecondsPerInGameMinute * 10) : new int?();
    int? nullable2 = nullable1.HasValue ? new int?(perGameTenMinutes + nullable1.GetValueOrDefault()) : new int?();
    int valueOrDefault = nullable2.GetValueOrDefault();
    if (!(gameTimeInterval > valueOrDefault & nullable2.HasValue))
      return;
    if (Game1.panMode)
      Game1.gameTimeInterval = 0;
    else
      Game1.performTenMinuteClockUpdate();
  }

  public static Event getAvailableWeddingEvent()
  {
    if (Game1.weddingsToday.Count <= 0)
      return (Event) null;
    long id = Game1.weddingsToday[0];
    Game1.weddingsToday.RemoveAt(0);
    Farmer player1 = Game1.GetPlayer(id);
    if (player1 == null)
      return (Event) null;
    if (player1.hasRoommate())
      return (Event) null;
    Event weddingEvent;
    if (player1.spouse != null)
    {
      weddingEvent = Utility.getWeddingEvent(player1);
    }
    else
    {
      long? spouse = player1.team.GetSpouse(player1.UniqueMultiplayerID);
      Farmer player2 = Game1.GetPlayer(spouse.Value);
      if (player2 == null)
        return (Event) null;
      if (!Game1.getOnlineFarmers().Contains(player1) || !Game1.getOnlineFarmers().Contains(player2))
        return (Event) null;
      Game1.player.team.GetFriendship(player1.UniqueMultiplayerID, spouse.Value).Status = FriendshipStatus.Married;
      Game1.player.team.GetFriendship(player1.UniqueMultiplayerID, spouse.Value).WeddingDate = new WorldDate(Game1.Date);
      weddingEvent = Utility.getWeddingEvent(player1);
    }
    return weddingEvent;
  }

  public static void exitActiveMenu() => Game1.activeClickableMenu = (IClickableMenu) null;

  /// <summary>Perform an action when <see cref="M:StardewValley.Farmer.IsBusyDoingSomething" /> becomes false for the current player (or do it immediately if it's already false).</summary>
  /// <param name="action">The action to perform.</param>
  public static void PerformActionWhenPlayerFree(Action action)
  {
    if (Game1.player.IsBusyDoingSomething())
      Game1.actionsWhenPlayerFree.Add(action);
    else
      action();
  }

  public static void fadeScreenToBlack() => Game1.screenFade.FadeScreenToBlack();

  public static void fadeClear() => Game1.screenFade.FadeClear();

  private bool onFadeToBlackComplete()
  {
    bool blackComplete = false;
    if (Game1.killScreen)
    {
      Game1.viewportFreeze = true;
      Game1.viewport.X = -10000;
    }
    if (Game1.exitToTitle)
    {
      Game1.setGameMode((byte) 4);
      Game1.fadeIn = false;
      Game1.fadeToBlack = true;
      Game1.fadeToBlackAlpha = 0.01f;
      Game1.exitToTitle = false;
      Game1.changeMusicTrack("none");
      Game1.debrisWeather.Clear();
      return true;
    }
    if (Game1.timeOfDayAfterFade != -1)
    {
      Game1.timeOfDay = Game1.timeOfDayAfterFade;
      Game1.timeOfDayAfterFade = -1;
    }
    if (!Game1.nonWarpFade && Game1.locationRequest != null)
    {
      if (Game1.IsMasterGame && Game1.locationRequest.Location == null)
      {
        Game1.log.Error($"Warp to {Game1.locationRequest.Name} failed: location wasn't found or couldn't be loaded.");
        Game1.locationRequest = (LocationRequest) null;
      }
      if (Game1.locationRequest != null)
      {
        GameLocation currentLocation = Game1.currentLocation;
        Game1.emoteMenu?.exitThisMenuNoSound();
        if (Game1.client != null)
          Game1.currentLocation?.StoreCachedMultiplayerMap(Game1.multiplayer.cachedMultiplayerMaps);
        Game1.currentLocation.cleanupBeforePlayerExit();
        Game1.multiplayer.broadcastLocationDelta(Game1.currentLocation);
        bool flag = false;
        Game1.displayFarmer = true;
        if (Game1.eventOver)
        {
          Game1.eventFinished();
          if (Game1.dayOfMonth == 0)
            Game1.newDayAfterFade((Action) (() => Game1.player.Position = new Vector2(320f, 320f)));
          return true;
        }
        if (Game1.locationRequest.IsRequestFor(Game1.currentLocation) && Game1.player.previousLocationName != "" && !Game1.eventUp && !MineShaft.IsGeneratedLevel(Game1.currentLocation))
        {
          Game1.player.Position = new Vector2((float) (Game1.xLocationAfterWarp * 64 /*0x40*/), (float) (Game1.yLocationAfterWarp * 64 /*0x40*/ - (Game1.player.Sprite.getHeight() - 32 /*0x20*/) + 16 /*0x10*/));
          Game1.viewportFreeze = false;
          Game1.currentLocation.resetForPlayerEntry();
          flag = true;
        }
        else
        {
          if (MineShaft.IsGeneratedLevel(Game1.locationRequest.Name))
          {
            MineShaft location = Game1.locationRequest.Location as MineShaft;
            if (Game1.player.IsSitting())
              Game1.player.StopSitting(false);
            Game1.player.Halt();
            Game1.player.forceCanMove();
            if (!Game1.IsClient || (NetFieldBase<GameLocation, NetRef<GameLocation>>) Game1.locationRequest.Location?.Root != (NetRef<GameLocation>) null)
            {
              Game1.currentLocation = (GameLocation) location;
              location.resetForPlayerEntry();
              flag = true;
            }
            Game1.currentLocation.Map.LoadTileSheets(Game1.mapDisplayDevice);
            Game1.checkForRunButton(Game1.GetKeyboardState());
          }
          if (!Game1.eventUp)
            Game1.player.Position = new Vector2((float) (Game1.xLocationAfterWarp * 64 /*0x40*/), (float) (Game1.yLocationAfterWarp * 64 /*0x40*/ - (Game1.player.Sprite.getHeight() - 32 /*0x20*/) + 16 /*0x10*/));
          if (!MineShaft.IsGeneratedLevel(Game1.locationRequest.Name) && Game1.locationRequest.Location != null)
          {
            Game1.currentLocation = Game1.locationRequest.Location;
            if (!Game1.IsClient)
            {
              Game1.locationRequest.Loaded(Game1.locationRequest.Location);
              Game1.currentLocation.resetForPlayerEntry();
              flag = true;
            }
            Game1.currentLocation.Map.LoadTileSheets(Game1.mapDisplayDevice);
            if (!Game1.viewportFreeze && Game1.currentLocation.Map.DisplayWidth <= Game1.viewport.Width)
              Game1.viewport.X = (Game1.currentLocation.Map.DisplayWidth - Game1.viewport.Width) / 2;
            if (!Game1.viewportFreeze && Game1.currentLocation.Map.DisplayHeight <= Game1.viewport.Height)
              Game1.viewport.Y = (Game1.currentLocation.Map.DisplayHeight - Game1.viewport.Height) / 2;
            Game1.checkForRunButton(Game1.GetKeyboardState(), true);
          }
          if (!Game1.eventUp)
            Game1.viewportFreeze = false;
        }
        Game1.forceSnapOnNextViewportUpdate = true;
        Game1.player.FarmerSprite.PauseForSingleAnimation = false;
        Game1.player.faceDirection(Game1.facingDirectionAfterWarp);
        Game1._isWarping = false;
        if (Game1.player.ActiveObject != null)
          Game1.player.showCarrying();
        else
          Game1.player.showNotCarrying();
        if (Game1.IsClient)
        {
          if (Game1.locationRequest.Location != null && (NetFieldBase<GameLocation, NetRef<GameLocation>>) Game1.locationRequest.Location.Root != (NetRef<GameLocation>) null && Game1.multiplayer.isActiveLocation(Game1.locationRequest.Location))
          {
            if (Game1.HasDedicatedHost)
              Game1.notifyServerOfWarp(false);
            Game1.currentLocation = Game1.locationRequest.Location;
            Game1.locationRequest.Loaded(Game1.locationRequest.Location);
            if (!flag)
              Game1.currentLocation.resetForPlayerEntry();
            Game1.player.currentLocation = Game1.currentLocation;
            Game1.locationRequest.Warped(Game1.currentLocation);
            Game1.currentLocation.updateSeasonalTileSheets();
            if (Game1.IsDebrisWeatherHere())
              Game1.populateDebrisWeatherArray();
            Game1.warpingForForcedRemoteEvent = false;
            Game1.locationRequest = (LocationRequest) null;
          }
          else
          {
            Game1.requestLocationInfoFromServer();
            if (Game1.currentLocation == null)
              return true;
          }
        }
        else
        {
          Game1.player.currentLocation = Game1.locationRequest.Location;
          Game1.locationRequest.Warped(Game1.locationRequest.Location);
          Game1.locationRequest = (LocationRequest) null;
        }
        if (Game1.locationRequest == null && Game1.currentLocation.Name == "Farm" && !Game1.eventUp)
        {
          if ((double) Game1.player.position.X / 64.0 >= (double) (Game1.currentLocation.map.Layers[0].LayerWidth - 1))
            Game1.player.position.X -= 64f;
          else if ((double) Game1.player.position.Y / 64.0 >= (double) (Game1.currentLocation.map.Layers[0].LayerHeight - 1))
            Game1.player.position.Y -= 32f;
          if ((double) Game1.player.position.Y / 64.0 >= (double) (Game1.currentLocation.map.Layers[0].LayerHeight - 2))
            Game1.player.position.X -= 48f;
        }
        if (MineShaft.IsGeneratedLevel(currentLocation) && Game1.currentLocation != null && !MineShaft.IsGeneratedLevel(Game1.currentLocation))
          MineShaft.OnLeftMines();
        Game1.player.OnWarp();
        blackComplete = true;
      }
    }
    if (Game1.newDay)
    {
      Game1.newDayAfterFade(new Action(AfterNewDay));
      return true;
    }
    if (Game1.eventOver)
    {
      Game1.eventFinished();
      if (Game1.dayOfMonth == 0)
        Game1.newDayAfterFade(new Action(AfterEventOver));
      return true;
    }
    if (Game1.currentSong?.Name == "rain" && Game1.currentLocation.IsRainingHere())
    {
      if (Game1.currentLocation.IsOutdoors)
        Game1.currentSong.SetVariable("Frequency", 100f);
      else if (!MineShaft.IsGeneratedLevel(Game1.currentLocation.Name))
        Game1.currentSong.SetVariable("Frequency", 15f);
    }
    return blackComplete;

    static void AfterNewDay()
    {
      if (Game1.eventOver)
      {
        Game1.eventFinished();
        if (Game1.dayOfMonth == 0)
          Game1.newDayAfterFade((Action) (() => Game1.player.Position = new Vector2(320f, 320f)));
      }
      Game1.nonWarpFade = false;
      Game1.fadeIn = false;
    }

    static void AfterEventOver()
    {
      Game1.currentLocation.resetForPlayerEntry();
      Game1.nonWarpFade = false;
      Game1.fadeIn = false;
    }
  }

  /// <summary>Update game state when the current player finishes warping to a new location.</summary>
  /// <param name="oldLocation">The location which the player just left (or <c>null</c> for the first location after loading the save).</param>
  /// <param name="newLocation">The location which the player just arrived in.</param>
  public static void OnLocationChanged(GameLocation oldLocation, GameLocation newLocation)
  {
    if (!Game1.hasLoadedGame)
      return;
    Game1.eventsSeenSinceLastLocationChange.Clear();
    if (newLocation.Name != null && !MineShaft.IsGeneratedLevel(newLocation) && !VolcanoDungeon.IsGeneratedLevel(newLocation.Name))
      Game1.player.locationsVisited.Add(newLocation.Name);
    if (newLocation.IsOutdoors && !newLocation.ignoreDebrisWeather.Value && newLocation.IsDebrisWeatherHere())
    {
      int seasonForLocation = (int) Game1.GetSeasonForLocation(newLocation);
      Season? debrisWeatherSeason = Game1.debrisWeatherSeason;
      int valueOrDefault = (int) debrisWeatherSeason.GetValueOrDefault();
      if (!(seasonForLocation == valueOrDefault & debrisWeatherSeason.HasValue))
      {
        Game1.windGust = 0.0f;
        WeatherDebris.globalWind = 0.0f;
        Game1.populateDebrisWeatherArray();
        if (Game1.wind != null)
        {
          Game1.wind.Stop(AudioStopOptions.AsAuthored);
          Game1.wind = (ICue) null;
        }
      }
    }
    GameLocation.HandleMusicChange(oldLocation, newLocation);
    TriggerActionManager.Raise("LocationChanged");
  }

  private static void onFadedBackInComplete()
  {
    if (Game1.killScreen)
      Game1.pauseThenMessage(1500, $"...{Game1.player.Name}?");
    else if (!Game1.eventUp)
      Game1.player.CanMove = true;
    Game1.checkForRunButton(Game1.oldKBState, true);
  }

  public static void UpdateOther(GameTime time)
  {
    if (Game1.currentLocation == null || !Game1.player.passedOut && Game1.screenFade.UpdateFade(time))
      return;
    if (Game1.dialogueUp)
      Game1.player.CanMove = false;
    for (int index = Game1.delayedActions.Count - 1; index >= 0; --index)
    {
      DelayedAction delayedAction = Game1.delayedActions[index];
      if (delayedAction.update(time) && Game1.delayedActions.Contains(delayedAction))
        Game1.delayedActions.Remove(delayedAction);
    }
    if (Game1.timeOfDay >= 2600 || (double) Game1.player.stamina <= -15.0)
    {
      if (Game1.currentMinigame != null && Game1.currentMinigame.forceQuit())
        Game1.currentMinigame = (IMinigame) null;
      if (Game1.currentMinigame == null && Game1.player.canMove && Game1.player.freezePause <= 0 && !Game1.player.UsingTool && !Game1.eventUp && (Game1.IsMasterGame || Game1.player.isCustomized.Value) && Game1.locationRequest == null && Game1.activeClickableMenu == null)
      {
        Game1.player.startToPassOut();
        Game1.player.freezePause = 7000;
      }
    }
    Game1.screenOverlayTempSprites.RemoveWhere<TemporaryAnimatedSprite>((Predicate<TemporaryAnimatedSprite>) (sprite => sprite.update(time)));
    Game1.uiOverlayTempSprites.RemoveWhere<TemporaryAnimatedSprite>((Predicate<TemporaryAnimatedSprite>) (sprite => sprite.update(time)));
    if ((Game1.player.CanMove || Game1.player.UsingTool) && Game1.shouldTimePass())
      Game1.buffsDisplay.update(time);
    Game1.player.CurrentItem?.actionWhenBeingHeld(Game1.player);
    float dialogueButtonScale = Game1.dialogueButtonScale;
    TimeSpan timeSpan = time.TotalGameTime;
    Game1.dialogueButtonScale = (float) (16.0 * Math.Sin(timeSpan.TotalMilliseconds % 1570.0 / 500.0));
    if ((double) dialogueButtonScale > (double) Game1.dialogueButtonScale && !Game1.dialogueButtonShrinking)
      Game1.dialogueButtonShrinking = true;
    else if ((double) dialogueButtonScale < (double) Game1.dialogueButtonScale && Game1.dialogueButtonShrinking)
      Game1.dialogueButtonShrinking = false;
    if (Game1.screenGlow)
    {
      if (Game1.screenGlowUp || Game1.screenGlowHold)
      {
        if (Game1.screenGlowHold)
        {
          Game1.screenGlowAlpha = Math.Min(Game1.screenGlowAlpha + Game1.screenGlowRate, Game1.screenGlowMax);
        }
        else
        {
          Game1.screenGlowAlpha = Math.Min(Game1.screenGlowAlpha + 0.03f, 0.6f);
          if ((double) Game1.screenGlowAlpha >= 0.60000002384185791)
            Game1.screenGlowUp = false;
        }
      }
      else
      {
        Game1.screenGlowAlpha -= 0.01f;
        if ((double) Game1.screenGlowAlpha <= 0.0)
          Game1.screenGlow = false;
      }
    }
    Game1.hudMessages.RemoveAll((Predicate<HUDMessage>) (hudMessage => hudMessage.update(time)));
    Game1.updateWeather(time);
    if (!Game1.fadeToBlack)
      Game1.currentLocation.checkForMusic(time);
    if ((double) Game1.debrisSoundInterval > 0.0)
    {
      double debrisSoundInterval = (double) Game1.debrisSoundInterval;
      timeSpan = time.ElapsedGameTime;
      double milliseconds = (double) timeSpan.Milliseconds;
      Game1.debrisSoundInterval = (float) (debrisSoundInterval - milliseconds);
    }
    double noteBlockTimer = (double) Game1.noteBlockTimer;
    timeSpan = time.ElapsedGameTime;
    double milliseconds1 = (double) timeSpan.Milliseconds;
    Game1.noteBlockTimer = (float) (noteBlockTimer + milliseconds1);
    if ((double) Game1.noteBlockTimer > 1000.0)
    {
      Game1.noteBlockTimer = 0.0f;
      if (Game1.player.health < 20 && Game1.CurrentEvent == null)
      {
        Game1.hitShakeTimer = 250;
        if (Game1.player.health <= 10)
        {
          Game1.hitShakeTimer = 500;
          if (Game1.showingHealthBar && (double) Game1.fadeToBlackAlpha <= 0.0)
          {
            for (int index = 0; index < 3; ++index)
              Game1.uiOverlayTempSprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(366, 412, 5, 6), new Vector2((float) (Game1.random.Next(32 /*0x20*/) + Game1.uiViewport.Width - 112 /*0x70*/), (float) (Game1.uiViewport.Height - 224 /*0xE0*/ - (Game1.player.maxHealth - 100) - 16 /*0x10*/ + 4)), false, 0.017f, Color.Red)
              {
                motion = new Vector2(-1.5f, (float) (Game1.random.Next(-1, 2) - 8)),
                acceleration = new Vector2(0.0f, 0.5f),
                local = true,
                scale = 4f,
                delayBeforeAnimationStart = index * 150
              });
          }
        }
      }
    }
    Game1.drawLighting = Game1.currentLocation.IsOutdoors && !Game1.outdoorLight.Equals(Color.White) || !Game1.ambientLight.Equals(Color.White) || Game1.currentLocation is MineShaft && !((MineShaft) Game1.currentLocation).getLightingColor(time).Equals(Color.White);
    if (Game1.player.hasBuff("26"))
      Game1.drawLighting = true;
    if (Game1.hitShakeTimer > 0)
    {
      int hitShakeTimer = Game1.hitShakeTimer;
      timeSpan = time.ElapsedGameTime;
      int milliseconds2 = timeSpan.Milliseconds;
      Game1.hitShakeTimer = hitShakeTimer - milliseconds2;
    }
    if (Game1.staminaShakeTimer > 0)
    {
      int staminaShakeTimer = Game1.staminaShakeTimer;
      timeSpan = time.ElapsedGameTime;
      int milliseconds3 = timeSpan.Milliseconds;
      Game1.staminaShakeTimer = staminaShakeTimer - milliseconds3;
    }
    Game1.background?.update(Game1.viewport);
    int tileHintCheckTimer = Game1.cursorTileHintCheckTimer;
    timeSpan = time.ElapsedGameTime;
    int totalMilliseconds = (int) timeSpan.TotalMilliseconds;
    Game1.cursorTileHintCheckTimer = tileHintCheckTimer - totalMilliseconds;
    Game1.currentCursorTile.X = (float) ((Game1.viewport.X + Game1.getOldMouseX()) / 64 /*0x40*/);
    Game1.currentCursorTile.Y = (float) ((Game1.viewport.Y + Game1.getOldMouseY()) / 64 /*0x40*/);
    if (Game1.cursorTileHintCheckTimer <= 0 || !Game1.currentCursorTile.Equals(Game1.lastCursorTile))
    {
      Game1.cursorTileHintCheckTimer = 250;
      Game1.updateCursorTileHint();
      if (Game1.player.CanMove)
        Game1.checkForRunButton(Game1.oldKBState, true);
    }
    if (!MineShaft.IsGeneratedLevel(Game1.currentLocation.Name))
      MineShaft.timeSinceLastMusic = 200000;
    if (Game1.activeClickableMenu != null || Game1.farmEvent != null || Game1.keyboardDispatcher == null || Game1.IsChatting)
      return;
    Game1.keyboardDispatcher.Subscriber = (IKeyboardSubscriber) null;
  }

  public static void updateWeather(GameTime time)
  {
    if (Game1.currentLocation.IsOutdoors && Game1.currentLocation.IsSnowingHere())
    {
      Vector2 current = new Vector2((float) Game1.viewport.X, (float) Game1.viewport.Y);
      Game1.snowPos = Game1.updateFloatingObjectPositionForMovement(Game1.snowPos, current, Game1.previousViewportPosition, -1f);
    }
    else if (Game1.currentLocation.IsOutdoors && Game1.currentLocation.IsRainingHere())
    {
      for (int index = 0; index < Game1.rainDrops.Length; ++index)
      {
        if (Game1.rainDrops[index].frame == 0)
        {
          Game1.rainDrops[index].accumulator += time.ElapsedGameTime.Milliseconds;
          if (Game1.rainDrops[index].accumulator >= 70)
          {
            Game1.rainDrops[index].position += new Vector2((float) (index * 8 / Game1.rainDrops.Length - 16 /*0x10*/), (float) (32 /*0x20*/ - index * 8 / Game1.rainDrops.Length));
            Game1.rainDrops[index].accumulator = 0;
            if (Game1.random.NextDouble() < 0.1)
              ++Game1.rainDrops[index].frame;
            if (Game1.currentLocation is IslandNorth || Game1.currentLocation is Caldera)
            {
              Point tile = new Point((int) ((double) Game1.rainDrops[index].position.X + (double) Game1.viewport.X) / 64 /*0x40*/, (int) ((double) Game1.rainDrops[index].position.Y + (double) Game1.viewport.Y) / 64 /*0x40*/);
              --tile.Y;
              if (Game1.currentLocation.isTileOnMap(tile.X, tile.Y) && !Game1.currentLocation.hasTileAt(tile, "Back") && !Game1.currentLocation.hasTileAt(tile, "Buildings"))
                Game1.rainDrops[index].frame = 0;
            }
            if ((double) Game1.rainDrops[index].position.Y > (double) (Game1.viewport.Height + 64 /*0x40*/))
              Game1.rainDrops[index].position.Y = -64f;
          }
        }
        else
        {
          Game1.rainDrops[index].accumulator += time.ElapsedGameTime.Milliseconds;
          if (Game1.rainDrops[index].accumulator > 70)
          {
            Game1.rainDrops[index].frame = (Game1.rainDrops[index].frame + 1) % 4;
            Game1.rainDrops[index].accumulator = 0;
            if (Game1.rainDrops[index].frame == 0)
              Game1.rainDrops[index].position = new Vector2((float) Game1.random.Next(Game1.viewport.Width), (float) Game1.random.Next(Game1.viewport.Height));
          }
        }
      }
    }
    else if (Game1.currentLocation.IsOutdoors && !Game1.currentLocation.ignoreDebrisWeather.Value && Game1.currentLocation.IsDebrisWeatherHere())
    {
      if (Game1.currentLocation.GetSeason() == Season.Fall)
      {
        if ((double) WeatherDebris.globalWind == 0.0)
          WeatherDebris.globalWind = -0.5f;
        if (Game1.random.NextDouble() < 0.001 && (double) Game1.windGust == 0.0 && (double) WeatherDebris.globalWind >= -0.5)
        {
          Game1.windGust += (float) Game1.random.Next(-10, -1) / 100f;
          Game1.playSound("wind", out Game1.wind);
        }
        else if ((double) Game1.windGust != 0.0)
        {
          Game1.windGust = Math.Max(-5f, Game1.windGust * 1.02f);
          WeatherDebris.globalWind = Game1.windGust - 0.5f;
          if ((double) Game1.windGust < -0.20000000298023224 && Game1.random.NextDouble() < 0.007)
            Game1.windGust = 0.0f;
        }
        if ((double) WeatherDebris.globalWind < -0.5)
        {
          WeatherDebris.globalWind = Math.Min(-0.5f, WeatherDebris.globalWind + 0.015f);
          if (Game1.wind != null)
          {
            Game1.wind.SetVariable("Volume", (float) (-(double) WeatherDebris.globalWind * 20.0));
            Game1.wind.SetVariable("Frequency", (float) (-(double) WeatherDebris.globalWind * 20.0));
            if ((double) WeatherDebris.globalWind == -0.5)
              Game1.wind.Stop(AudioStopOptions.AsAuthored);
          }
        }
      }
      else
      {
        if ((double) WeatherDebris.globalWind == 0.0)
          WeatherDebris.globalWind = -0.25f;
        if (Game1.wind != null)
        {
          Game1.wind.Stop(AudioStopOptions.AsAuthored);
          Game1.wind = (ICue) null;
        }
      }
      foreach (WeatherDebris weatherDebris in Game1.debrisWeather)
        weatherDebris.update();
    }
    else
    {
      if (Game1.wind == null)
        return;
      Game1.wind.Stop(AudioStopOptions.AsAuthored);
      Game1.wind = (ICue) null;
    }
  }

  public static void updateCursorTileHint()
  {
    if (Game1.activeClickableMenu != null)
      return;
    Game1.mouseCursorTransparency = 1f;
    Game1.isActionAtCurrentCursorTile = false;
    Game1.isInspectionAtCurrentCursorTile = false;
    Game1.isSpeechAtCurrentCursorTile = false;
    int xTile = (Game1.viewport.X + Game1.getOldMouseX()) / 64 /*0x40*/;
    int yTile = (Game1.viewport.Y + Game1.getOldMouseY()) / 64 /*0x40*/;
    if (Game1.currentLocation != null)
    {
      Game1.isActionAtCurrentCursorTile = Game1.currentLocation.isActionableTile(xTile, yTile, Game1.player);
      if (!Game1.isActionAtCurrentCursorTile)
        Game1.isActionAtCurrentCursorTile = Game1.currentLocation.isActionableTile(xTile, yTile + 1, Game1.player);
    }
    Game1.lastCursorTile = Game1.currentCursorTile;
  }

  public static void updateMusic()
  {
    if (Game1.game1.IsMainInstance)
    {
      Game1 game1 = (Game1) null;
      string newTrackName = (string) null;
      int num1 = 1;
      int num2 = 2;
      int num3 = 5;
      int num4 = 6;
      int num5 = 7;
      int num6 = 0;
      float num7 = (float) Game1.GetDefaultSongPriority(Game1.getMusicTrackName(), Game1.game1.instanceIsOverridingTrack, Game1.game1);
      MusicContext musicContext = MusicContext.Default;
      foreach (Game1 gameInstance in GameRunner.instance.gameInstances)
      {
        MusicContext activeMusicContext = gameInstance._instanceActiveMusicContext;
        if (gameInstance.IsMainInstance)
          musicContext = activeMusicContext;
        string song_name = (string) null;
        string str = (string) null;
        KeyValuePair<string, bool> keyValuePair;
        if (gameInstance._instanceRequestedMusicTracks.TryGetValue(activeMusicContext, out keyValuePair))
          song_name = keyValuePair.Key;
        if (gameInstance.instanceIsOverridingTrack && gameInstance.instanceCurrentSong != null)
          str = gameInstance.instanceCurrentSong.Name;
        switch (activeMusicContext)
        {
          case MusicContext.Default:
            if (song_name == "mermaidSong")
            {
              num6 = num5;
              game1 = gameInstance;
              newTrackName = song_name;
            }
            if (musicContext <= activeMusicContext && song_name != null)
            {
              float defaultSongPriority = (float) Game1.GetDefaultSongPriority(song_name, gameInstance.instanceIsOverridingTrack, gameInstance);
              if ((double) num7 < (double) defaultSongPriority)
              {
                num7 = defaultSongPriority;
                num6 = num2;
                game1 = gameInstance;
                newTrackName = str == null ? song_name : str;
                continue;
              }
              continue;
            }
            continue;
          case MusicContext.SubLocation:
            if (num6 < num1 && song_name != null)
            {
              num6 = num1;
              game1 = gameInstance;
              newTrackName = str == null ? song_name : str;
              continue;
            }
            continue;
          case MusicContext.Event:
            if (num6 < num4 && song_name != null)
            {
              num6 = num4;
              game1 = gameInstance;
              newTrackName = song_name;
              continue;
            }
            continue;
          case MusicContext.MiniGame:
            if (num6 < num3 && song_name != null)
            {
              num6 = num3;
              game1 = gameInstance;
              newTrackName = song_name;
              continue;
            }
            continue;
          default:
            continue;
        }
      }
      if (game1 == null || game1 == Game1.game1)
      {
        if (Game1.doesMusicContextHaveTrack(MusicContext.ImportantSplitScreenMusic))
          Game1.stopMusicTrack(MusicContext.ImportantSplitScreenMusic);
      }
      else if (newTrackName == null && Game1.doesMusicContextHaveTrack(MusicContext.ImportantSplitScreenMusic))
        Game1.stopMusicTrack(MusicContext.ImportantSplitScreenMusic);
      else if (newTrackName != null && Game1.getMusicTrackName(MusicContext.ImportantSplitScreenMusic) != newTrackName)
        Game1.changeMusicTrack(newTrackName, music_context: MusicContext.ImportantSplitScreenMusic);
    }
    string name1 = (string) null;
    bool flag1 = false;
    bool flag2 = false;
    if (Game1.currentLocation != null && Game1.currentLocation.IsMiniJukeboxPlaying() && (!Game1.requestedMusicDirty || Game1.requestedMusicTrackOverrideable) && Game1.currentTrackOverrideable)
    {
      name1 = (string) null;
      flag2 = true;
      string name2 = Game1.currentLocation.miniJukeboxTrack.Value;
      if (name2 == "random")
        name2 = Game1.currentLocation.randomMiniJukeboxTrack.Value != null ? Game1.currentLocation.randomMiniJukeboxTrack.Value : "";
      if (Game1.currentSong == null || !Game1.currentSong.IsPlaying || Game1.currentSong.Name != name2)
      {
        if (!Game1.soundBank.Exists(name2))
        {
          Game1.log.Error($"Location {Game1.currentLocation.NameOrUniqueName} has invalid jukebox track '{name2}' selected, turning off jukebox.");
          Game1.player.currentLocation.miniJukeboxTrack.Value = "";
        }
        else
        {
          name1 = name2;
          Game1.requestedMusicDirty = false;
          flag1 = true;
        }
      }
    }
    if (Game1.isOverridingTrack != flag2)
    {
      Game1.isOverridingTrack = flag2;
      if (!Game1.isOverridingTrack)
        Game1.requestedMusicDirty = true;
    }
    if (Game1.requestedMusicDirty)
    {
      name1 = Game1.requestedMusicTrack;
      flag1 = Game1.requestedMusicTrackOverrideable;
    }
    if (!string.IsNullOrEmpty(name1))
    {
      Game1.musicPlayerVolume = Math.Max(0.0f, Math.Min(Game1.options.musicVolumeLevel, Game1.musicPlayerVolume - 0.01f));
      Game1.ambientPlayerVolume = Math.Max(0.0f, Math.Min(Game1.options.musicVolumeLevel, Game1.ambientPlayerVolume - 0.01f));
      if (Game1.game1.IsMainInstance)
      {
        Game1.musicCategory.SetVolume(Game1.musicPlayerVolume);
        Game1.ambientCategory.SetVolume(Game1.ambientPlayerVolume);
      }
      if ((double) Game1.musicPlayerVolume != 0.0 || (double) Game1.ambientPlayerVolume != 0.0)
        return;
      if (name1 == "none" || name1 == "silence")
      {
        if (Game1.game1.IsMainInstance && Game1.currentSong != null)
        {
          Game1.currentSong.Stop(AudioStopOptions.Immediate);
          Game1.currentSong.Dispose();
          Game1.currentSong = (ICue) null;
        }
      }
      else if (((double) Game1.options.musicVolumeLevel != 0.0 || (double) Game1.options.ambientVolumeLevel != 0.0) && (name1 != "rain" || Game1.endOfNightMenus.Count == 0))
      {
        if (Game1.game1.IsMainInstance && Game1.currentSong != null)
        {
          Game1.currentSong.Stop(AudioStopOptions.Immediate);
          Game1.currentSong.Dispose();
          Game1.currentSong = (ICue) null;
        }
        Game1.currentSong = Game1.soundBank.GetCue(name1);
        if (Game1.game1.IsMainInstance)
          Game1.currentSong.Play();
        if (Game1.game1.IsMainInstance && Game1.currentSong != null && Game1.currentSong.Name == "rain" && Game1.currentLocation != null)
        {
          if (Game1.IsRainingHere())
          {
            if (Game1.currentLocation.IsOutdoors)
              Game1.currentSong.SetVariable("Frequency", 100f);
            else if (!MineShaft.IsGeneratedLevel(Game1.currentLocation))
              Game1.currentSong.SetVariable("Frequency", 15f);
          }
          else if (Game1.eventUp)
            Game1.currentSong.SetVariable("Frequency", 100f);
        }
      }
      else
        Game1.currentSong?.Stop(AudioStopOptions.Immediate);
      Game1.currentTrackOverrideable = flag1;
      Game1.requestedMusicDirty = false;
    }
    else if ((double) Game1.MusicDuckTimer > 0.0)
    {
      Game1.MusicDuckTimer -= (float) Game1.currentGameTime.ElapsedGameTime.TotalMilliseconds;
      Game1.musicPlayerVolume = Math.Max(Game1.musicPlayerVolume - Game1.options.musicVolumeLevel / 33f, Game1.options.musicVolumeLevel / 12f);
      if (!Game1.game1.IsMainInstance)
        return;
      Game1.musicCategory.SetVolume(Game1.musicPlayerVolume);
    }
    else if ((double) Game1.musicPlayerVolume < (double) Game1.options.musicVolumeLevel || (double) Game1.ambientPlayerVolume < (double) Game1.options.ambientVolumeLevel)
    {
      if ((double) Game1.musicPlayerVolume < (double) Game1.options.musicVolumeLevel)
      {
        Game1.musicPlayerVolume = Math.Min(1f, Game1.musicPlayerVolume += 0.01f);
        if (Game1.game1.IsMainInstance)
          Game1.musicCategory.SetVolume(Game1.musicPlayerVolume);
      }
      if ((double) Game1.ambientPlayerVolume >= (double) Game1.options.ambientVolumeLevel)
        return;
      Game1.ambientPlayerVolume = Math.Min(1f, Game1.ambientPlayerVolume += 0.015f);
      if (!Game1.game1.IsMainInstance)
        return;
      Game1.ambientCategory.SetVolume(Game1.ambientPlayerVolume);
    }
    else
    {
      if (Game1.currentSong == null || Game1.currentSong.IsPlaying || Game1.currentSong.IsStopped)
        return;
      Game1.currentSong = Game1.soundBank.GetCue(Game1.currentSong.Name);
      if (!Game1.game1.IsMainInstance)
        return;
      Game1.currentSong.Play();
    }
  }

  public static int GetDefaultSongPriority(
    string song_name,
    bool is_playing_override,
    Game1 instance)
  {
    if (is_playing_override)
      return 9;
    if (song_name == "none")
      return 0;
    if (instance._instanceIsPlayingOutdoorsAmbience || instance._instanceIsPlayingNightAmbience || song_name == "rain")
      return 1;
    if (instance._instanceIsPlayingMorningSong)
      return 2;
    if (instance._instanceIsPlayingTownMusic)
      return 3;
    if (song_name == "jungle_ambience")
      return 7;
    if (instance._instanceIsPlayingBackgroundMusic)
      return 8;
    if (instance.instanceGameLocation is MineShaft)
    {
      if (song_name.Contains("Ambient"))
        return 7;
      if (song_name.EndsWith("Mine"))
        return 20;
    }
    return 10;
  }

  public static void updateRainDropPositionForPlayerMovement(int direction, float speed)
  {
    if (Game1.currentLocation.IsRainingHere())
    {
      for (int index = 0; index < Game1.rainDrops.Length; ++index)
      {
        switch (direction)
        {
          case 0:
            Game1.rainDrops[index].position.Y += speed;
            if ((double) Game1.rainDrops[index].position.Y > (double) (Game1.viewport.Height + 64 /*0x40*/))
            {
              Game1.rainDrops[index].position.Y = -64f;
              break;
            }
            break;
          case 1:
            Game1.rainDrops[index].position.X -= speed;
            if ((double) Game1.rainDrops[index].position.X < -64.0)
            {
              Game1.rainDrops[index].position.X = (float) Game1.viewport.Width;
              break;
            }
            break;
          case 2:
            Game1.rainDrops[index].position.Y -= speed;
            if ((double) Game1.rainDrops[index].position.Y < -64.0)
            {
              Game1.rainDrops[index].position.Y = (float) Game1.viewport.Height;
              break;
            }
            break;
          case 3:
            Game1.rainDrops[index].position.X += speed;
            if ((double) Game1.rainDrops[index].position.X > (double) (Game1.viewport.Width + 64 /*0x40*/))
            {
              Game1.rainDrops[index].position.X = -64f;
              break;
            }
            break;
        }
      }
    }
    else
      Game1.updateDebrisWeatherForMovement(Game1.debrisWeather, direction, speed);
  }

  public static void initializeVolumeLevels()
  {
    if (LocalMultiplayer.IsLocalMultiplayer() && !Game1.game1.IsMainInstance)
      return;
    Game1.soundCategory.SetVolume(Game1.options.soundVolumeLevel);
    Game1.musicCategory.SetVolume(Game1.options.musicVolumeLevel);
    Game1.ambientCategory.SetVolume(Game1.options.ambientVolumeLevel);
    Game1.footstepCategory.SetVolume(Game1.options.footstepVolumeLevel);
  }

  public static void updateDebrisWeatherForMovement(
    List<WeatherDebris> debris,
    int direction,
    float speed)
  {
    if ((double) Game1.fadeToBlackAlpha > 0.0 || debris == null)
      return;
    foreach (WeatherDebris debri in debris)
    {
      switch (direction)
      {
        case 0:
          debri.position.Y += speed;
          if ((double) debri.position.Y > (double) (Game1.viewport.Height + 64 /*0x40*/))
          {
            debri.position.Y = -64f;
            continue;
          }
          continue;
        case 1:
          debri.position.X -= speed;
          if ((double) debri.position.X < -64.0)
          {
            debri.position.X = (float) Game1.viewport.Width;
            continue;
          }
          continue;
        case 2:
          debri.position.Y -= speed;
          if ((double) debri.position.Y < -64.0)
          {
            debri.position.Y = (float) Game1.viewport.Height;
            continue;
          }
          continue;
        case 3:
          debri.position.X += speed;
          if ((double) debri.position.X > (double) (Game1.viewport.Width + 64 /*0x40*/))
          {
            debri.position.X = -64f;
            continue;
          }
          continue;
        default:
          continue;
      }
    }
  }

  public static Vector2 updateFloatingObjectPositionForMovement(
    Vector2 w,
    Vector2 current,
    Vector2 previous,
    float speed)
  {
    if ((double) current.Y < (double) previous.Y)
      w.Y -= Math.Abs(current.Y - previous.Y) * speed;
    else if ((double) current.Y > (double) previous.Y)
      w.Y += Math.Abs(current.Y - previous.Y) * speed;
    if ((double) current.X > (double) previous.X)
      w.X += Math.Abs(current.X - previous.X) * speed;
    else if ((double) current.X < (double) previous.X)
      w.X -= Math.Abs(current.X - previous.X) * speed;
    return w;
  }

  public static void updateRaindropPosition()
  {
    if (Game1.HostPaused)
      return;
    if (Game1.IsRainingHere())
    {
      int num1 = Game1.viewport.X - (int) Game1.previousViewportPosition.X;
      int num2 = Game1.viewport.Y - (int) Game1.previousViewportPosition.Y;
      for (int index = 0; index < Game1.rainDrops.Length; ++index)
      {
        Game1.rainDrops[index].position.X -= (float) num1 * 1f;
        Game1.rainDrops[index].position.Y -= (float) num2 * 1f;
        if ((double) Game1.rainDrops[index].position.Y > (double) (Game1.viewport.Height + 64 /*0x40*/))
          Game1.rainDrops[index].position.Y = -64f;
        else if ((double) Game1.rainDrops[index].position.X < -64.0)
          Game1.rainDrops[index].position.X = (float) Game1.viewport.Width;
        else if ((double) Game1.rainDrops[index].position.Y < -64.0)
          Game1.rainDrops[index].position.Y = (float) Game1.viewport.Height;
        else if ((double) Game1.rainDrops[index].position.X > (double) (Game1.viewport.Width + 64 /*0x40*/))
          Game1.rainDrops[index].position.X = -64f;
      }
    }
    else
      Game1.updateDebrisWeatherForMovement(Game1.debrisWeather);
  }

  public static void updateDebrisWeatherForMovement(List<WeatherDebris> debris)
  {
    if (Game1.HostPaused || debris == null || (double) Game1.fadeToBlackAlpha >= 1.0)
      return;
    int num1 = Game1.viewport.X - (int) Game1.previousViewportPosition.X;
    int num2 = Game1.viewport.Y - (int) Game1.previousViewportPosition.Y;
    if (Math.Abs(num1) > 100 || Math.Abs(num2) > 80 /*0x50*/)
      return;
    int num3 = 16 /*0x10*/;
    foreach (WeatherDebris debri in debris)
    {
      debri.position.X -= (float) num1 * 1f;
      debri.position.Y -= (float) num2 * 1f;
      if ((double) debri.position.Y > (double) (Game1.viewport.Height + 64 /*0x40*/ + num3))
        debri.position.Y = -64f;
      else if ((double) debri.position.X < (double) (-64 - num3))
        debri.position.X = (float) Game1.viewport.Width;
      else if ((double) debri.position.Y < (double) (-64 - num3))
        debri.position.Y = (float) Game1.viewport.Height;
      else if ((double) debri.position.X > (double) (Game1.viewport.Width + 64 /*0x40*/ + num3))
        debri.position.X = -64f;
    }
  }

  public static void randomizeRainPositions()
  {
    for (int index = 0; index < 70; ++index)
      Game1.rainDrops[index] = new RainDrop(Game1.random.Next(Game1.viewport.Width), Game1.random.Next(Game1.viewport.Height), Game1.random.Next(4), Game1.random.Next(70));
  }

  public static void randomizeDebrisWeatherPositions(List<WeatherDebris> debris)
  {
    if (debris == null)
      return;
    foreach (WeatherDebris debri in debris)
      debri.position = Utility.getRandomPositionOnScreen();
  }

  public static void eventFinished()
  {
    Game1.player.canOnlyWalk = false;
    if (Game1.player.bathingClothes.Value)
      Game1.player.canOnlyWalk = true;
    Game1.eventOver = false;
    Game1.eventUp = false;
    Game1.player.CanMove = true;
    Game1.displayHUD = true;
    Game1.player.faceDirection(Game1.player.orientationBeforeEvent);
    Game1.player.completelyStopAnimatingOrDoingAction();
    Game1.viewportFreeze = false;
    Action action = (Action) null;
    if (Game1.currentLocation.currentEvent?.onEventFinished != null)
    {
      action = Game1.currentLocation.currentEvent.onEventFinished;
      Game1.currentLocation.currentEvent.onEventFinished = (Action) null;
    }
    LocationRequest locationRequest = (LocationRequest) null;
    if (Game1.currentLocation.currentEvent != null)
    {
      locationRequest = Game1.currentLocation.currentEvent.exitLocation;
      Game1.currentLocation.currentEvent.cleanup();
      Game1.currentLocation.currentEvent = (Event) null;
    }
    if (Game1.player.ActiveObject != null)
      Game1.player.showCarrying();
    if (Game1.dayOfMonth != 0)
      Game1.currentLightSources.Clear();
    if (locationRequest == null && Game1.currentLocation != null && Game1.locationRequest == null)
      locationRequest = new LocationRequest(Game1.currentLocation.NameOrUniqueName, Game1.currentLocation.isStructure.Value, Game1.currentLocation);
    if (locationRequest != null)
    {
      if (locationRequest.Location is Farm && (double) Game1.player.positionBeforeEvent.Y == 64.0)
        ++Game1.player.positionBeforeEvent.X;
      locationRequest.OnWarp += (LocationRequest.Callback) (() => Game1.player.locationBeforeForcedEvent.Value = (string) null);
      if (locationRequest.Location == Game1.currentLocation)
        GameLocation.HandleMusicChange(Game1.currentLocation, Game1.currentLocation);
      Game1.warpFarmer(locationRequest, (int) Game1.player.positionBeforeEvent.X, (int) Game1.player.positionBeforeEvent.Y, Game1.player.orientationBeforeEvent);
    }
    else
    {
      GameLocation.HandleMusicChange(Game1.currentLocation, Game1.currentLocation);
      Game1.player.setTileLocation(Game1.player.positionBeforeEvent);
      Game1.player.locationBeforeForcedEvent.Value = (string) null;
    }
    Game1.nonWarpFade = false;
    Game1.fadeToBlackAlpha = 1f;
    if (action == null)
      return;
    action();
  }

  public static void populateDebrisWeatherArray()
  {
    Season seasonForLocation = Game1.GetSeasonForLocation(Game1.currentLocation);
    int num = Game1.random.Next(16 /*0x10*/, 64 /*0x40*/);
    int which;
    switch (seasonForLocation)
    {
      case Season.Summer:
        which = 1;
        break;
      case Season.Fall:
        which = 2;
        break;
      case Season.Winter:
        which = 3;
        break;
      default:
        which = 0;
        break;
    }
    Game1.isDebrisWeather = true;
    Game1.debrisWeatherSeason = new Season?(seasonForLocation);
    Game1.debrisWeather.Clear();
    for (int index = 0; index < num; ++index)
      Game1.debrisWeather.Add(new WeatherDebris(new Vector2((float) Game1.random.Next(0, Game1.viewport.Width), (float) Game1.random.Next(0, Game1.viewport.Height)), which, (float) Game1.random.Next(15) / 500f, (float) Game1.random.Next(-10, 0) / 50f, (float) Game1.random.Next(10) / 50f));
  }

  private static void OnNewSeason()
  {
    Game1.setGraphicsForSeason();
    Utility.ForEachLocation((Func<GameLocation, bool>) (location =>
    {
      location.seasonUpdate();
      return true;
    }));
  }

  public static void prepareSpouseForWedding(Farmer farmer)
  {
    NPC npc = Game1.RequireCharacter(farmer.spouse);
    npc.ClearSchedule();
    npc.DefaultMap = farmer.homeLocation.Value;
    npc.DefaultPosition = Utility.PointToVector2(Game1.RequireLocation<FarmHouse>(farmer.homeLocation.Value).getSpouseBedSpot(farmer.spouse)) * 64f;
    npc.DefaultFacingDirection = 2;
  }

  public static bool AddCharacterIfNecessary(string characterId, bool bypassConditions = false)
  {
    CharacterData data;
    if (!NPC.TryGetData(characterId, out data))
      return false;
    bool flag = false;
    if (Game1.getCharacterFromName(characterId) == null)
    {
      if (!bypassConditions && !GameStateQuery.CheckConditions(data.UnlockConditions))
        return false;
      string locationName;
      Point tile;
      int direction;
      NPC.ReadNpcHomeData(data, (GameLocation) null, out locationName, out tile, out direction);
      bool canBeRomanced = data.CanBeRomanced;
      Point size = data.Size;
      GameLocation nameInLocationsList = Game1.getLocationFromNameInLocationsList(locationName);
      if (nameInLocationsList == null)
        return false;
      string nameForCharacter = NPC.getTextureNameForCharacter(characterId);
      NPC character;
      try
      {
        character = new NPC(new AnimatedSprite("Characters\\" + nameForCharacter, 0, size.X, size.Y), new Vector2((float) (tile.X * 64 /*0x40*/), (float) (tile.Y * 64 /*0x40*/)), locationName, direction, characterId, canBeRomanced, Game1.content.Load<Texture2D>("Portraits\\" + nameForCharacter));
      }
      catch (Exception ex)
      {
        Game1.log.Error($"Failed to spawn NPC '{characterId}'.", ex);
        return false;
      }
      character.Breather = data.Breather;
      nameInLocationsList.addCharacter(character);
      flag = true;
    }
    if (data.SocialTab == SocialTabBehavior.AlwaysShown && !Game1.player.friendshipData.ContainsKey(characterId))
      Game1.player.friendshipData.Add(characterId, new Friendship());
    return flag;
  }

  public static GameLocation CreateGameLocation(string id)
  {
    if (string.IsNullOrWhiteSpace(id))
      return (GameLocation) null;
    LocationData locationData;
    CreateLocationData createOnLoad = Game1.locationData.TryGetValue(id, out locationData) ? locationData.CreateOnLoad : (CreateLocationData) null;
    return Game1.CreateGameLocation(id, createOnLoad);
  }

  public static GameLocation CreateGameLocation(string id, CreateLocationData createData)
  {
    if (createData == null)
      return (GameLocation) null;
    GameLocation gameLocation;
    if (createData.Type != null)
      gameLocation = (GameLocation) Activator.CreateInstance(Type.GetType(createData.Type) ?? throw new Exception($"Invalid type for location {id}: {createData.Type}"), (object) createData.MapPath, (object) id);
    else
      gameLocation = new GameLocation(createData.MapPath, id);
    gameLocation.isAlwaysActive.Value = createData.AlwaysActive;
    return gameLocation;
  }

  public static void AddLocations()
  {
    bool flag = false;
    foreach (KeyValuePair<string, LocationData> keyValuePair in (IEnumerable<KeyValuePair<string, LocationData>>) Game1.locationData)
    {
      if (keyValuePair.Value.CreateOnLoad != null)
      {
        GameLocation gameLocation;
        try
        {
          gameLocation = Game1.CreateGameLocation(keyValuePair.Key, keyValuePair.Value.CreateOnLoad);
        }
        catch (Exception ex)
        {
          Game1.log.Error($"Couldn't create the '{keyValuePair.Key}' location. Is its data in Data/Locations invalid?", ex);
          continue;
        }
        if (gameLocation == null)
        {
          Game1.log.Error($"Couldn't create the '{keyValuePair.Key}' location. Is its data in Data/Locations invalid?");
        }
        else
        {
          if (!flag)
          {
            try
            {
              gameLocation.map.LoadTileSheets(Game1.mapDisplayDevice);
              Game1.currentLocation = gameLocation;
              flag = true;
            }
            catch (Exception ex)
            {
              Game1.log.Error($"Couldn't load tilesheets for the '{keyValuePair.Key}' location.", ex);
            }
          }
          Game1.locations.Add(gameLocation);
        }
      }
    }
    for (int index = 1; index < Game1.netWorldState.Value.HighestPlayerLimit; ++index)
    {
      GameLocation gameLocation = Game1.CreateGameLocation("Cellar");
      gameLocation.name.Value += (index + 1).ToString();
      Game1.locations.Add(gameLocation);
    }
  }

  public static void AddNPCs()
  {
    foreach (KeyValuePair<string, CharacterData> keyValuePair in (IEnumerable<KeyValuePair<string, CharacterData>>) Game1.characterData)
    {
      if (keyValuePair.Value.SpawnIfMissing)
        Game1.AddCharacterIfNecessary(keyValuePair.Key);
    }
    GameLocation nameInLocationsList = Game1.getLocationFromNameInLocationsList("QiNutRoom");
    if (nameInLocationsList.getCharacterFromName("Mister Qi") != null)
      return;
    AnimatedSprite sprite = new AnimatedSprite("Characters\\MrQi", 0, 16 /*0x10*/, 32 /*0x20*/);
    nameInLocationsList.addCharacter(new NPC(sprite, new Vector2(448f, 256f), "QiNutRoom", 0, "Mister Qi", false, Game1.content.Load<Texture2D>("Portraits\\MrQi")));
  }

  public static void AddModNPCs()
  {
  }

  public static void fixProblems()
  {
    if (!Game1.IsMasterGame)
      return;
    foreach (Farmer allFarmer in Game1.getAllFarmers())
    {
      allFarmer.LearnDefaultRecipes();
      allFarmer.AddMissedMailAndRecipes();
      LevelUpMenu.RevalidateHealth(allFarmer);
      LevelUpMenu.AddMissedProfessionChoices(allFarmer);
    }
    Utility.ForEachLocation((Func<GameLocation, bool>) (location =>
    {
      location.characters.RemoveWhere((Func<NPC, bool>) (npc =>
      {
        if (npc == null)
        {
          Game1.log.Warn($"Removed broken NPC in {location.NameOrUniqueName}: null instance.");
          return true;
        }
        if (npc.IsVillager)
        {
          if (npc.GetData() == null)
          {
            try
            {
              if (npc.Sprite.Texture == null)
              {
                Game1.log.Warn($"Removed broken NPC '{npc.Name}' in {location.NameOrUniqueName}: villager with no data or sprites.");
                return true;
              }
            }
            catch
            {
              Game1.log.Warn($"Removed broken NPC '{npc.Name}' in {location.NameOrUniqueName}: villager with no data or sprites.");
              return true;
            }
          }
        }
        return false;
      }));
      return true;
    }));
    Game1.AddNPCs();
    List<NPC> divorced = (List<NPC>) null;
    Utility.ForEachVillager((Func<NPC, bool>) (n =>
    {
      if (!n.datable.Value || n.getSpouse() != null || n.DefaultMap == null || !n.DefaultMap.ContainsIgnoreCase("cabin") || n.DefaultMap != "FarmHouse")
        return true;
      CharacterData data = n.GetData();
      if (data == null)
        return true;
      string locationName;
      NPC.ReadNpcHomeData(data, n.currentLocation, out locationName, out Point _, out int _);
      if (n.DefaultMap != locationName)
      {
        if (divorced == null)
          divorced = new List<NPC>();
        divorced.Add(n);
      }
      return true;
    }));
    if (divorced != null)
    {
      foreach (NPC npc in divorced)
      {
        Game1.log.Warn($"Fixing {npc.Name} who was improperly divorced and left stranded");
        npc.PerformDivorce();
      }
    }
    foreach (Farmer allFarmer in Game1.getAllFarmers())
    {
      if (allFarmer.hasQuest("130"))
      {
        HashSet<string> requiredQuestItems = new HashSet<string>()
        {
          "(O)864",
          "(O)865",
          "(O)866",
          "(O)867",
          "(O)868",
          "(O)869",
          "(O)870"
        };
        bool found = false;
        foreach (string itemId in requiredQuestItems)
        {
          if (allFarmer.Items.ContainsId(itemId))
          {
            found = true;
            break;
          }
        }
        if (!found)
          Utility.ForEachItem((Func<Item, bool>) (item =>
          {
            found = requiredQuestItems.Contains(item.QualifiedItemId);
            return !found;
          }));
        if (!found)
        {
          Object @object = ItemRegistry.Create<Object>("(O)864");
          @object.specialItem = true;
          @object.questItem.Value = true;
          if (!allFarmer.addItemToInventoryBool((Item) @object))
          {
            allFarmer.team.returnedDonations.Add((Item) @object);
            allFarmer.team.newLostAndFoundItems.Value = true;
          }
        }
      }
      else if (!allFarmer.craftingRecipes.ContainsKey("Fairy Dust") && allFarmer.mailReceived.Contains("birdieQuestBegun"))
        allFarmer.mailReceived.Remove("birdieQuestBegun");
    }
    int num1 = Game1.getAllFarmers().Count<Farmer>();
    Dictionary<Type, int> missingTools = new Dictionary<Type, int>()
    {
      [typeof (Axe)] = num1,
      [typeof (Pickaxe)] = num1,
      [typeof (Hoe)] = num1,
      [typeof (WateringCan)] = num1,
      [typeof (Wand)] = 0
    };
    foreach (Farmer allFarmer in Game1.getAllFarmers())
    {
      if (allFarmer.hasOrWillReceiveMail("ReturnScepter"))
        missingTools[typeof (Wand)]++;
    }
    int missingScythes = num1;
    foreach (Farmer allFarmer in Game1.getAllFarmers())
    {
      if (allFarmer.toolBeingUpgraded.Value != null)
      {
        allFarmer.toolBeingUpgraded.Value.FixStackSize();
        Type type = allFarmer.toolBeingUpgraded.Value.GetType();
        int num2;
        if (missingTools.TryGetValue(type, out num2))
          missingTools[type] = num2 - 1;
      }
      for (int index = 0; index < allFarmer.Items.Count; ++index)
      {
        if (allFarmer.Items[index] != null)
          Game1.checkIsMissingTool(missingTools, ref missingScythes, allFarmer.Items[index]);
      }
    }
    bool flag = true;
    foreach (int num3 in missingTools.Values)
    {
      if (num3 > 0)
      {
        flag = false;
        break;
      }
    }
    if (missingScythes > 0)
      flag = false;
    if (flag)
      return;
    Utility.ForEachLocation((Func<GameLocation, bool>) (l =>
    {
      List<Debris> debrisList = new List<Debris>();
      foreach (Debris debri in l.debris)
      {
        Item obj = debri.item;
        if (obj != null)
        {
          foreach (Type key in missingTools.Keys)
          {
            if (obj.GetType() == key)
              debrisList.Add(debri);
          }
          if (obj.QualifiedItemId == "(W)47")
            debrisList.Add(debri);
        }
      }
      foreach (Debris debris in debrisList)
        l.debris.Remove(debris);
      return true;
    }));
    Utility.iterateChestsAndStorage((Action<Item>) (item => Game1.checkIsMissingTool(missingTools, ref missingScythes, item)));
    List<string> stringList = new List<string>();
    foreach (KeyValuePair<Type, int> keyValuePair in missingTools)
    {
      if (keyValuePair.Value > 0)
      {
        for (int index = 0; index < keyValuePair.Value; ++index)
          stringList.Add(keyValuePair.Key.ToString());
      }
    }
    for (int index = 0; index < missingScythes; ++index)
      stringList.Add("Scythe");
    if (stringList.Count > 0)
      Game1.addMailForTomorrow("foundLostTools");
    for (int index = 0; index < stringList.Count; ++index)
    {
      Item obj = (Item) null;
      switch (stringList[index])
      {
        case "StardewValley.Tools.Axe":
          obj = ItemRegistry.Create("(T)Axe");
          break;
        case "StardewValley.Tools.Hoe":
          obj = ItemRegistry.Create("(T)Hoe");
          break;
        case "StardewValley.Tools.WateringCan":
          obj = ItemRegistry.Create("(T)WateringCan");
          break;
        case "Scythe":
          obj = ItemRegistry.Create("(W)47");
          break;
        case "StardewValley.Tools.Pickaxe":
          obj = ItemRegistry.Create("(T)Pickaxe");
          break;
        case "StardewValley.Tools.Wand":
          obj = ItemRegistry.Create("(T)ReturnScepter");
          break;
      }
      if (obj != null)
      {
        if (Game1.newDaySync.hasInstance())
          Game1.player.team.newLostAndFoundItems.Value = true;
        Game1.player.team.returnedDonations.Add(obj);
      }
    }
  }

  private static void checkIsMissingTool(
    Dictionary<Type, int> missingTools,
    ref int missingScythes,
    Item item)
  {
    foreach (Type key in missingTools.Keys)
    {
      if (item.GetType() == key)
        missingTools[key]--;
    }
    if (!(item.QualifiedItemId == "(W)47"))
      return;
    --missingScythes;
  }

  public static void newDayAfterFade(Action after)
  {
    if (Game1.player.currentLocation != null)
    {
      if (Game1.player.rightRing.Value != null)
        Game1.player.rightRing.Value.onLeaveLocation(Game1.player, Game1.player.currentLocation);
      if (Game1.player.leftRing.Value != null)
        Game1.player.leftRing.Value.onLeaveLocation(Game1.player, Game1.player.currentLocation);
    }
    if (LocalMultiplayer.IsLocalMultiplayer())
      Game1.hooks.OnGame1_NewDayAfterFade((Action) (() =>
      {
        Game1.game1.isLocalMultiplayerNewDayActive = true;
        Game1._afterNewDayAction = after;
        GameRunner.instance.activeNewDayProcesses.Add(new KeyValuePair<Game1, IEnumerator<int>>(Game1.game1, Game1._newDayAfterFade()));
      }));
    else
      Game1.hooks.OnGame1_NewDayAfterFade((Action) (() =>
      {
        Game1._afterNewDayAction = after;
        if (Game1._newDayTask != null)
          Game1.log.Warn("Warning: There is already a _newDayTask; unusual code path.\n" + StackTraceHelper.StackTrace);
        else
          Game1._newDayTask = new Task((Action) (() =>
          {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            IEnumerator<int> enumerator = Game1._newDayAfterFade();
            do
              ;
            while (enumerator.MoveNext());
          }));
      }));
  }

  public static bool CanAcceptDailyQuest()
  {
    return Game1.questOfTheDay != null && !Game1.player.acceptedDailyQuest.Value && !string.IsNullOrEmpty(Game1.questOfTheDay.questDescription);
  }

  private static IEnumerator<int> _newDayAfterFade()
  {
    TriggerActionManager.Raise("DayEnding");
    Game1.newDaySync.start();
    while (!Game1.newDaySync.hasStarted())
      yield return 0;
    int timeWentToSleep = Game1.timeOfDay;
    Game1.newDaySync.barrier("start");
    while (!Game1.newDaySync.isBarrierReady("start"))
      yield return 0;
    int overnightMinutesElapsed = Utility.CalculateMinutesUntilMorning(timeWentToSleep);
    Game1.stats.AverageBedtime = (uint) timeWentToSleep;
    if (Game1.IsMasterGame)
    {
      ++Game1.dayOfMonth;
      ++Game1.stats.DaysPlayed;
      if (Game1.dayOfMonth > 28)
      {
        Game1.dayOfMonth = 1;
        switch (Game1.season)
        {
          case Season.Spring:
            Game1.season = Season.Summer;
            break;
          case Season.Summer:
            Game1.season = Season.Fall;
            break;
          case Season.Fall:
            Game1.season = Season.Winter;
            break;
          case Season.Winter:
            Game1.season = Season.Spring;
            ++Game1.year;
            MineShaft.yearUpdate();
            break;
        }
      }
      Game1.timeOfDay = 600;
      Game1.netWorldState.Value.UpdateFromGame1();
    }
    Game1.newDaySync.barrier("date");
    while (!Game1.newDaySync.isBarrierReady("date"))
      yield return 0;
    Game1.player.dayOfMonthForSaveGame = new int?(Game1.dayOfMonth);
    Game1.player.seasonForSaveGame = new int?(Game1.seasonIndex);
    Game1.player.yearForSaveGame = new int?(Game1.year);
    Game1.flushLocationLookup();
    Event.OnNewDay();
    try
    {
      Game1.fixProblems();
    }
    catch (Exception ex)
    {
    }
    foreach (Farmer allFarmer in Game1.getAllFarmers())
      allFarmer.FarmerSprite.PauseForSingleAnimation = false;
    Game1.whereIsTodaysFest = (string) null;
    if (Game1.wind != null)
    {
      Game1.wind.Stop(AudioStopOptions.Immediate);
      Game1.wind = (ICue) null;
    }
    Game1.player.chestConsumedMineLevels.RemoveWhere((Func<KeyValuePair<int, bool>, bool>) (pair => pair.Key > 120));
    Game1.player.currentEyes = 0;
    int seedA;
    if (Game1.IsMasterGame)
    {
      Game1.player.team.announcedSleepingFarmers.Clear();
      seedA = Utility.CreateRandomSeed((double) (Game1.uniqueIDForThisGame / 100UL), (double) (uint) ((int) Game1.stats.DaysPlayed * 10 + 1), (double) Game1.stats.StepsTaken);
      Game1.newDaySync.sendVar<NetInt, int>("seed", seedA);
    }
    else
    {
      while (!Game1.newDaySync.isVarReady("seed"))
        yield return 0;
      seedA = Game1.newDaySync.waitForVar<NetInt, int>("seed");
    }
    Game1.random = Utility.CreateRandom((double) seedA);
    for (int index = 0; index < Game1.dayOfMonth; ++index)
      Game1.random.Next();
    Game1.player.team.endOfNightStatus.UpdateState("sleep");
    Game1.newDaySync.barrier("sleep");
    while (!Game1.newDaySync.isBarrierReady("sleep"))
      yield return 0;
    Game1.gameTimeInterval = 0;
    Game1.game1.wasAskedLeoMemory = false;
    Game1.player.team.Update();
    Game1.player.team.NewDay();
    Game1.player.passedOut = false;
    Game1.player.CanMove = true;
    Game1.player.FarmerSprite.PauseForSingleAnimation = false;
    Game1.player.FarmerSprite.StopAnimation();
    Game1.player.completelyStopAnimatingOrDoingAction();
    Game1.changeMusicTrack("silence");
    if (Game1.IsMasterGame)
      Game1.UpdateDishOfTheDay();
    Game1.newDaySync.barrier("dishOfTheDay");
    while (!Game1.newDaySync.isBarrierReady("dishOfTheDay"))
      yield return 0;
    Game1.npcDialogues = (Dictionary<string, Stack<Dialogue>>) null;
    Utility.ForEachCharacter((Func<NPC, bool>) (n =>
    {
      n.updatedDialogueYet = false;
      return true;
    }));
    Utility.ForEachLocation((Func<GameLocation, bool>) (location =>
    {
      location.currentEvent = (Event) null;
      if (Game1.IsMasterGame)
        location.passTimeForObjects(overnightMinutesElapsed);
      return true;
    }));
    Game1.outdoorLight = Color.White;
    Game1.ambientLight = Color.White;
    if (Game1.isLightning && Game1.IsMasterGame)
      Utility.overnightLightning(timeWentToSleep);
    if (Game1.MasterPlayer.hasOrWillReceiveMail("ccBulletinThankYou") && !Game1.player.hasOrWillReceiveMail("ccBulletinThankYou"))
      Game1.addMailForTomorrow("ccBulletinThankYou");
    Game1.ReceiveMailForTomorrow();
    string key1;
    Friendship friendship1;
    if (Utility.TryGetRandom<string, Friendship, NetRef<Friendship>, SerializableDictionary<string, Friendship>, NetStringDictionary<Friendship, NetRef<Friendship>>>((NetDictionary<string, Friendship, NetRef<Friendship>, SerializableDictionary<string, Friendship>, NetStringDictionary<Friendship, NetRef<Friendship>>>) Game1.player.friendshipData, out key1, out friendship1) && Game1.random.NextBool((double) (friendship1.Points / 250) * 0.1) && Game1.player.spouse != key1 && DataLoader.Mail(Game1.content).ContainsKey(key1))
      Game1.mailbox.Add(key1);
    MineShaft.clearActiveMines();
    VolcanoDungeon.ClearAllLevels();
    Game1.netWorldState.Value.CheckedGarbage.Clear();
    for (int index = Game1.player.enchantments.Count - 1; index >= 0; --index)
      Game1.player.enchantments[index].OnUnequip(Game1.player);
    Game1.player.dayupdate(timeWentToSleep);
    if (Game1.IsMasterGame)
      Game1.player.team.sharedDailyLuck.Value = Math.Min(0.10000000149011612, (double) Game1.random.Next(-100, 101) / 1000.0);
    Game1.player.showToolUpgradeAvailability();
    if (Game1.IsMasterGame)
    {
      Game1.queueWeddingsForToday();
      Game1.newDaySync.sendVar<NetRef<NetLongList>, NetLongList>("weddingsToday", new NetLongList((IEnumerable<long>) Game1.weddingsToday));
    }
    else
    {
      while (!Game1.newDaySync.isVarReady("weddingsToday"))
        yield return 0;
      Game1.weddingsToday = new List<long>((IEnumerable<long>) Game1.newDaySync.waitForVar<NetRef<NetLongList>, NetLongList>("weddingsToday"));
    }
    Game1.weddingToday = false;
    foreach (long id in Game1.weddingsToday)
    {
      Farmer player = Game1.GetPlayer(id);
      if (player != null && !player.hasCurrentOrPendingRoommate())
      {
        Game1.weddingToday = true;
        break;
      }
    }
    if (Game1.player.spouse != null && Game1.player.isEngaged() && Game1.weddingsToday.Contains(Game1.player.UniqueMultiplayerID))
    {
      Friendship friendship2 = Game1.player.friendshipData[Game1.player.spouse];
      friendship2.Status = FriendshipStatus.Married;
      friendship2.WeddingDate = new WorldDate(Game1.Date);
      Game1.prepareSpouseForWedding(Game1.player);
      if (!Game1.player.getSpouse().isRoommate())
      {
        Game1.player.autoGenerateActiveDialogueEvent("married_" + Game1.player.spouse);
        if (!Game1.player.autoGenerateActiveDialogueEvent("married"))
          Game1.player.autoGenerateActiveDialogueEvent("married_twice");
      }
      else
        Game1.player.autoGenerateActiveDialogueEvent("roommates_" + Game1.player.spouse);
    }
    NetLongDictionary<NetList<Item, NetRef<Item>>, NetRef<NetList<Item, NetRef<Item>>>> additional_shipped_items = new NetLongDictionary<NetList<Item, NetRef<Item>>, NetRef<NetList<Item, NetRef<Item>>>>();
    if (Game1.IsMasterGame)
      Utility.ForEachLocation((Func<GameLocation, bool>) (location =>
      {
        foreach (Object @object in location.objects.Values)
        {
          if (@object is Chest chest2 && chest2.SpecialChestType == Chest.SpecialChestTypes.MiniShippingBin)
          {
            chest2.clearNulls();
            if (Game1.player.team.useSeparateWallets.Value)
            {
              foreach (long key2 in chest2.separateWalletItems.Keys)
              {
                if (!additional_shipped_items.ContainsKey(key2))
                  additional_shipped_items[key2] = new NetList<Item, NetRef<Item>>();
                List<Item> objList = new List<Item>((IEnumerable<Item>) chest2.separateWalletItems[key2]);
                chest2.separateWalletItems[key2].Clear();
                foreach (Item obj in objList)
                {
                  obj.onDetachedFromParent();
                  additional_shipped_items[key2].Add(obj);
                }
              }
            }
            else
            {
              IInventory shippingBin = Game1.getFarm().getShippingBin(Game1.player);
              shippingBin.RemoveEmptySlots();
              foreach (Item obj in chest2.Items)
              {
                obj.onDetachedFromParent();
                shippingBin.Add(obj);
              }
            }
            chest2.Items.Clear();
            chest2.separateWalletItems.Clear();
          }
        }
        return true;
      }));
    if (Game1.IsMasterGame)
    {
      Game1.newDaySync.sendVar<NetRef<NetLongDictionary<NetList<Item, NetRef<Item>>, NetRef<NetList<Item, NetRef<Item>>>>>, NetLongDictionary<NetList<Item, NetRef<Item>>, NetRef<NetList<Item, NetRef<Item>>>>>("additional_shipped_items", additional_shipped_items);
    }
    else
    {
      while (!Game1.newDaySync.isVarReady("additional_shipped_items"))
        yield return 0;
      additional_shipped_items = Game1.newDaySync.waitForVar<NetRef<NetLongDictionary<NetList<Item, NetRef<Item>>, NetRef<NetList<Item, NetRef<Item>>>>>, NetLongDictionary<NetList<Item, NetRef<Item>>, NetRef<NetList<Item, NetRef<Item>>>>>("additional_shipped_items");
    }
    if (Game1.player.team.useSeparateWallets.Value)
    {
      IInventory shippingBin = Game1.getFarm().getShippingBin(Game1.player);
      NetList<Item, NetRef<Item>> netList;
      if (additional_shipped_items.TryGetValue(Game1.player.UniqueMultiplayerID, out netList))
      {
        foreach (Item obj in netList)
          shippingBin.Add(obj);
      }
    }
    Game1.newDaySync.barrier("handleMiniShippingBins");
    while (!Game1.newDaySync.isBarrierReady("handleMiniShippingBins"))
      yield return 0;
    IInventory shippingBin1 = Game1.getFarm().getShippingBin(Game1.player);
    shippingBin1.RemoveEmptySlots();
    foreach (Item obj in (IEnumerable<Item>) shippingBin1)
      Game1.player.displayedShippedItems.Add(obj);
    if (Game1.player.useSeparateWallets || Game1.player.IsMainPlayer)
    {
      int num1 = 0;
      foreach (Item obj in (IEnumerable<Item>) shippingBin1)
      {
        int num2 = 0;
        if (obj is Object @object)
        {
          num2 = @object.sellToStorePrice(-1L) * @object.Stack;
          num1 += num2;
        }
        if (Game1.player.team.specialOrders != null)
        {
          foreach (SpecialOrder specialOrder in Game1.player.team.specialOrders)
          {
            Action<Farmer, Item, int> onItemShipped = specialOrder.onItemShipped;
            if (onItemShipped != null)
              onItemShipped(Game1.player, obj, num2);
          }
        }
      }
      Game1.player.Money += num1;
    }
    if (Game1.IsMasterGame)
    {
      if (Game1.IsWinter && Game1.dayOfMonth == 18)
      {
        GameLocation source1 = Game1.RequireLocation("Submarine");
        if (source1.objects.Length >= 0)
          Utility.transferPlacedObjectsFromOneLocationToAnother(source1, (GameLocation) null, new Vector2?(new Vector2(20f, 20f)), Game1.getLocationFromName("Beach"));
        GameLocation source2 = Game1.RequireLocation("MermaidHouse");
        if (source2.objects.Length >= 0)
          Utility.transferPlacedObjectsFromOneLocationToAnother(source2, (GameLocation) null, new Vector2?(new Vector2(21f, 20f)), Game1.getLocationFromName("Beach"));
      }
      if (Game1.player.hasOrWillReceiveMail("pamHouseUpgrade") && !Game1.player.hasOrWillReceiveMail("transferredObjectsPamHouse"))
      {
        Game1.addMailForTomorrow("transferredObjectsPamHouse", true);
        GameLocation source = Game1.RequireLocation("Trailer");
        GameLocation locationFromName = Game1.getLocationFromName("Trailer_Big");
        if (source.objects.Length >= 0)
          Utility.transferPlacedObjectsFromOneLocationToAnother(source, locationFromName, new Vector2?(new Vector2(14f, 23f)));
      }
      if (Utility.HasAnyPlayerSeenEvent("191393") && !Game1.player.hasOrWillReceiveMail("transferredObjectsJojaMart"))
      {
        Game1.addMailForTomorrow("transferredObjectsJojaMart", true);
        GameLocation source = Game1.RequireLocation("JojaMart");
        if (source.objects.Length >= 0)
          Utility.transferPlacedObjectsFromOneLocationToAnother(source, (GameLocation) null, new Vector2?(new Vector2(89f, 51f)), Game1.getLocationFromName("Town"));
      }
    }
    if (Game1.player.useSeparateWallets && Game1.player.IsMainPlayer)
    {
      foreach (Farmer offlineFarmhand in Game1.getOfflineFarmhands())
      {
        if (!offlineFarmhand.isUnclaimedFarmhand)
        {
          int num3 = 0;
          IInventory shippingBin2 = Game1.getFarm().getShippingBin(offlineFarmhand);
          shippingBin2.RemoveEmptySlots();
          foreach (Item obj in (IEnumerable<Item>) shippingBin2)
          {
            int num4 = 0;
            if (obj is Object @object)
            {
              num4 = @object.sellToStorePrice(offlineFarmhand.UniqueMultiplayerID) * @object.Stack;
              num3 += num4;
            }
            if (Game1.player.team.specialOrders != null)
            {
              foreach (SpecialOrder specialOrder in Game1.player.team.specialOrders)
              {
                Action<Farmer, Item, int> onItemShipped = specialOrder.onItemShipped;
                if (onItemShipped != null)
                  onItemShipped(Game1.player, obj, num4);
              }
            }
          }
          Game1.player.team.AddIndividualMoney(offlineFarmhand, num3);
          shippingBin2.Clear();
        }
      }
    }
    List<NPC> divorceNPCs = new List<NPC>();
    if (Game1.IsMasterGame)
    {
      foreach (Farmer allFarmer in Game1.getAllFarmers())
      {
        if (allFarmer.isActive() && allFarmer.divorceTonight.Value && allFarmer.getSpouse() != null)
          divorceNPCs.Add(allFarmer.getSpouse());
      }
    }
    Game1.newDaySync.barrier("player.dayupdate");
    while (!Game1.newDaySync.isBarrierReady("player.dayupdate"))
      yield return 0;
    if (Game1.player.divorceTonight.Value)
      Game1.player.doDivorce();
    Game1.newDaySync.barrier("player.divorce");
    while (!Game1.newDaySync.isBarrierReady("player.divorce"))
      yield return 0;
    if (Game1.IsMasterGame)
    {
      foreach (NPC npc in divorceNPCs)
      {
        if (npc.getSpouse() == null)
          npc.PerformDivorce();
      }
    }
    Game1.newDaySync.barrier("player.finishDivorce");
    while (!Game1.newDaySync.isBarrierReady("player.finishDivorce"))
      yield return 0;
    if (Game1.IsMasterGame)
      Utility.ForEachBuilding((Func<Building, bool>) (building =>
      {
        if (building?.GetIndoors() is Cabin indoors2)
          indoors2.updateFarmLayout();
        return true;
      }));
    Game1.newDaySync.barrier("updateFarmLayout");
    while (!Game1.newDaySync.isBarrierReady("updateFarmLayout"))
      yield return 0;
    if (Game1.IsMasterGame && Game1.player.changeWalletTypeTonight.Value)
    {
      if (Game1.player.useSeparateWallets)
        ManorHouse.MergeWallets();
      else
        ManorHouse.SeparateWallets();
    }
    Game1.newDaySync.barrier("player.wallets");
    while (!Game1.newDaySync.isBarrierReady("player.wallets"))
      yield return 0;
    Game1.getFarm().lastItemShipped = (Item) null;
    Game1.getFarm().getShippingBin(Game1.player).Clear();
    Game1.newDaySync.barrier("clearShipping");
    while (!Game1.newDaySync.isBarrierReady("clearShipping"))
      yield return 0;
    if (Game1.IsClient)
    {
      Game1.multiplayer.sendFarmhand();
      Game1.newDaySync.processMessages();
    }
    Game1.newDaySync.barrier("sendFarmhands");
    while (!Game1.newDaySync.isBarrierReady("sendFarmhands"))
      yield return 0;
    if (Game1.IsMasterGame)
      Game1.multiplayer.saveFarmhands();
    Game1.newDaySync.barrier("saveFarmhands");
    while (!Game1.newDaySync.isBarrierReady("saveFarmhands"))
      yield return 0;
    if (Game1.IsMasterGame)
    {
      Game1.UpdatePassiveFestivalStates();
      if (Utility.IsPassiveFestivalDay("NightMarket") && Game1.IsMasterGame && Game1.netWorldState.Value.VisitsUntilY1Guarantee >= 0)
        --Game1.netWorldState.Value.VisitsUntilY1Guarantee;
    }
    if (Game1.dayOfMonth == 1)
      Game1.OnNewSeason();
    if (Game1.IsMasterGame && (Game1.dayOfMonth == 1 || Game1.dayOfMonth == 8 || Game1.dayOfMonth == 15 || Game1.dayOfMonth == 22))
    {
      SpecialOrder.UpdateAvailableSpecialOrders("", true);
      SpecialOrder.UpdateAvailableSpecialOrders("Qi", true);
    }
    if (Game1.IsMasterGame)
      Game1.netWorldState.Value.UpdateFromGame1();
    Game1.newDaySync.barrier("specialOrders");
    while (!Game1.newDaySync.isBarrierReady("specialOrders"))
      yield return 0;
    if (Game1.IsMasterGame)
      Game1.player.team.specialOrders.RemoveWhere((Func<SpecialOrder, bool>) (order =>
      {
        if (order.questState.Value == SpecialOrderStatus.Complete || order.GetDaysLeft() > 0)
          return false;
        order.OnFail();
        return true;
      }));
    Game1.newDaySync.barrier("processOrders");
    while (!Game1.newDaySync.isBarrierReady("processOrders"))
      yield return 0;
    if (Game1.IsMasterGame)
    {
      foreach (string rule in Game1.player.team.specialRulesRemovedToday)
        SpecialOrder.RemoveSpecialRuleAtEndOfDay(rule);
    }
    Game1.player.team.specialRulesRemovedToday.Clear();
    if (DataLoader.Mail(Game1.content).ContainsKey($"{Game1.currentSeason}_{Game1.dayOfMonth.ToString()}_{Game1.year.ToString()}"))
      Game1.mailbox.Add($"{Game1.currentSeason}_{Game1.dayOfMonth.ToString()}_{Game1.year.ToString()}");
    else if (DataLoader.Mail(Game1.content).ContainsKey($"{Game1.currentSeason}_{Game1.dayOfMonth.ToString()}"))
      Game1.mailbox.Add($"{Game1.currentSeason}_{Game1.dayOfMonth.ToString()}");
    if (Game1.MasterPlayer.mailReceived.Contains("ccVault") && Game1.IsSpring && Game1.dayOfMonth == 14)
      Game1.mailbox.Add("DesertFestival");
    if (Game1.IsMasterGame)
    {
      if (Game1.player.team.toggleMineShrineOvernight.Value)
      {
        Game1.player.team.toggleMineShrineOvernight.Value = false;
        Game1.player.team.mineShrineActivated.Value = !Game1.player.team.mineShrineActivated.Value;
        if (Game1.player.team.mineShrineActivated.Value)
          ++Game1.netWorldState.Value.MinesDifficulty;
        else
          --Game1.netWorldState.Value.MinesDifficulty;
      }
      if (Game1.player.team.toggleSkullShrineOvernight.Value)
      {
        Game1.player.team.toggleSkullShrineOvernight.Value = false;
        Game1.player.team.skullShrineActivated.Value = !Game1.player.team.skullShrineActivated.Value;
        if (Game1.player.team.skullShrineActivated.Value)
          ++Game1.netWorldState.Value.SkullCavesDifficulty;
        else
          --Game1.netWorldState.Value.SkullCavesDifficulty;
      }
    }
    if (Game1.IsMasterGame)
    {
      if (!Game1.player.team.SpecialOrderRuleActive("MINE_HARD") && Game1.netWorldState.Value.MinesDifficulty > 1)
        Game1.netWorldState.Value.MinesDifficulty = 1;
      if (!Game1.player.team.SpecialOrderRuleActive("SC_HARD") && Game1.netWorldState.Value.SkullCavesDifficulty > 1)
        Game1.netWorldState.Value.SkullCavesDifficulty = 1;
    }
    if (Game1.IsMasterGame)
      Game1.RefreshQuestOfTheDay();
    Game1.newDaySync.barrier("questOfTheDay");
    while (!Game1.newDaySync.isBarrierReady("questOfTheDay"))
      yield return 0;
    bool yesterdayWasGreenRain = Game1.wasGreenRain;
    Game1.wasGreenRain = false;
    Game1.UpdateWeatherForNewDay();
    Game1.newDaySync.barrier("updateWeather");
    while (!Game1.newDaySync.isBarrierReady("updateWeather"))
      yield return 0;
    Game1.ApplyWeatherForNewDay();
    if (Game1.isGreenRain)
    {
      Game1.morningQueue.Enqueue((Action) (() => Game1.showGlobalMessage(Game1.content.LoadString("Strings\\1_6_Strings:greenrainmessage"))));
      if (Game1.year == 1 && !Game1.player.hasOrWillReceiveMail("GreenRainGus"))
        Game1.mailbox.Add("GreenRainGus");
      if (Game1.IsMasterGame)
        Utility.ForEachLocation((Func<GameLocation, bool>) (location =>
        {
          location.performGreenRainUpdate();
          return true;
        }));
    }
    else if (yesterdayWasGreenRain)
    {
      if (Game1.IsMasterGame)
        Utility.ForEachLocation((Func<GameLocation, bool>) (location =>
        {
          location.performDayAfterGreenRainUpdate();
          return true;
        }));
      if (Game1.year == 1)
        Game1.player.activeDialogueEvents.TryAdd("GreenRainFinished", 1);
    }
    if (Utility.getDaysOfBooksellerThisSeason().Contains(Game1.dayOfMonth))
      Game1.addMorningFluffFunction((Action) (() => Game1.showGlobalMessage(Game1.content.LoadString("Strings\\1_6_Strings:BooksellerInTown"))));
    WeatherDebris.globalWind = 0.0f;
    Game1.windGust = 0.0f;
    Game1.AddNPCs();
    Utility.ForEachVillager((Func<NPC, bool>) (n =>
    {
      Game1.player.mailReceived.Remove(n.Name);
      Game1.player.mailReceived.Remove(n.Name + "Cooking");
      n.drawOffset = Vector2.Zero;
      if (!Game1.IsMasterGame)
        n.ChooseAppearance();
      return true;
    }));
    FarmAnimal.reservedGrass.Clear();
    if (Game1.IsMasterGame)
    {
      int num;
      NPC.hasSomeoneRepairedTheFences = (num = 0) != 0;
      NPC.hasSomeoneFedTheAnimals = num != 0;
      NPC.hasSomeoneFedThePet = num != 0;
      NPC.hasSomeoneWateredCrops = num != 0;
      foreach (GameLocation location in (IEnumerable<GameLocation>) Game1.locations)
      {
        location.ResetCharacterDialogues();
        location.DayUpdate(Game1.dayOfMonth);
      }
      Game1.netWorldState.Value.UpdateUnderConstruction();
      Game1.UpdateHorseOwnership();
      foreach (NPC allCharacter in Utility.getAllCharacters())
      {
        if (allCharacter.IsVillager)
        {
          allCharacter.islandScheduleName.Value = (string) null;
          allCharacter.currentScheduleDelay = 0.0f;
        }
        allCharacter.dayUpdate(Game1.dayOfMonth);
      }
      IslandSouth.SetupIslandSchedules();
      HashSet<NPC> purchased_item_npcs = new HashSet<NPC>();
      Game1.UpdateShopPlayerItemInventory("SeedShop", purchased_item_npcs);
      Game1.UpdateShopPlayerItemInventory("FishShop", purchased_item_npcs);
    }
    if (Game1.IsMasterGame && Game1.netWorldState.Value.GetWeatherForLocation("Island").IsRaining)
    {
      Vector2 tile_position = new Vector2(0.0f, 0.0f);
      IslandLocation islandLocation = (IslandLocation) null;
      List<int> list = new List<int>();
      for (int index = 0; index < 4; ++index)
        list.Add(index);
      Utility.Shuffle<int>(Utility.CreateRandom((double) Game1.uniqueIDForThisGame), list);
      switch (list[Game1.currentGemBirdIndex])
      {
        case 0:
          islandLocation = Game1.getLocationFromName("IslandSouth") as IslandLocation;
          tile_position = new Vector2(10f, 30f);
          break;
        case 1:
          islandLocation = Game1.getLocationFromName("IslandNorth") as IslandLocation;
          tile_position = new Vector2(56f, 56f);
          break;
        case 2:
          islandLocation = Game1.getLocationFromName("Islandwest") as IslandLocation;
          tile_position = new Vector2(53f, 51f);
          break;
        case 3:
          islandLocation = Game1.getLocationFromName("IslandEast") as IslandLocation;
          tile_position = new Vector2(21f, 35f);
          break;
      }
      Game1.currentGemBirdIndex = (Game1.currentGemBirdIndex + 1) % 4;
      if (islandLocation != null)
        islandLocation.locationGemBird.Value = new IslandGemBird(tile_position, IslandGemBird.GetBirdTypeForLocation(islandLocation.Name));
    }
    if (Game1.IsMasterGame)
      Utility.ForEachLocation((Func<GameLocation, bool>) (location =>
      {
        if (location.IsOutdoors && location.IsRainingHere())
        {
          foreach (Building building in location.buildings)
          {
            if (building is PetBowl petBowl2)
              petBowl2.watered.Value = true;
          }
          foreach (KeyValuePair<Vector2, TerrainFeature> pair in location.terrainFeatures.Pairs)
          {
            if (pair.Value is HoeDirt hoeDirt2 && hoeDirt2.state.Value != 2)
              hoeDirt2.state.Value = 1;
          }
        }
        return true;
      }));
    WorldDate worldDate = new WorldDate(Game1.Date);
    --worldDate.TotalDays;
    foreach (KeyValuePair<string, PassiveFestivalData> passiveFestival in DataLoader.PassiveFestivals(Game1.content))
    {
      string key3 = passiveFestival.Key;
      PassiveFestivalData passiveFestivalData = passiveFestival.Value;
      if (worldDate.DayOfMonth == passiveFestivalData.EndDay && worldDate.Season == passiveFestivalData.Season && GameStateQuery.CheckConditions(passiveFestivalData.Condition) && passiveFestivalData != null && passiveFestivalData.CleanupMethod != null)
      {
        FestivalCleanupDelegate createdDelegate;
        string error;
        if (StaticDelegateBuilder.TryCreateDelegate<FestivalCleanupDelegate>(passiveFestivalData.CleanupMethod, out createdDelegate, out error))
          createdDelegate();
        else
          Game1.log.Warn($"Passive festival '{key3}' has invalid cleanup method '{passiveFestivalData.CleanupMethod}': {error}");
      }
    }
    Game1.PerformPassiveFestivalSetup();
    Game1.newDaySync.barrier("buildingUpgrades");
    while (!Game1.newDaySync.isBarrierReady("buildingUpgrades"))
      yield return 0;
    List<string> stringList = new List<string>((IEnumerable<string>) Game1.player.team.mailToRemoveOvernight);
    foreach (string itemId in new List<string>((IEnumerable<string>) Game1.player.team.itemsToRemoveOvernight))
    {
      if (Game1.IsMasterGame)
      {
        Game1.game1._PerformRemoveNormalItemFromWorldOvernight(itemId);
        foreach (Farmer offlineFarmhand in Game1.getOfflineFarmhands())
          Game1.game1._PerformRemoveNormalItemFromFarmerOvernight(offlineFarmhand, itemId);
      }
      Game1.game1._PerformRemoveNormalItemFromFarmerOvernight(Game1.player, itemId);
    }
    foreach (string mail_key in stringList)
    {
      if (Game1.IsMasterGame)
      {
        foreach (Farmer allFarmer in Game1.getAllFarmers())
          allFarmer.RemoveMail(mail_key, allFarmer == Game1.MasterPlayer);
      }
      else
        Game1.player.RemoveMail(mail_key);
    }
    Game1.newDaySync.barrier("removeItemsFromWorld");
    while (!Game1.newDaySync.isBarrierReady("removeItemsFromWorld"))
      yield return 0;
    if (Game1.IsMasterGame)
    {
      Game1.player.team.itemsToRemoveOvernight.Clear();
      Game1.player.team.mailToRemoveOvernight.Clear();
    }
    Game1.newDay = false;
    if (Game1.IsMasterGame)
      Game1.netWorldState.Value.UpdateFromGame1();
    if (Game1.player.currentLocation != null)
    {
      Game1.player.currentLocation.resetForPlayerEntry();
      BedFurniture.ApplyWakeUpPosition(Game1.player);
      Game1.forceSnapOnNextViewportUpdate = true;
      Game1.UpdateViewPort(false, Game1.player.StandingPixel);
      Game1.previousViewportPosition = new Vector2((float) Game1.viewport.X, (float) Game1.viewport.Y);
    }
    Game1.displayFarmer = true;
    Game1.updateWeatherIcon();
    Game1.freezeControls = false;
    if (Game1.stats.DaysPlayed > 1U || !Game1.IsMasterGame)
    {
      Game1.farmEvent = (FarmEvent) null;
      if (Game1.IsMasterGame)
      {
        Game1.farmEvent = Utility.pickFarmEvent() ?? Game1.farmEventOverride;
        Game1.farmEventOverride = (FarmEvent) null;
        Game1.newDaySync.sendVar<NetRef<FarmEvent>, FarmEvent>("farmEvent", Game1.farmEvent);
      }
      else
      {
        while (!Game1.newDaySync.isVarReady("farmEvent"))
          yield return 0;
        Game1.farmEvent = Game1.newDaySync.waitForVar<NetRef<FarmEvent>, FarmEvent>("farmEvent");
      }
      if (Game1.farmEvent == null)
        Game1.farmEvent = Utility.pickPersonalFarmEvent();
      if (Game1.farmEvent != null && Game1.farmEvent.setUp())
        Game1.farmEvent = (FarmEvent) null;
    }
    if (Game1.farmEvent == null)
      Game1.RemoveDeliveredMailForTomorrow();
    if (Game1.player.team.newLostAndFoundItems.Value)
      Game1.morningQueue.Enqueue((Action) (() => Game1.showGlobalMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:NewLostAndFoundItems"))));
    Game1.newDaySync.barrier("mail");
    while (!Game1.newDaySync.isBarrierReady("mail"))
      yield return 0;
    if (Game1.IsMasterGame)
      Game1.player.team.newLostAndFoundItems.Value = false;
    Utility.ForEachBuilding((Func<Building, bool>) (building =>
    {
      if (!(building.GetIndoors() is Cabin))
        return true;
      Game1.player.slotCanHost = true;
      return false;
    }));
    if ((double) Utility.percentGameComplete() + (double) Game1.netWorldState.Value.PerfectionWaivers * 0.0099999997764825821 >= 1.0)
      Game1.player.team.farmPerfect.Value = true;
    Game1.newDaySync.barrier("checkcompletion");
    while (!Game1.newDaySync.isBarrierReady("checkcompletion"))
      yield return 0;
    Game1.UpdateFarmPerfection();
    if (Game1.farmEvent == null)
    {
      Game1.handlePostFarmEventActions();
      Game1.showEndOfNightStuff();
    }
    if (Game1.server != null)
      Game1.server.updateLobbyData();
    divorceNPCs = (List<NPC>) null;
  }

  /// <summary>Reset the Saloon's dish of the day.</summary>
  public static void UpdateDishOfTheDay()
  {
    string id;
    do
    {
      id = Game1.random.Next(194, 240 /*0xF0*/).ToString();
    }
    while (Utility.IsForbiddenDishOfTheDay(id));
    int amount = Game1.random.Next(1, 4 + (Game1.random.NextDouble() < 0.08 ? 10 : 0));
    Game1.dishOfTheDay = ItemRegistry.Create<Object>("(O)" + id, amount);
  }

  /// <summary>Apply updates overnight if this save has completed perfection.</summary>
  /// <remarks>See also <see cref="M:StardewValley.Utility.percentGameComplete" /> to check if the save has reached perfection.</remarks>
  public static void UpdateFarmPerfection()
  {
    if (Game1.MasterPlayer.mailReceived.Contains("Farm_Eternal") || !Game1.MasterPlayer.hasCompletedCommunityCenter() && !Utility.hasFinishedJojaRoute() || !Game1.player.team.farmPerfect.Value)
      return;
    Game1.addMorningFluffFunction((Action) (() =>
    {
      Game1.changeMusicTrack("none", true);
      if (Game1.IsMasterGame)
        Game1.multiplayer.globalChatInfoMessageEvenInSinglePlayer("Eternal1");
      Game1.playSound("discoverMineral");
      if (Game1.IsMasterGame)
        DelayedAction.functionAfterDelay((Action) (() => Game1.multiplayer.globalChatInfoMessageEvenInSinglePlayer("Eternal2", Game1.MasterPlayer.farmName.Value)), 4000);
      Game1.player.mailReceived.Add("Farm_Eternal");
      DelayedAction.functionAfterDelay((Action) (() =>
      {
        Game1.playSound("thunder_small");
        if (Game1.IsMultiplayer)
        {
          if (!Game1.IsMasterGame)
            return;
          Game1.multiplayer.globalChatInfoMessage("Eternal3");
        }
        else
          Game1.showGlobalMessage(Game1.content.LoadString("Strings\\UI:Chat_Eternal3"));
      }), 12000);
    }));
  }

  /// <summary>Get whether it's green raining in the given location's context (regardless of whether the player is currently indoors and sheltered from the green rain).</summary>
  /// <param name="location">The location to check, or <c>null</c> to use <see cref="P:StardewValley.Game1.currentLocation" />.</param>
  public static bool IsGreenRainingHere(GameLocation location = null)
  {
    if (location == null)
      location = Game1.currentLocation;
    return location != null && (NetFieldBase<NetWorldState, NetRef<NetWorldState>>) Game1.netWorldState != (NetRef<NetWorldState>) null && location.IsGreenRainingHere();
  }

  /// <summary>Get whether it's raining in the given location's context (regardless of whether the player is currently indoors and sheltered from the rain).</summary>
  /// <param name="location">The location to check, or <c>null</c> to use <see cref="P:StardewValley.Game1.currentLocation" />.</param>
  public static bool IsRainingHere(GameLocation location = null)
  {
    if (location == null)
      location = Game1.currentLocation;
    return location != null && (NetFieldBase<NetWorldState, NetRef<NetWorldState>>) Game1.netWorldState != (NetRef<NetWorldState>) null && location.IsRainingHere();
  }

  /// <summary>Get whether it's storming in the given location's context (regardless of whether the player is currently indoors and sheltered from the storm).</summary>
  /// <param name="location">The location to check, or <c>null</c> to use <see cref="P:StardewValley.Game1.currentLocation" />.</param>
  public static bool IsLightningHere(GameLocation location = null)
  {
    if (location == null)
      location = Game1.currentLocation;
    return location != null && (NetFieldBase<NetWorldState, NetRef<NetWorldState>>) Game1.netWorldState != (NetRef<NetWorldState>) null && location.IsLightningHere();
  }

  /// <summary>Get whether it's snowing in the given location's context (regardless of whether the player is currently indoors and sheltered from the snow).</summary>
  /// <param name="location">The location to check, or <c>null</c> to use <see cref="P:StardewValley.Game1.currentLocation" />.</param>
  public static bool IsSnowingHere(GameLocation location = null)
  {
    if (location == null)
      location = Game1.currentLocation;
    return location != null && (NetFieldBase<NetWorldState, NetRef<NetWorldState>>) Game1.netWorldState != (NetRef<NetWorldState>) null && location.IsSnowingHere();
  }

  /// <summary>Get whether it's blowing debris like leaves in the given location's context (regardless of whether the player is currently indoors and sheltered from the wind).</summary>
  /// <param name="location">The location to check, or <c>null</c> to use <see cref="P:StardewValley.Game1.currentLocation" />.</param>
  public static bool IsDebrisWeatherHere(GameLocation location = null)
  {
    if (location == null)
      location = Game1.currentLocation;
    return location != null && (NetFieldBase<NetWorldState, NetRef<NetWorldState>>) Game1.netWorldState != (NetRef<NetWorldState>) null && location.IsDebrisWeatherHere();
  }

  public static string getWeatherModificationsForDate(WorldDate date, string default_weather)
  {
    string modificationsForDate = default_weather;
    int num = date.TotalDays - Game1.Date.TotalDays;
    if (date.DayOfMonth == 1 || (long) Game1.stats.DaysPlayed + (long) num <= 4L)
      modificationsForDate = "Sun";
    if ((long) Game1.stats.DaysPlayed + (long) num == 3L)
      modificationsForDate = "Rain";
    if (Utility.isGreenRainDay(date.DayOfMonth, date.Season))
      modificationsForDate = "GreenRain";
    if (date.Season == Season.Summer && date.DayOfMonth % 13 == 0)
      modificationsForDate = "Storm";
    if (Utility.isFestivalDay(date.DayOfMonth, date.Season))
      modificationsForDate = "Festival";
    foreach (PassiveFestivalData passiveFestivalData in DataLoader.PassiveFestivals(Game1.content).Values)
    {
      if (date.DayOfMonth >= passiveFestivalData.StartDay && date.DayOfMonth <= passiveFestivalData.EndDay && date.Season == passiveFestivalData.Season && GameStateQuery.CheckConditions(passiveFestivalData.Condition) && passiveFestivalData.MapReplacements != null)
      {
        foreach (string key in passiveFestivalData.MapReplacements.Keys)
        {
          GameLocation locationFromName = Game1.getLocationFromName(key);
          if (locationFromName != null && locationFromName.InValleyContext())
          {
            modificationsForDate = "Sun";
            break;
          }
        }
      }
    }
    return modificationsForDate;
  }

  public static void UpdateWeatherForNewDay()
  {
    Game1.weatherForTomorrow = Game1.getWeatherModificationsForDate(Game1.Date, Game1.weatherForTomorrow);
    if (Game1.weddingToday)
      Game1.weatherForTomorrow = "Wedding";
    if (Game1.IsMasterGame)
      Game1.netWorldState.Value.GetWeatherForLocation("Default").WeatherForTomorrow = Game1.weatherForTomorrow;
    Game1.wasRainingYesterday = Game1.isRaining || Game1.isLightning;
    Game1.debrisWeather.Clear();
    if (!Game1.IsMasterGame)
      return;
    foreach (KeyValuePair<string, LocationContextData> keyValuePair in (IEnumerable<KeyValuePair<string, LocationContextData>>) Game1.locationContextData)
      Game1.netWorldState.Value.GetWeatherForLocation(keyValuePair.Key).UpdateDailyWeather(keyValuePair.Key, keyValuePair.Value, Game1.random);
    foreach (KeyValuePair<string, LocationContextData> keyValuePair in (IEnumerable<KeyValuePair<string, LocationContextData>>) Game1.locationContextData)
    {
      string weatherFromLocation = keyValuePair.Value.CopyWeatherFromLocation;
      if (weatherFromLocation != null)
      {
        try
        {
          Game1.netWorldState.Value.GetWeatherForLocation(keyValuePair.Key).CopyFrom(Game1.netWorldState.Value.GetWeatherForLocation(weatherFromLocation));
        }
        catch
        {
        }
      }
    }
  }

  public static void ApplyWeatherForNewDay()
  {
    LocationWeather weatherForLocation = Game1.netWorldState.Value.GetWeatherForLocation("Default");
    Game1.weatherForTomorrow = weatherForLocation.WeatherForTomorrow;
    Game1.isRaining = weatherForLocation.IsRaining;
    Game1.isSnowing = weatherForLocation.IsSnowing;
    Game1.isLightning = weatherForLocation.IsLightning;
    Game1.isDebrisWeather = weatherForLocation.IsDebrisWeather;
    Game1.isGreenRain = weatherForLocation.IsGreenRain;
    if (Game1.isDebrisWeather)
      Game1.populateDebrisWeatherArray();
    if (!Game1.IsMasterGame)
      return;
    foreach (string key in Game1.netWorldState.Value.LocationWeather.Keys)
    {
      LocationWeather locationWeather = Game1.netWorldState.Value.LocationWeather[key];
      if (Game1.dayOfMonth == 1)
        locationWeather.monthlyNonRainyDayCount.Value = 0;
      if (!locationWeather.IsRaining)
        ++locationWeather.monthlyNonRainyDayCount.Value;
    }
  }

  public static void UpdateShopPlayerItemInventory(
    string location_name,
    HashSet<NPC> purchased_item_npcs)
  {
    if (!(Game1.getLocationFromName(location_name) is ShopLocation locationFromName))
      return;
    NetObjectList<Item> fromPlayerToSell = locationFromName.itemsFromPlayerToSell;
    for (int index1 = fromPlayerToSell.Count - 1; index1 >= 0; --index1)
    {
      if (!(fromPlayerToSell[index1] is Object i))
      {
        fromPlayerToSell.RemoveAt(index1);
      }
      else
      {
        for (int index2 = 0; index2 < i.Stack; ++index2)
        {
          bool flag = false;
          if (i.edibility.Value != -300 && Game1.random.NextDouble() < 0.04)
          {
            NPC randomNpc = Utility.GetRandomNpc((Func<string, CharacterData, bool>) ((name, data) => data.CanCommentOnPurchasedShopItems ?? data.HomeRegion == "Town"));
            if (randomNpc.Age != 2 && randomNpc.getSpouse() == null)
            {
              if (!purchased_item_npcs.Contains(randomNpc))
              {
                Dialogue itemDialogueForNpc = locationFromName.getPurchasedItemDialogueForNPC(i, randomNpc);
                if (itemDialogueForNpc != null)
                {
                  randomNpc.addExtraDialogue(itemDialogueForNpc);
                  purchased_item_npcs.Add(randomNpc);
                }
              }
              fromPlayerToSell[index1] = i.ConsumeStack(1);
              flag = true;
            }
          }
          if (!flag && Game1.random.NextDouble() < 0.15)
            fromPlayerToSell[index1] = i.ConsumeStack(1);
          if (fromPlayerToSell[index1] == null)
          {
            fromPlayerToSell.RemoveAt(index1);
            break;
          }
        }
      }
    }
  }

  private static void handlePostFarmEventActions()
  {
    Utility.ForEachLocation((Func<GameLocation, bool>) (location =>
    {
      foreach (Action eventOvernightAction in location.postFarmEventOvernightActions)
        eventOvernightAction();
      location.postFarmEventOvernightActions.Clear();
      return true;
    }));
    if (!Game1.IsMasterGame)
      return;
    Mountain mountain = Game1.RequireLocation<Mountain>("Mountain");
    mountain.ApplyTreehouseIfNecessary();
    if (!mountain.treehouseDoorDirty)
      return;
    mountain.treehouseDoorDirty = false;
    WarpPathfindingCache.PopulateCache();
  }

  public static void ReceiveMailForTomorrow(string mail_to_transfer = null)
  {
    foreach (string str1 in (NetHashSet<string>) Game1.player.mailForTomorrow)
    {
      if (str1 != null)
      {
        string str2 = str1.Replace("%&NL&%", "");
        if (mail_to_transfer == null || !(mail_to_transfer != str1) || !(mail_to_transfer != str2))
        {
          Game1.mailDeliveredFromMailForTomorrow.Add(str1);
          if (str1.Contains("%&NL&%"))
            Game1.player.mailReceived.Add(str2);
          else
            Game1.mailbox.Add(str1);
        }
      }
    }
  }

  public static void RemoveDeliveredMailForTomorrow()
  {
    Game1.ReceiveMailForTomorrow("abandonedJojaMartAccessible");
    foreach (string str in Game1.mailDeliveredFromMailForTomorrow)
      Game1.player.mailForTomorrow.Remove(str);
    Game1.mailDeliveredFromMailForTomorrow.Clear();
  }

  public static void queueWeddingsForToday()
  {
    Game1.weddingsToday.Clear();
    Game1.weddingToday = false;
    if (!Game1.canHaveWeddingOnDay(Game1.dayOfMonth, Game1.season))
      return;
    foreach (Farmer farmer in (IEnumerable<Farmer>) Game1.getOnlineFarmers().OrderBy<Farmer, long>((Func<Farmer, long>) (farmer => farmer.UniqueMultiplayerID)))
    {
      if (farmer.spouse != null && farmer.isEngaged() && farmer.friendshipData[farmer.spouse].CountdownToWedding < 1)
        Game1.weddingsToday.Add(farmer.UniqueMultiplayerID);
      if (farmer.team.IsEngaged(farmer.UniqueMultiplayerID))
      {
        long? spouse = farmer.team.GetSpouse(farmer.UniqueMultiplayerID);
        if (spouse.HasValue && !Game1.weddingsToday.Contains(spouse.Value))
        {
          Farmer player = Game1.GetPlayer(spouse.Value);
          if (player != null && Game1.getOnlineFarmers().Contains(player) && Game1.getOnlineFarmers().Contains(farmer) && Game1.player.team.GetFriendship(farmer.UniqueMultiplayerID, spouse.Value).CountdownToWedding < 1)
            Game1.weddingsToday.Add(farmer.UniqueMultiplayerID);
        }
      }
    }
  }

  public static bool PollForEndOfNewDaySync()
  {
    if (!Game1.IsMultiplayer)
    {
      Game1.newDaySync.destroy();
      Game1.currentLocation.resetForPlayerEntry();
      return true;
    }
    if (Game1.newDaySync.readyForFinish())
    {
      if (Game1.IsMasterGame && Game1.newDaySync.hasInstance() && !Game1.newDaySync.hasFinished())
        Game1.newDaySync.finish();
      if (Game1.IsClient)
        Game1.player.sleptInTemporaryBed.Value = false;
      if (Game1.newDaySync.hasInstance() && Game1.newDaySync.hasFinished())
      {
        Game1.newDaySync.destroy();
        Game1.currentLocation.resetForPlayerEntry();
        return true;
      }
    }
    return false;
  }

  public static void updateWeatherIcon()
  {
    Game1.weatherIcon = !Game1.IsSnowingHere() ? (!Game1.IsRainingHere() ? (!Game1.IsDebrisWeatherHere() || !Game1.IsSpring ? (!Game1.IsDebrisWeatherHere() || !Game1.IsFall ? (!Game1.IsDebrisWeatherHere() || !Game1.IsWinter ? (!Game1.weddingToday ? 2 : 0) : 7) : 6) : 3) : 4) : 7;
    if (Game1.IsLightningHere())
      Game1.weatherIcon = 5;
    if (Utility.isFestivalDay())
      Game1.weatherIcon = 1;
    if (!Game1.IsGreenRainingHere())
      return;
    Game1.weatherIcon = 999;
  }

  public static void showEndOfNightStuff()
  {
    Game1.hooks.OnGame1_ShowEndOfNightStuff((Action) (() =>
    {
      if (!Game1.IsDedicatedHost)
      {
        bool flag1 = false;
        if (Game1.player.displayedShippedItems.Count > 0)
        {
          Game1.endOfNightMenus.Push((IClickableMenu) new ShippingMenu(Game1.player.displayedShippedItems));
          Game1.player.displayedShippedItems.Clear();
          flag1 = true;
        }
        bool flag2 = false;
        if (Game1.player.newLevels.Count > 0 && !flag1)
          Game1.endOfNightMenus.Push((IClickableMenu) new SaveGameMenu());
        for (int index = Game1.player.newLevels.Count - 1; index >= 0; --index)
        {
          Game1.endOfNightMenus.Push((IClickableMenu) new LevelUpMenu(Game1.player.newLevels[index].X, Game1.player.newLevels[index].Y));
          flag2 = true;
        }
        if ((Game1.player.farmingLevel.Value != 10 || Game1.player.miningLevel.Value != 10 || Game1.player.fishingLevel.Value != 10 || Game1.player.foragingLevel.Value != 10 ? 0 : (Game1.player.combatLevel.Value == 10 ? 1 : 0)) != 0 && Game1.player.mailReceived.Add("gotMasteryHint") && !Game1.player.locationsVisited.Contains("MasteryCave"))
          Game1.morningQueue.Enqueue((Action) (() => Game1.showGlobalMessage(Game1.content.LoadString("Strings\\1_6_Strings:MasteryHint"))));
        if (flag2)
          Game1.playSound("newRecord");
        if (Game1.client != null && Game1.client.timedOut)
          return;
      }
      if (Game1.endOfNightMenus.Count > 0)
      {
        Game1.showingEndOfNightStuff = true;
        Game1.activeClickableMenu = Game1.endOfNightMenus.Pop();
      }
      else
      {
        Game1.showingEndOfNightStuff = true;
        Game1.activeClickableMenu = (IClickableMenu) new SaveGameMenu();
      }
    }));
  }

  /// <summary>Update the game state when the season changes. Despite the name, this may update more than graphics (e.g. it'll remove grass in winter).</summary>
  /// <param name="onLoad">Whether the season is being initialized as part of loading the save, instead of an actual in-game season change.</param>
  public static void setGraphicsForSeason(bool onLoad = false)
  {
    foreach (GameLocation location in (IEnumerable<GameLocation>) Game1.locations)
    {
      Season season = location.GetSeason();
      location.seasonUpdate(onLoad);
      location.updateSeasonalTileSheets();
      if (location.IsOutdoors)
      {
        switch (season)
        {
          case Season.Spring:
            Game1.eveningColor = new Color((int) byte.MaxValue, (int) byte.MaxValue, 0);
            continue;
          case Season.Summer:
            foreach (Object @object in location.Objects.Values)
            {
              if (@object.IsWeeds())
              {
                switch (@object.QualifiedItemId)
                {
                  case "(O)882":
                  case "(O)883":
                  case "(O)884":
                    continue;
                  case "(O)792":
                    @object.SetIdAndSprite(@object.ParentSheetIndex + 1);
                    continue;
                  default:
                    if (Game1.random.NextDouble() < 0.3)
                    {
                      @object.SetIdAndSprite(676);
                      continue;
                    }
                    if (Game1.random.NextDouble() < 0.3)
                    {
                      @object.SetIdAndSprite(677);
                      continue;
                    }
                    continue;
                }
              }
            }
            Game1.eveningColor = new Color((int) byte.MaxValue, (int) byte.MaxValue, 0);
            continue;
          case Season.Fall:
            foreach (Object @object in location.Objects.Values)
            {
              if (@object.IsWeeds())
              {
                switch (@object.QualifiedItemId)
                {
                  case "(O)882":
                  case "(O)883":
                  case "(O)884":
                    continue;
                  case "(O)793":
                    @object.SetIdAndSprite(@object.ParentSheetIndex + 1);
                    continue;
                  default:
                    @object.SetIdAndSprite(Game1.random.Choose<int>(678, 679));
                    continue;
                }
              }
            }
            Game1.eveningColor = new Color((int) byte.MaxValue, (int) byte.MaxValue, 0);
            using (List<WeatherDebris>.Enumerator enumerator = Game1.debrisWeather.GetEnumerator())
            {
              while (enumerator.MoveNext())
                enumerator.Current.which = 2;
              continue;
            }
          case Season.Winter:
            foreach (KeyValuePair<Vector2, Object> keyValuePair in location.Objects.Pairs.ToArray<KeyValuePair<Vector2, Object>>())
            {
              Object @object = keyValuePair.Value;
              if (@object.IsWeeds())
              {
                string qualifiedItemId = @object.QualifiedItemId;
                if (!(qualifiedItemId == "(O)882") && !(qualifiedItemId == "(O)883") && !(qualifiedItemId == "(O)884"))
                  location.Objects.Remove(keyValuePair.Key);
              }
            }
            foreach (WeatherDebris weatherDebris in Game1.debrisWeather)
              weatherDebris.which = 3;
            Game1.eveningColor = new Color(245, 225, 170);
            continue;
          default:
            continue;
        }
      }
    }
  }

  public static void pauseThenMessage(int millisecondsPause, string message)
  {
    Game1.messageAfterPause = message;
    Game1.pauseTime = (float) millisecondsPause;
  }

  public static bool IsVisitingIslandToday(string npc_name)
  {
    return Game1.netWorldState.Value.IslandVisitors.Contains(npc_name);
  }

  public static bool shouldTimePass(bool ignore_multiplayer = false)
  {
    if (Game1.isFestival() || Game1.CurrentEvent != null && Game1.CurrentEvent.isWedding || Game1.farmEvent != null)
      return false;
    if (Game1.IsMultiplayer && !ignore_multiplayer)
      return !Game1.netWorldState.Value.IsTimePaused;
    if (Game1.paused || Game1.freezeControls || Game1.overlayMenu != null || Game1.isTimePaused || Game1.eventUp)
      return false;
    switch (Game1.activeClickableMenu)
    {
      case null:
      case BobberBar _:
        return Game1.player.CanMove || Game1.player.UsingTool || Game1.player.forceTimePass;
      default:
        return false;
    }
  }

  public static Farmer getPlayerOrEventFarmer()
  {
    return Game1.eventUp && Game1.CurrentEvent != null && !Game1.CurrentEvent.isFestival && Game1.CurrentEvent.farmer != null ? Game1.CurrentEvent.farmer : Game1.player;
  }

  public static void UpdateViewPort(bool overrideFreeze, Point centerPoint)
  {
    Game1.previousViewportPosition.X = (float) Game1.viewport.X;
    Game1.previousViewportPosition.Y = (float) Game1.viewport.Y;
    Farmer playerOrEventFarmer = Game1.getPlayerOrEventFarmer();
    if (Game1.currentLocation == null)
      return;
    if (!Game1.viewportFreeze | overrideFreeze)
    {
      Microsoft.Xna.Framework.Rectangle rectangle = Game1.viewportClampArea == Microsoft.Xna.Framework.Rectangle.Empty ? new Microsoft.Xna.Framework.Rectangle(0, 0, Game1.currentLocation.Map.DisplayWidth, Game1.currentLocation.Map.DisplayHeight) : Game1.viewportClampArea;
      Point standingPixel = playerOrEventFarmer.StandingPixel;
      bool flag = Game1.forceSnapOnNextViewportUpdate || (double) Math.Abs(Game1.currentViewportTarget.X + (float) (Game1.viewport.Width / 2) + (float) rectangle.X - (float) standingPixel.X) > 64.0 || (double) Math.Abs(Game1.currentViewportTarget.Y + (float) (Game1.viewport.Height / 2) + (float) rectangle.Y - (float) standingPixel.Y) > 64.0;
      if (centerPoint.X >= rectangle.X + Game1.viewport.Width / 2 && centerPoint.X <= rectangle.X + rectangle.Width - Game1.viewport.Width / 2)
      {
        if (playerOrEventFarmer.isRafting | flag)
          Game1.currentViewportTarget.X = (float) (centerPoint.X - Game1.viewport.Width / 2);
        else if ((double) Math.Abs(Game1.currentViewportTarget.X - (Game1.currentViewportTarget.X = (float) (centerPoint.X - Game1.viewport.Width / 2 + rectangle.X))) > (double) playerOrEventFarmer.getMovementSpeed())
          Game1.currentViewportTarget.X += (float) Math.Sign(Game1.currentViewportTarget.X - (Game1.currentViewportTarget.X = (float) (centerPoint.X - Game1.viewport.Width / 2 + rectangle.X))) * playerOrEventFarmer.getMovementSpeed();
      }
      else if (centerPoint.X < Game1.viewport.Width / 2 + rectangle.X && Game1.viewport.Width <= rectangle.Width)
      {
        if (playerOrEventFarmer.isRafting | flag)
          Game1.currentViewportTarget.X = (float) rectangle.X;
        else if ((double) Math.Abs(Game1.currentViewportTarget.X - (float) rectangle.X) > (double) playerOrEventFarmer.getMovementSpeed())
          Game1.currentViewportTarget.X -= (float) Math.Sign(Game1.currentViewportTarget.X - (float) rectangle.X) * playerOrEventFarmer.getMovementSpeed();
      }
      else if (Game1.viewport.Width <= rectangle.Width)
      {
        if (playerOrEventFarmer.isRafting | flag)
          Game1.currentViewportTarget.X = (float) (rectangle.X + rectangle.Width - Game1.viewport.Width);
        else if ((double) Math.Abs(Game1.currentViewportTarget.X - (float) (rectangle.Width - Game1.viewport.Width)) <= (double) playerOrEventFarmer.getMovementSpeed())
          ;
      }
      else if (rectangle.Width < Game1.viewport.Width)
      {
        if (playerOrEventFarmer.isRafting | flag)
        {
          Game1.currentViewportTarget.X = (float) ((rectangle.Width - Game1.viewport.Width) / 2 + rectangle.X);
        }
        else
        {
          double num = (double) Math.Abs(Game1.currentViewportTarget.X - (float) ((rectangle.Width + rectangle.X - Game1.viewport.Width) / 2));
          double movementSpeed = (double) playerOrEventFarmer.getMovementSpeed();
        }
      }
      if (centerPoint.Y >= Game1.viewport.Height / 2 && centerPoint.Y <= Game1.currentLocation.Map.DisplayHeight - Game1.viewport.Height / 2)
      {
        if (playerOrEventFarmer.isRafting | flag)
          Game1.currentViewportTarget.Y = (float) (centerPoint.Y - Game1.viewport.Height / 2);
        else if ((double) Math.Abs(Game1.currentViewportTarget.Y - (float) (centerPoint.Y - Game1.viewport.Height / 2)) >= (double) playerOrEventFarmer.getMovementSpeed())
          Game1.currentViewportTarget.Y -= (float) Math.Sign(Game1.currentViewportTarget.Y - (float) (centerPoint.Y - Game1.viewport.Height / 2)) * playerOrEventFarmer.getMovementSpeed();
      }
      else if (centerPoint.Y < Game1.viewport.Height / 2 && Game1.viewport.Height <= Game1.currentLocation.Map.DisplayHeight)
      {
        if (playerOrEventFarmer.isRafting | flag)
          Game1.currentViewportTarget.Y = 0.0f;
        else if ((double) Math.Abs(Game1.currentViewportTarget.Y - 0.0f) > (double) playerOrEventFarmer.getMovementSpeed())
          Game1.currentViewportTarget.Y -= (float) Math.Sign(Game1.currentViewportTarget.Y - 0.0f) * playerOrEventFarmer.getMovementSpeed();
        Game1.currentViewportTarget.Y = 0.0f;
      }
      else if (Game1.viewport.Height <= Game1.currentLocation.Map.DisplayHeight)
      {
        if (playerOrEventFarmer.isRafting | flag)
          Game1.currentViewportTarget.Y = (float) (Game1.currentLocation.Map.DisplayHeight - Game1.viewport.Height);
        else if ((double) Math.Abs(Game1.currentViewportTarget.Y - (float) (Game1.currentLocation.Map.DisplayHeight - Game1.viewport.Height)) > (double) playerOrEventFarmer.getMovementSpeed())
          Game1.currentViewportTarget.Y -= (float) Math.Sign(Game1.currentViewportTarget.Y - (float) (Game1.currentLocation.Map.DisplayHeight - Game1.viewport.Height)) * playerOrEventFarmer.getMovementSpeed();
      }
      else if (Game1.currentLocation.Map.DisplayHeight < Game1.viewport.Height)
      {
        if (playerOrEventFarmer.isRafting | flag)
          Game1.currentViewportTarget.Y = (float) ((Game1.currentLocation.Map.DisplayHeight - Game1.viewport.Height) / 2);
        else if ((double) Math.Abs(Game1.currentViewportTarget.Y - (float) ((Game1.currentLocation.Map.DisplayHeight - Game1.viewport.Height) / 2)) > (double) playerOrEventFarmer.getMovementSpeed())
          Game1.currentViewportTarget.Y -= (float) Math.Sign(Game1.currentViewportTarget.Y - (float) ((Game1.currentLocation.Map.DisplayHeight - Game1.viewport.Height) / 2)) * playerOrEventFarmer.getMovementSpeed();
      }
    }
    if (Game1.currentLocation.forceViewportPlayerFollow)
    {
      Game1.currentViewportTarget.X = playerOrEventFarmer.Position.X - (float) (Game1.viewport.Width / 2);
      Game1.currentViewportTarget.Y = playerOrEventFarmer.Position.Y - (float) (Game1.viewport.Height / 2);
    }
    bool nextViewportUpdate = Game1.forceSnapOnNextViewportUpdate;
    Game1.forceSnapOnNextViewportUpdate = false;
    if ((double) Game1.currentViewportTarget.X == (double) int.MinValue || Game1.viewportFreeze && !overrideFreeze)
      return;
    int num1 = (int) ((double) Game1.currentViewportTarget.X - (double) Game1.viewport.X);
    if (Math.Abs(num1) > 128 /*0x80*/)
      Game1.viewportPositionLerp.X = Game1.currentViewportTarget.X;
    else
      Game1.viewportPositionLerp.X += (float) ((double) num1 * (double) playerOrEventFarmer.getMovementSpeed() * 0.029999999329447746);
    int num2 = (int) ((double) Game1.currentViewportTarget.Y - (double) Game1.viewport.Y);
    if (Math.Abs(num2) > 128 /*0x80*/)
      Game1.viewportPositionLerp.Y = (float) (int) Game1.currentViewportTarget.Y;
    else
      Game1.viewportPositionLerp.Y += (float) ((double) num2 * (double) playerOrEventFarmer.getMovementSpeed() * 0.029999999329447746);
    if (nextViewportUpdate)
    {
      Game1.viewportPositionLerp.X = (float) (int) Game1.currentViewportTarget.X;
      Game1.viewportPositionLerp.Y = (float) (int) Game1.currentViewportTarget.Y;
    }
    Game1.viewport.X = (int) Game1.viewportPositionLerp.X;
    Game1.viewport.Y = (int) Game1.viewportPositionLerp.Y;
  }

  private void UpdateCharacters(GameTime time)
  {
    if (Game1.CurrentEvent?.farmer != null && Game1.CurrentEvent.farmer != Game1.player)
      Game1.CurrentEvent.farmer.Update(time, Game1.currentLocation);
    Game1.player.Update(time, Game1.currentLocation);
    foreach (KeyValuePair<long, Farmer> otherFarmer in Game1.otherFarmers)
    {
      if (otherFarmer.Key != Game1.player.UniqueMultiplayerID)
        otherFarmer.Value.UpdateIfOtherPlayer(time);
    }
  }

  public static void addMail(string mailName, bool noLetter = false, bool sendToEveryone = false)
  {
    if (sendToEveryone)
    {
      Game1.multiplayer.broadcastPartyWideMail(mailName, Multiplayer.PartyWideMessageQueue.SeenMail, noLetter);
    }
    else
    {
      mailName = mailName.Trim();
      mailName = mailName.Replace(Environment.NewLine, "");
      if (Game1.player.hasOrWillReceiveMail(mailName))
        return;
      if (noLetter)
        Game1.player.mailReceived.Add(mailName);
      else
        Game1.player.mailbox.Add(mailName);
    }
  }

  public static void addMailForTomorrow(string mailName, bool noLetter = false, bool sendToEveryone = false)
  {
    if (sendToEveryone)
    {
      Game1.multiplayer.broadcastPartyWideMail(mailName, no_letter: noLetter);
    }
    else
    {
      mailName = mailName.Trim();
      mailName = mailName.Replace(Environment.NewLine, "");
      if (Game1.player.hasOrWillReceiveMail(mailName))
        return;
      if (noLetter)
        mailName += "%&NL&%";
      Game1.player.mailForTomorrow.Add(mailName);
      if (!sendToEveryone || !Game1.IsMultiplayer)
        return;
      foreach (Farmer farmer in (IEnumerable<Farmer>) Game1.otherFarmers.Values)
      {
        if (farmer != Game1.player && !Game1.player.hasOrWillReceiveMail(mailName))
          farmer.mailForTomorrow.Add(mailName);
      }
    }
  }

  public static void drawDialogue(NPC speaker)
  {
    if (speaker.CurrentDialogue.Count == 0)
      return;
    Game1.activeClickableMenu = (IClickableMenu) new DialogueBox(speaker.CurrentDialogue.Peek());
    if (Game1.activeClickableMenu is DialogueBox activeClickableMenu && activeClickableMenu.dialogueFinished)
    {
      Game1.activeClickableMenu = (IClickableMenu) null;
    }
    else
    {
      Game1.dialogueUp = true;
      if (!Game1.eventUp)
      {
        Game1.player.Halt();
        Game1.player.CanMove = false;
      }
      if (speaker == null)
        return;
      Game1.currentSpeaker = speaker;
    }
  }

  public static void multipleDialogues(string[] messages)
  {
    Game1.activeClickableMenu = (IClickableMenu) new DialogueBox(((IEnumerable<string>) messages).ToList<string>());
    Game1.dialogueUp = true;
    Game1.player.CanMove = false;
  }

  public static void drawDialogueNoTyping(string dialogue)
  {
    Game1.drawObjectDialogue(dialogue);
    if (!(Game1.activeClickableMenu is DialogueBox activeClickableMenu))
      return;
    activeClickableMenu.showTyping = false;
  }

  public static void drawDialogueNoTyping(List<string> dialogues)
  {
    Game1.drawObjectDialogue(dialogues);
    if (!(Game1.activeClickableMenu is DialogueBox activeClickableMenu))
      return;
    activeClickableMenu.showTyping = false;
  }

  /// <summary>Show a dialogue box with text from an NPC's answering machine.</summary>
  /// <param name="npc">The NPC whose answering machine to display.</param>
  /// <param name="translationKey">The translation key for the message text.</param>
  /// <param name="substitutions">The token substitutions for placeholders in the translation text, if any.</param>
  public static void DrawAnsweringMachineDialogue(
    NPC npc,
    string translationKey,
    params object[] substitutions)
  {
    Dialogue dialogue = Dialogue.FromTranslation(npc, translationKey, substitutions);
    dialogue.overridePortrait = Game1.temporaryContent.Load<Texture2D>("Portraits\\AnsweringMachine");
    Game1.DrawDialogue(dialogue);
  }

  /// <summary>Show a dialogue box with text from an NPC.</summary>
  /// <param name="npc">The NPC whose dialogue to display.</param>
  /// <param name="translationKey">The translation from which to take the dialogue text, in the form <c>assetName:fieldKey</c> like <c>Strings/UI:Confirm</c>.</param>
  public static void DrawDialogue(NPC npc, string translationKey)
  {
    Game1.DrawDialogue(new Dialogue(npc, translationKey));
  }

  /// <summary>Show a dialogue box with text from an NPC.</summary>
  /// <param name="npc">The NPC whose dialogue to display.</param>
  /// <param name="translationKey">The translation from which to take the dialogue text, in the form <c>assetName:fieldKey</c> like <c>Strings/UI:Confirm</c>.</param>
  /// <param name="substitutions">The values with which to replace placeholders like <c>{0}</c> in the loaded text.</param>
  public static void DrawDialogue(NPC npc, string translationKey, params object[] substitutions)
  {
    Game1.DrawDialogue(Dialogue.FromTranslation(npc, translationKey, substitutions));
  }

  /// <summary>Show a dialogue box with text from an NPC.</summary>
  /// <param name="dialogue">The dialogue to display.</param>
  public static void DrawDialogue(Dialogue dialogue)
  {
    if (dialogue.speaker != null)
    {
      dialogue.speaker.CurrentDialogue.Push(dialogue);
      Game1.drawDialogue(dialogue.speaker);
    }
    else
    {
      Game1.activeClickableMenu = (IClickableMenu) new DialogueBox(dialogue);
      Game1.dialogueUp = true;
      if (Game1.eventUp)
        return;
      Game1.player.Halt();
      Game1.player.CanMove = false;
    }
  }

  private static void checkIfDialogueIsQuestion()
  {
    if (Game1.currentSpeaker == null || Game1.currentSpeaker.CurrentDialogue.Count <= 0 || !Game1.currentSpeaker.CurrentDialogue.Peek().isCurrentDialogueAQuestion())
      return;
    Game1.questionChoices.Clear();
    Game1.isQuestion = true;
    List<NPCDialogueResponse> npcResponseOptions = Game1.currentSpeaker.CurrentDialogue.Peek().getNPCResponseOptions();
    for (int index = 0; index < npcResponseOptions.Count; ++index)
      Game1.questionChoices.Add((Response) npcResponseOptions[index]);
  }

  public static void drawLetterMessage(string message)
  {
    Game1.activeClickableMenu = (IClickableMenu) new LetterViewerMenu(message);
  }

  public static void drawObjectDialogue(string dialogue)
  {
    Game1.activeClickableMenu?.emergencyShutDown();
    Game1.activeClickableMenu = (IClickableMenu) new DialogueBox(dialogue);
    Game1.player.CanMove = false;
    Game1.dialogueUp = true;
  }

  public static void drawObjectDialogue(List<string> dialogue)
  {
    Game1.activeClickableMenu?.emergencyShutDown();
    Game1.activeClickableMenu = (IClickableMenu) new DialogueBox(dialogue);
    Game1.player.CanMove = false;
    Game1.dialogueUp = true;
  }

  public static void drawObjectQuestionDialogue(string dialogue, Response[] choices, int width)
  {
    Game1.activeClickableMenu = (IClickableMenu) new DialogueBox(dialogue, choices, width);
    Game1.dialogueUp = true;
    Game1.player.CanMove = false;
  }

  public static void drawObjectQuestionDialogue(string dialogue, Response[] choices)
  {
    Game1.activeClickableMenu = (IClickableMenu) new DialogueBox(dialogue, choices);
    Game1.dialogueUp = true;
    Game1.player.CanMove = false;
  }

  /// <summary>Get whether it's summer in the valley.</summary>
  /// <remarks>See <see cref="M:StardewValley.GameLocation.IsSummerHere" /> to handle local seasons.</remarks>
  public static bool IsSummer => Game1.season == Season.Summer;

  /// <summary>Get whether it's spring in the valley.</summary>
  /// <remarks>See <see cref="M:StardewValley.GameLocation.IsSpringHere" /> to handle local seasons.</remarks>
  public static bool IsSpring => Game1.season == Season.Spring;

  /// <summary>Get whether it's fall in the valley.</summary>
  /// <remarks>See <see cref="M:StardewValley.GameLocation.IsFallHere" /> to handle local seasons.</remarks>
  public static bool IsFall => Game1.season == Season.Fall;

  /// <summary>Get whether it's winter in the valley.</summary>
  /// <remarks>See <see cref="M:StardewValley.GameLocation.IsWinterHere" /> to handle local seasons.</remarks>
  public static bool IsWinter => Game1.season == Season.Winter;

  public static void warpCharacter(NPC character, string targetLocationName, Point position)
  {
    Game1.warpCharacter(character, targetLocationName, new Vector2((float) position.X, (float) position.Y));
  }

  public static void warpCharacter(NPC character, string targetLocationName, Vector2 position)
  {
    Game1.warpCharacter(character, Game1.RequireLocation(targetLocationName), position);
  }

  public static void warpCharacter(NPC character, GameLocation targetLocation, Vector2 position)
  {
    foreach (string activePassiveFestival in (IEnumerable<string>) Game1.netWorldState.Value.ActivePassiveFestivals)
    {
      PassiveFestivalData data;
      string name;
      if (Utility.TryGetPassiveFestivalData(activePassiveFestival, out data) && Game1.dayOfMonth >= data.StartDay && Game1.dayOfMonth <= data.EndDay && data.Season == Game1.season && data.MapReplacements != null && data.MapReplacements.TryGetValue(targetLocation.name.Value, out name))
        targetLocation = Game1.RequireLocation(name);
    }
    if (targetLocation.name.Equals((object) "Trailer") && Game1.MasterPlayer.mailReceived.Contains("pamHouseUpgrade"))
    {
      targetLocation = Game1.RequireLocation("Trailer_Big");
      if ((double) position.X == 12.0 && (double) position.Y == 9.0)
      {
        position.X = 13f;
        position.Y = 24f;
      }
    }
    if (Game1.IsClient)
    {
      Game1.multiplayer.requestCharacterWarp(character, targetLocation, position);
    }
    else
    {
      if (!targetLocation.characters.Contains(character))
      {
        character.currentLocation?.characters.Remove(character);
        targetLocation.addCharacter(character);
      }
      character.isCharging = false;
      character.speed = 2;
      character.blockedInterval = 0;
      NPC.getTextureNameForCharacter(character.Name);
      character.position.X = position.X * 64f;
      character.position.Y = position.Y * 64f;
      if (character.CurrentDialogue.Count > 0 && character.CurrentDialogue.Peek().removeOnNextMove && character.Tile != character.DefaultPosition / 64f)
        character.CurrentDialogue.Pop();
      if (targetLocation is FarmHouse farmHouse)
        character.arriveAtFarmHouse(farmHouse);
      else
        character.arriveAt(targetLocation);
      if (character.currentLocation != null && !character.currentLocation.Equals(targetLocation))
        character.currentLocation.characters.Remove(character);
      character.currentLocation = targetLocation;
    }
  }

  public static LocationRequest getLocationRequest(string locationName, bool isStructure = false)
  {
    return locationName != null ? new LocationRequest(locationName, isStructure, Game1.getLocationFromName(locationName, isStructure)) : throw new ArgumentException();
  }

  public static void warpHome()
  {
    LocationRequest locationRequest = Game1.getLocationRequest(Game1.player.homeLocation.Value);
    locationRequest.OnWarp += (LocationRequest.Callback) (() => Game1.player.position.Set(Utility.PointToVector2((Game1.currentLocation as FarmHouse).GetPlayerBedSpot()) * 64f));
    Game1.warpFarmer(locationRequest, 5, 9, Game1.player.FacingDirection);
  }

  public static void warpFarmer(string locationName, int tileX, int tileY, bool flip)
  {
    Game1.warpFarmer(Game1.getLocationRequest(locationName), tileX, tileY, flip ? (Game1.player.FacingDirection + 2) % 4 : Game1.player.FacingDirection);
  }

  public static void warpFarmer(
    string locationName,
    int tileX,
    int tileY,
    int facingDirectionAfterWarp)
  {
    Game1.warpFarmer(Game1.getLocationRequest(locationName), tileX, tileY, facingDirectionAfterWarp);
  }

  public static void warpFarmer(
    string locationName,
    int tileX,
    int tileY,
    int facingDirectionAfterWarp,
    bool isStructure)
  {
    Game1.warpFarmer(Game1.getLocationRequest(locationName, isStructure), tileX, tileY, facingDirectionAfterWarp);
  }

  public virtual bool ShouldDismountOnWarp(
    Horse mount,
    GameLocation old_location,
    GameLocation new_location)
  {
    return mount != null && Game1.currentLocation != null && Game1.currentLocation.IsOutdoors && new_location != null && !new_location.IsOutdoors;
  }

  public static void warpFarmer(
    LocationRequest locationRequest,
    int tileX,
    int tileY,
    int facingDirectionAfterWarp)
  {
    int warp_offset_x = Game1.nextFarmerWarpOffsetX;
    int warp_offset_y = Game1.nextFarmerWarpOffsetY;
    Game1.nextFarmerWarpOffsetX = 0;
    Game1.nextFarmerWarpOffsetY = 0;
    foreach (string activePassiveFestival in (IEnumerable<string>) Game1.netWorldState.Value.ActivePassiveFestivals)
    {
      PassiveFestivalData data;
      string locationName;
      if (Utility.TryGetPassiveFestivalData(activePassiveFestival, out data) && Game1.dayOfMonth >= data.StartDay && Game1.dayOfMonth <= data.EndDay && data.Season == Game1.season && data.MapReplacements != null && data.MapReplacements.TryGetValue(locationRequest.Name, out locationName))
        locationRequest = Game1.getLocationRequest(locationName);
    }
    switch (locationRequest.Name)
    {
      case "BusStop":
        if (tileX < 10)
        {
          tileX = 10;
          break;
        }
        break;
      case "Farm":
        switch (Game1.currentLocation?.NameOrUniqueName)
        {
          case "FarmCave":
            if (tileX == 34 && tileY == 6)
            {
              Point parsed;
              if (Game1.getFarm().TryGetMapPropertyAs("FarmCaveEntry", out parsed))
              {
                tileX = parsed.X;
                tileY = parsed.Y;
                break;
              }
              switch (Game1.whichFarm)
              {
                case 5:
                  tileX = 30;
                  tileY = 36;
                  break;
                case 6:
                  tileX = 34;
                  tileY = 16 /*0x10*/;
                  break;
              }
            }
            else
              break;
            break;
          case "Forest":
            if (tileX == 41 && tileY == 64 /*0x40*/)
            {
              Point parsed;
              if (Game1.getFarm().TryGetMapPropertyAs("ForestEntry", out parsed))
              {
                tileX = parsed.X;
                tileY = parsed.Y;
                break;
              }
              switch (Game1.whichFarm)
              {
                case 5:
                  tileX = 40;
                  tileY = 64 /*0x40*/;
                  break;
                case 6:
                  tileX = 82;
                  tileY = 103;
                  break;
              }
            }
            else
              break;
            break;
          case "BusStop":
            Point parsed1;
            if (tileX == 79 && tileY == 17 && Game1.getFarm().TryGetMapPropertyAs("BusStopEntry", out parsed1))
            {
              tileX = parsed1.X;
              tileY = parsed1.Y;
              break;
            }
            break;
          case "Backwoods":
            Point parsed2;
            if (tileX == 40 && tileY == 0 && Game1.getFarm().TryGetMapPropertyAs("BackwoodsEntry", out parsed2))
            {
              tileX = parsed2.X;
              tileY = parsed2.Y;
              break;
            }
            break;
        }
        break;
      case "IslandSouth":
        if (tileX <= 15 && tileY <= 6)
        {
          tileX = 21;
          tileY = 43;
          break;
        }
        break;
      case "Trailer":
        if (Game1.MasterPlayer.mailReceived.Contains("pamHouseUpgrade"))
        {
          locationRequest = Game1.getLocationRequest("Trailer_Big");
          tileX = 13;
          tileY = 24;
          break;
        }
        break;
      case "Club":
        if (!Game1.player.hasClubCard)
        {
          locationRequest = Game1.getLocationRequest("SandyHouse");
          locationRequest.OnWarp += (LocationRequest.Callback) (() =>
          {
            NPC characterFromName = Game1.currentLocation.getCharacterFromName("Bouncer");
            if (characterFromName == null)
              return;
            Vector2 vector2 = new Vector2(17f, 4f);
            characterFromName.showTextAboveHead(Game1.content.LoadString("Strings\\Locations:Club_Bouncer_TextAboveHead" + (Game1.random.Next(2) + 1).ToString()));
            int num = Game1.random.Next();
            Game1.currentLocation.playSound("thudStep");
            Game1.multiplayer.broadcastSprites(Game1.currentLocation, new TemporaryAnimatedSprite(288, 100f, 1, 24, vector2 * 64f, true, false, Game1.currentLocation, Game1.player)
            {
              shakeIntensity = 0.5f,
              shakeIntensityChange = 1f / 500f,
              extraInfoForEndBehavior = num,
              endFunction = new TemporaryAnimatedSprite.endBehavior(Game1.currentLocation.removeTemporarySpritesWithID)
            }, new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(598, 1279 /*0x04FF*/, 3, 4), 53f, 5, 9, vector2 * 64f + new Vector2(5f, 0.0f) * 4f, true, false, 0.0263f, 0.0f, Color.Yellow, 4f, 0.0f, 0.0f, 0.0f)
            {
              id = num
            }, new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(598, 1279 /*0x04FF*/, 3, 4), 53f, 5, 9, vector2 * 64f + new Vector2(5f, 0.0f) * 4f, true, true, 0.0263f, 0.0f, Color.Orange, 4f, 0.0f, 0.0f, 0.0f)
            {
              delayBeforeAnimationStart = 100,
              id = num
            }, new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(598, 1279 /*0x04FF*/, 3, 4), 53f, 5, 9, vector2 * 64f + new Vector2(5f, 0.0f) * 4f, true, false, 0.0263f, 0.0f, Color.White, 3f, 0.0f, 0.0f, 0.0f)
            {
              delayBeforeAnimationStart = 200,
              id = num
            });
            Game1.currentLocation.netAudio.StartPlaying("fuse");
          });
          tileX = 17;
          tileY = 4;
          break;
        }
        break;
    }
    if (VolcanoDungeon.IsGeneratedLevel(locationRequest.Name))
    {
      warp_offset_x = 0;
      warp_offset_y = 0;
    }
    if (Game1.player.isRidingHorse() && Game1.currentLocation != null)
    {
      GameLocation new_location = locationRequest.Location ?? Game1.getLocationFromName(locationRequest.Name);
      if (Game1.game1.ShouldDismountOnWarp(Game1.player.mount, Game1.currentLocation, new_location))
      {
        Game1.player.mount.dismount();
        warp_offset_x = 0;
        warp_offset_y = 0;
      }
    }
    if (Game1.weatherIcon == 1 && Game1.whereIsTodaysFest != null && locationRequest.Name.Equals(Game1.whereIsTodaysFest) && !Game1.warpingForForcedRemoteEvent)
    {
      string[] strArray = ArgUtility.SplitBySpace(Game1.temporaryContent.Load<Dictionary<string, string>>($"Data\\Festivals\\{Game1.currentSeason}{Game1.dayOfMonth.ToString()}")["conditions"].Split('/')[1]);
      if (Game1.timeOfDay <= Convert.ToInt32(strArray[1]))
      {
        if (Game1.timeOfDay < Convert.ToInt32(strArray[0]))
        {
          if (Game1.currentLocation?.Name == "Hospital")
          {
            locationRequest = Game1.getLocationRequest("BusStop");
            tileX = 34;
            tileY = 23;
          }
          else
          {
            Game1.player.Position = Game1.player.lastPosition;
            Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.2973"));
            return;
          }
        }
        else
        {
          if (Game1.IsMultiplayer)
          {
            Game1.netReady.SetLocalReady("festivalStart", true);
            Game1.activeClickableMenu = (IClickableMenu) new ReadyCheckDialog("festivalStart", true, (ConfirmationDialog.behavior) (who =>
            {
              Game1.exitActiveMenu();
              if (Game1.player.mount != null)
              {
                Game1.player.mount.dismount();
                warp_offset_x = 0;
                warp_offset_y = 0;
              }
              Game1.performWarpFarmer(locationRequest, tileX, tileY, facingDirectionAfterWarp);
            }));
            return;
          }
          if (Game1.player.mount != null)
          {
            Game1.player.mount.dismount();
            warp_offset_x = 0;
            warp_offset_y = 0;
          }
        }
      }
    }
    tileX += warp_offset_x;
    tileY += warp_offset_y;
    Game1.performWarpFarmer(locationRequest, tileX, tileY, facingDirectionAfterWarp);
  }

  private static void performWarpFarmer(
    LocationRequest locationRequest,
    int tileX,
    int tileY,
    int facingDirectionAfterWarp)
  {
    if (locationRequest.Location != null)
    {
      if (tileX >= locationRequest.Location.Map.Layers[0].LayerWidth - 1)
        --tileX;
      if (Game1.IsMasterGame)
        locationRequest.Location.hostSetup();
    }
    Game1.log.Verbose("Warping to " + locationRequest.Name);
    if (Game1.player.IsSitting())
      Game1.player.StopSitting(false);
    if (Game1.player.UsingTool)
      Game1.player.completelyStopAnimatingOrDoingAction();
    Game1.player.previousLocationName = Game1.player.currentLocation != null ? Game1.player.currentLocation.name.Value : "";
    Game1.locationRequest = locationRequest;
    Game1.xLocationAfterWarp = tileX;
    Game1.yLocationAfterWarp = tileY;
    Game1._isWarping = true;
    Game1.facingDirectionAfterWarp = facingDirectionAfterWarp;
    Game1.fadeScreenToBlack();
    Game1.setRichPresence("location", (object) locationRequest.Name);
    if (!Game1.IsDedicatedHost)
      return;
    Game1.fadeToBlackAlpha = 1.1f;
    Game1.fadeToBlack = true;
    Game1.nonWarpFade = false;
  }

  private static void notifyServerOfWarp(bool needsLocationInfo)
  {
    if (Game1.locationRequest == null)
      return;
    byte num1 = (byte) ((Game1.locationRequest.IsStructure ? 1 : 0) | (Game1.warpingForForcedRemoteEvent ? 2 : 0) | (needsLocationInfo ? 4 : 0));
    byte num2;
    switch (Game1.facingDirectionAfterWarp)
    {
      case 1:
        num2 = (byte) ((uint) num1 | 16U /*0x10*/);
        break;
      case 2:
        num2 = (byte) ((uint) num1 | 32U /*0x20*/);
        break;
      case 3:
        num2 = (byte) ((uint) num1 | 64U /*0x40*/);
        break;
      default:
        num2 = (byte) ((uint) num1 | 8U);
        break;
    }
    Game1.client.sendMessage((byte) 5, (object) (short) Game1.xLocationAfterWarp, (object) (short) Game1.yLocationAfterWarp, (object) Game1.locationRequest.Name, (object) num2);
  }

  public static void requestLocationInfoFromServer()
  {
    Game1.notifyServerOfWarp(true);
    Game1.currentLocation = (GameLocation) null;
    Game1.player.Position = new Vector2((float) (Game1.xLocationAfterWarp * 64 /*0x40*/), (float) (Game1.yLocationAfterWarp * 64 /*0x40*/ - (Game1.player.Sprite.getHeight() - 32 /*0x20*/) + 16 /*0x10*/));
    Game1.player.faceDirection(Game1.facingDirectionAfterWarp);
  }

  /// <summary>Get the first NPC which matches a condition.</summary>
  /// <typeparam name="T">The expected NPC type.</typeparam>
  /// <param name="check">The condition to check on each NPC.</param>
  /// <param name="includeEventActors">Whether to match temporary event actors.</param>
  /// <returns>Returns the matching NPC if found, else <c>null</c>.</returns>
  public static T GetCharacterWhere<T>(Func<T, bool> check, bool includeEventActors = false) where T : NPC
  {
    T match = default (T);
    T fallback = default (T);
    Utility.ForEachCharacter((Func<NPC, bool>) (rawNpc =>
    {
      if (rawNpc is T obj2 && check(obj2))
      {
        bool? nullable = obj2.currentLocation?.IsActiveLocation();
        if (nullable.HasValue && nullable.GetValueOrDefault())
        {
          match = obj2;
          return false;
        }
        fallback = obj2;
      }
      return true;
    }), includeEventActors);
    return match ?? fallback;
  }

  /// <summary>Get the first NPC of the given type.</summary>
  /// <typeparam name="T">The expected NPC type.</typeparam>
  /// <param name="includeEventActors">Whether to match temporary event actors.</param>
  /// <returns>Returns the matching NPC if found, else <c>null</c>.</returns>
  public static T GetCharacterOfType<T>(bool includeEventActors = false) where T : NPC
  {
    T match = default (T);
    T fallback = default (T);
    Utility.ForEachCharacter((Func<NPC, bool>) (rawNpc =>
    {
      if (rawNpc is T obj2)
      {
        bool? nullable = rawNpc.currentLocation?.IsActiveLocation();
        if (nullable.HasValue && nullable.GetValueOrDefault())
        {
          match = obj2;
          return false;
        }
        fallback = obj2;
      }
      return true;
    }), includeEventActors);
    return match ?? fallback;
  }

  /// <summary>Get an NPC by its name.</summary>
  /// <typeparam name="T">The expected NPC type.</typeparam>
  /// <param name="name">The NPC name.</param>
  /// <param name="mustBeVillager">Whether to only match NPCs which return true for <see cref="P:StardewValley.NPC.IsVillager" />.</param>
  /// <param name="includeEventActors">Whether to match temporary event actors.</param>
  /// <returns>Returns the matching NPC if found, else <c>null</c>.</returns>
  public static T getCharacterFromName<T>(
    string name,
    bool mustBeVillager = true,
    bool includeEventActors = false)
    where T : NPC
  {
    T match = default (T);
    T fallback = default (T);
    Utility.ForEachCharacter((Func<NPC, bool>) (rawNpc =>
    {
      if (rawNpc is T obj2 && obj2.Name == name && (!mustBeVillager || obj2.IsVillager))
      {
        bool? nullable = obj2.currentLocation?.IsActiveLocation();
        if (nullable.HasValue && nullable.GetValueOrDefault())
        {
          match = obj2;
          return false;
        }
        fallback = obj2;
      }
      return true;
    }), includeEventActors);
    return match ?? fallback;
  }

  /// <summary>Get an NPC by its name.</summary>
  /// <param name="name">The NPC name.</param>
  /// <param name="mustBeVillager">Whether to only match NPCs which return true for <see cref="P:StardewValley.NPC.IsVillager" />.</param>
  /// <param name="includeEventActors">Whether to match temporary event actors.</param>
  /// <returns>Returns the matching NPC if found, else <c>null</c>.</returns>
  public static NPC getCharacterFromName(string name, bool mustBeVillager = true, bool includeEventActors = false)
  {
    NPC match = (NPC) null;
    NPC fallback = (NPC) null;
    Utility.ForEachCharacter((Func<NPC, bool>) (npc =>
    {
      if (npc.Name == name && (!mustBeVillager || npc.IsVillager))
      {
        bool? nullable = npc.currentLocation?.IsActiveLocation();
        if (nullable.HasValue && nullable.GetValueOrDefault())
        {
          match = npc;
          return false;
        }
        fallback = npc;
      }
      return true;
    }), includeEventActors);
    return match ?? fallback;
  }

  /// <summary>Get an NPC by its name, or throw an exception if it's not found.</summary>
  /// <param name="name">The NPC name.</param>
  /// <param name="mustBeVillager">Whether to only match NPCs which return true for <see cref="P:StardewValley.NPC.IsVillager" />.</param>
  public static NPC RequireCharacter(string name, bool mustBeVillager = true)
  {
    return Game1.getCharacterFromName(name, mustBeVillager) ?? throw new KeyNotFoundException($"Required {(mustBeVillager ? "villager" : "NPC")} '{name}' not found.");
  }

  /// <summary>Get an NPC by its name, or throw an exception if it's not found.</summary>
  /// <typeparam name="T">The expected NPC type.</typeparam>
  /// <param name="name">The NPC name.</param>
  /// <param name="mustBeVillager">Whether to only match NPCs which return true for <see cref="P:StardewValley.NPC.IsVillager" />.</param>
  /// <exception cref="T:System.Collections.Generic.KeyNotFoundException">There's no NPC matching the given arguments.</exception>
  /// <exception cref="T:System.InvalidCastException">The NPC found can't be converted to <typeparamref name="T" />.</exception>
  public static T RequireCharacter<T>(string name, bool mustBeVillager = true) where T : NPC
  {
    NPC characterFromName = Game1.getCharacterFromName(name, mustBeVillager);
    if (characterFromName is T obj)
      return obj;
    if (characterFromName == null)
      throw new KeyNotFoundException($"Required {(mustBeVillager ? "villager" : "NPC")} '{name}' not found.");
    throw new InvalidCastException($"Can't convert NPC '{name}' from '{characterFromName?.GetType().FullName}' to the required '{typeof (T).FullName}'.");
  }

  /// <summary>Get a location by its name, or throw an exception if it's not found.</summary>
  /// <param name="name">The location name.</param>
  /// <param name="isStructure">Whether the location is an interior structure.</param>
  /// <exception cref="T:System.Collections.Generic.KeyNotFoundException">There's no location matching the given arguments.</exception>
  public static GameLocation RequireLocation(string name, bool isStructure = false)
  {
    return Game1.getLocationFromName(name, isStructure) ?? throw new KeyNotFoundException($"Required {(isStructure ? "structure " : "")}location '{name}' not found.");
  }

  /// <summary>Get a location by its name, or throw an exception if it's not found.</summary>
  /// <typeparam name="TLocation">The expected location type.</typeparam>
  /// <param name="name">The location name.</param>
  /// <param name="isStructure">Whether the location is an interior structure.</param>
  /// <exception cref="T:System.Collections.Generic.KeyNotFoundException">There's no location matching the given arguments.</exception>
  /// <exception cref="T:System.InvalidCastException">The location found can't be converted to <typeparamref name="TLocation" />.</exception>
  public static TLocation RequireLocation<TLocation>(string name, bool isStructure = false) where TLocation : GameLocation
  {
    GameLocation locationFromName = Game1.getLocationFromName(name, isStructure);
    if (locationFromName is TLocation location)
      return location;
    if (locationFromName == null)
      throw new KeyNotFoundException($"Required {(isStructure ? "structure " : "")}location '{name}' not found.");
    throw new InvalidCastException($"Can't convert location {name} from '{locationFromName?.GetType().FullName}' to the required '{typeof (TLocation).FullName}'.");
  }

  /// <summary>Get a location by its name, or <c>null</c> if it's not found.</summary>
  /// <param name="name">The location name.</param>
  public static GameLocation getLocationFromName(string name)
  {
    return Game1.getLocationFromName(name, false);
  }

  /// <summary>Get a location by its name, or <c>null</c> if it's not found.</summary>
  /// <param name="name">The location name.</param>
  /// <param name="isStructure">Whether the location is an interior structure.</param>
  public static GameLocation getLocationFromName(string name, bool isStructure)
  {
    if (string.IsNullOrEmpty(name))
      return (GameLocation) null;
    if (Game1.currentLocation != null)
    {
      if (!isStructure)
      {
        if (Game1.currentLocation.name.Value.EqualsIgnoreCase(name))
          return Game1.currentLocation;
        if (Game1.currentLocation.isStructure.Value && (NetFieldBase<GameLocation, NetRef<GameLocation>>) Game1.currentLocation.Root != (NetRef<GameLocation>) null && Game1.currentLocation.Root.Value.NameOrUniqueName.EqualsIgnoreCase(name))
          return Game1.currentLocation.Root.Value;
      }
      else if (Game1.currentLocation.NameOrUniqueName == name)
        return Game1.currentLocation;
    }
    GameLocation gameLocation;
    return Game1._locationLookup.TryGetValue(name, out gameLocation) ? gameLocation : Game1.getLocationFromNameInLocationsList(name, isStructure);
  }

  /// <summary>Get a location by its name (ignoring the cache and current location), or <c>null</c> if it's not found.</summary>
  /// <param name="name">The location name.</param>
  /// <param name="isStructure">Whether the location is an interior structure.</param>
  public static GameLocation getLocationFromNameInLocationsList(string name, bool isStructure = false)
  {
    for (int index = 0; index < Game1.locations.Count; ++index)
    {
      GameLocation location = Game1.locations[index];
      if (!isStructure)
      {
        if (location.Name.EqualsIgnoreCase(name))
        {
          Game1._locationLookup[location.Name] = location;
          return location;
        }
      }
      else
      {
        GameLocation structure = Game1.findStructure(location, name);
        if (structure != null)
        {
          Game1._locationLookup[name] = structure;
          return structure;
        }
      }
    }
    if (MineShaft.IsGeneratedLevel(name))
      return (GameLocation) MineShaft.GetMine(name);
    if (VolcanoDungeon.IsGeneratedLevel(name))
      return (GameLocation) VolcanoDungeon.GetLevel(name);
    return !isStructure ? Game1.getLocationFromName(name, true) : (GameLocation) null;
  }

  public static void flushLocationLookup() => Game1._locationLookup.Clear();

  public static void removeLocationFromLocationLookup(string nameOrUniqueName)
  {
    Game1._locationLookup.RemoveWhere<string, GameLocation>((Func<KeyValuePair<string, GameLocation>, bool>) (p => p.Value.NameOrUniqueName == nameOrUniqueName));
  }

  public static void removeLocationFromLocationLookup(GameLocation location)
  {
    Game1._locationLookup.RemoveWhere<string, GameLocation>((Func<KeyValuePair<string, GameLocation>, bool>) (p => p.Value == location));
  }

  public static GameLocation findStructure(GameLocation parentLocation, string name)
  {
    foreach (Building building in parentLocation.buildings)
    {
      if (building.HasIndoorsName(name))
        return building.GetIndoors();
    }
    return (GameLocation) null;
  }

  public static void addNewFarmBuildingMaps()
  {
    FarmHouse homeOfFarmer = Utility.getHomeOfFarmer(Game1.player);
    if (Game1.player.HouseUpgradeLevel < 1 || !homeOfFarmer.Map.Id.Equals("FarmHouse"))
      return;
    homeOfFarmer.updateMap();
  }

  public static void PassOutNewDay()
  {
    Game1.player.lastSleepLocation.Value = Game1.currentLocation.NameOrUniqueName;
    Game1.player.lastSleepPoint.Value = Game1.player.TilePoint;
    if (!Game1.IsMultiplayer)
    {
      Game1.NewDay(0.0f);
    }
    else
    {
      Game1.player.FarmerSprite.setCurrentSingleFrame(5, (short) 3000);
      Game1.player.FarmerSprite.PauseForSingleAnimation = true;
      Game1.player.passedOut = true;
      if (Game1.activeClickableMenu != null)
      {
        Game1.activeClickableMenu.emergencyShutDown();
        Game1.exitActiveMenu();
      }
      Game1.activeClickableMenu = (IClickableMenu) new ReadyCheckDialog("sleep", false, (ConfirmationDialog.behavior) (_ => Game1.NewDay(0.0f)));
    }
  }

  public static void NewDay(float timeToPause)
  {
    if (Game1.activeClickableMenu is ReadyCheckDialog activeClickableMenu && activeClickableMenu.checkName == "sleep" && !activeClickableMenu.isCancelable())
      activeClickableMenu.confirm();
    Game1.currentMinigame = (IMinigame) null;
    Game1.newDay = true;
    Game1.newDaySync.create();
    if (Game1.player.isInBed.Value || Game1.player.passedOut)
    {
      Game1.nonWarpFade = true;
      Game1.screenFade.FadeScreenToBlack(Game1.player.passedOut ? 1.1f : 0.0f);
      Game1.player.Halt();
      Game1.player.currentEyes = 1;
      Game1.player.blinkTimer = -4000;
      Game1.player.CanMove = false;
      Game1.player.passedOut = false;
      Game1.pauseTime = timeToPause;
    }
    if (Game1.activeClickableMenu == null || Game1.dialogueUp)
      return;
    Game1.activeClickableMenu.emergencyShutDown();
    Game1.exitActiveMenu();
  }

  public static void screenGlowOnce(Color glowColor, bool hold, float rate = 0.005f, float maxAlpha = 0.3f)
  {
    Game1.screenGlowMax = maxAlpha;
    Game1.screenGlowRate = rate;
    Game1.screenGlowAlpha = 0.0f;
    Game1.screenGlowUp = true;
    Game1.screenGlowColor = glowColor;
    Game1.screenGlow = true;
    Game1.screenGlowHold = hold;
  }

  public static string shortDayNameFromDayOfSeason(int dayOfSeason)
  {
    switch (dayOfSeason % 7)
    {
      case 0:
        return "Sun";
      case 1:
        return "Mon";
      case 2:
        return "Tue";
      case 3:
        return "Wed";
      case 4:
        return "Thu";
      case 5:
        return "Fri";
      case 6:
        return "Sat";
      default:
        return "";
    }
  }

  public static string shortDayDisplayNameFromDayOfSeason(int dayOfSeason)
  {
    return dayOfSeason < 0 ? string.Empty : Game1._shortDayDisplayName[dayOfSeason % 7];
  }

  public static void runTestEvent()
  {
    StreamReader streamReader = new StreamReader("test_event.txt");
    string locationName = streamReader.ReadLine();
    string event_string = streamReader.ReadToEnd();
    event_string = event_string.Replace("\r\n", "/").Replace("\n", "/");
    Game1.log.Verbose("Running test event: " + event_string);
    LocationRequest locationRequest = Game1.getLocationRequest(locationName);
    locationRequest.OnWarp += (LocationRequest.Callback) (() =>
    {
      Game1.currentLocation.currentEvent = new Event(event_string);
      Game1.currentLocation.checkForEvents();
    });
    int x = 8;
    int y = 8;
    Utility.getDefaultWarpLocation(locationName, ref x, ref y);
    Game1.warpFarmer(locationRequest, x, y, Game1.player.FacingDirection);
  }

  public static bool isMusicContextActiveButNotPlaying(MusicContext music_context = MusicContext.Default)
  {
    if (Game1._activeMusicContext != music_context || Game1.morningSongPlayAction != null)
      return false;
    string musicTrackName = Game1.getMusicTrackName(music_context);
    return musicTrackName == "none" || Game1.currentSong != null && Game1.currentSong.Name == musicTrackName && !Game1.currentSong.IsPlaying;
  }

  public static bool IsMusicContextActive(MusicContext music_context = MusicContext.Default)
  {
    return Game1._activeMusicContext != music_context;
  }

  public static bool doesMusicContextHaveTrack(MusicContext music_context = MusicContext.Default)
  {
    return Game1._requestedMusicTracks.ContainsKey(music_context);
  }

  public static string getMusicTrackName(MusicContext music_context = MusicContext.Default)
  {
    KeyValuePair<string, bool> keyValuePair;
    if (Game1._requestedMusicTracks.TryGetValue(music_context, out keyValuePair))
      return keyValuePair.Key;
    return music_context == MusicContext.Default ? Game1.getMusicTrackName(MusicContext.SubLocation) : "none";
  }

  public static void stopMusicTrack(MusicContext music_context)
  {
    if (!Game1._requestedMusicTracks.Remove(music_context))
      return;
    if (music_context == MusicContext.Default)
      Game1.stopMusicTrack(MusicContext.SubLocation);
    Game1.UpdateRequestedMusicTrack();
  }

  public static void changeMusicTrack(
    string newTrackName,
    bool track_interruptable = false,
    MusicContext music_context = MusicContext.Default)
  {
    if (newTrackName == null)
      return;
    if (music_context == MusicContext.Default)
    {
      if (Game1.morningSongPlayAction != null)
      {
        if (Game1.delayedActions.Contains(Game1.morningSongPlayAction))
          Game1.delayedActions.Remove(Game1.morningSongPlayAction);
        Game1.morningSongPlayAction = (DelayedAction) null;
      }
      if (Game1.IsGreenRainingHere() && !Game1.currentLocation.InIslandContext() && Game1.IsRainingHere(Game1.currentLocation) && !newTrackName.Equals("rain"))
        return;
    }
    if (music_context == MusicContext.Default || music_context == MusicContext.SubLocation)
    {
      Game1.IsPlayingBackgroundMusic = false;
      Game1.IsPlayingOutdoorsAmbience = false;
      Game1.IsPlayingNightAmbience = false;
      Game1.IsPlayingTownMusic = false;
      Game1.IsPlayingMorningSong = false;
    }
    if (music_context != MusicContext.ImportantSplitScreenMusic && !Game1.player.songsHeard.Contains(newTrackName))
      Utility.farmerHeardSong(newTrackName);
    Game1._requestedMusicTracks[music_context] = new KeyValuePair<string, bool>(newTrackName, track_interruptable);
    Game1.UpdateRequestedMusicTrack();
  }

  public static void UpdateRequestedMusicTrack()
  {
    Game1._activeMusicContext = MusicContext.Default;
    KeyValuePair<string, bool> keyValuePair1 = new KeyValuePair<string, bool>("none", true);
    for (int index = 0; index < 6; ++index)
    {
      MusicContext key = (MusicContext) index;
      KeyValuePair<string, bool> keyValuePair2;
      if (Game1._requestedMusicTracks.TryGetValue(key, out keyValuePair2))
      {
        if (key != MusicContext.ImportantSplitScreenMusic)
          Game1._activeMusicContext = key;
        keyValuePair1 = keyValuePair2;
      }
    }
    if (!(keyValuePair1.Key != Game1.requestedMusicTrack) && keyValuePair1.Value == Game1.requestedMusicTrackOverrideable)
      return;
    Game1.requestedMusicDirty = true;
    Game1.requestedMusicTrack = keyValuePair1.Key;
    Game1.requestedMusicTrackOverrideable = keyValuePair1.Value;
  }

  /// <summary>Warp the player into a generated mine level.</summary>
  /// <param name="whatLevel">The mine level.</param>
  /// <param name="forceLayout">The layout in <c>Maps/Mines</c> to use, or <c>null</c> to choose a random one based on the level.</param>
  public static void enterMine(int whatLevel, int? forceLayout = null)
  {
    Game1.warpFarmer(MineShaft.GetLevelName(whatLevel, forceLayout), 6, 6, 2);
    Game1.player.temporarilyInvincible = true;
    Game1.player.temporaryInvincibilityTimer = 0;
    Game1.player.flashDuringThisTemporaryInvincibility = false;
    Game1.player.currentTemporaryInvincibilityDuration = 1000;
  }

  /// <summary>Get the season which currently applies to a location.</summary>
  /// <param name="location">The location to check, or <c>null</c> for the global season.</param>
  public static Season GetSeasonForLocation(GameLocation location)
  {
    return location == null ? Game1.season : location.GetSeason();
  }

  /// <summary>Get the season which currently applies to a location as a numeric index.</summary>
  /// <param name="location">The location to check, or <c>null</c> for the global season.</param>
  /// <remarks>Most code should use <see cref="M:StardewValley.Game1.GetSeasonForLocation(StardewValley.GameLocation)" /> instead.</remarks>
  public static int GetSeasonIndexForLocation(GameLocation location)
  {
    return location == null ? Game1.seasonIndex : location.GetSeasonIndex();
  }

  /// <summary>Get the season which currently applies to a location as a string.</summary>
  /// <param name="location">The location to check, or <c>null</c> for the global season.</param>
  /// <remarks>Most code should use <see cref="M:StardewValley.Game1.GetSeasonForLocation(StardewValley.GameLocation)" /> instead.</remarks>
  public static string GetSeasonKeyForLocation(GameLocation location)
  {
    return location?.GetSeasonKey() ?? Game1.currentSeason;
  }

  /// <summary>Unlock an achievement for the current platform.</summary>
  /// <param name="which">The achievement to unlock.</param>
  public static void getPlatformAchievement(string which) => Program.sdk.GetAchievement(which);

  public static void getSteamAchievement(string which)
  {
    if (which.Equals("0"))
      which = "a0";
    Game1.getPlatformAchievement(which);
  }

  public static void getAchievement(int which, bool allowBroadcasting = true)
  {
    string str1;
    if (Game1.player.achievements.Contains(which) || Game1.gameMode != (byte) 3 || !Game1.achievements.TryGetValue(which, out str1))
      return;
    string achievementName = str1.Split('^')[0];
    Game1.player.achievements.Add(which);
    if (which < 32 /*0x20*/ & allowBroadcasting)
    {
      if (Game1.stats.isSharedAchievement(which))
      {
        Game1.multiplayer.sendSharedAchievementMessage(which);
      }
      else
      {
        string str2 = Game1.player.Name;
        if (str2 == "")
          str2 = TokenStringBuilder.LocalizedText("Strings\\UI:Chat_PlayerJoinedNewName");
        Game1.multiplayer.globalChatInfoMessage("Achievement", str2, TokenStringBuilder.AchievementName(which));
      }
    }
    Game1.playSound("achievement");
    Game1.addHUDMessage(HUDMessage.ForAchievement(achievementName));
    Game1.player.autoGenerateActiveDialogueEvent("achievement_" + which.ToString());
    Game1.getPlatformAchievement(which.ToString());
    if (Game1.player.hasOrWillReceiveMail("hatter"))
      return;
    Game1.addMailForTomorrow("hatter");
  }

  public static void createMultipleObjectDebris(string id, int xTile, int yTile, int number)
  {
    for (int index = 0; index < number; ++index)
      Game1.createObjectDebris(id, xTile, yTile);
  }

  public static void createMultipleObjectDebris(
    string id,
    int xTile,
    int yTile,
    int number,
    GameLocation location)
  {
    for (int index = 0; index < number; ++index)
      Game1.createObjectDebris(id, xTile, yTile, -1, 0, 1f, location);
  }

  public static void createMultipleObjectDebris(
    string id,
    int xTile,
    int yTile,
    int number,
    float velocityMultiplier)
  {
    for (int index = 0; index < number; ++index)
      Game1.createObjectDebris(id, xTile, yTile, velocityMultiplyer: velocityMultiplier);
  }

  public static void createMultipleObjectDebris(
    string id,
    int xTile,
    int yTile,
    int number,
    long who)
  {
    for (int index = 0; index < number; ++index)
      Game1.createObjectDebris(id, xTile, yTile, who);
  }

  public static void createMultipleObjectDebris(
    string id,
    int xTile,
    int yTile,
    int number,
    long who,
    GameLocation location)
  {
    for (int index = 0; index < number; ++index)
      Game1.createObjectDebris(id, xTile, yTile, who, location);
  }

  public static void createDebris(int debrisType, int xTile, int yTile, int numberOfChunks)
  {
    Game1.createDebris(debrisType, xTile, yTile, numberOfChunks, Game1.currentLocation);
  }

  public static void createDebris(
    int debrisType,
    int xTile,
    int yTile,
    int numberOfChunks,
    GameLocation location)
  {
    if (location == null)
      location = Game1.currentLocation;
    location.debris.Add(new Debris(debrisType, numberOfChunks, new Vector2((float) (xTile * 64 /*0x40*/ + 32 /*0x20*/), (float) (yTile * 64 /*0x40*/ + 32 /*0x20*/)), Game1.player.getStandingPosition()));
  }

  public static Debris createItemDebris(
    Item item,
    Vector2 pixelOrigin,
    int direction,
    GameLocation location = null,
    int groundLevel = -1,
    bool flopFish = false)
  {
    if (location == null)
      location = Game1.currentLocation;
    Vector2 targetLocation = new Vector2(pixelOrigin.X, pixelOrigin.Y);
    switch (direction)
    {
      case -1:
        targetLocation = Game1.player.getStandingPosition();
        break;
      case 0:
        pixelOrigin.Y -= 16f + (float) Game1.recentMultiplayerRandom.Next(32 /*0x20*/);
        targetLocation.Y -= 35.2f;
        break;
      case 1:
        pixelOrigin.X += 16f;
        pixelOrigin.Y -= (float) (32 /*0x20*/ - Game1.recentMultiplayerRandom.Next(8));
        targetLocation.X += 128f;
        break;
      case 2:
        pixelOrigin.Y += (float) Game1.recentMultiplayerRandom.Next(16 /*0x10*/);
        targetLocation.Y += 64f;
        break;
      case 3:
        pixelOrigin.X -= 16f;
        pixelOrigin.Y -= (float) (32 /*0x20*/ - Game1.recentMultiplayerRandom.Next(8));
        targetLocation.X -= 128f;
        break;
    }
    Debris itemDebris = new Debris(item, pixelOrigin, targetLocation);
    if (flopFish && item.Category == -4)
      itemDebris.floppingFish.Value = true;
    if (groundLevel != -1)
      itemDebris.chunkFinalYLevel = groundLevel;
    location.debris.Add(itemDebris);
    return itemDebris;
  }

  public static void createMultipleItemDebris(
    Item item,
    Vector2 pixelOrigin,
    int direction,
    GameLocation location = null,
    int groundLevel = -1,
    bool flopFish = false)
  {
    int stack = item.Stack;
    item.Stack = 1;
    Game1.createItemDebris(item, pixelOrigin, direction == -1 ? Game1.random.Next(4) : direction, location, groundLevel, flopFish);
    for (int index = 1; index < stack; ++index)
      Game1.createItemDebris(item.getOne(), pixelOrigin, direction == -1 ? Game1.random.Next(4) : direction, location, groundLevel, flopFish);
  }

  public static void createRadialDebris(
    GameLocation location,
    int debrisType,
    int xTile,
    int yTile,
    int numberOfChunks,
    bool resource,
    int groundLevel = -1,
    bool item = false,
    Color? color = null)
  {
    if (groundLevel == -1)
      groundLevel = yTile * 64 /*0x40*/ + 32 /*0x20*/;
    Vector2 debrisOrigin = new Vector2((float) (xTile * 64 /*0x40*/ + 64 /*0x40*/), (float) (yTile * 64 /*0x40*/ + 64 /*0x40*/));
    if (item)
    {
      for (; numberOfChunks > 0; --numberOfChunks)
      {
        Vector2 vector2;
        switch (Game1.random.Next(4))
        {
          case 0:
            vector2 = new Vector2(-64f, 0.0f);
            break;
          case 1:
            vector2 = new Vector2(64f, 0.0f);
            break;
          case 2:
            vector2 = new Vector2(0.0f, 64f);
            break;
          default:
            vector2 = new Vector2(0.0f, -64f);
            break;
        }
        Item obj = ItemRegistry.Create("(O)" + debrisType.ToString());
        location.debris.Add(new Debris(obj, debrisOrigin, debrisOrigin + vector2));
      }
    }
    if (resource)
    {
      location.debris.Add(new Debris(debrisType, numberOfChunks / 4, debrisOrigin, debrisOrigin + new Vector2(-64f, 0.0f)));
      ++numberOfChunks;
      location.debris.Add(new Debris(debrisType, numberOfChunks / 4, debrisOrigin, debrisOrigin + new Vector2(64f, 0.0f)));
      ++numberOfChunks;
      location.debris.Add(new Debris(debrisType, numberOfChunks / 4, debrisOrigin, debrisOrigin + new Vector2(0.0f, -64f)));
      ++numberOfChunks;
      location.debris.Add(new Debris(debrisType, numberOfChunks / 4, debrisOrigin, debrisOrigin + new Vector2(0.0f, 64f)));
    }
    else
    {
      location.debris.Add(new Debris(debrisType, numberOfChunks / 4, debrisOrigin, debrisOrigin + new Vector2(-64f, 0.0f), groundLevel, color));
      ++numberOfChunks;
      location.debris.Add(new Debris(debrisType, numberOfChunks / 4, debrisOrigin, debrisOrigin + new Vector2(64f, 0.0f), groundLevel, color));
      ++numberOfChunks;
      location.debris.Add(new Debris(debrisType, numberOfChunks / 4, debrisOrigin, debrisOrigin + new Vector2(0.0f, -64f), groundLevel, color));
      ++numberOfChunks;
      location.debris.Add(new Debris(debrisType, numberOfChunks / 4, debrisOrigin, debrisOrigin + new Vector2(0.0f, 64f), groundLevel, color));
    }
  }

  public static void createRadialDebris(
    GameLocation location,
    string texture,
    Microsoft.Xna.Framework.Rectangle sourcerectangle,
    int xTile,
    int yTile,
    int numberOfChunks)
  {
    Game1.createRadialDebris(location, texture, sourcerectangle, xTile, yTile, numberOfChunks, yTile);
  }

  public static void createRadialDebris(
    GameLocation location,
    string texture,
    Microsoft.Xna.Framework.Rectangle sourcerectangle,
    int xTile,
    int yTile,
    int numberOfChunks,
    int groundLevelTile)
  {
    Game1.createRadialDebris(location, texture, sourcerectangle, 8, xTile * 64 /*0x40*/ + 32 /*0x20*/ + Game1.random.Next(32 /*0x20*/), yTile * 64 /*0x40*/ + 32 /*0x20*/ + Game1.random.Next(32 /*0x20*/), numberOfChunks, groundLevelTile);
  }

  public static void createRadialDebris(
    GameLocation location,
    string texture,
    Microsoft.Xna.Framework.Rectangle sourcerectangle,
    int sizeOfSourceRectSquares,
    int xPosition,
    int yPosition,
    int numberOfChunks,
    int groundLevelTile)
  {
    Vector2 debrisOrigin = new Vector2((float) xPosition, (float) yPosition);
    location.debris.Add(new Debris(texture, sourcerectangle, numberOfChunks / 4, debrisOrigin, debrisOrigin + new Vector2(-64f, 0.0f), groundLevelTile * 64 /*0x40*/, sizeOfSourceRectSquares));
    location.debris.Add(new Debris(texture, sourcerectangle, numberOfChunks / 4, debrisOrigin, debrisOrigin + new Vector2(64f, 0.0f), groundLevelTile * 64 /*0x40*/, sizeOfSourceRectSquares));
    location.debris.Add(new Debris(texture, sourcerectangle, numberOfChunks / 4, debrisOrigin, debrisOrigin + new Vector2(0.0f, -64f), groundLevelTile * 64 /*0x40*/, sizeOfSourceRectSquares));
    location.debris.Add(new Debris(texture, sourcerectangle, numberOfChunks / 4, debrisOrigin, debrisOrigin + new Vector2(0.0f, 64f), groundLevelTile * 64 /*0x40*/, sizeOfSourceRectSquares));
  }

  public static void createRadialDebris_MoreNatural(
    GameLocation location,
    string texture,
    Microsoft.Xna.Framework.Rectangle sourcerectangle,
    int sizeOfSourceRectSquares,
    int xPosition,
    int yPosition,
    int numberOfChunks,
    int groundLevel)
  {
    Vector2 debrisOrigin = new Vector2((float) xPosition, (float) yPosition);
    for (int index = 0; index < numberOfChunks; ++index)
      location.debris.Add(new Debris(texture, sourcerectangle, numberOfChunks / 4, debrisOrigin, debrisOrigin + new Vector2((float) Game1.random.Next(-64, 64 /*0x40*/), (float) Game1.random.Next(-64, 64 /*0x40*/)), groundLevel + Game1.random.Next(-32, 32 /*0x20*/), sizeOfSourceRectSquares));
  }

  public static void createRadialDebris(
    GameLocation location,
    string texture,
    Microsoft.Xna.Framework.Rectangle sourcerectangle,
    int sizeOfSourceRectSquares,
    int xPosition,
    int yPosition,
    int numberOfChunks,
    int groundLevelTile,
    Color color)
  {
    Game1.createRadialDebris(location, texture, sourcerectangle, sizeOfSourceRectSquares, xPosition, yPosition, numberOfChunks, groundLevelTile, color, 1f);
  }

  public static void createRadialDebris(
    GameLocation location,
    string texture,
    Microsoft.Xna.Framework.Rectangle sourcerectangle,
    int sizeOfSourceRectSquares,
    int xPosition,
    int yPosition,
    int numberOfChunks,
    int groundLevelTile,
    Color color,
    float scale)
  {
    Vector2 debrisOrigin = new Vector2((float) xPosition, (float) yPosition);
    for (; numberOfChunks > 0; --numberOfChunks)
    {
      switch (Game1.random.Next(4))
      {
        case 0:
          Debris debris1 = new Debris(texture, sourcerectangle, 1, debrisOrigin, debrisOrigin + new Vector2(-64f, 0.0f), groundLevelTile * 64 /*0x40*/, sizeOfSourceRectSquares);
          debris1.nonSpriteChunkColor.Value = color;
          location?.debris.Add(debris1);
          debris1.Chunks[0].scale = scale;
          break;
        case 1:
          Debris debris2 = new Debris(texture, sourcerectangle, 1, debrisOrigin, debrisOrigin + new Vector2(64f, 0.0f), groundLevelTile * 64 /*0x40*/, sizeOfSourceRectSquares);
          debris2.nonSpriteChunkColor.Value = color;
          location?.debris.Add(debris2);
          debris2.Chunks[0].scale = scale;
          break;
        case 2:
          Debris debris3 = new Debris(texture, sourcerectangle, 1, debrisOrigin, debrisOrigin + new Vector2((float) Game1.random.Next(-64, 64 /*0x40*/), -64f), groundLevelTile * 64 /*0x40*/, sizeOfSourceRectSquares);
          debris3.nonSpriteChunkColor.Value = color;
          location?.debris.Add(debris3);
          debris3.Chunks[0].scale = scale;
          break;
        case 3:
          Debris debris4 = new Debris(texture, sourcerectangle, 1, debrisOrigin, debrisOrigin + new Vector2((float) Game1.random.Next(-64, 64 /*0x40*/), 64f), groundLevelTile * 64 /*0x40*/, sizeOfSourceRectSquares);
          debris4.nonSpriteChunkColor.Value = color;
          location?.debris.Add(debris4);
          debris4.Chunks[0].scale = scale;
          break;
      }
    }
  }

  public static void createObjectDebris(string id, int xTile, int yTile, long whichPlayer)
  {
    Farmer farmer = Game1.GetPlayer(whichPlayer) ?? Game1.player;
    Game1.currentLocation.debris.Add(new Debris(id, new Vector2((float) (xTile * 64 /*0x40*/ + 32 /*0x20*/), (float) (yTile * 64 /*0x40*/ + 32 /*0x20*/)), farmer.getStandingPosition()));
  }

  public static void createObjectDebris(
    string id,
    int xTile,
    int yTile,
    long whichPlayer,
    GameLocation location)
  {
    Farmer farmer = Game1.GetPlayer(whichPlayer) ?? Game1.player;
    location.debris.Add(new Debris(id, new Vector2((float) (xTile * 64 /*0x40*/ + 32 /*0x20*/), (float) (yTile * 64 /*0x40*/ + 32 /*0x20*/)), farmer.getStandingPosition()));
  }

  public static void createObjectDebris(string id, int xTile, int yTile, GameLocation location)
  {
    Game1.createObjectDebris(id, xTile, yTile, -1, 0, 1f, location);
  }

  public static void createObjectDebris(
    string id,
    int xTile,
    int yTile,
    int groundLevel = -1,
    int itemQuality = 0,
    float velocityMultiplyer = 1f,
    GameLocation location = null)
  {
    if (location == null)
      location = Game1.currentLocation;
    Debris debris = new Debris(id, new Vector2((float) (xTile * 64 /*0x40*/ + 32 /*0x20*/), (float) (yTile * 64 /*0x40*/ + 32 /*0x20*/)), Game1.player.getStandingPosition())
    {
      itemQuality = itemQuality
    };
    foreach (Chunk chunk in debris.Chunks)
    {
      chunk.xVelocity.Value *= velocityMultiplyer;
      chunk.yVelocity.Value *= velocityMultiplyer;
    }
    if (groundLevel != -1)
      debris.chunkFinalYLevel = groundLevel;
    location.debris.Add(debris);
  }

  [Obsolete("Use GetPlayer instead. Equivalent usage: `GetPlayer(id, onlineOnly: true) ?? Game1.MasterPlayer`.")]
  public static Farmer getFarmer(long id) => Game1.GetPlayer(id, true) ?? Game1.MasterPlayer;

  [Obsolete("Use GetPlayer instead.")]
  public static Farmer getFarmerMaybeOffline(long id) => Game1.GetPlayer(id);

  /// <summary>Get the player matching a unique multiplayer ID, if it's valid.</summary>
  /// <param name="id">The unique multiplayer ID.</param>
  /// <param name="onlyOnline">Whether to only check online players.</param>
  /// <returns>Returns the matching player, or <c>null</c> if the ID isn't valid.</returns>
  public static 
  #nullable enable
  Farmer? GetPlayer(long id, bool onlyOnline = false)
  {
    if (Game1.MasterPlayer.UniqueMultiplayerID == id)
      return Game1.MasterPlayer;
    Farmer player;
    if (Game1.otherFarmers.TryGetValue(id, out player))
      return player;
    Farmer farmer;
    return !onlyOnline && Game1.netWorldState.Value.farmhandData.TryGetValue(id, out farmer) ? farmer : (Farmer) null;
  }

  /// <summary>Get all players including the host, online farmhands, and offline farmhands.</summary>
  public static 
  #nullable disable
  IEnumerable<Farmer> getAllFarmers()
  {
    return Enumerable.Repeat<Farmer>(Game1.MasterPlayer, 1).Concat<Farmer>(Game1.getAllFarmhands());
  }

  /// <summary>Get all players who are currently connected, including the host player.</summary>
  public static FarmerCollection getOnlineFarmers() => Game1._onlineFarmers;

  /// <summary>Get online and offline farmhands.</summary>
  public static IEnumerable<Farmer> getAllFarmhands()
  {
    foreach (Farmer allFarmhand in Game1.netWorldState.Value.farmhandData.Values)
    {
      if (allFarmhand.isActive())
        yield return Game1.otherFarmers[allFarmhand.UniqueMultiplayerID];
      else
        yield return allFarmhand;
    }
  }

  /// <summary>Get farmhands which aren't currently connected.</summary>
  public static IEnumerable<Farmer> getOfflineFarmhands()
  {
    foreach (Farmer offlineFarmhand in Game1.netWorldState.Value.farmhandData.Values)
    {
      if (!offlineFarmhand.isActive())
        yield return offlineFarmhand;
    }
  }

  public static void farmerFindsArtifact(string itemId)
  {
    Game1.player.addItemToInventoryBool(ItemRegistry.Create(itemId));
  }

  public static bool doesHUDMessageExist(string s)
  {
    for (int index = 0; index < Game1.hudMessages.Count; ++index)
    {
      if (s.Equals(Game1.hudMessages[index].message))
        return true;
    }
    return false;
  }

  public static void addHUDMessage(HUDMessage message)
  {
    if (message.type != null || message.whatType != 0)
    {
      for (int index = 0; index < Game1.hudMessages.Count; ++index)
      {
        if (message.type != null && message.type == Game1.hudMessages[index].type)
        {
          Game1.hudMessages[index].number += message.number;
          Game1.hudMessages[index].timeLeft = 3500f;
          Game1.hudMessages[index].transparency = 1f;
          if (Game1.hudMessages[index].number <= 50000)
            return;
          HUDMessage.numbersEasterEgg(Game1.hudMessages[index].number);
          return;
        }
        if (message.whatType == Game1.hudMessages[index].whatType && message.whatType != 1 && message.message != null && message.message.Equals(Game1.hudMessages[index].message))
        {
          Game1.hudMessages[index].timeLeft = message.timeLeft;
          Game1.hudMessages[index].transparency = 1f;
          return;
        }
      }
    }
    Game1.hudMessages.Add(message);
    for (int index = Game1.hudMessages.Count - 1; index >= 0; --index)
    {
      if (Game1.hudMessages[index].noIcon)
      {
        HUDMessage hudMessage = Game1.hudMessages[index];
        Game1.hudMessages.RemoveAt(index);
        Game1.hudMessages.Add(hudMessage);
      }
    }
  }

  public static void showSwordswipeAnimation(
    int direction,
    Vector2 source,
    float animationSpeed,
    bool flip)
  {
    switch (direction)
    {
      case 0:
        Game1.currentLocation.TemporarySprites.Add(new TemporaryAnimatedSprite(-1, animationSpeed, 5, 1, new Vector2(source.X + 32f, source.Y), false, false, !flip, -1.57079637f));
        break;
      case 1:
        Game1.currentLocation.TemporarySprites.Add(new TemporaryAnimatedSprite(-1, animationSpeed, 5, 1, new Vector2((float) ((double) source.X + 96.0 + 16.0), source.Y + 48f), false, flip, false, flip ? -3.14159274f : 0.0f));
        break;
      case 2:
        Game1.currentLocation.TemporarySprites.Add(new TemporaryAnimatedSprite(-1, animationSpeed, 5, 1, new Vector2(source.X + 32f, source.Y + 128f), false, false, !flip, 1.57079637f));
        break;
      case 3:
        Game1.currentLocation.TemporarySprites.Add(new TemporaryAnimatedSprite(-1, animationSpeed, 5, 1, new Vector2((float) ((double) source.X - 32.0 - 16.0), source.Y + 48f), false, !flip, false, flip ? -3.14159274f : 0.0f));
        break;
    }
  }

  public static void removeDebris(Debris.DebrisType type)
  {
    Game1.currentLocation.debris.RemoveWhere((Func<Debris, bool>) (debris => debris.debrisType.Value == type));
  }

  public static void toolAnimationDone(Farmer who)
  {
    float stamina = Game1.player.Stamina;
    if (who.CurrentTool == null)
      return;
    if ((double) who.Stamina > 0.0)
    {
      int power = 1;
      Vector2 toolLocation = who.GetToolLocation();
      if (who.CurrentTool is FishingRod currentTool && currentTool.isFishing)
        who.canReleaseTool = false;
      else if (!(who.CurrentTool is FishingRod))
      {
        who.UsingTool = false;
        if (who.CurrentTool.QualifiedItemId == "(T)WateringCan")
        {
          switch (who.FacingDirection)
          {
            case 0:
            case 2:
              who.CurrentTool.DoFunction(Game1.currentLocation, (int) toolLocation.X, (int) toolLocation.Y, power, who);
              break;
            case 1:
            case 3:
              who.CurrentTool.DoFunction(Game1.currentLocation, (int) toolLocation.X, (int) toolLocation.Y, power, who);
              break;
          }
        }
        else if (who.CurrentTool is MeleeWeapon)
        {
          who.CurrentTool.CurrentParentTileIndex = who.CurrentTool.IndexOfMenuItemView;
        }
        else
        {
          if (who.CurrentTool.QualifiedItemId == "(T)ReturnScepter")
            who.CurrentTool.CurrentParentTileIndex = who.CurrentTool.IndexOfMenuItemView;
          who.CurrentTool.DoFunction(Game1.currentLocation, (int) toolLocation.X, (int) toolLocation.Y, power, who);
        }
      }
      else
        who.UsingTool = false;
    }
    else if (who.CurrentTool.instantUse.Value)
      who.CurrentTool.DoFunction(Game1.currentLocation, 0, 0, 0, who);
    else
      who.UsingTool = false;
    who.lastClick = Vector2.Zero;
    if (who.IsLocalPlayer && !Game1.GetKeyboardState().IsKeyDown(Keys.LeftShift))
      who.setRunning(Game1.options.autoRun);
    if (!who.UsingTool && who.FarmerSprite.PauseForSingleAnimation)
      who.FarmerSprite.StopAnimation();
    if ((double) Game1.player.Stamina > 0.0 || (double) stamina <= 0.0)
      return;
    Game1.player.doEmote(36);
  }

  public static bool pressActionButton(
    KeyboardState currentKBState,
    MouseState currentMouseState,
    GamePadState currentPadState)
  {
    if (Game1.IsChatting)
      currentKBState = new KeyboardState();
    if (Game1.dialogueTyping)
    {
      bool flag = true;
      Game1.dialogueTyping = false;
      if (Game1.currentSpeaker != null)
        Game1.currentDialogueCharacterIndex = Game1.currentSpeaker.CurrentDialogue.Peek().getCurrentDialogue().Length;
      else if (Game1.currentObjectDialogue.Count > 0)
        Game1.currentDialogueCharacterIndex = Game1.currentObjectDialogue.Peek().Length;
      else
        flag = false;
      Game1.dialogueTypingInterval = 0;
      Game1.oldKBState = currentKBState;
      Game1.oldMouseState = Game1.input.GetMouseState();
      Game1.oldPadState = currentPadState;
      if (flag)
      {
        Game1.playSound("dialogueCharacterClose");
        return false;
      }
    }
    if (Game1.dialogueUp)
    {
      if (Game1.isQuestion)
      {
        Game1.isQuestion = false;
        if (Game1.currentSpeaker != null)
        {
          if (Game1.currentSpeaker.CurrentDialogue.Peek().chooseResponse(Game1.questionChoices[Game1.currentQuestionChoice]))
          {
            Game1.currentDialogueCharacterIndex = 1;
            Game1.dialogueTyping = true;
            Game1.oldKBState = currentKBState;
            Game1.oldMouseState = Game1.input.GetMouseState();
            Game1.oldPadState = currentPadState;
            return false;
          }
        }
        else
        {
          Game1.dialogueUp = false;
          if (Game1.eventUp && Game1.currentLocation.afterQuestion == null)
          {
            Game1.currentLocation.currentEvent.answerDialogue(Game1.currentLocation.lastQuestionKey, Game1.currentQuestionChoice);
            Game1.currentQuestionChoice = 0;
            Game1.oldKBState = currentKBState;
            Game1.oldMouseState = Game1.input.GetMouseState();
            Game1.oldPadState = currentPadState;
          }
          else if (Game1.currentLocation.answerDialogue(Game1.questionChoices[Game1.currentQuestionChoice]))
          {
            Game1.currentQuestionChoice = 0;
            Game1.oldKBState = currentKBState;
            Game1.oldMouseState = Game1.input.GetMouseState();
            Game1.oldPadState = currentPadState;
            return false;
          }
          if (Game1.dialogueUp)
          {
            Game1.currentDialogueCharacterIndex = 1;
            Game1.dialogueTyping = true;
            Game1.oldKBState = currentKBState;
            Game1.oldMouseState = Game1.input.GetMouseState();
            Game1.oldPadState = currentPadState;
            return false;
          }
        }
        Game1.currentQuestionChoice = 0;
      }
      string str = (string) null;
      if (Game1.currentSpeaker != null)
      {
        if (!Game1.currentSpeaker.immediateSpeak)
        {
          str = Game1.currentSpeaker.CurrentDialogue.Count > 0 ? Game1.currentSpeaker.CurrentDialogue.Peek().exitCurrentDialogue() : (string) null;
        }
        else
        {
          Game1.currentSpeaker.immediateSpeak = false;
          return false;
        }
      }
      if (str == null)
      {
        if (Game1.currentSpeaker != null && Game1.currentSpeaker.CurrentDialogue.Count > 0 && Game1.currentSpeaker.CurrentDialogue.Peek().isOnFinalDialogue() && Game1.currentSpeaker.CurrentDialogue.Count > 0)
          Game1.currentSpeaker.CurrentDialogue.Pop();
        Game1.dialogueUp = false;
        if (Game1.messagePause)
          Game1.pauseTime = 500f;
        if (Game1.currentObjectDialogue.Count > 0)
          Game1.currentObjectDialogue.Dequeue();
        Game1.currentDialogueCharacterIndex = 0;
        if (Game1.currentObjectDialogue.Count > 0)
        {
          Game1.dialogueUp = true;
          Game1.questionChoices.Clear();
          Game1.oldKBState = currentKBState;
          Game1.oldMouseState = Game1.input.GetMouseState();
          Game1.oldPadState = currentPadState;
          Game1.dialogueTyping = true;
          return false;
        }
        if (Game1.currentSpeaker != null && !Game1.currentSpeaker.Name.Equals("Gunther") && !Game1.eventUp && !Game1.currentSpeaker.doingEndOfRouteAnimation.Value)
          Game1.currentSpeaker.doneFacingPlayer(Game1.player);
        Game1.currentSpeaker = (NPC) null;
        if (!Game1.eventUp)
          Game1.player.CanMove = true;
        else if (Game1.currentLocation.currentEvent.CurrentCommand > 0 || Game1.currentLocation.currentEvent.specialEventVariable1)
        {
          if (!Game1.isFestival() || !Game1.currentLocation.currentEvent.canMoveAfterDialogue())
            ++Game1.currentLocation.currentEvent.CurrentCommand;
          else
            Game1.player.CanMove = true;
        }
        Game1.questionChoices.Clear();
        Game1.playSound("smallSelect");
      }
      else
      {
        Game1.playSound("smallSelect");
        Game1.currentDialogueCharacterIndex = 0;
        Game1.dialogueTyping = true;
        Game1.checkIfDialogueIsQuestion();
      }
      Game1.oldKBState = currentKBState;
      Game1.oldMouseState = Game1.input.GetMouseState();
      Game1.oldPadState = currentPadState;
      return false;
    }
    if (!Game1.player.UsingTool && (!Game1.eventUp || Game1.currentLocation.currentEvent != null && Game1.currentLocation.currentEvent.playerControlSequence) && !Game1.fadeToBlack)
    {
      if (Game1.wasMouseVisibleThisFrame && Game1.currentLocation.animals.Length > 0)
      {
        Vector2 position = new Vector2((float) (Game1.getOldMouseX() + Game1.viewport.X), (float) (Game1.getOldMouseY() + Game1.viewport.Y));
        if (Utility.withinRadiusOfPlayer((int) position.X, (int) position.Y, 1, Game1.player) && (Game1.currentLocation.CheckPetAnimal(position, Game1.player) || Game1.didPlayerJustRightClick(true) && Game1.currentLocation.CheckInspectAnimal(position, Game1.player)))
          return true;
      }
      Vector2 vector2_1 = new Vector2((float) (Game1.getOldMouseX() + Game1.viewport.X), (float) (Game1.getOldMouseY() + Game1.viewport.Y)) / 64f;
      Vector2 vector2_2 = vector2_1;
      bool flag1 = false;
      if (!Game1.wasMouseVisibleThisFrame || (double) Game1.mouseCursorTransparency == 0.0 || !Utility.tileWithinRadiusOfPlayer((int) vector2_1.X, (int) vector2_1.Y, 1, Game1.player))
      {
        vector2_1 = Game1.player.GetGrabTile();
        flag1 = true;
      }
      bool flag2 = false;
      if (!Game1.eventUp || Game1.isFestival())
      {
        if (Game1.tryToCheckAt(vector2_1, Game1.player))
          return false;
        if (Game1.player.isRidingHorse())
        {
          Game1.player.mount.checkAction(Game1.player, Game1.player.currentLocation);
          return false;
        }
        if (!Game1.player.canMove)
          return false;
        if (!flag2 && Game1.player.currentLocation.isCharacterAtTile(vector2_1) != null)
          flag2 = true;
        bool flag3 = false;
        if (Game1.player.ActiveObject != null && !(Game1.player.ActiveObject is Furniture))
        {
          if (Game1.player.ActiveObject.performUseAction(Game1.currentLocation))
          {
            Game1.player.reduceActiveItemByOne();
            Game1.oldKBState = currentKBState;
            Game1.oldMouseState = Game1.input.GetMouseState();
            Game1.oldPadState = currentPadState;
            return false;
          }
          int stack = Game1.player.ActiveObject.Stack;
          Game1.isCheckingNonMousePlacement = !Game1.IsPerformingMousePlacement();
          if (flag1)
            Game1.isCheckingNonMousePlacement = true;
          if (Game1.isOneOfTheseKeysDown(currentKBState, Game1.options.actionButton))
            Game1.isCheckingNonMousePlacement = true;
          Vector2 placementPosition = Utility.GetNearbyValidPlacementPosition(Game1.player, Game1.currentLocation, (Item) Game1.player.ActiveObject, (int) vector2_1.X * 64 /*0x40*/ + 32 /*0x20*/, (int) vector2_1.Y * 64 /*0x40*/ + 32 /*0x20*/);
          if (!Game1.isCheckingNonMousePlacement && Game1.player.ActiveObject is Wallpaper && Utility.tryToPlaceItem(Game1.currentLocation, Game1.player.ActiveObject, (int) vector2_2.X * 64 /*0x40*/, (int) vector2_2.Y * 64 /*0x40*/))
          {
            Game1.isCheckingNonMousePlacement = false;
            return true;
          }
          if (Utility.tryToPlaceItem(Game1.currentLocation, Game1.player.ActiveObject, (int) placementPosition.X, (int) placementPosition.Y))
          {
            Game1.isCheckingNonMousePlacement = false;
            return true;
          }
          if (!Game1.eventUp && (Game1.player.ActiveObject == null || Game1.player.ActiveObject.Stack < stack || Game1.player.ActiveObject.isPlaceable()))
            flag3 = true;
          Game1.isCheckingNonMousePlacement = false;
        }
        if (!flag3 && !flag2)
        {
          ++vector2_1.Y;
          if (Game1.player.FacingDirection >= 0 && Game1.player.FacingDirection <= 3)
          {
            Vector2 vector2_3 = vector2_1 - Game1.player.Tile;
            if ((double) vector2_3.X > 0.0 || (double) vector2_3.Y > 0.0)
              vector2_3.Normalize();
            if ((double) Vector2.Dot(Utility.DirectionsTileVectors[Game1.player.FacingDirection], vector2_3) >= 0.0 && Game1.tryToCheckAt(vector2_1, Game1.player))
              return false;
          }
          if (!Game1.eventUp && Game1.player.ActiveObject is Furniture activeObject1)
          {
            activeObject1.rotate();
            Game1.playSound("dwoop");
            Game1.oldKBState = currentKBState;
            Game1.oldMouseState = Game1.input.GetMouseState();
            Game1.oldPadState = currentPadState;
            return false;
          }
          vector2_1.Y -= 2f;
          if (Game1.player.FacingDirection >= 0 && Game1.player.FacingDirection <= 3 && !flag2)
          {
            Vector2 vector2_4 = vector2_1 - Game1.player.Tile;
            if ((double) vector2_4.X > 0.0 || (double) vector2_4.Y > 0.0)
              vector2_4.Normalize();
            if ((double) Vector2.Dot(Utility.DirectionsTileVectors[Game1.player.FacingDirection], vector2_4) >= 0.0 && Game1.tryToCheckAt(vector2_1, Game1.player))
              return false;
          }
          if (!Game1.eventUp && Game1.player.ActiveObject is Furniture activeObject2)
          {
            activeObject2.rotate();
            Game1.playSound("dwoop");
            Game1.oldKBState = currentKBState;
            Game1.oldMouseState = Game1.input.GetMouseState();
            Game1.oldPadState = currentPadState;
            return false;
          }
          if (Game1.tryToCheckAt(Game1.player.Tile, Game1.player))
            return false;
          if (!Game1.eventUp && Game1.player.ActiveObject is Furniture activeObject3)
          {
            activeObject3.rotate();
            Game1.playSound("dwoop");
            Game1.oldKBState = currentKBState;
            Game1.oldMouseState = Game1.input.GetMouseState();
            Game1.oldPadState = currentPadState;
            return false;
          }
        }
        if (!Game1.player.isEating && Game1.player.ActiveObject != null && !Game1.dialogueUp && !Game1.eventUp && !Game1.player.canOnlyWalk && !Game1.player.FarmerSprite.PauseForSingleAnimation && !Game1.fadeToBlack && Game1.player.ActiveObject.Edibility != -300 && Game1.didPlayerJustRightClick(true))
        {
          if (Game1.player.team.SpecialOrderRuleActive("SC_NO_FOOD") && (Game1.player.currentLocation is MineShaft currentLocation ? (currentLocation.getMineArea() == 121 ? 1 : 0) : 0) != 0)
          {
            Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.13053"), 3));
            return false;
          }
          if (Game1.player.hasBuff("25") && Game1.player.ActiveObject != null && !Game1.player.ActiveObject.HasContextTag("ginger_item"))
          {
            Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Nauseous_CantEat"), 3));
            return false;
          }
          Game1.player.faceDirection(2);
          Game1.player.itemToEat = (Item) Game1.player.ActiveObject;
          Game1.player.FarmerSprite.setCurrentSingleAnimation(304);
          ObjectData objectData;
          if (Game1.objectData.TryGetValue(Game1.player.ActiveObject.ItemId, out objectData))
            Game1.currentLocation.createQuestionDialogue(!objectData.IsDrink || Game1.player.ActiveObject.preserve.Value.GetValueOrDefault() == Object.PreserveType.Pickle ? Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3160", (object) Game1.player.ActiveObject.DisplayName) : Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3159", (object) Game1.player.ActiveObject.DisplayName), Game1.currentLocation.createYesNoResponses(), "Eat");
          Game1.oldKBState = currentKBState;
          Game1.oldMouseState = Game1.input.GetMouseState();
          Game1.oldPadState = currentPadState;
          return false;
        }
      }
      else
      {
        Game1.CurrentEvent?.receiveActionPress((int) vector2_1.X, (int) vector2_1.Y);
        Game1.oldKBState = currentKBState;
        Game1.oldMouseState = Game1.input.GetMouseState();
        Game1.oldPadState = currentPadState;
        return false;
      }
    }
    if (!(Game1.player.CurrentTool is MeleeWeapon) || !Game1.player.CanMove || Game1.player.canOnlyWalk || Game1.eventUp || Game1.player.onBridge.Value || !Game1.didPlayerJustRightClick(true))
      return true;
    ((MeleeWeapon) Game1.player.CurrentTool).animateSpecialMove(Game1.player);
    return false;
  }

  public static bool IsPerformingMousePlacement()
  {
    return (double) Game1.mouseCursorTransparency != 0.0 && Game1.wasMouseVisibleThisFrame && (Game1.lastCursorMotionWasMouse || Game1.player.ActiveObject != null && (Game1.player.ActiveObject.isPlaceable() || Game1.player.ActiveObject.Category == -74 || Game1.player.ActiveObject.isSapling()));
  }

  public static Vector2 GetPlacementGrabTile()
  {
    return !Game1.IsPerformingMousePlacement() ? Game1.player.GetGrabTile() : new Vector2((float) (Game1.getOldMouseX() + Game1.viewport.X), (float) (Game1.getOldMouseY() + Game1.viewport.Y)) / 64f;
  }

  public static bool tryToCheckAt(Vector2 grabTile, Farmer who)
  {
    if (Game1.player.onBridge.Value)
      return false;
    Game1.haltAfterCheck = true;
    if (!Utility.tileWithinRadiusOfPlayer((int) grabTile.X, (int) grabTile.Y, 1, Game1.player) || !Game1.hooks.OnGameLocation_CheckAction(Game1.currentLocation, new Location((int) grabTile.X, (int) grabTile.Y), Game1.viewport, who, (Func<bool>) (() => Game1.currentLocation.checkAction(new Location((int) grabTile.X, (int) grabTile.Y), Game1.viewport, who))))
      return false;
    Game1.updateCursorTileHint();
    who.lastGrabTile = grabTile;
    if (who.CanMove && Game1.haltAfterCheck)
    {
      who.faceGeneralDirection(grabTile * 64f);
      who.Halt();
    }
    Game1.oldKBState = Game1.GetKeyboardState();
    Game1.oldMouseState = Game1.input.GetMouseState();
    Game1.oldPadState = Game1.input.GetGamePadState();
    return true;
  }

  public static void pressSwitchToolButton()
  {
    if (Game1.player.netItemStowed.Value)
    {
      Game1.player.netItemStowed.Set(false);
      Game1.player.UpdateItemStow();
    }
    MouseState mouseState = Game1.input.GetMouseState();
    int num1;
    if (mouseState.ScrollWheelValue <= Game1.oldMouseState.ScrollWheelValue)
    {
      mouseState = Game1.input.GetMouseState();
      num1 = mouseState.ScrollWheelValue < Game1.oldMouseState.ScrollWheelValue ? 1 : 0;
    }
    else
      num1 = -1;
    int num2 = num1;
    if (Game1.options.gamepadControls && num2 == 0)
    {
      GamePadState gamePadState = Game1.input.GetGamePadState();
      if (gamePadState.IsButtonDown(Buttons.LeftTrigger))
      {
        num2 = -1;
      }
      else
      {
        gamePadState = Game1.input.GetGamePadState();
        if (gamePadState.IsButtonDown(Buttons.RightTrigger))
          num2 = 1;
      }
    }
    if (Game1.options.invertScrollDirection)
      num2 *= -1;
    if (num2 == 0)
      return;
    Game1.player.CurrentToolIndex = (Game1.player.CurrentToolIndex + num2) % 12;
    if (Game1.player.CurrentToolIndex < 0)
      Game1.player.CurrentToolIndex = 11;
    for (int index = 0; index < 12 && Game1.player.CurrentItem == null; ++index)
    {
      Game1.player.CurrentToolIndex = (num2 + Game1.player.CurrentToolIndex) % 12;
      if (Game1.player.CurrentToolIndex < 0)
        Game1.player.CurrentToolIndex = 11;
    }
    Game1.playSound("toolSwap");
    if (Game1.player.ActiveObject != null)
      Game1.player.showCarrying();
    else
      Game1.player.showNotCarrying();
  }

  public static bool pressUseToolButton()
  {
    bool initiateItemStow = Game1.game1._didInitiateItemStow;
    Game1.game1._didInitiateItemStow = false;
    if (Game1.fadeToBlack)
      return false;
    Game1.player.toolPower.Value = 0;
    Game1.player.toolHold.Value = 0;
    bool flag = false;
    if (Game1.player.CurrentTool == null && Game1.player.ActiveObject == null)
    {
      Vector2 key = Game1.player.GetToolLocation() / 64f;
      key.X = (float) (int) key.X;
      key.Y = (float) (int) key.Y;
      Object @object;
      if (Game1.currentLocation.Objects.TryGetValue(key, out @object) && !@object.readyForHarvest.Value && @object.heldObject.Value == null && !(@object is Fence) && !(@object is CrabPot) && (@object.Type == "Crafting" || @object.Type == "interactive") && !@object.IsTwig())
      {
        flag = true;
        @object.setHealth(@object.getHealth() - 1);
        @object.shakeTimer = 300;
        @object.playNearbySoundAll("hammer");
        if (@object.getHealth() < 2)
        {
          @object.playNearbySoundAll("hammer");
          if (@object.getHealth() < 1)
          {
            Tool t = ItemRegistry.Create<Tool>("(T)Pickaxe");
            t.DoFunction(Game1.currentLocation, -1, -1, 0, Game1.player);
            if (@object.performToolAction(t))
            {
              @object.performRemoveAction();
              if (@object.Type == "Crafting" && @object.fragility.Value != 2)
                Game1.currentLocation.debris.Add(new Debris(@object.QualifiedItemId, Game1.player.GetToolLocation(), Utility.PointToVector2(Game1.player.StandingPixel)));
              Game1.currentLocation.Objects.Remove(key);
              return true;
            }
          }
        }
      }
    }
    if (Game1.currentMinigame == null && !Game1.player.UsingTool)
    {
      if (!Game1.player.IsSitting() && !Game1.player.isRidingHorse() && !Game1.player.onBridge.Value && !Game1.dialogueUp && (!Game1.eventUp || Game1.CurrentEvent.canPlayerUseTool() || Game1.currentLocation.currentEvent.playerControlSequence && (Game1.activeClickableMenu != null || Game1.currentMinigame != null)))
      {
        if (Game1.player.CurrentTool != null)
        {
          bool? isVillager = Game1.currentLocation.doesPositionCollideWithCharacter(Utility.getRectangleCenteredAt(Game1.player.GetToolLocation(), 64 /*0x40*/), true)?.IsVillager;
          if (!isVillager.HasValue || !isVillager.GetValueOrDefault())
            goto label_15;
        }
        else
          goto label_15;
      }
      Game1.pressActionButton(Game1.GetKeyboardState(), Game1.input.GetMouseState(), Game1.input.GetGamePadState());
      return false;
    }
label_15:
    if (Game1.player.canOnlyWalk)
      return true;
    Vector2 position = !Game1.wasMouseVisibleThisFrame ? Game1.player.GetToolLocation() : new Vector2((float) (Game1.getOldMouseX() + Game1.viewport.X), (float) (Game1.getOldMouseY() + Game1.viewport.Y));
    if (Utility.canGrabSomethingFromHere((int) position.X, (int) position.Y, Game1.player))
    {
      Vector2 tile = new Vector2(position.X / 64f, position.Y / 64f);
      if (Game1.hooks.OnGameLocation_CheckAction(Game1.currentLocation, new Location((int) tile.X, (int) tile.Y), Game1.viewport, Game1.player, (Func<bool>) (() => Game1.currentLocation.checkAction(new Location((int) tile.X, (int) tile.Y), Game1.viewport, Game1.player))))
      {
        Game1.updateCursorTileHint();
        return true;
      }
      TerrainFeature terrainFeature;
      if (!Game1.currentLocation.terrainFeatures.TryGetValue(tile, out terrainFeature))
        return false;
      terrainFeature.performUseAction(tile);
      return true;
    }
    if (Game1.currentLocation.leftClick((int) position.X, (int) position.Y, Game1.player))
      return true;
    Game1.isCheckingNonMousePlacement = !Game1.IsPerformingMousePlacement();
    if (Game1.player.ActiveObject != null)
    {
      if (Game1.options.allowStowing && Game1.CanPlayerStowItem(Game1.GetPlacementGrabTile()))
      {
        if (!(Game1.didPlayerJustLeftClick() | initiateItemStow))
          return true;
        Game1.game1._didInitiateItemStow = true;
        Game1.playSound("stoneStep");
        Game1.player.netItemStowed.Set(true);
        return true;
      }
      if (Utility.withinRadiusOfPlayer((int) position.X, (int) position.Y, 1, Game1.player) && Game1.hooks.OnGameLocation_CheckAction(Game1.currentLocation, new Location((int) position.X / 64 /*0x40*/, (int) position.Y / 64 /*0x40*/), Game1.viewport, Game1.player, (Func<bool>) (() => Game1.currentLocation.checkAction(new Location((int) position.X / 64 /*0x40*/, (int) position.Y / 64 /*0x40*/), Game1.viewport, Game1.player))))
        return true;
      Vector2 placementGrabTile = Game1.GetPlacementGrabTile();
      Vector2 placementPosition = Utility.GetNearbyValidPlacementPosition(Game1.player, Game1.currentLocation, (Item) Game1.player.ActiveObject, (int) placementGrabTile.X * 64 /*0x40*/, (int) placementGrabTile.Y * 64 /*0x40*/);
      if (Utility.tryToPlaceItem(Game1.currentLocation, Game1.player.ActiveObject, (int) placementPosition.X, (int) placementPosition.Y))
      {
        Game1.isCheckingNonMousePlacement = false;
        return true;
      }
      Game1.isCheckingNonMousePlacement = false;
    }
    if (Game1.currentLocation.LowPriorityLeftClick((int) position.X, (int) position.Y, Game1.player))
      return true;
    if (Game1.options.allowStowing && Game1.player.netItemStowed.Value && !flag && (initiateItemStow || Game1.didPlayerJustLeftClick(true)))
    {
      Game1.game1._didInitiateItemStow = true;
      Game1.playSound("toolSwap");
      Game1.player.netItemStowed.Set(false);
      return true;
    }
    if (Game1.player.UsingTool)
    {
      Game1.player.lastClick = new Vector2((float) (int) position.X, (float) (int) position.Y);
      Game1.player.CurrentTool.DoFunction(Game1.player.currentLocation, (int) Game1.player.lastClick.X, (int) Game1.player.lastClick.Y, 1, Game1.player);
      return true;
    }
    if (Game1.player.ActiveObject == null && !Game1.player.isEating && Game1.player.CurrentTool != null)
    {
      if ((double) Game1.player.Stamina <= 20.0 && Game1.player.CurrentTool != null && !(Game1.player.CurrentTool is MeleeWeapon) && !Game1.eventUp)
      {
        Game1.staminaShakeTimer = 1000;
        for (int index = 0; index < 4; ++index)
          Game1.uiOverlayTempSprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(366, 412, 5, 6), new Vector2((float) (Game1.random.Next(32 /*0x20*/) + Game1.uiViewport.Width - 56), (float) (Game1.uiViewport.Height - 224 /*0xE0*/ - 16 /*0x10*/ - (int) ((double) (Game1.player.MaxStamina - 270) * 0.715))), false, 0.012f, Color.SkyBlue)
          {
            motion = new Vector2(-2f, -10f),
            acceleration = new Vector2(0.0f, 0.5f),
            local = true,
            scale = (float) (4 + Game1.random.Next(-1, 0)),
            delayBeforeAnimationStart = index * 30
          });
      }
      if (!(Game1.player.CurrentTool is MeleeWeapon) || Game1.didPlayerJustLeftClick(true))
      {
        int facingDirection = Game1.player.FacingDirection;
        Vector2 toolLocation = Game1.player.GetToolLocation(position);
        Game1.player.FacingDirection = Game1.player.getGeneralDirectionTowards(new Vector2((float) (int) toolLocation.X, (float) (int) toolLocation.Y));
        Game1.player.lastClick = new Vector2((float) (int) position.X, (float) (int) position.Y);
        Game1.player.BeginUsingTool();
        if (!Game1.player.usingTool.Value)
          Game1.player.FacingDirection = facingDirection;
        else if (Game1.player.FarmerSprite.IsPlayingBasicAnimation(facingDirection, true) || Game1.player.FarmerSprite.IsPlayingBasicAnimation(facingDirection, false))
          Game1.player.FarmerSprite.StopAnimation();
      }
    }
    return false;
  }

  public static bool CanPlayerStowItem(Vector2 position)
  {
    if (Game1.player.ActiveObject == null || Game1.player.ActiveObject.bigCraftable.Value)
      return false;
    Object activeObject = Game1.player.ActiveObject;
    if (activeObject is Furniture)
      return false;
    if (activeObject != null && (Game1.player.ActiveObject.Category == -74 || Game1.player.ActiveObject.Category == -19))
    {
      Vector2 placementPosition = Utility.GetNearbyValidPlacementPosition(Game1.player, Game1.currentLocation, (Item) Game1.player.ActiveObject, (int) position.X * 64 /*0x40*/, (int) position.Y * 64 /*0x40*/);
      if (Utility.playerCanPlaceItemHere(Game1.player.currentLocation, (Item) Game1.player.ActiveObject, (int) placementPosition.X, (int) placementPosition.Y, Game1.player) && (!Game1.player.ActiveObject.isSapling() || Game1.IsPerformingMousePlacement()))
        return false;
    }
    return true;
  }

  public static int getMouseXRaw() => Game1.input.GetMouseState().X;

  public static int getMouseYRaw() => Game1.input.GetMouseState().Y;

  public static bool IsOnMainThread()
  {
    return Thread.CurrentThread != null && !Thread.CurrentThread.IsBackground;
  }

  public static void PushUIMode()
  {
    if (!Game1.IsOnMainThread())
      return;
    ++Game1.uiModeCount;
    if (Game1.uiModeCount <= 0 || Game1.uiMode)
      return;
    Game1.uiMode = true;
    if (Game1.game1.isDrawing && Game1.IsOnMainThread())
    {
      if (Game1.game1.uiScreen != null && !Game1.game1.uiScreen.IsDisposed)
      {
        RenderTargetBinding[] renderTargets = Game1.graphics.GraphicsDevice.GetRenderTargets();
        Game1.nonUIRenderTarget = renderTargets.Length == 0 ? (RenderTarget2D) null : renderTargets[0].RenderTarget as RenderTarget2D;
        Game1.SetRenderTarget(Game1.game1.uiScreen);
      }
      if (Game1.isRenderingScreenBuffer)
        Game1.SetRenderTarget((RenderTarget2D) null);
    }
    Game1.uiViewport = new xTile.Dimensions.Rectangle(0, 0, (int) Math.Ceiling((double) Game1.viewport.Width * (double) Game1.options.zoomLevel / (double) Game1.options.uiScale), (int) Math.Ceiling((double) Game1.viewport.Height * (double) Game1.options.zoomLevel / (double) Game1.options.uiScale))
    {
      X = Game1.viewport.X,
      Y = Game1.viewport.Y
    };
  }

  public static void PopUIMode()
  {
    if (!Game1.IsOnMainThread())
      return;
    --Game1.uiModeCount;
    if (Game1.uiModeCount > 0 || !Game1.uiMode)
      return;
    if (Game1.game1.isDrawing)
    {
      if (Game1.graphics.GraphicsDevice.GetRenderTargets().Length != 0 && Game1.graphics.GraphicsDevice.GetRenderTargets()[0].RenderTarget == Game1.game1.uiScreen)
      {
        if (Game1.nonUIRenderTarget != null && !Game1.nonUIRenderTarget.IsDisposed)
          Game1.SetRenderTarget(Game1.nonUIRenderTarget);
        else
          Game1.SetRenderTarget((RenderTarget2D) null);
      }
      if (Game1.isRenderingScreenBuffer)
        Game1.SetRenderTarget((RenderTarget2D) null);
    }
    Game1.nonUIRenderTarget = (RenderTarget2D) null;
    Game1.uiMode = false;
  }

  public static void SetRenderTarget(RenderTarget2D target)
  {
    if (Game1.isRenderingScreenBuffer || !Game1.IsOnMainThread())
      return;
    Game1.graphics.GraphicsDevice.SetRenderTarget(target);
  }

  public static void InUIMode(Action action)
  {
    Game1.PushUIMode();
    try
    {
      action();
    }
    finally
    {
      Game1.PopUIMode();
    }
  }

  public static void StartWorldDrawInUI(SpriteBatch b)
  {
    Game1._oldUIModeCount = 0;
    if (!Game1.uiMode)
      return;
    Game1._oldUIModeCount = Game1.uiModeCount;
    b?.End();
    while (Game1.uiModeCount > 0)
      Game1.PopUIMode();
    b?.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
  }

  public static void EndWorldDrawInUI(SpriteBatch b)
  {
    if (Game1._oldUIModeCount > 0)
    {
      b?.End();
      for (int index = 0; index < Game1._oldUIModeCount; ++index)
        Game1.PushUIMode();
      b?.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
    }
    Game1._oldUIModeCount = 0;
  }

  public static int getMouseX() => Game1.getMouseX(Game1.uiMode);

  public static int getMouseX(bool ui_scale)
  {
    return ui_scale ? (int) ((double) Game1.input.GetMouseState().X / (double) Game1.options.uiScale) : (int) ((double) Game1.input.GetMouseState().X * (1.0 / (double) Game1.options.zoomLevel));
  }

  public static int getOldMouseX() => Game1.getOldMouseX(Game1.uiMode);

  public static int getOldMouseX(bool ui_scale)
  {
    return ui_scale ? (int) ((double) Game1.oldMouseState.X / (double) Game1.options.uiScale) : (int) ((double) Game1.oldMouseState.X * (1.0 / (double) Game1.options.zoomLevel));
  }

  public static int getMouseY() => Game1.getMouseY(Game1.uiMode);

  public static int getMouseY(bool ui_scale)
  {
    return ui_scale ? (int) ((double) Game1.input.GetMouseState().Y / (double) Game1.options.uiScale) : (int) ((double) Game1.input.GetMouseState().Y * (1.0 / (double) Game1.options.zoomLevel));
  }

  public static int getOldMouseY() => Game1.getOldMouseY(Game1.uiMode);

  public static int getOldMouseY(bool ui_scale)
  {
    return ui_scale ? (int) ((double) Game1.oldMouseState.Y / (double) Game1.options.uiScale) : (int) ((double) Game1.oldMouseState.Y * (1.0 / (double) Game1.options.zoomLevel));
  }

  public static bool PlayEvent(
    string eventId,
    GameLocation location,
    out bool validEvent,
    bool checkPreconditions = true,
    bool checkSeen = true)
  {
    Dictionary<string, string> locationEvents;
    try
    {
      if (!location.TryGetLocationEvents(out string _, out locationEvents))
      {
        validEvent = false;
        return false;
      }
    }
    catch
    {
      validEvent = false;
      return false;
    }
    if (locationEvents == null)
    {
      validEvent = false;
      return false;
    }
    foreach (string key1 in locationEvents.Keys)
    {
      string key = key1;
      if (key.Split('/')[0] == eventId)
      {
        validEvent = true;
        if (checkSeen && (Game1.player.eventsSeen.Contains(eventId) || Game1.eventsSeenSinceLastLocationChange.Contains(eventId)))
          return false;
        string id = eventId;
        if (checkPreconditions)
          id = location.checkEventPrecondition(key, false);
        if (string.IsNullOrEmpty(id) || !(id != "-1"))
          return false;
        if (location.Name != Game1.currentLocation.Name)
        {
          LocationRequest locationRequest = Game1.getLocationRequest(location.Name);
          locationRequest.OnLoad += (LocationRequest.Callback) (() => Game1.currentLocation.currentEvent = new Event(locationEvents[key], eventAssetName, id));
          int x = 8;
          int y = 8;
          Utility.getDefaultWarpLocation(locationRequest.Name, ref x, ref y);
          Game1.warpFarmer(locationRequest, x, y, Game1.player.FacingDirection);
        }
        else
          Game1.globalFadeToBlack((Game1.afterFadeFunction) (() =>
          {
            Game1.forceSnapOnNextViewportUpdate = true;
            Game1.currentLocation.startEvent(new Event(locationEvents[key], eventAssetName, id));
            Game1.globalFadeToClear();
          }));
        return true;
      }
    }
    validEvent = false;
    return false;
  }

  public static bool PlayEvent(string eventId, bool checkPreconditions = true, bool checkSeen = true)
  {
    if (checkSeen && (Game1.player.eventsSeen.Contains(eventId) || Game1.eventsSeenSinceLastLocationChange.Contains(eventId)))
      return false;
    bool validEvent;
    if (Game1.PlayEvent(eventId, Game1.currentLocation, out validEvent, checkPreconditions, checkSeen))
      return true;
    if (validEvent)
      return false;
    foreach (GameLocation location in (IEnumerable<GameLocation>) Game1.locations)
    {
      if (location != Game1.currentLocation)
      {
        if (Game1.PlayEvent(eventId, location, out validEvent, checkPreconditions, checkSeen))
          return true;
        if (validEvent)
          return false;
      }
    }
    return false;
  }

  public static int numberOfPlayers() => Game1._onlineFarmers.Count;

  public static bool isFestival() => Game1.currentLocation?.currentEvent?.isFestival ?? false;

  /// <summary>Parse a raw debug command and run it if it's valid.</summary>
  /// <param name="debugInput">The full debug command, including the command name and arguments.</param>
  /// <param name="log">The log to which to write command output, or <c>null</c> to use <see cref="F:StardewValley.Game1.log" />.</param>
  /// <returns>Returns whether the command was found and executed, regardless of whether the command logic succeeded.</returns>
  public bool parseDebugInput(string debugInput, IGameLogger log = null)
  {
    debugInput = debugInput.Trim();
    string[] command = ArgUtility.SplitBySpaceQuoteAware(debugInput);
    try
    {
      return DebugCommands.TryHandle(command, log);
    }
    catch (Exception ex)
    {
      Game1.log.Error("Debug command error.", ex);
      Game1.debugOutput = ex.Message;
      return false;
    }
  }

  public void RecountWalnuts()
  {
    if (!Game1.IsMasterGame || Game1.netWorldState.Value.ActivatedGoldenParrot || !(Game1.getLocationFromName("IslandHut") is IslandHut locationFromName))
      return;
    int num = 130 - locationFromName.ShowNutHint();
    Game1.netWorldState.Value.GoldenWalnutsFound = num;
    foreach (GameLocation location in (IEnumerable<GameLocation>) Game1.locations)
    {
      if (location is IslandLocation islandLocation)
      {
        foreach (ParrotUpgradePerch parrotUpgradePerch in islandLocation.parrotUpgradePerches)
        {
          if (parrotUpgradePerch.currentState.Value == ParrotUpgradePerch.UpgradeState.Complete)
            num -= parrotUpgradePerch.requiredNuts.Value;
        }
      }
    }
    if (Game1.MasterPlayer.hasOrWillReceiveMail("Island_VolcanoShortcutOut"))
      num -= 5;
    if (Game1.MasterPlayer.hasOrWillReceiveMail("Island_VolcanoBridge"))
      num -= 5;
    Game1.netWorldState.Value.GoldenWalnuts = num;
  }

  public void ResetIslandLocations()
  {
    Game1.netWorldState.Value.GoldenWalnutsFound = 0;
    Game1.player.team.collectedNutTracker.Clear();
    NetStringHashSet[] netStringHashSetArray = new NetStringHashSet[3]
    {
      Game1.player.mailReceived,
      Game1.player.mailForTomorrow,
      Game1.player.team.broadcastedMail
    };
    foreach (NetStringHashSet netStringHashSet in netStringHashSetArray)
    {
      netStringHashSet.Remove("birdieQuestBegun");
      netStringHashSet.Remove("birdieQuestFinished");
      netStringHashSet.Remove("tigerSlimeNut");
      netStringHashSet.Remove("Island_W_BuriedTreasureNut");
      netStringHashSet.Remove("Island_W_BuriedTreasure");
      netStringHashSet.Remove("islandNorthCaveOpened");
      netStringHashSet.Remove("Saw_Flame_Sprite_North_North");
      netStringHashSet.Remove("Saw_Flame_Sprite_North_South");
      netStringHashSet.Remove("Island_N_BuriedTreasureNut");
      netStringHashSet.Remove("Island_W_BuriedTreasure");
      netStringHashSet.Remove("Saw_Flame_Sprite_South");
      netStringHashSet.Remove("Visited_Island");
      netStringHashSet.Remove("Island_FirstParrot");
      netStringHashSet.Remove("gotBirdieReward");
      netStringHashSet.RemoveWhere((Predicate<string>) (key => key.StartsWith("Island_Upgrade")));
    }
    Game1.player.secretNotesSeen.RemoveWhere((Predicate<int>) (id => id >= GameLocation.JOURNAL_INDEX));
    Game1.player.team.limitedNutDrops.Clear();
    Game1.netWorldState.Value.GoldenCoconutCracked = false;
    Game1.netWorldState.Value.GoldenWalnuts = 0;
    Game1.netWorldState.Value.ParrotPlatformsUnlocked = false;
    Game1.netWorldState.Value.FoundBuriedNuts.Clear();
    for (int index = 0; index < Game1.locations.Count; ++index)
    {
      GameLocation location = Game1.locations[index];
      if (location.InIslandContext())
      {
        Game1._locationLookup.Clear();
        object[] objArray = new object[2]
        {
          (object) location.mapPath.Value,
          (object) location.name.Value
        };
        try
        {
          Game1.locations[index] = Activator.CreateInstance(location.GetType(), objArray) as GameLocation;
        }
        catch
        {
          Game1.locations[index] = Activator.CreateInstance(location.GetType()) as GameLocation;
        }
        Game1._locationLookup.Clear();
      }
    }
    Game1.AddCharacterIfNecessary("Birdie");
  }

  public void ShowTelephoneMenu()
  {
    Game1.playSound("openBox");
    if (Game1.IsGreenRainingHere())
    {
      Game1.drawObjectDialogue("...................");
    }
    else
    {
      List<KeyValuePair<string, string>> responses = new List<KeyValuePair<string, string>>();
      foreach (IPhoneHandler phoneHandler in Phone.PhoneHandlers)
        responses.AddRange(phoneHandler.GetOutgoingNumbers());
      responses.Add(new KeyValuePair<string, string>("HangUp", Game1.content.LoadString("Strings\\Locations:MineCart_Destination_Cancel")));
      Game1.currentLocation.ShowPagedResponses(Game1.content.LoadString("Strings\\Characters:Phone_SelectNumber"), responses, (Action<string>) (callId =>
      {
        if (callId == "HangUp")
        {
          Phone.HangUp();
        }
        else
        {
          foreach (IPhoneHandler phoneHandler in Phone.PhoneHandlers)
          {
            if (phoneHandler.TryHandleOutgoingCall(callId))
              return;
          }
          Phone.HangUp();
        }
      }), addCancel: false, itemsPerPage: 6);
    }
  }

  public void requestDebugInput()
  {
    Game1.chatBox.activate();
    Game1.chatBox.setText("/");
  }

  private void panModeSuccess(KeyboardState currentKBState)
  {
    this.panFacingDirectionWait = false;
    Game1.playSound("smallSelect");
    if (currentKBState.IsKeyDown(Keys.LeftShift))
      this.panModeString += " (animation_name_here)";
    Game1.debugOutput = this.panModeString;
  }

  private void updatePanModeControls(MouseState currentMouseState, KeyboardState currentKBState)
  {
    if (currentKBState.IsKeyDown(Keys.F8) && !Game1.oldKBState.IsKeyDown(Keys.F8))
    {
      this.requestDebugInput();
    }
    else
    {
      if (!this.panFacingDirectionWait)
      {
        if (currentKBState.IsKeyDown(Keys.W))
          Game1.viewport.Y -= 16 /*0x10*/;
        if (currentKBState.IsKeyDown(Keys.A))
          Game1.viewport.X -= 16 /*0x10*/;
        if (currentKBState.IsKeyDown(Keys.S))
          Game1.viewport.Y += 16 /*0x10*/;
        if (currentKBState.IsKeyDown(Keys.D))
          Game1.viewport.X += 16 /*0x10*/;
      }
      else
      {
        if (currentKBState.IsKeyDown(Keys.W))
        {
          this.panModeString += "0";
          this.panModeSuccess(currentKBState);
        }
        if (currentKBState.IsKeyDown(Keys.A))
        {
          this.panModeString += "3";
          this.panModeSuccess(currentKBState);
        }
        if (currentKBState.IsKeyDown(Keys.S))
        {
          this.panModeString += "2";
          this.panModeSuccess(currentKBState);
        }
        if (currentKBState.IsKeyDown(Keys.D))
        {
          this.panModeString += "1";
          this.panModeSuccess(currentKBState);
        }
      }
      if (Game1.getMouseX(false) < 192 /*0xC0*/)
      {
        Game1.viewport.X -= 8;
        Game1.viewport.X -= (192 /*0xC0*/ - Game1.getMouseX()) / 8;
      }
      if (Game1.getMouseX(false) > Game1.viewport.Width - 192 /*0xC0*/)
      {
        Game1.viewport.X += 8;
        Game1.viewport.X += (Game1.getMouseX() - Game1.viewport.Width + 192 /*0xC0*/) / 8;
      }
      if (Game1.getMouseY(false) < 192 /*0xC0*/)
      {
        Game1.viewport.Y -= 8;
        Game1.viewport.Y -= (192 /*0xC0*/ - Game1.getMouseY()) / 8;
      }
      if (Game1.getMouseY(false) > Game1.viewport.Height - 192 /*0xC0*/)
      {
        Game1.viewport.Y += 8;
        Game1.viewport.Y += (Game1.getMouseY() - Game1.viewport.Height + 192 /*0xC0*/) / 8;
      }
      if (currentMouseState.LeftButton == ButtonState.Pressed && Game1.oldMouseState.LeftButton == ButtonState.Released)
      {
        string panModeString = this.panModeString;
        if ((panModeString != null ? (panModeString.Length > 0 ? 1 : 0) : 0) != 0)
        {
          int x = (Game1.getMouseX() + Game1.viewport.X) / 64 /*0x40*/;
          int y = (Game1.getMouseY() + Game1.viewport.Y) / 64 /*0x40*/;
          this.panModeString = $"{this.panModeString}{Game1.currentLocation.Name} {x.ToString()} {y.ToString()} ";
          this.panFacingDirectionWait = true;
          Game1.currentLocation.playTerrainSound(new Vector2((float) x, (float) y));
          Game1.debugOutput = this.panModeString;
        }
      }
      if (currentMouseState.RightButton == ButtonState.Pressed && Game1.oldMouseState.RightButton == ButtonState.Released)
      {
        Warp warp = Game1.currentLocation.isCollidingWithWarpOrDoor(new Microsoft.Xna.Framework.Rectangle(Game1.getMouseX() + Game1.viewport.X, Game1.getMouseY() + Game1.viewport.Y, 1, 1));
        if (warp != null)
        {
          Game1.currentLocation = Game1.RequireLocation(warp.TargetName);
          Game1.currentLocation.map.LoadTileSheets(Game1.mapDisplayDevice);
          Game1.viewport.X = warp.TargetX * 64 /*0x40*/ - Game1.viewport.Width / 2;
          Game1.viewport.Y = warp.TargetY * 64 /*0x40*/ - Game1.viewport.Height / 2;
          Game1.playSound("dwop");
        }
      }
      if (currentKBState.IsKeyDown(Keys.Escape) && !Game1.oldKBState.IsKeyDown(Keys.Escape))
      {
        Warp warp = Game1.currentLocation.warps[0];
        Game1.currentLocation = Game1.RequireLocation(warp.TargetName);
        Game1.currentLocation.map.LoadTileSheets(Game1.mapDisplayDevice);
        Game1.viewport.X = warp.TargetX * 64 /*0x40*/ - Game1.viewport.Width / 2;
        Game1.viewport.Y = warp.TargetY * 64 /*0x40*/ - Game1.viewport.Height / 2;
        Game1.playSound("dwop");
      }
      if (Game1.viewport.X < -64)
        Game1.viewport.X = -64;
      if (Game1.viewport.X + Game1.viewport.Width > Game1.currentLocation.Map.Layers[0].LayerWidth * 64 /*0x40*/ + 128 /*0x80*/)
        Game1.viewport.X = Game1.currentLocation.Map.Layers[0].LayerWidth * 64 /*0x40*/ + 128 /*0x80*/ - Game1.viewport.Width;
      if (Game1.viewport.Y < -64)
        Game1.viewport.Y = -64;
      if (Game1.viewport.Y + Game1.viewport.Height > Game1.currentLocation.Map.Layers[0].LayerHeight * 64 /*0x40*/ + 128 /*0x80*/)
        Game1.viewport.Y = Game1.currentLocation.Map.Layers[0].LayerHeight * 64 /*0x40*/ + 128 /*0x80*/ - Game1.viewport.Height;
      Game1.oldMouseState = Game1.input.GetMouseState();
      Game1.oldKBState = currentKBState;
    }
  }

  public static bool isLocationAccessible(string locationName)
  {
    switch (locationName)
    {
      case "Desert":
        if (Game1.MasterPlayer.mailReceived.Contains("ccVault"))
          return true;
        break;
      case "CommunityCenter":
        if (Game1.player.eventsSeen.Contains("191393"))
          return true;
        break;
      case "JojaMart":
        if (!Utility.HasAnyPlayerSeenEvent("191393"))
          return true;
        break;
      case "Railroad":
        if (Game1.stats.DaysPlayed > 31U /*0x1F*/)
          return true;
        break;
      default:
        return true;
    }
    return false;
  }

  public static bool isDPadPressed() => Game1.isDPadPressed(Game1.input.GetGamePadState());

  public static bool isDPadPressed(GamePadState pad_state)
  {
    return pad_state.DPad.Up == ButtonState.Pressed || pad_state.DPad.Down == ButtonState.Pressed || pad_state.DPad.Left == ButtonState.Pressed || pad_state.DPad.Right == ButtonState.Pressed;
  }

  public static bool isGamePadThumbstickInMotion(double threshold = 0.2)
  {
    bool flag = false;
    GamePadState gamePadState = Game1.input.GetGamePadState();
    if ((double) gamePadState.ThumbSticks.Left.X < -threshold || gamePadState.IsButtonDown(Buttons.LeftThumbstickLeft))
      flag = true;
    if ((double) gamePadState.ThumbSticks.Left.X > threshold || gamePadState.IsButtonDown(Buttons.LeftThumbstickRight))
      flag = true;
    if ((double) gamePadState.ThumbSticks.Left.Y < -threshold || gamePadState.IsButtonDown(Buttons.LeftThumbstickUp))
      flag = true;
    if ((double) gamePadState.ThumbSticks.Left.Y > threshold || gamePadState.IsButtonDown(Buttons.LeftThumbstickDown))
      flag = true;
    if ((double) gamePadState.ThumbSticks.Right.X < -threshold)
      flag = true;
    if ((double) gamePadState.ThumbSticks.Right.X > threshold)
      flag = true;
    if ((double) gamePadState.ThumbSticks.Right.Y < -threshold)
      flag = true;
    if ((double) gamePadState.ThumbSticks.Right.Y > threshold)
      flag = true;
    if (flag)
      Game1.thumbstickMotionMargin = 50;
    return Game1.thumbstickMotionMargin > 0;
  }

  public static bool isAnyGamePadButtonBeingPressed()
  {
    return Utility.getPressedButtons(Game1.input.GetGamePadState(), Game1.oldPadState).Count > 0;
  }

  public static bool isAnyGamePadButtonBeingHeld()
  {
    return Utility.getHeldButtons(Game1.input.GetGamePadState()).Count > 0;
  }

  private static void UpdateChatBox()
  {
    if (Game1.chatBox == null)
      return;
    KeyboardState keyboardState = Game1.input.GetKeyboardState();
    GamePadState gamePadState = Game1.input.GetGamePadState();
    if (Game1.IsChatting)
    {
      if (Game1.textEntry != null)
        return;
      if (gamePadState.IsButtonDown(Buttons.A))
      {
        MouseState mouseState = Game1.input.GetMouseState();
        if (Game1.chatBox != null && Game1.chatBox.isActive() && !Game1.chatBox.isHoveringOverClickable(mouseState.X, mouseState.Y))
        {
          Game1.oldPadState = gamePadState;
          Game1.oldKBState = keyboardState;
          Game1.showTextEntry((TextBox) Game1.chatBox.chatBox);
        }
      }
      if (!keyboardState.IsKeyDown(Keys.Escape) && !gamePadState.IsButtonDown(Buttons.B) && !gamePadState.IsButtonDown(Buttons.Back))
        return;
      Game1.chatBox.clickAway();
      Game1.oldKBState = keyboardState;
    }
    else
    {
      if (Game1.keyboardDispatcher.Subscriber != null || (!Game1.isOneOfTheseKeysDown(keyboardState, Game1.options.chatButton) || !Game1.game1.HasKeyboardFocus()) && (gamePadState.IsButtonDown(Buttons.RightStick) || Game1.rightStickHoldTime <= 0 || Game1.rightStickHoldTime >= Game1.emoteMenuShowTime))
        return;
      Game1.chatBox.activate();
      if (!keyboardState.IsKeyDown(Keys.OemQuestion))
        return;
      Game1.chatBox.setText("/");
    }
  }

  public static KeyboardState GetKeyboardState()
  {
    KeyboardState keyboardState = Game1.input.GetKeyboardState();
    if (Game1.chatBox != null)
    {
      if (Game1.IsChatting)
        return new KeyboardState();
      if (Game1.keyboardDispatcher.Subscriber == null && Game1.isOneOfTheseKeysDown(keyboardState, Game1.options.chatButton) && Game1.game1.HasKeyboardFocus())
        return new KeyboardState();
    }
    return keyboardState;
  }

  private void UpdateControlInput(GameTime time)
  {
    KeyboardState currentKBState = Game1.GetKeyboardState();
    MouseState currentMouseState = Game1.input.GetMouseState();
    GamePadState currentPadState = Game1.input.GetGamePadState();
    if (Game1.ticks < Game1._activatedTick + 2 && Game1.oldKBState.IsKeyDown(Keys.Tab) != currentKBState.IsKeyDown(Keys.Tab))
    {
      List<Keys> list = ((IEnumerable<Keys>) Game1.oldKBState.GetPressedKeys()).ToList<Keys>();
      if (currentKBState.IsKeyDown(Keys.Tab))
        list.Add(Keys.Tab);
      else
        list.Remove(Keys.Tab);
      Game1.oldKBState = new KeyboardState(list.ToArray());
    }
    Game1.hooks.OnGame1_UpdateControlInput(ref currentKBState, ref currentMouseState, ref currentPadState, (Action) (() =>
    {
      GamePadThumbSticks thumbSticks;
      if (Game1.options.gamepadControls)
      {
        bool flag = false;
        if ((double) Math.Abs(currentPadState.ThumbSticks.Right.X) <= 0.0)
        {
          thumbSticks = currentPadState.ThumbSticks;
          if ((double) Math.Abs(thumbSticks.Right.Y) <= 0.0)
            goto label_4;
        }
        double x1 = (double) currentMouseState.X;
        thumbSticks = currentPadState.ThumbSticks;
        double num1 = (double) thumbSticks.Right.X * (double) Game1.thumbstickToMouseModifier;
        int x2 = (int) (x1 + num1);
        double y1 = (double) currentMouseState.Y;
        thumbSticks = currentPadState.ThumbSticks;
        double num2 = (double) thumbSticks.Right.Y * (double) Game1.thumbstickToMouseModifier;
        int y2 = (int) (y1 - num2);
        Game1.setMousePositionRaw(x2, y2);
        flag = true;
label_4:
        if (Game1.IsChatting)
          flag = true;
        if (((Game1.getMouseX() == Game1.getOldMouseX() && Game1.getMouseY() == Game1.getOldMouseY() || Game1.getMouseX() == 0 ? 0 : (Game1.getMouseY() != 0 ? 1 : 0)) | (flag ? 1 : 0)) != 0)
        {
          if (flag)
          {
            if (Game1.timerUntilMouseFade <= 0)
              Game1.lastMousePositionBeforeFade = new Point(this.localMultiplayerWindow.Width / 2, this.localMultiplayerWindow.Height / 2);
          }
          else
            Game1.lastCursorMotionWasMouse = true;
          if (Game1.timerUntilMouseFade <= 0 && !Game1.lastCursorMotionWasMouse)
            Game1.setMousePositionRaw(Game1.lastMousePositionBeforeFade.X, Game1.lastMousePositionBeforeFade.Y);
          Game1.timerUntilMouseFade = 4000;
        }
      }
      else if (Game1.getMouseX() != Game1.getOldMouseX() || Game1.getMouseY() != Game1.getOldMouseY())
        Game1.lastCursorMotionWasMouse = true;
      bool actionButtonPressed = false;
      bool switchToolButtonPressed = false;
      bool useToolButtonPressed = false;
      bool useToolButtonReleased = false;
      bool addItemToInventoryButtonPressed = false;
      bool cancelButtonPressed = false;
      bool moveUpPressed = false;
      bool moveRightPressed = false;
      bool moveLeftPressed = false;
      bool moveDownPressed = false;
      bool moveUpReleased = false;
      bool moveRightReleased = false;
      bool moveDownReleased = false;
      bool moveLeftReleased = false;
      bool moveUpHeld = false;
      bool moveRightHeld = false;
      bool moveDownHeld = false;
      bool moveLeftHeld = false;
      bool flag1 = false;
      if (Game1.isOneOfTheseKeysDown(currentKBState, Game1.options.actionButton) && Game1.areAllOfTheseKeysUp(Game1.oldKBState, Game1.options.actionButton) || currentMouseState.RightButton == ButtonState.Pressed && Game1.oldMouseState.RightButton == ButtonState.Released)
      {
        actionButtonPressed = true;
        Game1.rightClickPolling = 250;
      }
      if (Game1.isOneOfTheseKeysDown(currentKBState, Game1.options.useToolButton) && Game1.areAllOfTheseKeysUp(Game1.oldKBState, Game1.options.useToolButton) || currentMouseState.LeftButton == ButtonState.Pressed && Game1.oldMouseState.LeftButton == ButtonState.Released)
        useToolButtonPressed = true;
      if (Game1.areAllOfTheseKeysUp(currentKBState, Game1.options.useToolButton) && Game1.isOneOfTheseKeysDown(Game1.oldKBState, Game1.options.useToolButton) || currentMouseState.LeftButton == ButtonState.Released && Game1.oldMouseState.LeftButton == ButtonState.Pressed)
        useToolButtonReleased = true;
      if (currentMouseState.ScrollWheelValue != Game1.oldMouseState.ScrollWheelValue)
        switchToolButtonPressed = true;
      if (Game1.isOneOfTheseKeysDown(currentKBState, Game1.options.cancelButton) && Game1.areAllOfTheseKeysUp(Game1.oldKBState, Game1.options.cancelButton) || currentMouseState.RightButton == ButtonState.Pressed && Game1.oldMouseState.RightButton == ButtonState.Released)
        cancelButtonPressed = true;
      if (Game1.isOneOfTheseKeysDown(currentKBState, Game1.options.moveUpButton) && Game1.areAllOfTheseKeysUp(Game1.oldKBState, Game1.options.moveUpButton))
        moveUpPressed = true;
      if (Game1.isOneOfTheseKeysDown(currentKBState, Game1.options.moveRightButton) && Game1.areAllOfTheseKeysUp(Game1.oldKBState, Game1.options.moveRightButton))
        moveRightPressed = true;
      if (Game1.isOneOfTheseKeysDown(currentKBState, Game1.options.moveDownButton) && Game1.areAllOfTheseKeysUp(Game1.oldKBState, Game1.options.moveDownButton))
        moveDownPressed = true;
      if (Game1.isOneOfTheseKeysDown(currentKBState, Game1.options.moveLeftButton) && Game1.areAllOfTheseKeysUp(Game1.oldKBState, Game1.options.moveLeftButton))
        moveLeftPressed = true;
      if (Game1.areAllOfTheseKeysUp(currentKBState, Game1.options.moveUpButton) && Game1.isOneOfTheseKeysDown(Game1.oldKBState, Game1.options.moveUpButton))
        moveUpReleased = true;
      if (Game1.areAllOfTheseKeysUp(currentKBState, Game1.options.moveRightButton) && Game1.isOneOfTheseKeysDown(Game1.oldKBState, Game1.options.moveRightButton))
        moveRightReleased = true;
      if (Game1.areAllOfTheseKeysUp(currentKBState, Game1.options.moveDownButton) && Game1.isOneOfTheseKeysDown(Game1.oldKBState, Game1.options.moveDownButton))
        moveDownReleased = true;
      if (Game1.areAllOfTheseKeysUp(currentKBState, Game1.options.moveLeftButton) && Game1.isOneOfTheseKeysDown(Game1.oldKBState, Game1.options.moveLeftButton))
        moveLeftReleased = true;
      if (Game1.isOneOfTheseKeysDown(currentKBState, Game1.options.moveUpButton))
        moveUpHeld = true;
      if (Game1.isOneOfTheseKeysDown(currentKBState, Game1.options.moveRightButton))
        moveRightHeld = true;
      if (Game1.isOneOfTheseKeysDown(currentKBState, Game1.options.moveDownButton))
        moveDownHeld = true;
      if (Game1.isOneOfTheseKeysDown(currentKBState, Game1.options.moveLeftButton))
        moveLeftHeld = true;
      if (Game1.isOneOfTheseKeysDown(currentKBState, Game1.options.useToolButton) || currentMouseState.LeftButton == ButtonState.Pressed)
        flag1 = true;
      if (Game1.isOneOfTheseKeysDown(currentKBState, Game1.options.actionButton) || currentMouseState.RightButton == ButtonState.Pressed)
      {
        Game1.rightClickPolling -= time.ElapsedGameTime.Milliseconds;
        if (Game1.rightClickPolling <= 0)
        {
          Game1.rightClickPolling = 100;
          actionButtonPressed = true;
        }
      }
      if (Game1.options.gamepadControls)
      {
        if (currentKBState.GetPressedKeys().Length != 0 || currentMouseState.LeftButton == ButtonState.Pressed || currentMouseState.RightButton == ButtonState.Pressed)
          Game1.timerUntilMouseFade = 4000;
        if (currentPadState.IsButtonDown(Buttons.A) && !Game1.oldPadState.IsButtonDown(Buttons.A))
        {
          actionButtonPressed = true;
          Game1.lastCursorMotionWasMouse = false;
          Game1.rightClickPolling = 250;
        }
        if (currentPadState.IsButtonDown(Buttons.X) && !Game1.oldPadState.IsButtonDown(Buttons.X))
        {
          useToolButtonPressed = true;
          Game1.lastCursorMotionWasMouse = false;
        }
        if (!currentPadState.IsButtonDown(Buttons.X) && Game1.oldPadState.IsButtonDown(Buttons.X))
          useToolButtonReleased = true;
        if (currentPadState.IsButtonDown(Buttons.RightTrigger) && !Game1.oldPadState.IsButtonDown(Buttons.RightTrigger))
        {
          switchToolButtonPressed = true;
          Game1.triggerPolling = 300;
        }
        else if (currentPadState.IsButtonDown(Buttons.LeftTrigger) && !Game1.oldPadState.IsButtonDown(Buttons.LeftTrigger))
        {
          switchToolButtonPressed = true;
          Game1.triggerPolling = 300;
        }
        if (currentPadState.IsButtonDown(Buttons.X))
          flag1 = true;
        if (currentPadState.IsButtonDown(Buttons.A))
        {
          Game1.rightClickPolling -= time.ElapsedGameTime.Milliseconds;
          if (Game1.rightClickPolling <= 0)
          {
            Game1.rightClickPolling = 100;
            actionButtonPressed = true;
          }
        }
        if (currentPadState.IsButtonDown(Buttons.RightTrigger) || currentPadState.IsButtonDown(Buttons.LeftTrigger))
        {
          Game1.triggerPolling -= time.ElapsedGameTime.Milliseconds;
          if (Game1.triggerPolling <= 0)
          {
            Game1.triggerPolling = 100;
            switchToolButtonPressed = true;
          }
        }
        if (currentPadState.IsButtonDown(Buttons.RightShoulder) && !Game1.oldPadState.IsButtonDown(Buttons.RightShoulder) && Game1.IsHudDrawn)
          Game1.player.shiftToolbar(true);
        if (currentPadState.IsButtonDown(Buttons.LeftShoulder) && !Game1.oldPadState.IsButtonDown(Buttons.LeftShoulder) && Game1.IsHudDrawn)
          Game1.player.shiftToolbar(false);
        if (currentPadState.IsButtonDown(Buttons.DPadUp) && !Game1.oldPadState.IsButtonDown(Buttons.DPadUp))
          moveUpPressed = true;
        else if (!currentPadState.IsButtonDown(Buttons.DPadUp) && Game1.oldPadState.IsButtonDown(Buttons.DPadUp))
          moveUpReleased = true;
        if (currentPadState.IsButtonDown(Buttons.DPadRight) && !Game1.oldPadState.IsButtonDown(Buttons.DPadRight))
          moveRightPressed = true;
        else if (!currentPadState.IsButtonDown(Buttons.DPadRight) && Game1.oldPadState.IsButtonDown(Buttons.DPadRight))
          moveRightReleased = true;
        if (currentPadState.IsButtonDown(Buttons.DPadDown) && !Game1.oldPadState.IsButtonDown(Buttons.DPadDown))
          moveDownPressed = true;
        else if (!currentPadState.IsButtonDown(Buttons.DPadDown) && Game1.oldPadState.IsButtonDown(Buttons.DPadDown))
          moveDownReleased = true;
        if (currentPadState.IsButtonDown(Buttons.DPadLeft) && !Game1.oldPadState.IsButtonDown(Buttons.DPadLeft))
          moveLeftPressed = true;
        else if (!currentPadState.IsButtonDown(Buttons.DPadLeft) && Game1.oldPadState.IsButtonDown(Buttons.DPadLeft))
          moveLeftReleased = true;
        if (currentPadState.IsButtonDown(Buttons.DPadUp))
          moveUpHeld = true;
        if (currentPadState.IsButtonDown(Buttons.DPadRight))
          moveRightHeld = true;
        if (currentPadState.IsButtonDown(Buttons.DPadDown))
          moveDownHeld = true;
        if (currentPadState.IsButtonDown(Buttons.DPadLeft))
          moveLeftHeld = true;
        thumbSticks = currentPadState.ThumbSticks;
        if ((double) thumbSticks.Left.X < -0.2)
        {
          moveLeftPressed = true;
          moveLeftHeld = true;
        }
        else
        {
          thumbSticks = currentPadState.ThumbSticks;
          if ((double) thumbSticks.Left.X > 0.2)
          {
            moveRightPressed = true;
            moveRightHeld = true;
          }
        }
        thumbSticks = currentPadState.ThumbSticks;
        if ((double) thumbSticks.Left.Y < -0.2)
        {
          moveDownPressed = true;
          moveDownHeld = true;
        }
        else
        {
          thumbSticks = currentPadState.ThumbSticks;
          if ((double) thumbSticks.Left.Y > 0.2)
          {
            moveUpPressed = true;
            moveUpHeld = true;
          }
        }
        thumbSticks = Game1.oldPadState.ThumbSticks;
        if ((double) thumbSticks.Left.X < -0.2 && !moveLeftHeld)
          moveLeftReleased = true;
        thumbSticks = Game1.oldPadState.ThumbSticks;
        if ((double) thumbSticks.Left.X > 0.2 && !moveRightHeld)
          moveRightReleased = true;
        thumbSticks = Game1.oldPadState.ThumbSticks;
        if ((double) thumbSticks.Left.Y < -0.2 && !moveDownHeld)
          moveDownReleased = true;
        thumbSticks = Game1.oldPadState.ThumbSticks;
        if ((double) thumbSticks.Left.Y > 0.2 && !moveUpHeld)
          moveUpReleased = true;
        if ((double) this.controllerSlingshotSafeTime > 0.0)
        {
          if (!currentPadState.IsButtonDown(Buttons.DPadUp) && !currentPadState.IsButtonDown(Buttons.DPadDown) && !currentPadState.IsButtonDown(Buttons.DPadLeft) && !currentPadState.IsButtonDown(Buttons.DPadRight))
          {
            thumbSticks = currentPadState.ThumbSticks;
            if ((double) Math.Abs(thumbSticks.Left.X) < 0.04)
            {
              thumbSticks = currentPadState.ThumbSticks;
              if ((double) Math.Abs(thumbSticks.Left.Y) < 0.04)
                this.controllerSlingshotSafeTime = 0.0f;
            }
          }
          if ((double) this.controllerSlingshotSafeTime <= 0.0)
          {
            this.controllerSlingshotSafeTime = 0.0f;
          }
          else
          {
            this.controllerSlingshotSafeTime -= (float) time.ElapsedGameTime.TotalSeconds;
            moveUpPressed = false;
            moveDownPressed = false;
            moveLeftPressed = false;
            moveRightPressed = false;
            moveUpHeld = false;
            moveDownHeld = false;
            moveLeftHeld = false;
            moveRightHeld = false;
          }
        }
      }
      else
        this.controllerSlingshotSafeTime = 0.0f;
      Game1.ResetFreeCursorDrag();
      if (flag1)
        Game1.mouseClickPolling += time.ElapsedGameTime.Milliseconds;
      else
        Game1.mouseClickPolling = 0;
      if (Game1.isOneOfTheseKeysDown(currentKBState, Game1.options.toolbarSwap) && Game1.areAllOfTheseKeysUp(Game1.oldKBState, Game1.options.toolbarSwap) && Game1.IsHudDrawn)
        Game1.player.shiftToolbar(!currentKBState.IsKeyDown(Keys.LeftControl));
      if (Game1.mouseClickPolling > 250 && (!(Game1.player.CurrentTool is FishingRod) || Game1.player.CurrentTool.upgradeLevel.Value <= 0))
      {
        useToolButtonPressed = true;
        Game1.mouseClickPolling = 100;
      }
      Game1.PushUIMode();
      foreach (IClickableMenu onScreenMenu in (IEnumerable<IClickableMenu>) Game1.onScreenMenus)
      {
        if ((Game1.IsHudDrawn || onScreenMenu == Game1.chatBox) && Game1.wasMouseVisibleThisFrame && onScreenMenu.isWithinBounds(Game1.getMouseX(), Game1.getMouseY()))
          onScreenMenu.performHoverAction(Game1.getMouseX(), Game1.getMouseY());
      }
      Game1.PopUIMode();
      if (Game1.chatBox != null && Game1.chatBox.chatBox.Selected && Game1.oldMouseState.ScrollWheelValue != currentMouseState.ScrollWheelValue)
        Game1.chatBox.receiveScrollWheelAction(currentMouseState.ScrollWheelValue - Game1.oldMouseState.ScrollWheelValue);
      if (Game1.panMode)
      {
        this.updatePanModeControls(currentMouseState, currentKBState);
      }
      else
      {
        if (Game1.inputSimulator != null)
        {
          if (currentKBState.IsKeyDown(Keys.Escape))
            Game1.inputSimulator = (IInputSimulator) null;
          else
            Game1.inputSimulator.SimulateInput(ref actionButtonPressed, ref switchToolButtonPressed, ref useToolButtonPressed, ref useToolButtonReleased, ref addItemToInventoryButtonPressed, ref cancelButtonPressed, ref moveUpPressed, ref moveRightPressed, ref moveLeftPressed, ref moveDownPressed, ref moveUpReleased, ref moveRightReleased, ref moveLeftReleased, ref moveDownReleased, ref moveUpHeld, ref moveRightHeld, ref moveLeftHeld, ref moveDownHeld);
        }
        if (useToolButtonReleased && Game1.player.CurrentTool != null && Game1.CurrentEvent == null && (double) Game1.pauseTime <= 0.0 && Game1.player.CurrentTool.onRelease(Game1.currentLocation, Game1.getMouseX(), Game1.getMouseY(), Game1.player))
        {
          Game1.oldMouseState = Game1.input.GetMouseState();
          Game1.oldKBState = currentKBState;
          Game1.oldPadState = currentPadState;
          Game1.player.usingSlingshot = false;
          Game1.player.canReleaseTool = true;
          Game1.player.UsingTool = false;
          Game1.player.CanMove = true;
        }
        else
        {
          if ((useToolButtonPressed && !Game1.isAnyGamePadButtonBeingPressed() || actionButtonPressed && Game1.isAnyGamePadButtonBeingPressed()) && (double) Game1.pauseTime <= 0.0 && Game1.wasMouseVisibleThisFrame)
          {
            if (Game1.debugMode)
              Console.WriteLine($"{(Game1.getMouseX() + Game1.viewport.X).ToString()}, {(Game1.getMouseY() + Game1.viewport.Y).ToString()}");
            Game1.PushUIMode();
            foreach (IClickableMenu onScreenMenu in (IEnumerable<IClickableMenu>) Game1.onScreenMenus)
            {
              if (Game1.IsHudDrawn || onScreenMenu == Game1.chatBox)
              {
                if ((!Game1.IsChatting || onScreenMenu == Game1.chatBox) && (!(onScreenMenu is LevelUpMenu levelUpMenu3) || levelUpMenu3.informationUp) && onScreenMenu.isWithinBounds(Game1.getMouseX(), Game1.getMouseY()))
                {
                  onScreenMenu.receiveLeftClick(Game1.getMouseX(), Game1.getMouseY());
                  Game1.PopUIMode();
                  Game1.oldMouseState = Game1.input.GetMouseState();
                  Game1.oldKBState = currentKBState;
                  Game1.oldPadState = currentPadState;
                  return;
                }
                if (onScreenMenu == Game1.chatBox && Game1.options.gamepadControls && Game1.IsChatting)
                {
                  Game1.oldMouseState = Game1.input.GetMouseState();
                  Game1.oldKBState = currentKBState;
                  Game1.oldPadState = currentPadState;
                  Game1.PopUIMode();
                  return;
                }
                onScreenMenu.clickAway();
              }
            }
            Game1.PopUIMode();
          }
          if (Game1.IsChatting || Game1.player.freezePause > 0)
          {
            if (Game1.IsChatting)
            {
              foreach (Buttons pressedButton in Utility.getPressedButtons(currentPadState, Game1.oldPadState))
                Game1.chatBox.receiveGamePadButton(pressedButton);
            }
            Game1.oldMouseState = Game1.input.GetMouseState();
            Game1.oldKBState = currentKBState;
            Game1.oldPadState = currentPadState;
          }
          else
          {
            if (Game1.paused || Game1.HostPaused)
            {
              if (Game1.HostPaused && Game1.IsMasterGame && (Game1.isOneOfTheseKeysDown(currentKBState, Game1.options.menuButton) || currentPadState.IsButtonDown(Buttons.B) || currentPadState.IsButtonDown(Buttons.Back)))
              {
                Game1.netWorldState.Value.IsPaused = false;
                Game1.chatBox?.globalInfoMessage("Resumed");
              }
              else
              {
                Game1.oldMouseState = Game1.input.GetMouseState();
                return;
              }
            }
            if (Game1.eventUp)
            {
              if (Game1.currentLocation.currentEvent == null && Game1.locationRequest == null)
                Game1.eventUp = false;
              else if (actionButtonPressed | useToolButtonPressed)
                Game1.CurrentEvent?.receiveMouseClick(Game1.getMouseX(), Game1.getMouseY());
            }
            bool flag2 = Game1.eventUp || Game1.farmEvent != null;
            if (actionButtonPressed || Game1.dialogueUp & useToolButtonPressed)
            {
              Game1.PushUIMode();
              foreach (IClickableMenu onScreenMenu in (IEnumerable<IClickableMenu>) Game1.onScreenMenus)
              {
                if (Game1.wasMouseVisibleThisFrame && (Game1.IsHudDrawn || onScreenMenu == Game1.chatBox) && onScreenMenu.isWithinBounds(Game1.getMouseX(), Game1.getMouseY()) && (!(onScreenMenu is LevelUpMenu levelUpMenu4) || levelUpMenu4.informationUp))
                {
                  onScreenMenu.receiveRightClick(Game1.getMouseX(), Game1.getMouseY());
                  Game1.oldMouseState = Game1.input.GetMouseState();
                  if (!Game1.isAnyGamePadButtonBeingPressed())
                  {
                    Game1.PopUIMode();
                    Game1.oldKBState = currentKBState;
                    Game1.oldPadState = currentPadState;
                    return;
                  }
                }
              }
              Game1.PopUIMode();
              if (!Game1.pressActionButton(currentKBState, currentMouseState, currentPadState))
              {
                Game1.oldKBState = currentKBState;
                Game1.oldMouseState = Game1.input.GetMouseState();
                Game1.oldPadState = currentPadState;
                return;
              }
            }
            if (useToolButtonPressed && (!Game1.player.UsingTool || Game1.player.CurrentTool is MeleeWeapon) && !Game1.player.isEating && !Game1.dialogueUp && Game1.farmEvent == null && (Game1.player.CanMove || Game1.player.CurrentTool is MeleeWeapon))
            {
              if (Game1.player.CurrentTool != null && (!(Game1.player.CurrentTool is MeleeWeapon) || Game1.didPlayerJustLeftClick(true)))
                Game1.player.FireTool();
              if (!Game1.pressUseToolButton() && Game1.player.canReleaseTool && Game1.player.UsingTool)
              {
                Tool currentTool = Game1.player.CurrentTool;
              }
              if (Game1.player.UsingTool)
              {
                Game1.oldMouseState = Game1.input.GetMouseState();
                Game1.oldKBState = currentKBState;
                Game1.oldPadState = currentPadState;
                return;
              }
            }
            if (useToolButtonReleased && this._didInitiateItemStow)
              this._didInitiateItemStow = false;
            if (useToolButtonReleased && Game1.player.canReleaseTool && Game1.player.UsingTool && Game1.player.CurrentTool != null)
              Game1.player.EndUsingTool();
            if (switchToolButtonPressed && !Game1.player.UsingTool && !Game1.dialogueUp && Game1.player.CanMove && Game1.player.Items.HasAny() && !flag2)
              Game1.pressSwitchToolButton();
            TimeSpan elapsedGameTime;
            if (Game1.player.CurrentTool != null & flag1 && Game1.player.canReleaseTool && !flag2 && !Game1.dialogueUp && (double) Game1.player.Stamina >= 1.0 && !(Game1.player.CurrentTool is FishingRod))
            {
              int num3 = Game1.player.CurrentTool.hasEnchantmentOfType<ReachingToolEnchantment>() ? 1 : 0;
              if (Game1.player.toolHold.Value <= 0 && Game1.player.CurrentTool.upgradeLevel.Value + num3 > Game1.player.toolPower.Value)
              {
                float num4 = 1f;
                if (Game1.player.CurrentTool != null)
                  num4 = Game1.player.CurrentTool.AnimationSpeedModifier;
                Game1.player.toolHold.Value = (int) (600.0 * (double) num4);
                Game1.player.toolHoldStartTime.Value = Game1.player.toolHold.Value;
              }
              else if (Game1.player.CurrentTool.upgradeLevel.Value + num3 > Game1.player.toolPower.Value)
              {
                NetInt toolHold = Game1.player.toolHold;
                int num5 = toolHold.Value;
                elapsedGameTime = time.ElapsedGameTime;
                int milliseconds = elapsedGameTime.Milliseconds;
                toolHold.Value = num5 - milliseconds;
                if (Game1.player.toolHold.Value <= 0)
                  Game1.player.toolPowerIncrease();
              }
            }
            if ((double) Game1.upPolling >= 650.0)
              Game1.upPolling -= 100f;
            else if ((double) Game1.downPolling >= 650.0)
              Game1.downPolling -= 100f;
            else if ((double) Game1.rightPolling >= 650.0)
              Game1.rightPolling -= 100f;
            else if ((double) Game1.leftPolling >= 650.0)
              Game1.leftPolling -= 100f;
            else if ((double) Game1.pauseTime <= 0.0 && Game1.locationRequest == null && (!Game1.player.UsingTool || Game1.player.canStrafeForToolUse()) && (!flag2 || Game1.CurrentEvent != null && Game1.CurrentEvent.playerControlSequence))
            {
              if (Game1.player.movementDirections.Count < 2)
              {
                if (moveUpHeld)
                  Game1.player.setMoving((byte) 1);
                if (moveRightHeld)
                  Game1.player.setMoving((byte) 2);
                if (moveDownHeld)
                  Game1.player.setMoving((byte) 4);
                if (moveLeftHeld)
                  Game1.player.setMoving((byte) 8);
              }
              if (moveUpReleased || Game1.player.movementDirections.Contains(0) && !moveUpHeld)
              {
                Game1.player.setMoving((byte) 33);
                if (Game1.player.movementDirections.Count == 0)
                  Game1.player.setMoving((byte) 64 /*0x40*/);
              }
              if (moveRightReleased || Game1.player.movementDirections.Contains(1) && !moveRightHeld)
              {
                Game1.player.setMoving((byte) 34);
                if (Game1.player.movementDirections.Count == 0)
                  Game1.player.setMoving((byte) 64 /*0x40*/);
              }
              if (moveDownReleased || Game1.player.movementDirections.Contains(2) && !moveDownHeld)
              {
                Game1.player.setMoving((byte) 36);
                if (Game1.player.movementDirections.Count == 0)
                  Game1.player.setMoving((byte) 64 /*0x40*/);
              }
              if (moveLeftReleased || Game1.player.movementDirections.Contains(3) && !moveLeftHeld)
              {
                Game1.player.setMoving((byte) 40);
                if (Game1.player.movementDirections.Count == 0)
                  Game1.player.setMoving((byte) 64 /*0x40*/);
              }
              if (!moveUpHeld && !moveRightHeld && !moveDownHeld && !moveLeftHeld && !Game1.player.UsingTool || Game1.activeClickableMenu != null)
                Game1.player.Halt();
            }
            else if (Game1.isQuestion)
            {
              if (moveUpPressed)
              {
                Game1.currentQuestionChoice = Math.Max(Game1.currentQuestionChoice - 1, 0);
                Game1.playSound("toolSwap");
              }
              else if (moveDownPressed)
              {
                Game1.currentQuestionChoice = Math.Min(Game1.currentQuestionChoice + 1, Game1.questionChoices.Count - 1);
                Game1.playSound("toolSwap");
              }
            }
            if (moveUpHeld && !Game1.player.CanMove)
            {
              double upPolling = (double) Game1.upPolling;
              elapsedGameTime = time.ElapsedGameTime;
              double milliseconds = (double) elapsedGameTime.Milliseconds;
              Game1.upPolling = (float) (upPolling + milliseconds);
            }
            else if (moveDownHeld && !Game1.player.CanMove)
            {
              double downPolling = (double) Game1.downPolling;
              elapsedGameTime = time.ElapsedGameTime;
              double milliseconds = (double) elapsedGameTime.Milliseconds;
              Game1.downPolling = (float) (downPolling + milliseconds);
            }
            else if (moveRightHeld && !Game1.player.CanMove)
            {
              double rightPolling = (double) Game1.rightPolling;
              elapsedGameTime = time.ElapsedGameTime;
              double milliseconds = (double) elapsedGameTime.Milliseconds;
              Game1.rightPolling = (float) (rightPolling + milliseconds);
            }
            else if (moveLeftHeld && !Game1.player.CanMove)
            {
              double leftPolling = (double) Game1.leftPolling;
              elapsedGameTime = time.ElapsedGameTime;
              double milliseconds = (double) elapsedGameTime.Milliseconds;
              Game1.leftPolling = (float) (leftPolling + milliseconds);
            }
            else if (moveUpReleased)
              Game1.upPolling = 0.0f;
            else if (moveDownReleased)
              Game1.downPolling = 0.0f;
            else if (moveRightReleased)
              Game1.rightPolling = 0.0f;
            else if (moveLeftReleased)
              Game1.leftPolling = 0.0f;
            if (Game1.debugMode)
            {
              if (currentKBState.IsKeyDown(Keys.Q))
                Game1.oldKBState.IsKeyDown(Keys.Q);
              if (currentKBState.IsKeyDown(Keys.P) && !Game1.oldKBState.IsKeyDown(Keys.P))
                Game1.NewDay(0.0f);
              if (currentKBState.IsKeyDown(Keys.M) && !Game1.oldKBState.IsKeyDown(Keys.M))
              {
                Game1.dayOfMonth = 28;
                Game1.NewDay(0.0f);
              }
              if (currentKBState.IsKeyDown(Keys.T) && !Game1.oldKBState.IsKeyDown(Keys.T))
                Game1.addHour();
              if (currentKBState.IsKeyDown(Keys.Y) && !Game1.oldKBState.IsKeyDown(Keys.Y))
                Game1.addMinute();
              if (currentKBState.IsKeyDown(Keys.D1) && !Game1.oldKBState.IsKeyDown(Keys.D1))
                Game1.warpFarmer("Mountain", 15, 35, false);
              if (currentKBState.IsKeyDown(Keys.D2) && !Game1.oldKBState.IsKeyDown(Keys.D2))
                Game1.warpFarmer("Town", 35, 35, false);
              if (currentKBState.IsKeyDown(Keys.D3) && !Game1.oldKBState.IsKeyDown(Keys.D3))
                Game1.warpFarmer("Farm", 64 /*0x40*/, 15, false);
              if (currentKBState.IsKeyDown(Keys.D4) && !Game1.oldKBState.IsKeyDown(Keys.D4))
                Game1.warpFarmer("Forest", 34, 13, false);
              if (currentKBState.IsKeyDown(Keys.D5) && !Game1.oldKBState.IsKeyDown(Keys.D4))
                Game1.warpFarmer("Beach", 34, 10, false);
              if (currentKBState.IsKeyDown(Keys.D6) && !Game1.oldKBState.IsKeyDown(Keys.D6))
                Game1.warpFarmer("Mine", 18, 12, false);
              if (currentKBState.IsKeyDown(Keys.D7) && !Game1.oldKBState.IsKeyDown(Keys.D7))
                Game1.warpFarmer("SandyHouse", 16 /*0x10*/, 3, false);
              if (currentKBState.IsKeyDown(Keys.K) && !Game1.oldKBState.IsKeyDown(Keys.K))
                Game1.enterMine(Game1.mine.mineLevel + 1);
              if (currentKBState.IsKeyDown(Keys.H) && !Game1.oldKBState.IsKeyDown(Keys.H))
                Game1.player.changeHat(Game1.random.Next(FarmerRenderer.hatsTexture.Height / 80 /*0x50*/ * 12));
              if (currentKBState.IsKeyDown(Keys.I) && !Game1.oldKBState.IsKeyDown(Keys.I))
                Game1.player.changeHairStyle(Game1.random.Next(FarmerRenderer.hairStylesTexture.Height / 96 /*0x60*/ * 8));
              if (currentKBState.IsKeyDown(Keys.J) && !Game1.oldKBState.IsKeyDown(Keys.J))
              {
                Game1.player.changeShirt(Game1.random.Next(1000, 1040).ToString());
                Game1.player.changePantsColor(new Color(Game1.random.Next((int) byte.MaxValue), Game1.random.Next((int) byte.MaxValue), Game1.random.Next((int) byte.MaxValue)));
              }
              if (currentKBState.IsKeyDown(Keys.L) && !Game1.oldKBState.IsKeyDown(Keys.L))
              {
                Game1.player.changeShirt(Game1.random.Next(1000, 1040).ToString());
                Game1.player.changePantsColor(new Color(Game1.random.Next((int) byte.MaxValue), Game1.random.Next((int) byte.MaxValue), Game1.random.Next((int) byte.MaxValue)));
                Game1.player.changeHairStyle(Game1.random.Next(FarmerRenderer.hairStylesTexture.Height / 96 /*0x60*/ * 8));
                if (Game1.random.NextBool())
                  Game1.player.changeHat(Game1.random.Next(-1, FarmerRenderer.hatsTexture.Height / 80 /*0x50*/ * 12));
                else
                  Game1.player.changeHat(-1);
                Game1.player.changeHairColor(new Color(Game1.random.Next((int) byte.MaxValue), Game1.random.Next((int) byte.MaxValue), Game1.random.Next((int) byte.MaxValue)));
                Game1.player.changeSkinColor(Game1.random.Next(16 /*0x10*/));
              }
              if (currentKBState.IsKeyDown(Keys.U) && !Game1.oldKBState.IsKeyDown(Keys.U))
              {
                FarmHouse farmHouse = Game1.RequireLocation<FarmHouse>("FarmHouse");
                int num = Game1.random.Next(112 /*0x70*/);
                farmHouse.SetWallpaper(num.ToString(), (string) null);
                num = Game1.random.Next(40);
                farmHouse.SetFloor(num.ToString(), (string) null);
              }
              if (currentKBState.IsKeyDown(Keys.F2))
                Game1.oldKBState.IsKeyDown(Keys.F2);
              if (currentKBState.IsKeyDown(Keys.F5) && !Game1.oldKBState.IsKeyDown(Keys.F5))
                Game1.displayFarmer = !Game1.displayFarmer;
              if (currentKBState.IsKeyDown(Keys.F6))
                Game1.oldKBState.IsKeyDown(Keys.F6);
              if (currentKBState.IsKeyDown(Keys.F7) && !Game1.oldKBState.IsKeyDown(Keys.F7))
                Game1.drawGrid = !Game1.drawGrid;
              if (currentKBState.IsKeyDown(Keys.B) && !Game1.oldKBState.IsKeyDown(Keys.B) && Game1.IsHudDrawn)
                Game1.player.shiftToolbar(false);
              if (currentKBState.IsKeyDown(Keys.N) && !Game1.oldKBState.IsKeyDown(Keys.N) && Game1.IsHudDrawn)
                Game1.player.shiftToolbar(true);
              if (currentKBState.IsKeyDown(Keys.F10) && !Game1.oldKBState.IsKeyDown(Keys.F10) && Game1.server == null)
                Game1.multiplayer.StartServer();
            }
            else if (!Game1.player.UsingTool)
            {
              if (Game1.isOneOfTheseKeysDown(currentKBState, Game1.options.inventorySlot1) && Game1.areAllOfTheseKeysUp(Game1.oldKBState, Game1.options.inventorySlot1))
                Game1.player.CurrentToolIndex = 0;
              else if (Game1.isOneOfTheseKeysDown(currentKBState, Game1.options.inventorySlot2) && Game1.areAllOfTheseKeysUp(Game1.oldKBState, Game1.options.inventorySlot2))
                Game1.player.CurrentToolIndex = 1;
              else if (Game1.isOneOfTheseKeysDown(currentKBState, Game1.options.inventorySlot3) && Game1.areAllOfTheseKeysUp(Game1.oldKBState, Game1.options.inventorySlot3))
                Game1.player.CurrentToolIndex = 2;
              else if (Game1.isOneOfTheseKeysDown(currentKBState, Game1.options.inventorySlot4) && Game1.areAllOfTheseKeysUp(Game1.oldKBState, Game1.options.inventorySlot4))
                Game1.player.CurrentToolIndex = 3;
              else if (Game1.isOneOfTheseKeysDown(currentKBState, Game1.options.inventorySlot5) && Game1.areAllOfTheseKeysUp(Game1.oldKBState, Game1.options.inventorySlot5))
                Game1.player.CurrentToolIndex = 4;
              else if (Game1.isOneOfTheseKeysDown(currentKBState, Game1.options.inventorySlot6) && Game1.areAllOfTheseKeysUp(Game1.oldKBState, Game1.options.inventorySlot6))
                Game1.player.CurrentToolIndex = 5;
              else if (Game1.isOneOfTheseKeysDown(currentKBState, Game1.options.inventorySlot7) && Game1.areAllOfTheseKeysUp(Game1.oldKBState, Game1.options.inventorySlot7))
                Game1.player.CurrentToolIndex = 6;
              else if (Game1.isOneOfTheseKeysDown(currentKBState, Game1.options.inventorySlot8) && Game1.areAllOfTheseKeysUp(Game1.oldKBState, Game1.options.inventorySlot8))
                Game1.player.CurrentToolIndex = 7;
              else if (Game1.isOneOfTheseKeysDown(currentKBState, Game1.options.inventorySlot9) && Game1.areAllOfTheseKeysUp(Game1.oldKBState, Game1.options.inventorySlot9))
                Game1.player.CurrentToolIndex = 8;
              else if (Game1.isOneOfTheseKeysDown(currentKBState, Game1.options.inventorySlot10) && Game1.areAllOfTheseKeysUp(Game1.oldKBState, Game1.options.inventorySlot10))
                Game1.player.CurrentToolIndex = 9;
              else if (Game1.isOneOfTheseKeysDown(currentKBState, Game1.options.inventorySlot11) && Game1.areAllOfTheseKeysUp(Game1.oldKBState, Game1.options.inventorySlot11))
                Game1.player.CurrentToolIndex = 10;
              else if (Game1.isOneOfTheseKeysDown(currentKBState, Game1.options.inventorySlot12) && Game1.areAllOfTheseKeysUp(Game1.oldKBState, Game1.options.inventorySlot12))
                Game1.player.CurrentToolIndex = 11;
            }
            if ((Game1.options.gamepadControls && Game1.rightStickHoldTime >= Game1.emoteMenuShowTime && Game1.activeClickableMenu == null || Game1.isOneOfTheseKeysDown(Game1.input.GetKeyboardState(), Game1.options.emoteButton) && Game1.areAllOfTheseKeysUp(Game1.oldKBState, Game1.options.emoteButton)) && !Game1.debugMode && Game1.player.CanEmote())
            {
              if (Game1.player.CanMove)
                Game1.player.Halt();
              Game1.emoteMenu = new EmoteMenu();
              Game1.emoteMenu.gamepadMode = Game1.options.gamepadControls && Game1.rightStickHoldTime >= Game1.emoteMenuShowTime;
              Game1.timerUntilMouseFade = 0;
            }
            if (!Program.releaseBuild)
            {
              if (Game1.IsPressEvent(ref currentKBState, Keys.F3) || Game1.IsPressEvent(ref currentPadState, Buttons.LeftStick))
              {
                Game1.debugMode = !Game1.debugMode;
                if (Game1.gameMode == (byte) 11)
                  Game1.gameMode = (byte) 3;
              }
              if (Game1.IsPressEvent(ref currentKBState, Keys.F8))
                this.requestDebugInput();
            }
            if (currentKBState.IsKeyDown(Keys.F4) && !Game1.oldKBState.IsKeyDown(Keys.F4))
            {
              Game1.displayHUD = !Game1.displayHUD;
              Game1.playSound("smallSelect");
              if (!Game1.displayHUD)
                Game1.showGlobalMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3666"));
            }
            bool flag3 = Game1.isOneOfTheseKeysDown(currentKBState, Game1.options.menuButton) && Game1.areAllOfTheseKeysUp(Game1.oldKBState, Game1.options.menuButton);
            bool flag4 = Game1.isOneOfTheseKeysDown(currentKBState, Game1.options.journalButton) && Game1.areAllOfTheseKeysUp(Game1.oldKBState, Game1.options.journalButton);
            bool flag5 = Game1.isOneOfTheseKeysDown(currentKBState, Game1.options.mapButton) && Game1.areAllOfTheseKeysUp(Game1.oldKBState, Game1.options.mapButton);
            if (Game1.options.gamepadControls && !flag3)
              flag3 = currentPadState.IsButtonDown(Buttons.Start) && !Game1.oldPadState.IsButtonDown(Buttons.Start) || currentPadState.IsButtonDown(Buttons.B) && !Game1.oldPadState.IsButtonDown(Buttons.B);
            if (Game1.options.gamepadControls && !flag4)
              flag4 = currentPadState.IsButtonDown(Buttons.Back) && !Game1.oldPadState.IsButtonDown(Buttons.Back);
            if (Game1.options.gamepadControls && !flag5)
              flag5 = currentPadState.IsButtonDown(Buttons.Y) && !Game1.oldPadState.IsButtonDown(Buttons.Y);
            if (flag3 && Game1.CanShowPauseMenu())
            {
              if (Game1.activeClickableMenu == null)
              {
                Game1.PushUIMode();
                Game1.activeClickableMenu = (IClickableMenu) new GameMenu();
                Game1.PopUIMode();
              }
              else if (Game1.activeClickableMenu.readyToClose())
                Game1.exitActiveMenu();
            }
            if (((Game1.dayOfMonth <= 0 ? 0 : (Game1.player.CanMove ? 1 : 0)) & (flag4 ? 1 : 0)) != 0 && !Game1.dialogueUp && !flag2)
            {
              if (Game1.activeClickableMenu == null)
                Game1.activeClickableMenu = (IClickableMenu) new QuestLog();
            }
            else if (((!flag2 ? 0 : (Game1.CurrentEvent != null ? 1 : 0)) & (flag4 ? 1 : 0)) != 0 && !Game1.CurrentEvent.skipped && Game1.CurrentEvent.skippable)
            {
              Game1.CurrentEvent.skipped = true;
              Game1.CurrentEvent.skipEvent();
              Game1.freezeControls = false;
            }
            if (((!Game1.options.gamepadControls || Game1.dayOfMonth <= 0 || !Game1.player.CanMove ? 0 : (Game1.isAnyGamePadButtonBeingPressed() ? 1 : 0)) & (flag5 ? 1 : 0)) != 0 && !Game1.dialogueUp && !flag2)
            {
              if (Game1.activeClickableMenu == null)
              {
                Game1.PushUIMode();
                Game1.activeClickableMenu = (IClickableMenu) new GameMenu(GameMenu.craftingTab);
                Game1.PopUIMode();
              }
            }
            else if (((Game1.dayOfMonth <= 0 ? 0 : (Game1.player.CanMove ? 1 : 0)) & (flag5 ? 1 : 0)) != 0 && !Game1.dialogueUp && !flag2 && Game1.activeClickableMenu == null)
            {
              Game1.PushUIMode();
              Game1.activeClickableMenu = (IClickableMenu) new GameMenu(GameMenu.mapTab);
              Game1.PopUIMode();
            }
            Game1.checkForRunButton(currentKBState);
            Game1.oldKBState = currentKBState;
            Game1.oldMouseState = Game1.input.GetMouseState();
            Game1.oldPadState = currentPadState;
          }
        }
      }
    }));
  }

  public static bool CanShowPauseMenu()
  {
    return Game1.dayOfMonth > 0 && Game1.player.CanMove && !Game1.dialogueUp && (!Game1.eventUp || Game1.isFestival() && Game1.CurrentEvent.festivalTimer <= 0) && Game1.currentMinigame == null && Game1.farmEvent == null;
  }

  internal static void addHour()
  {
    Game1.timeOfDay += 100;
    foreach (GameLocation location in (IEnumerable<GameLocation>) Game1.locations)
    {
      for (int index = 0; index < location.characters.Count; ++index)
      {
        NPC character = location.characters[index];
        character.checkSchedule(Game1.timeOfDay);
        character.checkSchedule(Game1.timeOfDay - 50);
        character.checkSchedule(Game1.timeOfDay - 60);
        character.checkSchedule(Game1.timeOfDay - 70);
        character.checkSchedule(Game1.timeOfDay - 80 /*0x50*/);
        character.checkSchedule(Game1.timeOfDay - 90);
      }
    }
    switch (Game1.timeOfDay)
    {
      case 1900:
        Game1.currentLocation.switchOutNightTiles();
        break;
      case 2000:
        if (Game1.currentLocation.IsRainingHere())
          break;
        Game1.changeMusicTrack("none");
        break;
    }
  }

  internal static void addMinute()
  {
    if (Game1.GetKeyboardState().IsKeyDown(Keys.LeftShift))
      Game1.timeOfDay -= 10;
    else
      Game1.timeOfDay += 10;
    if (Game1.timeOfDay % 100 == 60)
      Game1.timeOfDay += 40;
    if (Game1.timeOfDay % 100 == 90)
      Game1.timeOfDay -= 40;
    Game1.currentLocation.performTenMinuteUpdate(Game1.timeOfDay);
    foreach (GameLocation location in (IEnumerable<GameLocation>) Game1.locations)
    {
      for (int index = 0; index < location.characters.Count; ++index)
        location.characters[index].checkSchedule(Game1.timeOfDay);
    }
    if (Game1.isLightning && Game1.IsMasterGame)
      Utility.performLightningUpdate(Game1.timeOfDay);
    switch (Game1.timeOfDay)
    {
      case 1750:
        Game1.outdoorLight = Color.White;
        break;
      case 1900:
        Game1.currentLocation.switchOutNightTiles();
        break;
      case 2000:
        if (Game1.currentLocation.IsRainingHere())
          break;
        Game1.changeMusicTrack("none");
        break;
    }
  }

  public static void checkForRunButton(KeyboardState kbState, bool ignoreKeyPressQualifier = false)
  {
    bool running = Game1.player.running;
    bool flag1 = Game1.isOneOfTheseKeysDown(kbState, Game1.options.runButton) && !Game1.isOneOfTheseKeysDown(Game1.oldKBState, Game1.options.runButton) | ignoreKeyPressQualifier;
    bool flag2 = !Game1.isOneOfTheseKeysDown(kbState, Game1.options.runButton) && Game1.isOneOfTheseKeysDown(Game1.oldKBState, Game1.options.runButton) | ignoreKeyPressQualifier;
    if (Game1.options.gamepadControls)
    {
      if (!Game1.options.autoRun && (double) Math.Abs(Vector2.Distance(Game1.input.GetGamePadState().ThumbSticks.Left, Vector2.Zero)) > 0.89999997615814209)
        flag1 = true;
      else if ((double) Math.Abs(Vector2.Distance(Game1.oldPadState.ThumbSticks.Left, Vector2.Zero)) > 0.89999997615814209 && (double) Math.Abs(Vector2.Distance(Game1.input.GetGamePadState().ThumbSticks.Left, Vector2.Zero)) <= 0.89999997615814209)
        flag2 = true;
    }
    if (flag1 && !Game1.player.canOnlyWalk)
    {
      Game1.player.setRunning(!Game1.options.autoRun);
      Game1.player.setMoving(Game1.player.running ? (byte) 16 /*0x10*/ : (byte) 48 /*0x30*/);
    }
    else if (flag2 && !Game1.player.canOnlyWalk)
    {
      Game1.player.setRunning(Game1.options.autoRun);
      Game1.player.setMoving(Game1.player.running ? (byte) 16 /*0x10*/ : (byte) 48 /*0x30*/);
    }
    if (Game1.player.running == running || Game1.player.UsingTool)
      return;
    Game1.player.Halt();
  }

  public static Vector2 getMostRecentViewportMotion()
  {
    return new Vector2((float) Game1.viewport.X - Game1.previousViewportPosition.X, (float) Game1.viewport.Y - Game1.previousViewportPosition.Y);
  }

  public RenderTarget2D screen
  {
    get => this._screen;
    set
    {
      if (this._screen != null)
      {
        this._screen.Dispose();
        this._screen = (RenderTarget2D) null;
      }
      this._screen = value;
    }
  }

  public RenderTarget2D uiScreen
  {
    get => this._uiScreen;
    set
    {
      if (this._uiScreen != null)
      {
        this._uiScreen.Dispose();
        this._uiScreen = (RenderTarget2D) null;
      }
      this._uiScreen = value;
    }
  }

  protected virtual void DrawOverlays(GameTime time, RenderTarget2D target_screen)
  {
    if (this.takingMapScreenshot)
      return;
    Game1.PushUIMode();
    Game1.spriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
    if (Game1.hooks.OnRendering(RenderSteps.Overlays, Game1.spriteBatch, time, target_screen))
    {
      Game1.specialCurrencyDisplay?.Draw(Game1.spriteBatch);
      Game1.emoteMenu?.draw(Game1.spriteBatch);
      Game1.currentLocation?.drawOverlays(Game1.spriteBatch);
      if (Game1.HostPaused && !this.takingMapScreenshot)
      {
        string s = Game1.content.LoadString("Strings\\StringsFromCSFiles:DayTimeMoneyBox.cs.10378");
        SpriteText.drawStringWithScrollBackground(Game1.spriteBatch, s, 96 /*0x60*/, 32 /*0x20*/);
      }
      if (Game1.overlayMenu != null)
      {
        if (Game1.hooks.OnRendering(RenderSteps.Overlays_OverlayMenu, Game1.spriteBatch, time, target_screen))
          Game1.overlayMenu.draw(Game1.spriteBatch);
        Game1.hooks.OnRendered(RenderSteps.Overlays_OverlayMenu, Game1.spriteBatch, time, target_screen);
      }
      if (Game1.chatBox != null)
      {
        if (Game1.hooks.OnRendering(RenderSteps.Overlays_Chatbox, Game1.spriteBatch, time, target_screen))
        {
          Game1.chatBox.update(Game1.currentGameTime);
          Game1.chatBox.draw(Game1.spriteBatch);
        }
        Game1.hooks.OnRendered(RenderSteps.Overlays_Chatbox, Game1.spriteBatch, time, target_screen);
      }
      if (Game1.textEntry != null)
      {
        if (Game1.hooks.OnRendering(RenderSteps.Overlays_OnscreenKeyboard, Game1.spriteBatch, time, target_screen))
          Game1.textEntry.draw(Game1.spriteBatch);
        Game1.hooks.OnRendered(RenderSteps.Overlays_OnscreenKeyboard, Game1.spriteBatch, time, target_screen);
      }
      if ((Game1.displayHUD || Game1.eventUp || Game1.currentLocation is Summit) && Game1.gameMode == (byte) 3 && !Game1.freezeControls && !Game1.panMode)
        this.drawMouseCursor();
    }
    Game1.hooks.OnRendered(RenderSteps.Overlays, Game1.spriteBatch, time, target_screen);
    Game1.spriteBatch.End();
    Game1.PopUIMode();
  }

  public static void setBGColor(byte r, byte g, byte b)
  {
    Game1.bgColor.R = r;
    Game1.bgColor.G = g;
    Game1.bgColor.B = b;
  }

  public void Instance_Draw(GameTime gameTime) => this.Draw(gameTime);

  /// <summary>This is called when the game should draw itself.</summary>
  /// <param name="gameTime">Provides a snapshot of timing values.</param>
  protected override void Draw(GameTime gameTime)
  {
    this.isDrawing = true;
    RenderTarget2D renderTarget2D = (RenderTarget2D) null;
    if (this.ShouldDrawOnBuffer())
      renderTarget2D = this.screen;
    if (this.uiScreen != null)
    {
      Game1.SetRenderTarget(this.uiScreen);
      this.GraphicsDevice.Clear(Color.Transparent);
      Game1.SetRenderTarget(renderTarget2D);
    }
    GameTime time = gameTime;
    DebugTools.BeforeGameDraw(this, ref time);
    this._draw(time, renderTarget2D);
    Game1.isRenderingScreenBuffer = true;
    this.renderScreenBuffer(renderTarget2D);
    Game1.isRenderingScreenBuffer = false;
    if (Game1.uiModeCount != 0)
    {
      Game1.log.Warn("WARNING: Mismatched UI Mode Push/Pop counts. Correcting.");
      while (Game1.uiModeCount < 0)
        Game1.PushUIMode();
      while (Game1.uiModeCount > 0)
        Game1.PopUIMode();
    }
    base.Draw(gameTime);
    this.isDrawing = false;
  }

  public virtual bool ShouldDrawOnBuffer()
  {
    return LocalMultiplayer.IsLocalMultiplayer() || (double) Game1.options.zoomLevel != 1.0;
  }

  public static bool ShouldShowOnscreenUsernames() => false;

  public virtual bool checkCharacterTilesForShadowDrawFlag(Character character)
  {
    if (character is Farmer farmer && farmer.onBridge.Value)
      return true;
    Microsoft.Xna.Framework.Rectangle boundingBox = character.GetBoundingBox();
    boundingBox.Height += 8;
    int num1 = boundingBox.Right / 64 /*0x40*/;
    int num2 = boundingBox.Bottom / 64 /*0x40*/;
    int num3 = boundingBox.Left / 64 /*0x40*/;
    int num4 = boundingBox.Top / 64 /*0x40*/;
    for (int x = num3; x <= num1; ++x)
    {
      for (int y = num4; y <= num2; ++y)
      {
        if (Game1.currentLocation.shouldShadowBeDrawnAboveBuildingsLayer(new Vector2((float) x, (float) y)))
          return true;
      }
    }
    return false;
  }

  protected virtual void _draw(GameTime gameTime, RenderTarget2D target_screen)
  {
    Game1.debugTimings.StartDrawTimer();
    Game1.showingHealthBar = false;
    if (Game1._newDayTask != null || this.isLocalMultiplayerNewDayActive || this.ShouldLoadIncrementally)
    {
      this.GraphicsDevice.Clear(Game1.bgColor);
    }
    else
    {
      if (target_screen != null)
        Game1.SetRenderTarget(target_screen);
      if (this.IsSaving)
      {
        this.GraphicsDevice.Clear(Game1.bgColor);
        this.DrawMenu(gameTime, target_screen);
        Game1.PushUIMode();
        if (Game1.overlayMenu != null)
        {
          Game1.spriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
          Game1.overlayMenu.draw(Game1.spriteBatch);
          Game1.spriteBatch.End();
        }
        Game1.PopUIMode();
      }
      else
      {
        this.GraphicsDevice.Clear(Game1.bgColor);
        if (Game1.hooks.OnRendering(RenderSteps.FullScene, Game1.spriteBatch, gameTime, target_screen))
        {
          if (Game1.gameMode == (byte) 11)
          {
            Game1.spriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
            Game1.spriteBatch.DrawString(Game1.dialogueFont, Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3685"), new Vector2(16f, 16f), Color.HotPink);
            Game1.spriteBatch.DrawString(Game1.dialogueFont, Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3686"), new Vector2(16f, 32f), new Color(0, (int) byte.MaxValue, 0));
            Game1.spriteBatch.DrawString(Game1.dialogueFont, Game1.parseText(Game1.errorMessage, Game1.dialogueFont, Game1.graphics.GraphicsDevice.Viewport.Width), new Vector2(16f, 48f), Color.White);
            Game1.spriteBatch.End();
            return;
          }
          bool flag1 = true;
          if (Game1.activeClickableMenu != null && Game1.options.showMenuBackground && Game1.activeClickableMenu.showWithoutTransparencyIfOptionIsSet() && !this.takingMapScreenshot)
          {
            Game1.PushUIMode();
            Game1.spriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
            if (Game1.hooks.OnRendering(RenderSteps.MenuBackground, Game1.spriteBatch, gameTime, target_screen))
            {
              Game1.activeClickableMenu.drawBackground(Game1.spriteBatch);
              flag1 = false;
            }
            Game1.hooks.OnRendered(RenderSteps.MenuBackground, Game1.spriteBatch, gameTime, target_screen);
            Game1.spriteBatch.End();
            Game1.PopUIMode();
          }
          if (Game1.currentMinigame != null)
          {
            if (Game1.hooks.OnRendering(RenderSteps.Minigame, Game1.spriteBatch, gameTime, target_screen))
            {
              Game1.currentMinigame.draw(Game1.spriteBatch);
              flag1 = false;
            }
            Game1.hooks.OnRendered(RenderSteps.Minigame, Game1.spriteBatch, gameTime, target_screen);
          }
          switch (Game1.gameMode)
          {
            case 3:
              if (Game1.currentLocation != null)
                break;
              goto case 6;
            case 6:
              if (Game1.hooks.OnRendering(RenderSteps.LoadingScreen, Game1.spriteBatch, gameTime, target_screen))
                this.DrawLoadScreen(gameTime, target_screen);
              Game1.hooks.OnRendered(RenderSteps.LoadingScreen, Game1.spriteBatch, gameTime, target_screen);
              flag1 = false;
              break;
          }
          if (Game1.showingEndOfNightStuff)
            flag1 = false;
          else if (Game1.gameMode == (byte) 0)
            flag1 = false;
          if (Game1.gameMode == (byte) 3 && Game1.dayOfMonth == 0 && Game1.newDay)
          {
            base.Draw(gameTime);
            return;
          }
          if (flag1)
          {
            this.DrawWorld(gameTime, target_screen);
            Game1.PushUIMode();
            Game1.spriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
            if (Game1.hooks.OnRendering(RenderSteps.HUD, Game1.spriteBatch, gameTime, target_screen))
            {
              if (Game1.IsHudDrawn)
                this.drawHUD();
              if (Game1.hudMessages.Count > 0 && !this.takingMapScreenshot)
              {
                int heightUsed = 0;
                for (int index = Game1.hudMessages.Count - 1; index >= 0; --index)
                  Game1.hudMessages[index].draw(Game1.spriteBatch, index, ref heightUsed);
              }
            }
            Game1.hooks.OnRendered(RenderSteps.HUD, Game1.spriteBatch, gameTime, target_screen);
            Game1.debugTimings.Draw();
            Game1.spriteBatch.End();
            Game1.PopUIMode();
          }
          bool flag2 = false;
          if (!this.takingMapScreenshot)
          {
            Game1.spriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
            Game1.PushUIMode();
            if ((Game1.messagePause || Game1.globalFade) && Game1.dialogueUp)
              flag2 = true;
            else if (Game1.dialogueUp && !Game1.messagePause && (Game1.activeClickableMenu == null || !(Game1.activeClickableMenu is DialogueBox)))
            {
              if (Game1.hooks.OnRendering(RenderSteps.DialogueBox, Game1.spriteBatch, gameTime, target_screen))
                this.drawDialogueBox();
              Game1.hooks.OnRendered(RenderSteps.DialogueBox, Game1.spriteBatch, gameTime, target_screen);
            }
            Game1.spriteBatch.End();
            Game1.PopUIMode();
            this.DrawGlobalFade(gameTime, target_screen);
            if (flag2)
            {
              Game1.spriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
              Game1.PushUIMode();
              if (Game1.hooks.OnRendering(RenderSteps.DialogueBox, Game1.spriteBatch, gameTime, target_screen))
                this.drawDialogueBox();
              Game1.hooks.OnRendered(RenderSteps.DialogueBox, Game1.spriteBatch, gameTime, target_screen);
              Game1.spriteBatch.End();
              Game1.PopUIMode();
            }
            this.DrawScreenOverlaySprites(gameTime, target_screen);
            if (Game1.debugMode)
              this.DrawDebugUIs(gameTime, target_screen);
            this.DrawMenu(gameTime, target_screen);
          }
          Game1.farmEvent?.drawAboveEverything(Game1.spriteBatch);
          this.DrawOverlays(gameTime, target_screen);
        }
        Game1.hooks.OnRendered(RenderSteps.FullScene, Game1.spriteBatch, gameTime, target_screen);
        Game1.debugTimings.StopDrawTimer();
      }
    }
  }

  public virtual void DrawLoadScreen(GameTime time, RenderTarget2D target_screen)
  {
    Game1.PushUIMode();
    this.GraphicsDevice.Clear(Game1.bgColor);
    Game1.spriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
    string str1 = "".PadRight((int) Math.Ceiling(time.TotalGameTime.TotalMilliseconds % 999.0 / 333.0), '.');
    string str2 = Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3688");
    string s = str2 + str1;
    string str3 = str2 + "... ";
    int widthOfString = SpriteText.getWidthOfString(str3);
    int height = 64 /*0x40*/;
    int x = 64 /*0x40*/;
    int y = Game1.graphics.GraphicsDevice.Viewport.GetTitleSafeArea().Bottom - height;
    SpriteText.drawString(Game1.spriteBatch, s, x, y, width: widthOfString, height: height, drawBGScroll: 0, placeHolderScrollWidthText: str3);
    Game1.spriteBatch.End();
    Game1.PopUIMode();
  }

  public virtual void DrawMenu(GameTime time, RenderTarget2D target_screen)
  {
    Game1.PushUIMode();
    Game1.spriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
    if (Game1.hooks.OnRendering(RenderSteps.Menu, Game1.spriteBatch, time, target_screen))
    {
      IClickableMenu menu = Game1.activeClickableMenu;
      while (menu != null && Game1.hooks.TryDrawMenu(menu, (Action) (() => menu.draw(Game1.spriteBatch))))
        menu = menu.GetChildMenu();
    }
    Game1.hooks.OnRendered(RenderSteps.Menu, Game1.spriteBatch, time, target_screen);
    Game1.spriteBatch.End();
    Game1.PopUIMode();
  }

  public virtual void DrawScreenOverlaySprites(GameTime time, RenderTarget2D target_screen)
  {
    if (Game1.hooks.OnRendering(RenderSteps.OverlayTemporarySprites, Game1.spriteBatch, time, target_screen))
    {
      Game1.spriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
      foreach (TemporaryAnimatedSprite overlayTempSprite in Game1.screenOverlayTempSprites)
        overlayTempSprite.draw(Game1.spriteBatch, true);
      Game1.spriteBatch.End();
      Game1.PushUIMode();
      Game1.spriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
      foreach (TemporaryAnimatedSprite overlayTempSprite in Game1.uiOverlayTempSprites)
        overlayTempSprite.draw(Game1.spriteBatch, true);
      Game1.spriteBatch.End();
      Game1.PopUIMode();
    }
    Game1.hooks.OnRendered(RenderSteps.OverlayTemporarySprites, Game1.spriteBatch, time, target_screen);
  }

  public virtual void DrawWorld(GameTime time, RenderTarget2D target_screen)
  {
    if (Game1.hooks.OnRendering(RenderSteps.World, Game1.spriteBatch, time, target_screen))
    {
      Game1.mapDisplayDevice.BeginScene(Game1.spriteBatch);
      if (Game1.drawLighting)
        this.DrawLighting(time, target_screen);
      this.GraphicsDevice.Clear(Game1.bgColor);
      Game1.spriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
      if (Game1.hooks.OnRendering(RenderSteps.World_Background, Game1.spriteBatch, time, target_screen))
      {
        Game1.background?.draw(Game1.spriteBatch);
        Game1.currentLocation.drawBackground(Game1.spriteBatch);
        Game1.spriteBatch.End();
        for (int index = 0; index < Game1.currentLocation.backgroundLayers.Count; ++index)
        {
          Game1.spriteBatch.Begin(SpriteSortMode.Texture, BlendState.AlphaBlend, SamplerState.PointClamp);
          Game1.currentLocation.backgroundLayers[index].Key.Draw(Game1.mapDisplayDevice, Game1.viewport, Location.Origin, false, 4, -1f);
          Game1.spriteBatch.End();
        }
        Game1.spriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
        Game1.currentLocation.drawWater(Game1.spriteBatch);
        Game1.spriteBatch.End();
        Game1.spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp);
        Game1.currentLocation.drawFloorDecorations(Game1.spriteBatch);
        Game1.spriteBatch.End();
      }
      Game1.hooks.OnRendered(RenderSteps.World_Background, Game1.spriteBatch, time, target_screen);
      Game1.spriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
      this._farmerShadows.Clear();
      if (Game1.currentLocation.currentEvent != null && !Game1.currentLocation.currentEvent.isFestival && Game1.currentLocation.currentEvent.farmerActors.Count > 0)
      {
        foreach (Farmer farmerActor in Game1.currentLocation.currentEvent.farmerActors)
        {
          if (farmerActor.IsLocalPlayer && Game1.displayFarmer || !farmerActor.hidden.Value)
            this._farmerShadows.Add(farmerActor);
        }
      }
      else
      {
        foreach (Farmer farmer in Game1.currentLocation.farmers)
        {
          if (farmer.IsLocalPlayer && Game1.displayFarmer || !farmer.hidden.Value)
            this._farmerShadows.Add(farmer);
        }
      }
      if (!Game1.currentLocation.shouldHideCharacters())
      {
        if (Game1.CurrentEvent == null)
        {
          foreach (NPC character in Game1.currentLocation.characters)
          {
            if (!character.swimming.Value && !character.HideShadow && !character.IsInvisible && !this.checkCharacterTilesForShadowDrawFlag((Character) character))
              character.DrawShadow(Game1.spriteBatch);
          }
        }
        else
        {
          foreach (NPC actor in Game1.CurrentEvent.actors)
          {
            if ((Game1.CurrentEvent == null || !Game1.CurrentEvent.ShouldHideCharacter(actor)) && !actor.swimming.Value && !actor.HideShadow && !this.checkCharacterTilesForShadowDrawFlag((Character) actor))
              actor.DrawShadow(Game1.spriteBatch);
          }
        }
        foreach (Farmer farmerShadow in this._farmerShadows)
        {
          if (!Game1.multiplayer.isDisconnecting(farmerShadow.UniqueMultiplayerID) && !farmerShadow.swimming.Value && !farmerShadow.isRidingHorse() && !farmerShadow.IsSitting() && (Game1.currentLocation == null || !this.checkCharacterTilesForShadowDrawFlag((Character) farmerShadow)))
            farmerShadow.DrawShadow(Game1.spriteBatch);
        }
      }
      float num1 = 0.1f;
      for (int index = 0; index < Game1.currentLocation.buildingLayers.Count; ++index)
      {
        float num2 = 0.0f;
        if (Game1.currentLocation.buildingLayers.Count > 1)
          num2 = (float) index / (float) (Game1.currentLocation.buildingLayers.Count - 1);
        Game1.currentLocation.buildingLayers[index].Key.Draw(Game1.mapDisplayDevice, Game1.viewport, Location.Origin, false, 4, num1 * num2);
      }
      Layer layer = Game1.currentLocation.Map.RequireLayer("Buildings");
      Game1.spriteBatch.End();
      Game1.spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp);
      if (Game1.hooks.OnRendering(RenderSteps.World_Sorted, Game1.spriteBatch, time, target_screen))
      {
        if (!Game1.currentLocation.shouldHideCharacters())
        {
          if (Game1.CurrentEvent == null)
          {
            foreach (NPC character in Game1.currentLocation.characters)
            {
              if (!character.swimming.Value && !character.HideShadow && !character.isInvisible.Value && this.checkCharacterTilesForShadowDrawFlag((Character) character))
                character.DrawShadow(Game1.spriteBatch);
            }
          }
          else
          {
            foreach (NPC actor in Game1.CurrentEvent.actors)
            {
              if ((Game1.CurrentEvent == null || !Game1.CurrentEvent.ShouldHideCharacter(actor)) && !actor.swimming.Value && !actor.HideShadow && this.checkCharacterTilesForShadowDrawFlag((Character) actor))
                actor.DrawShadow(Game1.spriteBatch);
            }
          }
          foreach (Farmer farmerShadow in this._farmerShadows)
          {
            if (!farmerShadow.swimming.Value && !farmerShadow.isRidingHorse() && !farmerShadow.IsSitting() && Game1.currentLocation != null && this.checkCharacterTilesForShadowDrawFlag((Character) farmerShadow))
              farmerShadow.DrawShadow(Game1.spriteBatch);
          }
        }
        if ((Game1.eventUp || Game1.killScreen) && !Game1.killScreen && Game1.currentLocation.currentEvent != null)
          Game1.currentLocation.currentEvent.draw(Game1.spriteBatch);
        Game1.currentLocation.draw(Game1.spriteBatch);
        foreach (Vector2 key in Game1.crabPotOverlayTiles.Keys)
        {
          Tile tile = layer.Tiles[(int) key.X, (int) key.Y];
          if (tile != null)
          {
            Vector2 local = Game1.GlobalToLocal(Game1.viewport, key * 64f);
            Location location = new Location((int) local.X, (int) local.Y);
            Game1.mapDisplayDevice.DrawTile(tile, location, (float) (((double) key.Y * 64.0 - 1.0) / 10000.0));
          }
        }
        if (Game1.player.ActiveObject == null && Game1.player.UsingTool && Game1.player.CurrentTool != null)
          Game1.drawTool(Game1.player);
        if (Game1.panMode)
        {
          Game1.spriteBatch.Draw(Game1.fadeToBlackRect, new Microsoft.Xna.Framework.Rectangle((int) Math.Floor((double) (Game1.getOldMouseX() + Game1.viewport.X) / 64.0) * 64 /*0x40*/ - Game1.viewport.X, (int) Math.Floor((double) (Game1.getOldMouseY() + Game1.viewport.Y) / 64.0) * 64 /*0x40*/ - Game1.viewport.Y, 64 /*0x40*/, 64 /*0x40*/), Color.Lime * 0.75f);
          foreach (Warp warp in (NetList<Warp, NetRef<Warp>>) Game1.currentLocation.warps)
            Game1.spriteBatch.Draw(Game1.fadeToBlackRect, new Microsoft.Xna.Framework.Rectangle(warp.X * 64 /*0x40*/ - Game1.viewport.X, warp.Y * 64 /*0x40*/ - Game1.viewport.Y, 64 /*0x40*/, 64 /*0x40*/), Color.Red * 0.75f);
        }
        for (int index = 0; index < Game1.currentLocation.frontLayers.Count; ++index)
        {
          float num3 = 0.0f;
          if (Game1.currentLocation.frontLayers.Count > 1)
            num3 = (float) index / (float) (Game1.currentLocation.frontLayers.Count - 1);
          Game1.currentLocation.frontLayers[index].Key.Draw(Game1.mapDisplayDevice, Game1.viewport, Location.Origin, false, 4, (float) (64.0 + (double) num1 * (double) num3));
        }
        Game1.currentLocation.drawAboveFrontLayer(Game1.spriteBatch);
      }
      Game1.hooks.OnRendered(RenderSteps.World_Sorted, Game1.spriteBatch, time, target_screen);
      Game1.spriteBatch.End();
      if (Game1.hooks.OnRendering(RenderSteps.World_AlwaysFront, Game1.spriteBatch, time, target_screen))
      {
        for (int index = 0; index < Game1.currentLocation.alwaysFrontLayers.Count; ++index)
        {
          Game1.spriteBatch.Begin(SpriteSortMode.Texture, BlendState.AlphaBlend, SamplerState.PointClamp);
          Game1.currentLocation.alwaysFrontLayers[index].Key.Draw(Game1.mapDisplayDevice, Game1.viewport, Location.Origin, false, 4, -1f);
          Game1.spriteBatch.End();
        }
      }
      Game1.hooks.OnRendered(RenderSteps.World_AlwaysFront, Game1.spriteBatch, time, target_screen);
      Game1.spriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
      Viewport viewport;
      if ((double) Game1.currentLocation.LightLevel > 0.0 && Game1.timeOfDay < 2000)
      {
        SpriteBatch spriteBatch = Game1.spriteBatch;
        Texture2D fadeToBlackRect = Game1.fadeToBlackRect;
        viewport = Game1.graphics.GraphicsDevice.Viewport;
        Microsoft.Xna.Framework.Rectangle bounds = viewport.Bounds;
        Color color = Color.Black * Game1.currentLocation.LightLevel;
        spriteBatch.Draw(fadeToBlackRect, bounds, color);
      }
      if (Game1.screenGlow)
      {
        SpriteBatch spriteBatch = Game1.spriteBatch;
        Texture2D fadeToBlackRect = Game1.fadeToBlackRect;
        viewport = Game1.graphics.GraphicsDevice.Viewport;
        Microsoft.Xna.Framework.Rectangle bounds = viewport.Bounds;
        Color color = Game1.screenGlowColor * Game1.screenGlowAlpha;
        spriteBatch.Draw(fadeToBlackRect, bounds, color);
      }
      Game1.spriteBatch.End();
      Game1.spriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
      Game1.currentLocation.drawAboveAlwaysFrontLayer(Game1.spriteBatch);
      if (!Game1.IsFakedBlackScreen())
      {
        Game1.spriteBatch.End();
        this.drawWeather(time, target_screen);
        Game1.spriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
      }
      if (Game1.player.CurrentTool is FishingRod currentTool && (currentTool.isTimingCast || (double) currentTool.castingChosenCountdown > 0.0 || currentTool.fishCaught || currentTool.showingTreasure))
        Game1.player.CurrentTool.draw(Game1.spriteBatch);
      Game1.spriteBatch.End();
      this.DrawCharacterEmotes(time, target_screen);
      Game1.mapDisplayDevice.EndScene();
      if (Game1.drawLighting && !Game1.IsFakedBlackScreen())
        this.DrawLightmapOnScreen(time, target_screen);
      if (!Game1.eventUp && Game1.farmEvent == null && Game1.gameMode == (byte) 3 && !this.takingMapScreenshot && Game1.isOutdoorMapSmallerThanViewport())
      {
        Game1.spriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
        SpriteBatch spriteBatch1 = Game1.spriteBatch;
        Texture2D fadeToBlackRect1 = Game1.fadeToBlackRect;
        int width1 = -Game1.viewport.X;
        viewport = Game1.graphics.GraphicsDevice.Viewport;
        int height1 = viewport.Height;
        Microsoft.Xna.Framework.Rectangle destinationRectangle1 = new Microsoft.Xna.Framework.Rectangle(0, 0, width1, height1);
        Color black1 = Color.Black;
        spriteBatch1.Draw(fadeToBlackRect1, destinationRectangle1, black1);
        SpriteBatch spriteBatch2 = Game1.spriteBatch;
        Texture2D fadeToBlackRect2 = Game1.fadeToBlackRect;
        int x = -Game1.viewport.X + Game1.currentLocation.map.Layers[0].LayerWidth * 64 /*0x40*/;
        viewport = Game1.graphics.GraphicsDevice.Viewport;
        int width2 = viewport.Width - (-Game1.viewport.X + Game1.currentLocation.map.Layers[0].LayerWidth * 64 /*0x40*/);
        viewport = Game1.graphics.GraphicsDevice.Viewport;
        int height2 = viewport.Height;
        Microsoft.Xna.Framework.Rectangle destinationRectangle2 = new Microsoft.Xna.Framework.Rectangle(x, 0, width2, height2);
        Color black2 = Color.Black;
        spriteBatch2.Draw(fadeToBlackRect2, destinationRectangle2, black2);
        SpriteBatch spriteBatch3 = Game1.spriteBatch;
        Texture2D fadeToBlackRect3 = Game1.fadeToBlackRect;
        viewport = Game1.graphics.GraphicsDevice.Viewport;
        Microsoft.Xna.Framework.Rectangle destinationRectangle3 = new Microsoft.Xna.Framework.Rectangle(0, 0, viewport.Width, -Game1.viewport.Y);
        Color black3 = Color.Black;
        spriteBatch3.Draw(fadeToBlackRect3, destinationRectangle3, black3);
        SpriteBatch spriteBatch4 = Game1.spriteBatch;
        Texture2D fadeToBlackRect4 = Game1.fadeToBlackRect;
        int y = -Game1.viewport.Y + Game1.currentLocation.map.Layers[0].LayerHeight * 64 /*0x40*/;
        viewport = Game1.graphics.GraphicsDevice.Viewport;
        int width3 = viewport.Width;
        viewport = Game1.graphics.GraphicsDevice.Viewport;
        int height3 = viewport.Height - (-Game1.viewport.Y + Game1.currentLocation.map.Layers[0].LayerHeight * 64 /*0x40*/);
        Microsoft.Xna.Framework.Rectangle destinationRectangle4 = new Microsoft.Xna.Framework.Rectangle(0, y, width3, height3);
        Color black4 = Color.Black;
        spriteBatch4.Draw(fadeToBlackRect4, destinationRectangle4, black4);
        Game1.spriteBatch.End();
      }
      Game1.spriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
      if (Game1.currentLocation != null && Game1.currentLocation.isOutdoors.Value && !Game1.IsFakedBlackScreen() && Game1.currentLocation.IsRainingHere())
      {
        bool flag = Game1.IsGreenRainingHere();
        SpriteBatch spriteBatch = Game1.spriteBatch;
        Texture2D staminaRect = Game1.staminaRect;
        viewport = Game1.graphics.GraphicsDevice.Viewport;
        Microsoft.Xna.Framework.Rectangle bounds = viewport.Bounds;
        Color color = flag ? new Color(0, 120, 150) * 0.22f : Color.Blue * 0.2f;
        spriteBatch.Draw(staminaRect, bounds, color);
      }
      Game1.spriteBatch.End();
      if (Game1.farmEvent != null)
      {
        Game1.spriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
        Game1.farmEvent.draw(Game1.spriteBatch);
        Game1.spriteBatch.End();
      }
      if (Game1.eventUp && Game1.currentLocation?.currentEvent != null)
      {
        Game1.spriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
        Game1.currentLocation.currentEvent.drawAfterMap(Game1.spriteBatch);
        Game1.spriteBatch.End();
      }
      if (!this.takingMapScreenshot)
      {
        if (Game1.drawGrid)
        {
          Game1.spriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
          int num4 = -Game1.viewport.X % 64 /*0x40*/;
          float num5 = (float) (-Game1.viewport.Y % 64 /*0x40*/);
          int num6 = num4;
          while (true)
          {
            int num7 = num6;
            viewport = Game1.graphics.GraphicsDevice.Viewport;
            int width = viewport.Width;
            if (num7 < width)
            {
              SpriteBatch spriteBatch = Game1.spriteBatch;
              Texture2D staminaRect = Game1.staminaRect;
              int x = num6;
              int y = (int) num5;
              viewport = Game1.graphics.GraphicsDevice.Viewport;
              int height = viewport.Height;
              Microsoft.Xna.Framework.Rectangle destinationRectangle = new Microsoft.Xna.Framework.Rectangle(x, y, 1, height);
              Color color = Color.Red * 0.5f;
              spriteBatch.Draw(staminaRect, destinationRectangle, color);
              num6 += 64 /*0x40*/;
            }
            else
              break;
          }
          float num8 = num5;
          while (true)
          {
            double num9 = (double) num8;
            viewport = Game1.graphics.GraphicsDevice.Viewport;
            double height = (double) viewport.Height;
            if (num9 < height)
            {
              SpriteBatch spriteBatch = Game1.spriteBatch;
              Texture2D staminaRect = Game1.staminaRect;
              int x = num4;
              int y = (int) num8;
              viewport = Game1.graphics.GraphicsDevice.Viewport;
              int width = viewport.Width;
              Microsoft.Xna.Framework.Rectangle destinationRectangle = new Microsoft.Xna.Framework.Rectangle(x, y, width, 1);
              Color color = Color.Red * 0.5f;
              spriteBatch.Draw(staminaRect, destinationRectangle, color);
              num8 += 64f;
            }
            else
              break;
          }
          Game1.spriteBatch.End();
        }
        if (Game1.ShouldShowOnscreenUsernames() && Game1.currentLocation != null)
        {
          Game1.spriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
          Game1.currentLocation.DrawFarmerUsernames(Game1.spriteBatch);
          Game1.spriteBatch.End();
        }
        if ((double) Game1.flashAlpha > 0.0)
        {
          if (Game1.options.screenFlash)
          {
            Game1.spriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
            SpriteBatch spriteBatch = Game1.spriteBatch;
            Texture2D fadeToBlackRect = Game1.fadeToBlackRect;
            viewport = Game1.graphics.GraphicsDevice.Viewport;
            Microsoft.Xna.Framework.Rectangle bounds = viewport.Bounds;
            Color color = Color.White * Math.Min(1f, Game1.flashAlpha);
            spriteBatch.Draw(fadeToBlackRect, bounds, color);
            Game1.spriteBatch.End();
          }
          Game1.flashAlpha -= 0.1f;
        }
      }
    }
    Game1.hooks.OnRendered(RenderSteps.World, Game1.spriteBatch, time, target_screen);
  }

  public virtual void DrawCharacterEmotes(GameTime time, RenderTarget2D target_screen)
  {
    if (!Game1.eventUp || Game1.currentLocation.currentEvent == null)
      return;
    Game1.spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp);
    foreach (NPC actor in Game1.currentLocation.currentEvent.actors)
      actor.DrawEmote(Game1.spriteBatch);
    Game1.spriteBatch.End();
  }

  public virtual void DrawLightmapOnScreen(GameTime time, RenderTarget2D target_screen)
  {
    if (Game1.hooks.OnRendering(RenderSteps.World_DrawLightmapOnScreen, Game1.spriteBatch, time, target_screen))
    {
      Game1.spriteBatch.Begin(blendState: this.lightingBlend, samplerState: SamplerState.LinearClamp);
      Viewport viewport = this.GraphicsDevice.Viewport with
      {
        Bounds = target_screen != null ? target_screen.Bounds : this.GraphicsDevice.PresentationParameters.Bounds
      };
      this.GraphicsDevice.Viewport = viewport;
      float scale = (float) (Game1.options.lightingQuality / 2);
      if (this.useUnscaledLighting)
        scale /= Game1.options.zoomLevel;
      Game1.spriteBatch.Draw((Texture2D) Game1.lightmap, Vector2.Zero, new Microsoft.Xna.Framework.Rectangle?(Game1.lightmap.Bounds), Color.White, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 1f);
      if (Game1.currentLocation.isOutdoors.Value && Game1.currentLocation.IsRainingHere())
        Game1.spriteBatch.Draw(Game1.lightingRect, viewport.Bounds, Color.OrangeRed * 0.45f);
    }
    Game1.hooks.OnRendered(RenderSteps.World_DrawLightmapOnScreen, Game1.spriteBatch, time, target_screen);
    Game1.spriteBatch.End();
  }

  public virtual void DrawDebugUIs(GameTime time, RenderTarget2D target_screen)
  {
    StringBuilder debugStringBuilder = Game1._debugStringBuilder;
    debugStringBuilder.Clear();
    if (Game1.panMode)
    {
      debugStringBuilder.Append((Game1.getOldMouseX() + Game1.viewport.X) / 64 /*0x40*/);
      debugStringBuilder.Append(",");
      debugStringBuilder.Append((Game1.getOldMouseY() + Game1.viewport.Y) / 64 /*0x40*/);
    }
    else
    {
      Point standingPixel = Game1.player.StandingPixel;
      debugStringBuilder.Append("player: ");
      debugStringBuilder.Append(standingPixel.X / 64 /*0x40*/);
      debugStringBuilder.Append(", ");
      debugStringBuilder.Append(standingPixel.Y / 64 /*0x40*/);
    }
    debugStringBuilder.Append(" mouseTransparency: ");
    debugStringBuilder.Append(Game1.mouseCursorTransparency);
    debugStringBuilder.Append(" mousePosition: ");
    debugStringBuilder.Append(Game1.getMouseX());
    debugStringBuilder.Append(",");
    debugStringBuilder.Append(Game1.getMouseY());
    debugStringBuilder.Append(Environment.NewLine);
    debugStringBuilder.Append(" mouseWorldPosition: ");
    debugStringBuilder.Append(Game1.getMouseX() + Game1.viewport.X);
    debugStringBuilder.Append(",");
    debugStringBuilder.Append(Game1.getMouseY() + Game1.viewport.Y);
    debugStringBuilder.Append("  debugOutput: ");
    debugStringBuilder.Append(Game1.debugOutput);
    Game1.PushUIMode();
    Game1.spriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
    Game1.spriteBatch.DrawString(Game1.smallFont, debugStringBuilder, new Vector2((float) this.GraphicsDevice.Viewport.GetTitleSafeArea().X, (float) (this.GraphicsDevice.Viewport.GetTitleSafeArea().Y + Game1.smallFont.LineSpacing * 8)), Color.Red, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.9999999f);
    Game1.spriteBatch.End();
    Game1.PopUIMode();
  }

  public virtual void DrawGlobalFade(GameTime time, RenderTarget2D target_screen)
  {
    if (!Game1.fadeToBlack && !Game1.globalFade || this.takingMapScreenshot)
      return;
    Game1.PushUIMode();
    Game1.spriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
    if (Game1.hooks.OnRendering(RenderSteps.GlobalFade, Game1.spriteBatch, time, target_screen))
      Game1.spriteBatch.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * (Game1.gameMode == (byte) 0 ? 1f - Game1.fadeToBlackAlpha : Game1.fadeToBlackAlpha));
    Game1.hooks.OnRendered(RenderSteps.GlobalFade, Game1.spriteBatch, time, target_screen);
    Game1.spriteBatch.End();
    Game1.PopUIMode();
  }

  public virtual void DrawLighting(GameTime time, RenderTarget2D target_screen)
  {
    Game1.SetRenderTarget(Game1.lightmap);
    this.GraphicsDevice.Clear(Color.White * 0.0f);
    Matrix matrix = Matrix.Identity;
    if (this.useUnscaledLighting)
      matrix = Matrix.CreateScale(Game1.options.zoomLevel);
    Game1.spriteBatch.Begin(blendState: BlendState.NonPremultiplied, samplerState: SamplerState.PointClamp, transformMatrix: new Matrix?(matrix));
    if (Game1.hooks.OnRendering(RenderSteps.World_RenderLightmap, Game1.spriteBatch, time, target_screen))
    {
      Color color = !(Game1.currentLocation is MineShaft currentLocation) ? (Game1.ambientLight.Equals(Color.White) || Game1.currentLocation.isOutdoors.Value && Game1.currentLocation.IsRainingHere() ? Game1.outdoorLight : Game1.ambientLight) : currentLocation.getLightingColor(time);
      float lightMultiplier = 1f;
      if (Game1.player.hasBuff("26"))
      {
        if (color == Color.White)
        {
          color = new Color(0.75f, 0.75f, 0.75f);
        }
        else
        {
          color.R = (byte) Utility.Lerp((float) color.R, (float) byte.MaxValue, 0.5f);
          color.G = (byte) Utility.Lerp((float) color.G, (float) byte.MaxValue, 0.5f);
          color.B = (byte) Utility.Lerp((float) color.B, (float) byte.MaxValue, 0.5f);
        }
        lightMultiplier = 0.33f;
      }
      if (Game1.IsGreenRainingHere())
      {
        color.R = (byte) Utility.Lerp((float) color.R, (float) byte.MaxValue, 0.25f);
        color.G = (byte) Utility.Lerp((float) color.R, 0.0f, 0.25f);
      }
      Game1.spriteBatch.Draw(Game1.staminaRect, Game1.lightmap.Bounds, color);
      foreach (KeyValuePair<string, LightSource> currentLightSource in Game1.currentLightSources)
        currentLightSource.Value.Draw(Game1.spriteBatch, Game1.currentLocation, lightMultiplier);
    }
    Game1.hooks.OnRendered(RenderSteps.World_RenderLightmap, Game1.spriteBatch, time, target_screen);
    Game1.spriteBatch.End();
    Game1.SetRenderTarget(target_screen);
  }

  public virtual void drawWeather(GameTime time, RenderTarget2D target_screen)
  {
    Game1.spriteBatch.Begin(SpriteSortMode.Texture, BlendState.AlphaBlend, SamplerState.PointClamp);
    if (Game1.hooks.OnRendering(RenderSteps.World_Weather, Game1.spriteBatch, time, target_screen) && Game1.currentLocation.IsOutdoors)
    {
      if (Game1.currentLocation.IsSnowingHere())
      {
        Game1.snowPos.X %= 64f;
        Vector2 position = new Vector2();
        for (float num1 = (float) ((double) Game1.snowPos.X % 64.0 - 64.0); (double) num1 < (double) Game1.viewport.Width; num1 += 64f)
        {
          for (float num2 = (float) ((double) Game1.snowPos.Y % 64.0 - 64.0); (double) num2 < (double) Game1.viewport.Height; num2 += 64f)
          {
            position.X = (float) (int) num1;
            position.Y = (float) (int) num2;
            Game1.spriteBatch.Draw(Game1.mouseCursors, position, new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(368 + (int) (Game1.currentGameTime.TotalGameTime.TotalMilliseconds % 1200.0) / 75 * 16 /*0x10*/, 192 /*0xC0*/, 16 /*0x10*/, 16 /*0x10*/)), Color.White * 0.8f * Game1.options.snowTransparency, 0.0f, Vector2.Zero, 4.001f, SpriteEffects.None, 1f);
          }
        }
      }
      if (!Game1.currentLocation.ignoreDebrisWeather.Value && Game1.currentLocation.IsDebrisWeatherHere())
      {
        if (this.takingMapScreenshot)
        {
          if (Game1.debrisWeather != null)
          {
            foreach (WeatherDebris weatherDebris in Game1.debrisWeather)
            {
              Vector2 position = weatherDebris.position;
              weatherDebris.position = new Vector2((float) Game1.random.Next(Game1.viewport.Width - weatherDebris.sourceRect.Width * 3), (float) Game1.random.Next(Game1.viewport.Height - weatherDebris.sourceRect.Height * 3));
              weatherDebris.draw(Game1.spriteBatch);
              weatherDebris.position = position;
            }
          }
        }
        else if (Game1.viewport.X > -Game1.viewport.Width)
        {
          foreach (WeatherDebris weatherDebris in Game1.debrisWeather)
            weatherDebris.draw(Game1.spriteBatch);
        }
      }
      if (Game1.currentLocation.IsRainingHere() && !(Game1.currentLocation is Summit) && (!Game1.eventUp || Game1.currentLocation.isTileOnMap(new Vector2((float) (Game1.viewport.X / 64 /*0x40*/), (float) (Game1.viewport.Y / 64 /*0x40*/)))))
      {
        bool flag = Game1.IsGreenRainingHere();
        Color color = flag ? Color.LimeGreen : Color.White;
        int num = flag ? 2 : 1;
        for (int index1 = 0; index1 < Game1.rainDrops.Length; ++index1)
        {
          for (int index2 = 0; index2 < num; ++index2)
            Game1.spriteBatch.Draw(Game1.rainTexture, Game1.rainDrops[index1].position, new Microsoft.Xna.Framework.Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.rainTexture, Game1.rainDrops[index1].frame + (flag ? 4 : 0), 16 /*0x10*/, 16 /*0x10*/)), color, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
        }
      }
    }
    Game1.hooks.OnRendered(RenderSteps.World_Weather, Game1.spriteBatch, time, target_screen);
    Game1.spriteBatch.End();
  }

  protected virtual void renderScreenBuffer(RenderTarget2D target_screen)
  {
    Game1.graphics.GraphicsDevice.SetRenderTarget((RenderTarget2D) null);
    if (this.takingMapScreenshot || LocalMultiplayer.IsLocalMultiplayer() || target_screen != null && target_screen.IsContentLost)
      return;
    if (this.ShouldDrawOnBuffer() && target_screen != null)
    {
      this.GraphicsDevice.Clear(Game1.bgColor);
      Game1.spriteBatch.Begin(blendState: BlendState.Opaque, samplerState: SamplerState.LinearClamp, depthStencilState: DepthStencilState.Default, rasterizerState: RasterizerState.CullNone);
      Game1.spriteBatch.Draw((Texture2D) target_screen, new Vector2(0.0f, 0.0f), new Microsoft.Xna.Framework.Rectangle?(target_screen.Bounds), Color.White, 0.0f, Vector2.Zero, Game1.options.zoomLevel, SpriteEffects.None, 1f);
      Game1.spriteBatch.End();
      Game1.spriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.LinearClamp, depthStencilState: DepthStencilState.Default, rasterizerState: RasterizerState.CullNone);
      Game1.spriteBatch.Draw((Texture2D) this.uiScreen, new Vector2(0.0f, 0.0f), new Microsoft.Xna.Framework.Rectangle?(this.uiScreen.Bounds), Color.White, 0.0f, Vector2.Zero, Game1.options.uiScale, SpriteEffects.None, 1f);
      Game1.spriteBatch.End();
    }
    else
    {
      Game1.spriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.LinearClamp, depthStencilState: DepthStencilState.Default, rasterizerState: RasterizerState.CullNone);
      Game1.spriteBatch.Draw((Texture2D) this.uiScreen, new Vector2(0.0f, 0.0f), new Microsoft.Xna.Framework.Rectangle?(this.uiScreen.Bounds), Color.White, 0.0f, Vector2.Zero, Game1.options.uiScale, SpriteEffects.None, 1f);
      Game1.spriteBatch.End();
    }
  }

  public virtual void DrawSplitScreenWindow()
  {
    if (!LocalMultiplayer.IsLocalMultiplayer())
      return;
    Game1.graphics.GraphicsDevice.SetRenderTarget((RenderTarget2D) null);
    if (this.screen != null && this.screen.IsContentLost)
      return;
    Viewport viewport = this.GraphicsDevice.Viewport;
    this.GraphicsDevice.Viewport = this.GraphicsDevice.Viewport = Game1.defaultDeviceViewport;
    Game1.spriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.LinearClamp, depthStencilState: DepthStencilState.Default, rasterizerState: RasterizerState.CullNone);
    Game1.spriteBatch.Draw((Texture2D) this.screen, new Vector2((float) this.localMultiplayerWindow.X, (float) this.localMultiplayerWindow.Y), new Microsoft.Xna.Framework.Rectangle?(this.screen.Bounds), Color.White, 0.0f, Vector2.Zero, this.instanceOptions.zoomLevel, SpriteEffects.None, 1f);
    if (this.uiScreen != null)
      Game1.spriteBatch.Draw((Texture2D) this.uiScreen, new Vector2((float) this.localMultiplayerWindow.X, (float) this.localMultiplayerWindow.Y), new Microsoft.Xna.Framework.Rectangle?(this.uiScreen.Bounds), Color.White, 0.0f, Vector2.Zero, this.instanceOptions.uiScale, SpriteEffects.None, 1f);
    Game1.spriteBatch.End();
    this.GraphicsDevice.Viewport = viewport;
  }

  /// ###########################
  ///             METHODS FOR DRAWING THINGS.
  ///             ############################
  public static void drawWithBorder(
    string message,
    Color borderColor,
    Color insideColor,
    Vector2 position)
  {
    Game1.drawWithBorder(message, borderColor, insideColor, position, 0.0f, 1f, 1f, false);
  }

  public static void drawWithBorder(
    string message,
    Color borderColor,
    Color insideColor,
    Vector2 position,
    float rotate,
    float scale,
    float layerDepth)
  {
    Game1.drawWithBorder(message, borderColor, insideColor, position, rotate, scale, layerDepth, false);
  }

  public static void drawWithBorder(
    string message,
    Color borderColor,
    Color insideColor,
    Vector2 position,
    float rotate,
    float scale,
    float layerDepth,
    bool tiny)
  {
    string[] strArray = ArgUtility.SplitBySpace(message);
    int num = 0;
    for (int index = 0; index < strArray.Length; ++index)
    {
      if (strArray[index].Contains('='))
      {
        Game1.spriteBatch.DrawString(tiny ? Game1.tinyFont : Game1.dialogueFont, strArray[index], new Vector2(position.X + (float) num, position.Y), Color.Purple, rotate, Vector2.Zero, scale, SpriteEffects.None, layerDepth);
        num += (int) ((double) (tiny ? Game1.tinyFont : Game1.dialogueFont).MeasureString(strArray[index]).X + 8.0);
      }
      else
      {
        Game1.spriteBatch.DrawString(tiny ? Game1.tinyFont : Game1.dialogueFont, strArray[index], new Vector2(position.X + (float) num, position.Y), insideColor, rotate, Vector2.Zero, scale, SpriteEffects.None, layerDepth);
        num += (int) ((double) (tiny ? Game1.tinyFont : Game1.dialogueFont).MeasureString(strArray[index]).X + 8.0);
      }
    }
  }

  public static bool isOutdoorMapSmallerThanViewport()
  {
    if (Game1.uiMode || Game1.currentLocation == null || !Game1.currentLocation.IsOutdoors || Game1.currentLocation is Summit)
      return false;
    return Game1.currentLocation.map.Layers[0].LayerWidth * 64 /*0x40*/ < Game1.viewport.Width || Game1.currentLocation.map.Layers[0].LayerHeight * 64 /*0x40*/ < Game1.viewport.Height;
  }

  protected virtual void drawHUD()
  {
    if (Game1.eventUp || Game1.farmEvent != null)
      return;
    float num1 = 0.625f;
    Vector2 position1 = new Vector2((float) (Game1.graphics.GraphicsDevice.Viewport.GetTitleSafeArea().Right - 48 /*0x30*/ - 8), (float) (Game1.graphics.GraphicsDevice.Viewport.GetTitleSafeArea().Bottom - 224 /*0xE0*/ - 16 /*0x10*/ - (int) ((double) (Game1.player.MaxStamina - 270) * (double) num1)));
    if (Game1.isOutdoorMapSmallerThanViewport())
      position1.X = Math.Min(position1.X, (float) (-Game1.viewport.X + Game1.currentLocation.map.Layers[0].LayerWidth * 64 /*0x40*/ - 48 /*0x30*/));
    if (Game1.staminaShakeTimer > 0)
    {
      position1.X += (float) Game1.random.Next(-3, 4);
      position1.Y += (float) Game1.random.Next(-3, 4);
    }
    Game1.spriteBatch.Draw(Game1.mouseCursors, position1, new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(256 /*0x0100*/, 408, 12, 16 /*0x10*/)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
    Game1.spriteBatch.Draw(Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle((int) position1.X, (int) ((double) position1.Y + 64.0), 48 /*0x30*/, Game1.graphics.GraphicsDevice.Viewport.GetTitleSafeArea().Bottom - 64 /*0x40*/ - 16 /*0x10*/ - (int) ((double) position1.Y + 64.0 - 8.0)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(256 /*0x0100*/, 424, 12, 16 /*0x10*/)), Color.White);
    Game1.spriteBatch.Draw(Game1.mouseCursors, new Vector2(position1.X, (float) ((double) position1.Y + 224.0 + (double) (int) ((double) (Game1.player.MaxStamina - 270) * (double) num1) - 64.0)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(256 /*0x0100*/, 448, 12, 16 /*0x10*/)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
    Microsoft.Xna.Framework.Rectangle destinationRectangle1 = new Microsoft.Xna.Framework.Rectangle((int) position1.X + 12, (int) position1.Y + 16 /*0x10*/ + 32 /*0x20*/ + (int) ((double) Game1.player.MaxStamina * (double) num1) - (int) ((double) Math.Max(0.0f, Game1.player.Stamina) * (double) num1), 24, (int) ((double) Game1.player.Stamina * (double) num1) - 1);
    int num2;
    if ((double) Game1.getOldMouseX() >= (double) position1.X && (double) Game1.getOldMouseY() >= (double) position1.Y)
    {
      num2 = (int) Math.Max(0.0f, Game1.player.Stamina);
      string str1 = num2.ToString();
      num2 = Game1.player.MaxStamina;
      string str2 = num2.ToString();
      Game1.drawWithBorder($"{str1}/{str2}", Color.Black * 0.0f, Color.White, position1 + new Vector2((float) (-(double) Game1.dialogueFont.MeasureString("999/999").X - 16.0 - (Game1.showingHealth ? 64.0 : 0.0)), 64f));
    }
    Color toGreenLerpColor1 = Utility.getRedToGreenLerpColor(Game1.player.stamina / (float) Game1.player.maxStamina.Value);
    Game1.spriteBatch.Draw(Game1.staminaRect, destinationRectangle1, toGreenLerpColor1);
    destinationRectangle1.Height = 4;
    toGreenLerpColor1.R = (byte) Math.Max(0, (int) toGreenLerpColor1.R - 50);
    toGreenLerpColor1.G = (byte) Math.Max(0, (int) toGreenLerpColor1.G - 50);
    Game1.spriteBatch.Draw(Game1.staminaRect, destinationRectangle1, toGreenLerpColor1);
    if (Game1.player.exhausted.Value)
    {
      Game1.spriteBatch.Draw(Game1.mouseCursors, position1 - new Vector2(0.0f, 11f) * 4f, new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(191, 406, 12, 11)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
      if ((double) Game1.getOldMouseX() >= (double) position1.X && (double) Game1.getOldMouseY() >= (double) position1.Y - 44.0)
        Game1.drawWithBorder(Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3747"), Color.Black * 0.0f, Color.White, position1 + new Vector2((float) (-(double) Game1.dialogueFont.MeasureString(Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3747")).X - 16.0 - (Game1.showingHealth ? 64.0 : 0.0)), 96f));
    }
    switch (Game1.currentLocation)
    {
      case MineShaft _:
      case Woods _:
      case SlimeHutch _:
      case VolcanoDungeon _:
label_12:
        Game1.showingHealthBar = true;
        Game1.showingHealth = true;
        int num3 = 168 + (Game1.player.maxHealth - 100);
        int height = (int) ((double) Game1.player.health / (double) Game1.player.maxHealth * (double) num3);
        position1.X -= (float) (56 + (Game1.hitShakeTimer > 0 ? Game1.random.Next(-3, 4) : 0));
        position1.Y = (float) (Game1.graphics.GraphicsDevice.Viewport.GetTitleSafeArea().Bottom - 224 /*0xE0*/ - 16 /*0x10*/ - (Game1.player.maxHealth - 100));
        Game1.spriteBatch.Draw(Game1.mouseCursors, position1, new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(268, 408, 12, 16 /*0x10*/)), Game1.player.health < 20 ? Color.Pink * (float) (Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / ((double) Game1.player.health * 50.0)) / 4.0 + 0.89999997615814209) : Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
        Game1.spriteBatch.Draw(Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle((int) position1.X, (int) ((double) position1.Y + 64.0), 48 /*0x30*/, Game1.graphics.GraphicsDevice.Viewport.GetTitleSafeArea().Bottom - 64 /*0x40*/ - 16 /*0x10*/ - (int) ((double) position1.Y + 64.0)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(268, 424, 12, 16 /*0x10*/)), Game1.player.health < 20 ? Color.Pink * (float) (Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / ((double) Game1.player.health * 50.0)) / 4.0 + 0.89999997615814209) : Color.White);
        Game1.spriteBatch.Draw(Game1.mouseCursors, new Vector2(position1.X, (float) ((double) position1.Y + 224.0 + (double) (Game1.player.maxHealth - 100) - 64.0)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(268, 448, 12, 16 /*0x10*/)), Game1.player.health < 20 ? Color.Pink * (float) (Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / ((double) Game1.player.health * 50.0)) / 4.0 + 0.89999997615814209) : Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
        Microsoft.Xna.Framework.Rectangle destinationRectangle2 = new Microsoft.Xna.Framework.Rectangle((int) position1.X + 12, (int) position1.Y + 16 /*0x10*/ + 32 /*0x20*/ + num3 - height, 24, height);
        Color toGreenLerpColor2 = Utility.getRedToGreenLerpColor((float) Game1.player.health / (float) Game1.player.maxHealth);
        Game1.spriteBatch.Draw(Game1.staminaRect, destinationRectangle2, new Microsoft.Xna.Framework.Rectangle?(Game1.staminaRect.Bounds), toGreenLerpColor2, 0.0f, Vector2.Zero, SpriteEffects.None, 1f);
        toGreenLerpColor2.R = (byte) Math.Max(0, (int) toGreenLerpColor2.R - 50);
        toGreenLerpColor2.G = (byte) Math.Max(0, (int) toGreenLerpColor2.G - 50);
        if ((double) Game1.getOldMouseX() >= (double) position1.X && (double) Game1.getOldMouseY() >= (double) position1.Y && (double) Game1.getOldMouseX() < (double) position1.X + 32.0)
        {
          num2 = Math.Max(0, Game1.player.health);
          Game1.drawWithBorder($"{num2.ToString()}/{Game1.player.maxHealth.ToString()}", Color.Black * 0.0f, Color.Red, position1 + new Vector2((float) (-(double) Game1.dialogueFont.MeasureString("999/999").X - 32.0), 64f));
        }
        destinationRectangle2.Height = 4;
        Game1.spriteBatch.Draw(Game1.staminaRect, destinationRectangle2, new Microsoft.Xna.Framework.Rectangle?(Game1.staminaRect.Bounds), toGreenLerpColor2, 0.0f, Vector2.Zero, SpriteEffects.None, 1f);
        break;
      default:
        if (Game1.player.health >= Game1.player.maxHealth)
        {
          Game1.showingHealth = false;
          break;
        }
        goto label_12;
    }
    foreach (IClickableMenu onScreenMenu in (IEnumerable<IClickableMenu>) Game1.onScreenMenus)
    {
      if (onScreenMenu != Game1.chatBox)
      {
        onScreenMenu.update(Game1.currentGameTime);
        onScreenMenu.draw(Game1.spriteBatch);
      }
    }
    if (!Game1.player.professions.Contains(17) || !Game1.currentLocation.IsOutdoors)
      return;
    foreach (KeyValuePair<Vector2, Object> pair in Game1.currentLocation.objects.Pairs)
    {
      if ((pair.Value.isSpawnedObject.Value || pair.Value.QualifiedItemId == "(O)590") && !Utility.isOnScreen(pair.Key * 64f + new Vector2(32f, 32f), 64 /*0x40*/))
      {
        Microsoft.Xna.Framework.Rectangle bounds = Game1.graphics.GraphicsDevice.Viewport.Bounds;
        Vector2 renderPos = new Vector2();
        float rotation = 0.0f;
        if ((double) pair.Key.X * 64.0 > (double) (Game1.viewport.MaxCorner.X - 64 /*0x40*/))
        {
          renderPos.X = (float) (bounds.Right - 8);
          rotation = 1.57079637f;
        }
        else if ((double) pair.Key.X * 64.0 < (double) Game1.viewport.X)
        {
          renderPos.X = 8f;
          rotation = -1.57079637f;
        }
        else
          renderPos.X = pair.Key.X * 64f - (float) Game1.viewport.X;
        if ((double) pair.Key.Y * 64.0 > (double) (Game1.viewport.MaxCorner.Y - 64 /*0x40*/))
        {
          renderPos.Y = (float) (bounds.Bottom - 8);
          rotation = 3.14159274f;
        }
        else
          renderPos.Y = (double) pair.Key.Y * 64.0 >= (double) Game1.viewport.Y ? pair.Key.Y * 64f - (float) Game1.viewport.Y : 8f;
        if ((double) renderPos.X == 8.0 && (double) renderPos.Y == 8.0)
          rotation += 0.7853982f;
        if ((double) renderPos.X == 8.0 && (double) renderPos.Y == (double) (bounds.Bottom - 8))
          rotation += 0.7853982f;
        if ((double) renderPos.X == (double) (bounds.Right - 8) && (double) renderPos.Y == 8.0)
          rotation -= 0.7853982f;
        if ((double) renderPos.X == (double) (bounds.Right - 8) && (double) renderPos.Y == (double) (bounds.Bottom - 8))
          rotation -= 0.7853982f;
        Microsoft.Xna.Framework.Rectangle rectangle = new Microsoft.Xna.Framework.Rectangle(412, 495, 5, 4);
        float scale = 4f;
        Vector2 renderSize = new Vector2((float) rectangle.Width * scale, (float) rectangle.Height * scale);
        Vector2 position2 = Utility.makeSafe(renderPos, renderSize);
        Game1.spriteBatch.Draw(Game1.mouseCursors, position2, new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.White, rotation, new Vector2(2f, 2f), scale, SpriteEffects.None, 1f);
      }
    }
    if (Game1.currentLocation.orePanPoint.Equals((object) Point.Zero) || Utility.isOnScreen(Utility.PointToVector2(Game1.currentLocation.orePanPoint.Value) * 64f + new Vector2(32f, 32f), 64 /*0x40*/))
      return;
    Vector2 position3 = new Vector2();
    float rotation1 = 0.0f;
    Viewport viewport;
    if (Game1.currentLocation.orePanPoint.X * 64 /*0x40*/ > Game1.viewport.MaxCorner.X - 64 /*0x40*/)
    {
      ref Vector2 local = ref position3;
      viewport = Game1.graphics.GraphicsDevice.Viewport;
      double num4 = (double) (viewport.Bounds.Right - 8);
      local.X = (float) num4;
      rotation1 = 1.57079637f;
    }
    else if (Game1.currentLocation.orePanPoint.X * 64 /*0x40*/ < Game1.viewport.X)
    {
      position3.X = 8f;
      rotation1 = -1.57079637f;
    }
    else
      position3.X = (float) (Game1.currentLocation.orePanPoint.X * 64 /*0x40*/ - Game1.viewport.X);
    if (Game1.currentLocation.orePanPoint.Y * 64 /*0x40*/ > Game1.viewport.MaxCorner.Y - 64 /*0x40*/)
    {
      ref Vector2 local = ref position3;
      viewport = Game1.graphics.GraphicsDevice.Viewport;
      double num5 = (double) (viewport.Bounds.Bottom - 8);
      local.Y = (float) num5;
      rotation1 = 3.14159274f;
    }
    else
      position3.Y = Game1.currentLocation.orePanPoint.Y * 64 /*0x40*/ >= Game1.viewport.Y ? (float) (Game1.currentLocation.orePanPoint.Y * 64 /*0x40*/ - Game1.viewport.Y) : 8f;
    if ((double) position3.X == 8.0 && (double) position3.Y == 8.0)
      rotation1 += 0.7853982f;
    if ((double) position3.X == 8.0)
    {
      double y = (double) position3.Y;
      viewport = Game1.graphics.GraphicsDevice.Viewport;
      double num6 = (double) (viewport.Bounds.Bottom - 8);
      if (y == num6)
        rotation1 += 0.7853982f;
    }
    double x1 = (double) position3.X;
    viewport = Game1.graphics.GraphicsDevice.Viewport;
    double num7 = (double) (viewport.Bounds.Right - 8);
    if (x1 == num7 && (double) position3.Y == 8.0)
      rotation1 -= 0.7853982f;
    double x2 = (double) position3.X;
    viewport = Game1.graphics.GraphicsDevice.Viewport;
    double num8 = (double) (viewport.Bounds.Right - 8);
    if (x2 == num8)
    {
      double y = (double) position3.Y;
      viewport = Game1.graphics.GraphicsDevice.Viewport;
      double num9 = (double) (viewport.Bounds.Bottom - 8);
      if (y == num9)
        rotation1 -= 0.7853982f;
    }
    Game1.spriteBatch.Draw(Game1.mouseCursors, position3, new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(412, 495, 5, 4)), Color.Cyan, rotation1, new Vector2(2f, 2f), 4f, SpriteEffects.None, 1f);
  }

  public static void InvalidateOldMouseMovement()
  {
    MouseState mouseState = Game1.input.GetMouseState();
    Game1.oldMouseState = new MouseState(mouseState.X, mouseState.Y, Game1.oldMouseState.ScrollWheelValue, Game1.oldMouseState.LeftButton, Game1.oldMouseState.MiddleButton, Game1.oldMouseState.RightButton, Game1.oldMouseState.XButton1, Game1.oldMouseState.XButton2);
  }

  public static bool IsRenderingNonNativeUIScale()
  {
    return (double) Game1.options.uiScale != (double) Game1.options.zoomLevel;
  }

  public virtual void drawMouseCursor()
  {
    if (Game1.activeClickableMenu == null && Game1.timerUntilMouseFade > 0)
    {
      Game1.timerUntilMouseFade -= Game1.currentGameTime.ElapsedGameTime.Milliseconds;
      Game1.lastMousePositionBeforeFade = Game1.getMousePosition();
    }
    if (Game1.options.gamepadControls && Game1.timerUntilMouseFade <= 0 && Game1.activeClickableMenu == null && (Game1.emoteMenu == null || Game1.emoteMenu.gamepadMode))
      Game1.mouseCursorTransparency = 0.0f;
    if (Game1.activeClickableMenu == null && Game1.mouseCursor > Game1.cursor_none && Game1.currentLocation != null)
    {
      if (Game1.IsRenderingNonNativeUIScale())
      {
        Game1.spriteBatch.End();
        Game1.PopUIMode();
        if (this.ShouldDrawOnBuffer())
          Game1.SetRenderTarget(this.screen);
        else
          Game1.SetRenderTarget((RenderTarget2D) null);
        Game1.spriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
      }
      if ((double) Game1.mouseCursorTransparency <= 0.0 || !Utility.canGrabSomethingFromHere(Game1.getOldMouseX() + Game1.viewport.X, Game1.getOldMouseY() + Game1.viewport.Y, Game1.player) || Game1.mouseCursor == Game1.cursor_gift)
      {
        if (Game1.player.ActiveObject != null && Game1.mouseCursor != Game1.cursor_gift && !Game1.eventUp && Game1.currentMinigame == null && !Game1.player.isRidingHorse() && Game1.player.CanMove && Game1.displayFarmer)
        {
          if ((double) Game1.mouseCursorTransparency > 0.0 || Game1.options.showPlacementTileForGamepad)
          {
            Game1.player.ActiveObject.drawPlacementBounds(Game1.spriteBatch, Game1.currentLocation);
            if ((double) Game1.mouseCursorTransparency > 0.0)
            {
              Game1.spriteBatch.End();
              Game1.PushUIMode();
              Game1.spriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
              bool flag = Utility.playerCanPlaceItemHere(Game1.currentLocation, Game1.player.CurrentItem, Game1.getMouseX() + Game1.viewport.X, Game1.getMouseY() + Game1.viewport.Y, Game1.player) || Utility.isThereAnObjectHereWhichAcceptsThisItem(Game1.currentLocation, Game1.player.CurrentItem, Game1.getMouseX() + Game1.viewport.X, Game1.getMouseY() + Game1.viewport.Y) && Utility.withinRadiusOfPlayer(Game1.getMouseX() + Game1.viewport.X, Game1.getMouseY() + Game1.viewport.Y, 1, Game1.player);
              Game1.player.CurrentItem?.drawInMenu(Game1.spriteBatch, new Vector2((float) (Game1.getMouseX() + 16 /*0x10*/), (float) (Game1.getMouseY() + 16 /*0x10*/)), flag ? (float) ((double) Game1.dialogueButtonScale / 75.0 + 1.0) : 1f, flag ? 1f : 0.5f, 0.999f);
              Game1.spriteBatch.End();
              Game1.PopUIMode();
              Game1.spriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
            }
          }
        }
        else if (Game1.mouseCursor == Game1.cursor_default && Game1.isActionAtCurrentCursorTile && Game1.currentMinigame == null)
          Game1.mouseCursor = Game1.isSpeechAtCurrentCursorTile ? Game1.cursor_talk : (Game1.isInspectionAtCurrentCursorTile ? Game1.cursor_look : Game1.cursor_grab);
        else if ((double) Game1.mouseCursorTransparency > 0.0)
        {
          NetLongDictionary<FarmAnimal, NetRef<FarmAnimal>> animals = Game1.currentLocation.animals;
          if (animals != null)
          {
            Vector2 vector2 = new Vector2((float) (Game1.getOldMouseX() + Game1.uiViewport.X), (float) (Game1.getOldMouseY() + Game1.uiViewport.Y));
            bool flag = Utility.withinRadiusOfPlayer((int) vector2.X, (int) vector2.Y, 1, Game1.player);
            foreach (KeyValuePair<long, FarmAnimal> pair in animals.Pairs)
            {
              Microsoft.Xna.Framework.Rectangle cursorPetBoundingBox = pair.Value.GetCursorPetBoundingBox();
              if (!pair.Value.wasPet.Value && cursorPetBoundingBox.Contains((int) vector2.X, (int) vector2.Y))
              {
                Game1.mouseCursor = Game1.cursor_grab;
                if (!flag)
                {
                  Game1.mouseCursorTransparency = 0.5f;
                  break;
                }
                break;
              }
            }
          }
        }
      }
      if (Game1.IsRenderingNonNativeUIScale())
      {
        Game1.spriteBatch.End();
        Game1.PushUIMode();
        Game1.SetRenderTarget(this.uiScreen);
        Game1.spriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
      }
      if (Game1.currentMinigame != null)
        Game1.mouseCursor = Game1.cursor_default;
      if (!Game1.freezeControls && !Game1.options.hardwareCursor)
        Game1.spriteBatch.Draw(Game1.mouseCursors, new Vector2((float) Game1.getMouseX(), (float) Game1.getMouseY()), new Microsoft.Xna.Framework.Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, Game1.mouseCursor, 16 /*0x10*/, 16 /*0x10*/)), Color.White * Game1.mouseCursorTransparency, 0.0f, Vector2.Zero, (float) (4.0 + (double) Game1.dialogueButtonScale / 150.0), SpriteEffects.None, 1f);
      Game1.wasMouseVisibleThisFrame = (double) Game1.mouseCursorTransparency > 0.0;
      this._lastDrewMouseCursor = Game1.wasMouseVisibleThisFrame;
    }
    Game1.mouseCursor = Game1.cursor_default;
    if (Game1.isActionAtCurrentCursorTile || Game1.activeClickableMenu != null)
      return;
    Game1.mouseCursorTransparency = 1f;
  }

  public static float mouseCursorTransparency
  {
    get => Game1._mouseCursorTransparency;
    set => Game1._mouseCursorTransparency = value;
  }

  public static void panScreen(int x, int y)
  {
    int uiModeCount = Game1.uiModeCount;
    while (Game1.uiModeCount > 0)
      Game1.PopUIMode();
    Game1.previousViewportPosition.X = (float) Game1.viewport.Location.X;
    Game1.previousViewportPosition.Y = (float) Game1.viewport.Location.Y;
    Game1.viewport.X += x;
    Game1.viewport.Y += y;
    Game1.clampViewportToGameMap();
    Game1.updateRaindropPosition();
    for (int index = 0; index < uiModeCount; ++index)
      Game1.PushUIMode();
  }

  public static void clampViewportToGameMap()
  {
    if (Game1.viewport.X < 0)
      Game1.viewport.X = 0;
    if (Game1.viewport.X > Game1.currentLocation.map.DisplayWidth - Game1.viewport.Width)
      Game1.viewport.X = Game1.currentLocation.map.DisplayWidth - Game1.viewport.Width;
    if (Game1.viewport.Y < 0)
      Game1.viewport.Y = 0;
    if (Game1.viewport.Y <= Game1.currentLocation.map.DisplayHeight - Game1.viewport.Height)
      return;
    Game1.viewport.Y = Game1.currentLocation.map.DisplayHeight - Game1.viewport.Height;
  }

  protected void drawDialogueBox()
  {
    if (Game1.currentSpeaker == null)
      return;
    int height = Math.Max((int) Game1.dialogueFont.MeasureString(Game1.currentSpeaker.CurrentDialogue.Peek().getCurrentDialogue()).Y, 320);
    Game1.drawDialogueBox((this.GraphicsDevice.Viewport.GetTitleSafeArea().Width - Math.Min(1280 /*0x0500*/, this.GraphicsDevice.Viewport.GetTitleSafeArea().Width - 128 /*0x80*/)) / 2, this.GraphicsDevice.Viewport.GetTitleSafeArea().Height - height, Math.Min(1280 /*0x0500*/, this.GraphicsDevice.Viewport.GetTitleSafeArea().Width - 128 /*0x80*/), height, true, false, objectDialogueWithPortrait: Game1.objectDialoguePortraitPerson != null && Game1.currentSpeaker == null);
  }

  public static void drawDialogueBox(string message)
  {
    Game1.drawDialogueBox(Game1.viewport.Width / 2, Game1.viewport.Height / 2, false, false, message);
  }

  public static void drawDialogueBox(
    int centerX,
    int centerY,
    bool speaker,
    bool drawOnlyBox,
    string message)
  {
    string text = (string) null;
    if (speaker && Game1.currentSpeaker != null)
      text = Game1.currentSpeaker.CurrentDialogue.Peek().getCurrentDialogue();
    else if (message != null)
      text = message;
    else if (Game1.currentObjectDialogue.Count > 0)
      text = Game1.currentObjectDialogue.Peek();
    if (text == null)
      return;
    Vector2 vector2 = Game1.dialogueFont.MeasureString(text);
    int width = (int) vector2.X + 128 /*0x80*/;
    int height = (int) vector2.Y + 128 /*0x80*/;
    Game1.drawDialogueBox(centerX - width / 2, centerY - height / 2, width, height, speaker, drawOnlyBox, message, Game1.objectDialoguePortraitPerson != null && !speaker);
  }

  public static void DrawBox(int x, int y, int width, int height, Color? color = null)
  {
    Microsoft.Xna.Framework.Rectangle rectangle = new Microsoft.Xna.Framework.Rectangle(0, 0, 64 /*0x40*/, 64 /*0x40*/);
    rectangle.X = 64 /*0x40*/;
    rectangle.Y = 128 /*0x80*/;
    Texture2D texture = Game1.menuTexture;
    Color white = Color.White;
    Color color1 = Color.White;
    if (color.HasValue)
    {
      white = color.Value;
      texture = Game1.uncoloredMenuTexture;
      color1 = new Color((int) Utility.Lerp((float) white.R, (float) Math.Min((int) byte.MaxValue, (int) white.R + 150), 0.65f), (int) Utility.Lerp((float) white.G, (float) Math.Min((int) byte.MaxValue, (int) white.G + 150), 0.65f), (int) Utility.Lerp((float) white.B, (float) Math.Min((int) byte.MaxValue, (int) white.B + 150), 0.65f));
    }
    Game1.spriteBatch.Draw(texture, new Microsoft.Xna.Framework.Rectangle(x, y, width, height), new Microsoft.Xna.Framework.Rectangle?(rectangle), color1);
    rectangle.Y = 0;
    Vector2 vector2 = new Vector2((float) -rectangle.Width * 0.5f, (float) -rectangle.Height * 0.5f);
    rectangle.X = 0;
    Game1.spriteBatch.Draw(texture, new Vector2((float) x + vector2.X, (float) y + vector2.Y), new Microsoft.Xna.Framework.Rectangle?(rectangle), white);
    rectangle.X = 192 /*0xC0*/;
    Game1.spriteBatch.Draw(texture, new Vector2((float) x + vector2.X + (float) width, (float) y + vector2.Y), new Microsoft.Xna.Framework.Rectangle?(rectangle), white);
    rectangle.Y = 192 /*0xC0*/;
    Game1.spriteBatch.Draw(texture, new Vector2((float) (x + width) + vector2.X, (float) (y + height) + vector2.Y), new Microsoft.Xna.Framework.Rectangle?(rectangle), white);
    rectangle.X = 0;
    Game1.spriteBatch.Draw(texture, new Vector2((float) x + vector2.X, (float) (y + height) + vector2.Y), new Microsoft.Xna.Framework.Rectangle?(rectangle), white);
    rectangle.X = 128 /*0x80*/;
    rectangle.Y = 0;
    Game1.spriteBatch.Draw(texture, new Microsoft.Xna.Framework.Rectangle(64 /*0x40*/ + x + (int) vector2.X, y + (int) vector2.Y, width - 64 /*0x40*/, 64 /*0x40*/), new Microsoft.Xna.Framework.Rectangle?(rectangle), white);
    rectangle.Y = 192 /*0xC0*/;
    Game1.spriteBatch.Draw(texture, new Microsoft.Xna.Framework.Rectangle(64 /*0x40*/ + x + (int) vector2.X, y + (int) vector2.Y + height, width - 64 /*0x40*/, 64 /*0x40*/), new Microsoft.Xna.Framework.Rectangle?(rectangle), white);
    rectangle.Y = 128 /*0x80*/;
    rectangle.X = 0;
    Game1.spriteBatch.Draw(texture, new Microsoft.Xna.Framework.Rectangle(x + (int) vector2.X, y + (int) vector2.Y + 64 /*0x40*/, 64 /*0x40*/, height - 64 /*0x40*/), new Microsoft.Xna.Framework.Rectangle?(rectangle), white);
    rectangle.X = 192 /*0xC0*/;
    Game1.spriteBatch.Draw(texture, new Microsoft.Xna.Framework.Rectangle(x + width + (int) vector2.X, y + (int) vector2.Y + 64 /*0x40*/, 64 /*0x40*/, height - 64 /*0x40*/), new Microsoft.Xna.Framework.Rectangle?(rectangle), white);
  }

  public static void drawDialogueBox(
    int x,
    int y,
    int width,
    int height,
    bool speaker,
    bool drawOnlyBox,
    string message = null,
    bool objectDialogueWithPortrait = false,
    bool ignoreTitleSafe = true,
    int r = -1,
    int g = -1,
    int b = -1)
  {
    if (!drawOnlyBox)
      return;
    Microsoft.Xna.Framework.Rectangle titleSafeArea = Game1.graphics.GraphicsDevice.Viewport.GetTitleSafeArea();
    int height1 = titleSafeArea.Height;
    int width1 = titleSafeArea.Width;
    int num1 = 0;
    int num2 = 0;
    if (!ignoreTitleSafe)
      num2 = y > titleSafeArea.Y ? 0 : titleSafeArea.Y - y;
    int num3 = 0;
    width = Math.Min(titleSafeArea.Width, width);
    if (!Game1.isQuestion && Game1.currentSpeaker == null && Game1.currentObjectDialogue.Count > 0 && !drawOnlyBox)
    {
      width = (int) Game1.dialogueFont.MeasureString(Game1.currentObjectDialogue.Peek()).X + 128 /*0x80*/;
      height = (int) Game1.dialogueFont.MeasureString(Game1.currentObjectDialogue.Peek()).Y + 64 /*0x40*/;
      x = width1 / 2 - width / 2;
      num3 = height > 256 /*0x0100*/ ? -(height - 256 /*0x0100*/) : 0;
    }
    Microsoft.Xna.Framework.Rectangle rectangle1 = new Microsoft.Xna.Framework.Rectangle(0, 0, 64 /*0x40*/, 64 /*0x40*/);
    int num4 = -1;
    if (Game1.questionChoices.Count >= 3)
      num4 = Game1.questionChoices.Count - 3;
    if (!drawOnlyBox && Game1.currentObjectDialogue.Count > 0)
    {
      if ((double) Game1.dialogueFont.MeasureString(Game1.currentObjectDialogue.Peek()).Y >= (double) (height - 128 /*0x80*/))
      {
        num4 -= (int) (((double) (height - 128 /*0x80*/) - (double) Game1.dialogueFont.MeasureString(Game1.currentObjectDialogue.Peek()).Y) / 64.0) - 1;
      }
      else
      {
        height += (int) Game1.dialogueFont.MeasureString(Game1.currentObjectDialogue.Peek()).Y / 2;
        num3 -= (int) Game1.dialogueFont.MeasureString(Game1.currentObjectDialogue.Peek()).Y / 2;
        if ((int) Game1.dialogueFont.MeasureString(Game1.currentObjectDialogue.Peek()).Y / 2 > 64 /*0x40*/)
          num4 = 0;
      }
    }
    if (Game1.currentSpeaker != null && Game1.isQuestion && Game1.currentSpeaker.CurrentDialogue.Peek().getCurrentDialogue().Substring(0, Game1.currentDialogueCharacterIndex).Contains(Environment.NewLine))
      ++num4;
    rectangle1.Width = 64 /*0x40*/;
    rectangle1.Height = 64 /*0x40*/;
    rectangle1.X = 64 /*0x40*/;
    rectangle1.Y = 128 /*0x80*/;
    Color color = r == -1 ? Color.White : new Color(r, g, b);
    Texture2D texture = r == -1 ? Game1.menuTexture : Game1.uncoloredMenuTexture;
    Game1.spriteBatch.Draw(texture, new Microsoft.Xna.Framework.Rectangle(28 + x + num1, 28 + y - 64 /*0x40*/ * num4 + num2 + num3, width - 64 /*0x40*/, height - 64 /*0x40*/ + num4 * 64 /*0x40*/), new Microsoft.Xna.Framework.Rectangle?(rectangle1), r == -1 ? color : new Color((int) Utility.Lerp((float) r, (float) Math.Min((int) byte.MaxValue, r + 150), 0.65f), (int) Utility.Lerp((float) g, (float) Math.Min((int) byte.MaxValue, g + 150), 0.65f), (int) Utility.Lerp((float) b, (float) Math.Min((int) byte.MaxValue, b + 150), 0.65f)));
    rectangle1.Y = 0;
    rectangle1.X = 0;
    Game1.spriteBatch.Draw(texture, new Vector2((float) (x + num1), (float) (y - 64 /*0x40*/ * num4 + num2 + num3)), new Microsoft.Xna.Framework.Rectangle?(rectangle1), color);
    rectangle1.X = 192 /*0xC0*/;
    Game1.spriteBatch.Draw(texture, new Vector2((float) (x + width + num1 - 64 /*0x40*/), (float) (y - 64 /*0x40*/ * num4 + num2 + num3)), new Microsoft.Xna.Framework.Rectangle?(rectangle1), color);
    rectangle1.Y = 192 /*0xC0*/;
    Game1.spriteBatch.Draw(texture, new Vector2((float) (x + width + num1 - 64 /*0x40*/), (float) (y + height + num2 - 64 /*0x40*/ + num3)), new Microsoft.Xna.Framework.Rectangle?(rectangle1), color);
    rectangle1.X = 0;
    Game1.spriteBatch.Draw(texture, new Vector2((float) (x + num1), (float) (y + height + num2 - 64 /*0x40*/ + num3)), new Microsoft.Xna.Framework.Rectangle?(rectangle1), color);
    rectangle1.X = 128 /*0x80*/;
    rectangle1.Y = 0;
    Game1.spriteBatch.Draw(texture, new Microsoft.Xna.Framework.Rectangle(64 /*0x40*/ + x + num1, y - 64 /*0x40*/ * num4 + num2 + num3, width - 128 /*0x80*/, 64 /*0x40*/), new Microsoft.Xna.Framework.Rectangle?(rectangle1), color);
    rectangle1.Y = 192 /*0xC0*/;
    Game1.spriteBatch.Draw(texture, new Microsoft.Xna.Framework.Rectangle(64 /*0x40*/ + x + num1, y + height + num2 - 64 /*0x40*/ + num3, width - 128 /*0x80*/, 64 /*0x40*/), new Microsoft.Xna.Framework.Rectangle?(rectangle1), color);
    rectangle1.Y = 128 /*0x80*/;
    rectangle1.X = 0;
    Game1.spriteBatch.Draw(texture, new Microsoft.Xna.Framework.Rectangle(x + num1, y - 64 /*0x40*/ * num4 + num2 + 64 /*0x40*/ + num3, 64 /*0x40*/, height - 128 /*0x80*/ + num4 * 64 /*0x40*/), new Microsoft.Xna.Framework.Rectangle?(rectangle1), color);
    rectangle1.X = 192 /*0xC0*/;
    Game1.spriteBatch.Draw(texture, new Microsoft.Xna.Framework.Rectangle(x + width + num1 - 64 /*0x40*/, y - 64 /*0x40*/ * num4 + num2 + 64 /*0x40*/ + num3, 64 /*0x40*/, height - 128 /*0x80*/ + num4 * 64 /*0x40*/), new Microsoft.Xna.Framework.Rectangle?(rectangle1), color);
    if (objectDialogueWithPortrait && Game1.objectDialoguePortraitPerson != null || speaker && Game1.currentSpeaker != null && Game1.currentSpeaker.CurrentDialogue.Count > 0 && Game1.currentSpeaker.CurrentDialogue.Peek().showPortrait)
    {
      NPC npc = objectDialogueWithPortrait ? Game1.objectDialoguePortraitPerson : Game1.currentSpeaker;
      string str = objectDialogueWithPortrait ? (Game1.objectDialoguePortraitPerson.Name == Game1.player.spouse ? "$l" : "$neutral") : npc.CurrentDialogue.Peek().CurrentEmotion;
      Microsoft.Xna.Framework.Rectangle rectangle2;
      if (str != null)
      {
        switch (str.Length)
        {
          case 2:
            switch (str[1])
            {
              case 'a':
                if (str == "$a")
                {
                  rectangle2 = new Microsoft.Xna.Framework.Rectangle(64 /*0x40*/, 128 /*0x80*/, 64 /*0x40*/, 64 /*0x40*/);
                  goto label_35;
                }
                goto label_34;
              case 'h':
                if (str == "$h")
                {
                  rectangle2 = new Microsoft.Xna.Framework.Rectangle(64 /*0x40*/, 0, 64 /*0x40*/, 64 /*0x40*/);
                  goto label_35;
                }
                goto label_34;
              case 'k':
                if (str == "$k")
                  break;
                goto label_34;
              case 'l':
                if (str == "$l")
                {
                  rectangle2 = new Microsoft.Xna.Framework.Rectangle(0, 128 /*0x80*/, 64 /*0x40*/, 64 /*0x40*/);
                  goto label_35;
                }
                goto label_34;
              case 's':
                if (str == "$s")
                {
                  rectangle2 = new Microsoft.Xna.Framework.Rectangle(0, 64 /*0x40*/, 64 /*0x40*/, 64 /*0x40*/);
                  goto label_35;
                }
                goto label_34;
              case 'u':
                if (str == "$u")
                {
                  rectangle2 = new Microsoft.Xna.Framework.Rectangle(64 /*0x40*/, 64 /*0x40*/, 64 /*0x40*/, 64 /*0x40*/);
                  goto label_35;
                }
                goto label_34;
              default:
                goto label_34;
            }
            break;
          case 8:
            if (str == "$neutral")
              break;
            goto label_34;
          default:
            goto label_34;
        }
        rectangle2 = new Microsoft.Xna.Framework.Rectangle(0, 0, 64 /*0x40*/, 64 /*0x40*/);
        goto label_35;
      }
label_34:
      rectangle2 = Game1.getSourceRectForStandardTileSheet(npc.Portrait, Convert.ToInt32(npc.CurrentDialogue.Peek().CurrentEmotion.Substring(1)));
label_35:
      Game1.spriteBatch.End();
      Game1.spriteBatch.Begin(blendState: BlendState.NonPremultiplied, samplerState: SamplerState.PointClamp);
      if (npc.Portrait != null)
      {
        Game1.spriteBatch.Draw(Game1.mouseCursors, new Vector2((float) (num1 + x + 768 /*0x0300*/), (float) (height1 - 320 - 64 /*0x40*/ * num4 - 256 /*0x0100*/ + num2 + 16 /*0x10*/ - 60 + num3)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(333, 305, 80 /*0x50*/, 87)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.98f);
        Game1.spriteBatch.Draw(npc.Portrait, new Vector2((float) (num1 + x + 768 /*0x0300*/ + 32 /*0x20*/), (float) (height1 - 320 - 64 /*0x40*/ * num4 - 256 /*0x0100*/ + num2 + 16 /*0x10*/ - 60 + num3)), new Microsoft.Xna.Framework.Rectangle?(rectangle2), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.99f);
      }
      Game1.spriteBatch.End();
      Game1.spriteBatch.Begin();
      if (Game1.isQuestion)
        Game1.spriteBatch.DrawString(Game1.dialogueFont, npc.displayName, new Vector2((float) (928.0 - (double) Game1.dialogueFont.MeasureString(npc.displayName).X / 2.0) + (float) num1 + (float) x, (float) ((double) (height1 - 320 - 64 /*0x40*/ * num4) - (double) Game1.dialogueFont.MeasureString(npc.displayName).Y + (double) num2 + 21.0) + (float) num3) + new Vector2(2f, 2f), new Color(150, 150, 150));
      Game1.spriteBatch.DrawString(Game1.dialogueFont, npc.Name.Equals("Lewis") ? Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3756") : npc.displayName, new Vector2((float) (num1 + x + 896 + 32 /*0x20*/) - Game1.dialogueFont.MeasureString(npc.Name.Equals("Lewis") ? Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3756") : npc.displayName).X / 2f, (float) ((double) (height1 - 320 - 64 /*0x40*/ * num4) - (double) Game1.dialogueFont.MeasureString(npc.Name.Equals("Lewis") ? Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3756") : npc.displayName).Y + (double) num2 + 21.0 + 8.0) + (float) num3), Game1.textColor);
    }
    if (drawOnlyBox)
      return;
    string text = "";
    if (Game1.currentSpeaker != null && Game1.currentSpeaker.CurrentDialogue.Count > 0)
    {
      if (Game1.currentSpeaker.CurrentDialogue.Peek() == null || Game1.currentSpeaker.CurrentDialogue.Peek().getCurrentDialogue().Length < Game1.currentDialogueCharacterIndex - 1)
      {
        Game1.dialogueUp = false;
        Game1.currentDialogueCharacterIndex = 0;
        Game1.playSound("dialogueCharacterClose");
        Game1.player.forceCanMove();
        return;
      }
      text = Game1.currentSpeaker.CurrentDialogue.Peek().getCurrentDialogue().Substring(0, Game1.currentDialogueCharacterIndex);
    }
    else if (message != null)
      text = message;
    else if (Game1.currentObjectDialogue.Count > 0)
      text = Game1.currentObjectDialogue.Peek().Length <= 1 ? "" : Game1.currentObjectDialogue.Peek().Substring(0, Game1.currentDialogueCharacterIndex);
    Vector2 position = (double) Game1.dialogueFont.MeasureString(text).X <= (double) (width1 - 256 /*0x0100*/ - num1) ? (Game1.currentSpeaker == null || Game1.currentSpeaker.CurrentDialogue.Count <= 0 ? (message == null ? (!Game1.isQuestion ? new Vector2((float) (width1 / 2) - Game1.dialogueFont.MeasureString(Game1.currentObjectDialogue.Count == 0 ? "" : Game1.currentObjectDialogue.Peek()).X / 2f + (float) num1, (float) (y + 4 + num3)) : new Vector2((float) (width1 / 2) - Game1.dialogueFont.MeasureString(Game1.currentObjectDialogue.Count == 0 ? "" : Game1.currentObjectDialogue.Peek()).X / 2f + (float) num1, (float) (height1 - 64 /*0x40*/ * num4 - 256 /*0x0100*/ - (16 /*0x10*/ + (Game1.questionChoices.Count - 2) * 64 /*0x40*/) + num2 + num3))) : new Vector2((float) (width1 / 2) - Game1.dialogueFont.MeasureString(text).X / 2f + (float) num1, (float) (y + 96 /*0x60*/ + 4))) : new Vector2((float) (width1 / 2) - Game1.dialogueFont.MeasureString(Game1.currentSpeaker.CurrentDialogue.Peek().getCurrentDialogue()).X / 2f + (float) num1, (float) (height1 - 64 /*0x40*/ * num4 - 256 /*0x0100*/ - 16 /*0x10*/ + num2 + num3))) : new Vector2((float) (128 /*0x80*/ + num1), (float) (height1 - 64 /*0x40*/ * num4 - 256 /*0x0100*/ - 16 /*0x10*/ + num2 + num3));
    if (!drawOnlyBox)
    {
      Game1.spriteBatch.DrawString(Game1.dialogueFont, text, position + new Vector2(3f, 0.0f), Game1.textShadowColor);
      Game1.spriteBatch.DrawString(Game1.dialogueFont, text, position + new Vector2(3f, 3f), Game1.textShadowColor);
      Game1.spriteBatch.DrawString(Game1.dialogueFont, text, position + new Vector2(0.0f, 3f), Game1.textShadowColor);
      Game1.spriteBatch.DrawString(Game1.dialogueFont, text, position, Game1.textColor);
    }
    if ((double) Game1.dialogueFont.MeasureString(text).Y <= 64.0)
      num2 += 64 /*0x40*/;
    if (Game1.isQuestion && !Game1.dialogueTyping)
    {
      for (int index = 0; index < Game1.questionChoices.Count; ++index)
      {
        if (Game1.currentQuestionChoice == index)
        {
          position.X = (float) (80 /*0x50*/ + num1 + x);
          position.Y = (float) ((double) (height1 - (5 + num4 + 1) * 64 /*0x40*/) + (text.Trim().Length > 0 ? (double) Game1.dialogueFont.MeasureString(text).Y : 0.0) + 128.0) + (float) (48 /*0x30*/ * index) - (float) (16 /*0x10*/ + (Game1.questionChoices.Count - 2) * 64 /*0x40*/) + (float) num2 + (float) num3;
          Game1.spriteBatch.End();
          Game1.spriteBatch.Begin(blendState: BlendState.NonPremultiplied, samplerState: SamplerState.PointClamp);
          Game1.spriteBatch.Draw(Game1.objectSpriteSheet, position + new Vector2((float) Math.Cos((double) Game1.currentGameTime.TotalGameTime.Milliseconds * Math.PI / 512.0) * 3f, 0.0f), new Microsoft.Xna.Framework.Rectangle?(GameLocation.getSourceRectForObject(26)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
          Game1.spriteBatch.End();
          Game1.spriteBatch.Begin();
          position.X = (float) (160 /*0xA0*/ + num1 + x);
          position.Y = (float) ((double) (height1 - (5 + num4 + 1) * 64 /*0x40*/) + (text.Trim().Length > 1 ? (double) Game1.dialogueFont.MeasureString(text).Y : 0.0) + 128.0) - (float) ((Game1.questionChoices.Count - 2) * 64 /*0x40*/) + (float) (48 /*0x30*/ * index) + (float) num2 + (float) num3;
          Game1.spriteBatch.DrawString(Game1.dialogueFont, Game1.questionChoices[index].responseText, position, Game1.textColor);
        }
        else
        {
          position.X = (float) (128 /*0x80*/ + num1 + x);
          position.Y = (float) ((double) (height1 - (5 + num4 + 1) * 64 /*0x40*/) + (text.Trim().Length > 1 ? (double) Game1.dialogueFont.MeasureString(text).Y : 0.0) + 128.0) - (float) ((Game1.questionChoices.Count - 2) * 64 /*0x40*/) + (float) (48 /*0x30*/ * index) + (float) num2 + (float) num3;
          Game1.spriteBatch.DrawString(Game1.dialogueFont, Game1.questionChoices[index].responseText, position, Game1.unselectedOptionColor);
        }
      }
    }
    if (drawOnlyBox || Game1.dialogueTyping || message != null)
      return;
    Game1.spriteBatch.Draw(Game1.mouseCursors, new Vector2((float) (x + num1 + width - 96 /*0x60*/), (float) (y + height + num2 + num3 - 96 /*0x60*/) - Game1.dialogueButtonScale), new Microsoft.Xna.Framework.Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, Game1.dialogueButtonShrinking || (double) Game1.dialogueButtonScale >= 8.0 ? 2 : 3)), Color.White, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.9999999f);
  }

  public static void drawPlayerHeldObject(Farmer f)
  {
    if (Game1.eventUp && (Game1.currentLocation.currentEvent == null || !Game1.currentLocation.currentEvent.showActiveObject) || f.FarmerSprite.PauseForSingleAnimation || f.isRidingHorse() || f.bathingClothes.Value || f.onBridge.Value)
      return;
    float x = f.getLocalPosition(Game1.viewport).X + ((double) f.rotation < 0.0 ? -8f : ((double) f.rotation > 0.0 ? 8f : 0.0f)) + (float) (f.FarmerSprite.CurrentAnimationFrame.xOffset * 4);
    float y = f.getLocalPosition(Game1.viewport).Y - 128f + (float) (f.FarmerSprite.CurrentAnimationFrame.positionOffset * 4) + (float) (FarmerRenderer.featureYOffsetPerFrame[f.FarmerSprite.CurrentFrame] * 4);
    if (f.ActiveObject.bigCraftable.Value)
      y -= 64f;
    if (f.isEating)
    {
      x = f.getLocalPosition(Game1.viewport).X - 21f;
      y = (float) ((double) f.getLocalPosition(Game1.viewport).Y - 128.0 + 12.0);
    }
    if (f.isEating && (!f.isEating || f.Sprite.currentFrame > 218))
      return;
    f.ActiveObject.drawWhenHeld(Game1.spriteBatch, new Vector2((float) (int) x, (float) (int) y), f);
  }

  public static void drawTool(Farmer f) => Game1.drawTool(f, f.CurrentTool.CurrentParentTileIndex);

  public static void drawTool(Farmer f, int currentToolIndex)
  {
    Vector2 playerPosition = f.getLocalPosition(Game1.viewport) + f.jitter + f.armOffset;
    FarmerSprite sprite = (FarmerSprite) f.Sprite;
    if (f.CurrentTool is MeleeWeapon currentTool6)
      currentTool6.drawDuringUse(sprite.currentAnimationIndex, f.FacingDirection, Game1.spriteBatch, playerPosition, f);
    else if (f.FarmerSprite.isUsingWeapon())
    {
      MeleeWeapon.drawDuringUse(sprite.currentAnimationIndex, f.FacingDirection, Game1.spriteBatch, playerPosition, f, f.FarmerSprite.CurrentToolIndex.ToString(), f.FarmerSprite.getWeaponTypeFromAnimation(), false);
    }
    else
    {
      switch (f.CurrentTool)
      {
        case Slingshot _:
        case Shears _:
        case MilkPail _:
        case Pan _:
          f.CurrentTool.draw(Game1.spriteBatch);
          break;
        case FishingRod _:
        case WateringCan _:
label_11:
          Texture2D texture = ItemRegistry.GetData(f.CurrentTool?.QualifiedItemId)?.GetTexture() ?? Game1.toolSpriteSheet;
          Microsoft.Xna.Framework.Rectangle rectangle = new Microsoft.Xna.Framework.Rectangle(currentToolIndex * 16 /*0x10*/ % texture.Width, currentToolIndex * 16 /*0x10*/ / texture.Width * 16 /*0x10*/, 16 /*0x10*/, 32 /*0x20*/);
          float drawLayer = f.getDrawLayer();
          if (f.CurrentTool is FishingRod currentTool1)
          {
            if (currentTool1.fishCaught || currentTool1.showingTreasure)
            {
              f.CurrentTool.draw(Game1.spriteBatch);
              break;
            }
            rectangle = new Microsoft.Xna.Framework.Rectangle(sprite.currentAnimationIndex * 48 /*0x30*/, 288, 48 /*0x30*/, 48 /*0x30*/);
            if (f.FacingDirection == 2 || f.FacingDirection == 0)
              rectangle.Y += 48 /*0x30*/;
            else if (currentTool1.isFishing && (!currentTool1.isReeling || currentTool1.hit))
              playerPosition.Y += 8f;
            if (currentTool1.isFishing)
              rectangle.X += (5 - sprite.currentAnimationIndex) * 48 /*0x30*/;
            if (currentTool1.isReeling)
            {
              if (f.FacingDirection == 2 || f.FacingDirection == 0)
              {
                rectangle.X = 288;
                if (f.IsLocalPlayer && Game1.didPlayerJustClickAtAll())
                  rectangle.X = 0;
              }
              else
              {
                rectangle.X = 288;
                rectangle.Y = 240 /*0xF0*/;
                if (f.IsLocalPlayer && Game1.didPlayerJustClickAtAll())
                  rectangle.Y += 48 /*0x30*/;
              }
            }
            if (f.FarmerSprite.CurrentFrame == 57)
              rectangle.Height = 0;
            if (f.FacingDirection == 0)
              playerPosition.X += 16f;
          }
          f.CurrentTool?.draw(Game1.spriteBatch);
          int num1 = 0;
          int num2 = 0;
          if (f.CurrentTool is WateringCan)
          {
            num1 += 80 /*0x50*/;
            num2 = f.FacingDirection == 1 ? 32 /*0x20*/ : (f.FacingDirection == 3 ? -32 : 0);
            if (sprite.currentAnimationIndex == 0 || sprite.currentAnimationIndex == 1)
              num2 = num2 * 3 / 2;
          }
          int num3 = num1 + f.yJumpOffset;
          FarmerRenderer.FarmerSpriteLayers layer;
          switch (f.FacingDirection)
          {
            case 0:
              layer = FarmerRenderer.FarmerSpriteLayers.ToolUp;
              break;
            case 2:
              layer = FarmerRenderer.FarmerSpriteLayers.ToolDown;
              break;
            default:
              layer = FarmerRenderer.FarmerSpriteLayers.TOOL_IN_USE_SIDE;
              break;
          }
          float layerDepth = FarmerRenderer.GetLayerDepth(drawLayer, layer);
          switch (f.FacingDirection)
          {
            case 1:
              if (sprite.currentAnimationIndex > 2)
              {
                Point tilePoint = f.TilePoint;
                ++tilePoint.X;
                --tilePoint.Y;
                if (!(f.CurrentTool is WateringCan) && f.currentLocation.hasTileAt(tilePoint, "Front"))
                  return;
                ++tilePoint.Y;
              }
              Tool currentTool2 = f.CurrentTool;
              if (!(currentTool2 is FishingRod fishingRod1))
              {
                if (currentTool2 is WateringCan)
                {
                  if (sprite.currentAnimationIndex == 1)
                  {
                    Point tilePoint = f.TilePoint;
                    --tilePoint.X;
                    --tilePoint.Y;
                    if (f.currentLocation.hasTileAt(tilePoint, "Front") && (double) f.Position.Y % 64.0 < 32.0)
                      return;
                  }
                  switch (sprite.currentAnimationIndex)
                  {
                    case 0:
                    case 1:
                      Game1.spriteBatch.Draw(texture, new Vector2((float) (int) ((double) playerPosition.X + (double) num2 - 4.0), (float) (int) ((double) playerPosition.Y - 128.0 + 8.0 + (double) num3)), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.White, 0.0f, new Vector2(0.0f, 16f), 4f, SpriteEffects.None, layerDepth);
                      return;
                    case 2:
                      Game1.spriteBatch.Draw(texture, new Vector2((float) ((int) playerPosition.X + num2 + 24), (float) (int) ((double) playerPosition.Y - 128.0 - 8.0 + (double) num3)), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.White, 0.2617994f, new Vector2(0.0f, 16f), 4f, SpriteEffects.None, layerDepth);
                      return;
                    case 3:
                      rectangle.X += 16 /*0x10*/;
                      Game1.spriteBatch.Draw(texture, new Vector2((float) (int) ((double) playerPosition.X + (double) num2 + 8.0), (float) (int) ((double) playerPosition.Y - 128.0 - 24.0 + (double) num3)), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.White, 0.0f, new Vector2(0.0f, 16f), 4f, SpriteEffects.None, layerDepth);
                      return;
                    default:
                      return;
                  }
                }
                else
                {
                  switch (sprite.currentAnimationIndex)
                  {
                    case 0:
                      Game1.spriteBatch.Draw(texture, Utility.snapToInt(new Vector2((float) ((double) playerPosition.X - 32.0 - 4.0) + (float) num2 - (float) Math.Min(8, f.toolPower.Value * 4), (float) ((double) playerPosition.Y - 128.0 + 24.0) + (float) num3 + (float) Math.Min(8, f.toolPower.Value * 4))), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.White, (float) (-0.2617993950843811 - (double) Math.Min(f.toolPower.Value, 2) * 0.049087386578321457), new Vector2(0.0f, 16f), 4f, SpriteEffects.None, layerDepth);
                      return;
                    case 1:
                      Game1.spriteBatch.Draw(texture, Utility.snapToInt(new Vector2((float) ((double) playerPosition.X + 32.0 - 24.0) + (float) num2, (float) ((double) playerPosition.Y - 124.0 + (double) num3 + 64.0))), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.White, 0.2617994f, new Vector2(0.0f, 32f), 4f, SpriteEffects.None, layerDepth);
                      return;
                    case 2:
                      Game1.spriteBatch.Draw(texture, Utility.snapToInt(new Vector2((float) ((double) playerPosition.X + 32.0 + (double) num2 - 4.0), (float) ((double) playerPosition.Y - 132.0 + (double) num3 + 64.0))), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.White, 0.7853982f, new Vector2(0.0f, 32f), 4f, SpriteEffects.None, layerDepth);
                      return;
                    case 3:
                      Game1.spriteBatch.Draw(texture, Utility.snapToInt(new Vector2((float) ((double) playerPosition.X + 32.0 + 28.0) + (float) num2, playerPosition.Y - 64f + (float) num3)), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.White, 1.83259583f, new Vector2(0.0f, 32f), 4f, SpriteEffects.None, layerDepth);
                      return;
                    case 4:
                      Game1.spriteBatch.Draw(texture, Utility.snapToInt(new Vector2((float) ((double) playerPosition.X + 32.0 + 28.0) + (float) num2, (float) ((double) playerPosition.Y - 64.0 + 4.0) + (float) num3)), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.White, 1.83259583f, new Vector2(0.0f, 32f), 4f, SpriteEffects.None, layerDepth);
                      return;
                    case 5:
                      Game1.spriteBatch.Draw(texture, Utility.snapToInt(new Vector2((float) ((double) playerPosition.X + 64.0 + 12.0) + (float) num2, (float) ((double) playerPosition.Y - 128.0 + 32.0 + (double) num3 + 128.0))), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.White, 0.7853982f, new Vector2(0.0f, 32f), 4f, SpriteEffects.None, layerDepth);
                      return;
                    case 6:
                      Game1.spriteBatch.Draw(texture, Utility.snapToInt(new Vector2((float) ((double) playerPosition.X + 42.0 + 8.0) + (float) num2, (float) ((double) playerPosition.Y - 64.0 + 24.0 + (double) num3 + 128.0))), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.White, 0.0f, new Vector2(0.0f, 128f), 4f, SpriteEffects.None, layerDepth);
                      return;
                    default:
                      return;
                  }
                }
              }
              else
              {
                Color color = fishingRod1.getColor();
                switch (sprite.currentAnimationIndex)
                {
                  case 0:
                    if (!fishingRod1.isReeling && !fishingRod1.isFishing && !fishingRod1.doneWithAnimation && fishingRod1.hasDoneFucntionYet && !fishingRod1.pullingOutOfWater)
                      return;
                    Game1.spriteBatch.Draw(texture, new Vector2(playerPosition.X - 64f + (float) num2, playerPosition.Y - 160f + (float) num3), new Microsoft.Xna.Framework.Rectangle?(rectangle), color, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth);
                    return;
                  case 1:
                    Game1.spriteBatch.Draw(texture, new Vector2(playerPosition.X - 64f + (float) num2, (float) ((double) playerPosition.Y - 160.0 + 8.0) + (float) num3), new Microsoft.Xna.Framework.Rectangle?(rectangle), color, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth);
                    return;
                  case 2:
                    Game1.spriteBatch.Draw(texture, new Vector2((float) ((double) playerPosition.X - 96.0 + 32.0) + (float) num2, (float) ((double) playerPosition.Y - 128.0 - 24.0) + (float) num3), new Microsoft.Xna.Framework.Rectangle?(rectangle), color, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth);
                    return;
                  case 3:
                    Game1.spriteBatch.Draw(texture, new Vector2((float) ((double) playerPosition.X - 96.0 + 24.0) + (float) num2, (float) ((double) playerPosition.Y - 128.0 - 32.0) + (float) num3), new Microsoft.Xna.Framework.Rectangle?(rectangle), color, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth);
                    return;
                  case 4:
                    if (fishingRod1.isFishing || fishingRod1.doneWithAnimation)
                    {
                      Game1.spriteBatch.Draw(texture, new Vector2(playerPosition.X - 64f + (float) num2, playerPosition.Y - 160f + (float) num3), new Microsoft.Xna.Framework.Rectangle?(rectangle), color, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth);
                      return;
                    }
                    Game1.spriteBatch.Draw(texture, new Vector2(playerPosition.X - 64f + (float) num2, (float) ((double) playerPosition.Y - 160.0 + 4.0) + (float) num3), new Microsoft.Xna.Framework.Rectangle?(rectangle), color, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth);
                    return;
                  case 5:
                    Game1.spriteBatch.Draw(texture, new Vector2(playerPosition.X - 64f + (float) num2, playerPosition.Y - 160f + (float) num3), new Microsoft.Xna.Framework.Rectangle?(rectangle), color, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth);
                    return;
                  default:
                    return;
                }
              }
            case 3:
              if (sprite.currentAnimationIndex > 2)
              {
                Point tilePoint = f.TilePoint;
                --tilePoint.X;
                --tilePoint.Y;
                if (!(f.CurrentTool is WateringCan) && f.currentLocation.hasTileAt(tilePoint, "Front") && (double) f.Position.Y % 64.0 < 32.0)
                  return;
                ++tilePoint.Y;
              }
              Tool currentTool3 = f.CurrentTool;
              if (!(currentTool3 is FishingRod fishingRod2))
              {
                if (currentTool3 is WateringCan)
                {
                  if (sprite.currentAnimationIndex == 1)
                  {
                    Point tilePoint = f.TilePoint;
                    --tilePoint.X;
                    --tilePoint.Y;
                    if (f.currentLocation.hasTileAt(tilePoint, "Front") && (double) f.Position.Y % 64.0 < 32.0)
                      return;
                  }
                  switch (sprite.currentAnimationIndex)
                  {
                    case 0:
                    case 1:
                      Game1.spriteBatch.Draw(texture, Utility.snapToInt(new Vector2((float) ((double) playerPosition.X + (double) num2 - 4.0), (float) ((double) playerPosition.Y - 128.0 + 8.0) + (float) num3)), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.White, 0.0f, new Vector2(0.0f, 16f), 4f, SpriteEffects.FlipHorizontally, layerDepth);
                      return;
                    case 2:
                      Game1.spriteBatch.Draw(texture, Utility.snapToInt(new Vector2((float) ((double) playerPosition.X + (double) num2 - 16.0), playerPosition.Y - 128f + (float) num3)), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.White, -0.2617994f, new Vector2(0.0f, 16f), 4f, SpriteEffects.FlipHorizontally, layerDepth);
                      return;
                    case 3:
                      rectangle.X += 16 /*0x10*/;
                      Game1.spriteBatch.Draw(texture, Utility.snapToInt(new Vector2((float) ((double) playerPosition.X + (double) num2 - 16.0), (float) ((double) playerPosition.Y - 128.0 - 24.0) + (float) num3)), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.White, 0.0f, new Vector2(0.0f, 16f), 4f, SpriteEffects.FlipHorizontally, layerDepth);
                      return;
                    default:
                      return;
                  }
                }
                else
                {
                  switch (sprite.currentAnimationIndex)
                  {
                    case 0:
                      Game1.spriteBatch.Draw(texture, Utility.snapToInt(new Vector2((float) ((double) playerPosition.X + 32.0 + 8.0) + (float) num2 + (float) Math.Min(8, f.toolPower.Value * 4), (float) ((double) playerPosition.Y - 128.0 + 8.0) + (float) num3 + (float) Math.Min(8, f.toolPower.Value * 4))), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.White, (float) (0.2617993950843811 + (double) Math.Min(f.toolPower.Value, 2) * 0.049087386578321457), new Vector2(0.0f, 16f), 4f, SpriteEffects.FlipHorizontally, layerDepth);
                      return;
                    case 1:
                      Game1.spriteBatch.Draw(texture, Utility.snapToInt(new Vector2(playerPosition.X - 16f + (float) num2, (float) ((double) playerPosition.Y - 128.0 + 16.0) + (float) num3)), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.White, -0.2617994f, new Vector2(0.0f, 16f), 4f, SpriteEffects.FlipHorizontally, layerDepth);
                      return;
                    case 2:
                      Game1.spriteBatch.Draw(texture, Utility.snapToInt(new Vector2((float) ((double) playerPosition.X - 64.0 + 4.0) + (float) num2, (float) ((double) playerPosition.Y - 128.0 + 60.0) + (float) num3)), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.White, -0.7853982f, new Vector2(0.0f, 16f), 4f, SpriteEffects.FlipHorizontally, layerDepth);
                      return;
                    case 3:
                      Game1.spriteBatch.Draw(texture, Utility.snapToInt(new Vector2((float) ((double) playerPosition.X - 64.0 + 20.0) + (float) num2, (float) ((double) playerPosition.Y - 64.0 + 76.0) + (float) num3)), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.White, -1.83259583f, new Vector2(0.0f, 16f), 4f, SpriteEffects.FlipHorizontally, layerDepth);
                      return;
                    case 4:
                      Game1.spriteBatch.Draw(texture, Utility.snapToInt(new Vector2((float) ((double) playerPosition.X - 64.0 + 24.0) + (float) num2, playerPosition.Y + 24f + (float) num3)), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.White, -1.83259583f, new Vector2(0.0f, 16f), 4f, SpriteEffects.FlipHorizontally, layerDepth);
                      return;
                    default:
                      return;
                  }
                }
              }
              else
              {
                Color color = fishingRod2.getColor();
                switch (sprite.currentAnimationIndex)
                {
                  case 0:
                    if (!fishingRod2.isReeling && !fishingRod2.isFishing && !fishingRod2.doneWithAnimation && fishingRod2.hasDoneFucntionYet && !fishingRod2.pullingOutOfWater)
                      return;
                    Game1.spriteBatch.Draw(texture, Utility.snapToInt(new Vector2(playerPosition.X - 64f + (float) num2, playerPosition.Y - 160f + (float) num3)), new Microsoft.Xna.Framework.Rectangle?(rectangle), color, 0.0f, Vector2.Zero, 4f, SpriteEffects.FlipHorizontally, layerDepth);
                    return;
                  case 1:
                    Game1.spriteBatch.Draw(texture, Utility.snapToInt(new Vector2(playerPosition.X - 64f + (float) num2, (float) ((double) playerPosition.Y - 160.0 + 8.0) + (float) num3)), new Microsoft.Xna.Framework.Rectangle?(rectangle), color, 0.0f, Vector2.Zero, 4f, SpriteEffects.FlipHorizontally, layerDepth);
                    return;
                  case 2:
                    Game1.spriteBatch.Draw(texture, Utility.snapToInt(new Vector2((float) ((double) playerPosition.X - 96.0 + 32.0) + (float) num2, (float) ((double) playerPosition.Y - 128.0 - 24.0) + (float) num3)), new Microsoft.Xna.Framework.Rectangle?(rectangle), color, 0.0f, Vector2.Zero, 4f, SpriteEffects.FlipHorizontally, layerDepth);
                    return;
                  case 3:
                    Game1.spriteBatch.Draw(texture, Utility.snapToInt(new Vector2((float) ((double) playerPosition.X - 96.0 + 24.0) + (float) num2, (float) ((double) playerPosition.Y - 128.0 - 32.0) + (float) num3)), new Microsoft.Xna.Framework.Rectangle?(rectangle), color, 0.0f, Vector2.Zero, 4f, SpriteEffects.FlipHorizontally, layerDepth);
                    return;
                  case 4:
                    if (fishingRod2.isFishing || fishingRod2.doneWithAnimation)
                    {
                      Game1.spriteBatch.Draw(texture, Utility.snapToInt(new Vector2(playerPosition.X - 64f + (float) num2, playerPosition.Y - 160f + (float) num3)), new Microsoft.Xna.Framework.Rectangle?(rectangle), color, 0.0f, Vector2.Zero, 4f, SpriteEffects.FlipHorizontally, layerDepth);
                      return;
                    }
                    Game1.spriteBatch.Draw(texture, Utility.snapToInt(new Vector2(playerPosition.X - 64f + (float) num2, (float) ((double) playerPosition.Y - 160.0 + 4.0) + (float) num3)), new Microsoft.Xna.Framework.Rectangle?(rectangle), color, 0.0f, Vector2.Zero, 4f, SpriteEffects.FlipHorizontally, layerDepth);
                    return;
                  case 5:
                    Game1.spriteBatch.Draw(texture, Utility.snapToInt(new Vector2(playerPosition.X - 64f + (float) num2, playerPosition.Y - 160f + (float) num3)), new Microsoft.Xna.Framework.Rectangle?(rectangle), color, 0.0f, Vector2.Zero, 4f, SpriteEffects.FlipHorizontally, layerDepth);
                    return;
                  default:
                    return;
                }
              }
            default:
              if (sprite.currentAnimationIndex > 2 && (!(f.CurrentTool is FishingRod currentTool4) || currentTool4.isCasting || currentTool4.castedButBobberStillInAir || currentTool4.isTimingCast))
              {
                Point tilePoint = f.TilePoint;
                if (f.currentLocation.hasTileAt(tilePoint, "Front") && (double) f.Position.Y % 64.0 < 32.0 && (double) f.Position.Y % 64.0 > 16.0)
                  return;
              }
              Tool currentTool5 = f.CurrentTool;
              if (!(currentTool5 is FishingRod fishingRod3))
              {
                if (currentTool5 is WateringCan)
                {
                  switch (sprite.currentAnimationIndex)
                  {
                    case 0:
                    case 1:
                      Game1.spriteBatch.Draw(texture, Utility.snapToInt(new Vector2(playerPosition.X + (float) num2, (float) ((double) playerPosition.Y - 128.0 + 16.0) + (float) num3)), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.White, 0.0f, new Vector2(0.0f, 16f), 4f, SpriteEffects.None, layerDepth);
                      return;
                    case 2:
                      Game1.spriteBatch.Draw(texture, Utility.snapToInt(new Vector2(playerPosition.X + (float) num2, (float) ((double) playerPosition.Y - 128.0 - (f.FacingDirection == 2 ? -4.0 : 32.0)) + (float) num3)), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.White, 0.0f, new Vector2(0.0f, 16f), 4f, SpriteEffects.None, layerDepth);
                      return;
                    case 3:
                      if (f.FacingDirection == 2)
                        rectangle.X += 16 /*0x10*/;
                      Game1.spriteBatch.Draw(texture, Utility.snapToInt(new Vector2((float) ((double) playerPosition.X + (double) num2 - (f.FacingDirection == 2 ? 4.0 : 0.0)), (float) ((double) playerPosition.Y - 128.0 - (f.FacingDirection == 2 ? -24.0 : 64.0)) + (float) num3)), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.White, 0.0f, new Vector2(0.0f, 16f), 4f, SpriteEffects.None, layerDepth);
                      return;
                    default:
                      return;
                  }
                }
                else
                {
                  switch (sprite.currentAnimationIndex)
                  {
                    case 0:
                      if (f.FacingDirection == 0)
                      {
                        Game1.spriteBatch.Draw(texture, Utility.snapToInt(new Vector2(playerPosition.X + (float) num2, (float) ((double) playerPosition.Y - 128.0 - 8.0) + (float) num3 + (float) Math.Min(8, f.toolPower.Value * 4))), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.White, 0.0f, new Vector2(0.0f, 16f), 4f, SpriteEffects.None, layerDepth);
                        return;
                      }
                      Game1.spriteBatch.Draw(texture, Utility.snapToInt(new Vector2((float) ((double) playerPosition.X + (double) num2 - 20.0), (float) ((double) playerPosition.Y - 128.0 + 12.0) + (float) num3 + (float) Math.Min(8, f.toolPower.Value * 4))), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.White, 0.0f, new Vector2(0.0f, 16f), 4f, SpriteEffects.None, layerDepth);
                      return;
                    case 1:
                      if (f.FacingDirection == 0)
                      {
                        Game1.spriteBatch.Draw(texture, Utility.snapToInt(new Vector2((float) ((double) playerPosition.X + (double) num2 + 4.0), (float) ((double) playerPosition.Y - 128.0 + 40.0) + (float) num3)), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.White, 0.0f, new Vector2(0.0f, 16f), 4f, SpriteEffects.None, layerDepth);
                        return;
                      }
                      Game1.spriteBatch.Draw(texture, Utility.snapToInt(new Vector2((float) ((double) playerPosition.X + (double) num2 - 12.0), (float) ((double) playerPosition.Y - 128.0 + 32.0) + (float) num3)), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.White, -0.1308997f, new Vector2(0.0f, 16f), 4f, SpriteEffects.None, layerDepth);
                      return;
                    case 2:
                      Game1.spriteBatch.Draw(texture, Utility.snapToInt(new Vector2(playerPosition.X + (float) num2, (float) ((double) playerPosition.Y - 128.0 + 64.0) + (float) num3)), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.White, 0.0f, new Vector2(0.0f, 16f), 4f, SpriteEffects.None, layerDepth);
                      return;
                    case 3:
                      if (f.FacingDirection == 0)
                        return;
                      Game1.spriteBatch.Draw(texture, Utility.snapToInt(new Vector2(playerPosition.X + (float) num2, (float) ((double) playerPosition.Y - 64.0 + 44.0) + (float) num3)), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.White, 0.0f, new Vector2(0.0f, 16f), 4f, SpriteEffects.None, layerDepth);
                      return;
                    case 4:
                      if (f.FacingDirection == 0)
                        return;
                      Game1.spriteBatch.Draw(texture, Utility.snapToInt(new Vector2(playerPosition.X + (float) num2, (float) ((double) playerPosition.Y - 64.0 + 48.0) + (float) num3)), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.White, 0.0f, new Vector2(0.0f, 16f), 4f, SpriteEffects.None, layerDepth);
                      return;
                    case 5:
                      Game1.spriteBatch.Draw(texture, Utility.snapToInt(new Vector2(playerPosition.X + (float) num2, (float) ((double) playerPosition.Y - 64.0 + 32.0) + (float) num3)), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.White, 0.0f, new Vector2(0.0f, 16f), 4f, SpriteEffects.None, layerDepth);
                      return;
                    default:
                      return;
                  }
                }
              }
              else
              {
                if (sprite.currentAnimationIndex <= 2)
                {
                  Point tilePoint = f.TilePoint;
                  --tilePoint.Y;
                  if (f.currentLocation.hasTileAt(tilePoint, "Front"))
                    return;
                }
                if (f.FacingDirection == 2)
                  layerDepth += 0.01f;
                Color color = fishingRod3.getColor();
                switch (sprite.currentAnimationIndex)
                {
                  case 0:
                    if (fishingRod3.showingTreasure || fishingRod3.fishCaught || f.FacingDirection == 0 && fishingRod3.isFishing && !fishingRod3.isReeling)
                      return;
                    Game1.spriteBatch.Draw(texture, Utility.snapToInt(new Vector2(playerPosition.X - 64f, (float) ((double) playerPosition.Y - 128.0 + 4.0))), new Microsoft.Xna.Framework.Rectangle?(rectangle), color, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth);
                    return;
                  case 1:
                    Game1.spriteBatch.Draw(texture, Utility.snapToInt(new Vector2(playerPosition.X - 64f, (float) ((double) playerPosition.Y - 128.0 + 4.0))), new Microsoft.Xna.Framework.Rectangle?(rectangle), color, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth);
                    return;
                  case 2:
                    Game1.spriteBatch.Draw(texture, Utility.snapToInt(new Vector2(playerPosition.X - 64f, (float) ((double) playerPosition.Y - 128.0 + 4.0))), new Microsoft.Xna.Framework.Rectangle?(rectangle), color, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth);
                    return;
                  case 3:
                    if (f.FacingDirection != 2)
                      return;
                    Game1.spriteBatch.Draw(texture, Utility.snapToInt(new Vector2(playerPosition.X - 64f, (float) ((double) playerPosition.Y - 128.0 + 4.0))), new Microsoft.Xna.Framework.Rectangle?(rectangle), color, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth);
                    return;
                  case 4:
                    if (f.FacingDirection == 0 && fishingRod3.isFishing)
                    {
                      Game1.spriteBatch.Draw(texture, Utility.snapToInt(new Vector2(playerPosition.X - 80f, playerPosition.Y - 96f)), new Microsoft.Xna.Framework.Rectangle?(rectangle), color, 0.0f, Vector2.Zero, 4f, SpriteEffects.FlipVertically, layerDepth);
                      return;
                    }
                    if (f.FacingDirection != 2)
                      return;
                    Game1.spriteBatch.Draw(texture, Utility.snapToInt(new Vector2(playerPosition.X - 64f, (float) ((double) playerPosition.Y - 128.0 + 4.0))), new Microsoft.Xna.Framework.Rectangle?(rectangle), color, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth);
                    return;
                  case 5:
                    if (f.FacingDirection != 2 || fishingRod3.showingTreasure || fishingRod3.fishCaught)
                      return;
                    Game1.spriteBatch.Draw(texture, Utility.snapToInt(new Vector2(playerPosition.X - 64f, (float) ((double) playerPosition.Y - 128.0 + 4.0))), new Microsoft.Xna.Framework.Rectangle?(rectangle), color, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth);
                    return;
                  default:
                    return;
                }
              }
          }
        default:
          if (f != Game1.player)
          {
            if (sprite.currentSingleAnimation < 160 /*0xA0*/ || sprite.currentSingleAnimation >= 192 /*0xC0*/)
              break;
            if (f.CurrentTool != null)
            {
              f.CurrentTool.Update(f.FacingDirection, 0, f);
              currentToolIndex = f.CurrentTool.CurrentParentTileIndex;
              goto label_11;
            }
            goto label_11;
          }
          goto label_11;
      }
    }
  }

  /// ####################
  ///             OTHER HELPER METHODS
  ///             ####################
  public static Vector2 GlobalToLocal(xTile.Dimensions.Rectangle viewport, Vector2 globalPosition)
  {
    return new Vector2(globalPosition.X - (float) viewport.X, globalPosition.Y - (float) viewport.Y);
  }

  public static bool IsEnglish()
  {
    return Game1.content.GetCurrentLanguage() == LocalizedContentManager.LanguageCode.en;
  }

  public static Vector2 GlobalToLocal(Vector2 globalPosition)
  {
    return new Vector2(globalPosition.X - (float) Game1.viewport.X, globalPosition.Y - (float) Game1.viewport.Y);
  }

  public static Microsoft.Xna.Framework.Rectangle GlobalToLocal(
    xTile.Dimensions.Rectangle viewport,
    Microsoft.Xna.Framework.Rectangle globalPosition)
  {
    return new Microsoft.Xna.Framework.Rectangle(globalPosition.X - viewport.X, globalPosition.Y - viewport.Y, globalPosition.Width, globalPosition.Height);
  }

  public static string parseText(string text, SpriteFont whichFont, int width)
  {
    if (text == null)
      return "";
    text = Dialogue.applyGenderSwitchBlocks(Game1.player.Gender, text);
    Game1._ParseTextStringBuilder.Clear();
    Game1._ParseTextStringBuilderLine.Clear();
    Game1._ParseTextStringBuilderWord.Clear();
    float num1 = 0.0f;
    switch (LocalizedContentManager.CurrentLanguageCode)
    {
      case LocalizedContentManager.LanguageCode.ja:
      case LocalizedContentManager.LanguageCode.zh:
      case LocalizedContentManager.LanguageCode.th:
        foreach (object match in Game1.asianSpacingRegex.Matches(text))
        {
          string text1 = match.ToString();
          float num2 = whichFont.MeasureString(text1).X + whichFont.Spacing;
          if ((double) num1 + (double) num2 > (double) width || text1.Equals(Environment.NewLine) || text1.Equals("\n"))
          {
            Game1._ParseTextStringBuilder.Append(Game1._ParseTextStringBuilderLine);
            Game1._ParseTextStringBuilder.Append(Environment.NewLine);
            Game1._ParseTextStringBuilderLine.Clear();
            num1 = 0.0f;
          }
          if (!text1.Equals(Environment.NewLine) && !text1.Equals("\n"))
          {
            Game1._ParseTextStringBuilderLine.Append(text1);
            num1 += num2;
          }
        }
        Game1._ParseTextStringBuilder.Append(Game1._ParseTextStringBuilderLine);
        return Game1._ParseTextStringBuilder.ToString();
      default:
        float num3 = 0.0f;
        for (int index = 0; index < text.Length; ++index)
        {
          char ch = text[index];
          bool flag;
          switch (ch)
          {
            case '\n':
              flag = true;
              break;
            case '\r':
              continue;
            case ' ':
              flag = true;
              break;
            default:
              Game1._ParseTextStringBuilderWord.Append(ch);
              flag = index == text.Length - 1;
              break;
          }
          if (flag)
          {
            try
            {
              float num4 = whichFont.MeasureString(Game1._ParseTextStringBuilderWord).X + whichFont.Spacing;
              if ((double) num3 + (double) num4 > (double) width)
              {
                Game1._ParseTextStringBuilder.Append(Game1._ParseTextStringBuilderLine);
                Game1._ParseTextStringBuilder.Append(Environment.NewLine);
                Game1._ParseTextStringBuilderLine.Clear();
                num3 = 0.0f;
              }
              if (ch == '\n')
              {
                Game1._ParseTextStringBuilderLine.Append(Game1._ParseTextStringBuilderWord);
                Game1._ParseTextStringBuilder.Append(Game1._ParseTextStringBuilderLine);
                Game1._ParseTextStringBuilder.Append(Environment.NewLine);
                Game1._ParseTextStringBuilderLine.Clear();
                Game1._ParseTextStringBuilderWord.Clear();
                num3 = 0.0f;
                continue;
              }
              Game1._ParseTextStringBuilderLine.Append(Game1._ParseTextStringBuilderWord);
              Game1._ParseTextStringBuilderLine.Append(" ");
              float num5 = whichFont.MeasureString(" ").X + whichFont.Spacing;
              num3 += num4 + num5;
            }
            catch (Exception ex)
            {
              Game1.log.Error("Exception measuring string: ", ex);
            }
            Game1._ParseTextStringBuilderWord.Clear();
          }
        }
        Game1._ParseTextStringBuilderLine.Append(Game1._ParseTextStringBuilderWord);
        Game1._ParseTextStringBuilder.Append(Game1._ParseTextStringBuilderLine);
        return Game1._ParseTextStringBuilder.ToString();
    }
  }

  public static void UpdateHorseOwnership()
  {
    bool flag1 = false;
    Dictionary<long, Horse> dictionary1 = new Dictionary<long, Horse>();
    HashSet<Horse> horseSet = new HashSet<Horse>();
    List<Stable> stables = new List<Stable>();
    Utility.ForEachBuilding<Stable>((Func<Stable, bool>) (stable =>
    {
      stables.Add(stable);
      return true;
    }));
    foreach (Stable stable in stables)
    {
      if (stable.owner.Value == -6666666L && Game1.GetPlayer(-6666666L) == null)
        stable.owner.Value = Game1.player.UniqueMultiplayerID;
      stable.grabHorse();
    }
    foreach (Stable stable in stables)
    {
      Horse stableHorse = stable.getStableHorse();
      if (stableHorse != null && !horseSet.Contains(stableHorse) && stableHorse.getOwner() != null && !dictionary1.ContainsKey(stableHorse.getOwner().UniqueMultiplayerID) && stableHorse.getOwner().horseName.Value != null && stableHorse.getOwner().horseName.Value.Length > 0 && stableHorse.Name == stableHorse.getOwner().horseName.Value)
      {
        dictionary1[stableHorse.getOwner().UniqueMultiplayerID] = stableHorse;
        horseSet.Add(stableHorse);
        if (flag1)
          Game1.log.Verbose($"Assigned horse {stableHorse.Name} to {stableHorse.getOwner().Name} (Exact match)");
      }
    }
    Dictionary<string, Farmer> dictionary2 = new Dictionary<string, Farmer>();
    foreach (Farmer allFarmer in Game1.getAllFarmers())
    {
      if (!string.IsNullOrEmpty(allFarmer?.horseName.Value))
      {
        bool flag2 = false;
        foreach (Horse horse in horseSet)
        {
          if (horse.getOwner() == allFarmer)
          {
            flag2 = true;
            break;
          }
        }
        if (!flag2)
          dictionary2[allFarmer.horseName.Value] = allFarmer;
      }
    }
    foreach (Stable stable in stables)
    {
      Horse stableHorse = stable.getStableHorse();
      Farmer farmer;
      if (stableHorse != null && !horseSet.Contains(stableHorse) && stableHorse.getOwner() != null && stableHorse.Name != null && stableHorse.Name.Length > 0 && dictionary2.TryGetValue(stableHorse.Name, out farmer) && !dictionary1.ContainsKey(farmer.UniqueMultiplayerID))
      {
        stable.owner.Value = farmer.UniqueMultiplayerID;
        stable.updateHorseOwnership();
        dictionary1[stableHorse.getOwner().UniqueMultiplayerID] = stableHorse;
        horseSet.Add(stableHorse);
        if (flag1)
          Game1.log.Verbose($"Assigned horse {stableHorse.Name} to {stableHorse.getOwner().Name} (Name match from different owner.)");
      }
    }
    foreach (Stable stable in stables)
    {
      Horse stableHorse = stable.getStableHorse();
      if (stableHorse != null && !horseSet.Contains(stableHorse) && stableHorse.getOwner() != null && !dictionary1.ContainsKey(stableHorse.getOwner().UniqueMultiplayerID))
      {
        dictionary1[stableHorse.getOwner().UniqueMultiplayerID] = stableHorse;
        horseSet.Add(stableHorse);
        stable.updateHorseOwnership();
        if (flag1)
          Game1.log.Verbose($"Assigned horse {stableHorse.Name} to {stableHorse.getOwner().Name} (Owner's only stable)");
      }
    }
    foreach (Stable stable in stables)
    {
      Horse stableHorse = stable.getStableHorse();
      if (stableHorse != null && !horseSet.Contains(stableHorse))
      {
        foreach (Horse horse in horseSet)
        {
          if ((NetFieldBase<long, NetLong>) stableHorse.ownerId == horse.ownerId)
          {
            stable.owner.Value = 0L;
            stable.updateHorseOwnership();
            if (flag1)
            {
              Game1.log.Verbose("Unassigned horse (stable owner already has a horse).");
              break;
            }
            break;
          }
        }
      }
    }
  }

  public static string LoadStringByGender(Gender npcGender, string key)
  {
    return npcGender == Gender.Male ? Game1.content.LoadString(key).Split('/')[0] : ((IEnumerable<string>) Game1.content.LoadString(key).Split('/')).Last<string>();
  }

  public static string LoadStringByGender(
    Gender npcGender,
    string key,
    params object[] substitutions)
  {
    if (npcGender == Gender.Male)
    {
      string format = Game1.content.LoadString(key).Split('/')[0];
      if (substitutions.Length != 0)
      {
        try
        {
          return string.Format(format, substitutions);
        }
        catch
        {
          return format;
        }
      }
    }
    string format1 = ((IEnumerable<string>) Game1.content.LoadString(key).Split('/')).Last<string>();
    if (substitutions.Length == 0)
      return format1;
    try
    {
      return string.Format(format1, substitutions);
    }
    catch
    {
      return format1;
    }
  }

  public static string parseText(string text)
  {
    return Game1.parseText(text, Game1.dialogueFont, Game1.dialogueWidth);
  }

  public static Microsoft.Xna.Framework.Rectangle getSourceRectForStandardTileSheet(
    Texture2D tileSheet,
    int tilePosition,
    int width = -1,
    int height = -1)
  {
    if (width == -1)
      width = 64 /*0x40*/;
    if (height == -1)
      height = 64 /*0x40*/;
    return new Microsoft.Xna.Framework.Rectangle(tilePosition * width % tileSheet.Width, tilePosition * width / tileSheet.Width * height, width, height);
  }

  public static Microsoft.Xna.Framework.Rectangle getSquareSourceRectForNonStandardTileSheet(
    Texture2D tileSheet,
    int tileWidth,
    int tileHeight,
    int tilePosition)
  {
    return new Microsoft.Xna.Framework.Rectangle(tilePosition * tileWidth % tileSheet.Width, tilePosition * tileWidth / tileSheet.Width * tileHeight, tileWidth, tileHeight);
  }

  public static Microsoft.Xna.Framework.Rectangle getArbitrarySourceRect(
    Texture2D tileSheet,
    int tileWidth,
    int tileHeight,
    int tilePosition)
  {
    return tileSheet != null ? new Microsoft.Xna.Framework.Rectangle(tilePosition * tileWidth % tileSheet.Width, tilePosition * tileWidth / tileSheet.Width * tileHeight, tileWidth, tileHeight) : Microsoft.Xna.Framework.Rectangle.Empty;
  }

  public static string getTimeOfDayString(int time)
  {
    string str1 = time % 100 == 0 ? "0" : string.Empty;
    string str2;
    switch (LocalizedContentManager.CurrentLanguageCode)
    {
      case LocalizedContentManager.LanguageCode.ja:
        str2 = time / 100 % 12 == 0 ? "0" : (time / 100 % 12).ToString();
        break;
      case LocalizedContentManager.LanguageCode.ru:
      case LocalizedContentManager.LanguageCode.pt:
      case LocalizedContentManager.LanguageCode.es:
      case LocalizedContentManager.LanguageCode.de:
      case LocalizedContentManager.LanguageCode.th:
      case LocalizedContentManager.LanguageCode.fr:
      case LocalizedContentManager.LanguageCode.tr:
      case LocalizedContentManager.LanguageCode.hu:
        string str3 = (time / 100 % 24).ToString();
        str2 = time / 100 % 24 <= 9 ? "0" + str3 : str3;
        break;
      case LocalizedContentManager.LanguageCode.zh:
        str2 = (time / 100 % 24).ToString();
        break;
      default:
        str2 = time / 100 % 12 == 0 ? "12" : (time / 100 % 12).ToString();
        break;
    }
    string timeOfDayString = $"{str2}:{(object) (time % 100)}{str1}";
    switch (LocalizedContentManager.CurrentLanguageCode)
    {
      case LocalizedContentManager.LanguageCode.en:
        return $"{timeOfDayString} {(time < 1200 || time >= 2400 ? Game1.content.LoadString("Strings\\StringsFromCSFiles:DayTimeMoneyBox.cs.10370") : Game1.content.LoadString("Strings\\StringsFromCSFiles:DayTimeMoneyBox.cs.10371"))}";
      case LocalizedContentManager.LanguageCode.ja:
        return time >= 1200 && time < 2400 ? $"{Game1.content.LoadString("Strings\\StringsFromCSFiles:DayTimeMoneyBox.cs.10371")} {timeOfDayString}" : $"{Game1.content.LoadString("Strings\\StringsFromCSFiles:DayTimeMoneyBox.cs.10370")} {timeOfDayString}";
      case LocalizedContentManager.LanguageCode.fr:
        return time % 100 != 0 ? $"{str2}h{(time % 100).ToString()}" : str2 + "h";
      case LocalizedContentManager.LanguageCode.mod:
        return LocalizedContentManager.FormatTimeString(time, LocalizedContentManager.CurrentModLanguage.TimeFormat).ToString();
      default:
        return timeOfDayString;
    }
  }

  public static bool[,] getCircleOutlineGrid(int radius)
  {
    bool[,] circleOutlineGrid = new bool[radius * 2 + 1, radius * 2 + 1];
    int num1 = 1 - radius;
    int num2 = 1;
    int num3 = -2 * radius;
    int num4 = 0;
    int num5 = radius;
    int index1 = radius;
    int index2 = radius;
    circleOutlineGrid[index1, index2 + radius] = true;
    circleOutlineGrid[index1, index2 - radius] = true;
    circleOutlineGrid[index1 + radius, index2] = true;
    circleOutlineGrid[index1 - radius, index2] = true;
    while (num4 < num5)
    {
      if (num1 >= 0)
      {
        --num5;
        num3 += 2;
        num1 += num3;
      }
      ++num4;
      num2 += 2;
      num1 += num2;
      circleOutlineGrid[index1 + num4, index2 + num5] = true;
      circleOutlineGrid[index1 - num4, index2 + num5] = true;
      circleOutlineGrid[index1 + num4, index2 - num5] = true;
      circleOutlineGrid[index1 - num4, index2 - num5] = true;
      circleOutlineGrid[index1 + num5, index2 + num4] = true;
      circleOutlineGrid[index1 - num5, index2 + num4] = true;
      circleOutlineGrid[index1 + num5, index2 - num4] = true;
      circleOutlineGrid[index1 - num5, index2 - num4] = true;
    }
    return circleOutlineGrid;
  }

  /// <summary>Get the internal identifier for the current farm type. This is either the numeric index for a vanilla farm, or the <see cref="F:StardewValley.GameData.ModFarmType.Id" /> field for a custom type.</summary>
  public static string GetFarmTypeID()
  {
    return Game1.whichFarm != 7 || Game1.whichModFarm == null ? Game1.whichFarm.ToString() : Game1.whichModFarm.Id;
  }

  /// <summary>Get the human-readable identifier for the current farm type. For a custom farm type, this is equivalent to <see cref="M:StardewValley.Game1.GetFarmTypeID" />.</summary>
  public static string GetFarmTypeKey()
  {
    switch (Game1.whichFarm)
    {
      case 0:
        return "Standard";
      case 1:
        return "Riverland";
      case 2:
        return "Forest";
      case 3:
        return "Hilltop";
      case 4:
        return "Wilderness";
      case 5:
        return "FourCorners";
      case 6:
        return "Beach";
      default:
        return Game1.GetFarmTypeID();
    }
  }

  public void _PerformRemoveNormalItemFromWorldOvernight(string itemId)
  {
    Utility.ForEachLocation((Func<GameLocation, bool>) (location =>
    {
      this._RecursiveRemoveThisNormalItemLocation(location, itemId);
      return true;
    }), includeGenerated: true);
    Game1.player.team.returnedDonations.RemoveWhere((Func<Item, bool>) (item => this._RecursiveRemoveThisNormalItemItem(item, itemId)));
    foreach (IList<Item> list in Game1.player.team.globalInventories.Values)
      list.RemoveWhere<Item>((Predicate<Item>) (item => this._RecursiveRemoveThisNormalItemItem(item, itemId)));
    foreach (SpecialOrder specialOrder in Game1.player.team.specialOrders)
    {
      for (int index = 0; index < specialOrder.donatedItems.Count; ++index)
      {
        if (this._RecursiveRemoveThisNormalItemItem(specialOrder.donatedItems[index], itemId))
          specialOrder.donatedItems[index] = (Item) null;
      }
    }
  }

  protected virtual void _PerformRemoveNormalItemFromFarmerOvernight(Farmer farmer, string itemId)
  {
    for (int index = 0; index < farmer.Items.Count; ++index)
    {
      if (this._RecursiveRemoveThisNormalItemItem(farmer.Items[index], itemId))
        farmer.Items[index] = (Item) null;
    }
    farmer.itemsLostLastDeath.RemoveWhere((Func<Item, bool>) (item => this._RecursiveRemoveThisNormalItemItem(item, itemId)));
    if (farmer.recoveredItem != null && this._RecursiveRemoveThisNormalItemItem(farmer.recoveredItem, itemId))
    {
      farmer.recoveredItem = (Item) null;
      farmer.mailbox.Remove("MarlonRecovery");
      farmer.mailForTomorrow.Remove("MarlonRecovery");
    }
    if (farmer.toolBeingUpgraded.Value == null || !this._RecursiveRemoveThisNormalItemItem((Item) farmer.toolBeingUpgraded.Value, itemId))
      return;
    farmer.toolBeingUpgraded.Value = (Tool) null;
  }

  protected virtual bool _RecursiveRemoveThisNormalItemItem(Item this_item, string itemId)
  {
    if (this_item == null)
      return false;
    if (this_item is Object @object)
    {
      if (@object.heldObject.Value != null && this._RecursiveRemoveThisNormalItemItem((Item) @object.heldObject.Value, itemId))
      {
        @object.ResetParentSheetIndex();
        @object.heldObject.Value = (Object) null;
        @object.readyForHarvest.Value = false;
        @object.showNextIndex.Value = false;
      }
      switch (@object)
      {
        case StorageFurniture storageFurniture:
          bool flag1 = false;
          for (int index = 0; index < storageFurniture.heldItems.Count; ++index)
          {
            Item heldItem = storageFurniture.heldItems[index];
            if (heldItem != null && this._RecursiveRemoveThisNormalItemItem(heldItem, itemId))
            {
              storageFurniture.heldItems[index] = (Item) null;
              flag1 = true;
            }
          }
          if (flag1)
          {
            storageFurniture.ClearNulls();
            break;
          }
          break;
        case IndoorPot indoorPot:
          if (indoorPot.hoeDirt.Value != null)
          {
            this._RecursiveRemoveThisNormalItemDirt(indoorPot.hoeDirt.Value, (GameLocation) null, Vector2.Zero, itemId);
            break;
          }
          break;
        case Chest chest:
          bool flag2 = false;
          IInventory items = (IInventory) chest.Items;
          for (int index = 0; index < items.Count; ++index)
          {
            Item this_item1 = items[index];
            if (this_item1 != null && this._RecursiveRemoveThisNormalItemItem(this_item1, itemId))
            {
              items[index] = (Item) null;
              flag2 = true;
            }
          }
          if (flag2)
          {
            chest.clearNulls();
            break;
          }
          break;
      }
      if (@object.heldObject.Value != null && this._RecursiveRemoveThisNormalItemItem((Item) @object.heldObject.Value, itemId))
        @object.heldObject.Value = (Object) null;
    }
    return Utility.IsNormalObjectAtParentSheetIndex(this_item, itemId);
  }

  protected virtual void _RecursiveRemoveThisNormalItemDirt(
    HoeDirt dirt,
    GameLocation location,
    Vector2 coord,
    string itemId)
  {
    if (dirt.crop == null || !(dirt.crop.indexOfHarvest.Value == itemId))
      return;
    dirt.destroyCrop(false);
  }

  protected virtual void _RecursiveRemoveThisNormalItemLocation(GameLocation l, string itemId)
  {
    if (l == null)
      return;
    List<Guid> guidList = new List<Guid>();
    foreach (Furniture this_item in l.furniture)
    {
      if (this._RecursiveRemoveThisNormalItemItem((Item) this_item, itemId))
        guidList.Add(l.furniture.GuidOf(this_item));
    }
    foreach (Guid guid in guidList)
      l.furniture.Remove(guid);
    foreach (NPC character in l.characters)
    {
      if (character is Monster monster)
        monster.objectsToDrop?.RemoveWhere((Func<string, bool>) (id => id == itemId));
    }
    Chest fridge = l.GetFridge(false);
    if (fridge != null)
    {
      IInventory items = (IInventory) fridge.Items;
      for (int index = 0; index < items.Count; ++index)
      {
        Item this_item = items[index];
        if (this_item != null && this._RecursiveRemoveThisNormalItemItem(this_item, itemId))
          items[index] = (Item) null;
      }
    }
    foreach (Vector2 key in l.terrainFeatures.Keys)
    {
      if (l.terrainFeatures[key] is HoeDirt terrainFeature)
        this._RecursiveRemoveThisNormalItemDirt(terrainFeature, l, key, itemId);
    }
    foreach (Building building in l.buildings)
    {
      foreach (Chest buildingChest in building.buildingChests)
      {
        bool flag = false;
        for (int index = 0; index < buildingChest.Items.Count; ++index)
        {
          Item this_item = buildingChest.Items[index];
          if (this_item != null && this._RecursiveRemoveThisNormalItemItem(this_item, itemId))
          {
            buildingChest.Items[index] = (Item) null;
            flag = true;
          }
        }
        if (flag)
          buildingChest.clearNulls();
      }
    }
    foreach (Vector2 key in l.objects.Keys.ToArray<Vector2>())
    {
      Object this_item = l.objects[key];
      if (this_item != fridge && this._RecursiveRemoveThisNormalItemItem((Item) this_item, itemId))
        l.objects.Remove(key);
    }
    l.debris.RemoveWhere((Func<Debris, bool>) (debris => debris.item != null && this._RecursiveRemoveThisNormalItemItem(debris.item, itemId)));
    if (!(l is ShopLocation shopLocation))
      return;
    shopLocation.itemsFromPlayerToSell.RemoveWhere((Func<Item, bool>) (item => this._RecursiveRemoveThisNormalItemItem(item, itemId)));
    shopLocation.itemsToStartSellingTomorrow.RemoveWhere((Func<Item, bool>) (item => this._RecursiveRemoveThisNormalItemItem(item, itemId)));
  }

  public static bool GetHasRoomAnotherFarm() => true;

  /// <summary>Reset most game state when returning to the title screen from a title sub-menu.</summary>
  /// <remarks>
  ///   <para>This is specialized code that normally shouldn't be called directly. This differs from <see cref="M:StardewValley.Game1.CleanupReturningToTitle" /> in that it doesn't exit split screens, reset the title menu, or reset the game window.</para>
  ///   <para>This is called automatically from <see cref="M:StardewValley.Game1.CleanupReturningToTitle" />.</para>
  /// </remarks>
  public virtual void ResetGameStateOnTitleScreen()
  {
    LocalizedContentManager.localizedAssetNames.Clear();
    Event.invalidFestivals.Clear();
    NPC.invalidDialogueFiles.Clear();
    SaveGame.CancelToTitle = false;
    Game1.overlayMenu = (IClickableMenu) null;
    Game1.multiplayer.cachedMultiplayerMaps.Clear();
    Game1.keyboardFocusInstance = (Game1) null;
    BuildingPaintMenu.savedColors = (List<Vector3>) null;
    Game1.startingGameSeed = new ulong?();
    Game1.UseLegacyRandom = false;
    Game1._afterNewDayAction = (Action) null;
    Game1._currentMinigame = (IMinigame) null;
    Game1.gameMode = (byte) 0;
    this._isSaving = false;
    Game1._mouseCursorTransparency = 1f;
    Game1._newDayTask = (Task) null;
    Game1.newDaySync.destroy();
    Game1.netReady.Reset();
    Game1.dedicatedServer.Reset();
    Game1.resetPlayer();
    Game1.afterDialogues = (Game1.afterFadeFunction) null;
    Game1.afterFade = (Game1.afterFadeFunction) null;
    Game1.afterPause = (Game1.afterFadeFunction) null;
    Game1.afterViewport = (Game1.afterFadeFunction) null;
    Game1.ambientLight = new Color(0, 0, 0, 0);
    Game1.background = (Background) null;
    Game1.chatBox = (ChatBox) null;
    Game1.specialCurrencyDisplay?.Cleanup();
    GameLocation.PlayedNewLocationContextMusic = false;
    Game1.IsPlayingBackgroundMusic = false;
    Game1.IsPlayingNightAmbience = false;
    Game1.IsPlayingOutdoorsAmbience = false;
    Game1.IsPlayingMorningSong = false;
    Game1.IsPlayingTownMusic = false;
    Game1.specialCurrencyDisplay = (SpecialCurrencyDisplay) null;
    Game1.conventionMode = false;
    Game1.currentCursorTile = Vector2.Zero;
    Game1.currentDialogueCharacterIndex = 0;
    Game1.currentLightSources.Clear();
    Game1.currentLoader = (IEnumerator<int>) null;
    Game1.currentLocation = (GameLocation) null;
    Game1._PreviousNonNullLocation = (GameLocation) null;
    Game1.currentObjectDialogue.Clear();
    Game1.currentQuestionChoice = 0;
    Game1.season = Season.Spring;
    Game1.currentSpeaker = (NPC) null;
    Game1.currentViewportTarget = Vector2.Zero;
    Game1.cursorTileHintCheckTimer = 0;
    Game1.CustomData = new SerializableDictionary<string, string>();
    Game1.player.team.sharedDailyLuck.Value = 0.001;
    Game1.dayOfMonth = 0;
    Game1.debrisSoundInterval = 0.0f;
    Game1.debrisWeather.Clear();
    Game1.debugMode = false;
    Game1.debugOutput = (string) null;
    Game1.debugPresenceString = "In menus";
    Game1.delayedActions.Clear();
    Game1.morningSongPlayAction = (DelayedAction) null;
    Game1.dialogueButtonScale = 1f;
    Game1.dialogueButtonShrinking = false;
    Game1.dialogueTyping = false;
    Game1.dialogueTypingInterval = 0;
    Game1.dialogueUp = false;
    Game1.dialogueWidth = 1024 /*0x0400*/;
    Game1.displayFarmer = true;
    Game1.displayHUD = true;
    Game1.downPolling = 0.0f;
    Game1.drawGrid = false;
    Game1.drawLighting = false;
    Game1.elliottBookName = "Blue Tower";
    Game1.endOfNightMenus.Clear();
    Game1.errorMessage = "";
    Game1.eveningColor = new Color((int) byte.MaxValue, (int) byte.MaxValue, 0, (int) byte.MaxValue);
    Game1.eventOver = false;
    Game1.eventUp = false;
    Game1.exitToTitle = false;
    Game1.facingDirectionAfterWarp = 0;
    Game1.fadeIn = true;
    Game1.fadeToBlack = false;
    Game1.fadeToBlackAlpha = 1.02f;
    Game1.farmEvent = (FarmEvent) null;
    Game1.flashAlpha = 0.0f;
    Game1.freezeControls = false;
    Game1.gamePadAButtonPolling = 0;
    Game1.gameTimeInterval = 0;
    Game1.globalFade = false;
    Game1.globalFadeSpeed = 0.0f;
    Game1.haltAfterCheck = false;
    Game1.hasLoadedGame = false;
    Game1.hasStartedDay = false;
    Game1.hitShakeTimer = 0;
    Game1.hudMessages.Clear();
    Game1.isActionAtCurrentCursorTile = false;
    Game1.isDebrisWeather = false;
    Game1.isInspectionAtCurrentCursorTile = false;
    Game1.isLightning = false;
    Game1.isQuestion = false;
    Game1.isRaining = false;
    Game1.wasGreenRain = false;
    Game1.isSnowing = false;
    Game1.killScreen = false;
    Game1.lastCursorMotionWasMouse = true;
    Game1.lastCursorTile = Vector2.Zero;
    Game1.lastMousePositionBeforeFade = Point.Zero;
    Game1.leftPolling = 0.0f;
    Game1.loadingMessage = "";
    Game1.locationRequest = (LocationRequest) null;
    Game1.warpingForForcedRemoteEvent = false;
    Game1.locations.Clear();
    Game1.mailbox.Clear();
    Game1.mapDisplayDevice = this.CreateDisplayDevice((ContentManager) Game1.content, this.GraphicsDevice);
    Game1.messageAfterPause = "";
    Game1.messagePause = false;
    Game1.mouseClickPolling = 0;
    Game1.mouseCursor = Game1.cursor_default;
    Game1.multiplayerMode = (byte) 0;
    Game1.netWorldState = new NetRoot<NetWorldState>(new NetWorldState());
    Game1.newDay = false;
    Game1.nonWarpFade = false;
    Game1.noteBlockTimer = 0.0f;
    Game1.npcDialogues = (Dictionary<string, Stack<Dialogue>>) null;
    Game1.objectDialoguePortraitPerson = (NPC) null;
    Game1.hasApplied1_3_UpdateChanges = false;
    Game1.hasApplied1_4_UpdateChanges = false;
    Game1.remoteEventQueue.Clear();
    Game1.bannedUsers?.Clear();
    Game1.nextClickableMenu.Clear();
    Game1.actionsWhenPlayerFree.Clear();
    Game1.onScreenMenus.Clear();
    Game1.onScreenMenus.Add((IClickableMenu) new Toolbar());
    Game1.dayTimeMoneyBox = new DayTimeMoneyBox();
    Game1.onScreenMenus.Add((IClickableMenu) Game1.dayTimeMoneyBox);
    Game1.buffsDisplay = new BuffsDisplay();
    Game1.onScreenMenus.Add((IClickableMenu) Game1.buffsDisplay);
    bool gamepadControls = Game1.options.gamepadControls;
    bool snappyMenus = Game1.options.snappyMenus;
    Game1.options = new Options();
    Game1.options.gamepadControls = gamepadControls;
    Game1.options.snappyMenus = snappyMenus;
    foreach (KeyValuePair<long, Farmer> otherFarmer in Game1.otherFarmers)
      otherFarmer.Value.unload();
    Game1.otherFarmers.Clear();
    Game1.outdoorLight = new Color((int) byte.MaxValue, (int) byte.MaxValue, 0, (int) byte.MaxValue);
    Game1.overlayMenu = (IClickableMenu) null;
    this.panFacingDirectionWait = false;
    Game1.panMode = false;
    this.panModeString = (string) null;
    Game1.pauseAccumulator = 0.0f;
    Game1.paused = false;
    Game1.pauseThenDoFunctionTimer = 0;
    Game1.pauseTime = 0.0f;
    Game1.previousViewportPosition = Vector2.Zero;
    Game1.questionChoices.Clear();
    Game1.quit = false;
    Game1.rightClickPolling = 0;
    Game1.rightPolling = 0.0f;
    Game1.runThreshold = 0.5f;
    Game1.samBandName = "The Alfalfas";
    Game1.saveOnNewDay = true;
    Game1.startingCabins = 0;
    Game1.cabinsSeparate = false;
    Game1.screenGlow = false;
    Game1.screenGlowAlpha = 0.0f;
    Game1.screenGlowColor = new Color(0, 0, 0, 0);
    Game1.screenGlowHold = false;
    Game1.screenGlowMax = 0.0f;
    Game1.screenGlowRate = 0.005f;
    Game1.screenGlowUp = false;
    Game1.screenOverlayTempSprites.Clear();
    Game1.uiOverlayTempSprites.Clear();
    this.newGameSetupOptions.Clear();
    Game1.showingEndOfNightStuff = false;
    Game1.spawnMonstersAtNight = false;
    Game1.staminaShakeTimer = 0;
    Game1.textColor = new Color(34, 17, 34, (int) byte.MaxValue);
    Game1.textShadowColor = new Color(206, 156, 95, (int) byte.MaxValue);
    Game1.thumbstickMotionAccell = 1f;
    Game1.thumbstickMotionMargin = 0;
    Game1.thumbstickPollingTimer = 0;
    Game1.thumbStickSensitivity = 0.1f;
    Game1.timeOfDay = 600;
    Game1.timeOfDayAfterFade = -1;
    Game1.timerUntilMouseFade = 0;
    Game1.toggleFullScreen = false;
    Game1.ResetToolSpriteSheet();
    Game1.triggerPolling = 0;
    Game1.uniqueIDForThisGame = (ulong) (DateTime.UtcNow - new DateTime(2012, 6, 22)).TotalSeconds;
    Game1.upPolling = 0.0f;
    Game1.viewportFreeze = false;
    Game1.viewportHold = 0;
    Game1.viewportPositionLerp = Vector2.Zero;
    Game1.viewportReachedTarget = (Game1.afterFadeFunction) null;
    Game1.viewportSpeed = 2f;
    Game1.viewportTarget = new Vector2((float) int.MinValue, (float) int.MinValue);
    Game1.wasMouseVisibleThisFrame = true;
    Game1.wasRainingYesterday = false;
    Game1.weatherForTomorrow = "Sun";
    Game1.elliottPiano = 0;
    Game1.weatherIcon = 0;
    Game1.weddingToday = false;
    Game1.whereIsTodaysFest = (string) null;
    Game1.worldStateIDs.Clear();
    Game1.whichFarm = 0;
    Game1.whichModFarm = (ModFarmType) null;
    Game1.windGust = 0.0f;
    Game1.xLocationAfterWarp = 0;
    Game1.game1.xTileContent.Dispose();
    Game1.game1.xTileContent = this.CreateContentManager(Game1.content.ServiceProvider, Game1.content.RootDirectory);
    Game1.year = 1;
    Game1.yLocationAfterWarp = 0;
    Game1.mailDeliveredFromMailForTomorrow.Clear();
    Game1.bundleType = Game1.BundleType.Default;
    JojaMart.Morris = (NPC) null;
    AmbientLocationSounds.onLocationLeave();
    WeatherDebris.globalWind = -0.25f;
    Utility.killAllStaticLoopingSoundCues();
    OptionsDropDown.selected = (OptionsDropDown) null;
    JunimoNoteMenu.tempSprites.Clear();
    JunimoNoteMenu.screenSwipe = (ScreenSwipe) null;
    JunimoNoteMenu.canClick = true;
    GameMenu.forcePreventClose = false;
    Club.timesPlayedCalicoJack = 0;
    MineShaft.activeMines.RemoveAll((Predicate<MineShaft>) (level =>
    {
      level.OnRemoved();
      return true;
    }));
    MineShaft.permanentMineChanges.Clear();
    MineShaft.numberOfCraftedStairsUsedThisRun = 0;
    MineShaft.mushroomLevelsGeneratedToday.Clear();
    VolcanoDungeon.activeLevels.RemoveAll((Predicate<VolcanoDungeon>) (level =>
    {
      level.OnRemoved();
      return true;
    }));
    ItemRegistry.ResetCache();
    Rumble.stopRumbling();
  }

  /// <summary>Reset all game state when returning to the title screen from a loaded save.</summary>
  /// <remarks>This is specialized code that should normally not be called directly. See also <see cref="M:StardewValley.Game1.ResetGameStateOnTitleScreen" />.</remarks>
  public virtual void CleanupReturningToTitle()
  {
    if (Game1.game1.IsMainInstance)
    {
      foreach (Game1 gameInstance in GameRunner.instance.gameInstances)
      {
        if (gameInstance != this)
          GameRunner.instance.RemoveGameInstance(gameInstance);
      }
    }
    else
      GameRunner.instance.RemoveGameInstance(this);
    Game1.multiplayer.Disconnect(Multiplayer.DisconnectType.ExitedToMainMenu);
    this.ResetGameStateOnTitleScreen();
    Game1.serverHost = (NetFarmerRoot) null;
    Game1.client = (Client) null;
    Game1.server = (IGameServer) null;
    TitleMenu.subMenu = (IClickableMenu) null;
    Game1.game1.refreshWindowSettings();
    if (!(Game1.activeClickableMenu is TitleMenu activeClickableMenu1))
      return;
    activeClickableMenu1.applyPreferences();
    IClickableMenu activeClickableMenu2 = Game1.activeClickableMenu;
    Viewport viewport = Game1.graphics.GraphicsDevice.Viewport;
    Microsoft.Xna.Framework.Rectangle bounds1 = viewport.Bounds;
    viewport = Game1.graphics.GraphicsDevice.Viewport;
    Microsoft.Xna.Framework.Rectangle bounds2 = viewport.Bounds;
    activeClickableMenu2.gameWindowSizeChanged(bounds1, bounds2);
  }

  public bool CanTakeScreenshots() => true;

  /// <summary>Get the absolute path to the folder containing screenshots.</summary>
  /// <param name="createIfMissing">Whether to create the folder if it doesn't exist already.</param>
  public string GetScreenshotFolder(bool createIfMissing = true)
  {
    return Program.GetLocalAppDataFolder("Screenshots", createIfMissing);
  }

  public bool CanBrowseScreenshots() => Directory.Exists(this.GetScreenshotFolder(false));

  public bool CanZoomScreenshots() => true;

  public void BrowseScreenshots()
  {
    string screenshotFolder = this.GetScreenshotFolder(false);
    if (!Directory.Exists(screenshotFolder))
      return;
    try
    {
      Process.Start(new ProcessStartInfo()
      {
        FileName = screenshotFolder,
        UseShellExecute = true,
        Verb = "open"
      });
    }
    catch (Exception ex)
    {
      Game1.log.Error("Failed to open screenshot folder.", ex);
    }
  }

  public string takeMapScreenshot(float? in_scale, string screenshot_name, Action onDone)
  {
    if (Game1.currentLocation == null)
      return (string) null;
    float scale = 1f;
    if (in_scale.HasValue)
      scale = in_scale.Value;
    string screenshot_name1 = screenshot_name;
    if (string.IsNullOrWhiteSpace(screenshot_name))
    {
      DateTime utcNow = DateTime.UtcNow;
      screenshot_name1 = SaveGame.FilterFileName(Game1.player.name.Value) + $"_{utcNow.Month}-{utcNow.Day}-{utcNow.Year}_{(int) utcNow.TimeOfDay.TotalMilliseconds}";
    }
    return this.takeMapScreenshot(Game1.currentLocation, scale, screenshot_name1, onDone);
  }

  private unsafe string takeMapScreenshot(
    GameLocation screenshotLocation,
    float scale,
    string screenshot_name,
    Action onDone)
  {
    string path2 = screenshot_name + ".png";
    int startX;
    int startY;
    int width1;
    int height1;
    Game1.GetScreenshotRegion(screenshotLocation, out startX, out startY, out width1, out height1);
    SKSurface skSurface = (SKSurface) null;
    bool flag1;
    int width2;
    int height2;
    do
    {
      flag1 = false;
      width2 = (int) ((double) width1 * (double) scale);
      height2 = (int) ((double) height1 * (double) scale);
      try
      {
        skSurface = SKSurface.Create(width2, height2, SKColorType.Rgb888x, SKAlphaType.Opaque);
      }
      catch (Exception ex)
      {
        Game1.log.Error("Map Screenshot: Error trying to create Bitmap.", ex);
        flag1 = true;
      }
      if (flag1)
        scale -= 0.25f;
      if ((double) scale <= 0.0)
        return (string) null;
    }
    while (flag1);
    int num1 = 2048 /*0x0800*/;
    int num2 = (int) ((double) num1 * (double) scale);
    xTile.Dimensions.Rectangle viewport = Game1.viewport;
    bool displayHud = Game1.displayHUD;
    this.takingMapScreenshot = true;
    float baseZoomLevel = Game1.options.baseZoomLevel;
    Game1.options.baseZoomLevel = 1f;
    RenderTarget2D lightmap = Game1._lightmap;
    Game1._lightmap = (RenderTarget2D) null;
    bool flag2 = false;
    try
    {
      Game1.allocateLightmap(num1, num1);
      int num3 = (int) Math.Ceiling((double) width2 / (double) num2);
      int num4 = (int) Math.Ceiling((double) height2 / (double) num2);
      for (int index1 = 0; index1 < num4; ++index1)
      {
        for (int index2 = 0; index2 < num3; ++index2)
        {
          int width3 = num2;
          int height3 = num2;
          int x = index2 * num2;
          int y = index1 * num2;
          if (x + num2 > width2)
            width3 += width2 - (x + num2);
          if (y + num2 > height2)
            height3 += height2 - (y + num2);
          if (height3 > 0 && width3 > 0)
          {
            Microsoft.Xna.Framework.Rectangle rectangle = new Microsoft.Xna.Framework.Rectangle(x, y, width3, height3);
            RenderTarget2D renderTarget2D = new RenderTarget2D(Game1.graphics.GraphicsDevice, num1, num1, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.DiscardContents);
            Game1.viewport = new xTile.Dimensions.Rectangle(index2 * num1 + startX, index1 * num1 + startY, num1, num1);
            this._draw(Game1.currentGameTime, renderTarget2D);
            RenderTarget2D renderTarget = new RenderTarget2D(Game1.graphics.GraphicsDevice, width3, height3, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.DiscardContents);
            this.GraphicsDevice.SetRenderTarget(renderTarget);
            Game1.spriteBatch.Begin(blendState: BlendState.Opaque, samplerState: SamplerState.PointClamp, depthStencilState: DepthStencilState.Default, rasterizerState: RasterizerState.CullNone);
            Color white = Color.White;
            Game1.spriteBatch.Draw((Texture2D) renderTarget2D, Vector2.Zero, new Microsoft.Xna.Framework.Rectangle?(renderTarget2D.Bounds), white, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 1f);
            Game1.spriteBatch.End();
            renderTarget2D.Dispose();
            this.GraphicsDevice.SetRenderTarget((RenderTarget2D) null);
            Color[] data = new Color[width3 * height3];
            renderTarget.GetData<Color>(data);
            SKBitmap bitmap = new SKBitmap(rectangle.Width, rectangle.Height, SKColorType.Rgb888x, SKAlphaType.Opaque);
            byte* numPtr1 = (byte*) bitmap.GetPixels().ToPointer();
            for (int index3 = 0; index3 < height3; ++index3)
            {
              for (int index4 = 0; index4 < width3; ++index4)
              {
                byte* numPtr2 = numPtr1;
                byte* numPtr3 = numPtr2 + 1;
                int r = (int) data[index4 + index3 * width3].R;
                *numPtr2 = (byte) r;
                byte* numPtr4 = numPtr3;
                byte* numPtr5 = numPtr4 + 1;
                int g = (int) data[index4 + index3 * width3].G;
                *numPtr4 = (byte) g;
                byte* numPtr6 = numPtr5;
                byte* numPtr7 = numPtr6 + 1;
                int b = (int) data[index4 + index3 * width3].B;
                *numPtr6 = (byte) b;
                byte* numPtr8 = numPtr7;
                numPtr1 = numPtr8 + 1;
                *numPtr8 = byte.MaxValue;
              }
            }
            SKPaint paint = new SKPaint();
            skSurface.Canvas.DrawBitmap(bitmap, SKRect.Create((float) rectangle.X, (float) rectangle.Y, (float) width3, (float) height3), paint);
            bitmap.Dispose();
            renderTarget.Dispose();
          }
        }
      }
      string path = Path.Combine(this.GetScreenshotFolder(), path2);
      skSurface.Snapshot().Encode(SKEncodedImageFormat.Png, 100).SaveTo((Stream) new FileStream(path, FileMode.OpenOrCreate));
      skSurface.Dispose();
    }
    catch (Exception ex)
    {
      Game1.log.Error("Map Screenshot: Error taking screenshot.", ex);
      this.GraphicsDevice.SetRenderTarget((RenderTarget2D) null);
      flag2 = true;
    }
    if (Game1._lightmap != null)
    {
      Game1._lightmap.Dispose();
      Game1._lightmap = (RenderTarget2D) null;
    }
    Game1._lightmap = lightmap;
    Game1.options.baseZoomLevel = baseZoomLevel;
    this.takingMapScreenshot = false;
    Game1.displayHUD = displayHud;
    Game1.viewport = viewport;
    return flag2 ? (string) null : path2;
  }

  private static void GetScreenshotRegion(
    GameLocation screenshotLocation,
    out int startX,
    out int startY,
    out int width,
    out int height)
  {
    startX = 0;
    startY = 0;
    width = screenshotLocation.map.DisplayWidth;
    height = screenshotLocation.map.DisplayHeight;
    try
    {
      string[] propertySplitBySpaces = screenshotLocation.GetMapPropertySplitBySpaces("ScreenshotRegion");
      if (propertySplitBySpaces.Length == 0)
        return;
      int num1;
      string error;
      int num2;
      int num3;
      int num4;
      if (!ArgUtility.TryGetInt(propertySplitBySpaces, 0, out num1, out error, "int topLeftX") || !ArgUtility.TryGetInt(propertySplitBySpaces, 1, out num2, out error, "int topLeftY") || !ArgUtility.TryGetInt(propertySplitBySpaces, 2, out num3, out error, "int bottomRightX") || !ArgUtility.TryGetInt(propertySplitBySpaces, 3, out num4, out error, "int bottomRightY"))
      {
        screenshotLocation.LogMapPropertyError("ScreenshotRegion", propertySplitBySpaces, error);
      }
      else
      {
        startX = num1 * 64 /*0x40*/;
        startY = num2 * 64 /*0x40*/;
        width = (num3 + 1) * 64 /*0x40*/ - startX;
        height = (num4 + 1) * 64 /*0x40*/ - startY;
      }
    }
    catch (Exception ex)
    {
      Game1.log.Error("GetScreenshotRegion failed with exception:", ex);
    }
  }

  public enum BundleType
  {
    Default,
    Remixed,
  }

  public enum MineChestType
  {
    Default,
    Remixed,
  }

  public delegate void afterFadeFunction();
}
