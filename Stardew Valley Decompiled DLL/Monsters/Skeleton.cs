// Decompiled with JetBrains decompiler
// Type: StardewValley.Monsters.Skeleton
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Netcode;
using StardewValley.Extensions;
using StardewValley.Pathfinding;
using StardewValley.Projectiles;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley.Monsters;

public class Skeleton : Monster
{
  [XmlIgnore]
  public bool spottedPlayer;
  [XmlIgnore]
  public readonly NetBool throwing = new NetBool();
  public readonly NetBool isMage = new NetBool();
  private int controllerAttemptTimer;

  public Skeleton()
  {
  }

  public Skeleton(Vector2 position, bool isMage = false)
    : base(nameof (Skeleton), position, Game1.random.Next(4))
  {
    this.isMage.Value = isMage;
    this.reloadSprite(false);
    this.Sprite.SpriteHeight = 32 /*0x20*/;
    this.Sprite.UpdateSourceRect();
    this.IsWalkingTowardPlayer = false;
    this.jitteriness.Value = 0.0;
  }

  protected override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.throwing, "throwing").AddField((INetSerializable) this.isMage, "isMage");
    this.position.Field.AxisAlignedMovement = true;
  }

  /// <inheritdoc />
  public override void reloadSprite(bool onlyAppearance = false)
  {
    this.Sprite = new AnimatedSprite("Characters\\Monsters\\Skeleton" + (this.isMage.Value ? " Mage" : ""));
    this.Sprite.SpriteHeight = 32 /*0x20*/;
    this.Sprite.UpdateSourceRect();
  }

  public override List<Item> getExtraDropItems()
  {
    List<Item> extraDropItems = new List<Item>();
    if (Game1.random.NextDouble() < 0.04)
      extraDropItems.Add(ItemRegistry.Create("(W)5"));
    return extraDropItems;
  }

  public override int takeDamage(
    int damage,
    int xTrajectory,
    int yTrajectory,
    bool isBomb,
    double addedPrecision,
    Farmer who)
  {
    this.currentLocation.playSound("skeletonHit");
    this.Slipperiness = 3;
    if (this.throwing.Value)
    {
      this.throwing.Value = false;
      this.Halt();
    }
    if (this.Health - damage <= 0)
    {
      Game1.multiplayer.broadcastSprites(this.currentLocation, new TemporaryAnimatedSprite(46, this.Position, Color.White, 10, animationInterval: 70f));
      Game1.multiplayer.broadcastSprites(this.currentLocation, new TemporaryAnimatedSprite(46, this.Position + new Vector2(-16f, 0.0f), Color.White, 10, animationInterval: 70f)
      {
        delayBeforeAnimationStart = 100
      });
      Game1.multiplayer.broadcastSprites(this.currentLocation, new TemporaryAnimatedSprite(46, this.Position + new Vector2(16f, 0.0f), Color.White, 10, animationInterval: 70f)
      {
        delayBeforeAnimationStart = 200
      });
    }
    return base.takeDamage(damage, xTrajectory, yTrajectory, isBomb, addedPrecision, who);
  }

  public override void shedChunks(int number)
  {
    Point standingPixel = this.StandingPixel;
    Game1.createRadialDebris(this.currentLocation, this.Sprite.textureName.Value, new Rectangle(0, 128 /*0x80*/, 16 /*0x10*/, 16 /*0x10*/), 8, standingPixel.X, standingPixel.Y, number, this.TilePoint.Y, Color.White, 4f);
  }

  public override void BuffForAdditionalDifficulty(int additional_difficulty)
  {
    base.BuffForAdditionalDifficulty(additional_difficulty);
    if (this.isMage.Value)
      return;
    this.MaxHealth += 300;
    this.Health += 300;
  }

  protected override void sharedDeathAnimation()
  {
    Point standingPixel = this.StandingPixel;
    this.currentLocation.playSound("skeletonDie");
    this.shedChunks(20);
    Game1.createRadialDebris(this.currentLocation, this.Sprite.textureName.Value, new Rectangle(3, Game1.random.Choose<int>(3, 35), 10, 10), 11, standingPixel.X, standingPixel.Y, 1, this.TilePoint.Y, Color.White, 4f);
  }

  public override void update(GameTime time, GameLocation location)
  {
    if (!this.throwing.Value)
    {
      base.update(time, location);
    }
    else
    {
      if (Game1.IsMasterGame)
        this.behaviorAtGameTick(time);
      this.updateAnimation(time);
    }
  }

  protected override void updateMonsterSlaveAnimation(GameTime time)
  {
    if (this.throwing.Value)
    {
      if (this.invincibleCountdown > 0)
      {
        this.invincibleCountdown -= time.ElapsedGameTime.Milliseconds;
        if (this.invincibleCountdown <= 0)
          this.stopGlowing();
      }
      if (!this.Sprite.Animate(time, 20, 4, 150f))
        return;
      this.Sprite.currentFrame = 23;
    }
    else if (this.isMoving())
    {
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
    else
      this.Sprite.StopAnimation();
  }

  public override void behaviorAtGameTick(GameTime time)
  {
    if (!this.throwing.Value)
      base.behaviorAtGameTick(time);
    TimeSpan elapsedGameTime;
    if (!this.spottedPlayer && !this.wildernessFarmMonster && Utility.doesPointHaveLineOfSightInMine(this.currentLocation, this.Tile, this.Player.Tile, 8))
    {
      this.controller = new PathFindController((Character) this, this.currentLocation, this.Player.TilePoint, -1, (PathFindController.endBehavior) null, 200);
      this.spottedPlayer = true;
      if (this.controller?.pathToEndPoint == null || this.controller.pathToEndPoint.Count == 0)
      {
        this.Halt();
        this.facePlayer(this.Player);
      }
      this.currentLocation.playSound("skeletonStep");
      this.IsWalkingTowardPlayer = true;
    }
    else if (this.throwing.Value)
    {
      if (this.invincibleCountdown > 0)
      {
        int invincibleCountdown = this.invincibleCountdown;
        elapsedGameTime = time.ElapsedGameTime;
        int milliseconds = elapsedGameTime.Milliseconds;
        this.invincibleCountdown = invincibleCountdown - milliseconds;
        if (this.invincibleCountdown <= 0)
          this.stopGlowing();
      }
      if (this.Sprite.Animate(time, 20, 4, 150f))
      {
        this.throwing.Value = false;
        this.Sprite.currentFrame = 0;
        this.faceDirection(2);
        Vector2 velocityTowardPlayer = Utility.getVelocityTowardPlayer(new Point((int) this.Position.X, (int) this.Position.Y), 8f, this.Player);
        if (this.isMage.Value)
        {
          if (Game1.random.NextBool())
            this.currentLocation.projectiles.Add((Projectile) new DebuffingProjectile("19", 14, 4, 4, 0.196349546f, velocityTowardPlayer.X, velocityTowardPlayer.Y, new Vector2(this.Position.X, this.Position.Y), this.currentLocation, (Character) this));
          else
            this.currentLocation.projectiles.Add((Projectile) new BasicProjectile(this.DamageToFarmer * 2, 9, 0, 4, 0.0f, velocityTowardPlayer.X, velocityTowardPlayer.Y, new Vector2(this.Position.X, this.Position.Y), "flameSpellHit", "flameSpell", location: this.currentLocation, firer: (Character) this));
        }
        else
          this.currentLocation.projectiles.Add((Projectile) new BasicProjectile(this.DamageToFarmer, 4, 0, 0, 0.196349546f, velocityTowardPlayer.X, velocityTowardPlayer.Y, new Vector2(this.Position.X, this.Position.Y), "skeletonHit", "skeletonStep", location: this.currentLocation, firer: (Character) this));
      }
    }
    else if (this.spottedPlayer && this.controller == null && Game1.random.NextDouble() < (this.isMage.Value ? 0.009 : 0.003) && !this.wildernessFarmMonster && Utility.doesPointHaveLineOfSightInMine(this.currentLocation, this.Tile, this.Player.Tile, 8))
    {
      this.throwing.Value = true;
      this.Halt();
      this.Sprite.currentFrame = 20;
      this.shake(750);
    }
    else if (this.withinPlayerThreshold(2))
      this.controller = (PathFindController) null;
    else if (this.spottedPlayer && this.controller == null && this.controllerAttemptTimer <= 0)
    {
      this.controller = new PathFindController((Character) this, this.currentLocation, this.Player.TilePoint, -1, (PathFindController.endBehavior) null, 200);
      this.controllerAttemptTimer = this.wildernessFarmMonster ? 2000 : 1000;
      if (this.controller?.pathToEndPoint == null || this.controller.pathToEndPoint.Count == 0)
        this.Halt();
    }
    else if (this.wildernessFarmMonster)
    {
      this.spottedPlayer = true;
      this.IsWalkingTowardPlayer = true;
    }
    int controllerAttemptTimer = this.controllerAttemptTimer;
    elapsedGameTime = time.ElapsedGameTime;
    int milliseconds1 = elapsedGameTime.Milliseconds;
    this.controllerAttemptTimer = controllerAttemptTimer - milliseconds1;
  }
}
