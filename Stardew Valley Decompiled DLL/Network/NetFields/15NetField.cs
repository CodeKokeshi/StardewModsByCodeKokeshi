// Decompiled with JetBrains decompiler
// Type: Netcode.NetColor
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using System.IO;

#nullable disable
namespace Netcode;

public sealed class NetColor : NetField<Color, NetColor>
{
  public byte R
  {
    get => this.Value.R;
    set => this.Value = new Color(value, this.G, this.B, this.A);
  }

  public byte G
  {
    get => this.Value.G;
    set => this.Value = new Color(this.R, value, this.B, this.A);
  }

  public byte B
  {
    get => this.Value.B;
    set => this.Value = new Color(this.R, this.G, value, this.A);
  }

  public byte A
  {
    get => this.Value.A;
    set => this.Value = new Color(this.R, this.G, this.B, value);
  }

  public NetColor()
  {
  }

  public NetColor(Color value)
    : base(value)
  {
  }

  public override void Set(Color newValue)
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

  public new bool Equals(NetColor other) => this.value == other.value;

  public bool Equals(Color other) => this.value == other;

  protected override void ReadDelta(BinaryReader reader, NetVersion version)
  {
    Color newValue = new Color();
    newValue.PackedValue = reader.ReadUInt32();
    if (!version.IsPriorityOver(this.ChangeVersion))
      return;
    this.setInterpolationTarget(newValue);
  }

  protected override void WriteDelta(BinaryWriter writer) => writer.Write(this.value.PackedValue);
}
