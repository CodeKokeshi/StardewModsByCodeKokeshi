// Decompiled with JetBrains decompiler
// Type: StardewValley.SpecialOrders.Objectives.FishObjective
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

public class FishObjective : OrderObjective
{
  [XmlElement("acceptableContextTagSets")]
  public NetStringList acceptableContextTagSets = new NetStringList();

  public override void InitializeNetFields()
  {
    base.InitializeNetFields();
    this.NetFields.AddField((INetSerializable) this.acceptableContextTagSets, "acceptableContextTagSets");
  }

  public override void Load(SpecialOrder order, Dictionary<string, string> data)
  {
    string data1;
    if (!data.TryGetValue("AcceptedContextTags", out data1))
      return;
    this.acceptableContextTagSets.Add(order.Parse(data1));
  }

  protected override void _Register()
  {
    base._Register();
    this._order.onFishCaught += new Action<Farmer, Item>(this.OnFishCaught);
  }

  protected override void _Unregister()
  {
    base._Unregister();
    this._order.onFishCaught -= new Action<Farmer, Item>(this.OnFishCaught);
  }

  public virtual void OnFishCaught(Farmer farmer, Item fish_item)
  {
    foreach (string acceptableContextTagSet in (NetList<string, NetString>) this.acceptableContextTagSets)
    {
      bool flag = false;
      foreach (string str in acceptableContextTagSet.Split(','))
      {
        if (!ItemContextTagManager.DoAnyTagsMatch((IList<string>) str.Split('/'), fish_item.GetContextTags()))
        {
          flag = true;
          break;
        }
      }
      if (!flag)
      {
        this.IncrementCount(fish_item.Stack);
        break;
      }
    }
  }
}
