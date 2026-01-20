// Decompiled with JetBrains decompiler
// Type: Netcode.NetIntList
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using System.Collections.Generic;

#nullable disable
namespace Netcode;

public sealed class NetIntList : NetList<int, NetInt>
{
  public NetIntList()
  {
  }

  public NetIntList(IEnumerable<int> values)
    : base(values)
  {
  }

  public NetIntList(int capacity)
    : base(capacity)
  {
  }

  public override bool Contains(int item)
  {
    foreach (int num in (NetList<int, NetInt>) this)
    {
      if (num == item)
        return true;
    }
    return false;
  }

  public override int IndexOf(int item)
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
