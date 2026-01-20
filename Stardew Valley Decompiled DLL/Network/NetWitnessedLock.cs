// Decompiled with JetBrains decompiler
// Type: StardewValley.Network.NetWitnessedLock
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Netcode;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley.Network;

public class NetWitnessedLock : INetObject<NetFields>
{
  private readonly NetBool requested = new NetBool().Interpolated(false, false);
  private readonly NetFarmerCollection witnesses = new NetFarmerCollection();
  private Action acquired;

  [XmlIgnore]
  public NetFields NetFields { get; } = new NetFields(nameof (NetWitnessedLock));

  public NetWitnessedLock()
  {
    this.NetFields.SetOwner((INetObject<NetFields>) this).AddField((INetSerializable) this.requested, nameof (requested)).AddField((INetSerializable) this.witnesses.NetFields, "witnesses.NetFields");
  }

  public void RequestLock(Action acquired, Action failed)
  {
    if (!Game1.IsMasterGame)
      throw new InvalidOperationException();
    if (acquired == null)
      throw new ArgumentException();
    if (this.requested.Value)
    {
      failed();
    }
    else
    {
      this.requested.Value = true;
      this.acquired = acquired;
    }
  }

  public bool IsLocked() => this.requested.Value;

  public void Update()
  {
    this.witnesses.RetainOnlinePlayers();
    if (!this.requested.Value)
      return;
    if (!this.witnesses.Contains(Game1.player))
      this.witnesses.Add(Game1.player);
    if (!Game1.IsMasterGame)
      return;
    foreach (Farmer farmer in (IEnumerable<Farmer>) Game1.otherFarmers.Values)
    {
      if (!this.witnesses.Contains(farmer))
        return;
    }
    this.acquired();
    this.acquired = (Action) null;
    this.requested.Value = false;
    this.witnesses.Clear();
  }
}
