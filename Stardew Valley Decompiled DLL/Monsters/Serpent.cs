// Decompiled with JetBrains decompiler
// Type: StardewValley.Monsters.Serpent
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Extensions;
using StardewValley.Network;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley.Monsters;

public class Serpent : Monster
{
  public const float rotationIncrement = 0.0490873866f;
  [XmlIgnore]
  public int wasHitCounter;
  [XmlIgnore]
  public float targetRotation;
  [XmlIgnore]
  public bool turningRight;
  [XmlIgnore]
  public readonly NetFarmerRef killer = new NetFarmerRef().Delayed(false);
  public List<Vector3> segments = new List<Vector3>();
  public NetInt segmentCount = new NetInt(0);

  public Serpent()
  {
  }

  public Serpent(Vector2 position)
    : base(nameof (Serpent), position)
  {
    this.InitializeAttributes();
  }

  public Serpent(Vector2 position, string name)
    : base(name, position)
  {
    this.InitializeAttributes();
    if (!(name == "Royal Serpent"))
      return;
    this.segmentCount.Value = Game1.random.Next(3, 7);
    if (Game1.random.NextDouble() < 0.1)
      this.segmentCount.Value = Game1.random.Next(5, 10);
    else if (Game1.random.NextDouble() < 0.01)
      this.segmentCount.Value *= 3;
    this.reloadSprite(false);
    this.MaxHealth += this.segmentCount.Value * 50;
    this.Health = this.MaxHealth;
  }

  public virtual void InitializeAttributes()
  {
    this.Slipperiness = 24 + Game1.random.Next(10);
    this.Halt();
    this.IsWalkingTowardPlayer = false;
    this.Sprite.SpriteWidth = 32 /*0x20*/;
    this.Sprite.SpriteHeight = 32 /*0x20*/;
    this.Scale = 0.75f;
    this.HideShadow = true;
  }

  public bool IsRoyalSerpent() => this.segmentCount.Value > 1;

  public override bool TakesDamageFromHitbox(Rectangle area_of_effect)
  {
    if (base.TakesDamageFromHitbox(area_of_effect))
      return true;
    if (this.IsRoyalSerpent())
    {
      Rectangle boundingBox = this.GetBoundingBox();
      Vector2 vector2 = new Vector2((float) boundingBox.X - this.Position.X, (float) boundingBox.Y - this.Position.Y);
      foreach (Vector3 segment in this.segments)
      {
        boundingBox.X = (int) ((double) segment.X + (double) vector2.X);
        boundingBox.Y = (int) ((double) segment.Y + (double) vector2.Y);
        if (boundingBox.Intersects(area_of_effect))
          return true;
      }
    }
    return false;
  }

  public override bool OverlapsFarmerForDamage(Farmer who)
  {
    if (base.OverlapsFarmerForDamage(who))
      return true;
    if (this.IsRoyalSerpent())
    {
      Rectangle boundingBox1 = this.GetBoundingBox();
      Rectangle boundingBox2 = who.GetBoundingBox();
      Vector2 vector2 = new Vector2((float) boundingBox1.X - this.Position.X, (float) boundingBox1.Y - this.Position.Y);
      foreach (Vector3 segment in this.segments)
      {
        boundingBox1.X = (int) ((double) segment.X + (double) vector2.X);
        boundingBox1.Y = (int) ((double) segment.Y + (double) vector2.Y);
        if (boundingBox1.Intersects(boundingBox2))
          return true;
      }
    }
    return false;
  }

