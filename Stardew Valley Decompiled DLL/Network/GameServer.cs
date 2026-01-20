// Decompiled with JetBrains decompiler
// Type: StardewValley.Network.GameServer
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Lidgren.Network;
using Microsoft.Xna.Framework;
using Netcode;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Minigames;
using StardewValley.Network.Dedicated;
using StardewValley.SaveSerialization;
using StardewValley.SDKs.Steam;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley.Network;

public class GameServer : IGameServer, IBandwidthMonitor
{
  internal List<Server> servers = new List<Server>();
  private Dictionary<Action, Func<bool>> pendingGameAvailableActions = new Dictionary<Action, Func<bool>>();
  /// <summary>A set of connections that are waiting to receive the list of available farmhands.</summary>
  private readonly HashSet<string> pendingAvailableFarmhands = new HashSet<string>();
  private List<Action> completedPendingActions = new List<Action>();
  private List<string> bannedUsers = new List<string>();
  protected bool _wasConnected;
  protected bool _isLocalMultiplayerInitiatedServer;

  public GameServer(bool local_multiplayer = false)
  {
    if (Game1.options != null)
      Game1.options.enableServer = true;
    this.servers.Add(Game1.multiplayer.InitServer((Server) new LidgrenServer((IGameServer) this)));
    this._isLocalMultiplayerInitiatedServer = local_multiplayer;
    if (this._isLocalMultiplayerInitiatedServer || Program.sdk.Networking == null)
      return;
    if (Program.sdk.Networking is SteamNetHelper networking)
      this.servers.Add(networking.CreateSteamServer((IGameServer) this));
    Server server = Program.sdk.Networking.CreateServer((IGameServer) this);
    if (server == null)
      return;
    this.servers.Add(server);
  }

  public int connectionsCount
  {
    get => this.servers.Sum<Server>((Func<Server, int>) (s => s.connectionsCount));
  }

  public bool isConnectionActive(string connectionId)
  {
    foreach (Server server in this.servers)
    {
      if (server.isConnectionActive(connectionId))
        return true;
    }
    return false;
  }

  public virtual void onConnect(string connectionID) => this.UpdateLocalOnlyFlag();

  public virtual void onDisconnect(string connectionID) => this.UpdateLocalOnlyFlag();

  public bool IsLocalMultiplayerInitiatedServer() => this._isLocalMultiplayerInitiatedServer;

  public virtual void UpdateLocalOnlyFlag()
  {
    if (!Game1.game1.IsMainInstance)
      return;
    bool flag = true;
    HashSet<long> local_clients = new HashSet<long>();
    GameRunner.instance.ExecuteForInstances((Action<Game1>) (instance =>
    {
      Client client = Game1.client;
      if (client == null && Game1.activeClickableMenu is FarmhandMenu activeClickableMenu2)
        client = activeClickableMenu2.client;
      if (!(client is LidgrenClient lidgrenClient2))
        return;
      local_clients.Add(lidgrenClient2.client.UniqueIdentifier);
    }));
    foreach (Server server in this.servers)
    {
      if (server is LidgrenServer lidgrenServer)
      {
        foreach (NetConnection connection in lidgrenServer.server.Connections)
        {
          if (!local_clients.Contains(connection.RemoteUniqueIdentifier))
          {
            flag = false;
            break;
          }
        }
      }
      else if (server.connectionsCount > 0)
      {
        flag = false;
        break;
      }
      if (!flag)
        break;
    }
    if (Game1.hasLocalClientsOnly == flag)
      return;
    Game1.hasLocalClientsOnly = flag;
    if (Game1.hasLocalClientsOnly)
      Game1.log.Verbose("Game has only local clients.");
    else
      Game1.log.Verbose("Game has remote clients.");
  }

  public string getInviteCode()
  {
    foreach (Server server in this.servers)
    {
      string inviteCode = server.getInviteCode();
      if (inviteCode != null)
        return inviteCode;
    }
    return (string) null;
  }

  public string getUserName(long farmerId)
  {
    foreach (Server server in this.servers)
    {
      string userName = server.getUserName(farmerId);
      if (userName != null)
        return userName;
    }
    return (string) null;
  }

