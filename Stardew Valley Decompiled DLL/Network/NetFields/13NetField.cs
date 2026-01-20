// Decompiled with JetBrains decompiler
// Type: Netcode.NetPoint
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using System.IO;

#nullable disable
namespace Netcode;

public sealed class NetPoint : NetField<Point, NetPoint>
{
  public NetPoint()
  {
  }

  public NetPoint(Point value)
    : base(value)
  {
  }

  public int X
  {
    get => this.Value.X;
    set
    {
      Point point = this.value;
      if (point.X == value)
        return;
      Point newValue = new Point(value, point.Y);
      if (this.canShortcutSet())
      {
        this.value = newValue;
      }
      else
      {
        this.cleanSet(newValue);
        this.MarkDirty();
      }
    }
  }

  public int Y
  {
    get => this.Value.Y;
    set
    {
      Point point = this.value;
      if (point.Y == value)
        return;
      Point newValue = new Point(point.X, value);
      if (this.canShortcutSet())
      {
        this.value = newValue;
      }
      else
      {
        this.cleanSet(newValue);
        this.MarkDirty();
      }
    }
  }

  public void Set(int x, int y) => this.Set(new Point(x, y));

  public override void Set(Point newValue)
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

  protected override Point interpolate(Point startValue, Point endValue, float factor)
  {
    Point point = new Point(endValue.X - startValue.X, endValue.Y - startValue.Y);
    point.X = (int) ((double) point.X * (double) factor);
    point.Y = (int) ((double) point.Y * (double) factor);
    return new Point(startValue.X + point.X, startValue.Y + point.Y);
  }

  protected override void ReadDelta(BinaryReader reader, NetVersion version)
  {
    int x = reader.ReadInt32();
    int y = reader.ReadInt32();
    if (!version.IsPriorityOver(this.ChangeVersion))
      return;
    this.setInterpolationTarget(new Point(x, y));
  }

  protected override void WriteDelta(BinaryWriter writer)
  {
    writer.Write(this.Value.X);
    writer.Write(this.Value.Y);
  }
}
