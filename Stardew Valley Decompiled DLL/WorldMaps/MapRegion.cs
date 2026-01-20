// Decompiled with JetBrains decompiler
// Type: StardewValley.WorldMaps.MapRegion
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.GameData.WorldMaps;
using StardewValley.Internal;
using StardewValley.Locations;
using System;
using System.Collections.Generic;

#nullable disable
namespace StardewValley.WorldMaps;

/// <inheritdoc cref="T:StardewValley.GameData.WorldMaps.WorldMapRegionData" />
public class MapRegion
{
  /// <summary>The cached value for <see cref="M:StardewValley.WorldMaps.MapRegion.GetMapPixelBounds" />.</summary>
  protected Rectangle? CachedPixelBounds;
  /// <summary>The cached value for <see cref="M:StardewValley.WorldMaps.MapRegion.GetAreas" />.</summary>
  protected MapArea[] CachedMapAreas;
  /// <summary>The cached value for <see cref="M:StardewValley.WorldMaps.MapRegion.GetBaseTexture" />.</summary>
  protected MapAreaTexture CachedBaseTexture;

  /// <summary>The unique identifier for the region.</summary>
  public string Id { get; }

  /// <summary>The underlying data.</summary>
  public WorldMapRegionData Data { get; }

  /// <summary>Construct an instance.</summary>
  /// <param name="id">The area ID.</param>
  /// <param name="data">The underlying data.</param>
  public MapRegion(string id, WorldMapRegionData data)
  {
    this.Id = id;
    this.Data = data;
  }

  /// <summary>Get a pixel area on screen which contains all the map areas being drawn, centered on-screen.</summary>
  public Rectangle GetMapPixelBounds()
  {
    if (!this.CachedPixelBounds.HasValue)
    {
      MapAreaTexture baseTexture = this.GetBaseTexture();
      MapArea[] areas = this.GetAreas();
      int num1 = baseTexture != null ? baseTexture.MapPixelArea.Width : 0;
      int num2 = baseTexture != null ? baseTexture.MapPixelArea.Height : 0;
      foreach (MapArea mapArea in areas)
      {
        foreach (MapAreaTexture texture in mapArea.GetTextures())
        {
          num1 = Math.Max(num1, texture.MapPixelArea.Width);
          num2 = Math.Max(num2, texture.MapPixelArea.Height);
        }
      }
      Vector2 centeringOnScreen = Utility.getTopLeftPositionForCenteringOnScreen(num1, num2);
      this.CachedPixelBounds = new Rectangle?(new Rectangle((int) centeringOnScreen.X, (int) centeringOnScreen.Y, num1 / 4, num2 / 4));
    }
    return this.CachedPixelBounds.Value;
  }

  /// <summary>Get the base texture to draw under the map areas (adjusted for pixel zoom), if any.</summary>
  public MapAreaTexture GetBaseTexture()
  {
    if (this.CachedBaseTexture == null)
    {
      if (this.Data.BaseTexture.Count > 0)
      {
        foreach (WorldMapTextureData worldMapTextureData in this.Data.BaseTexture)
        {
          if (GameStateQuery.CheckConditions(worldMapTextureData.Condition))
          {
            Texture2D texture = this.GetTexture(worldMapTextureData.Texture);
            Rectangle sourceRect = worldMapTextureData.SourceRect;
            if (sourceRect.IsEmpty)
              sourceRect = new Rectangle(0, 0, texture.Width, texture.Height);
            Rectangle mapPixelArea = worldMapTextureData.MapPixelArea;
            if (mapPixelArea.IsEmpty)
              mapPixelArea = sourceRect;
            mapPixelArea = new Rectangle(mapPixelArea.X * 4, mapPixelArea.Y * 4, mapPixelArea.Width * 4, mapPixelArea.Height * 4);
            this.CachedBaseTexture = new MapAreaTexture(texture, sourceRect, mapPixelArea);
            break;
          }
        }
      }
      if (this.CachedBaseTexture == null)
        this.CachedBaseTexture = new MapAreaTexture((Texture2D) null, Rectangle.Empty, Rectangle.Empty);
    }
    return this.CachedBaseTexture.Texture == null ? (MapAreaTexture) null : this.CachedBaseTexture;
  }

  /// <summary>Get all areas that are part of the region.</summary>
  public MapArea[] GetAreas()
  {
    if (this.CachedMapAreas == null)
    {
      List<MapArea> mapAreaList = new List<MapArea>();
      foreach (WorldMapAreaData mapArea in this.Data.MapAreas)
      {
        if (GameStateQuery.CheckConditions(mapArea.Condition))
          mapAreaList.Add(new MapArea(this, mapArea));
      }
      this.CachedMapAreas = mapAreaList.ToArray();
    }
    return this.CachedMapAreas;
  }

  /// <summary>Get the map position which contains a given location and tile coordinate, if any.</summary>
  /// <param name="location">The in-game location.</param>
  /// <param name="tile">The tile coordinate within the location.</param>
  public MapAreaPosition GetPositionData(GameLocation location, Point tile)
  {
    return this.GetPositionData(location, tile, (LogBuilder) null);
  }

  /// <summary>Get the map position which contains a given location and tile coordinate, if any.</summary>
  /// <param name="location">The in-game location.</param>
  /// <param name="tile">The tile coordinate within the location.</param>
  /// <param name="log">The detailed log to update with the steps used to match the position, if set.</param>
  internal MapAreaPosition GetPositionData(GameLocation location, Point tile, LogBuilder log)
  {
    if (location == null)
    {
      log?.AppendLine("Skipped: location is null.");
      return (MapAreaPosition) null;
    }
    string locationName = this.GetLocationName(location);
    string locationContextId = location.GetLocationContextId();
    LogBuilder indentedLog = log?.GetIndentedLog();
    foreach (MapArea area in this.GetAreas())
    {
      log?.AppendLine($"Checking map area '{area.Id}'...");
      MapAreaPosition worldPosition = area.GetWorldPosition(locationName, locationContextId, tile, indentedLog);
      if (worldPosition != null)
        return worldPosition;
    }
    return (MapAreaPosition) null;
  }

  /// <summary>Get a location's name as it appears in <c>Data/WorldMap</c>.</summary>
  /// <param name="location">The location whose name to get.</param>
  /// <remarks>For example, mine levels have internal names like <c>UndergroundMine14</c>, but they're all covered by <c>Mines</c> or <c>SkullCave</c> in <c>Data/Maps</c>.</remarks>
  protected string GetLocationName(GameLocation location)
  {
    string str = !location.IsTemporary || string.IsNullOrEmpty(location.Map.Id) ? location.Name : location.Map.Id;
    if (str == "Mine")
      return "Mines";
    return location is MineShaft mineShaft ? (mineShaft.mineLevel <= 120 || mineShaft.mineLevel == 77377 ? "Mines" : "SkullCave") : (VolcanoDungeon.IsGeneratedLevel(location.Name) ? "VolcanoDungeon" : str);
  }

  /// <summary>Get the texture to load for an asset name.</summary>
  /// <param name="assetName">The asset name to load.</param>
  private Texture2D GetTexture(string assetName)
  {
    if (Game1.season != Season.Spring)
    {
      string assetName1 = $"{assetName}_{Game1.currentSeason.ToLower()}";
      if (Game1.content.DoesAssetExist<Texture2D>(assetName1))
        return Game1.content.Load<Texture2D>(assetName1);
    }
    return Game1.content.Load<Texture2D>(assetName);
  }
}