  public float getPingToClient(long farmerId)
  {
    foreach (Server server in this.servers)
    {
      if ((double) server.getPingToClient(farmerId) != -1.0)
        return server.getPingToClient(farmerId);
    }
    return -1f;
  }

  protected void initialize()
  {
    foreach (Server server in this.servers)
      server.initialize();
    this.whenGameAvailable(new Action(this.updateLobbyData), (Func<bool>) null);
  }

  public void setPrivacy(ServerPrivacy privacy)
  {
    foreach (Server server in this.servers)
      server.setPrivacy(privacy);
    if (!((NetFieldBase<NetWorldState, NetRef<NetWorldState>>) Game1.netWorldState != (NetRef<NetWorldState>) null) || Game1.netWorldState.Value == null)
      return;
    Game1.netWorldState.Value.ServerPrivacy = privacy;
  }

  public void stopServer()
  {
    if (Game1.chatBox != null)
      Game1.chatBox.addInfoMessage(Game1.content.LoadString("Strings\\UI:Chat_DisablingServer"));
    foreach (Server server in this.servers)
      server.stopServer();
  }

  public void receiveMessages()
  {
    foreach (Server server in this.servers)
      server.receiveMessages();
    this.completedPendingActions.Clear();
    foreach (Action key in this.pendingGameAvailableActions.Keys)
    {
      if (this.pendingGameAvailableActions[key]())
      {
        key();
        this.completedPendingActions.Add(key);
      }
    }
    foreach (Action completedPendingAction in this.completedPendingActions)
      this.pendingGameAvailableActions.Remove(completedPendingAction);
    this.completedPendingActions.Clear();
    if (Game1.chatBox == null)
      return;
    bool flag = this.anyServerConnected();
    if (this._wasConnected == flag)
      return;
    this._wasConnected = flag;
    if (!this._wasConnected)
      return;
    Game1.chatBox.addInfoMessage(Game1.content.LoadString("Strings\\UI:Chat_StartingServer"));
  }

  public void sendMessage(long peerId, OutgoingMessage message)
  {
    foreach (Server server in this.servers)
      server.sendMessage(peerId, message);
  }

  public bool canAcceptIPConnections()
  {
    return this.servers.Select<Server, bool>((Func<Server, bool>) (s => s.canAcceptIPConnections())).Aggregate<bool, bool>(false, (Func<bool, bool, bool>) ((a, b) => a | b));
  }

  public bool canOfferInvite()
  {
    return this.servers.Select<Server, bool>((Func<Server, bool>) (s => s.canOfferInvite())).Aggregate<bool, bool>(false, (Func<bool, bool, bool>) ((a, b) => a | b));
  }

  public void offerInvite()
  {
    foreach (Server server in this.servers)
    {
      if (server.canOfferInvite())
        server.offerInvite();
    }
  }

  public bool anyServerConnected()
  {
    foreach (Server server in this.servers)
    {
      if (server.connected())
        return true;
    }
    return false;
  }

  public bool connected()
  {
    foreach (Server server in this.servers)
    {
      if (!server.connected())
        return false;
    }
    return true;
  }

  public void sendMessage(
    long peerId,
    byte messageType,
    Farmer sourceFarmer,
    params object[] data)
  {
    this.sendMessage(peerId, new OutgoingMessage(messageType, sourceFarmer, data));
  }

  public void sendMessages()
  {
    foreach (Farmer farmer in (IEnumerable<Farmer>) Game1.otherFarmers.Values)
    {
      foreach (OutgoingMessage message in (IEnumerable<OutgoingMessage>) farmer.messageQueue)
        this.sendMessage(farmer.UniqueMultiplayerID, message);
      farmer.messageQueue.Clear();
    }
  }

  public void startServer()
  {
    this._wasConnected = false;
    Game1.log.Verbose("Starting server. Protocol version: " + Multiplayer.protocolVersion);
    this.initialize();
    if ((NetFieldBase<NetWorldState, NetRef<NetWorldState>>) Game1.netWorldState == (NetRef<NetWorldState>) null)
      Game1.netWorldState = new NetRoot<NetWorldState>(new NetWorldState());
    Game1.netWorldState.Clock.InterpolationTicks = 0;
    Game1.netWorldState.Value.UpdateFromGame1();
  }

