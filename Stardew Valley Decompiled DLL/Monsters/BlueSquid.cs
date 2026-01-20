// Decompiled with JetBrains decompiler
// Type: StardewValley.Monsters.BlueSquid
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Extensions;
using StardewValley.Projectiles;
using System;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley.Monsters;

public class BlueSquid : Monster
{
  public float nextFire;
  public int squidYOffset;
  public float canMoveTimer;
  public NetFloat projectileIntroTimer = new NetFloat();
  public NetFloat projectileOutroTimer = new NetFloat();
  public NetBool nearFarmer = new NetBool();
  public NetFloat lastRotation = new NetFloat();
  [XmlIgnore]
  public bool justThrust;

  public BlueSquid()
  {
  }

  public BlueSquid(Vector2 position)
    : base("Blue Squid", position)
  {
    this.Sprite.SpriteHeight = 24;
    this.Sprite.SpriteWidth = 24;
    this.IsWalkingTowardPlayer = true;
    this.reloadSprite(false);
    this.Sprite.UpdateSourceRect();
    this.HideShadow = true;
    this.slipperiness.Value = Game1.random.Next(6, 9);
    this.canMoveTimer = (float) Game1.random.Next(500);
    this.isHardModeMonster.Value = true;
  }

  protected override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.projectileIntroTimer, "projectileIntroTimer").AddField((INetSerializable) this.projectileOutroTimer, "projectileOutroTimer").AddField((INetSerializable) this.lastRotation, "lastRotation").AddField((INetSerializable) this.nearFarmer, "nearFarmer");
    this.lastRotation.Interpolated(false, false);
    this.projectileIntroTimer.Interpolated(false, false);
    this.projectileOutroTimer.Interpolated(false, false);
  }

  /// <inheritdoc />
  public override void reloadSprite(bool onlyAppearance = false)
  {
    this.Sprite = new AnimatedSprite("Characters\\Monsters\\Blue Squid", 0, 24, 24);
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
      this.projectileOutroTimer.Value = 0.0f;
      this.projectileIntroTimer.Value = 0.0f;
      this.shakeTimer = 250;
      this.setTrajectory(xTrajectory, yTrajectory);
      this.lastRotation.Value = (float) Math.Atan2(-(double) this.yVelocity, (double) this.xVelocity) + 1.57079637f;
      DelayedAction.playSoundAfterDelay("squid_hit", 80 /*0x50*/, this.currentLocation);
      this.currentLocation.playSound("slimeHit");
      if (this.Health <= 0)
        this.deathAnimation();
    }
    return damage1;
  }

  protected override void sharedDeathAnimation()
  {
    this.currentLocation.localSound("slimedead");
    if (this.Sprite.Texture.Height <= this.Sprite.getHeight() * 4)
      return;
    Point standingPixel = this.StandingPixel;
    Game1.createRadialDebris(this.currentLocation, this.Sprite.textureName.Value, new Rectangle(0, 48 /*0x30*/, 16 /*0x10*/, 16 /*0x10*/), 8, standingPixel.X, standingPixel.Y, 6, this.TilePoint.Y, Color.White, 4f * this.scale.Value);
  }

  protected override void localDeathAnimation()
  {
    Game1.multiplayer.broadcastSprites(this.currentLocation, new TemporaryAnimatedSprite(44, this.Position, Color.HotPink * 0.86f, 10)
    {
      interval = 70f,
      holdLastFrame = true,
      alphaFade = 0.01f
    });
    Game1.multiplayer.broadcastSprites(this.currentLocation, new TemporaryAnimatedSprite(44, this.Position + new Vector2(-16f, 0.0f), Color.HotPink * 0.86f, 10)
    {
      interval = 70f,
      delayBeforeAnimationStart = 0,
      holdLastFrame = true,
      alphaFade = 0.01f
    });
    Game1.multiplayer.broadcastSprites(this.currentLocation, new TemporaryAnimatedSprite(44, this.Position + new Vector2(0.0f, -16f), Color.HotPink * 0.86f, 10)
    {
      interval = 70f,
      delayBeforeAnimationStart = 100,
      holdLastFrame = true,
      alphaFade = 0.01f
    });
    Game1.multiplayer.broadcastSprites(this.currentLocation, new TemporaryAnimatedSprite(44, this.Position + new Vector2(16f, 0.0f), Color.HotPink * 0.86f, 10)
    {
      interval = 70f,
      delayBeforeAnimationStart = 200,
      holdLastFrame = true,
      alphaFade = 0.01f
    });
  }

  public override Rectangle GetBoundingBox()
  {
    if (this.Sprite == null)
      return Rectangle.Empty;
    Vector2 position = this.Position;
    int width = this.GetSpriteWidthForPositioning() * 4 * 3 / 4;
    return new Rectangle((int) position.X, (int) position.Y + 16 /*0x10*/, width, 64 /*0x40*/);
  }

  public override void drawAboveAllLayers(SpriteBatch b)
  {
    int y1 = this.StandingPixel.Y;
    SpriteBatch spriteBatch = b;
    Texture2D shadowTexture = Game1.shadowTexture;
    Vector2 position = this.getLocalPosition(Game1.viewport) + new Vector2(32f, 96f);
    Rectangle? sourceRectangle = new Rectangle?(Game1.shadowTexture.Bounds);
    Color white = Color.White;
    Rectangle bounds = Game1.shadowTexture.Bounds;
    double x = (double) bounds.Center.X;
    bounds = Game1.shadowTexture.Bounds;
    double y2 = (double) bounds.Center.Y;
    Vector2 origin = new Vector2((float) x, (float) y2);
    double scale = (double) Math.Min(4f, (float) (4.0 + (double) this.squidYOffset / 20.0));
    double layerDepth = (double) (y1 - 32 /*0x20*/) / 10000.0;
    spriteBatch.Draw(shadowTexture, position, sourceRectangle, white, 0.0f, origin, (float) scale, SpriteEffects.None, (float) layerDepth);
    b.Draw(this.Sprite.Texture, this.getLocalPosition(Game1.viewport) + new Vector2(32f, (float) (21 + this.squidYOffset)) + new Vector2(this.shakeTimer > 0 ? (float) Game1.random.Next(-2, 3) : 0.0f, this.shakeTimer > 0 ? (float) Game1.random.Next(-2, 3) : 0.0f), new Rectangle?(this.Sprite.SourceRect), Color.White, this.lastRotation.Value, new Vector2(12f, 12f), Math.Max(0.2f, this.scale.Value) * 4f, this.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0.0f, this.drawOnTop ? 0.991f : (float) y1 / 10000f));
  }

  protected override void updateAnimation(GameTime time)
  {
    if (this.Sprite.CurrentFrame != 2)
      this.justThrust = false;
    if ((double) this.projectileIntroTimer.Value > 0.0)
    {
      this.shakeTimer = 10;
      this.Sprite.CurrentFrame = 6;
      --this.squidYOffset;
      if (this.squidYOffset < 0)
        this.squidYOffset = 0;
    }
    else if ((double) this.projectileOutroTimer.Value > 0.0)
    {
      this.Sprite.CurrentFrame = 5;
      this.squidYOffset += 2;
    }
    else
    {
      this.squidYOffset = (int) (Math.Sin(time.TotalGameTime.TotalMilliseconds / 2000.0 * Math.PI * 2.0) * 30.0);
      this.Sprite.currentFrame = Math.Abs(this.squidYOffset - 24) / 12;
      if (this.squidYOffset < 0)
        this.Sprite.CurrentFrame = 2;
    }
    this.Sprite.UpdateSourceRect();
  }

  public override void noMovementProgressNearPlayerBehavior()
  {
  }

  public override void behaviorAtGameTick(GameTime time)
  {
    base.behaviorAtGameTick(time);
    this.nearFarmer.Value = this.withinPlayerThreshold(10) || this.focusedOnFarmers;
    if ((double) this.projectileIntroTimer.Value <= 0.0 && (double) this.projectileOutroTimer.Value <= 0.0)
    {
      if ((double) Math.Abs(this.xVelocity) <= 1.0 && (double) Math.Abs(this.yVelocity) <= 1.0 && this.nearFarmer.Value)
      {
        Utility.getVelocityTowardPoint(this.findPlayer().position.Value, this.position.Value, (float) Game1.random.Next(25, 50)).X *= -1f;
        if ((double) this.canMoveTimer > 0.0)
          this.canMoveTimer -= (float) time.ElapsedGameTime.TotalMilliseconds;
        if (!this.justThrust && this.Sprite.CurrentFrame == 2 && (double) this.canMoveTimer <= 0.0)
        {
          this.justThrust = true;
          Vector2 velocityTowardPoint = Utility.getVelocityTowardPoint(this.findPlayer().position.Value, this.position.Value + new Vector2((float) Game1.random.Next(-64, 64 /*0x40*/)), (float) Game1.random.Next(25, 50));
          velocityTowardPoint.X *= -1f;
          this.setTrajectory(velocityTowardPoint);
          this.lastRotation.Value = (float) Math.Atan2(-(double) this.yVelocity, (double) this.xVelocity) + 1.57079637f;
          this.currentLocation.playSound("squid_move");
          this.canMoveTimer = 500f;
        }
      }
      else if (!this.nearFarmer.Value)
        this.lastRotation.Value = 0.0f;
    }
    if (((double) Math.Abs(this.xVelocity) >= 10.0 || (double) Math.Abs(this.yVelocity) >= 10.0) && Game1.random.NextDouble() < 0.25)
      Game1.multiplayer.broadcastSprites(this.currentLocation, new TemporaryAnimatedSprite("LooseSprites\\Cursors2", new Rectangle(Game1.random.Choose<int>(135, 140), 234, 5, 5), this.Position + new Vector2(32f, (float) (32 /*0x20*/ + Game1.random.Next(-8, 8))), false, 0.01f, Color.White)
      {
        interval = 9999f,
        holdLastFrame = true,
        alphaFade = 0.01f,
        motion = new Vector2(0.0f, -1f),
        xPeriodic = true,
        xPeriodicLoopTime = (float) Game1.random.Next(800, 1200),
        xPeriodicRange = (float) Game1.random.Next(8, 20),
        scale = 4f,
        drawAboveAlwaysFront = true
      });
    if ((double) this.projectileIntroTimer.Value > 0.0)
    {
      this.projectileIntroTimer.Value -= (float) time.ElapsedGameTime.TotalMilliseconds;
      this.shakeTimer = 10;
      if (Game1.random.NextDouble() < 0.25)
        Game1.multiplayer.broadcastSprites(this.currentLocation, new TemporaryAnimatedSprite("LooseSprites\\Cursors2", new Rectangle(Game1.random.Choose<int>(135, 140), 234, 5, 5), this.Position + new Vector2((float) (21 + Game1.random.Next(-21, 21)), (float) (this.squidYOffset / 2 + 32 /*0x20*/ + Game1.random.Next(-32, 32 /*0x20*/))), false, 0.01f, Color.White)
        {
          interval = 9999f,
          holdLastFrame = true,
          alphaFade = 0.01f,
          motion = new Vector2(0.0f, -1f),
          xPeriodic = true,
          xPeriodicLoopTime = (float) Game1.random.Next(800, 1200),
          xPeriodicRange = (float) Game1.random.Next(8, 20),
          scale = 4f,
          drawAboveAlwaysFront = true
        });
      if ((double) this.projectileIntroTimer.Value < 0.0)
      {
        this.projectileOutroTimer.Value = 500f;
        this.IsWalkingTowardPlayer = false;
        this.Halt();
        Point standingPixel = this.StandingPixel;
        Vector2 velocityTowardPlayer = Utility.getVelocityTowardPlayer(standingPixel, 8f, this.Player);
        DebuffingProjectile debuffingProjectile = new DebuffingProjectile("27", 8, 3, 4, 0.0f, velocityTowardPlayer.X, velocityTowardPlayer.Y, Utility.PointToVector2(standingPixel) - new Vector2(32f, (float) -this.squidYOffset), this.currentLocation, (Character) this);
        debuffingProjectile.height.Value = 48f;
        this.currentLocation.projectiles.Add((Projectile) debuffingProjectile);
        this.currentLocation.playSound("debuffSpell");
        this.nextFire = (float) Game1.random.Next(1200, 3500);
      }
    }
    else if ((double) this.projectileOutroTimer.Value > 0.0)
      this.projectileOutroTimer.Value -= (float) time.ElapsedGameTime.TotalMilliseconds;
    this.nextFire = Math.Max(0.0f, this.nextFire - (float) time.ElapsedGameTime.Milliseconds);
    if (!this.withinPlayerThreshold(6) || (double) this.nextFire != 0.0 || (double) this.projectileIntroTimer.Value > 0.0 || (double) Math.Abs(this.xVelocity) >= 1.0 || (double) Math.Abs(this.yVelocity) >= 1.0 || Game1.random.NextDouble() >= 0.003 || (double) this.canMoveTimer > 0.0 || !this.currentLocation.hasTileAt(this.TilePoint.X, this.TilePoint.Y, "Back") || this.currentLocation.hasTileAt(this.TilePoint.X, this.TilePoint.Y, "Buildings") || this.currentLocation.hasTileAt(this.TilePoint.X, this.TilePoint.Y, "Front"))
      return;
    this.projectileIntroTimer.Value = 1000f;
    this.lastRotation.Value = 0.0f;
    this.currentLocation.playSound("squid_bubble");
  }
}
