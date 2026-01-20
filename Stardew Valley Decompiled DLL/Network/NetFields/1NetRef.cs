// Decompiled with JetBrains decompiler
// Type: Netcode.NetExtendableRef`2
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using StardewValley.Util;
using System;
using System.IO;

#nullable disable
namespace Netcode;

public class NetExtendableRef<T, TSelf> : NetRefBase<T, TSelf>
  where T : class, INetObject<INetSerializable>
  where TSelf : NetExtendableRef<T, TSelf>
{
  public NetExtendableRef() => this.notifyOnTargetValueChange = true;

  public NetExtendableRef(T value)
    : this()
  {
    this.cleanSet(value);
  }

  protected override void ForEachChild(Action<INetSerializable> childAction)
  {
    if ((object) this.targetValue == null)
      return;
    childAction(this.targetValue.NetFields);
  }

  protected override void ReadValueFull(T value, BinaryReader reader, NetVersion version)
  {
    value.NetFields.ReadFull(reader, version);
  }

  protected override void ReadValueDelta(BinaryReader reader, NetVersion version)
  {
    this.targetValue.NetFields.Read(reader, version);
  }

  private void clearValueParent(T targetValue)
  {
    if (targetValue.NetFields.Parent != this)
      return;
    targetValue.NetFields.Parent = (INetSerializable) null;
  }

  private void setValueParent(T targetValue)
  {
    if (targetValue?.NetFields == null)
    {
      string message;
      if ((object) targetValue != null)
        message = $"Can't change net field parent for {targetValue.GetType().FullName} type's null {"NetFields"} to '{this.Name}'.";
      else
        message = $"Can't change net field parent for null target to '{this.Name}.";
      NetHelper.LogWarning(message);
      NetHelper.LogVerbose(new StackTraceHelper().ToString());
    }
    else
    {
      if (this.Parent != null || this.Root == this)
      {
        if (targetValue.NetFields.Parent != null && targetValue.NetFields.Parent != this)
        {
          NetHelper.LogWarning($"Changing net field parent for '{targetValue.NetFields.Name}' collection from '{targetValue.NetFields.Parent.Name}' to '{this.Name}'.");
          NetHelper.LogVerbose(new StackTraceHelper().ToString());
        }
        targetValue.NetFields.Parent = (INetSerializable) this;
      }
      targetValue.NetFields.MarkClean();
    }
  }

  protected override void targetValueChanged(T oldValue, T newValue)
  {
    base.targetValueChanged(oldValue, newValue);
    if ((object) oldValue != null)
      this.clearValueParent(oldValue);
    if ((object) newValue == null)
      return;
    this.setValueParent(newValue);
  }

  protected override void WriteValueFull(BinaryWriter writer)
  {
    this.targetValue.NetFields.WriteFull(writer);
  }

  protected override void WriteValueDelta(BinaryWriter writer)
  {
    this.targetValue.NetFields.Write(writer);
  }
}
