// Decompiled with JetBrains decompiler
// Type: StardewValley.SpecialOrders.Rewards.OrderReward
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Netcode;
using System.Collections.Generic;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley.SpecialOrders.Rewards;

[XmlInclude(typeof (FriendshipReward))]
[XmlInclude(typeof (GemsReward))]
[XmlInclude(typeof (MailReward))]
[XmlInclude(typeof (MoneyReward))]
[XmlInclude(typeof (ObjectReward))]
[XmlInclude(typeof (ResetEventReward))]
public class OrderReward : INetObject<NetFields>
{
  [XmlIgnore]
  public NetFields NetFields { get; } = new NetFields(nameof (OrderReward));

  public OrderReward() => this.InitializeNetFields();

  public virtual void InitializeNetFields()
  {
    this.NetFields.SetOwner((INetObject<NetFields>) this);
  }

  public virtual void Grant()
  {
  }

  public virtual void Load(SpecialOrder order, Dictionary<string, string> data)
  {
  }
}
