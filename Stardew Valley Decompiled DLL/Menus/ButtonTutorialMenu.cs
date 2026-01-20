// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.ButtonTutorialMenu
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

#nullable disable
namespace StardewValley.Menus;

public class ButtonTutorialMenu : IClickableMenu
{
  public const int move_run_check = 0;
  public const int useTool_menu = 1;
  public const float movementSpeed = 0.2f;
  public new const int width = 42;
  public new const int height = 109;
  private int timerToclose = 15000;
  private int which;
  private static int current;
  private int myID;

  public ButtonTutorialMenu(int which)
    : base(-168, Game1.uiViewport.Height / 2 - 218, 168, 436)
  {
    this.which = which;
    ++ButtonTutorialMenu.current;
    this.myID = ButtonTutorialMenu.current;
  }

  /// <inheritdoc />
  public override void update(GameTime time)
  {
    base.update(time);
    if (this.myID != ButtonTutorialMenu.current)
      this.destroy = true;
    if (this.xPositionOnScreen < 0 && this.timerToclose > 0)
    {
      this.xPositionOnScreen += (int) ((double) time.ElapsedGameTime.Milliseconds * 0.20000000298023224);
      if (this.xPositionOnScreen < 0)
        return;
      this.xPositionOnScreen = 0;
    }
    else
    {
      int timerToclose = this.timerToclose;
      TimeSpan elapsedGameTime = time.ElapsedGameTime;
      int milliseconds = elapsedGameTime.Milliseconds;
      this.timerToclose = timerToclose - milliseconds;
      if (this.timerToclose > 0)
        return;
      if (this.xPositionOnScreen >= -232)
      {
        int positionOnScreen = this.xPositionOnScreen;
        elapsedGameTime = time.ElapsedGameTime;
        int num = (int) ((double) elapsedGameTime.Milliseconds * 0.20000000298023224);
        this.xPositionOnScreen = positionOnScreen - num;
      }
      else
        this.destroy = true;
    }
  }

  /// <inheritdoc />
  public override void draw(SpriteBatch b)
  {
    if (this.destroy)
      return;
    if (!Game1.options.gamepadControls)
      b.Draw(Game1.mouseCursors, new Vector2((float) this.xPositionOnScreen, (float) this.yPositionOnScreen), new Rectangle?(new Rectangle(275 + this.which * 42, 0, 42, 109)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.82f);
    else
      b.Draw(Game1.controllerMaps, new Vector2((float) this.xPositionOnScreen, (float) this.yPositionOnScreen), new Rectangle?(Utility.controllerMapSourceRect(new Rectangle(512 /*0x0200*/ + this.which * 42 * 2, 0, 84, 218))), Color.White, 0.0f, Vector2.Zero, 2f, SpriteEffects.None, 0.82f);
  }
}
