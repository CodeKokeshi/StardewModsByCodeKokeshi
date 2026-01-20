// Decompiled with JetBrains decompiler
// Type: StardewValley.SDKs.GogGalaxy.GalaxyNetHelper
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Galaxy.Api;
using StardewValley.Network;
using StardewValley.SDKs.GogGalaxy.Internal;
using StardewValley.SDKs.GogGalaxy.Listeners;
using System;
using System.Collections.Generic;

#nullable disable
namespace StardewValley.SDKs.GogGalaxy;

public class GalaxyNetHelper : SDKNetHelper
{
  public const string GalaxyConnectionStringPrefix = "-connect-lobby-";
  public const string SteamConnectionStringPrefix = "+connect_lobby";
  /// <summary>The invite code prefix for a GOG Galaxy lobby.</summary>
  public const char GalaxyInvitePrefix = 'G';
  /// <summary>The invite code prefix for a Steam lobby.</summary>
  public const char SteamInvitePrefix = 'S';
  protected GalaxyID lobbyRequested;
  private GalaxyLobbyEnteredListener lobbyEntered;
  private GalaxyGameJoinRequestedListener lobbyJoinRequested;
  private GalaxyLobbyDataListener lobbyDataListener;
  private GalaxyRichPresenceListener richPresenceListener;
  private List<LobbyUpdateListener> lobbyUpdateListeners = new List<LobbyUpdateListener>();

  public GalaxyNetHelper()
  {
    this.lobbyRequested = this.getStartupLobby();
    this.lobbyJoinRequested = new GalaxyGameJoinRequestedListener(new Action<GalaxyID, string>(this.onLobbyJoinRequested));
    this.lobbyEntered = new GalaxyLobbyEnteredListener(new Action<GalaxyID, LobbyEnterResult>(this.onLobbyEntered));
    this.lobbyDataListener = new GalaxyLobbyDataListener(new Action<GalaxyID, GalaxyID>(this.onLobbyDataUpdated));
    this.richPresenceListener = new GalaxyRichPresenceListener(new Action<GalaxyID>(this.onRichPresenceUpdated));
    if (!(this.lobbyRequested != (GalaxyID) null))
      return;
    Game1.multiplayer.inviteAccepted();
  }

  /// <summary>Get the host's Steam display name from the underlying GOG Galaxy SDK if it's set.</summary>
  /// <param name="lobbyId">The GOG Galaxy lobby ID.</param>
  /// <returns>Returns the host's display name, or <c>null</c> if it's not set.</returns>
  public static string TryGetHostSteamDisplayName(GalaxyID lobbyId)
  {
    try
    {
      return GalaxyInstance.Matchmaking().GetLobbyData(lobbyId, "HostDisplayName");
    }
    catch (Exception ex)
    {
      return (string) null;
    }
  }

  public virtual string GetUserID()
  {
    return Convert.ToString(GalaxyInstance.User().GetGalaxyID().ToUint64());
  }

  protected virtual Client createClient(GalaxyID lobby)
  {
    return Game1.multiplayer.InitClient((Client) new GalaxyNetClient(lobby));
  }

  public Client CreateClient(object lobby) => this.createClient(new GalaxyID((ulong) lobby));

  public virtual Server CreateServer(IGameServer gameServer)
  {
    return Game1.multiplayer.InitServer((Server) new GalaxyNetServer(gameServer));
  }

  protected GalaxyID parseConnectionString(string connectionString)
  {
    if (connectionString == null)
      return (GalaxyID) null;
    if (connectionString.StartsWith("-connect-lobby-"))
      return new GalaxyID(Convert.ToUInt64(connectionString.Substring("-connect-lobby-".Length)));
    return connectionString.StartsWith("+connect_lobby ") ? new GalaxyID(Convert.ToUInt64(connectionString.Substring("+connect_lobby".Length + 1))) : (GalaxyID) null;
  }

  protected virtual GalaxyID getStartupLobby()
  {
    string[] commandLineArgs = Environment.GetCommandLineArgs();
    for (int index = 0; index < commandLineArgs.Length; ++index)
    {
      if (commandLineArgs[index].StartsWith("-connect-lobby-"))
        return this.parseConnectionString(commandLineArgs[index]);
    }
    return (GalaxyID) null;
  }

