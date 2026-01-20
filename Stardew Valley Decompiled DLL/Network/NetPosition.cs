// Decompiled with JetBrains decompiler
// Type: StardewValley.Network.NetPosition
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Netcode;

#nullable disable
namespace StardewValley.Network;

public sealed class NetPosition : NetPausableField<Vector2, NetVector2, NetVector2>
{
  private const float SmoothingFudge = 0.8f;
  private const ushort DefaultDeltaAggregateTicks = 0;
  public bool ExtrapolationEnabled;
  public readonly NetBool moving = new NetBool().Interpolated(false, false);

  public override NetFields NetFields { get; } = new NetFields(nameof (NetPosition));

  /// <summary>An event raised when this field's value is set (either locally or remotely). Not triggered by changes due to interpolation. May be triggered before the change is visible on the field, if InterpolationTicks &gt; 0.</summary>
  public event FieldChange<NetPosition, Vector2> fieldChangeEvent;

  /// <summary>An event raised after this field's value is set and interpolated.</summary>
  public event FieldChange<NetPosition, Vector2> fieldChangeVisibleEvent;

  public float X
  {
    get => this.Get().X;
    set => this.Set(new Vector2(value, this.Y));
  }

  public float Y
  {
    get => this.Get().Y;
    set => this.Set(new Vector2(this.X, value));
  }

  public NetPosition()
    : base(new NetVector2().Interpolated(true, true))
  {
  }

  public NetPosition(NetVector2 field)
    : base(field)
  {
  }

  protected override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.moving, "moving");
    this.NetFields.DeltaAggregateTicks = (ushort) 0;
    this.Field.fieldChangeEvent += (FieldChange<NetVector2, Vector2>) ((f, oldValue, newValue) =>
    {
      if (this.IsMaster())
        this.moving.Value = true;
      FieldChange<NetPosition, Vector2> fieldChangeEvent = this.fieldChangeEvent;
      if (fieldChangeEvent == null)
        return;
      fieldChangeEvent(this, oldValue, newValue);
    });
    this.Field.fieldChangeVisibleEvent += (FieldChange<NetVector2, Vector2>) ((field, oldValue, newValue) =>
    {
      FieldChange<NetPosition, Vector2> changeVisibleEvent = this.fieldChangeVisibleEvent;
      if (changeVisibleEvent == null)
        return;
      changeVisibleEvent(this, oldValue, newValue);
    });
    this.moving.fieldChangeEvent += (FieldChange<NetBool, bool>) ((f, oldValue, newValue) =>
    {
      if (this.IsMaster())
        return;
      this.Field.ExtrapolationEnabled = newValue && this.ExtrapolationEnabled;
    });
  }

  protected bool IsMaster()
  {
    INetRoot root = this.NetFields.Root;
    return root != null && root.Clock.LocalId == 0;
  }

  public override Vector2 Get()
  {
    if (Game1.HostPaused)
      this.Field.CancelInterpolation();
    return base.Get();
  }

  public Vector2 CurrentInterpolationDirection()
  {
    return this.Paused ? Vector2.Zero : this.Field.CurrentInterpolationDirection();
  }

  public float CurrentInterpolationSpeed()
  {
    return this.Paused ? 0.0f : this.Field.CurrentInterpolationSpeed();
  }

  public void UpdateExtrapolation(float extrapolationSpeed)
  {
    this.NetFields.DeltaAggregateTicks = this.NetFields.Root != null ? (ushort) ((double) this.NetFields.Root.Clock.InterpolationTicks * 0.800000011920929) : (ushort) 0;
    this.ExtrapolationEnabled = true;
    this.Field.ExtrapolationSpeed = extrapolationSpeed;
    if (!this.IsMaster())
      return;
    this.moving.Value = false;
  }
}
