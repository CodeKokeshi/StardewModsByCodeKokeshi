// Decompiled with JetBrains decompiler
// Type: StardewValley.Network.Server
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using System;

#nullable disable
namespace StardewValley.Network;

public abstract class Server : IBandwidthMonitor
{
  internal IGameServer gameServer;
  protected BandwidthLogger bandwidthLogger;

  public Server(IGameServer gameServer) => this.gameServer = gameServer;

  public abstract int connectionsCount { get; }

  public abstract void initialize();

  public abstract void setPrivacy(ServerPrivacy privacy);

  public abstract void stopServer();

  public abstract void receiveMessages();

  public abstract void sendMessage(long peerId, OutgoingMessage message);

  public abstract bool connected();

  public virtual bool canAcceptIPConnections() => false;

  public virtual bool canOfferInvite() => false;

  public virtual void offerInvite()
  {
  }

  public virtual string getInviteCode() => (string) null;

  public virtual bool PopulatePlatformData(Farmer farmer) => false;

  public virtual string getUserId(long farmerId) => (string) null;

  public virtual bool hasUserId(string userId) => false;

  public virtual float getPingToClient(long farmerId) => 0.0f;

  public virtual bool isConnectionActive(string connectionId)
  {
    throw new NotImplementedException();
  }

  public virtual void onConnect(string connectionId) => this.gameServer.onConnect(connectionId);

  public virtual void onDisconnect(string connectionId)
  {
    this.gameServer.onDisconnect(connectionId);
  }

  public abstract string getUserName(long farmerId);

  public abstract void setLobbyData(string key, string value);

  public virtual void kick(long disconnectee)
  {
  }

  public virtual void playerDisconnected(long disconnectee)
  {
    this.gameServer.playerDisconnected(disconnectee);
  }

  public bool LogBandwidth
  {
    get => this.bandwidthLogger != null;
    set
    {
      if (value)
        this.bandwidthLogger = new BandwidthLogger();
      else
        this.bandwidthLogger = (BandwidthLogger) null;
    }
  }

  public BandwidthLogger BandwidthLogger => this.bandwidthLogger;
}
