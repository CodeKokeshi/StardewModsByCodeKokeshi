// Decompiled with JetBrains decompiler
// Type: StardewValley.Monsters.Duggy
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Extensions;
using System;
using xTile.Layers;

#nullable disable
namespace StardewValley.Monsters;

public class Duggy : Monster
{
  public Duggy() => this.HideShadow = true;

  public Duggy(Vector2 position)
    : base(nameof (Duggy), position)
  {
    this.IsWalkingTowardPlayer = false;
    this.IsInvisible = true;
    this.DamageToFarmer = 0;
    this.Sprite.currentFrame = 0;
    this.HideShadow = true;
  }

  public Duggy(Vector2 position, bool magmaDuggy)
    : base("Magma Duggy", position)
  {
    this.IsWalkingTowardPlayer = false;
    this.IsInvisible = true;
    this.DamageToFarmer = 0;
    this.Sprite.currentFrame = 0;
    this.HideShadow = true;
  }

  protected override void initNetFields()
  {
    base.initNetFields();
    this.position.Field.Interpolated(false, true);
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
      this.currentLocation.playSound("hitEnemy");
      if (this.Health <= 0)
        this.deathAnimation();
    }
    return damage1;
  }

  protected override void localDeathAnimation()
  {
    this.currentLocation.localSound("monsterdead");
    Utility.makeTemporarySpriteJuicier(new TemporaryAnimatedSprite(44, this.Position, Color.DarkRed, 10)
    {
      holdLastFrame = true,
      alphaFade = 0.01f,
      interval = 70f
    }, this.currentLocation);
  }

  protected override void sharedDeathAnimation()
  {
  }

  public override void update(GameTime time, GameLocation location)
  {
    if (this.invincibleCountdown > 0)
    {
      this.glowingColor = Color.Cyan;
      this.invincibleCountdown -= time.ElapsedGameTime.Milliseconds;
      if (this.invincibleCountdown <= 0)
        this.stopGlowing();
    }
    if (!location.farmers.Any())
      return;
    this.behaviorAtGameTick(time);
    Layer layer = location.map.RequireLayer("Back");
    if ((double) this.Position.X < 0.0 || (double) this.Position.X > (double) (layer.LayerWidth * 64 /*0x40*/) || (double) this.Position.Y < 0.0 || (double) this.Position.Y > (double) (layer.LayerHeight * 64 /*0x40*/))
      location.characters.Remove((NPC) this);
    this.updateGlow();
    if (this.stunTime.Value <= 0)
      return;
    this.stunTime.Value -= (int) time.ElapsedGameTime.TotalMilliseconds;
  }

  public override void draw(SpriteBatch b)
  {
    if (this.IsInvisible || !Utility.isOnScreen(this.Position, 128 /*0x80*/))
      return;
    Rectangle boundingBox = this.GetBoundingBox();
    int y = this.StandingPixel.Y;
    b.Draw(this.Sprite.Texture, this.getLocalPosition(Game1.viewport) + new Vector2(32f, (float) (boundingBox.Height / 2 + this.yJumpOffset)), new Rectangle?(this.Sprite.SourceRect), Color.White, this.rotation, new Vector2(8f, 16f), Math.Max(0.2f, this.scale.Value) * 4f, this.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0.0f, this.drawOnTop ? 0.991f : (float) y / 10000f));
    if (!this.isGlowing)
      return;
    b.Draw(this.Sprite.Texture, this.getLocalPosition(Game1.viewport) + new Vector2(32f, (float) (boundingBox.Height / 2 + this.yJumpOffset)), new Rectangle?(this.Sprite.SourceRect), this.glowingColor * this.glowingTransparency, this.rotation, new Vector2(8f, 16f), Math.Max(0.2f, this.scale.Value) * 4f, this.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0.0f, this.drawOnTop ? 0.991f : (float) ((double) y / 10000.0 + 1.0 / 1000.0)));
  }

  public override void behaviorAtGameTick(GameTime time)
  {
    base.behaviorAtGameTick(time);
    this.isEmoting = false;
    this.Sprite.loop = false;
    if (this.stunTime.Value > 0)
      return;
    Rectangle boundingBox = this.GetBoundingBox();
    if (this.Sprite.currentFrame < 4)
    {
      boundingBox.Inflate(128 /*0x80*/, 128 /*0x80*/);
      if (!this.IsInvisible || boundingBox.Contains(this.Player.StandingPixel))
      {
        if (this.IsInvisible)
        {
          xTile.Tiles.Tile tile = this.currentLocation.map.RequireLayer("Back").Tiles[this.Player.TilePoint.X, this.Player.TilePoint.Y];
          if (tile.Properties.ContainsKey("NPCBarrier") || !tile.TileIndexProperties.ContainsKey("Diggable") && tile.TileIndex != 0)
            return;
          this.Position = new Vector2(this.Player.Position.X, this.Player.Position.Y + (float) this.Player.Sprite.SpriteHeight - (float) this.Sprite.SpriteHeight);
          this.currentLocation.localSound(nameof (Duggy));
          this.Position = this.Player.Tile * 64f;
        }
        this.IsInvisible = false;
        this.Sprite.interval = 100f;
        this.Sprite.AnimateDown(time);
      }
    }
    if (this.Sprite.currentFrame >= 4 && this.Sprite.currentFrame < 8)
    {
      boundingBox.Inflate((int) sbyte.MinValue, (int) sbyte.MinValue);
      this.currentLocation.isCollidingPosition(boundingBox, Game1.viewport, false, 8, false, (Character) this);
      this.Sprite.AnimateRight(time);
      this.Sprite.interval = 220f;
      this.DamageToFarmer = 8;
    }
    if (this.Sprite.currentFrame >= 8)
      this.Sprite.AnimateUp(time);
    if (this.Sprite.currentFrame < 10)
      return;
    this.IsInvisible = true;
    this.Sprite.currentFrame = 0;
    Point tilePoint = this.TilePoint;
    this.currentLocation.map.RequireLayer("Back").Tiles[tilePoint.X, tilePoint.Y].TileIndex = 0;
    this.currentLocation.removeObjectsAndSpawned(tilePoint.X, tilePoint.Y, 1, 1);
    this.DamageToFarmer = 0;
  }
}
