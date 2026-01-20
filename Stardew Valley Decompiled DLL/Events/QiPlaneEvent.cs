// Decompiled with JetBrains decompiler
// Type: StardewValley.Events.QiPlaneEvent
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley.BellsAndWhistles;
using System;
using System.Collections.Generic;

#nullable disable
namespace StardewValley.Events;

public class QiPlaneEvent : BaseFarmEvent
{
  private Vector2 qiPlanePos;
  private List<TemporaryAnimatedSprite> tempSprites = new List<TemporaryAnimatedSprite>();
  private float boxDropTimer;
  private float textTimer;
  private float finalFadeTimer;
  private string str;

  public QiPlaneEvent()
  {
    this.qiPlanePos = new Vector2(-400f, (float) (Game1.graphics.GraphicsDevice.Viewport.Height / 4));
    this.boxDropTimer = 2000f;
    this.str = Game1.content.LoadString("Strings\\1_6_Strings:MysteryBoxAnnounce");
    Game1.changeMusicTrack("nightTime");
    DelayedAction.playSoundAfterDelay("planeflyby", 1000);
    Game1.player.mailReceived.Add("sawQiPlane");
  }

  public override void draw(SpriteBatch b)
  {
    SpriteBatch spriteBatch1 = b;
    Texture2D staminaRect1 = Game1.staminaRect;
    Viewport viewport1 = Game1.graphics.GraphicsDevice.Viewport;
    int width1 = viewport1.Width;
    viewport1 = Game1.graphics.GraphicsDevice.Viewport;
    int height1 = viewport1.Height;
    Rectangle destinationRectangle1 = new Rectangle(0, 0, width1, height1);
    Rectangle? sourceRectangle1 = new Rectangle?(new Rectangle(0, 0, 1, 1));
    Color color1 = new Color(24, 34, 84);
    spriteBatch1.Draw(staminaRect1, destinationRectangle1, sourceRectangle1, color1);
    SpriteBatch spriteBatch2 = b;
    Texture2D mouseCursors = Game1.mouseCursors;
    Viewport viewport2 = Game1.graphics.GraphicsDevice.Viewport;
    int width2 = viewport2.Width;
    viewport2 = Game1.graphics.GraphicsDevice.Viewport;
    int height2 = (int) ((double) viewport2.Height * 0.699999988079071);
    Rectangle destinationRectangle2 = new Rectangle(0, 0, width2, height2);
    Rectangle? sourceRectangle2 = new Rectangle?(new Rectangle(639, 858, 1, 184));
    Color lightBlue = Color.LightBlue;
    spriteBatch2.Draw(mouseCursors, destinationRectangle2, sourceRectangle2, lightBlue);
    b.Draw(Game1.mouseCursors, new Vector2(1f, 1f), new Rectangle?(new Rectangle(0, 1453, 639, 191)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.8f);
    b.Draw(Game1.mouseCursors, new Vector2(2564f, 1f), new Rectangle?(new Rectangle(0, 1453, 639, 191)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.8f);
    b.Draw(Game1.mouseCursors, new Vector2(-50f, -10f) * 4f + new Vector2(0.0f, (float) (Game1.graphics.GraphicsDevice.Viewport.Height - 596)), new Rectangle?(new Rectangle(0, 885, 639, 149)), Color.DarkCyan, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.7f);
    b.Draw(Game1.mouseCursors, new Vector2(-50f, -10f) * 4f + new Vector2(2556f, (float) (Game1.graphics.GraphicsDevice.Viewport.Height - 596)), new Rectangle?(new Rectangle(0, 885, 639, 149)), Color.DarkCyan, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.7f);
    b.Draw(Game1.mouseCursors, new Vector2(0.0f, (float) (Game1.graphics.GraphicsDevice.Viewport.Height - 596)), new Rectangle?(new Rectangle(0, 885, 639, 149)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.8f);
    b.Draw(Game1.mouseCursors, new Vector2(2556f, (float) (Game1.graphics.GraphicsDevice.Viewport.Height - 596)), new Rectangle?(new Rectangle(0, 885, 639, 149)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.8f);
    foreach (TemporaryAnimatedSprite tempSprite in this.tempSprites)
      tempSprite.draw(b, true);
    b.Draw(Game1.mouseCursors_1_6, this.qiPlanePos, new Rectangle?(new Rectangle(113, 204, 79, 43)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.82f);
    b.Draw(Game1.mouseCursors_1_6, this.qiPlanePos + new Vector2(79f, 0.0f) * 4f, new Rectangle?(new Rectangle(192 /*0xC0*/ + (int) (Game1.currentGameTime.TotalGameTime.TotalMilliseconds % 90.0 / 30.0) * 4, 204, 4, 44)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.82f);
    Viewport viewport3;
    if ((double) this.qiPlanePos.X > (double) (Game1.graphics.GraphicsDevice.Viewport.Width - 480))
    {
      float textTimer = this.textTimer;
      this.textTimer += (float) Game1.currentGameTime.ElapsedGameTime.TotalMilliseconds;
      if ((double) this.textTimer % 100.0 < (double) textTimer % 100.0 && (int) ((double) this.textTimer / 100.0) < this.str.Length)
        Game1.playSound("dialogueCharacter");
      if ((int) ((double) this.textTimer / 100.0) > this.str.Length + 27)
        this.finalFadeTimer += (float) Game1.currentGameTime.ElapsedGameTime.TotalMilliseconds;
      b.Draw(Game1.staminaRect, new Rectangle(Game1.graphics.GraphicsDevice.Viewport.Width / 2 - SpriteText.getWidthOfString(this.str) / 2 - 18, Game1.graphics.GraphicsDevice.Viewport.Height / 2 - 56, SpriteText.getWidthOfString(this.str) + 20, 60), Color.Black * 0.4f);
      SpriteBatch b1 = b;
      string str = this.str;
      viewport3 = Game1.graphics.GraphicsDevice.Viewport;
      int x = viewport3.Width / 2;
      viewport3 = Game1.graphics.GraphicsDevice.Viewport;
      int y = viewport3.Height / 2 - 50;
      int characterPosition = (int) ((double) this.textTimer / 100.0);
      Color? color2 = new Color?(Color.White);
      SpriteText.drawStringHorizontallyCenteredAt(b1, str, x, y, characterPosition, layerDepth: 0.9f, color: color2);
    }
    SpriteBatch spriteBatch3 = b;
    Texture2D staminaRect2 = Game1.staminaRect;
    viewport3 = Game1.graphics.GraphicsDevice.Viewport;
    Rectangle bounds = viewport3.Bounds;
    Color color3 = Color.Black * (this.finalFadeTimer / 3000f);
    spriteBatch3.Draw(staminaRect2, bounds, color3);
    base.draw(b);
  }

  public override void drawAboveEverything(SpriteBatch b) => base.drawAboveEverything(b);

  public override bool setUp() => base.setUp();

  public override bool tickUpdate(GameTime time)
  {
    TimeSpan elapsedGameTime;
    if (Game1.GetKeyboardState().IsKeyDown(Keys.Escape))
    {
      this.qiPlanePos.X = (float) (Game1.graphics.GraphicsDevice.Viewport.Width + 1000);
      this.textTimer += (float) (Game1.currentGameTime.ElapsedGameTime.TotalMilliseconds * 2.0);
      if ((int) ((double) this.textTimer / 100.0) > this.str.Length + 27)
      {
        double finalFadeTimer = (double) this.finalFadeTimer;
        elapsedGameTime = Game1.currentGameTime.ElapsedGameTime;
        double num = elapsedGameTime.TotalMilliseconds * 2.0;
        this.finalFadeTimer = (float) (finalFadeTimer + num);
      }
    }
    double boxDropTimer = (double) this.boxDropTimer;
    elapsedGameTime = time.ElapsedGameTime;
    double totalMilliseconds = elapsedGameTime.TotalMilliseconds;
    this.boxDropTimer = (float) (boxDropTimer - totalMilliseconds);
    Viewport viewport;
    if ((double) this.boxDropTimer <= 0.0)
    {
      double x = (double) this.qiPlanePos.X;
      viewport = Game1.graphics.GraphicsDevice.Viewport;
      double width = (double) viewport.Width;
      if (x < width)
      {
        this.tempSprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors_1_6", new Rectangle(112 /*0x70*/, 166, 14, 35), 50f, 10, 1, this.qiPlanePos + new Vector2(52f, -4f) * 4f, false, false)
        {
          holdLastFrame = true,
          motion = new Vector2(-1f, (float) Game1.random.Next(3, 5)),
          accelerationChange = new Vector2(0.0f, (float) ((Game1.random.NextDouble() - 0.5) / 1000.0 - 1.0 / 1000.0)),
          acceleration = new Vector2(0.0f, 0.05f),
          scale = 4f
        });
        this.boxDropTimer = (float) Game1.random.Next(150, 500);
        DelayedAction.playSoundAfterDelay("parachute", 300);
      }
    }
    for (int index = this.tempSprites.Count - 1; index >= 0; --index)
    {
      this.tempSprites[index].update(time);
      if ((double) this.tempSprites[index].motion.Y < 1.0)
        this.tempSprites[index].motion.Y = 1f;
      double y = (double) this.tempSprites[index].position.Y;
      viewport = Game1.graphics.GraphicsDevice.Viewport;
      double num = (double) (viewport.Height + 500);
      if (y > num)
        this.tempSprites[index].alphaFade = 0.01f;
      if ((double) this.tempSprites[index].alpha <= 0.0)
        this.tempSprites.RemoveAt(index);
    }
    ref float local = ref this.qiPlanePos.X;
    double num1 = (double) local;
    elapsedGameTime = time.ElapsedGameTime;
    double num2 = elapsedGameTime.TotalMilliseconds * 0.25;
    local = (float) (num1 + num2);
    return (double) this.finalFadeTimer > 4000.0;
  }
}
