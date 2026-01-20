// Decompiled with JetBrains decompiler
// Type: StardewValley.Companions.HoppingCompanion
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

#nullable disable
namespace StardewValley.Companions;

public class HoppingCompanion : Companion
{
  public HoppingCompanion()
  {
  }

  public HoppingCompanion(int which = 0) => this.whichVariant.Value = which;

  public override void Draw(SpriteBatch b)
  {
    if (this.Owner?.currentLocation == null || this.Owner.currentLocation.DisplayName == "Temp" && !Game1.isFestival())
      return;
    Texture2D texture = Game1.content.Load<Texture2D>("TileSheets\\companions");
    this._draw(b, texture, new Rectangle(0, 16 /*0x10*/, 16 /*0x10*/, 16 /*0x10*/));
  }

  protected void _draw(SpriteBatch b, Texture2D texture, Rectangle startingSourceRect)
  {
    SpriteEffects effects = SpriteEffects.None;
    if (this.direction.Value == 3)
      effects = SpriteEffects.FlipHorizontally;
    if ((double) this.height > 0.0)
    {
      if ((double) this.gravity > 0.0)
        b.Draw(texture, Game1.GlobalToLocal(this.Position + this.Owner.drawOffset + new Vector2(0.0f, (float) (-(double) this.height * 4.0))), new Rectangle?(Utility.translateRect(startingSourceRect, 16 /*0x10*/)), Color.White, 0.0f, new Vector2(8f, 16f), 4f, effects, this._position.Y / 10000f);
      else if ((double) this.gravity > -0.15000000596046448)
        b.Draw(texture, Game1.GlobalToLocal(this.Position + this.Owner.drawOffset + new Vector2(0.0f, (float) (-(double) this.height * 4.0))), new Rectangle?(Utility.translateRect(startingSourceRect, 32 /*0x20*/)), Color.White, 0.0f, new Vector2(8f, 16f), 4f, effects, this._position.Y / 10000f);
      else
        b.Draw(texture, Game1.GlobalToLocal(this.Position + this.Owner.drawOffset + new Vector2(0.0f, (float) (-(double) this.height * 4.0))), new Rectangle?(Utility.translateRect(startingSourceRect, 48 /*0x30*/)), Color.White, 0.0f, new Vector2(8f, 16f), 4f, effects, this._position.Y / 10000f);
    }
    else
      b.Draw(texture, Game1.GlobalToLocal(this.Position + this.Owner.drawOffset + new Vector2(0.0f, (float) (-(double) this.height * 4.0))), new Rectangle?(startingSourceRect), Color.White, 0.0f, new Vector2(8f, 16f), 4f, effects, this._position.Y / 10000f);
    SpriteBatch spriteBatch = b;
    Texture2D shadowTexture = Game1.shadowTexture;
    Vector2 local = Game1.GlobalToLocal(this.Position + this.Owner.drawOffset);
    Rectangle? sourceRectangle = new Rectangle?(Game1.shadowTexture.Bounds);
    Color white = Color.White;
    Rectangle bounds = Game1.shadowTexture.Bounds;
    double x = (double) bounds.Center.X;
    bounds = Game1.shadowTexture.Bounds;
    double y = (double) bounds.Center.Y;
    Vector2 origin = new Vector2((float) x, (float) y);
    double scale = 3.0 * (double) Utility.Lerp(1f, 0.8f, Math.Min(this.height, 1f));
    spriteBatch.Draw(shadowTexture, local, sourceRectangle, white, 0.0f, origin, (float) scale, SpriteEffects.None, 0.0f);
  }
}
