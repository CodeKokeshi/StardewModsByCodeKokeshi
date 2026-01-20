// Decompiled with JetBrains decompiler
// Type: StardewValley.Monsters.RockCrab
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Netcode;
using StardewValley.Extensions;
using StardewValley.Tools;
using System;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley.Monsters;

public class RockCrab : Monster
{
  [XmlIgnore]
  public bool waiter;
  [XmlIgnore]
  public readonly NetBool shellGone = new NetBool();
  [XmlIgnore]
  public readonly NetInt shellHealth = new NetInt(5);
  [XmlIgnore]
  public readonly NetBool isStickBug = new NetBool();

  public RockCrab()
  {
  }

  public RockCrab(Vector2 position)
    : base("Rock Crab", position)
  {
    this.waiter = Game1.random.NextDouble() < 0.4;
    this.moveTowardPlayerThreshold.Value = 3;
  }

  /// <inheritdoc />
  public override void reloadSprite(bool onlyAppearance = false)
  {
    base.reloadSprite(onlyAppearance);
    this.Sprite.UpdateSourceRect();
  }

  /// <summary>constructor for Lava Crab</summary>
  /// <param name="position"></param>
  /// <param name="name"></param>
  public RockCrab(Vector2 position, string name)
    : base(name, position)
  {
    this.waiter = Game1.random.NextDouble() < 0.4;
    this.moveTowardPlayerThreshold.Value = 3;
    switch (name)
    {
      case "Truffle Crab":
        this.waiter = false;
        this.moveTowardPlayerThreshold.Value = 1;
        break;
      case "Iridium Crab":
        this.waiter = true;
        this.moveTowardPlayerThreshold.Value = 1;
        break;
      case "False Magma Cap":
        this.waiter = false;
        break;
    }
  }

  public void makeStickBug()
  {
    this.isStickBug.Value = true;
    this.waiter = false;
    this.Name = "Stick Bug";
    this.DamageToFarmer = 20;
    this.MaxHealth = 700;
    this.Health = 700;
    base.reloadSprite(false);
    this.HideShadow = true;
    this.Sprite.SpriteHeight = 24;
    this.Sprite.UpdateSourceRect();
    this.objectsToDrop.Clear();
    this.objectsToDrop.Add("858");
    while (Game1.random.NextBool())
      this.objectsToDrop.Add("858");
    this.objectsToDrop.Add("829");
  }

