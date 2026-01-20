// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.MapPage
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley.BellsAndWhistles;
using StardewValley.Extensions;
using StardewValley.WorldMaps;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace StardewValley.Menus;

/// <summary>The in-game world map view.</summary>
public class MapPage : IClickableMenu
{
  /// <summary>The world map debug lines to draw, if any.</summary>
  public static MapPage.WorldMapDebugLineType EnableDebugLines;
  /// <summary>The map position containing the current player.</summary>
  public readonly MapAreaPositionWithContext? mapPosition;
  /// <summary>The map region containing the <see cref="F:StardewValley.Menus.MapPage.mapPosition" />.</summary>
  public readonly MapRegion mapRegion;
  /// <summary>The smaller sections of the map linked to one or more in-game locations. Each map area might be edited/swapped depending on the context, have its own tooltip(s), or have its own player marker positions.</summary>
  public readonly MapArea[] mapAreas;
  /// <summary>The translated scroll text to show at the bottom of the map, if any.</summary>
  public readonly string scrollText;
  /// <summary>The default component ID in <see cref="F:StardewValley.Menus.MapPage.points" /> to which to snap the controller cursor by default.</summary>
  public readonly int defaultComponentID;
  /// <summary>The pixel area on screen containing all the map areas being drawn.</summary>
  public Rectangle mapBounds;
  /// <summary>The tooltips to render, indexed by <see cref="P:StardewValley.WorldMaps.MapAreaTooltip.NamespacedId" />.</summary>
  public readonly Dictionary<string, ClickableComponent> points = new Dictionary<string, ClickableComponent>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
  /// <summary>The tooltip text being drawn.</summary>
  public string hoverText = "";

  public MapPage(int x, int y, int width, int height)
    : base(x, y, width, height)
  {
    WorldMapManager.ReloadData();
    Point normalizedPlayerTile = this.GetNormalizedPlayerTile(Game1.player);
    this.mapPosition = WorldMapManager.GetPositionData(Game1.player.currentLocation, normalizedPlayerTile) ?? WorldMapManager.GetPositionData((GameLocation) Game1.getFarm(), Point.Zero);
    ref readonly MapAreaPositionWithContext? local1 = ref this.mapPosition;
    this.mapRegion = (local1.HasValue ? local1.GetValueOrDefault().Data.Region : (MapRegion) null) ?? WorldMapManager.GetMapRegions().First<MapRegion>();
    this.mapAreas = this.mapRegion.GetAreas();
    ref readonly MapAreaPositionWithContext? local2 = ref this.mapPosition;
    this.scrollText = local2.HasValue ? local2.GetValueOrDefault().Data.GetScrollText(normalizedPlayerTile) : (string) null;
    this.mapBounds = this.mapRegion.GetMapPixelBounds();
    int num = this.defaultComponentID = 1000;
    foreach (MapArea mapArea in this.mapAreas)
    {
      foreach (MapAreaTooltip tooltip in mapArea.GetTooltips())
      {
        Rectangle bounds = tooltip.GetPixelArea();
        bounds = new Rectangle(this.mapBounds.X + bounds.X, this.mapBounds.Y + bounds.Y, bounds.Width, bounds.Height);
        ++num;
        ClickableComponent clickableComponent = new ClickableComponent(bounds, tooltip.NamespacedId)
        {
          myID = num,
          label = tooltip.Text
        };
        this.points[tooltip.NamespacedId] = clickableComponent;
        if (tooltip.NamespacedId == "Farm/Default")
          this.defaultComponentID = num;
      }
    }
    foreach (MapArea mapArea in this.mapAreas)
    {
      foreach (MapAreaTooltip tooltip in mapArea.GetTooltips())
      {
        ClickableComponent component;
        if (this.points.TryGetValue(tooltip.NamespacedId, out component))
        {
          this.SetNeighborId(component, "left", tooltip.Data.LeftNeighbor);
          this.SetNeighborId(component, "right", tooltip.Data.RightNeighbor);
          this.SetNeighborId(component, "up", tooltip.Data.UpNeighbor);
          this.SetNeighborId(component, "down", tooltip.Data.DownNeighbor);
        }
      }
    }
  }

