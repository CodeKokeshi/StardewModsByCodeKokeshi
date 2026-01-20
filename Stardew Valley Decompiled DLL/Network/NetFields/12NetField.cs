// Decompiled with JetBrains decompiler
// Type: Netcode.NetVector2
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using System;
using System.IO;

#nullable disable
namespace Netcode;

public sealed class NetVector2 : NetField<Vector2, NetVector2>
{
  public bool AxisAlignedMovement;
  public float ExtrapolationSpeed;
  public float MinDeltaForDirectionChange = 8f;
  public float MaxInterpolationDistance = 320f;
  private bool interpolateXFirst;
  private bool isExtrapolating;
  private bool isFixingExtrapolation;

  public NetVector2()
  {
  }

  public NetVector2(Vector2 value)
    : base(value)
  {
  }

  public float X
  {
    get => this.Value.X;
    set
    {
      Vector2 vector2 = this.value;
      if ((double) vector2.X == (double) value)
        return;
      Vector2 newValue = new Vector2(value, vector2.Y);
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

  public float Y
  {
    get => this.Value.Y;
    set
    {
      Vector2 vector2 = this.value;
      if ((double) vector2.Y == (double) value)
        return;
      Vector2 newValue = new Vector2(vector2.X, value);
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

  public void Set(float x, float y) => this.Set(new Vector2(x, y));

  public override void Set(Vector2 newValue)
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

  public Vector2 InterpolationDelta()
  {
    return this.NeedsTick ? this.targetValue - this.previousValue : Vector2.Zero;
  }

  protected override bool setUpInterpolation(Vector2 oldValue, Vector2 newValue)
  {
    if ((double) (newValue - oldValue).LengthSquared() >= (double) this.MaxInterpolationDistance * (double) this.MaxInterpolationDistance)
      return false;
    if (this.AxisAlignedMovement)
    {
      if (this.NeedsTick)
      {
        Vector2 vector2_1 = this.targetValue - this.previousValue;
        Vector2 vector2_2 = new Vector2(Math.Abs(vector2_1.X), Math.Abs(vector2_1.Y));
        this.interpolateXFirst = !this.interpolateXFirst ? (double) this.InterpolationFactor() * ((double) vector2_2.X + (double) vector2_2.Y) > (double) vector2_2.Y : (double) this.InterpolationFactor() * ((double) vector2_2.X + (double) vector2_2.Y) < (double) vector2_2.X;
      }
      else
      {
        Vector2 vector2_3 = newValue - oldValue;
        Vector2 vector2_4 = new Vector2(Math.Abs(vector2_3.X), Math.Abs(vector2_3.Y));
        this.interpolateXFirst = (double) vector2_4.X < (double) vector2_4.Y;
      }
    }
    return true;
  }

  public Vector2 CurrentInterpolationDirection()
  {
    if (this.AxisAlignedMovement)
    {
      float num1 = this.InterpolationFactor();
      Vector2 vector2 = this.InterpolationDelta();
      float num2 = (Math.Abs(vector2.X) + Math.Abs(vector2.Y)) * num1;
      if ((double) Math.Abs(vector2.X) < (double) this.MinDeltaForDirectionChange && (double) Math.Abs(vector2.Y) < (double) this.MinDeltaForDirectionChange)
        return Vector2.Zero;
      if ((double) Math.Abs(vector2.X) < (double) this.MinDeltaForDirectionChange)
        return new Vector2(0.0f, (float) Math.Sign(vector2.Y));
      if ((double) Math.Abs(vector2.Y) < (double) this.MinDeltaForDirectionChange)
        return new Vector2((float) Math.Sign(vector2.X), 0.0f);
      return this.interpolateXFirst ? ((double) num2 > (double) Math.Abs(vector2.X) ? new Vector2(0.0f, (float) Math.Sign(vector2.Y)) : new Vector2((float) Math.Sign(vector2.X), 0.0f)) : ((double) num2 > (double) Math.Abs(vector2.Y) ? new Vector2((float) Math.Sign(vector2.X), 0.0f) : new Vector2(0.0f, (float) Math.Sign(vector2.Y)));
    }
    Vector2 vector2_1 = this.InterpolationDelta();
    vector2_1.Normalize();
    return vector2_1;
  }

  public float CurrentInterpolationSpeed()
  {
    float num = this.InterpolationDelta().Length();
    if (this.InterpolationTicks() == 0)
      return num;
    return (double) this.InterpolationFactor() > 1.0 ? this.ExtrapolationSpeed : num / (float) this.InterpolationTicks();
  }

  protected override Vector2 interpolate(Vector2 startValue, Vector2 endValue, float factor)
  {
    if (this.AxisAlignedMovement && (double) factor <= 1.0 && !this.isFixingExtrapolation)
    {
      this.isExtrapolating = false;
      Vector2 vector2_1 = this.InterpolationDelta();
      Vector2 vector2_2 = new Vector2(Math.Abs(vector2_1.X), Math.Abs(vector2_1.Y));
      float num = (vector2_2.X + vector2_2.Y) * factor;
      float x;
      float y;
      if (this.interpolateXFirst)
      {
        if ((double) num > (double) vector2_2.X)
        {
          x = endValue.X;
          y = startValue.Y + (num - vector2_2.X) * (float) Math.Sign(vector2_1.Y);
        }
        else
        {
          x = startValue.X + num * (float) Math.Sign(vector2_1.X);
          y = startValue.Y;
        }
      }
      else if ((double) num > (double) vector2_2.Y)
      {
        y = endValue.Y;
        x = startValue.X + (num - vector2_2.Y) * (float) Math.Sign(vector2_1.X);
      }
      else
      {
        y = startValue.Y + num * (float) Math.Sign(vector2_1.Y);
        x = startValue.X;
      }
      return new Vector2(x, y);
    }
    if ((double) factor > 1.0)
    {
      this.isExtrapolating = true;
      uint num = (uint) ((int) this.Root.Clock.GetLocalTick() - (int) this.interpolationStartTick - this.InterpolationTicks());
      Vector2 vector2 = endValue - startValue;
      if ((double) vector2.LengthSquared() > (double) this.ExtrapolationSpeed * (double) this.ExtrapolationSpeed)
      {
        vector2.Normalize();
        return endValue + vector2 * (float) num * this.ExtrapolationSpeed;
      }
    }
    this.isExtrapolating = false;
    return startValue + (endValue - startValue) * factor;
  }

  protected override void ReadDelta(BinaryReader reader, NetVersion version)
  {
    float x = reader.ReadSingle();
    float y = reader.ReadSingle();
    if (!version.IsPriorityOver(this.ChangeVersion))
      return;
    this.isFixingExtrapolation = this.isExtrapolating;
    this.setInterpolationTarget(new Vector2(x, y));
    this.isExtrapolating = false;
  }

  protected override void WriteDelta(BinaryWriter writer)
  {
    writer.Write(this.Value.X);
    writer.Write(this.Value.Y);
  }
}
