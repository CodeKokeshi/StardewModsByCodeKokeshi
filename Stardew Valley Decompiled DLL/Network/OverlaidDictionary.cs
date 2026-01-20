// Decompiled with JetBrains decompiler
// Type: StardewValley.Network.OverlaidDictionary
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Netcode;
using System;
using System.Collections;
using System.Collections.Generic;

#nullable disable
namespace StardewValley.Network;

/// <summary>A hybrid synchronized/unsynchronized dictionary where values either come from a synchronized NetDictionary, or from a locally maintained overlay dictionary.</summary>
public class OverlaidDictionary : IEnumerable<SerializableDictionary<Vector2, StardewValley.Object>>, IEnumerable
{
  private NetVector2Dictionary<StardewValley.Object, NetRef<StardewValley.Object>> baseDict;
  private OverlayDictionary<Vector2, StardewValley.Object> overlayDict;
  private Dictionary<Vector2, StardewValley.Object> compositeDict;
  private bool _locked;
  private Dictionary<Vector2, StardewValley.Object> _changes = new Dictionary<Vector2, StardewValley.Object>();

  /// <summary>The number of key/value pairs in the dictionary.</summary>
  public int Length => this.compositeDict.Count;

  public StardewValley.Object this[Vector2 key]
  {
    get
    {
      StardewValley.Object object1;
      if (this.overlayDict.TryGetValue(key, out object1))
        return object1;
      StardewValley.Object object2;
      if (!this._locked || !this._changes.TryGetValue(key, out object2))
        return this.baseDict[key];
      return object2 != null ? object2 : throw new KeyNotFoundException();
    }
    set
    {
      if (this._locked)
        this._changes[key] = value;
      else
        this.baseDict[key] = value;
    }
  }

  public void OnValueAdded(Vector2 key, StardewValley.Object value)
  {
    StardewValley.Object @object;
    if (this.overlayDict.TryGetValue(key, out @object))
    {
      this.compositeDict[key] = @object;
    }
    else
    {
      if (!this.baseDict.TryGetValue(key, out @object))
        return;
      this.compositeDict[key] = @object;
    }
  }

  public void OnValueRemoved(Vector2 key, StardewValley.Object value)
  {
    StardewValley.Object @object;
    if (this.overlayDict.TryGetValue(key, out @object))
      this.compositeDict[key] = @object;
    else if (this.baseDict.TryGetValue(key, out @object))
      this.compositeDict[key] = @object;
    else
      this.compositeDict.Remove(key);
  }

  public Dictionary<Vector2, StardewValley.Object>.KeyCollection Keys => this.compositeDict.Keys;

  public Dictionary<Vector2, StardewValley.Object>.ValueCollection Values
  {
    get => this.compositeDict.Values;
  }

  public IEnumerable<KeyValuePair<Vector2, StardewValley.Object>> Pairs
  {
    get => (IEnumerable<KeyValuePair<Vector2, StardewValley.Object>>) this.compositeDict;
  }

  public void SetEqualityComparer(
    IEqualityComparer<Vector2> comparer,
    ref NetVector2Dictionary<StardewValley.Object, NetRef<StardewValley.Object>> base_dict,
    ref OverlayDictionary<Vector2, StardewValley.Object> overlay_dict)
  {
    this.baseDict.SetEqualityComparer(comparer);
    this.overlayDict.onValueAdded -= new Action<Vector2, StardewValley.Object>(this.OnValueAdded);
    this.overlayDict.onValueRemoved -= new Action<Vector2, StardewValley.Object>(this.OnValueRemoved);
    this.overlayDict = new OverlayDictionary<Vector2, StardewValley.Object>((IDictionary<Vector2, StardewValley.Object>) this.overlayDict, comparer);
    this.compositeDict = new Dictionary<Vector2, StardewValley.Object>((IDictionary<Vector2, StardewValley.Object>) this.compositeDict, comparer);
    this.overlayDict.onValueAdded += new Action<Vector2, StardewValley.Object>(this.OnValueAdded);
    this.overlayDict.onValueRemoved += new Action<Vector2, StardewValley.Object>(this.OnValueRemoved);
    this.overlayDict.onValueAdded += new Action<Vector2, StardewValley.Object>(this.OnValueAdded);
    this.overlayDict.onValueRemoved += new Action<Vector2, StardewValley.Object>(this.OnValueRemoved);
    base_dict = this.baseDict;
    overlay_dict = this.overlayDict;
  }