  public void initializeHost()
  {
    if ((NetFieldBase<Farmer, NetRef<Farmer>>) Game1.serverHost == (NetRef<Farmer>) null)
      Game1.serverHost = new NetFarmerRoot();
    Game1.serverHost.Value = Game1.player;
    foreach (Server server in this.servers)
    {
      if (server.PopulatePlatformData(Game1.player))
        break;
    }
    Game1.serverHost.MarkClean();
    Game1.serverHost.Clock.InterpolationTicks = Game1.multiplayer.defaultInterpolationTicks;
  }

  public void sendServerIntroduction(long peer)
  {
    this.sendMessage(peer, new OutgoingMessage((byte) 1, Game1.serverHost.Value, new object[3]
    {
      (object) Game1.multiplayer.writeObjectFullBytes<Farmer>((NetRoot<Farmer>) Game1.serverHost, new long?(peer)),
      (object) Game1.multiplayer.writeObjectFullBytes<FarmerTeam>(Game1.player.teamRoot, new long?(peer)),
      (object) Game1.multiplayer.writeObjectFullBytes<NetWorldState>(Game1.netWorldState, new long?(peer))
    }));
    foreach (KeyValuePair<long, NetRoot<Farmer>> root in Game1.otherFarmers.Roots)
    {
      if (root.Key != Game1.player.UniqueMultiplayerID && root.Key != peer)
        this.sendMessage(peer, new OutgoingMessage((byte) 2, root.Value.Value, new object[2]
        {
          (object) this.getUserName(root.Value.Value.UniqueMultiplayerID),
          (object) Game1.multiplayer.writeObjectFullBytes<Farmer>(root.Value, new long?(peer))
        }));
    }
  }

  public void kick(long disconnectee)
  {
    foreach (Server server in this.servers)
      server.kick(disconnectee);
  }

  public string ban(long farmerId)
  {
    string key = (string) null;
    foreach (Server server in this.servers)
    {
      key = server.getUserId(farmerId);
      if (key != null)
        break;
    }
    if (key == null || Game1.bannedUsers.ContainsKey(key))
      return (string) null;
    string str = Game1.multiplayer.getUserName(farmerId);
    if (str == "" || str == key)
      str = (string) null;
    Game1.bannedUsers.Add(key, str);
    this.kick(farmerId);
    return key;
  }

  public void playerDisconnected(long disconnectee)
  {
    Farmer sourceFarmer;
    Game1.otherFarmers.TryGetValue(disconnectee, out sourceFarmer);
    Game1.multiplayer.playerDisconnected(disconnectee);
    if (sourceFarmer == null)
      return;
    OutgoingMessage message = new OutgoingMessage((byte) 19, sourceFarmer, Array.Empty<object>());
    foreach (long key in (IEnumerable<long>) Game1.otherFarmers.Keys)
    {
      if (key != disconnectee)
        this.sendMessage(key, message);
    }
  }

  public bool isGameAvailable()
  {
    bool flag1 = Game1.currentMinigame is Intro || Game1.Date.DayOfMonth == 0;
    bool flag2 = Game1.CurrentEvent != null && Game1.CurrentEvent.isWedding;
    bool flag3 = Game1.newDaySync.hasInstance() && !Game1.newDaySync.hasFinished();
    bool flag4 = Game1.player.team.demolishLock.IsLocked();
    return !Game1.isFestival() && !flag2 && !flag1 && !flag3 && !flag4 && Game1.weddingsToday.Count == 0 && Game1.gameMode != (byte) 6;
  }

  public bool whenGameAvailable(Action action, Func<bool> customAvailabilityCheck = null)
  {
    Func<bool> func = customAvailabilityCheck != null ? customAvailabilityCheck : new Func<bool>(this.isGameAvailable);
    if (func())
    {
      action();
      return true;
    }
    this.pendingGameAvailableActions.Add(action, func);
    return false;
  }

