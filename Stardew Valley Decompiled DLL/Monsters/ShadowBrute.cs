// Decompiled with JetBrains decompiler
// Type: StardewValley.Monsters.ShadowBrute
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;

#nullable disable
namespace StardewValley.Monsters;

public class ShadowBrute : Monster
{
  public ShadowBrute()
  {
  }

  public ShadowBrute(Vector2 position)
    : base("Shadow Brute", position)
  {
    this.Sprite.SpriteHeight = 32 /*0x20*/;
    this.Sprite.UpdateSourceRect();
  }

  /// <inheritdoc />
  public override void reloadSprite(bool onlyAppearance = false)
  {
    this.Sprite = new AnimatedSprite("Characters\\Monsters\\Shadow Brute");
    this.Sprite.SpriteHeight = 32 /*0x20*/;
    this.Sprite.UpdateSourceRect();
  }

  public override int takeDamage(
    int damage,
    int xTrajectory,
    int yTrajectory,
    bool isBomb,
    double addedPrecision,
    Farmer who)
  {
    this.currentLocation.playSound("shadowHit");
    return base.takeDamage(damage, xTrajectory, yTrajectory, isBomb, addedPrecision, who);
  }

  protected override void localDeathAnimation()
  {
    Utility.makeTemporarySpriteJuicier(new TemporaryAnimatedSprite(45, this.Position, Color.White, 10), this.currentLocation);
    for (int index = 1; index < 3; ++index)
    {
      this.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(6, this.Position + new Vector2(0.0f, 1f) * 64f * (float) index, Color.Gray * 0.75f, 10)
      {
        delayBeforeAnimationStart = index * 159
      });
      this.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(6, this.Position + new Vector2(0.0f, -1f) * 64f * (float) index, Color.Gray * 0.75f, 10)
      {
        delayBeforeAnimationStart = index * 159
      });
      this.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(6, this.Position + new Vector2(1f, 0.0f) * 64f * (float) index, Color.Gray * 0.75f, 10)
      {
        delayBeforeAnimationStart = index * 159
      });
      this.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(6, this.Position + new Vector2(-1f, 0.0f) * 64f * (float) index, Color.Gray * 0.75f, 10)
      {
        delayBeforeAnimationStart = index * 159
      });
    }
    this.currentLocation.localSound("shadowDie");
  }

  protected override void sharedDeathAnimation()
  {
    Point standingPixel = this.StandingPixel;
    Game1.createRadialDebris(this.currentLocation, this.Sprite.textureName.Value, new Rectangle(this.Sprite.SourceRect.X, this.Sprite.SourceRect.Y, 16 /*0x10*/, 5), 16 /*0x10*/, standingPixel.X, standingPixel.Y - 32 /*0x20*/, 1, standingPixel.Y / 64 /*0x40*/, Color.White, 4f);
    Game1.createRadialDebris(this.currentLocation, this.Sprite.textureName.Value, new Rectangle(this.Sprite.SourceRect.X + 2, this.Sprite.SourceRect.Y + 5, 16 /*0x10*/, 5), 10, standingPixel.X, standingPixel.Y - 32 /*0x20*/, 1, standingPixel.Y / 64 /*0x40*/, Color.White, 4f);
  }

  protected override void updateMonsterSlaveAnimation(GameTime time)
  {
    if (!this.isMoving())
      return;
    switch (this.FacingDirection)
    {
      case 0:
        this.Sprite.AnimateUp(time);
        break;
      case 1:
        this.Sprite.AnimateRight(time);
        break;
      case 2:
        this.Sprite.AnimateDown(time);
        break;
      case 3:
        this.Sprite.AnimateLeft(time);
        break;
    }
  }
}
