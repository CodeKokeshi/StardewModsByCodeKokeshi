// Decompiled with JetBrains decompiler
// Type: Netcode.NetList`2
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

public class NetList<T, TField> : 
  AbstractNetSerializable,
  IList<T>,
  ICollection<T>,
  IEnumerable<T>,
  IEnumerable,
  IEquatable<NetList<T, TField>>
  where TField : NetField<T, TField>, new()
{
  private const int initialSize = 10;
  private const double resizeFactor = 1.5;
  protected readonly NetInt count = new NetInt(0).Interpolated(false, false);
  protected readonly NetRef<NetArray<T, TField>> array = new NetRef<NetArray<T, TField>>(new NetArray<T, TField>(10)).Interpolated(false, false);

  public virtual T this[int index]
  {
    get
    {
      if (index >= this.Count || index < 0)
        throw new ArgumentOutOfRangeException();
      return this.array.Value[index];
    }
    set
    {
      if (index >= this.Count || index < 0)
        throw new ArgumentOutOfRangeException();
      this.array.Value[index] = value;
    }
  }

  public int Count => this.count.Value;

  public int Capacity => this.array.Value.Count;

  public bool IsReadOnly => false;

  public event NetList<T, TField>.ElementChangedEvent OnElementChanged;

  public event NetList<T, TField>.ArrayReplacedEvent OnArrayReplaced;

  public NetList()
  {
    this.hookArray(this.array.Value);
    this.array.fieldChangeVisibleEvent += (FieldChange<NetRef<NetArray<T, TField>>, NetArray<T, TField>>) ((arrayRef, oldArray, newArray) =>
    {
      if (newArray != null)
        this.hookArray(newArray);
      NetList<T, TField>.ArrayReplacedEvent onArrayReplaced = this.OnArrayReplaced;
      if (onArrayReplaced == null)
        return;
      onArrayReplaced(this, (IList<T>) oldArray, (IList<T>) newArray);
    });
  }

  public NetList(IEnumerable<T> values)
    : this()
  {
    foreach (T obj in values)
      this.Add(obj);
  }

  public NetList(int capacity)
    : this()
  {
    this.Resize(capacity);
  }

  private void hookField(int index, TField field)
  {
    if ((NetFieldBase<T, TField>) field == default (TField))
      return;
    field.fieldChangeVisibleEvent += (FieldChange<TField, T>) ((f, oldValue, newValue) =>
    {
      NetList<T, TField>.ElementChangedEvent onElementChanged = this.OnElementChanged;
      if (onElementChanged == null)
        return;
      onElementChanged(this, index, oldValue, newValue);
    });
  }

  private void hookArray(NetArray<T, TField> array)
  {
    for (int index = 0; index < array.Count; ++index)
      this.hookField(index, array.Fields[index]);
    array.OnFieldCreate += new NetArray<T, TField>.FieldCreateEvent(this.hookField);
  }

  private void Resize(int capacity)
  {
    this.count.Set(Math.Min(capacity, this.count.Value));
    NetArray<T, TField> netArray = this.array.Value;
    this.array.Value = new NetArray<T, TField>(capacity);
    for (int index = 0; index < capacity && index < this.Count; ++index)
    {
      T obj = netArray[index];
      netArray[index] = default (T);
      this.array.Value[index] = obj;
    }
  }

  private void EnsureCapacity(int neededCapacity)
  {
    if (neededCapacity <= this.Capacity)
      return;
    int capacity = (int) ((double) this.Capacity * 1.5);
    while (neededCapacity > capacity)
      capacity = (int) ((double) capacity * 1.5);
    this.Resize(capacity);
  }

  public virtual void Add(T item)
  {
    this.EnsureCapacity(this.Count + 1);
    this.array.Value[this.Count] = item;
    this.count.Set(this.count.Value + 1);
  }

  public virtual void Clear()
  {
    this.count.Set(0);
    this.Resize(10);
    this.fillNull();
  }

  private void fillNull()
  {
    for (int index = 0; index < this.Capacity; ++index)
      this.array.Value[index] = default (T);
  }

  public virtual void CopyFrom(IList<T> list)
  {
    if (list == this)
      return;
    this.EnsureCapacity(list.Count);
    this.fillNull();
    this.count.Set(list.Count);
    for (int index = 0; index < list.Count; ++index)
      this.array.Value[index] = list[index];
  }

  public void Set(IList<T> list) => this.CopyFrom(list);

  public void MoveFrom(NetList<T, TField> list)
  {
    List<T> list1 = new List<T>((IEnumerable<T>) list);
    list.Clear();
    this.Set((IList<T>) list1);
  }

  public bool Any() => this.count.Value > 0;

  public virtual bool Contains(T item)
  {
    foreach (T objA in this)
    {
      if (object.Equals((object) objA, (object) item))
        return true;
    }
    return false;
  }

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

  public List<T> GetRange(int index, int count)
  {
    List<T> range = new List<T>();
    for (int index1 = index; index1 < index + count; ++index1)
      range.Add(this[index1]);
    return range;
  }

  public void AddRange(IEnumerable<T> collection)
  {
    foreach (T obj in collection)
      this.Add(obj);
  }

  public void RemoveRange(int index, int count)
  {
    for (int index1 = 0; index1 < count; ++index1)
      this.RemoveAt(index);
  }

  public bool Equals(NetList<T, TField> other)
  {
    return object.Equals((object) this.array, (object) other.array);
  }

  public NetList<T, TField>.Enumerator GetEnumerator() => new NetList<T, TField>.Enumerator(this);

  IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) new NetList<T, TField>.Enumerator(this);

  IEnumerator<T> IEnumerable<T>.GetEnumerator()
  {
    return (IEnumerator<T>) new NetList<T, TField>.Enumerator(this);
  }

  public virtual int IndexOf(T item)
  {
    for (int index = 0; index < this.Count; ++index)
    {
      if (object.Equals((object) this.array.Value[index], (object) item))
        return index;
    }
    return -1;
  }

  public virtual void Insert(int index, T item)
  {
    if (index > this.Count || index < 0)
      throw new ArgumentOutOfRangeException();
    this.EnsureCapacity(this.Count + 1);
    this.count.Set(this.count.Value + 1);
    for (int index1 = this.Count - 1; index1 > index; --index1)
    {
      T obj = this.array.Value[index1 - 1];
      this.array.Value[index1 - 1] = default (T);
      this.array.Value[index1] = obj;
    }
    this.array.Value[index] = item;
  }

  public override void Read(BinaryReader reader, NetVersion version)
  {
    this.count.Read(reader, version);
    this.array.Read(reader, version);
  }

  public override void ReadFull(BinaryReader reader, NetVersion version)
  {
    this.count.ReadFull(reader, version);
    this.array.ReadFull(reader, version);
  }

  public bool Remove(T item)
  {
    int index = this.IndexOf(item);
    if (index == -1)
      return false;
    this.RemoveAt(index);
    return true;
  }

  public virtual void RemoveAt(int index)
  {
    if (index < 0 || index >= this.Count)
      throw new ArgumentOutOfRangeException();
    this.count.Set(this.count.Value - 1);
    for (int index1 = index; index1 < this.Count; ++index1)
    {
      T obj = this.array.Value[index1 + 1];
      this.array.Value[index1 + 1] = default (T);
      this.array.Value[index1] = obj;
    }
    this.array.Value[this.Count] = default (T);
  }

  /// <summary>Remove all elements that match a condition.</summary>
  /// <param name="match">The predicate matching values to remove.</param>
  /// <returns>Returns the number of values removed from the set.</returns>
  public int RemoveWhere(Func<T, bool> match)
  {
    int num = 0;
    for (int index = this.Count - 1; index >= 0; --index)
    {
      if (match(this[index]))
      {
        this.RemoveAt(index);
        ++num;
      }
    }
    return num;
  }

  /// <summary>Remove all entries which don't match the filter.</summary>
  /// <param name="f">Get whether to keep the given item.</param>
  [Obsolete("Use RemoveWhere instead.")]
  public void Filter(Func<T, bool> f) => this.RemoveWhere((Func<T, bool>) (pair => !f(pair)));

  public override void Write(BinaryWriter writer)
  {
    this.count.Write(writer);
    this.array.Write(writer);
  }

  public override void WriteFull(BinaryWriter writer)
  {
    this.count.WriteFull(writer);
    this.array.WriteFull(writer);
  }

  protected override void ForEachChild(Action<INetSerializable> childAction)
  {
    childAction((INetSerializable) this.count);
    childAction((INetSerializable) this.array);
  }

  public override string ToString() => string.Join<T>(",", (IEnumerable<T>) this);

  public delegate void ElementChangedEvent(
    NetList<T, TField> list,
    int index,
    T oldValue,
    T newValue)
    where TField : NetField<T, TField>, new();

  public delegate void ArrayReplacedEvent(NetList<T, TField> list, IList<T> before, IList<T> after) where TField : NetField<T, TField>, new();

  public struct Enumerator(NetList<T, TField> list) : IEnumerator<T>, IEnumerator, IDisposable
  {
    private readonly NetList<T, TField> _list = list;
    private int _index = 0;
    private T _current = default (T);
    private bool _done = false;

    public bool MoveNext()
    {
      if (this._index < this._list.count.Value)
      {
        this._current = this._list.array.Value[this._index];
        ++this._index;
        return true;
      }
      this._done = true;
      this._current = default (T);
      return false;
    }

    public T Current => this._current;

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
      this._index = 0;
      this._current = default (T);
      this._done = false;
    }
  }
}