  private void rejectFarmhandRequest(
    string userId,
    string connectionId,
    NetFarmerRoot farmer,
    Action<OutgoingMessage> sendMessage)
  {
    this.sendAvailableFarmhands(userId, connectionId, sendMessage);
    Game1.log.Verbose("Rejected request for farmhand " + (farmer.Value != null ? farmer.Value.UniqueMultiplayerID.ToString() : "???"));
  }

  public bool isUserBanned(string userID) => Game1.bannedUsers.ContainsKey(userID);

  private bool authCheck(string userID, Farmer farmhand)
  {
    if (!Game1.options.enableFarmhandCreation && !this.IsLocalMultiplayerInitiatedServer() && !farmhand.isCustomized.Value)
      return false;
    return userID == "" || farmhand.userID.Value == "" || farmhand.userID.Value == userID;
  }

  public bool IsFarmhandAvailable(Farmer farmhand)
  {
    return Game1.netWorldState.Value.TryAssignFarmhandHome(farmhand) && (Utility.getHomeOfFarmer(farmhand) is Cabin homeOfFarmer ? (homeOfFarmer.isInventoryOpen() ? 1 : 0) : 0) == 0;
  }

  public void checkFarmhandRequest(
    string userId,
    string connectionId,
    NetFarmerRoot farmer,
    Action<OutgoingMessage> sendMessage,
    Action approve)
  {
    if (farmer.Value == null)
    {
      this.rejectFarmhandRequest(userId, connectionId, farmer, sendMessage);
    }
    else
    {
      long id = farmer.Value.UniqueMultiplayerID;
      if (this.isGameAvailable())
        Check();
      else
        this.sendAvailableFarmhands(userId, connectionId, sendMessage);

      void Check()
      {
        Farmer farmhand = Game1.netWorldState.Value.farmhandData[farmer.Value.UniqueMultiplayerID];
        if (!this.isConnectionActive(connectionId))
          Game1.log.Verbose($"Rejected request for connection ID {connectionId}: Connection not active.");
        else if (farmhand == null)
        {
          Game1.log.Verbose($"Rejected request for farmhand {id.ToString()}: doesn't exist");
          this.rejectFarmhandRequest(userId, connectionId, farmer, sendMessage);
        }
        else if (!this.authCheck(userId, farmhand))
        {
          Game1.log.Verbose($"Rejected request for farmhand {id.ToString()}: authorization failure {userId} {farmhand.userID.Value}");
          this.rejectFarmhandRequest(userId, connectionId, farmer, sendMessage);
        }
        else if (Game1.otherFarmers.ContainsKey(id) && !Game1.multiplayer.isDisconnecting(id) || Game1.serverHost.Value.UniqueMultiplayerID == id)
        {
          Game1.log.Verbose($"Rejected request for farmhand {id.ToString()}: already in use");
          this.rejectFarmhandRequest(userId, connectionId, farmer, sendMessage);
        }
        else if (!this.IsFarmhandAvailable(farmer.Value))
        {
          Game1.log.Verbose($"Rejected request for farmhand {id.ToString()}: farmhand availability failed");
          this.rejectFarmhandRequest(userId, connectionId, farmer, sendMessage);
        }
        else if (!Game1.netWorldState.Value.TryAssignFarmhandHome(farmer.Value))
        {
          Game1.log.Verbose($"Rejected request for farmhand {id.ToString()}: farmhand has no assigned cabin, and none is available to assign.");
          this.rejectFarmhandRequest(userId, connectionId, farmer, sendMessage);
        }
        else
        {
          Game1.log.Verbose("Approved request for farmhand " + id.ToString());
          approve();
          Game1.updateCellarAssignments();
          Game1.multiplayer.addPlayer(farmer);
          Game1.multiplayer.broadcastPlayerIntroduction(farmer);
          foreach (GameLocation location in (IEnumerable<GameLocation>) Game1.locations)
          {
            if (Game1.multiplayer.isAlwaysActiveLocation(location))
              this.sendLocation(id, location);
          }
          if ((long) farmer.Value.disconnectDay.Value == (long) Game1.MasterPlayer.stats.DaysPlayed)
          {
            GameLocation locationFromName = Game1.getLocationFromName(farmer.Value.disconnectLocation.Value);
            if (locationFromName != null && !Game1.multiplayer.isAlwaysActiveLocation(locationFromName))
              this.sendLocation(id, locationFromName, true);
          }
          else if (!string.IsNullOrEmpty(farmer.Value.lastSleepLocation.Value))
          {
            GameLocation locationFromName = Game1.getLocationFromName(farmer.Value.lastSleepLocation.Value);
            if (locationFromName != null && Game1.isLocationAccessible(locationFromName.Name) && !Game1.multiplayer.isAlwaysActiveLocation(locationFromName))
              this.sendLocation(id, locationFromName, true);
          }
          this.sendServerIntroduction(id);
          this.updateLobbyData();
        }
      }
    }
  }

