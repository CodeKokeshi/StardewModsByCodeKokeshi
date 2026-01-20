// Decompiled with JetBrains decompiler
// Type: StardewValley.SDKs.GogGalaxy.Listeners.GalaxySpecificUserDataListener
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Galaxy.Api;
using System;

#nullable disable
namespace StardewValley.SDKs.GogGalaxy.Listeners;

/// <summary>Listener for events related to Galaxy user data changes for any user.</summary>
internal sealed class GalaxySpecificUserDataListener : ISpecificUserDataListener
{
  /// <summary>The callback to invoke when the user data changes for a Galaxy user.</summary>
  private readonly Action<GalaxyID> Callback;

  /// <summary>Constructs an instance of the listener and registers it with the Galaxy SDK.</summary>
  /// <param name="callback">The callback to invoke when the user data changes for a Galaxy user.</param>
  public GalaxySpecificUserDataListener(Action<GalaxyID> callback)
  {
    this.Callback = callback;
    GalaxyInstance.ListenerRegistrar().Register(GalaxyTypeAwareListenerSpecificUserData.GetListenerType(), (IGalaxyListener) this);
  }

  /// <summary>Handles Galaxy user data changes, and passes the information to <see cref="F:StardewValley.SDKs.GogGalaxy.Listeners.GalaxySpecificUserDataListener.Callback" />.</summary>
  /// <param name="userID">The Galaxy ID of the user whose data was updated.</param>
  public override void OnSpecificUserDataUpdated(GalaxyID userID)
  {
    Action<GalaxyID> callback = this.Callback;
    if (callback == null)
      return;
    callback(userID);
  }

  /// <summary>Unregisters the listener from the Galaxy SDK.</summary>
  public override void Dispose()
  {
    GalaxyInstance.ListenerRegistrar().Unregister(GalaxyTypeAwareListenerSpecificUserData.GetListenerType(), (IGalaxyListener) this);
    base.Dispose();
  }
}
