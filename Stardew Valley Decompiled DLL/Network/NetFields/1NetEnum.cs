// Decompiled with JetBrains decompiler
// Type: Netcode.NetNullableEnum`1
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

public class NetNullableEnum<T> : NetField<T?, NetNullableEnum<T>>, IEnumerable<string>, IEnumerable where T : struct, IConvertible
{
  private bool xmlInitialized;

  public NetNullableEnum()
    : base(new T?())
  {
  }

  public NetNullableEnum(T value)
    : base(new T?(value))
  {
  }

  public override void Set(T? newValue)
  {
    if (EqualityComparer<T?>.Default.Equals(newValue, this.value))
      return;
    this.cleanSet(newValue);
    this.MarkDirty();
  }

  protected override void ReadDelta(BinaryReader reader, NetVersion version)
  {
    T? newValue = new T?();
    if (reader.ReadBoolean())
      newValue = new T?((T) Enum.ToObject(typeof (T), reader.ReadInt16()));
    if (!version.IsPriorityOver(this.ChangeVersion))
      return;
    this.setInterpolationTarget(newValue);
  }

  protected override void WriteDelta(BinaryWriter writer)
  {
    if (!this.value.HasValue)
    {
      writer.Write(false);
    }
    else
    {
      writer.Write(true);
      writer.Write(Convert.ToInt16((object) this.value));
    }
  }

  public IEnumerator<string> GetEnumerator()
  {
    T? nullable = this.Get();
    return !nullable.HasValue ? Enumerable.Repeat<string>((string) null, 1).GetEnumerator() : Enumerable.Repeat<string>(Convert.ToString((object) nullable), 1).GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

  public void Add(string value)
  {
    if (this.xmlInitialized || this.Parent != null)
      throw new InvalidOperationException($"{this.GetType().Name} already has value {this.ToString()}");
    if (!string.IsNullOrEmpty(value))
      this.cleanSet(new T?((T) Enum.Parse(typeof (T), value)));
    else
      this.cleanSet(new T?());
    this.xmlInitialized = true;
  }
}
