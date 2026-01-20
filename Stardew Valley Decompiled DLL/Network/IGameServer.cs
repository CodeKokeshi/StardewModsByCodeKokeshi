// Decompiled with JetBrains decompiler
// Type: StardewValley.Network.IGameServer
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using System;

#nullable disable
namespace StardewValley.Network;

public interface IGameServer : IBandwidthMonitor
{
  int connectionsCount { get; }

  string getInviteCode();

  string getUserName(long farmerId);

  void setPrivacy(ServerPrivacy privacy);

  void stopServer();

  void receiveMessages();

  void sendMessage(long peerId, OutgoingMessage message);

  bool canAcceptIPConnections();

  bool canOfferInvite();

  void offerInvite();

  bool connected();

  void sendMessage(long peerId, byte messageType, Farmer sourceFarmer, params object[] data);

  void sendMessages();

  void startServer();

  void initializeHost();

  void sendServerIntroduction(long peer);

  void kick(long disconnectee);

  string ban(long farmerId);

  void playerDisconnected(long disconnectee);

  bool isGameAvailable();

  bool whenGameAvailable(Action action, Func<bool> customAvailabilityCheck = null);

  void checkFarmhandRequest(
    string userId,
    string connectionId,
    NetFarmerRoot farmer,
    Action<OutgoingMessage> sendMessage,
    Action approve);

  void sendAvailableFarmhands(
    string userId,
    string connectionId,
    Action<OutgoingMessage> sendMessage);

  void processIncomingMessage(IncomingMessage message);

  void updateLobbyData();

  float getPingToClient(long peer);

  bool isUserBanned(string userID);

  void onConnect(string connectionID);

  void onDisconnect(string connectionID);

  bool IsLocalMultiplayerInitiatedServer();
}
