// Decompiled with JetBrains decompiler
// Type: StardewValley.Extensions.CollectionExtensions
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Netcode;
using System;
using System.Collections.Generic;

#nullable disable
namespace StardewValley.Extensions;

/// <summary>Provides utility extension methods on .NET collection types.</summary>
public static class CollectionExtensions
{
  /// <summary>Remove all elements that match a condition.</summary>
  /// <typeparam name="TKey">The dictionary key type.</typeparam>
  /// <typeparam name="TValue">The dictionary value type.</typeparam>
  /// <param name="dictionary">The dictionary to update.</param>
  /// <param name="match">The predicate matching values to remove.</param>
  /// <returns>Returns the number of entries removed.</returns>
  public static int RemoveWhere<TKey, TValue>(
    this IDictionary<TKey, TValue> dictionary,
    Func<KeyValuePair<TKey, TValue>, bool> match)
  {
    if (dictionary.Count == 0)
      return 0;
    int num = 0;
    foreach (KeyValuePair<TKey, TValue> keyValuePair in (IEnumerable<KeyValuePair<TKey, TValue>>) dictionary)
    {
      if (match(keyValuePair))
      {
        dictionary.Remove(keyValuePair.Key);
        ++num;
      }
    }
    return num;
  }

  /// <summary>Add multiple key/value pairs to the dictionary, skipping keys which already exist in the dictionary.</summary>
  /// <typeparam name="TKey">The dictionary key type.</typeparam>
  /// <typeparam name="TValue">The dictionary value type.</typeparam>
  /// <param name="dict">The dictionary to update.</param>
  /// <param name="values">The key/value pairs to add.</param>
  /// <returns>Returns the number of pairs added to the dictionary.</returns>
  public static int TryAddMany<TKey, TValue>(
    this IDictionary<TKey, TValue> dict,
    Dictionary<TKey, TValue> values)
  {
    if (values == null)
      return 0;
    int num = 0;
    foreach (KeyValuePair<TKey, TValue> keyValuePair in values)
    {
      if (dict.TryAdd<TKey, TValue>(keyValuePair.Key, keyValuePair.Value))
        ++num;
    }
    return num;
  }

  /// <summary>Remove all elements that match a condition.</summary>
  /// <typeparam name="T">The set item type.</typeparam>
  /// <param name="list">The list to update.</param>
  /// <param name="match">The predicate matching values to remove.</param>
  /// <returns>Returns the number of values removed from the list.</returns>
  public static int RemoveWhere<T>(this IList<T> list, Predicate<T> match)
  {
    if (list is List<T> objList)
      return objList.RemoveAll(match);
    int num = 0;
    for (int index = list.Count - 1; index >= 0; --index)
    {
      if (match(list[index]))
      {
        list.RemoveAt(index);
        ++num;
      }
    }
    return num;
  }

  /// <summary>Add or remove value to the set.</summary>
  /// <typeparam name="T">The set item type.</typeparam>
  /// <param name="set">The set to update.</param>
  /// <param name="value">The value to add or remove.</param>
  /// <param name="add">Whether to add the value; else it's removed.</param>
  public static void Toggle<T>(this ISet<T> set, T value, bool add)
  {
    if (add)
      set.Add(value);
    else
      set.Remove(value);
  }

  /// <summary>Add a list of values to the set.</summary>
  /// <typeparam name="T">The set item type.</typeparam>
  /// <param name="set">The set to update.</param>
  /// <param name="values">The values to add to the set.</param>
  /// <returns>Returns the number of values added to the set.</returns>
  public static int AddRange<T>(this ISet<T> set, IEnumerable<T> values)
  {
    if (values == null)
      return 0;
    int num = 0;
    foreach (T obj in values)
    {
      if (set.Add(obj))
        ++num;
    }
    return num;
  }

  /// <summary>Remove all elements that match a condition.</summary>
  /// <typeparam name="T">The set item type.</typeparam>
  /// <param name="set">The set to update.</param>
  /// <param name="match">The predicate matching values to remove.</param>
  /// <returns>Returns the number of values removed from the set.</returns>
  public static int RemoveWhere<T>(this ISet<T> set, Predicate<T> match)
  {
    switch (set)
    {
      case HashSet<T> objSet:
        return objSet.RemoveWhere(match);
      case NetHashSet<T> netHashSet:
        return netHashSet.RemoveWhere(match);
      default:
        List<T> objList = (List<T>) null;
        foreach (T obj in (IEnumerable<T>) set)
        {
          if (match(obj))
          {
            if (objList == null)
              objList = new List<T>();
            objList.Add(obj);
          }
        }
        if (objList == null)
          return 0;
        foreach (T obj in objList)
          set.Remove(obj);
        return objList.Count;
    }
  }
}
