// Decompiled with JetBrains decompiler
// Type: StardewValley.SDKs.GogGalaxy.Listeners.GalaxyLobbyDataListener
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Galaxy.Api;
using System;

#nullable disable
namespace StardewValley.SDKs.GogGalaxy.Listeners;

/// <summary>Listener for when we receive an updated version of a Galaxy lobby's data.</summary>
internal sealed class GalaxyLobbyDataListener : ILobbyDataListener
{
  /// <summary>The callback to invoke when the data for a Galaxy lobby changes.</summary>
  private readonly Action<GalaxyID, GalaxyID> Callback;

  /// <summary>Constructs an instance of the listener and registers it with the Galaxy SDK.</summary>
  /// <param name="callback">The callback to invoke when the data for a Galaxy lobby changes.</param>
  public GalaxyLobbyDataListener(Action<GalaxyID, GalaxyID> callback)
  {
    this.Callback = callback;
    GalaxyInstance.ListenerRegistrar().Register(GalaxyTypeAwareListenerLobbyData.GetListenerType(), (IGalaxyListener) this);
  }

  /// <summary>Handles changes to Galaxy lobby data, and passes the information to <see cref="F:StardewValley.SDKs.GogGalaxy.Listeners.GalaxyLobbyDataListener.Callback" />.</summary>
  /// <param name="lobbyID">The Galaxy ID of the lobby.</param>
  /// <param name="memberID">The Galaxy ID the lobby member whose data changed (if applicable).</param>
  public override void OnLobbyDataUpdated(GalaxyID lobbyID, GalaxyID memberID)
  {
    Action<GalaxyID, GalaxyID> callback = this.Callback;
    if (callback == null)
      return;
    callback(lobbyID, memberID);
  }

  /// <summary>Unregisters the listener from the Galaxy SDK.</summary>
  public override void Dispose()
  {
    GalaxyInstance.ListenerRegistrar().Unregister(GalaxyTypeAwareListenerLobbyData.GetListenerType(), (IGalaxyListener) this);
    base.Dispose();
  }
}
