// Decompiled with JetBrains decompiler
// Type: StardewValley.SDKs.Steam.SteamNetHelper
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Galaxy.Api;
using StardewValley.Network;
using StardewValley.SDKs.GogGalaxy;
using StardewValley.SDKs.Steam.Internal;
using Steamworks;
using System;
using System.Collections.Generic;

#nullable disable
namespace StardewValley.SDKs.Steam;

internal sealed class SteamNetHelper : SDKNetHelper
{
  /// <summary>List of active listeners to call when we receive lobby update events.</summary>
  private List<LobbyUpdateListener> LobbyUpdateListeners;
  /// <summary>The callback used to receive lobby data updates and pass them to <see cref="F:StardewValley.SDKs.Steam.SteamNetHelper.LobbyUpdateListeners" />.</summary>
  private readonly Callback<LobbyDataUpdate_t> LobbyDataUpdateCallback;
  /// <summary>The callback used to handle requests to join lobbies, either through Steam overlay or by invite.</summary>
  private readonly Callback<GameLobbyJoinRequested_t> GameLobbyJoinRequestedCallback;
  /// <summary>The lobby the player requested to join, either through Steam overlay or by invite.</summary>
  private HybridLobby RequestedLobby;

  /// <summary>Constructs an instance and registers its Steam SDK callbacks.</summary>
  public SteamNetHelper()
  {
    this.LobbyUpdateListeners = new List<LobbyUpdateListener>();
    this.GameLobbyJoinRequestedCallback = Callback<GameLobbyJoinRequested_t>.Create(new Callback<GameLobbyJoinRequested_t>.DispatchDelegate(this.OnGameLobbyJoinRequested));
    this.LobbyDataUpdateCallback = Callback<LobbyDataUpdate_t>.Create(new Callback<LobbyDataUpdate_t>.DispatchDelegate(this.OnLobbyDataUpdate));
    this.RequestedLobby.Clear();
    this.FindLaunchLobby();
  }

  /// <summary>Cleans up the instance and unregisters its Steam SDK callbacks.</summary>
  ~SteamNetHelper()
  {
    this.GameLobbyJoinRequestedCallback.Unregister();
    this.LobbyDataUpdateCallback.Unregister();
  }

