// Decompiled with JetBrains decompiler
// Type: StardewValley.Network.LidgrenServer
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

#nullable disable
namespace StardewValley.Network;

public class LidgrenServer(IGameServer gameServer) : HookableServer(gameServer)
{
  public const int defaultPort = 24642;
  public NetServer server;
  private HashSet<NetConnection> introductionsSent = new HashSet<NetConnection>();
  protected Bimap<long, NetConnection> peers = new Bimap<long, NetConnection>();

  public override int connectionsCount => this.server == null ? 0 : this.server.ConnectionsCount;

  public override bool isConnectionActive(string connectionID)
  {
    foreach (NetConnection connection in this.server.Connections)
    {
      if (this.getConnectionId(connection) == connectionID && connection.Status == NetConnectionStatus.Connected)
        return true;
    }
    return false;
  }

  public override string getUserId(long farmerId)
  {
    return !this.peers.ContainsLeft(farmerId) ? (string) null : this.peers[farmerId].RemoteEndPoint.Address.ToString();
  }

  public override bool hasUserId(string userId)
  {
    foreach (NetConnection rightValue in (IEnumerable<NetConnection>) this.peers.RightValues)
    {
      if (rightValue.RemoteEndPoint.Address.ToString().Equals(userId))
        return true;
    }
    return false;
  }

  public override string getUserName(long farmerId)
  {
    return !this.peers.ContainsLeft(farmerId) ? (string) null : this.peers[farmerId].RemoteEndPoint.Address.ToString();
  }

  public override float getPingToClient(long farmerId)
  {
    return !this.peers.ContainsLeft(farmerId) ? -1f : (float) ((double) this.peers[farmerId].AverageRoundtripTime / 2.0 * 1000.0);
  }

  public override void setPrivacy(ServerPrivacy privacy)
  {
  }

  public override bool canAcceptIPConnections() => true;

  public override bool connected() => this.server != null;

