// Decompiled with JetBrains decompiler
// Type: Netcode.NetString
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using System.IO;

#nullable disable
namespace Netcode;

public class NetString : NetField<string, NetString>
{
  public int Length => this.Value.Length;

  public NetString()
    : base((string) null)
  {
  }

  public NetString(string value)
    : base(value)
  {
  }

  public event NetString.FilterString FilterStringEvent;

  public override void Set(string newValue)
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

  public bool Contains(string substr) => this.Value != null && this.Value.Contains(substr);

  protected override void ReadDelta(BinaryReader reader, NetVersion version)
  {
    string newValue = (string) null;
    if (reader.ReadBoolean())
    {
      newValue = reader.ReadString();
      if (this.FilterStringEvent != null)
        newValue = this.FilterStringEvent(newValue);
    }
    if (!version.IsPriorityOver(this.ChangeVersion))
      return;
    this.setInterpolationTarget(newValue);
  }

  protected override void WriteDelta(BinaryWriter writer)
  {
    writer.Write(this.value != null);
    if (this.value == null)
      return;
    writer.Write(this.value);
  }

  public delegate string FilterString(string newValue);
}
