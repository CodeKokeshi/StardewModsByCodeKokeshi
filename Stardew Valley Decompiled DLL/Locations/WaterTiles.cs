// Decompiled with JetBrains decompiler
// Type: StardewValley.WaterTiles
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

#nullable disable
namespace StardewValley;

public class WaterTiles
{
  /// <summary>The water data for each tile in the grid.</summary>
  public WaterTiles.WaterTileData[,] waterTiles;

  /// <summary>Construct an instance.</summary>
  /// <param name="source">The grid of tiles to represent, where each value indicates whether it's water.</param>
  public WaterTiles(bool[,] source)
  {
    int length1 = source.GetLength(0);
    int length2 = source.GetLength(1);
    this.waterTiles = new WaterTiles.WaterTileData[length1, length2];
    for (int index1 = 0; index1 < length1; ++index1)
    {
      for (int index2 = 0; index2 < length2; ++index2)
        this.waterTiles[index1, index2] = new WaterTiles.WaterTileData(source[index1, index2], true);
    }
  }

  /// <summary>Construct an instance.</summary>
  /// <param name="width">The width of the tile grid.</param>
  /// <param name="height">The height of the tile grid.</param>
  public WaterTiles(int width, int height)
  {
    this.waterTiles = new WaterTiles.WaterTileData[width, height];
    for (int index1 = 0; index1 < width; ++index1)
    {
      for (int index2 = 0; index2 < height; ++index2)
        this.waterTiles[index1, index2] = new WaterTiles.WaterTileData(false, true);
    }
  }

  /// <summary>Get or set whether a tile is water.</summary>
  /// <param name="x">The tile's X tile position within the grid.</param>
  /// <param name="y">The tile's Y tile position within the grid.</param>
  public bool this[int x, int y]
  {
    get => this.waterTiles[x, y].isWater;
    set => this.waterTiles[x, y] = new WaterTiles.WaterTileData(value, true);
  }

  public struct WaterTileData(bool is_water, bool is_visible)
  {
    public bool isWater = is_water;
    public bool isVisible = is_visible;
  }
}
