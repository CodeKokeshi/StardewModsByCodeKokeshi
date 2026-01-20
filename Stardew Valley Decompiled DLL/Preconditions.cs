// Decompiled with JetBrains decompiler
// Type: StardewValley.Preconditions
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using StardewValley.Extensions;
using StardewValley.Internal;
using StardewValley.Locations;
using StardewValley.Network;
using StardewValley.Network.Dedicated;
using System;

#nullable disable
namespace StardewValley;

/// <summary>The low-level handlers for vanilla event preconditions. Most code should use <see cref="M:StardewValley.Event.CheckPrecondition(StardewValley.GameLocation,System.String,System.String)" /> instead.</summary>
public class Preconditions
{
  /// <summary>The current farmer has seen any of the given events.</summary>
  /// <inheritdoc cref="T:StardewValley.Delegates.EventPreconditionDelegate" />
  [OtherNames(new string[] {"e"})]
  public static bool SawEvent(GameLocation location, string eventId, string[] args)
  {
    for (int index = 1; index < args.Length; ++index)
    {
      string str;
      string error;
      if (!ArgUtility.TryGet(args, index, out str, out error, false, "string id"))
        return Event.LogPreconditionError(location, eventId, args, error);
      if (Game1.player.eventsSeen.Contains(str))
        return true;
    }
    return false;
  }

  /// <summary>The current farmer hasn't received a pet yet, and (if specified) has this pet preference.</summary>
  /// <inheritdoc cref="T:StardewValley.Delegates.EventPreconditionDelegate" />
  [OtherNames(new string[] {"h"})]
  public static bool MissingPet(GameLocation location, string eventId, string[] args)
  {
    string str;
    string error;
    if (!ArgUtility.TryGetOptional(args, 1, out str, out error, allowBlank: false, name: "string petType"))
      return Event.LogPreconditionError(location, eventId, args, error);
    if (Game1.player.hasPet())
      return false;
    return str == null || str.EqualsIgnoreCase(Game1.player.whichPetType);
  }

  /// <summary>The current farmer is the host.</summary>
  /// <inheritdoc cref="T:StardewValley.Delegates.EventPreconditionDelegate" />
  [OtherNames(new string[] {"H"})]
  public static bool IsHost(GameLocation location, string eventId, string[] args)
  {
    if (Game1.dedicatedServer != null)
      Game1.dedicatedServer.CheckedHostPrecondition = true;
    return Game1.IsMasterGame;
  }

  /// <summary>The host farmer has this mail.</summary>
  /// <inheritdoc cref="T:StardewValley.Delegates.EventPreconditionDelegate" />
  [OtherNames(new string[] {"Hn"})]
  public static bool HostMail(GameLocation location, string eventId, string[] args)
  {
    string str;
    string error;
    return !ArgUtility.TryGet(args, 1, out str, out error, false, "string mailId") ? Event.LogPreconditionError(location, eventId, args, error) : Game1.MasterPlayer.mailReceived.Contains(str);
  }

  /// <summary>The host farmer does NOT have this mail.</summary>
  /// <inheritdoc cref="T:StardewValley.Delegates.EventPreconditionDelegate" />
  [Obsolete("New events should use !HostMail instead.")]
  [OtherNames(new string[] {"Hl"})]
  public static bool NotHostMail(GameLocation location, string eventId, string[] args)
  {
    string str;
    string error;
    return !ArgUtility.TryGet(args, 1, out str, out error, false, "string mailId") ? Event.LogPreconditionError(location, eventId, args, error) : !Game1.MasterPlayer.mailReceived.Contains(str);
  }

  /// <summary>This world state ID is active anywhere.</summary>
  /// <inheritdoc cref="T:StardewValley.Delegates.EventPreconditionDelegate" />
  [OtherNames(new string[] {"*"})]
  public static bool WorldState(GameLocation location, string eventId, string[] args)
  {
    string id;
    string error;
    return !ArgUtility.TryGet(args, 1, out id, out error, false, "string worldStateId") ? Event.LogPreconditionError(location, eventId, args, error) : NetWorldState.checkAnywhereForWorldStateID(id);
  }

