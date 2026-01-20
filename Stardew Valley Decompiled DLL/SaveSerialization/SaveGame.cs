// Decompiled with JetBrains decompiler
// Type: StardewValley.SaveGame
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Ionic.Zlib;
using Microsoft.Xna.Framework;
using Netcode;
using StardewValley.Buildings;
using StardewValley.Characters;
using StardewValley.GameData;
using StardewValley.GameData.Characters;
using StardewValley.GameData.Locations;
using StardewValley.Inventories;
using StardewValley.Locations;
using StardewValley.Logging;
using StardewValley.Minigames;
using StardewValley.Network;
using StardewValley.Objects;
using StardewValley.Pathfinding;
using StardewValley.Quests;
using StardewValley.SaveMigrations;
using StardewValley.SaveSerialization;
using StardewValley.SpecialOrders;
using StardewValley.TerrainFeatures;
using StardewValley.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley;

public class SaveGame
{
  /// <summary>The filename suffix for a save file that's currently being written.</summary>
  public const string TempNameSuffix = "_STARDEWVALLEYSAVETMP";
  /// <summary>The filename suffix for a previous save file.</summary>
  public const string BackupNameSuffix = "_old";
  /// <summary>Whether the current platform supports save backup files with the <see cref="F:StardewValley.SaveGame.TempNameSuffix" /> and <see cref="F:StardewValley.SaveGame.BackupNameSuffix" /> suffixes.</summary>
  public const bool PlatformSupportsBackups = true;
  [InstancedStatic]
  public static bool IsProcessing;
  [InstancedStatic]
  public static bool CancelToTitle;
  public Farmer player;
  public List<Farmer> farmhands;
  public List<GameLocation> locations;
  public string currentSeason;
  public string samBandName;
  public string elliottBookName;
  /// <summary>Obsolete. This is only kept to preserve data from old save files.</summary>
  [XmlArray("mailbox")]
  public List<string> obsolete_mailbox;
  public HashSet<string> broadcastedMail;
  public HashSet<string> constructedBuildings;
  public HashSet<string> worldStateIDs;
  public int lostBooksFound = -1;
  public int goldenWalnuts = -1;
  public int goldenWalnutsFound;
  public int miniShippingBinsObtained;
  public bool mineShrineActivated;
  public bool skullShrineActivated;
  public bool goldenCoconutCracked;
  public bool parrotPlatformsUnlocked;
  public bool farmPerfect;
  public List<string> foundBuriedNuts = new List<string>();
  public List<string> checkedGarbage = new List<string>();
  public int visitsUntilY1Guarantee = -1;
  public Game1.MineChestType shuffleMineChests;
  public int dayOfMonth;
  public int year;
  public int? countdownToWedding;
  public double dailyLuck;
  public ulong uniqueIDForThisGame;
  public bool weddingToday;
  public bool isRaining;
  public bool isDebrisWeather;
  public bool isLightning;
  public bool isSnowing;
  public bool shouldSpawnMonsters;
  public bool hasApplied1_3_UpdateChanges;
  public bool hasApplied1_4_UpdateChanges;
  public List<long> weddingsToday;
  /// <summary>Obsolete. This is only kept to preserve data from old save files.</summary>
  [XmlElement("stats")]
  public Stats obsolete_stats;
  [InstancedStatic]
  public static SaveGame loaded;
  public float musicVolume;
  public float soundVolume;
  public Object dishOfTheDay;
  public int highestPlayerLimit = -1;
  public int moveBuildingPermissionMode;
  public bool useLegacyRandom;
  public bool allowChatCheats;
  public bool hasDedicatedHost;
  public SerializableDictionary<string, LocationWeather> locationWeather;
  [XmlArrayItem("item")]
  public SaveablePair<string, BuilderData>[] builders;
  [XmlArrayItem("item")]
  public SaveablePair<string, string>[] bannedUsers = LegacyShims.EmptyArray<SaveablePair<string, string>>();
  [XmlArrayItem("item")]
  public SaveablePair<string, string>[] bundleData = LegacyShims.EmptyArray<SaveablePair<string, string>>();
  [XmlArrayItem("item")]
  public SaveablePair<string, int>[] limitedNutDrops = LegacyShims.EmptyArray<SaveablePair<string, int>>();
  public long latestID;
  public Options options;
  [XmlArrayItem("item")]
  public SaveablePair<long, Options>[] splitscreenOptions = LegacyShims.EmptyArray<SaveablePair<long, Options>>();
  public SerializableDictionary<string, string> CustomData = new SerializableDictionary<string, string>();
  [XmlArrayItem("item")]
  public SaveablePair<int, MineInfo>[] mine_permanentMineChanges;
  public int mine_lowestLevelReached;
  public string weatherForTomorrow;
  public string whichFarm;
  public int mine_lowestLevelReachedForOrder = -1;
  public int skullCavesDifficulty;
  public int minesDifficulty;
  public int currentGemBirdIndex;
  public NetLeaderboards junimoKartLeaderboards;
  public List<SpecialOrder> specialOrders;
  public List<SpecialOrder> availableSpecialOrders;
  public List<string> completedSpecialOrders;
  public List<string> acceptedSpecialOrderTypes = new List<string>();
  public List<Item> returnedDonations;
  /// <summary>Obsolete. This is only kept to preserve data from old save files. Use <see cref="F:StardewValley.SaveGame.globalInventories" /> instead.</summary>
  public List<Item> junimoChest;
  public Item[] shippingBin = LegacyShims.EmptyArray<Item>();
  /// <inheritdoc cref="F:StardewValley.FarmerTeam.globalInventories" />
  [XmlArrayItem("item")]
  public SaveablePair<string, Item[]>[] globalInventories = LegacyShims.EmptyArray<SaveablePair<string, Item[]>>();
  public List<string> collectedNutTracker = new List<string>();
  [XmlArrayItem("item")]
  public SaveablePair<FarmerPair, Friendship>[] farmerFriendships = LegacyShims.EmptyArray<SaveablePair<FarmerPair, Friendship>>();
  [XmlArrayItem("item")]
  public SaveablePair<int, long>[] cellarAssignments = LegacyShims.EmptyArray<SaveablePair<int, long>>();
  public int timesFedRaccoons;
  public int treasureTotemsUsed;
  public int perfectionWaivers;
  public int seasonOfCurrentRaccoonBundle;
  public bool[] raccoonBundles = new bool[2];
  public bool activatedGoldenParrot;
  public int daysPlayedWhenLastRaccoonBundleWasFinished;
  public int lastAppliedSaveFix;
  public string gameVersion = Game1.version;
  public string gameVersionLabel;
  public static XmlSerializer serializer = new XmlSerializer(typeof (SaveGame), new Type[5]
  {
    typeof (Character),
    typeof (GameLocation),
    typeof (Item),
    typeof (Quest),
    typeof (TerrainFeature)
  });
  public static XmlSerializer farmerSerializer = new XmlSerializer(typeof (Farmer), new Type[1]
  {
    typeof (Item)
  });
  public static XmlSerializer locationSerializer = new XmlSerializer(typeof (GameLocation), new Type[3]
  {
    typeof (Character),
    typeof (Item),
    typeof (TerrainFeature)
  });
  public static XmlSerializer descriptionElementSerializer = new XmlSerializer(typeof (DescriptionElement), new Type[2]
  {
    typeof (Character),
    typeof (Item)
  });
  public static XmlSerializer legacyDescriptionElementSerializer = new XmlSerializer(typeof (SaveMigrator_1_6.LegacyDescriptionElement), new Type[3]
  {
    typeof (DescriptionElement),
    typeof (Character),
    typeof (Item)
  });

  /// <summary>Get whether a fix was applied to the loaded data before it was last saved.</summary>
  /// <param name="fix">The save fix to check.</param>
  public bool HasSaveFix(SaveFixes fix) => (SaveFixes) this.lastAppliedSaveFix >= fix;

  public static IEnumerator<int> Save()
  {
    SaveGame.IsProcessing = true;
    if (LocalMultiplayer.IsLocalMultiplayer())
    {
      IEnumerator<int> save = SaveGame.getSaveEnumerator();
      while (save.MoveNext())
        yield return save.Current;
      yield return 100;
      save = (IEnumerator<int>) null;
    }
    else
    {
      SaveGame.LogVerbose("SaveGame.Save() called.");
      yield return 1;
      IEnumerator<int> loader = SaveGame.getSaveEnumerator();
      Task saveTask = new Task((Action) (() =>
      {
        Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
        if (loader == null)
          return;
        do
          ;
        while (loader.MoveNext() && loader.Current < 100);
      }));
      Game1.hooks.StartTask(saveTask, nameof (Save));
      while (!saveTask.IsCanceled && !saveTask.IsCompleted && !saveTask.IsFaulted)
        yield return 1;
      SaveGame.IsProcessing = false;
      if (saveTask.IsFaulted)
      {
        Exception baseException = saveTask.Exception.GetBaseException();
        SaveGame.LogError("saveTask failed with an exception", baseException);
        if (!(baseException is TaskCanceledException))
          throw baseException;
        Game1.ExitToTitle();
      }
      else
      {
        SaveGame.LogVerbose("SaveGame.Save() completed without exceptions.");
        yield return 100;
        saveTask = (Task) null;
      }
    }
  }

