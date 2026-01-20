// Decompiled with JetBrains decompiler
// Type: Netcode.NetRoot`1
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using System.Collections.Generic;
using System.IO;

#nullable disable
namespace Netcode;

public class NetRoot<T> : NetRef<T>, INetRoot where T : class, INetObject<INetSerializable>
{
  private Dictionary<long, int> connections = new Dictionary<long, int>();

  public NetClock Clock { get; } = new NetClock();

  public override bool Dirty => this.DirtyTick <= this.Clock.GetLocalTick();

  public NetRoot() => this.Root = (INetRoot) this;

  public NetRoot(T value)
    : this()
  {
    this.cleanSet(value);
  }

  public void TickTree()
  {
    this.Clock.Tick();
    this.Tick();
  }

  public override void Read(BinaryReader reader, NetVersion _)
  {
    NetVersion netVersion = new NetVersion();
    netVersion.Read(reader);
    base.Read(reader, netVersion);
    this.Clock.netVersion.Merge(netVersion);
  }

  public void Read(BinaryReader reader)
  {
    NetVersion netVersion = new NetVersion();
    netVersion.Read(reader);
    base.Read(reader, netVersion);
    this.Clock.netVersion.Merge(netVersion);
  }

  public override void Write(BinaryWriter writer)
  {
    this.Clock.netVersion.Write(writer);
    base.Write(writer);
    this.MarkClean();
  }

  public override void ReadFull(BinaryReader reader, NetVersion _)
  {
    base.ReadFull(reader, this.Clock.netVersion);
  }

  public static NetRoot<T> Connect(BinaryReader reader)
  {
    NetRoot<T> netRoot = new NetRoot<T>();
    netRoot.ReadConnectionPacket(reader);
    return netRoot;
  }

  public void ReadConnectionPacket(BinaryReader reader)
  {
    this.Clock.LocalId = (int) reader.ReadByte();
    this.Clock.netVersion.Read(reader);
    base.ReadFull(reader, this.Clock.netVersion);
  }

  public void CreateConnectionPacket(BinaryWriter writer, long? connection)
  {
    int num;
    if (!connection.HasValue || !this.connections.TryGetValue(connection.Value, out num))
    {
      num = this.Clock.AddNewPeer();
      if (connection.HasValue)
        this.connections[connection.Value] = num;
    }
    writer.Write((byte) num);
    this.Clock.netVersion.Write(writer);
    this.WriteFull(writer);
  }

  public void Disconnect(long connection)
  {
    int id;
    if (!this.connections.TryGetValue(connection, out id))
      return;
    this.Clock.RemovePeer(id);
  }

  public virtual NetRoot<T> Clone()
  {
    using (MemoryStream memoryStream = new MemoryStream())
    {
      using (BinaryWriter writer = new BinaryWriter((Stream) memoryStream))
      {
        using (BinaryReader reader = new BinaryReader((Stream) memoryStream))
        {
          this.WriteFull(writer);
          memoryStream.Seek(0L, SeekOrigin.Begin);
          NetRoot<T> netRoot = new NetRoot<T>();
          netRoot.Serializer = this.Serializer;
          netRoot.ReadFull(reader, this.Clock.netVersion);
          netRoot.reassigned.Set(new NetVersion());
          netRoot.MarkClean();
          return netRoot;
        }
      }
    }
  }

  public void CloneInto(NetRef<T> netref)
  {
    NetRoot<T> netRoot = this.Clone();
    T obj = netRoot.Value;
    netRoot.Value = default (T);
    netref.Value = obj;
  }
}