  /// <summary>Either the host or current farmer have this mail.</summary>
  /// <inheritdoc cref="T:StardewValley.Delegates.EventPreconditionDelegate" />
  [OtherNames(new string[] {"*n"})]
  public static bool HostOrLocalMail(GameLocation location, string eventId, string[] args)
  {
    string str;
    string error;
    if (!ArgUtility.TryGet(args, 1, out str, out error, false, "string mailId"))
      return Event.LogPreconditionError(location, eventId, args, error);
    return Game1.MasterPlayer.mailReceived.Contains(str) || Game1.player.mailReceived.Contains(str);
  }

  /// <summary>Neither the host nor current farmer have this mail.</summary>
  /// <inheritdoc cref="T:StardewValley.Delegates.EventPreconditionDelegate" />
  [Obsolete("New events should use !HostOrLocalMail instead.")]
  [OtherNames(new string[] {"*l"})]
  public static bool NotHostOrLocalMail(GameLocation location, string eventId, string[] args)
  {
    return !Preconditions.HostOrLocalMail(location, eventId, args);
  }

  /// <summary>The current farmer has earned at least this much money, including spent money.</summary>
  /// <inheritdoc cref="T:StardewValley.Delegates.EventPreconditionDelegate" />
  [OtherNames(new string[] {"m"})]
  public static bool EarnedMoney(GameLocation location, string eventId, string[] args)
  {
    int num;
    string error;
    return !ArgUtility.TryGetInt(args, 1, out num, out error, "int minMoney") ? Event.LogPreconditionError(location, eventId, args, error) : (long) Game1.player.totalMoneyEarned >= (long) num;
  }

  /// <summary>The current farmer has at least this much money, not including spent money.</summary>
  /// <inheritdoc cref="T:StardewValley.Delegates.EventPreconditionDelegate" />
  [OtherNames(new string[] {"M"})]
  public static bool HasMoney(GameLocation location, string eventId, string[] args)
  {
    int num;
    string error;
    return !ArgUtility.TryGetInt(args, 1, out num, out error, "int minMoney") ? Event.LogPreconditionError(location, eventId, args, error) : Game1.player.Money >= num;
  }

  /// <summary>The current farmer has at least this many free slots in their inventory.</summary>
  /// <inheritdoc cref="T:StardewValley.Delegates.EventPreconditionDelegate" />
  [OtherNames(new string[] {"c"})]
  public static bool FreeInventorySlots(GameLocation location, string eventId, string[] args)
  {
    int num;
    string error;
    return !ArgUtility.TryGetInt(args, 1, out num, out error, "int minFreeSpots") ? Event.LogPreconditionError(location, eventId, args, error) : Game1.player.freeSpotsInInventory() >= num;
  }

  /// <summary>The community center or Joja warehouse have been completed.</summary>
  /// <inheritdoc cref="T:StardewValley.Delegates.EventPreconditionDelegate" />
  [OtherNames(new string[] {"C"})]
  public static bool CommunityCenterOrWarehouseDone(
    GameLocation location,
    string eventId,
    string[] args)
  {
    return Game1.MasterPlayer.eventsSeen.Contains("191393") || Game1.MasterPlayer.eventsSeen.Contains("502261") || Game1.MasterPlayer.hasCompletedCommunityCenter();
  }

  /// <summary>The community center or Joja warehouse have NOT been completed.</summary>
  /// <inheritdoc cref="T:StardewValley.Delegates.EventPreconditionDelegate" />
  [Obsolete("New events should use !CommunityCenterOrWarehouseDone instead.")]
  [OtherNames(new string[] {"X"})]
  public static bool NotCommunityCenterOrWarehouseDone(
    GameLocation location,
    string eventId,
    string[] args)
  {
    return !Preconditions.CommunityCenterOrWarehouseDone(location, eventId, args);
  }

