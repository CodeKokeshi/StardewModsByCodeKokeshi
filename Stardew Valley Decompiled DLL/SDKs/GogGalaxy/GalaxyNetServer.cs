// Decompiled with JetBrains decompiler
// Type: StardewValley.SDKs.GogGalaxy.GalaxyNetServer
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Galaxy.Api;
using StardewValley.Network;
using StardewValley.SDKs.GogGalaxy.Listeners;
using System;
using System.Collections.Generic;
using System.IO;

#nullable disable
namespace StardewValley.SDKs.GogGalaxy;

public class GalaxyNetServer(IGameServer gameServer) : HookableServer(gameServer)
{
  private GalaxyID host;
  protected GalaxySocket server;
  private GalaxySpecificUserDataListener galaxySpecificUserDataListener;
  protected Bimap<long, ulong> peers = new Bimap<long, ulong>();
  /// <summary>A mapping of raw GalaxyIDs to custom display names.</summary>
  protected Dictionary<ulong, string> displayNames = new Dictionary<ulong, string>();

  public override int connectionsCount => this.server == null ? 0 : this.server.ConnectionCount;

  public override string getUserId(long farmerId)
  {
    return !this.peers.ContainsLeft(farmerId) ? (string) null : this.peers[farmerId].ToString();
  }

  public override bool hasUserId(string userId)
  {
    foreach (ulong rightValue in (IEnumerable<ulong>) this.peers.RightValues)
    {
      if (rightValue.ToString().Equals(userId))
        return true;
    }
    return false;
  }

  public override bool isConnectionActive(string connection_id)
  {
    foreach (GalaxyID connection in this.server.Connections)
    {
      if (this.getConnectionId(connection) == connection_id && connection.IsValid())
        return true;
    }
    return false;
  }

  public override string getUserName(long farmerId)
  {
    if (!this.peers.ContainsLeft(farmerId))
      return (string) null;
    ulong peer = this.peers[farmerId];
    string userName;
    if (this.displayNames.TryGetValue(peer, out userName))
      return userName;
    GalaxyID userID = new GalaxyID(peer);
    return GalaxyInstance.Friends().GetFriendPersonaName(userID);
  }

  public override float getPingToClient(long farmerId)
  {
    return !this.peers.ContainsLeft(farmerId) ? -1f : (float) this.server.GetPingWith(new GalaxyID(this.peers[farmerId]));
  }

  public override void setPrivacy(ServerPrivacy privacy) => this.server.SetPrivacy(privacy);

  public override bool connected() => this.server.Connected;

  public override string getInviteCode() => this.server.GetInviteCode();

  public override void initialize()
  {
    Game1.log.Verbose("Starting Galaxy server");
    this.host = GalaxyInstance.User().GetGalaxyID();
    this.galaxySpecificUserDataListener = new GalaxySpecificUserDataListener(new Action<GalaxyID>(this.onProfileDataReady));
    this.server = new GalaxySocket(Multiplayer.protocolVersion);
    this.server.CreateLobby(Game1.options.serverPrivacy, (uint) (Game1.multiplayer.playerLimit * 2));
  }

  public override void stopServer()
  {
    Game1.log.Verbose("Stopping Galaxy server");
    this.server.Close();
    this.galaxySpecificUserDataListener?.Dispose();
    this.galaxySpecificUserDataListener = (GalaxySpecificUserDataListener) null;
  }

  private void onProfileDataReady(GalaxyID userID)
  {
    if (userID == this.host || this.displayNames.ContainsKey(userID.ToUint64()))
      return;
    string str = (string) null;
    try
    {
      str = GalaxyInstance.User().GetUserData("StardewDisplayName", userID);
    }
    catch (Exception ex)
    {
    }
    if (!string.IsNullOrEmpty(str))
    {
      this.displayNames[userID.ToUint64()] = str;
      Game1.log.Verbose($"{userID} ({str}) connected");
    }
    else
      Game1.log.Verbose(userID?.ToString() + " connected");
    this.onConnect(this.getConnectionId(userID));
    this.gameServer.sendAvailableFarmhands(this.createUserID(userID), this.getConnectionId(userID), (Action<OutgoingMessage>) (msg => this.sendMessage(userID, msg)));
  }

