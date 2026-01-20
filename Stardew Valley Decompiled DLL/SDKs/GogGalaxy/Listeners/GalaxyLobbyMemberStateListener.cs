// Decompiled with JetBrains decompiler
// Type: StardewValley.SDKs.GogGalaxy.Listeners.GalaxyLobbyMemberStateListener
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Galaxy.Api;
using System;

#nullable disable
namespace StardewValley.SDKs.GogGalaxy.Listeners;

/// <summary>Listener for events related to Galaxy lobby member state changes (joining, leaving, disconnecting, etc.).</summary>
internal sealed class GalaxyLobbyMemberStateListener : ILobbyMemberStateListener
{
  /// <summary>The callback to invoke when a Galaxy lobby member changes state.</summary>
  private readonly Action<GalaxyID, GalaxyID, LobbyMemberStateChange> Callback;

  /// <summary>Constructs an instance of the listener and registers it with the Galaxy SDK.</summary>
  /// <param name="callback">The callback to invoke when a Galaxy lobby member changes state.</param>
  public GalaxyLobbyMemberStateListener(
    Action<GalaxyID, GalaxyID, LobbyMemberStateChange> callback)
  {
    this.Callback = callback;
    GalaxyInstance.ListenerRegistrar().Register(GalaxyTypeAwareListenerLobbyMemberState.GetListenerType(), (IGalaxyListener) this);
  }

  /// <summary>Handles Galaxy lobby member state changes (joining, leaving, disconnecting, etc.) and passes the information to <see cref="F:StardewValley.SDKs.GogGalaxy.Listeners.GalaxyLobbyMemberStateListener.Callback" />.</summary>
  /// <param name="lobbyID">The Galaxy ID of the lobby.</param>
  /// <param name="memberID">The Galaxy ID of the lobby member whose state changed.</param>
  /// <param name="memberStateChange">The updated state of the lobby member.</param>
  public override void OnLobbyMemberStateChanged(
    GalaxyID lobbyID,
    GalaxyID memberID,
    LobbyMemberStateChange memberStateChange)
  {
    Action<GalaxyID, GalaxyID, LobbyMemberStateChange> callback = this.Callback;
    if (callback == null)
      return;
    callback(lobbyID, memberID, memberStateChange);
  }

  /// <summary>Unregisters the listener from the Galaxy SDK.</summary>
  public override void Dispose()
  {
    GalaxyInstance.ListenerRegistrar().Unregister(GalaxyTypeAwareListenerLobbyMemberState.GetListenerType(), (IGalaxyListener) this);
    base.Dispose();
  }
}
