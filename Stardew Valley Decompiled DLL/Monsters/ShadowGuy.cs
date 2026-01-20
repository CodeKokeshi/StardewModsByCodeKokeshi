// Decompiled with JetBrains decompiler
// Type: StardewValley.Monsters.ShadowGuy
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Extensions;
using StardewValley.Pathfinding;
using StardewValley.Projectiles;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley.Monsters;

public class ShadowGuy : Monster
{
  public const int visionDistance = 8;
  public const int spellCooldown = 1500;
  [XmlIgnore]
  public bool spottedPlayer;
  [XmlIgnore]
  public bool casting;
  [XmlIgnore]
  public bool teleporting;
  [XmlIgnore]
  public int coolDown = 1500;
  [XmlIgnore]
  public IEnumerator<Point> teleportationPath;
  [XmlIgnore]
  public float rotationTimer;

  public ShadowGuy()
  {
  }

  public ShadowGuy(Vector2 position)
    : base("Shadow Guy", position)
  {
    Friendship friendship;
    if (Game1.MasterPlayer.friendshipData.TryGetValue("???", out friendship) && friendship.Points >= 1250)
      this.DamageToFarmer = 0;
    this.Halt();
  }

  /// <inheritdoc />
  public override void reloadSprite(bool onlyAppearance = false)
  {
    this.Sprite = new AnimatedSprite("Characters\\Monsters\\Shadow " + ((double) this.Position.X % 4.0 == 0.0 ? "Girl" : "Guy"));
  }

