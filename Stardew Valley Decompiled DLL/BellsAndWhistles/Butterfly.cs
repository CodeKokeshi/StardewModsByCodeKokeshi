// Decompiled with JetBrains decompiler
// Type: StardewValley.BellsAndWhistles.Butterfly
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

public class Butterfly : Critter
{
  public const float maxSpeed = 3f;
  private int flapTimer;
  private int flapSpeed = 50;
  private Vector2 motion;
  private float motionMultiplier = 1f;
  private float prismaticCaptureTimer = -1f;
  private float prismaticSprinkleTimer;
  private bool summerButterfly;
  public bool stayInbounds;
  public bool isPrismatic;
  public bool isLit;
  private string lightId;

  public Butterfly(
    GameLocation location,
    Vector2 position,
    bool islandButterfly = false,
    bool forceSummerButterfly = false,
    int baseFrameOverride = -1,
    bool prismatic = false)
  {
    this.position = position * 64f;
    this.startingPosition = this.position;
    this.isPrismatic = prismatic;
    if (location.IsWinterHere())
    {
      this.baseFrame = 397;
      this.isLit = true;
    }
    else if (location.IsSpringHere() && !forceSummerButterfly)
    {
      this.baseFrame = Game1.random.NextBool() ? Game1.random.Next(3) * 3 + 160 /*0xA0*/ : Game1.random.Next(3) * 3 + 180;
    }
    else
    {
      this.baseFrame = Game1.random.NextBool() ? Game1.random.Next(3) * 4 + 128 /*0x80*/ : Game1.random.Next(3) * 4 + 148;
      this.summerButterfly = true;
      if (Game1.random.NextDouble() < 0.05)
        this.baseFrame = Game1.random.Next(2) * 4 + 169;
      if (Game1.random.NextDouble() < 0.01)
        this.baseFrame = Game1.random.Next(2) * 4 + 480;
    }
    if (islandButterfly)
    {
      this.baseFrame = Game1.random.Next(4) * 4 + 364;
      this.summerButterfly = true;
    }
    if (baseFrameOverride != -1)
    {
      this.baseFrame = baseFrameOverride;
      this.summerButterfly = false;
      this.isLit = false;
    }
    this.motion = new Vector2((float) ((Game1.random.NextDouble() + 0.25) * 3.0 * (double) Game1.random.Choose<int>(-1, 1) / 2.0), (float) ((Game1.random.NextDouble() + 0.5) * 3.0 * (double) Game1.random.Choose<int>(-1, 1) / 2.0));
    this.flapSpeed = Game1.random.Next(45, 80 /*0x50*/);
    this.sprite = new AnimatedSprite(Critter.critterTexture, this.baseFrame, 16 /*0x10*/, 16 /*0x10*/);
    this.sprite.loop = false;
    this.startingPosition = position;
    if (!this.isLit)
      return;
    this.lightId = this.GenerateLightSourceId(Game1.random.Next());
    Game1.currentLightSources.Add(new LightSource(this.lightId, 10, position + new Vector2(-30.72f, -93.44f), 0.66f, Color.Black * 0.75f, onlyLocation: location.NameOrUniqueName));
  }

  public void doneWithFlap(Farmer who) => this.flapTimer = 200 + Game1.random.Next(-5, 6);

  public Butterfly setStayInbounds(bool stayInbounds)
  {
    this.stayInbounds = stayInbounds;
    return this;
  }

