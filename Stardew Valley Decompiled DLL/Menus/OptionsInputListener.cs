// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.OptionsInputListener
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace StardewValley.Menus;

public class OptionsInputListener : OptionsElement
{
  public List<string> buttonNames = new List<string>();
  private string listenerMessage;
  private bool listening;
  private Rectangle setbuttonBounds;
  public static Rectangle setButtonSource = new Rectangle(294, 428, 21, 11);

  public OptionsInputListener(string label, int whichOption, int slotWidth, int x = -1, int y = -1)
    : base(label, x, y, slotWidth - x, 44, whichOption)
  {
    this.setbuttonBounds = new Rectangle(slotWidth - 112 /*0x70*/, y + 12, 84, 44);
    if (whichOption == -1)
      return;
    Game1.options.setInputListenerToProperValue(this);
  }

  /// <inheritdoc />
  public override void receiveLeftClick(int x, int y)
  {
    if (this.greyedOut || this.listening || !this.setbuttonBounds.Contains(x, y))
      return;
    if (this.whichOption == -1)
    {
      Game1.options.setControlsToDefault();
      if (!(Game1.activeClickableMenu is GameMenu activeClickableMenu) || !(activeClickableMenu.GetCurrentPage() is OptionsPage currentPage))
        return;
      foreach (OptionsElement option in currentPage.options)
      {
        if (option is OptionsInputListener inputListener)
          Game1.options.setInputListenerToProperValue(inputListener);
      }
    }
    else
    {
      this.listening = true;
      Game1.playSound("breathin");
      GameMenu.forcePreventClose = true;
      this.listenerMessage = Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsElement.cs.11225");
    }
  }

  /// <inheritdoc />
  public override void receiveKeyPress(Keys key)
  {
    if (this.greyedOut || !this.listening)
      return;
    if (Game1.activeClickableMenu is GameMenu activeClickableMenu && activeClickableMenu.GetCurrentPage() is OptionsPage currentPage)
      currentPage.lastRebindTick = Game1.ticks;
    if (key == Keys.Escape)
    {
      Game1.playSound("bigDeSelect");
      this.listening = false;
      GameMenu.forcePreventClose = false;
    }
    else if (!Game1.options.isKeyInUse(key) || new InputButton(key).ToString().Equals(this.buttonNames[0]))
    {
      Game1.options.changeInputListenerValue(this.whichOption, key);
      this.buttonNames[0] = new InputButton(key).ToString();
      Game1.playSound("coin");
      this.listening = false;
      GameMenu.forcePreventClose = false;
    }
    else
      this.listenerMessage = Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsElement.cs.11228");
  }

  /// <inheritdoc />
  public override void draw(SpriteBatch b, int slotX, int slotY, IClickableMenu context = null)
  {
    if (this.buttonNames.Count > 0 || this.whichOption == -1)
    {
      if (this.whichOption == -1)
        Utility.drawTextWithShadow(b, this.label, Game1.dialogueFont, new Vector2((float) (this.bounds.X + slotX), (float) (this.bounds.Y + slotY)), Game1.textColor, layerDepth: 0.15f);
      else
        Utility.drawTextWithShadow(b, $"{this.label}: {this.buttonNames.Last<string>()}{(this.buttonNames.Count > 1 ? ", " + this.buttonNames[0] : "")}", Game1.dialogueFont, new Vector2((float) (this.bounds.X + slotX), (float) (this.bounds.Y + slotY)), Game1.textColor, layerDepth: 0.15f);
    }
    Utility.drawWithShadow(b, Game1.mouseCursors, new Vector2((float) (this.setbuttonBounds.X + slotX), (float) (this.setbuttonBounds.Y + slotY)), OptionsInputListener.setButtonSource, Color.White, 0.0f, Vector2.Zero, 4f, layerDepth: 0.15f);
    if (!this.listening)
      return;
    b.Draw(Game1.staminaRect, new Rectangle(0, 0, Game1.uiViewport.Width, Game1.uiViewport.Height), new Rectangle?(new Rectangle(0, 0, 1, 1)), Color.Black * 0.75f, 0.0f, Vector2.Zero, SpriteEffects.None, 0.999f);
    b.DrawString(Game1.dialogueFont, this.listenerMessage, Utility.getTopLeftPositionForCenteringOnScreen(192 /*0xC0*/, 64 /*0x40*/), Color.White, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.9999f);
  }
}
