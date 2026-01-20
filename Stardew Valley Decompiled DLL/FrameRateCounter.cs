// Decompiled with JetBrains decompiler
// Type: FrameRateCounter
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System;

#nullable disable
public class FrameRateCounter : DrawableGameComponent
{
  private LocalizedContentManager content;
  private SpriteBatch spriteBatch;
  private int frameRate;
  private int frameCounter;
  private TimeSpan elapsedTime = TimeSpan.Zero;

  public FrameRateCounter(Game game)
    : base(game)
  {
    this.content = new LocalizedContentManager((IServiceProvider) game.Services, this.Game.Content.RootDirectory);
  }

  protected override void LoadContent() => this.spriteBatch = new SpriteBatch(this.GraphicsDevice);

  protected override void UnloadContent() => this.content.Unload();

  public override void Update(GameTime gameTime)
  {
    this.elapsedTime += gameTime.ElapsedGameTime;
    if (!(this.elapsedTime > TimeSpan.FromSeconds(1.0)))
      return;
    this.elapsedTime -= TimeSpan.FromSeconds(1.0);
    this.frameRate = this.frameCounter;
    this.frameCounter = 0;
  }

  public override void Draw(GameTime gameTime)
  {
    ++this.frameCounter;
    string text = $"fps: {this.frameRate}";
    this.spriteBatch.Begin();
    this.spriteBatch.DrawString(Game1.dialogueFont, text, new Vector2(33f, 33f), Color.Black);
    this.spriteBatch.DrawString(Game1.dialogueFont, text, new Vector2(32f, 32f), Color.White);
    this.spriteBatch.End();
  }
}
