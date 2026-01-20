// Decompiled with JetBrains decompiler
// Type: Netcode.AbstractNetSerializable
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using System;
using System.IO;

#nullable disable
namespace Netcode;

public abstract class AbstractNetSerializable : INetSerializable, INetObject<INetSerializable>
{
  private uint dirtyTick = uint.MaxValue;
  private uint minNextDirtyTime;
  protected NetVersion ChangeVersion;
  public ushort DeltaAggregateTicks;
  private bool needsTick;
  private bool childNeedsTick;
  private INetSerializable parent;

  /// <summary>
  /// Use this when you want to always use the update from the other end, even if
  /// it is "older" (such as us updating a position every frame, but we receive
  /// a better position from the host from a couple frames ago)
  /// </summary>
  public void ResetNewestReceivedChangeVersion() => this.ChangeVersion.Clear();

  public uint DirtyTick
  {
    get => this.dirtyTick;
    set
    {
      if (value < this.dirtyTick)
      {
        this.SetDirtySooner(value);
      }
      else
      {
        if (value <= this.dirtyTick)
          return;
        this.SetDirtyLater(value);
      }
    }
  }

  public virtual bool Dirty => this.dirtyTick != uint.MaxValue;

  protected void SetDirtySooner(uint tick)
  {
    tick = Math.Max(tick, this.minNextDirtyTime);
    if (this.dirtyTick <= tick)
      return;
    this.dirtyTick = tick;
    if (this.Parent != null)
      this.Parent.DirtyTick = Math.Min(this.Parent.DirtyTick, tick);
    if (this.Root != null)
    {
      this.minNextDirtyTime = this.Root.Clock.GetLocalTick() + (uint) this.DeltaAggregateTicks;
      this.ChangeVersion.Set(this.Root.Clock.netVersion);
    }
    else
    {
      this.minNextDirtyTime = 0U;
      this.ChangeVersion.Clear();
    }
  }

  protected void SetDirtyLater(uint tick)
  {
    if (this.dirtyTick >= tick)
      return;
    this.dirtyTick = tick;
    this.ForEachChild((Action<INetSerializable>) (child => child.DirtyTick = Math.Max(child.DirtyTick, tick)));
    if (tick != uint.MaxValue)
      return;
    this.CleanImpl();
  }

  protected virtual void CleanImpl()
  {
    if (this.Root == null)
      this.minNextDirtyTime = 0U;
    else
      this.minNextDirtyTime = this.Root.Clock.GetLocalTick() + (uint) this.DeltaAggregateTicks;
  }

  public void MarkDirty()
  {
    if (this.Root == null)
      this.SetDirtySooner(0U);
    else
      this.SetDirtySooner(this.Root.Clock.GetLocalTick());
  }

  public void MarkClean() => this.SetDirtyLater(uint.MaxValue);

  public bool NeedsTick
  {
    get => this.needsTick;
    set
    {
      if (value == this.needsTick)
        return;
      this.needsTick = value;
      if (!value || this.Parent == null)
        return;
      this.Parent.ChildNeedsTick = true;
    }
  }

  public bool ChildNeedsTick
  {
    get => this.childNeedsTick;
    set
    {
      if (value == this.childNeedsTick)
        return;
      this.childNeedsTick = value;
      if (!value || this.Parent == null)
        return;
      this.Parent.ChildNeedsTick = true;
    }
  }

  /// <inheritdoc />
  public string Name { get; set; }

  public INetRoot Root { get; protected set; }

  public INetSerializable Parent
  {
    get => this.parent;
    set => this.SetParent(value);
  }

  public INetSerializable NetFields => (INetSerializable) this;

  protected virtual bool tickImpl() => false;

  public bool Tick()
  {
    if (this.needsTick)
      this.needsTick = this.tickImpl();
    if (this.childNeedsTick)
    {
      this.childNeedsTick = false;
      this.ForEachChild((Action<INetSerializable>) (child =>
      {
        if (!child.NeedsTick && !child.ChildNeedsTick)
          return;
        this.childNeedsTick |= child.Tick();
      }));
    }
    return this.childNeedsTick | this.needsTick;
  }

  public abstract void Read(BinaryReader reader, NetVersion version);

  public abstract void Write(BinaryWriter writer);

  public abstract void ReadFull(BinaryReader reader, NetVersion version);

  public abstract void WriteFull(BinaryWriter writer);

  protected uint GetLocalTick() => this.Root != null ? this.Root.Clock.GetLocalTick() : 0U;

  protected NetVersion GetLocalVersion()
  {
    NetVersion localVersion = new NetVersion();
    if (this.Root != null)
      localVersion.Set(this.Root.Clock.netVersion);
    return localVersion;
  }

  protected virtual void SetParent(INetSerializable parent)
  {
    this.parent = parent;
    if (parent != null)
    {
      this.Root = parent.Root;
      this.SetChildParents();
    }
    else
      this.ClearChildParents();
    this.MarkClean();
    this.ChangeVersion.Clear();
    this.minNextDirtyTime = 0U;
  }

  protected virtual void SetChildParents()
  {
    this.ForEachChild((Action<INetSerializable>) (child => child.Parent = (INetSerializable) this));
  }

  protected virtual void ClearChildParents()
  {
    this.ForEachChild((Action<INetSerializable>) (child =>
    {
      if (child.Parent != this)
        return;
      child.Parent = (INetSerializable) null;
    }));
  }

  protected virtual void ValidateChild(INetSerializable child)
  {
    if (child == null)
      throw new InvalidOperationException($"Net field '{this.Name}' incorrectly contains a null field.");
    if ((this.Parent != null || this.Root == this) && child.Parent != this)
      throw new InvalidOperationException($"Net field '{this.Name}' has child '{child.Name}' which is already linked to parent '{child.Parent?.Name ?? "<null>"}'.");
  }

  protected virtual void ValidateChildren()
  {
    if (this.Parent == null && this.Root != this)
      return;
    this.ForEachChild(new Action<INetSerializable>(this.ValidateChild));
  }

  protected virtual void ForEachChild(Action<INetSerializable> childAction)
  {
  }
}
