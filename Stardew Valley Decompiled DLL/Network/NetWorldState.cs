// Decompiled with JetBrains decompiler
// Type: StardewValley.Network.NetWorldState
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Netcode;
using StardewValley.Buildings;
using StardewValley.GameData;
using StardewValley.GameData.LocationContexts;
using StardewValley.Locations;
using StardewValley.Quests;
using StardewValley.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

#nullable disable
namespace StardewValley.Network;

public class NetWorldState : INetObject<NetFields>
{
  protected readonly NetLong uniqueIDForThisGame;
  protected readonly NetEnum<ServerPrivacy> serverPrivacy;
  protected readonly NetInt whichFarm;
  protected readonly NetString whichModFarm;
  protected string _oldModFarmType;
  public readonly NetEnum<Game1.MineChestType> shuffleMineChests;
  public readonly NetInt minesDifficulty;
  public readonly NetInt skullCavesDifficulty;
  public readonly NetInt highestPlayerLimit;
  public readonly NetInt currentPlayerLimit;
  protected readonly NetInt year;
  protected readonly NetEnum<Season> season;
  protected readonly NetInt dayOfMonth;
  protected readonly NetInt timeOfDay;
  protected readonly NetInt daysPlayed;
  public readonly NetInt visitsUntilY1Guarantee;
  protected readonly NetBool isPaused;
  protected readonly NetBool isTimePaused;
  protected readonly NetStringDictionary<StardewValley.Network.LocationWeather, NetRef<StardewValley.Network.LocationWeather>> locationWeather;
  protected readonly NetBool isRaining;
  protected readonly NetBool isSnowing;
  protected readonly NetBool isLightning;
  protected readonly NetBool isDebrisWeather;
  public readonly NetString weatherForTomorrow;
  protected readonly NetBundles bundles;
  protected readonly NetIntDictionary<bool, NetBool> bundleRewards;
  protected readonly NetStringDictionary<string, NetString> netBundleData;
  protected Dictionary<string, string> _bundleData;
  protected bool _bundleDataDirty;
  public readonly NetArray<bool, NetBool> raccoonBundles;
  public readonly NetInt seasonOfCurrentRacconBundle;
  public readonly NetBool parrotPlatformsUnlocked;
  public readonly NetBool goblinRemoved;
  public readonly NetBool submarineLocked;
  public readonly NetInt lowestMineLevel;
  public readonly NetInt lowestMineLevelForOrder;
  protected readonly NetVector2Dictionary<string, NetString> museumPieces;
  protected readonly NetIntDelta lostBooksFound;
  protected readonly NetIntDelta goldenWalnuts;
  protected readonly NetIntDelta goldenWalnutsFound;
  protected readonly NetBool goldenCoconutCracked;
  protected readonly NetStringHashSet foundBuriedNuts;
  protected readonly NetIntDelta miniShippingBinsObtained;
  protected readonly NetIntDelta perfectionWaivers;
  protected readonly NetIntDelta timesFedRaccoons;
  protected readonly NetIntDelta treasureTotemsUsed;
  public NetLongDictionary<Farmer, NetRef<Farmer>> farmhandData;
  /// <summary>The backing field for <see cref="P:StardewValley.Network.NetWorldState.LocationsWithBuildings" />.</summary>
  public readonly NetStringHashSet locationsWithBuildings;
  public NetStringDictionary<BuilderData, NetRef<BuilderData>> builders;
  public NetStringHashSet activePassiveFestivals;
  protected readonly NetStringHashSet worldStateIDs;
  protected readonly NetStringHashSet islandVisitors;
  protected readonly NetStringHashSet checkedGarbage;
  public readonly NetRef<StardewValley.Object> dishOfTheDay;
  private readonly NetBool activatedGoldenParrot;
  private readonly NetInt daysPlayedWhenLastRaccoonBundleWasFinished;
  public readonly NetBool canDriveYourselfToday;
  public readonly NetBool goldenClocksTurnedOff;
  /// <summary>The backing field for <see cref="P:StardewValley.Network.NetWorldState.QuestOfTheDay" />.</summary>
  protected readonly NetRef<Quest> netQuestOfTheDay;

  public NetFields NetFields { get; }

