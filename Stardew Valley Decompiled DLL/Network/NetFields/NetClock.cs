// Decompiled with JetBrains decompiler
// Type: Netcode.NetClock
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using System.Collections.Generic;

#nullable disable
namespace Netcode;

public class NetClock
{
  public NetVersion netVersion;
  public int LocalId;
  public int InterpolationTicks;
  public List<bool> blanks = new List<bool>();

  public NetClock()
  {
    this.netVersion = new NetVersion();
    this.LocalId = this.AddNewPeer();
  }

  public int AddNewPeer()
  {
    int num = this.blanks.IndexOf(true);
    if (num != -1)
    {
      this.blanks[num] = false;
    }
    else
    {
      num = this.netVersion.Size();
      while (this.blanks.Count < this.netVersion.Size())
        this.blanks.Add(false);
      this.netVersion[num] = 0U;
    }
    return num;
  }

  public void RemovePeer(int id)
  {
    while (this.blanks.Count <= id)
      this.blanks.Add(false);
    this.blanks[id] = true;
  }

  public uint GetLocalTick() => this.netVersion[this.LocalId];

  public void Tick() => ++this.netVersion[this.LocalId];

  public void Clear()
  {
    this.netVersion.Clear();
    this.LocalId = 0;
  }

  public override string ToString() => $"{base.ToString()};LocalId={this.LocalId.ToString()}";
}
