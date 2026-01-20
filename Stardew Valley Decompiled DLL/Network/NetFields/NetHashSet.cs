// Decompiled with JetBrains decompiler
// Type: Netcode.NetHashSet`1
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

#nullable disable
namespace Netcode;

public abstract class NetHashSet<TValue> : 
  AbstractNetSerializable,
  IEquatable<NetHashSet<TValue>>,
  ISet<TValue>,
  ICollection<TValue>,
  IEnumerable<TValue>,
  IEnumerable
{
  public bool InterpolationWait = true;
  private readonly HashSet<TValue> Set = new HashSet<TValue>();
  private readonly List<NetHashSet<TValue>.IncomingChange> IncomingChanges = new List<NetHashSet<TValue>.IncomingChange>();
  private readonly List<NetHashSet<TValue>.OutgoingChange> OutgoingChanges = new List<NetHashSet<TValue>.OutgoingChange>();

  public event NetHashSet<TValue>.ContentsChangeEvent OnValueAdded;

  public event NetHashSet<TValue>.ContentsChangeEvent OnValueRemoved;

  public NetHashSet()
  {
  }

  public NetHashSet(IEnumerable<TValue> values)
    : this()
  {
    foreach (TValue obj in values)
      this.Add(obj);
  }

  /// <inheritdoc />
  public int Count => this.Set.Count;

  /// <inheritdoc />
  public bool IsReadOnly => false;

  public bool Add(TValue item)
  {
    if (!this.Set.Add(item))
      return false;
    this.OutgoingChanges.Add(new NetHashSet<TValue>.OutgoingChange(false, item));
    this.MarkDirty();
    this.addedEvent(item);
    return true;
  }

  /// <inheritdoc />
  public void Clear()
  {
    foreach (TValue obj in this.Set.ToArray<TValue>())
      this.Remove(obj);
    this.OutgoingChanges.RemoveAll((Predicate<NetHashSet<TValue>.OutgoingChange>) (ch => !ch.Removal));
  }

  /// <inheritdoc />
  public bool Contains(TValue item) => this.Set.Contains(item);

  /// <inheritdoc />
  public void CopyTo(TValue[] array, int arrayIndex) => this.Set.CopyTo(array, arrayIndex);

  /// <inheritdoc />
  public bool Equals(NetHashSet<TValue> other) => this.Set.Equals((object) other?.Set);

  /// <inheritdoc />
  public void ExceptWith(IEnumerable<TValue> other) => this.Set.ExceptWith(other);

  /// <inheritdoc />
  public IEnumerator<TValue> GetEnumerator() => (IEnumerator<TValue>) this.Set.GetEnumerator();

  /// <inheritdoc />
  public void IntersectWith(IEnumerable<TValue> other) => this.Set.IntersectWith(other);

  /// <inheritdoc />
  public bool IsProperSubsetOf(IEnumerable<TValue> other) => this.Set.IsProperSubsetOf(other);

  /// <inheritdoc />
  public bool IsProperSupersetOf(IEnumerable<TValue> other) => this.Set.IsProperSupersetOf(other);

  /// <inheritdoc />
  public bool IsSubsetOf(IEnumerable<TValue> other) => this.Set.IsSubsetOf(other);

  /// <inheritdoc />
  public bool IsSupersetOf(IEnumerable<TValue> other) => this.Set.IsSupersetOf(other);

  /// <inheritdoc />
  public bool Overlaps(IEnumerable<TValue> other) => this.Set.Overlaps(other);

  /// <inheritdoc />
  public bool Remove(TValue item)
  {
    if (!this.Set.Remove(item))
      return false;
    this.OutgoingChanges.Add(new NetHashSet<TValue>.OutgoingChange(true, item));
    this.MarkDirty();
    this.removedEvent(item);
    return true;
  }

  /// <summary>Remove all elements that match a condition.</summary>
  /// <param name="match">The predicate matching values to remove.</param>
  /// <returns>Returns the number of values removed from the set.</returns>
  public int RemoveWhere(Predicate<TValue> match)
  {
    int num = this.Set.RemoveWhere((Predicate<TValue>) (value =>
    {
      if (!match(value))
        return false;
      this.OutgoingChanges.Add(new NetHashSet<TValue>.OutgoingChange(true, value));
      this.removedEvent(value);
      return true;
    }));
    if (num <= 0)
      return num;
    this.MarkDirty();
    return num;
  }

  /// <inheritdoc />
  public bool SetEquals(IEnumerable<TValue> other) => this.Set.SetEquals(other);

  /// <inheritdoc />
  public void SymmetricExceptWith(IEnumerable<TValue> other) => this.Set.SymmetricExceptWith(other);

  /// <inheritdoc />
  public void UnionWith(IEnumerable<TValue> other) => this.Set.UnionWith(other);

  /// <inheritdoc />
  void ICollection<TValue>.Add(TValue item) => this.Add(item);

  /// <inheritdoc />
  IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.Set.GetEnumerator();

  protected override bool tickImpl()
  {
    List<NetHashSet<TValue>.IncomingChange> incomingChangeList = (List<NetHashSet<TValue>.IncomingChange>) null;
    foreach (NetHashSet<TValue>.IncomingChange incomingChange in this.IncomingChanges)
    {
      if (this.Root != null)
      {
        if (this.GetLocalTick() < incomingChange.Tick)
          break;
      }
      if (incomingChangeList == null)
        incomingChangeList = new List<NetHashSet<TValue>.IncomingChange>();
      incomingChangeList.Add(incomingChange);
    }
    if (incomingChangeList != null)
    {
      foreach (NetHashSet<TValue>.IncomingChange incomingChange in incomingChangeList)
        this.IncomingChanges.Remove(incomingChange);
      foreach (NetHashSet<TValue>.IncomingChange incomingChange in incomingChangeList)
      {
        if (incomingChange.Removal)
        {
          if (this.Set.Remove(incomingChange.Value))
            this.removedEvent(incomingChange.Value);
        }
        else if (this.Set.Add(incomingChange.Value))
          this.addedEvent(incomingChange.Value);
      }
    }
    return this.IncomingChanges.Count > 0;
  }

  private void removedEvent(TValue value)
  {
    NetHashSet<TValue>.ContentsChangeEvent onValueRemoved = this.OnValueRemoved;
    if (onValueRemoved == null)
      return;
    onValueRemoved(value);
  }

  private void addedEvent(TValue value)
  {
    NetHashSet<TValue>.ContentsChangeEvent onValueAdded = this.OnValueAdded;
    if (onValueAdded == null)
      return;
    onValueAdded(value);
  }

  /// <inheritdoc />
  public override bool Equals(object obj) => obj is NetHashSet<TValue> other && this.Equals(other);

  /// <inheritdoc />
  public override void Read(BinaryReader reader, NetVersion version)
  {
    uint tick = this.GetLocalTick() + (!this.InterpolationWait || this.Root == null ? 0U : (uint) this.Root.Clock.InterpolationTicks);
    uint num = reader.Read7BitEncoded();
    for (uint index = 0; index < num; ++index)
    {
      bool removal = reader.ReadBoolean();
      TValue obj = this.ReadValue(reader);
      this.IncomingChanges.Add(new NetHashSet<TValue>.IncomingChange(tick, removal, obj));
      this.NeedsTick = true;
    }
  }

  /// <inheritdoc />
  public override void Write(BinaryWriter writer)
  {
    writer.Write7BitEncoded((uint) this.OutgoingChanges.Count);
    foreach (NetHashSet<TValue>.OutgoingChange outgoingChange in this.OutgoingChanges)
    {
      writer.Write(outgoingChange.Removal);
      this.WriteValue(writer, outgoingChange.Value);
    }
  }

  /// <inheritdoc />
  public override void ReadFull(BinaryReader reader, NetVersion version)
  {
    this.Set.Clear();
    int capacity = reader.ReadInt32();
    this.Set.EnsureCapacity(capacity);
    for (int index = 0; index < capacity; ++index)
    {
      TValue obj = this.ReadValue(reader);
      this.Set.Add(obj);
      this.addedEvent(obj);
    }
  }

  /// <inheritdoc />
  public override void WriteFull(BinaryWriter writer)
  {
    writer.Write(this.Set.Count);
    foreach (TValue obj in this.Set)
      this.WriteValue(writer, obj);
  }

  public override int GetHashCode() => this.Set.GetHashCode();

  public abstract TValue ReadValue(BinaryReader reader);

  public abstract void WriteValue(BinaryWriter writer, TValue value);

  protected override void CleanImpl()
  {
    base.CleanImpl();
    this.OutgoingChanges.Clear();
  }

  public class IncomingChange
  {
    public uint Tick;
    public bool Removal;
    public TValue Value;

    public IncomingChange(uint tick, bool removal, TValue value)
    {
      this.Tick = tick;
      this.Removal = removal;
      this.Value = value;
    }
  }

  public class OutgoingChange
  {
    public bool Removal;
    public TValue Value;

    public OutgoingChange(bool removal, TValue value)
    {
      this.Removal = removal;
      this.Value = value;
    }
  }

  public delegate void ContentsChangeEvent(TValue value);
}
