// Decompiled with JetBrains decompiler
// Type: StardewValley.BellsAndWhistles.SuspensionBridge
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

#nullable disable
namespace StardewValley.BellsAndWhistles;

public class SuspensionBridge
{
  public Rectangle bridgeBounds;
  public List<Rectangle> bridgeEntrances = new List<Rectangle>();
  public List<Rectangle> bridgeSortRegions = new List<Rectangle>();
  public const float BRIDGE_SORT_OFFSET = 0.0256f;
  protected Texture2D _texture;
  public float shakeTime;

  public SuspensionBridge()
  {
    this._texture = Game1.content.Load<Texture2D>("LooseSprites\\SuspensionBridge");
  }

  public SuspensionBridge(int tile_x, int tile_y)
    : this()
  {
    this.bridgeBounds = new Rectangle(tile_x * 64 /*0x40*/, tile_y * 64 /*0x40*/, 384, 64 /*0x40*/);
    this.bridgeEntrances.Add(new Rectangle((tile_x - 1) * 64 /*0x40*/, tile_y * 64 /*0x40*/, 64 /*0x40*/, 64 /*0x40*/));
    this.bridgeEntrances.Add(new Rectangle((tile_x + 6) * 64 /*0x40*/, tile_y * 64 /*0x40*/, 64 /*0x40*/, 64 /*0x40*/));
    this.bridgeSortRegions.Add(new Rectangle((tile_x - 1) * 64 /*0x40*/, (tile_y - 1) * 64 /*0x40*/, 128 /*0x80*/, 192 /*0xC0*/));
    this.bridgeSortRegions.Add(new Rectangle((tile_x + 5) * 64 /*0x40*/, (tile_y - 1) * 64 /*0x40*/, 128 /*0x80*/, 192 /*0xC0*/));
  }

  public virtual bool InEntranceArea(int x, int y)
  {
    foreach (Rectangle bridgeEntrance in this.bridgeEntrances)
    {
      if (bridgeEntrance.Contains(x, y))
        return true;
    }
    return false;
  }

  public virtual bool InEntranceArea(Rectangle rectangle)
  {
    foreach (Rectangle bridgeEntrance in this.bridgeEntrances)
    {
      if (bridgeEntrance.Contains(rectangle))
        return true;
    }
    return false;
  }

  public virtual bool CheckPlacementPrevention(Vector2 tileLocation)
  {
    foreach (Rectangle bridgeEntrance in this.bridgeEntrances)
    {
      if (Utility.doesRectangleIntersectTile(bridgeEntrance, (int) tileLocation.X, (int) tileLocation.Y))
        return true;
    }
    return false;
  }

  public virtual void OnFootstep(Vector2 position)
  {
    if (!this.bridgeBounds.Contains((int) position.X, (int) position.Y) || (double) position.X <= (double) (this.bridgeBounds.X + 64 /*0x40*/) || (double) position.X >= (double) (this.bridgeBounds.Right - 64 /*0x40*/))
      return;
    this.shakeTime = 0.4f;
  }

  public virtual void Update(GameTime time)
  {
    if ((double) this.shakeTime > 0.0)
    {
      this.shakeTime -= (float) time.ElapsedGameTime.TotalSeconds;
      if ((double) this.shakeTime < 0.0)
        this.shakeTime = 0.0f;
    }
    if (Game1.player.bridge == null && this.InEntranceArea(Game1.player.GetBoundingBox()))
      Game1.player.bridge = this;
    if (Game1.player.bridge != this)
      return;
    Rectangle boundingBox = Game1.player.GetBoundingBox();
    if (boundingBox.Top >= this.bridgeBounds.Top && boundingBox.Bottom <= this.bridgeBounds.Bottom && (boundingBox.Intersects(this.bridgeBounds) || this.InEntranceArea(boundingBox)))
    {
      Game1.player.SetOnBridge(true);
    }
    else
    {
      if (this.InEntranceArea(boundingBox) || boundingBox.Intersects(this.bridgeBounds))
        return;
      Game1.player.SetOnBridge(false);
      Game1.player.bridge = (SuspensionBridge) null;
    }
  }

  public virtual void Draw(SpriteBatch b)
  {
    b.Draw(this._texture, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) this.bridgeBounds.X, (float) (this.bridgeBounds.Y - 128 /*0x80*/))), new Rectangle?(new Rectangle(0, 0, 96 /*0x60*/, 32 /*0x20*/)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) ((double) this.bridgeBounds.Y / 10000.0 + 0.025599999353289604));
    float[] numArray = new float[6]
    {
      0.0f,
      0.5f,
      1f,
      1f,
      0.5f,
      0.0f
    };
    for (int index = 0; index < 6; ++index)
    {
      float num = (float) (Math.Sin(Game1.currentGameTime.TotalGameTime.TotalSeconds * 10.0 + (double) (index * 5)) * 1.0 * 4.0 * (double) numArray[index] * ((double) this.shakeTime / 0.40000000596046448));
      b.Draw(this._texture, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (this.bridgeBounds.X + index * 64 /*0x40*/), (float) this.bridgeBounds.Y + num)), new Rectangle?(new Rectangle(16 /*0x10*/ * index, 32 /*0x20*/, 16 /*0x10*/, 16 /*0x10*/)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) ((double) this.bridgeBounds.Y / 10000.0 + 0.025599999353289604));
    }
  }
}
