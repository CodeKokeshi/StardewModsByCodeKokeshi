// Decompiled with JetBrains decompiler
// Type: StardewValley.Network.Dedicated.DedicatedServer
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using StardewValley.Delegates;
using StardewValley.Extensions;
using StardewValley.Locations;
using StardewValley.Menus;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;

#nullable disable
namespace StardewValley.Network.Dedicated;

public class DedicatedServer
{
  private const string BROADCAST_EVENT_KEY = "BroadcastEvent";
  private readonly ConcurrentQueue<DedicatedServer.FarmerWarp> farmerWarps = new ConcurrentQueue<DedicatedServer.FarmerWarp>();
  private readonly Dictionary<string, Dictionary<string, long>> eventLocks = new Dictionary<string, Dictionary<string, long>>();
  private readonly HashSet<long> onlineIds = new HashSet<long>();
  private readonly HashSet<string> broadcastEvents = new HashSet<string>();
  private readonly HashSet<string> notBroadcastEvents = new HashSet<string>();
  private bool fakeWarp;
  private bool warpingSleep;
  private bool warpingFestival;
  private bool warpingHostBroadcastEvent;
  private bool startedFestivalMainEvent;
  private bool startedFestivalEnd;
  private bool shouldJudgeGrange;
  public bool CheckedHostPrecondition;
  private long fakeFarmerId;

  public bool FakeWarp => Game1.IsDedicatedHost && this.fakeWarp;

  public Farmer FakeFarmer
  {
    get
    {
      if (!Game1.IsDedicatedHost)
        return Game1.player;
      Farmer farmer = Game1.getFarmer(this.fakeFarmerId);
      return !Game1.multiplayer.isDisconnecting(farmer) ? farmer : Game1.player;
    }
  }

  public DedicatedServer() => this.Reset();

  public void Reset()
  {
    this.fakeWarp = false;
    this.warpingSleep = false;
    this.warpingFestival = false;
    this.startedFestivalMainEvent = false;
    this.startedFestivalEnd = false;
    this.shouldJudgeGrange = false;
    this.warpingHostBroadcastEvent = false;
    this.broadcastEvents.Clear();
    this.eventLocks.Clear();
  }

  public void ResetForNewDay()
  {
    if (!Game1.IsDedicatedHost)
      return;
    this.fakeWarp = false;
    this.warpingSleep = false;
    this.warpingFestival = false;
    this.startedFestivalMainEvent = false;
    this.startedFestivalEnd = false;
    this.shouldJudgeGrange = false;
    this.warpingHostBroadcastEvent = false;
    this.eventLocks.Clear();
  }

  private bool TryForceClientHostEvent(
    DedicatedServer.FarmerWarp warp,
    GameLocation location,
    string eventId)
  {
    if (Game1.server == null)
      return false;
    string key = (warp.isStructure ? "1" : "0") + location.NameOrUniqueName;
    Dictionary<string, long> dictionary;
    if (!this.eventLocks.TryGetValue(key, out dictionary))
      this.eventLocks[key] = new Dictionary<string, long>();
    else if (dictionary.ContainsKey(eventId))
      return false;
    this.eventLocks[key][eventId] = warp.who.UniqueMultiplayerID;
    object[] forceEventMessage = Game1.multiplayer.generateForceEventMessage(eventId, location, (int) warp.x, (int) warp.y, true, true);
    Game1.server.sendMessage(warp.who.UniqueMultiplayerID, (byte) 4, Game1.player, forceEventMessage);
    return true;
  }

