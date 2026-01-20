// Decompiled with JetBrains decompiler
// Type: StardewValley.Monsters.RockGolem
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Netcode;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley.Pathfinding;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley.Monsters;

public class RockGolem : Monster
{
  [XmlIgnore]
  public readonly NetBool seenPlayer = new NetBool();

  public RockGolem()
  {
  }

  public RockGolem(Vector2 position)
    : base("Stone Golem", position)
  {
    this.IsWalkingTowardPlayer = false;
    this.Slipperiness = 2;
    this.jitteriness.Value = 0.0;
    this.HideShadow = true;
  }

  public RockGolem(Vector2 position, MineShaft mineArea)
    : this(position)
  {
    int mineLevel = mineArea.mineLevel;
    if (mineLevel > 80 /*0x50*/)
    {
      this.DamageToFarmer *= 2;
      this.Health = (int) ((double) this.Health * 2.5);
    }
    else
    {
      if (mineLevel <= 40)
        return;
      this.DamageToFarmer = (int) ((double) this.DamageToFarmer * 1.5);
      this.Health = (int) ((double) this.Health * 1.75);
    }
  }

  /// <summary>
  /// constructor for wilderness golems that spawn on combat farm.
  /// </summary>
  /// <param name="position"></param>
  /// <param name="difficultyMod">player combat level is good</param>
  public RockGolem(Vector2 position, int difficultyMod)
    : base(difficultyMod < 9 || Game1.random.NextDouble() >= 0.5 || Game1.whichFarm != 4 ? "Wilderness Golem" : "Iridium Golem", position)
  {
    this.IsWalkingTowardPlayer = false;
    this.Slipperiness = 3;
    this.HideShadow = true;
    this.jitteriness.Value = 0.0;
    this.DamageToFarmer += difficultyMod;
    this.Health += (int) ((double) (difficultyMod * difficultyMod) * 2.0);
    this.ExperienceGained += difficultyMod;
    if (difficultyMod >= 5 && Game1.random.NextDouble() < 0.05)
      this.objectsToDrop.Add("749");
    if (difficultyMod >= 5 && Game1.random.NextDouble() < 0.2)
      this.objectsToDrop.Add("770");
    if (difficultyMod >= 10 && Game1.random.NextDouble() < 0.01)
      this.objectsToDrop.Add("386");
    if (difficultyMod >= 10 && Game1.random.NextDouble() < 0.01)
      this.objectsToDrop.Add("386");
    if (difficultyMod >= 10 && Game1.random.NextDouble() < 0.001)
      this.objectsToDrop.Add("74");
    if (this.name.Value == "Iridium Golem")
    {
      this.Speed *= 2;
      this.Health += 400;
      this.DamageToFarmer += 10;
      this.ExperienceGained += 10;
      if (Game1.random.NextDouble() < 0.03)
        this.objectsToDrop.Add("337");
      if (Game1.random.NextDouble() < 0.03)
        this.objectsToDrop.Add("337");
    }
    this.Sprite.currentFrame = 16 /*0x10*/;
    this.Sprite.loop = false;
    this.Sprite.UpdateSourceRect();
  }

  public RockGolem(Vector2 position, bool alreadySpawned)
    : base("Stone Golem", position)
  {
    if (alreadySpawned)
    {
      this.IsWalkingTowardPlayer = true;
      this.seenPlayer.Value = true;
      this.moveTowardPlayerThreshold.Value = 16 /*0x10*/;
    }
    else
      this.IsWalkingTowardPlayer = false;
    this.Sprite.loop = false;
    this.Slipperiness = 2;
  }

