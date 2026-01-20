// Decompiled with JetBrains decompiler
// Type: StardewValley.Monsters.GreenSlime
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Extensions;
using StardewValley.Projectiles;
using StardewValley.SpecialOrders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley.Monsters;

public class GreenSlime : Monster
{
  public const float mutationFactor = 0.25f;
  public const int matingInterval = 120000;
  public const int childhoodLength = 120000;
  public const int durationOfMating = 2000;
  public const double chanceToMate = 0.001;
  public static int matingRange = 192 /*0xC0*/;
  public const int AQUA_SLIME = 9999899;
  public NetIntDelta stackedSlimes;
  public float randomStackOffset;
  [XmlIgnore]
  public NetEvent1Field<Vector2, NetVector2> attackedEvent;
  [XmlElement("leftDrift")]
  public readonly NetBool leftDrift;
  [XmlElement("cute")]
  public readonly NetBool cute;
  [XmlIgnore]
  public int readyToJump;
  [XmlIgnore]
  public int matingCountdown;
  [XmlIgnore]
  public int yOffset;
  [XmlIgnore]
  public int wagTimer;
  public int readyToMate;
  [XmlElement("ageUntilFullGrown")]
  public readonly NetInt ageUntilFullGrown;
  public int animateTimer;
  public int timeSinceLastJump;
  [XmlElement("specialNumber")]
  public readonly NetInt specialNumber;
  [XmlElement("firstGeneration")]
  public readonly NetBool firstGeneration;
  [XmlElement("color")]
  public readonly NetColor color;
  private readonly NetBool pursuingMate;
  private readonly NetBool avoidingMate;
  private GreenSlime mate;
  public readonly NetBool prismatic;
  private readonly NetVector2 facePosition;
  private readonly NetEvent1Field<Vector2, NetVector2> jumpEvent;