  /// <summary>The current farmer is dating the given NPC.</summary>
  /// <inheritdoc cref="T:StardewValley.Delegates.EventPreconditionDelegate" />
  [OtherNames(new string[] {"D"})]
  public static bool Dating(GameLocation location, string eventId, string[] args)
  {
    string key;
    string error;
    if (!ArgUtility.TryGet(args, 1, out key, out error, false, "string npcName"))
      return Event.LogPreconditionError(location, eventId, args, error);
    StardewValley.Friendship friendship;
    return Game1.player.friendshipData.TryGetValue(key, out friendship) && friendship.IsDating();
  }

  /// <summary>The main farmer has played at least this many days.</summary>
  /// <inheritdoc cref="T:StardewValley.Delegates.EventPreconditionDelegate" />
  [OtherNames(new string[] {"j"})]
  public static bool DaysPlayed(GameLocation location, string eventId, string[] args)
  {
    int num;
    string error;
    return !ArgUtility.TryGetInt(args, 1, out num, out error, "int minDaysPlayed") ? Event.LogPreconditionError(location, eventId, args, error) : (long) Game1.stats.DaysPlayed > (long) num;
  }

  /// <summary>All Joja bundles has been completed.</summary>
  /// <inheritdoc cref="T:StardewValley.Delegates.EventPreconditionDelegate" />
  [OtherNames(new string[] {"J"})]
  public static bool JojaBundlesDone(GameLocation location, string eventId, string[] args)
  {
    return Utility.hasFinishedJojaRoute();
  }

  /// <summary>The current farmer has at least this many friendship points with all of the given NPCs.</summary>
  /// <inheritdoc cref="T:StardewValley.Delegates.EventPreconditionDelegate" />
  [OtherNames(new string[] {"f"})]
  public static bool Friendship(GameLocation location, string eventId, string[] args)
  {
    for (int index = 1; index < args.Length; index += 2)
    {
      string key;
      string error;
      int num;
      if (!ArgUtility.TryGet(args, index, out key, out error, false, "string npcName") || !ArgUtility.TryGetInt(args, index + 1, out num, out error, "int minPoints"))
        return Event.LogPreconditionError(location, eventId, args, error);
      StardewValley.Friendship friendship;
      if (!Game1.player.friendshipData.TryGetValue(key, out friendship) || friendship.Points < num)
        return false;
    }
    return true;
  }

  /// <summary>Today is a festival day.</summary>
  /// <inheritdoc cref="T:StardewValley.Delegates.EventPreconditionDelegate" />
  public static bool FestivalDay(GameLocation location, string eventId, string[] args)
  {
    return Utility.isFestivalDay();
  }

  /// <summary>Today is NOT a festival day.</summary>
  /// <inheritdoc cref="T:StardewValley.Delegates.EventPreconditionDelegate" />
  [Obsolete("New events should use !FestivalDay instead.")]
  [OtherNames(new string[] {"F"})]
  public static bool NotFestivalDay(GameLocation location, string eventId, string[] args)
  {
    return !Preconditions.FestivalDay(location, eventId, args);
  }

  /// <summary>A random check with the given probability matches.</summary>
  /// <inheritdoc cref="T:StardewValley.Delegates.EventPreconditionDelegate" />
  [OtherNames(new string[] {"r"})]
  public static bool Random(GameLocation location, string eventId, string[] args)
  {
    float num;
    string error;
    return !ArgUtility.TryGetFloat(args, 1, out num, out error, "float probability") ? Event.LogPreconditionError(location, eventId, args, error) : Game1.random.NextDouble() <= (double) num;
  }

  /// <summary>The current farmer has shipped at least this many of each given item ID.</summary>
  /// <inheritdoc cref="T:StardewValley.Delegates.EventPreconditionDelegate" />
  [OtherNames(new string[] {"s"})]
  public static bool Shipped(GameLocation location, string eventId, string[] args)
  {
    for (int index = 1; index < args.Length; index += 2)
    {
      string key;
      string error;
      int num1;
      if (!ArgUtility.TryGet(args, index, out key, out error, false, "string itemId") || !ArgUtility.TryGetInt(args, index + 1, out num1, out error, "int minShipped"))
        return Event.LogPreconditionError(location, eventId, args, error);
      int num2;
      if (!Game1.player.basicShipped.TryGetValue(key, out num2) || num2 < num1)
        return false;
    }
    return true;
  }

