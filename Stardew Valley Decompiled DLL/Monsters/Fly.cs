// Decompiled with JetBrains decompiler
// Type: StardewValley.Monsters.Fly
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Extensions;
using System;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley.Monsters;

public class Fly : Monster
{
  public const float rotationIncrement = 0.0490873866f;
  public const int volumeTileRange = 16 /*0x10*/;
  public const int spawnTime = 1000;
  [XmlIgnore]
  public int spawningCounter = 1000;
  [XmlIgnore]
  public int wasHitCounter;
  [XmlIgnore]
  public float targetRotation;
  public static ICue buzz;
  [XmlIgnore]
  public bool turningRight;
  public bool hard;

  public Fly()
  {
  }

  public Fly(Vector2 position)
    : this(position, false)
  {
  }

  public Fly(Vector2 position, bool hard)
    : base(nameof (Fly), position)
  {
    this.Slipperiness = 24 + Game1.random.Next(-10, 10);
    this.Halt();
    this.IsWalkingTowardPlayer = false;
    this.hard = hard;
    if (hard)
    {
      this.DamageToFarmer *= 2;
      this.MaxHealth *= 3;
      this.Health = this.MaxHealth;
    }
    this.HideShadow = true;
  }

  public void setHard()
  {
    this.hard = true;
    if (!this.hard)
      return;
    this.DamageToFarmer = 12;
    this.MaxHealth = 66;
    this.Health = this.MaxHealth;
  }

  /// <inheritdoc />
  public override void reloadSprite(bool onlyAppearance = false)
  {
    this.Sprite = new AnimatedSprite("Characters\\Monsters\\Fly");
    this.HideShadow = true;
    if (onlyAppearance)
      return;
    Fly.buzz = Game1.soundBank.GetCue("flybuzzing");
  }

  public override int takeDamage(
    int damage,
    int xTrajectory,
    int yTrajectory,
    bool isBomb,
    double addedPrecision,
    Farmer who)
  {
    int damage1 = Math.Max(1, damage - this.resilience.Value);
    if (Game1.random.NextDouble() < this.missChance.Value - this.missChance.Value * addedPrecision)
    {
      damage1 = -1;
    }
    else
    {
      this.Health -= damage1;
      this.setTrajectory(xTrajectory / 3, yTrajectory / 3);
      this.wasHitCounter = 500;
      this.currentLocation?.playSound("hitEnemy");
      if (this.Health <= 0)
      {
        if (this.currentLocation != null)
        {
          this.currentLocation.playSound("monsterdead");
          Utility.makeTemporarySpriteJuicier(new TemporaryAnimatedSprite(44, this.Position, Color.HotPink, 10)
          {
            interval = 70f
          }, this.currentLocation);
        }
        Fly.buzz?.Stop(AudioStopOptions.AsAuthored);
      }
    }
    this.addedSpeed = (float) Game1.random.Next(-1, 1);
    return damage1;
  }

