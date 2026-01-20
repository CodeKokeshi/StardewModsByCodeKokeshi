// Decompiled with JetBrains decompiler
// Type: StardewValley.SDKs.GogGalaxy.GalaxySocket
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Galaxy.Api;
using StardewValley.Network;
using StardewValley.SDKs.GogGalaxy.Internal;
using StardewValley.SDKs.GogGalaxy.Listeners;
using Steamworks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

#nullable enable
namespace StardewValley.SDKs.GogGalaxy;

public class GalaxySocket
{
  public const long Timeout = 30000;
  /// <summary>The key for the multiplayer protocol version in the Galaxy lobby data.</summary>
  public const 
  #nullable disable
  string ProtocolVersionKey = "protocolVersion";
  /// <summary>The key for the host's display name in the Galaxy lobby data.</summary>
  public const string HostNameDataKey = "HostDisplayName";
  /// <summary>The key for the Steam host's ID in the Galaxy lobby data.</summary>
  public const string SteamHostIdDataKey = "SteamHostId";
  /// <summary>The key for the Steam lobby's ID in the Galaxy lobby data.</summary>
  public const string SteamLobbyIdDataKey = "SteamLobbyId";
  private const int SendMaxPacketSize = 1100;
  private const int ReceiveMaxPacketSize = 1300;
  private const long RecreateLobbyDelay = 20000;
  private const long HeartbeatDelay = 8;
  private const byte HeartbeatMessage = 255 /*0xFF*/;
  public bool isRecreatedLobby;
  public bool isFirstRecreateAttempt;
  private GalaxyID selfId;
  private GalaxyID connectingLobbyID;
  private GalaxyID lobby;
  private GalaxyID lobbyOwner;
  private GalaxyLobbyEnteredListener galaxyLobbyEnterCallback;
  private GalaxyLobbyCreatedListener galaxyLobbyCreatedCallback;
  private GalaxyLobbyLeftListener galaxyLobbyLeftCallback;
  private GalaxyLobbyMemberStateListener galaxyLobbyMemberStateCallback;
  private string protocolVersion;
  private bool checkedProcotolVersion;
  private Dictionary<string, string> lobbyData = new Dictionary<string, string>();
  private ServerPrivacy privacy;
  private uint memberLimit;
  private long recreateTimer;
  private long heartbeatTimer;
  private Dictionary<ulong, GalaxyID> connections = new Dictionary<ulong, GalaxyID>();
  private HashSet<ulong> ghosts = new HashSet<ulong>();
  private Dictionary<ulong, MemoryStream> incompletePackets = new Dictionary<ulong, MemoryStream>();

  public int ConnectionCount => this.connections.Count;

  public IEnumerable<GalaxyID> Connections => (IEnumerable<GalaxyID>) this.connections.Values;

  public bool Connected => this.lobby != (GalaxyID) null;

  public GalaxyID LobbyOwner => this.lobbyOwner;

  public GalaxyID Lobby => this.lobby;

  public ulong? InviteDialogLobby => new ulong?();

  public GalaxySocket(string protocolVersion)
  {
    this.protocolVersion = protocolVersion;
    this.checkedProcotolVersion = false;
    this.lobbyData[nameof (protocolVersion)] = protocolVersion;
    this.selfId = GalaxyInstance.User().GetGalaxyID();
    this.galaxyLobbyEnterCallback = new GalaxyLobbyEnteredListener(new Action<GalaxyID, LobbyEnterResult>(this.onGalaxyLobbyEnter));
    this.galaxyLobbyCreatedCallback = new GalaxyLobbyCreatedListener(new Action<GalaxyID, LobbyCreateResult>(this.onGalaxyLobbyCreated));
    this.galaxyLobbyMemberStateCallback = new GalaxyLobbyMemberStateListener(new Action<GalaxyID, GalaxyID, LobbyMemberStateChange>(this.onGalaxyMemberState));
    this.lobbyData["SteamHostId"] = SteamUser.GetSteamID().m_SteamID.ToString();
    this.lobbyData["HostDisplayName"] = SteamFriends.GetPersonaName();
  }

  public string GetInviteCode()
  {
    return this.lobby == (GalaxyID) null ? (string) null : "S" + Base36.Encode(this.lobby.GetRealID());
  }

  private string getConnectionString()
  {
    return this.lobby == (GalaxyID) null ? "" : "-connect-lobby-" + this.lobby.ToUint64().ToString();
  }

