// Decompiled with JetBrains decompiler
// Type: StardewValley.Farmer
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Netcode;
using Netcode.Validation;
using StardewValley.Audio;
using StardewValley.BellsAndWhistles;
using StardewValley.Buffs;
using StardewValley.Buildings;
using StardewValley.Characters;
using StardewValley.Companions;
using StardewValley.Constants;
using StardewValley.Enchantments;
using StardewValley.Extensions;
using StardewValley.GameData.LocationContexts;
using StardewValley.GameData.Objects;
using StardewValley.GameData.Shirts;
using StardewValley.Inventories;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Minigames;
using StardewValley.Monsters;
using StardewValley.Network;
using StardewValley.Network.NetEvents;
using StardewValley.Objects;
using StardewValley.Objects.Trinkets;
using StardewValley.Pathfinding;
using StardewValley.Quests;
using StardewValley.SpecialOrders;
using StardewValley.Tools;
using StardewValley.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using xTile.Dimensions;

#nullable disable
namespace StardewValley;

public class Farmer : Character, IComparable
{
  public const int millisecondsPerSpeedUnit = 64 /*0x40*/;
  public const byte halt = 64 /*0x40*/;
  public const byte up = 1;
  public const byte right = 2;
  public const byte down = 4;
  public const byte left = 8;
  public const byte run = 16 /*0x10*/;
  public const byte release = 32 /*0x20*/;
  public const int farmingSkill = 0;
  public const int miningSkill = 3;
  public const int fishingSkill = 1;
  public const int foragingSkill = 2;
  public const int combatSkill = 4;
  public const int luckSkill = 5;
  public const float interpolationConstant = 0.5f;
  public const int runningSpeed = 5;
  public const int walkingSpeed = 2;
  public const int caveNothing = 0;
  public const int caveBats = 1;
  public const int caveMushrooms = 2;
  public const int millisecondsInvincibleAfterDamage = 1200;
  public const int millisecondsPerFlickerWhenInvincible = 50;
  public const int startingStamina = 270;
  public const int totalLevels = 35;
  public const int maxInventorySpace = 36;
  public const int hotbarSize = 12;
  public const int eyesOpen = 0;
  public const int eyesHalfShut = 4;
  public const int eyesClosed = 1;
  public const int eyesRight = 2;
  public const int eyesLeft = 3;
  public const int eyesWide = 5;
  public const int rancher = 0;
  public const int tiller = 1;
  public const int butcher = 2;
  public const int shepherd = 3;
  public const int artisan = 4;
  public const int agriculturist = 5;
  public const int fisher = 6;
  public const int trapper = 7;
  public const int angler = 8;
  public const int pirate = 9;
  /// <summary>The Mariner profession, which makes crab pots no longer produce junk items.</summary>
  /// <remarks>For legacy reasons, the Luremaster and Mariner profession constants are swapped (i.e. <see cref="F:StardewValley.Farmer.mariner" /> is Luremaster and <see cref="F:StardewValley.Farmer.baitmaster" /> is Mariner).</remarks>
  public const int baitmaster = 10;
  /// <summary>The Luremaster profession, which makes crab pots no longer require bait.</summary>
  /// <inheritdoc cref="F:StardewValley.Farmer.baitmaster" path="/remarks" />
  public const int mariner = 11;
  public const int forester = 12;
  public const int gatherer = 13;
  public const int lumberjack = 14;
  public const int tapper = 15;
  public const int botanist = 16 /*0x10*/;
  public const int tracker = 17;
  public const int miner = 18;
  public const int geologist = 19;
  public const int blacksmith = 20;
  public const int burrower = 21;
  public const int excavator = 22;
  public const int gemologist = 23;
  public const int fighter = 24;
  public const int scout = 25;
  public const int brute = 26;
  public const int defender = 27;
  public const int acrobat = 28;
  public const int desperado = 29;
  public static int MaximumTrinkets = 1;
  public readonly NetObjectList<Quest> questLog;
  public readonly NetIntHashSet professions;
  public readonly NetList<Point, NetPoint> newLevels;
  [XmlIgnore]
  public Queue<int> newLevelSparklingTexts;
  [XmlIgnore]
  public SparklingText sparklingText;
  public readonly NetArray<int, NetInt> experiencePoints;
  /// <summary>The backing field for <see cref="P:StardewValley.Farmer.Items" />.</summary>
  [XmlElement("items")]
  public readonly NetRef<Inventory> netItems;
  [XmlArrayItem("int")]
  public readonly NetStringHashSet dialogueQuestionsAnswered;
  [XmlElement("cookingRecipes")]
  public readonly NetStringDictionary<int, NetInt> cookingRecipes;
  [XmlElement("craftingRecipes")]
  public readonly NetStringDictionary<int, NetInt> craftingRecipes;
  [XmlElement("activeDialogueEvents")]
  public readonly NetStringDictionary<int, NetInt> activeDialogueEvents;
  [XmlElement("previousActiveDialogueEvents")]
  public readonly NetStringDictionary<int, NetInt> previousActiveDialogueEvents;
  /// <summary>The trigger actions which have been run for the player.</summary>
  public readonly NetStringHashSet triggerActionsRun;
  /// <summary>The event IDs which the player has seen.</summary>
  [XmlArrayItem("int")]
  public readonly NetStringHashSet eventsSeen;
  public readonly NetIntHashSet secretNotesSeen;
  public HashSet<string> songsHeard;
  public readonly NetIntHashSet achievements;
  [XmlArrayItem("int")]
  public readonly NetStringList specialItems;
  [XmlArrayItem("int")]
  public readonly NetStringList specialBigCraftables;
  /// <summary>The mail flags set on the player. This includes both actual mail letter IDs matching <c>Data/mail</c>, and non-mail flags used to track game state like <c>ccIsComplete</c> (community center complete).</summary>
  /// <remarks>See also <see cref="F:StardewValley.Farmer.mailForTomorrow" /> and <see cref="F:StardewValley.Farmer.mailbox" />.</remarks>
  public readonly NetStringHashSet mailReceived;
  /// <summary>The mail flags that will be added to the <see cref="F:StardewValley.Farmer.mailbox" /> tomorrow.</summary>
  public readonly NetStringHashSet mailForTomorrow;
  /// <summary>The mail IDs matching <c>Data/mail</c> in the player's mailbox, if any. Each time the player checks their mailbox, one letter from this set will be displayed and moved into <see cref="F:StardewValley.Farmer.mailReceived" />.</summary>
  public readonly NetStringList mailbox;
  /// <summary>The internal names of locations which the player has previously visited.</summary>
  /// <remarks>This contains the <see cref="P:StardewValley.GameLocation.Name" /> field, not <see cref="P:StardewValley.GameLocation.NameOrUniqueName" />. They're equivalent for most locations, but building interiors will use their common name (like <c>Barn</c> instead of <c>Barn{unique ID}</c> for barns).</remarks>
  public readonly NetStringHashSet locationsVisited;
  public readonly NetInt timeWentToBed;
  [XmlIgnore]
  public readonly NetList<Companion, NetRef<Companion>> companions;
  /// <summary>Whether the local player has moved today.</summary>
  [XmlIgnore]
  public bool hasMoved;
  /// <summary>Whether the local player has interacted with a statue of blessings today.</summary>
  [XmlIgnore]
  public bool hasBeenBlessedByStatueToday;
  /// <summary>Whether the player slept using an item bed in a place that doesn't have an actual bed furniture item.</summary>
  public readonly NetBool sleptInTemporaryBed;
  [XmlIgnore]
  public readonly NetBool requestingTimePause;
  public Stats stats;
  [XmlIgnore]
  public readonly NetRef<Inventory> personalShippingBin;
  [XmlIgnore]
  public IList<Item> displayedShippedItems;
  [XmlElement("biteChime")]
  public NetInt biteChime;
  [XmlIgnore]
  public float usernameDisplayTime;
  [XmlIgnore]
  protected NetRef<Item> _recoveredItem;
  public NetObjectList<Item> itemsLostLastDeath;
  public List<int> movementDirections;
  [XmlElement("farmName")]
  public readonly NetString farmName;
  [XmlElement("favoriteThing")]
  public readonly NetString favoriteThing;
  [XmlElement("horseName")]
  public readonly NetString horseName;
  public string slotName;
  public bool slotCanHost;
  [XmlIgnore]
  public readonly NetString tempFoodItemTextureName;
  [XmlIgnore]
  public readonly NetRectangle tempFoodItemSourceRect;
  [XmlIgnore]
  public bool hasReceivedToolUpgradeMessageYet;
  [XmlIgnore]
  public readonly BuffManager buffs;
  [XmlIgnore]
  public IList<OutgoingMessage> messageQueue;
  [XmlIgnore]
  public readonly NetLong uniqueMultiplayerID;
  [XmlElement("userID")]
  public readonly NetString userID;
  [XmlIgnore]
  public string previousLocationName;
  [XmlIgnore]
  public readonly NetString platformType;
  [XmlIgnore]
  public readonly NetString platformID;
  [XmlIgnore]
  public readonly NetBool hasMenuOpen;
  [XmlIgnore]
  public readonly Color DEFAULT_SHIRT_COLOR;
  public string defaultChatColor;
  /// <summary>Obsolete since 1.6. This is only kept to preserve data from old save files. Use <see cref="F:StardewValley.Farmer.whichPetType" /> instead.</summary>
  [XmlElement("catPerson")]
  public bool? obsolete_catPerson;
  /// <summary>Obsolete since 1.6. This is only kept to preserve data from old save files. Use <see cref="P:StardewValley.Farmer.canUnderstandDwarves" /> instead.</summary>
  [XmlElement("canUnderstandDwarves")]
  public bool? obsolete_canUnderstandDwarves;
  /// <summary>Obsolete since 1.6. This is only kept to preserve data from old save files. Use <see cref="P:StardewValley.Farmer.hasClubCard" /> instead.</summary>
  [XmlElement("hasClubCard")]
  public bool? obsolete_hasClubCard;
  /// <summary>Obsolete since 1.6. This is only kept to preserve data from old save files. Use <see cref="P:StardewValley.Farmer.hasDarkTalisman" /> instead.</summary>
  [XmlElement("hasDarkTalisman")]
  public bool? obsolete_hasDarkTalisman;
  /// <summary>Obsolete since 1.6. This is only kept to preserve data from old save files. Use <see cref="P:StardewValley.Farmer.hasMagicInk" /> instead.</summary>
  [XmlElement("hasMagicInk")]
  public bool? obsolete_hasMagicInk;
  /// <summary>Obsolete since 1.6. This is only kept to preserve data from old save files. Use <see cref="P:StardewValley.Farmer.hasMagnifyingGlass" /> instead.</summary>
  [XmlElement("hasMagnifyingGlass")]
  public bool? obsolete_hasMagnifyingGlass;
  /// <summary>Obsolete since 1.6. This is only kept to preserve data from old save files. Use <see cref="P:StardewValley.Farmer.hasRustyKey" /> instead.</summary>
  [XmlElement("hasRustyKey")]
  public bool? obsolete_hasRustyKey;
  /// <summary>Obsolete since 1.6. This is only kept to preserve data from old save files. Use <see cref="P:StardewValley.Farmer.hasSkullKey" /> instead.</summary>
  [XmlElement("hasSkullKey")]
  public bool? obsolete_hasSkullKey;
  /// <summary>Obsolete since 1.6. This is only kept to preserve data from old save files. Use <see cref="P:StardewValley.Farmer.hasSpecialCharm" /> instead.</summary>
  [XmlElement("hasSpecialCharm")]
  public bool? obsolete_hasSpecialCharm;
  /// <summary>Obsolete since 1.6. This is only kept to preserve data from old save files. Use <see cref="P:StardewValley.Farmer.HasTownKey" /> instead.</summary>
  [XmlElement("HasTownKey")]
  public bool? obsolete_hasTownKey;
  /// <summary>Obsolete since 1.6. This is only kept to preserve data from old save files. Use <see cref="P:StardewValley.Farmer.hasUnlockedSkullDoor" /> instead.</summary>
  [XmlElement("hasUnlockedSkullDoor")]
  public bool? obsolete_hasUnlockedSkullDoor;
  /// <summary>Obsolete since 1.3. This is only kept to preserve data from old save files. Use <see cref="F:StardewValley.Farmer.friendshipData" /> for NPC friendships or <see cref="F:StardewValley.FarmerTeam.friendshipData" /> for farmhands instead.</summary>
  [XmlElement("friendships")]
  public SerializableDictionary<string, int[]> obsolete_friendships;
  /// <summary>Obsolete since 1.3. This is only kept to preserve data from old save files. Use <see cref="M:StardewValley.Farmer.GetDaysMarried" /> instead.</summary>
  [XmlElement("daysMarried")]
  public int? obsolete_daysMarried;
  /// <summary>The preferred pet type, matching an ID in <c>Data/Pets</c>. The vanilla pet types are <see cref="F:StardewValley.Characters.Pet.type_cat" /> and <see cref="F:StardewValley.Characters.Pet.type_dog" />.</summary>
  public string whichPetType;
  /// <summary>The selected breed ID in <c>Data/Pets</c> for the <see cref="F:StardewValley.Farmer.whichPetType" />.</summary>
  public string whichPetBreed;
  [XmlIgnore]
  public bool isAnimatingMount;
  [XmlElement("acceptedDailyQuest")]
  public readonly NetBool acceptedDailyQuest;
  [XmlIgnore]
  public Item mostRecentlyGrabbedItem;
  [XmlIgnore]
  public Item itemToEat;
  [XmlElement("farmerRenderer")]
  private readonly NetRef<FarmerRenderer> farmerRenderer;
  [XmlIgnore]
  public readonly NetInt toolPower;
  [XmlIgnore]
  public readonly NetInt toolHold;
  public Vector2 mostRecentBed;
  public static Dictionary<int, string> hairStyleMetadataFile = (Dictionary<int, string>) null;
  public static List<int> allHairStyleIndices = (List<int>) null;
  [XmlIgnore]
  public static Dictionary<int, HairStyleMetadata> hairStyleMetadata = new Dictionary<int, HairStyleMetadata>();
  [XmlElement("emoteFavorites")]
  public readonly List<string> emoteFavorites;
  [XmlElement("performedEmotes")]
  public readonly SerializableDictionary<string, bool> performedEmotes;
  /// <summary>If set, the unqualified item ID of the <see cref="F:StardewValley.ItemRegistry.type_shirt" /> item to show this player wearing instead of the equipped <see cref="F:StardewValley.Farmer.shirtItem" />.</summary>
  [XmlElement("shirt")]
  public readonly NetString shirt;
  [XmlElement("hair")]
  public readonly NetInt hair;
  [XmlElement("skin")]
  public readonly NetInt skin;
  [XmlElement("shoes")]
  public readonly NetString shoes;
  [XmlElement("accessory")]
  public readonly NetInt accessory;
  [XmlElement("facialHair")]
  public readonly NetInt facialHair;
  /// <summary>If set, the unqualified item ID of the <see cref="F:StardewValley.ItemRegistry.type_pants" /> item to show this player wearing instead of the equipped <see cref="F:StardewValley.Farmer.pantsItem" />.</summary>
  [XmlElement("pants")]
  public readonly NetString pants;
  [XmlIgnore]
  public int currentEyes;
  [XmlIgnore]
  public int blinkTimer;
  [XmlIgnore]
  public readonly NetInt netFestivalScore;
  /// <summary>The last date that we submitted the Calico Egg Rating to Gil.</summary>
  public readonly NetRef<WorldDate> lastGotPrizeFromGil;
  /// <summary>The last date that we accepted a Desert Festival fishing quest.</summary>
  public readonly NetRef<WorldDate> lastDesertFestivalFishingQuest;
  [XmlIgnore]
  public float temporarySpeedBuff;
  [XmlElement("hairstyleColor")]
  public readonly NetColor hairstyleColor;
  [XmlIgnore]
  public NetBool prismaticHair;
  /// <summary>The color to apply when rendering <see cref="F:StardewValley.Farmer.pants" />. Most code should use <see cref="M:StardewValley.Farmer.GetPantsColor" /> instead.</summary>
  [XmlElement("pantsColor")]
  public readonly NetColor pantsColor;
  [XmlElement("newEyeColor")]
  public readonly NetColor newEyeColor;
  [XmlElement("hat")]
  public readonly NetRef<Hat> hat;
  [XmlElement("boots")]
  public readonly NetRef<Boots> boots;
  [XmlElement("leftRing")]
  public readonly NetRef<Ring> leftRing;
  [XmlElement("rightRing")]
  public readonly NetRef<Ring> rightRing;
  [XmlElement("shirtItem")]
  public readonly NetRef<Clothing> shirtItem;
  [XmlElement("pantsItem")]
  public readonly NetRef<Clothing> pantsItem;
  [XmlIgnore]
  public readonly NetDancePartner dancePartner;
  [XmlIgnore]
  public bool ridingMineElevator;
  [XmlIgnore]
  public readonly NetBool exhausted;
  [XmlElement("divorceTonight")]
  public readonly NetBool divorceTonight;
  [XmlElement("changeWalletTypeTonight")]
  public readonly NetBool changeWalletTypeTonight;
  [XmlIgnore]
  public AnimatedSprite.endOfAnimationBehavior toolOverrideFunction;
  [XmlIgnore]
  public NetBool onBridge;
  [XmlIgnore]
  public SuspensionBridge bridge;
  private readonly NetInt netDeepestMineLevel;
  [XmlElement("currentToolIndex")]
  private readonly NetInt currentToolIndex;
  [XmlIgnore]
  private readonly NetRef<Item> temporaryItem;
  [XmlIgnore]
  private readonly NetRef<Item> cursorSlotItem;
  [XmlIgnore]
  public readonly NetBool netItemStowed;
  protected bool _itemStowed;
  public string gameVersion;
  public string gameVersionLabel;
  [XmlIgnore]
  public bool isFakeEventActor;
  [XmlElement("bibberstyke")]
  public readonly NetInt bobberStyle;
  public bool usingRandomizedBobber;
  [XmlElement("caveChoice")]
  public readonly NetInt caveChoice;
  [XmlElement("farmingLevel")]
  public readonly NetInt farmingLevel;
  [XmlElement("miningLevel")]
  public readonly NetInt miningLevel;
  [XmlElement("combatLevel")]
  public readonly NetInt combatLevel;
  [XmlElement("foragingLevel")]
  public readonly NetInt foragingLevel;
  [XmlElement("fishingLevel")]
  public readonly NetInt fishingLevel;
  [XmlElement("luckLevel")]
  public readonly NetInt luckLevel;
  [XmlElement("maxStamina")]
  public readonly NetInt maxStamina;
  [XmlElement("maxItems")]
  public readonly NetInt maxItems;
  [XmlElement("lastSeenMovieWeek")]
  public readonly NetInt lastSeenMovieWeek;
  [XmlIgnore]
  public readonly NetString viewingLocation;
  private readonly NetFloat netStamina;
  [XmlIgnore]
  public bool ignoreItemConsumptionThisFrame;
  [XmlIgnore]
  [NotNetField]
  public NetRoot<FarmerTeam> teamRoot;
  public int clubCoins;
  public int trashCanLevel;
  private NetLong netMillisecondsPlayed;
  [XmlElement("toolBeingUpgraded")]
  public readonly NetRef<Tool> toolBeingUpgraded;
  [XmlElement("daysLeftForToolUpgrade")]
  public readonly NetInt daysLeftForToolUpgrade;
  [XmlElement("houseUpgradeLevel")]
  public readonly NetInt houseUpgradeLevel;
  [XmlElement("daysUntilHouseUpgrade")]
  public readonly NetInt daysUntilHouseUpgrade;
  public bool showChestColorPicker;
  public bool hasWateringCanEnchantment;
  [XmlIgnore]
  public List<BaseEnchantment> enchantments;
  public readonly int BaseMagneticRadius;
  public int temporaryInvincibilityTimer;
  public int currentTemporaryInvincibilityDuration;
  [XmlIgnore]
  public float rotation;
  private int craftingTime;
  private int raftPuddleCounter;
  private int raftBobCounter;
  public int health;
  public int maxHealth;
  private readonly NetInt netTimesReachedMineBottom;
  public float difficultyModifier;
  [XmlIgnore]
  public Vector2 jitter;
  [XmlIgnore]
  public Vector2 lastPosition;
  [XmlIgnore]
  public Vector2 lastGrabTile;
  [XmlIgnore]
  public float jitterStrength;
  [XmlIgnore]
  public float xOffset;
  /// <summary>The net-synchronized backing field for <see cref="P:StardewValley.Farmer.Gender" />.</summary>
  [XmlElement("gender")]
  public readonly NetEnum<Gender> netGender;
  [XmlIgnore]
  public bool canMove;
  [XmlIgnore]
  public bool running;
  [XmlIgnore]
  public bool ignoreCollisions;
  [XmlIgnore]
  public readonly NetBool usingTool;
  [XmlIgnore]
  public bool isEating;
  [XmlIgnore]
  public readonly NetBool isInBed;
  [XmlIgnore]
  public bool forceTimePass;
  [XmlIgnore]
  public bool isRafting;
  [XmlIgnore]
  public bool usingSlingshot;
  [XmlIgnore]
  public readonly NetBool bathingClothes;
  [XmlIgnore]
  public bool canOnlyWalk;
  [XmlIgnore]
  public bool temporarilyInvincible;
  [XmlIgnore]
  public bool flashDuringThisTemporaryInvincibility;
  private readonly NetBool netCanReleaseTool;
  [XmlIgnore]
  public bool isCrafting;
  [XmlIgnore]
  public bool isEmoteAnimating;
  [XmlIgnore]
  public bool passedOut;
  [XmlIgnore]
  protected int _emoteGracePeriod;
  [XmlIgnore]
  private BoundingBoxGroup temporaryPassableTiles;
  [XmlIgnore]
  public readonly NetBool hidden;
  [XmlElement("basicShipped")]
  public readonly NetStringDictionary<int, NetInt> basicShipped;
  [XmlElement("mineralsFound")]
  public readonly NetStringDictionary<int, NetInt> mineralsFound;
  [XmlElement("recipesCooked")]
  public readonly NetStringDictionary<int, NetInt> recipesCooked;
  [XmlElement("fishCaught")]
  public readonly NetStringIntArrayDictionary fishCaught;
  [XmlElement("archaeologyFound")]
  public readonly NetStringIntArrayDictionary archaeologyFound;
  [XmlElement("callsReceived")]
  public readonly NetStringDictionary<int, NetInt> callsReceived;
  public SerializableDictionary<string, SerializableDictionary<string, int>> giftedItems;
  [XmlElement("tailoredItems")]
  public readonly NetStringDictionary<int, NetInt> tailoredItems;
  [XmlElement("friendshipData")]
  public readonly NetStringDictionary<Friendship, NetRef<Friendship>> friendshipData;
  [XmlIgnore]
  public NetString locationBeforeForcedEvent;
  [XmlIgnore]
  public Vector2 positionBeforeEvent;
  [XmlIgnore]
  public int orientationBeforeEvent;
  [XmlIgnore]
  public int swimTimer;
  [XmlIgnore]
  public int regenTimer;
  [XmlIgnore]
  public int timerSinceLastMovement;
  [XmlIgnore]
  public int noMovementPause;
  [XmlIgnore]
  public int freezePause;
  [XmlIgnore]
  public float yOffset;
  /// <summary>The backing field for <see cref="P:StardewValley.Farmer.spouse" />.</summary>
  protected readonly NetString netSpouse;
  public string dateStringForSaveGame;
  public int? dayOfMonthForSaveGame;
  public int? seasonForSaveGame;
  public int? yearForSaveGame;
  [XmlIgnore]
  public Vector2 armOffset;
  [XmlIgnore]
  public readonly NetRef<Horse> netMount;
  [XmlIgnore]
  public ISittable sittingFurniture;
  [XmlIgnore]
  public NetBool isSitting;
  [XmlIgnore]
  public NetVector2 mapChairSitPosition;
  [XmlIgnore]
  public NetBool hasCompletedAllMonsterSlayerQuests;
  [XmlIgnore]
  public bool isStopSitting;
  [XmlIgnore]
  protected bool _wasSitting;
  [XmlIgnore]
  public Vector2 lerpStartPosition;
  [XmlIgnore]
  public Vector2 lerpEndPosition;
  [XmlIgnore]
  public float lerpPosition;
  [XmlIgnore]
  public float lerpDuration;
  [XmlIgnore]
  protected Item _lastSelectedItem;
  [XmlIgnore]
  protected internal Tool _lastEquippedTool;
  [XmlElement("qiGems")]
  public NetIntDelta netQiGems;
  [XmlElement("JOTPKProgress")]
  public NetRef<AbigailGame.JOTPKProgress> jotpkProgress;
  [XmlIgnore]
  public NetBool hasUsedDailyRevive;
  [XmlElement("trinketItem")]
  public readonly NetList<Trinket, NetRef<Trinket>> trinketItems;
  private readonly NetEvent0 fireToolEvent;
  private readonly NetEvent0 beginUsingToolEvent;
  private readonly NetEvent0 endUsingToolEvent;
  private readonly NetEvent0 sickAnimationEvent;
  private readonly NetEvent0 passOutEvent;
  private readonly NetEvent0 haltAnimationEvent;
  private readonly NetEvent1Field<Object, NetRef<Object>> drinkAnimationEvent;
  private readonly NetEvent1Field<Object, NetRef<Object>> eatAnimationEvent;
  private readonly NetEvent1Field<string, NetString> doEmoteEvent;
  private readonly NetEvent1Field<long, NetLong> kissFarmerEvent;
  private readonly NetEvent1Field<float, NetFloat> synchronizedJumpEvent;
  public readonly NetEvent1Field<string, NetString> renovateEvent;
  [XmlElement("chestConsumedLevels")]
  public readonly NetIntDictionary<bool, NetBool> chestConsumedMineLevels;
  public int saveTime;
  [XmlIgnore]
  public float drawLayerDisambiguator;
  [XmlElement("isCustomized")]
  public readonly NetBool isCustomized;
  [XmlElement("homeLocation")]
  public readonly NetString homeLocation;
  [XmlElement("lastSleepLocation")]
  public readonly NetString lastSleepLocation;
  [XmlElement("lastSleepPoint")]
  public readonly NetPoint lastSleepPoint;
  [XmlElement("disconnectDay")]
  public readonly NetInt disconnectDay;
  [XmlElement("disconnectLocation")]
  public readonly NetString disconnectLocation;
  [XmlElement("disconnectPosition")]
  public readonly NetVector2 disconnectPosition;
  public static readonly Farmer.EmoteType[] EMOTES;
  [XmlIgnore]
  public int emoteFacingDirection;
  private int toolPitchAccumulator;
  [XmlIgnore]
  public readonly NetInt toolHoldStartTime;
  private int charactercollisionTimer;
  private NPC collisionNPC;
  public float movementMultiplier;

  public bool hasVisibleQuests
  {
    get
    {
      foreach (SpecialOrder specialOrder in this.team.specialOrders)
      {
        if (!specialOrder.IsHidden())
          return true;
      }
      foreach (Quest quest in (NetList<Quest, NetRef<Quest>>) this.questLog)
      {
        if (quest != null && !quest.IsHidden())
          return true;
      }
      return false;
    }
  }

  public Item recoveredItem
  {
    get => this._recoveredItem.Value;
    set => this._recoveredItem.Value = value;
  }

  /// <summary>Obsolete since 1.6. This is only kept to preserve data from old save files. Use <see cref="P:StardewValley.Farmer.Gender" /> or <see cref="P:StardewValley.Farmer.IsMale" /> instead.</summary>
  [XmlElement("isMale")]
  public bool? obsolete_isMale
  {
    get => new bool?();
    set
    {
      if (!value.HasValue)
        return;
      this.Gender = value.Value ? Gender.Male : Gender.Female;
    }
  }

  /// <summary>Whether the player's preferred pet type is <see cref="F:StardewValley.Characters.Pet.type_cat" />.</summary>
  /// <remarks>See also <see cref="F:StardewValley.Farmer.whichPetType" />.</remarks>
  [XmlIgnore]
  public bool catPerson => this.whichPetType == "Cat";

  [XmlIgnore]
  public int festivalScore
  {
    get => this.netFestivalScore.Value;
    set
    {
      if (this.team?.festivalScoreStatus != null)
        this.team.festivalScoreStatus.UpdateState(this.festivalScore.ToString() ?? "");
      this.netFestivalScore.Value = value;
    }
  }

  public int deepestMineLevel
  {
    get => this.netDeepestMineLevel.Value;
    set => this.netDeepestMineLevel.Value = value;
  }

  public float stamina
  {
    get => this.netStamina.Value;
    set => this.netStamina.Value = value;
  }

  [XmlIgnore]
  public FarmerTeam team
  {
    get => Game1.player != null && this != Game1.player ? Game1.player.team : this.teamRoot.Value;
  }

  public uint totalMoneyEarned
  {
    get => (uint) this.teamRoot.Value.totalMoneyEarned.Value;
    set
    {
      if (this.teamRoot.Value.totalMoneyEarned.Value != 0)
      {
        if (value >= 15000U && this.teamRoot.Value.totalMoneyEarned.Value < 15000)
          Game1.multiplayer.globalChatInfoMessage("Earned15k", this.farmName.Value);
        if (value >= 50000U && this.teamRoot.Value.totalMoneyEarned.Value < 50000)
          Game1.multiplayer.globalChatInfoMessage("Earned50k", this.farmName.Value);
        if (value >= 250000U && this.teamRoot.Value.totalMoneyEarned.Value < 250000)
          Game1.multiplayer.globalChatInfoMessage("Earned250k", this.farmName.Value);
        if (value >= 1000000U && this.teamRoot.Value.totalMoneyEarned.Value < 1000000)
          Game1.multiplayer.globalChatInfoMessage("Earned1m", this.farmName.Value);
        if (value >= 10000000U && this.teamRoot.Value.totalMoneyEarned.Value < 10000000)
          Game1.multiplayer.globalChatInfoMessage("Earned10m", this.farmName.Value);
        if (value >= 100000000U && this.teamRoot.Value.totalMoneyEarned.Value < 100000000)
          Game1.multiplayer.globalChatInfoMessage("Earned100m", this.farmName.Value);
      }
      this.teamRoot.Value.totalMoneyEarned.Value = (int) value;
    }
  }

  public ulong millisecondsPlayed
  {
    get => (ulong) this.netMillisecondsPlayed.Value;
    set => this.netMillisecondsPlayed.Value = (long) value;
  }

  /// <summary>Whether <strong>any player</strong> has found the Dwarvish Translation Guide that allows speaking to dwarves.</summary>
  [XmlIgnore]
  public bool canUnderstandDwarves
  {
    get => Game1.MasterPlayer.mailReceived.Contains("HasDwarvishTranslationGuide");
    set
    {
      Game1.player.team.RequestSetMail(PlayerActionTarget.Host, "HasDwarvishTranslationGuide", MailType.Received, value);
    }
  }

  /// <summary>Whether this player has unlocked access to the casino club.</summary>
  [XmlIgnore]
  public bool hasClubCard
  {
    get => this.mailReceived.Contains("HasClubCard");
    set => this.mailReceived.Toggle<string>("HasClubCard", value);
  }

  /// <summary>Whether this player has found the dark talisman, which unblocks the railroad's northeast path.</summary>
  [XmlIgnore]
  public bool hasDarkTalisman
  {
    get => this.mailReceived.Contains("HasDarkTalisman");
    set => this.mailReceived.Toggle<string>("HasDarkTalisman", value);
  }

  /// <summary>Whether this player has found the magic ink which allows magical building construction by the Wizard.</summary>
  [XmlIgnore]
  public bool hasMagicInk
  {
    get => this.mailReceived.Contains("HasMagicInk");
    set => this.mailReceived.Toggle<string>("HasMagicInk", value);
  }

  /// <summary>Whether this player has found the magnifying glass which allows finding secret notes.</summary>
  [XmlIgnore]
  public bool hasMagnifyingGlass
  {
    get => this.mailReceived.Contains("HasMagnifyingGlass");
    set => this.mailReceived.Toggle<string>("HasMagnifyingGlass", value);
  }

  /// <summary>Whether <strong>any player</strong> has found the Rusty Key which unlocks the sewers.</summary>
  [XmlIgnore]
  public bool hasRustyKey
  {
    get => Game1.MasterPlayer.mailReceived.Contains("HasRustyKey");
    set
    {
      Game1.player.team.RequestSetMail(PlayerActionTarget.Host, "HasRustyKey", MailType.Received, value);
    }
  }

  /// <summary>Whether <strong>any player</strong> has found the Skull Key which unlocks the skull caverns.</summary>
  [XmlIgnore]
  public bool hasSkullKey
  {
    get => Game1.MasterPlayer.mailReceived.Contains("HasSkullKey");
    set
    {
      Game1.player.team.RequestSetMail(PlayerActionTarget.Host, "HasSkullKey", MailType.Received, value);
    }
  }

  /// <summary>Whether this player has the Special Charm which increases daily luck.</summary>
  [XmlIgnore]
  public bool hasSpecialCharm
  {
    get => this.mailReceived.Contains("HasSpecialCharm");
    set => this.mailReceived.Toggle<string>("HasSpecialCharm", value);
  }

  /// <summary>Whether this player has unlocked the 'Key to the Town' item which lets them enter all town buildings.</summary>
  [XmlIgnore]
  public bool HasTownKey
  {
    get => this.mailReceived.Contains(nameof (HasTownKey));
    set => this.mailReceived.Toggle<string>(nameof (HasTownKey), value);
  }

  /// <summary>Whether the player has unlocked the door to the skull caverns using <see cref="P:StardewValley.Farmer.hasSkullKey" />.</summary>
  [XmlIgnore]
  public bool hasUnlockedSkullDoor
  {
    get => this.mailReceived.Contains("HasUnlockedSkullDoor");
    set => this.mailReceived.Toggle<string>("HasUnlockedSkullDoor", value);
  }

  [XmlIgnore]
  public bool hasPendingCompletedQuests
  {
    get
    {
      foreach (SpecialOrder specialOrder in this.team.specialOrders)
      {
        if (specialOrder.participants.ContainsKey(this.UniqueMultiplayerID) && specialOrder.ShouldDisplayAsComplete())
          return true;
      }
      foreach (Quest quest in (NetList<Quest, NetRef<Quest>>) this.questLog)
      {
        if (!quest.IsHidden() && quest.ShouldDisplayAsComplete() && !quest.destroy.Value)
          return true;
      }
      return false;
    }
  }

  [XmlElement("useSeparateWallets")]
  public bool useSeparateWallets
  {
    get => this.teamRoot.Value.useSeparateWallets.Value;
    set => this.teamRoot.Value.useSeparateWallets.Value = value;
  }

  [XmlElement("theaterBuildDate")]
  public long theaterBuildDate
  {
    get => this.teamRoot.Value.theaterBuildDate.Value;
    set => this.teamRoot.Value.theaterBuildDate.Value = value;
  }

  public int timesReachedMineBottom
  {
    get => this.netTimesReachedMineBottom.Value;
    set => this.netTimesReachedMineBottom.Value = value;
  }

  [XmlIgnore]
  public bool canReleaseTool
  {
    get => this.netCanReleaseTool.Value;
    set => this.netCanReleaseTool.Value = value;
  }

  /// <summary>The player's NPC spouse or roommate.</summary>
  [XmlElement("spouse")]
  public string spouse
  {
    get => !string.IsNullOrEmpty(this.netSpouse.Value) ? this.netSpouse.Value : (string) null;
    set
    {
      if (value == null)
        this.netSpouse.Value = "";
      else
        this.netSpouse.Value = value;
    }
  }

  [XmlIgnore]
  public bool isUnclaimedFarmhand => !this.IsMainPlayer && !this.isCustomized.Value;

  [XmlIgnore]
  public Horse mount
  {
    get => this.netMount.Value;
    set => this.setMount(value);
  }

  [XmlIgnore]
  public int MaxItems
  {
    get => this.maxItems.Value;
    set => this.maxItems.Value = value;
  }

  [XmlIgnore]
  public int Level
  {
    get
    {
      return (this.farmingLevel.Value + this.fishingLevel.Value + this.foragingLevel.Value + this.combatLevel.Value + this.miningLevel.Value + this.luckLevel.Value) / 2;
    }
  }

  [XmlIgnore]
  public int FarmingLevel => Math.Max(this.farmingLevel.Value + this.buffs.FarmingLevel, 0);

  [XmlIgnore]
  public int MiningLevel => Math.Max(this.miningLevel.Value + this.buffs.MiningLevel, 0);

  [XmlIgnore]
  public int CombatLevel => Math.Max(this.combatLevel.Value + this.buffs.CombatLevel, 0);

  [XmlIgnore]
  public int ForagingLevel => Math.Max(this.foragingLevel.Value + this.buffs.ForagingLevel, 0);

  [XmlIgnore]
  public int FishingLevel => Math.Max(this.fishingLevel.Value + this.buffs.FishingLevel, 0);

  [XmlIgnore]
  public int LuckLevel => Math.Max(this.luckLevel.Value + this.buffs.LuckLevel, 0);

  [XmlIgnore]
  public double DailyLuck
  {
    get
    {
      return Math.Min(Math.Max(this.team.sharedDailyLuck.Value + (this.hasSpecialCharm ? 0.02500000037252903 : 0.0), -0.20000000298023224), 0.20000000298023224);
    }
  }

  [XmlIgnore]
  public int HouseUpgradeLevel
  {
    get => this.houseUpgradeLevel.Value;
    set => this.houseUpgradeLevel.Value = value;
  }

  [XmlIgnore]
  public BoundingBoxGroup TemporaryPassableTiles
  {
    get => this.temporaryPassableTiles;
    set => this.temporaryPassableTiles = value;
  }

  [XmlIgnore]
  public Inventory Items => this.netItems.Value;

  [XmlIgnore]
  public int MagneticRadius => Math.Max(this.BaseMagneticRadius + this.buffs.MagneticRadius, 0);

  [XmlIgnore]
  public Item ActiveItem
  {
    get
    {
      if (this.TemporaryItem != null)
        return this.TemporaryItem;
      if (this._itemStowed)
        return (Item) null;
      return this.currentToolIndex.Value < this.Items.Count && this.Items[this.currentToolIndex.Value] != null ? this.Items[this.currentToolIndex.Value] : (Item) null;
    }
    set
    {
      this.netItemStowed.Set(false);
      if (value == null)
        this.removeItemFromInventory(this.ActiveItem);
      else
        this.addItemToInventory(value, this.CurrentToolIndex);
    }
  }

  [XmlIgnore]
  public Object ActiveObject
  {
    get => this.ActiveItem as Object;
    set => this.ActiveItem = (Item) value;
  }

  /// <summary>The player's gender identity.</summary>
  [XmlIgnore]
  public override Gender Gender
  {
    get => this.netGender.Value;
    set => this.netGender.Value = value;
  }

  [XmlIgnore]
  public bool IsMale => this.netGender.Value == Gender.Male;

  [XmlIgnore]
  public ISet<string> DialogueQuestionsAnswered => (ISet<string>) this.dialogueQuestionsAnswered;

  [XmlIgnore]
  public bool CanMove
  {
    get => this.canMove;
    set => this.canMove = value;
  }

  [XmlIgnore]
  public bool UsingTool
  {
    get => this.usingTool.Value;
    set => this.usingTool.Set(value);
  }

  [XmlIgnore]
  public Tool CurrentTool
  {
    get => this.CurrentItem as Tool;
    set
    {
      while (this.CurrentToolIndex >= this.Items.Count)
        this.Items.Add((Item) null);
      this.Items[this.CurrentToolIndex] = (Item) value;
    }
  }

  [XmlIgnore]
  public Item TemporaryItem
  {
    get => this.temporaryItem.Value;
    set => this.temporaryItem.Value = value;
  }

  public Item CursorSlotItem
  {
    get => this.cursorSlotItem.Value;
    set => this.cursorSlotItem.Value = value;
  }

  [XmlIgnore]
  public Item CurrentItem
  {
    get
    {
      if (this.TemporaryItem != null)
        return this.TemporaryItem;
      if (this._itemStowed)
        return (Item) null;
      return this.currentToolIndex.Value >= this.Items.Count ? (Item) null : this.Items[this.currentToolIndex.Value];
    }
  }

  [XmlIgnore]
  public int CurrentToolIndex
  {
    get => this.currentToolIndex.Value;
    set
    {
      this.netItemStowed.Set(false);
      if (this.currentToolIndex.Value >= 0 && this.CurrentItem != null && value != this.currentToolIndex.Value)
        this.CurrentItem.actionWhenStopBeingHeld(this);
      this.currentToolIndex.Set(value);
    }
  }

  [XmlIgnore]
  public float Stamina
  {
    get => this.stamina;
    set
    {
      if (this.hasBuff("statue_of_blessings_2") && (double) value < (double) this.stamina)
        return;
      this.stamina = Math.Min((float) this.MaxStamina, Math.Max(value, -16f));
    }
  }

  [XmlIgnore]
  public int MaxStamina => Math.Max(this.maxStamina.Value + this.buffs.MaxStamina, 0);

  [XmlIgnore]
  public int Attack => this.buffs.Attack;

  [XmlIgnore]
  public int Immunity => this.buffs.Immunity;

  [XmlIgnore]
  public override float addedSpeed
  {
    get
    {
      return (float) ((double) this.buffs.Speed + (this.stats.Get("Book_Speed") <= 0U || this.isRidingHorse() ? 0.0 : 0.25) + (this.stats.Get("Book_Speed2") <= 0U || this.isRidingHorse() ? 0.0 : 0.25));
    }
    [Obsolete("Player speed can't be changed directly. You can add a speed buff via applyBuff instead (and optionally mark it invisible).")] set
    {
    }
  }

  public long UniqueMultiplayerID
  {
    get => this.uniqueMultiplayerID.Value;
    set => this.uniqueMultiplayerID.Value = value;
  }

  /// <summary>Whether this is the farmer controlled by the local player, <strong>or</strong> the main farmer in an event being viewed by the local player (even if that farmer instance is a different player).</summary>
  [XmlIgnore]
  public bool IsLocalPlayer
  {
    get
    {
      if (this.UniqueMultiplayerID == Game1.player.UniqueMultiplayerID)
        return true;
      return Game1.CurrentEvent != null && Game1.CurrentEvent.farmer == this;
    }
  }

  [XmlIgnore]
  public bool IsMainPlayer
  {
    get
    {
      if ((NetFieldBase<Farmer, NetRef<Farmer>>) Game1.serverHost == (NetRef<Farmer>) null && this.IsLocalPlayer)
        return true;
      return (NetFieldBase<Farmer, NetRef<Farmer>>) Game1.serverHost != (NetRef<Farmer>) null && this.UniqueMultiplayerID == Game1.serverHost.Value.UniqueMultiplayerID;
    }
  }

  [XmlIgnore]
  public bool IsDedicatedPlayer
  {
    get
    {
      return Game1.HasDedicatedHost && (NetFieldBase<Farmer, NetRef<Farmer>>) Game1.serverHost != (NetRef<Farmer>) null && this.UniqueMultiplayerID == Game1.serverHost.Value.UniqueMultiplayerID;
    }
  }

  [XmlIgnore]
  public override AnimatedSprite Sprite
  {
    get => base.Sprite;
    set => base.Sprite = value;
  }

  [XmlIgnore]
  public FarmerSprite FarmerSprite
  {
    get => (FarmerSprite) this.Sprite;
    set => this.Sprite = (AnimatedSprite) value;
  }

  [XmlIgnore]
  public FarmerRenderer FarmerRenderer
  {
    get => this.farmerRenderer.Value;
    set => this.farmerRenderer.Set(value);
  }

  [XmlElement("money")]
  public int _money
  {
    get => this.teamRoot.Value.GetMoney(this).Value;
    set => this.teamRoot.Value.GetMoney(this).Value = value;
  }

  [XmlIgnore]
  public int QiGems
  {
    get => this.netQiGems.Value;
    set => this.netQiGems.Value = value;
  }

  [XmlIgnore]
  public int Money
  {
    get => this._money;
    set
    {
      if (Game1.player != this)
        throw new Exception("Cannot change another farmer's money. Use Game1.player.team.SetIndividualMoney");
      int money = this._money;
      this._money = value;
      if (value <= money)
        return;
      uint num = (uint) (value - money);
      this.totalMoneyEarned += num;
      if (this.useSeparateWallets)
        this.stats.IndividualMoneyEarned += num;
      Game1.stats.checkForMoneyAchievements();
    }
  }

  public void addUnearnedMoney(int money) => this._money += money;

  public List<string> GetEmoteFavorites()
  {
    if (this.emoteFavorites.Count == 0)
    {
      this.emoteFavorites.Add("question");
      this.emoteFavorites.Add("heart");
      this.emoteFavorites.Add("yes");
      this.emoteFavorites.Add("happy");
      this.emoteFavorites.Add("pause");
      this.emoteFavorites.Add("sad");
      this.emoteFavorites.Add("no");
      this.emoteFavorites.Add("angry");
    }
    return this.emoteFavorites;
  }

  public Farmer()
  {
    NetBool netBool = new NetBool();
    netBool.InterpolationWait = false;
    this.requestingTimePause = netBool;
    this.stats = new Stats();
    this.personalShippingBin = new NetRef<Inventory>(new Inventory());
    this.displayedShippedItems = (IList<Item>) new List<Item>();
    this.biteChime = new NetInt(-1);
    this._recoveredItem = new NetRef<Item>();
    this.itemsLostLastDeath = new NetObjectList<Item>();
    this.movementDirections = new List<int>();
    this.farmName = new NetString("");
    this.favoriteThing = new NetString();
    this.horseName = new NetString();
    this.tempFoodItemTextureName = new NetString();
    this.tempFoodItemSourceRect = new NetRectangle();
    this.buffs = new BuffManager();
    this.messageQueue = (IList<OutgoingMessage>) new List<OutgoingMessage>();
    this.uniqueMultiplayerID = new NetLong(Utility.RandomLong());
    this.userID = new NetString("");
    this.previousLocationName = "";
    this.platformType = new NetString("");
    this.platformID = new NetString("");
    this.hasMenuOpen = new NetBool(false);
    this.DEFAULT_SHIRT_COLOR = Color.White;
    this.whichPetType = "Cat";
    this.whichPetBreed = "0";
    this.acceptedDailyQuest = new NetBool(false);
    this.farmerRenderer = new NetRef<FarmerRenderer>();
    this.toolPower = new NetInt();
    this.toolHold = new NetInt();
    this.emoteFavorites = new List<string>();
    this.performedEmotes = new SerializableDictionary<string, bool>();
    this.shirt = new NetString("1000");
    this.hair = new NetInt(0);
    this.skin = new NetInt(0);
    this.shoes = new NetString("2");
    this.accessory = new NetInt(-1);
    this.facialHair = new NetInt(-1);
    this.pants = new NetString("0");
    this.netFestivalScore = new NetInt();
    this.lastGotPrizeFromGil = new NetRef<WorldDate>();
    this.lastDesertFestivalFishingQuest = new NetRef<WorldDate>();
    this.hairstyleColor = new NetColor(new Color(193, 90, 50));
    this.prismaticHair = new NetBool();
    this.pantsColor = new NetColor(new Color(46, 85, 183));
    this.newEyeColor = new NetColor(new Color(122, 68, 52));
    this.hat = new NetRef<Hat>();
    this.boots = new NetRef<Boots>();
    this.leftRing = new NetRef<Ring>();
    this.rightRing = new NetRef<Ring>();
    this.shirtItem = new NetRef<Clothing>();
    this.pantsItem = new NetRef<Clothing>();
    this.dancePartner = new NetDancePartner();
    this.exhausted = new NetBool();
    this.divorceTonight = new NetBool();
    this.changeWalletTypeTonight = new NetBool();
    this.onBridge = new NetBool();
    this.netDeepestMineLevel = new NetInt();
    this.currentToolIndex = new NetInt(0);
    this.temporaryItem = new NetRef<Item>();
    this.cursorSlotItem = new NetRef<Item>();
    this.netItemStowed = new NetBool(false);
    this.gameVersion = "-1";
    this.bobberStyle = new NetInt(0);
    this.caveChoice = new NetInt();
    this.farmingLevel = new NetInt();
    this.miningLevel = new NetInt();
    this.combatLevel = new NetInt();
    this.foragingLevel = new NetInt();
    this.fishingLevel = new NetInt();
    this.luckLevel = new NetInt();
    this.maxStamina = new NetInt(270);
    this.maxItems = new NetInt(12);
    this.lastSeenMovieWeek = new NetInt(-1);
    this.viewingLocation = new NetString();
    this.netStamina = new NetFloat(270f);
    this.teamRoot = new NetRoot<FarmerTeam>(new FarmerTeam());
    NetLong netLong = new NetLong();
    netLong.DeltaAggregateTicks = (ushort) (60 * (Game1.realMilliSecondsPerGameTenMinutes / 1000));
    this.netMillisecondsPlayed = netLong;
    this.toolBeingUpgraded = new NetRef<Tool>();
    this.daysLeftForToolUpgrade = new NetInt();
    this.houseUpgradeLevel = new NetInt(0);
    this.daysUntilHouseUpgrade = new NetInt(-1);
    this.showChestColorPicker = true;
    this.enchantments = new List<BaseEnchantment>();
    this.BaseMagneticRadius = 128 /*0x80*/;
    this.currentTemporaryInvincibilityDuration = 1200;
    this.craftingTime = 1000;
    this.raftPuddleCounter = 250;
    this.raftBobCounter = 1000;
    this.health = 100;
    this.maxHealth = 100;
    this.netTimesReachedMineBottom = new NetInt(0);
    this.difficultyModifier = 1f;
    this.jitter = Vector2.Zero;
    this.lastGrabTile = Vector2.Zero;
    this.netGender = new NetEnum<Gender>();
    this.canMove = true;
    this.usingTool = new NetBool(false);
    this.isInBed = new NetBool(false);
    this.bathingClothes = new NetBool(false);
    this.flashDuringThisTemporaryInvincibility = true;
    this.netCanReleaseTool = new NetBool(false);
    this.temporaryPassableTiles = new BoundingBoxGroup();
    this.hidden = new NetBool();
    this.basicShipped = new NetStringDictionary<int, NetInt>();
    this.mineralsFound = new NetStringDictionary<int, NetInt>();
    this.recipesCooked = new NetStringDictionary<int, NetInt>();
    this.fishCaught = new NetStringIntArrayDictionary();
    this.archaeologyFound = new NetStringIntArrayDictionary();
    this.callsReceived = new NetStringDictionary<int, NetInt>();
    this.tailoredItems = new NetStringDictionary<int, NetInt>();
    this.friendshipData = new NetStringDictionary<Friendship, NetRef<Friendship>>();
    this.locationBeforeForcedEvent = new NetString((string) null);
    this.netSpouse = new NetString();
    this.netMount = new NetRef<Horse>();
    this.isSitting = new NetBool();
    this.mapChairSitPosition = new NetVector2(new Vector2(-1f, -1f));
    this.hasCompletedAllMonsterSlayerQuests = new NetBool(false);
    this.lerpPosition = -1f;
    this.lerpDuration = -1f;
    this.netQiGems = new NetIntDelta()
    {
      Minimum = new int?(0)
    };
    this.jotpkProgress = new NetRef<AbigailGame.JOTPKProgress>();
    this.hasUsedDailyRevive = new NetBool(false);
    this.trinketItems = new NetList<Trinket, NetRef<Trinket>>();
    this.fireToolEvent = new NetEvent0(true);
    this.beginUsingToolEvent = new NetEvent0(true);
    this.endUsingToolEvent = new NetEvent0(true);
    this.sickAnimationEvent = new NetEvent0();
    this.passOutEvent = new NetEvent0();
    this.haltAnimationEvent = new NetEvent0();
    this.drinkAnimationEvent = new NetEvent1Field<Object, NetRef<Object>>();
    this.eatAnimationEvent = new NetEvent1Field<Object, NetRef<Object>>();
    this.doEmoteEvent = new NetEvent1Field<string, NetString>();
    this.kissFarmerEvent = new NetEvent1Field<long, NetLong>();
    this.synchronizedJumpEvent = new NetEvent1Field<float, NetFloat>();
    this.renovateEvent = new NetEvent1Field<string, NetString>();
    this.chestConsumedMineLevels = new NetIntDictionary<bool, NetBool>();
    this.isCustomized = new NetBool(false);
    this.homeLocation = new NetString("FarmHouse");
    this.lastSleepLocation = new NetString();
    this.lastSleepPoint = new NetPoint();
    this.disconnectDay = new NetInt(-1);
    this.disconnectLocation = new NetString();
    this.disconnectPosition = new NetVector2();
    this.emoteFacingDirection = 2;
    this.toolHoldStartTime = new NetInt();
    this.movementMultiplier = 0.01f;
    // ISSUE: explicit constructor call
    base.\u002Ector();
    this.farmerInit();
    this.Sprite = (AnimatedSprite) new FarmerSprite((string) null);
  }

  public Farmer(
    FarmerSprite sprite,
    Vector2 position,
    int speed,
    string name,
    List<Item> initialTools,
    bool isMale)
  {
    NetBool netBool = new NetBool();
    netBool.InterpolationWait = false;
    this.requestingTimePause = netBool;
    this.stats = new Stats();
    this.personalShippingBin = new NetRef<Inventory>(new Inventory());
    this.displayedShippedItems = (IList<Item>) new List<Item>();
    this.biteChime = new NetInt(-1);
    this._recoveredItem = new NetRef<Item>();
    this.itemsLostLastDeath = new NetObjectList<Item>();
    this.movementDirections = new List<int>();
    this.farmName = new NetString("");
    this.favoriteThing = new NetString();
    this.horseName = new NetString();
    this.tempFoodItemTextureName = new NetString();
    this.tempFoodItemSourceRect = new NetRectangle();
    this.buffs = new BuffManager();
    this.messageQueue = (IList<OutgoingMessage>) new List<OutgoingMessage>();
    this.uniqueMultiplayerID = new NetLong(Utility.RandomLong());
    this.userID = new NetString("");
    this.previousLocationName = "";
    this.platformType = new NetString("");
    this.platformID = new NetString("");
    this.hasMenuOpen = new NetBool(false);
    this.DEFAULT_SHIRT_COLOR = Color.White;
    this.whichPetType = "Cat";
    this.whichPetBreed = "0";
    this.acceptedDailyQuest = new NetBool(false);
    this.farmerRenderer = new NetRef<FarmerRenderer>();
    this.toolPower = new NetInt();
    this.toolHold = new NetInt();
    this.emoteFavorites = new List<string>();
    this.performedEmotes = new SerializableDictionary<string, bool>();
    this.shirt = new NetString("1000");
    this.hair = new NetInt(0);
    this.skin = new NetInt(0);
    this.shoes = new NetString("2");
    this.accessory = new NetInt(-1);
    this.facialHair = new NetInt(-1);
    this.pants = new NetString("0");
    this.netFestivalScore = new NetInt();
    this.lastGotPrizeFromGil = new NetRef<WorldDate>();
    this.lastDesertFestivalFishingQuest = new NetRef<WorldDate>();
    this.hairstyleColor = new NetColor(new Color(193, 90, 50));
    this.prismaticHair = new NetBool();
    this.pantsColor = new NetColor(new Color(46, 85, 183));
    this.newEyeColor = new NetColor(new Color(122, 68, 52));
    this.hat = new NetRef<Hat>();
    this.boots = new NetRef<Boots>();
    this.leftRing = new NetRef<Ring>();
    this.rightRing = new NetRef<Ring>();
    this.shirtItem = new NetRef<Clothing>();
    this.pantsItem = new NetRef<Clothing>();
    this.dancePartner = new NetDancePartner();
    this.exhausted = new NetBool();
    this.divorceTonight = new NetBool();
    this.changeWalletTypeTonight = new NetBool();
    this.onBridge = new NetBool();
    this.netDeepestMineLevel = new NetInt();
    this.currentToolIndex = new NetInt(0);
    this.temporaryItem = new NetRef<Item>();
    this.cursorSlotItem = new NetRef<Item>();
    this.netItemStowed = new NetBool(false);
    this.gameVersion = "-1";
    this.bobberStyle = new NetInt(0);
    this.caveChoice = new NetInt();
    this.farmingLevel = new NetInt();
    this.miningLevel = new NetInt();
    this.combatLevel = new NetInt();
    this.foragingLevel = new NetInt();
    this.fishingLevel = new NetInt();
    this.luckLevel = new NetInt();
    this.maxStamina = new NetInt(270);
    this.maxItems = new NetInt(12);
    this.lastSeenMovieWeek = new NetInt(-1);
    this.viewingLocation = new NetString();
    this.netStamina = new NetFloat(270f);
    this.teamRoot = new NetRoot<FarmerTeam>(new FarmerTeam());
    NetLong netLong = new NetLong();
    netLong.DeltaAggregateTicks = (ushort) (60 * (Game1.realMilliSecondsPerGameTenMinutes / 1000));
    this.netMillisecondsPlayed = netLong;
    this.toolBeingUpgraded = new NetRef<Tool>();
    this.daysLeftForToolUpgrade = new NetInt();
    this.houseUpgradeLevel = new NetInt(0);
    this.daysUntilHouseUpgrade = new NetInt(-1);
    this.showChestColorPicker = true;
    this.enchantments = new List<BaseEnchantment>();
    this.BaseMagneticRadius = 128 /*0x80*/;
    this.currentTemporaryInvincibilityDuration = 1200;
    this.craftingTime = 1000;
    this.raftPuddleCounter = 250;
    this.raftBobCounter = 1000;
    this.health = 100;
    this.maxHealth = 100;
    this.netTimesReachedMineBottom = new NetInt(0);
    this.difficultyModifier = 1f;
    this.jitter = Vector2.Zero;
    this.lastGrabTile = Vector2.Zero;
    this.netGender = new NetEnum<Gender>();
    this.canMove = true;
    this.usingTool = new NetBool(false);
    this.isInBed = new NetBool(false);
    this.bathingClothes = new NetBool(false);
    this.flashDuringThisTemporaryInvincibility = true;
    this.netCanReleaseTool = new NetBool(false);
    this.temporaryPassableTiles = new BoundingBoxGroup();
    this.hidden = new NetBool();
    this.basicShipped = new NetStringDictionary<int, NetInt>();
    this.mineralsFound = new NetStringDictionary<int, NetInt>();
    this.recipesCooked = new NetStringDictionary<int, NetInt>();
    this.fishCaught = new NetStringIntArrayDictionary();
    this.archaeologyFound = new NetStringIntArrayDictionary();
    this.callsReceived = new NetStringDictionary<int, NetInt>();
    this.tailoredItems = new NetStringDictionary<int, NetInt>();
    this.friendshipData = new NetStringDictionary<Friendship, NetRef<Friendship>>();
    this.locationBeforeForcedEvent = new NetString((string) null);
    this.netSpouse = new NetString();
    this.netMount = new NetRef<Horse>();
    this.isSitting = new NetBool();
    this.mapChairSitPosition = new NetVector2(new Vector2(-1f, -1f));
    this.hasCompletedAllMonsterSlayerQuests = new NetBool(false);
    this.lerpPosition = -1f;
    this.lerpDuration = -1f;
    this.netQiGems = new NetIntDelta()
    {
      Minimum = new int?(0)
    };
    this.jotpkProgress = new NetRef<AbigailGame.JOTPKProgress>();
    this.hasUsedDailyRevive = new NetBool(false);
    this.trinketItems = new NetList<Trinket, NetRef<Trinket>>();
    this.fireToolEvent = new NetEvent0(true);
    this.beginUsingToolEvent = new NetEvent0(true);
    this.endUsingToolEvent = new NetEvent0(true);
    this.sickAnimationEvent = new NetEvent0();
    this.passOutEvent = new NetEvent0();
    this.haltAnimationEvent = new NetEvent0();
    this.drinkAnimationEvent = new NetEvent1Field<Object, NetRef<Object>>();
    this.eatAnimationEvent = new NetEvent1Field<Object, NetRef<Object>>();
    this.doEmoteEvent = new NetEvent1Field<string, NetString>();
    this.kissFarmerEvent = new NetEvent1Field<long, NetLong>();
    this.synchronizedJumpEvent = new NetEvent1Field<float, NetFloat>();
    this.renovateEvent = new NetEvent1Field<string, NetString>();
    this.chestConsumedMineLevels = new NetIntDictionary<bool, NetBool>();
    this.isCustomized = new NetBool(false);
    this.homeLocation = new NetString("FarmHouse");
    this.lastSleepLocation = new NetString();
    this.lastSleepPoint = new NetPoint();
    this.disconnectDay = new NetInt(-1);
    this.disconnectLocation = new NetString();
    this.disconnectPosition = new NetVector2();
    this.emoteFacingDirection = 2;
    this.toolHoldStartTime = new NetInt();
    this.movementMultiplier = 0.01f;
    // ISSUE: explicit constructor call
    base.\u002Ector((AnimatedSprite) sprite, position, speed, name);
    this.farmerInit();
    this.Name = name;
    this.displayName = name;
    this.Gender = isMale ? Gender.Male : Gender.Female;
    this.stamina = (float) this.maxStamina.Value;
    this.Items.OverwriteWith((IList<Item>) initialTools);
    for (int count = this.Items.Count; count < this.maxItems.Value; ++count)
      this.Items.Add((Item) null);
    this.activeDialogueEvents["Introduction"] = 6;
    if (this.currentLocation != null)
      this.mostRecentBed = Utility.PointToVector2((this.currentLocation as FarmHouse).GetPlayerBedSpot()) * 64f;
    else
      this.mostRecentBed = new Vector2(9f, 9f) * 64f;
  }

  protected override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.uniqueMultiplayerID, "uniqueMultiplayerID").AddField((INetSerializable) this.userID, "userID").AddField((INetSerializable) this.platformType, "platformType").AddField((INetSerializable) this.platformID, "platformID").AddField((INetSerializable) this.hasMenuOpen, "hasMenuOpen").AddField((INetSerializable) this.farmerRenderer, "farmerRenderer").AddField((INetSerializable) this.netGender, "netGender").AddField((INetSerializable) this.bathingClothes, "bathingClothes").AddField((INetSerializable) this.shirt, "shirt").AddField((INetSerializable) this.pants, "pants").AddField((INetSerializable) this.hair, "hair").AddField((INetSerializable) this.skin, "skin").AddField((INetSerializable) this.shoes, "shoes").AddField((INetSerializable) this.accessory, "accessory").AddField((INetSerializable) this.facialHair, "facialHair").AddField((INetSerializable) this.hairstyleColor, "hairstyleColor").AddField((INetSerializable) this.pantsColor, "pantsColor").AddField((INetSerializable) this.newEyeColor, "newEyeColor").AddField((INetSerializable) this.netItems, "netItems").AddField((INetSerializable) this.currentToolIndex, "currentToolIndex").AddField((INetSerializable) this.temporaryItem, "temporaryItem").AddField((INetSerializable) this.cursorSlotItem, "cursorSlotItem").AddField((INetSerializable) this.fireToolEvent, "fireToolEvent").AddField((INetSerializable) this.beginUsingToolEvent, "beginUsingToolEvent").AddField((INetSerializable) this.endUsingToolEvent, "endUsingToolEvent").AddField((INetSerializable) this.hat, "hat").AddField((INetSerializable) this.boots, "boots").AddField((INetSerializable) this.leftRing, "leftRing").AddField((INetSerializable) this.rightRing, "rightRing").AddField((INetSerializable) this.hidden, "hidden").AddField((INetSerializable) this.usingTool, "usingTool").AddField((INetSerializable) this.isInBed, "isInBed").AddField((INetSerializable) this.bobberStyle, "bobberStyle").AddField((INetSerializable) this.caveChoice, "caveChoice").AddField((INetSerializable) this.houseUpgradeLevel, "houseUpgradeLevel").AddField((INetSerializable) this.daysUntilHouseUpgrade, "daysUntilHouseUpgrade").AddField((INetSerializable) this.netSpouse, "netSpouse").AddField((INetSerializable) this.mailReceived, "mailReceived").AddField((INetSerializable) this.mailForTomorrow, "mailForTomorrow").AddField((INetSerializable) this.mailbox, "mailbox").AddField((INetSerializable) this.triggerActionsRun, "triggerActionsRun").AddField((INetSerializable) this.eventsSeen, "eventsSeen").AddField((INetSerializable) this.locationsVisited, "locationsVisited").AddField((INetSerializable) this.secretNotesSeen, "secretNotesSeen").AddField(this.netMount.NetFields, "netMount.NetFields").AddField((INetSerializable) this.dancePartner.NetFields, "dancePartner.NetFields").AddField((INetSerializable) this.divorceTonight, "divorceTonight").AddField((INetSerializable) this.changeWalletTypeTonight, "changeWalletTypeTonight").AddField((INetSerializable) this.isCustomized, "isCustomized").AddField((INetSerializable) this.homeLocation, "homeLocation").AddField((INetSerializable) this.farmName, "farmName").AddField((INetSerializable) this.favoriteThing, "favoriteThing").AddField((INetSerializable) this.horseName, "horseName").AddField((INetSerializable) this.netMillisecondsPlayed, "netMillisecondsPlayed").AddField((INetSerializable) this.netFestivalScore, "netFestivalScore").AddField((INetSerializable) this.friendshipData, "friendshipData").AddField((INetSerializable) this.drinkAnimationEvent, "drinkAnimationEvent").AddField((INetSerializable) this.eatAnimationEvent, "eatAnimationEvent").AddField((INetSerializable) this.sickAnimationEvent, "sickAnimationEvent").AddField((INetSerializable) this.passOutEvent, "passOutEvent").AddField((INetSerializable) this.doEmoteEvent, "doEmoteEvent").AddField((INetSerializable) this.questLog, "questLog").AddField((INetSerializable) this.professions, "professions").AddField((INetSerializable) this.newLevels, "newLevels").AddField((INetSerializable) this.experiencePoints, "experiencePoints").AddField((INetSerializable) this.dialogueQuestionsAnswered, "dialogueQuestionsAnswered").AddField((INetSerializable) this.cookingRecipes, "cookingRecipes").AddField((INetSerializable) this.craftingRecipes, "craftingRecipes").AddField((INetSerializable) this.activeDialogueEvents, "activeDialogueEvents").AddField((INetSerializable) this.previousActiveDialogueEvents, "previousActiveDialogueEvents").AddField((INetSerializable) this.achievements, "achievements").AddField((INetSerializable) this.specialItems, "specialItems").AddField((INetSerializable) this.specialBigCraftables, "specialBigCraftables").AddField((INetSerializable) this.farmingLevel, "farmingLevel").AddField((INetSerializable) this.miningLevel, "miningLevel").AddField((INetSerializable) this.combatLevel, "combatLevel").AddField((INetSerializable) this.foragingLevel, "foragingLevel").AddField((INetSerializable) this.fishingLevel, "fishingLevel").AddField((INetSerializable) this.luckLevel, "luckLevel").AddField((INetSerializable) this.maxStamina, "maxStamina").AddField((INetSerializable) this.netStamina, "netStamina").AddField((INetSerializable) this.maxItems, "maxItems").AddField((INetSerializable) this.chestConsumedMineLevels, "chestConsumedMineLevels").AddField((INetSerializable) this.toolBeingUpgraded, "toolBeingUpgraded").AddField((INetSerializable) this.daysLeftForToolUpgrade, "daysLeftForToolUpgrade").AddField((INetSerializable) this.exhausted, "exhausted").AddField((INetSerializable) this.netDeepestMineLevel, "netDeepestMineLevel").AddField((INetSerializable) this.netTimesReachedMineBottom, "netTimesReachedMineBottom").AddField((INetSerializable) this.netItemStowed, "netItemStowed").AddField((INetSerializable) this.acceptedDailyQuest, "acceptedDailyQuest").AddField((INetSerializable) this.lastSeenMovieWeek, "lastSeenMovieWeek").AddField((INetSerializable) this.shirtItem, "shirtItem").AddField((INetSerializable) this.pantsItem, "pantsItem").AddField((INetSerializable) this.personalShippingBin, "personalShippingBin").AddField((INetSerializable) this.viewingLocation, "viewingLocation").AddField((INetSerializable) this.kissFarmerEvent, "kissFarmerEvent").AddField((INetSerializable) this.haltAnimationEvent, "haltAnimationEvent").AddField((INetSerializable) this.synchronizedJumpEvent, "synchronizedJumpEvent").AddField((INetSerializable) this.tailoredItems, "tailoredItems").AddField((INetSerializable) this.basicShipped, "basicShipped").AddField((INetSerializable) this.mineralsFound, "mineralsFound").AddField((INetSerializable) this.recipesCooked, "recipesCooked").AddField((INetSerializable) this.archaeologyFound, "archaeologyFound").AddField((INetSerializable) this.fishCaught, "fishCaught").AddField((INetSerializable) this.biteChime, "biteChime").AddField((INetSerializable) this._recoveredItem, "_recoveredItem").AddField((INetSerializable) this.itemsLostLastDeath, "itemsLostLastDeath").AddField((INetSerializable) this.renovateEvent, "renovateEvent").AddField((INetSerializable) this.callsReceived, "callsReceived").AddField((INetSerializable) this.onBridge, "onBridge").AddField((INetSerializable) this.lastSleepLocation, "lastSleepLocation").AddField((INetSerializable) this.lastSleepPoint, "lastSleepPoint").AddField((INetSerializable) this.sleptInTemporaryBed, "sleptInTemporaryBed").AddField((INetSerializable) this.timeWentToBed, "timeWentToBed").AddField((INetSerializable) this.hasUsedDailyRevive, "hasUsedDailyRevive").AddField((INetSerializable) this.jotpkProgress, "jotpkProgress").AddField((INetSerializable) this.requestingTimePause, "requestingTimePause").AddField((INetSerializable) this.isSitting, "isSitting").AddField((INetSerializable) this.mapChairSitPosition, "mapChairSitPosition").AddField((INetSerializable) this.netQiGems, "netQiGems").AddField((INetSerializable) this.locationBeforeForcedEvent, "locationBeforeForcedEvent").AddField((INetSerializable) this.hasCompletedAllMonsterSlayerQuests, "hasCompletedAllMonsterSlayerQuests").AddField((INetSerializable) this.buffs.NetFields, "buffs.NetFields").AddField((INetSerializable) this.trinketItems, "trinketItems").AddField((INetSerializable) this.companions, "companions").AddField((INetSerializable) this.prismaticHair, "prismaticHair").AddField((INetSerializable) this.disconnectDay, "disconnectDay").AddField((INetSerializable) this.disconnectLocation, "disconnectLocation").AddField((INetSerializable) this.disconnectPosition, "disconnectPosition").AddField((INetSerializable) this.tempFoodItemTextureName, "tempFoodItemTextureName").AddField((INetSerializable) this.tempFoodItemSourceRect, "tempFoodItemSourceRect").AddField((INetSerializable) this.toolHoldStartTime, "toolHoldStartTime").AddField((INetSerializable) this.toolHold, "toolHold").AddField((INetSerializable) this.toolPower, "toolPower").AddField((INetSerializable) this.netCanReleaseTool, "netCanReleaseTool").AddField((INetSerializable) this.lastGotPrizeFromGil, "lastGotPrizeFromGil").AddField((INetSerializable) this.lastDesertFestivalFishingQuest, "lastDesertFestivalFishingQuest");
    this.fireToolEvent.onEvent += new NetEvent0.Event(this.performFireTool);
    this.beginUsingToolEvent.onEvent += new NetEvent0.Event(this.performBeginUsingTool);
    this.endUsingToolEvent.onEvent += new NetEvent0.Event(this.performEndUsingTool);
    this.drinkAnimationEvent.onEvent += new AbstractNetEvent1<Object>.Event(this.performDrinkAnimation);
    this.eatAnimationEvent.onEvent += new AbstractNetEvent1<Object>.Event(this.performEatAnimation);
    this.sickAnimationEvent.onEvent += new NetEvent0.Event(this.performSickAnimation);
    this.passOutEvent.onEvent += new NetEvent0.Event(this.performPassOut);
    this.doEmoteEvent.onEvent += new AbstractNetEvent1<string>.Event(this.performPlayerEmote);
    this.kissFarmerEvent.onEvent += new AbstractNetEvent1<long>.Event(this.performKissFarmer);
    this.haltAnimationEvent.onEvent += new NetEvent0.Event(this.performHaltAnimation);
    this.synchronizedJumpEvent.onEvent += new AbstractNetEvent1<float>.Event(this.performSynchronizedJump);
    this.renovateEvent.onEvent += new AbstractNetEvent1<string>.Event(this.performRenovation);
    this.netMount.fieldChangeEvent += (FieldChange<NetRef<Horse>, Horse>) ((_param1, _param2, _param3) => this.ClearCachedPosition());
    this.shirtItem.fieldChangeVisibleEvent += (FieldChange<NetRef<Clothing>, Clothing>) ((_param1, _param2, _param3) => this.UpdateClothing());
    this.pantsItem.fieldChangeVisibleEvent += (FieldChange<NetRef<Clothing>, Clothing>) ((_param1, _param2, _param3) => this.UpdateClothing());
    this.trinketItems.OnArrayReplaced += new NetList<Trinket, NetRef<Trinket>>.ArrayReplacedEvent(this.OnTrinketArrayReplaced);
    this.trinketItems.OnElementChanged += new NetList<Trinket, NetRef<Trinket>>.ElementChangedEvent(this.OnTrinketChange);
    this.netItems.fieldChangeEvent += (FieldChange<NetRef<Inventory>, Inventory>) ((field, oldValue, newValue) => newValue.IsLocalPlayerInventory = this.IsLocalPlayer);
  }

  private void farmerInit()
  {
    this.buffs.SetOwner(this);
    this.FarmerRenderer = new FarmerRenderer($"Characters\\Farmer\\farmer_{(this.IsMale ? "" : "girl_")}base", this);
    this.currentLocation = Game1.getLocationFromName(this.homeLocation.Value);
    this.Items.Clear();
    this.giftedItems = new SerializableDictionary<string, SerializableDictionary<string, int>>();
    this.LearnDefaultRecipes();
    this.songsHeard.Add("title_day");
    this.songsHeard.Add("title_night");
    this.changeShirt("1000");
    this.changeSkinColor(0);
    this.changeShoeColor("2");
    this.farmName.FilterStringEvent += new NetString.FilterString(Utility.FilterDirtyWords);
    this.name.FilterStringEvent += new NetString.FilterString(Utility.FilterDirtyWords);
  }

  public virtual void OnWarp()
  {
    foreach (Companion companion in this.companions)
      companion.OnOwnerWarp();
    this.autoGenerateActiveDialogueEvent("firstVisit_" + this.currentLocation.Name);
    if (Stats.AllowRetroactiveAchievements)
      return;
    switch (this.currentLocation.Name)
    {
      case "CommunityCenter":
      case "JojaMart":
        Game1.stats.checkForCommunityCenterOrJojaAchievements(true);
        break;
      case "MasteryCave":
        Game1.stats.checkForSkillAchievements(true);
        Game1.stats.checkForStardropAchievement(true);
        break;
    }
  }

  public Trinket getFirstTrinketWithID(string id)
  {
    foreach (Trinket trinketItem in this.trinketItems)
    {
      if (trinketItem != null && trinketItem.ItemId == id)
        return trinketItem;
    }
    return (Trinket) null;
  }

  public bool hasTrinketWithID(string id)
  {
    foreach (Trinket trinketItem in this.trinketItems)
    {
      if (trinketItem != null && trinketItem.ItemId == id)
        return true;
    }
    return false;
  }

  public void resetAllTrinketEffects()
  {
    this.UnapplyAllTrinketEffects();
    this.ApplyAllTrinketEffects();
  }

  public virtual void ApplyAllTrinketEffects()
  {
    foreach (Trinket trinketItem in this.trinketItems)
    {
      if (trinketItem != null)
      {
        trinketItem.reloadSprite();
        trinketItem.Apply(this);
      }
    }
  }

  public virtual void UnapplyAllTrinketEffects()
  {
    foreach (Trinket trinketItem in this.trinketItems)
      trinketItem?.Unapply(this);
  }

  public virtual void OnTrinketArrayReplaced(
    NetList<Trinket, NetRef<Trinket>> list,
    IList<Trinket> before,
    IList<Trinket> after)
  {
    if (Game1.gameMode != (byte) 0 && Utility.ShouldIgnoreValueChangeCallback() || !this.IsLocalPlayer && !this.isFakeEventActor && Game1.gameMode != (byte) 0)
      return;
    foreach (Trinket trinket in (IEnumerable<Trinket>) before)
      trinket?.Unapply(this);
    foreach (Trinket trinket in (IEnumerable<Trinket>) after)
      trinket?.Apply(this);
  }

  public virtual void OnTrinketChange(
    NetList<Trinket, NetRef<Trinket>> list,
    int index,
    Trinket old_value,
    Trinket new_value)
  {
    if (Game1.gameMode != (byte) 0 && Utility.ShouldIgnoreValueChangeCallback() || !this.IsLocalPlayer && !this.isFakeEventActor && Game1.gameMode != (byte) 0)
      return;
    old_value?.Unapply(this);
    new_value?.Apply(this);
  }

  public bool CanEmote()
  {
    return Game1.farmEvent == null && (!Game1.eventUp || Game1.CurrentEvent == null || Game1.CurrentEvent.playerControlSequence || !this.IsLocalPlayer) && !this.usingSlingshot && !this.isEating && !this.UsingTool && (this.CanMove || !this.IsLocalPlayer) && !this.IsSitting() && !this.isRidingHorse() && !this.bathingClothes.Value;
  }

  /// <summary>Learn the recipes that have no unlock requirements.</summary>
  public void LearnDefaultRecipes()
  {
    foreach (KeyValuePair<string, string> craftingRecipe in CraftingRecipe.craftingRecipes)
    {
      if (!this.craftingRecipes.ContainsKey(craftingRecipe.Key) && ArgUtility.Get(craftingRecipe.Value.Split('/'), 4) == "default")
        this.craftingRecipes.Add(craftingRecipe.Key, 0);
    }
    foreach (KeyValuePair<string, string> cookingRecipe in CraftingRecipe.cookingRecipes)
    {
      if (!this.cookingRecipes.ContainsKey(cookingRecipe.Key) && ArgUtility.Get(cookingRecipe.Value.Split('/'), 3) == "default")
        this.cookingRecipes.Add(cookingRecipe.Key, 0);
    }
  }

  /// <summary>Add any recipes and mail that should have been unlocked based on criteria like skill levels and stats.</summary>
  public void AddMissedMailAndRecipes()
  {
    bool flag = false;
    foreach (KeyValuePair<string, string> cookingRecipe in CraftingRecipe.cookingRecipes)
    {
      int skillNumber;
      int minLevel;
      if (CraftingRecipe.TryParseLevelRequirement(cookingRecipe.Key, cookingRecipe.Value, true, out skillNumber, out minLevel) && this.GetUnmodifiedSkillLevel(skillNumber) >= minLevel)
      {
        this.cookingRecipes.TryAdd(cookingRecipe.Key, 0);
        flag = true;
      }
    }
    foreach (KeyValuePair<string, string> craftingRecipe in CraftingRecipe.craftingRecipes)
    {
      int skillNumber;
      int minLevel;
      if (CraftingRecipe.TryParseLevelRequirement(craftingRecipe.Key, craftingRecipe.Value, false, out skillNumber, out minLevel) && this.GetUnmodifiedSkillLevel(skillNumber) >= minLevel)
        this.craftingRecipes.TryAdd(craftingRecipe.Key, 0);
    }
    if (flag && !this.hasOrWillReceiveMail("robinKitchenLetter"))
      this.team.RequestSetMail(PlayerActionTarget.All, "robinKitchenLetter", MailType.Now, true, new long?(this.uniqueMultiplayerID.Value));
    if (this.farmingLevel.Value >= 10 && !this.hasOrWillReceiveMail("marnieAutoGrabber"))
      this.team.RequestSetMail(PlayerActionTarget.All, "marnieAutoGrabber", MailType.Tomorrow, true, new long?(this.uniqueMultiplayerID.Value));
    if (this.stats.Get("completedJunimoKart") > 0U && !this.hasOrWillReceiveMail("JunimoKart"))
      this.team.RequestSetMail(PlayerActionTarget.All, "JunimoKart", MailType.Tomorrow, true, new long?(this.uniqueMultiplayerID.Value));
    if (this.stats.Get("completedPrairieKing") <= 0U || this.hasOrWillReceiveMail("Beat_PK"))
      return;
    this.team.RequestSetMail(PlayerActionTarget.All, "Beat_PK", MailType.Tomorrow, true, new long?(this.uniqueMultiplayerID.Value));
  }

  public void performRenovation(string location_name)
  {
    if (!(Game1.RequireLocation(location_name) is FarmHouse farmHouse))
      return;
    farmHouse.UpdateForRenovation();
  }

  public void performPlayerEmote(string emote_string)
  {
    for (int index = 0; index < Farmer.EMOTES.Length; ++index)
    {
      Farmer.EmoteType emoteType = Farmer.EMOTES[index];
      if (emoteType.emoteString == emote_string)
      {
        this.performedEmotes[emote_string] = true;
        if (emoteType.animationFrames != null)
        {
          if (!this.CanEmote())
            break;
          if (this.isEmoteAnimating)
            this.EndEmoteAnimation();
          else if (this.FarmerSprite.PauseForSingleAnimation)
            break;
          this.isEmoteAnimating = true;
          this._emoteGracePeriod = 200;
          if (this == Game1.player)
            this.noMovementPause = Math.Max(this.noMovementPause, 200);
          this.emoteFacingDirection = emoteType.facingDirection;
          this.FarmerSprite.animateOnce(emoteType.animationFrames, new AnimatedSprite.endOfAnimationBehavior(this.OnEmoteAnimationEnd));
        }
        if (emoteType.emoteIconIndex >= 0)
        {
          this.isEmoting = false;
          this.doEmote(emoteType.emoteIconIndex, false);
        }
      }
    }
  }

  public bool ShouldHandleAnimationSound()
  {
    return !LocalMultiplayer.IsLocalMultiplayer(true) || this.IsLocalPlayer;
  }

  public static List<Item> initialTools()
  {
    return new List<Item>()
    {
      ItemRegistry.Create("(T)Axe"),
      ItemRegistry.Create("(T)Hoe"),
      ItemRegistry.Create("(T)WateringCan"),
      ItemRegistry.Create("(T)Pickaxe"),
      ItemRegistry.Create("(W)47")
    };
  }

  private void playHarpEmoteSound()
  {
    int[] numArray1 = new int[4]{ 1200, 1600, 1900, 2400 };
    switch (Game1.random.Next(5))
    {
      case 0:
        numArray1 = new int[4]{ 1200, 1600, 1900, 2400 };
        break;
      case 1:
        numArray1 = new int[4]{ 1200, 1700, 2100, 2400 };
        break;
      case 2:
        numArray1 = new int[4]{ 1100, 1400, 1900, 2300 };
        break;
      case 3:
        numArray1 = new int[3]{ 1600, 1900, 2400 };
        break;
      case 4:
        numArray1 = new int[3]{ 700, 1200, 1900 };
        break;
    }
    if (!this.IsLocalPlayer)
      return;
    if (Game1.IsMultiplayer && this.UniqueMultiplayerID % 111L == 0L)
    {
      int[] numArray2 = new int[4]
      {
        800 + Game1.random.Next(4) * 100,
        1200 + Game1.random.Next(4) * 100,
        1600 + Game1.random.Next(4) * 100,
        2000 + Game1.random.Next(4) * 100
      };
      for (int index = 0; index < numArray2.Length; ++index)
      {
        DelayedAction.playSoundAfterDelay("miniharp_note", Game1.random.Next(60, 150) * index, this.currentLocation, new Vector2?(this.Tile), numArray2[index]);
        if (index > 1 && Game1.random.NextDouble() < 0.25)
          break;
      }
    }
    else
    {
      for (int index = 0; index < numArray1.Length; ++index)
        DelayedAction.playSoundAfterDelay("miniharp_note", index > 0 ? 150 + Game1.random.Next(35, 51) * index : 0, this.currentLocation, new Vector2?(this.Tile), numArray1[index]);
    }
  }

  private static void removeLowestUpgradeLevelTool(List<Item> items, Type toolType)
  {
    Tool tool1 = (Tool) null;
    foreach (Item obj in items)
    {
      if (obj is Tool tool2 && tool2.GetType() == toolType && (tool1 == null || tool2.upgradeLevel.Value < tool1.upgradeLevel.Value))
        tool1 = tool2;
    }
    if (tool1 == null)
      return;
    items.Remove((Item) tool1);
  }

  public static void removeInitialTools(List<Item> items)
  {
    Farmer.removeLowestUpgradeLevelTool(items, typeof (Axe));
    Farmer.removeLowestUpgradeLevelTool(items, typeof (Hoe));
    Farmer.removeLowestUpgradeLevelTool(items, typeof (WateringCan));
    Farmer.removeLowestUpgradeLevelTool(items, typeof (Pickaxe));
    Item obj = items.FirstOrDefault<Item>((Func<Item, bool>) (item => item is MeleeWeapon && item.ItemId == "47"));
    if (obj == null)
      return;
    items.Remove(obj);
  }

  public Point getMailboxPosition()
  {
    foreach (Building building in Game1.getFarm().buildings)
    {
      if (building.isCabin && building.HasIndoorsName(this.homeLocation.Value))
        return building.getMailboxPosition();
    }
    return Game1.getFarm().GetMainMailboxPosition();
  }

  public void ClearBuffs()
  {
    this.buffs.Clear();
    this.stopGlowing();
  }

  public bool isActive()
  {
    return this == Game1.player || Game1.otherFarmers.ContainsKey(this.UniqueMultiplayerID);
  }

  public string getTexture()
  {
    return $"Characters\\Farmer\\farmer_{(this.IsMale ? "" : "girl_")}base{(this.isBald() ? "_bald" : "")}";
  }

  public void unload() => this.FarmerRenderer?.unload();

  public void setInventory(List<Item> newInventory)
  {
    this.Items.OverwriteWith((IList<Item>) newInventory);
    for (int count = this.Items.Count; count < this.maxItems.Value; ++count)
      this.Items.Add((Item) null);
  }

  public void makeThisTheActiveObject(Object o)
  {
    if (this.freeSpotsInInventory() <= 0)
      return;
    Item currentItem = this.CurrentItem;
    this.ActiveObject = o;
    this.addItemToInventory(currentItem);
  }

  public int getNumberOfChildren() => this.getChildrenCount();

  private void setMount(Horse mount)
  {
    if (mount != null)
    {
      this.netMount.Value = mount;
      this.xOffset = -11f;
      this.Position = Utility.PointToVector2(mount.GetBoundingBox().Location);
      this.position.Y -= 16f;
      this.position.X -= 8f;
      this.speed = 2;
      this.showNotCarrying();
    }
    else
    {
      this.netMount.Value = (Horse) null;
      this.collisionNPC = (NPC) null;
      this.running = false;
      this.speed = !Game1.isOneOfTheseKeysDown(Game1.GetKeyboardState(), Game1.options.runButton) || Game1.options.autoRun ? 2 : 5;
      this.running = this.speed == 5;
      if (this.running)
        this.speed = 5;
      else
        this.speed = 2;
      this.completelyStopAnimatingOrDoingAction();
      this.xOffset = 0.0f;
    }
  }

  public bool isRidingHorse() => this.mount != null && !Game1.eventUp;

  public List<Child> getChildren() => Utility.getHomeOfFarmer(this).getChildren();

  public int getChildrenCount() => Utility.getHomeOfFarmer(this).getChildrenCount();

  public Tool getToolFromName(string name)
  {
    foreach (Item obj in this.Items)
    {
      if (obj is Tool toolFromName && toolFromName.Name.Contains(name))
        return toolFromName;
    }
    return (Tool) null;
  }

  public override void SetMovingDown(bool b) => this.setMoving((byte) (4 + (b ? 0 : 32 /*0x20*/)));

  public override void SetMovingRight(bool b) => this.setMoving((byte) (2 + (b ? 0 : 32 /*0x20*/)));

  public override void SetMovingUp(bool b) => this.setMoving((byte) (1 + (b ? 0 : 32 /*0x20*/)));

  public override void SetMovingLeft(bool b) => this.setMoving((byte) (8 + (b ? 0 : 32 /*0x20*/)));

  public int? tryGetFriendshipLevelForNPC(string name)
  {
    Friendship friendship;
    return this.friendshipData.TryGetValue(name, out friendship) ? new int?(friendship.Points) : new int?();
  }

  public int getFriendshipLevelForNPC(string name)
  {
    Friendship friendship;
    return this.friendshipData.TryGetValue(name, out friendship) ? friendship.Points : 0;
  }

  public int getFriendshipHeartLevelForNPC(string name)
  {
    return this.getFriendshipLevelForNPC(name) / 250;
  }

  /// <summary>Get whether the player is roommates with a given NPC (excluding marriage).</summary>
  /// <param name="npc">The NPC's internal name.</param>
  /// <remarks>See also <see cref="M:StardewValley.Farmer.hasRoommate" />.</remarks>
  public bool isRoommate(string name)
  {
    Friendship friendship;
    return name != null && this.friendshipData.TryGetValue(name, out friendship) && friendship.IsRoommate();
  }

  /// <summary>Get whether the player is or will soon be roommates with an NPC (excluding marriage).</summary>
  public bool hasCurrentOrPendingRoommate()
  {
    Friendship friendship;
    return this.spouse != null && this.friendshipData.TryGetValue(this.spouse, out friendship) && friendship.RoommateMarriage;
  }

  /// <summary>Get whether the player is roommates with an NPC (excluding marriage).</summary>
  /// <remarks>See also <see cref="M:StardewValley.Farmer.isRoommate(System.String)" />.</remarks>
  public bool hasRoommate() => this.isRoommate(this.spouse);

  public bool hasAFriendWithFriendshipPoints(int minPoints, bool datablesOnly, int maxPoints = 2147483647 /*0x7FFFFFFF*/)
  {
    bool found = false;
    Utility.ForEachVillager((Func<NPC, bool>) (n =>
    {
      if (!datablesOnly || n.datable.Value)
      {
        int friendshipLevelForNpc = this.getFriendshipLevelForNPC(n.Name);
        if (friendshipLevelForNpc >= minPoints && friendshipLevelForNpc <= maxPoints)
          found = true;
      }
      return !found;
    }));
    return found;
  }

  public bool hasAFriendWithHeartLevel(int minHeartLevel, bool datablesOnly, int maxHeartLevel = 2147483647 /*0x7FFFFFFF*/)
  {
    int minPoints = minHeartLevel * 250;
    int maxPoints = maxHeartLevel * 250;
    if (maxPoints < maxHeartLevel)
      maxPoints = int.MaxValue;
    return this.hasAFriendWithFriendshipPoints(minPoints, datablesOnly, maxPoints);
  }

  public void shippedBasic(string itemId, int number)
  {
    int num;
    if (!this.basicShipped.TryGetValue(itemId, out num))
      num = 0;
    this.basicShipped[itemId] = num + number;
  }

  public void shiftToolbar(bool right)
  {
    if (this.Items == null || this.Items.Count < 12 || this.UsingTool || Game1.dialogueUp || !this.CanMove || !this.Items.HasAny() || Game1.eventUp || Game1.farmEvent != null)
      return;
    Game1.playSound("shwip");
    this.CurrentItem?.actionWhenStopBeingHeld(this);
    if (right)
    {
      IList<Item> range = this.Items.GetRange(0, 12);
      this.Items.RemoveRange(0, 12);
      this.Items.AddRange((ICollection<Item>) range);
    }
    else
    {
      IList<Item> range = this.Items.GetRange(this.Items.Count - 12, 12);
      for (int index = 0; index < this.Items.Count - 12; ++index)
        range.Add(this.Items[index]);
      this.Items.OverwriteWith(range);
    }
    this.netItemStowed.Set(false);
    this.CurrentItem?.actionWhenBeingHeld(this);
    for (int index = 0; index < Game1.onScreenMenus.Count; ++index)
    {
      if (Game1.onScreenMenus[index] is Toolbar onScreenMenu)
      {
        onScreenMenu.shifted(right);
        break;
      }
    }
  }

  public void foundWalnut(int stack = 1)
  {
    if (Game1.netWorldState.Value.GoldenWalnutsFound >= 130)
      return;
    Game1.netWorldState.Value.GoldenWalnuts += stack;
    Game1.netWorldState.Value.GoldenWalnutsFound += stack;
    Game1.PerformActionWhenPlayerFree(new Action(this.showNutPickup));
  }

  public virtual void RemoveMail(string mail_key, bool from_broadcast_list = false)
  {
    mail_key = mail_key.Replace("%&NL&%", "");
    this.mailReceived.Remove(mail_key);
    this.mailbox.Remove(mail_key);
    this.mailForTomorrow.Remove(mail_key);
    this.mailForTomorrow.Remove(mail_key + "%&NL&%");
    if (!from_broadcast_list)
      return;
    this.team.broadcastedMail.Remove("%&SM&%" + mail_key);
    this.team.broadcastedMail.Remove("%&MFT&%" + mail_key);
    this.team.broadcastedMail.Remove("%&MB&%" + mail_key);
  }

  public virtual void showNutPickup()
  {
    if (!this.hasOrWillReceiveMail("lostWalnutFound") && !Game1.eventUp)
    {
      Game1.addMailForTomorrow("lostWalnutFound", true);
      this.completelyStopAnimatingOrDoingAction();
      this.holdUpItemThenMessage(ItemRegistry.Create("(O)73"));
    }
    else
    {
      if (!this.hasOrWillReceiveMail("lostWalnutFound") || Game1.eventUp)
        return;
      this.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors2", new Microsoft.Xna.Framework.Rectangle(0, 240 /*0xF0*/, 16 /*0x10*/, 16 /*0x10*/), 100f, 4, 2, new Vector2(0.0f, -96f), false, false, 1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
      {
        motion = new Vector2(0.0f, -6f),
        acceleration = new Vector2(0.0f, 0.2f),
        stopAcceleratingWhenVelocityIsZero = true,
        attachedCharacter = (Character) this,
        positionFollowsAttachedCharacter = true
      });
    }
  }

  /// <summary>Handle the player finding an artifact object.</summary>
  /// <param name="itemId">The unqualified item ID for an <see cref="F:StardewValley.ItemRegistry.type_object" />-type item.</param>
  /// <param name="number">The number found.</param>
  public void foundArtifact(string itemId, int number)
  {
    bool flag = false;
    if (itemId == "102")
    {
      if (!this.hasOrWillReceiveMail("lostBookFound"))
      {
        Game1.addMailForTomorrow("lostBookFound", true);
        flag = true;
      }
      else
        Game1.showGlobalMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:FishingRod.cs.14100"));
      Game1.playSound("newRecipe");
      ++Game1.netWorldState.Value.LostBooksFound;
      Game1.multiplayer.globalChatInfoMessage("LostBook", this.displayName);
    }
    int[] numArray;
    if (this.archaeologyFound.TryGetValue(itemId, out numArray))
    {
      numArray[0] += number;
      numArray[1] += number;
      this.archaeologyFound[itemId] = numArray;
    }
    else
    {
      if (this.archaeologyFound.Length == 0)
      {
        if (!this.eventsSeen.Contains("0") && itemId != "102")
          this.addQuest("23");
        this.mailReceived.Add("artifactFound");
        flag = true;
      }
      this.archaeologyFound.Add(itemId, new int[2]
      {
        number,
        number
      });
    }
    if (!flag)
      return;
    this.holdUpItemThenMessage(ItemRegistry.Create("(O)" + itemId));
  }

  public void cookedRecipe(string itemId)
  {
    int num;
    if (!this.recipesCooked.TryGetValue(itemId, out num))
      num = 0;
    this.recipesCooked[itemId] = num + 1;
  }

  public bool caughtFish(string itemId, int size, bool from_fish_pond = false, int numberCaught = 1)
  {
    ItemMetadata metadata = ItemRegistry.GetMetadata(itemId);
    itemId = metadata.QualifiedItemId;
    int num1 = from_fish_pond || !metadata.Exists() || ItemContextTagManager.HasBaseTag(metadata.QualifiedItemId, "trash_item") || itemId == "(O)167" ? 0 : (metadata.GetParsedData()?.ObjectType == "Fish" ? 1 : (metadata.QualifiedItemId == "(O)372" ? 1 : 0));
    bool flag = false;
    if (num1 != 0)
    {
      int[] numArray;
      if (this.fishCaught.TryGetValue(itemId, out numArray))
      {
        numArray[0] += numberCaught;
        Game1.stats.checkForFishingAchievements();
        if (size > this.fishCaught[itemId][1])
        {
          numArray[1] = size;
          flag = true;
        }
        this.fishCaught[itemId] = numArray;
      }
      else
      {
        this.fishCaught.Add(itemId, new int[2]
        {
          numberCaught,
          size
        });
        Game1.stats.checkForFishingAchievements();
        this.autoGenerateActiveDialogueEvent("fishCaught_" + metadata.LocalItemId);
      }
      this.NotifyQuests((Func<Quest, bool>) (quest => quest.OnFishCaught(itemId, numberCaught, size)));
      if (Utility.GetDayOfPassiveFestival("SquidFest") > 0 && itemId == "(O)151")
      {
        int num2 = (int) Game1.stats.Increment(StatKeys.SquidFestScore(Game1.dayOfMonth, Game1.year), numberCaught);
      }
    }
    return flag;
  }

  public virtual void gainExperience(int which, int howMuch)
  {
    if (which == 5 || howMuch <= 0)
      return;
    if (!this.IsLocalPlayer && Game1.IsServer)
    {
      this.queueMessage((byte) 17, Game1.player, (object) which, (object) howMuch);
    }
    else
    {
      if (this.Level >= 25)
      {
        int currentMasteryLevel = MasteryTrackerMenu.getCurrentMasteryLevel();
        int num = (int) Game1.stats.Increment("MasteryExp", Math.Max(1, which == 0 ? howMuch / 2 : howMuch));
        if (MasteryTrackerMenu.getCurrentMasteryLevel() > currentMasteryLevel)
        {
          Game1.showGlobalMessage(Game1.content.LoadString("Strings\\1_6_Strings:Mastery_newlevel"));
          Game1.playSound("newArtifact");
        }
      }
      int num1 = Farmer.checkForLevelGain(this.experiencePoints[which], this.experiencePoints[which] + howMuch);
      this.experiencePoints[which] += howMuch;
      int num2 = -1;
      if (num1 != -1)
      {
        switch (which)
        {
          case 0:
            num2 = this.farmingLevel.Value;
            this.farmingLevel.Value = num1;
            break;
          case 1:
            num2 = this.fishingLevel.Value;
            this.fishingLevel.Value = num1;
            break;
          case 2:
            num2 = this.foragingLevel.Value;
            this.foragingLevel.Value = num1;
            break;
          case 3:
            num2 = this.miningLevel.Value;
            this.miningLevel.Value = num1;
            break;
          case 4:
            num2 = this.combatLevel.Value;
            this.combatLevel.Value = num1;
            break;
          case 5:
            num2 = this.luckLevel.Value;
            this.luckLevel.Value = num1;
            break;
        }
      }
      if (num1 <= num2)
        return;
      for (int y = num2 + 1; y <= num1; ++y)
      {
        this.newLevels.Add(new Point(which, y));
        if (this.newLevels.Count == 1)
          Game1.showGlobalMessage(Game1.content.LoadString("Strings\\1_6_Strings:NewIdeas"));
      }
    }
  }

  public int getEffectiveSkillLevel(int whichSkill)
  {
    if (whichSkill < 0 || whichSkill > 5)
      return -1;
    int[] numArray = new int[6]
    {
      this.farmingLevel.Value,
      this.fishingLevel.Value,
      this.foragingLevel.Value,
      this.miningLevel.Value,
      this.combatLevel.Value,
      this.luckLevel.Value
    };
    for (int index = 0; index < this.newLevels.Count; ++index)
      --numArray[this.newLevels[index].X];
    return numArray[whichSkill];
  }

  public static int checkForLevelGain(int oldXP, int newXP)
  {
    for (int level = 10; level >= 1; --level)
    {
      if (oldXP < Farmer.getBaseExperienceForLevel(level) && newXP >= Farmer.getBaseExperienceForLevel(level))
        return level;
    }
    return -1;
  }

  public static int getBaseExperienceForLevel(int level)
  {
    switch (level)
    {
      case 1:
        return 100;
      case 2:
        return 380;
      case 3:
        return 770;
      case 4:
        return 1300;
      case 5:
        return 2150;
      case 6:
        return 3300;
      case 7:
        return 4800;
      case 8:
        return 6900;
      case 9:
        return 10000;
      case 10:
        return 15000;
      default:
        return -1;
    }
  }

  /// <summary>Mark a gift as having been revealed to the player, even if it hasn't yet been gifted.</summary>
  /// <param name="npcName">The name of the NPC.</param>
  /// <param name="itemId">The item ID.</param>
  public void revealGiftTaste(string npcName, string itemId)
  {
    if (npcName == null)
      return;
    SerializableDictionary<string, int> serializableDictionary;
    if (!this.giftedItems.TryGetValue(npcName, out serializableDictionary))
      this.giftedItems[npcName] = serializableDictionary = new SerializableDictionary<string, int>();
    serializableDictionary.TryAdd(itemId, 0);
  }

  public void onGiftGiven(NPC npc, Object item)
  {
    if (item.bigCraftable.Value)
      return;
    SerializableDictionary<string, int> dictionary;
    if (!this.giftedItems.TryGetValue(npc.name.Value, out dictionary))
      this.giftedItems[npc.name.Value] = dictionary = new SerializableDictionary<string, int>();
    int valueOrDefault = dictionary.GetValueOrDefault<string, int>(item.ItemId);
    dictionary[item.ItemId] = valueOrDefault + 1;
    if (this.team.specialOrders == null)
      return;
    foreach (SpecialOrder specialOrder in this.team.specialOrders)
    {
      Action<Farmer, NPC, Item> onGiftGiven = specialOrder.onGiftGiven;
      if (onGiftGiven != null)
        onGiftGiven(this, npc, (Item) item);
    }
  }

  public bool hasGiftTasteBeenRevealed(NPC npc, string itemId)
  {
    if (this.hasItemBeenGifted(npc, itemId))
      return true;
    SerializableDictionary<string, int> serializableDictionary;
    return this.giftedItems.TryGetValue(npc.name.Value, out serializableDictionary) && serializableDictionary.ContainsKey(itemId);
  }

  public bool hasItemBeenGifted(NPC npc, string itemId)
  {
    SerializableDictionary<string, int> serializableDictionary;
    int num;
    return this.giftedItems.TryGetValue(npc.name.Value, out serializableDictionary) && serializableDictionary.TryGetValue(itemId, out num) && num > 0;
  }

  public void MarkItemAsTailored(Item item)
  {
    if (item == null)
      return;
    string descriptionFromItem = Utility.getStandardDescriptionFromItem(item, 1);
    int num;
    if (!this.tailoredItems.TryGetValue(descriptionFromItem, out num))
      num = 0;
    this.tailoredItems[descriptionFromItem] = num + 1;
  }

  public bool HasTailoredThisItem(Item item)
  {
    return item != null && this.tailoredItems.ContainsKey(Utility.getStandardDescriptionFromItem(item, 1));
  }

  /// <summary>Handle the player finding a mineral object.</summary>
  /// <param name="itemId">The unqualified item ID for an <see cref="F:StardewValley.ItemRegistry.type_object" />-type item.</param>
  public void foundMineral(string itemId)
  {
    int num;
    if (!this.mineralsFound.TryGetValue(itemId, out num))
      num = 0;
    this.mineralsFound[itemId] = num + 1;
    if (this.hasOrWillReceiveMail("artifactFound"))
      return;
    this.mailReceived.Add("artifactFound");
  }

  public void increaseBackpackSize(int howMuch)
  {
    this.MaxItems += howMuch;
    while (this.Items.Count < this.MaxItems)
      this.Items.Add((Item) null);
  }

  public override int FacingDirection
  {
    get
    {
      if (!this.IsLocalPlayer && !this.isFakeEventActor && this.UsingTool && this.CurrentTool is FishingRod currentTool && currentTool.CastDirection >= 0)
        return currentTool.CastDirection;
      return this.isEmoteAnimating ? this.emoteFacingDirection : this.facingDirection.Value;
    }
    set => this.facingDirection.Set(value);
  }

  [Obsolete("Most code should use Items.CountId instead. However this method works a bit differently in that the item ID can be 858 (Qi Gems), 73 (Golden Walnuts), a category number, or -777 to match seasonal wild seeds.")]
  public int getItemCount(string itemId)
  {
    return this.getItemCountInList((IList<Item>) this.Items, itemId);
  }

  [Obsolete("Most code should use Items.CountId instead. However this method works a bit differently in that the item ID can be a category number, or -777 to match seasonal wild seeds.")]
  public int getItemCountInList(IList<Item> list, string itemId)
  {
    int itemCountInList = 0;
    for (int index = 0; index < list.Count; ++index)
    {
      if (list[index] != null && CraftingRecipe.ItemMatchesForCrafting(list[index], itemId))
        itemCountInList += list[index].Stack;
    }
    return itemCountInList;
  }

  /// <summary>Cause the player to lose a random number of items based on their luck after dying. These will be added to <see cref="F:StardewValley.Farmer.itemsLostLastDeath" /> so they can recover one of them.</summary>
  /// <param name="random">The RNG to use, or <c>null</c> to create one.</param>
  /// <returns>Returns the number of items lost.</returns>
  public int LoseItemsOnDeath(Random random = null)
  {
    if (random == null)
      random = Utility.CreateDaySaveRandom((double) Game1.timeOfDay);
    double chance = 0.22 - (double) this.LuckLevel * 0.04 - this.DailyLuck;
    int num = 0;
    this.itemsLostLastDeath.Clear();
    for (int index = this.Items.Count - 1; index >= 0; --index)
    {
      Item obj = this.Items[index];
      if (obj != null && obj.CanBeLostOnDeath() && random.NextBool(chance))
      {
        ++num;
        this.Items[index] = (Item) null;
        this.itemsLostLastDeath.Add(obj);
        if (num == 3)
          break;
      }
    }
    return num;
  }

  public void ShowSitting()
  {
    if (!this.IsSitting())
      return;
    if (this.sittingFurniture != null)
      this.FacingDirection = this.sittingFurniture.GetSittingDirection();
    if (this.yJumpOffset != 0)
    {
      switch (this.FacingDirection)
      {
        case 0:
          this.FarmerSprite.setCurrentSingleFrame(12);
          break;
        case 1:
          this.FarmerSprite.setCurrentSingleFrame(6);
          break;
        case 2:
          this.FarmerSprite.setCurrentSingleFrame(0);
          break;
        case 3:
          this.FarmerSprite.setCurrentSingleFrame(6, flip: true);
          break;
      }
    }
    else
    {
      switch (this.FacingDirection)
      {
        case 0:
          this.FarmerSprite.setCurrentSingleFrame(113);
          this.xOffset = 0.0f;
          this.yOffset = -40f;
          break;
        case 1:
          this.FarmerSprite.setCurrentSingleFrame(117);
          this.xOffset = -4f;
          this.yOffset = -32f;
          break;
        case 2:
          this.FarmerSprite.setCurrentSingleFrame(107, secondaryArm: true);
          this.xOffset = 0.0f;
          this.yOffset = -48f;
          break;
        case 3:
          this.FarmerSprite.setCurrentSingleFrame(117, flip: true);
          this.xOffset = 4f;
          this.yOffset = -32f;
          break;
      }
    }
  }

  public void showRiding()
  {
    if (!this.isRidingHorse())
      return;
    this.xOffset = -6f;
    switch (this.FacingDirection)
    {
      case 0:
        this.FarmerSprite.setCurrentSingleFrame(113);
        break;
      case 1:
        this.FarmerSprite.setCurrentSingleFrame(106);
        this.xOffset += 2f;
        break;
      case 2:
        this.FarmerSprite.setCurrentSingleFrame(107);
        break;
      case 3:
        this.FarmerSprite.setCurrentSingleFrame(106, flip: true);
        this.xOffset = -12f;
        break;
    }
    if (this.isMoving())
    {
      switch (this.mount.Sprite.currentAnimationIndex)
      {
        case 0:
          this.yOffset = 0.0f;
          break;
        case 1:
          this.yOffset = -4f;
          break;
        case 2:
          this.yOffset = -4f;
          break;
        case 3:
          this.yOffset = 0.0f;
          break;
        case 4:
          this.yOffset = 4f;
          break;
        case 5:
          this.yOffset = 4f;
          break;
      }
    }
    else
      this.yOffset = 0.0f;
  }

  public void showCarrying()
  {
    if (Game1.eventUp || this.isRidingHorse() || Game1.killScreen || this.IsSitting())
      return;
    if (this.bathingClothes.Value || this.onBridge.Value)
    {
      this.showNotCarrying();
    }
    else
    {
      if (!this.FarmerSprite.PauseForSingleAnimation && !this.isMoving())
      {
        switch (this.FacingDirection)
        {
          case 0:
            this.FarmerSprite.setCurrentFrame(144 /*0x90*/);
            break;
          case 1:
            this.FarmerSprite.setCurrentFrame(136);
            break;
          case 2:
            this.FarmerSprite.setCurrentFrame(128 /*0x80*/);
            break;
          case 3:
            this.FarmerSprite.setCurrentFrame(152);
            break;
        }
      }
      if (this.ActiveObject != null)
        this.mostRecentlyGrabbedItem = (Item) this.ActiveObject;
      if (!this.IsLocalPlayer || !(this.mostRecentlyGrabbedItem?.QualifiedItemId == "(O)434"))
        return;
      this.eatHeldObject();
    }
  }

  public void showNotCarrying()
  {
    if (this.FarmerSprite.PauseForSingleAnimation || this.isMoving())
      return;
    bool flag = this.canOnlyWalk || this.bathingClothes.Value || this.onBridge.Value;
    switch (this.FacingDirection)
    {
      case 0:
        this.FarmerSprite.setCurrentFrame(flag ? 16 /*0x10*/ : 48 /*0x30*/, flag ? 1 : 0);
        break;
      case 1:
        this.FarmerSprite.setCurrentFrame(flag ? 8 : 40, flag ? 1 : 0);
        break;
      case 2:
        this.FarmerSprite.setCurrentFrame(flag ? 0 : 32 /*0x20*/, flag ? 1 : 0);
        break;
      case 3:
        this.FarmerSprite.setCurrentFrame(flag ? 24 : 56, flag ? 1 : 0);
        break;
    }
  }

  public int GetDaysMarried()
  {
    Friendship spouseFriendship = this.GetSpouseFriendship();
    return spouseFriendship == null ? 0 : spouseFriendship.DaysMarried;
  }

  public Friendship GetSpouseFriendship()
  {
    long? spouse = this.team.GetSpouse(this.UniqueMultiplayerID);
    if (spouse.HasValue)
      return this.team.GetFriendship(this.UniqueMultiplayerID, spouse.Value);
    Friendship friendship;
    return string.IsNullOrEmpty(this.spouse) || !this.friendshipData.TryGetValue(this.spouse, out friendship) ? (Friendship) null : friendship;
  }

  public bool hasDailyQuest()
  {
    for (int index = this.questLog.Count - 1; index >= 0; --index)
    {
      if (this.questLog[index].dailyQuest.Value)
        return true;
    }
    return false;
  }

  public void showToolUpgradeAvailability()
  {
    int dayOfMonth = Game1.dayOfMonth;
    if (!((NetFieldBase<Tool, NetRef<Tool>>) this.toolBeingUpgraded != (NetRef<Tool>) null) || this.daysLeftForToolUpgrade.Value > 0 || this.toolBeingUpgraded.Value == null || Utility.isFestivalDay() || !(Game1.shortDayNameFromDayOfSeason(dayOfMonth) != "Fri") && this.hasCompletedCommunityCenter() && !Game1.isRaining || this.hasReceivedToolUpgradeMessageYet)
      return;
    if (Game1.newDay)
      Game1.morningQueue.Enqueue((Action) (() => Game1.showGlobalMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:ToolReady", (object) this.toolBeingUpgraded.Value.DisplayName))));
    else
      Game1.showGlobalMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:ToolReady", (object) this.toolBeingUpgraded.Value.DisplayName));
    this.hasReceivedToolUpgradeMessageYet = true;
  }

  public void dayupdate(int timeWentToSleep)
  {
    if (this.IsSitting())
      this.StopSitting(false);
    this.resetFriendshipsForNewDay();
    this.LearnDefaultRecipes();
    this.hasUsedDailyRevive.Value = false;
    this.hasBeenBlessedByStatueToday = false;
    this.acceptedDailyQuest.Set(false);
    this.dancePartner.Value = (Character) null;
    this.festivalScore = 0;
    this.forceTimePass = false;
    if (this.daysLeftForToolUpgrade.Value > 0)
      --this.daysLeftForToolUpgrade.Value;
    if (this.daysUntilHouseUpgrade.Value > 0)
    {
      --this.daysUntilHouseUpgrade.Value;
      if (this.daysUntilHouseUpgrade.Value <= 0)
      {
        FarmHouse homeOfFarmer = Utility.getHomeOfFarmer(this);
        homeOfFarmer.moveObjectsForHouseUpgrade(this.houseUpgradeLevel.Value + 1);
        ++this.houseUpgradeLevel.Value;
        this.daysUntilHouseUpgrade.Value = -1;
        homeOfFarmer.setMapForUpgradeLevel(this.houseUpgradeLevel.Value);
        Game1.stats.checkForBuildingUpgradeAchievements();
        this.autoGenerateActiveDialogueEvent("houseUpgrade_" + this.houseUpgradeLevel.Value.ToString());
      }
    }
    this.questLog.RemoveWhere((Func<Quest, bool>) (quest =>
    {
      if (!quest.IsTimedQuest())
        return false;
      --quest.daysLeft.Value;
      return quest.daysLeft.Value <= 0 && !quest.completed.Value;
    }));
    this.ClearBuffs();
    if (this.MaxStamina >= 508)
      this.mailReceived.Add("gotMaxStamina");
    float stamina = this.Stamina;
    this.Stamina = (float) this.MaxStamina;
    bool flag = this.exhausted.Value;
    if (flag)
    {
      this.exhausted.Value = false;
      this.Stamina = (float) (this.MaxStamina / 2 + 1);
    }
    int val2 = this.timeWentToBed.Value == 0 ? timeWentToSleep : this.timeWentToBed.Value;
    if (val2 > 2400)
    {
      this.Stamina -= (float) (1.0 - (double) (2600 - Math.Min(2600, val2)) / 200.0) * (float) (this.MaxStamina / 2);
      if (timeWentToSleep > 2700)
        this.Stamina /= 2f;
    }
    if (timeWentToSleep < 2700 && (double) stamina > (double) this.Stamina && !flag)
      this.Stamina = stamina;
    this.health = this.maxHealth;
    this.activeDialogueEvents.RemoveWhere((Func<KeyValuePair<string, int>, bool>) (pair =>
    {
      string key = pair.Key;
      if (!key.Contains("_memory_"))
        this.previousActiveDialogueEvents.TryAdd(key, 0);
      this.activeDialogueEvents[key]--;
      if (this.activeDialogueEvents[key] < 0)
      {
        if (!(key == "pennyRedecorating") || Utility.getHomeOfFarmer(this).GetSpouseBed() != null)
          return true;
        this.activeDialogueEvents[key] = 0;
      }
      return false;
    }));
    foreach (string key in this.previousActiveDialogueEvents.Keys)
    {
      this.previousActiveDialogueEvents[key]++;
      if (this.previousActiveDialogueEvents[key] == 1)
        this.activeDialogueEvents[key + "_memory_oneday"] = 4;
      if (this.previousActiveDialogueEvents[key] == 7)
        this.activeDialogueEvents[key + "_memory_oneweek"] = 4;
      if (this.previousActiveDialogueEvents[key] == 14)
        this.activeDialogueEvents[key + "_memory_twoweeks"] = 4;
      if (this.previousActiveDialogueEvents[key] == 28)
        this.activeDialogueEvents[key + "_memory_fourweeks"] = 4;
      if (this.previousActiveDialogueEvents[key] == 56)
        this.activeDialogueEvents[key + "_memory_eightweeks"] = 4;
      if (this.previousActiveDialogueEvents[key] == 104)
        this.activeDialogueEvents[key + "_memory_oneyear"] = 4;
    }
    this.hasMoved = false;
    if (Game1.random.NextDouble() < 0.905 && !this.hasOrWillReceiveMail("RarecrowSociety") && Utility.doesItemExistAnywhere("(BC)136") && Utility.doesItemExistAnywhere("(BC)137") && Utility.doesItemExistAnywhere("(BC)138") && Utility.doesItemExistAnywhere("(BC)139") && Utility.doesItemExistAnywhere("(BC)140") && Utility.doesItemExistAnywhere("(BC)126") && Utility.doesItemExistAnywhere("(BC)110") && Utility.doesItemExistAnywhere("(BC)113"))
      this.mailbox.Add("RarecrowSociety");
    this.timeWentToBed.Value = 0;
    this.stats.Set("blessingOfWaters", 0);
    if (this.shirtItem.Value == null || this.pantsItem.Value == null || !(this.currentLocation is FarmHouse) && !(this.currentLocation is IslandFarmHouse) && !(this.currentLocation is Shed))
      return;
    foreach (Object @object in this.currentLocation.netObjects.Values)
    {
      if (@object is Mannequin mannequin && mannequin.GetMannequinData().Cursed && Game1.random.NextDouble() < 0.005 && !mannequin.swappedWithFarmerTonight.Value)
      {
        mannequin.hat.Value = this.Equip<Hat>(mannequin.hat.Value, this.hat);
        mannequin.shirt.Value = this.Equip<Clothing>(mannequin.shirt.Value, this.shirtItem);
        mannequin.pants.Value = this.Equip<Clothing>(mannequin.pants.Value, this.pantsItem);
        mannequin.boots.Value = this.Equip<Boots>(mannequin.boots.Value, this.boots);
        mannequin.swappedWithFarmerTonight.Value = true;
        this.currentLocation.playSound("cursed_mannequin");
        mannequin.eyeTimer = 1000;
      }
    }
  }

  public bool hasSeenActiveDialogueEvent(string eventName)
  {
    return this.activeDialogueEvents.ContainsKey(eventName) || this.previousActiveDialogueEvents.ContainsKey(eventName);
  }

  public bool autoGenerateActiveDialogueEvent(string eventName, int duration = 4)
  {
    if (this.hasSeenActiveDialogueEvent(eventName))
      return false;
    this.activeDialogueEvents[eventName] = duration;
    return true;
  }

  public void removeDatingActiveDialogueEvents(string npcName)
  {
    this.activeDialogueEvents.Remove("dating_" + npcName);
    this.removeActiveDialogMemoryEvents("dating_" + npcName);
    this.previousActiveDialogueEvents.Remove("dating_" + npcName);
  }

  public void removeMarriageActiveDialogueEvents(string npcName)
  {
    this.activeDialogueEvents.Remove("married_" + npcName);
    this.removeActiveDialogMemoryEvents("married_" + npcName);
    this.previousActiveDialogueEvents.Remove("married_" + npcName);
  }

  public void removeActiveDialogMemoryEvents(string activeDialogKey)
  {
    this.activeDialogueEvents.Remove(activeDialogKey + "_memory_oneday");
    this.activeDialogueEvents.Remove(activeDialogKey + "_memory_oneweek");
    this.activeDialogueEvents.Remove(activeDialogKey + "_memory_twoweeks");
    this.activeDialogueEvents.Remove(activeDialogKey + "_memory_fourweeks");
    this.activeDialogueEvents.Remove(activeDialogKey + "_memory_eightweeks");
    this.activeDialogueEvents.Remove(activeDialogKey + "_memory_oneyear");
  }

  public void doDivorce()
  {
    this.divorceTonight.Value = false;
    if (!this.isMarriedOrRoommates())
      return;
    NPC npc = (NPC) null;
    if (this.spouse != null)
    {
      npc = this.getSpouse();
      if (npc != null)
      {
        this.removeMarriageActiveDialogueEvents(npc.Name);
        if (!npc.isRoommate())
          this.autoGenerateActiveDialogueEvent("divorced_" + npc.Name);
        this.spouse = (string) null;
        this.specialItems.RemoveWhere((Func<string, bool>) (id => id == "460"));
        Friendship friendship;
        if (this.friendshipData.TryGetValue(npc.name.Value, out friendship))
        {
          friendship.Points = 0;
          friendship.RoommateMarriage = false;
          friendship.Status = FriendshipStatus.Divorced;
        }
        Utility.getHomeOfFarmer(this).showSpouseRoom();
        Game1.getFarm().UpdatePatio();
        this.removeQuest("126");
      }
    }
    else
    {
      long? spouse = this.team.GetSpouse(this.UniqueMultiplayerID);
      if (spouse.HasValue)
      {
        spouse = this.team.GetSpouse(this.UniqueMultiplayerID);
        Friendship friendship = this.team.GetFriendship(this.UniqueMultiplayerID, spouse.Value);
        friendship.Points = 0;
        friendship.RoommateMarriage = false;
        friendship.Status = FriendshipStatus.Divorced;
      }
    }
    bool? nullable = npc?.isRoommate();
    if (nullable.HasValue && nullable.GetValueOrDefault() || this.autoGenerateActiveDialogueEvent("divorced_once"))
      return;
    this.autoGenerateActiveDialogueEvent("divorced_twice");
  }

  public static void showReceiveNewItemMessage(Farmer who, Item item, int countAdded)
  {
    bool flag;
    Game1.drawObjectDialogue(new List<string>()
    {
      item.checkForSpecialItemHoldUpMeessage() ?? (!(item.TryGetTempData<bool>("FromStarterGiftBox", out flag) & flag) || !(item.QualifiedItemId == "(O)472") || countAdded != 15 ? (!item.HasContextTag("book_item") ? (countAdded > 1 ? Game1.content.LoadString("Strings\\StringsFromCSFiles:Farmer.cs.1922", (object) countAdded, (object) item.DisplayName) : Game1.content.LoadString("Strings\\StringsFromCSFiles:Farmer.cs.1919", (object) item.DisplayName, (object) Lexicon.getProperArticleForWord(item.DisplayName))) : Game1.content.LoadString("Strings\\1_6_Strings:FoundABook", (object) item.DisplayName)) : Game1.content.LoadString("Strings\\StringsFromCSFiles:Farmer.cs.1918"))
    });
    who.completelyStopAnimatingOrDoingAction();
  }

  public static void showEatingItem(Farmer who)
  {
    TemporaryAnimatedSprite temporaryAnimatedSprite1 = (TemporaryAnimatedSprite) null;
    if (who.itemToEat == null)
      return;
    TemporaryAnimatedSprite temporaryAnimatedSprite2 = (TemporaryAnimatedSprite) null;
    ParsedItemData dataOrErrorItem1 = ItemRegistry.GetDataOrErrorItem(who.itemToEat.QualifiedItemId);
    string textureName = dataOrErrorItem1.TextureName;
    Microsoft.Xna.Framework.Rectangle sourceRect1 = dataOrErrorItem1.GetSourceRect();
    Color color = Color.White;
    Color white = Color.White;
    if (who.tempFoodItemTextureName.Value != null)
    {
      textureName = who.tempFoodItemTextureName.Value;
      sourceRect1 = who.tempFoodItemSourceRect.Value;
    }
    else if ((who.itemToEat is Object itemToEat2 ? itemToEat2.preservedParentSheetIndex.Value : (string) null) != null)
    {
      if (who.itemToEat.ItemId.Equals("SmokedFish"))
      {
        ParsedItemData dataOrErrorItem2 = ItemRegistry.GetDataOrErrorItem("(O)" + (who.itemToEat as Object).preservedParentSheetIndex.Value);
        textureName = dataOrErrorItem2.TextureName;
        sourceRect1 = dataOrErrorItem2.GetSourceRect();
        color = new Color(130, 100, 83);
      }
      else if (who.itemToEat is ColoredObject itemToEat1)
        white = itemToEat1.color.Value;
    }
    switch (who.FarmerSprite.currentAnimationIndex)
    {
      case 1:
        if (who.IsLocalPlayer && who.itemToEat.QualifiedItemId == "(O)434")
        {
          temporaryAnimatedSprite1 = new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(368, 16 /*0x10*/, 16 /*0x10*/, 16 /*0x10*/), 62.75f, 8, 2, who.Position + new Vector2(-21f, -112f), false, false, (float) ((double) who.StandingPixel.Y / 10000.0 + 0.0099999997764825821), 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f);
          break;
        }
        temporaryAnimatedSprite1 = new TemporaryAnimatedSprite(textureName, sourceRect1, 254f, 1, 0, who.Position + new Vector2(-21f, -112f), false, false, (float) ((double) who.StandingPixel.Y / 10000.0 + 0.0099999997764825821), 0.0f, color, 4f, 0.0f, 0.0f, 0.0f);
        if (!white.Equals(Color.White))
        {
          sourceRect1.X += sourceRect1.Width;
          temporaryAnimatedSprite2 = new TemporaryAnimatedSprite(textureName, sourceRect1, 254f, 1, 0, who.Position + new Vector2(-21f, -112f), false, false, (float) ((double) (who.StandingPixel.Y + 1) / 10000.0 + 0.0099999997764825821), 0.0f, white, 4f, 0.0f, 0.0f, 0.0f);
          break;
        }
        break;
      case 2:
        if (who.IsLocalPlayer && who.itemToEat.QualifiedItemId == "(O)434")
        {
          temporaryAnimatedSprite1 = new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(368, 16 /*0x10*/, 16 /*0x10*/, 16 /*0x10*/), 81.25f, 8, 0, who.Position + new Vector2(-21f, -108f), false, false, (float) ((double) who.StandingPixel.Y / 10000.0 + 0.0099999997764825821), 0.0f, Color.White, 4f, -0.01f, 0.0f, 0.0f)
          {
            motion = new Vector2(0.8f, -11f),
            acceleration = new Vector2(0.0f, 0.5f)
          };
          break;
        }
        if (Game1.currentLocation == who.currentLocation)
          Game1.playSound("dwop");
        temporaryAnimatedSprite1 = new TemporaryAnimatedSprite(textureName, sourceRect1, 650f, 1, 0, who.Position + new Vector2(-21f, -108f), false, false, (float) ((double) who.StandingPixel.Y / 10000.0 + 0.0099999997764825821), 0.0f, color, 4f, -0.01f, 0.0f, 0.0f)
        {
          motion = new Vector2(0.8f, -11f),
          acceleration = new Vector2(0.0f, 0.5f)
        };
        if (!white.Equals(Color.White))
        {
          sourceRect1.X += sourceRect1.Width;
          temporaryAnimatedSprite2 = new TemporaryAnimatedSprite(textureName, sourceRect1, 650f, 1, 0, who.Position + new Vector2(-21f, -108f), false, false, (float) ((double) (who.StandingPixel.Y + 1) / 10000.0 + 0.0099999997764825821), 0.0f, white, 4f, 0.0f, 0.0f, 0.0f)
          {
            motion = new Vector2(0.8f, -11f),
            acceleration = new Vector2(0.0f, 0.5f)
          };
          break;
        }
        break;
      case 3:
        who.yJumpVelocity = 6f;
        who.yJumpOffset = 1;
        break;
      case 4:
        if (Game1.currentLocation == who.currentLocation && who.ShouldHandleAnimationSound())
          Game1.playSound("eat");
        for (int index = 0; index < 8; ++index)
        {
          int num = Game1.random.Next(2, 4);
          Microsoft.Xna.Framework.Rectangle sourceRect2 = sourceRect1.Clone();
          sourceRect2.X += 8;
          sourceRect2.Y += 8;
          sourceRect2.Width = num;
          sourceRect2.Height = num;
          TemporaryAnimatedSprite temporaryAnimatedSprite3 = new TemporaryAnimatedSprite(textureName, sourceRect2, 400f, 1, 0, who.Position + new Vector2(24f, -48f), false, false, (float) ((double) who.StandingPixel.Y / 10000.0 + 0.0099999997764825821), 0.0f, color, 4f, 0.0f, 0.0f, 0.0f)
          {
            motion = new Vector2((float) Game1.random.Next(-30, 31 /*0x1F*/) / 10f, (float) Game1.random.Next(-6, -3)),
            acceleration = new Vector2(0.0f, 0.5f)
          };
          who.currentLocation.temporarySprites.Add(temporaryAnimatedSprite3);
        }
        return;
      default:
        who.freezePause = 0;
        break;
    }
    if (temporaryAnimatedSprite1 != null)
      who.currentLocation.temporarySprites.Add(temporaryAnimatedSprite1);
    if (temporaryAnimatedSprite2 == null)
      return;
    who.currentLocation.temporarySprites.Add(temporaryAnimatedSprite2);
  }

  public static void eatItem(Farmer who)
  {
  }

  /// <summary>Get whether the player has a buff applied.</summary>
  /// <param name="id">The buff ID, like <see cref="F:StardewValley.Buff.tipsy" />.</param>
  public bool hasBuff(string id) => this.buffs.IsApplied(id);

  /// <summary>Add a buff to the player, or refresh it if it's already applied.</summary>
  /// <param name="id">The buff ID, like <see cref="F:StardewValley.Buff.tipsy" />.</param>
  public void applyBuff(string id) => this.buffs.Apply(new Buff(id));

  /// <summary>Add a buff to the player, or refresh it if it's already applied.</summary>
  /// <param name="id">The buff to apply.</param>
  public void applyBuff(Buff buff) => this.buffs.Apply(buff);

  /// <summary>Get whether the player has a buff with an ID containing the given string.</summary>
  /// <param name="idSubstring">The substring to match in the buff ID.</param>
  public bool hasBuffWithNameContainingString(string idSubstr)
  {
    return this.buffs.HasBuffWithNameContaining(idSubstr);
  }

  public bool hasOrWillReceiveMail(string id)
  {
    return this.mailReceived.Contains(id) || this.mailForTomorrow.Contains(id) || this.mailbox.Contains(id) || this.mailForTomorrow.Contains(id + "%&NL&%");
  }

  public static void showHoldingItem(Farmer who, Item item)
  {
    switch (item)
    {
      case SpecialItem specialItem:
        TemporaryAnimatedSprite spriteForHoldingUp = specialItem.getTemporarySpriteForHoldingUp(who.Position + new Vector2(0.0f, -124f));
        spriteForHoldingUp.motion = new Vector2(0.0f, -0.1f);
        spriteForHoldingUp.scale = 4f;
        spriteForHoldingUp.interval = 2500f;
        spriteForHoldingUp.totalNumberOfLoops = 0;
        spriteForHoldingUp.animationLength = 1;
        Game1.currentLocation.temporarySprites.Add(spriteForHoldingUp);
        break;
      case Slingshot _:
      case MeleeWeapon _:
      case Boots _:
        TemporaryAnimatedSprite temporaryAnimatedSprite1 = new TemporaryAnimatedSprite((string) null, new Microsoft.Xna.Framework.Rectangle(), 2500f, 1, 0, who.Position + new Vector2(0.0f, -124f), false, false, 1f, 0.0f, Color.White, 1f, 0.0f, 0.0f, 0.0f)
        {
          motion = new Vector2(0.0f, -0.1f)
        };
        temporaryAnimatedSprite1.CopyAppearanceFromItemId(item.QualifiedItemId);
        Game1.currentLocation.temporarySprites.Add(temporaryAnimatedSprite1);
        break;
      case Hat _:
        TemporaryAnimatedSprite temporaryAnimatedSprite2 = new TemporaryAnimatedSprite((string) null, new Microsoft.Xna.Framework.Rectangle(), 2500f, 1, 0, who.Position + new Vector2(-8f, -124f), false, false, 1f, 0.0f, Color.White, 1f, 0.0f, 0.0f, 0.0f)
        {
          motion = new Vector2(0.0f, -0.1f)
        };
        temporaryAnimatedSprite2.CopyAppearanceFromItemId(item.QualifiedItemId);
        Game1.currentLocation.temporarySprites.Add(temporaryAnimatedSprite2);
        break;
      case Furniture _:
        TemporaryAnimatedSprite temporaryAnimatedSprite3 = new TemporaryAnimatedSprite((string) null, new Microsoft.Xna.Framework.Rectangle(), 2500f, 1, 0, Vector2.Zero, false, false)
        {
          motion = new Vector2(0.0f, -0.1f),
          layerDepth = 1f
        };
        temporaryAnimatedSprite3.CopyAppearanceFromItemId(item.QualifiedItemId);
        temporaryAnimatedSprite3.initialPosition = temporaryAnimatedSprite3.position = who.Position + new Vector2((float) (32 /*0x20*/ - temporaryAnimatedSprite3.sourceRect.Width / 2 * 4), -188f);
        Game1.currentLocation.temporarySprites.Add(temporaryAnimatedSprite3);
        break;
      case Object _:
      case Tool _:
        if (item is Object @object && @object.bigCraftable.Value)
        {
          TemporaryAnimatedSprite temporaryAnimatedSprite4 = new TemporaryAnimatedSprite((string) null, new Microsoft.Xna.Framework.Rectangle(), 2500f, 1, 0, who.Position + new Vector2(0.0f, -188f), false, false)
          {
            motion = new Vector2(0.0f, -0.1f),
            layerDepth = 1f
          };
          temporaryAnimatedSprite4.CopyAppearanceFromItemId(item.QualifiedItemId);
          Game1.currentLocation.temporarySprites.Add(temporaryAnimatedSprite4);
          break;
        }
        TemporaryAnimatedSprite temporaryAnimatedSprite5 = new TemporaryAnimatedSprite((string) null, new Microsoft.Xna.Framework.Rectangle(), 2500f, 1, 0, who.Position + new Vector2(0.0f, -124f), false, false)
        {
          motion = new Vector2(0.0f, -0.1f),
          layerDepth = 1f
        };
        temporaryAnimatedSprite5.CopyAppearanceFromItemId(item.QualifiedItemId);
        Game1.currentLocation.temporarySprites.Add(temporaryAnimatedSprite5);
        if (!who.IsLocalPlayer || !(item.QualifiedItemId == "(O)434"))
          break;
        who.eatHeldObject();
        break;
      case Ring _:
        TemporaryAnimatedSprite temporaryAnimatedSprite6 = new TemporaryAnimatedSprite((string) null, new Microsoft.Xna.Framework.Rectangle(), 2500f, 1, 0, who.Position + new Vector2(-4f, -124f), false, false)
        {
          motion = new Vector2(0.0f, -0.1f),
          layerDepth = 1f
        };
        temporaryAnimatedSprite6.CopyAppearanceFromItemId(item.QualifiedItemId);
        Game1.currentLocation.temporarySprites.Add(temporaryAnimatedSprite6);
        break;
      case null:
        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(420, 489, 25, 18), 2500f, 1, 0, who.Position + new Vector2(-20f, -152f), false, false)
        {
          motion = new Vector2(0.0f, -0.1f),
          scale = 4f,
          layerDepth = 1f
        });
        break;
      default:
        TemporaryAnimatedSprite temporaryAnimatedSprite7 = new TemporaryAnimatedSprite((string) null, new Microsoft.Xna.Framework.Rectangle(), 2500f, 1, 0, who.Position + new Vector2(0.0f, -124f), false, false)
        {
          motion = new Vector2(0.0f, -0.1f),
          layerDepth = 1f
        };
        temporaryAnimatedSprite7.CopyAppearanceFromItemId(item.QualifiedItemId);
        Game1.currentLocation.temporarySprites.Add(temporaryAnimatedSprite7);
        break;
    }
  }

  public void holdUpItemThenMessage(Item item, bool showMessage = true)
  {
    this.holdUpItemThenMessage(item, item == null ? 1 : item.Stack, showMessage);
  }

  public void holdUpItemThenMessage(Item item, int countAdded, bool showMessage = true)
  {
    this.completelyStopAnimatingOrDoingAction();
    if (showMessage)
    {
      Game1.MusicDuckTimer = 2000f;
      DelayedAction.playSoundAfterDelay("getNewSpecialItem", 750);
    }
    this.faceDirection(2);
    this.freezePause = 4000;
    this.FarmerSprite.animateOnce(new FarmerSprite.AnimationFrame[3]
    {
      new FarmerSprite.AnimationFrame(57, 0),
      new FarmerSprite.AnimationFrame(57, 2500, false, false, (AnimatedSprite.endOfAnimationBehavior) (who => Farmer.showHoldingItem(who, item))),
      showMessage ? new FarmerSprite.AnimationFrame((int) (short) this.FarmerSprite.CurrentFrame, 500, false, false, (AnimatedSprite.endOfAnimationBehavior) (who => Farmer.showReceiveNewItemMessage(who, item, countAdded)), true) : new FarmerSprite.AnimationFrame((int) (short) this.FarmerSprite.CurrentFrame, 500, false, false)
    });
    this.mostRecentlyGrabbedItem = item;
    this.canMove = false;
  }

  public void resetState()
  {
    this.mount = (Horse) null;
    this.ClearBuffs();
    this.TemporaryItem = (Item) null;
    this.swimming.Value = false;
    this.bathingClothes.Value = false;
    this.ignoreCollisions = false;
    this.resetItemStates();
    this.fireToolEvent.Clear();
    this.beginUsingToolEvent.Clear();
    this.endUsingToolEvent.Clear();
    this.sickAnimationEvent.Clear();
    this.passOutEvent.Clear();
    this.drinkAnimationEvent.Clear();
    this.eatAnimationEvent.Clear();
  }

  public void resetItemStates()
  {
    for (int index = 0; index < this.Items.Count; ++index)
      this.Items[index]?.resetState();
  }

  public void clearBackpack()
  {
    for (int index = 0; index < this.Items.Count; ++index)
      this.Items[index] = (Item) null;
  }

  public void resetFriendshipsForNewDay()
  {
    foreach (string key in this.friendshipData.Keys)
    {
      bool flag = false;
      NPC n = Game1.getCharacterFromName(key) ?? (NPC) Game1.getCharacterFromName<Child>(key, false);
      if (n != null)
      {
        if (n != null && n.datable.Value && !this.friendshipData[key].IsDating() && !n.isMarried())
          flag = true;
        if (this.spouse != null && key == this.spouse && !this.hasPlayerTalkedToNPC(key))
          this.changeFriendship(-20, n);
        else if (n != null && this.friendshipData[key].IsDating() && !this.hasPlayerTalkedToNPC(key) && this.friendshipData[key].Points < 2500)
          this.changeFriendship(-8, n);
        if (this.hasPlayerTalkedToNPC(key))
          this.friendshipData[key].TalkedToToday = false;
        else if (!flag && this.friendshipData[key].Points < 2500 || flag && this.friendshipData[key].Points < 2000)
          this.changeFriendship(-2, n);
      }
    }
    this.updateFriendshipGifts(Game1.Date);
  }

  public virtual int GetAppliedMagneticRadius() => Math.Max(128 /*0x80*/, this.MagneticRadius);

  public void updateFriendshipGifts(WorldDate date)
  {
    foreach (string key in this.friendshipData.Keys)
    {
      int totalDays1 = date.TotalDays;
      int? totalDays2 = this.friendshipData[key].LastGiftDate?.TotalDays;
      int valueOrDefault1 = totalDays2.GetValueOrDefault();
      if (!(totalDays1 == valueOrDefault1 & totalDays2.HasValue))
        this.friendshipData[key].GiftsToday = 0;
      int totalSundayWeeks1 = date.TotalSundayWeeks;
      int? totalSundayWeeks2 = this.friendshipData[key].LastGiftDate?.TotalSundayWeeks;
      int valueOrDefault2 = totalSundayWeeks2.GetValueOrDefault();
      if (!(totalSundayWeeks1 == valueOrDefault2 & totalSundayWeeks2.HasValue))
      {
        if (this.friendshipData[key].GiftsThisWeek >= 2)
          this.changeFriendship(10, Game1.getCharacterFromName(key));
        this.friendshipData[key].GiftsThisWeek = 0;
      }
    }
  }

  public bool hasPlayerTalkedToNPC(string name)
  {
    Friendship friendship;
    if (!this.friendshipData.TryGetValue(name, out friendship) && NPC.CanSocializePerData(name, this.currentLocation))
      this.friendshipData[name] = friendship = new Friendship();
    return friendship != null && friendship.TalkedToToday;
  }

  public void fuelLantern(int units)
  {
    Tool toolFromName = this.getToolFromName("Lantern");
    if (toolFromName == null)
      return;
    ((Lantern) toolFromName).fuelLeft = Math.Min(100, ((Lantern) toolFromName).fuelLeft + units);
  }

  public bool IsEquippedItem(Item item)
  {
    if (item != null)
    {
      foreach (Item equippedItem in this.GetEquippedItems())
      {
        if (equippedItem == item)
          return true;
      }
    }
    return false;
  }

  public IEnumerable<Item> GetEquippedItems()
  {
    return ((IEnumerable<Item>) new Item[7]
    {
      (Item) this.CurrentTool,
      (Item) this.hat.Value,
      (Item) this.shirtItem.Value,
      (Item) this.pantsItem.Value,
      (Item) this.boots.Value,
      (Item) this.leftRing.Value,
      (Item) this.rightRing.Value
    }).Where<Item>((Func<Item, bool>) (item => item != null));
  }

  public override bool collideWith(Object o)
  {
    base.collideWith(o);
    if (this.isRidingHorse() && o is Fence)
    {
      this.mount.squeezeForGate();
      switch (this.FacingDirection)
      {
        case 1:
          if ((double) o.tileLocation.X < (double) this.Tile.X)
            return false;
          break;
        case 3:
          if ((double) o.tileLocation.X > (double) this.Tile.X)
            return false;
          break;
      }
    }
    return true;
  }

  public void changeIntoSwimsuit()
  {
    this.bathingClothes.Value = true;
    this.Halt();
    this.setRunning(false);
    this.canOnlyWalk = true;
  }

  public void changeOutOfSwimSuit()
  {
    this.bathingClothes.Value = false;
    this.canOnlyWalk = false;
    this.Halt();
    this.FarmerSprite.StopAnimation();
    if (!Game1.options.autoRun)
      return;
    this.setRunning(true);
  }

  public void showFrame(int frame, bool flip = false)
  {
    this.FarmerSprite.setCurrentAnimation(new FarmerSprite.AnimationFrame[1]
    {
      new FarmerSprite.AnimationFrame(Convert.ToInt32(frame), 100, false, flip)
    });
    this.FarmerSprite.loop = true;
    this.FarmerSprite.PauseForSingleAnimation = true;
    this.Sprite.currentFrame = Convert.ToInt32(frame);
  }

  public void stopShowingFrame()
  {
    this.FarmerSprite.loop = false;
    this.FarmerSprite.PauseForSingleAnimation = false;
    this.completelyStopAnimatingOrDoingAction();
  }

  /// <summary>Add an item to the player's inventory if there's room for it.</summary>
  /// <param name="item">The item to add.</param>
  /// <returns>If the item was fully added to the inventory, returns <c>null</c>. Else returns the input item with its stack reduced to the amount that couldn't be added.</returns>
  public Item addItemToInventory(Item item) => this.addItemToInventory(item, (List<Item>) null);

  /// <summary>Add an item to the player's inventory if there's room for it.</summary>
  /// <param name="item">The item to add.</param>
  /// <param name="affected_items_list">A list to update with the inventory item stacks it was merged into, or <c>null</c> to ignore it.</param>
  /// <returns>If the item was fully added to the inventory, returns <c>null</c>. Else returns the input item with its stack reduced to the amount that couldn't be added.</returns>
  public Item addItemToInventory(Item item, List<Item> affected_items_list)
  {
    if (item == null)
      return (Item) null;
    bool needsInventorySpace;
    this.GetItemReceiveBehavior(item, out needsInventorySpace, out bool _);
    if (!needsInventorySpace)
    {
      this.OnItemReceived(item, item.Stack, (Item) null);
      return (Item) null;
    }
    int stack1 = item.Stack;
    int num1 = stack1;
    foreach (Item obj in this.Items)
    {
      if (item.canStackWith((ISalable) obj))
      {
        int stack2 = item.Stack;
        num1 = obj.addToStack(item);
        int num2 = num1;
        int countAdded = stack2 - num2;
        if (countAdded > 0)
        {
          item.Stack = num1;
          this.OnItemReceived(item, countAdded, obj, true);
          affected_items_list?.Add(obj);
          if (num1 < 1)
            break;
        }
      }
    }
    if (num1 > 0)
    {
      for (int index = 0; index < this.maxItems.Value && index < this.Items.Count; ++index)
      {
        if (this.Items[index] == null)
        {
          item.onDetachedFromParent();
          this.Items[index] = item;
          num1 = 0;
          this.OnItemReceived(item, item.Stack, (Item) null, true);
          if (affected_items_list != null)
          {
            // ISSUE: explicit non-virtual call
            __nonvirtual (affected_items_list.Add(this.Items[index]));
            break;
          }
          break;
        }
      }
    }
    if (stack1 > num1)
      this.ShowItemReceivedHudMessageIfNeeded(item, stack1 - num1);
    return num1 <= 0 ? (Item) null : item;
  }

  /// <summary>Add an item to the player's inventory at a specific index position. If there's already an item at that position, the stacks are merged (if possible) else they're swapped.</summary>
  /// <param name="item">The item to add.</param>
  /// <param name="position">The index position within the list at which to add the item.</param>
  /// <returns>If the item was fully added to the inventory, returns <c>null</c>. If it replaced an item stack previously at that position, returns the replaced item stack. Else returns the input item with its stack reduced to the amount that couldn't be added.</returns>
  public Item addItemToInventory(Item item, int position)
  {
    if (item == null)
      return (Item) null;
    bool needsInventorySpace;
    this.GetItemReceiveBehavior(item, out needsInventorySpace, out bool _);
    if (!needsInventorySpace)
    {
      this.OnItemReceived(item, item.Stack, (Item) null);
      return (Item) null;
    }
    if (position >= 0 && position < this.Items.Count)
    {
      if (this.Items[position] == null)
      {
        this.Items[position] = item;
        this.OnItemReceived(item, item.Stack, (Item) null);
        return (Item) null;
      }
      if (this.Items[position].canStackWith((ISalable) item))
      {
        int stack1 = item.Stack;
        int stack2 = this.Items[position].addToStack(item);
        int num = stack2;
        int countAdded = stack1 - num;
        if (countAdded > 0)
        {
          item.Stack = stack2;
          this.OnItemReceived(item, countAdded, this.Items[position]);
          return stack2 <= 0 ? (Item) null : item;
        }
      }
      else
      {
        Item inventory = this.Items[position];
        this.Items[position] = item;
        this.OnItemReceived(item, item.Stack, (Item) null);
        return inventory;
      }
    }
    return item;
  }

  /// <summary>Add an item to the player's inventory if there's room for it.</summary>
  /// <param name="item">The item to add.</param>
  /// <param name="makeActiveObject">Legacy option which may behave in unexpected ways; shouldn't be used by most code.</param>
  /// <returns>Returns whether the item was at least partially added to the inventory. The number of items added will be deducted from the <paramref name="item" />'s <see cref="P:StardewValley.Item.Stack" />.</returns>
  public bool addItemToInventoryBool(Item item, bool makeActiveObject = false)
  {
    if (item == null || !this.IsLocalPlayer)
      return false;
    Item obj1 = (Item) null;
    bool needsInventorySpace;
    this.GetItemReceiveBehavior(item, out needsInventorySpace, out bool _);
    if (needsInventorySpace)
      obj1 = this.addItemToInventory(item);
    else
      this.OnItemReceived(item, item.Stack, (Item) null);
    int? stack1 = obj1?.Stack;
    int stack2 = item.Stack;
    bool inventoryBool = !(stack1.GetValueOrDefault() == stack2 & stack1.HasValue) || item is SpecialItem;
    if (makeActiveObject & inventoryBool && !(item is SpecialItem) && obj1 != null && item.Stack <= 1)
    {
      int indexOfInventoryItem = this.getIndexOfInventoryItem(item);
      if (indexOfInventoryItem > -1)
      {
        Item obj2 = this.Items[this.currentToolIndex.Value];
        this.Items[this.currentToolIndex.Value] = this.Items[indexOfInventoryItem];
        this.Items[indexOfInventoryItem] = obj2;
      }
    }
    return inventoryBool;
  }

  /// <summary>Add an item to the player's inventory if there's room for it, then show an animation of the player holding up the item above their head. If the item can't be fully added to the player's inventory, show (or queue) an item-grab menu to let the player collect the remainder.</summary>
  /// <param name="item">The item to add.</param>
  /// <param name="itemSelectedCallback">The callback to invoke when the item is added to the player's inventory.</param>
  /// <param name="forceQueue">For any remainder that can't be added to the inventory directly, whether to add the item-grab menu to <see cref="F:StardewValley.Game1.nextClickableMenu" /> even if there's no active menu currently open.</param>
  public void addItemByMenuIfNecessaryElseHoldUp(
    Item item,
    ItemGrabMenu.behaviorOnItemSelect itemSelectedCallback = null,
    bool forceQueue = false)
  {
    int stack = item.Stack;
    this.mostRecentlyGrabbedItem = item;
    this.addItemsByMenuIfNecessary(new List<Item>() { item }, itemSelectedCallback, forceQueue);
    if (Game1.activeClickableMenu != null || !(item?.QualifiedItemId != "(O)434"))
      return;
    this.holdUpItemThenMessage(item, stack);
  }

  /// <summary>Add an item to the player's inventory if there's room for it. If the item can't be fully added to the player's inventory, show (or queue) an item-grab menu to let the player collect the remainder.</summary>
  /// <param name="item">The item to add.</param>
  /// <param name="itemSelectedCallback">The callback to invoke when the item is added to the player's inventory.</param>
  /// <param name="forceQueue">For any remainder that can't be added to the inventory directly, whether to add the item-grab menu to <see cref="F:StardewValley.Game1.nextClickableMenu" /> even if there's no active menu currently open.</param>
  public void addItemByMenuIfNecessary(
    Item item,
    ItemGrabMenu.behaviorOnItemSelect itemSelectedCallback = null,
    bool forceQueue = false)
  {
    this.addItemsByMenuIfNecessary(new List<Item>() { item }, itemSelectedCallback, forceQueue);
  }

  /// <summary>Add items to the player's inventory if there's room for them. If the items can't be fully added to the player's inventory, show (or queue) an item-grab menu to let the player collect the remainder.</summary>
  /// <param name="itemsToAdd">The items to add.</param>
  /// <param name="itemSelectedCallback">The callback to invoke when an item is added to the player's inventory.</param>
  /// <param name="forceQueue">For any items that can't be added to the inventory directly, whether to add the item-grab menu to <see cref="F:StardewValley.Game1.nextClickableMenu" /> even if there's no active menu currently open.</param>
  public void addItemsByMenuIfNecessary(
    List<Item> itemsToAdd,
    ItemGrabMenu.behaviorOnItemSelect itemSelectedCallback = null,
    bool forceQueue = false)
  {
    if (itemsToAdd == null || !this.IsLocalPlayer)
      return;
    if (itemsToAdd.Count > 0 && itemsToAdd[0]?.QualifiedItemId == "(O)434")
    {
      if (Game1.activeClickableMenu == null && !forceQueue)
        this.eatObject(itemsToAdd[0] as Object, true);
      else
        Game1.nextClickableMenu.Add((IClickableMenu) ItemGrabMenu.CreateOverflowMenu((IList<Item>) itemsToAdd));
    }
    else
    {
      for (int index = itemsToAdd.Count - 1; index >= 0; --index)
      {
        if (this.addItemToInventoryBool(itemsToAdd[index]))
        {
          if (itemSelectedCallback != null)
            itemSelectedCallback(itemsToAdd[index], this);
          itemsToAdd.Remove(itemsToAdd[index]);
        }
      }
      if (itemsToAdd.Count > 0 && (forceQueue || Game1.activeClickableMenu != null))
      {
        for (int index1 = 0; index1 < Game1.nextClickableMenu.Count; ++index1)
        {
          if (Game1.nextClickableMenu[index1] is ItemGrabMenu itemGrabMenu && itemGrabMenu.source == 4)
          {
            IList<Item> actualInventory = itemGrabMenu.ItemsToGrabMenu.actualInventory;
            int capacity = itemGrabMenu.ItemsToGrabMenu.capacity;
            bool flag = false;
            for (int index2 = 0; index2 < itemsToAdd.Count; ++index2)
            {
              Item i = itemsToAdd[index2];
              int stack1 = i.Stack;
              Item thisInventoryList;
              itemsToAdd[index2] = thisInventoryList = Utility.addItemToThisInventoryList(i, actualInventory, capacity);
              int? stack2 = thisInventoryList?.Stack;
              int valueOrDefault = stack2.GetValueOrDefault();
              if (!(stack1 == valueOrDefault & stack2.HasValue))
              {
                flag = true;
                if (thisInventoryList == null)
                {
                  itemsToAdd.RemoveAt(index2);
                  --index2;
                }
              }
            }
            if (flag)
              Game1.nextClickableMenu[index1] = (IClickableMenu) ItemGrabMenu.CreateOverflowMenu(actualInventory);
          }
          if (itemsToAdd.Count == 0)
            break;
        }
      }
      if (itemsToAdd.Count <= 0)
        return;
      ItemGrabMenu overflowMenu = ItemGrabMenu.CreateOverflowMenu((IList<Item>) itemsToAdd);
      if (forceQueue || Game1.activeClickableMenu != null)
        Game1.nextClickableMenu.Add((IClickableMenu) overflowMenu);
      else
        Game1.activeClickableMenu = (IClickableMenu) overflowMenu;
    }
  }

  public virtual void BeginSitting(ISittable furniture)
  {
    if (furniture == null || this.bathingClothes.Value || this.swimming.Value || this.isRidingHorse() || !this.CanMove || this.UsingTool || this.IsEmoting)
      return;
    Vector2? nullable = furniture.AddSittingFarmer(this);
    if (!nullable.HasValue)
      return;
    this.playNearbySoundAll("woodyStep");
    this.Halt();
    this.synchronizedJump(4f);
    this.FarmerSprite.StopAnimation();
    this.sittingFurniture = furniture;
    this.mapChairSitPosition.Value = new Vector2(-1f, -1f);
    if (this.sittingFurniture is MapSeat)
    {
      Vector2? sittingPosition = this.sittingFurniture.GetSittingPosition(this, true);
      if (sittingPosition.HasValue)
        this.mapChairSitPosition.Value = sittingPosition.Value;
    }
    this.isSitting.Value = true;
    this.LerpPosition(this.Position, new Vector2(nullable.Value.X * 64f, nullable.Value.Y * 64f), 0.15f);
    this.freezePause += 100;
  }

  public virtual void LerpPosition(Vector2 start_position, Vector2 end_position, float duration)
  {
    this.freezePause = (int) ((double) duration * 1000.0);
    this.lerpStartPosition = start_position;
    this.lerpEndPosition = end_position;
    this.lerpPosition = 0.0f;
    this.lerpDuration = duration;
  }

  public virtual void StopSitting(bool animate = true)
  {
    if (this.sittingFurniture == null)
      return;
    ISittable sittingFurniture = this.sittingFurniture;
    if (!animate)
    {
      this.mapChairSitPosition.Value = new Vector2(-1f, -1f);
      sittingFurniture.RemoveSittingFarmer(this);
    }
    bool flag1 = false;
    bool flag2 = false;
    Vector2 position = this.Position;
    if (sittingFurniture.IsSeatHere(this.currentLocation))
    {
      flag1 = true;
      List<Vector2> list = new List<Vector2>();
      Vector2 vector2_1;
      ref Vector2 local = ref vector2_1;
      Microsoft.Xna.Framework.Rectangle seatBounds1 = sittingFurniture.GetSeatBounds();
      double left = (double) seatBounds1.Left;
      seatBounds1 = sittingFurniture.GetSeatBounds();
      double top = (double) seatBounds1.Top;
      local = new Vector2((float) left, (float) top);
      if (sittingFurniture.IsSittingHere(this))
        vector2_1 = sittingFurniture.GetSittingPosition(this, true).Value;
      if (sittingFurniture.GetSittingDirection() == 2)
      {
        list.Add(vector2_1 + new Vector2(0.0f, 1f));
        this.SortSeatExitPositions(list, vector2_1 + new Vector2(1f, 0.0f), vector2_1 + new Vector2(-1f, 0.0f), vector2_1 + new Vector2(0.0f, -1f));
      }
      else if (sittingFurniture.GetSittingDirection() == 1)
      {
        list.Add(vector2_1 + new Vector2(1f, 0.0f));
        this.SortSeatExitPositions(list, vector2_1 + new Vector2(0.0f, -1f), vector2_1 + new Vector2(0.0f, 1f), vector2_1 + new Vector2(-1f, 0.0f));
      }
      else if (sittingFurniture.GetSittingDirection() == 3)
      {
        list.Add(vector2_1 + new Vector2(-1f, 0.0f));
        this.SortSeatExitPositions(list, vector2_1 + new Vector2(0.0f, 1f), vector2_1 + new Vector2(0.0f, -1f), vector2_1 + new Vector2(1f, 0.0f));
      }
      else if (sittingFurniture.GetSittingDirection() == 0)
      {
        list.Add(vector2_1 + new Vector2(0.0f, -1f));
        this.SortSeatExitPositions(list, vector2_1 + new Vector2(-1f, 0.0f), vector2_1 + new Vector2(1f, 0.0f), vector2_1 + new Vector2(0.0f, 1f));
      }
      Microsoft.Xna.Framework.Rectangle seatBounds2 = sittingFurniture.GetSeatBounds();
      seatBounds2.Inflate(1, 1);
      foreach (Vector2 vector2_2 in Utility.getBorderOfThisRectangle(seatBounds2))
        list.Add(vector2_2);
      foreach (Vector2 tileLocation in list)
      {
        this.setTileLocation(tileLocation);
        Microsoft.Xna.Framework.Rectangle boundingBox = this.GetBoundingBox();
        this.Position = position;
        Object objectAtTile = this.currentLocation.getObjectAtTile((int) tileLocation.X, (int) tileLocation.Y, true);
        if (!this.currentLocation.isCollidingPosition(boundingBox, Game1.viewport, true, 0, false, (Character) this) && (objectAtTile == null || objectAtTile.isPassable()))
        {
          if (animate)
          {
            this.playNearbySoundAll("coin");
            this.synchronizedJump(4f);
            this.LerpPosition(vector2_1 * 64f, tileLocation * 64f, 0.15f);
          }
          flag2 = true;
          break;
        }
      }
    }
    if (!flag2)
    {
      if (animate)
        this.playNearbySoundAll("coin");
      this.Position = position;
      if (flag1)
      {
        Microsoft.Xna.Framework.Rectangle seatBounds = sittingFurniture.GetSeatBounds();
        seatBounds.X *= 64 /*0x40*/;
        seatBounds.Y *= 64 /*0x40*/;
        seatBounds.Width *= 64 /*0x40*/;
        seatBounds.Height *= 64 /*0x40*/;
        this.temporaryPassableTiles.Add(seatBounds);
      }
    }
    if (!animate)
    {
      this.sittingFurniture = (ISittable) null;
      this.isSitting.Value = false;
      this.Halt();
      this.showNotCarrying();
    }
    else
      this.isStopSitting = true;
    Game1.haltAfterCheck = false;
    this.yOffset = 0.0f;
    this.xOffset = 0.0f;
  }

  public void SortSeatExitPositions(List<Vector2> list, Vector2 a, Vector2 b, Vector2 c)
  {
    Vector2 mouse_pos = Utility.PointToVector2(Game1.getMousePosition(false)) + new Vector2((float) Game1.viewport.X, (float) Game1.viewport.Y);
    Vector2 zero = Vector2.Zero;
    GamePadState gamePadState;
    GamePadThumbSticks thumbSticks;
    if (!Game1.isOneOfTheseKeysDown(Game1.input.GetKeyboardState(), Game1.options.moveUpButton))
    {
      if (Game1.options.gamepadControls)
      {
        gamePadState = Game1.input.GetGamePadState();
        thumbSticks = gamePadState.ThumbSticks;
        if ((double) thumbSticks.Left.Y <= 0.25)
        {
          gamePadState = Game1.input.GetGamePadState();
          if (gamePadState.IsButtonDown(Buttons.DPadUp))
            goto label_4;
        }
        else
          goto label_4;
      }
      if (!Game1.isOneOfTheseKeysDown(Game1.input.GetKeyboardState(), Game1.options.moveDownButton))
      {
        if (Game1.options.gamepadControls)
        {
          gamePadState = Game1.input.GetGamePadState();
          thumbSticks = gamePadState.ThumbSticks;
          if ((double) thumbSticks.Left.Y >= -0.25)
          {
            gamePadState = Game1.input.GetGamePadState();
            if (!gamePadState.IsButtonDown(Buttons.DPadDown))
              goto label_10;
          }
        }
        else
          goto label_10;
      }
      ++zero.Y;
      goto label_10;
    }
label_4:
    --zero.Y;
label_10:
    if (!Game1.isOneOfTheseKeysDown(Game1.input.GetKeyboardState(), Game1.options.moveLeftButton))
    {
      if (Game1.options.gamepadControls)
      {
        gamePadState = Game1.input.GetGamePadState();
        thumbSticks = gamePadState.ThumbSticks;
        if ((double) thumbSticks.Left.X >= -0.25)
        {
          gamePadState = Game1.input.GetGamePadState();
          if (gamePadState.IsButtonDown(Buttons.DPadLeft))
            goto label_14;
        }
        else
          goto label_14;
      }
      if (!Game1.isOneOfTheseKeysDown(Game1.input.GetKeyboardState(), Game1.options.moveRightButton))
      {
        if (Game1.options.gamepadControls)
        {
          gamePadState = Game1.input.GetGamePadState();
          thumbSticks = gamePadState.ThumbSticks;
          if ((double) thumbSticks.Left.X <= 0.25)
          {
            gamePadState = Game1.input.GetGamePadState();
            if (!gamePadState.IsButtonDown(Buttons.DPadRight))
              goto label_20;
          }
        }
        else
          goto label_20;
      }
      ++zero.X;
      goto label_20;
    }
label_14:
    --zero.X;
label_20:
    if (zero != Vector2.Zero)
      mouse_pos = this.getStandingPosition() + zero * 64f;
    mouse_pos /= 64f;
    List<Vector2> collection = new List<Vector2>()
    {
      a,
      b,
      c
    };
    collection.Sort((Comparison<Vector2>) ((d, e) => (d + new Vector2(0.5f, 0.5f) - mouse_pos).Length().CompareTo((e + new Vector2(0.5f, 0.5f) - mouse_pos).Length())));
    list.AddRange((IEnumerable<Vector2>) collection);
  }

  public virtual bool IsSitting() => this.isSitting.Value;

  public bool isInventoryFull()
  {
    for (int index = 0; index < this.maxItems.Value; ++index)
    {
      if (this.Items.Count > index && this.Items[index] == null)
        return false;
    }
    return true;
  }

  public bool couldInventoryAcceptThisItem(Item item)
  {
    if (item == null)
      return false;
    if (item.IsRecipe)
      return true;
    string qualifiedItemId = item.QualifiedItemId;
    if (qualifiedItemId == "(O)73" || qualifiedItemId == "(O)930" || qualifiedItemId == "(O)102" || qualifiedItemId == "(O)858" || qualifiedItemId == "(O)GoldCoin")
      return true;
    for (int index = 0; index < this.maxItems.Value; ++index)
    {
      if (this.Items.Count > index && (this.Items[index] == null || item is Object && this.Items[index] is Object && this.Items[index].Stack + item.Stack <= this.Items[index].maximumStackSize() && (this.Items[index] as Object).canStackWith((ISalable) item)))
        return true;
    }
    if (this.IsLocalPlayer && this.isInventoryFull() && Game1.hudMessages.Count == 0)
      Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Crop.cs.588"));
    return false;
  }

  public bool couldInventoryAcceptThisItem(string id, int stack, int quality = 0)
  {
    ParsedItemData dataOrErrorItem = ItemRegistry.GetDataOrErrorItem(id);
    string qualifiedItemId = dataOrErrorItem.QualifiedItemId;
    if (qualifiedItemId == "(O)73" || qualifiedItemId == "(O)930" || qualifiedItemId == "(O)102" || qualifiedItemId == "(O)858" || qualifiedItemId == "(O)GoldCoin")
      return true;
    for (int index = 0; index < this.maxItems.Value; ++index)
    {
      if (this.Items.Count > index && (this.Items[index] == null || this.Items[index].Stack + stack <= this.Items[index].maximumStackSize() && this.Items[index].QualifiedItemId == dataOrErrorItem.QualifiedItemId && this.Items[index].quality.Value == quality))
        return true;
    }
    if (this.IsLocalPlayer && this.isInventoryFull() && Game1.hudMessages.Count == 0)
      Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Crop.cs.588"));
    return false;
  }

  public NPC getSpouse()
  {
    return this.isMarriedOrRoommates() && this.spouse != null ? Game1.getCharacterFromName(this.spouse) : (NPC) null;
  }

  public int freeSpotsInInventory()
  {
    int num = this.Items.CountItemStacks();
    return num >= this.maxItems.Value ? 0 : this.maxItems.Value - num;
  }

  /// <summary>Get the behavior that applies when this item is received.</summary>
  /// <param name="item">The item being received.</param>
  /// <param name="needsInventorySpace">Whether this item takes space in the player inventory. This is false for special items like Qi Gems.</param>
  /// <param name="showNotification">Whether to show a HUD notification when the item is received.</param>
  public void GetItemReceiveBehavior(
    Item item,
    out bool needsInventorySpace,
    out bool showNotification)
  {
    if (item is SpecialItem)
    {
      needsInventorySpace = false;
      showNotification = false;
    }
    else
    {
      switch (item.QualifiedItemId)
      {
        case "(O)73":
        case "(O)102":
        case "(O)858":
          needsInventorySpace = false;
          showNotification = true;
          break;
        case "(O)GoldCoin":
        case "(O)930":
          needsInventorySpace = false;
          showNotification = false;
          break;
        default:
          needsInventorySpace = true;
          showNotification = true;
          break;
      }
      if (!item.IsRecipe)
        return;
      needsInventorySpace = false;
      showNotification = true;
    }
  }

  /// <summary>Handle an item being added to the current player's inventory.</summary>
  /// <param name="item">The item that was added. If <see cref="!:mergedIntoStack" /> is set, this is the original item rather than the one actually in the player's inventory.</param>
  /// <param name="countAdded">The number of the item that was added. This may differ from <paramref name="item" />'s stack size if it was only partly added or split across multiple stacks.</param>
  /// <param name="mergedIntoStack">The previous item stack it was merged into, if applicable.</param>
  /// <param name="hideHudNotification">Hide the 'item received' HUD notification even if it would normally be shown. This is used when merging the item into multiple stacks, so the HUD notification is shown once.</param>
  public void OnItemReceived(
    Item item,
    int countAdded,
    Item mergedIntoStack,
    bool hideHudNotification = false)
  {
    if (!this.IsLocalPlayer)
      return;
    if (item is Object object1)
      object1.reloadSprite();
    if (item.HasBeenInInventory)
      return;
    Item actualItem = mergedIntoStack ?? item;
    if (!hideHudNotification)
    {
      bool showNotification;
      this.GetItemReceiveBehavior(actualItem, out bool _, out showNotification);
      if (showNotification)
        this.ShowItemReceivedHudMessage(actualItem, countAdded);
    }
    if (this.freezePause <= 0)
      this.mostRecentlyGrabbedItem = actualItem;
    if (item.SetFlagOnPickup != null)
    {
      if (!this.hasOrWillReceiveMail(item.SetFlagOnPickup))
        Game1.addMail(item.SetFlagOnPickup, true);
      actualItem.SetFlagOnPickup = (string) null;
    }
    if (actualItem is SpecialItem specialItem)
      specialItem.actionWhenReceived(this);
    if (actualItem is Object object2 && object2.specialItem)
    {
      string str = object2.IsRecipe ? "-" + object2.ItemId : object2.ItemId;
      if (object2.bigCraftable.Value || object2 is Furniture)
      {
        if (!this.specialBigCraftables.Contains(str))
          this.specialBigCraftables.Add(str);
      }
      else if (!this.specialItems.Contains(str))
        this.specialItems.Add(str);
    }
    if (item.IsRecipe)
    {
      item.LearnRecipe();
      Game1.playSound("newRecipe");
    }
    else
    {
      int stack = actualItem.Stack;
      try
      {
        actualItem.Stack = countAdded;
        this.NotifyQuests((Func<Quest, bool>) (quest => quest.OnItemReceived(actualItem, countAdded)));
        if (this.team.specialOrders != null)
        {
          foreach (SpecialOrder specialOrder in this.team.specialOrders)
          {
            Action<Farmer, Item> onItemCollected = specialOrder.onItemCollected;
            if (onItemCollected != null)
              onItemCollected(this, actualItem);
          }
        }
      }
      finally
      {
        actualItem.Stack = stack;
      }
      if (actualItem.HasTypeObject() && actualItem is Object object3)
      {
        if (object3.Category == -2 || object3.Type == "Minerals")
          this.foundMineral(object3.ItemId);
        else if (object3.Type == "Arch")
          this.foundArtifact(object3.ItemId, 1);
      }
      this.stats.checkForHeldItemAchievements();
      string qualifiedItemId = actualItem.QualifiedItemId;
      if (qualifiedItemId != null)
      {
        switch (qualifiedItemId.Length)
        {
          case 5:
            switch (qualifiedItemId[4])
            {
              case '2':
                if (qualifiedItemId == "(O)72")
                {
                  ++this.stats.DiamondsFound;
                  break;
                }
                break;
              case '3':
                if (qualifiedItemId == "(O)73")
                {
                  this.foundWalnut(countAdded);
                  this.removeItemFromInventory(actualItem);
                  break;
                }
                break;
              case '4':
                if (qualifiedItemId == "(O)74")
                {
                  ++this.stats.PrismaticShardsFound;
                  break;
                }
                break;
            }
            break;
          case 6:
            switch (qualifiedItemId[3])
            {
              case '1':
                if (qualifiedItemId == "(O)102")
                {
                  Game1.PerformActionWhenPlayerFree((Action) (() => this.foundArtifact(actualItem.ItemId, 1)));
                  this.removeItemFromInventory(actualItem);
                  ++this.stats.NotesFound;
                  break;
                }
                break;
              case '3':
                switch (qualifiedItemId)
                {
                  case "(O)390":
                    ++this.stats.StoneGathered;
                    if (this.stats.StoneGathered >= 100U && !this.hasOrWillReceiveMail("robinWell"))
                    {
                      Game1.addMailForTomorrow("robinWell");
                      break;
                    }
                    break;
                  case "(O)384":
                    this.stats.GoldFound += (uint) countAdded;
                    break;
                  case "(O)380":
                    this.stats.IronFound += (uint) countAdded;
                    break;
                  case "(O)386":
                    this.stats.IridiumFound += (uint) countAdded;
                    break;
                  case "(O)378":
                    this.stats.CopperFound += (uint) countAdded;
                    if (!this.hasOrWillReceiveMail("copperFound"))
                    {
                      Game1.addMailForTomorrow("copperFound", true);
                      break;
                    }
                    break;
                }
                break;
              case '4':
                if (qualifiedItemId == "(O)428" && !this.hasOrWillReceiveMail("clothFound"))
                {
                  Game1.addMailForTomorrow("clothFound", true);
                  break;
                }
                break;
              case '5':
                if (qualifiedItemId == "(O)535")
                {
                  Game1.PerformActionWhenPlayerFree((Action) (() =>
                  {
                    if (this.hasOrWillReceiveMail("geodeFound"))
                      return;
                    this.mailReceived.Add("geodeFound");
                    this.holdUpItemThenMessage(actualItem);
                  }));
                  break;
                }
                break;
              case '8':
                switch (qualifiedItemId)
                {
                  case "(O)858":
                    this.QiGems += countAdded;
                    Game1.playSound("qi_shop_purchase");
                    this.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("Maps\\springobjects", Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, 858, 16 /*0x10*/, 16 /*0x10*/), 100f, 1, 8, new Vector2(0.0f, -96f), false, false, 1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
                    {
                      motion = new Vector2(0.0f, -6f),
                      acceleration = new Vector2(0.0f, 0.2f),
                      stopAcceleratingWhenVelocityIsZero = true,
                      attachedCharacter = (Character) this,
                      positionFollowsAttachedCharacter = true
                    });
                    this.removeItemFromInventory(actualItem);
                    break;
                  case "(O)875":
                    if (!Game1.MasterPlayer.hasOrWillReceiveMail("ectoplasmDrop") && this.team.SpecialOrderActive("Wizard"))
                    {
                      Game1.addMailForTomorrow("ectoplasmDrop", true, true);
                      break;
                    }
                    break;
                  case "(O)876":
                    if (!Game1.MasterPlayer.hasOrWillReceiveMail("prismaticJellyDrop") && this.team.SpecialOrderActive("Wizard2"))
                    {
                      Game1.addMailForTomorrow("prismaticJellyDrop", true, true);
                      break;
                    }
                    break;
                  case "(O)897":
                    if (!Game1.MasterPlayer.hasOrWillReceiveMail("gotMissingStocklist"))
                    {
                      Game1.addMailForTomorrow("gotMissingStocklist", true, true);
                      break;
                    }
                    break;
                }
                break;
              case '9':
                if (qualifiedItemId == "(O)930")
                {
                  int number = 10 * countAdded;
                  this.health = Math.Min(this.maxHealth, this.health + number);
                  this.currentLocation.debris.Add(new Debris(number, this.getStandingPosition(), Color.Lime, 1f, (Character) this));
                  Game1.playSound("healSound");
                  this.removeItemFromInventory(actualItem);
                  break;
                }
                break;
            }
            break;
          case 7:
            switch (qualifiedItemId[5])
            {
              case '4':
                if (qualifiedItemId == "(BC)248")
                {
                  ++Game1.netWorldState.Value.MiniShippingBinsObtained;
                  break;
                }
                break;
              case '5':
                if (qualifiedItemId == "(BC)256" && !Game1.MasterPlayer.hasOrWillReceiveMail("gotFirstJunimoChest"))
                {
                  Game1.addMailForTomorrow("gotFirstJunimoChest", true, true);
                  break;
                }
                break;
            }
            break;
          case 11:
            if (qualifiedItemId == "(O)GoldCoin")
            {
              Game1.playSound("moneyDial");
              int amount = 250 * countAdded;
              if (Game1.IsSpring && Game1.dayOfMonth == 17 && this.currentLocation is Forest && (double) this.Tile.Y > 90.0)
                amount = 25;
              this.Money += amount;
              this.removeItemFromInventory(item);
              Game1.dayTimeMoneyBox.gotGoldCoin(amount);
              break;
            }
            break;
        }
      }
      actualItem.HasBeenInInventory = true;
    }
  }

  /// <summary>Show the item-received HUD message for an item if applicable for the item type.</summary>
  /// <param name="item">The item that was added.</param>
  /// <param name="countAdded">The number of the item that was added. This may differ from <paramref name="item" />'s stack size if it was only partly added or split across multiple stacks.</param>
  public void ShowItemReceivedHudMessageIfNeeded(Item item, int countAdded)
  {
    bool showNotification;
    this.GetItemReceiveBehavior(item, out bool _, out showNotification);
    if (!showNotification)
      return;
    this.ShowItemReceivedHudMessage(item, countAdded);
  }

  /// <summary>Show the item-received HUD message for an item.</summary>
  /// <param name="item">The item that was added.</param>
  /// <param name="countAdded">The number of the item that was added. This may differ from <paramref name="item" />'s stack size if it was only partly added or split across multiple stacks.</param>
  public void ShowItemReceivedHudMessage(Item item, int countAdded)
  {
    if (Game1.activeClickableMenu != null && Game1.activeClickableMenu is ItemGrabMenu)
      return;
    Game1.addHUDMessage(HUDMessage.ForItemGained(item, countAdded));
  }

  public int getIndexOfInventoryItem(Item item)
  {
    for (int index = 0; index < this.Items.Count; ++index)
    {
      if (this.Items[index] == item || this.Items[index] != null && item != null && item.canStackWith((ISalable) this.Items[index]))
        return index;
    }
    return -1;
  }

  public void reduceActiveItemByOne()
  {
    if (this.CurrentItem == null || --this.CurrentItem.Stack > 0)
      return;
    this.removeItemFromInventory(this.CurrentItem);
    this.showNotCarrying();
  }

  public void ReequipEnchantments()
  {
    Tool currentTool = this.CurrentTool;
    if (currentTool == null)
      return;
    foreach (BaseEnchantment enchantment in currentTool.enchantments)
      enchantment.OnEquip(this);
  }

  public void removeItemFromInventory(Item which)
  {
    if (which == null)
      return;
    int index = this.Items.IndexOf(which);
    if (index < 0 || index >= this.Items.Count)
      return;
    this.Items[index].actionWhenStopBeingHeld(this);
    this.Items[index] = (Item) null;
  }

  /// <summary>Get whether the player is married to or roommates with an NPC or player.</summary>
  public bool isMarriedOrRoommates()
  {
    if (this.team.IsMarried(this.UniqueMultiplayerID))
      return true;
    Friendship friendship;
    return this.spouse != null && this.friendshipData.TryGetValue(this.spouse, out friendship) && friendship.IsMarried();
  }

  public bool isEngaged()
  {
    if (this.team.IsEngaged(this.UniqueMultiplayerID))
      return true;
    Friendship friendship;
    return this.spouse != null && this.friendshipData.TryGetValue(this.spouse, out friendship) && friendship.IsEngaged();
  }

  public void removeFirstOfThisItemFromInventory(string itemId, int count = 1)
  {
    itemId = ItemRegistry.QualifyItemId(itemId);
    if (itemId == null)
      return;
    int num = count;
    if (this.ActiveObject?.QualifiedItemId == itemId)
    {
      int amount = Math.Min(num, this.ActiveObject.Stack);
      num -= amount;
      if (this.ActiveObject.ConsumeStack(amount) == null)
      {
        this.ActiveObject = (Object) null;
        this.showNotCarrying();
      }
    }
    if (num <= 0)
      return;
    this.Items.ReduceId(itemId, num);
  }

  public void rotateShirt(int direction, List<string> validIds = null)
  {
    string str = this.shirt.Value;
    if (validIds == null)
      validIds = new List<string>((IEnumerable<string>) Game1.shirtData.Keys);
    int num = validIds.IndexOf(str);
    if (num == -1)
    {
      string itemId = validIds.FirstOrDefault<string>();
      if (itemId == null)
        return;
      this.changeShirt(itemId);
    }
    else
    {
      int index = Utility.WrapIndex(num + direction, validIds.Count);
      this.changeShirt(validIds[index]);
    }
  }

  public void changeShirt(string itemId)
  {
    this.shirt.Set(itemId);
    this.FarmerRenderer.changeShirt(itemId);
  }

  public void rotatePantStyle(int direction, List<string> validIds = null)
  {
    string str = this.pants.Value;
    if (validIds == null)
      validIds = new List<string>((IEnumerable<string>) Game1.pantsData.Keys);
    int num = validIds.IndexOf(str);
    if (num == -1)
    {
      string itemId = validIds.FirstOrDefault<string>();
      if (itemId == null)
        return;
      this.changePantStyle(itemId);
    }
    else
    {
      int index = Utility.WrapIndex(num + direction, validIds.Count);
      this.changePantStyle(validIds[index]);
    }
  }

  public void changePantStyle(string itemId)
  {
    this.pants.Set(itemId);
    this.FarmerRenderer.changePants(itemId);
  }

  public void ConvertClothingOverrideToClothesItems()
  {
    string id1;
    Color? color;
    if (this.IsOverridingPants(out id1, out color))
    {
      if (ItemRegistry.Exists("(P)" + id1))
      {
        Clothing newItem = new Clothing(id1);
        newItem.clothesColor.Value = color ?? Color.White;
        this.Equip<Clothing>(newItem, this.pantsItem);
      }
      this.pants.Value = "-1";
    }
    string id2;
    if (!this.IsOverridingShirt(out id2))
      return;
    int result;
    if (int.TryParse(id2, out result) && result < 1000)
      id2 = (result + 1000).ToString();
    if (ItemRegistry.Exists("(S)" + id2))
      this.Equip<Clothing>(new Clothing(id2), this.shirtItem);
    this.shirt.Value = "-1";
  }

  public static Dictionary<int, string> GetHairStyleMetadataFile()
  {
    if (Farmer.hairStyleMetadataFile == null)
      Farmer.hairStyleMetadataFile = DataLoader.HairData(Game1.content);
    return Farmer.hairStyleMetadataFile;
  }

  public static HairStyleMetadata GetHairStyleMetadata(int hair_index)
  {
    Farmer.GetHairStyleMetadataFile();
    HairStyleMetadata hairStyleMetadata1;
    if (Farmer.hairStyleMetadata.TryGetValue(hair_index, out hairStyleMetadata1))
      return hairStyleMetadata1;
    try
    {
      string str;
      if (Farmer.hairStyleMetadataFile.TryGetValue(hair_index, out str))
      {
        string[] strArray = str.Split('/');
        HairStyleMetadata hairStyleMetadata2 = new HairStyleMetadata();
        hairStyleMetadata2.texture = Game1.content.Load<Texture2D>("Characters\\Farmer\\" + strArray[0]);
        hairStyleMetadata2.tileX = int.Parse(strArray[1]);
        hairStyleMetadata2.tileY = int.Parse(strArray[2]);
        hairStyleMetadata2.usesUniqueLeftSprite = strArray.Length > 3 && strArray[3].EqualsIgnoreCase("true");
        if (strArray.Length > 4)
          hairStyleMetadata2.coveredIndex = int.Parse(strArray[4]);
        hairStyleMetadata2.isBaldStyle = strArray.Length > 5 && strArray[5].EqualsIgnoreCase("true");
        hairStyleMetadata1 = hairStyleMetadata2;
      }
    }
    catch (Exception ex)
    {
    }
    Farmer.hairStyleMetadata[hair_index] = hairStyleMetadata1;
    return hairStyleMetadata1;
  }

  public static List<int> GetAllHairstyleIndices()
  {
    if (Farmer.allHairStyleIndices != null)
      return Farmer.allHairStyleIndices;
    Farmer.GetHairStyleMetadataFile();
    Farmer.allHairStyleIndices = new List<int>();
    int num = FarmerRenderer.hairStylesTexture.Height / 96 /*0x60*/ * 8;
    for (int index = 0; index < num; ++index)
      Farmer.allHairStyleIndices.Add(index);
    foreach (int key in Farmer.hairStyleMetadataFile.Keys)
    {
      if (key >= 0 && !Farmer.allHairStyleIndices.Contains(key))
        Farmer.allHairStyleIndices.Add(key);
    }
    Farmer.allHairStyleIndices.Sort();
    return Farmer.allHairStyleIndices;
  }

  public static int GetLastHairStyle()
  {
    return Farmer.GetAllHairstyleIndices()[Farmer.GetAllHairstyleIndices().Count - 1];
  }

  public void changeHairStyle(int whichHair)
  {
    int num = this.isBald() ? 1 : 0;
    if (Farmer.GetHairStyleMetadata(whichHair) != null)
    {
      this.hair.Set(whichHair);
    }
    else
    {
      if (whichHair < 0)
        whichHair = Farmer.GetLastHairStyle();
      else if (whichHair > Farmer.GetLastHairStyle())
        whichHair = 0;
      this.hair.Set(whichHair);
    }
    if (this.IsBaldHairStyle(whichHair))
      this.FarmerRenderer.textureName.Set(this.getTexture());
    if (num == 0 || this.isBald())
      return;
    this.FarmerRenderer.textureName.Set(this.getTexture());
  }

  public virtual bool IsBaldHairStyle(int style)
  {
    if (Farmer.GetHairStyleMetadata(this.hair.Value) != null)
      return Farmer.GetHairStyleMetadata(this.hair.Value).isBaldStyle;
    switch (style)
    {
      case 49:
      case 50:
      case 51:
      case 52:
      case 53:
      case 54:
      case 55:
        return true;
      default:
        return false;
    }
  }

  private bool isBald() => this.IsBaldHairStyle(this.getHair());

  /// <summary>Change the color of the player's shoes.</summary>
  /// <param name="color">The new color to set.</param>
  public void changeShoeColor(string which)
  {
    this.FarmerRenderer.recolorShoes(which);
    this.shoes.Set(which);
  }

  /// <summary>Change the color of the player's hair.</summary>
  /// <param name="color">The new color to set.</param>
  public void changeHairColor(Color c) => this.hairstyleColor.Set(c);

  /// <summary>Change the color of the player's equipped pants.</summary>
  /// <param name="color">The new color to set.</param>
  public void changePantsColor(Color color)
  {
    this.pantsColor.Set(color);
    this.pantsItem.Value?.clothesColor.Set(color);
  }

  public void changeHat(int newHat)
  {
    if (newHat < 0)
      this.Equip<Hat>((Hat) null, this.hat);
    else
      this.Equip<Hat>(ItemRegistry.Create<Hat>("(H)" + newHat.ToString()), this.hat);
  }

  public void changeAccessory(int which)
  {
    if (which < -1)
      which = 29;
    if (which < -1)
      return;
    if (which >= 30)
      which = -1;
    this.accessory.Set(which);
  }

  public void changeSkinColor(int which, bool force = false)
  {
    if (which < 0)
      which = 23;
    else if (which >= 24)
      which = 0;
    this.skin.Set(this.FarmerRenderer.recolorSkin(which, force));
  }

  /// <summary>Whether this player has dark skin for the purposes of child genetics.</summary>
  public virtual bool hasDarkSkin()
  {
    return this.skin.Value >= 4 && this.skin.Value <= 8 && this.skin.Value != 7 || this.skin.Value == 14;
  }

  /// <summary>Change the color of the player's eyes.</summary>
  /// <param name="color">The new color to set.</param>
  public void changeEyeColor(Color c)
  {
    this.newEyeColor.Set(c);
    this.FarmerRenderer.recolorEyes(c);
  }

  public int getHair(bool ignore_hat = false)
  {
    if (this.hat.Value != null && !this.bathingClothes.Value && !ignore_hat)
    {
      switch ((Hat.HairDrawType) this.hat.Value.hairDrawType.Value)
      {
        case Hat.HairDrawType.DrawObscuredHair:
          switch (this.hair.Value)
          {
            case 1:
            case 5:
            case 6:
            case 9:
            case 11:
            case 17:
            case 20:
            case 23:
            case 24:
            case 25:
            case 27:
            case 28:
            case 29:
            case 30:
            case 32 /*0x20*/:
            case 33:
            case 34:
            case 36:
            case 39:
            case 41:
            case 43:
            case 44:
            case 45:
            case 46:
            case 47:
              return this.hair.Value;
            case 3:
              return 11;
            case 18:
            case 19:
            case 21:
            case 31 /*0x1F*/:
              return 23;
            case 42:
              return 46;
            case 48 /*0x30*/:
              return 6;
            case 49:
              return 52;
            case 50:
            case 51:
            case 52:
            case 53:
            case 54:
            case 55:
              return this.hair.Value;
            default:
              if (this.hair.Value < 16 /*0x10*/)
                return 7;
              return this.hair.Value < 100 ? 30 : this.hair.Value;
          }
        case Hat.HairDrawType.HideHair:
          return -1;
      }
    }
    return this.hair.Value;
  }

  public void changeGender(bool male)
  {
    if (male)
    {
      this.Gender = Gender.Male;
      this.FarmerRenderer.textureName.Set(this.getTexture());
      this.FarmerRenderer.heightOffset.Set(0);
    }
    else
    {
      this.Gender = Gender.Female;
      this.FarmerRenderer.heightOffset.Set(4);
      this.FarmerRenderer.textureName.Set(this.getTexture());
    }
    this.changeShirt(this.shirt.Value);
  }

  public void changeFriendship(int amount, NPC n)
  {
    if (n == null || !(n is Child) && !n.IsVillager)
      return;
    if (amount > 0 && this.stats.Get("Book_Friendship") > 0U)
      amount = (int) ((double) amount * 1.1000000238418579);
    if (amount > 0 && n.SpeaksDwarvish() && !this.canUnderstandDwarves)
      return;
    Friendship friendship;
    if (this.friendshipData.TryGetValue(n.Name, out friendship))
    {
      if (n.isDivorcedFrom(this) && amount > 0)
        return;
      if (n.Equals((object) this.getSpouse()))
        amount = (int) ((double) amount * 0.6600000262260437);
      friendship.Points = Math.Max(0, Math.Min(friendship.Points + amount, (Utility.GetMaximumHeartsForCharacter((Character) n) + 1) * 250 - 1));
      if (n.datable.Value && friendship.Points >= 2000 && !this.hasOrWillReceiveMail("Bouquet"))
        Game1.addMailForTomorrow("Bouquet");
      if (n.datable.Value && friendship.Points >= 2500 && !this.hasOrWillReceiveMail("SeaAmulet"))
        Game1.addMailForTomorrow("SeaAmulet");
      if (friendship.Points < 0)
        friendship.Points = 0;
    }
    else
      Game1.debugOutput = "Tried to change friendship for a friend that wasn't there.";
    Game1.stats.checkForFriendshipAchievements();
  }

  public bool knowsRecipe(string name)
  {
    if (name.EndsWith(" Recipe"))
      name = name.Substring(0, name.Length - " Recipe".Length);
    NetDictionary<string, int, NetInt, SerializableDictionary<string, int>, NetStringDictionary<int, NetInt>>.KeysCollection keys = this.craftingRecipes.Keys;
    if (keys.Contains(name))
      return true;
    keys = this.cookingRecipes.Keys;
    return keys.Contains(name);
  }

  public Vector2 getUniformPositionAwayFromBox(int direction, int distance)
  {
    Microsoft.Xna.Framework.Rectangle boundingBox = this.GetBoundingBox();
    switch (this.FacingDirection)
    {
      case 0:
        return new Vector2((float) boundingBox.Center.X, (float) (boundingBox.Y - distance));
      case 1:
        return new Vector2((float) (boundingBox.Right + distance), (float) boundingBox.Center.Y);
      case 2:
        return new Vector2((float) boundingBox.Center.X, (float) (boundingBox.Bottom + distance));
      case 3:
        return new Vector2((float) (boundingBox.X - distance), (float) boundingBox.Center.Y);
      default:
        return Vector2.Zero;
    }
  }

  public bool hasTalkedToFriendToday(string npcName)
  {
    Friendship friendship;
    return this.friendshipData.TryGetValue(npcName, out friendship) && friendship.TalkedToToday;
  }

  public void talkToFriend(NPC n, int friendshipPointChange = 20)
  {
    Friendship friendship;
    if (!this.friendshipData.TryGetValue(n.Name, out friendship) || friendship.TalkedToToday)
      return;
    this.changeFriendship(friendshipPointChange, n);
    friendship.TalkedToToday = true;
  }

  public void moveRaft(GameLocation currentLocation, GameTime time)
  {
    float num = 0.2f;
    if (this.CanMove && Game1.isOneOfTheseKeysDown(Game1.oldKBState, Game1.options.moveUpButton))
    {
      this.yVelocity = Math.Max(this.yVelocity - num, (float) ((double) Math.Abs(this.xVelocity) / 2.0 - 3.0));
      this.faceDirection(0);
    }
    if (this.CanMove && Game1.isOneOfTheseKeysDown(Game1.oldKBState, Game1.options.moveRightButton))
    {
      this.xVelocity = Math.Min(this.xVelocity + num, (float) (3.0 - (double) Math.Abs(this.yVelocity) / 2.0));
      this.faceDirection(1);
    }
    if (this.CanMove && Game1.isOneOfTheseKeysDown(Game1.oldKBState, Game1.options.moveDownButton))
    {
      this.yVelocity = Math.Min(this.yVelocity + num, (float) (3.0 - (double) Math.Abs(this.xVelocity) / 2.0));
      this.faceDirection(2);
    }
    if (this.CanMove && Game1.isOneOfTheseKeysDown(Game1.oldKBState, Game1.options.moveLeftButton))
    {
      this.xVelocity = Math.Max(this.xVelocity - num, (float) ((double) Math.Abs(this.yVelocity) / 2.0 - 3.0));
      this.faceDirection(3);
    }
    Microsoft.Xna.Framework.Rectangle position = new Microsoft.Xna.Framework.Rectangle((int) this.Position.X, (int) ((double) this.Position.Y + 64.0 + 16.0), 64 /*0x40*/, 64 /*0x40*/);
    position.X += (int) Math.Ceiling((double) this.xVelocity);
    if (!currentLocation.isCollidingPosition(position, Game1.viewport, (Character) this))
      this.position.X += this.xVelocity;
    position.X -= (int) Math.Ceiling((double) this.xVelocity);
    position.Y += (int) Math.Floor((double) this.yVelocity);
    if (!currentLocation.isCollidingPosition(position, Game1.viewport, (Character) this))
      this.position.Y += this.yVelocity;
    if ((double) this.xVelocity != 0.0 || (double) this.yVelocity != 0.0)
    {
      this.raftPuddleCounter -= time.ElapsedGameTime.Milliseconds;
      if (this.raftPuddleCounter <= 0)
      {
        this.raftPuddleCounter = 250;
        currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(0, 0, 64 /*0x40*/, 64 /*0x40*/), (float) (150.0 - ((double) Math.Abs(this.xVelocity) + (double) Math.Abs(this.yVelocity)) * 3.0), 8, 0, new Vector2((float) position.X, (float) (position.Y - 64 /*0x40*/)), false, Game1.random.NextBool(), 1f / 1000f, 0.01f, Color.White, 1f, 3f / 1000f, 0.0f, 0.0f));
        if (Game1.random.NextDouble() < 0.6)
          Game1.playSound("wateringCan");
        if (Game1.random.NextDouble() < 0.6)
          this.raftBobCounter /= 2;
      }
    }
    this.raftBobCounter -= time.ElapsedGameTime.Milliseconds;
    if (this.raftBobCounter <= 0)
    {
      this.raftBobCounter = Game1.random.Next(15, 28) * 100;
      if ((double) this.yOffset <= 0.0)
      {
        this.yOffset = 4f;
        currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(0, 0, 64 /*0x40*/, 64 /*0x40*/), (float) (150.0 - ((double) Math.Abs(this.xVelocity) + (double) Math.Abs(this.yVelocity)) * 3.0), 8, 0, new Vector2((float) position.X, (float) (position.Y - 64 /*0x40*/)), false, Game1.random.NextBool(), 1f / 1000f, 0.01f, Color.White, 1f, 3f / 1000f, 0.0f, 0.0f));
      }
      else
        this.yOffset = 0.0f;
    }
    if ((double) this.xVelocity > 0.0)
      this.xVelocity = Math.Max(0.0f, this.xVelocity - num / 2f);
    else if ((double) this.xVelocity < 0.0)
      this.xVelocity = Math.Min(0.0f, this.xVelocity + num / 2f);
    if ((double) this.yVelocity > 0.0)
    {
      this.yVelocity = Math.Max(0.0f, this.yVelocity - num / 2f);
    }
    else
    {
      if ((double) this.yVelocity >= 0.0)
        return;
      this.yVelocity = Math.Min(0.0f, this.yVelocity + num / 2f);
    }
  }

  public void warpFarmer(Warp w, int warp_collide_direction)
  {
    if (w == null || Game1.eventUp)
      return;
    this.Halt();
    int targetX = w.TargetX;
    int targetY = w.TargetY;
    if (this.isRidingHorse())
    {
      switch (warp_collide_direction)
      {
        case 0:
          Game1.nextFarmerWarpOffsetY = -1;
          break;
        case 3:
          Game1.nextFarmerWarpOffsetX = -1;
          break;
      }
    }
    Game1.warpFarmer(w.TargetName, targetX, targetY, w.flipFarmer.Value);
  }

  public void warpFarmer(Warp w) => this.warpFarmer(w, -1);

  public void startToPassOut() => this.passOutEvent.Fire();

  private void performPassOut()
  {
    if (this.isEmoteAnimating)
      this.EndEmoteAnimation();
    if (!this.swimming.Value && this.bathingClothes.Value)
      this.bathingClothes.Value = false;
    if (this.passedOut || this.FarmerSprite.isPassingOut())
      return;
    this.faceDirection(2);
    this.completelyStopAnimatingOrDoingAction();
    this.animateOnce(293);
  }

  public static void passOutFromTired(Farmer who)
  {
    if (!who.IsLocalPlayer)
      return;
    if (who.IsSitting())
      who.StopSitting(false);
    if (who.isRidingHorse())
      who.mount.dismount();
    if (Game1.activeClickableMenu != null)
    {
      Game1.activeClickableMenu.emergencyShutDown();
      Game1.exitActiveMenu();
    }
    who.completelyStopAnimatingOrDoingAction();
    if (who.bathingClothes.Value)
      who.changeOutOfSwimSuit();
    who.swimming.Value = false;
    who.CanMove = false;
    who.FarmerSprite.setCurrentSingleFrame(5, (short) 3000);
    who.FarmerSprite.PauseForSingleAnimation = true;
    if (!who.IsDedicatedPlayer && who == Game1.player && who.team.sleepAnnounceMode.Value != FarmerTeam.SleepAnnounceModes.Off)
    {
      string str = "PassedOut";
      string messageKey = "PassedOut_" + who.currentLocation.Name.TrimEnd('0', '1', '2', '3', '4', '5', '6', '7', '8', '9');
      if (Game1.content.LoadStringReturnNullIfNotFound("Strings\\UI:Chat_" + messageKey) != null)
      {
        Game1.multiplayer.globalChatInfoMessage(messageKey, who.displayName);
      }
      else
      {
        int num = 0;
        for (int index = 0; index < 2; ++index)
        {
          if (Game1.random.NextDouble() < 0.25)
            ++num;
        }
        Game1.multiplayer.globalChatInfoMessage(str + num.ToString(), who.displayName);
      }
    }
    if (Game1.currentLocation is FarmHouse currentLocation)
    {
      who.lastSleepLocation.Value = currentLocation.NameOrUniqueName;
      who.lastSleepPoint.Value = currentLocation.GetPlayerBedSpot();
    }
    Game1.multiplayer.sendPassoutRequest();
  }

  public static void performPassoutWarp(
    Farmer who,
    string bed_location_name,
    Point bed_point,
    bool has_bed)
  {
    GameLocation passOutLocation = who.currentLocationRef.Value;
    Vector2 vector2 = Utility.PointToVector2(bed_point) * 64f;
    Vector2 bed_tile = new Vector2((float) ((int) vector2.X / 64 /*0x40*/), (float) ((int) vector2.Y / 64 /*0x40*/));
    Vector2 bed_sleep_position = vector2;
    if (!who.isInBed.Value)
    {
      LocationRequest locationRequest = Game1.getLocationRequest(bed_location_name);
      Game1.warpFarmer(locationRequest, (int) vector2.X / 64 /*0x40*/, (int) vector2.Y / 64 /*0x40*/, 2);
      locationRequest.OnWarp += new LocationRequest.Callback(ContinuePassOut);
      who.FarmerSprite.setCurrentSingleFrame(5, (short) 3000);
      who.FarmerSprite.PauseForSingleAnimation = true;
    }
    else
      ContinuePassOut();

    void ContinuePassOut()
    {
      who.Position = bed_sleep_position;
      who.currentLocation.lastTouchActionLocation = bed_tile;
      if (who.NetFields.Root is NetRoot<Farmer> root)
      {
        // ISSUE: explicit non-virtual call
        __nonvirtual (root.CancelInterpolation());
      }
      if (!Game1.IsMultiplayer || Game1.timeOfDay >= 2600)
        Game1.PassOutNewDay();
      Game1.changeMusicTrack("none");
      if (passOutLocation is FarmHouse || passOutLocation is IslandFarmHouse || passOutLocation is Cellar || passOutLocation.HasMapPropertyWithValue("PassOutSafe"))
        return;
      Random random = Utility.CreateRandom((double) Game1.uniqueIDForThisGame, (double) Game1.stats.DaysPlayed, (double) who.UniqueMultiplayerID);
      int maxPassOutCost = passOutLocation.GetLocationContext().MaxPassOutCost;
      if (maxPassOutCost == -1)
        maxPassOutCost = StardewValley.LocationContexts.Default.MaxPassOutCost;
      int val1 = Math.Min(maxPassOutCost, who.Money / 10);
      List<PassOutMailData> passOutMailDataList = passOutLocation.GetLocationContext().PassOutMail ?? StardewValley.LocationContexts.Default.PassOutMail;
      PassOutMailData passOutMailData1 = (PassOutMailData) null;
      List<PassOutMailData> options = new List<PassOutMailData>();
      foreach (PassOutMailData passOutMailData2 in passOutMailDataList)
      {
        if (GameStateQuery.CheckConditions(passOutMailData2.Condition, passOutLocation, random: random))
        {
          if (passOutMailData2.SkipRandomSelection)
          {
            passOutMailData1 = passOutMailData2;
            break;
          }
          options.Add(passOutMailData2);
        }
      }
      if (passOutMailData1 == null && options.Count > 0)
        passOutMailData1 = random.ChooseFrom<PassOutMailData>((IList<PassOutMailData>) options);
      string str = (string) null;
      if (passOutMailData1 != null)
      {
        if (passOutMailData1.MaxPassOutCost >= 0)
          val1 = Math.Min(val1, passOutMailData1.MaxPassOutCost);
        string mail = passOutMailData1.Mail;
        if (!string.IsNullOrEmpty(mail))
        {
          Dictionary<string, string> dictionary = DataLoader.Mail(Game1.content);
          if (dictionary.ContainsKey($"{mail}_{(val1 > 0 ? "Billed" : "NotBilled")}_{(who.IsMale ? "Male" : "Female")}"))
            str = $"{mail}_{(val1 > 0 ? "Billed" : "NotBilled")}_{(who.IsMale ? "Male" : "Female")}";
          else
            str = !dictionary.ContainsKey($"{mail}_{(val1 > 0 ? "Billed" : "NotBilled")}") ? (!dictionary.ContainsKey(mail) ? "passedOut2" : mail) : $"{mail}_{(val1 > 0 ? "Billed" : "NotBilled")}";
          if (str.StartsWith("passedOut"))
            str = $"{str} {val1.ToString()}";
        }
      }
      if (val1 > 0)
        who.Money -= val1;
      if (str == null)
        return;
      who.mailForTomorrow.Add(str);
    }
  }

  public static void doSleepEmote(Farmer who)
  {
    who.doEmote(24);
    who.yJumpVelocity = -2f;
  }

  public override Microsoft.Xna.Framework.Rectangle GetBoundingBox()
  {
    if (this.mount != null && !this.mount.dismounting.Value)
      return this.mount.GetBoundingBox();
    Vector2 position = this.Position;
    return new Microsoft.Xna.Framework.Rectangle((int) position.X + 8, (int) position.Y + this.Sprite.getHeight() - 32 /*0x20*/, 48 /*0x30*/, 32 /*0x20*/);
  }

  public string getPetName()
  {
    foreach (NPC character in Game1.getFarm().characters)
    {
      if (character is Pet)
        return character.Name;
    }
    foreach (Farmer allFarmer in Game1.getAllFarmers())
    {
      foreach (NPC character in Utility.getHomeOfFarmer(allFarmer).characters)
      {
        if (character is Pet)
          return character.Name;
      }
    }
    return "your pet";
  }

  public Pet getPet()
  {
    foreach (NPC character in Game1.getFarm().characters)
    {
      if (character is Pet pet)
        return pet;
    }
    foreach (Farmer allFarmer in Game1.getAllFarmers())
    {
      foreach (NPC character in Utility.getHomeOfFarmer(allFarmer).characters)
      {
        if (character is Pet pet)
          return pet;
      }
    }
    return (Pet) null;
  }

  public string getPetDisplayName()
  {
    foreach (NPC character in Game1.getFarm().characters)
    {
      if (character is Pet)
        return character.displayName;
    }
    foreach (Farmer allFarmer in Game1.getAllFarmers())
    {
      foreach (NPC character in Utility.getHomeOfFarmer(allFarmer).characters)
      {
        if (character is Pet)
          return character.displayName;
      }
    }
    return Game1.content.LoadString("Strings\\StringsFromCSFiles:Farmer.cs.1972");
  }

  public bool hasPet()
  {
    foreach (NPC character in Game1.getFarm().characters)
    {
      if (character is Pet)
        return true;
    }
    foreach (Farmer allFarmer in Game1.getAllFarmers())
    {
      foreach (NPC character in Utility.getHomeOfFarmer(allFarmer).characters)
      {
        if (character is Pet)
          return true;
      }
    }
    return false;
  }

  public void UpdateClothing() => this.FarmerRenderer.MarkSpriteDirty();

  /// <summary>Get whether custom pants should be drawn instead of the equipped pants item.</summary>
  /// <param name="id">The pants ID to draw, if overridden.</param>
  /// <param name="color">The pants color to draw, if overridden.</param>
  public bool IsOverridingPants(out string id, out Color? color)
  {
    if (this.pants.Value != null && this.pants.Value != "-1")
    {
      id = this.pants.Value;
      color = new Color?(this.pantsColor.Value);
      return true;
    }
    id = (string) null;
    color = new Color?();
    return false;
  }

  /// <summary>Get whether the current pants can be dyed.</summary>
  public bool CanDyePants() => this.pantsItem.Value?.dyeable.Value ?? false;

  /// <summary>Get the pants to draw on the farmer.</summary>
  /// <param name="texture">The texture to render.</param>
  /// <param name="spriteIndex">The sprite index in the <paramref name="texture" />.</param>
  public void GetDisplayPants(out Texture2D texture, out int spriteIndex)
  {
    string id;
    if (this.IsOverridingPants(out id, out Color? _))
    {
      ParsedItemData data = ItemRegistry.GetData("(P)" + id);
      if (data != null && !data.IsErrorItem)
      {
        texture = data.GetTexture();
        spriteIndex = data.SpriteIndex;
        return;
      }
    }
    if (this.pantsItem.Value != null)
    {
      ParsedItemData dataOrErrorItem = ItemRegistry.GetDataOrErrorItem(this.pantsItem.Value.QualifiedItemId);
      if (dataOrErrorItem != null && !dataOrErrorItem.IsErrorItem)
      {
        texture = dataOrErrorItem.GetTexture();
        spriteIndex = this.pantsItem.Value.indexInTileSheet.Value;
        return;
      }
    }
    texture = FarmerRenderer.pantsTexture;
    spriteIndex = 14;
  }

  /// <summary>Get the unqualified item ID for the displayed pants (which aren't necessarily the equipped ones).</summary>
  public string GetPantsId()
  {
    string id;
    return this.IsOverridingPants(out id, out Color? _) ? id : this.pantsItem.Value?.ItemId ?? "14";
  }

  public int GetPantsIndex()
  {
    int spriteIndex;
    this.GetDisplayPants(out Texture2D _, out spriteIndex);
    return spriteIndex;
  }

  /// <summary>Get whether a custom shirt should be drawn instead of the equipped shirt item.</summary>
  /// <param name="id">The shirt ID to draw, if overridden.</param>
  public bool IsOverridingShirt(out string id)
  {
    if (this.shirt.Value != null && this.shirt.Value != "-1")
    {
      id = this.shirt.Value;
      return true;
    }
    id = (string) null;
    return false;
  }

  /// <summary>Get whether the current shirt can be dyed.</summary>
  public bool CanDyeShirt() => this.shirtItem.Value?.dyeable.Value ?? false;

  /// <summary>Get the shirt to draw on the farmer.</summary>
  /// <param name="texture">The texture to render.</param>
  /// <param name="spriteIndex">The sprite index in the <paramref name="texture" />.</param>
  public void GetDisplayShirt(out Texture2D texture, out int spriteIndex)
  {
    string id;
    if (this.IsOverridingShirt(out id))
    {
      ParsedItemData data = ItemRegistry.GetData("(S)" + id);
      if (data != null && !data.IsErrorItem)
      {
        texture = data.GetTexture();
        spriteIndex = data.SpriteIndex;
        return;
      }
    }
    if (this.shirtItem.Value != null)
    {
      ParsedItemData dataOrErrorItem = ItemRegistry.GetDataOrErrorItem(this.shirtItem.Value.QualifiedItemId);
      if (dataOrErrorItem != null && !dataOrErrorItem.IsErrorItem)
      {
        texture = dataOrErrorItem.GetTexture();
        spriteIndex = this.shirtItem.Value.indexInTileSheet.Value;
        return;
      }
    }
    texture = FarmerRenderer.shirtsTexture;
    spriteIndex = this.IsMale ? 209 : 41;
  }

  /// <summary>Get the unqualified item ID for the displayed shirt (which isn't necessarily the equipped one).</summary>
  public string GetShirtId()
  {
    string id;
    if (this.IsOverridingShirt(out id))
      return id;
    if (this.shirtItem.Value != null)
      return this.shirtItem.Value.ItemId;
    return !this.IsMale ? "1041" : "1209";
  }

  public int GetShirtIndex()
  {
    int spriteIndex;
    this.GetDisplayShirt(out Texture2D _, out spriteIndex);
    return spriteIndex;
  }

  public bool ShirtHasSleeves()
  {
    string id;
    if (!this.IsOverridingShirt(out id))
      id = this.shirtItem.Value?.ItemId;
    ShirtData shirtData;
    return id == null || !Game1.shirtData.TryGetValue(id, out shirtData) || shirtData.HasSleeves;
  }

  /// <summary>Get the color of the currently worn shirt.</summary>
  public Color GetShirtColor()
  {
    string id;
    ShirtData shirtData;
    if (this.IsOverridingShirt(out id) && Game1.shirtData.TryGetValue(id, out shirtData))
      return !shirtData.IsPrismatic ? Utility.StringToColor(shirtData.DefaultColor) ?? Color.White : Utility.GetPrismaticColor();
    if (this.shirtItem.Value == null)
      return this.DEFAULT_SHIRT_COLOR;
    return this.shirtItem.Value.isPrismatic.Value ? Utility.GetPrismaticColor() : this.shirtItem.Value.clothesColor.Value;
  }

  /// <summary>Get the color of the currently worn pants.</summary>
  public Color GetPantsColor()
  {
    Color? color;
    if (this.IsOverridingPants(out string _, out color))
      return color ?? Color.White;
    if (this.pantsItem.Value == null)
      return Color.White;
    return this.pantsItem.Value.isPrismatic.Value ? Utility.GetPrismaticColor() : this.pantsItem.Value.clothesColor.Value;
  }

  public bool movedDuringLastTick() => !this.Position.Equals(this.lastPosition);

  public int CompareTo(object obj) => ((Farmer) obj).saveTime - this.saveTime;

  public virtual void SetOnBridge(bool val)
  {
    if (this.onBridge.Value == val)
      return;
    this.onBridge.Value = val;
    if (!this.onBridge.Value)
      return;
    this.showNotCarrying();
  }

  public float getDrawLayer()
  {
    if (this.onBridge.Value)
      return (float) ((double) this.StandingPixel.Y / 10000.0 + (double) this.drawLayerDisambiguator + 0.025599999353289604);
    return this.IsSitting() && (double) this.mapChairSitPosition.Value.X != -1.0 && (double) this.mapChairSitPosition.Value.Y != -1.0 ? (float) (((double) this.mapChairSitPosition.Value.Y + 1.0) * 64.0 / 10000.0) : (float) this.StandingPixel.Y / 10000f + this.drawLayerDisambiguator;
  }

  public override void draw(SpriteBatch b)
  {
    if (this.currentLocation == null || !this.currentLocation.Equals(Game1.currentLocation) && !this.IsLocalPlayer && !Game1.currentLocation.IsTemporary && !this.isFakeEventActor || this.hidden.Value && (this.currentLocation.currentEvent == null || this != this.currentLocation.currentEvent.farmer) && (!this.IsLocalPlayer || Game1.locationRequest == null) || this.viewingLocation.Value != null && this.IsLocalPlayer)
      return;
    float drawLayer = this.getDrawLayer();
    if (this.isRidingHorse())
    {
      this.mount.SyncPositionToRider();
      this.mount.draw(b);
      if (this.FacingDirection == 3 || this.FacingDirection == 1)
        drawLayer += 1f / 625f;
    }
    double layerDepth = (double) FarmerRenderer.GetLayerDepth(0.0f, FarmerRenderer.FarmerSpriteLayers.MAX);
    Vector2 origin = new Vector2(this.xOffset, (float) (((double) this.yOffset + 128.0 - (double) (this.GetBoundingBox().Height / 2)) / 4.0 + 4.0));
    Point standingPixel = this.StandingPixel;
    xTile.Tiles.Tile tile = Game1.currentLocation.Map.RequireLayer("Buildings").PickTile(new Location(standingPixel.X, standingPixel.Y), Game1.viewport.Size);
    float num1 = (float) (layerDepth * 1.0);
    float num2 = (float) (layerDepth * 2.0);
    if (this.isGlowing)
    {
      if (this.coloredBorder)
        b.Draw(this.Sprite.Texture, new Vector2(this.getLocalPosition(Game1.viewport).X - 4f, this.getLocalPosition(Game1.viewport).Y - 4f), new Microsoft.Xna.Framework.Rectangle?(this.Sprite.SourceRect), this.glowingColor * this.glowingTransparency, 0.0f, Vector2.Zero, 1.1f, SpriteEffects.None, drawLayer + num1);
      else
        this.FarmerRenderer.draw(b, this.FarmerSprite, this.FarmerSprite.SourceRect, this.getLocalPosition(Game1.viewport) + this.jitter + new Vector2(0.0f, (float) this.yJumpOffset), origin, drawLayer + num1, this.glowingColor * this.glowingTransparency, this.rotation, this);
    }
    bool? nullable = tile?.TileIndexProperties.ContainsKey("Shadow");
    if ((!nullable.HasValue ? 0 : (nullable.GetValueOrDefault() ? 1 : 0)) == 0)
    {
      if (this.IsSitting() || !Game1.shouldTimePass() || !this.temporarilyInvincible || !this.flashDuringThisTemporaryInvincibility || this.temporaryInvincibilityTimer % 100 < 50)
        this.farmerRenderer.Value.draw(b, this.FarmerSprite, this.FarmerSprite.SourceRect, this.getLocalPosition(Game1.viewport) + this.jitter + new Vector2(0.0f, (float) this.yJumpOffset), origin, drawLayer, Color.White, this.rotation, this);
    }
    else
    {
      this.farmerRenderer.Value.draw(b, this.FarmerSprite, this.FarmerSprite.SourceRect, this.getLocalPosition(Game1.viewport), origin, drawLayer, Color.White, this.rotation, this);
      this.farmerRenderer.Value.draw(b, this.FarmerSprite, this.FarmerSprite.SourceRect, this.getLocalPosition(Game1.viewport), origin, drawLayer + num2, Color.Black * 0.25f, this.rotation, this);
    }
    if (this.isRafting)
      b.Draw(Game1.toolSpriteSheet, this.getLocalPosition(Game1.viewport) + new Vector2(0.0f, this.yOffset), new Microsoft.Xna.Framework.Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.toolSpriteSheet, 1)), Color.White, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, FarmerRenderer.GetLayerDepth(drawLayer, FarmerRenderer.FarmerSpriteLayers.ToolUp));
    if (Game1.activeClickableMenu == null && !Game1.eventUp && this.IsLocalPlayer && this.CurrentTool != null && (Game1.oldKBState.IsKeyDown(Keys.LeftShift) || Game1.options.alwaysShowToolHitLocation) && this.CurrentTool.doesShowTileLocationMarker() && (!Game1.options.hideToolHitLocationWhenInMotion || !this.isMoving()))
    {
      Vector2 target_position = Utility.PointToVector2(Game1.getMousePosition()) + new Vector2((float) Game1.viewport.X, (float) Game1.viewport.Y);
      Vector2 local = Game1.GlobalToLocal(Game1.viewport, Utility.clampToTile(this.GetToolLocation(target_position)));
      b.Draw(Game1.mouseCursors, local, new Microsoft.Xna.Framework.Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 29)), Color.White, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, local.Y / 10000f);
    }
    if (this.IsEmoting)
    {
      Vector2 localPosition = this.getLocalPosition(Game1.viewport);
      localPosition.Y -= 160f;
      b.Draw(Game1.emoteSpriteSheet, localPosition, new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(this.CurrentEmoteIndex * 16 /*0x10*/ % Game1.emoteSpriteSheet.Width, this.CurrentEmoteIndex * 16 /*0x10*/ / Game1.emoteSpriteSheet.Width * 16 /*0x10*/, 16 /*0x10*/, 16 /*0x10*/)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, drawLayer);
    }
    if (this.ActiveObject != null && this.IsCarrying())
      Game1.drawPlayerHeldObject(this);
    this.sparklingText?.draw(b, Game1.GlobalToLocal(Game1.viewport, this.Position + new Vector2((float) (32.0 - (double) this.sparklingText.textWidth / 2.0), (float) sbyte.MinValue)));
    if (this.UsingTool && this.CurrentTool != null)
      Game1.drawTool(this);
    foreach (Companion companion in this.companions)
      companion.Draw(b);
  }

  public virtual void DrawUsername(SpriteBatch b)
  {
    if (!Game1.IsMultiplayer || Game1.multiplayer == null || LocalMultiplayer.IsLocalMultiplayer(true) || (double) this.usernameDisplayTime <= 0.0)
      return;
    string userName = Game1.multiplayer.getUserName(this.UniqueMultiplayerID);
    if (userName == null)
      return;
    Vector2 vector2 = Game1.smallFont.MeasureString(userName);
    Vector2 position = this.getLocalPosition(Game1.viewport) + new Vector2(32f, -104f) - vector2 / 2f;
    for (int x = -1; x <= 1; ++x)
    {
      for (int y = -1; y <= 1; ++y)
      {
        if (x != 0 || y != 0)
          b.DrawString(Game1.smallFont, userName, position + new Vector2((float) x, (float) y) * 2f, Color.Black, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.9999f);
      }
    }
    b.DrawString(Game1.smallFont, userName, position, Color.White, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
  }

  public static void drinkGlug(Farmer who)
  {
    Color color = Color.LightBlue;
    if (who.itemToEat != null)
    {
      string str = ((IEnumerable<string>) ArgUtility.SplitBySpace(who.itemToEat.Name)).Last<string>();
      if (str != null)
      {
        switch (str.Length)
        {
          case 3:
            if (str == "Tea")
              goto label_24;
            goto label_27;
          case 4:
            switch (str[0])
            {
              case 'B':
                if (str == "Beer")
                {
                  color = Color.Orange;
                  goto label_27;
                }
                goto label_27;
              case 'C':
                if (str == "Cola")
                  break;
                goto label_27;
              case 'M':
                if (str == "Milk")
                {
                  color = Color.White;
                  goto label_27;
                }
                goto label_27;
              case 'S':
                if (str == "Soup")
                {
                  color = Color.LightGreen;
                  goto label_27;
                }
                goto label_27;
              case 'W':
                if (str == "Wine")
                {
                  color = Color.Purple;
                  goto label_27;
                }
                goto label_27;
              default:
                goto label_27;
            }
            break;
          case 5:
            switch (str[0])
            {
              case 'J':
                if (str == "Juice")
                  goto label_24;
                goto label_27;
              case 'T':
                if (str == "Tonic")
                {
                  color = Color.Red;
                  goto label_27;
                }
                goto label_27;
              default:
                goto label_27;
            }
          case 6:
            switch (str[0])
            {
              case 'C':
                if (str == "Coffee")
                  break;
                goto label_27;
              case 'R':
                if (str == "Remedy")
                {
                  color = Color.LimeGreen;
                  goto label_27;
                }
                goto label_27;
              default:
                goto label_27;
            }
            break;
          case 8:
            if (str == "Espresso")
              break;
            goto label_27;
          case 10:
            if (str == "Mayonnaise")
            {
              color = who.itemToEat.Name == "Void Mayonnaise" ? Color.Black : Color.White;
              goto label_27;
            }
            goto label_27;
          default:
            goto label_27;
        }
        color = new Color(46, 20, 0);
        goto label_27;
label_24:
        color = Color.LightGreen;
      }
    }
label_27:
    if (Game1.currentLocation == who.currentLocation)
      Game1.playSound(!(who.itemToEat is Object itemToEat) || itemToEat.preserve.Value.GetValueOrDefault() != Object.PreserveType.Pickle ? "gulp" : "eat");
    who.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(653, 858, 1, 1), 9999f, 1, 1, who.Position + new Vector2((float) (32 /*0x20*/ + Game1.random.Next(-2, 3) * 4), -48f), false, false, (float) ((double) who.StandingPixel.Y / 10000.0 + 1.0 / 1000.0), 0.04f, color, 5f, 0.0f, 0.0f, 0.0f)
    {
      acceleration = new Vector2(0.0f, 0.5f)
    });
  }

  public void handleDisconnect()
  {
    if (this.currentLocation != null)
    {
      this.rightRing.Value?.onLeaveLocation(this, this.currentLocation);
      this.leftRing.Value?.onLeaveLocation(this, this.currentLocation);
    }
    this.UnapplyAllTrinketEffects();
    this.disconnectDay.Value = (int) Game1.stats.DaysPlayed;
    this.disconnectLocation.Value = this.currentLocation?.NameOrUniqueName ?? "";
    this.disconnectPosition.Value = this.Position;
  }

  public bool isDivorced()
  {
    foreach (Friendship friendship in this.friendshipData.Values)
    {
      if (friendship.IsDivorced())
        return true;
    }
    return false;
  }

  public void wipeExMemories()
  {
    foreach (string key in this.friendshipData.Keys)
    {
      Friendship friendship = this.friendshipData[key];
      if (friendship.IsDivorced())
      {
        friendship.Clear();
        NPC characterFromName = Game1.getCharacterFromName(key);
        if (characterFromName != null)
        {
          characterFromName.CurrentDialogue.Clear();
          characterFromName.CurrentDialogue.Push(characterFromName.TryGetDialogue("WipedMemory") ?? new Dialogue(characterFromName, "Strings\\Characters:WipedMemory"));
          int num = (int) Game1.stats.Increment("exMemoriesWiped");
        }
      }
    }
  }

  public void getRidOfChildren()
  {
    FarmHouse homeOfFarmer = Utility.getHomeOfFarmer(this);
    for (int index = homeOfFarmer.characters.Count - 1; index >= 0; --index)
    {
      if (homeOfFarmer.characters[index] is Child character)
      {
        homeOfFarmer.GetChildBed((int) character.Gender)?.mutex.ReleaseLock();
        if (character.hat.Value != null)
        {
          Hat hat = character.hat.Value;
          character.hat.Value = (Hat) null;
          this.team.returnedDonations.Add((Item) hat);
          this.team.newLostAndFoundItems.Value = true;
        }
        homeOfFarmer.characters.RemoveAt(index);
        int num = (int) Game1.stats.Increment("childrenTurnedToDoves");
      }
    }
  }

  public void animateOnce(int whichAnimation)
  {
    this.FarmerSprite.animateOnce(whichAnimation, 100f, 6);
    this.CanMove = false;
  }

  public static void showItemIntake(Farmer who)
  {
    TemporaryAnimatedSprite temporaryAnimatedSprite = (TemporaryAnimatedSprite) null;
    Object @object = !(who.mostRecentlyGrabbedItem is Object recentlyGrabbedItem1) ? (who.ActiveObject == null ? (Object) null : who.ActiveObject) : recentlyGrabbedItem1;
    if (@object == null)
      return;
    ParsedItemData dataOrErrorItem = ItemRegistry.GetDataOrErrorItem(@object.QualifiedItemId);
    string textureName = dataOrErrorItem.TextureName;
    Microsoft.Xna.Framework.Rectangle sourceRect1 = dataOrErrorItem.GetSourceRect();
    switch (who.FacingDirection)
    {
      case 0:
        switch (who.FarmerSprite.currentAnimationIndex)
        {
          case 1:
            temporaryAnimatedSprite = new TemporaryAnimatedSprite(textureName, sourceRect1, 100f, 1, 0, who.Position + new Vector2(0.0f, -32f), false, false, (float) ((double) who.StandingPixel.Y / 10000.0 - 1.0 / 1000.0), 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f);
            break;
          case 2:
            temporaryAnimatedSprite = new TemporaryAnimatedSprite(textureName, sourceRect1, 100f, 1, 0, who.Position + new Vector2(0.0f, -43f), false, false, (float) ((double) who.StandingPixel.Y / 10000.0 - 1.0 / 1000.0), 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f);
            break;
          case 3:
            temporaryAnimatedSprite = new TemporaryAnimatedSprite(textureName, sourceRect1, 100f, 1, 0, who.Position + new Vector2(0.0f, (float) sbyte.MinValue), false, false, (float) ((double) who.StandingPixel.Y / 10000.0 - 1.0 / 1000.0), 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f);
            break;
          case 4:
            temporaryAnimatedSprite = new TemporaryAnimatedSprite(textureName, sourceRect1, 200f, 1, 0, who.Position + new Vector2(0.0f, -120f), false, false, (float) ((double) who.StandingPixel.Y / 10000.0 - 1.0 / 1000.0), 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f);
            break;
          case 5:
            temporaryAnimatedSprite = new TemporaryAnimatedSprite(textureName, sourceRect1, 200f, 1, 0, who.Position + new Vector2(0.0f, -120f), false, false, (float) ((double) who.StandingPixel.Y / 10000.0 - 1.0 / 1000.0), 0.02f, Color.White, 4f, -0.02f, 0.0f, 0.0f);
            break;
        }
        break;
      case 1:
        switch (who.FarmerSprite.currentAnimationIndex)
        {
          case 1:
            temporaryAnimatedSprite = new TemporaryAnimatedSprite(textureName, sourceRect1, 100f, 1, 0, who.Position + new Vector2(28f, -64f), false, false, (float) ((double) who.StandingPixel.Y / 10000.0 + 0.0099999997764825821), 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f);
            break;
          case 2:
            temporaryAnimatedSprite = new TemporaryAnimatedSprite(textureName, sourceRect1, 100f, 1, 0, who.Position + new Vector2(24f, -72f), false, false, (float) ((double) who.StandingPixel.Y / 10000.0 + 0.0099999997764825821), 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f);
            break;
          case 3:
            temporaryAnimatedSprite = new TemporaryAnimatedSprite(textureName, sourceRect1, 100f, 1, 0, who.Position + new Vector2(4f, (float) sbyte.MinValue), false, false, (float) ((double) who.StandingPixel.Y / 10000.0 + 0.0099999997764825821), 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f);
            break;
          case 4:
            temporaryAnimatedSprite = new TemporaryAnimatedSprite(textureName, sourceRect1, 200f, 1, 0, who.Position + new Vector2(0.0f, -124f), false, false, (float) ((double) who.StandingPixel.Y / 10000.0 + 0.0099999997764825821), 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f);
            break;
          case 5:
            temporaryAnimatedSprite = new TemporaryAnimatedSprite(textureName, sourceRect1, 200f, 1, 0, who.Position + new Vector2(0.0f, -124f), false, false, (float) ((double) who.StandingPixel.Y / 10000.0 + 0.0099999997764825821), 0.02f, Color.White, 4f, -0.02f, 0.0f, 0.0f);
            break;
        }
        break;
      case 2:
        switch (who.FarmerSprite.currentAnimationIndex)
        {
          case 1:
            temporaryAnimatedSprite = new TemporaryAnimatedSprite(textureName, sourceRect1, 100f, 1, 0, who.Position + new Vector2(0.0f, -32f), false, false, (float) ((double) who.StandingPixel.Y / 10000.0 + 0.0099999997764825821), 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f);
            break;
          case 2:
            temporaryAnimatedSprite = new TemporaryAnimatedSprite(textureName, sourceRect1, 100f, 1, 0, who.Position + new Vector2(0.0f, -43f), false, false, (float) ((double) who.StandingPixel.Y / 10000.0 + 0.0099999997764825821), 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f);
            break;
          case 3:
            temporaryAnimatedSprite = new TemporaryAnimatedSprite(textureName, sourceRect1, 100f, 1, 0, who.Position + new Vector2(0.0f, (float) sbyte.MinValue), false, false, (float) ((double) who.StandingPixel.Y / 10000.0 + 0.0099999997764825821), 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f);
            break;
          case 4:
            temporaryAnimatedSprite = new TemporaryAnimatedSprite(textureName, sourceRect1, 200f, 1, 0, who.Position + new Vector2(0.0f, -120f), false, false, (float) ((double) who.StandingPixel.Y / 10000.0 + 0.0099999997764825821), 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f);
            break;
          case 5:
            temporaryAnimatedSprite = new TemporaryAnimatedSprite(textureName, sourceRect1, 200f, 1, 0, who.Position + new Vector2(0.0f, -120f), false, false, (float) ((double) who.StandingPixel.Y / 10000.0 + 0.0099999997764825821), 0.02f, Color.White, 4f, -0.02f, 0.0f, 0.0f);
            break;
        }
        break;
      case 3:
        switch (who.FarmerSprite.currentAnimationIndex)
        {
          case 1:
            temporaryAnimatedSprite = new TemporaryAnimatedSprite(textureName, sourceRect1, 100f, 1, 0, who.Position + new Vector2(-32f, -64f), false, false, (float) ((double) who.StandingPixel.Y / 10000.0 + 0.0099999997764825821), 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f);
            break;
          case 2:
            temporaryAnimatedSprite = new TemporaryAnimatedSprite(textureName, sourceRect1, 100f, 1, 0, who.Position + new Vector2(-28f, -76f), false, false, (float) ((double) who.StandingPixel.Y / 10000.0 + 0.0099999997764825821), 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f);
            break;
          case 3:
            temporaryAnimatedSprite = new TemporaryAnimatedSprite(textureName, sourceRect1, 100f, 1, 0, who.Position + new Vector2(-16f, (float) sbyte.MinValue), false, false, (float) ((double) who.StandingPixel.Y / 10000.0 + 0.0099999997764825821), 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f);
            break;
          case 4:
            temporaryAnimatedSprite = new TemporaryAnimatedSprite(textureName, sourceRect1, 200f, 1, 0, who.Position + new Vector2(0.0f, -124f), false, false, (float) ((double) who.StandingPixel.Y / 10000.0 + 0.0099999997764825821), 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f);
            break;
          case 5:
            temporaryAnimatedSprite = new TemporaryAnimatedSprite(textureName, sourceRect1, 200f, 1, 0, who.Position + new Vector2(0.0f, -124f), false, false, (float) ((double) who.StandingPixel.Y / 10000.0 + 0.0099999997764825821), 0.02f, Color.White, 4f, -0.02f, 0.0f, 0.0f);
            break;
        }
        break;
    }
    if (@object.QualifiedItemId == who.ActiveObject?.QualifiedItemId && who.FarmerSprite.currentAnimationIndex == 5)
      temporaryAnimatedSprite = (TemporaryAnimatedSprite) null;
    if (temporaryAnimatedSprite != null)
      who.currentLocation.temporarySprites.Add(temporaryAnimatedSprite);
    if (who.mostRecentlyGrabbedItem is ColoredObject recentlyGrabbedItem2 && temporaryAnimatedSprite != null)
    {
      Microsoft.Xna.Framework.Rectangle sourceRect2 = ItemRegistry.GetDataOrErrorItem(recentlyGrabbedItem2.QualifiedItemId).GetSourceRect(1);
      who.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(textureName, sourceRect2, temporaryAnimatedSprite.interval, 1, 0, temporaryAnimatedSprite.Position, false, false, temporaryAnimatedSprite.layerDepth + 0.0001f, temporaryAnimatedSprite.alphaFade, recentlyGrabbedItem2.color.Value, 4f, temporaryAnimatedSprite.scaleChange, 0.0f, 0.0f));
    }
    if (who.FarmerSprite.currentAnimationIndex != 5)
      return;
    who.Halt();
    who.FarmerSprite.CurrentAnimation = (List<FarmerSprite.AnimationFrame>) null;
  }

  public virtual void showSwordSwipe(Farmer who)
  {
    TemporaryAnimatedSprite temporaryAnimatedSprite = (TemporaryAnimatedSprite) null;
    Vector2 toolLocation = who.GetToolLocation(true);
    bool flag = false;
    if (who.CurrentTool is MeleeWeapon currentTool)
    {
      flag = currentTool.type.Value == 1;
      if (!flag)
        currentTool.DoDamage(who.currentLocation, (int) toolLocation.X, (int) toolLocation.Y, who.FacingDirection, 1, who);
    }
    int val2 = 20;
    switch (who.FacingDirection)
    {
      case 0:
        switch (who.FarmerSprite.currentAnimationIndex)
        {
          case 0:
            if (flag)
            {
              who.yVelocity = 0.6f;
              break;
            }
            break;
          case 1:
            who.yVelocity = flag ? -0.5f : 0.5f;
            break;
          case 5:
            who.yVelocity = -0.3f;
            temporaryAnimatedSprite = new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(518, 274, 23, 31 /*0x1F*/), who.Position + new Vector2(0.0f, -32f) * 4f, false, 0.07f, Color.White)
            {
              scale = 4f,
              animationLength = 1,
              interval = (float) Math.Max(who.FarmerSprite.CurrentAnimationFrame.milliseconds, val2),
              alpha = 0.5f,
              rotation = 3.926991f
            };
            break;
        }
        break;
      case 1:
        switch (who.FarmerSprite.currentAnimationIndex)
        {
          case 0:
            if (flag)
            {
              who.xVelocity = 0.6f;
              break;
            }
            break;
          case 1:
            who.xVelocity = flag ? -0.5f : 0.5f;
            break;
          case 5:
            who.xVelocity = -0.3f;
            temporaryAnimatedSprite = new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(518, 274, 23, 31 /*0x1F*/), who.Position + new Vector2(4f, -12f) * 4f, false, 0.07f, Color.White)
            {
              scale = 4f,
              animationLength = 1,
              interval = (float) Math.Max(who.FarmerSprite.CurrentAnimationFrame.milliseconds, val2),
              alpha = 0.5f
            };
            break;
        }
        break;
      case 2:
        switch (who.FarmerSprite.currentAnimationIndex)
        {
          case 0:
            if (flag)
            {
              who.yVelocity = -0.6f;
              break;
            }
            break;
          case 1:
            who.yVelocity = flag ? 0.5f : -0.5f;
            break;
          case 5:
            who.yVelocity = 0.3f;
            temporaryAnimatedSprite = new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(503, 256 /*0x0100*/, 42, 17), who.Position + new Vector2(-16f, -2f) * 4f, false, 0.07f, Color.White)
            {
              scale = 4f,
              animationLength = 1,
              interval = (float) Math.Max(who.FarmerSprite.CurrentAnimationFrame.milliseconds, val2),
              alpha = 0.5f,
              layerDepth = (float) (((double) who.Position.Y + 64.0) / 10000.0)
            };
            break;
        }
        break;
      case 3:
        switch (who.FarmerSprite.currentAnimationIndex)
        {
          case 0:
            if (flag)
            {
              who.xVelocity = -0.6f;
              break;
            }
            break;
          case 1:
            who.xVelocity = flag ? 0.5f : -0.5f;
            break;
          case 5:
            who.xVelocity = 0.3f;
            temporaryAnimatedSprite = new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(518, 274, 23, 31 /*0x1F*/), who.Position + new Vector2(-15f, -12f) * 4f, false, 0.07f, Color.White)
            {
              scale = 4f,
              animationLength = 1,
              interval = (float) Math.Max(who.FarmerSprite.CurrentAnimationFrame.milliseconds, val2),
              flipped = true,
              alpha = 0.5f
            };
            break;
        }
        break;
    }
    if (temporaryAnimatedSprite == null)
      return;
    if (who.CurrentTool?.QualifiedItemId == "(W)4")
      temporaryAnimatedSprite.color = Color.HotPink;
    who.currentLocation.temporarySprites.Add(temporaryAnimatedSprite);
  }

  public static void showToolSwipeEffect(Farmer who)
  {
    if (who.CurrentTool is WateringCan)
      return;
    switch (who.FacingDirection)
    {
      case 0:
        who.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(18, who.Position + new Vector2(0.0f, -132f), Color.White, 4, animationInterval: (double) who.stamina <= 0.0 ? 100f : 50f, sourceRectWidth: 64 /*0x40*/, layerDepth: 1f, sourceRectHeight: 64 /*0x40*/)
        {
          layerDepth = (float) (who.StandingPixel.Y - 9) / 10000f
        });
        break;
      case 1:
        who.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(15, who.Position + new Vector2(20f, -132f), Color.White, 4, animationInterval: (double) who.stamina <= 0.0 ? 80f : 40f, sourceRectWidth: 128 /*0x80*/, layerDepth: 1f, sourceRectHeight: 128 /*0x80*/)
        {
          layerDepth = (float) (who.GetBoundingBox().Bottom + 1) / 10000f
        });
        break;
      case 2:
        who.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(19, who.Position + new Vector2(-4f, (float) sbyte.MinValue), Color.White, 4, animationInterval: (double) who.stamina <= 0.0 ? 80f : 40f, sourceRectWidth: 128 /*0x80*/, layerDepth: 1f, sourceRectHeight: 128 /*0x80*/)
        {
          layerDepth = (float) (who.GetBoundingBox().Bottom + 1) / 10000f
        });
        break;
      case 3:
        who.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(15, who.Position + new Vector2(-92f, -132f), Color.White, 4, true, (double) who.stamina <= 0.0 ? 80f : 40f, sourceRectWidth: 128 /*0x80*/, layerDepth: 1f, sourceRectHeight: 128 /*0x80*/)
        {
          layerDepth = (float) (who.GetBoundingBox().Bottom + 1) / 10000f
        });
        break;
    }
  }

  public static void canMoveNow(Farmer who)
  {
    who.CanMove = true;
    who.UsingTool = false;
    who.usingSlingshot = false;
    who.FarmerSprite.PauseForSingleAnimation = false;
    who.yVelocity = 0.0f;
    who.xVelocity = 0.0f;
  }

  public void FireTool() => this.fireToolEvent.Fire();

  public void synchronizedJump(float velocity)
  {
    if (!this.IsLocalPlayer)
      return;
    this.synchronizedJumpEvent.Fire(velocity);
    this.synchronizedJumpEvent.Poll();
  }

  protected void performSynchronizedJump(float velocity)
  {
    this.yJumpVelocity = velocity;
    this.yJumpOffset = -1;
  }

  private void performFireTool()
  {
    if (this.isEmoteAnimating)
      this.EndEmoteAnimation();
    this.CurrentTool?.leftClick(this);
  }

  public static void useTool(Farmer who)
  {
    if (who.toolOverrideFunction != null)
    {
      who.toolOverrideFunction(who);
    }
    else
    {
      if (who.CurrentTool == null)
        return;
      float stamina = who.stamina;
      if (who.IsLocalPlayer)
        who.CurrentTool.DoFunction(who.currentLocation, (int) who.GetToolLocation().X, (int) who.GetToolLocation().Y, 1, who);
      who.lastClick = Vector2.Zero;
      who.checkForExhaustion(stamina);
    }
  }

  public void BeginUsingTool() => this.beginUsingToolEvent.Fire();

  private void performBeginUsingTool()
  {
    if (this.isEmoteAnimating)
      this.EndEmoteAnimation();
    if (this.CurrentTool == null)
      return;
    this.CanMove = false;
    this.UsingTool = true;
    this.canReleaseTool = true;
    this.CurrentTool.beginUsing(this.currentLocation, (int) this.lastClick.X, (int) this.lastClick.Y, this);
  }

  public void EndUsingTool()
  {
    if (this == Game1.player)
      this.endUsingToolEvent.Fire();
    else
      this.performEndUsingTool();
  }

  private void performEndUsingTool()
  {
    if (this.isEmoteAnimating)
      this.EndEmoteAnimation();
    this.CurrentTool?.endUsing(this.currentLocation, this);
  }

  public void checkForExhaustion(float oldStamina)
  {
    if ((double) this.stamina <= 0.0 && (double) oldStamina > 0.0)
    {
      if (!this.exhausted.Value && this.IsLocalPlayer)
        Game1.showGlobalMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Farmer.cs.1986"));
      this.setRunning(false);
      this.doEmote(36);
    }
    else if ((double) this.stamina <= 15.0 && (double) oldStamina > 15.0 && this.IsLocalPlayer)
      Game1.showGlobalMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Farmer.cs.1987"));
    if ((double) this.stamina > 0.0)
      return;
    this.exhausted.Value = true;
  }

  public void setMoving(byte command)
  {
    switch (command)
    {
      case 1:
        if (this.movementDirections.Count < 2 && !this.movementDirections.Contains(0) && !this.movementDirections.Contains(2))
        {
          this.movementDirections.Insert(0, 0);
          break;
        }
        break;
      case 2:
        if (this.movementDirections.Count < 2 && !this.movementDirections.Contains(1) && !this.movementDirections.Contains(3))
        {
          this.movementDirections.Insert(0, 1);
          break;
        }
        break;
      case 4:
        if (this.movementDirections.Count < 2 && !this.movementDirections.Contains(2) && !this.movementDirections.Contains(0))
        {
          this.movementDirections.Insert(0, 2);
          break;
        }
        break;
      case 8:
        if (this.movementDirections.Count < 2 && !this.movementDirections.Contains(3) && !this.movementDirections.Contains(1))
        {
          this.movementDirections.Insert(0, 3);
          break;
        }
        break;
      case 16 /*0x10*/:
        this.setRunning(true);
        break;
      case 33:
        this.movementDirections.Remove(0);
        break;
      case 34:
        this.movementDirections.Remove(1);
        break;
      case 36:
        this.movementDirections.Remove(2);
        break;
      case 40:
        this.movementDirections.Remove(3);
        break;
      case 48 /*0x30*/:
        this.setRunning(false);
        break;
    }
    if (((int) command & 64 /*0x40*/) != 64 /*0x40*/)
      return;
    this.Halt();
    this.running = false;
  }

  public void toolPowerIncrease()
  {
    if (this.CurrentTool is Pan)
      return;
    if (this.toolPower.Value == 0)
      this.toolPitchAccumulator = 0;
    ++this.toolPower.Value;
    if (this.CurrentTool is Pickaxe && this.toolPower.Value == 1)
      this.toolPower.Value += 2;
    Color color = Color.White;
    int num = this.FacingDirection == 0 ? 4 : (this.FacingDirection == 2 ? 2 : 0);
    switch (this.toolPower.Value)
    {
      case 1:
        color = Color.Orange;
        if (!(this.CurrentTool is WateringCan))
          this.FarmerSprite.CurrentFrame = 72 + num;
        this.jitterStrength = 0.25f;
        break;
      case 2:
        color = Color.LightSteelBlue;
        if (!(this.CurrentTool is WateringCan))
          ++this.FarmerSprite.CurrentFrame;
        this.jitterStrength = 0.5f;
        break;
      case 3:
        color = Color.Gold;
        this.jitterStrength = 1f;
        break;
      case 4:
        color = Color.Violet;
        this.jitterStrength = 2f;
        break;
      case 5:
        color = Color.BlueViolet;
        this.jitterStrength = 3f;
        break;
    }
    int x = this.FacingDirection == 1 ? 40 : (this.FacingDirection == 3 ? -40 : (this.FacingDirection == 2 ? 32 /*0x20*/ : 0));
    int y1 = 192 /*0xC0*/;
    if (this.CurrentTool is WateringCan)
    {
      switch (this.FacingDirection)
      {
        case 1:
          x = -48;
          break;
        case 2:
          x = 0;
          break;
        case 3:
          x = 48 /*0x30*/;
          break;
      }
      y1 = 128 /*0x80*/;
    }
    int y2 = this.StandingPixel.Y;
    Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(21, this.Position - new Vector2((float) x, (float) y1), color, animationInterval: 70f, sourceRectWidth: 64 /*0x40*/, layerDepth: (float) ((double) y2 / 10000.0 + 0.004999999888241291), sourceRectHeight: 128 /*0x80*/));
    Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(192 /*0xC0*/, 1152, 64 /*0x40*/, 64 /*0x40*/), 50f, 4, 0, this.Position - new Vector2(this.FacingDirection == 1 ? 0.0f : -64f, 128f), false, this.FacingDirection == 1, (float) y2 / 10000f, 0.01f, Color.White, 1f, 0.0f, 0.0f, 0.0f));
    Game1.playSound("toolCharge", new int?(Utility.CreateRandom((double) Game1.dayOfMonth, (double) this.Position.X * 1000.0, (double) this.Position.Y).Next(12, 16 /*0x10*/) * 100 + this.toolPower.Value * 100));
  }

  public void UpdateIfOtherPlayer(GameTime time)
  {
    if (this.currentLocation == null)
      return;
    this.position.UpdateExtrapolation(this.getMovementSpeed());
    this.position.Field.InterpolationEnabled = !this.currentLocationRef.IsChanging();
    if (Game1.ShouldShowOnscreenUsernames() && (double) Game1.mouseCursorTransparency > 0.0 && this.currentLocation == Game1.currentLocation && Game1.currentMinigame == null && Game1.activeClickableMenu == null)
    {
      Vector2 localPosition = this.getLocalPosition(Game1.viewport);
      Microsoft.Xna.Framework.Rectangle rectangle = new Microsoft.Xna.Framework.Rectangle(0, 0, 128 /*0x80*/, 192 /*0xC0*/);
      rectangle.X = (int) ((double) localPosition.X + 32.0 - (double) (rectangle.Width / 2));
      rectangle.Y = (int) ((double) localPosition.Y - (double) rectangle.Height + 48.0);
      if (rectangle.Contains(Game1.getMouseX(false), Game1.getMouseY(false)))
        this.usernameDisplayTime = 1f;
    }
    if (this._lastSelectedItem != this.CurrentItem)
    {
      this._lastSelectedItem?.actionWhenStopBeingHeld(this);
      this._lastSelectedItem = this.CurrentItem;
    }
    this.fireToolEvent.Poll();
    this.beginUsingToolEvent.Poll();
    this.endUsingToolEvent.Poll();
    this.drinkAnimationEvent.Poll();
    this.eatAnimationEvent.Poll();
    this.sickAnimationEvent.Poll();
    this.passOutEvent.Poll();
    this.doEmoteEvent.Poll();
    this.kissFarmerEvent.Poll();
    this.haltAnimationEvent.Poll();
    this.synchronizedJumpEvent.Poll();
    this.renovateEvent.Poll();
    this.FarmerSprite.checkForSingleAnimation(time);
    this.updateCommon(time, this.currentLocation);
  }

  /// <summary>Put an item into an equipment slot with appropriate updates (e.g. calling <see cref="M:StardewValley.Item.onEquip(StardewValley.Farmer)" /> or <see cref="M:StardewValley.Item.onUnequip(StardewValley.Farmer)" />).</summary>
  /// <typeparam name="TItem">The item type.</typeparam>
  /// <param name="newItem">The item to place in the equipment slot, or <c>null</c> to just unequip the old item.</param>
  /// <param name="slot">The equipment slot to update.</param>
  /// <returns>Returns the item that was previously in the equipment slot, or <c>null</c> if it was empty.</returns>
  public TItem Equip<TItem>(TItem newItem, NetRef<TItem> slot) where TItem : Item
  {
    TItem oldItem = slot.Value;
    oldItem?.onDetachedFromParent();
    newItem?.onDetachedFromParent();
    this.Equip<TItem>(oldItem, newItem, (Action<TItem>) (val => ((NetFieldBase<TItem, NetRef<TItem>>) slot).Value = val));
    return oldItem;
  }

  /// <summary>Place an item into an equipment slot manually with appropriate updates (e.g. calling <see cref="M:StardewValley.Item.onEquip(StardewValley.Farmer)" /> or <see cref="M:StardewValley.Item.onUnequip(StardewValley.Farmer)" />).</summary>
  /// <typeparam name="TItem">The item type.</typeparam>
  /// <param name="oldItem">The item previously in the equipment slot, or <c>null</c> if it was empty.</param>
  /// <param name="newItem">The item to place in the equipment slot, or <c>null</c> to just unequip the old item.</param>
  /// <param name="equip">A callback which equips an item in the slot.</param>
  /// <remarks>Most code should use <see cref="M:StardewValley.Farmer.Equip``1(``0,Netcode.NetRef{``0})" /> instead. When calling this form, you should call <see cref="M:StardewValley.Item.onDetachedFromParent" /> on the old/new items as needed to avoid warnings.</remarks>
  public void Equip<TItem>(TItem oldItem, TItem newItem, Action<TItem> equip) where TItem : Item
  {
    bool flag = Game1.hasLoadedGame && Game1.dayOfMonth > 0 && this.IsLocalPlayer;
    if (flag)
      oldItem?.onUnequip(this);
    equip(newItem);
    if ((object) newItem != null)
    {
      newItem.HasBeenInInventory = true;
      if (flag)
        newItem.onEquip(this);
    }
    bool? nullable = oldItem?.HasEquipmentBuffs();
    if (!nullable.HasValue || !nullable.GetValueOrDefault())
    {
      nullable = newItem?.HasEquipmentBuffs();
      if (!nullable.HasValue || !nullable.GetValueOrDefault())
        return;
    }
    this.buffs.Dirty = true;
  }

  public void forceCanMove()
  {
    this.forceTimePass = false;
    this.movementDirections.Clear();
    this.isEating = false;
    this.CanMove = true;
    Game1.freezeControls = false;
    this.freezePause = 0;
    this.UsingTool = false;
    this.usingSlingshot = false;
    this.FarmerSprite.PauseForSingleAnimation = false;
    if (!(this.CurrentTool is FishingRod currentTool))
      return;
    currentTool.isFishing = false;
  }

  public void dropItem(Item i)
  {
    if (i == null || !i.canBeDropped())
      return;
    Game1.createItemDebris(i.getOne(), this.getStandingPosition(), this.FacingDirection);
  }

  public bool addEvent(string eventName, int daysActive)
  {
    return this.activeDialogueEvents.TryAdd(eventName, daysActive);
  }

  public Vector2 getMostRecentMovementVector()
  {
    return new Vector2(this.Position.X - this.lastPosition.X, this.Position.Y - this.lastPosition.Y);
  }

  public int GetSkillLevel(int index)
  {
    switch (index)
    {
      case 0:
        return this.FarmingLevel;
      case 1:
        return this.FishingLevel;
      case 2:
        return this.ForagingLevel;
      case 3:
        return this.MiningLevel;
      case 4:
        return this.CombatLevel;
      case 5:
        return this.LuckLevel;
      default:
        return 0;
    }
  }

  public int GetUnmodifiedSkillLevel(int index)
  {
    switch (index)
    {
      case 0:
        return this.farmingLevel.Value;
      case 1:
        return this.fishingLevel.Value;
      case 2:
        return this.foragingLevel.Value;
      case 3:
        return this.miningLevel.Value;
      case 4:
        return this.combatLevel.Value;
      case 5:
        return this.luckLevel.Value;
      default:
        return 0;
    }
  }

  public static string getSkillNameFromIndex(int index)
  {
    switch (index)
    {
      case 0:
        return "Farming";
      case 1:
        return "Fishing";
      case 2:
        return "Foraging";
      case 3:
        return "Mining";
      case 4:
        return "Combat";
      case 5:
        return "Luck";
      default:
        return "";
    }
  }

  public static int getSkillNumberFromName(string name)
  {
    switch (name.ToLower())
    {
      case "farming":
        return 0;
      case "mining":
        return 3;
      case "fishing":
        return 1;
      case "foraging":
        return 2;
      case "luck":
        return 5;
      case "combat":
        return 4;
      default:
        return -1;
    }
  }

  public bool setSkillLevel(string nameOfSkill, int level)
  {
    int skillNumberFromName = Farmer.getSkillNumberFromName(nameOfSkill);
    switch (nameOfSkill)
    {
      case "Farming":
        if (this.farmingLevel.Value < level)
        {
          this.newLevels.Add(new Point(skillNumberFromName, level - this.farmingLevel.Value));
          this.farmingLevel.Value = level;
          this.experiencePoints[skillNumberFromName] = Farmer.getBaseExperienceForLevel(level);
          return true;
        }
        break;
      case "Fishing":
        if (this.fishingLevel.Value < level)
        {
          this.newLevels.Add(new Point(skillNumberFromName, level - this.fishingLevel.Value));
          this.fishingLevel.Value = level;
          this.experiencePoints[skillNumberFromName] = Farmer.getBaseExperienceForLevel(level);
          return true;
        }
        break;
      case "Foraging":
        if (this.foragingLevel.Value < level)
        {
          this.newLevels.Add(new Point(skillNumberFromName, level - this.foragingLevel.Value));
          this.foragingLevel.Value = level;
          this.experiencePoints[skillNumberFromName] = Farmer.getBaseExperienceForLevel(level);
          return true;
        }
        break;
      case "Mining":
        if (this.miningLevel.Value < level)
        {
          this.newLevels.Add(new Point(skillNumberFromName, level - this.miningLevel.Value));
          this.miningLevel.Value = level;
          this.experiencePoints[skillNumberFromName] = Farmer.getBaseExperienceForLevel(level);
          return true;
        }
        break;
      case "Combat":
        if (this.combatLevel.Value < level)
        {
          this.newLevels.Add(new Point(skillNumberFromName, level - this.combatLevel.Value));
          this.combatLevel.Value = level;
          this.experiencePoints[skillNumberFromName] = Farmer.getBaseExperienceForLevel(level);
          return true;
        }
        break;
    }
    return false;
  }

  public static string getSkillDisplayNameFromIndex(int index)
  {
    switch (index)
    {
      case 0:
        return Game1.content.LoadString("Strings\\StringsFromCSFiles:Farmer.cs.1991");
      case 1:
        return Game1.content.LoadString("Strings\\StringsFromCSFiles:Farmer.cs.1993");
      case 2:
        return Game1.content.LoadString("Strings\\StringsFromCSFiles:Farmer.cs.1994");
      case 3:
        return Game1.content.LoadString("Strings\\StringsFromCSFiles:Farmer.cs.1992");
      case 4:
        return Game1.content.LoadString("Strings\\StringsFromCSFiles:Farmer.cs.1996");
      case 5:
        return Game1.content.LoadString("Strings\\StringsFromCSFiles:Farmer.cs.1995");
      default:
        return "";
    }
  }

  public bool hasCompletedCommunityCenter()
  {
    return this.mailReceived.Contains("ccBoilerRoom") && this.mailReceived.Contains("ccCraftsRoom") && this.mailReceived.Contains("ccPantry") && this.mailReceived.Contains("ccFishTank") && this.mailReceived.Contains("ccVault") && this.mailReceived.Contains("ccBulletin");
  }

  private bool localBusMoving()
  {
    switch (this.currentLocation)
    {
      case Desert desert:
        return desert.drivingOff || desert.drivingBack;
      case BusStop busStop:
        return busStop.drivingOff || busStop.drivingBack;
      default:
        return false;
    }
  }

  public virtual bool CanBeDamaged()
  {
    return !this.IsDedicatedPlayer && !this.temporarilyInvincible && !this.isEating && !Game1.fadeToBlack && !this.hasBuff("21");
  }

  public void takeDamage(int damage, bool overrideParry, Monster damager)
  {
    if (Game1.eventUp || this.IsDedicatedPlayer || this.FarmerSprite.isPassingOut() || this.isInBed.Value && Game1.activeClickableMenu != null && Game1.activeClickableMenu is ReadyCheckDialog)
      return;
    int num1 = damager == null || damager.isInvincible() ? 0 : (!overrideParry ? 1 : 0);
    bool flag1 = (damager == null || !damager.isInvincible()) && (damager == null || !(damager is GreenSlime) && !(damager is BigSlime) || !this.isWearingRing("520"));
    bool flag2 = this.CurrentTool is MeleeWeapon && ((MeleeWeapon) this.CurrentTool).isOnSpecial && ((MeleeWeapon) this.CurrentTool).type.Value == 3;
    bool flag3 = this.CanBeDamaged();
    int num2 = flag2 ? 1 : 0;
    if ((num1 & num2) != 0)
    {
      Rumble.rumble(0.75f, 150f);
      this.playNearbySoundAll("parry");
      damager.parried(damage, this);
    }
    else
    {
      if (!(flag1 & flag3))
        return;
      damager?.onDealContactDamage(this);
      damage += Game1.random.Next(Math.Min(-1, -damage / 8), Math.Max(1, damage / 8));
      int defense = this.buffs.Defense;
      if (this.stats.Get("Book_Defense") > 0U)
        ++defense;
      if ((double) defense >= (double) damage * 0.5)
        defense -= (int) ((double) defense * (double) Game1.random.Next(3) / 10.0);
      if (damager != null && this.isWearingRing("839"))
      {
        Microsoft.Xna.Framework.Rectangle boundingBox = damager.GetBoundingBox();
        Vector2 vector2 = Utility.getAwayFromPlayerTrajectory(boundingBox, this) / 2f;
        int num3 = damage;
        int num4 = Math.Max(1, damage - defense);
        if (num4 < 10)
          num3 = (int) Math.Ceiling((double) (num3 + num4) / 2.0);
        int ofWornRingsWithId = this.getNumberOfWornRingsWithID("839");
        int minDamage = num3 * ofWornRingsWithId;
        this.currentLocation?.damageMonster(boundingBox, minDamage, minDamage + 1, false, this);
      }
      if (this.isWearingRing("524") && !this.hasBuff("21") && Game1.random.NextDouble() < (0.9 - (double) this.health / 100.0) / (double) (3 - this.LuckLevel / 10) + (this.health <= 15 ? 0.2 : 0.0))
      {
        this.playNearbySoundAll("yoba");
        this.applyBuff("21");
      }
      else
      {
        Rumble.rumble(0.75f, 150f);
        damage = Math.Max(1, damage - defense);
        if (Utility.GetDayOfPassiveFestival("DesertFestival") > 0 && this.currentLocation is MineShaft && Game1.mine.getMineArea() == 121)
        {
          float num5 = 1f;
          int num6;
          if (this.team.calicoStatueEffects.TryGetValue(8, out num6))
            num5 += (float) num6 * 0.25f;
          int num7;
          if (this.team.calicoStatueEffects.TryGetValue(14, out num7))
            num5 -= (float) num7 * 0.25f;
          damage = Math.Max(1, (int) ((double) damage * (double) num5));
        }
        this.health = Math.Max(0, this.health - damage);
        foreach (Trinket trinketItem in this.trinketItems)
          trinketItem?.OnReceiveDamage(this, damage);
        if (this.health <= 0 && this.GetEffectsOfRingMultiplier("863") > 0 && !this.hasUsedDailyRevive.Value)
        {
          this.startGlowing(new Color((int) byte.MaxValue, (int) byte.MaxValue, 0), false, 0.25f);
          DelayedAction.functionAfterDelay(new Action(((Character) this).stopGlowing), 500);
          Game1.playSound("yoba");
          for (int index = 0; index < 13; ++index)
          {
            float num8 = (float) Game1.random.Next(-32, 33);
            this.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors2", new Microsoft.Xna.Framework.Rectangle(114, 46, 2, 2), 200f, 5, 1, new Vector2(num8 + 32f, -96f), false, false, 1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
            {
              attachedCharacter = (Character) this,
              positionFollowsAttachedCharacter = true,
              motion = new Vector2(num8 / 32f, -3f),
              delayBeforeAnimationStart = index * 50,
              alphaFade = 1f / 1000f,
              acceleration = new Vector2(0.0f, 0.1f)
            });
          }
          this.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors2", new Microsoft.Xna.Framework.Rectangle(157, 280, 28, 19), 2000f, 1, 1, new Vector2(-20f, -16f), false, false, 1E-06f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            attachedCharacter = (Character) this,
            positionFollowsAttachedCharacter = true,
            alpha = 0.1f,
            alphaFade = -0.01f,
            alphaFadeFade = -0.00025f
          });
          this.health = (int) Math.Min((float) this.maxHealth, (float) this.maxHealth * 0.5f + (float) this.GetEffectsOfRingMultiplier("863"));
          this.hasUsedDailyRevive.Value = true;
        }
        this.temporarilyInvincible = true;
        this.flashDuringThisTemporaryInvincibility = true;
        this.temporaryInvincibilityTimer = 0;
        this.currentTemporaryInvincibilityDuration = 1200 + this.GetEffectsOfRingMultiplier("861") * 400;
        Point standingPixel = this.StandingPixel;
        this.currentLocation.debris.Add(new Debris(damage, new Vector2((float) (standingPixel.X + 8), (float) standingPixel.Y), Color.Red, 1f, (Character) this));
        this.playNearbySoundAll("ow");
        Game1.hitShakeTimer = 100 * damage;
      }
    }
  }

  public int GetEffectsOfRingMultiplier(string ringId)
  {
    int ofRingMultiplier = 0;
    if (this.leftRing.Value != null)
      ofRingMultiplier += this.leftRing.Value.GetEffectsOfRingMultiplier(ringId);
    if (this.rightRing.Value != null)
      ofRingMultiplier += this.rightRing.Value.GetEffectsOfRingMultiplier(ringId);
    return ofRingMultiplier;
  }

  private void checkDamage(GameLocation location)
  {
    if (Game1.eventUp)
      return;
    for (int index = location.characters.Count - 1; index >= 0; --index)
    {
      if (index < location.characters.Count && location.characters[index] is Monster character && character.OverlapsFarmerForDamage(this))
      {
        character.currentLocation = location;
        character.collisionWithFarmerBehavior();
        if (character.DamageToFarmer > 0)
        {
          if (this.CurrentTool is MeleeWeapon && ((MeleeWeapon) this.CurrentTool).isOnSpecial && ((MeleeWeapon) this.CurrentTool).type.Value == 3)
            this.takeDamage(character.DamageToFarmer, false, character);
          else
            this.takeDamage(Math.Max(1, character.DamageToFarmer + Game1.random.Next(-character.DamageToFarmer / 4, character.DamageToFarmer / 4)), false, character);
        }
      }
    }
  }

  public bool checkAction(Farmer who, GameLocation location)
  {
    if (who.isRidingHorse())
      who.Halt();
    if (this.hidden.Value)
      return false;
    if (Game1.CurrentEvent != null)
    {
      if (!Game1.CurrentEvent.isSpecificFestival("spring24") || who.dancePartner.Value != null)
        return false;
      who.Halt();
      who.faceGeneralDirection(this.getStandingPosition(), 0, false, false);
      string question = Game1.content.LoadString("Strings\\UI:AskToDance_" + (this.IsMale ? "Male" : "Female"), (object) this.Name);
      location.createQuestionDialogue(question, location.createYesNoResponses(), (GameLocation.afterQuestionBehavior) ((_, answer) =>
      {
        if (!(answer == "Yes"))
          return;
        who.team.SendProposal(this, ProposalType.Dance);
        Game1.activeClickableMenu = (IClickableMenu) new PendingProposalDialog();
      }));
      return true;
    }
    if (who.CurrentItem != null && who.CurrentItem.QualifiedItemId == "(O)801" && !this.isMarriedOrRoommates() && !this.isEngaged() && !who.isMarriedOrRoommates() && !who.isEngaged())
    {
      who.Halt();
      who.faceGeneralDirection(this.getStandingPosition(), 0, false, false);
      string question = Game1.content.LoadString("Strings\\UI:AskToMarry_" + (this.IsMale ? "Male" : "Female"), (object) this.Name);
      location.createQuestionDialogue(question, location.createYesNoResponses(), (GameLocation.afterQuestionBehavior) ((_, answer) =>
      {
        if (!(answer == "Yes"))
          return;
        who.team.SendProposal(this, ProposalType.Marriage, who.CurrentItem.getOne());
        Game1.activeClickableMenu = (IClickableMenu) new PendingProposalDialog();
      }));
      return true;
    }
    if (who.CanMove)
    {
      bool? nullable = who.ActiveObject?.canBeGivenAsGift();
      if (nullable.HasValue && nullable.GetValueOrDefault() && !who.ActiveObject.questItem.Value)
      {
        who.Halt();
        who.faceGeneralDirection(this.getStandingPosition(), 0, false, false);
        string question = Game1.content.LoadString("Strings\\UI:GiftPlayerItem_" + (this.IsMale ? "Male" : "Female"), (object) who.ActiveObject.DisplayName, (object) this.Name);
        location.createQuestionDialogue(question, location.createYesNoResponses(), (GameLocation.afterQuestionBehavior) ((_, answer) =>
        {
          if (!(answer == "Yes"))
            return;
          who.team.SendProposal(this, ProposalType.Gift, who.ActiveObject.getOne());
          Game1.activeClickableMenu = (IClickableMenu) new PendingProposalDialog();
        }));
        return true;
      }
    }
    long? spouse = this.team.GetSpouse(this.UniqueMultiplayerID);
    int num1 = spouse.HasValue ? 1 : 0;
    long uniqueMultiplayerId = who.UniqueMultiplayerID;
    long? nullable1 = spouse;
    long valueOrDefault = nullable1.GetValueOrDefault();
    int num2 = uniqueMultiplayerId == valueOrDefault & nullable1.HasValue ? 1 : 0;
    if ((num1 & num2) == 0 || !who.CanMove || who.isMoving() || this.isMoving() || !Utility.IsHorizontalDirection(this.getGeneralDirectionTowards(who.getStandingPosition(), -10, useTileCalculations: false)))
      return false;
    who.Halt();
    who.faceGeneralDirection(this.getStandingPosition(), 0, false, false);
    who.kissFarmerEvent.Fire(this.UniqueMultiplayerID);
    Game1.multiplayer.broadcastSprites(this.currentLocation, new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(211, 428, 7, 6), 2000f, 1, 0, this.Tile * 64f + new Vector2(16f, -64f), false, false, 1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
    {
      motion = new Vector2(0.0f, -0.5f),
      alphaFade = 0.01f
    });
    this.playNearbySoundAll("dwop", context: SoundContext.NPC);
    return true;
  }

  public void Update(GameTime time, GameLocation location)
  {
    if (this._lastEquippedTool != this.CurrentTool)
      this.Equip<Tool>(this._lastEquippedTool, this.CurrentTool, (Action<Tool>) (tool => this._lastEquippedTool = tool));
    this.buffs.SetOwner(this);
    this.buffs.Update(time);
    this.position.UpdateExtrapolation(this.getMovementSpeed());
    this.fireToolEvent.Poll();
    this.beginUsingToolEvent.Poll();
    this.endUsingToolEvent.Poll();
    this.drinkAnimationEvent.Poll();
    this.eatAnimationEvent.Poll();
    this.sickAnimationEvent.Poll();
    this.passOutEvent.Poll();
    this.doEmoteEvent.Poll();
    this.kissFarmerEvent.Poll();
    this.synchronizedJumpEvent.Poll();
    this.renovateEvent.Poll();
    if (this.IsLocalPlayer)
    {
      if (this.currentLocation == null)
        return;
      this.hidden.Value = this.IsDedicatedPlayer || this.localBusMoving() || location.currentEvent != null && !location.currentEvent.isFestival || location.currentEvent != null && location.currentEvent.doingSecretSanta || Game1.locationRequest != null || !Game1.displayFarmer;
      this.isInBed.Value = this.currentLocation.doesTileHaveProperty(this.TilePoint.X, this.TilePoint.Y, "Bed", "Back") != null || this.sleptInTemporaryBed.Value;
      if (!Game1.options.allowStowing)
        this.netItemStowed.Value = false;
      this.hasMenuOpen.Value = Game1.activeClickableMenu != null;
    }
    if (this.IsSitting())
    {
      this.movementDirections.Clear();
      if (this.IsSitting() && !this.isStopSitting)
      {
        if (!this.sittingFurniture.IsSeatHere(this.currentLocation))
          this.StopSitting(false);
        else if (this.sittingFurniture is MapSeat sittingFurniture)
        {
          if (!((IEnumerable<ISittable>) this.currentLocation.mapSeats).Contains<ISittable>(this.sittingFurniture))
            this.StopSitting(false);
          else if (sittingFurniture.IsBlocked(this.currentLocation))
            this.StopSitting();
        }
      }
    }
    if (Game1.CurrentEvent == null && !this.bathingClothes.Value && !this.onBridge.Value)
      this.canOnlyWalk = false;
    if (this.noMovementPause > 0)
    {
      this.CanMove = false;
      this.noMovementPause -= time.ElapsedGameTime.Milliseconds;
      if (this.noMovementPause <= 0)
        this.CanMove = true;
    }
    if (this.freezePause > 0)
    {
      this.CanMove = false;
      this.freezePause -= time.ElapsedGameTime.Milliseconds;
      if (this.freezePause <= 0)
        this.CanMove = true;
    }
    if (this.sparklingText != null && this.sparklingText.update(time))
      this.sparklingText = (SparklingText) null;
    if (this.newLevelSparklingTexts.Count > 0 && this.sparklingText == null && !this.UsingTool && this.CanMove && Game1.activeClickableMenu == null)
    {
      this.sparklingText = new SparklingText(Game1.dialogueFont, Game1.content.LoadString("Strings\\StringsFromCSFiles:Farmer.cs.2003", (object) Farmer.getSkillDisplayNameFromIndex(this.newLevelSparklingTexts.Peek())), Color.White, Color.White, true);
      this.newLevelSparklingTexts.Dequeue();
    }
    if ((double) this.lerpPosition >= 0.0)
    {
      this.lerpPosition += (float) time.ElapsedGameTime.TotalSeconds;
      if ((double) this.lerpPosition >= (double) this.lerpDuration)
        this.lerpPosition = this.lerpDuration;
      this.Position = new Vector2(Utility.Lerp(this.lerpStartPosition.X, this.lerpEndPosition.X, this.lerpPosition / this.lerpDuration), Utility.Lerp(this.lerpStartPosition.Y, this.lerpEndPosition.Y, this.lerpPosition / this.lerpDuration));
      if ((double) this.lerpPosition >= (double) this.lerpDuration)
        this.lerpPosition = -1f;
    }
    if (this.isStopSitting && (double) this.lerpPosition < 0.0)
    {
      this.isStopSitting = false;
      if (this.sittingFurniture != null)
      {
        this.mapChairSitPosition.Value = new Vector2(-1f, -1f);
        this.sittingFurniture.RemoveSittingFarmer(this);
        this.sittingFurniture = (ISittable) null;
        this.isSitting.Value = false;
      }
    }
    if (this.isInBed.Value && Game1.IsMultiplayer && Game1.shouldTimePass())
    {
      this.regenTimer -= time.ElapsedGameTime.Milliseconds;
      if (this.regenTimer < 0)
      {
        this.regenTimer = 500;
        if ((double) this.stamina < (double) this.MaxStamina)
          ++this.stamina;
        if (this.health < this.maxHealth)
          ++this.health;
      }
    }
    this.FarmerSprite.checkForSingleAnimation(time);
    if (this.CanMove)
    {
      this.rotation = 0.0f;
      if (this.health <= 0 && !Game1.killScreen && Game1.timeOfDay < 2600)
      {
        if (this.IsSitting())
          this.StopSitting(false);
        this.CanMove = false;
        Game1.screenGlowOnce(Color.Red, true);
        Game1.killScreen = true;
        this.faceDirection(2);
        this.FarmerSprite.setCurrentFrame(5);
        this.jitterStrength = 1f;
        Game1.pauseTime = 3000f;
        Rumble.rumbleAndFade(0.75f, 1500f);
        this.freezePause = 8000;
        if (Game1.currentSong != null && Game1.currentSong.IsPlaying)
          Game1.currentSong.Stop(AudioStopOptions.Immediate);
        Game1.changeMusicTrack("silence");
        this.playNearbySoundAll("death");
        Game1.dialogueUp = false;
        ++Game1.stats.TimesUnconscious;
        if (Utility.GetDayOfPassiveFestival("DesertFestival") > 0 && Game1.player.currentLocation is MineShaft && Game1.mine.getMineArea() == 121)
        {
          float num1 = 0.2f;
          if (Game1.player.team.calicoStatueEffects.ContainsKey(5))
            num1 = 0.5f;
          int num2 = (int) ((double) num1 * (double) Game1.player.getItemCount("CalicoEgg"));
          Game1.player.Items.ReduceId("CalicoEgg", num2);
          this.itemsLostLastDeath.Clear();
          if (num2 > 0)
            this.itemsLostLastDeath.Add((Item) new Object("CalicoEgg", num2));
        }
        if (Game1.activeClickableMenu is GameMenu)
        {
          Game1.activeClickableMenu.emergencyShutDown();
          Game1.activeClickableMenu = (IClickableMenu) null;
        }
      }
      if (this.collisionNPC != null)
        this.collisionNPC.farmerPassesThrough = true;
      NPC npc;
      if (this.movementDirections.Count > 0 && !this.isRidingHorse() && (npc = location.isCollidingWithCharacter(this.nextPosition(this.FacingDirection))) != null)
      {
        this.charactercollisionTimer += time.ElapsedGameTime.Milliseconds;
        if (this.charactercollisionTimer > npc.getTimeFarmerMustPushBeforeStartShaking())
          npc.shake(50);
        if (this.charactercollisionTimer >= npc.getTimeFarmerMustPushBeforePassingThrough() && this.collisionNPC == null)
        {
          this.collisionNPC = npc;
          if (this.collisionNPC.Name.Equals("Bouncer") && this.currentLocation != null && this.currentLocation.name.Equals((object) "SandyHouse"))
          {
            this.collisionNPC.showTextAboveHead(Game1.content.LoadString("Strings\\StringsFromCSFiles:Farmer.cs.2010"));
            this.collisionNPC = (NPC) null;
            this.charactercollisionTimer = 0;
          }
          else if (this.collisionNPC.name.Equals((object) "Henchman") && this.currentLocation != null && this.currentLocation.name.Equals((object) "WitchSwamp"))
          {
            this.collisionNPC = (NPC) null;
            this.charactercollisionTimer = 0;
          }
          else if (this.collisionNPC is Raccoon)
          {
            this.collisionNPC = (NPC) null;
            this.charactercollisionTimer = 0;
          }
        }
      }
      else
      {
        this.charactercollisionTimer = 0;
        if (this.collisionNPC != null && location.isCollidingWithCharacter(this.nextPosition(this.FacingDirection)) == null)
        {
          this.collisionNPC.farmerPassesThrough = false;
          this.collisionNPC = (NPC) null;
        }
      }
    }
    if (Game1.shouldTimePass())
      MeleeWeapon.weaponsTypeUpdate(time);
    if (!Game1.eventUp || this.movementDirections.Count <= 0 || this.currentLocation.currentEvent == null || this.currentLocation.currentEvent.playerControlSequence || this.controller != null && this.controller.allowPlayerPathingInEvent)
    {
      this.lastPosition = this.Position;
      if (this.controller != null)
      {
        if (this.controller.update(time))
          this.controller = (PathFindController) null;
      }
      else if (this.controller == null)
        this.MovePosition(time, Game1.viewport, location);
    }
    if (Game1.actionsWhenPlayerFree.Count > 0 && this.IsLocalPlayer && !this.IsBusyDoingSomething())
    {
      Action action = Game1.actionsWhenPlayerFree[0];
      Game1.actionsWhenPlayerFree.RemoveAt(0);
      action();
    }
    this.updateCommon(time, location);
    this.position.Paused = this.FarmerSprite.PauseForSingleAnimation || this.UsingTool && !this.canStrafeForToolUse() || this.isEating;
    this.checkDamage(location);
  }

  private void updateCommon(GameTime time, GameLocation location)
  {
    if ((double) this.usernameDisplayTime > 0.0)
    {
      this.usernameDisplayTime -= (float) time.ElapsedGameTime.TotalSeconds;
      if ((double) this.usernameDisplayTime < 0.0)
        this.usernameDisplayTime = 0.0f;
    }
    if ((double) this.jitterStrength > 0.0)
      this.jitter = new Vector2((float) Game1.random.Next(-(int) ((double) this.jitterStrength * 100.0), (int) (((double) this.jitterStrength + 1.0) * 100.0)) / 100f, (float) Game1.random.Next(-(int) ((double) this.jitterStrength * 100.0), (int) (((double) this.jitterStrength + 1.0) * 100.0)) / 100f);
    if (this._wasSitting != this.isSitting.Value)
    {
      if (this._wasSitting)
      {
        this.yOffset = 0.0f;
        this.xOffset = 0.0f;
      }
      this._wasSitting = this.isSitting.Value;
    }
    if (this.yJumpOffset != 0)
    {
      this.yJumpVelocity -= !this.UsingTool || !this.canStrafeForToolUse() || this.movementDirections.Count <= 0 && (this.IsLocalPlayer || !this.IsRemoteMoving()) ? 0.5f : 0.25f;
      this.yJumpOffset -= (int) this.yJumpVelocity;
      if (this.yJumpOffset >= 0)
      {
        this.yJumpOffset = 0;
        this.yJumpVelocity = 0.0f;
      }
    }
    this.updateMovementAnimation(time);
    this.updateEmote(time);
    this.updateGlow();
    this.currentLocationRef.Update();
    if (this.exhausted.Value && (double) this.stamina <= 1.0)
    {
      this.currentEyes = 4;
      this.blinkTimer = -1000;
    }
    int blinkTimer = this.blinkTimer;
    TimeSpan timeSpan = time.ElapsedGameTime;
    int milliseconds1 = timeSpan.Milliseconds;
    this.blinkTimer = blinkTimer + milliseconds1;
    if (this.blinkTimer > 2200 && Game1.random.NextDouble() < 0.01)
    {
      this.blinkTimer = -150;
      this.currentEyes = 4;
    }
    else if (this.blinkTimer > -100)
      this.currentEyes = this.blinkTimer >= -50 ? (this.blinkTimer >= 0 ? 0 : 4) : 1;
    if (this.isCustomized.Value && this.isInBed.Value && !Game1.eventUp && (this.timerSinceLastMovement >= 3000 && Game1.timeOfDay >= 630 || this.timeWentToBed.Value != 0))
    {
      this.currentEyes = 1;
      this.blinkTimer = -10;
    }
    this.UpdateItemStow();
    if (this.swimming.Value)
    {
      timeSpan = time.TotalGameTime;
      this.yOffset = (float) (Math.Cos(timeSpan.TotalMilliseconds / 2000.0) * 4.0);
      int swimTimer1 = this.swimTimer;
      int swimTimer2 = this.swimTimer;
      timeSpan = time.ElapsedGameTime;
      int milliseconds2 = timeSpan.Milliseconds;
      this.swimTimer = swimTimer2 - milliseconds2;
      if (this.timerSinceLastMovement == 0)
      {
        if (swimTimer1 > 400 && this.swimTimer <= 400 && this.IsLocalPlayer)
          Game1.multiplayer.broadcastSprites(this.currentLocation, new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(0, 0, 64 /*0x40*/, 64 /*0x40*/), (float) (150.0 - ((double) Math.Abs(this.xVelocity) + (double) Math.Abs(this.yVelocity)) * 3.0), 8, 0, new Vector2(this.Position.X, (float) (this.StandingPixel.Y - 32 /*0x20*/)), false, Game1.random.NextBool(), 0.01f, 0.01f, Color.White, 1f, 3f / 1000f, 0.0f, 0.0f));
        if (this.swimTimer < 0)
        {
          this.swimTimer = 800;
          if (this.IsLocalPlayer)
          {
            this.playNearbySoundAll("slosh");
            Game1.multiplayer.broadcastSprites(this.currentLocation, new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(0, 0, 64 /*0x40*/, 64 /*0x40*/), (float) (150.0 - ((double) Math.Abs(this.xVelocity) + (double) Math.Abs(this.yVelocity)) * 3.0), 8, 0, new Vector2(this.Position.X, (float) (this.StandingPixel.Y - 32 /*0x20*/)), false, Game1.random.NextBool(), 0.01f, 0.01f, Color.White, 1f, 3f / 1000f, 0.0f, 0.0f));
          }
        }
      }
      else if (!Game1.eventUp && (Game1.activeClickableMenu == null || Game1.IsMultiplayer) && !Game1.paused)
      {
        if (this.timerSinceLastMovement > 800)
          this.currentEyes = 1;
        else if (this.timerSinceLastMovement > 700)
          this.currentEyes = 4;
        if (this.swimTimer < 0)
        {
          this.swimTimer = 100;
          if ((double) this.Stamina < (double) this.MaxStamina)
            ++this.Stamina;
          if (this.health < this.maxHealth)
            ++this.health;
        }
      }
    }
    if (!this.isMoving())
    {
      int sinceLastMovement = this.timerSinceLastMovement;
      timeSpan = time.ElapsedGameTime;
      int milliseconds3 = timeSpan.Milliseconds;
      this.timerSinceLastMovement = sinceLastMovement + milliseconds3;
    }
    else
      this.timerSinceLastMovement = 0;
    for (int index = this.Items.Count - 1; index >= 0; --index)
    {
      if (this.Items[index] is Tool tool)
        tool.tickUpdate(time, this);
    }
    if (this.TemporaryItem is Tool temporaryItem)
      temporaryItem.tickUpdate(time, this);
    this.rightRing.Value?.update(time, location, this);
    this.leftRing.Value?.update(time, location, this);
    if (Game1.shouldTimePass() && this.IsLocalPlayer)
    {
      foreach (Trinket trinketItem in this.trinketItems)
        trinketItem?.Update(this, time, location);
    }
    this.mount?.update(time, location);
    this.mount?.SyncPositionToRider();
    foreach (Companion companion in this.companions)
      companion.Update(time, location);
  }

  /// <summary>Get whether the player is engaged in any action and shouldn't be interrupted. This includes viewing a menu or event, fading to black, warping, using a tool, etc. If this returns false, we should be free to interrupt the player.</summary>
  public virtual bool IsBusyDoingSomething()
  {
    return Game1.eventUp || Game1.fadeToBlack || Game1.currentMinigame != null || Game1.activeClickableMenu != null || Game1.isWarping || this.UsingTool || Game1.killScreen || this.freezePause > 0 || !this.CanMove || this.FarmerSprite.PauseForSingleAnimation || this.usingSlingshot;
  }

  public void UpdateItemStow()
  {
    if (this._itemStowed == this.netItemStowed.Value)
      return;
    if (this.netItemStowed.Value && this.ActiveObject != null)
      this.ActiveObject.actionWhenStopBeingHeld(this);
    this._itemStowed = this.netItemStowed.Value;
    if (this.netItemStowed.Value)
      return;
    this.ActiveObject?.actionWhenBeingHeld(this);
  }

  /// <summary>Add a quest to the player's quest log, or log a warning if it doesn't exist.</summary>
  /// <param name="questId">The quest ID in <c>Data/Quests</c>.</param>
  public void addQuest(string questId)
  {
    if (this.hasQuest(questId))
      return;
    Quest questFromId = Quest.getQuestFromId(questId);
    if (questFromId == null)
    {
      Game1.log.Warn($"Can't add quest with ID '{questId}' because no such ID was found.");
    }
    else
    {
      this.questLog.Add(questFromId);
      if (!questFromId.IsHidden())
        Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Farmer.cs.2011"), 2));
      foreach (string constructedBuilding in (NetHashSet<string>) Game1.player.team.constructedBuildings)
        questFromId.OnBuildingExists(constructedBuilding);
    }
  }

  public void removeQuest(string questID)
  {
    this.questLog.RemoveWhere((Func<Quest, bool>) (quest => quest.id.Value == questID));
  }

  public void completeQuest(string questID)
  {
    for (int index = this.questLog.Count - 1; index >= 0; --index)
    {
      if (this.questLog[index].id.Value == questID)
        this.questLog[index].questComplete();
    }
  }

  public bool hasQuest(string id)
  {
    for (int index = this.questLog.Count - 1; index >= 0; --index)
    {
      if (this.questLog[index].id.Value == id)
        return true;
    }
    return false;
  }

  public bool hasNewQuestActivity()
  {
    foreach (SpecialOrder specialOrder in this.team.specialOrders)
    {
      if (!specialOrder.IsHidden() && (specialOrder.ShouldDisplayAsNew() || specialOrder.ShouldDisplayAsComplete()))
        return true;
    }
    foreach (Quest quest in (NetList<Quest, NetRef<Quest>>) this.questLog)
    {
      if (!quest.IsHidden() && (quest.showNew.Value || quest.completed.Value && !quest.destroy.Value))
        return true;
    }
    return false;
  }

  public float getMovementSpeed()
  {
    if (this.UsingTool && this.canStrafeForToolUse())
      return 2f;
    if (Game1.CurrentEvent == null || Game1.CurrentEvent.playerControlSequence)
    {
      this.movementMultiplier = 0.066f;
      float movementSpeed = !this.isRidingHorse() ? Math.Max(1f, ((float) this.speed + (Game1.eventUp ? 0.0f : this.addedSpeed + this.temporarySpeedBuff)) * this.movementMultiplier * (float) Game1.currentGameTime.ElapsedGameTime.Milliseconds) : Math.Max(1f, ((float) this.speed + (Game1.eventUp ? 0.0f : (float) ((double) this.addedSpeed + 4.5999999046325684 + (this.mount.ateCarrotToday ? 0.40000000596046448 : 0.0) + (this.stats.Get("Book_Horse") > 0U ? 0.5 : 0.0)))) * this.movementMultiplier * (float) Game1.currentGameTime.ElapsedGameTime.Milliseconds);
      if (this.movementDirections.Count > 1)
        movementSpeed *= 0.707f;
      if (Game1.CurrentEvent == null && this.hasBuff("19"))
        movementSpeed = 0.0f;
      return movementSpeed;
    }
    float movementSpeed1 = Math.Max(1f, (float) this.speed + (Game1.eventUp ? (float) Math.Max(0, Game1.CurrentEvent.farmerAddedSpeed - 2) : this.addedSpeed + (this.isRidingHorse() ? 5f : this.temporarySpeedBuff)));
    if (this.movementDirections.Count > 1)
      movementSpeed1 = (float) Math.Max(1, (int) Math.Sqrt(2.0 * ((double) movementSpeed1 * (double) movementSpeed1)) / 2);
    return movementSpeed1;
  }

  public bool isWearingRing(string itemId)
  {
    if (this.rightRing.Value != null && this.rightRing.Value.GetsEffectOfRing(itemId))
      return true;
    return this.leftRing.Value != null && this.leftRing.Value.GetsEffectOfRing(itemId);
  }

  public int getNumberOfWornRingsWithID(string itemId)
  {
    int ofWornRingsWithId = 0;
    if (this.rightRing.Value != null && this.rightRing.Value.GetsEffectOfRing(itemId))
      ++ofWornRingsWithId;
    if (this.leftRing.Value != null && this.leftRing.Value.GetsEffectOfRing(itemId))
      ++ofWornRingsWithId;
    return ofWornRingsWithId;
  }

  public override void Halt()
  {
    if (!this.FarmerSprite.PauseForSingleAnimation && !this.isRidingHorse() && !this.UsingTool)
      base.Halt();
    this.movementDirections.Clear();
    if (!this.isEmoteAnimating && !this.UsingTool)
      this.stopJittering();
    this.armOffset = Vector2.Zero;
    if (this.isRidingHorse())
    {
      this.mount.Halt();
      this.mount.Sprite.CurrentAnimation = (List<FarmerSprite.AnimationFrame>) null;
    }
    if (!this.IsSitting())
      return;
    this.ShowSitting();
  }

  public void stopJittering()
  {
    this.jitterStrength = 0.0f;
    this.jitter = Vector2.Zero;
  }

  public override Microsoft.Xna.Framework.Rectangle nextPosition(int direction)
  {
    Microsoft.Xna.Framework.Rectangle boundingBox = this.GetBoundingBox();
    switch (direction)
    {
      case 0:
        boundingBox.Y -= (int) Math.Ceiling((double) this.getMovementSpeed());
        break;
      case 1:
        boundingBox.X += (int) Math.Ceiling((double) this.getMovementSpeed());
        break;
      case 2:
        boundingBox.Y += (int) Math.Ceiling((double) this.getMovementSpeed());
        break;
      case 3:
        boundingBox.X -= (int) Math.Ceiling((double) this.getMovementSpeed());
        break;
    }
    return boundingBox;
  }

  public Microsoft.Xna.Framework.Rectangle nextPositionHalf(int direction)
  {
    Microsoft.Xna.Framework.Rectangle boundingBox = this.GetBoundingBox();
    switch (direction)
    {
      case 0:
        boundingBox.Y -= (int) Math.Ceiling((double) this.getMovementSpeed() / 2.0);
        break;
      case 1:
        boundingBox.X += (int) Math.Ceiling((double) this.getMovementSpeed() / 2.0);
        break;
      case 2:
        boundingBox.Y += (int) Math.Ceiling((double) this.getMovementSpeed() / 2.0);
        break;
      case 3:
        boundingBox.X -= (int) Math.Ceiling((double) this.getMovementSpeed() / 2.0);
        break;
    }
    return boundingBox;
  }

  /// <summary>
  /// 
  /// </summary>
  /// <param name="skillType">e.g. farming, fishing, foraging</param>
  /// <param name="skillLevel">5 or 10</param>
  /// <returns></returns>
  public int getProfessionForSkill(int skillType, int skillLevel)
  {
    switch (skillLevel)
    {
      case 5:
        switch (skillType)
        {
          case 0:
            if (this.professions.Contains(0))
              return 0;
            if (this.professions.Contains(1))
              return 1;
            break;
          case 1:
            if (this.professions.Contains(6))
              return 6;
            if (this.professions.Contains(7))
              return 7;
            break;
          case 2:
            if (this.professions.Contains(12))
              return 12;
            if (this.professions.Contains(13))
              return 13;
            break;
          case 3:
            if (this.professions.Contains(18))
              return 18;
            if (this.professions.Contains(19))
              return 19;
            break;
          case 4:
            if (this.professions.Contains(24))
              return 24;
            if (this.professions.Contains(25))
              return 25;
            break;
        }
        break;
      case 10:
        switch (skillType)
        {
          case 0:
            if (this.professions.Contains(1))
            {
              if (this.professions.Contains(4))
                return 4;
              if (this.professions.Contains(5))
                return 5;
              break;
            }
            if (this.professions.Contains(2))
              return 2;
            if (this.professions.Contains(3))
              return 3;
            break;
          case 1:
            if (this.professions.Contains(6))
            {
              if (this.professions.Contains(8))
                return 8;
              if (this.professions.Contains(9))
                return 9;
              break;
            }
            if (this.professions.Contains(10))
              return 10;
            if (this.professions.Contains(11))
              return 11;
            break;
          case 2:
            if (this.professions.Contains(12))
            {
              if (this.professions.Contains(14))
                return 14;
              if (this.professions.Contains(15))
                return 15;
              break;
            }
            if (this.professions.Contains(16 /*0x10*/))
              return 16 /*0x10*/;
            if (this.professions.Contains(17))
              return 17;
            break;
          case 3:
            if (this.professions.Contains(18))
            {
              if (this.professions.Contains(20))
                return 20;
              if (this.professions.Contains(21))
                return 21;
              break;
            }
            if (this.professions.Contains(23))
              return 23;
            if (this.professions.Contains(22))
              return 22;
            break;
          case 4:
            if (this.professions.Contains(24))
            {
              if (this.professions.Contains(26))
                return 26;
              if (this.professions.Contains(27))
                return 27;
              break;
            }
            if (this.professions.Contains(28))
              return 28;
            if (this.professions.Contains(29))
              return 29;
            break;
        }
        break;
    }
    return -1;
  }

  public void behaviorOnMovement(int direction) => this.hasMoved = true;

  public void OnEmoteAnimationEnd(Farmer farmer)
  {
    if (farmer != this || !this.isEmoteAnimating)
      return;
    this.EndEmoteAnimation();
  }

  public void EndEmoteAnimation()
  {
    if (!this.isEmoteAnimating)
      return;
    if ((double) this.jitterStrength > 0.0)
      this.stopJittering();
    if (this.yJumpOffset != 0)
    {
      this.yJumpOffset = 0;
      this.yJumpVelocity = 0.0f;
    }
    this.FarmerSprite.PauseForSingleAnimation = false;
    this.FarmerSprite.StopAnimation();
    this.isEmoteAnimating = false;
  }

  private void broadcastHaltAnimation(Farmer who)
  {
    if (this.IsLocalPlayer)
      this.haltAnimationEvent.Fire();
    else
      Farmer.completelyStopAnimating(who);
  }

  private void performHaltAnimation() => this.completelyStopAnimatingOrDoingAction();

  public void performKissFarmer(long otherPlayerID)
  {
    Farmer player = Game1.GetPlayer(otherPlayerID);
    if (player == null)
      return;
    bool flag = this.StandingPixel.X < player.StandingPixel.X;
    this.PerformKiss(flag ? 1 : 3);
    player.PerformKiss(flag ? 3 : 1);
  }

  public void PerformKiss(int facingDirection)
  {
    if (Game1.eventUp || this.UsingTool || this.IsLocalPlayer && Game1.activeClickableMenu != null || this.isRidingHorse() || this.IsSitting() || this.IsEmoting || !this.CanMove)
      return;
    this.CanMove = false;
    this.FarmerSprite.PauseForSingleAnimation = false;
    this.faceDirection(facingDirection);
    this.FarmerSprite.animateOnce(new FarmerSprite.AnimationFrame[2]
    {
      new FarmerSprite.AnimationFrame(101, 1000, 0, false, this.FacingDirection == 3),
      new FarmerSprite.AnimationFrame(6, 1, false, this.FacingDirection == 3, new AnimatedSprite.endOfAnimationBehavior(this.broadcastHaltAnimation))
    });
    if (Stats.AllowRetroactiveAchievements)
      return;
    Game1.stats.checkForFullHouseAchievement(true);
  }

  public override void MovePosition(
    GameTime time,
    xTile.Dimensions.Rectangle viewport,
    GameLocation currentLocation)
  {
    if (this.IsSitting())
      return;
    if (Game1.CurrentEvent == null || Game1.CurrentEvent.playerControlSequence)
    {
      if (Game1.shouldTimePass() && this.temporarilyInvincible)
      {
        if (this.temporaryInvincibilityTimer < 0)
          this.currentTemporaryInvincibilityDuration = 1200;
        this.temporaryInvincibilityTimer += time.ElapsedGameTime.Milliseconds;
        if (this.temporaryInvincibilityTimer > this.currentTemporaryInvincibilityDuration)
        {
          this.temporarilyInvincible = false;
          this.temporaryInvincibilityTimer = 0;
        }
      }
    }
    else if (this.temporarilyInvincible)
    {
      this.temporarilyInvincible = false;
      this.temporaryInvincibilityTimer = 0;
    }
    if (Game1.activeClickableMenu != null && (Game1.CurrentEvent == null || Game1.CurrentEvent.playerControlSequence))
      return;
    if (this.isRafting)
    {
      this.moveRaft(currentLocation, time);
    }
    else
    {
      if ((double) this.xVelocity != 0.0 || (double) this.yVelocity != 0.0)
      {
        if (double.IsNaN((double) this.xVelocity) || double.IsNaN((double) this.yVelocity))
        {
          this.xVelocity = 0.0f;
          this.yVelocity = 0.0f;
        }
        Microsoft.Xna.Framework.Rectangle boundingBox = this.GetBoundingBox();
        Microsoft.Xna.Framework.Rectangle position = Microsoft.Xna.Framework.Rectangle.Union(new Microsoft.Xna.Framework.Rectangle(boundingBox.X + (int) Math.Floor((double) this.xVelocity), boundingBox.Y - (int) Math.Floor((double) this.yVelocity), boundingBox.Width, boundingBox.Height), new Microsoft.Xna.Framework.Rectangle(boundingBox.X + (int) Math.Ceiling((double) this.xVelocity), boundingBox.Y - (int) Math.Ceiling((double) this.yVelocity), boundingBox.Width, boundingBox.Height));
        if (!currentLocation.isCollidingPosition(position, viewport, true, -1, false, (Character) this))
        {
          this.position.X += this.xVelocity;
          this.position.Y -= this.yVelocity;
          this.xVelocity -= this.xVelocity / 16f;
          this.yVelocity -= this.yVelocity / 16f;
          if ((double) Math.Abs(this.xVelocity) <= 0.05000000074505806)
            this.xVelocity = 0.0f;
          if ((double) Math.Abs(this.yVelocity) <= 0.05000000074505806)
            this.yVelocity = 0.0f;
        }
        else
        {
          this.xVelocity -= this.xVelocity / 16f;
          this.yVelocity -= this.yVelocity / 16f;
          if ((double) Math.Abs(this.xVelocity) <= 0.05000000074505806)
            this.xVelocity = 0.0f;
          if ((double) Math.Abs(this.yVelocity) <= 0.05000000074505806)
            this.yVelocity = 0.0f;
        }
      }
      if (this.CanMove || Game1.eventUp || this.controller != null || this.canStrafeForToolUse())
      {
        this.temporaryPassableTiles.ClearNonIntersecting(this.GetBoundingBox());
        float movementSpeed = this.getMovementSpeed();
        this.temporarySpeedBuff = 0.0f;
        if (this.movementDirections.Contains(0) && this.MovePositionImpl(0, 0.0f, -movementSpeed, time, viewport) || this.movementDirections.Contains(2) && this.MovePositionImpl(2, 0.0f, movementSpeed, time, viewport) || this.movementDirections.Contains(1) && this.MovePositionImpl(1, movementSpeed, 0.0f, time, viewport) || this.movementDirections.Contains(3) && this.MovePositionImpl(3, -movementSpeed, 0.0f, time, viewport))
          return;
      }
      this.FarmerSprite.intervalModifier = this.movementDirections.Count <= 0 || this.UsingTool ? 1f : (float) (1.0 - (this.running ? 0.025499999523162842 : 0.02500000037252903) * ((double) Math.Max(1f, ((float) this.speed + (Game1.eventUp ? 0.0f : (float) (int) this.addedSpeed + (this.isRidingHorse() ? 4.6f : 0.0f))) * this.movementMultiplier * (float) Game1.currentGameTime.ElapsedGameTime.Milliseconds) * 1.25));
      if (currentLocation == null || !currentLocation.isFarmerCollidingWithAnyCharacter())
        return;
      this.temporaryPassableTiles.Add(new Microsoft.Xna.Framework.Rectangle(this.TilePoint.X * 64 /*0x40*/, this.TilePoint.Y * 64 /*0x40*/, 64 /*0x40*/, 64 /*0x40*/));
    }
  }

  public bool canStrafeForToolUse()
  {
    if (this.toolHold.Value == 0 || !this.canReleaseTool)
      return false;
    return this.toolPower.Value >= 1 || this.toolHoldStartTime.Value - this.toolHold.Value > 150;
  }

  /// <summary>Handle a player's movement in a specific direction, after the game has already checked whether movement is allowed.</summary>
  /// <param name="direction">The direction the player is moving in, matching a constant like <see cref="F:StardewValley.Game1.up" />.</param>
  /// <param name="movementSpeedX">The player's movement speed along the X axis for this direction.</param>
  /// <param name="movementSpeedY">The player's movement speed along the Y axis for this direction.</param>
  /// <param name="time">The elapsed game time.</param>
  /// <param name="viewport">The pixel area being viewed relative to the top-left corner of the map.</param>
  /// <returns>Returns whether the movement was fully handled (e.g. a warp was activated), so no further movement logic should be applied.</returns>
  protected virtual bool MovePositionImpl(
    int direction,
    float movementSpeedX,
    float movementSpeedY,
    GameTime time,
    xTile.Dimensions.Rectangle viewport)
  {
    Microsoft.Xna.Framework.Rectangle position1 = this.nextPosition(direction);
    Warp w = Game1.currentLocation.isCollidingWithWarp(position1, (Character) this);
    if (w != null && this.IsLocalPlayer)
    {
      if (Game1.eventUp)
      {
        bool? isFestival = Game1.CurrentEvent?.isFestival;
        if (isFestival.HasValue && isFestival.GetValueOrDefault())
        {
          Game1.CurrentEvent.TryStartEndFestivalDialogue(this);
          goto label_5;
        }
      }
      this.warpFarmer(w, direction);
label_5:
      return true;
    }
    int num;
    if (Game1.eventUp)
    {
      bool? nullable = Game1.CurrentEvent?.isFestival;
      if (nullable.HasValue && !nullable.GetValueOrDefault())
      {
        nullable = Game1.CurrentEvent?.playerControlSequence;
        num = !nullable.HasValue ? 0 : (!nullable.GetValueOrDefault() ? 1 : 0);
        goto label_10;
      }
    }
    num = 0;
label_10:
    bool flag1 = num != 0;
    if (((!this.currentLocation.isCollidingPosition(position1, viewport, true, 0, false, (Character) this) ? 1 : (this.ignoreCollisions ? 1 : 0)) | (flag1 ? 1 : 0)) != 0)
    {
      this.position.X += movementSpeedX;
      this.position.Y += movementSpeedY;
      this.behaviorOnMovement(direction);
      return false;
    }
    if (!this.currentLocation.isCollidingPosition(this.nextPositionHalf(direction), viewport, true, 0, false, (Character) this))
    {
      this.position.X += movementSpeedX / 2f;
      this.position.Y += movementSpeedY / 2f;
      this.behaviorOnMovement(direction);
      return false;
    }
    if (this.movementDirections.Count == 1)
    {
      Microsoft.Xna.Framework.Rectangle position2 = position1;
      if (direction == 0 || direction == 2)
      {
        position2.Width /= 4;
        bool flag2 = this.currentLocation.isCollidingPosition(position2, viewport, true, 0, false, (Character) this);
        position2.X += position2.Width * 3;
        bool flag3 = this.currentLocation.isCollidingPosition(position2, viewport, true, 0, false, (Character) this);
        if (flag2 && !flag3 && !this.currentLocation.isCollidingPosition(this.nextPosition(1), viewport, true, 0, false, (Character) this))
          this.position.X += (float) this.speed * ((float) time.ElapsedGameTime.Milliseconds / 64f);
        else if (flag3 && !flag2 && !this.currentLocation.isCollidingPosition(this.nextPosition(3), viewport, true, 0, false, (Character) this))
          this.position.X -= (float) this.speed * ((float) time.ElapsedGameTime.Milliseconds / 64f);
      }
      else
      {
        position2.Height /= 4;
        bool flag4 = this.currentLocation.isCollidingPosition(position2, viewport, true, 0, false, (Character) this);
        position2.Y += position2.Height * 3;
        bool flag5 = this.currentLocation.isCollidingPosition(position2, viewport, true, 0, false, (Character) this);
        if (flag4 && !flag5 && !this.currentLocation.isCollidingPosition(this.nextPosition(2), viewport, true, 0, false, (Character) this))
          this.position.Y += (float) this.speed * ((float) time.ElapsedGameTime.Milliseconds / 64f);
        else if (flag5 && !flag4 && !this.currentLocation.isCollidingPosition(this.nextPosition(0), viewport, true, 0, false, (Character) this))
          this.position.Y -= (float) this.speed * ((float) time.ElapsedGameTime.Milliseconds / 64f);
      }
    }
    return false;
  }

  public void updateMovementAnimation(GameTime time)
  {
    TimeSpan elapsedGameTime;
    if (this._emoteGracePeriod > 0)
    {
      int emoteGracePeriod = this._emoteGracePeriod;
      elapsedGameTime = time.ElapsedGameTime;
      int milliseconds = elapsedGameTime.Milliseconds;
      this._emoteGracePeriod = emoteGracePeriod - milliseconds;
    }
    if (this.isEmoteAnimating && ((this.IsLocalPlayer ? (this.movementDirections.Count > 0 ? 1 : 0) : (this.IsRemoteMoving() ? 1 : 0)) != 0 && this._emoteGracePeriod <= 0 || !this.FarmerSprite.PauseForSingleAnimation))
      this.EndEmoteAnimation();
    bool flag = this.IsCarrying();
    if (!this.isRidingHorse())
      this.xOffset = 0.0f;
    if (this.CurrentTool is FishingRod currentTool && (currentTool.isTimingCast || currentTool.isCasting))
      currentTool.setTimingCastAnimation(this);
    else if (this.FarmerSprite.PauseForSingleAnimation || this.UsingTool)
    {
      if (!this.UsingTool || !this.canStrafeForToolUse() || this.movementDirections.Count <= 0 && (this.IsLocalPlayer || !this.IsRemoteMoving()) || this.yJumpOffset != 0)
        return;
      this.jumpWithoutSound(2.5f);
    }
    else if (this.IsSitting())
      this.ShowSitting();
    else if (this.IsLocalPlayer && !this.CanMove && !Game1.eventUp)
    {
      if (this.isRidingHorse() && this.mount != null && !this.isAnimatingMount)
      {
        this.showRiding();
      }
      else
      {
        if (!flag)
          return;
        this.showCarrying();
      }
    }
    else
    {
      if (this.IsLocalPlayer || this.isFakeEventActor)
      {
        this.moveUp = this.movementDirections.Contains(0);
        this.moveRight = this.movementDirections.Contains(1);
        this.moveDown = this.movementDirections.Contains(2);
        this.moveLeft = this.movementDirections.Contains(3);
        if (this.moveLeft)
          this.FacingDirection = 3;
        else if (this.moveRight)
          this.FacingDirection = 1;
        else if (this.moveUp)
          this.FacingDirection = 0;
        else if (this.moveDown)
          this.FacingDirection = 2;
        if (this.isRidingHorse() && !this.mount.dismounting.Value)
          this.speed = 2;
      }
      else
      {
        this.moveLeft = this.IsRemoteMoving() && this.FacingDirection == 3;
        this.moveRight = this.IsRemoteMoving() && this.FacingDirection == 1;
        this.moveUp = this.IsRemoteMoving() && this.FacingDirection == 0;
        this.moveDown = this.IsRemoteMoving() && this.FacingDirection == 2;
        int num1 = this.moveUp || this.moveRight || this.moveDown ? 1 : (this.moveLeft ? 1 : 0);
        double num2 = (double) this.position.CurrentInterpolationSpeed();
        elapsedGameTime = Game1.currentGameTime.ElapsedGameTime;
        double num3 = (double) elapsedGameTime.Milliseconds * 0.065999999642372131;
        float num4 = (float) (num2 / num3);
        this.running = (double) Math.Abs(num4 - 5f) < (double) Math.Abs(num4 - 2f) && !this.bathingClothes.Value && !this.onBridge.Value;
        if (num1 == 0)
          this.FarmerSprite.StopAnimation();
      }
      if (this.hasBuff("19"))
      {
        this.running = false;
        this.moveUp = false;
        this.moveDown = false;
        this.moveLeft = false;
        this.moveRight = false;
      }
      if (this.FarmerSprite.PauseForSingleAnimation || this.UsingTool)
        return;
      if (this.isRidingHorse() && !this.mount.dismounting.Value)
        this.showRiding();
      else if (this.moveLeft && this.running && !flag)
        this.FarmerSprite.animate(56, time);
      else if (this.moveRight && this.running && !flag)
        this.FarmerSprite.animate(40, time);
      else if (this.moveUp && this.running && !flag)
        this.FarmerSprite.animate(48 /*0x30*/, time);
      else if (this.moveDown && this.running && !flag)
        this.FarmerSprite.animate(32 /*0x20*/, time);
      else if (this.moveLeft && this.running)
        this.FarmerSprite.animate(152, time);
      else if (this.moveRight && this.running)
        this.FarmerSprite.animate(136, time);
      else if (this.moveUp && this.running)
        this.FarmerSprite.animate(144 /*0x90*/, time);
      else if (this.moveDown && this.running)
        this.FarmerSprite.animate(128 /*0x80*/, time);
      else if (this.moveLeft && !flag)
        this.FarmerSprite.animate(24, time);
      else if (this.moveRight && !flag)
        this.FarmerSprite.animate(8, time);
      else if (this.moveUp && !flag)
        this.FarmerSprite.animate(16 /*0x10*/, time);
      else if (this.moveDown && !flag)
        this.FarmerSprite.animate(0, time);
      else if (this.moveLeft)
        this.FarmerSprite.animate(120, time);
      else if (this.moveRight)
        this.FarmerSprite.animate(104, time);
      else if (this.moveUp)
        this.FarmerSprite.animate(112 /*0x70*/, time);
      else if (this.moveDown)
        this.FarmerSprite.animate(96 /*0x60*/, time);
      else if (flag)
        this.showCarrying();
      else
        this.showNotCarrying();
    }
  }

  public bool IsCarrying()
  {
    return this.mount == null && !this.isAnimatingMount && !this.IsSitting() && !this.onBridge.Value && this.ActiveObject != null && !Game1.eventUp && !Game1.killScreen && this.ActiveObject.IsHeldOverHead();
  }

  public void doneEating()
  {
    this.isEating = false;
    this.tempFoodItemTextureName.Value = (string) null;
    this.completelyStopAnimatingOrDoingAction();
    this.forceCanMove();
    if (this.mostRecentlyGrabbedItem == null || !this.IsLocalPlayer)
      return;
    Object itemToEat = this.itemToEat as Object;
    if (itemToEat.QualifiedItemId == "(O)434")
    {
      Game1.stats.checkForStardropAchievement(true);
      this.yOffset = 0.0f;
      this.yJumpOffset = 0;
      Game1.changeMusicTrack("none");
      Game1.playSound("stardrop");
      string str = Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs." + Game1.random.Choose<string>("3094", "3095"));
      if (this.favoriteThing.Value != null)
        str = !this.favoriteThing.Value.Contains("Stardew") ? (!this.favoriteThing.Equals((object) "ConcernedApe") ? str + this.favoriteThing.Value : Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3099")) : Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3097");
      DelayedAction.showDialogueAfterDelay(Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3100") + str + Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3101"), 6000);
      this.maxStamina.Value += 34;
      this.stamina = (float) this.MaxStamina;
      this.FarmerSprite.animateOnce(new FarmerSprite.AnimationFrame[1]
      {
        new FarmerSprite.AnimationFrame(57, 6000)
      });
      this.startGlowing(new Color(200, 0, (int) byte.MaxValue), false, 0.1f);
      this.jitterStrength = 1f;
      Game1.staminaShakeTimer = 12000;
      Game1.screenGlowOnce(new Color(200, 0, (int) byte.MaxValue), true);
      this.CanMove = false;
      this.freezePause = 8000;
      this.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(368, 16 /*0x10*/, 16 /*0x10*/, 16 /*0x10*/), 60f, 8, 40, this.Position + new Vector2(-8f, (float) sbyte.MinValue), false, false, 1f, 0.0f, Color.White, 4f, 0.0075f, 0.0f, 0.0f)
      {
        alpha = 0.75f,
        alphaFade = 1f / 400f,
        motion = new Vector2(0.0f, -0.25f)
      });
      if (Game1.displayHUD && !Game1.eventUp)
      {
        for (int index = 0; index < 40; ++index)
        {
          TemporaryAnimatedSpriteList overlayTempSprites = Game1.uiOverlayTempSprites;
          int rowInAnimationTexture = Game1.random.Next(10, 12);
          Microsoft.Xna.Framework.Rectangle titleSafeArea = Game1.graphics.GraphicsDevice.Viewport.TitleSafeArea;
          double x = (double) titleSafeArea.Right / (double) Game1.options.uiScale - 48.0 - 8.0 - (double) Game1.random.Next(64 /*0x40*/);
          double num1 = (double) Game1.random.Next(-64, 64 /*0x40*/);
          titleSafeArea = Game1.graphics.GraphicsDevice.Viewport.TitleSafeArea;
          double num2 = (double) titleSafeArea.Bottom / (double) Game1.options.uiScale;
          double y = num1 + num2 - 224.0 - 16.0 - (double) (int) ((double) (this.MaxStamina - 270) * 0.715);
          Vector2 position = new Vector2((float) x, (float) y);
          Color color = Game1.random.Choose<Color>(Color.White, Color.Lime);
          overlayTempSprites.Add(new TemporaryAnimatedSprite(rowInAnimationTexture, position, color, animationInterval: 50f)
          {
            layerDepth = 1f,
            delayBeforeAnimationStart = 200 * index,
            interval = 100f,
            local = true
          });
        }
      }
      Point tilePoint = this.TilePoint;
      Utility.addSprinklesToLocation(this.currentLocation, tilePoint.X, tilePoint.Y, 9, 9, 6000, 100, new Color(200, 0, (int) byte.MaxValue), motionTowardCenter: true);
      DelayedAction.stopFarmerGlowing(6000);
      Utility.addSprinklesToLocation(this.currentLocation, tilePoint.X, tilePoint.Y, 9, 9, 6000, 300, Color.Cyan, motionTowardCenter: true);
      this.mostRecentlyGrabbedItem = (Item) null;
    }
    else
    {
      if (itemToEat.HasContextTag("ginger_item"))
        this.buffs.Remove("25");
      foreach (Buff foodOrDrinkBuff in itemToEat.GetFoodOrDrinkBuffs())
        this.applyBuff(foodOrDrinkBuff);
      switch (itemToEat.QualifiedItemId)
      {
        case "(O)773":
          this.health = this.maxHealth;
          break;
        case "(O)351":
          this.exhausted.Value = false;
          break;
        case "(O)349":
          this.Stamina = (float) this.MaxStamina;
          break;
      }
      float stamina = this.Stamina;
      int health = this.health;
      int num3 = itemToEat.staminaRecoveredOnConsumption();
      int num4 = itemToEat.healthRecoveredOnConsumption();
      if (Utility.GetDayOfPassiveFestival("DesertFestival") > 0 && this.currentLocation is MineShaft && Game1.mine.getMineArea() == 121 && this.team.calicoStatueEffects.ContainsKey(6))
      {
        num3 = Math.Max(1, num3 / 2);
        num4 = Math.Max(1, num4 / 2);
      }
      this.Stamina = Math.Min((float) this.MaxStamina, this.Stamina + (float) num3);
      this.health = Math.Min(this.maxHealth, this.health + num4);
      if ((double) stamina < (double) this.Stamina)
        Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3116", (object) (int) ((double) this.Stamina - (double) stamina)), 4));
      if (health < this.health)
        Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3118", (object) (this.health - health)), 5));
    }
    if (itemToEat == null || itemToEat.Edibility >= 0)
      return;
    this.CanMove = false;
    this.sickAnimationEvent.Fire();
  }

  /// <summary>Perform an action for each active non-completed quest in the local player's log.</summary>
  /// <param name="check">The action to perform on the quest. This should return whether the quest state changed.</param>
  /// <param name="onlyOneQuest">Whether to stop after any quest is updated.</param>
  /// <returns>Returns whether a quest was updated (e.g. closer to completion).</returns>
  public virtual bool NotifyQuests(Func<Quest, bool> check, bool onlyOneQuest = false)
  {
    bool flag = false;
    for (int index = this.questLog.Count - 1; index >= 0; --index)
    {
      Quest quest = this.questLog[index];
      if (!quest.completed.Value)
      {
        if (quest == null)
          this.questLog.RemoveAt(index);
        else if (check(quest))
        {
          flag = true;
          if (onlyOneQuest)
            break;
        }
      }
    }
    return flag;
  }

  public virtual void AddCompanion(Companion companion)
  {
    if (this.companions.Contains(companion))
      return;
    companion.InitializeCompanion(this);
    this.companions.Add(companion);
  }

  public virtual void RemoveCompanion(Companion companion)
  {
    if (!this.companions.Contains(companion))
      return;
    this.companions.Remove(companion);
    companion.CleanupCompanion();
  }

  public static void completelyStopAnimating(Farmer who)
  {
    who.completelyStopAnimatingOrDoingAction();
  }

  public void completelyStopAnimatingOrDoingAction()
  {
    this.CanMove = !Game1.eventUp;
    if (this.isEmoteAnimating)
      this.EndEmoteAnimation();
    if (this.UsingTool)
    {
      this.EndUsingTool();
      if (this.CurrentTool is FishingRod currentTool)
        currentTool.resetState();
    }
    if (this.usingSlingshot && this.CurrentTool is Slingshot currentTool1)
      currentTool1.finish();
    this.UsingTool = false;
    this.isEating = false;
    this.FarmerSprite.PauseForSingleAnimation = false;
    this.usingSlingshot = false;
    this.canReleaseTool = false;
    this.Halt();
    this.Sprite.StopAnimation();
    if (this.CurrentTool is MeleeWeapon currentTool2)
      currentTool2.isOnSpecial = false;
    this.stopJittering();
  }

  public void doEmote(int whichEmote)
  {
    if (Game1.eventUp || this.isEmoting)
      return;
    this.isEmoting = true;
    this.currentEmote = whichEmote;
    this.currentEmoteFrame = 0;
    this.emoteInterval = 0.0f;
  }

  public void performTenMinuteUpdate()
  {
  }

  public void setRunning(bool isRunning, bool force = false)
  {
    if (this.canOnlyWalk || this.bathingClothes.Value && !this.running || Game1.CurrentEvent != null & isRunning && !Game1.CurrentEvent.isFestival && !Game1.CurrentEvent.playerControlSequence && (this.controller == null || !this.controller.allowPlayerPathingInEvent))
      return;
    if (this.isRidingHorse())
      this.running = true;
    else if ((double) this.stamina <= 0.0)
    {
      this.speed = 2;
      if (this.running)
        this.Halt();
      this.running = false;
    }
    else if (force || this.CanMove && !this.isEating && Game1.currentLocation != null && (Game1.currentLocation.currentEvent == null || Game1.currentLocation.currentEvent.playerControlSequence) && (isRunning || !this.UsingTool) && (this.Sprite == null || !((FarmerSprite) this.Sprite).PauseForSingleAnimation))
    {
      this.running = isRunning;
      if (this.running)
        this.speed = 5;
      else
        this.speed = 2;
    }
    else
    {
      if (!this.UsingTool)
        return;
      this.running = isRunning;
      if (this.running)
        this.speed = 5;
      else
        this.speed = 2;
    }
  }

  public void addSeenResponse(string id) => this.dialogueQuestionsAnswered.Add(id);

  public void eatObject(Object o, bool overrideFullness = false)
  {
    if (o?.QualifiedItemId == "(O)434")
    {
      Game1.MusicDuckTimer = 10000f;
      Game1.changeMusicTrack("none");
      Game1.multiplayer.globalChatInfoMessage("Stardrop", this.Name);
    }
    if (this.getFacingDirection() != 2)
      this.faceDirection(2);
    this.itemToEat = (Item) o;
    this.mostRecentlyGrabbedItem = (Item) o;
    this.forceCanMove();
    this.completelyStopAnimatingOrDoingAction();
    ObjectData objectData;
    if ((!Game1.objectData.TryGetValue(o.ItemId, out objectData) ? 0 : (objectData.IsDrink ? 1 : 0)) != 0)
    {
      if (this.IsLocalPlayer && this.hasBuff("7") && !overrideFullness)
      {
        Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.2898")));
        return;
      }
      this.drinkAnimationEvent.Fire(o.getOne() as Object);
    }
    else if (o.Edibility != -300)
    {
      if (this.hasBuff("6") && !overrideFullness)
      {
        Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.2899")));
        return;
      }
      this.eatAnimationEvent.Fire(o.getOne() as Object);
    }
    this.freezePause = 20000;
    this.CanMove = false;
    this.isEating = true;
  }

  /// <inheritdoc />
  public override void DrawShadow(SpriteBatch b)
  {
    float num = this.getDrawLayer() - 1E-06f;
    SpriteBatch spriteBatch = b;
    Texture2D shadowTexture = Game1.shadowTexture;
    Vector2 local = Game1.GlobalToLocal(this.GetShadowOffset() + this.Position + new Vector2(32f, 24f));
    Microsoft.Xna.Framework.Rectangle? sourceRectangle = new Microsoft.Xna.Framework.Rectangle?(Game1.shadowTexture.Bounds);
    Color white = Color.White;
    Microsoft.Xna.Framework.Rectangle bounds = Game1.shadowTexture.Bounds;
    double x = (double) bounds.Center.X;
    bounds = Game1.shadowTexture.Bounds;
    double y = (double) bounds.Center.Y;
    Vector2 origin = new Vector2((float) x, (float) y);
    double scale = 4.0 - (!this.running && !this.UsingTool || this.FarmerSprite.currentAnimationIndex <= 1 ? 0.0 : (double) Math.Abs(FarmerRenderer.featureYOffsetPerFrame[this.FarmerSprite.CurrentFrame]) * 0.5);
    double layerDepth = (double) num;
    spriteBatch.Draw(shadowTexture, local, sourceRectangle, white, 0.0f, origin, (float) scale, SpriteEffects.None, (float) layerDepth);
  }

  private void performDrinkAnimation(Object item)
  {
    if (this.isEmoteAnimating)
      this.EndEmoteAnimation();
    if (!this.IsLocalPlayer)
      this.itemToEat = (Item) item;
    this.FarmerSprite.animateOnce(294, 80f, 8);
    this.isEating = true;
    if (item == null || !item.HasContextTag("mayo_item") || !(Utility.isThereAFarmerOrCharacterWithinDistance(this.Tile, 7, this.currentLocation) is NPC npc) || npc.Age == 2)
      return;
    int num = Game1.random.Next(3);
    if (npc.Manners == 2 || npc.SocialAnxiety == 1)
      num = 3;
    if (npc.Name == "Emily" || npc.Name == "Sandy" || npc.Name == "Linus" || npc.Name == "Krobus" && item.QualifiedItemId == "(O)308")
    {
      num = 4;
    }
    else
    {
      if (npc.Name == "Krobus" || npc.Name == "Dwarf")
        return;
      switch (npc)
      {
        case Monster _:
          return;
        case Horse _:
          return;
        case Pet _:
          return;
        case Child _:
          return;
      }
    }
    npc.showTextAboveHead(Game1.content.LoadString("Strings\\1_6_Strings:Mayo_reaction" + num.ToString()), preTimer: 500);
    npc.faceTowardFarmerForPeriod(1500, 7, false, this);
  }

  public Farmer CreateFakeEventFarmer()
  {
    Farmer fakeEventFarmer = new Farmer(new FarmerSprite(this.FarmerSprite.textureName.Value), new Vector2(192f, 192f), 1, "", new List<Item>(), this.IsMale);
    fakeEventFarmer.Name = this.Name;
    fakeEventFarmer.displayName = this.displayName;
    fakeEventFarmer.isFakeEventActor = true;
    fakeEventFarmer.changeGender(this.IsMale);
    fakeEventFarmer.changeHairStyle(this.hair.Value);
    fakeEventFarmer.UniqueMultiplayerID = this.UniqueMultiplayerID;
    fakeEventFarmer.shirtItem.Set(this.shirtItem.Value);
    fakeEventFarmer.pantsItem.Set(this.pantsItem.Value);
    fakeEventFarmer.shirt.Set(this.shirt.Value);
    fakeEventFarmer.pants.Set(this.pants.Value);
    foreach (Trinket trinketItem in this.trinketItems)
      fakeEventFarmer.trinketItems.Add((Trinket) trinketItem?.getOne());
    fakeEventFarmer.changeShoeColor(this.shoes.Value);
    fakeEventFarmer.boots.Set(this.boots.Value);
    fakeEventFarmer.leftRing.Set(this.leftRing.Value);
    fakeEventFarmer.rightRing.Set(this.rightRing.Value);
    fakeEventFarmer.hat.Set(this.hat.Value);
    fakeEventFarmer.pantsColor.Set(this.pantsColor.Value);
    fakeEventFarmer.changeHairColor(this.hairstyleColor.Value);
    fakeEventFarmer.changeSkinColor(this.skin.Value);
    fakeEventFarmer.accessory.Set(this.accessory.Value);
    fakeEventFarmer.changeEyeColor(this.newEyeColor.Value);
    fakeEventFarmer.UpdateClothing();
    return fakeEventFarmer;
  }

  private void performEatAnimation(Object item)
  {
    if (this.isEmoteAnimating)
      this.EndEmoteAnimation();
    if (!this.IsLocalPlayer)
      this.itemToEat = (Item) item;
    this.FarmerSprite.animateOnce(216, 80f, 8);
    this.isEating = true;
  }

  public void netDoEmote(string emote_type) => this.doEmoteEvent.Fire(emote_type);

  private void performSickAnimation()
  {
    if (this.isEmoteAnimating)
      this.EndEmoteAnimation();
    this.isEating = false;
    this.FarmerSprite.animateOnce(224 /*0xE0*/, 350f, 4);
    this.doEmote(12);
  }

  public void eatHeldObject()
  {
    if (this.isEmoteAnimating)
      this.EndEmoteAnimation();
    if (Game1.fadeToBlack)
      return;
    Item obj = (Item) null;
    int num = 0;
    bool flag1 = false;
    bool flag2 = false;
    if (this.ActiveItem == null || this.ActiveItem != this.mostRecentlyGrabbedItem)
    {
      if (this.netItemStowed.Value)
      {
        flag2 = true;
        this.netItemStowed.Value = false;
        this.UpdateItemStow();
      }
      if (this.ActiveItem == null)
        this.ActiveItem = this.mostRecentlyGrabbedItem;
      else if (this.ActiveItem != this.mostRecentlyGrabbedItem)
      {
        num = this.currentToolIndex.Value;
        if (this.currentToolIndex.Value < 0 || this.currentToolIndex.Value >= this.Items.Count)
          this.currentToolIndex.Value = 0;
        obj = this.Items[this.currentToolIndex.Value];
        this.Items[this.currentToolIndex.Value] = this.mostRecentlyGrabbedItem;
        this.OnItemReceived(this.mostRecentlyGrabbedItem, this.mostRecentlyGrabbedItem.Stack, (Item) null);
        flag1 = true;
      }
    }
    this.eatObject(this.ActiveObject);
    if (this.isEating)
    {
      this.reduceActiveItemByOne();
      this.CanMove = false;
    }
    if (flag1)
    {
      this.Items[this.currentToolIndex.Value] = obj;
      this.currentToolIndex.Value = num;
    }
    if (!flag2)
      return;
    this.netItemStowed.Value = true;
    this.UpdateItemStow();
  }

  public void grabObject(Object obj)
  {
    if (this.isEmoteAnimating)
      this.EndEmoteAnimation();
    if (obj == null)
      return;
    this.CanMove = false;
    switch (this.FacingDirection)
    {
      case 0:
        ((FarmerSprite) this.Sprite).animateOnce(80 /*0x50*/, 50f, 8);
        break;
      case 1:
        ((FarmerSprite) this.Sprite).animateOnce(72, 50f, 8);
        break;
      case 2:
        ((FarmerSprite) this.Sprite).animateOnce(64 /*0x40*/, 50f, 8);
        break;
      case 3:
        ((FarmerSprite) this.Sprite).animateOnce(88, 50f, 8);
        break;
    }
    Game1.playSound("pickUpItem");
  }

  public virtual void PlayFishBiteChime()
  {
    int num = this.biteChime.Value;
    if (num < 0)
      num = Game1.game1.instanceIndex;
    if (num > 3)
      num = 3;
    if (num == 0)
      this.playNearbySoundLocal("fishBite");
    else
      this.playNearbySoundLocal("fishBite_alternate_" + (num - 1).ToString());
  }

  public string getTitle()
  {
    int level = this.Level;
    if (level >= 30)
      return Game1.content.LoadString("Strings\\StringsFromCSFiles:Farmer.cs.2016");
    switch (level - 3)
    {
      case 0:
      case 1:
        return Game1.content.LoadString("Strings\\StringsFromCSFiles:Farmer.cs.2031");
      case 2:
      case 3:
        return Game1.content.LoadString("Strings\\StringsFromCSFiles:Farmer.cs.2030");
      case 4:
      case 5:
        return Game1.content.LoadString("Strings\\StringsFromCSFiles:Farmer.cs.2029");
      case 6:
      case 7:
        return Game1.content.LoadString("Strings\\StringsFromCSFiles:Farmer.cs.2028");
      case 8:
      case 9:
        return Game1.content.LoadString("Strings\\StringsFromCSFiles:Farmer.cs.2027");
      case 10:
      case 11:
        return Game1.content.LoadString("Strings\\StringsFromCSFiles:Farmer.cs.2026");
      case 12:
      case 13:
        return Game1.content.LoadString("Strings\\StringsFromCSFiles:Farmer.cs.2025");
      case 14:
      case 15:
        return !this.IsMale ? Game1.content.LoadString("Strings\\StringsFromCSFiles:Farmer.cs.2024") : Game1.content.LoadString("Strings\\StringsFromCSFiles:Farmer.cs.2023");
      case 16 /*0x10*/:
      case 17:
        return Game1.content.LoadString("Strings\\StringsFromCSFiles:Farmer.cs.2022");
      case 18:
      case 19:
        return Game1.content.LoadString("Strings\\StringsFromCSFiles:Farmer.cs.2021");
      case 20:
      case 21:
        return Game1.content.LoadString("Strings\\StringsFromCSFiles:Farmer.cs.2020");
      case 22:
      case 23:
        return Game1.content.LoadString("Strings\\StringsFromCSFiles:Farmer.cs.2019");
      case 24:
      case 25:
        return Game1.content.LoadString("Strings\\StringsFromCSFiles:Farmer.cs.2018");
      case 26:
        return Game1.content.LoadString("Strings\\StringsFromCSFiles:Farmer.cs.2017");
      default:
        return Game1.content.LoadString("Strings\\StringsFromCSFiles:Farmer.cs.2032");
    }
  }

  public void queueMessage(byte messageType, Farmer sourceFarmer, params object[] data)
  {
    this.queueMessage(new OutgoingMessage(messageType, sourceFarmer, data));
  }

  public void queueMessage(OutgoingMessage message) => this.messageQueue.Add(message);

  static Farmer()
  {
    Farmer.EmoteType[] emoteTypeArray = new Farmer.EmoteType[22]
    {
      new Farmer.EmoteType("happy", "Emote_Happy", 32 /*0x20*/),
      new Farmer.EmoteType("sad", "Emote_Sad", 28),
      new Farmer.EmoteType("heart", "Emote_Heart", 20),
      new Farmer.EmoteType("exclamation", "Emote_Exclamation", 16 /*0x10*/),
      new Farmer.EmoteType("note", "Emote_Note", 56),
      new Farmer.EmoteType("sleep", "Emote_Sleep", 24),
      new Farmer.EmoteType("game", "Emote_Game", 52),
      new Farmer.EmoteType("question", "Emote_Question", 8),
      new Farmer.EmoteType("x", "Emote_X", 36),
      new Farmer.EmoteType("pause", "Emote_Pause", 40),
      new Farmer.EmoteType("blush", "Emote_Blush", 60, is_hidden: true),
      new Farmer.EmoteType("angry", "Emote_Angry", 12),
      new Farmer.EmoteType("yes", "Emote_Yes", 56, new FarmerSprite.AnimationFrame[7]
      {
        new FarmerSprite.AnimationFrame(0, 250, false, false).AddFrameAction((AnimatedSprite.endOfAnimationBehavior) (who =>
        {
          if (!who.ShouldHandleAnimationSound())
            return;
          who.playNearbySoundLocal("jingle1");
        })),
        new FarmerSprite.AnimationFrame(16 /*0x10*/, 150, false, false),
        new FarmerSprite.AnimationFrame(0, 250, false, false),
        new FarmerSprite.AnimationFrame(16 /*0x10*/, 150, false, false),
        new FarmerSprite.AnimationFrame(0, 250, false, false),
        new FarmerSprite.AnimationFrame(16 /*0x10*/, 150, false, false),
        new FarmerSprite.AnimationFrame(0, 250, false, false)
      }),
      new Farmer.EmoteType("no", "Emote_No", 36, new FarmerSprite.AnimationFrame[5]
      {
        new FarmerSprite.AnimationFrame(25, 250, false, false).AddFrameAction((AnimatedSprite.endOfAnimationBehavior) (who =>
        {
          if (!who.ShouldHandleAnimationSound())
            return;
          who.playNearbySoundLocal("cancel");
        })),
        new FarmerSprite.AnimationFrame(27, 250, true, false),
        new FarmerSprite.AnimationFrame(25, 250, false, false),
        new FarmerSprite.AnimationFrame(27, 250, true, false),
        new FarmerSprite.AnimationFrame(25, 250, false, false)
      }),
      new Farmer.EmoteType("sick", "Emote_Sick", 12, new FarmerSprite.AnimationFrame[8]
      {
        new FarmerSprite.AnimationFrame(104, 350, false, false).AddFrameAction((AnimatedSprite.endOfAnimationBehavior) (who =>
        {
          if (!who.ShouldHandleAnimationSound())
            return;
          who.playNearbySoundLocal("croak");
        })),
        new FarmerSprite.AnimationFrame(105, 350, false, false),
        new FarmerSprite.AnimationFrame(104, 350, false, false),
        new FarmerSprite.AnimationFrame(105, 350, false, false),
        new FarmerSprite.AnimationFrame(104, 350, false, false),
        new FarmerSprite.AnimationFrame(105, 350, false, false),
        new FarmerSprite.AnimationFrame(104, 350, false, false),
        new FarmerSprite.AnimationFrame(105, 350, false, false)
      }),
      new Farmer.EmoteType("laugh", "Emote_Laugh", 56, new FarmerSprite.AnimationFrame[8]
      {
        new FarmerSprite.AnimationFrame(102, 150, false, false).AddFrameAction((AnimatedSprite.endOfAnimationBehavior) (who =>
        {
          if (!who.ShouldHandleAnimationSound())
            return;
          who.playNearbySoundLocal("dustMeep");
        })),
        new FarmerSprite.AnimationFrame(103, 150, false, false),
        new FarmerSprite.AnimationFrame(102, 150, false, false).AddFrameAction((AnimatedSprite.endOfAnimationBehavior) (who =>
        {
          if (!who.ShouldHandleAnimationSound())
            return;
          who.playNearbySoundLocal("dustMeep");
        })),
        new FarmerSprite.AnimationFrame(103, 150, false, false),
        new FarmerSprite.AnimationFrame(102, 150, false, false).AddFrameAction((AnimatedSprite.endOfAnimationBehavior) (who =>
        {
          if (!who.ShouldHandleAnimationSound())
            return;
          who.playNearbySoundLocal("dustMeep");
        })),
        new FarmerSprite.AnimationFrame(103, 150, false, false),
        new FarmerSprite.AnimationFrame(102, 150, false, false).AddFrameAction((AnimatedSprite.endOfAnimationBehavior) (who =>
        {
          if (!who.ShouldHandleAnimationSound())
            return;
          who.playNearbySoundLocal("dustMeep");
        })),
        new FarmerSprite.AnimationFrame(103, 150, false, false)
      }),
      new Farmer.EmoteType("surprised", "Emote_Surprised", 16 /*0x10*/, new FarmerSprite.AnimationFrame[1]
      {
        new FarmerSprite.AnimationFrame(94, 1500, false, false).AddFrameAction((AnimatedSprite.endOfAnimationBehavior) (who =>
        {
          if (who.ShouldHandleAnimationSound())
            who.playNearbySoundLocal("batScreech");
          who.jumpWithoutSound(4f);
          who.jitterStrength = 1f;
        }))
      }),
      new Farmer.EmoteType("hi", "Emote_Hi", 56, new FarmerSprite.AnimationFrame[4]
      {
        new FarmerSprite.AnimationFrame(3, 250, false, false).AddFrameAction((AnimatedSprite.endOfAnimationBehavior) (who =>
        {
          if (!who.ShouldHandleAnimationSound())
            return;
          who.playNearbySoundLocal("give_gift");
        })),
        new FarmerSprite.AnimationFrame(85, 250, false, false),
        new FarmerSprite.AnimationFrame(3, 250, false, false),
        new FarmerSprite.AnimationFrame(85, 250, false, false)
      }),
      null,
      null,
      null,
      null
    };
    FarmerSprite.AnimationFrame[] frames1 = new FarmerSprite.AnimationFrame[10];
    frames1[0] = new FarmerSprite.AnimationFrame(3, 250, false, false);
    frames1[1] = new FarmerSprite.AnimationFrame(102, 50, false, false);
    FarmerSprite.AnimationFrame animationFrame1 = new FarmerSprite.AnimationFrame(10, 250, false, false);
    animationFrame1 = animationFrame1.AddFrameAction((AnimatedSprite.endOfAnimationBehavior) (who =>
    {
      if (who.ShouldHandleAnimationSound())
        who.playNearbySoundLocal("hitEnemy");
      who.jitterStrength = 1f;
    }));
    frames1[2] = animationFrame1.AddFrameEndAction((AnimatedSprite.endOfAnimationBehavior) (who => who.stopJittering()));
    frames1[3] = new FarmerSprite.AnimationFrame(3, 250, false, false);
    frames1[4] = new FarmerSprite.AnimationFrame(102, 50, false, false);
    FarmerSprite.AnimationFrame animationFrame2 = new FarmerSprite.AnimationFrame(10, 250, false, false);
    animationFrame2 = animationFrame2.AddFrameAction((AnimatedSprite.endOfAnimationBehavior) (who =>
    {
      if (who.ShouldHandleAnimationSound())
        who.playNearbySoundLocal("hitEnemy");
      who.jitterStrength = 1f;
    }));
    frames1[5] = animationFrame2.AddFrameEndAction((AnimatedSprite.endOfAnimationBehavior) (who => who.stopJittering()));
    frames1[6] = new FarmerSprite.AnimationFrame(3, 250, false, false);
    frames1[7] = new FarmerSprite.AnimationFrame(102, 50, false, false);
    FarmerSprite.AnimationFrame animationFrame3 = new FarmerSprite.AnimationFrame(10, 250, false, false);
    animationFrame3 = animationFrame3.AddFrameAction((AnimatedSprite.endOfAnimationBehavior) (who =>
    {
      if (who.ShouldHandleAnimationSound())
        who.playNearbySoundLocal("hitEnemy");
      who.jitterStrength = 1f;
    }));
    frames1[8] = animationFrame3.AddFrameEndAction((AnimatedSprite.endOfAnimationBehavior) (who => who.stopJittering()));
    frames1[9] = new FarmerSprite.AnimationFrame(3, 500, false, false);
    emoteTypeArray[18] = new Farmer.EmoteType("taunt", "Emote_Taunt", 12, frames1, is_hidden: true);
    emoteTypeArray[19] = new Farmer.EmoteType("uh", "Emote_Uh", 40, new FarmerSprite.AnimationFrame[1]
    {
      new FarmerSprite.AnimationFrame(10, 1500, false, false).AddFrameAction((AnimatedSprite.endOfAnimationBehavior) (who =>
      {
        if (!who.ShouldHandleAnimationSound())
          return;
        who.playNearbySoundLocal("clam_tone");
      }))
    });
    emoteTypeArray[20] = new Farmer.EmoteType("music", "Emote_Music", 56, new FarmerSprite.AnimationFrame[9]
    {
      new FarmerSprite.AnimationFrame(98, 150, false, false).AddFrameAction((AnimatedSprite.endOfAnimationBehavior) (who => who.playHarpEmoteSound())),
      new FarmerSprite.AnimationFrame(99, 150, false, false),
      new FarmerSprite.AnimationFrame(100, 150, false, false),
      new FarmerSprite.AnimationFrame(98, 150, false, false),
      new FarmerSprite.AnimationFrame(99, 150, false, false),
      new FarmerSprite.AnimationFrame(100, 150, false, false),
      new FarmerSprite.AnimationFrame(98, 150, false, false),
      new FarmerSprite.AnimationFrame(99, 150, false, false),
      new FarmerSprite.AnimationFrame(100, 150, false, false)
    }, is_hidden: true);
    FarmerSprite.AnimationFrame[] frames2 = new FarmerSprite.AnimationFrame[6];
    frames2[0] = new FarmerSprite.AnimationFrame(111, 150, false, false);
    FarmerSprite.AnimationFrame animationFrame4 = new FarmerSprite.AnimationFrame(111, 300, false, false);
    animationFrame4 = animationFrame4.AddFrameAction((AnimatedSprite.endOfAnimationBehavior) (who =>
    {
      if (who.ShouldHandleAnimationSound())
        who.playNearbySoundLocal("fishingRodBend");
      who.jitterStrength = 1f;
    }));
    frames2[1] = animationFrame4.AddFrameEndAction((AnimatedSprite.endOfAnimationBehavior) (who => who.stopJittering()));
    frames2[2] = new FarmerSprite.AnimationFrame(111, 500, false, false);
    FarmerSprite.AnimationFrame animationFrame5 = new FarmerSprite.AnimationFrame(111, 300, false, false);
    animationFrame5 = animationFrame5.AddFrameAction((AnimatedSprite.endOfAnimationBehavior) (who =>
    {
      if (who.ShouldHandleAnimationSound())
        who.playNearbySoundLocal("fishingRodBend");
      who.jitterStrength = 1f;
    }));
    frames2[3] = animationFrame5.AddFrameEndAction((AnimatedSprite.endOfAnimationBehavior) (who => who.stopJittering()));
    frames2[4] = new FarmerSprite.AnimationFrame(111, 500, false, false);
    frames2[5] = new FarmerSprite.AnimationFrame(112 /*0x70*/, 1000, false, false).AddFrameAction((AnimatedSprite.endOfAnimationBehavior) (who =>
    {
      if (who.ShouldHandleAnimationSound())
        who.playNearbySoundLocal("coin");
      who.jumpWithoutSound(4f);
    }));
    emoteTypeArray[21] = new Farmer.EmoteType("jar", "Emote_Jar", frames: frames2, facing_direction: 1, is_hidden: true);
    Farmer.EMOTES = emoteTypeArray;
  }

  public class EmoteType
  {
    public string emoteString = "";
    public int emoteIconIndex = -1;
    public FarmerSprite.AnimationFrame[] animationFrames;
    public bool hidden;
    public int facingDirection = 2;
    public string displayNameKey;

    public EmoteType(
      string emote_string = "",
      string display_name_key = "",
      int icon_index = -1,
      FarmerSprite.AnimationFrame[] frames = null,
      int facing_direction = 2,
      bool is_hidden = false)
    {
      this.emoteString = emote_string;
      this.emoteIconIndex = icon_index;
      this.animationFrames = frames;
      this.facingDirection = facing_direction;
      this.hidden = is_hidden;
      this.displayNameKey = "Strings\\UI:" + display_name_key;
    }

    public string displayName => Game1.content.LoadString(this.displayNameKey);
  }
}
