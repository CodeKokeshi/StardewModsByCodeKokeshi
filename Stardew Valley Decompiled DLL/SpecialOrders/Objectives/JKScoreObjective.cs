// Decompiled with JetBrains decompiler
// Type: StardewValley.SpecialOrders.Objectives.JKScoreObjective
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using System;

#nullable disable
namespace StardewValley.SpecialOrders.Objectives;

public class JKScoreObjective : OrderObjective
{
  protected override void _Register()
  {
    base._Register();
    this._order.onJKScoreAchieved += new Action<Farmer, int>(this.OnNewValue);
  }

  protected override void _Unregister()
  {
    base._Unregister();
    this._order.onJKScoreAchieved -= new Action<Farmer, int>(this.OnNewValue);
  }

  public virtual void OnNewValue(Farmer who, int new_value)
  {
    this.SetCount(Math.Min(Math.Max(new_value, this.currentCount.Value), this.GetMaxCount()));
  }
}
