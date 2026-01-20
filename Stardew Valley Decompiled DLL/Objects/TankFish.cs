// Decompiled with JetBrains decompiler
// Type: StardewValley.Objects.TankFish
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Objects.Trinkets;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

#nullable disable
namespace StardewValley.Objects;

public class TankFish
{
  /// <summary>The field index in <c>Data/AquariumFish</c> for the sprite index.</summary>
  public const int field_spriteIndex = 0;
  /// <summary>The field index in <c>Data/AquariumFish</c> for the type.</summary>
  public const int field_type = 1;
  /// <summary>The field index in <c>Data/AquariumFish</c> for the idle animations.</summary>
  public const int field_idleAnimations = 2;
  /// <summary>The field index in <c>Data/AquariumFish</c> for the dart start animation frames.</summary>
  public const int field_dartStartFrames = 3;
  /// <summary>The field index in <c>Data/AquariumFish</c> for the dart hold animation frames.</summary>
  public const int field_dartHoldFrames = 4;
  /// <summary>The field index in <c>Data/AquariumFish</c> for the dart end animation frames.</summary>
  public const int field_dartEndFrames = 5;
  /// <summary>The field index in <c>Data/AquariumFish</c> for the texture, if set.</summary>
  public const int field_texture = 6;
  /// <summary>The field index in <c>Data/AquariumFish</c> for the pixel offset from the upper-left corner of sprite that the hat sits on, if set.</summary>
  public const int field_hatOffset = 7;
  protected FishTankFurniture _tank;
  public Vector2 position;
  public float zPosition;
  public bool facingLeft;
  public Vector2 velocity = Vector2.Zero;
  protected Texture2D _texture;
  public float nextSwim;
  public string fishItemId = "";
  public int fishIndex;
  public int currentFrame;
  public Point? hatPosition;
  public int frogVariant;
  public int numberOfDarts;
  public TankFish.FishType fishType;
  public float minimumVelocity;
  public float fishScale = 1f;
  public List<int> currentAnimation;
  public List<int> idleAnimation;
  public List<int> dartStartAnimation;
  public List<int> dartHoldAnimation;
  public List<int> dartEndAnimation;
  public int currentAnimationFrame;
  public float currentFrameTime;
  public float nextBubble;
  public bool isErrorFish;

