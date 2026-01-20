// Decompiled with JetBrains decompiler
// Type: StardewValley.NetIntIntArrayDictionary
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Netcode;
using System.Collections.Generic;
using System.IO;
using System.Linq;

#nullable disable
namespace StardewValley;

public class NetIntIntArrayDictionary : 
  NetDictionary<int, int[], NetArray<int, NetInt>, SerializableDictionary<int, int[]>, NetIntIntArrayDictionary>
{
  protected override int ReadKey(BinaryReader reader) => reader.ReadInt32();

  protected override void WriteKey(BinaryWriter writer, int key) => writer.Write(key);

  protected override void setFieldValue(NetArray<int, NetInt> field, int key, int[] value)
  {
    field.Set((IList<int>) value);
  }

  protected override int[] getFieldValue(NetArray<int, NetInt> field) => field.ToArray<int>();

  protected override int[] getFieldTargetValue(NetArray<int, NetInt> field) => field.ToArray<int>();
}