  public static string FilterFileName(string fileName)
  {
    StringBuilder stringBuilder = new StringBuilder(fileName.Length);
    foreach (char c in fileName)
    {
      if (char.IsLetterOrDigit(c))
        stringBuilder.Append(c);
    }
    fileName = stringBuilder.ToString();
    return fileName;
  }

  /// <summary>Get an enumerator which writes the loaded save to a save file.</summary>
  /// <returns>Returns an enumeration of incrementing progress values between 0 and 100.</returns>
  public static IEnumerator<int> getSaveEnumerator()
  {
    if (SaveGame.CancelToTitle)
      throw new TaskCanceledException();
    yield return 1;
    foreach (Farmer allFarmer in Game1.getAllFarmers())
      allFarmer.UnapplyAllTrinketEffects();
    Game1.player.gameVersion = Game1.version;
    Game1.player.gameVersionLabel = Game1.versionLabel;
    foreach (GameLocation location in (IEnumerable<GameLocation>) Game1.locations)
      location.cleanupBeforeSave();
    Game1.player.team.globalInventories.RemoveWhere((Func<KeyValuePair<string, Inventory>, bool>) (p => !p.Value.HasAny()));
    SaveGame saveData = new SaveGame()
    {
      player = Game1.player,
      farmhands = new List<Farmer>((IEnumerable<Farmer>) Game1.netWorldState.Value.farmhandData.Values),
      locations = new List<GameLocation>((IEnumerable<GameLocation>) Game1.locations),
      currentSeason = Game1.currentSeason,
      samBandName = Game1.samBandName,
      broadcastedMail = new HashSet<string>((IEnumerable<string>) Game1.player.team.broadcastedMail),
      constructedBuildings = new HashSet<string>((IEnumerable<string>) Game1.player.team.constructedBuildings),
      bannedUsers = Game1.bannedUsers.ToSaveableArray<string, string>(),
      skullCavesDifficulty = Game1.netWorldState.Value.SkullCavesDifficulty,
      minesDifficulty = Game1.netWorldState.Value.MinesDifficulty,
      visitsUntilY1Guarantee = Game1.netWorldState.Value.VisitsUntilY1Guarantee,
      shuffleMineChests = Game1.netWorldState.Value.ShuffleMineChests,
      elliottBookName = Game1.elliottBookName,
      dayOfMonth = Game1.dayOfMonth,
      year = Game1.year,
      dailyLuck = Game1.player.team.sharedDailyLuck.Value,
      isRaining = Game1.isRaining,
      isLightning = Game1.isLightning,
      isSnowing = Game1.isSnowing,
      isDebrisWeather = Game1.isDebrisWeather,
      shouldSpawnMonsters = Game1.spawnMonstersAtNight,
      specialOrders = Game1.player.team.specialOrders.ToList<SpecialOrder>(),
      availableSpecialOrders = Game1.player.team.availableSpecialOrders.ToList<SpecialOrder>(),
      completedSpecialOrders = Game1.player.team.completedSpecialOrders.ToList<string>(),
      collectedNutTracker = Game1.player.team.collectedNutTracker.ToList<string>(),
      acceptedSpecialOrderTypes = Game1.player.team.acceptedSpecialOrderTypes.ToList<string>(),
      returnedDonations = Game1.player.team.returnedDonations.ToList<Item>(),
      weddingToday = Game1.weddingToday,
      weddingsToday = Game1.weddingsToday.ToList<long>(),
      shippingBin = Game1.getFarm().getShippingBin(Game1.player).ToArray<Item>(),
      globalInventories = DictionarySaver<string, Item[]>.ArrayFrom<NetRef<Inventory>>((IDictionary<string, NetRef<Inventory>>) Game1.player.team.globalInventories.FieldDict, (Func<NetRef<Inventory>, Item[]>) (value => value.Value.ToArray<Item>())),
      whichFarm = Game1.GetFarmTypeID(),
      junimoKartLeaderboards = Game1.player.team.junimoKartScores,
      lastAppliedSaveFix = 98,
      locationWeather = SerializableDictionary<string, LocationWeather>.BuildFrom<NetRef<LocationWeather>>((IDictionary<string, NetRef<LocationWeather>>) Game1.netWorldState.Value.LocationWeather.FieldDict, (Func<NetRef<LocationWeather>, LocationWeather>) (value => value.Value)),
      builders = DictionarySaver<string, BuilderData>.ArrayFrom<NetRef<BuilderData>>((IDictionary<string, NetRef<BuilderData>>) Game1.netWorldState.Value.Builders.FieldDict, (Func<NetRef<BuilderData>, BuilderData>) (value => value.Value)),
      cellarAssignments = DictionarySaver<int, long>.ArrayFrom<NetLong>((IDictionary<int, NetLong>) Game1.player.team.cellarAssignments.FieldDict, (Func<NetLong, long>) (value => value.Value)),
      uniqueIDForThisGame = Game1.uniqueIDForThisGame,
      musicVolume = Game1.options.musicVolumeLevel,
      soundVolume = Game1.options.soundVolumeLevel,
      mine_lowestLevelReached = Game1.netWorldState.Value.LowestMineLevel,
      mine_lowestLevelReachedForOrder = Game1.netWorldState.Value.LowestMineLevelForOrder,
      currentGemBirdIndex = Game1.currentGemBirdIndex,
      mine_permanentMineChanges = MineShaft.permanentMineChanges.ToSaveableArray<int, MineInfo>(),
      dishOfTheDay = Game1.dishOfTheDay,
      latestID = (long) Game1.multiplayer.latestID,
      highestPlayerLimit = Game1.netWorldState.Value.HighestPlayerLimit,
      options = Game1.options,
      splitscreenOptions = Game1.splitscreenOptions.ToSaveableArray<long, Options>(),
      CustomData = Game1.CustomData,
      worldStateIDs = Game1.worldStateIDs,
      weatherForTomorrow = Game1.weatherForTomorrow,
      goldenWalnuts = Game1.netWorldState.Value.GoldenWalnuts,
      goldenWalnutsFound = Game1.netWorldState.Value.GoldenWalnutsFound,
      miniShippingBinsObtained = Game1.netWorldState.Value.MiniShippingBinsObtained,
      goldenCoconutCracked = Game1.netWorldState.Value.GoldenCoconutCracked,
      parrotPlatformsUnlocked = Game1.netWorldState.Value.ParrotPlatformsUnlocked,
      farmPerfect = Game1.player.team.farmPerfect.Value,
      lostBooksFound = Game1.netWorldState.Value.LostBooksFound,
      foundBuriedNuts = Game1.netWorldState.Value.FoundBuriedNuts.ToList<string>(),
      checkedGarbage = Game1.netWorldState.Value.CheckedGarbage.ToList<string>(),
      mineShrineActivated = Game1.player.team.mineShrineActivated.Value,
      skullShrineActivated = Game1.player.team.skullShrineActivated.Value,
      timesFedRaccoons = Game1.netWorldState.Value.TimesFedRaccoons,
      treasureTotemsUsed = Game1.netWorldState.Value.TreasureTotemsUsed,
      perfectionWaivers = Game1.netWorldState.Value.PerfectionWaivers,
      seasonOfCurrentRaccoonBundle = Game1.netWorldState.Value.SeasonOfCurrentRacconBundle,
      raccoonBundles = Game1.netWorldState.Value.raccoonBundles.ToArray<bool>(),
      activatedGoldenParrot = Game1.netWorldState.Value.ActivatedGoldenParrot,
      daysPlayedWhenLastRaccoonBundleWasFinished = Game1.netWorldState.Value.DaysPlayedWhenLastRaccoonBundleWasFinished,
      gameVersion = Game1.version,
      gameVersionLabel = Game1.versionLabel,
      limitedNutDrops = DictionarySaver<string, int>.ArrayFrom<NetInt>((IDictionary<string, NetInt>) Game1.player.team.limitedNutDrops.FieldDict, (Func<NetInt, int>) (value => value.Value)),
      bundleData = Game1.netWorldState.Value.BundleData.ToSaveableArray<string, string>(),
      moveBuildingPermissionMode = (int) Game1.player.team.farmhandsCanMoveBuildings.Value,
      useLegacyRandom = Game1.player.team.useLegacyRandom.Value,
      allowChatCheats = Game1.player.team.allowChatCheats.Value,
      hasDedicatedHost = Game1.player.team.hasDedicatedHost.Value,
      hasApplied1_3_UpdateChanges = true,
      hasApplied1_4_UpdateChanges = true,
      farmerFriendships = DictionarySaver<FarmerPair, Friendship>.ArrayFrom<NetRef<Friendship>>((IDictionary<FarmerPair, NetRef<Friendship>>) Game1.player.team.friendshipData.FieldDict, (Func<NetRef<Friendship>, Friendship>) (value => value.Value))
    };
    string path2 = $"{SaveGame.FilterFileName(Game1.GetSaveGameName())}_{Game1.uniqueIDForThisGame.ToString()}";
    string path1 = Path.Combine(Program.GetSavesFolder(), path2 + Path.DirectorySeparatorChar.ToString());
    string finalFarmerPath = Path.Combine(path1, "SaveGameInfo");
    string finalDataPath = Path.Combine(path1, path2);
    string tempFarmerPath = finalFarmerPath + "_STARDEWVALLEYSAVETMP";
    string tempDataPath = finalDataPath + "_STARDEWVALLEYSAVETMP";
    SaveGame.ensureFolderStructureExists();
    Stream fstream = (Stream) null;
    try
    {
      fstream = (Stream) File.Open(tempDataPath, FileMode.Create);
    }
    catch (IOException ex)
    {
      if (fstream != null)
      {
        fstream.Close();
        fstream.Dispose();
      }
      Game1.gameMode = (byte) 9;
      Game1.debugOutput = Game1.parseText(ex.Message);
      yield break;
    }
    MemoryStream mstream1 = new MemoryStream(1024 /*0x0400*/);
    MemoryStream mstream2 = new MemoryStream(1024 /*0x0400*/);
    Stream stream = (Stream) null;
    byte[] buffer1 = (byte[]) null;
    if (SaveGame.CancelToTitle)
      throw new TaskCanceledException();
    yield return 2;
    SaveGame.LogVerbose("Saving without compression...");
    MemoryStream output1 = mstream1;
    stream = (Stream) mstream2;
    XmlWriterSettings settings = new XmlWriterSettings()
    {
      CloseOutput = false
    };
    XmlWriter xmlWriter1 = XmlWriter.Create((Stream) output1, settings);
    xmlWriter1.WriteStartDocument();
    SaveSerializer.Serialize<SaveGame>(xmlWriter1, saveData);
    xmlWriter1.WriteEndDocument();
    xmlWriter1.Flush();
    xmlWriter1.Close();
    output1.Close();
    buffer1 = mstream1.ToArray();
    mstream1 = (MemoryStream) null;
    if (SaveGame.CancelToTitle)
      throw new TaskCanceledException();
    yield return 2;
    fstream.Write(buffer1, 0, buffer1.Length);
    fstream.Close();
    buffer1 = (byte[]) null;
    mstream1 = (MemoryStream) null;
    Game1.player.saveTime = (int) (DateTime.UtcNow - new DateTime(2012, 6, 22)).TotalMinutes;
    try
    {
      fstream = (Stream) File.Open(tempFarmerPath, FileMode.Create);
    }
    catch (IOException ex)
    {
      fstream?.Close();
      Game1.gameMode = (byte) 9;
      Game1.debugOutput = Game1.parseText(ex.Message);
      yield break;
    }
    Stream output2 = fstream;
    XmlWriter xmlWriter2 = XmlWriter.Create(output2, settings);
    xmlWriter2.WriteStartDocument();
    SaveSerializer.Serialize<Farmer>(xmlWriter2, Game1.player);
    xmlWriter2.WriteEndDocument();
    xmlWriter2.Flush();
    xmlWriter2.Close();
    output2.Close();
    fstream.Close();
    if (SaveGame.CancelToTitle)
      throw new TaskCanceledException();
    yield return 2;
    string destFilePath1 = finalDataPath + "_old";
    string destFilePath2 = finalFarmerPath + "_old";
    try
    {
      LegacyShims.MoveFileWithOverwrite(finalDataPath, destFilePath1);
      LegacyShims.MoveFileWithOverwrite(finalFarmerPath, destFilePath2);
    }
    catch
    {
    }
    LegacyShims.MoveFileWithOverwrite(tempDataPath, finalDataPath);
    LegacyShims.MoveFileWithOverwrite(tempFarmerPath, finalFarmerPath);
    foreach (Farmer allFarmer in Game1.getAllFarmers())
      allFarmer.resetAllTrinketEffects();
    Game1.player.sleptInTemporaryBed.Value = false;
    if (SaveGame.CancelToTitle)
      throw new TaskCanceledException();
    yield return 100;
  }

