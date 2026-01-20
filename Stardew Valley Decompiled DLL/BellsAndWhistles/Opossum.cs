// Decompiled with JetBrains decompiler
// Type: StardewValley.BellsAndWhistles.Opossum
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;

#nullable disable
namespace StardewValley.BellsAndWhistles;

public class Opossum : Critter
{
  private int characterCheckTimer = 1500;
  private bool running;
  private int jumpTimer = -1;

  public Opossum(GameLocation location, Vector2 position, bool flip)
  {
    this.characterCheckTimer = Game1.random.Next(500, 3000);
    this.position = position * 64f;
    position.Y += 48f;
    this.flip = flip;
    this.baseFrame = 150;
    this.sprite = new AnimatedSprite(Critter.critterTexture, 150, 32 /*0x20*/, 32 /*0x20*/);
    this.sprite.loop = true;
    this.sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
    {
      new FarmerSprite.AnimationFrame(this.baseFrame, 500),
      new FarmerSprite.AnimationFrame(this.baseFrame + 1, 50),
      new FarmerSprite.AnimationFrame(this.baseFrame + 2, 500),
      new FarmerSprite.AnimationFrame(this.baseFrame + 1, 50),
      new FarmerSprite.AnimationFrame(this.baseFrame, 1000),
      new FarmerSprite.AnimationFrame(this.baseFrame + 1, 50),
      new FarmerSprite.AnimationFrame(this.baseFrame + 2, 700),
      new FarmerSprite.AnimationFrame(this.baseFrame + 1, 50)
    });
    this.startingPosition = position;
  }

  public override bool update(GameTime time, GameLocation environment)
  {
    this.characterCheckTimer -= (int) time.ElapsedGameTime.TotalMilliseconds;
    if (Utility.isThereAFarmerOrCharacterWithinDistance(this.position / 64f, 8, environment) != null)
      this.characterCheckTimer = 0;
    if (this.jumpTimer > -1)
    {
      this.jumpTimer -= (int) time.ElapsedGameTime.TotalMilliseconds;
      this.yJumpOffset = (float) (-Math.Sin((600.0 - (double) this.jumpTimer) / 600.0 * Math.PI) * 4.0 * 16.0);
      if (this.jumpTimer <= -1)
      {
        this.running = true;
        this.sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
        {
          new FarmerSprite.AnimationFrame(this.baseFrame + 5, 40),
          new FarmerSprite.AnimationFrame(this.baseFrame + 6, 40),
          new FarmerSprite.AnimationFrame(this.baseFrame + 7, 40),
          new FarmerSprite.AnimationFrame(this.baseFrame + 8, 40)
        });
        this.sprite.loop = true;
      }
    }
    else if (this.characterCheckTimer <= 0 && !this.running)
    {
      if (Utility.isOnScreen(this.position, -32) && this.jumpTimer == -1)
      {
        this.jumpTimer = 600;
        this.sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
        {
          new FarmerSprite.AnimationFrame(this.baseFrame + 4, 20)
        });
      }
      this.characterCheckTimer = 200;
    }
    if (this.running)
      this.position.X += this.flip ? -6f : 6f;
    if (this.running && this.characterCheckTimer <= 0)
    {
      this.characterCheckTimer = 200;
      if (environment.largeTerrainFeatures != null)
      {
        Rectangle rectangle = new Rectangle((int) this.position.X + 32 /*0x20*/, (int) this.position.Y - 32 /*0x20*/, 4, 192 /*0xC0*/);
        foreach (LargeTerrainFeature largeTerrainFeature in environment.largeTerrainFeatures)
        {
          if (largeTerrainFeature is Bush bush && largeTerrainFeature.getBoundingBox().Intersects(rectangle))
          {
            bush.performUseAction(largeTerrainFeature.Tile);
            return true;
          }
        }
      }
    }
    return base.update(time, environment);
  }
}
