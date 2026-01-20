// Decompiled with JetBrains decompiler
// Type: Netcode.NetObjectList`1
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using System.Collections.Generic;

#nullable disable
namespace Netcode;

public class NetObjectList<T> : NetList<T, NetRef<T>> where T : class, INetObject<INetSerializable>
{
  public NetObjectList()
  {
  }

  public NetObjectList(IEnumerable<T> values)
    : base(values)
  {
  }

  public NetObjectList(int capacity)
    : base(capacity)
  {
  }
}