  public override void populateClickableComponentList()
  {
    base.populateClickableComponentList();
    this.allClickableComponents.AddRange((IEnumerable<ClickableComponent>) this.points.Values);
  }

  /// <summary>Set a controller navigation ID for a tooltip component.</summary>
  /// <param name="component">The tooltip component whose neighbor ID to set.</param>
  /// <param name="direction">The direction to set.</param>
  /// <param name="neighborKeys">The tooltip neighbor keys to match. See remarks on <see cref="F:StardewValley.GameData.WorldMaps.WorldMapTooltipData.LeftNeighbor" /> for details on the format.</param>
  /// <returns>Returns whether the <paramref name="neighborKeys" /> matched an existing tooltip neighbor ID.</returns>
  public void SetNeighborId(ClickableComponent component, string direction, string neighborKeys)
  {
    if (string.IsNullOrWhiteSpace(neighborKeys))
      return;
    int id;
    bool foundIgnore;
    if (!this.TryGetNeighborId(neighborKeys, out id, out foundIgnore))
    {
      if (foundIgnore)
        return;
      Game1.log.Warn($"World map tooltip '{component.name}' has {direction} neighbor keys '{neighborKeys}' which don't match a tooltip namespaced ID or alias.");
    }
    else
    {
      switch (direction)
      {
        case "left":
          component.leftNeighborID = id;
          break;
        case "right":
          component.rightNeighborID = id;
          break;
        case "up":
          component.upNeighborID = id;
          break;
        case "down":
          component.downNeighborID = id;
          break;
        default:
          Game1.log.Warn($"Can't set neighbor ID for unknown direction '{direction}'.");
          break;
      }
    }
  }

  /// <summary>Get the controller navigation ID for a tooltip neighbor field value.</summary>
  /// <param name="keys">The tooltip neighbor keys to match. See remarks on <see cref="F:StardewValley.GameData.WorldMaps.WorldMapTooltipData.LeftNeighbor" /> for details on the format.</param>
  /// <param name="id">The matching controller navigation ID, if found.</param>
  /// <param name="foundIgnore">Whether the neighbor IDs contains <c>ignore</c>, which indicates it should be skipped silently if none match.</param>
  /// <param name="isAlias">Whether the <paramref name="keys" /> are from an alias in <see cref="F:StardewValley.GameData.WorldMaps.WorldMapRegionData.MapNeighborIdAliases" />.</param>
  /// <returns>Returns <c>true</c> if the neighbor ID was found, else <c>false</c>.</returns>
  public bool TryGetNeighborId(string keys, out int id, out bool foundIgnore, bool isAlias = false)
  {
    foundIgnore = false;
    if (!string.IsNullOrWhiteSpace(keys))
    {
      foreach (string str1 in keys.Split(',', StringSplitOptions.RemoveEmptyEntries))
      {
        string str2 = str1.Trim();
        if (str2.EqualsIgnoreCase("ignore"))
        {
          foundIgnore = true;
        }
        else
        {
          ClickableComponent clickableComponent;
          if (this.points.TryGetValue(str2, out clickableComponent))
          {
            id = clickableComponent.myID;
            return true;
          }
          string keys1;
          if (!isAlias && this.mapRegion.Data.MapNeighborIdAliases.TryGetValue(str2, out keys1))
          {
            bool foundIgnore1;
            if (this.TryGetNeighborId(keys1, out id, out foundIgnore1, true))
            {
              foundIgnore |= foundIgnore1;
              return true;
            }
            foundIgnore |= foundIgnore1;
          }
        }
      }
    }
    id = -1;
    return false;
  }

  public override void snapToDefaultClickableComponent()
  {
    this.currentlySnappedComponent = this.getComponentWithID(this.defaultComponentID);
    this.snapCursorToCurrentSnappedComponent();
  }

