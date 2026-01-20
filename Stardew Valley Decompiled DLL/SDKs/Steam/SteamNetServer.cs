// Decompiled with JetBrains decompiler
// Type: StardewValley.SDKs.Steam.SteamNetServer
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using StardewValley.Network;
using StardewValley.SDKs.GogGalaxy;
using StardewValley.SDKs.Steam.Internal;
using Steamworks;
using System;
using System.Collections.Generic;

#nullable disable
namespace StardewValley.SDKs.Steam;

/// <summary>Creates an instance of the <see cref="T:StardewValley.SDKs.Steam.SteamNetServer" />.</summary>
internal sealed class SteamNetServer(IGameServer gameServer) : HookableServer(gameServer)
{
  /// <summary>The max number of messages we can receive in a single frame.</summary>
  private const int ServerBufferSize = 256 /*0x0100*/;
  /// <summary>The bit mask to check if a player entered the lobby.</summary>
  private const int FlagsLobbyEntered = 1;
  /// <summary>The bit mask to check if a player left the lobby for any reason.</summary>
  private const int FlagsLobbyLeft = 30;
  /// <summary>The callback used to check the result of creating a Steam lobby.</summary>
  private CallResult<LobbyCreated_t> LobbyCreatedCallResult;
  /// <summary>The callback used to handle changes in the connection state (connecting, connected, disconnected, etc).</summary>
  private Callback<SteamNetConnectionStatusChangedCallback_t> SteamNetConnectionStatusChangedCallback;
  /// <summary>The callback used to handle changes to chat room members (joined the lobby, left the lobby, etc).</summary>
  private Callback<LobbyChatUpdate_t> LobbyChatUpdateCallback;
  /// <summary>A local copy of the lobby data, in case the Steam lobby is not ready.</summary>
  private Dictionary<string, string> LobbyData;
  /// <summary>The connection data by Steam Networking Socket.</summary>
  private Dictionary<HSteamNetConnection, ConnectionData> ConnectionDataMap;
  /// <summary>The connection data by farmer ID.</summary>
  private Dictionary<long, ConnectionData> FarmerConnectionMap;
  /// <summary>The cached display names of Steam lobby members.</summary>
  private Dictionary<CSteamID, string> CachedDisplayNames;
  /// <summary>The connections that changed poll groups during a call to <see cref="M:StardewValley.SDKs.Steam.SteamNetServer.PollJoiningMessages" />.</summary>
  private HashSet<HSteamNetConnection> RecentlyJoined;
  /// <summary>The pointers to received messages.</summary>
  private readonly IntPtr[] Messages = new IntPtr[256 /*0x0100*/];
  /// <summary>The Steam ID of the game server's lobby.</summary>
  private CSteamID Lobby;
  /// <summary>The Steam socket used to handle incoming connections.</summary>
  private HSteamListenSocket ListenSocket = HSteamListenSocket.Invalid;
  /// <summary>The poll group used for connections that have not selected a farmhand.</summary>
  private HSteamNetPollGroup JoiningGroup = HSteamNetPollGroup.Invalid;
  /// <summary>The poll group used for connections currently playing as a farmhand.</summary>
  private HSteamNetPollGroup FarmhandGroup = HSteamNetPollGroup.Invalid;
  /// <summary>The privacy setting for the server's lobby.</summary>
  private ServerPrivacy Privacy;

  /// <summary>Applies the privacy setting from <see cref="F:StardewValley.SDKs.Steam.SteamNetServer.Privacy" /> to the game server's lobby.</summary>
  private void UpdateLobbyPrivacy()
  {
    if (!this.Lobby.IsValid())
      return;
    ELobbyType eLobbyType;
    switch (this.Privacy)
    {
      case ServerPrivacy.FriendsOnly:
        eLobbyType = ELobbyType.k_ELobbyTypeFriendsOnly;
        break;
      case ServerPrivacy.Public:
        eLobbyType = ELobbyType.k_ELobbyTypePublic;
        break;
      default:
        eLobbyType = ELobbyType.k_ELobbyTypePrivate;
        break;
    }
    SteamMatchmaking.SetLobbyType(this.Lobby, eLobbyType);
  }

