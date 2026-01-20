// Decompiled with JetBrains decompiler
// Type: StardewValley.SpecialOrders.Objectives.ShipObjective
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Netcode;
using StardewValley.Extensions;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley.SpecialOrders.Objectives;

public class ShipObjective : OrderObjective
{
  [XmlElement("acceptableContextTagSets")]
  public NetStringList acceptableContextTagSets = new NetStringList();
  [XmlElement("useShipmentValue")]
  public NetBool useShipmentValue = new NetBool();

  public override void Load(SpecialOrder order, Dictionary<string, string> data)
  {
    string data1;
    if (data.TryGetValue("AcceptedContextTags", out data1))
      this.acceptableContextTagSets.Add(order.Parse(data1));
    if (!data.TryGetValue("UseShipmentValue", out data1) || !data1.Trim().EqualsIgnoreCase("true"))
      return;
    this.useShipmentValue.Value = true;
  }

  public override void InitializeNetFields()
  {
    base.InitializeNetFields();
    this.NetFields.AddField((INetSerializable) this.acceptableContextTagSets, "acceptableContextTagSets").AddField((INetSerializable) this.useShipmentValue, "useShipmentValue");
  }

  protected override void _Register()
  {
    base._Register();
    this._order.onItemShipped += new Action<Farmer, Item, int>(this.OnItemShipped);
  }

  protected override void _Unregister()
  {
    base._Unregister();
    this._order.onItemShipped -= new Action<Farmer, Item, int>(this.OnItemShipped);
  }

  public virtual void OnItemShipped(Farmer farmer, Item item, int shipped_price)
  {
    foreach (string acceptableContextTagSet in (NetList<string, NetString>) this.acceptableContextTagSets)
    {
      bool flag = false;
      foreach (string str in acceptableContextTagSet.Split(','))
      {
        if (!ItemContextTagManager.DoAnyTagsMatch((IList<string>) str.Split('/'), item.GetContextTags()))
          flag = true;
      }
      if (!flag)
      {
        if (this.useShipmentValue.Value)
        {
          this.IncrementCount(shipped_price);
          break;
        }
        this.IncrementCount(item.Stack);
        break;
      }
    }
  }
}
