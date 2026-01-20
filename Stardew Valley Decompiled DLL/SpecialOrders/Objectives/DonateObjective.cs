// Decompiled with JetBrains decompiler
// Type: StardewValley.SpecialOrders.Objectives.DonateObjective
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Netcode;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley.SpecialOrders.Objectives;

public class DonateObjective : OrderObjective
{
  [XmlElement("dropBox")]
  public NetString dropBox = new NetString();
  [XmlElement("dropBoxGameLocation")]
  public NetString dropBoxGameLocation = new NetString();
  [XmlElement("dropBoxTileLocation")]
  public NetVector2 dropBoxTileLocation = new NetVector2();
  [XmlElement("acceptableContextTagSets")]
  public NetStringList acceptableContextTagSets = new NetStringList();
  [XmlElement("minimumCapacity")]
  public NetInt minimumCapacity = new NetInt(-1);
  [XmlElement("confirmed")]
  public NetBool confirmed = new NetBool(false);

  public virtual string GetDropboxLocationName()
  {
    return this.dropBoxGameLocation.Value == "Trailer" && Game1.MasterPlayer.hasOrWillReceiveMail("pamHouseUpgrade") ? "Trailer_Big" : this.dropBoxGameLocation.Value;
  }

  public override void Load(SpecialOrder order, Dictionary<string, string> data)
  {
    string data1;
    if (data.TryGetValue("AcceptedContextTags", out data1))
      this.acceptableContextTagSets.Add(order.Parse(data1.Trim()));
    if (data.TryGetValue("DropBox", out data1))
      this.dropBox.Value = order.Parse(data1.Trim());
    if (data.TryGetValue("DropBoxGameLocation", out data1))
      this.dropBoxGameLocation.Value = order.Parse(data1.Trim());
    if (data.TryGetValue("DropBoxIndicatorLocation", out data1))
    {
      string[] strArray = ArgUtility.SplitBySpace(order.Parse(data1));
      this.dropBoxTileLocation.Value = new Vector2((float) Convert.ToDouble(strArray[0]), (float) Convert.ToDouble(strArray[1]));
    }
    if (!data.TryGetValue("MinimumCapacity", out data1))
      return;
    this.minimumCapacity.Value = int.Parse(order.Parse(data1));
  }

  public int GetAcceptCount(Item item, int stack_count)
  {
    return this.IsValidItem(item) ? Math.Min(this.GetMaxCount() - this.GetCount(), stack_count) : 0;
  }

  public override void OnCompletion()
  {
    base.OnCompletion();
    if (string.IsNullOrEmpty(this.dropBoxGameLocation.Value))
      return;
    GameLocation locationFromName = Game1.getLocationFromName(this.GetDropboxLocationName());
    if (locationFromName == null)
      return;
    locationFromName.showDropboxIndicator = false;
  }

  public override bool CanComplete() => this.confirmed.Value;

  public virtual void Confirm()
  {
    if (this.GetCount() >= this.GetMaxCount())
      this.confirmed.Value = true;
    else
      this.confirmed.Value = false;
  }

  public override bool CanUncomplete() => true;

  public override void InitializeNetFields()
  {
    base.InitializeNetFields();
    this.NetFields.AddField((INetSerializable) this.acceptableContextTagSets, "acceptableContextTagSets").AddField((INetSerializable) this.dropBox, "dropBox").AddField((INetSerializable) this.dropBoxGameLocation, "dropBoxGameLocation").AddField((INetSerializable) this.dropBoxTileLocation, "dropBoxTileLocation").AddField((INetSerializable) this.minimumCapacity, "minimumCapacity").AddField((INetSerializable) this.confirmed, "confirmed");
    this.confirmed.fieldChangeVisibleEvent += new FieldChange<NetBool, bool>(this.OnConfirmed);
  }

  protected void OnConfirmed(NetBool field, bool oldValue, bool newValue)
  {
    if (Utility.ShouldIgnoreValueChangeCallback())
      return;
    this.CheckCompletion();
  }

  public virtual bool IsValidItem(Item item)
  {
    if (item == null)
      return false;
    foreach (string acceptableContextTagSet in (NetList<string, NetString>) this.acceptableContextTagSets)
    {
      bool flag = false;
      foreach (string str in acceptableContextTagSet.Split(','))
      {
        if (str.StartsWith("color") && item is ColoredObject coloredObject && coloredObject.preservedParentSheetIndex.Value != null)
        {
          if (ItemContextTagManager.DoAnyTagsMatch((IList<string>) str.Split('/'), ItemContextTagManager.GetBaseContextTags(coloredObject.preservedParentSheetIndex.Value)))
            return true;
          flag = true;
          break;
        }
        if (!ItemContextTagManager.DoAnyTagsMatch((IList<string>) str.Split('/'), item.GetContextTags()))
        {
          flag = true;
          break;
        }
      }
      if (!flag)
        return true;
    }
    return false;
  }
}
