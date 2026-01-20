// Decompiled with JetBrains decompiler
// Type: StardewValley.Monsters.AngryRoger
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Extensions;
using System;
using System.Xml.Serialization;
using xTile.Dimensions;
using xTile.Layers;

#nullable disable
namespace StardewValley.Monsters;

public class AngryRoger : Monster
{
  public const float rotationIncrement = 0.0490873866f;
  [XmlIgnore]
  public int wasHitCounter;
  [XmlIgnore]
  public float targetRotation;
  [XmlIgnore]
  public bool turningRight;
  [XmlIgnore]
  public int identifier = Game1.random.Next(-99999, 99999);
  [XmlIgnore]
  public int yOffset;
  [XmlIgnore]
  public int yOffsetExtra;
  public string lightSourceId;

  public AngryRoger() => this.lightSourceId = this.GenerateLightSourceId(this.identifier);

  public AngryRoger(Vector2 position)
    : base("Ghost", position)
  {
    this.Slipperiness = 8;
    this.isGlider.Value = true;
    this.HideShadow = true;
    this.lightSourceId = this.GenerateLightSourceId(this.identifier);
  }

  /// <summary>constructor for non-default ghosts</summary>
  /// <param name="position"></param>
  /// <param name="name"></param>
  public AngryRoger(Vector2 position, string name)
    : base(name, position)
  {
    this.Slipperiness = 8;
    this.isGlider.Value = true;
    this.HideShadow = true;
  }

  /// <inheritdoc />
  public override void reloadSprite(bool onlyAppearance = false)
  {
    this.Sprite = new AnimatedSprite("Characters\\Monsters\\" + this.name.Value);
  }