  public ServerPrivacy ServerPrivacy
  {
    get => this.serverPrivacy.Value;
    set => this.serverPrivacy.Value = value;
  }

  public Game1.MineChestType ShuffleMineChests
  {
    get => this.shuffleMineChests.Value;
    set => this.shuffleMineChests.Value = value;
  }

  public int MinesDifficulty
  {
    get => this.minesDifficulty.Value;
    set => this.minesDifficulty.Value = value;
  }

  public int SkullCavesDifficulty
  {
    get => this.skullCavesDifficulty.Value;
    set => this.skullCavesDifficulty.Value = value;
  }

  public int HighestPlayerLimit
  {
    get => this.highestPlayerLimit.Value;
    set => this.highestPlayerLimit.Value = value;
  }

  public int CurrentPlayerLimit
  {
    get => this.currentPlayerLimit.Value;
    set => this.currentPlayerLimit.Value = value;
  }

  public WorldDate Date => WorldDate.Now();

  public int VisitsUntilY1Guarantee
  {
    get => this.visitsUntilY1Guarantee.Value;
    set => this.visitsUntilY1Guarantee.Value = value;
  }

  public bool IsPaused
  {
    get => this.isPaused.Value;
    set => this.isPaused.Value = value;
  }

  public bool IsTimePaused
  {
    get => this.isTimePaused.Value;
    set => this.isTimePaused.Value = value;
  }

  public NetStringDictionary<StardewValley.Network.LocationWeather, NetRef<StardewValley.Network.LocationWeather>> LocationWeather
  {
    get => this.locationWeather;
  }

  public string WeatherForTomorrow
  {
    get => this.weatherForTomorrow.Value;
    set => this.weatherForTomorrow.Value = value;
  }

  public NetBundles Bundles => this.bundles;

  public NetIntDictionary<bool, NetBool> BundleRewards => this.bundleRewards;

  public Dictionary<string, string> BundleData
  {
    get
    {
      if (this.netBundleData.Length == 0)
        this.SetBundleData(DataLoader.Bundles(Game1.content));
      if (this._bundleDataDirty)
      {
        this._bundleDataDirty = false;
        this._bundleData = new Dictionary<string, string>();
        foreach (string key in this.netBundleData.Keys)
          this._bundleData[key] = this.netBundleData[key];
        this.UpdateBundleDisplayNames();
      }
      return this._bundleData;
    }
  }

  public bool ParrotPlatformsUnlocked
  {
    get => this.parrotPlatformsUnlocked.Value;
    set => this.parrotPlatformsUnlocked.Value = value;
  }

  public bool IsGoblinRemoved
  {
    get => this.goblinRemoved.Value;
    set => this.goblinRemoved.Value = value;
  }

  public bool IsSubmarineLocked
  {
    get => this.submarineLocked.Value;
    set => this.submarineLocked.Value = value;
  }

  public int LowestMineLevel
  {
    get => this.lowestMineLevel.Value;
    set => this.lowestMineLevel.Value = value;
  }

  public int LowestMineLevelForOrder
  {
    get => this.lowestMineLevelForOrder.Value;
    set => this.lowestMineLevelForOrder.Value = value;
  }

  public NetVector2Dictionary<string, NetString> MuseumPieces => this.museumPieces;

  public int LostBooksFound
  {
    get => this.lostBooksFound.Value;
    set => this.lostBooksFound.Value = value;
  }

  public int GoldenWalnuts
  {
    get => this.goldenWalnuts.Value;
    set => this.goldenWalnuts.Value = value;
  }

  public int GoldenWalnutsFound
  {
    get => this.goldenWalnutsFound.Value;
    set => this.goldenWalnutsFound.Value = value;
  }

  public bool GoldenCoconutCracked
  {
    get => this.goldenCoconutCracked.Value;
    set => this.goldenCoconutCracked.Value = value;
  }

  public bool ActivatedGoldenParrot
  {
    get => this.activatedGoldenParrot.Value;
    set => this.activatedGoldenParrot.Value = value;
  }

  public ISet<string> FoundBuriedNuts => (ISet<string>) this.foundBuriedNuts;

  public int MiniShippingBinsObtained
  {
    get => this.miniShippingBinsObtained.Value;
    set => this.miniShippingBinsObtained.Value = value;
  }

