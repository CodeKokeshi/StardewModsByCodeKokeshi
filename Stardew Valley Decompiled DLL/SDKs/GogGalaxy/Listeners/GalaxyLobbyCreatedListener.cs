// Decompiled with JetBrains decompiler
// Type: StardewValley.SDKs.GogGalaxy.Listeners.GalaxyLobbyCreatedListener
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Galaxy.Api;
using System;

#nullable disable
namespace StardewValley.SDKs.GogGalaxy.Listeners;

/// <summary>Listener for Galaxy lobby creation.</summary>
internal sealed class GalaxyLobbyCreatedListener : ILobbyCreatedListener
{
  /// <summary>The callback to invoke when creating a Galaxy lobby succeeds or fails.</summary>
  private readonly Action<GalaxyID, LobbyCreateResult> Callback;

  /// <summary>Constructs an instance of the listener and registers it with the Galaxy SDK.</summary>
  /// <param name="callback">The callback to invoke when creating a Galaxy lobby succeeds or fails.</param>
  public GalaxyLobbyCreatedListener(Action<GalaxyID, LobbyCreateResult> callback)
  {
    this.Callback = callback;
    GalaxyInstance.ListenerRegistrar().Register(GalaxyTypeAwareListenerLobbyCreated.GetListenerType(), (IGalaxyListener) this);
  }

  /// <summary>Handles success/failure for Galaxy lobby creation, and passes the information to <see cref="F:StardewValley.SDKs.GogGalaxy.Listeners.GalaxyLobbyCreatedListener.Callback" />.</summary>
  /// <param name="lobbyID">The Galaxy ID of the lobby being created.</param>
  /// <param name="result">An enum representing whether the lobby creation succeeded or failed.</param>
  public override void OnLobbyCreated(GalaxyID lobbyID, LobbyCreateResult result)
  {
    Action<GalaxyID, LobbyCreateResult> callback = this.Callback;
    if (callback == null)
      return;
    callback(lobbyID, result);
  }

  /// <summary>Unregisters the listener from the Galaxy SDK.</summary>
  public override void Dispose()
  {
    GalaxyInstance.ListenerRegistrar().Unregister(GalaxyTypeAwareListenerLobbyCreated.GetListenerType(), (IGalaxyListener) this);
    base.Dispose();
  }
}
