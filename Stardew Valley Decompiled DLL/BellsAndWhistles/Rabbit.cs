// Decompiled with JetBrains decompiler
// Type: StardewValley.BellsAndWhistles.Rabbit
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using StardewValley.TerrainFeatures;
using System.Collections.Generic;

#nullable disable
namespace StardewValley.BellsAndWhistles;

public class Rabbit : Critter
{
  private int characterCheckTimer = 200;
  private bool running;

  public Rabbit(GameLocation location, Vector2 position, bool flip)
  {
    bool flag = location.IsWinterHere();
    this.position = position * 64f;
    position.Y += 48f;
    this.flip = flip;
    this.baseFrame = flag ? 74 : 54;
    this.sprite = new AnimatedSprite(Critter.critterTexture, flag ? 69 : 68, 32 /*0x20*/, 32 /*0x20*/);
    this.sprite.loop = true;
    this.startingPosition = position;
  }

  public override bool update(GameTime time, GameLocation environment)
  {
    this.characterCheckTimer -= time.ElapsedGameTime.Milliseconds;
    if (this.characterCheckTimer <= 0 && !this.running)
    {
      if (Utility.isOnScreen(this.position, -32))
      {
        this.running = true;
        this.sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
        {
          new FarmerSprite.AnimationFrame(this.baseFrame, 40),
          new FarmerSprite.AnimationFrame(this.baseFrame + 1, 40),
          new FarmerSprite.AnimationFrame(this.baseFrame + 2, 40),
          new FarmerSprite.AnimationFrame(this.baseFrame + 3, 100),
          new FarmerSprite.AnimationFrame(this.baseFrame + 5, 70),
          new FarmerSprite.AnimationFrame(this.baseFrame + 5, 40)
        });
        this.sprite.loop = true;
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
