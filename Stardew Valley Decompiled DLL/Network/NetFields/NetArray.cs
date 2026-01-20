// Decompiled with JetBrains decompiler
// Type: Netcode.NetArray`2
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

public class NetArray<T, TField> : 
  AbstractNetSerializable,
  IList<T>,
  ICollection<T>,
  IEnumerable<T>,
  IEnumerable,
  IEquatable<NetArray<T, TField>>
  where TField : NetField<T, TField>, new()
{
  private int appendPosition;
  private readonly List<TField> elements = new List<TField>();

  public List<TField> Fields => this.elements;

  public event NetArray<T, TField>.FieldCreateEvent OnFieldCreate;

  public NetArray()
  {
  }

  public NetArray(IEnumerable<T> values)
    : this()
  {
    int num = 0;
    foreach (T newValue in values)
    {
      TField field = this.createField(num++);
      field.Set(newValue);
      this.elements.Add(field);
    }
  }

  public NetArray(int size)
    : this()
  {
    for (int index = 0; index < size; ++index)
      this.elements.Add(this.createField(index));
  }

  private TField createField(int index)
  {
    TField field = new TField().Interpolated(false, false);
    NetArray<T, TField>.FieldCreateEvent onFieldCreate = this.OnFieldCreate;
    if (onFieldCreate != null)
      onFieldCreate(index, field);
    return field;
  }

  public T this[int index]
  {
    get => this.elements[index].Get();
    set => this.elements[index].Set(value);
  }

  public int Count => this.elements.Count;

  public int Length => this.elements.Count;

  public bool IsReadOnly => false;

  public bool IsFixedSize => this.Parent != null;

  public void Add(T item)
  {
    if (this.IsFixedSize)
      throw new InvalidOperationException();
    while (this.appendPosition >= this.elements.Count)
      this.elements.Add(this.createField(this.elements.Count));
    this.elements[this.appendPosition].Set(item);
    ++this.appendPosition;
  }

  public void Clear()
  {
    if (this.IsFixedSize)
      throw new InvalidOperationException();
    this.elements.Clear();
  }

  public bool Contains(T item)
  {
    foreach (TField element in this.elements)
    {
      if (object.Equals((object) element.Get(), (object) item))
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

  private void ensureCapacity(int size)
  {
    if (this.IsFixedSize && size != this.Count)
      throw new InvalidOperationException();
    while (this.Count < size)
      this.elements.Add(this.createField(this.Count));
  }

  public void SetCount(int size) => this.ensureCapacity(size);

  public void Set(IList<T> values)
  {
    this.ensureCapacity(values.Count);
    for (int index = 0; index < this.Count; ++index)
      this[index] = values[index];
  }

  public bool Equals(NetArray<T, TField> other)
  {
    return object.Equals((object) this.elements, (object) other.elements);
  }

  public override bool Equals(object obj) => obj is NetArray<T, TField> other && this.Equals(other);

  public override int GetHashCode() => this.elements.GetHashCode() ^ 805984909;

  public IEnumerator<T> GetEnumerator()
  {
    foreach (TField element in this.elements)
      yield return element.Get();
  }

  public int IndexOf(T item)
  {
    for (int index = 0; index < this.Count; ++index)
    {
      if (object.Equals((object) this.elements[index].Get(), (object) item))
        return index;
    }
    return -1;
  }

  public void Insert(int index, T item)
  {
    if (this.IsFixedSize)
      throw new InvalidOperationException();
    TField field = this.createField(index);
    field.Set(item);
    this.elements.Insert(index, field);
  }

  public bool Remove(T item)
  {
    int index = this.IndexOf(item);
    if (index == -1)
      return false;
    this.RemoveAt(index);
    return true;
  }

  public void RemoveAt(int index)
  {
    if (this.IsFixedSize)
      throw new InvalidOperationException();
    this.elements.RemoveAt(index);
  }

  IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

  public override void Read(BinaryReader reader, NetVersion version)
  {
    BitArray bitArray = reader.ReadBitArray();
    for (int index = 0; index < this.elements.Count; ++index)
    {
      if (bitArray[index])
        this.elements[index].Read(reader, version);
    }
  }

  public override void Write(BinaryWriter writer)
  {
    BitArray bits = new BitArray(this.elements.Count);
    for (int index = 0; index < this.elements.Count; ++index)
      bits[index] = this.elements[index].Dirty;
    writer.WriteBitArray(bits);
    for (int index = 0; index < this.elements.Count; ++index)
    {
      if (bits[index])
        this.elements[index].Write(writer);
    }
  }

  public override void ReadFull(BinaryReader reader, NetVersion version)
  {
    int num = reader.ReadInt32();
    this.elements.Clear();
    for (int index = 0; index < num; ++index)
    {
      TField field = this.createField(this.elements.Count);
      field.ReadFull(reader, version);
      if (this.Parent != null)
        field.Parent = (INetSerializable) this;
      this.elements.Add(field);
    }
  }

  public override void WriteFull(BinaryWriter writer)
  {
    writer.Write(this.Count);
    foreach (TField element in this.elements)
      element.WriteFull(writer);
  }

  protected override void ForEachChild(Action<INetSerializable> childAction)
  {
    foreach (TField element in this.elements)
      childAction((INetSerializable) element);
  }

  public override string ToString() => string.Join<T>(",", (IEnumerable<T>) this);

  public delegate void FieldCreateEvent(int index, TField field) where TField : NetField<T, TField>, new();
}