  public static bool IsNewGameSaveNameCollision(string save_name)
  {
    string path2 = $"{SaveGame.FilterFileName(save_name)}_{Game1.uniqueIDForThisGame.ToString()}";
    return Directory.Exists(Path.Combine(Program.GetSavesFolder(), path2));
  }

  public static void ensureFolderStructureExists()
  {
    string path2 = $"{SaveGame.FilterFileName(Game1.GetSaveGameName())}_{Game1.uniqueIDForThisGame.ToString()}";
    Directory.CreateDirectory(Path.Combine(Program.GetSavesFolder(), path2));
  }

  public static void Load(string filename)
  {
    Game1.gameMode = (byte) 6;
    Game1.loadingMessage = Game1.content.LoadString("Strings\\StringsFromCSFiles:SaveGame.cs.4690");
    Game1.currentLoader = SaveGame.getLoadEnumerator(filename);
  }

  public static void LoadFarmType()
  {
    List<ModFarmType> modFarmTypeList = DataLoader.AdditionalFarms(Game1.content);
    Game1.whichFarm = -1;
    if (modFarmTypeList != null)
    {
      foreach (ModFarmType modFarmType in modFarmTypeList)
      {
        if (modFarmType.Id == SaveGame.loaded.whichFarm)
        {
          Game1.whichModFarm = modFarmType;
          Game1.whichFarm = 7;
          break;
        }
      }
    }
    if (SaveGame.loaded.whichFarm == null)
      Game1.whichFarm = 0;
    if (Game1.whichFarm >= 0)
      return;
    int result;
    if (int.TryParse(SaveGame.loaded.whichFarm, out result))
    {
      Game1.whichFarm = result;
    }
    else
    {
      SaveGame.LogWarn($"Ignored unknown farm type '{SaveGame.loaded.whichFarm}' which no longer exists in the data.");
      Game1.whichFarm = 0;
      Game1.whichModFarm = (ModFarmType) null;
    }
  }

  /// <summary>Read a raw save file, if it's valid.</summary>
  /// <param name="file">The save folder name to load, in the form <c>{farmer name}_{unique id}</c>.</param>
  /// <param name="fileNameSuffix">The suffix for the filename within the save folder to load, if supported by the platform. This should usually be <c>null</c> (main file), <see cref="F:StardewValley.SaveGame.BackupNameSuffix" />, or <see cref="F:StardewValley.SaveGame.TempNameSuffix" />.</param>
  /// <param name="error">An error indicating why loading the save file failed, if applicable.</param>
  /// <remarks>This is a low-level method. Most code should use <see cref="M:StardewValley.SaveGame.getLoadEnumerator(System.String)" /> instead.</remarks>
  public static SaveGame TryReadSaveFile(string file, string fileNameSuffix, out string error)
  {
    string path = Path.Combine(Program.GetSavesFolder(), file, file + fileNameSuffix);
    if (!File.Exists(path))
    {
      path += ".xml";
      if (!File.Exists(path))
        return FileDoesNotExist(out error);
    }
    Stream stream1 = (Stream) null;
    Stream stream2;
    try
    {
      stream2 = (Stream) new MemoryStream(File.ReadAllBytes(path), false);
    }
    catch (IOException ex)
    {
      error = ex.Message;
      stream1?.Close();
      return (SaveGame) null;
    }
    byte num = (byte) stream2.ReadByte();
    --stream2.Position;
    if (num == (byte) 120)
    {
      SaveGame.LogVerbose("zlib stream detected...");
      stream2 = (Stream) new ZlibStream(stream2, CompressionMode.Decompress);
    }
    try
    {
      error = (string) null;
      return SaveSerializer.Deserialize<SaveGame>(stream2);
    }
    catch (Exception ex)
    {
      error = ex.Message;
      return (SaveGame) null;
    }
    finally
    {
      stream2.Dispose();
    }

    static SaveGame FileDoesNotExist(out string outError)
    {
      outError = "File does not exist";
      return (SaveGame) null;
    }
  }

  /// <summary>Read a raw save file with automatic fallback to the backup files, if any of them are valid.</summary>
  /// <param name="file">The save folder name to load, in the form <c>{farmer name}_{unique id}</c>.</param>
  /// <param name="error">An error indicating why loading the save file failed, if applicable.</param>
  /// <param name="autoRecovered">Whether the save was auto-recovered by loading a backup.</param>
  /// <remarks>This is a low-level method. Most code should use <see cref="M:StardewValley.SaveGame.getLoadEnumerator(System.String)" /> instead.</remarks>
  public static SaveGame TryReadSaveFileWithFallback(
    string file,
    out string error,
    out bool autoRecovered)
  {
    SaveGame saveGame1 = SaveGame.TryReadSaveFile(file, (string) null, out error);
    if (saveGame1 != null)
    {
      error = (string) null;
      autoRecovered = false;
      return saveGame1;
    }
    string error1;
    SaveGame saveGame2 = SaveGame.TryReadSaveFile(file, "_old", out error1);
    if (saveGame2 != null)
    {
      error = (string) null;
      autoRecovered = true;
      return saveGame2;
    }
    SaveGame saveGame3 = SaveGame.TryReadSaveFile(file, "_STARDEWVALLEYSAVETMP", out error1);
    if (saveGame3 != null)
    {
      error = (string) null;
      autoRecovered = true;
      return saveGame3;
    }
    error = error ?? "Save could not be loaded";
    autoRecovered = false;
    return (SaveGame) null;
  }

