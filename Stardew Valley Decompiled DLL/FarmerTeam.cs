// Decompiled with JetBrains decompiler
// Type: StardewValley.FarmerTeam
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Netcode;
using StardewValley.BellsAndWhistles;
using StardewValley.Buildings;
using StardewValley.Characters;
using StardewValley.GameData.Shops;
using StardewValley.GameData.SpecialOrders;
using StardewValley.Inventories;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Minigames;
using StardewValley.Network;
using StardewValley.Network.ChestHit;
using StardewValley.Network.NetEvents;
using StardewValley.Objects;
using StardewValley.Pathfinding;
using StardewValley.Quests;
using StardewValley.SpecialOrders;
using StardewValley.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

#nullable disable
namespace StardewValley;

public class FarmerTeam : INetObject<NetFields>
{
  /// <summary>The unique ID in <see cref="F:StardewValley.FarmerTeam.globalInventories" /> for the crow shop which sells lost unique items.</summary>
  public const string GlobalInventoryId_LostItemsShop = "LostItemsShop";
  /// <summary>The unique ID in <see cref="F:StardewValley.FarmerTeam.globalInventories" /> for Junimo chests.</summary>
  public const string GlobalInventoryId_JunimoChest = "JunimoChests";
  public readonly NetIntDelta money;
  public readonly NetLongDictionary<NetIntDelta, NetRef<NetIntDelta>> individualMoney;
  public readonly NetIntDelta totalMoneyEarned;
  public readonly NetBool useSeparateWallets;
  public readonly NetBool newLostAndFoundItems;
  public readonly NetBool toggleMineShrineOvernight;
  public readonly NetBool mineShrineActivated;
  public readonly NetBool toggleSkullShrineOvernight;
  public readonly NetBool skullShrineActivated;
  public readonly NetBool farmPerfect;
  public readonly NetList<string, NetString> specialRulesRemovedToday;
  /// <summary>The unqualified item IDs to remove everywhere in the world when the current day ends.</summary>
  public readonly NetList<string, NetString> itemsToRemoveOvernight;
  /// <summary>The mail IDs to remove from all players when the current day ends.</summary>
  public readonly NetList<string, NetString> mailToRemoveOvernight;
  public NetIntDictionary<long, NetLong> cellarAssignments;
  /// <summary>The mail IDs that have been broadcast globally.</summary>
  public NetStringHashSet broadcastedMail;
  /// <summary>The building type IDs which have been constructed.</summary>
  public readonly NetStringHashSet constructedBuildings;
  public NetStringHashSet collectedNutTracker;
  /// <summary>The special order IDs which were previously completed.</summary>
  public NetStringHashSet completedSpecialOrders;
  /// <summary>The special orders which are currently active.</summary>
  public NetList<SpecialOrder, NetRef<SpecialOrder>> specialOrders;
  /// <summary>The special orders which are currently available to choose from, across all special order boards.</summary>
  public NetList<SpecialOrder, NetRef<SpecialOrder>> availableSpecialOrders;
  /// <summary>The order board types for the active special order.</summary>
  public NetStringHashSet acceptedSpecialOrderTypes;
  public readonly NetCollection<Item> returnedDonations;
  /// <summary>The synchronizer that prevents race conditions when multiplayer players hit a chest.</summary>
  internal readonly ChestHitSynchronizer chestHit;
  /// <summary>The global inventories for special containers like Junimo chests.</summary>
  /// <remarks>
  ///   <para>The vanilla keys have constants like <see cref="F:StardewValley.FarmerTeam.GlobalInventoryId_JunimoChest" />.</para>
  /// 
  ///   <para>Most code should call <see cref="M:StardewValley.FarmerTeam.GetOrCreateGlobalInventory(System.String)" /> instead of accessing this field directly.</para>
  /// </remarks>
  public readonly NetStringDictionary<Inventory, NetRef<Inventory>> globalInventories;
  /// <summary>The mutexes which prevent multiple players from opening the same global inventory at once.</summary>
  public readonly NetStringDictionary<NetMutex, NetRef<NetMutex>> globalInventoryMutexes;
  public readonly NetFarmerCollection announcedSleepingFarmers;
  public readonly NetEnum<FarmerTeam.SleepAnnounceModes> sleepAnnounceMode;
  public readonly NetEnum<FarmerTeam.RemoteBuildingPermissions> farmhandsCanMoveBuildings;
  private readonly NetLongDictionary<Proposal, NetRef<Proposal>> proposals;
  public readonly NetList<MovieInvitation, NetRef<MovieInvitation>> movieInvitations;
  public readonly NetCollection<Item> luauIngredients;
  public readonly NetCollection<Item> grangeDisplay;
  public readonly NetMutex grangeMutex;
  public readonly NetMutex returnedDonationsMutex;
  public readonly NetMutex ordersBoardMutex;
  public readonly NetMutex qiChallengeBoardMutex;
  private readonly NetEvent1Field<Rectangle, NetRectangle> festivalPropRemovalEvent;
  public readonly NetEvent1Field<int, NetInt> addQiGemsToTeam;
  public readonly NetEvent1Field<string, NetString> addCharacterEvent;
  public readonly NetEvent1Field<string, NetString> requestAddCharacterEvent;
  public readonly NetEvent0 requestLeoMove;
  /// <summary>An event raised when a mine area needs to kick players.</summary>
  public readonly NetEvent1Field<int, NetInt> kickOutOfMinesEvent;
  public readonly NetEvent1Field<string, NetString> requestNPCGoHome;
  public readonly NetEvent1Field<long, NetLong> requestSpouseSleepEvent;
  public readonly NetEvent1Field<string, NetString> ringPhoneEvent;
  public readonly NetEvent1Field<long, NetLong> requestHorseWarpEvent;
  public readonly NetEvent1Field<long, NetLong> requestPetWarpHomeEvent;
  public readonly NetEvent1Field<long, NetLong> requestMovieEndEvent;
  public readonly NetEvent1Field<long, NetLong> endMovieEvent;
  /// <summary>An event raised when a building is placed.</summary>
  public readonly NetEventBinary buildingConstructedEvent;
  /// <summary>An event raised when a building is moved.</summary>
  public readonly NetEventBinary buildingMovedEvent;
  /// <summary>An event raised when a building is demolished.</summary>
  public readonly NetEventBinary buildingDemolishedEvent;
  public readonly NetStringDictionary<int, NetInt> limitedNutDrops;
  /// <summary>An event raised when a nut should be dropped.</summary>
  private readonly NetEvent1<NutDropRequest> requestNutDrop;
  /// <summary>An event raised when an action needs to set a simple flag (e.g. event seen or song heard) for a group of players.</summary>
  private readonly NetEvent1<SetSimpleFlagRequest> requestSetSimpleFlag;
  /// <summary>An event raised to add or remove mail for a group of players.</summary>
  private readonly NetEvent1<SetMailRequest> requestSetMail;
  public readonly NetFarmerPairDictionary<Friendship, NetRef<Friendship>> friendshipData;
  public readonly NetWitnessedLock demolishLock;
  public readonly NetMutex buildLock;
  public readonly NetMutex movieMutex;
  public readonly NetMutex goldenCoconutMutex;
  public readonly SynchronizedShopStock synchronizedShopStock;
  public readonly NetLong theaterBuildDate;
  public readonly NetInt lastDayQueenOfSauceRerunUpdated;
  public readonly NetInt queenOfSauceRerunWeek;
  public readonly NetDouble sharedDailyLuck;
  public readonly NetBool spawnMonstersAtNight;
  /// <summary>When the game makes a random choice, whether to use a simpler method that's prone to repeating patterns.</summary>
  /// <remarks>This is mainly intended for speedrunning, where full randomization might be undesirable. Most code should use <see cref="P:StardewValley.Game1.UseLegacyRandom" /> instead.</remarks>
  public readonly NetBool useLegacyRandom;
  /// <summary>Whether players should be allowed to use cheat commands in the in-game chat.</summary>
  /// <remarks>This is a low-level field. See <see cref="F:StardewValley.Program.enableCheats" /> instead to enable them locally, and <see cref="!:ChatBox.AllowCheats" /> to check if they're enabled.</remarks>
  internal readonly NetBool allowChatCheats;
  /// <summary>Whether the game is hosted by an automated dedicated server host.</summary>
  /// <remarks>This is a low-level field. See <see cref="!:ChatBox.HasDedicatedHost" /> to check for a dedicated server host.</remarks>
  internal readonly NetBool hasDedicatedHost;
  public readonly NetInt calicoEggSkullCavernRating;
  /// <summary>The highest Calico Egg Rating reached by any player today.</summary>
  public readonly NetInt highestCalicoEggRatingToday;
  /// <summary>The Calico Statue effects currently applied, where the key is an effect ID like <see cref="!:DesertFestival.CALICO_STATUE_BAT_INVASION" /> and the key is the number of that effect currently applied.</summary>
  public readonly NetIntDictionary<int, NetInt> calicoStatueEffects;
  public readonly NetLeaderboards junimoKartScores;
  public PlayerStatusList junimoKartStatus;
  public PlayerStatusList endOfNightStatus;
  public PlayerStatusList festivalScoreStatus;
  public PlayerStatusList sleepStatus;

