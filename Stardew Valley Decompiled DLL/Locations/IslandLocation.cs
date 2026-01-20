// Decompiled with JetBrains decompiler
// Type: StardewValley.Locations.IslandLocation
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.BellsAndWhistles;
using StardewValley.Extensions;
using StardewValley.GameData.Locations;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using xTile;
using xTile.Dimensions;

#nullable disable
namespace StardewValley.Locations;

public class IslandLocation : GameLocation
{
  public const int TOTAL_WALNUTS = 130;
  [XmlIgnore]
  public List<ParrotPlatform> parrotPlatforms = new List<ParrotPlatform>();
  [XmlIgnore]
  public NetList<ParrotUpgradePerch, NetRef<ParrotUpgradePerch>> parrotUpgradePerches = new NetList<ParrotUpgradePerch, NetRef<ParrotUpgradePerch>>();
  [XmlIgnore]
  public NetList<Point, NetPoint> buriedNutPoints = new NetList<Point, NetPoint>();
  [XmlElement("locationGemBird")]
  public NetRef<IslandGemBird> locationGemBird = new NetRef<IslandGemBird>();
  [XmlIgnore]
  protected Texture2D _dayParallaxTexture;
  [XmlIgnore]
  protected Texture2D _nightParallaxTexture;
  [XmlIgnore]
  protected TemporaryAnimatedSpriteList underwaterSprites = new TemporaryAnimatedSpriteList();

  public IslandLocation()
  {
  }

  public void ApplyUnsafeMapOverride(
    string override_map,
    Microsoft.Xna.Framework.Rectangle? source_rect,
    Microsoft.Xna.Framework.Rectangle dest_rect)
  {
    this.ApplyMapOverride(override_map, source_rect, new Microsoft.Xna.Framework.Rectangle?(dest_rect));
    Microsoft.Xna.Framework.Rectangle rect = new Microsoft.Xna.Framework.Rectangle(dest_rect.X * 64 /*0x40*/, dest_rect.Y * 64 /*0x40*/, dest_rect.Width * 64 /*0x40*/, dest_rect.Height * 64 /*0x40*/);
    if (this != Game1.player.currentLocation)
      return;
    Microsoft.Xna.Framework.Rectangle boundingBox = Game1.player.GetBoundingBox();
    if (!rect.Intersects(boundingBox) || !this.isCollidingPosition(boundingBox, Game1.viewport, true, 0, false, (Character) Game1.player))
      return;
    Game1.player.TemporaryPassableTiles.Add(rect);
  }

