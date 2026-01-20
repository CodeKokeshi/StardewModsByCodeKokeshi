// Decompiled with JetBrains decompiler
// Type: StardewValley.SDKs.GogGalaxy.Listeners.GalaxyRichPresenceListener
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Galaxy.Api;
using System;

#nullable disable
namespace StardewValley.SDKs.GogGalaxy.Listeners;

/// <summary>Listener for any change in a Galaxy user's rich presence.</summary>
internal sealed class GalaxyRichPresenceListener : IRichPresenceListener
{
  /// <summary>The callback to invoke when the rich presence for a Galaxy user changes.</summary>
  private readonly Action<GalaxyID> Callback;

  /// <summary>Constructs an instance of the listener and registers it with the Galaxy SDK.</summary>
  /// <param name="callback">The callback to invoke when the rich presence for a Galaxy user changes.</param>
  public GalaxyRichPresenceListener(Action<GalaxyID> callback)
  {
    this.Callback = callback;
    GalaxyInstance.ListenerRegistrar().Register(GalaxyTypeAwareListenerRichPresence.GetListenerType(), (IGalaxyListener) this);
  }

  /// <summary>Handles changes to a Galaxy user's rich presence, and passes the information to <see cref="F:StardewValley.SDKs.GogGalaxy.Listeners.GalaxyRichPresenceListener.Callback" />.</summary>
  /// <param name="userID">The Galaxy ID of the user whose rich presence was updated.</param>
  public override void OnRichPresenceUpdated(GalaxyID userID)
  {
    Action<GalaxyID> callback = this.Callback;
    if (callback == null)
      return;
    callback(userID);
  }

  /// <summary>Unregisters the listener from the Galaxy SDK.</summary>
  public override void Dispose()
  {
    GalaxyInstance.ListenerRegistrar().Unregister(GalaxyTypeAwareListenerRichPresence.GetListenerType(), (IGalaxyListener) this);
    base.Dispose();
  }
}
