// Decompiled with JetBrains decompiler
// Type: Netcode.NetFloat
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using System.IO;

#nullable disable
namespace Netcode;

public class NetFloat : NetField<float, NetFloat>
{
  public NetFloat()
  {
  }

  public NetFloat(float value)
    : base(value)
  {
  }

  public override void Set(float newValue)
  {
    if (this.canShortcutSet())
    {
      this.value = newValue;
    }
    else
    {
      if ((double) newValue == (double) this.value)
        return;
      this.cleanSet(newValue);
      this.MarkDirty();
    }
  }

  protected override float interpolate(float startValue, float endValue, float factor)
  {
    return startValue + (endValue - startValue) * factor;
  }

  protected override void ReadDelta(BinaryReader reader, NetVersion version)
  {
    float newValue = reader.ReadSingle();
    if (!version.IsPriorityOver(this.ChangeVersion))
      return;
    this.setInterpolationTarget(newValue);
  }

  protected override void WriteDelta(BinaryWriter writer) => writer.Write(this.value);
}
