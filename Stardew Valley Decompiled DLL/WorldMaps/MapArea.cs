// Decompiled with JetBrains decompiler
// Type: StardewValley.WorldMaps.MapArea
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.GameData.WorldMaps;
using StardewValley.Internal;
using StardewValley.TokenizableStrings;
using System.Collections.Generic;

#nullable disable
namespace StardewValley.WorldMaps;

/// <summary>A smaller section of the map which is linked to one or more in-game locations. The map area might be edited/swapped depending on the context, have its own tooltip(s), or have its own player marker positions.</summary>
public class MapArea
{
  /// <summary>The cached value for <see cref="M:StardewValley.WorldMaps.MapArea.GetTextures" />.</summary>
  protected MapAreaTexture[] CachedTextures;
  /// <summary>The cached value for <see cref="M:StardewValley.WorldMaps.MapArea.GetTooltips" />.</summary>
  protected MapAreaTooltip[] CachedTooltips;
  /// <summary>The cached value for <see cref="M:StardewValley.WorldMaps.MapArea.GetWorldPositions" />.</summary>
  protected MapAreaPosition[] CachedWorldPositions;
  /// <summary>The cached value for <see cref="M:StardewValley.WorldMaps.MapArea.GetScrollText" />.</summary>
  protected string CachedScrollText;

  /// <summary>The unique identifier for the area.</summary>
  public string Id { get; }

  /// <summary>The large-scale part of the world (like the Valley) which contains this area.</summary>
  public MapRegion Region { get; }

  /// <summary>The underlying data.</summary>
  public WorldMapAreaData Data { get; }

  /// <summary>Construct an instance.</summary>
  /// <param name="region">The large-scale part of the world (like the Valley) which contains this area.</param>
  /// <param name="data">The underlying data.</param>
  public MapArea(MapRegion region, WorldMapAreaData data)
  {
    this.Data = data;
    this.Id = data.Id;
    this.Region = region;
  }

  /// <summary>Get the textures to draw onto the map (adjusted for pixel zoom), if any.</summary>
  public MapAreaTexture[] GetTextures()
  {
    if (this.CachedTextures == null)
    {
      if (this.Data.Textures.Count > 0)
      {
        List<MapAreaTexture> mapAreaTextureList = new List<MapAreaTexture>();
        foreach (WorldMapTextureData texture1 in this.Data.Textures)
        {
          if (GameStateQuery.CheckConditions(texture1.Condition))
          {
            Texture2D texture2;
            if (texture1.Condition == "IS_CUSTOM_FARM_TYPE")
            {
              string worldMapTexture = Game1.whichModFarm?.WorldMapTexture;
              if (worldMapTexture != null)
              {
                texture2 = this.GetTexture(worldMapTexture);
                if (texture2.Width <= 200)
                  texture1.SourceRect = texture2.Bounds;
              }
              else
                continue;
            }
            else
              texture2 = this.GetTexture(texture1.Texture);
            Rectangle sourceRect = texture1.SourceRect;
            if (sourceRect.IsEmpty)
              sourceRect = new Rectangle(0, 0, texture2.Width, texture2.Height);
            Rectangle mapPixelArea = texture1.MapPixelArea;
            if (mapPixelArea.IsEmpty)
              mapPixelArea = this.Data.PixelArea;
            mapPixelArea = new Rectangle(mapPixelArea.X * 4, mapPixelArea.Y * 4, mapPixelArea.Width * 4, mapPixelArea.Height * 4);
            mapAreaTextureList.Add(new MapAreaTexture(texture2, sourceRect, mapPixelArea));
          }
        }
        this.CachedTextures = mapAreaTextureList.ToArray();
      }
      else
        this.CachedTextures = LegacyShims.EmptyArray<MapAreaTexture>();
    }
    return this.CachedTextures;
  }

  /// <summary>Get the tooltips to draw onto the map, if any.</summary>
  public MapAreaTooltip[] GetTooltips()
  {
    if (this.CachedTooltips == null)
    {
      List<WorldMapTooltipData> tooltips = this.Data.Tooltips;
      // ISSUE: explicit non-virtual call
      if ((tooltips != null ? (__nonvirtual (tooltips.Count) > 0 ? 1 : 0) : 0) != 0)
      {
        List<MapAreaTooltip> mapAreaTooltipList = new List<MapAreaTooltip>();
        foreach (WorldMapTooltipData tooltip in this.Data.Tooltips)
        {
          if (GameStateQuery.CheckConditions(tooltip.Condition))
          {
            string text = GameStateQuery.CheckConditions(tooltip.KnownCondition) ? TokenParser.ParseText(Utility.TrimLines(tooltip.Text)) : "???";
            if (!string.IsNullOrWhiteSpace(text))
              mapAreaTooltipList.Add(new MapAreaTooltip(this, tooltip, text));
          }
        }
        this.CachedTooltips = mapAreaTooltipList.ToArray();
      }
      else
        this.CachedTooltips = LegacyShims.EmptyArray<MapAreaTooltip>();
    }
    return this.CachedTooltips;
  }

  /// <summary>Get all valid world positions in this area.</summary>
  public IEnumerable<MapAreaPosition> GetWorldPositions()
  {
    if (this.CachedWorldPositions == null)
    {
      List<MapAreaPosition> mapAreaPositionList = new List<MapAreaPosition>();
      foreach (WorldMapAreaPositionData worldPosition in this.Data.WorldPositions)
      {
        if (GameStateQuery.CheckConditions(worldPosition.Condition))
          mapAreaPositionList.Add(new MapAreaPosition(this, worldPosition));
      }
      this.CachedWorldPositions = mapAreaPositionList.ToArray();
    }
    return (IEnumerable<MapAreaPosition>) this.CachedWorldPositions;
  }

  /// <summary>Get a valid world position matching the given values, if any.</summary>
  /// <param name="locationName">The location name containing the tile.</param>
  /// <param name="contextName">The location's context name.</param>
  /// <param name="tile">The tile coordinate to match.</param>
  public MapAreaPosition GetWorldPosition(string locationName, string contextName, Point tile)
  {
    return this.GetWorldPosition(locationName, contextName, tile, (LogBuilder) null);
  }

  /// <summary>Get a valid world position matching the given values, if any.</summary>
  /// <param name="locationName">The location name containing the tile.</param>
  /// <param name="contextName">The location's context name.</param>
  /// <param name="tile">The tile coordinate to match.</param>
  /// <param name="log">The detailed log to update with the steps used to match the position, if set.</param>
  internal MapAreaPosition GetWorldPosition(
    string locationName,
    string contextName,
    Point tile,
    LogBuilder log)
  {
    LogBuilder indentedLog = log?.GetIndentedLog();
    foreach (MapAreaPosition worldPosition in this.GetWorldPositions())
    {
      log?.AppendLine($"Checking position '{worldPosition.Data.Id}'...");
      if (worldPosition.Matches(locationName, contextName, tile, indentedLog))
        return worldPosition;
    }
    return (MapAreaPosition) null;
  }

  /// <summary>Get the translated tooltip text to display when hovering the cursor over the map area.</summary>
  public virtual string GetScrollText()
  {
    if (this.CachedScrollText == null)
      this.CachedScrollText = TokenParser.ParseText(Utility.TrimLines(this.Data.ScrollText));
    return this.CachedScrollText;
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