  public int PerfectionWaivers
  {
    get => this.perfectionWaivers.Value;
    set => this.perfectionWaivers.Value = value;
  }

  public int TimesFedRaccoons
  {
    get => this.timesFedRaccoons.Value;
    set => this.timesFedRaccoons.Value = value;
  }

  public int TreasureTotemsUsed
  {
    get => this.treasureTotemsUsed.Value;
    set => this.treasureTotemsUsed.Value = value;
  }

  public int SeasonOfCurrentRacconBundle
  {
    get => this.seasonOfCurrentRacconBundle.Value;
    set => this.seasonOfCurrentRacconBundle.Value = value;
  }

  public int DaysPlayedWhenLastRaccoonBundleWasFinished
  {
    get => this.daysPlayedWhenLastRaccoonBundleWasFinished.Value;
    set => this.daysPlayedWhenLastRaccoonBundleWasFinished.Value = value;
  }

  /// <summary>The unique names for locations which contain at least one constructed building.</summary>
  public ISet<string> LocationsWithBuildings => (ISet<string>) this.locationsWithBuildings;

  public NetStringDictionary<BuilderData, NetRef<BuilderData>> Builders => this.builders;

  public ISet<string> ActivePassiveFestivals => (ISet<string>) this.activePassiveFestivals;

  public ISet<string> IslandVisitors => (ISet<string>) this.islandVisitors;

  public ISet<string> CheckedGarbage => (ISet<string>) this.checkedGarbage;

  public StardewValley.Object DishOfTheDay
  {
    get => this.dishOfTheDay.Value;
    set => this.dishOfTheDay.Value = value;
  }

  /// <summary>The daily quest that's shown on the billboard, if any.</summary>
  /// <remarks>This is synchronized from the host in multiplayer. See <see cref="M:StardewValley.Network.NetWorldState.SetQuestOfTheDay(StardewValley.Quests.Quest)" /> to set it.</remarks>
  public Quest QuestOfTheDay { get; private set; }