  private long getTimeNow() => DateTime.UtcNow.Ticks / 10000L;

  public long GetPingWith(GalaxyID peer) => (long) GalaxyInstance.Networking().GetPingWith(peer);

  private LobbyType privacyToLobbyType(ServerPrivacy privacy)
  {
    switch (privacy)
    {
      case ServerPrivacy.InviteOnly:
        return LobbyType.LOBBY_TYPE_PRIVATE;
      case ServerPrivacy.FriendsOnly:
        return LobbyType.LOBBY_TYPE_FRIENDS_ONLY;
      case ServerPrivacy.Public:
        return LobbyType.LOBBY_TYPE_PUBLIC;
      default:
        throw new ArgumentException($"Unknown server privacy type '{privacy}'");
    }
  }

  public void SetPrivacy(ServerPrivacy privacy)
  {
    this.privacy = privacy;
    this.updateLobbyPrivacy();
  }

  public void CreateLobby(ServerPrivacy privacy, uint memberLimit)
  {
    this.privacy = privacy;
    this.memberLimit = memberLimit;
    this.lobbyOwner = this.selfId;
    this.isRecreatedLobby = false;
    this.tryCreateLobby();
  }

  private void tryCreateLobby()
  {
    Game1.log.Verbose("Creating lobby...");
    if (this.galaxyLobbyLeftCallback != null)
    {
      this.galaxyLobbyLeftCallback.Dispose();
      this.galaxyLobbyLeftCallback = (GalaxyLobbyLeftListener) null;
    }
    this.galaxyLobbyLeftCallback = new GalaxyLobbyLeftListener(new Action<GalaxyID, ILobbyLeftListener.LobbyLeaveReason>(this.onGalaxyLobbyLeft));
    try
    {
      GalaxyInstance.Matchmaking().CreateLobby(this.privacyToLobbyType(this.privacy), this.memberLimit, true, LobbyTopologyType.LOBBY_TOPOLOGY_TYPE_STAR);
    }
    catch (Exception ex)
    {
      Game1.log.Error("Galaxy CreateLobby failed with an exception:", ex);
      this.OnLobbyCreateFailed();
    }
    this.recreateTimer = 0L;
  }

  public void JoinLobby(GalaxyID lobbyId, Action<string> onError)
  {
    try
    {
      this.connectingLobbyID = lobbyId;
      GalaxyInstance.Matchmaking().JoinLobby(this.connectingLobbyID);
    }
    catch (Exception ex)
    {
      Game1.log.Error("Error joining Galaxy lobby.", ex);
      string str1 = Game1.content.LoadString("Strings\\UI:CoopMenu_Failed");
      string str2 = !ex.Message.EndsWith("already joined this lobby") ? $"{str1} ({ex.Message})" : str1 + " (already connected)";
      onError(str2);
      this.Close();
    }
  }

  public void SetLobbyData(string key, string value)
  {
    this.lobbyData[key] = value;
    if (!(this.lobby != (GalaxyID) null))
      return;
    GalaxyInstance.Matchmaking().SetLobbyData(this.lobby, key, value);
  }

  private void updateLobbyPrivacy()
  {
    if (this.lobbyOwner != this.selfId || !(this.lobby != (GalaxyID) null))
      return;
    GalaxyInstance.Matchmaking().SetLobbyType(this.lobby, this.privacyToLobbyType(this.privacy));
  }

  /// <summary>Logs a failure to create a lobby, and attempts to create a new lobby.</summary>
  private void OnLobbyCreateFailed()
  {
    if (Game1.chatBox != null && this.isFirstRecreateAttempt)
    {
      if (this.isRecreatedLobby)
        Game1.chatBox.addInfoMessage(Game1.content.LoadString("Strings\\UI:Chat_LobbyCreateFail"));
      else
        Game1.chatBox.addInfoMessage(Game1.content.LoadString("Strings\\UI:Chat_LobbyCreateFail"));
    }
    this.recreateTimer = this.getTimeNow() + 20000L;
    this.isRecreatedLobby = true;
    this.isFirstRecreateAttempt = false;
  }

  private void onGalaxyLobbyCreated(GalaxyID lobbyID, LobbyCreateResult result)
  {
    if (result != LobbyCreateResult.LOBBY_CREATE_RESULT_ERROR)
      return;
    Game1.log.Error("Failed to create Galaxy lobby.");
    this.OnLobbyCreateFailed();
  }

