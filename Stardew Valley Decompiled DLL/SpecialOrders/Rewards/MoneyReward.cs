// Decompiled with JetBrains decompiler
// Type: StardewValley.SpecialOrders.Rewards.MoneyReward
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Netcode;
using System.Collections.Generic;

#nullable disable
namespace StardewValley.SpecialOrders.Rewards;

public class MoneyReward : OrderReward
{
  public NetInt amount = new NetInt(0);
  public NetFloat multiplier = new NetFloat(1f);

  public override void InitializeNetFields()
  {
    base.InitializeNetFields();
    this.NetFields.AddField((INetSerializable) this.amount, "amount").AddField((INetSerializable) this.multiplier, "multiplier");
  }

  public virtual int GetRewardMoneyAmount()
  {
    return (int) ((double) this.amount.Value * (double) this.multiplier.Value);
  }

  public override void Load(SpecialOrder order, Dictionary<string, string> data)
  {
    this.amount.Value = int.Parse(order.Parse(data["Amount"]));
    string data1;
    if (!data.TryGetValue("Multiplier", out data1))
      return;
    this.multiplier.Value = float.Parse(order.Parse(data1));
  }
}