  protected override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.parrotUpgradePerches, "parrotUpgradePerches").AddField((INetSerializable) this.buriedNutPoints, "buriedNutPoints").AddField((INetSerializable) this.locationGemBird, "locationGemBird");
  }

  public override string doesTileHaveProperty(
    int xTile,
    int yTile,
    string propertyName,
    string layerName,
    bool ignoreTileSheetProperties = false)
  {
    return layerName == "Back" && propertyName == "Diggable" && this.IsBuriedNutLocation(new Point(xTile, yTile)) ? "T" : base.doesTileHaveProperty(xTile, yTile, propertyName, layerName, ignoreTileSheetProperties);
  }

  public virtual void SetBuriedNutLocations()
  {
  }

  public virtual List<Vector2> GetAdditionalWalnutBushes() => (List<Vector2>) null;

  public IslandLocation(string map, string name)
    : base(map, name)
  {
    this.SetBuriedNutLocations();
    foreach (LargeTerrainFeature largeTerrainFeature in this.largeTerrainFeatures)
    {
      if (largeTerrainFeature is Bush bush)
        bush.setUpSourceRect();
    }
  }

  /// <inheritdoc />
  public override bool SeedsIgnoreSeasonsHere() => true;

  /// <inheritdoc />
  public override bool catchOceanCrabPotFishFromThisSpot(int x, int y)
  {
    return !this.TryGetFishAreaForTile(new Vector2((float) x, (float) y), out string _, out FishAreaData _);
  }

  public override bool answerDialogue(Response answer)
  {
    foreach (ParrotPlatform parrotPlatform in this.parrotPlatforms)
    {
      if (parrotPlatform.AnswerQuestion(answer))
        return true;
    }
    foreach (ParrotUpgradePerch parrotUpgradePerch in this.parrotUpgradePerches)
    {
      if (parrotUpgradePerch.AnswerQuestion(answer))
        return true;
    }
    return base.answerDialogue(answer);
  }

  public override void cleanupBeforePlayerExit()
  {
    foreach (ParrotPlatform parrotPlatform in this.parrotPlatforms)
      parrotPlatform.Cleanup();
    foreach (ParrotUpgradePerch parrotUpgradePerch in this.parrotUpgradePerches)
      parrotUpgradePerch.Cleanup();
    this._dayParallaxTexture = (Texture2D) null;
    this._nightParallaxTexture = (Texture2D) null;
    this.underwaterSprites.Clear();
    base.cleanupBeforePlayerExit();
  }

  public override bool isCollidingPosition(
    Microsoft.Xna.Framework.Rectangle position,
    xTile.Dimensions.Rectangle viewport,
    bool isFarmer,
    int damagesFarmer,
    bool glider,
    Character character,
    bool pathfinding,
    bool projectile = false,
    bool ignoreCharacterRequirement = false,
    bool skipCollisionEffects = false)
  {
    foreach (ParrotPlatform parrotPlatform in this.parrotPlatforms)
    {
      if (parrotPlatform.CheckCollisions(position))
        return true;
    }
    return base.isCollidingPosition(position, viewport, isFarmer, damagesFarmer, glider, character, pathfinding, projectile, ignoreCharacterRequirement);
  }

  protected void addMoonlightJellies(int numTries, Random r, Microsoft.Xna.Framework.Rectangle exclusionRect)
  {
    for (int index = 0; index < numTries; ++index)
    {
      Point point1 = new Point(r.Next(this.Map.Layers[0].LayerWidth), r.Next(this.Map.Layers[0].LayerHeight));
      if (this.isOpenWater(point1.X, point1.Y) && !exclusionRect.Contains(point1) && FishingRod.distanceToLand(point1.X, point1.Y, (GameLocation) this) >= 2)
      {
        bool flag = false;
        foreach (TemporaryAnimatedSprite underwaterSprite in this.underwaterSprites)
        {
          Point point2 = new Point((int) underwaterSprite.position.X / 64 /*0x40*/, (int) underwaterSprite.position.Y / 64 /*0x40*/);
          if ((double) Utility.distance((float) point1.X, (float) point2.X, (float) point1.Y, (float) point2.Y) <= 2.0)
          {
            flag = true;
            break;
          }
        }
        if (!flag)
        {
          TemporaryAnimatedSpriteList underwaterSprites = this.underwaterSprites;
          TemporaryAnimatedSprite temporaryAnimatedSprite = new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(r.NextDouble() < 0.2 ? 304 : 256 /*0x0100*/, r.NextDouble() < 0.01 ? 32 /*0x20*/ : 16 /*0x10*/, 16 /*0x10*/, 16 /*0x10*/), 250f, 3, 9999, new Vector2((float) point1.X, (float) point1.Y) * 64f, false, false, 0.1f, 0.0f, Color.White * 0.66f, 4f, 0.0f, 0.0f, 0.0f);
          temporaryAnimatedSprite.yPeriodic = Game1.random.NextDouble() < 0.76;
          temporaryAnimatedSprite.yPeriodicRange = 12f;
          temporaryAnimatedSprite.yPeriodicLoopTime = (float) Game1.random.Next(5500, 8000);
          temporaryAnimatedSprite.xPeriodic = Game1.random.NextDouble() < 0.76;
          temporaryAnimatedSprite.xPeriodicLoopTime = (float) Game1.random.Next(5500, 8000);
          temporaryAnimatedSprite.xPeriodicRange = 16f;
          temporaryAnimatedSprite.lightId = $"{this.NameOrUniqueName}_MoonlightJelly_{point1.X}_{point1.Y}";
          temporaryAnimatedSprite.lightcolor = Color.Black;
          temporaryAnimatedSprite.lightRadius = 1f;
          temporaryAnimatedSprite.pingPong = true;
          underwaterSprites.Add(temporaryAnimatedSprite);
        }
      }
    }
  }

  public override void UpdateWhenCurrentLocation(GameTime time)
  {
    if (Game1.currentLocation == this)
    {
      foreach (ParrotPlatform parrotPlatform in this.parrotPlatforms)
        parrotPlatform.Update(time);
    }
    foreach (ParrotUpgradePerch parrotUpgradePerch in this.parrotUpgradePerches)
      parrotUpgradePerch.Update(time);
    this.underwaterSprites.RemoveWhere<TemporaryAnimatedSprite>((Predicate<TemporaryAnimatedSprite>) (sprite => sprite.update(time)));
    base.UpdateWhenCurrentLocation(time);
  }

  public override void tryToAddCritters(bool onlyIfOnScreen = false)
  {
    if (Game1.random.NextDouble() < 0.20000000298023224 && !this.IsRainingHere() && !Game1.isDarkOut((GameLocation) this))
    {
      Vector2 vector2 = Game1.random.NextDouble() >= 0.75 ? new Vector2((float) (Game1.viewport.X + Game1.viewport.Width + 64 /*0x40*/), Utility.RandomFloat(0.0f, (float) Game1.viewport.Height)) : new Vector2((float) Game1.viewport.X + Utility.RandomFloat(0.0f, (float) Game1.viewport.Width), (float) (Game1.viewport.Y - 64 /*0x40*/));
      int num = 1;
      if (Game1.random.NextBool())
        ++num;
      if (Game1.random.NextBool())
        ++num;
      for (int index = 0; index < num; ++index)
        this.addCritter((Critter) new OverheadParrot(vector2 + new Vector2((float) (index * 64 /*0x40*/), (float) (-index * 64 /*0x40*/))));
    }
    if (this.IsRainingHere())
      return;
    this.addButterflies(Math.Max(0.1, Math.Min(0.25, (double) (this.map.Layers[0].LayerWidth * this.map.Layers[0].LayerHeight) / 15000.0)), onlyIfOnScreen);
  }

  public override void DayUpdate(int dayOfMonth)
  {
    base.DayUpdate(dayOfMonth);
    this.locationGemBird.Value = (IslandGemBird) null;
  }

  public override void updateEvenIfFarmerIsntHere(GameTime time, bool ignoreWasUpdatedFlush = false)
  {
    base.updateEvenIfFarmerIsntHere(time, ignoreWasUpdatedFlush);
    foreach (ParrotUpgradePerch parrotUpgradePerch in this.parrotUpgradePerches)
      parrotUpgradePerch.UpdateEvenIfFarmerIsntHere(time);
    if (this.locationGemBird.Value == null || !this.locationGemBird.Value.Update(time, (GameLocation) this) || !Game1.IsMasterGame)
      return;
    this.locationGemBird.Value = (IslandGemBird) null;
  }

  public override void TransferDataFromSavedLocation(GameLocation l)
  {
    base.TransferDataFromSavedLocation(l);
    foreach (ParrotUpgradePerch parrotUpgradePerch in this.parrotUpgradePerches)
      parrotUpgradePerch.UpdateCompletionStatus();
    if (!(l is IslandLocation islandLocation))
      return;
    this.locationGemBird.Value = islandLocation.locationGemBird.Value;
  }

  public void AddAdditionalWalnutBushes()
  {
    List<Vector2> additionalWalnutBushes = this.GetAdditionalWalnutBushes();
    if (additionalWalnutBushes == null)
      return;
    foreach (Vector2 vector2 in additionalWalnutBushes)
    {
      if (!(this.getLargeTerrainFeatureAt((int) vector2.X, (int) vector2.Y) is Bush terrainFeatureAt) || terrainFeatureAt.size.Value != 4)
        this.largeTerrainFeatures.Add((LargeTerrainFeature) new Bush(new Vector2((float) (int) vector2.X, (float) (int) vector2.Y), 4, (GameLocation) this));
    }
  }

  public override bool isActionableTile(int xTile, int yTile, Farmer who)
  {
    foreach (ParrotUpgradePerch parrotUpgradePerch in this.parrotUpgradePerches)
    {
      if (parrotUpgradePerch.IsAtTile(xTile, yTile) && parrotUpgradePerch.IsAvailable(true) && parrotUpgradePerch.parrotPresent)
        return true;
    }
    return base.isActionableTile(xTile, yTile, who);
  }

  public override string checkForBuriedItem(
    int xLocation,
    int yLocation,
    bool explosion,
    bool detectOnly,
    Farmer who)
  {
    if (!this.IsBuriedNutLocation(new Point(xLocation, yLocation)))
      return base.checkForBuriedItem(xLocation, yLocation, explosion, detectOnly, who);
    Game1.player.team.MarkCollectedNut($"Buried_{this.Name}_{xLocation.ToString()}_{yLocation.ToString()}");
    Game1.multiplayer.broadcastNutDig((GameLocation) this, new Point(xLocation, yLocation));
    return "";
  }

  public override void digUpArtifactSpot(int xLocation, int yLocation, Farmer who)
  {
    Random daySaveRandom = Utility.CreateDaySaveRandom((double) (xLocation * 2000), (double) yLocation);
    string itemId = (string) null;
    int num = 1;
    if (Game1.netWorldState.Value.GoldenCoconutCracked && daySaveRandom.NextDouble() < 0.1)
      itemId = "(O)791";
    else if (daySaveRandom.NextDouble() < 0.33)
    {
      itemId = "(O)831";
      num = daySaveRandom.Next(2, 5);
    }
    else if (daySaveRandom.NextDouble() < 0.15)
    {
      itemId = "(O)275";
      num = daySaveRandom.Next(1, 3);
    }
    if (itemId != null)
    {
      for (int index = 0; index < num; ++index)
        Game1.createItemDebris(ItemRegistry.Create(itemId), new Vector2((float) xLocation, (float) yLocation) * 64f, -1, (GameLocation) this);
    }
    base.digUpArtifactSpot(xLocation, yLocation, who);
  }

  public virtual bool IsBuriedNutLocation(Point point)
  {
    foreach (Point buriedNutPoint in this.buriedNutPoints)
    {
      if (buriedNutPoint == point)
        return true;
    }
    return false;
  }

  public override bool checkAction(Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who)
  {
    foreach (ParrotUpgradePerch parrotUpgradePerch in this.parrotUpgradePerches)
    {
      if (parrotUpgradePerch.CheckAction(tileLocation, who))
        return true;
    }
    return base.checkAction(tileLocation, viewport, who);
  }

  public override Item getFish(
    float millisecondsAfterNibble,
    string bait,
    int waterDepth,
    Farmer who,
    double baitPotency,
    Vector2 bobberTile,
    string locationName = null)
  {
    if (Utility.CreateRandom((double) Game1.stats.DaysPlayed, (double) Game1.stats.TimesFished, (double) Game1.uniqueIDForThisGame).NextDouble() < 0.15)
    {
      int num;
      if (!Game1.player.team.limitedNutDrops.TryGetValue("IslandFishing", out num))
        num = 0;
      if (num < 5)
      {
        if (!Game1.IsMultiplayer)
        {
          Game1.player.team.limitedNutDrops["IslandFishing"] = num + 1;
          return ItemRegistry.Create("(O)73");
        }
        Game1.player.team.RequestLimitedNutDrops("IslandFishing", (GameLocation) this, (int) bobberTile.X * 64 /*0x40*/, (int) bobberTile.Y * 64 /*0x40*/, 5);
        return (Item) null;
      }
    }
    return base.getFish(millisecondsAfterNibble, bait, waterDepth, who, baitPotency, bobberTile, locationName);
  }

  public override void draw(SpriteBatch b)
  {
    base.draw(b);
    foreach (ParrotPlatform parrotPlatform in this.parrotPlatforms)
      parrotPlatform.Draw(b);
    foreach (ParrotUpgradePerch parrotUpgradePerch in this.parrotUpgradePerches)
      parrotUpgradePerch.Draw(b);
    this.locationGemBird.Value?.Draw(b);
  }

  public override void drawAboveAlwaysFrontLayer(SpriteBatch b)
  {
    base.drawAboveAlwaysFrontLayer(b);
    foreach (ParrotUpgradePerch parrotUpgradePerch in this.parrotUpgradePerches)
      parrotUpgradePerch.DrawAboveAlwaysFrontLayer(b);
  }

  public override bool IsLocationSpecificOccupantOnTile(Vector2 tileLocation)
  {
    foreach (ParrotPlatform parrotPlatform in this.parrotPlatforms)
    {
      if (parrotPlatform.OccupiesTile(tileLocation))
        return true;
    }
    return base.IsLocationSpecificOccupantOnTile(tileLocation);
  }

  protected override void resetLocalState()
  {
    this.parrotPlatforms.Clear();
    this.parrotPlatforms = ParrotPlatform.CreateParrotPlatformsForArea((GameLocation) this);
    foreach (ParrotUpgradePerch parrotUpgradePerch in this.parrotUpgradePerches)
      parrotUpgradePerch.ResetForPlayerEntry();
    base.resetLocalState();
  }

  /// <inheritdoc />
  public override void seasonUpdate(bool onLoad = false)
  {
  }

  public override void updateSeasonalTileSheets(Map map = null)
  {
  }

  public override void drawWater(SpriteBatch b)
  {
    foreach (TemporaryAnimatedSprite underwaterSprite in this.underwaterSprites)
      underwaterSprite.draw(b);
    base.drawWater(b);
  }

  public virtual void DrawParallaxHorizon(SpriteBatch b, bool horizontal_parallax = true)
  {
    float num1 = 4f;
    if (this._dayParallaxTexture == null || this._dayParallaxTexture.IsDisposed)
      this._dayParallaxTexture = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\Cloudy_Ocean_BG");
    if (this._nightParallaxTexture == null || this._nightParallaxTexture.IsDisposed)
      this._nightParallaxTexture = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\Cloudy_Ocean_BG_Night");
    float num2 = (float) this._dayParallaxTexture.Width * num1 - (float) this.map.DisplayWidth;
    float t = 0.0f;
    int num3 = -640;
    int y = (int) ((double) Game1.viewport.Y * 0.20000000298023224 + (double) num3);
    if (horizontal_parallax)
    {
      if (this.map.DisplayWidth - Game1.viewport.Width < 0)
        t = 0.5f;
      else if (this.map.DisplayWidth - Game1.viewport.Width > 0)
        t = (float) Game1.viewport.X / (float) (this.map.DisplayWidth - Game1.viewport.Width);
    }
    else
      t = 0.5f;
    if (Game1.game1.takingMapScreenshot)
    {
      y = num3;
      t = 0.5f;
    }
    float num4 = 0.25f;
    float num5 = Utility.Lerp(0.5f + num4, 0.5f - num4, t);
    float num6 = Utility.Clamp((float) Utility.ConvertTimeToMinutes(Game1.timeOfDay + (int) ((double) Game1.gameTimeInterval / (double) Game1.realMilliSecondsPerGameMinute % 10.0) - Game1.getStartingToGetDarkTime((GameLocation) this)) / (float) Utility.ConvertTimeToMinutes(Game1.getTrulyDarkTime((GameLocation) this) - Game1.getStartingToGetDarkTime((GameLocation) this)), 0.0f, 1f);
    b.Draw(Game1.staminaRect, Game1.GlobalToLocal(Game1.viewport, new Microsoft.Xna.Framework.Rectangle(0, 0, this.map.DisplayWidth, this.map.DisplayHeight)), new Color(1, 122, 217, (int) byte.MaxValue));
    b.Draw(Game1.staminaRect, Game1.GlobalToLocal(Game1.viewport, new Microsoft.Xna.Framework.Rectangle(0, 0, this.map.DisplayWidth, this.map.DisplayHeight)), new Color(0, 7, 63 /*0x3F*/, (int) byte.MaxValue) * num6);
    Microsoft.Xna.Framework.Rectangle globalPosition = new Microsoft.Xna.Framework.Rectangle((int) (-(double) num2 * (double) num5), y, (int) ((double) this._dayParallaxTexture.Width * (double) num1), (int) ((double) this._dayParallaxTexture.Height * (double) num1));
    Microsoft.Xna.Framework.Rectangle rectangle = new Microsoft.Xna.Framework.Rectangle(0, 0, this._dayParallaxTexture.Width, this._dayParallaxTexture.Height);
    int num7 = 0;
    if (globalPosition.X < num7)
    {
      int num8 = num7 - globalPosition.X;
      globalPosition.X += num8;
      globalPosition.Width -= num8;
      rectangle.X += (int) ((double) num8 / (double) num1);
      rectangle.Width -= (int) ((double) num8 / (double) num1);
    }
    int displayWidth = this.map.DisplayWidth;
    if (globalPosition.X + globalPosition.Width > displayWidth)
    {
      int num9 = globalPosition.X + globalPosition.Width - displayWidth;
      globalPosition.Width -= num9;
      rectangle.Width -= (int) ((double) num9 / (double) num1);
    }
    if (rectangle.Width <= 0 || globalPosition.Width <= 0)
      return;
    b.Draw(this._dayParallaxTexture, Game1.GlobalToLocal(Game1.viewport, globalPosition), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 0.0f);
    b.Draw(this._nightParallaxTexture, Game1.GlobalToLocal(Game1.viewport, globalPosition), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.White * num6, 0.0f, Vector2.Zero, SpriteEffects.None, 0.0f);
  }

  /// <summary>Get whether the moonlight jellies are out right now.</summary>
  public bool AreMoonlightJelliesOut()
  {
    if (!Game1.IsWinter)
      return false;
    if (!this.IsOutdoors)
      return true;
    return !this.IsRainingHere() && Game1.isDarkOut((GameLocation) this);
  }
}
