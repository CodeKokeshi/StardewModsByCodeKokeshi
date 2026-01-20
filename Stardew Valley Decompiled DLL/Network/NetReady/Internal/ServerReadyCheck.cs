// Decompiled with JetBrains decompiler
// Type: StardewValley.Network.NetReady.Internal.ServerReadyCheck
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using System.Collections.Generic;

#nullable disable
namespace StardewValley.Network.NetReady.Internal;

/// <summary>A cancelable ready-check for the host player.</summary>
/// <inheritdoc />
internal sealed class ServerReadyCheck(string id) : BaseReadyCheck(id)
{
  /// <summary>The ready states for all farmers required by this ready check.</summary>
  private readonly Dictionary<long, ReadyState> ReadyStates = new Dictionary<long, ReadyState>();
  /// <summary>Whether we're currently attempting to lock all clients.</summary>
  private bool Locking;
  /// <summary>All farmers that should be included in this check.</summary>
  private readonly HashSet<long> RequiredFarmers = new HashSet<long>();

  /// <summary>Whether all farmers (including those that recently joined) should be included in this check.</summary>
  private bool IncludesAll => this.RequiredFarmers.Count == 0;

  /// <inheritdoc />
  public override void SetRequiredFarmers(List<long> farmerIds)
  {
    this.RequireFarmers((ICollection<long>) farmerIds);
  }

  /// <inheritdoc />
  public override bool SetLocalReady(bool ready)
  {
    if (!base.SetLocalReady(ready))
      return false;
    if (!this.IsFarmerRequired(Game1.player.UniqueMultiplayerID))
    {
      this.State = ReadyState.NotReady;
      return false;
    }
    this.ReadyStates[Game1.player.UniqueMultiplayerID] = this.State;
    return true;
  }

  /// <inheritdoc />
  public override void Update()
  {
    if (this.IsReady)
      return;
    int num1 = 0;
    int num2 = 0;
    int num3 = 0;
    bool flag = this.IsFarmerRequired(Game1.player.UniqueMultiplayerID);
    foreach (Farmer onlineFarmer in Game1.getOnlineFarmers())
    {
      if (this.IsFarmerRequired(onlineFarmer.UniqueMultiplayerID) && !Game1.multiplayer.isDisconnecting(onlineFarmer))
      {
        ReadyState readyState;
        if (!this.ReadyStates.TryGetValue(onlineFarmer.UniqueMultiplayerID, out readyState))
        {
          readyState = ReadyState.NotReady;
          this.ReadyStates[onlineFarmer.UniqueMultiplayerID] = readyState;
        }
        ++num2;
        switch (readyState)
        {
          case ReadyState.Ready:
            ++num1;
            continue;
          case ReadyState.Locked:
            ++num1;
            ++num3;
            continue;
          default:
            continue;
        }
      }
    }
    if (num1 != this.NumberReady || num2 != this.NumberRequired)
    {
      if (flag && Game1.IsDedicatedHost)
        this.SendMessage(ReadyCheckMessageType.UpdateAmounts, (object) (num1 - (this.State == ReadyState.Ready ? 1 : 0)), (object) (num2 - 1));
      else
        this.SendMessage(ReadyCheckMessageType.UpdateAmounts, (object) num1, (object) num2);
      if (num1 == num2)
      {
        if (!this.Locking)
        {
          ++this.ActiveLockId;
          this.Locking = true;
          if (flag && this.State == ReadyState.Ready)
          {
            this.ReadyStates[Game1.player.UniqueMultiplayerID] = this.State = ReadyState.Locked;
            num3 = 1;
          }
          this.SendMessage(ReadyCheckMessageType.Lock, (object) this.ActiveLockId);
        }
      }
      else if (this.Locking)
      {
        this.Locking = false;
        if (this.State == ReadyState.Locked)
          this.State = ReadyState.Ready;
        foreach (long key in this.ReadyStates.Keys)
        {
          if (this.ReadyStates[key] == ReadyState.Locked && this.IsFarmerRequired(key))
            this.ReadyStates[key] = ReadyState.Ready;
        }
        num3 = 0;
        this.SendMessage(ReadyCheckMessageType.Release, (object) this.ActiveLockId);
      }
    }
    if (this.Locking && num3 == num2)
    {
      this.IsReady = true;
      this.SendMessage(ReadyCheckMessageType.Finish);
    }
    this.NumberReady = num1;
    this.NumberRequired = num2;
  }