  /// <summary>Converts a <see cref="T:StardewValley.SDKs.Steam.Internal.ConnectionData" /> to a connection string to be used in the Stardew API.</summary>
  /// <param name="connection">The connection data to convert to a unique connection string.</param>
  /// <returns>Returns a string that uniquely corresponds to the <see cref="T:StardewValley.SDKs.Steam.Internal.ConnectionData" />.</returns>
  private string ConnectionDataToId(ConnectionData connection)
  {
    return $"SN_{connection.SteamId.m_SteamID}_{connection.Connection.m_HSteamNetConnection}";
  }

  /// <summary>Gets the internal <see cref="T:StardewValley.SDKs.Steam.Internal.ConnectionData" /> for a corresponding connection string.</summary>
  /// <param name="connectionId">The unique connection string to fetch <see cref="T:StardewValley.SDKs.Steam.Internal.ConnectionData" /> for.</param>
  /// <returns>Returns the <see cref="T:StardewValley.SDKs.Steam.Internal.ConnectionData" /> bookkeeping object that corresponds to the <paramref name="connectionId" /> string, or <c>null</c> if not found.</returns>
  private ConnectionData IdToConnectionData(string connectionId)
  {
    if (connectionId.Length <= 3 || !connectionId.StartsWith("SN_"))
      return (ConnectionData) null;
    string str = connectionId.Substring(3);
    int length = str.IndexOf('_');
    if (length < 0)
      return (ConnectionData) null;
    CSteamID csteamId = new CSteamID();
    ulong ulSteamID = csteamId.m_SteamID;
    uint num = HSteamNetConnection.Invalid.m_HSteamNetConnection;
    try
    {
      ulSteamID = Convert.ToUInt64(str.Substring(0, length));
      num = Convert.ToUInt32(str.Substring(length + 1));
    }
    catch (Exception ex)
    {
    }
    csteamId = new CSteamID(ulSteamID);
    if (!csteamId.IsValid())
      return (ConnectionData) null;
    ConnectionData connectionData;
    if (!this.ConnectionDataMap.TryGetValue(HSteamNetConnection.Invalid with
    {
      m_HSteamNetConnection = num
    }, out connectionData))
      return (ConnectionData) null;
    return (long) connectionData.SteamId.m_SteamID != (long) ulSteamID ? (ConnectionData) null : connectionData;
  }

  public override bool isConnectionActive(string connectionId)
  {
    return this.IdToConnectionData(connectionId) != null;
  }

  public override int connectionsCount
  {
    get
    {
      Dictionary<HSteamNetConnection, ConnectionData> connectionDataMap = this.ConnectionDataMap;
      return connectionDataMap == null ? 0 : __nonvirtual (connectionDataMap.Count);
    }
  }

  public override string getUserId(long farmerId)
  {
    ConnectionData connectionData;
    return !this.FarmerConnectionMap.TryGetValue(farmerId, out connectionData) ? (string) null : connectionData.SteamId.m_SteamID.ToString();
  }

  public override bool hasUserId(string userId)
  {
    CSteamID csteamId = new CSteamID();
    try
    {
      csteamId = new CSteamID(Convert.ToUInt64(userId));
    }
    catch (Exception ex)
    {
    }
    if (!csteamId.IsValid())
      return false;
    foreach (KeyValuePair<HSteamNetConnection, ConnectionData> connectionData in this.ConnectionDataMap)
    {
      if ((long) connectionData.Value.SteamId.m_SteamID == (long) csteamId.m_SteamID)
        return true;
    }
    return false;
  }

  public override string getUserName(long farmerId)
  {
    ConnectionData connectionData;
    if (!this.FarmerConnectionMap.TryGetValue(farmerId, out connectionData))
      return "";
    string userName = SteamFriends.GetFriendPersonaName(connectionData.SteamId);
    if (string.IsNullOrWhiteSpace(userName) || userName == "[unknown]")
      userName = connectionData.DisplayName;
    connectionData.DisplayName = userName;
    return userName;
  }