  public TankFish(FishTankFurniture tank, Item item)
  {
    this._tank = tank;
    this.fishItemId = item.ItemId;
    string str1;
    if (!this._tank.GetAquariumData().TryGetValue(item.ItemId, out str1))
    {
      str1 = "0/float";
      this.isErrorFish = true;
    }
    string[] array = str1.Split('/');
    string assetName = ArgUtility.Get(array, 6, allowBlank: false);
    if (assetName != null)
    {
      try
      {
        this._texture = Game1.content.Load<Texture2D>(assetName);
      }
      catch (Exception ex)
      {
        this.isErrorFish = true;
      }
    }
    if (this._texture == null)
      this._texture = this._tank.GetAquariumTexture();
    string str2 = ArgUtility.Get(array, 7, allowBlank: false);
    if (str2 != null)
    {
      try
      {
        string[] strArray = ArgUtility.SplitBySpace(str2);
        this.hatPosition = new Point?(new Point(int.Parse(strArray[0]), int.Parse(strArray[1])));
      }
      catch (Exception ex)
      {
        this.hatPosition = new Point?();
      }
    }
    this.fishIndex = int.Parse(array[0]);
    this.currentFrame = this.fishIndex;
    this.zPosition = Utility.RandomFloat(4f, 10f);
    this.fishScale = 0.75f;
    string str3;
    if (DataLoader.Fish(Game1.content).TryGetValue(item.ItemId, out str3))
    {
      string[] strArray = str3.Split('/');
      if (!(strArray[1] == "trap"))
      {
        this.minimumVelocity = Utility.RandomFloat(0.25f, 0.35f);
        if (strArray[2] == "smooth")
          this.minimumVelocity = Utility.RandomFloat(0.5f, 0.6f);
        if (strArray[2] == "dart")
          this.minimumVelocity = 0.0f;
      }
    }
    string str4 = ArgUtility.Get(array, 1);
    if (str4 != null)
    {
      switch (str4.Length)
      {
        case 3:
          if (str4 == "eel")
          {
            this.fishType = TankFish.FishType.Eel;
            this.minimumVelocity = Utility.Clamp(this.fishScale, 0.3f, 0.4f);
            break;
          }
          break;
        case 5:
          switch (str4[0])
          {
            case 'c':
              if (str4 == "crawl")
              {
                this.fishType = TankFish.FishType.Crawl;
                this.minimumVelocity = 0.0f;
                break;
              }
              break;
            case 'f':
              if (str4 == "float")
              {
                this.fishType = TankFish.FishType.Float;
                break;
              }
              break;
          }
          break;
        case 6:
          switch (str4[0])
          {
            case 'g':
              if (str4 == "ground")
              {
                this.fishType = TankFish.FishType.Ground;
                this.zPosition = 4f;
                this.minimumVelocity = 0.0f;
                break;
              }
              break;
            case 's':
              if (str4 == "static")
              {
                this.fishType = TankFish.FishType.Static;
                break;
              }
              break;
          }
          break;
        case 10:
          if (str4 == "cephalopod")
          {
            this.fishType = TankFish.FishType.Cephalopod;
            this.minimumVelocity = 0.0f;
            break;
          }
          break;
        case 11:
          if (str4 == "front_crawl")
          {
            this.fishType = TankFish.FishType.Crawl;
            this.zPosition = 3f;
            this.minimumVelocity = 0.0f;
            break;
          }
          break;
      }
    }
    string str5 = ArgUtility.Get(array, 2, allowBlank: false);
    if (str5 != null)
    {
      string[] strArray = ArgUtility.SplitBySpace(str5);
      this.idleAnimation = new List<int>();
      foreach (string s in strArray)
        this.idleAnimation.Add(int.Parse(s));
      this.SetAnimation(this.idleAnimation);
    }
    string str6 = ArgUtility.Get(array, 3, allowBlank: false);
    if (str6 != null)
    {
      string[] strArray = ArgUtility.SplitBySpace(str6);
      this.dartStartAnimation = new List<int>();
      foreach (string s in strArray)
        this.dartStartAnimation.Add(int.Parse(s));
    }
    string str7 = ArgUtility.Get(array, 4, allowBlank: false);
    if (str7 != null)
    {
      string[] strArray = ArgUtility.SplitBySpace(str7);
      this.dartHoldAnimation = new List<int>();
      foreach (string s in strArray)
        this.dartHoldAnimation.Add(int.Parse(s));
    }
    string str8 = ArgUtility.Get(array, 5, allowBlank: false);
    if (str8 != null)
    {
      string[] strArray = ArgUtility.SplitBySpace(str8);
      this.dartEndAnimation = new List<int>();
      foreach (string s in strArray)
        this.dartEndAnimation.Add(int.Parse(s));
    }
    Rectangle tankBounds = this._tank.GetTankBounds() with
    {
      X = 0,
      Y = 0
    };
    this.position = Vector2.Zero;
    this.position = Utility.getRandomPositionInThisRectangle(tankBounds, Game1.random);
    this.nextSwim = Utility.RandomFloat(0.1f, 10f);
    this.nextBubble = Utility.RandomFloat(0.1f, 10f);
    this.facingLeft = Game1.random.Next(2) == 1;
    this.velocity = !this.facingLeft ? new Vector2(1f, 0.0f) : new Vector2(-1f, 0.0f);
    this.velocity *= this.minimumVelocity;
    if (item.QualifiedItemId == "(TR)FrogEgg")
    {
      this.fishType = TankFish.FishType.Hop;
      this._texture = Game1.content.Load<Texture2D>("TileSheets\\companions");
      this.frogVariant = ((item as Trinket).GetEffect() as CompanionTrinketEffect).Variant;
      this.isErrorFish = false;
    }
    if (this.fishType == TankFish.FishType.Ground || this.fishType == TankFish.FishType.Crawl || this.fishType == TankFish.FishType.Hop || this.fishType == TankFish.FishType.Static)
      this.position.Y = 0.0f;
    this.ConstrainToTank();
  }

