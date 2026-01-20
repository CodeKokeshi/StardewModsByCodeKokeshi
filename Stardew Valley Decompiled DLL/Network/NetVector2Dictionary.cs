// Decompiled with JetBrains decompiler
// Type: StardewValley.Network.NetVector2Dictionary`2
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Netcode;
using System.Collections.Generic;
using System.IO;

#nullable disable
namespace StardewValley.Network;

public sealed class NetVector2Dictionary<T, TField> : 
  NetFieldDictionary<Vector2, T, TField, SerializableDictionary<Vector2, T>, NetVector2Dictionary<T, TField>>
  where TField : NetField<T, TField>, new()
{
  public NetVector2Dictionary()
  {
  }

  public NetVector2Dictionary(IEnumerable<KeyValuePair<Vector2, T>> dict)
    : base(dict)
  {
  }

  protected override Vector2 ReadKey(BinaryReader reader)
  {
    return new Vector2(reader.ReadSingle(), reader.ReadSingle());
  }

  protected override void WriteKey(BinaryWriter writer, Vector2 key)
  {
    writer.Write(key.X);
    writer.Write(key.Y);
  }
}
