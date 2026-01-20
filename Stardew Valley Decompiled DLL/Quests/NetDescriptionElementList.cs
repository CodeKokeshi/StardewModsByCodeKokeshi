// Decompiled with JetBrains decompiler
// Type: StardewValley.Quests.NetDescriptionElementList
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Netcode;
using System;
using System.Collections.Generic;

#nullable disable
namespace StardewValley.Quests;

public class NetDescriptionElementList : NetList<DescriptionElement, NetDescriptionElementRef>
{
  public NetDescriptionElementList()
  {
  }

  public NetDescriptionElementList(IEnumerable<DescriptionElement> values)
    : base(values)
  {
  }

  public NetDescriptionElementList(int capacity)
    : base(capacity)
  {
  }

  public void Add(string key) => this.Add(new DescriptionElement(key, Array.Empty<object>()));
}
