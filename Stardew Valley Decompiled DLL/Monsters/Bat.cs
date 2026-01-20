// Decompiled with JetBrains decompiler
// Type: StardewValley.Monsters.Bat
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Enchantments;
using StardewValley.Extensions;
using StardewValley.Objects;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley.Monsters;

public class Bat : Monster
{
  public const float rotationIncrement = 0.0490873866f;
  [XmlIgnore]
  public readonly NetInt wasHitCounter = new NetInt(0);
  [XmlIgnore]
  public float targetRotation;
  [XmlIgnore]
  public readonly NetBool turningRight = new NetBool();
  [XmlIgnore]
  public readonly NetBool seenPlayer = new NetBool();
  public readonly NetBool cursedDoll = new NetBool();
  public readonly NetBool hauntedSkull = new NetBool();
  public readonly NetBool magmaSprite = new NetBool();
  public readonly NetBool canLunge = new NetBool();
  private ICue batFlap;
  private float extraVelocity;
  private float maxSpeed = 5f;
  public int lungeFrequency = 3000;
  public int lungeChargeTime = 500;
  public int lungeSpeed = 30;
  public int lungeDecelerationTicks = 60;
  public int nextLunge = -1;
  public int lungeTimer;
  public Vector2 lungeVelocity = Vector2.Zero;
  private List<Vector2> previousPositions = new List<Vector2>();

  public Bat()
  {
  }

  public Bat(Vector2 position)
    : base(nameof (Bat), position)
  {
    this.Slipperiness = 24 + Game1.random.Next(-10, 11);
    this.Halt();
    this.IsWalkingTowardPlayer = false;
    this.HideShadow = true;
  }

  public Bat(Vector2 position, int mineLevel)
    : base(nameof (Bat), position)
  {
    this.Slipperiness = 20 + Game1.random.Next(-5, 6);
    switch (mineLevel)
    {
      case -789:
        this.parseMonsterInfo("Iridium Bat");
        this.reloadSprite(false);
        this.extraVelocity = 1f;
        this.extraVelocity = 3f;
        this.maxSpeed = 4f;
        this.Health *= 2;
        this.shakeTimer = 100;
        this.cursedDoll.Value = true;
        this.objectsToDrop.Clear();
        this.Age = 789;
        break;
      case -666:
        this.parseMonsterInfo("Iridium Bat");
        this.reloadSprite(false);
        this.extraVelocity = 1f;
        this.extraVelocity = 3f;
        this.maxSpeed = 8f;
        this.Health *= 2;
        this.shakeTimer = 100;
        this.cursedDoll.Value = true;
        this.objectsToDrop.Clear();
        break;
      case -556:
        this.parseMonsterInfo("Magma Sparker");
        this.Name = "Magma Sparker";
        this.reloadSprite(false);
        this.extraVelocity = 2f;
        this.Slipperiness += 3;
        this.maxSpeed = (float) Game1.random.Next(6, 8);
        this.shakeTimer = 100;
        this.cursedDoll.Value = true;
        this.magmaSprite.Value = true;
        this.canLunge.Value = true;
        break;
      case -555:
        this.parseMonsterInfo("Magma Sprite");
        this.Name = "Magma Sprite";
        this.reloadSprite(false);
        this.Slipperiness *= 2;
        this.extraVelocity = 2f;
        this.maxSpeed = (float) Game1.random.Next(6, 9);
        this.shakeTimer = 100;
        this.cursedDoll.Value = true;
        this.magmaSprite.Value = true;
        break;
      case 77377:
        this.parseMonsterInfo("Lava Bat");
        this.Name = "Haunted Skull";
        this.reloadSprite(false);
        this.extraVelocity = 1f;
        this.extraVelocity = 3f;
        this.maxSpeed = 8f;
        this.shakeTimer = 100;
        this.cursedDoll.Value = true;
        this.hauntedSkull.Value = true;
        this.objectsToDrop.Clear();
        break;
      default:
        if (mineLevel >= 40 && mineLevel < 80 /*0x50*/)
        {
          this.Name = "Frost Bat";
          this.parseMonsterInfo("Frost Bat");
          this.reloadSprite(false);
          break;
        }
        if (mineLevel >= 80 /*0x50*/ && mineLevel < 171)
        {
          this.Name = "Lava Bat";
          this.parseMonsterInfo("Lava Bat");
          this.reloadSprite(false);
          break;
        }
        if (mineLevel >= 171)
        {
          this.Name = "Iridium Bat";
          this.parseMonsterInfo("Iridium Bat");
          this.reloadSprite(false);
          this.extraVelocity = 1f;
          break;
        }
        break;
    }
    if (mineLevel > 999)
    {
      this.extraVelocity = 3f;
      this.maxSpeed = 8f;
      this.Health *= 2;
      this.shakeTimer = 999999;
    }
    if (this.canLunge.Value)
      this.nextLunge = this.lungeFrequency;
    this.Halt();
    this.IsWalkingTowardPlayer = false;
    this.HideShadow = true;
  }

