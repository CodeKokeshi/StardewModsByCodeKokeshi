// Decompiled with JetBrains decompiler
// Type: Netcode.NetInt
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using System.IO;

#nullable disable
namespace Netcode;

/// <summary>Stores an integer value.</summary>
/// <inheritdoc cref="T:Netcode.NetIntDelta" path="/remarks" />
public sealed class NetInt : NetField<int, NetInt>
{
  public NetInt()
  {
  }

  public NetInt(int value)
    : base(value)
  {
  }

  public override void Set(int newValue)
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

  public new bool Equals(NetInt other) => this.value == other.value;

  public bool Equals(int other) => this.value == other;

  protected override int interpolate(int startValue, int endValue, float factor)
  {
    return startValue + (int) ((double) (endValue - startValue) * (double) factor);
  }

  protected override void ReadDelta(BinaryReader reader, NetVersion version)
  {
    int newValue = reader.ReadInt32();
    if (!version.IsPriorityOver(this.ChangeVersion))
      return;
    this.setInterpolationTarget(newValue);
  }

  protected override void WriteDelta(BinaryWriter writer) => writer.Write(this.value);
}
