// Decompiled with JetBrains decompiler
// Type: StardewValley.BuildingPaintColor
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Netcode;
using System;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley;

public class BuildingPaintColor : INetObject<NetFields>
{
  public NetString ColorName = new NetString();
  public NetBool Color1Default = new NetBool(true);
  public NetInt Color1Hue = new NetInt();
  public NetInt Color1Saturation = new NetInt();
  public NetInt Color1Lightness = new NetInt();
  public NetBool Color2Default = new NetBool(true);
  public NetInt Color2Hue = new NetInt();
  public NetInt Color2Saturation = new NetInt();
  public NetInt Color2Lightness = new NetInt();
  public NetBool Color3Default = new NetBool(true);
  public NetInt Color3Hue = new NetInt();
  public NetInt Color3Saturation = new NetInt();
  public NetInt Color3Lightness = new NetInt();
  protected bool _dirty;

  [XmlIgnore]
  public NetFields NetFields { get; } = new NetFields(nameof (BuildingPaintColor));

  public BuildingPaintColor()
  {
    this.NetFields.SetOwner((INetObject<NetFields>) this).AddField((INetSerializable) this.ColorName, nameof (ColorName)).AddField((INetSerializable) this.Color1Default, nameof (Color1Default)).AddField((INetSerializable) this.Color2Default, nameof (Color2Default)).AddField((INetSerializable) this.Color3Default, nameof (Color3Default)).AddField((INetSerializable) this.Color1Hue, nameof (Color1Hue)).AddField((INetSerializable) this.Color1Saturation, nameof (Color1Saturation)).AddField((INetSerializable) this.Color1Lightness, nameof (Color1Lightness)).AddField((INetSerializable) this.Color2Hue, nameof (Color2Hue)).AddField((INetSerializable) this.Color2Saturation, nameof (Color2Saturation)).AddField((INetSerializable) this.Color2Lightness, nameof (Color2Lightness)).AddField((INetSerializable) this.Color3Hue, nameof (Color3Hue)).AddField((INetSerializable) this.Color3Saturation, nameof (Color3Saturation)).AddField((INetSerializable) this.Color3Lightness, nameof (Color3Lightness));
    this.Color1Default.fieldChangeVisibleEvent += new FieldChange<NetBool, bool>(this.OnDefaultFlagChanged);
    this.Color2Default.fieldChangeVisibleEvent += new FieldChange<NetBool, bool>(this.OnDefaultFlagChanged);
    this.Color3Default.fieldChangeVisibleEvent += new FieldChange<NetBool, bool>(this.OnDefaultFlagChanged);
    this.Color1Hue.fieldChangeVisibleEvent += new FieldChange<NetInt, int>(this.OnColorChanged);
    this.Color1Saturation.fieldChangeVisibleEvent += new FieldChange<NetInt, int>(this.OnColorChanged);
    this.Color1Lightness.fieldChangeVisibleEvent += new FieldChange<NetInt, int>(this.OnColorChanged);
    this.Color2Hue.fieldChangeVisibleEvent += new FieldChange<NetInt, int>(this.OnColorChanged);
    this.Color2Saturation.fieldChangeVisibleEvent += new FieldChange<NetInt, int>(this.OnColorChanged);
    this.Color2Lightness.fieldChangeVisibleEvent += new FieldChange<NetInt, int>(this.OnColorChanged);
    this.Color3Hue.fieldChangeVisibleEvent += new FieldChange<NetInt, int>(this.OnColorChanged);
    this.Color3Saturation.fieldChangeVisibleEvent += new FieldChange<NetInt, int>(this.OnColorChanged);
    this.Color3Lightness.fieldChangeVisibleEvent += new FieldChange<NetInt, int>(this.OnColorChanged);
  }

  public virtual void CopyFrom(BuildingPaintColor other)
  {
    this.ColorName.Value = other.ColorName.Value;
    this.Color1Default.Value = other.Color1Default.Value;
    this.Color1Hue.Value = other.Color1Hue.Value;
    this.Color1Saturation.Value = other.Color1Saturation.Value;
    this.Color1Lightness.Value = other.Color1Lightness.Value;
    this.Color2Default.Value = other.Color2Default.Value;
    this.Color2Hue.Value = other.Color2Hue.Value;
    this.Color2Saturation.Value = other.Color2Saturation.Value;
    this.Color2Lightness.Value = other.Color2Lightness.Value;
    this.Color3Default.Value = other.Color3Default.Value;
    this.Color3Hue.Value = other.Color3Hue.Value;
    this.Color3Saturation.Value = other.Color3Saturation.Value;
    this.Color3Lightness.Value = other.Color3Lightness.Value;
  }

  public virtual void OnDefaultFlagChanged(NetBool field, bool old_value, bool new_value)
  {
    this._dirty = true;
  }

  public virtual void OnColorChanged(NetInt field, int old_value, int new_value)
  {
    this._dirty = true;
  }

  public virtual void Poll(Action apply)
  {
    if (!this._dirty)
      return;
    if (apply != null)
      apply();
    this._dirty = false;
  }

  public bool IsDirty() => this._dirty;

  public bool RequiresRecolor()
  {
    return !this.Color1Default.Value || !this.Color2Default.Value || !this.Color3Default.Value;
  }
}