  /// <summary>The current farmer has seen this secret note.</summary>
  /// <inheritdoc cref="T:StardewValley.Delegates.EventPreconditionDelegate" />
  [OtherNames(new string[] {"S"})]
  public static bool SawSecretNote(GameLocation location, string eventId, string[] args)
  {
    int num;
    string error;
    return !ArgUtility.TryGetInt(args, 1, out num, out error, "int secretNoteId") ? Event.LogPreconditionError(location, eventId, args, error) : Game1.player.secretNotesSeen.Contains(num);
  }

  /// <summary>The current farmer has selected all of the given dialogue answer IDs.</summary>
  /// <inheritdoc cref="T:StardewValley.Delegates.EventPreconditionDelegate" />
  [OtherNames(new string[] {"q"})]
  public static bool ChoseDialogueAnswers(GameLocation location, string eventId, string[] args)
  {
    for (int index = 1; index < args.Length; ++index)
    {
      string str;
      string error;
      if (!ArgUtility.TryGet(args, index, out str, out error, false, "string answerId"))
        return Event.LogPreconditionError(location, eventId, args, error);
      if (!Game1.player.DialogueQuestionsAnswered.Contains(str))
        return false;
    }
    return true;
  }

  /// <summary>The current farmer has received this mail.</summary>
  /// <inheritdoc cref="T:StardewValley.Delegates.EventPreconditionDelegate" />
  [OtherNames(new string[] {"n"})]
  public static bool LocalMail(GameLocation location, string eventId, string[] args)
  {
    string str;
    string error;
    return !ArgUtility.TryGet(args, 1, out str, out error, false, "string mailId") ? Event.LogPreconditionError(location, eventId, args, error) : Game1.player.mailReceived.Contains(str);
  }

  /// <summary>All players have found at least this many golden walnuts combined, including spent walnuts.</summary>
  /// <inheritdoc cref="T:StardewValley.Delegates.EventPreconditionDelegate" />
  [OtherNames(new string[] {"N"})]
  public static bool GoldenWalnuts(GameLocation location, string eventId, string[] args)
  {
    int num;
    string error;
    return !ArgUtility.TryGetInt(args, 1, out num, out error, "int minWalnuts") ? Event.LogPreconditionError(location, eventId, args, error) : Game1.netWorldState.Value.GoldenWalnutsFound >= num;
  }

  /// <summary>The current farmer has NOT received this mail.</summary>
  /// <inheritdoc cref="T:StardewValley.Delegates.EventPreconditionDelegate" />
  [Obsolete("New events should use !LocalMail instead.")]
  [OtherNames(new string[] {"l"})]
  public static bool NotLocalMail(GameLocation location, string eventId, string[] args)
  {
    return !Preconditions.LocalMail(location, eventId, args);
  }

  /// <summary>The current location is a farmhouse or cabin, and it has been upgraded to the max level (level 2 or greater).</summary>
  /// <inheritdoc cref="T:StardewValley.Delegates.EventPreconditionDelegate" />
  [OtherNames(new string[] {"L"})]
  public static bool InUpgradedHouse(GameLocation location, string eventId, string[] args)
  {
    int num;
    string error;
    if (!ArgUtility.TryGetOptionalInt(args, 1, out num, out error, 2, "int minUpgradeLevel"))
      return Event.LogPreconditionError(location, eventId, args, error);
    return location is FarmHouse farmHouse && farmHouse.upgradeLevel >= num;
  }

  /// <summary>The current time of day is between the given values inclusively.</summary>
  /// <inheritdoc cref="T:StardewValley.Delegates.EventPreconditionDelegate" />
  [OtherNames(new string[] {"t"})]
  public static bool Time(GameLocation location, string eventId, string[] args)
  {
    int num1;
    string error;
    int num2;
    if (!ArgUtility.TryGetInt(args, 1, out num1, out error, "int minTime") || !ArgUtility.TryGetInt(args, 2, out num2, out error, "int maxTime"))
      return Event.LogPreconditionError(location, eventId, args, error);
    return Game1.timeOfDay >= num1 && Game1.timeOfDay <= num2;
  }

