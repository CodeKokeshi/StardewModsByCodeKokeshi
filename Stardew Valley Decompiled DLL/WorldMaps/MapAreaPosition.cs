// Decompiled with JetBrains decompiler
// Type: StardewValley.WorldMaps.MapAreaPosition
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using StardewValley.GameData.WorldMaps;
using StardewValley.Internal;
using StardewValley.TokenizableStrings;
using System;
using System.Collections.Generic;
using xTile.Dimensions;

#nullable disable
namespace StardewValley.WorldMaps;

/// <summary>Maps in-game locations and tile positions to the parent <see cref="T:StardewValley.WorldMaps.MapArea" />.</summary>
public class MapAreaPosition
{
  /// <summary>The cached map pixel area for <see cref="M:StardewValley.WorldMaps.MapAreaPosition.GetMapPixelPosition(StardewValley.GameLocation,Microsoft.Xna.Framework.Point)" />, adjusted for zoom.</summary>
  protected Microsoft.Xna.Framework.Rectangle? CachedMapPixelArea;
  /// <summary>The cached value for <see cref="M:StardewValley.WorldMaps.MapAreaPosition.GetScrollText(Microsoft.Xna.Framework.Point)" />.</summary>
  protected string CachedScrollText;
  /// <summary>Whether this is mapped to a fixed pixel coordinate on the map.</summary>
  protected bool IsFixedMapPosition;

  /// <summary>The map region which contains this position.</summary>
  public MapRegion Region { get; }

  /// <summary>The map area which contains this position.</summary>
  public MapArea Area { get; }

  /// <summary>The underlying map position data.</summary>
  public WorldMapAreaPositionData Data { get; }

  /// <summary>Construct an instance.</summary>
  /// <param name="mapArea">The map area which contains this position.</param>
  /// <param name="data">The underlying map position data.</param>
  public MapAreaPosition(MapArea mapArea, WorldMapAreaPositionData data)
  {
    this.Region = mapArea.Region;
    this.Area = mapArea;
    this.Data = data;
  }

  /// <summary>Get whether this position matches the given values.</summary>
  /// <param name="locationName">The location name containing the tile.</param>
  /// <param name="contextName">The location's context name.</param>
  /// <param name="tile">The tile coordinate to match.</param>
  public bool Matches(string locationName, string contextName, Point tile)
  {
    return this.Matches(locationName, contextName, tile, (LogBuilder) null);
  }

  /// <summary>Get whether this position matches the given values.</summary>
  /// <param name="locationName">The location name containing the tile.</param>
  /// <param name="contextName">The location's context name.</param>
  /// <param name="tile">The tile coordinate to match.</param>
  /// <param name="log">The detailed log to update with the steps used to match the position, if set.</param>
  internal bool Matches(string locationName, string contextName, Point tile, LogBuilder log)
  {
    WorldMapAreaPositionData data = this.Data;
    if (data.LocationContext != null && data.LocationContext != contextName)
    {
      if (log != null)
        log.AppendLine($"Skipped: location context '{contextName}' doesn't match required context '{data.LocationContext}'.");
      return false;
    }
    if (data.LocationName != null && data.LocationName != locationName)
    {
      if (log != null)
        log.AppendLine($"Skipped: location '{locationName}' doesn't match required location '{data.LocationName}'.");
      return false;
    }
    List<string> locationNames = data.LocationNames;
    // ISSUE: explicit non-virtual call
    if ((locationNames != null ? (__nonvirtual (locationNames.Count) > 0 ? 1 : 0) : 0) != 0 && !data.LocationNames.Contains(locationName))
    {
      if (log != null)
        log.AppendLine($"Skipped: location '{locationName}' doesn't match one of the required locations '{string.Join("', '", (IEnumerable<string>) data.LocationNames)}'.");
      return false;
    }
    if (!this.IsTileWithinZone(tile))
    {
      if (log != null)
        log.AppendLine($"Skipped: tile position {tile} doesn't match required tile zone {this.Data.ExtendedTileArea ?? this.Data.TileArea}.");
      return false;
    }
    log?.AppendLine("Matched successfully.");
    return true;
  }