  private void CheckForWarpEvents(DedicatedServer.FarmerWarp warp)
  {
    if (warp.warpingForForcedRemoteEvent || Game1.eventUp || Game1.farmEvent != null || this.IsWarping())
      return;
    GameLocation locationFromName = Game1.getLocationFromName(warp.name, warp.isStructure);
    Dictionary<string, string> events;
    try
    {
      if (!locationFromName.TryGetLocationEvents(out string _, out events))
        return;
      if (events == null)
        return;
    }
    catch
    {
      return;
    }
    int locationAfterWarp1 = Game1.xLocationAfterWarp;
    int locationAfterWarp2 = Game1.yLocationAfterWarp;
    Game1.xLocationAfterWarp = (int) warp.x;
    Game1.yLocationAfterWarp = (int) warp.y;
    this.fakeWarp = true;
    EventCommandDelegate handler1 = (EventCommandDelegate) null;
    foreach (string key1 in events.Keys)
    {
      this.CheckedHostPrecondition = false;
      string eventId = locationFromName.checkEventPrecondition(key1);
      if (this.CheckedHostPrecondition && !(eventId == "-1") && !string.IsNullOrEmpty(eventId) && GameLocation.IsValidLocationEvent(key1, events[key1]) && (handler1 != null || Event.TryGetEventCommandHandler("BroadcastEvent", out handler1)))
      {
        if (this.notBroadcastEvents.Contains(eventId))
        {
          if (this.TryForceClientHostEvent(warp, locationFromName, eventId))
            break;
        }
        else
        {
          if (this.broadcastEvents.Contains(eventId))
          {
            this.fakeFarmerId = warp.who.UniqueMultiplayerID;
            this.warpingHostBroadcastEvent = true;
            break;
          }
          foreach (string command in Event.ParseCommands(events[key1]))
          {
            string key2 = ArgUtility.Get(ArgUtility.SplitBySpaceQuoteAware(command), 0);
            bool? nullable = key2?.StartsWith("--");
            EventCommandDelegate handler2;
            if (nullable.HasValue && !nullable.GetValueOrDefault() && Event.TryGetEventCommandHandler(key2, out handler2) && handler2 == handler1)
            {
              this.fakeFarmerId = warp.who.UniqueMultiplayerID;
              this.warpingHostBroadcastEvent = true;
              this.broadcastEvents.Add(eventId);
              break;
            }
          }
          if (!this.warpingHostBroadcastEvent)
          {
            this.notBroadcastEvents.Add(eventId);
            if (this.TryForceClientHostEvent(warp, locationFromName, eventId))
              break;
          }
        }
      }
    }
    this.fakeWarp = false;
    Game1.xLocationAfterWarp = locationAfterWarp1;
    Game1.yLocationAfterWarp = locationAfterWarp2;
    if (!this.warpingHostBroadcastEvent)
      return;
    LocationRequest locationRequest = Game1.getLocationRequest(warp.name, warp.isStructure);
    locationRequest.OnWarp += (LocationRequest.Callback) (() => this.warpingHostBroadcastEvent = false);
    Game1.warpFarmer(locationRequest, (int) warp.x, (int) warp.y, warp.facingDirection);
  }

  private bool IsWarping()
  {
    return Game1.isWarping || this.warpingHostBroadcastEvent || this.warpingSleep || this.warpingFestival;
  }

  public void DoHostAction(string action, params object[] data)
  {
    object[] destinationArray = new object[data.Length + 2];
    destinationArray[0] = (object) (byte) 1;
    destinationArray[1] = (object) action;
    Array.Copy((Array) data, 0, (Array) destinationArray, 2, data.Length);
    OutgoingMessage message = new OutgoingMessage((byte) 33, Game1.player, destinationArray);
    if (Game1.IsMasterGame)
    {
      IncomingMessage msg = new IncomingMessage();
      using (MemoryStream memoryStream = new MemoryStream())
      {
        using (BinaryWriter writer = new BinaryWriter((Stream) memoryStream))
        {
          message.Write(writer);
          memoryStream.Seek(0L, SeekOrigin.Begin);
          using (BinaryReader reader = new BinaryReader((Stream) memoryStream))
            msg.Read(reader);
        }
      }
      Game1.multiplayer.processIncomingMessage(msg);
    }
    else if (Game1.HasDedicatedHost)
    {
      if (Game1.client == null)
        return;
      Game1.client.sendMessage(message);
    }
    else
      Game1.log.Error($"Tried to execute a host-only action '{action}' as a client on a non-dedicated server.");
  }