  /// <summary>The weather in the current location's context is <c>rainy</c>, <c>sunny</c>, or the given weather ID.</summary>
  /// <inheritdoc cref="T:StardewValley.Delegates.EventPreconditionDelegate" />
  [OtherNames(new string[] {"w"})]
  public static bool Weather(GameLocation location, string eventId, string[] args)
  {
    string str;
    string error;
    if (!ArgUtility.TryGet(args, 1, out str, out error, false, "string weather"))
      return Event.LogPreconditionError(location, eventId, args, error);
    switch (str)
    {
      case "rainy":
        return location.IsRainingHere();
      case "sunny":
        return !location.IsRainingHere();
      default:
        return str == location.GetWeather().Weather;
    }
  }

  /// <summary>The current day of week is one of these values (in the form <c>Mon</c>, <c>Tue</c>, etc).</summary>
  /// <inheritdoc cref="T:StardewValley.Delegates.EventPreconditionDelegate" />
  public static bool DayOfWeek(GameLocation location, string eventId, string[] args)
  {
    System.DayOfWeek dayOfWeek1 = Game1.Date.DayOfWeek;
    for (int index = 1; index < args.Length; ++index)
    {
      string day;
      string error;
      if (!ArgUtility.TryGet(args, index, out day, out error, false, "string rawDayName"))
        return Event.LogPreconditionError(location, eventId, args, error);
      System.DayOfWeek dayOfWeek2;
      if (!WorldDate.TryGetDayOfWeekFor(day, out dayOfWeek2))
        return Event.LogPreconditionError(location, eventId, args, $"can't parse '{day}' as a day of week");
      if (dayOfWeek1 == dayOfWeek2)
        return true;
    }
    return false;
  }

  /// <summary>The current day of week is NOT one of these values (in the form <c>Mon</c>, <c>Tue</c>, etc).</summary>
  /// <inheritdoc cref="T:StardewValley.Delegates.EventPreconditionDelegate" />
  [Obsolete("New events should use !DayOfWeek instead.")]
  [OtherNames(new string[] {"d"})]
  public static bool NotDayOfWeek(GameLocation location, string eventId, string[] args)
  {
    return !Preconditions.DayOfWeek(location, eventId, args);
  }

  /// <summary>The current farmer is married to or engaged with this NPC.</summary>
  /// <inheritdoc cref="T:StardewValley.Delegates.EventPreconditionDelegate" />
  [OtherNames(new string[] {"O"})]
  public static bool Spouse(GameLocation location, string eventId, string[] args)
  {
    string str;
    string error;
    return !ArgUtility.TryGet(args, 1, out str, out error, false, "string npcName") ? Event.LogPreconditionError(location, eventId, args, error) : Game1.player.spouse == str;
  }

  /// <summary>The current farmer is NOT married to or engaged with this NPC.</summary>
  /// <inheritdoc cref="T:StardewValley.Delegates.EventPreconditionDelegate" />
  [Obsolete("New events should use !Spouse instead.")]
  [OtherNames(new string[] {"o"})]
  public static bool NotSpouse(GameLocation location, string eventId, string[] args)
  {
    return !Preconditions.Spouse(location, eventId, args);
  }

  /// <summary>The current farmer is roommates with any NPC.</summary>
  /// <inheritdoc cref="T:StardewValley.Delegates.EventPreconditionDelegate" />
  [OtherNames(new string[] {"R"})]
  public static bool Roommate(GameLocation location, string eventId, string[] args)
  {
    return Game1.player.hasCurrentOrPendingRoommate();
  }