  protected override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.wasHitCounter, "wasHitCounter").AddField((INetSerializable) this.turningRight, "turningRight").AddField((INetSerializable) this.seenPlayer, "seenPlayer").AddField((INetSerializable) this.cursedDoll, "cursedDoll").AddField((INetSerializable) this.hauntedSkull, "hauntedSkull").AddField((INetSerializable) this.magmaSprite, "magmaSprite").AddField((INetSerializable) this.canLunge, "canLunge");
  }

  /// <inheritdoc />
  public override void reloadSprite(bool onlyAppearance = false)
  {
    if (this.Sprite == null)
      this.Sprite = new AnimatedSprite("Characters\\Monsters\\" + this.Name);
    else
      this.Sprite.textureName.Value = "Characters\\Monsters\\" + this.Name;
    this.HideShadow = true;
  }

  public override Debris ModifyMonsterLoot(Debris debris)
  {
    if (debris != null && this.magmaSprite.Value)
      debris.chunksMoveTowardPlayer = true;
    return debris;
  }

  public override List<Item> getExtraDropItems()
  {
    List<Item> extraDropItems = new List<Item>();
    if (this.cursedDoll.Value && Game1.random.NextDouble() < 0.1429 && this.hauntedSkull.Value)
    {
      switch (Game1.random.Next(11))
      {
        case 0:
          switch (Game1.random.Next(6))
          {
            case 0:
              Clothing clothing1 = ItemRegistry.Create<Clothing>("(P)10");
              clothing1.clothesColor.Value = Color.DimGray;
              extraDropItems.Add((Item) clothing1);
              break;
            case 1:
              extraDropItems.Add((Item) ItemRegistry.Create<Clothing>("(S)1004"));
              break;
            case 2:
              extraDropItems.Add((Item) ItemRegistry.Create<Clothing>("(S)1014"));
              break;
            case 3:
              extraDropItems.Add((Item) ItemRegistry.Create<Clothing>("(S)1263"));
              break;
            case 4:
              extraDropItems.Add((Item) ItemRegistry.Create<Clothing>("(S)1262"));
              break;
            case 5:
              Clothing clothing2 = ItemRegistry.Create<Clothing>("(P)12");
              clothing2.clothesColor.Value = Color.DimGray;
              extraDropItems.Add((Item) clothing2);
              break;
          }
          break;
        case 1:
          MeleeWeapon meleeWeapon = ItemRegistry.Create<MeleeWeapon>("(W)2");
          meleeWeapon.AddEnchantment((BaseEnchantment) new VampiricEnchantment());
          extraDropItems.Add((Item) meleeWeapon);
          break;
        case 2:
          extraDropItems.Add(ItemRegistry.Create("(O)288"));
          break;
        case 3:
          extraDropItems.Add((Item) new Ring("534"));
          break;
        case 4:
          extraDropItems.Add((Item) new Ring("531"));
          break;
        case 5:
          do
          {
            extraDropItems.Add(ItemRegistry.Create("(O)768"));
            extraDropItems.Add(ItemRegistry.Create("(O)769"));
          }
          while (Game1.random.NextDouble() < 0.33);
          break;
        case 6:
          extraDropItems.Add(ItemRegistry.Create("(O)581"));
          break;
        case 7:
          extraDropItems.Add(ItemRegistry.Create("(O)582"));
          break;
        case 8:
          extraDropItems.Add(ItemRegistry.Create("(O)725"));
          break;
        case 9:
          extraDropItems.Add(ItemRegistry.Create("(O)86"));
          break;
        case 10:
          if (Utility.doesMasterPlayerHaveMailReceivedButNotMailForTomorrow("ccVault"))
          {
            extraDropItems.Add(ItemRegistry.Create("(O)275"));
            break;
          }
          extraDropItems.Add(ItemRegistry.Create("(O)749"));
          break;
      }
      return extraDropItems;
    }
    if (this.hauntedSkull.Value && Game1.random.NextDouble() < 0.25 && Game1.IsWinter)
    {
      do
      {
        extraDropItems.Add(ItemRegistry.Create("(O)273"));
      }
      while (Game1.random.NextDouble() < 0.4);
    }
    if (this.hauntedSkull.Value && Game1.random.NextDouble() < 0.01)
      extraDropItems.Add(ItemRegistry.Create("(M)CursedMannequin" + (Game1.random.NextDouble() < 0.5 ? "Male" : "Female")));
    if (this.hauntedSkull.Value && Game1.random.NextDouble() < 0.001502)
      extraDropItems.Add(ItemRegistry.Create("(O)279"));
    return extraDropItems.Count > 0 ? extraDropItems : base.getExtraDropItems();
  }

  public override int takeDamage(
    int damage,
    int xTrajectory,
    int yTrajectory,
    bool isBomb,
    double addedPrecision,
    Farmer who)
  {
    if (this.Age == 789)
      return -1;
    this.lungeVelocity = Vector2.Zero;
    if (this.lungeTimer > 0)
    {
      this.nextLunge = this.lungeFrequency;
      this.lungeTimer = 0;
    }
    else if (this.nextLunge < 1000)
      this.nextLunge = 1000;
    int damage1 = Math.Max(1, damage - this.resilience.Value);
    this.seenPlayer.Value = true;
    if (Game1.random.NextDouble() < this.missChance.Value - this.missChance.Value * addedPrecision)
    {
      damage1 = -1;
    }
    else
    {
      this.Health -= damage1;
      this.setTrajectory(xTrajectory / 3, yTrajectory / 3);
      this.wasHitCounter.Value = 500;
      if (this.magmaSprite.Value)
        this.currentLocation.playSound("magma_sprite_hit");
      else
        this.currentLocation.playSound("hitEnemy");
      if (this.Health <= 0)
      {
        this.deathAnimation();
        if (!this.magmaSprite.Value)
          Game1.multiplayer.broadcastSprites(this.currentLocation, new TemporaryAnimatedSprite(44, this.Position, Color.DarkMagenta, 10));
        if (this.cursedDoll.Value)
        {
          Vector2 position = this.Position;
          if (this.magmaSprite.Value)
          {
            this.currentLocation.playSound("magma_sprite_die");
            for (int index = 0; index < 20; ++index)
              this.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("Characters\\Monsters\\Magma Sprite", new Rectangle(0, 64 /*0x40*/, 8, 8), position + new Vector2((float) Game1.random.Next(64 /*0x40*/), (float) Game1.random.Next(64 /*0x40*/)), false, 0.0f, Color.White)
              {
                scale = 4f,
                scaleChange = 0.0f,
                motion = new Vector2((float) Game1.random.Next(-30, 31 /*0x1F*/) / 10f, -6f),
                acceleration = new Vector2(0.0f, 0.25f),
                layerDepth = 0.9f,
                animationLength = 6,
                totalNumberOfLoops = 2,
                interval = 60f,
                delayBeforeAnimationStart = index * 10
              });
            Utility.addSmokePuff(this.currentLocation, position, baseScale: 4f, scaleChange: 0.01f, alpha: 1f, alphaFade: 0.01f);
            Utility.addSmokePuff(this.currentLocation, position + new Vector2(32f, 16f), 400, 4f, 0.01f, 1f, 0.02f);
            Utility.addSmokePuff(this.currentLocation, position + new Vector2(-32f, -16f), 200, 4f, 0.01f, 1f, 0.02f);
            Utility.addSmokePuff(this.currentLocation, position + new Vector2(0.0f, 32f), 200, 4f, 0.01f, 1f, 0.01f);
            Utility.addSmokePuff(this.currentLocation, position, baseScale: 3f, scaleChange: 0.01f, alpha: 1f, alphaFade: 0.02f);
            Utility.addSmokePuff(this.currentLocation, position + new Vector2(21f, 16f), 500, 3f, 0.01f, 1f, 0.01f);
            Utility.addSmokePuff(this.currentLocation, position + new Vector2(-32f, -21f), 100, 3f, 0.01f, 1f, 0.02f);
            Utility.addSmokePuff(this.currentLocation, position + new Vector2(0.0f, 32f), 250, 3f, 0.01f, 1f, 0.01f);
          }
          else
            this.currentLocation.playSound("rockGolemHit");
          if (this.hauntedSkull.Value)
            Game1.multiplayer.broadcastSprites(who.currentLocation, new TemporaryAnimatedSprite(this.Sprite.textureName.Value, new Rectangle(0, 32 /*0x20*/, 16 /*0x10*/, 16 /*0x10*/), 2000f, 1, 9999, position, false, false, 1f, 0.02f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
            {
              motion = new Vector2((float) xTrajectory / 4f, (float) Game1.random.Next(-12, -7)),
              acceleration = new Vector2(0.0f, 0.4f),
              rotationChange = (float) Game1.random.Next(-200, 200) / 1000f
            });
          else if (who != null && !this.magmaSprite.Value)
            Game1.multiplayer.broadcastSprites(who.currentLocation, new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(388, 1894, 24, 22), 40f, 6, 9999, position, false, true, 1f, 0.0f, Color.Black * 0.67f, 4f, 0.0f, 0.0f, 0.0f)
            {
              motion = new Vector2(8f, -4f)
            });
        }
        else
          this.currentLocation.playSound("batScreech");
      }
    }
    this.addedSpeed = (float) Game1.random.Next(-1, 1);
    return damage1;
  }

  public override void shedChunks(int number, float scale)
  {
    Point standingPixel = this.StandingPixel;
    if (this.cursedDoll.Value && this.hauntedSkull.Value)
      Game1.createRadialDebris(this.currentLocation, this.Sprite.textureName.Value, new Rectangle(0, 64 /*0x40*/, 16 /*0x10*/, 16 /*0x10*/), 8, standingPixel.X, standingPixel.Y, number, this.TilePoint.Y, Color.White, 4f);
    else
      Game1.createRadialDebris(this.currentLocation, this.Sprite.textureName.Value, new Rectangle(0, 384, 64 /*0x40*/, 64 /*0x40*/), 32 /*0x20*/, standingPixel.X, standingPixel.Y, number, this.TilePoint.Y, Color.White, scale);
  }

  public override void onDealContactDamage(Farmer who)
  {
    base.onDealContactDamage(who);
    if (!this.magmaSprite.Value || Game1.random.NextDouble() >= 0.5 || !this.name.Equals((object) "Magma Sparker") || Game1.random.Next(11) < who.Immunity || who.hasBuff("28") || who.hasTrinketWithID("BasiliskPaw"))
      return;
    who.applyBuff("12");
  }

  public override void drawAboveAllLayers(SpriteBatch b)
  {
    if (!Utility.isOnScreen(this.Position, 128 /*0x80*/))
      return;
    if (this.cursedDoll.Value)
    {
      if (this.hauntedSkull.Value)
      {
        Vector2 vector2_1 = Vector2.Zero;
        if (this.previousPositions.Count > 2)
          vector2_1 = this.Position - this.previousPositions[1];
        int num = (double) Math.Abs(vector2_1.X) > (double) Math.Abs(vector2_1.Y) ? ((double) vector2_1.X > 0.0 ? 1 : 3) : ((double) vector2_1.Y < 0.0 ? 0 : 2);
        if (num == -1)
          num = 2;
        Vector2 vector2_2 = new Vector2(0.0f, (float) (8.0 * Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / (60.0 * Math.PI))));
        b.Draw(Game1.shadowTexture, this.getLocalPosition(Game1.viewport) + new Vector2(32f, 64f), new Rectangle?(Game1.shadowTexture.Bounds), Color.White, 0.0f, new Vector2((float) Game1.shadowTexture.Bounds.Center.X, (float) Game1.shadowTexture.Bounds.Center.Y), (float) (3.0 + (double) vector2_2.Y / 20.0), SpriteEffects.None, 0.0001f);
        b.Draw(this.Sprite.Texture, this.getLocalPosition(Game1.viewport) + new Vector2((float) (32 /*0x20*/ + Game1.random.Next(-6, 7)), (float) (32 /*0x20*/ + Game1.random.Next(-6, 7))) + vector2_2, new Rectangle?(Game1.getSourceRectForStandardTileSheet(this.Sprite.Texture, num * 2 + (!this.seenPlayer.Value || Game1.currentGameTime.TotalGameTime.TotalMilliseconds % 500.0 >= 250.0 ? 0 : 1), 16 /*0x10*/, 16 /*0x10*/)), Color.Red * 0.44f, 0.0f, new Vector2(8f, 16f), Math.Max(0.2f, this.scale.Value) * 4f, this.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, (float) (((double) this.position.Y + 128.0 - 1.0) / 10000.0));
        b.Draw(this.Sprite.Texture, this.getLocalPosition(Game1.viewport) + new Vector2((float) (32 /*0x20*/ + Game1.random.Next(-6, 7)), (float) (32 /*0x20*/ + Game1.random.Next(-6, 7))) + vector2_2, new Rectangle?(Game1.getSourceRectForStandardTileSheet(this.Sprite.Texture, num * 2 + (!this.seenPlayer.Value || Game1.currentGameTime.TotalGameTime.TotalMilliseconds % 500.0 >= 250.0 ? 0 : 1), 16 /*0x10*/, 16 /*0x10*/)), Color.Yellow * 0.44f, 0.0f, new Vector2(8f, 16f), Math.Max(0.2f, this.scale.Value) * 4f, this.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, (float) (((double) this.position.Y + 128.0) / 10000.0));
        if (this.seenPlayer.Value)
        {
          for (int index = this.previousPositions.Count - 1; index >= 0; index -= 2)
            b.Draw(this.Sprite.Texture, new Vector2(this.previousPositions[index].X - (float) Game1.viewport.X, this.previousPositions[index].Y - (float) Game1.viewport.Y + (float) this.yJumpOffset) + this.drawOffset + new Vector2(32f, 32f) + vector2_2, new Rectangle?(Game1.getSourceRectForStandardTileSheet(this.Sprite.Texture, num * 2 + (!this.seenPlayer.Value || Game1.currentGameTime.TotalGameTime.TotalMilliseconds % 500.0 >= 250.0 ? 0 : 1), 16 /*0x10*/, 16 /*0x10*/)), Color.White * (float) (0.0 + 0.125 * (double) index), 0.0f, new Vector2(8f, 16f), Math.Max(0.2f, this.scale.Value) * 4f, this.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, (float) (((double) this.position.Y + 128.0 - (double) index) / 10000.0));
        }
        b.Draw(this.Sprite.Texture, this.getLocalPosition(Game1.viewport) + new Vector2(32f, 32f) + vector2_2, new Rectangle?(Game1.getSourceRectForStandardTileSheet(this.Sprite.Texture, num * 2 + (!this.seenPlayer.Value || Game1.currentGameTime.TotalGameTime.TotalMilliseconds % 500.0 >= 250.0 ? 0 : 1), 16 /*0x10*/, 16 /*0x10*/)), Color.White, 0.0f, new Vector2(8f, 16f), Math.Max(0.2f, this.scale.Value) * 4f, this.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, (float) (((double) this.position.Y + 128.0 + 1.0) / 10000.0));
      }
      else if (this.magmaSprite.Value)
      {
        Vector2 vector2_3 = Vector2.Zero;
        if (this.previousPositions.Count > 2)
          vector2_3 = this.Position - this.previousPositions[1];
        int num = (double) Math.Abs(vector2_3.X) > (double) Math.Abs(vector2_3.Y) ? ((double) vector2_3.X > 0.0 ? 1 : 3) : ((double) vector2_3.Y < 0.0 ? 0 : 2);
        if (num == -1)
          num = 2;
        Vector2 vector2_4 = new Vector2(0.0f, (float) (8.0 * Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / (60.0 * Math.PI))));
        b.Draw(Game1.shadowTexture, this.getLocalPosition(Game1.viewport) + new Vector2(32f, 64f), new Rectangle?(Game1.shadowTexture.Bounds), Color.White, 0.0f, new Vector2((float) Game1.shadowTexture.Bounds.Center.X, (float) Game1.shadowTexture.Bounds.Center.Y), (float) (3.0 + (double) vector2_4.Y / 20.0), SpriteEffects.None, 0.0001f);
        b.Draw(this.Sprite.Texture, this.getLocalPosition(Game1.viewport) + new Vector2((float) (32 /*0x20*/ + Game1.random.Next(-6, 7)), (float) (32 /*0x20*/ + Game1.random.Next(-6, 7))) + vector2_4, new Rectangle?(Game1.getSourceRectForStandardTileSheet(this.Sprite.Texture, num * 7 + (int) (Game1.currentGameTime.TotalGameTime.TotalMilliseconds % 490.0 / 70.0), 16 /*0x10*/, 16 /*0x10*/)), Color.Red * 0.44f, 0.0f, new Vector2(8f, 16f), Math.Max(0.2f, this.scale.Value) * 4f, this.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0.9955f);
        b.Draw(this.Sprite.Texture, this.getLocalPosition(Game1.viewport) + new Vector2((float) (32 /*0x20*/ + Game1.random.Next(-6, 7)), (float) (32 /*0x20*/ + Game1.random.Next(-6, 7))) + vector2_4, new Rectangle?(Game1.getSourceRectForStandardTileSheet(this.Sprite.Texture, num * 7 + (int) (Game1.currentGameTime.TotalGameTime.TotalMilliseconds % 490.0 / 70.0), 16 /*0x10*/, 16 /*0x10*/)), Color.Yellow * 0.44f, 0.0f, new Vector2(8f, 16f), Math.Max(0.2f, this.scale.Value) * 4f, this.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0.9975f);
        for (int index = this.previousPositions.Count - 1; index >= 0; index -= 2)
          b.Draw(this.Sprite.Texture, new Vector2(this.previousPositions[index].X - (float) Game1.viewport.X, this.previousPositions[index].Y - (float) Game1.viewport.Y + (float) this.yJumpOffset) + this.drawOffset + new Vector2(32f, 32f) + vector2_4, new Rectangle?(Game1.getSourceRectForStandardTileSheet(this.Sprite.Texture, num * 7 + (int) (Game1.currentGameTime.TotalGameTime.TotalMilliseconds % 490.0 / 70.0), 16 /*0x10*/, 16 /*0x10*/)), Color.White * (float) (0.0 + 0.125 * (double) index), 0.0f, new Vector2(8f, 16f), Math.Max(0.2f, this.scale.Value) * 4f, this.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0.9985f);
        b.Draw(this.Sprite.Texture, this.getLocalPosition(Game1.viewport) + new Vector2(32f, 32f) + vector2_4, new Rectangle?(Game1.getSourceRectForStandardTileSheet(this.Sprite.Texture, num * 7 + (int) (Game1.currentGameTime.TotalGameTime.TotalMilliseconds % 490.0 / 70.0), 16 /*0x10*/, 16 /*0x10*/)), Color.White, 0.0f, new Vector2(8f, 16f), Math.Max(0.2f, this.scale.Value) * 4f, this.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 1f);
      }
      else
      {
        int tilePosition = 103;
        if (this.Age == 789)
          tilePosition = 789;
        Vector2 vector2 = new Vector2(0.0f, (float) (8.0 * Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / (60.0 * Math.PI))));
        b.Draw(Game1.shadowTexture, this.getLocalPosition(Game1.viewport) + new Vector2(32f, 64f), new Rectangle?(Game1.shadowTexture.Bounds), Color.White, 0.0f, new Vector2((float) Game1.shadowTexture.Bounds.Center.X, (float) Game1.shadowTexture.Bounds.Center.Y), (float) (3.0 + (double) vector2.Y / 20.0), SpriteEffects.None, 0.0001f);
        b.Draw(Game1.objectSpriteSheet, this.getLocalPosition(Game1.viewport) + new Vector2((float) (32 /*0x20*/ + Game1.random.Next(-6, 7)), (float) (32 /*0x20*/ + Game1.random.Next(-6, 7))) + vector2, new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, tilePosition, 16 /*0x10*/, 16 /*0x10*/)), Color.Violet * 0.44f, 0.0f, new Vector2(8f, 16f), Math.Max(0.2f, this.scale.Value) * 4f, this.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, (float) (((double) this.position.Y + 128.0 - 1.0) / 10000.0));
        b.Draw(Game1.objectSpriteSheet, this.getLocalPosition(Game1.viewport) + new Vector2((float) (32 /*0x20*/ + Game1.random.Next(-6, 7)), (float) (32 /*0x20*/ + Game1.random.Next(-6, 7))) + vector2, new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, tilePosition, 16 /*0x10*/, 16 /*0x10*/)), Color.Lime * 0.44f, 0.0f, new Vector2(8f, 16f), Math.Max(0.2f, this.scale.Value) * 4f, this.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, (float) (((double) this.position.Y + 128.0) / 10000.0));
        b.Draw(Game1.objectSpriteSheet, this.getLocalPosition(Game1.viewport) + new Vector2(32f, 32f) + vector2, new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, tilePosition, 16 /*0x10*/, 16 /*0x10*/)), tilePosition == 789 ? Color.White : new Color((int) byte.MaxValue, 50, 50), 0.0f, new Vector2(8f, 16f), Math.Max(0.2f, this.scale.Value) * 4f, this.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, (float) (((double) this.position.Y + 128.0 + 1.0) / 10000.0));
      }
    }
    else
    {
      int y1 = this.StandingPixel.Y;
      b.Draw(this.Sprite.Texture, this.getLocalPosition(Game1.viewport) + new Vector2(32f, 32f), new Rectangle?(this.Sprite.SourceRect), this.shakeTimer > 0 ? Color.Red : Color.White, 0.0f, new Vector2(8f, 16f), Math.Max(0.2f, this.scale.Value) * 4f, this.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0.92f);
      SpriteBatch spriteBatch = b;
      Texture2D shadowTexture = Game1.shadowTexture;
      Vector2 position = this.getLocalPosition(Game1.viewport) + new Vector2(32f, 64f);
      Rectangle? sourceRectangle = new Rectangle?(Game1.shadowTexture.Bounds);
      Color white = Color.White;
      Rectangle bounds = Game1.shadowTexture.Bounds;
      double x = (double) bounds.Center.X;
      bounds = Game1.shadowTexture.Bounds;
      double y2 = (double) bounds.Center.Y;
      Vector2 origin = new Vector2((float) x, (float) y2);
      double layerDepth = this.wildernessFarmMonster ? 9.9999997473787516E-05 : (double) (y1 - 1) / 10000.0;
      spriteBatch.Draw(shadowTexture, position, sourceRectangle, white, 0.0f, origin, 4f, SpriteEffects.None, (float) layerDepth);
      if (!this.isGlowing)
        return;
      b.Draw(this.Sprite.Texture, this.getLocalPosition(Game1.viewport) + new Vector2(32f, 32f), new Rectangle?(this.Sprite.SourceRect), this.glowingColor * this.glowingTransparency, 0.0f, new Vector2(8f, 16f), Math.Max(0.2f, this.scale.Value) * 4f, this.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0.0f, this.drawOnTop ? 0.99f : (float) ((double) y1 / 10000.0 + 1.0 / 1000.0)));
    }
  }

  public override void behaviorAtGameTick(GameTime time)
  {
    base.behaviorAtGameTick(time);
    TimeSpan elapsedGameTime;
    if (this.wasHitCounter.Value >= 0)
    {
      NetInt wasHitCounter = this.wasHitCounter;
      int num = wasHitCounter.Value;
      elapsedGameTime = time.ElapsedGameTime;
      int milliseconds = elapsedGameTime.Milliseconds;
      wasHitCounter.Value = num - milliseconds;
    }
    if (double.IsNaN((double) this.xVelocity) || double.IsNaN((double) this.yVelocity) || (double) this.Position.X < -2000.0 || (double) this.Position.Y < -2000.0)
      this.Health = -500;
    if ((double) this.Position.X <= -640.0 || (double) this.Position.Y <= -640.0 || (double) this.Position.X >= (double) (this.currentLocation.Map.Layers[0].LayerWidth * 64 /*0x40*/ + 640) || (double) this.Position.Y >= (double) (this.currentLocation.Map.Layers[0].LayerHeight * 64 /*0x40*/ + 640))
      this.Health = -500;
    if (this.canLunge.Value)
    {
      if (this.nextLunge == -1)
        this.nextLunge = this.lungeFrequency;
      if ((double) this.lungeVelocity.LengthSquared() > 0.0)
      {
        float delta = (float) this.lungeSpeed / (float) this.lungeDecelerationTicks;
        this.lungeVelocity = new Vector2(Utility.MoveTowards(this.lungeVelocity.X, 0.0f, delta), Utility.MoveTowards(this.lungeVelocity.Y, 0.0f, delta));
        this.xVelocity = this.lungeVelocity.X;
        this.yVelocity = -this.lungeVelocity.Y;
        if ((double) this.lungeVelocity.LengthSquared() != 0.0)
          return;
        this.xVelocity = 0.0f;
        this.yVelocity = 0.0f;
        return;
      }
      if (this.lungeTimer > 0)
      {
        int lungeTimer = this.lungeTimer;
        elapsedGameTime = time.ElapsedGameTime;
        int totalMilliseconds = (int) elapsedGameTime.TotalMilliseconds;
        this.lungeTimer = lungeTimer - totalMilliseconds;
        Vector2 vector2 = Utility.PointToVector2(this.Player.StandingPixel) - Utility.PointToVector2(this.StandingPixel);
        if ((double) vector2.LengthSquared() == 0.0)
          vector2 = new Vector2(1f, 0.0f);
        vector2.Normalize();
        if (this.lungeTimer < 0)
        {
          this.lungeVelocity = vector2 * 25f;
          this.lungeTimer = 0;
          this.nextLunge = this.lungeFrequency;
        }
        this.xVelocity = vector2.X * 0.5f;
        this.yVelocity = (float) (-(double) vector2.Y * 0.5);
      }
      else if (this.nextLunge > 0 && this.withinPlayerThreshold(6))
      {
        int nextLunge = this.nextLunge;
        elapsedGameTime = time.ElapsedGameTime;
        int totalMilliseconds = (int) elapsedGameTime.TotalMilliseconds;
        this.nextLunge = nextLunge - totalMilliseconds;
        if (this.nextLunge < 0)
        {
          this.currentLocation.playSound("magma_sprite_spot");
          this.nextLunge = 0;
          this.lungeTimer = this.lungeChargeTime;
          return;
        }
      }
    }
    if (!this.focusedOnFarmers && !this.withinPlayerThreshold(6) && !this.seenPlayer.Value)
      return;
    if (this.magmaSprite.Value && !this.seenPlayer.Value)
      this.currentLocation.playSound("magma_sprite_spot");
    this.seenPlayer.Value = true;
    if (this.invincibleCountdown > 0)
    {
      if (!this.Name.Equals("Lava Bat"))
        return;
      this.glowingColor = Color.Cyan;
    }
    else
    {
      Point standingPixel1 = this.StandingPixel;
      Point standingPixel2 = this.Player.StandingPixel;
      float num1 = (float) -(standingPixel2.X - standingPixel1.X);
      float num2 = (float) (standingPixel2.Y - standingPixel1.Y);
      float num3 = Math.Max(1f, Math.Abs(num1) + Math.Abs(num2));
      if ((double) num3 < ((double) this.extraVelocity > 0.0 ? 192.0 : 64.0))
      {
        this.xVelocity = Math.Max(-this.maxSpeed, Math.Min(this.maxSpeed, this.xVelocity * 1.05f));
        this.yVelocity = Math.Max(-this.maxSpeed, Math.Min(this.maxSpeed, this.yVelocity * 1.05f));
      }
      float x = num1 / num3;
      float num4 = num2 / num3;
      if (this.wasHitCounter.Value <= 0)
      {
        this.targetRotation = (float) Math.Atan2(-(double) num4, (double) x) - 1.57079637f;
        if ((double) Math.Abs(this.targetRotation) - (double) Math.Abs(this.rotation) > 7.0 * Math.PI / 8.0 && Game1.random.NextBool())
          this.turningRight.Value = true;
        else if ((double) Math.Abs(this.targetRotation) - (double) Math.Abs(this.rotation) < Math.PI / 8.0)
          this.turningRight.Value = false;
        if (this.turningRight.Value)
          this.rotation -= (float) Math.Sign(this.targetRotation - this.rotation) * ((float) Math.PI / 64f);
        else
          this.rotation += (float) Math.Sign(this.targetRotation - this.rotation) * ((float) Math.PI / 64f);
        this.rotation %= 6.28318548f;
        this.wasHitCounter.Value = 0;
      }
      float num5 = Math.Min(5f, Math.Max(1f, (float) (5.0 - (double) num3 / 64.0 / 2.0))) + this.extraVelocity;
      float num6 = (float) Math.Cos((double) this.rotation + Math.PI / 2.0);
      float num7 = -(float) Math.Sin((double) this.rotation + Math.PI / 2.0);
      this.xVelocity += (float) (-(double) num6 * (double) num5 / 6.0 + (double) Game1.random.Next(-10, 10) / 100.0);
      this.yVelocity += (float) (-(double) num7 * (double) num5 / 6.0 + (double) Game1.random.Next(-10, 10) / 100.0);
      if ((double) Math.Abs(this.xVelocity) > (double) Math.Abs(-num6 * this.maxSpeed))
        this.xVelocity -= (float) (-(double) num6 * (double) num5 / 6.0);
      if ((double) Math.Abs(this.yVelocity) <= (double) Math.Abs(-num7 * this.maxSpeed))
        return;
      this.yVelocity -= (float) (-(double) num7 * (double) num5 / 6.0);
    }
  }

  protected override void updateAnimation(GameTime time)
  {
    if (this.focusedOnFarmers || this.withinPlayerThreshold(6) || this.seenPlayer.Value || this.magmaSprite.Value)
    {
      this.Sprite.Animate(time, 0, 4, 80f);
      if (this.Sprite.currentFrame % 3 == 0 && Utility.isOnScreen(this.Position, 512 /*0x0200*/) && (this.batFlap == null || !this.batFlap.IsPlaying) && this.currentLocation == Game1.currentLocation && !this.cursedDoll.Value)
        Game1.playSound("batFlap", out this.batFlap);
      if (this.cursedDoll.Value)
      {
        this.shakeTimer -= time.ElapsedGameTime.Milliseconds;
        if (this.shakeTimer < 0)
        {
          this.shakeTimer = 50;
          if (this.magmaSprite.Value)
          {
            this.shakeTimer = this.lungeTimer > 0 ? 50 : 100;
            this.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("Characters\\Monsters\\Magma Sprite", new Rectangle(0, 64 /*0x40*/, 8, 8), this.Position + new Vector2((float) Game1.random.Next(32 /*0x20*/), (float) (-16 - Game1.random.Next(32 /*0x20*/))), false, 0.0f, Color.White)
            {
              scale = 4f,
              scaleChange = -0.05f,
              motion = new Vector2(this.lungeTimer > 0 ? (float) Game1.random.Next(-30, 31 /*0x1F*/) / 10f : 0.0f, (float) (-(double) this.maxSpeed / (this.lungeTimer > 0 ? 2.0 : 8.0))),
              layerDepth = 0.9f,
              animationLength = 6,
              totalNumberOfLoops = 1,
              interval = 50f,
              xPeriodic = this.lungeTimer <= 0,
              xPeriodicLoopTime = (float) Game1.random.Next(500, 800),
              xPeriodicRange = (float) (4 * (this.lungeTimer > 0 ? 2 : 1))
            });
          }
          else if (!this.hauntedSkull.Value)
            this.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("Maps\\springobjects", Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, this.Age == 789 ? 789 : 103, 16 /*0x10*/, 16 /*0x10*/), this.Position + new Vector2(0.0f, -32f), false, 0.1f, new Color((int) byte.MaxValue, 50, (int) byte.MaxValue) * 0.8f)
            {
              scale = 4f
            });
        }
        this.previousPositions.Add(this.Position);
        if (this.previousPositions.Count > 8)
          this.previousPositions.RemoveAt(0);
      }
    }
    else
    {
      this.Sprite.currentFrame = 4;
      this.Halt();
    }
    this.resetAnimationSpeed();
  }
}