  public NetWorldState()
  {
    NetBool netBool = new NetBool();
    netBool.InterpolationWait = false;
    this.isTimePaused = netBool;
    this.locationWeather = new NetStringDictionary<StardewValley.Network.LocationWeather, NetRef<StardewValley.Network.LocationWeather>>();
    this.isRaining = new NetBool();
    this.isSnowing = new NetBool();
    this.isLightning = new NetBool();
    this.isDebrisWeather = new NetBool();
    this.weatherForTomorrow = new NetString();
    this.bundles = new NetBundles();
    this.bundleRewards = new NetIntDictionary<bool, NetBool>();
    this.netBundleData = new NetStringDictionary<string, NetString>();
    this._bundleDataDirty = true;
    this.raccoonBundles = new NetArray<bool, NetBool>(2);
    this.seasonOfCurrentRacconBundle = new NetInt(-1);
    this.parrotPlatformsUnlocked = new NetBool();
    this.goblinRemoved = new NetBool();
    this.submarineLocked = new NetBool();
    this.lowestMineLevel = new NetInt();
    this.lowestMineLevelForOrder = new NetInt(-1);
    this.museumPieces = new NetVector2Dictionary<string, NetString>();
    this.lostBooksFound = new NetIntDelta()
    {
      Minimum = new int?(0),
      Maximum = new int?(21)
    };
    this.goldenWalnuts = new NetIntDelta()
    {
      Minimum = new int?(0)
    };
    this.goldenWalnutsFound = new NetIntDelta()
    {
      Minimum = new int?(0)
    };
    this.goldenCoconutCracked = new NetBool();
    this.foundBuriedNuts = new NetStringHashSet();
    this.miniShippingBinsObtained = new NetIntDelta()
    {
      Minimum = new int?(0)
    };
    this.perfectionWaivers = new NetIntDelta()
    {
      Minimum = new int?(0)
    };
    this.timesFedRaccoons = new NetIntDelta()
    {
      Minimum = new int?(0)
    };
    this.treasureTotemsUsed = new NetIntDelta()
    {
      Minimum = new int?(0)
    };
    this.farmhandData = new NetLongDictionary<Farmer, NetRef<Farmer>>();
    this.locationsWithBuildings = new NetStringHashSet();
    this.builders = new NetStringDictionary<BuilderData, NetRef<BuilderData>>();
    this.activePassiveFestivals = new NetStringHashSet();
    this.worldStateIDs = new NetStringHashSet();
    this.islandVisitors = new NetStringHashSet();
    this.checkedGarbage = new NetStringHashSet();
    this.dishOfTheDay = new NetRef<StardewValley.Object>();
    this.activatedGoldenParrot = new NetBool();
    this.daysPlayedWhenLastRaccoonBundleWasFinished = new NetInt();
    this.canDriveYourselfToday = new NetBool();
    this.goldenClocksTurnedOff = new NetBool();
    this.netQuestOfTheDay = new NetRef<Quest>();
    this.NetFields = new NetFields(nameof (NetWorldState));
    // ISSUE: explicit constructor call
    base.\u002Ector();
    this.RegisterSpecialCurrencies();
    this.NetFields.SetOwner((INetObject<NetFields>) this).AddField((INetSerializable) this.uniqueIDForThisGame, nameof (uniqueIDForThisGame)).AddField((INetSerializable) this.serverPrivacy, nameof (serverPrivacy)).AddField((INetSerializable) this.whichFarm, nameof (whichFarm)).AddField((INetSerializable) this.whichModFarm, nameof (whichModFarm)).AddField((INetSerializable) this.shuffleMineChests, nameof (shuffleMineChests)).AddField((INetSerializable) this.minesDifficulty, nameof (minesDifficulty)).AddField((INetSerializable) this.skullCavesDifficulty, nameof (skullCavesDifficulty)).AddField((INetSerializable) this.highestPlayerLimit, nameof (highestPlayerLimit)).AddField((INetSerializable) this.currentPlayerLimit, nameof (currentPlayerLimit)).AddField((INetSerializable) this.year, nameof (year)).AddField((INetSerializable) this.season, nameof (season)).AddField((INetSerializable) this.dayOfMonth, nameof (dayOfMonth)).AddField((INetSerializable) this.timeOfDay, nameof (timeOfDay)).AddField((INetSerializable) this.daysPlayed, nameof (daysPlayed)).AddField((INetSerializable) this.visitsUntilY1Guarantee, nameof (visitsUntilY1Guarantee)).AddField((INetSerializable) this.isPaused, nameof (isPaused)).AddField((INetSerializable) this.isTimePaused, nameof (isTimePaused)).AddField((INetSerializable) this.locationWeather, nameof (locationWeather)).AddField((INetSerializable) this.isRaining, nameof (isRaining)).AddField((INetSerializable) this.isSnowing, nameof (isSnowing)).AddField((INetSerializable) this.isLightning, nameof (isLightning)).AddField((INetSerializable) this.isDebrisWeather, nameof (isDebrisWeather)).AddField((INetSerializable) this.weatherForTomorrow, nameof (weatherForTomorrow)).AddField((INetSerializable) this.bundles, nameof (bundles)).AddField((INetSerializable) this.bundleRewards, nameof (bundleRewards)).AddField((INetSerializable) this.netBundleData, nameof (netBundleData)).AddField((INetSerializable) this.raccoonBundles, nameof (raccoonBundles)).AddField((INetSerializable) this.seasonOfCurrentRacconBundle, nameof (seasonOfCurrentRacconBundle)).AddField((INetSerializable) this.parrotPlatformsUnlocked, nameof (parrotPlatformsUnlocked)).AddField((INetSerializable) this.goblinRemoved, nameof (goblinRemoved)).AddField((INetSerializable) this.submarineLocked, nameof (submarineLocked)).AddField((INetSerializable) this.lowestMineLevel, nameof (lowestMineLevel)).AddField((INetSerializable) this.lowestMineLevelForOrder, nameof (lowestMineLevelForOrder)).AddField((INetSerializable) this.museumPieces, nameof (museumPieces)).AddField((INetSerializable) this.lostBooksFound, nameof (lostBooksFound)).AddField((INetSerializable) this.goldenWalnuts, nameof (goldenWalnuts)).AddField((INetSerializable) this.goldenWalnutsFound, nameof (goldenWalnutsFound)).AddField((INetSerializable) this.goldenCoconutCracked, nameof (goldenCoconutCracked)).AddField((INetSerializable) this.foundBuriedNuts, nameof (foundBuriedNuts)).AddField((INetSerializable) this.miniShippingBinsObtained, nameof (miniShippingBinsObtained)).AddField((INetSerializable) this.perfectionWaivers, nameof (perfectionWaivers)).AddField((INetSerializable) this.timesFedRaccoons, nameof (timesFedRaccoons)).AddField((INetSerializable) this.treasureTotemsUsed, nameof (treasureTotemsUsed)).AddField((INetSerializable) this.farmhandData, nameof (farmhandData)).AddField((INetSerializable) this.locationsWithBuildings, nameof (locationsWithBuildings)).AddField((INetSerializable) this.builders, nameof (builders)).AddField((INetSerializable) this.activePassiveFestivals, nameof (activePassiveFestivals)).AddField((INetSerializable) this.worldStateIDs, nameof (worldStateIDs)).AddField((INetSerializable) this.islandVisitors, nameof (islandVisitors)).AddField((INetSerializable) this.checkedGarbage, nameof (checkedGarbage)).AddField((INetSerializable) this.dishOfTheDay, nameof (dishOfTheDay)).AddField((INetSerializable) this.netQuestOfTheDay, nameof (netQuestOfTheDay)).AddField((INetSerializable) this.activatedGoldenParrot, nameof (activatedGoldenParrot)).AddField((INetSerializable) this.daysPlayedWhenLastRaccoonBundleWasFinished, nameof (daysPlayedWhenLastRaccoonBundleWasFinished)).AddField((INetSerializable) this.canDriveYourselfToday, nameof (canDriveYourselfToday)).AddField((INetSerializable) this.goldenClocksTurnedOff, nameof (goldenClocksTurnedOff));
    this.netBundleData.OnConflictResolve += (NetDictionary<string, string, NetString, SerializableDictionary<string, string>, NetStringDictionary<string, NetString>>.ConflictResolveEvent) ((key, rejected, accepted) => this._bundleDataDirty = true);
    this.netBundleData.OnValueAdded += (NetDictionary<string, string, NetString, SerializableDictionary<string, string>, NetStringDictionary<string, NetString>>.ContentsChangeEvent) ((key, value) => this._bundleDataDirty = true);
    this.netBundleData.OnValueRemoved += (NetDictionary<string, string, NetString, SerializableDictionary<string, string>, NetStringDictionary<string, NetString>>.ContentsChangeEvent) ((key, value) => this._bundleDataDirty = true);
    this.netQuestOfTheDay.fieldChangeVisibleEvent += (FieldChange<NetRef<Quest>, Quest>) ((field, oldQuest, newQuest) =>
    {
      if (newQuest == null)
      {
        this.QuestOfTheDay = (Quest) null;
      }
      else
      {
        using (MemoryStream memoryStream = new MemoryStream())
        {
          using (BinaryWriter writer = new BinaryWriter((Stream) memoryStream))
          {
            new NetRef<Quest>() { Value = newQuest }.WriteFull(writer);
            memoryStream.Seek(0L, SeekOrigin.Begin);
            using (BinaryReader reader = new BinaryReader((Stream) memoryStream))
            {
              NetRef<Quest> netRef = new NetRef<Quest>();
              netRef.ReadFull(reader, new NetVersion());
              this.QuestOfTheDay = netRef.Value;
            }
          }
        }
      }
    });
  }