  public void sendAvailableFarmhands(
    string userId,
    string connectionId,
    Action<OutgoingMessage> sendMessage)
  {
    if (!this.isGameAvailable())
    {
      sendMessage(new OutgoingMessage((byte) 11, Game1.player, new object[1]
      {
        (object) "Strings\\UI:Client_WaitForHostAvailability"
      }));
      if (this.pendingAvailableFarmhands.Contains(connectionId))
      {
        Game1.log.Verbose($"Connection {connectionId} is already waiting to receive available farmhands");
      }
      else
      {
        Game1.log.Verbose("Postponing sending available farmhands to connection ID " + connectionId);
        this.pendingAvailableFarmhands.Add(connectionId);
        this.whenGameAvailable((Action) (() =>
        {
          this.pendingAvailableFarmhands.Remove(connectionId);
          if (this.isConnectionActive(connectionId))
            this.sendAvailableFarmhands(userId, connectionId, sendMessage);
          else
            Game1.log.Verbose($"Failed to send available farmhands to connection ID {connectionId}: Connection not active.");
        }), (Func<bool>) null);
      }
    }
    else
    {
      Game1.log.Verbose("Sending available farmhands to connection ID " + connectionId);
      List<NetRef<Farmer>> netRefList = new List<NetRef<Farmer>>();
      foreach (NetRef<Farmer> netRef in Game1.netWorldState.Value.farmhandData.FieldDict.Values)
      {
        if ((!netRef.Value.isActive() || Game1.multiplayer.isDisconnecting(netRef.Value.UniqueMultiplayerID)) && this.IsFarmhandAvailable(netRef.Value))
          netRefList.Add(netRef);
      }
      using (MemoryStream output = new MemoryStream())
      {
        using (BinaryWriter writer = new BinaryWriter((Stream) output))
        {
          writer.Write(Game1.year);
          writer.Write(Game1.seasonIndex);
          writer.Write(Game1.dayOfMonth);
          writer.Write((byte) netRefList.Count);
          foreach (NetRef<Farmer> netRef in netRefList)
          {
            try
            {
              netRef.Serializer = SaveSerializer.GetSerializer(typeof (Farmer));
              netRef.WriteFull(writer);
            }
            finally
            {
              netRef.Serializer = (XmlSerializer) null;
            }
          }
          output.Seek(0L, SeekOrigin.Begin);
          sendMessage(new OutgoingMessage((byte) 9, Game1.player, new object[1]
          {
            (object) output.ToArray()
          }));
        }
      }
    }
  }

  public T GetServer<T>() where T : Server
  {
    foreach (Server server1 in this.servers)
    {
      if (server1 is T server2)
        return server2;
    }
    return default (T);
  }

  private void sendLocation(long peer, GameLocation location, bool force_current = false)
  {
    this.sendMessage(peer, (byte) 3, Game1.serverHost.Value, new object[2]
    {
      (object) force_current,
      (object) Game1.multiplayer.writeObjectFullBytes<GameLocation>(Game1.multiplayer.locationRoot(location), new long?(peer))
    });
  }

  private void warpFarmer(Farmer farmer, short x, short y, string name, bool isStructure)
  {
    GameLocation location = Game1.RequireLocation(name, isStructure);
    if (Game1.IsMasterGame)
      location.hostSetup();
    farmer.currentLocation = location;
    farmer.Position = new Vector2((float) ((int) x * 64 /*0x40*/), (float) ((int) y * 64 /*0x40*/ - (farmer.Sprite.getHeight() - 32 /*0x20*/) + 16 /*0x10*/));
    this.sendLocation(farmer.UniqueMultiplayerID, location);
  }

