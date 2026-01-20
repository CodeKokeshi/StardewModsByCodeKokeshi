// Decompiled with JetBrains decompiler
// Type: StardewValley.Monsters.BigSlime
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Extensions;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley.Monsters;

public class BigSlime : Monster
{
  [XmlElement("c")]
  public readonly NetColor c = new NetColor();
  [XmlElement("heldObject")]
  public readonly NetRef<Item> heldItem = new NetRef<Item>();
  private float heldObjectBobTimer;

  public BigSlime()
  {
  }

  public BigSlime(Vector2 position, MineShaft mine)
    : this(position, mine.getMineArea())
  {
    this.Sprite.ignoreStopAnimation = true;
    this.ignoreMovementAnimations = true;
    this.HideShadow = true;
  }

  public BigSlime(Vector2 position, int mineArea)
    : base("Big Slime", position)
  {
    this.ignoreMovementAnimations = true;
    this.Sprite.ignoreStopAnimation = true;
    this.Sprite.SpriteWidth = 32 /*0x20*/;
    this.Sprite.SpriteHeight = 32 /*0x20*/;
    this.Sprite.UpdateSourceRect();
    this.Sprite.framesPerAnimation = 8;
    this.c.Value = Color.White;
    switch (mineArea)
    {
      case 0:
      case 10:
        this.c.Value = Color.Lime;
        break;
      case 40:
        this.c.Value = Color.Turquoise;
        this.Health *= 2;
        this.ExperienceGained *= 2;
        break;
      case 80 /*0x50*/:
        this.c.Value = Color.Red;
        this.Health *= 3;
        this.DamageToFarmer *= 2;
        this.ExperienceGained *= 3;
        break;
      case 121:
        this.c.Value = Color.BlueViolet;
        this.Health *= 4;
        this.DamageToFarmer *= 3;
        this.ExperienceGained *= 3;
        break;
    }
    int r = (int) this.c.R;
    int g = (int) this.c.G;
    int b = (int) this.c.B;
    int val2_1 = r + Game1.random.Next(-20, 21);
    int val2_2 = g + Game1.random.Next(-20, 21);
    int val2_3 = b + Game1.random.Next(-20, 21);
    this.c.R = (byte) Math.Max(Math.Min((int) byte.MaxValue, val2_1), 0);
    this.c.G = (byte) Math.Max(Math.Min((int) byte.MaxValue, val2_2), 0);
    this.c.B = (byte) Math.Max(Math.Min((int) byte.MaxValue, val2_3), 0);
    NetColor c = this.c;
    c.Value = c.Value * ((float) Game1.random.Next(7, 11) / 10f);
    this.Sprite.interval = 300f;
    this.HideShadow = true;
    if (Game1.random.NextDouble() < 0.01 && mineArea >= 40)
      this.heldItem.Value = ItemRegistry.Create("(O)221");
    if (Game1.mine != null && Game1.mine.GetAdditionalDifficulty() > 0)
    {
      if (Game1.random.NextDouble() < 0.1)
        this.heldItem.Value = ItemRegistry.Create("(O)858");
      else if (Game1.random.NextDouble() < 0.005)
        this.heldItem.Value = ItemRegistry.Create("(O)896");
    }
    if (!Game1.random.NextBool() || !Game1.player.team.SpecialOrderRuleActive("SC_NO_FOOD"))
      return;
    this.heldItem.Value = ItemRegistry.Create("(O)930");
  }

