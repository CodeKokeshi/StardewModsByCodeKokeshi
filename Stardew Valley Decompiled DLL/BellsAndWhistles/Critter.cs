// Decompiled with JetBrains decompiler
// Type: StardewValley.BellsAndWhistles.Critter
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

#nullable disable
namespace StardewValley.BellsAndWhistles;

public abstract class Critter
{
  public const int spriteWidth = 32 /*0x20*/;
  public const int spriteHeight = 32 /*0x20*/;
  public const float gravity = 0.25f;
  public static string critterTexture = "TileSheets\\critters";
  public Vector2 position;
  public Vector2 startingPosition;
  public int baseFrame;
  public AnimatedSprite sprite;
  public bool flip;
  public float gravityAffectedDY;
  public float yOffset;
  public float yJumpOffset;

  public Critter()
  {
  }

  public Critter(int baseFrame, Vector2 position)
  {
    this.baseFrame = baseFrame;
    this.position = position;
    this.sprite = new AnimatedSprite(Critter.critterTexture, baseFrame, 32 /*0x20*/, 32 /*0x20*/);
    this.startingPosition = position;
  }

  public virtual Rectangle getBoundingBox(int xOffset, int yOffset)
  {
    return new Rectangle((int) this.position.X - 32 /*0x20*/ + xOffset, (int) this.position.Y - 16 /*0x10*/ + yOffset, 64 /*0x40*/, 32 /*0x20*/);
  }

  public virtual bool update(GameTime time, GameLocation environment)
  {
    this.sprite.animateOnce(time);
    if ((double) this.gravityAffectedDY < 0.0 || (double) this.yJumpOffset < 0.0)
    {
      this.yJumpOffset += this.gravityAffectedDY;
      this.gravityAffectedDY += 0.25f;
    }
    return (double) this.position.X < (double) sbyte.MinValue || (double) this.position.Y < (double) sbyte.MinValue || (double) this.position.X > (double) environment.map.DisplayWidth || (double) this.position.Y > (double) environment.map.DisplayHeight;
  }

  public virtual void draw(SpriteBatch b)
  {
    if (this.sprite == null)
      return;
    this.sprite.draw(b, Game1.GlobalToLocal(Game1.viewport, this.position + new Vector2(-64f, this.yJumpOffset - 128f + this.yOffset)), (float) ((double) this.position.Y / 10000.0 + (double) this.position.X / 1000000.0), 0, 0, Color.White, this.flip, 4f);
    b.Draw(Game1.shadowTexture, Game1.GlobalToLocal(Game1.viewport, this.position + new Vector2(0.0f, -4f)), new Rectangle?(Game1.shadowTexture.Bounds), Color.White * (1f - Math.Min(1f, Math.Abs((float) (((double) this.yJumpOffset + (double) this.yOffset) / 64.0)))), 0.0f, new Vector2((float) Game1.shadowTexture.Bounds.Center.X, (float) Game1.shadowTexture.Bounds.Center.Y), 3f + Math.Max(-3f, (float) (((double) this.yJumpOffset + (double) this.yOffset) / 64.0)), SpriteEffects.None, (float) (((double) this.position.Y - 1.0) / 10000.0));
  }

  public virtual void drawAboveFrontLayer(SpriteBatch b)
  {
  }

  /// <summary>Auto-generate a default light source ID for this critter.</summary>
  /// <param name="identifier">A unique ID for this critter instance.</param>
  protected virtual string GenerateLightSourceId(int identifier)
  {
    return $"{this.GetType().Name}_{identifier}";
  }
}
