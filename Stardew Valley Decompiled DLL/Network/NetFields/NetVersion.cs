// Decompiled with JetBrains decompiler
// Type: Netcode.NetVersion
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using System;
using System.Collections.Generic;
using System.IO;

#nullable disable
namespace Netcode;

public struct NetVersion : IEquatable<NetVersion>
{
  private List<uint> _vector;

  private List<uint> vector
  {
    get
    {
      if (this._vector == null)
        this._vector = new List<uint>();
      return this._vector;
    }
  }

  public uint this[int peerId]
  {
    get => peerId >= this.vector.Count ? 0U : this.vector[peerId];
    set
    {
      while (this.vector.Count <= peerId)
        this.vector.Add(0U);
      this.vector[peerId] = value;
    }
  }

  public NetVersion(NetVersion other)
  {
    this._vector = new List<uint>();
    this.Set(other);
  }

  public int Size() => this.vector.Count;

  public void Set(NetVersion other)
  {
    for (int peerId = 0; peerId < Math.Max(this.Size(), other.Size()); ++peerId)
      this[peerId] = other[peerId];
  }

  public void Merge(NetVersion other)
  {
    for (int peerId = 0; peerId < Math.Max(this.Size(), other.Size()); ++peerId)
      this[peerId] = Math.Max(this[peerId], other[peerId]);
  }

  public bool IsPriorityOver(NetVersion other)
  {
    for (int peerId = 0; peerId < Math.Max(this.Size(), other.Size()) && this[peerId] <= other[peerId]; ++peerId)
    {
      if (this[peerId] < other[peerId])
        return false;
    }
    return true;
  }

  public bool IsSimultaneousWith(NetVersion other)
  {
    return this.isOrdered(other, (Func<uint, uint, bool>) ((a, b) => (int) a == (int) b));
  }

  public bool IsPrecededBy(NetVersion other)
  {
    return this.isOrdered(other, (Func<uint, uint, bool>) ((a, b) => a >= b));
  }

  public bool IsFollowedBy(NetVersion other)
  {
    return this.isOrdered(other, (Func<uint, uint, bool>) ((a, b) => a < b));
  }

  public bool IsIndependent(NetVersion other)
  {
    return !this.IsSimultaneousWith(other) && !this.IsPrecededBy(other) && !this.IsFollowedBy(other);
  }

  private bool isOrdered(NetVersion other, Func<uint, uint, bool> comparison)
  {
    for (int peerId = 0; peerId < Math.Max(this.Size(), other.Size()); ++peerId)
    {
      if (!comparison(this[peerId], other[peerId]))
        return false;
    }
    return true;
  }

  public override string ToString()
  {
    return this.Size() == 0 ? "v0" : "v" + string.Join<uint>(",", (IEnumerable<uint>) this.vector);
  }

  public bool Equals(NetVersion other)
  {
    for (int peerId = 0; peerId < Math.Max(this.Size(), other.Size()); ++peerId)
    {
      if ((int) this[peerId] != (int) other[peerId])
        return false;
    }
    return true;
  }

  public override int GetHashCode() => this.vector.GetHashCode() ^ -583558975;

  public void Write(BinaryWriter writer)
  {
    writer.Write((byte) this.Size());
    for (int peerId = 0; peerId < this.Size(); ++peerId)
      writer.Write(this[peerId]);
  }

  public void Read(BinaryReader reader)
  {
    int index = (int) reader.ReadByte();
    while (this.vector.Count > index)
      this.vector.RemoveAt(index);
    while (this.vector.Count < index)
      this.vector.Add(0U);
    for (int peerId = 0; peerId < index; ++peerId)
      this[peerId] = reader.ReadUInt32();
    for (int peerId = index; peerId < this.Size(); ++peerId)
      this[peerId] = 0U;
  }

  public void Clear()
  {
    for (int peerId = 0; peerId < this.Size(); ++peerId)
      this[peerId] = 0U;
  }
}