  /// <summary>The current farmer is NOT roommates with any NPC.</summary>
  /// <inheritdoc cref="T:StardewValley.Delegates.EventPreconditionDelegate" />
  [Obsolete("New events should use !Roommate instead.")]
  [OtherNames(new string[] {"Rf"})]
  public static bool NotRoommate(GameLocation location, string eventId, string[] args)
  {
    return !Preconditions.Roommate(location, eventId, args);
  }

  /// <summary>The given NPC is present and visible in any location.</summary>
  /// <inheritdoc cref="T:StardewValley.Delegates.EventPreconditionDelegate" />
  [OtherNames(new string[] {"v"})]
  public static bool NpcVisible(GameLocation location, string eventId, string[] args)
  {
    string name;
    string error;
    if (!ArgUtility.TryGet(args, 1, out name, out error, false, "string npcName"))
      return Event.LogPreconditionError(location, eventId, args, error);
    NPC characterFromName = Game1.getCharacterFromName(name);
    return characterFromName != null && !characterFromName.IsInvisible;
  }

  /// <summary>The given NPC is present and visible in the current location.</summary>
  /// <inheritdoc cref="T:StardewValley.Delegates.EventPreconditionDelegate" />
  [OtherNames(new string[] {"p"})]
  public static bool NpcVisibleHere(GameLocation location, string eventId, string[] args)
  {
    string str;
    string error;
    if (!ArgUtility.TryGet(args, 1, out str, out error, false, "string npcName"))
      return Event.LogPreconditionError(location, eventId, args, error);
    foreach (NPC character in location.characters)
    {
      if (character.Name == str && !character.IsInvisible)
        return true;
    }
    return false;
  }

  /// <summary>The current calendar season is one of the given values.</summary>
  /// <inheritdoc cref="T:StardewValley.Delegates.EventPreconditionDelegate" />
  public static bool Season(GameLocation location, string eventId, string[] args)
  {
    for (int index = 1; index < args.Length; ++index)
    {
      StardewValley.Season season;
      string error;
      if (!ArgUtility.TryGetEnum<StardewValley.Season>(args, 1, out season, out error, "Season season"))
        return Event.LogPreconditionError(location, eventId, args, error);
      if (Game1.season == season)
        return true;
    }
    return false;
  }

  /// <summary>The current calendar season is NOT one of the given values.</summary>
  /// <inheritdoc cref="T:StardewValley.Delegates.EventPreconditionDelegate" />
  [Obsolete("New events should use !Season instead.")]
  [OtherNames(new string[] {"z"})]
  public static bool NotSeason(GameLocation location, string eventId, string[] args)
  {
    return !Preconditions.Season(location, eventId, args);
  }

  /// <summary>The current farmer has a spouse bed in their house.</summary>
  /// <inheritdoc cref="T:StardewValley.Delegates.EventPreconditionDelegate" />
  [OtherNames(new string[] {"B"})]
  public static bool SpouseBed(GameLocation location, string eventId, string[] args)
  {
    return Utility.getHomeOfFarmer(Game1.player).GetSpouseBed() != null;
  }

  /// <summary>The current farmer has reached the bottom of the mines (i.e. level 120) at least this many times.</summary>
  /// <inheritdoc cref="T:StardewValley.Delegates.EventPreconditionDelegate" />
  [OtherNames(new string[] {"b"})]
  public static bool ReachedMineBottom(GameLocation location, string eventId, string[] args)
  {
    int num;
    string error;
    return !ArgUtility.TryGetOptionalInt(args, 1, out num, out error, 1, "int minTimes") ? Event.LogPreconditionError(location, eventId, args, error) : Game1.player.timesReachedMineBottom >= num;
  }

  /// <summary>The current year is exactly 1 (if specified 1) or at least the given value (if specified any other value).</summary>
  /// <inheritdoc cref="T:StardewValley.Delegates.EventPreconditionDelegate" />
  [OtherNames(new string[] {"y"})]
  public static bool Year(GameLocation location, string eventId, string[] args)
  {
    int num;
    string error;
    if (!ArgUtility.TryGetInt(args, 1, out num, out error, "int desiredYear"))
      return Event.LogPreconditionError(location, eventId, args, error);
    return num != 1 ? Game1.year >= num : Game1.year == 1;
  }

