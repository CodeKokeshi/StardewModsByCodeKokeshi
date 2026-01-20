// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.OptionsCheckbox
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#nullable disable
namespace StardewValley.Menus;

public class OptionsCheckbox : OptionsElement
{
  public const int pixelsWide = 9;
  public static OptionsCheckbox selected;
  public bool isChecked;
  public static Rectangle sourceRectUnchecked = new Rectangle(227, 425, 9, 9);
  public static Rectangle sourceRectChecked = new Rectangle(236, 425, 9, 9);

  public OptionsCheckbox(string label, int whichOption, int x = -1, int y = -1)
    : base(label, x, y, 36, 36, whichOption)
  {
    Game1.options.setCheckBoxToProperValue(this);
  }

  /// <inheritdoc />
  public override void receiveLeftClick(int x, int y)
  {
    if (this.greyedOut)
      return;
    Game1.playSound("drumkit6");
    OptionsCheckbox.selected = this;
    base.receiveLeftClick(x, y);
    this.isChecked = !this.isChecked;
    Game1.options.changeCheckBoxOption(this.whichOption, this.isChecked);
    OptionsCheckbox.selected = (OptionsCheckbox) null;
  }

  /// <inheritdoc />
  public override void draw(SpriteBatch b, int slotX, int slotY, IClickableMenu context = null)
  {
    b.Draw(Game1.mouseCursors, new Vector2((float) (slotX + this.bounds.X), (float) (slotY + this.bounds.Y)), new Rectangle?(this.isChecked ? OptionsCheckbox.sourceRectChecked : OptionsCheckbox.sourceRectUnchecked), Color.White * (this.greyedOut ? 0.33f : 1f), 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.4f);
    base.draw(b, slotX, slotY, context);
  }
}
