// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.TitleTextInputMenu
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

#nullable disable
namespace StardewValley.Menus;

public class TitleTextInputMenu : NamingMenu
{
  public ClickableTextureComponent pasteButton;
  public const int region_pasteButton = 105;
  public string context = "";

  public TitleTextInputMenu(
    string title,
    NamingMenu.doneNamingBehavior b,
    string default_text = "",
    string context = "",
    bool filterInput = true)
    : base(b, title, "")
  {
    this.FilterInput = filterInput;
    this.context = context;
    this.textBox.limitWidth = false;
    this.textBox.Width = 512 /*0x0200*/;
    this.textBox.X -= 128 /*0x80*/;
    this.randomButton.visible = false;
    ClickableTextureComponent textureComponent = new ClickableTextureComponent(new Rectangle(this.textBox.X + this.textBox.Width + 32 /*0x20*/ + 4 + 64 /*0x40*/, Game1.viewport.Height / 2 - 8, 64 /*0x40*/, 64 /*0x40*/), Game1.mouseCursors, new Rectangle(274, 284, 16 /*0x10*/, 16 /*0x10*/), 4f);
    textureComponent.myID = 105;
    textureComponent.leftNeighborID = 102;
    this.pasteButton = textureComponent;
    this.pasteButton.visible = true;
    this.doneNamingButton.rightNeighborID = 105;
    this.doneNamingButton.bounds.X += 128 /*0x80*/;
    this.minLength = 0;
    if (Game1.options.SnappyMenus)
    {
      this.populateClickableComponentList();
      this.snapToDefaultClickableComponent();
    }
    this.textBox.Text = default_text;
  }

  /// <inheritdoc />
  public override void performHoverAction(int x, int y)
  {
    this.pasteButton?.tryHover(x, y);
    base.performHoverAction(x, y);
  }

  /// <inheritdoc />
  public override void receiveLeftClick(int x, int y, bool playSound = true)
  {
    if (this.pasteButton != null && this.pasteButton.containsPoint(x, y))
    {
      string output = "";
      if (DesktopClipboard.GetText(ref output))
      {
        Game1.playSound("drumkit6");
        this.textBox.Text = output;
      }
      else
        Game1.playSound("cancel");
    }
    base.receiveLeftClick(x, y, playSound);
  }

  /// <inheritdoc />
  public override void update(GameTime time)
  {
    GamePadState gamePadState = Game1.input.GetGamePadState();
    KeyboardState keyboardState = Game1.GetKeyboardState();
    if (Game1.IsPressEvent(ref gamePadState, Buttons.B) || Game1.IsPressEvent(ref keyboardState, Keys.Escape))
    {
      if (Game1.activeClickableMenu is TitleMenu activeClickableMenu)
        activeClickableMenu.backButtonPressed();
      else
        Game1.exitActiveMenu();
    }
    base.update(time);
  }

  /// <inheritdoc />
  public override void draw(SpriteBatch b)
  {
    base.draw(b);
    this.pasteButton?.draw(b);
  }
}