  public NetFields NetFields { get; }

  public FarmerTeam()
  {
    NetEvent1Field<string, NetString> netEvent1Field1 = new NetEvent1Field<string, NetString>();
    netEvent1Field1.InterpolationWait = false;
    this.requestNPCGoHome = netEvent1Field1;
    NetEvent1Field<long, NetLong> netEvent1Field2 = new NetEvent1Field<long, NetLong>();
    netEvent1Field2.InterpolationWait = false;
    this.requestSpouseSleepEvent = netEvent1Field2;
    this.ringPhoneEvent = new NetEvent1Field<string, NetString>();
    NetEvent1Field<long, NetLong> netEvent1Field3 = new NetEvent1Field<long, NetLong>();
    netEvent1Field3.InterpolationWait = false;
    this.requestHorseWarpEvent = netEvent1Field3;
    NetEvent1Field<long, NetLong> netEvent1Field4 = new NetEvent1Field<long, NetLong>();
    netEvent1Field4.InterpolationWait = false;
    this.requestPetWarpHomeEvent = netEvent1Field4;
    this.requestMovieEndEvent = new NetEvent1Field<long, NetLong>();
    this.endMovieEvent = new NetEvent1Field<long, NetLong>();
    this.buildingConstructedEvent = new NetEventBinary();
    this.buildingMovedEvent = new NetEventBinary();
    this.buildingDemolishedEvent = new NetEventBinary();
    this.limitedNutDrops = new NetStringDictionary<int, NetInt>();
    this.requestNutDrop = new NetEvent1<NutDropRequest>();
    this.requestSetSimpleFlag = new NetEvent1<SetSimpleFlagRequest>();
    this.requestSetMail = new NetEvent1<SetMailRequest>();
    this.friendshipData = new NetFarmerPairDictionary<Friendship, NetRef<Friendship>>();
    this.demolishLock = new NetWitnessedLock();
    this.buildLock = new NetMutex();
    this.movieMutex = new NetMutex();
    this.goldenCoconutMutex = new NetMutex();
    this.synchronizedShopStock = new SynchronizedShopStock();
    this.theaterBuildDate = new NetLong(-1L);
    this.lastDayQueenOfSauceRerunUpdated = new NetInt(0);
    this.queenOfSauceRerunWeek = new NetInt(1);
    this.sharedDailyLuck = new NetDouble(1.0 / 1000.0);
    this.spawnMonstersAtNight = new NetBool(false);
    this.useLegacyRandom = new NetBool(false);
    this.allowChatCheats = new NetBool(false);
    this.hasDedicatedHost = new NetBool(false);
    this.calicoEggSkullCavernRating = new NetInt(0);
    this.highestCalicoEggRatingToday = new NetInt(0);
    this.calicoStatueEffects = new NetIntDictionary<int, NetInt>();
    this.junimoKartScores = new NetLeaderboards();
    this.NetFields = new NetFields(nameof (FarmerTeam));
    this.junimoKartStatus = new PlayerStatusList();
    this.endOfNightStatus = new PlayerStatusList();
    this.festivalScoreStatus = new PlayerStatusList();
    this.sleepStatus = new PlayerStatusList();
    // ISSUE: explicit constructor call
    base.\u002Ector();
    this.NetFields.SetOwner((INetObject<NetFields>) this).AddField((INetSerializable) this.money, nameof (money)).AddField((INetSerializable) this.totalMoneyEarned, nameof (totalMoneyEarned)).AddField((INetSerializable) this.proposals, nameof (proposals)).AddField((INetSerializable) this.luauIngredients, nameof (luauIngredients)).AddField((INetSerializable) this.grangeDisplay, nameof (grangeDisplay)).AddField((INetSerializable) this.grangeMutex.NetFields, "grangeMutex.NetFields").AddField((INetSerializable) this.festivalPropRemovalEvent, nameof (festivalPropRemovalEvent)).AddField((INetSerializable) this.friendshipData, nameof (friendshipData)).AddField((INetSerializable) this.demolishLock.NetFields, "demolishLock.NetFields").AddField((INetSerializable) this.buildLock.NetFields, "buildLock.NetFields").AddField((INetSerializable) this.movieInvitations, nameof (movieInvitations)).AddField((INetSerializable) this.movieMutex.NetFields, "movieMutex.NetFields").AddField((INetSerializable) this.requestMovieEndEvent, nameof (requestMovieEndEvent)).AddField((INetSerializable) this.endMovieEvent, nameof (endMovieEvent)).AddField((INetSerializable) this.requestSpouseSleepEvent, nameof (requestSpouseSleepEvent)).AddField((INetSerializable) this.requestNPCGoHome, nameof (requestNPCGoHome)).AddField((INetSerializable) this.useSeparateWallets, nameof (useSeparateWallets)).AddField((INetSerializable) this.individualMoney, nameof (individualMoney)).AddField((INetSerializable) this.announcedSleepingFarmers.NetFields, "announcedSleepingFarmers.NetFields").AddField((INetSerializable) this.sleepAnnounceMode, nameof (sleepAnnounceMode)).AddField((INetSerializable) this.theaterBuildDate, nameof (theaterBuildDate)).AddField((INetSerializable) this.buildingConstructedEvent, nameof (buildingConstructedEvent)).AddField((INetSerializable) this.buildingMovedEvent, nameof (buildingMovedEvent)).AddField((INetSerializable) this.buildingDemolishedEvent, nameof (buildingDemolishedEvent)).AddField((INetSerializable) this.queenOfSauceRerunWeek, nameof (queenOfSauceRerunWeek)).AddField((INetSerializable) this.lastDayQueenOfSauceRerunUpdated, nameof (lastDayQueenOfSauceRerunUpdated)).AddField((INetSerializable) this.broadcastedMail, nameof (broadcastedMail)).AddField((INetSerializable) this.constructedBuildings, nameof (constructedBuildings)).AddField((INetSerializable) this.sharedDailyLuck, nameof (sharedDailyLuck)).AddField((INetSerializable) this.spawnMonstersAtNight, nameof (spawnMonstersAtNight)).AddField((INetSerializable) this.useLegacyRandom, nameof (useLegacyRandom)).AddField((INetSerializable) this.allowChatCheats, nameof (allowChatCheats)).AddField((INetSerializable) this.hasDedicatedHost, nameof (hasDedicatedHost)).AddField((INetSerializable) this.junimoKartScores.NetFields, "junimoKartScores.NetFields").AddField((INetSerializable) this.cellarAssignments, nameof (cellarAssignments)).AddField((INetSerializable) this.synchronizedShopStock.NetFields, "synchronizedShopStock.NetFields").AddField((INetSerializable) this.junimoKartStatus.NetFields, "junimoKartStatus.NetFields").AddField((INetSerializable) this.endOfNightStatus.NetFields, "endOfNightStatus.NetFields").AddField((INetSerializable) this.festivalScoreStatus.NetFields, "festivalScoreStatus.NetFields").AddField((INetSerializable) this.sleepStatus.NetFields, "sleepStatus.NetFields").AddField((INetSerializable) this.farmhandsCanMoveBuildings, nameof (farmhandsCanMoveBuildings)).AddField((INetSerializable) this.requestPetWarpHomeEvent, nameof (requestPetWarpHomeEvent)).AddField((INetSerializable) this.ringPhoneEvent, nameof (ringPhoneEvent)).AddField((INetSerializable) this.specialOrders, nameof (specialOrders)).AddField((INetSerializable) this.returnedDonations, nameof (returnedDonations)).AddField((INetSerializable) this.returnedDonationsMutex.NetFields, "returnedDonationsMutex.NetFields").AddField((INetSerializable) this.goldenCoconutMutex.NetFields, "goldenCoconutMutex.NetFields").AddField((INetSerializable) this.requestNutDrop, nameof (requestNutDrop)).AddField((INetSerializable) this.requestSetSimpleFlag, nameof (requestSetSimpleFlag)).AddField((INetSerializable) this.requestSetMail, nameof (requestSetMail)).AddField((INetSerializable) this.limitedNutDrops, nameof (limitedNutDrops)).AddField((INetSerializable) this.availableSpecialOrders, nameof (availableSpecialOrders)).AddField((INetSerializable) this.acceptedSpecialOrderTypes, nameof (acceptedSpecialOrderTypes)).AddField((INetSerializable) this.ordersBoardMutex.NetFields, "ordersBoardMutex.NetFields").AddField((INetSerializable) this.qiChallengeBoardMutex.NetFields, "qiChallengeBoardMutex.NetFields").AddField((INetSerializable) this.completedSpecialOrders, nameof (completedSpecialOrders)).AddField((INetSerializable) this.addCharacterEvent, nameof (addCharacterEvent)).AddField((INetSerializable) this.requestAddCharacterEvent, nameof (requestAddCharacterEvent)).AddField((INetSerializable) this.requestLeoMove, nameof (requestLeoMove)).AddField((INetSerializable) this.collectedNutTracker, nameof (collectedNutTracker)).AddField((INetSerializable) this.itemsToRemoveOvernight, nameof (itemsToRemoveOvernight)).AddField((INetSerializable) this.mailToRemoveOvernight, nameof (mailToRemoveOvernight)).AddField((INetSerializable) this.newLostAndFoundItems, nameof (newLostAndFoundItems)).AddField((INetSerializable) this.globalInventories, nameof (globalInventories)).AddField((INetSerializable) this.globalInventoryMutexes, nameof (globalInventoryMutexes)).AddField((INetSerializable) this.requestHorseWarpEvent, nameof (requestHorseWarpEvent)).AddField((INetSerializable) this.kickOutOfMinesEvent, nameof (kickOutOfMinesEvent)).AddField((INetSerializable) this.toggleMineShrineOvernight, nameof (toggleMineShrineOvernight)).AddField((INetSerializable) this.mineShrineActivated, nameof (mineShrineActivated)).AddField((INetSerializable) this.toggleSkullShrineOvernight, nameof (toggleSkullShrineOvernight)).AddField((INetSerializable) this.skullShrineActivated, nameof (skullShrineActivated)).AddField((INetSerializable) this.specialRulesRemovedToday, nameof (specialRulesRemovedToday)).AddField((INetSerializable) this.addQiGemsToTeam, nameof (addQiGemsToTeam)).AddField((INetSerializable) this.farmPerfect, nameof (farmPerfect)).AddField((INetSerializable) this.calicoEggSkullCavernRating, nameof (calicoEggSkullCavernRating)).AddField((INetSerializable) this.highestCalicoEggRatingToday, nameof (highestCalicoEggRatingToday)).AddField((INetSerializable) this.calicoStatueEffects, nameof (calicoStatueEffects));
    this.newLostAndFoundItems.Interpolated(false, false);
    this.junimoKartStatus.sortMode = PlayerStatusList.SortMode.NumberSortDescending;
    this.festivalScoreStatus.sortMode = PlayerStatusList.SortMode.NumberSortDescending;
    this.endOfNightStatus.displayMode = PlayerStatusList.DisplayMode.Icons;
    this.endOfNightStatus.AddSpriteDefinition("sleep", "LooseSprites\\PlayerStatusList", 0, 0, 16 /*0x10*/, 16 /*0x10*/);
    this.endOfNightStatus.AddSpriteDefinition("level", "LooseSprites\\PlayerStatusList", 16 /*0x10*/, 0, 16 /*0x10*/, 16 /*0x10*/);
    this.endOfNightStatus.AddSpriteDefinition("shipment", "LooseSprites\\PlayerStatusList", 32 /*0x20*/, 0, 16 /*0x10*/, 16 /*0x10*/);
    this.endOfNightStatus.AddSpriteDefinition("ready", "LooseSprites\\PlayerStatusList", 48 /*0x30*/, 0, 16 /*0x10*/, 16 /*0x10*/);
    this.endOfNightStatus.iconAnimationFrames = 4;
    this.festivalPropRemovalEvent.onEvent += (AbstractNetEvent1<Rectangle>.Event) (rect =>
    {
      if (Game1.CurrentEvent == null)
        return;
      Game1.CurrentEvent.removeFestivalProps(rect);
    });
    this.toggleSkullShrineOvernight.fieldChangeEvent += (FieldChange<NetBool, bool>) ((field, oldVal, newVal) =>
    {
      if (!newVal && !Game1.player.team.skullShrineActivated.Value || !(Game1.currentLocation.NameOrUniqueName == "SkullCave"))
        return;
      Game1.currentLocation.MakeMapModifications(true);
    });
    this.requestSpouseSleepEvent.onEvent += new AbstractNetEvent1<long>.Event(this.OnRequestSpouseSleepEvent);
    this.requestNPCGoHome.onEvent += new AbstractNetEvent1<string>.Event(this.OnRequestNPCGoHome);
    this.requestPetWarpHomeEvent.onEvent += new AbstractNetEvent1<long>.Event(this.OnRequestPetWarpHomeEvent);
    this.requestMovieEndEvent.onEvent += new AbstractNetEvent1<long>.Event(this.OnRequestMovieEndEvent);
    this.endMovieEvent.onEvent += new AbstractNetEvent1<long>.Event(this.OnEndMovieEvent);
    this.buildingConstructedEvent.AddReaderHandler(new Action<BinaryReader>(this.OnBuildingConstructedEvent));
    this.buildingMovedEvent.AddReaderHandler(new Action<BinaryReader>(this.OnBuildingMovedEvent));
    this.buildingDemolishedEvent.AddReaderHandler(new Action<BinaryReader>(this.OnBuildingDemolishedEvent));
    this.ringPhoneEvent.onEvent += new AbstractNetEvent1<string>.Event(this.OnRingPhoneEvent);
    this.requestNutDrop.onEvent += new AbstractNetEvent1<NutDropRequest>.Event(this.OnRequestNutDrop);
    this.requestSetSimpleFlag.onEvent += new AbstractNetEvent1<SetSimpleFlagRequest>.Event(this.OnRequestPlayerAction);
    this.requestSetMail.onEvent += new AbstractNetEvent1<SetMailRequest>.Event(this.OnRequestPlayerAction);
    this.requestAddCharacterEvent.onEvent += new AbstractNetEvent1<string>.Event(this.OnRequestAddCharacterEvent);
    this.addCharacterEvent.onEvent += new AbstractNetEvent1<string>.Event(this.OnAddCharacterEvent);
    this.requestLeoMove.onEvent += new NetEvent0.Event(this.OnRequestLeoMoveEvent);
    this.requestHorseWarpEvent.onEvent += new AbstractNetEvent1<long>.Event(this.OnRequestHorseWarp);
    this.calicoEggSkullCavernRating.fieldChangeEvent += new FieldChange<NetInt, int>(this.OnCalicoEggRatingChanged);
    this.calicoStatueEffects.OnValueAdded += (NetDictionary<int, int, NetInt, SerializableDictionary<int, int>, NetIntDictionary<int, NetInt>>.ContentsChangeEvent) ((key, _) => this.OnCalicoStatueEffectAdded(key));
    this.calicoStatueEffects.OnValueTargetUpdated += (NetDictionary<int, int, NetInt, SerializableDictionary<int, int>, NetIntDictionary<int, NetInt>>.ContentsUpdateEvent) ((key, oldValue, newValue) => this.OnCalicoStatueEffectAdded(key));
    this.kickOutOfMinesEvent.onEvent += new AbstractNetEvent1<int>.Event(this.OnKickOutOfMinesEvent);
    this.addQiGemsToTeam.onEvent += new AbstractNetEvent1<int>.Event(this._AddQiGemsToTeam);
    this.constructedBuildings.OnValueAdded += (NetHashSet<string>.ContentsChangeEvent) (buildingType =>
    {
      if (!Game1.hasStartedDay)
        return;
      Game1.player.NotifyQuests((Func<Quest, bool>) (quest => quest.OnBuildingExists(buildingType)));
    });
  }

