// Decompiled with JetBrains decompiler
// Type: StardewValley.SDKs.GogGalaxy.Listeners.GalaxyOperationalStateChangeListener
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Galaxy.Api;
using System;

#nullable disable
namespace StardewValley.SDKs.GogGalaxy.Listeners;

/// <summary>Listener for changes in Galaxy's operational state (e.g. signing in and logging on).</summary>
internal sealed class GalaxyOperationalStateChangeListener : IOperationalStateChangeListener
{
  /// <summary>The callback to invoke when Galaxy's operational state changes.</summary>
  private readonly Action<uint> Callback;

  /// <summary>Constructs an instance of the listener and registers it with the Galaxy SDK.</summary>
  /// <param name="callback">The callback to invoke when Galaxy's operational state changes.</param>
  public GalaxyOperationalStateChangeListener(Action<uint> callback)
  {
    this.Callback = callback;
    GalaxyInstance.ListenerRegistrar().Register(GalaxyTypeAwareListenerOperationalStateChange.GetListenerType(), (IGalaxyListener) this);
  }

  /// <summary>Handles operational state changes, and passes the information to <see cref="F:StardewValley.SDKs.GogGalaxy.Listeners.GalaxyOperationalStateChangeListener.Callback" />.</summary>
  /// <param name="operationalState">A bit-field representing the operational state change.</param>
  public override void OnOperationalStateChanged(uint operationalState)
  {
    Action<uint> callback = this.Callback;
    if (callback == null)
      return;
    callback(operationalState);
  }

  /// <summary>Unregisters the listener from the Galaxy SDK.</summary>
  public override void Dispose()
  {
    GalaxyInstance.ListenerRegistrar().Unregister(GalaxyTypeAwareListenerOperationalStateChange.GetListenerType(), (IGalaxyListener) this);
    base.Dispose();
  }
}
