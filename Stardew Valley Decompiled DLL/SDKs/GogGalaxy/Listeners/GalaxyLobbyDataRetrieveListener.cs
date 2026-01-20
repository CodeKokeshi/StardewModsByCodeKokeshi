// Decompiled with JetBrains decompiler
// Type: StardewValley.SDKs.GogGalaxy.Listeners.GalaxyLobbyDataRetrieveListener
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Galaxy.Api;
using System;

#nullable disable
namespace StardewValley.SDKs.GogGalaxy.Listeners;

/// <summary>Listener notified when the Galaxy SDK retrieves lobby data.</summary>
internal sealed class GalaxyLobbyDataRetrieveListener : ILobbyDataRetrieveListener
{
  /// <summary>The callback to invoke when fetching Galaxy lobby data succeeds.</summary>
  private readonly Action<GalaxyID> OnSuccess;
  /// <summary>The callback to invoke when fetching Galaxy lobby data fails.</summary>
  private readonly Action<GalaxyID, ILobbyDataRetrieveListener.FailureReason> OnFailure;

  /// <summary>Constructs an instance of the listener.</summary>
  /// <param name="success">The callback to invoke when fetching Galaxy lobby data succeeds.</param>
  /// <param name="failure">The callback to invoke when fetching Galaxy lobby data fails.</param>
  public GalaxyLobbyDataRetrieveListener(
    Action<GalaxyID> success,
    Action<GalaxyID, ILobbyDataRetrieveListener.FailureReason> failure)
  {
    this.OnSuccess = success;
    this.OnFailure = failure;
  }

  /// <summary>Handles successful retrieval of Galaxy lobby data, and invokes <see cref="F:StardewValley.SDKs.GogGalaxy.Listeners.GalaxyLobbyDataRetrieveListener.OnSuccess" />.</summary>
  public override void OnLobbyDataRetrieveSuccess(GalaxyID lobbyID)
  {
    Action<GalaxyID> onSuccess = this.OnSuccess;
    if (onSuccess == null)
      return;
    onSuccess(lobbyID);
  }

  /// <summary>Handles failure to retrieve Galaxy lobby data, and invokes <see cref="F:StardewValley.SDKs.GogGalaxy.Listeners.GalaxyLobbyDataRetrieveListener.OnFailure" />.</summary>
  public override void OnLobbyDataRetrieveFailure(
    GalaxyID lobbyID,
    ILobbyDataRetrieveListener.FailureReason failureReason)
  {
    Action<GalaxyID, ILobbyDataRetrieveListener.FailureReason> onFailure = this.OnFailure;
    if (onFailure == null)
      return;
    onFailure(lobbyID, failureReason);
  }
}