  /// <summary>The current farmer has this gender.</summary>
  /// <inheritdoc cref="T:StardewValley.Delegates.EventPreconditionDelegate" />
  [OtherNames(new string[] {"g"})]
  public static bool Gender(GameLocation location, string eventId, string[] args)
  {
    string str;
    string error;
    return !ArgUtility.TryGet(args, 1, out str, out error, false, "string gender") ? Event.LogPreconditionError(location, eventId, args, error) : Game1.player.IsMale == str.EqualsIgnoreCase("male");
  }

  /// <summary>The current farmer has this item ID in their inventory.</summary>
  /// <inheritdoc cref="T:StardewValley.Delegates.EventPreconditionDelegate" />
  [OtherNames(new string[] {"i"})]
  public static bool HasItem(GameLocation location, string eventId, string[] args)
  {
    string itemId;
    string error;
    if (!ArgUtility.TryGet(args, 1, out itemId, out error, false, "string itemId"))
      return Event.LogPreconditionError(location, eventId, args, error);
    if (Game1.player.Items.ContainsId(itemId))
      return true;
    return Game1.player.ActiveObject != null && ItemRegistry.HasItemId((Item) Game1.player.ActiveObject, itemId);
  }

  /// <summary>The current farmer has NOT seen this event.</summary>
  /// <inheritdoc cref="T:StardewValley.Delegates.EventPreconditionDelegate" />
  [Obsolete("New events should use !SawEvent instead.")]
  [OtherNames(new string[] {"k"})]
  public static bool NotSawEvent(GameLocation location, string eventId, string[] args)
  {
    return !Preconditions.SawEvent(location, eventId, args);
  }

  /// <summary>The current farmer is standing on one of these tile positions.</summary>
  /// <inheritdoc cref="T:StardewValley.Delegates.EventPreconditionDelegate" />
  [OtherNames(new string[] {"a"})]
  public static bool Tile(GameLocation location, string eventId, string[] args)
  {
    Point point1;
    if (!Game1.isWarping)
    {
      DedicatedServer dedicatedServer = Game1.dedicatedServer;
      if ((dedicatedServer != null ? (dedicatedServer.FakeWarp ? 1 : 0) : 0) == 0)
      {
        point1 = Game1.player.TilePoint;
        goto label_4;
      }
    }
    point1 = new Point(Game1.xLocationAfterWarp, Game1.yLocationAfterWarp);
label_4:
    Point point2 = point1;
    for (int index = 1; index < args.Length - 1; index += 2)
    {
      Point point3;
      string error;
      if (!ArgUtility.TryGetPoint(args, index, out point3, out error, "Point tile"))
        return Event.LogPreconditionError(location, eventId, args, error);
      if (point3 == point2)
        return true;
    }
    return false;
  }

  /// <summary>The current player has this active dialogue event.</summary>
  /// <inheritdoc cref="T:StardewValley.Delegates.EventPreconditionDelegate" />
  public static bool ActiveDialogueEvent(GameLocation location, string eventId, string[] args)
  {
    string key;
    string error;
    return !ArgUtility.TryGet(args, 1, out key, out error, false, "string id") ? Event.LogPreconditionError(location, eventId, args, error) : Game1.player.activeDialogueEvents.ContainsKey(key);
  }

  /// <summary>The current player does NOT have this active dialogue event.</summary>
  /// <inheritdoc cref="T:StardewValley.Delegates.EventPreconditionDelegate" />
  [Obsolete("New events should use !ActiveDialogueEvent instead.")]
  [OtherNames(new string[] {"A"})]
  public static bool NotActiveDialogueEvent(GameLocation location, string eventId, string[] args)
  {
    return !Preconditions.ActiveDialogueEvent(location, eventId, args);
  }

