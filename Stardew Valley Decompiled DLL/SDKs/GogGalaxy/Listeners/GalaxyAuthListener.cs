// Decompiled with JetBrains decompiler
// Type: StardewValley.SDKs.GogGalaxy.Listeners.GalaxyAuthListener
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Galaxy.Api;
using System;

#nullable disable
namespace StardewValley.SDKs.GogGalaxy.Listeners;

/// <summary>Listener for events related to Galaxy user authentication.</summary>
internal sealed class GalaxyAuthListener : IAuthListener
{
  /// <summary>The callback to invoke when Galaxy user authentication succeeds.</summary>
  private readonly Action OnSuccess;
  /// <summary>The callback to invoke when Galaxy user authentication fails.</summary>
  private readonly Action<IAuthListener.FailureReason> OnFailure;
  /// <summary>The callback to invoke when Galaxy loses user authentication.</summary>
  private readonly Action OnLost;

  /// <summary>Constructs an instance of the listener and registers it with the Galaxy SDK.</summary>
  /// <param name="success">The callback to invoke when Galaxy user authentication succeeds.</param>
  /// <param name="failure">The callback to invoke when Galaxy user authentication fails.</param>
  /// <param name="lost">The callback to invoke when Galaxy loses user authentication.</param>
  public GalaxyAuthListener(
    Action success,
    Action<IAuthListener.FailureReason> failure,
    Action lost)
  {
    this.OnSuccess = success;
    this.OnFailure = failure;
    this.OnLost = lost;
    GalaxyInstance.ListenerRegistrar().Register(GalaxyTypeAwareListenerAuth.GetListenerType(), (IGalaxyListener) this);
  }

  /// <summary>Handles user authentication success, and invokes <see cref="F:StardewValley.SDKs.GogGalaxy.Listeners.GalaxyAuthListener.OnSuccess" />.</summary>
  public override void OnAuthSuccess()
  {
    Action onSuccess = this.OnSuccess;
    if (onSuccess == null)
      return;
    onSuccess();
  }

  /// <summary>Handles user authentication failure, and invokes <see cref="F:StardewValley.SDKs.GogGalaxy.Listeners.GalaxyAuthListener.OnFailure" />.</summary>
  public override void OnAuthFailure(IAuthListener.FailureReason reason)
  {
    Action<IAuthListener.FailureReason> onFailure = this.OnFailure;
    if (onFailure == null)
      return;
    onFailure(reason);
  }

  /// <summary>Handles loosing user authentication, and invokes <see cref="F:StardewValley.SDKs.GogGalaxy.Listeners.GalaxyAuthListener.OnLost" />.</summary>
  public override void OnAuthLost()
  {
    Action onLost = this.OnLost;
    if (onLost == null)
      return;
    onLost();
  }

  /// <summary>Unregisters the listener from the Galaxy SDK.</summary>
  public override void Dispose()
  {
    GalaxyInstance.ListenerRegistrar().Unregister(GalaxyTypeAwareListenerAuth.GetListenerType(), (IGalaxyListener) this);
    base.Dispose();
  }
}