  public override void draw(SpriteBatch b)
  {
    if (!this.casting)
    {
      base.draw(b);
    }
    else
    {
      Vector2 standingPosition = this.getStandingPosition();
      int y = (int) standingPosition.Y;
      b.Draw(this.Sprite.Texture, this.getLocalPosition(Game1.viewport) + new Vector2((float) (32 /*0x20*/ + Game1.random.Next(-8, 9)), (float) (64 /*0x40*/ + Game1.random.Next(-8, 9))), new Rectangle?(this.Sprite.SourceRect), Color.White * 0.5f, this.rotation, new Vector2(32f, 64f), Math.Max(0.2f, this.scale.Value), this.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0.0f, this.drawOnTop ? 0.991f : (float) y / 10000f));
      b.Draw(this.Sprite.Texture, this.getLocalPosition(Game1.viewport) + new Vector2((float) (32 /*0x20*/ + Game1.random.Next(-8, 9)), (float) (64 /*0x40*/ + Game1.random.Next(-8, 9))), new Rectangle?(this.Sprite.SourceRect), Color.White * 0.5f, this.rotation, new Vector2(32f, 64f), Math.Max(0.2f, this.scale.Value), this.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0.0f, this.drawOnTop ? 0.991f : (float) (y + 1) / 10000f));
      Vector2 local = Game1.GlobalToLocal(Game1.viewport, standingPosition);
      Rectangle rectangle = new Rectangle(212, 20, 24, 24);
      Color color = Color.White * 0.7f;
      Vector2 origin = new Vector2(32f, 256f);
      for (int index = 0; index < 8; ++index)
        b.Draw(Projectile.projectileSheet, local, new Rectangle?(rectangle), color, this.rotationTimer + (float) ((double) index * 3.1415927410125732 / 4.0), origin, 1.5f, SpriteEffects.None, 0.95f);
    }
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
      if (this.casting && Game1.random.NextBool())
        this.coolDown += 200;
      else if (Game1.random.NextDouble() < 0.4 + 1.0 / (double) this.Health && !this.currentLocation.IsFarm)
      {
        this.castTeleport();
        if (this.Health <= 10)
          this.speed = Math.Min(3, this.speed + 1);
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

  protected override void localDeathAnimation()
  {
    this.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(45, this.Position, Color.White, 10));
  }

  protected override void sharedDeathAnimation()
  {
    Point standingPixel = this.StandingPixel;
    Game1.createRadialDebris(this.currentLocation, this.Sprite.textureName.Value, new Rectangle(this.Sprite.SourceRect.X, this.Sprite.SourceRect.Y, 64 /*0x40*/, 21), 64 /*0x40*/, standingPixel.X, standingPixel.Y - 32 /*0x20*/, 1, standingPixel.Y / 64 /*0x40*/, Color.White);
    Game1.createRadialDebris(this.currentLocation, this.Sprite.textureName.Value, new Rectangle(this.Sprite.SourceRect.X + 10, this.Sprite.SourceRect.Y + 21, 64 /*0x40*/, 21), 42, standingPixel.X, standingPixel.Y - 32 /*0x20*/, 1, standingPixel.Y / 64 /*0x40*/, Color.White);
  }

  public void castTeleport()
  {
    int num = 0;
    Vector2 tile = this.Tile;
    Vector2 vector2;
    for (vector2 = new Vector2(tile.X + (Game1.random.NextBool() ? (float) Game1.random.Next(-5, -1) : (float) Game1.random.Next(2, 6)), tile.Y + (Game1.random.NextBool() ? (float) Game1.random.Next(-5, -1) : (float) Game1.random.Next(2, 6))); num < 6 && (!this.currentLocation.isTileOnMap(vector2) || !this.currentLocation.isTileLocationOpen(vector2) || !this.currentLocation.CanSpawnCharacterHere(vector2)); ++num)
      vector2 = new Vector2(tile.X + (Game1.random.NextBool() ? (float) Game1.random.Next(-5, -1) : (float) Game1.random.Next(2, 6)), tile.Y + (Game1.random.NextBool() ? (float) Game1.random.Next(-5, -1) : (float) Game1.random.Next(2, 6)));
    if (num >= 6)
      return;
    this.teleporting = true;
    this.teleportationPath = Utility.GetPointsOnLine((int) tile.X, (int) tile.Y, (int) vector2.X, (int) vector2.Y, true).GetEnumerator();
    this.coolDown = 20;
  }

  public override void behaviorAtGameTick(GameTime time)
  {
    base.behaviorAtGameTick(time);
    if ((double) this.timeBeforeAIMovementAgain <= 0.0)
      this.IsInvisible = false;
    if (this.teleporting)
    {
      this.coolDown -= time.ElapsedGameTime.Milliseconds;
      if (this.coolDown > 0)
        return;
      if (this.teleportationPath.MoveNext())
      {
        Game1.multiplayer.broadcastSprites(this.currentLocation, new TemporaryAnimatedSprite(this.Sprite.textureName.Value, this.Sprite.SourceRect, this.Position, false, 0.04f, Color.White));
        this.Position = new Vector2((float) (this.teleportationPath.Current.X * 64 /*0x40*/ + 4), (float) (this.teleportationPath.Current.Y * 64 /*0x40*/ - 32 /*0x20*/ - 4));
        this.coolDown = 20;
      }
      else
      {
        this.teleporting = false;
        this.coolDown = 500;
      }
    }
    else if (!this.spottedPlayer && Utility.couldSeePlayerInPeripheralVision(this.Player, (Character) this) && Utility.doesPointHaveLineOfSightInMine(this.currentLocation, this.Tile, this.Player.Tile, 8))
    {
      this.controller = (PathFindController) null;
      this.spottedPlayer = true;
      this.Halt();
      this.facePlayer(this.Player);
      if (Game1.random.NextDouble() >= 0.3)
        return;
      this.currentLocation.playSound("shadowpeep");
    }
    else if (this.casting)
    {
      this.Halt();
      this.IsWalkingTowardPlayer = false;
      TimeSpan timeSpan = time.TotalGameTime;
      this.rotationTimer = (float) ((double) timeSpan.Milliseconds * 0.024543693289160728 / 24.0 % (1024.0 * Math.PI));
      int coolDown = this.coolDown;
      timeSpan = time.ElapsedGameTime;
      int milliseconds = timeSpan.Milliseconds;
      this.coolDown = coolDown - milliseconds;
      if (this.coolDown > 0)
        return;
      Rectangle boundingBox = this.GetBoundingBox();
      this.Scale = 1f;
      Vector2 velocityTowardPlayer = Utility.getVelocityTowardPlayer(boundingBox.Center, 15f, this.Player);
      if (this.Player.Attack >= 0 && Game1.random.NextDouble() < 0.6)
      {
        this.currentLocation.projectiles.Add((Projectile) new DebuffingProjectile("18", 2, 4, 4, 0.196349546f, velocityTowardPlayer.X, velocityTowardPlayer.Y, new Vector2((float) boundingBox.X, (float) boundingBox.Y)));
      }
      else
      {
        this.currentLocation.playSound("fireball");
        this.currentLocation.projectiles.Add((Projectile) new BasicProjectile(10, 3, 0, 3, 0.0f, velocityTowardPlayer.X, velocityTowardPlayer.Y, new Vector2((float) boundingBox.X, (float) boundingBox.Y)));
      }
      this.casting = false;
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
          this.casting = true;
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
