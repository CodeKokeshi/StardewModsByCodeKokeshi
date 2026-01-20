// Decompiled with JetBrains decompiler
// Type: StardewValley.SpecialOrders.Objectives.DeliverObjective
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Netcode;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley.SpecialOrders.Objectives;

public class DeliverObjective : OrderObjective
{
  [XmlElement("acceptableContextTagSets")]
  public NetStringList acceptableContextTagSets = new NetStringList();
  [XmlElement("targetName")]
  public NetString targetName = new NetString();
  [XmlElement("message")]
  public NetString message = new NetString();

  public override void Load(SpecialOrder order, Dictionary<string, string> data)
  {
    string data1;
    if (data.TryGetValue("AcceptedContextTags", out data1))
      this.acceptableContextTagSets.Add(order.Parse(data1));
    if (data.TryGetValue("TargetName", out data1))
      this.targetName.Value = order.Parse(data1);
    else
      this.targetName.Value = this._order.requester.Value;
    if (data.TryGetValue("Message", out data1))
      this.message.Value = order.Parse(data1);
    else
      this.message.Value = "";
  }

  public override void InitializeNetFields()
  {
    base.InitializeNetFields();
    this.NetFields.AddField((INetSerializable) this.acceptableContextTagSets, "acceptableContextTagSets").AddField((INetSerializable) this.targetName, "targetName").AddField((INetSerializable) this.message, "message");
  }

  public override bool ShouldShowProgress() => false;

  protected override void _Register()
  {
    base._Register();
    this._order.onItemDelivered += new Func<Farmer, NPC, Item, bool, int>(this.OnItemDelivered);
  }

  protected override void _Unregister()
  {
    base._Unregister();
    this._order.onItemDelivered -= new Func<Farmer, NPC, Item, bool, int>(this.OnItemDelivered);
  }

  public virtual int OnItemDelivered(Farmer farmer, NPC npc, Item item, bool probe)
  {
    if (this.IsComplete() || npc.Name != this.targetName.Value)
      return 0;
    bool flag1 = true;
    foreach (string acceptableContextTagSet in (NetList<string, NetString>) this.acceptableContextTagSets)
    {
      flag1 = false;
      bool flag2 = false;
      foreach (string str in acceptableContextTagSet.Split(','))
      {
        if (!ItemContextTagManager.DoAnyTagsMatch((IList<string>) str.Split('/'), item.GetContextTags()))
        {
          flag2 = true;
          break;
        }
      }
      if (!flag2)
      {
        flag1 = true;
        break;
      }
    }
    if (!flag1)
      return 0;
    int val2 = this.GetMaxCount() - this.GetCount();
    int amount = Math.Min(item.Stack, val2);
    if (amount < val2)
      return 0;
    if (!probe)
    {
      Item one = item.getOne();
      one.Stack = amount;
      this._order.donatedItems.Add(one);
      item.Stack -= amount;
      this.IncrementCount(amount);
      if (!string.IsNullOrEmpty(this.message.Value))
      {
        npc.CurrentDialogue.Push(new Dialogue(npc, (string) null, this.message.Value));
        Game1.drawDialogue(npc);
      }
    }
    return amount;
  }
}
