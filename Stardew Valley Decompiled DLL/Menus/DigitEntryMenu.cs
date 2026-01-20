// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.DigitEntryMenu
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.BellsAndWhistles;
using System;
using System.Collections.Generic;

#nullable disable
namespace StardewValley.Menus;

internal class DigitEntryMenu : NumberSelectionMenu
{
  public List<ClickableComponent> digits = new List<ClickableComponent>();
  private int calculatorX;
  private int calculatorY;
  private int calculatorWidth;
  private int calculatorHeight;
  private static string clear = "c";

  public DigitEntryMenu(
    string message,
    NumberSelectionMenu.behaviorOnNumberSelect behaviorOnSelection,
    int price = -1,
    int minValue = 0,
    int maxValue = 99,
    int defaultNumber = 0)
    : base(message, behaviorOnSelection, price, minValue, maxValue, defaultNumber)
  {
    int num1 = 3;
    int width = 44;
    int height = width;
    int num2 = 8;
    int num3 = num2;
    int num4 = num1 * width + (num1 - 1) * num2;
    this.calculatorWidth = width * num1 + num2 * (num1 - 1) + IClickableMenu.spaceToClearSideBorder * 2 + 128 /*0x80*/;
    this.calculatorHeight = height * 4 + num3 * 3 + IClickableMenu.spaceToClearTopBorder * 2;
    this.calculatorX = Game1.uiViewport.Width / 2 - this.calculatorWidth / 2;
    this.calculatorY = Game1.uiViewport.Height / 2 - this.calculatorHeight;
    int num5 = Game1.uiViewport.Width / 2;
    int num6 = Game1.uiViewport.Height / 2 - 384 + 24 + IClickableMenu.spaceToClearTopBorder;
    for (int index = 0; index < 11; ++index)
    {
      string name = index == 9 ? DigitEntryMenu.clear : (index == 10 ? "0" : (index + 1).ToString());
      this.digits.Add(new ClickableComponent(new Rectangle(num5 - num4 / 2 + index % num1 * (num2 + width), num6 + index / num1 * (num3 + height), width, height), name)
      {
        myID = index,
        rightNeighborID = -99998,
        leftNeighborID = -99998,
        downNeighborID = -99998,
        upNeighborID = -99998
      });
    }
    this.populateClickableComponentList();
  }

  protected override Vector2 centerPosition
  {
    get
    {
      return new Vector2((float) (Game1.uiViewport.Width / 2), (float) (Game1.uiViewport.Height / 2 + 128 /*0x80*/));
    }
  }

  private void onDigitPressed(string digit)
  {
    if (digit == DigitEntryMenu.clear)
    {
      this.currentValue = 0;
      this.numberSelectedBox.Text = this.currentValue.ToString();
    }
    else
    {
      string str = this.currentValue.ToString();
      this.currentValue = Math.Min(this.maxValue, Convert.ToInt32(!(str == "0") ? str + digit : digit));
      this.numberSelectedBox.Text = this.currentValue.ToString();
    }
  }

  public override bool isWithinBounds(int x, int y)
  {
    if (base.isWithinBounds(x, y))
      return true;
    return x - this.calculatorX < this.calculatorWidth && x - this.calculatorX >= 0 && y - this.calculatorY < this.calculatorHeight && y - this.calculatorY >= 0;
  }

  /// <inheritdoc />
  public override void receiveLeftClick(int x, int y, bool playSound = true)
  {
    foreach (ClickableComponent digit in this.digits)
    {
      if (digit.containsPoint(x, y))
      {
        Game1.playSound("smallSelect");
        this.onDigitPressed(digit.name);
      }
    }
    base.receiveLeftClick(x, y, true);
  }

  /// <inheritdoc />
  public override void performHoverAction(int x, int y)
  {
    base.performHoverAction(x, y);
    foreach (ClickableComponent digit in this.digits)
      digit.scale = !digit.containsPoint(x, y) ? 1f : 2f;
  }

  /// <inheritdoc />
  public override void draw(SpriteBatch b)
  {
    base.draw(b);
    Game1.drawDialogueBox(this.calculatorX, this.calculatorY, this.calculatorWidth, this.calculatorHeight, false, true);
    foreach (ClickableComponent digit in this.digits)
    {
      if (digit.name == DigitEntryMenu.clear)
      {
        b.Draw(Game1.mouseCursors, new Vector2((float) (digit.bounds.X - 4), (float) (digit.bounds.Y + 4)), new Rectangle?(new Rectangle((double) digit.scale > 1.0 ? 267 : 256 /*0x0100*/, 256 /*0x0100*/, 10, 10)), Color.Black * 0.5f, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.865f);
        b.Draw(Game1.mouseCursors, new Vector2((float) digit.bounds.X, (float) digit.bounds.Y), new Rectangle?(new Rectangle((double) digit.scale > 1.0 ? 267 : 256 /*0x0100*/, 256 /*0x0100*/, 10, 10)), Color.White * 0.6f, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.868f);
        Vector2 vector2 = new Vector2((float) (digit.bounds.X + digit.bounds.Width / 2 - SpriteText.getWidthOfString(digit.name) / 2), (float) (digit.bounds.Y + digit.bounds.Height / 2 - SpriteText.getHeightOfString(digit.name) / 2 - 4));
        SpriteText.drawString(b, digit.name, (int) vector2.X, (int) vector2.Y);
      }
      else
      {
        b.Draw(Game1.mouseCursors, new Vector2((float) (digit.bounds.X - 4), (float) (digit.bounds.Y + 4)), new Rectangle?(new Rectangle((double) digit.scale > 1.0 ? 267 : 256 /*0x0100*/, 256 /*0x0100*/, 10, 10)), Color.Black * 0.5f, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.865f);
        b.Draw(Game1.mouseCursors, new Vector2((float) digit.bounds.X, (float) digit.bounds.Y), new Rectangle?(new Rectangle((double) digit.scale > 1.0 ? 267 : 256 /*0x0100*/, 256 /*0x0100*/, 10, 10)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.868f);
        Vector2 position = new Vector2((float) (digit.bounds.X + 16 /*0x10*/ + NumberSprite.numberOfDigits(Convert.ToInt32(digit.name)) * 6), (float) (digit.bounds.Y + 24 - NumberSprite.getHeight() / 4));
        NumberSprite.draw(Convert.ToInt32(digit.name), b, position, Color.Gold, 0.5f, 0.86f, 1f, 0);
      }
    }
    this.drawMouse(b);
  }
}
