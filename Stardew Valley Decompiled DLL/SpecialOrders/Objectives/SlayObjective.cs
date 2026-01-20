// Decompiled with JetBrains decompiler
// Type: StardewValley.SpecialOrders.Objectives.SlayObjective
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Netcode;
using StardewValley.Monsters;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley.SpecialOrders.Objectives;

public class SlayObjective : OrderObjective
{
  [XmlElement("targetNames")]
  public NetStringList targetNames = new NetStringList();
  /// <summary>Whether to ignore monsters killed on the farm.</summary>
  [XmlElement("ignoreFarmMonsters")]
  public NetBool ignoreFarmMonsters = new NetBool(true);

  public override void InitializeNetFields()
  {
    base.InitializeNetFields();
    this.NetFields.AddField((INetSerializable) this.targetNames, "targetNames").AddField((INetSerializable) this.ignoreFarmMonsters, "ignoreFarmMonsters");
  }

  public override void Load(SpecialOrder order, Dictionary<string, string> data)
  {
    base.Load(order, data);
    string data1;
    if (data.TryGetValue("TargetName", out data1))
    {
      foreach (string str in order.Parse(data1).Split(','))
        this.targetNames.Add(str.Trim());
    }
    string str1;
    if (!data.TryGetValue("IgnoreFarmMonsters", out str1))
      return;
    bool result;
    if (bool.TryParse(str1, out result))
      this.ignoreFarmMonsters.Value = result;
    else
      Game1.log.Warn($"Special order slay objective can't parse IgnoreFarmMonsters value '{str1}' as a boolean.");
  }

  protected override void _Register()
  {
    base._Register();
    this._order.onMonsterSlain += new Action<Farmer, Monster>(this.OnMonsterSlain);
  }

  protected override void _Unregister()
  {
    base._Unregister();
    this._order.onMonsterSlain -= new Action<Farmer, Monster>(this.OnMonsterSlain);
  }

  public virtual void OnMonsterSlain(Farmer farmer, Monster monster)
  {
    if (this.ignoreFarmMonsters.Value && monster.currentLocation?.Name == "Farm")
      return;
    foreach (string targetName in (NetList<string, NetString>) this.targetNames)
    {
      if (monster.Name.Contains(targetName))
      {
        this.IncrementCount(1);
        break;
      }
    }
  }
}