  public void AddCalicoStatueEffect(int effectId)
  {
    if (this.calicoStatueEffects.TryAdd(effectId, 1))
      return;
    ++this.calicoStatueEffects[effectId];
  }

  private void OnCalicoStatueEffectAdded(int key)
  {
    switch (key)
    {
      case 10:
        if (Game1.player.currentLocation is MineShaft && Game1.mine.getMineArea() == 121)
        {
          DesertFestival.addCalicoStatueSpeedBuff();
          break;
        }
        break;
      case 11:
        Game1.player.health = Game1.player.maxHealth;
        Game1.player.stamina = (float) Game1.player.maxStamina.Value;
        break;
      case 12:
        if (!Game1.player.addItemToInventoryBool(ItemRegistry.Create("(O)CalicoEgg", 50)))
        {
          Game1.createItemDebris(ItemRegistry.Create("(O)CalicoEgg", 50), Game1.player.getStandingPosition(), 0, Game1.player.currentLocation);
          break;
        }
        break;
      case 15:
        if (!Game1.player.addItemToInventoryBool(ItemRegistry.Create("(O)CalicoEgg", 25)))
        {
          Game1.createItemDebris(ItemRegistry.Create("(O)CalicoEgg", 25), Game1.player.getStandingPosition(), 0, Game1.player.currentLocation);
          break;
        }
        break;
      case 16 /*0x10*/:
        if (!Game1.player.addItemToInventoryBool(ItemRegistry.Create("(O)CalicoEgg", 10)))
        {
          Game1.createItemDebris(ItemRegistry.Create("(O)CalicoEgg", 10), Game1.player.getStandingPosition(), 0, Game1.player.currentLocation);
          break;
        }
        break;
      case 17:
        if (!Game1.player.addItemToInventoryBool(ItemRegistry.Create("(O)CalicoEgg", 100)))
        {
          Game1.createItemDebris(ItemRegistry.Create("(O)CalicoEgg", 100), Game1.player.getStandingPosition(), 0, Game1.player.currentLocation);
          break;
        }
        break;
    }
    if (!(Game1.currentLocation is MineShaft) || Game1.mine.getMineArea() != 121)
      return;
    string s = Game1.content.LoadString("Strings\\1_6_Strings:DF_Mine_CalicoStatue_Description_" + key.ToString());
    Point point = Game1.mine.calicoStatueSpot.Value;
    foreach (Vector2 adjacentTileLocation in Utility.getAdjacentTileLocations(Vector2.Zero))
      Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite((string) null, Rectangle.Empty, new Vector2((float) (point.X * 64 /*0x40*/ + 32 /*0x20*/) - (float) SpriteText.getWidthOfString(s) / 2f, (float) ((point.Y - 3) * 64 /*0x40*/)) + adjacentTileLocation * 4f, false, 0.0f, Color.Black)
      {
        text = s,
        extraInfoForEndBehavior = -777,
        layerDepth = 0.99f,
        motion = new Vector2(0.0f, -1f),
        yStopCoordinate = (point.Y - 4) * 64 /*0x40*/,
        animationLength = 1,
        delayBeforeAnimationStart = 500,
        totalNumberOfLoops = 10,
        interval = 300f,
        drawAboveAlwaysFront = true
      });
    Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite((string) null, Rectangle.Empty, new Vector2((float) (point.X * 64 /*0x40*/ + 32 /*0x20*/) - (float) SpriteText.getWidthOfString(s) / 2f, (float) ((point.Y - 3) * 64 /*0x40*/)), false, 0.0f, Color.White)
    {
      text = s,
      extraInfoForEndBehavior = -777,
      layerDepth = 1f,
      motion = new Vector2(0.0f, -1f),
      yStopCoordinate = (point.Y - 4) * 64 /*0x40*/,
      animationLength = 1,
      delayBeforeAnimationStart = 500,
      totalNumberOfLoops = 10,
      interval = 300f,
      drawAboveAlwaysFront = true
    });
  }

  private void OnCalicoEggRatingChanged(NetInt field, int oldValue, int newValue)
  {
    if (newValue > oldValue && Game1.currentLocation is MineShaft)
    {
      if (Game1.mine != null)
        Game1.mine.calicoEggIconTimerShake = 1500f;
      DelayedAction.playSoundAfterDelay("yoba", 800);
    }
    if (!Game1.IsMasterGame || !Game1.hasStartedDay || newValue <= Game1.player.team.highestCalicoEggRatingToday.Value)
      return;
    Game1.player.team.highestCalicoEggRatingToday.Value = newValue;
  }

  protected virtual void _AddQiGemsToTeam(int amount) => Game1.player.QiGems += amount;

  /// <summary>Kick the player out of a mine area.</summary>
  /// <param name="mineshaftType">The type of mine from which to kick players, or <see cref="F:StardewValley.Locations.MineShaft.bottomOfMineLevel" /> for any area in the regular mines.</param>
  public virtual void OnKickOutOfMinesEvent(int mineshaftType)
  {
    if ((!(Game1.currentLocation is MineShaft currentLocation) ? 0 : (mineshaftType == 120 ? (currentLocation.mineLevel <= mineshaftType ? 1 : 0) : (currentLocation.getMineArea() == mineshaftType ? 1 : 0))) == 0)
      return;
    if (mineshaftType != 121)
    {
      if (mineshaftType == 77377)
      {
        Game1.player.completelyStopAnimatingOrDoingAction();
        Game1.warpFarmer(Game1.getLocationRequest("Mine"), 67, 10, 2);
      }
      else
      {
        Game1.player.completelyStopAnimatingOrDoingAction();
        Game1.warpFarmer(Game1.getLocationRequest("Mine"), 18, 4, 2);
      }
    }
    else
    {
      Game1.player.completelyStopAnimatingOrDoingAction();
      Game1.warpFarmer(Game1.getLocationRequest("SkullCave"), 3, 4, 2);
    }
  }

  public virtual void OnRequestHorseWarp(long uid)
  {
    if (!Game1.IsMasterGame)
      return;
    Farmer farmer = Game1.GetPlayer(uid);
    if (farmer == null)
      return;
    Horse horse = (Horse) null;
    Utility.ForEachBuilding<Stable>((Func<Stable, bool>) (stable =>
    {
      Horse stableHorse = stable.getStableHorse();
      if (stableHorse == null || stableHorse.getOwner() != farmer)
        return true;
      horse = stableHorse;
      return false;
    }));
    if (horse == null || Utility.GetHorseWarpRestrictionsForFarmer(farmer) != Utility.HorseWarpRestrictions.None)
      return;
    horse.mutex.RequestLock((Action) (() =>
    {
      horse.mutex.ReleaseLock();
      GameLocation currentLocation1 = horse.currentLocation;
      Vector2 tile1 = horse.Tile;
      for (int index = 0; index < 8; ++index)
        Game1.multiplayer.broadcastSprites(currentLocation1, new TemporaryAnimatedSprite(10, new Vector2(tile1.X + Utility.RandomFloat(-1f, 1f), tile1.Y + Utility.RandomFloat(-1f, 0.0f)) * 64f, Color.White, animationInterval: 50f)
        {
          layerDepth = 1f,
          motion = new Vector2(Utility.RandomFloat(-0.5f, 0.5f), Utility.RandomFloat(-0.5f, 0.5f))
        });
      currentLocation1.playSound("wand", new Vector2?(horse.Tile));
      GameLocation currentLocation2 = farmer.currentLocation;
      Vector2 tile2 = farmer.Tile;
      currentLocation2.playSound("wand", new Vector2?(tile2));
      for (int index = 0; index < 8; ++index)
        Game1.multiplayer.broadcastSprites(currentLocation2, new TemporaryAnimatedSprite(10, new Vector2(tile2.X + Utility.RandomFloat(-1f, 1f), tile2.Y + Utility.RandomFloat(-1f, 0.0f)) * 64f, Color.White, animationInterval: 50f)
        {
          layerDepth = 1f,
          motion = new Vector2(Utility.RandomFloat(-0.5f, 0.5f), Utility.RandomFloat(-0.5f, 0.5f))
        });
      Game1.warpCharacter((NPC) horse, farmer.currentLocation, tile2);
      int num = 0;
      for (int x = (int) tile2.X + 3; x >= (int) tile2.X - 3; --x)
      {
        Game1.multiplayer.broadcastSprites(currentLocation2, new TemporaryAnimatedSprite(6, new Vector2((float) x, tile2.Y) * 64f, Color.White, animationInterval: 50f)
        {
          layerDepth = 1f,
          delayBeforeAnimationStart = num * 25,
          motion = new Vector2(-0.25f, 0.0f)
        });
        ++num;
      }
    }));
  }

  public virtual void OnRequestLeoMoveEvent()
  {
    if (!Game1.IsMasterGame)
      return;
    Game1.player.team.requestAddCharacterEvent.Fire("Leo");
    NPC characterFromName = Game1.getCharacterFromName("Leo");
    if (characterFromName == null)
      return;
    characterFromName.reloadDefaultLocation();
    characterFromName.faceDirection(2);
    characterFromName.InvalidateMasterSchedule();
    characterFromName.ClearSchedule();
    characterFromName.controller = (PathFindController) null;
    characterFromName.temporaryController = (PathFindController) null;
    Game1.warpCharacter(characterFromName, Game1.RequireLocation("Mountain"), new Vector2(16f, 8f));
    characterFromName.Halt();
    characterFromName.ignoreScheduleToday = false;
  }

  public virtual void MarkCollectedNut(string key) => this.collectedNutTracker.Add(key);

  public int GetIndividualMoney(Farmer who) => this.GetMoney(who).Value;

  public void AddIndividualMoney(Farmer who, int value) => this.GetMoney(who).Value += value;

  public void SetIndividualMoney(Farmer who, int value) => this.GetMoney(who).Value = value;

  public NetIntDelta GetMoney(Farmer who)
  {
    if (!this.useSeparateWallets.Value)
      return this.money;
    NetIntDelta money;
    if (!this.individualMoney.TryGetValue(who.UniqueMultiplayerID, out money))
    {
      NetLongDictionary<NetIntDelta, NetRef<NetIntDelta>> individualMoney = this.individualMoney;
      long uniqueMultiplayerId = who.UniqueMultiplayerID;
      NetIntDelta netIntDelta = new NetIntDelta(500);
      netIntDelta.Minimum = new int?(0);
      money = netIntDelta;
      individualMoney[uniqueMultiplayerId] = netIntDelta;
    }
    return money;
  }

  public bool SpecialOrderActive(string special_order_key)
  {
    foreach (SpecialOrder specialOrder in this.specialOrders)
    {
      if (specialOrder.questKey.Value == special_order_key && specialOrder.questState.Value == SpecialOrderStatus.InProgress)
        return true;
    }
    return false;
  }

  public bool SpecialOrderRuleActive(string special_rule, SpecialOrder order_to_ignore = null)
  {
    foreach (SpecialOrder specialOrder in this.specialOrders)
    {
      if (specialOrder != order_to_ignore && specialOrder.questState.Value == SpecialOrderStatus.InProgress && specialOrder.specialRule.Value != null)
      {
        foreach (string str in specialOrder.specialRule.Value.Split(','))
        {
          if (str.Trim() == special_rule)
            return true;
        }
      }
    }
    return false;
  }

  /// <summary>Add a special order to the player team if it's not already active, or log a warning if it doesn't exist.</summary>
  /// <param name="id">The special order ID in <c>Data/SpecialOrders</c>.</param>
  /// <param name="generationSeed">The seed to use for randomizing the special order, or <c>null</c> for a random seed.</param>
  /// <param name="forceRepeatable">Whether to consider the special order repeatable regardless of <see cref="F:StardewValley.GameData.SpecialOrders.SpecialOrderData.Repeatable" />.</param>
  public void AddSpecialOrder(string id, int? generationSeed = null, bool forceRepeatable = false)
  {
    if (this.specialOrders.Any<SpecialOrder>((Func<SpecialOrder, bool>) (p => p.questKey.Value == id)))
      return;
    SpecialOrder specialOrder = SpecialOrder.GetSpecialOrder(id, generationSeed);
    if (specialOrder == null)
    {
      Game1.log.Warn($"Can't add special order with ID '{id}' because no such ID was found.");
    }
    else
    {
      if (this.completedSpecialOrders.Contains(specialOrder.questKey.Value) && !forceRepeatable)
      {
        SpecialOrderData data = specialOrder.GetData();
        if ((data != null ? (!data.Repeatable ? 1 : 0) : 1) != 0)
          return;
      }
      this.specialOrders.Add(specialOrder);
    }
  }

  public SpecialOrder GetAvailableSpecialOrder(int index = 0, string type = "")
  {
    foreach (SpecialOrder availableSpecialOrder in this.availableSpecialOrders)
    {
      if (availableSpecialOrder.orderType.Value == type)
      {
        if (index <= 0)
          return availableSpecialOrder;
        --index;
      }
    }
    return (SpecialOrder) null;
  }

  public void CheckReturnedDonations()
  {
    this.returnedDonationsMutex.RequestLock((Action) (() =>
    {
      this.returnedDonations.RemoveWhere((Func<Item, bool>) (item => item == null));
      Dictionary<ISalable, ItemStockInformation> itemPriceAndStock = new Dictionary<ISalable, ItemStockInformation>();
      foreach (Item returnedDonation in this.returnedDonations)
        itemPriceAndStock[(ISalable) returnedDonation] = new ItemStockInformation(0, 1, stockMode: LimitedStockMode.None);
      Game1.activeClickableMenu = (IClickableMenu) new ShopMenu("ReturnedDonations", itemPriceAndStock, on_purchase: new ShopMenu.OnPurchaseDelegate(this.OnDonatedItemWithdrawn), on_sell: new Func<ISalable, bool>(this.OnReturnedDonationDeposited))
      {
        source = (object) this,
        behaviorBeforeCleanup = (Action<IClickableMenu>) (menu => this.returnedDonationsMutex.ReleaseLock())
      };
    }));
  }

  /// <summary>Handle an item being taken from the lost and found box.</summary>
  /// <inheritdoc cref="T:StardewValley.Menus.ShopMenu.OnPurchaseDelegate" />
  public bool OnDonatedItemWithdrawn(
    ISalable salable,
    Farmer who,
    int countTaken,
    ItemStockInformation stock)
  {
    if (salable is Item obj && stock.Stock < 1)
      this.returnedDonations.Remove(obj);
    return false;
  }

  public bool OnReturnedDonationDeposited(ISalable deposited_salable) => false;

  public void OnRequestMovieEndEvent(long uid)
  {
    if (!Game1.IsMasterGame)
      return;
    Game1.RequireLocation<MovieTheater>("MovieTheater").RequestEndMovie(uid);
  }

  public void OnRequestPetWarpHomeEvent(long uid)
  {
    if (!Game1.IsMasterGame)
      return;
    Farmer who = Game1.GetPlayer(uid) ?? Game1.MasterPlayer;
    Pet characterFromName = Game1.getCharacterFromName<Pet>(who.getPetName(), false);
    if (characterFromName?.currentLocation is FarmHouse || characterFromName == null)
      return;
    characterFromName.warpToFarmHouse(who);
  }

  public void OnRequestNPCGoHome(string npc_name)
  {
    if (!Game1.IsMasterGame)
      return;
    NPC characterFromName = Game1.getCharacterFromName(npc_name);
    if (!string.IsNullOrEmpty(characterFromName.defaultMap.Value))
      return;
    characterFromName.doingEndOfRouteAnimation.Value = false;
    characterFromName.nextEndOfRouteMessage = (string) null;
    characterFromName.endOfRouteMessage.Value = (string) null;
    characterFromName.controller = (PathFindController) null;
    characterFromName.temporaryController = (PathFindController) null;
    characterFromName.Halt();
    Game1.warpCharacter(characterFromName, characterFromName.defaultMap.Value, characterFromName.DefaultPosition / 64f);
    characterFromName.ignoreScheduleToday = true;
  }

  public void OnRequestSpouseSleepEvent(long uid)
  {
    if (!Game1.IsMasterGame)
      return;
    Farmer player = Game1.GetPlayer(uid);
    if (player == null)
      return;
    NPC characterFromName = Game1.getCharacterFromName(player.spouse);
    if (characterFromName == null || characterFromName.isSleeping.Value)
      return;
    FarmHouse homeOfFarmer = Utility.getHomeOfFarmer(player);
    Game1.warpCharacter(characterFromName, (GameLocation) homeOfFarmer, new Vector2((float) homeOfFarmer.getSpouseBedSpot(player.spouse).X, (float) homeOfFarmer.getSpouseBedSpot(player.spouse).Y));
    characterFromName.NetFields.CancelInterpolation();
    characterFromName.Halt();
    characterFromName.faceDirection(0);
    characterFromName.controller = (PathFindController) null;
    characterFromName.temporaryController = (PathFindController) null;
    characterFromName.ignoreScheduleToday = true;
    if (homeOfFarmer.GetSpouseBed() == null)
      return;
    FarmHouse.spouseSleepEndFunction((Character) characterFromName, (GameLocation) homeOfFarmer);
  }

  public virtual void OnRequestAddCharacterEvent(string character_name)
  {
    if (!Game1.IsMasterGame || !Game1.AddCharacterIfNecessary(character_name))
      return;
    this.addCharacterEvent.Fire(character_name);
  }

  public virtual void OnAddCharacterEvent(string character_name)
  {
    if (Game1.IsMasterGame)
      return;
    Game1.AddCharacterIfNecessary(character_name, true);
  }

  /// <summary>Request a nut drop that only happens a set number of times.</summary>
  /// <param name="key">The key for the limited pool of nut drops.</param>
  /// <param name="location">The location where the nut will be dropped.</param>
  /// <param name="x">The x component of the coordinate where we will drop the nut in <paramref name="location" />.</param>
  /// <param name="y">The y component of the coordinate where we will drop the nut in <paramref name="location" />.</param>
  /// <param name="limit">The max amount of nuts that should be dropped from the pool specified by <paramref name="key" />.</param>
  /// <param name="rewardAmount">The amount of nuts that should be dropped. Defaults to 1.</param>
  public void RequestLimitedNutDrops(
    string key,
    GameLocation location,
    int x,
    int y,
    int limit,
    int rewardAmount = 1)
  {
    int num;
    if (this.limitedNutDrops.TryGetValue(key, out num) && num >= limit)
      return;
    this.requestNutDrop.Fire(new NutDropRequest(key, location?.NameOrUniqueName, new Point(x, y), limit, rewardAmount));
  }

  public int GetDroppedLimitedNutCount(string key)
  {
    int num;
    return !this.limitedNutDrops.TryGetValue(key, out num) ? 0 : num;
  }

  protected void OnRequestNutDrop(NutDropRequest request)
  {
    if (!Game1.IsMasterGame)
      return;
    int droppedLimitedNutCount = this.GetDroppedLimitedNutCount(request.Key);
    if (droppedLimitedNutCount >= request.Limit)
      return;
    int rewardAmount = request.RewardAmount;
    int num = Math.Min(request.Limit - droppedLimitedNutCount, rewardAmount);
    this.limitedNutDrops[request.Key] = droppedLimitedNutCount + num;
    GameLocation location = (GameLocation) null;
    if (request.LocationName != "null")
      location = Game1.getLocationFromName(request.LocationName);
    if (location != null)
    {
      for (int index = 0; index < num; ++index)
        Game1.createItemDebris(ItemRegistry.Create("(O)73"), new Vector2((float) request.Tile.X, (float) request.Tile.Y), -1, location);
    }
    else
    {
      Game1.netWorldState.Value.GoldenWalnutsFound += num;
      Game1.netWorldState.Value.GoldenWalnuts += num;
    }
  }

  /// <summary>Sends a request to set or unset a simple flag for a group of players.</summary>
  /// <param name="flag">The flag type to set for the players.</param>
  /// <param name="target">The players for which to perform the action.</param>
  /// <param name="flagId">The flag ID to update.</param>
  /// <param name="flagState">The flag state to set.</param>
  /// <param name="onlyPlayerId">The specific player ID to apply this event to, or <c>null</c> to apply it to all players matching <paramref name="target" />.</param>
  public void RequestSetSimpleFlag(
    SimpleFlagType flag,
    PlayerActionTarget target,
    string flagId,
    bool flagState,
    long? onlyPlayerId = null)
  {
    this.RequestPlayerAction<SetSimpleFlagRequest>(new SetSimpleFlagRequest(flag, target, flagId, flagState, onlyPlayerId), this.requestSetSimpleFlag);
  }

  /// <summary>Sends a request to add mail for a group of players.</summary>
  /// <param name="playerTarget">The players to update.</param>
  /// <param name="mailId">The mail to add.</param>
  /// <param name="mailType">When the mail should be received.</param>
  /// <param name="add">Whether to add the mail; else it'll be removed.</param>
  /// <param name="onlyPlayerId">The specific player ID to apply this event to, or <c>null</c> to apply it to all players matching <paramref name="playerTarget" />.</param>
  public void RequestSetMail(
    PlayerActionTarget playerTarget,
    string mailId,
    MailType mailType,
    bool add,
    long? onlyPlayerId = null)
  {
    this.RequestPlayerAction<SetMailRequest>(new SetMailRequest(playerTarget, mailId, mailType, add, onlyPlayerId), this.requestSetMail);
  }

  public void OnRingPhoneEvent(string callId) => Phone.Ring(callId);

  public void OnEndMovieEvent(long uid)
  {
    if (Game1.player.UniqueMultiplayerID != uid)
      return;
    Game1.player.lastSeenMovieWeek.Set(Game1.Date.TotalWeeks);
    if (Game1.CurrentEvent == null)
      return;
    Game1.CurrentEvent.onEventFinished += (Action) (() =>
    {
      Game1.warpFarmer(Game1.getLocationRequest("MovieTheater"), 13, 4, 2);
      Game1.fadeToBlackAlpha = 1f;
    });
    Game1.CurrentEvent.endBehaviors();
  }

  /// <summary>Notify all players that a building has been constructed.</summary>
  /// <param name="location">The location containing the building.</param>
  /// <param name="building">The building that was constructed.</param>
  /// <param name="who">The player that constructed the building.</param>
  /// <remarks>This is received via <see cref="M:StardewValley.FarmerTeam.OnBuildingConstructedEvent(System.IO.BinaryReader)" /> on all players, including the one who sent it.</remarks>
  public void SendBuildingConstructedEvent(GameLocation location, Building building, Farmer who)
  {
    this.buildingConstructedEvent.Fire((NetEventBinary.ArgWriter) (writer =>
    {
      writer.Write(location.NameOrUniqueName);
      writer.WriteGuid(building.id.Value);
      writer.Write(who.UniqueMultiplayerID);
    }));
  }

  /// <summary>Receive an event indicating that a building has been constructed.</summary>
  /// <param name="reader">The event argument reader.</param>
  /// <remarks>This receives an event sent via <see cref="M:StardewValley.FarmerTeam.SendBuildingConstructedEvent(StardewValley.GameLocation,StardewValley.Buildings.Building,StardewValley.Farmer)" />.</remarks>
  public void OnBuildingConstructedEvent(BinaryReader reader)
  {
    string name = reader.ReadString();
    Guid id1 = reader.ReadGuid();
    long id2 = reader.ReadInt64();
    GameLocation locationFromName = Game1.getLocationFromName(name);
    Building buildingById = locationFromName?.getBuildingById(id1);
    Farmer player = Game1.GetPlayer(id2);
    if (buildingById == null)
      return;
    locationFromName.OnBuildingConstructed(buildingById, player);
  }

  /// <summary>Notify all players that a building has been moved.</summary>
  /// <param name="location">The location containing the building.</param>
  /// <param name="building">The building that was moved.</param>
  /// <remarks>This is received via <see cref="M:StardewValley.FarmerTeam.OnBuildingMovedEvent(System.IO.BinaryReader)" /> on all players, including the one who sent it.</remarks>
  public void SendBuildingMovedEvent(GameLocation location, Building building)
  {
    this.buildingMovedEvent.Fire((NetEventBinary.ArgWriter) (writer =>
    {
      writer.Write(location.NameOrUniqueName);
      writer.WriteGuid(building.id.Value);
    }));
  }

  /// <summary>Receive an event indicating that a building has been moved.</summary>
  /// <param name="reader">The event argument reader.</param>
  /// <remarks>This receives an event sent via <see cref="M:StardewValley.FarmerTeam.SendBuildingMovedEvent(StardewValley.GameLocation,StardewValley.Buildings.Building)" />.</remarks>
  public void OnBuildingMovedEvent(BinaryReader reader)
  {
    string name = reader.ReadString();
    Guid id = reader.ReadGuid();
    GameLocation locationFromName = Game1.getLocationFromName(name);
    Building buildingById = locationFromName?.getBuildingById(id);
    if (buildingById == null)
      return;
    locationFromName.OnBuildingMoved(buildingById);
  }

  /// <summary>Notify all players that a building has been demolished.</summary>
  /// <param name="location">The location containing the building.</param>
  /// <param name="building">The building that was demolished.</param>
  /// <remarks>This is received via <see cref="M:StardewValley.FarmerTeam.OnBuildingDemolishedEvent(System.IO.BinaryReader)" /> on all players, including the one who sent it.</remarks>
  public void SendBuildingDemolishedEvent(GameLocation location, Building building)
  {
    this.buildingDemolishedEvent.Fire((NetEventBinary.ArgWriter) (writer =>
    {
      writer.Write(location.NameOrUniqueName);
      writer.Write(building.buildingType.Value);
      writer.WriteGuid(building.id.Value);
    }));
  }

  /// <summary>Receive an event indicating that a building has been demolished.</summary>
  /// <param name="reader">The event argument reader.</param>
  /// <remarks>This receives an event sent via <see cref="M:StardewValley.FarmerTeam.SendBuildingDemolishedEvent(StardewValley.GameLocation,StardewValley.Buildings.Building)" />.</remarks>
  public void OnBuildingDemolishedEvent(BinaryReader reader)
  {
    string name = reader.ReadString();
    string type = reader.ReadString();
    Guid id = reader.ReadGuid();
    Game1.getLocationFromName(name).OnBuildingDemolished(type, id);
  }

  /// <summary>Fully remove a farmhand player from the save. This will permanently remove their data if the game is saved.</summary>
  /// <param name="farmhand">The player to delete.</param>
  public void DeleteFarmhand(Farmer farmhand)
  {
    this.friendshipData.RemoveWhere((Func<KeyValuePair<FarmerPair, Friendship>, bool>) (pair => pair.Key.Contains(farmhand.UniqueMultiplayerID)));
    Game1.netWorldState.Value.farmhandData.Remove(farmhand.UniqueMultiplayerID);
  }

  public Friendship GetFriendship(long farmer1, long farmer2)
  {
    FarmerPair key = FarmerPair.MakePair(farmer1, farmer2);
    if (!this.friendshipData.ContainsKey(key))
      this.friendshipData.Add(key, new Friendship());
    return this.friendshipData[key];
  }

  public void AddAnyBroadcastedMail()
  {
    foreach (string str in (NetHashSet<string>) this.broadcastedMail)
    {
      Multiplayer.PartyWideMessageQueue wideMessageQueue = Multiplayer.PartyWideMessageQueue.SeenMail;
      string id = str;
      if (id.StartsWith("%&SM&%"))
      {
        id = id.Substring("%&SM&%".Length);
        wideMessageQueue = Multiplayer.PartyWideMessageQueue.SeenMail;
      }
      else if (id.StartsWith("%&MFT&%"))
      {
        id = id.Substring("%&MFT&%".Length);
        wideMessageQueue = Multiplayer.PartyWideMessageQueue.MailForTomorrow;
      }
      if (wideMessageQueue == Multiplayer.PartyWideMessageQueue.SeenMail)
      {
        if (id.Contains("%&NL&%") || id.StartsWith("NightMarketYear"))
          Game1.player.mailReceived.Add(id.Replace("%&NL&%", ""));
        else if (!Game1.player.hasOrWillReceiveMail(id))
          Game1.player.mailbox.Add(id);
      }
      else if (!Game1.MasterPlayer.mailForTomorrow.Contains(id))
      {
        if (!Game1.player.hasOrWillReceiveMail(id))
        {
          if (id.Contains("%&NL&%"))
            Game1.player.mailReceived.Add(id.Replace("%&NL&%", ""));
          else if (!Game1.player.mailbox.Contains(id))
            Game1.player.mailbox.Add(id);
        }
      }
      else if (!Game1.player.hasOrWillReceiveMail(id))
        Game1.player.mailForTomorrow.Add(id);
    }
  }

  public bool IsMarried(long farmer)
  {
    foreach (KeyValuePair<FarmerPair, Friendship> pair in this.friendshipData.Pairs)
    {
      if (pair.Key.Contains(farmer) && pair.Value.IsMarried())
        return true;
    }
    return false;
  }

  public bool IsEngaged(long farmer)
  {
    foreach (KeyValuePair<FarmerPair, Friendship> pair in this.friendshipData.Pairs)
    {
      if (pair.Key.Contains(farmer) && pair.Value.IsEngaged())
        return true;
    }
    return false;
  }

  public long? GetSpouse(long farmer)
  {
    foreach (KeyValuePair<FarmerPair, Friendship> pair in this.friendshipData.Pairs)
    {
      FarmerPair key = pair.Key;
      if (key.Contains(farmer) && (pair.Value.IsEngaged() || pair.Value.IsMarried()))
      {
        key = pair.Key;
        return new long?(key.GetOther(farmer));
      }
    }
    return new long?();
  }

  public void FestivalPropsRemoved(Rectangle rect) => this.festivalPropRemovalEvent.Fire(rect);

  public void SendProposal(Farmer receiver, ProposalType proposalType, Item gift = null)
  {
    Proposal proposal = new Proposal();
    proposal.sender.Value = Game1.player;
    proposal.receiver.Value = receiver;
    proposal.proposalType.Value = proposalType;
    proposal.gift.Value = gift;
    this.proposals[Game1.player.UniqueMultiplayerID] = proposal;
  }

  public Proposal GetOutgoingProposal()
  {
    Proposal proposal;
    return this.proposals.TryGetValue(Game1.player.UniqueMultiplayerID, out proposal) ? proposal : (Proposal) null;
  }

  public void RemoveOutgoingProposal() => this.proposals.Remove(Game1.player.UniqueMultiplayerID);

  public Proposal GetIncomingProposal()
  {
    foreach (Proposal incomingProposal in this.proposals.Values)
    {
      if (incomingProposal.receiver.Value == Game1.player && incomingProposal.response.Value == ProposalResponse.None)
        return incomingProposal;
    }
    return (Proposal) null;
  }

  private bool locationsMatch(GameLocation location1, GameLocation location2)
  {
    int level;
    return location1 != null && location2 != null && (location1.Name == location2.Name || (location1 is Mine || MineShaft.IsGeneratedLevel(location1, out level) && level < 121) && (location2 is Mine || MineShaft.IsGeneratedLevel(location2, out level) && level < 121) || (location1.Name.Equals("SkullCave") || MineShaft.IsGeneratedLevel(location1, out level) && level >= 121) && (location2.Name.Equals("SkullCave") || MineShaft.IsGeneratedLevel(location2, out level) && level >= 121));
  }

  public double AverageDailyLuck(GameLocation inThisLocation = null)
  {
    double num = 0.0;
    int val1 = 0;
    foreach (Farmer onlineFarmer in Game1.getOnlineFarmers())
    {
      if (inThisLocation == null || this.locationsMatch(inThisLocation, onlineFarmer.currentLocation))
      {
        num += onlineFarmer.DailyLuck;
        ++val1;
      }
    }
    return num / (double) Math.Max(val1, 1);
  }

  public double AverageLuckLevel(GameLocation inThisLocation = null)
  {
    double num = 0.0;
    int val1 = 0;
    foreach (Farmer onlineFarmer in Game1.getOnlineFarmers())
    {
      if (inThisLocation == null || this.locationsMatch(inThisLocation, onlineFarmer.currentLocation))
      {
        num += (double) onlineFarmer.LuckLevel;
        ++val1;
      }
    }
    return num / (double) Math.Max(val1, 1);
  }

  public double AverageSkillLevel(int skillIndex, GameLocation inThisLocation = null)
  {
    double num = 0.0;
    int val1 = 0;
    foreach (Farmer onlineFarmer in Game1.getOnlineFarmers())
    {
      if (inThisLocation == null || this.locationsMatch(inThisLocation, onlineFarmer.currentLocation))
      {
        num += (double) onlineFarmer.GetSkillLevel(skillIndex);
        ++val1;
      }
    }
    return num / (double) Math.Max(val1, 1);
  }

  public void Update()
  {
    this.requestLeoMove.Poll();
    this.requestMovieEndEvent.Poll();
    this.endMovieEvent.Poll();
    this.ringPhoneEvent.Poll();
    this.festivalPropRemovalEvent.Poll();
    this.buildingConstructedEvent.Poll();
    this.buildingMovedEvent.Poll();
    this.buildingDemolishedEvent.Poll();
    this.requestSpouseSleepEvent.Poll();
    this.requestNPCGoHome.Poll();
    this.requestHorseWarpEvent.Poll();
    this.kickOutOfMinesEvent.Poll();
    this.requestPetWarpHomeEvent.Poll();
    this.requestNutDrop.Poll();
    this.requestSetSimpleFlag.Poll();
    this.requestSetMail.Poll();
    this.requestAddCharacterEvent.Poll();
    this.addCharacterEvent.Poll();
    this.addQiGemsToTeam.Poll();
    this.grangeMutex.Update(Game1.getOnlineFarmers());
    this.returnedDonationsMutex.Update(Game1.getOnlineFarmers());
    this.ordersBoardMutex.Update(Game1.getOnlineFarmers());
    this.qiChallengeBoardMutex.Update(Game1.getOnlineFarmers());
    this.chestHit.Update();
    foreach (NetMutex netMutex in this.globalInventoryMutexes.Values)
      netMutex.Update(Game1.getOnlineFarmers());
    this.demolishLock.Update();
    this.buildLock.Update(Game1.getOnlineFarmers());
    this.movieMutex.Update(Game1.getOnlineFarmers());
    this.goldenCoconutMutex.Update(Game1.getOnlineFarmers());
    if (this.grangeMutex.IsLockHeld() && Game1.activeClickableMenu == null)
      this.grangeMutex.ReleaseLock();
    foreach (SpecialOrder specialOrder in this.specialOrders)
      specialOrder.Update();
    Game1.netReady.Update();
    if (Game1.IsMasterGame && this.proposals.Length > 0)
      this.proposals.RemoveWhere((Func<KeyValuePair<long, Proposal>, bool>) (pair => !this.playerIsOnline(pair.Key) || !this.playerIsOnline(pair.Value.receiver.UID)));
    Proposal incomingProposal = this.GetIncomingProposal();
    if (incomingProposal != null && incomingProposal.canceled.Value)
      incomingProposal.cancelConfirmed.Value = true;
    if (Game1.dialogueUp)
      return;
    if (incomingProposal != null)
    {
      if (this.handleIncomingProposal(incomingProposal))
        return;
      incomingProposal.responseMessageKey.Value = this.genderedKey("Strings\\UI:Proposal_PlayerBusy", Game1.player);
      incomingProposal.response.Value = ProposalResponse.Rejected;
    }
    else
    {
      if (Game1.activeClickableMenu != null || this.GetOutgoingProposal() == null)
        return;
      Game1.activeClickableMenu = (IClickableMenu) new PendingProposalDialog();
    }
  }

  private string genderedKey(string baseKey, Farmer farmer)
  {
    return baseKey + (farmer.IsMale ? "_Male" : "_Female");
  }

  private bool handleIncomingProposal(Proposal proposal)
  {
    if (Game1.gameMode != (byte) 3 || Game1.activeClickableMenu != null || Game1.currentMinigame != null)
      return proposal.proposalType.Value == ProposalType.Baby;
    if (Game1.currentLocation == null || proposal.proposalType.Value != ProposalType.Dance && Game1.CurrentEvent != null)
      return false;
    string sub2 = "";
    string responseYes = (string) null;
    string responseNo = (string) null;
    string baseKey;
    switch (proposal.proposalType.Value)
    {
      case ProposalType.Gift:
        if (proposal.gift.Value == null)
          return false;
        if (!Game1.player.couldInventoryAcceptThisItem(proposal.gift.Value))
        {
          proposal.response.Value = ProposalResponse.Rejected;
          proposal.responseMessageKey.Value = this.genderedKey("Strings\\UI:GiftPlayerItem_NoInventorySpace", Game1.player);
          return true;
        }
        baseKey = "Strings\\UI:GivenGift";
        sub2 = proposal.gift.Value.DisplayName;
        break;
      case ProposalType.Marriage:
        if (Game1.player.isMarriedOrRoommates() || Game1.player.isEngaged())
        {
          proposal.response.Value = ProposalResponse.Rejected;
          proposal.responseMessageKey.Value = this.genderedKey("Strings\\UI:AskedToMarry_NotSingle", Game1.player);
          return true;
        }
        baseKey = "Strings\\UI:AskedToMarry";
        responseYes = "Strings\\UI:AskedToMarry_Accepted";
        responseNo = "Strings\\UI:AskedToMarry_Rejected";
        break;
      case ProposalType.Dance:
        if (Game1.CurrentEvent == null || !Game1.CurrentEvent.isSpecificFestival("spring24"))
          return false;
        baseKey = "Strings\\UI:AskedToDance";
        responseYes = "Strings\\UI:AskedToDance_Accepted";
        responseNo = "Strings\\UI:AskedToDance_Rejected";
        if (Game1.player.dancePartner.Value != null)
          return false;
        break;
      case ProposalType.Baby:
        if (proposal.sender.Value.IsMale != Game1.player.IsMale)
        {
          baseKey = "Strings\\UI:AskedToHaveBaby";
          responseYes = "Strings\\UI:AskedToHaveBaby_Accepted";
          responseNo = "Strings\\UI:AskedToHaveBaby_Rejected";
          break;
        }
        baseKey = "Strings\\UI:AskedToAdoptBaby";
        responseYes = "Strings\\UI:AskedToAdoptBaby_Accepted";
        responseNo = "Strings\\UI:AskedToAdoptBaby_Rejected";
        break;
      default:
        return false;
    }
    string path = this.genderedKey(baseKey, proposal.sender.Value);
    if (responseYes != null)
      responseYes = this.genderedKey(responseYes, Game1.player);
    if (responseNo != null)
      responseNo = this.genderedKey(responseNo, Game1.player);
    Game1.currentLocation.createQuestionDialogue(Game1.content.LoadString(path, (object) proposal.sender.Value.Name, (object) sub2), Game1.currentLocation.createYesNoResponses(), (GameLocation.afterQuestionBehavior) ((_, answer) =>
    {
      if (proposal.canceled.Value)
      {
        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\UI:ProposalWithdrawn", (object) proposal.sender.Value.Name));
        proposal.response.Value = ProposalResponse.Rejected;
        proposal.responseMessageKey.Value = responseNo;
      }
      else if (answer == "Yes")
      {
        proposal.response.Value = ProposalResponse.Accepted;
        proposal.responseMessageKey.Value = responseYes;
        if (proposal.proposalType.Value == ProposalType.Gift || proposal.proposalType.Value == ProposalType.Marriage)
        {
          Item obj = proposal.gift.Value;
          proposal.gift.Value = (Item) null;
          Item inventory = Game1.player.addItemToInventory(obj);
          if (inventory != null)
            Game1.currentLocation.debris.Add(new Debris(inventory, Game1.player.Position));
        }
        switch (proposal.proposalType.Value)
        {
          case ProposalType.Marriage:
            Friendship friendship1 = this.GetFriendship(proposal.sender.Value.UniqueMultiplayerID, Game1.player.UniqueMultiplayerID);
            friendship1.Status = FriendshipStatus.Engaged;
            friendship1.Proposer = proposal.sender.Value.UniqueMultiplayerID;
            WorldDate worldDate1 = new WorldDate(Game1.Date);
            worldDate1.TotalDays += 3;
            while (!Game1.canHaveWeddingOnDay(worldDate1.DayOfMonth, worldDate1.Season))
              ++worldDate1.TotalDays;
            friendship1.WeddingDate = worldDate1;
            Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\UI:PlayerWeddingArranged"));
            Game1.multiplayer.globalChatInfoMessage("Engaged", Game1.player.Name, proposal.sender.Value.Name);
            break;
          case ProposalType.Dance:
            Game1.player.dancePartner.Value = (Character) proposal.sender.Value;
            break;
          case ProposalType.Baby:
            Friendship friendship2 = this.GetFriendship(proposal.sender.Value.UniqueMultiplayerID, Game1.player.UniqueMultiplayerID);
            WorldDate worldDate2 = new WorldDate(Game1.Date);
            worldDate2.TotalDays += 14;
            WorldDate worldDate3 = worldDate2;
            friendship2.NextBirthingDate = worldDate3;
            break;
        }
        Game1.player.doEmote(20);
      }
      else
      {
        proposal.response.Value = ProposalResponse.Rejected;
        proposal.responseMessageKey.Value = responseNo;
      }
    }));
    return true;
  }

  public bool playerIsOnline(long uid)
  {
    if (Game1.MasterPlayer.UniqueMultiplayerID == uid || (NetFieldBase<Farmer, NetRef<Farmer>>) Game1.serverHost != (NetRef<Farmer>) null && Game1.serverHost.Value.UniqueMultiplayerID == uid)
      return true;
    return Game1.otherFarmers.ContainsKey(uid) && !Game1.multiplayer.isDisconnecting(uid);
  }

  /// <summary>Get a global inventory from <see cref="F:StardewValley.FarmerTeam.globalInventories" />, creating it if needed.</summary>
  /// <param name="id">The inventory ID to get, usually matching a constant like <see cref="F:StardewValley.FarmerTeam.GlobalInventoryId_JunimoChest" />.</param>
  public Inventory GetOrCreateGlobalInventory(string id)
  {
    Inventory globalInventory;
    if (!this.globalInventories.TryGetValue(id, out globalInventory))
      this.globalInventories[id] = globalInventory = new Inventory();
    return globalInventory;
  }

  /// <summary>Get the mutex which restricts access to a global inventory, creating it if needed.</summary>
  /// <param name="id">The inventory ID to get, usually matching a constant like <see cref="F:StardewValley.FarmerTeam.GlobalInventoryId_JunimoChest" />.</param>
  public NetMutex GetOrCreateGlobalInventoryMutex(string id)
  {
    NetMutex globalInventoryMutex;
    if (!this.globalInventoryMutexes.TryGetValue(id, out globalInventoryMutex))
      this.globalInventoryMutexes[id] = globalInventoryMutex = new NetMutex();
    return globalInventoryMutex;
  }

  public void NewDay()
  {
    Game1.dedicatedServer.ResetForNewDay();
    Game1.netReady.Reset();
    this.chestHit.Reset();
    if (Game1.IsClient)
      return;
    this.luauIngredients.Clear();
    if (this.grangeDisplay.Count > 0)
    {
      for (int index = 0; index < this.grangeDisplay.Count; ++index)
      {
        Item obj = this.grangeDisplay[index];
        this.grangeDisplay[index] = (Item) null;
        if (obj != null)
        {
          this.returnedDonations.Add(obj);
          this.newLostAndFoundItems.Value = true;
        }
      }
    }
    this.grangeDisplay.Clear();
    this.movieInvitations.Clear();
    this.synchronizedShopStock.Clear();
  }

  /// <summary>Synchronizes a request to perform an action on a group of players.</summary>
  /// <param name="request">The data of the requested action to synchronize.</param>
  /// <param name="event">The net event used to send the synchronization data.</param>
  private void RequestPlayerAction<T>(T request, NetEvent1<T> @event) where T : BasePlayerActionRequest, new()
  {
    if (request.OnlyForLocalPlayer())
      request.PerformAction(Game1.player);
    else
      @event.Fire(request);
  }

  /// <summary>Handles a request to perform an action on a group of players.</summary>
  /// <param name="request">The arguments for the event.</param>
  private void OnRequestPlayerAction(BasePlayerActionRequest request)
  {
    if (request.MatchesPlayer(Game1.player))
      request.PerformAction(Game1.player);
    if (request.Target != PlayerActionTarget.All || !Game1.IsMasterGame)
      return;
    foreach (Farmer offlineFarmhand in Game1.getOfflineFarmhands())
    {
      if (request.MatchesPlayer(offlineFarmhand))
        request.PerformAction(offlineFarmhand);
    }
  }

  public enum RemoteBuildingPermissions
  {
    Off,
    OwnedBuildings,
    On,
  }

  public enum SleepAnnounceModes
  {
    All,
    First,
    Off,
  }
}
