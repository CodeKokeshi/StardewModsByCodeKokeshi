// Decompiled with JetBrains decompiler
// Type: StardewValley.Network.NetIntDictionary`2
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Netcode;
using System.Collections.Generic;
using System.IO;

#nullable disable
namespace StardewValley.Network;

public class NetIntDictionary<T, TField> : 
  NetFieldDictionary<int, T, TField, SerializableDictionary<int, T>, NetIntDictionary<T, TField>>
  where TField : NetField<T, TField>, new()
{
  public NetIntDictionary()
  {
  }

  public NetIntDictionary(IEnumerable<KeyValuePair<int, T>> dict)
    : base(dict)
  {
  }

  protected override int ReadKey(BinaryReader reader) => reader.ReadInt32();

  protected override void WriteKey(BinaryWriter writer, int key) => writer.Write(key);
}
