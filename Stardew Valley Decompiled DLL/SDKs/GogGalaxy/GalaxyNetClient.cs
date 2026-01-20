// Decompiled with JetBrains decompiler
// Type: StardewValley.SDKs.GogGalaxy.GalaxyNetClient
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Galaxy.Api;
using StardewValley.Network;
using StardewValley.SDKs.GogGalaxy.Listeners;
using System;
using System.IO;
using System.Linq;

#nullable disable
namespace StardewValley.SDKs.GogGalaxy;

public class GalaxyNetClient : HookableClient
{
  public GalaxyID lobbyId;
  protected GalaxySocket client;
  private GalaxyID serverId;
  /// <summary>The custom display name for the host player, or null if no custom name was found.</summary>
  private string hostDisplayName;
  private GalaxySpecificUserDataListener galaxySpecificUserDataListener;
  private float lastPingMs;

  public GalaxyNetClient(GalaxyID lobbyId)
  {
    this.lobbyId = lobbyId;
    this.hostDisplayName = (string) null;
  }

  ~GalaxyNetClient()
  {
    this.galaxySpecificUserDataListener?.Dispose();
    this.galaxySpecificUserDataListener = (GalaxySpecificUserDataListener) null;
  }

  private void onProfileDataReady(GalaxyID userID)
  {
    if (userID != this.serverId)
      return;
    this.hostDisplayName = (string) null;
    try
    {
      this.hostDisplayName = GalaxyInstance.User().GetUserData("StardewDisplayName", userID);
    }
    catch (Exception ex)
    {
    }
    this.galaxySpecificUserDataListener?.Dispose();
    this.galaxySpecificUserDataListener = (GalaxySpecificUserDataListener) null;
  }

  public override string getUserID()
  {
    return Convert.ToString(GalaxyInstance.User().GetGalaxyID().ToUint64());
  }

  protected override string getHostUserName()
  {
    return !string.IsNullOrEmpty(this.hostDisplayName) ? this.hostDisplayName : GalaxyInstance.Friends().GetFriendPersonaName(this.serverId);
  }

  public override float GetPingToHost() => this.lastPingMs;

  protected override void connectImpl()
  {
    this.client = new GalaxySocket(Multiplayer.protocolVersion);
    GalaxyInstance.User().GetGalaxyID();
    this.client.JoinLobby(this.lobbyId, new Action<string>(this.onReceiveError));
  }

  public override void disconnect(bool neatly = true)
  {
    if (this.client == null)
      return;
    Game1.log.Verbose("Disconnecting from server " + this.lobbyId?.ToString());
    this.client.Close();
    this.client = (GalaxySocket) null;
    this.connectionMessage = (string) null;
  }

  protected override void receiveMessagesImpl()
  {
    if (this.client == null || !this.client.Connected)
      return;
    if (this.client.Connected && this.serverId == (GalaxyID) null)
    {
      Game1.log.Verbose("Connected to server " + this.lobbyId?.ToString());
      this.serverId = this.client.LobbyOwner;
      if (GalaxyInstance.User().IsUserDataAvailable(this.serverId))
      {
        this.onProfileDataReady(this.serverId);
      }
      else
      {
        this.hostDisplayName = GalaxyNetHelper.TryGetHostSteamDisplayName(this.lobbyId);
        this.galaxySpecificUserDataListener = new GalaxySpecificUserDataListener(new Action<GalaxyID>(this.onProfileDataReady));
        GalaxyInstance.User().RequestUserData(this.serverId);
      }
    }
    this.client.Receive(new Action<GalaxyID>(this.onReceiveConnection), new Action<GalaxyID, Stream>(this.onReceiveMessage), new Action<GalaxyID>(this.onReceiveDisconnect), new Action<string>(this.onReceiveError));
    if (this.client == null)
      return;
    this.client.Heartbeat(Enumerable.Repeat<GalaxyID>(this.serverId, 1));
    this.lastPingMs = (float) this.client.GetPingWith(this.serverId);
    if ((double) this.lastPingMs <= 30000.0)
      return;
    this.timedOut = true;
    this.pendingDisconnect = Multiplayer.DisconnectType.GalaxyTimeout;
    this.disconnect(true);
  }

  protected virtual void onReceiveConnection(GalaxyID peer)
  {
  }

  protected virtual void onReceiveMessage(GalaxyID peer, Stream messageStream)
  {
    if (peer != this.serverId)
      return;
    this.bandwidthLogger?.RecordBytesDown(messageStream.Length);
    using (IncomingMessage message = new IncomingMessage())
    {
      using (BinaryReader reader = new BinaryReader(messageStream))
      {
        message.Read(reader);
        this.OnProcessingMessage(message, new Action<OutgoingMessage>(this.sendMessageImpl), (Action) (() => this.processIncomingMessage(message)));
      }
    }
  }

  protected virtual void onReceiveDisconnect(GalaxyID peer)
  {
    if (peer != this.serverId)
    {
      Game1.multiplayer.playerDisconnected((long) peer.ToUint64());
    }
    else
    {
      this.timedOut = true;
      this.pendingDisconnect = Multiplayer.DisconnectType.HostLeft;
    }
  }

  protected virtual void onReceiveError(string message) => this.connectionMessage = message;

  protected virtual void sendMessageImpl(OutgoingMessage message)
  {
    if (this.client == null || !this.client.Connected || this.serverId == (GalaxyID) null)
      return;
    if (this.bandwidthLogger != null)
    {
      using (MemoryStream output = new MemoryStream())
      {
        using (BinaryWriter writer = new BinaryWriter((Stream) output))
        {
          message.Write(writer);
          output.Seek(0L, SeekOrigin.Begin);
          byte[] array = output.ToArray();
          this.client.Send(this.serverId, array);
          this.bandwidthLogger.RecordBytesUp((long) array.Length);
        }
      }
    }
    else
      this.client.Send(this.serverId, message);
  }

  public override void sendMessage(OutgoingMessage message)
  {
    this.OnSendingMessage(message, new Action<OutgoingMessage>(this.sendMessageImpl), (Action) (() => this.sendMessageImpl(message)));
  }
}