  public override void setPrivacy(ServerPrivacy privacy)
  {
    this.Privacy = privacy;
    this.UpdateLobbyPrivacy();
  }

  public override bool connected()
  {
    return this.Lobby.IsValid() && this.Lobby.IsLobby() && this.ListenSocket != HSteamListenSocket.Invalid && this.JoiningGroup != HSteamNetPollGroup.Invalid && this.FarmhandGroup != HSteamNetPollGroup.Invalid;
  }

  /// <summary>Handles new incoming connections, and rejects users that are banned.</summary>
  /// <param name="evt">The data about the incoming client connection.</param>
  /// <param name="steamId">The Steam ID of the connecting client.</param>
  private void OnConnecting(SteamNetConnectionStatusChangedCallback_t evt, CSteamID steamId)
  {
    Game1.log.Verbose($"{steamId.m_SteamID} connecting to server");
    if (this.gameServer.isUserBanned(steamId.m_SteamID.ToString()))
    {
      Game1.log.Verbose($"{steamId.m_SteamID} is banned");
      this.ShutdownConnection(evt.m_hConn);
    }
    else
    {
      SteamFriends.RequestUserInformation(steamId, true);
      int num = (int) SteamNetworkingSockets.AcceptConnection(evt.m_hConn);
    }
  }

  /// <summary>Handles newly connected clients, and creates internal bookkeeping structures for the connection.</summary>
  /// <param name="evt">A structure containing data about the newly connected client.</param>
  /// <param name="steamId">The Steam ID of the connected client.</param>
  private void OnConnected(SteamNetConnectionStatusChangedCallback_t evt, CSteamID steamId)
  {
    Game1.log.Verbose($"{steamId.m_SteamID} connected to server");
    string valueOrDefault = this.CachedDisplayNames.GetValueOrDefault<CSteamID, string>(steamId);
    ConnectionData connection = new ConnectionData(evt.m_hConn, steamId, valueOrDefault);
    this.ConnectionDataMap[evt.m_hConn] = connection;
    SteamNetworkingSockets.SetConnectionPollGroup(evt.m_hConn, this.JoiningGroup);
    string id = this.ConnectionDataToId(connection);
    this.onConnect(id);
    this.gameServer.sendAvailableFarmhands("", id, (Action<OutgoingMessage>) (outgoing => this.SendMessageToConnection(evt.m_hConn, outgoing)));
  }

  /// <summary>Handles client disconnects, and cleans up all bookkeeping data about the connection.</summary>
  /// <param name="evt">The data about the disconnected client.</param>
  /// <param name="steamId">The Steam ID of the disconnected client.</param>
  private void OnDisconnected(SteamNetConnectionStatusChangedCallback_t evt, CSteamID steamId)
  {
    if (!steamId.IsValid())
      return;
    Game1.log.Verbose($"{steamId.m_SteamID} disconnected from server");
    ConnectionData connection;
    if (!this.ConnectionDataMap.TryGetValue(evt.m_hConn, out connection))
    {
      SteamSocketUtils.CloseConnection(evt.m_hConn);
    }
    else
    {
      this.onDisconnect(this.ConnectionDataToId(connection));
      if (connection.Online)
        this.playerDisconnected(connection.FarmerId);
      this.ConnectionDataMap.Remove(evt.m_hConn);
      SteamSocketUtils.CloseConnection(evt.m_hConn);
    }
  }

  /// <summary>Handles clients disconnected via <see cref="M:Steamworks.SteamNetworkingSockets.CloseConnection(Steamworks.HSteamNetConnection,System.Int32,System.String,System.Boolean)" />, and cleans up all bookkeeping data about the connection.</summary>
  /// <param name="connection">The connection to clean up.</param>
  private void OnDisconnected(HSteamNetConnection connection)
  {
    SteamNetConnectionStatusChangedCallback_t evt = new SteamNetConnectionStatusChangedCallback_t()
    {
      m_hConn = connection,
      m_eOldState = ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_Connected
    };
    SteamNetworkingSockets.GetConnectionInfo(connection, out evt.m_info);
    this.OnDisconnected(evt, evt.m_info.m_identityRemote.GetSteamID());
  }