  public void Tick()
  {
    if (!Game1.IsDedicatedHost)
      return;
    this.onlineIds.Clear();
    foreach (Farmer onlineFarmer in Game1.getOnlineFarmers())
    {
      if (!Game1.multiplayer.isDisconnecting(onlineFarmer) && onlineFarmer.UniqueMultiplayerID != Game1.player.UniqueMultiplayerID)
        this.onlineIds.Add(onlineFarmer.UniqueMultiplayerID);
    }
    if (this.onlineIds.Count == 0)
    {
      this.farmerWarps.Clear();
      this.eventLocks.Clear();
      bool? isFestival = Game1.CurrentEvent?.isFestival;
      if (isFestival.HasValue && isFestival.GetValueOrDefault())
      {
        if (Game1.netWorldState.Value.IsPaused)
          Game1.netWorldState.Value.IsPaused = false;
        if (this.startedFestivalEnd)
          return;
        Game1.CurrentEvent.TryStartEndFestivalDialogue(Game1.player);
        this.startedFestivalEnd = true;
      }
      else
      {
        if (Game1.netWorldState.Value.IsPaused)
          return;
        Game1.netWorldState.Value.IsPaused = true;
      }
    }
    else
    {
      if (Game1.netWorldState.Value.IsPaused)
        Game1.netWorldState.Value.IsPaused = false;
      if ((double) Game1.player.Stamina < (double) Game1.player.MaxStamina)
        Game1.player.Stamina = (float) Game1.player.MaxStamina;
      if (Game1.player.health < Game1.player.maxHealth)
        Game1.player.health = Game1.player.maxHealth;
      if (this.eventLocks.Count > 0)
      {
        List<string> stringList1 = new List<string>();
        List<string> stringList2 = new List<string>();
        foreach (KeyValuePair<string, Dictionary<string, long>> eventLock in this.eventLocks)
        {
          stringList2.Clear();
          foreach (KeyValuePair<string, long> keyValuePair in eventLock.Value)
          {
            if (!this.onlineIds.Contains(keyValuePair.Value))
              stringList2.Add(keyValuePair.Key);
          }
          if (eventLock.Value.Count - stringList2.Count <= 0)
          {
            stringList1.Add(eventLock.Key);
          }
          else
          {
            foreach (string key in stringList2)
              eventLock.Value.Remove(key);
          }
        }
        foreach (string key in stringList1)
          this.eventLocks.Remove(key);
      }
      DedicatedServer.FarmerWarp result;
      while (this.farmerWarps.TryDequeue(out result))
      {
        if (result.who != null && this.onlineIds.Contains(result.who.UniqueMultiplayerID))
          this.CheckForWarpEvents(result);
      }
      if (this.IsWarping())
        return;
      if (Game1.activeClickableMenu is DialogueBox activeClickableMenu)
      {
        if (activeClickableMenu.isQuestion)
          activeClickableMenu.selectedResponse = 0;
        activeClickableMenu.receiveLeftClick(0, 0, true);
      }
      if (Game1.CurrentEvent != null)
      {
        if (!Game1.CurrentEvent.skipped && Game1.CurrentEvent.skippable)
        {
          Game1.CurrentEvent.skipped = true;
          Game1.CurrentEvent.skipEvent();
          Game1.freezeControls = false;
        }
        if (Game1.CurrentEvent.isFestival)
        {
          NPC festivalHost = Game1.CurrentEvent.festivalHost;
          if (festivalHost != null && !this.startedFestivalMainEvent && this.CheckOthersReady("MainEvent_" + Game1.CurrentEvent.id))
          {
            Game1.CurrentEvent.answerDialogueQuestion(festivalHost, "yes");
            this.startedFestivalMainEvent = true;
          }
        }
        if (this.startedFestivalEnd || !Game1.CurrentEvent.isFestival || !this.CheckOthersReady("festivalEnd"))
          return;
        Game1.CurrentEvent.TryStartEndFestivalDialogue(Game1.player);
        this.startedFestivalEnd = true;
      }
      else
      {
        if (!this.warpingSleep && this.CheckOthersReady("sleep"))
        {
          if (Game1.currentLocation.NameOrUniqueName.EqualsIgnoreCase(Game1.player.homeLocation.Value))
          {
            this.HostSleepInBed();
          }
          else
          {
            this.warpingSleep = true;
            LocationRequest locationRequest = Game1.getLocationRequest(Game1.player.homeLocation.Value);
            locationRequest.OnWarp += (LocationRequest.Callback) (() => this.HostSleepInBed());
            Game1.warpFarmer(locationRequest, 5, 9, Game1.player.FacingDirection);
          }
        }
        if (this.warpingFestival || Game1.whereIsTodaysFest == null || !this.CheckOthersReady("festivalStart"))
          return;
        this.warpingFestival = true;
        LocationRequest locationRequest1 = Game1.getLocationRequest(Game1.whereIsTodaysFest);
        locationRequest1.OnWarp += (LocationRequest.Callback) (() => this.warpingFestival = false);
        int x = -1;
        int y = -1;
        Utility.getDefaultWarpLocation(Game1.whereIsTodaysFest, ref x, ref y);
        Game1.warpFarmer(locationRequest1, x, y, 2);
      }
    }
  }