  /// <summary>Get an enumerator which loads a save file.</summary>
  /// <param name="file">The save folder name to load, in the form <c>{farmer name}_{unique id}</c>.</param>
  /// <returns>Returns an enumeration of incrementing progress values between 0 and 100.</returns>
  public static IEnumerator<int> getLoadEnumerator(string file)
  {
    SaveGame.LogVerbose($"getLoadEnumerator('{file}')");
    Stopwatch stopwatch = Stopwatch.StartNew();
    Game1.SetSaveName(((IEnumerable<string>) Path.GetFileNameWithoutExtension(file).Split('_')).FirstOrDefault<string>());
    Game1.loadingMessage = Game1.content.LoadString("Strings\\StringsFromCSFiles:SaveGame.cs.4690");
    SaveGame.IsProcessing = true;
    if (SaveGame.CancelToTitle)
      Game1.ExitToTitle();
    yield return 1;
    string error = (string) null;
    bool autoRecovered = false;
    Task readSaveTask = new Task((Action) (() =>
    {
      Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
      SaveGame.loaded = SaveGame.TryReadSaveFileWithFallback(file, out error, out autoRecovered);
    }));
    Game1.hooks.StartTask(readSaveTask, "Load_ReadSave");
    while (!readSaveTask.IsCanceled && !readSaveTask.IsCompleted && !readSaveTask.IsFaulted)
      yield return 15;
    if (SaveGame.loaded == null)
    {
      Game1.gameMode = (byte) 9;
      Game1.debugOutput = Game1.parseText(error);
    }
    else
    {
      if (autoRecovered)
        SaveGame.LogWarn($"Save file {file} was corrupted; auto-recovered it from the backup.");
      readSaveTask = (Task) null;
      yield return 19;
      Game1.hasApplied1_3_UpdateChanges = SaveGame.loaded.hasApplied1_3_UpdateChanges;
      Game1.hasApplied1_4_UpdateChanges = SaveGame.loaded.hasApplied1_4_UpdateChanges;
      Game1.lastAppliedSaveFix = (SaveFixes) SaveGame.loaded.lastAppliedSaveFix;
      Game1.player.team.useLegacyRandom.Value = SaveGame.loaded.useLegacyRandom;
      Game1.loadingMessage = Game1.content.LoadString("Strings\\StringsFromCSFiles:SaveGame.cs.4697");
      if (SaveGame.CancelToTitle)
        Game1.ExitToTitle();
      yield return 20;
      SaveGame.LoadFarmType();
      Game1.year = SaveGame.loaded.year;
      Game1.netWorldState.Value.CurrentPlayerLimit = Game1.multiplayer.playerLimit;
      if (SaveGame.loaded.highestPlayerLimit >= 0)
        Game1.netWorldState.Value.HighestPlayerLimit = SaveGame.loaded.highestPlayerLimit;
      else
        Game1.netWorldState.Value.HighestPlayerLimit = Math.Max(Game1.netWorldState.Value.HighestPlayerLimit, Game1.multiplayer.MaxPlayers);
      Game1.uniqueIDForThisGame = SaveGame.loaded.uniqueIDForThisGame;
      if (LocalMultiplayer.IsLocalMultiplayer())
      {
        Game1.game1.loadForNewGame(true);
      }
      else
      {
        readSaveTask = new Task((Action) (() =>
        {
          Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
          Game1.game1.loadForNewGame(true);
        }));
        Game1.hooks.StartTask(readSaveTask, "Load_LoadForNewGame");
        while (!readSaveTask.IsCanceled && !readSaveTask.IsCompleted && !readSaveTask.IsFaulted)
          yield return 24;
        if (readSaveTask.IsFaulted)
        {
          Exception baseException = readSaveTask.Exception.GetBaseException();
          SaveGame.LogError("loadNewGameTask failed with an exception.", baseException);
          throw baseException;
        }
        if (SaveGame.CancelToTitle)
          Game1.ExitToTitle();
        yield return 25;
        readSaveTask = (Task) null;
      }
      int result;
      Game1.weatherForTomorrow = int.TryParse(SaveGame.loaded.weatherForTomorrow, out result) ? Utility.LegacyWeatherToWeather(result) : SaveGame.loaded.weatherForTomorrow;
      Game1.dayOfMonth = SaveGame.loaded.dayOfMonth;
      Game1.year = SaveGame.loaded.year;
      Game1.currentSeason = SaveGame.loaded.currentSeason;
      Game1.worldStateIDs = SaveGame.loaded.worldStateIDs;
      Game1.loadingMessage = Game1.content.LoadString("Strings\\StringsFromCSFiles:SaveGame.cs.4698");
      if (SaveGame.loaded.mine_permanentMineChanges != null)
      {
        MineShaft.permanentMineChanges = new SerializableDictionary<int, MineInfo>((IDictionary<int, MineInfo>) SaveGame.loaded.mine_permanentMineChanges.ToDictionary<int, MineInfo>());
        Game1.netWorldState.Value.LowestMineLevel = SaveGame.loaded.mine_lowestLevelReached;
        Game1.netWorldState.Value.LowestMineLevelForOrder = SaveGame.loaded.mine_lowestLevelReachedForOrder;
      }
      Game1.currentGemBirdIndex = SaveGame.loaded.currentGemBirdIndex;
      if (SaveGame.loaded.bundleData.Length != 0)
      {
        Dictionary<string, string> dictionary = SaveGame.loaded.bundleData.ToDictionary<string, string>();
        if (!SaveGame.loaded.HasSaveFix(SaveFixes.StandardizeBundleFields))
          SaveMigrator_1_6.StandardizeBundleFields(dictionary);
        Game1.netWorldState.Value.SetBundleData(dictionary);
      }
      if (SaveGame.CancelToTitle)
        Game1.ExitToTitle();
      yield return 26;
      Game1.isRaining = SaveGame.loaded.isRaining;
      Game1.isLightning = SaveGame.loaded.isLightning;
      Game1.isSnowing = SaveGame.loaded.isSnowing;
      Game1.isGreenRain = Utility.isGreenRainDay();
      if (Game1.IsMasterGame)
        Game1.netWorldState.Value.UpdateFromGame1();
      if (SaveGame.loaded.locationWeather != null)
      {
        Game1.netWorldState.Value.LocationWeather.Clear();
        foreach (KeyValuePair<string, LocationWeather> keyValuePair in (Dictionary<string, LocationWeather>) SaveGame.loaded.locationWeather)
          Game1.netWorldState.Value.LocationWeather[keyValuePair.Key] = keyValuePair.Value;
      }
      if (SaveGame.loaded.builders != null)
      {
        foreach (SaveablePair<string, BuilderData> builder in SaveGame.loaded.builders)
          Game1.netWorldState.Value.Builders[builder.Key] = builder.Value;
      }
      if (LocalMultiplayer.IsLocalMultiplayer())
      {
        SaveGame.loadDataToFarmer(SaveGame.loaded.player);
      }
      else
      {
        readSaveTask = new Task((Action) (() =>
        {
          Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
          SaveGame.loadDataToFarmer(SaveGame.loaded.player);
        }));
        Game1.hooks.StartTask(readSaveTask, "Load_Farmer");
        while (!readSaveTask.IsCanceled && !readSaveTask.IsCompleted && !readSaveTask.IsFaulted)
          yield return 1;
        if (readSaveTask.IsFaulted)
        {
          Exception baseException = readSaveTask.Exception.GetBaseException();
          SaveGame.LogError("loadFarmerTask failed with an exception", baseException);
          throw baseException;
        }
        readSaveTask = (Task) null;
      }
      Game1.player = SaveGame.loaded.player;
      Game1.player.team.useLegacyRandom.Value = SaveGame.loaded.useLegacyRandom;
      Game1.player.team.allowChatCheats.Value = SaveGame.loaded.allowChatCheats;
      Game1.player.team.hasDedicatedHost.Value = SaveGame.loaded.hasDedicatedHost;
      Game1.netWorldState.Value.farmhandData.Clear();
      if (Game1.lastAppliedSaveFix < SaveFixes.MigrateFarmhands)
        SaveMigrator_1_6.MigrateFarmhands(SaveGame.loaded.locations);
      if (SaveGame.loaded.farmhands != null)
      {
        foreach (Farmer farmhand in SaveGame.loaded.farmhands)
          Game1.netWorldState.Value.farmhandData[farmhand.UniqueMultiplayerID] = farmhand;
      }
      foreach (Farmer target in Game1.netWorldState.Value.farmhandData.Values)
        SaveGame.loadDataToFarmer(target);
      if (Game1.MasterPlayer.hasOrWillReceiveMail("leoMoved") && Game1.getLocationFromName("Mountain") is Mountain locationFromName1)
      {
        locationFromName1.reloadMap();
        locationFromName1.ApplyTreehouseIfNecessary();
        if (locationFromName1.treehouseDoorDirty)
        {
          locationFromName1.treehouseDoorDirty = false;
          WarpPathfindingCache.PopulateCache();
        }
      }
      if (SaveGame.loaded.farmerFriendships != null)
      {
        foreach (SaveablePair<FarmerPair, Friendship> farmerFriendship in SaveGame.loaded.farmerFriendships)
          Game1.player.team.friendshipData[farmerFriendship.Key] = farmerFriendship.Value;
      }
      Game1.spawnMonstersAtNight = SaveGame.loaded.shouldSpawnMonsters;
      Game1.player.team.limitedNutDrops.Clear();
      if ((NetFieldBase<NetWorldState, NetRef<NetWorldState>>) Game1.netWorldState != (NetRef<NetWorldState>) null && Game1.netWorldState.Value != null)
        Game1.netWorldState.Value.RegisterSpecialCurrencies();
      if (SaveGame.loaded.limitedNutDrops != null)
      {
        foreach (SaveablePair<string, int> limitedNutDrop in SaveGame.loaded.limitedNutDrops)
        {
          if (limitedNutDrop.Value > 0)
            Game1.player.team.limitedNutDrops[limitedNutDrop.Key] = limitedNutDrop.Value;
        }
      }
      Game1.player.team.completedSpecialOrders.Clear();
      Game1.player.team.completedSpecialOrders.AddRange<string>((IEnumerable<string>) SaveGame.loaded.completedSpecialOrders);
      Game1.player.team.specialOrders.Clear();
      foreach (SpecialOrder specialOrder in SaveGame.loaded.specialOrders)
      {
        if (specialOrder != null)
          Game1.player.team.specialOrders.Add(specialOrder);
      }
      Game1.player.team.availableSpecialOrders.Clear();
      foreach (SpecialOrder availableSpecialOrder in SaveGame.loaded.availableSpecialOrders)
      {
        if (availableSpecialOrder != null)
          Game1.player.team.availableSpecialOrders.Add(availableSpecialOrder);
      }
      Game1.player.team.acceptedSpecialOrderTypes.Clear();
      Game1.player.team.acceptedSpecialOrderTypes.AddRange<string>((IEnumerable<string>) SaveGame.loaded.acceptedSpecialOrderTypes);
      Game1.player.team.collectedNutTracker.Clear();
      Game1.player.team.collectedNutTracker.AddRange<string>((IEnumerable<string>) SaveGame.loaded.collectedNutTracker);
      Game1.player.team.globalInventories.Clear();
      if (SaveGame.loaded.globalInventories != null)
      {
        foreach (SaveablePair<string, Item[]> globalInventory in SaveGame.loaded.globalInventories)
          Game1.player.team.GetOrCreateGlobalInventory(globalInventory.Key).AddRange((ICollection<Item>) globalInventory.Value);
      }
      List<Item> junimoChest = SaveGame.loaded.junimoChest;
      // ISSUE: explicit non-virtual call
      if ((junimoChest != null ? (__nonvirtual (junimoChest.Count) > 0 ? 1 : 0) : 0) != 0)
        Game1.player.team.GetOrCreateGlobalInventory("JunimoChests").AddRange((ICollection<Item>) SaveGame.loaded.junimoChest);
      Game1.player.team.returnedDonations.Clear();
      foreach (Item returnedDonation in SaveGame.loaded.returnedDonations)
        Game1.player.team.returnedDonations.Add(returnedDonation);
      if (SaveGame.loaded.obsolete_stats != null)
        Game1.player.stats = SaveGame.loaded.obsolete_stats;
      if (SaveGame.loaded.obsolete_mailbox != null && !Game1.player.mailbox.Any())
        Game1.player.mailbox.AddRange((IEnumerable<string>) SaveGame.loaded.obsolete_mailbox);
      Game1.random = Utility.CreateDaySaveRandom(1.0);
      Game1.loadingMessage = Game1.content.LoadString("Strings\\StringsFromCSFiles:SaveGame.cs.4699");
      if (SaveGame.CancelToTitle)
        Game1.ExitToTitle();
      yield return 36;
      Game1.UpdatePassiveFestivalStates();
      if (SaveGame.loaded.cellarAssignments != null)
      {
        foreach (SaveablePair<int, long> cellarAssignment in SaveGame.loaded.cellarAssignments)
          Game1.player.team.cellarAssignments[cellarAssignment.Key] = cellarAssignment.Value;
      }
      if (LocalMultiplayer.IsLocalMultiplayer())
      {
        SaveGame.loadDataToLocations(SaveGame.loaded.locations);
      }
      else
      {
        readSaveTask = new Task((Action) (() =>
        {
          Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
          SaveGame.loadDataToLocations(SaveGame.loaded.locations);
        }));
        Game1.hooks.StartTask(readSaveTask, "Load_Locations");
        while (!readSaveTask.IsCanceled && !readSaveTask.IsCompleted && !readSaveTask.IsFaulted)
          yield return 1;
        if (readSaveTask.IsFaulted)
        {
          SaveGame.LogError("loadLocationsTask failed with an exception", readSaveTask.Exception.GetBaseException());
          throw readSaveTask.Exception.GetBaseException();
        }
        readSaveTask = (Task) null;
      }
      if (SaveGame.loaded.shippingBin != null)
      {
        Game1.getFarm().getShippingBin(Game1.player).Clear();
        Game1.getFarm().getShippingBin(Game1.player).AddRange((ICollection<Item>) SaveGame.loaded.shippingBin);
      }
      if (Game1.getLocationFromName("Railroad") is Railroad locationFromName2)
        locationFromName2.ResetTrainForNewDay();
      HashSet<long> validFarmhands = new HashSet<long>();
      Utility.ForEachBuilding((Func<Building, bool>) (building =>
      {
        if (building?.GetIndoors() is Cabin indoors2)
          validFarmhands.Add(indoors2.farmhandReference.UID);
        return true;
      }));
      List<Farmer> farmerList = new List<Farmer>();
      foreach (Farmer farmer in Game1.netWorldState.Value.farmhandData.Values)
      {
        if (!farmer.isCustomized.Value && !validFarmhands.Contains(farmer.UniqueMultiplayerID))
          farmerList.Add(farmer);
      }
      foreach (Farmer farmhand in farmerList)
        Game1.player.team.DeleteFarmhand(farmhand);
      foreach (Farmer allFarmer in Game1.getAllFarmers())
      {
        int money = allFarmer.Money;
        NetIntDelta netIntDelta;
        if (!Game1.player.team.individualMoney.TryGetValue(allFarmer.UniqueMultiplayerID, out netIntDelta))
          Game1.player.team.individualMoney[allFarmer.UniqueMultiplayerID] = netIntDelta = new NetIntDelta(money);
        netIntDelta.Value = money;
      }
      Game1.updateCellarAssignments();
      foreach (GameLocation location in (IEnumerable<GameLocation>) Game1.locations)
      {
        foreach (Building building in location.buildings)
        {
          GameLocation indoors3 = building.GetIndoors();
          if (indoors3 != null)
          {
            if (indoors3 is FarmHouse farmHouse)
              farmHouse.updateCellarWarps();
            indoors3.parentLocationName.Value = location.NameOrUniqueName;
          }
        }
        if (location is FarmHouse farmHouse1)
          farmHouse1.updateCellarWarps();
      }
      foreach (Farmer farmhand in Game1.netWorldState.Value.farmhandData.Values)
        Game1.netWorldState.Value.ResetFarmhandState(farmhand);
      if (SaveGame.CancelToTitle)
        Game1.ExitToTitle();
      yield return 50;
      yield return 51;
      Game1.isDebrisWeather = SaveGame.loaded.isDebrisWeather;
      if (Game1.isDebrisWeather)
        Game1.populateDebrisWeatherArray();
      else
        Game1.debrisWeather.Clear();
      yield return 53;
      Game1.player.team.sharedDailyLuck.Value = SaveGame.loaded.dailyLuck;
      yield return 54;
      yield return 55;
      Game1.setGraphicsForSeason(true);
      yield return 56;
      Game1.samBandName = SaveGame.loaded.samBandName;
      Game1.elliottBookName = SaveGame.loaded.elliottBookName;
      yield return 63 /*0x3F*/;
      Game1.weddingToday = SaveGame.loaded.weddingToday;
      Game1.weddingsToday = SaveGame.loaded.weddingsToday.ToList<long>();
      Game1.loadingMessage = Game1.content.LoadString("Strings\\StringsFromCSFiles:SaveGame.cs.4700");
      yield return 64 /*0x40*/;
      Game1.loadingMessage = Game1.content.LoadString("Strings\\StringsFromCSFiles:SaveGame.cs.4701");
      if (SaveGame.CancelToTitle)
        Game1.ExitToTitle();
      yield return 79;
      Game1.options.musicVolumeLevel = SaveGame.loaded.musicVolume;
      Game1.options.soundVolumeLevel = SaveGame.loaded.soundVolume;
      yield return 83;
      if (SaveGame.loaded.countdownToWedding.HasValue && SaveGame.loaded.countdownToWedding.Value != 0 && !string.IsNullOrEmpty(SaveGame.loaded.player.spouse))
      {
        WorldDate worldDate = WorldDate.Now();
        worldDate.TotalDays += SaveGame.loaded.countdownToWedding.Value;
        Friendship friendship = SaveGame.loaded.player.friendshipData[SaveGame.loaded.player.spouse];
        friendship.Status = FriendshipStatus.Engaged;
        friendship.WeddingDate = worldDate;
      }
      yield return 85;
      yield return 87;
      yield return 88;
      yield return 95;
      Game1.fadeToBlack = true;
      Game1.fadeIn = false;
      Game1.fadeToBlackAlpha = 0.99f;
      if ((double) Game1.player.mostRecentBed.X <= 0.0)
        Game1.player.Position = new Vector2(192f, 384f);
      Game1.addNewFarmBuildingMaps();
      GameLocation gameLocation = (GameLocation) null;
      if (Game1.player.lastSleepLocation.Value != null && Game1.isLocationAccessible(Game1.player.lastSleepLocation.Value))
        gameLocation = Game1.getLocationFromName(Game1.player.lastSleepLocation.Value);
      bool flag = true;
      if (gameLocation != null && gameLocation.CanWakeUpHere(Game1.player))
      {
        Game1.currentLocation = gameLocation;
        Game1.player.currentLocation = Game1.currentLocation;
        Game1.player.Position = Utility.PointToVector2(Game1.player.lastSleepPoint.Value) * 64f;
        flag = false;
      }
      if (flag)
        Game1.currentLocation = Game1.RequireLocation("FarmHouse");
      Game1.currentLocation.map.LoadTileSheets(Game1.mapDisplayDevice);
      Game1.player.CanMove = true;
      Game1.player.ReequipEnchantments();
      if (SaveGame.loaded.junimoKartLeaderboards != null)
        Game1.player.team.junimoKartScores.LoadScores(SaveGame.loaded.junimoKartLeaderboards.GetScores());
      Game1.options = SaveGame.loaded.options;
      Game1.splitscreenOptions = new SerializableDictionary<long, Options>((IDictionary<long, Options>) SaveGame.loaded.splitscreenOptions.ToDictionary<long, Options>());
      Game1.CustomData = SaveGame.loaded.CustomData;
      Game1.player.team.broadcastedMail.Clear();
      if (SaveGame.loaded.broadcastedMail != null)
        Game1.player.team.broadcastedMail.AddRange<string>((IEnumerable<string>) SaveGame.loaded.broadcastedMail);
      Game1.player.team.constructedBuildings.Clear();
      if (SaveGame.loaded.constructedBuildings != null)
        Game1.player.team.constructedBuildings.AddRange<string>((IEnumerable<string>) SaveGame.loaded.constructedBuildings);
      if (Game1.options == null)
      {
        Game1.options = new Options();
        Game1.options.LoadDefaultOptions();
      }
      else
      {
        if (Game1.content.GetCurrentLanguage() == LocalizedContentManager.LanguageCode.zh)
          Game1.options.loadChineseFonts();
        else
          Game1.options.dialogueFontScale = 1f;
        Game1.options.platformClampValues();
        Game1.options.SaveDefaultOptions();
      }
      try
      {
        StartupPreferences startupPreferences = new StartupPreferences();
        startupPreferences.loadPreferences(false, false);
        Game1.options.gamepadMode = startupPreferences.gamepadMode;
      }
      catch
      {
      }
      Game1.initializeVolumeLevels();
      Game1.multiplayer.latestID = (ulong) SaveGame.loaded.latestID;
      Game1.netWorldState.Value.SkullCavesDifficulty = SaveGame.loaded.skullCavesDifficulty;
      Game1.netWorldState.Value.MinesDifficulty = SaveGame.loaded.minesDifficulty;
      Game1.netWorldState.Value.VisitsUntilY1Guarantee = SaveGame.loaded.visitsUntilY1Guarantee;
      Game1.netWorldState.Value.ShuffleMineChests = SaveGame.loaded.shuffleMineChests;
      Game1.netWorldState.Value.DishOfTheDay = SaveGame.loaded.dishOfTheDay;
      if (Game1.IsRainingHere())
        Game1.changeMusicTrack("rain", true);
      Game1.updateWeatherIcon();
      Game1.netWorldState.Value.MiniShippingBinsObtained = SaveGame.loaded.miniShippingBinsObtained;
      Game1.netWorldState.Value.LostBooksFound = SaveGame.loaded.lostBooksFound;
      Game1.netWorldState.Value.GoldenWalnuts = SaveGame.loaded.goldenWalnuts;
      Game1.netWorldState.Value.GoldenWalnutsFound = SaveGame.loaded.goldenWalnutsFound;
      Game1.netWorldState.Value.GoldenCoconutCracked = SaveGame.loaded.goldenCoconutCracked;
      Game1.netWorldState.Value.FoundBuriedNuts.Clear();
      Game1.netWorldState.Value.FoundBuriedNuts.AddRange<string>((IEnumerable<string>) SaveGame.loaded.foundBuriedNuts);
      Game1.netWorldState.Value.CheckedGarbage.Clear();
      Game1.netWorldState.Value.CheckedGarbage.AddRange<string>((IEnumerable<string>) SaveGame.loaded.checkedGarbage);
      IslandSouth.SetupIslandSchedules();
      Game1.netWorldState.Value.TimesFedRaccoons = SaveGame.loaded.timesFedRaccoons;
      Game1.netWorldState.Value.TreasureTotemsUsed = SaveGame.loaded.treasureTotemsUsed;
      Game1.netWorldState.Value.PerfectionWaivers = SaveGame.loaded.perfectionWaivers;
      Game1.netWorldState.Value.SeasonOfCurrentRacconBundle = SaveGame.loaded.seasonOfCurrentRaccoonBundle;
      Game1.netWorldState.Value.raccoonBundles.Set((IList<bool>) SaveGame.loaded.raccoonBundles);
      Game1.netWorldState.Value.ActivatedGoldenParrot = SaveGame.loaded.activatedGoldenParrot;
      Game1.netWorldState.Value.DaysPlayedWhenLastRaccoonBundleWasFinished = SaveGame.loaded.daysPlayedWhenLastRaccoonBundleWasFinished;
      Game1.PerformPassiveFestivalSetup();
      Game1.player.team.farmhandsCanMoveBuildings.Value = (FarmerTeam.RemoteBuildingPermissions) SaveGame.loaded.moveBuildingPermissionMode;
      Game1.player.team.mineShrineActivated.Value = SaveGame.loaded.mineShrineActivated;
      Game1.player.team.skullShrineActivated.Value = SaveGame.loaded.skullShrineActivated;
      if (Game1.multiplayerMode == (byte) 2)
      {
        if (Program.sdk.Networking != null && Game1.options.serverPrivacy == ServerPrivacy.InviteOnly)
          Game1.options.setServerMode("invite");
        else if (Program.sdk.Networking != null && Game1.options.serverPrivacy == ServerPrivacy.FriendsOnly)
          Game1.options.setServerMode("friends");
        else
          Game1.options.setServerMode("friends");
      }
      Game1.bannedUsers = new SerializableDictionary<string, string>((IDictionary<string, string>) SaveGame.loaded.bannedUsers.ToDictionary<string, string>());
      int num = SaveGame.loaded.lostBooksFound < 0 ? 1 : 0;
      SaveGame.loaded = (SaveGame) null;
      Game1.currentLocation.lastTouchActionLocation = Game1.player.Tile;
      if (Game1.player.horseName.Value == null)
      {
        Horse horse = Utility.findHorse(Guid.Empty);
        if (horse != null && horse.displayName != "")
        {
          Game1.player.horseName.Value = horse.displayName;
          horse.ownerId.Value = Game1.player.UniqueMultiplayerID;
        }
      }
      SaveMigrator.ApplySaveFixes();
      if (num != 0)
        SaveMigrator_1_4.RecalculateLostBookCount();
      foreach (Item obj in Game1.player.Items)
      {
        if (obj is Object @object)
          @object.reloadSprite();
      }
      foreach (Object trinketItem in Game1.player.trinketItems)
        trinketItem.reloadSprite();
      Game1.gameMode = (byte) 3;
      Game1.AddNPCs();
      Game1.AddModNPCs();
      Game1.RefreshQuestOfTheDay();
      try
      {
        Game1.fixProblems();
      }
      catch (Exception ex)
      {
        Game1.log.Error("Failed to fix problems.", ex);
      }
      Utility.ForEachBuilding((Func<Building, bool>) (building =>
      {
        if (building is Stable stable2)
        {
          stable2.grabHorse();
        }
        else
        {
          switch (building.GetIndoors())
          {
            case Cabin cabin2:
              cabin2.updateFarmLayout();
              break;
            case Shed interior2:
              interior2.updateLayout();
              building.updateInteriorWarps((GameLocation) interior2);
              break;
          }
        }
        return true;
      }));
      Game1.UpdateHorseOwnership();
      Game1.UpdateFarmPerfection();
      Game1.doMorningStuff();
      if (flag && Game1.player.currentLocation is FarmHouse currentLocation)
        Game1.player.Position = Utility.PointToVector2(currentLocation.GetPlayerBedSpot()) * 64f;
      BedFurniture.ShiftPositionForBed(Game1.player);
      Game1.stats.checkForAchievements();
      if (Game1.IsMasterGame)
        Game1.netWorldState.Value.UpdateFromGame1();
      SaveGame.LogVerbose($"getLoadEnumerator() exited, elapsed = '{stopwatch.Elapsed.ToString()}'");
      if (SaveGame.CancelToTitle)
        Game1.ExitToTitle();
      SaveGame.IsProcessing = false;
      Game1.player.currentLocation.lastTouchActionLocation = Game1.player.Tile;
      if (Game1.IsMasterGame)
      {
        Game1.player.currentLocation.hostSetup();
        Game1.player.currentLocation.interiorDoors.ResetSharedState();
      }
      Game1.player.currentLocation.resetForPlayerEntry();
      Game1.player.sleptInTemporaryBed.Value = false;
      Game1.player.showToolUpgradeAvailability();
      Game1.player.resetAllTrinketEffects();
      Game1.dayTimeMoneyBox.questsDirty = true;
      yield return 100;
    }
  }

