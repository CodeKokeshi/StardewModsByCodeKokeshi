// Decompiled with JetBrains decompiler
// Type: StardewValley.Network.NetMutex
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Netcode;
using System;
using System.Linq;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley.Network;

public class NetMutex : INetObject<NetFields>
{
  public const long NoOwner = -1;
  private long prevOwner;
  private readonly NetLong owner;
  private readonly NetEvent1Field<long, NetLong> lockRequest;
  private Action onLockAcquired;
  private Action onLockFailed;

  [XmlIgnore]
  public NetFields NetFields { get; }

  public NetMutex()
  {
    NetLong netLong = new NetLong(-1L);
    netLong.InterpolationWait = false;
    this.owner = netLong;
    NetEvent1Field<long, NetLong> netEvent1Field = new NetEvent1Field<long, NetLong>();
    netEvent1Field.InterpolationWait = false;
    this.lockRequest = netEvent1Field;
    this.NetFields = new NetFields(nameof (NetMutex));
    // ISSUE: explicit constructor call
    base.\u002Ector();
    this.NetFields.SetOwner((INetObject<NetFields>) this).AddField((INetSerializable) this.owner, nameof (owner)).AddField((INetSerializable) this.lockRequest, nameof (lockRequest));
    this.lockRequest.onEvent += (AbstractNetEvent1<long>.Event) (playerId =>
    {
      if (!Game1.IsMasterGame || this.owner.Value != -1L && this.owner.Value != playerId)
        return;
      this.owner.Value = playerId;
      this.owner.MarkDirty();
    });
  }

  public void RequestLock(Action acquired = null, Action failed = null)
  {
    if (this.owner.Value == Game1.player.UniqueMultiplayerID)
    {
      if (acquired == null)
        return;
      acquired();
    }
    else if (this.owner.Value != -1L)
    {
      if (failed == null)
        return;
      failed();
    }
    else
    {
      this.lockRequest.Fire(Game1.player.UniqueMultiplayerID);
      this.onLockAcquired = acquired;
      this.onLockFailed = failed;
    }
  }

  public void ReleaseLock()
  {
    this.owner.Value = -1L;
    this.onLockFailed = (Action) null;
    this.onLockAcquired = (Action) null;
  }

  public bool IsLocked() => this.owner.Value != -1L;

  public bool IsLockHeld() => this.owner.Value == Game1.player.UniqueMultiplayerID;

  public void Update(GameLocation location) => this.Update(location.farmers);

  public void Update(FarmerCollection farmers)
  {
    this.lockRequest.Poll();
    if (this.owner.Value != this.prevOwner)
    {
      if (this.owner.Value == Game1.player.UniqueMultiplayerID && this.onLockAcquired != null)
        this.onLockAcquired();
      if (this.owner.Value != Game1.player.UniqueMultiplayerID && this.onLockFailed != null)
        this.onLockFailed();
      this.onLockAcquired = (Action) null;
      this.onLockFailed = (Action) null;
      this.prevOwner = this.owner.Value;
    }
    if (!Game1.IsMasterGame || this.owner.Value == -1L || farmers.FirstOrDefault<Farmer>((Func<Farmer, bool>) (f => f.UniqueMultiplayerID == this.owner.Value && f.locationBeforeForcedEvent.Value == null)) != null)
      return;
    this.ReleaseLock();
  }
}
