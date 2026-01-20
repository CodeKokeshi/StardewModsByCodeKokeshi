// Decompiled with JetBrains decompiler
// Type: StardewValley.SeaMonsterTemporarySprite
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#nullable disable
namespace StardewValley;

public class SeaMonsterTemporarySprite : TemporaryAnimatedSprite
{
  public new Texture2D texture;

  public SeaMonsterTemporarySprite(
    float animationInterval,
    int animationLength,
    int numberOfLoops,
    Vector2 position)
    : base(-666, animationInterval, animationLength, numberOfLoops, position, false, false)
  {
    this.texture = Game1.content.Load<Texture2D>("LooseSprites\\SeaMonster");
    Game1.playSound("pullItemFromWater");
    this.currentParentTileIndex = 0;
  }

  public override void draw(
    SpriteBatch spriteBatch,
    bool localPosition = false,
    int xOffset = 0,
    int yOffset = 0,
    float extraAlpha = 1f)
  {
    spriteBatch.Draw(this.texture, Game1.GlobalToLocal(Game1.viewport, this.Position), new Rectangle?(new Rectangle(this.currentParentTileIndex * 16 /*0x10*/, 0, 16 /*0x10*/, 16 /*0x10*/)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) (((double) this.Position.Y + 32.0) / 10000.0));
  }

  public override bool update(GameTime time)
  {
    this.timer += (float) time.ElapsedGameTime.Milliseconds;
    if ((double) this.timer > (double) this.interval)
    {
      ++this.currentParentTileIndex;
      this.timer = 0.0f;
      if (this.currentParentTileIndex >= this.animationLength)
      {
        ++this.currentNumberOfLoops;
        this.currentParentTileIndex = 2;
      }
    }
    if (this.currentNumberOfLoops >= this.totalNumberOfLoops)
    {
      this.position.Y += 2f;
      if ((double) this.position.Y >= (double) Game1.currentLocation.Map.DisplayHeight)
        return true;
    }
    return false;
  }
}
