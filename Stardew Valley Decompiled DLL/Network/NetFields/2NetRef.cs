// Decompiled with JetBrains decompiler
// Type: Netcode.NetRef`1
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

#nullable disable
namespace Netcode;

public class NetRef<T> : NetExtendableRef<T, NetRef<T>> where T : class, INetObject<INetSerializable>
{
  public NetRef()
  {
  }

  public NetRef(T value)
    : base(value)
  {
  }
}
