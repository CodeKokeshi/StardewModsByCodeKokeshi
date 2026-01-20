// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.SpecialCurrencyDisplay
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.BellsAndWhistles;
using System;
using System.Collections.Generic;

#nullable disable
namespace StardewValley.Menus;

public class SpecialCurrencyDisplay
{
  /// <summary>The currency ID for golden walnuts.</summary>
  public const string currency_walnuts = "walnuts";
  /// <summary>The currency ID for Qi gems.</summary>
  public const string currency_qiGems = "qiGems";
  /// <summary>The default <see cref="F:StardewValley.Menus.SpecialCurrencyDisplay.CurrencyRenderInfo.timeToLive" /> value.</summary>
  public const int defaultSeconds = 5;
  /// <summary>The currencies which can be displayed, indexed by currency ID like <see cref="F:StardewValley.Menus.SpecialCurrencyDisplay.currency_walnuts" />.</summary>
  public Dictionary<string, SpecialCurrencyDisplay.CurrencyDisplayType> registeredCurrencyDisplays = new Dictionary<string, SpecialCurrencyDisplay.CurrencyDisplayType>();
  /// <summary>The currencies from <see cref="F:StardewValley.Menus.SpecialCurrencyDisplay.registeredCurrencyDisplays" /> to render.</summary>
  public readonly List<SpecialCurrencyDisplay.CurrencyRenderInfo> displayedCurrencies = new List<SpecialCurrencyDisplay.CurrencyRenderInfo>();

  /// <summary>Register a currency which can be rendered manually (via <see cref="M:StardewValley.Menus.SpecialCurrencyDisplay.ShowCurrency(System.String,System.Func{System.Boolean},System.Single)" />) or automatically (via <see cref="!:field" /> event listeners).</summary>
  /// <param name="key">The currency ID, like <see cref="F:StardewValley.Menus.SpecialCurrencyDisplay.currency_walnuts" />.</param>
  /// <param name="field">The field which contains the currency amount.</param>
  /// <param name="playSound">Play a sound when the currency amount changes, or <c>null</c> to play the default sound.</param>
  /// <param name="drawIcon">Draw the currency sprite at the given position, or <c>null</c> to draw the default sprite.</param>
  public virtual void Register(
    string key,
    NetIntDelta field,
    Action<int> playSound = null,
    Action<SpriteBatch, Vector2> drawIcon = null)
  {
    if (this.registeredCurrencyDisplays.ContainsKey(key))
      this.Unregister(key);
    playSound = playSound ?? (Action<int>) (delta => this.PlaySound(key, delta));
    drawIcon = drawIcon ?? (Action<SpriteBatch, Vector2>) ((b, position) => this.DrawIcon(key, b, position));
    this.registeredCurrencyDisplays[key] = new SpecialCurrencyDisplay.CurrencyDisplayType()
    {
      key = key,
      field = field,
      playSound = playSound,
      drawIcon = drawIcon
    };
    field.fieldChangeVisibleEvent += new FieldChange<NetIntDelta, int>(this.OnCurrencyChange);
  }

  /// <summary>Show the currency display.</summary>
  /// <param name="currency">The currency ID to display (like <see cref="F:StardewValley.Menus.SpecialCurrencyDisplay.currency_walnuts" />).</param>
  /// <param name="keepOpen">If set, pause the <see cref="!:timeToLive" /> until it returns false.</param>
  /// <param name="timeToLive">The number of seconds until the currency disappears.</param>
  public virtual void ShowCurrency(string currency, Func<bool> keepOpen = null, float timeToLive = 5f)
  {
    if (currency == null)
      return;
    foreach (SpecialCurrencyDisplay.CurrencyRenderInfo displayedCurrency in this.displayedCurrencies)
    {
      if (displayedCurrency.currency.key == currency)
      {
        displayedCurrency.keepOpen = keepOpen ?? displayedCurrency.keepOpen;
        displayedCurrency.timeToLive = Math.Max(displayedCurrency.timeToLive, timeToLive);
        return;
      }
    }
    SpecialCurrencyDisplay.CurrencyDisplayType currency1;
    if (this.registeredCurrencyDisplays.TryGetValue(currency, out currency1))
      this.displayedCurrencies.Add(new SpecialCurrencyDisplay.CurrencyRenderInfo(currency1, keepOpen, timeToLive));
    else
      Game1.log.Warn($"Can't show unknown currency type '{currency}'.");
  }

