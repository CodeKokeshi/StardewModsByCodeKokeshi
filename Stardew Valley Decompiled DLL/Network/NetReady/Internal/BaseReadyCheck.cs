// Decompiled with JetBrains decompiler
// Type: StardewValley.Network.NetReady.Internal.BaseReadyCheck
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using System;
using System.Collections.Generic;

#nullable disable
namespace StardewValley.Network.NetReady.Internal;

/// <summary>A cancelable ready-check.</summary>
internal abstract class BaseReadyCheck
{
  /// <summary>The unique ID for this ready check.</summary>
  public string Id { get; }

  /// <summary>The ID of the active lock.</summary>
  public int ActiveLockId { get; protected set; }

  /// <summary>The current local ready state of the check.</summary>
  public ReadyState State { get; protected set; }

  /// <summary>The number of farmers that are ready to proceed.</summary>
  public int NumberReady { get; protected set; }

  /// <summary>The number of farmers that are required to proceed.</summary>
  public int NumberRequired { get; protected set; }

  /// <summary>Whether all required farmers are ready to proceed.</summary>
  public bool IsReady { get; protected set; }

  /// <summary>Whether we can still cancel our ready state.</summary>
  public bool IsCancelable => this.State != ReadyState.Locked;

  /// <summary>Construct an instance.</summary>
  /// <param name="id">The unique ID for this ready check.</param>
  protected BaseReadyCheck(string id)
  {
    this.Id = id;
    this.State = ReadyState.NotReady;
    this.NumberReady = 0;
    this.NumberRequired = Game1.getOnlineFarmers().Count;
    this.IsReady = false;
  }

  /// <summary>Set the players that are needed for this ready check to pass.</summary>
  /// <param name="farmerIds">The required player IDs.</param>
  public abstract void SetRequiredFarmers(List<long> farmerIds);

  /// <summary>Set whether the local player is ready to proceed.</summary>
  /// <param name="ready">Whether the local player is ready.</param>
  /// <returns>Returns <c>true</c> if we successfully updated the local state, or <c>false</c> if we can no longer update the state.</returns>
  public virtual bool SetLocalReady(bool ready)
  {
    if (!this.IsCancelable)
      return false;
    int state1 = (int) this.State;
    this.State = ready ? ReadyState.Ready : ReadyState.NotReady;
    int state2 = (int) this.State;
    return state1 != state2;
  }

  /// <summary>Update this ready check.</summary>
  public abstract void Update();

  /// <summary>Process an incoming ready check sync message.</summary>
  /// <param name="messageType">The ready check sync type.</param>
  /// <param name="message">The incoming sync message.</param>
  public abstract void ProcessMessage(ReadyCheckMessageType messageType, IncomingMessage message);

  /// <summary>Send a message to other players.</summary>
  /// <param name="messageType">The ready check sync type.</param>
  /// <param name="data">The message data to send.</param>
  protected abstract void SendMessage(ReadyCheckMessageType messageType, params object[] data);

  /// <summary>Create a ready check sync message that can be sent to other players.</summary>
  /// <param name="messageType">The ready check sync type.</param>
  /// <param name="data">The message data to send.</param>
  protected OutgoingMessage CreateSyncMessage(
    ReadyCheckMessageType messageType,
    params object[] data)
  {
    object[] destinationArray = new object[data.Length + 2];
    destinationArray[0] = (object) this.Id;
    destinationArray[1] = (object) (byte) messageType;
    Array.Copy((Array) data, 0, (Array) destinationArray, 2, data.Length);
    return new OutgoingMessage((byte) 31 /*0x1F*/, Game1.player, destinationArray);
  }
}