  public static void loadDataToFarmer(Farmer target)
  {
    Farmer farmer = target;
    target.gameVersion = farmer.gameVersion;
    target.Items.OverwriteWith((IList<Item>) farmer.Items);
    target.canMove = true;
    target.Sprite = (AnimatedSprite) new FarmerSprite((string) null);
    target.songsHeard.Add("title_day");
    target.songsHeard.Add("title_night");
    target.maxItems.Value = farmer.maxItems.Value;
    for (int index = 0; index < target.maxItems.Value; ++index)
    {
      if (target.Items.Count <= index)
        target.Items.Add((Item) null);
    }
    if (target.FarmerRenderer == null)
      target.FarmerRenderer = new FarmerRenderer(target.getTexture(), target);
    target.changeGender(farmer.IsMale);
    target.changeAccessory(farmer.accessory.Value);
    target.changeShirt(farmer.shirt.Value);
    target.changePantsColor(farmer.GetPantsColor());
    target.changeSkinColor(farmer.skin.Value);
    target.changeHairColor(farmer.hairstyleColor.Value);
    target.changeHairStyle(farmer.hair.Value);
    target.changeShoeColor(farmer.shoes.Value);
    target.changeEyeColor(farmer.newEyeColor.Value);
    target.Stamina = farmer.Stamina;
    target.health = farmer.health;
    target.maxStamina.Value = farmer.maxStamina.Value;
    target.mostRecentBed = farmer.mostRecentBed;
    target.Position = target.mostRecentBed;
    target.position.X -= 64f;
    if (!Game1.hasApplied1_3_UpdateChanges)
      SaveMigrator_1_3.MigrateFriendshipData(target);
    target.questLog.RemoveWhere((Func<Quest, bool>) (quest => quest == null));
    target.ConvertClothingOverrideToClothesItems();
    target.UpdateClothing();
    target._lastEquippedTool = target.CurrentTool;
  }

