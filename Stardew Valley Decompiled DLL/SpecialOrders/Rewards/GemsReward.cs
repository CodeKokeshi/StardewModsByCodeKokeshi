// Decompiled with JetBrains decompiler
// Type: StardewValley.SpecialOrders.Rewards.GemsReward
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Netcode;
using System.Collections.Generic;

#nullable disable
namespace StardewValley.SpecialOrders.Rewards;

public class GemsReward : OrderReward
{
  public NetInt amount = new NetInt(0);

  public override void InitializeNetFields()
  {
    base.InitializeNetFields();
    this.NetFields.AddField((INetSerializable) this.amount, "amount");
  }

  public override void Load(SpecialOrder order, Dictionary<string, string> data)
  {
    this.amount.Value = int.Parse(order.Parse(data["Amount"]));
  }

  public override void Grant() => Game1.player.QiGems += this.amount.Value;
}