  internal void HandleFarmerWarp(DedicatedServer.FarmerWarp warp)
  {
    if (!Game1.IsDedicatedHost || warp.who == null)
      return;
    this.farmerWarps.Enqueue(warp);
  }

  private bool CheckOthersReady(string readyCheck)
  {
    if (readyCheck == "MainEvent_festival_fall16")
      return this.shouldJudgeGrange;
    int numberReady = Game1.netReady.GetNumberReady(readyCheck);
    return numberReady > 0 && !Game1.netReady.IsReady(readyCheck) && numberReady >= Game1.netReady.GetNumberRequired(readyCheck) - 1;
  }

  private void HostSleepInBed()
  {
    if (Game1.currentLocation is FarmHouse currentLocation)
    {
      Game1.player.position.Set(Utility.PointToVector2(currentLocation.GetPlayerBedSpot()) * 64f);
      currentLocation.answerDialogueAction("Sleep_Yes", (string[]) null);
    }
    this.warpingSleep = false;
  }

  private void ProcessEventDone(IncomingMessage message)
  {
    if (message.SourceFarmer == null)
      return;
    string name = message.Reader.ReadString();
    bool flag = message.Reader.ReadByte() > (byte) 0;
    string key = message.Reader.ReadString();
    int num1 = flag ? 1 : 0;
    GameLocation locationFromName = Game1.getLocationFromName(name, num1 != 0);
    Dictionary<string, long> dictionary;
    long num2;
    if (locationFromName == null || !this.eventLocks.TryGetValue((flag ? "1" : "0") + locationFromName.NameOrUniqueName, out dictionary) || !dictionary.TryGetValue(key, out num2) || num2 != message.SourceFarmer.UniqueMultiplayerID)
      return;
    Game1.player.eventsSeen.Add(key);
    dictionary.Remove(key);
  }

  private void ProcessHostAction(IncomingMessage message)
  {
    switch (message.Reader.ReadString())
    {
      case "ChooseCave":
        Event.hostActionChooseCave(message.SourceFarmer, message.Reader);
        break;
      case "NamePet":
        Event.hostActionNamePet(message.SourceFarmer, message.Reader);
        break;
      case "JudgeGrange":
        this.shouldJudgeGrange = true;
        break;
    }
  }

  public void ProcessMessage(IncomingMessage message)
  {
    switch ((DedicatedServerMessageType) message.Reader.ReadByte())
    {
      case DedicatedServerMessageType.EventDone:
        this.ProcessEventDone(message);
        break;
      case DedicatedServerMessageType.HostAction:
        this.ProcessHostAction(message);
        break;
    }
  }

  public class FarmerWarp
  {
    public Farmer who;
    public string name;
    public int facingDirection;
    public short x;
    public short y;
    public bool isStructure;
    public bool warpingForForcedRemoteEvent;

    public FarmerWarp(
      Farmer who,
      short x,
      short y,
      string name,
      bool isStructure,
      int facingDirection,
      bool warpingForForcedRemoteEvent)
    {
      this.who = who;
      this.name = name;
      this.facingDirection = facingDirection;
      this.x = x;
      this.y = y;
      this.isStructure = isStructure;
      this.warpingForForcedRemoteEvent = warpingForForcedRemoteEvent;
    }
  }
}
