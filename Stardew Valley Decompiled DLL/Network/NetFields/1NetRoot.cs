// Decompiled with JetBrains decompiler
// Type: Netcode.NetRootDictionary`2
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

#nullable disable
namespace Netcode;

public class NetRootDictionary<TKey, TValue> : 
  IDictionary<TKey, TValue>,
  ICollection<KeyValuePair<TKey, TValue>>,
  IEnumerable<KeyValuePair<TKey, TValue>>,
  IEnumerable
  where TValue : class, INetObject<INetSerializable>
{
  public XmlSerializer Serializer;
  public Dictionary<TKey, NetRoot<TValue>> Roots = new Dictionary<TKey, NetRoot<TValue>>();

  public NetRootDictionary()
  {
  }

  public NetRootDictionary(IEnumerable<KeyValuePair<TKey, TValue>> values)
  {
    foreach (KeyValuePair<TKey, TValue> keyValuePair in values)
      this.Add(keyValuePair.Key, keyValuePair.Value);
  }

  public TValue this[TKey key]
  {
    get => this.Roots[key].Get();
    set
    {
      if (!this.ContainsKey(key))
        this.Add(key, value);
      else
        this.Roots[key].Set(value);
    }
  }

  public int Count => this.Roots.Count;

  public bool IsReadOnly => ((IDictionary) this.Roots).IsReadOnly;

  public ICollection<TKey> Keys => (ICollection<TKey>) this.Roots.Keys;

  public ICollection<TValue> Values
  {
    get
    {
      return (ICollection<TValue>) this.Roots.Values.Select<NetRoot<TValue>, TValue>((Func<NetRoot<TValue>, TValue>) (root => root.Get())).ToList<TValue>();
    }
  }

  public void Add(KeyValuePair<TKey, TValue> item) => this.Add(item.Key, item.Value);

  public void Add(TKey key, TValue value)
  {
    NetRoot<TValue> netRoot = new NetRoot<TValue>(value);
    netRoot.Serializer = this.Serializer;
    this.Roots.Add(key, netRoot);
  }

  public void Clear() => this.Roots.Clear();

  public bool Contains(KeyValuePair<TKey, TValue> item)
  {
    NetRoot<TValue> netRoot;
    return this.Roots.TryGetValue(item.Key, out netRoot) && netRoot == (object) item.Value;
  }

  public bool ContainsKey(TKey key) => this.Roots.ContainsKey(key);

  public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
  {
    if (array == null)
      throw new ArgumentNullException();
    if (arrayIndex < 0)
      throw new ArgumentOutOfRangeException();
    if (array.Length < this.Count - arrayIndex)
      throw new ArgumentException();
    foreach (KeyValuePair<TKey, TValue> keyValuePair in this)
      array[arrayIndex++] = keyValuePair;
  }

  public NetRootDictionary<TKey, TValue>.Enumerator GetEnumerator()
  {
    return new NetRootDictionary<TKey, TValue>.Enumerator(this.Roots);
  }

  IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
  {
    return (IEnumerator<KeyValuePair<TKey, TValue>>) new NetRootDictionary<TKey, TValue>.Enumerator(this.Roots);
  }

  IEnumerator IEnumerable.GetEnumerator()
  {
    return (IEnumerator) new NetRootDictionary<TKey, TValue>.Enumerator(this.Roots);
  }

  public bool Remove(KeyValuePair<TKey, TValue> item)
  {
    return this.Contains(item) && this.Remove(item.Key);
  }

  public bool Remove(TKey key) => this.Roots.Remove(key);

  public bool TryGetValue(TKey key, out TValue value)
  {
    NetRoot<TValue> netRoot;
    if (this.Roots.TryGetValue(key, out netRoot))
    {
      value = netRoot.Get();
      return true;
    }
    value = default (TValue);
    return false;
  }

  public struct Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>, IEnumerator, IDisposable
  {
    private Dictionary<TKey, NetRoot<TValue>> _roots;
    private Dictionary<TKey, NetRoot<TValue>>.Enumerator _enumerator;
    private KeyValuePair<TKey, TValue> _current;
    private bool _done;

    public Enumerator(Dictionary<TKey, NetRoot<TValue>> roots)
    {
      this._roots = roots;
      this._enumerator = this._roots.GetEnumerator();
      this._current = new KeyValuePair<TKey, TValue>();
      this._done = false;
    }

    public bool MoveNext()
    {
      if (this._enumerator.MoveNext())
      {
        KeyValuePair<TKey, NetRoot<TValue>> current = this._enumerator.Current;
        this._current = new KeyValuePair<TKey, TValue>(current.Key, current.Value.Get());
        return true;
      }
      this._done = true;
      this._current = new KeyValuePair<TKey, TValue>();
      return false;
    }

    public KeyValuePair<TKey, TValue> Current => this._current;

    public void Dispose()
    {
    }

    object IEnumerator.Current
    {
      get
      {
        if (this._done)
          throw new InvalidOperationException();
        return (object) this._current;
      }
    }

    void IEnumerator.Reset()
    {
      this._enumerator = this._roots.GetEnumerator();
      this._current = new KeyValuePair<TKey, TValue>();
      this._done = false;
    }
  }
}
