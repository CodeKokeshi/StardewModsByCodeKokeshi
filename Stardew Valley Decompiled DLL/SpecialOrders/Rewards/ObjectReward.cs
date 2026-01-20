// Decompiled with JetBrains decompiler
// Type: StardewValley.SpecialOrders.Rewards.ObjectReward
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Netcode;
using Netcode.Validation;
using System.Collections.Generic;

#nullable disable
namespace StardewValley.SpecialOrders.Rewards;

public class ObjectReward : OrderReward
{
  public readonly NetString itemKey = new NetString("");
  public readonly NetInt amount = new NetInt(0);
  [NotNetField]
  private Object _objectInstance;

  /// <summary>The item stack to be drawn on the special orders board.</summary>
  public Object objectInstance
  {
    get
    {
      if (this._objectInstance == null && !string.IsNullOrEmpty(this.itemKey.Value) && this.amount.Value > 0)
        this._objectInstance = new Object(this.itemKey.Value, this.amount.Value);
      return this._objectInstance;
    }
  }

  public override void InitializeNetFields()
  {
    base.InitializeNetFields();
    this.NetFields.AddField((INetSerializable) this.itemKey, "itemKey").AddField((INetSerializable) this.amount, "amount");
  }

  public override void Load(SpecialOrder order, Dictionary<string, string> data)
  {
    this.itemKey.Value = order.Parse(data["Item"]);
    this.amount.Value = int.Parse(order.Parse(data["Amount"]));
    this._objectInstance = new Object(this.itemKey.Value, this.amount.Value);
  }

  public override void Grant()
  {
    Game1.player.addItemByMenuIfNecessary((Item) new Object(this.itemKey.Value, this.amount.Value));
  }
}