  /// <summary>Handles all changes in client connection status, and invokes the corresponding handler.</summary>
  /// <param name="evt">A structure containing data about the client whose connection status changed.</param>
  private void OnSteamNetConnectionStatusChanged(SteamNetConnectionStatusChangedCallback_t evt)
  {
    switch (evt.m_info.m_eState)
    {
      case ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_Connecting:
        this.OnConnecting(evt, evt.m_info.m_identityRemote.GetSteamID());
        break;
      case ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_Connected:
        this.OnConnected(evt, evt.m_info.m_identityRemote.GetSteamID());
        break;
      case ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_ClosedByPeer:
      case ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_ProblemDetectedLocally:
        this.OnDisconnected(evt, evt.m_info.m_identityRemote.GetSteamID());
        break;
    }
  }

  /// <summary>Handles all changes in lobby member status.</summary>
  /// <param name="evt">A structure containing data about the changes to a lobby member.</param>
  private void OnLobbyChatUpdate(LobbyChatUpdate_t evt)
  {
    if ((long) evt.m_ulSteamIDLobby != (long) this.Lobby.m_SteamID)
      return;
    CSteamID csteamId = new CSteamID(evt.m_ulSteamIDUserChanged);
    if (((int) evt.m_rgfChatMemberStateChange & 1) != 0)
    {
      this.CachedDisplayNames[csteamId] = SteamFriends.GetFriendPersonaName(csteamId);
    }
    else
    {
      if (((int) evt.m_rgfChatMemberStateChange & 30) == 0)
        return;
      this.CachedDisplayNames.Remove(csteamId);
    }
  }

  /// <summary>Handles the result of Steam lobby creation.</summary>
  /// <param name="evt">The data for the Lobby creation event.</param>
  /// <param name="ioFailure">Whether creating the lobby failed due to an I/O error.</param>
  /// <returns>Returns an error indicating why creation failed, if applicable.</returns>
  private string OnLobbyCreatedHelper(LobbyCreated_t evt, bool ioFailure)
  {
    if (ioFailure)
      return "IO Failure";
    switch (evt.m_eResult)
    {
      case EResult.k_EResultOK:
        this.Lobby = new CSteamID(evt.m_ulSteamIDLobby);
        return (string) null;
      case EResult.k_EResultNoConnection:
        return "No connection to Steam";
      case EResult.k_EResultAccessDenied:
        return "Steam denied access";
      case EResult.k_EResultTimeout:
        return "Steam timed out";
      case EResult.k_EResultLimitExceeded:
        return "Too many Steam lobbies created";
      default:
        return "Unknown Steam failure";
    }
  }

  /// <summary>Handles the result of Steam lobby creation.</summary>
  /// <param name="evt">The data for the Lobby creation event.</param>
  /// <param name="ioFailure">Whether creating the lobby failed due to an I/O error.</param>
  private void OnLobbyCreated(LobbyCreated_t evt, bool ioFailure)
  {
    string str = this.OnLobbyCreatedHelper(evt, ioFailure);
    if (str == null)
    {
      SteamNetworkingConfigValue_t[] networkingOptions = SteamSocketUtils.GetNetworkingOptions();
      this.ListenSocket = SteamNetworkingSockets.CreateListenSocketP2P(0, networkingOptions.Length, networkingOptions);
      this.JoiningGroup = SteamNetworkingSockets.CreatePollGroup();
      this.FarmhandGroup = SteamNetworkingSockets.CreatePollGroup();
      SteamMatchmaking.SetLobbyGameServer(this.Lobby, 0U, (ushort) 0, SteamUser.GetSteamID());
      foreach (KeyValuePair<string, string> keyValuePair in this.LobbyData)
        SteamMatchmaking.SetLobbyData(this.Lobby, keyValuePair.Key, keyValuePair.Value);
      SteamMatchmaking.SetLobbyJoinable(this.Lobby, true);
      this.UpdateLobbyPrivacy();
      Game1.log.Verbose($"Steam server successfully created lobby {this.Lobby.m_SteamID}");
      if (!(this.gameServer is StardewValley.Network.GameServer gameServer))
        return;
      foreach (Server server in gameServer.servers)
      {
        if (server is GalaxyNetServer galaxyNetServer)
        {
          galaxyNetServer.setLobbyData("SteamLobbyId", this.Lobby.m_SteamID.ToString());
          Game1.log.Verbose("Updated Galaxy server with Steam lobby info");
          break;
        }
      }
    }
    else
      Game1.log.Verbose($"Server failed to create lobby ({str})");
  }