  /// <summary>Handles a request to join a Steam lobby.</summary>
  /// <param name="evt">A structure containing information about the lobby join request.</param>
  private void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t evt)
  {
    this.RequestJoinLobby(evt.m_steamIDLobby);
  }

  /// <summary>Handles changes in lobby data (likely in response to <see cref="M:StardewValley.SDKs.Steam.SteamNetHelper.RequestFriendLobbyData" />) and invokes listeners from <see cref="F:StardewValley.SDKs.Steam.SteamNetHelper.LobbyUpdateListeners" />.</summary>
  /// <param name="evt">A structure containing information about the lobby whose data changed.</param>
  private void OnLobbyDataUpdate(LobbyDataUpdate_t evt)
  {
    CSteamID csteamId = new CSteamID(evt.m_ulSteamIDLobby);
    if (SteamMatchmaking.GetLobbyOwner(csteamId) == SteamUser.GetSteamID())
      return;
    HybridLobby lobby = new HybridLobby(csteamId);
    foreach (LobbyUpdateListener lobbyUpdateListener in this.LobbyUpdateListeners)
      lobbyUpdateListener.OnLobbyUpdate((object) lobby);
  }

  /// <summary>Reads the command line arguments to find the launch option "+connect_lobby &lt;lobbyID&gt;".</summary>
  private void FindLaunchLobby()
  {
    CSteamID requestedLobby = new CSteamID();
    string[] commandLineArgs = Environment.GetCommandLineArgs();
    for (int index = 0; index < commandLineArgs.Length - 1; ++index)
    {
      if (!(commandLineArgs[index] != "+connect_lobby"))
      {
        requestedLobby.Clear();
        try
        {
          requestedLobby = new CSteamID(Convert.ToUInt64(commandLineArgs[index + 1]));
          Game1.log.Verbose($"Found startup Steam lobby {requestedLobby.m_SteamID}");
          this.RequestJoinLobby(requestedLobby);
          break;
        }
        catch (Exception ex)
        {
          Game1.log.Verbose("Could not parse argument for +connect_lobby: " + commandLineArgs[index + 1]);
        }
      }
    }
  }

  /// <summary>Queues a request to a lobby if it's a valid Steam lobby ID.</summary>
  /// <param name="requestedLobby">The lobby we are requesting to join.</param>
  private void RequestJoinLobby(CSteamID requestedLobby)
  {
    if (requestedLobby.IsValid() && requestedLobby.IsLobby())
    {
      Game1.log.Verbose($"Requesting to join Steam lobby {requestedLobby.m_SteamID}");
      this.RequestedLobby = new HybridLobby(requestedLobby);
      Game1.multiplayer.inviteAccepted();
    }
    else
      Game1.log.Verbose($"Denied request to join invalid Steam lobby {requestedLobby.m_SteamID}");
  }

  public string GetUserID()
  {
    try
    {
      return GalaxyInstance.User().GetGalaxyID().ToUint64().ToString();
    }
    catch (Exception ex)
    {
      return "";
    }
  }

  /// <summary>Creates a client corresponding to the type of <paramref name="lobby" />.</summary>
  /// <param name="lobby">The lobby that we will be joining with the resulting client.</param>
  /// <returns>Returns a client that will join <paramref name="lobby" />.</returns>
  private Client CreateClientFromHybrid(HybridLobby lobby)
  {
    switch (lobby.LobbyType)
    {
      case LobbyConnectionType.Steam:
        return (Client) new SteamNetClient(new CSteamID(lobby.SteamId));
      case LobbyConnectionType.Galaxy:
        return (Client) new GalaxyNetClient(new GalaxyID(lobby.GalaxyId));
      case LobbyConnectionType.Hybrid:
        return (Client) new SteamNetClient(new GalaxyID(lobby.GalaxyId));
      default:
        return (Client) null;
    }
  }

  /// <summary>Creates a client with <see cref="M:StardewValley.SDKs.Steam.SteamNetHelper.CreateClientFromHybrid(StardewValley.SDKs.Steam.Internal.HybridLobby)" /> and initializes it with <see cref="M:StardewValley.Multiplayer.InitClient(StardewValley.Network.Client)" />.</summary>
  /// <param name="lobby">The lobby that we will be joining with the resulting client.</param>
  /// <returns>Returns an initialized client that will join <paramref name="lobby" />.</returns>
  private Client CreateClientHelper(HybridLobby lobby)
  {
    Client clientFromHybrid = this.CreateClientFromHybrid(lobby);
    return clientFromHybrid == null ? (Client) null : Game1.multiplayer.InitClient(clientFromHybrid);
  }

  public Client CreateClient(object lobby)
  {
    return lobby is HybridLobby lobby1 ? this.CreateClientHelper(lobby1) : (Client) null;
  }

  public Client GetRequestedClient()
  {
    Client clientHelper = this.CreateClientHelper(this.RequestedLobby);
    this.RequestedLobby.Clear();
    return clientHelper;
  }

  /// <summary>Creates an additional Steam server with an underlying <paramref name="gameServer" />.</summary>
  /// <param name="gameServer">The master game server that manages all <see cref="T:StardewValley.Network.Server" /> objects.</param>
  /// <returns>Returns an initialized instance of <see cref="T:StardewValley.SDKs.Steam.SteamNetServer" />.</returns>
  public Server CreateSteamServer(IGameServer gameServer)
  {
    return Game1.multiplayer.InitServer((Server) new SteamNetServer(gameServer));
  }

  public Server CreateServer(IGameServer gameServer)
  {
    if (!(Program.sdk is SteamHelper sdk) || sdk.GalaxyConnected)
      return Game1.multiplayer.InitServer((Server) new GalaxyNetServer(gameServer));
    Game1.log.Error("Could not create a Galaxy server: not logged on");
    return (Server) null;
  }

  public void AddLobbyUpdateListener(LobbyUpdateListener listener)
  {
    this.LobbyUpdateListeners.Add(listener);
  }

  public void RemoveLobbyUpdateListener(LobbyUpdateListener listener)
  {
    this.LobbyUpdateListeners.Remove(listener);
  }

  public void RequestFriendLobbyData()
  {
    int friendCount = SteamFriends.GetFriendCount(EFriendFlags.k_EFriendFlagImmediate);
    for (int iFriend = 0; iFriend < friendCount; ++iFriend)
    {
      CSteamID friendByIndex = SteamFriends.GetFriendByIndex(iFriend, EFriendFlags.k_EFriendFlagImmediate);
      if (!(friendByIndex == SteamUser.GetSteamID()))
      {
        FriendGameInfo_t pFriendGameInfo;
        SteamFriends.GetFriendGamePlayed(friendByIndex, out pFriendGameInfo);
        if (!(pFriendGameInfo.m_gameID.AppID() != SteamUtils.GetAppID()))
          SteamMatchmaking.RequestLobbyData(pFriendGameInfo.m_steamIDLobby);
      }
    }
  }

  public string GetLobbyData(object lobby, string key)
  {
    if (!(lobby is HybridLobby hybridLobby))
      return "";
    switch (hybridLobby.LobbyType)
    {
      case LobbyConnectionType.Steam:
        return SteamMatchmaking.GetLobbyData(new CSteamID(hybridLobby.SteamId), key);
      case LobbyConnectionType.Galaxy:
      case LobbyConnectionType.Hybrid:
        try
        {
          return GalaxyInstance.Matchmaking().GetLobbyData(new GalaxyID(hybridLobby.GalaxyId), key);
        }
        catch (Exception ex)
        {
          return "";
        }
      default:
        return "";
    }
  }

  public string GetLobbyOwnerName(object lobby)
  {
    if (!(lobby is HybridLobby hybridLobby))
      return (string) null;
    switch (hybridLobby.LobbyType)
    {
      case LobbyConnectionType.Steam:
        return SteamFriends.GetFriendPersonaName(SteamMatchmaking.GetLobbyOwner(new CSteamID(hybridLobby.SteamId)));
      case LobbyConnectionType.Galaxy:
        try
        {
          GalaxyID lobbyOwner = GalaxyInstance.Matchmaking().GetLobbyOwner(new GalaxyID(hybridLobby.GalaxyId));
          return GalaxyInstance.Friends().GetFriendPersonaName(lobbyOwner);
        }
        catch (Exception ex)
        {
          return "";
        }
      case LobbyConnectionType.Hybrid:
        return GalaxyNetHelper.TryGetHostSteamDisplayName(new GalaxyID(hybridLobby.GalaxyId)) ?? "";
      default:
        return "";
    }
  }

  public bool SupportsInviteCodes() => true;

  public object GetLobbyFromInviteCode(string inviteCode)
  {
    GalaxyID fromGalaxyInvite = GalaxyNetHelper.GetLobbyFromGalaxyInvite(inviteCode);
    return !(fromGalaxyInvite != (GalaxyID) null) ? (object) null : (object) new HybridLobby(fromGalaxyInvite, inviteCode[0] == 'S');
  }

  public void ShowInviteDialog(object lobby)
  {
    if (!(lobby is CSteamID steamIDLobby))
      return;
    SteamFriends.ActivateGameOverlayInviteDialog(steamIDLobby);
  }

  public void MutePlayer(string userId, bool mute)
  {
  }

  public bool IsPlayerMuted(string userId) => false;

  public void ShowProfile(string userId)
  {
  }
}
