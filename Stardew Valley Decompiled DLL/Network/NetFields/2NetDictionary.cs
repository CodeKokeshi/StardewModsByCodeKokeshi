// Decompiled with JetBrains decompiler
// Type: Netcode.NetGuidDictionary`2
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using System;
using System.Collections.Generic;
using System.IO;

#nullable disable
namespace Netcode;

public class NetGuidDictionary<T, TField> : 
  NetFieldDictionary<Guid, T, TField, Dictionary<Guid, T>, NetGuidDictionary<T, TField>>
  where TField : NetField<T, TField>, new()
{
  public NetGuidDictionary()
  {
  }

  public NetGuidDictionary(IEnumerable<KeyValuePair<Guid, T>> pairs)
    : base(pairs)
  {
  }

  protected override Guid ReadKey(BinaryReader reader) => reader.ReadGuid();

  protected override void WriteKey(BinaryWriter writer, Guid key) => writer.WriteGuid(key);
}