  public override void initialize()
  {
    Game1.log.Verbose("Starting Steam server");
    this.LobbyCreatedCallResult = CallResult<LobbyCreated_t>.Create(new CallResult<LobbyCreated_t>.APIDispatchDelegate(this.OnLobbyCreated));
    this.SteamNetConnectionStatusChangedCallback = Callback<SteamNetConnectionStatusChangedCallback_t>.Create(new Callback<SteamNetConnectionStatusChangedCallback_t>.DispatchDelegate(this.OnSteamNetConnectionStatusChanged));
    this.LobbyChatUpdateCallback = Callback<LobbyChatUpdate_t>.Create(new Callback<LobbyChatUpdate_t>.DispatchDelegate(this.OnLobbyChatUpdate));
    this.LobbyData = new Dictionary<string, string>();
    this.ConnectionDataMap = new Dictionary<HSteamNetConnection, ConnectionData>();
    this.FarmerConnectionMap = new Dictionary<long, ConnectionData>();
    this.CachedDisplayNames = new Dictionary<CSteamID, string>();
    this.RecentlyJoined = new HashSet<HSteamNetConnection>();
    this.LobbyData["protocolVersion"] = Multiplayer.protocolVersion;
    this.Lobby.Clear();
    this.ListenSocket = HSteamListenSocket.Invalid;
    this.JoiningGroup = HSteamNetPollGroup.Invalid;
    this.FarmhandGroup = HSteamNetPollGroup.Invalid;
    this.Privacy = Game1.options.serverPrivacy;
    this.LobbyCreatedCallResult.Set(SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePrivate, Game1.multiplayer.playerLimit * 2));
  }

  public override void stopServer()
  {
    Game1.log.Verbose("Stopping Steam server");
    foreach (KeyValuePair<HSteamNetConnection, ConnectionData> connectionData in this.ConnectionDataMap)
      this.ShutdownConnection(connectionData.Value.Connection);
    if (this.Lobby.IsValid())
      SteamMatchmaking.LeaveLobby(this.Lobby);
    if (this.ListenSocket != HSteamListenSocket.Invalid)
    {
      SteamNetworkingSockets.CloseListenSocket(this.ListenSocket);
      this.ListenSocket = HSteamListenSocket.Invalid;
    }
    if (this.JoiningGroup != HSteamNetPollGroup.Invalid)
    {
      SteamNetworkingSockets.DestroyPollGroup(this.JoiningGroup);
      this.JoiningGroup = HSteamNetPollGroup.Invalid;
    }
    if (this.FarmhandGroup != HSteamNetPollGroup.Invalid)
    {
      SteamNetworkingSockets.DestroyPollGroup(this.FarmhandGroup);
      this.FarmhandGroup = HSteamNetPollGroup.Invalid;
    }
    this.SteamNetConnectionStatusChangedCallback?.Unregister();
    this.LobbyChatUpdateCallback?.Unregister();
  }

  /// <summary>Handles an incoming <see cref="F:StardewValley.Multiplayer.playerIntroduction" /> message.</summary>
  /// <param name="message">The incoming <see cref="F:StardewValley.Multiplayer.playerIntroduction" /> message containing information about the requested farmhand.</param>
  /// <param name="connectionData">The connection data for the player who sent the <paramref name="message" />.</param>
  private void HandleFarmhandRequest(IncomingMessage message, ConnectionData connectionData)
  {
    NetFarmerRoot farmer = Game1.multiplayer.readFarmer(message.Reader);
    long farmerId = farmer.Value.UniqueMultiplayerID;
    Game1.log.Verbose($"Server received farmhand request from {connectionData.SteamId.m_SteamID} for {farmerId}");
    this.gameServer.checkFarmhandRequest("", this.ConnectionDataToId(connectionData), farmer, (Action<OutgoingMessage>) (outgoing => this.SendMessageToConnection(connectionData.Connection, outgoing)), (Action) (() =>
    {
      Game1.log.Verbose($"Server accepted {connectionData.SteamId.m_SteamID} as farmhand {farmerId}");
      SteamNetworkingSockets.SetConnectionUserData(connectionData.Connection, farmerId);
      SteamNetworkingSockets.SetConnectionPollGroup(connectionData.Connection, this.FarmhandGroup);
      this.RecentlyJoined.Add(connectionData.Connection);
      connectionData.FarmerId = farmerId;
      connectionData.Online = true;
      this.FarmerConnectionMap[farmerId] = connectionData;
    }));
  }

  /// <summary>Receives messages from the <see cref="F:StardewValley.SDKs.Steam.SteamNetServer.JoiningGroup" /> poll group, where all clients without farmhands should be.</summary>
  private void PollJoiningMessages()
  {
    this.RecentlyJoined.Clear();
    int messagesOnPollGroup = SteamNetworkingSockets.ReceiveMessagesOnPollGroup(this.JoiningGroup, this.Messages, 256 /*0x0100*/);
    for (int index = 0; index < messagesOnPollGroup; ++index)
    {
      IncomingMessage message = new IncomingMessage();
      HSteamNetConnection messageConnection;
      SteamSocketUtils.ProcessSteamMessage(this.Messages[index], message, out messageConnection, this.bandwidthLogger);
      ConnectionData connectionData;
      if (!this.ConnectionDataMap.TryGetValue(messageConnection, out connectionData))
      {
        Game1.log.Warn("Tried to process multiplayer message from an invalid connection.");
        this.ShutdownConnection(messageConnection);
      }
      else
      {
        bool isRecentlyJoined = this.RecentlyJoined.Contains(messageConnection);
        if (connectionData.Online && !isRecentlyJoined)
        {
          Game1.log.Warn($"Online farmhand {connectionData.FarmerId} is in the wrong poll group. Closing their connection.");
          this.ShutdownConnection(messageConnection);
        }
        else
          this.OnProcessingMessage(message, (Action<OutgoingMessage>) (outgoing => this.SendMessageToConnection(messageConnection, outgoing)), (Action) (() =>
          {
            if (isRecentlyJoined)
            {
              this.gameServer.processIncomingMessage(message);
            }
            else
            {
              if (message.MessageType != (byte) 2)
                return;
              this.HandleFarmhandRequest(message, connectionData);
            }
          }));
      }
    }
  }

  /// <summary>Receives messages from the <see cref="F:StardewValley.SDKs.Steam.SteamNetServer.FarmhandGroup" /> poll group, where all clients with actively playing farmhands should be.</summary>
  private void PollFarmhandMessages()
  {
    int messagesOnPollGroup = SteamNetworkingSockets.ReceiveMessagesOnPollGroup(this.FarmhandGroup, this.Messages, 256 /*0x0100*/);
    for (int index = 0; index < messagesOnPollGroup; ++index)
    {
      IncomingMessage message = new IncomingMessage();
      HSteamNetConnection messageConnection;
      SteamSocketUtils.ProcessSteamMessage(this.Messages[index], message, out messageConnection, this.bandwidthLogger);
      if (message.MessageType == (byte) 2)
      {
        Game1.log.Warn("Received farmhand request in the wrong poll group. Closing their connection.");
        this.ShutdownConnection(messageConnection);
      }
      else
      {
        ConnectionData connectionData;
        if (!this.ConnectionDataMap.TryGetValue(messageConnection, out connectionData))
        {
          Game1.log.Warn("Tried to process multiplayer message from an invalid connection.");
          this.ShutdownConnection(messageConnection);
        }
        else if (!connectionData.Online)
        {
          Game1.log.Warn("A non-farmhand connection is in the wrong poll group. Closing their connection.");
          this.ShutdownConnection(messageConnection);
        }
        else
          this.OnProcessingMessage(message, (Action<OutgoingMessage>) (outgoing => this.SendMessageToConnection(messageConnection, outgoing)), (Action) (() => this.gameServer.processIncomingMessage(message)));
      }
    }
  }

  public override void receiveMessages()
  {
    if (!this.connected())
      return;
    this.PollJoiningMessages();
    this.PollFarmhandMessages();
    foreach (KeyValuePair<HSteamNetConnection, ConnectionData> connectionData in this.ConnectionDataMap)
    {
      int num = (int) SteamNetworkingSockets.FlushMessagesOnConnection(connectionData.Value.Connection);
    }
  }

  private void SendMessageToConnection(HSteamNetConnection connection, OutgoingMessage message)
  {
    SteamSocketUtils.SendMessage(connection, message, this.bandwidthLogger, new Action<HSteamNetConnection>(this.OnDisconnected));
  }

  public override void sendMessage(long peerId, OutgoingMessage message)
  {
    ConnectionData connectionData;
    if (!this.connected() || !this.FarmerConnectionMap.TryGetValue(peerId, out connectionData) || connectionData.Connection == HSteamNetConnection.Invalid)
      return;
    this.SendMessageToConnection(connectionData.Connection, message);
  }

  public override void setLobbyData(string key, string value)
  {
    if (this.LobbyData == null)
      return;
    this.LobbyData[key] = value;
    if (!this.Lobby.IsValid())
      return;
    SteamMatchmaking.SetLobbyData(this.Lobby, key, value);
  }

  public override void kick(long disconnectee)
  {
    base.kick(disconnectee);
    this.sendMessage(disconnectee, new OutgoingMessage((byte) 23, Game1.player, Array.Empty<object>()));
    ConnectionData connectionData;
    if (!this.FarmerConnectionMap.TryGetValue(disconnectee, out connectionData))
      return;
    this.ShutdownConnection(connectionData.Connection);
  }

  public override void playerDisconnected(long disconnectee)
  {
    if (!this.FarmerConnectionMap.TryGetValue(disconnectee, out ConnectionData _))
      return;
    base.playerDisconnected(disconnectee);
    this.FarmerConnectionMap.Remove(disconnectee);
  }

  public override float getPingToClient(long farmerId)
  {
    ConnectionData connectionData;
    if (!this.FarmerConnectionMap.TryGetValue(farmerId, out connectionData))
      return -1f;
    SteamNetworkingQuickConnectionStatus pStats;
    SteamNetworkingSockets.GetQuickConnectionStatus(connectionData.Connection, out pStats);
    return (float) pStats.m_nPing;
  }

  public override bool canOfferInvite() => this.connected();

  public override void offerInvite()
  {
    if (!this.connected())
      return;
    Program.sdk.Networking.ShowInviteDialog((object) this.Lobby);
  }

  /// <summary>Closes a connection and cleans up the corresponding bookkeeping data.</summary>
  /// <param name="connection">The connection to close and clean up.</param>
  /// <remarks>
  /// In most cases, this should be used instead of calling <see cref="M:StardewValley.SDKs.Steam.Internal.SteamSocketUtils.CloseConnection(Steamworks.HSteamNetConnection,System.Action{Steamworks.HSteamNetConnection})" /> directly,
  /// otherwise the <see cref="M:StardewValley.SDKs.Steam.SteamNetServer.OnDisconnected(Steamworks.SteamNetConnectionStatusChangedCallback_t,Steamworks.CSteamID)" /> handler will not get called. However, the  <see cref="M:StardewValley.SDKs.Steam.SteamNetServer.OnDisconnected(Steamworks.SteamNetConnectionStatusChangedCallback_t,Steamworks.CSteamID)" />
  /// handler itself should not use this method.
  /// </remarks>
  private void ShutdownConnection(HSteamNetConnection connection)
  {
    SteamSocketUtils.CloseConnection(connection, new Action<HSteamNetConnection>(this.OnDisconnected));
  }
}
