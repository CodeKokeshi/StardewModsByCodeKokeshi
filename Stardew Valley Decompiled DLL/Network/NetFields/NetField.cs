// Decompiled with JetBrains decompiler
// Type: Netcode.NetFieldBase`2
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using System;
using System.IO;

#nullable disable
namespace Netcode;

public abstract class NetFieldBase<T, TSelf> : 
  AbstractNetSerializable,
  IEquatable<TSelf>,
  InterpolationCancellable
  where TSelf : NetFieldBase<T, TSelf>
{
  protected NetFieldBase<T, TSelf>.NetFieldBaseBool _bools;
  protected uint interpolationStartTick;
  protected T value;
  protected T previousValue;
  protected T targetValue;

  /// <summary>An event raised when this field's value is set (either locally or remotely). Not triggered by changes due to interpolation. May be triggered before the change is visible on the field, if InterpolationTicks &gt; 0.</summary>
  public event FieldChange<TSelf, T> fieldChangeEvent;

  /// <summary>An event raised after this field's value is set and interpolated.</summary>
  public event FieldChange<TSelf, T> fieldChangeVisibleEvent;

  public bool InterpolationEnabled
  {
    get => (this._bools & NetFieldBase<,>.NetFieldBaseBool.InterpolationEnabled) != 0;
    set
    {
      if (value)
        this._bools |= NetFieldBase<,>.NetFieldBaseBool.InterpolationEnabled;
      else
        this._bools &= ~NetFieldBase<,>.NetFieldBaseBool.InterpolationEnabled;
    }
  }

  public bool ExtrapolationEnabled
  {
    get => (this._bools & NetFieldBase<,>.NetFieldBaseBool.ExtrapolationEnabled) != 0;
    set
    {
      if (value)
        this._bools |= NetFieldBase<,>.NetFieldBaseBool.ExtrapolationEnabled;
      else
        this._bools &= ~NetFieldBase<,>.NetFieldBaseBool.ExtrapolationEnabled;
    }
  }

  public bool InterpolationWait
  {
    get => (this._bools & NetFieldBase<,>.NetFieldBaseBool.InterpolationWait) != 0;
    set
    {
      if (value)
        this._bools |= NetFieldBase<,>.NetFieldBaseBool.InterpolationWait;
      else
        this._bools &= ~NetFieldBase<,>.NetFieldBaseBool.InterpolationWait;
    }
  }

  protected bool notifyOnTargetValueChange
  {
    get => (this._bools & NetFieldBase<,>.NetFieldBaseBool.notifyOnTargetValueChange) != 0;
    set
    {
      if (value)
        this._bools |= NetFieldBase<,>.NetFieldBaseBool.notifyOnTargetValueChange;
      else
        this._bools &= ~NetFieldBase<,>.NetFieldBaseBool.notifyOnTargetValueChange;
    }
  }

  public T TargetValue => this.targetValue;

  public T Value
  {
    get => this.value;
    set => this.Set(value);
  }

  public NetFieldBase()
  {
    this.InterpolationWait = true;
    this.value = default (T);
    this.previousValue = default (T);
    this.targetValue = default (T);
  }

  public NetFieldBase(T value)
    : this()
  {
    this.cleanSet(value);
  }

  public TSelf Interpolated(bool interpolate, bool wait)
  {
    this.InterpolationEnabled = interpolate;
    this.InterpolationWait = wait;
    return (TSelf) this;
  }

  protected virtual int InterpolationTicks()
  {
    return this.Root == null ? 0 : this.Root.Clock.InterpolationTicks;
  }

  protected float InterpolationFactor()
  {
    return (float) (this.Root.Clock.GetLocalTick() - this.interpolationStartTick) / (float) this.InterpolationTicks();
  }

  public bool IsInterpolating() => this.InterpolationEnabled && this.NeedsTick;

  public bool IsChanging() => this.NeedsTick;

  protected override bool tickImpl()
  {
    if (this.Root != null && this.InterpolationTicks() > 0)
    {
      float factor = this.InterpolationFactor();
      bool flag = this.ExtrapolationEnabled && (int) this.ChangeVersion[0] == (int) this.Root.Clock.netVersion[0];
      if ((double) factor < 1.0 && this.InterpolationEnabled || flag && (double) factor < 3.0)
      {
        this.value = this.interpolate(this.previousValue, this.targetValue, factor);
        return true;
      }
      if ((double) factor < 1.0 && this.InterpolationWait)
      {
        this.value = this.previousValue;
        return true;
      }
    }
    T previousValue = this.previousValue;
    this.CancelInterpolation();
    if (this.fieldChangeVisibleEvent != null)
      this.fieldChangeVisibleEvent((TSelf) this, previousValue, this.value);
    return false;
  }

  public void CancelInterpolation()
  {
    if (!this.NeedsTick)
      return;
    this.value = this.targetValue;
    this.previousValue = default (T);
    this.NeedsTick = false;
  }

  public T Get() => this.value;

  protected virtual T interpolate(T startValue, T endValue, float factor) => startValue;

  public abstract void Set(T newValue);

  protected bool canShortcutSet()
  {
    return this.Dirty && this.fieldChangeEvent == null && this.fieldChangeVisibleEvent == null;
  }

  protected virtual void targetValueChanged(T oldValue, T newValue)
  {
  }

  protected void cleanSet(T newValue)
  {
    T oldValue = this.value;
    T targetValue = this.targetValue;
    this.targetValue = newValue;
    this.value = newValue;
    this.previousValue = default (T);
    this.NeedsTick = false;
    if (this.notifyOnTargetValueChange)
      this.targetValueChanged(targetValue, newValue);
    if (this.fieldChangeEvent != null)
      this.fieldChangeEvent((TSelf) this, oldValue, newValue);
    if (this.fieldChangeVisibleEvent == null)
      return;
    this.fieldChangeVisibleEvent((TSelf) this, oldValue, newValue);
  }

  protected virtual bool setUpInterpolation(T oldValue, T newValue) => true;

  protected void setInterpolationTarget(T newValue)
  {
    T oldValue = this.value;
    if (!this.InterpolationWait || this.Root == null || !this.setUpInterpolation(oldValue, newValue))
    {
      this.cleanSet(newValue);
    }
    else
    {
      T targetValue = this.targetValue;
      this.previousValue = oldValue;
      this.NeedsTick = true;
      this.targetValue = newValue;
      this.interpolationStartTick = this.Root.Clock.GetLocalTick();
      if (this.notifyOnTargetValueChange)
        this.targetValueChanged(targetValue, newValue);
      if (this.fieldChangeEvent == null)
        return;
      this.fieldChangeEvent((TSelf) this, oldValue, newValue);
    }
  }

  protected abstract void ReadDelta(BinaryReader reader, NetVersion version);

  protected abstract void WriteDelta(BinaryWriter writer);

  public override void ReadFull(BinaryReader reader, NetVersion version)
  {
    this.ReadDelta(reader, version);
    this.CancelInterpolation();
    this.ChangeVersion.Merge(version);
  }

  public override void WriteFull(BinaryWriter writer) => this.WriteDelta(writer);

  public override void Read(BinaryReader reader, NetVersion version)
  {
    this.ReadDelta(reader, version);
    this.ChangeVersion.Merge(version);
  }

  public override void Write(BinaryWriter writer) => this.WriteDelta(writer);

  public override string ToString() => (object) this.value != null ? this.value.ToString() : "null";

  public override bool Equals(object obj)
  {
    return obj is TSelf other && this.Equals(other) || object.Equals((object) this.Value, obj);
  }

  public bool Equals(TSelf other) => object.Equals((object) this.Value, (object) other.Value);

  public static bool operator ==(NetFieldBase<T, TSelf> self, TSelf other)
  {
    return self == (object) other || object.Equals((object) self, (object) other);
  }

  public static bool operator !=(NetFieldBase<T, TSelf> self, TSelf other)
  {
    return self != (object) other && !object.Equals((object) self, (object) other);
  }

  public override int GetHashCode()
  {
    return ((object) this.value != null ? this.value.GetHashCode() : 0) ^ -858436897;
  }

  [Flags]
  protected enum NetFieldBaseBool : byte
  {
    None = 0,
    InterpolationEnabled = 1,
    ExtrapolationEnabled = 2,
    InterpolationWait = 4,
    notifyOnTargetValueChange = 8,
  }
}