  public virtual void RegisterSpecialCurrencies()
  {
    if (Game1.specialCurrencyDisplay == null)
      return;
    Game1.specialCurrencyDisplay.Register("walnuts", this.goldenWalnuts);
    Game1.specialCurrencyDisplay.Register("qiGems", Game1.player.netQiGems);
  }

  /// <summary>Sets the quest of the day and synchronizes it to other players. In multiplayer, this can only be called on the host instance.</summary>
  /// <param name="quest">The daily quest to set.</param>
  public void SetQuestOfTheDay(Quest quest)
  {
    if (!Game1.IsMasterGame)
    {
      Game1.log.Warn("Can't set the daily quest from a farmhand instance.");
      Game1.log.Verbose(new StackTraceHelper().ToString());
    }
    else
      this.netQuestOfTheDay.Value = quest;
  }

  public void SetBundleData(Dictionary<string, string> data)
  {
    this._bundleDataDirty = true;
    this.netBundleData.CopyFrom((IEnumerable<KeyValuePair<string, string>>) data);
    foreach (KeyValuePair<string, string> pair in this.netBundleData.Pairs)
    {
      string key = pair.Key;
      string str = pair.Value;
      int int32 = Convert.ToInt32(key.Split('/')[1]);
      int length = ArgUtility.SplitBySpace(str.Split('/')[2]).Length;
      if (!this.bundles.ContainsKey(int32))
        this.bundles.Add(int32, new NetArray<bool, NetBool>(length));
      else if (this.bundles[int32].Length < length)
      {
        NetArray<bool, NetBool> field = new NetArray<bool, NetBool>(length);
        for (int index = 0; index < Math.Min(this.bundles[int32].Length, length); ++index)
          field[index] = this.bundles[int32][index];
        this.bundles.Remove(int32);
        this.bundles.Add(int32, field);
      }
      if (!this.bundleRewards.ContainsKey(int32))
        this.bundleRewards.Add(int32, new NetBool(false));
    }
  }