  public override void initialize()
  {
    Game1.log.Verbose("Starting LAN server");
    NetPeerConfiguration config = new NetPeerConfiguration("StardewValley");
    config.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);
    config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
    config.Port = 24642;
    config.ConnectionTimeout = 30f;
    config.PingInterval = 5f;
    config.MaximumConnections = Game1.multiplayer.playerLimit * 2;
    config.MaximumTransmissionUnit = 1200;
    this.server = new NetServer(config);
    this.server.Start();
  }

  public override void stopServer()
  {
    Game1.log.Verbose("Stopping LAN server");
    this.server.Shutdown("Server shutting down...");
    this.server.FlushSendQueue();
    this.introductionsSent.Clear();
    this.peers.Clear();
  }

  public static bool IsLocal(string host_name_or_address)
  {
    if (string.IsNullOrEmpty(host_name_or_address))
      return false;
    try
    {
      IPAddress[] hostAddresses = Dns.GetHostAddresses(host_name_or_address);
      IPAddress[] local_ips = Dns.GetHostAddresses(Dns.GetHostName());
      Func<IPAddress, bool> predicate = (Func<IPAddress, bool>) (host_ip => IPAddress.IsLoopback(host_ip) || ((IEnumerable<IPAddress>) local_ips).Contains<IPAddress>(host_ip));
      return ((IEnumerable<IPAddress>) hostAddresses).Any<IPAddress>(predicate);
    }
    catch
    {
      return false;
    }
  }

  public override void receiveMessages()
  {
    NetIncomingMessage netIncomingMessage;
    while ((netIncomingMessage = this.server.ReadMessage()) != null)
    {
      this.bandwidthLogger?.RecordBytesDown((long) netIncomingMessage.LengthBytes);
      switch (netIncomingMessage.MessageType)
      {
        case NetIncomingMessageType.StatusChanged:
          this.statusChanged(netIncomingMessage);
          break;
        case NetIncomingMessageType.ConnectionApproval:
          if (Game1.options.ipConnectionsEnabled || this.gameServer.IsLocalMultiplayerInitiatedServer())
          {
            netIncomingMessage.SenderConnection.Approve();
            break;
          }
          netIncomingMessage.SenderConnection.Deny();
          break;
        case NetIncomingMessageType.Data:
          this.parseDataMessageFromClient(netIncomingMessage);
          break;
        case NetIncomingMessageType.DiscoveryRequest:
          if ((Game1.options.ipConnectionsEnabled || this.gameServer.IsLocalMultiplayerInitiatedServer()) && (!this.gameServer.IsLocalMultiplayerInitiatedServer() || LidgrenServer.IsLocal(netIncomingMessage.SenderEndPoint.Address.ToString())) && !this.gameServer.isUserBanned(netIncomingMessage.SenderEndPoint.Address.ToString()))
          {
            this.sendVersionInfo(netIncomingMessage);
            break;
          }
          break;
        case NetIncomingMessageType.DebugMessage:
        case NetIncomingMessageType.WarningMessage:
        case NetIncomingMessageType.ErrorMessage:
          string str = netIncomingMessage.ReadString();
          Game1.log.Verbose($"{netIncomingMessage.MessageType.ToString()}: {str}");
          Game1.debugOutput = str;
          break;
        default:
          Game1.debugOutput = netIncomingMessage.ToString();
          break;
      }
      this.server.Recycle(netIncomingMessage);
    }
    foreach (NetConnection connection in this.server.Connections)
    {
      NetConnection conn = connection;
      if (conn.Status == NetConnectionStatus.Connected && !this.introductionsSent.Contains(conn))
      {
        if (!this.gameServer.whenGameAvailable((Action) (() => this.gameServer.sendAvailableFarmhands("", this.getConnectionId(conn), (Action<OutgoingMessage>) (msg => this.sendMessage(conn, msg)))), (Func<bool>) (() => Game1.gameMode != (byte) 6)))
        {
          Game1.log.Verbose("Postponing introduction message");
          this.sendMessage(conn, new OutgoingMessage((byte) 11, Game1.player, new object[1]
          {
            (object) "Strings\\UI:Client_WaitForHostLoad"
          }));
        }
        this.introductionsSent.Add(conn);
      }
    }
    this.bandwidthLogger?.Update();
  }

  private void sendVersionInfo(NetIncomingMessage message)
  {
    NetOutgoingMessage message1 = this.server.CreateMessage();
    message1.Write(Multiplayer.protocolVersion);
    message1.Write("StardewValley");
    this.server.SendDiscoveryResponse(message1, message.SenderEndPoint);
    this.bandwidthLogger?.RecordBytesUp((long) message1.LengthBytes);
  }

  private void statusChanged(NetIncomingMessage message)
  {
    switch ((NetConnectionStatus) message.ReadByte())
    {
      case NetConnectionStatus.Connected:
        this.onConnect(this.getConnectionId(message.SenderConnection));
        break;
      case NetConnectionStatus.Disconnecting:
      case NetConnectionStatus.Disconnected:
        this.onDisconnect(this.getConnectionId(message.SenderConnection));
        if (!this.peers.ContainsRight(message.SenderConnection))
          break;
        this.playerDisconnected(this.peers[message.SenderConnection]);
        break;
    }
  }

  public override void kick(long disconnectee)
  {
    base.kick(disconnectee);
    if (!this.peers.ContainsLeft(disconnectee))
      return;
    this.peers[disconnectee].Disconnect(Multiplayer.kicked);
    this.server.FlushSendQueue();
    this.playerDisconnected(disconnectee);
  }

  public override void playerDisconnected(long disconnectee)
  {
    base.playerDisconnected(disconnectee);
    this.introductionsSent.Remove(this.peers[disconnectee]);
    this.peers.RemoveLeft(disconnectee);
  }

  protected virtual void parseDataMessageFromClient(NetIncomingMessage dataMsg)
  {
    NetConnection peer = dataMsg.SenderConnection;
    using (IncomingMessage message = new IncomingMessage())
    {
      using (NetBufferReadStream stream = new NetBufferReadStream((NetBuffer) dataMsg))
      {
        while ((long) dataMsg.LengthBits - dataMsg.Position >= 8L)
        {
          LidgrenMessageUtils.ReadStreamToMessage(stream, message);
          this.OnProcessingMessage(message, (Action<OutgoingMessage>) (outgoing => this.sendMessage(peer, outgoing)), (Action) (() =>
          {
            if (this.peers.ContainsLeft(message.FarmerID) && this.peers[message.FarmerID] == peer)
            {
              this.gameServer.processIncomingMessage(message);
            }
            else
            {
              if (message.MessageType != (byte) 2)
                return;
              NetFarmerRoot farmer = Game1.multiplayer.readFarmer(message.Reader);
              this.gameServer.checkFarmhandRequest("", this.getConnectionId(dataMsg.SenderConnection), farmer, (Action<OutgoingMessage>) (msg => this.sendMessage(peer, msg)), (Action) (() => this.peers[farmer.Value.UniqueMultiplayerID] = peer));
            }
          }));
        }
      }
    }
  }

  public string getConnectionId(NetConnection connection)
  {
    return "L_" + connection.RemoteUniqueIdentifier.ToString();
  }

  public override void sendMessage(long peerId, OutgoingMessage message)
  {
    if (!this.peers.ContainsLeft(peerId))
      return;
    this.sendMessage(this.peers[peerId], message);
  }

  protected virtual void sendMessage(NetConnection connection, OutgoingMessage message)
  {
    NetOutgoingMessage message1 = this.server.CreateMessage();
    LidgrenMessageUtils.WriteMessage(message, message1);
    int num = (int) this.server.SendMessage(message1, connection, NetDeliveryMethod.ReliableOrdered);
    this.bandwidthLogger?.RecordBytesUp((long) message1.LengthBytes);
  }

  public override void setLobbyData(string key, string value)
  {
  }
}
