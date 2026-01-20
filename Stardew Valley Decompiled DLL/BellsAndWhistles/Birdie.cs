// Decompiled with JetBrains decompiler
// Type: StardewValley.BellsAndWhistles.Birdie
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Extensions;
using System;
using System.Collections.Generic;

#nullable disable
namespace StardewValley.BellsAndWhistles;

public class Birdie : Critter
{
  public const int brownBird = 25;
  public const int blueBird = 45;
  public const int flyingSpeed = 6;
  public const int walkingSpeed = 1;
  public const int pecking = 0;
  public const int flyingAway = 1;
  public const int sleeping = 2;
  public const int stopped = 3;
  public const int walking = 4;
  private int state;
  private float flightOffset;
  private bool stationary;
  private int characterCheckTimer = 200;
  private int walkTimer;

  public Birdie(int tileX, int tileY, int startingIndex = 25)
    : base(startingIndex, new Vector2((float) (tileX * 64 /*0x40*/), (float) (tileY * 64 /*0x40*/)))
  {
    this.flip = Game1.random.NextBool();
    this.position.X += 32f;
    this.position.Y += 32f;
    this.startingPosition = this.position;
    this.flightOffset = (float) Game1.random.NextDouble() - 0.5f;
    this.state = 0;
  }

  public Birdie(Vector2 position, float yOffset, int startingIndex = 25, bool stationary = false)
    : base(startingIndex, position)
  {
    this.yOffset = yOffset;
    this.flip = Game1.random.NextBool();
    this.startingPosition = position;
    this.stationary = stationary;
    this.state = Game1.random.Next(2, 5);
    this.flightOffset = (float) Game1.random.NextDouble() - 0.5f;
  }

  public void hop(Farmer who) => this.gravityAffectedDY = -2f;

  public override void drawAboveFrontLayer(SpriteBatch b)
  {
    if (this.state != 1)
      return;
    base.draw(b);
  }

  public override void draw(SpriteBatch b)
  {
    if (this.state == 1)
      return;
    base.draw(b);
  }

  private void donePecking(Farmer who) => this.state = Game1.random.Choose<int>(0, 3);

  private void playFlap(Farmer who)
  {
    if (!Utility.isOnScreen(this.position, 64 /*0x40*/))
      return;
    Game1.playSound("batFlap");
  }

  private void playPeck(Farmer who)
  {
    if (!Utility.isOnScreen(this.position, 64 /*0x40*/))
      return;
    Game1.playSound("shiny4");
  }

