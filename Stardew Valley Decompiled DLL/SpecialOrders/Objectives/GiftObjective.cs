// Decompiled with JetBrains decompiler
// Type: StardewValley.SpecialOrders.Objectives.GiftObjective
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

public class GiftObjective : OrderObjective
{
  [XmlElement("acceptableContextTagSets")]
  public NetStringList acceptableContextTagSets = new NetStringList();
  [XmlElement("minimumLikeLevel")]
  public NetEnum<GiftObjective.LikeLevels> minimumLikeLevel = new NetEnum<GiftObjective.LikeLevels>(GiftObjective.LikeLevels.None);

  public override void Load(SpecialOrder order, Dictionary<string, string> data)
  {
    string data1;
    if (data.TryGetValue("AcceptedContextTags", out data1))
      this.acceptableContextTagSets.Add(order.Parse(data1));
    if (!data.TryGetValue("MinimumLikeLevel", out data1))
      return;
    this.minimumLikeLevel.Value = (GiftObjective.LikeLevels) Enum.Parse(typeof (GiftObjective.LikeLevels), data1);
  }

  public override void InitializeNetFields()
  {
    base.InitializeNetFields();
    this.NetFields.AddField((INetSerializable) this.acceptableContextTagSets, "acceptableContextTagSets").AddField((INetSerializable) this.minimumLikeLevel, "minimumLikeLevel");
  }

  protected override void _Register()
  {
    base._Register();
    this._order.onGiftGiven += new Action<Farmer, NPC, Item>(this.OnGiftGiven);
  }

  protected override void _Unregister()
  {
    base._Unregister();
    this._order.onGiftGiven -= new Action<Farmer, NPC, Item>(this.OnGiftGiven);
  }

  public virtual void OnGiftGiven(Farmer farmer, NPC npc, Item item)
  {
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
      return;
    if (this.minimumLikeLevel.Value > GiftObjective.LikeLevels.None)
    {
      int tasteForThisItem = npc.getGiftTasteForThisItem(item);
      GiftObjective.LikeLevels likeLevels = GiftObjective.LikeLevels.None;
      switch (tasteForThisItem)
      {
        case 0:
          likeLevels = GiftObjective.LikeLevels.Loved;
          break;
        case 2:
          likeLevels = GiftObjective.LikeLevels.Liked;
          break;
        case 4:
          likeLevels = GiftObjective.LikeLevels.Disliked;
          break;
        case 6:
          likeLevels = GiftObjective.LikeLevels.Hated;
          break;
        case 8:
          likeLevels = GiftObjective.LikeLevels.Neutral;
          break;
      }
      if (likeLevels < this.minimumLikeLevel.Value)
        return;
    }
    this.IncrementCount(1);
  }

  public enum LikeLevels
  {
    None,
    Hated,
    Disliked,
    Neutral,
    Liked,
    Loved,
  }
}
