// Decompiled with JetBrains decompiler
// Type: Netcode.NetGuid
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using System;
using System.IO;

#nullable disable
namespace Netcode;

public sealed class NetGuid : NetField<Guid, NetGuid>
{
  public NetGuid()
  {
  }

  public NetGuid(Guid value)
    : base(value)
  {
  }

  public override void Set(Guid newValue)
  {
    if (this.canShortcutSet())
    {
      this.value = newValue;
    }
    else
    {
      if (!(newValue != this.value))
        return;
      this.cleanSet(newValue);
      this.MarkDirty();
    }
  }

  protected override void ReadDelta(BinaryReader reader, NetVersion version)
  {
    Guid newValue = reader.ReadGuid();
    if (!version.IsPriorityOver(this.ChangeVersion))
      return;
    this.setInterpolationTarget(newValue);
  }

  protected override void WriteDelta(BinaryWriter writer) => writer.WriteGuid(this.value);
}
