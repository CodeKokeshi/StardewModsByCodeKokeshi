// Decompiled with JetBrains decompiler
// Type: StardewValley.Network.NetDirection
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Netcode;
using System;
using System.IO;

#nullable disable
namespace StardewValley.Network;

public sealed class NetDirection : NetField<int, NetDirection>
{
  public NetPosition Position;

  public NetDirection()
  {
    this.InterpolationEnabled = true;
    this.InterpolationWait = true;
  }

  public NetDirection(int value)
    : base(value)
  {
    this.InterpolationEnabled = true;
    this.InterpolationWait = true;
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

  protected override bool setUpInterpolation(int oldValue, int newValue) => true;

  public int getInterpolatedDirection()
  {
    if (this.Position != null && this.Position.IsInterpolating() && !this.Position.IsPausePending())
    {
      Vector2 vector2 = this.Position.CurrentInterpolationDirection();
      if ((double) Math.Abs(vector2.X) > (double) Math.Abs(vector2.Y))
        return (double) vector2.X < 0.0 ? 3 : 1;
      if ((double) Math.Abs(vector2.Y) > (double) Math.Abs(vector2.X))
        return (double) vector2.Y < 0.0 ? 0 : 2;
    }
    return this.value;
  }

  protected override int interpolate(int startValue, int endValue, float factor)
  {
    if (this.Position != null && this.Position.IsInterpolating() && !this.Position.IsPausePending())
    {
      Vector2 vector2 = this.Position.CurrentInterpolationDirection();
      if ((double) Math.Abs(vector2.X) > (double) Math.Abs(vector2.Y))
        return (double) vector2.X < 0.0 ? 3 : 1;
      if ((double) Math.Abs(vector2.Y) > (double) Math.Abs(vector2.X))
        return (double) vector2.Y < 0.0 ? 0 : 2;
    }
    return startValue;
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