  protected override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.c, "c").AddField((INetSerializable) this.heldItem, "heldItem");
  }

  /// <inheritdoc />
  public override void reloadSprite(bool onlyAppearance = false)
  {
    base.reloadSprite(onlyAppearance);
    this.Sprite.SpriteWidth = 32 /*0x20*/;
    this.Sprite.SpriteHeight = 32 /*0x20*/;
    this.Sprite.interval = 300f;
    this.Sprite.ignoreStopAnimation = true;
    this.ignoreMovementAnimations = true;
    this.HideShadow = true;
    this.Sprite.UpdateSourceRect();
    this.Sprite.framesPerAnimation = 8;
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
      this.Slipperiness = 3;
      this.Health -= damage1;
      this.setTrajectory(xTrajectory, yTrajectory);
      this.currentLocation.playSound("hitEnemy");
      this.IsWalkingTowardPlayer = true;
      if (this.Health <= 0)
      {
        this.deathAnimation();
        ++Game1.stats.SlimesKilled;
        if (Game1.gameMode == (byte) 3 && Game1.random.NextDouble() < 0.75)
        {
          int num = Game1.random.Next(2, 5);
          for (int index = 0; index < num; ++index)
          {
            this.currentLocation.characters.Add((NPC) new GreenSlime(this.Position, Game1.CurrentMineLevel));
            this.currentLocation.characters[this.currentLocation.characters.Count - 1].setTrajectory(xTrajectory / 8 + Game1.random.Next(-2, 3), yTrajectory / 8 + Game1.random.Next(-2, 3));
            this.currentLocation.characters[this.currentLocation.characters.Count - 1].willDestroyObjectsUnderfoot = false;
            this.currentLocation.characters[this.currentLocation.characters.Count - 1].moveTowardPlayer(4);
            this.currentLocation.characters[this.currentLocation.characters.Count - 1].Scale = (float) (0.75 + (double) Game1.random.Next(-5, 10) / 100.0);
            this.currentLocation.characters[this.currentLocation.characters.Count - 1].currentLocation = this.currentLocation;
          }
        }
      }
    }
    return damage1;
  }

  protected override void localDeathAnimation()
  {
    this.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(44, this.Position, this.c.Value, 10, animationInterval: 70f));
    this.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(44, this.Position + new Vector2(-32f, 0.0f), this.c.Value, 10, animationInterval: 70f)
    {
      delayBeforeAnimationStart = 100
    });
    this.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(44, this.Position + new Vector2(32f, 0.0f), this.c.Value, 10, animationInterval: 70f)
    {
      delayBeforeAnimationStart = 200
    });
    this.currentLocation.localSound("slimedead");
    this.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(44, this.Position + new Vector2(0.0f, -32f), this.c.Value, 10)
    {
      delayBeforeAnimationStart = 300
    });
  }

  protected override void updateAnimation(GameTime time)
  {
    int currentFrame = this.Sprite.currentFrame;
    this.Sprite.AnimateDown(time);
    if (this.isMoving())
    {
      this.Sprite.interval = 100f;
      this.heldObjectBobTimer += (float) (time.ElapsedGameTime.TotalMilliseconds * 0.007853982038795948);
    }
    else
    {
      this.Sprite.interval = 200f;
      this.heldObjectBobTimer += (float) (time.ElapsedGameTime.TotalMilliseconds * (Math.PI / 800.0));
    }
    if (!Utility.isOnScreen(this.Position, 128 /*0x80*/) || this.Sprite.currentFrame != 0 || currentFrame != 7)
      return;
    this.currentLocation.localSound("slimeHit");
  }

  public override List<Item> getExtraDropItems()
  {
    if (this.heldItem.Value == null)
      return base.getExtraDropItems();
    return new List<Item>() { this.heldItem.Value };
  }

  public override void draw(SpriteBatch b)
  {
    if (this.IsInvisible || !Utility.isOnScreen(this.Position, 128 /*0x80*/))
      return;
    int y = this.StandingPixel.Y;
    this.heldItem.Value?.drawInMenu(b, this.getLocalPosition(Game1.viewport) + new Vector2(28f, (float) (Math.Sin((double) this.heldObjectBobTimer + 1.0) * 4.0 - 16.0)), 1f, 1f, (float) (y - 1) / 10000f, StackDrawType.Hide, Color.White, false);
    b.Draw(this.Sprite.Texture, this.getLocalPosition(Game1.viewport) + new Vector2(56f, (float) (16 /*0x10*/ + this.yJumpOffset)), new Rectangle?(this.Sprite.SourceRect), this.c.Value, this.rotation, new Vector2(16f, 16f), Math.Max(0.2f, this.scale.Value) * 4f, this.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0.0f, this.drawOnTop ? 0.991f : (float) y / 10000f));
    if (!this.isGlowing)
      return;
    b.Draw(this.Sprite.Texture, this.getLocalPosition(Game1.viewport) + new Vector2(56f, (float) (16 /*0x10*/ + this.yJumpOffset)), new Rectangle?(this.Sprite.SourceRect), this.glowingColor * this.glowingTransparency, 0.0f, new Vector2(16f, 16f), 4f * Math.Max(0.2f, this.scale.Value), this.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0.0f, this.drawOnTop ? 0.991f : (float) ((double) y / 10000.0 + 1.0 / 1000.0)));
  }

  public override Rectangle GetBoundingBox()
  {
    Vector2 position = this.Position;
    return new Rectangle((int) position.X + 8, (int) position.Y, this.Sprite.SpriteWidth * 4 * 3 / 4, 64 /*0x40*/);
  }

  public override void shedChunks(int number, float scale)
  {
  }
}