  public static void loadDataToLocations(List<GameLocation> fromLocations)
  {
    Dictionary<string, string> formerLocationNames = SaveGame.GetFormerLocationNames();
    if (formerLocationNames.Count > 0)
    {
      foreach (GameLocation fromLocation in fromLocations)
      {
        foreach (NPC character in fromLocation.characters)
        {
          string defaultMap = character.DefaultMap;
          string str;
          if (defaultMap != null && formerLocationNames.TryGetValue(defaultMap, out str))
          {
            SaveGame.LogDebug($"Updated {character.Name}'s home from '{defaultMap}' to '{str}'.");
            character.DefaultMap = str;
          }
        }
      }
    }
    Game1.netWorldState.Value.ParrotPlatformsUnlocked = SaveGame.loaded.parrotPlatformsUnlocked;
    Game1.player.team.farmPerfect.Value = SaveGame.loaded.farmPerfect;
    List<GameLocation> gameLocationList = new List<GameLocation>();
    Dictionary<string, Tuple<NPC, GameLocation>> lostVillagers = new Dictionary<string, Tuple<NPC, GameLocation>>();
    foreach (GameLocation fromLocation in fromLocations)
    {
      GameLocation location = Game1.getLocationFromName(fromLocation.name.Value);
      if (location == null)
      {
        if (fromLocation is Cellar)
        {
          location = Game1.CreateGameLocation("Cellar");
          if (location == null)
          {
            SaveGame.LogError("Couldn't create 'Cellar' location. Was it removed from Data/Locations?");
            continue;
          }
          location.name.Value = fromLocation.name.Value;
          Game1.locations.Add(location);
        }
        string name;
        if (location == null && formerLocationNames.TryGetValue(fromLocation.name.Value, out name))
        {
          SaveGame.LogDebug($"Mapped legacy location '{fromLocation.Name}' to '{name}'.");
          location = Game1.getLocationFromName(name);
        }
        if (location == null)
        {
          List<string> source = new List<string>();
          foreach (NPC character in fromLocation.characters)
          {
            if (character.IsVillager && character.Name != null)
            {
              source.Add(character.Name);
              lostVillagers[character.Name] = Tuple.Create<NPC, GameLocation>(character, fromLocation);
            }
          }
          IGameLogger log = Game1.log;
          DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(41, 2);
          interpolatedStringHandler.AppendLiteral("Ignored unknown location '");
          interpolatedStringHandler.AppendFormatted(fromLocation.NameOrUniqueName);
          interpolatedStringHandler.AppendLiteral("' in save data");
          ref DefaultInterpolatedStringHandler local = ref interpolatedStringHandler;
          string str;
          if (source.Count <= 0)
            str = "";
          else
            str = $", including NPC{(source.Count > 1 ? "s" : "")} '{string.Join("', '", (IEnumerable<string>) source.OrderBy<string, string>((Func<string, string>) (p => p)))}'";
          local.AppendFormatted(str);
          interpolatedStringHandler.AppendLiteral(".");
          string stringAndClear = interpolatedStringHandler.ToStringAndClear();
          log.Warn(stringAndClear);
          continue;
        }
      }
      if (!(location is Farm farm2))
      {
        if (!(location is FarmHouse farmHouse2))
        {
          if (!(location is Forest forest2))
          {
            if (!(location is MovieTheater movieTheater2))
            {
              if (!(location is Town town2))
              {
                if (!(location is Beach beach2))
                {
                  if (!(location is Woods woods2))
                  {
                    if (!(location is CommunityCenter communityCenter2))
                    {
                      if (location is ShopLocation shopLocation1 && fromLocation is ShopLocation shopLocation2)
                      {
                        shopLocation1.itemsFromPlayerToSell.MoveFrom((NetList<Item, NetRef<Item>>) shopLocation2.itemsFromPlayerToSell);
                        shopLocation1.itemsToStartSellingTomorrow.MoveFrom((NetList<Item, NetRef<Item>>) shopLocation2.itemsToStartSellingTomorrow);
                      }
                    }
                    else if (fromLocation is CommunityCenter communityCenter1)
                      communityCenter2.areasComplete.Set((IList<bool>) communityCenter1.areasComplete);
                  }
                  else if (fromLocation is Woods woods1)
                    woods2.hasUnlockedStatue.Value = woods1.hasUnlockedStatue.Value;
                }
                else if (fromLocation is Beach beach1)
                  beach2.bridgeFixed.Value = beach1.bridgeFixed.Value;
              }
              else if (fromLocation is Town town1)
                town2.daysUntilCommunityUpgrade.Value = town1.daysUntilCommunityUpgrade.Value;
            }
            else if (fromLocation is MovieTheater movieTheater1)
              movieTheater2.dayFirstEntered.Set(movieTheater1.dayFirstEntered.Value);
          }
          else if (fromLocation is Forest forest1)
          {
            forest2.stumpFixed.Value = forest1.stumpFixed.Value;
            forest2.obsolete_log = forest1.obsolete_log;
          }
        }
        else if (fromLocation is FarmHouse farmHouse1)
        {
          farmHouse2.setMapForUpgradeLevel(farmHouse2.upgradeLevel);
          farmHouse2.fridge.Value = farmHouse1.fridge.Value;
          farmHouse2.ReadWallpaperAndFloorTileData();
        }
      }
      else if (fromLocation is Farm farm1)
      {
        farm2.greenhouseUnlocked.Value = farm1.greenhouseUnlocked.Value;
        farm2.greenhouseMoved.Value = farm1.greenhouseMoved.Value;
        farm2.hasSeenGrandpaNote = farm1.hasSeenGrandpaNote;
        farm2.grandpaScore.Value = farm1.grandpaScore.Value;
        farm2.UpdatePatio();
      }
      location.TransferDataFromSavedLocation(fromLocation);
      location.animals.MoveFrom(fromLocation.animals);
      location.buildings.Set((ICollection<Building>) fromLocation.buildings);
      location.characters.Set((ICollection<NPC>) fromLocation.characters);
      location.furniture.Set((ICollection<Furniture>) fromLocation.furniture);
      location.largeTerrainFeatures.Set((ICollection<LargeTerrainFeature>) fromLocation.largeTerrainFeatures);
      location.miniJukeboxCount.Value = fromLocation.miniJukeboxCount.Value;
      location.miniJukeboxTrack.Value = fromLocation.miniJukeboxTrack.Value;
      location.netObjects.Set((IEnumerable<KeyValuePair<Vector2, Object>>) fromLocation.netObjects.Pairs);
      location.numberOfSpawnedObjectsOnMap = fromLocation.numberOfSpawnedObjectsOnMap;
      location.piecesOfHay.Value = fromLocation.piecesOfHay.Value;
      location.resourceClumps.Set((ICollection<ResourceClump>) new List<ResourceClump>((IEnumerable<ResourceClump>) fromLocation.resourceClumps));
      location.terrainFeatures.Set((IEnumerable<KeyValuePair<Vector2, TerrainFeature>>) fromLocation.terrainFeatures.Pairs);
      if (!SaveGame.loaded.HasSaveFix(SaveFixes.MigrateBuildingsToData))
        SaveMigrator_1_6.ConvertBuildingsToData(location);
      gameLocationList.Add(location);
    }
    SaveGame.MigrateLostVillagers(lostVillagers);
    foreach (GameLocation gameLocation in gameLocationList)
    {
      gameLocation.AddDefaultBuildings(false);
      foreach (Building building in gameLocation.buildings)
      {
        building.load();
        if (building.GetIndoorsType() == IndoorsType.Instanced)
          building.GetIndoors()?.addLightGlows();
      }
      foreach (FarmAnimal farmAnimal in gameLocation.animals.Values)
        farmAnimal.reload((GameLocation) null);
      foreach (Furniture furniture in gameLocation.furniture)
        furniture.updateDrawPosition();
      foreach (LargeTerrainFeature largeTerrainFeature in gameLocation.largeTerrainFeatures)
      {
        largeTerrainFeature.Location = gameLocation;
        largeTerrainFeature.loadSprite();
      }
      foreach (TerrainFeature terrainFeature in gameLocation.terrainFeatures.Values)
      {
        terrainFeature.Location = gameLocation;
        terrainFeature.loadSprite();
        if (terrainFeature is HoeDirt hoeDirt)
          hoeDirt.updateNeighbors();
      }
      foreach (KeyValuePair<Vector2, Object> pair in gameLocation.objects.Pairs)
      {
        pair.Value.initializeLightSource(pair.Key);
        pair.Value.reloadSprite();
      }
      gameLocation.addLightGlows();
      if (!(gameLocation is IslandLocation islandLocation))
      {
        if (gameLocation is FarmCave farmCave)
          farmCave.UpdateReadyFlag();
      }
      else
        islandLocation.AddAdditionalWalnutBushes();
    }
    Utility.ForEachLocation((Func<GameLocation, bool>) (location =>
    {
      if (location.characters.Count > 0)
      {
        foreach (NPC c in location.characters.ToArray<NPC>())
        {
          SaveGame.initializeCharacter(c, location);
          c.reloadSprite();
        }
      }
      return true;
    }));
    Game1.player.currentLocation = (GameLocation) Utility.getHomeOfFarmer(Game1.player);
  }