  /// <summary>A Galaxy lobby listener that logs member state changes (entering the lobby, leaving the lobby, etc.)</summary>
  private void onGalaxyMemberState(
    GalaxyID lobbyID,
    GalaxyID memberID,
    LobbyMemberStateChange memberStateChange)
  {
    switch (memberStateChange)
    {
      case LobbyMemberStateChange.LOBBY_MEMBER_STATE_CHANGED_ENTERED:
        Game1.log.Verbose($"{memberID} connected to lobby {lobbyID}");
        break;
      case LobbyMemberStateChange.LOBBY_MEMBER_STATE_CHANGED_LEFT:
        Game1.log.Verbose($"{memberID} left lobby {lobbyID}");
        break;
      case LobbyMemberStateChange.LOBBY_MEMBER_STATE_CHANGED_DISCONNECTED:
        Game1.log.Verbose($"{memberID} disconnected from lobby {lobbyID} without leaving");
        break;
      case LobbyMemberStateChange.LOBBY_MEMBER_STATE_CHANGED_KICKED:
        Game1.log.Verbose($"{memberID} was kicked from lobby {lobbyID}");
        break;
      case LobbyMemberStateChange.LOBBY_MEMBER_STATE_CHANGED_BANNED:
        Game1.log.Verbose($"{memberID} was banned from lobby {lobbyID}");
        break;
    }
  }

  private void onGalaxyLobbyLeft(GalaxyID lobbyID, ILobbyLeftListener.LobbyLeaveReason leaveReason)
  {
    if (leaveReason != ILobbyLeftListener.LobbyLeaveReason.LOBBY_LEAVE_REASON_USER_LEFT)
      Program.WriteLog(Program.LogType.Disconnect, $"Forcibly left Galaxy lobby at {DateTime.Now.ToLongTimeString()} - {leaveReason.ToString()}", true);
    if (Game1.chatBox != null)
    {
      string sub1;
      switch (leaveReason)
      {
        case ILobbyLeftListener.LobbyLeaveReason.LOBBY_LEAVE_REASON_USER_LEFT:
          sub1 = Game1.content.LoadString("Strings\\UI:Chat_LobbyLost_UserLeft");
          break;
        case ILobbyLeftListener.LobbyLeaveReason.LOBBY_LEAVE_REASON_LOBBY_CLOSED:
          sub1 = Game1.content.LoadString("Strings\\UI:Chat_LobbyLost_LobbyClosed");
          break;
        case ILobbyLeftListener.LobbyLeaveReason.LOBBY_LEAVE_REASON_CONNECTION_LOST:
          sub1 = Game1.content.LoadString("Strings\\UI:Chat_LobbyLost_ConnectionLost");
          break;
        default:
          sub1 = "";
          break;
      }
      Game1.chatBox.addInfoMessage(Game1.content.LoadString("Strings\\UI:Chat_LobbyLost", (object) sub1).Trim());
    }
    Game1.log.Verbose($"Left lobby {lobbyID.ToUint64().ToString()} - leaveReason: {leaveReason.ToString()}");
    this.lobby = (GalaxyID) null;
    this.recreateTimer = this.getTimeNow() + 20000L;
    this.isRecreatedLobby = true;
    this.isFirstRecreateAttempt = true;
  }

  private void onGalaxyLobbyEnter(GalaxyID lobbyID, LobbyEnterResult result)
  {
    this.connectingLobbyID = (GalaxyID) null;
    if (result != LobbyEnterResult.LOBBY_ENTER_RESULT_SUCCESS)
      return;
    Game1.log.Verbose("Lobby entered: " + lobbyID.ToUint64().ToString());
    this.lobby = lobbyID;
    this.lobbyOwner = GalaxyInstance.Matchmaking().GetLobbyOwner(lobbyID);
    if (Game1.chatBox != null)
    {
      string sub1 = "";
      if (Program.sdk.Networking != null && Program.sdk.Networking.SupportsInviteCodes())
        sub1 = Game1.content.LoadString("Strings\\UI:Chat_LobbyJoined_InviteCode", (object) this.GetInviteCode());
      if (this.isRecreatedLobby)
        Game1.chatBox.addInfoMessage(Game1.content.LoadString("Strings\\UI:Chat_LobbyRecreated", (object) sub1).Trim());
      else
        Game1.chatBox.addInfoMessage(Game1.content.LoadString("Strings\\UI:Chat_LobbyJoined", (object) sub1).Trim());
    }
    if (!(this.lobbyOwner == this.selfId))
      return;
    foreach (KeyValuePair<string, string> keyValuePair in this.lobbyData)
      GalaxyInstance.Matchmaking().SetLobbyData(this.lobby, keyValuePair.Key, keyValuePair.Value);
    this.updateLobbyPrivacy();
  }

