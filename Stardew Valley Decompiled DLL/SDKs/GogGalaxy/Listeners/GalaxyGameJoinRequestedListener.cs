// Decompiled with JetBrains decompiler
// Type: StardewValley.SDKs.GogGalaxy.Listeners.GalaxyGameJoinRequestedListener
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Galaxy.Api;
using System;

#nullable disable
namespace StardewValley.SDKs.GogGalaxy.Listeners;

/// <summary>Listener for when a user requests to join a game on Galaxy, either by accepting an invitation or by joining a friend.</summary>
internal sealed class GalaxyGameJoinRequestedListener : IGameJoinRequestedListener
{
  /// <summary>The callback to invoke when a Galaxy user requests to join a game.</summary>
  private readonly Action<GalaxyID, string> Callback;

  /// <summary>Constructs an instance of the listener and registers it with the Galaxy SDK.</summary>
  /// <param name="callback">The callback to invoke when a Galaxy user requests to join a game.</param>
  public GalaxyGameJoinRequestedListener(Action<GalaxyID, string> callback)
  {
    this.Callback = callback;
    GalaxyInstance.ListenerRegistrar().Register(GalaxyTypeAwareListenerGameJoinRequested.GetListenerType(), (IGalaxyListener) this);
  }

  /// <summary>Handles user requests to join games, and passes the information to <see cref="F:StardewValley.SDKs.GogGalaxy.Listeners.GalaxyGameJoinRequestedListener.Callback" />.</summary>
  /// <param name="lobbyID">The Galaxy ID of the lobby.</param>
  /// <param name="result">The Galaxy connection string.</param>
  public override void OnGameJoinRequested(GalaxyID lobbyID, string result)
  {
    Action<GalaxyID, string> callback = this.Callback;
    if (callback == null)
      return;
    callback(lobbyID, result);
  }

  /// <summary>Unregisters the listener from the Galaxy SDK.</summary>
  public override void Dispose()
  {
    GalaxyInstance.ListenerRegistrar().Unregister(GalaxyTypeAwareListenerGameJoinRequested.GetListenerType(), (IGalaxyListener) this);
    base.Dispose();
  }
}
