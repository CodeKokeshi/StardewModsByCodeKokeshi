// Decompiled with JetBrains decompiler
// Type: StardewValley.NetFarmerPairDictionary`2
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Netcode;
using System.Collections.Generic;
using System.IO;

#nullable disable
namespace StardewValley;

public class NetFarmerPairDictionary<T, TField> : 
  NetFieldDictionary<FarmerPair, T, TField, SerializableDictionary<FarmerPair, T>, NetFarmerPairDictionary<T, TField>>
  where TField : NetField<T, TField>, new()
{
  public NetFarmerPairDictionary()
  {
  }

  public NetFarmerPairDictionary(IEnumerable<KeyValuePair<FarmerPair, T>> dict)
    : base(dict)
  {
  }

  protected override FarmerPair ReadKey(BinaryReader reader)
  {
    return FarmerPair.MakePair(reader.ReadInt64(), reader.ReadInt64());
  }

  protected override void WriteKey(BinaryWriter writer, FarmerPair key)
  {
    writer.Write(key.Farmer1);
    writer.Write(key.Farmer2);
  }
}
