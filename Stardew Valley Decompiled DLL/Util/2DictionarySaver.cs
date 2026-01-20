// Decompiled with JetBrains decompiler
// Type: StardewValley.Util.DictionarySaver`2
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using System;
using System.Collections.Generic;

#nullable disable
namespace StardewValley.Util;

public static class DictionarySaver<TKey, TValue>
{
  /// <summary>Create an array of <see cref="T:StardewValley.Util.SaveablePair`2" /> from a dictionary.</summary>
  /// <param name="data">The data to copy.</param>
  public static SaveablePair<TKey, TValue>[] ArrayFrom(IDictionary<TKey, TValue> data)
  {
    SaveablePair<TKey, TValue>[] saveablePairArray = new SaveablePair<TKey, TValue>[data != null ? data.Count : 0];
    int num = 0;
    if (data != null)
    {
      foreach (KeyValuePair<TKey, TValue> keyValuePair in (IEnumerable<KeyValuePair<TKey, TValue>>) data)
        saveablePairArray[num++] = new SaveablePair<TKey, TValue>(keyValuePair.Key, keyValuePair.Value);
    }
    return saveablePairArray;
  }

  /// <summary>Create an array of <see cref="T:StardewValley.Util.SaveablePairExtensions" /> from a dictionary with a different value type.</summary>
  /// <typeparam name="TSourceValue">The value type in the source data to copy.</typeparam>
  /// <param name="data">The data to copy.</param>
  /// <param name="getValue">Get the value to use for an entry in the original data.</param>
  public static SaveablePair<TKey, TValue>[] ArrayFrom<TSourceValue>(
    IDictionary<TKey, TSourceValue> data,
    Func<TSourceValue, TValue> getValue)
  {
    SaveablePair<TKey, TValue>[] saveablePairArray = new SaveablePair<TKey, TValue>[data != null ? data.Count : 0];
    int num = 0;
    if (data != null)
    {
      foreach (KeyValuePair<TKey, TSourceValue> keyValuePair in (IEnumerable<KeyValuePair<TKey, TSourceValue>>) data)
        saveablePairArray[num++] = new SaveablePair<TKey, TValue>(keyValuePair.Key, getValue(keyValuePair.Value));
    }
    return saveablePairArray;
  }

  /// <summary>Create an array of <see cref="T:StardewValley.Util.SaveablePair`2" /> from a dictionary with different key and value types.</summary>
  /// <typeparam name="TSourceKey">The key type in the source data to copy.</typeparam>
  /// <typeparam name="TSourceValue">The value type in the source data to copy.</typeparam>
  /// <param name="data">The data to copy.</param>
  /// <param name="getKey">Get the key to use for an entry in the original data.</param>
  /// <param name="getValue">Get the value to use for an entry in the original data.</param>
  public static SaveablePair<TKey, TValue>[] ArrayFrom<TSourceKey, TSourceValue>(
    IDictionary<TSourceKey, TSourceValue> data,
    Func<TSourceKey, TKey> getKey,
    Func<TSourceValue, TValue> getValue)
  {
    SaveablePair<TKey, TValue>[] saveablePairArray = new SaveablePair<TKey, TValue>[data != null ? data.Count : 0];
    int num = 0;
    if (data != null)
    {
      foreach (KeyValuePair<TSourceKey, TSourceValue> keyValuePair in (IEnumerable<KeyValuePair<TSourceKey, TSourceValue>>) data)
        saveablePairArray[num++] = new SaveablePair<TKey, TValue>(getKey(keyValuePair.Key), getValue(keyValuePair.Value));
    }
    return saveablePairArray;
  }
}
