// Decompiled with JetBrains decompiler
// Type: StardewValley.Events.FairyEvent
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Extensions;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace StardewValley.Events;

public class FairyEvent : BaseFarmEvent
{
  public string lightSourceId;
  private Vector2 fairyPosition;
  private Vector2 targetCrop;
  private Farm f;
  private int fairyFrame;
  private int fairyAnimationTimer;
  private int animationLoopsDone;
  private int timerSinceFade;
  private bool animateLeft;
  private bool terminate;

  /// <inheritdoc />
  public override bool setUp()
  {
    this.lightSourceId = this.GenerateLightSourceId();
    this.f = Game1.getFarm();
    if (this.f.IsRainingHere())
      return true;
    this.targetCrop = this.ChooseCrop();
    if (this.targetCrop == Vector2.Zero)
      return true;
    Game1.currentLocation.cleanupBeforePlayerExit();
    Game1.currentLightSources.Add(new LightSource(this.lightSourceId, 4, this.fairyPosition, 1f, Color.Black));
    Game1.currentLocation = (GameLocation) this.f;
    this.f.resetForPlayerEntry();
    Game1.fadeClear();
    Game1.nonWarpFade = true;
    Game1.timeOfDay = 2400;
    Game1.displayHUD = false;
    Game1.freezeControls = true;
    Game1.viewportFreeze = true;
    Game1.displayFarmer = false;
    Game1.viewport.X = Math.Max(0, Math.Min(this.f.map.DisplayWidth - Game1.viewport.Width, (int) this.targetCrop.X * 64 /*0x40*/ - Game1.viewport.Width / 2));
    Game1.viewport.Y = Math.Max(0, Math.Min(this.f.map.DisplayHeight - Game1.viewport.Height, (int) this.targetCrop.Y * 64 /*0x40*/ - Game1.viewport.Height / 2));
    this.fairyPosition = new Vector2((float) (Game1.viewport.X + Game1.viewport.Width + 128 /*0x80*/), (float) ((double) this.targetCrop.Y * 64.0 - 64.0));
    Game1.changeMusicTrack("nightTime");
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
    Utility.repositionLightSource(this.lightSourceId, this.fairyPosition + new Vector2(32f, 32f));
    TimeSpan timeSpan;
    if (this.animationLoopsDone < 1)
    {
      int timerSinceFade = this.timerSinceFade;
      timeSpan = time.ElapsedGameTime;
      int milliseconds = timeSpan.Milliseconds;
      this.timerSinceFade = timerSinceFade + milliseconds;
    }
    if ((double) this.fairyPosition.X > (double) this.targetCrop.X * 64.0 + 32.0)
    {
      if (this.timerSinceFade < 2000)
        return false;
      ref float local1 = ref this.fairyPosition.X;
      double num1 = (double) local1;
      timeSpan = time.ElapsedGameTime;
      double num2 = (double) timeSpan.Milliseconds * 0.10000000149011612;
      local1 = (float) (num1 - num2);
      ref float local2 = ref this.fairyPosition.Y;
      double num3 = (double) local2;
      timeSpan = time.TotalGameTime;
      double num4 = Math.Cos((double) timeSpan.Milliseconds * Math.PI / 512.0) * 1.0;
      local2 = (float) (num3 + num4);
      int fairyFrame1 = this.fairyFrame;
      timeSpan = time.TotalGameTime;
      this.fairyFrame = timeSpan.Milliseconds % 500 <= 250 ? 0 : 1;
      int fairyFrame2 = this.fairyFrame;
      if (fairyFrame1 != fairyFrame2 && this.fairyFrame == 1)
      {
        Game1.playSound("batFlap");
        this.f.temporarySprites.Add(new TemporaryAnimatedSprite(11, this.fairyPosition + new Vector2(32f, 0.0f), Color.Purple));
      }
      if ((double) this.fairyPosition.X <= (double) this.targetCrop.X * 64.0 + 32.0)
        this.fairyFrame = 1;
    }
    else if (this.animationLoopsDone < 4)
    {
      int fairyAnimationTimer = this.fairyAnimationTimer;
      timeSpan = time.ElapsedGameTime;
      int milliseconds = timeSpan.Milliseconds;
      this.fairyAnimationTimer = fairyAnimationTimer + milliseconds;
      if (this.fairyAnimationTimer > 250)
      {
        this.fairyAnimationTimer = 0;
        if (!this.animateLeft)
        {
          ++this.fairyFrame;
          if (this.fairyFrame == 3)
          {
            this.animateLeft = true;
            this.f.temporarySprites.Add(new TemporaryAnimatedSprite(10, this.fairyPosition + new Vector2(-16f, 64f), Color.LightPink));
            Game1.playSound("yoba");
            TerrainFeature terrainFeature;
            if (this.f.terrainFeatures.TryGetValue(this.targetCrop, out terrainFeature) && terrainFeature is HoeDirt hoeDirt)
              hoeDirt.crop.currentPhase.Value = Math.Min(hoeDirt.crop.currentPhase.Value + 1, hoeDirt.crop.phaseDays.Count - 1);
          }
        }
        else
        {
          --this.fairyFrame;
          if (this.fairyFrame == 1)
          {
            this.animateLeft = false;
            ++this.animationLoopsDone;
            if (this.animationLoopsDone >= 4)
            {
              for (int index = 0; index < 10; ++index)
                DelayedAction.playSoundAfterDelay("batFlap", 4000 + 500 * index);
            }
          }
        }
      }
    }
    else
    {
      int fairyAnimationTimer = this.fairyAnimationTimer;
      timeSpan = time.ElapsedGameTime;
      int milliseconds1 = timeSpan.Milliseconds;
      this.fairyAnimationTimer = fairyAnimationTimer + milliseconds1;
      timeSpan = time.TotalGameTime;
      this.fairyFrame = timeSpan.Milliseconds % 500 <= 250 ? 0 : 1;
      if (this.fairyAnimationTimer > 2000 && (double) this.fairyPosition.Y > -999999.0)
      {
        ref float local3 = ref this.fairyPosition.X;
        double num5 = (double) local3;
        timeSpan = time.TotalGameTime;
        double num6 = Math.Cos((double) timeSpan.Milliseconds * Math.PI / 256.0) * 2.0;
        local3 = (float) (num5 + num6);
        ref float local4 = ref this.fairyPosition.Y;
        double num7 = (double) local4;
        timeSpan = time.ElapsedGameTime;
        double num8 = (double) timeSpan.Milliseconds * 0.20000000298023224;
        local4 = (float) (num7 - num8);
      }
      if ((double) this.fairyPosition.Y < (double) (Game1.viewport.Y - 128 /*0x80*/) || float.IsNaN(this.fairyPosition.Y))
      {
        if (!Game1.fadeToBlack && (double) this.fairyPosition.Y != -999999.0)
        {
          Game1.globalFadeToBlack(new Game1.afterFadeFunction(this.afterLastFade));
          Game1.changeMusicTrack("none");
          this.timerSinceFade = 0;
          this.fairyPosition.Y = -999999f;
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
    b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, this.fairyPosition), new Rectangle?(new Rectangle(16 /*0x10*/ + this.fairyFrame * 16 /*0x10*/, 592, 16 /*0x10*/, 16 /*0x10*/)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.9999999f);
  }

  /// <inheritdoc />
  public override void makeChangesToLocation()
  {
    if (!Game1.IsMasterGame)
      return;
    for (int x = (int) this.targetCrop.X - 2; (double) x <= (double) this.targetCrop.X + 2.0; ++x)
    {
      for (int y = (int) this.targetCrop.Y - 2; (double) y <= (double) this.targetCrop.Y + 2.0; ++y)
      {
        TerrainFeature terrainFeature;
        if (this.f.terrainFeatures.TryGetValue(new Vector2((float) x, (float) y), out terrainFeature) && terrainFeature is HoeDirt hoeDirt && hoeDirt.crop != null)
          hoeDirt.crop.growCompletely();
      }
    }
  }

  /// <summary>Choose a random valid crop to target.</summary>
  protected Vector2 ChooseCrop()
  {
    Vector2[] array = this.f.terrainFeatures.Pairs.Where<KeyValuePair<Vector2, TerrainFeature>>((Func<KeyValuePair<Vector2, TerrainFeature>, bool>) (p => p.Value is HoeDirt hoeDirt && hoeDirt.crop != null && !hoeDirt.crop.dead.Value && !hoeDirt.crop.isWildSeedCrop() && hoeDirt.crop.currentPhase.Value < hoeDirt.crop.phaseDays.Count - 1)).OrderBy<KeyValuePair<Vector2, TerrainFeature>, float>((Func<KeyValuePair<Vector2, TerrainFeature>, float>) (p => p.Key.X)).ThenBy<KeyValuePair<Vector2, TerrainFeature>, float>((Func<KeyValuePair<Vector2, TerrainFeature>, float>) (p => p.Key.Y)).Select<KeyValuePair<Vector2, TerrainFeature>, Vector2>((Func<KeyValuePair<Vector2, TerrainFeature>, Vector2>) (p => p.Key)).ToArray<Vector2>();
    return Utility.CreateRandom((double) Game1.uniqueIDForThisGame, (double) Game1.stats.DaysPlayed).ChooseFrom<Vector2>((IList<Vector2>) array);
  }
}
