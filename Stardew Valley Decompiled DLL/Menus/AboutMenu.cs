// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.AboutMenu
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

#nullable disable
namespace StardewValley.Menus;

public class AboutMenu : IClickableMenu
{
  public const int region_upArrow = 94444;
  public const int region_downArrow = 95555;
  public new const int height = 700;
  public ClickableComponent backButton;
  public ClickableTextureComponent upButton;
  public ClickableTextureComponent downButton;
  public List<ICreditsBlock> credits = new List<ICreditsBlock>();
  private int currentCreditsIndex;

  public AboutMenu()
  {
    this.width = 1280 /*0x0500*/;
    base.height = 700;
    this.SetUpCredits();
    if (!Game1.options.snappyMenus || !Game1.options.gamepadControls)
      return;
    this.populateClickableComponentList();
    this.snapToDefaultClickableComponent();
  }

  public void SetUpCredits()
  {
    foreach (string rawtext in Game1.temporaryContent.Load<List<string>>("Strings\\credits"))
    {
      if (rawtext != null && rawtext.Length >= 6 && rawtext.StartsWith("[image"))
      {
        string[] strArray = ArgUtility.SplitBySpace(rawtext);
        string assetName = strArray[1];
        int int32_1 = Convert.ToInt32(strArray[2]);
        int int32_2 = Convert.ToInt32(strArray[3]);
        int width = Convert.ToInt32(strArray[4]);
        int height = Convert.ToInt32(strArray[5]);
        int int32_3 = Convert.ToInt32(strArray[6]);
        int animationFrames = strArray.Length > 7 ? Convert.ToInt32(strArray[7]) : 1;
        Texture2D texture = (Texture2D) null;
        try
        {
          texture = Game1.temporaryContent.Load<Texture2D>(assetName);
        }
        catch (Exception ex)
        {
        }
        if (texture != null)
        {
          if (width == -1)
          {
            width = texture.Width;
            height = texture.Height;
          }
          this.credits.Add((ICreditsBlock) new ImageCreditsBlock(texture, new Rectangle(int32_1, int32_2, width, height), int32_3, animationFrames));
        }
      }
      else if (rawtext != null && rawtext.Length >= 6 && rawtext.StartsWith("[link"))
      {
        string[] strArray = ArgUtility.SplitBySpace(rawtext, 3);
        this.credits.Add((ICreditsBlock) new LinkCreditsBlock(strArray[2], strArray[1]));
      }
      else
        this.credits.Add((ICreditsBlock) new TextCreditsBlock(rawtext));
    }
    Vector2 centeringOnScreen = Utility.getTopLeftPositionForCenteringOnScreen(this.width, base.height);
    this.xPositionOnScreen = (int) centeringOnScreen.X;
    this.yPositionOnScreen = (int) centeringOnScreen.Y;
    ClickableTextureComponent textureComponent1 = new ClickableTextureComponent(new Rectangle((int) centeringOnScreen.X + this.width - 80 /*0x50*/, (int) centeringOnScreen.Y + 64 /*0x40*/ + 16 /*0x10*/, 64 /*0x40*/, 64 /*0x40*/), Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 12), 0.8f);
    textureComponent1.myID = 94444;
    textureComponent1.downNeighborID = 95555;
    textureComponent1.rightNeighborID = -99998;
    textureComponent1.leftNeighborID = -99998;
    this.upButton = textureComponent1;
    ClickableTextureComponent textureComponent2 = new ClickableTextureComponent(new Rectangle((int) centeringOnScreen.X + this.width - 80 /*0x50*/, (int) centeringOnScreen.Y + base.height - 32 /*0x20*/, 64 /*0x40*/, 64 /*0x40*/), Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 11), 0.8f);
    textureComponent2.myID = 95555;
    textureComponent2.upNeighborID = -99998;
    textureComponent2.rightNeighborID = -99998;
    textureComponent2.leftNeighborID = -99998;
    this.downButton = textureComponent2;
    this.backButton = new ClickableComponent(new Rectangle(Game1.uiViewport.Width + -66 * TitleMenu.pixelZoom - 8 * TitleMenu.pixelZoom * 2, Game1.uiViewport.Height - 27 * TitleMenu.pixelZoom - 8 * TitleMenu.pixelZoom, 66 * TitleMenu.pixelZoom, 27 * TitleMenu.pixelZoom), "")
    {
      myID = 81114,
      leftNeighborID = -99998,
      rightNeighborID = -99998,
      upNeighborID = 95555
    };
  }

  public override void snapToDefaultClickableComponent()
  {
    this.currentlySnappedComponent = this.getComponentWithID(81114);
    this.snapCursorToCurrentSnappedComponent();
  }

  /// <inheritdoc />
  public override void receiveLeftClick(int x, int y, bool playSound = true)
  {
    base.receiveLeftClick(x, y, playSound);
    if (this.upButton.containsPoint(x, y))
    {
      if (this.currentCreditsIndex <= 0)
        return;
      --this.currentCreditsIndex;
      Game1.playSound("shiny4");
      this.upButton.scale = this.upButton.baseScale;
    }
    else if (this.downButton.containsPoint(x, y))
    {
      if (this.currentCreditsIndex >= this.credits.Count - 1)
        return;
      ++this.currentCreditsIndex;
      Game1.playSound("shiny4");
      this.downButton.scale = this.downButton.baseScale;
    }
    else
    {
      if (!this.isWithinBounds(x, y))
        return;
      int num1 = this.yPositionOnScreen + 96 /*0x60*/;
      int num2 = num1;
      int num3 = 0;
      while (num1 < this.yPositionOnScreen + base.height - 64 /*0x40*/ && this.credits.Count > this.currentCreditsIndex + num3)
      {
        num1 += this.credits[this.currentCreditsIndex + num3].getHeight(this.width - 64 /*0x40*/) + (this.credits.Count <= this.currentCreditsIndex + num3 + 1 || !(this.credits[this.currentCreditsIndex + num3 + 1] is ImageCreditsBlock) ? 8 : 0);
        if (y >= num2 && y < num1)
        {
          this.credits[this.currentCreditsIndex + num3].clicked();
          break;
        }
        ++num3;
        num2 = num1;
      }
    }
  }

  /// <inheritdoc />
  public override void update(GameTime time)
  {
    base.update(time);
    this.upButton.visible = this.currentCreditsIndex > 0;
    this.downButton.visible = this.currentCreditsIndex < this.credits.Count - 1;
  }

  /// <inheritdoc />
  public override void receiveScrollWheelAction(int direction)
  {
    if (direction > 0 && this.currentCreditsIndex > 0)
    {
      --this.currentCreditsIndex;
      Game1.playSound("shiny4");
    }
    else
    {
      if (direction >= 0 || this.currentCreditsIndex >= this.credits.Count - 1)
        return;
      ++this.currentCreditsIndex;
      Game1.playSound("shiny4");
    }
  }

  /// <inheritdoc />
  public override void performHoverAction(int x, int y)
  {
    base.performHoverAction(x, y);
    this.upButton.tryHover(x, y);
    this.downButton.tryHover(x, y);
    if (!this.isWithinBounds(x, y))
      return;
    int num1 = this.yPositionOnScreen + 96 /*0x60*/;
    int num2 = num1;
    int num3 = 0;
    while (num1 < this.yPositionOnScreen + base.height - 64 /*0x40*/ && this.credits.Count > this.currentCreditsIndex + num3)
    {
      num1 += this.credits[this.currentCreditsIndex + num3].getHeight(this.width - 64 /*0x40*/) + (this.credits.Count <= this.currentCreditsIndex + num3 + 1 || !(this.credits[this.currentCreditsIndex + num3 + 1] is ImageCreditsBlock) ? 8 : 0);
      if (y >= num2 && y < num1)
      {
        this.credits[this.currentCreditsIndex + num3].hovered();
        break;
      }
      ++num3;
      num2 = num1;
    }
  }

  /// <inheritdoc />
  public override void draw(SpriteBatch b)
  {
    Vector2 centeringOnScreen = Utility.getTopLeftPositionForCenteringOnScreen(this.width, base.height - 100);
    if (!Game1.options.showClearBackgrounds)
      b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
    IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(473, 36, 24, 24), (int) centeringOnScreen.X, (int) centeringOnScreen.Y, this.width, base.height, Color.White, 4f, false);
    int topLeftY = this.yPositionOnScreen + 96 /*0x60*/;
    for (int index = 0; topLeftY < this.yPositionOnScreen + base.height - 64 /*0x40*/ && this.credits.Count > this.currentCreditsIndex + index; ++index)
    {
      this.credits[this.currentCreditsIndex + index].draw(this.xPositionOnScreen + 32 /*0x20*/, topLeftY, this.width - 64 /*0x40*/, b);
      topLeftY += this.credits[this.currentCreditsIndex + index].getHeight(this.width - 64 /*0x40*/) + (this.credits.Count <= this.currentCreditsIndex + index + 1 || !(this.credits[this.currentCreditsIndex + index + 1] is ImageCreditsBlock) ? 8 : 0);
    }
    if (this.currentCreditsIndex > 0)
      this.upButton.draw(b);
    if (this.currentCreditsIndex < this.credits.Count - 1)
      this.downButton.draw(b);
    string text1 = "v" + Game1.GetVersionString();
    float y1 = Game1.smallFont.MeasureString(text1).Y;
    b.DrawString(Game1.smallFont, text1, new Vector2(16f, (float) ((double) Game1.uiViewport.Height - (double) y1 - 8.0)), Color.White);
    if (Game1.activeClickableMenu is TitleMenu activeClickableMenu && !string.IsNullOrWhiteSpace(activeClickableMenu.startupMessage))
    {
      string text2 = Game1.parseText(activeClickableMenu.startupMessage, Game1.smallFont, 640);
      float y2 = Game1.smallFont.MeasureString(text2).Y;
      b.DrawString(Game1.smallFont, text2, new Vector2(8f, (float) ((double) Game1.uiViewport.Height - (double) y1 - (double) y2 - 4.0)), Color.White);
    }
    base.draw(b);
  }

  /// <inheritdoc />
  public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
  {
    base.gameWindowSizeChanged(oldBounds, newBounds);
    this.SetUpCredits();
    if (!Game1.options.snappyMenus || !Game1.options.gamepadControls)
      return;
    int id = this.currentlySnappedComponent != null ? this.currentlySnappedComponent.myID : 81114;
    this.populateClickableComponentList();
    this.currentlySnappedComponent = this.getComponentWithID(id);
    this.snapCursorToCurrentSnappedComponent();
  }
}