  /// <summary>Hide a currency if it's displayed.</summary>
  /// <param name="currency">The currency ID to hide (like <see cref="F:StardewValley.Menus.SpecialCurrencyDisplay.currency_walnuts" />).</param>
  /// <param name="immediate">Remove the currency immediately, instead of letting it slide out.</param>
  public virtual void HideCurrency(string currency, bool immediate = true)
  {
    if (immediate)
    {
      this.displayedCurrencies.RemoveAll((Predicate<SpecialCurrencyDisplay.CurrencyRenderInfo>) (p => p.currency.key == currency));
    }
    else
    {
      foreach (SpecialCurrencyDisplay.CurrencyRenderInfo displayedCurrency in this.displayedCurrencies)
      {
        if (displayedCurrency.currency.key == currency)
        {
          displayedCurrency.keepOpen = (Func<bool>) null;
          displayedCurrency.timeToLive = 0.0f;
        }
      }
    }
  }

  /// <summary>Update the display if needed when a currency value changes.</summary>
  /// <param name="field">The field containing the currency value.</param>
  /// <param name="oldValue">The previous currency value.</param>
  /// <param name="newValue">The new currency value.</param>
  public virtual void OnCurrencyChange(NetIntDelta field, int oldValue, int newValue)
  {
    if (Game1.gameMode != (byte) 3 || oldValue == newValue)
      return;
    foreach (SpecialCurrencyDisplay.CurrencyRenderInfo displayedCurrency in this.displayedCurrencies)
    {
      if (displayedCurrency.currency.field == field)
      {
        displayedCurrency.OnCurrencyChanged(oldValue, newValue);
        return;
      }
    }
    foreach (SpecialCurrencyDisplay.CurrencyDisplayType currency in this.registeredCurrencyDisplays.Values)
    {
      if (currency.field == field)
      {
        SpecialCurrencyDisplay.CurrencyRenderInfo currencyRenderInfo = new SpecialCurrencyDisplay.CurrencyRenderInfo(currency);
        currencyRenderInfo.OnCurrencyChanged(oldValue, newValue);
        this.displayedCurrencies.Add(currencyRenderInfo);
        return;
      }
    }
    Game1.log.Warn($"Can't show currency change for unknown field '{field.Name}'.");
  }

  /// <summary>Remove a currency that was registered via <see cref="M:StardewValley.Menus.SpecialCurrencyDisplay.Register(System.String,Netcode.NetIntDelta,System.Action{System.Int32},System.Action{Microsoft.Xna.Framework.Graphics.SpriteBatch,Microsoft.Xna.Framework.Vector2})" />.</summary>
  /// <param name="key">The currency ID, like <see cref="F:StardewValley.Menus.SpecialCurrencyDisplay.currency_walnuts" />.</param>
  public virtual void Unregister(string key)
  {
    this.HideCurrency(key);
    SpecialCurrencyDisplay.CurrencyDisplayType currencyDisplayType;
    if (!this.registeredCurrencyDisplays.TryGetValue(key, out currencyDisplayType))
      return;
    currencyDisplayType.field.fieldChangeVisibleEvent -= new FieldChange<NetIntDelta, int>(this.OnCurrencyChange);
    this.registeredCurrencyDisplays.Remove(key);
  }

  /// <summary>Unregister all currencies.</summary>
  public virtual void Cleanup()
  {
    foreach (string key in new List<string>((IEnumerable<string>) this.registeredCurrencyDisplays.Keys))
      this.Unregister(key);
  }

