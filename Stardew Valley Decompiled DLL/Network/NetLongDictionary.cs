// Decompiled with JetBrains decompiler
// Type: StardewValley.Network.NetLongDictionary`2
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Netcode;
using System.Collections.Generic;
using System.IO;

#nullable disable
namespace StardewValley.Network;

public class NetLongDictionary<T, TField> : 
  NetFieldDictionary<long, T, TField, SerializableDictionary<long, T>, NetLongDictionary<T, TField>>
  where TField : NetField<T, TField>, new()
{
  public NetLongDictionary()
  {
  }

  public NetLongDictionary(IEnumerable<KeyValuePair<long, T>> dict)
    : base(dict)
  {
  }

  protected override long ReadKey(BinaryReader reader) => reader.ReadInt64();

  protected override void WriteKey(BinaryWriter writer, long key) => writer.Write(key);
}
