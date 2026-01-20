// Decompiled with JetBrains decompiler
// Type: Netcode.NetIntDelta
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using System;
using System.IO;

#nullable disable
namespace Netcode;

/// <summary>Stores an integer value.</summary>
/// <remarks><see cref="T:Netcode.NetInt" /> and <see cref="T:Netcode.NetIntDelta" /> are closely related, but resolve simultaneous changes differently. Whereas NetInt sends absolute values over the network, NetIntDelta sends the change relative to the previous value, so that a simultaneous increase of 1 on two peers results in an overall increase by 2.</remarks>
public sealed class NetIntDelta : NetField<int, NetIntDelta>
{
  private int networkValue;
  public int DirtyThreshold;
  public int? Minimum;
  public int? Maximum;

  public NetIntDelta() => this.Interpolated(false, false);

  public NetIntDelta(int value)
    : base(value)
  {
    this.Interpolated(false, false);
  }

  private int fixRange(int value)
  {
    if (this.Minimum.HasValue)
      value = Math.Max(this.Minimum.Value, value);
    if (this.Maximum.HasValue)
      value = Math.Min(this.Maximum.Value, value);
    return value;
  }

  public override void Set(int newValue)
  {
    newValue = this.fixRange(newValue);
    if (newValue == this.value)
      return;
    this.cleanSet(newValue);
    if (Math.Abs(newValue - this.networkValue) <= this.DirtyThreshold)
      return;
    this.MarkDirty();
  }

  protected override int interpolate(int startValue, int endValue, float factor)
  {
    return startValue + (int) ((double) (endValue - startValue) * (double) factor);
  }

  protected override void ReadDelta(BinaryReader reader, NetVersion version)
  {
    int num = reader.ReadInt32();
    this.networkValue = this.fixRange(this.networkValue + num);
    this.setInterpolationTarget(this.fixRange(this.targetValue + num));
  }

  protected override void WriteDelta(BinaryWriter writer)
  {
    writer.Write(this.targetValue - this.networkValue);
    this.networkValue = this.targetValue;
  }

  public override void ReadFull(BinaryReader reader, NetVersion version)
  {
    int newValue = reader.ReadInt32();
    this.cleanSet(newValue);
    this.networkValue = newValue;
    this.ChangeVersion.Merge(version);
  }

  public override void WriteFull(BinaryWriter writer)
  {
    writer.Write(this.targetValue);
    this.networkValue = this.targetValue;
  }
}
