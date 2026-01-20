// Decompiled with JetBrains decompiler
// Type: Netcode.NetCollection`1
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

#nullable disable
namespace Netcode;

public sealed class NetCollection<T> : 
  AbstractNetSerializable,
  IList<T>,
  ICollection<T>,
  IEnumerable<T>,
  IEnumerable,
  IEquatable<NetCollection<T>>
  where T : class, INetObject<INetSerializable>
{
  private List<Guid> guids = new List<Guid>();
  private List<T> list = new List<T>();
  private NetGuidDictionary<T, NetRef<T>> elements = new NetGuidDictionary<T, NetRef<T>>();

  public int Count => this.list.Count;

  public bool IsReadOnly => false;

  public bool InterpolationWait
  {
    get => this.elements.InterpolationWait;
    set => this.elements.InterpolationWait = value;
  }

  public T this[int index]
  {
    get => this.list[index];
    set => this.elements[this.guids[index]] = value;
  }

  public T this[Guid guid] => this.elements[guid];

  public event NetCollection<T>.ContentsChangeEvent OnValueAdded;

  public event NetCollection<T>.ContentsChangeEvent OnValueRemoved;

  public NetCollection()
  {
    this.elements.OnValueTargetUpdated += (NetDictionary<Guid, T, NetRef<T>, Dictionary<Guid, T>, NetGuidDictionary<T, NetRef<T>>>.ContentsUpdateEvent) ((guid, old_target_value, new_target_value) =>
    {
      if ((object) old_target_value == (object) new_target_value)
        return;
      int index = this.guids.IndexOf(guid);
      if (index == -1)
      {
        this.guids.Add(guid);
        this.list.Add(new_target_value);
      }
      else
        this.list[index] = new_target_value;
    });
    this.elements.OnValueAdded += (NetDictionary<Guid, T, NetRef<T>, Dictionary<Guid, T>, NetGuidDictionary<T, NetRef<T>>>.ContentsChangeEvent) ((guid, value) =>
    {
      int index = this.guids.IndexOf(guid);
      if (index == -1)
      {
        this.guids.Add(guid);
        this.list.Add(value);
      }
      else
        this.list[index] = value;
      NetCollection<T>.ContentsChangeEvent onValueAdded = this.OnValueAdded;
      if (onValueAdded == null)
        return;
      onValueAdded(value);
    });
    this.elements.OnValueRemoved += (NetDictionary<Guid, T, NetRef<T>, Dictionary<Guid, T>, NetGuidDictionary<T, NetRef<T>>>.ContentsChangeEvent) ((guid, value) =>
    {
      int index = this.guids.IndexOf(guid);
      if (index != -1)
      {
        this.guids.RemoveAt(index);
        this.list.RemoveAt(index);
      }
      NetCollection<T>.ContentsChangeEvent onValueRemoved = this.OnValueRemoved;
      if (onValueRemoved == null)
        return;
      onValueRemoved(value);
    });
  }

  public NetCollection(IEnumerable<T> values)
    : this()
  {
    foreach (T obj in values)
      this.Add(obj);
  }

  /// <summary>Try to get a value from the collection by its ID.</summary>
  /// <param name="id">The entry ID.</param>
  /// <param name="value">The entry value, if found.</param>
  /// <returns>Returns whether a matching entry was found.</returns>
  public bool TryGetValue(Guid id, out T value) => this.elements.TryGetValue(id, out value);

  public void Add(T item) => this.elements.Add(Guid.NewGuid(), item);

  public bool Equals(NetCollection<T> other) => this.elements.Equals(other.elements);

  public List<T>.Enumerator GetEnumerator() => this.list.GetEnumerator();

  IEnumerator<T> IEnumerable<T>.GetEnumerator() => (IEnumerator<T>) this.list.GetEnumerator();

  IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

  public void Clear() => this.elements.Clear();

  public void Set(ICollection<T> other)
  {
    this.Clear();
    foreach (T obj in (IEnumerable<T>) other)
      this.Add(obj);
  }

  public bool Contains(T item) => this.list.Contains(item);

  public bool ContainsGuid(Guid guid) => this.elements.ContainsKey(guid);

  public Guid GuidOf(T item)
  {
    for (int index = 0; index < this.list.Count; ++index)
    {
      if ((object) this.list[index] == (object) item)
        return this.guids[index];
    }
    return Guid.Empty;
  }

  public int IndexOf(T item) => this.list.IndexOf(item);

  public void Insert(int index, T item) => throw new NotSupportedException();

  public void CopyTo(T[] array, int arrayIndex)
  {
    if (array == null)
      throw new ArgumentNullException();
    if (arrayIndex < 0)
      throw new ArgumentOutOfRangeException();
    if (this.Count - arrayIndex > array.Length)
      throw new ArgumentException();
    foreach (T obj in this)
      array[arrayIndex++] = obj;
  }

  public bool Remove(T item)
  {
    foreach (Guid guid in this.guids)
    {
      if ((object) this.elements[guid] == (object) item)
      {
        this.elements.Remove(guid);
        return true;
      }
    }
    return false;
  }

  public void RemoveAt(int index) => this.elements.Remove(this.guids[index]);

  public void Remove(Guid guid) => this.elements.Remove(guid);

  /// <summary>Remove all elements that match a condition.</summary>
  /// <param name="match">The predicate matching values to remove.</param>
  /// <returns>Returns the number of values removed from the collection.</returns>
  public int RemoveWhere(Func<T, bool> match)
  {
    int num = 0;
    for (int index = this.list.Count - 1; index >= 0; --index)
    {
      if (match(this.list[index]))
      {
        this.elements.Remove(this.guids[index]);
        ++num;
      }
    }
    return num;
  }

  [Obsolete("Use RemoveWhere instead.")]
  public void Filter(Func<T, bool> f) => this.RemoveWhere((Func<T, bool>) (pair => !f(pair)));

  protected override void ForEachChild(Action<INetSerializable> childAction)
  {
    childAction((INetSerializable) this.elements);
  }

  public override void Read(BinaryReader reader, NetVersion version)
  {
    this.elements.Read(reader, version);
  }

  public override void Write(BinaryWriter writer) => this.elements.Write(writer);

  public override void ReadFull(BinaryReader reader, NetVersion version)
  {
    this.elements.ReadFull(reader, version);
  }

  public override void WriteFull(BinaryWriter writer) => this.elements.WriteFull(writer);

  public delegate void ContentsChangeEvent(T value) where T : class, INetObject<INetSerializable>;
}
