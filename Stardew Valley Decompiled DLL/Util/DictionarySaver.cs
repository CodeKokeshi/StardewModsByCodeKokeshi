// Decompiled with JetBrains decompiler
// Type: StardewValley.Util.SaveablePair`2
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using System.Xml.Serialization;

#nullable disable
namespace StardewValley.Util;

/// <summary>Stores the key/value pairs of a dictionary in an easily serializable way.</summary>
/// <summary>Constructs a key/value pair entry.</summary>
/// <param name="key">The dictionary entry key.</param>
/// <param name="value">The dictionary entry value.</param>
public struct SaveablePair<TKey, TValue>(TKey key, TValue value)
{
  /// <summary>An 1-length array that stores the dictionary entry key.</summary>
  public TKey[] key = new TKey[1]{ key };
  /// <summary>An 1-length array that stores the dictionary entry value.</summary>
  public TValue[] value = new TValue[1]{ value };

  [XmlIgnore]
  public TKey Key => this.key[0];

  [XmlIgnore]
  public TValue Value => this.value[0];
}
