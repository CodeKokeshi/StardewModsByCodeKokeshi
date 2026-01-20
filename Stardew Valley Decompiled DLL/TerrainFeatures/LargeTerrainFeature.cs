// Decompiled with JetBrains decompiler
// Type: StardewValley.TerrainFeatures.LargeTerrainFeature
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Netcode;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley.TerrainFeatures;

[XmlInclude(typeof (Bush))]
public abstract class LargeTerrainFeature(bool needsTick) : TerrainFeature(needsTick)
{
  /// <summary>The backing field for <see cref="P:StardewValley.TerrainFeatures.LargeTerrainFeature.Tile" />.</summary>
  [XmlElement("tilePosition")]
  public readonly NetVector2 netTilePosition = new NetVector2();
  public bool isDestroyedByNPCTrample;

  /// <inheritdoc />
  [XmlIgnore]
  public override Vector2 Tile
  {
    get => this.netTilePosition.Value;
    set => this.netTilePosition.Value = value;
  }

  public override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.netTilePosition, "netTilePosition");
  }

  public virtual void onDestroy()
  {
  }
}