  protected override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.killer.NetFields, "killer.NetFields").AddField((INetSerializable) this.segmentCount, "segmentCount");
    this.segmentCount.fieldChangeVisibleEvent += (FieldChange<NetInt, int>) ((field, old_value, new_value) =>
    {
      if (new_value <= 0)
        return;
      this.reloadSprite(false);
    });
  }

  /// <inheritdoc />
  public override void reloadSprite(bool onlyAppearance = false)
  {
    if (this.IsRoyalSerpent())
    {
      this.Sprite = new AnimatedSprite("Characters\\Monsters\\Royal Serpent");
      this.Scale = 1f;
    }
    else
    {
      this.Sprite = new AnimatedSprite("Characters\\Monsters\\Serpent");
      this.Scale = 0.75f;
    }
    this.Sprite.SpriteWidth = 32 /*0x20*/;
    this.Sprite.SpriteHeight = 32 /*0x20*/;
    this.HideShadow = true;
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
      this.setTrajectory(xTrajectory / 3, yTrajectory / 3);
      this.wasHitCounter = 500;
      this.currentLocation.playSound("serpentHit");
      if (this.Health <= 0)
      {
        this.killer.Value = who;
        this.deathAnimation();
      }
    }
    this.addedSpeed = (float) Game1.random.Next(-1, 1);
    return damage1;
  }

  protected override void sharedDeathAnimation()
  {
  }

  protected override void localDeathAnimation()
  {
    if (this.killer.Value == null)
      return;
    Rectangle boundingBox = this.GetBoundingBox();
    boundingBox.Inflate(-boundingBox.Width / 2 + 1, -boundingBox.Height / 2 + 1);
    Vector2 velocityTowardPlayer = Utility.getVelocityTowardPlayer(boundingBox.Center, 4f, this.killer.Value);
    int x = -(int) velocityTowardPlayer.X;
    int y = -(int) velocityTowardPlayer.Y;
    if (this.IsRoyalSerpent())
    {
      this.currentLocation.localSound("serpentDie");
      for (int index = -1; index < this.segments.Count; ++index)
      {
        Vector2 position;
        Rectangle sourceRect;
        float num;
        float t;
        if (index == -1)
        {
          position = this.Position;
          sourceRect = new Rectangle(0, 64 /*0x40*/, 32 /*0x20*/, 32 /*0x20*/);
          num = this.rotation;
          t = 0.0f;
        }
        else
        {
          if (this.segments.Count <= 0 || index >= this.segments.Count)
            break;
          t = (float) (index + 1) / (float) this.segments.Count;
          position = new Vector2(this.segments[index].X, this.segments[index].Y);
          boundingBox.X = (int) ((double) position.X - (double) (boundingBox.Width / 2));
          boundingBox.Y = (int) ((double) position.Y - (double) (boundingBox.Height / 2));
          sourceRect = new Rectangle(32 /*0x20*/, 64 /*0x40*/, 32 /*0x20*/, 32 /*0x20*/);
          if (index == this.segments.Count - 1)
            sourceRect = new Rectangle(64 /*0x40*/, 64 /*0x40*/, 32 /*0x20*/, 32 /*0x20*/);
          num = this.segments[index].Z;
        }
        TemporaryAnimatedSprite temporaryAnimatedSprite1 = new TemporaryAnimatedSprite(this.Sprite.textureName.Value, sourceRect, 800f, 1, 0, position, false, false, 0.9f, 1f / 1000f, new Color()
        {
          R = (byte) Utility.Lerp((float) byte.MaxValue, (float) byte.MaxValue, t),
          G = (byte) Utility.Lerp(0.0f, 166f, t),
          B = (byte) Utility.Lerp(0.0f, 0.0f, t),
          A = byte.MaxValue
        }, 4f * this.scale.Value, 0.01f, num + 3.14159274f, (float) ((double) Game1.random.Next(3, 5) * Math.PI / 64.0))
        {
          motion = new Vector2((float) x, (float) y),
          layerDepth = 1f
        };
        temporaryAnimatedSprite1.alphaFade = 0.025f;
        this.currentLocation.temporarySprites.Add(temporaryAnimatedSprite1);
        TemporaryAnimatedSprite temporaryAnimatedSprite2 = new TemporaryAnimatedSprite(5, Utility.PointToVector2(boundingBox.Center) + new Vector2(-32f, 0.0f), Color.LightGreen * 0.9f, 10, animationInterval: 70f)
        {
          delayBeforeAnimationStart = 50,
          motion = new Vector2((float) x, (float) y),
          layerDepth = 1f
        };
        if (index == -1)
          temporaryAnimatedSprite2.startSound = "cowboy_monsterhit";
        this.currentLocation.temporarySprites.Add(temporaryAnimatedSprite2);
        TemporaryAnimatedSprite temporaryAnimatedSprite3 = new TemporaryAnimatedSprite(5, Utility.PointToVector2(boundingBox.Center) + new Vector2(32f, 0.0f), Color.LightGreen * 0.8f, 10, animationInterval: 70f)
        {
          delayBeforeAnimationStart = 100,
          startSound = "cowboy_monsterhit",
          motion = new Vector2((float) x, (float) y) * 0.8f,
          layerDepth = 1f
        };
        if (index == -1)
          temporaryAnimatedSprite3.startSound = "cowboy_monsterhit";
        this.currentLocation.temporarySprites.Add(temporaryAnimatedSprite3);
        TemporaryAnimatedSprite temporaryAnimatedSprite4 = new TemporaryAnimatedSprite(5, Utility.PointToVector2(boundingBox.Center) + new Vector2(0.0f, -32f), Color.LightGreen * 0.7f, 10)
        {
          delayBeforeAnimationStart = 150,
          startSound = "cowboy_monsterhit",
          motion = new Vector2((float) x, (float) y) * 0.6f,
          layerDepth = 1f
        };
        if (index == -1)
          temporaryAnimatedSprite4.startSound = "cowboy_monsterhit";
        this.currentLocation.temporarySprites.Add(temporaryAnimatedSprite4);
        TemporaryAnimatedSprite temporaryAnimatedSprite5 = new TemporaryAnimatedSprite(5, Utility.PointToVector2(boundingBox.Center), Color.LightGreen * 0.6f, 10, animationInterval: 70f)
        {
          delayBeforeAnimationStart = 200,
          startSound = "cowboy_monsterhit",
          motion = new Vector2((float) x, (float) y) * 0.4f,
          layerDepth = 1f
        };
        if (index == -1)
          temporaryAnimatedSprite5.startSound = "cowboy_monsterhit";
        this.currentLocation.temporarySprites.Add(temporaryAnimatedSprite5);
        TemporaryAnimatedSprite temporaryAnimatedSprite6 = new TemporaryAnimatedSprite(5, Utility.PointToVector2(boundingBox.Center) + new Vector2(0.0f, 32f), Color.LightGreen * 0.5f, 10)
        {
          delayBeforeAnimationStart = 250,
          startSound = "cowboy_monsterhit",
          motion = new Vector2((float) x, (float) y) * 0.2f,
          layerDepth = 1f
        };
        if (index == -1)
          temporaryAnimatedSprite6.startSound = "cowboy_monsterhit";
        this.currentLocation.temporarySprites.Add(temporaryAnimatedSprite6);
      }
    }
    else
    {
      Vector2 vector2 = Utility.PointToVector2(this.StandingPixel);
      this.currentLocation.localSound("serpentDie");
      this.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(this.Sprite.textureName.Value, new Rectangle(0, 64 /*0x40*/, 32 /*0x20*/, 32 /*0x20*/), 200f, 4, 0, this.Position, false, false, 0.9f, 1f / 1000f, Color.White, 4f * this.scale.Value, 0.01f, this.rotation + 3.14159274f, (float) ((double) Game1.random.Next(3, 5) * Math.PI / 64.0))
      {
        motion = new Vector2((float) x, (float) y),
        layerDepth = 1f
      });
      this.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(5, vector2 + new Vector2(-32f, 0.0f), Color.LightGreen * 0.9f, 10, animationInterval: 70f)
      {
        delayBeforeAnimationStart = 50,
        startSound = "cowboy_monsterhit",
        motion = new Vector2((float) x, (float) y),
        layerDepth = 1f
      });
      this.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(5, vector2 + new Vector2(32f, 0.0f), Color.LightGreen * 0.8f, 10, animationInterval: 70f)
      {
        delayBeforeAnimationStart = 100,
        startSound = "cowboy_monsterhit",
        motion = new Vector2((float) x, (float) y) * 0.8f,
        layerDepth = 1f
      });
      this.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(5, vector2 + new Vector2(0.0f, -32f), Color.LightGreen * 0.7f, 10)
      {
        delayBeforeAnimationStart = 150,
        startSound = "cowboy_monsterhit",
        motion = new Vector2((float) x, (float) y) * 0.6f,
        layerDepth = 1f
      });
      this.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(5, vector2, Color.LightGreen * 0.6f, 10, animationInterval: 70f)
      {
        delayBeforeAnimationStart = 200,
        startSound = "cowboy_monsterhit",
        motion = new Vector2((float) x, (float) y) * 0.4f,
        layerDepth = 1f
      });
      this.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(5, vector2 + new Vector2(0.0f, 32f), Color.LightGreen * 0.5f, 10)
      {
        delayBeforeAnimationStart = 250,
        startSound = "cowboy_monsterhit",
        motion = new Vector2((float) x, (float) y) * 0.2f,
        layerDepth = 1f
      });
    }
  }

  public override List<Item> getExtraDropItems()
  {
    List<Item> extraDropItems = new List<Item>();
    if (Game1.random.NextDouble() < 0.002)
      extraDropItems.Add(ItemRegistry.Create("(O)485"));
    return extraDropItems;
  }

  public override void drawAboveAllLayers(SpriteBatch b)
  {
    Vector2 globalPosition1 = this.Position;
    bool flag = this.IsRoyalSerpent();
    int y1 = this.StandingPixel.Y;
    for (int index = -1; index < this.segmentCount.Value; ++index)
    {
      float num1 = (float) ((double) (index + 1) * -0.25 / 10000.0);
      float num2 = (float) ((double) this.segmentCount.Value * -0.25 / 10000.0 - 4.9999998736893758E-05);
      if ((double) (y1 - 1) / 10000.0 + (double) num2 < 0.0)
        num1 += (float) -((double) (y1 - 1) / 10000.0 + (double) num2);
      Rectangle rectangle = this.Sprite.SourceRect;
      Vector2 globalPosition2 = this.Position;
      Vector2 vector2_1;
      float rotation1;
      if (index == -1)
      {
        if (flag)
          rectangle = new Rectangle(0, 0, 32 /*0x20*/, 32 /*0x20*/);
        vector2_1 = this.Position;
        rotation1 = this.rotation;
      }
      else
      {
        if (index >= this.segments.Count)
          break;
        Vector3 segment = this.segments[index];
        vector2_1 = new Vector2(segment.X, segment.Y);
        rectangle = new Rectangle(32 /*0x20*/, 0, 32 /*0x20*/, 32 /*0x20*/);
        if (index == this.segments.Count - 1)
          rectangle = new Rectangle(64 /*0x40*/, 0, 32 /*0x20*/, 32 /*0x20*/);
        rotation1 = segment.Z;
        globalPosition2 = (globalPosition1 + vector2_1) / 2f;
      }
      if (Utility.isOnScreen(vector2_1, 128 /*0x80*/))
      {
        Vector2 vector2_2 = Game1.GlobalToLocal(Game1.viewport, vector2_1) + this.drawOffset + new Vector2(0.0f, (float) this.yJumpOffset);
        Vector2 vector2_3 = Game1.GlobalToLocal(Game1.viewport, globalPosition2) + this.drawOffset + new Vector2(0.0f, (float) this.yJumpOffset);
        int height = this.GetBoundingBox().Height;
        SpriteBatch spriteBatch = b;
        Texture2D shadowTexture = Game1.shadowTexture;
        Vector2 position = vector2_3 + new Vector2(64f, (float) height);
        Rectangle? sourceRectangle = new Rectangle?(Game1.shadowTexture.Bounds);
        Color white = Color.White;
        Rectangle bounds = Game1.shadowTexture.Bounds;
        double x = (double) bounds.Center.X;
        bounds = Game1.shadowTexture.Bounds;
        double y2 = (double) bounds.Center.Y;
        Vector2 origin = new Vector2((float) x, (float) y2);
        double layerDepth = (double) (y1 - 1) / 10000.0 + (double) num1;
        spriteBatch.Draw(shadowTexture, position, sourceRectangle, white, 0.0f, origin, 4f, SpriteEffects.None, (float) layerDepth);
        b.Draw(this.Sprite.Texture, vector2_2 + new Vector2(64f, (float) (height / 2)), new Rectangle?(rectangle), Color.White, rotation1, new Vector2(16f, 16f), Math.Max(0.2f, this.scale.Value) * 4f, this.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0.0f, this.drawOnTop ? 0.991f : (float) (y1 + 8) / 10000f + num1));
        if (this.isGlowing)
          b.Draw(this.Sprite.Texture, vector2_2 + new Vector2(64f, (float) (height / 2)), new Rectangle?(rectangle), this.glowingColor * this.glowingTransparency, rotation1, new Vector2(16f, 16f), Math.Max(0.2f, this.scale.Value) * 4f, this.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0.0f, this.drawOnTop ? 0.991f : (float) ((double) (y1 + 8) / 10000.0 + 9.9999997473787516E-05) + num1));
        if (flag)
        {
          float num3 = num1 - 5E-05f;
          float rotation2 = 0.0f;
          rectangle = new Rectangle(96 /*0x60*/, 0, 32 /*0x20*/, 32 /*0x20*/);
          Vector2 vector2_4 = Game1.GlobalToLocal(Game1.viewport, globalPosition1) + this.drawOffset + new Vector2(0.0f, (float) this.yJumpOffset);
          if (index > 0)
            b.Draw(Game1.shadowTexture, vector2_4 + new Vector2(64f, (float) height), new Rectangle?(Game1.shadowTexture.Bounds), Color.White, 0.0f, new Vector2((float) Game1.shadowTexture.Bounds.Center.X, (float) Game1.shadowTexture.Bounds.Center.Y), 4f, SpriteEffects.None, (float) (y1 - 1) / 10000f + num3);
          b.Draw(this.Sprite.Texture, vector2_4 + new Vector2(64f, (float) (height / 2)), new Rectangle?(rectangle), Color.White, rotation2, new Vector2(16f, 16f), Math.Max(0.2f, this.scale.Value) * 4f, this.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0.0f, this.drawOnTop ? 0.991f : (float) (y1 + 8) / 10000f + num3));
          if (this.isGlowing)
            b.Draw(this.Sprite.Texture, vector2_4 + new Vector2(64f, (float) (height / 2)), new Rectangle?(rectangle), this.glowingColor * this.glowingTransparency, rotation2, new Vector2(16f, 16f), Math.Max(0.2f, this.scale.Value) * 4f, this.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0.0f, this.drawOnTop ? 0.991f : (float) ((double) (y1 + 8) / 10000.0 + 9.9999997473787516E-05) + num3));
        }
      }
      globalPosition1 = vector2_1;
    }
  }

  public override Rectangle GetBoundingBox()
  {
    Vector2 position = this.Position;
    return new Rectangle((int) position.X + 8, (int) position.Y, this.Sprite.SpriteWidth * 4 * 3 / 4, 96 /*0x60*/);
  }

  protected override void updateAnimation(GameTime time)
  {
    if (this.IsRoyalSerpent())
    {
      if (this.segments.Count < this.segmentCount.Value)
      {
        for (int index = 0; index < this.segmentCount.Value; ++index)
        {
          Vector2 position = this.Position;
          this.segments.Add(new Vector3(position.X, position.Y, 0.0f));
        }
      }
      Vector2 vector2_1 = this.Position;
      for (int index = 0; index < this.segments.Count; ++index)
      {
        Vector2 vector2_2 = new Vector2(this.segments[index].X, this.segments[index].Y);
        Vector2 vector2_3 = vector2_2 - vector2_1;
        int num1 = 64 /*0x40*/;
        int num2 = (int) vector2_3.Length();
        vector2_3.Normalize();
        int num3 = num1;
        if (num2 > num3)
          vector2_2 = vector2_3 * (float) num1 + vector2_1;
        double z = Math.Atan2((double) vector2_3.Y, (double) vector2_3.X) - Math.PI / 2.0;
        this.segments[index] = new Vector3(vector2_2.X, vector2_2.Y, (float) z);
        vector2_1 = vector2_2;
      }
    }
    base.updateAnimation(time);
    if (this.wasHitCounter >= 0)
      this.wasHitCounter -= time.ElapsedGameTime.Milliseconds;
    if (!this.IsRoyalSerpent())
      this.Sprite.Animate(time, 0, 9, 40f);
    if (this.withinPlayerThreshold() && this.invincibleCountdown <= 0)
    {
      Point standingPixel1 = this.StandingPixel;
      Point standingPixel2 = this.Player.StandingPixel;
      float num4 = (float) -(standingPixel2.X - standingPixel1.X);
      float num5 = (float) (standingPixel2.Y - standingPixel1.Y);
      float num6 = Math.Max(1f, Math.Abs(num4) + Math.Abs(num5));
      if ((double) num6 < 64.0)
      {
        this.xVelocity = Math.Max(-7f, Math.Min(7f, this.xVelocity * 1.1f));
        this.yVelocity = Math.Max(-7f, Math.Min(7f, this.yVelocity * 1.1f));
      }
      float x = num4 / num6;
      float num7 = num5 / num6;
      if (this.wasHitCounter <= 0)
      {
        this.targetRotation = (float) Math.Atan2(-(double) num7, (double) x) - 1.57079637f;
        if ((double) Math.Abs(this.targetRotation) - (double) Math.Abs(this.rotation) > 7.0 * Math.PI / 8.0 && Game1.random.NextBool())
          this.turningRight = true;
        else if ((double) Math.Abs(this.targetRotation) - (double) Math.Abs(this.rotation) < Math.PI / 8.0)
          this.turningRight = false;
        if (this.turningRight)
          this.rotation -= (float) Math.Sign(this.targetRotation - this.rotation) * ((float) Math.PI / 64f);
        else
          this.rotation += (float) Math.Sign(this.targetRotation - this.rotation) * ((float) Math.PI / 64f);
        this.rotation %= 6.28318548f;
        this.wasHitCounter = 5 + Game1.random.Next(-1, 2);
      }
      float num8 = Math.Min(7f, Math.Max(2f, (float) (7.0 - (double) num6 / 64.0 / 2.0)));
      float num9 = (float) Math.Cos((double) this.rotation + Math.PI / 2.0);
      float num10 = -(float) Math.Sin((double) this.rotation + Math.PI / 2.0);
      this.xVelocity += (float) (-(double) num9 * (double) num8 / 6.0 + (double) Game1.random.Next(-10, 10) / 100.0);
      this.yVelocity += (float) (-(double) num10 * (double) num8 / 6.0 + (double) Game1.random.Next(-10, 10) / 100.0);
      if ((double) Math.Abs(this.xVelocity) > (double) Math.Abs((float) (-(double) num9 * 7.0)))
        this.xVelocity -= (float) (-(double) num9 * (double) num8 / 6.0);
      if ((double) Math.Abs(this.yVelocity) > (double) Math.Abs((float) (-(double) num10 * 7.0)))
        this.yVelocity -= (float) (-(double) num10 * (double) num8 / 6.0);
    }
    this.resetAnimationSpeed();
  }

  public override void behaviorAtGameTick(GameTime time)
  {
    base.behaviorAtGameTick(time);
    if (double.IsNaN((double) this.xVelocity) || double.IsNaN((double) this.yVelocity))
      this.Health = -500;
    if ((double) this.Position.X <= -640.0 || (double) this.Position.Y <= -640.0 || (double) this.Position.X >= (double) (this.currentLocation.Map.Layers[0].LayerWidth * 64 /*0x40*/ + 640) || (double) this.Position.Y >= (double) (this.currentLocation.Map.Layers[0].LayerHeight * 64 /*0x40*/ + 640))
      this.Health = -500;
    if (!this.withinPlayerThreshold() || this.invincibleCountdown > 0)
      return;
    this.faceDirection(2);
  }
}
