// Decompiled with JetBrains decompiler
// Type: StardewValley.Network.NetPointDictionary`2
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

public class NetPointDictionary<T, TField> : 
  NetFieldDictionary<Point, T, TField, SerializableDictionary<Point, T>, NetPointDictionary<T, TField>>
  where TField : NetField<T, TField>, new()
{
  public NetPointDictionary()
  {
  }

  public NetPointDictionary(IEnumerable<KeyValuePair<Point, T>> dict)
    : base(dict)
  {
  }

  protected override Point ReadKey(BinaryReader reader)
  {
    return new Point(reader.ReadInt32(), reader.ReadInt32());
  }

  protected override void WriteKey(BinaryWriter writer, Point key)
  {
    writer.Write(key.X);
    writer.Write(key.Y);
  }
}
