// Decompiled with JetBrains decompiler
// Type: StardewValley.SpecialOrders.Objectives.ReachMineFloorObjective
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Netcode;
using StardewValley.Extensions;
using System;
using System.Collections.Generic;

#nullable disable
namespace StardewValley.SpecialOrders.Objectives;

public class ReachMineFloorObjective : OrderObjective
{
  public NetBool skullCave = new NetBool(false);

  public override void InitializeNetFields()
  {
    base.InitializeNetFields();
    this.NetFields.AddField((INetSerializable) this.skullCave, "skullCave");
  }

  public override void Load(SpecialOrder order, Dictionary<string, string> data)
  {
    base.Load(order, data);
    string str;
    if (!data.TryGetValue("SkullCave", out str) || !str.EqualsIgnoreCase("true"))
      return;
    this.skullCave.Value = true;
  }

  protected override void _Register()
  {
    base._Register();
    this._order.onMineFloorReached += new Action<Farmer, int>(this.OnNewValue);
  }

  protected override void _Unregister()
  {
    base._Unregister();
    this._order.onMineFloorReached -= new Action<Farmer, int>(this.OnNewValue);
  }

  public virtual void OnNewValue(Farmer who, int new_value)
  {
    if (this.skullCave.Value)
      new_value -= 120;
    else if (new_value > 120)
      return;
    if (new_value <= 0)
      return;
    this.SetCount(Math.Min(Math.Max(new_value, this.currentCount.Value), this.GetMaxCount()));
  }
}
