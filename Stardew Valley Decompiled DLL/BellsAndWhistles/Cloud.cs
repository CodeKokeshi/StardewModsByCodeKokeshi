// Decompiled with JetBrains decompiler
// Type: StardewValley.BellsAndWhistles.Cloud
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Extensions;

#nullable disable
namespace StardewValley.BellsAndWhistles;

public class Cloud : Critter
{
  public const int width = 147;
  public const int height = 100;
  public int zoom = 5;
  private bool verticalFlip;
  private bool horizontalFlip;

  public Cloud()
  {
  }

  public Cloud(Vector2 position)
  {
    this.position = position * 64f;
    this.startingPosition = position;
    this.verticalFlip = Game1.random.NextBool();
    this.horizontalFlip = Game1.random.NextBool();
    this.zoom = Game1.random.Next(4, 7);
  }

  public override bool update(GameTime time, GameLocation environment)
  {
    this.position.Y -= (float) (time.ElapsedGameTime.TotalMilliseconds * 0.019999999552965164);
    this.position.X -= (float) (time.ElapsedGameTime.TotalMilliseconds * 0.019999999552965164);
    return (double) this.position.X < (double) (-147 * this.zoom) || (double) this.position.Y < (double) (-100 * this.zoom);
  }

  public override Rectangle getBoundingBox(int xOffset, int yOffset)
  {
    return new Rectangle((int) this.position.X, (int) this.position.Y, 147 * this.zoom, 100 * this.zoom);
  }

  public override void draw(SpriteBatch b)
  {
  }

  public override void drawAboveFrontLayer(SpriteBatch b)
  {
    b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(this.position), new Rectangle?(new Rectangle(128 /*0x80*/, 0, 146, 99)), Color.White, !this.verticalFlip || !this.horizontalFlip ? 0.0f : 3.14159274f, Vector2.Zero, (float) this.zoom, !this.verticalFlip || this.horizontalFlip ? (!this.horizontalFlip || this.verticalFlip ? SpriteEffects.None : SpriteEffects.FlipHorizontally) : SpriteEffects.FlipVertically, 1f);
  }
}
