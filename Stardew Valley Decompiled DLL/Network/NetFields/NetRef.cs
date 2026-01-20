// Decompiled with JetBrains decompiler
// Type: Netcode.NetRefBase`2
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using StardewValley.SaveSerialization;
using System;
using System.IO;
using System.Xml.Serialization;

#nullable disable
namespace Netcode;

public abstract class NetRefBase<T, TSelf> : NetField<T, TSelf>
  where T : class
  where TSelf : NetRefBase<T, TSelf>
{
  public XmlSerializer Serializer;
  private NetRefBase<T, TSelf>.RefDeltaType deltaType;
  protected NetVersion reassigned;

  public event NetRefBase<T, TSelf>.ConflictResolveEvent OnConflictResolve;

  public NetRefBase()
  {
  }

  public NetRefBase(T value)
    : this()
  {
    this.cleanSet(value);
  }

  protected override void SetParent(INetSerializable parent)
  {
    if (parent == null || parent.Root != this.Root)
      this.reassigned.Clear();
    base.SetParent(parent);
  }

  protected override void CleanImpl()
  {
    base.CleanImpl();
    this.deltaType = NetRefBase<T, TSelf>.RefDeltaType.ChildDelta;
  }

  public void MarkReassigned()
  {
    this.deltaType = NetRefBase<T, TSelf>.RefDeltaType.Reassigned;
    if (this.Root != null)
      this.reassigned.Set(this.Root.Clock.netVersion);
    this.MarkDirty();
  }

  public override void Set(T newValue)
  {
    if ((object) newValue == (object) this.Value)
      return;
    this.deltaType = NetRefBase<T, TSelf>.RefDeltaType.Reassigned;
    if (this.Root != null)
      this.reassigned.Set(this.Root.Clock.netVersion);
    this.cleanSet(newValue);
    this.MarkDirty();
  }

  private T createType(Type type)
  {
    if ((object) type == null)
      return default (T);
    return typeof (T).IsAssignableFrom(type) ? (T) Activator.CreateInstance(type) : throw new InvalidCastException($"Net ref field '{this.Name}' received invalid type '{type.FullName}', which can't be converted to expected type '{typeof (T).FullName}'.");
  }

  protected T ReadType(BinaryReader reader) => this.createType(reader.ReadType());

  protected void WriteType(BinaryWriter writer) => writer.WriteTypeOf<T>(this.targetValue);

  private void serialize(BinaryWriter writer, XmlSerializer serializer = null)
  {
    using (MemoryStream memoryStream = new MemoryStream())
    {
      (serializer ?? this.Serializer).SerializeFast((Stream) memoryStream, (object) this.targetValue);
      memoryStream.Seek(0L, SeekOrigin.Begin);
      writer.Write((int) memoryStream.Length);
      writer.Write(memoryStream.ToArray());
    }
  }

  private T deserialize(BinaryReader reader, XmlSerializer serializer = null)
  {
    int count = reader.ReadInt32();
    using (MemoryStream memoryStream = new MemoryStream(reader.ReadBytes(count)))
      return (T) (serializer ?? this.Serializer).DeserializeFast((Stream) memoryStream);
  }

  protected abstract void ReadValueFull(T value, BinaryReader reader, NetVersion version);

  protected abstract void ReadValueDelta(BinaryReader reader, NetVersion version);

  protected abstract void WriteValueFull(BinaryWriter writer);

  protected abstract void WriteValueDelta(BinaryWriter writer);

  private void writeBaseValue(BinaryWriter writer)
  {
    if (this.Serializer != null)
      this.serialize(writer);
    else
      this.WriteType(writer);
  }

  private T readBaseValue(BinaryReader reader, NetVersion version)
  {
    return this.Serializer != null ? this.deserialize(reader) : this.ReadType(reader);
  }

  protected override void ReadDelta(BinaryReader reader, NetVersion version)
  {
    if (reader.ReadByte() == (byte) 1)
      reader.ReadSkippable((Action) (() =>
      {
        NetVersion other = new NetVersion();
        other.Read(reader);
        T obj = this.readBaseValue(reader, version);
        if ((object) obj != null)
          this.ReadValueFull(obj, reader, version);
        if (other.IsIndependent(this.reassigned))
        {
          if (other.IsPriorityOver(this.reassigned))
          {
            if (this.OnConflictResolve != null)
              this.OnConflictResolve(this.targetValue, obj);
          }
          else
          {
            if (this.OnConflictResolve == null)
              return;
            this.OnConflictResolve(obj, this.targetValue);
            return;
          }
        }
        else if (!other.IsPriorityOver(this.reassigned))
          return;
        this.reassigned.Set(other);
        this.setInterpolationTarget(obj);
      }));
    else
      reader.ReadSkippable((Action) (() =>
      {
        if (!version.IsPrecededBy(this.reassigned) || (object) this.targetValue == null)
          return;
        this.ReadValueDelta(reader, version);
      }));
  }

  protected override void WriteDelta(BinaryWriter writer)
  {
    writer.Push((object) this.targetValue != null ? this.targetValue.GetType().Name : "null");
    writer.Write((byte) this.deltaType);
    if (this.deltaType == NetRefBase<T, TSelf>.RefDeltaType.Reassigned)
      writer.WriteSkippable((Action) (() =>
      {
        this.reassigned.Write(writer);
        this.writeBaseValue(writer);
        if ((object) this.targetValue == null)
          return;
        this.WriteValueFull(writer);
      }));
    else
      writer.WriteSkippable((Action) (() =>
      {
        if ((object) this.targetValue == null)
          return;
        this.WriteValueDelta(writer);
      }));
    this.deltaType = NetRefBase<T, TSelf>.RefDeltaType.ChildDelta;
    writer.Pop();
  }

  public override void ReadFull(BinaryReader reader, NetVersion version)
  {
    this.reassigned.Read(reader);
    T newValue = this.readBaseValue(reader, version);
    if ((object) newValue != null)
      this.ReadValueFull(newValue, reader, version);
    this.cleanSet(newValue);
    this.ChangeVersion.Merge(version);
  }

  public override void WriteFull(BinaryWriter writer)
  {
    writer.Push((object) this.targetValue != null ? this.targetValue.GetType().Name : "null");
    this.reassigned.Write(writer);
    this.writeBaseValue(writer);
    if ((object) this.targetValue != null)
      this.WriteValueFull(writer);
    writer.Pop();
  }

  private enum RefDeltaType : byte
  {
    ChildDelta,
    Reassigned,
  }

  public delegate void ConflictResolveEvent(T rejected, T accepted)
    where T : class
    where TSelf : NetRefBase<T, TSelf>;
}
