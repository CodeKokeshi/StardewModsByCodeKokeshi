// Decompiled with JetBrains decompiler
// Type: StardewValley.SpecialOrders.Rewards.FriendshipReward
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Netcode;
using System.Collections.Generic;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley.SpecialOrders.Rewards;

public class FriendshipReward : OrderReward
{
  [XmlElement("targetName")]
  public NetString targetName = new NetString();
  [XmlElement("amount")]
  public NetInt amount = new NetInt();

  public override void InitializeNetFields()
  {
    base.InitializeNetFields();
    this.NetFields.AddField((INetSerializable) this.targetName, "targetName").AddField((INetSerializable) this.amount, "amount");
  }

  public override void Load(SpecialOrder order, Dictionary<string, string> data)
  {
    string data1;
    if (!data.TryGetValue("TargetName", out data1))
      data1 = order.requester.Value;
    this.targetName.Value = order.Parse(data1);
    string valueOrDefault = data.GetValueOrDefault<string, string>("Amount", "250");
    this.amount.Value = int.Parse(order.Parse(valueOrDefault));
  }

  public override void Grant()
  {
    NPC characterFromName = Game1.getCharacterFromName(this.targetName.Value);
    if (characterFromName == null)
      return;
    Game1.player.changeFriendship(this.amount.Value, characterFromName);
  }
}
