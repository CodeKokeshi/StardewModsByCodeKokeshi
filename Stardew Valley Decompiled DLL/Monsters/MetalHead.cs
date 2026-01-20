// Decompiled with JetBrains decompiler
// Type: StardewValley.Monsters.MetalHead
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley.Monsters;

public class MetalHead : Monster
{
  [XmlElement("c")]
  public readonly NetColor c = new NetColor();

  public MetalHead()
  {
  }

  public MetalHead(Vector2 tileLocation, MineShaft mine)
    : this(tileLocation, mine.getMineArea())
  {
  }

  public MetalHead(string name, Vector2 tileLocation)
    : base(name, tileLocation)
  {
    this.Sprite.SpriteHeight = 16 /*0x10*/;
    this.Sprite.UpdateSourceRect();
    this.c.Value = Color.White;
    this.IsWalkingTowardPlayer = true;
  }

  public MetalHead(Vector2 tileLocation, int mineArea)
    : base("Metal Head", tileLocation)
  {
    this.Sprite.SpriteHeight = 16 /*0x10*/;
    this.Sprite.UpdateSourceRect();
    this.c.Value = Color.White;
    this.IsWalkingTowardPlayer = true;
    switch (mineArea)
    {
      case 0:
        this.c.Value = Color.White;
        break;
      case 40:
        this.c.Value = Color.Turquoise;
        this.Health *= 2;
        break;
      case 80 /*0x50*/:
        this.c.Value = Color.White;
        this.Health *= 3;
        break;
    }
  }

  protected override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.c, "c");
    this.position.Field.AxisAlignedMovement = true;
  }

  public override int takeDamage(
    int damage,
    int xTrajectory,
    int yTrajectory,
    bool isBomb,
    double addedPrecision,
    Farmer who)
  {
    return this.takeDamage(damage, xTrajectory, yTrajectory, isBomb, addedPrecision, "clank");
  }

  protected override void localDeathAnimation()
  {
    this.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(46, this.Position, Color.DarkGray, 10, animationInterval: 70f));
    this.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(46, this.Position + new Vector2(-32f, 0.0f), Color.DarkGray, 10, animationInterval: 70f)
    {
      delayBeforeAnimationStart = 300
    });
    this.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(46, this.Position + new Vector2(32f, 0.0f), Color.DarkGray, 10, animationInterval: 70f)
    {
      delayBeforeAnimationStart = 600
    });
    this.currentLocation.localSound("monsterdead");
    Utility.makeTemporarySpriteJuicier(new TemporaryAnimatedSprite(44, this.Position, Color.MediumPurple, 10)
    {
      holdLastFrame = true,
      alphaFade = 0.01f,
      interval = 70f
    }, this.currentLocation);
    base.localDeathAnimation();
  }

  public override void draw(SpriteBatch b)
  {
    if (this.IsInvisible || !Utility.isOnScreen(this.Position, 128 /*0x80*/))
      return;
    int y1 = this.StandingPixel.Y;
    SpriteBatch spriteBatch = b;
    Texture2D shadowTexture = Game1.shadowTexture;
    Vector2 position = this.getLocalPosition(Game1.viewport) + new Vector2(32f, 42f + this.yOffset);
    Rectangle? sourceRectangle = new Rectangle?(Game1.shadowTexture.Bounds);
    Color white = Color.White;
    Rectangle bounds = Game1.shadowTexture.Bounds;
    double x = (double) bounds.Center.X;
    bounds = Game1.shadowTexture.Bounds;
    double y2 = (double) bounds.Center.Y;
    Vector2 origin = new Vector2((float) x, (float) y2);
    double scale = 3.5 + (double) this.scale.Value + (double) this.yOffset / 30.0;
    double layerDepth = (double) (y1 - 1) / 10000.0;
    spriteBatch.Draw(shadowTexture, position, sourceRectangle, white, 0.0f, origin, (float) scale, SpriteEffects.None, (float) layerDepth);
    b.Draw(this.Sprite.Texture, this.getLocalPosition(Game1.viewport) + new Vector2(32f, (float) (48 /*0x30*/ + this.yJumpOffset)), new Rectangle?(this.Sprite.SourceRect), this.c.Value, this.rotation, new Vector2(8f, 16f), Math.Max(0.2f, this.scale.Value) * 4f, this.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0.0f, this.drawOnTop ? 0.991f : (float) y1 / 10000f));
  }

  public override void shedChunks(int number, float scale)
  {
    Point standingPixel = this.StandingPixel;
    Game1.createRadialDebris(this.currentLocation, this.Sprite.textureName.Value, new Rectangle(0, this.Sprite.getHeight() * 4, 16 /*0x10*/, 16 /*0x10*/), 8, standingPixel.X, standingPixel.Y, number, this.TilePoint.Y, Color.White, scale * 4f);
  }

  public override List<Item> getExtraDropItems()
  {
    List<Item> extraDropItems = new List<Item>();
    if ((Game1.stats.getMonstersKilled(this.name.Value) + (int) Game1.uniqueIDForThisGame) % 100 == 0)
      extraDropItems.Add(ItemRegistry.Create("(H)51"));
    return extraDropItems;
  }

  protected override void updateMonsterSlaveAnimation(GameTime time)
  {
    if (this.isMoving())
    {
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
    else
      this.Sprite.StopAnimation();
  }
}
