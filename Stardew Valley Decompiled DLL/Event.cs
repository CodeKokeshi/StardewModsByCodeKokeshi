// Decompiled with JetBrains decompiler
// Type: StardewValley.Event
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.BellsAndWhistles;
using StardewValley.Buildings;
using StardewValley.Characters;
using StardewValley.Delegates;
using StardewValley.Extensions;
using StardewValley.GameData;
using StardewValley.GameData.Characters;
using StardewValley.GameData.Movies;
using StardewValley.GameData.Pets;
using StardewValley.GameData.Shops;
using StardewValley.Internal;
using StardewValley.Locations;
using StardewValley.Logging;
using StardewValley.Menus;
using StardewValley.Minigames;
using StardewValley.Monsters;
using StardewValley.Network;
using StardewValley.Network.NetEvents;
using StardewValley.Objects;
using StardewValley.Pathfinding;
using StardewValley.Quests;
using StardewValley.SpecialOrders;
using StardewValley.TerrainFeatures;
using StardewValley.TokenizableStrings;
using StardewValley.Tools;
using StardewValley.Triggers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using xTile.Dimensions;
using xTile.Layers;
using xTile.Tiles;

#nullable disable
namespace StardewValley;

public class Event
{
  /// <summary>The event commands indexed by name.</summary>
  /// <remarks>Command names are case-insensitive.</remarks>
  protected static readonly Dictionary<string, EventCommandDelegate> Commands = new Dictionary<string, EventCommandDelegate>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
  /// <summary>Alternate names for event commands.</summary>
  protected static readonly Dictionary<string, string> CommandAliases = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
  /// <summary>The registered command names.</summary>
  protected static readonly HashSet<string> CommandNames = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
  /// <summary>The event preconditions indexed by name.</summary>
  /// <remarks>Precondition names are case-<strong>sensitive</strong>.</remarks>
  protected static readonly Dictionary<string, EventPreconditionDelegate> Preconditions = new Dictionary<string, EventPreconditionDelegate>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
  /// <summary>Alternate names for event preconditions (e.g. shorthand or acronyms).</summary>
  /// <remarks>Aliases are case-sensitive for compatibility with older preconditions like 'h' vs 'H'.</remarks>
  private static readonly Dictionary<string, string> PreconditionAliases = new Dictionary<string, string>();
  private const float timeBetweenSpeech = 500f;
  public const string festivalTextureName = "Maps\\Festivals";
  private string festivalDataAssetName;
  /// <summary>
  ///   The unique identifier for the event, if available. This may be...
  ///   <list type="bullet">
  ///     <item><description>for a regular event, the unique event ID from its data file (i.e. the first number in its entry key);</description></item>
  ///     <item><description>for a generated event, an <see cref="T:StardewValley.Constants.EventIds" /> constant;</description></item>
  ///     <item><description>for a festival, <c>festival_{asset name}</c> (like <c>festival_fall16</c>);</description></item>
  ///     <item><description>else <see cref="F:StardewValley.Constants.EventIds.Unknown" />.</description></item>
  ///   </list>
  /// </summary>
  public string id = "-1";
  /// <summary>The data asset name from which the event script was taken, or <c>null</c> for a generated event.</summary>
  public string fromAssetName;
  public bool isFestival;
  public bool isWedding;
  public bool isMemory;
  /// <summary>Whether the player can skip the rest of the event.</summary>
  public bool skippable;
  /// <summary>The actions to perform when the event is skipped, if any.</summary>
  public string[] actionsOnSkip;
  public bool skipped;
  public bool forked;
  public bool eventSwitched;
  /// <summary>Whether we need to notify the dedicated server once the event finishes running.</summary>
  internal bool notifyWhenDone;
  /// <summary>The location name sent to the dedicated server to identify an event.</summary>
  internal string notifyLocationName;
  /// <summary>Whether the location name sent to the dedicated server corresponds to a structure.</summary>
  internal byte notifyLocationIsStructure;
  private readonly LocalizedContentManager festivalContent = Game1.content.CreateTemporary();
  public string[] eventCommands;
  public int currentCommand;
  private Dictionary<string, Vector3> actorPositionsAfterMove;
  private float timeAccumulator;
  private Vector3 viewportTarget;
  private Color previousAmbientLight;
  private HashSet<long> festivalWinners = new HashSet<long>();
  private GameLocation temporaryLocation;
  private Dictionary<string, string> festivalData;
  private Texture2D _festivalTexture;
  private bool drawTool;
  private string hostMessageKey;
  private int previousFacingDirection = -1;
  private int previousAnswerChoice = -1;
  private bool startSecretSantaAfterDialogue;
  private List<Farmer> iceFishWinners;
  protected static LocalizedContentManager FestivalReadContentLoader;
  protected bool _playerControlSequence;
  protected bool _repeatingLocationSpecificCommand;
  [NonInstancedStatic]
  public static HashSet<string> invalidFestivals = new HashSet<string>();
  public List<NPC> actors = new List<NPC>();
  public List<Object> props = new List<Object>();
  public List<Prop> festivalProps = new List<Prop>();
  public List<Farmer> farmerActors = new List<Farmer>();
  public Dictionary<string, Dictionary<ISalable, ItemStockInformation>> festivalShops;
  public List<NPCController> npcControllers;
  internal NPC festivalHost;
  public NPC secretSantaRecipient;
  public NPC mySecretSanta;
  public TemporaryAnimatedSpriteList underwaterSprites;
  public TemporaryAnimatedSpriteList aboveMapSprites;
  /// <summary>The custom sounds started during the event via <see cref="M:StardewValley.Event.DefaultCommands.PlaySound(StardewValley.Event,System.String[],StardewValley.EventContext)" />.</summary>
  public IDictionary<string, List<ICue>> CustomSounds = (IDictionary<string, List<ICue>>) new Dictionary<string, List<ICue>>();
  public ICustomEventScript currentCustomEventScript;
  public bool simultaneousCommand;
  public int farmerAddedSpeed;
  public int int_useMeForAnything;
  public int int_useMeForAnything2;
  public float float_useMeForAnything;
  public string playerControlSequenceID;
  public string spriteTextToDraw;
  public bool showActiveObject;
  public bool continueAfterMove;
  public bool specialEventVariable1;
  public bool specialEventVariable2;
  public bool showGroundObjects = true;
  public bool doingSecretSanta;
  public bool showWorldCharacters;
  public bool ignoreObjectCollisions = true;
  public Point playerControlTargetTile;
  public List<Vector2> characterWalkLocations = new List<Vector2>();
  public Vector2 eventPositionTileOffset = Vector2.Zero;
  public int festivalTimer;
  public int grangeScore = -1000;
  public bool grangeJudged;
  /// <summary>Used to offset positions specified in events.</summary>
  public bool ignoreTileOffsets;
  private Stopwatch stopWatch;
  public LocationRequest exitLocation;
  public Action onEventFinished;
  /// <summary>Whether to add this event's ID to <see cref="F:StardewValley.Farmer.eventsSeen" /> when it ends, if it has a valid ID.</summary>
  /// <remarks>This has no effect on <see cref="F:StardewValley.Game1.eventsSeenSinceLastLocationChange" />, which is updated regardless (if it has a valid ID) to prevent event loops.</remarks>
  public bool markEventSeen = true;
  private bool eventFinished;
  private bool gotPet;

  /// <summary>Register an event command.</summary>
  /// <param name="name">The command name that can be used in event scripts. This is case-insensitive.</param>
  /// <param name="action">The handler to call when the command is used.</param>
  public static void RegisterCommand(string name, EventCommandDelegate action)
  {
    Event.SetupEventCommandsIfNeeded();
    if (Event.Commands.ContainsKey(name))
      Game1.log.Warn($"Warning: event command {name} is already defined and will be overwritten.");
    Event.Commands[name] = action;
    Event.CommandNames.Add(name);
    Game1.log.Verbose("Registered event command: " + name);
  }

  /// <summary>Register an alternate name for an event command.</summary>
  /// <param name="alias">The alternate name. This is case-insensitive.</param>
  /// <param name="commandName">The original command name to alias. This is case-insensitive.</param>
  public static void RegisterCommandAlias(string alias, string commandName)
  {
    Event.SetupEventCommandsIfNeeded();
    if (string.IsNullOrWhiteSpace(alias) || string.IsNullOrWhiteSpace(commandName))
      Game1.log.Error($"Can't register event command alias '{alias}' for '{commandName}' because the alias and command name must both be non-null and non-empty strings.");
    else if (Event.Commands.ContainsKey(alias))
    {
      Game1.log.Error($"Can't register event command alias '{alias}' for command '{commandName}', because there's a command with that name.");
    }
    else
    {
      string str;
      if (Event.CommandAliases.TryGetValue(alias, out str))
        Game1.log.Error($"Can't register event command alias '{alias}' for command '{commandName}', because that's already an alias for '{str}'.");
      else if (!Event.Commands.ContainsKey(commandName))
        Game1.log.Error($"Can't register event command alias '{alias}' for command '{commandName}', because there's no such command.");
      else
        Event.CommandAliases[alias] = commandName;
    }
  }

  /// <summary>Get the actual command name which matches an input command or alias name.</summary>
  /// <param name="name">The command or alias name to resolve.</param>
  /// <param name="actualName">The resolved command name.</param>
  /// <returns>Returns whether a matching command was found.</returns>
  /// <remarks>For example, this can be used to resolve an alias (like <c>mailReceived</c> → <c>AddMailReceived</c>) or normalize the capitalization (like <c>itemnamed</c> → <c>ItemNamed</c>).</remarks>
  public static bool TryResolveCommandName(string name, out string actualName)
  {
    Event.SetupEventCommandsIfNeeded();
    if (Event.CommandAliases.TryGetValue(name, out actualName) || Event.CommandNames.TryGetValue(name, out actualName))
      return true;
    actualName = (string) null;
    return false;
  }

  /// <summary>Register an event precondition.</summary>
  /// <param name="name">The precondition key that can be used in event precondition strings. This is case-insensitive.</param>
  /// <param name="action">The handler to call when the precondition is used.</param>
  public static void RegisterPrecondition(string name, EventPreconditionDelegate action)
  {
    Event.SetupEventCommandsIfNeeded();
    if (Event.Preconditions.ContainsKey(name))
      Game1.log.Warn($"Warning: event precondition {name} is already defined and will be overwritten.");
    if (Event.PreconditionAliases.Remove(name))
      Game1.log.Warn($"Warning: '{name}' was previously registered as a precondition alias. The alias was removed.");
    Event.Preconditions[name] = action;
    Game1.log.Verbose("Registered precondition: " + name);
  }

  /// <summary>Register an alternate name for an event precondition.</summary>
  /// <param name="alias">The alternate name. This is <strong>case-sensitive</strong> for legacy reasons.</param>
  /// <param name="preconditionName">The original precondition name to alias. This is case-insensitive.</param>
  public static void RegisterPreconditionAlias(string alias, string preconditionName)
  {
    Event.SetupEventCommandsIfNeeded();
    if (string.IsNullOrWhiteSpace(alias) || string.IsNullOrWhiteSpace(preconditionName))
      Game1.log.Error($"Can't register event precondition alias '{alias}' for '{preconditionName}' because the alias and precondition name must both be non-null and non-empty strings.");
    else if (Event.Preconditions.ContainsKey(alias))
    {
      Game1.log.Error($"Can't register event precondition alias '{alias}' for precondition '{preconditionName}', because there's a precondition with that name.");
    }
    else
    {
      string str;
      if (Event.PreconditionAliases.TryGetValue(alias, out str))
        Game1.log.Error($"Can't register event precondition alias '{alias}' for precondition '{preconditionName}', because that's already an alias for '{str}'.");
      else if (!Event.Preconditions.ContainsKey(preconditionName))
        Game1.log.Error($"Can't register event precondition alias '{alias}' for precondition '{preconditionName}', because there's no such precondition.");
      else
        Event.PreconditionAliases[alias] = preconditionName;
    }
  }

  /// <summary>Register the vanilla event commands and preconditions if they haven't already been registered.</summary>
  private static void SetupEventCommandsIfNeeded()
  {
    if (Event.Commands.Count == 0)
    {
      MethodInfo[] methods = typeof (Event.DefaultCommands).GetMethods(BindingFlags.Static | BindingFlags.Public);
      foreach (MethodInfo method in methods)
      {
        EventCommandDelegate eventCommandDelegate = (EventCommandDelegate) Delegate.CreateDelegate(typeof (EventCommandDelegate), method);
        Event.Commands.Add(method.Name, eventCommandDelegate);
        Event.CommandNames.Add(method.Name);
      }
      foreach (MethodInfo element in methods)
      {
        OtherNamesAttribute customAttribute = element.GetCustomAttribute<OtherNamesAttribute>();
        if (customAttribute != null)
        {
          foreach (string alias in customAttribute.Aliases)
            Event.RegisterCommandAlias(alias, element.Name);
        }
      }
    }
    if (Event.Preconditions.Count != 0)
      return;
    MethodInfo[] methods1 = typeof (StardewValley.Preconditions).GetMethods(BindingFlags.Static | BindingFlags.Public);
    foreach (MethodInfo method in methods1)
    {
      EventPreconditionDelegate preconditionDelegate = (EventPreconditionDelegate) Delegate.CreateDelegate(typeof (EventPreconditionDelegate), method);
      Event.Preconditions[method.Name] = preconditionDelegate;
    }
    foreach (MethodInfo element in methods1)
    {
      OtherNamesAttribute customAttribute = element.GetCustomAttribute<OtherNamesAttribute>();
      if (customAttribute != null)
      {
        foreach (string alias in customAttribute.Aliases)
          Event.RegisterPreconditionAlias(alias, element.Name);
      }
    }
  }

  /// <summary>Get the handler for a precondition key, if any.</summary>
  /// <param name="key">The precondition key, which can be either the case-insensitive canonical name (like <c>DaysPlayed</c>) or case-sensitive alias (like <c>j</c>).</param>
  /// <param name="handler">The precondition handler, if found.</param>
  /// <returns>Returns whether a handler was found for the precondition key.</returns>
  public static bool TryGetPreconditionHandler(string key, out EventPreconditionDelegate handler)
  {
    Event.SetupEventCommandsIfNeeded();
    string str;
    if (Event.PreconditionAliases.TryGetValue(key, out str))
      key = str;
    return Event.Preconditions.TryGetValue(key, out handler);
  }

  /// <summary>Get whether an event precondition matches the current context.</summary>
  /// <param name="location">The location which is checking the event.</param>
  /// <param name="eventId">The unique ID for the event being checked.</param>
  /// <param name="precondition">The event precondition string, including the precondition name.</param>
  public static bool CheckPrecondition(GameLocation location, string eventId, string precondition)
  {
    string[] args = ArgUtility.SplitBySpaceQuoteAware(precondition);
    string key = args[0];
    bool flag = true;
    if (key.StartsWith('!'))
    {
      key = key.Substring(1);
      flag = false;
    }
    EventPreconditionDelegate handler;
    if (!Event.TryGetPreconditionHandler(key, out handler))
    {
      Game1.log.Warn($"Unknown precondition for event {eventId}: {precondition}");
      return false;
    }
    try
    {
      return handler(location, eventId, args) == flag;
    }
    catch (Exception ex)
    {
      Game1.log.Error($"Failed checking precondition '{precondition}' for event {eventId}.", ex);
      return false;
    }
  }

  /// <summary>Get the handler for an event command key, if any.</summary>
  /// <param name="key">The event command key, which can be either the case-insensitive canonical name (like <c>AddMailReceived</c>) or case-sensitive alias (like <c>mailReceived</c>).</param>
  /// <param name="handler">The event command handler, if found.</param>
  /// <returns>Returns whether a handler was found for the event command key.</returns>
  public static bool TryGetEventCommandHandler(string key, out EventCommandDelegate handler)
  {
    string str;
    if (Event.CommandAliases.TryGetValue(key, out str))
      key = str;
    return Event.Commands.TryGetValue(key, out handler);
  }

  /// <summary>Try to run an event command for the current event.</summary>
  /// <param name="location">The location in which the event is running.</param>
  /// <param name="time">The current game execution time.</param>
  /// <param name="args">The space-delimited event command string, including the command name.</param>
  public virtual void tryEventCommand(GameLocation location, GameTime time, string[] args)
  {
    string key = ArgUtility.Get(args, 0);
    if (string.IsNullOrWhiteSpace(key))
    {
      this.LogCommandErrorAndSkip(args, "can't run an empty or null command");
    }
    else
    {
      EventCommandDelegate handler;
      if (!Event.TryGetEventCommandHandler(key, out handler))
      {
        this.LogCommandErrorAndSkip(args, $"unknown command '{key}'");
      }
      else
      {
        try
        {
          EventContext context = new EventContext(this, location, time, args);
          handler(this, args, context);
        }
        catch (Exception ex)
        {
          this.LogErrorAndHalt(ex);
        }
      }
    }
  }

  public string FestivalName
  {
    get
    {
      string data;
      return !this.TryGetFestivalDataForYear("name", out data) ? "" : data;
    }
  }

  public bool playerControlSequence
  {
    get => this._playerControlSequence;
    set
    {
      if (this._playerControlSequence == value)
        return;
      this._playerControlSequence = value;
      if (this._playerControlSequence)
        return;
      this.OnPlayerControlSequenceEnd(this.playerControlSequenceID);
    }
  }

  public Farmer farmer => this.farmerActors.Count <= 0 ? Game1.player : this.farmerActors[0];

  public Texture2D festivalTexture
  {
    get
    {
      if (this._festivalTexture == null)
        this._festivalTexture = this.festivalContent.Load<Texture2D>("Maps\\Festivals");
      return this._festivalTexture;
    }
  }

  public int CurrentCommand
  {
    get => this.currentCommand;
    set => this.currentCommand = value;
  }

  /// <summary>Construct an instance.</summary>
  /// <param name="eventString">The raw event script.</param>
  /// <param name="farmerActor">The player to add as an actor in the event script, or <c>null</c> to use <see cref="P:StardewValley.Game1.player" />.</param>
  public Event(string eventString, Farmer farmerActor = null)
    : this(eventString, (string) null, "-1", farmerActor)
  {
  }

  /// <summary>Construct an instance.</summary>
  /// <param name="eventString">The raw event script.</param>
  /// <param name="fromAssetName">The data asset name from which the event script was taken, or <c>null</c> for a generated event.</param>
  /// <param name="eventID">The event's unique ID from the event data files, if known. This may be a number matching one of the <see cref="T:StardewValley.Event" /> constants in <see cref="T:StardewValley.Constants.EventIds" /> for a generated event.</param>
  /// <param name="farmerActor">The player to add as an actor in the event script, or <c>null</c> to use <see cref="P:StardewValley.Game1.player" />.</param>
  public Event(string eventString, string fromAssetName, string eventID, Farmer farmerActor = null)
    : this()
  {
    this.fromAssetName = fromAssetName;
    this.id = eventID;
    this.eventCommands = Event.ParseCommands(eventString, farmerActor);
    this.actorPositionsAfterMove = new Dictionary<string, Vector3>();
    this.previousAmbientLight = Game1.ambientLight;
    if (farmerActor != null)
      this.farmerActors.Add(farmerActor);
    this.farmer.canOnlyWalk = true;
    this.farmer.showNotCarrying();
    this.drawTool = false;
    if (!(eventID == "-2"))
      return;
    this.isWedding = true;
  }

  /// <summary>Construct an instance.</summary>
  public Event() => Event.SetupEventCommandsIfNeeded();

  ~Event() => this.notifyDone();

  public static void OnNewDay() => Event.FestivalReadContentLoader?.Unload();

  /// <summary>Load the raw data for a festival, if it exists and is valid.</summary>
  /// <param name="festival">The festival ID to load, matching the asset name under <c>Data/Festivals</c> (like <samp>spring13</samp>).</param>
  /// <param name="assetName">The asset name for the loaded festival data.</param>
  /// <param name="data">The loaded festival data.</param>
  /// <param name="locationName">The location name in which the festival takes place.</param>
  /// <param name="startTime">The time of day when the festival opens.</param>
  /// <param name="endTime">The time of day when the festival closes.</param>
  /// <returns>Returns whether the festival data was loaded successfully.</returns>
  public static bool tryToLoadFestivalData(
    string festival,
    out string assetName,
    out Dictionary<string, string> data,
    out string locationName,
    out int startTime,
    out int endTime)
  {
    assetName = "Data\\Festivals\\" + festival;
    data = (Dictionary<string, string>) null;
    locationName = (string) null;
    startTime = 0;
    endTime = 0;
    if (Event.invalidFestivals.Contains(festival))
      return false;
    if (Event.FestivalReadContentLoader == null)
      Event.FestivalReadContentLoader = Game1.content.CreateTemporary();
    try
    {
      if (!Event.FestivalReadContentLoader.DoesAssetExist<Dictionary<string, string>>(assetName))
      {
        Event.invalidFestivals.Add(festival);
        return false;
      }
      data = Event.FestivalReadContentLoader.Load<Dictionary<string, string>>(assetName);
    }
    catch
    {
      Event.invalidFestivals.Add(festival);
      return false;
    }
    string str1;
    if (!data.TryGetValue("conditions", out str1))
    {
      Game1.log.Error($"Festival '{festival}' doesn't have the required 'conditions' data field.");
      return false;
    }
    string[] array1 = LegacyShims.SplitAndTrim(str1, '/');
    string error;
    string str2;
    if (!ArgUtility.TryGet(array1, 0, out locationName, out error, false, nameof (locationName)) || !ArgUtility.TryGet(array1, 1, out str2, out error, false, "string rawTimeSpan"))
    {
      Game1.log.Error($"Festival '{festival}' has preconditions '{str1}' which couldn't be parsed: {error}.");
      return false;
    }
    string[] array2 = ArgUtility.SplitBySpace(str2);
    if (ArgUtility.TryGetInt(array2, 0, out startTime, out error, nameof (startTime)) && ArgUtility.TryGetInt(array2, 1, out endTime, out error, nameof (endTime)))
      return true;
    Game1.log.Error($"Festival '{festival}' has preconditions '{str1}' with time range '{string.Join(" ", array2)}' which couldn't be parsed: {error}.");
    return false;
  }

  /// <summary>Load a festival if it exists and its preconditions match the current time and the local player's current location.</summary>
  /// <param name="festival">The festival ID to load, matching the asset name under <c>Data/Festivals</c> (like <samp>spring13</samp>).</param>
  /// <param name="ev">The loaded festival event, if it was loaded successfully.</param>
  /// <returns>Returns whether the festival was loaded successfully.</returns>
  public static bool tryToLoadFestival(string festival, out Event ev)
  {
    ev = (Event) null;
    string assetName;
    Dictionary<string, string> data1;
    string locationName;
    int startTime;
    int endTime;
    if (!Event.tryToLoadFestivalData(festival, out assetName, out data1, out locationName, out startTime, out endTime) || locationName != Game1.currentLocation.Name || Game1.timeOfDay < startTime || Game1.timeOfDay >= endTime)
      return false;
    ev = new Event()
    {
      id = "festival_" + festival,
      isFestival = true,
      festivalDataAssetName = assetName,
      festivalData = data1,
      actorPositionsAfterMove = new Dictionary<string, Vector3>(),
      previousAmbientLight = Game1.ambientLight
    };
    ev.festivalData["file"] = festival;
    string data2;
    if (!ev.TryGetFestivalDataForYear("set-up", out data2))
      Game1.log.Error($"Festival {ev.id} doesn't have the required 'set-up' data field.");
    ev.eventCommands = Event.ParseCommands(data2, ev.farmer);
    Game1.player.festivalScore = 0;
    Game1.setRichPresence(nameof (festival), (object) festival);
    return true;
  }

  /// <summary>Try to get an NPC dialogue from the festival data, automatically adjusted to use the closest <c>{key}_y{year}</c> variant if any.</summary>
  /// <param name="npc">The NPC for which to get a dialogue.</param>
  /// <param name="key">The base field key for the dialogue text.</param>
  /// <param name="data">The resulting dialogue instance, or <c>null</c> if the key wasn't found.</param>
  /// <returns>Returns whether a matching dialogue was found.</returns>
  public bool TryGetFestivalDialogueForYear(NPC npc, string key, out Dialogue dialogue)
  {
    string data;
    string actualKey;
    if (this.TryGetFestivalDataForYear(key, out data, out actualKey))
    {
      dialogue = new Dialogue(npc, $"{this.festivalDataAssetName}:{actualKey}", data);
      return true;
    }
    dialogue = (Dialogue) null;
    return false;
  }

  /// <summary>Try to get a value from the festival data, automatically adjusted to use the closest <c>{key}_y{year}</c> variant if any.</summary>
  /// <param name="key">The base field key.</param>
  /// <param name="data">The resolved data, or <c>null</c> if the key wasn't found.</param>
  /// <param name="actualKey">The resolved field key, including the variant suffix if applicable, or <c>null</c> if the key wasn't found.</param>
  /// <returns>Returns whether a matching field was found.</returns>
  public bool TryGetFestivalDataForYear(string key, out string data, out string actualKey)
  {
    if (this.festivalData == null)
    {
      data = (string) null;
      actualKey = (string) null;
      return false;
    }
    int num1 = 1;
    while (true)
    {
      if (this.festivalData.ContainsKey($"{key}_y{num1 + 1}"))
        ++num1;
      else
        break;
    }
    int num2 = Game1.year % num1;
    if (num2 == 0)
      num2 = num1;
    ref string local = ref actualKey;
    string str;
    if (num2 <= 1)
      str = key;
    else
      str = $"{key}_y{num2}";
    local = str;
    if (this.festivalData.TryGetValue(actualKey, out data))
      return true;
    actualKey = (string) null;
    data = (string) null;
    return false;
  }

  /// <summary>Get a value from the festival data, automatically adjusted to use the closest <c>{key}_y{year}</c> variant if any.</summary>
  /// <param name="key">The base field key.</param>
  /// <param name="data">The resolved data, or <c>null</c> if the key wasn't found.</param>
  /// <returns>Returns whether a matching field was found.</returns>
  public bool TryGetFestivalDataForYear(string key, out string data)
  {
    return this.TryGetFestivalDataForYear(key, out data, out string _);
  }

  /// <summary>Set the location and tile position at which to warp the player once the event ends.</summary>
  /// <param name="warp">The warp whose endpoint to use as the exit location.</param>
  public void setExitLocation(Warp warp)
  {
    this.setExitLocation(warp.TargetName, warp.TargetX, warp.TargetY);
  }

  /// <summary>Set the location and tile position at which to warp the player once the event ends.</summary>
  /// <param name="location">The location name.</param>
  /// <param name="x">The X tile position.</param>
  /// <param name="y">The Y tile position.</param>
  public void setExitLocation(string location, int x, int y)
  {
    if (!string.IsNullOrEmpty(Game1.player.locationBeforeForcedEvent.Value))
      return;
    this.exitLocation = Game1.getLocationRequest(location);
    Game1.player.positionBeforeEvent = new Vector2((float) x, (float) y);
  }

  public void endBehaviors(GameLocation location = null)
  {
    this.endBehaviors(LegacyShims.EmptyArray<string>(), location ?? Game1.currentLocation);
  }

  public void endBehaviors(string[] args, GameLocation location)
  {
    if (Game1.getMusicTrackName().Contains(Game1.currentSeason) && ArgUtility.Get(this.eventCommands, 0) != "continue")
      Game1.stopMusicTrack(MusicContext.Default);
    string str = ArgUtility.Get(args, 1);
    if (str != null)
    {
      switch (str.Length)
      {
        case 3:
          switch (str[0])
          {
            case 'L':
              if (str == "Leo" && !this.isMemory)
              {
                Game1.addMailForTomorrow("leoMoved", true, true);
                Game1.player.team.requestLeoMove.Fire();
                break;
              }
              break;
            case 'b':
              if (str == "bed")
              {
                Game1.player.Position = Game1.player.mostRecentBed + new Vector2(0.0f, 64f);
                break;
              }
              break;
          }
          break;
        case 5:
          if (str == "Maru1")
          {
            (Game1.getCharacterFromName("Demetrius") ?? this.getActorByName("Demetrius"))?.setNewDialogue("Strings\\StringsFromCSFiles:Event.cs.1018");
            (Game1.getCharacterFromName("Maru") ?? this.getActorByName("Maru"))?.setNewDialogue("Strings\\StringsFromCSFiles:Event.cs.1020");
            this.setExitLocation(location.GetFirstPlayerWarp());
            Game1.fadeScreenToBlack();
            Game1.eventOver = true;
            this.CurrentCommand += 2;
            break;
          }
          break;
        case 6:
          if (str == "newDay")
          {
            Game1.player.faceDirection(2);
            this.setExitLocation(Game1.player.homeLocation.Value, (int) Game1.player.mostRecentBed.X / 64 /*0x40*/, (int) Game1.player.mostRecentBed.Y / 64 /*0x40*/);
            if (!Game1.IsMultiplayer)
              this.exitLocation.OnWarp += (LocationRequest.Callback) (() =>
              {
                Game1.NewDay(0.0f);
                Game1.player.currentLocation.lastTouchActionLocation = new Vector2((float) ((int) Game1.player.mostRecentBed.X / 64 /*0x40*/), (float) ((int) Game1.player.mostRecentBed.Y / 64 /*0x40*/));
              });
            Game1.player.completelyStopAnimatingOrDoingAction();
            if (Game1.player.bathingClothes.Value)
              Game1.player.changeOutOfSwimSuit();
            Game1.player.swimming.Value = false;
            Game1.player.CanMove = false;
            Game1.changeMusicTrack("none");
            break;
          }
          break;
        case 7:
          switch (str[1])
          {
            case 'a':
              if (str == "warpOut")
              {
                this.setExitLocation(location.GetFirstPlayerWarp());
                Game1.eventOver = true;
                this.CurrentCommand += 2;
                Game1.screenGlowHold = false;
                break;
              }
              break;
            case 'e':
              if (str == "wedding")
              {
                Game1.RequireCharacter("Lewis").CurrentDialogue.Push(new Dialogue(Game1.getCharacterFromName("Lewis"), "Strings\\StringsFromCSFiles:Event.cs.1025"));
                FarmHouse homeOfFarmer = Utility.getHomeOfFarmer(Game1.player);
                Point porchStandingSpot = homeOfFarmer.getPorchStandingSpot();
                if (homeOfFarmer is Cabin)
                  this.setExitLocation("Farm", porchStandingSpot.X + 1, porchStandingSpot.Y);
                else
                  this.setExitLocation("Farm", porchStandingSpot.X - 1, porchStandingSpot.Y);
                if (Game1.IsMasterGame)
                {
                  NPC characterFromName = Game1.getCharacterFromName(this.farmer.spouse);
                  if (characterFromName != null)
                  {
                    characterFromName.ClearSchedule();
                    characterFromName.ignoreScheduleToday = true;
                    characterFromName.shouldPlaySpousePatioAnimation.Value = false;
                    characterFromName.controller = (PathFindController) null;
                    characterFromName.temporaryController = (PathFindController) null;
                    characterFromName.currentMarriageDialogue.Clear();
                    Game1.warpCharacter(characterFromName, "Farm", Utility.getHomeOfFarmer(this.farmer).getPorchStandingSpot());
                    characterFromName.faceDirection(2);
                    if (Game1.content.LoadStringReturnNullIfNotFound($"Strings\\StringsFromCSFiles:{characterFromName.Name}_AfterWedding") != null)
                    {
                      characterFromName.addMarriageDialogue("Strings\\StringsFromCSFiles", characterFromName.Name + "_AfterWedding");
                      break;
                    }
                    characterFromName.addMarriageDialogue("Strings\\StringsFromCSFiles", "Game1.cs.2782");
                    break;
                  }
                  break;
                }
                break;
              }
              break;
            case 'r':
              if (str == "credits")
              {
                Game1.debrisWeather.Clear();
                Game1.isDebrisWeather = false;
                Game1.changeMusicTrack("wedding", music_context: MusicContext.Event);
                Game1.gameMode = (byte) 10;
                this.CurrentCommand += 2;
                break;
              }
              break;
          }
          break;
        case 8:
          switch (str[0])
          {
            case 'd':
              if (str == "dialogue")
              {
                string name;
                string error;
                string dialogueText;
                if (!ArgUtility.TryGet(args, 2, out name, out error, false, "string npcName") || !ArgUtility.TryGet(args, 3, out dialogueText, out error, name: "string dialogue"))
                {
                  this.LogCommandError(args, error);
                  break;
                }
                NPC characterFromName = Game1.getCharacterFromName(name);
                if (characterFromName == null)
                {
                  this.LogCommandError(args, $"NPC '{name}' not found");
                  break;
                }
                characterFromName.shouldSayMarriageDialogue.Value = false;
                characterFromName.currentMarriageDialogue.Clear();
                characterFromName.CurrentDialogue.Clear();
                characterFromName.CurrentDialogue.Push(new Dialogue(characterFromName, (string) null, dialogueText));
                break;
              }
              break;
            case 'p':
              if (str == "position")
              {
                Vector2 vector2;
                string error;
                if (!ArgUtility.TryGetVector2(args, 2, out vector2, out error, true, "Vector2 position"))
                {
                  this.LogCommandError(args, error);
                  break;
                }
                if (string.IsNullOrEmpty(Game1.player.locationBeforeForcedEvent.Value))
                {
                  Game1.player.positionBeforeEvent = vector2;
                  break;
                }
                break;
              }
              break;
          }
          break;
        case 9:
          switch (str[0])
          {
            case 'b':
              if (str == "beginGame")
              {
                Game1.gameMode = (byte) 3;
                this.setExitLocation("FarmHouse", 9, 9);
                Game1.NewDay(1000f);
                this.exitEvent();
                Game1.eventFinished();
                return;
              }
              break;
            case 'i':
              if (str == "invisible")
              {
                string name;
                string error;
                if (!ArgUtility.TryGet(args, 2, out name, out error, false, "string npcName"))
                {
                  this.LogCommandError(args, error);
                  break;
                }
                if (!this.isMemory)
                {
                  NPC characterFromName = Game1.getCharacterFromName(name);
                  if (characterFromName == null)
                  {
                    this.LogCommandError(args, $"NPC '{name}' not found");
                    break;
                  }
                  characterFromName.IsInvisible = true;
                  characterFromName.daysUntilNotInvisible = 1;
                  break;
                }
                break;
              }
              break;
          }
          break;
        case 12:
          switch (str[0])
          {
            case 'i':
              if (str == "islandDepart")
              {
                Game1.player.orientationBeforeEvent = 2;
                switch (Game1.whereIsTodaysFest)
                {
                  case "Beach":
                    Game1.player.orientationBeforeEvent = 0;
                    this.setExitLocation("Town", 54, 109);
                    break;
                  case "Town":
                    Game1.player.orientationBeforeEvent = 3;
                    this.setExitLocation("BusStop", 43, 23);
                    break;
                  default:
                    this.setExitLocation("BoatTunnel", 6, 9);
                    break;
                }
                GameLocation left_location = Game1.currentLocation;
                this.exitLocation.OnLoad += (LocationRequest.Callback) (() =>
                {
                  foreach (NPC actor in this.actors)
                  {
                    actor.shouldShadowBeOffset = true;
                    actor.drawOffset.Y = 0.0f;
                  }
                  foreach (Farmer farmerActor in this.farmerActors)
                  {
                    farmerActor.shouldShadowBeOffset = true;
                    farmerActor.drawOffset.Y = 0.0f;
                  }
                  Game1.player.drawOffset = Vector2.Zero;
                  Game1.player.shouldShadowBeOffset = false;
                  if (!(left_location is IslandSouth islandSouth2))
                    return;
                  islandSouth2.ResetBoat();
                });
                break;
              }
              break;
            case 't':
              if (str == "tunnelDepart" && Game1.player.hasOrWillReceiveMail("seenBoatJourney"))
              {
                Game1.warpFarmer("IslandSouth", 21, 43, 0);
                break;
              }
              break;
          }
          break;
        case 13:
          if (str == "qiSummitCheat")
          {
            Game1.playSound("death");
            Game1.player.health = -1;
            Game1.player.position.X = -99999f;
            Game1.background = (Background) null;
            Game1.viewport.X = -999999;
            Game1.viewport.Y = -999999;
            Game1.viewportHold = 6000;
            Game1.eventOver = true;
            this.CurrentCommand += 2;
            Game1.screenGlowHold = false;
            Game1.screenGlowOnce(Color.Black, true, 1f, 1f);
            break;
          }
          break;
        case 15:
          if (str == "dialogueWarpOut")
          {
            string name;
            string error;
            string dialogueText;
            if (!ArgUtility.TryGet(args, 2, out name, out error, false, "string npcName") || !ArgUtility.TryGet(args, 3, out dialogueText, out error, name: "string dialogue"))
            {
              this.LogCommandError(args, error);
              break;
            }
            this.setExitLocation(location.GetFirstPlayerWarp());
            NPC characterFromName = Game1.getCharacterFromName(name);
            if (characterFromName == null)
            {
              this.LogCommandError(args, $"NPC '{name}' not found");
              break;
            }
            characterFromName.CurrentDialogue.Clear();
            characterFromName.CurrentDialogue.Push(new Dialogue(characterFromName, (string) null, dialogueText));
            Game1.eventOver = true;
            this.CurrentCommand += 2;
            Game1.screenGlowHold = false;
            break;
          }
          break;
        case 16 /*0x10*/:
          if (str == "invisibleWarpOut")
          {
            string name;
            string error;
            if (!ArgUtility.TryGet(args, 2, out name, out error, false, "string npcName"))
            {
              this.LogCommandError(args, error);
              break;
            }
            NPC characterFromName = Game1.getCharacterFromName(name);
            if (characterFromName == null)
            {
              this.LogCommandError(args, $"NPC '{name}' not found");
              break;
            }
            characterFromName.IsInvisible = true;
            characterFromName.daysUntilNotInvisible = 1;
            this.setExitLocation(location.GetFirstPlayerWarp());
            Game1.fadeScreenToBlack();
            Game1.eventOver = true;
            this.CurrentCommand += 2;
            Game1.screenGlowHold = false;
            break;
          }
          break;
      }
    }
    this.exitEvent();
  }

  public void exitEvent()
  {
    this.eventFinished = true;
    if (!string.IsNullOrEmpty(this.id) && this.id != "-1")
    {
      if (this.markEventSeen)
        Game1.player.eventsSeen.Add(this.id);
      Game1.eventsSeenSinceLastLocationChange.Add(this.id);
    }
    this.notifyDone();
    Game1.stopMusicTrack(MusicContext.Event);
    this.StopTrackedSounds();
    if (this.id == "1039573")
    {
      Game1.addMail("addedParrotBoy", true, true);
      Game1.player.team.requestAddCharacterEvent.Fire("Leo");
    }
    Game1.player.ignoreCollisions = false;
    Game1.player.canOnlyWalk = false;
    Game1.nonWarpFade = true;
    if (!Game1.fadeIn || (double) Game1.fadeToBlackAlpha >= 1.0)
      Game1.fadeScreenToBlack();
    Game1.eventOver = true;
    Game1.fadeToBlack = true;
    Game1.setBGColor((byte) 5, (byte) 3, (byte) 4);
    this.CurrentCommand += 2;
    Game1.screenGlowHold = false;
    if (this.isFestival)
    {
      Game1.timeOfDayAfterFade = 2200;
      if (this.festivalData != null && (this.isSpecificFestival("summer28") || this.isSpecificFestival("fall27")))
        Game1.timeOfDayAfterFade = 2400;
      int minutesBetweenTimes = Utility.CalculateMinutesBetweenTimes(Game1.timeOfDay, Game1.timeOfDayAfterFade);
      if (Game1.IsMasterGame)
      {
        Point mainFarmHouseEntry = Game1.getFarm().GetMainFarmHouseEntry();
        this.setExitLocation("Farm", mainFarmHouseEntry.X, mainFarmHouseEntry.Y);
      }
      else
      {
        Point porchStandingSpot = Utility.getHomeOfFarmer(Game1.player).getPorchStandingSpot();
        this.setExitLocation("Farm", porchStandingSpot.X, porchStandingSpot.Y);
      }
      Game1.player.toolOverrideFunction = (AnimatedSprite.endOfAnimationBehavior) null;
      this.isFestival = false;
      foreach (NPC actor in this.actors)
      {
        if (actor != null)
          this.resetDialogueIfNecessary(actor);
      }
      if (Game1.IsMasterGame)
      {
        foreach (NPC allVillager in Utility.getAllVillagers())
        {
          if (allVillager.getSpouse() != null)
          {
            Farmer spouse = allVillager.getSpouse();
            if (spouse.isMarriedOrRoommates())
            {
              allVillager.controller = (PathFindController) null;
              allVillager.temporaryController = (PathFindController) null;
              FarmHouse homeOfFarmer = Utility.getHomeOfFarmer(spouse);
              allVillager.Halt();
              Game1.warpCharacter(allVillager, (GameLocation) homeOfFarmer, Utility.PointToVector2(homeOfFarmer.getSpouseBedSpot(spouse.spouse)));
              if (homeOfFarmer.GetSpouseBed() != null)
                FarmHouse.spouseSleepEndFunction((Character) allVillager, (GameLocation) Utility.getHomeOfFarmer(spouse));
              allVillager.ignoreScheduleToday = true;
              if (Game1.timeOfDayAfterFade >= 1800)
              {
                allVillager.currentMarriageDialogue.Clear();
                allVillager.checkForMarriageDialogue(1800, (GameLocation) Utility.getHomeOfFarmer(spouse));
                continue;
              }
              if (Game1.timeOfDayAfterFade >= 1100)
              {
                allVillager.currentMarriageDialogue.Clear();
                allVillager.checkForMarriageDialogue(1100, (GameLocation) Utility.getHomeOfFarmer(spouse));
                continue;
              }
              continue;
            }
          }
          if (allVillager.currentLocation != null && allVillager.defaultMap.Value != null)
          {
            allVillager.doingEndOfRouteAnimation.Value = false;
            allVillager.nextEndOfRouteMessage = (string) null;
            allVillager.endOfRouteMessage.Value = (string) null;
            allVillager.controller = (PathFindController) null;
            allVillager.temporaryController = (PathFindController) null;
            allVillager.Halt();
            Game1.warpCharacter(allVillager, allVillager.defaultMap.Value, allVillager.DefaultPosition / 64f);
            allVillager.ignoreScheduleToday = true;
          }
        }
      }
      foreach (GameLocation location in (IEnumerable<GameLocation>) Game1.locations)
      {
        foreach (Vector2 key in new List<Vector2>((IEnumerable<Vector2>) location.objects.Keys))
        {
          if (location.objects[key].minutesElapsed(minutesBetweenTimes))
            location.objects.Remove(key);
        }
        if (location is Farm farm)
          farm.timeUpdate(minutesBetweenTimes);
      }
      Game1.player.freezePause = 1500;
    }
    else
      Game1.player.forceCanMove();
  }

  public void notifyDone()
  {
    if (this.id == "-1" || string.IsNullOrEmpty(this.id) || !this.notifyWhenDone || this.notifyLocationName == null || !Game1.HasDedicatedHost || Game1.client == null)
      return;
    Game1.client.sendMessage((byte) 33, (object) (byte) 0, (object) this.notifyLocationName, (object) this.notifyLocationIsStructure, (object) this.id);
    this.notifyWhenDone = false;
  }

  public void resetDialogueIfNecessary(NPC n)
  {
    if (!Game1.player.hasTalkedToFriendToday(n.Name))
      n.resetCurrentDialogue();
    else
      n.CurrentDialogue?.Clear();
  }

  public void incrementCommandAfterFade()
  {
    ++this.CurrentCommand;
    Game1.globalFade = false;
  }

  public void cleanup()
  {
    Game1.ambientLight = this.previousAmbientLight;
    this._festivalTexture = (Texture2D) null;
    this.festivalContent.Unload();
  }

  private void changeLocation(string locationName, int x, int y, Action onComplete = null)
  {
    Event e = Game1.currentLocation.currentEvent;
    Game1.currentLocation.currentEvent = (Event) null;
    LocationRequest locationRequest = Game1.getLocationRequest(locationName);
    locationRequest.OnLoad += (LocationRequest.Callback) (() =>
    {
      if (!e.isFestival)
        Game1.currentLocation.currentEvent = e;
      this.temporaryLocation = (GameLocation) null;
      Action action = onComplete;
      if (action != null)
        action();
      locationRequest.Location.ResetForEvent(this);
    });
    locationRequest.OnWarp += (LocationRequest.Callback) (() =>
    {
      this.farmer.currentLocation = Game1.currentLocation;
      if (!e.isFestival)
        return;
      Game1.currentLocation.currentEvent = e;
    });
    Game1.warpFarmer(locationRequest, x, y, this.farmer.FacingDirection);
  }

  /// <summary>Log an error indicating that an event command format is invalid.</summary>
  /// <param name="args">The space-delimited event command string, including the command name.</param>
  /// <param name="error">The error to log.</param>
  /// <param name="willSkip">Whether the event command will be skipped entirely. If false, the event command will be applied without the argument(s) that failed. This only affects the wording of the message logged.</param>
  public void LogCommandError(string[] args, string error, bool willSkip = false)
  {
    IGameLogger log = Game1.log;
    string error1;
    if (!willSkip)
      error1 = $"Event '{this.id}' has command '{string.Join(" ", args)}' which reported errors: {error}.";
    else
      error1 = $"Event '{this.id}' has command '{string.Join(" ", args)}' which couldn't be parsed: {error}.";
    log.Error(error1);
  }

  /// <summary>Log an error indicating that a command format is invalid and skip the current command.</summary>
  /// <param name="args">The space-delimited event command string, including the command name.</param>
  /// <param name="error">The error to log.</param>
  /// <param name="hideError">Whether to skip without logging an error message.</param>
  public void LogCommandErrorAndSkip(string[] args, string error, bool hideError = false)
  {
    if (!hideError)
      this.LogCommandError(args, error, true);
    ++this.CurrentCommand;
  }

  /// <summary>Log an error indicating that the entire event has failed, and immediately stop the event.</summary>
  /// <param name="error">An error message indicating why the event failed.</param>
  /// <param name="e">The exception which caused the error, if applicable.</param>
  public void LogErrorAndHalt(string error, Exception e = null)
  {
    string str = $"Error running event script {this.fromAssetName}#{this.id}";
    Game1.chatBox.addErrorMessage("Event script error: " + error);
    string currentCommand = this.GetCurrentCommand();
    if (currentCommand != null)
    {
      str += $" on line #{this.CurrentCommand} ({currentCommand})";
      Game1.chatBox.addErrorMessage($"On line #{this.CurrentCommand}: {currentCommand}");
    }
    Game1.log.Error(str + ".", e);
    this.skipEvent();
  }

  /// <summary>Log an error indicating that the entire event has failed, and immediately stop the event.</summary>
  /// <param name="e">The exception which caused the error.</param>
  public void LogErrorAndHalt(Exception e)
  {
    this.LogErrorAndHalt(e?.Message ?? "An unknown error occurred.", e);
  }

  /// <summary>Log an error indicating that an event precondition is invalid.</summary>
  /// <param name="location">The location containing the event.</param>
  /// <param name="eventId">The unique event ID whose preconditions are being checked.</param>
  /// <param name="args">The precondition arguments, including the precondition key at the zeroth index.</param>
  /// <param name="error">The error phrase indicating why the precondition is invalid.</param>
  /// <returns>Returns false to simplify failing the precondition.</returns>
  public static bool LogPreconditionError(
    GameLocation location,
    string eventId,
    string[] args,
    string error)
  {
    Game1.log.Error($"Event '{eventId}' in location '{location.NameOrUniqueName}' has invalid event precondition '{string.Join(" ", args)}': {error}.");
    return false;
  }

  /// <summary>Update the event state.</summary>
  /// <param name="location">The location in which the event is running.</param>
  /// <param name="time">The current game execution time.</param>
  public void Update(GameLocation location, GameTime time)
  {
    try
    {
      if (this.eventFinished)
        return;
      int num = this.CurrentCommand != 0 || this.forked ? 0 : (!this.eventSwitched ? 1 : 0);
      if (num != 0)
        this.InitializeEvent(location, time);
      if (!(num == 0 & this.UpdateBeforeNextCommand(location, time)))
        return;
      this.CheckForNextCommand(location, time);
    }
    catch (Exception ex)
    {
      this.LogErrorAndHalt(ex);
    }
  }

  /// <summary>Initialize the event when it first starts.</summary>
  /// <param name="location">The location in which the event is running.</param>
  /// <param name="time">The current game execution time.</param>
  protected void InitializeEvent(GameLocation location, GameTime time)
  {
    this.farmer.speed = 2;
    this.farmer.running = false;
    Game1.eventOver = false;
    string newTrackName;
    string error;
    string str1;
    string description;
    string str2;
    if (!ArgUtility.TryGet(this.eventCommands, 0, out newTrackName, out error, name: "string musicId") || !ArgUtility.TryGet(this.eventCommands, 1, out str1, out error, false, "string rawCameraPosition") || !ArgUtility.TryGet(this.eventCommands, 2, out description, out error, false, "string rawCharacterPositions") || !ArgUtility.TryGetOptional(this.eventCommands, 3, out str2, out error, name: "string rawOption"))
    {
      Game1.log.Error($"Event '{this.id}' has initial fields '{string.Join("/", ((IEnumerable<string>) this.eventCommands).Take<string>(3))}' which couldn't be parsed: {error}.");
      this.LogErrorAndHalt("event script is invalid");
    }
    else
    {
      if (string.IsNullOrWhiteSpace(newTrackName))
        newTrackName = "none";
      Point point;
      if (str1 != "follow")
      {
        string[] array = ArgUtility.SplitBySpace(str1);
        if (!ArgUtility.TryGetPoint(array, 0, out point, out error, "cameraPosition"))
        {
          Game1.log.Error($"Event '{this.id}' has initial fields '{string.Join("/", ((IEnumerable<string>) this.eventCommands).Take<string>(3))}' with camera value '{string.Join(" ", array)}' which couldn't be parsed (must be 'follow' or tile coordinates): {error}.");
          this.LogErrorAndHalt("event script is invalid");
          return;
        }
      }
      else
        point = new Point(-1000, -1000);
      if (str2 == "ignoreEventTileOffset")
        this.ignoreTileOffsets = true;
      if ((newTrackName != "none" || !Game1.isRaining) && newTrackName != "continue" && !newTrackName.Contains("pause"))
        Game1.changeMusicTrack(newTrackName, music_context: MusicContext.Event);
      if (location is Farm && point.X >= -1000 && this.id != "-2" && !this.ignoreTileOffsets)
      {
        Point positionForFarmer = Farm.getFrontDoorPositionForFarmer(this.farmer);
        positionForFarmer.X *= 64 /*0x40*/;
        positionForFarmer.Y *= 64 /*0x40*/;
        ref xTile.Dimensions.Rectangle local1 = ref Game1.viewport;
        Viewport viewport;
        int num1;
        if (!Game1.currentLocation.IsOutdoors)
        {
          num1 = positionForFarmer.X - Game1.graphics.GraphicsDevice.Viewport.Width / 2;
        }
        else
        {
          int x = positionForFarmer.X;
          viewport = Game1.graphics.GraphicsDevice.Viewport;
          int num2 = viewport.Width / 2;
          int val1 = x - num2;
          int displayWidth = Game1.currentLocation.Map.DisplayWidth;
          viewport = Game1.graphics.GraphicsDevice.Viewport;
          int width = viewport.Width;
          int val2 = displayWidth - width;
          num1 = Math.Max(0, Math.Min(val1, val2));
        }
        local1.X = num1;
        ref xTile.Dimensions.Rectangle local2 = ref Game1.viewport;
        int num3;
        if (!Game1.currentLocation.IsOutdoors)
        {
          int y = positionForFarmer.Y;
          viewport = Game1.graphics.GraphicsDevice.Viewport;
          int num4 = viewport.Height / 2;
          num3 = y - num4;
        }
        else
        {
          int y = positionForFarmer.Y;
          viewport = Game1.graphics.GraphicsDevice.Viewport;
          int num5 = viewport.Height / 2;
          int val1 = y - num5;
          int displayHeight = Game1.currentLocation.Map.DisplayHeight;
          viewport = Game1.graphics.GraphicsDevice.Viewport;
          int height = viewport.Height;
          int val2 = displayHeight - height;
          num3 = Math.Max(0, Math.Min(val1, val2));
        }
        local2.Y = num3;
      }
      else if (str1 != "follow")
      {
        try
        {
          Game1.viewportFreeze = true;
          int num6 = this.OffsetTileX(point.X) * 64 /*0x40*/ + 32 /*0x20*/;
          int num7 = this.OffsetTileY(point.Y) * 64 /*0x40*/ + 32 /*0x20*/;
          if (num6 < 0)
          {
            Game1.viewport.X = num6;
            Game1.viewport.Y = num7;
          }
          else
          {
            Game1.viewport.X = Game1.currentLocation.IsOutdoors ? Math.Max(0, Math.Min(num6 - Game1.viewport.Width / 2, Game1.currentLocation.Map.DisplayWidth - Game1.viewport.Width)) : num6 - Game1.viewport.Width / 2;
            Game1.viewport.Y = Game1.currentLocation.IsOutdoors ? Math.Max(0, Math.Min(num7 - Game1.viewport.Height / 2, Game1.currentLocation.Map.DisplayHeight - Game1.viewport.Height)) : num7 - Game1.viewport.Height / 2;
          }
          if (num6 > 0 && Game1.graphics.GraphicsDevice.Viewport.Width > Game1.currentLocation.Map.DisplayWidth)
            Game1.viewport.X = (Game1.currentLocation.Map.DisplayWidth - Game1.viewport.Width) / 2;
          if (num7 > 0)
          {
            if (Game1.graphics.GraphicsDevice.Viewport.Height > Game1.currentLocation.Map.DisplayHeight)
              Game1.viewport.Y = (Game1.currentLocation.Map.DisplayHeight - Game1.viewport.Height) / 2;
          }
        }
        catch (Exception ex)
        {
          this.forked = true;
          return;
        }
      }
      this.setUpCharacters(description, location);
      this.trySpecialSetUp(location);
      this.populateWalkLocationsList();
      this.CurrentCommand = 3;
    }
  }

  /// <summary>Run any updates needed before checking for the next script command.</summary>
  /// <param name="location">The location in which the event is running.</param>
  /// <param name="time">The current game execution time.</param>
  /// <returns>Returns whether to run the next command.</returns>
  protected bool UpdateBeforeNextCommand(GameLocation location, GameTime time)
  {
    if (this.skipped || Game1.farmEvent != null)
      return false;
    foreach (NPC actor in this.actors)
    {
      actor.update(time, Game1.currentLocation);
      if (actor.Sprite.CurrentAnimation != null)
        actor.Sprite.animateOnce(time);
    }
    TemporaryAnimatedSpriteList aboveMapSprites = this.aboveMapSprites;
    if (aboveMapSprites != null)
      aboveMapSprites.RemoveWhere<TemporaryAnimatedSprite>((Predicate<TemporaryAnimatedSprite>) (sprite => sprite.update(time)));
    if (this.underwaterSprites != null)
    {
      foreach (TemporaryAnimatedSprite underwaterSprite in this.underwaterSprites)
        underwaterSprite.update(time);
    }
    if (!this.playerControlSequence)
      this.farmer.setRunning(false);
    if (this.npcControllers != null)
    {
      for (int index = this.npcControllers.Count - 1; index >= 0; --index)
      {
        this.npcControllers[index].puppet.isCharging = !this.isFestival;
        if (this.npcControllers[index].update(time, location, this.npcControllers))
          this.npcControllers.RemoveAt(index);
      }
    }
    if (this.isFestival)
      this.festivalUpdate(time);
    if (this.temporaryLocation != null && !Game1.currentLocation.Equals(this.temporaryLocation))
      this.temporaryLocation.updateEvenIfFarmerIsntHere(time, true);
    if (!Game1.fadeToBlack || this.actorPositionsAfterMove.Count > 0 || this.CurrentCommand > 3 || this.forked)
    {
      if (this.eventCommands.Length <= this.CurrentCommand)
        return false;
      if (this.viewportTarget != Vector3.Zero)
      {
        int speed = this.farmer.speed;
        this.farmer.speed = (int) this.viewportTarget.X;
        int x = Game1.viewport.X;
        Game1.viewport.X += (int) this.viewportTarget.X;
        if (x > 0 && Game1.viewport.X <= 0 && location.IsOutdoors)
        {
          Game1.viewport.X = 0;
          this.viewportTarget.X = 0.0f;
        }
        else if (x < location.map.DisplayWidth - Game1.viewport.Width && Game1.viewport.X >= location.Map.DisplayWidth - Game1.viewport.Width)
        {
          Game1.viewport.X = location.Map.DisplayWidth - Game1.viewport.Width;
          this.viewportTarget.X = 0.0f;
        }
        if ((double) this.viewportTarget.X != 0.0)
          Game1.updateRainDropPositionForPlayerMovement((double) this.viewportTarget.X < 0.0 ? 3 : 1, Math.Abs(this.viewportTarget.X + (!this.farmer.isMoving() || this.farmer.FacingDirection != 3 ? (!this.farmer.isMoving() || this.farmer.FacingDirection != 1 ? 0.0f : (float) this.farmer.speed) : (float) -this.farmer.speed)));
        int y = Game1.viewport.Y;
        Game1.viewport.Y += (int) this.viewportTarget.Y;
        if (y > 0 && Game1.viewport.Y <= 0 && location.IsOutdoors)
        {
          Game1.viewport.Y = 0;
          this.viewportTarget.Y = 0.0f;
        }
        else if (y < location.map.DisplayHeight - Game1.viewport.Height && Game1.viewport.Y >= location.Map.DisplayHeight - Game1.viewport.Height)
        {
          Game1.viewport.Y = location.Map.DisplayHeight - Game1.viewport.Height;
          this.viewportTarget.Y = 0.0f;
        }
        this.farmer.speed = (int) this.viewportTarget.Y;
        if ((double) this.viewportTarget.Y != 0.0)
          Game1.updateRainDropPositionForPlayerMovement((double) this.viewportTarget.Y < 0.0 ? 0 : 2, Math.Abs(this.viewportTarget.Y - (!this.farmer.isMoving() || this.farmer.FacingDirection != 0 ? (!this.farmer.isMoving() || this.farmer.FacingDirection != 2 ? 0.0f : (float) this.farmer.speed) : (float) -this.farmer.speed)));
        this.farmer.speed = speed;
        this.viewportTarget.Z -= (float) time.ElapsedGameTime.Milliseconds;
        if ((double) this.viewportTarget.Z <= 0.0)
          this.viewportTarget = Vector3.Zero;
      }
      if (this.actorPositionsAfterMove.Count > 0)
      {
        foreach (string str in this.actorPositionsAfterMove.Keys.ToArray<string>())
        {
          Microsoft.Xna.Framework.Rectangle rectangle = new Microsoft.Xna.Framework.Rectangle((int) this.actorPositionsAfterMove[str].X * 64 /*0x40*/, (int) this.actorPositionsAfterMove[str].Y * 64 /*0x40*/, 64 /*0x40*/, 64 /*0x40*/);
          rectangle.Inflate(-4, 0);
          NPC actorByName = this.getActorByName(str);
          if (actorByName != null)
          {
            Microsoft.Xna.Framework.Rectangle boundingBox = actorByName.GetBoundingBox();
            if (boundingBox.Width > 64 /*0x40*/)
            {
              rectangle.Inflate(4, 0);
              rectangle.Width = boundingBox.Width + 4;
              rectangle.Height = boundingBox.Height + 4;
              rectangle.X += 8;
              rectangle.Y += 16 /*0x10*/;
            }
          }
          int farmerNumber;
          if (this.IsFarmerActorId(str, out farmerNumber))
          {
            Farmer farmerActor = this.GetFarmerActor(farmerNumber);
            if (farmerActor != null)
            {
              Microsoft.Xna.Framework.Rectangle boundingBox = farmerActor.GetBoundingBox();
              float movementSpeed = farmerActor.getMovementSpeed();
              if (rectangle.Contains(boundingBox) && ((double) (boundingBox.Y - rectangle.Top) <= 16.0 + (double) movementSpeed && farmerActor.FacingDirection != 2 || (double) (rectangle.Bottom - boundingBox.Bottom) <= 16.0 + (double) movementSpeed && farmerActor.FacingDirection == 2))
              {
                farmerActor.showNotCarrying();
                farmerActor.Halt();
                farmerActor.faceDirection((int) this.actorPositionsAfterMove[str].Z);
                farmerActor.FarmerSprite.StopAnimation();
                farmerActor.Halt();
                this.actorPositionsAfterMove.Remove(str);
              }
              else if (farmerActor != null)
              {
                farmerActor.canOnlyWalk = false;
                farmerActor.setRunning(false, true);
                farmerActor.canOnlyWalk = true;
                farmerActor.lastPosition = this.farmer.Position;
                farmerActor.MovePosition(time, Game1.viewport, location);
              }
            }
          }
          else
          {
            foreach (NPC actor in this.actors)
            {
              Microsoft.Xna.Framework.Rectangle boundingBox = actor.GetBoundingBox();
              if (actor.Name.Equals(str) && rectangle.Contains(boundingBox) && boundingBox.Y - rectangle.Top <= 16 /*0x10*/)
              {
                actor.Halt();
                actor.faceDirection((int) this.actorPositionsAfterMove[str].Z);
                this.actorPositionsAfterMove.Remove(str);
                break;
              }
              if (actor.Name.Equals(str))
              {
                if (actor is Monster)
                {
                  actor.MovePosition(time, Game1.viewport, location);
                  break;
                }
                actor.MovePosition(time, Game1.viewport, (GameLocation) null);
                break;
              }
            }
          }
        }
        if (this.actorPositionsAfterMove.Count == 0)
        {
          if (this.continueAfterMove)
            this.continueAfterMove = false;
          else
            ++this.CurrentCommand;
        }
        if (!this.continueAfterMove)
          return false;
      }
    }
    return true;
  }

  protected void CheckForNextCommand(GameLocation location, GameTime time)
  {
    string[] strArray = ArgUtility.SplitBySpaceQuoteAware(this.eventCommands[Math.Min(this.eventCommands.Length - 1, this.CurrentCommand)]);
    string str = ArgUtility.Get(strArray, 0);
    int num = str != null ? (str.StartsWith("--") ? 1 : 0) : 0;
    if (this.temporaryLocation != null && !Game1.currentLocation.Equals(this.temporaryLocation))
      this.temporaryLocation.updateEvenIfFarmerIsntHere(time, true);
    if (num != 0)
      ++this.CurrentCommand;
    else
      this.tryEventCommand(location, time, strArray);
  }

  /// <summary>Get the text of the current event command being executed.</summary>
  public string GetCurrentCommand() => ArgUtility.Get(this.eventCommands, this.currentCommand);

  /// <summary>Replace the command at the current index.</summary>
  /// <param name="command">The new command text to parse.</param>
  public void ReplaceCurrentCommand(string command)
  {
    if (!ArgUtility.HasIndex<string>(this.eventCommands, this.currentCommand))
      return;
    this.eventCommands[this.currentCommand] = command;
  }

  /// <summary>Replace the entire list of commands with the given values.</summary>
  /// <param name="commands">The new commands to parse.</param>
  public void ReplaceAllCommands(params string[] commands)
  {
    this.eventCommands = commands;
    this.CurrentCommand = 0;
  }

  /// <summary>Add a new event command to run after the current one.</summary>
  /// <param name="command">The new command text to parse.</param>
  public void InsertNextCommand(string command)
  {
    int index = this.currentCommand + 1;
    List<string> list = ((IEnumerable<string>) this.eventCommands).ToList<string>();
    if (index <= list.Count)
      list.Insert(index, command);
    else
      list.Add(command);
    this.eventCommands = list.ToArray();
  }

  /// <summary>Register a sound cue to remove when the event ends.</summary>
  /// <param name="cue">The audio cue to register.</param>
  public void TrackSound(ICue cue)
  {
    if (cue == null)
      return;
    List<ICue> cueList;
    if (!this.CustomSounds.TryGetValue(cue.Name, out cueList))
      this.CustomSounds[cue.Name] = cueList = new List<ICue>();
    cueList.Add(cue);
  }

  /// <summary>Stop a tracked sound registered via <see cref="M:StardewValley.Event.TrackSound(StardewValley.ICue)" />.</summary>
  /// <param name="cueId">The audio cue ID to stop.</param>
  /// <param name="immediate">Whether to stop the sound immediately, instead of letting it finish the current loop.</param>
  public void StopTrackedSound(string cueId, bool immediate)
  {
    List<ICue> cueList;
    if (cueId == null || !this.CustomSounds.TryGetValue(cueId, out cueList))
      return;
    foreach (ICue cue in cueList)
      cue.Stop(immediate ? AudioStopOptions.Immediate : AudioStopOptions.AsAuthored);
    if (!immediate)
      return;
    this.CustomSounds.Remove(cueId);
  }

  /// <summary>Stop all tracked sounds registered via <see cref="M:StardewValley.Event.TrackSound(StardewValley.ICue)" />.</summary>
  public void StopTrackedSounds()
  {
    foreach (List<ICue> cueList in (IEnumerable<List<ICue>>) this.CustomSounds.Values)
    {
      foreach (ICue cue in cueList)
        cue.Stop(AudioStopOptions.Immediate);
    }
    this.CustomSounds.Clear();
  }

  public bool isTileWalkedOn(int x, int y)
  {
    return this.characterWalkLocations.Contains(new Vector2((float) x, (float) y));
  }

  private void populateWalkLocationsList()
  {
    this.characterWalkLocations.Add(this.farmer.Tile);
    foreach (Character actor in this.actors)
      this.characterWalkLocations.Add(actor.Tile);
    for (int index1 = 2; index1 < this.eventCommands.Length; ++index1)
    {
      string[] strArray = ArgUtility.SplitBySpace(this.eventCommands[index1]);
      if (!(ArgUtility.Get(strArray, 0) != "move") && (!(ArgUtility.Get(strArray, 1) == "false") || strArray.Length != 2))
      {
        string str;
        string error;
        Point point;
        if (!ArgUtility.TryGet(strArray, 1, out str, out error, name: "string actorName") || !ArgUtility.TryGetPoint(strArray, 2, out point, out error, "Point position"))
        {
          this.LogCommandError(strArray, error);
        }
        else
        {
          Character character = this.IsCurrentFarmerActorId(str) ? (Character) this.farmer : (Character) this.getActorByName(str);
          if (character != null)
          {
            Vector2 tile = character.Tile;
            for (int index2 = 0; index2 < Math.Abs(point.X); ++index2)
            {
              tile.X += (float) Math.Sign(point.X);
              this.characterWalkLocations.Add(tile);
            }
            for (int index3 = 0; index3 < Math.Abs(point.Y); ++index3)
            {
              tile.Y += (float) Math.Sign(point.Y);
              this.characterWalkLocations.Add(tile);
            }
          }
        }
      }
    }
  }

  /// <summary>Get an NPC actor in the event by its name.</summary>
  /// <param name="name">The actor name.</param>
  /// <param name="legacyReplaceUnderscores">Whether to try replacing underscores with spaces in <paramref name="name" /> if an exact match wasn't found. This is only meant for backwards compatibility, for event commands which predate argument quoting.</param>
  /// <returns>Returns the matching actor, else <c>null</c>.</returns>
  public NPC getActorByName(string name, bool legacyReplaceUnderscores = false)
  {
    return this.getActorByName(name, out bool _, legacyReplaceUnderscores);
  }

  /// <summary>Get an NPC actor in the event by its name.</summary>
  /// <param name="name">The actor name.</param>
  /// <param name="isOptionalNpc">Whether the NPC is marked optional, so no error should be shown if they're missing.</param>
  /// <param name="legacyReplaceUnderscores">Whether to try replacing underscores with spaces in <paramref name="name" /> if an exact match wasn't found. This is only meant for backwards compatibility, for event commands which predate argument quoting.</param>
  /// <returns>Returns the matching actor, else <c>null</c>.</returns>
  public NPC getActorByName(string name, out bool isOptionalNpc, bool legacyReplaceUnderscores = false)
  {
    ref bool local = ref isOptionalNpc;
    bool? nullable = name?.EndsWith('?');
    int num = !nullable.HasValue ? 0 : (nullable.GetValueOrDefault() ? 1 : 0);
    local = num != 0;
    if (isOptionalNpc)
      name = name.Substring(0, name.Length - 1);
    switch (name)
    {
      case null:
        return (NPC) null;
      case "spouse":
        name = this.farmer.spouse;
        break;
    }
    foreach (NPC actor in this.actors)
    {
      if (actor.Name == name)
        return actor;
    }
    if (legacyReplaceUnderscores)
    {
      string str = name.Replace('_', ' ');
      if (str != name)
      {
        foreach (NPC actor in this.actors)
        {
          if (actor.Name == str)
            return actor;
        }
      }
    }
    return (NPC) null;
  }

  private void addActor(string name, int x, int y, int facingDirection, GameLocation location)
  {
    bool isOptionalNpc;
    NPC actorByName = this.getActorByName(name, out isOptionalNpc);
    if (actorByName != null)
    {
      actorByName.Position = new Vector2((float) (x * 64 /*0x40*/), (float) (y * 64 /*0x40*/));
      actorByName.FacingDirection = facingDirection;
    }
    else
    {
      if (isOptionalNpc)
      {
        name = name.Substring(0, name.Length - 1);
        CharacterData data;
        if (!NPC.TryGetData(name, out data) || !GameStateQuery.CheckConditions(data.UnlockConditions))
          return;
      }
      NPC npc;
      try
      {
        string nameForCharacter = NPC.getTextureNameForCharacter(name);
        Texture2D portrait = (Texture2D) null;
        try
        {
          portrait = Game1.content.Load<Texture2D>("Portraits\\" + nameForCharacter);
        }
        catch (Exception ex)
        {
        }
        int num = name.Contains("Dwarf") || name.Equals("Krobus") ? 96 /*0x60*/ : 128 /*0x80*/;
        npc = new NPC(new AnimatedSprite("Characters\\" + nameForCharacter, 0, 16 /*0x10*/, num / 4), new Vector2((float) (x * 64 /*0x40*/), (float) (y * 64 /*0x40*/)), location.Name, facingDirection, name, portrait, true);
        npc.EventActor = true;
        if (this.isFestival)
        {
          try
          {
            Dialogue dialogue;
            if (this.TryGetFestivalDialogueForYear(npc, npc.Name, out dialogue))
              npc.setNewDialogue(dialogue);
          }
          catch (Exception ex)
          {
          }
        }
        if (npc.name.Equals((object) "MrQi"))
          npc.displayName = Game1.content.LoadString("Strings\\NPCNames:MisterQi");
      }
      catch (Exception ex)
      {
        Game1.log.Error($"Event '{this.id}' has character '{name}' which couldn't be added.", ex);
        return;
      }
      npc.EventActor = true;
      this.actors.Add(npc);
    }
  }

  /// <summary>Get the player in the event matching a farmer number, if found.</summary>
  /// <param name="farmerNumber">The farmer number. This can be -1 (current player), 1 (main player), or higher numbers for farmhands.</param>
  /// <returns>Returns the matching event actor or real farmer, or <c>null</c> if neither was found.</returns>
  public Farmer GetFarmerActor(int farmerNumber)
  {
    Farmer farmerActor1 = farmerNumber < 1 ? this.farmer : Utility.getFarmerFromFarmerNumber(farmerNumber);
    if (farmerActor1 == null)
      return (Farmer) null;
    foreach (Farmer farmerActor2 in this.farmerActors)
    {
      if (farmerActor2.UniqueMultiplayerID == farmerActor1.UniqueMultiplayerID)
        return farmerActor2;
    }
    return farmerActor1;
  }

  /// <summary>Get whether an actor ID is the current player.</summary>
  /// <param name="actor">The actor ID to check.</param>
  public bool IsCurrentFarmerActorId(string actor)
  {
    int farmerNumber;
    return this.IsFarmerActorId(actor, out farmerNumber) && this.IsCurrentFarmerActorId(farmerNumber);
  }

  /// <summary>Get whether an actor ID is the current player.</summary>
  /// <param name="farmerNumber">The farmer number to check.</param>
  public bool IsCurrentFarmerActorId(int farmerNumber)
  {
    return farmerNumber < 1 || farmerNumber == Utility.getFarmerNumberFromFarmer(Game1.player);
  }

  /// <summary>Get whether an actor ID is a farmer like <c>farmer</c> (current player) or <samp>farmer3</samp> (player #3), regardless of whether that player is present.</summary>
  /// <param name="actor">The actor ID to check.</param>
  /// <param name="farmerNumber">The parsed farmer number, if applicable. This can be <samp>-1</samp> (current player), 1 (main player), or higher numbers for farmhands.</param>
  public bool IsFarmerActorId(string actor, out int farmerNumber)
  {
    if (actor == null || !actor.StartsWith("farmer"))
    {
      farmerNumber = -1;
      return false;
    }
    if (actor.Length != "farmer".Length)
      return int.TryParse(actor.Substring("farmer".Length), out farmerNumber);
    farmerNumber = -1;
    return true;
  }

  public Character getCharacterByName(string name)
  {
    int farmerNumber;
    if (this.IsFarmerActorId(name, out farmerNumber))
      return (Character) this.GetFarmerActor(farmerNumber);
    foreach (NPC actor in this.actors)
    {
      if (actor.Name.Equals(name))
        return (Character) actor;
    }
    return (Character) null;
  }

  public Vector3 getPositionAfterMove(Character c, int xMove, int yMove, int facingDirection)
  {
    Vector2 tile = c.Tile;
    return new Vector3(tile.X + (float) xMove, tile.Y + (float) yMove, (float) facingDirection);
  }

  private void trySpecialSetUp(GameLocation location)
  {
    string id = this.id;
    if (id == null)
      return;
    switch (id.Length)
    {
      case 6:
        if (!(id == "739330"))
          break;
        if (!Game1.player.friendshipData.ContainsKey("Willy"))
          Game1.player.friendshipData.Add("Willy", new Friendship(0));
        NPC willy = Game1.getCharacterFromName("Willy");
        Game1.player.NotifyQuests((Func<Quest, bool>) (quest => quest.OnNpcSocialized(willy)));
        break;
      case 7:
        switch (id[6])
        {
          case '0':
            if (!(id == "9333220") || !(location is FarmHouse farmHouse1) || farmHouse1.upgradeLevel != 1)
              return;
            this.farmer.Position = new Vector2(1920f, 400f);
            this.getActorByName("Sebastian").setTilePosition(31 /*0x1F*/, 6);
            return;
          case '1':
            switch (id)
            {
              case "8675611":
                if (!(location is FarmHouse farmHouse2) || farmHouse2.upgradeLevel != 1)
                  return;
                this.getActorByName("Haley").setTilePosition(4, 5);
                this.farmer.Position = new Vector2(320f, 336f);
                return;
              case "3917601":
                if (!(location is DecoratableLocation decoratableLocation))
                  return;
                foreach (Furniture furniture in decoratableLocation.furniture)
                {
                  if (furniture.furniture_type.Value == 14 && !location.IsTileBlockedBy(furniture.TileLocation + new Vector2(0.0f, 1f), ignorePassables: CollisionMask.All) && !location.IsTileBlockedBy(furniture.TileLocation + new Vector2(1f, 1f), ignorePassables: CollisionMask.All))
                  {
                    this.getActorByName("Emily").setTilePosition((int) furniture.TileLocation.X, (int) furniture.TileLocation.Y + 1);
                    this.farmer.Position = new Vector2((float) (((double) furniture.TileLocation.X + 1.0) * 64.0), (float) (((double) furniture.tileLocation.Y + 1.0) * 64.0 + 16.0));
                    furniture.isOn.Value = true;
                    furniture.setFireplace(false);
                    return;
                  }
                }
                if (!(location is FarmHouse farmHouse3) || farmHouse3.upgradeLevel != 1)
                  return;
                this.getActorByName("Emily").setTilePosition(4, 5);
                this.farmer.Position = new Vector2(320f, 336f);
                return;
              default:
                return;
            }
          case '2':
            if (!(id == "3912132") || !(location is FarmHouse farmHouse4))
              return;
            Point playerBedSpot1 = farmHouse4.GetPlayerBedSpot();
            --playerBedSpot1.X;
            if (!location.CanItemBePlacedHere(Utility.PointToVector2(playerBedSpot1) + new Vector2(-2f, 0.0f)))
              ++playerBedSpot1.X;
            this.farmer.setTileLocation(Utility.PointToVector2(playerBedSpot1));
            this.getActorByName("Elliott").setTileLocation(Utility.PointToVector2(playerBedSpot1) + new Vector2(-2f, 0.0f));
            for (int index = 0; index < this.eventCommands.Length; ++index)
            {
              if (this.eventCommands[index].StartsWith("makeInvisible"))
              {
                string[] strArray1 = ArgUtility.SplitBySpace(this.eventCommands[index]);
                Point point;
                string error;
                if (!ArgUtility.TryGetPoint(strArray1, 1, out point, out error, "Point tile"))
                {
                  this.LogCommandError(strArray1, error);
                }
                else
                {
                  string[] strArray2 = strArray1;
                  int num = point.X - 26 + playerBedSpot1.X;
                  string str1 = num.ToString() ?? "";
                  strArray2[1] = str1;
                  string[] strArray3 = strArray1;
                  num = point.Y - 13 + playerBedSpot1.Y;
                  string str2 = num.ToString() ?? "";
                  strArray3[2] = str2;
                  this.eventCommands[index] = location.getObjectAtTile(point.X, point.Y) != farmHouse4.GetPlayerBed() ? string.Join(" ", strArray1) : "makeInvisible -1000 -1000";
                }
              }
            }
            return;
          case '3':
            if (!(id == "4324303") || !(location is FarmHouse farmHouse5))
              return;
            Point playerBedSpot2 = farmHouse5.GetPlayerBedSpot();
            --playerBedSpot2.X;
            this.farmer.Position = new Vector2((float) (playerBedSpot2.X * 64 /*0x40*/), (float) (playerBedSpot2.Y * 64 /*0x40*/ + 16 /*0x10*/));
            this.getActorByName("Penny").setTilePosition(playerBedSpot2.X - 1, playerBedSpot2.Y);
            Microsoft.Xna.Framework.Rectangle rectangle = new Microsoft.Xna.Framework.Rectangle(23, 12, 10, 10);
            if (farmHouse5.upgradeLevel == 1)
              rectangle = new Microsoft.Xna.Framework.Rectangle(20, 3, 8, 7);
            Point center = rectangle.Center;
            if (!rectangle.Contains(Game1.player.TilePoint))
            {
              List<string> stringList = new List<string>((IEnumerable<string>) this.eventCommands);
              int index1 = 56;
              stringList.Insert(index1, "globalFade 0.03");
              int index2 = index1 + 1;
              stringList.Insert(index2, "beginSimultaneousCommand");
              int index3 = index2 + 1;
              stringList.Insert(index3, $"viewport {center.X.ToString()} {center.Y.ToString()}");
              int index4 = index3 + 1;
              stringList.Insert(index4, "globalFadeToClear 0.03");
              int index5 = index4 + 1;
              stringList.Insert(index5, "endSimultaneousCommand");
              int index6 = index5 + 1;
              stringList.Insert(index6, "pause 2000");
              int index7 = index6 + 1;
              stringList.Insert(index7, "globalFade 0.03");
              int index8 = index7 + 1;
              stringList.Insert(index8, "beginSimultaneousCommand");
              int index9 = index8 + 1;
              stringList.Insert(index9, $"viewport {Game1.player.TilePoint.X.ToString()} {Game1.player.TilePoint.Y.ToString()}");
              int index10 = index9 + 1;
              stringList.Insert(index10, "globalFadeToClear 0.03");
              int index11 = index10 + 1;
              stringList.Insert(index11, "endSimultaneousCommand");
              int num = index11 + 1;
              this.eventCommands = stringList.ToArray();
            }
            for (int index = 0; index < this.eventCommands.Length; ++index)
            {
              if (this.eventCommands[index].StartsWith("makeInvisible"))
              {
                string[] strArray4 = ArgUtility.SplitBySpace(this.eventCommands[index]);
                Point point;
                string error;
                if (!ArgUtility.TryGetPoint(strArray4, 1, out point, out error, "Point tile"))
                {
                  this.LogCommandError(strArray4, error);
                }
                else
                {
                  string[] strArray5 = strArray4;
                  int num = point.X - 26 + playerBedSpot2.X;
                  string str3 = num.ToString() ?? "";
                  strArray5[1] = str3;
                  string[] strArray6 = strArray4;
                  num = point.Y - 13 + playerBedSpot2.Y;
                  string str4 = num.ToString() ?? "";
                  strArray6[2] = str4;
                  this.eventCommands[index] = location.getObjectAtTile(point.X, point.Y) != farmHouse5.GetPlayerBed() ? string.Join(" ", strArray4) : "makeInvisible -1000 -1000";
                }
              }
            }
            return;
          case '4':
            if (!(id == "4325434") || !(location is FarmHouse farmHouse6) || farmHouse6.upgradeLevel != 1)
              return;
            this.farmer.Position = new Vector2(512f, 336f);
            this.getActorByName("Penny").setTilePosition(5, 5);
            return;
          case '5':
            return;
          case '6':
            if (!(id == "3917666") || !(location is FarmHouse farmHouse7) || farmHouse7.upgradeLevel != 1)
              return;
            this.getActorByName("Maru").setTilePosition(4, 5);
            this.farmer.Position = new Vector2(320f, 336f);
            return;
          default:
            return;
        }
    }
  }

  private void setUpCharacters(string description, GameLocation location)
  {
    this.farmer.Halt();
    if (string.IsNullOrEmpty(Game1.player.locationBeforeForcedEvent.Value) && !this.isMemory)
    {
      Game1.player.positionBeforeEvent = Game1.player.Tile;
      Game1.player.orientationBeforeEvent = Game1.player.FacingDirection;
    }
    string[] array = ArgUtility.SplitBySpace(description);
    for (int index = 0; index < array.Length; index += 4)
    {
      string actor;
      string error;
      Point point;
      int direction;
      if (!ArgUtility.TryGet(array, index, out actor, out error, name: "string actorName") || !ArgUtility.TryGetPoint(array, index + 1, out point, out error, "Point tile") || !ArgUtility.TryGetInt(array, index + 3, out direction, out error, "int direction"))
      {
        Game1.log.Error($"Event '{this.id}' has character positions '{string.Join(" ", array)}' which couldn't be parsed: {error}.");
      }
      else
      {
        int farmerNumber;
        bool flag1 = this.IsFarmerActorId(actor, out farmerNumber);
        bool flag2 = flag1 && this.IsCurrentFarmerActorId(farmerNumber);
        if (point.X == -1 && !flag2)
        {
          foreach (NPC character in location.characters)
          {
            if (character.Name == actor)
              this.actors.Add(character);
          }
        }
        else if (actor != "farmer")
        {
          if (actor == "otherFarmers")
          {
            int x = this.OffsetTileX(point.X);
            int y = this.OffsetTileY(point.Y);
            foreach (Farmer onlineFarmer in Game1.getOnlineFarmers())
            {
              if (onlineFarmer.UniqueMultiplayerID != this.farmer.UniqueMultiplayerID)
              {
                Farmer fakeEventFarmer = onlineFarmer.CreateFakeEventFarmer();
                fakeEventFarmer.completelyStopAnimatingOrDoingAction();
                fakeEventFarmer.hidden.Value = false;
                fakeEventFarmer.faceDirection(direction);
                fakeEventFarmer.setTileLocation(new Vector2((float) x, (float) y));
                fakeEventFarmer.currentLocation = Game1.currentLocation;
                ++x;
                this.farmerActors.Add(fakeEventFarmer);
              }
            }
          }
          else if (flag1)
          {
            int x = this.OffsetTileX(point.X);
            int y = this.OffsetTileY(point.Y);
            Farmer farmerActor = this.GetFarmerActor(farmerNumber);
            if (farmerActor != null)
            {
              Farmer fakeEventFarmer = farmerActor.CreateFakeEventFarmer();
              fakeEventFarmer.completelyStopAnimatingOrDoingAction();
              fakeEventFarmer.hidden.Value = false;
              fakeEventFarmer.faceDirection(direction);
              fakeEventFarmer.setTileLocation(new Vector2((float) x, (float) y));
              fakeEventFarmer.currentLocation = Game1.currentLocation;
              fakeEventFarmer.isFakeEventActor = true;
              this.farmerActors.Add(fakeEventFarmer);
            }
          }
          else
          {
            string name = !(actor == "spouse") ? actor : this.farmer.spouse;
            switch (actor)
            {
              case "cat":
                Pet pet1 = new Pet(this.OffsetTileX(point.X), this.OffsetTileY(point.Y), Game1.player.whichPetBreed, "Cat");
                pet1.Name = "Cat";
                pet1.position.X -= 32f;
                this.actors.Add((NPC) pet1);
                continue;
              case "dog":
                Pet pet2 = new Pet(this.OffsetTileX(point.X), this.OffsetTileY(point.Y), Game1.player.whichPetBreed, "Dog");
                pet2.Name = "Dog";
                pet2.position.X -= 42f;
                this.actors.Add((NPC) pet2);
                continue;
              case "pet":
                Pet pet3 = new Pet(this.OffsetTileX(point.X), this.OffsetTileY(point.Y), Game1.player.whichPetBreed, Game1.player.whichPetType);
                pet3.Name = "PetActor";
                PetData data;
                if (Pet.TryGetData(Game1.player.whichPetType, out data))
                  pet3.Position = new Vector2(pet3.Position.X + (float) data.EventOffset.X, pet3.Position.Y + (float) data.EventOffset.Y);
                this.actors.Add((NPC) pet3);
                continue;
              case "golem":
                this.actors.Add(new NPC(new AnimatedSprite("Characters\\Monsters\\Wilderness Golem", 0, 16 /*0x10*/, 24), this.OffsetPosition(new Vector2((float) point.X, (float) point.Y) * 64f), 0, "Golem")
                {
                  AllowDynamicAppearance = false
                });
                continue;
              case "Junimo":
                List<NPC> actors = this.actors;
                Junimo junimo = new Junimo(this.OffsetPosition(new Vector2((float) (point.X * 64 /*0x40*/), (float) (point.Y * 64 /*0x40*/ - 32 /*0x20*/))), Game1.currentLocation.Name.Equals("AbandonedJojaMart") ? 6 : -1);
                junimo.Name = "Junimo";
                junimo.EventActor = true;
                actors.Add((NPC) junimo);
                continue;
              default:
                int x = this.OffsetTileX(point.X);
                int y = this.OffsetTileY(point.Y);
                int facingDirection = direction;
                if (location is Farm && this.id != "-2" && !this.ignoreTileOffsets)
                {
                  x = Farm.getFrontDoorPositionForFarmer(this.farmer).X;
                  y = Farm.getFrontDoorPositionForFarmer(this.farmer).Y + 2;
                  facingDirection = 0;
                }
                this.addActor(name, x, y, facingDirection, location);
                continue;
            }
          }
        }
        else if (point.X != -1)
        {
          this.farmer.position.X = this.OffsetPositionX((float) (point.X * 64 /*0x40*/));
          this.farmer.position.Y = this.OffsetPositionY((float) (point.Y * 64 /*0x40*/ + 16 /*0x10*/));
          this.farmer.faceDirection(direction);
          if (location is Farm && this.id != "-2" && !this.ignoreTileOffsets)
          {
            this.farmer.position.X = (float) (Farm.getFrontDoorPositionForFarmer(this.farmer).X * 64 /*0x40*/);
            this.farmer.position.Y = (float) ((Farm.getFrontDoorPositionForFarmer(this.farmer).Y + 1) * 64 /*0x40*/);
            this.farmer.faceDirection(2);
          }
          this.farmer.FarmerSprite.StopAnimation();
        }
      }
    }
  }

  private void beakerSmashEndFunction(int extraInfo)
  {
    Game1.playSound("breakingGlass");
    Game1.currentLocation.TemporarySprites.Add(new TemporaryAnimatedSprite(47, new Vector2(9f, 16f) * 64f, Color.LightBlue, 10));
    Game1.currentLocation.TemporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(400, 3008, 64 /*0x40*/, 64 /*0x40*/), 99999f, 2, 0, new Vector2(9f, 16f) * 64f, false, false, 0.01f, 0.0f, Color.LightBlue, 1f, 0.0f, 0.0f, 0.0f)
    {
      delayBeforeAnimationStart = 700
    });
    Game1.currentLocation.TemporarySprites.Add(new TemporaryAnimatedSprite(46, new Vector2(9f, 16f) * 64f, Color.White * 0.75f, 10)
    {
      motion = new Vector2(0.0f, -1f)
    });
  }

  private void eggSmashEndFunction(int extraInfo)
  {
    Game1.playSound("slimedead");
    Game1.currentLocation.TemporarySprites.Add(new TemporaryAnimatedSprite(47, new Vector2(9f, 16f) * 64f, Color.White, 10));
    Game1.currentLocation.TemporarySprites.Add(new TemporaryAnimatedSprite(177, 99999f, 9999, 0, new Vector2(6f, 5f) * 64f, false, false)
    {
      layerDepth = 1E-06f
    });
  }

  private void balloonInSky(int extraInfo)
  {
    TemporaryAnimatedSprite temporarySpriteById1 = Game1.currentLocation.getTemporarySpriteByID(2);
    if (temporarySpriteById1 != null)
      temporarySpriteById1.motion = Vector2.Zero;
    TemporaryAnimatedSprite temporarySpriteById2 = Game1.currentLocation.getTemporarySpriteByID(1);
    if (temporarySpriteById2 == null)
      return;
    temporarySpriteById2.motion = Vector2.Zero;
  }

  private void marcelloBalloonLand(int extraInfo)
  {
    Game1.playSound("thudStep");
    Game1.playSound("dirtyHit");
    TemporaryAnimatedSprite temporarySpriteById1 = Game1.currentLocation.getTemporarySpriteByID(2);
    if (temporarySpriteById1 != null)
      temporarySpriteById1.motion = Vector2.Zero;
    TemporaryAnimatedSprite temporarySpriteById2 = Game1.currentLocation.getTemporarySpriteByID(3);
    if (temporarySpriteById2 != null)
      temporarySpriteById2.scaleChange = 0.0f;
    Game1.currentLocation.TemporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(0, 2944, 64 /*0x40*/, 64 /*0x40*/), 120f, 8, 1, (new Vector2(25f, 39f) + this.eventPositionTileOffset) * 64f + new Vector2(-32f, 32f), false, true, 1f, 0.0f, Color.White, 1f, 0.0f, 0.0f, 0.0f));
    Game1.currentLocation.TemporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(0, 2944, 64 /*0x40*/, 64 /*0x40*/), 120f, 8, 1, (new Vector2(27f, 39f) + this.eventPositionTileOffset) * 64f + new Vector2(0.0f, 48f), false, false, 1f, 0.0f, Color.White, 1f, 0.0f, 0.0f, 0.0f)
    {
      delayBeforeAnimationStart = 300
    });
    ++this.CurrentCommand;
  }

  private void samPreOllie(int extraInfo)
  {
    this.getActorByName("Sam").Sprite.currentFrame = 27;
    this.farmer.faceDirection(0);
    TemporaryAnimatedSprite temporarySpriteById = Game1.currentLocation.getTemporarySpriteByID(92473);
    temporarySpriteById.xStopCoordinate = 1408;
    temporarySpriteById.reachedStopCoordinate = new TemporaryAnimatedSprite.endBehavior(this.samOllie);
    temporarySpriteById.motion = new Vector2(2f, 0.0f);
  }

  private void samOllie(int extraInfo)
  {
    Game1.playSound("crafting");
    this.getActorByName("Sam").Sprite.currentFrame = 26;
    TemporaryAnimatedSprite temporarySpriteById = Game1.currentLocation.getTemporarySpriteByID(92473);
    temporarySpriteById.currentNumberOfLoops = 0;
    temporarySpriteById.totalNumberOfLoops = 1;
    temporarySpriteById.motion.Y = -9f;
    temporarySpriteById.motion.X = 2f;
    temporarySpriteById.acceleration = new Vector2(0.0f, 0.4f);
    temporarySpriteById.animationLength = 1;
    temporarySpriteById.interval = 530f;
    temporarySpriteById.timer = 0.0f;
    temporarySpriteById.endFunction = new TemporaryAnimatedSprite.endBehavior(this.samGrind);
    temporarySpriteById.destroyable = false;
  }

  private void samGrind(int extraInfo)
  {
    Game1.playSound("hammer");
    this.getActorByName("Sam").Sprite.currentFrame = 28;
    TemporaryAnimatedSprite temporarySpriteById = Game1.currentLocation.getTemporarySpriteByID(92473);
    temporarySpriteById.currentNumberOfLoops = 0;
    temporarySpriteById.totalNumberOfLoops = 9999;
    temporarySpriteById.motion.Y = 0.0f;
    temporarySpriteById.motion.X = 2f;
    temporarySpriteById.acceleration = new Vector2(0.0f, 0.0f);
    temporarySpriteById.animationLength = 1;
    temporarySpriteById.interval = 99999f;
    temporarySpriteById.timer = 0.0f;
    temporarySpriteById.xStopCoordinate = 1664;
    temporarySpriteById.yStopCoordinate = -1;
    temporarySpriteById.reachedStopCoordinate = new TemporaryAnimatedSprite.endBehavior(this.samDropOff);
  }

  private void samDropOff(int extraInfo)
  {
    NPC actorByName = this.getActorByName("Sam");
    actorByName.Sprite.currentFrame = 31 /*0x1F*/;
    TemporaryAnimatedSprite temporarySpriteById = Game1.currentLocation.getTemporarySpriteByID(92473);
    temporarySpriteById.currentNumberOfLoops = 9999;
    temporarySpriteById.totalNumberOfLoops = 0;
    temporarySpriteById.motion.Y = 0.0f;
    temporarySpriteById.motion.X = 2f;
    temporarySpriteById.acceleration = new Vector2(0.0f, 0.4f);
    temporarySpriteById.animationLength = 1;
    temporarySpriteById.interval = 99999f;
    temporarySpriteById.yStopCoordinate = 5760;
    temporarySpriteById.reachedStopCoordinate = new TemporaryAnimatedSprite.endBehavior(this.samGround);
    temporarySpriteById.endFunction = (TemporaryAnimatedSprite.endBehavior) null;
    actorByName.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
    {
      new FarmerSprite.AnimationFrame(29, 100),
      new FarmerSprite.AnimationFrame(30, 100),
      new FarmerSprite.AnimationFrame(31 /*0x1F*/, 100),
      new FarmerSprite.AnimationFrame(32 /*0x20*/, 100)
    });
    actorByName.Sprite.loop = false;
  }

  private void samGround(int extraInfo)
  {
    TemporaryAnimatedSprite temporarySpriteById = Game1.currentLocation.getTemporarySpriteByID(92473);
    Game1.playSound("thudStep");
    temporarySpriteById.attachedCharacter = (Character) null;
    temporarySpriteById.reachedStopCoordinate = (TemporaryAnimatedSprite.endBehavior) null;
    temporarySpriteById.totalNumberOfLoops = -1;
    temporarySpriteById.interval = 0.0f;
    temporarySpriteById.destroyable = true;
    ++this.CurrentCommand;
  }

  private void catchFootball(int extraInfo)
  {
    TemporaryAnimatedSprite temporarySpriteById = Game1.currentLocation.getTemporarySpriteByID(56232);
    Game1.playSound("fishSlap");
    temporarySpriteById.motion = new Vector2(2f, -8f);
    temporarySpriteById.rotationChange = 0.1308997f;
    temporarySpriteById.reachedStopCoordinate = new TemporaryAnimatedSprite.endBehavior(this.footballLand);
    temporarySpriteById.yStopCoordinate = 1088;
    this.farmer.jump();
  }

  private void footballLand(int extraInfo)
  {
    TemporaryAnimatedSprite temporarySpriteById = Game1.currentLocation.getTemporarySpriteByID(56232);
    Game1.playSound("sandyStep");
    temporarySpriteById.motion = new Vector2(0.0f, 0.0f);
    temporarySpriteById.rotationChange = 0.0f;
    temporarySpriteById.reachedStopCoordinate = (TemporaryAnimatedSprite.endBehavior) null;
    temporarySpriteById.animationLength = 1;
    temporarySpriteById.interval = 999999f;
    ++this.CurrentCommand;
  }

  private void parrotSplat(int extraInfo)
  {
    Game1.playSound("drumkit0");
    DelayedAction.playSoundAfterDelay("drumkit5", 100);
    Game1.playSound("slimeHit");
    foreach (TemporaryAnimatedSprite aboveMapSprite in this.aboveMapSprites)
      aboveMapSprite.alpha = 0.0f;
    Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(174, 168, 4, 11), 99999f, 1, 99999, new Vector2(1504f, 5568f), false, false, 0.02f, 0.01f, Color.White, 4f, 0.0f, 1.57079637f, (float) Math.PI / 64f)
    {
      motion = new Vector2(2f, -2f),
      acceleration = new Vector2(0.0f, 0.1f)
    });
    Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(174, 168, 4, 11), 99999f, 1, 99999, new Vector2(1504f, 5568f), false, false, 0.02f, 0.01f, Color.White, 4f, 0.0f, 0.7853982f, (float) Math.PI / 64f)
    {
      motion = new Vector2(-2f, -1f),
      acceleration = new Vector2(0.0f, 0.1f)
    });
    Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(174, 168, 4, 11), 99999f, 1, 99999, new Vector2(1504f, 5568f), false, false, 0.02f, 0.01f, Color.White, 4f, 0.0f, 3.14159274f, (float) Math.PI / 64f)
    {
      motion = new Vector2(1f, 1f),
      acceleration = new Vector2(0.0f, 0.1f)
    });
    Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(174, 168, 4, 11), 99999f, 1, 99999, new Vector2(1504f, 5568f), false, false, 0.02f, 0.01f, Color.White, 4f, 0.0f, 0.0f, (float) Math.PI / 64f)
    {
      motion = new Vector2(-2f, -2f),
      acceleration = new Vector2(0.0f, 0.1f)
    });
    Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(148, 165, 25, 23), 99999f, 1, 99999, new Vector2(1504f, 5568f), false, false, 0.01f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
    {
      id = 666
    });
    ++this.CurrentCommand;
  }

  public virtual Vector2 OffsetPosition(Vector2 original)
  {
    return new Vector2(this.OffsetPositionX(original.X), this.OffsetPositionY(original.Y));
  }

  public virtual Vector2 OffsetTile(Vector2 original)
  {
    return new Vector2((float) this.OffsetTileX((int) original.X), (float) this.OffsetTileY((int) original.Y));
  }

  public virtual float OffsetPositionX(float original)
  {
    return (double) original < 0.0 || this.ignoreTileOffsets ? original : original + this.eventPositionTileOffset.X * 64f;
  }

  public virtual float OffsetPositionY(float original)
  {
    return (double) original < 0.0 || this.ignoreTileOffsets ? original : original + this.eventPositionTileOffset.Y * 64f;
  }

  public virtual int OffsetTileX(int original)
  {
    return original < 0 || this.ignoreTileOffsets ? original : (int) ((double) original + (double) this.eventPositionTileOffset.X);
  }

  public virtual int OffsetTileY(int original)
  {
    return original < 0 || this.ignoreTileOffsets ? original : (int) ((double) original + (double) this.eventPositionTileOffset.Y);
  }

  private void addSpecificTemporarySprite(string key, GameLocation location, string[] args)
  {
    if (key == null)
      return;
    switch (key.Length)
    {
      case 3:
        if (!(key == "wed"))
          break;
        this.aboveMapSprites = new TemporaryAnimatedSpriteList();
        Game1.flashAlpha = 1f;
        for (int index = 0; index < 150; ++index)
        {
          Vector2 position = new Vector2((float) Game1.random.Next(Game1.viewport.Width - 128 /*0x80*/), (float) Game1.random.Next(Game1.viewport.Height));
          int scale = Game1.random.Next(2, 5);
          this.aboveMapSprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(424, 1266, 8, 8), 60f + (float) Game1.random.Next(-10, 10), 7, 999999, position, false, false, 0.99f, 0.0f, Color.White, (float) scale, 0.0f, 0.0f, 0.0f)
          {
            local = true,
            motion = new Vector2(0.1625f, -0.25f) * (float) scale
          });
        }
        Game1.changeMusicTrack("wedding", music_context: MusicContext.Event);
        Game1.musicPlayerVolume = 0.0f;
        break;
      case 5:
        switch (key[0])
        {
          case 'h':
            if (!(key == "heart"))
              return;
            Vector2 original;
            string error1;
            if (!ArgUtility.TryGetVector2(args, 2, out original, out error1, true, "Vector2 tile"))
            {
              this.LogCommandError(args, error1);
              return;
            }
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(211, 428, 7, 6), 2000f, 1, 0, this.OffsetPosition(original) * 64f + new Vector2(-16f, -16f), false, false, 1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
            {
              motion = new Vector2(0.0f, -0.5f),
              alphaFade = 0.01f
            });
            return;
          case 'r':
            if (!(key == "robot"))
              return;
            TemporaryAnimatedSprite temporaryAnimatedSprite1 = new TemporaryAnimatedSprite(this.getActorByName("robot").Sprite.textureName.Value, new Microsoft.Xna.Framework.Rectangle(35, 42, 35, 42), 50f, 1, 9999, new Vector2(13f, 27f) * 64f - new Vector2(0.0f, 32f), false, false, 0.98f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
            {
              acceleration = new Vector2(0.0f, -0.01f),
              accelerationChange = new Vector2(0.0f, -0.0001f)
            };
            location.temporarySprites.Add(temporaryAnimatedSprite1);
            for (int index = 0; index < 420; ++index)
              location.TemporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(Game1.random.Next(4) * 64 /*0x40*/, 320, 64 /*0x40*/, 64 /*0x40*/), new Vector2((float) Game1.random.Next(96 /*0x60*/), 136f), false, 0.01f, Color.White * 0.75f)
              {
                layerDepth = 1f,
                delayBeforeAnimationStart = index * 10,
                animationLength = 1,
                currentNumberOfLoops = 0,
                interval = 9999f,
                motion = new Vector2((float) (Game1.random.Next(-100, 100) / (index + 20)), (float) (0.25 + (double) index / 100.0)),
                parentSprite = temporaryAnimatedSprite1
              });
            return;
          case 's':
            if (!(key == "samTV"))
              return;
            Texture2D texture2D1 = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\temporary_sprites_1");
            location.TemporarySprites.Add(new TemporaryAnimatedSprite()
            {
              texture = texture2D1,
              sourceRect = new Microsoft.Xna.Framework.Rectangle(368, 350, 25, 29),
              animationLength = 1,
              sourceRectStartingPos = new Vector2(368f, 350f),
              interval = 5000f,
              totalNumberOfLoops = 99999,
              position = new Vector2(52f, 24f) * 64f + new Vector2(4f, -12f) * 4f,
              scale = 4f,
              layerDepth = 0.9f
            });
            return;
          default:
            return;
        }
      case 6:
        if (!(key == "qiCave"))
          break;
        Texture2D texture2D2 = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\temporary_sprites_1");
        location.TemporarySprites.Add(new TemporaryAnimatedSprite()
        {
          texture = texture2D2,
          sourceRect = new Microsoft.Xna.Framework.Rectangle(415, 216, 96 /*0x60*/, 89),
          animationLength = 1,
          sourceRectStartingPos = new Vector2(415f, 216f),
          interval = 999999f,
          totalNumberOfLoops = 99999,
          position = new Vector2(2f, 2f) * 64f + new Vector2(112f, 25f) * 4f,
          scale = 4f,
          layerDepth = 1E-07f
        });
        location.TemporarySprites.Add(new TemporaryAnimatedSprite()
        {
          texture = texture2D2,
          sourceRect = new Microsoft.Xna.Framework.Rectangle(370, 272, 107, 64 /*0x40*/),
          animationLength = 1,
          sourceRectStartingPos = new Vector2(370f, 216f),
          interval = 999999f,
          totalNumberOfLoops = 99999,
          position = new Vector2(2f, 2f) * 64f + new Vector2(67f, 81f) * 4f,
          scale = 4f,
          layerDepth = 1.1E-07f
        });
        location.TemporarySprites.Add(new TemporaryAnimatedSprite()
        {
          texture = Game1.objectSpriteSheet,
          sourceRect = Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, 803, 16 /*0x10*/, 16 /*0x10*/),
          sourceRectStartingPos = new Vector2((float) Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, 803, 16 /*0x10*/, 16 /*0x10*/).X, (float) Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, 803, 16 /*0x10*/, 16 /*0x10*/).Y),
          animationLength = 1,
          interval = 999999f,
          id = 803,
          totalNumberOfLoops = 99999,
          position = new Vector2(13f, 7f) * 64f + new Vector2(1f, 9f) * 4f,
          scale = 4f,
          layerDepth = 2.1E-06f
        });
        location.TemporarySprites.Add(new TemporaryAnimatedSprite()
        {
          texture = texture2D2,
          sourceRect = new Microsoft.Xna.Framework.Rectangle(432, 171, 16 /*0x10*/, 30),
          animationLength = 5,
          sourceRectStartingPos = new Vector2(432f, 171f),
          pingPong = true,
          interval = 100f,
          totalNumberOfLoops = 99999,
          id = 11,
          position = new Vector2(8f, 6f) * 64f,
          scale = 4f
        });
        location.TemporarySprites.Add(new TemporaryAnimatedSprite()
        {
          texture = texture2D2,
          sourceRect = new Microsoft.Xna.Framework.Rectangle(432, 171, 16 /*0x10*/, 30),
          animationLength = 5,
          sourceRectStartingPos = new Vector2(432f, 171f),
          pingPong = true,
          interval = 90f,
          totalNumberOfLoops = 99999,
          id = 11,
          position = new Vector2(5f, 7f) * 64f,
          scale = 4f
        });
        location.TemporarySprites.Add(new TemporaryAnimatedSprite()
        {
          texture = texture2D2,
          sourceRect = new Microsoft.Xna.Framework.Rectangle(432, 171, 16 /*0x10*/, 30),
          animationLength = 5,
          sourceRectStartingPos = new Vector2(432f, 171f),
          pingPong = true,
          interval = 120f,
          totalNumberOfLoops = 99999,
          id = 11,
          position = new Vector2(7f, 10f) * 64f,
          scale = 4f,
          layerDepth = 1f
        });
        location.TemporarySprites.Add(new TemporaryAnimatedSprite()
        {
          texture = texture2D2,
          sourceRect = new Microsoft.Xna.Framework.Rectangle(432, 171, 16 /*0x10*/, 30),
          animationLength = 5,
          sourceRectStartingPos = new Vector2(432f, 171f),
          pingPong = true,
          interval = 80f,
          totalNumberOfLoops = 99999,
          id = 11,
          position = new Vector2(15f, 7f) * 64f,
          scale = 4f
        });
        location.TemporarySprites.Add(new TemporaryAnimatedSprite()
        {
          texture = texture2D2,
          sourceRect = new Microsoft.Xna.Framework.Rectangle(432, 171, 16 /*0x10*/, 30),
          animationLength = 5,
          sourceRectStartingPos = new Vector2(432f, 171f),
          pingPong = true,
          interval = 100f,
          totalNumberOfLoops = 99999,
          id = 11,
          position = new Vector2(12f, 11f) * 64f,
          scale = 4f
        });
        location.TemporarySprites.Add(new TemporaryAnimatedSprite()
        {
          texture = texture2D2,
          sourceRect = new Microsoft.Xna.Framework.Rectangle(432, 171, 16 /*0x10*/, 30),
          animationLength = 5,
          sourceRectStartingPos = new Vector2(432f, 171f),
          pingPong = true,
          interval = 105f,
          totalNumberOfLoops = 99999,
          id = 11,
          position = new Vector2(16f, 10f) * 64f,
          scale = 4f
        });
        location.TemporarySprites.Add(new TemporaryAnimatedSprite()
        {
          texture = texture2D2,
          sourceRect = new Microsoft.Xna.Framework.Rectangle(432, 171, 16 /*0x10*/, 30),
          animationLength = 5,
          sourceRectStartingPos = new Vector2(432f, 171f),
          pingPong = true,
          interval = 85f,
          totalNumberOfLoops = 99999,
          id = 11,
          position = new Vector2(3f, 9f) * 64f,
          scale = 4f
        });
        break;
      case 7:
        switch (key[1])
        {
          case 'a':
            if (!(key == "jasGift"))
              return;
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(288, 1231, 16 /*0x10*/, 16 /*0x10*/), 100f, 6, 1, new Vector2(22f, 16f) * 64f, false, false, 0.01f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
            {
              id = 999,
              paused = true,
              holdLastFrame = true
            });
            return;
          case 'e':
            if (!(key == "wedding"))
              return;
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(540, 1196, 98, 54), 99999f, 1, 99999, new Vector2(25f, 60f) * 64f + new Vector2(0.0f, -64f), false, false, 1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f));
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(540, 1250, 98, 25), 99999f, 1, 99999, new Vector2(25f, 60f) * 64f + new Vector2(0.0f, 54f) * 4f + new Vector2(0.0f, -64f), false, false, 0.0f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f));
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(527, 1249, 12, 25), 99999f, 1, 99999, new Vector2(24f, 62f) * 64f, false, false, 0.0f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f));
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(527, 1249, 12, 25), 99999f, 1, 99999, new Vector2(32f, 62f) * 64f, false, false, 0.0f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f));
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(527, 1249, 12, 25), 99999f, 1, 99999, new Vector2(24f, 69f) * 64f, false, false, 1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f));
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(527, 1249, 12, 25), 99999f, 1, 99999, new Vector2(32f, 69f) * 64f, false, false, 1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f));
            return;
          case 'i':
            if (!(key == "dickBag"))
              return;
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(528, 1435, 16 /*0x10*/, 16 /*0x10*/), 99999f, 1, 99999, new Vector2(48f, 7f) * 64f, false, false, 1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f));
            return;
          case 'o':
            switch (key)
            {
              case "JoshMom":
                TemporaryAnimatedSprite temporaryAnimatedSprite2 = new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(416, 1931, 58, 65), 750f, 2, 99999, new Vector2((float) (Game1.viewport.Width / 2), (float) Game1.viewport.Height), false, false, 1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
                {
                  alpha = 0.6f,
                  local = true,
                  xPeriodic = true,
                  xPeriodicLoopTime = 2000f,
                  xPeriodicRange = 32f,
                  motion = new Vector2(0.0f, -1.25f),
                  initialPosition = new Vector2((float) (Game1.viewport.Width / 2), (float) Game1.viewport.Height)
                };
                location.temporarySprites.Add(temporaryAnimatedSprite2);
                for (int index = 0; index < 19; ++index)
                  location.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(516, 1916, 7, 10), 99999f, 1, 99999, new Vector2(64f, 32f), false, false, 1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
                  {
                    alphaFade = 0.01f,
                    local = true,
                    motion = new Vector2(-1f, -1f),
                    parentSprite = temporaryAnimatedSprite2,
                    delayBeforeAnimationStart = (index + 1) * 1000
                  });
                return;
              case "joshDog":
                location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(324, 1916, 12, 20), 500f, 6, 9999, new Vector2(53f, 67f) * 64f + new Vector2(3f, 3f) * 4f, false, false, 1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
                {
                  id = 1
                });
                return;
              default:
                return;
            }
          case 'r':
            if (!(key == "dropEgg"))
              return;
            location.TemporarySprites.Add(new TemporaryAnimatedSprite(176 /*0xB0*/, 800f, 1, 0, new Vector2(6f, 4f) * 64f + new Vector2(0.0f, 32f), false, false)
            {
              rotationChange = 0.1308997f,
              motion = new Vector2(0.0f, -7f),
              acceleration = new Vector2(0.0f, 0.3f),
              endFunction = new TemporaryAnimatedSprite.endBehavior(this.eggSmashEndFunction),
              layerDepth = 1f
            });
            return;
          case 'u':
            if (!(key == "sunroom"))
              return;
            location.temporarySprites.Add(new TemporaryAnimatedSprite()
            {
              texture = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\temporary_sprites_1"),
              sourceRect = new Microsoft.Xna.Framework.Rectangle(304, 486, 24, 26),
              sourceRectStartingPos = new Vector2(304f, 486f),
              animationLength = 1,
              totalNumberOfLoops = 997,
              interval = 99999f,
              scale = 4f,
              position = new Vector2(4f, 8f) * 64f + new Vector2(8f, -8f) * 4f,
              layerDepth = 0.0512f,
              id = 996
            });
            location.addCritter((Critter) new Butterfly(location, location.getRandomTile()).setStayInbounds(true));
            while (Game1.random.NextBool())
              location.addCritter((Critter) new Butterfly(location, location.getRandomTile()).setStayInbounds(true));
            return;
          default:
            return;
        }
      case 8:
        switch (key[4])
        {
          case 'J':
            if (!(key == "frogJump"))
              return;
            TemporaryAnimatedSprite temporarySpriteById1 = location.getTemporarySpriteByID(777);
            temporarySpriteById1.motion = new Vector2(-2f, 0.0f);
            temporarySpriteById1.animationLength = 4;
            temporarySpriteById1.interval = 150f;
            return;
          case 'S':
            if (!(key == "leahShow"))
              return;
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(144 /*0x90*/, 688, 16 /*0x10*/, 32 /*0x20*/), 9999f, 1, 999, new Vector2(29f, 59f) * 64f - new Vector2(0.0f, 16f), false, false, 0.377500027f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f));
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(112 /*0x70*/, 656, 16 /*0x10*/, 64 /*0x40*/), 9999f, 1, 999, new Vector2(29f, 56f) * 64f, false, false, 0.3776f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f));
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(144 /*0x90*/, 688, 16 /*0x10*/, 32 /*0x20*/), 9999f, 1, 999, new Vector2(33f, 59f) * 64f - new Vector2(0.0f, 16f), false, false, 0.377500027f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f));
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(128 /*0x80*/, 688, 16 /*0x10*/, 32 /*0x20*/), 9999f, 1, 999, new Vector2(33f, 58f) * 64f, false, false, 0.3776f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f));
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(160 /*0xA0*/, 656, 32 /*0x20*/, 64 /*0x40*/), 9999f, 1, 999, new Vector2(29f, 60f) * 64f, false, false, 0.4032f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f));
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(144 /*0x90*/, 688, 16 /*0x10*/, 32 /*0x20*/), 9999f, 1, 999, new Vector2(34f, 63f) * 64f, false, false, 0.4031f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f));
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(113, 592, 16 /*0x10*/, 64 /*0x40*/), 100f, 4, 99999, new Vector2(34f, 60f) * 64f, false, false, 0.4032f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f));
            this.actors.Add(new NPC(new AnimatedSprite((ContentManager) this.festivalContent, "Characters\\" + (this.farmer.IsMale ? "LeahExMale" : "LeahExFemale"), 0, 16 /*0x10*/, 32 /*0x20*/), new Vector2(46f, 57f) * 64f, 2, "LeahEx")
            {
              AllowDynamicAppearance = false
            });
            return;
          case 'T':
            if (!(key == "leahTree"))
              return;
            location.TemporarySprites.Add(new TemporaryAnimatedSprite(744, 999999f, 1, 0, new Vector2(42f, 8f) * 64f, false, false));
            return;
          case 'e':
            if (!(key == "umbrella"))
              return;
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(324, 1843, 27, 23), 80f, 3, 9999, new Vector2(12f, 39f) * 64f + new Vector2(-20f, -104f), false, false, 1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f));
            return;
          case 'm':
            if (!(key == "golemDie"))
              return;
            location.temporarySprites.Add(new TemporaryAnimatedSprite(46, new Vector2(40f, 11f) * 64f, Color.DarkGray, 10));
            Utility.makeTemporarySpriteJuicier(new TemporaryAnimatedSprite(44, new Vector2(40f, 11f) * 64f, Color.LimeGreen, 10), location, 2);
            Texture2D texture2D3 = Game1.temporaryContent.Load<Texture2D>("Characters\\Monsters\\Wilderness Golem");
            location.TemporarySprites.Add(new TemporaryAnimatedSprite()
            {
              texture = texture2D3,
              sourceRect = new Microsoft.Xna.Framework.Rectangle(0, 0, 16 /*0x10*/, 24),
              animationLength = 1,
              sourceRectStartingPos = new Vector2(0.0f, 0.0f),
              interval = 5000f,
              totalNumberOfLoops = 9999,
              position = new Vector2(40f, 11f) * 64f + new Vector2(2f, -8f) * 4f,
              scale = 4f,
              layerDepth = 0.01f,
              rotation = 1.57079637f,
              motion = new Vector2(0.0f, 4f),
              yStopCoordinate = 832
            });
            return;
          case 'o':
            if (!(key == "parrots1"))
              return;
            TemporaryAnimatedSpriteList animatedSpriteList1 = new TemporaryAnimatedSpriteList();
            Microsoft.Xna.Framework.Rectangle sourceRect1 = new Microsoft.Xna.Framework.Rectangle(0, 165, 24, 22);
            Viewport viewport1 = Game1.graphics.GraphicsDevice.Viewport;
            Vector2 position1 = new Vector2((float) viewport1.Width, 256f);
            Color white1 = Color.White;
            animatedSpriteList1.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", sourceRect1, 100f, 6, 9999, position1, false, false, 1f, 0.0f, white1, 4f, 0.0f, 0.0f, 0.0f)
            {
              motion = new Vector2(-3f, 0.0f),
              yPeriodic = true,
              yPeriodicLoopTime = 2000f,
              yPeriodicRange = 32f,
              delayBeforeAnimationStart = 0,
              local = true
            });
            Microsoft.Xna.Framework.Rectangle sourceRect2 = new Microsoft.Xna.Framework.Rectangle(0, 165, 24, 22);
            viewport1 = Game1.graphics.GraphicsDevice.Viewport;
            Vector2 position2 = new Vector2((float) viewport1.Width, 192f);
            Color white2 = Color.White;
            animatedSpriteList1.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", sourceRect2, 100f, 6, 9999, position2, false, false, 1f, 0.0f, white2, 4f, 0.0f, 0.0f, 0.0f)
            {
              motion = new Vector2(-3f, 0.0f),
              yPeriodic = true,
              yPeriodicLoopTime = 2000f,
              yPeriodicRange = 32f,
              delayBeforeAnimationStart = 600,
              local = true
            });
            Microsoft.Xna.Framework.Rectangle sourceRect3 = new Microsoft.Xna.Framework.Rectangle(0, 165, 24, 22);
            viewport1 = Game1.graphics.GraphicsDevice.Viewport;
            Vector2 position3 = new Vector2((float) viewport1.Width, 320f);
            Color white3 = Color.White;
            animatedSpriteList1.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", sourceRect3, 100f, 6, 9999, position3, false, false, 1f, 0.0f, white3, 4f, 0.0f, 0.0f, 0.0f)
            {
              motion = new Vector2(-3f, 0.0f),
              yPeriodic = true,
              yPeriodicLoopTime = 2000f,
              yPeriodicRange = 32f,
              delayBeforeAnimationStart = 1200,
              local = true
            });
            this.aboveMapSprites = animatedSpriteList1;
            return;
          case 'y':
            if (!(key == "WillyWad"))
              return;
            location.temporarySprites.Add(new TemporaryAnimatedSprite()
            {
              texture = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\Cursors2"),
              sourceRect = new Microsoft.Xna.Framework.Rectangle(192 /*0xC0*/, 61, 32 /*0x20*/, 32 /*0x20*/),
              sourceRectStartingPos = new Vector2(192f, 61f),
              animationLength = 2,
              totalNumberOfLoops = 99999,
              interval = 400f,
              scale = 4f,
              position = new Vector2(50f, 23f) * 64f,
              layerDepth = 0.1536f,
              id = 996
            });
            location.TemporarySprites.Add(new TemporaryAnimatedSprite(51, new Vector2(3328f, 1728f), Color.White, 10, animationInterval: 80f, numberOfLoops: 999999));
            location.TemporarySprites.Add(new TemporaryAnimatedSprite(51, new Vector2(3264f, 1792f), Color.White, 10, animationInterval: 70f, numberOfLoops: 999999));
            location.TemporarySprites.Add(new TemporaryAnimatedSprite(51, new Vector2(3392f, 1792f), Color.White, 10, animationInterval: 85f, numberOfLoops: 999999));
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(160 /*0xA0*/, 368, 16 /*0x10*/, 32 /*0x20*/), 500f, 3, 99999, new Vector2(53f, 24f) * 64f, false, false, 0.1984f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f));
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(160 /*0xA0*/, 368, 16 /*0x10*/, 32 /*0x20*/), 510f, 3, 99999, new Vector2(54f, 23f) * 64f, false, false, 0.1984f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f));
            return;
          default:
            return;
        }
      case 9:
        switch (key[5])
        {
          case 'B':
            switch (key)
            {
              case "shakeBush":
                location.getTemporarySpriteByID(777).shakeIntensity = 1f;
                return;
              case "movieBush":
                location.temporarySprites.Add(new TemporaryAnimatedSprite()
                {
                  texture = Game1.temporaryContent.Load<Texture2D>("TileSheets\\bushes"),
                  sourceRect = new Microsoft.Xna.Framework.Rectangle(65, 58, 30, 35),
                  sourceRectStartingPos = new Vector2(65f, 58f),
                  animationLength = 1,
                  totalNumberOfLoops = 999,
                  interval = 999f,
                  scale = 4f,
                  position = new Vector2(4f, 1f) * 64f + new Vector2(33f, 13f) * 4f,
                  layerDepth = 0.99f,
                  id = 777
                });
                return;
              default:
                return;
            }
          case 'C':
            if (!(key == "pennyCook"))
              return;
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(256 /*0x0100*/, 1856, 64 /*0x40*/, 128 /*0x80*/), new Vector2(10f, 6f) * 64f, false, 0.0f, Color.White)
            {
              layerDepth = 1f,
              animationLength = 6,
              interval = 75f,
              motion = new Vector2(0.0f, -0.5f)
            });
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(256 /*0x0100*/, 1856, 64 /*0x40*/, 128 /*0x80*/), new Vector2(10f, 6f) * 64f + new Vector2(16f, 0.0f), false, 0.0f, Color.White)
            {
              layerDepth = 0.1f,
              animationLength = 6,
              interval = 75f,
              motion = new Vector2(0.0f, -0.5f),
              delayBeforeAnimationStart = 500
            });
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(256 /*0x0100*/, 1856, 64 /*0x40*/, 128 /*0x80*/), new Vector2(10f, 6f) * 64f + new Vector2(-16f, 0.0f), false, 0.0f, Color.White)
            {
              layerDepth = 1f,
              animationLength = 6,
              interval = 75f,
              motion = new Vector2(0.0f, -0.5f),
              delayBeforeAnimationStart = 750
            });
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(256 /*0x0100*/, 1856, 64 /*0x40*/, 128 /*0x80*/), new Vector2(10f, 6f) * 64f, false, 0.0f, Color.White)
            {
              layerDepth = 0.1f,
              animationLength = 6,
              interval = 75f,
              motion = new Vector2(0.0f, -0.5f),
              delayBeforeAnimationStart = 1000
            });
            return;
          case 'D':
            return;
          case 'E':
            return;
          case 'F':
            if (!(key == "sauceFire"))
              return;
            location.TemporarySprites.Add(new TemporaryAnimatedSprite()
            {
              texture = Game1.mouseCursors,
              sourceRect = new Microsoft.Xna.Framework.Rectangle(276, 1985, 12, 11),
              animationLength = 4,
              sourceRectStartingPos = new Vector2(276f, 1985f),
              interval = 100f,
              totalNumberOfLoops = 5,
              position = this.OffsetPosition(new Vector2(64f, 16f) * 64f + new Vector2(3f, -4f) * 4f),
              scale = 4f,
              layerDepth = 1f
            });
            this.aboveMapSprites = new TemporaryAnimatedSpriteList();
            for (int index = 0; index < 8; ++index)
              this.aboveMapSprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(372, 1956, 10, 10), this.OffsetPosition(new Vector2(64f, 16f) * 64f) + new Vector2((float) Game1.random.Next(-16, 32 /*0x20*/), 0.0f), false, 1f / 500f, Color.Gray)
              {
                alpha = 0.75f,
                motion = new Vector2(1f, -1f) + new Vector2((float) (Game1.random.Next(100) - 50) / 100f, (float) (Game1.random.Next(100) - 50) / 100f),
                interval = 99999f,
                layerDepth = (float) (0.03840000182390213 + (double) Game1.random.Next(100) / 10000.0),
                scale = 3f,
                scaleChange = 0.01f,
                rotationChange = (float) ((double) Game1.random.Next(-5, 6) * 3.1415927410125732 / 256.0),
                delayBeforeAnimationStart = index * 25
              });
            return;
          case 'G':
            if (!(key == "sauceGood"))
              return;
            Utility.addSprinklesToLocation(location, this.OffsetTileX(64 /*0x40*/), this.OffsetTileY(16 /*0x10*/), 3, 1, 800, 200, Color.White);
            return;
          case 'M':
            if (!(key == "pennyMess"))
              return;
            location.TemporarySprites.Add(new TemporaryAnimatedSprite(739, 999999f, 1, 0, new Vector2(10f, 5f) * 64f, false, false));
            location.TemporarySprites.Add(new TemporaryAnimatedSprite(740, 999999f, 1, 0, new Vector2(15f, 5f) * 64f, false, false));
            location.TemporarySprites.Add(new TemporaryAnimatedSprite(741, 999999f, 1, 0, new Vector2(16f, 6f) * 64f, false, false));
            return;
          case 'S':
            if (!(key == "EmilySign"))
              return;
            this.aboveMapSprites = new TemporaryAnimatedSpriteList();
            for (int index = 0; index < 10; ++index)
            {
              int num = 0;
              Random random = Game1.random;
              Viewport viewport2 = Game1.graphics.GraphicsDevice.Viewport;
              int maxValue = viewport2.Height - 128 /*0x80*/;
              int y = random.Next(maxValue);
              viewport2 = Game1.graphics.GraphicsDevice.Viewport;
              for (int width = viewport2.Width; width >= -64; width -= 48 /*0x30*/)
              {
                this.aboveMapSprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(597, 1888, 16 /*0x10*/, 16 /*0x10*/), 99999f, 1, 99999, new Vector2((float) width, (float) y), false, false, 1f, 0.02f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
                {
                  delayBeforeAnimationStart = index * 600 + num * 25,
                  startSound = num == 0 ? "dwoop" : (string) null,
                  local = true
                });
                ++num;
              }
            }
            return;
          case 'T':
            if (!(key == "shakeTent"))
              return;
            location.getTemporarySpriteByID(999).shakeIntensity = 1f;
            return;
          case 'a':
            if (!(key == "samSkate1"))
              return;
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(0, 0, 0, 0), 9999f, 1, 999, new Vector2(12f, 90f) * 64f, false, false, 1E-05f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
            {
              motion = new Vector2(4f, 0.0f),
              acceleration = new Vector2(-0.008f, 0.0f),
              xStopCoordinate = 1344,
              reachedStopCoordinate = new TemporaryAnimatedSprite.endBehavior(this.samPreOllie),
              attachedCharacter = (Character) this.getActorByName("Sam"),
              id = 92473
            });
            return;
          case 't':
            if (!(key == "joshSteak"))
              return;
            location.temporarySprites.Clear();
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(324, 1936, 12, 20), 80f, 4, 99999, new Vector2(53f, 67f) * 64f + new Vector2(3f, 3f) * 4f, false, false, 1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
            {
              id = 1
            });
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(497, 1918, 11, 11), 999f, 1, 9999, new Vector2(50f, 68f) * 64f + new Vector2(32f, -8f), false, false, 1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f));
            return;
          case 'u':
            if (!(key == "abbyOuija"))
              return;
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(0, 960, 128 /*0x80*/, 128 /*0x80*/), 60f, 4, 0, new Vector2(6f, 9f) * 64f, false, false, 1f, 0.0f, Color.White, 1f, 0.0f, 0.0f, 0.0f));
            return;
          default:
            return;
        }
      case 10:
        switch (key[6])
        {
          case 'B':
            switch (key)
            {
              case "arcaneBook":
                for (int index = 0; index < 16 /*0x10*/; ++index)
                  location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(536, 1945, 8, 8), new Vector2(128f, 792f) + new Vector2((float) Game1.random.Next(32 /*0x20*/), (float) (Game1.random.Next(32 /*0x20*/) - index * 4)), false, 0.0f, Color.White)
                  {
                    interval = 50f,
                    totalNumberOfLoops = 99999,
                    animationLength = 7,
                    layerDepth = 1f,
                    scale = 4f,
                    alphaFade = 0.008f,
                    motion = new Vector2(0.0f, -0.5f)
                  });
                this.aboveMapSprites = new TemporaryAnimatedSpriteList()
                {
                  new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(325, 1977, 18, 18), new Vector2(160f, 800f), false, 0.0f, Color.White)
                  {
                    interval = 25f,
                    totalNumberOfLoops = 99999,
                    animationLength = 3,
                    layerDepth = 1f,
                    scale = 1f,
                    scaleChange = 1f,
                    scaleChangeChange = -0.05f,
                    alpha = 0.65f,
                    alphaFade = 0.005f,
                    motion = new Vector2(-8f, -8f),
                    acceleration = new Vector2(0.4f, 0.4f)
                  }
                };
                for (int index = 0; index < 16 /*0x10*/; ++index)
                  this.aboveMapSprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(372, 1956, 10, 10), new Vector2(2f, 12f) * 64f + new Vector2((float) Game1.random.Next(-32, 64 /*0x40*/), 0.0f), false, 1f / 500f, Color.Gray)
                  {
                    alpha = 0.75f,
                    motion = new Vector2(1f, -1f) + new Vector2((float) (Game1.random.Next(100) - 50) / 100f, (float) (Game1.random.Next(100) - 50) / 100f),
                    interval = 99999f,
                    layerDepth = (float) (0.03840000182390213 + (double) Game1.random.Next(100) / 10000.0),
                    scale = 3f,
                    scaleChange = 0.01f,
                    rotationChange = (float) ((double) Game1.random.Next(-5, 6) * 3.1415927410125732 / 256.0),
                    delayBeforeAnimationStart = index * 25
                  });
                location.setMapTile(2, 12, 2143, "Front", "untitled tile sheet");
                return;
              case "candleBoat":
                this.showGroundObjects = false;
                location.TemporarySprites.Add(new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(240 /*0xF0*/, 112 /*0x70*/, 16 /*0x10*/, 32 /*0x20*/), 1000f, 2, 99999, new Vector2(22f, 36f) * 64f, false, false, 1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
                {
                  id = 1,
                  lightId = this.GenerateLightSourceId("candleBoat"),
                  lightRadius = 2f,
                  lightcolor = Color.Black
                });
                return;
              default:
                return;
            }
          case 'C':
            if (!(key == "junimoCage"))
              return;
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(325, 1977, 18, 19), 60f, 3, 999999, new Vector2(10f, 17f) * 64f + new Vector2(0.0f, -4f), false, false, 0.0f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
            {
              lightId = this.GenerateLightSourceId("junimoCage_1"),
              lightRadius = 1f,
              lightcolor = Color.Black,
              id = 1,
              shakeIntensity = 0.0f
            });
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(379, 1991, 5, 5), 9999f, 1, 999999, new Vector2(10f, 17f) * 64f + new Vector2(0.0f, -4f), false, false, 1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
            {
              lightId = this.GenerateLightSourceId("junimoCage_2"),
              lightRadius = 0.5f,
              lightcolor = Color.Black,
              id = 1,
              xPeriodic = true,
              xPeriodicLoopTime = 2000f,
              xPeriodicRange = 24f,
              yPeriodic = true,
              yPeriodicLoopTime = 2000f,
              yPeriodicRange = 24f
            });
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(379, 1991, 5, 5), 9999f, 1, 999999, new Vector2(10f, 17f) * 64f + new Vector2(72f, -4f), false, false, 1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
            {
              lightId = this.GenerateLightSourceId("junimoCage_3"),
              lightRadius = 0.5f,
              lightcolor = Color.Black,
              id = 1,
              xPeriodic = true,
              xPeriodicLoopTime = 2000f,
              xPeriodicRange = -24f,
              yPeriodic = true,
              yPeriodicLoopTime = 2000f,
              yPeriodicRange = 24f,
              delayBeforeAnimationStart = 250
            });
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(379, 1991, 5, 5), 9999f, 1, 999999, new Vector2(10f, 17f) * 64f + new Vector2(0.0f, 52f), false, false, 1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
            {
              lightId = this.GenerateLightSourceId("junimoCage_3"),
              lightRadius = 0.5f,
              lightcolor = Color.Black,
              id = 1,
              xPeriodic = true,
              xPeriodicLoopTime = 2000f,
              xPeriodicRange = -24f,
              yPeriodic = true,
              yPeriodicLoopTime = 2000f,
              yPeriodicRange = 24f,
              delayBeforeAnimationStart = 450
            });
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(379, 1991, 5, 5), 9999f, 1, 999999, new Vector2(10f, 17f) * 64f + new Vector2(72f, 52f), false, false, 1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
            {
              lightId = this.GenerateLightSourceId("junimoCage_4"),
              lightRadius = 0.5f,
              lightcolor = Color.Black,
              id = 1,
              xPeriodic = true,
              xPeriodicLoopTime = 2000f,
              xPeriodicRange = 24f,
              yPeriodic = true,
              yPeriodicLoopTime = 2000f,
              yPeriodicRange = 24f,
              delayBeforeAnimationStart = 650
            });
            return;
          case 'G':
            switch (key)
            {
              case "parrotGone":
                location.removeTemporarySpritesWithID(666);
                return;
              case "secretGift":
                location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(288, 1231, 16 /*0x10*/, 16 /*0x10*/), new Vector2(30f, 70f) * 64f + new Vector2(0.0f, -21f), false, 0.0f, Color.White)
                {
                  animationLength = 1,
                  interval = 999999f,
                  id = 666,
                  scale = 4f
                });
                return;
              default:
                return;
            }
          case 'L':
            if (!(key == "abbyAtLake"))
              return;
            int num1 = 1;
            TemporaryAnimatedSpriteList temporarySprites1 = location.TemporarySprites;
            TemporaryAnimatedSprite temporaryAnimatedSprite3 = new TemporaryAnimatedSprite(735, 999999f, 1, 0, new Vector2(48f, 30f) * 64f, false, false);
            DefaultInterpolatedStringHandler interpolatedStringHandler1 = new DefaultInterpolatedStringHandler(11, 1);
            interpolatedStringHandler1.AppendLiteral("abbyAtLake_");
            ref DefaultInterpolatedStringHandler local1 = ref interpolatedStringHandler1;
            int num2 = num1;
            int num3 = num2 + 1;
            local1.AppendFormatted<int>(num2);
            temporaryAnimatedSprite3.lightId = this.GenerateLightSourceId(interpolatedStringHandler1.ToStringAndClear());
            temporaryAnimatedSprite3.lightRadius = 2f;
            temporarySprites1.Add(temporaryAnimatedSprite3);
            TemporaryAnimatedSpriteList temporarySprites2 = location.TemporarySprites;
            TemporaryAnimatedSprite temporaryAnimatedSprite4 = new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(232, 328, 4, 4), 9999999f, 1, 0, new Vector2(48f, 30f) * 64f + new Vector2(32f, 0.0f), false, false, 1f, 0.0f, Color.White, 1f, 0.0f, 0.0f, 0.0f);
            interpolatedStringHandler1 = new DefaultInterpolatedStringHandler(11, 1);
            interpolatedStringHandler1.AppendLiteral("abbyAtLake_");
            ref DefaultInterpolatedStringHandler local2 = ref interpolatedStringHandler1;
            int num4 = num3;
            int num5 = num4 + 1;
            local2.AppendFormatted<int>(num4);
            temporaryAnimatedSprite4.lightId = this.GenerateLightSourceId(interpolatedStringHandler1.ToStringAndClear());
            temporaryAnimatedSprite4.lightRadius = 0.2f;
            temporaryAnimatedSprite4.xPeriodic = true;
            temporaryAnimatedSprite4.yPeriodic = true;
            temporaryAnimatedSprite4.xPeriodicLoopTime = 2000f;
            temporaryAnimatedSprite4.yPeriodicLoopTime = 1600f;
            temporaryAnimatedSprite4.xPeriodicRange = 32f;
            temporaryAnimatedSprite4.yPeriodicRange = 21f;
            temporarySprites2.Add(temporaryAnimatedSprite4);
            TemporaryAnimatedSpriteList temporarySprites3 = location.TemporarySprites;
            TemporaryAnimatedSprite temporaryAnimatedSprite5 = new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(232, 328, 4, 4), 9999999f, 1, 0, new Vector2(48f, 30f) * 64f + new Vector2(32f, 0.0f), false, false, 1f, 0.0f, Color.White, 1f, 0.0f, 0.0f, 0.0f);
            interpolatedStringHandler1 = new DefaultInterpolatedStringHandler(11, 1);
            interpolatedStringHandler1.AppendLiteral("abbyAtLake_");
            ref DefaultInterpolatedStringHandler local3 = ref interpolatedStringHandler1;
            int num6 = num5;
            int num7 = num6 + 1;
            local3.AppendFormatted<int>(num6);
            temporaryAnimatedSprite5.lightId = this.GenerateLightSourceId(interpolatedStringHandler1.ToStringAndClear());
            temporaryAnimatedSprite5.lightRadius = 0.2f;
            temporaryAnimatedSprite5.xPeriodic = true;
            temporaryAnimatedSprite5.yPeriodic = true;
            temporaryAnimatedSprite5.xPeriodicLoopTime = 1000f;
            temporaryAnimatedSprite5.yPeriodicLoopTime = 1600f;
            temporaryAnimatedSprite5.xPeriodicRange = 16f;
            temporaryAnimatedSprite5.yPeriodicRange = 21f;
            temporarySprites3.Add(temporaryAnimatedSprite5);
            TemporaryAnimatedSpriteList temporarySprites4 = location.TemporarySprites;
            TemporaryAnimatedSprite temporaryAnimatedSprite6 = new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(232, 328, 4, 4), 9999999f, 1, 0, new Vector2(48f, 30f) * 64f + new Vector2(32f, 0.0f), false, false, 1f, 0.0f, Color.White, 1f, 0.0f, 0.0f, 0.0f);
            interpolatedStringHandler1 = new DefaultInterpolatedStringHandler(11, 1);
            interpolatedStringHandler1.AppendLiteral("abbyAtLake_");
            ref DefaultInterpolatedStringHandler local4 = ref interpolatedStringHandler1;
            int num8 = num7;
            int num9 = num8 + 1;
            local4.AppendFormatted<int>(num8);
            temporaryAnimatedSprite6.lightId = this.GenerateLightSourceId(interpolatedStringHandler1.ToStringAndClear());
            temporaryAnimatedSprite6.lightRadius = 0.2f;
            temporaryAnimatedSprite6.xPeriodic = true;
            temporaryAnimatedSprite6.yPeriodic = true;
            temporaryAnimatedSprite6.xPeriodicLoopTime = 2400f;
            temporaryAnimatedSprite6.yPeriodicLoopTime = 2800f;
            temporaryAnimatedSprite6.xPeriodicRange = 21f;
            temporaryAnimatedSprite6.yPeriodicRange = 32f;
            temporarySprites4.Add(temporaryAnimatedSprite6);
            TemporaryAnimatedSpriteList temporarySprites5 = location.TemporarySprites;
            TemporaryAnimatedSprite temporaryAnimatedSprite7 = new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(232, 328, 4, 4), 9999999f, 1, 0, new Vector2(48f, 30f) * 64f + new Vector2(32f, 0.0f), false, false, 1f, 0.0f, Color.White, 1f, 0.0f, 0.0f, 0.0f);
            interpolatedStringHandler1 = new DefaultInterpolatedStringHandler(11, 1);
            interpolatedStringHandler1.AppendLiteral("abbyAtLake_");
            ref DefaultInterpolatedStringHandler local5 = ref interpolatedStringHandler1;
            int num10 = num9;
            int num11 = num10 + 1;
            local5.AppendFormatted<int>(num10);
            temporaryAnimatedSprite7.lightId = this.GenerateLightSourceId(interpolatedStringHandler1.ToStringAndClear());
            temporaryAnimatedSprite7.lightRadius = 0.2f;
            temporaryAnimatedSprite7.xPeriodic = true;
            temporaryAnimatedSprite7.yPeriodic = true;
            temporaryAnimatedSprite7.xPeriodicLoopTime = 2000f;
            temporaryAnimatedSprite7.yPeriodicLoopTime = 2400f;
            temporaryAnimatedSprite7.xPeriodicRange = 16f;
            temporaryAnimatedSprite7.yPeriodicRange = 16f;
            temporarySprites5.Add(temporaryAnimatedSprite7);
            TemporaryAnimatedSpriteList temporarySprites6 = location.TemporarySprites;
            TemporaryAnimatedSprite temporaryAnimatedSprite8 = new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(232, 328, 4, 4), 9999999f, 1, 0, new Vector2(66f, 34f) * 64f + new Vector2(-32f, 0.0f), false, false, 1f, 0.0f, Color.White, 1f, 0.0f, 0.0f, 0.0f);
            temporaryAnimatedSprite8.lightcolor = Color.Orange;
            interpolatedStringHandler1 = new DefaultInterpolatedStringHandler(11, 1);
            interpolatedStringHandler1.AppendLiteral("abbyAtLake_");
            ref DefaultInterpolatedStringHandler local6 = ref interpolatedStringHandler1;
            int num12 = num11;
            int num13 = num12 + 1;
            local6.AppendFormatted<int>(num12);
            temporaryAnimatedSprite8.lightId = this.GenerateLightSourceId(interpolatedStringHandler1.ToStringAndClear());
            temporaryAnimatedSprite8.lightRadius = 0.2f;
            temporaryAnimatedSprite8.xPeriodic = true;
            temporaryAnimatedSprite8.yPeriodic = true;
            temporaryAnimatedSprite8.xPeriodicLoopTime = 2000f;
            temporaryAnimatedSprite8.yPeriodicLoopTime = 2600f;
            temporaryAnimatedSprite8.xPeriodicRange = 21f;
            temporaryAnimatedSprite8.yPeriodicRange = 48f;
            temporarySprites6.Add(temporaryAnimatedSprite8);
            TemporaryAnimatedSpriteList temporarySprites7 = location.TemporarySprites;
            TemporaryAnimatedSprite temporaryAnimatedSprite9 = new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(232, 328, 4, 4), 9999999f, 1, 0, new Vector2(66f, 34f) * 64f + new Vector2(32f, 0.0f), false, false, 1f, 0.0f, Color.White, 1f, 0.0f, 0.0f, 0.0f);
            temporaryAnimatedSprite9.lightcolor = Color.Orange;
            interpolatedStringHandler1 = new DefaultInterpolatedStringHandler(11, 1);
            interpolatedStringHandler1.AppendLiteral("abbyAtLake_");
            ref DefaultInterpolatedStringHandler local7 = ref interpolatedStringHandler1;
            int num14 = num13;
            int num15 = num14 + 1;
            local7.AppendFormatted<int>(num14);
            temporaryAnimatedSprite9.lightId = this.GenerateLightSourceId(interpolatedStringHandler1.ToStringAndClear());
            temporaryAnimatedSprite9.lightRadius = 0.2f;
            temporaryAnimatedSprite9.xPeriodic = true;
            temporaryAnimatedSprite9.yPeriodic = true;
            temporaryAnimatedSprite9.xPeriodicLoopTime = 2000f;
            temporaryAnimatedSprite9.yPeriodicLoopTime = 2600f;
            temporaryAnimatedSprite9.xPeriodicRange = 32f;
            temporaryAnimatedSprite9.yPeriodicRange = 21f;
            temporarySprites7.Add(temporaryAnimatedSprite9);
            TemporaryAnimatedSpriteList temporarySprites8 = location.TemporarySprites;
            TemporaryAnimatedSprite temporaryAnimatedSprite10 = new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(232, 328, 4, 4), 9999999f, 1, 0, new Vector2(66f, 34f) * 64f + new Vector2(32f, 32f), false, false, 1f, 0.0f, Color.White, 1f, 0.0f, 0.0f, 0.0f);
            temporaryAnimatedSprite10.lightcolor = Color.Orange;
            interpolatedStringHandler1 = new DefaultInterpolatedStringHandler(11, 1);
            interpolatedStringHandler1.AppendLiteral("abbyAtLake_");
            ref DefaultInterpolatedStringHandler local8 = ref interpolatedStringHandler1;
            int num16 = num15;
            int num17 = num16 + 1;
            local8.AppendFormatted<int>(num16);
            temporaryAnimatedSprite10.lightId = this.GenerateLightSourceId(interpolatedStringHandler1.ToStringAndClear());
            temporaryAnimatedSprite10.lightRadius = 0.2f;
            temporaryAnimatedSprite10.xPeriodic = true;
            temporaryAnimatedSprite10.yPeriodic = true;
            temporaryAnimatedSprite10.xPeriodicLoopTime = 4000f;
            temporaryAnimatedSprite10.yPeriodicLoopTime = 5000f;
            temporaryAnimatedSprite10.xPeriodicRange = 42f;
            temporaryAnimatedSprite10.yPeriodicRange = 32f;
            temporarySprites8.Add(temporaryAnimatedSprite10);
            TemporaryAnimatedSpriteList temporarySprites9 = location.TemporarySprites;
            TemporaryAnimatedSprite temporaryAnimatedSprite11 = new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(232, 328, 4, 4), 9999999f, 1, 0, new Vector2(66f, 34f) * 64f + new Vector2(0.0f, -32f), false, false, 1f, 0.0f, Color.White, 1f, 0.0f, 0.0f, 0.0f);
            temporaryAnimatedSprite11.lightcolor = Color.Orange;
            interpolatedStringHandler1 = new DefaultInterpolatedStringHandler(11, 1);
            interpolatedStringHandler1.AppendLiteral("abbyAtLake_");
            ref DefaultInterpolatedStringHandler local9 = ref interpolatedStringHandler1;
            int num18 = num17;
            int num19 = num18 + 1;
            local9.AppendFormatted<int>(num18);
            temporaryAnimatedSprite11.lightId = this.GenerateLightSourceId(interpolatedStringHandler1.ToStringAndClear());
            temporaryAnimatedSprite11.lightRadius = 0.2f;
            temporaryAnimatedSprite11.xPeriodic = true;
            temporaryAnimatedSprite11.yPeriodic = true;
            temporaryAnimatedSprite11.xPeriodicLoopTime = 4000f;
            temporaryAnimatedSprite11.yPeriodicLoopTime = 5500f;
            temporaryAnimatedSprite11.xPeriodicRange = 32f;
            temporaryAnimatedSprite11.yPeriodicRange = 32f;
            temporarySprites9.Add(temporaryAnimatedSprite11);
            TemporaryAnimatedSpriteList temporarySprites10 = location.TemporarySprites;
            TemporaryAnimatedSprite temporaryAnimatedSprite12 = new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(232, 328, 4, 4), 9999999f, 1, 0, new Vector2(69f, 28f) * 64f + new Vector2(-32f, 0.0f), false, false, 1f, 0.0f, Color.White, 1f, 0.0f, 0.0f, 0.0f);
            temporaryAnimatedSprite12.lightcolor = Color.Orange;
            interpolatedStringHandler1 = new DefaultInterpolatedStringHandler(11, 1);
            interpolatedStringHandler1.AppendLiteral("abbyAtLake_");
            ref DefaultInterpolatedStringHandler local10 = ref interpolatedStringHandler1;
            int num20 = num19;
            int num21 = num20 + 1;
            local10.AppendFormatted<int>(num20);
            temporaryAnimatedSprite12.lightId = this.GenerateLightSourceId(interpolatedStringHandler1.ToStringAndClear());
            temporaryAnimatedSprite12.lightRadius = 0.2f;
            temporaryAnimatedSprite12.xPeriodic = true;
            temporaryAnimatedSprite12.yPeriodic = true;
            temporaryAnimatedSprite12.xPeriodicLoopTime = 2400f;
            temporaryAnimatedSprite12.yPeriodicLoopTime = 3600f;
            temporaryAnimatedSprite12.xPeriodicRange = 32f;
            temporaryAnimatedSprite12.yPeriodicRange = 21f;
            temporarySprites10.Add(temporaryAnimatedSprite12);
            TemporaryAnimatedSpriteList temporarySprites11 = location.TemporarySprites;
            TemporaryAnimatedSprite temporaryAnimatedSprite13 = new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(232, 328, 4, 4), 9999999f, 1, 0, new Vector2(69f, 28f) * 64f + new Vector2(32f, 0.0f), false, false, 1f, 0.0f, Color.White, 1f, 0.0f, 0.0f, 0.0f);
            temporaryAnimatedSprite13.lightcolor = Color.Orange;
            interpolatedStringHandler1 = new DefaultInterpolatedStringHandler(11, 1);
            interpolatedStringHandler1.AppendLiteral("abbyAtLake_");
            ref DefaultInterpolatedStringHandler local11 = ref interpolatedStringHandler1;
            int num22 = num21;
            int num23 = num22 + 1;
            local11.AppendFormatted<int>(num22);
            temporaryAnimatedSprite13.lightId = this.GenerateLightSourceId(interpolatedStringHandler1.ToStringAndClear());
            temporaryAnimatedSprite13.lightRadius = 0.2f;
            temporaryAnimatedSprite13.xPeriodic = true;
            temporaryAnimatedSprite13.yPeriodic = true;
            temporaryAnimatedSprite13.xPeriodicLoopTime = 2500f;
            temporaryAnimatedSprite13.yPeriodicLoopTime = 3600f;
            temporaryAnimatedSprite13.xPeriodicRange = 42f;
            temporaryAnimatedSprite13.yPeriodicRange = 51f;
            temporarySprites11.Add(temporaryAnimatedSprite13);
            TemporaryAnimatedSpriteList temporarySprites12 = location.TemporarySprites;
            TemporaryAnimatedSprite temporaryAnimatedSprite14 = new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(232, 328, 4, 4), 9999999f, 1, 0, new Vector2(69f, 28f) * 64f + new Vector2(32f, 32f), false, false, 1f, 0.0f, Color.White, 1f, 0.0f, 0.0f, 0.0f);
            temporaryAnimatedSprite14.lightcolor = Color.Orange;
            interpolatedStringHandler1 = new DefaultInterpolatedStringHandler(11, 1);
            interpolatedStringHandler1.AppendLiteral("abbyAtLake_");
            ref DefaultInterpolatedStringHandler local12 = ref interpolatedStringHandler1;
            int num24 = num23;
            int num25 = num24 + 1;
            local12.AppendFormatted<int>(num24);
            temporaryAnimatedSprite14.lightId = this.GenerateLightSourceId(interpolatedStringHandler1.ToStringAndClear());
            temporaryAnimatedSprite14.lightRadius = 0.2f;
            temporaryAnimatedSprite14.xPeriodic = true;
            temporaryAnimatedSprite14.yPeriodic = true;
            temporaryAnimatedSprite14.xPeriodicLoopTime = 4500f;
            temporaryAnimatedSprite14.yPeriodicLoopTime = 3000f;
            temporaryAnimatedSprite14.xPeriodicRange = 21f;
            temporaryAnimatedSprite14.yPeriodicRange = 32f;
            temporarySprites12.Add(temporaryAnimatedSprite14);
            TemporaryAnimatedSpriteList temporarySprites13 = location.TemporarySprites;
            TemporaryAnimatedSprite temporaryAnimatedSprite15 = new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(232, 328, 4, 4), 9999999f, 1, 0, new Vector2(69f, 28f) * 64f + new Vector2(0.0f, -32f), false, false, 1f, 0.0f, Color.White, 1f, 0.0f, 0.0f, 0.0f);
            temporaryAnimatedSprite15.lightcolor = Color.Orange;
            interpolatedStringHandler1 = new DefaultInterpolatedStringHandler(11, 1);
            interpolatedStringHandler1.AppendLiteral("abbyAtLake_");
            ref DefaultInterpolatedStringHandler local13 = ref interpolatedStringHandler1;
            int num26 = num25;
            int num27 = num26 + 1;
            local13.AppendFormatted<int>(num26);
            temporaryAnimatedSprite15.lightId = this.GenerateLightSourceId(interpolatedStringHandler1.ToStringAndClear());
            temporaryAnimatedSprite15.lightRadius = 0.2f;
            temporaryAnimatedSprite15.xPeriodic = true;
            temporaryAnimatedSprite15.yPeriodic = true;
            temporaryAnimatedSprite15.xPeriodicLoopTime = 5000f;
            temporaryAnimatedSprite15.yPeriodicLoopTime = 4500f;
            temporaryAnimatedSprite15.xPeriodicRange = 64f;
            temporaryAnimatedSprite15.yPeriodicRange = 48f;
            temporarySprites13.Add(temporaryAnimatedSprite15);
            TemporaryAnimatedSpriteList temporarySprites14 = location.TemporarySprites;
            TemporaryAnimatedSprite temporaryAnimatedSprite16 = new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(232, 328, 4, 4), 9999999f, 1, 0, new Vector2(72f, 33f) * 64f + new Vector2(-32f, 0.0f), false, false, 1f, 0.0f, Color.White, 1f, 0.0f, 0.0f, 0.0f);
            temporaryAnimatedSprite16.lightcolor = Color.Orange;
            interpolatedStringHandler1 = new DefaultInterpolatedStringHandler(11, 1);
            interpolatedStringHandler1.AppendLiteral("abbyAtLake_");
            ref DefaultInterpolatedStringHandler local14 = ref interpolatedStringHandler1;
            int num28 = num27;
            int num29 = num28 + 1;
            local14.AppendFormatted<int>(num28);
            temporaryAnimatedSprite16.lightId = this.GenerateLightSourceId(interpolatedStringHandler1.ToStringAndClear());
            temporaryAnimatedSprite16.lightRadius = 0.2f;
            temporaryAnimatedSprite16.xPeriodic = true;
            temporaryAnimatedSprite16.yPeriodic = true;
            temporaryAnimatedSprite16.xPeriodicLoopTime = 2000f;
            temporaryAnimatedSprite16.yPeriodicLoopTime = 3000f;
            temporaryAnimatedSprite16.xPeriodicRange = 32f;
            temporaryAnimatedSprite16.yPeriodicRange = 21f;
            temporarySprites14.Add(temporaryAnimatedSprite16);
            TemporaryAnimatedSpriteList temporarySprites15 = location.TemporarySprites;
            TemporaryAnimatedSprite temporaryAnimatedSprite17 = new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(232, 328, 4, 4), 9999999f, 1, 0, new Vector2(72f, 33f) * 64f + new Vector2(32f, 0.0f), false, false, 1f, 0.0f, Color.White, 1f, 0.0f, 0.0f, 0.0f);
            temporaryAnimatedSprite17.lightcolor = Color.Orange;
            interpolatedStringHandler1 = new DefaultInterpolatedStringHandler(11, 1);
            interpolatedStringHandler1.AppendLiteral("abbyAtLake_");
            ref DefaultInterpolatedStringHandler local15 = ref interpolatedStringHandler1;
            int num30 = num29;
            int num31 = num30 + 1;
            local15.AppendFormatted<int>(num30);
            temporaryAnimatedSprite17.lightId = this.GenerateLightSourceId(interpolatedStringHandler1.ToStringAndClear());
            temporaryAnimatedSprite17.lightRadius = 0.2f;
            temporaryAnimatedSprite17.xPeriodic = true;
            temporaryAnimatedSprite17.yPeriodic = true;
            temporaryAnimatedSprite17.xPeriodicLoopTime = 2900f;
            temporaryAnimatedSprite17.yPeriodicLoopTime = 3200f;
            temporaryAnimatedSprite17.xPeriodicRange = 21f;
            temporaryAnimatedSprite17.yPeriodicRange = 32f;
            temporarySprites15.Add(temporaryAnimatedSprite17);
            TemporaryAnimatedSpriteList temporarySprites16 = location.TemporarySprites;
            TemporaryAnimatedSprite temporaryAnimatedSprite18 = new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(232, 328, 4, 4), 9999999f, 1, 0, new Vector2(72f, 33f) * 64f + new Vector2(32f, 32f), false, false, 1f, 0.0f, Color.White, 1f, 0.0f, 0.0f, 0.0f);
            temporaryAnimatedSprite18.lightcolor = Color.Orange;
            interpolatedStringHandler1 = new DefaultInterpolatedStringHandler(11, 1);
            interpolatedStringHandler1.AppendLiteral("abbyAtLake_");
            ref DefaultInterpolatedStringHandler local16 = ref interpolatedStringHandler1;
            int num32 = num31;
            int num33 = num32 + 1;
            local16.AppendFormatted<int>(num32);
            temporaryAnimatedSprite18.lightId = this.GenerateLightSourceId(interpolatedStringHandler1.ToStringAndClear());
            temporaryAnimatedSprite18.lightRadius = 0.2f;
            temporaryAnimatedSprite18.xPeriodic = true;
            temporaryAnimatedSprite18.yPeriodic = true;
            temporaryAnimatedSprite18.xPeriodicLoopTime = 4200f;
            temporaryAnimatedSprite18.yPeriodicLoopTime = 3300f;
            temporaryAnimatedSprite18.xPeriodicRange = 16f;
            temporaryAnimatedSprite18.yPeriodicRange = 32f;
            temporarySprites16.Add(temporaryAnimatedSprite18);
            TemporaryAnimatedSpriteList temporarySprites17 = location.TemporarySprites;
            TemporaryAnimatedSprite temporaryAnimatedSprite19 = new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(232, 328, 4, 4), 9999999f, 1, 0, new Vector2(72f, 33f) * 64f + new Vector2(0.0f, -32f), false, false, 1f, 0.0f, Color.White, 1f, 0.0f, 0.0f, 0.0f);
            temporaryAnimatedSprite19.lightcolor = Color.Orange;
            interpolatedStringHandler1 = new DefaultInterpolatedStringHandler(11, 1);
            interpolatedStringHandler1.AppendLiteral("abbyAtLake_");
            ref DefaultInterpolatedStringHandler local17 = ref interpolatedStringHandler1;
            int num34 = num33;
            int num35 = num34 + 1;
            local17.AppendFormatted<int>(num34);
            temporaryAnimatedSprite19.lightId = this.GenerateLightSourceId(interpolatedStringHandler1.ToStringAndClear());
            temporaryAnimatedSprite19.lightRadius = 0.2f;
            temporaryAnimatedSprite19.xPeriodic = true;
            temporaryAnimatedSprite19.yPeriodic = true;
            temporaryAnimatedSprite19.xPeriodicLoopTime = 5100f;
            temporaryAnimatedSprite19.yPeriodicLoopTime = 4000f;
            temporaryAnimatedSprite19.xPeriodicRange = 32f;
            temporaryAnimatedSprite19.yPeriodicRange = 16f;
            temporarySprites17.Add(temporaryAnimatedSprite19);
            return;
          case 'S':
            if (!(key == "junimoShow"))
              return;
            Texture2D texture2D4 = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\temporary_sprites_1");
            location.TemporarySprites.Add(new TemporaryAnimatedSprite()
            {
              texture = texture2D4,
              sourceRect = new Microsoft.Xna.Framework.Rectangle(393, 350, 19, 14),
              animationLength = 6,
              sourceRectStartingPos = new Vector2(393f, 350f),
              interval = 90f,
              totalNumberOfLoops = 86,
              position = new Vector2(52f, 24f) * 64f + new Vector2(7f, -2f) * 4f,
              scale = 4f,
              layerDepth = 0.95f
            });
            location.TemporarySprites.Add(new TemporaryAnimatedSprite()
            {
              texture = texture2D4,
              sourceRect = new Microsoft.Xna.Framework.Rectangle(393, 364, 19, 14),
              animationLength = 4,
              sourceRectStartingPos = new Vector2(393f, 364f),
              interval = 90f,
              totalNumberOfLoops = 31 /*0x1F*/,
              position = new Vector2(52f, 24f) * 64f + new Vector2(7f, -2f) * 4f,
              scale = 4f,
              layerDepth = 0.97f,
              delayBeforeAnimationStart = 11034
            });
            location.TemporarySprites.Add(new TemporaryAnimatedSprite()
            {
              texture = texture2D4,
              sourceRect = new Microsoft.Xna.Framework.Rectangle(393, 378, 19, 14),
              animationLength = 6,
              sourceRectStartingPos = new Vector2(393f, 378f),
              interval = 90f,
              totalNumberOfLoops = 21,
              position = new Vector2(52f, 24f) * 64f + new Vector2(7f, -2f) * 4f,
              scale = 4f,
              layerDepth = 1f,
              delayBeforeAnimationStart = 22069
            });
            return;
          case 'W':
            if (!(key == "wizardWarp"))
              return;
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(387, 1965, 16 /*0x10*/, 31 /*0x1F*/), 9999f, 1, 999999, new Vector2(8f, 16f) * 64f + new Vector2(0.0f, 4f), false, false, 1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
            {
              motion = new Vector2(2f, -2f),
              acceleration = new Vector2(0.1f, 0.0f),
              scaleChange = -0.02f,
              alphaFade = 1f / 1000f
            });
            return;
          case 'a':
            if (!(key == "maruBeaker"))
              return;
            location.TemporarySprites.Add(new TemporaryAnimatedSprite(738, 1380f, 1, 0, new Vector2(9f, 14f) * 64f + new Vector2(0.0f, 32f), false, false)
            {
              rotationChange = 0.1308997f,
              motion = new Vector2(0.0f, -7f),
              acceleration = new Vector2(0.0f, 0.2f),
              endFunction = new TemporaryAnimatedSprite.endBehavior(this.beakerSmashEndFunction),
              layerDepth = 1f
            });
            return;
          case 'b':
            if (!(key == "evilRabbit"))
              return;
            location.temporarySprites.Add(new TemporaryAnimatedSprite()
            {
              texture = Game1.temporaryContent.Load<Texture2D>("TileSheets\\critters"),
              sourceRect = new Microsoft.Xna.Framework.Rectangle(264, 209, 19, 16 /*0x10*/),
              sourceRectStartingPos = new Vector2(264f, 209f),
              animationLength = 1,
              totalNumberOfLoops = 999,
              interval = 999f,
              scale = 4f,
              position = new Vector2(4f, 1f) * 64f + new Vector2(38f, 23f) * 4f,
              layerDepth = 1f,
              motion = new Vector2(-2f, -2f),
              acceleration = new Vector2(0.0f, 0.1f),
              yStopCoordinate = 204,
              xStopCoordinate = 316,
              flipped = true,
              id = 778
            });
            return;
          case 'c':
            if (!(key == "leahPicnic"))
              return;
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(96 /*0x60*/, 1808, 32 /*0x20*/, 48 /*0x30*/), 9999f, 1, 999, new Vector2(75f, 37f) * 64f, false, false, 0.2496f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f));
            this.actors.Add(new NPC(new AnimatedSprite((ContentManager) this.festivalContent, "Characters\\" + (this.farmer.IsMale ? "LeahExMale" : "LeahExFemale"), 0, 16 /*0x10*/, 32 /*0x20*/), new Vector2(-100f, -100f) * 64f, 2, "LeahEx")
            {
              AllowDynamicAppearance = false
            });
            return;
          case 'd':
            return;
          case 'e':
            if (!(key == "abbyOneBat"))
              return;
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(640, 1664, 16 /*0x10*/, 16 /*0x10*/), 80f, 4, 9999, new Vector2(23f, 9f) * 64f, false, false, 1f, 3f / 1000f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
            {
              xPeriodic = true,
              xPeriodicLoopTime = 2000f,
              xPeriodicRange = 128f,
              motion = new Vector2(0.0f, -8f)
            });
            return;
          case 'f':
            return;
          case 'g':
            return;
          case 'h':
            if (!(key == "waterShane"))
              return;
            this.drawTool = true;
            this.farmer.TemporaryItem = ItemRegistry.Create("(T)WateringCan");
            this.farmer.CurrentTool.Update(1, 0, this.farmer);
            this.farmer.FarmerSprite.animateOnce(new FarmerSprite.AnimationFrame[4]
            {
              new FarmerSprite.AnimationFrame(58, 0, false, false),
              new FarmerSprite.AnimationFrame(58, 75, false, false, new AnimatedSprite.endOfAnimationBehavior(Farmer.showToolSwipeEffect)),
              new FarmerSprite.AnimationFrame(59, 100, false, false, new AnimatedSprite.endOfAnimationBehavior(Farmer.useTool), true),
              new FarmerSprite.AnimationFrame(45, 500, true, false, new AnimatedSprite.endOfAnimationBehavior(Farmer.canMoveNow), true)
            });
            return;
          case 'i':
            return;
          case 'j':
            return;
          case 'k':
            return;
          case 'l':
            if (!(key == "witchFlyby"))
              return;
            Game1.screenOverlayTempSprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(276, 1886, 35, 29), 9999f, 1, 999999, new Vector2((float) Game1.graphics.GraphicsDevice.Viewport.Width, 192f), false, false, 1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
            {
              motion = new Vector2(-4f, 0.0f),
              acceleration = new Vector2(-0.025f, 0.0f),
              yPeriodic = true,
              yPeriodicLoopTime = 2000f,
              yPeriodicRange = 64f,
              local = true
            });
            return;
          case 'm':
            return;
          case 'n':
            if (!(key == "joshDinner"))
              return;
            location.TemporarySprites.Add(new TemporaryAnimatedSprite(649, 9999f, 1, 9999, new Vector2(6f, 4f) * 64f + new Vector2(8f, 32f), false, false)
            {
              layerDepth = 0.0256f
            });
            location.TemporarySprites.Add(new TemporaryAnimatedSprite(664, 9999f, 1, 9999, new Vector2(8f, 4f) * 64f + new Vector2(-8f, 32f), false, false)
            {
              layerDepth = 0.0256f
            });
            return;
          case 'o':
            switch (key)
            {
              case "luauShorts":
                Vector2 vector2_1 = Game1.year % 2 == 0 ? new Vector2(24f, 10f) : new Vector2(35f, 10f);
                location.TemporarySprites.Add(new TemporaryAnimatedSprite("Maps\\springobjects", new Microsoft.Xna.Framework.Rectangle(336, 512 /*0x0200*/, 16 /*0x10*/, 16 /*0x10*/), 9999f, 1, 99999, vector2_1 * 64f, false, false, 1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
                {
                  motion = new Vector2(-2f, -8f),
                  acceleration = new Vector2(0.0f, 0.25f),
                  yStopCoordinate = ((int) vector2_1.Y + 1) * 64 /*0x40*/,
                  xStopCoordinate = ((int) vector2_1.X - 2) * 64 /*0x40*/
                });
                return;
              case "linusMoney":
                location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(397, 1941, 19, 20), 9999f, 1, 99999, new Vector2(-1002f, -1000f) * 64f, false, false, 1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
                {
                  startSound = "money",
                  delayBeforeAnimationStart = 10,
                  overrideLocationDestroy = true
                });
                location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(397, 1941, 19, 20), 9999f, 1, 99999, new Vector2(-1003f, -1002f) * 64f, false, false, 1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
                {
                  startSound = "money",
                  delayBeforeAnimationStart = 100,
                  overrideLocationDestroy = true
                });
                location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(397, 1941, 19, 20), 9999f, 1, 99999, new Vector2(-999f, -1000f) * 64f, false, false, 1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
                {
                  startSound = "money",
                  delayBeforeAnimationStart = 200,
                  overrideLocationDestroy = true
                });
                location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(397, 1941, 19, 20), 9999f, 1, 99999, new Vector2(-1004f, -1001f) * 64f, false, false, 1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
                {
                  startSound = "money",
                  delayBeforeAnimationStart = 300,
                  overrideLocationDestroy = true
                });
                location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(397, 1941, 19, 20), 9999f, 1, 99999, new Vector2(-1001f, -998f) * 64f, false, false, 1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
                {
                  startSound = "money",
                  delayBeforeAnimationStart = 400,
                  overrideLocationDestroy = true
                });
                location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(397, 1941, 19, 20), 9999f, 1, 99999, new Vector2(-998f, -999f) * 64f, false, false, 1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
                {
                  startSound = "money",
                  delayBeforeAnimationStart = 500,
                  overrideLocationDestroy = true
                });
                location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(397, 1941, 19, 20), 9999f, 1, 99999, new Vector2(-998f, -1002f) * 64f, false, false, 1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
                {
                  startSound = "money",
                  delayBeforeAnimationStart = 600,
                  overrideLocationDestroy = true
                });
                location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(397, 1941, 19, 20), 9999f, 1, 99999, new Vector2(-997f, -1001f) * 64f, false, false, 1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
                {
                  startSound = "money",
                  delayBeforeAnimationStart = 700,
                  overrideLocationDestroy = true
                });
                return;
              default:
                return;
            }
          case 'p':
            if (!(key == "leahLaptop"))
              return;
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(130, 1849, 19, 19), 9999f, 1, 999, new Vector2(12f, 10f) * 64f + new Vector2(0.0f, 24f), false, false, 0.1856f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f));
            return;
          case 'q':
            return;
          case 'r':
            switch (key)
            {
              case "BoatParrot":
                this.aboveMapSprites = new TemporaryAnimatedSpriteList();
                this.aboveMapSprites.Add(new TemporaryAnimatedSprite("LooseSprites\\parrots", new Microsoft.Xna.Framework.Rectangle(48 /*0x30*/, 0, 24, 24), 100f, 3, 99999, new Vector2((float) (Game1.viewport.X - 64 /*0x40*/), 2112f), false, true, 1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
                {
                  id = 999,
                  motion = new Vector2(6f, 1f),
                  delayBeforeAnimationStart = 0,
                  pingPong = true,
                  xStopCoordinate = 1040,
                  reachedStopCoordinate = (TemporaryAnimatedSprite.endBehavior) (param2 =>
                  {
                    TemporaryAnimatedSprite aboveMapSprite1 = this.aboveMapSprites[0];
                    if (aboveMapSprite1 == null)
                      return;
                    aboveMapSprite1.motion = new Vector2(0.0f, 2f);
                    aboveMapSprite1.yStopCoordinate = 2336;
                    aboveMapSprite1.reachedStopCoordinate = (TemporaryAnimatedSprite.endBehavior) (param3 =>
                    {
                      TemporaryAnimatedSprite aboveMapSprite2 = this.aboveMapSprites[0];
                      aboveMapSprite2.animationLength = 1;
                      aboveMapSprite2.pingPong = false;
                      aboveMapSprite2.sourceRect = new Microsoft.Xna.Framework.Rectangle(0, 0, 24, 24);
                      aboveMapSprite2.sourceRectStartingPos = Vector2.Zero;
                    });
                  })
                });
                return;
              case "movieFrame":
                string id1;
                string error2;
                int frame1;
                int num36;
                if (!ArgUtility.TryGet(args, 2, out id1, out error2, name: "string movieId") || !ArgUtility.TryGetInt(args, 3, out frame1, out error2, "int frame") || !ArgUtility.TryGetInt(args, 4, out num36, out error2, "int duration"))
                {
                  this.LogCommandError(args, error2);
                  return;
                }
                string idFromLegacyIndex1 = MovieTheater.GetMovieIdFromLegacyIndex(id1);
                MovieData data1;
                if (!MovieTheater.TryGetMovieData(idFromLegacyIndex1, out data1))
                {
                  this.LogCommandError(args, $"no movie found with ID '{idFromLegacyIndex1}'");
                  return;
                }
                Microsoft.Xna.Framework.Rectangle sourceRectForScreen1 = MovieTheater.GetSourceRectForScreen(data1.SheetIndex, frame1);
                location.temporarySprites.Add(new TemporaryAnimatedSprite()
                {
                  texture = Game1.temporaryContent.Load<Texture2D>(data1.Texture ?? "LooseSprites\\Movies"),
                  sourceRect = sourceRectForScreen1,
                  sourceRectStartingPos = new Vector2((float) sourceRectForScreen1.X, (float) sourceRectForScreen1.Y),
                  animationLength = 1,
                  totalNumberOfLoops = 1,
                  interval = (float) num36,
                  scale = 4f,
                  position = new Vector2(4f, 1f) * 64f + new Vector2(3f, 7f) * 4f,
                  shakeIntensity = 0.25f,
                  layerDepth = 0.0192f,
                  id = 997
                });
                return;
              default:
                return;
            }
          case 's':
            return;
          case 't':
            if (!(key == "beachStuff"))
              return;
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(324, 1887, 47, 29), 9999f, 1, 999, new Vector2(44f, 21f) * 64f, false, false, 1E-05f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f));
            return;
          case 'u':
            return;
          case 'v':
            return;
          case 'w':
            if (!(key == "swordswipe"))
              return;
            Vector2 vector2_2;
            string error3;
            if (!ArgUtility.TryGetVector2(args, 2, out vector2_2, out error3, true, "Vector2 position"))
            {
              this.LogCommandError(args, error3);
              return;
            }
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(0, 960, 128 /*0x80*/, 128 /*0x80*/), 60f, 4, 0, vector2_2 * 64f + new Vector2(0.0f, -32f), false, false, 1f, 0.0f, Color.White, 1f, 0.0f, 0.0f, 0.0f));
            return;
          default:
            return;
        }
      case 11:
        switch (key[5])
        {
          case 'C':
            if (!(key == "shaneCliffs"))
              return;
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(533, 1864, 19, 27), 99999f, 1, 99999, new Vector2(83f, 98f) * 64f, false, false, 0.01f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
            {
              id = 999
            });
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(552, 1862, 31 /*0x1F*/, 21), 99999f, 1, 99999, new Vector2(83f, 98f) * 64f + new Vector2(-16f, 0.0f), false, false, 0.0001f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f));
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(549, 1891, 19, 12), 99999f, 1, 99999, new Vector2(84f, 99f) * 64f, false, false, 0.01f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
            {
              id = 999
            });
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(549, 1891, 19, 12), 99999f, 1, 99999, new Vector2(82f, 98f) * 64f, false, false, 0.01f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
            {
              id = 999
            });
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(542, 1893, 4, 6), 99999f, 1, 99999, new Vector2(83f, 99f) * 64f + new Vector2(-8f, 4f) * 4f, false, false, 1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f));
            return;
          case 'L':
            if (!(key == "linusLights"))
              return;
            string lightSourceId = this.GenerateLightSourceId("linusLights");
            Game1.currentLightSources.Add(new LightSource(lightSourceId + "1", 2, new Vector2(55f, 62f) * 64f, 2f));
            Game1.currentLightSources.Add(new LightSource(lightSourceId + "2", 2, new Vector2(60f, 62f) * 64f, 2f));
            Game1.currentLightSources.Add(new LightSource(lightSourceId + "3", 2, new Vector2(57f, 60f) * 64f, 3f));
            Game1.currentLightSources.Add(new LightSource(lightSourceId + "4", 2, new Vector2(57f, 60f) * 64f, 2f));
            Game1.currentLightSources.Add(new LightSource(lightSourceId + "5", 2, new Vector2(47f, 70f) * 64f, 2f));
            Game1.currentLightSources.Add(new LightSource(lightSourceId + "6", 2, new Vector2(52f, 63f) * 64f, 2f));
            return;
          case 'd':
            if (!(key == "wizardWarp2"))
              return;
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(387, 1965, 16 /*0x10*/, 31 /*0x1F*/), 9999f, 1, 999999, new Vector2(54f, 34f) * 64f + new Vector2(0.0f, 4f), false, false, 1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
            {
              motion = new Vector2(-1f, 2f),
              acceleration = new Vector2(-0.1f, 0.2f),
              scaleChange = 0.03f,
              alphaFade = 1f / 1000f
            });
            return;
          case 'e':
            return;
          case 'f':
            if (!(key == "jasGiftOpen"))
              return;
            location.getTemporarySpriteByID(999).paused = false;
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(537, 1850, 11, 10), 1500f, 1, 1, new Vector2(23f, 16f) * 64f + new Vector2(16f, -48f), false, false, 0.99f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
            {
              motion = new Vector2(0.0f, -0.25f),
              delayBeforeAnimationStart = 500,
              yStopCoordinate = 928
            });
            location.temporarySprites.AddRange((IEnumerable<TemporaryAnimatedSprite>) Utility.sparkleWithinArea(new Microsoft.Xna.Framework.Rectangle(1440, 992, 128 /*0x80*/, 64 /*0x40*/), 5, Color.White, 300));
            return;
          case 'g':
            if (!(key == "springOnion"))
              return;
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(1, 129, 16 /*0x10*/, 16 /*0x10*/), 200f, 8, 999999, new Vector2(84f, 39f) * 64f, false, false, 0.4736f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
            {
              id = 999
            });
            return;
          case 'h':
            return;
          case 'i':
            if (!(key == "curtainOpen"))
              return;
            location.getTemporarySpriteByID(999).sourceRect.X = 672;
            Game1.playSound("shwip");
            return;
          case 'j':
            return;
          case 'k':
            return;
          case 'l':
            if (!(key == "dickGlitter"))
              return;
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(432, 1435, 16 /*0x10*/, 16 /*0x10*/), 100f, 6, 99999, new Vector2(47f, 8f) * 64f, false, false, 1f, 0.0f, Color.White, 2f, 0.0f, 0.0f, 0.0f));
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(432, 1435, 16 /*0x10*/, 16 /*0x10*/), 100f, 6, 99999, new Vector2(47f, 8f) * 64f + new Vector2(32f, 0.0f), false, false, 1f, 0.0f, Color.White, 2f, 0.0f, 0.0f, 0.0f)
            {
              delayBeforeAnimationStart = 200
            });
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(432, 1435, 16 /*0x10*/, 16 /*0x10*/), 100f, 6, 99999, new Vector2(47f, 8f) * 64f + new Vector2(32f, 32f), false, false, 1f, 0.0f, Color.White, 2f, 0.0f, 0.0f, 0.0f)
            {
              delayBeforeAnimationStart = 300
            });
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(432, 1435, 16 /*0x10*/, 16 /*0x10*/), 100f, 6, 99999, new Vector2(47f, 8f) * 64f + new Vector2(0.0f, 32f), false, false, 1f, 0.0f, Color.White, 2f, 0.0f, 0.0f, 0.0f)
            {
              delayBeforeAnimationStart = 100
            });
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(432, 1435, 16 /*0x10*/, 16 /*0x10*/), 100f, 6, 99999, new Vector2(47f, 8f) * 64f + new Vector2(16f, 16f), false, false, 1f, 0.0f, Color.White, 2f, 0.0f, 0.0f, 0.0f)
            {
              delayBeforeAnimationStart = 400
            });
            return;
          case 'o':
            if (!(key == "raccoonSong"))
              return;
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors_1_6", new Microsoft.Xna.Framework.Rectangle(279, 55, 12, 15), 297f, 8, 999, new Vector2(3706f, 340f) - new Vector2(6.5f, 12f) * 4f, false, false)
            {
              scale = 4f,
              layerDepth = 0.0448099971f,
              usePreciseTiming = true
            });
            for (int index = 0; index < 8; ++index)
            {
              location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors_1_6", new Microsoft.Xna.Framework.Rectangle(304, 397, 11, 11), 49f, 12, 1, new Vector2(3706f, 340f) + new Vector2(14f, -12f) * 4f, false, false)
              {
                scale = 4f,
                layerDepth = 0.05057f,
                delayBeforeAnimationStart = 2376 * index,
                usePreciseTiming = true,
                motion = new Vector2(1f, 0.0f),
                acceleration = new Vector2(0.0f, 1f / 1000f),
                color = new Color((int) byte.MaxValue, 200, 200),
                rotationChange = (float) Game1.random.Next(-20, 20) / 1000f
              });
              location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors_1_6", new Microsoft.Xna.Framework.Rectangle(455, 414, 14, 17), 2376f, 1, 999, new Vector2(3706f, 340f) + new Vector2(7f, -12f) * 4f, false, false)
              {
                scale = 4f,
                layerDepth = 0.0512099974f,
                delayBeforeAnimationStart = 2376 * index,
                alphaFade = 0.02f,
                usePreciseTiming = true
              });
            }
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors_1_6", new Microsoft.Xna.Framework.Rectangle(374, 55, 12, 15), 297f, 8, 999, new Vector2(54f, 4f) * 64f + new Vector2(0.0f, -16f), false, true)
            {
              scale = 4f,
              layerDepth = 0.0448099971f,
              delayBeforeAnimationStart = 297,
              usePreciseTiming = true
            });
            for (int index = 0; index < 8; ++index)
              location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors_1_6", new Microsoft.Xna.Framework.Rectangle(385, 414, 14, 17), 2376f, 1, 999, new Vector2(54f, 4f) * 64f + new Vector2(16f, -8f) + new Vector2(-15f, -17f) * 4f, false, false)
              {
                scale = 4f,
                layerDepth = 0.0512099974f,
                delayBeforeAnimationStart = 2376 * index + 297,
                alphaFade = 0.02f,
                usePreciseTiming = true
              });
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors_1_6", new Microsoft.Xna.Framework.Rectangle(279, 55, 12, 15), 297f, 8, 999, new Vector2(3462f, 433f) - new Vector2(6.5f, 12f) * 4f, false, false)
            {
              scale = 4f,
              layerDepth = 0.0448099971f,
              delayBeforeAnimationStart = 594,
              usePreciseTiming = true
            });
            for (int index = 0; index < 8; ++index)
            {
              location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors_1_6", new Microsoft.Xna.Framework.Rectangle(304, 397, 11, 11), 49f, 12, 1, new Vector2(3462f, 433f) + new Vector2(-20f, -16f) + new Vector2(-15f, -17f) * 4f, false, false)
              {
                scale = 4f,
                layerDepth = 0.05057f,
                delayBeforeAnimationStart = 2376 * index + 594,
                usePreciseTiming = true,
                motion = new Vector2(-1f, -1f),
                acceleration = new Vector2(0.0f, 1f / 1000f),
                color = new Color(180, 200, (int) byte.MaxValue),
                rotationChange = (float) Game1.random.Next(-20, 20) / 1000f
              });
              location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors_1_6", new Microsoft.Xna.Framework.Rectangle(371, 414, 14, 17), 2376f, 1, 999, new Vector2(3462f, 433f) + new Vector2(-20f, -16f) + new Vector2(-15f, -17f) * 4f, false, false)
              {
                scale = 4f,
                layerDepth = 0.0512099974f,
                delayBeforeAnimationStart = 2376 * index + 594,
                alphaFade = 0.013f,
                usePreciseTiming = true
              });
            }
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors_1_6", new Microsoft.Xna.Framework.Rectangle(374, 55, 12, 15), 297f, 8, 999, new Vector2(58f, 4f) * 64f + new Vector2(0.0f, -24f), false, false)
            {
              scale = 4f,
              layerDepth = 0.0448099971f,
              delayBeforeAnimationStart = 891,
              usePreciseTiming = true
            });
            for (int index = 0; index < 8; ++index)
              location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors_1_6", new Microsoft.Xna.Framework.Rectangle(440, 415, 14, 15), 2376f, 1, 999, new Vector2(58f, 4f) * 64f + new Vector2(48f, -56f), false, false)
              {
                scale = 4f,
                layerDepth = 0.0512099974f,
                delayBeforeAnimationStart = 2376 * index + 891,
                alphaFade = 0.02f,
                usePreciseTiming = true
              });
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors_1_6", new Microsoft.Xna.Framework.Rectangle(279, 55, 12, 15), 297f, 8, 999, new Vector2(3770f, 408f) - new Vector2(6.5f, 12f) * 4f, false, false)
            {
              scale = 4f,
              layerDepth = 0.0448099971f,
              delayBeforeAnimationStart = 1188,
              usePreciseTiming = true
            });
            for (int index = 0; index < 8; ++index)
              location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors_1_6", new Microsoft.Xna.Framework.Rectangle(469, 415, 14, 14), 2376f, 1, 999, new Vector2(3770f, 408f) + new Vector2(24f, -64f), false, false)
              {
                scale = 4f,
                layerDepth = 0.0512099974f,
                delayBeforeAnimationStart = 2376 * index + 1188,
                alphaFade = 0.02f,
                usePreciseTiming = true
              });
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors_1_6", new Microsoft.Xna.Framework.Rectangle(279, 55, 12, 15), 297f, 8, 999, new Vector2(55f, 3f) * 64f + new Vector2(12f, 4f) - new Vector2(6.5f, 12f) * 4f, false, false)
            {
              scale = 4f,
              layerDepth = 0.0448099971f,
              delayBeforeAnimationStart = 1485,
              usePreciseTiming = true
            });
            for (int index = 0; index < 8; ++index)
              location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors_1_6", new Microsoft.Xna.Framework.Rectangle(400, 414, 12, 16 /*0x10*/), 2376f, 1, 999, new Vector2(55f, 3f) * 64f + new Vector2(-32f, -100f), false, false)
              {
                scale = 4f,
                layerDepth = 0.0512099974f,
                delayBeforeAnimationStart = 2376 * index + 1485,
                alphaFade = 0.02f,
                usePreciseTiming = true
              });
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors_1_6", new Microsoft.Xna.Framework.Rectangle(279, 55, 12, 15), 297f, 8, 999, new Vector2(56f, 3f) * 64f + new Vector2(40f, -8f) - new Vector2(6.5f, 12f) * 4f, false, false)
            {
              scale = 4f,
              layerDepth = 0.0448099971f,
              delayBeforeAnimationStart = 1782,
              usePreciseTiming = true
            });
            for (int index = 0; index < 8; ++index)
            {
              location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors_1_6", new Microsoft.Xna.Framework.Rectangle(304, 397, 11, 11), 49f, 12, 1, new Vector2(56f, 3f) * 64f + new Vector2(12f, -112f), false, false)
              {
                scale = 4f,
                layerDepth = 0.05057f,
                delayBeforeAnimationStart = 2376 * index + 1782,
                usePreciseTiming = true,
                motion = new Vector2(-0.25f, -1.5f),
                acceleration = new Vector2(0.0f, 1f / 1000f),
                color = new Color(220, (int) byte.MaxValue, 180),
                rotationChange = (float) Game1.random.Next(-20, 20) / 1000f
              });
              location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors_1_6", new Microsoft.Xna.Framework.Rectangle(414, 414, 12, 16 /*0x10*/), 2376f, 1, 999, new Vector2(56f, 3f) * 64f + new Vector2(12f, -112f), false, false)
              {
                scale = 4f,
                layerDepth = 0.0512099974f,
                delayBeforeAnimationStart = 2376 * index + 1782,
                alphaFade = 0.013f,
                usePreciseTiming = true
              });
            }
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors_1_6", new Microsoft.Xna.Framework.Rectangle(374, 55, 12, 15), 297f, 8, 999, new Vector2(58f, 3f) * 64f + new Vector2(-24f, -52f), false, false)
            {
              scale = 4f,
              layerDepth = 0.0448099971f,
              delayBeforeAnimationStart = 2079,
              usePreciseTiming = true
            });
            for (int index = 0; index < 8; ++index)
              location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors_1_6", new Microsoft.Xna.Framework.Rectangle(426, 414, 14, 15), 2376f, 1, 999, new Vector2(58f, 3f) * 64f + new Vector2(28f, -88f), false, false)
              {
                scale = 4f,
                layerDepth = 0.0512099974f,
                delayBeforeAnimationStart = 2376 * index + 2079,
                alphaFade = 0.02f,
                usePreciseTiming = true
              });
            return;
          case 's':
            switch (key)
            {
              case "krobusBeach":
                for (int index = 0; index < 8; ++index)
                  location.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(0, 0, 64 /*0x40*/, 64 /*0x40*/), 150f, 4, 0, new Vector2((float) (84.0 + (index % 2 == 0 ? 0.25 : -0.05000000074505806)), 41f) * 64f, false, Game1.random.NextBool(), 1f / 1000f, 0.02f, Color.White, 0.75f, 3f / 1000f, 0.0f, 0.0f)
                  {
                    delayBeforeAnimationStart = 500 + index * 1000,
                    startSound = "waterSlosh"
                  });
                this.underwaterSprites = new TemporaryAnimatedSpriteList()
                {
                  new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(256 /*0x0100*/, 16 /*0x10*/, 16 /*0x10*/, 16 /*0x10*/), 250f, 3, 9999, new Vector2(82f, 52f) * 64f, false, false, 0.1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
                  {
                    motion = new Vector2(0.0f, -1f),
                    xPeriodic = true,
                    xPeriodicLoopTime = 3000f,
                    xPeriodicRange = 16f,
                    lightId = this.GenerateLightSourceId("krobusBeach_1"),
                    lightcolor = Color.Black,
                    lightRadius = 1f,
                    yStopCoordinate = 2688,
                    delayBeforeAnimationStart = 0,
                    pingPong = true
                  },
                  new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(256 /*0x0100*/, 16 /*0x10*/, 16 /*0x10*/, 16 /*0x10*/), 250f, 3, 9999, new Vector2(82f, 52f) * 64f, false, false, 0.1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
                  {
                    motion = new Vector2(0.0f, -1f),
                    xPeriodic = true,
                    xPeriodicLoopTime = 3000f,
                    xPeriodicRange = 16f,
                    lightId = this.GenerateLightSourceId("krobusBeach_2"),
                    lightcolor = Color.Black,
                    lightRadius = 1f,
                    yStopCoordinate = 3008,
                    delayBeforeAnimationStart = 2000,
                    pingPong = true
                  },
                  new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(256 /*0x0100*/, 16 /*0x10*/, 16 /*0x10*/, 16 /*0x10*/), 250f, 3, 9999, new Vector2(88f, 52f) * 64f, false, false, 0.1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
                  {
                    motion = new Vector2(0.0f, -1f),
                    xPeriodic = true,
                    xPeriodicLoopTime = 3000f,
                    xPeriodicRange = 16f,
                    lightId = this.GenerateLightSourceId("krobusBeach_3"),
                    lightcolor = Color.Black,
                    lightRadius = 1f,
                    yStopCoordinate = 2688,
                    delayBeforeAnimationStart = 150,
                    pingPong = true
                  },
                  new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(256 /*0x0100*/, 16 /*0x10*/, 16 /*0x10*/, 16 /*0x10*/), 250f, 3, 9999, new Vector2(88f, 52f) * 64f, false, false, 0.1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
                  {
                    motion = new Vector2(0.0f, -1f),
                    xPeriodic = true,
                    xPeriodicLoopTime = 3000f,
                    xPeriodicRange = 16f,
                    lightId = this.GenerateLightSourceId("krobusBeach_4"),
                    lightcolor = Color.Black,
                    lightRadius = 1f,
                    yStopCoordinate = 3008,
                    delayBeforeAnimationStart = 2000,
                    pingPong = true
                  },
                  new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(256 /*0x0100*/, 16 /*0x10*/, 16 /*0x10*/, 16 /*0x10*/), 250f, 3, 9999, new Vector2(90f, 52f) * 64f, false, false, 0.1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
                  {
                    motion = new Vector2(0.0f, -1f),
                    xPeriodic = true,
                    xPeriodicLoopTime = 3000f,
                    xPeriodicRange = 16f,
                    lightId = this.GenerateLightSourceId("krobusBeach_5"),
                    lightcolor = Color.Black,
                    lightRadius = 1f,
                    yStopCoordinate = 2816 /*0x0B00*/,
                    delayBeforeAnimationStart = 300,
                    pingPong = true
                  },
                  new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(256 /*0x0100*/, 16 /*0x10*/, 16 /*0x10*/, 16 /*0x10*/), 250f, 3, 9999, new Vector2(79f, 52f) * 64f, false, false, 0.1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
                  {
                    motion = new Vector2(0.0f, -1f),
                    xPeriodic = true,
                    xPeriodicLoopTime = 3000f,
                    xPeriodicRange = 16f,
                    lightId = this.GenerateLightSourceId("krobusBeach_6"),
                    lightcolor = Color.Black,
                    lightRadius = 1f,
                    yStopCoordinate = 2816 /*0x0B00*/,
                    delayBeforeAnimationStart = 1000,
                    pingPong = true
                  }
                };
                return;
              case "krobusraven":
                location.TemporarySprites.Add(new TemporaryAnimatedSprite("Characters\\KrobusRaven", new Microsoft.Xna.Framework.Rectangle(0, 0, 32 /*0x20*/, 32 /*0x20*/), 100f, 5, 999999, new Vector2((float) Game1.viewport.Width, (float) Game1.viewport.Height * 0.33f), false, false, 0.9f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
                {
                  pingPong = true,
                  motion = new Vector2(-2f, 0.0f),
                  yPeriodic = true,
                  yPeriodicLoopTime = 3000f,
                  yPeriodicRange = 16f,
                  startSound = "shadowpeep"
                });
                location.TemporarySprites.Add(new TemporaryAnimatedSprite("Characters\\KrobusRaven", new Microsoft.Xna.Framework.Rectangle(0, 32 /*0x20*/, 32 /*0x20*/, 32 /*0x20*/), 30f, 5, 999999, new Vector2((float) Game1.viewport.Width, (float) Game1.viewport.Height * 0.33f), false, false, 0.9f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
                {
                  motion = new Vector2(-2.5f, 0.0f),
                  yPeriodic = true,
                  yPeriodicLoopTime = 2800f,
                  yPeriodicRange = 16f,
                  delayBeforeAnimationStart = 8000
                });
                location.TemporarySprites.Add(new TemporaryAnimatedSprite("Characters\\KrobusRaven", new Microsoft.Xna.Framework.Rectangle(0, 64 /*0x40*/, 32 /*0x20*/, 39), 100f, 4, 999999, new Vector2((float) Game1.viewport.Width, (float) Game1.viewport.Height * 0.33f), false, false, 0.9f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
                {
                  pingPong = true,
                  motion = new Vector2(-3f, 0.0f),
                  yPeriodic = true,
                  yPeriodicLoopTime = 2000f,
                  yPeriodicRange = 16f,
                  delayBeforeAnimationStart = 15000,
                  startSound = "fireball"
                });
                location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(276, 1886, 35, 29), 9999f, 1, 999999, new Vector2((float) Game1.viewport.Width, (float) Game1.viewport.Height * 0.33f), false, false, 1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
                {
                  motion = new Vector2(-3f, 0.0f),
                  yPeriodic = true,
                  yPeriodicLoopTime = 2200f,
                  yPeriodicRange = 32f,
                  local = true,
                  delayBeforeAnimationStart = 20000
                });
                for (int index = 0; index < 12; ++index)
                  location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(16 /*0x10*/, 594, 16 /*0x10*/, 12), 100f, 2, 999999, new Vector2((float) Game1.viewport.Width, (float) Game1.viewport.Height * 0.33f + (float) Game1.random.Next((int) sbyte.MinValue, 128 /*0x80*/)), false, false, 1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
                  {
                    motion = new Vector2(-2f, 0.0f),
                    yPeriodic = true,
                    yPeriodicLoopTime = (float) Game1.random.Next(1500, 2000),
                    yPeriodicRange = 32f,
                    local = true,
                    delayBeforeAnimationStart = 24000 + index * 200,
                    startSound = index == 0 ? "yoba" : (string) null
                  });
                int num37 = 0;
                if (Game1.player.mailReceived.Contains("Capsule_Broken"))
                {
                  for (int index = 0; index < 3; ++index)
                    location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(639, 785, 16 /*0x10*/, 16 /*0x10*/), 100f, 4, 999999, new Vector2((float) Game1.viewport.Width, (float) Game1.viewport.Height * 0.33f + (float) Game1.random.Next((int) sbyte.MinValue, 128 /*0x80*/)), false, false, 1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
                    {
                      motion = new Vector2(-2f, 0.0f),
                      yPeriodic = true,
                      yPeriodicLoopTime = (float) Game1.random.Next(1500, 2000),
                      yPeriodicRange = 16f,
                      local = true,
                      delayBeforeAnimationStart = 30000 + index * 500,
                      startSound = index == 0 ? "UFO" : (string) null
                    });
                  num37 += 5000;
                }
                if (Game1.year <= 2)
                {
                  location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors2", new Microsoft.Xna.Framework.Rectangle(150, 259, 9, 9), 10f, 4, 9999999, new Vector2((float) (Game1.viewport.Width + 4), (float) ((double) Game1.viewport.Height * 0.33000001311302185 + 44.0)), false, false, 0.9f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
                  {
                    motion = new Vector2(-2f, 0.0f),
                    yPeriodic = true,
                    yPeriodicLoopTime = 3000f,
                    yPeriodicRange = 8f,
                    delayBeforeAnimationStart = 30000 + num37
                  });
                  location.TemporarySprites.Add(new TemporaryAnimatedSprite("Characters\\KrobusRaven", new Microsoft.Xna.Framework.Rectangle(2, 129, 120, 27), 1090f, 1, 999999, new Vector2((float) Game1.viewport.Width, (float) Game1.viewport.Height * 0.33f), false, false, 0.9f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
                  {
                    motion = new Vector2(-2f, 0.0f),
                    yPeriodic = true,
                    yPeriodicLoopTime = 3000f,
                    yPeriodicRange = 8f,
                    startSound = "discoverMineral",
                    delayBeforeAnimationStart = 30000 + num37
                  });
                  num37 += 5000;
                }
                else if (Game1.year <= 3)
                {
                  location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors2", new Microsoft.Xna.Framework.Rectangle(150, 259, 9, 9), 10f, 4, 9999999, new Vector2((float) (Game1.viewport.Width + 4), (float) ((double) Game1.viewport.Height * 0.33000001311302185 + 44.0)), false, false, 0.9f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
                  {
                    motion = new Vector2(-2f, 0.0f),
                    yPeriodic = true,
                    yPeriodicLoopTime = 3000f,
                    yPeriodicRange = 8f,
                    delayBeforeAnimationStart = 30000 + num37
                  });
                  location.TemporarySprites.Add(new TemporaryAnimatedSprite("Characters\\KrobusRaven", new Microsoft.Xna.Framework.Rectangle(1, 104, 100, 24), 1090f, 1, 999999, new Vector2((float) Game1.viewport.Width, (float) Game1.viewport.Height * 0.33f), false, false, 0.9f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
                  {
                    motion = new Vector2(-2f, 0.0f),
                    yPeriodic = true,
                    yPeriodicLoopTime = 3000f,
                    yPeriodicRange = 8f,
                    startSound = "newArtifact",
                    delayBeforeAnimationStart = 30000 + num37
                  });
                  num37 += 5000;
                }
                if (Game1.MasterPlayer.totalMoneyEarned < 100000000U)
                  return;
                location.TemporarySprites.Add(new TemporaryAnimatedSprite("Characters\\KrobusRaven", new Microsoft.Xna.Framework.Rectangle(125, 108, 34, 50), 1090f, 1, 999999, new Vector2((float) Game1.viewport.Width, (float) Game1.viewport.Height * 0.33f), false, false, 0.9f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
                {
                  motion = new Vector2(-2f, 0.0f),
                  yPeriodic = true,
                  yPeriodicLoopTime = 3000f,
                  yPeriodicRange = 8f,
                  startSound = "discoverMineral",
                  delayBeforeAnimationStart = 30000 + num37
                });
                int num38 = num37 + 5000;
                return;
              default:
                return;
            }
          case 't':
            switch (key)
            {
              case "parrotSlide":
                location.getTemporarySpriteByID(666).yStopCoordinate = 5632;
                location.getTemporarySpriteByID(666).motion.X = 0.0f;
                location.getTemporarySpriteByID(666).motion.Y = 1f;
                return;
              case "parrotSplat":
                this.aboveMapSprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(0, 165, 24, 22), 100f, 6, 9999, new Vector2((float) (Game1.viewport.X + Game1.graphics.GraphicsDevice.Viewport.Width), (float) (Game1.viewport.Y + 64 /*0x40*/)), false, false, 1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
                {
                  id = 999,
                  motion = new Vector2(-2f, 4f),
                  acceleration = new Vector2(-0.1f, 0.0f),
                  delayBeforeAnimationStart = 0,
                  yStopCoordinate = 5568,
                  xStopCoordinate = 1504,
                  reachedStopCoordinate = new TemporaryAnimatedSprite.endBehavior(this.parrotSplat)
                });
                return;
              case "elliottBoat":
                location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(461, 1843, 32 /*0x20*/, 51), 1000f, 2, 9999, new Vector2(15f, 26f) * 64f + new Vector2(-28f, 0.0f), false, false, 0.1664f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f));
                return;
              default:
                return;
            }
          case 'u':
            return;
          case 'v':
            return;
          case 'w':
            if (!(key == "woodswalker"))
              return;
            location.temporarySprites.Add(new TemporaryAnimatedSprite()
            {
              texture = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\temporary_sprites_1"),
              sourceRect = new Microsoft.Xna.Framework.Rectangle(448, 419, 16 /*0x10*/, 21),
              sourceRectStartingPos = new Vector2(448f, 419f),
              animationLength = 4,
              totalNumberOfLoops = 7,
              interval = 150f,
              scale = 4f,
              position = new Vector2(4f, 1f) * 64f + new Vector2(5f, 22f) * 4f,
              shakeIntensity = 1f,
              motion = new Vector2(1f, 0.0f),
              xStopCoordinate = 576,
              layerDepth = 1f,
              id = 996
            });
            return;
          default:
            return;
        }
      case 12:
        switch (key[4])
        {
          case 'C':
            if (!(key == "jojaCeremony"))
              return;
            this.aboveMapSprites = new TemporaryAnimatedSpriteList();
            for (int index = 0; index < 16 /*0x10*/; ++index)
            {
              Vector2 position4 = new Vector2((float) Game1.random.Next(Game1.viewport.Width - 128 /*0x80*/), (float) (Game1.viewport.Height + index * 64 /*0x40*/));
              this.aboveMapSprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(534, 1413, 11, 16 /*0x10*/), 99999f, 1, 99999, position4, false, false, 0.99f, 0.0f, Color.DeepSkyBlue, 4f, 0.0f, 0.0f, 0.0f)
              {
                local = true,
                motion = new Vector2(0.25f, -1.5f),
                acceleration = new Vector2(0.0f, -1f / 1000f),
                id = 79797 + index
              });
              this.aboveMapSprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(545, 1413, 11, 34), 99999f, 1, 99999, position4 + new Vector2(0.0f, 0.0f), false, false, 1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
              {
                local = true,
                motion = new Vector2(0.25f, -1.5f),
                acceleration = new Vector2(0.0f, -1f / 1000f),
                id = 79797 + index
              });
            }
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(0, 1363, 114, 58), 99999f, 1, 99999, new Vector2(50f, 20f) * 64f, false, false, 0.1472f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f));
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(595, 1387, 14, 34), 200f, 3, 99999, new Vector2(48f, 20f) * 64f, false, false, 0.157200009f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
            {
              pingPong = true
            });
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(595, 1387, 14, 34), 200f, 3, 99999, new Vector2(49f, 20f) * 64f, false, false, 0.157200009f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
            {
              pingPong = true
            });
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(595, 1387, 14, 34), 210f, 3, 99999, new Vector2(62f, 20f) * 64f, false, false, 0.157200009f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
            {
              pingPong = true
            });
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(595, 1387, 14, 34), 190f, 3, 99999, new Vector2(60f, 20f) * 64f, false, false, 0.157200009f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
            {
              pingPong = true
            });
            return;
          case 'F':
            if (!(key == "joshFootball"))
              return;
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(405, 1916, 14, 8), 40f, 6, 9999, new Vector2(25f, 16f) * 64f, false, false, 1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
            {
              rotation = -0.7853982f,
              rotationChange = (float) Math.PI / 200f,
              motion = new Vector2(6f, -4f),
              acceleration = new Vector2(0.0f, 0.2f),
              xStopCoordinate = 1856,
              reachedStopCoordinate = new TemporaryAnimatedSprite.endBehavior(this.catchFootball),
              layerDepth = 1f,
              id = 56232
            });
            return;
          case 'M':
            if (!(key == "abbyManyBats"))
              return;
            for (int index = 0; index < 100; ++index)
              location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(640, 1664, 16 /*0x10*/, 16 /*0x10*/), 80f, 4, 9999, new Vector2(23f, 9f) * 64f, false, false, 1f, 3f / 1000f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
              {
                xPeriodic = true,
                xPeriodicLoopTime = (float) Game1.random.Next(1500, 2500),
                xPeriodicRange = (float) Game1.random.Next(64 /*0x40*/, 192 /*0xC0*/),
                motion = new Vector2((float) Game1.random.Next(-2, 3), (float) Game1.random.Next(-8, -4)),
                delayBeforeAnimationStart = index * 30,
                startSound = index % 10 == 0 || Game1.random.NextDouble() < 0.1 ? "batScreech" : (string) null
              });
            for (int index = 0; index < 100; ++index)
              location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(640, 1664, 16 /*0x10*/, 16 /*0x10*/), 80f, 4, 9999, new Vector2(23f, 9f) * 64f, false, false, 1f, 3f / 1000f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
              {
                motion = new Vector2((float) Game1.random.Next(-4, 5), (float) Game1.random.Next(-8, -4)),
                delayBeforeAnimationStart = 10 + index * 30
              });
            return;
          case 'T':
            if (!(key == "maruTrapdoor"))
              return;
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(640, 1632, 16 /*0x10*/, 32 /*0x20*/), 150f, 4, 0, new Vector2(1f, 5f) * 64f, false, false, 1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f));
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(688, 1632, 16 /*0x10*/, 32 /*0x20*/), 99999f, 1, 0, new Vector2(1f, 5f) * 64f, false, false, 0.99f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
            {
              delayBeforeAnimationStart = 500
            });
            return;
          case 'a':
            if (!(key == "curtainClose"))
              return;
            location.getTemporarySpriteByID(999).sourceRect.X = 644;
            Game1.playSound("shwip");
            return;
          case 'b':
            return;
          case 'c':
            return;
          case 'd':
            if (!(key == "grandpaNight"))
              return;
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(0, 1453, 639, 176 /*0xB0*/), 9999f, 1, 999999, new Vector2(0.0f, 1f) * 64f, false, false, 0.9f, 0.0f, Color.Cyan, 4f, 0.0f, 0.0f, 0.0f, true)
            {
              alpha = 0.01f,
              alphaFade = -1f / 500f,
              local = true
            });
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(0, 1453, 639, 176 /*0xB0*/), 9999f, 1, 999999, new Vector2(0.0f, 768f), false, true, 0.9f, 0.0f, Color.Blue, 4f, 0.0f, 0.0f, 0.0f, true)
            {
              alpha = 0.01f,
              alphaFade = -1f / 500f,
              local = true
            });
            return;
          case 'e':
            if (!(key == "marcelloLand"))
              return;
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(0, 1183, 84, 160 /*0xA0*/), 10000f, 1, 99999, (new Vector2(25f, 19f) + this.eventPositionTileOffset) * 64f + new Vector2(-23f, 0.0f) * 4f, false, false, 2E-05f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
            {
              motion = new Vector2(0.0f, 2f),
              yStopCoordinate = (41 + (int) this.eventPositionTileOffset.Y) * 64 /*0x40*/ - 640,
              reachedStopCoordinate = new TemporaryAnimatedSprite.endBehavior(this.marcelloBalloonLand),
              attachedCharacter = (Character) this.getActorByName("Marcello"),
              id = 1
            });
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(84, 1205, 38, 26), 10000f, 1, 99999, (new Vector2(25f, 19f) + this.eventPositionTileOffset) * 64f + new Vector2(0.0f, 134f) * 4f, false, false, (float) ((41.0 + (double) this.eventPositionTileOffset.Y) * 64.0 / 10000.0 + 9.9999997473787516E-05), 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
            {
              motion = new Vector2(0.0f, 2f),
              id = 2
            });
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(24, 1343, 36, 19), 7000f, 1, 99999, (new Vector2(25f, 40f) + this.eventPositionTileOffset) * 64f, false, false, 1E-05f, 0.0f, Color.White, 0.0f, 0.0f, 0.0f, 0.0f)
            {
              scaleChange = 0.01f,
              id = 3
            });
            return;
          case 'i':
            switch (key)
            {
              case "staticSprite":
                string textureName;
                string error4;
                Microsoft.Xna.Framework.Rectangle sourceRect4;
                Vector2 vector2_3;
                int num39;
                float num40;
                if (!ArgUtility.TryGet(args, 2, out textureName, out error4, name: "string textureName") || !ArgUtility.TryGetRectangle(args, 3, out sourceRect4, out error4, "Rectangle sourceRect") || !ArgUtility.TryGetVector2(args, 7, out vector2_3, out error4, name: "Vector2 tile") || !ArgUtility.TryGetOptionalInt(args, 9, out num39, out error4, 999, "int id") || !ArgUtility.TryGetOptionalFloat(args, 10, out num40, out error4, 1f, "float layerDepth"))
                {
                  this.LogCommandError(args, error4);
                  return;
                }
                location.temporarySprites.Add(new TemporaryAnimatedSprite(textureName, sourceRect4, vector2_3 * 64f, false, 0.0f, Color.White)
                {
                  animationLength = 1,
                  interval = 999999f,
                  scale = 4f,
                  layerDepth = num40,
                  id = num39
                });
                return;
              case "morrisFlying":
                location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(105, 1318, 13, 31 /*0x1F*/), 9999f, 1, 99999, new Vector2(32f, 13f) * 64f, false, false, 1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
                {
                  motion = new Vector2(4f, -8f),
                  rotationChange = 0.196349546f,
                  shakeIntensity = 1f
                });
                return;
              default:
                return;
            }
          case 'o':
            if (!(key == "balloonBirds"))
              return;
            int num41;
            string error5;
            if (!ArgUtility.TryGetOptionalInt(args, 2, out num41, out error5, name: "int positionOffset"))
            {
              this.LogCommandError(args, error5);
              return;
            }
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(388, 1894, 24, 22), 100f, 6, 9999, new Vector2(48f, (float) (num41 + 12)) * 64f, false, false, 1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
            {
              motion = new Vector2(-3f, 0.0f),
              delayBeforeAnimationStart = 1500
            });
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(388, 1894, 24, 22), 100f, 6, 9999, new Vector2(47f, (float) (num41 + 13)) * 64f, false, false, 1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
            {
              motion = new Vector2(-3f, 0.0f),
              delayBeforeAnimationStart = 1250
            });
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(388, 1894, 24, 22), 100f, 6, 9999, new Vector2(46f, (float) (num41 + 14)) * 64f, false, false, 1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
            {
              motion = new Vector2(-3f, 0.0f),
              delayBeforeAnimationStart = 1100
            });
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(388, 1894, 24, 22), 100f, 6, 9999, new Vector2(45f, (float) (num41 + 15)) * 64f, false, false, 1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
            {
              motion = new Vector2(-3f, 0.0f),
              delayBeforeAnimationStart = 1000
            });
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(388, 1894, 24, 22), 100f, 6, 9999, new Vector2(46f, (float) (num41 + 16 /*0x10*/)) * 64f, false, false, 1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
            {
              motion = new Vector2(-3f, 0.0f),
              delayBeforeAnimationStart = 1080
            });
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(388, 1894, 24, 22), 100f, 6, 9999, new Vector2(47f, (float) (num41 + 17)) * 64f, false, false, 1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
            {
              motion = new Vector2(-3f, 0.0f),
              delayBeforeAnimationStart = 1300
            });
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(388, 1894, 24, 22), 100f, 6, 9999, new Vector2(48f, (float) (num41 + 18)) * 64f, false, false, 1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
            {
              motion = new Vector2(-3f, 0.0f),
              delayBeforeAnimationStart = 1450
            });
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(388, 1894, 24, 22), 100f, 6, 9999, new Vector2(46f, (float) (num41 + 15)) * 64f, false, false, 1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
            {
              motion = new Vector2(-4f, 0.0f),
              delayBeforeAnimationStart = 5450
            });
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(388, 1894, 24, 22), 100f, 6, 9999, new Vector2(48f, (float) (num41 + 10)) * 64f, false, false, 0.0f, 0.0f, Color.White, 2f, 0.0f, 0.0f, 0.0f)
            {
              motion = new Vector2(-2f, 0.0f),
              delayBeforeAnimationStart = 500
            });
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(388, 1894, 24, 22), 100f, 6, 9999, new Vector2(47f, (float) (num41 + 11)) * 64f, false, false, 0.0f, 0.0f, Color.White, 2f, 0.0f, 0.0f, 0.0f)
            {
              motion = new Vector2(-2f, 0.0f),
              delayBeforeAnimationStart = 250
            });
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(388, 1894, 24, 22), 100f, 6, 9999, new Vector2(46f, (float) (num41 + 12)) * 64f, false, false, 0.0f, 0.0f, Color.White, 2f, 0.0f, 0.0f, 0.0f)
            {
              motion = new Vector2(-2f, 0.0f),
              delayBeforeAnimationStart = 100
            });
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(388, 1894, 24, 22), 100f, 6, 9999, new Vector2(45f, (float) (num41 + 13)) * 64f, false, false, 0.0f, 0.0f, Color.White, 2f, 0.0f, 0.0f, 0.0f)
            {
              motion = new Vector2(-2f, 0.0f)
            });
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(388, 1894, 24, 22), 100f, 6, 9999, new Vector2(46f, (float) (num41 + 14)) * 64f, false, false, 0.0f, 0.0f, Color.White, 2f, 0.0f, 0.0f, 0.0f)
            {
              motion = new Vector2(-2f, 0.0f),
              delayBeforeAnimationStart = 80 /*0x50*/
            });
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(388, 1894, 24, 22), 100f, 6, 9999, new Vector2(47f, (float) (num41 + 15)) * 64f, false, false, 0.0f, 0.0f, Color.White, 2f, 0.0f, 0.0f, 0.0f)
            {
              motion = new Vector2(-2f, 0.0f),
              delayBeforeAnimationStart = 300
            });
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(388, 1894, 24, 22), 100f, 6, 9999, new Vector2(48f, (float) (num41 + 16 /*0x10*/)) * 64f, false, false, 0.0f, 0.0f, Color.White, 2f, 0.0f, 0.0f, 0.0f)
            {
              motion = new Vector2(-2f, 0.0f),
              delayBeforeAnimationStart = 450
            });
            return;
          case 'v':
            if (!(key == "removeSprite"))
              return;
            int id2;
            string error6;
            if (!ArgUtility.TryGetInt(args, 2, out id2, out error6, "int spriteId"))
            {
              this.LogCommandError(args, error6);
              return;
            }
            location.removeTemporarySpritesWithID(id2);
            return;
          case 'y':
            switch (key)
            {
              case "EmilyCamping":
                this.showGroundObjects = false;
                location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(644, 1578, 59, 53), 999999f, 1, 99999, new Vector2(26f, 9f) * 64f + new Vector2(-16f, 0.0f), false, false, 0.0788f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
                {
                  id = 999
                });
                location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(675, 1299, 29, 24), 999999f, 1, 99999, new Vector2(27f, 14f) * 64f, false, false, 1f / 1000f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
                {
                  id = 99
                });
                location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(276, 1985, 12, 11), new Vector2(27f, 14f) * 64f + new Vector2(8f, 4f) * 4f, false, 0.0f, Color.White)
                {
                  interval = 50f,
                  totalNumberOfLoops = 99999,
                  animationLength = 4,
                  lightId = this.GenerateLightSourceId("EmilyCamping_1"),
                  id = 666,
                  lightRadius = 2f,
                  scale = 4f,
                  layerDepth = 0.01f
                });
                Game1.currentLightSources.Add(new LightSource(this.GenerateLightSourceId("EmilyCamping_2"), 4, new Vector2(27f, 14f) * 64f, 2f));
                location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(585, 1846, 26, 22), 999999f, 1, 99999, new Vector2(25f, 12f) * 64f + new Vector2(-32f, 0.0f), false, false, 1f / 1000f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
                {
                  id = 96 /*0x60*/
                });
                AmbientLocationSounds.addSound(new Vector2(27f, 14f), 1);
                return;
              case "EmilyBoomBox":
                location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(586, 1871, 24, 14), 99999f, 1, 99999, new Vector2(15f, 4f) * 64f, false, false, 0.01f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
                {
                  id = 999
                });
                return;
              default:
                return;
            }
          default:
            return;
        }
      case 13:
        switch (key[9])
        {
          case 'D':
            if (!(key == "haleyRoomDark"))
              return;
            Game1.currentLightSources.Clear();
            Game1.ambientLight = new Color(200, 200, 100);
            location.TemporarySprites.Add(new TemporaryAnimatedSprite(743, 999999f, 1, 0, new Vector2(4f, 1f) * 64f, false, false)
            {
              lightId = this.GenerateLightSourceId("haleyRoomDark"),
              lightcolor = new Color(0, (int) byte.MaxValue, (int) byte.MaxValue),
              lightRadius = 2f
            });
            return;
          case 'F':
            if (!(key == "sebastianFrog"))
              return;
            Texture2D texture2D5 = Game1.temporaryContent.Load<Texture2D>("TileSheets\\critters");
            location.TemporarySprites.Add(new TemporaryAnimatedSprite()
            {
              texture = texture2D5,
              sourceRect = new Microsoft.Xna.Framework.Rectangle(0, 224 /*0xE0*/, 16 /*0x10*/, 16 /*0x10*/),
              animationLength = 4,
              sourceRectStartingPos = new Vector2(0.0f, 224f),
              interval = 120f,
              totalNumberOfLoops = 9999,
              position = new Vector2(45f, 36f) * 64f,
              scale = 4f,
              layerDepth = 0.00064f,
              motion = new Vector2(2f, 0.0f),
              xStopCoordinate = 3136,
              id = 777,
              reachedStopCoordinate = (TemporaryAnimatedSprite.endBehavior) (param1 =>
              {
                ++this.CurrentCommand;
                location.removeTemporarySpritesWithID(777);
              })
            });
            return;
          case 'R':
            if (!(key == "sebastianRide"))
              return;
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(405, 1843, 14, 9), 40f, 4, 999, new Vector2(19f, 8f) * 64f + new Vector2(0.0f, 28f), false, false, 0.1792f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
            {
              motion = new Vector2(-2f, 0.0f)
            });
            return;
          case 'S':
            if (!(key == "shakeBushStop"))
              return;
            location.getTemporarySpriteByID(777).shakeIntensity = 0.0f;
            return;
          case 'T':
            switch (key)
            {
              case "trashBearTown":
                this.aboveMapSprites = new TemporaryAnimatedSpriteList();
                this.aboveMapSprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors2", new Microsoft.Xna.Framework.Rectangle(46, 80 /*0x50*/, 46, 56), new Vector2(43f, 64f) * 64f, false, 0.0f, Color.White)
                {
                  animationLength = 1,
                  interval = 999999f,
                  motion = new Vector2(4f, 0.0f),
                  scale = 4f,
                  layerDepth = 1f,
                  yPeriodic = true,
                  yPeriodicLoopTime = 2000f,
                  yPeriodicRange = 32f,
                  id = 777,
                  xStopCoordinate = 3392,
                  reachedStopCoordinate = (TemporaryAnimatedSprite.endBehavior) (param =>
                  {
                    this.aboveMapSprites[0].xStopCoordinate = -1;
                    this.aboveMapSprites[0].motion = new Vector2(4f, 0.0f);
                    location.ApplyMapOverride("Town-TrashGone", destination_rect: new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(57, 68, 17, 5)));
                    location.ApplyMapOverride("Town-DogHouse", destination_rect: new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(51, 65, 5, 6)));
                    Game1.flashAlpha = 0.75f;
                    Game1.screenGlowOnce(Color.Lime, false, 0.25f, 1f);
                    location.playSound("yoba");
                    TemporaryAnimatedSprite temporaryAnimatedSprite20 = new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(497, 1918, 11, 11), new Vector2(3456f, 4160f), false, 0.0f, Color.White)
                    {
                      yStopCoordinate = 4372,
                      motion = new Vector2(-0.5f, -10f),
                      acceleration = new Vector2(0.0f, 0.25f),
                      scale = 4f,
                      alphaFade = 0.0f,
                      extraInfoForEndBehavior = -777
                    };
                    temporaryAnimatedSprite20.reachedStopCoordinate = new TemporaryAnimatedSprite.endBehavior(temporaryAnimatedSprite20.bounce);
                    temporaryAnimatedSprite20.initialPosition.Y = 4372f;
                    this.aboveMapSprites.Add(temporaryAnimatedSprite20);
                    this.aboveMapSprites.AddRange((IEnumerable<TemporaryAnimatedSprite>) Utility.getStarsAndSpirals(location, 54, 69, 6, 5, 1000, 10, Color.Lime));
                    location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(324, 1936, 12, 20), 80f, 4, 99999, new Vector2(53f, 67f) * 64f + new Vector2(3f, 3f) * 4f, false, false, 1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
                    {
                      id = 1,
                      delayBeforeAnimationStart = 3000,
                      startSound = "dogWhining"
                    });
                  })
                });
                return;
              case "stopShakeTent":
                location.getTemporarySpriteByID(999).shakeIntensity = 0.0f;
                return;
              default:
                return;
            }
          case 'U':
            return;
          case 'V':
            return;
          case 'W':
            if (!(key == "haleyCakeWalk"))
              return;
            Texture2D texture2D6 = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\temporary_sprites_1");
            location.TemporarySprites.Add(new TemporaryAnimatedSprite()
            {
              texture = texture2D6,
              sourceRect = new Microsoft.Xna.Framework.Rectangle(0, 400, 144 /*0x90*/, 112 /*0x70*/),
              animationLength = 1,
              sourceRectStartingPos = new Vector2(0.0f, 400f),
              interval = 5000f,
              totalNumberOfLoops = 9999,
              position = new Vector2(26f, 65f) * 64f,
              scale = 4f,
              layerDepth = 0.00064f
            });
            return;
          case 'a':
            if (!(key == "pamYobaStatue"))
              return;
            location.objects.Remove(new Vector2(26f, 9f));
            location.objects.Add(new Vector2(26f, 9f), ItemRegistry.Create<Object>("(BC)34"));
            GameLocation gameLocation = Game1.RequireLocation("Trailer_Big");
            gameLocation.objects.Remove(new Vector2(26f, 9f));
            gameLocation.objects.Add(new Vector2(26f, 9f), ItemRegistry.Create<Object>("(BC)34"));
            return;
          case 'b':
            return;
          case 'c':
            if (!(key == "maruTelescope"))
              return;
            for (int index = 0; index < 9; ++index)
              location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(256 /*0x0100*/, 1680, 16 /*0x10*/, 16 /*0x10*/), 80f, 5, 0, new Vector2((float) Game1.random.Next(1, 28), (float) Game1.random.Next(1, 20)) * 64f, false, false, 1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
              {
                delayBeforeAnimationStart = 8000 + index * Game1.random.Next(2000),
                motion = new Vector2(4f, 4f)
              });
            if (!(this.id == "5183338"))
              return;
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(206, 1827, 15, 27), 80f, 4, 999, new Vector2(-2f, 13f) * 64f, false, false, 1f, 0.0f, Color.White, 4f, 0.0f, 1.2f, 0.0f)
            {
              delayBeforeAnimationStart = 7000,
              motion = new Vector2(2f, -0.5f),
              alpha = 0.01f,
              alphaFade = -0.005f
            });
            return;
          case 'd':
            if (!(key == "skateboardFly"))
              return;
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(388, 1875, 16 /*0x10*/, 6), 9999f, 1, 999, new Vector2(26f, 90f) * 64f, false, false, 1E-05f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
            {
              rotationChange = 0.1308997f,
              motion = new Vector2(-8f, -10f),
              acceleration = new Vector2(0.02f, 0.3f),
              yStopCoordinate = 5824,
              xStopCoordinate = 1024 /*0x0400*/,
              layerDepth = 1f
            });
            return;
          case 'e':
            return;
          case 'f':
            if (!(key == "linusCampfire"))
              return;
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(276, 1985, 12, 11), 50f, 4, 99999, new Vector2(29f, 9f) * 64f + new Vector2(8f, 0.0f), false, false, 0.0576f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
            {
              lightId = this.GenerateLightSourceId("linusCampfire"),
              lightRadius = 3f,
              lightcolor = Color.Black
            });
            return;
          case 'g':
            if (!(key == "alexDiningDog"))
              return;
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(324, 1936, 12, 20), 80f, 4, 99999, new Vector2(7f, 2f) * 64f + new Vector2(2f, -8f) * 4f, false, false, 1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
            {
              id = 1
            });
            return;
          case 'h':
            return;
          case 'i':
            switch (key)
            {
              case "shaneHospital":
                location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(533, 1864, 19, 10), 99999f, 1, 99999, new Vector2(20f, 3f) * 64f + new Vector2(16f, 12f), false, false, 0.01f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
                {
                  id = 999
                });
                return;
              case "grandpaSpirit":
                TemporaryAnimatedSprite temporaryAnimatedSprite21 = new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(555, 1956, 18, 35), 9999f, 1, 99999, new Vector2(-1000f, -1010f) * 64f, false, false, 1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
                {
                  yStopCoordinate = -64128,
                  xPeriodic = true,
                  xPeriodicLoopTime = 3000f,
                  xPeriodicRange = 16f,
                  motion = new Vector2(0.0f, 1f),
                  overrideLocationDestroy = true,
                  id = 77777
                };
                location.temporarySprites.Add(temporaryAnimatedSprite21);
                for (int index = 0; index < 19; ++index)
                  location.temporarySprites.Add(new TemporaryAnimatedSprite(10, new Vector2(32f, 32f), Color.White)
                  {
                    parentSprite = temporaryAnimatedSprite21,
                    delayBeforeAnimationStart = (index + 1) * 500,
                    overrideLocationDestroy = true,
                    scale = 1f,
                    alpha = 1f
                  });
                return;
              default:
                return;
            }
          case 'j':
            return;
          case 'k':
            return;
          case 'l':
            return;
          case 'm':
            if (!(key == "WizardPromise"))
              return;
            Utility.addSprinklesToLocation(location, 16 /*0x10*/, 15, 9, 9, 2000, 50, Color.White);
            return;
          case 'n':
            switch (key)
            {
              case "raccoondance2":
                location.removeTemporarySpritesWithIDLocal(9786);
                TemporaryAnimatedSprite temporarySpriteById2 = location.getTemporarySpriteByID(9785);
                temporarySpriteById2.sourceRect.Y = 64 /*0x40*/;
                temporarySpriteById2.sourceRectStartingPos.Y = 64f;
                temporarySpriteById2.currentParentTileIndex = 0;
                temporarySpriteById2.motion.X = 0.0f;
                temporarySpriteById2.interval *= 2f;
                temporarySpriteById2.timer = 0.0f;
                temporarySpriteById2.sourceRect.X = 0;
                temporarySpriteById2.position.X -= 32f;
                temporarySpriteById2.position.Y += 8f;
                return;
              case "raccoondance1":
                TemporaryAnimatedSprite temporarySpriteById3 = location.getTemporarySpriteByID(9786);
                TemporaryAnimatedSprite temporarySpriteById4 = location.getTemporarySpriteByID(9785);
                temporarySpriteById3.sourceRect.Y = 96 /*0x60*/;
                temporarySpriteById3.sourceRectStartingPos.Y = 96f;
                temporarySpriteById3.currentParentTileIndex = 1;
                temporarySpriteById3.motion.X = 0.07f;
                temporarySpriteById3.timer = 0.0f;
                temporarySpriteById4.sourceRect.Y = 32 /*0x20*/;
                temporarySpriteById4.sourceRectStartingPos.Y = 32f;
                temporarySpriteById4.currentParentTileIndex = 1;
                temporarySpriteById4.motion.X = -0.07f;
                temporarySpriteById4.timer = 0.0f;
                return;
              default:
                return;
            }
          case 'o':
            return;
          case 'p':
            if (!(key == "EmilySleeping"))
              return;
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(574, 1892, 11, 11), 1000f, 2, 99999, new Vector2(20f, 3f) * 64f + new Vector2(8f, 32f), false, false, 1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
            {
              id = 999
            });
            return;
          case 'q':
            return;
          case 'r':
            if (!(key == "raccoonCircle"))
              return;
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("Characters\\raccoon", new Microsoft.Xna.Framework.Rectangle(0, 0, 32 /*0x20*/, 32 /*0x20*/), 148f, 8, 999, new Vector2(54.5f, 7f) * 64f, false, false)
            {
              scale = 4f,
              layerDepth = 0.0518400036f,
              usePreciseTiming = true,
              id = 9786
            });
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("Characters\\mrs_raccoon", new Microsoft.Xna.Framework.Rectangle(0, 0, 32 /*0x20*/, 32 /*0x20*/), 148f, 8, 999, new Vector2(56.5f, 7f) * 64f, false, false)
            {
              scale = 4f,
              layerDepth = 0.0512f,
              usePreciseTiming = true,
              id = 9785
            });
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\raccoon_circle_cutout", new Microsoft.Xna.Framework.Rectangle(0, 0, 1, 1), Vector2.Zero, false, 0.0f, Color.White)
            {
              drawAboveAlwaysFront = true,
              vectorScale = new Vector2(3090f, 1052f),
              interval = 99999f,
              totalNumberOfLoops = 1,
              id = 997799
            });
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\raccoon_circle_cutout", new Microsoft.Xna.Framework.Rectangle(0, 0, 1, 1), new Vector2(56.5f, 0.0f) * 64f + new Vector2(131.5f, 0.0f) * 4f, false, 0.0f, Color.White)
            {
              drawAboveAlwaysFront = true,
              vectorScale = new Vector2(5536f, 1052f),
              interval = 99999f,
              totalNumberOfLoops = 1,
              id = 997799
            });
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\raccoon_circle_cutout", new Microsoft.Xna.Framework.Rectangle(0, 0, 1, 1), new Vector2(0.0f, 876f), false, 0.0f, Color.White)
            {
              drawAboveAlwaysFront = true,
              vectorScale = new Vector2(7552f, 7488f),
              interval = 99999f,
              totalNumberOfLoops = 1,
              id = 997799
            });
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\raccoon_circle_cutout", new Microsoft.Xna.Framework.Rectangle(0, 0, 263, 263), new Vector2(56.5f, 0.0f) * 64f - new Vector2(131.5f, 44f) * 4f, false, 0.0f, Color.Black)
            {
              drawAboveAlwaysFront = true,
              interval = 297f,
              animationLength = 3,
              totalNumberOfLoops = 99999,
              id = 997799,
              scale = 4f
            });
            return;
          case 's':
            return;
          case 't':
            if (!(key == "ccCelebration"))
              return;
            this.aboveMapSprites = new TemporaryAnimatedSpriteList();
            for (int index = 0; index < 32 /*0x20*/; ++index)
            {
              Vector2 position5 = new Vector2((float) Game1.random.Next(Game1.viewport.Width - 128 /*0x80*/), (float) (Game1.viewport.Height + index * 64 /*0x40*/));
              this.aboveMapSprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(534, 1413, 11, 16 /*0x10*/), 99999f, 1, 99999, position5, false, false, 1f, 0.0f, Utility.getRandomRainbowColor(), 4f, 0.0f, 0.0f, 0.0f)
              {
                local = true,
                motion = new Vector2(0.25f, -1.5f),
                acceleration = new Vector2(0.0f, -1f / 1000f),
                id = 79797 + index
              });
              this.aboveMapSprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(545, 1413, 11, 34), 99999f, 1, 99999, position5 + new Vector2(0.0f, 0.0f), false, false, 1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
              {
                local = true,
                motion = new Vector2(0.25f, -1.5f),
                acceleration = new Vector2(0.0f, -1f / 1000f),
                id = 79797 + index
              });
            }
            if (Game1.IsWinter)
            {
              location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\marnie_winter_dance", new Microsoft.Xna.Framework.Rectangle(0, 0, 20, 26), 400f, 3, 99999, new Vector2(53f, 21f) * 64f, false, false, 0.5f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
              {
                pingPong = true
              });
              return;
            }
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(558, 1425, 20, 26), 400f, 3, 99999, new Vector2(53f, 21f) * 64f, false, false, 0.5f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
            {
              pingPong = true
            });
            return;
          case 'u':
            return;
          case 'v':
            return;
          case 'w':
            if (!(key == "shaneThrowCan"))
              return;
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(542, 1893, 4, 6), 99999f, 1, 99999, new Vector2(103f, 95f) * 64f + new Vector2(0.0f, 4f) * 4f, false, false, 1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
            {
              motion = new Vector2(0.0f, -4f),
              acceleration = new Vector2(0.0f, 0.25f),
              rotationChange = (float) Math.PI / 128f
            });
            Game1.playSound("shwip");
            return;
          case 'x':
            return;
          case 'y':
            if (!(key == "abbyGraveyard"))
              return;
            location.TemporarySprites.Add(new TemporaryAnimatedSprite(736, 999999f, 1, 0, new Vector2(48f, 86f) * 64f, false, false));
            return;
          default:
            return;
        }
      case 14:
        switch (key[6])
        {
          case 'B':
            if (!(key == "candleBoatMove"))
              return;
            this.showGroundObjects = false;
            location.getTemporarySpriteByID(1).motion = new Vector2(0.0f, 2f);
            return;
          case 'C':
            if (!(key == "junimoCageGone"))
              return;
            location.removeTemporarySpritesWithID(1);
            return;
          case 'D':
          case 'E':
          case 'F':
          case 'H':
          case 'I':
          case 'J':
          case 'K':
            return;
          case 'G':
            if (!(key == "secretGiftOpen"))
              return;
            TemporaryAnimatedSprite temporarySpriteById5 = location.getTemporarySpriteByID(666);
            if (temporarySpriteById5 == null)
              return;
            temporarySpriteById5.animationLength = 6;
            temporarySpriteById5.interval = 100f;
            temporarySpriteById5.totalNumberOfLoops = 1;
            temporarySpriteById5.timer = 0.0f;
            temporarySpriteById5.holdLastFrame = true;
            return;
          case 'L':
            if (!(key == "georgeLeekGift"))
              return;
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(288, 1231, 16 /*0x10*/, 16 /*0x10*/), 100f, 6, 1, new Vector2(17f, 19f) * 64f, false, false, 0.01f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
            {
              id = 999,
              paused = false,
              holdLastFrame = true
            });
            return;
          case 'P':
            if (!(key == "parrotPerchHut"))
              return;
            location.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\parrots", new Microsoft.Xna.Framework.Rectangle(0, 0, 24, 24), new Vector2(7f, 4f) * 64f, false, 0.0f, Color.White)
            {
              animationLength = 1,
              interval = 999999f,
              scale = 4f,
              layerDepth = 1f,
              id = 999
            });
            return;
          case 'a':
            if (!(key == "shanePassedOut"))
              return;
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(533, 1864, 19, 27), 99999f, 1, 99999, new Vector2(25f, 7f) * 64f, false, false, 0.01f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
            {
              id = 999
            });
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(552, 1862, 31 /*0x1F*/, 21), 99999f, 1, 99999, new Vector2(25f, 7f) * 64f + new Vector2(-16f, 0.0f), false, false, 0.0001f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f));
            return;
          case 'e':
            if (!(key == "trashBearMagic"))
              return;
            Utility.addStarsAndSpirals(location, 95, 103, 24, 12, 2000, 10, Color.Lime);
            (location as Forest).removeSewerTrash();
            Game1.flashAlpha = 0.75f;
            Game1.screenGlowOnce(Color.Lime, false, 0.25f, 1f);
            return;
          case 'f':
            return;
          case 'g':
            return;
          case 'h':
            if (!(key == "waterShaneDone"))
              return;
            this.farmer.completelyStopAnimatingOrDoingAction();
            this.farmer.TemporaryItem = (Item) null;
            this.drawTool = false;
            location.removeTemporarySpritesWithID(999);
            return;
          case 'i':
            if (!(key == "pennyFieldTrip"))
              return;
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(0, 1813, 86, 54), 999999f, 1, 0, new Vector2(68f, 44f) * 64f, false, false, 0.0001f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f));
            return;
          case 'l':
            if (!(key == "gridballGameTV"))
              return;
            Texture2D texture2D7 = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\temporary_sprites_1");
            location.TemporarySprites.Add(new TemporaryAnimatedSprite()
            {
              texture = texture2D7,
              sourceRect = new Microsoft.Xna.Framework.Rectangle(368, 336, 19, 14),
              animationLength = 7,
              sourceRectStartingPos = new Vector2(368f, 336f),
              interval = 5000f,
              totalNumberOfLoops = 99999,
              position = new Vector2(34f, 3f) * 64f + new Vector2(7f, 13f) * 4f,
              scale = 4f,
              layerDepth = 1f
            });
            return;
          case 'n':
            if (!(key == "raccoonCircle2"))
              return;
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\raccoon_circle_cutout", new Microsoft.Xna.Framework.Rectangle(0, 0, 263, 263), new Vector2(56.5f, 0.0f) * 64f - new Vector2(131.5f, 44f) * 4f, false, 0.0f, Color.White)
            {
              drawAboveAlwaysFront = true,
              interval = 297f,
              animationLength = 3,
              totalNumberOfLoops = 99999,
              id = 997797,
              scale = 4f,
              alpha = 0.01f,
              alphaFade = -3f / 1000f,
              layerDepth = 0.8f
            });
            return;
          default:
            return;
        }
      case 15:
        switch (key[10])
        {
          case 'C':
            if (!(key == "iceFishingCatch"))
              return;
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(160 /*0xA0*/, 368, 16 /*0x10*/, 32 /*0x20*/), 500f, 3, 99999, new Vector2(68f, 30f) * 64f, false, false, 0.1984f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f));
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(160 /*0xA0*/, 368, 16 /*0x10*/, 32 /*0x20*/), 510f, 3, 99999, new Vector2(74f, 30f) * 64f, false, false, 0.1984f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f));
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(160 /*0xA0*/, 368, 16 /*0x10*/, 32 /*0x20*/), 490f, 3, 99999, new Vector2(67f, 36f) * 64f, false, false, 148f / 625f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f));
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(160 /*0xA0*/, 368, 16 /*0x10*/, 32 /*0x20*/), 500f, 3, 99999, new Vector2(76f, 35f) * 64f, false, false, 0.2304f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f));
            return;
          case 'G':
            if (!(key == "junimoCageGone2"))
              return;
            location.removeTemporarySpritesWithID(1);
            Game1.viewportFreeze = true;
            Game1.viewport.X = -1000;
            Game1.viewport.Y = -1000;
            return;
          case 'L':
            if (!(key == "BoatParrotLeave"))
              return;
            TemporaryAnimatedSprite aboveMapSprite3 = this.aboveMapSprites[0];
            aboveMapSprite3.motion = new Vector2(4f, -6f);
            aboveMapSprite3.sourceRect.X = 48 /*0x30*/;
            aboveMapSprite3.sourceRectStartingPos.X = 48f;
            aboveMapSprite3.animationLength = 3;
            aboveMapSprite3.pingPong = true;
            return;
          case 'P':
            if (!(key == "shaneCliffProps"))
              return;
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(549, 1891, 19, 12), 99999f, 1, 99999, new Vector2(104f, 96f) * 64f, false, false, 0.01f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
            {
              id = 999
            });
            return;
          case 'a':
            if (!(key == "sebastianGarage"))
              return;
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(276, 1843, 48 /*0x30*/, 42), 9999f, 1, 999, new Vector2(17f, 23f) * 64f + new Vector2(0.0f, 8f), false, false, 0.1472f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f));
            this.getActorByName("Sebastian").HideShadow = true;
            return;
          case 'b':
            return;
          case 'c':
            if (!(key == "abbyvideoscreen"))
              return;
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(167, 1714, 19, 14), 100f, 3, 9999, new Vector2(2f, 3f) * 64f + new Vector2(7f, 12f) * 4f, false, false, 0.0002f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f));
            return;
          case 'd':
            return;
          case 'e':
            switch (key)
            {
              case "harveyDinnerSet":
                Vector2 vector2_4 = new Vector2(5f, 16f);
                if (location is DecoratableLocation decoratableLocation)
                {
                  foreach (Furniture furniture in decoratableLocation.furniture)
                  {
                    if (furniture.furniture_type.Value == 14 && !location.hasTileAt((int) furniture.tileLocation.X, (int) furniture.tileLocation.Y + 1, "Buildings") && !location.hasTileAt((int) furniture.tileLocation.X + 1, (int) furniture.tileLocation.Y + 1, "Buildings") && !location.hasTileAt((int) furniture.tileLocation.X + 2, (int) furniture.tileLocation.Y + 1, "Buildings") && !location.hasTileAt((int) furniture.tileLocation.X - 1, (int) furniture.tileLocation.Y + 1, "Buildings"))
                    {
                      vector2_4 = new Vector2((float) (int) furniture.TileLocation.X, (float) ((int) furniture.TileLocation.Y + 1));
                      furniture.isOn.Value = true;
                      furniture.setFireplace(false);
                      break;
                    }
                  }
                }
                location.TemporarySprites.Clear();
                this.getActorByName("Harvey").setTilePosition((int) vector2_4.X + 2, (int) vector2_4.Y);
                this.getActorByName("Harvey").Position = new Vector2(this.getActorByName("Harvey").Position.X - 32f, this.getActorByName("Harvey").Position.Y);
                this.farmer.Position = new Vector2((float) ((double) vector2_4.X * 64.0 - 32.0), (float) ((double) vector2_4.Y * 64.0 + 32.0));
                Object objectAtTile1 = location.getObjectAtTile((int) vector2_4.X, (int) vector2_4.Y);
                if (objectAtTile1 != null)
                  objectAtTile1.isTemporarilyInvisible = true;
                Object objectAtTile2 = location.getObjectAtTile((int) vector2_4.X + 1, (int) vector2_4.Y);
                if (objectAtTile2 != null)
                  objectAtTile2.isTemporarilyInvisible = true;
                Object objectAtTile3 = location.getObjectAtTile((int) vector2_4.X - 1, (int) vector2_4.Y);
                if (objectAtTile3 != null)
                  objectAtTile3.isTemporarilyInvisible = true;
                Object objectAtTile4 = location.getObjectAtTile((int) vector2_4.X + 2, (int) vector2_4.Y);
                if (objectAtTile4 != null)
                  objectAtTile4.isTemporarilyInvisible = true;
                Texture2D texture2D8 = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\temporary_sprites_1");
                location.TemporarySprites.Add(new TemporaryAnimatedSprite()
                {
                  texture = texture2D8,
                  sourceRect = new Microsoft.Xna.Framework.Rectangle(385, 423, 48 /*0x30*/, 32 /*0x20*/),
                  animationLength = 1,
                  sourceRectStartingPos = new Vector2(385f, 423f),
                  interval = 5000f,
                  totalNumberOfLoops = 9999,
                  position = vector2_4 * 64f + new Vector2(-8f, -16f) * 4f,
                  scale = 4f,
                  layerDepth = (float) (((double) vector2_4.Y + 0.20000000298023224) * 64.0 / 10000.0),
                  lightId = this.GenerateLightSourceId("harveyDinnerSet"),
                  lightRadius = 4f,
                  lightcolor = Color.Black
                });
                List<string> list1 = ((IEnumerable<string>) this.eventCommands).ToList<string>();
                List<string> stringList = list1;
                int index1 = this.CurrentCommand + 1;
                string[] strArray = new string[5]
                {
                  "viewport ",
                  null,
                  null,
                  null,
                  null
                };
                int num42 = (int) vector2_4.X;
                strArray[1] = num42.ToString();
                strArray[2] = " ";
                num42 = (int) vector2_4.Y;
                strArray[3] = num42.ToString();
                strArray[4] = " true";
                string str = string.Concat(strArray);
                stringList.Insert(index1, str);
                this.eventCommands = list1.ToArray();
                return;
              case "ClothingTherapy":
                location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(644, 1405, 28, 46), 999999f, 1, 99999, new Vector2(5f, 6f) * 64f + new Vector2(-32f, -144f), false, false, 0.0424f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
                {
                  id = 999
                });
                return;
              case "getEndSlideshow":
                Summit summit = location as Summit;
                string[] commands = Event.ParseCommands(summit.getEndSlideshow());
                List<string> list2 = ((IEnumerable<string>) this.eventCommands).ToList<string>();
                list2.InsertRange(this.CurrentCommand + 1, (IEnumerable<string>) commands);
                this.eventCommands = list2.ToArray();
                summit.isShowingEndSlideshow = true;
                return;
              default:
                return;
            }
          case 'f':
            return;
          case 'g':
            return;
          case 'h':
            return;
          case 'i':
            return;
          case 'j':
            return;
          case 'k':
            return;
          case 'l':
            if (!(key == "junimoSpotlight"))
              return;
            this.actors[0].drawOnTop = true;
            location.TemporarySprites.Add(new TemporaryAnimatedSprite()
            {
              texture = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\temporary_sprites_1"),
              sourceRect = new Microsoft.Xna.Framework.Rectangle(316, 123, 67, 43),
              sourceRectStartingPos = new Vector2(316f, 123f),
              animationLength = 1,
              interval = 5000f,
              totalNumberOfLoops = 9999,
              scale = 4f,
              position = Utility.getTopLeftPositionForCenteringOnScreen(Game1.viewport, 268, 172, yOffset: -20),
              layerDepth = 0.0001f,
              local = true,
              id = 999
            });
            return;
          case 'm':
            if (!(key == "grandpaThumbsUp"))
              return;
            TemporaryAnimatedSprite temporarySpriteById6 = location.getTemporarySpriteByID(77777);
            temporarySpriteById6.texture = Game1.mouseCursors2;
            temporarySpriteById6.sourceRect = new Microsoft.Xna.Framework.Rectangle(186, 265, 22, 34);
            temporarySpriteById6.sourceRectStartingPos = new Vector2(186f, 265f);
            temporarySpriteById6.yPeriodic = true;
            temporarySpriteById6.yPeriodicLoopTime = 1000f;
            temporarySpriteById6.yPeriodicRange = 16f;
            temporarySpriteById6.xPeriodicLoopTime = 2500f;
            temporarySpriteById6.xPeriodicRange = 16f;
            temporarySpriteById6.initialPosition = temporarySpriteById6.position;
            return;
          case 'n':
            switch (key)
            {
              case "shaneSaloonCola":
                location.TemporarySprites.Add(new TemporaryAnimatedSprite()
                {
                  texture = Game1.mouseCursors,
                  sourceRect = new Microsoft.Xna.Framework.Rectangle(552, 1862, 31 /*0x1F*/, 21),
                  animationLength = 1,
                  sourceRectStartingPos = new Vector2(552f, 1862f),
                  interval = 999999f,
                  totalNumberOfLoops = 99999,
                  position = new Vector2(32f, 17f) * 64f + new Vector2(10f, 3f) * 4f,
                  scale = 4f,
                  layerDepth = 1E-07f
                });
                return;
              case "springOnionPeel":
                TemporaryAnimatedSprite temporarySpriteById7 = location.getTemporarySpriteByID(777);
                temporarySpriteById7.sourceRectStartingPos = new Vector2(144f, 327f);
                temporarySpriteById7.sourceRect = new Microsoft.Xna.Framework.Rectangle(144 /*0x90*/, 327, 112 /*0x70*/, 112 /*0x70*/);
                return;
              case "springOnionDemo":
                Texture2D texture2D9 = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\temporary_sprites_1");
                TemporaryAnimatedSpriteList temporarySprites18 = location.TemporarySprites;
                TemporaryAnimatedSprite temporaryAnimatedSprite22 = new TemporaryAnimatedSprite();
                temporaryAnimatedSprite22.texture = texture2D9;
                temporaryAnimatedSprite22.sourceRect = new Microsoft.Xna.Framework.Rectangle(144 /*0x90*/, 215, 112 /*0x70*/, 112 /*0x70*/);
                temporaryAnimatedSprite22.animationLength = 2;
                temporaryAnimatedSprite22.sourceRectStartingPos = new Vector2(144f, 215f);
                temporaryAnimatedSprite22.interval = 200f;
                temporaryAnimatedSprite22.totalNumberOfLoops = 99999;
                temporaryAnimatedSprite22.id = 777;
                Viewport viewport3 = Game1.graphics.GraphicsDevice.Viewport;
                double x1 = (double) (viewport3.Width / 2 - 264);
                viewport3 = Game1.graphics.GraphicsDevice.Viewport;
                double y1 = (double) (viewport3.Height / 3 - 264);
                temporaryAnimatedSprite22.position = new Vector2((float) x1, (float) y1);
                temporaryAnimatedSprite22.local = true;
                temporaryAnimatedSprite22.scale = 4f;
                temporaryAnimatedSprite22.destroyable = false;
                temporaryAnimatedSprite22.overrideLocationDestroy = true;
                temporarySprites18.Add(temporaryAnimatedSprite22);
                return;
              case "sebastianOnBike":
                location.TemporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(0, 1600, 64 /*0x40*/, 128 /*0x80*/), 80f, 8, 9999, new Vector2(19f, 27f) * 64f + new Vector2(32f, -16f), false, true, 0.1792f, 0.0f, Color.White, 1f, 0.0f, 0.0f, 0.0f));
                location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(405, 1854, 47, 33), 9999f, 1, 999, new Vector2(17f, 27f) * 64f + new Vector2(0.0f, -8f), false, false, 0.1792f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f));
                return;
              default:
                return;
            }
          case 'o':
            if (!(key == "LeoLinusCooking"))
              return;
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("Maps\\springobjects", new Microsoft.Xna.Framework.Rectangle(240 /*0xF0*/, 128 /*0x80*/, 16 /*0x10*/, 16 /*0x10*/), 9999f, 1, 1, new Vector2(29f, 8.5f) * 64f, false, false, 0.01f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
            {
              layerDepth = 1f
            });
            for (int index2 = 0; index2 < 10; ++index2)
              Utility.addSmokePuff(location, new Vector2(29.5f, 8.6f) * 64f, index2 * 500);
            return;
          case 'p':
            return;
          case 'q':
            if (!(key == "parrotHutSquawk"))
              return;
            (location as IslandHut).parrotUpgradePerches[0].timeUntilSqwawk = 1f;
            return;
          case 'r':
            if (!(key == "coldstarMiracle"))
              return;
            MovieData data2;
            if (!MovieTheater.TryGetMovieData("winter_movie_0", out data2))
            {
              Game1.log.Error("Can't find data for movie 'winter_movie_0'.");
              return;
            }
            Microsoft.Xna.Framework.Rectangle sourceRectForScreen2 = MovieTheater.GetSourceRectForScreen(data2.SheetIndex, 9);
            location.temporarySprites.Add(new TemporaryAnimatedSprite()
            {
              texture = Game1.temporaryContent.Load<Texture2D>(data2.Texture ?? "LooseSprites\\Movies"),
              sourceRect = sourceRectForScreen2,
              sourceRectStartingPos = new Vector2((float) sourceRectForScreen2.X, (float) sourceRectForScreen2.Y),
              animationLength = 1,
              totalNumberOfLoops = 1,
              interval = 99999f,
              alpha = 0.01f,
              alphaFade = -0.01f,
              scale = 4f,
              position = new Vector2(4f, 1f) * 64f + new Vector2(3f, 7f) * 4f,
              layerDepth = 0.8535f,
              id = 989
            });
            return;
          case 's':
            if (!(key == "LeoWillyFishing"))
              return;
            for (int index3 = 0; index3 < 20; ++index3)
              location.TemporarySprites.Add(new TemporaryAnimatedSprite(0, new Vector2(42.5f, 38f) * 64f + new Vector2((float) Game1.random.Next(64 /*0x40*/), (float) Game1.random.Next(64 /*0x40*/)), Color.White * 0.7f)
              {
                layerDepth = (float) (1280 /*0x0500*/ + index3) / 10000f,
                delayBeforeAnimationStart = index3 * 150
              });
            return;
          default:
            return;
        }
      case 16 /*0x10*/:
        switch (key[0])
        {
          case 'B':
            if (!(key == "BoatParrotSquawk"))
              return;
            TemporaryAnimatedSprite aboveMapSprite4 = this.aboveMapSprites[0];
            aboveMapSprite4.sourceRect.X = 24;
            aboveMapSprite4.sourceRectStartingPos.X = 24f;
            Game1.playSound("parrot_squawk");
            return;
          case 'E':
            if (!(key == "EmilyBoomBoxStop"))
              return;
            location.getTemporarySpriteByID(999).pulse = false;
            location.getTemporarySpriteByID(999).scale = 4f;
            return;
          case 'a':
            if (!(key == "abbyOuijaCandles"))
              return;
            location.TemporarySprites.Add(new TemporaryAnimatedSprite(737, 999999f, 1, 0, new Vector2(5f, 9f) * 64f, false, false)
            {
              lightId = this.GenerateLightSourceId("abbyOuijaCandles_1"),
              lightRadius = 1f
            });
            location.TemporarySprites.Add(new TemporaryAnimatedSprite(737, 999999f, 1, 0, new Vector2(7f, 8f) * 64f, false, false)
            {
              lightId = this.GenerateLightSourceId("abbyOuijaCandles_2"),
              lightRadius = 1f
            });
            return;
          case 'i':
            if (!(key == "islandFishSplash"))
              return;
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("Maps\\springobjects", new Microsoft.Xna.Framework.Rectangle(336, 544, 16 /*0x10*/, 16 /*0x10*/), 100000f, 1, 1, new Vector2(81f, 92f) * 64f, false, false, 0.01f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
            {
              id = 9999,
              motion = new Vector2(-2f, -8f),
              acceleration = new Vector2(0.0f, 0.2f),
              flipped = true,
              rotationChange = -0.02f,
              yStopCoordinate = 5952,
              layerDepth = 0.99f,
              reachedStopCoordinate = (TemporaryAnimatedSprite.endBehavior) (param1 =>
              {
                location.TemporarySprites.Add(new TemporaryAnimatedSprite("Maps\\springobjects", new Microsoft.Xna.Framework.Rectangle(48 /*0x30*/, 16 /*0x10*/, 16 /*0x10*/, 16 /*0x10*/), 100f, 5, 1, location.getTemporarySpriteByID(9999).position, false, false, 0.01f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
                {
                  layerDepth = 1f
                });
                location.removeTemporarySpritesWithID(9999);
                Game1.playSound("waterSlosh");
              })
            });
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("Maps\\springobjects", new Microsoft.Xna.Framework.Rectangle(48 /*0x30*/, 16 /*0x10*/, 16 /*0x10*/, 16 /*0x10*/), 100f, 5, 1, new Vector2(81f, 92f) * 64f, false, false, 0.01f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
            {
              layerDepth = 1f
            });
            return;
          case 'j':
            return;
          case 'k':
            return;
          case 'l':
            if (!(key == "leahHoldPainting"))
              return;
            Texture2D texture2D10 = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\temporary_sprites_1");
            location.getTemporarySpriteByID(999).sourceRect.X += 15;
            location.getTemporarySpriteByID(999).sourceRectStartingPos.X += 15f;
            int num43 = Game1.netWorldState.Value.hasWorldStateID("m_painting0") ? 0 : (Game1.netWorldState.Value.hasWorldStateID("m_painting1") ? 1 : 2);
            location.TemporarySprites.Add(new TemporaryAnimatedSprite()
            {
              texture = texture2D10,
              sourceRect = new Microsoft.Xna.Framework.Rectangle(400 + num43 * 25, 394, 25, 23),
              animationLength = 1,
              sourceRectStartingPos = new Vector2((float) (400 + num43 * 25), 394f),
              interval = 5000f,
              totalNumberOfLoops = 9999,
              position = new Vector2(73f, 38f) * 64f + new Vector2(-2f, -16f) * 4f,
              scale = 4f,
              layerDepth = 1f,
              id = 777
            });
            return;
          case 'm':
            if (!(key == "moonlightJellies"))
              return;
            int num44 = 1;
            this.showGroundObjects = false;
            this.npcControllers?.Clear();
            TemporaryAnimatedSpriteList animatedSpriteList2 = new TemporaryAnimatedSpriteList();
            TemporaryAnimatedSprite temporaryAnimatedSprite23 = new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(256 /*0x0100*/, 16 /*0x10*/, 16 /*0x10*/, 16 /*0x10*/), 250f, 3, 9999, new Vector2(26f, 49f) * 64f, false, false, 0.1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f);
            temporaryAnimatedSprite23.motion = new Vector2(0.0f, -1f);
            temporaryAnimatedSprite23.xPeriodic = true;
            temporaryAnimatedSprite23.xPeriodicLoopTime = 3000f;
            temporaryAnimatedSprite23.xPeriodicRange = 16f;
            DefaultInterpolatedStringHandler interpolatedStringHandler2 = new DefaultInterpolatedStringHandler(17, 1);
            interpolatedStringHandler2.AppendLiteral("moonlightJellies_");
            ref DefaultInterpolatedStringHandler local18 = ref interpolatedStringHandler2;
            int num45 = num44;
            int num46 = num45 + 1;
            local18.AppendFormatted<int>(num45);
            temporaryAnimatedSprite23.lightId = this.GenerateLightSourceId(interpolatedStringHandler2.ToStringAndClear());
            temporaryAnimatedSprite23.lightcolor = Color.Black;
            temporaryAnimatedSprite23.lightRadius = 1f;
            temporaryAnimatedSprite23.yStopCoordinate = 2560 /*0x0A00*/;
            temporaryAnimatedSprite23.delayBeforeAnimationStart = 10000;
            temporaryAnimatedSprite23.pingPong = true;
            animatedSpriteList2.Add(temporaryAnimatedSprite23);
            TemporaryAnimatedSprite temporaryAnimatedSprite24 = new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(256 /*0x0100*/, 16 /*0x10*/, 16 /*0x10*/, 16 /*0x10*/), 250f, 3, 9999, new Vector2(29f, 49f) * 64f, false, false, 0.1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f);
            temporaryAnimatedSprite24.motion = new Vector2(0.0f, -1f);
            temporaryAnimatedSprite24.xPeriodic = true;
            temporaryAnimatedSprite24.xPeriodicLoopTime = 3000f;
            temporaryAnimatedSprite24.xPeriodicRange = 16f;
            interpolatedStringHandler2 = new DefaultInterpolatedStringHandler(17, 1);
            interpolatedStringHandler2.AppendLiteral("moonlightJellies_");
            ref DefaultInterpolatedStringHandler local19 = ref interpolatedStringHandler2;
            int num47 = num46;
            int num48 = num47 + 1;
            local19.AppendFormatted<int>(num47);
            temporaryAnimatedSprite24.lightId = this.GenerateLightSourceId(interpolatedStringHandler2.ToStringAndClear());
            temporaryAnimatedSprite24.lightcolor = Color.Black;
            temporaryAnimatedSprite24.lightRadius = 1f;
            temporaryAnimatedSprite24.yStopCoordinate = 2560 /*0x0A00*/;
            temporaryAnimatedSprite24.pingPong = true;
            animatedSpriteList2.Add(temporaryAnimatedSprite24);
            TemporaryAnimatedSprite temporaryAnimatedSprite25 = new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(256 /*0x0100*/, 16 /*0x10*/, 16 /*0x10*/, 16 /*0x10*/), 250f, 3, 9999, new Vector2(31f, 49f) * 64f, false, false, 0.1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f);
            temporaryAnimatedSprite25.motion = new Vector2(0.0f, -1f);
            temporaryAnimatedSprite25.xPeriodic = true;
            temporaryAnimatedSprite25.xPeriodicLoopTime = 3000f;
            temporaryAnimatedSprite25.xPeriodicRange = 16f;
            interpolatedStringHandler2 = new DefaultInterpolatedStringHandler(17, 1);
            interpolatedStringHandler2.AppendLiteral("moonlightJellies_");
            ref DefaultInterpolatedStringHandler local20 = ref interpolatedStringHandler2;
            int num49 = num48;
            int num50 = num49 + 1;
            local20.AppendFormatted<int>(num49);
            temporaryAnimatedSprite25.lightId = this.GenerateLightSourceId(interpolatedStringHandler2.ToStringAndClear());
            temporaryAnimatedSprite25.lightcolor = Color.Black;
            temporaryAnimatedSprite25.lightRadius = 1f;
            temporaryAnimatedSprite25.yStopCoordinate = 2624;
            temporaryAnimatedSprite25.delayBeforeAnimationStart = 12000;
            temporaryAnimatedSprite25.pingPong = true;
            animatedSpriteList2.Add(temporaryAnimatedSprite25);
            TemporaryAnimatedSprite temporaryAnimatedSprite26 = new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(256 /*0x0100*/, 16 /*0x10*/, 16 /*0x10*/, 16 /*0x10*/), 250f, 3, 9999, new Vector2(20f, 49f) * 64f, false, false, 0.1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f);
            temporaryAnimatedSprite26.motion = new Vector2(0.0f, -1f);
            temporaryAnimatedSprite26.xPeriodic = true;
            temporaryAnimatedSprite26.xPeriodicLoopTime = 3000f;
            temporaryAnimatedSprite26.xPeriodicRange = 16f;
            interpolatedStringHandler2 = new DefaultInterpolatedStringHandler(17, 1);
            interpolatedStringHandler2.AppendLiteral("moonlightJellies_");
            ref DefaultInterpolatedStringHandler local21 = ref interpolatedStringHandler2;
            int num51 = num50;
            int num52 = num51 + 1;
            local21.AppendFormatted<int>(num51);
            temporaryAnimatedSprite26.lightId = this.GenerateLightSourceId(interpolatedStringHandler2.ToStringAndClear());
            temporaryAnimatedSprite26.lightcolor = Color.Black;
            temporaryAnimatedSprite26.lightRadius = 1f;
            temporaryAnimatedSprite26.yStopCoordinate = 1728;
            temporaryAnimatedSprite26.delayBeforeAnimationStart = 14000;
            temporaryAnimatedSprite26.pingPong = true;
            animatedSpriteList2.Add(temporaryAnimatedSprite26);
            TemporaryAnimatedSprite temporaryAnimatedSprite27 = new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(256 /*0x0100*/, 16 /*0x10*/, 16 /*0x10*/, 16 /*0x10*/), 250f, 3, 9999, new Vector2(17f, 49f) * 64f, false, false, 0.1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f);
            temporaryAnimatedSprite27.motion = new Vector2(0.0f, -1f);
            temporaryAnimatedSprite27.xPeriodic = true;
            temporaryAnimatedSprite27.xPeriodicLoopTime = 3000f;
            temporaryAnimatedSprite27.xPeriodicRange = 16f;
            interpolatedStringHandler2 = new DefaultInterpolatedStringHandler(17, 1);
            interpolatedStringHandler2.AppendLiteral("moonlightJellies_");
            ref DefaultInterpolatedStringHandler local22 = ref interpolatedStringHandler2;
            int num53 = num52;
            int num54 = num53 + 1;
            local22.AppendFormatted<int>(num53);
            temporaryAnimatedSprite27.lightId = this.GenerateLightSourceId(interpolatedStringHandler2.ToStringAndClear());
            temporaryAnimatedSprite27.lightcolor = Color.Black;
            temporaryAnimatedSprite27.lightRadius = 1f;
            temporaryAnimatedSprite27.yStopCoordinate = 1856;
            temporaryAnimatedSprite27.delayBeforeAnimationStart = 19500;
            temporaryAnimatedSprite27.pingPong = true;
            animatedSpriteList2.Add(temporaryAnimatedSprite27);
            TemporaryAnimatedSprite temporaryAnimatedSprite28 = new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(256 /*0x0100*/, 16 /*0x10*/, 16 /*0x10*/, 16 /*0x10*/), 250f, 3, 9999, new Vector2(16f, 49f) * 64f, false, false, 0.1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f);
            temporaryAnimatedSprite28.motion = new Vector2(0.0f, -1f);
            temporaryAnimatedSprite28.xPeriodic = true;
            temporaryAnimatedSprite28.xPeriodicLoopTime = 3000f;
            temporaryAnimatedSprite28.xPeriodicRange = 16f;
            interpolatedStringHandler2 = new DefaultInterpolatedStringHandler(17, 1);
            interpolatedStringHandler2.AppendLiteral("moonlightJellies_");
            ref DefaultInterpolatedStringHandler local23 = ref interpolatedStringHandler2;
            int num55 = num54;
            int num56 = num55 + 1;
            local23.AppendFormatted<int>(num55);
            temporaryAnimatedSprite28.lightId = this.GenerateLightSourceId(interpolatedStringHandler2.ToStringAndClear());
            temporaryAnimatedSprite28.lightcolor = Color.Black;
            temporaryAnimatedSprite28.lightRadius = 1f;
            temporaryAnimatedSprite28.yStopCoordinate = 2048 /*0x0800*/;
            temporaryAnimatedSprite28.delayBeforeAnimationStart = 20300;
            temporaryAnimatedSprite28.pingPong = true;
            animatedSpriteList2.Add(temporaryAnimatedSprite28);
            TemporaryAnimatedSprite temporaryAnimatedSprite29 = new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(256 /*0x0100*/, 16 /*0x10*/, 16 /*0x10*/, 16 /*0x10*/), 250f, 3, 9999, new Vector2(17f, 49f) * 64f, false, false, 0.1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f);
            temporaryAnimatedSprite29.motion = new Vector2(0.0f, -1f);
            temporaryAnimatedSprite29.xPeriodic = true;
            temporaryAnimatedSprite29.xPeriodicLoopTime = 3000f;
            temporaryAnimatedSprite29.xPeriodicRange = 16f;
            interpolatedStringHandler2 = new DefaultInterpolatedStringHandler(17, 1);
            interpolatedStringHandler2.AppendLiteral("moonlightJellies_");
            ref DefaultInterpolatedStringHandler local24 = ref interpolatedStringHandler2;
            int num57 = num56;
            int num58 = num57 + 1;
            local24.AppendFormatted<int>(num57);
            temporaryAnimatedSprite29.lightId = this.GenerateLightSourceId(interpolatedStringHandler2.ToStringAndClear());
            temporaryAnimatedSprite29.lightcolor = Color.Black;
            temporaryAnimatedSprite29.lightRadius = 1f;
            temporaryAnimatedSprite29.yStopCoordinate = 2496;
            temporaryAnimatedSprite29.delayBeforeAnimationStart = 21500;
            temporaryAnimatedSprite29.pingPong = true;
            animatedSpriteList2.Add(temporaryAnimatedSprite29);
            TemporaryAnimatedSprite temporaryAnimatedSprite30 = new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(256 /*0x0100*/, 16 /*0x10*/, 16 /*0x10*/, 16 /*0x10*/), 250f, 3, 9999, new Vector2(16f, 49f) * 64f, false, false, 0.1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f);
            temporaryAnimatedSprite30.motion = new Vector2(0.0f, -1f);
            temporaryAnimatedSprite30.xPeriodic = true;
            temporaryAnimatedSprite30.xPeriodicLoopTime = 3000f;
            temporaryAnimatedSprite30.xPeriodicRange = 16f;
            interpolatedStringHandler2 = new DefaultInterpolatedStringHandler(17, 1);
            interpolatedStringHandler2.AppendLiteral("moonlightJellies_");
            ref DefaultInterpolatedStringHandler local25 = ref interpolatedStringHandler2;
            int num59 = num58;
            int num60 = num59 + 1;
            local25.AppendFormatted<int>(num59);
            temporaryAnimatedSprite30.lightId = this.GenerateLightSourceId(interpolatedStringHandler2.ToStringAndClear());
            temporaryAnimatedSprite30.lightcolor = Color.Black;
            temporaryAnimatedSprite30.lightRadius = 1f;
            temporaryAnimatedSprite30.yStopCoordinate = 2816 /*0x0B00*/;
            temporaryAnimatedSprite30.delayBeforeAnimationStart = 22400;
            temporaryAnimatedSprite30.pingPong = true;
            animatedSpriteList2.Add(temporaryAnimatedSprite30);
            TemporaryAnimatedSprite temporaryAnimatedSprite31 = new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(256 /*0x0100*/, 16 /*0x10*/, 16 /*0x10*/, 16 /*0x10*/), 250f, 3, 9999, new Vector2(12f, 49f) * 64f, false, false, 0.1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f);
            temporaryAnimatedSprite31.motion = new Vector2(0.0f, -1f);
            temporaryAnimatedSprite31.xPeriodic = true;
            temporaryAnimatedSprite31.xPeriodicLoopTime = 3000f;
            temporaryAnimatedSprite31.xPeriodicRange = 16f;
            interpolatedStringHandler2 = new DefaultInterpolatedStringHandler(17, 1);
            interpolatedStringHandler2.AppendLiteral("moonlightJellies_");
            ref DefaultInterpolatedStringHandler local26 = ref interpolatedStringHandler2;
            int num61 = num60;
            int num62 = num61 + 1;
            local26.AppendFormatted<int>(num61);
            temporaryAnimatedSprite31.lightId = this.GenerateLightSourceId(interpolatedStringHandler2.ToStringAndClear());
            temporaryAnimatedSprite31.lightcolor = Color.Black;
            temporaryAnimatedSprite31.lightRadius = 1f;
            temporaryAnimatedSprite31.yStopCoordinate = 2688;
            temporaryAnimatedSprite31.delayBeforeAnimationStart = 23200;
            temporaryAnimatedSprite31.pingPong = true;
            animatedSpriteList2.Add(temporaryAnimatedSprite31);
            TemporaryAnimatedSprite temporaryAnimatedSprite32 = new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(256 /*0x0100*/, 16 /*0x10*/, 16 /*0x10*/, 16 /*0x10*/), 250f, 3, 9999, new Vector2(9f, 49f) * 64f, false, false, 0.1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f);
            temporaryAnimatedSprite32.motion = new Vector2(0.0f, -1f);
            temporaryAnimatedSprite32.xPeriodic = true;
            temporaryAnimatedSprite32.xPeriodicLoopTime = 3000f;
            temporaryAnimatedSprite32.xPeriodicRange = 16f;
            interpolatedStringHandler2 = new DefaultInterpolatedStringHandler(17, 1);
            interpolatedStringHandler2.AppendLiteral("moonlightJellies_");
            ref DefaultInterpolatedStringHandler local27 = ref interpolatedStringHandler2;
            int num63 = num62;
            int num64 = num63 + 1;
            local27.AppendFormatted<int>(num63);
            temporaryAnimatedSprite32.lightId = this.GenerateLightSourceId(interpolatedStringHandler2.ToStringAndClear());
            temporaryAnimatedSprite32.lightcolor = Color.Black;
            temporaryAnimatedSprite32.lightRadius = 1f;
            temporaryAnimatedSprite32.yStopCoordinate = 2752;
            temporaryAnimatedSprite32.delayBeforeAnimationStart = 24000;
            temporaryAnimatedSprite32.pingPong = true;
            animatedSpriteList2.Add(temporaryAnimatedSprite32);
            TemporaryAnimatedSprite temporaryAnimatedSprite33 = new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(256 /*0x0100*/, 16 /*0x10*/, 16 /*0x10*/, 16 /*0x10*/), 250f, 3, 9999, new Vector2(18f, 49f) * 64f, false, false, 0.1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f);
            temporaryAnimatedSprite33.motion = new Vector2(0.0f, -1f);
            temporaryAnimatedSprite33.xPeriodic = true;
            temporaryAnimatedSprite33.xPeriodicLoopTime = 3000f;
            temporaryAnimatedSprite33.xPeriodicRange = 16f;
            interpolatedStringHandler2 = new DefaultInterpolatedStringHandler(17, 1);
            interpolatedStringHandler2.AppendLiteral("moonlightJellies_");
            ref DefaultInterpolatedStringHandler local28 = ref interpolatedStringHandler2;
            int num65 = num64;
            int num66 = num65 + 1;
            local28.AppendFormatted<int>(num65);
            temporaryAnimatedSprite33.lightId = this.GenerateLightSourceId(interpolatedStringHandler2.ToStringAndClear());
            temporaryAnimatedSprite33.lightcolor = Color.Black;
            temporaryAnimatedSprite33.lightRadius = 1f;
            temporaryAnimatedSprite33.yStopCoordinate = 1920;
            temporaryAnimatedSprite33.delayBeforeAnimationStart = 24600;
            temporaryAnimatedSprite33.pingPong = true;
            animatedSpriteList2.Add(temporaryAnimatedSprite33);
            TemporaryAnimatedSprite temporaryAnimatedSprite34 = new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(256 /*0x0100*/, 16 /*0x10*/, 16 /*0x10*/, 16 /*0x10*/), 250f, 3, 9999, new Vector2(33f, 49f) * 64f, false, false, 0.1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f);
            temporaryAnimatedSprite34.motion = new Vector2(0.0f, -1f);
            temporaryAnimatedSprite34.xPeriodic = true;
            temporaryAnimatedSprite34.xPeriodicLoopTime = 3000f;
            temporaryAnimatedSprite34.xPeriodicRange = 16f;
            interpolatedStringHandler2 = new DefaultInterpolatedStringHandler(17, 1);
            interpolatedStringHandler2.AppendLiteral("moonlightJellies_");
            ref DefaultInterpolatedStringHandler local29 = ref interpolatedStringHandler2;
            int num67 = num66;
            int num68 = num67 + 1;
            local29.AppendFormatted<int>(num67);
            temporaryAnimatedSprite34.lightId = this.GenerateLightSourceId(interpolatedStringHandler2.ToStringAndClear());
            temporaryAnimatedSprite34.lightcolor = Color.Black;
            temporaryAnimatedSprite34.lightRadius = 1f;
            temporaryAnimatedSprite34.yStopCoordinate = 2560 /*0x0A00*/;
            temporaryAnimatedSprite34.delayBeforeAnimationStart = 25600;
            temporaryAnimatedSprite34.pingPong = true;
            animatedSpriteList2.Add(temporaryAnimatedSprite34);
            TemporaryAnimatedSprite temporaryAnimatedSprite35 = new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(256 /*0x0100*/, 16 /*0x10*/, 16 /*0x10*/, 16 /*0x10*/), 250f, 3, 9999, new Vector2(36f, 49f) * 64f, false, false, 0.1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f);
            temporaryAnimatedSprite35.motion = new Vector2(0.0f, -1f);
            temporaryAnimatedSprite35.xPeriodic = true;
            temporaryAnimatedSprite35.xPeriodicLoopTime = 3000f;
            temporaryAnimatedSprite35.xPeriodicRange = 16f;
            interpolatedStringHandler2 = new DefaultInterpolatedStringHandler(17, 1);
            interpolatedStringHandler2.AppendLiteral("moonlightJellies_");
            ref DefaultInterpolatedStringHandler local30 = ref interpolatedStringHandler2;
            int num69 = num68;
            int num70 = num69 + 1;
            local30.AppendFormatted<int>(num69);
            temporaryAnimatedSprite35.lightId = this.GenerateLightSourceId(interpolatedStringHandler2.ToStringAndClear());
            temporaryAnimatedSprite35.lightcolor = Color.Black;
            temporaryAnimatedSprite35.lightRadius = 1f;
            temporaryAnimatedSprite35.yStopCoordinate = 2496;
            temporaryAnimatedSprite35.delayBeforeAnimationStart = 26900;
            temporaryAnimatedSprite35.pingPong = true;
            animatedSpriteList2.Add(temporaryAnimatedSprite35);
            TemporaryAnimatedSprite temporaryAnimatedSprite36 = new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(304, 16 /*0x10*/, 16 /*0x10*/, 16 /*0x10*/), 200f, 3, 9999, new Vector2(21f, 49f) * 64f, false, false, 0.1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f);
            temporaryAnimatedSprite36.motion = new Vector2(0.0f, -1.5f);
            temporaryAnimatedSprite36.xPeriodic = true;
            temporaryAnimatedSprite36.xPeriodicLoopTime = 2500f;
            temporaryAnimatedSprite36.xPeriodicRange = 10f;
            interpolatedStringHandler2 = new DefaultInterpolatedStringHandler(17, 1);
            interpolatedStringHandler2.AppendLiteral("moonlightJellies_");
            ref DefaultInterpolatedStringHandler local31 = ref interpolatedStringHandler2;
            int num71 = num70;
            int num72 = num71 + 1;
            local31.AppendFormatted<int>(num71);
            temporaryAnimatedSprite36.lightId = this.GenerateLightSourceId(interpolatedStringHandler2.ToStringAndClear());
            temporaryAnimatedSprite36.lightcolor = Color.Black;
            temporaryAnimatedSprite36.lightRadius = 1f;
            temporaryAnimatedSprite36.yStopCoordinate = 2176;
            temporaryAnimatedSprite36.delayBeforeAnimationStart = 28000;
            temporaryAnimatedSprite36.pingPong = true;
            animatedSpriteList2.Add(temporaryAnimatedSprite36);
            TemporaryAnimatedSprite temporaryAnimatedSprite37 = new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(304, 16 /*0x10*/, 16 /*0x10*/, 16 /*0x10*/), 200f, 3, 9999, new Vector2(20f, 49f) * 64f, false, false, 0.1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f);
            temporaryAnimatedSprite37.motion = new Vector2(0.0f, -1.5f);
            temporaryAnimatedSprite37.xPeriodic = true;
            temporaryAnimatedSprite37.xPeriodicLoopTime = 2500f;
            temporaryAnimatedSprite37.xPeriodicRange = 10f;
            interpolatedStringHandler2 = new DefaultInterpolatedStringHandler(17, 1);
            interpolatedStringHandler2.AppendLiteral("moonlightJellies_");
            ref DefaultInterpolatedStringHandler local32 = ref interpolatedStringHandler2;
            int num73 = num72;
            int num74 = num73 + 1;
            local32.AppendFormatted<int>(num73);
            temporaryAnimatedSprite37.lightId = this.GenerateLightSourceId(interpolatedStringHandler2.ToStringAndClear());
            temporaryAnimatedSprite37.lightcolor = Color.Black;
            temporaryAnimatedSprite37.lightRadius = 1f;
            temporaryAnimatedSprite37.yStopCoordinate = 2240;
            temporaryAnimatedSprite37.delayBeforeAnimationStart = 28500;
            temporaryAnimatedSprite37.pingPong = true;
            animatedSpriteList2.Add(temporaryAnimatedSprite37);
            TemporaryAnimatedSprite temporaryAnimatedSprite38 = new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(304, 16 /*0x10*/, 16 /*0x10*/, 16 /*0x10*/), 200f, 3, 9999, new Vector2(22f, 49f) * 64f, false, false, 0.1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f);
            temporaryAnimatedSprite38.motion = new Vector2(0.0f, -1.5f);
            temporaryAnimatedSprite38.xPeriodic = true;
            temporaryAnimatedSprite38.xPeriodicLoopTime = 2500f;
            temporaryAnimatedSprite38.xPeriodicRange = 10f;
            interpolatedStringHandler2 = new DefaultInterpolatedStringHandler(17, 1);
            interpolatedStringHandler2.AppendLiteral("moonlightJellies_");
            ref DefaultInterpolatedStringHandler local33 = ref interpolatedStringHandler2;
            int num75 = num74;
            int num76 = num75 + 1;
            local33.AppendFormatted<int>(num75);
            temporaryAnimatedSprite38.lightId = this.GenerateLightSourceId(interpolatedStringHandler2.ToStringAndClear());
            temporaryAnimatedSprite38.lightcolor = Color.Black;
            temporaryAnimatedSprite38.lightRadius = 1f;
            temporaryAnimatedSprite38.yStopCoordinate = 2304 /*0x0900*/;
            temporaryAnimatedSprite38.delayBeforeAnimationStart = 28500;
            temporaryAnimatedSprite38.pingPong = true;
            animatedSpriteList2.Add(temporaryAnimatedSprite38);
            TemporaryAnimatedSprite temporaryAnimatedSprite39 = new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(304, 16 /*0x10*/, 16 /*0x10*/, 16 /*0x10*/), 200f, 3, 9999, new Vector2(33f, 49f) * 64f, false, false, 0.1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f);
            temporaryAnimatedSprite39.motion = new Vector2(0.0f, -1.5f);
            temporaryAnimatedSprite39.xPeriodic = true;
            temporaryAnimatedSprite39.xPeriodicLoopTime = 2500f;
            temporaryAnimatedSprite39.xPeriodicRange = 10f;
            interpolatedStringHandler2 = new DefaultInterpolatedStringHandler(17, 1);
            interpolatedStringHandler2.AppendLiteral("moonlightJellies_");
            ref DefaultInterpolatedStringHandler local34 = ref interpolatedStringHandler2;
            int num77 = num76;
            int num78 = num77 + 1;
            local34.AppendFormatted<int>(num77);
            temporaryAnimatedSprite39.lightId = this.GenerateLightSourceId(interpolatedStringHandler2.ToStringAndClear());
            temporaryAnimatedSprite39.lightcolor = Color.Black;
            temporaryAnimatedSprite39.lightRadius = 1f;
            temporaryAnimatedSprite39.yStopCoordinate = 2752;
            temporaryAnimatedSprite39.delayBeforeAnimationStart = 29000;
            temporaryAnimatedSprite39.pingPong = true;
            animatedSpriteList2.Add(temporaryAnimatedSprite39);
            TemporaryAnimatedSprite temporaryAnimatedSprite40 = new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(304, 16 /*0x10*/, 16 /*0x10*/, 16 /*0x10*/), 200f, 3, 9999, new Vector2(36f, 49f) * 64f, false, false, 0.1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f);
            temporaryAnimatedSprite40.motion = new Vector2(0.0f, -1.5f);
            temporaryAnimatedSprite40.xPeriodic = true;
            temporaryAnimatedSprite40.xPeriodicLoopTime = 2500f;
            temporaryAnimatedSprite40.xPeriodicRange = 10f;
            interpolatedStringHandler2 = new DefaultInterpolatedStringHandler(17, 1);
            interpolatedStringHandler2.AppendLiteral("moonlightJellies_");
            ref DefaultInterpolatedStringHandler local35 = ref interpolatedStringHandler2;
            int num79 = num78;
            int num80 = num79 + 1;
            local35.AppendFormatted<int>(num79);
            temporaryAnimatedSprite40.lightId = this.GenerateLightSourceId(interpolatedStringHandler2.ToStringAndClear());
            temporaryAnimatedSprite40.lightcolor = Color.Black;
            temporaryAnimatedSprite40.lightRadius = 1f;
            temporaryAnimatedSprite40.yStopCoordinate = 2752;
            temporaryAnimatedSprite40.delayBeforeAnimationStart = 30000;
            temporaryAnimatedSprite40.pingPong = true;
            animatedSpriteList2.Add(temporaryAnimatedSprite40);
            TemporaryAnimatedSprite temporaryAnimatedSprite41 = new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(256 /*0x0100*/, 32 /*0x20*/, 16 /*0x10*/, 16 /*0x10*/), 250f, 3, 9999, new Vector2(28f, 49f) * 64f, false, false, 0.1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f);
            temporaryAnimatedSprite41.motion = new Vector2(-0.5f, -0.5f);
            temporaryAnimatedSprite41.xPeriodic = true;
            temporaryAnimatedSprite41.xPeriodicLoopTime = 4000f;
            temporaryAnimatedSprite41.xPeriodicRange = 16f;
            interpolatedStringHandler2 = new DefaultInterpolatedStringHandler(17, 1);
            interpolatedStringHandler2.AppendLiteral("moonlightJellies_");
            ref DefaultInterpolatedStringHandler local36 = ref interpolatedStringHandler2;
            int num81 = num80;
            int num82 = num81 + 1;
            local36.AppendFormatted<int>(num81);
            temporaryAnimatedSprite41.lightId = this.GenerateLightSourceId(interpolatedStringHandler2.ToStringAndClear());
            temporaryAnimatedSprite41.lightcolor = Color.Black;
            temporaryAnimatedSprite41.lightRadius = 2f;
            temporaryAnimatedSprite41.xStopCoordinate = 1216;
            temporaryAnimatedSprite41.yStopCoordinate = 2432;
            temporaryAnimatedSprite41.delayBeforeAnimationStart = 32000;
            temporaryAnimatedSprite41.pingPong = true;
            animatedSpriteList2.Add(temporaryAnimatedSprite41);
            TemporaryAnimatedSprite temporaryAnimatedSprite42 = new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(256 /*0x0100*/, 16 /*0x10*/, 16 /*0x10*/, 16 /*0x10*/), 250f, 3, 9999, new Vector2(40f, 49f) * 64f, false, false, 0.1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f);
            temporaryAnimatedSprite42.motion = new Vector2(0.0f, -1f);
            temporaryAnimatedSprite42.xPeriodic = true;
            temporaryAnimatedSprite42.xPeriodicLoopTime = 3000f;
            temporaryAnimatedSprite42.xPeriodicRange = 16f;
            interpolatedStringHandler2 = new DefaultInterpolatedStringHandler(17, 1);
            interpolatedStringHandler2.AppendLiteral("moonlightJellies_");
            ref DefaultInterpolatedStringHandler local37 = ref interpolatedStringHandler2;
            int num83 = num82;
            int num84 = num83 + 1;
            local37.AppendFormatted<int>(num83);
            temporaryAnimatedSprite42.lightId = this.GenerateLightSourceId(interpolatedStringHandler2.ToStringAndClear());
            temporaryAnimatedSprite42.lightcolor = Color.Black;
            temporaryAnimatedSprite42.lightRadius = 1f;
            temporaryAnimatedSprite42.yStopCoordinate = 2560 /*0x0A00*/;
            temporaryAnimatedSprite42.delayBeforeAnimationStart = 10000;
            temporaryAnimatedSprite42.pingPong = true;
            animatedSpriteList2.Add(temporaryAnimatedSprite42);
            TemporaryAnimatedSprite temporaryAnimatedSprite43 = new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(256 /*0x0100*/, 16 /*0x10*/, 16 /*0x10*/, 16 /*0x10*/), 250f, 3, 9999, new Vector2(42f, 49f) * 64f, false, false, 0.1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f);
            temporaryAnimatedSprite43.motion = new Vector2(0.0f, -1f);
            temporaryAnimatedSprite43.xPeriodic = true;
            temporaryAnimatedSprite43.xPeriodicLoopTime = 3000f;
            temporaryAnimatedSprite43.xPeriodicRange = 16f;
            interpolatedStringHandler2 = new DefaultInterpolatedStringHandler(17, 1);
            interpolatedStringHandler2.AppendLiteral("moonlightJellies_");
            ref DefaultInterpolatedStringHandler local38 = ref interpolatedStringHandler2;
            int num85 = num84;
            int num86 = num85 + 1;
            local38.AppendFormatted<int>(num85);
            temporaryAnimatedSprite43.lightId = this.GenerateLightSourceId(interpolatedStringHandler2.ToStringAndClear());
            temporaryAnimatedSprite43.lightcolor = Color.Black;
            temporaryAnimatedSprite43.lightRadius = 1f;
            temporaryAnimatedSprite43.yStopCoordinate = 2752;
            temporaryAnimatedSprite43.pingPong = true;
            animatedSpriteList2.Add(temporaryAnimatedSprite43);
            TemporaryAnimatedSprite temporaryAnimatedSprite44 = new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(256 /*0x0100*/, 16 /*0x10*/, 16 /*0x10*/, 16 /*0x10*/), 250f, 3, 9999, new Vector2(43f, 49f) * 64f, false, false, 0.1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f);
            temporaryAnimatedSprite44.motion = new Vector2(0.0f, -1f);
            temporaryAnimatedSprite44.xPeriodic = true;
            temporaryAnimatedSprite44.xPeriodicLoopTime = 3000f;
            temporaryAnimatedSprite44.xPeriodicRange = 16f;
            interpolatedStringHandler2 = new DefaultInterpolatedStringHandler(17, 1);
            interpolatedStringHandler2.AppendLiteral("moonlightJellies_");
            ref DefaultInterpolatedStringHandler local39 = ref interpolatedStringHandler2;
            int num87 = num86;
            int num88 = num87 + 1;
            local39.AppendFormatted<int>(num87);
            temporaryAnimatedSprite44.lightId = this.GenerateLightSourceId(interpolatedStringHandler2.ToStringAndClear());
            temporaryAnimatedSprite44.lightcolor = Color.Black;
            temporaryAnimatedSprite44.lightRadius = 1f;
            temporaryAnimatedSprite44.yStopCoordinate = 2624;
            temporaryAnimatedSprite44.delayBeforeAnimationStart = 12000;
            temporaryAnimatedSprite44.pingPong = true;
            animatedSpriteList2.Add(temporaryAnimatedSprite44);
            TemporaryAnimatedSprite temporaryAnimatedSprite45 = new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(256 /*0x0100*/, 16 /*0x10*/, 16 /*0x10*/, 16 /*0x10*/), 250f, 3, 9999, new Vector2(45f, 49f) * 64f, false, false, 0.1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f);
            temporaryAnimatedSprite45.motion = new Vector2(0.0f, -1f);
            temporaryAnimatedSprite45.xPeriodic = true;
            temporaryAnimatedSprite45.xPeriodicLoopTime = 3000f;
            temporaryAnimatedSprite45.xPeriodicRange = 16f;
            interpolatedStringHandler2 = new DefaultInterpolatedStringHandler(17, 1);
            interpolatedStringHandler2.AppendLiteral("moonlightJellies_");
            ref DefaultInterpolatedStringHandler local40 = ref interpolatedStringHandler2;
            int num89 = num88;
            int num90 = num89 + 1;
            local40.AppendFormatted<int>(num89);
            temporaryAnimatedSprite45.lightId = this.GenerateLightSourceId(interpolatedStringHandler2.ToStringAndClear());
            temporaryAnimatedSprite45.lightcolor = Color.Black;
            temporaryAnimatedSprite45.lightRadius = 1f;
            temporaryAnimatedSprite45.yStopCoordinate = 2496;
            temporaryAnimatedSprite45.delayBeforeAnimationStart = 14000;
            temporaryAnimatedSprite45.pingPong = true;
            animatedSpriteList2.Add(temporaryAnimatedSprite45);
            TemporaryAnimatedSprite temporaryAnimatedSprite46 = new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(256 /*0x0100*/, 16 /*0x10*/, 16 /*0x10*/, 16 /*0x10*/), 250f, 3, 9999, new Vector2(46f, 49f) * 64f, false, false, 0.1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f);
            temporaryAnimatedSprite46.motion = new Vector2(0.0f, -1f);
            temporaryAnimatedSprite46.xPeriodic = true;
            temporaryAnimatedSprite46.xPeriodicLoopTime = 3000f;
            temporaryAnimatedSprite46.xPeriodicRange = 16f;
            interpolatedStringHandler2 = new DefaultInterpolatedStringHandler(17, 1);
            interpolatedStringHandler2.AppendLiteral("moonlightJellies_");
            ref DefaultInterpolatedStringHandler local41 = ref interpolatedStringHandler2;
            int num91 = num90;
            int num92 = num91 + 1;
            local41.AppendFormatted<int>(num91);
            temporaryAnimatedSprite46.lightId = this.GenerateLightSourceId(interpolatedStringHandler2.ToStringAndClear());
            temporaryAnimatedSprite46.lightcolor = Color.Black;
            temporaryAnimatedSprite46.lightRadius = 1f;
            temporaryAnimatedSprite46.yStopCoordinate = 1856;
            temporaryAnimatedSprite46.delayBeforeAnimationStart = 19500;
            temporaryAnimatedSprite46.pingPong = true;
            animatedSpriteList2.Add(temporaryAnimatedSprite46);
            TemporaryAnimatedSprite temporaryAnimatedSprite47 = new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(256 /*0x0100*/, 16 /*0x10*/, 16 /*0x10*/, 16 /*0x10*/), 250f, 3, 9999, new Vector2(48f, 49f) * 64f, false, false, 0.1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f);
            temporaryAnimatedSprite47.motion = new Vector2(0.0f, -1f);
            temporaryAnimatedSprite47.xPeriodic = true;
            temporaryAnimatedSprite47.xPeriodicLoopTime = 3000f;
            temporaryAnimatedSprite47.xPeriodicRange = 16f;
            interpolatedStringHandler2 = new DefaultInterpolatedStringHandler(17, 1);
            interpolatedStringHandler2.AppendLiteral("moonlightJellies_");
            ref DefaultInterpolatedStringHandler local42 = ref interpolatedStringHandler2;
            int num93 = num92;
            int num94 = num93 + 1;
            local42.AppendFormatted<int>(num93);
            temporaryAnimatedSprite47.lightId = this.GenerateLightSourceId(interpolatedStringHandler2.ToStringAndClear());
            temporaryAnimatedSprite47.lightcolor = Color.Black;
            temporaryAnimatedSprite47.lightRadius = 1f;
            temporaryAnimatedSprite47.yStopCoordinate = 2240;
            temporaryAnimatedSprite47.delayBeforeAnimationStart = 20300;
            temporaryAnimatedSprite47.pingPong = true;
            animatedSpriteList2.Add(temporaryAnimatedSprite47);
            TemporaryAnimatedSprite temporaryAnimatedSprite48 = new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(256 /*0x0100*/, 16 /*0x10*/, 16 /*0x10*/, 16 /*0x10*/), 250f, 3, 9999, new Vector2(49f, 49f) * 64f, false, false, 0.1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f);
            temporaryAnimatedSprite48.motion = new Vector2(0.0f, -1f);
            temporaryAnimatedSprite48.xPeriodic = true;
            temporaryAnimatedSprite48.xPeriodicLoopTime = 3000f;
            temporaryAnimatedSprite48.xPeriodicRange = 16f;
            interpolatedStringHandler2 = new DefaultInterpolatedStringHandler(17, 1);
            interpolatedStringHandler2.AppendLiteral("moonlightJellies_");
            ref DefaultInterpolatedStringHandler local43 = ref interpolatedStringHandler2;
            int num95 = num94;
            int num96 = num95 + 1;
            local43.AppendFormatted<int>(num95);
            temporaryAnimatedSprite48.lightId = this.GenerateLightSourceId(interpolatedStringHandler2.ToStringAndClear());
            temporaryAnimatedSprite48.lightcolor = Color.Black;
            temporaryAnimatedSprite48.lightRadius = 1f;
            temporaryAnimatedSprite48.yStopCoordinate = 2560 /*0x0A00*/;
            temporaryAnimatedSprite48.delayBeforeAnimationStart = 21500;
            temporaryAnimatedSprite48.pingPong = true;
            animatedSpriteList2.Add(temporaryAnimatedSprite48);
            TemporaryAnimatedSprite temporaryAnimatedSprite49 = new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(256 /*0x0100*/, 16 /*0x10*/, 16 /*0x10*/, 16 /*0x10*/), 250f, 3, 9999, new Vector2(50f, 49f) * 64f, false, false, 0.1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f);
            temporaryAnimatedSprite49.motion = new Vector2(0.0f, -1f);
            temporaryAnimatedSprite49.xPeriodic = true;
            temporaryAnimatedSprite49.xPeriodicLoopTime = 3000f;
            temporaryAnimatedSprite49.xPeriodicRange = 16f;
            interpolatedStringHandler2 = new DefaultInterpolatedStringHandler(17, 1);
            interpolatedStringHandler2.AppendLiteral("moonlightJellies_");
            ref DefaultInterpolatedStringHandler local44 = ref interpolatedStringHandler2;
            int num97 = num96;
            int num98 = num97 + 1;
            local44.AppendFormatted<int>(num97);
            temporaryAnimatedSprite49.lightId = this.GenerateLightSourceId(interpolatedStringHandler2.ToStringAndClear());
            temporaryAnimatedSprite49.lightcolor = Color.Black;
            temporaryAnimatedSprite49.lightRadius = 1f;
            temporaryAnimatedSprite49.yStopCoordinate = 1920;
            temporaryAnimatedSprite49.delayBeforeAnimationStart = 22400;
            temporaryAnimatedSprite49.pingPong = true;
            animatedSpriteList2.Add(temporaryAnimatedSprite49);
            TemporaryAnimatedSprite temporaryAnimatedSprite50 = new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(256 /*0x0100*/, 16 /*0x10*/, 16 /*0x10*/, 16 /*0x10*/), 250f, 3, 9999, new Vector2(51f, 49f) * 64f, false, false, 0.1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f);
            temporaryAnimatedSprite50.motion = new Vector2(0.0f, -1f);
            temporaryAnimatedSprite50.xPeriodic = true;
            temporaryAnimatedSprite50.xPeriodicLoopTime = 3000f;
            temporaryAnimatedSprite50.xPeriodicRange = 16f;
            interpolatedStringHandler2 = new DefaultInterpolatedStringHandler(17, 1);
            interpolatedStringHandler2.AppendLiteral("moonlightJellies_");
            ref DefaultInterpolatedStringHandler local45 = ref interpolatedStringHandler2;
            int num99 = num98;
            int num100 = num99 + 1;
            local45.AppendFormatted<int>(num99);
            temporaryAnimatedSprite50.lightId = this.GenerateLightSourceId(interpolatedStringHandler2.ToStringAndClear());
            temporaryAnimatedSprite50.lightcolor = Color.Black;
            temporaryAnimatedSprite50.lightRadius = 1f;
            temporaryAnimatedSprite50.yStopCoordinate = 2112;
            temporaryAnimatedSprite50.delayBeforeAnimationStart = 23200;
            temporaryAnimatedSprite50.pingPong = true;
            animatedSpriteList2.Add(temporaryAnimatedSprite50);
            TemporaryAnimatedSprite temporaryAnimatedSprite51 = new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(256 /*0x0100*/, 16 /*0x10*/, 16 /*0x10*/, 16 /*0x10*/), 250f, 3, 9999, new Vector2(52f, 49f) * 64f, false, false, 0.1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f);
            temporaryAnimatedSprite51.motion = new Vector2(0.0f, -1f);
            temporaryAnimatedSprite51.xPeriodic = true;
            temporaryAnimatedSprite51.xPeriodicLoopTime = 3000f;
            temporaryAnimatedSprite51.xPeriodicRange = 16f;
            interpolatedStringHandler2 = new DefaultInterpolatedStringHandler(17, 1);
            interpolatedStringHandler2.AppendLiteral("moonlightJellies_");
            ref DefaultInterpolatedStringHandler local46 = ref interpolatedStringHandler2;
            int num101 = num100;
            int num102 = num101 + 1;
            local46.AppendFormatted<int>(num101);
            temporaryAnimatedSprite51.lightId = this.GenerateLightSourceId(interpolatedStringHandler2.ToStringAndClear());
            temporaryAnimatedSprite51.lightcolor = Color.Black;
            temporaryAnimatedSprite51.lightRadius = 1f;
            temporaryAnimatedSprite51.yStopCoordinate = 2432;
            temporaryAnimatedSprite51.delayBeforeAnimationStart = 24000;
            temporaryAnimatedSprite51.pingPong = true;
            animatedSpriteList2.Add(temporaryAnimatedSprite51);
            TemporaryAnimatedSprite temporaryAnimatedSprite52 = new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(256 /*0x0100*/, 16 /*0x10*/, 16 /*0x10*/, 16 /*0x10*/), 250f, 3, 9999, new Vector2(53f, 49f) * 64f, false, false, 0.1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f);
            temporaryAnimatedSprite52.motion = new Vector2(0.0f, -1f);
            temporaryAnimatedSprite52.xPeriodic = true;
            temporaryAnimatedSprite52.xPeriodicLoopTime = 3000f;
            temporaryAnimatedSprite52.xPeriodicRange = 16f;
            interpolatedStringHandler2 = new DefaultInterpolatedStringHandler(17, 1);
            interpolatedStringHandler2.AppendLiteral("moonlightJellies_");
            ref DefaultInterpolatedStringHandler local47 = ref interpolatedStringHandler2;
            int num103 = num102;
            int num104 = num103 + 1;
            local47.AppendFormatted<int>(num103);
            temporaryAnimatedSprite52.lightId = this.GenerateLightSourceId(interpolatedStringHandler2.ToStringAndClear());
            temporaryAnimatedSprite52.lightcolor = Color.Black;
            temporaryAnimatedSprite52.lightRadius = 1f;
            temporaryAnimatedSprite52.yStopCoordinate = 2240;
            temporaryAnimatedSprite52.delayBeforeAnimationStart = 24600;
            temporaryAnimatedSprite52.pingPong = true;
            animatedSpriteList2.Add(temporaryAnimatedSprite52);
            TemporaryAnimatedSprite temporaryAnimatedSprite53 = new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(256 /*0x0100*/, 16 /*0x10*/, 16 /*0x10*/, 16 /*0x10*/), 250f, 3, 9999, new Vector2(54f, 49f) * 64f, false, false, 0.1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f);
            temporaryAnimatedSprite53.motion = new Vector2(0.0f, -1f);
            temporaryAnimatedSprite53.xPeriodic = true;
            temporaryAnimatedSprite53.xPeriodicLoopTime = 3000f;
            temporaryAnimatedSprite53.xPeriodicRange = 16f;
            interpolatedStringHandler2 = new DefaultInterpolatedStringHandler(17, 1);
            interpolatedStringHandler2.AppendLiteral("moonlightJellies_");
            ref DefaultInterpolatedStringHandler local48 = ref interpolatedStringHandler2;
            int num105 = num104;
            int num106 = num105 + 1;
            local48.AppendFormatted<int>(num105);
            temporaryAnimatedSprite53.lightId = this.GenerateLightSourceId(interpolatedStringHandler2.ToStringAndClear());
            temporaryAnimatedSprite53.lightcolor = Color.Black;
            temporaryAnimatedSprite53.lightRadius = 1f;
            temporaryAnimatedSprite53.yStopCoordinate = 1920;
            temporaryAnimatedSprite53.delayBeforeAnimationStart = 25600;
            temporaryAnimatedSprite53.pingPong = true;
            animatedSpriteList2.Add(temporaryAnimatedSprite53);
            TemporaryAnimatedSprite temporaryAnimatedSprite54 = new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(256 /*0x0100*/, 16 /*0x10*/, 16 /*0x10*/, 16 /*0x10*/), 250f, 3, 9999, new Vector2(55f, 49f) * 64f, false, false, 0.1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f);
            temporaryAnimatedSprite54.motion = new Vector2(0.0f, -1f);
            temporaryAnimatedSprite54.xPeriodic = true;
            temporaryAnimatedSprite54.xPeriodicLoopTime = 3000f;
            temporaryAnimatedSprite54.xPeriodicRange = 16f;
            interpolatedStringHandler2 = new DefaultInterpolatedStringHandler(17, 1);
            interpolatedStringHandler2.AppendLiteral("moonlightJellies_");
            ref DefaultInterpolatedStringHandler local49 = ref interpolatedStringHandler2;
            int num107 = num106;
            int num108 = num107 + 1;
            local49.AppendFormatted<int>(num107);
            temporaryAnimatedSprite54.lightId = this.GenerateLightSourceId(interpolatedStringHandler2.ToStringAndClear());
            temporaryAnimatedSprite54.lightcolor = Color.Black;
            temporaryAnimatedSprite54.lightRadius = 1f;
            temporaryAnimatedSprite54.yStopCoordinate = 2560 /*0x0A00*/;
            temporaryAnimatedSprite54.delayBeforeAnimationStart = 26900;
            temporaryAnimatedSprite54.pingPong = true;
            animatedSpriteList2.Add(temporaryAnimatedSprite54);
            TemporaryAnimatedSprite temporaryAnimatedSprite55 = new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(256 /*0x0100*/, 16 /*0x10*/, 16 /*0x10*/, 16 /*0x10*/), 250f, 3, 9999, new Vector2(4f, 49f) * 64f, false, false, 0.1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f);
            temporaryAnimatedSprite55.motion = new Vector2(0.0f, -1f);
            temporaryAnimatedSprite55.xPeriodic = true;
            temporaryAnimatedSprite55.xPeriodicLoopTime = 3000f;
            temporaryAnimatedSprite55.xPeriodicRange = 16f;
            interpolatedStringHandler2 = new DefaultInterpolatedStringHandler(17, 1);
            interpolatedStringHandler2.AppendLiteral("moonlightJellies_");
            ref DefaultInterpolatedStringHandler local50 = ref interpolatedStringHandler2;
            int num109 = num108;
            int num110 = num109 + 1;
            local50.AppendFormatted<int>(num109);
            temporaryAnimatedSprite55.lightId = this.GenerateLightSourceId(interpolatedStringHandler2.ToStringAndClear());
            temporaryAnimatedSprite55.lightcolor = Color.Black;
            temporaryAnimatedSprite55.lightRadius = 1f;
            temporaryAnimatedSprite55.yStopCoordinate = 1920;
            temporaryAnimatedSprite55.delayBeforeAnimationStart = 24000;
            temporaryAnimatedSprite55.pingPong = true;
            animatedSpriteList2.Add(temporaryAnimatedSprite55);
            TemporaryAnimatedSprite temporaryAnimatedSprite56 = new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(256 /*0x0100*/, 16 /*0x10*/, 16 /*0x10*/, 16 /*0x10*/), 250f, 3, 9999, new Vector2(5f, 49f) * 64f, false, false, 0.1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f);
            temporaryAnimatedSprite56.motion = new Vector2(0.0f, -1f);
            temporaryAnimatedSprite56.xPeriodic = true;
            temporaryAnimatedSprite56.xPeriodicLoopTime = 3000f;
            temporaryAnimatedSprite56.xPeriodicRange = 16f;
            interpolatedStringHandler2 = new DefaultInterpolatedStringHandler(17, 1);
            interpolatedStringHandler2.AppendLiteral("moonlightJellies_");
            ref DefaultInterpolatedStringHandler local51 = ref interpolatedStringHandler2;
            int num111 = num110;
            int num112 = num111 + 1;
            local51.AppendFormatted<int>(num111);
            temporaryAnimatedSprite56.lightId = this.GenerateLightSourceId(interpolatedStringHandler2.ToStringAndClear());
            temporaryAnimatedSprite56.lightcolor = Color.Black;
            temporaryAnimatedSprite56.lightRadius = 1f;
            temporaryAnimatedSprite56.yStopCoordinate = 2560 /*0x0A00*/;
            temporaryAnimatedSprite56.delayBeforeAnimationStart = 24600;
            temporaryAnimatedSprite56.pingPong = true;
            animatedSpriteList2.Add(temporaryAnimatedSprite56);
            TemporaryAnimatedSprite temporaryAnimatedSprite57 = new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(256 /*0x0100*/, 16 /*0x10*/, 16 /*0x10*/, 16 /*0x10*/), 250f, 3, 9999, new Vector2(3f, 49f) * 64f, false, false, 0.1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f);
            temporaryAnimatedSprite57.motion = new Vector2(0.0f, -1f);
            temporaryAnimatedSprite57.xPeriodic = true;
            temporaryAnimatedSprite57.xPeriodicLoopTime = 3000f;
            temporaryAnimatedSprite57.xPeriodicRange = 16f;
            interpolatedStringHandler2 = new DefaultInterpolatedStringHandler(17, 1);
            interpolatedStringHandler2.AppendLiteral("moonlightJellies_");
            ref DefaultInterpolatedStringHandler local52 = ref interpolatedStringHandler2;
            int num113 = num112;
            int num114 = num113 + 1;
            local52.AppendFormatted<int>(num113);
            temporaryAnimatedSprite57.lightId = this.GenerateLightSourceId(interpolatedStringHandler2.ToStringAndClear());
            temporaryAnimatedSprite57.lightcolor = Color.Black;
            temporaryAnimatedSprite57.lightRadius = 1f;
            temporaryAnimatedSprite57.yStopCoordinate = 2176;
            temporaryAnimatedSprite57.delayBeforeAnimationStart = 25600;
            temporaryAnimatedSprite57.pingPong = true;
            animatedSpriteList2.Add(temporaryAnimatedSprite57);
            TemporaryAnimatedSprite temporaryAnimatedSprite58 = new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(256 /*0x0100*/, 16 /*0x10*/, 16 /*0x10*/, 16 /*0x10*/), 250f, 3, 9999, new Vector2(6f, 49f) * 64f, false, false, 0.1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f);
            temporaryAnimatedSprite58.motion = new Vector2(0.0f, -1f);
            temporaryAnimatedSprite58.xPeriodic = true;
            temporaryAnimatedSprite58.xPeriodicLoopTime = 3000f;
            temporaryAnimatedSprite58.xPeriodicRange = 16f;
            interpolatedStringHandler2 = new DefaultInterpolatedStringHandler(17, 1);
            interpolatedStringHandler2.AppendLiteral("moonlightJellies_");
            ref DefaultInterpolatedStringHandler local53 = ref interpolatedStringHandler2;
            int num115 = num114;
            int num116 = num115 + 1;
            local53.AppendFormatted<int>(num115);
            temporaryAnimatedSprite58.lightId = this.GenerateLightSourceId(interpolatedStringHandler2.ToStringAndClear());
            temporaryAnimatedSprite58.lightcolor = Color.Black;
            temporaryAnimatedSprite58.lightRadius = 1f;
            temporaryAnimatedSprite58.yStopCoordinate = 2368;
            temporaryAnimatedSprite58.delayBeforeAnimationStart = 26900;
            temporaryAnimatedSprite58.pingPong = true;
            animatedSpriteList2.Add(temporaryAnimatedSprite58);
            TemporaryAnimatedSprite temporaryAnimatedSprite59 = new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(256 /*0x0100*/, 16 /*0x10*/, 16 /*0x10*/, 16 /*0x10*/), 250f, 3, 9999, new Vector2(8f, 49f) * 64f, false, false, 0.1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f);
            temporaryAnimatedSprite59.motion = new Vector2(0.0f, -1f);
            temporaryAnimatedSprite59.xPeriodic = true;
            temporaryAnimatedSprite59.xPeriodicLoopTime = 3000f;
            temporaryAnimatedSprite59.xPeriodicRange = 16f;
            interpolatedStringHandler2 = new DefaultInterpolatedStringHandler(17, 1);
            interpolatedStringHandler2.AppendLiteral("moonlightJellies_");
            ref DefaultInterpolatedStringHandler local54 = ref interpolatedStringHandler2;
            int num117 = num116;
            int num118 = num117 + 1;
            local54.AppendFormatted<int>(num117);
            temporaryAnimatedSprite59.lightId = this.GenerateLightSourceId(interpolatedStringHandler2.ToStringAndClear());
            temporaryAnimatedSprite59.lightcolor = Color.Black;
            temporaryAnimatedSprite59.lightRadius = 1f;
            temporaryAnimatedSprite59.yStopCoordinate = 2688;
            temporaryAnimatedSprite59.delayBeforeAnimationStart = 26900;
            temporaryAnimatedSprite59.pingPong = true;
            animatedSpriteList2.Add(temporaryAnimatedSprite59);
            TemporaryAnimatedSprite temporaryAnimatedSprite60 = new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(304, 16 /*0x10*/, 16 /*0x10*/, 16 /*0x10*/), 200f, 3, 9999, new Vector2(50f, 49f) * 64f, false, false, 0.1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f);
            temporaryAnimatedSprite60.motion = new Vector2(0.0f, -1.5f);
            temporaryAnimatedSprite60.xPeriodic = true;
            temporaryAnimatedSprite60.xPeriodicLoopTime = 2500f;
            temporaryAnimatedSprite60.xPeriodicRange = 10f;
            interpolatedStringHandler2 = new DefaultInterpolatedStringHandler(17, 1);
            interpolatedStringHandler2.AppendLiteral("moonlightJellies_");
            ref DefaultInterpolatedStringHandler local55 = ref interpolatedStringHandler2;
            int num119 = num118;
            int num120 = num119 + 1;
            local55.AppendFormatted<int>(num119);
            temporaryAnimatedSprite60.lightId = this.GenerateLightSourceId(interpolatedStringHandler2.ToStringAndClear());
            temporaryAnimatedSprite60.lightcolor = Color.Black;
            temporaryAnimatedSprite60.lightRadius = 1f;
            temporaryAnimatedSprite60.yStopCoordinate = 2688;
            temporaryAnimatedSprite60.delayBeforeAnimationStart = 28500;
            temporaryAnimatedSprite60.pingPong = true;
            animatedSpriteList2.Add(temporaryAnimatedSprite60);
            TemporaryAnimatedSprite temporaryAnimatedSprite61 = new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(304, 16 /*0x10*/, 16 /*0x10*/, 16 /*0x10*/), 200f, 3, 9999, new Vector2(51f, 49f) * 64f, false, false, 0.1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f);
            temporaryAnimatedSprite61.motion = new Vector2(0.0f, -1.5f);
            temporaryAnimatedSprite61.xPeriodic = true;
            temporaryAnimatedSprite61.xPeriodicLoopTime = 2500f;
            temporaryAnimatedSprite61.xPeriodicRange = 10f;
            interpolatedStringHandler2 = new DefaultInterpolatedStringHandler(17, 1);
            interpolatedStringHandler2.AppendLiteral("moonlightJellies_");
            ref DefaultInterpolatedStringHandler local56 = ref interpolatedStringHandler2;
            int num121 = num120;
            int num122 = num121 + 1;
            local56.AppendFormatted<int>(num121);
            temporaryAnimatedSprite61.lightId = this.GenerateLightSourceId(interpolatedStringHandler2.ToStringAndClear());
            temporaryAnimatedSprite61.lightcolor = Color.Black;
            temporaryAnimatedSprite61.lightRadius = 1f;
            temporaryAnimatedSprite61.yStopCoordinate = 2752;
            temporaryAnimatedSprite61.delayBeforeAnimationStart = 28500;
            temporaryAnimatedSprite61.pingPong = true;
            animatedSpriteList2.Add(temporaryAnimatedSprite61);
            TemporaryAnimatedSprite temporaryAnimatedSprite62 = new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(304, 16 /*0x10*/, 16 /*0x10*/, 16 /*0x10*/), 200f, 3, 9999, new Vector2(52f, 49f) * 64f, false, false, 0.1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f);
            temporaryAnimatedSprite62.motion = new Vector2(0.0f, -1.5f);
            temporaryAnimatedSprite62.xPeriodic = true;
            temporaryAnimatedSprite62.xPeriodicLoopTime = 2500f;
            temporaryAnimatedSprite62.xPeriodicRange = 10f;
            interpolatedStringHandler2 = new DefaultInterpolatedStringHandler(17, 1);
            interpolatedStringHandler2.AppendLiteral("moonlightJellies_");
            ref DefaultInterpolatedStringHandler local57 = ref interpolatedStringHandler2;
            int num123 = num122;
            int num124 = num123 + 1;
            local57.AppendFormatted<int>(num123);
            temporaryAnimatedSprite62.lightId = this.GenerateLightSourceId(interpolatedStringHandler2.ToStringAndClear());
            temporaryAnimatedSprite62.lightcolor = Color.Black;
            temporaryAnimatedSprite62.lightRadius = 1f;
            temporaryAnimatedSprite62.yStopCoordinate = 2816 /*0x0B00*/;
            temporaryAnimatedSprite62.delayBeforeAnimationStart = 29000;
            temporaryAnimatedSprite62.pingPong = true;
            animatedSpriteList2.Add(temporaryAnimatedSprite62);
            this.underwaterSprites = animatedSpriteList2;
            return;
          case 't':
            if (!(key == "trashBearPrelude"))
              return;
            Utility.addStarsAndSpirals(location, 95, 106, 23, 4, 10000, 275, Color.Lime);
            return;
          case 'w':
            if (!(key == "wizardSewerMagic"))
              return;
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(276, 1985, 12, 11), 50f, 4, 20, new Vector2(15f, 13f) * 64f + new Vector2(8f, 0.0f), false, false, 1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
            {
              lightId = this.GenerateLightSourceId("wizardSewerMagic_1"),
              lightRadius = 1f,
              lightcolor = Color.Black,
              alphaFade = 0.005f
            });
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(276, 1985, 12, 11), 50f, 4, 20, new Vector2(17f, 13f) * 64f + new Vector2(8f, 0.0f), false, false, 1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
            {
              lightId = this.GenerateLightSourceId("wizardSewerMagic_2"),
              lightRadius = 1f,
              lightcolor = Color.Black,
              alphaFade = 0.005f
            });
            return;
          default:
            return;
        }
      case 17:
        switch (key[0])
        {
          case 'E':
            if (!(key == "EmilyBoomBoxStart"))
              return;
            location.getTemporarySpriteByID(999).pulse = true;
            location.getTemporarySpriteByID(999).pulseTime = 420f;
            return;
          case 'd':
            if (!(key == "doneWithSlideShow"))
              return;
            (location as Summit).isShowingEndSlideshow = false;
            return;
          case 'l':
            if (!(key == "leahPaintingSetup"))
              return;
            Texture2D texture2D11 = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\temporary_sprites_1");
            location.TemporarySprites.Add(new TemporaryAnimatedSprite()
            {
              texture = texture2D11,
              sourceRect = new Microsoft.Xna.Framework.Rectangle(368, 393, 15, 28),
              animationLength = 1,
              sourceRectStartingPos = new Vector2(368f, 393f),
              interval = 5000f,
              totalNumberOfLoops = 99999,
              position = new Vector2(72f, 38f) * 64f + new Vector2(3f, -13f) * 4f,
              scale = 4f,
              layerDepth = 0.1f,
              id = 999
            });
            location.TemporarySprites.Add(new TemporaryAnimatedSprite()
            {
              texture = texture2D11,
              sourceRect = new Microsoft.Xna.Framework.Rectangle(368, 393, 15, 28),
              animationLength = 1,
              sourceRectStartingPos = new Vector2(368f, 393f),
              interval = 5000f,
              totalNumberOfLoops = 99999,
              position = new Vector2(74f, 40f) * 64f + new Vector2(3f, -17f) * 4f,
              scale = 4f,
              layerDepth = 0.1f,
              id = 888
            });
            location.TemporarySprites.Add(new TemporaryAnimatedSprite()
            {
              texture = texture2D11,
              sourceRect = new Microsoft.Xna.Framework.Rectangle(369, 424, 11, 15),
              animationLength = 1,
              sourceRectStartingPos = new Vector2(369f, 424f),
              interval = 9999f,
              totalNumberOfLoops = 99999,
              position = new Vector2(75f, 40f) * 64f + new Vector2(-2f, -11f) * 4f,
              scale = 4f,
              layerDepth = 0.01f,
              id = 444
            });
            location.TemporarySprites.Add(new TemporaryAnimatedSprite()
            {
              texture = Game1.mouseCursors,
              sourceRect = new Microsoft.Xna.Framework.Rectangle(96 /*0x60*/, 1822, 32 /*0x20*/, 34),
              animationLength = 1,
              sourceRectStartingPos = new Vector2(96f, 1822f),
              interval = 5000f,
              totalNumberOfLoops = 99999,
              position = new Vector2(79f, 36f) * 64f,
              scale = 4f,
              layerDepth = 0.1f
            });
            return;
          case 'm':
            if (!(key == "maruElectrocution"))
              return;
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(432, 1664, 16 /*0x10*/, 32 /*0x20*/), 40f, 1, 20, new Vector2(7f, 5f) * 64f - new Vector2(-4f, 8f), true, false, 1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f));
            return;
          case 's':
            if (!(key == "springOnionRemove"))
              return;
            location.removeTemporarySpritesWithID(777);
            return;
          default:
            return;
        }
      case 18:
        switch (key[10])
        {
          case 'P':
            if (!(key == "farmerHoldPainting"))
              return;
            Texture2D texture2D12 = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\temporary_sprites_1");
            location.getTemporarySpriteByID(888).sourceRect.X += 15;
            location.getTemporarySpriteByID(888).sourceRectStartingPos.X += 15f;
            location.removeTemporarySpritesWithID(444);
            location.TemporarySprites.Add(new TemporaryAnimatedSprite()
            {
              texture = texture2D12,
              sourceRect = new Microsoft.Xna.Framework.Rectangle(476, 394, 25, 22),
              animationLength = 1,
              sourceRectStartingPos = new Vector2(476f, 394f),
              interval = 5000f,
              totalNumberOfLoops = 9999,
              position = new Vector2(75f, 40f) * 64f + new Vector2(-4f, -33f) * 4f,
              scale = 4f,
              layerDepth = 1f,
              id = 777
            });
            return;
          case 'a':
            if (!(key == "terraria_cat_leave"))
              return;
            TemporaryAnimatedSprite terraria_cat = location.getTemporarySpriteByID(777);
            if (terraria_cat == null)
              return;
            terraria_cat.sourceRect.Y = 0;
            terraria_cat.sourceRect.X = terraria_cat.currentParentTileIndex * 16 /*0x10*/;
            terraria_cat.paused = false;
            terraria_cat.motion = new Vector2(1f, 0.0f);
            terraria_cat.xStopCoordinate = 1152;
            terraria_cat.flipped = true;
            Microsoft.Xna.Framework.Rectangle warpRect2 = new Microsoft.Xna.Framework.Rectangle(1024 /*0x0400*/, 120, 144 /*0x90*/, 272);
            terraria_cat.reachedStopCoordinate = (TemporaryAnimatedSprite.endBehavior) (param2 =>
            {
              terraria_cat.position.X = -4000f;
              location.removeTemporarySpritesWithID(888);
              Game1.playSound("terraria_warp");
              for (int index4 = 0; index4 < 80 /*0x50*/; ++index4)
              {
                Vector2 positionInThisRectangle = Utility.getRandomPositionInThisRectangle(warpRect2, Game1.random);
                Vector2 vector2_5 = positionInThisRectangle - Utility.PointToVector2(warpRect2.Center);
                vector2_5.Normalize();
                Vector2 vector2_6 = vector2_5 * (float) (Game1.random.Next(10, 21) / 10);
                location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\terraria_cat", new Microsoft.Xna.Framework.Rectangle(113 + Game1.random.Next(3) * 5, 123, 5, 5), 999f, 1, 9999, positionInThisRectangle, false, false, 0.8f, 0.02f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
                {
                  layerDepth = 0.99f,
                  rotationChange = (float) Game1.random.Next(-10, 10) / 100f,
                  motion = vector2_6,
                  acceleration = -vector2_6 / 150f,
                  scaleChange = (float) Game1.random.Next(-10, 0) / 500f,
                  delayBeforeAnimationStart = index4 * 5
                });
              }
            });
            return;
          case 'e':
            if (!(key == "movieTheater_setup"))
              return;
            Game1.currentLightSources.Add(new LightSource("Event_MovieProjector", 7, new Vector2(192f, 64f) + new Vector2(64f, 80f) * 4f, 4f));
            location.temporarySprites.Add(new TemporaryAnimatedSprite()
            {
              texture = Game1.temporaryContent.Load<Texture2D>("Maps\\MovieTheaterScreen_TileSheet"),
              sourceRect = new Microsoft.Xna.Framework.Rectangle(224 /*0xE0*/, 0, 96 /*0x60*/, 112 /*0x70*/),
              sourceRectStartingPos = new Vector2(224f, 0.0f),
              animationLength = 1,
              interval = 5000f,
              totalNumberOfLoops = 9999,
              scale = 4f,
              position = new Vector2(4f, 4f) * 64f,
              layerDepth = 1f,
              id = 999,
              delayBeforeAnimationStart = 7950
            });
            return;
          case 'f':
            return;
          case 'g':
            return;
          case 'h':
            switch (key)
            {
              case "harveyKitchenFlame":
                location.TemporarySprites.Add(new TemporaryAnimatedSprite()
                {
                  texture = Game1.mouseCursors,
                  sourceRect = new Microsoft.Xna.Framework.Rectangle(276, 1985, 12, 11),
                  animationLength = 4,
                  sourceRectStartingPos = new Vector2(276f, 1985f),
                  interval = 100f,
                  totalNumberOfLoops = 6,
                  position = new Vector2(22f, 22f) * 64f + new Vector2(8f, 5f) * 4f,
                  scale = 4f,
                  layerDepth = 0.155840009f
                });
                return;
              case "harveyKitchenSetup":
                Texture2D texture2D13 = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\temporary_sprites_1");
                location.TemporarySprites.Add(new TemporaryAnimatedSprite()
                {
                  texture = texture2D13,
                  sourceRect = new Microsoft.Xna.Framework.Rectangle(379, 251, 31 /*0x1F*/, 13),
                  animationLength = 1,
                  sourceRectStartingPos = new Vector2(379f, 251f),
                  interval = 5000f,
                  totalNumberOfLoops = 9999,
                  position = new Vector2(22f, 22f) * 64f + new Vector2(-2f, 6f) * 4f,
                  scale = 4f,
                  layerDepth = 0.155519992f
                });
                location.TemporarySprites.Add(new TemporaryAnimatedSprite()
                {
                  texture = texture2D13,
                  sourceRect = new Microsoft.Xna.Framework.Rectangle(391, 235, 5, 13),
                  animationLength = 1,
                  sourceRectStartingPos = new Vector2(391f, 235f),
                  interval = 5000f,
                  totalNumberOfLoops = 9999,
                  position = new Vector2(21f, 22f) * 64f + new Vector2(8f, 4f) * 4f,
                  scale = 4f,
                  layerDepth = 0.155519992f
                });
                location.TemporarySprites.Add(new TemporaryAnimatedSprite()
                {
                  texture = texture2D13,
                  sourceRect = new Microsoft.Xna.Framework.Rectangle(399, 229, 11, 21),
                  animationLength = 1,
                  sourceRectStartingPos = new Vector2(399f, 229f),
                  interval = 5000f,
                  totalNumberOfLoops = 9999,
                  position = new Vector2(19f, 22f) * 64f + new Vector2(8f, -5f) * 4f,
                  scale = 4f,
                  layerDepth = 0.155519992f
                });
                location.temporarySprites.Add(new TemporaryAnimatedSprite(27, new Vector2(21f, 22f) * 64f + new Vector2(0.0f, -5f) * 4f, Color.White, 10)
                {
                  totalNumberOfLoops = 999,
                  layerDepth = 0.15616f
                });
                location.temporarySprites.Add(new TemporaryAnimatedSprite(27, new Vector2(21f, 22f) * 64f + new Vector2(24f, -5f) * 4f, Color.White, 10)
                {
                  totalNumberOfLoops = 999,
                  flipped = true,
                  delayBeforeAnimationStart = 400,
                  layerDepth = 0.15616f
                });
                return;
              default:
                return;
            }
          case 'i':
            if (!(key == "missingJunimoStars"))
              return;
            location.removeTemporarySpritesWithID(999);
            Texture2D texture2D14 = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\temporary_sprites_1");
            for (int index5 = 0; index5 < 48 /*0x30*/; ++index5)
              location.TemporarySprites.Add(new TemporaryAnimatedSprite()
              {
                texture = texture2D14,
                sourceRect = new Microsoft.Xna.Framework.Rectangle(477, 306, 28, 28),
                sourceRectStartingPos = new Vector2(477f, 306f),
                animationLength = 1,
                interval = 5000f,
                totalNumberOfLoops = 10,
                scale = (float) Game1.random.Next(1, 5),
                position = Utility.getTopLeftPositionForCenteringOnScreen(Game1.viewport, 84, 84) + new Vector2((float) Game1.random.Next(-32, 32 /*0x20*/), (float) Game1.random.Next(-32, 32 /*0x20*/)),
                rotationChange = 3.14159274f / (float) Game1.random.Next(16 /*0x10*/, 128 /*0x80*/),
                motion = new Vector2((float) Game1.random.Next(-30, 40) / 10f, (float) Game1.random.Next(20, 90) * -0.1f),
                acceleration = new Vector2(0.0f, 0.05f),
                local = true,
                layerDepth = (float) index5 / 100f,
                color = Game1.random.NextBool() ? Color.White : Utility.getRandomRainbowColor()
              });
            return;
          case 'm':
            if (!(key == "trashBearUmbrella1"))
              return;
            location.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors2", new Microsoft.Xna.Framework.Rectangle(0, 80 /*0x50*/, 46, 56), new Vector2(102f, 94.5f) * 64f, false, 0.0f, Color.White)
            {
              animationLength = 1,
              interval = 999999f,
              motion = new Vector2(0.0f, -9f),
              acceleration = new Vector2(0.0f, 0.4f),
              scale = 4f,
              layerDepth = 1f,
              id = 777,
              yStopCoordinate = 6144,
              reachedStopCoordinate = (TemporaryAnimatedSprite.endBehavior) (param =>
              {
                location.getTemporarySpriteByID(777).yStopCoordinate = -1;
                location.getTemporarySpriteByID(777).motion = new Vector2(0.0f, (float) param * 0.75f);
                location.getTemporarySpriteByID(777).acceleration = new Vector2(0.04f, -0.19f);
                location.getTemporarySpriteByID(777).accelerationChange = new Vector2(0.0f, 0.0015f);
                location.getTemporarySpriteByID(777).sourceRect.X += 46;
                location.playSound("batFlap");
                location.playSound("tinyWhip");
              })
            });
            return;
          case 'r':
            if (!(key == "sebastianFrogHouse"))
              return;
            Point spouseRoomCorner = (location as FarmHouse).GetSpouseRoomCorner();
            ++spouseRoomCorner.X;
            spouseRoomCorner.Y += 6;
            Vector2 vector2_7 = Utility.PointToVector2(spouseRoomCorner);
            location.TemporarySprites.Add(new TemporaryAnimatedSprite()
            {
              texture = Game1.mouseCursors,
              sourceRect = new Microsoft.Xna.Framework.Rectangle(641, 1534, 48 /*0x30*/, 37),
              animationLength = 1,
              sourceRectStartingPos = new Vector2(641f, 1534f),
              interval = 5000f,
              totalNumberOfLoops = 9999,
              position = vector2_7 * 64f + new Vector2(0.0f, -5f) * 4f,
              scale = 4f,
              layerDepth = (float) (((double) vector2_7.Y + 2.0 + 0.10000000149011612) * 64.0 / 10000.0)
            });
            Texture2D texture2D15 = Game1.temporaryContent.Load<Texture2D>("TileSheets\\critters");
            location.TemporarySprites.Add(new TemporaryAnimatedSprite()
            {
              texture = texture2D15,
              sourceRect = new Microsoft.Xna.Framework.Rectangle(0, 224 /*0xE0*/, 16 /*0x10*/, 16 /*0x10*/),
              animationLength = 1,
              sourceRectStartingPos = new Vector2(0.0f, 224f),
              interval = 5000f,
              totalNumberOfLoops = 9999,
              position = vector2_7 * 64f + new Vector2(25f, 2f) * 4f,
              scale = 4f,
              flipped = true,
              layerDepth = (float) (((double) vector2_7.Y + 2.0 + 0.10999999940395355) * 64.0 / 10000.0),
              id = 777
            });
            return;
          case 's':
            if (!(key == "farmerForestVision"))
              return;
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(393, 1973, 1, 1), 9999f, 1, 999999, new Vector2(0.0f, 0.0f) * 64f, false, false, 0.9f, 0.0f, Color.LimeGreen * 0.85f, (float) (Game1.viewport.Width * 2), 0.0f, 0.0f, 0.0f, true)
            {
              alpha = 0.0f,
              alphaFade = -1f / 500f,
              id = 1
            });
            Game1.player.mailReceived.Add("canReadJunimoText");
            int x2 = -64;
            int y2 = -64;
            int num125 = 0;
            int num126 = 0;
            while (y2 < Game1.viewport.Height + 128 /*0x80*/)
            {
              location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(367 + (num125 % 2 == 0 ? 8 : 0), 1969, 8, 8), 9999f, 1, 999999, new Vector2((float) x2, (float) y2), false, false, 0.99f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
              {
                alpha = 0.0f,
                alphaFade = -0.0015f,
                xPeriodic = true,
                xPeriodicLoopTime = 4000f,
                xPeriodicRange = 64f,
                yPeriodic = true,
                yPeriodicLoopTime = 5000f,
                yPeriodicRange = 96f,
                rotationChange = (float) ((double) Game1.random.Next(-1, 2) * 3.1415927410125732 / 256.0),
                id = 1,
                delayBeforeAnimationStart = 20 * num125
              });
              x2 += 128 /*0x80*/;
              if (x2 > Game1.viewport.Width + 64 /*0x40*/)
              {
                ++num126;
                x2 = num126 % 2 == 0 ? -64 : 64 /*0x40*/;
                y2 += 128 /*0x80*/;
              }
              ++num125;
            }
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(648, 895, 51, 101), 9999f, 1, 999999, new Vector2((float) (Game1.viewport.Width / 2 - 100), (float) (Game1.viewport.Height / 2 - 240 /*0xF0*/)), false, false, 1f, 0.0f, Color.White, 3f, 0.0f, 0.0f, 0.0f, true)
            {
              alpha = 0.0f,
              alphaFade = -1f / 1000f,
              id = 1,
              delayBeforeAnimationStart = 6000,
              scaleChange = 0.004f,
              xPeriodic = true,
              xPeriodicLoopTime = 4000f,
              xPeriodicRange = 64f,
              yPeriodic = true,
              yPeriodicLoopTime = 5000f,
              yPeriodicRange = 32f
            });
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(648, 895, 51, 101), 9999f, 1, 999999, new Vector2((float) (Game1.viewport.Width / 4 - 100), (float) (Game1.viewport.Height / 4 - 120)), false, false, 0.99f, 0.0f, Color.White, 3f, 0.0f, 0.0f, 0.0f, true)
            {
              alpha = 0.0f,
              alphaFade = -1f / 1000f,
              id = 1,
              delayBeforeAnimationStart = 9000,
              scaleChange = 0.004f,
              xPeriodic = true,
              xPeriodicLoopTime = 4000f,
              xPeriodicRange = 64f,
              yPeriodic = true,
              yPeriodicLoopTime = 5000f,
              yPeriodicRange = 32f
            });
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(648, 895, 51, 101), 9999f, 1, 999999, new Vector2((float) (Game1.viewport.Width * 3 / 4), (float) (Game1.viewport.Height / 3 - 120)), false, false, 0.98f, 0.0f, Color.White, 3f, 0.0f, 0.0f, 0.0f, true)
            {
              alpha = 0.0f,
              alphaFade = -1f / 1000f,
              id = 1,
              delayBeforeAnimationStart = 12000,
              scaleChange = 0.004f,
              xPeriodic = true,
              xPeriodicLoopTime = 4000f,
              xPeriodicRange = 64f,
              yPeriodic = true,
              yPeriodicLoopTime = 5000f,
              yPeriodicRange = 32f
            });
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(648, 895, 51, 101), 9999f, 1, 999999, new Vector2((float) (Game1.viewport.Width / 3 - 60), (float) (Game1.viewport.Height * 3 / 4 - 120)), false, false, 0.97f, 0.0f, Color.White, 3f, 0.0f, 0.0f, 0.0f, true)
            {
              alpha = 0.0f,
              alphaFade = -1f / 1000f,
              id = 1,
              delayBeforeAnimationStart = 15000,
              scaleChange = 0.004f,
              xPeriodic = true,
              xPeriodicLoopTime = 4000f,
              xPeriodicRange = 64f,
              yPeriodic = true,
              yPeriodicLoopTime = 5000f,
              yPeriodicRange = 32f
            });
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(648, 895, 51, 101), 9999f, 1, 999999, new Vector2((float) (Game1.viewport.Width * 2 / 3), (float) (Game1.viewport.Height * 2 / 3 - 120)), false, false, 0.96f, 0.0f, Color.White, 3f, 0.0f, 0.0f, 0.0f, true)
            {
              alpha = 0.0f,
              alphaFade = -1f / 1000f,
              id = 1,
              delayBeforeAnimationStart = 18000,
              scaleChange = 0.004f,
              xPeriodic = true,
              xPeriodicLoopTime = 4000f,
              xPeriodicRange = 64f,
              yPeriodic = true,
              yPeriodicLoopTime = 5000f,
              yPeriodicRange = 32f
            });
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(648, 895, 51, 101), 9999f, 1, 999999, new Vector2((float) (Game1.viewport.Width / 8), (float) (Game1.viewport.Height / 5 - 120)), false, false, 0.95f, 0.0f, Color.White, 3f, 0.0f, 0.0f, 0.0f, true)
            {
              alpha = 0.0f,
              alphaFade = -1f / 1000f,
              id = 1,
              delayBeforeAnimationStart = 19500,
              scaleChange = 0.004f,
              xPeriodic = true,
              xPeriodicLoopTime = 4000f,
              xPeriodicRange = 64f,
              yPeriodic = true,
              yPeriodicLoopTime = 5000f,
              yPeriodicRange = 32f
            });
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(648, 895, 51, 101), 9999f, 1, 999999, new Vector2((float) (Game1.viewport.Width * 2 / 3), (float) (Game1.viewport.Height / 5 - 120)), false, false, 0.94f, 0.0f, Color.White, 3f, 0.0f, 0.0f, 0.0f, true)
            {
              alpha = 0.0f,
              alphaFade = -1f / 1000f,
              id = 1,
              delayBeforeAnimationStart = 21000,
              scaleChange = 0.004f,
              xPeriodic = true,
              xPeriodicLoopTime = 4000f,
              xPeriodicRange = 64f,
              yPeriodic = true,
              yPeriodicLoopTime = 5000f,
              yPeriodicRange = 32f
            });
            return;
          case 't':
            if (!(key == "raccoonbutterflies"))
              return;
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\critters", new Microsoft.Xna.Framework.Rectangle(128 /*0x80*/, 336, 16 /*0x10*/, 16 /*0x10*/), new Vector2(52.5f, 0.0f) * 64f - new Vector2(131.5f, -60f) * 4f, false, 0.0f, Color.White)
            {
              drawAboveAlwaysFront = true,
              interval = 148f,
              animationLength = 4,
              pingPong = true,
              totalNumberOfLoops = 99999,
              id = 997799,
              scale = 4f,
              xPeriodic = true,
              xPeriodicRange = 32f,
              xPeriodicLoopTime = 2800f,
              alpha = 0.01f,
              alphaFade = -0.01f,
              yPeriodic = true,
              yPeriodicRange = 8f,
              yPeriodicLoopTime = 3800f,
              overrideLocationDestroy = true
            });
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\critters", new Microsoft.Xna.Framework.Rectangle(192 /*0xC0*/, 336, 16 /*0x10*/, 16 /*0x10*/), new Vector2(56.5f, 0.0f) * 64f - new Vector2(131.5f, 0.0f) * 4f, false, 0.0f, Color.White)
            {
              drawAboveAlwaysFront = true,
              interval = 148f,
              animationLength = 4,
              pingPong = true,
              totalNumberOfLoops = 99999,
              id = 997799,
              scale = 4f,
              xPeriodic = true,
              xPeriodicRange = 32f,
              xPeriodicLoopTime = 2600f,
              alpha = 0.01f,
              alphaFade = -0.01f,
              yPeriodic = true,
              yPeriodicRange = 4f,
              yPeriodicLoopTime = 2900f
            });
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\critters", new Microsoft.Xna.Framework.Rectangle(128 /*0x80*/, 288, 16 /*0x10*/, 16 /*0x10*/), new Vector2(53.5f, 0.0f) * 64f + new Vector2(263f, 24f) * 4f, false, 0.0f, Color.White)
            {
              drawAboveAlwaysFront = true,
              interval = 148f,
              animationLength = 4,
              pingPong = true,
              totalNumberOfLoops = 99999,
              id = 997799,
              scale = 4f,
              xPeriodic = true,
              xPeriodicRange = 32f,
              xPeriodicLoopTime = 3000f,
              alpha = 0.01f,
              alphaFade = -0.01f,
              yPeriodic = true,
              yPeriodicRange = 6f,
              yPeriodicLoopTime = 3100f
            });
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\critters", new Microsoft.Xna.Framework.Rectangle(192 /*0xC0*/, 288, 16 /*0x10*/, 16 /*0x10*/), new Vector2(52.5f, 0.0f) * 64f + new Vector2(131.5f, 220f) * 4f, false, 0.0f, Color.White)
            {
              drawAboveAlwaysFront = true,
              interval = 148f,
              animationLength = 4,
              pingPong = true,
              totalNumberOfLoops = 99999,
              id = 997799,
              scale = 4f,
              xPeriodic = true,
              xPeriodicRange = 32f,
              xPeriodicLoopTime = 2400f,
              alpha = 0.01f,
              alphaFade = -0.01f,
              yPeriodic = true,
              yPeriodicRange = 12f,
              yPeriodicLoopTime = 2800f
            });
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\critters", new Microsoft.Xna.Framework.Rectangle(64 /*0x40*/, 288, 16 /*0x10*/, 16 /*0x10*/), new Vector2(52.5f, 0.0f) * 64f + new Vector2(186.5f, 150f) * 4f, false, 0.0f, Color.White)
            {
              drawAboveAlwaysFront = true,
              interval = 148f,
              animationLength = 4,
              pingPong = true,
              totalNumberOfLoops = 99999,
              id = 997799,
              scale = 4f,
              xPeriodic = true,
              xPeriodicRange = 32f,
              xPeriodicLoopTime = 3400f,
              alpha = 0.01f,
              alphaFade = -0.01f,
              yPeriodic = true,
              yPeriodicRange = 4f,
              yPeriodicLoopTime = 3200f
            });
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\critters", new Microsoft.Xna.Framework.Rectangle(128 /*0x80*/, 96 /*0x60*/, 16 /*0x10*/, 16 /*0x10*/), new Vector2(52.5f, 0.0f) * 64f + new Vector2(211.5f, 180f) * 4f, false, 0.0f, Color.White)
            {
              drawAboveAlwaysFront = true,
              interval = 148f,
              animationLength = 4,
              pingPong = true,
              totalNumberOfLoops = 99999,
              id = 997799,
              scale = 4f,
              xPeriodic = true,
              xPeriodicRange = 32f,
              xPeriodicLoopTime = 3500f,
              alpha = 0.01f,
              alphaFade = -0.01f,
              yPeriodic = true,
              yPeriodicRange = 4f,
              yPeriodicLoopTime = 2700f
            });
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\critters", new Microsoft.Xna.Framework.Rectangle(192 /*0xC0*/, 112 /*0x70*/, 16 /*0x10*/, 16 /*0x10*/), new Vector2(52.5f, 0.0f) * 64f - new Vector2(126.5f, -120f) * 4f, false, 0.0f, Color.White)
            {
              drawAboveAlwaysFront = true,
              interval = 148f,
              animationLength = 4,
              pingPong = true,
              totalNumberOfLoops = 99999,
              id = 997799,
              scale = 4f,
              xPeriodic = true,
              xPeriodicRange = 16f,
              xPeriodicLoopTime = 2500f,
              alpha = 0.01f,
              alphaFade = -0.01f,
              yPeriodic = true,
              yPeriodicRange = 4f,
              yPeriodicLoopTime = 3300f
            });
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\critters", new Microsoft.Xna.Framework.Rectangle(128 /*0x80*/, 288, 16 /*0x10*/, 16 /*0x10*/), new Vector2(49.5f, 0.0f) * 64f - new Vector2(126.5f, -100f) * 4f, false, 0.0f, Color.White)
            {
              drawAboveAlwaysFront = true,
              interval = 148f,
              animationLength = 4,
              pingPong = true,
              totalNumberOfLoops = 99999,
              id = 997799,
              scale = 4f,
              xPeriodic = true,
              xPeriodicRange = 16f,
              xPeriodicLoopTime = 2200f,
              alpha = 0.01f,
              alphaFade = -0.01f,
              yPeriodic = true,
              yPeriodicRange = 4f,
              yPeriodicLoopTime = 3400f
            });
            TemporaryAnimatedSprite temporarySpriteById8 = location.getTemporarySpriteByID(9786);
            TemporaryAnimatedSprite temporarySpriteById9 = location.getTemporarySpriteByID(9785);
            temporarySpriteById8.sourceRect.Y = 224 /*0xE0*/;
            temporarySpriteById8.sourceRectStartingPos.Y = 224f;
            temporarySpriteById8.currentParentTileIndex = 3;
            temporarySpriteById8.timer = 0.0f;
            temporarySpriteById8.sourceRect.X = 96 /*0x60*/;
            temporarySpriteById9.sourceRect.Y = 224 /*0xE0*/;
            temporarySpriteById9.sourceRectStartingPos.Y = 224f;
            temporarySpriteById9.currentParentTileIndex = 3;
            temporarySpriteById9.timer = 0.0f;
            temporarySpriteById9.sourceRect.X = 96 /*0x60*/;
            return;
          default:
            return;
        }
      case 19:
        switch (key[0])
        {
          case 'E':
            if (!(key == "EmilySongBackLights"))
              return;
            this.aboveMapSprites = new TemporaryAnimatedSpriteList();
            Viewport viewport4;
            for (int index6 = 0; index6 < 5; ++index6)
            {
              int num127 = 0;
              while (true)
              {
                int num128 = num127;
                viewport4 = Game1.graphics.GraphicsDevice.Viewport;
                int num129 = viewport4.Height + 48 /*0x30*/;
                if (num128 < num129)
                {
                  TemporaryAnimatedSpriteList aboveMapSprites = this.aboveMapSprites;
                  Microsoft.Xna.Framework.Rectangle sourceRect5 = new Microsoft.Xna.Framework.Rectangle(681, 1890, 18, 12);
                  int num130 = index6 + 1;
                  viewport4 = Game1.graphics.GraphicsDevice.Viewport;
                  int width = viewport4.Width;
                  int num131 = num130 * width / 5;
                  viewport4 = Game1.graphics.GraphicsDevice.Viewport;
                  int num132 = viewport4.Width / 7;
                  Vector2 position6 = new Vector2((float) (num131 - num132), (float) (num127 - 24));
                  Color white4 = Color.White;
                  aboveMapSprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", sourceRect5, 42241f, 1, 1, position6, false, false, 0.01f, 0.0f, white4, 4f, 0.0f, 0.0f, 0.0f)
                  {
                    xPeriodic = true,
                    xPeriodicLoopTime = 1760f,
                    xPeriodicRange = (float) (128 /*0x80*/ + num127 / 12 * 4),
                    delayBeforeAnimationStart = index6 * 100 + num127 / 4,
                    local = true
                  });
                  num127 += 48 /*0x30*/;
                }
                else
                  break;
              }
            }
            for (int index7 = 0; index7 < 27; ++index7)
            {
              int num133 = 0;
              Random random = Game1.random;
              viewport4 = Game1.graphics.GraphicsDevice.Viewport;
              int maxValue = viewport4.Height - 64 /*0x40*/;
              int y3 = random.Next(64 /*0x40*/, maxValue);
              int num134 = Game1.random.Next(800, 2000);
              int num135 = Game1.random.Next(32 /*0x20*/, 64 /*0x40*/);
              bool flag = Game1.random.NextDouble() < 0.25;
              int x3 = Game1.random.Next(-6, -3);
              for (int index8 = 0; index8 < 8; ++index8)
              {
                TemporaryAnimatedSpriteList aboveMapSprites = this.aboveMapSprites;
                Microsoft.Xna.Framework.Rectangle sourceRect6 = new Microsoft.Xna.Framework.Rectangle(616 + num133 * 10, 1891, 10, 10);
                viewport4 = Game1.graphics.GraphicsDevice.Viewport;
                Vector2 position7 = new Vector2((float) viewport4.Width, (float) y3);
                Color color = Color.White * (float) (1.0 - (double) index8 * 0.10999999940395355);
                aboveMapSprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", sourceRect6, 42241f, 1, 1, position7, false, false, 0.01f, 0.0f, color, 4f, 0.0f, 0.0f, 0.0f)
                {
                  yPeriodic = true,
                  motion = new Vector2((float) x3, 0.0f),
                  yPeriodicLoopTime = (float) num134,
                  pulse = flag,
                  pulseTime = 440f,
                  pulseAmount = 1.5f,
                  yPeriodicRange = (float) num135,
                  delayBeforeAnimationStart = 14000 + index7 * 900 + index8 * 100,
                  local = true
                });
              }
            }
            for (int index9 = 0; index9 < 15; ++index9)
            {
              int num136 = 0;
              Random random = Game1.random;
              viewport4 = Game1.graphics.GraphicsDevice.Viewport;
              int maxValue = viewport4.Width - 128 /*0x80*/;
              int x4 = random.Next(maxValue);
              viewport4 = Game1.graphics.GraphicsDevice.Viewport;
              for (int height = viewport4.Height; height >= -64; height -= 48 /*0x30*/)
              {
                this.aboveMapSprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(597, 1888, 16 /*0x10*/, 16 /*0x10*/), 99999f, 1, 99999, new Vector2((float) x4, (float) height), false, false, 1f, 0.02f, Color.White, 4f, 0.0f, -1.57079637f, 0.0f)
                {
                  delayBeforeAnimationStart = 27500 + index9 * 880 + num136 * 25,
                  local = true
                });
                ++num136;
              }
            }
            for (int index10 = 0; index10 < 120; ++index10)
            {
              TemporaryAnimatedSpriteList aboveMapSprites = this.aboveMapSprites;
              Microsoft.Xna.Framework.Rectangle sourceRect7 = new Microsoft.Xna.Framework.Rectangle(626 + index10 / 28 * 10, 1891, 10, 10);
              Random random1 = Game1.random;
              viewport4 = Game1.graphics.GraphicsDevice.Viewport;
              int width = viewport4.Width;
              double x5 = (double) random1.Next(width);
              Random random2 = Game1.random;
              viewport4 = Game1.graphics.GraphicsDevice.Viewport;
              int height = viewport4.Height;
              double y4 = (double) random2.Next(height);
              Vector2 position8 = new Vector2((float) x5, (float) y4);
              Color white5 = Color.White;
              aboveMapSprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", sourceRect7, 2000f, 1, 1, position8, false, false, 0.01f, 0.0f, white5, 0.1f, 0.0f, 0.0f, 0.0f)
              {
                motion = new Vector2(0.0f, -2f),
                alphaFade = 1f / 500f,
                scaleChange = 0.5f,
                scaleChangeChange = -0.0085f,
                delayBeforeAnimationStart = 27500 + index10 * 110,
                local = true
              });
            }
            return;
          case 'm':
            if (!(key == "movieTheater_screen"))
              return;
            string id3;
            string error7;
            int frame2;
            bool flag1;
            if (!ArgUtility.TryGet(args, 2, out id3, out error7, name: "string movieId") || !ArgUtility.TryGetInt(args, 3, out frame2, out error7, "int screenIndex") || !ArgUtility.TryGetBool(args, 4, out flag1, out error7, "bool shake"))
            {
              this.LogCommandError(args, error7);
              return;
            }
            string idFromLegacyIndex2 = MovieTheater.GetMovieIdFromLegacyIndex(id3);
            MovieData data3;
            if (!MovieTheater.TryGetMovieData(idFromLegacyIndex2, out data3))
            {
              this.LogCommandError(args, $"No movie found with ID '{idFromLegacyIndex2}'.");
              return;
            }
            Microsoft.Xna.Framework.Rectangle sourceRectForScreen3 = MovieTheater.GetSourceRectForScreen(data3.SheetIndex, frame2);
            location.removeTemporarySpritesWithIDLocal(998);
            if (frame2 < 0)
              return;
            location.temporarySprites.Add(new TemporaryAnimatedSprite()
            {
              texture = Game1.temporaryContent.Load<Texture2D>(data3.Texture ?? "LooseSprites\\Movies"),
              sourceRect = sourceRectForScreen3,
              sourceRectStartingPos = new Vector2((float) sourceRectForScreen3.X, (float) sourceRectForScreen3.Y),
              animationLength = 1,
              totalNumberOfLoops = 9999,
              interval = 5000f,
              scale = 4f,
              position = new Vector2(4f, 1f) * 64f + new Vector2(3f, 7f) * 4f,
              shakeIntensity = flag1 ? 1f : 0.0f,
              layerDepth = 0.0128f,
              id = 998
            });
            return;
          case 't':
            if (!(key == "terraria_warp_begin"))
              return;
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\terraria_cat", new Microsoft.Xna.Framework.Rectangle(0, 18, 36, 68), 90f, 3, 9999, new Vector2(16f, 5f) * 64f + new Vector2(0.0f, -50f) * 4f, false, false, 0.8f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
            {
              layerDepth = 0.8f,
              id = 888
            });
            TemporaryAnimatedSprite cat_sprite = new TemporaryAnimatedSprite("LooseSprites\\terraria_cat", new Microsoft.Xna.Framework.Rectangle(0, 0, 16 /*0x10*/, 16 /*0x10*/), 90f, 8, 9999, new Vector2(16f, 5f) * 64f + new Vector2(34f, -12f) * 4f, false, false, 0.8f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
            {
              id = 777,
              layerDepth = 0.85f,
              motion = new Vector2(-1f, 0.0f),
              delayBeforeAnimationStart = 1000,
              xStopCoordinate = 960
            };
            cat_sprite.reachedStopCoordinate = (TemporaryAnimatedSprite.endBehavior) (param =>
            {
              cat_sprite.paused = true;
              cat_sprite.sourceRect = new Microsoft.Xna.Framework.Rectangle(112 /*0x70*/, 16 /*0x10*/, 16 /*0x10*/, 16 /*0x10*/);
              DelayedAction.functionAfterDelay((Action) (() =>
              {
                Game1.playSound("terraria_meowmere");
                cat_sprite.shakeIntensity = 1f;
                location.TemporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\weapons", new Microsoft.Xna.Framework.Rectangle(16 /*0x10*/, 128 /*0x80*/, 16 /*0x10*/, 16 /*0x10*/), 1000f, 1, 1, new Vector2(15f, 5f) * 64f, false, false, 0.8f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
                {
                  layerDepth = 0.86f,
                  motion = new Vector2(-1f, -4f),
                  acceleration = new Vector2(0.0f, 0.1f)
                });
              }), 1000);
              DelayedAction.functionAfterDelay((Action) (() => cat_sprite.shakeIntensity = 0.0f), 1300);
            });
            location.TemporarySprites.Add(cat_sprite);
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\terraria_cat", new Microsoft.Xna.Framework.Rectangle(4, 88, 19, 15), 90f, 3, 9999, new Vector2(16f, 5f) * 64f + new Vector2(31f, -10f) * 4f, false, false, 0.8f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
            {
              layerDepth = 0.9f,
              id = 888
            });
            Microsoft.Xna.Framework.Rectangle r = new Microsoft.Xna.Framework.Rectangle(1024 /*0x0400*/, 120, 144 /*0x90*/, 272);
            for (int index11 = 0; index11 < 80 /*0x50*/; ++index11)
            {
              Vector2 positionInThisRectangle = Utility.getRandomPositionInThisRectangle(r, Game1.random);
              Vector2 vector2_8 = positionInThisRectangle - Utility.PointToVector2(r.Center);
              vector2_8.Normalize();
              Vector2 vector2_9 = vector2_8 * (float) (Game1.random.Next(10, 21) / 10);
              location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\terraria_cat", new Microsoft.Xna.Framework.Rectangle(113 + Game1.random.Next(3) * 5, 123, 5, 5), 999f, 1, 9999, positionInThisRectangle, false, false, 0.8f, 0.02f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
              {
                layerDepth = 0.99f,
                rotationChange = (float) Game1.random.Next(-10, 10) / 100f,
                motion = vector2_9,
                acceleration = -vector2_9 / 150f,
                scaleChange = (float) Game1.random.Next(-10, 0) / 500f,
                delayBeforeAnimationStart = index11 * 5
              });
            }
            return;
          case 'w':
            if (!(key == "willyCrabExperiment"))
              return;
            Texture2D texture2D16 = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\temporary_sprites_1");
            location.TemporarySprites.Add(new TemporaryAnimatedSprite()
            {
              texture = texture2D16,
              sourceRect = new Microsoft.Xna.Framework.Rectangle(259, (int) sbyte.MaxValue, 18, 18),
              animationLength = 3,
              sourceRectStartingPos = new Vector2(259f, (float) sbyte.MaxValue),
              pingPong = true,
              interval = 250f,
              totalNumberOfLoops = 99999,
              id = 11,
              position = new Vector2(2f, 4f) * 64f,
              scale = 4f
            });
            location.TemporarySprites.Add(new TemporaryAnimatedSprite()
            {
              texture = texture2D16,
              sourceRect = new Microsoft.Xna.Framework.Rectangle(259, 146, 18, 18),
              animationLength = 3,
              sourceRectStartingPos = new Vector2(259f, 146f),
              pingPong = true,
              interval = 200f,
              totalNumberOfLoops = 99999,
              id = 1,
              initialPosition = new Vector2(2f, 6f) * 64f,
              yPeriodic = true,
              yPeriodicLoopTime = 8000f,
              yPeriodicRange = 32f,
              position = new Vector2(2f, 6f) * 64f,
              scale = 4f
            });
            location.TemporarySprites.Add(new TemporaryAnimatedSprite()
            {
              texture = texture2D16,
              sourceRect = new Microsoft.Xna.Framework.Rectangle(259, (int) sbyte.MaxValue, 18, 18),
              animationLength = 3,
              sourceRectStartingPos = new Vector2(259f, (float) sbyte.MaxValue),
              pingPong = true,
              interval = 100f,
              totalNumberOfLoops = 99999,
              id = 11,
              position = new Vector2(1f, 5.75f) * 64f,
              scale = 4f
            });
            location.TemporarySprites.Add(new TemporaryAnimatedSprite()
            {
              texture = texture2D16,
              sourceRect = new Microsoft.Xna.Framework.Rectangle(259, (int) sbyte.MaxValue, 18, 18),
              animationLength = 3,
              sourceRectStartingPos = new Vector2(259f, (float) sbyte.MaxValue),
              pingPong = true,
              interval = 100f,
              totalNumberOfLoops = 99999,
              id = 11,
              position = new Vector2(5f, 3f) * 64f,
              scale = 4f
            });
            location.TemporarySprites.Add(new TemporaryAnimatedSprite()
            {
              texture = texture2D16,
              sourceRect = new Microsoft.Xna.Framework.Rectangle(259, (int) sbyte.MaxValue, 18, 18),
              animationLength = 3,
              sourceRectStartingPos = new Vector2(259f, (float) sbyte.MaxValue),
              pingPong = true,
              interval = 140f,
              totalNumberOfLoops = 99999,
              id = 22,
              position = new Vector2(4f, 6f) * 64f,
              scale = 4f
            });
            location.TemporarySprites.Add(new TemporaryAnimatedSprite()
            {
              texture = texture2D16,
              sourceRect = new Microsoft.Xna.Framework.Rectangle(259, (int) sbyte.MaxValue, 18, 18),
              animationLength = 3,
              sourceRectStartingPos = new Vector2(259f, (float) sbyte.MaxValue),
              pingPong = true,
              interval = 140f,
              totalNumberOfLoops = 99999,
              id = 22,
              position = new Vector2(8.5f, 5f) * 64f,
              scale = 4f
            });
            location.TemporarySprites.Add(new TemporaryAnimatedSprite()
            {
              texture = texture2D16,
              sourceRect = new Microsoft.Xna.Framework.Rectangle(259, 146, 18, 18),
              animationLength = 3,
              sourceRectStartingPos = new Vector2(259f, 146f),
              pingPong = true,
              interval = 170f,
              totalNumberOfLoops = 99999,
              id = 222,
              position = new Vector2(6f, 3.25f) * 64f,
              scale = 4f
            });
            location.TemporarySprites.Add(new TemporaryAnimatedSprite()
            {
              texture = texture2D16,
              sourceRect = new Microsoft.Xna.Framework.Rectangle(259, 146, 18, 18),
              animationLength = 3,
              sourceRectStartingPos = new Vector2(259f, 146f),
              pingPong = true,
              interval = 190f,
              totalNumberOfLoops = 99999,
              id = 222,
              position = new Vector2(6f, 6f) * 64f,
              scale = 4f
            });
            location.TemporarySprites.Add(new TemporaryAnimatedSprite()
            {
              texture = texture2D16,
              sourceRect = new Microsoft.Xna.Framework.Rectangle(259, 146, 18, 18),
              animationLength = 3,
              sourceRectStartingPos = new Vector2(259f, 146f),
              pingPong = true,
              interval = 150f,
              totalNumberOfLoops = 99999,
              id = 222,
              position = new Vector2(7f, 4f) * 64f,
              scale = 4f
            });
            location.TemporarySprites.Add(new TemporaryAnimatedSprite()
            {
              texture = texture2D16,
              sourceRect = new Microsoft.Xna.Framework.Rectangle(259, 146, 18, 18),
              animationLength = 3,
              sourceRectStartingPos = new Vector2(259f, 146f),
              pingPong = true,
              interval = 200f,
              totalNumberOfLoops = 99999,
              id = 2,
              position = new Vector2(4f, 7f) * 64f,
              scale = 4f
            });
            location.TemporarySprites.Add(new TemporaryAnimatedSprite()
            {
              texture = texture2D16,
              sourceRect = new Microsoft.Xna.Framework.Rectangle(259, (int) sbyte.MaxValue, 18, 18),
              animationLength = 3,
              sourceRectStartingPos = new Vector2(259f, (float) sbyte.MaxValue),
              pingPong = true,
              interval = 180f,
              totalNumberOfLoops = 99999,
              id = 3,
              position = new Vector2(8f, 6f) * 64f,
              yPeriodic = true,
              yPeriodicLoopTime = 10000f,
              yPeriodicRange = 32f,
              initialPosition = new Vector2(8f, 6f) * 64f,
              scale = 4f
            });
            location.TemporarySprites.Add(new TemporaryAnimatedSprite()
            {
              texture = texture2D16,
              sourceRect = new Microsoft.Xna.Framework.Rectangle(259, 146, 18, 18),
              animationLength = 3,
              sourceRectStartingPos = new Vector2(259f, 146f),
              pingPong = true,
              interval = 220f,
              totalNumberOfLoops = 99999,
              id = 33,
              position = new Vector2(9f, 6f) * 64f,
              scale = 4f
            });
            location.TemporarySprites.Add(new TemporaryAnimatedSprite()
            {
              texture = texture2D16,
              sourceRect = new Microsoft.Xna.Framework.Rectangle(259, 146, 18, 18),
              animationLength = 3,
              sourceRectStartingPos = new Vector2(259f, 146f),
              pingPong = true,
              interval = 150f,
              totalNumberOfLoops = 99999,
              id = 33,
              position = new Vector2(10f, 5f) * 64f,
              scale = 4f
            });
            return;
          default:
            return;
        }
      case 20:
        if (!(key == "BoatParrotSquawkStop"))
          break;
        TemporaryAnimatedSprite aboveMapSprite5 = this.aboveMapSprites[0];
        aboveMapSprite5.sourceRect.X = 0;
        aboveMapSprite5.sourceRectStartingPos.X = 0.0f;
        break;
      case 23:
        if (!(key == "leahStopHoldingPainting"))
          break;
        location.getTemporarySpriteByID(999).sourceRect.X -= 15;
        location.getTemporarySpriteByID(999).sourceRectStartingPos.X -= 15f;
        location.removeTemporarySpritesWithIDLocal(777);
        Game1.playSound("thudStep");
        break;
    }
  }

  private Microsoft.Xna.Framework.Rectangle skipBounds()
  {
    int num = 4;
    int width = 22 * num;
    Microsoft.Xna.Framework.Rectangle bounds = new Microsoft.Xna.Framework.Rectangle(Game1.viewport.Width - width - 8, Game1.viewport.Height - 64 /*0x40*/, width, 15 * num);
    Utility.makeSafe(ref bounds);
    return bounds;
  }

  public void receiveMouseClick(int x, int y)
  {
    if (!Game1.options.SnappyMenus && !this.skipped && this.skippable && this.skipBounds().Contains(x, y))
    {
      this.skipped = true;
      this.skipEvent();
      Game1.freezeControls = false;
    }
    this.popBalloons(x, y);
  }

  public void skipEvent()
  {
    if (this.playerControlSequence)
      this.EndPlayerControlSequence();
    Game1.playSound("drumkit6");
    this.actorPositionsAfterMove.Clear();
    foreach (NPC actor in this.actors)
    {
      bool ignoreStopAnimation = actor.Sprite.ignoreStopAnimation;
      actor.Sprite.ignoreStopAnimation = true;
      actor.Halt();
      actor.Sprite.ignoreStopAnimation = ignoreStopAnimation;
      this.resetDialogueIfNecessary(actor);
    }
    this.farmer.Halt();
    this.farmer.ignoreCollisions = false;
    Game1.exitActiveMenu();
    Game1.fadeClear();
    Game1.dialogueUp = false;
    Game1.dialogueTyping = false;
    Game1.pauseTime = 0.0f;
    string[] actionsOnSkip = this.actionsOnSkip;
    if ((actionsOnSkip != null ? (actionsOnSkip.Length != 0 ? 1 : 0) : 0) != 0)
    {
      foreach (string action in this.actionsOnSkip)
      {
        string error;
        Exception exception;
        if (!TriggerActionManager.TryRunAction(action, out error, out exception))
          Game1.log.Error($"Event '{this.id}' failed applying post-skip action '{action}': {error}.", exception);
      }
      Game1.log.Verbose($"Event '{this.id}' applied post-skip actions [{string.Join(", ", this.actionsOnSkip)}].");
    }
    string id = this.id;
    if (id != null)
    {
      switch (id.Length)
      {
        case 2:
          switch (id[0])
          {
            case '1':
              if (id == "19")
              {
                Game1.player.cookingRecipes.TryAdd("Cookies", 0);
                this.endBehaviors();
                return;
              }
              goto label_95;
            case '2':
              if (id == "26")
              {
                Game1.player.craftingRecipes.TryAdd("Wild Bait", 0);
                this.endBehaviors();
                return;
              }
              goto label_95;
            case '3':
              if (id == "33")
              {
                Game1.player.craftingRecipes.TryAdd("Drum Block", 0);
                Game1.player.craftingRecipes.TryAdd("Flute Block", 0);
                this.endBehaviors();
                return;
              }
              goto label_95;
            default:
              goto label_95;
          }
        case 3:
          if (id == "112")
          {
            this.endBehaviors();
            Game1.player.mailReceived.Add("canReadJunimoText");
            return;
          }
          goto label_95;
        case 5:
          if (id == "60367")
          {
            this.endBehaviors(new string[2]
            {
              "End",
              "beginGame"
            }, Game1.currentLocation);
            return;
          }
          goto label_95;
        case 6:
          switch (id[0])
          {
            case '-':
              if (id == "-78765")
              {
                this.endBehaviors(new string[2]
                {
                  "End",
                  "tunnelDepart"
                }, Game1.currentLocation);
                return;
              }
              goto label_95;
            case '1':
              switch (id)
              {
                case "191393":
                  if (!Game1.player.Items.ContainsId("(BC)116"))
                    Game1.player.addItemByMenuIfNecessary(ItemRegistry.Create("(BC)116"));
                  this.endBehaviors(new string[4]
                  {
                    "End",
                    "position",
                    "52",
                    "20"
                  }, Game1.currentLocation);
                  return;
                case "100162":
                  if (!Game1.player.Items.ContainsId("(W)0"))
                    Game1.player.addItemByMenuIfNecessary(ItemRegistry.Create("(W)0"));
                  Game1.player.Position = new Vector2(-9999f, -99999f);
                  this.endBehaviors();
                  return;
                default:
                  goto label_95;
              }
            case '4':
              if (id == "404798")
              {
                if (!Game1.player.Items.ContainsId("(T)Pan"))
                  Game1.player.addItemByMenuIfNecessary(ItemRegistry.Create("(T)Pan"));
                this.endBehaviors();
                return;
              }
              goto label_95;
            case '5':
              if (id == "558292")
              {
                Game1.player.eventsSeen.Remove("2146991");
                this.endBehaviors(new string[2]
                {
                  "End",
                  "bed"
                }, Game1.currentLocation);
                return;
              }
              goto label_95;
            case '6':
              switch (id)
              {
                case "690006":
                  if (!Game1.player.Items.ContainsId("(O)680"))
                    Game1.player.addItemByMenuIfNecessary(ItemRegistry.Create("(O)680"));
                  this.endBehaviors();
                  return;
                case "611173":
                  if (!Game1.player.activeDialogueEvents.ContainsKey("pamHouseUpgradeAnonymous"))
                    Game1.player.activeDialogueEvents.TryAdd("pamHouseUpgrade", 4);
                  this.endBehaviors();
                  return;
                default:
                  goto label_95;
              }
            case '7':
              if (id == "739330")
              {
                if (!Game1.player.Items.ContainsId("(T)BambooPole"))
                  Game1.player.addItemByMenuIfNecessary(ItemRegistry.Create("(T)BambooPole"));
                this.endBehaviors(new string[4]
                {
                  "End",
                  "position",
                  "43",
                  "36"
                }, Game1.currentLocation);
                return;
              }
              goto label_95;
            case '8':
              if (id == "897405")
                break;
              goto label_95;
            case '9':
              switch (id)
              {
                case "980559":
                  if (Game1.player.GetSkillLevel(1) < 1)
                    Game1.player.setSkillLevel("Fishing", 1);
                  if (!Game1.player.Items.ContainsId("(T)TrainingRod"))
                    Game1.player.addItemByMenuIfNecessary(ItemRegistry.Create("(T)TrainingRod"));
                  this.endBehaviors();
                  return;
                case "992553":
                  Game1.player.craftingRecipes.TryAdd("Furnace", 0);
                  Game1.player.addQuest("11");
                  this.endBehaviors();
                  return;
                case "900553":
                  Game1.player.craftingRecipes.TryAdd("Garden Pot", 0);
                  if (!Game1.player.Items.ContainsId("(BC)62"))
                    Game1.player.addItemByMenuIfNecessary(ItemRegistry.Create("(BC)62"));
                  this.endBehaviors();
                  return;
                case "980558":
                  Game1.player.craftingRecipes.TryAdd("Mini-Jukebox", 0);
                  if (!Game1.player.Items.ContainsId("(BC)209"))
                    Game1.player.addItemByMenuIfNecessary(ItemRegistry.Create("(BC)209"));
                  this.endBehaviors();
                  return;
                default:
                  goto label_95;
              }
            default:
              goto label_95;
          }
          break;
        case 7:
          switch (id[1])
          {
            case '0':
              if (id == "3091462")
              {
                if (!Game1.player.Items.ContainsId("(F)1802"))
                  Game1.player.addItemByMenuIfNecessary(ItemRegistry.Create("(F)1802"));
                this.endBehaviors();
                return;
              }
              goto label_95;
            case '1':
              if (id == "2123343")
              {
                this.endBehaviors(new string[2]
                {
                  "End",
                  "newDay"
                }, Game1.currentLocation);
                return;
              }
              goto label_95;
            case '4':
              if (id == "6497428")
              {
                this.endBehaviors(new string[2]
                {
                  "End",
                  "Leo"
                }, Game1.currentLocation);
                return;
              }
              goto label_95;
            case '5':
              if (id == "1590166")
                break;
              goto label_95;
            case '6':
              if (id == "-666777")
              {
                if (!Game1.netWorldState.Value.ActivatedGoldenParrot)
                  Game1.player.team.RequestLimitedNutDrops("Birdie", (GameLocation) null, 0, 0, 5, 5);
                if (!Game1.MasterPlayer.hasOrWillReceiveMail("gotBirdieReward"))
                  Game1.addMailForTomorrow("gotBirdieReward", true, true);
                Game1.player.craftingRecipes.TryAdd("Fairy Dust", 0);
                this.endBehaviors();
                return;
              }
              goto label_95;
            case '8':
              if (id == "-888999")
              {
                Object @object = ItemRegistry.Create<Object>("(O)864");
                @object.specialItem = true;
                @object.questItem.Value = true;
                Game1.player.addItemByMenuIfNecessary((Item) @object);
                Game1.player.addQuest("130");
                this.endBehaviors();
                return;
              }
              goto label_95;
            case '9':
              if (id == "3918602")
              {
                if (!Game1.player.Items.ContainsId("(F)1309"))
                  Game1.player.addItemByMenuIfNecessary(ItemRegistry.Create("(F)1309"));
                this.endBehaviors();
                return;
              }
              goto label_95;
            default:
              goto label_95;
          }
          break;
        case 10:
          if (id == "-157039427")
          {
            this.endBehaviors(new string[2]
            {
              "End",
              "islandDepart"
            }, Game1.currentLocation);
            return;
          }
          goto label_95;
        default:
          goto label_95;
      }
      if (!this.gotPet)
        this.namePet(!Game1.player.IsMale ? Game1.content.LoadString(Game1.player.whichPetType == "Dog" ? "Strings\\StringsFromCSFiles:Event.cs.1797" : "Strings\\StringsFromCSFiles:Event.cs.1796") : Game1.content.LoadString(Game1.player.catPerson ? "Strings\\StringsFromCSFiles:Event.cs.1794" : "Strings\\StringsFromCSFiles:Event.cs.1795"));
      this.endBehaviors();
      return;
    }
label_95:
    this.endBehaviors();
  }

  public void receiveActionPress(int xTile, int yTile)
  {
    if (xTile != this.playerControlTargetTile.X || yTile != this.playerControlTargetTile.Y)
      return;
    switch (this.playerControlSequenceID)
    {
      case "haleyBeach":
        this.props.Clear();
        Game1.playSound("coin");
        this.playerControlTargetTile = new Point(35, 11);
        this.playerControlSequenceID = "haleyBeach2";
        break;
      case "haleyBeach2":
        this.EndPlayerControlSequence();
        ++this.CurrentCommand;
        break;
    }
  }

  public void startSecretSantaEvent()
  {
    this.playerControlSequence = false;
    this.playerControlSequenceID = (string) null;
    string data;
    if (!this.TryGetFestivalDataForYear("secretSanta", out data))
      Game1.log.Error($"Festival {this.id} doesn't have the required 'secretSanta' data field.");
    this.eventCommands = Event.ParseCommands(data);
    this.doingSecretSanta = true;
    this.setUpSecretSantaCommands();
    this.currentCommand = 0;
  }

  public void festivalUpdate(GameTime time)
  {
    Game1.player.team.festivalScoreStatus.UpdateState(Game1.player.festivalScore.ToString() ?? "");
    if (this.festivalTimer > 0)
    {
      int festivalTimer = this.festivalTimer;
      this.festivalTimer -= time.ElapsedGameTime.Milliseconds;
      if (this.playerControlSequenceID == "iceFishing")
      {
        if (!Game1.player.UsingTool)
          Game1.player.forceCanMove();
        if (festivalTimer % 500 < this.festivalTimer % 500)
        {
          NPC actorByName1 = this.getActorByName("Pam");
          actorByName1.Sprite.sourceRect.Offset(actorByName1.Sprite.SourceRect.Width, 0);
          if (actorByName1.Sprite.sourceRect.X >= actorByName1.Sprite.Texture.Width)
            actorByName1.Sprite.sourceRect.Offset(-actorByName1.Sprite.Texture.Width, 0);
          NPC actorByName2 = this.getActorByName("Elliott");
          actorByName2.Sprite.sourceRect.Offset(actorByName2.Sprite.SourceRect.Width, 0);
          if (actorByName2.Sprite.sourceRect.X >= actorByName2.Sprite.Texture.Width)
            actorByName2.Sprite.sourceRect.Offset(-actorByName2.Sprite.Texture.Width, 0);
          NPC actorByName3 = this.getActorByName("Willy");
          actorByName3.Sprite.sourceRect.Offset(actorByName3.Sprite.SourceRect.Width, 0);
          if (actorByName3.Sprite.sourceRect.X >= actorByName3.Sprite.Texture.Width)
            actorByName3.Sprite.sourceRect.Offset(-actorByName3.Sprite.Texture.Width, 0);
        }
        if (festivalTimer % 29900 < this.festivalTimer % 29900)
        {
          this.getActorByName("Willy").shake(500);
          Game1.playSound("dwop");
          this.temporaryLocation.temporarySprites.Add(new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(112 /*0x70*/, 432, 16 /*0x10*/, 16 /*0x10*/), this.getActorByName("Willy").Position + new Vector2(0.0f, -96f), false, 0.015f, Color.White)
          {
            layerDepth = 1f,
            scale = 4f,
            interval = 9999f,
            motion = new Vector2(0.0f, -1f)
          });
        }
        if (festivalTimer % 45900 < this.festivalTimer % 45900)
        {
          this.getActorByName("Pam").shake(500);
          Game1.playSound("dwop");
          this.temporaryLocation.temporarySprites.Add(new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(112 /*0x70*/, 432, 16 /*0x10*/, 16 /*0x10*/), this.getActorByName("Pam").Position + new Vector2(0.0f, -96f), false, 0.015f, Color.White)
          {
            layerDepth = 1f,
            scale = 4f,
            interval = 9999f,
            motion = new Vector2(0.0f, -1f)
          });
        }
        if (festivalTimer % 59900 < this.festivalTimer % 59900)
        {
          this.getActorByName("Elliott").shake(500);
          Game1.playSound("dwop");
          this.temporaryLocation.temporarySprites.Add(new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(112 /*0x70*/, 432, 16 /*0x10*/, 16 /*0x10*/), this.getActorByName("Elliott").Position + new Vector2(0.0f, -96f), false, 0.015f, Color.White)
          {
            layerDepth = 1f,
            scale = 4f,
            interval = 9999f,
            motion = new Vector2(0.0f, -1f)
          });
        }
      }
      if (this.festivalTimer <= 0)
      {
        Game1.player.Halt();
        switch (this.playerControlSequenceID)
        {
          case "eggHunt":
            this.EndPlayerControlSequence();
            string data1;
            if (!this.TryGetFestivalDataForYear("afterEggHunt", out data1))
              Game1.log.Error($"Festival {this.id} doesn't have the required 'afterEggHunt' data field.");
            this.eventCommands = Event.ParseCommands(data1);
            this.currentCommand = 0;
            break;
          case "iceFishing":
            this.EndPlayerControlSequence();
            string data2;
            if (!this.TryGetFestivalDataForYear("afterIceFishing", out data2))
              Game1.log.Error($"Festival {this.id} doesn't have the required 'afterIceFishing' data field.");
            this.eventCommands = Event.ParseCommands(data2);
            this.currentCommand = 0;
            if (Game1.activeClickableMenu != null)
              Game1.activeClickableMenu.emergencyShutDown();
            Game1.activeClickableMenu = (IClickableMenu) null;
            if (Game1.player.UsingTool && Game1.player.CurrentTool is FishingRod currentTool)
              currentTool.doneFishing(Game1.player);
            Game1.screenOverlayTempSprites.Clear();
            Game1.player.forceCanMove();
            break;
        }
      }
    }
    if (this.startSecretSantaAfterDialogue && !Game1.dialogueUp)
    {
      Game1.globalFadeToBlack(new Game1.afterFadeFunction(this.startSecretSantaEvent), 0.01f);
      this.startSecretSantaAfterDialogue = false;
    }
    Game1.player.festivalScore = Math.Min(Game1.player.festivalScore, 9999);
  }

  private void setUpSecretSantaCommands()
  {
    Point tilePoint;
    try
    {
      tilePoint = this.getActorByName(this.mySecretSanta.Name).TilePoint;
    }
    catch
    {
      this.mySecretSanta = this.getActorByName("Lewis");
      tilePoint = this.getActorByName(this.mySecretSanta.Name).TilePoint;
    }
    Dictionary<string, string> dialogue1 = this.mySecretSanta.Dialogue;
    string newValue1 = dialogue1 != null ? dialogue1.GetValueOrDefault<string, string>("WinterStar_GiveGift_Before") : (string) null;
    Dictionary<string, string> dialogue2 = this.mySecretSanta.Dialogue;
    string newValue2 = dialogue2 != null ? dialogue2.GetValueOrDefault<string, string>("WinterStar_GiveGift_After") : (string) null;
    if (Game1.player.spouse == this.mySecretSanta.Name)
    {
      Dictionary<string, string> dialogue3 = this.mySecretSanta.Dialogue;
      newValue1 = (dialogue3 != null ? dialogue3.GetValueOrDefault<string, string>("WinterStar_GiveGift_Before_" + (Game1.player.isRoommate(this.mySecretSanta.Name) ? "Roommate" : "Spouse")) : (string) null) ?? newValue1;
      Dictionary<string, string> dialogue4 = this.mySecretSanta.Dialogue;
      newValue2 = (dialogue4 != null ? dialogue4.GetValueOrDefault<string, string>("WinterStar_GiveGift_After_" + (Game1.player.isRoommate(this.mySecretSanta.Name) ? "Roommate" : "Spouse")) : (string) null) ?? newValue2;
    }
    if (this.mySecretSanta.Age == 2)
    {
      if (newValue1 == null)
        newValue1 = Game1.LoadStringByGender(this.mySecretSanta.Gender, "Strings\\StringsFromCSFiles:Event.cs.1497");
      if (newValue2 == null)
        newValue2 = Game1.LoadStringByGender(this.mySecretSanta.Gender, "Strings\\StringsFromCSFiles:Event.cs.1498");
    }
    else if (this.mySecretSanta.Manners == 2)
    {
      if (newValue1 == null)
        newValue1 = Game1.LoadStringByGender(this.mySecretSanta.Gender, "Strings\\StringsFromCSFiles:Event.cs.1501");
      if (newValue2 == null)
        newValue2 = Game1.LoadStringByGender(this.mySecretSanta.Gender, "Strings\\StringsFromCSFiles:Event.cs.1504");
    }
    else
    {
      if (newValue1 == null)
        newValue1 = Game1.LoadStringByGender(this.mySecretSanta.Gender, "Strings\\StringsFromCSFiles:Event.cs.1499");
      if (newValue2 == null)
        newValue2 = Game1.LoadStringByGender(this.mySecretSanta.Gender, "Strings\\StringsFromCSFiles:Event.cs.1500");
    }
    for (int index = 0; index < this.eventCommands.Length; ++index)
    {
      this.eventCommands[index] = this.eventCommands[index].Replace("secretSanta", this.mySecretSanta.Name);
      this.eventCommands[index] = this.eventCommands[index].Replace("warpX", tilePoint.X.ToString() ?? "");
      this.eventCommands[index] = this.eventCommands[index].Replace("warpY", tilePoint.Y.ToString() ?? "");
      this.eventCommands[index] = this.eventCommands[index].Replace("dialogue1", newValue1);
      this.eventCommands[index] = this.eventCommands[index].Replace("dialogue2", newValue2);
    }
  }

  public void drawFarmers(SpriteBatch b)
  {
    foreach (Character farmerActor in this.farmerActors)
      farmerActor.draw(b);
  }

  public virtual bool ShouldHideCharacter(NPC n) => n is Child && this.doingSecretSanta;

  public void draw(SpriteBatch b)
  {
    if (this.currentCustomEventScript != null)
    {
      this.currentCustomEventScript.draw(b);
    }
    else
    {
      foreach (NPC actor in this.actors)
      {
        if (!this.ShouldHideCharacter(actor))
        {
          actor.Name.Equals("Marcello");
          if (actor.ySourceRectOffset == 0)
            actor.draw(b);
          else
            actor.draw(b, actor.ySourceRectOffset);
        }
      }
      foreach (Object prop in this.props)
        prop.drawAsProp(b);
      foreach (Prop festivalProp in this.festivalProps)
        festivalProp.draw(b);
      if (this.isSpecificFestival("fall16"))
      {
        Vector2 local = Game1.GlobalToLocal(Game1.viewport, new Vector2(37f, 56f) * 64f);
        local.X += 4f;
        int num = (int) local.X + 168;
        local.Y += 8f;
        for (int index = 0; index < Game1.player.team.grangeDisplay.Count; ++index)
        {
          if (Game1.player.team.grangeDisplay[index] != null)
          {
            local.Y += 42f;
            local.X += 4f;
            b.Draw(Game1.shadowTexture, local, new Microsoft.Xna.Framework.Rectangle?(Game1.shadowTexture.Bounds), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.0001f);
            local.Y -= 42f;
            local.X -= 4f;
            Game1.player.team.grangeDisplay[index].drawInMenu(b, local, 1f, 1f, (float) ((double) index / 1000.0 + 1.0 / 1000.0), StackDrawType.Hide);
          }
          local.X += 60f;
          if ((double) local.X >= (double) num)
          {
            local.X = (float) (num - 168);
            local.Y += 64f;
          }
        }
      }
      if (!this.drawTool)
        return;
      Game1.drawTool(this.farmer);
    }
  }

  public void drawUnderWater(SpriteBatch b)
  {
    if (this.underwaterSprites == null)
      return;
    foreach (TemporaryAnimatedSprite underwaterSprite in this.underwaterSprites)
      underwaterSprite.draw(b);
  }

  public void drawAfterMap(SpriteBatch b)
  {
    if (this.aboveMapSprites != null)
    {
      foreach (TemporaryAnimatedSprite aboveMapSprite in this.aboveMapSprites)
        aboveMapSprite.draw(b);
    }
    if (!Game1.game1.takingMapScreenshot && this.playerControlSequenceID != null)
    {
      switch (this.playerControlSequenceID)
      {
        case "eggHunt":
          b.Draw(Game1.fadeToBlackRect, new Microsoft.Xna.Framework.Rectangle(32 /*0x20*/, 32 /*0x20*/, 224 /*0xE0*/, 160 /*0xA0*/), Color.Black * 0.5f);
          Game1.drawWithBorder(Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1514", (object) (this.festivalTimer / 1000)), Color.Black, Color.Yellow, new Vector2(64f, 64f), 0.0f, 1f, 1f, false);
          Game1.drawWithBorder(Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1515", (object) Game1.player.festivalScore), Color.Black, Color.Pink, new Vector2(64f, 128f), 0.0f, 1f, 1f, false);
          if (Game1.IsMultiplayer)
          {
            Game1.player.team.festivalScoreStatus.Draw(b, new Vector2(32f, (float) (Game1.viewport.Height - 32 /*0x20*/)), draw_layer: 0.99f, vertical_origin: PlayerStatusList.VerticalAlignment.Bottom);
            break;
          }
          break;
        case "fair":
          b.End();
          Game1.PushUIMode();
          b.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
          b.Draw(Game1.fadeToBlackRect, new Microsoft.Xna.Framework.Rectangle(16 /*0x10*/, 16 /*0x10*/, 128 /*0x80*/ + (Game1.player.festivalScore > 999 ? 16 /*0x10*/ : 0), 64 /*0x40*/), Color.Black * 0.75f);
          b.Draw(Game1.mouseCursors, new Vector2(32f, 32f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(338, 400, 8, 8)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
          Game1.drawWithBorder(Game1.player.festivalScore.ToString() ?? "", Color.Black, Color.White, new Vector2(72f, (float) (21 + (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.en ? 8 : (LocalizedContentManager.CurrentLanguageLatin ? 16 /*0x10*/ : 8)))), 0.0f, 1f, 1f, false);
          if (Game1.activeClickableMenu == null)
            Game1.dayTimeMoneyBox.drawMoneyBox(b, Game1.dayTimeMoneyBox.xPositionOnScreen, 4);
          b.End();
          Game1.PopUIMode();
          b.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
          if (Game1.IsMultiplayer)
          {
            Game1.player.team.festivalScoreStatus.Draw(b, new Vector2(32f, (float) (Game1.viewport.Height - 32 /*0x20*/)), draw_layer: 0.99f, vertical_origin: PlayerStatusList.VerticalAlignment.Bottom);
            break;
          }
          break;
        case "iceFishing":
          b.End();
          b.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
          b.Draw(Game1.fadeToBlackRect, new Microsoft.Xna.Framework.Rectangle(16 /*0x10*/, 16 /*0x10*/, 128 /*0x80*/ + (Game1.player.festivalScore > 999 ? 16 /*0x10*/ : 0), 128 /*0x80*/), Color.Black * 0.75f);
          b.Draw(this.festivalTexture, new Vector2(32f, 16f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(112 /*0x70*/, 432, 16 /*0x10*/, 16 /*0x10*/)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
          Game1.drawWithBorder(Game1.player.festivalScore.ToString() ?? "", Color.Black, Color.White, new Vector2(96f, (float) (21 + (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.en ? 8 : (LocalizedContentManager.CurrentLanguageLatin ? 16 /*0x10*/ : 8)))), 0.0f, 1f, 1f, false);
          Game1.drawWithBorder(Utility.getMinutesSecondsStringFromMilliseconds(this.festivalTimer), Color.Black, Color.White, new Vector2(32f, 93f), 0.0f, 1f, 1f, false);
          b.End();
          b.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
          if (Game1.IsMultiplayer)
          {
            Game1.player.team.festivalScoreStatus.Draw(b, new Vector2(32f, (float) (Game1.viewport.Height - 32 /*0x20*/)), draw_layer: 0.99f, vertical_origin: PlayerStatusList.VerticalAlignment.Bottom);
            break;
          }
          break;
      }
    }
    string spriteTextToDraw = this.spriteTextToDraw;
    if ((spriteTextToDraw != null ? (spriteTextToDraw.Length > 0 ? 1 : 0) : 0) != 0)
    {
      Color colorFromIndex = SpriteText.getColorFromIndex(this.int_useMeForAnything2);
      SpriteText.drawStringHorizontallyCenteredAt(b, this.spriteTextToDraw, Game1.graphics.GraphicsDevice.Viewport.Width / 2, Game1.graphics.GraphicsDevice.Viewport.Height - 192 /*0xC0*/, this.int_useMeForAnything, layerDepth: 1f, color: new Color?(colorFromIndex));
    }
    foreach (Character actor in this.actors)
      actor.drawAboveAlwaysFrontLayer(b);
    if (this.skippable && !Game1.options.SnappyMenus && !Game1.game1.takingMapScreenshot)
    {
      Microsoft.Xna.Framework.Rectangle rectangle1 = this.skipBounds();
      Color white = Color.White;
      if (rectangle1.Contains(Game1.getOldMouseX(), Game1.getOldMouseY()))
        white *= 0.5f;
      Microsoft.Xna.Framework.Rectangle rectangle2 = new Microsoft.Xna.Framework.Rectangle(205, 406, 22, 15);
      b.Draw(Game1.mouseCursors, Utility.PointToVector2(rectangle1.Location), new Microsoft.Xna.Framework.Rectangle?(rectangle2), white, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.92f);
    }
    this.currentCustomEventScript?.drawAboveAlwaysFront(b);
  }

  public void EndPlayerControlSequence()
  {
    this.playerControlSequence = false;
    this.playerControlSequenceID = (string) null;
  }

  public void OnPlayerControlSequenceEnd(string id)
  {
    Game1.player.StopSitting();
    Game1.player.CanMove = false;
    Game1.player.Halt();
  }

  public void setUpPlayerControlSequence(string id)
  {
    this.playerControlSequenceID = id;
    this.playerControlSequence = true;
    Game1.player.CanMove = true;
    Game1.viewportFreeze = false;
    Game1.forceSnapOnNextViewportUpdate = true;
    Game1.globalFade = false;
    this.doingSecretSanta = false;
    if (id == null)
      return;
    switch (id.Length)
    {
      case 4:
        switch (id[0])
        {
          case 'f':
            if (!(id == "fair"))
              return;
            this.festivalHost = this.getActorByName("Lewis");
            this.hostMessageKey = "Strings\\StringsFromCSFiles:Event.cs.1535";
            return;
          case 'l':
            if (!(id == "luau"))
              return;
            this.festivalHost = this.getActorByName("Lewis");
            this.hostMessageKey = "Strings\\StringsFromCSFiles:Event.cs.1527";
            return;
          default:
            return;
        }
      case 7:
        switch (id[0])
        {
          case 'e':
            if (!(id == "eggHunt"))
              return;
            Layer layer = Game1.currentLocation.map.RequireLayer("Paths");
            for (int index1 = 0; index1 < layer.LayerWidth; ++index1)
            {
              for (int index2 = 0; index2 < layer.LayerHeight; ++index2)
              {
                Tile tile = layer.Tiles[index1, index2];
                if (tile != null && tile.TileSheet.Id.StartsWith("fest"))
                  this.festivalProps.Add(new Prop(this.festivalTexture, tile.TileIndex, 1, 1, 1, index1, index2));
              }
            }
            this.festivalTimer = 52000;
            ++this.currentCommand;
            return;
          case 'j':
            if (!(id == "jellies"))
              return;
            this.festivalHost = this.getActorByName("Lewis");
            this.hostMessageKey = "Strings\\StringsFromCSFiles:Event.cs.1531";
            return;
          default:
            return;
        }
      case 8:
        if (!(id == "boatRide"))
          break;
        Game1.viewportFreeze = true;
        Game1.currentViewportTarget = Utility.PointToVector2(Game1.viewportCenter);
        ++this.currentCommand;
        break;
      case 9:
        switch (id[0])
        {
          case 'c':
            if (!(id == "christmas"))
              return;
            this.secretSantaRecipient = Utility.GetRandomWinterStarParticipant();
            this.mySecretSanta = Utility.GetRandomWinterStarParticipant((Func<string, bool>) (name => name == this.secretSantaRecipient.Name || NPC.IsDivorcedFrom(this.farmer, name))) ?? this.secretSantaRecipient;
            Game1.debugOutput = $"Secret Santa Recipient: {this.secretSantaRecipient.Name}  My Secret Santa: {this.mySecretSanta.Name}";
            return;
          case 'h':
            if (!(id == "halloween"))
              return;
            if (Game1.year % 2 == 0)
            {
              this.temporaryLocation.objects.Add(new Vector2(63f, 16f), (Object) new Chest(new List<Item>()
              {
                ItemRegistry.Create("(O)PrizeTicket")
              }, new Vector2(63f, 16f)));
              return;
            }
            this.temporaryLocation.objects.Add(new Vector2(33f, 13f), (Object) new Chest(new List<Item>()
            {
              ItemRegistry.Create("(O)373")
            }, new Vector2(33f, 13f)));
            return;
          default:
            return;
        }
      case 10:
        switch (id[0])
        {
          case 'h':
            if (!(id == "haleyBeach"))
              return;
            Vector2 vector2 = new Vector2(53f, 8f);
            Object @object = ItemRegistry.Create<Object>("(O)742");
            @object.TileLocation = vector2;
            @object.Flipped = false;
            this.props.Add(@object);
            this.playerControlTargetTile = new Point(53, 8);
            Game1.player.canOnlyWalk = false;
            return;
          case 'i':
            if (!(id == "iceFishing"))
              return;
            Tool tool = ItemRegistry.Create<Tool>("(T)BambooPole");
            tool.AttachmentSlotsCount = 2;
            tool.attachments[1] = ItemRegistry.Create<Object>("(O)687");
            this.festivalTimer = 120000;
            this.farmer.festivalScore = 0;
            this.farmer.CurrentToolIndex = 0;
            this.farmer.TemporaryItem = (Item) tool;
            this.farmer.CurrentToolIndex = 0;
            return;
          case 'p':
            if (!(id == "parrotRide"))
              return;
            Game1.player.canOnlyWalk = false;
            ++this.currentCommand;
            return;
          default:
            return;
        }
      case 11:
        switch (id[0])
        {
          case 'e':
            if (!(id == "eggFestival"))
              return;
            this.festivalHost = this.getActorByName("Lewis");
            this.hostMessageKey = "Strings\\StringsFromCSFiles:Event.cs.1521";
            return;
          case 'i':
            if (!(id == "iceFestival"))
              return;
            this.festivalHost = this.getActorByName("Lewis");
            this.hostMessageKey = "Strings\\StringsFromCSFiles:Event.cs.1548";
            if (Game1.year % 2 == 0)
            {
              this.temporaryLocation.setFireplace(true, 46, 16 /*0x10*/, false, -28, 28);
              this.temporaryLocation.setFireplace(true, 61, 43, false, -28, 28);
            }
            else
            {
              this.temporaryLocation.setFireplace(true, 11, 44, false, -28, 28);
              this.temporaryLocation.setFireplace(true, 65, 45, false, -28, 28);
            }
            if (!Game1.MasterPlayer.mailReceived.Contains("raccoonTreeFallen"))
              return;
            for (int x = 52; x < 60; ++x)
            {
              for (int y = 0; y < 2; ++y)
                this.temporaryLocation.removeTile(x, y, "AlwaysFront");
            }
            if (!NetWorldState.checkAnywhereForWorldStateID("forestStumpFixed"))
            {
              this.temporaryLocation.ApplyMapOverride("Forest_RaccoonStump", destination_rect: new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(53, 2, 7, 6)));
              return;
            }
            this.temporaryLocation.ApplyMapOverride("Forest_RaccoonHouse", destination_rect: new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(53, 2, 7, 6)));
            return;
          default:
            return;
        }
      case 14:
        if (!(id == "flowerFestival"))
          break;
        this.festivalHost = this.getActorByName("Lewis");
        this.hostMessageKey = "Strings\\StringsFromCSFiles:Event.cs.1524";
        if (!NetWorldState.checkAnywhereForWorldStateID("trashBearDone"))
          break;
        Game1.currentLocation.removeMapTile(62, 28, "Buildings");
        Game1.currentLocation.removeMapTile(64 /*0x40*/, 28, "Buildings");
        Game1.currentLocation.removeMapTile(73, 48 /*0x30*/, "Buildings");
        break;
    }
  }

  public bool canMoveAfterDialogue()
  {
    if (this.playerControlSequenceID != null && this.playerControlSequenceID.Equals("eggHunt"))
    {
      Game1.player.canMove = true;
      ++this.CurrentCommand;
    }
    return this.playerControlSequence;
  }

  public void forceFestivalContinue()
  {
    bool flag = this.isSpecificFestival("fall16");
    if (flag)
    {
      this.initiateGrangeJudging();
    }
    else
    {
      Game1.dialogueUp = false;
      if (Game1.activeClickableMenu != null)
        Game1.activeClickableMenu.emergencyShutDown();
      Game1.exitActiveMenu();
      string data;
      if (!this.TryGetFestivalDataForYear("mainEvent", out data))
        Game1.log.Error($"Festival {this.id} doesn't have the required 'mainEvent' data field.");
      this.eventCommands = Event.ParseCommands(data);
      this.CurrentCommand = 0;
      this.eventSwitched = true;
      this.playerControlSequence = false;
      this.setUpFestivalMainEvent();
      Game1.player.Halt();
    }
    if (!Game1.IsServer || !flag && Game1.HasDedicatedHost)
      return;
    Game1.multiplayer.sendServerToClientsMessage("festivalEvent");
  }

  /// <summary>Split an event's key into its ID and preconditions.</summary>
  /// <param name="rawScript">The event key to split.</param>
  public static string[] SplitPreconditions(string rawScript)
  {
    return ArgUtility.SplitQuoteAware(rawScript, '/', StringSplitOptions.RemoveEmptyEntries, true);
  }

  /// <summary>Split and preprocess a raw event script into its component commands.</summary>
  /// <param name="rawScript">The raw event script to split.</param>
  /// <param name="player">The player for which the event is being parsed.</param>
  public static string[] ParseCommands(string rawScript, Farmer player = null)
  {
    rawScript = Dialogue.applyGenderSwitchBlocks((player != null ? new Gender?(player.Gender) : Game1.player?.Gender).GetValueOrDefault(), rawScript);
    rawScript = TokenParser.ParseText(rawScript);
    return ArgUtility.SplitQuoteAware(rawScript, '/', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries, true);
  }

  public bool isSpecificFestival(string festivalId)
  {
    return this.isFestival && this.id == "festival_" + festivalId;
  }

  public void setUpFestivalMainEvent()
  {
    if (!this.isSpecificFestival("spring24"))
      return;
    List<NetDancePartner> netDancePartnerList1 = new List<NetDancePartner>();
    List<NetDancePartner> netDancePartnerList2 = new List<NetDancePartner>();
    List<string> source1 = new List<string>()
    {
      "Abigail",
      "Penny",
      "Leah",
      "Maru",
      "Haley",
      "Emily"
    };
    List<string> stringList = new List<string>()
    {
      "Sebastian",
      "Sam",
      "Elliott",
      "Harvey",
      "Alex",
      "Shane"
    };
    List<Farmer> list = Game1.getOnlineFarmers().OrderBy<Farmer, long>((Func<Farmer, long>) (f => f.UniqueMultiplayerID)).ToList<Farmer>();
    while (list.Count > 0)
    {
      Farmer farmer = list[0];
      list.RemoveAt(0);
      if (!Game1.multiplayer.isDisconnecting(farmer) && farmer.dancePartner.Value != null)
      {
        if (farmer.dancePartner.GetGender() == Gender.Female)
        {
          netDancePartnerList1.Add(farmer.dancePartner);
          if (farmer.dancePartner.IsVillager())
            source1.Remove(farmer.dancePartner.TryGetVillager().Name);
          netDancePartnerList2.Add(new NetDancePartner(farmer));
        }
        else
        {
          netDancePartnerList2.Add(farmer.dancePartner);
          if (farmer.dancePartner.IsVillager())
            stringList.Remove(farmer.dancePartner.TryGetVillager().Name);
          netDancePartnerList1.Add(new NetDancePartner(farmer));
        }
        if (farmer.dancePartner.IsFarmer())
          list.Remove(farmer.dancePartner.TryGetFarmer());
      }
    }
    while (netDancePartnerList1.Count < 6)
    {
      string str = source1.Last<string>();
      if (stringList.Contains(Utility.getLoveInterest(str)))
      {
        netDancePartnerList1.Add(new NetDancePartner(str));
        netDancePartnerList2.Add(new NetDancePartner(Utility.getLoveInterest(str)));
      }
      source1.Remove(str);
    }
    string data;
    if (!this.TryGetFestivalDataForYear("mainEvent", out data))
      data = string.Empty;
    for (int index = 1; index <= 6; ++index)
    {
      string newValue1 = !netDancePartnerList1[index - 1].IsVillager() ? "farmer" + Utility.getFarmerNumberFromFarmer(netDancePartnerList1[index - 1].TryGetFarmer()).ToString() : netDancePartnerList1[index - 1].TryGetVillager().Name;
      string newValue2 = !netDancePartnerList2[index - 1].IsVillager() ? "farmer" + Utility.getFarmerNumberFromFarmer(netDancePartnerList2[index - 1].TryGetFarmer()).ToString() : netDancePartnerList2[index - 1].TryGetVillager().Name;
      data = data.Replace("Girl" + index.ToString(), newValue1).Replace("Guy" + index.ToString(), newValue2);
    }
    List<KeyValuePair<NetDancePartner, NetDancePartner>> keyValuePairList = new List<KeyValuePair<NetDancePartner, NetDancePartner>>();
    List<KeyValuePair<NetDancePartner, NetDancePartner>> source2 = new List<KeyValuePair<NetDancePartner, NetDancePartner>>();
    for (int index = netDancePartnerList1.Count - 1; index >= 0; --index)
    {
      NetDancePartner key = netDancePartnerList1[index];
      NetDancePartner netDancePartner = netDancePartnerList2[index];
      if (key.IsFarmer() || netDancePartner.IsFarmer())
      {
        source2.Add(new KeyValuePair<NetDancePartner, NetDancePartner>(key, netDancePartner));
        netDancePartnerList1.RemoveAt(index);
        netDancePartnerList2.RemoveAt(index);
      }
    }
    keyValuePairList.AddRange((IEnumerable<KeyValuePair<NetDancePartner, NetDancePartner>>) source2.OrderBy<KeyValuePair<NetDancePartner, NetDancePartner>, int>((Func<KeyValuePair<NetDancePartner, NetDancePartner>, int>) (pair =>
    {
      int numberFromFarmer1 = Utility.getFarmerNumberFromFarmer(pair.Key.TryGetFarmer());
      int numberFromFarmer2 = Utility.getFarmerNumberFromFarmer(pair.Value.TryGetFarmer());
      if (numberFromFarmer1 > -1 && numberFromFarmer2 > -1)
        return Math.Min(numberFromFarmer1, numberFromFarmer2);
      return numberFromFarmer1 <= -1 ? numberFromFarmer2 : numberFromFarmer1;
    })));
    for (int index = 0; index < netDancePartnerList1.Count; ++index)
      keyValuePairList.Add(new KeyValuePair<NetDancePartner, NetDancePartner>(netDancePartnerList1[index], netDancePartnerList2[index]));
    netDancePartnerList1.Clear();
    netDancePartnerList2.Clear();
    bool flag = true;
    foreach (KeyValuePair<NetDancePartner, NetDancePartner> keyValuePair in keyValuePairList)
    {
      if (flag)
      {
        netDancePartnerList1.Insert(0, keyValuePair.Key);
        netDancePartnerList2.Insert(0, keyValuePair.Value);
      }
      else
      {
        netDancePartnerList1.Add(keyValuePair.Key);
        netDancePartnerList2.Add(keyValuePair.Value);
      }
      flag = !flag;
    }
    List<string> values = new List<string>((IEnumerable<string>) Event.ParseCommands(data));
    for (int index1 = 0; index1 < values.Count; ++index1)
    {
      string str1 = values[index1];
      List<NetDancePartner> netDancePartnerList3 = (List<NetDancePartner>) null;
      string oldValue = (string) null;
      if (str1.Contains("Girls"))
      {
        oldValue = "Girls";
        netDancePartnerList3 = netDancePartnerList1;
      }
      else if (str1.Contains("Guys"))
      {
        oldValue = "Guys";
        netDancePartnerList3 = netDancePartnerList2;
      }
      if (netDancePartnerList3 != null)
      {
        float num1 = 10f / (float) (netDancePartnerList3.Count - 1);
        if ((double) num1 < 1.0)
          num1 = 1f;
        for (int index2 = 0; index2 < netDancePartnerList3.Count; ++index2)
        {
          string newValue = netDancePartnerList3[index2].IsVillager() ? netDancePartnerList3[index2].TryGetVillager().Name : "farmer" + Utility.getFarmerNumberFromFarmer(netDancePartnerList3[index2].TryGetFarmer()).ToString();
          string str2 = str1.Replace(oldValue, newValue);
          if (str2.StartsWith("warp "))
          {
            string[] strArray = ArgUtility.SplitBySpace(str2);
            int num2 = int.Parse(strArray[2]);
            num2 += (int) Math.Round((double) index2 * (double) num1);
            strArray[2] = num2.ToString();
            str2 = string.Join(" ", strArray);
          }
          values.Insert(index1 + index2, str2);
        }
        int index3 = index1 + netDancePartnerList3.Count;
        values.RemoveAt(index3);
        index1 = index3 - 1;
      }
    }
    string str3 = string.Join("/", (IEnumerable<string>) values);
    Regex regex1 = new Regex("showFrame (?<farmerName>farmer\\d) 44");
    Regex regex2 = new Regex("showFrame (?<farmerName>farmer\\d) 40");
    Regex regex3 = new Regex("animate (?<farmerName>farmer\\d) false true 600 44 45");
    Regex regex4 = new Regex("animate (?<farmerName>farmer\\d) false true 600 43 41 43 42");
    Regex regex5 = new Regex("animate (?<farmerName>farmer\\d) false true 300 46 47");
    Regex regex6 = new Regex("animate (?<farmerName>farmer\\d) false true 600 46 47");
    string input1 = str3;
    string input2 = regex1.Replace(input1, "showFrame $1 12/faceDirection $1 0");
    string input3 = regex2.Replace(input2, "showFrame $1 0/faceDirection $1 2");
    string input4 = regex3.Replace(input3, "animate $1 false true 600 12 13 12 14");
    string input5 = regex4.Replace(input4, "animate $1 false true 596 4 0");
    string input6 = regex5.Replace(input5, "animate $1 false true 150 12 13 12 14");
    this.eventCommands = Event.ParseCommands(regex6.Replace(input6, "animate $1 false true 600 0 3"));
  }

  private void judgeGrange()
  {
    int num1 = 14;
    Dictionary<int, bool> dictionary = new Dictionary<int, bool>();
    int num2 = 0;
    bool flag = false;
    foreach (Item obj in Game1.player.team.grangeDisplay)
    {
      if (obj is Object i)
      {
        if (Event.IsItemMayorShorts((Item) i))
          flag = true;
        num1 += i.Quality + 1;
        int storePrice = i.sellToStorePrice(-1L);
        if (storePrice >= 20)
          ++num1;
        if (storePrice >= 90)
          ++num1;
        if (storePrice >= 200)
          ++num1;
        if (storePrice >= 300 && i.Quality < 2)
          ++num1;
        if (storePrice >= 400 && i.Quality < 1)
          ++num1;
        switch (i.Category)
        {
          case -81:
          case -80:
          case -27:
            dictionary[-81] = true;
            continue;
          case -79:
            dictionary[-79] = true;
            continue;
          case -75:
            dictionary[-75] = true;
            continue;
          case -26:
            dictionary[-26] = true;
            continue;
          case -18:
          case -14:
          case -6:
          case -5:
            dictionary[-5] = true;
            continue;
          case -12:
          case -2:
            dictionary[-12] = true;
            continue;
          case -7:
            dictionary[-7] = true;
            continue;
          case -4:
            dictionary[-4] = true;
            continue;
          default:
            continue;
        }
      }
      else if (obj == null)
        ++num2;
    }
    this.grangeScore = num1 + Math.Min(30, dictionary.Count * 5) + (9 - 2 * num2);
    if (!flag)
      return;
    this.grangeScore = -666;
  }

  private void lewisDoneJudgingGrange()
  {
    if (Game1.activeClickableMenu == null)
    {
      Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1584")));
      Game1.player.Halt();
    }
    this.interpretGrangeResults();
  }

  public void interpretGrangeResults()
  {
    List<Character> characterList = new List<Character>()
    {
      (Character) this.getActorByName("Pierre"),
      (Character) this.getActorByName("Marnie"),
      (Character) this.getActorByName("Willy")
    };
    if (this.grangeScore >= 90)
      characterList.Insert(0, (Character) Game1.player);
    else if (this.grangeScore >= 75)
      characterList.Insert(1, (Character) Game1.player);
    else if (this.grangeScore >= 60)
      characterList.Insert(2, (Character) Game1.player);
    else
      characterList.Add((Character) Game1.player);
    bool flag1 = (characterList[0] is NPC npc ? npc.Name : (string) null) == "Pierre";
    bool flag2 = Game1.player.team.grangeDisplay.Count == 0;
    bool flag3 = this.grangeScore == -666;
    foreach (NPC actor in this.actors)
    {
      Dialogue dialogue = !flag1 ? actor.TryGetDialogue("Fair_Judged_PlayerWon") ?? actor.TryGetDialogue("Fair_Judged") : (flag3 ? actor.TryGetDialogue("Fair_Judged_PlayerLost_PurpleShorts") : (Dialogue) null) ?? (flag2 ? actor.TryGetDialogue("Fair_Judged_PlayerLost_Skipped") : (Dialogue) null) ?? actor.TryGetDialogue("Fair_Judged_PlayerLost") ?? actor.TryGetDialogue("Fair_Judged");
      if (dialogue != null)
        actor.setNewDialogue(dialogue);
    }
    this.grangeJudged = true;
    if (!(characterList[0] is Farmer))
      return;
    foreach (Farmer allFarmer in Game1.getAllFarmers())
      allFarmer.autoGenerateActiveDialogueEvent("wonGrange");
  }

  private void initiateGrangeJudging()
  {
    this.judgeGrange();
    this.hostMessageKey = (string) null;
    this.setUpAdvancedMove(ArgUtility.SplitBySpace("advancedMove Lewis False 2 0 0 7 8 0 4 3000 3 0 4 3000 3 0 4 3000 3 0 4 3000 -14 0 2 1000"), new NPCController.endBehavior(this.lewisDoneJudgingGrange));
    this.getActorByName("Lewis").CurrentDialogue.Clear();
    if (this.getActorByName("Marnie") != null)
      this.npcControllers.RemoveAll((Predicate<NPCController>) (npcController => npcController.puppet.Name == "Marnie"));
    this.setUpAdvancedMove(ArgUtility.SplitBySpace("advancedMove Marnie False 0 1 4 1000"));
    foreach (NPC actor in this.actors)
    {
      Dialogue dialogue = actor.TryGetDialogue("Fair_Judging");
      if (dialogue != null)
        actor.setNewDialogue(dialogue);
    }
  }

  public void answerDialogueQuestion(NPC who, string answerKey)
  {
    if (!this.isFestival)
      return;
    switch (answerKey)
    {
      case "yes":
        if (Game1.HasDedicatedHost)
        {
          if (this.isSpecificFestival("fall16"))
          {
            if (Game1.IsServer)
            {
              this.forceFestivalContinue();
              break;
            }
            Game1.dedicatedServer.DoHostAction("JudgeGrange");
            break;
          }
          string str = "MainEvent_" + this.id;
          Game1.netReady.SetLocalReady(str, true);
          Game1.activeClickableMenu = (IClickableMenu) new ReadyCheckDialog(str, true, (ConfirmationDialog.behavior) (farmer => this.forceFestivalContinue()));
          break;
        }
        this.forceFestivalContinue();
        break;
      case "danceAsk":
        if (Game1.player.spouse != null && who.Name == Game1.player.spouse)
        {
          Game1.player.dancePartner.Value = (Character) who;
          who.setNewDialogue(who.TryGetDialogue("FlowerDance_Accept_" + (Game1.player.isRoommate(who.Name) ? "Roommate" : "Spouse")) ?? who.TryGetDialogue("FlowerDance_Accept") ?? new Dialogue(who, "Strings\\StringsFromCSFiles:Event.cs.1632"));
          foreach (NPC actor in this.actors)
          {
            Stack<Dialogue> currentDialogue = actor.CurrentDialogue;
            // ISSUE: explicit non-virtual call
            if ((currentDialogue != null ? (__nonvirtual (currentDialogue.Count) > 0 ? 1 : 0) : 0) != 0 && actor.CurrentDialogue.Peek().getCurrentDialogue().Equals("..."))
              actor.CurrentDialogue.Clear();
          }
        }
        else
        {
          if (!who.HasPartnerForDance && Game1.player.getFriendshipLevelForNPC(who.Name) >= 1000)
          {
            if (!who.isMarried())
            {
              try
              {
                Game1.player.changeFriendship(250, Game1.getCharacterFromName(who.Name));
              }
              catch
              {
              }
              Game1.player.dancePartner.Value = (Character) who;
              who.setNewDialogue(who.TryGetDialogue("FlowerDance_Accept") ?? (who.Gender == Gender.Female ? new Dialogue(who, "Strings\\StringsFromCSFiles:Event.cs.1634") : new Dialogue(who, "Strings\\StringsFromCSFiles:Event.cs.1633")));
              using (List<NPC>.Enumerator enumerator = this.actors.GetEnumerator())
              {
                while (enumerator.MoveNext())
                {
                  NPC current = enumerator.Current;
                  Stack<Dialogue> currentDialogue = current.CurrentDialogue;
                  // ISSUE: explicit non-virtual call
                  if ((currentDialogue != null ? (__nonvirtual (currentDialogue.Count) > 0 ? 1 : 0) : 0) != 0 && current.CurrentDialogue.Peek().getCurrentDialogue().Equals("..."))
                    current.CurrentDialogue.Clear();
                }
                goto label_33;
              }
            }
          }
          if (who.HasPartnerForDance)
          {
            who.setNewDialogue("Strings\\StringsFromCSFiles:Event.cs.1635");
          }
          else
          {
            Dialogue dialogue = who.TryGetDialogue("FlowerDance_Decline") ?? who.TryGetDialogue("danceRejection");
            if (dialogue == null)
              break;
            who.setNewDialogue(dialogue);
          }
        }
label_33:
        Game1.drawDialogue(who);
        who.immediateSpeak = true;
        who.facePlayer(Game1.player);
        who.Halt();
        break;
    }
  }

  public void addItemToGrangeDisplay(Item i, int position, bool force)
  {
    while (Game1.player.team.grangeDisplay.Count < 9)
      Game1.player.team.grangeDisplay.Add((Item) null);
    if (position < 0 || position >= Game1.player.team.grangeDisplay.Count || Game1.player.team.grangeDisplay[position] != null && !force)
      return;
    Game1.player.team.grangeDisplay[position] = i;
  }

  private bool onGrangeChange(
    Item i,
    int position,
    Item old,
    StorageContainer container,
    bool onRemoval)
  {
    if (!onRemoval)
    {
      if (i.Stack > 1 || i.Stack == 1 && old != null && old.Stack == 1 && i.canStackWith((ISalable) old))
      {
        if (old != null && i != null && old.canStackWith((ISalable) i))
        {
          container.ItemsToGrabMenu.actualInventory[position].Stack = 1;
          container.heldItem = old;
          return false;
        }
        if (old != null)
        {
          Utility.addItemToInventory(old, position, container.ItemsToGrabMenu.actualInventory);
          container.heldItem = i;
          return false;
        }
        int num = i.Stack - 1;
        Item one = i.getOne();
        one.Stack = num;
        container.heldItem = one;
        i.Stack = 1;
      }
    }
    else if (old != null && old.Stack > 1 && !old.Equals((object) i))
      return false;
    this.addItemToGrangeDisplay(!onRemoval || old != null && !old.Equals((object) i) ? i : (Item) null, position, true);
    return true;
  }

  public bool canPlayerUseTool()
  {
    if (!this.isSpecificFestival("winter8") || this.festivalTimer <= 0 || Game1.player.UsingTool)
      return false;
    this.previousFacingDirection = Game1.player.FacingDirection;
    return true;
  }

  public bool checkAction(Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who)
  {
    if (this.isFestival)
    {
      Object @object;
      if (this.temporaryLocation != null && this.temporaryLocation.objects.TryGetValue(new Vector2((float) tileLocation.X, (float) tileLocation.Y), out @object))
        @object.checkForAction(who);
      GameLocation currentLocation = Game1.currentLocation;
      switch (this.id)
      {
        case "festival_fall16":
          switch (currentLocation.getTileIndexAt(tileLocation.X, tileLocation.Y, "Buildings", "untitled tile sheet"))
          {
            case 87:
            case 88:
              if (who.IsLocalPlayer)
              {
                Response[] answerChoices = new Response[2]
                {
                  new Response("Buy", Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1654")),
                  new Response("Leave", Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1656"))
                };
                currentLocation.createQuestionDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1659"), answerChoices, "StarTokenShop");
              }
              return true;
            case 175:
            case 176 /*0xB0*/:
              if (who.IsLocalPlayer)
                Game1.player.eatObject(ItemRegistry.Create<Object>("(O)241"), true);
              return true;
            case 308:
            case 309:
              if (who.IsLocalPlayer)
              {
                Response[] answerChoices = new Response[3]
                {
                  new Response("Orange", Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1645")),
                  new Response("Green", Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1647")),
                  new Response("I", Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1650"))
                };
                currentLocation.createQuestionDialogue(Game1.parseText(Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1652")), answerChoices, "wheelBet");
              }
              return true;
            case 349:
            case 350:
            case 351:
              Game1.player.team.grangeMutex.RequestLock((Action) (() =>
              {
                while (Game1.player.team.grangeDisplay.Count < 9)
                  Game1.player.team.grangeDisplay.Add((Item) null);
                Game1.activeClickableMenu = (IClickableMenu) new StorageContainer((IList<Item>) Game1.player.team.grangeDisplay.ToList<Item>(), 9, itemChangeBehavior: new StorageContainer.behaviorOnItemChange(this.onGrangeChange), highlightMethod: new InventoryMenu.highlightThisItem(Utility.highlightSmallObjects));
              }));
              return true;
            case 501:
            case 502:
              if (who.IsLocalPlayer)
              {
                Response[] answerChoices = new Response[2]
                {
                  new Response("Play", Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1662")),
                  new Response("Leave", Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1663"))
                };
                currentLocation.createQuestionDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1666"), answerChoices, "slingshotGame");
              }
              return true;
            case 503:
            case 504:
              if (who.IsLocalPlayer)
              {
                Response[] answerChoices = new Response[2]
                {
                  new Response("Play", Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1662")),
                  new Response("Leave", Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1663"))
                };
                currentLocation.createQuestionDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1681"), answerChoices, "fishingGame");
              }
              return true;
            case 505:
            case 506:
              if (who.IsLocalPlayer)
              {
                if (who.Money >= 100 && !who.mailReceived.Contains("fortuneTeller" + Game1.year.ToString()))
                {
                  Response[] answerChoices = new Response[2]
                  {
                    new Response("Read", Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1688")),
                    new Response("No", Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1690"))
                  };
                  currentLocation.createQuestionDialogue(Game1.parseText(Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1691")), answerChoices, "fortuneTeller");
                }
                else if (who.mailReceived.Contains("fortuneTeller" + Game1.year.ToString()))
                  Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1694")));
                else
                  Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1695")));
                who.Halt();
              }
              return true;
            case 510:
            case 511 /*0x01FF*/:
              if (who.IsLocalPlayer)
                currentLocation.createQuestionDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1672"), currentLocation.createYesNoResponses(), "starTokenShop");
              return true;
            case 540:
              if (who.IsLocalPlayer)
              {
                if (who.TilePoint.X == 29)
                  Game1.activeClickableMenu = (IClickableMenu) new StrengthGame();
                else
                  Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1684")));
              }
              return true;
          }
          break;
        case "festival_fall27":
          if (currentLocation.getTileIndexAt(tileLocation.X, tileLocation.Y, "Buildings", "Landscape") == 958 && (tileLocation.X == 44 && tileLocation.Y == 9 || tileLocation.X == 61 && tileLocation.Y == 13))
          {
            if (who.IsLocalPlayer)
              currentLocation.createQuestionDialogue(Game1.content.LoadString("Strings\\1_6_Strings:SpiritsEveCart"), currentLocation.createYesNoResponses(), "spirits_eve_shortcut");
            return true;
          }
          break;
        case "festival_winter8":
          switch (currentLocation.getTileIndexAt(tileLocation.X, tileLocation.Y, "Buildings", "fest"))
          {
            case 1009:
            case 1010:
            case 1012:
            case 1013:
              Game1.playSound("pig");
              return true;
          }
          break;
      }
      string str1 = currentLocation.doesTileHaveProperty(tileLocation.X, tileLocation.Y, "Action", "Buildings");
      if (str1 != null)
      {
        try
        {
          string[] strArray1 = ArgUtility.SplitBySpace(str1);
          switch (ArgUtility.Get(strArray1, 0))
          {
            case "OpenShop":
            case "Shop":
              string str2;
              string error1;
              if (!ArgUtility.TryGet(strArray1, 1, out str2, out error1, name: "string shop_id"))
              {
                currentLocation.LogTileActionError(strArray1, tileLocation.X, tileLocation.Y, error1);
                return false;
              }
              if (!who.IsLocalPlayer)
                return false;
              bool flag = false;
              if (str2 == "shop" && this.isFestival)
              {
                string id = this.id;
                if (id != null)
                {
                  switch (id.Length)
                  {
                    case 15:
                      if (id == "festival_fall27")
                      {
                        str2 = "Festival_SpiritsEve_Pierre";
                        break;
                      }
                      break;
                    case 16 /*0x10*/:
                      if (id == "festival_winter8")
                      {
                        str2 = "Festival_FestivalOfIce_TravelingMerchant";
                        break;
                      }
                      break;
                    case 17:
                      switch (id[16 /*0x10*/])
                      {
                        case '1':
                          if (id == "festival_summer11")
                          {
                            str2 = "Festival_Luau_Pierre";
                            break;
                          }
                          break;
                        case '3':
                          if (id == "festival_spring13")
                          {
                            str2 = "Festival_EggFestival_Pierre";
                            break;
                          }
                          break;
                        case '4':
                          if (id == "festival_spring24")
                          {
                            str2 = "Festival_FlowerDance_Pierre";
                            break;
                          }
                          break;
                        case '5':
                          if (id == "festival_winter25")
                          {
                            str2 = "Festival_FeastOfTheWinterStar_Pierre";
                            break;
                          }
                          break;
                        case '8':
                          if (id == "festival_summer28")
                          {
                            str2 = "Festival_DanceOfTheMoonlightJellies_Pierre";
                            break;
                          }
                          break;
                      }
                      break;
                  }
                }
              }
              string str3;
              if (this.festivalData.TryGetValue(str2, out str3))
              {
                if (this.festivalShops == null)
                  this.festivalShops = new Dictionary<string, Dictionary<ISalable, ItemStockInformation>>();
                Dictionary<ISalable, ItemStockInformation> dictionary;
                if (!this.festivalShops.TryGetValue(str2, out dictionary))
                {
                  string[] strArray2 = ArgUtility.SplitBySpace(str3);
                  dictionary = new Dictionary<ISalable, ItemStockInformation>();
                  for (int index = 0; index < strArray2.Length; index += 4)
                  {
                    string type;
                    string itemId;
                    int price;
                    int stock;
                    if (!ArgUtility.TryGet(strArray1, index, out type, out error1, name: "string type") || !ArgUtility.TryGet(strArray1, index + 1, out itemId, out error1, name: "string itemId") || !ArgUtility.TryGetInt(strArray1, index + 2, out price, out error1, "int price") || !ArgUtility.TryGetInt(strArray1, index + 3, out stock, out error1, "int stock"))
                    {
                      Game1.log.Error($"Festival '{this.id}' has legacy shop inventory '{string.Join(" ", strArray2)}' which couldn't be parsed: {error1}.");
                      break;
                    }
                    Item standardTextDescription = Utility.getItemFromStandardTextDescription(type, itemId, stock, who);
                    if (standardTextDescription != null)
                    {
                      if (standardTextDescription.Category == -74)
                        price = (int) Math.Max(1f, (float) price * Game1.MasterPlayer.difficultyModifier);
                      if (!standardTextDescription.IsRecipe || !who.knowsRecipe(standardTextDescription.Name))
                        dictionary.Add((ISalable) standardTextDescription, new ItemStockInformation(price, stock <= 0 ? int.MaxValue : stock, stockMode: LimitedStockMode.Player));
                    }
                  }
                  this.festivalShops[str2] = dictionary;
                }
                // ISSUE: explicit non-virtual call
                if (dictionary != null && __nonvirtual (dictionary.Count) > 0)
                {
                  who.team.synchronizedShopStock.UpdateLocalStockWithSyncedQuanitities(who.currentLocation.Name + str2, dictionary);
                  Game1.activeClickableMenu = (IClickableMenu) new ShopMenu($"{this.id}_{str2}", dictionary);
                  flag = true;
                }
              }
              bool showedClosedMessage = false;
              if (!flag && Utility.TryOpenShopMenu(str2, this.temporaryLocation, showClosedMessage: (Action<string>) (message =>
              {
                showedClosedMessage = true;
                Game1.drawObjectDialogue(message);
              })))
                flag = true;
              if (!flag)
              {
                if (!showedClosedMessage)
                {
                  Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1714"));
                  break;
                }
                break;
              }
              break;
            case "Message":
              string str4;
              string error2;
              if (!ArgUtility.TryGet(strArray1, 1, out str4, out error2, name: "string translationKey"))
              {
                currentLocation.LogTileActionError(strArray1, tileLocation.X, tileLocation.Y, error2);
                return false;
              }
              Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\StringsFromMaps:" + str4.Replace("\"", "")));
              break;
            case "Dialogue":
              string str5;
              string error3;
              if (!ArgUtility.TryGetRemainder(strArray1, 1, out str5, out error3, name: "string dialogue"))
              {
                currentLocation.LogTileActionError(strArray1, tileLocation.X, tileLocation.Y, error3);
                return false;
              }
              Game1.drawObjectDialogue(str5.Replace("#", " "));
              break;
            case "LuauSoup":
              if (!this.specialEventVariable2)
              {
                Game1.activeClickableMenu = (IClickableMenu) new ItemGrabMenu((IList<Item>) null, true, false, new InventoryMenu.highlightThisItem(Utility.highlightLuauSoupItems), new ItemGrabMenu.behaviorOnItemSelect(this.clickToAddItemToLuauSoup), Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1719"), canBeExitedWithKey: true, context: (object) this);
                break;
              }
              break;
          }
        }
        catch (Exception ex)
        {
        }
        return false;
      }
      if (who.IsLocalPlayer && (!this.playerControlSequence || !this.playerControlSequenceID.Equals("iceFishing")))
      {
        foreach (NPC actor in this.actors)
        {
          Point tilePoint = actor.TilePoint;
          Microsoft.Xna.Framework.Rectangle rectangle = new Microsoft.Xna.Framework.Rectangle(tileLocation.X * 64 /*0x40*/, tileLocation.Y * 64 /*0x40*/, 64 /*0x40*/, 64 /*0x40*/);
          if (tilePoint.X == tileLocation.X && tilePoint.Y == tileLocation.Y && actor is Child child)
          {
            child.checkAction(who, this.temporaryLocation);
            return true;
          }
          if ((tilePoint.X == tileLocation.X && (tilePoint.Y == tileLocation.Y || tilePoint.Y == tileLocation.Y + 1) || actor.GetBoundingBox().Intersects(rectangle)) && (actor.CurrentDialogue.Count >= 1 || actor.CurrentDialogue.Count > 0 && !actor.CurrentDialogue.Peek().isOnFinalDialogue() || actor.Equals((object) this.festivalHost) || actor.datable.Value && this.isSpecificFestival("spring24") || this.secretSantaRecipient != null && actor.Name.Equals(this.secretSantaRecipient.Name)))
          {
            Friendship friendship;
            bool flag = who.friendshipData.TryGetValue(actor.Name, out friendship) && friendship.IsDivorced();
            if ((this.grangeScore > -100 || this.grangeScore == -666) && actor.Equals((object) this.festivalHost) && this.grangeJudged)
            {
              Dialogue dialogue;
              if (this.grangeScore >= 90)
              {
                Game1.playSound("reward");
                dialogue = Dialogue.FromTranslation(actor, "Strings\\StringsFromCSFiles:Event.cs.1723", (object) this.grangeScore);
                Game1.player.festivalScore += 1000;
                Game1.getAchievement(37);
              }
              else if (this.grangeScore >= 75)
              {
                Game1.playSound("reward");
                dialogue = Dialogue.FromTranslation(actor, "Strings\\StringsFromCSFiles:Event.cs.1726", (object) this.grangeScore);
                Game1.player.festivalScore += 500;
              }
              else if (this.grangeScore >= 60)
              {
                Game1.playSound("newArtifact");
                dialogue = Dialogue.FromTranslation(actor, "Strings\\StringsFromCSFiles:Event.cs.1729", (object) this.grangeScore);
                Game1.player.festivalScore += 250;
              }
              else if (this.grangeScore == -666)
              {
                Game1.playSound("secret1");
                dialogue = new Dialogue(actor, "Strings\\StringsFromCSFiles:Event.cs.1730");
                Game1.player.festivalScore += 750;
              }
              else
              {
                Game1.playSound("newArtifact");
                dialogue = Dialogue.FromTranslation(actor, "Strings\\StringsFromCSFiles:Event.cs.1732", (object) this.grangeScore);
                Game1.player.festivalScore += 50;
              }
              this.grangeScore = -100;
              actor.setNewDialogue(dialogue);
            }
            else if ((Game1.HasDedicatedHost || (NetFieldBase<Farmer, NetRef<Farmer>>) Game1.serverHost == (NetRef<Farmer>) null || Game1.player.Equals((object) Game1.serverHost.Value)) && actor.Equals((object) this.festivalHost) && (actor.CurrentDialogue.Count == 0 || actor.CurrentDialogue.Peek().isOnFinalDialogue()) && this.hostMessageKey != null)
              actor.setNewDialogue(this.hostMessageKey);
            if (this.isSpecificFestival("spring24") && !flag && ((int) actor.GetData()?.FlowerDanceCanDance ?? (actor.datable.Value ? 1 : (actor.Name == who.spouse ? 1 : 0))) != 0)
            {
              actor.grantConversationFriendship(who);
              if (who.dancePartner.Value == null)
              {
                if (actor.CurrentDialogue.Count > 0 && actor.CurrentDialogue.Peek().getCurrentDialogue().Equals("..."))
                  actor.CurrentDialogue.Clear();
                if (actor.CurrentDialogue.Count == 0)
                {
                  actor.CurrentDialogue.Push(new Dialogue(actor, (string) null, "..."));
                  if (actor.name.Value == who.spouse)
                    actor.setNewDialogue(Dialogue.FromTranslation(actor, "Strings\\StringsFromCSFiles:Event.cs.1736", (object) actor.displayName), true);
                  else
                    actor.setNewDialogue(Dialogue.FromTranslation(actor, "Strings\\StringsFromCSFiles:Event.cs.1738", (object) actor.displayName), true);
                }
                else if (actor.CurrentDialogue.Peek().isOnFinalDialogue())
                {
                  Dialogue dialogue1 = actor.CurrentDialogue.Peek();
                  if (who.spouse != null && actor.Name == who.spouse)
                  {
                    Dialogue dialogue2 = (Dialogue) null;
                    if (actor.isRoommate())
                      this.TryGetFestivalDialogueForYear(actor, actor.Name + "_roommate", out dialogue2);
                    if (dialogue2 == null)
                      this.TryGetFestivalDialogueForYear(actor, actor.Name + "_spouse", out dialogue2);
                    if (dialogue2 != null)
                    {
                      actor.CurrentDialogue.Clear();
                      actor.CurrentDialogue.Push(dialogue2);
                      dialogue1 = actor.CurrentDialogue.Peek();
                    }
                  }
                  Game1.drawDialogue(actor);
                  actor.faceTowardFarmerForPeriod(3000, 2, false, who);
                  who.Halt();
                  actor.CurrentDialogue = new Stack<Dialogue>();
                  actor.CurrentDialogue.Push(new Dialogue(actor, (string) null, "..."));
                  actor.CurrentDialogue.Push(dialogue1);
                  return true;
                }
              }
              else if (actor.CurrentDialogue.Count > 0 && actor.CurrentDialogue.Peek().getCurrentDialogue().Equals("..."))
                actor.CurrentDialogue.Clear();
            }
            if (!flag && this.secretSantaRecipient != null && actor.Name.Equals(this.secretSantaRecipient.Name))
            {
              actor.grantConversationFriendship(who);
              currentLocation.createQuestionDialogue(Game1.parseText(this.secretSantaRecipient.Gender == Gender.Male ? Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1740", (object) this.secretSantaRecipient.displayName) : Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1741", (object) this.secretSantaRecipient.displayName)), currentLocation.createYesNoResponses(), "secretSanta");
              who.Halt();
              return true;
            }
            if (actor.CurrentDialogue.Count == 0)
              return true;
            if (who.spouse != null && actor.Name == who.spouse && !this.isSpecificFestival("spring24"))
            {
              Dialogue dialogue = (Dialogue) null;
              if (actor.isRoommate())
                this.TryGetFestivalDialogueForYear(actor, actor.Name + "_roommate", out dialogue);
              if (dialogue == null)
                this.TryGetFestivalDialogueForYear(actor, actor.Name + "_spouse", out dialogue);
              if (dialogue != null && (actor.CurrentDialogue.Count == 0 || !actor.CurrentDialogue.Peek().TranslationKey.Equals(dialogue.TranslationKey)))
              {
                actor.CurrentDialogue.Clear();
                actor.CurrentDialogue.Push(dialogue);
              }
            }
            if (flag)
            {
              actor.CurrentDialogue.Clear();
              actor.CurrentDialogue.Push(new Dialogue(actor, $"Characters\\Dialogue\\{actor.Name}:divorced"));
            }
            actor.grantConversationFriendship(who);
            if (actor.CurrentDialogue == null || actor.CurrentDialogue.Count == 0 || !actor.CurrentDialogue.Peek().dontFaceFarmer)
              actor.faceTowardFarmerForPeriod(3000, 2, false, who);
            Game1.drawDialogue(actor);
            who.Halt();
            return true;
          }
        }
      }
      if (this.festivalData != null && this.isSpecificFestival("spring13"))
      {
        Microsoft.Xna.Framework.Rectangle rectangle = new Microsoft.Xna.Framework.Rectangle(tileLocation.X * 64 /*0x40*/, tileLocation.Y * 64 /*0x40*/, 64 /*0x40*/, 64 /*0x40*/);
        for (int index = this.festivalProps.Count - 1; index >= 0; --index)
        {
          if (this.festivalProps[index].isColliding(rectangle))
          {
            ++who.festivalScore;
            this.festivalProps.RemoveAt(index);
            who.team.FestivalPropsRemoved(rectangle);
            if (who.IsLocalPlayer)
              Game1.playSound("coin");
            return true;
          }
        }
      }
      foreach (MapSeat mapSeat in currentLocation.mapSeats)
      {
        if (mapSeat.OccupiesTile(tileLocation.X, tileLocation.Y) && !mapSeat.IsBlocked(currentLocation))
        {
          who.BeginSitting((ISittable) mapSeat);
          return true;
        }
      }
    }
    return false;
  }

  public void removeFestivalProps(Microsoft.Xna.Framework.Rectangle rect)
  {
    this.festivalProps.RemoveAll((Predicate<Prop>) (prop => prop.isColliding(rect)));
  }

  public void checkForSpecialCharacterIconAtThisTile(Vector2 tileLocation)
  {
    if (!this.isFestival || this.festivalHost == null || !(this.festivalHost.Tile == tileLocation))
      return;
    Game1.mouseCursor = Game1.cursor_talk;
  }

  public void forceEndFestival(Farmer who)
  {
    Game1.currentMinigame = (IMinigame) null;
    Game1.exitActiveMenu();
    Game1.player.Halt();
    this.endBehaviors();
    if (Game1.IsServer)
      Game1.multiplayer.sendServerToClientsMessage("endFest");
    Game1.changeMusicTrack("none");
  }

  public bool checkForCollision(Microsoft.Xna.Framework.Rectangle position, Farmer who)
  {
    Microsoft.Xna.Framework.Rectangle boundingBox1 = who.GetBoundingBox();
    foreach (NPC actor in this.actors)
    {
      Microsoft.Xna.Framework.Rectangle boundingBox2 = actor.GetBoundingBox();
      if (boundingBox2.Intersects(position) && !this.farmer.temporarilyInvincible && this.farmer.TemporaryPassableTiles.IsEmpty() && !actor.IsInvisible && !boundingBox1.Intersects(boundingBox2) && !actor.farmerPassesThrough)
        return true;
    }
    if (Game1.currentLocation.IsOutOfBounds(position))
    {
      this.TryStartEndFestivalDialogue(who);
      return true;
    }
    foreach (Object prop in this.props)
    {
      if (prop.GetBoundingBox().Intersects(position))
        return true;
    }
    if (this.temporaryLocation != null)
    {
      foreach (Object @object in this.temporaryLocation.objects.Values)
      {
        if (@object.GetBoundingBox().Intersects(position))
          return true;
      }
    }
    foreach (Prop festivalProp in this.festivalProps)
    {
      if (festivalProp.isColliding(position))
        return true;
    }
    return false;
  }

  /// <summary>Show the dialogue to end the current festival when the player tries to leave the location.</summary>
  /// <param name="who">The local player instance.</param>
  /// <returns>Returns whether the dialogue was displayed.</returns>
  public bool TryStartEndFestivalDialogue(Farmer who)
  {
    if (!who.IsLocalPlayer || !this.isFestival)
      return false;
    who.Halt();
    who.Position = who.lastPosition;
    if (!Game1.IsMultiplayer && Game1.activeClickableMenu == null)
      Game1.activeClickableMenu = (IClickableMenu) new ConfirmationDialog(Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1758", (object) this.FestivalName), new ConfirmationDialog.behavior(this.forceEndFestival));
    else if (Game1.activeClickableMenu == null)
    {
      Game1.netReady.SetLocalReady("festivalEnd", true);
      Game1.activeClickableMenu = (IClickableMenu) new ReadyCheckDialog("festivalEnd", true, new ConfirmationDialog.behavior(this.forceEndFestival));
    }
    return true;
  }

  public void answerDialogue(string questionKey, int answerChoice)
  {
    this.previousAnswerChoice = answerChoice;
    if (questionKey.Contains("fork"))
    {
      int int32 = Convert.ToInt32(questionKey.Replace("fork", ""));
      if (answerChoice != int32)
        return;
      this.specialEventVariable1 = !this.specialEventVariable1;
    }
    else if (questionKey.Contains("quickQuestion"))
    {
      string eventCommand = this.eventCommands[Math.Min(this.eventCommands.Length - 1, this.CurrentCommand)];
      string[] collection = eventCommand.Substring(eventCommand.IndexOf(' ') + 1).Split("(break)")[1 + answerChoice].Split('\\');
      List<string> list = ((IEnumerable<string>) this.eventCommands).ToList<string>();
      list.InsertRange(this.CurrentCommand + 1, (IEnumerable<string>) collection);
      this.eventCommands = list.ToArray();
    }
    else
    {
      if (questionKey == null)
        return;
      switch (questionKey.Length)
      {
        case 3:
          if (!(questionKey == "pet"))
            break;
          if (answerChoice == 0)
          {
            Game1.activeClickableMenu = (IClickableMenu) new NamingMenu(new NamingMenu.doneNamingBehavior(this.namePet), Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1236"), !Game1.player.IsMale ? Game1.content.LoadString(Game1.player.whichPetType == "Dog" ? "Strings\\StringsFromCSFiles:Event.cs.1797" : "Strings\\StringsFromCSFiles:Event.cs.1796") : Game1.content.LoadString(Game1.player.catPerson ? "Strings\\StringsFromCSFiles:Event.cs.1794" : "Strings\\StringsFromCSFiles:Event.cs.1795"));
            break;
          }
          Game1.player.team.RequestSetMail(PlayerActionTarget.Host, "rejectedPet", MailType.Received, true);
          this.eventCommands = new string[2];
          this.eventCommands[1] = "end";
          this.eventCommands[0] = $"speak Marnie \"{Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1798")}\"";
          this.currentCommand = 0;
          this.eventSwitched = true;
          this.specialEventVariable1 = true;
          break;
        case 4:
          if (!(questionKey == "cave"))
            break;
          Game1.dedicatedServer.DoHostAction("ChooseCave", (object) answerChoice);
          break;
        case 8:
          switch (questionKey[0])
          {
            case 'b':
              if (!(questionKey == "bandFork"))
                return;
              switch (answerChoice)
              {
                case 76:
                  this.specialEventVariable1 = true;
                  this.eventCommands[this.currentCommand + 1] = "fork poppy";
                  return;
                case 77:
                  this.specialEventVariable1 = true;
                  this.eventCommands[this.currentCommand + 1] = "fork heavy";
                  return;
                case 78:
                  this.specialEventVariable1 = true;
                  this.eventCommands[this.currentCommand + 1] = "fork techno";
                  return;
                case 79:
                  this.specialEventVariable1 = true;
                  this.eventCommands[this.currentCommand + 1] = "fork honkytonk";
                  return;
                default:
                  return;
              }
            case 'w':
              if (!(questionKey == "wheelBet"))
                return;
              this.specialEventVariable2 = answerChoice == 1;
              if (answerChoice == 2)
                return;
              Game1.activeClickableMenu = (IClickableMenu) new NumberSelectionMenu(Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1776"), new NumberSelectionMenu.behaviorOnNumberSelect(this.betStarTokens), minValue: 1, maxValue: Game1.player.festivalScore, defaultNumber: Math.Min(1, Game1.player.festivalScore));
              return;
            default:
              return;
          }
        case 9:
          if (!(questionKey == "shaneLoan"))
            break;
          if (answerChoice != 0)
            break;
          this.specialEventVariable1 = true;
          this.eventCommands[this.currentCommand + 1] = "fork giveShaneLoan";
          Game1.player.Money -= 3000;
          break;
        case 11:
          switch (questionKey[1])
          {
            case 'e':
              if (!(questionKey == "secretSanta") || answerChoice != 0)
                return;
              Game1.activeClickableMenu = (IClickableMenu) new ItemGrabMenu((IList<Item>) null, true, false, new InventoryMenu.highlightThisItem(Utility.highlightSantaObjects), new ItemGrabMenu.behaviorOnItemSelect(this.chooseSecretSantaGift), Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1788", (object) this.secretSantaRecipient.displayName), context: (object) this);
              return;
            case 'f':
              return;
            case 'g':
              return;
            case 'h':
              if (!(questionKey == "shaneCliffs"))
                return;
              switch (answerChoice)
              {
                case 0:
                  this.eventCommands[this.currentCommand + 2] = $"speak Shane \"{Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1760")}\"";
                  return;
                case 1:
                  this.eventCommands[this.currentCommand + 2] = $"speak Shane \"{Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1761")}\"";
                  return;
                case 2:
                  this.eventCommands[this.currentCommand + 2] = $"speak Shane \"{Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1763")}\"";
                  return;
                case 3:
                  this.eventCommands[this.currentCommand + 2] = $"speak Shane \"{Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1764")}\"";
                  return;
                default:
                  return;
              }
            case 'i':
              if (!(questionKey == "fishingGame") || answerChoice != 0)
                return;
              if (Game1.player.Money >= 50)
              {
                Game1.globalFadeToBlack(new Game1.afterFadeFunction(FishingGame.startMe), 0.01f);
                Game1.player.Money -= 50;
                return;
              }
              Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1780"));
              return;
            default:
              return;
          }
        case 13:
          switch (questionKey[0])
          {
            case 'S':
              if (!(questionKey == "StarTokenShop") || answerChoice != 0)
                return;
              Game1.activeClickableMenu = (IClickableMenu) new NumberSelectionMenu(Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1774"), new NumberSelectionMenu.behaviorOnNumberSelect(this.buyStarTokens), 50, maxValue: 999);
              return;
            case 'f':
              if (!(questionKey == "fortuneTeller") || answerChoice != 0)
                return;
              Game1.globalFadeToBlack(new Game1.afterFadeFunction(this.readFortune));
              Game1.player.Money -= 100;
              Game1.player.mailReceived.Add("fortuneTeller" + Game1.year.ToString());
              return;
            case 'h':
              if (!(questionKey == "haleyDarkRoom"))
                return;
              switch (answerChoice)
              {
                case 0:
                  this.specialEventVariable1 = true;
                  this.eventCommands[this.currentCommand + 1] = "fork decorate";
                  return;
                case 1:
                  this.specialEventVariable1 = true;
                  this.eventCommands[this.currentCommand + 1] = "fork leave";
                  return;
                case 2:
                  return;
                default:
                  return;
              }
            case 's':
              switch (questionKey)
              {
                case "slingshotGame":
                  if (answerChoice != 0)
                    return;
                  if (Game1.player.Money >= 50)
                  {
                    Game1.globalFadeToBlack(new Game1.afterFadeFunction(TargetGame.startMe), 0.01f);
                    Game1.player.Money -= 50;
                    return;
                  }
                  Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1780"));
                  return;
                case "starTokenShop":
                  if (answerChoice != 0 || !Utility.TryOpenShopMenu("Festival_StardewValleyFair_StarTokens", this.temporaryLocation, playOpenSound: false) || !(Game1.activeClickableMenu is ShopMenu activeClickableMenu))
                    return;
                  if (activeClickableMenu.IsOutOfStock())
                  {
                    activeClickableMenu.exitThisMenuNoSound();
                    Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1785")));
                    return;
                  }
                  activeClickableMenu.PlayOpenSound();
                  return;
                default:
                  return;
              }
            default:
              return;
          }
        case 15:
          if (!(questionKey == "chooseCharacter"))
            break;
          switch (answerChoice)
          {
            case 0:
              this.specialEventVariable1 = true;
              this.eventCommands[this.currentCommand + 1] = "fork warrior";
              return;
            case 1:
              this.specialEventVariable1 = true;
              this.eventCommands[this.currentCommand + 1] = "fork healer";
              return;
            case 2:
              return;
            default:
              return;
          }
        case 20:
          if (!(questionKey == "spirits_eve_shortcut") || answerChoice != 0)
            break;
          Game1.player.freezePause = 2000;
          Game1.globalFadeToBlack((Game1.afterFadeFunction) (() =>
          {
            Game1.player.Position = new Vector2(32f, 49f) * 64f;
            Game1.player.faceDirection(2);
            Game1.playSound("stairsdown");
            Game1.globalFadeToClear();
          }));
          break;
      }
    }
  }

  internal static void hostActionChooseCave(Farmer who, BinaryReader reader)
  {
    if (reader.ReadInt32() == 0)
    {
      Game1.MasterPlayer.caveChoice.Value = 2;
      Game1.RequireLocation<FarmCave>("FarmCave").setUpMushroomHouse();
    }
    else
      Game1.MasterPlayer.caveChoice.Value = 1;
  }

  internal static void hostActionNamePet(Farmer who, BinaryReader reader)
  {
    string str = reader.ReadString();
    Pet pet = new Pet(68, 13, Game1.player.whichPetBreed, Game1.player.whichPetType);
    pet.warpToFarmHouse(Game1.player);
    pet.Name = str;
    pet.displayName = pet.name.Value;
    foreach (Building building in Game1.getFarm().buildings)
    {
      if (building is PetBowl petBowl && !petBowl.HasPet())
      {
        petBowl.AssignPet(pet);
        break;
      }
    }
    foreach (Farmer allFarmer in Game1.getAllFarmers())
      allFarmer.autoGenerateActiveDialogueEvent("gotPet");
  }

  private void namePet(string name)
  {
    this.gotPet = true;
    Game1.dedicatedServer.DoHostAction("NamePet", (object) name);
    Game1.exitActiveMenu();
    ++this.CurrentCommand;
  }

  public void chooseSecretSantaGift(Item i, Farmer who)
  {
    if (i == null)
      return;
    Object obj = i as Object;
    if (obj != null)
    {
      if (obj.Stack > 1)
      {
        --obj.Stack;
        who.addItemToInventory((Item) obj);
      }
      Game1.exitActiveMenu();
      NPC recipient = this.getActorByName(this.secretSantaRecipient.Name);
      recipient.faceTowardFarmerForPeriod(15000, 5, false, who);
      recipient.receiveGift(obj, who, false, 5f, false);
      recipient.CurrentDialogue.Clear();
      string article = Lexicon.getProperArticleForWord(obj.DisplayName);
      Stack<Dialogue> currentDialogue = recipient.CurrentDialogue;
      Dialogue dialogue1 = recipient.TryGetDialogue("WinterStar_ReceiveGift_" + obj.QualifiedItemId, (object) obj.DisplayName, (object) article);
      if (dialogue1 == null)
      {
        Dialogue dialogue2 = obj.GetContextTags().Select<string, Dialogue>((Func<string, Dialogue>) (tag => recipient.TryGetDialogue("WinterStar_ReceiveGift_" + tag, (object) obj.DisplayName, (object) article))).FirstOrDefault<Dialogue>((Func<Dialogue, bool>) (p => p != null));
        if (dialogue2 == null)
          dialogue1 = recipient.TryGetDialogue("WinterStar_ReceiveGift", (object) obj.DisplayName, (object) article) ?? Dialogue.FromTranslation(recipient, "Strings\\StringsFromCSFiles:Event.cs.1801", (object) obj.DisplayName, (object) article);
        else
          dialogue1 = dialogue2;
      }
      currentDialogue.Push(dialogue1);
      Game1.drawDialogue(recipient);
      this.secretSantaRecipient = (NPC) null;
      this.startSecretSantaAfterDialogue = true;
      who.Halt();
      who.completelyStopAnimatingOrDoingAction();
      who.faceGeneralDirection(recipient.Position, 0, false, false);
    }
    else
      Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1803"));
  }

  public void perfectFishing()
  {
    if (!this.isFestival || !(Game1.currentMinigame is FishingGame currentMinigame) || !this.isSpecificFestival("fall16"))
      return;
    ++currentMinigame.perfections;
  }

  public void caughtFish(string itemId, int size, Farmer who)
  {
    if (itemId == null || !this.isFestival)
      return;
    if (Game1.currentMinigame is FishingGame currentMinigame && this.isSpecificFestival("fall16"))
    {
      currentMinigame.score += size > 0 ? size + 5 : 1;
      if (size > 0)
        ++currentMinigame.fishCaught;
      Game1.player.FarmerSprite.PauseForSingleAnimation = false;
      Game1.player.FarmerSprite.StopAnimation();
    }
    else
    {
      if (!this.isSpecificFestival("winter8"))
        return;
      if (size > 0 && who.TilePoint.X < 79 && who.TilePoint.Y < 43)
      {
        ++who.festivalScore;
        Game1.playSound("newArtifact");
      }
      who.forceCanMove();
      if (this.previousFacingDirection == -1)
        return;
      who.faceDirection(this.previousFacingDirection);
    }
  }

  public void readFortune()
  {
    Game1.globalFade = true;
    Game1.fadeToBlackAlpha = 1f;
    NPC romanticInterest1 = Utility.getTopRomanticInterest(Game1.player);
    NPC romanticInterest2 = Utility.getTopNonRomanticInterest(Game1.player);
    int highestSkill = Utility.getHighestSkill(Game1.player);
    string[] messages = new string[5];
    if (romanticInterest2 != null && Game1.player.getFriendshipLevelForNPC(romanticInterest2.Name) > 100)
    {
      if (Utility.getNumberOfFriendsWithinThisRange(Game1.player, Game1.player.getFriendshipLevelForNPC(romanticInterest2.Name) - 100, Game1.player.getFriendshipLevelForNPC(romanticInterest2.Name)) > 3 && Game1.random.NextBool())
      {
        messages[0] = Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1810");
      }
      else
      {
        switch (Game1.random.Next(4))
        {
          case 0:
            messages[0] = Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1811", (object) romanticInterest2.displayName);
            break;
          case 1:
            messages[0] = Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1813", (object) romanticInterest2.displayName) + (romanticInterest2.Gender == Gender.Male ? Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1815") : Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1816"));
            break;
          case 2:
            messages[0] = Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1818", (object) romanticInterest2.displayName);
            break;
          case 3:
            messages[0] = (romanticInterest2.Gender == Gender.Male ? Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1820") : Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1821")) + Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1823", (object) romanticInterest2.displayName);
            break;
        }
      }
    }
    else
      messages[0] = Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1825");
    if (romanticInterest1 != null && Game1.player.getFriendshipLevelForNPC(romanticInterest1.Name) > 250)
    {
      if (Utility.getNumberOfFriendsWithinThisRange(Game1.player, Game1.player.getFriendshipLevelForNPC(romanticInterest1.Name) - 100, Game1.player.getFriendshipLevelForNPC(romanticInterest1.Name), true) > 2)
      {
        messages[1] = Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1826");
      }
      else
      {
        switch (Game1.random.Next(4))
        {
          case 0:
            messages[1] = Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1827", (object) romanticInterest1.displayName);
            break;
          case 1:
            messages[1] = Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1829", (object) romanticInterest1.displayName);
            break;
          case 2:
            messages[1] = $"{(romanticInterest1.Gender == Gender.Male ? (romanticInterest1.SocialAnxiety == 1 ? Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1831") : Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1832")) : (romanticInterest1.SocialAnxiety == 1 ? Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1833") : Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1834")))} {(romanticInterest1.Gender == Gender.Male ? Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1837", (object) romanticInterest1.displayName[0]) : Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1838", (object) romanticInterest1.displayName[0]))}";
            break;
          case 3:
            messages[1] = Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1843", (object) romanticInterest1.displayName);
            break;
        }
      }
    }
    else
      messages[1] = Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1845");
    switch (highestSkill)
    {
      case 0:
        messages[2] = Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1846");
        break;
      case 1:
        messages[2] = Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1849");
        break;
      case 2:
        messages[2] = Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1850");
        break;
      case 3:
        messages[2] = Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1847");
        break;
      case 4:
        messages[2] = Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1848");
        break;
      case 5:
        messages[2] = Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1851");
        break;
    }
    messages[3] = Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1852");
    messages[4] = Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1853");
    Game1.multipleDialogues(messages);
    Game1.afterDialogues = new Game1.afterFadeFunction(this.fadeClearAndviewportUnfreeze);
    Game1.viewportFreeze = true;
    Game1.viewport.X = -9999;
  }

  public void fadeClearAndviewportUnfreeze()
  {
    Game1.fadeClear();
    Game1.viewportFreeze = false;
  }

  public void betStarTokens(int value, int price, Farmer who)
  {
    if (value > who.festivalScore)
      return;
    Game1.playSound("smallSelect");
    Game1.activeClickableMenu = (IClickableMenu) new WheelSpinGame(value);
  }

  public void buyStarTokens(int value, int price, Farmer who)
  {
    if (value <= 0 || value * price > who.Money)
      return;
    who.Money -= price * value;
    who.festivalScore += value;
    Game1.playSound("purchase");
    Game1.exitActiveMenu();
  }

  public void clickToAddItemToLuauSoup(Item i, Farmer who) => this.addItemToLuauSoup(i, who);

  public void setUpAdvancedMove(string[] args, NPCController.endBehavior endBehavior = null)
  {
    string str;
    string error;
    bool loop;
    if (!ArgUtility.TryGet(args, 1, out str, out error, false, "string actorName") || !ArgUtility.TryGetBool(args, 2, out loop, out error, "bool loop"))
    {
      this.LogCommandError(args, error);
    }
    else
    {
      List<Vector2> path = new List<Vector2>();
      for (int index = 3; index < args.Length; index += 2)
      {
        Vector2 vector2;
        if (ArgUtility.TryGetVector2(args, index, out vector2, out error, true, "Vector2 tile"))
          path.Add(vector2);
        else
          this.LogCommandError(args, error);
      }
      if (this.npcControllers == null)
        this.npcControllers = new List<NPCController>();
      int farmerNumber;
      if (this.IsFarmerActorId(str, out farmerNumber))
      {
        Farmer farmerActor = this.GetFarmerActor(farmerNumber);
        if (farmerActor == null)
          return;
        this.npcControllers.Add(new NPCController((Character) farmerActor, path, loop, endBehavior));
      }
      else
      {
        NPC actorByName = this.getActorByName(str, true);
        if (actorByName == null)
          return;
        this.npcControllers.Add(new NPCController((Character) actorByName, path, loop, endBehavior));
      }
    }
  }

  public static bool IsItemMayorShorts(Item i)
  {
    return i?.QualifiedItemId == "(O)789" || i?.QualifiedItemId == "(O)71";
  }

  public void addItemToLuauSoup(Item i, Farmer who)
  {
    if (i == null)
      return;
    who.team.luauIngredients.Add(i.getOne());
    if (!who.IsLocalPlayer)
      return;
    this.specialEventVariable2 = true;
    bool flag = Event.IsItemMayorShorts(i);
    if (i != null && i.Stack > 1 && !flag)
    {
      --i.Stack;
      who.addItemToInventory(i);
    }
    else if (flag)
      who.addItemToInventory(i);
    Game1.exitActiveMenu();
    Game1.playSound("dropItemInWater");
    if (i != null)
      Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1857", (object) i.DisplayName));
    string str = "";
    switch (i.Quality)
    {
      case 1:
        str = " ([51])";
        break;
      case 2:
        str = " ([52])";
        break;
      case 4:
        str = " ([53])";
        break;
    }
    if (flag)
      return;
    Game1.multiplayer.globalChatInfoMessage("LuauSoup", Game1.player.Name, TokenStringBuilder.ItemNameFor(i) + str);
  }

  private void governorTaste()
  {
    int num1 = 5;
    foreach (Item luauIngredient in Game1.player.team.luauIngredients)
    {
      Object i = luauIngredient as Object;
      int num2 = 5;
      if (Event.IsItemMayorShorts((Item) i))
      {
        num1 = 6;
        break;
      }
      if (i.Quality >= 2 && i.price.Value >= 160 /*0xA0*/ || i.Quality == 1 && i.price.Value >= 300 && i.edibility.Value > 10)
      {
        num2 = 4;
        Utility.improveFriendshipWithEveryoneInRegion(Game1.player, 120, "Town");
      }
      else if (i.edibility.Value >= 20 || i.price.Value >= 100 || i.price.Value >= 70 && i.Quality >= 1)
      {
        num2 = 3;
        Utility.improveFriendshipWithEveryoneInRegion(Game1.player, 60, "Town");
      }
      else if (i.price.Value > 20 && i.edibility.Value >= 10 || i.price.Value >= 40 && i.edibility.Value >= 5)
        num2 = 2;
      else if (i.edibility.Value >= 0)
      {
        num2 = 1;
        Utility.improveFriendshipWithEveryoneInRegion(Game1.player, -50, "Town");
      }
      if (i.edibility.Value > -300 && i.edibility.Value < 0)
      {
        num2 = 0;
        Utility.improveFriendshipWithEveryoneInRegion(Game1.player, -100, "Town");
      }
      if (num2 < num1)
        num1 = num2;
    }
    int num3 = Game1.numberOfPlayers() - (Game1.HasDedicatedHost ? 1 : 0);
    if (num1 != 6 && Game1.player.team.luauIngredients.Count < num3)
      num1 = 5;
    this.eventCommands[this.CurrentCommand + 1] = "switchEvent governorReaction" + num1.ToString();
    if (num1 != 4)
      return;
    Game1.getAchievement(38);
  }

  private void eggHuntWinner()
  {
    int num;
    switch (Game1.numberOfPlayers() - (Game1.HasDedicatedHost ? 1 : 0))
    {
      case 1:
        num = 9;
        break;
      case 2:
        num = 6;
        break;
      case 3:
        num = 5;
        break;
      default:
        num = 4;
        break;
    }
    List<Farmer> farmerList = new List<Farmer>();
    int festivalScore = Game1.player.festivalScore;
    foreach (Farmer onlineFarmer in Game1.getOnlineFarmers())
    {
      if (onlineFarmer.festivalScore > festivalScore)
        festivalScore = onlineFarmer.festivalScore;
    }
    foreach (Farmer onlineFarmer in Game1.getOnlineFarmers())
    {
      if (onlineFarmer.festivalScore == festivalScore)
      {
        farmerList.Add(onlineFarmer);
        this.festivalWinners.Add(onlineFarmer.UniqueMultiplayerID);
      }
    }
    string dialogueText = Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1862");
    if (festivalScore >= num)
    {
      foreach (Farmer farmer in farmerList)
        farmer.autoGenerateActiveDialogueEvent("wonEggHunt");
      if (farmerList.Count == 1)
      {
        dialogueText = LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.es ? $"¡{farmerList[0].displayName}!" : farmerList[0].displayName + "!";
      }
      else
      {
        string str = Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1864");
        for (int index = 0; index < farmerList.Count; ++index)
        {
          if (index == farmerList.Count - 1)
            str += Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1865");
          str = $"{str} {farmerList[index].displayName}";
          if (index < farmerList.Count - 1)
            str += ",";
        }
        dialogueText = str + "!";
      }
      this.specialEventVariable1 = false;
    }
    else
      this.specialEventVariable1 = true;
    NPC actorByName = this.getActorByName("Lewis");
    actorByName.CurrentDialogue.Push(new Dialogue(actorByName, (string) null, dialogueText));
    Game1.drawDialogue(actorByName);
  }

  private void iceFishingWinner()
  {
    int num = 5;
    this.iceFishWinners = new List<Farmer>();
    int festivalScore = Game1.player.festivalScore;
    for (int farmerNumber = 1; farmerNumber <= Game1.numberOfPlayers(); ++farmerNumber)
    {
      Farmer farmerActor = this.GetFarmerActor(farmerNumber);
      if (farmerActor != null && farmerActor.festivalScore > festivalScore)
        festivalScore = farmerActor.festivalScore;
    }
    for (int farmerNumber = 1; farmerNumber <= Game1.numberOfPlayers(); ++farmerNumber)
    {
      Farmer farmerActor = this.GetFarmerActor(farmerNumber);
      if (farmerActor != null && farmerActor.festivalScore == festivalScore)
      {
        this.iceFishWinners.Add(farmerActor);
        this.festivalWinners.Add(farmerActor.UniqueMultiplayerID);
      }
    }
    string dialogueText = Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1871");
    if (festivalScore >= num)
    {
      foreach (Farmer iceFishWinner in this.iceFishWinners)
        iceFishWinner.autoGenerateActiveDialogueEvent("wonIceFishing");
      if (this.iceFishWinners.Count == 1)
      {
        dialogueText = Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1872", (object) this.iceFishWinners[0].displayName, (object) this.iceFishWinners[0].festivalScore);
      }
      else
      {
        string str = Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1864");
        for (int index = 0; index < this.iceFishWinners.Count; ++index)
        {
          if (index == this.iceFishWinners.Count - 1)
            str += Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1865");
          str = $"{str} {this.iceFishWinners[index].displayName}";
          if (index < this.iceFishWinners.Count - 1)
            str += ",";
        }
        dialogueText = str + "!";
      }
      this.specialEventVariable1 = false;
    }
    else
      this.specialEventVariable1 = true;
    NPC actorByName = this.getActorByName("Lewis");
    actorByName.CurrentDialogue.Push(new Dialogue(actorByName, (string) null, dialogueText));
    Game1.drawDialogue(actorByName);
  }

  private void iceFishingWinnerMP()
  {
    this.specialEventVariable1 = !this.iceFishWinners.Contains(Game1.player);
  }

  public void popBalloons(int x, int y)
  {
    if (!this.id.Equals("191393") && !this.id.Equals("502261") || this.aboveMapSprites == null)
      return;
    List<int> idsToRemove = new List<int>();
    for (int index1 = this.aboveMapSprites.Count - 1; index1 >= 0; --index1)
    {
      TemporaryAnimatedSprite aboveMapSprite = this.aboveMapSprites[index1];
      int width = aboveMapSprite.sourceRect.Width * 4;
      int height = aboveMapSprite.sourceRect.Height * 4;
      Microsoft.Xna.Framework.Rectangle r = new Microsoft.Xna.Framework.Rectangle((int) aboveMapSprite.Position.X, (int) aboveMapSprite.Position.Y, width, height);
      if (r.Contains(x, y))
      {
        idsToRemove.Add(aboveMapSprite.id);
        if (aboveMapSprite.sourceRect.Height <= 16 /*0x10*/)
        {
          for (int index2 = 0; index2 < 3; ++index2)
            this.aboveMapSprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(280 + Game1.random.Choose<int>(8, 0), 1954, 8, 8), 1000f, 1, 99, Utility.getRandomPositionInThisRectangle(r, Game1.random), false, false, 1f, 0.0f, aboveMapSprite.color, 4f, 0.0f, 0.0f, (float) Game1.random.Next(-10, 11) / 100f)
            {
              motion = new Vector2((float) Game1.random.Next(-4, 5), (float) ((double) Game1.random.Next(-10, 1) / 100.0 - 8.0)),
              acceleration = new Vector2(0.0f, 0.3f),
              local = true
            });
        }
      }
    }
    this.aboveMapSprites.RemoveWhere<TemporaryAnimatedSprite>((Predicate<TemporaryAnimatedSprite>) (sprite => sprite.id == 9988 || idsToRemove.Contains(sprite.id)));
    if (idsToRemove.Count <= 0)
      return;
    ++this.int_useMeForAnything;
    this.aboveMapSprites.Add(new TemporaryAnimatedSprite((string) null, Microsoft.Xna.Framework.Rectangle.Empty, new Vector2(16f, 16f), false, 0.0f, Color.White)
    {
      text = this.int_useMeForAnything.ToString() ?? "",
      layerDepth = 1f,
      animationLength = 1,
      totalNumberOfLoops = 10,
      interval = 300f,
      scale = 2f,
      local = true,
      id = 9988
    });
    Game1.playSound("coin");
  }

  /// <summary>Auto-generate a default light source ID for an event.</summary>
  /// <param name="suffix">A suffix which distinguishes the specific light source for this event.</param>
  public virtual string GenerateLightSourceId(string suffix)
  {
    return $"{nameof (Event)}_{this.id}_{suffix}";
  }

  /// <summary>The low-level event commands defined by the base game. Most code should use <see cref="T:StardewValley.Event" /> methods instead.</summary>
  /// <remarks>Every method within this class is an event command whose name matches the method name. All event commands must be static, public, and match <see cref="T:StardewValley.Delegates.EventCommandDelegate" />.</remarks>
  public static class DefaultCommands
  {
    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void IgnoreEventTileOffset(Event @event, string[] args, EventContext context)
    {
      @event.ignoreTileOffsets = true;
      ++@event.CurrentCommand;
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void Move(Event @event, string[] args, EventContext context)
    {
      bool? nullable = new bool?();
      int num = (args.Length - 1) % 4;
      if (num == 1)
      {
        bool flag;
        string error;
        if (!ArgUtility.TryGetOptionalBool(args, args.Length - 1, out flag, out error, name: "bool rawValue"))
        {
          context.LogErrorAndSkip(error);
          return;
        }
        nullable = new bool?(flag);
      }
      else if (num > 1)
      {
        context.LogErrorAndSkip("invalid number of arguments, expected sets of [actor x y direction] fields plus an optional continue-after-move boolean field");
        return;
      }
      if (!nullable.HasValue || args.Length > 2)
      {
        for (int index = 1; index < args.Length && ArgUtility.HasIndex<string>(args, index + 3); index += 4)
        {
          string str;
          string error;
          Point p;
          int facingDirection;
          if (!ArgUtility.TryGet(args, index, out str, out error, name: "string actorName") || !ArgUtility.TryGetPoint(args, index + 1, out p, out error, "Point tile") || !ArgUtility.TryGetDirection(args, index + 3, out facingDirection, out error, "int facingDirection"))
          {
            context.LogError(error);
          }
          else
          {
            int farmerNumber;
            if (@event.IsFarmerActorId(str, out farmerNumber))
            {
              if (!@event.actorPositionsAfterMove.ContainsKey(str))
              {
                Farmer farmerActor = @event.GetFarmerActor(farmerNumber);
                if (farmerActor != null)
                {
                  farmerActor.canOnlyWalk = false;
                  farmerActor.setRunning(false, true);
                  farmerActor.canOnlyWalk = true;
                  farmerActor.convertEventMotionCommandToMovement(Utility.PointToVector2(p));
                  @event.actorPositionsAfterMove.Add(str, @event.getPositionAfterMove((Character) farmerActor, p.X, p.Y, facingDirection));
                }
              }
            }
            else
            {
              bool isOptionalNpc;
              NPC actorByName = @event.getActorByName(str, out isOptionalNpc);
              if (actorByName == null)
              {
                if (!isOptionalNpc)
                {
                  context.LogErrorAndSkip($"no NPC found with name '{str}'");
                  return;
                }
              }
              else if (!@event.actorPositionsAfterMove.ContainsKey(actorByName.Name))
              {
                actorByName.convertEventMotionCommandToMovement(Utility.PointToVector2(p));
                @event.actorPositionsAfterMove.Add(actorByName.Name, @event.getPositionAfterMove((Character) actorByName, p.X, p.Y, facingDirection));
              }
            }
          }
        }
      }
      if (!nullable.HasValue)
        return;
      if (nullable.GetValueOrDefault())
      {
        @event.continueAfterMove = true;
        ++@event.CurrentCommand;
      }
      else
      {
        @event.continueAfterMove = false;
        if (args.Length != 2 || @event.actorPositionsAfterMove.Count != 0)
          return;
        ++@event.CurrentCommand;
      }
    }

    /// <summary>Run an action string.</summary>
    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void Action(Event @event, string[] args, EventContext context)
    {
      string action;
      string error;
      if (!ArgUtility.TryGetRemainder(args, 1, out action, out error, name: "string action"))
      {
        context.LogErrorAndSkip(error);
      }
      else
      {
        Exception exception;
        if (!TriggerActionManager.TryRunAction(action, out error, out exception))
        {
          if (exception != null)
            error += $"\n{exception}";
          context.LogErrorAndSkip(error);
        }
        else
          ++@event.CurrentCommand;
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void Speak(Event @event, string[] args, EventContext context)
    {
      if (@event.skipped)
        return;
      string name;
      string error;
      string str;
      if (!ArgUtility.TryGet(args, 1, out name, out error, name: "string actorName") || !ArgUtility.TryGet(args, 2, out str, out error, name: "string textOrTranslationKey"))
      {
        context.LogErrorAndSkip(error);
      }
      else
      {
        if (Game1.dialogueUp)
          return;
        @event.timeAccumulator += (float) context.Time.ElapsedGameTime.Milliseconds;
        if ((double) @event.timeAccumulator < 500.0)
          return;
        @event.timeAccumulator = 0.0f;
        bool isOptionalNpc;
        NPC n = @event.getActorByName(name, out isOptionalNpc) ?? Game1.getCharacterFromName(name.TrimEnd('?'));
        if (n == null)
        {
          context.LogErrorAndSkip($"no NPC found with name '{name}'", isOptionalNpc);
          if (isOptionalNpc)
            return;
          Game1.eventFinished();
        }
        else
        {
          Game1.player.NotifyQuests((Func<Quest, bool>) (quest => quest.OnNpcSocialized(n)));
          if (n.CanSocialize && !Game1.player.friendshipData.ContainsKey(n.Name))
            Game1.player.friendshipData.Add(n.Name, new Friendship(0));
          Dialogue dialogue = Game1.content.IsValidTranslationKey(str) ? new Dialogue(n, str) : new Dialogue(n, (string) null, str);
          n.CurrentDialogue.Push(dialogue);
          Game1.drawDialogue(n);
        }
      }
    }

    /// <summary>Try to execute all commands in one tick until <see cref="M:StardewValley.Event.DefaultCommands.EndSimultaneousCommand(StardewValley.Event,System.String[],StardewValley.EventContext)" /> is called.</summary>
    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void BeginSimultaneousCommand(Event @event, string[] args, EventContext context)
    {
      @event.simultaneousCommand = true;
      ++@event.CurrentCommand;
    }

    /// <summary>If commands are being executed in one tick due to <see cref="M:StardewValley.Event.DefaultCommands.BeginSimultaneousCommand(StardewValley.Event,System.String[],StardewValley.EventContext)" />, stop doing so for the remaining commands.</summary>
    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void EndSimultaneousCommand(Event @event, string[] args, EventContext context)
    {
      @event.simultaneousCommand = false;
      ++@event.CurrentCommand;
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void MineDeath(Event @event, string[] args, EventContext context)
    {
      if (Game1.dialogueUp)
        return;
      Random daySaveRandom = Utility.CreateDaySaveRandom((double) Game1.timeOfDay);
      int num1 = Math.Min(daySaveRandom.Next(Game1.player.Money / 40, Game1.player.Money / 8), 15000);
      int num2 = num1 - (int) ((double) Game1.player.LuckLevel * 0.01 * (double) num1);
      int sub1_1 = num2 - num2 % 100;
      int sub1_2 = Game1.player.LoseItemsOnDeath(daySaveRandom);
      Game1.player.Stamina = Math.Min(Game1.player.Stamina, 2f);
      Game1.player.Money = Math.Max(0, Game1.player.Money - sub1_1);
      Game1.drawObjectDialogue($"{Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1057")} {(sub1_1 <= 0 ? "" : Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1058", (object) sub1_1))}{(sub1_2 > 0 ? (sub1_1 <= 0 ? Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1060") + (sub1_2 == 1 ? Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1061") : Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1062", (object) sub1_2)) : Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1063") + (sub1_2 == 1 ? Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1061") : Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1062", (object) sub1_2))) : (sub1_1 <= 0 ? "" : "."))}");
      @event.InsertNextCommand("showItemsLost");
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void HospitalDeath(Event @event, string[] args, EventContext context)
    {
      if (Game1.dialogueUp)
        return;
      int sub1_1 = Game1.player.LoseItemsOnDeath();
      Game1.player.Stamina = Math.Min(Game1.player.Stamina, 2f);
      int sub1_2 = Math.Min(1000, Game1.player.Money);
      Game1.player.Money -= sub1_2;
      Game1.drawObjectDialogue((sub1_2 > 0 ? Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1068", (object) sub1_2) : Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1070")) + (sub1_1 > 0 ? Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1071") + (sub1_1 == 1 ? Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1061") : Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1062", (object) sub1_1)) : ""));
      @event.InsertNextCommand("showItemsLost");
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void ShowItemsLost(Event @event, string[] args, EventContext context)
    {
      if (Game1.activeClickableMenu != null)
        return;
      Game1.activeClickableMenu = (IClickableMenu) new ItemListMenu(Game1.content.LoadString("Strings\\UI:ItemList_ItemsLost"), Game1.player.itemsLostLastDeath.ToList<Item>());
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void End(Event @event, string[] args, EventContext context)
    {
      @event.endBehaviors(args, context.Location);
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void LocationSpecificCommand(Event @event, string[] args, EventContext context)
    {
      string command_string;
      string error;
      if (!ArgUtility.TryGet(args, 1, out command_string, out error, name: "string command"))
      {
        context.LogErrorAndSkip(error);
      }
      else
      {
        string[] array = ((IEnumerable<string>) args).Skip<string>(2).ToArray<string>();
        if (context.Location.RunLocationSpecificEventCommand(@event, command_string, !@event._repeatingLocationSpecificCommand, array))
        {
          @event._repeatingLocationSpecificCommand = false;
          ++@event.CurrentCommand;
        }
        else
          @event._repeatingLocationSpecificCommand = true;
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void Unskippable(Event @event, string[] args, EventContext context)
    {
      @event.skippable = false;
      ++@event.CurrentCommand;
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void Skippable(Event @event, string[] args, EventContext context)
    {
      @event.skippable = true;
      ++@event.CurrentCommand;
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void SetSkipActions(Event @event, string[] args, EventContext context)
    {
      string str;
      string error;
      if (!ArgUtility.TryGetRemainder(args, 1, out str, out error, name: "string skipActions"))
      {
        context.LogErrorAndSkip(error);
      }
      else
      {
        if (string.IsNullOrWhiteSpace(str))
        {
          @event.actionsOnSkip = (string[]) null;
        }
        else
        {
          string[] strArray = LegacyShims.SplitAndTrim(str, '#');
          foreach (string action in strArray)
          {
            if (!TriggerActionManager.TryValidateActionExists(action, out error))
            {
              context.LogErrorAndSkip(error);
              return;
            }
          }
          @event.actionsOnSkip = strArray;
        }
        ++@event.CurrentCommand;
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void Emote(Event @event, string[] args, EventContext context)
    {
      string str;
      string error;
      int whichEmote;
      bool flag;
      if (!ArgUtility.TryGet(args, 1, out str, out error, name: "string actorName") || !ArgUtility.TryGetInt(args, 2, out whichEmote, out error, "int emoteId") || !ArgUtility.TryGetOptionalBool(args, 3, out flag, out error, name: "bool nextCommandImmediate"))
      {
        context.LogErrorAndSkip(error);
      }
      else
      {
        int farmerNumber;
        if (@event.IsFarmerActorId(str, out farmerNumber))
        {
          @event.GetFarmerActor(farmerNumber)?.doEmote(whichEmote, !flag);
        }
        else
        {
          bool isOptionalNpc;
          NPC actorByName = @event.getActorByName(str, out isOptionalNpc);
          if (actorByName == null)
          {
            context.LogErrorAndSkip($"no NPC found with name '{str}'", isOptionalNpc);
            return;
          }
          if (!actorByName.isEmoting)
            actorByName.doEmote(whichEmote, !flag);
        }
        if (!flag)
          return;
        ++@event.CurrentCommand;
        @event.Update(context.Location, context.Time);
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void StopMusic(Event @event, string[] args, EventContext context)
    {
      Game1.changeMusicTrack("none", music_context: MusicContext.Event);
      ++@event.CurrentCommand;
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void PlayPetSound(Event @event, string[] args, EventContext context)
    {
      string barkOverride;
      string error;
      if (!ArgUtility.TryGet(args, 1, out barkOverride, out error, name: "string sound"))
      {
        context.LogErrorAndSkip(error);
      }
      else
      {
        Pet pet = (Pet) null;
        foreach (NPC actor in @event.actors)
        {
          if (actor is Pet)
          {
            pet = actor as Pet;
            break;
          }
        }
        if (pet == null)
          pet = Game1.player.getPet();
        float num = 1200f;
        if (pet != null)
        {
          PetData petData = pet.GetPetData();
          PetBreed breedById = petData?.GetBreedById(pet.whichBreed.Value);
          if (breedById != null)
          {
            num *= breedById.VoicePitch;
            if (barkOverride == petData.BarkSound && breedById.BarkOverride != null)
              barkOverride = breedById.BarkOverride;
          }
        }
        Game1.playSound(barkOverride, new int?((int) num));
        ++@event.CurrentCommand;
        @event.Update(context.Location, context.Time);
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void PlaySound(Event @event, string[] args, EventContext context)
    {
      string cueName;
      string error;
      if (!ArgUtility.TryGet(args, 1, out cueName, out error, name: "string soundId"))
      {
        context.LogErrorAndSkip(error);
      }
      else
      {
        ICue cue;
        Game1.playSound(cueName, out cue);
        @event.TrackSound(cue);
        ++@event.CurrentCommand;
        @event.Update(context.Location, context.Time);
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void StopSound(Event @event, string[] args, EventContext context)
    {
      string cueId;
      string error;
      bool immediate;
      if (!ArgUtility.TryGet(args, 1, out cueId, out error, name: "string soundId") || !ArgUtility.TryGetOptionalBool(args, 2, out immediate, out error, true, "bool immediate"))
      {
        context.LogErrorAndSkip(error);
      }
      else
      {
        @event.StopTrackedSound(cueId, immediate);
        ++@event.CurrentCommand;
        @event.Update(context.Location, context.Time);
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void TossConcession(Event @event, string[] args, EventContext context)
    {
      string name;
      string error;
      string id;
      if (!ArgUtility.TryGet(args, 1, out name, out error, name: "string actorName") || !ArgUtility.TryGet(args, 2, out id, out error, name: "string concessionId"))
      {
        context.LogErrorAndSkip(error);
      }
      else
      {
        bool isOptionalNpc;
        NPC actorByName = @event.getActorByName(name, out isOptionalNpc);
        if (actorByName == null)
        {
          context.LogErrorAndSkip($"no NPC found with name '{name}'", isOptionalNpc);
        }
        else
        {
          MovieConcession concessionItem = MovieTheater.GetConcessionItem(id);
          if (concessionItem == null)
          {
            context.LogErrorAndSkip($"no concession found with ID '{id}'");
          }
          else
          {
            Texture2D texture = concessionItem.GetTexture();
            int spriteIndex = concessionItem.GetSpriteIndex();
            Game1.playSound("dwop");
            context.Location.temporarySprites.Add(new TemporaryAnimatedSprite()
            {
              texture = texture,
              sourceRect = Game1.getSourceRectForStandardTileSheet(texture, spriteIndex, 16 /*0x10*/, 16 /*0x10*/),
              animationLength = 1,
              totalNumberOfLoops = 1,
              motion = new Vector2(0.0f, -6f),
              acceleration = new Vector2(0.0f, 0.2f),
              interval = 1000f,
              scale = 4f,
              position = @event.OffsetPosition(new Vector2(actorByName.Position.X, actorByName.Position.Y - 96f)),
              layerDepth = (float) actorByName.StandingPixel.Y / 10000f
            });
            ++@event.CurrentCommand;
            @event.Update(context.Location, context.Time);
          }
        }
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void Pause(Event @event, string[] args, EventContext context)
    {
      int num;
      string error;
      if (!ArgUtility.TryGetInt(args, 1, out num, out error, "int pauseTime"))
      {
        context.LogErrorAndSkip(error);
      }
      else
      {
        if ((double) Game1.pauseTime > 0.0)
          return;
        Game1.pauseTime = (float) num;
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void PrecisePause(Event @event, string[] args, EventContext context)
    {
      int num;
      string error;
      if (!ArgUtility.TryGetInt(args, 1, out num, out error, "int pauseTime"))
      {
        context.LogErrorAndSkip(error);
      }
      else
      {
        if (@event.stopWatch == null)
          @event.stopWatch = new Stopwatch();
        if (!@event.stopWatch.IsRunning)
          @event.stopWatch.Start();
        if (@event.stopWatch.ElapsedMilliseconds < (long) num)
          return;
        @event.stopWatch.Stop();
        @event.stopWatch.Reset();
        ++@event.CurrentCommand;
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void ResetVariable(Event @event, string[] args, EventContext context)
    {
      @event.specialEventVariable1 = false;
      ++@event.currentCommand;
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void FaceDirection(Event @event, string[] args, EventContext context)
    {
      string str;
      string error;
      int direction;
      bool flag;
      if (!ArgUtility.TryGet(args, 1, out str, out error, name: "string actorName") || !ArgUtility.TryGetDirection(args, 2, out direction, out error, "int faceDirection") || !ArgUtility.TryGetOptionalBool(args, 3, out flag, out error, name: "bool continueImmediate"))
      {
        context.LogErrorAndSkip(error);
      }
      else
      {
        int farmerNumber;
        if (@event.IsFarmerActorId(str, out farmerNumber))
        {
          Farmer farmerActor = @event.GetFarmerActor(farmerNumber);
          if (farmerActor != null)
          {
            farmerActor.FarmerSprite.StopAnimation();
            farmerActor.completelyStopAnimatingOrDoingAction();
            farmerActor.faceDirection(direction);
          }
        }
        else if (str.Contains("spouse"))
        {
          NPC actorByName = @event.getActorByName(Game1.player.spouse);
          if (actorByName != null && !Game1.player.hasRoommate())
            actorByName.faceDirection(direction);
        }
        else
        {
          bool isOptionalNpc;
          NPC actorByName = @event.getActorByName(str, out isOptionalNpc);
          if (actorByName == null)
          {
            context.LogErrorAndSkip($"no NPC found with name '{str}'", isOptionalNpc);
            return;
          }
          actorByName.faceDirection(direction);
        }
        if (flag)
        {
          ++@event.CurrentCommand;
          @event.Update(context.Location, context.Time);
        }
        else
        {
          if ((double) Game1.pauseTime > 0.0)
            return;
          Game1.pauseTime = 500f;
        }
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void Warp(Event @event, string[] args, EventContext context)
    {
      string str;
      string error;
      Vector2 original;
      bool flag;
      if (!ArgUtility.TryGet(args, 1, out str, out error, name: "string actorName") || !ArgUtility.TryGetVector2(args, 2, out original, out error, true, "Vector2 tile") || !ArgUtility.TryGetOptionalBool(args, 4, out flag, out error, name: "bool continueImmediate"))
      {
        context.LogErrorAndSkip(error);
      }
      else
      {
        int farmerNumber;
        if (@event.IsFarmerActorId(str, out farmerNumber))
        {
          Farmer farmerActor = @event.GetFarmerActor(farmerNumber);
          if (farmerActor != null)
          {
            farmerActor.setTileLocation(@event.OffsetTile(original));
            farmerActor.position.Y -= 16f;
            if (@event.farmerActors.Contains(farmerActor))
              farmerActor.completelyStopAnimatingOrDoingAction();
          }
        }
        else if (str.Contains("spouse"))
        {
          NPC actorByName = @event.getActorByName(Game1.player.spouse);
          if (actorByName != null && !Game1.player.hasRoommate())
          {
            @event.npcControllers?.RemoveAll((Predicate<NPCController>) (npcController => npcController.puppet.Name == Game1.player.spouse));
            actorByName.Position = @event.OffsetPosition(original * 64f);
            actorByName.IsWalkingInSquare = false;
          }
        }
        else
        {
          bool isOptionalNpc;
          NPC actorByName = @event.getActorByName(str, out isOptionalNpc);
          if (actorByName == null)
          {
            context.LogErrorAndSkip($"no NPC found with name '{str}'", isOptionalNpc);
            return;
          }
          actorByName.position.X = @event.OffsetPositionX((float) ((double) original.X * 64.0 + 4.0));
          actorByName.position.Y = @event.OffsetPositionY(original.Y * 64f);
          actorByName.IsWalkingInSquare = false;
        }
        ++@event.CurrentCommand;
        if (!flag)
          return;
        @event.Update(context.Location, context.Time);
      }
    }

    /// <summary>Change the event position for all connected farmers.</summary>
    /// <remarks>This expects at least four fields:
    /// 1. zero or more [x y direction] triplets (one per possible farmer);
    /// 2. an offset direction (up/down/left/right), which sets where each subsequent farmer is set when using the default triplet;
    /// 3. and a default [x y direction] triplet which applies to any unlisted farmer.</remarks>
    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void WarpFarmers(Event @event, string[] args, EventContext context)
    {
      int num1 = (args.Length - 1) % 3;
      if (args.Length < 5 || num1 != 1)
      {
        context.LogErrorAndSkip("invalid number of arguments; expected zero or more [x y direction] triplets, one offset direction (up/down/left/right), and one triplet which applies to any other farmer");
      }
      else
      {
        int index1 = args.Length - 4;
        int num2;
        string error;
        Point point1;
        int num3;
        if (!ArgUtility.TryGetDirection(args, index1, out num2, out error, "int offsetDirection") || !ArgUtility.TryGetPoint(args, index1 + 1, out point1, out error, "Point defaultPosition") || !ArgUtility.TryGetDirection(args, index1 + 3, out num3, out error, "int defaultFacingDirection"))
        {
          context.LogErrorAndSkip(error);
        }
        else
        {
          List<Vector3> vector3List = new List<Vector3>();
          for (int index2 = 1; index2 < index1; index2 += 3)
          {
            Point point2;
            int z;
            if (!ArgUtility.TryGetPoint(args, index2, out point2, out error, "Point position") || !ArgUtility.TryGetDirection(args, index2 + 2, out z, out error, "int facingDirection"))
            {
              context.LogErrorAndSkip(error);
              return;
            }
            vector3List.Add(new Vector3((float) point2.X, (float) point2.Y, (float) z));
          }
          Point point3;
          switch (num2)
          {
            case 0:
              point3 = new Point(0, -1);
              break;
            case 1:
              point3 = new Point(1, 0);
              break;
            case 2:
              point3 = new Point(0, 1);
              break;
            case 3:
              point3 = new Point(-1, 0);
              break;
            default:
              context.LogErrorAndSkip($"invalid offset direction '{num2}'; must be one of 'left', 'right', 'up', or 'down'");
              return;
          }
          int x1 = point1.X;
          int y1 = point1.Y;
          for (int index3 = 0; index3 < Game1.numberOfPlayers(); ++index3)
          {
            Farmer farmerActor = @event.GetFarmerActor(index3 + 1);
            float x2;
            float y2;
            int direction;
            if (index3 < vector3List.Count)
            {
              x2 = vector3List[index3].X;
              y2 = vector3List[index3].Y;
              direction = (int) vector3List[index3].Z;
            }
            else
            {
              x2 = (float) x1;
              y2 = (float) y1;
              direction = num3;
              x1 += point3.X;
              y1 += point3.Y;
              if (context.Location.map.GetLayer("Buildings")?.Tiles[x1, y1] != null && point3 != Point.Zero)
              {
                x1 -= point3.X;
                y1 -= point3.Y;
                point3 = Point.Zero;
              }
            }
            if (farmerActor != null)
            {
              farmerActor.setTileLocation(@event.OffsetTile(new Vector2(x2, y2)));
              farmerActor.faceDirection(direction);
              farmerActor.position.Y -= 16f;
              farmerActor.completelyStopAnimatingOrDoingAction();
            }
          }
          ++@event.CurrentCommand;
        }
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void Speed(Event @event, string[] args, EventContext context)
    {
      string str;
      string error;
      int num;
      if (!ArgUtility.TryGet(args, 1, out str, out error, name: "string actorName") || !ArgUtility.TryGetInt(args, 2, out num, out error, "int speed"))
      {
        context.LogErrorAndSkip(error);
      }
      else
      {
        int farmerNumber;
        if (@event.IsFarmerActorId(str, out farmerNumber))
        {
          if (@event.IsCurrentFarmerActorId(farmerNumber))
            @event.farmerAddedSpeed = num;
        }
        else
        {
          bool isOptionalNpc;
          NPC actorByName = @event.getActorByName(str, out isOptionalNpc);
          if (actorByName == null)
          {
            context.LogErrorAndSkip($"no NPC found with name '{str}'", isOptionalNpc);
            return;
          }
          actorByName.speed = num;
        }
        ++@event.CurrentCommand;
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void StopAdvancedMoves(Event @event, string[] args, EventContext context)
    {
      string str = ArgUtility.Get(args, 1);
      switch (str)
      {
        case null:
          @event.npcControllers.Clear();
          break;
        case "next":
          using (List<NPCController>.Enumerator enumerator = @event.npcControllers.GetEnumerator())
          {
            while (enumerator.MoveNext())
              enumerator.Current.destroyAtNextCrossroad();
            break;
          }
        default:
          context.LogErrorAndSkip($"unknown option {str}, must be 'next' or omitted");
          return;
      }
      ++@event.CurrentCommand;
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void DoAction(Event @event, string[] args, EventContext context)
    {
      Point point;
      string error;
      if (!ArgUtility.TryGetPoint(args, 1, out point, out error, "Point tile"))
      {
        context.LogErrorAndSkip(error);
      }
      else
      {
        Location tileLocation = new Location(@event.OffsetTileX(point.X), @event.OffsetTileY(point.Y));
        Game1.hooks.OnGameLocation_CheckAction(context.Location, tileLocation, Game1.viewport, @event.farmer, (Func<bool>) (() => context.Location.checkAction(tileLocation, Game1.viewport, @event.farmer)));
        ++@event.CurrentCommand;
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void RemoveTile(Event @event, string[] args, EventContext context)
    {
      Point point;
      string error;
      string layer;
      if (!ArgUtility.TryGetPoint(args, 1, out point, out error, "Point tile") || !ArgUtility.TryGet(args, 3, out layer, out error, name: "string layerId"))
      {
        context.LogErrorAndSkip(error);
      }
      else
      {
        context.Location.removeTile(@event.OffsetTileX(point.X), @event.OffsetTileY(point.Y), layer);
        ++@event.CurrentCommand;
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void TextAboveHead(Event @event, string[] args, EventContext context)
    {
      string name;
      string error;
      string text;
      if (!ArgUtility.TryGet(args, 1, out name, out error, name: "string actorName") || !ArgUtility.TryGet(args, 2, out text, out error, name: "string text"))
      {
        context.LogErrorAndSkip(error);
      }
      else
      {
        bool isOptionalNpc;
        NPC actorByName = @event.getActorByName(name, out isOptionalNpc);
        if (actorByName == null)
        {
          context.LogErrorAndSkip($"no NPC found with name '{name}'", isOptionalNpc);
        }
        else
        {
          actorByName.showTextAboveHead(text);
          ++@event.CurrentCommand;
        }
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void ShowFrame(Event @event, string[] args, EventContext context)
    {
      bool flip = false;
      string str;
      int frame;
      if (args.Length == 2)
      {
        str = "farmer";
        string error;
        if (!ArgUtility.TryGetInt(args, 1, out frame, out error, "frame"))
        {
          context.LogErrorAndSkip(error);
          return;
        }
      }
      else
      {
        string error;
        if (!ArgUtility.TryGet(args, 1, out str, out error, name: "actorName") || !ArgUtility.TryGetInt(args, 2, out frame, out error, "frame") || !ArgUtility.TryGetOptionalBool(args, 3, out flip, out error, name: "flip"))
        {
          context.LogErrorAndSkip(error);
          return;
        }
      }
      int farmerNumber;
      if (!@event.IsFarmerActorId(str, out farmerNumber))
      {
        bool isOptionalNpc;
        NPC actorByName = @event.getActorByName(str, out isOptionalNpc);
        if (actorByName == null)
        {
          context.LogErrorAndSkip($"no NPC found with name '{str}'", isOptionalNpc);
          return;
        }
        if (str == "spouse" && actorByName.Gender == Gender.Male && frame >= 36 && frame <= 38)
          frame += 12;
        actorByName.Sprite.CurrentFrame = frame;
      }
      else
      {
        Farmer farmerActor = @event.GetFarmerActor(farmerNumber);
        if (farmerActor != null)
        {
          farmerActor.FarmerSprite.setCurrentAnimation(new FarmerSprite.AnimationFrame[1]
          {
            new FarmerSprite.AnimationFrame(frame, 100, false, flip)
          });
          farmerActor.FarmerSprite.loop = true;
          farmerActor.FarmerSprite.loopThisAnimation = true;
          farmerActor.FarmerSprite.PauseForSingleAnimation = true;
          farmerActor.Sprite.currentFrame = frame;
        }
      }
      ++@event.CurrentCommand;
      @event.Update(context.Location, context.Time);
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void FarmerAnimation(Event @event, string[] args, EventContext context)
    {
      int which;
      string error;
      if (!ArgUtility.TryGetInt(args, 1, out which, out error, "int animationId"))
      {
        context.LogErrorAndSkip(error);
      }
      else
      {
        @event.farmer.FarmerSprite.setCurrentSingleAnimation(which);
        @event.farmer.FarmerSprite.PauseForSingleAnimation = true;
        ++@event.CurrentCommand;
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void IgnoreMovementAnimation(Event @event, string[] args, EventContext context)
    {
      string str;
      string error;
      bool flag;
      if (!ArgUtility.TryGet(args, 1, out str, out error, name: "string actorName") || !ArgUtility.TryGetOptionalBool(args, 2, out flag, out error, true, "bool ignore"))
      {
        context.LogErrorAndSkip(error);
      }
      else
      {
        int farmerNumber;
        if (@event.IsFarmerActorId(str, out farmerNumber))
        {
          Farmer farmerActor = @event.GetFarmerActor(farmerNumber);
          if (farmerActor != null)
            farmerActor.ignoreMovementAnimation = flag;
        }
        else
        {
          bool isOptionalNpc;
          NPC actorByName = @event.getActorByName(str, out isOptionalNpc, true);
          if (actorByName == null)
          {
            context.LogErrorAndSkip($"no NPC found with name '{str}'", isOptionalNpc);
            return;
          }
          actorByName.ignoreMovementAnimation = flag;
        }
        ++@event.CurrentCommand;
        @event.Update(context.Location, context.Time);
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void Animate(Event @event, string[] args, EventContext context)
    {
      string str;
      string error;
      bool flip;
      bool flag;
      int milliseconds;
      if (!ArgUtility.TryGet(args, 1, out str, out error, name: "string actorName") || !ArgUtility.TryGetBool(args, 2, out flip, out error, "bool flip") || !ArgUtility.TryGetBool(args, 3, out flag, out error, "bool loop") || !ArgUtility.TryGetInt(args, 4, out milliseconds, out error, "int frameDuration"))
      {
        context.LogErrorAndSkip(error);
      }
      else
      {
        List<FarmerSprite.AnimationFrame> animation = new List<FarmerSprite.AnimationFrame>();
        for (int index = 5; index < args.Length; ++index)
        {
          int frame;
          if (!ArgUtility.TryGetInt(args, index, out frame, out error, "int frame"))
          {
            context.LogErrorAndSkip(error);
            return;
          }
          animation.Add(new FarmerSprite.AnimationFrame(frame, milliseconds, false, flip));
        }
        int farmerNumber;
        if (@event.IsFarmerActorId(str, out farmerNumber))
        {
          Farmer farmerActor = @event.GetFarmerActor(farmerNumber);
          if (farmerActor != null)
          {
            farmerActor.FarmerSprite.setCurrentAnimation(animation.ToArray());
            farmerActor.FarmerSprite.loop = true;
            farmerActor.FarmerSprite.loopThisAnimation = flag;
            farmerActor.FarmerSprite.PauseForSingleAnimation = true;
          }
        }
        else
        {
          bool isOptionalNpc;
          NPC actorByName = @event.getActorByName(str, out isOptionalNpc, true);
          if (actorByName == null)
          {
            context.LogErrorAndSkip($"no NPC found with name '{str}'", isOptionalNpc);
            return;
          }
          actorByName.Sprite.setCurrentAnimation(animation);
          actorByName.Sprite.loop = flag;
        }
        ++@event.CurrentCommand;
        @event.Update(context.Location, context.Time);
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void StopAnimation(Event @event, string[] args, EventContext context)
    {
      string str;
      string error;
      int num;
      if (!ArgUtility.TryGet(args, 1, out str, out error, name: "string actorName") || !ArgUtility.TryGetOptionalInt(args, 2, out num, out error, -1, "int endFrame"))
      {
        context.LogErrorAndSkip(error);
      }
      else
      {
        int farmerNumber;
        if (@event.IsFarmerActorId(str, out farmerNumber))
        {
          Farmer farmerActor = @event.GetFarmerActor(farmerNumber);
          if (farmerActor != null)
          {
            farmerActor.completelyStopAnimatingOrDoingAction();
            farmerActor.Halt();
            farmerActor.FarmerSprite.CurrentAnimation = (List<FarmerSprite.AnimationFrame>) null;
            switch (farmerActor.FacingDirection)
            {
              case 0:
                farmerActor.FarmerSprite.setCurrentSingleFrame(12);
                break;
              case 1:
                farmerActor.FarmerSprite.setCurrentSingleFrame(6);
                break;
              case 2:
                farmerActor.FarmerSprite.setCurrentSingleFrame(0);
                break;
              case 3:
                farmerActor.FarmerSprite.setCurrentSingleFrame(6, flip: true);
                break;
            }
          }
        }
        else
        {
          bool isOptionalNpc;
          NPC actorByName = @event.getActorByName(str, out isOptionalNpc);
          if (actorByName == null)
          {
            context.LogErrorAndSkip($"no NPC found with name '{str}'", isOptionalNpc);
            return;
          }
          actorByName.Sprite.StopAnimation();
          if (num > -1)
          {
            actorByName.Sprite.currentFrame = num;
            actorByName.Sprite.UpdateSourceRect();
          }
        }
        ++@event.CurrentCommand;
        @event.Update(context.Location, context.Time);
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void ChangeLocation(Event @event, string[] args, EventContext context)
    {
      string locationName;
      string error;
      if (!ArgUtility.TryGet(args, 1, out locationName, out error, name: "string locationName"))
      {
        context.LogErrorAndSkip(error);
      }
      else
      {
        Point tilePoint = @event.farmer.TilePoint;
        @event.changeLocation(locationName, tilePoint.X, tilePoint.Y, (Action) (() =>
        {
          Game1.currentLocation.ResetForEvent(@event);
          ++@event.CurrentCommand;
        }));
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void Halt(Event @event, string[] args, EventContext context)
    {
      foreach (Character actor in @event.actors)
        actor.Halt();
      @event.farmer.Halt();
      ++@event.CurrentCommand;
      @event.continueAfterMove = false;
      @event.actorPositionsAfterMove.Clear();
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void Message(Event @event, string[] args, EventContext context)
    {
      string text;
      string error;
      if (!ArgUtility.TryGet(args, 1, out text, out error, name: "string dialogue"))
        context.LogError(error);
      if (Game1.dialogueUp || Game1.activeClickableMenu != null)
        return;
      Game1.drawDialogueNoTyping(Game1.parseText(text));
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void AddCookingRecipe(Event @event, string[] args, EventContext context)
    {
      string key;
      string error;
      if (!ArgUtility.TryGetRemainder(args, 1, out key, out error, name: "string recipeKey"))
      {
        context.LogErrorAndSkip(error);
      }
      else
      {
        Game1.player.cookingRecipes.TryAdd(key, 0);
        ++@event.CurrentCommand;
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void ItemAboveHead(Event @event, string[] args, EventContext context)
    {
      string itemId = ArgUtility.Get(args, 1);
      bool showMessage1 = ArgUtility.GetBool(args, 2, true);
      string lower = itemId?.ToLower();
      if (lower != null)
      {
        switch (lower.Length)
        {
          case 3:
            switch (lower[2])
            {
              case 'd':
                if (lower == "rod")
                {
                  @event.farmer.holdUpItemThenMessage(ItemRegistry.Create("(T)BambooPole"), showMessage1);
                  goto label_29;
                }
                break;
              case 'e':
                if (lower == "ore")
                {
                  @event.farmer.holdUpItemThenMessage(ItemRegistry.Create("(O)334"), showMessage1);
                  goto label_29;
                }
                break;
              case 'n':
                if (lower == "pan")
                {
                  @event.farmer.holdUpItemThenMessage(ItemRegistry.Create("(T)Pan"), showMessage1);
                  goto label_29;
                }
                break;
              case 't':
                if (lower == "pot")
                {
                  bool showMessage2 = ArgUtility.GetBool(args, 2);
                  @event.farmer.holdUpItemThenMessage(ItemRegistry.Create("(BC)62"), showMessage2);
                  goto label_29;
                }
                break;
            }
            break;
          case 4:
            switch (lower[0])
            {
              case 'h':
                if (lower == "hero")
                {
                  @event.farmer.holdUpItemThenMessage(ItemRegistry.Create("(BC)116"), showMessage1);
                  goto label_29;
                }
                break;
              case 'j':
                if (lower == "joja")
                {
                  @event.farmer.holdUpItemThenMessage(ItemRegistry.Create("(BC)117"), showMessage1);
                  goto label_29;
                }
                break;
            }
            break;
          case 5:
            if (lower == "sword")
            {
              @event.farmer.holdUpItemThenMessage(ItemRegistry.Create("(W)0"), showMessage1);
              goto label_29;
            }
            break;
          case 7:
            if (lower == "jukebox")
            {
              bool showMessage3 = ArgUtility.GetBool(args, 2);
              @event.farmer.holdUpItemThenMessage(ItemRegistry.Create("(BC)209"), showMessage3);
              goto label_29;
            }
            break;
          case 8:
            if (lower == "slimeegg")
            {
              @event.farmer.holdUpItemThenMessage(ItemRegistry.Create("(O)680"), showMessage1);
              goto label_29;
            }
            break;
          case 9:
            if (lower == "sculpture")
            {
              @event.farmer.holdUpItemThenMessage(ItemRegistry.Create("(F)1306"), showMessage1);
              goto label_29;
            }
            break;
          case 10:
            if (lower == "samboombox")
            {
              @event.farmer.holdUpItemThenMessage(ItemRegistry.Create("(F)1309"), showMessage1);
              goto label_29;
            }
            break;
        }
      }
      if (lower == null)
        @event.farmer.holdUpItemThenMessage((Item) null, false);
      else
        @event.farmer.holdUpItemThenMessage(ItemRegistry.Create(itemId), showMessage1);
label_29:
      ++@event.CurrentCommand;
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void AddCraftingRecipe(Event @event, string[] args, EventContext context)
    {
      string key;
      string error;
      if (!ArgUtility.TryGetRemainder(args, 1, out key, out error, name: "string recipeKey"))
      {
        context.LogErrorAndSkip(error);
      }
      else
      {
        Game1.player.craftingRecipes.TryAdd(key, 0);
        ++@event.CurrentCommand;
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void HostMail(Event @event, string[] args, EventContext context)
    {
      string str;
      string error;
      if (!ArgUtility.TryGet(args, 1, out str, out error, name: "string mailId"))
      {
        context.LogErrorAndSkip(error);
      }
      else
      {
        if (Game1.IsMasterGame && !Game1.player.hasOrWillReceiveMail(str))
          Game1.addMailForTomorrow(str);
        ++@event.CurrentCommand;
      }
    }

    /// <summary>Add a letter to the mailbox tomorrow.</summary>
    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void Mail(Event @event, string[] args, EventContext context)
    {
      string str;
      string error;
      if (!ArgUtility.TryGet(args, 1, out str, out error, name: "string mailId"))
      {
        context.LogErrorAndSkip(error);
      }
      else
      {
        if (!Game1.player.hasOrWillReceiveMail(str))
          Game1.addMailForTomorrow(str);
        ++@event.CurrentCommand;
      }
    }

    /// <summary>Add a letter to the mailbox immediately.</summary>
    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void MailToday(Event @event, string[] args, EventContext context)
    {
      string str;
      string error;
      if (!ArgUtility.TryGet(args, 1, out str, out error, name: "string mailId"))
      {
        context.LogErrorAndSkip(error);
      }
      else
      {
        if (!Game1.player.hasOrWillReceiveMail(str))
          Game1.addMail(str);
        ++@event.CurrentCommand;
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void Shake(Event @event, string[] args, EventContext context)
    {
      string name;
      string error;
      int duration;
      if (!ArgUtility.TryGet(args, 1, out name, out error, name: "string actorName") || !ArgUtility.TryGetInt(args, 2, out duration, out error, "int duration"))
      {
        context.LogErrorAndSkip(error);
      }
      else
      {
        bool isOptionalNpc;
        NPC actorByName = @event.getActorByName(name, out isOptionalNpc);
        if (actorByName == null)
        {
          context.LogErrorAndSkip($"no NPC found with name '{name}'", isOptionalNpc);
        }
        else
        {
          actorByName.shake(duration);
          ++@event.CurrentCommand;
        }
      }
    }

    /// <remarks>Main format: <c>temporaryAnimatedSprite texture rect_x rect_y rect_width rect_height animation_interval animation_length number_of_loops tile_x tile_y flicker flipped layer_depth alpha_fade scale scale_change rotation rotation_change</c>. This also supports a number of extended options (like <c>color Green</c>).</remarks>
    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void TemporaryAnimatedSprite(Event @event, string[] args, EventContext context)
    {
      string textureName;
      string error;
      Microsoft.Xna.Framework.Rectangle sourceRect;
      float animationInterval;
      int animationLength;
      int numberOfLoops;
      Vector2 vector2_1;
      bool flicker;
      bool flipped;
      float y;
      float alphaFade;
      int num;
      float scaleChange;
      float rotation;
      float rotationChange;
      if (!ArgUtility.TryGet(args, 1, out textureName, out error, name: "string textureName") || !ArgUtility.TryGetRectangle(args, 2, out sourceRect, out error, "Rectangle sourceRect") || !ArgUtility.TryGetFloat(args, 6, out animationInterval, out error, "float animationInterval") || !ArgUtility.TryGetInt(args, 7, out animationLength, out error, "int animationLength") || !ArgUtility.TryGetInt(args, 8, out numberOfLoops, out error, "int numberOfLoops") || !ArgUtility.TryGetVector2(args, 9, out vector2_1, out error, true, "Vector2 tile") || !ArgUtility.TryGetBool(args, 11, out flicker, out error, "bool flicker") || !ArgUtility.TryGetBool(args, 12, out flipped, out error, "bool flip") || !ArgUtility.TryGetFloat(args, 13, out y, out error, "float layerDepth") || !ArgUtility.TryGetFloat(args, 14, out alphaFade, out error, "float alphaFade") || !ArgUtility.TryGetInt(args, 15, out num, out error, "int scale") || !ArgUtility.TryGetFloat(args, 16 /*0x10*/, out scaleChange, out error, "float scaleChange") || !ArgUtility.TryGetFloat(args, 17, out rotation, out error, "float rotation") || !ArgUtility.TryGetFloat(args, 18, out rotationChange, out error, "float rotationChange"))
      {
        context.LogErrorAndSkip(error);
      }
      else
      {
        TemporaryAnimatedSprite temporaryAnimatedSprite = new TemporaryAnimatedSprite(textureName, sourceRect, animationInterval, animationLength, numberOfLoops, @event.OffsetPosition(vector2_1 * 64f), flicker, flipped, @event.OffsetPosition(new Vector2(0.0f, y) * 64f).Y / 10000f, alphaFade, Color.White, (float) (4 * num), scaleChange, rotation, rotationChange);
        for (int index = 19; index < args.Length; ++index)
        {
          switch (args[index])
          {
            case "color":
              string rawColor;
              if (!ArgUtility.TryGet(args, index + 1, out rawColor, out error, name: "string rawColor"))
              {
                context.LogError(error);
                break;
              }
              Color? color = Utility.StringToColor(rawColor);
              if (color.HasValue)
                temporaryAnimatedSprite.color = color.Value;
              else
                context.LogError($"index {index + 1} has value '{rawColor}', which can't be parsed as a color");
              ++index;
              break;
            case "hold_last_frame":
              temporaryAnimatedSprite.holdLastFrame = true;
              break;
            case "ping_pong":
              temporaryAnimatedSprite.pingPong = true;
              break;
            case "motion":
              Vector2 vector2_2;
              if (!ArgUtility.TryGetVector2(args, index + 1, out vector2_2, out error, name: "Vector2 value"))
              {
                context.LogError(error);
                break;
              }
              temporaryAnimatedSprite.motion = vector2_2;
              index += 2;
              break;
            case "acceleration":
              Vector2 vector2_3;
              if (!ArgUtility.TryGetVector2(args, index + 1, out vector2_3, out error, name: "Vector2 value"))
              {
                context.LogError(error);
                break;
              }
              temporaryAnimatedSprite.acceleration = vector2_3;
              index += 2;
              break;
            case "acceleration_change":
              Vector2 vector2_4;
              if (!ArgUtility.TryGetVector2(args, index + 1, out vector2_4, out error, name: "Vector2 value"))
              {
                context.LogError(error);
                break;
              }
              temporaryAnimatedSprite.accelerationChange = vector2_4;
              index += 2;
              break;
            default:
              context.LogError($"unknown option '{args[index]}'");
              break;
          }
        }
        context.Location.TemporarySprites.Add(temporaryAnimatedSprite);
        ++@event.CurrentCommand;
      }
    }

    /// <remarks>Format: <c>temporarySprite xTile yTile rowInAnimationSheet animationLength animationInterval=300 flipped=false layerDepth=-1</c>.</remarks>
    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void TemporarySprite(Event @event, string[] args, EventContext context)
    {
      Vector2 vector2;
      string error;
      int rowInAnimationTexture;
      int animationLength;
      float animationInterval;
      bool flipped;
      float layerDepth;
      if (!ArgUtility.TryGetVector2(args, 1, out vector2, out error, true, "Vector2 tile") || !ArgUtility.TryGetInt(args, 3, out rowInAnimationTexture, out error, "int rowInAnimationSheet") || !ArgUtility.TryGetInt(args, 4, out animationLength, out error, "int animationLength") || !ArgUtility.TryGetOptionalFloat(args, 5, out animationInterval, out error, 300f, "float animationInterval") || !ArgUtility.TryGetOptionalBool(args, 6, out flipped, out error, name: "bool flipped") || !ArgUtility.TryGetOptionalFloat(args, 7, out layerDepth, out error, -1f, "float layerDepth"))
      {
        context.LogErrorAndSkip(error);
      }
      else
      {
        context.Location.TemporarySprites.Add(new TemporaryAnimatedSprite(rowInAnimationTexture, @event.OffsetPosition(vector2 * 64f), Color.White, animationLength, flipped, animationInterval, sourceRectWidth: 64 /*0x40*/, layerDepth: layerDepth));
        ++@event.CurrentCommand;
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void RemoveTemporarySprites(Event @event, string[] args, EventContext context)
    {
      context.Location.TemporarySprites.Clear();
      ++@event.CurrentCommand;
    }

    /// <summary>A command that does nothing. Used just to wait for another event to finish.</summary>
    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void Null(Event @event, string[] args, EventContext context)
    {
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void SpecificTemporarySprite(Event @event, string[] args, EventContext context)
    {
      string key;
      string error;
      if (!ArgUtility.TryGet(args, 1, out key, out error, name: "string spriteId"))
      {
        context.LogErrorAndSkip(error);
      }
      else
      {
        @event.addSpecificTemporarySprite(key, context.Location, args);
        ++@event.CurrentCommand;
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void PlayMusic(Event @event, string[] args, EventContext context)
    {
      string newTrackName;
      string error;
      if (!ArgUtility.TryGetRemainder(args, 1, out newTrackName, out error, name: "string musicId"))
      {
        context.LogErrorAndSkip(error);
      }
      else
      {
        if (newTrackName == "samBand")
        {
          if (Game1.player.DialogueQuestionsAnswered.Contains("78"))
            Game1.changeMusicTrack("shimmeringbastion", music_context: MusicContext.Event);
          else if (Game1.player.DialogueQuestionsAnswered.Contains("79"))
            Game1.changeMusicTrack("honkytonky", music_context: MusicContext.Event);
          else if (Game1.player.DialogueQuestionsAnswered.Contains("77"))
            Game1.changeMusicTrack("heavy", music_context: MusicContext.Event);
          else
            Game1.changeMusicTrack("poppy", music_context: MusicContext.Event);
        }
        else if ((double) Game1.options.musicVolumeLevel > 0.0)
          Game1.changeMusicTrack(newTrackName, music_context: MusicContext.Event);
        ++@event.CurrentCommand;
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void MakeInvisible(Event @event, string[] args, EventContext context)
    {
      Point point;
      string error;
      int num1;
      int num2;
      if (!ArgUtility.TryGetPoint(args, 1, out point, out error, "Point tile") || !ArgUtility.TryGetOptionalInt(args, 3, out num1, out error, 1, "int width") || !ArgUtility.TryGetOptionalInt(args, 4, out num2, out error, 1, "int height"))
      {
        context.LogErrorAndSkip(error);
      }
      else
      {
        GameLocation location = context.Location;
        int num3 = @event.OffsetTileX(point.X);
        int num4 = @event.OffsetTileY(point.Y);
        for (int y = num4; y < num4 + num2; ++y)
        {
          for (int x = num3; x < num3 + num1; ++x)
          {
            Object objectAtTile = location.getObjectAtTile(x, y);
            if (objectAtTile != null)
            {
              if (objectAtTile is BedFurniture bedFurniture && bedFurniture.GetBoundingBox().Contains(Utility.Vector2ToPoint(Game1.player.mostRecentBed)))
              {
                ++@event.CurrentCommand;
                return;
              }
              objectAtTile.isTemporarilyInvisible = true;
            }
            else
            {
              TerrainFeature terrainFeature;
              if (location.terrainFeatures.TryGetValue(new Vector2((float) x, (float) y), out terrainFeature))
                terrainFeature.isTemporarilyInvisible = true;
            }
          }
        }
        ++@event.CurrentCommand;
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void AddObject(Event @event, string[] args, EventContext context)
    {
      Point point;
      string error;
      string itemId;
      float num;
      if (!ArgUtility.TryGetPoint(args, 1, out point, out error, "Point tile") || !ArgUtility.TryGet(args, 3, out itemId, out error, name: "string itemId") || !ArgUtility.TryGetOptionalFloat(args, 4, out num, out error, -1f, "float layerDepth"))
      {
        context.LogErrorAndSkip(error);
      }
      else
      {
        TemporaryAnimatedSprite temporaryAnimatedSprite = new TemporaryAnimatedSprite((string) null, Microsoft.Xna.Framework.Rectangle.Empty, @event.OffsetPosition(new Vector2((float) (point.X * 64 /*0x40*/), (float) (point.Y * 64 /*0x40*/))), false, 0.0f, Color.White)
        {
          layerDepth = (double) num >= 0.0 ? num : (float) (@event.OffsetTileY(point.Y) * 64 /*0x40*/) / 10000f
        };
        temporaryAnimatedSprite.CopyAppearanceFromItemId(itemId);
        context.Location.TemporarySprites.Add(temporaryAnimatedSprite);
        ++@event.CurrentCommand;
        @event.Update(context.Location, context.Time);
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void AddBigProp(Event @event, string[] args, EventContext context)
    {
      Vector2 original;
      string error;
      string str;
      if (!ArgUtility.TryGetVector2(args, 1, out original, out error, true, "Vector2 tile") || !ArgUtility.TryGet(args, 3, out str, out error, name: "string itemId"))
      {
        context.LogErrorAndSkip(error);
      }
      else
      {
        Object @object = ItemRegistry.Create<Object>("(BC)" + str);
        @object.TileLocation = @event.OffsetTile(original);
        @event.props.Add(@object);
        ++@event.CurrentCommand;
        @event.Update(context.Location, context.Time);
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void AddFloorProp(Event @event, string[] args, EventContext context)
    {
      Event.DefaultCommands.AddProp(@event, args, context);
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void AddProp(Event @event, string[] args, EventContext context)
    {
      string str;
      string error;
      int index;
      Point point;
      int tilesWideSolid;
      int num1;
      int tilesHighSolid;
      int num2;
      int num3;
      if (!ArgUtility.TryGet(args, 0, out str, out error, name: "string commandName") || !ArgUtility.TryGetInt(args, 1, out index, out error, "int index") || !ArgUtility.TryGetPoint(args, 2, out point, out error, "Point tile") || !ArgUtility.TryGetOptionalInt(args, 4, out tilesWideSolid, out error, 1, "int drawWidth") || !ArgUtility.TryGetOptionalInt(args, 5, out num1, out error, 1, "int drawHeight") || !ArgUtility.TryGetOptionalInt(args, 6, out tilesHighSolid, out error, num1, "int boundingHeight") || !ArgUtility.TryGetOptionalInt(args, 7, out num2, out error, name: "int tilesHorizontal") || !ArgUtility.TryGetOptionalInt(args, 8, out num3, out error, name: "int tilesVertical"))
      {
        context.LogErrorAndSkip(error);
      }
      else
      {
        int tileX1 = @event.OffsetTileX(point.X);
        int tileY1 = @event.OffsetTileY(point.Y);
        bool solid = !str.EqualsIgnoreCase("AddFloorProp");
        @event.festivalProps.Add(new Prop(@event.festivalTexture, index, tilesWideSolid, tilesHighSolid, num1, tileX1, tileY1, solid));
        if (num2 != 0)
        {
          for (int tileX2 = tileX1 + num2; tileX2 != tileX1; tileX2 -= Math.Sign(num2))
            @event.festivalProps.Add(new Prop(@event.festivalTexture, index, tilesWideSolid, tilesHighSolid, num1, tileX2, tileY1, solid));
        }
        if (num3 != 0)
        {
          for (int tileY2 = tileY1 + num3; tileY2 != tileY1; tileY2 -= Math.Sign(num3))
            @event.festivalProps.Add(new Prop(@event.festivalTexture, index, tilesWideSolid, tilesHighSolid, num1, tileX1, tileY2, solid));
        }
        ++@event.CurrentCommand;
        @event.Update(context.Location, context.Time);
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void RemoveObject(Event @event, string[] args, EventContext context)
    {
      Point point;
      string error;
      if (!ArgUtility.TryGetPoint(args, 1, out point, out error, "Point tile"))
      {
        context.LogErrorAndSkip(error);
      }
      else
      {
        GameLocation location = context.Location;
        Vector2 position = @event.OffsetPosition(new Vector2((float) point.X, (float) point.Y) * 64f);
        location.temporarySprites.RemoveWhere<TemporaryAnimatedSprite>((Predicate<TemporaryAnimatedSprite>) (sprite => sprite.position == position));
        ++@event.CurrentCommand;
        @event.Update(location, context.Time);
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void Glow(Event @event, string[] args, EventContext context)
    {
      int r;
      string error;
      int g;
      int b;
      bool hold;
      if (!ArgUtility.TryGetInt(args, 1, out r, out error, "int red") || !ArgUtility.TryGetInt(args, 2, out g, out error, "int green") || !ArgUtility.TryGetInt(args, 3, out b, out error, "int blue") || !ArgUtility.TryGetOptionalBool(args, 4, out hold, out error, name: "bool hold"))
      {
        context.LogErrorAndSkip(error);
      }
      else
      {
        Game1.screenGlowOnce(new Color(r, g, b), hold);
        ++@event.CurrentCommand;
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void StopGlowing(Event @event, string[] args, EventContext context)
    {
      Game1.screenGlowUp = false;
      Game1.screenGlowHold = false;
      ++@event.CurrentCommand;
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void AddQuest(Event @event, string[] args, EventContext context)
    {
      string questId;
      string error;
      if (!ArgUtility.TryGet(args, 1, out questId, out error, name: "string questId"))
      {
        context.LogErrorAndSkip(error);
      }
      else
      {
        Game1.player.addQuest(questId);
        ++@event.CurrentCommand;
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void RemoveQuest(Event @event, string[] args, EventContext context)
    {
      string questID;
      string error;
      if (!ArgUtility.TryGet(args, 1, out questID, out error, name: "string questId"))
      {
        context.LogErrorAndSkip(error);
      }
      else
      {
        Game1.player.removeQuest(questID);
        ++@event.CurrentCommand;
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void AddSpecialOrder(Event @event, string[] args, EventContext context)
    {
      string id;
      string error;
      if (!ArgUtility.TryGet(args, 1, out id, out error, name: "string orderId"))
      {
        context.LogErrorAndSkip(error);
      }
      else
      {
        Game1.player.team.AddSpecialOrder(id);
        ++@event.CurrentCommand;
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void RemoveSpecialOrder(Event @event, string[] args, EventContext context)
    {
      string error;
      string orderId;
      if (!ArgUtility.TryGet(args, 1, out orderId, out error, name: "string orderId"))
      {
        context.LogErrorAndSkip(error);
      }
      else
      {
        Game1.player.team.specialOrders.RemoveWhere((Func<SpecialOrder, bool>) (order => order.questKey.Value == orderId));
        ++@event.CurrentCommand;
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void AddItem(Event @event, string[] args, EventContext context)
    {
      string itemId;
      string error;
      int amount;
      int quality;
      if (!ArgUtility.TryGet(args, 1, out itemId, out error, name: "string itemId") || !ArgUtility.TryGetOptionalInt(args, 2, out amount, out error, 1, "int count") || !ArgUtility.TryGetOptionalInt(args, 3, out quality, out error, name: "int quality"))
      {
        context.LogErrorAndSkip(error);
      }
      else
      {
        Item obj = ItemRegistry.Create(itemId, amount, quality);
        if (obj != null)
          Game1.player.addItemByMenuIfNecessary(obj);
        ++@event.CurrentCommand;
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void AwardFestivalPrize(Event @event, string[] args, EventContext context)
    {
      if (args.Length == 1)
      {
        switch (@event.id)
        {
          case "festival_spring13":
            if (@event.festivalWinners.Contains(Game1.player.UniqueMultiplayerID))
            {
              if (Game1.player.mailReceived.Add("Egg Festival"))
              {
                if (Game1.activeClickableMenu == null)
                  Game1.player.addItemByMenuIfNecessary(ItemRegistry.Create("(H)4"));
                ++@event.CurrentCommand;
                if (Game1.activeClickableMenu != null)
                  return;
                ++@event.CurrentCommand;
                return;
              }
              if (Game1.activeClickableMenu == null)
                Game1.player.addItemByMenuIfNecessary(ItemRegistry.Create("(O)PrizeTicket"));
              ++@event.CurrentCommand;
              if (Game1.activeClickableMenu != null)
                return;
              ++@event.CurrentCommand;
              return;
            }
            @event.CurrentCommand += 2;
            return;
          case "festival_winter8":
            if (@event.festivalWinners.Contains(Game1.player.UniqueMultiplayerID))
            {
              if (Game1.player.mailReceived.Add("Ice Festival"))
              {
                if (Game1.activeClickableMenu == null)
                  Game1.activeClickableMenu = (IClickableMenu) new ItemGrabMenu((IList<Item>) new Item[4]
                  {
                    ItemRegistry.Create("(H)17"),
                    ItemRegistry.Create("(O)687"),
                    ItemRegistry.Create("(O)691"),
                    ItemRegistry.Create("(O)703")
                  }, (object) @event).setEssential(true);
                ++@event.CurrentCommand;
                return;
              }
              if (Game1.activeClickableMenu == null)
                Game1.player.addItemByMenuIfNecessary(ItemRegistry.Create("(O)PrizeTicket"));
              ++@event.CurrentCommand;
              if (Game1.activeClickableMenu != null)
                return;
              ++@event.CurrentCommand;
              return;
            }
            @event.CurrentCommand += 2;
            return;
        }
      }
      string itemId;
      string error;
      if (!ArgUtility.TryGet(args, 1, out itemId, out error, name: "string itemId"))
      {
        context.LogErrorAndSkip(error);
      }
      else
      {
        string lower = itemId.ToLower();
        if (lower != null)
        {
          switch (lower.Length)
          {
            case 3:
              switch (lower[2])
              {
                case 'd':
                  if (lower == "rod")
                  {
                    Game1.player.addItemByMenuIfNecessary(ItemRegistry.Create("(T)BambooPole"));
                    if (Game1.activeClickableMenu == null)
                      ++@event.CurrentCommand;
                    ++@event.CurrentCommand;
                    return;
                  }
                  break;
                case 'n':
                  if (lower == "pan")
                  {
                    Game1.player.addItemByMenuIfNecessary(ItemRegistry.Create("(T)Pan"));
                    if (Game1.activeClickableMenu == null)
                      ++@event.CurrentCommand;
                    ++@event.CurrentCommand;
                    return;
                  }
                  break;
                case 't':
                  if (lower == "pot")
                  {
                    Game1.player.addItemByMenuIfNecessary(ItemRegistry.Create("(BC)62"));
                    if (Game1.activeClickableMenu == null)
                      ++@event.CurrentCommand;
                    ++@event.CurrentCommand;
                    return;
                  }
                  break;
              }
              break;
            case 4:
              switch (lower[0])
              {
                case 'h':
                  if (lower == "hero")
                  {
                    Game1.getSteamAchievement("Achievement_LocalLegend");
                    Game1.player.addItemByMenuIfNecessary(ItemRegistry.Create("(BC)116"));
                    if (Game1.activeClickableMenu == null)
                      ++@event.CurrentCommand;
                    ++@event.CurrentCommand;
                    return;
                  }
                  break;
                case 'j':
                  if (lower == "joja")
                  {
                    Game1.getSteamAchievement("Achievement_Joja");
                    Game1.player.addItemByMenuIfNecessary(ItemRegistry.Create("(BC)117"));
                    if (Game1.activeClickableMenu == null)
                      ++@event.CurrentCommand;
                    ++@event.CurrentCommand;
                    return;
                  }
                  break;
              }
              break;
            case 5:
              if (lower == "sword")
              {
                Game1.player.addItemByMenuIfNecessary(ItemRegistry.Create("(W)0"));
                if (Game1.activeClickableMenu == null)
                  ++@event.CurrentCommand;
                ++@event.CurrentCommand;
                return;
              }
              break;
            case 6:
              if (lower == "qimilk")
              {
                if (Game1.player.mailReceived.Add("qiCave"))
                  Game1.player.maxHealth += 25;
                ++@event.CurrentCommand;
                return;
              }
              break;
            case 7:
              switch (lower[0])
              {
                case 'j':
                  if (lower == "jukebox")
                  {
                    Game1.player.addItemByMenuIfNecessary(ItemRegistry.Create("(BC)209"));
                    if (Game1.activeClickableMenu == null)
                      ++@event.CurrentCommand;
                    ++@event.CurrentCommand;
                    return;
                  }
                  break;
                case 'm':
                  if (lower == "memento")
                  {
                    Object @object = ItemRegistry.Create<Object>("(O)864");
                    @object.specialItem = true;
                    @object.questItem.Value = true;
                    Game1.player.addItemByMenuIfNecessary((Item) @object);
                    if (Game1.activeClickableMenu == null)
                      ++@event.CurrentCommand;
                    ++@event.CurrentCommand;
                    return;
                  }
                  break;
              }
              break;
            case 8:
              switch (lower[0])
              {
                case 'm':
                  if (lower == "meowmere")
                  {
                    Game1.player.addItemByMenuIfNecessary(ItemRegistry.Create("(W)65"));
                    if (Game1.activeClickableMenu == null)
                      ++@event.CurrentCommand;
                    ++@event.CurrentCommand;
                    return;
                  }
                  break;
                case 's':
                  if (lower == "slimeegg")
                  {
                    Game1.player.addItemByMenuIfNecessary(ItemRegistry.Create("(O)680"));
                    if (Game1.activeClickableMenu == null)
                      ++@event.CurrentCommand;
                    ++@event.CurrentCommand;
                    return;
                  }
                  break;
              }
              break;
            case 9:
              if (lower == "sculpture")
              {
                Game1.player.addItemByMenuIfNecessary(ItemRegistry.Create("(F)1306"));
                if (Game1.activeClickableMenu == null)
                  ++@event.CurrentCommand;
                ++@event.CurrentCommand;
                return;
              }
              break;
            case 10:
              if (lower == "samboombox")
              {
                Game1.player.addItemByMenuIfNecessary(ItemRegistry.Create("(F)1309"));
                if (Game1.activeClickableMenu == null)
                  ++@event.CurrentCommand;
                ++@event.CurrentCommand;
                return;
              }
              break;
            case 12:
              switch (lower[0])
              {
                case 'b':
                  if (lower == "birdiereward")
                  {
                    Game1.player.team.RequestLimitedNutDrops("Birdie", (GameLocation) null, 0, 0, 5, 5);
                    if (!Game1.MasterPlayer.hasOrWillReceiveMail("gotBirdieReward"))
                      Game1.addMailForTomorrow("gotBirdieReward", true, true);
                    ++@event.CurrentCommand;
                    ++@event.CurrentCommand;
                    return;
                  }
                  break;
                case 'e':
                  if (lower == "emilyclothes")
                  {
                    Clothing clothing = ItemRegistry.Create<Clothing>("(P)8");
                    clothing.Dye(new Color(0, 143, 239), 1f);
                    Game1.player.addItemsByMenuIfNecessary(new List<Item>()
                    {
                      ItemRegistry.Create("(B)804"),
                      ItemRegistry.Create("(H)41"),
                      ItemRegistry.Create("(S)1127"),
                      (Item) clothing
                    });
                    if (Game1.activeClickableMenu == null)
                      ++@event.CurrentCommand;
                    ++@event.CurrentCommand;
                    return;
                  }
                  break;
              }
              break;
            case 14:
              if (lower == "marniepainting")
              {
                Game1.player.addItemByMenuIfNecessary(ItemRegistry.Create("(F)1802"));
                if (Game1.activeClickableMenu == null)
                  ++@event.CurrentCommand;
                ++@event.CurrentCommand;
                return;
              }
              break;
          }
        }
        Game1.player.addItemByMenuIfNecessary(ItemRegistry.Create(itemId));
        if (Game1.activeClickableMenu == null)
          ++@event.CurrentCommand;
        ++@event.CurrentCommand;
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void AttachCharacterToTempSprite(
      Event @event,
      string[] args,
      EventContext context)
    {
      string name;
      string error;
      if (!ArgUtility.TryGet(args, 1, out name, out error, name: "string actorName"))
      {
        context.LogErrorAndSkip(error);
      }
      else
      {
        TemporaryAnimatedSprite temporaryAnimatedSprite = context.Location.temporarySprites.Last<TemporaryAnimatedSprite>();
        if (temporaryAnimatedSprite == null)
          return;
        temporaryAnimatedSprite.attachedCharacter = (Character) @event.getActorByName(name);
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void Fork(Event @event, string[] args, EventContext context)
    {
      string str1;
      string error;
      string str2;
      bool flag;
      if (!ArgUtility.TryGet(args, 1, out str1, out error, name: "string requiredId") || !ArgUtility.TryGetOptional(args, 2, out str2, out error, name: "string newKey") || !ArgUtility.TryGetOptionalBool(args, 3, out flag, out error, name: "bool isTranslationKey"))
      {
        context.LogErrorAndSkip(error);
      }
      else
      {
        if (str2 == null)
        {
          str2 = str1;
          str1 = (string) null;
        }
        if ((str1 != null ? (Game1.player.mailReceived.Contains(str1) ? 1 : (Game1.player.dialogueQuestionsAnswered.Contains(str1) ? 1 : 0)) : (@event.specialEventVariable1 ? 1 : 0)) == 0)
        {
          ++@event.CurrentCommand;
        }
        else
        {
          string[] commands;
          if (flag)
          {
            string rawScript = Game1.content.LoadStringReturnNullIfNotFound(str2);
            if (rawScript == null)
            {
              context.LogErrorAndSkip($"can't load new script from translation key '{str2}' because that translation wasn't found");
              return;
            }
            commands = Event.ParseCommands(rawScript, context.Event.farmer);
          }
          else if (@event.isFestival)
          {
            string data;
            if (!@event.TryGetFestivalDataForYear(str2, out data))
            {
              context.LogErrorAndSkip($"can't load new script from festival field '{str2}' because there's no such key in the '{@event.id}' festival");
              return;
            }
            commands = Event.ParseCommands(data, context.Event.farmer);
          }
          else
          {
            string assetName = "Data\\Events\\" + Game1.currentLocation.Name;
            if (!Game1.content.DoesAssetExist<Dictionary<string, string>>(assetName))
            {
              context.LogErrorAndSkip($"can't load new script from event asset '{assetName}' because it doesn't exist");
              return;
            }
            string rawScript;
            if (!Game1.content.Load<Dictionary<string, string>>(assetName).TryGetValue(str2, out rawScript))
            {
              context.LogErrorAndSkip($"can't load new script from event asset '{assetName}' because it doesn't contain the required '{str2}' key");
              return;
            }
            commands = Event.ParseCommands(rawScript, context.Event.farmer);
          }
          @event.ReplaceAllCommands(commands);
          @event.forked = true;
        }
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void SwitchEvent(Event @event, string[] args, EventContext context)
    {
      string key;
      string error;
      if (!ArgUtility.TryGet(args, 1, out key, out error, name: "string newKey"))
      {
        context.LogErrorAndSkip(error);
      }
      else
      {
        string[] commands;
        if (@event.isFestival)
        {
          string data;
          if (!@event.TryGetFestivalDataForYear(key, out data))
          {
            context.LogErrorAndSkip($"can't load new event from festival field '{key}' because there's no such key in the '{@event.id}' festival");
            return;
          }
          commands = Event.ParseCommands(data, context.Event.farmer);
        }
        else
        {
          string assetName = "Data\\Events\\" + Game1.currentLocation.Name;
          if (!Game1.content.DoesAssetExist<Dictionary<string, string>>(assetName))
          {
            context.LogErrorAndSkip($"can't load new event from asset '{assetName}' because it doesn't exist");
            return;
          }
          string rawScript;
          if (!Game1.content.Load<Dictionary<string, string>>(assetName).TryGetValue(key, out rawScript))
          {
            context.LogErrorAndSkip($"can't load new event from asset '{assetName}' because it doesn't contain the required '{key}' key");
            return;
          }
          commands = Event.ParseCommands(rawScript, context.Event.farmer);
        }
        @event.ReplaceAllCommands(commands);
        @event.eventSwitched = true;
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void GlobalFade(Event @event, string[] args, EventContext context)
    {
      float fadeSpeed;
      string error;
      bool flag;
      if (!ArgUtility.TryGetOptionalFloat(args, 1, out fadeSpeed, out error, 0.007f, "float fadeSpeed") || !ArgUtility.TryGetOptionalBool(args, 2, out flag, out error, name: "bool continueEventDuringFade"))
      {
        context.LogErrorAndSkip(error);
      }
      else
      {
        if (Game1.globalFade)
          return;
        if (flag)
        {
          Game1.globalFadeToBlack(fadeSpeed: fadeSpeed);
          ++@event.CurrentCommand;
        }
        else
          Game1.globalFadeToBlack(new Game1.afterFadeFunction(@event.incrementCommandAfterFade), fadeSpeed);
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void GlobalFadeToClear(Event @event, string[] args, EventContext context)
    {
      float fadeSpeed;
      string error;
      bool flag;
      if (!ArgUtility.TryGetOptionalFloat(args, 1, out fadeSpeed, out error, 0.007f, "float fadeSpeed") || !ArgUtility.TryGetOptionalBool(args, 2, out flag, out error, name: "bool continueEventDuringFade"))
      {
        context.LogErrorAndSkip(error);
      }
      else
      {
        if (Game1.globalFade)
          return;
        if (flag)
        {
          Game1.globalFadeToClear(fadeSpeed: fadeSpeed);
          ++@event.CurrentCommand;
        }
        else
          Game1.globalFadeToClear(new Game1.afterFadeFunction(@event.incrementCommandAfterFade), fadeSpeed);
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void Cutscene(Event @event, string[] args, EventContext context)
    {
      string str;
      string error;
      if (!ArgUtility.TryGet(args, 1, out str, out error, name: "string cutsceneId"))
      {
        context.LogErrorAndSkip(error);
      }
      else
      {
        GameLocation location = context.Location;
        GameTime time = context.Time;
        if (@event.currentCustomEventScript != null)
        {
          if (!@event.currentCustomEventScript.update(time, @event))
            return;
          @event.currentCustomEventScript = (ICustomEventScript) null;
          ++@event.CurrentCommand;
        }
        else
        {
          if (Game1.currentMinigame != null)
            return;
          if (str != null)
          {
            switch (str.Length)
            {
              case 5:
                switch (str[0])
                {
                  case 'p':
                    if (str == "plane")
                    {
                      Game1.currentMinigame = (IMinigame) new PlaneFlyBy();
                      break;
                    }
                    break;
                  case 'r':
                    if (str == "robot")
                    {
                      Game1.currentMinigame = (IMinigame) new RobotBlastoff();
                      break;
                    }
                    break;
                }
                break;
              case 8:
                switch (str[0])
                {
                  case 'b':
                    if (str == "bandFork")
                    {
                      int answerChoice = 76;
                      if (Game1.player.dialogueQuestionsAnswered.Contains("77"))
                        answerChoice = 77;
                      else if (Game1.player.dialogueQuestionsAnswered.Contains("78"))
                        answerChoice = 78;
                      else if (Game1.player.dialogueQuestionsAnswered.Contains("79"))
                        answerChoice = 79;
                      @event.answerDialogue("bandFork", answerChoice);
                      ++@event.CurrentCommand;
                      return;
                    }
                    break;
                  case 'g':
                    if (str == "greenTea")
                    {
                      @event.currentCustomEventScript = (ICustomEventScript) new EventScript_GreenTea(new Vector2(-64000f, -64000f), @event);
                      break;
                    }
                    break;
                }
                break;
              case 9:
                switch (str[0])
                {
                  case 'b':
                    if (str == "boardGame")
                    {
                      Game1.currentMinigame = (IMinigame) new FantasyBoardGame();
                      ++@event.CurrentCommand;
                      break;
                    }
                    break;
                  case 'h':
                    if (str == "haleyCows")
                    {
                      Game1.currentMinigame = (IMinigame) new HaleyCowPictures();
                      break;
                    }
                    break;
                  case 'm':
                    if (str == "marucomet")
                    {
                      Game1.currentMinigame = (IMinigame) new MaruComet();
                      break;
                    }
                    break;
                }
                break;
              case 11:
                if (str == "AbigailGame")
                {
                  Game1.currentMinigame = (IMinigame) new AbigailGame(@event.getActorByName("Abigail") ?? Game1.RequireCharacter("Abigail"));
                  break;
                }
                break;
              case 13:
                switch (str[0])
                {
                  case 'b':
                    if (str == "balloonDepart")
                    {
                      TemporaryAnimatedSprite temporarySpriteById1 = location.getTemporarySpriteByID(1);
                      temporarySpriteById1.attachedCharacter = (Character) @event.farmer;
                      temporarySpriteById1.motion = new Vector2(0.0f, -2f);
                      TemporaryAnimatedSprite temporarySpriteById2 = location.getTemporarySpriteByID(2);
                      temporarySpriteById2.attachedCharacter = (Character) @event.getActorByName("Harvey");
                      temporarySpriteById2.motion = new Vector2(0.0f, -2f);
                      location.getTemporarySpriteByID(3).scaleChange = -0.01f;
                      ++@event.CurrentCommand;
                      return;
                    }
                    break;
                  case 'e':
                    if (str == "eggHuntWinner")
                    {
                      @event.eggHuntWinner();
                      ++@event.CurrentCommand;
                      return;
                    }
                    break;
                  case 'g':
                    if (str == "governorTaste")
                    {
                      @event.governorTaste();
                      ++@event.currentCommand;
                      return;
                    }
                    break;
                }
                break;
              case 14:
                if (str == "linusMoneyGone")
                {
                  foreach (TemporaryAnimatedSprite temporarySprite in location.temporarySprites)
                  {
                    temporarySprite.alphaFade = 0.01f;
                    temporarySprite.motion = new Vector2(0.0f, -1f);
                  }
                  ++@event.CurrentCommand;
                  return;
                }
                break;
              case 16 /*0x10*/:
                switch (str[0])
                {
                  case 'b':
                    if (str == "balloonChangeMap")
                    {
                      @event.eventPositionTileOffset = Vector2.Zero;
                      location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(0, 1183, 84, 160 /*0xA0*/), 10000f, 1, 99999, @event.OffsetPosition(new Vector2(22f, 36f) * 64f + new Vector2(-23f, 0.0f) * 4f), false, false, 2E-05f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
                      {
                        motion = new Vector2(0.0f, -2f),
                        yStopCoordinate = (int) @event.OffsetPositionY(576f),
                        reachedStopCoordinate = new TemporaryAnimatedSprite.endBehavior(@event.balloonInSky),
                        attachedCharacter = (Character) @event.farmer,
                        id = 1
                      });
                      location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(84, 1205, 38, 26), 10000f, 1, 99999, @event.OffsetPosition(new Vector2(22f, 36f) * 64f + new Vector2(0.0f, 134f) * 4f), false, false, 0.2625f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
                      {
                        motion = new Vector2(0.0f, -2f),
                        id = 2,
                        attachedCharacter = (Character) @event.getActorByName("Harvey")
                      });
                      ++@event.CurrentCommand;
                      break;
                    }
                    break;
                  case 'c':
                    if (str == "clearTempSprites")
                    {
                      location.temporarySprites.Clear();
                      ++@event.CurrentCommand;
                      break;
                    }
                    break;
                  case 'i':
                    if (str == "iceFishingWinner")
                    {
                      @event.iceFishingWinner();
                      ++@event.currentCommand;
                      return;
                    }
                    break;
                }
                break;
              case 18:
                switch (str[0])
                {
                  case 'a':
                    if (str == "addSecretSantaItem")
                    {
                      Game1.player.addItemByMenuIfNecessaryElseHoldUp(Utility.getGiftFromNPC(@event.mySecretSanta));
                      ++@event.currentCommand;
                      return;
                    }
                    break;
                  case 'i':
                    if (str == "iceFishingWinnerMP")
                    {
                      @event.iceFishingWinnerMP();
                      ++@event.currentCommand;
                      return;
                    }
                    break;
                }
                break;
            }
          }
          Game1.globalFadeToClear(fadeSpeed: 0.01f);
        }
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void WaitForTempSprite(Event @event, string[] args, EventContext context)
    {
      int id;
      string error;
      if (!ArgUtility.TryGetInt(args, 1, out id, out error, "int spriteId"))
      {
        context.LogErrorAndSkip(error);
      }
      else
      {
        if (Game1.currentLocation.getTemporarySpriteByID(id) == null)
          return;
        ++@event.CurrentCommand;
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void Cave(Event @event, string[] args, EventContext context)
    {
      if (Game1.activeClickableMenu != null)
        return;
      Response[] answerChoices = new Response[2]
      {
        new Response("Mushrooms", Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1220")),
        new Response("Bats", Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1222"))
      };
      Game1.currentLocation.createQuestionDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1223"), answerChoices, "cave");
      Game1.dialogueTyping = false;
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void UpdateMinigame(Event @event, string[] args, EventContext context)
    {
      int data;
      string error;
      if (!ArgUtility.TryGetInt(args, 1, out data, out error, "int eventData"))
      {
        context.LogErrorAndSkip(error);
      }
      else
      {
        Game1.currentMinigame?.receiveEventPoke(data);
        ++@event.CurrentCommand;
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void StartJittering(Event @event, string[] args, EventContext context)
    {
      @event.farmer.jitterStrength = 1f;
      ++@event.CurrentCommand;
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void Money(Event @event, string[] args, EventContext context)
    {
      int num;
      string error;
      if (!ArgUtility.TryGetInt(args, 1, out num, out error, "int amount"))
      {
        context.LogErrorAndSkip(error);
      }
      else
      {
        @event.farmer.Money += num;
        if (@event.farmer.Money < 0)
          @event.farmer.Money = 0;
        ++@event.CurrentCommand;
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void StopJittering(Event @event, string[] args, EventContext context)
    {
      @event.farmer.stopJittering();
      ++@event.CurrentCommand;
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void AddLantern(Event @event, string[] args, EventContext context)
    {
      int initialParentTileIndex;
      string error;
      Vector2 vector2;
      int num;
      if (!ArgUtility.TryGetInt(args, 1, out initialParentTileIndex, out error, "int initialParentSheetIndex") || !ArgUtility.TryGetVector2(args, 2, out vector2, out error, true, "Vector2 tile") || !ArgUtility.TryGetInt(args, 4, out num, out error, "int lightRadius"))
      {
        context.LogErrorAndSkip(error);
      }
      else
      {
        TemporaryAnimatedSpriteList temporarySprites = context.Location.TemporarySprites;
        TemporaryAnimatedSprite temporaryAnimatedSprite = new TemporaryAnimatedSprite(initialParentTileIndex, 999999f, 1, 0, @event.OffsetPosition(vector2 * 64f), false, false);
        temporaryAnimatedSprite.lightId = @event.GenerateLightSourceId($"{nameof (AddLantern)}_{vector2.X}_{vector2.Y}");
        temporaryAnimatedSprite.lightRadius = (float) num;
        temporarySprites.Add(temporaryAnimatedSprite);
        ++@event.CurrentCommand;
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void RustyKey(Event @event, string[] args, EventContext context)
    {
      Game1.player.hasRustyKey = true;
      ++@event.CurrentCommand;
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void Swimming(Event @event, string[] args, EventContext context)
    {
      string str;
      string error;
      if (!ArgUtility.TryGet(args, 1, out str, out error, name: "string actorName"))
      {
        context.LogErrorAndSkip(error);
      }
      else
      {
        int farmerNumber;
        if (@event.IsFarmerActorId(str, out farmerNumber))
        {
          Farmer farmerActor = @event.GetFarmerActor(farmerNumber);
          if (farmerActor != null)
          {
            farmerActor.bathingClothes.Value = true;
            farmerActor.swimming.Value = true;
          }
        }
        else
        {
          bool isOptionalNpc;
          NPC actorByName = @event.getActorByName(str, out isOptionalNpc);
          if (actorByName == null)
          {
            context.LogErrorAndSkip($"no NPC found with name '{str}'", isOptionalNpc);
            return;
          }
          actorByName.swimming.Value = true;
        }
        ++@event.CurrentCommand;
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void StopSwimming(Event @event, string[] args, EventContext context)
    {
      string str;
      string error;
      if (!ArgUtility.TryGet(args, 1, out str, out error, name: "string actorName"))
      {
        context.LogErrorAndSkip(error);
      }
      else
      {
        int farmerNumber;
        if (@event.IsFarmerActorId(str, out farmerNumber))
        {
          Farmer farmerActor = @event.GetFarmerActor(farmerNumber);
          if (farmerActor != null)
          {
            farmerActor.bathingClothes.Value = context.Location is BathHousePool;
            farmerActor.swimming.Value = false;
          }
        }
        else
        {
          bool isOptionalNpc;
          NPC actorByName = @event.getActorByName(str, out isOptionalNpc);
          if (actorByName == null)
          {
            context.LogErrorAndSkip($"no NPC found with name '{str}'", isOptionalNpc);
            return;
          }
          actorByName.swimming.Value = false;
        }
        ++@event.CurrentCommand;
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void TutorialMenu(Event @event, string[] args, EventContext context)
    {
      if (Game1.activeClickableMenu != null)
        return;
      Game1.activeClickableMenu = (IClickableMenu) new TutorialMenu();
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void AnimalNaming(Event @event, string[] args, EventContext context)
    {
      AnimalHouse animalHouse = Game1.currentLocation as AnimalHouse;
      if (animalHouse == null)
      {
        context.LogErrorAndSkip("this command only works when run in an AnimalHouse location");
      }
      else
      {
        if (Game1.activeClickableMenu != null)
          return;
        Game1.activeClickableMenu = (IClickableMenu) new NamingMenu((NamingMenu.doneNamingBehavior) (animalName =>
        {
          animalHouse.addNewHatchedAnimal(animalName);
          ++@event.CurrentCommand;
        }), Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1236"));
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void SplitSpeak(Event @event, string[] args, EventContext context)
    {
      string name;
      string error;
      string str;
      if (!ArgUtility.TryGet(args, 1, out name, out error, name: "string actorName") || !ArgUtility.TryGet(args, 2, out str, out error, name: "string dialogue"))
      {
        context.LogErrorAndSkip(error);
      }
      else
      {
        string[] array = LegacyShims.SplitAndTrim(str, '~');
        if (Game1.dialogueUp)
          return;
        @event.timeAccumulator += (float) context.Time.ElapsedGameTime.Milliseconds;
        if ((double) @event.timeAccumulator < 500.0)
          return;
        @event.timeAccumulator = 0.0f;
        bool isOptionalNpc;
        NPC speaker = @event.getActorByName(name, out isOptionalNpc) ?? Game1.getCharacterFromName(name);
        if (speaker == null)
          context.LogErrorAndSkip($"no NPC found with name '{name}'", isOptionalNpc);
        else if (!ArgUtility.HasIndex<string>(array, @event.previousAnswerChoice))
        {
          ++@event.CurrentCommand;
        }
        else
        {
          speaker.CurrentDialogue.Push(new Dialogue(speaker, (string) null, array[@event.previousAnswerChoice]));
          Game1.drawDialogue(speaker);
        }
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void CatQuestion(Event @event, string[] args, EventContext context)
    {
      if (Game1.isQuestion || Game1.activeClickableMenu != null)
        return;
      PetData data;
      string sub1 = Pet.TryGetData(Game1.player.whichPetType, out data) ? TokenParser.ParseText(data.DisplayName) ?? "pet" : "pet";
      Game1.currentLocation.createQuestionDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:AdoptPet", (object) sub1), Game1.currentLocation.createYesNoResponses(), "pet");
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void AmbientLight(Event @event, string[] args, EventContext context)
    {
      int r;
      string error;
      int g;
      int b;
      if (!ArgUtility.TryGetInt(args, 1, out r, out error, "int red") || !ArgUtility.TryGetInt(args, 2, out g, out error, "int green") || !ArgUtility.TryGetInt(args, 3, out b, out error, "int blue"))
      {
        context.LogErrorAndSkip(error);
      }
      else
      {
        Game1.ambientLight = new Color(r, g, b);
        ++@event.CurrentCommand;
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void BgColor(Event @event, string[] args, EventContext context)
    {
      int r;
      string error;
      int g;
      int b;
      if (!ArgUtility.TryGetInt(args, 1, out r, out error, "int red") || !ArgUtility.TryGetInt(args, 2, out g, out error, "int green") || !ArgUtility.TryGetInt(args, 3, out b, out error, "int blue"))
      {
        context.LogErrorAndSkip(error);
      }
      else
      {
        Game1.setBGColor((byte) r, (byte) g, (byte) b);
        ++@event.CurrentCommand;
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void ElliottBookTalk(Event @event, string[] args, EventContext context)
    {
      if (Game1.dialogueUp)
        return;
      string translationKey = !Game1.player.dialogueQuestionsAnswered.Contains("958699") ? (!Game1.player.dialogueQuestionsAnswered.Contains("958700") ? (!Game1.player.dialogueQuestionsAnswered.Contains("9586701") ? "Strings\\StringsFromCSFiles:Event.cs.1260" : "Strings\\StringsFromCSFiles:Event.cs.1259") : "Strings\\StringsFromCSFiles:Event.cs.1258") : "Strings\\StringsFromCSFiles:Event.cs.1257";
      NPC speaker = @event.getActorByName("Elliott") ?? Game1.getCharacterFromName("Elliott");
      speaker.CurrentDialogue.Push(new Dialogue(speaker, translationKey));
      Game1.drawDialogue(speaker);
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void RemoveItem(Event @event, string[] args, EventContext context)
    {
      string itemId;
      string error;
      int count;
      if (!ArgUtility.TryGet(args, 1, out itemId, out error, name: "string itemId") || !ArgUtility.TryGetOptionalInt(args, 2, out count, out error, 1, "int count"))
      {
        context.LogErrorAndSkip(error);
      }
      else
      {
        Game1.player.removeFirstOfThisItemFromInventory(itemId, count);
        ++@event.CurrentCommand;
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void Friendship(Event @event, string[] args, EventContext context)
    {
      string name;
      string error;
      int amount;
      if (!ArgUtility.TryGet(args, 1, out name, out error, name: "string actorName") || !ArgUtility.TryGetInt(args, 2, out amount, out error, "int friendshipChange"))
      {
        context.LogErrorAndSkip(error);
      }
      else
      {
        NPC characterFromName = Game1.getCharacterFromName(name);
        if (characterFromName == null)
        {
          context.LogErrorAndSkip($"no NPC found with name '{name}'");
        }
        else
        {
          Game1.player.changeFriendship(amount, characterFromName);
          ++@event.CurrentCommand;
        }
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void SetRunning(Event @event, string[] args, EventContext context)
    {
      @event.farmer.setRunning(true);
      ++@event.CurrentCommand;
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void ExtendSourceRect(Event @event, string[] args, EventContext context)
    {
      string name;
      string error;
      string str;
      if (!ArgUtility.TryGet(args, 1, out name, out error, name: "string actorName") || !ArgUtility.TryGet(args, 2, out str, out error, name: "string rawOption"))
      {
        context.LogErrorAndSkip(error);
      }
      else
      {
        bool flag = str == "reset";
        int horizontal = -1;
        int vertical = -1;
        bool ignoreSourceRectUpdates = false;
        if (!flag && (!ArgUtility.TryGetInt(args, 2, out horizontal, out error, "horizontal") || !ArgUtility.TryGetInt(args, 3, out vertical, out error, "vertical") || !ArgUtility.TryGetOptionalBool(args, 4, out ignoreSourceRectUpdates, out error, name: "ignoreSourceRectUpdates")))
        {
          context.LogErrorAndSkip(error);
        }
        else
        {
          bool isOptionalNpc;
          NPC actorByName = @event.getActorByName(name, out isOptionalNpc);
          if (actorByName == null)
          {
            context.LogErrorAndSkip($"no NPC found with name '{name}'", isOptionalNpc);
          }
          else
          {
            if (flag)
            {
              actorByName.reloadSprite();
              actorByName.Sprite.SpriteWidth = 16 /*0x10*/;
              actorByName.Sprite.SpriteHeight = 32 /*0x20*/;
              actorByName.HideShadow = false;
            }
            else
              actorByName.extendSourceRect(horizontal, vertical, ignoreSourceRectUpdates);
            ++@event.CurrentCommand;
          }
        }
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void WaitForOtherPlayers(Event @event, string[] args, EventContext context)
    {
      string str;
      string error;
      if (!ArgUtility.TryGet(args, 1, out str, out error, name: "string gateId"))
        context.LogErrorAndSkip(error);
      else if (Game1.IsMultiplayer)
      {
        Game1.netReady.SetLocalReady(str, true);
        if (Game1.netReady.IsReady(str))
        {
          if (Game1.activeClickableMenu is ReadyCheckDialog)
            Game1.exitActiveMenu();
          ++@event.CurrentCommand;
        }
        else
        {
          if (Game1.activeClickableMenu != null)
            return;
          Game1.activeClickableMenu = (IClickableMenu) new ReadyCheckDialog(str, false);
        }
      }
      else
        ++@event.CurrentCommand;
    }

    /// <summary>Used in the movie theater, requests that the server end the movie.</summary>
    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void RequestMovieEnd(Event @event, string[] args, EventContext context)
    {
      Game1.player.team.requestMovieEndEvent.Fire(Game1.player.UniqueMultiplayerID);
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void RestoreStashedItem(Event @event, string[] args, EventContext context)
    {
      Game1.player.TemporaryItem = (Item) null;
      ++@event.CurrentCommand;
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void AdvancedMove(Event @event, string[] args, EventContext context)
    {
      @event.setUpAdvancedMove(args);
      ++@event.CurrentCommand;
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void StopRunning(Event @event, string[] args, EventContext context)
    {
      @event.farmer.setRunning(false);
      ++@event.CurrentCommand;
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void Eyes(Event @event, string[] args, EventContext context)
    {
      int num1;
      string error;
      int num2;
      if (!ArgUtility.TryGetInt(args, 1, out num1, out error, "int eyes") || !ArgUtility.TryGetInt(args, 2, out num2, out error, "int blinkTimer"))
      {
        context.LogErrorAndSkip(error);
      }
      else
      {
        @event.farmer.currentEyes = num1;
        @event.farmer.blinkTimer = num2;
        ++@event.CurrentCommand;
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    [OtherNames(new string[] {"mailReceived"})]
    public static void AddMailReceived(Event @event, string[] args, EventContext context)
    {
      string str;
      string error;
      bool add;
      if (!ArgUtility.TryGet(args, 1, out str, out error, name: "string mailId") || !ArgUtility.TryGetOptionalBool(args, 2, out add, out error, true, "bool add"))
      {
        context.LogErrorAndSkip(error);
      }
      else
      {
        Game1.player.mailReceived.Toggle<string>(str, add);
        ++@event.CurrentCommand;
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void AddWorldState(Event @event, string[] args, EventContext context)
    {
      string id;
      string error;
      if (!ArgUtility.TryGet(args, 1, out id, out error, name: "string worldStateId"))
      {
        context.LogErrorAndSkip(error);
      }
      else
      {
        Game1.worldStateIDs.Add(id);
        Game1.netWorldState.Value.addWorldStateID(id);
        ++@event.CurrentCommand;
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void Fade(Event @event, string[] args, EventContext context)
    {
      string str = ArgUtility.Get(args, 1);
      if (str == "unfade")
      {
        Game1.fadeIn = false;
        Game1.fadeToBlack = false;
        ++@event.CurrentCommand;
      }
      else
      {
        Game1.fadeToBlack = true;
        Game1.fadeIn = true;
        if ((double) Game1.fadeToBlackAlpha < 0.97000002861022949)
          return;
        if (str == null)
          Game1.fadeIn = false;
        ++@event.CurrentCommand;
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void ChangeMapTile(Event @event, string[] args, EventContext context)
    {
      string layerId;
      string error1;
      Point point;
      int num;
      if (!ArgUtility.TryGet(args, 1, out layerId, out error1, name: "string layerId") || !ArgUtility.TryGetPoint(args, 2, out point, out error1, "Point tilePos") || !ArgUtility.TryGetInt(args, 4, out num, out error1, "int newTileIndex"))
      {
        context.LogErrorAndSkip(error1);
      }
      else
      {
        Layer layer = context.Location.map.GetLayer(layerId);
        if (layer == null)
        {
          context.LogErrorAndSkip($"the '{context.Location.NameOrUniqueName}' location doesn't have required map layer {layerId}");
        }
        else
        {
          int x = @event.OffsetTileX(point.X);
          int y = @event.OffsetTileY(point.Y);
          Tile tile = layer.Tiles[x, y];
          if (tile == null)
          {
            EventContext eventContext = context;
            string str1 = $"the '{context.Location.NameOrUniqueName}' location doesn't have required tile ({point.X}, {point.Y})";
            string str2;
            if (x == point.X && y == point.Y)
              str2 = "";
            else
              str2 = $" (adjusted to ({x}, {y})";
            string str3 = layerId;
            string error2 = $"{str1}{str2} on layer {str3}";
            eventContext.LogErrorAndSkip(error2);
          }
          else
          {
            tile.TileIndex = num;
            ++@event.CurrentCommand;
          }
        }
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void ChangeSprite(Event @event, string[] args, EventContext context)
    {
      string name;
      string error;
      string str;
      if (!ArgUtility.TryGet(args, 1, out name, out error, name: "string actorName") || !ArgUtility.TryGetOptional(args, 2, out str, out error, name: "string spriteSuffix"))
      {
        context.LogErrorAndSkip(error);
      }
      else
      {
        bool isOptionalNpc;
        NPC actorByName = @event.getActorByName(name, out isOptionalNpc);
        if (actorByName == null)
        {
          context.LogErrorAndSkip($"no NPC found with name '{name}'", isOptionalNpc);
        }
        else
        {
          if (str != null)
          {
            actorByName.spriteOverridden = true;
            actorByName.Sprite.LoadTexture($"Characters\\{NPC.getTextureNameForCharacter(actorByName.Name)}_{str}");
          }
          else
          {
            actorByName.spriteOverridden = false;
            actorByName.ChooseAppearance();
          }
          ++@event.CurrentCommand;
        }
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void WaitForAllStationary(Event @event, string[] args, EventContext context)
    {
      List<NPCController> npcControllers = @event.npcControllers;
      // ISSUE: explicit non-virtual call
      bool flag = npcControllers != null && __nonvirtual (npcControllers.Count) > 0;
      if (!flag)
      {
        foreach (Character actor in @event.actors)
        {
          if (actor.isMoving())
          {
            flag = true;
            break;
          }
        }
      }
      if (!flag)
      {
        foreach (Character farmerActor in @event.farmerActors)
        {
          if (farmerActor.isMoving())
          {
            flag = true;
            break;
          }
        }
      }
      if (flag)
        return;
      ++@event.CurrentCommand;
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void ProceedPosition(Event @event, string[] args, EventContext context)
    {
      string name;
      string error;
      if (!ArgUtility.TryGet(args, 1, out name, out error, name: "string actorName"))
      {
        context.LogErrorAndSkip(error);
      }
      else
      {
        Character characterByName = @event.getCharacterByName(name);
        if (characterByName == null)
        {
          context.LogErrorAndSkip($"no NPC found with name '{name}'");
        }
        else
        {
          @event.continueAfterMove = true;
          try
          {
            if (characterByName.isMoving())
            {
              List<NPCController> npcControllers = @event.npcControllers;
              // ISSUE: explicit non-virtual call
              if ((npcControllers != null ? (__nonvirtual (npcControllers.Count) == 0 ? 1 : 0) : 0) == 0)
                return;
            }
            characterByName.Halt();
            ++@event.CurrentCommand;
          }
          catch
          {
            ++@event.CurrentCommand;
          }
        }
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void ChangePortrait(Event @event, string[] args, EventContext context)
    {
      string name;
      string error;
      string str;
      if (!ArgUtility.TryGet(args, 1, out name, out error, name: "string actorName") || !ArgUtility.TryGetOptional(args, 2, out str, out error, name: "string portraitSuffix"))
      {
        context.LogErrorAndSkip(error);
      }
      else
      {
        bool isOptionalNpc;
        NPC npc = @event.getActorByName(name, out isOptionalNpc) ?? Game1.getCharacterFromName(name);
        if (npc == null)
        {
          context.LogErrorAndSkip($"no NPC found with name '{name}'", isOptionalNpc);
        }
        else
        {
          if (str != null)
          {
            npc.portraitOverridden = true;
            npc.Portrait = Game1.content.Load<Texture2D>($"Portraits\\{NPC.getTextureNameForCharacter(npc.Name)}_{str}");
          }
          else
          {
            npc.portraitOverridden = false;
            npc.ChooseAppearance();
          }
          ++@event.CurrentCommand;
        }
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void ChangeYSourceRectOffset(Event @event, string[] args, EventContext context)
    {
      string name;
      string error;
      int num;
      if (!ArgUtility.TryGet(args, 1, out name, out error, name: "string actorName") || !ArgUtility.TryGetInt(args, 2, out num, out error, "int ySourceRectOffset"))
      {
        context.LogErrorAndSkip(error);
      }
      else
      {
        bool isOptionalNpc;
        NPC actorByName = @event.getActorByName(name, out isOptionalNpc);
        if (actorByName == null)
        {
          context.LogErrorAndSkip($"no NPC found with name '{name}'", isOptionalNpc);
        }
        else
        {
          actorByName.ySourceRectOffset = num;
          ++@event.CurrentCommand;
        }
      }
    }

    /// <summary>Set the display name for an event actor to an exact value.</summary>
    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void ChangeName(Event @event, string[] args, EventContext context)
    {
      string name;
      string error;
      string str;
      if (!ArgUtility.TryGet(args, 1, out name, out error, name: "string actorName") || !ArgUtility.TryGet(args, 2, out str, out error, name: "string newName"))
      {
        context.LogErrorAndSkip(error);
      }
      else
      {
        bool isOptionalNpc;
        NPC actorByName = @event.getActorByName(name, out isOptionalNpc);
        if (actorByName == null)
        {
          context.LogErrorAndSkip($"no NPC found with name '{name}'", isOptionalNpc);
        }
        else
        {
          actorByName.displayName = str;
          ++@event.CurrentCommand;
        }
      }
    }

    /// <summary>Set the display name for an event actor to a translation key.</summary>
    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void TranslateName(Event @event, string[] args, EventContext context)
    {
      string name;
      string error;
      string path;
      if (!ArgUtility.TryGet(args, 1, out name, out error, name: "string actorName") || !ArgUtility.TryGet(args, 2, out path, out error, name: "string translationKey"))
      {
        context.LogErrorAndSkip(error);
      }
      else
      {
        bool isOptionalNpc;
        NPC actorByName = @event.getActorByName(name, out isOptionalNpc);
        if (actorByName == null)
        {
          context.LogErrorAndSkip($"no NPC found with name '{name}'", isOptionalNpc);
        }
        else
        {
          actorByName.displayName = Game1.content.LoadString(path);
          ++@event.CurrentCommand;
        }
      }
    }

    /// <summary>Replace an NPC in the event with a temporary copy that only exists for the duration of the event. This allows changing the NPC in the event (e.g. renaming them) without affecting the real NPC.</summary>
    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void ReplaceWithClone(Event @event, string[] args, EventContext context)
    {
      string name;
      string error;
      if (!ArgUtility.TryGet(args, 1, out name, out error, name: "string actorName"))
      {
        context.LogErrorAndSkip(error);
      }
      else
      {
        bool isOptionalNpc;
        NPC actorByName = @event.getActorByName(name, out isOptionalNpc);
        if (actorByName == null)
        {
          context.LogErrorAndSkip($"no NPC found with name '{name}'", isOptionalNpc);
        }
        else
        {
          @event.actors.Remove(actorByName);
          List<NPC> actors = @event.actors;
          NPC npc = new NPC(actorByName.Sprite.Clone(), actorByName.Position, actorByName.FacingDirection, actorByName.Name);
          npc.Birthday_Day = actorByName.Birthday_Day;
          npc.Birthday_Season = actorByName.Birthday_Season;
          npc.Gender = actorByName.Gender;
          npc.Portrait = actorByName.Portrait;
          npc.EventActor = true;
          npc.displayName = actorByName.displayName;
          npc.drawOffset = actorByName.drawOffset;
          npc.TemporaryDialogue = new Stack<Dialogue>(actorByName.CurrentDialogue.Select<Dialogue, Dialogue>((Func<Dialogue, Dialogue>) (p => new Dialogue(p))));
          actors.Add(npc);
          ++@event.CurrentCommand;
        }
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void PlayFramesAhead(Event @event, string[] args, EventContext context)
    {
      int num;
      string error;
      if (!ArgUtility.TryGetInt(args, 1, out num, out error, "int framesToSkip"))
      {
        context.LogErrorAndSkip(error);
      }
      else
      {
        ++@event.CurrentCommand;
        for (int index = 0; index < num; ++index)
          @event.Update(context.Location, context.Time);
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void ShowKissFrame(Event @event, string[] args, EventContext context)
    {
      string name;
      string error;
      bool flag;
      if (!ArgUtility.TryGet(args, 1, out name, out error, name: "string actorName") || !ArgUtility.TryGetOptionalBool(args, 2, out flag, out error, name: "bool flip"))
      {
        context.LogErrorAndSkip(error);
      }
      else
      {
        bool isOptionalNpc;
        NPC actorByName = @event.getActorByName(name, out isOptionalNpc);
        if (actorByName == null)
        {
          context.LogErrorAndSkip($"no NPC found with name '{name}'", isOptionalNpc);
        }
        else
        {
          CharacterData data = actorByName.GetData();
          int frame = data != null ? data.KissSpriteIndex : 28;
          bool flip = data == null || data.KissSpriteFacingRight;
          if (flag)
            flip = !flip;
          actorByName.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
          {
            new FarmerSprite.AnimationFrame(frame, 1000, false, flip)
          });
          ++@event.CurrentCommand;
        }
      }
    }

    /// <remarks>Format: <c>addTemporaryActor name spriteWidth spriteHeight xPosition yPosition facingDirection breather=true animal=false</c>.</remarks>
    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void AddTemporaryActor(Event @event, string[] args, EventContext context)
    {
      string name;
      string error;
      Point point;
      Vector2 vector2;
      int facingDir;
      bool flag1;
      string str1;
      string str2;
      if (!ArgUtility.TryGet(args, 1, out name, out error, name: "string spriteAssetName") || !ArgUtility.TryGetPoint(args, 2, out point, out error, "Point spriteSize") || !ArgUtility.TryGetVector2(args, 4, out vector2, out error, name: "Vector2 tile") || !ArgUtility.TryGetDirection(args, 6, out facingDir, out error, "int facingDirection") || !ArgUtility.TryGetOptionalBool(args, 7, out flag1, out error, true, "bool isBreather") || !ArgUtility.TryGetOptional(args, 8, out str1, out error, name: "string typeOrDisplayName") || !ArgUtility.TryGetOptional(args, 9, out str2, out error, allowBlank: false, name: "string overrideName"))
      {
        context.LogErrorAndSkip(error);
      }
      else
      {
        string str3 = "Characters\\";
        bool flag2 = true;
        switch (str1?.ToLower())
        {
          case "animal":
            str3 = "Animals\\";
            goto case "character";
          case "monster":
            str3 = "Characters\\Monsters\\";
            goto case "character";
          case "character":
            string str4 = str3 + name;
            if (!Game1.content.DoesAssetExist<Texture2D>(str4))
            {
              string str5 = name.Replace('_', ' ');
              string assetName = str3 + str5;
              if (str5 != name && Game1.content.DoesAssetExist<Texture2D>(assetName))
              {
                name = str5;
                str4 = assetName;
              }
            }
            NPC npc = new NPC(new AnimatedSprite((ContentManager) @event.festivalContent, str4, 0, point.X, point.Y), @event.OffsetPosition(vector2 * 64f), facingDir, name, @event.festivalContent)
            {
              AllowDynamicAppearance = false,
              Breather = flag1
            };
            npc.HideShadow = npc.Sprite.SpriteWidth >= 32 /*0x20*/;
            npc.TemporaryDialogue = new Stack<Dialogue>();
            if (!flag2 && str1 != null)
              npc.displayName = str1;
            Dialogue dialogue;
            if (@event.isFestival && @event.TryGetFestivalDialogueForYear(npc, npc.Name, out dialogue))
              npc.CurrentDialogue.Push(dialogue);
            if (str2 != null)
              npc.Name = str2;
            npc.EventActor = true;
            @event.actors.Add(npc);
            ++@event.CurrentCommand;
            break;
          default:
            flag2 = false;
            goto case "character";
        }
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void ChangeToTemporaryMap(Event @event, string[] args, EventContext context)
    {
      string str;
      string error;
      bool flag;
      if (!ArgUtility.TryGet(args, 1, out str, out error, name: "string mapName") || !ArgUtility.TryGetOptionalBool(args, 2, out flag, out error, true, "bool shouldPan"))
      {
        context.LogErrorAndSkip(error);
      }
      else
      {
        @event.temporaryLocation = str == "Town" ? (GameLocation) new Town("Maps\\Town", "Temp") : (!@event.isFestival || !str.Contains("Town") ? new GameLocation("Maps\\" + str, "Temp") : (GameLocation) new Town("Maps\\" + str, "Temp"));
        @event.temporaryLocation.map.LoadTileSheets(Game1.mapDisplayDevice);
        Event currentEvent = Game1.currentLocation.currentEvent;
        Game1.currentLocation.cleanupBeforePlayerExit();
        Game1.currentLocation.currentEvent = (Event) null;
        Game1.currentLightSources.Clear();
        Game1.currentLocation = @event.temporaryLocation;
        Game1.currentLocation.resetForPlayerEntry();
        Game1.currentLocation.UpdateMapSeats();
        Game1.currentLocation.currentEvent = currentEvent;
        ++@event.CurrentCommand;
        Game1.player.currentLocation = Game1.currentLocation;
        @event.farmer.currentLocation = Game1.currentLocation;
        Game1.currentLocation.ResetForEvent(@event);
        if (!flag)
          return;
        Game1.panScreen(0, 0);
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void PositionOffset(Event @event, string[] args, EventContext context)
    {
      string str;
      string error;
      Point point;
      bool flag;
      if (!ArgUtility.TryGet(args, 1, out str, out error, name: "string actorName") || !ArgUtility.TryGetPoint(args, 2, out point, out error, "Point offset") || !ArgUtility.TryGetOptionalBool(args, 4, out flag, out error, name: "bool continueImmediately"))
      {
        context.LogErrorAndSkip(error);
      }
      else
      {
        int farmerNumber;
        if (@event.IsFarmerActorId(str, out farmerNumber))
        {
          Farmer farmerActor = @event.GetFarmerActor(farmerNumber);
          if (farmerActor != null)
          {
            farmerActor.position.X += (float) point.X;
            farmerActor.position.Y += (float) point.Y;
          }
        }
        else
        {
          bool isOptionalNpc;
          NPC actorByName = @event.getActorByName(str, out isOptionalNpc);
          if (actorByName == null)
          {
            context.LogErrorAndSkip($"no NPC found with name '{str}'", isOptionalNpc);
            return;
          }
          actorByName.position.X += (float) point.X;
          actorByName.position.Y += (float) point.Y;
        }
        ++@event.CurrentCommand;
        if (!flag)
          return;
        @event.Update(context.Location, context.Time);
      }
    }

    /// <remarks>Format: <c>question &lt;questionKey (forkN to make the nth answer fork)&gt; "question#answer1#answer2#...#answerN"</c>.</remarks>
    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void Question(Event @event, string[] args, EventContext context)
    {
      string dialogKey;
      string error;
      string str;
      if (!ArgUtility.TryGet(args, 1, out dialogKey, out error, name: "string dialogueKey") || !ArgUtility.TryGet(args, 2, out str, out error, name: "string rawQuestionsAndAnswers"))
      {
        context.LogErrorAndSkip(error);
      }
      else
      {
        if (Game1.isQuestion || Game1.activeClickableMenu != null)
          return;
        string[] strArray = LegacyShims.SplitAndTrim(str, '#');
        string question = strArray[0];
        Response[] answerChoices = new Response[strArray.Length - 1];
        for (int index = 1; index < strArray.Length; ++index)
          answerChoices[index - 1] = new Response((index - 1).ToString(), strArray[index]);
        Game1.currentLocation.createQuestionDialogue(question, answerChoices, dialogKey);
      }
    }

    /// <remarks>Format: <c>quickQuestion question#answer1#answer2#...#answerN(break)answerLogic1(break)answerLogic2(break)...(break)answerLogicN</c>. Use <c>\</c> instead of <c>/</c> inside the <c>answerLogic</c> sections.</remarks>
    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void QuickQuestion(Event @event, string[] args, EventContext context)
    {
      if (Game1.isQuestion || Game1.activeClickableMenu != null)
        return;
      string currentCommand = @event.GetCurrentCommand();
      string[] strArray = LegacyShims.SplitAndTrim(LegacyShims.SplitAndTrim(currentCommand.Substring(currentCommand.IndexOf(' ') + 1), "(break)")[0], '#');
      string question = strArray[0];
      Response[] answerChoices = new Response[strArray.Length - 1];
      for (int index = 1; index < strArray.Length; ++index)
        answerChoices[index - 1] = new Response((index - 1).ToString(), strArray[index]);
      Game1.currentLocation.createQuestionDialogue(question, answerChoices, "quickQuestion");
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void DrawOffset(Event @event, string[] args, EventContext context)
    {
      string str;
      string error;
      Vector2 vector2;
      if (!ArgUtility.TryGet(args, 1, out str, out error, name: "string actorName") || !ArgUtility.TryGetVector2(args, 2, out vector2, out error, true, "Vector2 offset"))
      {
        context.LogErrorAndSkip(error);
      }
      else
      {
        bool isOptionalNpc = false;
        int farmerNumber;
        Character character = @event.IsFarmerActorId(str, out farmerNumber) ? (Character) @event.GetFarmerActor(farmerNumber) : (Character) @event.getActorByName(str, out isOptionalNpc);
        if (character == null)
        {
          context.LogErrorAndSkip($"no actor found with name '{str}'", isOptionalNpc);
        }
        else
        {
          character.drawOffset = vector2 * 4f;
          ++@event.CurrentCommand;
        }
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void HideShadow(Event @event, string[] args, EventContext context)
    {
      string name;
      string error;
      bool flag;
      if (!ArgUtility.TryGet(args, 1, out name, out error, name: "string actorName") || !ArgUtility.TryGetBool(args, 2, out flag, out error, "bool hideShadow"))
      {
        context.LogErrorAndSkip(error);
      }
      else
      {
        bool isOptionalNpc;
        NPC actorByName = @event.getActorByName(name, out isOptionalNpc);
        if (actorByName == null)
        {
          context.LogErrorAndSkip($"no actor found with name '{name}'", isOptionalNpc);
        }
        else
        {
          actorByName.HideShadow = flag;
          ++@event.CurrentCommand;
        }
      }
    }

    /// <summary>Animates properties of character "jumps". If any argument is set to "keep", it'll retain the current value.</summary>
    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void AnimateHeight(Event @event, string[] args, EventContext context)
    {
      string str;
      string error;
      string s1;
      string s2;
      string s3;
      if (!ArgUtility.TryGet(args, 1, out str, out error, name: "string actorName") || !ArgUtility.TryGet(args, 2, out s1, out error, name: "string rawHeight") || !ArgUtility.TryGet(args, 3, out s2, out error, name: "string rawGravity") || !ArgUtility.TryGet(args, 4, out s3, out error, name: "string rawVelocity"))
      {
        context.LogErrorAndSkip(error);
      }
      else
      {
        int? nullable1 = new int?();
        float? nullable2 = new float?();
        float? nullable3 = new float?();
        if (s1 != "keep")
        {
          int result;
          if (!int.TryParse(s1, out result))
          {
            context.LogErrorAndSkip("required index 2 must be 'keep' or an integer height");
            return;
          }
          nullable1 = new int?(result);
        }
        if (s2 != "keep")
        {
          float result;
          if (!float.TryParse(s2, out result))
          {
            context.LogErrorAndSkip("required index 3 must be 'keep' or a float gravity value");
            return;
          }
          nullable2 = new float?(result);
        }
        if (s3 != "keep")
        {
          float result;
          if (!float.TryParse(s3, out result))
          {
            context.LogErrorAndSkip("required index 4 must be 'keep' or a float velocity value");
            return;
          }
          nullable3 = new float?(result);
        }
        bool isOptionalNpc = false;
        int farmerNumber;
        Character character = @event.IsFarmerActorId(str, out farmerNumber) ? (Character) @event.GetFarmerActor(farmerNumber) : (Character) @event.getActorByName(str, out isOptionalNpc);
        if (character == null)
        {
          context.LogErrorAndSkip($"no actor found with name '{str}'", isOptionalNpc);
        }
        else
        {
          if (nullable1.HasValue)
            character.yJumpOffset = -nullable1.Value;
          if (nullable2.HasValue)
            character.yJumpGravity = nullable2.Value;
          if (nullable3.HasValue)
            character.yJumpVelocity = nullable3.Value;
          ++@event.CurrentCommand;
        }
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void Jump(Event @event, string[] args, EventContext context)
    {
      string str;
      string error;
      float num;
      bool flag;
      if (!ArgUtility.TryGet(args, 1, out str, out error, name: "string actorName") || !ArgUtility.TryGetOptionalFloat(args, 2, out num, out error, 8f, "float jumpV") || !ArgUtility.TryGetOptionalBool(args, 3, out flag, out error, name: "bool noSound"))
      {
        context.LogErrorAndSkip(error);
      }
      else
      {
        int farmerNumber;
        if (@event.IsFarmerActorId(str, out farmerNumber))
        {
          @event.GetFarmerActor(farmerNumber)?.jump(num);
        }
        else
        {
          bool isOptionalNpc;
          NPC actorByName = @event.getActorByName(str, out isOptionalNpc);
          if (actorByName == null)
          {
            context.LogErrorAndSkip($"no NPC found with name '{str}'", isOptionalNpc);
            return;
          }
          if (flag)
            actorByName.jumpWithoutSound(num);
          else
            actorByName.jump(num);
        }
        ++@event.CurrentCommand;
        @event.Update(context.Location, context.Time);
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void FarmerEat(Event @event, string[] args, EventContext context)
    {
      string str;
      string error;
      if (!ArgUtility.TryGet(args, 1, out str, out error, name: "string itemId"))
      {
        context.LogErrorAndSkip(error);
      }
      else
      {
        Object o = ItemRegistry.Create<Object>("(O)" + str);
        @event.farmer.eatObject(o, true);
        ++@event.CurrentCommand;
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void SpriteText(Event @event, string[] args, EventContext context)
    {
      int num;
      string error;
      string str;
      if (!ArgUtility.TryGetInt(args, 1, out num, out error, "int colorIndex") || !ArgUtility.TryGet(args, 2, out str, out error, name: "string text"))
      {
        context.LogErrorAndSkip(error);
      }
      else
      {
        @event.int_useMeForAnything2 = num;
        @event.float_useMeForAnything += (float) context.Time.ElapsedGameTime.Milliseconds;
        if ((double) @event.float_useMeForAnything > 80.0)
        {
          if (@event.int_useMeForAnything >= str.Length)
          {
            if ((double) @event.float_useMeForAnything >= 2500.0)
            {
              @event.int_useMeForAnything = 0;
              @event.float_useMeForAnything = 0.0f;
              @event.spriteTextToDraw = "";
              ++@event.CurrentCommand;
            }
          }
          else
          {
            ++@event.int_useMeForAnything;
            @event.float_useMeForAnything = 0.0f;
            Game1.playSound("dialogueCharacter");
          }
        }
        @event.spriteTextToDraw = str;
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void IgnoreCollisions(Event @event, string[] args, EventContext context)
    {
      string str;
      string error;
      if (!ArgUtility.TryGet(args, 1, out str, out error, name: "string actorName"))
      {
        context.LogErrorAndSkip(error);
      }
      else
      {
        int farmerNumber;
        if (@event.IsFarmerActorId(str, out farmerNumber))
        {
          Farmer farmerActor = @event.GetFarmerActor(farmerNumber);
          if (farmerActor != null)
            farmerActor.ignoreCollisions = true;
        }
        else
        {
          bool isOptionalNpc;
          NPC actorByName = @event.getActorByName(str, out isOptionalNpc);
          if (actorByName == null)
          {
            context.LogErrorAndSkip($"no NPC found with name '{str}'", isOptionalNpc);
            return;
          }
          actorByName.isCharging = true;
        }
        ++@event.CurrentCommand;
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void ScreenFlash(Event @event, string[] args, EventContext context)
    {
      float num;
      string error;
      if (!ArgUtility.TryGetFloat(args, 1, out num, out error, "float flashAlpha"))
      {
        context.LogErrorAndSkip(error);
      }
      else
      {
        Game1.flashAlpha = num;
        ++@event.CurrentCommand;
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void GrandpaCandles(Event @event, string[] args, EventContext context)
    {
      int candlesFromScore = Utility.getGrandpaCandlesFromScore(Utility.getGrandpaScore());
      Game1.getFarm().grandpaScore.Value = candlesFromScore;
      for (int index = 0; index < candlesFromScore; ++index)
        DelayedAction.playSoundAfterDelay("fireball", 100 * index);
      Game1.getFarm().addGrandpaCandles();
      ++@event.CurrentCommand;
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void GrandpaEvaluation2(Event @event, string[] args, EventContext context)
    {
      switch (Utility.getGrandpaCandlesFromScore(Utility.getGrandpaScore()))
      {
        case 1:
          @event.ReplaceCurrentCommand($"speak Grandpa \"{Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1306")}\"");
          break;
        case 2:
          @event.ReplaceCurrentCommand($"speak Grandpa \"{Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1307")}\"");
          break;
        case 3:
          @event.ReplaceCurrentCommand($"speak Grandpa \"{Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1308")}\"");
          break;
        case 4:
          @event.ReplaceCurrentCommand($"speak Grandpa \"{Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1309")}\"");
          break;
      }
      Game1.player.eventsSeen.Remove("2146991");
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void GrandpaEvaluation(Event @event, string[] args, EventContext context)
    {
      switch (Utility.getGrandpaCandlesFromScore(Utility.getGrandpaScore()))
      {
        case 1:
          @event.ReplaceCurrentCommand($"speak Grandpa \"{Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1315")}\"");
          break;
        case 2:
          @event.ReplaceCurrentCommand($"speak Grandpa \"{Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1316")}\"");
          break;
        case 3:
          @event.ReplaceCurrentCommand($"speak Grandpa \"{Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1317")}\"");
          break;
        case 4:
          @event.ReplaceCurrentCommand($"speak Grandpa \"{Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1318")}\"");
          break;
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void LoadActors(Event @event, string[] args, EventContext context)
    {
      string layerId;
      string error;
      if (!ArgUtility.TryGet(args, 1, out layerId, out error, name: "string layerId"))
      {
        context.LogErrorAndSkip(error);
      }
      else
      {
        Layer layer = @event.temporaryLocation?.map.GetLayer(layerId);
        if (layer == null)
        {
          context.LogErrorAndSkip($"the '{context.Location.NameOrUniqueName}' location doesn't have required map layer {layerId}");
        }
        else
        {
          @event.actors.Clear();
          @event.npcControllers?.Clear();
          Dictionary<int, string> dictionary = new Dictionary<int, string>();
          foreach (KeyValuePair<string, CharacterData> keyValuePair in (IEnumerable<KeyValuePair<string, CharacterData>>) Game1.characterData)
          {
            int vanillaActorIndex = keyValuePair.Value.FestivalVanillaActorIndex;
            if (vanillaActorIndex >= 0 && !dictionary.TryAdd(vanillaActorIndex, keyValuePair.Key))
              Game1.log.Warn($"NPC '{keyValuePair.Key}' has the same festival actor index as '{dictionary[vanillaActorIndex]}' in Data/Characters, so it'll be ignored for festival placement.");
          }
          HashSet<string> stringSet = new HashSet<string>();
          for (int x = 0; x < layer.LayerWidth; ++x)
          {
            for (int y = 0; y < layer.LayerHeight; ++y)
            {
              Tile tile = layer.Tiles[x, y];
              if (tile != null)
              {
                int tileIndex = tile.TileIndex;
                int key = tileIndex / 4;
                int facingDirection = tileIndex % 4;
                string name;
                if (dictionary.TryGetValue(key, out name) && Game1.getCharacterFromName(name) != null && (!(name == "Leo") || Game1.MasterPlayer.mailReceived.Contains("leoMoved")))
                {
                  @event.addActor(name, x, y, facingDirection, @event.temporaryLocation);
                  stringSet.Add(name);
                }
              }
            }
          }
          string data;
          string actualKey;
          if (@event.festivalData != null && @event.TryGetFestivalDataForYear(layerId + "_additionalCharacters", out data, out actualKey))
          {
            foreach (string command in Event.ParseCommands(data, context.Event.farmer))
            {
              string[] array = ArgUtility.SplitBySpaceQuoteAware(command);
              string name;
              Point point;
              int facingDirection;
              if (!ArgUtility.TryGet(array, 0, out name, out error, name: "string actorName") || !ArgUtility.TryGetPoint(array, 1, out point, out error, "Point tile") || !ArgUtility.TryGetDirection(array, 3, out facingDirection, out error, "int direction"))
                context.LogError($"'{actualKey}' festival field has invalid additional character entry '{string.Join(" ", array)}': {error}");
              else if (Game1.getCharacterFromName(name) != null)
              {
                if (!(name == "Leo") || Game1.MasterPlayer.mailReceived.Contains("leoMoved"))
                {
                  @event.addActor(name, point.X, point.Y, facingDirection, @event.temporaryLocation);
                  stringSet.Add(name);
                }
              }
              else
                context.LogError($"'{actualKey}' festival field has invalid additional character entry '{string.Join(" ", array)}': no NPC found with name '{name}'");
            }
          }
          if (layerId == "Set-Up")
          {
            foreach (string name in stringSet)
            {
              NPC characterFromName = Game1.getCharacterFromName(name);
              if (characterFromName.isMarried() && characterFromName.getSpouse() != null && characterFromName.getSpouse().getChildren().Count > 0)
              {
                Farmer parent = Game1.player;
                if (characterFromName.getSpouse() != null)
                  parent = characterFromName.getSpouse();
                List<Child> children = parent.getChildren();
                NPC characterByName = @event.getCharacterByName(name) as NPC;
                for (int index1 = 0; index1 < children.Count; ++index1)
                {
                  Child child1 = children[index1];
                  if (child1.Age >= 3)
                  {
                    Child child2 = new Child(child1.Name, child1.Gender == Gender.Male, child1.darkSkinned.Value, parent);
                    child2.NetFields.CopyFrom(child1.NetFields);
                    child2.Halt();
                    Point[] pointArray;
                    switch (characterByName.FacingDirection)
                    {
                      case 0:
                        pointArray = new Point[4]
                        {
                          new Point(0, 1),
                          new Point(-1, 0),
                          new Point(1, 0),
                          new Point(0, -1)
                        };
                        break;
                      case 1:
                        pointArray = new Point[4]
                        {
                          new Point(-1, 0),
                          new Point(0, 1),
                          new Point(0, -1),
                          new Point(1, 0)
                        };
                        break;
                      case 2:
                        pointArray = new Point[4]
                        {
                          new Point(0, -1),
                          new Point(1, 0),
                          new Point(-1, 0),
                          new Point(0, 1)
                        };
                        break;
                      case 3:
                        pointArray = new Point[4]
                        {
                          new Point(1, 0),
                          new Point(0, -1),
                          new Point(0, 1),
                          new Point(-1, 0)
                        };
                        break;
                      default:
                        pointArray = new Point[4]
                        {
                          new Point(-1, 0),
                          new Point(1, 0),
                          new Point(0, -1),
                          new Point(0, 1)
                        };
                        break;
                    }
                    Point point1 = characterByName.TilePoint;
                    List<Point> pointList = new List<Point>();
                    foreach (Point point2 in pointArray)
                      pointList.Add(new Point(point1.X + point2.X, point1.Y + point2.Y));
                    bool flag = false;
                    for (int index2 = 0; index2 < 5 && !flag; ++index2)
                    {
                      int count = pointList.Count;
                      for (int index3 = 0; index3 < count; ++index3)
                      {
                        Point point3 = pointList[0];
                        pointList.RemoveAt(0);
                        if (IsWalkableTileCheck(point3))
                        {
                          if (HasClearanceCheck(point3))
                          {
                            flag = true;
                            point1 = point3;
                            break;
                          }
                          foreach (Point point4 in pointArray)
                            pointList.Add(new Point(point3.X + point4.X, point3.Y + point4.Y));
                        }
                      }
                    }
                    if (flag)
                    {
                      child2.setTilePosition(point1.X, point1.Y);
                      child2.DefaultPosition = characterByName.DefaultPosition;
                      child2.faceDirection(characterByName.FacingDirection);
                      child2.EventActor = true;
                      child2.lastCrossroad = new Microsoft.Xna.Framework.Rectangle(point1.X * 64 /*0x40*/, point1.Y * 64 /*0x40*/, 64 /*0x40*/, 64 /*0x40*/);
                      child2.squareMovementFacingPreference = -1;
                      child2.walkInSquare(3, 3, 2000);
                      child2.controller = (PathFindController) null;
                      child2.temporaryController = (PathFindController) null;
                      @event.actors.Add((NPC) child2);
                    }
                  }
                }
              }
            }
          }
          ++@event.CurrentCommand;
        }
      }

      bool IsWalkableTileCheck(Point point)
      {
        return @event.temporaryLocation.isTilePassable(new Location(point.X, point.Y), Game1.viewport);
      }

      bool HasClearanceCheck(Point point)
      {
        int num = 1;
        for (int x = point.X - num; x <= point.X + num; ++x)
        {
          for (int y = point.Y - num; y <= point.Y + num; ++y)
          {
            if (@event.temporaryLocation.IsTileBlockedBy(new Vector2((float) x, (float) y)))
              return false;
            foreach (NPC actor in @event.actors)
            {
              if (!(actor is Child))
              {
                Point tilePoint = actor.TilePoint;
                if (tilePoint.X == x && tilePoint.Y == y)
                  return false;
              }
            }
          }
        }
        return true;
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void PlayerControl(Event @event, string[] args, EventContext context)
    {
      string id;
      string error;
      if (!ArgUtility.TryGet(args, 1, out id, out error, name: "string sequenceId"))
      {
        context.LogErrorAndSkip(error);
      }
      else
      {
        if (@event.playerControlSequence)
          return;
        @event.setUpPlayerControlSequence(id);
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void RemoveSprite(Event @event, string[] args, EventContext context)
    {
      Vector2 vector2;
      string error;
      if (!ArgUtility.TryGetVector2(args, 1, out vector2, out error, true, "Vector2 tile"))
      {
        context.LogErrorAndSkip(error);
      }
      else
      {
        Vector2 tilePixel = @event.OffsetPosition(vector2 * 64f);
        Game1.currentLocation.temporarySprites.RemoveWhere<TemporaryAnimatedSprite>((Predicate<TemporaryAnimatedSprite>) (sprite => sprite.position == tilePixel));
        ++@event.CurrentCommand;
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void Viewport(Event @event, string[] args, EventContext context)
    {
      if (ArgUtility.Get(args, 1) == "move")
      {
        Point point;
        string error;
        int z;
        if (!ArgUtility.TryGetPoint(args, 2, out point, out error, "Point direction") || !ArgUtility.TryGetInt(args, 4, out z, out error, "int duration"))
        {
          context.LogErrorAndSkip(error);
          return;
        }
        @event.viewportTarget = new Vector3((float) point.X, (float) point.Y, (float) z);
      }
      else
      {
        Point point = Point.Zero;
        string str1 = (string) null;
        bool flag = false;
        string str2 = (string) null;
        string name;
        string error1;
        if (!int.TryParse(args[1], out int _) && ArgUtility.TryGet(args, 1, out name, out error1, name: "string NPCTarget"))
        {
          point = !(name == "player") ? @event.getActorByName(name).TilePoint : Game1.MasterPlayer.TilePoint;
          if (!ArgUtility.TryGetOptional(args, 2, out str1, out error1, name: "action") || !ArgUtility.TryGetOptionalBool(args, str1 == "clamp" ? 3 : 2, out flag, out error1, name: "shouldFade") || !ArgUtility.TryGetOptional(args, str1 == "clamp" ? 4 : 2, out str2, out error1, name: "option"))
            context.LogErrorAndSkip(error1);
        }
        else
        {
          string error2;
          if (!ArgUtility.TryGetPoint(args, 1, out point, out error2, "position") || !ArgUtility.TryGetOptional(args, 3, out str1, out error2, name: "action") || !ArgUtility.TryGetOptionalBool(args, str1 == "clamp" ? 4 : 3, out flag, out error2, name: "shouldFade") || !ArgUtility.TryGetOptional(args, str1 == "clamp" ? 5 : 4, out str2, out error2, name: "option"))
          {
            context.LogErrorAndSkip(error2);
            return;
          }
        }
        if (@event.aboveMapSprites != null && point.X < 0)
        {
          @event.aboveMapSprites.Clear();
          @event.aboveMapSprites = (TemporaryAnimatedSpriteList) null;
        }
        Game1.viewportFreeze = true;
        int num1 = @event.OffsetTileX(point.X);
        int num2 = @event.OffsetTileY(point.Y);
        if (@event.id == "2146991")
        {
          Point grandpaShrinePosition = Game1.getFarm().GetGrandpaShrinePosition();
          num1 = grandpaShrinePosition.X;
          num2 = grandpaShrinePosition.Y;
        }
        Game1.viewport.X = num1 * 64 /*0x40*/ + 32 /*0x20*/ - Game1.viewport.Width / 2;
        Game1.viewport.Y = num2 * 64 /*0x40*/ + 32 /*0x20*/ - Game1.viewport.Height / 2;
        if (Game1.viewport.X > 0 && Game1.viewport.Width > Game1.currentLocation.Map.DisplayWidth)
          Game1.viewport.X = (Game1.currentLocation.Map.DisplayWidth - Game1.viewport.Width) / 2;
        if (Game1.viewport.Y > 0 && Game1.viewport.Height > Game1.currentLocation.Map.DisplayHeight)
          Game1.viewport.Y = (Game1.currentLocation.Map.DisplayHeight - Game1.viewport.Height) / 2;
        if (str1 == "clamp")
        {
          if (Game1.currentLocation.map.DisplayWidth >= Game1.viewport.Width)
          {
            if (Game1.viewport.X + Game1.viewport.Width > Game1.currentLocation.Map.DisplayWidth)
              Game1.viewport.X = Game1.currentLocation.Map.DisplayWidth - Game1.viewport.Width;
            if (Game1.viewport.X < 0)
              Game1.viewport.X = 0;
          }
          else
            Game1.viewport.X = Game1.currentLocation.Map.DisplayWidth / 2 - Game1.viewport.Width / 2;
          if (Game1.currentLocation.map.DisplayHeight >= Game1.viewport.Height)
          {
            if (Game1.viewport.Y + Game1.viewport.Height > Game1.currentLocation.Map.DisplayHeight)
              Game1.viewport.Y = Game1.currentLocation.Map.DisplayHeight - Game1.viewport.Height;
          }
          else
            Game1.viewport.Y = Game1.currentLocation.Map.DisplayHeight / 2 - Game1.viewport.Height / 2;
          if (Game1.viewport.Y < 0)
            Game1.viewport.Y = 0;
        }
        if (flag)
        {
          Game1.fadeScreenToBlack();
          Game1.fadeToBlackAlpha = 1f;
          Game1.nonWarpFade = true;
        }
        if (str2 == "unfreeze")
          Game1.viewportFreeze = false;
        if (Game1.gameMode == (byte) 2)
          Game1.viewport.X = Game1.currentLocation.Map.DisplayWidth - Game1.viewport.Width;
      }
      ++@event.CurrentCommand;
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void BroadcastEvent(Event @event, string[] args, EventContext context)
    {
      bool use_local_farmer;
      string error;
      if (!ArgUtility.TryGetOptionalBool(args, 1, out use_local_farmer, out error, name: "bool useLocalFarmer"))
      {
        context.LogErrorAndSkip(error);
      }
      else
      {
        if (@event.farmer == Game1.player)
        {
          if (@event.id == "558291" || @event.id == "558292")
            use_local_farmer = true;
          Game1.multiplayer.broadcastEvent(@event, Game1.currentLocation, Game1.player.positionBeforeEvent, use_local_farmer);
        }
        ++@event.CurrentCommand;
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void AddConversationTopic(Event @event, string[] args, EventContext context)
    {
      string key;
      string error;
      int num;
      if (!ArgUtility.TryGet(args, 1, out key, out error, name: "string topicId") || !ArgUtility.TryGetOptionalInt(args, 2, out num, out error, 4, "int daysDuration"))
        context.LogErrorAndSkip(error);
      else if (@event.isMemory)
      {
        ++@event.CurrentCommand;
      }
      else
      {
        Game1.player.activeDialogueEvents.TryAdd(key, num);
        ++@event.CurrentCommand;
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void Dump(Event @event, string[] args, EventContext context)
    {
      string str;
      string error;
      if (!ArgUtility.TryGet(args, 1, out str, out error, name: "string which"))
      {
        context.LogErrorAndSkip(error);
      }
      else
      {
        switch (str)
        {
          case "girls":
            Game1.player.activeDialogueEvents["dumped_Girls"] = 7;
            Game1.player.activeDialogueEvents["secondChance_Girls"] = 14;
            break;
          case "guys":
            Game1.player.activeDialogueEvents["dumped_Guys"] = 7;
            Game1.player.activeDialogueEvents["secondChance_Guys"] = 14;
            break;
          default:
            context.LogErrorAndSkip($"unknown ID '{str}', expected 'girls' or 'guys'");
            return;
        }
        ++@event.CurrentCommand;
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void EventSeen(Event @event, string[] args, EventContext context)
    {
      string str;
      string error;
      bool add;
      if (!ArgUtility.TryGet(args, 1, out str, out error, false, "string eventId") || !ArgUtility.TryGetOptionalBool(args, 2, out add, out error, true, "bool seen"))
      {
        context.LogErrorAndSkip(error);
      }
      else
      {
        Game1.player.eventsSeen.Toggle<string>(str, add);
        if (str == @event.id)
          @event.markEventSeen = false;
        ++@event.CurrentCommand;
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void QuestionAnswered(Event @event, string[] args, EventContext context)
    {
      string str;
      string error;
      bool add;
      if (!ArgUtility.TryGet(args, 1, out str, out error, false, "string questionId") || !ArgUtility.TryGetOptionalBool(args, 2, out add, out error, true, "bool seen"))
      {
        context.LogErrorAndSkip(error);
      }
      else
      {
        Game1.player.dialogueQuestionsAnswered.Toggle<string>(str, add);
        ++@event.CurrentCommand;
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void GainSkill(Event @event, string[] args, EventContext context)
    {
      int skillNumberFromName = Farmer.getSkillNumberFromName(args[1]);
      int int32 = Convert.ToInt32(args[2]);
      if (Game1.player.GetUnmodifiedSkillLevel(skillNumberFromName) < int32)
        Game1.player.setSkillLevel(args[1], int32);
      ++@event.CurrentCommand;
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.EventCommandDelegate" />
    public static void MoveToSoup(Event @event, string[] args, EventContext context)
    {
      if (Game1.year % 2 == 1)
      {
        @event.setUpAdvancedMove(new string[9]
        {
          "",
          "Gus",
          "false",
          "0",
          "-1",
          "5",
          "0",
          "4",
          "1000"
        });
        @event.setUpAdvancedMove(new string[5]
        {
          "",
          "Jodi",
          "false",
          "0",
          "-2"
        });
        @event.setUpAdvancedMove(new string[11]
        {
          "",
          "Clint",
          "false",
          "0",
          "1",
          "-1",
          "0",
          "0",
          "3",
          "-2",
          "0"
        });
        @event.setUpAdvancedMove(new string[5]
        {
          "",
          "Emily",
          "false",
          "3",
          "0"
        });
        @event.setUpAdvancedMove(new string[7]
        {
          "",
          "Pam",
          "false",
          "0",
          "2",
          "7",
          "0"
        });
      }
      else
      {
        @event.setUpAdvancedMove(new string[5]
        {
          "",
          "Pierre",
          "false",
          "3",
          "0"
        });
        @event.setUpAdvancedMove(new string[9]
        {
          "",
          "Pam",
          "false",
          "0",
          "2",
          "-4",
          "0",
          "0",
          "1"
        });
        @event.setUpAdvancedMove(new string[9]
        {
          "",
          "Abigail",
          "false",
          "4",
          "0",
          "0",
          "-3",
          "1",
          "4000"
        });
        @event.setUpAdvancedMove(new string[9]
        {
          "",
          "Alex",
          "false",
          "-5",
          "0",
          "0",
          "-1",
          "3",
          "2000"
        });
        @event.setUpAdvancedMove(new string[5]
        {
          "",
          "Gus",
          "false",
          "0",
          "-1"
        });
      }
      ++@event.CurrentCommand;
    }
  }
}
