// Decompiled with JetBrains decompiler
// Type: Netcode.NetDictionary`5
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

#nullable enable
namespace Netcode;

public abstract class NetDictionary<TKey, TValue, TField, TSerialDict, TSelf> : 
  AbstractNetSerializable,
  IEquatable<
  #nullable disable
  TSelf>,
  IEnumerable<TSerialDict>,
  IEnumerable
  where TField : class, INetObject<INetSerializable>, new()
  where TSerialDict : IDictionary<TKey, TValue>, new()
  where TSelf : NetDictionary<TKey, TValue, TField, TSerialDict, TSelf>
{
  public bool InterpolationWait = true;
  private Dictionary<TKey, TField> dict = new Dictionary<TKey, TField>();
  private Dictionary<TKey, NetVersion> dictReassigns = new Dictionary<TKey, NetVersion>();
  private List<NetDictionary<TKey, TValue, TField, TSerialDict, TSelf>.OutgoingChange> outgoingChanges = new List<NetDictionary<TKey, TValue, TField, TSerialDict, TSelf>.OutgoingChange>();
  private List<NetDictionary<TKey, TValue, TField, TSerialDict, TSelf>.IncomingChange> incomingChanges = new List<NetDictionary<TKey, TValue, TField, TSerialDict, TSelf>.IncomingChange>();

  /// <summary>The number of key/value pairs in the dictionary.</summary>
  public int Length => this.dict.Count;

  public bool Any() => this.dict.Count > 0;

  public bool IsReadOnly => false;

  public event NetDictionary<TKey, TValue, TField, TSerialDict, TSelf>.ContentsChangeEvent OnValueAdded;

  public event NetDictionary<TKey, TValue, TField, TSerialDict, TSelf>.ContentsChangeEvent OnValueRemoved;

  public event NetDictionary<TKey, TValue, TField, TSerialDict, TSelf>.ContentsUpdateEvent OnValueTargetUpdated;

  public event NetDictionary<TKey, TValue, TField, TSerialDict, TSelf>.ConflictResolveEvent OnConflictResolve;

  public TValue this[TKey key]
  {
    get => this.getFieldValue(this.dict[key]);
    set
    {
      TField field1;
      if (!this.dict.TryGetValue(key, out field1))
      {
        TField field2;
        this.dict[key] = field2 = new TField();
        this.dictReassigns[key] = this.GetLocalVersion();
        this.setFieldValue(field2, key, value);
        this.added(key, field2, this.dictReassigns[key]);
      }
      else
      {
        this.setFieldValue(field1, key, value);
        this.addedEvent(key, field1);
      }
    }
  }

  public NetDictionary<TKey, TValue, TField, TSerialDict, TSelf>.KeysCollection Keys
  {
    get => new NetDictionary<TKey, TValue, TField, TSerialDict, TSelf>.KeysCollection(this.dict);
  }

  public NetDictionary<TKey, TValue, TField, TSerialDict, TSelf>.ValuesCollection Values
  {
    get => new NetDictionary<TKey, TValue, TField, TSerialDict, TSelf>.ValuesCollection(this);
  }

  public NetDictionary<TKey, TValue, TField, TSerialDict, TSelf>.PairsCollection Pairs
  {
    get => new NetDictionary<TKey, TValue, TField, TSerialDict, TSelf>.PairsCollection(this);
  }

  public Dictionary<TKey, TField> FieldDict => this.dict;

  public NetDictionary()
  {
  }

  public NetDictionary(IEnumerable<KeyValuePair<TKey, TValue>> dict)
    : this()
  {
    this.CopyFrom(dict);
  }

  protected override bool tickImpl()
  {
    List<NetDictionary<TKey, TValue, TField, TSerialDict, TSelf>.IncomingChange> incomingChangeList = (List<NetDictionary<TKey, TValue, TField, TSerialDict, TSelf>.IncomingChange>) null;
    foreach (NetDictionary<TKey, TValue, TField, TSerialDict, TSelf>.IncomingChange incomingChange in this.incomingChanges)
    {
      if (this.Root != null)
      {
        if (this.GetLocalTick() < incomingChange.Tick)
          break;
      }
      if (incomingChangeList == null)
        incomingChangeList = new List<NetDictionary<TKey, TValue, TField, TSerialDict, TSelf>.IncomingChange>();
      incomingChangeList.Add(incomingChange);
    }
    // ISSUE: explicit non-virtual call
    if (incomingChangeList != null && __nonvirtual (incomingChangeList.Count) > 0)
    {
      foreach (NetDictionary<TKey, TValue, TField, TSerialDict, TSelf>.IncomingChange incomingChange in incomingChangeList)
        this.incomingChanges.Remove(incomingChange);
      foreach (NetDictionary<TKey, TValue, TField, TSerialDict, TSelf>.IncomingChange incomingChange in incomingChangeList)
      {
        if (incomingChange.Removal)
          this.performIncomingRemove(incomingChange);
        else
          this.performIncomingAdd(incomingChange);
      }
    }
    return this.incomingChanges.Count > 0;
  }

  protected abstract void setFieldValue(TField field, TKey key, TValue value);

  protected abstract TValue getFieldValue(TField field);

  protected abstract TValue getFieldTargetValue(TField field);

  protected TField createField(TKey key, TValue value)
  {
    TField field = new TField();
    this.setFieldValue(field, key, value);
    return field;
  }

  public void CopyFrom(IEnumerable<KeyValuePair<TKey, TValue>> dict)
  {
    foreach (KeyValuePair<TKey, TValue> keyValuePair in dict)
      this[keyValuePair.Key] = keyValuePair.Value;
  }

  public void Set(IEnumerable<KeyValuePair<TKey, TValue>> dict)
  {
    this.Clear();
    this.CopyFrom(dict);
  }

  public void MoveFrom(TSelf dict)
  {
    List<KeyValuePair<TKey, TValue>> dict1 = new List<KeyValuePair<TKey, TValue>>((IEnumerable<KeyValuePair<TKey, TValue>>) dict.Pairs);
    dict.Clear();
    this.Set((IEnumerable<KeyValuePair<TKey, TValue>>) dict1);
  }

  public void SetEqualityComparer(IEqualityComparer<TKey> comparer)
  {
    this.dict = new Dictionary<TKey, TField>((IDictionary<TKey, TField>) this.dict, comparer);
    this.dictReassigns = new Dictionary<TKey, NetVersion>((IDictionary<TKey, NetVersion>) this.dictReassigns, comparer);
  }

  private void setFieldParent(TField arg)
  {
    INetObject<INetSerializable> netObject = (INetObject<INetSerializable>) arg;
    if (this.Parent == null)
      return;
    netObject.NetFields.Parent = (INetSerializable) this;
  }

  private void added(TKey key, TField field, NetVersion reassign)
  {
    this.outgoingChanges.Add(new NetDictionary<TKey, TValue, TField, TSerialDict, TSelf>.OutgoingChange(false, key, field, reassign));
    this.setFieldParent(field);
    this.MarkDirty();
    this.addedEvent(key, field);
    foreach (NetDictionary<TKey, TValue, TField, TSerialDict, TSelf>.IncomingChange incomingChange in this.incomingChanges)
    {
      if (!incomingChange.Removal && object.Equals((object) incomingChange.Key, (object) key))
      {
        this.clearFieldParent(incomingChange.Field);
        if (this.OnConflictResolve != null)
          this.OnConflictResolve(key, incomingChange.Field, field);
      }
    }
    this.incomingChanges.RemoveAll((Predicate<NetDictionary<TKey, TValue, TField, TSerialDict, TSelf>.IncomingChange>) (change => object.Equals((object) key, (object) change.Key)));
  }

  private void addedEvent(TKey key, TField field)
  {
    if (this.OnValueAdded == null)
      return;
    this.OnValueAdded(key, this.getFieldValue(field));
  }

  private void updatedEvent(TKey key, TValue old_target_value, TValue new_target_value)
  {
    if (this.OnValueTargetUpdated == null)
      return;
    this.OnValueTargetUpdated(key, old_target_value, new_target_value);
  }

  private void clearFieldParent(TField arg)
  {
    INetObject<INetSerializable> netObject = (INetObject<INetSerializable>) arg;
    if (netObject.NetFields.Parent != this)
      return;
    netObject.NetFields.Parent = (INetSerializable) null;
  }

  private void removed(TKey key, TField field, NetVersion reassign)
  {
    this.outgoingChanges.Add(new NetDictionary<TKey, TValue, TField, TSerialDict, TSelf>.OutgoingChange(true, key, field, reassign));
    this.clearFieldParent(field);
    this.MarkDirty();
    this.removedEvent(key, field);
  }

  private void removedEvent(TKey key, TField field)
  {
    if (this.OnValueRemoved == null)
      return;
    this.OnValueRemoved(key, this.getFieldValue(field));
  }

  /// <summary>Add an entry to the dictionary.</summary>
  /// <param name="key">The key of the element to add.</param>
  /// <param name="value">The value of the element to add.</param>
  /// <exception cref="T:System.ArgumentException">The key is already present in the dictionary.</exception>
  public void Add(TKey key, TValue value)
  {
    TField field = this.createField(key, value);
    this.Add(key, field);
  }

  /// <summary>Add an entry to the dictionary.</summary>
  /// <param name="key">The key of the element to add.</param>
  /// <param name="field">The net field to add.</param>
  /// <exception cref="T:System.ArgumentException">The key is already present in the dictionary.</exception>
  public void Add(TKey key, TField field)
  {
    this.dict.Add(key, field);
    this.dictReassigns.Add(key, this.GetLocalVersion());
    this.added(key, field, this.dictReassigns[key]);
  }

  /// <summary>Add an entry to the dictionary if the key isn't already present.</summary>
  /// <param name="key">The key of the element to add.</param>
  /// <param name="value">The value of the element to add.</param>
  /// <returns>Returns whether the value was successfully added.</returns>
  public bool TryAdd(TKey key, TValue value)
  {
    if (this.dict.ContainsKey(key))
      return false;
    TField field = this.createField(key, value);
    this.Add(key, field);
    return true;
  }

  public void Clear()
  {
    NetDictionary<TKey, TValue, TField, TSerialDict, TSelf>.KeysCollection keys = this.Keys;
    while (keys.Any())
      this.Remove(keys.First());
    this.outgoingChanges.RemoveAll((Predicate<NetDictionary<TKey, TValue, TField, TSerialDict, TSelf>.OutgoingChange>) (ch => !ch.Removal));
  }

  public bool ContainsKey(TKey key) => this.dict.ContainsKey(key);

  public int Count() => this.dict.Count;

  public bool Remove(TKey key)
  {
    TField field;
    if (!this.dict.TryGetValue(key, out field))
      return false;
    NetVersion dictReassign = this.dictReassigns[key];
    this.dict.Remove(key);
    this.dictReassigns.Remove(key);
    this.removed(key, field, dictReassign);
    return true;
  }

  /// <summary>Remove all elements that match a condition.</summary>
  /// <param name="match">The predicate matching values to remove.</param>
  public int RemoveWhere(Func<KeyValuePair<TKey, TValue>, bool> match)
  {
    if (this.dict.Count == 0)
      return 0;
    int num = 0;
    foreach (KeyValuePair<TKey, TValue> pair in this.Pairs)
    {
      if (match(pair))
      {
        this.Remove(pair.Key);
        ++num;
      }
    }
    return num;
  }

  [Obsolete("Use RemoveWhere instead.")]
  public void Filter(Func<KeyValuePair<TKey, TValue>, bool> f)
  {
    this.RemoveWhere((Func<KeyValuePair<TKey, TValue>, bool>) (pair => !f(pair)));
  }

  /// <summary>Try to get the value associated with a specified key.</summary>
  /// <param name="key">The key of the element to find.</param>
  /// <param name="value">The value that was found, or the default value if none was found.</param>
  /// <returns>Returns whether a value was found.</returns>
  public bool TryGetValue(TKey key, out TValue value)
  {
    TField field;
    if (this.dict.TryGetValue(key, out field))
    {
      value = this.getFieldValue(field);
      return true;
    }
    value = default (TValue);
    return false;
  }

  /// <summary>Get the value associated with a specified key, or the default value if none was found.</summary>
  /// <param name="key">The key of the element to find.</param>
  /// <param name="defaultValue">The value to return if the element isn't found.</param>
  public TValue GetValueOrDefault(TKey key, TValue defaultValue = null)
  {
    TField field;
    return !this.dict.TryGetValue(key, out field) ? defaultValue : this.getFieldValue(field);
  }

  public bool Equals(TSelf other) => object.Equals((object) this.dict, (object) other.dict);

  protected override void CleanImpl()
  {
    base.CleanImpl();
    this.outgoingChanges.Clear();
  }

  protected abstract TKey ReadKey(BinaryReader reader);

  protected abstract void WriteKey(BinaryWriter writer, TKey key);

  private void readMultiple(
    NetDictionary<TKey, TValue, TField, TSerialDict, TSelf>.ReadFunc readFunc,
    BinaryReader reader,
    NetVersion version)
  {
    uint num = reader.Read7BitEncoded();
    for (uint index = 0; index < num; ++index)
      readFunc(reader, version);
  }

  private void writeMultiple<T>(
    NetDictionary<TKey, TValue, TField, TSerialDict, TSelf>.WriteFunc<T> writeFunc,
    BinaryWriter writer,
    IEnumerable<T> values)
  {
    writer.Write7BitEncoded((uint) values.Count<T>());
    foreach (T obj in values)
      writeFunc(writer, obj);
  }

  protected virtual TField ReadFieldFull(BinaryReader reader, NetVersion version)
  {
    TField field = new TField();
    field.NetFields.ReadFull(reader, version);
    return field;
  }

  protected virtual void WriteFieldFull(BinaryWriter writer, TField field)
  {
    field.NetFields.WriteFull(writer);
  }

  private void readAddition(BinaryReader reader, NetVersion version)
  {
    TKey key = this.ReadKey(reader);
    NetVersion fieldReassign = new NetVersion();
    fieldReassign.Read(reader);
    TField field = this.ReadFieldFull(reader, version);
    this.setFieldParent(field);
    this.queueIncomingChange(false, key, field, fieldReassign);
  }

  protected virtual bool resolveConflict(
    TKey key,
    TField currentField,
    NetVersion currentReassign,
    TField incomingField,
    NetVersion incomingReassign)
  {
    if (incomingReassign.IsPriorityOver(currentReassign))
    {
      this.clearFieldParent(currentField);
      if (this.OnConflictResolve != null)
        this.OnConflictResolve(key, currentField, incomingField);
      return true;
    }
    this.clearFieldParent(incomingField);
    if (this.OnConflictResolve != null)
      this.OnConflictResolve(key, incomingField, currentField);
    return false;
  }

  private KeyValuePair<NetVersion, TField>? findConflict(TKey key)
  {
    foreach (NetDictionary<TKey, TValue, TField, TSerialDict, TSelf>.IncomingChange incomingChange in this.incomingChanges.AsEnumerable<NetDictionary<TKey, TValue, TField, TSerialDict, TSelf>.IncomingChange>().Reverse<NetDictionary<TKey, TValue, TField, TSerialDict, TSelf>.IncomingChange>())
    {
      if (object.Equals((object) incomingChange.Key, (object) key))
        return incomingChange.Removal ? new KeyValuePair<NetVersion, TField>?() : new KeyValuePair<NetVersion, TField>?(new KeyValuePair<NetVersion, TField>(incomingChange.Reassigned, incomingChange.Field));
    }
    TField field;
    return this.dict.TryGetValue(key, out field) ? new KeyValuePair<NetVersion, TField>?(new KeyValuePair<NetVersion, TField>(this.dictReassigns[key], field)) : new KeyValuePair<NetVersion, TField>?();
  }

  private void queueIncomingChange(bool removal, TKey key, TField field, NetVersion fieldReassign)
  {
    if (!removal)
    {
      KeyValuePair<NetVersion, TField>? conflict = this.findConflict(key);
      if (conflict.HasValue)
      {
        TKey key1 = key;
        KeyValuePair<NetVersion, TField> keyValuePair = conflict.Value;
        TField currentField = keyValuePair.Value;
        keyValuePair = conflict.Value;
        NetVersion key2 = keyValuePair.Key;
        TField incomingField = field;
        NetVersion incomingReassign = fieldReassign;
        if (!this.resolveConflict(key1, currentField, key2, incomingField, incomingReassign))
          return;
      }
    }
    this.incomingChanges.Add(new NetDictionary<TKey, TValue, TField, TSerialDict, TSelf>.IncomingChange(this.GetLocalTick() + (!this.InterpolationWait || this.Root == null ? 0U : (uint) this.Root.Clock.InterpolationTicks), removal, key, field, fieldReassign));
    this.NeedsTick = true;
  }

  private void performIncomingAdd(
    NetDictionary<TKey, TValue, TField, TSerialDict, TSelf>.IncomingChange add)
  {
    this.dict[add.Key] = add.Field;
    this.dictReassigns[add.Key] = add.Reassigned;
    this.addedEvent(add.Key, add.Field);
  }

  private void readRemoval(BinaryReader reader, NetVersion version)
  {
    TKey key = this.ReadKey(reader);
    NetVersion fieldReassign = new NetVersion();
    fieldReassign.Read(reader);
    this.queueIncomingChange(true, key, default (TField), fieldReassign);
  }

  private void readDictChange(BinaryReader reader, NetVersion version)
  {
    if (reader.ReadByte() != (byte) 0)
      this.readRemoval(reader, version);
    else
      this.readAddition(reader, version);
  }

  private void performIncomingRemove(
    NetDictionary<TKey, TValue, TField, TSerialDict, TSelf>.IncomingChange remove)
  {
    TField field;
    if (!this.dict.TryGetValue(remove.Key, out field))
      return;
    this.clearFieldParent(field);
    this.dict.Remove(remove.Key);
    this.dictReassigns.Remove(remove.Key);
    this.removedEvent(remove.Key, field);
  }

  private void readUpdate(BinaryReader reader, NetVersion version)
  {
    TKey key = this.ReadKey(reader);
    NetVersion reassign = new NetVersion();
    reassign.Read(reader);
    reader.ReadSkippable((Action) (() =>
    {
      int lastIndex = this.incomingChanges.FindLastIndex((Predicate<NetDictionary<TKey, TValue, TField, TSerialDict, TSelf>.IncomingChange>) (ch => !ch.Removal && object.Equals((object) ch.Key, (object) key) && reassign.Equals(ch.Reassigned)));
      if (lastIndex != -1)
      {
        TField field = this.incomingChanges[lastIndex].Field;
        if (this.OnValueTargetUpdated != null)
        {
          TValue fieldTargetValue = this.getFieldTargetValue(field);
          field.NetFields.Read(reader, version);
          this.updatedEvent(key, fieldTargetValue, this.getFieldTargetValue(field));
        }
        else
          field.NetFields.Read(reader, version);
      }
      else
      {
        TField field;
        if (!this.dict.TryGetValue(key, out field) || !this.dictReassigns[key].Equals(reassign))
          return;
        if (this.OnValueTargetUpdated != null)
        {
          TValue fieldTargetValue = this.getFieldTargetValue(field);
          field.NetFields.Read(reader, version);
          this.updatedEvent(key, fieldTargetValue, this.getFieldTargetValue(field));
        }
        else
          field.NetFields.Read(reader, version);
      }
    }));
  }

  public override void Read(BinaryReader reader, NetVersion version)
  {
    this.readMultiple(new NetDictionary<TKey, TValue, TField, TSerialDict, TSelf>.ReadFunc(this.readDictChange), reader, version);
    this.readMultiple(new NetDictionary<TKey, TValue, TField, TSerialDict, TSelf>.ReadFunc(this.readUpdate), reader, version);
  }

  public override void ReadFull(BinaryReader reader, NetVersion version)
  {
    this.dict.Clear();
    this.dictReassigns.Clear();
    this.outgoingChanges.Clear();
    this.incomingChanges.Clear();
    int num = reader.ReadInt32();
    for (int index = 0; index < num; ++index)
    {
      TKey key = this.ReadKey(reader);
      NetVersion netVersion = new NetVersion();
      netVersion.Read(reader);
      TField field = this.ReadFieldFull(reader, version);
      this.dict.Add(key, field);
      this.dictReassigns.Add(key, netVersion);
      this.setFieldParent(field);
      this.addedEvent(key, field);
    }
  }

  private void writeAddition(
    BinaryWriter writer,
    NetDictionary<TKey, TValue, TField, TSerialDict, TSelf>.OutgoingChange update)
  {
    this.WriteKey(writer, update.Key);
    update.Reassigned.Write(writer);
    this.WriteFieldFull(writer, update.Field);
  }

  private void writeRemoval(
    BinaryWriter writer,
    NetDictionary<TKey, TValue, TField, TSerialDict, TSelf>.OutgoingChange update)
  {
    this.WriteKey(writer, update.Key);
    update.Reassigned.Write(writer);
  }

  private void writeDictChange(
    BinaryWriter writer,
    NetDictionary<TKey, TValue, TField, TSerialDict, TSelf>.OutgoingChange ch)
  {
    if (ch.Removal)
    {
      writer.Write((byte) 1);
      this.writeRemoval(writer, ch);
    }
    else
    {
      writer.Write((byte) 0);
      this.writeAddition(writer, ch);
    }
  }

  private void writeUpdate(
    BinaryWriter writer,
    NetDictionary<TKey, TValue, TField, TSerialDict, TSelf>.OutgoingChange update)
  {
    this.WriteKey(writer, update.Key);
    update.Reassigned.Write(writer);
    writer.WriteSkippable((Action) (() => update.Field.NetFields.Write(writer)));
  }

  private IEnumerable<NetDictionary<TKey, TValue, TField, TSerialDict, TSelf>.OutgoingChange> updates()
  {
    foreach (KeyValuePair<TKey, TField> keyValuePair in this.dict)
    {
      if (keyValuePair.Value.NetFields.Dirty)
        yield return new NetDictionary<TKey, TValue, TField, TSerialDict, TSelf>.OutgoingChange(false, keyValuePair.Key, keyValuePair.Value, this.dictReassigns[keyValuePair.Key]);
    }
    foreach (NetDictionary<TKey, TValue, TField, TSerialDict, TSelf>.OutgoingChange outgoingChange in this.outgoingChanges.Where<NetDictionary<TKey, TValue, TField, TSerialDict, TSelf>.OutgoingChange>((Func<NetDictionary<TKey, TValue, TField, TSerialDict, TSelf>.OutgoingChange, bool>) (ch => ch.Removal)))
    {
      if (outgoingChange.Field.NetFields.Dirty)
        yield return outgoingChange;
    }
  }

  public override void Write(BinaryWriter writer)
  {
    this.writeMultiple<NetDictionary<TKey, TValue, TField, TSerialDict, TSelf>.OutgoingChange>(new NetDictionary<TKey, TValue, TField, TSerialDict, TSelf>.WriteFunc<NetDictionary<TKey, TValue, TField, TSerialDict, TSelf>.OutgoingChange>(this.writeDictChange), writer, (IEnumerable<NetDictionary<TKey, TValue, TField, TSerialDict, TSelf>.OutgoingChange>) this.outgoingChanges);
    this.writeMultiple<NetDictionary<TKey, TValue, TField, TSerialDict, TSelf>.OutgoingChange>(new NetDictionary<TKey, TValue, TField, TSerialDict, TSelf>.WriteFunc<NetDictionary<TKey, TValue, TField, TSerialDict, TSelf>.OutgoingChange>(this.writeUpdate), writer, this.updates());
  }

  public override void WriteFull(BinaryWriter writer)
  {
    writer.Write(this.Length);
    foreach (TKey key in this.dict.Keys)
    {
      this.WriteKey(writer, key);
      this.dictReassigns[key].Write(writer);
      this.WriteFieldFull(writer, this.dict[key]);
    }
  }

  public IEnumerator<TSerialDict> GetEnumerator()
  {
    TSerialDict serialDict1 = new TSerialDict();
    foreach (KeyValuePair<TKey, TField> keyValuePair in this.dict)
    {
      ref TSerialDict local = ref serialDict1;
      TSerialDict serialDict2 = default (TSerialDict);
      if ((object) serialDict2 == null)
      {
        serialDict2 = local;
        local = ref serialDict2;
      }
      TKey key = keyValuePair.Key;
      TValue fieldValue = this.getFieldValue(keyValuePair.Value);
      local.Add(key, fieldValue);
    }
    return (IEnumerator<TSerialDict>) new List<TSerialDict>()
    {
      serialDict1
    }.GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

  protected override void ForEachChild(Action<INetSerializable> childAction)
  {
    foreach (NetDictionary<TKey, TValue, TField, TSerialDict, TSelf>.IncomingChange incomingChange in this.incomingChanges)
    {
      if ((object) incomingChange.Field != null)
        childAction(incomingChange.Field.NetFields);
    }
    foreach (TField field in this.dict.Values)
      childAction(field.NetFields);
  }

  public void Add(TSerialDict dict) => this.Set((IEnumerable<KeyValuePair<TKey, TValue>>) dict);

  protected override void ValidateChildren()
  {
    if (this.Parent == null && this.Root != this || this.NeedsTick)
      return;
    this.ForEachChild(new Action<INetSerializable>(((AbstractNetSerializable) this).ValidateChild));
  }

  public class IncomingChange
  {
    public uint Tick;
    public bool Removal;
    public TKey Key;
    public TField Field;
    public NetVersion Reassigned;

    public IncomingChange(uint tick, bool removal, TKey key, TField field, NetVersion reassigned)
    {
      this.Tick = tick;
      this.Removal = removal;
      this.Key = key;
      this.Field = field;
      this.Reassigned = reassigned;
    }
  }

  public class OutgoingChange
  {
    public bool Removal;
    public TKey Key;
    public TField Field;
    public NetVersion Reassigned;

    public OutgoingChange(bool removal, TKey key, TField field, NetVersion reassigned)
    {
      this.Removal = removal;
      this.Key = key;
      this.Field = field;
      this.Reassigned = reassigned;
    }
  }

  public delegate void ContentsChangeEvent(TKey key, TValue value)
    where TField : class, INetObject<INetSerializable>, new()
    where TSerialDict : IDictionary<TKey, TValue>, new()
    where TSelf : NetDictionary<TKey, TValue, TField, TSerialDict, TSelf>;

  public delegate void ConflictResolveEvent(TKey key, TField rejected, TField accepted)
    where TField : class, INetObject<INetSerializable>, new()
    where TSerialDict : IDictionary<TKey, TValue>, new()
    where TSelf : NetDictionary<TKey, TValue, TField, TSerialDict, TSelf>;

  public delegate void ContentsUpdateEvent(
    TKey key,
    TValue old_target_value,
    TValue new_target_value)
    where TField : class, INetObject<INetSerializable>, new()
    where TSerialDict : IDictionary<TKey, TValue>, new()
    where TSelf : NetDictionary<TKey, TValue, TField, TSerialDict, TSelf>;

  private delegate void ReadFunc(BinaryReader reader, NetVersion version)
    where TField : class, INetObject<INetSerializable>, new()
    where TSerialDict : IDictionary<TKey, TValue>, new()
    where TSelf : NetDictionary<TKey, TValue, TField, TSerialDict, TSelf>;

  private delegate void WriteFunc<T>(BinaryWriter writer, T value)
    where TField : class, INetObject<INetSerializable>, new()
    where TSerialDict : IDictionary<TKey, TValue>, new()
    where TSelf : NetDictionary<TKey, TValue, TField, TSerialDict, TSelf>;

  public struct PairsCollection(
    NetDictionary<TKey, TValue, TField, TSerialDict, TSelf> net) : 
    IEnumerable<KeyValuePair<TKey, TValue>>,
    IEnumerable
  {
    private NetDictionary<TKey, TValue, TField, TSerialDict, TSelf> _net = net;

    public int Count() => this._net.dict.Count;

    public KeyValuePair<TKey, TValue> ElementAt(int index)
    {
      int num = 0;
      foreach (KeyValuePair<TKey, TValue> keyValuePair in this)
      {
        if (num == index)
          return keyValuePair;
        ++num;
      }
      throw new ArgumentOutOfRangeException();
    }

    public NetDictionary<TKey, TValue, TField, TSerialDict, TSelf>.PairsCollection.Enumerator GetEnumerator()
    {
      return new NetDictionary<TKey, TValue, TField, TSerialDict, TSelf>.PairsCollection.Enumerator(this._net);
    }

    IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
    {
      return (IEnumerator<KeyValuePair<TKey, TValue>>) new NetDictionary<TKey, TValue, TField, TSerialDict, TSelf>.PairsCollection.Enumerator(this._net);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return (IEnumerator) new NetDictionary<TKey, TValue, TField, TSerialDict, TSelf>.PairsCollection.Enumerator(this._net);
    }

    public struct Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>, IEnumerator, IDisposable
    {
      private readonly NetDictionary<TKey, TValue, TField, TSerialDict, TSelf> _net;
      private Dictionary<TKey, TField>.Enumerator _enumerator;
      private KeyValuePair<TKey, TValue> _current;
      private bool _done;

      public Enumerator(
        NetDictionary<TKey, TValue, TField, TSerialDict, TSelf> net)
      {
        this._net = net;
        this._enumerator = this._net.dict.GetEnumerator();
        this._current = new KeyValuePair<TKey, TValue>();
        this._done = false;
      }

      public bool MoveNext()
      {
        if (this._enumerator.MoveNext())
        {
          KeyValuePair<TKey, TField> current = this._enumerator.Current;
          this._current = new KeyValuePair<TKey, TValue>(current.Key, this._net.getFieldValue(current.Value));
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
        this._enumerator = this._net.dict.GetEnumerator();
        this._current = new KeyValuePair<TKey, TValue>();
        this._done = false;
      }
    }
  }

  public struct ValuesCollection(
    NetDictionary<TKey, TValue, TField, TSerialDict, TSelf> net) : IEnumerable<TValue>, IEnumerable
  {
    private NetDictionary<TKey, TValue, TField, TSerialDict, TSelf> _net = net;

    public NetDictionary<TKey, TValue, TField, TSerialDict, TSelf>.ValuesCollection.Enumerator GetEnumerator()
    {
      return new NetDictionary<TKey, TValue, TField, TSerialDict, TSelf>.ValuesCollection.Enumerator(this._net);
    }

    IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator()
    {
      return (IEnumerator<TValue>) new NetDictionary<TKey, TValue, TField, TSerialDict, TSelf>.ValuesCollection.Enumerator(this._net);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return (IEnumerator) new NetDictionary<TKey, TValue, TField, TSerialDict, TSelf>.ValuesCollection.Enumerator(this._net);
    }

    public struct Enumerator : IEnumerator<TValue>, IEnumerator, IDisposable
    {
      private readonly NetDictionary<TKey, TValue, TField, TSerialDict, TSelf> _net;
      private Dictionary<TKey, TField>.Enumerator _enumerator;
      private TValue _current;
      private bool _done;

      public Enumerator(
        NetDictionary<TKey, TValue, TField, TSerialDict, TSelf> net)
      {
        this._net = net;
        this._enumerator = this._net.dict.GetEnumerator();
        this._current = default (TValue);
        this._done = false;
      }

      public bool MoveNext()
      {
        if (this._enumerator.MoveNext())
        {
          this._current = this._net.getFieldValue(this._enumerator.Current.Value);
          return true;
        }
        this._done = true;
        this._current = default (TValue);
        return false;
      }

      public TValue Current => this._current;

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
        this._enumerator = this._net.dict.GetEnumerator();
        this._current = default (TValue);
        this._done = false;
      }
    }
  }

  public struct KeysCollection(Dictionary<TKey, TField> dict) : IEnumerable<TKey>, IEnumerable
  {
    private Dictionary<TKey, TField> _dict = dict;

    public bool Any() => this._dict.Count > 0;

    public TKey First()
    {
      using (Dictionary<TKey, TField>.Enumerator enumerator = this._dict.GetEnumerator())
      {
        if (enumerator.MoveNext())
          return enumerator.Current.Key;
      }
      return default (TKey);
    }

    public bool Contains(TKey key) => this._dict.ContainsKey(key);

    public NetDictionary<TKey, TValue, TField, TSerialDict, TSelf>.KeysCollection.Enumerator GetEnumerator()
    {
      return new NetDictionary<TKey, TValue, TField, TSerialDict, TSelf>.KeysCollection.Enumerator(this._dict);
    }

    IEnumerator<TKey> IEnumerable<TKey>.GetEnumerator()
    {
      return (IEnumerator<TKey>) new NetDictionary<TKey, TValue, TField, TSerialDict, TSelf>.KeysCollection.Enumerator(this._dict);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return (IEnumerator) new NetDictionary<TKey, TValue, TField, TSerialDict, TSelf>.KeysCollection.Enumerator(this._dict);
    }

    public struct Enumerator : IEnumerator<TKey>, IEnumerator, IDisposable
    {
      private readonly Dictionary<TKey, TField> _dict;
      private Dictionary<TKey, TField>.Enumerator _enumerator;
      private TKey _current;
      private bool _done;

      public Enumerator(Dictionary<TKey, TField> dict)
      {
        this._dict = dict;
        this._enumerator = this._dict.GetEnumerator();
        this._current = default (TKey);
        this._done = false;
      }

      public bool MoveNext()
      {
        if (this._enumerator.MoveNext())
        {
          this._current = this._enumerator.Current.Key;
          return true;
        }
        this._done = true;
        this._current = default (TKey);
        return false;
      }

      public TKey Current => this._current;

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
        this._enumerator = this._dict.GetEnumerator();
        this._current = default (TKey);
        this._done = false;
      }
    }
  }
}
