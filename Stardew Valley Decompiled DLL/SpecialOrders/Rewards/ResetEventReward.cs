// Decompiled with JetBrains decompiler
// Type: StardewValley.SpecialOrders.Rewards.ResetEventReward
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Netcode;
using System.Collections.Generic;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley.SpecialOrders.Rewards;

public class ResetEventReward : OrderReward
{
  [XmlArrayItem("int")]
  public NetStringList resetEvents = new NetStringList();

  public override void InitializeNetFields()
  {
    base.InitializeNetFields();
    this.NetFields.AddField((INetSerializable) this.resetEvents, "resetEvents");
  }

  public override void Load(SpecialOrder order, Dictionary<string, string> data)
  {
    this.resetEvents.AddRange((IEnumerable<string>) ArgUtility.SplitBySpace(order.Parse(data["ResetEvents"])));
  }

  public override void Grant()
  {
    foreach (string resetEvent in (NetList<string, NetString>) this.resetEvents)
      Game1.player.eventsSeen.Remove(resetEvent);
  }
}
