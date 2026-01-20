// Decompiled with JetBrains decompiler
// Type: StardewValley.SDKs.GogGalaxy.Listeners.GalaxyLobbyEnteredListener
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Galaxy.Api;
using System;

#nullable disable
namespace StardewValley.SDKs.GogGalaxy.Listeners;

/// <summary>Listener for entering a Galaxy lobby.</summary>
internal sealed class GalaxyLobbyEnteredListener : ILobbyEnteredListener
{
  /// <summary>The callback to invoke when entering a Galaxy lobby succeeds or fails.</summary>
  private readonly Action<GalaxyID, LobbyEnterResult> Callback;

  /// <summary>Constructs an instance of the listener and registers it with the Galaxy SDK.</summary>
  /// <param name="callback">The callback to invoke when entering a Galaxy lobby succeeds or fails.</param>
  public GalaxyLobbyEnteredListener(Action<GalaxyID, LobbyEnterResult> callback)
  {
    this.Callback = callback;
    GalaxyInstance.ListenerRegistrar().Register(GalaxyTypeAwareListenerLobbyEntered.GetListenerType(), (IGalaxyListener) this);
  }

  /// <summary>Handles success/failure for entering a Galaxy lobby, and passes the information to <see cref="F:StardewValley.SDKs.GogGalaxy.Listeners.GalaxyLobbyEnteredListener.Callback" />.</summary>
  /// <param name="lobbyID">The Galaxy ID of the lobby that was entered.</param>
  /// <param name="result">An enum representing whether or not we successfully entered the lobby.</param>
  public override void OnLobbyEntered(GalaxyID lobbyID, LobbyEnterResult result)
  {
    Action<GalaxyID, LobbyEnterResult> callback = this.Callback;
    if (callback == null)
      return;
    callback(lobbyID, result);
  }

  /// <summary>Unregisters the listener from the Galaxy SDK.</summary>
  public override void Dispose()
  {
    GalaxyInstance.ListenerRegistrar().Unregister(GalaxyTypeAwareListenerLobbyEntered.GetListenerType(), (IGalaxyListener) this);
    base.Dispose();
  }
}
