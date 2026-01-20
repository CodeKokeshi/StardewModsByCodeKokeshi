// Decompiled with JetBrains decompiler
// Type: Netcode.NetEvent0
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using System;
using System.IO;

#nullable disable
namespace Netcode;

public class NetEvent0 : AbstractNetSerializable
{
  public readonly NetInt Counter = new NetInt();
  private int currentCount;

  public event NetEvent0.Event onEvent;

  public NetEvent0(bool interpolate = false) => this.Counter.InterpolationEnabled = interpolate;

  public void Fire()
  {
    ++this.Counter.Value;
    this.Poll();
  }

  public void Poll()
  {
    if (this.Counter.Value == this.currentCount)
      return;
    this.currentCount = this.Counter.Value;
    if (this.onEvent == null)
      return;
    this.onEvent();
  }

  public void Clear()
  {
    this.Counter.Set(0);
    this.currentCount = 0;
  }

  public override void Read(BinaryReader reader, NetVersion version)
  {
    this.Counter.Read(reader, version);
  }

  public override void ReadFull(BinaryReader reader, NetVersion version)
  {
    this.Counter.ReadFull(reader, version);
    this.currentCount = this.Counter.Value;
  }

  public override void Write(BinaryWriter writer) => this.Counter.Write(writer);

  public override void WriteFull(BinaryWriter writer) => this.Counter.WriteFull(writer);

  protected override void ForEachChild(Action<INetSerializable> childAction)
  {
    childAction((INetSerializable) this.Counter);
  }

  public delegate void Event();
}