  public static bool checkAnywhereForWorldStateID(string id)
  {
    return Game1.worldStateIDs.Contains(id) || Game1.netWorldState.Value.hasWorldStateID(id);
  }

  public static void addWorldStateIDEverywhere(string id)
  {
    Game1.netWorldState.Value.addWorldStateID(id);
    if (Game1.worldStateIDs.Contains(id))
      return;
    Game1.worldStateIDs.Add(id);
  }

  public virtual void UpdateBundleDisplayNames()
  {
    List<string> stringList = new List<string>((IEnumerable<string>) this._bundleData.Keys);
    Dictionary<string, string> dictionary = DataLoader.Bundles(Game1.content);
    foreach (string key in stringList)
    {
      string[] array1 = this._bundleData[key].Split('/');
      string str1 = array1[0];
      if (!ArgUtility.HasIndex<string>(array1, 6))
        Array.Resize<string>(ref array1, 7);
      string str2 = (string) null;
      foreach (string str3 in dictionary.Values)
      {
        string[] array2 = str3.Split('/');
        if (ArgUtility.Get(array2, 0) == str1)
        {
          str2 = ArgUtility.Get(array2, 6);
          break;
        }
      }
      if (str2 == null)
        str2 = Game1.content.LoadStringReturnNullIfNotFound("Strings\\BundleNames:" + str1);
      array1[6] = str2 ?? str1;
      this._bundleData[key] = string.Join("/", array1);
    }
  }

  public bool hasWorldStateID(string id) => this.worldStateIDs.Contains(id);

  public void addWorldStateID(string id) => this.worldStateIDs.Add(id);

  public void removeWorldStateID(string id) => this.worldStateIDs.Remove(id);

  public void SaveFarmhand(NetFarmerRoot farmhand)
  {
    NetRef<Farmer> netref;
    if (Game1.netWorldState.Value.farmhandData.FieldDict.TryGetValue(farmhand.Value.UniqueMultiplayerID, out netref))
      farmhand.CloneInto(netref);
    this.ResetFarmhandState(farmhand.Value);
  }

  public void ResetFarmhandState(Farmer farmhand)
  {
    farmhand.farmName.Value = Game1.MasterPlayer.farmName.Value;
    if (this.TryAssignFarmhandHome(farmhand))
    {
      FarmHouse homeOfFarmer = Utility.getHomeOfFarmer(farmhand);
      if (farmhand.lastSleepLocation.Value == null || farmhand.lastSleepLocation.Value == homeOfFarmer.NameOrUniqueName)
      {
        farmhand.currentLocation = (GameLocation) homeOfFarmer;
        farmhand.Position = Utility.PointToVector2(homeOfFarmer.GetPlayerBedSpot()) * 64f;
      }
    }
    else
    {
      farmhand.userID.Value = "";
      farmhand.homeLocation.Value = (string) null;
      Game1.otherFarmers.Remove(farmhand.UniqueMultiplayerID);
    }
    farmhand.resetState();
  }

