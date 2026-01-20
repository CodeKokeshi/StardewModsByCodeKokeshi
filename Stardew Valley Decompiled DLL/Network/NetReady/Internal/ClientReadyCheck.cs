// Decompiled with JetBrains decompiler
// Type: StardewValley.Network.NetReady.Internal.ClientReadyCheck
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using System.Collections.Generic;

#nullable disable
namespace StardewValley.Network.NetReady.Internal;

/// <summary>A cancelable ready-check for a farmhand player.</summary>
/// <inheritdoc />
internal sealed class ClientReadyCheck(string id) : BaseReadyCheck(id)
{
  /// <inheritdoc />
  public override void SetRequiredFarmers(List<long> farmerIds)
  {
    if (farmerIds == null)
    {
      int num = 0;
      foreach (Farmer onlineFarmer in Game1.getOnlineFarmers())
      {
        if (!Game1.multiplayer.isDisconnecting(onlineFarmer) && !onlineFarmer.IsDedicatedPlayer)
          ++num;
      }
      this.NumberRequired = num;
      this.SendMessage(ReadyCheckMessageType.RequireFarmers, (object) -1);
    }
    else
    {
      this.NumberRequired = farmerIds.Count;
      object[] objArray = new object[farmerIds.Count + 1];
      objArray[0] = (object) farmerIds.Count;
      for (int index = 0; index < farmerIds.Count; ++index)
        objArray[index + 1] = (object) farmerIds[index];
      this.SendMessage(ReadyCheckMessageType.RequireFarmers, objArray);
    }
  }

  /// <inheritdoc />
  public override bool SetLocalReady(bool ready)
  {
    if (!base.SetLocalReady(ready))
      return false;
    ++this.NumberReady;
    this.SendMessage(ready ? ReadyCheckMessageType.Ready : ReadyCheckMessageType.Cancel);
    return true;
  }

  /// <inheritdoc />
  public override void Update()
  {
  }

  /// <inheritdoc />
  public override void ProcessMessage(ReadyCheckMessageType messageType, IncomingMessage message)
  {
    switch (messageType)
    {
      case ReadyCheckMessageType.Lock:
        this.ProcessLock(message);
        break;
      case ReadyCheckMessageType.Release:
        this.ProcessRelease(message);
        break;
      case ReadyCheckMessageType.UpdateAmounts:
        this.ProcessUpdateAmounts(message);
        break;
      case ReadyCheckMessageType.Finish:
        this.ProcessFinish(message);
        break;
      default:
        Game1.log.Warn($"{nameof (ClientReadyCheck)} '{this.Id}' received invalid message type '{messageType}'.");
        break;
    }
  }

  /// <inheritdoc />
  protected override void SendMessage(ReadyCheckMessageType messageType, params object[] data)
  {
    Game1.client?.sendMessage(this.CreateSyncMessage(messageType, data));
  }

  /// <summary>Handle a request to mark this check as non-cancelable.</summary>
  /// <param name="message">The incoming <see cref="F:StardewValley.Network.NetReady.Internal.ReadyCheckMessageType.Lock" /> message.</param>
  private void ProcessLock(IncomingMessage message)
  {
    this.ActiveLockId = message.Reader.ReadInt32();
    if (this.State == ReadyState.NotReady)
    {
      this.SendMessage(ReadyCheckMessageType.RejectLock, (object) this.ActiveLockId);
    }
    else
    {
      this.State = ReadyState.Locked;
      this.SendMessage(ReadyCheckMessageType.AcceptLock, (object) this.ActiveLockId);
    }
  }

  /// <summary>Handle a request to mark this check as cancelable.</summary>
  /// <param name="message">The incoming <see cref="F:StardewValley.Network.NetReady.Internal.ReadyCheckMessageType.Release" /> message.</param>
  private void ProcessRelease(IncomingMessage message)
  {
    int num = message.Reader.ReadInt32();
    if (this.State != ReadyState.Locked || num != this.ActiveLockId)
      return;
    this.State = ReadyState.Ready;
  }

  /// <summary>Handle a request to update the displayed ready and required farmer counts.</summary>
  /// <param name="message">The incoming <see cref="F:StardewValley.Network.NetReady.Internal.ReadyCheckMessageType.UpdateAmounts" /> message.</param>
  private void ProcessUpdateAmounts(IncomingMessage message)
  {
    this.NumberReady = message.Reader.ReadInt32();
    this.NumberRequired = message.Reader.ReadInt32();
  }

  /// <summary>Handle a request to flag this check as ready to proceed.</summary>
  /// <param name="message">The incoming <see cref="F:StardewValley.Network.NetReady.Internal.ReadyCheckMessageType.Finish" /> message.</param>
  private void ProcessFinish(IncomingMessage message) => this.IsReady = true;
}