  public IEnumerable<GalaxyID> LobbyMembers()
  {
    if (!(this.lobby == (GalaxyID) null))
    {
      uint lobby_members_count;
      try
      {
        lobby_members_count = GalaxyInstance.Matchmaking().GetNumLobbyMembers(this.lobby);
      }
      catch
      {
        yield break;
      }
      for (uint i = 0; i < lobby_members_count; ++i)
      {
        GalaxyID lobbyMemberByIndex = GalaxyInstance.Matchmaking().GetLobbyMemberByIndex(this.lobby, i);
        if (!(lobbyMemberByIndex == this.selfId) && !this.ghosts.Contains(lobbyMemberByIndex.ToUint64()))
          yield return lobbyMemberByIndex;
      }
    }
  }

  private void close(GalaxyID peer)
  {
    this.connections.Remove(peer.ToUint64());
    this.incompletePackets.Remove(peer.ToUint64());
  }

  public void Kick(GalaxyID user) => this.ghosts.Add(user.ToUint64());

  public void Close()
  {
    if (this.connectingLobbyID != (GalaxyID) null)
    {
      GalaxyInstance.Matchmaking().LeaveLobby(this.connectingLobbyID);
      this.connectingLobbyID = (GalaxyID) null;
    }
    if (this.lobby != (GalaxyID) null)
    {
      while (this.ConnectionCount > 0)
        this.close(this.Connections.First<GalaxyID>());
      GalaxyInstance.Matchmaking().LeaveLobby(this.lobby);
      this.lobby = (GalaxyID) null;
    }
    this.updateLobbyPrivacy();
    try
    {
      this.galaxyLobbyEnterCallback.Dispose();
    }
    catch (Exception ex)
    {
    }
    try
    {
      this.galaxyLobbyCreatedCallback.Dispose();
    }
    catch (Exception ex)
    {
    }
    try
    {
      this.galaxyLobbyMemberStateCallback.Dispose();
    }
    catch (Exception ex)
    {
    }
    this.galaxyLobbyLeftCallback?.Dispose();
  }

  /// <summary>Decompress a message if necessary and pass the result to <paramref name="onMessage" />.</summary>
  /// <param name="peer">The Galaxy ID of the peer who sent this message to us.</param>
  /// <param name="stream">A memory stream containing the message data.</param>
  /// <param name="onMessage">A callback to handle the message the processed data.</param>
  private void PreprocessMessage(
    GalaxyID peer,
    MemoryStream stream,
    Action<GalaxyID, Stream> onMessage)
  {
    byte[] decompressed;
    if (Program.netCompression.TryDecompressStream((Stream) stream, out decompressed))
      stream = new MemoryStream(decompressed);
    onMessage(peer, (Stream) stream);
  }