  /// <summary>Assign a farmhand to a cabin if their current home is invalid.</summary>
  /// <param name="farmhand">The farmhand instance.</param>
  /// <returns>Returns whether the farmhand has a valid home (either already assigned or just assigned).</returns>
  public bool TryAssignFarmhandHome(Farmer farmhand)
  {
    if (farmhand.IsMainPlayer || Game1.getLocationFromName(farmhand.homeLocation.Value) is Cabin)
      return true;
    if (farmhand.currentLocation is Cabin currentLocation && currentLocation.CanAssignTo(farmhand))
    {
      currentLocation.AssignFarmhand(farmhand);
      return true;
    }
    if (Game1.getLocationFromName(farmhand.lastSleepLocation.Value) is Cabin locationFromName && locationFromName.CanAssignTo(farmhand))
    {
      locationFromName.AssignFarmhand(farmhand);
      return true;
    }
    bool found = false;
    Utility.ForEachBuilding((Func<Building, bool>) (building =>
    {
      if (!(building.GetIndoors() is Cabin indoors2) || !indoors2.CanAssignTo(farmhand))
        return true;
      indoors2.AssignFarmhand(farmhand);
      found = true;
      return false;
    }));
    return found;
  }

  public void UpdateFromGame1()
  {
    this.year.Value = Game1.year;
    this.season.Value = Game1.season;
    this.dayOfMonth.Value = Game1.dayOfMonth;
    this.timeOfDay.Value = Game1.timeOfDay;
    StardewValley.Network.LocationWeather weatherForLocation = this.GetWeatherForLocation("Default");
    weatherForLocation.WeatherForTomorrow = Game1.weatherForTomorrow;
    weatherForLocation.IsRaining = Game1.isRaining;
    weatherForLocation.IsSnowing = Game1.isSnowing;
    weatherForLocation.IsDebrisWeather = Game1.isDebrisWeather;
    weatherForLocation.IsGreenRain = Game1.isGreenRain;
    this.isDebrisWeather.Value = Game1.isDebrisWeather;
    this.whichFarm.Value = Game1.whichFarm;
    this.weatherForTomorrow.Value = Game1.weatherForTomorrow;
    this.daysPlayed.Value = (int) Game1.stats.DaysPlayed;
    this.uniqueIDForThisGame.Value = (long) Game1.uniqueIDForThisGame;
    if (Game1.whichFarm != 7 || Game1.whichModFarm == null)
      this.whichModFarm.Value = (string) null;
    else
      this.whichModFarm.Value = Game1.whichModFarm.Id;
    this.currentPlayerLimit.Value = Game1.multiplayer.playerLimit;
    this.highestPlayerLimit.Value = Math.Max(this.highestPlayerLimit.Value, Game1.multiplayer.playerLimit);
    this.worldStateIDs.Clear();
    this.worldStateIDs.AddRange<string>((IEnumerable<string>) Game1.worldStateIDs);
  }

  public StardewValley.Network.LocationWeather GetWeatherForLocation(string locationContextId)
  {
    StardewValley.Network.LocationWeather weatherForLocation;
    if (!this.locationWeather.TryGetValue(locationContextId, out weatherForLocation))
    {
      this.locationWeather[locationContextId] = weatherForLocation = new StardewValley.Network.LocationWeather();
      LocationContextData data;
      if (Game1.locationContextData.TryGetValue(locationContextId, out data))
      {
        weatherForLocation.UpdateDailyWeather(locationContextId, data, Game1.random);
        weatherForLocation.UpdateDailyWeather(locationContextId, data, Game1.random);
      }
    }
    return weatherForLocation;
  }

