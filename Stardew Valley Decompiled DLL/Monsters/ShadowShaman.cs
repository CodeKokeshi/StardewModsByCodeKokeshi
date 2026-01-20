// Decompiled with JetBrains decompiler
// Type: StardewValley.Monsters.ShadowShaman
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Extensions;
using StardewValley.Pathfinding;
using StardewValley.Projectiles;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley.Monsters;

public class ShadowShaman : Monster
{
  public const int visionDistance = 8;
  public const int spellCooldown = 1500;
  [XmlIgnore]
  public bool spottedPlayer;
  [XmlIgnore]
  public readonly NetBool casting = new NetBool();
  [XmlIgnore]
  public int coolDown = 1500;
  [XmlIgnore]
  public float rotationTimer;

  public ShadowShaman()
  {
  }

  public ShadowShaman(Vector2 position)
    : base("Shadow Shaman", position)
  {
    Friendship friendship;
    if (!Game1.MasterPlayer.friendshipData.TryGetValue("???", out friendship) || friendship.Points < 1250)
      return;
    this.DamageToFarmer = 0;
  }

  protected override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.casting, "casting");
  }

  /// <inheritdoc />
  public override void reloadSprite(bool onlyAppearance = false)
  {
    this.Sprite = new AnimatedSprite("Characters\\Monsters\\Shadow Shaman");
  }

  public override void draw(SpriteBatch b)
  {
    base.draw(b);
    if (!this.casting.Value)
      return;
    for (int index = 0; index < 8; ++index)
      b.Draw(Projectile.projectileSheet, Game1.GlobalToLocal(Game1.viewport, this.getStandingPosition()), new Rectangle?(new Rectangle(119, 6, 3, 3)), Color.White * 0.7f, this.rotationTimer + (float) ((double) index * 3.1415927410125732 / 4.0), new Vector2(8f, 48f), 6f, SpriteEffects.None, 0.95f);
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
      if (this.casting.Value && Game1.random.NextBool())
      {
        this.coolDown += 200;
      }
      else
      {
        this.setTrajectory(xTrajectory, yTrajectory);
        this.currentLocation.playSound("shadowHit");
      }
      if (this.Health <= 0)
      {
        this.currentLocation.playSound("shadowDie");
        this.deathAnimation();
      }
    }
    return damage1;
  }

  protected override void sharedDeathAnimation()
  {
    Point standingPixel = this.StandingPixel;
    Game1.createRadialDebris(this.currentLocation, this.Sprite.textureName.Value, new Rectangle(this.Sprite.SourceRect.X, this.Sprite.SourceRect.Y, 16 /*0x10*/, 5), 16 /*0x10*/, standingPixel.X, standingPixel.Y - 32 /*0x20*/, 1, standingPixel.Y / 64 /*0x40*/, Color.White);
    Game1.createRadialDebris(this.currentLocation, this.Sprite.textureName.Value, new Rectangle(this.Sprite.SourceRect.X + 2, this.Sprite.SourceRect.Y + 5, 16 /*0x10*/, 5), 10, standingPixel.X, standingPixel.Y - 32 /*0x20*/, 1, standingPixel.Y / 64 /*0x40*/, Color.White);
    Game1.createRadialDebris(this.currentLocation, this.Sprite.textureName.Value, new Rectangle(0, 10, 16 /*0x10*/, 5), 16 /*0x10*/, standingPixel.X, standingPixel.Y - 32 /*0x20*/, 1, standingPixel.Y / 64 /*0x40*/, Color.White);
  }

  protected override void localDeathAnimation()
  {
    Utility.makeTemporarySpriteJuicier(new TemporaryAnimatedSprite(45, this.Position, Color.White, 10), this.currentLocation);
    for (int index = 1; index < 3; ++index)
    {
      this.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(6, this.Position + new Vector2(1f, 1f) * 64f * (float) index, Color.Gray * 0.75f, 10)
      {
        delayBeforeAnimationStart = index * 159
      });
      this.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(6, this.Position + new Vector2(1f, -1f) * 64f * (float) index, Color.Gray * 0.75f, 10)
      {
        delayBeforeAnimationStart = index * 159
      });
      this.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(6, this.Position + new Vector2(-1f, 1f) * 64f * (float) index, Color.Gray * 0.75f, 10)
      {
        delayBeforeAnimationStart = index * 159
      });
      this.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(6, this.Position + new Vector2(-1f, -1f) * 64f * (float) index, Color.Gray * 0.75f, 10)
      {
        delayBeforeAnimationStart = index * 159
      });
    }
  }

  protected override void updateMonsterSlaveAnimation(GameTime time)
  {
    if (this.casting.Value)
    {
      this.Sprite.Animate(time, 16 /*0x10*/, 4, 200f);
      this.rotationTimer = (float) ((double) time.TotalGameTime.Milliseconds * 0.024543693289160728 / 24.0 % (1024.0 * Math.PI));
    }
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
  }

  public override void behaviorAtGameTick(GameTime time)
  {
    base.behaviorAtGameTick(time);
    if ((double) this.timeBeforeAIMovementAgain <= 0.0)
      this.IsInvisible = false;
    if (!this.spottedPlayer && Utility.couldSeePlayerInPeripheralVision(this.Player, (Character) this) && Utility.doesPointHaveLineOfSightInMine(this.currentLocation, this.Tile, this.Player.Tile, 8))
    {
      this.controller = (PathFindController) null;
      this.spottedPlayer = true;
      this.Halt();
      this.facePlayer(this.Player);
      if (Game1.random.NextDouble() >= 0.3)
        return;
      this.currentLocation.playSound("shadowpeep");
    }
    else if (this.casting.Value)
    {
      this.IsWalkingTowardPlayer = false;
      this.Sprite.Animate(time, 16 /*0x10*/, 4, 200f);
      this.rotationTimer = (float) ((double) time.TotalGameTime.Milliseconds * 0.024543693289160728 / 24.0 % (1024.0 * Math.PI));
      this.coolDown -= time.ElapsedGameTime.Milliseconds;
      if (this.coolDown > 0)
        return;
      this.Scale = 1f;
      Rectangle boundingBox1 = this.GetBoundingBox();
      Vector2 velocityTowardPlayer = Utility.getVelocityTowardPlayer(boundingBox1.Center, 15f, this.Player);
      if (this.Player.Attack >= 0 && Game1.random.NextDouble() < 0.6)
      {
        this.currentLocation.projectiles.Add((Projectile) new DebuffingProjectile("14", 7, 4, 4, 0.196349546f, velocityTowardPlayer.X, velocityTowardPlayer.Y, new Vector2((float) boundingBox1.X, (float) boundingBox1.Y), this.currentLocation, (Character) this));
      }
      else
      {
        List<Monster> monsterList = new List<Monster>();
        foreach (NPC character in this.currentLocation.characters)
        {
          if (character is Monster monster && monster.withinPlayerThreshold(6))
            monsterList.Add(monster);
        }
        Monster monster1 = (Monster) null;
        double num1 = 1.0;
        foreach (Monster monster2 in monsterList)
        {
          if ((double) monster2.Health / (double) monster2.MaxHealth <= num1)
          {
            monster1 = monster2;
            num1 = (double) monster2.Health / (double) monster2.MaxHealth;
          }
        }
        if (monster1 != null)
        {
          int num2 = this.isHardModeMonster.Value ? 250 : 60;
          monster1.Health = Math.Min(monster1.MaxHealth, monster1.Health + num2);
          this.currentLocation.playSound("healSound");
          Game1.multiplayer.broadcastSprites(this.currentLocation, new TemporaryAnimatedSprite("TileSheets\\animations", new Rectangle(0, 256 /*0x0100*/, 64 /*0x40*/, 64 /*0x40*/), 40f, 8, 0, monster1.Position + new Vector2(32f, 64f), false, false));
          NetCollection<Debris> debris1 = this.currentLocation.debris;
          int number = num2;
          Rectangle boundingBox2 = monster1.GetBoundingBox();
          double x = (double) boundingBox2.Center.X;
          boundingBox2 = monster1.GetBoundingBox();
          double y = (double) boundingBox2.Center.Y;
          Vector2 debrisOrigin = new Vector2((float) x, (float) y);
          Color green = Color.Green;
          Monster toHover = monster1;
          Debris debris2 = new Debris(number, debrisOrigin, green, 1f, (Character) toHover);
          debris1.Add(debris2);
        }
      }
      this.casting.Value = false;
      this.coolDown = 1500;
      this.IsWalkingTowardPlayer = true;
    }
    else if (this.spottedPlayer)
    {
      if (this.withinPlayerThreshold(8))
      {
        if (this.Health < 30)
        {
          this.IsWalkingTowardPlayer = false;
          Point standingPixel1 = this.StandingPixel;
          Point standingPixel2 = this.Player.StandingPixel;
          if (Math.Abs(standingPixel2.Y - standingPixel1.Y) > 192 /*0xC0*/)
          {
            if (standingPixel2.X - standingPixel1.X > 0)
              this.SetMovingLeft(true);
            else
              this.SetMovingRight(true);
          }
          else if (standingPixel2.Y - standingPixel1.Y > 0)
            this.SetMovingUp(true);
          else
            this.SetMovingDown(true);
        }
        else if (this.controller == null && !Utility.doesPointHaveLineOfSightInMine(this.currentLocation, this.Tile, this.Player.Tile, 8))
        {
          this.controller = new PathFindController((Character) this, this.currentLocation, this.Player.TilePoint, -1, (PathFindController.endBehavior) null, 300);
          if (this.controller?.pathToEndPoint == null || this.controller.pathToEndPoint.Count == 0)
          {
            this.spottedPlayer = false;
            this.Halt();
            this.controller = (PathFindController) null;
            this.addedSpeed = 0.0f;
          }
        }
        else if (this.coolDown <= 0 && Game1.random.NextDouble() < 0.02)
        {
          this.casting.Value = true;
          this.controller = (PathFindController) null;
          this.IsWalkingTowardPlayer = false;
          this.Halt();
          this.coolDown = 500;
        }
        this.coolDown -= time.ElapsedGameTime.Milliseconds;
      }
      else
      {
        this.IsWalkingTowardPlayer = false;
        this.spottedPlayer = false;
        this.controller = (PathFindController) null;
        this.addedSpeed = 0.0f;
      }
    }
    else
      this.defaultMovementBehavior(time);
  }
}
