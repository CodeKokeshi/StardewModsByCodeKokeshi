// Decompiled with JetBrains decompiler
// Type: StardewValley.Network.NetPausableField`3
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Netcode;
using System;

#nullable disable
namespace StardewValley.Network;

public abstract class NetPausableField<T, TField, TBaseField> : INetObject<NetFields>
  where TField : TBaseField, new()
  where TBaseField : NetFieldBase<T, TBaseField>, new()
{
  private bool paused;
  public readonly TField Field;
  private readonly NetEvent1Field<bool, NetBool> pauseEvent = new NetEvent1Field<bool, NetBool>();

  public T Value
  {
    get => this.Get();
    set => this.Set(value);
  }

  public bool Paused
  {
    get
    {
      this.pauseEvent.Poll();
      return this.paused;
    }
    set
    {
      if (value == this.paused)
        return;
      this.pauseEvent.Fire(value);
      this.pauseEvent.Poll();
    }
  }

  public abstract NetFields NetFields { get; }

  public NetPausableField(TField field)
  {
    this.Field = field;
    this.initNetFields();
  }

  protected virtual void initNetFields()
  {
    this.NetFields.SetOwner((INetObject<NetFields>) this).AddField((INetSerializable) this.Field, "Field").AddField((INetSerializable) this.pauseEvent, "pauseEvent");
    this.pauseEvent.onEvent += (AbstractNetEvent1<bool>.Event) (newPauseValue => this.paused = newPauseValue);
  }

  public NetPausableField()
    : this(new TField())
  {
  }

  public virtual T Get()
  {
    if (this.Paused)
      this.Field.CancelInterpolation();
    return this.Field.Get();
  }

  public void Set(T value) => this.Field.Set(value);

  public bool IsPausePending() => this.pauseEvent.HasPendingEvent((Predicate<bool>) (p => p));

  public bool IsInterpolating() => this.Field.IsInterpolating() && !this.Paused;
}