  protected override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.seenPlayer, "seenPlayer");
    this.position.Field.AxisAlignedMovement = true;
  }

  public override List<Item> getExtraDropItems()
  {
    if (this.name.Equals((object) "Wilderness Golem"))
    {
      if (Game1.random.NextDouble() <= 0.0001)
        return new List<Item>()
        {
          ItemRegistry.Create("(H)40")
        };
      if (Game1.IsSpring && Game1.random.NextDouble() < 33.0 / 400.0)
      {
        List<Item> extraDropItems = new List<Item>();
        int num = Game1.random.Next(2, 6);
        for (int index = 0; index < num; ++index)
          extraDropItems.Add(ItemRegistry.Create("(O)273"));
        return extraDropItems;
      }
    }
    else if (this.name.Equals((object) "Iridium Golem"))
    {
      List<Item> extraDropItems = new List<Item>();
      while (Game1.random.NextDouble() < 0.5)
        extraDropItems.Add(Utility.getRaccoonSeedForCurrentTimeOfYear(Game1.player, Game1.random, 1));
      while (Game1.random.NextDouble() < 0.2)
        extraDropItems.Add(ItemRegistry.Create("(O)386"));
      if (Game1.random.NextDouble() < 0.01)
        extraDropItems.Add(ItemRegistry.Create("(O)SkillBook_" + Game1.random.Next(5).ToString()));
      if (Game1.random.NextDouble() < 0.001)
        extraDropItems.Add((Item) ItemRegistry.Create<Ring>("(O)527"));
      if (Game1.random.NextDouble() <= 0.0002)
        extraDropItems.Add(ItemRegistry.Create("(H)40"));
      return extraDropItems;
    }
    return base.getExtraDropItems();
  }

  public override void BuffForAdditionalDifficulty(int additional_difficulty)
  {
    base.BuffForAdditionalDifficulty(additional_difficulty);
    this.resilience.Value *= 2;
    ++this.Speed;
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
    this.focusedOnFarmers = true;
    this.IsWalkingTowardPlayer = true;
    if (Game1.random.NextDouble() < this.missChance.Value - this.missChance.Value * addedPrecision)
    {
      damage1 = -1;
    }
    else
    {
      this.Health -= damage1;
      this.setTrajectory(xTrajectory, yTrajectory);
      if (this.Health <= 0)
        this.deathAnimation();
      else
        this.currentLocation.playSound("rockGolemHit");
      this.currentLocation.playSound("hitEnemy");
      if (this.name.Value == "Iridium Golem")
        this.Speed = Game1.random.Next(2, 7);
    }
    return damage1;
  }

  protected override void localDeathAnimation()
  {
    this.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(46, this.Position, Color.DarkGray, 10));
    this.currentLocation.localSound("rockGolemDie");
  }

  protected override void sharedDeathAnimation()
  {
    Point standingPixel = this.StandingPixel;
    Game1.createRadialDebris(this.currentLocation, this.Sprite.textureName.Value, new Rectangle(0, 576, 64 /*0x40*/, 64 /*0x40*/), 32 /*0x20*/, standingPixel.X, standingPixel.Y, Game1.random.Next(4, 9), this.TilePoint.Y);
  }

  public override void noMovementProgressNearPlayerBehavior()
  {
    if (!this.IsWalkingTowardPlayer)
      return;
    this.Halt();
    this.faceGeneralDirection(this.Player.getStandingPosition());
  }

  public override void behaviorAtGameTick(GameTime time)
  {
    if (this.IsWalkingTowardPlayer)
      base.behaviorAtGameTick(time);
    if (!this.seenPlayer.Value)
    {
      if (this.withinPlayerThreshold())
      {
        this.currentLocation.playSound("rockGolemSpawn");
        this.seenPlayer.Value = true;
      }
      else
      {
        this.Sprite.currentFrame = 16 /*0x10*/;
        this.Sprite.loop = false;
        this.Sprite.UpdateSourceRect();
      }
    }
    else if (this.Sprite.currentFrame >= 16 /*0x10*/)
    {
      this.Sprite.Animate(time, 16 /*0x10*/, 8, 75f);
      if (this.Sprite.currentFrame < 24)
        return;
      this.Sprite.loop = true;
      this.Sprite.currentFrame = 0;
      this.moveTowardPlayerThreshold.Value = 16 /*0x10*/;
      this.IsWalkingTowardPlayer = true;
      this.jitteriness.Value = 0.01;
      if (this.name.Value == "Iridium Golem")
        this.jitteriness.Value += 0.01;
      this.HideShadow = false;
    }
    else
    {
      if (!this.IsWalkingTowardPlayer || Game1.random.NextDouble() >= 0.001 || !Utility.isOnScreen(this.getStandingPosition(), 0))
        return;
      this.controller = new PathFindController((Character) this, this.currentLocation, new Point(this.Player.TilePoint.X, this.Player.TilePoint.Y), -1, (PathFindController.endBehavior) null, 200);
    }
  }

  protected override void updateMonsterSlaveAnimation(GameTime time)
  {
    if (this.IsWalkingTowardPlayer)
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
    if (!this.seenPlayer.Value)
    {
      this.Sprite.currentFrame = 16 /*0x10*/;
      this.Sprite.loop = false;
      this.Sprite.UpdateSourceRect();
    }
    else
    {
      if (this.Sprite.currentFrame < 16 /*0x10*/)
        return;
      this.Sprite.Animate(time, 16 /*0x10*/, 8, 75f);
      if (this.Sprite.currentFrame < 24)
        return;
      this.Sprite.loop = true;
      this.Sprite.currentFrame = 0;
      this.Sprite.UpdateSourceRect();
    }
  }
}
