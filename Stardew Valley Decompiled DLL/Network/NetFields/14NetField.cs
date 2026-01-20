// Decompiled with JetBrains decompiler
// Type: Netcode.NetRectangle
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using System.IO;

#nullable disable
namespace Netcode;

public sealed class NetRectangle : NetField<Rectangle, NetRectangle>
{
  public NetRectangle()
  {
  }

  public NetRectangle(Rectangle value)
    : base(value)
  {
  }

  public int X
  {
    get => this.Value.X;
    set
    {
      Rectangle rectangle = this.value;
      if (rectangle.X == value)
        return;
      Rectangle newValue = new Rectangle(value, rectangle.Y, rectangle.Width, rectangle.Height);
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
      Rectangle rectangle = this.value;
      if (rectangle.Y == value)
        return;
      Rectangle newValue = new Rectangle(rectangle.X, value, rectangle.Width, rectangle.Height);
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

  public int Width
  {
    get => this.Value.Width;
    set
    {
      Rectangle rectangle = this.value;
      if (rectangle.Width == value)
        return;
      Rectangle newValue = new Rectangle(rectangle.X, rectangle.Y, value, rectangle.Height);
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

  public int Height
  {
    get => this.Value.Height;
    set
    {
      Rectangle rectangle = this.value;
      if (rectangle.Height == value)
        return;
      Rectangle newValue = new Rectangle(rectangle.X, rectangle.Y, rectangle.Width, value);
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

  public Point Center => this.value.Center;

  public int Top => this.value.Top;

  public int Bottom => this.value.Bottom;

  public int Left => this.value.Left;

  public int Right => this.value.Right;

  public void Set(int x, int y, int width, int height)
  {
    this.Set(new Rectangle(x, y, width, height));
  }

  public override void Set(Rectangle newValue)
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
    int x = reader.ReadInt32();
    int y = reader.ReadInt32();
    int width = reader.ReadInt32();
    int height = reader.ReadInt32();
    if (!version.IsPriorityOver(this.ChangeVersion))
      return;
    this.setInterpolationTarget(new Rectangle(x, y, width, height));
  }

  protected override void WriteDelta(BinaryWriter writer)
  {
    writer.Write(this.value.X);
    writer.Write(this.value.Y);
    writer.Write(this.value.Width);
    writer.Write(this.value.Height);
  }
}