  /// <summary>Draw the default icon for a currency.</summary>
  /// <param name="currency">The currency ID to render.</param>
  /// <param name="b">The sprite batch being drawn.</param>
  /// <param name="position">The position at which to draw the icon.</param>
  public virtual void DrawIcon(string currency, SpriteBatch b, Vector2 position)
  {
    switch (currency)
    {
      case "walnuts":
        b.Draw(Game1.objectSpriteSheet, position, new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, 73, 16 /*0x10*/, 16 /*0x10*/)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.0f);
        break;
      case "qiGems":
        b.Draw(Game1.objectSpriteSheet, position, new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, 858, 16 /*0x10*/, 16 /*0x10*/)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.0f);
        break;
    }
  }

  /// <summary>Play the default sound.</summary>
  /// <param name="currency">The currency ID whose sound to play.</param>
  /// <param name="direction">The change to the currency value.</param>
  public virtual void PlaySound(string currency, int direction)
  {
    if (!(currency == "walnuts"))
      return;
    Game1.playSound("goldenWalnut");
  }

  /// <summary>Update the display if it's currently active.</summary>
  /// <param name="time">The elapsed game time.</param>
  public virtual void Update(GameTime time)
  {
    for (int index = 0; index < this.displayedCurrencies.Count; ++index)
    {
      SpecialCurrencyDisplay.CurrencyRenderInfo displayedCurrency = this.displayedCurrencies[index];
      Func<bool> keepOpen = displayedCurrency.keepOpen;
      bool flag = keepOpen != null && keepOpen();
      if (!flag)
      {
        displayedCurrency.keepOpen = (Func<bool>) null;
        displayedCurrency.timeToLive -= (float) time.ElapsedGameTime.TotalSeconds;
        if ((double) displayedCurrency.timeToLive < 0.0)
          displayedCurrency.timeToLive = 0.0f;
      }
      float num = (float) time.ElapsedGameTime.TotalSeconds / 0.5f;
      displayedCurrency.slidePosition += flag || (double) displayedCurrency.timeToLive > 0.0 ? num : -num;
      displayedCurrency.slidePosition = Utility.Clamp(displayedCurrency.slidePosition, 0.0f, 1f);
      if (!flag && (double) displayedCurrency.timeToLive <= 0.0 && (double) displayedCurrency.slidePosition <= 0.0)
      {
        this.displayedCurrencies.RemoveAt(index);
        --index;
      }
    }
  }

  /// <summary>Get the default draw position.</summary>
  /// <param name="slidePosition">The slide position of the display, as a value between 0 (hidden) and 1 (fully displayed)..</param>
  public Vector2 GetUpperLeft(float slidePosition)
  {
    return new Vector2(16f, (float) ((int) Utility.Lerp(-26f, 0.0f, slidePosition) * 4));
  }

  /// <summary>Draw the currency display if needed.</summary>
  /// <param name="b">The sprite batch being drawn.</param>
  public virtual void Draw(SpriteBatch b)
  {
    if (this.displayedCurrencies.Count == 0)
      return;
    int num = 0;
    foreach (SpecialCurrencyDisplay.CurrencyRenderInfo displayedCurrency in this.displayedCurrencies)
    {
      MoneyDial moneyDial = displayedCurrency.moneyDial;
      Vector2 upperLeft = this.GetUpperLeft(displayedCurrency.slidePosition);
      if (num > 0)
        upperLeft.X += (float) num;
      Rectangle rectangle = new Rectangle(48 /*0x30*/, 176 /*0xB0*/, 52, 26);
      b.Draw(Game1.mouseCursors2, upperLeft, new Rectangle?(rectangle), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.0f);
      num += rectangle.Width * 4;
      int previousTargetValue = displayedCurrency.currency.field.Value;
      if ((double) displayedCurrency.slidePosition < 0.5)
        previousTargetValue = moneyDial.previousTargetValue;
      moneyDial.draw(b, upperLeft + new Vector2(108f, 40f), previousTargetValue);
      Action<SpriteBatch, Vector2> drawIcon = displayedCurrency.currency.drawIcon;
      if (drawIcon != null)
        drawIcon(b, upperLeft + new Vector2(4f, 6f) * 4f);
    }
  }

  /// <summary>Draw a currency display to the screen.</summary>
  /// <param name="b">The sprite batch being drawn.</param>
  /// <param name="drawPosition">The position at which to draw the display.</param>
  /// <param name="moneyDial">The currency dial to render.</param>
  /// <param name="displayedValue">The currency value.</param>
  /// <param name="drawSpriteTexture">The sprite texture for the currency icon.</param>
  /// <param name="drawSpriteSourceRect">The pixel area within the <paramref name="drawSpriteTexture" /> for the currency icon.</param>
  public static void Draw(
    SpriteBatch b,
    Vector2 drawPosition,
    MoneyDial moneyDial,
    int displayedValue,
    Texture2D drawSpriteTexture,
    Rectangle drawSpriteSourceRect)
  {
    if (moneyDial != null && moneyDial.numDigits > 3)
      b.Draw(Game1.mouseCursors_1_6, drawPosition, new Rectangle?(new Rectangle(42, 0, 57, 26)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.0f);
    else
      b.Draw(Game1.mouseCursors2, drawPosition, new Rectangle?(new Rectangle(48 /*0x30*/, 176 /*0xB0*/, 52, 26)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.0f);
    moneyDial?.draw(b, drawPosition + new Vector2(108f, 40f), displayedValue);
    b.Draw(drawSpriteTexture, drawPosition + new Vector2(4f, 6f) * 4f, new Rectangle?(drawSpriteSourceRect), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.0f);
  }

  /// <summary>Draw a very basic static money dial which can only do 3 digits.</summary>
  /// <param name="b">The sprite batch being drawn.</param>
  /// <param name="drawPosition">The position at which to draw the display.</param>
  /// <param name="displayedValue">The currency value.</param>
  /// <param name="drawSpriteTexture">The sprite texture for the currency icon.</param>
  /// <param name="drawSpriteSourceRect">The pixel area within the <paramref name="drawSpriteTexture" /> for the currency icon.</param>
  public static void Draw(
    SpriteBatch b,
    Vector2 drawPosition,
    int displayedValue,
    Texture2D drawSpriteTexture,
    Rectangle drawSpriteSourceRect)
  {
    b.Draw(Game1.mouseCursors2, drawPosition, new Rectangle?(new Rectangle(48 /*0x30*/, 176 /*0xB0*/, 52, 26)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.0f);
    int num1 = 3;
    int x = 0;
    int num2 = (int) Math.Pow(10.0, (double) (num1 - 1));
    bool flag = false;
    for (int index = 0; index < num1; ++index)
    {
      int num3 = displayedValue / num2 % 10;
      if (num3 > 0 || index == num1 - 1)
        flag = true;
      if (flag)
        b.Draw(Game1.mouseCursors, drawPosition + new Vector2(108f, 40f) + new Vector2((float) x, 0.0f), new Rectangle?(new Rectangle(286, 502 - num3 * 8, 5, 8)), Color.Maroon, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
      x += 24;
      num2 /= 10;
    }
    b.Draw(drawSpriteTexture, drawPosition + new Vector2(4f, 6f) * 4f, new Rectangle?(drawSpriteSourceRect), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.0f);
  }

  /// <summary>The metadata for a currency which can be rendered by <see cref="T:StardewValley.Menus.SpecialCurrencyDisplay" />.</summary>
  public class CurrencyDisplayType
  {
    /// <summary>The currency ID, like <see cref="F:StardewValley.Menus.SpecialCurrencyDisplay.currency_walnuts" />.</summary>
    public string key;
    /// <summary>The field which contains the currency amount.</summary>
    public NetIntDelta field;
    /// <summary>Play a sound when the currency amount changes.</summary>
    public Action<int> playSound;
    /// <summary>Draw the currency sprite at the given position.</summary>
    public Action<SpriteBatch, Vector2> drawIcon;
  }

  /// <summary>The render info for a currency being drawn to the screen.</summary>
  public class CurrencyRenderInfo
  {
    /// <summary>The currency to display.</summary>
    public SpecialCurrencyDisplay.CurrencyDisplayType currency;
    /// <summary>The currency dial UI to render.</summary>
    public MoneyDial moneyDial = new MoneyDial(3)
    {
      onPlaySound = (Action<int>) null
    };
    /// <summary>The slide position of the display, as a value between 0 (hidden) and 1 (fully displayed).</summary>
    public float slidePosition;
    /// <summary>If set, pause the <see cref="F:StardewValley.Menus.SpecialCurrencyDisplay.CurrencyRenderInfo.timeToLive" /> until it returns false.</summary>
    public Func<bool> keepOpen;
    /// <summary>The number of seconds until the <see cref="F:StardewValley.Menus.SpecialCurrencyDisplay.displayedCurrencies" /> begins to slide out of view.</summary>
    public float timeToLive;

    /// <summary>Construct an instance.</summary>
    /// <param name="currency">The currency ID to display (like <see cref="F:StardewValley.Menus.SpecialCurrencyDisplay.currency_walnuts" />), or <c>null</c> to hide it.</param>
    /// <param name="keepOpen">If set, pause the <paramref name="timeToLive" /> until it returns false.</param>
    /// <param name="timeToLive">The number of seconds until the currency disappears.</param>
    public CurrencyRenderInfo(
      SpecialCurrencyDisplay.CurrencyDisplayType currency,
      Func<bool> keepOpen = null,
      float timeToLive = 5f)
    {
      this.currency = currency;
      this.keepOpen = keepOpen;
      this.timeToLive = timeToLive;
      this.moneyDial.currentValue = currency.field.TargetValue;
      this.moneyDial.previousTargetValue = currency.field.Value;
      this.moneyDial.onPlaySound = currency.playSound;
    }

    public void OnCurrencyChanged(int oldValue, int newValue)
    {
      this.timeToLive = Math.Max(this.timeToLive, 5f);
      this.moneyDial.currentValue = oldValue;
      Action<int> onPlaySound = this.moneyDial.onPlaySound;
      if (onPlaySound == null)
        return;
      onPlaySound(newValue - oldValue);
    }
  }
}
