// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.OptionsElement
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley.BellsAndWhistles;

#nullable disable
namespace StardewValley.Menus;

public class OptionsElement : IScreenReadable
{
  public const int defaultX = 8;
  public const int defaultY = 4;
  public const int defaultPixelWidth = 9;
  public Rectangle bounds;
  public string label;
  public int whichOption;
  public bool greyedOut;
  public Vector2 labelOffset = Vector2.Zero;
  public OptionsElement.Style style;

  /// <inheritdoc />
  public string ScreenReaderText { get; set; }

  /// <inheritdoc />
  public string ScreenReaderDescription { get; set; }

  /// <inheritdoc />
  public bool ScreenReaderIgnore { get; set; }

  public OptionsElement(string label)
  {
    this.label = label;
    this.bounds = new Rectangle(32 /*0x20*/, 16 /*0x10*/, 36, 36);
    this.whichOption = -1;
  }

  public OptionsElement(string label, int x, int y, int width, int height, int whichOption = -1)
  {
    if (x == -1)
      x = 32 /*0x20*/;
    if (y == -1)
      y = 16 /*0x10*/;
    this.bounds = new Rectangle(x, y, width, height);
    this.label = label;
    this.whichOption = whichOption;
  }

  public OptionsElement(string label, Rectangle bounds, int whichOption)
  {
    this.whichOption = whichOption;
    this.label = label;
    this.bounds = bounds;
  }

  /// <summary>Handle a user left-click on the element (including a 'click' through controller selection).</summary>
  /// <param name="x">The pixel X coordinate that was clicked.</param>
  /// <param name="y">The pixel Y coordinate that was clicked.</param>
  public virtual void receiveLeftClick(int x, int y)
  {
  }

  /// <summary>Handle the left-click button being held down (including a button resulting in a 'click' through controller selection). This is called each tick that it's held.</summary>
  /// <param name="x">The cursor's current pixel X coordinate.</param>
  /// <param name="y">The cursor's current pixel Y coordinate.</param>
  public virtual void leftClickHeld(int x, int y)
  {
  }

  /// <summary>Handle the left-click button being released (including a button resulting in a 'click' through controller selection).</summary>
  /// <param name="x">The cursor's current pixel X coordinate.</param>
  /// <param name="y">The cursor's current pixel Y coordinate.</param>
  public virtual void leftClickReleased(int x, int y)
  {
  }

  /// <summary>Handle a keyboard button pressed.</summary>
  /// <param name="key">The keyboard button that was pressed.</param>
  public virtual void receiveKeyPress(Keys key)
  {
  }

  /// <summary>Render the element.</summary>
  /// <param name="b">The sprite batch being drawn.</param>
  /// <param name="slotX">The pixel X position at which to draw, relative to the bounds.</param>
  /// <param name="slotY">The pixel Y position at which to draw, relative to the bounds.</param>
  /// <param name="context">The menu which contains this element, if applicable.</param>
  public virtual void draw(SpriteBatch b, int slotX, int slotY, IClickableMenu context = null)
  {
    if (this.style == OptionsElement.Style.OptionLabel)
      Utility.drawTextWithShadow(b, this.label, Game1.dialogueFont, new Vector2((float) (slotX + this.bounds.X + (int) this.labelOffset.X), (float) (slotY + this.bounds.Y + (int) this.labelOffset.Y + 12)), this.greyedOut ? Game1.textColor * 0.33f : Game1.textColor, layerDepth: 0.1f);
    else if (this.whichOption == -1)
    {
      SpriteText.drawString(b, this.label, slotX + this.bounds.X + (int) this.labelOffset.X, slotY + this.bounds.Y + (int) this.labelOffset.Y + 56 - SpriteText.getHeightOfString(this.label), 999, height: 999, layerDepth: 0.1f);
    }
    else
    {
      int x = slotX + this.bounds.X + this.bounds.Width + 8 + (int) this.labelOffset.X;
      int y = slotY + this.bounds.Y + (int) this.labelOffset.Y;
      string text = this.label;
      SpriteFont spriteFont = Game1.dialogueFont;
      if (context != null)
      {
        int num = context.width - 64 /*0x40*/;
        int positionOnScreen = context.xPositionOnScreen;
        if ((double) spriteFont.MeasureString(this.label).X + (double) x > (double) (num + positionOnScreen))
        {
          int width = num + positionOnScreen - x;
          spriteFont = Game1.smallFont;
          text = Game1.parseText(this.label, spriteFont, width);
          y -= (int) (((double) spriteFont.MeasureString(text).Y - (double) spriteFont.MeasureString("T").Y) / 2.0);
        }
      }
      Utility.drawTextWithShadow(b, text, spriteFont, new Vector2((float) x, (float) y), this.greyedOut ? Game1.textColor * 0.33f : Game1.textColor, layerDepth: 0.1f);
    }
  }

  public enum Style
  {
    Default,
    OptionLabel,
  }
}