  public override bool update(GameTime time, GameLocation environment)
  {
    if ((double) this.yJumpOffset < 0.0 && this.state != 1 && !this.stationary)
    {
      if (!this.flip && !environment.isCollidingPosition(this.getBoundingBox(-2, 0), Game1.viewport, false, 0, false, (Character) null, false, ignoreCharacterRequirement: true))
        this.position.X -= 2f;
      else if (!environment.isCollidingPosition(this.getBoundingBox(2, 0), Game1.viewport, false, 0, false, (Character) null, false, ignoreCharacterRequirement: true))
        this.position.X += 2f;
    }
    this.characterCheckTimer -= time.ElapsedGameTime.Milliseconds;
    if (this.characterCheckTimer < 0)
    {
      Character character = Utility.isThereAFarmerOrCharacterWithinDistance(this.position / 64f, 4, environment);
      this.characterCheckTimer = 200;
      if (character != null && this.state != 1)
      {
        if (Game1.random.NextDouble() < 0.85)
          Game1.playSound("SpringBirds");
        this.state = 1;
        if ((double) character.Position.X > (double) this.position.X)
          this.flip = false;
        else
          this.flip = true;
        this.sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
        {
          new FarmerSprite.AnimationFrame((int) (short) (this.baseFrame + 6), 70),
          new FarmerSprite.AnimationFrame((int) (short) (this.baseFrame + 7), 60, false, this.flip, new AnimatedSprite.endOfAnimationBehavior(this.playFlap)),
          new FarmerSprite.AnimationFrame((int) (short) (this.baseFrame + 8), 70),
          new FarmerSprite.AnimationFrame((int) (short) (this.baseFrame + 7), 60)
        });
        this.sprite.loop = true;
      }
    }
    switch (this.state)
    {
      case 0:
        if (this.sprite.CurrentAnimation == null)
        {
          List<FarmerSprite.AnimationFrame> animation = new List<FarmerSprite.AnimationFrame>()
          {
            new FarmerSprite.AnimationFrame((int) (short) (this.baseFrame + 2), 480),
            new FarmerSprite.AnimationFrame((int) (short) (this.baseFrame + 3), 170, false, this.flip),
            new FarmerSprite.AnimationFrame((int) (short) (this.baseFrame + 4), 170, false, this.flip)
          };
          int num = Game1.random.Next(1, 5);
          for (int index = 0; index < num; ++index)
          {
            animation.Add(new FarmerSprite.AnimationFrame((int) (short) (this.baseFrame + 3), 70));
            animation.Add(new FarmerSprite.AnimationFrame((int) (short) (this.baseFrame + 4), 100, false, this.flip, new AnimatedSprite.endOfAnimationBehavior(this.playPeck)));
          }
          animation.Add(new FarmerSprite.AnimationFrame((int) (short) (this.baseFrame + 3), 100));
          animation.Add(new FarmerSprite.AnimationFrame((int) (short) (this.baseFrame + 2), 70, false, this.flip));
          animation.Add(new FarmerSprite.AnimationFrame((int) (short) (this.baseFrame + 1), 70, false, this.flip));
          animation.Add(new FarmerSprite.AnimationFrame((int) (short) this.baseFrame, 500, false, this.flip, new AnimatedSprite.endOfAnimationBehavior(this.donePecking)));
          this.sprite.loop = false;
          this.sprite.setCurrentAnimation(animation);
          break;
        }
        break;
      case 1:
        if (!this.flip)
          this.position.X -= 6f;
        else
          this.position.X += 6f;
        this.yOffset -= 2f + this.flightOffset;
        break;
      case 2:
        if (this.sprite.CurrentAnimation == null)
          this.sprite.currentFrame = this.baseFrame + 5;
        if (Game1.random.NextDouble() < 0.003 && this.sprite.CurrentAnimation == null)
        {
          this.state = 3;
          break;
        }
        break;
      case 3:
        if (Game1.random.NextDouble() < 0.008 && this.sprite.CurrentAnimation == null && (double) this.yJumpOffset >= 0.0)
        {
          switch (Game1.random.Next(6))
          {
            case 0:
              this.state = 2;
              break;
            case 1:
              this.state = 0;
              break;
            case 2:
              this.hop((Farmer) null);
              break;
            case 3:
              this.flip = !this.flip;
              this.hop((Farmer) null);
              break;
            case 4:
            case 5:
              this.state = 4;
              this.sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
              {
                new FarmerSprite.AnimationFrame((int) (short) this.baseFrame, 100),
                new FarmerSprite.AnimationFrame((int) (short) (this.baseFrame + 1), 100)
              });
              this.sprite.loop = true;
              if ((double) this.position.X >= (double) this.startingPosition.X)
                this.flip = false;
              else
                this.flip = true;
              this.walkTimer = Game1.random.Next(5, 15) * 100;
              break;
          }
        }
        else
        {
          if (this.sprite.CurrentAnimation == null)
          {
            this.sprite.currentFrame = this.baseFrame;
            break;
          }
          break;
        }
        break;
      case 4:
        if (!this.stationary)
        {
          int xOffset = this.flip ? 1 : -1;
          if (!environment.isCollidingPosition(this.getBoundingBox(xOffset, 0), Game1.viewport, false, 0, false, (Character) null, false, ignoreCharacterRequirement: true))
            this.position.X += (float) xOffset;
        }
        else
        {
          float num = this.flip ? 0.5f : -0.5f;
          if ((double) Math.Abs(this.position.X + num - this.startingPosition.X) < 8.0)
            this.position.X += num;
          else
            this.flip = !this.flip;
        }
        this.walkTimer -= time.ElapsedGameTime.Milliseconds;
        if (this.walkTimer < 0)
        {
          this.state = 3;
          this.sprite.loop = false;
          this.sprite.CurrentAnimation = (List<FarmerSprite.AnimationFrame>) null;
          this.sprite.currentFrame = this.baseFrame;
          break;
        }
        break;
    }
    return base.update(time, environment);
  }
}