  public void SetAnimation(List<int> frames)
  {
    if (this.fishType == TankFish.FishType.Hop || this.currentAnimation == frames)
      return;
    this.currentAnimation = frames;
    this.currentAnimationFrame = 0;
    this.currentFrameTime = 0.0f;
    List<int> currentAnimation = this.currentAnimation;
    // ISSUE: explicit non-virtual call
    if ((currentAnimation != null ? (__nonvirtual (currentAnimation.Count) > 0 ? 1 : 0) : 0) == 0)
      return;
    this.currentFrame = frames[0];
  }

  public virtual void Draw(SpriteBatch b, float alpha, float draw_layer)
  {
    SpriteEffects effects = SpriteEffects.None;
    int num1 = -12;
    int width = 8;
    if (this.fishType == TankFish.FishType.Eel)
      width = 4;
    int num2 = width;
    if (this.facingLeft)
    {
      effects = SpriteEffects.FlipHorizontally;
      num2 *= -1;
      num1 = -num1 - width;
    }
    TimeSpan totalGameTime = Game1.currentGameTime.TotalGameTime;
    float y1 = (float) Math.Sin(totalGameTime.TotalSeconds * 1.25 + (double) this.position.X / 32.0) * 2f;
    if (this.fishType == TankFish.FishType.Crawl || this.fishType == TankFish.FishType.Ground || this.fishType == TankFish.FishType.Static)
      y1 = 0.0f;
    float scale = this.GetScale();
    int num3 = this._texture.Width / 24;
    int x1 = this.currentFrame % num3 * 24;
    int y2 = this.currentFrame / num3 * 48 /*0x30*/;
    int num4 = 10;
    float num5 = 1f;
    if (this.fishType == TankFish.FishType.Eel)
    {
      num4 = 20;
      y1 *= 0.0f;
    }
    float y3 = -12f;
    float num6 = 0.0f;
    if (this.isErrorFish)
    {
      num6 = 0.0f;
      IItemDataDefinition itemDataDefinition = ItemRegistry.RequireTypeDefinition("(F)");
      b.Draw(itemDataDefinition.GetErrorTexture(), Game1.GlobalToLocal(this.GetWorldPosition() + new Vector2(0.0f, y1) * 4f * scale), new Rectangle?(itemDataDefinition.GetErrorSourceRect()), Color.White * alpha, num6, new Vector2(12f, 12f), 4f * scale, effects, draw_layer);
    }
    else
    {
      switch (this.fishType)
      {
        case TankFish.FishType.Cephalopod:
        case TankFish.FishType.Float:
          num6 = Utility.Clamp(this.velocity.X, -0.5f, 0.5f);
          b.Draw(this._texture, Game1.GlobalToLocal(this.GetWorldPosition() + new Vector2(0.0f, y1) * 4f * scale), new Rectangle?(new Rectangle(x1, y2, 24, 24)), Color.White * alpha, num6, new Vector2(12f, 12f), 4f * scale, effects, draw_layer);
          break;
        case TankFish.FishType.Ground:
        case TankFish.FishType.Crawl:
        case TankFish.FishType.Static:
          num6 = 0.0f;
          b.Draw(this._texture, Game1.GlobalToLocal(this.GetWorldPosition() + new Vector2(0.0f, y1) * 4f * scale), new Rectangle?(new Rectangle(x1, y2, 24, 24)), Color.White * alpha, num6, new Vector2(12f, 12f), 4f * scale, effects, draw_layer);
          break;
        case TankFish.FishType.Hop:
          int num7 = 0;
          if ((double) this.position.Y > 0.0)
            num7 = (double) this.velocity.Y <= 0.2 ? 3 : ((double) this.velocity.Y <= 0.3 ? 2 : 1);
          else if ((double) this.nextSwim <= 3.0)
          {
            totalGameTime = Game1.currentGameTime.TotalGameTime;
            num7 = totalGameTime.TotalMilliseconds % 400.0 >= 200.0 ? 5 : 6;
          }
          Rectangle rectangle = new Rectangle(num7 * 16 /*0x10*/, 16 /*0x10*/ + this.frogVariant * 16 /*0x10*/, 16 /*0x10*/, 16 /*0x10*/);
          Color color = Color.White;
          if (this.frogVariant == 7)
            color = Utility.GetPrismaticColor();
          b.Draw(this._texture, Game1.GlobalToLocal(this.GetWorldPosition() + new Vector2(16f, -8f)), new Rectangle?(rectangle), color * alpha, num6, new Vector2(12f, 12f), 4f * scale, effects, draw_layer);
          break;
        default:
          for (int index = 0; index < 24 / width; ++index)
          {
            float num8 = 1f - (float) (index * width) / (float) num4;
            float num9 = this.velocity.Length() / 1f;
            float num10 = 1f;
            float num11 = 0.0f;
            float num12 = Utility.Clamp(num9, 0.2f, 1f);
            float num13 = Utility.Clamp(num8, 0.0f, 1f);
            if (this.fishType == TankFish.FishType.Eel)
            {
              num13 = 1f;
              num12 = 1f;
              num10 = 0.1f;
              num11 = 4f;
            }
            if (this.facingLeft)
              num11 *= -1f;
            double num14 = (double) (index * 20);
            totalGameTime = Game1.currentGameTime.TotalGameTime;
            double num15 = totalGameTime.TotalSeconds * 25.0 * (double) num10;
            float num16 = (float) Math.Sin(num14 + num15 + (double) num11 * (double) this.position.X / 16.0) * num5 * num13 * num12;
            if (index == 24 / width - 1)
              y3 = num16 - 12f;
            b.Draw(this._texture, Game1.GlobalToLocal(this.GetWorldPosition() + new Vector2((float) (num1 + index * num2), y1 + num16) * 4f * scale), new Rectangle?(new Rectangle(x1 + index * width, y2, width, 24)), Color.White * alpha, 0.0f, new Vector2(0.0f, 12f), 4f * scale, effects, draw_layer);
          }
          break;
      }
    }
    float x2 = this.facingLeft ? 12f : -12f;
    b.Draw(Game1.shadowTexture, Game1.GlobalToLocal(new Vector2(this.GetWorldPosition().X, (float) this._tank.GetTankBounds().Bottom - this.zPosition * 4f)), new Rectangle?(), Color.White * alpha * 0.75f, 0.0f, new Vector2((float) (Game1.shadowTexture.Width / 2), (float) (Game1.shadowTexture.Height / 2)), new Vector2(4f * scale, 1f), SpriteEffects.None, this._tank.GetFishSortRegion().X - 1E-07f);
    int num17 = 0;
    foreach (TankFish tankFish in this._tank.tankFish)
    {
      if (tankFish != this)
      {
        if (tankFish.CanWearHat())
          ++num17;
      }
      else
        break;
    }
    if (!this.CanWearHat())
      return;
    int num18 = 0;
    foreach (Item heldItem in (NetList<Item, NetRef<Item>>) this._tank.heldItems)
    {
      if (heldItem is Hat hat)
      {
        if (num18 == num17)
        {
          Vector2 vector2_1 = new Vector2((float) this.hatPosition.Value.X, (float) this.hatPosition.Value.Y);
          if (this.facingLeft)
            vector2_1.X *= -1f;
          Vector2 vector2_2 = new Vector2(x2, y3) + vector2_1;
          if ((double) num6 != 0.0)
          {
            float num19 = (float) Math.Cos((double) num6);
            float num20 = (float) Math.Sin((double) num6);
            vector2_2.X = (float) ((double) vector2_2.X * (double) num19 - (double) vector2_2.Y * (double) num20);
            vector2_2.Y = (float) ((double) vector2_2.X * (double) num20 + (double) vector2_2.Y * (double) num19);
          }
          Vector2 vector2_3 = vector2_2 * (4f * scale);
          Vector2 local = Game1.GlobalToLocal(this.GetWorldPosition() + vector2_3);
          local.Y += y1;
          int direction = this.fishType == TankFish.FishType.Cephalopod || this.fishType == TankFish.FishType.Static ? 2 : (!this.facingLeft ? 1 : 3);
          Vector2 location = local - new Vector2(10f, 10f) + new Vector2(3f, 3f) * scale * 3f - new Vector2(10f, 10f) * scale * 3f;
          hat.draw(b, location, scale, 1f, draw_layer + 1E-08f, direction);
          int num21 = num17 + 1;
          break;
        }
        ++num18;
      }
    }
  }