  /// <summary>Send the specified mail and end the event. This is a way to send mail without actually starting the event, it's not a regular event precondition.</summary>
  /// <inheritdoc cref="T:StardewValley.Delegates.EventPreconditionDelegate" />
  [Obsolete("This is a deprecated way to send mail using a hidden pseudo-event. Newer code should use Data/TriggerActions instead.")]
  [OtherNames(new string[] {"x"})]
  public static bool SendMail(GameLocation location, string eventId, string[] args)
  {
    string mailName;
    string error;
    bool flag;
    if (!ArgUtility.TryGet(args, 1, out mailName, out error, false, "string mailId") || !ArgUtility.TryGetOptionalBool(args, 2, out flag, out error, name: "bool inMailboxToday"))
      return Event.LogPreconditionError(location, eventId, args, error);
    if (flag)
      Game1.player.mailbox.Add(mailName);
    else
      Game1.addMailForTomorrow(mailName);
    Game1.player.eventsSeen.Add(eventId);
    return false;
  }

  /// <summary>Today is one of the given days of month.</summary>
  /// <inheritdoc cref="T:StardewValley.Delegates.EventPreconditionDelegate" />
  [OtherNames(new string[] {"u"})]
  public static bool DayOfMonth(GameLocation location, string eventId, string[] args)
  {
    bool flag = false;
    for (int index = 1; index < args.Length; ++index)
    {
      int num;
      string error;
      if (!ArgUtility.TryGetInt(args, index, out num, out error, "int day"))
        return Event.LogPreconditionError(location, eventId, args, error);
      if (Game1.dayOfMonth == num)
      {
        flag = true;
        break;
      }
    }
    return flag;
  }

  /// <summary>A festival day will occur within the given number of days.</summary>
  /// <inheritdoc cref="T:StardewValley.Delegates.EventPreconditionDelegate" />
  public static bool UpcomingFestival(GameLocation location, string eventId, string[] args)
  {
    int num;
    string error;
    if (!ArgUtility.TryGetInt(args, 1, out num, out error, "int numberOfDays"))
      return Event.LogPreconditionError(location, eventId, args, error);
    StardewValley.Season season = Game1.season;
    int seasonIndex = Game1.seasonIndex;
    int day = Game1.dayOfMonth;
    for (int index = 0; index < num; ++index)
    {
      if (Utility.isFestivalDay(day, season))
        return true;
      ++day;
      if (day > 28)
      {
        day = 1;
        season = (StardewValley.Season) ((seasonIndex + 1) % 4);
      }
    }
    return false;
  }

  /// <summary>There is no festival planned within the given number of days.</summary>
  /// <inheritdoc cref="T:StardewValley.Delegates.EventPreconditionDelegate" />
  [Obsolete("New events should use !UpcomingFestival instead.")]
  [OtherNames(new string[] {"U"})]
  public static bool NotUpcomingFestival(GameLocation location, string eventId, string[] args)
  {
    return !Preconditions.UpcomingFestival(location, eventId, args);
  }

  /// <summary>A game state query matches.</summary>
  /// <inheritdoc cref="T:StardewValley.Delegates.EventPreconditionDelegate" />
  [OtherNames(new string[] {"G"})]
  public static bool GameStateQuery(GameLocation location, string eventId, string[] args)
  {
    string queryString = ArgUtility.UnsplitQuoteAware(args, ' ', 1);
    return string.IsNullOrWhiteSpace(queryString) ? Event.LogPreconditionError(location, eventId, args, "must specify a game state query") : StardewValley.GameStateQuery.CheckConditions(queryString, location);
  }

  /// <summary>The current farmer has a minimum skill level.</summary>
  /// <inheritdoc cref="T:StardewValley.Delegates.EventPreconditionDelegate" />
  public static bool Skill(GameLocation location, string eventId, string[] args)
  {
    string name;
    string error;
    int num;
    return !ArgUtility.TryGet(args, 1, out name, out error, false, "string name") || !ArgUtility.TryGetInt(args, 2, out num, out error, "int minSkillLevel") ? Event.LogPreconditionError(location, eventId, args, error) : Game1.player.GetUnmodifiedSkillLevel(Farmer.getSkillNumberFromName(name)) >= num;
  }
}