  public void Receive(
    Action<GalaxyID> onConnection,
    Action<GalaxyID, Stream> onMessage,
    Action<GalaxyID> onDisconnect,
    Action<string> onError)
  {
    long timeNow = this.getTimeNow();
    if (this.lobby == (GalaxyID) null)
    {
      if (this.lobbyOwner == this.selfId && this.recreateTimer > 0L && this.recreateTimer <= timeNow)
      {
        this.recreateTimer = 0L;
        this.tryCreateLobby();
      }
      this.DisconnectPeers(onDisconnect);
    }
    else
    {
      if (!this.checkedProcotolVersion)
      {
        try
        {
          string lobbyData = GalaxyInstance.Matchmaking().GetLobbyData(this.lobby, "protocolVersion");
          if (lobbyData != "")
          {
            this.checkedProcotolVersion = true;
            if (lobbyData != this.protocolVersion)
            {
              onError(Game1.content.LoadString("Strings\\UI:CoopMenu_FailedProtocolVersion"));
              this.Close();
              return;
            }
          }
        }
        catch (Exception ex)
        {
        }
      }
      IEnumerable<GalaxyID> source = this.LobbyMembers();
      foreach (GalaxyID galaxyId in source)
      {
        if (!this.connections.ContainsKey(galaxyId.ToUint64()) && !this.ghosts.Contains(galaxyId.ToUint64()))
        {
          this.connections.Add(galaxyId.ToUint64(), galaxyId);
          onConnection(galaxyId);
        }
      }
      this.ghosts.IntersectWith(source.Select<GalaxyID, ulong>((Func<GalaxyID, ulong>) (peer => peer.ToUint64())));
      byte[] numArray = new byte[1300];
      uint outMsgSize = 1300;
      GalaxyID outGalaxyID = new GalaxyID();
      while (GalaxyInstance.Networking().ReadP2PPacket(numArray, (uint) numArray.Length, ref outMsgSize, ref outGalaxyID))
      {
        if (this.connections.ContainsKey(outGalaxyID.ToUint64()) && numArray[0] != byte.MaxValue)
        {
          bool flag = numArray[0] == (byte) 1;
          MemoryStream stream1 = new MemoryStream();
          stream1.Write(numArray, 4, (int) outMsgSize - 4);
          MemoryStream destination;
          if (this.incompletePackets.TryGetValue(outGalaxyID.ToUint64(), out destination))
          {
            stream1.Position = 0L;
            stream1.CopyTo((Stream) destination);
            if (!flag)
            {
              MemoryStream stream2 = destination;
              this.incompletePackets.Remove(outGalaxyID.ToUint64());
              stream2.Position = 0L;
              this.PreprocessMessage(outGalaxyID, stream2, onMessage);
            }
          }
          else if (flag)
          {
            stream1.Position = stream1.Length;
            this.incompletePackets[outGalaxyID.ToUint64()] = stream1;
          }
          else
          {
            stream1.Position = 0L;
            this.PreprocessMessage(outGalaxyID, stream1, onMessage);
          }
        }
      }
      this.DisconnectPeers(onDisconnect);
    }
  }

  public virtual void DisconnectPeers(Action<GalaxyID> onDisconnect)
  {
    List<GalaxyID> galaxyIdList = new List<GalaxyID>();
    HashSet<GalaxyID> galaxyIdSet = new HashSet<GalaxyID>();
    foreach (GalaxyID lobbyMember in this.LobbyMembers())
      galaxyIdSet.Add(lobbyMember);
    foreach (GalaxyID galaxyId in this.connections.Values)
    {
      if (this.lobby == (GalaxyID) null || !galaxyIdSet.Contains(galaxyId))
        galaxyIdList.Add(galaxyId);
    }
    foreach (GalaxyID peer in galaxyIdList)
    {
      onDisconnect(peer);
      this.close(peer);
    }
  }

  public void Heartbeat(IEnumerable<GalaxyID> peers)
  {
    long timeNow = this.getTimeNow();
    if (this.heartbeatTimer > timeNow)
      return;
    this.heartbeatTimer = timeNow + 8L;
    byte[] data = new byte[1]{ byte.MaxValue };
    foreach (GalaxyID peer in peers)
      GalaxyInstance.Networking().SendP2PPacket(peer, data, (uint) data.Length, P2PSendType.P2P_SEND_RELIABLE_IMMEDIATE);
  }

  public void Send(GalaxyID peer, byte[] data)
  {
    if (!this.connections.ContainsKey(peer.ToUint64()))
      return;
    data = Program.netCompression.CompressAbove(data);
    if (data.Length <= 1100)
    {
      byte[] data1 = new byte[data.Length + 4];
      data.CopyTo((Array) data1, 4);
      GalaxyInstance.Networking().SendP2PPacket(peer, data1, (uint) data1.Length, P2PSendType.P2P_SEND_RELIABLE);
    }
    else
    {
      int num = 1096;
      int srcOffset = 0;
      byte[] numArray = new byte[1100];
      numArray[0] = (byte) 1;
      while (srcOffset < data.Length)
      {
        int count = num;
        if (srcOffset + num >= data.Length)
        {
          numArray[0] = (byte) 0;
          count = data.Length - srcOffset;
        }
        Buffer.BlockCopy((Array) data, srcOffset, (Array) numArray, 4, count);
        srcOffset += count;
        GalaxyInstance.Networking().SendP2PPacket(peer, numArray, (uint) (count + 4), P2PSendType.P2P_SEND_RELIABLE);
      }
    }
  }

  public void Send(GalaxyID peer, OutgoingMessage message)
  {
    using (MemoryStream output = new MemoryStream())
    {
      using (BinaryWriter writer = new BinaryWriter((Stream) output))
      {
        message.Write(writer);
        output.Seek(0L, SeekOrigin.Begin);
        this.Send(peer, output.ToArray());
      }
    }
  }
}