  public override bool update(GameTime time, GameLocation environment)
  {
    this.flapTimer -= time.ElapsedGameTime.Milliseconds;
    if (this.flapTimer <= 0 && this.sprite.CurrentAnimation == null)
    {
      this.motionMultiplier = 1f;
      this.motion.X += (float) Game1.random.Next(-80, 81) / 100f;
      this.motion.Y = (float) ((Game1.random.NextDouble() + 0.25) * -3.0 / 2.0);
      if ((double) Math.Abs(this.motion.X) > 1.5)
        this.motion.X = (float) (3.0 * (double) Math.Sign(this.motion.X) / 2.0);
      if ((double) Math.Abs(this.motion.Y) > 3.0)
        this.motion.Y = 3f * (float) Math.Sign(this.motion.Y);
      if (this.stayInbounds)
      {
        if ((double) this.position.X < 128.0)
          this.motion.X = 0.8f;
        if ((double) this.position.Y < 192.0)
        {
          this.motion.Y /= 2f;
          this.flapTimer = 1000;
        }
        if ((double) this.position.X > (double) (environment.map.DisplayWidth - 128 /*0x80*/))
          this.motion.X = -0.8f;
        if ((double) this.position.Y > (double) (environment.map.DisplayHeight - 128 /*0x80*/))
        {
          this.motion.Y = -1f;
          this.flapTimer = 100;
        }
      }
      if (this.summerButterfly)
        this.sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
        {
          new FarmerSprite.AnimationFrame(this.baseFrame + 1, this.flapSpeed),
          new FarmerSprite.AnimationFrame(this.baseFrame + 2, this.flapSpeed),
          new FarmerSprite.AnimationFrame(this.baseFrame + 3, this.flapSpeed),
          new FarmerSprite.AnimationFrame(this.baseFrame + 2, this.flapSpeed),
          new FarmerSprite.AnimationFrame(this.baseFrame + 1, this.flapSpeed),
          new FarmerSprite.AnimationFrame(this.baseFrame, this.flapSpeed, false, false, new AnimatedSprite.endOfAnimationBehavior(this.doneWithFlap))
        });
      else
        this.sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
        {
          new FarmerSprite.AnimationFrame(this.baseFrame + 1, this.flapSpeed),
          new FarmerSprite.AnimationFrame(this.baseFrame + 2, this.flapSpeed),
          new FarmerSprite.AnimationFrame(this.baseFrame + 1, this.flapSpeed),
          new FarmerSprite.AnimationFrame(this.baseFrame, this.flapSpeed, false, false, new AnimatedSprite.endOfAnimationBehavior(this.doneWithFlap))
        });
      if (this.isPrismatic && (double) this.prismaticCaptureTimer < 0.0)
        Utility.makeTemporarySpriteJuicier(new TemporaryAnimatedSprite("LooseSprites\\Cursors_1_6", new Rectangle(144 /*0x90*/, 249, 7, 7), (float) Game1.random.Next(100, 200), 6, 1, this.position + new Vector2((float) (Game1.random.Next(-32, 32 /*0x20*/) - 48 /*0x30*/), (float) (Game1.random.Next(-32, 32 /*0x20*/) - 96 /*0x60*/)), false, false, Math.Max(0.0f, (float) (((double) this.position.Y + 64.0 - 24.0) / 10000.0)) + (float) ((double) this.position.X / 64.0 * 9.9999997473787516E-06), 0.0f, Utility.GetPrismaticColor(Game1.random.Next(7), 10f), 4f, 0.0f, 0.0f, 0.0f)
        {
          drawAboveAlwaysFront = true
        }, environment);
    }
    if ((double) this.prismaticCaptureTimer > 0.0)
    {
      this.motion = Game1.player.position.Value + new Vector2(64f, -32f) - this.position;
      this.motion *= 0.1f;
      this.prismaticCaptureTimer -= (float) Game1.currentGameTime.ElapsedGameTime.TotalMilliseconds;
      this.position = this.position + this.motion;
      this.position = this.position + new Vector2((float) (Math.Cos(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 100.0) * ((double) this.prismaticCaptureTimer / 150.0)), (float) (Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 100.0) * ((double) this.prismaticCaptureTimer / 150.0)));
      this.prismaticSprinkleTimer -= (float) Game1.currentGameTime.ElapsedGameTime.TotalMilliseconds;
      if ((double) this.prismaticSprinkleTimer <= 0.0)
      {
        environment.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors_1_6", new Rectangle(144 /*0x90*/, 249, 7, 7), (float) Game1.random.Next(100, 200), 6, 1, this.position + new Vector2(-48f, -96f), false, false, Math.Max(0.0f, (float) (((double) this.position.Y + 64.0 - 24.0) / 10000.0)) + (float) ((double) this.position.X / 64.0 * 9.9999997473787516E-06), 0.0f, Utility.GetPrismaticColor(Game1.random.Next(7), 10f), 4f, 0.0f, 0.0f, 0.0f)
        {
          drawAboveAlwaysFront = true
        });
        this.prismaticSprinkleTimer = 80f;
      }
      if ((double) this.prismaticCaptureTimer <= 0.0)
      {
        Utility.makeTemporarySpriteJuicier(new TemporaryAnimatedSprite("LooseSprites\\Cursors_1_6", new Rectangle(144 /*0x90*/, 249, 7, 7), (float) Game1.random.Next(100, 200), 6, 1, this.position + new Vector2(-48f, -96f), false, false, Math.Max(0.0f, (float) (((double) this.position.Y + 64.0 - 24.0) / 10000.0)) + (float) ((double) this.position.X / 64.0 * 9.9999997473787516E-06), 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
        {
          drawAboveAlwaysFront = true
        }, environment, 16 /*0x10*/);
        Game1.playSound("yoba");
        Game1.player.buffs.Remove("statue_of_blessings_6");
        if (Utility.CreateDaySaveRandom((double) (Game1.player.UniqueMultiplayerID % 10000L)).NextDouble() < 0.05000000074505806 + Game1.player.DailyLuck)
          Game1.createItemDebris(ItemRegistry.Create("(O)74"), this.position + new Vector2(-48f, -96f), 2, environment, (int) Game1.player.position.Y);
        Game1.player.Money += Math.Max(100, Math.Min(50000, (int) ((double) Game1.player.totalMoneyEarned * 0.004999999888241291)));
        return true;
      }
    }
    else
    {
      this.position = this.position + this.motion * this.motionMultiplier;
      this.motion.Y += 0.005f * (float) time.ElapsedGameTime.Milliseconds;
      this.motionMultiplier -= 0.0005f * (float) time.ElapsedGameTime.Milliseconds;
      if ((double) this.motionMultiplier <= 0.0)
        this.motionMultiplier = 0.0f;
    }
    if (this.isPrismatic && (double) this.prismaticCaptureTimer < 0.0 && (double) Utility.distance(this.position.X, Game1.player.position.X, this.position.Y, Game1.player.position.Y) < 128.0)
      this.prismaticCaptureTimer = 2000f;
    if (this.isLit)
      Utility.repositionLightSource(this.lightId, this.position + new Vector2(-30.72f, -93.44f));
    return base.update(time, environment);
  }

  public override void draw(SpriteBatch b)
  {
  }

  public override void drawAboveFrontLayer(SpriteBatch b)
  {
    this.sprite.draw(b, Game1.GlobalToLocal(Game1.viewport, this.position + new Vector2(-64f, this.yJumpOffset - 128f + this.yOffset)), this.position.Y / 10000f, 0, 0, this.isPrismatic ? Utility.GetPrismaticColor(speedMultiplier: 10f) : Color.White, this.flip, 4f);
  }
}