  public static void initializeCharacter(NPC c, GameLocation location)
  {
    c.currentLocation = location;
    c.reloadData();
    if (c.DefaultPosition.Equals(Vector2.Zero))
      return;
    c.Position = c.DefaultPosition;
  }

  /// <summary>Migrate villager NPCs from the save file based on their <see cref="T:StardewValley.GameData.Characters.CharacterData" /> data.</summary>
  /// <param name="lostVillagers">The villager NPCs from the save data which were in a location that no longer exists.</param>
  public static void MigrateLostVillagers(
    Dictionary<string, Tuple<NPC, GameLocation>> lostVillagers)
  {
    Dictionary<string, string> formerNpcNames = SaveGame.GetFormerNpcNames((Func<string, CharacterData, bool>) ((newName, _) => Game1.getCharacterFromName(newName) == null));
    foreach (KeyValuePair<string, Tuple<NPC, GameLocation>> lostVillager in lostVillagers)
    {
      NPC npc = lostVillager.Value.Item1;
      GameLocation gameLocation = lostVillager.Value.Item2;
      string name1;
      if (Game1.getCharacterFromName(npc.Name) == null && (!formerNpcNames.TryGetValue(npc.Name, out name1) || Game1.getCharacterFromName(name1) == null) && NPC.TryGetData(name1 ?? npc.Name, out CharacterData _))
      {
        string name2 = npc.Name;
        npc.Name = name1 ?? name2;
        string defaultMap = npc.DefaultMap;
        npc.reloadDefaultLocation();
        GameLocation home;
        try
        {
          home = npc.getHome();
        }
        catch (Exception ex)
        {
          continue;
        }
        npc.Name = name2;
        if (home != null)
        {
          home.characters.Add(npc);
          npc.currentLocation = home;
          npc.position.Value = npc.DefaultPosition * 64f;
          Game1.log.Debug($"Moved NPC '{npc.Name}' from deleted location '{gameLocation.Name}' to their new home in '{npc.currentLocation.Name}'.");
        }
      }
    }
    foreach (KeyValuePair<string, string> keyValuePair in formerNpcNames)
    {
      string key1 = keyValuePair.Key;
      string key2 = keyValuePair.Value;
      NPC characterFromName = Game1.getCharacterFromName(key1);
      if (characterFromName != null)
      {
        characterFromName.Name = key2;
        foreach (Farmer allFarmer in Game1.getAllFarmers())
        {
          if (allFarmer.spouse == key1)
            allFarmer.spouse = key2;
          Friendship friendship;
          if (allFarmer.friendshipData.TryGetValue(key1, out friendship))
          {
            allFarmer.friendshipData.Remove(key1);
            allFarmer.friendshipData.TryAdd(key2, friendship);
          }
          SerializableDictionary<string, int> serializableDictionary;
          if (allFarmer.giftedItems.TryGetValue(key1, out serializableDictionary))
          {
            allFarmer.giftedItems.Remove(key1);
            allFarmer.giftedItems.TryAdd(key2, serializableDictionary);
          }
        }
        Game1.log.Debug($"Migrated legacy NPC '{key1}' in save data to '{key2}'.");
      }
    }
  }