  public override void drawAboveAllLayers(SpriteBatch b)
  {
    if (!Utility.isOnScreen(this.Position, 128 /*0x80*/))
      return;
    int height = this.GetBoundingBox().Height;
    int y = this.StandingPixel.Y;
    b.Draw(this.Sprite.Texture, this.getLocalPosition(Game1.viewport) + new Vector2(32f, (float) (height / 2 - 32 /*0x20*/)), new Rectangle?(this.Sprite.SourceRect), this.hard ? Color.Lime : Color.White, this.rotation, new Vector2(8f, 16f), Math.Max(0.2f, this.scale.Value) * 4f, this.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0.0f, this.drawOnTop ? 0.991f : (float) (y + 8) / 10000f));
    b.Draw(Game1.shadowTexture, this.getLocalPosition(Game1.viewport) + new Vector2(32f, (float) (height / 2)), new Rectangle?(Game1.shadowTexture.Bounds), Color.White, 0.0f, new Vector2((float) Game1.shadowTexture.Bounds.Center.X, (float) Game1.shadowTexture.Bounds.Center.Y), 4f, SpriteEffects.None, (float) (y - 1) / 10000f);
    if (!this.isGlowing)
      return;
    b.Draw(this.Sprite.Texture, this.getLocalPosition(Game1.viewport) + new Vector2(32f, (float) (height / 2 - 32 /*0x20*/)), new Rectangle?(this.Sprite.SourceRect), this.glowingColor * this.glowingTransparency, this.rotation, new Vector2(8f, 16f), Math.Max(0.2f, this.scale.Value) * 4f, this.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0.0f, this.drawOnTop ? 0.99f : (float) ((double) y / 10000.0 + 1.0 / 1000.0)));
  }

  public override void drawAboveAlwaysFrontLayer(SpriteBatch b)
  {
    if (this.currentLocation == null || !this.currentLocation.treatAsOutdoors.Value)
      return;
    this.drawAboveAllLayers(b);
  }

  protected override void updateAnimation(GameTime time)
  {
    if ((Fly.buzz == null || !Fly.buzz.IsPlaying) && (this.currentLocation == null || this.currentLocation.Equals(Game1.currentLocation)))
    {
      Game1.playSound("flybuzzing", out Fly.buzz);
      Fly.buzz.SetVariable("Volume", 0.0f);
    }
    if ((double) Game1.fadeToBlackAlpha > 0.8 && Game1.fadeIn && Fly.buzz != null)
      Fly.buzz.Stop(AudioStopOptions.AsAuthored);
    else if (Fly.buzz != null)
    {
      Fly.buzz.SetVariable("Volume", Math.Max(0.0f, Fly.buzz.GetVariable("Volume") - 1f));
      float val = Math.Max(0.0f, (float) (100.0 - (double) Vector2.Distance(this.Position, this.Player.Position) / 64.0 / 16.0 * 100.0));
      if ((double) val > (double) Fly.buzz.GetVariable("Volume"))
        Fly.buzz.SetVariable("Volume", val);
    }
    if (this.wasHitCounter >= 0)
      this.wasHitCounter -= time.ElapsedGameTime.Milliseconds;
    this.Sprite.Animate(time, this.FacingDirection == 0 ? 8 : (this.FacingDirection == 2 ? 0 : this.FacingDirection * 4), 4, 75f);
    if (this.spawningCounter >= 0)
    {
      this.spawningCounter -= time.ElapsedGameTime.Milliseconds;
      this.Scale = (float) (1.0 - (double) this.spawningCounter / 1000.0);
    }
    else if ((this.withinPlayerThreshold() || Utility.isOnScreen(this.Position, 256 /*0x0100*/)) && this.invincibleCountdown <= 0)
    {
      this.faceDirection(0);
      Point standingPixel1 = this.StandingPixel;
      Point standingPixel2 = this.Player.StandingPixel;
      float num1 = (float) -(standingPixel2.X - standingPixel1.X);
      float num2 = (float) (standingPixel2.Y - standingPixel1.Y);
      float num3 = Math.Max(1f, Math.Abs(num1) + Math.Abs(num2));
      if ((double) num3 < 64.0)
      {
        this.xVelocity = Math.Max(-7f, Math.Min(7f, this.xVelocity * 1.1f));
        this.yVelocity = Math.Max(-7f, Math.Min(7f, this.yVelocity * 1.1f));
      }
      float x = num1 / num3;
      float num4 = num2 / num3;
      if (this.wasHitCounter <= 0)
      {
        this.targetRotation = (float) Math.Atan2(-(double) num4, (double) x) - 1.57079637f;
        if ((double) Math.Abs(this.targetRotation) - (double) Math.Abs(this.rotation) > 7.0 * Math.PI / 8.0 && Game1.random.NextBool())
          this.turningRight = true;
        else if ((double) Math.Abs(this.targetRotation) - (double) Math.Abs(this.rotation) < Math.PI / 8.0)
          this.turningRight = false;
        if (this.turningRight)
          this.rotation -= (float) Math.Sign(this.targetRotation - this.rotation) * ((float) Math.PI / 64f);
        else
          this.rotation += (float) Math.Sign(this.targetRotation - this.rotation) * ((float) Math.PI / 64f);
        this.rotation %= 6.28318548f;
        this.wasHitCounter = 5 + Game1.random.Next(-1, 2);
      }
      float num5 = Math.Min(7f, Math.Max(2f, (float) (7.0 - (double) num3 / 64.0 / 2.0)));
      float num6 = (float) Math.Cos((double) this.rotation + Math.PI / 2.0);
      float num7 = -(float) Math.Sin((double) this.rotation + Math.PI / 2.0);
      this.xVelocity += (float) (-(double) num6 * (double) num5 / 6.0 + (double) Game1.random.Next(-10, 10) / 100.0);
      this.yVelocity += (float) (-(double) num7 * (double) num5 / 6.0 + (double) Game1.random.Next(-10, 10) / 100.0);
      if ((double) Math.Abs(this.xVelocity) > (double) Math.Abs((float) (-(double) num6 * 7.0)))
        this.xVelocity -= (float) (-(double) num6 * (double) num5 / 6.0);
      if ((double) Math.Abs(this.yVelocity) > (double) Math.Abs((float) (-(double) num7 * 7.0)))
        this.yVelocity -= (float) (-(double) num7 * (double) num5 / 6.0);
    }
    this.resetAnimationSpeed();
  }

  public override void behaviorAtGameTick(GameTime time)
  {
    base.behaviorAtGameTick(time);
    if (double.IsNaN((double) this.xVelocity) || double.IsNaN((double) this.yVelocity))
      this.Health = -500;
    if ((double) this.Position.X > -640.0 && (double) this.Position.Y > -640.0 && (double) this.Position.X < (double) (this.currentLocation.Map.Layers[0].LayerWidth * 64 /*0x40*/ + 640) && (double) this.Position.Y < (double) (this.currentLocation.Map.Layers[0].LayerHeight * 64 /*0x40*/ + 640))
      return;
    this.Health = -500;
  }

  public override void Removed()
  {
    base.Removed();
    Fly.buzz?.Stop(AudioStopOptions.AsAuthored);
  }
}
