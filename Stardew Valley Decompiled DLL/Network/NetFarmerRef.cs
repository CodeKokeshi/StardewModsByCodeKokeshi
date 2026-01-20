// Decompiled with JetBrains decompiler
// Type: StardewValley.Network.NetFarmerRef
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Netcode;
using System.Collections;
using System.Collections.Generic;

#nullable disable
namespace StardewValley.Network;

public class NetFarmerRef : INetObject<NetFields>, IEnumerable<long?>, IEnumerable
{
  public readonly NetBool defined = new NetBool();
  public readonly NetLong uid = new NetLong();

  public NetFields NetFields { get; } = new NetFields(nameof (NetFarmerRef));

  public long UID
  {
    get => !this.defined.Value ? 0L : this.uid.Value;
    set
    {
      this.uid.Value = value;
      this.defined.Value = true;
    }
  }

  public Farmer Value
  {
    get => !this.defined.Value ? (Farmer) null : this.getFarmer(this.uid.Value);
    set
    {
      this.defined.Value = value != null;
      this.uid.Value = value != null ? value.UniqueMultiplayerID : 0L;
    }
  }

  public NetFarmerRef()
  {
    this.NetFields.SetOwner((INetObject<NetFields>) this).AddField((INetSerializable) this.defined, nameof (defined)).AddField((INetSerializable) this.uid, nameof (uid));
  }

  private Farmer getFarmer(long uid)
  {
    foreach (Farmer allFarmer in Game1.getAllFarmers())
    {
      if (allFarmer.UniqueMultiplayerID == uid)
        return allFarmer;
    }
    return (Farmer) null;
  }

  public NetFarmerRef Delayed(bool interpolationWait)
  {
    this.defined.Interpolated(false, interpolationWait);
    this.uid.Interpolated(false, interpolationWait);
    return this;
  }

  public void Set(NetFarmerRef other)
  {
    this.uid.Value = other.uid.Value;
    this.defined.Value = other.defined.Value;
  }

  public IEnumerator<long?> GetEnumerator()
  {
    yield return this.defined.Value ? new long?(this.uid.Value) : new long?();
  }

  IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

  public void Add(long? value)
  {
    if (!value.HasValue)
    {
      this.defined.Value = false;
      this.uid.Value = 0L;
    }
    else
    {
      this.defined.Value = true;
      this.uid.Value = value.Value;
    }
  }
}
