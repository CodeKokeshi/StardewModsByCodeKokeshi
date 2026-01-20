// Decompiled with JetBrains decompiler
// Type: Netcode.NetLongList
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using System.Collections.Generic;

#nullable disable
namespace Netcode;

public sealed class NetLongList : NetList<long, NetLong>
{
  public NetLongList()
  {
  }

  public NetLongList(IEnumerable<long> values)
    : base(values)
  {
  }

  public NetLongList(int capacity)
    : base(capacity)
  {
  }

  public override bool Contains(long item)
  {
    foreach (long num in (NetList<long, NetLong>) this)
    {
      if (num == item)
        return true;
    }
    return false;
  }

  public override int IndexOf(long item)
  {
    NetInt count = this.count;
    for (int index = 0; index < count.Value; ++index)
    {
      if (this.array.Value[index] == item)
        return index;
    }
    return -1;
  }
}