  /// <inheritdoc />
  public override void receiveLeftClick(int x, int y, bool playSound = true)
  {
    foreach (ClickableComponent clickableComponent in this.points.Values)
    {
      if (clickableComponent.containsPoint(x, y))
      {
        switch (clickableComponent.name)
        {
          case "Beach/LonelyStone":
            Game1.playSound("stoneCrack");
            return;
          case "Forest/SewerPipe":
            Game1.playSound("shadowpeep");
            return;
          default:
            return;
        }
      }
    }
    if (!(Game1.activeClickableMenu is GameMenu activeClickableMenu))
      return;
    activeClickableMenu.changeTab(activeClickableMenu.lastOpenedNonMapTab);
  }

  /// <inheritdoc />
  public override void performHoverAction(int x, int y)
  {
    this.hoverText = "";
    foreach (ClickableComponent clickableComponent in this.points.Values)
    {
      if (clickableComponent.containsPoint(x, y))
      {
        this.hoverText = clickableComponent.label;
        break;
      }
    }
  }

  /// <inheritdoc />
  public override void draw(SpriteBatch b)
  {
    this.drawMap(b);
    this.drawMiniPortraits(b);
    this.drawScroll(b);
    this.drawTooltip(b);
  }

  /// <inheritdoc />
  public override void receiveKeyPress(Keys key)
  {
    if (Game1.options.doesInputListContain(Game1.options.mapButton, key) && this.readyToClose())
      this.exitThisMenu();
    base.receiveKeyPress(key);
  }

  public virtual void drawMiniPortraits(SpriteBatch b, float alpha = 1f)
  {
    Dictionary<Vector2, int> dictionary = new Dictionary<Vector2, int>();
    foreach (Farmer onlineFarmer in Game1.getOnlineFarmers())
    {
      Point normalizedPlayerTile = this.GetNormalizedPlayerTile(onlineFarmer);
      MapAreaPositionWithContext? nullable = onlineFarmer.IsLocalPlayer ? this.mapPosition : WorldMapManager.GetPositionData(onlineFarmer.currentLocation, normalizedPlayerTile);
      if (nullable.HasValue && !(nullable.Value.Data.Region.Id != this.mapRegion.Id))
      {
        Vector2 vector2 = nullable.Value.GetMapPixelPosition();
        vector2 = new Vector2((float) ((double) vector2.X + (double) this.mapBounds.X - 32.0), (float) ((double) vector2.Y + (double) this.mapBounds.Y - 32.0));
        int num;
        dictionary.TryGetValue(vector2, out num);
        dictionary[vector2] = num + 1;
        if (num > 0)
          vector2 += new Vector2((float) (48 /*0x30*/ * (num % 2)), (float) (48 /*0x30*/ * (num / 2)));
        onlineFarmer.FarmerRenderer.drawMiniPortrat(b, vector2, 0.00011f, 4f, 2, onlineFarmer, alpha);
      }
    }
  }

  public virtual void drawScroll(SpriteBatch b)
  {
    if (this.scrollText == null)
      return;
    float y = (float) (this.yPositionOnScreen + this.height + 32 /*0x20*/ + 4);
    float num = y + 80f;
    if ((double) num > (double) Game1.uiViewport.Height)
      y -= num - (float) Game1.uiViewport.Height;
    SpriteText.drawStringWithScrollCenteredAt(b, this.scrollText, this.xPositionOnScreen + this.width / 2, (int) y);
  }

