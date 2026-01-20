// Decompiled with JetBrains decompiler
// Type: StardewValley.NPC
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Netcode;
using StardewValley.Audio;
using StardewValley.BellsAndWhistles;
using StardewValley.Buildings;
using StardewValley.Characters;
using StardewValley.Extensions;
using StardewValley.GameData;
using StardewValley.GameData.Characters;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Locations;
using StardewValley.Monsters;
using StardewValley.Network;
using StardewValley.Objects;
using StardewValley.Pathfinding;
using StardewValley.Quests;
using StardewValley.SpecialOrders;
using StardewValley.TerrainFeatures;
using StardewValley.TokenizableStrings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

#nullable enable
namespace StardewValley;

[XmlInclude(typeof (Cat))]
[XmlInclude(typeof (Child))]
[XmlInclude(typeof (Dog))]
[XmlInclude(typeof (Horse))]
[XmlInclude(typeof (Junimo))]
[XmlInclude(typeof (JunimoHarvester))]
[XmlInclude(typeof (Pet))]
[XmlInclude(typeof (TrashBear))]
[XmlInclude(typeof (Raccoon))]
[XmlInclude(typeof (Monster))]
public class NPC : Character, IComparable
{
  public const int minimum_square_pause = 6000;
  public const int maximum_square_pause = 12000;
  public const int portrait_width = 64 /*0x40*/;
  public const int portrait_height = 64 /*0x40*/;
  public const int portrait_neutral_index = 0;
  public const int portrait_happy_index = 1;
  public const int portrait_sad_index = 2;
  public const int portrait_custom_index = 3;
  public const int portrait_blush_index = 4;
  public const int portrait_angry_index = 5;
  public const int startingFriendship = 0;
  public const int defaultSpeed = 2;
  public const int maxGiftsPerWeek = 2;
  public const int friendshipPointsPerHeartLevel = 250;
  public const int maxFriendshipPoints = 2500;
  public const int gift_taste_love = 0;
  public const int gift_taste_like = 2;
  public const int gift_taste_neutral = 8;
  public const int gift_taste_dislike = 4;
  public const int gift_taste_hate = 6;
  public const int gift_taste_stardroptea = 7;
  public const int textStyle_shake = 0;
  public const int textStyle_none = 2;
  public const int adult = 0;
  public const int teen = 1;
  public const int child = 2;
  public const int neutral = 0;
  public const int polite = 1;
  public const int rude = 2;
  public const int outgoing = 0;
  public const int shy = 1;
  public const int positive = 0;
  public const int negative = 1;
  public const 
  #nullable disable
  string region_desert = "Desert";
  public const string region_town = "Town";
  public const string region_other = "Other";
  /// <summary>The default pixel width for the <see cref="P:StardewValley.Character.Sprite" />.</summary>
  public const int defaultSpriteWidth = 16 /*0x10*/;
  /// <summary>The default pixel height for the <see cref="P:StardewValley.Character.Sprite" />.</summary>
  public const int defaultSpriteHeight = 32 /*0x20*/;
  private Dictionary<string, string> dialogue;
  private SchedulePathDescription directionsToNewLocation;
  private int lengthOfWalkingSquareX;
  private int lengthOfWalkingSquareY;
  private int squarePauseAccumulation;
  private int squarePauseTotal;
  private int squarePauseOffset;
  public Microsoft.Xna.Framework.Rectangle lastCrossroad;
  /// <summary>The loaded portrait asset.</summary>
  /// <remarks>This is normally set via <see cref="M:StardewValley.NPC.ChooseAppearance(StardewValley.LocalizedContentManager)" />.</remarks>
  private Texture2D portrait;
  /// <summary>The last location for which <see cref="M:StardewValley.NPC.ChooseAppearance(StardewValley.LocalizedContentManager)" /> was applied.</summary>
  private string LastLocationNameForAppearance;
  /// <summary>The appearance ID from <c>Data/Characters</c> chosen by the last <see cref="M:StardewValley.NPC.ChooseAppearance(StardewValley.LocalizedContentManager)" /> call, or <c>null</c> if the last call didn't apply an appearance entry. This may not match their current textures if they were manually overridden after calling <see cref="M:StardewValley.NPC.ChooseAppearance(StardewValley.LocalizedContentManager)" />.</summary>
  [XmlIgnore]
  public string LastAppearanceId;
  private Vector2 nextSquarePosition;
  [XmlIgnore]
  public int shakeTimer;
  private bool isWalkingInSquare;
  private readonly NetBool isWalkingTowardPlayer = new NetBool();
  protected string textAboveHead;
  protected int textAboveHeadPreTimer;
  protected int textAboveHeadTimer;
  protected int textAboveHeadStyle;
  protected Color? textAboveHeadColor;
  protected float textAboveHeadAlpha;
  public int daysAfterLastBirth = -1;
  protected StardewValley.Dialogue extraDialogueMessageToAddThisMorning;
  [XmlElement("birthday_Season")]
  public readonly NetString birthday_Season = new NetString();
  [XmlElement("birthday_Day")]
  public readonly NetInt birthday_Day = new NetInt();
  [XmlElement("age")]
  public readonly NetInt age = new NetInt();
  [XmlElement("manners")]
  public readonly NetInt manners = new NetInt();
  [XmlElement("socialAnxiety")]
  public readonly NetInt socialAnxiety = new NetInt();
  [XmlElement("optimism")]
  public readonly NetInt optimism = new NetInt();
  /// <summary>The net-synchronized backing field for <see cref="P:StardewValley.NPC.Gender" />.</summary>
  [XmlElement("gender")]
  public readonly NetEnum<Gender> gender = new NetEnum<Gender>();
  [XmlIgnore]
  public readonly NetBool breather = new NetBool(true);
  [XmlIgnore]
  public readonly NetBool isSleeping = new NetBool(false);
  [XmlElement("sleptInBed")]
  public readonly NetBool sleptInBed = new NetBool(true);
  [XmlIgnore]
  public readonly NetBool hideShadow = new NetBool();
  [XmlElement("isInvisible")]
  public readonly NetBool isInvisible = new NetBool(false);
  [XmlElement("lastSeenMovieWeek")]
  public readonly NetInt lastSeenMovieWeek = new NetInt(-1);
  /// <summary>Obsolete. This is only kept to preserve data from old save files. Use <see cref="F:StardewValley.Farmer.friendshipData" /> instead.</summary>
  public bool? datingFarmer;
  /// <summary>Obsolete. This is only kept to preserve data from old save files. Use <see cref="F:StardewValley.Farmer.friendshipData" /> instead.</summary>
  public bool? divorcedFromFarmer;
  [XmlElement("datable")]
  public readonly NetBool datable = new NetBool();
  [XmlIgnore]
  public bool updatedDialogueYet;
  [XmlIgnore]
  public bool immediateSpeak;
  [XmlIgnore]
  public bool ignoreScheduleToday;
  protected int defaultFacingDirection;
  private readonly NetVector2 defaultPosition = new NetVector2();
  [XmlElement("defaultMap")]
  public readonly NetString defaultMap = new NetString();
  public string loveInterest;
  public int id = -1;
  public int daysUntilNotInvisible;
  public bool followSchedule = true;
  [XmlIgnore]
  public PathFindController temporaryController;
  [XmlElement("moveTowardPlayerThreshold")]
  public readonly NetInt moveTowardPlayerThreshold = new NetInt();
  [XmlIgnore]
  public float rotation;
  [XmlIgnore]
  public float yOffset;
  [XmlIgnore]
  public float swimTimer;
  [XmlIgnore]
  public float timerSinceLastMovement;
  [XmlIgnore]
  public string mapBeforeEvent;
  [XmlIgnore]
  public Vector2 positionBeforeEvent;
  [XmlIgnore]
  public Vector2 lastPosition;
  [XmlIgnore]
  public float currentScheduleDelay;
  [XmlIgnore]
  public float scheduleDelaySeconds;
  [XmlIgnore]
  public bool layingDown;
  [XmlIgnore]
  public Vector2 appliedRouteAnimationOffset = Vector2.Zero;
  [XmlIgnore]
  public string[] routeAnimationMetadata;
  [XmlElement("hasSaidAfternoonDialogue")]
  private NetBool hasSaidAfternoonDialogue = new NetBool(false);
  [XmlIgnore]
  public static bool hasSomeoneWateredCrops;
  [XmlIgnore]
  public static bool hasSomeoneFedThePet;
  [XmlIgnore]
  public static bool hasSomeoneFedTheAnimals;
  [XmlIgnore]
  public static bool hasSomeoneRepairedTheFences = false;
  [XmlIgnore]
  protected bool _skipRouteEndIntro;
  [NonInstancedStatic]
  public static HashSet<string> invalidDialogueFiles = new HashSet<string>();
  [XmlIgnore]
  protected bool _hasLoadedMasterScheduleData;
  [XmlIgnore]
  protected Dictionary<string, string> _masterScheduleData;
  protected static Stack<StardewValley.Dialogue> _EmptyDialogue = new Stack<StardewValley.Dialogue>();
  /// <summary>If set to a non-null value, the dialogue to return for <see cref="P:StardewValley.NPC.CurrentDialogue" /> instead of reading <see cref="F:StardewValley.Game1.npcDialogues" />.</summary>
  [XmlIgnore]
  public Stack<StardewValley.Dialogue> TemporaryDialogue;
  [XmlIgnore]
  public readonly NetList<MarriageDialogueReference, NetRef<MarriageDialogueReference>> currentMarriageDialogue = new NetList<MarriageDialogueReference, NetRef<MarriageDialogueReference>>();
  public readonly NetBool hasBeenKissedToday = new NetBool(false);
  [XmlIgnore]
  public readonly NetRef<MarriageDialogueReference> marriageDefaultDialogue = new NetRef<MarriageDialogueReference>((MarriageDialogueReference) null);
  [XmlIgnore]
  public readonly NetBool shouldSayMarriageDialogue = new NetBool(false);
  public readonly NetEvent0 removeHenchmanEvent = new NetEvent0();
  private bool isPlayingSleepingAnimation;
  public readonly NetBool shouldPlayRobinHammerAnimation = new NetBool();
  private bool isPlayingRobinHammerAnimation;
  public readonly NetBool shouldPlaySpousePatioAnimation = new NetBool();
  private bool isPlayingSpousePatioAnimation;
  public readonly NetBool shouldWearIslandAttire = new NetBool();
  private bool isWearingIslandAttire;
  public readonly NetBool isMovingOnPathFindPath = new NetBool();
  /// <summary>Whether the NPC's portrait has been explicitly overridden (e.g. using the <c>changePortrait</c> event command) and shouldn't be changed automatically.</summary>
  [XmlIgnore]
  public bool portraitOverridden;
  /// <summary>Whether the NPC's sprite has been explicitly overridden (e.g. using the <c>changeSprite</c> event command) and shouldn't be changed automatically.</summary>
  [XmlIgnore]
  public bool spriteOverridden;
  [XmlIgnore]
  public List<SchedulePathDescription> queuedSchedulePaths = new List<SchedulePathDescription>();
  [XmlIgnore]
  public int lastAttemptedSchedule = -1;
  [XmlIgnore]
  public readonly NetBool doingEndOfRouteAnimation = new NetBool();
  private bool currentlyDoingEndOfRouteAnimation;
  [XmlIgnore]
  public readonly NetBool goingToDoEndOfRouteAnimation = new NetBool();
  [XmlIgnore]
  public readonly NetString endOfRouteMessage = new NetString();
  /// <summary>The backing field for <see cref="P:StardewValley.NPC.ScheduleKey" />. Most code should use that property instead.</summary>
  [XmlElement("dayScheduleName")]
  public readonly NetString dayScheduleName = new NetString();
  [XmlElement("islandScheduleName")]
  public readonly NetString islandScheduleName = new NetString();
  private int[] routeEndIntro;
  private int[] routeEndAnimation;
  private int[] routeEndOutro;
  [XmlIgnore]
  public string nextEndOfRouteMessage;
  private string loadedEndOfRouteBehavior;
  [XmlIgnore]
  protected string _startedEndOfRouteBehavior;
  [XmlIgnore]
  protected string _finishingEndOfRouteBehavior;
  [XmlIgnore]
  protected int _beforeEndOfRouteAnimationFrame;
  public readonly NetString endOfRouteBehaviorName = new NetString();
  public Point previousEndPoint;
  public int squareMovementFacingPreference;
  protected bool returningToEndPoint;
  private bool wasKissedYesterday;

  [XmlIgnore]
  public SchedulePathDescription DirectionsToNewLocation
  {
    get => this.directionsToNewLocation;
    set => this.directionsToNewLocation = value;
  }

  public int DefaultFacingDirection
  {
    get => this.defaultFacingDirection;
    set => this.defaultFacingDirection = value;
  }

