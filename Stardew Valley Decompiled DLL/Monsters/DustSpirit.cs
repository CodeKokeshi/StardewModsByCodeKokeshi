// Decompiled with JetBrains decompiler
// Type: StardewValley.Monsters.DustSpirit
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Pathfinding;
using StardewValley.TerrainFeatures;
using System;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley.Monsters;

public class DustSpirit : Monster
{
  [XmlIgnore]
  public bool seenFarmer;
  [XmlIgnore]
  public bool runningAwayFromFarmer;
  [XmlIgnore]
  public bool chargingFarmer;
  public byte voice;
  [XmlIgnore]
  public ICue meep;

  public DustSpirit()
  {
  }

  public DustSpirit(Vector2 position)
    : base("Dust Spirit", position)
  {
    this.IsWalkingTowardPlayer = false;
    this.Sprite.interval = 45f;
    this.Scale = (float) Game1.random.Next(75, 101) / 100f;
    this.voice = (byte) Game1.random.Next(1, 24);
    this.HideShadow = true;
  }

  public DustSpirit(Vector2 position, bool chargingTowardFarmer)
    : base("Dust Spirit", position)
  {
    this.IsWalkingTowardPlayer = false;
    if (chargingTowardFarmer)
    {
      this.chargingFarmer = true;
      this.seenFarmer = true;
    }
    this.Sprite.interval = 45f;
    this.Scale = (float) Game1.random.Next(75, 101) / 100f;
    this.HideShadow = true;
  }

