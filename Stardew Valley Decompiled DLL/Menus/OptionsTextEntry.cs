// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.OptionsTextEntry
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#nullable disable
namespace StardewValley.Menus;

public class OptionsTextEntry : OptionsElement
{
  public const int pixelsHigh = 11;
  public TextBox textBox;

  public OptionsTextEntry(string label, int whichOption, int x = -1, int y = -1)
    : base(label, x, y, (int) Game1.smallFont.MeasureString("Windowed Borderless Mode   ").X + 48 /*0x30*/, 44, whichOption)
  {
    this.textBox = new TextBox(Game1.content.Load<Texture2D>("LooseSprites\\textBox"), (Texture2D) null, Game1.smallFont, Color.Black);
    this.textBox.Width = this.bounds.Width;
  }

  /// <inheritdoc />
  public override void draw(SpriteBatch b, int slotX, int slotY, IClickableMenu context = null)
  {
    this.textBox.X = slotX + this.bounds.Left - 8;
    this.textBox.Y = slotY + this.bounds.Top;
    this.textBox.Draw(b);
    base.draw(b, slotX, slotY, context);
  }

  /// <inheritdoc />
  public override void receiveLeftClick(int x, int y)
  {
    this.textBox.SelectMe();
    this.textBox.Update();
  }
}