  /// <summary>The main dialogue data for this NPC, if available.</summary>
  [XmlIgnore]
  public Dictionary<string, string> Dialogue
  {
    get
    {
      switch (this)
      {
        case Monster _:
        case Pet _:
        case Horse _:
        case Child _:
          this.LoadedDialogueKey = (string) null;
          return (Dictionary<string, string>) null;
        default:
          if (this.dialogue == null)
          {
            string assetName = "Characters\\Dialogue\\" + this.GetDialogueSheetName();
            if (NPC.invalidDialogueFiles.Contains(assetName))
            {
              this.LoadedDialogueKey = (string) null;
              this.dialogue = new Dictionary<string, string>();
            }
            try
            {
              this.dialogue = Game1.content.Load<Dictionary<string, string>>(assetName).Select<KeyValuePair<string, string>, KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, KeyValuePair<string, string>>) (pair => new KeyValuePair<string, string>(pair.Key, StardewValley.Dialogue.applyGenderSwitch(Game1.player.Gender, pair.Value, true)))).ToDictionary<KeyValuePair<string, string>, string, string>((Func<KeyValuePair<string, string>, string>) (p => p.Key), (Func<KeyValuePair<string, string>, string>) (p => p.Value));
              this.LoadedDialogueKey = assetName;
            }
            catch (ContentLoadException ex)
            {
              NPC.invalidDialogueFiles.Add(assetName);
              this.dialogue = new Dictionary<string, string>();
              this.LoadedDialogueKey = (string) null;
            }
          }
          return this.dialogue;
      }
    }
  }

  /// <summary>The dialogue key that was loaded via <see cref="P:StardewValley.NPC.Dialogue" />, if any.</summary>
  [XmlIgnore]
  public string LoadedDialogueKey { get; private set; }

  [XmlIgnore]
  public string DefaultMap
  {
    get => this.defaultMap.Value;
    set => this.defaultMap.Value = value;
  }

  public Vector2 DefaultPosition
  {
    get => this.defaultPosition.Value;
    set => this.defaultPosition.Value = value;
  }

  [XmlIgnore]
  public Texture2D Portrait
  {
    get
    {
      if (this.portrait == null && this.IsVillager)
        this.ChooseAppearance();
      return this.portrait;
    }
    set => this.portrait = value;
  }

  /// <summary>Whether this NPC can dynamically change appearance based on their data in <c>Data/Characters</c>. This can be disabled for temporary NPCs and event actors.</summary>
  [XmlIgnore]
  public bool AllowDynamicAppearance { get; set; } = true;

  /// <inheritdoc />
  [XmlIgnore]
  public override bool IsVillager => true;

  /// <summary>The schedule of this NPC's movements and actions today, if loaded. The key is the time of departure, and the value is a list of directions to reach the new position.</summary>
  /// <remarks>You can set the schedule using <see cref="M:StardewValley.NPC.TryLoadSchedule" /> or one of its overloads.</remarks>
  [XmlIgnore]
  public Dictionary<int, SchedulePathDescription> Schedule { get; private set; }

  /// <summary>The <see cref="P:StardewValley.NPC.Schedule" />'s key in the original data asset, if loaded.</summary>
  [XmlIgnore]
  public string ScheduleKey => this.dayScheduleName.Value;

  public bool IsWalkingInSquare
  {
    get => this.isWalkingInSquare;
    set => this.isWalkingInSquare = value;
  }

  public bool IsWalkingTowardPlayer
  {
    get => this.isWalkingTowardPlayer.Value;
    set => this.isWalkingTowardPlayer.Value = value;
  }

  [XmlIgnore]
  public virtual Stack<StardewValley.Dialogue> CurrentDialogue
  {
    get
    {
      if (this.TemporaryDialogue != null)
        return this.TemporaryDialogue;
      if (Game1.npcDialogues == null)
        Game1.npcDialogues = new Dictionary<string, Stack<StardewValley.Dialogue>>();
      if (!this.IsVillager)
        return NPC._EmptyDialogue;
      Stack<StardewValley.Dialogue> currentDialogue;
      Game1.npcDialogues.TryGetValue(this.Name, out currentDialogue);
      if (currentDialogue == null)
        currentDialogue = Game1.npcDialogues[this.Name] = this.loadCurrentDialogue();
      return currentDialogue;
    }
    set
    {
      if (this.TemporaryDialogue != null)
      {
        this.TemporaryDialogue = value;
      }
      else
      {
        if (Game1.npcDialogues == null)
          return;
        Game1.npcDialogues[this.Name] = value;
      }
    }
  }

  [XmlIgnore]
  public string Birthday_Season
  {
    get => this.birthday_Season.Value;
    set => this.birthday_Season.Value = value;
  }

  [XmlIgnore]
  public int Birthday_Day
  {
    get => this.birthday_Day.Value;
    set => this.birthday_Day.Value = value;
  }

  [XmlIgnore]
  public int Age
  {
    get => this.age.Value;
    set => this.age.Value = value;
  }

  [XmlIgnore]
  public int Manners
  {
    get => this.manners.Value;
    set => this.manners.Value = value;
  }

  [XmlIgnore]
  public int SocialAnxiety
  {
    get => this.socialAnxiety.Value;
    set => this.socialAnxiety.Value = value;
  }

  [XmlIgnore]
  public int Optimism
  {
    get => this.optimism.Value;
    set => this.optimism.Value = value;
  }

  /// <summary>The character's gender identity.</summary>
  [XmlIgnore]
  public override Gender Gender
  {
    get => this.gender.Value;
    set => this.gender.Value = value;
  }

  [XmlIgnore]
  public bool Breather
  {
    get => this.breather.Value;
    set => this.breather.Value = value;
  }

  [XmlIgnore]
  public bool HideShadow
  {
    get => this.hideShadow.Value;
    set => this.hideShadow.Value = value;
  }

  [XmlIgnore]
  public bool HasPartnerForDance
  {
    get
    {
      foreach (Farmer onlineFarmer in Game1.getOnlineFarmers())
      {
        if (onlineFarmer.dancePartner.TryGetVillager() == this)
          return true;
      }
      return false;
    }
  }

  [XmlIgnore]
  public bool IsInvisible
  {
    get => this.isInvisible.Value;
    set => this.isInvisible.Value = value;
  }

  /// <summary>Whether social features (like birthdays, gift giving, friendship, and an entry in the social tab) are enabled for this NPC.</summary>
  public virtual bool CanSocialize
  {
    get => this.IsVillager && NPC.CanSocializePerData(this.Name, this.currentLocation);
  }

  public NPC()
  {
  }

  public NPC(
    AnimatedSprite sprite,
    Vector2 position,
    int facingDir,
    string name,
    LocalizedContentManager content = null)
    : base(sprite, position, 2, name)
  {
    this.faceDirection(facingDir);
    this.defaultPosition.Value = position;
    this.defaultFacingDirection = facingDir;
    this.lastCrossroad = new Microsoft.Xna.Framework.Rectangle((int) position.X, (int) position.Y + 64 /*0x40*/, 64 /*0x40*/, 64 /*0x40*/);
    if (content == null)
      return;
    try
    {
      this.portrait = content.Load<Texture2D>("Portraits\\" + name);
    }
    catch (Exception ex)
    {
    }
  }

  public NPC(
    AnimatedSprite sprite,
    Vector2 position,
    string defaultMap,
    int facingDirection,
    string name,
    bool datable,
    Texture2D portrait)
    : this(sprite, position, defaultMap, facingDirection, name, portrait, false)
  {
    this.datable.Value = datable;
  }

  public NPC(
    AnimatedSprite sprite,
    Vector2 position,
    string defaultMap,
    int facingDir,
    string name,
    Texture2D portrait,
    bool eventActor)
    : base(sprite, position, 2, name)
  {
    this.portrait = portrait;
    this.faceDirection(facingDir);
    if (!eventActor)
      this.lastCrossroad = new Microsoft.Xna.Framework.Rectangle((int) position.X, (int) position.Y + 64 /*0x40*/, 64 /*0x40*/, 64 /*0x40*/);
    this.reloadData();
    this.defaultPosition.Value = position;
    this.defaultMap.Value = defaultMap;
    this.currentLocation = Game1.getLocationFromName(defaultMap);
    this.defaultFacingDirection = facingDir;
  }

  public virtual void reloadData()
  {
    if (this is Child)
      return;
    CharacterData data = this.GetData();
    if (data == null)
      return;
    this.Age = (int) Utility.GetEnumOrDefault<NpcAge>(data.Age, NpcAge.Adult);
    this.Manners = (int) Utility.GetEnumOrDefault<NpcManner>(data.Manner, NpcManner.Neutral);
    this.SocialAnxiety = (int) Utility.GetEnumOrDefault<NpcSocialAnxiety>(data.SocialAnxiety, NpcSocialAnxiety.Outgoing);
    this.Optimism = (int) Utility.GetEnumOrDefault<NpcOptimism>(data.Optimism, NpcOptimism.Positive);
    this.Gender = Utility.GetEnumOrDefault<Gender>(data.Gender, Gender.Male);
    this.datable.Value = data.CanBeRomanced;
    this.loveInterest = data.LoveInterest;
    this.Birthday_Season = data.BirthSeason.HasValue ? Utility.getSeasonKey(data.BirthSeason.Value) : (string) null;
    this.Birthday_Day = data.BirthDay;
    this.id = data.FestivalVanillaActorIndex > -1 ? data.FestivalVanillaActorIndex : Game1.hash.GetDeterministicHashCode(this.name.Value);
    this.breather.Value = data.Breather;
    if (!this.isMarried())
      this.reloadDefaultLocation();
    this.displayName = this.translateName();
  }

  public virtual void reloadDefaultLocation()
  {
    CharacterData data = this.GetData();
    string locationName;
    Point tile;
    int direction;
    if (data == null || !NPC.ReadNpcHomeData(data, this.currentLocation, out locationName, out tile, out direction))
      return;
    this.DefaultMap = locationName;
    this.DefaultPosition = new Vector2((float) (tile.X * 64 /*0x40*/), (float) (tile.Y * 64 /*0x40*/));
    this.DefaultFacingDirection = direction;
  }

  /// <summary>Get an NPC's home location from its data, or fallback values if it doesn't exist.</summary>
  /// <param name="data">The character data for the NPC.</param>
  /// <param name="currentLocation">The NPC's current location, if applicable.</param>
  /// <param name="locationName">The internal name of the NPC's default map.</param>
  /// <param name="tile">The NPC's default tile position within the <paramref name="locationName" />.</param>
  /// <param name="direction">The default facing direction.</param>
  /// <returns>Returns whether a valid home was found in the given character data.</returns>
  public static bool ReadNpcHomeData(
    CharacterData data,
    GameLocation currentLocation,
    out string locationName,
    out Point tile,
    out int direction)
  {
    if (data?.Home != null)
    {
      foreach (CharacterHomeData characterHomeData in data.Home)
      {
        if (characterHomeData.Condition == null || GameStateQuery.CheckConditions(characterHomeData.Condition, currentLocation))
        {
          locationName = characterHomeData.Location;
          tile = characterHomeData.Tile;
          int parsed;
          direction = Utility.TryParseDirection(characterHomeData.Direction, out parsed) ? parsed : 0;
          return true;
        }
      }
    }
    locationName = "Town";
    tile = new Point(29, 67);
    direction = 2;
    return false;
  }

  public virtual bool canTalk() => true;

  protected override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.birthday_Season, "birthday_Season").AddField((INetSerializable) this.birthday_Day, "birthday_Day").AddField((INetSerializable) this.datable, "datable").AddField((INetSerializable) this.shouldPlayRobinHammerAnimation, "shouldPlayRobinHammerAnimation").AddField((INetSerializable) this.shouldPlaySpousePatioAnimation, "shouldPlaySpousePatioAnimation").AddField((INetSerializable) this.isWalkingTowardPlayer, "isWalkingTowardPlayer").AddField((INetSerializable) this.moveTowardPlayerThreshold, "moveTowardPlayerThreshold").AddField((INetSerializable) this.age, "age").AddField((INetSerializable) this.manners, "manners").AddField((INetSerializable) this.socialAnxiety, "socialAnxiety").AddField((INetSerializable) this.optimism, "optimism").AddField((INetSerializable) this.gender, "gender").AddField((INetSerializable) this.breather, "breather").AddField((INetSerializable) this.isSleeping, "isSleeping").AddField((INetSerializable) this.hideShadow, "hideShadow").AddField((INetSerializable) this.isInvisible, "isInvisible").AddField((INetSerializable) this.defaultMap, "defaultMap").AddField((INetSerializable) this.defaultPosition, "defaultPosition").AddField((INetSerializable) this.removeHenchmanEvent, "removeHenchmanEvent").AddField((INetSerializable) this.doingEndOfRouteAnimation, "doingEndOfRouteAnimation").AddField((INetSerializable) this.goingToDoEndOfRouteAnimation, "goingToDoEndOfRouteAnimation").AddField((INetSerializable) this.endOfRouteMessage, "endOfRouteMessage").AddField((INetSerializable) this.endOfRouteBehaviorName, "endOfRouteBehaviorName").AddField((INetSerializable) this.lastSeenMovieWeek, "lastSeenMovieWeek").AddField((INetSerializable) this.currentMarriageDialogue, "currentMarriageDialogue").AddField((INetSerializable) this.marriageDefaultDialogue, "marriageDefaultDialogue").AddField((INetSerializable) this.shouldSayMarriageDialogue, "shouldSayMarriageDialogue").AddField((INetSerializable) this.hasBeenKissedToday, "hasBeenKissedToday").AddField((INetSerializable) this.hasSaidAfternoonDialogue, "hasSaidAfternoonDialogue").AddField((INetSerializable) this.dayScheduleName, "dayScheduleName").AddField((INetSerializable) this.islandScheduleName, "islandScheduleName").AddField((INetSerializable) this.sleptInBed, "sleptInBed").AddField((INetSerializable) this.shouldWearIslandAttire, "shouldWearIslandAttire").AddField((INetSerializable) this.isMovingOnPathFindPath, "isMovingOnPathFindPath");
    this.position.Field.AxisAlignedMovement = true;
    this.removeHenchmanEvent.onEvent += new NetEvent0.Event(this.performRemoveHenchman);
  }

  /// <summary>Reload the NPC's sprite or portrait based on their character data within the current context.</summary>
  /// <param name="content">The content manager from which to load assets, or <c>null</c> for the default content manager.</param>
  public virtual void ChooseAppearance(LocalizedContentManager content = null)
  {
    this.LastAppearanceId = (string) null;
    if (this.SimpleNonVillagerNPC)
      return;
    content = content ?? Game1.content;
    GameLocation currentLocation = this.currentLocation;
    if (currentLocation == null)
      return;
    this.LastLocationNameForAppearance = currentLocation.NameOrUniqueName;
    bool flag1 = false;
    string propertyValue1;
    if (currentLocation.TryGetMapProperty("UniquePortrait", out propertyValue1) && ((IEnumerable<string>) ArgUtility.SplitBySpace(propertyValue1)).Contains<string>(this.Name))
    {
      string assetName = $"Portraits\\{this.getTextureName()}_{currentLocation.Name}";
      string error;
      flag1 = this.TryLoadPortraits(assetName, out error, content);
      if (!flag1)
        Game1.log.Warn($"NPC {this.Name} can't load portraits from '{assetName}' (per the {"UniquePortrait"} map property in '{currentLocation.NameOrUniqueName}'): {error}. Falling back to default portraits.");
    }
    bool flag2 = false;
    string propertyValue2;
    if (currentLocation.TryGetMapProperty("UniqueSprite", out propertyValue2) && ((IEnumerable<string>) ArgUtility.SplitBySpace(propertyValue2)).Contains<string>(this.Name))
    {
      string assetName = $"Characters\\{this.getTextureName()}_{currentLocation.Name}";
      string error;
      flag2 = this.TryLoadSprites(assetName, out error, content);
      if (!flag2)
        Game1.log.Warn($"NPC {this.Name} can't load sprites from '{assetName}' (per the {"UniqueSprite"} map property in '{currentLocation.NameOrUniqueName}'): {error}. Falling back to default sprites.");
    }
    if (flag1 & flag2)
      return;
    CharacterData characterData = (CharacterData) null;
    CharacterAppearanceData characterAppearanceData1 = (CharacterAppearanceData) null;
    if (!this.IsMonster)
    {
      characterData = this.GetData();
      if (characterData != null)
      {
        int? count = characterData.Appearance?.Count;
        int num1 = 0;
        if (count.GetValueOrDefault() > num1 & count.HasValue)
        {
          List<CharacterAppearanceData> characterAppearanceDataList = new List<CharacterAppearanceData>();
          int num2 = 0;
          Random daySaveRandom = Utility.CreateDaySaveRandom((double) Game1.hash.GetDeterministicHashCode(this.Name));
          Season season = currentLocation.GetSeason();
          bool isOutdoors = currentLocation.IsOutdoors;
          int num3 = int.MaxValue;
          foreach (CharacterAppearanceData characterAppearanceData2 in characterData.Appearance)
          {
            if (characterAppearanceData2.Precedence <= num3 && (characterAppearanceData2.IsIslandAttire != this.isWearingIslandAttire || characterAppearanceData2.Season.HasValue && characterAppearanceData2.Season.Value != season || (isOutdoors ? (characterAppearanceData2.Outdoors ? 1 : 0) : (characterAppearanceData2.Indoors ? 1 : 0)) == 0 ? 0 : (GameStateQuery.CheckConditions(characterAppearanceData2.Condition, currentLocation, random: daySaveRandom) ? 1 : 0)) != 0)
            {
              if (characterAppearanceData2.Precedence < num3)
              {
                num3 = characterAppearanceData2.Precedence;
                characterAppearanceDataList.Clear();
                num2 = 0;
              }
              characterAppearanceDataList.Add(characterAppearanceData2);
              num2 += characterAppearanceData2.Weight;
            }
          }
          switch (characterAppearanceDataList.Count)
          {
            case 0:
              break;
            case 1:
              characterAppearanceData1 = characterAppearanceDataList[0];
              break;
            default:
              characterAppearanceData1 = characterAppearanceDataList[characterAppearanceDataList.Count - 1];
              int num4 = Utility.CreateDaySaveRandom((double) Game1.hash.GetDeterministicHashCode(this.Name)).Next(num2 + 1);
              using (List<CharacterAppearanceData>.Enumerator enumerator = characterAppearanceDataList.GetEnumerator())
              {
                while (enumerator.MoveNext())
                {
                  CharacterAppearanceData current = enumerator.Current;
                  num4 -= current.Weight;
                  if (num4 <= 0)
                  {
                    characterAppearanceData1 = current;
                    break;
                  }
                }
                break;
              }
          }
        }
      }
    }
    if (!flag1)
    {
      string assetName1 = "Portraits/" + this.getTextureName();
      bool flag3 = false;
      if (characterAppearanceData1 != null && characterAppearanceData1.Portrait != null && characterAppearanceData1.Portrait != assetName1)
      {
        string error;
        flag3 = this.TryLoadPortraits(characterAppearanceData1.Portrait, out error, content);
        if (!flag3)
          Game1.log.Warn($"NPC {this.Name} can't load portraits from '{characterAppearanceData1.Portrait}' (per appearance entry '{characterAppearanceData1.Id}' in Data/Characters): {error}. Falling back to default portraits.");
      }
      if (!flag3 && this.isWearingIslandAttire)
      {
        string assetName2 = assetName1 + "_Beach";
        if (content.DoesAssetExist<Texture2D>(assetName2))
        {
          string error;
          flag3 = this.TryLoadPortraits(assetName2, out error, content);
          if (!flag3)
            Game1.log.Warn($"NPC {this.Name} can't load portraits from '{assetName2}' for island attire: {error}. Falling back to default portraits.");
        }
      }
      string error1;
      if (!flag3 && !this.TryLoadPortraits(assetName1, out error1, content))
        Game1.log.Warn($"NPC {this.Name} can't load portraits from '{assetName1}': {error1}.");
      if (flag3)
        this.LastAppearanceId = characterAppearanceData1?.Id;
    }
    if (!flag2)
    {
      string assetName3 = "Characters/" + this.getTextureName();
      bool flag4 = false;
      if (characterAppearanceData1 != null && characterAppearanceData1.Sprite != null && characterAppearanceData1.Sprite != assetName3)
      {
        string error;
        flag4 = this.TryLoadSprites(characterAppearanceData1.Sprite, out error, content);
        if (!flag4)
          Game1.log.Warn($"NPC {this.Name} can't load sprites from '{characterAppearanceData1.Sprite}' (per appearance entry '{characterAppearanceData1.Id}' in Data/Characters): {error}. Falling back to default sprites.");
      }
      if (!flag4 && this.isWearingIslandAttire)
      {
        string assetName4 = assetName3 + "_Beach";
        if (content.DoesAssetExist<Texture2D>(assetName4))
        {
          string error;
          flag4 = this.TryLoadSprites(assetName4, out error, content);
          if (!flag4)
            Game1.log.Warn($"NPC {this.Name} can't load sprites from '{assetName4}' for island attire: {error}. Falling back to default sprites.");
        }
      }
      string error2;
      if (!flag4 && !this.TryLoadSprites(assetName3, out error2, content))
        Game1.log.Warn($"NPC {this.Name} can't load sprites from '{assetName3}': {error2}.");
      if (flag4)
        this.LastAppearanceId = characterAppearanceData1?.Id;
    }
    if (characterData == null || this.Sprite == null)
      return;
    this.Sprite.SpriteWidth = characterData.Size.X;
    this.Sprite.SpriteHeight = characterData.Size.Y;
    this.Sprite.ignoreSourceRectUpdates = false;
  }

  protected override string translateName() => NPC.GetDisplayName(this.name.Value);

  public string getName()
  {
    return this.displayName != null && this.displayName.Length > 0 ? this.displayName : this.Name;
  }

  public virtual string getTextureName() => NPC.getTextureNameForCharacter(this.Name);

  public static string getTextureNameForCharacter(string character_name)
  {
    CharacterData data;
    NPC.TryGetData(character_name, out data);
    string textureName = data?.TextureName;
    return string.IsNullOrEmpty(textureName) ? character_name : textureName;
  }

  public void resetSeasonalDialogue() => this.dialogue = (Dictionary<string, string>) null;

  public void performSpecialScheduleChanges()
  {
    if (this.Schedule == null || !this.Name.Equals("Pam") || !Game1.MasterPlayer.mailReceived.Contains("ccVault"))
      return;
    bool flag1 = false;
    foreach (KeyValuePair<int, SchedulePathDescription> keyValuePair in this.Schedule)
    {
      bool flag2 = false;
      switch (keyValuePair.Value.targetLocationName)
      {
        case "BusStop":
          flag1 = true;
          break;
        case "DesertFestival":
        case "Desert":
        case "IslandSouth":
          BusStop busStop = Game1.RequireLocation<BusStop>("BusStop");
          Game1.netWorldState.Value.canDriveYourselfToday.Value = true;
          Object @object = ItemRegistry.Create<Object>("(BC)TextSign");
          @object.signText.Value = TokenStringBuilder.LocalizedText(keyValuePair.Value.targetLocationName == "IslandSouth" ? "Strings\\1_6_Strings:Pam_busSign_resort" : "Strings\\1_6_Strings:Pam_busSign");
          @object.SpecialVariable = 987659;
          Vector2 tile = new Vector2(25f, 10f);
          Object o = @object;
          busStop.tryPlaceObject(tile, o);
          flag1 = true;
          flag2 = true;
          break;
      }
      if (flag2)
        break;
    }
    if (flag1 || Game1.isGreenRain)
      return;
    BusStop locationFromName = Game1.getLocationFromName("BusStop") as BusStop;
    Game1.netWorldState.Value.canDriveYourselfToday.Value = true;
    Object object1 = (Object) ItemRegistry.Create("(BC)TextSign");
    object1.signText.Value = TokenStringBuilder.LocalizedText("Strings\\1_6_Strings:Pam_busSign_generic");
    object1.SpecialVariable = 987659;
    Vector2 tile1 = new Vector2(25f, 10f);
    Object o1 = object1;
    locationFromName.tryPlaceObject(tile1, o1);
  }

  /// <summary>Update the NPC state (including sprite, dialogue, facing direction, schedules, etc). Despite the name, this doesn't only affect the sprite.</summary>
  /// <param name="onlyAppearance">Only reload the NPC's appearance (e.g. sprite, portraits, or breather/shadow fields), don't change any other data.</param>
  public virtual void reloadSprite(bool onlyAppearance = false)
  {
    if (this.SimpleNonVillagerNPC)
      return;
    this.ChooseAppearance();
    if (onlyAppearance || !Game1.newDay && Game1.gameMode != (byte) 6)
      return;
    this.faceDirection(this.DefaultFacingDirection);
    this.previousEndPoint = new Point((int) this.defaultPosition.X / 64 /*0x40*/, (int) this.defaultPosition.Y / 64 /*0x40*/);
    this.TryLoadSchedule();
    this.performSpecialScheduleChanges();
    this.resetSeasonalDialogue();
    this.resetCurrentDialogue();
    this.updateConstructionAnimation();
    try
    {
      this.displayName = this.translateName();
    }
    catch (Exception ex)
    {
    }
  }

  /// <summary>Try to load a portraits texture, or keep the current texture if the load fails.</summary>
  /// <param name="assetName">The asset name to load.</param>
  /// <param name="error">If loading the portrait failed, an error phrase indicating why it failed.</param>
  /// <param name="content">The content manager from which to load the asset, or <c>null</c> for the default content manager.</param>
  /// <returns>Returns whether the texture was successfully loaded.</returns>
  public bool TryLoadPortraits(string assetName, out string error, LocalizedContentManager content = null)
  {
    if (this.Name == "Raccoon" || this.Name == "MrsRaccoon")
    {
      error = (string) null;
      return true;
    }
    if (this.portraitOverridden)
    {
      error = (string) null;
      return true;
    }
    if (string.IsNullOrWhiteSpace(assetName))
    {
      error = "the asset name is empty";
      return false;
    }
    if (this.portrait?.Name == assetName && !this.portrait.IsDisposed)
    {
      error = (string) null;
      return true;
    }
    if (content == null)
      content = Game1.content;
    try
    {
      this.portrait = content.Load<Texture2D>(assetName);
      this.portrait.Name = assetName;
      error = (string) null;
      return true;
    }
    catch (Exception ex)
    {
      error = ex.ToString();
      return false;
    }
  }

  /// <summary>Try to load a sprite texture, or keep the current texture if the load fails.</summary>
  /// <param name="assetName">The asset name to load.</param>
  /// <param name="error">If loading the portrait failed, an error phrase indicating why it failed.</param>
  /// <param name="content">The content manager from which to load the asset, or <c>null</c> for the default content manager.</param>
  /// <param name="logOnFail">Whether to log a warning if the texture can't be loaded.</param>
  /// <returns>Returns whether the texture was successfully loaded.</returns>
  public bool TryLoadSprites(string assetName, out string error, LocalizedContentManager content = null)
  {
    if (this.spriteOverridden)
    {
      error = (string) null;
      return true;
    }
    if (string.IsNullOrWhiteSpace(assetName))
    {
      error = "the asset name is empty";
      return false;
    }
    if (this.Sprite?.spriteTexture != null && ((this.Sprite.overrideTextureName ?? this.Sprite.textureName.Value) == assetName || this.Sprite.spriteTexture.Name == assetName) && !this.Sprite.spriteTexture.IsDisposed)
    {
      error = (string) null;
      return true;
    }
    if (content == null)
      content = Game1.content;
    try
    {
      if (this.Sprite == null)
        this.Sprite = new AnimatedSprite((ContentManager) content, assetName);
      else
        this.Sprite.LoadTexture(assetName, Game1.IsMasterGame);
      error = (string) null;
      return true;
    }
    catch (Exception ex)
    {
      error = ex.ToString();
      return false;
    }
  }

  private void updateConstructionAnimation()
  {
    bool flag = Utility.isFestivalDay();
    if (Game1.IsMasterGame && this.Name == "Robin" && !flag && (!Game1.isGreenRain || Game1.year > 1))
    {
      if (Game1.player.daysUntilHouseUpgrade.Value > 0)
      {
        Farm farm = Game1.getFarm();
        Game1.warpCharacter(this, farm.NameOrUniqueName, new Vector2((float) (farm.GetMainFarmHouseEntry().X + 4), (float) (farm.GetMainFarmHouseEntry().Y - 1)));
        this.isPlayingRobinHammerAnimation = false;
        this.shouldPlayRobinHammerAnimation.Value = true;
        return;
      }
      if (Game1.IsThereABuildingUnderConstruction())
      {
        Building underConstruction = Game1.GetBuildingUnderConstruction();
        if (underConstruction == null)
          return;
        GameLocation indoors = underConstruction.GetIndoors();
        if (underConstruction.daysUntilUpgrade.Value > 0 && indoors != null)
        {
          this.currentLocation?.characters.Remove(this);
          this.currentLocation = indoors;
          if (this.currentLocation != null && !this.currentLocation.characters.Contains(this))
            this.currentLocation.addCharacter(this);
          string indoorsName = underConstruction.GetIndoorsName();
          if ((indoorsName != null ? (indoorsName.StartsWith("Shed") ? 1 : 0) : 0) != 0)
          {
            this.setTilePosition(2, 2);
            this.position.X -= 28f;
          }
          else
            this.setTilePosition(1, 5);
        }
        else
        {
          Game1.warpCharacter(this, underConstruction.parentLocationName.Value, new Vector2((float) (underConstruction.tileX.Value + underConstruction.tilesWide.Value / 2), (float) (underConstruction.tileY.Value + underConstruction.tilesHigh.Value / 2)));
          this.position.X += 16f;
          this.position.Y -= 32f;
        }
        this.isPlayingRobinHammerAnimation = false;
        this.shouldPlayRobinHammerAnimation.Value = true;
        return;
      }
      if (Game1.RequireLocation<Town>("Town").daysUntilCommunityUpgrade.Value > 0)
      {
        if (Game1.MasterPlayer.mailReceived.Contains("pamHouseUpgrade"))
        {
          Game1.warpCharacter(this, "Backwoods", new Vector2(41f, 23f));
          this.isPlayingRobinHammerAnimation = false;
          this.shouldPlayRobinHammerAnimation.Value = true;
          return;
        }
        if (!Game1.MasterPlayer.mailReceived.Contains("pamHouseUpgrade"))
          return;
        Game1.warpCharacter(this, "Town", new Vector2(77f, 68f));
        this.isPlayingRobinHammerAnimation = false;
        this.shouldPlayRobinHammerAnimation.Value = true;
        return;
      }
    }
    this.shouldPlayRobinHammerAnimation.Value = false;
  }

  private void doPlayRobinHammerAnimation()
  {
    this.Sprite.ClearAnimation();
    this.Sprite.AddFrame(new FarmerSprite.AnimationFrame(24, 75));
    this.Sprite.AddFrame(new FarmerSprite.AnimationFrame(25, 75));
    this.Sprite.AddFrame(new FarmerSprite.AnimationFrame(26, 300, false, false, new AnimatedSprite.endOfAnimationBehavior(this.robinHammerSound)));
    this.Sprite.AddFrame(new FarmerSprite.AnimationFrame(27, 1000, false, false, new AnimatedSprite.endOfAnimationBehavior(this.robinVariablePause)));
    this.ignoreScheduleToday = true;
    bool flag = Game1.player.daysUntilHouseUpgrade.Value == 1 || Game1.RequireLocation<Town>("Town").daysUntilCommunityUpgrade.Value == 1;
    this.CurrentDialogue.Clear();
    this.CurrentDialogue.Push(new StardewValley.Dialogue(this, flag ? "Strings\\StringsFromCSFiles:NPC.cs.3927" : "Strings\\StringsFromCSFiles:NPC.cs.3926"));
  }

  public void showTextAboveHead(
    string text,
    Color? spriteTextColor = null,
    int style = 2,
    int duration = 3000,
    int preTimer = 0)
  {
    if (this.IsInvisible)
      return;
    this.textAboveHeadAlpha = 0.0f;
    this.textAboveHead = StardewValley.Dialogue.applyGenderSwitchBlocks(Game1.player.Gender, text);
    this.textAboveHeadPreTimer = preTimer;
    this.textAboveHeadTimer = duration;
    this.textAboveHeadStyle = style;
    this.textAboveHeadColor = spriteTextColor;
  }

  public virtual bool hitWithTool(Tool t) => false;

  /// <summary>Get whether this NPC can receive gifts from the player (regardless of whether they've already received one today).</summary>
  public bool CanReceiveGifts()
  {
    if (!this.CanSocialize || this.SimpleNonVillagerNPC || !Game1.NPCGiftTastes.ContainsKey(this.Name))
      return false;
    CharacterData data = this.GetData();
    return data == null || data.CanReceiveGifts;
  }

  /// <summary>Get how much the NPC likes receiving an item as a gift.</summary>
  /// <param name="item">The item to check.</param>
  /// <returns>Returns one of <see cref="F:StardewValley.NPC.gift_taste_hate" />, <see cref="F:StardewValley.NPC.gift_taste_dislike" />, <see cref="F:StardewValley.NPC.gift_taste_neutral" />, <see cref="F:StardewValley.NPC.gift_taste_like" />, or <see cref="F:StardewValley.NPC.gift_taste_love" />.</returns>
  public int getGiftTasteForThisItem(Item item)
  {
    if (item.QualifiedItemId == "(O)StardropTea")
      return 7;
    int tasteForThisItem = 8;
    if (item is Object @object)
    {
      int category = @object.Category;
      string str1 = category.ToString() ?? "";
      string[] strArray1 = ArgUtility.SplitBySpace(Game1.NPCGiftTastes["Universal_Love"]);
      string[] strArray2 = ArgUtility.SplitBySpace(Game1.NPCGiftTastes["Universal_Hate"]);
      string[] strArray3 = ArgUtility.SplitBySpace(Game1.NPCGiftTastes["Universal_Like"]);
      string[] strArray4 = ArgUtility.SplitBySpace(Game1.NPCGiftTastes["Universal_Dislike"]);
      string[] list = ArgUtility.SplitBySpace(Game1.NPCGiftTastes["Universal_Neutral"]);
      if (((IEnumerable<string>) strArray1).Contains<string>(str1))
        tasteForThisItem = 0;
      else if (((IEnumerable<string>) strArray2).Contains<string>(str1))
        tasteForThisItem = 6;
      else if (((IEnumerable<string>) strArray3).Contains<string>(str1))
        tasteForThisItem = 2;
      else if (((IEnumerable<string>) strArray4).Contains<string>(str1))
        tasteForThisItem = 4;
      if (this.CheckTasteContextTags((Item) @object, strArray1))
        tasteForThisItem = 0;
      else if (this.CheckTasteContextTags((Item) @object, strArray2))
        tasteForThisItem = 6;
      else if (this.CheckTasteContextTags((Item) @object, strArray3))
        tasteForThisItem = 2;
      else if (this.CheckTasteContextTags((Item) @object, strArray4))
        tasteForThisItem = 4;
      bool flag1 = false;
      bool flag2 = false;
      if (this.CheckTaste((IEnumerable<string>) strArray1, (Item) @object))
      {
        tasteForThisItem = 0;
        flag1 = true;
      }
      else if (this.CheckTaste((IEnumerable<string>) strArray2, (Item) @object))
      {
        tasteForThisItem = 6;
        flag1 = true;
      }
      else if (this.CheckTaste((IEnumerable<string>) strArray3, (Item) @object))
      {
        tasteForThisItem = 2;
        flag1 = true;
      }
      else if (this.CheckTaste((IEnumerable<string>) strArray4, (Item) @object))
      {
        tasteForThisItem = 4;
        flag1 = true;
      }
      else if (this.CheckTaste((IEnumerable<string>) list, (Item) @object))
      {
        tasteForThisItem = 8;
        flag1 = true;
        flag2 = true;
      }
      if (@object.Type == "Arch")
      {
        tasteForThisItem = 4;
        if (this.Name.Equals("Penny") || this.name.Equals((object) "Dwarf"))
          tasteForThisItem = 2;
      }
      if (tasteForThisItem == 8 && !flag2)
      {
        if (@object.edibility.Value != -300 && @object.edibility.Value < 0)
          tasteForThisItem = 6;
        else if (@object.price.Value < 20)
          tasteForThisItem = 4;
      }
      string str2;
      if (Game1.NPCGiftTastes.TryGetValue(this.Name, out str2))
      {
        string[] strArray5 = str2.Split('/');
        List<string[]> strArrayList = new List<string[]>();
        for (int index1 = 0; index1 < 10; index1 += 2)
        {
          string[] strArray6 = ArgUtility.SplitBySpace(strArray5[index1 + 1]);
          string[] strArray7 = new string[strArray6.Length];
          for (int index2 = 0; index2 < strArray6.Length; ++index2)
          {
            if (strArray6[index2].Length > 0)
              strArray7[index2] = strArray6[index2];
          }
          strArrayList.Add(strArray7);
        }
        if (this.CheckTaste((IEnumerable<string>) strArrayList[0], (Item) @object))
          return 0;
        if (this.CheckTaste((IEnumerable<string>) strArrayList[3], (Item) @object))
          return 6;
        if (this.CheckTaste((IEnumerable<string>) strArrayList[1], (Item) @object))
          return 2;
        if (this.CheckTaste((IEnumerable<string>) strArrayList[2], (Item) @object))
          return 4;
        if (this.CheckTaste((IEnumerable<string>) strArrayList[4], (Item) @object))
          return 8;
        if (this.CheckTasteContextTags((Item) @object, strArrayList[0]))
          return 0;
        if (this.CheckTasteContextTags((Item) @object, strArrayList[3]))
          return 6;
        if (this.CheckTasteContextTags((Item) @object, strArrayList[1]))
          return 2;
        if (this.CheckTasteContextTags((Item) @object, strArrayList[2]))
          return 4;
        if (this.CheckTasteContextTags((Item) @object, strArrayList[4]))
          return 8;
        if (!flag1)
        {
          if (category != 0 && ((IEnumerable<string>) strArrayList[0]).Contains<string>(str1))
            return 0;
          if (category != 0 && ((IEnumerable<string>) strArrayList[3]).Contains<string>(str1))
            return 6;
          if (category != 0 && ((IEnumerable<string>) strArrayList[1]).Contains<string>(str1))
            return 2;
          if (category != 0 && ((IEnumerable<string>) strArrayList[2]).Contains<string>(str1))
            return 4;
          if (category != 0 && ((IEnumerable<string>) strArrayList[4]).Contains<string>(str1))
            return 8;
        }
      }
    }
    return tasteForThisItem;
  }

  public bool CheckTaste(IEnumerable<string> list, Item item)
  {
    foreach (string itemId in list)
    {
      if (itemId != null && !itemId.StartsWith('-'))
      {
        ParsedItemData data = ItemRegistry.GetData(itemId);
        if (data?.ItemType != null && item.QualifiedItemId == data.QualifiedItemId)
          return true;
      }
    }
    return false;
  }

  public virtual bool CheckTasteContextTags(Item item, string[] list)
  {
    foreach (string tag in list)
    {
      if (tag != null && tag.Length > 0 && !char.IsNumber(tag[0]) && tag[0] != '-' && item.HasContextTag(tag))
        return true;
    }
    return false;
  }

  private void goblinDoorEndBehavior(Character c, GameLocation l)
  {
    l.characters.Remove(this);
    l.playSound("doorClose");
  }

  private void performRemoveHenchman()
  {
    this.Sprite.CurrentFrame = 4;
    Game1.netWorldState.Value.IsGoblinRemoved = true;
    Game1.player.removeQuest("27");
    Stack<Point> pathToEndPoint = new Stack<Point>();
    pathToEndPoint.Push(new Point(20, 21));
    pathToEndPoint.Push(new Point(20, 22));
    pathToEndPoint.Push(new Point(20, 23));
    pathToEndPoint.Push(new Point(20, 24));
    pathToEndPoint.Push(new Point(20, 25));
    pathToEndPoint.Push(new Point(20, 26));
    pathToEndPoint.Push(new Point(20, 27));
    pathToEndPoint.Push(new Point(20, 28));
    this.addedSpeed = 2f;
    this.controller = new PathFindController(pathToEndPoint, (Character) this, this.currentLocation);
    this.controller.endBehaviorFunction = new PathFindController.endBehavior(this.goblinDoorEndBehavior);
    this.showTextAboveHead(Game1.content.LoadString("Strings\\Characters:Henchman6"));
    Game1.player.mailReceived.Add("henchmanGone");
    this.currentLocation.removeTile(20, 29, "Buildings");
  }

  private void engagementResponse(Farmer who, bool asRoommate = false)
  {
    Game1.changeMusicTrack("silence");
    who.spouse = this.Name;
    if (!asRoommate)
      Game1.multiplayer.globalChatInfoMessage("Engaged", Game1.player.Name, this.GetTokenizedDisplayName());
    Friendship friendship = who.friendshipData[this.Name];
    friendship.Status = FriendshipStatus.Engaged;
    friendship.RoommateMarriage = asRoommate;
    WorldDate worldDate = new WorldDate(Game1.Date);
    worldDate.TotalDays += 3;
    who.removeDatingActiveDialogueEvents(Game1.player.spouse);
    while (!Game1.canHaveWeddingOnDay(worldDate.DayOfMonth, worldDate.Season))
      ++worldDate.TotalDays;
    friendship.WeddingDate = worldDate;
    this.CurrentDialogue.Clear();
    if (asRoommate && DataLoader.EngagementDialogue(Game1.content).ContainsKey(this.Name + "Roommate0"))
    {
      this.CurrentDialogue.Push(new StardewValley.Dialogue(this, $"Data\\EngagementDialogue:{this.Name}Roommate0"));
      StardewValley.Dialogue dialogue1 = StardewValley.Dialogue.TryGetDialogue(this, $"Strings\\StringsFromCSFiles:{this.Name}_EngagedRoommate");
      if (dialogue1 != null)
      {
        this.CurrentDialogue.Push(dialogue1);
      }
      else
      {
        StardewValley.Dialogue dialogue2 = StardewValley.Dialogue.TryGetDialogue(this, $"Strings\\StringsFromCSFiles:{this.Name}_Engaged");
        if (dialogue2 != null)
          this.CurrentDialogue.Push(dialogue2);
        else
          this.CurrentDialogue.Push(new StardewValley.Dialogue(this, "Strings\\StringsFromCSFiles:NPC.cs.3980"));
      }
    }
    else
    {
      StardewValley.Dialogue dialogue3 = StardewValley.Dialogue.TryGetDialogue(this, $"Data\\EngagementDialogue:{this.Name}0");
      if (dialogue3 != null)
        this.CurrentDialogue.Push(dialogue3);
      StardewValley.Dialogue dialogue4 = StardewValley.Dialogue.TryGetDialogue(this, $"Strings\\StringsFromCSFiles:{this.Name}_Engaged");
      if (dialogue4 != null)
        this.CurrentDialogue.Push(dialogue4);
      else
        this.CurrentDialogue.Push(new StardewValley.Dialogue(this, "Strings\\StringsFromCSFiles:NPC.cs.3980"));
    }
    this.CurrentDialogue.Peek().onFinish += (Action) (() =>
    {
      Game1.changeMusicTrack("none", true);
      GameLocation.HandleMusicChange((GameLocation) null, who.currentLocation);
    });
    who.changeFriendship(1, this);
    who.reduceActiveItemByOne();
    who.completelyStopAnimatingOrDoingAction();
    Game1.drawDialogue(this);
  }

  /// <summary>Try to receive an item from the player.</summary>
  /// <param name="who">The player whose active object to receive.</param>
  /// <param name="probe">Whether to return what the method would return if called normally, but without actually accepting the item or making any changes to the NPC. This is used to accurately predict whether the NPC would accept or react to the offer.</param>
  /// <returns>Returns true if the NPC accepted the item or reacted to the offer, else false.</returns>
  public virtual bool tryToReceiveActiveObject(Farmer who, bool probe = false)
  {
    if (this.SimpleNonVillagerNPC)
      return false;
    Object activeObj = who.ActiveObject;
    if (activeObj == null)
      return false;
    if (!probe)
    {
      who.Halt();
      who.faceGeneralDirection(this.getStandingPosition(), 0, false, false);
    }
    if (this.Name == "Henchman" && Game1.currentLocation.NameOrUniqueName == "WitchSwamp")
    {
      if (activeObj.QualifiedItemId == "(O)308")
      {
        if (this.controller != null)
          return false;
        if (!probe)
        {
          who.currentLocation.localSound("coin");
          who.reduceActiveItemByOne();
          this.CurrentDialogue.Push(new StardewValley.Dialogue(this, "Strings\\Characters:Henchman5"));
          Game1.drawDialogue(this);
          who.freezePause = 2000;
          this.removeHenchmanEvent.Fire();
        }
      }
      else if (!probe)
      {
        this.CurrentDialogue.Push(new StardewValley.Dialogue(this, activeObj.QualifiedItemId == "(O)684" ? "Strings\\Characters:Henchman4" : "Strings\\Characters:Henchman3"));
        Game1.drawDialogue(this);
      }
      return true;
    }
    if (Game1.player.team.specialOrders != null)
    {
      foreach (SpecialOrder specialOrder in Game1.player.team.specialOrders)
      {
        if (specialOrder.onItemDelivered != null)
        {
          foreach (Func<Farmer, NPC, Item, bool, int> invocation in specialOrder.onItemDelivered.GetInvocationList())
          {
            if (invocation(Game1.player, this, (Item) activeObj, probe) > 0)
            {
              if (!probe && activeObj.Stack <= 0)
              {
                who.ActiveObject = (Object) null;
                who.showNotCarrying();
              }
              return true;
            }
          }
        }
      }
    }
    if (who.NotifyQuests((Func<Quest, bool>) (quest => quest.OnItemOfferedToNpc(this, (Item) activeObj, probe)), true))
    {
      if (!probe)
      {
        who.completelyStopAnimatingOrDoingAction();
        if (Game1.random.NextDouble() < 0.3 && this.Name != "Wizard")
          this.doEmote(32 /*0x20*/);
      }
      return true;
    }
    string qualifiedItemId = who.ActiveObject?.QualifiedItemId;
    if (qualifiedItemId != null)
    {
      switch (qualifiedItemId.Length)
      {
        case 5:
          if (qualifiedItemId == "(O)71")
          {
            if (!(this.Name == "Lewis") || !who.hasQuest("102"))
              return false;
            if (!probe)
            {
              if (who.currentLocation?.NameOrUniqueName == "IslandSouth")
                Game1.player.activeDialogueEvents["lucky_pants_lewis"] = 28;
              who.completeQuest("102");
              this.setNewDialogue(new StardewValley.Dialogue(this, (string) null, ArgUtility.Get(Quest.GetRawQuestFields("102"), 9, "Data\\ExtraDialogue:LostItemQuest_DefaultThankYou", false)));
              Game1.drawDialogue(this);
              Game1.player.changeFriendship(250, this);
              who.ActiveObject = (Object) null;
            }
            return true;
          }
          break;
        case 6:
          switch (qualifiedItemId[5])
          {
            case '0':
              if (qualifiedItemId == "(O)870")
                break;
              goto label_71;
            case '3':
              if (qualifiedItemId == "(O)233" && this.name.Value == "Jas" && Utility.GetDayOfPassiveFestival("DesertFestival") > 0 && this.currentLocation is Desert && !who.mailReceived.Contains("Jas_IceCream_DF_" + Game1.year.ToString()))
              {
                if (!probe)
                {
                  who.reduceActiveItemByOne();
                  this.jump();
                  this.doEmote(16 /*0x10*/);
                  this.CurrentDialogue.Clear();
                  this.setNewDialogue("Strings\\1_6_Strings:Jas_IceCream", true);
                  Game1.drawDialogue(this);
                  who.mailReceived.Add("Jas_IceCream_DF_" + Game1.year.ToString());
                  who.changeFriendship(200, this);
                }
                return true;
              }
              goto label_71;
            case '4':
              if (qualifiedItemId == "(O)864")
                break;
              goto label_71;
            case '5':
              if (qualifiedItemId == "(O)865")
                break;
              goto label_71;
            case '6':
              if (qualifiedItemId == "(O)866")
                break;
              goto label_71;
            case '7':
              switch (qualifiedItemId)
              {
                case "(O)897":
                  if (!probe)
                  {
                    if (this.Name == "Pierre" && !Game1.player.hasOrWillReceiveMail("PierreStocklist"))
                    {
                      Game1.addMail("PierreStocklist", true, true);
                      who.reduceActiveItemByOne();
                      who.completelyStopAnimatingOrDoingAction();
                      who.currentLocation.localSound("give_gift");
                      Game1.player.team.itemsToRemoveOvernight.Add("897");
                      this.setNewDialogue("Strings\\Characters:PierreStockListDialogue", true);
                      Game1.drawDialogue(this);
                      Game1.afterDialogues += (Game1.afterFadeFunction) (() => Game1.multiplayer.globalChatInfoMessage("StockList"));
                    }
                    else
                      Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\Characters:MovieInvite_NoTheater", (object) this.displayName)));
                  }
                  return true;
                case "(O)867":
                  break;
                default:
                  goto label_71;
              }
              break;
            case '8':
              if (qualifiedItemId == "(O)868")
                break;
              goto label_71;
            case '9':
              if (qualifiedItemId == "(O)869")
                break;
              goto label_71;
            default:
              goto label_71;
          }
          if (who.hasQuest("130"))
          {
            StardewValley.Dialogue dialogue1 = this.TryGetDialogue("accept_" + activeObj.ItemId);
            if (dialogue1 != null)
            {
              if (!probe)
              {
                this.setNewDialogue(dialogue1);
                Game1.drawDialogue(this);
                this.CurrentDialogue.Peek().onFinish = (Action) (() =>
                {
                  Object o = ItemRegistry.Create<Object>("(O)" + (activeObj.ParentSheetIndex + 1).ToString());
                  o.specialItem = true;
                  o.questItem.Value = true;
                  who.reduceActiveItemByOne();
                  DelayedAction.playSoundAfterDelay("coin", 200);
                  DelayedAction.functionAfterDelay((Action) (() => who.addItemByMenuIfNecessary((Item) o)), 200);
                  Game1.player.freezePause = 550;
                  DelayedAction.functionAfterDelay((Action) (() => Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:Farmer.cs.1919", (object) o.DisplayName, (object) Lexicon.getProperArticleForWord(o.DisplayName)))), 550);
                });
                this.faceTowardFarmerForPeriod(6000, 4, false, who);
              }
              return true;
            }
            StardewValley.Dialogue dialogue2 = this.TryGetDialogue("reject_" + activeObj.ItemId);
            if (dialogue2 != null)
            {
              if (!probe)
              {
                this.setNewDialogue(dialogue2);
                Game1.drawDialogue(this);
              }
              return true;
            }
          }
          return false;
      }
    }
label_71:
    if (activeObj.questItem.Value)
      return false;
    StardewValley.Dialogue dialogue3 = this.TryGetDialogue("RejectItem_" + activeObj.QualifiedItemId) ?? activeObj.GetContextTags().Select<string, StardewValley.Dialogue>((Func<string, StardewValley.Dialogue>) (tag => this.TryGetDialogue("RejectItem_" + tag))).FirstOrDefault<StardewValley.Dialogue>((Func<StardewValley.Dialogue, bool>) (p => p != null)) ?? (activeObj.HasTypeObject() ? this.TryGetDialogue("reject_" + activeObj.ItemId) : (StardewValley.Dialogue) null);
    if (dialogue3 != null)
    {
      if (!probe)
      {
        this.setNewDialogue(dialogue3);
        Game1.drawDialogue(this);
      }
      return true;
    }
    Friendship friendship;
    who.friendshipData.TryGetValue(this.Name, out friendship);
    bool gifts = this.CanReceiveGifts();
    switch (activeObj.QualifiedItemId)
    {
      case "(O)809":
        if (!Utility.doesMasterPlayerHaveMailReceivedButNotMailForTomorrow("ccMovieTheater"))
        {
          if (!probe)
            Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\Characters:MovieInvite_NoTheater", (object) this.displayName)));
          return true;
        }
        if (this.SpeaksDwarvish() && !who.canUnderstandDwarves)
        {
          if (!probe)
            Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\Characters:MovieInvite_NoTheater", (object) this.displayName)));
          return true;
        }
        switch (this.Name)
        {
          case "Krobus":
            if (Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth) == "Fri")
            {
              if (!probe)
                Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\Characters:MovieInvite_NoTheater", (object) this.displayName)));
              return true;
            }
            break;
          case "Leo":
            if (!Game1.MasterPlayer.mailReceived.Contains("leoMoved"))
            {
              if (!probe)
                Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\Characters:MovieInvite_NoTheater", (object) this.displayName)));
              return true;
            }
            break;
        }
        if (!this.IsVillager || !this.CanSocialize)
        {
          if (!probe)
            Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\Characters:MovieInvite_CantInvite", (object) this.displayName)));
          return true;
        }
        if (friendship == null)
        {
          if (!probe)
            Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\Characters:MovieInvite_NoTheater", (object) this.displayName)));
          return true;
        }
        if (friendship.IsDivorced())
        {
          if (!probe)
          {
            if (who == Game1.player)
              Game1.multiplayer.globalChatInfoMessage("MovieInviteReject", Game1.player.displayName, this.GetTokenizedDisplayName());
            this.CurrentDialogue.Push(this.TryGetDialogue("RejectMovieTicket_Divorced") ?? this.TryGetDialogue("RejectMovieTicket") ?? new StardewValley.Dialogue(this, "Strings\\Characters:Divorced_gift"));
            Game1.drawDialogue(this);
          }
          return true;
        }
        if (who.lastSeenMovieWeek.Value >= Game1.Date.TotalWeeks)
        {
          if (!probe)
            Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\Characters:MovieInvite_FarmerAlreadySeen")));
          return true;
        }
        if (Utility.isFestivalDay())
        {
          if (!probe)
            Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\Characters:MovieInvite_Festival")));
          return true;
        }
        if (Game1.timeOfDay > 2100)
        {
          if (!probe)
            Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\Characters:MovieInvite_Closed")));
          return true;
        }
        foreach (MovieInvitation movieInvitation in who.team.movieInvitations)
        {
          if (movieInvitation.farmer == who)
          {
            if (!probe)
              Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\Characters:MovieInvite_AlreadyInvitedSomeone", (object) movieInvitation.invitedNPC.displayName)));
            return true;
          }
        }
        if (!probe)
          this.faceTowardFarmerForPeriod(4000, 3, false, who);
        foreach (MovieInvitation movieInvitation in who.team.movieInvitations)
        {
          if (movieInvitation.invitedNPC == this)
          {
            if (!probe)
            {
              if (who == Game1.player)
                Game1.multiplayer.globalChatInfoMessage("MovieInviteReject", Game1.player.displayName, this.GetTokenizedDisplayName());
              Stack<StardewValley.Dialogue> currentDialogue = this.CurrentDialogue;
              StardewValley.Dialogue dialogue4 = this.TryGetDialogue("RejectMovieTicket_AlreadyInvitedBySomeoneElse", (object) movieInvitation.farmer.displayName);
              if (dialogue4 == null)
              {
                StardewValley.Dialogue dialogue5 = this.TryGetDialogue("RejectMovieTicket");
                if (dialogue5 == null)
                  dialogue4 = new StardewValley.Dialogue(this, "Strings\\Characters:MovieInvite_InvitedBySomeoneElse", this.GetDispositionModifiedString("Strings\\Characters:MovieInvite_InvitedBySomeoneElse", (object) movieInvitation.farmer.displayName));
                else
                  dialogue4 = dialogue5;
              }
              currentDialogue.Push(dialogue4);
              Game1.drawDialogue(this);
            }
            return true;
          }
        }
        if (this.lastSeenMovieWeek.Value >= Game1.Date.TotalWeeks)
        {
          if (!probe)
          {
            if (who == Game1.player)
              Game1.multiplayer.globalChatInfoMessage("MovieInviteReject", Game1.player.displayName, this.GetTokenizedDisplayName());
            this.CurrentDialogue.Push(this.TryGetDialogue("RejectMovieTicket_AlreadyWatchedThisWeek") ?? this.TryGetDialogue("RejectMovieTicket") ?? new StardewValley.Dialogue(this, "Strings\\Characters:MovieInvite_AlreadySeen", this.GetDispositionModifiedString("Strings\\Characters:MovieInvite_AlreadySeen")));
            Game1.drawDialogue(this);
          }
          return true;
        }
        if (MovieTheater.GetResponseForMovie(this) == "reject")
        {
          if (!probe)
          {
            if (who == Game1.player)
              Game1.multiplayer.globalChatInfoMessage("MovieInviteReject", Game1.player.displayName, this.GetTokenizedDisplayName());
            this.CurrentDialogue.Push(this.TryGetDialogue("RejectMovieTicket_DontWantToSeeThatMovie") ?? this.TryGetDialogue("RejectMovieTicket") ?? new StardewValley.Dialogue(this, "Strings\\Characters:MovieInvite_Reject", this.GetDispositionModifiedString("Strings\\Characters:MovieInvite_Reject")));
            Game1.drawDialogue(this);
          }
          return true;
        }
        if (!probe)
        {
          this.CurrentDialogue.Push((this.getSpouse() == who ? StardewValley.Dialogue.TryGetDialogue(this, "Strings\\Characters:MovieInvite_Spouse_" + this.name.Value) : (StardewValley.Dialogue) null) ?? this.TryGetDialogue("MovieInvitation") ?? new StardewValley.Dialogue(this, "Strings\\Characters:MovieInvite_Invited", this.GetDispositionModifiedString("Strings\\Characters:MovieInvite_Invited")));
          Game1.drawDialogue(this);
          who.reduceActiveItemByOne();
          who.completelyStopAnimatingOrDoingAction();
          who.currentLocation.localSound("give_gift");
          MovieTheater.Invite(who, this);
          if (who == Game1.player)
            Game1.multiplayer.globalChatInfoMessage("MovieInviteAccept", Game1.player.displayName, this.GetTokenizedDisplayName());
        }
        return true;
      case "(O)458":
        if (!gifts)
          return false;
        bool flag1 = who.spouse != this.Name && this.isMarriedOrEngaged();
        if (!this.datable.Value | flag1)
        {
          if (!probe)
          {
            if (Game1.random.NextBool())
            {
              Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.3955", (object) this.displayName));
            }
            else
            {
              Stack<StardewValley.Dialogue> currentDialogue = this.CurrentDialogue;
              StardewValley.Dialogue dialogue6 = !this.datable.Value ? this.TryGetDialogue("RejectBouquet_NotDatable") : (StardewValley.Dialogue) null;
              if (dialogue6 == null)
              {
                StardewValley.Dialogue dialogue7;
                if (!flag1)
                  dialogue7 = (StardewValley.Dialogue) null;
                else
                  dialogue7 = this.TryGetDialogue("RejectBouquet_NpcAlreadyMarried", (object) this.getSpouse()?.Name);
                dialogue6 = dialogue7 ?? this.TryGetDialogue("RejectBouquet") ?? (Game1.random.NextBool() ? new StardewValley.Dialogue(this, "Strings\\StringsFromCSFiles:NPC.cs.3956") : new StardewValley.Dialogue(this, "Strings\\StringsFromCSFiles:NPC.cs.3957", true));
              }
              currentDialogue.Push(dialogue6);
              Game1.drawDialogue(this);
            }
          }
          return true;
        }
        if (friendship == null)
          who.friendshipData[this.Name] = friendship = new Friendship();
        if (friendship.IsDating())
        {
          if (!probe)
          {
            StardewValley.Dialogue dialogue8 = this.TryGetDialogue($"RejectBouquet_AlreadyAccepted_{friendship.Status}") ?? this.TryGetDialogue("RejectBouquet_AlreadyAccepted");
            if (dialogue8 != null)
            {
              this.CurrentDialogue.Push(dialogue8);
              Game1.drawDialogue(this);
            }
            else
              Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\UI:AlreadyDatingBouquet", (object) this.displayName));
          }
          return true;
        }
        if (friendship.IsDivorced())
        {
          if (!probe)
          {
            this.CurrentDialogue.Push(this.TryGetDialogue("RejectBouquet_Divorced") ?? this.TryGetDialogue("RejectBouquet") ?? new StardewValley.Dialogue(this, "Strings\\Characters:Divorced_bouquet"));
            Game1.drawDialogue(this);
          }
          return true;
        }
        if (friendship.Points < 1000)
        {
          if (!probe)
          {
            this.CurrentDialogue.Push(this.TryGetDialogue("RejectBouquet_VeryLowHearts") ?? this.TryGetDialogue("RejectBouquet") ?? (Game1.random.NextBool() ? new StardewValley.Dialogue(this, "Strings\\StringsFromCSFiles:NPC.cs.3958") : new StardewValley.Dialogue(this, "Strings\\StringsFromCSFiles:NPC.cs.3959", true)));
            Game1.drawDialogue(this);
          }
          return true;
        }
        if (friendship.Points < 2000)
        {
          if (!probe)
          {
            this.CurrentDialogue.Push(this.TryGetDialogue("RejectBouquet_LowHearts") ?? this.TryGetDialogue("RejectBouquet") ?? new StardewValley.Dialogue(this, "Strings\\StringsFromCSFiles:NPC.cs." + Game1.random.Choose<string>("3960", "3961")));
            Game1.drawDialogue(this);
          }
          return true;
        }
        if (!probe)
        {
          friendship.Status = FriendshipStatus.Dating;
          Game1.multiplayer.globalChatInfoMessage("Dating", Game1.player.Name, this.GetTokenizedDisplayName());
          this.CurrentDialogue.Push(this.TryGetDialogue("AcceptBouquet") ?? new StardewValley.Dialogue(this, "Strings\\StringsFromCSFiles:NPC.cs." + Game1.random.Choose<string>("3962", "3963"), true));
          who.autoGenerateActiveDialogueEvent("dating_" + this.Name);
          who.autoGenerateActiveDialogueEvent("dating");
          who.changeFriendship(25, this);
          who.reduceActiveItemByOne();
          who.completelyStopAnimatingOrDoingAction();
          this.doEmote(20);
          Game1.drawDialogue(this);
        }
        return true;
      case "(O)277":
        if (!gifts)
          return false;
        if (!probe)
        {
          if (!this.datable.Value || (friendship != null ? (!friendship.IsDating() ? 1 : 0) : 1) != 0 || friendship != null && friendship.IsMarried())
          {
            Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:Wilted_Bouquet_Meaningless", (object) this.displayName));
          }
          else
          {
            Game1.showGlobalMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Wilted_Bouquet_Effect", (object) this.displayName));
            Game1.multiplayer.globalChatInfoMessage("BreakUp", Game1.player.Name, this.GetTokenizedDisplayName());
            who.removeDatingActiveDialogueEvents(this.Name);
            who.reduceActiveItemByOne();
            friendship.Status = FriendshipStatus.Friendly;
            if (who.spouse == this.Name)
              who.spouse = (string) null;
            friendship.WeddingDate = (WorldDate) null;
            who.completelyStopAnimatingOrDoingAction();
            friendship.Points = Math.Min(friendship.Points, 1250);
            switch (this.name.Value)
            {
              case "Maru":
              case "Haley":
                this.doEmote(12);
                goto case "Shane";
              case "Shane":
              case "Alex":
                this.CurrentDialogue.Clear();
                this.CurrentDialogue.Push(new StardewValley.Dialogue(this, $"Characters\\Dialogue\\{this.GetDialogueSheetName()}:breakUp"));
                Game1.drawDialogue(this);
                break;
              default:
                this.doEmote(28);
                goto case "Shane";
            }
          }
        }
        return true;
      case "(O)460":
        if (!gifts)
          return false;
        bool flag2 = friendship != null && friendship.IsDivorced();
        if (who.spouse == this.Name)
        {
          StardewValley.Dialogue dialogue9 = this.TryGetDialogue($"RejectMermaidPendant_AlreadyAccepted_{friendship?.Status}") ?? this.TryGetDialogue("RejectMermaidPendant_AlreadyAccepted");
          if (!probe && dialogue9 != null)
          {
            this.CurrentDialogue.Push(dialogue9);
            Game1.drawDialogue(this);
          }
          return dialogue9 != null;
        }
        if (who.isMarriedOrRoommates() || who.isEngaged())
        {
          if (!probe)
          {
            if (who.hasCurrentOrPendingRoommate())
              Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:TriedToMarryButKrobus"));
            else if (who.isEngaged())
            {
              this.CurrentDialogue.Push(this.TryGetDialogue("RejectMermaidPendant_PlayerWithSomeoneElse", (object) (who.getSpouse()?.displayName ?? who.spouse)) ?? this.TryGetDialogue("RejectMermaidPendant") ?? new StardewValley.Dialogue(this, "Strings\\StringsFromCSFiles:NPC.cs." + Game1.random.Choose<string>("3965", "3966"), true));
              Game1.drawDialogue(this);
            }
            else
            {
              this.CurrentDialogue.Push(this.TryGetDialogue("RejectMermaidPendant_PlayerWithSomeoneElse") ?? this.TryGetDialogue("RejectMermaidPendant") ?? (Game1.random.NextBool() ? new StardewValley.Dialogue(this, "Strings\\StringsFromCSFiles:NPC.cs.3967") : new StardewValley.Dialogue(this, "Strings\\StringsFromCSFiles:NPC.cs.3968", true)));
              Game1.drawDialogue(this);
            }
          }
          return true;
        }
        if (((!this.datable.Value ? 1 : (this.isMarriedOrEngaged() ? 1 : 0)) | (flag2 ? 1 : 0)) != 0 || friendship != null && friendship.Points < 1500)
        {
          if (!probe)
          {
            if (Game1.random.NextBool())
            {
              Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.3969", (object) this.displayName));
            }
            else
            {
              Stack<StardewValley.Dialogue> currentDialogue = this.CurrentDialogue;
              StardewValley.Dialogue dialogue10 = !this.datable.Value ? this.TryGetDialogue("RejectMermaidPendant_NotDatable") : (StardewValley.Dialogue) null;
              if (dialogue10 == null)
              {
                StardewValley.Dialogue dialogue11 = flag2 ? this.TryGetDialogue("RejectMermaidPendant_Divorced") : (StardewValley.Dialogue) null;
                if (dialogue11 == null)
                {
                  StardewValley.Dialogue dialogue12;
                  if (!this.isMarriedOrEngaged())
                    dialogue12 = (StardewValley.Dialogue) null;
                  else
                    dialogue12 = this.TryGetDialogue("RejectMermaidPendant_NpcWithSomeoneElse", (object) this.getSpouse()?.Name);
                  dialogue10 = dialogue12 ?? (!this.datable.Value || friendship == null || friendship.Points >= 1500 ? (StardewValley.Dialogue) null : this.TryGetDialogue("RejectMermaidPendant_Under8Hearts")) ?? this.TryGetDialogue("RejectMermaidPendant") ?? new StardewValley.Dialogue(this, "Strings\\StringsFromCSFiles:NPC.cs." + (this.Gender == Gender.Female ? "3970" : "3971"));
                }
                else
                  dialogue10 = dialogue11;
              }
              currentDialogue.Push(dialogue10);
              Game1.drawDialogue(this);
            }
          }
          return true;
        }
        if (this.datable.Value && friendship != null && friendship.Points < 2500)
        {
          if (!probe)
          {
            if (!friendship.ProposalRejected)
            {
              this.CurrentDialogue.Push(this.TryGetDialogue("RejectMermaidPendant_Under10Hearts") ?? new StardewValley.Dialogue(this, "Strings\\StringsFromCSFiles:NPC.cs." + Game1.random.Choose<string>("3972", "3973")));
              Game1.drawDialogue(this);
              who.changeFriendship(-20, this);
              friendship.ProposalRejected = true;
            }
            else
            {
              this.CurrentDialogue.Push(this.TryGetDialogue("RejectMermaidPendant_Under10Hearts_AskedAgain") ?? this.TryGetDialogue("RejectMermaidPendant_Under10Hearts") ?? this.TryGetDialogue("RejectMermaidPendant") ?? new StardewValley.Dialogue(this, "Strings\\StringsFromCSFiles:NPC.cs." + Game1.random.Choose<string>("3974", "3975"), true));
              Game1.drawDialogue(this);
              who.changeFriendship(-50, this);
            }
          }
          return true;
        }
        if (this.datable.Value && who.houseUpgradeLevel.Value < 1)
        {
          if (!probe)
          {
            if (Game1.random.NextBool())
            {
              Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.3969", (object) this.displayName));
            }
            else
            {
              this.CurrentDialogue.Push(this.TryGetDialogue("RejectMermaidPendant_NeedHouseUpgrade") ?? this.TryGetDialogue("RejectMermaidPendant") ?? new StardewValley.Dialogue(this, "Strings\\StringsFromCSFiles:NPC.cs.3972"));
              Game1.drawDialogue(this);
            }
          }
          return true;
        }
        if (!probe)
          this.engagementResponse(who);
        return true;
      default:
        if (gifts && activeObj.HasContextTag(ItemContextTagManager.SanitizeContextTag("propose_roommate_" + this.Name)))
        {
          string key = (string) null;
          object[] objArray = (object[]) null;
          bool flag3 = this.Name != "Krobus";
          if (who.spouse == this.Name)
          {
            key = "RejectRoommateProposal_AlreadyAccepted";
            flag3 = false;
          }
          else if (this.isMarriedOrEngaged())
            key = "RejectRoommateProposal_NpcWithSomeoneElse";
          else if (who.isMarriedOrRoommates() || who.isEngaged())
          {
            key = "RejectRoommateProposal_PlayerWithSomeoneElse";
            objArray = new object[1]
            {
              (object) (who.getSpouse()?.displayName ?? who.spouse)
            };
          }
          else if (who.getFriendshipHeartLevelForNPC(this.Name) < 10)
            key = "RejectRoommateProposal_LowFriendship";
          else if (who.houseUpgradeLevel.Value < 1)
            key = "RejectRoommateProposal_SmallHouse";
          if (key != null)
          {
            StardewValley.Dialogue dialogue13 = (objArray != null ? this.TryGetDialogue(key, objArray) : this.TryGetDialogue(key)) ?? this.TryGetDialogue("RejectRoommateProposal");
            if (!probe)
            {
              if (dialogue13 != null)
              {
                this.CurrentDialogue.Push(dialogue13);
                Game1.drawDialogue(this);
              }
              else if (flag3)
                Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\Characters:MovieInvite_NoTheater", (object) this.displayName)));
            }
            return dialogue13 != null | flag3;
          }
          if (!probe)
            this.engagementResponse(who, true);
          return true;
        }
        bool flag4 = ItemContextTagManager.HasBaseTag(activeObj.QualifiedItemId, "not_giftable");
        if (!gifts || !activeObj.canBeGivenAsGift() || flag4)
          return false;
        foreach (string key in who.activeDialogueEvents.Keys)
        {
          if (key.Contains("dumped") && this.Dialogue.ContainsKey(key))
          {
            if (!probe)
              this.doEmote(12);
            return true;
          }
        }
        if (!probe)
          who.completeQuest("25");
        if (Game1.IsGreenRainingHere() && Game1.year == 1 && !this.isMarried())
        {
          if (!probe)
            Game1.showRedMessage(".........");
          return false;
        }
        if (friendship != null && friendship.GiftsThisWeek < 2 || who.spouse == this.Name || this is Child || this.isBirthday() || who.ActiveObject.QualifiedItemId == "(O)StardropTea")
        {
          if (!probe)
          {
            if (friendship == null)
              who.friendshipData[this.Name] = friendship = new Friendship();
            if (friendship.IsDivorced())
            {
              this.CurrentDialogue.Push(this.TryGetDialogue("RejectGift_Divorced") ?? new StardewValley.Dialogue(this, "Strings\\Characters:Divorced_gift"));
              Game1.drawDialogue(this);
              return true;
            }
            if (friendship.GiftsToday == 1 && who.ActiveObject.QualifiedItemId != "(O)StardropTea")
            {
              Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.3981", (object) this.displayName)));
              return true;
            }
            this.receiveGift(who.ActiveObject, who, who.ActiveObject.QualifiedItemId != "(O)StardropTea");
            who.reduceActiveItemByOne();
            who.completelyStopAnimatingOrDoingAction();
            this.faceTowardFarmerForPeriod(4000, 3, false, who);
            if (this.datable.Value && who.spouse != null && who.spouse != this.Name && !who.hasCurrentOrPendingRoommate() && Utility.isMale(who.spouse) == Utility.isMale(this.Name) && Game1.random.NextDouble() < 0.3 - (double) who.LuckLevel / 100.0 - who.DailyLuck && !this.isBirthday() && friendship.IsDating())
            {
              NPC characterFromName = Game1.getCharacterFromName(who.spouse);
              CharacterData data = characterFromName?.GetData();
              if (characterFromName != null && GameStateQuery.CheckConditions(data?.SpouseGiftJealousy, player: who, targetItem: (Item) activeObj))
              {
                who.changeFriendship(data != null ? data.SpouseGiftJealousyFriendshipChange : -30, characterFromName);
                characterFromName.CurrentDialogue.Clear();
                characterFromName.CurrentDialogue.Push(characterFromName.TryGetDialogue("SpouseGiftJealous", (object) this.displayName, (object) activeObj.DisplayName) ?? StardewValley.Dialogue.FromTranslation(characterFromName, "Strings\\StringsFromCSFiles:NPC.cs.3985", (object) this.displayName));
              }
            }
          }
          return true;
        }
        if (!probe)
          Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.3987", (object) this.displayName, (object) 2)));
        return true;
    }
  }

  public string GetDispositionModifiedString(string path, params object[] substitutions)
  {
    List<string> stringList = new List<string>();
    stringList.Add(this.name.Value);
    if (Game1.player.isMarriedOrRoommates() && Game1.player.getSpouse() == this)
      stringList.Add("spouse");
    CharacterData data = this.GetData();
    if (data != null)
    {
      stringList.Add(data.Manner.ToString().ToLower());
      stringList.Add(data.SocialAnxiety.ToString().ToLower());
      stringList.Add(data.Optimism.ToString().ToLower());
      stringList.Add(data.Age.ToString().ToLower());
    }
    foreach (string s in stringList)
    {
      string path1 = $"{path}_{Utility.capitalizeFirstLetter(s)}";
      string dispositionModifiedString = Game1.content.LoadString(path1, substitutions);
      if (!(dispositionModifiedString == path1))
        return dispositionModifiedString;
    }
    return Game1.content.LoadString(path, substitutions);
  }

  public void haltMe(Farmer who) => this.Halt();

  public virtual bool checkAction(Farmer who, GameLocation l)
  {
    if (this.IsInvisible)
      return false;
    if (this.isSleeping.Value)
    {
      if (!this.isEmoting)
        this.doEmote(24);
      this.shake(250);
      return false;
    }
    if (!who.CanMove)
      return false;
    Friendship friendship;
    Game1.player.friendshipData.TryGetValue(this.Name, out friendship);
    if (this.Name.Equals("Henchman") && l.Name.Equals("WitchSwamp"))
    {
      if (Game1.player.mailReceived.Add("Henchman1"))
      {
        this.CurrentDialogue.Push(new StardewValley.Dialogue(this, "Strings\\Characters:Henchman1"));
        Game1.drawDialogue(this);
        Game1.player.addQuest("27");
        if (!Game1.player.friendshipData.ContainsKey("Henchman"))
          Game1.player.friendshipData.Add("Henchman", friendship = new Friendship());
      }
      else
      {
        if (who.ActiveObject != null && !who.isRidingHorse() && this.tryToReceiveActiveObject(who) || this.controller != null)
          return true;
        this.CurrentDialogue.Push(new StardewValley.Dialogue(this, "Strings\\Characters:Henchman2"));
        Game1.drawDialogue(this);
      }
      return true;
    }
    bool flag1 = who.pantsItem.Value?.QualifiedItemId == "(P)15" && (this.Name == "Lewis" || this.Name == "Marnie");
    if (this.CanReceiveGifts() && friendship == null)
    {
      Game1.player.friendshipData.Add(this.Name, friendship = new Friendship(0));
      if (this.Name.Equals("Krobus"))
      {
        this.CurrentDialogue.Push(new StardewValley.Dialogue(this, "Strings\\StringsFromCSFiles:NPC.cs.3990"));
        Game1.drawDialogue(this);
        return true;
      }
    }
    if (who.NotifyQuests((Func<Quest, bool>) (quest => quest.OnNpcSocialized(this))) && Game1.dialogueUp)
    {
      this.faceTowardFarmerForPeriod(6000, 3, false, who);
      return true;
    }
    if (this.Name.Equals("Krobus") && who.hasQuest("28"))
    {
      this.CurrentDialogue.Push(new StardewValley.Dialogue(this, l is Sewer ? "Strings\\Characters:KrobusDarkTalisman" : "Strings\\Characters:KrobusDarkTalisman_elsewhere"));
      Game1.drawDialogue(this);
      who.removeQuest("28");
      who.mailReceived.Add("krobusUnseal");
      if (l is Sewer)
      {
        DelayedAction.addTemporarySpriteAfterDelay(new TemporaryAnimatedSprite("TileSheets\\Projectiles", new Microsoft.Xna.Framework.Rectangle(0, 0, 16 /*0x10*/, 16 /*0x10*/), 3000f, 1, 0, new Vector2(31f, 17f) * 64f, false, false)
        {
          scale = 4f,
          delayBeforeAnimationStart = 1,
          startSound = "debuffSpell",
          motion = new Vector2(-9f, 1f),
          rotationChange = (float) Math.PI / 64f,
          lightId = "Krobus_Unseal_1",
          lightRadius = 1f,
          lightcolor = new Color(150, 0, 50),
          layerDepth = 1f,
          alphaFade = 3f / 1000f
        }, l, 200, true);
        DelayedAction.addTemporarySpriteAfterDelay(new TemporaryAnimatedSprite("TileSheets\\Projectiles", new Microsoft.Xna.Framework.Rectangle(0, 0, 16 /*0x10*/, 16 /*0x10*/), 3000f, 1, 0, new Vector2(31f, 17f) * 64f, false, false)
        {
          startSound = "debuffSpell",
          delayBeforeAnimationStart = 1,
          scale = 4f,
          motion = new Vector2(-9f, 1f),
          rotationChange = (float) Math.PI / 64f,
          lightId = "Krobus_Unseal_2",
          lightRadius = 1f,
          lightcolor = new Color(150, 0, 50),
          layerDepth = 1f,
          alphaFade = 3f / 1000f
        }, l, 700, true);
      }
      return true;
    }
    if (this.name.Value == "Jas" && this.currentLocation is Desert && who.mailReceived.Contains("Jas_IceCream_DF_" + Game1.year.ToString()))
    {
      this.doEmote(32 /*0x20*/);
      return true;
    }
    if (this.Name == who.spouse && who.IsLocalPlayer && this.Sprite.CurrentAnimation == null)
    {
      this.faceDirection(-3);
      if (friendship != null && friendship.Points >= 3125 && who.mailReceived.Add("CF_Spouse"))
      {
        this.CurrentDialogue.Push(this.TryGetDialogue("SpouseStardrop") ?? new StardewValley.Dialogue(this, "Strings\\StringsFromCSFiles:NPC.cs.4001"));
        Object @object = ItemRegistry.Create<Object>("(O)434");
        @object.CanBeSetDown = false;
        @object.CanBeGrabbed = false;
        Game1.player.addItemByMenuIfNecessary((Item) @object);
        this.shouldSayMarriageDialogue.Value = false;
        this.currentMarriageDialogue.Clear();
        return true;
      }
      if (!this.hasTemporaryMessageAvailable() && this.currentMarriageDialogue.Count == 0 && this.CurrentDialogue.Count == 0 && Game1.timeOfDay < 2200 && !this.isMoving() && who.ActiveObject == null)
      {
        if (this.faceTowardFarmerTimer <= 0)
          this.facingDirectionBeforeSpeakingToPlayer.Value = this.FacingDirection;
        this.faceGeneralDirection(who.getStandingPosition(), 0, false, false);
        who.faceGeneralDirection(this.getStandingPosition(), 0, false, false);
        if (this.FacingDirection == 3 || this.FacingDirection == 1)
        {
          CharacterData data = this.GetData();
          int frame = data != null ? data.KissSpriteIndex : 28;
          bool flag2 = data == null || data.KissSpriteFacingRight;
          bool flip = flag2 != (this.FacingDirection == 1);
          if (who.getFriendshipHeartLevelForNPC(this.Name) > 9 && this.sleptInBed.Value)
          {
            int milliseconds = Game1.IsMultiplayer ? 1000 : 10;
            this.movementPause = milliseconds;
            this.faceTowardFarmerForPeriod(3000, 3, false, who);
            this.Sprite.ClearAnimation();
            this.Sprite.AddFrame(new FarmerSprite.AnimationFrame(frame, milliseconds, false, flip, new AnimatedSprite.endOfAnimationBehavior(this.haltMe), true));
            if (!this.hasBeenKissedToday.Value)
            {
              who.changeFriendship(10, this);
              if (who.hasCurrentOrPendingRoommate())
                Game1.multiplayer.broadcastSprites(who.currentLocation, new TemporaryAnimatedSprite("LooseSprites\\emojis", new Microsoft.Xna.Framework.Rectangle(0, 0, 9, 9), 2000f, 1, 0, this.Tile * 64f + new Vector2(16f, -64f), false, false, 1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
                {
                  motion = new Vector2(0.0f, -0.5f),
                  alphaFade = 0.01f
                });
              else
                Game1.multiplayer.broadcastSprites(who.currentLocation, new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(211, 428, 7, 6), 2000f, 1, 0, this.Tile * 64f + new Vector2(16f, -64f), false, false, 1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
                {
                  motion = new Vector2(0.0f, -0.5f),
                  alphaFade = 0.01f
                });
              l.playSound("dwop", context: SoundContext.NPC);
              who.exhausted.Value = false;
            }
            else if (Game1.random.NextDouble() < 0.1)
              this.doEmote(20);
            this.hasBeenKissedToday.Value = true;
            this.Sprite.UpdateSourceRect();
          }
          else
          {
            this.faceDirection(Game1.random.Choose<int>(2, 0));
            this.doEmote(12);
          }
          int facingDirection = 1;
          if (flag2 && !flip || !flag2 & flip)
            facingDirection = 3;
          who.PerformKiss(facingDirection);
          return true;
        }
        if (this.faceTowardFarmerTimer <= 0 && Game1.random.NextDouble() < 0.1)
        {
          Game1.playSound("dwop");
          if (who.getFriendshipHeartLevelForNPC(this.Name) > 9)
            this.doEmote(Game1.random.NextDouble() < 0.5 ? 32 /*0x20*/ : 20);
          else if (who.getFriendshipHeartLevelForNPC(this.Name) > 7)
            this.doEmote(Game1.random.NextDouble() < 0.5 ? 40 : 8);
          else
            this.doEmote(Game1.random.NextDouble() < 0.5 ? 28 : 12);
        }
        else if (this.facingDirectionBeforeSpeakingToPlayer.Value >= 0 && Math.Abs(this.facingDirectionBeforeSpeakingToPlayer.Value - this.FacingDirection) == 2 && Game1.random.NextDouble() < 0.1)
        {
          this.jump();
          this.doEmote(16 /*0x10*/);
        }
        this.faceTowardFarmerForPeriod(3000, 4, false, who);
      }
    }
    if (this.SimpleNonVillagerNPC)
    {
      if (this.name.Value == "Fizz")
      {
        int perfectionWaivers = Game1.netWorldState.Value.PerfectionWaivers;
        if ((double) Utility.percentGameComplete() + (double) perfectionWaivers * 0.0099999997764825821 >= 1.0)
        {
          this.doEmote(56);
          this.shakeTimer = 250;
        }
        else
        {
          this.CurrentDialogue.Clear();
          if (!Game1.player.mailReceived.Contains("FizzFirstDialogue"))
          {
            Game1.player.mailReceived.Add("FizzFirstDialogue");
            this.CurrentDialogue.Push(new StardewValley.Dialogue(this, "Strings\\1_6_Strings:Fizz_Intro_1"));
            Game1.drawDialogue(this);
          }
          else
          {
            this.CurrentDialogue.Push(new StardewValley.Dialogue(this, "Strings\\1_6_Strings:Fizz_Intro_2"));
            Game1.drawDialogue(this);
            Game1.afterDialogues = (Game1.afterFadeFunction) (() => Game1.currentLocation.createQuestionDialogue("", new Response[2]
            {
              new Response("Yes", Game1.content.LoadString("Strings\\1_6_Strings:Fizz_Yes")).SetHotKey(Keys.Y),
              new Response("No", Game1.content.LoadString("Strings\\Lexicon:QuestionDialogue_No")).SetHotKey(Keys.Escape)
            }, "Fizz"));
          }
        }
      }
      else
      {
        string path = "Strings\\SimpleNonVillagerDialogues:" + this.Name;
        string str = Game1.content.LoadString(path);
        if (str != path)
        {
          string[] strArray = str.Split("||");
          if (this.nonVillagerNPCTimesTalked != -1 && this.nonVillagerNPCTimesTalked < strArray.Length)
          {
            Game1.drawObjectDialogue(strArray[this.nonVillagerNPCTimesTalked]);
            ++this.nonVillagerNPCTimesTalked;
            if (this.nonVillagerNPCTimesTalked >= strArray.Length)
              this.nonVillagerNPCTimesTalked = -1;
          }
        }
      }
      return true;
    }
    bool flag3 = false;
    if (friendship != null)
    {
      if (this.getSpouse() == Game1.player && this.shouldSayMarriageDialogue.Value && this.currentMarriageDialogue.Count > 0 && this.currentMarriageDialogue.Count > 0)
      {
        while (this.currentMarriageDialogue.Count > 0)
        {
          MarriageDialogueReference dialogueReference = this.currentMarriageDialogue[this.currentMarriageDialogue.Count - 1];
          if (dialogueReference == this.marriageDefaultDialogue.Value)
            this.marriageDefaultDialogue.Value = (MarriageDialogueReference) null;
          this.currentMarriageDialogue.RemoveAt(this.currentMarriageDialogue.Count - 1);
          this.CurrentDialogue.Push(dialogueReference.GetDialogue(this));
        }
        flag3 = true;
      }
      if (!flag3)
      {
        flag3 = this.checkForNewCurrentDialogue(friendship.Points / 250);
        if (!flag3)
          flag3 = this.checkForNewCurrentDialogue(friendship.Points / 250, true);
      }
    }
    if (who.IsLocalPlayer && friendship != null && (this.endOfRouteMessage.Value != null | flag3 || this.currentLocation != null && this.currentLocation.HasLocationOverrideDialogue(this)))
    {
      if (!flag3 && this.setTemporaryMessages(who))
      {
        who.NotifyQuests((Func<Quest, bool>) (quest => quest.OnNpcSocialized(this)));
        return false;
      }
      Texture2D texture = this.Sprite.Texture;
      if ((texture != null ? (texture.Bounds.Height > 32 /*0x20*/ ? 1 : 0) : 0) != 0 && (this.CurrentDialogue.Count <= 0 || !this.CurrentDialogue.Peek().dontFaceFarmer))
        this.faceTowardFarmerForPeriod(5000, 4, false, who);
      if (who.ActiveObject != null && !who.isRidingHorse() && this.tryToReceiveActiveObject(who))
      {
        who.NotifyQuests((Func<Quest, bool>) (quest => quest.OnNpcSocialized(this)));
        this.faceTowardFarmerForPeriod(3000, 4, false, who);
        return true;
      }
      this.grantConversationFriendship(who);
      Game1.drawDialogue(this);
      return true;
    }
    if (this.canTalk() && who.hasClubCard && this.Name.Equals("Bouncer") && who.IsLocalPlayer)
    {
      Response[] answerChoices = new Response[2]
      {
        new Response("Yes.", Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4018")),
        new Response("That's", Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4020"))
      };
      l.createQuestionDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4021"), answerChoices, "ClubCard");
    }
    else if (this.canTalk() && this.CurrentDialogue.Count > 0)
    {
      if (who.ActiveObject != null && !who.isRidingHorse() && this.tryToReceiveActiveObject(who, true))
      {
        if (who.IsLocalPlayer)
          this.tryToReceiveActiveObject(who);
        else
          this.faceTowardFarmerForPeriod(3000, 4, false, who);
        return true;
      }
      if (this.CurrentDialogue.Count >= 1 || this.endOfRouteMessage.Value != null || this.currentLocation != null && this.currentLocation.HasLocationOverrideDialogue(this))
      {
        if (this.setTemporaryMessages(who))
        {
          who.NotifyQuests((Func<Quest, bool>) (quest => quest.OnNpcSocialized(this)));
          return false;
        }
        Texture2D texture = this.Sprite.Texture;
        if ((texture != null ? (texture.Bounds.Height > 32 /*0x20*/ ? 1 : 0) : 0) != 0 && !this.CurrentDialogue.Peek().dontFaceFarmer)
          this.faceTowardFarmerForPeriod(5000, 4, false, who);
        if (who.IsLocalPlayer)
        {
          this.grantConversationFriendship(who);
          if (!flag1)
          {
            Game1.drawDialogue(this);
            return true;
          }
        }
      }
      else if (!this.doingEndOfRouteAnimation.Value)
      {
        try
        {
          if (friendship != null)
            this.faceTowardFarmerForPeriod(friendship.Points / 125 * 1000 + 1000, 4, false, who);
        }
        catch (Exception ex)
        {
        }
        if (Game1.random.NextDouble() < 0.1)
          this.doEmote(8);
      }
    }
    else if (this.canTalk() && !Game1.game1.wasAskedLeoMemory && Game1.CurrentEvent == null && this.name.Value == "Leo" && this.currentLocation != null && (this.currentLocation.NameOrUniqueName == "LeoTreeHouse" || this.currentLocation.NameOrUniqueName == "Mountain") && Game1.MasterPlayer.hasOrWillReceiveMail("leoMoved") && this.GetUnseenLeoEvent().HasValue && this.CanRevisitLeoMemory(this.GetUnseenLeoEvent()))
    {
      Game1.DrawDialogue(this, "Strings\\Characters:Leo_Memory");
      Game1.afterDialogues += new Game1.afterFadeFunction(this.AskLeoMemoryPrompt);
    }
    else
    {
      if (who.ActiveObject != null && !who.isRidingHorse() && this.tryToReceiveActiveObject(who))
      {
        this.faceTowardFarmerForPeriod(3000, 4, false, who);
        return true;
      }
      switch (this.Name)
      {
        case "Krobus":
          if (l is Sewer)
          {
            Utility.TryOpenShopMenu("ShadowShop", "Krobus");
            return true;
          }
          break;
        case "Dwarf":
          if (who.canUnderstandDwarves && l is Mine)
          {
            Utility.TryOpenShopMenu("Dwarf", this.Name);
            return true;
          }
          break;
      }
    }
    if (flag1)
    {
      if ((double) this.yJumpVelocity != 0.0 || this.Sprite.CurrentAnimation != null)
        return true;
      switch (this.Name)
      {
        case "Lewis":
          this.faceTowardFarmerForPeriod(1000, 3, false, who);
          this.jump();
          this.Sprite.ClearAnimation();
          this.Sprite.AddFrame(new FarmerSprite.AnimationFrame(26, 1000, false, false, (AnimatedSprite.endOfAnimationBehavior) (x => this.doEmote(12)), true));
          this.Sprite.loop = false;
          this.shakeTimer = 1000;
          l.playSound("batScreech");
          break;
        case "Marnie":
          this.faceTowardFarmerForPeriod(1000, 3, false, who);
          this.Sprite.ClearAnimation();
          this.Sprite.AddFrame(new FarmerSprite.AnimationFrame(33, 150, false, false, (AnimatedSprite.endOfAnimationBehavior) (x => l.playSound("dustMeep"))));
          this.Sprite.AddFrame(new FarmerSprite.AnimationFrame(34, 180));
          this.Sprite.AddFrame(new FarmerSprite.AnimationFrame(33, 180, false, false, (AnimatedSprite.endOfAnimationBehavior) (x => l.playSound("dustMeep"))));
          this.Sprite.AddFrame(new FarmerSprite.AnimationFrame(34, 180));
          this.Sprite.AddFrame(new FarmerSprite.AnimationFrame(33, 180, false, false, (AnimatedSprite.endOfAnimationBehavior) (x => l.playSound("dustMeep"))));
          this.Sprite.AddFrame(new FarmerSprite.AnimationFrame(34, 180));
          this.Sprite.AddFrame(new FarmerSprite.AnimationFrame(33, 180, false, false, (AnimatedSprite.endOfAnimationBehavior) (x => l.playSound("dustMeep"))));
          this.Sprite.AddFrame(new FarmerSprite.AnimationFrame(34, 180));
          this.Sprite.loop = false;
          break;
      }
      return true;
    }
    if (this.setTemporaryMessages(who) || !this.doingEndOfRouteAnimation.Value && this.goingToDoEndOfRouteAnimation.Value || this.endOfRouteMessage.Value == null)
      return false;
    Game1.drawDialogue(this);
    return true;
  }

  public void grantConversationFriendship(Farmer who, int amount = 20)
  {
    Friendship friendship;
    if (who.hasPlayerTalkedToNPC(this.Name) || !who.friendshipData.TryGetValue(this.Name, out friendship))
      return;
    friendship.TalkedToToday = true;
    who.NotifyQuests((Func<Quest, bool>) (quest => quest.OnNpcSocialized(this)));
    if (this.isDivorcedFrom(who))
      return;
    if (who.hasBuff("statue_of_blessings_4"))
      amount = 60;
    who.changeFriendship(amount, this);
  }

  public virtual void AskLeoMemoryPrompt()
  {
    GameLocation currentLocation = this.currentLocation;
    Response[] answerChoices = new Response[2]
    {
      new Response("Yes", Game1.content.LoadString("Strings\\Characters:Leo_Memory_Answer_Yes")),
      new Response("No", Game1.content.LoadString("Strings\\Characters:Leo_Memory_Answer_No"))
    };
    string question = Game1.content.LoadStringReturnNullIfNotFound("Strings\\Characters:Leo_Memory_" + this.GetUnseenLeoEvent().Value.Value) ?? "";
    currentLocation.createQuestionDialogue(question, answerChoices, new GameLocation.afterQuestionBehavior(this.OnLeoMemoryResponse), this);
  }

  public bool CanRevisitLeoMemory(KeyValuePair<string, string>? event_data)
  {
    if (!event_data.HasValue)
      return false;
    string key1 = event_data.Value.Key;
    string str1 = event_data.Value.Value;
    Dictionary<string, string> dictionary;
    try
    {
      dictionary = Game1.content.Load<Dictionary<string, string>>("Data\\Events\\" + key1);
    }
    catch
    {
      return false;
    }
    if (dictionary == null)
      return false;
    foreach (string key2 in dictionary.Keys)
    {
      if (Event.SplitPreconditions(key2)[0] == str1)
      {
        GameLocation locationFromName = Game1.getLocationFromName(key1);
        string str2 = locationFromName?.checkEventPrecondition(key2.Replace("/e 1039573", "").Replace("/Hl leoMoved", ""));
        if (locationFromName != null && string.IsNullOrEmpty(str2) && str2 != "-1")
          return true;
      }
    }
    return false;
  }

  public KeyValuePair<string, string>? GetUnseenLeoEvent()
  {
    if (!Game1.player.eventsSeen.Contains("6497423"))
      return new KeyValuePair<string, string>?(new KeyValuePair<string, string>("IslandWest", "6497423"));
    if (!Game1.player.eventsSeen.Contains("6497421"))
      return new KeyValuePair<string, string>?(new KeyValuePair<string, string>("IslandNorth", "6497421"));
    return !Game1.player.eventsSeen.Contains("6497428") ? new KeyValuePair<string, string>?(new KeyValuePair<string, string>("IslandSouth", "6497428")) : new KeyValuePair<string, string>?();
  }

  public void OnLeoMemoryResponse(Farmer who, string whichAnswer)
  {
    if (whichAnswer.EqualsIgnoreCase("yes"))
    {
      KeyValuePair<string, string>? unseenLeoEvent = this.GetUnseenLeoEvent();
      if (!unseenLeoEvent.HasValue)
        return;
      KeyValuePair<string, string> keyValuePair = unseenLeoEvent.Value;
      string key1 = keyValuePair.Key;
      keyValuePair = unseenLeoEvent.Value;
      string str = keyValuePair.Value;
      string eventAssetName = "Data\\Events\\" + key1;
      Dictionary<string, string> location_events;
      try
      {
        location_events = Game1.content.Load<Dictionary<string, string>>(eventAssetName);
      }
      catch
      {
        return;
      }
      if (location_events == null)
        return;
      Point oldTile = Game1.player.TilePoint;
      string oldLocation = Game1.player.currentLocation.NameOrUniqueName;
      int oldDirection = Game1.player.FacingDirection;
      foreach (string key2 in location_events.Keys)
      {
        string key = key2;
        if (Event.SplitPreconditions(key)[0] == str)
        {
          LocationRequest location_request = Game1.getLocationRequest(key1);
          Game1.warpingForForcedRemoteEvent = true;
          location_request.OnWarp += (LocationRequest.Callback) (() =>
          {
            Event evt = new Event(location_events[key], eventAssetName, "event_id");
            evt.isMemory = true;
            evt.setExitLocation(oldLocation, oldTile.X, oldTile.Y);
            Game1.player.orientationBeforeEvent = oldDirection;
            location_request.Location.currentEvent = evt;
            location_request.Location.startEvent(evt);
            Game1.warpingForForcedRemoteEvent = false;
          });
          int x = 8;
          int y = 8;
          Utility.getDefaultWarpLocation(location_request.Name, ref x, ref y);
          Game1.warpFarmer(location_request, x, y, Game1.player.FacingDirection);
        }
      }
    }
    else
      Game1.game1.wasAskedLeoMemory = true;
  }

  public bool isDivorcedFrom(Farmer who) => NPC.IsDivorcedFrom(who, this.Name);

  public static bool IsDivorcedFrom(Farmer player, string npcName)
  {
    Friendship friendship;
    return player != null && player.friendshipData.TryGetValue(npcName, out friendship) && friendship.IsDivorced();
  }

  public override void MovePosition(
    GameTime time,
    xTile.Dimensions.Rectangle viewport,
    GameLocation currentLocation)
  {
    if (this.movementPause > 0)
      return;
    this.faceTowardFarmerTimer = 0;
    base.MovePosition(time, viewport, currentLocation);
  }

  public GameLocation getHome()
  {
    return this.isMarried() && this.getSpouse() != null ? (GameLocation) Utility.getHomeOfFarmer(this.getSpouse()) : Game1.RequireLocation(this.defaultMap.Value);
  }

  public override bool canPassThroughActionTiles() => true;

  public virtual void behaviorOnFarmerPushing()
  {
  }

  public virtual void behaviorOnFarmerLocationEntry(GameLocation location, Farmer who)
  {
    if (this.Sprite == null || this.Sprite.CurrentAnimation != null || this.Sprite.SourceRect.Height <= 32 /*0x20*/ || this.SimpleNonVillagerNPC)
      return;
    this.Sprite.SpriteWidth = 16 /*0x10*/;
    this.Sprite.SpriteHeight = 16 /*0x10*/;
    this.Sprite.currentFrame = 0;
  }

  public virtual void behaviorOnLocalFarmerLocationEntry(GameLocation location)
  {
    this.shouldPlayRobinHammerAnimation.CancelInterpolation();
    this.shouldPlaySpousePatioAnimation.CancelInterpolation();
    this.shouldWearIslandAttire.CancelInterpolation();
    this.isSleeping.CancelInterpolation();
    this.doingEndOfRouteAnimation.CancelInterpolation();
    this._skipRouteEndIntro = this.doingEndOfRouteAnimation.Value;
    this.endOfRouteBehaviorName.CancelInterpolation();
    if (!this.isSleeping.Value)
      return;
    this.position.Field.CancelInterpolation();
  }

  public override void updateMovement(GameLocation location, GameTime time)
  {
    this.lastPosition = this.Position;
    if (this.DirectionsToNewLocation != null && !Game1.newDay)
    {
      Point standingPixel = this.StandingPixel;
      if (standingPixel.X < -64 || standingPixel.X > location.map.DisplayWidth + 64 /*0x40*/ || standingPixel.Y < -64 || standingPixel.Y > location.map.DisplayHeight + 64 /*0x40*/)
      {
        this.IsWalkingInSquare = false;
        Game1.warpCharacter(this, this.DefaultMap, this.DefaultPosition);
        location.characters.Remove(this);
      }
      else
      {
        if (!this.IsWalkingInSquare)
          return;
        this.returnToEndPoint();
        this.MovePosition(time, Game1.viewport, location);
      }
    }
    else
    {
      if (!this.IsWalkingInSquare)
        return;
      this.randomSquareMovement(time);
      this.MovePosition(time, Game1.viewport, location);
    }
  }

  public void facePlayer(Farmer who)
  {
    if (this.facingDirectionBeforeSpeakingToPlayer.Value == -1)
      this.facingDirectionBeforeSpeakingToPlayer.Value = this.getFacingDirection();
    this.faceDirection((who.FacingDirection + 2) % 4);
  }

  public void doneFacingPlayer(Farmer who)
  {
  }

  public override void update(GameTime time, GameLocation location)
  {
    if (this.AllowDynamicAppearance && this.currentLocation != null && this.currentLocation.NameOrUniqueName != this.LastLocationNameForAppearance)
      this.ChooseAppearance();
    if (Game1.IsMasterGame && (double) this.currentScheduleDelay > 0.0)
    {
      this.currentScheduleDelay -= (float) time.ElapsedGameTime.TotalSeconds;
      if ((double) this.currentScheduleDelay <= 0.0)
      {
        this.currentScheduleDelay = -1f;
        this.checkSchedule(Game1.timeOfDay);
        this.currentScheduleDelay = 0.0f;
      }
    }
    this.removeHenchmanEvent.Poll();
    if (Game1.IsMasterGame && this.shouldWearIslandAttire.Value && (this.currentLocation == null || this.currentLocation.InValleyContext()))
      this.shouldWearIslandAttire.Value = false;
    if (this._startedEndOfRouteBehavior == null && this._finishingEndOfRouteBehavior == null && this.loadedEndOfRouteBehavior != this.endOfRouteBehaviorName.Value)
      this.loadEndOfRouteBehavior(this.endOfRouteBehaviorName.Value);
    if (this.doingEndOfRouteAnimation.Value != this.currentlyDoingEndOfRouteAnimation)
    {
      if (!this.currentlyDoingEndOfRouteAnimation)
      {
        if (string.Equals(this.loadedEndOfRouteBehavior, this.endOfRouteBehaviorName.Value, StringComparison.Ordinal))
          this.reallyDoAnimationAtEndOfScheduleRoute();
      }
      else
        this.finishEndOfRouteAnimation();
      this.currentlyDoingEndOfRouteAnimation = this.doingEndOfRouteAnimation.Value;
    }
    if (this.shouldWearIslandAttire.Value != this.isWearingIslandAttire)
    {
      if (!this.isWearingIslandAttire)
        this.wearIslandAttire();
      else
        this.wearNormalClothes();
    }
    if (this.isSleeping.Value != this.isPlayingSleepingAnimation)
    {
      if (!this.isPlayingSleepingAnimation)
      {
        this.playSleepingAnimation();
      }
      else
      {
        this.Sprite.StopAnimation();
        this.isPlayingSleepingAnimation = false;
      }
    }
    if (this.shouldPlayRobinHammerAnimation.Value != this.isPlayingRobinHammerAnimation)
    {
      if (!this.isPlayingRobinHammerAnimation)
      {
        this.doPlayRobinHammerAnimation();
        this.isPlayingRobinHammerAnimation = true;
      }
      else
      {
        this.Sprite.StopAnimation();
        this.isPlayingRobinHammerAnimation = false;
      }
    }
    if (this.shouldPlaySpousePatioAnimation.Value != this.isPlayingSpousePatioAnimation)
    {
      if (!this.isPlayingSpousePatioAnimation)
      {
        this.doPlaySpousePatioAnimation();
        this.isPlayingSpousePatioAnimation = true;
      }
      else
      {
        this.Sprite.StopAnimation();
        this.isPlayingSpousePatioAnimation = false;
      }
    }
    if (this.returningToEndPoint)
    {
      this.returnToEndPoint();
      this.MovePosition(time, Game1.viewport, location);
    }
    else if (this.temporaryController != null)
    {
      if (this.temporaryController.update(time))
      {
        int num = this.temporaryController.NPCSchedule ? 1 : 0;
        this.temporaryController = (PathFindController) null;
        if (num != 0)
        {
          this.currentScheduleDelay = -1f;
          this.checkSchedule(Game1.timeOfDay);
          this.currentScheduleDelay = 0.0f;
        }
      }
      this.updateEmote(time);
    }
    else
      base.update(time, location);
    TimeSpan timeSpan;
    if (this.textAboveHeadTimer > 0)
    {
      if (this.textAboveHeadPreTimer > 0)
      {
        int aboveHeadPreTimer = this.textAboveHeadPreTimer;
        timeSpan = time.ElapsedGameTime;
        int milliseconds = timeSpan.Milliseconds;
        this.textAboveHeadPreTimer = aboveHeadPreTimer - milliseconds;
      }
      else
      {
        this.textAboveHeadTimer -= time.ElapsedGameTime.Milliseconds;
        this.textAboveHeadAlpha = this.textAboveHeadTimer <= 500 ? Math.Max(0.0f, this.textAboveHeadAlpha - 0.04f) : Math.Min(1f, this.textAboveHeadAlpha + 0.1f);
      }
    }
    if (this.isWalkingInSquare && !this.returningToEndPoint)
      this.randomSquareMovement(time);
    if (this.Sprite?.CurrentAnimation != null && !Game1.eventUp && Game1.IsMasterGame && this.Sprite.animateOnce(time))
      this.Sprite.CurrentAnimation = (List<FarmerSprite.AnimationFrame>) null;
    if (this.movementPause > 0 && (!Game1.dialogueUp || this.controller != null))
    {
      this.freezeMotion = true;
      int movementPause = this.movementPause;
      timeSpan = time.ElapsedGameTime;
      int milliseconds = timeSpan.Milliseconds;
      this.movementPause = movementPause - milliseconds;
      if (this.movementPause <= 0)
        this.freezeMotion = false;
    }
    if (this.shakeTimer > 0)
    {
      int shakeTimer = this.shakeTimer;
      timeSpan = time.ElapsedGameTime;
      int milliseconds = timeSpan.Milliseconds;
      this.shakeTimer = shakeTimer - milliseconds;
    }
    if (this.lastPosition.Equals(this.Position))
    {
      double sinceLastMovement = (double) this.timerSinceLastMovement;
      timeSpan = time.ElapsedGameTime;
      double milliseconds = (double) timeSpan.Milliseconds;
      this.timerSinceLastMovement = (float) (sinceLastMovement + milliseconds);
    }
    else
      this.timerSinceLastMovement = 0.0f;
    if (this.swimming.Value)
    {
      timeSpan = time.TotalGameTime;
      this.yOffset = (float) (Math.Cos(timeSpan.TotalMilliseconds / 2000.0) * 4.0);
      float swimTimer1 = this.swimTimer;
      double swimTimer2 = (double) this.swimTimer;
      timeSpan = time.ElapsedGameTime;
      double milliseconds = (double) timeSpan.Milliseconds;
      this.swimTimer = (float) (swimTimer2 - milliseconds);
      if ((double) this.timerSinceLastMovement == 0.0)
      {
        if ((double) swimTimer1 > 400.0 && (double) this.swimTimer <= 400.0 && location.Equals(Game1.currentLocation))
        {
          Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(0, 0, 64 /*0x40*/, 64 /*0x40*/), (float) (150.0 - ((double) Math.Abs(this.xVelocity) + (double) Math.Abs(this.yVelocity)) * 3.0), 8, 0, new Vector2(this.Position.X, (float) (this.StandingPixel.Y - 32 /*0x20*/)), false, Game1.random.NextBool(), 0.01f, 0.01f, Color.White, 1f, 3f / 1000f, 0.0f, 0.0f));
          location.playSound("slosh", context: SoundContext.NPC);
        }
        if ((double) this.swimTimer < 0.0)
        {
          this.swimTimer = 800f;
          if (location.Equals(Game1.currentLocation))
          {
            location.playSound("slosh", context: SoundContext.NPC);
            Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(0, 0, 64 /*0x40*/, 64 /*0x40*/), (float) (150.0 - ((double) Math.Abs(this.xVelocity) + (double) Math.Abs(this.yVelocity)) * 3.0), 8, 0, new Vector2(this.Position.X, (float) (this.StandingPixel.Y - 32 /*0x20*/)), false, Game1.random.NextBool(), 0.01f, 0.01f, Color.White, 1f, 3f / 1000f, 0.0f, 0.0f));
          }
        }
      }
      else if ((double) this.swimTimer < 0.0)
        this.swimTimer = 100f;
    }
    if (!Game1.IsMasterGame)
      return;
    this.isMovingOnPathFindPath.Value = this.controller != null && this.temporaryController != null;
  }

  public virtual void wearIslandAttire()
  {
    this.isWearingIslandAttire = true;
    this.ChooseAppearance();
  }

  public virtual void wearNormalClothes()
  {
    this.isWearingIslandAttire = false;
    this.ChooseAppearance();
  }

  /// <summary>Runs NPC update logic on ten in-game minute intervals (e.g. greeting players or other NPCs)</summary>
  /// <param name="timeOfDay">The new in-game time.</param>
  /// <param name="location">The location where the update is occurring.</param>
  public virtual void performTenMinuteUpdate(int timeOfDay, GameLocation location)
  {
    if (Game1.eventUp || location == null)
      return;
    string str;
    if (Game1.random.NextDouble() < 0.1 && this.Dialogue != null && this.Dialogue.TryGetValue(location.Name + "_Ambient", out str))
    {
      CharacterData data = this.GetData();
      if ((data != null ? (data.CanGreetNearbyCharacters ? 1 : 0) : 1) != 0)
      {
        string[] strArray = str.Split('/');
        int preTimer = Game1.random.Next(4) * 1000;
        this.showTextAboveHead(Game1.random.Choose<string>(strArray), preTimer: preTimer);
        return;
      }
    }
    if (!this.isMoving() || !location.IsOutdoors || timeOfDay >= 1800 || Game1.random.NextDouble() >= 0.3 + (this.SocialAnxiety == 0 ? 0.25 : (this.SocialAnxiety == 1 ? (this.Manners == 2 ? -1.0 : -0.2) : 0.0)) || this.Age == 1 && (this.Manners != 1 || this.SocialAnxiety != 0) || this.isMarried())
      return;
    CharacterData data1 = this.GetData();
    if (data1 == null || !data1.CanGreetNearbyCharacters)
      return;
    Character c = Utility.isThereAFarmerOrCharacterWithinDistance(this.Tile, 4, location);
    if (c == null || c.Name == this.Name)
      return;
    int num;
    switch (c)
    {
      case Horse _:
        return;
      case NPC npc1:
        bool? nearbyCharacters = npc1.GetData()?.CanGreetNearbyCharacters;
        bool flag = false;
        num = nearbyCharacters.GetValueOrDefault() == flag & nearbyCharacters.HasValue ? 1 : 0;
        break;
      default:
        num = 0;
        break;
    }
    if (num != 0 || (c is NPC npc2 ? (npc2.SimpleNonVillagerNPC ? 1 : 0) : 0) != 0)
      return;
    Dictionary<string, string> friendsAndFamily = data1.FriendsAndFamily;
    // ISSUE: explicit non-virtual call
    if ((friendsAndFamily != null ? (!__nonvirtual (friendsAndFamily.ContainsKey(c.Name)) ? 1 : 0) : 1) == 0 || !this.isFacingToward(c.Tile))
      return;
    this.sayHiTo(c);
  }

  public void sayHiTo(Character c)
  {
    if (this.getHi(c.displayName) == null)
      return;
    this.showTextAboveHead(this.getHi(c.displayName));
    if (!(c is NPC npc) || Game1.random.NextDouble() >= 0.66 || npc.getHi(this.displayName) == null)
      return;
    npc.showTextAboveHead(npc.getHi(this.displayName), preTimer: 1000 + Game1.random.Next(500));
  }

  public string getHi(string nameToGreet)
  {
    if (this.Age == 2)
      return this.SocialAnxiety != 1 ? Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4059") : Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4058");
    switch (this.SocialAnxiety)
    {
      case 0:
        if (Game1.random.NextDouble() < 0.33)
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4062");
        return !Game1.random.NextBool() ? Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4068", (object) nameToGreet) : $"{(Game1.timeOfDay < 1200 ? Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4063") : (Game1.timeOfDay < 1700 ? Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4064") : Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4065")))}, {Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4066", (object) nameToGreet)}";
      case 1:
        return Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs." + Game1.random.Choose<string>("4060", "4061"));
      default:
        if (Game1.random.NextDouble() < 0.33)
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4060");
        return !Game1.random.NextBool() ? Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4072") : Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4071", (object) nameToGreet);
    }
  }

  public bool isFacingToward(Vector2 tileLocation)
  {
    switch (this.FacingDirection)
    {
      case 0:
        return (double) this.TilePoint.Y > (double) tileLocation.Y;
      case 1:
        return (double) this.TilePoint.X < (double) tileLocation.X;
      case 2:
        return (double) this.TilePoint.Y < (double) tileLocation.Y;
      case 3:
        return (double) this.TilePoint.X > (double) tileLocation.X;
      default:
        return false;
    }
  }

  public virtual void arriveAt(GameLocation l)
  {
    string str;
    if (Game1.eventUp || !Game1.random.NextBool() || this.Dialogue == null || !this.Dialogue.TryGetValue(l.name.Value + "_Entry", out str))
      return;
    this.showTextAboveHead(Game1.random.Choose<string>(str.Split('/')));
  }

  public override void Halt()
  {
    base.Halt();
    this.shouldPlaySpousePatioAnimation.Value = false;
    this.isPlayingSleepingAnimation = false;
    this.isCharging = false;
    this.speed = 2;
    this.addedSpeed = 0.0f;
    if (!this.isSleeping.Value)
      return;
    this.playSleepingAnimation();
    this.Sprite.UpdateSourceRect();
  }

  public void addExtraDialogue(StardewValley.Dialogue dialogue)
  {
    if (this.updatedDialogueYet)
    {
      if (dialogue == null)
        return;
      this.CurrentDialogue.Push(dialogue);
    }
    else
      this.extraDialogueMessageToAddThisMorning = dialogue;
  }

  public void PerformDivorce()
  {
    this.reloadDefaultLocation();
    Game1.warpCharacter(this, this.defaultMap.Value, this.DefaultPosition / 64f);
  }

  public StardewValley.Dialogue tryToGetMarriageSpecificDialogue(string dialogueKey)
  {
    Dictionary<string, string> dictionary1 = (Dictionary<string, string>) null;
    string assetName1 = (string) null;
    bool flag = false;
    if (this.isRoommate())
    {
      try
      {
        assetName1 = $"Characters\\Dialogue\\MarriageDialogue{this.GetDialogueSheetName()}Roommate";
        Dictionary<string, string> dictionary2 = Game1.content.Load<Dictionary<string, string>>(assetName1);
        if (dictionary2 != null)
        {
          flag = true;
          dictionary1 = dictionary2;
          if (dictionary1 != null)
          {
            string dialogueText;
            if (dictionary1.TryGetValue(dialogueKey, out dialogueText))
              return new StardewValley.Dialogue(this, $"{assetName1}:{dialogueKey}", dialogueText);
          }
        }
      }
      catch (Exception ex)
      {
        assetName1 = (string) null;
      }
    }
    if (!flag)
    {
      try
      {
        assetName1 = "Characters\\Dialogue\\MarriageDialogue" + this.GetDialogueSheetName();
        dictionary1 = Game1.content.Load<Dictionary<string, string>>(assetName1);
      }
      catch (Exception ex)
      {
        assetName1 = (string) null;
      }
    }
    string dialogueText1;
    if (dictionary1 != null && dictionary1.TryGetValue(dialogueKey, out dialogueText1))
      return new StardewValley.Dialogue(this, $"{assetName1}:{dialogueKey}", dialogueText1);
    string assetName2 = "Characters\\Dialogue\\MarriageDialogue";
    Dictionary<string, string> dictionary3 = Game1.content.Load<Dictionary<string, string>>(assetName2);
    if (this.isRoommate())
    {
      string key = dialogueKey + "Roommate";
      string dialogueText2;
      if (dictionary3 != null && dictionary3.TryGetValue(key, out dialogueText2))
        return new StardewValley.Dialogue(this, $"{assetName2}:{dialogueKey}", dialogueText2);
    }
    string dialogueText3;
    return dictionary3 != null && dictionary3.TryGetValue(dialogueKey, out dialogueText3) ? new StardewValley.Dialogue(this, $"{assetName2}:{dialogueKey}", dialogueText3) : (StardewValley.Dialogue) null;
  }

  public void resetCurrentDialogue()
  {
    this.CurrentDialogue = (Stack<StardewValley.Dialogue>) null;
    this.shouldSayMarriageDialogue.Value = false;
    this.currentMarriageDialogue.Clear();
  }

  private Stack<StardewValley.Dialogue> loadCurrentDialogue()
  {
    this.updatedDialogueYet = true;
    Stack<StardewValley.Dialogue> dialogueStack = new Stack<StardewValley.Dialogue>();
    try
    {
      Friendship friendship1;
      int heartLevel = Game1.player.friendshipData.TryGetValue(this.Name, out friendship1) ? friendship1.Points / 250 : 0;
      Random daySaveRandom = Utility.CreateDaySaveRandom((double) (Game1.stats.DaysPlayed * 77U), 2.0 + (double) this.defaultPosition.X * 77.0, (double) this.defaultPosition.Y * 777.0);
      if (this.currentLocation != null && this.currentLocation.IsGreenRainingHere())
      {
        StardewValley.Dialogue dialogue = (StardewValley.Dialogue) null;
        if (Game1.year >= 2)
          dialogue = this.TryGetDialogue("GreenRain_2");
        if (dialogue == null)
          dialogue = this.TryGetDialogue("GreenRain");
        if (dialogue != null)
        {
          dialogueStack.Clear();
          dialogueStack.Push(dialogue);
          return dialogueStack;
        }
      }
      if (daySaveRandom.NextDouble() < 0.025 && heartLevel >= 1)
      {
        CharacterData data1 = this.GetData();
        string key;
        string text;
        if (data1?.FriendsAndFamily != null && Utility.TryGetRandom<string, string>((IDictionary<string, string>) data1.FriendsAndFamily, out key, out text))
        {
          NPC characterFromName = Game1.getCharacterFromName(key);
          string sub1_1 = characterFromName?.displayName ?? NPC.GetDisplayName(key);
          CharacterData data2;
          bool flag = characterFromName != null ? characterFromName.gender.Value == Gender.Male : NPC.TryGetData(key, out data2) && data2.Gender == Gender.Male;
          string str1 = TokenParser.ParseText(text);
          if (string.IsNullOrWhiteSpace(str1))
            str1 = (string) null;
          IDictionary<string, string> npcGiftTastes = Game1.NPCGiftTastes;
          string str2;
          if (npcGiftTastes.TryGetValue(key, out str2))
          {
            string[] array = str2.Split('/');
            string itemId = (string) null;
            string str3 = (string) null;
            string str4;
            if (str1 == null || LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ja)
              str4 = sub1_1;
            else if (!flag)
              str4 = Game1.LoadStringByGender(this.Gender, "Strings\\StringsFromCSFiles:NPC.cs.4080", (object) str1);
            else
              str4 = Game1.LoadStringByGender(this.Gender, "Strings\\StringsFromCSFiles:NPC.cs.4079", (object) str1);
            string sub1_2 = str4;
            string dialogueText = Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4083", (object) sub1_2);
            if (daySaveRandom.NextBool())
            {
              int num = 0;
              string[] strArray = ArgUtility.SplitBySpace(ArgUtility.Get(array, 1));
              for (; (itemId == null || itemId.StartsWith("-")) && num < 30; ++num)
                itemId = daySaveRandom.Choose<string>(strArray);
              if (this.Name == "Penny" && key == "Pam")
              {
                while (itemId == "303" || itemId == "346" || itemId == "348" || itemId == "459")
                  itemId = daySaveRandom.Choose<string>(strArray);
              }
              if (itemId != null)
              {
                ParsedItemData data3 = ItemRegistry.GetData(itemId);
                if (data3 != null)
                {
                  str3 = data3.DisplayName;
                  dialogueText += Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4084", (object) str3);
                  if (this.Age == 2)
                  {
                    dialogueText = Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4086", (object) sub1_2, (object) str3) + (flag ? Game1.LoadStringByGender(this.Gender, "Strings\\StringsFromCSFiles:NPC.cs.4088") : Game1.LoadStringByGender(this.Gender, "Strings\\StringsFromCSFiles:NPC.cs.4089"));
                  }
                  else
                  {
                    switch (daySaveRandom.Next(5))
                    {
                      case 0:
                        dialogueText = Game1.LoadStringByGender(this.Gender, "Strings\\StringsFromCSFiles:NPC.cs.4091", (object) sub1_2, (object) str3);
                        break;
                      case 1:
                        dialogueText = flag ? Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4094", (object) sub1_2, (object) str3) : Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4097", (object) sub1_2, (object) str3);
                        break;
                      case 2:
                        dialogueText = flag ? Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4100", (object) sub1_2, (object) str3) : Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4103", (object) sub1_2, (object) str3);
                        break;
                      case 3:
                        dialogueText = Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4106", (object) sub1_2, (object) str3);
                        break;
                    }
                    if (daySaveRandom.NextDouble() < 0.65)
                    {
                      switch (daySaveRandom.Next(5))
                      {
                        case 0:
                          dialogueText += flag ? Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4109") : Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4111");
                          break;
                        case 1:
                          dialogueText += flag ? (daySaveRandom.NextBool() ? Game1.LoadStringByGender(this.Gender, "Strings\\StringsFromCSFiles:NPC.cs.4113") : Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4114")) : (daySaveRandom.NextBool() ? Game1.LoadStringByGender(this.Gender, "Strings\\StringsFromCSFiles:NPC.cs.4115") : Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4116"));
                          break;
                        case 2:
                          dialogueText += flag ? Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4118") : Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4120");
                          break;
                        case 3:
                          dialogueText += Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4125");
                          break;
                        case 4:
                          dialogueText += flag ? Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4126") : Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4128");
                          break;
                      }
                      if (key.Equals("Abigail") && daySaveRandom.NextBool())
                        dialogueText = Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4128", (object) sub1_1, (object) str3);
                    }
                  }
                }
              }
            }
            else
            {
              string[] source = ArgUtility.SplitBySpace(ArgUtility.Get(array, 7));
              if (((IEnumerable<string>) source).Count<string>() > 0)
              {
                for (int index = 0; (itemId == null || itemId.StartsWith("-")) && index < 30; ++index)
                  itemId = daySaveRandom.Choose<string>(source);
              }
              if (itemId == null)
              {
                for (int index = 0; (itemId == null || itemId.StartsWith("-")) && index < 30; ++index)
                  itemId = daySaveRandom.Choose<string>(ArgUtility.SplitBySpace(npcGiftTastes["Universal_Hate"]));
              }
              if (itemId != null)
              {
                ParsedItemData data4 = ItemRegistry.GetData(itemId);
                if (data4 != null)
                {
                  str3 = data4.DisplayName;
                  dialogueText += flag ? Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4135", (object) str3, (object) Lexicon.getRandomNegativeFoodAdjective()) : Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4138", (object) str3, (object) Lexicon.getRandomNegativeFoodAdjective());
                  if (this.Age == 2)
                  {
                    string str5;
                    if (!flag)
                      str5 = Game1.LoadStringByGender(this.Gender, "Strings\\StringsFromCSFiles:NPC.cs.4144", (object) sub1_1, (object) str3);
                    else
                      str5 = Game1.LoadStringByGender(this.Gender, "Strings\\StringsFromCSFiles:NPC.cs.4141", (object) sub1_1, (object) str3);
                    dialogueText = str5;
                  }
                  else
                  {
                    switch (daySaveRandom.Next(4))
                    {
                      case 0:
                        dialogueText = (daySaveRandom.NextBool() ? Game1.LoadStringByGender(this.Gender, "Strings\\StringsFromCSFiles:NPC.cs.4146") : "") + Game1.LoadStringByGender(this.Gender, "Strings\\StringsFromCSFiles:NPC.cs.4147", (object) sub1_2, (object) str3);
                        break;
                      case 1:
                        string str6;
                        if (!flag)
                        {
                          if (!daySaveRandom.NextBool())
                            str6 = Game1.LoadStringByGender(this.Gender, "Strings\\StringsFromCSFiles:NPC.cs.4154", (object) sub1_2, (object) str3);
                          else
                            str6 = Game1.LoadStringByGender(this.Gender, "Strings\\StringsFromCSFiles:NPC.cs.4153", (object) sub1_2, (object) str3);
                        }
                        else if (!daySaveRandom.NextBool())
                          str6 = Game1.LoadStringByGender(this.Gender, "Strings\\StringsFromCSFiles:NPC.cs.4152", (object) sub1_2, (object) str3);
                        else
                          str6 = Game1.LoadStringByGender(this.Gender, "Strings\\StringsFromCSFiles:NPC.cs.4149", (object) sub1_2, (object) str3);
                        dialogueText = str6;
                        break;
                      case 2:
                        dialogueText = flag ? Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4161", (object) sub1_2, (object) str3) : Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4164", (object) sub1_2, (object) str3);
                        break;
                    }
                    if (daySaveRandom.NextDouble() < 0.65)
                    {
                      switch (daySaveRandom.Next(5))
                      {
                        case 0:
                          dialogueText += Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4170");
                          break;
                        case 1:
                          dialogueText += Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4171");
                          break;
                        case 2:
                          dialogueText += flag ? Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4172") : Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4174");
                          break;
                        case 3:
                          dialogueText += flag ? Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4176") : Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4178");
                          break;
                        case 4:
                          dialogueText += Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4180");
                          break;
                      }
                      if (this.Name.Equals("Lewis") && daySaveRandom.NextBool())
                        dialogueText = Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4182", (object) sub1_1, (object) str3);
                    }
                  }
                }
              }
            }
            if (str3 != null)
            {
              if (Game1.getCharacterFromName(key) != null)
                dialogueText = $"{dialogueText}%revealtaste:{key}:{itemId}";
              dialogueStack.Clear();
              if (dialogueText.Length > 0)
              {
                try
                {
                  dialogueText = dialogueText.Substring(0, 1).ToUpper() + dialogueText.Substring(1, dialogueText.Length - 1);
                }
                catch (Exception ex)
                {
                }
              }
              dialogueStack.Push(new StardewValley.Dialogue(this, (string) null, dialogueText));
              return dialogueStack;
            }
          }
        }
      }
      if (this.Dialogue != null && this.Dialogue.Count != 0)
      {
        dialogueStack.Clear();
        if (Game1.player.spouse != null && Game1.player.spouse == this.Name)
        {
          if (Game1.player.isEngaged())
          {
            Dictionary<string, string> dictionary = Game1.content.Load<Dictionary<string, string>>("Data\\EngagementDialogue");
            if (Game1.player.hasCurrentOrPendingRoommate() && dictionary.ContainsKey(this.Name + "Roommate0"))
              dialogueStack.Push(new StardewValley.Dialogue(this, $"Data\\EngagementDialogue:{this.Name}Roommate{daySaveRandom.Next(2).ToString()}"));
            else if (dictionary.ContainsKey(this.Name + "0"))
              dialogueStack.Push(new StardewValley.Dialogue(this, $"Data\\EngagementDialogue:{this.Name}{daySaveRandom.Next(2).ToString()}"));
          }
          else if (!Game1.newDay && this.marriageDefaultDialogue.Value != null && !this.shouldSayMarriageDialogue.Value)
          {
            dialogueStack.Push(this.marriageDefaultDialogue.Value.GetDialogue(this));
            this.marriageDefaultDialogue.Value = (MarriageDialogueReference) null;
          }
        }
        else
        {
          Friendship friendship2;
          if (Game1.player.friendshipData.TryGetValue(this.Name, out friendship2) && friendship2.IsDivorced())
          {
            StardewValley.Dialogue dialogue = StardewValley.Dialogue.TryGetDialogue(this, $"Characters\\Dialogue\\{this.GetDialogueSheetName()}:divorced");
            if (dialogue != null)
            {
              dialogueStack.Push(dialogue);
              return dialogueStack;
            }
          }
          if (Game1.isRaining && daySaveRandom.NextBool() && (this.currentLocation == null || this.currentLocation.InValleyContext()) && (!this.Name.Equals("Krobus") || !(Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth) == "Fri")) && (!this.Name.Equals("Penny") || !Game1.MasterPlayer.mailReceived.Contains("pamHouseUpgrade")) && (!this.Name.Equals("Emily") || !Game1.IsFall || Game1.dayOfMonth != 15))
          {
            StardewValley.Dialogue dialogue = StardewValley.Dialogue.TryGetDialogue(this, "Characters\\Dialogue\\rainy:" + this.GetDialogueSheetName());
            if (dialogue != null)
            {
              dialogueStack.Push(dialogue);
              return dialogueStack;
            }
          }
          StardewValley.Dialogue dialogue1 = this.tryToRetrieveDialogue(Game1.currentSeason + "_", heartLevel) ?? this.tryToRetrieveDialogue("", heartLevel);
          if (dialogue1 != null)
            dialogueStack.Push(dialogue1);
        }
      }
      else if (this.Name.Equals("Bouncer"))
        dialogueStack.Push(new StardewValley.Dialogue(this, "Strings\\StringsFromCSFiles:NPC.cs.4192"));
      if (this.extraDialogueMessageToAddThisMorning != null)
        dialogueStack.Push(this.extraDialogueMessageToAddThisMorning);
    }
    catch (Exception ex)
    {
      Game1.log.Error($"NPC '{this.Name}' failed loading their current dialogue.", ex);
    }
    return dialogueStack;
  }

  public bool checkForNewCurrentDialogue(int heartLevel, bool noPreface = false)
  {
    if (Game1.IsGreenRainingHere())
      return false;
    foreach (string key in Game1.player.activeDialogueEvents.Keys)
    {
      if (!(key == ""))
      {
        StardewValley.Dialogue dialogue = this.TryGetDialogue(key);
        if (dialogue != null)
        {
          string str = $"{this.Name}_{key}";
          if (dialogue != null && !Game1.player.mailReceived.Contains(str))
          {
            this.CurrentDialogue.Clear();
            this.CurrentDialogue.Push(dialogue);
            if (!key.Contains("dumped"))
              Game1.player.mailReceived.Add(str);
            return true;
          }
        }
      }
    }
    string currentSeason = Game1.season == Season.Spring || noPreface ? "" : Game1.currentSeason;
    string[] strArray = new string[6]
    {
      currentSeason,
      Game1.currentLocation.name.Value,
      "_",
      null,
      null,
      null
    };
    Point tilePoint = this.TilePoint;
    strArray[3] = tilePoint.X.ToString();
    strArray[4] = "_";
    tilePoint = this.TilePoint;
    strArray[5] = tilePoint.Y.ToString();
    StardewValley.Dialogue dialogue1 = this.TryGetDialogue(string.Concat(strArray)) ?? this.TryGetDialogue($"{currentSeason}{Game1.currentLocation.name.Value}_{Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth)}");
    for (int index = 10; dialogue1 == null && index >= 2; index -= 2)
    {
      if (heartLevel >= index)
        dialogue1 = this.TryGetDialogue(currentSeason + Game1.currentLocation.name.Value + index.ToString());
    }
    StardewValley.Dialogue dialogue2 = dialogue1 ?? this.TryGetDialogue(currentSeason + Game1.currentLocation.Name);
    if (dialogue2 == null)
      return false;
    dialogue2.removeOnNextMove = true;
    this.CurrentDialogue.Push(dialogue2);
    return true;
  }

  /// <summary>Try to get a specific dialogue from the loaded <see cref="P:StardewValley.NPC.Dialogue" />.</summary>
  /// <param name="key">The dialogue key.</param>
  /// <returns>Returns the matched dialogue if found, else <c>null</c>.</returns>
  public StardewValley.Dialogue TryGetDialogue(string key)
  {
    Dictionary<string, string> dialogue = this.Dialogue;
    string dialogueText;
    return dialogue != null && dialogue.TryGetValue(key, out dialogueText) ? new StardewValley.Dialogue(this, $"{this.LoadedDialogueKey}:{key}", dialogueText) : (StardewValley.Dialogue) null;
  }

  /// <summary>Try to get a specific dialogue from the loaded <see cref="P:StardewValley.NPC.Dialogue" /> using a matching gift taste tag like 'Positive'.</summary>
  /// <param name="giftTaste">The gift taste, matching a value lke <see cref="F:StardewValley.NPC.gift_taste_love" />.</param>
  /// <param name="key">Get the dialogue key to try, given a gift taste tag like 'Positive'.</param>
  /// <returns>Returns the matched dialogue if found, else <c>null</c>.</returns>
  public StardewValley.Dialogue TryGetDialogueByGiftTaste(
    int giftTaste,
    Func<string, string> getKey)
  {
    switch (giftTaste)
    {
      case 0:
      case 7:
        return this.TryGetDialogue(getKey("Loved")) ?? this.TryGetDialogue(getKey("Positive"));
      case 2:
        return this.TryGetDialogue(getKey("Liked")) ?? this.TryGetDialogue(getKey("Positive"));
      case 4:
        return this.TryGetDialogue(getKey("Disliked")) ?? this.TryGetDialogue(getKey("Negative"));
      case 6:
        return this.TryGetDialogue(getKey("Hated")) ?? this.TryGetDialogue(getKey("Negative"));
      default:
        return this.TryGetDialogue(getKey("Neutral")) ?? this.TryGetDialogue(getKey("Positive"));
    }
  }

  /// <summary>Try to get a specific dialogue from the loaded <see cref="P:StardewValley.NPC.Dialogue" />.</summary>
  /// <param name="key">The dialogue key.</param>
  /// <param name="substitutions">The values with which to replace placeholders like <c>{0}</c> in the loaded text.</param>
  /// <returns>Returns the matched dialogue if found, else <c>null</c>.</returns>
  public StardewValley.Dialogue TryGetDialogue(string key, params object[] substitutions)
  {
    Dictionary<string, string> dialogue = this.Dialogue;
    string format;
    return dialogue != null && dialogue.TryGetValue(key, out format) ? new StardewValley.Dialogue(this, $"{this.LoadedDialogueKey}:{key}", string.Format(format, substitutions)) : (StardewValley.Dialogue) null;
  }

  /// <summary>Try to get a dialogue from the loaded <see cref="P:StardewValley.NPC.Dialogue" />, applying variant rules for roommates, marriage, inlaws, dates, etc.</summary>
  /// <param name="preface">A prefix added to the translation keys to look up.</param>
  /// <param name="heartLevel">The NPC's heart level with the player.</param>
  /// <param name="appendToEnd">A suffix added to the translation keys to look up.</param>
  /// <returns>Returns the best matched dialogue if found, else <c>null</c>.</returns>
  public StardewValley.Dialogue tryToRetrieveDialogue(
    string preface,
    int heartLevel,
    string appendToEnd = "")
  {
    int num = Game1.year;
    if (Game1.year > 2)
      num = 2;
    if (!string.IsNullOrEmpty(Game1.player.spouse) && appendToEnd.Equals(""))
    {
      if (Game1.player.hasCurrentOrPendingRoommate())
      {
        StardewValley.Dialogue retrieveDialogue = this.tryToRetrieveDialogue(preface, heartLevel, "_roommate_" + Game1.player.spouse);
        if (retrieveDialogue != null)
          return retrieveDialogue;
      }
      else
      {
        StardewValley.Dialogue retrieveDialogue = this.tryToRetrieveDialogue(preface, heartLevel, "_inlaw_" + Game1.player.spouse);
        if (retrieveDialogue != null)
          return retrieveDialogue;
      }
    }
    string str = Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth);
    if (num == 1)
    {
      StardewValley.Dialogue dialogue = this.TryGetDialogue(preface + Game1.dayOfMonth.ToString() + appendToEnd);
      if (dialogue != null)
        return dialogue;
    }
    StardewValley.Dialogue dialogue1 = this.TryGetDialogue($"{preface}{Game1.dayOfMonth.ToString()}_{num.ToString()}{appendToEnd}");
    if (dialogue1 != null)
      return dialogue1;
    StardewValley.Dialogue dialogue2 = this.TryGetDialogue($"{preface}{Game1.dayOfMonth.ToString()}_*{appendToEnd}");
    if (dialogue2 != null)
      return dialogue2;
    for (int index = 10; index >= 2; index -= 2)
    {
      if (heartLevel >= index)
      {
        StardewValley.Dialogue retrieveDialogue = this.TryGetDialogue($"{preface}{str}{index.ToString()}_{num.ToString()}{appendToEnd}") ?? this.TryGetDialogue(preface + str + index.ToString() + appendToEnd);
        if (retrieveDialogue != null)
        {
          if (index != 4 || !(preface == "fall_") || !(str == "Mon") || !this.Name.Equals("Penny") || !Game1.MasterPlayer.mailReceived.Contains("pamHouseUpgrade"))
            return retrieveDialogue;
          return this.TryGetDialogue($"{preface}{str}_{num.ToString()}{appendToEnd}") ?? this.TryGetDialogue("fall_Mon");
        }
      }
    }
    StardewValley.Dialogue dialogue3 = this.TryGetDialogue(preface + str + appendToEnd);
    if (dialogue3 != null)
    {
      StardewValley.Dialogue dialogue4 = this.TryGetDialogue($"{preface}{str}_{num.ToString()}{appendToEnd}");
      if (dialogue4 != null)
        dialogue3 = dialogue4;
    }
    if (dialogue3 != null && this.Name.Equals("Caroline") && Game1.isLocationAccessible("CommunityCenter") && preface == "summer_" && str == "Mon")
      dialogue3 = this.TryGetDialogue("summer_Wed");
    return dialogue3 ?? (StardewValley.Dialogue) null;
  }

  public virtual void checkSchedule(int timeOfDay)
  {
    if ((double) this.currentScheduleDelay == 0.0 && (double) this.scheduleDelaySeconds > 0.0)
    {
      this.currentScheduleDelay = this.scheduleDelaySeconds;
    }
    else
    {
      if (this.returningToEndPoint)
        return;
      this.updatedDialogueYet = false;
      this.extraDialogueMessageToAddThisMorning = (StardewValley.Dialogue) null;
      if (this.ignoreScheduleToday || this.Schedule == null)
        return;
      SchedulePathDescription schedulePathDescription = (SchedulePathDescription) null;
      if (this.lastAttemptedSchedule < timeOfDay)
      {
        this.lastAttemptedSchedule = timeOfDay;
        this.Schedule.TryGetValue(timeOfDay, out schedulePathDescription);
        if (schedulePathDescription != null)
          this.queuedSchedulePaths.Add(schedulePathDescription);
        schedulePathDescription = (SchedulePathDescription) null;
      }
      PathFindController controller = this.controller;
      int num1;
      if (controller == null)
      {
        num1 = 0;
      }
      else
      {
        int? count = controller.pathToEndPoint?.Count;
        int num2 = 0;
        num1 = count.GetValueOrDefault() > num2 & count.HasValue ? 1 : 0;
      }
      if (num1 != 0)
        return;
      if (this.queuedSchedulePaths.Count > 0 && timeOfDay >= this.queuedSchedulePaths[0].time)
        schedulePathDescription = this.queuedSchedulePaths[0];
      if (schedulePathDescription == null)
        return;
      this.prepareToDisembarkOnNewSchedulePath();
      if (this.returningToEndPoint || this.temporaryController != null)
        return;
      this.directionsToNewLocation = schedulePathDescription;
      if (this.queuedSchedulePaths.Count > 0)
        this.queuedSchedulePaths.RemoveAt(0);
      this.controller = new PathFindController(this.directionsToNewLocation.route, (Character) this, Utility.getGameLocationOfCharacter(this))
      {
        finalFacingDirection = this.directionsToNewLocation.facingDirection,
        endBehaviorFunction = this.getRouteEndBehaviorFunction(this.directionsToNewLocation.endOfRouteBehavior, this.directionsToNewLocation.endOfRouteMessage)
      };
      if (this.controller.pathToEndPoint == null || this.controller.pathToEndPoint.Count == 0)
      {
        PathFindController.endBehavior behaviorFunction = this.controller.endBehaviorFunction;
        if (behaviorFunction != null)
          behaviorFunction((Character) this, this.currentLocation);
        this.controller = (PathFindController) null;
      }
      if (this.directionsToNewLocation?.route == null)
        return;
      this.previousEndPoint = this.directionsToNewLocation.route.LastOrDefault<Point>();
    }
  }

  private void finishEndOfRouteAnimation()
  {
    this._finishingEndOfRouteBehavior = this._startedEndOfRouteBehavior;
    this._startedEndOfRouteBehavior = (string) null;
    switch (this._finishingEndOfRouteBehavior)
    {
      case "change_beach":
        this.shouldWearIslandAttire.Value = true;
        this.currentlyDoingEndOfRouteAnimation = false;
        break;
      case "change_normal":
        this.shouldWearIslandAttire.Value = false;
        this.currentlyDoingEndOfRouteAnimation = false;
        break;
    }
    while (this.CurrentDialogue.Count > 0 && this.CurrentDialogue.Peek().removeOnNextMove)
      this.CurrentDialogue.Pop();
    this.shouldSayMarriageDialogue.Value = false;
    this.currentMarriageDialogue.Clear();
    this.nextEndOfRouteMessage = (string) null;
    this.endOfRouteMessage.Value = (string) null;
    if (this.currentlyDoingEndOfRouteAnimation && this.routeEndOutro != null)
    {
      bool flag = false;
      for (int index = 0; index < this.routeEndOutro.Length; ++index)
      {
        if (!flag)
        {
          this.Sprite.ClearAnimation();
          flag = true;
        }
        if (index == this.routeEndOutro.Length - 1)
          this.Sprite.AddFrame(new FarmerSprite.AnimationFrame(this.routeEndOutro[index], 100, 0, false, false, new AnimatedSprite.endOfAnimationBehavior(this.routeEndAnimationFinished), true));
        else
          this.Sprite.AddFrame(new FarmerSprite.AnimationFrame(this.routeEndOutro[index], 100, 0, false, false));
      }
      if (!flag)
        this.routeEndAnimationFinished((Farmer) null);
      if (this._finishingEndOfRouteBehavior == null)
        return;
      this.finishRouteBehavior(this._finishingEndOfRouteBehavior);
    }
    else
      this.routeEndAnimationFinished((Farmer) null);
  }

  protected virtual void prepareToDisembarkOnNewSchedulePath()
  {
    this.finishEndOfRouteAnimation();
    this.doingEndOfRouteAnimation.Value = false;
    this.currentlyDoingEndOfRouteAnimation = false;
    if (!this.isMarried())
      return;
    if (this.temporaryController == null && Utility.getGameLocationOfCharacter(this) is FarmHouse)
    {
      this.temporaryController = new PathFindController((Character) this, this.getHome(), new Point(this.getHome().warps[0].X, this.getHome().warps[0].Y), 2, true)
      {
        NPCSchedule = true
      };
      if (this.temporaryController.pathToEndPoint == null || this.temporaryController.pathToEndPoint.Count <= 0)
      {
        this.temporaryController = (PathFindController) null;
        this.ClearSchedule();
      }
      else
        this.followSchedule = true;
    }
    else
    {
      if (!(Utility.getGameLocationOfCharacter(this) is Farm))
        return;
      this.temporaryController = (PathFindController) null;
      this.ClearSchedule();
    }
  }

  public void checkForMarriageDialogue(int timeOfDay, GameLocation location)
  {
    if (this.Name == "Krobus" && Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth) == "Fri")
      return;
    switch (timeOfDay)
    {
      case 1100:
        this.setRandomAfternoonMarriageDialogue(1100, location);
        break;
      case 1800:
        if (!(location is FarmHouse))
          break;
        int num = Utility.CreateDaySaveRandom((double) timeOfDay, (double) this.getSpouse().UniqueMultiplayerID).Next(Game1.isRaining ? 7 : 6) - 1;
        string str = num >= 0 ? num.ToString() ?? "" : this.Name;
        this.currentMarriageDialogue.Clear();
        this.addMarriageDialogue("MarriageDialogue", $"{(Game1.isRaining ? "Rainy" : "Indoor")}_Night_{str}");
        break;
    }
  }

  private void routeEndAnimationFinished(Farmer who)
  {
    this.doingEndOfRouteAnimation.Value = false;
    this.freezeMotion = false;
    CharacterData data = this.GetData();
    this.Sprite.SpriteWidth = data != null ? data.Size.X : 16 /*0x10*/;
    this.Sprite.SpriteHeight = data != null ? data.Size.Y : 32 /*0x20*/;
    this.Sprite.UpdateSourceRect();
    this.Sprite.oldFrame = this._beforeEndOfRouteAnimationFrame;
    this.Sprite.StopAnimation();
    this.endOfRouteMessage.Value = (string) null;
    this.isCharging = false;
    this.speed = 2;
    this.addedSpeed = 0.0f;
    this.goingToDoEndOfRouteAnimation.Value = false;
    if (this.isWalkingInSquare)
      this.returningToEndPoint = true;
    if (this._finishingEndOfRouteBehavior == "penny_dishes")
      this.drawOffset = Vector2.Zero;
    if (this.appliedRouteAnimationOffset != Vector2.Zero)
    {
      this.drawOffset = Vector2.Zero;
      this.appliedRouteAnimationOffset = Vector2.Zero;
    }
    this._finishingEndOfRouteBehavior = (string) null;
  }

  public bool isOnSilentTemporaryMessage()
  {
    return (this.doingEndOfRouteAnimation.Value || !this.goingToDoEndOfRouteAnimation.Value) && this.endOfRouteMessage.Value != null && this.endOfRouteMessage.Value.EqualsIgnoreCase("silent");
  }

  public bool hasTemporaryMessageAvailable()
  {
    return !this.isDivorcedFrom(Game1.player) && (this.currentLocation != null && this.currentLocation.HasLocationOverrideDialogue(this) || this.endOfRouteMessage.Value != null && (this.doingEndOfRouteAnimation.Value || !this.goingToDoEndOfRouteAnimation.Value));
  }

  public bool setTemporaryMessages(Farmer who)
  {
    if (this.isOnSilentTemporaryMessage())
      return true;
    if (this.endOfRouteMessage.Value != null && (this.doingEndOfRouteAnimation.Value || !this.goingToDoEndOfRouteAnimation.Value))
    {
      if (!this.isDivorcedFrom(Game1.player) && (!this.endOfRouteMessage.Value.Contains("marriage") || this.getSpouse() == Game1.player))
      {
        this._PushTemporaryDialogue(this.endOfRouteMessage.Value);
        return false;
      }
    }
    else if (this.currentLocation != null && this.currentLocation.HasLocationOverrideDialogue(this))
    {
      this._PushTemporaryDialogue(this.currentLocation.GetLocationOverrideDialogue(this));
      return false;
    }
    return false;
  }

  /// <summary>Add a dialogue to the NPC's queue which is shown before other dialogues, and replaced if another temporary dialogue is added later.</summary>
  /// <param name="translationKey">The translation from which to take the dialogue text, in the form <c>assetName:fieldKey</c> like <c>Strings/UI:Confirm</c>.</param>
  protected void _PushTemporaryDialogue(string translationKey)
  {
    string str = translationKey;
    try
    {
      Friendship friendship;
      if (Game1.player.friendshipData.TryGetValue(this.Name, out friendship))
      {
        string path = $"{translationKey}_{friendship.Status}";
        if (Game1.content.LoadStringReturnNullIfNotFound(path) != null)
          translationKey = path;
      }
      if (this.CurrentDialogue.Count != 0 && !(this.CurrentDialogue.Peek().temporaryDialogueKey != translationKey))
        return;
      this.CurrentDialogue.Push(new StardewValley.Dialogue(this, translationKey)
      {
        removeOnNextMove = true,
        temporaryDialogueKey = translationKey
      });
    }
    catch (Exception ex)
    {
      Game1.log.Error($"NPC '{this.Name}' failed setting temporary dialogue key '{translationKey}'{(translationKey != str ? $" (from dialogue key '{str}')" : "")}", ex);
    }
  }

  private void walkInSquareAtEndOfRoute(Character c, GameLocation l)
  {
    this.startRouteBehavior(this.endOfRouteBehaviorName.Value);
  }

  private void doAnimationAtEndOfScheduleRoute(Character c, GameLocation l)
  {
    this.doingEndOfRouteAnimation.Value = true;
    this.reallyDoAnimationAtEndOfScheduleRoute();
    this.currentlyDoingEndOfRouteAnimation = true;
  }

  private void reallyDoAnimationAtEndOfScheduleRoute()
  {
    this._startedEndOfRouteBehavior = this.loadedEndOfRouteBehavior;
    bool flag = false;
    string endOfRouteBehavior = this._startedEndOfRouteBehavior;
    if (endOfRouteBehavior == "change_beach" || endOfRouteBehavior == "change_normal")
      flag = true;
    if (!flag)
    {
      if (this._startedEndOfRouteBehavior == "penny_dishes")
        this.drawOffset = new Vector2(0.0f, 16f);
      if (this._startedEndOfRouteBehavior.EndsWith("_sleep"))
      {
        this.layingDown = true;
        this.HideShadow = true;
      }
      if (this.routeAnimationMetadata != null)
      {
        for (int index = 0; index < this.routeAnimationMetadata.Length; ++index)
        {
          string[] strArray = ArgUtility.SplitBySpace(this.routeAnimationMetadata[index]);
          switch (strArray[0])
          {
            case "laying_down":
              this.layingDown = true;
              this.HideShadow = true;
              break;
            case "offset":
              this.appliedRouteAnimationOffset = new Vector2((float) int.Parse(strArray[1]), (float) int.Parse(strArray[2]));
              break;
          }
        }
      }
      if (this.appliedRouteAnimationOffset != Vector2.Zero)
        this.drawOffset = this.appliedRouteAnimationOffset;
      if (this._skipRouteEndIntro)
      {
        this.doMiddleAnimation((Farmer) null);
      }
      else
      {
        this.Sprite.ClearAnimation();
        for (int index = 0; index < this.routeEndIntro.Length; ++index)
        {
          if (index == this.routeEndIntro.Length - 1)
            this.Sprite.AddFrame(new FarmerSprite.AnimationFrame(this.routeEndIntro[index], 100, 0, false, false, new AnimatedSprite.endOfAnimationBehavior(this.doMiddleAnimation), true));
          else
            this.Sprite.AddFrame(new FarmerSprite.AnimationFrame(this.routeEndIntro[index], 100, 0, false, false));
        }
      }
    }
    this._skipRouteEndIntro = false;
    this.doingEndOfRouteAnimation.Value = true;
    this.freezeMotion = true;
    this._beforeEndOfRouteAnimationFrame = this.Sprite.oldFrame;
  }

  private void doMiddleAnimation(Farmer who)
  {
    this.Sprite.ClearAnimation();
    for (int index = 0; index < this.routeEndAnimation.Length; ++index)
      this.Sprite.AddFrame(new FarmerSprite.AnimationFrame(this.routeEndAnimation[index], 100, 0, false, false));
    this.Sprite.loop = true;
    if (this._startedEndOfRouteBehavior == null)
      return;
    this.startRouteBehavior(this._startedEndOfRouteBehavior);
  }

  private void startRouteBehavior(string behaviorName)
  {
    if (behaviorName.Length > 0 && behaviorName[0] == '"')
    {
      if (!Game1.IsMasterGame)
        return;
      this.endOfRouteMessage.Value = behaviorName.Replace("\"", "");
    }
    else
    {
      if (behaviorName.Contains("square_") && Game1.IsMasterGame)
      {
        this.lastCrossroad = new Microsoft.Xna.Framework.Rectangle(this.TilePoint.X * 64 /*0x40*/, this.TilePoint.Y * 64 /*0x40*/, 64 /*0x40*/, 64 /*0x40*/);
        string[] strArray = behaviorName.Split('_');
        this.walkInSquare(Convert.ToInt32(strArray[1]), Convert.ToInt32(strArray[2]), 6000);
        this.squareMovementFacingPreference = strArray.Length <= 3 ? -1 : Convert.ToInt32(strArray[3]);
      }
      if (behaviorName.Contains("sleep"))
      {
        this.isPlayingSleepingAnimation = true;
        this.playSleepingAnimation();
      }
      switch (behaviorName)
      {
        case "abigail_videogames":
          if (!Game1.IsMasterGame)
            break;
          Game1.multiplayer.broadcastSprites(Utility.getGameLocationOfCharacter(this), new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(167, 1714, 19, 14), 100f, 3, 999999, new Vector2(2f, 3f) * 64f + new Vector2(7f, 12f) * 4f, false, false, 0.0002f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            id = 688
          });
          this.doEmote(52);
          break;
        case "dick_fish":
          this.extendSourceRect(0, 32 /*0x20*/);
          this.Sprite.tempSpriteHeight = 64 /*0x40*/;
          this.drawOffset = new Vector2(0.0f, 96f);
          this.Sprite.ignoreSourceRectUpdates = false;
          if (!Utility.isOnScreen(Utility.Vector2ToPoint(this.Position), 64 /*0x40*/, this.currentLocation))
            break;
          this.currentLocation.playSound("slosh", new Vector2?(this.Tile));
          break;
        case "clint_hammer":
          this.extendSourceRect(16 /*0x10*/, 0);
          this.Sprite.SpriteWidth = 32 /*0x20*/;
          this.Sprite.ignoreSourceRectUpdates = false;
          this.Sprite.currentFrame = 8;
          this.Sprite.CurrentAnimation[14] = new FarmerSprite.AnimationFrame(9, 100, 0, false, false, new AnimatedSprite.endOfAnimationBehavior(this.clintHammerSound));
          break;
        case "birdie_fish":
          this.extendSourceRect(16 /*0x10*/, 0);
          this.Sprite.SpriteWidth = 32 /*0x20*/;
          this.Sprite.ignoreSourceRectUpdates = false;
          this.Sprite.currentFrame = 8;
          break;
      }
    }
  }

  public void playSleepingAnimation()
  {
    this.isSleeping.Value = true;
    Vector2 vector2 = new Vector2(0.0f, this.name.Equals((object) "Sebastian") ? 12f : -4f);
    if (this.isMarried())
      vector2.X = -12f;
    this.drawOffset = vector2;
    if (this.isPlayingSleepingAnimation)
      return;
    string str;
    if (DataLoader.AnimationDescriptions(Game1.content).TryGetValue(this.name.Value.ToLower() + "_sleep", out str))
    {
      int int32 = Convert.ToInt32(str.Split('/')[0]);
      this.Sprite.ClearAnimation();
      this.Sprite.AddFrame(new FarmerSprite.AnimationFrame(int32, 100, false, false));
      this.Sprite.loop = true;
    }
    this.isPlayingSleepingAnimation = true;
  }

  private void finishRouteBehavior(string behaviorName)
  {
    switch (behaviorName)
    {
      case "abigail_videogames":
        Utility.getGameLocationOfCharacter(this).removeTemporarySpritesWithID(688);
        break;
      case "birdie_fish":
      case "clint_hammer":
      case "dick_fish":
        this.reloadSprite();
        CharacterData data = this.GetData();
        this.Sprite.SpriteWidth = data != null ? data.Size.X : 16 /*0x10*/;
        this.Sprite.SpriteHeight = data != null ? data.Size.Y : 32 /*0x20*/;
        this.Sprite.UpdateSourceRect();
        this.drawOffset = Vector2.Zero;
        this.Halt();
        this.movementPause = 1;
        break;
    }
    if (!this.layingDown)
      return;
    this.layingDown = false;
    this.HideShadow = false;
  }

  public bool IsReturningToEndPoint() => this.returningToEndPoint;

  public void StartActivityWalkInSquare(int square_width, int square_height, int pause_offset)
  {
    Point tilePoint = this.TilePoint;
    this.lastCrossroad = new Microsoft.Xna.Framework.Rectangle(tilePoint.X * 64 /*0x40*/, tilePoint.Y * 64 /*0x40*/, 64 /*0x40*/, 64 /*0x40*/);
    this.walkInSquare(square_height, square_height, pause_offset);
  }

  public void EndActivityRouteEndBehavior() => this.finishEndOfRouteAnimation();

  public void StartActivityRouteEndBehavior(string behavior_name, string end_message)
  {
    PathFindController.endBehavior behaviorFunction = this.getRouteEndBehaviorFunction(behavior_name, end_message);
    if (behaviorFunction == null)
      return;
    behaviorFunction((Character) this, this.currentLocation);
  }

  protected PathFindController.endBehavior getRouteEndBehaviorFunction(
    string behaviorName,
    string endMessage)
  {
    if (endMessage != null || behaviorName != null && behaviorName.Length > 0 && behaviorName[0] == '"')
      this.nextEndOfRouteMessage = endMessage.Replace("\"", "");
    if (behaviorName == null)
      return (PathFindController.endBehavior) null;
    if (behaviorName.Length > 0 && behaviorName.Contains("square_"))
    {
      this.endOfRouteBehaviorName.Value = behaviorName;
      return new PathFindController.endBehavior(this.walkInSquareAtEndOfRoute);
    }
    Dictionary<string, string> dictionary = DataLoader.AnimationDescriptions(Game1.content);
    if (behaviorName == "change_beach" || behaviorName == "change_normal")
    {
      this.endOfRouteBehaviorName.Value = behaviorName;
      this.goingToDoEndOfRouteAnimation.Value = true;
    }
    else
    {
      if (!dictionary.ContainsKey(behaviorName))
        return (PathFindController.endBehavior) null;
      this.endOfRouteBehaviorName.Value = behaviorName;
      this.loadEndOfRouteBehavior(this.endOfRouteBehaviorName.Value);
      this.goingToDoEndOfRouteAnimation.Value = true;
    }
    return new PathFindController.endBehavior(this.doAnimationAtEndOfScheduleRoute);
  }

  private void loadEndOfRouteBehavior(string name)
  {
    this.loadedEndOfRouteBehavior = name;
    if (name.Length > 0 && name.Contains("square_"))
      return;
    string str = (string) null;
    try
    {
      if (!DataLoader.AnimationDescriptions(Game1.content).TryGetValue(name, out str))
        return;
      string[] source = str.Split('/');
      this.routeEndIntro = Utility.parseStringToIntArray(source[0]);
      this.routeEndAnimation = Utility.parseStringToIntArray(source[1]);
      this.routeEndOutro = Utility.parseStringToIntArray(source[2]);
      if (source.Length > 3 && source[3] != "")
        this.nextEndOfRouteMessage = source[3];
      if (source.Length > 4)
        this.routeAnimationMetadata = ((IEnumerable<string>) source).Skip<string>(4).ToArray<string>();
      else
        this.routeAnimationMetadata = (string[]) null;
    }
    catch (Exception ex)
    {
      Game1.log.Error($"NPC {this.Name} failed to apply end-of-route behavior '{name}'{(str != null ? $" with raw data '{str}'" : "")}.", ex);
    }
  }

  public void shake(int duration) => this.shakeTimer = duration;

  public void setNewDialogue(string translationKey, bool add = false, bool clearOnMovement = false)
  {
    this.setNewDialogue(new StardewValley.Dialogue(this, translationKey), add, clearOnMovement);
  }

  public void setNewDialogue(StardewValley.Dialogue dialogue, bool add = false, bool clearOnMovement = false)
  {
    if (!add)
      this.CurrentDialogue.Clear();
    dialogue.removeOnNextMove = clearOnMovement;
    this.CurrentDialogue.Push(dialogue);
  }

  private void setNewDialogue(
    string dialogueSheetName,
    string dialogueSheetKey,
    bool clearOnMovement = false)
  {
    this.CurrentDialogue.Clear();
    string dialogueKey = dialogueSheetKey + this.Name;
    if (dialogueSheetName.Contains("Marriage"))
    {
      if (this.getSpouse() != Game1.player)
        return;
      StardewValley.Dialogue dialogue = this.tryToGetMarriageSpecificDialogue(dialogueKey);
      if (dialogue == null)
      {
        Game1.log.Warn($"NPC '{this.Name}' couldn't set marriage dialogue key '{dialogueKey}': not found.");
        dialogue = StardewValley.Dialogue.GetFallbackForError(this);
      }
      dialogue.removeOnNextMove = clearOnMovement;
      this.CurrentDialogue.Push(dialogue);
    }
    else
    {
      string translationKey = $"Characters\\Dialogue\\{dialogueSheetName}:{dialogueKey}";
      StardewValley.Dialogue dialogue = StardewValley.Dialogue.TryGetDialogue(this, translationKey);
      if (dialogue == null)
      {
        Game1.log.Warn($"NPC '{this.Name}' couldn't set dialogue key '{translationKey}': not found.");
        dialogue = StardewValley.Dialogue.GetFallbackForError(this);
      }
      if (dialogue == null)
        return;
      dialogue.removeOnNextMove = clearOnMovement;
      this.CurrentDialogue.Push(dialogue);
    }
  }

  public string GetDialogueSheetName()
  {
    return this.Name == "Leo" && this.DefaultMap != "IslandHut" ? this.Name + "Mainland" : this.Name;
  }

  public void setSpouseRoomMarriageDialogue()
  {
    this.currentMarriageDialogue.Clear();
    this.addMarriageDialogue("MarriageDialogue", "spouseRoom_" + this.Name);
  }

  public void setRandomAfternoonMarriageDialogue(
    int time,
    GameLocation location,
    bool countAsDailyAfternoon = false)
  {
    if (this.Name == "Krobus" && Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth) == "Fri" || this.hasSaidAfternoonDialogue.Value)
      return;
    if (countAsDailyAfternoon)
      this.hasSaidAfternoonDialogue.Value = true;
    Random daySaveRandom = Utility.CreateDaySaveRandom((double) time);
    int heartLevelForNpc = this.getSpouse().getFriendshipHeartLevelForNPC(this.Name);
    switch (location)
    {
      case FarmHouse _:
        if (!daySaveRandom.NextBool())
          break;
        if (heartLevelForNpc < 9)
        {
          this.currentMarriageDialogue.Clear();
          this.addMarriageDialogue("MarriageDialogue", (daySaveRandom.NextDouble() < (double) heartLevelForNpc / 11.0 ? "Neutral_" : "Bad_") + daySaveRandom.Next(10).ToString());
          break;
        }
        if (daySaveRandom.NextDouble() < 0.05)
        {
          this.currentMarriageDialogue.Clear();
          this.addMarriageDialogue("MarriageDialogue", $"{Game1.currentSeason}_{this.Name}");
          break;
        }
        if (heartLevelForNpc >= 10 && daySaveRandom.NextBool() || heartLevelForNpc >= 11 && daySaveRandom.NextDouble() < 0.75 || heartLevelForNpc >= 12 && daySaveRandom.NextDouble() < 0.95)
        {
          this.currentMarriageDialogue.Clear();
          this.addMarriageDialogue("MarriageDialogue", "Good_" + daySaveRandom.Next(10).ToString());
          break;
        }
        this.currentMarriageDialogue.Clear();
        this.addMarriageDialogue("MarriageDialogue", "Neutral_" + daySaveRandom.Next(10).ToString());
        break;
      case Farm _:
        this.currentMarriageDialogue.Clear();
        if (daySaveRandom.NextDouble() < 0.2)
        {
          this.addMarriageDialogue("MarriageDialogue", "Outdoor_" + this.Name);
          break;
        }
        this.addMarriageDialogue("MarriageDialogue", "Outdoor_" + daySaveRandom.Next(5).ToString());
        break;
    }
  }

  /// <summary>Get whether it's the NPC's birthday today.</summary>
  public bool isBirthday()
  {
    return this.Birthday_Season == Game1.currentSeason && this.Birthday_Day == Game1.dayOfMonth;
  }

  /// <summary>Get the NPC's first loved item for the Statue of Endless Fortune.</summary>
  public Item getFavoriteItem()
  {
    string str;
    if (Game1.NPCGiftTastes.TryGetValue(this.Name, out str))
    {
      Item favoriteItem = ((IEnumerable<string>) ArgUtility.SplitBySpace(str.Split('/')[1])).Select<string, Item>((Func<string, Item>) (id => ItemRegistry.ResolveMetadata(id)?.CreateItem())).FirstOrDefault<Item>((Func<Item, bool>) (p => p != null));
      if (favoriteItem != null)
        return favoriteItem;
    }
    return (Item) null;
  }

  /// <summary>Get the NPC's data from <see cref="F:StardewValley.Game1.characterData" />, if found.</summary>
  public CharacterData GetData()
  {
    CharacterData data;
    return !this.IsVillager || !NPC.TryGetData(this.name.Value, out data) ? (CharacterData) null : data;
  }

  /// <summary>Try to get an NPC's data from <see cref="F:StardewValley.Game1.characterData" />.</summary>
  /// <param name="name">The NPC's internal name (i.e. the key in <see cref="F:StardewValley.Game1.characterData" />).</param>
  /// <param name="data">The NPC data, if found.</param>
  /// <returns>Returns whether the NPC data was found.</returns>
  public static bool TryGetData(string name, out CharacterData data)
  {
    if (name != null)
      return Game1.characterData.TryGetValue(name, out data);
    data = (CharacterData) null;
    return false;
  }

  /// <summary>Get the translated display name for an NPC from the underlying data, if any.</summary>
  /// <param name="name">The NPC's internal name.</param>
  public static string GetDisplayName(string name)
  {
    CharacterData data;
    NPC.TryGetData(name, out data);
    return TokenParser.ParseText(data?.DisplayName) ?? name;
  }

  /// <summary>Get whether social features (like birthdays, gift giving, friendship, and an entry in the social tab) should be enabled for an NPC based on their entry in <c>Data/Characters</c>.</summary>
  /// <param name="name">The NPC name to check.</param>
  /// <param name="location">The location to assume they're in when parsing conditions.</param>
  /// <remarks>Most code should use <see cref="P:StardewValley.NPC.CanSocialize" /> instead.</remarks>
  public static bool CanSocializePerData(string name, GameLocation location)
  {
    CharacterData data;
    return NPC.TryGetData(name, out data) && GameStateQuery.CheckConditions(data.CanSocialize, location);
  }

  /// <summary>Get a tokenized string for the NPC's display name.</summary>
  public string GetTokenizedDisplayName() => this.GetData()?.DisplayName ?? this.displayName;

  /// <summary>Get whether this NPC speaks Dwarvish, which the player can only understand after finding the Dwarvish Translation Guide.</summary>
  public bool SpeaksDwarvish()
  {
    CharacterData data = this.GetData();
    return data != null && data.Language == NpcLanguage.Dwarvish;
  }

  public virtual void receiveGift(
    Object o,
    Farmer giver,
    bool updateGiftLimitInfo = true,
    float friendshipChangeMultiplier = 1f,
    bool showResponse = true)
  {
    if (!this.CanReceiveGifts())
      return;
    float num = 1f;
    switch (o.Quality)
    {
      case 1:
        num = 1.1f;
        break;
      case 2:
        num = 1.25f;
        break;
      case 4:
        num = 1.5f;
        break;
    }
    if (this.isBirthday())
      friendshipChangeMultiplier = 8f;
    if (this.getSpouse() != null && this.getSpouse().Equals((object) giver))
      friendshipChangeMultiplier /= 2f;
    giver.onGiftGiven(this, o);
    ++Game1.stats.GiftsGiven;
    giver.currentLocation.localSound("give_gift");
    if (updateGiftLimitInfo)
    {
      ++giver.friendshipData[this.Name].GiftsToday;
      ++giver.friendshipData[this.Name].GiftsThisWeek;
      giver.friendshipData[this.Name].LastGiftDate = new WorldDate(Game1.Date);
    }
    switch (giver.FacingDirection)
    {
      case 0:
        ((FarmerSprite) giver.Sprite).animateBackwardsOnce(80 /*0x50*/, 50f);
        break;
      case 1:
        ((FarmerSprite) giver.Sprite).animateBackwardsOnce(72, 50f);
        break;
      case 2:
        ((FarmerSprite) giver.Sprite).animateBackwardsOnce(64 /*0x40*/, 50f);
        break;
      case 3:
        ((FarmerSprite) giver.Sprite).animateBackwardsOnce(88, 50f);
        break;
    }
    int tasteForThisItem = this.getGiftTasteForThisItem((Item) o);
    switch (tasteForThisItem)
    {
      case 0:
        giver.changeFriendship((int) (80.0 * (double) friendshipChangeMultiplier * (double) num), this);
        this.doEmote(20);
        this.faceTowardFarmerForPeriod(15000, 4, false, giver);
        break;
      case 2:
        giver.changeFriendship((int) (45.0 * (double) friendshipChangeMultiplier * (double) num), this);
        this.faceTowardFarmerForPeriod(7000, 3, true, giver);
        break;
      case 4:
        giver.changeFriendship((int) (-20.0 * (double) friendshipChangeMultiplier), this);
        break;
      case 6:
        giver.changeFriendship((int) (-40.0 * (double) friendshipChangeMultiplier), this);
        this.doEmote(12);
        this.faceTowardFarmerForPeriod(15000, 4, true, giver);
        break;
      case 7:
        giver.changeFriendship(Math.Min(750, (int) (250.0 * (double) friendshipChangeMultiplier)), this);
        this.doEmote(56);
        this.faceTowardFarmerForPeriod(15000, 4, false, giver);
        break;
      default:
        giver.changeFriendship((int) (20.0 * (double) friendshipChangeMultiplier), this);
        break;
    }
    if (!showResponse)
      return;
    Game1.DrawDialogue(this.GetGiftReaction(giver, o, tasteForThisItem));
  }

  /// <summary>Get the NPC's reaction dialogue for receiving an item as a gift.</summary>
  /// <param name="giver">The player giving the gift.</param>
  /// <param name="gift">The item being gifted.</param>
  /// <param name="taste">The NPC's gift taste for this item, as returned by <see cref="M:StardewValley.NPC.getGiftTasteForThisItem(StardewValley.Item)" />.</param>
  /// <returns>Returns the dialogue if the NPC can receive gifts, else <c>null</c>.</returns>
  public virtual StardewValley.Dialogue GetGiftReaction(Farmer giver, Object gift, int taste)
  {
    string str1;
    if (!this.CanReceiveGifts() || !Game1.NPCGiftTastes.TryGetValue(this.Name, out str1))
      return (StardewValley.Dialogue) null;
    string str2 = (string) null;
    StardewValley.Dialogue giftReaction;
    if (this.Name == "Krobus" && Game1.Date.DayOfWeek == DayOfWeek.Friday)
      giftReaction = this.TryGetDialogue("Fri") ?? StardewValley.Dialogue.GetFallbackForError(this);
    else if (this.isBirthday())
    {
      StardewValley.Dialogue dialogue = this.TryGetDialogue("AcceptBirthdayGift_" + gift.QualifiedItemId) ?? gift.GetContextTags().Select<string, StardewValley.Dialogue>((Func<string, StardewValley.Dialogue>) (tag => this.TryGetDialogueByGiftTaste(taste, (Func<string, string>) (tasteTag => $"AcceptBirthdayGift_{tasteTag}_{tag}")))).FirstOrDefault<StardewValley.Dialogue>((Func<StardewValley.Dialogue, bool>) (p => p != null)) ?? gift.GetContextTags().Select<string, StardewValley.Dialogue>((Func<string, StardewValley.Dialogue>) (tag => this.TryGetDialogue("AcceptBirthdayGift_" + tag))).FirstOrDefault<StardewValley.Dialogue>((Func<StardewValley.Dialogue, bool>) (p => p != null)) ?? this.TryGetDialogueByGiftTaste(taste, (Func<string, string>) (tasteTag => "AcceptBirthdayGift_" + tasteTag)) ?? this.TryGetDialogue("AcceptBirthdayGift");
      switch (taste)
      {
        case 0:
        case 2:
        case 7:
          str2 = "$h";
          giftReaction = dialogue ?? (Game1.random.NextBool() ? (this.Manners == 2 ? new StardewValley.Dialogue(this, "Strings\\StringsFromCSFiles:NPC.cs.4276", true) : new StardewValley.Dialogue(this, "Strings\\StringsFromCSFiles:NPC.cs.4277", true)) : (this.Manners == 2 ? new StardewValley.Dialogue(this, "Strings\\StringsFromCSFiles:NPC.cs.4274", true) : new StardewValley.Dialogue(this, "Strings\\StringsFromCSFiles:NPC.cs.4275")));
          break;
        case 4:
        case 6:
          str2 = "$s";
          giftReaction = dialogue ?? (this.Manners == 2 ? new StardewValley.Dialogue(this, "Strings\\StringsFromCSFiles:NPC.cs.4278", true) : new StardewValley.Dialogue(this, "Strings\\StringsFromCSFiles:NPC.cs.4279", true));
          break;
        default:
          giftReaction = dialogue ?? (this.Manners == 2 ? new StardewValley.Dialogue(this, "Strings\\StringsFromCSFiles:NPC.cs.4280") : new StardewValley.Dialogue(this, "Strings\\StringsFromCSFiles:NPC.cs.4281", true));
          break;
      }
    }
    else
    {
      StardewValley.Dialogue dialogue = this.TryGetDialogue("AcceptGift_" + gift.QualifiedItemId) ?? gift.GetContextTags().Select<string, StardewValley.Dialogue>((Func<string, StardewValley.Dialogue>) (tag => this.TryGetDialogueByGiftTaste(taste, (Func<string, string>) (tasteTag => $"AcceptGift_{tasteTag}_{tag}")))).FirstOrDefault<StardewValley.Dialogue>((Func<StardewValley.Dialogue, bool>) (p => p != null)) ?? gift.GetContextTags().Select<string, StardewValley.Dialogue>((Func<string, StardewValley.Dialogue>) (tag => this.TryGetDialogue("AcceptGift_" + tag))).FirstOrDefault<StardewValley.Dialogue>((Func<StardewValley.Dialogue, bool>) (p => p != null)) ?? this.TryGetDialogueByGiftTaste(taste, (Func<string, string>) (tasteTag => "AcceptGift_" + tasteTag)) ?? this.TryGetDialogue("AcceptGift");
      string[] array = str1.Split('/');
      switch (taste)
      {
        case 0:
        case 2:
          if (dialogue == null)
            str2 = "$h";
          giftReaction = dialogue ?? new StardewValley.Dialogue(this, (string) null, ArgUtility.Get(array, taste));
          break;
        case 4:
        case 6:
          str2 = "$s";
          giftReaction = dialogue ?? new StardewValley.Dialogue(this, (string) null, ArgUtility.Get(array, taste));
          break;
        case 7:
          str2 = "$h";
          giftReaction = dialogue ?? new StardewValley.Dialogue(this, (string) null, ArgUtility.Get(array, 0));
          break;
        default:
          giftReaction = dialogue ?? new StardewValley.Dialogue(this, (string) null, ArgUtility.Get(array, 8));
          break;
      }
    }
    if (!giver.canUnderstandDwarves && this.SpeaksDwarvish())
      giftReaction.convertToDwarvish();
    else if (str2 != null && !giftReaction.CurrentEmotionSetExplicitly)
      giftReaction.CurrentEmotion = str2;
    return giftReaction;
  }

  public override void draw(SpriteBatch b, float alpha = 1f)
  {
    int y = this.StandingPixel.Y;
    float layerDepth = Math.Max(0.0f, this.drawOnTop ? 0.991f : (float) y / 10000f);
    if (this.Sprite.Texture == null)
    {
      Vector2 local = Game1.GlobalToLocal(Game1.viewport, this.Position);
      Microsoft.Xna.Framework.Rectangle screenArea = new Microsoft.Xna.Framework.Rectangle((int) local.X, (int) local.Y - this.Sprite.SpriteWidth * 4, this.Sprite.SpriteWidth * 4, this.Sprite.SpriteHeight * 4);
      Utility.DrawErrorTexture(b, screenArea, layerDepth);
    }
    else
    {
      if (this.IsInvisible || !Utility.isOnScreen(this.Position, 128 /*0x80*/) && (!this.EventActor || !(this.currentLocation is Summit)))
        return;
      if (this.swimming.Value)
      {
        b.Draw(this.Sprite.Texture, this.getLocalPosition(Game1.viewport) + new Vector2(32f, (float) (80 /*0x50*/ + this.yJumpOffset * 2)) + (this.shakeTimer > 0 ? new Vector2((float) Game1.random.Next(-1, 2), (float) Game1.random.Next(-1, 2)) : Vector2.Zero) - new Vector2(0.0f, this.yOffset), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(this.Sprite.SourceRect.X, this.Sprite.SourceRect.Y, this.Sprite.SourceRect.Width, this.Sprite.SourceRect.Height / 2 - (int) ((double) this.yOffset / 4.0))), Color.White, this.rotation, new Vector2(32f, 96f) / 4f, Math.Max(0.2f, this.scale.Value) * 4f, this.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, layerDepth);
        Vector2 localPosition = this.getLocalPosition(Game1.viewport);
        b.Draw(Game1.staminaRect, new Microsoft.Xna.Framework.Rectangle((int) localPosition.X + (int) this.yOffset + 8, (int) localPosition.Y - 128 /*0x80*/ + this.Sprite.SourceRect.Height * 4 + 48 /*0x30*/ + this.yJumpOffset * 2 - (int) this.yOffset, this.Sprite.SourceRect.Width * 4 - (int) this.yOffset * 2 - 16 /*0x10*/, 4), new Microsoft.Xna.Framework.Rectangle?(Game1.staminaRect.Bounds), Color.White * 0.75f, 0.0f, Vector2.Zero, SpriteEffects.None, (float) ((double) y / 10000.0 + 1.0 / 1000.0));
      }
      else
        b.Draw(this.Sprite.Texture, this.getLocalPosition(Game1.viewport) + new Vector2((float) (this.GetSpriteWidthForPositioning() * 4 / 2), (float) (this.GetBoundingBox().Height / 2)) + (this.shakeTimer > 0 ? new Vector2((float) Game1.random.Next(-1, 2), (float) Game1.random.Next(-1, 2)) : Vector2.Zero), new Microsoft.Xna.Framework.Rectangle?(this.Sprite.SourceRect), Color.White * alpha, this.rotation, new Vector2((float) (this.Sprite.SpriteWidth / 2), (float) ((double) this.Sprite.SpriteHeight * 3.0 / 4.0)), Math.Max(0.2f, this.scale.Value) * 4f, this.flip || this.Sprite.CurrentAnimation != null && this.Sprite.CurrentAnimation[this.Sprite.currentAnimationIndex].flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, layerDepth);
      this.DrawBreathing(b, alpha);
      this.DrawGlow(b);
      if (Game1.eventUp)
        return;
      this.DrawEmote(b);
    }
  }

  public virtual void DrawBreathing(SpriteBatch b, float alpha = 1f)
  {
    if (!this.Breather || this.shakeTimer > 0 || this.swimming.Value || this.farmerPassesThrough)
      return;
    AnimatedSprite sprite1 = this.Sprite;
    if ((sprite1 != null ? (sprite1.SpriteHeight > 32 /*0x20*/ ? 1 : 0) : 0) != 0)
      return;
    AnimatedSprite sprite2 = this.Sprite;
    if ((sprite2 != null ? (sprite2.SpriteWidth > 16 /*0x10*/ ? 1 : 0) : 0) != 0)
      return;
    AnimatedSprite sprite3 = this.Sprite;
    if (sprite3.currentFrame >= 16 /*0x10*/)
      return;
    CharacterData data = this.GetData();
    Microsoft.Xna.Framework.Rectangle sourceRect = sprite3.SourceRect;
    Microsoft.Xna.Framework.Rectangle rectangle1;
    if (data != null && data.BreathChestRect.HasValue)
    {
      Microsoft.Xna.Framework.Rectangle rectangle2 = data.BreathChestRect.Value;
      rectangle1 = new Microsoft.Xna.Framework.Rectangle(sourceRect.X + rectangle2.X, sourceRect.Y + rectangle2.Y, rectangle2.Width, rectangle2.Height);
    }
    else
    {
      rectangle1 = new Microsoft.Xna.Framework.Rectangle(sourceRect.X + sprite3.SpriteWidth / 4, sourceRect.Y + sprite3.SpriteHeight / 2 + sprite3.SpriteHeight / 32 /*0x20*/, sprite3.SpriteHeight / 4, sprite3.SpriteWidth / 2);
      if (this.Age == 2)
      {
        rectangle1.Y += sprite3.SpriteHeight / 6 + 1;
        rectangle1.Height /= 2;
      }
      else if (this.Gender == Gender.Female)
      {
        ++rectangle1.Y;
        rectangle1.Height /= 2;
      }
    }
    Vector2 vector2;
    if (data != null && data.BreathChestPosition.HasValue)
    {
      vector2 = Utility.PointToVector2(data.BreathChestPosition.Value);
    }
    else
    {
      vector2 = new Vector2((float) (sprite3.SpriteWidth * 4 / 2), 8f);
      if (this.Age == 2)
      {
        vector2.Y += (float) (sprite3.SpriteHeight / 8 * 4);
        if (this is Child child)
        {
          switch (child.Age)
          {
            case 0:
              vector2.X -= 12f;
              break;
            case 1:
              vector2.X -= 4f;
              break;
          }
        }
      }
      else if (this.Gender == Gender.Female)
        vector2.Y -= 4f;
    }
    float num = Math.Max(0.0f, (float) (Math.Ceiling(Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 600.0 + (double) this.defaultPosition.X * 20.0)) / 4.0));
    int y = this.StandingPixel.Y;
    b.Draw(sprite3.Texture, this.getLocalPosition(Game1.viewport) + vector2 + (this.shakeTimer > 0 ? new Vector2((float) Game1.random.Next(-1, 2), (float) Game1.random.Next(-1, 2)) : Vector2.Zero), new Microsoft.Xna.Framework.Rectangle?(rectangle1), Color.White * alpha, this.rotation, new Vector2((float) (rectangle1.Width / 2), (float) (rectangle1.Height / 2 + 1)), Math.Max(0.2f, this.scale.Value) * 4f + num, this.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0.0f, this.drawOnTop ? 0.992f : (float) (((double) y + 0.0099999997764825821) / 10000.0)));
  }

  public virtual void DrawGlow(SpriteBatch b)
  {
    int y = this.StandingPixel.Y;
    if (!this.isGlowing)
      return;
    b.Draw(this.Sprite.Texture, this.getLocalPosition(Game1.viewport) + new Vector2((float) (this.GetSpriteWidthForPositioning() * 4 / 2), (float) (this.GetBoundingBox().Height / 2)) + (this.shakeTimer > 0 ? new Vector2((float) Game1.random.Next(-1, 2), (float) Game1.random.Next(-1, 2)) : Vector2.Zero), new Microsoft.Xna.Framework.Rectangle?(this.Sprite.SourceRect), this.glowingColor * this.glowingTransparency, this.rotation, new Vector2((float) (this.Sprite.SpriteWidth / 2), (float) ((double) this.Sprite.SpriteHeight * 3.0 / 4.0)), Math.Max(0.2f, this.scale.Value) * 4f, this.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0.0f, this.drawOnTop ? 0.99f : (float) ((double) y / 10000.0 + 1.0 / 1000.0)));
  }

  public virtual void DrawEmote(SpriteBatch b)
  {
    if (!this.IsEmoting)
      return;
    switch (this)
    {
      case Child _:
        break;
      case Pet _:
        break;
      default:
        CharacterData data = this.GetData();
        Point point = data != null ? data.EmoteOffset : Point.Zero;
        Vector2 position = this.getLocalPosition(Game1.viewport);
        position = new Vector2((float) ((double) position.X + (double) point.X + ((double) this.Sprite.SourceRect.Width / 2.0 - 8.0) * 4.0), position.Y + (float) point.Y + (float) this.emoteYOffset - (float) ((this.Sprite.SpriteHeight + 3) * 4));
        if (this.NeedsBirdieEmoteHack())
          position.X += 64f;
        if (this.Age == 2)
          position.Y += 32f;
        else if (this.Gender == Gender.Female)
          position.Y += 10f;
        b.Draw(Game1.emoteSpriteSheet, position, new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(this.CurrentEmoteIndex * 16 /*0x10*/ % Game1.emoteSpriteSheet.Width, this.CurrentEmoteIndex * 16 /*0x10*/ / Game1.emoteSpriteSheet.Width * 16 /*0x10*/, 16 /*0x10*/, 16 /*0x10*/)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) this.StandingPixel.Y / 10000f);
        break;
    }
  }

  public override void drawAboveAlwaysFrontLayer(SpriteBatch b)
  {
    if (this.textAboveHeadTimer <= 0 || this.textAboveHead == null)
      return;
    Point standingPixel = this.StandingPixel;
    Vector2 local = Game1.GlobalToLocal(new Vector2((float) standingPixel.X, (float) (standingPixel.Y - this.Sprite.SpriteHeight * 4 - 64 /*0x40*/ + this.yJumpOffset)));
    if (this.textAboveHeadStyle == 0)
      local += new Vector2((float) Game1.random.Next(-1, 2), (float) Game1.random.Next(-1, 2));
    if (this.NeedsBirdieEmoteHack())
      local.X += (float) (-this.GetBoundingBox().Width / 4 + 64 /*0x40*/);
    if (this.shouldShadowBeOffset)
      local += this.drawOffset;
    Point tilePoint = this.TilePoint;
    SpriteText.drawStringWithScrollCenteredAt(b, this.textAboveHead, (int) local.X, (int) local.Y, alpha: this.textAboveHeadAlpha, color: this.textAboveHeadColor, scrollType: 1, layerDepth: (float) ((double) (tilePoint.Y * 64 /*0x40*/) / 10000.0 + 1.0 / 1000.0 + (double) tilePoint.X / 10000.0));
  }

  public bool NeedsBirdieEmoteHack()
  {
    return Game1.eventUp && this.Sprite.SpriteWidth == 32 /*0x20*/ && this.Name == "Birdie";
  }

  public void warpToPathControllerDestination()
  {
    if (this.controller == null)
      return;
    while (this.controller.pathToEndPoint.Count > 2)
    {
      this.controller.pathToEndPoint.Pop();
      this.controller.handleWarps(new Microsoft.Xna.Framework.Rectangle(this.controller.pathToEndPoint.Peek().X * 64 /*0x40*/, this.controller.pathToEndPoint.Peek().Y * 64 /*0x40*/, 64 /*0x40*/, 64 /*0x40*/));
      this.Position = new Vector2((float) (this.controller.pathToEndPoint.Peek().X * 64 /*0x40*/), (float) (this.controller.pathToEndPoint.Peek().Y * 64 /*0x40*/ + 16 /*0x10*/));
      this.Halt();
    }
  }

  /// <summary>Get the pixel area in the <see cref="P:StardewValley.Character.Sprite" /> texture to show as the NPC's icon in contexts like the calendar and social menu.</summary>
  public virtual Microsoft.Xna.Framework.Rectangle getMugShotSourceRect()
  {
    return (Microsoft.Xna.Framework.Rectangle?) this.GetData()?.MugShotSourceRect ?? new Microsoft.Xna.Framework.Rectangle(0, this.Age == 2 ? 4 : 0, 16 /*0x10*/, 24);
  }

  public void getHitByPlayer(Farmer who, GameLocation location)
  {
    this.doEmote(12);
    if (who == null)
    {
      if (Game1.IsMultiplayer)
        return;
      who = Game1.player;
    }
    if (who.friendshipData.ContainsKey(this.Name))
    {
      who.changeFriendship(-30, this);
      if (who.IsLocalPlayer)
      {
        this.CurrentDialogue.Clear();
        this.CurrentDialogue.Push(this.TryGetDialogue("HitBySlingshot") ?? (Game1.random.NextBool() ? new StardewValley.Dialogue(this, "Strings\\StringsFromCSFiles:NPC.cs.4293", true) : new StardewValley.Dialogue(this, "Strings\\StringsFromCSFiles:NPC.cs.4294")));
      }
      if (this.Sprite.Texture != null)
        location.debris.Add(new Debris(this.Sprite.textureName.Value, Game1.random.Next(3, 8), Utility.PointToVector2(this.StandingPixel)));
    }
    if (this.Name.Equals("Bouncer"))
      location.localSound("crafting");
    else
      location.localSound("hitEnemy");
  }

  public void walkInSquare(int squareWidth, int squareHeight, int squarePauseOffset)
  {
    this.isWalkingInSquare = true;
    this.lengthOfWalkingSquareX = squareWidth;
    this.lengthOfWalkingSquareY = squareHeight;
    this.squarePauseOffset = squarePauseOffset;
  }

  public void moveTowardPlayer(int threshold)
  {
    this.isWalkingTowardPlayer.Value = true;
    this.moveTowardPlayerThreshold.Value = threshold;
  }

  protected virtual Farmer findPlayer() => Game1.MasterPlayer;

  public virtual bool withinPlayerThreshold()
  {
    return this.withinPlayerThreshold(this.moveTowardPlayerThreshold.Value);
  }

  public virtual bool withinPlayerThreshold(int threshold)
  {
    if (this.currentLocation != null && !this.currentLocation.farmers.Any())
      return false;
    Vector2 tile1 = this.findPlayer().Tile;
    Vector2 tile2 = this.Tile;
    return (double) Math.Abs(tile2.X - tile1.X) <= (double) threshold && (double) Math.Abs(tile2.Y - tile1.Y) <= (double) threshold;
  }

  private Stack<Point> addToStackForSchedule(Stack<Point> original, Stack<Point> toAdd)
  {
    if (toAdd == null)
      return original;
    original = new Stack<Point>((IEnumerable<Point>) original);
    while (original.Count > 0)
      toAdd.Push(original.Pop());
    return toAdd;
  }

  public virtual SchedulePathDescription pathfindToNextScheduleLocation(
    string scheduleKey,
    string startingLocation,
    int startingX,
    int startingY,
    string endingLocation,
    int endingX,
    int endingY,
    int finalFacingDirection,
    string endBehavior,
    string endMessage)
  {
    Stack<Point> pointStack = new Stack<Point>();
    Point startPoint = new Point(startingX, startingY);
    if (startPoint == Point.Zero)
      throw new Exception($"NPC {this.Name} has an invalid schedule with key '{scheduleKey}': start position in {startingLocation} is at tile (0, 0), which isn't valid.");
    string[] locationRoute = !startingLocation.Equals(endingLocation, StringComparison.Ordinal) ? this.getLocationRoute(startingLocation, endingLocation) : (string[]) null;
    if (locationRoute != null)
    {
      for (int index = 0; index < locationRoute.Length; ++index)
      {
        string str1 = locationRoute[index];
        foreach (string activePassiveFestival in (IEnumerable<string>) Game1.netWorldState.Value.ActivePassiveFestivals)
        {
          PassiveFestivalData data;
          string str2;
          if (Utility.TryGetPassiveFestivalData(activePassiveFestival, out data) && data.MapReplacements != null && data.MapReplacements.TryGetValue(str1, out str2))
          {
            str1 = str2;
            break;
          }
        }
        GameLocation location = Game1.RequireLocation(str1);
        if (location.Name.Equals("Trailer") && Game1.MasterPlayer.mailReceived.Contains("pamHouseUpgrade"))
          location = Game1.RequireLocation("Trailer_Big");
        if (index < locationRoute.Length - 1)
        {
          Point warpPointTo = location.getWarpPointTo(locationRoute[index + 1]);
          if (warpPointTo == Point.Zero)
            throw new Exception($"NPC {this.Name} has an invalid schedule with key '{scheduleKey}': it requires a warp from {location.NameOrUniqueName} to {locationRoute[index + 1]}, but none was found.");
          pointStack = this.addToStackForSchedule(pointStack, PathFindController.findPathForNPCSchedules(startPoint, warpPointTo, location, 30000, (Character) this));
          startPoint = location.getWarpPointTarget(warpPointTo, (Character) this);
        }
        else
          pointStack = this.addToStackForSchedule(pointStack, PathFindController.findPathForNPCSchedules(startPoint, new Point(endingX, endingY), location, 30000, (Character) this));
      }
    }
    else if (startingLocation.Equals(endingLocation, StringComparison.Ordinal))
    {
      string str3 = startingLocation;
      foreach (string activePassiveFestival in (IEnumerable<string>) Game1.netWorldState.Value.ActivePassiveFestivals)
      {
        PassiveFestivalData data;
        string str4;
        if (Utility.TryGetPassiveFestivalData(activePassiveFestival, out data) && data.MapReplacements != null && data.MapReplacements.TryGetValue(str3, out str4))
        {
          str3 = str4;
          break;
        }
      }
      GameLocation location = Game1.RequireLocation(str3);
      if (location.Name.Equals("Trailer") && Game1.MasterPlayer.mailReceived.Contains("pamHouseUpgrade"))
        location = Game1.RequireLocation("Trailer_Big");
      pointStack = PathFindController.findPathForNPCSchedules(startPoint, new Point(endingX, endingY), location, 30000, (Character) this);
    }
    return new SchedulePathDescription(pointStack, finalFacingDirection, endBehavior, endMessage, endingLocation, new Point(endingX, endingY));
  }

  private string[] getLocationRoute(string startingLocation, string endingLocation)
  {
    return WarpPathfindingCache.GetLocationRoute(startingLocation, endingLocation, this.Gender);
  }

  /// <summary>
  /// returns true if location is inaccessable and should use "Default" instead.
  /// 
  /// 
  /// </summary>
  /// <param name="locationName"></param>
  /// <param name="tileX"></param>
  /// <param name="tileY"></param>
  /// <param name="facingDirection"></param>
  /// <returns></returns>
  private bool changeScheduleForLocationAccessibility(
    ref string locationName,
    ref int tileX,
    ref int tileY,
    ref int facingDirection)
  {
    switch (locationName)
    {
      case "JojaMart":
      case "Railroad":
        if (!Game1.isLocationAccessible(locationName))
        {
          if (!this.hasMasterScheduleEntry(locationName + "_Replacement"))
            return true;
          string[] strArray = ArgUtility.SplitBySpace(this.getMasterScheduleEntry(locationName + "_Replacement"));
          locationName = strArray[0];
          tileX = Convert.ToInt32(strArray[1]);
          tileY = Convert.ToInt32(strArray[2]);
          facingDirection = Convert.ToInt32(strArray[3]);
          break;
        }
        break;
      case "CommunityCenter":
        return !Game1.isLocationAccessible(locationName);
    }
    return false;
  }

  /// <inheritdoc cref="M:StardewValley.NPC.parseMasterScheduleImpl(System.String,System.String,System.Collections.Generic.List{System.String})" />
  public virtual Dictionary<int, SchedulePathDescription> parseMasterSchedule(
    string scheduleKey,
    string rawData)
  {
    return this.parseMasterScheduleImpl(scheduleKey, rawData, new List<string>());
  }

  /// <summary>Parse a schedule script into its component commands, handling redirection like <c>GOTO</c> automatically.</summary>
  /// <param name="scheduleKey">The schedule key being parsed.</param>
  /// <param name="rawData">The raw schedule script to parse.</param>
  /// <param name="visited">The schedule keys which led to this parse (if any).</param>
  /// <remarks>This is a low-level method. Most code should call <see cref="M:StardewValley.NPC.TryLoadSchedule(System.String)" /> instead.</remarks>
  protected virtual Dictionary<int, SchedulePathDescription> parseMasterScheduleImpl(
    string scheduleKey,
    string rawData,
    List<string> visited)
  {
    if (visited.Contains<string>(scheduleKey, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))
    {
      Game1.log.Warn($"NPC {this.Name} can't load schedules because they led to an infinite loop ({string.Join(" -> ", (IEnumerable<string>) visited)} -> {scheduleKey}).");
      return new Dictionary<int, SchedulePathDescription>();
    }
    visited.Add(scheduleKey);
    try
    {
      string[] strArray1 = NPC.SplitScheduleCommands(rawData);
      Dictionary<int, SchedulePathDescription> masterScheduleImpl = new Dictionary<int, SchedulePathDescription>();
      int index1 = 0;
      if (strArray1[0].Contains("GOTO"))
      {
        string str = ArgUtility.SplitBySpaceAndGet(strArray1[0], 1);
        Dictionary<string, string> masterScheduleRawData = this.getMasterScheduleRawData();
        if (str.EqualsIgnoreCase("season"))
        {
          str = Game1.currentSeason;
          if (!masterScheduleRawData.ContainsKey(str))
            str = "spring";
        }
        try
        {
          string rawData1;
          if (masterScheduleRawData.TryGetValue(str, out rawData1))
            return this.parseMasterScheduleImpl(str, rawData1, visited);
          Game1.log.Error($"Failed to load schedule '{scheduleKey}' for NPC '{this.Name}': GOTO references schedule '{str}' which doesn't exist. Falling back to 'spring'.");
        }
        catch (Exception ex)
        {
          Game1.log.Error($"Failed to load schedule '{scheduleKey}' for NPC '{this.Name}': GOTO references schedule '{str}' which couldn't be parsed. Falling back to 'spring'.", ex);
        }
        return this.parseMasterScheduleImpl("spring", this.getMasterScheduleEntry("spring"), visited);
      }
      if (strArray1[0].Contains("NOT"))
      {
        string[] strArray2 = ArgUtility.SplitBySpace(strArray1[0]);
        if (strArray2[1].ToLower() == "friendship")
        {
          int index2 = 2;
          bool flag = false;
          for (; index2 < strArray2.Length; index2 += 2)
          {
            string name = strArray2[index2];
            int result;
            if (int.TryParse(strArray2[index2 + 1], out result))
            {
              foreach (Farmer allFarmer in Game1.getAllFarmers())
              {
                if (allFarmer.getFriendshipHeartLevelForNPC(name) >= result)
                {
                  flag = true;
                  break;
                }
              }
            }
            if (flag)
              break;
          }
          if (flag)
            return this.parseMasterScheduleImpl("spring", this.getMasterScheduleEntry("spring"), visited);
          ++index1;
        }
      }
      else if (strArray1[0].Contains("MAIL"))
      {
        string id = ArgUtility.SplitBySpace(strArray1[0])[1];
        if (Game1.MasterPlayer.mailReceived.Contains(id) || NetWorldState.checkAnywhereForWorldStateID(id))
          index1 += 2;
        else
          ++index1;
      }
      if (strArray1[index1].Contains("GOTO"))
      {
        string currentSeason = ArgUtility.SplitBySpaceAndGet(strArray1[index1], 1);
        switch (currentSeason.ToLower())
        {
          case "season":
            currentSeason = Game1.currentSeason;
            break;
          case "no_schedule":
            this.followSchedule = false;
            return (Dictionary<int, SchedulePathDescription>) null;
        }
        return this.parseMasterScheduleImpl(currentSeason, this.getMasterScheduleEntry(currentSeason), visited);
      }
      Point point1 = this.isMarried() ? new Point(10, 23) : new Point((int) this.defaultPosition.X / 64 /*0x40*/, (int) this.defaultPosition.Y / 64 /*0x40*/);
      string startingLocation = this.isMarried() ? "BusStop" : this.defaultMap.Value;
      int val2 = 610;
      string targetLocationName = this.DefaultMap;
      int x = (int) ((double) this.defaultPosition.X / 64.0);
      int y = (int) ((double) this.defaultPosition.Y / 64.0);
      bool flag1 = false;
      for (int index3 = index1; index3 < strArray1.Length; ++index3)
      {
        int index4 = 0;
        string[] strArray3 = ArgUtility.SplitBySpace(strArray1[index3]);
        bool flag2 = false;
        string str1 = strArray3[index4];
        if (str1.Length > 0 && strArray3[index4][0] == 'a')
        {
          flag2 = true;
          str1 = str1.Substring(1);
        }
        int num1 = Convert.ToInt32(str1);
        int index5 = index4 + 1;
        string locationName = strArray3[index5];
        string endBehavior = (string) null;
        string endMessage = (string) null;
        int result1 = 0;
        int result2 = 0;
        int result3 = 2;
        int index6;
        if (locationName == "bed")
        {
          if (this.isMarried())
          {
            locationName = "BusStop";
            result1 = 9;
            result2 = 23;
            result3 = 3;
          }
          else
          {
            string rawScript = (string) null;
            if (this.hasMasterScheduleEntry("default"))
              rawScript = this.getMasterScheduleEntry("default");
            else if (this.hasMasterScheduleEntry("spring"))
              rawScript = this.getMasterScheduleEntry("spring");
            if (rawScript != null)
            {
              try
              {
                string[] strArray4 = NPC.SplitScheduleCommands(rawScript);
                string[] strArray5 = ArgUtility.SplitBySpace(strArray4[strArray4.Length - 1]);
                locationName = strArray5[1];
                if (strArray5.Length > 3)
                {
                  if (int.TryParse(strArray5[2], out result1))
                  {
                    if (int.TryParse(strArray5[3], out result2))
                      goto label_57;
                  }
                  rawScript = (string) null;
                }
                else
                  rawScript = (string) null;
              }
              catch (Exception ex)
              {
                rawScript = (string) null;
              }
            }
label_57:
            if (rawScript == null)
            {
              locationName = targetLocationName;
              result1 = x;
              result2 = y;
            }
          }
          index6 = index5 + 1;
          Dictionary<string, string> dictionary = DataLoader.AnimationDescriptions(Game1.content);
          string str2 = this.name.Value.ToLower() + "_sleep";
          string key = str2;
          if (dictionary.ContainsKey(key))
            endBehavior = str2;
        }
        else
        {
          if (int.TryParse(locationName, out int _))
          {
            locationName = startingLocation;
            --index5;
          }
          int index7 = index5 + 1;
          result1 = Convert.ToInt32(strArray3[index7]);
          int index8 = index7 + 1;
          result2 = Convert.ToInt32(strArray3[index8]);
          index6 = index8 + 1;
          try
          {
            if (strArray3.Length > index6)
            {
              if (int.TryParse(strArray3[index6], out result3))
                ++index6;
              else
                result3 = 2;
            }
          }
          catch (Exception ex)
          {
            result3 = 2;
          }
        }
        if (this.changeScheduleForLocationAccessibility(ref locationName, ref result1, ref result2, ref result3))
        {
          string str3 = this.getMasterScheduleRawData().ContainsKey("default") ? "default" : "spring";
          return this.parseMasterScheduleImpl(str3, this.getMasterScheduleEntry(str3), visited);
        }
        if (index6 < strArray3.Length)
        {
          if (strArray3[index6].Length > 0 && strArray3[index6][0] == '"')
          {
            endMessage = strArray1[index3].Substring(strArray1[index3].IndexOf('"'));
          }
          else
          {
            endBehavior = strArray3[index6];
            int index9 = index6 + 1;
            if (index9 < strArray3.Length && strArray3[index9].Length > 0 && strArray3[index9][0] == '"')
              endMessage = strArray1[index3].Substring(strArray1[index3].IndexOf('"')).Replace("\"", "");
          }
        }
        if (num1 == 0)
        {
          flag1 = true;
          targetLocationName = locationName;
          x = result1;
          y = result2;
          startingLocation = locationName;
          point1.X = result1;
          point1.Y = result2;
          this.faceDirection(result3);
          this.previousEndPoint = new Point(result1, result2);
        }
        else
        {
          SchedulePathDescription scheduleLocation = this.pathfindToNextScheduleLocation(scheduleKey, startingLocation, point1.X, point1.Y, locationName, result1, result2, result3, endBehavior, endMessage);
          if (flag2)
          {
            int num2 = 0;
            Point? nullable = new Point?();
            foreach (Point point2 in scheduleLocation.route)
            {
              if (!nullable.HasValue)
              {
                nullable = new Point?(point2);
              }
              else
              {
                if (Math.Abs(nullable.Value.X - point2.X) + Math.Abs(nullable.Value.Y - point2.Y) == 1)
                  num2 += 64 /*0x40*/;
                nullable = new Point?(point2);
              }
            }
            int num3 = (int) Math.Round((double) (num2 / 2) / (double) (Game1.realMilliSecondsPerGameTenMinutes / 1000 * 60)) * 10;
            num1 = Math.Max(Utility.ConvertMinutesToTime(Utility.ConvertTimeToMinutes(num1) - num3), val2);
          }
          scheduleLocation.time = num1;
          masterScheduleImpl.Add(num1, scheduleLocation);
          point1.X = result1;
          point1.Y = result2;
          startingLocation = locationName;
          val2 = num1;
        }
      }
      if (Game1.IsMasterGame & flag1)
        Game1.warpCharacter(this, targetLocationName, new Point(x, y));
      return masterScheduleImpl;
    }
    catch (Exception ex)
    {
      Game1.log.Error($"NPC '{this.Name}' failed to parse master schedule '{scheduleKey}' with raw data '{rawData}'.", ex);
      return new Dictionary<int, SchedulePathDescription>();
    }
  }

  /// <summary>Split a raw schedule script into its component commands.</summary>
  /// <param name="rawScript">The raw schedule script to split.</param>
  public static string[] SplitScheduleCommands(string rawScript)
  {
    return LegacyShims.SplitAndTrim(rawScript, '/', StringSplitOptions.RemoveEmptyEntries);
  }

  /// <summary>Try to load a schedule that applies today, or disable the schedule if none is found.</summary>
  /// <returns>Returns whether a schedule was successfully loaded.</returns>
  public bool TryLoadSchedule()
  {
    string currentSeason = Game1.currentSeason;
    int dayOfMonth = Game1.dayOfMonth;
    string key = Game1.shortDayNameFromDayOfSeason(dayOfMonth);
    int num = Math.Max(0, Utility.GetAllPlayerFriendshipLevel(this)) / 250;
    if (this.getMasterScheduleRawData() == null)
    {
      this.ClearSchedule();
      return false;
    }
    if (Game1.isGreenRain && Game1.year == 1 && this.TryLoadSchedule("GreenRain"))
      return true;
    if (!string.IsNullOrWhiteSpace(this.islandScheduleName.Value))
    {
      this.TryLoadSchedule(this.islandScheduleName.Value, this.Schedule);
      return true;
    }
    foreach (string activePassiveFestival in (IEnumerable<string>) Game1.netWorldState.Value.ActivePassiveFestivals)
    {
      int ofPassiveFestival = Utility.GetDayOfPassiveFestival(activePassiveFestival);
      if (this.isMarried())
      {
        if (this.TryLoadSchedule($"marriage_{activePassiveFestival}_{ofPassiveFestival.ToString()}") || this.TryLoadSchedule("marriage_" + activePassiveFestival))
          return true;
      }
      else if (this.TryLoadSchedule($"{activePassiveFestival}_{ofPassiveFestival.ToString()}") || this.TryLoadSchedule(activePassiveFestival))
        return true;
    }
    if (this.isMarried())
    {
      if (this.TryLoadSchedule($"marriage_{currentSeason}_{dayOfMonth.ToString()}") || (this.Name == "Penny" && (key == "Tue" || key == "Wed" || key == "Fri") || this.Name == "Maru" && (key == "Tue" || key == "Thu") || this.Name == "Harvey" && (key == "Tue" || key == "Thu")) && this.TryLoadSchedule("marriageJob") || !Game1.isRaining && this.TryLoadSchedule("marriage_" + key))
        return true;
      this.ClearSchedule();
      return false;
    }
    if (this.TryLoadSchedule($"{currentSeason}_{dayOfMonth.ToString()}"))
      return true;
    for (int index = num; index > 0; --index)
    {
      if (this.TryLoadSchedule($"{dayOfMonth.ToString()}_{index.ToString()}"))
        return true;
    }
    if (this.TryLoadSchedule(dayOfMonth.ToString()) || this.Name == "Pam" && Game1.player.mailReceived.Contains("ccVault") && this.TryLoadSchedule("bus"))
      return true;
    bool? nullable = this.currentLocation?.IsRainingHere();
    if (nullable.HasValue && nullable.GetValueOrDefault() && (Game1.random.NextBool() && this.TryLoadSchedule("rain2") || this.TryLoadSchedule("rain")))
      return true;
    for (int index = num; index > 0; index = index - 1 - 1)
    {
      if (this.TryLoadSchedule($"{currentSeason}_{key}_{index.ToString()}"))
        return true;
    }
    if (this.TryLoadSchedule($"{currentSeason}_{key}"))
      return true;
    for (int index = num; index > 0; --index)
    {
      if (this.TryLoadSchedule($"{key}_{index.ToString()}"))
        return true;
      --index;
    }
    if (this.TryLoadSchedule(key) || this.TryLoadSchedule(currentSeason) || this.TryLoadSchedule("spring_" + key) || this.TryLoadSchedule("spring"))
      return true;
    this.ClearSchedule();
    return false;
  }

  /// <summary>Try to load a schedule matching the the given key, or disable the schedule if it's missing or invalid.</summary>
  /// <param name="key">The key for the schedule to load.</param>
  /// <returns>Returns whether the schedule was successfully loaded.</returns>
  public bool TryLoadSchedule(string key)
  {
    try
    {
      if (this.hasMasterScheduleEntry(key))
      {
        this.TryLoadSchedule(key, this.parseMasterSchedule(key, this.getMasterScheduleEntry(key)));
        return true;
      }
    }
    catch (Exception ex)
    {
      Game1.log.Error($"Failed to load schedule key '{key}' for NPC '{this.Name}'.", ex);
    }
    this.ClearSchedule();
    return false;
  }

  /// <summary>Try to load a raw schedule script, or disable the schedule if it's invalid.</summary>
  /// <param name="key">The schedule's key in the data asset.</param>
  /// <param name="rawSchedule">The schedule script to load.</param>
  public bool TryLoadSchedule(string key, string rawSchedule)
  {
    Dictionary<int, SchedulePathDescription> masterSchedule;
    try
    {
      masterSchedule = this.parseMasterSchedule(key, rawSchedule);
    }
    catch (Exception ex)
    {
      Game1.log.Error($"Failed to load schedule key '{key}' from raw string for NPC '{this.Name}'.", ex);
      this.ClearSchedule();
      return false;
    }
    return this.TryLoadSchedule(key, masterSchedule);
  }

  /// <summary>Try to load raw schedule data, or disable the schedule if it's invalid.</summary>
  /// <param name="key">The schedule's key in the data asset.</param>
  /// <param name="schedule">The schedule data to load.</param>
  public bool TryLoadSchedule(string key, Dictionary<int, SchedulePathDescription> schedule)
  {
    if (schedule == null)
    {
      this.ClearSchedule();
      return false;
    }
    this.Schedule = schedule;
    if (Game1.IsMasterGame)
      this.dayScheduleName.Value = key;
    this.followSchedule = true;
    return true;
  }

  /// <summary>Disable the schedule for today.</summary>
  public void ClearSchedule()
  {
    this.Schedule = (Dictionary<int, SchedulePathDescription>) null;
    if (Game1.IsMasterGame)
      this.dayScheduleName.Value = (string) null;
    this.followSchedule = false;
  }

  public virtual void handleMasterScheduleFileLoadError(Exception e)
  {
    Game1.log.Error($"NPC '{this.Name}' failed loading schedule file.", e);
  }

  public virtual void InvalidateMasterSchedule() => this._hasLoadedMasterScheduleData = false;

  public Dictionary<string, string> getMasterScheduleRawData()
  {
    if (!this._hasLoadedMasterScheduleData)
    {
      this._hasLoadedMasterScheduleData = true;
      string assetName = "Characters\\schedules\\" + this.Name;
      if (this.Name == "Leo" && this.DefaultMap != "IslandHut")
        assetName += "Mainland";
      try
      {
        if (Game1.content.DoesAssetExist<Dictionary<string, string>>(assetName))
        {
          this._masterScheduleData = Game1.content.Load<Dictionary<string, string>>(assetName);
          this._masterScheduleData = new Dictionary<string, string>((IDictionary<string, string>) this._masterScheduleData, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        }
      }
      catch (Exception ex)
      {
        this.handleMasterScheduleFileLoadError(ex);
      }
    }
    return this._masterScheduleData;
  }

  public string getMasterScheduleEntry(string schedule_key)
  {
    if (this.getMasterScheduleRawData() == null)
      throw new KeyNotFoundException($"The schedule file for NPC '{this.Name}' could not be loaded...");
    string masterScheduleEntry;
    if (this._masterScheduleData.TryGetValue(schedule_key, out masterScheduleEntry))
      return masterScheduleEntry;
    throw new KeyNotFoundException($"The schedule file for NPC '{this.Name}' has no schedule named '{schedule_key}'.");
  }

  public bool hasMasterScheduleEntry(string key)
  {
    return this.getMasterScheduleRawData() != null && this.getMasterScheduleRawData().ContainsKey(key);
  }

  public virtual bool isRoommate()
  {
    if (!this.IsVillager)
      return false;
    foreach (Farmer allFarmer in Game1.getAllFarmers())
    {
      if (allFarmer.spouse != null && allFarmer.spouse == this.Name && !allFarmer.isEngaged() && allFarmer.isRoommate(this.Name))
        return true;
    }
    return false;
  }

  public bool isMarried()
  {
    if (!this.IsVillager)
      return false;
    foreach (Farmer allFarmer in Game1.getAllFarmers())
    {
      if (allFarmer.spouse != null && allFarmer.spouse == this.Name && !allFarmer.isEngaged())
        return true;
    }
    return false;
  }

  public bool isMarriedOrEngaged()
  {
    if (!this.IsVillager)
      return false;
    foreach (Farmer allFarmer in Game1.getAllFarmers())
    {
      if (allFarmer.spouse != null && allFarmer.spouse == this.Name)
        return true;
    }
    return false;
  }

  /// <summary>Update the NPC state when setting up the new day, before the game saves overnight.</summary>
  /// <param name="dayOfMonth">The current day of month.</param>
  /// <remarks>See also <see cref="M:StardewValley.NPC.OnDayStarted" />, which happens after saving when the day has started.</remarks>
  public virtual void dayUpdate(int dayOfMonth)
  {
    bool isVillager = this.IsVillager;
    this.isMovingOnPathFindPath.Value = false;
    this.queuedSchedulePaths.Clear();
    this.lastAttemptedSchedule = -1;
    this.drawOffset = Vector2.Zero;
    this.appliedRouteAnimationOffset = Vector2.Zero;
    this.shouldWearIslandAttire.Value = false;
    if (this.layingDown)
    {
      this.layingDown = false;
      this.HideShadow = false;
    }
    if (this.isWearingIslandAttire)
      this.wearNormalClothes();
    if (this.currentLocation != null)
    {
      if (this.defaultMap.Value != null)
      {
        try
        {
          Game1.warpCharacter(this, this.defaultMap.Value, this.defaultPosition.Value / 64f);
        }
        catch (Exception ex)
        {
          Game1.log.Error($"NPC '{this.Name}' failed to warp home to '{this.defaultMap}' overnight.", ex);
        }
      }
    }
    if (isVillager)
    {
      switch (this.Name)
      {
        case "Willy":
          this.IsInvisible = false;
          break;
        case "Elliott":
          if (Game1.IsMasterGame && Game1.netWorldState.Value.hasWorldStateID("elliottGone"))
          {
            this.daysUntilNotInvisible = 7;
            Game1.netWorldState.Value.removeWorldStateID("elliottGone");
            Game1.worldStateIDs.Remove("elliottGone");
            break;
          }
          break;
      }
    }
    this.UpdateInvisibilityOnNewDay();
    this.resetForNewDay(dayOfMonth);
    this.ChooseAppearance();
    if (isVillager)
      this.updateConstructionAnimation();
    this.clearTextAboveHead();
  }

  /// <summary>Handle the new day starting after the player saves, loads, or connects.</summary>
  /// <remarks>See also <see cref="M:StardewValley.NPC.dayUpdate(System.Int32)" />, which happens while setting up the day before saving.</remarks>
  public void OnDayStarted()
  {
    if (!Game1.IsMasterGame || !this.isMarried() || this.getSpouse().divorceTonight.Value || this.IsInvisible)
      return;
    this.marriageDuties();
  }

  protected void UpdateInvisibilityOnNewDay()
  {
    if (!Game1.IsMasterGame || !this.IsInvisible && this.daysUntilNotInvisible <= 0)
      return;
    --this.daysUntilNotInvisible;
    this.IsInvisible = this.daysUntilNotInvisible > 0;
    if (this.IsInvisible)
      return;
    this.daysUntilNotInvisible = 0;
  }

  public virtual void resetForNewDay(int dayOfMonth)
  {
    this.sleptInBed.Value = true;
    if (this.isMarried() && !this.isRoommate())
    {
      FarmHouse homeOfFarmer = Utility.getHomeOfFarmer(this.getSpouse());
      if (homeOfFarmer != null && homeOfFarmer.GetSpouseBed() == null)
        this.sleptInBed.Value = false;
    }
    if (this.doingEndOfRouteAnimation.Value)
      this.routeEndAnimationFinished((Farmer) null);
    this.Halt();
    this.wasKissedYesterday = this.hasBeenKissedToday.Value;
    this.hasBeenKissedToday.Value = false;
    this.currentMarriageDialogue.Clear();
    this.marriageDefaultDialogue.Value = (MarriageDialogueReference) null;
    this.shouldSayMarriageDialogue.Value = false;
    this.isSleeping.Value = false;
    this.drawOffset = Vector2.Zero;
    this.faceTowardFarmer = false;
    this.faceTowardFarmerTimer = 0;
    this.drawOffset = Vector2.Zero;
    this.hasSaidAfternoonDialogue.Value = false;
    this.isPlayingSleepingAnimation = false;
    this.ignoreScheduleToday = false;
    this.Halt();
    this.controller = (PathFindController) null;
    this.temporaryController = (PathFindController) null;
    this.directionsToNewLocation = (SchedulePathDescription) null;
    this.faceDirection(this.DefaultFacingDirection);
    this.Sprite.oldFrame = this.Sprite.CurrentFrame;
    this.previousEndPoint = new Point((int) this.defaultPosition.X / 64 /*0x40*/, (int) this.defaultPosition.Y / 64 /*0x40*/);
    this.isWalkingInSquare = false;
    this.returningToEndPoint = false;
    this.lastCrossroad = Microsoft.Xna.Framework.Rectangle.Empty;
    this._startedEndOfRouteBehavior = (string) null;
    this._finishingEndOfRouteBehavior = (string) null;
    this.loadedEndOfRouteBehavior = (string) null;
    this._beforeEndOfRouteAnimationFrame = this.Sprite.CurrentFrame;
    if (this.IsVillager)
    {
      if (this.Name == "Willy" && Game1.stats.DaysPlayed < 2U)
      {
        this.IsInvisible = true;
        this.daysUntilNotInvisible = 1;
      }
      this.TryLoadSchedule();
      this.performSpecialScheduleChanges();
    }
    this.endOfRouteMessage.Value = (string) null;
  }

  public void returnHomeFromFarmPosition(Farm farm)
  {
    Farmer spouse = this.getSpouse();
    if (spouse == null)
      return;
    FarmHouse homeOfFarmer = Utility.getHomeOfFarmer(spouse);
    if (this.TilePoint == homeOfFarmer.getPorchStandingSpot())
    {
      this.drawOffset = Vector2.Zero;
      string nameOrUniqueName = this.getHome().NameOrUniqueName;
      this.willDestroyObjectsUnderfoot = true;
      Point warpPointTo = farm.getWarpPointTo(nameOrUniqueName, (Character) this);
      this.controller = new PathFindController((Character) this, (GameLocation) farm, warpPointTo, 0)
      {
        NPCSchedule = true
      };
    }
    else
    {
      if (this.shouldPlaySpousePatioAnimation.Value && farm.farmers.Any())
        return;
      this.drawOffset = Vector2.Zero;
      this.Halt();
      this.controller = (PathFindController) null;
      this.temporaryController = (PathFindController) null;
      this.ignoreScheduleToday = true;
      Game1.warpCharacter(this, (GameLocation) homeOfFarmer, Utility.PointToVector2(homeOfFarmer.getKitchenStandingSpot()));
    }
  }

  public virtual Vector2 GetSpousePatioPosition()
  {
    return Utility.PointToVector2(Game1.getFarm().spousePatioSpot);
  }

  public void setUpForOutdoorPatioActivity()
  {
    Vector2 spousePatioPosition = this.GetSpousePatioPosition();
    if (NPC.checkTileOccupancyForSpouse((GameLocation) Game1.getFarm(), spousePatioPosition))
      return;
    Game1.warpCharacter(this, "Farm", spousePatioPosition);
    this.popOffAnyNonEssentialItems();
    this.currentMarriageDialogue.Clear();
    this.addMarriageDialogue("MarriageDialogue", "patio_" + this.Name);
    this.setTilePosition((int) spousePatioPosition.X, (int) spousePatioPosition.Y);
    this.shouldPlaySpousePatioAnimation.Value = true;
  }

  private void doPlaySpousePatioAnimation()
  {
    CharacterSpousePatioData spousePatio = this.GetData()?.SpousePatio;
    if (spousePatio == null)
      return;
    List<int[]> spriteAnimationFrames = spousePatio.SpriteAnimationFrames;
    // ISSUE: explicit non-virtual call
    if (spriteAnimationFrames == null || __nonvirtual (spriteAnimationFrames.Count) <= 0)
      return;
    this.drawOffset = Utility.PointToVector2(spousePatio.SpriteAnimationPixelOffset);
    this.Sprite.ClearAnimation();
    for (int index = 0; index < spriteAnimationFrames.Count; ++index)
    {
      int[] array = spriteAnimationFrames[index];
      if (array != null && array.Length != 0)
        this.Sprite.AddFrame(new FarmerSprite.AnimationFrame(array[0], ArgUtility.HasIndex<int>(array, 1) ? array[1] : 100, 0, false, false));
    }
  }

  /// <summary>Whether this character has dark skin for the purposes of child genetics.</summary>
  public virtual bool hasDarkSkin()
  {
    if (!this.IsVillager)
      return false;
    CharacterData data = this.GetData();
    return data != null && data.IsDarkSkinned;
  }

  /// <summary>Whether the player will need to adopt children with this spouse, instead of either the player or NPC giving birth.</summary>
  public bool isAdoptionSpouse()
  {
    Farmer spouse = this.getSpouse();
    if (spouse == null)
      return false;
    string spouseAdopts = this.GetData()?.SpouseAdopts;
    return spouseAdopts != null ? GameStateQuery.CheckConditions(spouseAdopts, this.currentLocation, spouse) : this.Gender == spouse.Gender;
  }

  public bool canGetPregnant()
  {
    if (this is Horse || this.Name.Equals("Krobus") || this.isRoommate() || this.IsInvisible)
      return false;
    Farmer spouse = this.getSpouse();
    if (spouse == null || spouse.divorceTonight.Value)
      return false;
    int heartLevelForNpc = spouse.getFriendshipHeartLevelForNPC(this.Name);
    Friendship spouseFriendship = spouse.GetSpouseFriendship();
    List<Child> children = spouse.getChildren();
    this.defaultMap.Value = spouse.homeLocation.Value;
    FarmHouse homeOfFarmer = Utility.getHomeOfFarmer(spouse);
    if (homeOfFarmer.cribStyle.Value <= 0 || homeOfFarmer.upgradeLevel < 2 || spouseFriendship.DaysUntilBirthing >= 0 || heartLevelForNpc < 10 || spouse.GetDaysMarried() < 7)
      return false;
    if (children.Count == 0)
      return true;
    return children.Count < 2 && children[0].Age > 2;
  }

  public void marriageDuties()
  {
    Farmer spouse = this.getSpouse();
    if (spouse == null)
      return;
    this.shouldSayMarriageDialogue.Value = true;
    this.DefaultMap = spouse.homeLocation.Value;
    FarmHouse farmHouse = Game1.RequireLocation<FarmHouse>(spouse.homeLocation.Value);
    Random daySaveRandom = Utility.CreateDaySaveRandom((double) spouse.UniqueMultiplayerID);
    int heartLevelForNpc = spouse.getFriendshipHeartLevelForNPC(this.Name);
    if (Game1.IsMasterGame && (this.currentLocation == null || !this.currentLocation.Equals((GameLocation) farmHouse)))
      Game1.warpCharacter(this, spouse.homeLocation.Value, farmHouse.getSpouseBedSpot(this.Name));
    int num1;
    if (Game1.isRaining)
    {
      NetRef<MarriageDialogueReference> marriageDefaultDialogue = this.marriageDefaultDialogue;
      num1 = daySaveRandom.Next(5);
      MarriageDialogueReference dialogueReference = new MarriageDialogueReference("MarriageDialogue", "Rainy_Day_" + num1.ToString(), false, Array.Empty<string>());
      marriageDefaultDialogue.Value = dialogueReference;
    }
    else
    {
      NetRef<MarriageDialogueReference> marriageDefaultDialogue = this.marriageDefaultDialogue;
      num1 = daySaveRandom.Next(5);
      MarriageDialogueReference dialogueReference = new MarriageDialogueReference("MarriageDialogue", "Indoor_Day_" + num1.ToString(), false, Array.Empty<string>());
      marriageDefaultDialogue.Value = dialogueReference;
    }
    this.currentMarriageDialogue.Add(new MarriageDialogueReference(this.marriageDefaultDialogue.Value.DialogueFile, this.marriageDefaultDialogue.Value.DialogueKey, this.marriageDefaultDialogue.Value.IsGendered, this.marriageDefaultDialogue.Value.Substitutions));
    if (spouse.GetSpouseFriendship().DaysUntilBirthing == 0)
    {
      this.setTilePosition(farmHouse.getKitchenStandingSpot());
      this.currentMarriageDialogue.Clear();
    }
    else
    {
      if (this.daysAfterLastBirth >= 0)
      {
        --this.daysAfterLastBirth;
        num1 = this.getSpouse().getChildrenCount();
        switch (num1)
        {
          case 1:
            this.setTilePosition(farmHouse.getKitchenStandingSpot());
            if (this.spouseObstacleCheck(new MarriageDialogueReference("Strings\\StringsFromCSFiles", "NPC.cs.4406", false, Array.Empty<string>()), (GameLocation) farmHouse))
              return;
            this.currentMarriageDialogue.Clear();
            this.addMarriageDialogue("MarriageDialogue", "OneKid_" + daySaveRandom.Next(4).ToString());
            return;
          case 2:
            this.setTilePosition(farmHouse.getKitchenStandingSpot());
            if (this.spouseObstacleCheck(new MarriageDialogueReference("Strings\\StringsFromCSFiles", "NPC.cs.4406", false, Array.Empty<string>()), (GameLocation) farmHouse))
              return;
            this.currentMarriageDialogue.Clear();
            this.addMarriageDialogue("MarriageDialogue", "TwoKids_" + daySaveRandom.Next(4).ToString());
            return;
        }
      }
      this.setTilePosition(farmHouse.getKitchenStandingSpot());
      if (!this.sleptInBed.Value)
      {
        this.currentMarriageDialogue.Clear();
        num1 = daySaveRandom.Next(4);
        this.addMarriageDialogue("MarriageDialogue", "NoBed_" + num1.ToString());
      }
      else if (this.tryToGetMarriageSpecificDialogue($"{Game1.currentSeason}_{Game1.dayOfMonth.ToString()}") != null)
      {
        if (spouse == null)
          return;
        this.currentMarriageDialogue.Clear();
        this.addMarriageDialogue("MarriageDialogue", $"{Game1.currentSeason}_{Game1.dayOfMonth.ToString()}");
      }
      else if (this.Schedule != null)
      {
        if (this.ScheduleKey == "marriage_" + Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth))
        {
          this.currentMarriageDialogue.Clear();
          this.addMarriageDialogue("MarriageDialogue", "funLeave_" + this.Name);
        }
        else
        {
          if (!(this.ScheduleKey == "marriageJob"))
            return;
          this.currentMarriageDialogue.Clear();
          this.addMarriageDialogue("MarriageDialogue", "jobLeave_" + this.Name);
        }
      }
      else if (!Game1.isRaining && !Game1.IsWinter && Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth).Equals("Sat") && spouse == Game1.MasterPlayer && !this.Name.Equals("Krobus"))
      {
        this.setUpForOutdoorPatioActivity();
      }
      else
      {
        int num2 = 12;
        int totalDays1 = Game1.Date.TotalDays;
        int? totalDays2 = spouse.GetSpouseFriendship().LastGiftDate?.TotalDays;
        int? nullable = totalDays2.HasValue ? new int?(totalDays1 - totalDays2.GetValueOrDefault()) : new int?();
        num1 = 1;
        if (nullable.GetValueOrDefault() <= num1 & nullable.HasValue)
          --num2;
        if (this.wasKissedYesterday)
          --num2;
        if (spouse.GetDaysMarried() > 7 && daySaveRandom.NextDouble() < 1.0 - (double) Math.Max(1, heartLevelForNpc) / (double) num2)
        {
          Furniture randomFurniture = farmHouse.getRandomFurniture(daySaveRandom);
          if (randomFurniture != null && randomFurniture.isGroundFurniture() && randomFurniture.furniture_type.Value != 15 && randomFurniture.furniture_type.Value != 12)
          {
            Point p = new Point((int) randomFurniture.tileLocation.X - 1, (int) randomFurniture.tileLocation.Y);
            if (farmHouse.CanItemBePlacedHere(new Vector2((float) p.X, (float) p.Y)))
            {
              this.setTilePosition(p);
              this.faceDirection(1);
              num1 = daySaveRandom.Next(10);
              switch (num1)
              {
                case 0:
                  this.currentMarriageDialogue.Clear();
                  this.addMarriageDialogue("Strings\\StringsFromCSFiles", "NPC.cs.4420");
                  return;
                case 1:
                  this.currentMarriageDialogue.Clear();
                  this.addMarriageDialogue("Strings\\StringsFromCSFiles", "NPC.cs.4421");
                  return;
                case 2:
                  this.currentMarriageDialogue.Clear();
                  this.addMarriageDialogue("Strings\\StringsFromCSFiles", "NPC.cs.4422", true);
                  return;
                case 3:
                  this.currentMarriageDialogue.Clear();
                  this.addMarriageDialogue("Strings\\StringsFromCSFiles", "NPC.cs.4423");
                  return;
                case 4:
                  this.currentMarriageDialogue.Clear();
                  this.addMarriageDialogue("Strings\\StringsFromCSFiles", "NPC.cs.4424");
                  return;
                case 5:
                  this.currentMarriageDialogue.Clear();
                  this.addMarriageDialogue("Strings\\StringsFromCSFiles", "NPC.cs.4425");
                  return;
                case 6:
                  this.currentMarriageDialogue.Clear();
                  this.addMarriageDialogue("Strings\\StringsFromCSFiles", "NPC.cs.4426");
                  return;
                case 7:
                  if (this.Gender == Gender.Female)
                  {
                    this.currentMarriageDialogue.Clear();
                    this.addMarriageDialogue("Strings\\StringsFromCSFiles", daySaveRandom.Choose<string>("NPC.cs.4427", "NPC.cs.4429"));
                    return;
                  }
                  this.currentMarriageDialogue.Clear();
                  this.addMarriageDialogue("Strings\\StringsFromCSFiles", "NPC.cs.4431");
                  return;
                case 8:
                  this.currentMarriageDialogue.Clear();
                  this.addMarriageDialogue("Strings\\StringsFromCSFiles", "NPC.cs.4432");
                  return;
                case 9:
                  this.currentMarriageDialogue.Clear();
                  this.addMarriageDialogue("Strings\\StringsFromCSFiles", "NPC.cs.4433");
                  return;
                default:
                  return;
              }
            }
          }
          this.spouseObstacleCheck(new MarriageDialogueReference("Strings\\StringsFromCSFiles", "NPC.cs.4406", false, Array.Empty<string>()), (GameLocation) farmHouse, true);
        }
        else
        {
          Friendship spouseFriendship = spouse.GetSpouseFriendship();
          if (spouseFriendship.DaysUntilBirthing != -1 && spouseFriendship.DaysUntilBirthing <= 7 && daySaveRandom.NextBool())
          {
            if (this.isAdoptionSpouse())
            {
              this.setTilePosition(farmHouse.getKitchenStandingSpot());
              if (this.spouseObstacleCheck(new MarriageDialogueReference("Strings\\StringsFromCSFiles", "NPC.cs.4439", false, Array.Empty<string>()), (GameLocation) farmHouse))
                return;
              if (daySaveRandom.NextBool())
                this.currentMarriageDialogue.Clear();
              if (daySaveRandom.NextBool())
                this.addMarriageDialogue("Strings\\StringsFromCSFiles", "NPC.cs.4440", false, this.getSpouse().displayName);
              else
                this.addMarriageDialogue("Strings\\StringsFromCSFiles", "NPC.cs.4441", false, "%endearment");
            }
            else if (this.Gender == Gender.Female)
            {
              this.setTilePosition(farmHouse.getKitchenStandingSpot());
              if (this.spouseObstacleCheck(daySaveRandom.NextBool() ? new MarriageDialogueReference("Strings\\StringsFromCSFiles", "NPC.cs.4442", false, Array.Empty<string>()) : new MarriageDialogueReference("Strings\\StringsFromCSFiles", "NPC.cs.4443", false, Array.Empty<string>()), (GameLocation) farmHouse))
                return;
              if (daySaveRandom.NextBool())
                this.currentMarriageDialogue.Clear();
              NetList<MarriageDialogueReference, NetRef<MarriageDialogueReference>> marriageDialogue = this.currentMarriageDialogue;
              MarriageDialogueReference dialogueReference;
              if (!daySaveRandom.NextBool())
                dialogueReference = new MarriageDialogueReference("Strings\\StringsFromCSFiles", "NPC.cs.4445", false, new string[1]
                {
                  "%endearment"
                });
              else
                dialogueReference = new MarriageDialogueReference("Strings\\StringsFromCSFiles", "NPC.cs.4444", false, new string[1]
                {
                  this.getSpouse().displayName
                });
              marriageDialogue.Add(dialogueReference);
            }
            else
            {
              this.setTilePosition(farmHouse.getKitchenStandingSpot());
              if (this.spouseObstacleCheck(new MarriageDialogueReference("Strings\\StringsFromCSFiles", "NPC.cs.4446", true, Array.Empty<string>()), (GameLocation) farmHouse))
                return;
              if (daySaveRandom.NextBool())
                this.currentMarriageDialogue.Clear();
              NetList<MarriageDialogueReference, NetRef<MarriageDialogueReference>> marriageDialogue = this.currentMarriageDialogue;
              MarriageDialogueReference dialogueReference;
              if (!daySaveRandom.NextBool())
                dialogueReference = new MarriageDialogueReference("Strings\\StringsFromCSFiles", "NPC.cs.4448", false, new string[1]
                {
                  "%endearment"
                });
              else
                dialogueReference = new MarriageDialogueReference("Strings\\StringsFromCSFiles", "NPC.cs.4447", true, new string[1]
                {
                  this.getSpouse().displayName
                });
              marriageDialogue.Add(dialogueReference);
            }
          }
          else
          {
            if (daySaveRandom.NextDouble() < 0.07)
            {
              num1 = this.getSpouse().getChildrenCount();
              switch (num1)
              {
                case 1:
                  this.setTilePosition(farmHouse.getKitchenStandingSpot());
                  if (this.spouseObstacleCheck(new MarriageDialogueReference("Strings\\StringsFromCSFiles", "NPC.cs.4449", true, Array.Empty<string>()), (GameLocation) farmHouse))
                    return;
                  this.currentMarriageDialogue.Clear();
                  this.addMarriageDialogue("MarriageDialogue", "OneKid_" + daySaveRandom.Next(4).ToString());
                  return;
                case 2:
                  this.setTilePosition(farmHouse.getKitchenStandingSpot());
                  if (this.spouseObstacleCheck(new MarriageDialogueReference("Strings\\StringsFromCSFiles", "NPC.cs.4452", true, Array.Empty<string>()), (GameLocation) farmHouse))
                    return;
                  this.currentMarriageDialogue.Clear();
                  this.addMarriageDialogue("MarriageDialogue", "TwoKids_" + daySaveRandom.Next(4).ToString());
                  return;
              }
            }
            Farm farm = Game1.getFarm();
            if (this.currentMarriageDialogue.Count > 0 && this.currentMarriageDialogue[0].IsItemGrabDialogue(this))
            {
              this.setTilePosition(farmHouse.getKitchenStandingSpot());
              this.spouseObstacleCheck(new MarriageDialogueReference("Strings\\StringsFromCSFiles", "NPC.cs.4455", true, Array.Empty<string>()), (GameLocation) farmHouse);
            }
            else if (!Game1.isRaining && daySaveRandom.NextDouble() < 0.4 && !NPC.checkTileOccupancyForSpouse((GameLocation) farm, Utility.PointToVector2(farmHouse.getPorchStandingSpot())) && !this.Name.Equals("Krobus"))
            {
              bool flag1 = false;
              if (!NPC.hasSomeoneFedThePet)
              {
                foreach (Building building in farm.buildings)
                {
                  if (building is PetBowl petBowl && !petBowl.watered.Value)
                  {
                    flag1 = true;
                    petBowl.watered.Value = true;
                    NPC.hasSomeoneFedThePet = true;
                  }
                }
              }
              if (daySaveRandom.NextDouble() < 0.6 && Game1.season != Season.Winter && !NPC.hasSomeoneWateredCrops)
              {
                Vector2 vector2 = Vector2.Zero;
                int num3 = 0;
                bool flag2 = false;
                for (; num3 < Math.Min(50, farm.terrainFeatures.Length) && vector2.Equals(Vector2.Zero); ++num3)
                {
                  Vector2 key;
                  TerrainFeature terrainFeature;
                  if (Utility.TryGetRandom<Vector2, TerrainFeature, NetRef<TerrainFeature>, SerializableDictionary<Vector2, TerrainFeature>, NetVector2Dictionary<TerrainFeature, NetRef<TerrainFeature>>>((NetDictionary<Vector2, TerrainFeature, NetRef<TerrainFeature>, SerializableDictionary<Vector2, TerrainFeature>, NetVector2Dictionary<TerrainFeature, NetRef<TerrainFeature>>>) farm.terrainFeatures, out key, out terrainFeature) && terrainFeature is HoeDirt hoeDirt && hoeDirt.needsWatering())
                  {
                    if (!hoeDirt.isWatered())
                      vector2 = key;
                    else
                      flag2 = true;
                  }
                }
                if (!vector2.Equals(Vector2.Zero))
                {
                  foreach (Vector2 vector in new Microsoft.Xna.Framework.Rectangle((int) vector2.X - 30, (int) vector2.Y - 30, 60, 60).GetVectors())
                  {
                    TerrainFeature terrainFeature;
                    if (farm.isTileOnMap(vector) && farm.terrainFeatures.TryGetValue(vector, out terrainFeature) && terrainFeature is HoeDirt hoeDirt && Game1.IsMasterGame && hoeDirt.needsWatering())
                      hoeDirt.state.Value = 1;
                  }
                  this.faceDirection(2);
                  this.currentMarriageDialogue.Clear();
                  this.addMarriageDialogue("Strings\\StringsFromCSFiles", "NPC.cs.4462", true);
                  if (flag1)
                  {
                    if (Utility.getAllPets().Count > 1 && Game1.content.GetCurrentLanguage() == LocalizedContentManager.LanguageCode.en)
                      this.addMarriageDialogue("Strings\\StringsFromCSFiles", "MultiplePetBowls_watered", false, Game1.player.getPetDisplayName());
                    else
                      this.addMarriageDialogue("Strings\\StringsFromCSFiles", "NPC.cs.4463", false, Game1.player.getPetDisplayName());
                  }
                  this.addMarriageDialogue("MarriageDialogue", "Outdoor_" + daySaveRandom.Next(5).ToString());
                  NPC.hasSomeoneWateredCrops = true;
                }
                else
                {
                  this.faceDirection(2);
                  if (flag2)
                  {
                    this.currentMarriageDialogue.Clear();
                    if (Game1.gameMode == (byte) 6)
                    {
                      if (daySaveRandom.NextBool())
                      {
                        this.addMarriageDialogue("Strings\\StringsFromCSFiles", "NPC.cs.4465", false, "%endearment");
                      }
                      else
                      {
                        this.addMarriageDialogue("Strings\\StringsFromCSFiles", "NPC.cs.4466", false, "%endearment");
                        this.addMarriageDialogue("Strings\\StringsFromCSFiles", "NPC.cs.4462", true);
                        if (flag1)
                        {
                          if (Utility.getAllPets().Count > 1 && Game1.content.GetCurrentLanguage() == LocalizedContentManager.LanguageCode.en)
                            this.addMarriageDialogue("Strings\\StringsFromCSFiles", "MultiplePetBowls_watered", false, Game1.player.getPetDisplayName());
                          else
                            this.addMarriageDialogue("Strings\\StringsFromCSFiles", "NPC.cs.4463", false, Game1.player.getPetDisplayName());
                        }
                      }
                    }
                    else
                    {
                      this.currentMarriageDialogue.Clear();
                      this.addMarriageDialogue("Strings\\StringsFromCSFiles", "NPC.cs.4470", true);
                    }
                  }
                  else
                  {
                    this.currentMarriageDialogue.Clear();
                    num1 = daySaveRandom.Next(5);
                    this.addMarriageDialogue("MarriageDialogue", "Outdoor_" + num1.ToString());
                  }
                }
              }
              else if (daySaveRandom.NextDouble() < 0.6 && !NPC.hasSomeoneFedTheAnimals)
              {
                bool flag3 = false;
                foreach (Building building in farm.buildings)
                {
                  if (building.GetIndoors() is AnimalHouse indoors && building.daysOfConstructionLeft.Value <= 0 && Game1.IsMasterGame)
                  {
                    indoors.feedAllAnimals();
                    flag3 = true;
                  }
                }
                this.faceDirection(2);
                if (flag3)
                {
                  NPC.hasSomeoneFedTheAnimals = true;
                  this.currentMarriageDialogue.Clear();
                  this.addMarriageDialogue("Strings\\StringsFromCSFiles", "NPC.cs.4474", true);
                  if (flag1)
                  {
                    if (Utility.getAllPets().Count > 1 && Game1.content.GetCurrentLanguage() == LocalizedContentManager.LanguageCode.en)
                      this.addMarriageDialogue("Strings\\StringsFromCSFiles", "MultiplePetBowls_watered", false, Game1.player.getPetDisplayName());
                    else
                      this.addMarriageDialogue("Strings\\StringsFromCSFiles", "NPC.cs.4463", false, Game1.player.getPetDisplayName());
                  }
                  this.addMarriageDialogue("MarriageDialogue", "Outdoor_" + daySaveRandom.Next(5).ToString());
                }
                else
                {
                  this.currentMarriageDialogue.Clear();
                  this.addMarriageDialogue("MarriageDialogue", "Outdoor_" + daySaveRandom.Next(5).ToString());
                }
                if (Game1.IsMasterGame)
                {
                  foreach (Building building in farm.buildings)
                  {
                    if (building is PetBowl petBowl && !petBowl.watered.Value)
                    {
                      petBowl.watered.Value = true;
                      NPC.hasSomeoneFedThePet = true;
                    }
                  }
                }
              }
              else if (!NPC.hasSomeoneRepairedTheFences)
              {
                int num4 = 0;
                this.faceDirection(2);
                Vector2 vector2;
                for (vector2 = Vector2.Zero; num4 < Math.Min(50, farm.objects.Length) && vector2.Equals(Vector2.Zero); ++num4)
                {
                  Vector2 key;
                  Object @object;
                  if (Utility.TryGetRandom(farm.objects, out key, out @object) && @object is Fence)
                    vector2 = key;
                }
                if (!vector2.Equals(Vector2.Zero))
                {
                  foreach (Vector2 vector in new Microsoft.Xna.Framework.Rectangle((int) vector2.X - 10, (int) vector2.Y - 10, 20, 20).GetVectors())
                  {
                    Object @object;
                    if (farm.isTileOnMap(vector) && farm.objects.TryGetValue(vector, out @object) && @object is Fence fence && Game1.IsMasterGame)
                      fence.repair();
                  }
                  this.currentMarriageDialogue.Clear();
                  this.addMarriageDialogue("Strings\\StringsFromCSFiles", "NPC.cs.4481", true);
                  if (flag1)
                  {
                    if (Utility.getAllPets().Count > 1 && Game1.content.GetCurrentLanguage() == LocalizedContentManager.LanguageCode.en)
                      this.addMarriageDialogue("Strings\\StringsFromCSFiles", "MultiplePetBowls_watered", false, Game1.player.getPetDisplayName());
                    else
                      this.addMarriageDialogue("Strings\\StringsFromCSFiles", "NPC.cs.4463", false, Game1.player.getPetDisplayName());
                  }
                  this.addMarriageDialogue("MarriageDialogue", "Outdoor_" + daySaveRandom.Next(5).ToString());
                  NPC.hasSomeoneRepairedTheFences = true;
                }
                else
                {
                  this.currentMarriageDialogue.Clear();
                  num1 = daySaveRandom.Next(5);
                  this.addMarriageDialogue("MarriageDialogue", "Outdoor_" + num1.ToString());
                }
              }
              Game1.warpCharacter(this, "Farm", farmHouse.getPorchStandingSpot());
              this.popOffAnyNonEssentialItems();
              this.faceDirection(2);
            }
            else if (this.Name.Equals("Krobus") && Game1.isRaining && daySaveRandom.NextDouble() < 0.4 && !NPC.checkTileOccupancyForSpouse((GameLocation) farm, Utility.PointToVector2(farmHouse.getPorchStandingSpot())))
            {
              num1 = daySaveRandom.Next(5);
              this.addMarriageDialogue("MarriageDialogue", "Outdoor_" + num1.ToString());
              Game1.warpCharacter(this, "Farm", farmHouse.getPorchStandingSpot());
              this.popOffAnyNonEssentialItems();
              this.faceDirection(2);
            }
            else if (spouse.GetDaysMarried() >= 1 && daySaveRandom.NextDouble() < 0.045)
            {
              if (daySaveRandom.NextDouble() < 0.75)
              {
                Point openPointInHouse = farmHouse.getRandomOpenPointInHouse(daySaveRandom, 1);
                Furniture furniture;
                try
                {
                  furniture = ItemRegistry.Create<Furniture>(Utility.getRandomSingleTileFurniture(daySaveRandom)).SetPlacement(openPointInHouse);
                }
                catch
                {
                  furniture = (Furniture) null;
                }
                if (furniture != null && openPointInHouse.X > 0 && farmHouse.CanItemBePlacedHere(new Vector2((float) (openPointInHouse.X - 1), (float) openPointInHouse.Y)))
                {
                  farmHouse.furniture.Add(furniture);
                  this.setTilePosition(openPointInHouse.X - 1, openPointInHouse.Y);
                  this.faceDirection(1);
                  this.addMarriageDialogue("Strings\\StringsFromCSFiles", "NPC.cs.4486", false, "%endearmentlower");
                  if (Game1.random.NextBool())
                    this.addMarriageDialogue("Strings\\StringsFromCSFiles", "NPC.cs.4488", true);
                  else
                    this.addMarriageDialogue("Strings\\StringsFromCSFiles", "NPC.cs.4489");
                }
                else
                {
                  this.setTilePosition(farmHouse.getKitchenStandingSpot());
                  this.spouseObstacleCheck(new MarriageDialogueReference("Strings\\StringsFromCSFiles", "NPC.cs.4490", false, Array.Empty<string>()), (GameLocation) farmHouse);
                }
              }
              else
              {
                Point openPointInHouse = farmHouse.getRandomOpenPointInHouse(daySaveRandom);
                if (openPointInHouse.X <= 0)
                  return;
                this.setTilePosition(openPointInHouse.X, openPointInHouse.Y);
                this.faceDirection(0);
                if (daySaveRandom.NextBool())
                {
                  string wallpaperId = farmHouse.GetWallpaperID(openPointInHouse.X, openPointInHouse.Y);
                  if (wallpaperId == null)
                    return;
                  string str = daySaveRandom.ChooseFrom<string>((IList<string>) this.GetData()?.SpouseWallpapers);
                  if (str == null)
                  {
                    num1 = daySaveRandom.Next(112 /*0x70*/);
                    str = num1.ToString();
                  }
                  string which = str;
                  farmHouse.SetWallpaper(which, wallpaperId);
                  this.addMarriageDialogue("Strings\\StringsFromCSFiles", "NPC.cs.4496");
                }
                else
                {
                  string floorRoomIdAt = farmHouse.getFloorRoomIdAt(openPointInHouse);
                  if (floorRoomIdAt == null)
                    return;
                  string str = daySaveRandom.ChooseFrom<string>((IList<string>) this.GetData()?.SpouseFloors);
                  if (str == null)
                  {
                    num1 = daySaveRandom.Next(40);
                    str = num1.ToString();
                  }
                  string which = str;
                  farmHouse.SetFloor(which, floorRoomIdAt);
                  this.addMarriageDialogue("Strings\\StringsFromCSFiles", "NPC.cs.4497");
                }
              }
            }
            else if (Game1.isRaining && daySaveRandom.NextDouble() < 0.08 && heartLevelForNpc < 11 && spouse.GetDaysMarried() > 7 && this.Name != "Krobus")
            {
              foreach (Furniture furniture in farmHouse.furniture)
              {
                if (furniture.furniture_type.Value == 13 && farmHouse.CanItemBePlacedHere(new Vector2((float) (int) furniture.tileLocation.X, (float) ((int) furniture.tileLocation.Y + 1))))
                {
                  this.setTilePosition((int) furniture.tileLocation.X, (int) furniture.tileLocation.Y + 1);
                  this.faceDirection(0);
                  this.currentMarriageDialogue.Clear();
                  this.addMarriageDialogue("Strings\\StringsFromCSFiles", "NPC.cs.4498", true);
                  return;
                }
              }
              this.spouseObstacleCheck(new MarriageDialogueReference("Strings\\StringsFromCSFiles", "NPC.cs.4499", false, Array.Empty<string>()), (GameLocation) farmHouse, true);
            }
            else if (daySaveRandom.NextDouble() < 0.45)
            {
              Vector2 vector2 = Utility.PointToVector2(farmHouse.GetSpouseRoomSpot());
              this.setTilePosition((int) vector2.X, (int) vector2.Y);
              this.faceDirection(0);
              this.setSpouseRoomMarriageDialogue();
              if (!(this.name.Value == "Sebastian") || !Game1.netWorldState.Value.hasWorldStateID("sebastianFrog"))
                return;
              Point spouseRoomCorner = farmHouse.GetSpouseRoomCorner();
              spouseRoomCorner.X += 2;
              spouseRoomCorner.Y += 5;
              this.setTilePosition(spouseRoomCorner);
              this.faceDirection(2);
            }
            else
            {
              this.setTilePosition(farmHouse.getKitchenStandingSpot());
              this.faceDirection(0);
              if (daySaveRandom.NextDouble() >= 0.2)
                return;
              this.setRandomAfternoonMarriageDialogue(Game1.timeOfDay, (GameLocation) farmHouse);
            }
          }
        }
      }
    }
  }

  public virtual void popOffAnyNonEssentialItems()
  {
    if (!Game1.IsMasterGame || this.currentLocation == null)
      return;
    Point tilePoint = this.TilePoint;
    Object objectAtTile = this.currentLocation.getObjectAtTile(tilePoint.X, tilePoint.Y);
    if (objectAtTile == null || !(objectAtTile.QualifiedItemId == "(O)93") && !(objectAtTile is Torch))
      return;
    Vector2 tileLocation = objectAtTile.TileLocation;
    objectAtTile.performRemoveAction();
    this.currentLocation.objects.Remove(tileLocation);
    objectAtTile.dropItem(this.currentLocation, tileLocation * 64f, tileLocation * 64f);
  }

  public static bool checkTileOccupancyForSpouse(
    GameLocation location,
    Vector2 point,
    string characterToIgnore = "")
  {
    return location == null || location.IsTileOccupiedBy(point, CollisionMask.Buildings | CollisionMask.Flooring | CollisionMask.Furniture | CollisionMask.Objects | CollisionMask.TerrainFeatures | CollisionMask.LocationSpecific, CollisionMask.All);
  }

  public void addMarriageDialogue(
    string dialogue_file,
    string dialogue_key,
    bool gendered = false,
    params string[] substitutions)
  {
    this.shouldSayMarriageDialogue.Value = true;
    this.currentMarriageDialogue.Add(new MarriageDialogueReference(dialogue_file, dialogue_key, gendered, substitutions));
  }

  public void clearTextAboveHead()
  {
    this.textAboveHead = (string) null;
    this.textAboveHeadPreTimer = -1;
    this.textAboveHeadTimer = -1;
  }

  /// <summary>Get whether this is a villager NPC, regardless of whether they're present in <c>Data/Characters</c>.</summary>
  [Obsolete("Use IsVillager instead.")]
  public bool isVillager() => this.IsVillager;

  public override bool shouldCollideWithBuildingLayer(GameLocation location)
  {
    return this.isMarried() && (this.Schedule == null || location is FarmHouse) || base.shouldCollideWithBuildingLayer(location);
  }

  public virtual void arriveAtFarmHouse(FarmHouse farmHouse)
  {
    if (Game1.newDay || !this.isMarried() || Game1.timeOfDay <= 630 || !(this.TilePoint != farmHouse.getSpouseBedSpot(this.name.Value)))
      return;
    this.setTilePosition(farmHouse.getEntryLocation());
    this.ignoreScheduleToday = true;
    this.temporaryController = (PathFindController) null;
    this.controller = (PathFindController) null;
    if (Game1.timeOfDay >= 2130)
    {
      Point spouseBedSpot = farmHouse.getSpouseBedSpot(this.name.Value);
      bool flag = farmHouse.GetSpouseBed() != null;
      PathFindController.endBehavior endBehaviorFunction = (PathFindController.endBehavior) null;
      if (flag)
        endBehaviorFunction = new PathFindController.endBehavior(FarmHouse.spouseSleepEndFunction);
      this.controller = new PathFindController((Character) this, (GameLocation) farmHouse, spouseBedSpot, 0, endBehaviorFunction);
      if (this.controller.pathToEndPoint != null && flag)
      {
        foreach (Furniture furniture in farmHouse.furniture)
        {
          if (furniture is BedFurniture bedFurniture && furniture.GetBoundingBox().Intersects(new Microsoft.Xna.Framework.Rectangle(spouseBedSpot.X * 64 /*0x40*/, spouseBedSpot.Y * 64 /*0x40*/, 64 /*0x40*/, 64 /*0x40*/)))
          {
            bedFurniture.ReserveForNPC();
            break;
          }
        }
      }
    }
    else
      this.controller = new PathFindController((Character) this, (GameLocation) farmHouse, farmHouse.getKitchenStandingSpot(), 0);
    if (this.controller.pathToEndPoint == null)
    {
      this.willDestroyObjectsUnderfoot = true;
      this.controller = new PathFindController((Character) this, (GameLocation) farmHouse, farmHouse.getKitchenStandingSpot(), 0);
      this.setNewDialogue(this.TryGetDialogue("SpouseFarmhouseClutter") ?? new StardewValley.Dialogue(this, "Strings\\StringsFromCSFiles:NPC.cs.4500", true));
    }
    else if (Game1.timeOfDay > 1300)
    {
      if (this.ScheduleKey == "marriage_" + Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth))
        this.setNewDialogue("MarriageDialogue", "funReturn_", true);
      else if (this.ScheduleKey == "marriageJob")
        this.setNewDialogue("MarriageDialogue", "jobReturn_");
      else if (Game1.timeOfDay < 1800)
        this.setRandomAfternoonMarriageDialogue(Game1.timeOfDay, this.currentLocation, true);
    }
    if (Game1.currentLocation != farmHouse)
      return;
    Game1.currentLocation.playSound("doorClose", context: SoundContext.NPC);
  }

  public Farmer getSpouse()
  {
    foreach (Farmer allFarmer in Game1.getAllFarmers())
    {
      if (allFarmer.spouse != null && allFarmer.spouse == this.Name)
        return allFarmer;
    }
    return (Farmer) null;
  }

  public string getTermOfSpousalEndearment(bool happy = true)
  {
    Farmer spouse = this.getSpouse();
    if (spouse == null)
      return Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4517");
    if (this.isRoommate())
      return spouse.displayName;
    if (spouse.getFriendshipHeartLevelForNPC(this.Name) < 9)
      return spouse.displayName;
    if (!happy)
    {
      switch (Game1.random.Next(2))
      {
        case 0:
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4517");
        case 1:
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4518");
        default:
          return spouse.displayName;
      }
    }
    else if (Game1.random.NextDouble() < 0.08)
    {
      switch (Game1.random.Next(8))
      {
        case 0:
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4507");
        case 1:
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4508");
        case 2:
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4509");
        case 3:
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4510");
        case 4:
          return !spouse.IsMale ? Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4512") : Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4511");
        case 5:
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4513");
        case 6:
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4514");
        default:
          return !spouse.IsMale ? Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4516") : Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4515");
      }
    }
    else
    {
      switch (Game1.random.Next(5))
      {
        case 0:
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4519");
        case 1:
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4518");
        case 2:
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4517");
        case 3:
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4522");
        default:
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.4523");
      }
    }
  }

  /// <summary>
  /// return true if spouse encountered obstacle.
  /// if force == true then the obstacle check will be ignored and spouse will absolutely be put into bed.
  /// </summary>
  /// <param name="backToBedMessage"></param>
  /// <param name="currentLocation"></param>
  /// <returns></returns>
  public bool spouseObstacleCheck(
    MarriageDialogueReference backToBedMessage,
    GameLocation currentLocation,
    bool force = false)
  {
    if (!force && !NPC.checkTileOccupancyForSpouse(currentLocation, this.Tile, this.Name))
      return false;
    Game1.warpCharacter(this, this.defaultMap.Value, Game1.RequireLocation<FarmHouse>(this.defaultMap.Value).getSpouseBedSpot(this.name.Value));
    this.faceDirection(1);
    this.currentMarriageDialogue.Clear();
    this.currentMarriageDialogue.Add(backToBedMessage);
    this.shouldSayMarriageDialogue.Value = true;
    return true;
  }

  public void setTilePosition(Point p) => this.setTilePosition(p.X, p.Y);

  public void setTilePosition(int x, int y)
  {
    this.Position = new Vector2((float) (x * 64 /*0x40*/), (float) (y * 64 /*0x40*/));
  }

  private void clintHammerSound(Farmer who)
  {
    this.currentLocation.playSound("hammer", new Vector2?(this.Tile));
  }

  private void robinHammerSound(Farmer who)
  {
    if (!Game1.currentLocation.Equals(this.currentLocation) || !Utility.isOnScreen(this.Position, 256 /*0x0100*/))
      return;
    Game1.playSound(Game1.random.NextDouble() < 0.1 ? "clank" : "axchop");
    this.shakeTimer = 250;
  }

  private void robinVariablePause(Farmer who)
  {
    if (Game1.random.NextDouble() < 0.4)
      this.Sprite.CurrentAnimation[this.Sprite.currentAnimationIndex] = new FarmerSprite.AnimationFrame(27, 300, false, false, new AnimatedSprite.endOfAnimationBehavior(this.robinVariablePause));
    else if (Game1.random.NextDouble() < 0.25)
      this.Sprite.CurrentAnimation[this.Sprite.currentAnimationIndex] = new FarmerSprite.AnimationFrame(23, Game1.random.Next(500, 4000), false, false, new AnimatedSprite.endOfAnimationBehavior(this.robinVariablePause));
    else
      this.Sprite.CurrentAnimation[this.Sprite.currentAnimationIndex] = new FarmerSprite.AnimationFrame(27, Game1.random.Next(1000, 4000), false, false, new AnimatedSprite.endOfAnimationBehavior(this.robinVariablePause));
  }

  public void randomSquareMovement(GameTime time)
  {
    Microsoft.Xna.Framework.Rectangle boundingBox = this.GetBoundingBox();
    boundingBox.Inflate(2, 2);
    Microsoft.Xna.Framework.Rectangle rectangle = new Microsoft.Xna.Framework.Rectangle((int) this.nextSquarePosition.X * 64 /*0x40*/, (int) this.nextSquarePosition.Y * 64 /*0x40*/, 64 /*0x40*/, 64 /*0x40*/);
    if (this.nextSquarePosition.Equals(Vector2.Zero))
    {
      this.squarePauseAccumulation = 0;
      this.squarePauseTotal = Game1.random.Next(6000 + this.squarePauseOffset, 12000 + this.squarePauseOffset);
      this.nextSquarePosition = new Vector2((float) (this.lastCrossroad.X / 64 /*0x40*/ - this.lengthOfWalkingSquareX / 2 + Game1.random.Next(this.lengthOfWalkingSquareX)), (float) (this.lastCrossroad.Y / 64 /*0x40*/ - this.lengthOfWalkingSquareY / 2 + Game1.random.Next(this.lengthOfWalkingSquareY)));
    }
    else if (rectangle.Contains(boundingBox))
    {
      this.Halt();
      if (this.squareMovementFacingPreference != -1)
        this.faceDirection(this.squareMovementFacingPreference);
      this.isCharging = false;
      this.speed = 2;
    }
    else if (boundingBox.Left <= rectangle.Left)
      this.SetMovingOnlyRight();
    else if (boundingBox.Right >= rectangle.Right)
      this.SetMovingOnlyLeft();
    else if (boundingBox.Top <= rectangle.Top)
      this.SetMovingOnlyDown();
    else if (boundingBox.Bottom >= rectangle.Bottom)
      this.SetMovingOnlyUp();
    this.squarePauseAccumulation += time.ElapsedGameTime.Milliseconds;
    if (this.squarePauseAccumulation < this.squarePauseTotal || !rectangle.Contains(boundingBox))
      return;
    this.nextSquarePosition = Vector2.Zero;
    this.isCharging = false;
    this.speed = 2;
  }

  public void returnToEndPoint()
  {
    Microsoft.Xna.Framework.Rectangle boundingBox = this.GetBoundingBox();
    boundingBox.Inflate(2, 2);
    if (boundingBox.Left <= this.lastCrossroad.Left)
      this.SetMovingOnlyRight();
    else if (boundingBox.Right >= this.lastCrossroad.Right)
      this.SetMovingOnlyLeft();
    else if (boundingBox.Top <= this.lastCrossroad.Top)
      this.SetMovingOnlyDown();
    else if (boundingBox.Bottom >= this.lastCrossroad.Bottom)
      this.SetMovingOnlyUp();
    boundingBox.Inflate(-2, -2);
    if (!this.lastCrossroad.Contains(boundingBox))
      return;
    this.isWalkingInSquare = false;
    this.nextSquarePosition = Vector2.Zero;
    this.returningToEndPoint = false;
    this.Halt();
  }

  public void SetMovingOnlyUp()
  {
    this.moveUp = true;
    this.moveDown = false;
    this.moveLeft = false;
    this.moveRight = false;
  }

  public void SetMovingOnlyRight()
  {
    this.moveUp = false;
    this.moveDown = false;
    this.moveLeft = false;
    this.moveRight = true;
  }

  public void SetMovingOnlyDown()
  {
    this.moveUp = false;
    this.moveDown = true;
    this.moveLeft = false;
    this.moveRight = false;
  }

  public void SetMovingOnlyLeft()
  {
    this.moveUp = false;
    this.moveDown = false;
    this.moveLeft = true;
    this.moveRight = false;
  }

  public virtual int getTimeFarmerMustPushBeforePassingThrough() => 1500;

  public virtual int getTimeFarmerMustPushBeforeStartShaking() => 400;

  public int CompareTo(object obj) => obj is NPC npc ? npc.id - this.id : 0;

  public virtual void Removed()
  {
  }
}
