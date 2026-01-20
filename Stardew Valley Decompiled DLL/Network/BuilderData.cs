// Decompiled with JetBrains decompiler
// Type: StardewValley.Network.BuilderData
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Netcode;

#nullable disable
namespace StardewValley.Network;

public class BuilderData : INetObject<NetFields>
{
  /// <summary>The current building type (i.e. the one being constructed, or the one being upgraded from).</summary>
  public NetString buildingType = new NetString();
  /// <summary>The number of days until it's completed.</summary>
  public NetInt daysUntilBuilt = new NetInt();
  /// <summary>The unique name for the location containing the building.</summary>
  public NetString buildingLocation = new NetString();
  /// <summary>The building's tile position within the <see cref="F:StardewValley.Network.BuilderData.buildingLocation" /> location.</summary>
  public NetPoint buildingTile = new NetPoint();
  /// <summary>Whether this is an upgrade (instead of a new building being constructed).</summary>
  public NetBool isUpgrade = new NetBool();

  public NetFields NetFields { get; } = new NetFields(nameof (BuilderData));

  /// <summary>Construct an empty instance.</summary>
  public BuilderData()
  {
    this.NetFields.SetOwner((INetObject<NetFields>) this).AddField((INetSerializable) this.buildingType, nameof (buildingType)).AddField((INetSerializable) this.daysUntilBuilt, nameof (daysUntilBuilt)).AddField((INetSerializable) this.buildingLocation, nameof (buildingLocation)).AddField((INetSerializable) this.buildingTile, nameof (buildingTile)).AddField((INetSerializable) this.isUpgrade, nameof (isUpgrade));
  }

  /// <summary>Construct an instance.</summary>
  /// <param name="buildingType">The current building type (i.e. the one being constructed, or the one being upgraded from).</param>
  /// <param name="daysUntilBuilt">The number of days until it's completed.</param>
  /// <param name="location">The unique name for the location containing the building.</param>
  /// <param name="tile">The building's tile position within the <see cref="F:StardewValley.Network.BuilderData.buildingLocation" /> location.</param>
  /// <param name="isUpgrade">Whether this is an upgrade (instead of a new building being constructed).</param>
  public BuilderData(
    string buildingType,
    int daysUntilBuilt,
    string location,
    Point tile,
    bool isUpgrade)
    : this()
  {
    this.buildingType.Value = buildingType;
    this.daysUntilBuilt.Value = daysUntilBuilt;
    this.buildingLocation.Value = location;
    this.buildingTile.Value = tile;
    this.isUpgrade.Value = isUpgrade;
  }
}
