// Decompiled with JetBrains decompiler
// Type: StardewValley.WorldMaps.WorldMapManager
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using StardewValley.Buildings;
using StardewValley.GameData.WorldMaps;
using StardewValley.Internal;
using System.Collections.Generic;

#nullable disable
namespace StardewValley.WorldMaps;

/// <summary>Manages data related to the world map shown in the game menu.</summary>
public static class WorldMapManager
{
  /// <summary>The <see cref="F:StardewValley.Game1.ticks" /> value when cached data should be reset.</summary>
  private static int NextClearCacheTick;
  /// <summary>The maximum update ticks before any cached data should be refreshed.</summary>
  private static int MaxCacheTicks = 3600;
  /// <summary>The cached map regions.</summary>
  private static readonly List<MapRegion> Regions = new List<MapRegion>();

  /// <summary>Initialize before the class is first accessed.</summary>
  static WorldMapManager() => WorldMapManager.ReloadData();

  /// <summary>Load the raw world map data.</summary>
  public static void ReloadData()
  {
    WorldMapManager.Regions.Clear();
    foreach (KeyValuePair<string, WorldMapRegionData> world in DataLoader.WorldMap(Game1.content))
      WorldMapManager.Regions.Add(new MapRegion(world.Key, world.Value));
    WorldMapManager.NextClearCacheTick = Game1.ticks + WorldMapManager.MaxCacheTicks;
  }

  /// <summary>Get all map regions in the underlying data which are currently valid.</summary>
  public static IEnumerable<MapRegion> GetMapRegions()
  {
    WorldMapManager.ReloadDataIfStale();
    return (IEnumerable<MapRegion>) WorldMapManager.Regions;
  }

  /// <summary>Get the map position which contains a given location and tile coordinate, if any.</summary>
  /// <param name="location">The in-game location.</param>
  /// <param name="tile">The tile coordinate within the location.</param>
  public static MapAreaPositionWithContext? GetPositionData(GameLocation location, Point tile)
  {
    return WorldMapManager.GetPositionData(location, tile, (LogBuilder) null);
  }

  /// <summary>Get the map position which contains a given location and tile coordinate, if any.</summary>
  /// <param name="location">The in-game location.</param>
  /// <param name="tile">The tile coordinate within the location.</param>
  /// <param name="log">The detailed log to update with the steps used to match the position, if set.</param>
  internal static MapAreaPositionWithContext? GetPositionData(
    GameLocation location,
    Point tile,
    LogBuilder log)
  {
    if (location == null)
    {
      log?.AppendLine("Skipped: location is null.");
      return new MapAreaPositionWithContext?();
    }
    LogBuilder indentedLog = log?.GetIndentedLog();
    log?.AppendLine("Searching for the player position...");
    MapAreaPosition dataWithoutFallback1 = WorldMapManager.GetPositionDataWithoutFallback(location, tile, indentedLog);
    if (dataWithoutFallback1 != null)
    {
      log?.AppendLine($"Found match: position '{dataWithoutFallback1.Data.Id}'.");
      return new MapAreaPositionWithContext?(new MapAreaPositionWithContext(dataWithoutFallback1, location, tile));
    }
    Building parentBuilding = location.ParentBuilding;
    GameLocation parentLocation = parentBuilding?.GetParentLocation();
    if (parentLocation != null)
    {
      log?.AppendLine("");
      if (log != null)
        log.AppendLine($"Searching for the exterior position of the '{parentBuilding.buildingType.Value}' building in {parentLocation.NameOrUniqueName}...");
      Point tile1 = new Point(parentBuilding.tileX.Value + parentBuilding.tilesWide.Value / 2, parentBuilding.tileY.Value + parentBuilding.tilesHigh.Value / 2);
      MapAreaPosition dataWithoutFallback2 = WorldMapManager.GetPositionDataWithoutFallback(parentLocation, tile1, indentedLog);
      if (dataWithoutFallback2 != null)
      {
        log?.AppendLine($"Found match: position '{dataWithoutFallback2.Data.Id}'.");
        return new MapAreaPositionWithContext?(new MapAreaPositionWithContext(dataWithoutFallback2, parentLocation, tile1));
      }
    }
    log?.AppendLine("");
    log?.AppendLine("No match found.");
    return new MapAreaPositionWithContext?();
  }

  /// <summary>Get the map position which contains a given location and tile coordinate, if any, without checking for parent buildings or locations.</summary>
  /// <param name="location">The in-game location.</param>
  /// <param name="tile">The tile coordinate within the location.</param>
  public static MapAreaPosition GetPositionDataWithoutFallback(GameLocation location, Point tile)
  {
    return WorldMapManager.GetPositionDataWithoutFallback(location, tile, (LogBuilder) null);
  }

  /// <summary>Get the map position which contains a given location and tile coordinate, if any, without checking for parent buildings or locations.</summary>
  /// <param name="location">The in-game location.</param>
  /// <param name="tile">The tile coordinate within the location.</param>
  /// <param name="log">The detailed log to update with the steps used to match the position, if set.</param>
  internal static MapAreaPosition GetPositionDataWithoutFallback(
    GameLocation location,
    Point tile,
    LogBuilder log)
  {
    if (location == null)
    {
      log?.AppendLine("Skipped: location is null.");
      return (MapAreaPosition) null;
    }
    LogBuilder indentedLog = log?.GetIndentedLog();
    foreach (MapRegion mapRegion in WorldMapManager.GetMapRegions())
    {
      log?.AppendLine($"Checking region '{mapRegion.Id}'...");
      MapAreaPosition positionData = mapRegion.GetPositionData(location, tile, indentedLog);
      if (positionData != null)
        return positionData;
    }
    return (MapAreaPosition) null;
  }

  /// <summary>Update the world map data if needed.</summary>
  private static void ReloadDataIfStale()
  {
    if (Game1.ticks < WorldMapManager.NextClearCacheTick)
      return;
    WorldMapManager.ReloadData();
  }
}
