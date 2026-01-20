// Decompiled with JetBrains decompiler
// Type: Netcode.NetBool
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using System.IO;

#nullable disable
namespace Netcode;

public sealed class NetBool : NetField<bool, NetBool>
{
  public NetBool()
  {
  }

  public NetBool(bool value)
    : base(value)
  {
  }

  public override void Set(bool newValue)
  {
    if (this.canShortcutSet())
    {
      this.value = newValue;
    }
    else
    {
      if (newValue == this.value)
        return;
      this.cleanSet(newValue);
      this.MarkDirty();
    }
  }

  protected override void ReadDelta(BinaryReader reader, NetVersion version)
  {
    bool newValue = reader.ReadBoolean();
    if (!version.IsPriorityOver(this.ChangeVersion))
      return;
    this.setInterpolationTarget(newValue);
  }

  protected override void WriteDelta(BinaryWriter writer) => writer.Write(this.value);
}