  [MemberNotNullWhen(true, "hatPosition")]
  public bool CanWearHat() => this.hatPosition.HasValue;

  public Vector2 GetWorldPosition()
  {
    return new Vector2((float) this._tank.GetTankBounds().X + this.position.X, (float) ((double) this._tank.GetTankBounds().Bottom - (double) this.position.Y - (double) this.zPosition * 4.0));
  }

  public void ConstrainToTank()
  {
    Rectangle tankBounds = this._tank.GetTankBounds();
    Rectangle bounds = this.GetBounds();
    tankBounds.X = 0;
    tankBounds.Y = 0;
    if (bounds.X < tankBounds.X)
    {
      this.position.X += (float) (tankBounds.X - bounds.X);
      bounds = this.GetBounds();
    }
    if (bounds.Y < tankBounds.Y)
    {
      this.position.Y -= (float) (tankBounds.Y - bounds.Y);
      bounds = this.GetBounds();
    }
    if (bounds.Right > tankBounds.Right)
    {
      this.position.X += (float) (tankBounds.Right - bounds.Right);
      bounds = this.GetBounds();
    }
    if (this.fishType == TankFish.FishType.Crawl || this.fishType == TankFish.FishType.Ground || this.fishType == TankFish.FishType.Static || this.fishType == TankFish.FishType.Hop)
    {
      if ((double) this.position.Y <= (double) tankBounds.Bottom)
        return;
      this.position.Y -= (float) tankBounds.Bottom - this.position.Y;
    }
    else
    {
      if (bounds.Bottom <= tankBounds.Bottom)
        return;
      this.position.Y -= (float) (tankBounds.Bottom - bounds.Bottom);
    }
  }