  public override void receiveMessages()
  {
    if (this.server == null)
      return;
    this.server.Receive(new Action<GalaxyID>(this.onReceiveConnection), new Action<GalaxyID, Stream>(this.onReceiveMessage), new Action<GalaxyID>(this.onReceiveDisconnect), new Action<string>(this.onReceiveError));
    this.server.Heartbeat(this.server.LobbyMembers());
    foreach (GalaxyID connection in this.server.Connections)
    {
      if (this.server.GetPingWith(connection) > 30000L)
        this.server.Kick(connection);
    }
    this.bandwidthLogger?.Update();
  }

  public override void kick(long disconnectee)
  {
    base.kick(disconnectee);
    if (!this.peers.ContainsLeft(disconnectee))
      return;
    GalaxyID galaxyId = new GalaxyID(this.peers[disconnectee]);
    this.server.Kick(galaxyId);
    this.sendMessage(galaxyId, new OutgoingMessage((byte) 23, Game1.player, Array.Empty<object>()));
  }

  public string getConnectionId(GalaxyID peer) => "GN_" + Convert.ToString(peer.ToUint64());

  private string createUserID(GalaxyID peer) => Convert.ToString(peer.ToUint64());

  protected virtual void onReceiveConnection(GalaxyID peer)
  {
    if (this.gameServer.isUserBanned(peer.ToString()))
      return;
    if (GalaxyInstance.User().IsUserDataAvailable(peer))
      this.onProfileDataReady(peer);
    else
      GalaxyInstance.User().RequestUserData(peer);
  }

  protected virtual void onReceiveMessage(GalaxyID peer, Stream messageStream)
  {
    this.bandwidthLogger?.RecordBytesDown(messageStream.Length);
    using (IncomingMessage message = new IncomingMessage())
    {
      using (BinaryReader reader = new BinaryReader(messageStream))
      {
        message.Read(reader);
        this.OnProcessingMessage(message, (Action<OutgoingMessage>) (outgoing => this.sendMessage(peer, outgoing)), (Action) (() =>
        {
          if (this.peers.ContainsLeft(message.FarmerID) && (long) this.peers[message.FarmerID] == (long) peer.ToUint64())
          {
            this.gameServer.processIncomingMessage(message);
          }
          else
          {
            if (message.MessageType != (byte) 2)
              return;
            NetFarmerRoot farmer = Game1.multiplayer.readFarmer(message.Reader);
            GalaxyID capturedPeer = new GalaxyID(peer.ToUint64());
            this.gameServer.checkFarmhandRequest(this.createUserID(peer), this.getConnectionId(peer), farmer, (Action<OutgoingMessage>) (msg => this.sendMessage(capturedPeer, msg)), (Action) (() => this.peers[farmer.Value.UniqueMultiplayerID] = capturedPeer.ToUint64()));
          }
        }));
      }
    }
  }

  public virtual void onReceiveDisconnect(GalaxyID peer)
  {
    Game1.log.Verbose(peer?.ToString() + " disconnected");
    this.onDisconnect(this.getConnectionId(peer));
    if (this.peers.ContainsRight(peer.ToUint64()))
      this.playerDisconnected(this.peers[peer.ToUint64()]);
    this.displayNames.Remove(peer.ToUint64());
  }

  protected virtual void onReceiveError(string messageKey)
  {
    Game1.log.Error("Server error: " + Game1.content.LoadString(messageKey));
  }

  public override void playerDisconnected(long disconnectee)
  {
    base.playerDisconnected(disconnectee);
    this.peers.RemoveLeft(disconnectee);
  }

  public override void sendMessage(long peerId, OutgoingMessage message)
  {
    if (!this.peers.ContainsLeft(peerId))
      return;
    this.sendMessage(new GalaxyID(this.peers[peerId]), message);
  }

  protected virtual void sendMessage(GalaxyID peer, OutgoingMessage message)
  {
    if (this.bandwidthLogger != null)
    {
      using (MemoryStream output = new MemoryStream())
      {
        using (BinaryWriter writer = new BinaryWriter((Stream) output))
        {
          message.Write(writer);
          output.Seek(0L, SeekOrigin.Begin);
          byte[] array = output.ToArray();
          this.server.Send(peer, array);
          this.bandwidthLogger.RecordBytesUp((long) array.Length);
        }
      }
    }
    else
      this.server.Send(peer, message);
  }

  public override void setLobbyData(string key, string value)
  {
    this.server.SetLobbyData(key, value);
  }
}
