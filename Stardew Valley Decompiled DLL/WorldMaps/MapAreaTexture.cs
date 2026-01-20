// Decompiled with JetBrains decompiler
// Type: StardewValley.WorldMaps.MapAreaTexture
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#nullable disable
namespace StardewValley.WorldMaps;

public class MapAreaTexture
{
  /// <summary>The texture to draw when the area is applied to the map.</summary>
  public Texture2D Texture { get; }

  /// <summary>The pixel area within the <see cref="P:StardewValley.WorldMaps.MapAreaTexture.Texture" /> to draw.</summary>
  public Rectangle SourceRect { get; }

  /// <summary>The pixel area within the map area to draw the texture to, adjusted for <see cref="F:StardewValley.Game1.pixelZoom" />.</summary>
  public Rectangle MapPixelArea { get; }

  /// <summary>Construct an instance.</summary>
  /// <param name="texture">The texture to draw when the area is applied to the map.</param>
  /// <param name="sourceRect">The pixel area within the <paramref name="texture" /> to draw.</param>
  /// <param name="mapPixelArea">The pixel area within the map area to draw the texture to, adjusted for <see cref="F:StardewValley.Game1.pixelZoom" />.</param>
  public MapAreaTexture(Texture2D texture, Rectangle sourceRect, Rectangle mapPixelArea)
  {
    this.Texture = texture;
    this.SourceRect = sourceRect;
    this.MapPixelArea = mapPixelArea;
  }

  /// <summary>Get the <see cref="P:StardewValley.WorldMaps.MapAreaTexture.MapPixelArea" /> offset for the given map position.</summary>
  /// <param name="x">The X pixel position of the map being drawn.</param>
  /// <param name="y">The Y pixel position of the map being drawn.</param>
  public Rectangle GetOffsetMapPixelArea(int x, int y)
  {
    return new Rectangle(this.MapPixelArea.X + x, this.MapPixelArea.Y + y, this.MapPixelArea.Width, this.MapPixelArea.Height);
  }
}
