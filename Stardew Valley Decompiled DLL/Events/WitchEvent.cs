// Decompiled with JetBrains decompiler
// Type: StardewValley.Events.WitchEvent
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Buildings;
using StardewValley.Extensions;
using StardewValley.Monsters;
using StardewValley.TerrainFeatures;
using System;

#nullable disable
namespace StardewValley.Events;

public class WitchEvent : BaseFarmEvent
{
  public string lightSourceId;
  private Vector2 witchPosition;
  private Building targetBuilding;
  private Farm f;
  private Random r;
  private int witchFrame;
  private int witchAnimationTimer;
  private int animationLoopsDone;
  private int timerSinceFade;
  private bool animateLeft;
  private bool terminate;
  public bool goldenWitch;

  /// <inheritdoc />
  public override bool setUp()
  {
    this.lightSourceId = this.GenerateLightSourceId();
    this.f = Game1.getFarm();
    this.r = Utility.CreateRandom((double) Game1.uniqueIDForThisGame, (double) Game1.stats.DaysPlayed);
    foreach (Building building in this.f.buildings)
    {
      if (building.buildingType.Value == "Big Coop" || building.buildingType.Value == "Deluxe Coop")
      {
        AnimalHouse indoors = (AnimalHouse) building.GetIndoors();
        if (!indoors.isFull() && indoors.objects.Length < 50 && this.r.NextDouble() < 0.8)
        {
          this.targetBuilding = building;
          if (Game1.MasterPlayer.mailReceived.Contains("Farm_Eternal") && this.r.NextDouble() < 0.6)
            this.goldenWitch = true;
        }
      }
    }
    if (this.targetBuilding == null)
    {
      foreach (Building building in this.f.buildings)
      {
        if (building.buildingType.Value == "Slime Hutch")
        {
          GameLocation indoors = building.GetIndoors();
          if (indoors.characters.Count > 0 && this.r.NextBool() && indoors.numberOfObjectsOfType("83", true) == 0)
            this.targetBuilding = building;
        }
      }
    }
    if (this.targetBuilding == null)
      return true;
    Game1.currentLightSources.Add(new LightSource(this.lightSourceId, 4, this.witchPosition, 2f, Color.Black));
    Game1.currentLocation = (GameLocation) this.f;
    this.f.resetForPlayerEntry();
    Game1.fadeClear();
    Game1.nonWarpFade = true;
    Game1.timeOfDay = 2400;
    Game1.ambientLight = new Color(200, 190, 40);
    Game1.displayHUD = false;
    Game1.freezeControls = true;
    Game1.viewportFreeze = true;
    Game1.displayFarmer = false;
    Game1.viewport.X = Math.Max(0, Math.Min(this.f.map.DisplayWidth - Game1.viewport.Width, this.targetBuilding.tileX.Value * 64 /*0x40*/ - Game1.viewport.Width / 2));
    Game1.viewport.Y = Math.Max(0, Math.Min(this.f.map.DisplayHeight - Game1.viewport.Height, (this.targetBuilding.tileY.Value - 3) * 64 /*0x40*/ - Game1.viewport.Height / 2));
    this.witchPosition = new Vector2((float) (Game1.viewport.X + Game1.viewport.Width + 128 /*0x80*/), (float) (this.targetBuilding.tileY.Value * 64 /*0x40*/ - 64 /*0x40*/));
    Game1.changeMusicTrack("nightTime");
    DelayedAction.playSoundAfterDelay(this.goldenWitch ? "yoba" : "cacklingWitch", 3200);
    return false;
  }

