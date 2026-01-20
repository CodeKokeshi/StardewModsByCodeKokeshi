// Decompiled with JetBrains decompiler
// Type: Netcode.NetObjectShrinkList`1
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

public class NetObjectShrinkList<T> : 
  AbstractNetSerializable,
  IList<T>,
  ICollection<T>,
  IEnumerable<T>,
  IEnumerable,
  IEquatable<NetObjectShrinkList<T>>
  where T : class, INetObject<INetSerializable>
{
  private NetArray<T, NetRef<T>> array = new NetArray<T, NetRef<T>>();

  public T this[int index]
  {
    get
    {
      int num = 0;
      for (int index1 = 0; index1 < this.array.Count; ++index1)
      {
        T obj = this.array[index1];
        if ((object) obj != null)
        {
          if (index == num)
            return obj;
          ++num;
        }
      }
      throw new ArgumentOutOfRangeException(nameof (index));
    }
    set
    {
      int num = 0;
      for (int index1 = 0; index1 < this.array.Count; ++index1)
      {
        if ((object) this.array[index1] != null)
        {
          if (index == num)
          {
            this.array[index1] = value;
            return;
          }
          ++num;
        }
      }
      throw new ArgumentOutOfRangeException(nameof (index));
    }
  }

  public int Count
  {
    get
    {
      int count = 0;
      for (int index = 0; index < this.array.Count; ++index)
      {
        if ((object) this.array[index] != null)
          ++count;
      }
      return count;
    }
  }

  public bool IsReadOnly => false;

  public NetObjectShrinkList()
  {
  }

  public NetObjectShrinkList(IEnumerable<T> values)
    : this()
  {
    foreach (T obj in values)
      this.array.Add(obj);
  }

  public void Add(T item) => this.array.Add(item);

  public void Clear()
  {
    for (int index = 0; index < this.array.Count; ++index)
      this.array[index] = default (T);
  }

  public void CopyFrom(IList<T> list)
  {
    if (list == this)
      return;
    if (list.Count > this.array.Count)
      throw new InvalidOperationException();
    for (int index = 0; index < this.array.Count; ++index)
      this.array[index] = index >= list.Count ? default (T) : list[index];
  }

  public void Set(IList<T> list) => this.CopyFrom(list);

  public void MoveFrom(IList<T> list)
  {
    List<T> list1 = new List<T>((IEnumerable<T>) list);
    list.Clear();
    this.Set((IList<T>) list1);
  }

  public bool Contains(T item)
  {
    foreach (T obj in this)
    {
      if ((object) obj == (object) item)
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

  public bool Equals(NetObjectShrinkList<T> other)
  {
    if (this.Count != other.Count)
      return false;
    for (int index = 0; index < this.Count; ++index)
    {
      if ((object) this[index] != (object) other[index])
        return false;
    }
    return true;
  }

  public NetObjectShrinkList<T>.Enumerator GetEnumerator()
  {
    return new NetObjectShrinkList<T>.Enumerator(this.array);
  }

  IEnumerator<T> IEnumerable<T>.GetEnumerator()
  {
    return (IEnumerator<T>) new NetObjectShrinkList<T>.Enumerator(this.array);
  }

  IEnumerator IEnumerable.GetEnumerator()
  {
    return (IEnumerator) new NetObjectShrinkList<T>.Enumerator(this.array);
  }

  public int IndexOf(T item)
  {
    int num = 0;
    for (int index = 0; index < this.array.Count; ++index)
    {
      T obj = this.array[index];
      if ((object) obj != null)
      {
        if ((object) obj == (object) item)
          return num;
        ++num;
      }
    }
    return -1;
  }

  public void Insert(int index, T item)
  {
    int num = 0;
    for (int index1 = 0; index1 < this.array.Count; ++index1)
    {
      if ((object) this.array[index1] != null)
      {
        if (num == index)
        {
          this.array.Insert(index1, item);
          return;
        }
        ++num;
      }
    }
    throw new ArgumentOutOfRangeException(nameof (index));
  }

  public override void Read(BinaryReader reader, NetVersion version)
  {
    this.array.Read(reader, version);
  }

  public override void ReadFull(BinaryReader reader, NetVersion version)
  {
    this.array.ReadFull(reader, version);
  }

  public bool Remove(T item)
  {
    for (int index = 0; index < this.array.Count; ++index)
    {
      if ((object) this.array[index] == (object) item)
      {
        this.array[index] = default (T);
        return true;
      }
    }
    return false;
  }

  public void RemoveAt(int index)
  {
    int num = 0;
    for (int index1 = 0; index1 < this.array.Count; ++index1)
    {
      if ((object) this.array[index1] != null)
      {
        if (num == index)
        {
          this.array[index1] = default (T);
          break;
        }
        ++num;
      }
    }
  }

  public override void Write(BinaryWriter writer) => this.array.Write(writer);

  public override void WriteFull(BinaryWriter writer) => this.array.WriteFull(writer);

  protected override void ForEachChild(Action<INetSerializable> childAction)
  {
    childAction((INetSerializable) this.array);
  }

  public override string ToString() => string.Join<T>(",", (IEnumerable<T>) this);

  public struct Enumerator(NetArray<T, NetRef<T>> array) : IEnumerator<T>, IEnumerator, IDisposable
  {
    private readonly NetArray<T, NetRef<T>> _array = array;
    private int _index = 0;
    private T _current = default (T);
    private bool _done = false;

    public bool MoveNext()
    {
      while (this._index < this._array.Count)
      {
        T obj = this._array[this._index];
        ++this._index;
        if ((object) obj != null)
        {
          this._current = obj;
          return true;
        }
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