  public override void draw(SpriteBatch b)
  {
    if (this.IsInvisible || !Utility.isOnScreen(this.Position, 128 /*0x80*/))
      return;
    int y = this.StandingPixel.Y;
    b.Draw(this.Sprite.Texture, this.getLocalPosition(Game1.viewport) + new Vector2((float) (32 /*0x20*/ + (this.shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0)), (float) (64 /*0x40*/ + this.yJumpOffset)), new Rectangle?(this.Sprite.SourceRect), Color.White, this.rotation, new Vector2(8f, 16f), new Vector2(this.scale.Value + (float) Math.Max(-0.1, (double) (this.yJumpOffset + 32 /*0x20*/) / 128.0), this.scale.Value - Math.Max(-0.1f, (float) this.yJumpOffset / 256f)) * 4f, this.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0.0f, this.drawOnTop ? 0.991f : (float) y / 10000f));
    if (this.isGlowing)
      b.Draw(this.Sprite.Texture, this.getLocalPosition(Game1.viewport) + new Vector2(32f, (float) (64 /*0x40*/ + this.yJumpOffset)), new Rectangle?(this.Sprite.SourceRect), this.glowingColor * this.glowingTransparency, this.rotation, new Vector2(8f, 16f), Math.Max(0.2f, this.scale.Value) * 4f, this.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0.0f, this.drawOnTop ? 0.99f : (float) ((double) y / 10000.0 + 1.0 / 1000.0)));
    b.Draw(Game1.shadowTexture, this.getLocalPosition(Game1.viewport) + new Vector2(32f, 80f), new Rectangle?(Game1.shadowTexture.Bounds), Color.White, 0.0f, new Vector2((float) Game1.shadowTexture.Bounds.Center.X, (float) Game1.shadowTexture.Bounds.Center.Y), (float) (4.0 + (double) this.yJumpOffset / 64.0), SpriteEffects.None, (float) (y - 1) / 10000f);
  }

  protected override void sharedDeathAnimation()
  {
  }

  protected override void localDeathAnimation()
  {
    this.currentLocation.localSound("dustMeep");
    this.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(44, this.Position, new Color(50, 50, 80 /*0x50*/), 10));
    this.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(44, this.Position + new Vector2((float) Game1.random.Next(-32, 32 /*0x20*/), (float) Game1.random.Next(-32, 32 /*0x20*/)), new Color(50, 50, 80 /*0x50*/), 10)
    {
      delayBeforeAnimationStart = 150,
      scale = 0.5f
    });
    this.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(44, this.Position + new Vector2((float) Game1.random.Next(-32, 32 /*0x20*/), (float) Game1.random.Next(-32, 32 /*0x20*/)), new Color(50, 50, 80 /*0x50*/), 10)
    {
      delayBeforeAnimationStart = 300,
      scale = 0.5f
    });
    this.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(44, this.Position + new Vector2((float) Game1.random.Next(-32, 32 /*0x20*/), (float) Game1.random.Next(-32, 32 /*0x20*/)), new Color(50, 50, 80 /*0x50*/), 10)
    {
      delayBeforeAnimationStart = 450,
      scale = 0.5f
    });
  }

  public override void shedChunks(int number, float scale)
  {
    Point standingPixel = this.StandingPixel;
    Game1.createRadialDebris(this.currentLocation, this.Sprite.textureName.Value, new Rectangle(0, 16 /*0x10*/, 16 /*0x10*/, 16 /*0x10*/), 8, standingPixel.X, standingPixel.Y, number, this.TilePoint.Y, Color.White, this.Health <= 0 ? 4f : 2f);
  }

  public void offScreenBehavior(Character c, GameLocation l)
  {
  }

  public virtual bool CaughtInWeb()
  {
    TerrainFeature terrainFeature;
    return this.currentLocation != null && this.currentLocation.terrainFeatures.TryGetValue(this.Tile, out terrainFeature) && terrainFeature is Grass grass && grass.grassType.Value == (byte) 6;
  }

  protected override void updateAnimation(GameTime time)
  {
    if (this.yJumpOffset == 0)
    {
      if (this.isHardModeMonster.Value && this.CaughtInWeb())
      {
        this.Sprite.Animate(time, 5, 3, 200f);
        return;
      }
      this.jumpWithoutSound();
      this.yJumpVelocity = (float) Game1.random.Next(50, 70) / 10f;
      if (Game1.random.NextDouble() < 0.1 && (this.meep == null || !this.meep.IsPlaying) && Utility.isOnScreen(this.Position, 64 /*0x40*/) && Game1.currentLocation == this.currentLocation)
        Game1.playSound("dustMeep", (int) this.voice * 100 + Game1.random.Next(-100, 100), out this.meep);
    }
    this.Sprite.AnimateDown(time);
    this.resetAnimationSpeed();
  }

  public override void behaviorAtGameTick(GameTime time)
  {
    base.behaviorAtGameTick(time);
    if (this.yJumpOffset == 0)
    {
      if (Game1.random.NextDouble() < 0.01)
      {
        Vector2 standingPosition = this.getStandingPosition();
        Game1.multiplayer.broadcastSprites(this.currentLocation, new TemporaryAnimatedSprite("TileSheets\\animations", new Rectangle(0, 128 /*0x80*/, 64 /*0x40*/, 64 /*0x40*/), 40f, 4, 0, standingPosition + new Vector2(-21f, 0.0f), false, false)
        {
          layerDepth = (float) (((double) standingPosition.Y - 10.0) / 10000.0)
        });
        foreach (Vector2 adjacentTileLocation in Utility.getAdjacentTileLocations(this.Tile))
        {
          StardewValley.Object @object;
          if (this.currentLocation.objects.TryGetValue(adjacentTileLocation, out @object) && (@object.IsBreakableStone() || @object.IsTwig()))
            this.currentLocation.destroyObject(adjacentTileLocation, (Farmer) null);
        }
        this.yJumpVelocity *= 2f;
      }
      if (!this.chargingFarmer)
        this.xVelocity = (float) Game1.random.Next(-20, 21) / 5f;
    }
    if (this.chargingFarmer)
    {
      this.Slipperiness = 10;
      Vector2 playerTrajectory = Utility.getAwayFromPlayerTrajectory(this.GetBoundingBox(), this.Player);
      this.xVelocity += (float) (-(double) playerTrajectory.X / 150.0 + (Game1.random.NextDouble() < 0.01 ? (double) Game1.random.Next(-50, 50) / 10.0 : 0.0));
      if ((double) Math.Abs(this.xVelocity) > 5.0)
        this.xVelocity = (float) (Math.Sign(this.xVelocity) * 5);
      this.yVelocity += (float) (-(double) playerTrajectory.Y / 150.0 + (Game1.random.NextDouble() < 0.01 ? (double) Game1.random.Next(-50, 50) / 10.0 : 0.0));
      if ((double) Math.Abs(this.yVelocity) > 5.0)
        this.yVelocity = (float) (Math.Sign(this.yVelocity) * 5);
      if (Game1.random.NextDouble() < 0.0001)
      {
        this.controller = new PathFindController((Character) this, this.currentLocation, this.Player.TilePoint, Game1.random.Next(4), (PathFindController.endBehavior) null, 300);
        this.chargingFarmer = false;
      }
      if (!this.isHardModeMonster.Value || !this.CaughtInWeb())
        return;
      this.xVelocity = 0.0f;
      this.yVelocity = 0.0f;
      if (this.shakeTimer > 0 || Game1.random.NextDouble() >= 0.05)
        return;
      this.shakeTimer = 200;
    }
    else if (!this.seenFarmer && Utility.doesPointHaveLineOfSightInMine(this.currentLocation, this.getStandingPosition() / 64f, this.Player.getStandingPosition() / 64f, 8))
      this.seenFarmer = true;
    else if (this.seenFarmer && this.controller == null && !this.runningAwayFromFarmer)
    {
      this.addedSpeed = 2f;
      this.controller = new PathFindController((Character) this, this.currentLocation, new PathFindController.isAtEnd(Utility.isOffScreenEndFunction), -1, new PathFindController.endBehavior(this.offScreenBehavior), 350, Point.Zero);
      this.runningAwayFromFarmer = true;
    }
    else
    {
      if (this.controller != null || !this.runningAwayFromFarmer)
        return;
      this.chargingFarmer = true;
    }
  }
}