  /// <summary>Get the pixel area covered by this position, adjusted for pixel zoom.</summary>
  public Microsoft.Xna.Framework.Rectangle GetPixelArea()
  {
    if (!this.CachedMapPixelArea.HasValue)
    {
      Microsoft.Xna.Framework.Rectangle rectangle = this.Data.MapPixelArea;
      if (rectangle.IsEmpty)
        rectangle = this.Area.Data.PixelArea;
      this.CachedMapPixelArea = new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(rectangle.X * 4, rectangle.Y * 4, rectangle.Width * 4, rectangle.Height * 4));
      this.IsFixedMapPosition = rectangle.Width <= 1 && rectangle.Height <= 1;
    }
    return this.CachedMapPixelArea.Value;
  }

  /// <summary>Get the pixel position within the world map which corresponds to an in-game location's tile within the map area, adjusted for pixel zoom.</summary>
  /// <param name="location">The in-game location containing the tile.</param>
  /// <param name="tileLocation">The tile position within the location.</param>
  public Vector2 GetMapPixelPosition(GameLocation location, Point tileLocation)
  {
    Microsoft.Xna.Framework.Rectangle pixelArea = this.GetPixelArea();
    if (this.IsFixedMapPosition)
      return new Vector2((float) pixelArea.X, (float) pixelArea.Y);
    Vector2? positionRatioIfValid = this.GetPositionRatioIfValid(location, tileLocation);
    if (positionRatioIfValid.HasValue)
      return new Vector2(Utility.Lerp((float) pixelArea.Left, (float) pixelArea.Right, positionRatioIfValid.Value.X), Utility.Lerp((float) pixelArea.Top, (float) pixelArea.Bottom, positionRatioIfValid.Value.Y));
    Point center = pixelArea.Center;
    return new Vector2((float) center.X, (float) center.Y);
  }

  /// <summary>Get the translated display name to show when the player is in this position.</summary>
  /// <param name="playerTile">The player's tile position within the position.</param>
  public string GetScrollText(Point playerTile)
  {
    if (this.CachedScrollText == null)
    {
      string scrollText = this.Data.ScrollText;
      List<WorldMapAreaPositionScrollTextZoneData> scrollTextZones = this.Data.ScrollTextZones;
      // ISSUE: explicit non-virtual call
      if ((scrollTextZones != null ? (__nonvirtual (scrollTextZones.Count) > 0 ? 1 : 0) : 0) != 0)
      {
        foreach (WorldMapAreaPositionScrollTextZoneData scrollTextZone in this.Data.ScrollTextZones)
        {
          if (scrollTextZone.TileArea.Contains(playerTile))
          {
            scrollText = scrollTextZone.ScrollText;
            break;
          }
        }
      }
      this.CachedScrollText = scrollText != null ? TokenParser.ParseText(Utility.TrimLines(scrollText)) : this.Area.GetScrollText();
    }
    return this.CachedScrollText;
  }

  /// <summary>Get the player's position as a percentage along the X and Y axes.</summary>
  /// <param name="location">The in-game location containing the tile.</param>
  /// <param name="tile">The tile position within the location.</param>
  public virtual Vector2? GetPositionRatioIfValid(GameLocation location, Point tile)
  {
    if (location?.map == null || !this.IsTileWithinZone(tile))
      return new Vector2?();
    Size layerSize = location.map.Layers[0].LayerSize;
    Microsoft.Xna.Framework.Rectangle rectangle = this.Data.TileArea;
    if (rectangle.IsEmpty || rectangle.Right > layerSize.Width || rectangle.Bottom > layerSize.Height)
      rectangle = rectangle.IsEmpty ? new Microsoft.Xna.Framework.Rectangle(0, 0, layerSize.Width, layerSize.Height) : new Microsoft.Xna.Framework.Rectangle(rectangle.X, rectangle.Y, Math.Min(rectangle.Width, layerSize.Width - rectangle.X), Math.Min(rectangle.Height, layerSize.Height - rectangle.Y));
    double num1 = (double) MathHelper.Clamp(tile.X, rectangle.X, rectangle.Right - 1);
    float num2 = (float) MathHelper.Clamp(tile.Y, rectangle.Y, rectangle.Bottom - 1);
    double x = (double) rectangle.X;
    return new Vector2?(new Vector2((float) (num1 - x) / (float) rectangle.Width, (num2 - (float) rectangle.Y) / (float) rectangle.Height));
  }

  /// <summary>Get whether a tile position is within the bounds of this position data.</summary>
  /// <param name="tile">The tile position within the location.</param>
  public virtual bool IsTileWithinZone(Point tile)
  {
    Microsoft.Xna.Framework.Rectangle rectangle = this.Data.ExtendedTileArea ?? this.Data.TileArea;
    return rectangle.IsEmpty || rectangle.Contains(tile);
  }
}
