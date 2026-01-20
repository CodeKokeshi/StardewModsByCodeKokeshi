// Decompiled with JetBrains decompiler
// Type: StardewValley.Prop
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#nullable disable
namespace StardewValley;

public class Prop
{
  private Texture2D texture;
  private Rectangle sourceRect;
  private Rectangle drawRect;
  private Rectangle boundingRect;
  private bool solid;

  /// <summary>
  /// 
  /// </summary>
  /// <param name="texture"></param>
  /// <param name="index"></param>
  /// <param name="tilesWideSolid"></param>
  /// <param name="tilesHighSolid">how many tiles high this prop's bounding box should be. The difference between this and the tilesHighDraw parameter dictate wwhat portion is drawn in front of the player.</param>
  /// <param name="tilesHighDraw">how many tiles high this prop's sprite is in total.</param>
  /// <param name="tileX"></param>
  /// <param name="tileY">y-tile of the solid portion of the prop. stuff considered "height" that the player is draw behind doesn't count.</param>
  public Prop(
    Texture2D texture,
    int index,
    int tilesWideSolid,
    int tilesHighSolid,
    int tilesHighDraw,
    int tileX,
    int tileY,
    bool solid = true)
  {
    this.texture = texture;
    this.sourceRect = Game1.getSourceRectForStandardTileSheet(texture, index, 16 /*0x10*/, 16 /*0x10*/);
    this.sourceRect.Width = tilesWideSolid * 16 /*0x10*/;
    this.sourceRect.Height = tilesHighDraw * 16 /*0x10*/;
    this.drawRect = new Rectangle(tileX * 64 /*0x40*/, tileY * 64 /*0x40*/ + (tilesHighSolid - tilesHighDraw) * 64 /*0x40*/, tilesWideSolid * 64 /*0x40*/, tilesHighDraw * 64 /*0x40*/);
    this.boundingRect = new Rectangle(tileX * 64 /*0x40*/, tileY * 64 /*0x40*/, tilesWideSolid * 64 /*0x40*/, tilesHighSolid * 64 /*0x40*/);
    this.solid = solid;
  }

  public bool isColliding(Rectangle r) => this.solid && r.Intersects(this.boundingRect);

  public void draw(SpriteBatch b)
  {
    this.drawRect.X = this.boundingRect.X - Game1.viewport.X;
    this.drawRect.Y = this.boundingRect.Y + (this.boundingRect.Height - this.drawRect.Height) - Game1.viewport.Y;
    b.Draw(this.texture, this.drawRect, new Rectangle?(this.sourceRect), Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, this.solid ? (float) this.boundingRect.Y / 10000f : 0.0f);
  }
}
