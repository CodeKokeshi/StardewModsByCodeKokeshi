// Decompiled with JetBrains decompiler
// Type: StardewValley.Network.NetStringDictionary`2
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Netcode;
using System.Collections.Generic;
using System.IO;

#nullable disable
namespace StardewValley.Network;

public class NetStringDictionary<T, TField> : 
  NetFieldDictionary<string, T, TField, SerializableDictionary<string, T>, NetStringDictionary<T, TField>>
  where TField : NetField<T, TField>, new()
{
  public NetStringDictionary()
  {
  }

  public NetStringDictionary(IEnumerable<KeyValuePair<string, T>> dict)
    : base(dict)
  {
  }

  protected override string ReadKey(BinaryReader reader) => reader.ReadString();

  protected override void WriteKey(BinaryWriter writer, string key) => writer.Write(key);
}
