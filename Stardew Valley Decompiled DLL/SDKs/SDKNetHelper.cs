// Decompiled with JetBrains decompiler
// Type: StardewValley.SDKs.SDKNetHelper
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using StardewValley.Network;

#nullable disable
namespace StardewValley.SDKs;

public interface SDKNetHelper
{
  string GetUserID();

  Client CreateClient(object lobby);

  Client GetRequestedClient();

  Server CreateServer(IGameServer gameServer);

  void AddLobbyUpdateListener(LobbyUpdateListener listener);

  void RemoveLobbyUpdateListener(LobbyUpdateListener listener);

  void RequestFriendLobbyData();

  string GetLobbyData(object lobby, string key);

  string GetLobbyOwnerName(object lobby);

  bool SupportsInviteCodes();

  object GetLobbyFromInviteCode(string inviteCode);

  void ShowInviteDialog(object lobby);

  void MutePlayer(string userId, bool mute);

  bool IsPlayerMuted(string userId);

  void ShowProfile(string userId);
}
