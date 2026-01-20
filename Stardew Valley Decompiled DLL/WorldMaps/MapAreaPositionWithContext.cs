// Decompiled with JetBrains decompiler
// Type: StardewValley.WorldMaps.MapAreaPositionWithContext
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;

#nullable disable
namespace StardewValley.WorldMaps;

/// <summary>The data which maps an in-game location and tile position to a parent <see cref="T:StardewValley.WorldMaps.MapArea" />, along with the location and tile that matched it.</summary>
/// <summary>Construct an instance.</summary>
/// <param name="data"><inheritdoc cref="P:StardewValley.WorldMaps.MapAreaPositionWithContext.Data" path="/summary" /></param>
/// <param name="location"><inheritdoc cref="P:StardewValley.WorldMaps.MapAreaPositionWithContext.Location" path="/summary" /></param>
/// <param name="tile"><inheritdoc cref="P:StardewValley.WorldMaps.MapAreaPositionWithContext.Tile" path="/summary" /></param>
public readonly struct MapAreaPositionWithContext(
  MapAreaPosition data,
  GameLocation location,
  Point tile)
{
  /// <summary>The data which maps an in-game location and tile position to a parent <see cref="T:StardewValley.WorldMaps.MapArea" />.</summary>
  public MapAreaPosition Data { get; } = data;

  /// <summary>The location for which the <see cref="P:StardewValley.WorldMaps.MapAreaPositionWithContext.Data" /> was selected.</summary>
  public GameLocation Location { get; } = location;

  /// <summary>The location for which the <see cref="P:StardewValley.WorldMaps.MapAreaPositionWithContext.Tile" /> was selected.</summary>
  public Point Tile { get; } = tile;

  /// <summary>Get the pixel position within the world map which corresponds to the in-game location's tile within the map area, adjusted for pixel zoom.</summary>
  public Vector2 GetMapPixelPosition() => this.Data.GetMapPixelPosition(this.Location, this.Tile);

  /// <summary>Get the player's position as a percentage along the X and Y axes.</summary>
  public Vector2? GetPositionRatioIfValid()
  {
    return this.Data.GetPositionRatioIfValid(this.Location, this.Tile);
  }

  /// <summary>Get the translated display name to show when the player is in this position.</summary>
  public string GetScrollText() => this.Data.GetScrollText(this.Tile);
}