  /// <summary>Get a lookup of former → new location names based on their <see cref="F:StardewValley.GameData.Locations.LocationData.FormerLocationNames" /> field.</summary>
  public static Dictionary<string, string> GetFormerLocationNames()
  {
    Dictionary<string, string> formerLocationNames1 = new Dictionary<string, string>();
    foreach (KeyValuePair<string, LocationData> keyValuePair in (IEnumerable<KeyValuePair<string, LocationData>>) Game1.locationData)
    {
      LocationData locationData = keyValuePair.Value;
      List<string> formerLocationNames2 = locationData.FormerLocationNames;
      // ISSUE: explicit non-virtual call
      if ((formerLocationNames2 != null ? (__nonvirtual (formerLocationNames2.Count) > 0 ? 1 : 0) : 0) != 0)
      {
        foreach (string formerLocationName in locationData.FormerLocationNames)
        {
          if (Game1.locationData.ContainsKey(formerLocationName))
          {
            SaveGame.LogError($"Location '{keyValuePair.Key}' in Data/Locations has former name '{formerLocationName}', which can't be added because there's a location with that ID in Data/Locations.");
          }
          else
          {
            string str;
            if (formerLocationNames1.TryGetValue(formerLocationName, out str))
            {
              if (str != keyValuePair.Key)
                SaveGame.LogError($"Location '{keyValuePair.Key}' in Data/Locations has former name '{formerLocationName}', which can't be added because that name is already mapped to '{str}'.");
            }
            else
              formerLocationNames1[formerLocationName] = keyValuePair.Key;
          }
        }
      }
    }
    return formerLocationNames1;
  }

  /// <summary>Get a lookup of former → new NPC names based on their <see cref="F:StardewValley.GameData.Characters.CharacterData.FormerCharacterNames" /> field.</summary>
  /// <param name="filter">A filter to apply to the list of NPCs with former names.</param>
  public static Dictionary<string, string> GetFormerNpcNames(
    Func<string, CharacterData, bool> filter)
  {
    Dictionary<string, string> formerNpcNames = new Dictionary<string, string>();
    foreach (KeyValuePair<string, CharacterData> keyValuePair in (IEnumerable<KeyValuePair<string, CharacterData>>) Game1.characterData)
    {
      CharacterData characterData = keyValuePair.Value;
      List<string> formerCharacterNames = characterData.FormerCharacterNames;
      // ISSUE: explicit non-virtual call
      if ((formerCharacterNames != null ? (__nonvirtual (formerCharacterNames.Count) > 0 ? 1 : 0) : 0) != 0 && filter(keyValuePair.Key, characterData))
      {
        foreach (string formerCharacterName in characterData.FormerCharacterNames)
        {
          if (Game1.characterData.ContainsKey(formerCharacterName))
          {
            SaveGame.LogError($"NPC '{keyValuePair.Key}' in Data/Characters has former name '{formerCharacterName}', which can't be added because there's an NPC with that ID in Data/Characters.");
          }
          else
          {
            string str;
            if (formerNpcNames.TryGetValue(formerCharacterName, out str))
            {
              if (str != keyValuePair.Key)
                SaveGame.LogError($"NPC '{keyValuePair.Key}' in Data/Characters has former name '{formerCharacterName}', which can't be added because that name is already mapped to '{str}'.");
            }
            else
              formerNpcNames[formerCharacterName] = keyValuePair.Key;
          }
        }
      }
    }
    return formerNpcNames;
  }

  /// <inheritdoc cref="M:StardewValley.Logging.IGameLogger.Verbose(System.String)" />
  private static void LogVerbose(string message) => Game1.log.Verbose(message);

  /// <inheritdoc cref="M:StardewValley.Logging.IGameLogger.Debug(System.String)" />
  private static void LogDebug(string message) => Game1.log.Debug(message);

  /// <inheritdoc cref="M:StardewValley.Logging.IGameLogger.Warn(System.String)" />
  private static void LogWarn(string message) => Game1.log.Warn(message);

  /// <inheritdoc cref="M:StardewValley.Logging.IGameLogger.Error(System.String,System.Exception)" />
  private static void LogError(string message, Exception exception = null)
  {
    Game1.log.Error(message, exception);
  }
}