  public override void drawAboveAllLayers(SpriteBatch b)
  {
    int y1 = this.StandingPixel.Y;
    b.Draw(this.Sprite.Texture, this.getLocalPosition(Game1.viewport) + new Vector2(32f, (float) (21 + this.yOffset)), new Microsoft.Xna.Framework.Rectangle?(this.Sprite.SourceRect), Color.White, 0.0f, new Vector2(8f, 16f), Math.Max(0.2f, this.scale.Value) * 4f, this.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0.0f, this.drawOnTop ? 0.991f : (float) y1 / 10000f));
    SpriteBatch spriteBatch = b;
    Texture2D shadowTexture = Game1.shadowTexture;
    Vector2 position = this.getLocalPosition(Game1.viewport) + new Vector2(32f, 64f);
    Microsoft.Xna.Framework.Rectangle? sourceRectangle = new Microsoft.Xna.Framework.Rectangle?(Game1.shadowTexture.Bounds);
    Color white = Color.White;
    Microsoft.Xna.Framework.Rectangle bounds = Game1.shadowTexture.Bounds;
    double x = (double) bounds.Center.X;
    bounds = Game1.shadowTexture.Bounds;
    double y2 = (double) bounds.Center.Y;
    Vector2 origin = new Vector2((float) x, (float) y2);
    double scale = 3.0 + (double) this.yOffset / 20.0;
    double layerDepth = (double) (y1 - 1) / 10000.0;
    spriteBatch.Draw(shadowTexture, position, sourceRectangle, white, 0.0f, origin, (float) scale, SpriteEffects.None, (float) layerDepth);
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
    this.Slipperiness = 8;
    Utility.addSprinklesToLocation(this.currentLocation, this.TilePoint.X, this.TilePoint.Y, 2, 2, 101, 50, Color.LightBlue);
    if (Game1.random.NextDouble() < this.missChance.Value - this.missChance.Value * addedPrecision)
    {
      damage1 = -1;
    }
    else
    {
      this.Health -= damage1;
      if (this.Health <= 0)
        this.deathAnimation();
      this.setTrajectory(xTrajectory, yTrajectory);
    }
    this.addedSpeed = -1f;
    Utility.removeLightSource(this.lightSourceId);
    return damage1;
  }

  protected override void localDeathAnimation()
  {
    this.currentLocation.localSound("ghost");
    this.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(this.Sprite.textureName.Value, new Microsoft.Xna.Framework.Rectangle(0, 96 /*0x60*/, 16 /*0x10*/, 24), 100f, 4, 0, this.Position, false, false, 0.9f, 1f / 1000f, Color.White, 4f, 0.01f, 0.0f, (float) Math.PI / 64f));
  }

  protected override void sharedDeathAnimation()
  {
  }

  protected override void updateAnimation(GameTime time)
  {
    this.yOffset = (int) (Math.Sin((double) time.TotalGameTime.Milliseconds / 1000.0 * (2.0 * Math.PI)) * 20.0) - this.yOffsetExtra;
    if (this.currentLocation == Game1.currentLocation)
    {
      LightSource lightSource;
      if (Game1.currentLightSources.TryGetValue(this.lightSourceId, out lightSource))
        lightSource.position.Value = new Vector2(this.Position.X + 32f, this.Position.Y + 64f + (float) this.yOffset);
      else
        Game1.currentLightSources.Add(new LightSource(this.lightSourceId, 5, new Vector2(this.Position.X + 8f, this.Position.Y + 64f), 1f, Color.White * 0.7f, onlyLocation: Game1.currentLocation.NameOrUniqueName));
    }
    Point standingPixel1 = this.StandingPixel;
    Point standingPixel2 = this.Player.StandingPixel;
    float num1 = (float) -(standingPixel2.X - standingPixel1.X);
    float num2 = (float) (standingPixel2.Y - standingPixel1.Y);
    float num3 = 400f;
    float x = num1 / num3;
    float num4 = num2 / num3;
    if (this.wasHitCounter <= 0)
    {
      this.targetRotation = (float) Math.Atan2(-(double) num4, (double) x) - 1.57079637f;
      if ((double) Math.Abs(this.targetRotation) - (double) Math.Abs(this.rotation) > 7.0 * Math.PI / 8.0 && Game1.random.NextBool())
        this.turningRight = true;
      else if ((double) Math.Abs(this.targetRotation) - (double) Math.Abs(this.rotation) < Math.PI / 8.0)
        this.turningRight = false;
      if (this.turningRight)
        this.rotation -= (float) Math.Sign(this.targetRotation - this.rotation) * ((float) Math.PI / 64f);
      else
        this.rotation += (float) Math.Sign(this.targetRotation - this.rotation) * ((float) Math.PI / 64f);
      this.rotation %= 6.28318548f;
      this.wasHitCounter = 0;
    }
    float num5 = Math.Min(4f, Math.Max(1f, (float) (5.0 - (double) num3 / 64.0 / 2.0)));
    float num6 = (float) Math.Cos((double) this.rotation + Math.PI / 2.0);
    float num7 = -(float) Math.Sin((double) this.rotation + Math.PI / 2.0);
    this.xVelocity += (float) (-(double) num6 * (double) num5 / 6.0 + (double) Game1.random.Next(-10, 10) / 100.0);
    this.yVelocity += (float) (-(double) num7 * (double) num5 / 6.0 + (double) Game1.random.Next(-10, 10) / 100.0);
    if ((double) Math.Abs(this.xVelocity) > (double) Math.Abs((float) (-(double) num6 * 5.0)))
      this.xVelocity -= (float) (-(double) num6 * (double) num5 / 6.0);
    if ((double) Math.Abs(this.yVelocity) > (double) Math.Abs((float) (-(double) num7 * 5.0)))
      this.yVelocity -= (float) (-(double) num7 * (double) num5 / 6.0);
    this.faceGeneralDirection(this.Player.getStandingPosition());
    this.resetAnimationSpeed();
  }

  public override void behaviorAtGameTick(GameTime time)
  {
    base.behaviorAtGameTick(time);
    Microsoft.Xna.Framework.Rectangle boundingBox1 = this.GetBoundingBox();
    Microsoft.Xna.Framework.Rectangle boundingBox2 = this.Player.GetBoundingBox();
    if (!boundingBox1.Intersects(boundingBox2) || !this.Player.temporarilyInvincible)
      return;
    Layer layer = this.currentLocation.map.RequireLayer("Back");
    Point center = boundingBox2.Center;
    int num = 0;
    Vector2 vector2;
    for (vector2 = new Vector2((float) (center.X / 64 /*0x40*/ + Game1.random.Next(-12, 12)), (float) (center.Y / 64 /*0x40*/ + Game1.random.Next(-12, 12))); num < 3 && ((double) vector2.X >= (double) layer.LayerWidth || (double) vector2.Y >= (double) layer.LayerHeight || (double) vector2.X < 0.0 || (double) vector2.Y < 0.0 || layer.Tiles[(int) vector2.X, (int) vector2.Y] == null || !this.currentLocation.isTilePassable(new Location((int) vector2.X, (int) vector2.Y), Game1.viewport) || vector2.Equals(new Vector2((float) (center.X / 64 /*0x40*/), (float) (center.Y / 64 /*0x40*/)))); ++num)
      vector2 = new Vector2((float) (center.X / 64 /*0x40*/ + Game1.random.Next(-12, 12)), (float) (center.Y / 64 /*0x40*/ + Game1.random.Next(-12, 12)));
    if (num >= 3)
      return;
    this.Position = new Vector2(vector2.X * 64f, (float) ((double) vector2.Y * 64.0 - 32.0));
    this.Halt();
  }
}
