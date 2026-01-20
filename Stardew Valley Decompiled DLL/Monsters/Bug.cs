// Decompiled with JetBrains decompiler
// Type: StardewValley.Monsters.Bug
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Enchantments;
using StardewValley.Locations;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley.Monsters;

public class Bug : Monster
{
  [XmlElement("isArmoredBug")]
  public readonly NetBool isArmoredBug = new NetBool(false);

  public Bug()
  {
  }

  public Bug(Vector2 position, int facingDirection, string specialType)
    : this(position, 0)
  {
    this.faceDirection(facingDirection);
    if (!specialType.Contains("Assassin"))
      return;
    this.Sprite.LoadTexture("Characters\\Monsters\\Assassin Bug");
    this.DamageToFarmer = 50;
    this.Health = 500;
    ++this.speed;
  }

  public Bug(Vector2 position, int areaType)
    : base(nameof (Bug), position)
  {
    this.Sprite.SpriteHeight = 16 /*0x10*/;
    this.Sprite.UpdateSourceRect();
    this.onCollision = new Monster.collisionBehavior(this.collide);
    this.yOffset = -32f;
    this.IsWalkingTowardPlayer = false;
    this.setMovingInFacingDirection();
    this.defaultAnimationInterval.Value = 40;
    this.collidesWithOtherCharacters.Value = false;
    if (areaType == 121)
    {
      this.isArmoredBug.Value = true;
      this.Sprite.LoadTexture("Characters\\Monsters\\Armored Bug");
      this.DamageToFarmer *= 2;
      this.Slipperiness = -1;
      this.Health = 150;
    }
    this.HideShadow = true;
  }

  public Bug(Vector2 position, int facingDirection, MineShaft mine)
    : this(position, mine.getMineArea())
  {
    this.faceDirection(facingDirection);
    this.HideShadow = true;
  }

  protected override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.isArmoredBug, "isArmoredBug");
  }

  protected override void updateMonsterSlaveAnimation(GameTime time)
  {
    this.Sprite.faceDirection(this.FacingDirection);
    this.Sprite.animateOnce(time);
  }

  /// <inheritdoc />
  public override void reloadSprite(bool onlyAppearance = false)
  {
    base.reloadSprite(onlyAppearance);
    this.Sprite.SpriteHeight = 16 /*0x10*/;
    this.Sprite.UpdateSourceRect();
  }

  private void collide(GameLocation location)
  {
    Rectangle rectangle = this.nextPosition(this.FacingDirection);
    foreach (Character farmer in location.farmers)
    {
      if (farmer.GetBoundingBox().Intersects(rectangle))
        return;
    }
    this.FacingDirection = (this.FacingDirection + 2) % 4;
    this.setMovingInFacingDirection();
  }

  public override void BuffForAdditionalDifficulty(int additional_difficulty)
  {
    this.FacingDirection = Math.Abs((this.FacingDirection + Game1.random.Next(-1, 2)) % 4);
    this.Halt();
    this.setMovingInFacingDirection();
    base.BuffForAdditionalDifficulty(additional_difficulty);
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
    if (this.isArmoredBug.Value && (isBomb || !(who.CurrentTool is MeleeWeapon currentTool) || !currentTool.hasEnchantmentOfType<BugKillerEnchantment>()))
    {
      this.currentLocation.playSound("crafting");
      return 0;
    }
    if (Game1.random.NextDouble() < this.missChance.Value - this.missChance.Value * addedPrecision)
    {
      damage1 = -1;
    }
    else
    {
      this.Health -= damage1;
      this.currentLocation.playSound("hitEnemy");
      this.setTrajectory(xTrajectory / 3, yTrajectory / 3);
      if (this.isHardModeMonster.Value)
      {
        this.FacingDirection = Math.Abs((this.FacingDirection + Game1.random.Next(-1, 2)) % 4);
        this.Halt();
        this.setMovingInFacingDirection();
      }
      if (this.Health <= 0)
        this.deathAnimation();
    }
    return damage1;
  }

  public override List<Item> getExtraDropItems()
  {
    if (!this.isArmoredBug.Value)
      return base.getExtraDropItems();
    List<Item> extraDropItems = new List<Item>();
    if (Game1.random.NextDouble() <= 0.1)
      extraDropItems.Add(ItemRegistry.Create("(O)874"));
    return extraDropItems;
  }

  public override void draw(SpriteBatch b)
  {
    if (this.IsInvisible || !Utility.isOnScreen(this.Position, 128 /*0x80*/))
      return;
    Vector2 vector2 = new Vector2();
    if (this.FacingDirection % 2 == 0)
      vector2.X = (float) (Math.Sin((double) Game1.currentGameTime.TotalGameTime.Milliseconds / 1000.0 * (2.0 * Math.PI)) * 10.0);
    else
      vector2.Y = (float) (Math.Sin((double) Game1.currentGameTime.TotalGameTime.Milliseconds / 1000.0 * (2.0 * Math.PI)) * 10.0);
    int y = this.StandingPixel.Y;
    b.Draw(Game1.shadowTexture, this.getLocalPosition(Game1.viewport) + new Vector2((float) (this.Sprite.SpriteWidth * 4) / 2f + vector2.X, (float) (this.GetBoundingBox().Height * 5 / 2 - 48 /*0x30*/)), new Rectangle?(Game1.shadowTexture.Bounds), Color.White, 0.0f, Utility.PointToVector2(Game1.shadowTexture.Bounds.Center), (float) (4.0 + (double) this.yJumpOffset / 40.0) * this.scale.Value, SpriteEffects.None, Math.Max(0.0f, (float) y / 10000f) - 1E-06f);
    b.Draw(this.Sprite.Texture, this.getLocalPosition(Game1.viewport) + new Vector2(32f, (float) this.yJumpOffset) + vector2, new Rectangle?(this.Sprite.SourceRect), Color.White, this.rotation, new Vector2(8f, 16f), 4f, this.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0.0f, this.drawOnTop ? 0.991f : (float) y / 10000f));
  }

  protected override void localDeathAnimation()
  {
    base.localDeathAnimation();
    this.currentLocation.localSound("slimedead");
    Utility.makeTemporarySpriteJuicier(new TemporaryAnimatedSprite(44, this.Position + new Vector2(0.0f, -32f), Color.Violet, 10)
    {
      holdLastFrame = true,
      alphaFade = 0.01f,
      interval = 70f
    }, this.currentLocation);
  }

  public override void shedChunks(int number, float scale)
  {
    Point standingPixel = this.StandingPixel;
    Game1.createRadialDebris(this.currentLocation, this.Sprite.textureName.Value, new Rectangle(0, this.Sprite.getHeight() * 4, 16 /*0x10*/, 16 /*0x10*/), 8, standingPixel.X, standingPixel.Y, number, this.TilePoint.Y, Color.White, 4f);
  }
}
