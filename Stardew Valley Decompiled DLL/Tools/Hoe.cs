// Decompiled with JetBrains decompiler
// Type: StardewValley.Tools.Hoe
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using StardewValley.Extensions;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;
using System.Collections.Generic;
using xTile.Dimensions;

#nullable disable
namespace StardewValley.Tools;

public class Hoe : Tool
{
  public Hoe()
    : base(nameof (Hoe), 0, 21, 47, false)
  {
  }

  /// <inheritdoc />
  protected override void MigrateLegacyItemId()
  {
    switch (this.UpgradeLevel)
    {
      case 0:
        this.ItemId = nameof (Hoe);
        break;
      case 1:
        this.ItemId = "CopperHoe";
        break;
      case 2:
        this.ItemId = "SteelHoe";
        break;
      case 3:
        this.ItemId = "GoldHoe";
        break;
      case 4:
        this.ItemId = "IridiumHoe";
        break;
      default:
        this.ItemId = nameof (Hoe);
        break;
    }
  }

  /// <inheritdoc />
  protected override Item GetOneNew() => (Item) new Hoe();

  public override void DoFunction(GameLocation location, int x, int y, int power, Farmer who)
  {
    Vector2 tileLocation = new Vector2((float) (x / 64 /*0x40*/), (float) (y / 64 /*0x40*/));
    base.DoFunction(location, x, y, power, who);
    if (MineShaft.IsGeneratedLevel(location))
      power = 1;
    if (!this.isEfficient.Value)
      who.Stamina -= (float) (2 * power) - (float) who.FarmingLevel * 0.1f;
    power = who.toolPower.Value;
    who.stopJittering();
    if (this.PlayUseSounds)
      location.playSound("woodyHit", new Vector2?(tileLocation));
    List<Vector2> vector2List = this.tilesAffected(tileLocation, power, who);
    foreach (Vector2 vector2 in vector2List)
    {
      TerrainFeature terrainFeature;
      if (location.terrainFeatures.TryGetValue(vector2, out terrainFeature))
      {
        if (terrainFeature.performToolAction((Tool) this, 0, vector2))
          location.terrainFeatures.Remove(vector2);
      }
      else
      {
        Object @object;
        if (location.objects.TryGetValue(vector2, out @object) && @object.performToolAction((Tool) this))
        {
          if (@object.Type == "Crafting" && @object.fragility.Value != 2)
            location.debris.Add(new Debris(@object.QualifiedItemId, who.GetToolLocation(), Utility.PointToVector2(who.StandingPixel)));
          @object.performRemoveAction();
          location.Objects.Remove(vector2);
        }
        if (location.doesTileHaveProperty((int) vector2.X, (int) vector2.Y, "Diggable", "Back") != null)
        {
          if (location is MineShaft && !location.IsTileOccupiedBy(vector2, useFarmerTile: true))
          {
            if (location.makeHoeDirt(vector2))
            {
              if (this.PlayUseSounds)
                location.playSound("hoeHit", new Vector2?(vector2));
              location.checkForBuriedItem((int) vector2.X, (int) vector2.Y, false, false, who);
              Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(12, new Vector2(tileLocation.X * 64f, tileLocation.Y * 64f), Color.White, flipped: Game1.random.NextBool(), animationInterval: 50f));
              if (vector2List.Count > 2)
                Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(6, new Vector2(vector2.X * 64f, vector2.Y * 64f), Color.White, flipped: Game1.random.NextBool(), animationInterval: Vector2.Distance(tileLocation, vector2) * 30f));
            }
          }
          else if (location.isTilePassable(new Location((int) vector2.X, (int) vector2.Y), Game1.viewport) && location.makeHoeDirt(vector2))
          {
            if (this.PlayUseSounds)
              location.playSound("hoeHit", new Vector2?(vector2));
            Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(12, new Vector2(vector2.X * 64f, vector2.Y * 64f), Color.White, flipped: Game1.random.NextBool(), animationInterval: 50f));
            if (vector2List.Count > 2)
              Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(6, new Vector2(vector2.X * 64f, vector2.Y * 64f), Color.White, flipped: Game1.random.NextBool(), animationInterval: Vector2.Distance(tileLocation, vector2) * 30f));
            location.checkForBuriedItem((int) vector2.X, (int) vector2.Y, false, false, who);
          }
          ++Game1.stats.DirtHoed;
        }
      }
    }
  }
}