  public void processIncomingMessage(IncomingMessage message)
  {
    switch (message.MessageType)
    {
      case 2:
        message.Reader.ReadString();
        Game1.multiplayer.processIncomingMessage(message);
        break;
      case 5:
        short x = message.Reader.ReadInt16();
        short y = message.Reader.ReadInt16();
        string name = message.Reader.ReadString();
        byte num = message.Reader.ReadByte();
        bool isStructure = ((uint) num & 1U) > 0U;
        bool warpingForForcedRemoteEvent = ((uint) num & 2U) > 0U;
        bool flag = ((uint) num & 4U) > 0U;
        int facingDirection = 0;
        if (((int) num & 16 /*0x10*/) != 0)
          facingDirection = 1;
        else if (((int) num & 32 /*0x20*/) != 0)
          facingDirection = 2;
        else if (((int) num & 64 /*0x40*/) != 0)
          facingDirection = 3;
        if (flag)
          this.warpFarmer(message.SourceFarmer, x, y, name, isStructure);
        Game1.dedicatedServer.HandleFarmerWarp(new DedicatedServer.FarmerWarp(message.SourceFarmer, x, y, name, isStructure, facingDirection, warpingForForcedRemoteEvent));
        break;
      case 10:
        long peerID = message.Reader.ReadInt64();
        message.Reader.BaseStream.Position -= 8L;
        if (peerID == Multiplayer.AllPlayers || peerID == Game1.player.UniqueMultiplayerID)
          Game1.multiplayer.processIncomingMessage(message);
        this.rebroadcastClientMessage(message, peerID);
        break;
      default:
        Game1.multiplayer.processIncomingMessage(message);
        break;
    }
    if (!Game1.multiplayer.isClientBroadcastType(message.MessageType))
      return;
    this.rebroadcastClientMessage(message, Multiplayer.AllPlayers);
  }

  private void rebroadcastClientMessage(IncomingMessage message, long peerID)
  {
    OutgoingMessage message1 = new OutgoingMessage(message);
    foreach (long key in (IEnumerable<long>) Game1.otherFarmers.Keys)
    {
      if (key != message.FarmerID && (peerID == Multiplayer.AllPlayers || key == peerID))
        this.sendMessage(key, message1);
    }
  }

  private void setLobbyData(string key, string value)
  {
    foreach (Server server in this.servers)
      server.setLobbyData(key, value);
  }

  private bool unclaimedFarmhandsExist()
  {
    foreach (Farmer farmer in Game1.netWorldState.Value.farmhandData.Values)
    {
      if (farmer.userID.Value == "")
        return true;
    }
    return false;
  }

  public void updateLobbyData()
  {
    this.setLobbyData("farmName", Game1.player.farmName.Value);
    this.setLobbyData("farmType", Convert.ToString(Game1.whichFarm));
    if (Game1.whichFarm == 7)
      this.setLobbyData("modFarmType", Game1.GetFarmTypeID());
    else
      this.setLobbyData("modFarmType", "");
    this.setLobbyData("date", Convert.ToString(WorldDate.Now().TotalDays));
    this.setLobbyData("farmhands", string.Join(",", Game1.getAllFarmhands().Select<Farmer, string>((Func<Farmer, string>) (farmhand => farmhand.userID.Value)).Where<string>((Func<string, bool>) (user => user != ""))));
    this.setLobbyData("newFarmhands", Convert.ToString(Game1.options.enableFarmhandCreation && this.unclaimedFarmhandsExist()));
  }

  public BandwidthLogger BandwidthLogger
  {
    get
    {
      foreach (Server server in this.servers)
      {
        if (server.connectionsCount > 0)
          return server.BandwidthLogger;
      }
      return (BandwidthLogger) null;
    }
  }

  public bool LogBandwidth
  {
    get
    {
      foreach (Server server in this.servers)
      {
        if (server.connectionsCount > 0)
          return server.LogBandwidth;
      }
      return false;
    }
    set
    {
      foreach (Server server in this.servers)
      {
        if (server.connectionsCount > 0)
        {
          server.LogBandwidth = value;
          break;
        }
      }
    }
  }
}
