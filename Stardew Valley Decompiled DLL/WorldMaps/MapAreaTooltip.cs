// Decompiled with JetBrains decompiler
// Type: StardewValley.WorldMaps.MapAreaTooltip
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using StardewValley.GameData.WorldMaps;

#nullable disable
namespace StardewValley.WorldMaps;

/// <summary>A tooltip shown when hovering over parts of a larger <see cref="T:StardewValley.WorldMaps.MapArea" /> on the world map.</summary>
public class MapAreaTooltip
{
  /// <summary>The cached value for <see cref="M:StardewValley.WorldMaps.MapAreaTooltip.GetPixelArea" />.</summary>
  protected Rectangle? CachedPixelArea;

  /// <summary>The map area which contains this position.</summary>
  public MapArea Area { get; }

  /// <summary>The underlying tooltip data.</summary>
  public WorldMapTooltipData Data { get; }

  /// <summary>The tooltip text to display.</summary>
  public string Text { get; }

  /// <summary>A unique ID for this tooltip within the map region.</summary>
  public string NamespacedId { get; }

  /// <summary>Construct an instance.</summary>
  /// <param name="mapArea">The map area which contains this position.</param>
  /// <param name="data">The underlying map position data.</param>
  /// <param name="text">The tooltip text to display.</param>
  public MapAreaTooltip(MapArea mapArea, WorldMapTooltipData data, string text)
  {
    this.Area = mapArea;
    this.Data = data;
    this.Text = text;
    this.NamespacedId = $"{mapArea.Id}/{data.Id}";
  }

  /// <summary>Get the pixel area within the map which can be hovered to show this tooltip, adjusted for pixel zoom.</summary>
  public Rectangle GetPixelArea()
  {
    if (!this.CachedPixelArea.HasValue)
    {
      Rectangle pixelArea = this.Data.PixelArea;
      if (pixelArea.IsEmpty)
        pixelArea = this.Area.Data.PixelArea;
      this.CachedPixelArea = new Rectangle?(new Rectangle(pixelArea.X * 4, pixelArea.Y * 4, pixelArea.Width * 4, pixelArea.Height * 4));
    }
    return this.CachedPixelArea.Value;
  }
}
