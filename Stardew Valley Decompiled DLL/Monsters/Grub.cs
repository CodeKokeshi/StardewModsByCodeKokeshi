// Decompiled with JetBrains decompiler
// Type: StardewValley.Monsters.Grub
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Extensions;
using StardewValley.Locations;
using System;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley.Monsters;

public class Grub : Monster
{
  public const int healthToRunAway = 8;
  [XmlIgnore]
  public readonly NetBool leftDrift = new NetBool();
  [XmlIgnore]
  public readonly NetBool pupating = new NetBool();
  [XmlElement("hard")]
  public readonly NetBool hard = new NetBool();
  [XmlIgnore]
  public int metamorphCounter = 2000;
  [XmlIgnore]
  public readonly NetFloat targetRotation = new NetFloat();

  public Grub()
  {
  }

  public Grub(Vector2 position)
    : this(position, false)
  {
  }

  public Grub(Vector2 position, bool hard)
    : base(nameof (Grub), position)
  {
    if (Game1.random.NextBool())
      this.leftDrift.Value = true;
    this.FacingDirection = Game1.random.Next(4);
    this.targetRotation.Value = this.rotation = (float) Game1.random.Next(4) / 3.14159274f;
    this.hard.Value = hard;
    if (!hard)
      return;
    this.DamageToFarmer *= 3;
    this.Health *= 5;
    this.MaxHealth = this.Health;
    this.ExperienceGained *= 3;
    if (Game1.random.NextDouble() >= 0.1)
      return;
    this.objectsToDrop.Add("456");
  }

