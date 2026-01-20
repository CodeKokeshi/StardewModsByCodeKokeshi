// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.NamingMenu
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley.BellsAndWhistles;
using System;

#nullable disable
namespace StardewValley.Menus;

public class NamingMenu : IClickableMenu
{
  public const int region_okButton = 101;
  public const int region_doneNamingButton = 102;
  public const int region_randomButton = 103;
  public const int region_namingBox = 104;
  public ClickableTextureComponent doneNamingButton;
  public ClickableTextureComponent randomButton;
  public TextBox textBox;
  public ClickableComponent textBoxCC;
  public NamingMenu.doneNamingBehavior doneNaming;
  public string title;
  public int minLength = 1;
  /// <summary>Whether to apply filtering to the text input.</summary>
  public bool FilterInput = true;

  public NamingMenu(NamingMenu.doneNamingBehavior b, string title, string defaultName = null)
  {
    this.doneNaming = b;
    this.xPositionOnScreen = 0;
    this.yPositionOnScreen = 0;
    this.width = Game1.uiViewport.Width;
    this.height = Game1.uiViewport.Height;
    this.title = title;
    this.randomButton = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + this.width + 51 + 64 /*0x40*/, Game1.uiViewport.Height / 2, 64 /*0x40*/, 64 /*0x40*/), Game1.mouseCursors, new Rectangle(381, 361, 10, 10), 4f);
    this.textBox = new TextBox((Texture2D) null, (Texture2D) null, Game1.dialogueFont, Game1.textColor);
    this.textBox.X = Game1.uiViewport.Width / 2 - 192 /*0xC0*/;
    this.textBox.Y = Game1.uiViewport.Height / 2;
    this.textBox.Width = 256 /*0x0100*/;
    this.textBox.Height = 192 /*0xC0*/;
    this.textBox.OnEnterPressed += new TextBoxEvent(this.textBoxEnter);
    Game1.keyboardDispatcher.Subscriber = (IKeyboardSubscriber) this.textBox;
    this.textBox.Text = defaultName != null ? defaultName : Dialogue.randomName();
    this.textBox.Selected = true;
    ClickableTextureComponent textureComponent1 = new ClickableTextureComponent(new Rectangle(this.textBox.X + this.textBox.Width + 64 /*0x40*/ + 48 /*0x30*/ - 8, Game1.uiViewport.Height / 2 + 4, 64 /*0x40*/, 64 /*0x40*/), Game1.mouseCursors, new Rectangle(381, 361, 10, 10), 4f);
    textureComponent1.myID = 103;
    textureComponent1.leftNeighborID = 102;
    this.randomButton = textureComponent1;
    ClickableTextureComponent textureComponent2 = new ClickableTextureComponent(new Rectangle(this.textBox.X + this.textBox.Width + 32 /*0x20*/ + 4, Game1.uiViewport.Height / 2 - 8, 64 /*0x40*/, 64 /*0x40*/), Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46), 1f);
    textureComponent2.myID = 102;
    textureComponent2.rightNeighborID = 103;
    textureComponent2.leftNeighborID = 104;
    this.doneNamingButton = textureComponent2;
    this.textBoxCC = new ClickableComponent(new Rectangle(this.textBox.X, this.textBox.Y, 192 /*0xC0*/, 48 /*0x30*/), "")
    {
      myID = 104,
      rightNeighborID = 102
    };
    if (!Game1.options.SnappyMenus)
      return;
    this.populateClickableComponentList();
    this.snapToDefaultClickableComponent();
  }

  public override void snapToDefaultClickableComponent()
  {
    this.currentlySnappedComponent = this.getComponentWithID(104);
    this.snapCursorToCurrentSnappedComponent();
  }

  public void textBoxEnter(TextBox sender)
  {
    if (sender.Text.Length < this.minLength)
      return;
    if (this.doneNaming != null)
    {
      this.doneNaming(this.FilterInput ? Utility.FilterDirtyWords(sender.Text) : sender.Text);
      this.textBox.Selected = false;
    }
    else
      Game1.exitActiveMenu();
  }

  /// <inheritdoc />
  public override void receiveGamePadButton(Buttons button)
  {
    base.receiveGamePadButton(button);
    if (!this.textBox.Selected)
      return;
    switch (button)
    {
      case Buttons.DPadUp:
      case Buttons.DPadDown:
      case Buttons.DPadLeft:
      case Buttons.DPadRight:
      case Buttons.LeftThumbstickLeft:
      case Buttons.LeftThumbstickUp:
      case Buttons.LeftThumbstickDown:
      case Buttons.LeftThumbstickRight:
        this.textBox.Selected = false;
        break;
    }
  }

  /// <inheritdoc />
  public override void receiveKeyPress(Keys key)
  {
    if (this.textBox.Selected || Game1.options.doesInputListContain(Game1.options.menuButton, key))
      return;
    base.receiveKeyPress(key);
  }

  /// <inheritdoc />
  public override void performHoverAction(int x, int y)
  {
    base.performHoverAction(x, y);
    if (this.doneNamingButton != null)
    {
      if (this.doneNamingButton.containsPoint(x, y))
        this.doneNamingButton.scale = Math.Min(1.1f, this.doneNamingButton.scale + 0.05f);
      else
        this.doneNamingButton.scale = Math.Max(1f, this.doneNamingButton.scale - 0.05f);
    }
    this.randomButton.tryHover(x, y, 0.5f);
  }

  /// <inheritdoc />
  public override void receiveLeftClick(int x, int y, bool playSound = true)
  {
    base.receiveLeftClick(x, y, playSound);
    this.textBox.Update();
    if (this.doneNamingButton.containsPoint(x, y))
    {
      this.textBoxEnter(this.textBox);
      Game1.playSound("smallSelect");
    }
    else
    {
      if (!this.randomButton.containsPoint(x, y))
        return;
      this.textBox.Text = Dialogue.randomName();
      this.randomButton.scale = this.randomButton.baseScale;
      Game1.playSound("drumkit6");
    }
  }

  /// <inheritdoc />
  public override void draw(SpriteBatch b)
  {
    base.draw(b);
    if (!Game1.options.showClearBackgrounds)
      b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
    SpriteText.drawStringWithScrollCenteredAt(b, this.title, Game1.uiViewport.Width / 2, Game1.uiViewport.Height / 2 - 128 /*0x80*/, this.title);
    this.textBox.Draw(b);
    this.doneNamingButton.draw(b);
    this.randomButton.draw(b);
    this.drawMouse(b);
  }

  public delegate void doneNamingBehavior(string s);
}
