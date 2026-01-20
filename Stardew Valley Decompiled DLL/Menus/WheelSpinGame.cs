// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.WheelSpinGame
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley.BellsAndWhistles;
using StardewValley.Extensions;
using System;

#nullable disable
namespace StardewValley.Menus;

public class WheelSpinGame : IClickableMenu
{
  public new const int width = 640;
  public new const int height = 448;
  public double arrowRotation;
  public double arrowRotationVelocity;
  public double arrowRotationDeceleration;
  private int timerBeforeStart;
  private int wager;
  private SparklingText resultText;
  private bool doneSpinning;

  public WheelSpinGame(int wager)
    : base(Game1.uiViewport.Width / 2 - 320, Game1.uiViewport.Height / 2 - 224 /*0xE0*/, 640, 448)
  {
    this.timerBeforeStart = 1000;
    this.arrowRotationVelocity = Math.PI / 16.0;
    this.arrowRotationVelocity += (double) Game1.random.Next(0, 15) * Math.PI / 256.0;
    this.arrowRotationDeceleration = -0.00062831853071795862;
    if (Game1.random.NextBool())
      this.arrowRotationVelocity += Math.PI / 64.0;
    this.wager = wager;
    Game1.player.Halt();
  }

  /// <inheritdoc />
  public override void receiveLeftClick(int x, int y, bool playSound = true)
  {
  }

  /// <inheritdoc />
  public override void update(GameTime time)
  {
    base.update(time);
    if (this.timerBeforeStart <= 0)
    {
      double rotationVelocity = this.arrowRotationVelocity;
      this.arrowRotationVelocity += this.arrowRotationDeceleration;
      if (this.arrowRotationVelocity <= Math.PI / 80.0 && rotationVelocity > Math.PI / 80.0)
      {
        bool specialEventVariable2 = Game1.currentLocation.currentEvent.specialEventVariable2;
        if (this.arrowRotation > Math.PI / 2.0 && this.arrowRotation <= 11.0 * Math.PI / 8.0 && Game1.random.NextDouble() < (double) Game1.player.LuckLevel / 15.0)
        {
          if (specialEventVariable2)
          {
            this.arrowRotationVelocity = Math.PI / 48.0;
            Game1.playSound("dwop");
          }
        }
        else if ((this.arrowRotation + Math.PI) % (2.0 * Math.PI) <= 11.0 * Math.PI / 8.0 && !specialEventVariable2 && Game1.random.NextDouble() < (double) Game1.player.LuckLevel / 20.0)
        {
          this.arrowRotationVelocity = Math.PI / 48.0;
          Game1.playSound("dwop");
        }
      }
      if (this.arrowRotationVelocity <= 0.0 && !this.doneSpinning)
      {
        this.doneSpinning = true;
        this.arrowRotationDeceleration = 0.0;
        this.arrowRotationVelocity = 0.0;
        bool specialEventVariable2 = Game1.currentLocation.currentEvent.specialEventVariable2;
        bool flag = false;
        if (this.arrowRotation > Math.PI / 2.0 && this.arrowRotation <= 3.0 * Math.PI / 2.0)
        {
          if (!specialEventVariable2)
            flag = true;
        }
        else if (specialEventVariable2)
          flag = true;
        if (flag)
        {
          Game1.playSound("reward");
          this.resultText = new SparklingText(Game1.dialogueFont, Game1.content.LoadString("Strings\\StringsFromCSFiles:WheelSpinGame.cs.11829"), Color.Lime, Color.White);
          Game1.player.festivalScore += this.wager;
        }
        else
        {
          this.resultText = new SparklingText(Game1.dialogueFont, Game1.content.LoadString("Strings\\StringsFromCSFiles:WheelSpinGame.cs.11830"), Color.Red, Color.Transparent);
          Game1.playSound("fishEscape");
          Game1.player.festivalScore -= this.wager;
        }
      }
      double arrowRotation = this.arrowRotation;
      this.arrowRotation += this.arrowRotationVelocity;
      if (arrowRotation % (Math.PI / 2.0) > this.arrowRotation % (Math.PI / 2.0))
        Game1.playSound("Cowboy_gunshot");
      this.arrowRotation %= 2.0 * Math.PI;
    }
    else
    {
      this.timerBeforeStart -= time.ElapsedGameTime.Milliseconds;
      if (this.timerBeforeStart <= 0)
        Game1.playSound("cowboy_monsterhit");
    }
    if (this.resultText != null && this.resultText.update(time))
      this.resultText = (SparklingText) null;
    if (!this.doneSpinning || this.resultText != null)
      return;
    Game1.exitActiveMenu();
    Game1.player.canMove = true;
  }

  /// <inheritdoc />
  public override void performHoverAction(int x, int y)
  {
  }

  /// <inheritdoc />
  public override void receiveKeyPress(Keys key)
  {
  }

  /// <inheritdoc />
  public override void draw(SpriteBatch b)
  {
    b.Draw(Game1.fadeToBlackRect, new Rectangle(0, 0, Game1.uiViewport.Width, Game1.uiViewport.Height), Color.Black * 0.5f);
    b.Draw(Game1.mouseCursors, new Vector2((float) this.xPositionOnScreen, (float) this.yPositionOnScreen), new Rectangle?(new Rectangle(128 /*0x80*/, 1184, 160 /*0xA0*/, 112 /*0x70*/)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.95f);
    b.Draw(Game1.mouseCursors, new Vector2((float) (this.xPositionOnScreen + 320), (float) (this.yPositionOnScreen + 224 /*0xE0*/ + 4)), new Rectangle?(new Rectangle(120, 1234, 8, 16 /*0x10*/)), Color.White, (float) this.arrowRotation, new Vector2(4f, 15f), 4f, SpriteEffects.None, 0.96f);
    this.resultText?.draw(b, new Vector2((float) (this.xPositionOnScreen + 320) - this.resultText.textWidth, (float) (this.yPositionOnScreen - 64 /*0x40*/)));
  }
}