  public Client GetRequestedClient()
  {
    return this.lobbyRequested != (GalaxyID) null ? this.createClient(this.lobbyRequested) : (Client) null;
  }

  public void AddLobbyUpdateListener(LobbyUpdateListener listener)
  {
    this.lobbyUpdateListeners.Add(listener);
  }

  public void RemoveLobbyUpdateListener(LobbyUpdateListener listener)
  {
    this.lobbyUpdateListeners.Remove(listener);
  }

  public virtual void RequestFriendLobbyData()
  {
    uint friendCount = GalaxyInstance.Friends().GetFriendCount();
    for (uint index = 0; index < friendCount; ++index)
    {
      GalaxyID friendByIndex = GalaxyInstance.Friends().GetFriendByIndex(index);
      GalaxyInstance.Friends().RequestRichPresence(friendByIndex);
    }
  }

  private void onRichPresenceUpdated(GalaxyID userID)
  {
    GalaxyID connectionString = this.parseConnectionString(GalaxyInstance.Friends().GetRichPresence("connect", userID));
    if (!(connectionString != (GalaxyID) null))
      return;
    GalaxyInstance.Matchmaking().RequestLobbyData(connectionString);
  }

  private void onLobbyDataUpdated(GalaxyID lobbyID, GalaxyID memberID)
  {
    foreach (LobbyUpdateListener lobbyUpdateListener in this.lobbyUpdateListeners)
      lobbyUpdateListener.OnLobbyUpdate((object) lobbyID.ToUint64());
  }

  public virtual string GetLobbyData(object lobby, string key)
  {
    return GalaxyInstance.Matchmaking().GetLobbyData(new GalaxyID((ulong) lobby), key);
  }

  public virtual string GetLobbyOwnerName(object lobbyId)
  {
    GalaxyID lobbyID = new GalaxyID((ulong) lobbyId);
    GalaxyID lobbyOwner = GalaxyInstance.Matchmaking().GetLobbyOwner(lobbyID);
    return GalaxyInstance.Friends().GetFriendPersonaName(lobbyOwner);
  }

  protected virtual void onLobbyEntered(GalaxyID lobby_id, LobbyEnterResult result)
  {
  }

  private void onLobbyJoinRequested(GalaxyID userID, string connectionString)
  {
    this.lobbyRequested = this.parseConnectionString(connectionString);
    if (!(this.lobbyRequested != (GalaxyID) null))
      return;
    Game1.multiplayer.inviteAccepted();
  }

  public bool SupportsInviteCodes() => true;

  /// <summary>Gets a GOG Galaxy user ID from an invite code.</summary>
  /// <param name="inviteCode">The invite code string to parse.</param>
  /// <returns>Returns a valid GOG Galaxy user ID for the lobby corresponding to <paramref name="inviteCode" />, or <c>null</c> if none was found.</returns>
  public static GalaxyID GetLobbyFromGalaxyInvite(string inviteCode)
  {
    if (inviteCode.Length <= 1)
      return (GalaxyID) null;
    switch (inviteCode[0])
    {
      case 'G':
      case 'S':
        ulong num;
        try
        {
          num = Base36.Decode(inviteCode.Substring(1));
        }
        catch (FormatException ex)
        {
          return (GalaxyID) null;
        }
        return num == 0UL || num >> 56 != 0UL ? (GalaxyID) null : GalaxyID.FromRealID(GalaxyID.IDType.ID_TYPE_LOBBY, num);
      default:
        return (GalaxyID) null;
    }
  }

  public object GetLobbyFromInviteCode(string inviteCode)
  {
    GalaxyID fromGalaxyInvite = GalaxyNetHelper.GetLobbyFromGalaxyInvite(inviteCode);
    return fromGalaxyInvite == (GalaxyID) null ? (object) null : (object) fromGalaxyInvite.ToUint64();
  }

  public virtual void ShowInviteDialog(object lobby)
  {
    GalaxyInstance.Friends().ShowOverlayInviteDialog("-connect-lobby-" + Convert.ToString((ulong) lobby));
  }

  public void MutePlayer(string userId, bool mute)
  {
  }

  public bool IsPlayerMuted(string userId) => false;

  public void ShowProfile(string userId)
  {
  }
}
