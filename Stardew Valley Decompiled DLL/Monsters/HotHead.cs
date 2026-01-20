// Decompiled with JetBrains decompiler
// Type: StardewValley.Monsters.HotHead
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
using System.Xml.Serialization;

#nullable disable
namespace StardewValley.Monsters;

public class HotHead : MetalHead
{
  [XmlIgnore]
  public NetFarmerRef lastAttacker = new NetFarmerRef();
  [XmlIgnore]
  public NetFloat timeUntilExplode = new NetFloat(-1f);
  [XmlIgnore]
  public NetBool angry = new NetBool();

  public HotHead()
  {
  }

  protected override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.lastAttacker.NetFields, "lastAttacker.NetFields").AddField((INetSerializable) this.angry, "angry").AddField((INetSerializable) this.timeUntilExplode, "timeUntilExplode");
  }

  public HotHead(Vector2 position)
    : base("Hot Head", position)
  {
    this.Slipperiness *= 2;
  }

  public override int takeDamage(
    int damage,
    int xTrajectory,
    int yTrajectory,
    bool isBomb,
    double addedPrecision,
    Farmer who)
  {
    this.lastAttacker.Value = who;
    int damage1 = base.takeDamage(damage, xTrajectory, yTrajectory, isBomb, addedPrecision, who);
    if ((double) this.timeUntilExplode.Value != -1.0 || this.Health >= 25)
      return damage1;
    this.currentLocation.netAudio.StartPlaying("fuse");
    this.timeUntilExplode.Value = 2.4f;
    this.Speed = 5;
    this.angry.Value = true;
    return damage1;
  }

  public override void behaviorAtGameTick(GameTime time)
  {
    if (Game1.IsMasterGame && (double) this.timeUntilExplode.Value > 0.0)
    {
      this.timeUntilExplode.Value -= (float) time.ElapsedGameTime.TotalSeconds;
      if ((double) this.timeUntilExplode.Value <= 0.0)
      {
        this.currentLocation.netAudio.StopPlaying("fuse");
        this.timeUntilExplode.Value = 0.0f;
        this.DropBomb();
        this.Health = -9999;
        return;
      }
    }
    base.behaviorAtGameTick(time);
  }

  public virtual void DropBomb()
  {
    this.currentLocation.netAudio.StopPlaying("fuse");
    if (this.lastAttacker.Value == null)
      return;
    Farmer farmer = this.lastAttacker.Value;
    int num1 = Game1.random.Next();
    this.currentLocation.playSound("thudStep");
    Vector2 tile = this.Tile;
    float y = this.Position.Y;
    float num2 = 2.4f;
    if ((double) this.timeUntilExplode.Value >= 0.0)
    {
      num2 = this.timeUntilExplode.Value;
      this.currentLocation.netAudio.StartPlaying("fuse");
    }
    int numberOfLoops = Math.Max(1, (int) ((double) num2 * 1000.0 / 100.0));
    Game1.multiplayer.broadcastSprites(this.currentLocation, new TemporaryAnimatedSprite("Characters\\Monsters\\Hot Head", new Rectangle(16 /*0x10*/, 64 /*0x40*/, 16 /*0x10*/, 16 /*0x10*/), 25f, 3, numberOfLoops, tile * 64f, false, Game1.random.NextBool())
    {
      shakeIntensity = 0.5f,
      shakeIntensityChange = 1f / 500f,
      extraInfoForEndBehavior = num1,
      endFunction = new TemporaryAnimatedSprite.endBehavior(this.currentLocation.removeTemporarySpritesWithID),
      bombRadius = 2,
      bombDamage = this.DamageToFarmer,
      Parent = this.currentLocation,
      scale = 4f,
      owner = farmer
    });
    Game1.multiplayer.broadcastSprites(this.currentLocation, new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(598, 1279 /*0x04FF*/, 3, 4), 53f, 5, 9, tile * 64f, true, false, (float) (((double) y + 7.0) / 10000.0), 0.0f, Color.Yellow, 4f, 0.0f, 0.0f, 0.0f)
    {
      id = num1
    });
    Game1.multiplayer.broadcastSprites(this.currentLocation, new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(598, 1279 /*0x04FF*/, 3, 4), 53f, 5, 9, tile * 64f, true, false, (float) (((double) y + 7.0) / 10000.0), 0.0f, Color.Orange, 4f, 0.0f, 0.0f, 0.0f)
    {
      delayBeforeAnimationStart = 100,
      id = num1
    });
    Game1.multiplayer.broadcastSprites(this.currentLocation, new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(598, 1279 /*0x04FF*/, 3, 4), 53f, 5, 9, tile * 64f, true, false, (float) (((double) y + 7.0) / 10000.0), 0.0f, Color.White, 3f, 0.0f, 0.0f, 0.0f)
    {
      delayBeforeAnimationStart = 200,
      id = num1
    });
  }

  protected override void sharedDeathAnimation()
  {
    base.sharedDeathAnimation();
    this.DropBomb();
  }

  public override void draw(SpriteBatch b)
  {
    if (this.angry.Value)
    {
      if (this.IsInvisible || !Utility.isOnScreen(this.Position, 128 /*0x80*/))
        return;
      Rectangle sourceRect = this.Sprite.SourceRect;
      sourceRect.Y += 80 /*0x50*/;
      int y = this.StandingPixel.Y;
      b.Draw(Game1.shadowTexture, this.getLocalPosition(Game1.viewport) + new Vector2(32f, 42f + this.yOffset), new Rectangle?(Game1.shadowTexture.Bounds), Color.White, 0.0f, new Vector2((float) Game1.shadowTexture.Bounds.Center.X, (float) Game1.shadowTexture.Bounds.Center.Y), (float) (3.5 + (double) this.scale.Value + (double) this.yOffset / 30.0), SpriteEffects.None, (float) (y - 1) / 10000f);
      b.Draw(this.Sprite.Texture, this.getLocalPosition(Game1.viewport) + new Vector2(32f, (float) (48 /*0x30*/ + this.yJumpOffset)), new Rectangle?(sourceRect), this.c.Value, this.rotation, new Vector2(8f, 16f), Math.Max(0.2f, this.scale.Value) * 4f, this.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0.0f, this.drawOnTop ? 0.991f : (float) y / 10000f));
    }
    else
      base.draw(b);
  }
}
