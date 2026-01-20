// Decompiled with JetBrains decompiler
// Type: Netcode.NetDouble
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using System.IO;

#nullable disable
namespace Netcode;

public sealed class NetDouble : NetField<double, NetDouble>
{
  public NetDouble()
  {
  }

  public NetDouble(double value)
    : base(value)
  {
  }

  public override void Set(double newValue)
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

  protected override double interpolate(double startValue, double endValue, float factor)
  {
    return startValue + (endValue - startValue) * (double) factor;
  }

  protected override void ReadDelta(BinaryReader reader, NetVersion version)
  {
    double newValue = reader.ReadDouble();
    if (!version.IsPriorityOver(this.ChangeVersion))
      return;
    this.setInterpolationTarget(newValue);
  }

  protected override void WriteDelta(BinaryWriter writer) => writer.Write(this.value);
}
