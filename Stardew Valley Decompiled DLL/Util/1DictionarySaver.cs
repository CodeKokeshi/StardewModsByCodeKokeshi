// Decompiled with JetBrains decompiler
// Type: StardewValley.Util.SaveablePairExtensions
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using System.Collections.Generic;

#nullable disable
namespace StardewValley.Util;

/// <summary>Provides utility extension methods for converting between <see cref="T:StardewValley.Util.SaveablePair`2" /> arrays and dictionaries.</summary>
public static class SaveablePairExtensions
{
  /// <summary>Create a dictionary from an array of <see cref="T:StardewValley.Util.SaveablePair`2" />.</summary>
  /// <param name="pairs">The array of pairs.</param>
  public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(
    this SaveablePair<TKey, TValue>[] pairs)
  {
    Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();
    if (pairs != null)
    {
      foreach (SaveablePair<TKey, TValue> pair in pairs)
        dictionary[pair.Key] = pair.Value;
    }
    return dictionary;
  }

  /// <summary>Create an array of <see cref="T:StardewValley.Util.SaveablePair`2" /> from a dictionary.</summary>
  /// <param name="data">The data to copy.</param>
  public static SaveablePair<TKey, TValue>[] ToSaveableArray<TKey, TValue>(
    this IDictionary<TKey, TValue> data)
  {
    return DictionarySaver<TKey, TValue>.ArrayFrom(data);
  }
}
