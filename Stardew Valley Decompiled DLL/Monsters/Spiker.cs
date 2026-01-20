// Decompiled with JetBrains decompiler
// Type: StardewValley.Monsters.Spiker
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using System;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley.Monsters;

public class Spiker : Monster
{
  [XmlIgnore]
  public int targetDirection;
  [XmlIgnore]
  public NetBool moving = new NetBool(false);
  protected bool _localMoving;
  [XmlIgnore]
  public float nextMoveCheck;

  public Spiker()
  {
  }

  public Spiker(Vector2 position, int direction)
    : base(nameof (Spiker), position)
  {
    this.Sprite.SpriteWidth = 16 /*0x10*/;
    this.Sprite.SpriteHeight = 16 /*0x10*/;
    this.Sprite.UpdateSourceRect();
    this.targetDirection = direction;
    this.speed = 14;
    this.ignoreMovementAnimations = true;
    this.onCollision = new Monster.collisionBehavior(this.collide);
  }

  protected override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.moving, "moving");
  }

  public override void update(GameTime time, GameLocation location)
  {
    base.update(time, location);
    if (this.moving.Value == this._localMoving)
      return;
    this._localMoving = this.moving.Value;
    if (this._localMoving)
    {
      if (this.currentLocation != Game1.currentLocation || !Utility.isOnScreen(this.Position, 64 /*0x40*/))
        return;
      Game1.playSound("parry");
    }
    else
    {
      if (this.currentLocation != Game1.currentLocation || !Utility.isOnScreen(this.Position, 64 /*0x40*/))
        return;
      Game1.playSound("hammer");
    }
  }

  public override void draw(SpriteBatch b)
  {
    this.Sprite.draw(b, Game1.GlobalToLocal(Game1.viewport, this.Position), (float) this.StandingPixel.Y / 10000f);
  }

  private void collide(GameLocation location)
  {
    Rectangle rectangle = this.nextPosition(this.FacingDirection);
    foreach (Character farmer in location.farmers)
    {
      if (farmer.GetBoundingBox().Intersects(rectangle))
        return;
    }
    if (!this.moving.Value)
      return;
    this.moving.Value = false;
    this.targetDirection = (this.targetDirection + 2) % 4;
    this.nextMoveCheck = 0.75f;
  }

  public override void updateMovement(GameLocation location, GameTime time)
  {
  }

  public override int takeDamage(
    int damage,
    int xTrajectory,
    int yTrajectory,
    bool isBomb,
    double addedPrecision,
    Farmer who)
  {
    return -1;
  }

  public override void behaviorAtGameTick(GameTime time)
  {
    if ((double) this.nextMoveCheck > 0.0)
      this.nextMoveCheck -= (float) time.ElapsedGameTime.TotalSeconds;
    if ((double) this.nextMoveCheck <= 0.0)
    {
      this.nextMoveCheck = 0.25f;
      foreach (Farmer farmer in this.currentLocation.farmers)
      {
        if ((this.targetDirection == 0 || this.targetDirection == 2) && Math.Abs(farmer.TilePoint.X - this.TilePoint.X) <= 1)
        {
          if (this.targetDirection == 0 && (double) farmer.Position.Y < (double) this.Position.Y)
          {
            this.moving.Value = true;
            break;
          }
          if (this.targetDirection == 2 && (double) farmer.Position.Y > (double) this.Position.Y)
          {
            this.moving.Value = true;
            break;
          }
        }
        if ((this.targetDirection == 3 || this.targetDirection == 1) && Math.Abs(farmer.TilePoint.Y - this.TilePoint.Y) <= 1)
        {
          if (this.targetDirection == 3 && (double) farmer.Position.X < (double) this.Position.X)
          {
            this.moving.Value = true;
            break;
          }
          if (this.targetDirection == 1 && (double) farmer.Position.X > (double) this.Position.X)
          {
            this.moving.Value = true;
            break;
          }
        }
      }
    }
    this.moveUp = false;
    this.moveDown = false;
    this.moveLeft = false;
    this.moveRight = false;
    if (this.moving.Value)
    {
      switch (this.targetDirection)
      {
        case 0:
          this.moveUp = true;
          break;
        case 1:
          this.moveRight = true;
          break;
        case 2:
          this.moveDown = true;
          break;
        case 3:
          this.moveLeft = true;
          break;
      }
      this.MovePosition(time, Game1.viewport, this.currentLocation);
    }
    this.faceDirection(2);
  }
}
