// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.TextBox
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley.SDKs.Steam;

#nullable disable
namespace StardewValley.Menus;

public class TextBox : IKeyboardSubscriber
{
  protected Texture2D _textBoxTexture;
  protected Texture2D _caretTexture;
  protected SpriteFont _font;
  protected Color _textColor;
  public bool numbersOnly;
  public int textLimit = -1;
  public bool limitWidth = true;
  private string _text = "";
  private bool _showKeyboard;
  private bool _selected;

  public SpriteFont Font => this._font;

  public Color TextColor => this._textColor;

  public int X { get; set; }

  public int Y { get; set; }

  public int Width { get; set; }

  public int Height { get; set; }

  public bool PasswordBox { get; set; }

  public string Text
  {
    get => this._text;
    set
    {
      this._text = value;
      if (this._text == null)
        this._text = "";
      if (!(this._text != ""))
        return;
      this._text = Utility.FilterDirtyWordsIfStrictPlatform(this._text);
      if (!this.limitWidth || (double) this._font.MeasureString(this._text).X <= (double) (this.Width - 21))
        return;
      this.Text = this._text.Substring(0, this._text.Length - 1);
    }
  }

  /// <summary>Displayed as the title for virtual keyboards.</summary>
  public string TitleText { get; set; }

  public TextBox(
    Texture2D textBoxTexture,
    Texture2D caretTexture,
    SpriteFont font,
    Color textColor)
  {
    this._textBoxTexture = textBoxTexture;
    if (textBoxTexture != null)
    {
      this.Width = textBoxTexture.Width;
      this.Height = textBoxTexture.Height;
    }
    this._caretTexture = caretTexture;
    this._font = font;
    this._textColor = textColor;
  }

  public void SelectMe() => this.Selected = true;

  public void Update()
  {
    this.Selected = new Rectangle(this.X, this.Y, this.Width, this.Height).Contains(new Point(Game1.getMouseX(), Game1.getMouseY()));
    if (!this._showKeyboard)
      return;
    if (Game1.options.gamepadControls && !Game1.lastCursorMotionWasMouse)
      Game1.showTextEntry(this);
    this._showKeyboard = false;
  }

  public virtual void Draw(SpriteBatch spriteBatch, bool drawShadow = true)
  {
    bool flag = Game1.currentGameTime.TotalGameTime.TotalMilliseconds % 1000.0 >= 500.0;
    string text = this.Text;
    if (this.PasswordBox)
    {
      text = "";
      for (int index = 0; index < this.Text.Length; ++index)
        text += "•";
    }
    if (this._textBoxTexture != null)
    {
      spriteBatch.Draw(this._textBoxTexture, new Rectangle(this.X, this.Y, 16 /*0x10*/, this.Height), new Rectangle?(new Rectangle(0, 0, 16 /*0x10*/, this.Height)), Color.White);
      spriteBatch.Draw(this._textBoxTexture, new Rectangle(this.X + 16 /*0x10*/, this.Y, this.Width - 32 /*0x20*/, this.Height), new Rectangle?(new Rectangle(16 /*0x10*/, 0, 4, this.Height)), Color.White);
      spriteBatch.Draw(this._textBoxTexture, new Rectangle(this.X + this.Width - 16 /*0x10*/, this.Y, 16 /*0x10*/, this.Height), new Rectangle?(new Rectangle(this._textBoxTexture.Bounds.Width - 16 /*0x10*/, 0, 16 /*0x10*/, this.Height)), Color.White);
    }
    else
      Game1.drawDialogueBox(this.X - 32 /*0x20*/, this.Y - 112 /*0x70*/ + 10, this.Width + 80 /*0x50*/, this.Height, false, true);
    Vector2 vector2;
    for (vector2 = this._font.MeasureString(text); (double) vector2.X > (double) this.Width; vector2 = this._font.MeasureString(text))
      text = text.Substring(1);
    if (flag && this.Selected)
      spriteBatch.Draw(Game1.staminaRect, new Rectangle(this.X + 16 /*0x10*/ + (int) vector2.X + 2, this.Y + 8, 4, 32 /*0x20*/), this._textColor);
    if (drawShadow)
      Utility.drawTextWithShadow(spriteBatch, text, this._font, new Vector2((float) (this.X + 16 /*0x10*/), (float) (this.Y + (this._textBoxTexture != null ? 12 : 8))), this._textColor);
    else
      spriteBatch.DrawString(this._font, text, new Vector2((float) (this.X + 16 /*0x10*/), (float) (this.Y + (this._textBoxTexture != null ? 12 : 8))), this._textColor, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.99f);
  }

  public virtual void RecieveTextInput(char inputChar)
  {
    if (!this.Selected || this.numbersOnly && !char.IsDigit(inputChar) || this.textLimit != -1 && this.Text.Length >= this.textLimit)
      return;
    if (Game1.gameMode != (byte) 3)
    {
      switch (inputChar)
      {
        case '"':
          return;
        case '$':
          Game1.playSound("money");
          break;
        case '*':
          Game1.playSound("hammer");
          break;
        case '+':
          Game1.playSound("slimeHit");
          break;
        case '<':
          Game1.playSound("crystal", new int?(0));
          break;
        case '=':
          Game1.playSound("coin");
          break;
        default:
          Game1.playSound("cowboy_monsterhit");
          break;
      }
    }
    this.Text += inputChar.ToString();
  }

  public virtual void RecieveTextInput(string text)
  {
    int result = -1;
    if (!this.Selected || this.numbersOnly && !int.TryParse(text, out result) || this.textLimit != -1 && this.Text.Length >= this.textLimit)
      return;
    this.Text += text;
  }

  public virtual void RecieveCommandInput(char command)
  {
    if (!this.Selected)
      return;
    switch (command)
    {
      case '\b':
        if (this.Text.Length <= 0)
          break;
        if (this.OnBackspacePressed != null)
        {
          this.OnBackspacePressed(this);
          break;
        }
        this.Text = this.Text.Substring(0, this.Text.Length - 1);
        if (Game1.gameMode == (byte) 3)
          break;
        Game1.playSound("tinyWhip");
        break;
      case '\t':
        TextBoxEvent onTabPressed = this.OnTabPressed;
        if (onTabPressed == null)
          break;
        onTabPressed(this);
        break;
      case '\r':
        TextBoxEvent onEnterPressed = this.OnEnterPressed;
        if (onEnterPressed == null)
          break;
        onEnterPressed(this);
        break;
    }
  }

  public void RecieveSpecialInput(Keys key)
  {
  }

  public void Hover(int x, int y)
  {
    if (x <= this.X || x >= this.X + this.Width || y <= this.Y || y >= this.Y + this.Height)
      return;
    Game1.SetFreeCursorDrag();
  }

  public event TextBoxEvent OnEnterPressed;

  public event TextBoxEvent OnTabPressed;

  public event TextBoxEvent OnBackspacePressed;

  public bool Selected
  {
    get => this._selected;
    set
    {
      if (this._selected == value)
        return;
      this._selected = value;
      if (this._selected)
      {
        Game1.keyboardDispatcher.Subscriber = (IKeyboardSubscriber) this;
        this._showKeyboard = true;
      }
      else
      {
        this._showKeyboard = false;
        if (Program.sdk is SteamHelper sdk && sdk.active)
          sdk.CancelKeyboard();
        if (Game1.keyboardDispatcher.Subscriber != this)
          return;
        Game1.keyboardDispatcher.Subscriber = (IKeyboardSubscriber) null;
      }
    }
  }
}
