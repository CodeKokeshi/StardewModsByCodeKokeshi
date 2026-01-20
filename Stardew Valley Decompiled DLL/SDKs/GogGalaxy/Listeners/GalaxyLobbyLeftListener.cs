// Decompiled with JetBrains decompiler
// Type: StardewValley.SDKs.GogGalaxy.Listeners.GalaxyLobbyLeftListener
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Galaxy.Api;
using System;

#nullable disable
namespace StardewValley.SDKs.GogGalaxy.Listeners;

/// <summary>Listener for leaving a Galaxy lobby.</summary>
internal sealed class GalaxyLobbyLeftListener : ILobbyLeftListener
{
  /// <summary>The callback to invoke when leaving a Galaxy lobby for any reason.</summary>
  private readonly Action<GalaxyID, ILobbyLeftListener.LobbyLeaveReason> Callback;

  /// <summary>Constructs an instance of the listener and registers it with the Galaxy SDK.</summary>
  /// <param name="callback">The callback to invoke when leaving a Galaxy lobby for any reason.</param>
  public GalaxyLobbyLeftListener(
    Action<GalaxyID, ILobbyLeftListener.LobbyLeaveReason> callback)
  {
    this.Callback = callback;
    GalaxyInstance.ListenerRegistrar().Register(GalaxyTypeAwareListenerLobbyLeft.GetListenerType(), (IGalaxyListener) this);
  }

  /// <summary>Handles leaving a lobby for any reason (leaving normally, losing connection, etc.) and passes the information to <see cref="F:StardewValley.SDKs.GogGalaxy.Listeners.GalaxyLobbyLeftListener.Callback" />.</summary>
  /// <param name="lobbyID">The Galaxy ID of the lobby that was left.</param>
  /// <param name="leaveReason">The reason why we left the lobby.</param>
  public override void OnLobbyLeft(
    GalaxyID lobbyID,
    ILobbyLeftListener.LobbyLeaveReason leaveReason)
  {
    Action<GalaxyID, ILobbyLeftListener.LobbyLeaveReason> callback = this.Callback;
    if (callback == null)
      return;
    callback(lobbyID, leaveReason);
  }

  /// <summary>Unregisters the listener from the Galaxy SDK.</summary>
  public override void Dispose()
  {
    GalaxyInstance.ListenerRegistrar().Unregister(GalaxyTypeAwareListenerLobbyLeft.GetListenerType(), (IGalaxyListener) this);
    base.Dispose();
  }
}
