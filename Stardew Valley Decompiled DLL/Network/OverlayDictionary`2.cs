// Decompiled with JetBrains decompiler
// Type: StardewValley.Network.OverlayDictionary`2
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using System;
using System.Collections;
using System.Collections.Generic;

#nullable disable
namespace StardewValley.Network;

public class OverlayDictionary<TKey, TValue> : 
  IDictionary<TKey, TValue>,
  ICollection<KeyValuePair<TKey, TValue>>,
  IEnumerable<KeyValuePair<TKey, TValue>>,
  IEnumerable
{
  protected Dictionary<TKey, TValue> _dictionary;
  protected List<KeyValuePair<TKey, TValue>> _removedPairs = new List<KeyValuePair<TKey, TValue>>();

  public event Action<TKey, TValue> onValueAdded;

  public event Action<TKey, TValue> onValueRemoved;

  public OverlayDictionary() => this._dictionary = new Dictionary<TKey, TValue>();

  public OverlayDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer)
  {
    this._dictionary = new Dictionary<TKey, TValue>(dictionary, comparer);
  }

  public OverlayDictionary(IEqualityComparer<TKey> comparer)
  {
    this._dictionary = new Dictionary<TKey, TValue>(comparer);
  }

  public TValue this[TKey key]
  {
    get => this._dictionary[key];
    set
    {
      this._dictionary[key] = value;
      Action<TKey, TValue> onValueAdded = this.onValueAdded;
      if (onValueAdded == null)
        return;
      onValueAdded(key, value);
    }
  }

  public ICollection<TKey> Keys => (ICollection<TKey>) this._dictionary.Keys;

  public ICollection<TValue> Values => (ICollection<TValue>) this._dictionary.Values;

  public int Count => this._dictionary.Count;

  public bool IsReadOnly => ((ICollection<KeyValuePair<TKey, TValue>>) this._dictionary).IsReadOnly;

  public void Add(TKey key, TValue value)
  {
    this._dictionary.Add(key, value);
    Action<TKey, TValue> onValueAdded = this.onValueAdded;
    if (onValueAdded == null)
      return;
    onValueAdded(key, value);
  }

  public void Add(KeyValuePair<TKey, TValue> item) => this.Add(item.Key, item.Value);

  public void Clear()
  {
    this._removedPairs.AddRange((IEnumerable<KeyValuePair<TKey, TValue>>) this._dictionary);
    this._dictionary.Clear();
    foreach (KeyValuePair<TKey, TValue> removedPair in this._removedPairs)
      this.onValueRemoved(removedPair.Key, removedPair.Value);
    this._removedPairs.Clear();
  }

  public bool Contains(KeyValuePair<TKey, TValue> item)
  {
    return ((ICollection<KeyValuePair<TKey, TValue>>) this._dictionary).Contains(item);
  }

  public bool ContainsKey(TKey key) => this._dictionary.ContainsKey(key);

  public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
  {
    ((ICollection<KeyValuePair<TKey, TValue>>) this._dictionary).CopyTo(array, arrayIndex);
  }

  public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
  {
    return (IEnumerator<KeyValuePair<TKey, TValue>>) this._dictionary.GetEnumerator();
  }

  public bool Remove(TKey key)
  {
    TValue obj;
    if (!this._dictionary.TryGetValue(key, out obj))
      return false;
    this._dictionary.Remove(key);
    Action<TKey, TValue> onValueRemoved = this.onValueRemoved;
    if (onValueRemoved != null)
      onValueRemoved(key, obj);
    return true;
  }

  public bool Remove(KeyValuePair<TKey, TValue> item)
  {
    return this.Contains(item) && this.Remove(item.Key);
  }

  /// <summary>Get the value with a given key if it exists.</summary>
  /// <param name="key">The key to check.</param>
  /// <param name="value">The value that was found, if applicable.</param>
  /// <returns>Returns whether the value was found.</returns>
  public bool TryGetValue(TKey key, out TValue value)
  {
    return this._dictionary.TryGetValue(key, out value);
  }

  /// <summary>Get the value with a given key if it exists, else a default value.</summary>
  /// <param name="key">The key to check.</param>
  /// <param name="defaultValue">The value to return if the element isn't found.</param>
  public TValue GetValueOrDefault(TKey key, TValue defaultValue = null)
  {
    return this._dictionary.GetValueOrDefault<TKey, TValue>(key, defaultValue);
  }

  IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this._dictionary.GetEnumerator();
}
