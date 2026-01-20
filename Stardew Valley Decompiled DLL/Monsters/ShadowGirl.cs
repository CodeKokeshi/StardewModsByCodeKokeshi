// Decompiled with JetBrains decompiler
// Type: StardewValley.Monsters.ShadowGirl
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using StardewValley.Extensions;
using StardewValley.Pathfinding;
using System;
using System.Xml.Serialization;
using xTile.Layers;

#nullable disable
namespace StardewValley.Monsters;

public class ShadowGirl : Monster
{
  public const int blockTimeBeforePathfinding = 500;
  [XmlIgnore]
  public new Vector2 lastPosition = Vector2.Zero;
  [XmlIgnore]
  public int howLongOnThisPosition;

  public ShadowGirl()
  {
  }

  public ShadowGirl(Vector2 position)
    : base("Shadow Girl", position)
  {
    this.IsWalkingTowardPlayer = false;
    this.moveTowardPlayerThreshold.Value = 8;
    Friendship friendship;
    if (!Game1.MasterPlayer.friendshipData.TryGetValue("???", out friendship) || friendship.Points < 1250)
      return;
    this.DamageToFarmer = 0;
  }

  /// <inheritdoc />
  public override void reloadSprite(bool onlyAppearance = false)
  {
    this.Sprite = new AnimatedSprite("Characters\\Monsters\\Shadow Girl");
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
      this.setTrajectory(xTrajectory, yTrajectory);
      if (this.Health <= 0)
        this.deathAnimation();
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

  public override void update(GameTime time, GameLocation location)
  {
    if (!location.farmers.Any())
      return;
    if (!this.Player.isRafting || !this.withinPlayerThreshold(4))
    {
      this.updateGlow();
      this.updateEmote(time);
      if (this.controller == null)
        this.updateMovement(location, time);
      if (this.controller != null && this.controller.update(time))
        this.controller = (PathFindController) null;
    }
    this.behaviorAtGameTick(time);
    Layer layer = location.map.RequireLayer("Back");
    if ((double) this.Position.X >= 0.0 && (double) this.Position.X <= (double) (layer.LayerWidth * 64 /*0x40*/) && (double) this.Position.Y >= 0.0 && (double) this.Position.Y <= (double) (layer.LayerHeight * 64 /*0x40*/))
      return;
    location.characters.Remove((NPC) this);
  }

  public override void behaviorAtGameTick(GameTime time)
  {
    base.behaviorAtGameTick(time);
    this.addedSpeed = 0.0f;
    this.speed = 3;
    if (this.howLongOnThisPosition > 500 && this.controller == null)
    {
      this.IsWalkingTowardPlayer = false;
      this.controller = new PathFindController((Character) this, this.currentLocation, new Point(this.Player.TilePoint.X, this.Player.TilePoint.Y), Game1.random.Next(4), (PathFindController.endBehavior) null, 300);
      this.timeBeforeAIMovementAgain = 2000f;
      this.howLongOnThisPosition = 0;
    }
    else if (this.controller == null)
      this.IsWalkingTowardPlayer = true;
    if (this.Position.Equals(this.lastPosition))
      this.howLongOnThisPosition += time.ElapsedGameTime.Milliseconds;
    else
      this.howLongOnThisPosition = 0;
    this.lastPosition = this.Position;
  }
}