  /// <inheritdoc />
  public override void ProcessMessage(ReadyCheckMessageType messageType, IncomingMessage message)
  {
    switch (messageType)
    {
      case ReadyCheckMessageType.Ready:
        this.ProcessReady(message);
        break;
      case ReadyCheckMessageType.Cancel:
        this.ProcessCancel(message);
        break;
      case ReadyCheckMessageType.AcceptLock:
        this.ProcessAcceptLock(message);
        break;
      case ReadyCheckMessageType.RejectLock:
        this.ProcessRejectLock(message);
        break;
      case ReadyCheckMessageType.RequireFarmers:
        this.ProcessRequireFarmers(message);
        break;
      default:
        Game1.log.Warn($"{nameof (ServerReadyCheck)} '{this.Id}' received invalid message type '{messageType}'.");
        break;
    }
  }

  /// <inheritdoc />
  protected override void SendMessage(ReadyCheckMessageType messageType, params object[] data)
  {
    if (Game1.server == null)
      return;
    foreach (Farmer farmer in (IEnumerable<Farmer>) Game1.otherFarmers.Values)
      Game1.server.sendMessage(farmer.UniqueMultiplayerID, this.CreateSyncMessage(messageType, data));
  }

  /// <summary>Handle a request to mark a farmer's state as ready.</summary>
  /// <param name="message">The incoming <see cref="F:StardewValley.Network.NetReady.Internal.ReadyCheckMessageType.Ready" /> message.</param>
  private void ProcessReady(IncomingMessage message)
  {
    if (this.Locking)
      return;
    this.ReadyStates[message.FarmerID] = ReadyState.Ready;
  }

  /// <summary>Handle a request to mark a farmer as non-ready.</summary>
  /// <param name="message">The incoming <see cref="F:StardewValley.Network.NetReady.Internal.ReadyCheckMessageType.Cancel" /> message.</param>
  private void ProcessCancel(IncomingMessage message)
  {
    if (this.Locking)
      return;
    this.ReadyStates[message.FarmerID] = ReadyState.NotReady;
  }

  /// <summary>Handle a request to mark a farmer as locked.</summary>
  /// <param name="message">The incoming <see cref="F:StardewValley.Network.NetReady.Internal.ReadyCheckMessageType.AcceptLock" /> message.</param>
  private void ProcessAcceptLock(IncomingMessage message)
  {
    if (message.Reader.ReadInt32() != this.ActiveLockId)
      return;
    this.ReadyStates[message.FarmerID] = ReadyState.Locked;
  }

  /// <summary>Handle a request to mark a farmer as not ready to lock.</summary>
  /// <param name="message">The incoming <see cref="F:StardewValley.Network.NetReady.Internal.ReadyCheckMessageType.RejectLock" /> message.</param>
  private void ProcessRejectLock(IncomingMessage message)
  {
    if (message.Reader.ReadInt32() != this.ActiveLockId)
      return;
    this.ReadyStates[message.FarmerID] = ReadyState.NotReady;
  }

  /// <summary>Handle a request to set the required farmers for this check.</summary>
  /// <param name="message">The incoming <see cref="F:StardewValley.Network.NetReady.Internal.ReadyCheckMessageType.RequireFarmers" /> message.</param>
  private void ProcessRequireFarmers(IncomingMessage message)
  {
    int num = message.Reader.ReadInt32();
    HashSet<long> farmerIds = new HashSet<long>();
    for (int index = 0; index < num; ++index)
      farmerIds.Add(message.Reader.ReadInt64());
    this.RequireFarmers((ICollection<long>) farmerIds);
  }

  /// <summary>Update the required farmers in <see cref="F:StardewValley.Network.NetReady.Internal.ServerReadyCheck.ReadyStates" /> to be the set of <paramref name="farmerIds" />.</summary>
  /// <param name="farmerIds">The list of farmer multiplayer IDs that should be required for this check.</param>
  private void RequireFarmers(ICollection<long> farmerIds)
  {
    this.RequiredFarmers.Clear();
    if (farmerIds == null)
      return;
    foreach (long farmerId in (IEnumerable<long>) farmerIds)
      this.RequiredFarmers.Add(farmerId);
  }

  /// <summary>Checks if a farmer is required for this ready check to pass.</summary>
  /// <param name="uid">The unique multiplayer ID of the farmer to check.</param>
  private bool IsFarmerRequired(long uid) => this.IncludesAll || this.RequiredFarmers.Contains(uid);
}
