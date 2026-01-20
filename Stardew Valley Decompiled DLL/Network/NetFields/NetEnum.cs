// Decompiled with JetBrains decompiler
// Type: Netcode.NetEnum`1
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

public class NetEnum<T> : NetFieldBase<T, NetEnum<T>>, IEnumerable<string>, IEnumerable where T : struct, IConvertible
{
  private bool xmlInitialized;

  public NetEnum()
  {
  }

  public NetEnum(T value)
    : base(value)
  {
  }

  public override void Set(T newValue)
  {
    if (EqualityComparer<T>.Default.Equals(newValue, this.value))
      return;
    this.cleanSet(newValue);
    this.MarkDirty();
  }

  protected override void ReadDelta(BinaryReader reader, NetVersion version)
  {
    T newValue = (T) Enum.ToObject(typeof (T), reader.ReadInt16());
    if (!version.IsPriorityOver(this.ChangeVersion))
      return;
    this.setInterpolationTarget(newValue);
  }

  protected override void WriteDelta(BinaryWriter writer)
  {
    writer.Write(Convert.ToInt16((object) this.value));
  }

  public IEnumerator<string> GetEnumerator()
  {
    return Enumerable.Repeat<string>(Convert.ToString((object) this.Get()), 1).GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

  public void Add(string value)
  {
    if (this.xmlInitialized || this.Parent != null)
      throw new InvalidOperationException($"{this.GetType().Name} already has value {this.ToString()}");
    this.cleanSet((T) Enum.Parse(typeof (T), value));
    this.xmlInitialized = true;
  }
}