  public OverlaidDictionary(
    NetVector2Dictionary<StardewValley.Object, NetRef<StardewValley.Object>> baseDict,
    OverlayDictionary<Vector2, StardewValley.Object> overlayDict)
  {
    this.baseDict = baseDict;
    this.overlayDict = overlayDict;
    this.compositeDict = new Dictionary<Vector2, StardewValley.Object>();
    foreach (KeyValuePair<Vector2, StardewValley.Object> keyValuePair in overlayDict)
      this.OnValueAdded(keyValuePair.Key, keyValuePair.Value);
    foreach (KeyValuePair<Vector2, StardewValley.Object> pair in baseDict.Pairs)
      this.OnValueAdded(pair.Key, pair.Value);
    baseDict.OnValueAdded += new NetDictionary<Vector2, StardewValley.Object, NetRef<StardewValley.Object>, SerializableDictionary<Vector2, StardewValley.Object>, NetVector2Dictionary<StardewValley.Object, NetRef<StardewValley.Object>>>.ContentsChangeEvent(this.OnValueAdded);
    baseDict.OnConflictResolve += (NetDictionary<Vector2, StardewValley.Object, NetRef<StardewValley.Object>, SerializableDictionary<Vector2, StardewValley.Object>, NetVector2Dictionary<StardewValley.Object, NetRef<StardewValley.Object>>>.ConflictResolveEvent) ((key, rejected, accepted) =>
    {
      this.OnValueRemoved(key, rejected.Value);
      this.OnValueAdded(key, accepted.Value);
    });
    baseDict.OnValueRemoved += new NetDictionary<Vector2, StardewValley.Object, NetRef<StardewValley.Object>, SerializableDictionary<Vector2, StardewValley.Object>, NetVector2Dictionary<StardewValley.Object, NetRef<StardewValley.Object>>>.ContentsChangeEvent(this.OnValueRemoved);
  }

  public bool Any() => this.compositeDict.Count > 0;

  public int Count() => this.compositeDict.Count;

  /// <summary>Freeze the object list, so changes will be queued until <see cref="M:StardewValley.Network.OverlaidDictionary.Unlock" /> is called.</summary>
  public void Lock() => this._locked = true;

  /// <summary>Remove the freeze added by <see cref="M:StardewValley.Network.OverlaidDictionary.Lock" /> and apply all changes that were queued while it was locked.</summary>
  public void Unlock()
  {
    if (!this._locked)
      return;
    this._locked = false;
    if (this._changes.Count <= 0)
      return;
    foreach (KeyValuePair<Vector2, StardewValley.Object> change in this._changes)
    {
      if (change.Value != null)
        this.baseDict[change.Key] = change.Value;
      else
        this.baseDict.Remove(change.Key);
    }
    this._changes.Clear();
  }

  /// <summary>Add an object to the dictionary.</summary>
  /// <param name="key">The tile position.</param>
  /// <param name="value">The object instance.</param>
  /// <exception cref="T:System.ArgumentException">The key is already present in the dictionary.</exception>
  public void Add(Vector2 key, StardewValley.Object value)
  {
    if (this._locked)
    {
      StardewValley.Object @object;
      if (this._changes.TryGetValue(key, out @object))
      {
        if (@object != null)
          throw new ArgumentException();
        this._changes[key] = value;
      }
      else
      {
        if (this.baseDict.ContainsKey(key))
          throw new ArgumentException();
        this._changes[key] = value;
      }
    }
    else
      this.baseDict.Add(key, value);
  }

  /// <summary>Add an object to the dictionary if the key isn't already present.</summary>
  /// <param name="key">The tile position.</param>
  /// <param name="value">The object instance.</param>
  /// <returns>Returns whether the object was successfully added.</returns>
  public bool TryAdd(Vector2 key, StardewValley.Object value)
  {
    if (this.ContainsKey(key))
      return false;
    this.Add(key, value);
    return true;
  }

  public void Clear()
  {
    if (this._locked)
      throw new NotImplementedException();
    this.baseDict.Clear();
    this.overlayDict.Clear();
    this.compositeDict.Clear();
  }

  public bool ContainsKey(Vector2 key)
  {
    StardewValley.Object @object;
    return this._locked && this._changes.TryGetValue(key, out @object) ? @object != null : this.compositeDict.ContainsKey(key);
  }

  public bool Remove(Vector2 key)
  {
    if (this.overlayDict.Remove(key))
      return true;
    if (!this._locked)
      return this.baseDict.Remove(key);
    StardewValley.Object @object;
    if (this._changes.TryGetValue(key, out @object))
    {
      this._changes[key] = (StardewValley.Object) null;
      return @object != null;
    }
    if (!this.baseDict.ContainsKey(key))
      return false;
    this._changes[key] = (StardewValley.Object) null;
    return true;
  }

  /// <summary>Get the object on a given tile if it exists.</summary>
  /// <param name="key">The tile position to check.</param>
  /// <param name="value">The object that was found, if applicable.</param>
  /// <returns>Returns whether the object was found.</returns>
  public bool TryGetValue(Vector2 key, out StardewValley.Object value)
  {
    return this.compositeDict.TryGetValue(key, out value);
  }

  /// <summary>Get the object on a given tile if it exists, else a default value.</summary>
  /// <param name="key">The tile position to check.</param>
  /// <param name="defaultValue">The value to return if the element isn't found.</param>
  public StardewValley.Object GetValueOrDefault(Vector2 key, StardewValley.Object defaultValue = null)
  {
    return this.compositeDict.GetValueOrDefault<Vector2, StardewValley.Object>(key, defaultValue);
  }

  public IEnumerator<SerializableDictionary<Vector2, StardewValley.Object>> GetEnumerator()
  {
    return this.baseDict.GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.baseDict.GetEnumerator();

  public void Add(SerializableDictionary<Vector2, StardewValley.Object> dict)
  {
    foreach (KeyValuePair<Vector2, StardewValley.Object> keyValuePair in (Dictionary<Vector2, StardewValley.Object>) dict)
    {
      if (keyValuePair.Value != null)
        this.Add(keyValuePair.Key, keyValuePair.Value);
    }
  }
}