  public void WriteToGame1(bool onLoad = false)
  {
    if (Game1.farmEvent != null)
      return;
    StardewValley.Network.LocationWeather weatherForLocation = this.GetWeatherForLocation("Default");
    Game1.weatherForTomorrow = weatherForLocation.WeatherForTomorrow;
    Game1.isRaining = weatherForLocation.IsRaining;
    Game1.isSnowing = weatherForLocation.IsSnowing;
    Game1.isLightning = weatherForLocation.IsLightning;
    Game1.isDebrisWeather = weatherForLocation.IsDebrisWeather;
    Game1.isGreenRain = weatherForLocation.IsGreenRain;
    Game1.weatherForTomorrow = this.weatherForTomorrow.Value;
    Game1.worldStateIDs = new HashSet<string>((IEnumerable<string>) this.worldStateIDs);
    if (!Game1.IsServer)
    {
      bool flag = Game1.season != this.season.Value;
      Game1.year = this.year.Value;
      Game1.season = this.season.Value;
      Game1.dayOfMonth = this.dayOfMonth.Value;
      Game1.timeOfDay = this.timeOfDay.Value;
      Game1.whichFarm = this.whichFarm.Value;
      if (Game1.whichFarm != 7)
        Game1.whichModFarm = (ModFarmType) null;
      else if (this._oldModFarmType != this.whichModFarm.Value)
      {
        this._oldModFarmType = this.whichModFarm.Value;
        Game1.whichModFarm = (ModFarmType) null;
        List<ModFarmType> modFarmTypeList = DataLoader.AdditionalFarms(Game1.content);
        if (modFarmTypeList != null)
        {
          foreach (ModFarmType modFarmType in modFarmTypeList)
          {
            if (modFarmType.Id == this.whichModFarm.Value)
            {
              Game1.whichModFarm = modFarmType;
              break;
            }
          }
        }
        if (Game1.whichModFarm == null)
          throw new Exception(this.whichModFarm.Value + " is not a valid farm type.");
      }
      Game1.stats.DaysPlayed = (uint) this.daysPlayed.Value;
      Game1.uniqueIDForThisGame = (ulong) this.uniqueIDForThisGame.Value;
      if (flag)
        Game1.setGraphicsForSeason(onLoad);
    }
    Game1.updateWeatherIcon();
    if (!this.IsGoblinRemoved)
      return;
    Game1.player.removeQuest("27");
  }

  /// <summary>Get cached info about the building being constructed by an NPC.</summary>
  /// <param name="builderName">The internal name of the NPC constructing buildings.</param>
  public BuilderData GetBuilderData(string builderName)
  {
    BuilderData builderData;
    return !this.builders.TryGetValue(builderName, out builderData) ? (BuilderData) null : builderData;
  }

  /// <summary>Mark a building as being under construction.</summary>
  /// <param name="builderName">The internal name of the NPC constructing it.</param>
  /// <param name="building">The building being constructed.</param>
  public void MarkUnderConstruction(string builderName, Building building)
  {
    int val1 = building.daysOfConstructionLeft.Value;
    int val2 = building.daysUntilUpgrade.Value;
    int daysUntilBuilt = Math.Max(val1, val2);
    if (daysUntilBuilt == 0)
      return;
    this.builders[builderName] = new BuilderData(building.buildingType.Value, daysUntilBuilt, building.parentLocationName.Value, new Point(building.tileX.Value, building.tileY.Value), val2 > 0 && val1 <= 0);
  }

  /// <summary>Remove constructed buildings from the cached list of buildings under construction.</summary>
  public void UpdateUnderConstruction()
  {
    foreach (KeyValuePair<string, BuilderData> keyValuePair in this.builders.Pairs.ToArray<KeyValuePair<string, BuilderData>>())
    {
      string key = keyValuePair.Key;
      BuilderData builderData = keyValuePair.Value;
      GameLocation locationFromName = Game1.getLocationFromName(builderData.buildingLocation.Value);
      if (locationFromName == null)
      {
        this.builders.Remove(key);
      }
      else
      {
        Building buildingAt = locationFromName.getBuildingAt(Utility.PointToVector2(builderData.buildingTile.Value));
        if (buildingAt == null || !buildingAt.isUnderConstruction(false))
          this.builders.Remove(key);
      }
    }
  }

  /// <summary>Add or remove the location from the <see cref="P:StardewValley.Network.NetWorldState.LocationsWithBuildings" /> cache.</summary>
  /// <param name="location">The location to update.</param>
  public void UpdateBuildingCache(GameLocation location)
  {
    string nameOrUniqueName = location.NameOrUniqueName;
    if (location.buildings.Count > 0)
      this.locationsWithBuildings.Add(nameOrUniqueName);
    else
      this.locationsWithBuildings.Remove(nameOrUniqueName);
  }
}
