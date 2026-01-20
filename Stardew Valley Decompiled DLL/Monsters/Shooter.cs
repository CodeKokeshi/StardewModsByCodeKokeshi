// Decompiled with JetBrains decompiler
// Type: StardewValley.Monsters.Shooter
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Netcode;
using StardewValley.Projectiles;
using System;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley.Monsters;

public class Shooter : Monster
{
  public NetBool shooting = new NetBool();
  public int shotsLeft;
  public float nextShot;
  public int projectileSpeed = 12;
  public string projectileDebuff = "26";
  public int numberOfShotsPerFire = 1;
  public float aimTime = 0.25f;
  public float burstTime = 0.25f;
  public float aimEndTime = 1f;
  public int firedProjectile = 12;
  public string damageSound = "shadowHit";
  public string fireSound = "Cowboy_gunshot";
  public int projectileRange = 10;
  public int desiredDistance = 5;
  public int fireRange = 8;
  [XmlIgnore]
  public NetEvent0 fireEvent = new NetEvent0();

  public Shooter()
  {
  }

  protected override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.shooting, "shooting").AddField((INetSerializable) this.fireEvent, "fireEvent");
    this.fireEvent.onEvent += new NetEvent0.Event(this.OnFire);
  }

  public override int GetBaseDifficultyLevel() => 1;

  public virtual void OnFire() => this.shakeTimer = 250;

  public override bool ShouldActuallyMoveAwayFromPlayer()
  {
    if (this.Player != null)
    {
      Point tilePoint1 = this.Player.TilePoint;
      Point tilePoint2 = this.TilePoint;
      if (Math.Abs(tilePoint1.X - tilePoint2.X) < this.desiredDistance && Math.Abs(tilePoint1.Y - tilePoint2.Y) < this.desiredDistance)
        return true;
    }
    return base.ShouldActuallyMoveAwayFromPlayer();
  }

  public Shooter(Vector2 position)
    : base("Shadow Sniper", position)
  {
    this.Sprite.SpriteHeight = 32 /*0x20*/;
    this.Sprite.SpriteWidth = 32 /*0x20*/;
    this.forceOneTileWide.Value = true;
    this.Sprite.UpdateSourceRect();
    this.InitializeVariant();
  }

  public Shooter(Vector2 position, string monster_name)
    : base(monster_name, position)
  {
    this.Sprite.SpriteHeight = 32 /*0x20*/;
    this.Sprite.SpriteWidth = 32 /*0x20*/;
    this.forceOneTileWide.Value = true;
    this.Sprite.UpdateSourceRect();
    this.InitializeVariant();
  }

  public virtual void InitializeVariant() => this.nextShot = 1f;

  /// <inheritdoc />
  public override void reloadSprite(bool onlyAppearance = false)
  {
    this.Sprite = new AnimatedSprite("Characters\\Monsters\\" + this.Name);
    this.Sprite.SpriteHeight = 32 /*0x20*/;
    this.Sprite.UpdateSourceRect();
  }

  protected override void updateAnimation(GameTime time)
  {
    if (this.shooting.Value)
    {
      switch (this.FacingDirection)
      {
        case 0:
          this.Sprite.CurrentFrame = 18;
          break;
        case 1:
          this.Sprite.CurrentFrame = 17;
          break;
        case 2:
          this.Sprite.CurrentFrame = 16 /*0x10*/;
          break;
        case 3:
          this.Sprite.CurrentFrame = 19;
          break;
      }
    }
    if (Game1.IsMasterGame || !this.isMoving())
      return;
    switch (this.FacingDirection)
    {
      case 0:
        this.Sprite.AnimateUp(time);
        break;
      case 1:
        this.Sprite.AnimateRight(time);
        break;
      case 2:
        this.Sprite.AnimateDown(time);
        break;
      case 3:
        this.Sprite.AnimateLeft(time);
        break;
    }
  }

  public override void behaviorAtGameTick(GameTime time)
  {
    if (!this.shooting.Value)
    {
      if ((double) this.nextShot > 0.0)
        this.nextShot -= (float) time.ElapsedGameTime.TotalSeconds;
      else if (this.Player != null)
      {
        Point tilePoint1 = this.Player.TilePoint;
        Point tilePoint2 = this.TilePoint;
        int x1 = tilePoint1.X;
        int y1 = tilePoint1.Y;
        int x2 = tilePoint2.X;
        int y2 = tilePoint2.Y;
        if (Math.Abs(x1 - x2) <= this.fireRange && Math.Abs(y1 - y2) <= this.fireRange && (Math.Abs(x1 - x2) < 2 || Math.Abs(y1 - y2) < 2))
        {
          this.Halt();
          this.faceGeneralDirection(this.Player.getStandingPosition());
          this.shooting.Value = true;
          this.nextShot = this.aimTime;
          this.shotsLeft = this.numberOfShotsPerFire;
        }
      }
    }
    else
    {
      this.xVelocity = 0.0f;
      this.yVelocity = 0.0f;
      if (this.shotsLeft > 0)
      {
        if ((double) this.nextShot > 0.0)
        {
          this.nextShot -= (float) time.ElapsedGameTime.TotalSeconds;
          if ((double) this.nextShot <= 0.0)
          {
            Vector2 vector2_1;
            float num;
            switch (this.FacingDirection)
            {
              case 0:
                vector2_1 = new Vector2(0.0f, -1f);
                num = 0.0f;
                break;
              case 1:
                vector2_1 = new Vector2(1f, 0.0f);
                num = 1.57079637f;
                break;
              case 2:
                vector2_1 = new Vector2(0.0f, 1f);
                num = 3.14159274f;
                break;
              case 3:
                vector2_1 = new Vector2(-1f, 0.0f);
                num = -1.57079637f;
                break;
              default:
                vector2_1 = Vector2.Zero;
                num = 0.0f;
                break;
            }
            Vector2 vector2_2 = vector2_1 * (float) this.projectileSpeed;
            this.fireEvent.Fire();
            this.currentLocation.playSound(this.fireSound);
            BasicProjectile basicProjectile = new BasicProjectile(this.DamageToFarmer, this.firedProjectile, 0, 0, 0.0f, vector2_2.X, vector2_2.Y, this.Position, location: this.currentLocation, firer: (Character) this);
            basicProjectile.startingRotation.Value = num;
            basicProjectile.height.Value = 24f;
            basicProjectile.debuff.Value = this.projectileDebuff;
            basicProjectile.ignoreTravelGracePeriod.Value = true;
            basicProjectile.IgnoreLocationCollision = true;
            basicProjectile.maxTravelDistance.Value = 64 /*0x40*/ * this.projectileRange;
            this.currentLocation.projectiles.Add((Projectile) basicProjectile);
            --this.shotsLeft;
            this.nextShot = this.shotsLeft != 0 ? this.burstTime : this.aimEndTime;
          }
        }
      }
      else if ((double) this.nextShot > 0.0)
      {
        this.nextShot -= (float) time.ElapsedGameTime.TotalSeconds;
      }
      else
      {
        this.shooting.Value = false;
        this.nextShot = 2f;
      }
    }
    base.behaviorAtGameTick(time);
  }

  public override void updateMovement(GameLocation location, GameTime time)
  {
    if (this.shooting.Value)
      this.MovePosition(time, Game1.viewport, location);
    else
      base.updateMovement(location, time);
  }

  public override int takeDamage(
    int damage,
    int xTrajectory,
    int yTrajectory,
    bool isBomb,
    double addedPrecision,
    Farmer who)
  {
    this.shooting.Value = false;
    this.shotsLeft = 0;
    this.nextShot = Math.Max(0.5f, this.nextShot);
    this.currentLocation.playSound(this.damageSound);
    return base.takeDamage(damage, xTrajectory, yTrajectory, isBomb, addedPrecision, who);
  }

  protected override void localDeathAnimation()
  {
    if (!(this.Name == "Shadow Sniper"))
      return;
    Utility.makeTemporarySpriteJuicier(new TemporaryAnimatedSprite(45, this.Position, Color.White, 10), this.currentLocation);
    for (int index = 1; index < 3; ++index)
    {
      this.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(6, this.Position + new Vector2(0.0f, 1f) * 64f * (float) index, Color.Gray * 0.75f, 10)
      {
        delayBeforeAnimationStart = index * 159
      });
      this.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(6, this.Position + new Vector2(0.0f, -1f) * 64f * (float) index, Color.Gray * 0.75f, 10)
      {
        delayBeforeAnimationStart = index * 159
      });
      this.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(6, this.Position + new Vector2(1f, 0.0f) * 64f * (float) index, Color.Gray * 0.75f, 10)
      {
        delayBeforeAnimationStart = index * 159
      });
      this.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(6, this.Position + new Vector2(-1f, 0.0f) * 64f * (float) index, Color.Gray * 0.75f, 10)
      {
        delayBeforeAnimationStart = index * 159
      });
    }
    this.currentLocation.localSound("shadowDie");
  }

  protected override void sharedDeathAnimation()
  {
    Point standingPixel = this.StandingPixel;
    Game1.createRadialDebris(this.currentLocation, this.Sprite.textureName.Value, new Rectangle(this.Sprite.SourceRect.X, this.Sprite.SourceRect.Y, 16 /*0x10*/, 5), 16 /*0x10*/, standingPixel.X, standingPixel.Y - 32 /*0x20*/, 1, standingPixel.Y / 64 /*0x40*/, Color.White, 4f);
    Game1.createRadialDebris(this.currentLocation, this.Sprite.textureName.Value, new Rectangle(this.Sprite.SourceRect.X + 2, this.Sprite.SourceRect.Y + 5, 16 /*0x10*/, 5), 10, standingPixel.X, standingPixel.Y - 32 /*0x20*/, 1, standingPixel.Y / 64 /*0x40*/, Color.White, 4f);
  }

  public override void update(GameTime time, GameLocation location)
  {
    base.update(time, location);
    this.fireEvent.Poll();
  }
}