  protected override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.leftDrift, "leftDrift").AddField((INetSerializable) this.cute, "cute").AddField((INetSerializable) this.ageUntilFullGrown, "ageUntilFullGrown").AddField((INetSerializable) this.specialNumber, "specialNumber").AddField((INetSerializable) this.firstGeneration, "firstGeneration").AddField((INetSerializable) this.color, "color").AddField((INetSerializable) this.pursuingMate, "pursuingMate").AddField((INetSerializable) this.avoidingMate, "avoidingMate").AddField((INetSerializable) this.facePosition, "facePosition").AddField((INetSerializable) this.jumpEvent, "jumpEvent").AddField((INetSerializable) this.prismatic, "prismatic").AddField((INetSerializable) this.stackedSlimes, "stackedSlimes").AddField(this.attackedEvent.NetFields, "attackedEvent.NetFields");
    this.attackedEvent.onEvent += new AbstractNetEvent1<Vector2>.Event(this.OnAttacked);
    this.jumpEvent.onEvent += new AbstractNetEvent1<Vector2>.Event(this.doJump);
  }

  public GreenSlime()
  {
    NetEvent1Field<Vector2, NetVector2> netEvent1Field = new NetEvent1Field<Vector2, NetVector2>();
    netEvent1Field.InterpolationWait = false;
    this.jumpEvent = netEvent1Field;
    // ISSUE: explicit constructor call
    base.\u002Ector();
  }

  public GreenSlime(Vector2 position)
  {
    NetEvent1Field<Vector2, NetVector2> netEvent1Field = new NetEvent1Field<Vector2, NetVector2>();
    netEvent1Field.InterpolationWait = false;
    this.jumpEvent = netEvent1Field;
    // ISSUE: explicit constructor call
    base.\u002Ector("Green Slime", position);
    if (Game1.random.NextBool())
      this.leftDrift.Value = true;
    this.Slipperiness = 4;
    this.readyToMate = Game1.random.Next(1000, 120000);
    int num = Game1.random.Next(200, 256 /*0x0100*/);
    this.color.Value = new Color(num / Game1.random.Next(2, 10), Game1.random.Next(180, 256 /*0x0100*/), Game1.random.NextDouble() < 0.1 ? (int) byte.MaxValue : (int) byte.MaxValue - num);
    this.firstGeneration.Value = true;
    this.flip = Game1.random.NextBool();
    this.cute.Value = Game1.random.NextDouble() < 0.49;
    this.HideShadow = true;
  }

  public GreenSlime(Vector2 position, int mineLevel)
  {
    NetEvent1Field<Vector2, NetVector2> netEvent1Field = new NetEvent1Field<Vector2, NetVector2>();
    netEvent1Field.InterpolationWait = false;
    this.jumpEvent = netEvent1Field;
    // ISSUE: explicit constructor call
    base.\u002Ector("Green Slime", position);
    this.randomStackOffset = Utility.RandomFloat(0.0f, 100f);
    this.cute.Value = Game1.random.NextDouble() < 0.49;
    this.flip = Game1.random.NextBool();
    this.specialNumber.Value = Game1.random.Next(100);
    if (mineLevel < 40)
    {
      this.parseMonsterInfo("Green Slime");
      int g = Game1.random.Next(200, 256 /*0x0100*/);
      this.color.Value = new Color(g / Game1.random.Next(2, 10), g, Game1.random.NextDouble() < 0.01 ? (int) byte.MaxValue : (int) byte.MaxValue - g);
      if (Game1.random.NextDouble() < 0.01 && mineLevel % 5 != 0 && mineLevel % 5 != 1)
      {
        this.color.Value = new Color(205, (int) byte.MaxValue, 0) * 0.7f;
        this.hasSpecialItem.Value = true;
        this.Health *= 3;
        this.DamageToFarmer *= 2;
      }
      if (Game1.random.NextDouble() < 0.01 && Game1.MasterPlayer.mailReceived.Contains("slimeHutchBuilt"))
        this.objectsToDrop.Add("680");
    }
    else if (mineLevel < 80 /*0x50*/)
    {
      this.Name = "Frost Jelly";
      this.parseMonsterInfo("Frost Jelly");
      int b = Game1.random.Next(200, 256 /*0x0100*/);
      this.color.Value = new Color(Game1.random.NextDouble() < 0.01 ? 180 : b / Game1.random.Next(2, 10), Game1.random.NextDouble() < 0.1 ? (int) byte.MaxValue : (int) byte.MaxValue - b / 3, b);
      if (Game1.random.NextDouble() < 0.01 && mineLevel % 5 != 0 && mineLevel % 5 != 1)
      {
        this.color.Value = new Color(0, 0, 0) * 0.7f;
        this.hasSpecialItem.Value = true;
        this.Health *= 3;
        this.DamageToFarmer *= 2;
      }
      if (Game1.random.NextDouble() < 0.01 && Game1.MasterPlayer.mailReceived.Contains("slimeHutchBuilt"))
        this.objectsToDrop.Add("413");
    }
    else if (mineLevel >= 77377 && mineLevel < 77387)
    {
      this.Name = "Sludge";
      this.parseMonsterInfo("Sludge");
    }
    else if (mineLevel > 120)
    {
      this.Name = "Sludge";
      this.parseMonsterInfo("Sludge");
      this.color.Value = Color.BlueViolet;
      this.Health *= 2;
      int r = (int) this.color.R;
      int g = (int) this.color.G;
      int b = (int) this.color.B;
      int val2_1 = r + Game1.random.Next(-20, 21);
      int val2_2 = g + Game1.random.Next(-20, 21);
      int val2_3 = b + Game1.random.Next(-20, 21);
      this.color.R = (byte) Math.Max(Math.Min((int) byte.MaxValue, val2_1), 0);
      this.color.G = (byte) Math.Max(Math.Min((int) byte.MaxValue, val2_2), 0);
      this.color.B = (byte) Math.Max(Math.Min((int) byte.MaxValue, val2_3), 0);
      while (Game1.random.NextDouble() < 0.08)
        this.objectsToDrop.Add("386");
      if (Game1.random.NextDouble() < 0.009)
        this.objectsToDrop.Add("337");
      if (Game1.random.NextDouble() < 0.01 && Game1.MasterPlayer.mailReceived.Contains("slimeHutchBuilt"))
        this.objectsToDrop.Add("439");
    }
    else
    {
      this.Name = "Sludge";
      this.parseMonsterInfo("Sludge");
      int r = Game1.random.Next(200, 256 /*0x0100*/);
      this.color.Value = new Color(r, Game1.random.NextDouble() < 0.01 ? (int) byte.MaxValue : (int) byte.MaxValue - r, r / Game1.random.Next(2, 10));
      if (Game1.random.NextDouble() < 0.01 && mineLevel % 5 != 0 && mineLevel % 5 != 1)
      {
        this.color.Value = new Color(50, 10, 50) * 0.7f;
        this.hasSpecialItem.Value = true;
        this.Health *= 3;
        this.DamageToFarmer *= 2;
      }
      if (Game1.random.NextDouble() < 0.01 && Game1.MasterPlayer.mailReceived.Contains("slimeHutchBuilt"))
        this.objectsToDrop.Add("437");
    }
    if (this.cute.Value)
    {
      this.Health += this.Health / 4;
      ++this.DamageToFarmer;
    }
    if (Game1.random.NextBool())
      this.leftDrift.Value = true;
    this.Slipperiness = 3;
    this.readyToMate = Game1.random.Next(1000, 120000);
    if (Game1.random.NextDouble() < 0.001)
    {
      this.color.Value = new Color((int) byte.MaxValue, (int) byte.MaxValue, 50);
      this.objectsToDrop.Add("GoldCoin");
      double num = Math.Min((double) (int) (Game1.stats.DaysPlayed / 28U) * 0.08, 0.55);
      while (Game1.random.NextDouble() < 0.1 + num)
        this.objectsToDrop.Add("GoldCoin");
    }
    if (mineLevel == 9999899)
    {
      this.color.Value = new Color(0, (int) byte.MaxValue, 200);
      this.Health *= 2;
      this.objectsToDrop.Clear();
      if (Game1.random.NextDouble() < 0.02)
        this.objectsToDrop.Add("394");
      if (Game1.random.NextDouble() < 0.02)
        this.objectsToDrop.Add("60");
      if (Game1.random.NextDouble() < 0.02)
        this.objectsToDrop.Add("62");
      if (Game1.random.NextDouble() < 0.01)
        this.objectsToDrop.Add("797");
      if (Game1.random.NextDouble() < 0.03 && Game1.MasterPlayer.mailReceived.Contains("slimeHutchBuilt"))
        this.objectsToDrop.Add("413");
      while (Game1.random.NextBool())
        this.objectsToDrop.Add("766");
    }
    this.firstGeneration.Value = true;
    this.HideShadow = true;
  }

  public GreenSlime(Vector2 position, Color color)
  {
    NetEvent1Field<Vector2, NetVector2> netEvent1Field = new NetEvent1Field<Vector2, NetVector2>();
    netEvent1Field.InterpolationWait = false;
    this.jumpEvent = netEvent1Field;
    // ISSUE: explicit constructor call
    base.\u002Ector("Green Slime", position);
    this.color.Value = color;
    this.firstGeneration.Value = true;
    this.HideShadow = true;
  }

  public void makeTigerSlime(bool onlyAppearance = false)
  {
    string name = this.Name;
    try
    {
      this.Name = "Tiger Slime";
      base.reloadSprite(false);
    }
    finally
    {
      if (onlyAppearance)
        this.Name = name;
    }
    this.Sprite.SpriteHeight = 24;
    this.Sprite.UpdateSourceRect();
    this.color.Value = Color.White;
    if (onlyAppearance)
      return;
    this.parseMonsterInfo("Tiger Slime");
  }

  public void makePrismatic()
  {
    this.prismatic.Value = true;
    this.Name = "Prismatic Slime";
    this.Health = 1000;
    this.damageToFarmer.Value = 35;
    this.hasSpecialItem.Value = false;
  }

  /// <inheritdoc />
  public override void reloadSprite(bool onlyAppearance = false)
  {
    if (this.Name == "Tiger Slime")
    {
      this.makeTigerSlime(onlyAppearance);
    }
    else
    {
      string str = this.name.Value;
      try
      {
        this.Name = "Green Slime";
        base.reloadSprite(onlyAppearance);
      }
      finally
      {
        this.Name = str;
      }
      this.Sprite.SpriteHeight = 24;
      this.Sprite.UpdateSourceRect();
      this.HideShadow = true;
    }
  }

  public virtual void OnAttacked(Vector2 trajectory)
  {
    if (!Game1.IsMasterGame || this.stackedSlimes.Value <= 0)
      return;
    --this.stackedSlimes.Value;
    if ((double) trajectory.LengthSquared() == 0.0)
      trajectory = new Vector2(0.0f, -1f);
    else
      trajectory.Normalize();
    trajectory *= 16f;
    BasicProjectile basicProjectile = new BasicProjectile(this.DamageToFarmer / 3 * 2, 13, 3, 0, 0.196349546f, trajectory.X, trajectory.Y, this.Position, explode: true, location: this.currentLocation, firer: (Character) this);
    basicProjectile.height.Value = 24f;
    basicProjectile.color.Value = this.color.Value;
    basicProjectile.ignoreMeleeAttacks.Value = true;
    basicProjectile.hostTimeUntilAttackable = 0.1f;
    if (Game1.random.NextBool())
      basicProjectile.debuff.Value = "13";
    this.currentLocation.projectiles.Add((Projectile) basicProjectile);
  }

  public override int takeDamage(
    int damage,
    int xTrajectory,
    int yTrajectory,
    bool isBomb,
    double addedPrecision,
    Farmer who)
  {
    if (this.stackedSlimes.Value > 0)
    {
      this.attackedEvent.Fire(new Vector2((float) xTrajectory, (float) -yTrajectory));
      xTrajectory = 0;
      yTrajectory = 0;
      damage = 1;
    }
    int damage1 = Math.Max(1, damage - this.resilience.Value);
    if (Game1.random.NextDouble() < this.missChance.Value - this.missChance.Value * addedPrecision)
    {
      damage1 = -1;
    }
    else
    {
      if (Game1.random.NextDouble() < 0.025 && this.cute.Value)
      {
        if (!this.focusedOnFarmers)
        {
          this.DamageToFarmer += this.DamageToFarmer / 2;
          this.shake(1000);
        }
        this.focusedOnFarmers = true;
      }
      this.Slipperiness = 3;
      this.Health -= damage1;
      this.setTrajectory(xTrajectory, yTrajectory);
      this.currentLocation.playSound("slimeHit");
      this.readyToJump = -1;
      this.IsWalkingTowardPlayer = true;
      if (this.Health <= 0)
      {
        this.currentLocation.playSound("slimedead");
        ++Game1.stats.SlimesKilled;
        if (this.mate != null)
          this.mate.mate = (GreenSlime) null;
        if (Game1.gameMode == (byte) 3 && (double) this.scale.Value > 1.7999999523162842)
        {
          this.Health = 10;
          int num = (double) this.scale.Value > 1.7999999523162842 ? Game1.random.Next(3, 5) : 1;
          this.Scale *= 0.6666667f;
          Rectangle boundingBox = this.GetBoundingBox();
          for (int index = 0; index < num; ++index)
          {
            GreenSlime greenSlime = new GreenSlime(this.Position + new Vector2((float) (index * boundingBox.Width), 0.0f), Game1.CurrentMineLevel);
            greenSlime.setTrajectory(xTrajectory + Game1.random.Next(-20, 20), yTrajectory + Game1.random.Next(-20, 20));
            greenSlime.willDestroyObjectsUnderfoot = false;
            greenSlime.moveTowardPlayer(4);
            greenSlime.Scale = (float) (0.75 + (double) Game1.random.Next(-5, 10) / 100.0);
            this.currentLocation.characters.Add((NPC) greenSlime);
          }
        }
        else
        {
          Game1.multiplayer.broadcastSprites(this.currentLocation, new TemporaryAnimatedSprite(44, this.Position, this.color.Value * 0.66f, 10)
          {
            interval = 70f,
            holdLastFrame = true,
            alphaFade = 0.01f
          });
          Game1.multiplayer.broadcastSprites(this.currentLocation, new TemporaryAnimatedSprite(44, this.Position + new Vector2(-16f, 0.0f), this.color.Value * 0.66f, 10)
          {
            interval = 70f,
            delayBeforeAnimationStart = 0,
            holdLastFrame = true,
            alphaFade = 0.01f
          });
          Game1.multiplayer.broadcastSprites(this.currentLocation, new TemporaryAnimatedSprite(44, this.Position + new Vector2(0.0f, 16f), this.color.Value * 0.66f, 10)
          {
            interval = 70f,
            delayBeforeAnimationStart = 100,
            holdLastFrame = true,
            alphaFade = 0.01f
          });
          Game1.multiplayer.broadcastSprites(this.currentLocation, new TemporaryAnimatedSprite(44, this.Position + new Vector2(16f, 0.0f), this.color.Value * 0.66f, 10)
          {
            interval = 70f,
            delayBeforeAnimationStart = 200,
            holdLastFrame = true,
            alphaFade = 0.01f
          });
        }
      }
    }
    return damage1;
  }

  public override void shedChunks(int number, float scale)
  {
    Point standingPixel = this.StandingPixel;
    Game1.createRadialDebris(this.currentLocation, this.Sprite.textureName.Value, new Rectangle(0, 120, 16 /*0x10*/, 16 /*0x10*/), 8, standingPixel.X + 32 /*0x20*/, standingPixel.Y, number, this.TilePoint.Y, this.color.Value, 4f * scale);
  }

  public override void collisionWithFarmerBehavior()
  {
    this.farmerPassesThrough = this.Player.isWearingRing("520");
  }

  public override void onDealContactDamage(Farmer who)
  {
    if (Game1.random.NextDouble() < 0.3 && this.Player == Game1.player && !this.Player.temporarilyInvincible && !this.Player.isWearingRing("520") && Game1.random.Next(11) >= who.Immunity && !this.Player.hasBuff("28") && !this.Player.hasTrinketWithID("BasiliskPaw"))
    {
      this.Player.applyBuff("13");
      this.currentLocation.playSound("slime");
    }
    base.onDealContactDamage(who);
  }

  public override void draw(SpriteBatch b)
  {
    if (this.IsInvisible || !Utility.isOnScreen(this.Position, 128 /*0x80*/))
      return;
    int height = this.GetBoundingBox().Height;
    int y1 = this.StandingPixel.Y;
    for (int index = 0; index <= this.stackedSlimes.Value; ++index)
    {
      bool flag = index == this.stackedSlimes.Value;
      Vector2 zero = Vector2.Zero;
      TimeSpan totalGameTime;
      if (this.stackedSlimes.Value > 0)
      {
        ref Vector2 local = ref zero;
        double randomStackOffset = (double) this.randomStackOffset;
        totalGameTime = Game1.currentGameTime.TotalGameTime;
        double num = totalGameTime.TotalSeconds * Math.PI * 2.0;
        double x = Math.Sin(randomStackOffset + num + (double) (index * 30)) * 8.0;
        double y2 = (double) (-30 * index);
        local = new Vector2((float) x, (float) y2);
      }
      b.Draw(this.Sprite.Texture, this.getLocalPosition(Game1.viewport) + new Vector2(32f, (float) (height / 2 + this.yOffset)) + zero, new Rectangle?(this.Sprite.SourceRect), this.prismatic.Value ? Utility.GetPrismaticColor(348 + this.specialNumber.Value, 5f) : this.color.Value, 0.0f, new Vector2(8f, 16f), 4f * Math.Max(0.2f, this.scale.Value - (float) (0.40000000596046448 * ((double) this.ageUntilFullGrown.Value / 120000.0))), SpriteEffects.None, Math.Max(0.0f, this.drawOnTop ? 0.991f : (float) (y1 + index * 2) / 10000f));
      b.Draw(Game1.shadowTexture, this.getLocalPosition(Game1.viewport) + new Vector2(32f, (float) ((double) (height / 2 * 7) / 4.0 + (double) this.yOffset + 8.0 * (double) this.scale.Value - (this.ageUntilFullGrown.Value > 0 ? 8.0 : 0.0))) + zero, new Rectangle?(Game1.shadowTexture.Bounds), Color.White, 0.0f, new Vector2((float) Game1.shadowTexture.Bounds.Center.X, (float) Game1.shadowTexture.Bounds.Center.Y), (float) (3.0 + (double) this.scale.Value - (double) this.ageUntilFullGrown.Value / 120000.0 - (this.Sprite.currentFrame % 4 % 3 != 0 || index != 0 ? 1.0 : 0.0) + (double) this.yOffset / 30.0), SpriteEffects.None, (float) (y1 - 1 + index * 2) / 10000f);
      if (this.ageUntilFullGrown.Value <= 0)
      {
        if (flag && (this.cute.Value || this.hasSpecialItem.Value))
        {
          int num1;
          if (!this.isMoving() && this.wagTimer <= 0)
          {
            num1 = 48 /*0x30*/;
          }
          else
          {
            int num2;
            if (this.wagTimer <= 0)
            {
              totalGameTime = Game1.currentGameTime.TotalGameTime;
              num2 = totalGameTime.Milliseconds % 992;
            }
            else
              num2 = 992 - this.wagTimer;
            num1 = 16 /*0x10*/ * Math.Min(7, Math.Abs(num2 - 496) / 62) % 64 /*0x40*/;
          }
          int x = num1;
          int num3;
          if (!this.isMoving() && this.wagTimer <= 0)
          {
            num3 = 24;
          }
          else
          {
            int num4;
            if (this.wagTimer <= 0)
            {
              totalGameTime = Game1.currentGameTime.TotalGameTime;
              num4 = totalGameTime.Milliseconds % 992;
            }
            else
              num4 = 992 - this.wagTimer;
            num3 = 24 * Math.Min(1, Math.Max(1, Math.Abs(num4 - 496) / 62) / 4);
          }
          int num5 = num3;
          if (this.hasSpecialItem.Value)
            num5 += 48 /*0x30*/;
          b.Draw(this.Sprite.Texture, this.getLocalPosition(Game1.viewport) + zero + new Vector2(32f, (float) (height - 16 /*0x10*/ + (this.readyToJump <= 0 ? 4 * (Math.Abs(this.Sprite.currentFrame % 4 - 2) - 2) : 4 + 4 * (this.Sprite.currentFrame % 4 % 3)) + this.yOffset)) * this.scale.Value, new Rectangle?(new Rectangle(x, 168 + num5, 16 /*0x10*/, 24)), this.hasSpecialItem.Value ? Color.White : this.color.Value, 0.0f, new Vector2(8f, 16f), 4f * Math.Max(0.2f, this.scale.Value - (float) (0.40000000596046448 * ((double) this.ageUntilFullGrown.Value / 120000.0))), this.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0.0f, this.drawOnTop ? 0.991f : (float) ((double) y1 / 10000.0 + 9.9999997473787516E-05)));
        }
        b.Draw(this.Sprite.Texture, this.getLocalPosition(Game1.viewport) + zero + (new Vector2(32f, (float) (height / 2 + (this.readyToJump <= 0 ? 4 * (Math.Abs(this.Sprite.currentFrame % 4 - 2) - 2) : 4 - 4 * (this.Sprite.currentFrame % 4 % 3)) + this.yOffset)) + this.facePosition.Value) * Math.Max(0.2f, this.scale.Value - (float) (0.40000000596046448 * ((double) this.ageUntilFullGrown.Value / 120000.0))), new Rectangle?(new Rectangle(32 /*0x20*/ + (this.readyToJump > 0 || this.focusedOnFarmers ? 16 /*0x10*/ : 0), 120 + (this.readyToJump >= 0 || !this.focusedOnFarmers && this.invincibleCountdown <= 0 ? 0 : 24), 16 /*0x10*/, 24)), Color.White * (this.FacingDirection == 0 ? 0.5f : 1f), 0.0f, new Vector2(8f, 16f), 4f * Math.Max(0.2f, this.scale.Value - (float) (0.40000000596046448 * ((double) this.ageUntilFullGrown.Value / 120000.0))), SpriteEffects.None, Math.Max(0.0f, this.drawOnTop ? 0.991f : (float) ((double) (y1 + index * 2) / 10000.0 + 9.9999997473787516E-05)));
      }
      if (this.isGlowing)
        b.Draw(this.Sprite.Texture, this.getLocalPosition(Game1.viewport) + zero + new Vector2(32f, (float) (height / 2 + this.yOffset)), new Rectangle?(this.Sprite.SourceRect), this.glowingColor * this.glowingTransparency, 0.0f, new Vector2(8f, 16f), 4f * Math.Max(0.2f, this.scale.Value), SpriteEffects.None, Math.Max(0.0f, this.drawOnTop ? 0.99f : (float) ((double) y1 / 10000.0 + 1.0 / 1000.0)));
    }
    if (this.pursuingMate.Value)
    {
      b.Draw(this.Sprite.Texture, this.getLocalPosition(Game1.viewport) + new Vector2(32f, (float) (this.yOffset - 32 /*0x20*/)), new Rectangle?(new Rectangle(16 /*0x10*/, 120, 8, 8)), Color.White, 0.0f, new Vector2(3f, 3f), 4f, SpriteEffects.None, Math.Max(0.0f, this.drawOnTop ? 0.991f : (float) this.StandingPixel.Y / 10000f));
    }
    else
    {
      if (!this.avoidingMate.Value)
        return;
      b.Draw(this.Sprite.Texture, this.getLocalPosition(Game1.viewport) + new Vector2(32f, (float) (this.yOffset - 32 /*0x20*/)), new Rectangle?(new Rectangle(24, 120, 8, 8)), Color.White, 0.0f, new Vector2(4f, 4f), 4f, SpriteEffects.None, Math.Max(0.0f, this.drawOnTop ? 0.991f : (float) this.StandingPixel.Y / 10000f));
    }
  }

  public void moveTowardOtherSlime(GreenSlime other, bool moveAway, GameTime time)
  {
    Point standingPixel1 = this.StandingPixel;
    Point standingPixel2 = other.StandingPixel;
    int num1 = Math.Abs(standingPixel2.X - standingPixel1.X);
    int num2 = Math.Abs(standingPixel2.Y - standingPixel1.Y);
    if (num1 > 4 || num2 > 4)
    {
      int num3 = standingPixel2.X > standingPixel1.X ? 1 : -1;
      int num4 = standingPixel2.Y > standingPixel1.Y ? 1 : -1;
      if (moveAway)
      {
        num3 = -num3;
        num4 = -num4;
      }
      double num5 = (double) num1 / (double) (num1 + num2);
      if (Game1.random.NextDouble() < num5)
        this.tryToMoveInDirection(num3 > 0 ? 1 : 3, false, this.DamageToFarmer, false);
      else
        this.tryToMoveInDirection(num4 > 0 ? 2 : 0, false, this.DamageToFarmer, false);
    }
    this.Sprite.AnimateDown(time);
    if (this.invincibleCountdown <= 0)
      return;
    this.invincibleCountdown -= time.ElapsedGameTime.Milliseconds;
    if (this.invincibleCountdown > 0)
      return;
    this.stopGlowing();
  }

  public void doneMating()
  {
    this.readyToMate = 120000;
    this.matingCountdown = 2000;
    this.mate = (GreenSlime) null;
    this.pursuingMate.Value = false;
    this.avoidingMate.Value = false;
  }

  public override void noMovementProgressNearPlayerBehavior()
  {
    this.faceGeneralDirection(this.Player.getStandingPosition());
  }

  public void mateWith(GreenSlime mateToPursue, GameLocation location)
  {
    if (location.canSlimeMateHere())
    {
      GreenSlime c = new GreenSlime(Vector2.Zero);
      Utility.recursiveFindPositionForCharacter((NPC) c, location, this.Tile, 30);
      Random random = Utility.CreateRandom((double) Game1.stats.DaysPlayed, (double) Game1.uniqueIDForThisGame / 10.0, (double) this.scale.Value * 100.0, (double) mateToPursue.scale.Value * 100.0);
      switch (random.Next(4))
      {
        case 0:
          c.color.Value = new Color(Math.Min((int) byte.MaxValue, Math.Max(0, (int) this.color.R + random.Next((int) ((double) -this.color.R * 0.25), (int) ((double) this.color.R * 0.25)))), Math.Min((int) byte.MaxValue, Math.Max(0, (int) this.color.G + random.Next((int) ((double) -this.color.G * 0.25), (int) ((double) this.color.G * 0.25)))), Math.Min((int) byte.MaxValue, Math.Max(0, (int) this.color.B + random.Next((int) ((double) -this.color.B * 0.25), (int) ((double) this.color.B * 0.25)))));
          break;
        case 1:
        case 2:
          c.color.Value = Utility.getBlendedColor(this.color.Value, mateToPursue.color.Value);
          break;
        case 3:
          c.color.Value = new Color(Math.Min((int) byte.MaxValue, Math.Max(0, (int) mateToPursue.color.R + random.Next((int) ((double) -mateToPursue.color.R * 0.25), (int) ((double) mateToPursue.color.R * 0.25)))), Math.Min((int) byte.MaxValue, Math.Max(0, (int) mateToPursue.color.G + random.Next((int) ((double) -mateToPursue.color.G * 0.25), (int) ((double) mateToPursue.color.G * 0.25)))), Math.Min((int) byte.MaxValue, Math.Max(0, (int) mateToPursue.color.B + random.Next((int) ((double) -mateToPursue.color.B * 0.25), (int) ((double) mateToPursue.color.B * 0.25)))));
          break;
      }
      int r = (int) c.color.R;
      int g = (int) c.color.G;
      int b = (int) c.color.B;
      c.Name = this.name.Value;
      if (c.Name == "Tiger Slime")
        c.makeTigerSlime();
      else if (r > 100 && b > 100 && g < 50)
      {
        c.parseMonsterInfo("Sludge");
        while (random.NextDouble() < 0.1)
          c.objectsToDrop.Add("386");
        if (random.NextDouble() < 0.01)
          c.objectsToDrop.Add("337");
      }
      else if (r >= 200 && g < 75)
        c.parseMonsterInfo("Sludge");
      else if (b >= 200 && r < 100)
        c.parseMonsterInfo("Frost Jelly");
      c.Health = random.Choose<int>(this.Health, mateToPursue.Health);
      c.Health = Math.Max(1, this.Health + random.Next(-4, 5));
      c.DamageToFarmer = random.Choose<int>(this.DamageToFarmer, mateToPursue.DamageToFarmer);
      c.DamageToFarmer = Math.Max(0, this.DamageToFarmer + random.Next(-1, 2));
      c.resilience.Value = random.Choose<int>(this.resilience.Value, mateToPursue.resilience.Value);
      c.resilience.Value = Math.Max(0, this.resilience.Value + random.Next(-1, 2));
      c.missChance.Value = random.Choose<double>(this.missChance.Value, mateToPursue.missChance.Value);
      c.missChance.Value = Math.Max(0.0, this.missChance.Value + (double) random.Next(-1, 2) / 100.0);
      c.Scale = random.Choose<float>(this.scale.Value, mateToPursue.scale.Value);
      c.Scale = Math.Max(0.6f, Math.Min(1.5f, this.scale.Value + (float) random.Next(-2, 3) / 100f));
      c.Slipperiness = 8;
      this.speed = random.Choose<int>(this.speed, mateToPursue.speed);
      if (random.NextDouble() < 0.015)
        this.speed = Math.Max(1, Math.Min(6, this.speed + random.Next(-1, 2)));
      c.setTrajectory(Utility.getAwayFromPositionTrajectory(c.GetBoundingBox(), this.getStandingPosition()) / 2f);
      c.ageUntilFullGrown.Value = 120000;
      c.Halt();
      c.firstGeneration.Value = false;
      if (Utility.isOnScreen(this.Position, 128 /*0x80*/))
        this.currentLocation.playSound("slime");
    }
    mateToPursue.doneMating();
    this.doneMating();
  }

  public override List<Item> getExtraDropItems()
  {
    List<Item> extraDropItems = new List<Item>();
    if (this.name.Value != "Tiger Slime")
    {
      if (this.color.R >= (byte) 50 && this.color.R <= (byte) 100 && this.color.G >= (byte) 25 && this.color.G <= (byte) 50 && this.color.B <= (byte) 25)
      {
        extraDropItems.Add(ItemRegistry.Create("(O)388", Game1.random.Next(3, 7)));
        if (Game1.random.NextDouble() < 0.1)
          extraDropItems.Add(ItemRegistry.Create("(O)709"));
      }
      else if (this.color.R < (byte) 80 /*0x50*/ && this.color.G < (byte) 80 /*0x50*/ && this.color.B < (byte) 80 /*0x50*/)
      {
        extraDropItems.Add(ItemRegistry.Create("(O)382"));
        Random random = Utility.CreateRandom((double) this.Position.X * 777.0, (double) this.Position.Y * 77.0, (double) Game1.stats.DaysPlayed);
        if (random.NextDouble() < 0.05)
          extraDropItems.Add(ItemRegistry.Create("(O)553"));
        if (random.NextDouble() < 0.05)
          extraDropItems.Add(ItemRegistry.Create("(O)539"));
      }
      else if (this.color.R > (byte) 200 && this.color.G > (byte) 180 && this.color.B < (byte) 50)
        extraDropItems.Add(ItemRegistry.Create("(O)384", 2));
      else if (this.color.R > (byte) 220 && this.color.G > (byte) 90 && this.color.G < (byte) 150 && this.color.B < (byte) 50)
        extraDropItems.Add(ItemRegistry.Create("(O)378", 2));
      else if (this.color.R > (byte) 230 && this.color.G > (byte) 230 && this.color.B > (byte) 230)
      {
        if ((int) this.color.R % 2 == 1)
        {
          extraDropItems.Add(ItemRegistry.Create("(O)338"));
          if ((int) this.color.G % 2 == 1)
            extraDropItems.Add(ItemRegistry.Create("(O)338"));
        }
        else
          extraDropItems.Add(ItemRegistry.Create("(O)380"));
        if ((int) this.color.R % 2 == 0 && (int) this.color.G % 2 == 0 && (int) this.color.B % 2 == 0 || this.color.Equals(Color.White))
          extraDropItems.Add((Item) new StardewValley.Object("72", 1));
      }
      else if (this.color.R > (byte) 150 && this.color.G > (byte) 150 && this.color.B > (byte) 150)
        extraDropItems.Add(ItemRegistry.Create("(O)390", 2));
      else if (this.color.R > (byte) 150 && this.color.B > (byte) 180 && this.color.G < (byte) 50 && this.specialNumber.Value % (this.firstGeneration.Value ? 4 : 2) == 0)
      {
        extraDropItems.Add(ItemRegistry.Create("(O)386", 2));
        if (this.firstGeneration.Value && Game1.random.NextDouble() < 0.005)
          extraDropItems.Add(ItemRegistry.Create("(O)485"));
      }
    }
    if (Game1.MasterPlayer.mailReceived.Contains("slimeHutchBuilt") && this.specialNumber.Value == 1)
    {
      switch (this.Name)
      {
        case "Green Slime":
          extraDropItems.Add(ItemRegistry.Create("(O)680"));
          break;
        case "Frost Jelly":
          extraDropItems.Add(ItemRegistry.Create("(O)413"));
          break;
        case "Tiger Slime":
          extraDropItems.Add(ItemRegistry.Create("(O)857"));
          break;
      }
    }
    if (this.Name == "Tiger Slime")
    {
      if (Game1.random.NextDouble() < 0.001)
        extraDropItems.Add(ItemRegistry.Create("(H)91"));
      if (Game1.random.NextDouble() < 0.1)
      {
        extraDropItems.Add(ItemRegistry.Create("(O)831"));
        while (Game1.random.NextBool())
          extraDropItems.Add(ItemRegistry.Create("(O)831"));
      }
      else if (Game1.random.NextDouble() < 0.1)
        extraDropItems.Add(ItemRegistry.Create("(O)829"));
      else if (Game1.random.NextDouble() < 0.02)
      {
        extraDropItems.Add(ItemRegistry.Create("(O)833"));
        while (Game1.random.NextBool())
          extraDropItems.Add(ItemRegistry.Create("(O)833"));
      }
      else if (Game1.random.NextDouble() < 0.006)
        extraDropItems.Add(ItemRegistry.Create("(O)835"));
    }
    if (!this.prismatic.Value || Game1.player.team.specialOrders.Where<SpecialOrder>((Func<SpecialOrder, bool>) (x => x.questKey.Value == "Wizard2")) == null)
      return extraDropItems;
    StardewValley.Object @object = ItemRegistry.Create<StardewValley.Object>("(O)876");
    @object.specialItem = true;
    @object.questItem.Value = true;
    return new List<Item>() { (Item) @object };
  }

  public override void dayUpdate(int dayOfMonth)
  {
    if (this.ageUntilFullGrown.Value > 0)
      this.ageUntilFullGrown.Value /= 2;
    if (this.readyToMate > 0)
      this.readyToMate /= 2;
    base.dayUpdate(dayOfMonth);
  }

  protected override void updateAnimation(GameTime time)
  {
    TimeSpan elapsedGameTime;
    if (this.wagTimer > 0)
    {
      int wagTimer = this.wagTimer;
      elapsedGameTime = time.ElapsedGameTime;
      int totalMilliseconds = (int) elapsedGameTime.TotalMilliseconds;
      this.wagTimer = wagTimer - totalMilliseconds;
    }
    if (this.stunTime.Value > 0)
    {
      this.yOffset = 0;
    }
    else
    {
      this.yOffset = Math.Max(this.yOffset - (int) Math.Abs(this.xVelocity + this.yVelocity) / 2, -64);
      if (this.yOffset < 0)
        this.yOffset = Math.Min(0, this.yOffset + 4 + (this.yOffset <= -64 ? (int) ((double) -this.yOffset / 8.0) : (int) ((double) -this.yOffset / 16.0)));
      int timeSinceLastJump = this.timeSinceLastJump;
      elapsedGameTime = time.ElapsedGameTime;
      int milliseconds = elapsedGameTime.Milliseconds;
      this.timeSinceLastJump = timeSinceLastJump + milliseconds;
    }
    if (Game1.random.NextDouble() < 0.01 && this.wagTimer <= 0)
      this.wagTimer = 992;
    if ((double) Math.Abs(this.xVelocity) >= 0.5 || (double) Math.Abs(this.yVelocity) >= 0.5)
      this.Sprite.AnimateDown(time);
    else if (!this.Position.Equals(this.lastPosition))
      this.animateTimer = 500;
    if (this.animateTimer > 0 && this.readyToJump <= 0)
    {
      int animateTimer = this.animateTimer;
      elapsedGameTime = time.ElapsedGameTime;
      int milliseconds = elapsedGameTime.Milliseconds;
      this.animateTimer = animateTimer - milliseconds;
      this.Sprite.AnimateDown(time);
    }
    this.resetAnimationSpeed();
  }

  public override void update(GameTime time, GameLocation location)
  {
    base.update(time, location);
    this.jumpEvent.Poll();
    this.attackedEvent.Poll();
  }

  public override void behaviorAtGameTick(GameTime time)
  {
    if (this.mate == null)
    {
      this.pursuingMate.Value = false;
      this.avoidingMate.Value = false;
    }
    switch (this.FacingDirection)
    {
      case 0:
        if ((double) this.facePosition.X > 0.0)
          this.facePosition.X -= 2f;
        else if ((double) this.facePosition.X < 0.0)
          this.facePosition.X += 2f;
        if ((double) this.facePosition.Y > -8.0)
        {
          this.facePosition.Y -= 2f;
          break;
        }
        break;
      case 1:
        if ((double) this.facePosition.X < 8.0)
          this.facePosition.X += 2f;
        if ((double) this.facePosition.Y < 0.0)
        {
          this.facePosition.Y += 2f;
          break;
        }
        break;
      case 2:
        if ((double) this.facePosition.X > 0.0)
          this.facePosition.X -= 2f;
        else if ((double) this.facePosition.X < 0.0)
          this.facePosition.X += 2f;
        if ((double) this.facePosition.Y < 0.0)
        {
          this.facePosition.Y += 2f;
          break;
        }
        break;
      case 3:
        if ((double) this.facePosition.X > -8.0)
          this.facePosition.X -= 2f;
        if ((double) this.facePosition.Y < 0.0)
        {
          this.facePosition.Y += 2f;
          break;
        }
        break;
    }
    TimeSpan elapsedGameTime;
    if (this.stackedSlimes.Value <= 0)
    {
      if (this.ageUntilFullGrown.Value <= 0)
      {
        int readyToMate = this.readyToMate;
        elapsedGameTime = time.ElapsedGameTime;
        int milliseconds = elapsedGameTime.Milliseconds;
        this.readyToMate = readyToMate - milliseconds;
      }
      else
        this.ageUntilFullGrown.Value -= time.ElapsedGameTime.Milliseconds;
    }
    if (this.pursuingMate.Value && this.mate != null)
    {
      if (this.readyToMate <= -35000)
      {
        this.mate.doneMating();
        this.doneMating();
      }
      else
      {
        this.moveTowardOtherSlime(this.mate, false, time);
        if (this.mate.mate != null && this.mate.pursuingMate.Value && !this.mate.mate.Equals((object) this))
        {
          this.doneMating();
        }
        else
        {
          Vector2 standingPosition1 = this.getStandingPosition();
          Vector2 standingPosition2 = this.mate.getStandingPosition();
          if ((double) Vector2.Distance(standingPosition1, standingPosition2) < (double) (this.GetBoundingBox().Width + 4))
          {
            if (this.mate.mate != null && this.mate.avoidingMate.Value && this.mate.mate.Equals((object) this))
            {
              this.mate.avoidingMate.Value = false;
              this.mate.matingCountdown = 2000;
              this.mate.pursuingMate.Value = true;
            }
            int matingCountdown = this.matingCountdown;
            elapsedGameTime = time.ElapsedGameTime;
            int milliseconds = elapsedGameTime.Milliseconds;
            this.matingCountdown = matingCountdown - milliseconds;
            if (this.currentLocation == null || this.matingCountdown > 0 || !this.pursuingMate.Value || this.currentLocation.isOutdoors.Value && Utility.getNumberOfCharactersInRadius(this.currentLocation, Utility.Vector2ToPoint(this.Position), 1) > 4)
              return;
            this.mateWith(this.mate, this.currentLocation);
          }
          else
          {
            if ((double) Vector2.Distance(standingPosition1, standingPosition2) <= (double) (GreenSlime.matingRange * 2))
              return;
            this.mate.mate = (GreenSlime) null;
            this.mate.avoidingMate.Value = false;
            this.mate = (GreenSlime) null;
          }
        }
      }
    }
    else if (this.avoidingMate.Value && this.mate != null)
    {
      this.moveTowardOtherSlime(this.mate, true, time);
    }
    else
    {
      if (this.readyToMate < 0 && this.cute.Value)
      {
        this.readyToMate = -1;
        if (Game1.random.NextDouble() < 0.001)
        {
          Point standingPixel = this.StandingPixel;
          GreenSlime greenSlime = (GreenSlime) Utility.checkForCharacterWithinArea(this.GetType(), this.Position, this.currentLocation, new Rectangle(standingPixel.X - GreenSlime.matingRange, standingPixel.Y - GreenSlime.matingRange, GreenSlime.matingRange * 2, GreenSlime.matingRange * 2));
          if (greenSlime != null && greenSlime.readyToMate <= 0 && !greenSlime.cute.Value && greenSlime.stackedSlimes.Value <= 0)
          {
            this.matingCountdown = 2000;
            this.mate = greenSlime;
            this.pursuingMate.Value = true;
            greenSlime.mate = this;
            greenSlime.avoidingMate.Value = true;
            this.addedSpeed = 1f;
            this.mate.addedSpeed = 1f;
            return;
          }
        }
      }
      else if (!this.isGlowing)
        this.addedSpeed = 0.0f;
      base.behaviorAtGameTick(time);
      if (this.readyToJump != -1)
      {
        this.Halt();
        this.IsWalkingTowardPlayer = false;
        int readyToJump = this.readyToJump;
        elapsedGameTime = time.ElapsedGameTime;
        int milliseconds = elapsedGameTime.Milliseconds;
        this.readyToJump = readyToJump - milliseconds;
        this.Sprite.currentFrame = 16 /*0x10*/ + (800 - this.readyToJump) / 200;
        if (this.readyToJump > 0)
          return;
        this.timeSinceLastJump = this.timeSinceLastJump;
        this.Slipperiness = 10;
        this.IsWalkingTowardPlayer = true;
        this.readyToJump = -1;
        this.invincibleCountdown = 0;
        Vector2 playerTrajectory = Utility.getAwayFromPlayerTrajectory(this.GetBoundingBox(), this.Player);
        playerTrajectory.X = (float) (-(double) playerTrajectory.X / 2.0);
        playerTrajectory.Y = (float) (-(double) playerTrajectory.Y / 2.0);
        this.jumpEvent.Fire(playerTrajectory);
        this.setTrajectory((int) playerTrajectory.X, (int) playerTrajectory.Y);
      }
      else if (Game1.random.NextDouble() < 0.1 && !this.focusedOnFarmers)
      {
        if (this.FacingDirection == 0 || this.FacingDirection == 2)
        {
          if (this.leftDrift.Value && !this.currentLocation.isCollidingPosition(this.nextPosition(3), Game1.viewport, false, 1, false, (Character) this))
            this.position.X -= (float) this.speed;
          else if (!this.leftDrift.Value && !this.currentLocation.isCollidingPosition(this.nextPosition(1), Game1.viewport, false, 1, false, (Character) this))
            this.position.X += (float) this.speed;
        }
        else if (this.leftDrift.Value && !this.currentLocation.isCollidingPosition(this.nextPosition(0), Game1.viewport, false, 1, false, (Character) this))
          this.position.Y -= (float) this.speed;
        else if (!this.leftDrift.Value && !this.currentLocation.isCollidingPosition(this.nextPosition(2), Game1.viewport, false, 1, false, (Character) this))
          this.position.Y += (float) this.speed;
        if (Game1.random.NextDouble() >= 0.08)
          return;
        this.leftDrift.Value = !this.leftDrift.Value;
      }
      else
      {
        if (!this.withinPlayerThreshold() || this.timeSinceLastJump <= (this.focusedOnFarmers ? 1000 : 4000) || Game1.random.NextDouble() >= 0.01 || this.stackedSlimes.Value > 0)
          return;
        if (this.Name.Equals("Frost Jelly") && Game1.random.NextDouble() < 0.25)
        {
          this.addedSpeed = 2f;
          this.startGlowing(Color.Cyan, false, 0.15f);
        }
        else
        {
          this.addedSpeed = 0.0f;
          this.stopGlowing();
          this.readyToJump = 800;
        }
      }
    }
  }

  private void doJump(Vector2 trajectory)
  {
    if (Utility.isOnScreen(this.Position, 128 /*0x80*/))
      this.currentLocation.localSound("slime");
    this.Sprite.currentFrame = 1;
  }
}