  /// <inheritdoc />
  public override bool tickUpdate(GameTime time)
  {
    if (this.terminate)
      return true;
    Game1.UpdateGameClock(time);
    this.f.UpdateWhenCurrentLocation(time);
    this.f.updateEvenIfFarmerIsntHere(time, false);
    Game1.UpdateOther(time);
    Utility.repositionLightSource(this.lightSourceId, this.witchPosition + new Vector2(32f, 32f));
    TimeSpan timeSpan;
    if (this.animationLoopsDone < 1)
    {
      int timerSinceFade = this.timerSinceFade;
      timeSpan = time.ElapsedGameTime;
      int milliseconds = timeSpan.Milliseconds;
      this.timerSinceFade = timerSinceFade + milliseconds;
    }
    if ((double) this.witchPosition.X > (double) (this.targetBuilding.tileX.Value * 64 /*0x40*/ + 96 /*0x60*/))
    {
      if (this.timerSinceFade < 2000)
        return false;
      ref float local1 = ref this.witchPosition.X;
      double num1 = (double) local1;
      timeSpan = time.ElapsedGameTime;
      double num2 = (double) timeSpan.Milliseconds * 0.40000000596046448;
      local1 = (float) (num1 - num2);
      ref float local2 = ref this.witchPosition.Y;
      double num3 = (double) local2;
      timeSpan = time.TotalGameTime;
      double num4 = Math.Cos((double) timeSpan.Milliseconds * Math.PI / 512.0) * 1.0;
      local2 = (float) (num3 + num4);
    }
    else if (this.animationLoopsDone < 4)
    {
      ref float local = ref this.witchPosition.Y;
      double num5 = (double) local;
      timeSpan = time.TotalGameTime;
      double num6 = Math.Cos((double) timeSpan.Milliseconds * Math.PI / 512.0) * 1.0;
      local = (float) (num5 + num6);
      int witchAnimationTimer = this.witchAnimationTimer;
      timeSpan = time.ElapsedGameTime;
      int milliseconds = timeSpan.Milliseconds;
      this.witchAnimationTimer = witchAnimationTimer + milliseconds;
      if (this.witchAnimationTimer > 2000)
      {
        this.witchAnimationTimer = 0;
        if (!this.animateLeft)
        {
          ++this.witchFrame;
          if (this.witchFrame == 1)
          {
            this.animateLeft = true;
            for (int index = 0; index < 75; ++index)
              this.f.temporarySprites.Add(new TemporaryAnimatedSprite(10, this.witchPosition + new Vector2(8f, 80f), this.goldenWitch ? (this.r.NextBool() ? Color.Gold : new Color((int) byte.MaxValue, 150, 0)) : (this.r.NextBool() ? Color.Lime : Color.DarkViolet))
              {
                motion = new Vector2((float) this.r.Next(-100, 100) / 100f, 1.5f),
                alphaFade = 0.015f,
                delayBeforeAnimationStart = index * 30,
                layerDepth = 1f
              });
            Game1.playSound(this.goldenWitch ? "discoverMineral" : "debuffSpell");
          }
        }
        else
        {
          --this.witchFrame;
          this.animationLoopsDone = 4;
          DelayedAction.playSoundAfterDelay(this.goldenWitch ? "yoba" : "cacklingWitch", 2500);
        }
      }
    }
    else
    {
      int witchAnimationTimer = this.witchAnimationTimer;
      timeSpan = time.ElapsedGameTime;
      int milliseconds1 = timeSpan.Milliseconds;
      this.witchAnimationTimer = witchAnimationTimer + milliseconds1;
      this.witchFrame = 0;
      if (this.witchAnimationTimer > 1000 && (double) this.witchPosition.X > -999999.0)
      {
        ref float local3 = ref this.witchPosition.Y;
        double num7 = (double) local3;
        timeSpan = time.TotalGameTime;
        double num8 = Math.Cos((double) timeSpan.Milliseconds * Math.PI / 256.0) * 2.0;
        local3 = (float) (num7 + num8);
        ref float local4 = ref this.witchPosition.X;
        double num9 = (double) local4;
        timeSpan = time.ElapsedGameTime;
        double num10 = (double) timeSpan.Milliseconds * 0.40000000596046448;
        local4 = (float) (num9 - num10);
      }
      if ((double) this.witchPosition.X < (double) (Game1.viewport.X - 128 /*0x80*/) || float.IsNaN(this.witchPosition.X))
      {
        if (!Game1.fadeToBlack && (double) this.witchPosition.X != -999999.0)
        {
          Game1.globalFadeToBlack(new Game1.afterFadeFunction(this.afterLastFade));
          Game1.changeMusicTrack("none");
          this.timerSinceFade = 0;
          this.witchPosition.X = -999999f;
        }
        int timerSinceFade = this.timerSinceFade;
        timeSpan = time.ElapsedGameTime;
        int milliseconds2 = timeSpan.Milliseconds;
        this.timerSinceFade = timerSinceFade + milliseconds2;
      }
    }
    return false;
  }

  public void afterLastFade()
  {
    this.terminate = true;
    Game1.globalFadeToClear();
  }

  /// <inheritdoc />
  public override void draw(SpriteBatch b)
  {
    if (this.goldenWitch)
      b.Draw(Game1.mouseCursors2, Game1.GlobalToLocal(Game1.viewport, this.witchPosition), new Rectangle?(new Rectangle(215, 262 + this.witchFrame * 29, 34, 29)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.9999999f);
    else
      b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, this.witchPosition), new Rectangle?(new Rectangle(277, 1886 + this.witchFrame * 29, 34, 29)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.9999999f);
  }

  /// <inheritdoc />
  public override void makeChangesToLocation()
  {
    if (!Game1.IsMasterGame)
      return;
    GameLocation indoors = this.targetBuilding.GetIndoors();
    if (this.targetBuilding.buildingType.Value == "Slime Hutch")
    {
      foreach (NPC character in indoors.characters)
      {
        if (character is GreenSlime greenSlime)
          greenSlime.color.Value = new Color(40 + this.r.Next(10), 40 + this.r.Next(10), 40 + this.r.Next(10));
      }
    }
    else
    {
      for (int index = 0; index < 200; ++index)
      {
        Vector2 vector2 = new Vector2((float) this.r.Next(2, indoors.Map.Layers[0].LayerWidth - 2), (float) this.r.Next(2, indoors.Map.Layers[0].LayerHeight - 2));
        TerrainFeature terrainFeature;
        if ((indoors.CanItemBePlacedHere(vector2) || indoors.terrainFeatures.TryGetValue(vector2, out terrainFeature) && terrainFeature is Flooring) && !indoors.objects.ContainsKey(vector2))
        {
          StardewValley.Object @object = ItemRegistry.Create<StardewValley.Object>(this.goldenWitch ? "(O)928" : "(O)305");
          @object.CanBeSetDown = false;
          @object.IsSpawnedObject = true;
          indoors.objects.Add(vector2, @object);
          break;
        }
      }
    }
  }
}
