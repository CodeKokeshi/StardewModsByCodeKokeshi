// Decompiled with JetBrains decompiler
// Type: StardewValley.Network.LidgrenClient
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Lidgren.Network;
using System;

#nullable disable
namespace StardewValley.Network;

public class LidgrenClient : HookableClient
{
  public string address;
  public NetClient client;
  private bool serverDiscovered;
  private int maxRetryAttempts;
  private int retryMs = 10000;
  private double lastAttemptMs;
  private int retryAttempts;
  private float lastLatencyMs;

  public LidgrenClient(string address) => this.address = address;

  public override string getUserID() => "";

  public override float GetPingToHost() => this.lastLatencyMs / 2f;

  protected override string getHostUserName()
  {
    return this.client?.ServerConnection?.RemoteEndPoint?.Address?.ToString() ?? "";
  }

  protected override void connectImpl()
  {
    NetPeerConfiguration config = new NetPeerConfiguration("StardewValley");
    config.EnableMessageType(NetIncomingMessageType.DiscoveryResponse);
    config.EnableMessageType(NetIncomingMessageType.ConnectionLatencyUpdated);
    config.ConnectionTimeout = 30f;
    config.PingInterval = 5f;
    config.MaximumTransmissionUnit = 1200;
    this.client = new NetClient(config);
    this.client.Start();
    this.attemptConnection();
  }

  private void attemptConnection()
  {
    int serverPort = 24642;
    if (this.address.Contains(':'))
    {
      string[] strArray = this.address.Split(':');
      this.address = strArray[0];
      try
      {
        serverPort = Convert.ToInt32(strArray[1]);
      }
      catch (Exception ex)
      {
        serverPort = 24642;
      }
    }
    this.client.DiscoverKnownPeer(this.address, serverPort);
    this.lastAttemptMs = DateTime.UtcNow.TimeOfDay.TotalMilliseconds;
  }

  public override void disconnect(bool neatly = true)
  {
    if (this.client == null)
      return;
    if (this.client.ConnectionStatus != NetConnectionStatus.Disconnected && this.client.ConnectionStatus != NetConnectionStatus.Disconnecting)
    {
      if (neatly)
        this.sendMessage(new OutgoingMessage((byte) 19, Game1.player, Array.Empty<object>()));
      this.client.FlushSendQueue();
      this.client.Disconnect("");
      this.client.FlushSendQueue();
    }
    this.connectionMessage = (string) null;
  }

  protected virtual bool validateProtocol(string version) => version == Multiplayer.protocolVersion;

  protected override void receiveMessagesImpl()
  {
    if (this.client != null && !this.serverDiscovered && DateTime.UtcNow.TimeOfDay.TotalMilliseconds >= this.lastAttemptMs + (double) this.retryMs && this.retryAttempts < this.maxRetryAttempts)
    {
      this.attemptConnection();
      ++this.retryAttempts;
    }
    NetIncomingMessage netIncomingMessage;
    while ((netIncomingMessage = this.client.ReadMessage()) != null)
    {
      switch (netIncomingMessage.MessageType)
      {
        case NetIncomingMessageType.StatusChanged:
          this.statusChanged(netIncomingMessage);
          continue;
        case NetIncomingMessageType.Data:
          this.parseDataMessageFromServer(netIncomingMessage);
          continue;
        case NetIncomingMessageType.DiscoveryResponse:
          if (!this.serverDiscovered)
          {
            Game1.log.Verbose("Found server at " + netIncomingMessage.SenderEndPoint?.ToString());
            string version = netIncomingMessage.ReadString();
            if (this.validateProtocol(version))
            {
              this.serverName = netIncomingMessage.ReadString();
              this.receiveHandshake(netIncomingMessage);
              this.serverDiscovered = true;
              continue;
            }
            Game1.log.Warn($"Failed to connect. The server's protocol ({version}) does not match our own ({Multiplayer.protocolVersion}).");
            this.connectionMessage = Game1.content.LoadString("Strings\\UI:CoopMenu_FailedProtocolVersion");
            this.client.Disconnect("");
            continue;
          }
          continue;
        case NetIncomingMessageType.DebugMessage:
        case NetIncomingMessageType.WarningMessage:
        case NetIncomingMessageType.ErrorMessage:
          string str = netIncomingMessage.ReadString();
          Game1.log.Verbose($"{netIncomingMessage.MessageType.ToString()}: {str}");
          Game1.debugOutput = str;
          continue;
        case NetIncomingMessageType.ConnectionLatencyUpdated:
          this.readLatency(netIncomingMessage);
          continue;
        default:
          continue;
      }
    }
  }

  private void readLatency(NetIncomingMessage msg) => this.lastLatencyMs = msg.ReadFloat() * 1000f;

  private void receiveHandshake(NetIncomingMessage msg)
  {
    this.client.Connect(msg.SenderEndPoint.Address.ToString(), msg.SenderEndPoint.Port);
  }

  private void statusChanged(NetIncomingMessage message)
  {
    NetConnectionStatus status = (NetConnectionStatus) message.ReadByte();
    switch (status)
    {
      case NetConnectionStatus.Disconnecting:
      case NetConnectionStatus.Disconnected:
        string message1 = message.ReadString();
        this.clientRemotelyDisconnected(status, message1);
        break;
    }
  }

  private void clientRemotelyDisconnected(NetConnectionStatus status, string message)
  {
    this.timedOut = true;
    if (status == NetConnectionStatus.Disconnected)
    {
      if (message == Multiplayer.kicked)
        this.pendingDisconnect = Multiplayer.DisconnectType.Kicked;
      else
        this.pendingDisconnect = Multiplayer.DisconnectType.LidgrenTimeout;
    }
    else
      this.pendingDisconnect = Multiplayer.DisconnectType.LidgrenDisconnect_Unknown;
  }

  protected virtual void sendMessageImpl(OutgoingMessage message)
  {
    NetOutgoingMessage message1 = this.client.CreateMessage();
    LidgrenMessageUtils.WriteMessage(message, message1);
    int num = (int) this.client.SendMessage(message1, NetDeliveryMethod.ReliableOrdered);
    this.bandwidthLogger?.RecordBytesUp((long) message1.LengthBytes);
  }

  public override void sendMessage(OutgoingMessage message)
  {
    this.OnSendingMessage(message, new Action<OutgoingMessage>(this.sendMessageImpl), (Action) (() => this.sendMessageImpl(message)));
  }

  private void parseDataMessageFromServer(NetIncomingMessage dataMsg)
  {
    this.bandwidthLogger?.RecordBytesDown((long) dataMsg.LengthBytes);
    using (IncomingMessage message = new IncomingMessage())
    {
      using (NetBufferReadStream stream = new NetBufferReadStream((NetBuffer) dataMsg))
      {
        while ((long) dataMsg.LengthBits - dataMsg.Position >= 8L)
        {
          LidgrenMessageUtils.ReadStreamToMessage(stream, message);
          this.OnProcessingMessage(message, new Action<OutgoingMessage>(this.sendMessageImpl), (Action) (() => this.processIncomingMessage(message)));
        }
      }
    }
  }
}
