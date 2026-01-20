// Decompiled with JetBrains decompiler
// Type: StardewValley.Network.NetLocationRef
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Netcode;
using System;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley.Network;

/// <summary>A cached reference to a local location.</summary>
/// <remarks>This fetches and caches the location from <see cref="M:StardewValley.Game1.getLocationFromName(System.String)" /> based on the <see cref="P:StardewValley.Network.NetLocationRef.LocationName" /> and <see cref="P:StardewValley.Network.NetLocationRef.IsStructure" /> values.</remarks>
public class NetLocationRef : INetObject<NetFields>
{
  public readonly NetString locationName = new NetString();
  public readonly NetBool isStructure = new NetBool();
  protected GameLocation _gameLocation;
  protected bool _dirty = true;
  protected bool _usedLocalLocation;
  [XmlIgnore]
  public Action OnLocationChanged;

  /// <summary>The unique name of the target location.</summary>
  public string LocationName => this.locationName.Value;

  /// <summary>Whether the target location is a building interior.</summary>
  public bool IsStructure => this.isStructure.Value;

  /// <summary>The cached location instance.</summary>
  [XmlIgnore]
  public GameLocation Value
  {
    get => this.Get();
    set => this.Set(value);
  }

  [XmlIgnore]
  public NetFields NetFields { get; } = new NetFields(nameof (NetLocationRef));

  public NetLocationRef()
  {
    this.NetFields.SetOwner((INetObject<NetFields>) this).AddField((INetSerializable) this.locationName, nameof (locationName)).AddField((INetSerializable) this.isStructure, nameof (isStructure));
    this.locationName.fieldChangeVisibleEvent += (FieldChange<NetString, string>) ((_param1, _param2, _param3) => this._dirty = true);
    this.isStructure.fieldChangeVisibleEvent += (FieldChange<NetBool, bool>) ((_param1, _param2, _param3) => this._dirty = true);
  }

  public NetLocationRef(GameLocation value)
    : this()
  {
    this.Set(value);
  }

  public bool IsChanging() => this.locationName.IsChanging() || this.isStructure.IsChanging();

  /// <summary>Update the location instance if the <see cref="P:StardewValley.Network.NetLocationRef.LocationName" /> or <see cref="P:StardewValley.Network.NetLocationRef.IsStructure" /> values changed.</summary>
  /// <param name="forceUpdate">Whether to update the location reference even if the target values didn't change.</param>
  public void Update(bool forceUpdate = false)
  {
    if (forceUpdate)
      this._dirty = true;
    this.ApplyChangesIfDirty();
  }

  public void ApplyChangesIfDirty()
  {
    if (this._usedLocalLocation && this._gameLocation != Game1.currentLocation)
    {
      this._dirty = true;
      this._usedLocalLocation = false;
    }
    if (this._dirty)
    {
      this._gameLocation = Game1.getLocationFromName(this.locationName.Value, this.isStructure.Value);
      this._dirty = false;
      Action onLocationChanged = this.OnLocationChanged;
      if (onLocationChanged != null)
        onLocationChanged();
    }
    if (this._usedLocalLocation || this._gameLocation == Game1.currentLocation || !this.IsCurrentlyViewedLocation())
      return;
    this._usedLocalLocation = true;
    this._gameLocation = Game1.currentLocation;
  }

  public GameLocation Get()
  {
    this.ApplyChangesIfDirty();
    return this._gameLocation;
  }

  public void Set(GameLocation location)
  {
    if (location == null)
    {
      this.isStructure.Value = false;
      this.locationName.Value = "";
    }
    else
    {
      this.isStructure.Value = location.isStructure.Value;
      this.locationName.Value = location.NameOrUniqueName;
    }
    if (this.IsCurrentlyViewedLocation())
    {
      this._usedLocalLocation = true;
      this._gameLocation = Game1.currentLocation;
    }
    else
      this._gameLocation = location;
    bool? isTemporary = this._gameLocation?.IsTemporary;
    if (isTemporary.HasValue && isTemporary.GetValueOrDefault())
      this._gameLocation = (GameLocation) null;
    this._dirty = false;
  }

  public bool IsCurrentlyViewedLocation()
  {
    return Game1.currentLocation != null && this.locationName.Value == Game1.currentLocation.NameOrUniqueName;
  }
}