  protected override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.leftDrift, "leftDrift").AddField((INetSerializable) this.pupating, "pupating").AddField((INetSerializable) this.hard, "hard").AddField((INetSerializable) this.targetRotation, "targetRotation");
    this.position.Field.AxisAlignedMovement = true;
  }

  /// <inheritdoc />
  public override void reloadSprite(bool onlyAppearance = false)
  {
    base.reloadSprite(onlyAppearance);
    this.Sprite.SpriteHeight = 24;
    this.Sprite.UpdateSourceRect();
  }

  public void setHard()
  {
    this.hard.Value = true;
    if (!this.hard.Value)
      return;
    this.DamageToFarmer = 12;
    this.Health = 100;
    this.MaxHealth = this.Health;
    this.ExperienceGained = 10;
    if (Game1.random.NextDouble() >= 0.1)
      return;
    this.objectsToDrop.Add("456");
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
      this.currentLocation.playSound("slimeHit");
      if (this.pupating.Value)
      {
        this.currentLocation.playSound("crafting");
        this.setTrajectory(xTrajectory / 2, yTrajectory / 2);
        return 0;
      }
      this.Slipperiness = 4;
      this.Health -= damage1;
      this.setTrajectory(xTrajectory, yTrajectory);
      if (this.Health <= 0)
      {
        this.currentLocation.playSound("slimedead");
        Utility.makeTemporarySpriteJuicier(new TemporaryAnimatedSprite(44, this.Position, this.isHardModeMonster.Value ? Color.LimeGreen : Color.Orange, 10)
        {
          holdLastFrame = true,
          alphaFade = 0.01f,
          interval = 50f
        }, this.currentLocation);
      }
    }
    return damage1;
  }

  public override void defaultMovementBehavior(GameTime time)
  {
    this.Scale = (float) (1.0 + 0.125 * Math.Sin(time.TotalGameTime.TotalMilliseconds / (500.0 + (double) this.Position.X / 100.0)));
  }

  public override void BuffForAdditionalDifficulty(int additional_difficulty)
  {
    base.BuffForAdditionalDifficulty(additional_difficulty);
    this.rotation = 0.0f;
    this.targetRotation.Value = 0.0f;
  }

  public override void update(GameTime time, GameLocation location)
  {
    if ((this.Health > 8 || this.hard.Value && this.Health >= this.MaxHealth) && !this.pupating.Value)
    {
      base.update(time, location);
    }
    else
    {
      if (this.invincibleCountdown > 0)
      {
        this.invincibleCountdown -= time.ElapsedGameTime.Milliseconds;
        if (this.invincibleCountdown <= 0)
          this.stopGlowing();
      }
      if (Game1.IsMasterGame)
        this.behaviorAtGameTick(time);
      this.updateAnimation(time);
    }
  }

  public override void draw(SpriteBatch b)
  {
    b.Draw(this.Sprite.Texture, this.getLocalPosition(Game1.viewport) + new Vector2((float) (this.Sprite.SpriteWidth * 4 / 2), (float) (this.GetBoundingBox().Height / 2)) + (this.shakeTimer > 0 ? new Vector2((float) Game1.random.Next(-1, 2), (float) Game1.random.Next(-1, 2)) : Vector2.Zero), new Rectangle?(this.Sprite.SourceRect), this.hard.Value ? Color.Lime : Color.White, this.rotation, new Vector2((float) (this.Sprite.SpriteWidth / 2), (float) ((double) this.Sprite.SpriteHeight * 3.0 / 4.0)), Math.Max(0.2f, this.scale.Value) * 4f, this.flip || this.Sprite.CurrentAnimation != null && this.Sprite.CurrentAnimation[this.Sprite.currentAnimationIndex].flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0.0f, this.drawOnTop ? 0.991f : (float) this.StandingPixel.Y / 10000f));
  }

  protected override void updateMonsterSlaveAnimation(GameTime time)
  {
    if (this.pupating.Value)
    {
      this.Scale = (float) (1.0 + Math.Sin((double) time.TotalGameTime.Milliseconds * 0.39269909262657166) / 12.0);
      this.metamorphCounter -= time.ElapsedGameTime.Milliseconds;
    }
    else if (this.Health <= 8 || this.hard.Value && this.Health < this.MaxHealth)
    {
      this.metamorphCounter -= time.ElapsedGameTime.Milliseconds;
      if (this.metamorphCounter > 0)
        return;
      this.Sprite.Animate(time, 16 /*0x10*/, 4, 125f);
      if (this.Sprite.currentFrame != 19)
        return;
      this.metamorphCounter = 4500;
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
      this.rotation = 0.0f;
      this.Scale = 1f;
    }
    else
    {
      if (this.withinPlayerThreshold())
        return;
      this.Halt();
      this.rotation = this.targetRotation.Value;
    }
  }

  public override void behaviorAtGameTick(GameTime time)
  {
    base.behaviorAtGameTick(time);
    if (this.pupating.Value)
    {
      this.Scale = (float) (1.0 + Math.Sin((double) time.TotalGameTime.Milliseconds * 0.39269909262657166) / 12.0);
      this.metamorphCounter -= time.ElapsedGameTime.Milliseconds;
      if (this.metamorphCounter > 0)
        return;
      Point standingPixel = this.StandingPixel;
      this.Health = -500;
      Game1.createRadialDebris(this.currentLocation, this.Sprite.textureName.Value, new Rectangle(208 /*0xD0*/, 424, 32 /*0x20*/, 40), 4, standingPixel.X, standingPixel.Y, 25, this.TilePoint.Y);
      Game1.createRadialDebris(this.currentLocation, this.Sprite.textureName.Value, new Rectangle(208 /*0xD0*/, 424, 32 /*0x20*/, 40), 8, standingPixel.X, standingPixel.Y, 15, this.TilePoint.Y);
      if (this.currentLocation is MineShaft currentLocation)
      {
        NetCollection<NPC> characters = this.currentLocation.characters;
        Fly fly = new Fly(this.Position, this.hard.Value);
        fly.currentLocation = this.currentLocation;
        Monster monster = currentLocation.BuffMonsterIfNecessary((Monster) fly);
        characters.Add((NPC) monster);
      }
      else
      {
        NetCollection<NPC> characters = this.currentLocation.characters;
        Fly fly = new Fly(this.Position, this.hard.Value);
        fly.currentLocation = this.currentLocation;
        characters.Add((NPC) fly);
      }
    }
    else if (this.Health <= this.MaxHealth / 2 - 2 || this.hard.Value && this.Health < this.MaxHealth)
    {
      this.metamorphCounter -= time.ElapsedGameTime.Milliseconds;
      if (this.metamorphCounter <= 0)
      {
        this.Sprite.Animate(time, 16 /*0x10*/, 4, 125f);
        if (this.Sprite.currentFrame != 19)
          return;
        this.pupating.Value = true;
        this.metamorphCounter = 4500;
      }
      else
      {
        Point standingPixel1 = this.StandingPixel;
        Point standingPixel2 = this.Player.StandingPixel;
        if (Math.Abs(standingPixel2.Y - standingPixel1.Y) > 128 /*0x80*/)
        {
          if (standingPixel2.X > standingPixel1.X)
            this.SetMovingLeft(true);
          else
            this.SetMovingRight(true);
        }
        else if (Math.Abs(standingPixel2.X - standingPixel1.X) > 128 /*0x80*/)
        {
          if (standingPixel2.Y > standingPixel1.Y)
            this.SetMovingUp(true);
          else
            this.SetMovingDown(true);
        }
        this.MovePosition(time, Game1.viewport, this.currentLocation);
      }
    }
    else if (this.withinPlayerThreshold())
    {
      this.Scale = 1f;
      this.rotation = 0.0f;
    }
    else
    {
      if (!this.isMoving())
        return;
      this.Halt();
      this.faceDirection(Game1.random.Next(4));
      this.targetRotation.Value = this.rotation = (float) Game1.random.Next(4) / 3.14159274f;
    }
  }
}