  public virtual void drawMap(SpriteBatch b, bool drawBorders = true, float alpha = 1f)
  {
    if (drawBorders)
      Game1.drawDialogueBox(this.mapBounds.X - 32 /*0x20*/, this.mapBounds.Y - 96 /*0x60*/, (this.mapBounds.Width + 16 /*0x10*/) * 4, (this.mapBounds.Height + 32 /*0x20*/) * 4, false, true);
    float layerDepth = 0.86f;
    MapAreaTexture baseTexture = this.mapRegion.GetBaseTexture();
    if (baseTexture != null)
    {
      Rectangle offsetMapPixelArea = baseTexture.GetOffsetMapPixelArea(this.mapBounds.X, this.mapBounds.Y);
      b.Draw(baseTexture.Texture, offsetMapPixelArea, new Rectangle?(baseTexture.SourceRect), Color.White * alpha, 0.0f, Vector2.Zero, SpriteEffects.None, layerDepth);
      layerDepth += 1f / 1000f;
    }
    foreach (MapArea mapArea in this.mapAreas)
    {
      foreach (MapAreaTexture texture in mapArea.GetTextures())
      {
        Rectangle offsetMapPixelArea = texture.GetOffsetMapPixelArea(this.mapBounds.X, this.mapBounds.Y);
        b.Draw(texture.Texture, offsetMapPixelArea, new Rectangle?(texture.SourceRect), Color.White * alpha, 0.0f, Vector2.Zero, SpriteEffects.None, layerDepth);
        layerDepth += 1f / 1000f;
      }
    }
    if (MapPage.EnableDebugLines == MapPage.WorldMapDebugLineType.None)
      return;
    foreach (MapArea mapArea in this.mapAreas)
    {
      if (MapPage.EnableDebugLines.HasFlag((Enum) MapPage.WorldMapDebugLineType.Tooltips))
      {
        foreach (MapAreaTooltip tooltip in mapArea.GetTooltips())
        {
          Rectangle pixelArea = tooltip.GetPixelArea();
          pixelArea = new Rectangle(this.mapBounds.X + pixelArea.X, this.mapBounds.Y + pixelArea.Y, pixelArea.Width, pixelArea.Height);
          Utility.DrawSquare(b, pixelArea, 2, new Color?(Color.Blue * alpha));
        }
      }
      if (MapPage.EnableDebugLines.HasFlag((Enum) MapPage.WorldMapDebugLineType.Areas))
      {
        Rectangle pixelArea = mapArea.Data.PixelArea;
        if (pixelArea.Width > 0 || pixelArea.Height > 0)
        {
          pixelArea = new Rectangle(this.mapBounds.X + pixelArea.X * 4, this.mapBounds.Y + pixelArea.Y * 4, pixelArea.Width * 4, pixelArea.Height * 4);
          Utility.DrawSquare(b, pixelArea, 4, new Color?(Color.Black * alpha));
        }
      }
      if (MapPage.EnableDebugLines.HasFlag((Enum) MapPage.WorldMapDebugLineType.Positions))
      {
        foreach (MapAreaPosition worldPosition in mapArea.GetWorldPositions())
        {
          Rectangle pixelArea = worldPosition.GetPixelArea();
          pixelArea = new Rectangle(this.mapBounds.X + pixelArea.X, this.mapBounds.Y + pixelArea.Y, pixelArea.Width, pixelArea.Height);
          Utility.DrawSquare(b, pixelArea, 2, new Color?(Color.Red * alpha));
        }
      }
    }
  }

  public virtual void drawTooltip(SpriteBatch b)
  {
    if (string.IsNullOrEmpty(this.hoverText))
      return;
    IClickableMenu.drawHoverText(b, this.hoverText, Game1.smallFont);
  }

  /// <summary>Get the tile coordinate for a player, with negative values snapped to zero.</summary>
  /// <param name="player">The player instance.</param>
  public Point GetNormalizedPlayerTile(Farmer player)
  {
    Point normalizedPlayerTile = player.TilePoint;
    if (normalizedPlayerTile.X < 0 || normalizedPlayerTile.Y < 0)
      normalizedPlayerTile = new Point(Math.Max(0, normalizedPlayerTile.X), Math.Max(0, normalizedPlayerTile.Y));
    return normalizedPlayerTile;
  }

  /// <summary>The world map debug lines to draw.</summary>
  [Flags]
  public enum WorldMapDebugLineType
  {
    /// <summary>Don't show debug lines on the map.</summary>
    None = 0,
    /// <summary>Highlight map areas.</summary>
    Areas = 1,
    /// <summary>Highlight map position rectangles.</summary>
    Positions = 2,
    /// <summary>Highlight tooltip rectangles.</summary>
    Tooltips = 4,
    /// <summary>Highlight all types.</summary>
    All = -1, // 0xFFFFFFFF
  }
}
