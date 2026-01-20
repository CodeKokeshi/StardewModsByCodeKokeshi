// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.DiscreteColorPicker
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Objects;

#nullable disable
namespace StardewValley.Menus;

public class DiscreteColorPicker : IClickableMenu
{
  public const int sizeOfEachSwatch = 7;
  public Item itemToDrawColored;
  public bool visible = true;
  public static int totalColors = 21;
  public int colorSelection;

  public DiscreteColorPicker(
    int xPosition,
    int yPosition,
    int startingColor = 0,
    Item itemToDrawColored = null)
  {
    this.xPositionOnScreen = xPosition;
    this.yPositionOnScreen = yPosition;
    this.width = DiscreteColorPicker.totalColors * 9 * 4 + IClickableMenu.borderWidth;
    this.height = 28 + IClickableMenu.borderWidth;
    this.colorSelection = startingColor == 0 || !(DiscreteColorPicker.getColorFromSelection(startingColor) != Color.Black) ? 0 : startingColor;
    this.itemToDrawColored = itemToDrawColored;
    if (this.itemToDrawColored is Chest itemToDrawColored1)
      itemToDrawColored1.resetLidFrame();
    this.visible = Game1.player.showChestColorPicker;
  }

  public DiscreteColorPicker(
    int xPosition,
    int yPosition,
    Color startingColor,
    Item itemToDrawColored = null)
    : this(xPosition, yPosition, DiscreteColorPicker.getSelectionFromColor(startingColor), itemToDrawColored)
  {
  }

  public static int getSelectionFromColor(Color c)
  {
    for (int selection = 0; selection < DiscreteColorPicker.totalColors; ++selection)
    {
      if (DiscreteColorPicker.getColorFromSelection(selection).Equals(c))
        return selection;
    }
    return -1;
  }

  public Color getCurrentColor() => DiscreteColorPicker.getColorFromSelection(this.colorSelection);

  /// <inheritdoc />
  public override void receiveLeftClick(int x, int y, bool playSound = true)
  {
    if (!this.visible)
      return;
    base.receiveLeftClick(x, y, playSound);
    Rectangle rectangle = new Rectangle(this.xPositionOnScreen + IClickableMenu.borderWidth / 2, this.yPositionOnScreen + IClickableMenu.borderWidth / 2, 36 * DiscreteColorPicker.totalColors, 28);
    if (!rectangle.Contains(x, y))
      return;
    this.colorSelection = (x - rectangle.X) / 36;
    try
    {
      Game1.playSound("coin");
    }
    catch
    {
    }
    if (!(this.itemToDrawColored is Chest itemToDrawColored))
      return;
    itemToDrawColored.playerChoiceColor.Value = DiscreteColorPicker.getColorFromSelection(this.colorSelection);
    itemToDrawColored.resetLidFrame();
  }

  public static Color getColorFromSelection(int selection)
  {
    switch (selection)
    {
      case 1:
        return new Color(85, 85, (int) byte.MaxValue);
      case 2:
        return new Color(119, 191, (int) byte.MaxValue);
      case 3:
        return new Color(0, 170, 170);
      case 4:
        return new Color(0, 234, 175);
      case 5:
        return new Color(0, 170, 0);
      case 6:
        return new Color(159, 236, 0);
      case 7:
        return new Color((int) byte.MaxValue, 234, 18);
      case 8:
        return new Color((int) byte.MaxValue, 167, 18);
      case 9:
        return new Color((int) byte.MaxValue, 105, 18);
      case 10:
        return new Color((int) byte.MaxValue, 0, 0);
      case 11:
        return new Color(135, 0, 35);
      case 12:
        return new Color((int) byte.MaxValue, 173, 199);
      case 13:
        return new Color((int) byte.MaxValue, 117, 195);
      case 14:
        return new Color(172, 0, 198);
      case 15:
        return new Color(143, 0, (int) byte.MaxValue);
      case 16 /*0x10*/:
        return new Color(89, 11, 142);
      case 17:
        return new Color(64 /*0x40*/, 64 /*0x40*/, 64 /*0x40*/);
      case 18:
        return new Color(100, 100, 100);
      case 19:
        return new Color(200, 200, 200);
      case 20:
        return new Color(254, 254, 254);
      default:
        return Color.Black;
    }
  }

  /// <inheritdoc />
  public override void draw(SpriteBatch b)
  {
    if (!this.visible)
      return;
    IClickableMenu.drawTextureBox(b, this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height, Color.LightGray);
    for (int selection = 0; selection < DiscreteColorPicker.totalColors; ++selection)
    {
      if (selection == 0)
        b.Draw(Game1.mouseCursors, new Vector2((float) (this.xPositionOnScreen + IClickableMenu.borderWidth / 2), (float) (this.yPositionOnScreen + IClickableMenu.borderWidth / 2)), new Rectangle?(new Rectangle(295, 503, 7, 7)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.88f);
      else
        b.Draw(Game1.staminaRect, new Rectangle(this.xPositionOnScreen + IClickableMenu.borderWidth / 2 + selection * 9 * 4, this.yPositionOnScreen + IClickableMenu.borderWidth / 2, 28, 28), DiscreteColorPicker.getColorFromSelection(selection));
      if (selection == this.colorSelection)
        IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(375, 357, 3, 3), this.xPositionOnScreen + IClickableMenu.borderWidth / 2 - 4 + selection * 9 * 4, this.yPositionOnScreen + IClickableMenu.borderWidth / 2 - 4, 36, 36, Color.Black, 4f, false);
    }
    if (!(this.itemToDrawColored is Chest itemToDrawColored))
      return;
    itemToDrawColored.draw(b, this.xPositionOnScreen + this.width + IClickableMenu.borderWidth / 2, this.yPositionOnScreen + 16 /*0x10*/, local: true);
  }
}