  public virtual float GetScale() => this.fishScale;

  public Rectangle GetBounds()
  {
    Vector2 vector2 = new Vector2(24f, 18f) * (4f * this.GetScale());
    return this.fishType == TankFish.FishType.Crawl || this.fishType == TankFish.FishType.Ground || this.fishType == TankFish.FishType.Static || this.fishType == TankFish.FishType.Hop ? new Rectangle((int) ((double) this.position.X - (double) vector2.X / 2.0), (int) ((double) this._tank.GetTankBounds().Height - (double) this.position.Y - (double) vector2.Y), (int) vector2.X, (int) vector2.Y) : new Rectangle((int) ((double) this.position.X - (double) vector2.X / 2.0), (int) ((double) this._tank.GetTankBounds().Height - (double) this.position.Y - (double) vector2.Y / 2.0), (int) vector2.X, (int) vector2.Y);
  }

  public virtual void Update(GameTime time)
  {
    List<int> currentAnimation = this.currentAnimation;
    // ISSUE: explicit non-virtual call
    if ((currentAnimation != null ? (__nonvirtual (currentAnimation.Count) > 0 ? 1 : 0) : 0) != 0)
    {
      this.currentFrameTime += (float) time.ElapsedGameTime.TotalSeconds;
      float num = 0.125f;
      if ((double) this.currentFrameTime > (double) num)
      {
        this.currentAnimationFrame += (int) ((double) this.currentFrameTime / (double) num);
        this.currentFrameTime %= num;
        if (this.currentAnimationFrame >= this.currentAnimation.Count)
        {
          if (this.currentAnimation == this.idleAnimation)
          {
            this.currentAnimationFrame %= this.currentAnimation.Count;
            this.currentFrame = this.currentAnimation[this.currentAnimationFrame];
          }
          else if (this.currentAnimation == this.dartStartAnimation)
          {
            if (this.dartHoldAnimation != null)
              this.SetAnimation(this.dartHoldAnimation);
            else
              this.SetAnimation(this.idleAnimation);
          }
          else if (this.currentAnimation == this.dartHoldAnimation)
          {
            this.currentAnimationFrame %= this.currentAnimation.Count;
            this.currentFrame = this.currentAnimation[this.currentAnimationFrame];
          }
          else if (this.currentAnimation == this.dartEndAnimation)
            this.SetAnimation(this.idleAnimation);
        }
        else
          this.currentFrame = this.currentAnimation[this.currentAnimationFrame];
      }
    }
    if (this.fishType != TankFish.FishType.Static)
    {
      Rectangle tankBounds = this._tank.GetTankBounds() with
      {
        X = 0,
        Y = 0
      };
      float num1 = this.velocity.X;
      if (this.fishType == TankFish.FishType.Crawl)
        num1 = Utility.Clamp(num1, -0.5f, 0.5f);
      this.position.X += num1;
      Rectangle bounds = this.GetBounds();
      if (bounds.Left < tankBounds.Left || bounds.Right > tankBounds.Right)
      {
        this.ConstrainToTank();
        bounds = this.GetBounds();
        this.velocity.X *= -1f;
        this.facingLeft = !this.facingLeft;
      }
      this.position.Y += this.velocity.Y;
      bounds = this.GetBounds();
      if (bounds.Top < tankBounds.Top || bounds.Bottom > tankBounds.Bottom)
      {
        this.ConstrainToTank();
        this.velocity.Y *= 0.0f;
      }
      float a = this.velocity.Length();
      if ((double) a > (double) this.minimumVelocity)
      {
        float t = 0.015f;
        if (this.fishType == TankFish.FishType.Crawl || this.fishType == TankFish.FishType.Ground || this.fishType == TankFish.FishType.Hop)
          t = 0.03f;
        float num2 = Utility.Lerp(a, this.minimumVelocity, t);
        if ((double) num2 < 9.9999997473787516E-05)
          num2 = 0.0f;
        this.velocity.Normalize();
        this.velocity *= num2;
        if (this.currentAnimation == this.dartHoldAnimation && (double) num2 <= (double) this.minimumVelocity + 0.5)
        {
          List<int> dartEndAnimation = this.dartEndAnimation;
          // ISSUE: explicit non-virtual call
          if ((dartEndAnimation != null ? (__nonvirtual (dartEndAnimation.Count) > 0 ? 1 : 0) : 0) != 0)
          {
            this.SetAnimation(this.dartEndAnimation);
          }
          else
          {
            List<int> idleAnimation = this.idleAnimation;
            // ISSUE: explicit non-virtual call
            if ((idleAnimation != null ? (__nonvirtual (idleAnimation.Count) > 0 ? 1 : 0) : 0) != 0)
              this.SetAnimation(this.idleAnimation);
          }
        }
      }
      this.nextSwim -= (float) time.ElapsedGameTime.TotalSeconds;
      if ((double) this.nextSwim <= 0.0)
      {
        if (this.numberOfDarts == 0)
        {
          this.numberOfDarts = Game1.random.Next(1, 4);
          this.nextSwim = Utility.RandomFloat(6f, 12f);
          switch (this.fishType)
          {
            case TankFish.FishType.Cephalopod:
              this.nextSwim = Utility.RandomFloat(2f, 5f);
              break;
            case TankFish.FishType.Hop:
              this.numberOfDarts = 0;
              break;
          }
          if (Game1.random.NextDouble() < 0.30000001192092896)
            this.facingLeft = !this.facingLeft;
        }
        else
        {
          this.nextSwim = Utility.RandomFloat(0.1f, 0.5f);
          --this.numberOfDarts;
          if (Game1.random.NextDouble() < 0.05000000074505806)
            this.facingLeft = !this.facingLeft;
        }
        List<int> dartStartAnimation = this.dartStartAnimation;
        // ISSUE: explicit non-virtual call
        if ((dartStartAnimation != null ? (__nonvirtual (dartStartAnimation.Count) > 0 ? 1 : 0) : 0) != 0)
        {
          this.SetAnimation(this.dartStartAnimation);
        }
        else
        {
          List<int> dartHoldAnimation = this.dartHoldAnimation;
          // ISSUE: explicit non-virtual call
          if ((dartHoldAnimation != null ? (__nonvirtual (dartHoldAnimation.Count) > 0 ? 1 : 0) : 0) != 0)
            this.SetAnimation(this.dartHoldAnimation);
        }
        this.velocity.X = 1.5f;
        if (this._tank.getTilesWide() <= 2)
          this.velocity.X *= 0.5f;
        if (this.facingLeft)
          this.velocity.X *= -1f;
        switch (this.fishType)
        {
          case TankFish.FishType.Cephalopod:
            this.velocity.Y = Utility.RandomFloat(0.5f, 0.75f);
            break;
          case TankFish.FishType.Ground:
            this.velocity.X *= 0.5f;
            this.velocity.Y = Utility.RandomFloat(0.5f, 0.25f);
            break;
          case TankFish.FishType.Hop:
            this.velocity.Y = Utility.RandomFloat(0.35f, 0.65f);
            break;
          default:
            this.velocity.Y = Utility.RandomFloat(-0.5f, 0.5f);
            break;
        }
        if (this.fishType == TankFish.FishType.Crawl)
          this.velocity.Y = 0.0f;
      }
    }
    if (this.fishType == TankFish.FishType.Cephalopod || this.fishType == TankFish.FishType.Ground || this.fishType == TankFish.FishType.Crawl || this.fishType == TankFish.FishType.Static || this.fishType == TankFish.FishType.Hop)
    {
      float num = 0.2f;
      if (this.fishType == TankFish.FishType.Static)
        num = 0.6f;
      if ((double) this.position.Y > 0.0)
        this.position.Y -= num;
    }
    this.nextBubble -= (float) time.ElapsedGameTime.TotalSeconds;
    if ((double) this.nextBubble <= 0.0)
    {
      this.nextBubble = Utility.RandomFloat(1f, 10f);
      float num = 0.0f;
      if (this.fishType == TankFish.FishType.Ground || this.fishType == TankFish.FishType.Normal || this.fishType == TankFish.FishType.Eel)
        num = 32f;
      if (this.facingLeft)
        num *= -1f;
      this._tank.bubbles.Add(new Vector4(this.position.X + num * this.fishScale, this.position.Y + this.zPosition, this.zPosition, 0.25f));
    }
    this.ConstrainToTank();
  }

  public enum FishType
  {
    Normal,
    Eel,
    Cephalopod,
    Float,
    Ground,
    Crawl,
    Hop,
    Static,
  }
}
