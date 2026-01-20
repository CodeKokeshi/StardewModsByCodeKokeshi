// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.OptionsButton
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

#nullable disable
namespace StardewValley.Menus;

public class OptionsButton : OptionsElement
{
  private Action action;

  public OptionsButton(string label, Action action)
    : base(label)
  {
    this.action = action;
    this.bounds = new Rectangle(32 /*0x20*/, 0, (int) Game1.dialogueFont.MeasureString(label).X + 64 /*0x40*/, 68);
  }

  /// <inheritdoc />
  public override void receiveLeftClick(int x, int y)
  {
    if (!this.greyedOut && this.bounds.Contains(x, y) && this.action != null)
      this.action();
    base.receiveLeftClick(x, y);
  }

  /// <inheritdoc />
  public override void draw(SpriteBatch b, int slotX, int slotY, IClickableMenu context = null)
  {
    float draw_layer = (float) (0.800000011920929 - (double) (slotY + this.bounds.Y) * 9.9999999747524271E-07);
    IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(432, 439, 9, 9), slotX + this.bounds.X, slotY + this.bounds.Y, this.bounds.Width, this.bounds.Height, Color.White * (this.greyedOut ? 0.33f : 1f), 4f, draw_layer: draw_layer);
    Vector2 vector2 = Game1.dialogueFont.MeasureString(this.label) / 2f;
    vector2.X = (float) ((int) ((double) vector2.X / 4.0) * 4);
    vector2.Y = (float) ((int) ((double) vector2.Y / 4.0) * 4);
    Utility.drawTextWithShadow(b, this.label, Game1.dialogueFont, new Vector2((float) (slotX + this.bounds.Center.X), (float) (slotY + this.bounds.Center.Y)) - vector2, Game1.textColor * (this.greyedOut ? 0.33f : 1f), layerDepth: draw_layer + 1E-06f, shadowIntensity: 0.0f);
  }
}
