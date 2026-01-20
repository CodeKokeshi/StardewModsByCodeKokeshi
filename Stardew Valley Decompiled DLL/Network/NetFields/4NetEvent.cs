// Decompiled with JetBrains decompiler
// Type: Netcode.NetEvent1`1
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using System.IO;

#nullable disable
namespace Netcode;

public class NetEvent1<T> : AbstractNetEvent1<T> where T : NetEventArg, new()
{
  protected override T readEventArg(BinaryReader reader, NetVersion version)
  {
    T obj = new T();
    obj.Read(reader);
    return obj;
  }

  protected override void writeEventArg(BinaryWriter writer, T eventArg) => eventArg.Write(writer);
}
