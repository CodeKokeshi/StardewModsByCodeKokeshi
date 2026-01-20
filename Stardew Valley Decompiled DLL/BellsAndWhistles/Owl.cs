// Decompiled with JetBrains decompiler
// Type: StardewValley.BellsAndWhistles.Owl
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

#nullable disable
namespace StardewValley.BellsAndWhistles;

public class Owl : Critter
{
  public Owl()
  {
  }

  public Owl(Vector2 position)
  {
    this.baseFrame = 83;
    this.position = position;
    this.sprite = new AnimatedSprite(Critter.critterTexture, this.baseFrame, 32 /*0x20*/, 32 /*0x20*/);
    this.startingPosition = position;
    this.sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
    {
      new FarmerSprite.AnimationFrame(83, 100),
      new FarmerSprite.AnimationFrame(84, 100),
      new FarmerSprite.AnimationFrame(85, 100),
      new FarmerSprite.AnimationFrame(86, 100)
    });
  }

  public override bool update(GameTime time, GameLocation environment)
  {
    Vector2 vector2 = new Vector2((float) Game1.viewport.X - Game1.previousViewportPosition.X, (float) Game1.viewport.Y - Game1.previousViewportPosition.Y) * 0.15f;
    this.position.Y += (float) (time.ElapsedGameTime.TotalMilliseconds * 0.20000000298023224);
    this.position.X += (float) (time.ElapsedGameTime.TotalMilliseconds * 0.05000000074505806);
    this.position = this.position - vector2;
    return base.update(time, environment);
  }

  public override void draw(SpriteBatch b)
  {
  }

  public override void drawAboveFrontLayer(SpriteBatch b)
  {
    this.sprite.draw(b, Game1.GlobalToLocal(Game1.viewport, this.position + new Vector2(-64f, this.yJumpOffset - 128f + this.yOffset)), (float) ((double) this.position.Y / 10000.0 + (double) this.position.X / 100000.0), 0, 0, Color.MediumBlue, this.flip, 4f);
  }
}