  protected override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.shellGone, "shellGone").AddField((INetSerializable) this.shellHealth, "shellHealth").AddField((INetSerializable) this.isStickBug, "isStickBug");
    this.position.Field.AxisAlignedMovement = true;
  }

  public override bool hitWithTool(Tool t)
  {
    if (this.isStickBug.Value)
      return false;
    if (!(t is Pickaxe) || t.getLastFarmerToUse() == null || this.shellHealth.Value <= 0)
      return base.hitWithTool(t);
    this.currentLocation.playSound("hammer");
    --this.shellHealth.Value;
    this.shake(500);
    this.waiter = false;
    this.moveTowardPlayerThreshold.Value = 3;
    this.setTrajectory(Utility.getAwayFromPlayerTrajectory(this.GetBoundingBox(), t.getLastFarmerToUse()));
    if (this.shellHealth.Value <= 0)
    {
      Point tilePoint = this.TilePoint;
      this.shellGone.Value = true;
      this.moveTowardPlayer(-1);
      this.currentLocation.playSound("stoneCrack");
      Game1.createRadialDebris(this.currentLocation, 14, tilePoint.X, tilePoint.Y, Game1.random.Next(2, 7), false);
      Game1.createRadialDebris(this.currentLocation, 14, tilePoint.X, tilePoint.Y, Game1.random.Next(2, 7), false);
    }
    return true;
  }

  public override void shedChunks(int number)
  {
    Point standingPixel = this.StandingPixel;
    Game1.createRadialDebris(this.currentLocation, this.Sprite.textureName.Value, new Rectangle(0, 120, 16 /*0x10*/, 16 /*0x10*/), 8, standingPixel.X, standingPixel.Y, number, this.TilePoint.Y, Color.White, 4f * this.scale.Value);
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
    if (isBomb && !this.isStickBug.Value)
    {
      this.shellGone.Value = true;
      this.waiter = false;
      this.moveTowardPlayer(-1);
    }
    if (Game1.random.NextDouble() < this.missChance.Value - this.missChance.Value * addedPrecision)
      damage1 = -1;
    else if (this.Sprite.currentFrame % 4 == 0 && !this.shellGone.Value)
    {
      damage1 = 0;
      this.currentLocation.playSound("crafting");
    }
    else
    {
      this.Health -= damage1;
      this.Slipperiness = 3;
      this.setTrajectory(xTrajectory, yTrajectory);
      this.currentLocation.playSound("hitEnemy");
      this.glowingColor = Color.Cyan;
      if (this.Health <= 0)
      {
        this.currentLocation.playSound("monsterdead");
        this.deathAnimation();
        Utility.makeTemporarySpriteJuicier(new TemporaryAnimatedSprite(44, this.Position, Color.Red, 10)
        {
          holdLastFrame = true,
          alphaFade = 0.01f
        }, this.currentLocation);
      }
    }
    return damage1;
  }

  public override void update(GameTime time, GameLocation location)
  {
    if (!location.farmers.Any())
      return;
    if (!this.shellGone.Value && !this.Player.isRafting)
    {
      base.update(time, location);
    }
    else
    {
      if (this.Player.isRafting)
        return;
      if (Game1.IsMasterGame)
        this.behaviorAtGameTick(time);
      this.updateAnimation(time);
    }
  }

  public override void behaviorAtGameTick(GameTime time)
  {
    if (this.waiter && this.shellHealth.Value > 4)
    {
      this.moveTowardPlayerThreshold.Value = 0;
    }
    else
    {
      base.behaviorAtGameTick(time);
      if (this.isMoving() && this.Sprite.currentFrame % 4 == 0)
      {
        ++this.Sprite.currentFrame;
        this.Sprite.UpdateSourceRect();
      }
      if (!this.withinPlayerThreshold() && !this.shellGone.Value)
        this.Halt();
      else if (this.withinPlayerThreshold() && !this.shellGone.Value && this.name.Equals((object) "Truffle Crab"))
      {
        this.shellGone.Value = true;
      }
      else
      {
        if (!this.shellGone.Value)
          return;
        this.updateGlow();
        if (this.invincibleCountdown > 0)
        {
          this.glowingColor = Color.Cyan;
          this.invincibleCountdown -= time.ElapsedGameTime.Milliseconds;
          if (this.invincibleCountdown <= 0)
            this.stopGlowing();
        }
        this.IsWalkingTowardPlayer = false;
        Point standingPixel1 = this.StandingPixel;
        Point standingPixel2 = this.Player.StandingPixel;
        this.FacingDirection = this.getGeneralDirectionTowards(this.Player.getStandingPosition(), opposite: true, useTileCalculations: false);
        this.moveUp = false;
        this.moveDown = false;
        this.moveRight = false;
        this.moveLeft = false;
        this.setMovingInFacingDirection();
        this.MovePosition(time, Game1.viewport, this.currentLocation);
        this.Sprite.CurrentFrame = 16 /*0x10*/ + this.Sprite.currentFrame % 4;
      }
    }
  }

  protected override void updateMonsterSlaveAnimation(GameTime time)
  {
    if (this.isMoving())
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
    if (this.isMoving() && this.Sprite.currentFrame % 4 == 0)
    {
      ++this.Sprite.currentFrame;
      this.Sprite.UpdateSourceRect();
    }
    if (!this.shellGone.Value)
      return;
    this.updateGlow();
    if (this.invincibleCountdown > 0)
    {
      this.glowingColor = Color.Cyan;
      this.invincibleCountdown -= time.ElapsedGameTime.Milliseconds;
      if (this.invincibleCountdown <= 0)
        this.stopGlowing();
    }
    this.Sprite.currentFrame = 16 /*0x10*/ + this.Sprite.currentFrame % 4;
  }
}
