// Decompiled with JetBrains decompiler
// Type: StardewValley.HUDMessage
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Extensions;
using StardewValley.Menus;
using System;

#nullable disable
namespace StardewValley;

public class HUDMessage
{
  public const float defaultTime = 3500f;
  public const int achievement_type = 1;
  public const int newQuest_type = 2;
  public const int error_type = 3;
  public const int stamina_type = 4;
  public const int health_type = 5;
  public const int screenshot_type = 6;
  /// <summary>The message text to show.</summary>
  public string message;
  /// <summary>A key used to prevent multiple HUD messages from stacking, or <c>null</c> to use the item name.</summary>
  public string type;
  /// <summary>The duration in milliseconds until the message should disappear.</summary>
  public float timeLeft;
  /// <summary>The current opacity, from 0 (fully transparent) to 1 (fully opaque).</summary>
  public float transparency = 1f;
  /// <summary>The count of the <see cref="F:StardewValley.HUDMessage.messageSubject" /> that was received, if applicable.</summary>
  public int number = -1;
  /// <summary>The icon to show, matching a constant like <see cref="F:StardewValley.HUDMessage.error_type" />.</summary>
  public int whatType;
  /// <summary>Whether this is an achievement-unlocked message.</summary>
  public bool achievement;
  /// <summary>Whether to hide the icon portion of the box.</summary>
  public bool noIcon;
  /// <summary>The item that was received, if applicable.</summary>
  public Item messageSubject;

  /// <summary>Construct an instance with the default time and an empty icon.</summary>
  /// <param name="message">The message text to show.</param>
  public HUDMessage(string message)
    : this(message, 3500f)
  {
  }

  /// <summary>Construct an instance with a specified icon type, and a duration 1.5× default.</summary>
  /// <param name="message">The message text to show.</param>
  /// <param name="whatType">The icon to show, matching a constant like <see cref="F:StardewValley.HUDMessage.error_type" />.</param>
  public HUDMessage(string message, int whatType)
    : this(message, 5250f)
  {
    this.achievement = true;
    this.whatType = whatType;
  }

  /// <summary>Construct an instance with the given values.</summary>
  /// <param name="message">The message text to show.</param>
  /// <param name="timeLeft">The duration in milliseconds for which to show the message.</param>
  /// <param name="fadeIn">Whether the message should start transparent and fade in.</param>
  public HUDMessage(string message, float timeLeft, bool fadeIn = false)
  {
    this.message = message;
    this.timeLeft = timeLeft;
    if (!fadeIn)
      return;
    this.transparency = 0.0f;
  }

  /// <summary>Construct a message indicating an item received.</summary>
  /// <param name="item">The item that was received.</param>
  /// <param name="count">The number of the item received.</param>
  /// <param name="type">A key used to prevent multiple HUD messages from stacking, or <c>null</c> to use the item name.</param>
  public static HUDMessage ForItemGained(Item item, int count, string type = null)
  {
    return new HUDMessage(item.DisplayName)
    {
      number = count,
      type = type ?? item.Name,
      messageSubject = item
    };
  }

  /// <summary>Construct a larger textbox with line wrapping and no icon.</summary>
  /// <param name="message">The message text to show.</param>
  public static HUDMessage ForCornerTextbox(string message)
  {
    message = Game1.parseText(message, Game1.dialogueFont, 384);
    return new HUDMessage(message)
    {
      noIcon = true,
      timeLeft = 5250f
    };
  }

  /// <summary>Construct an achievement display.</summary>
  /// <param name="achievementName">The translated achievement name.</param>
  public static HUDMessage ForAchievement(string achievementName)
  {
    return new HUDMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:HUDMessage.cs.3824") + achievementName, 5250f)
    {
      achievement = true,
      whatType = 1
    };
  }

  public virtual bool update(GameTime time)
  {
    this.timeLeft -= (float) time.ElapsedGameTime.Milliseconds;
    if ((double) this.timeLeft < 0.0)
    {
      this.transparency -= 0.02f;
      if ((double) this.transparency < 0.0)
        return true;
    }
    else if ((double) this.transparency < 1.0)
      this.transparency = Math.Min(this.transparency + 0.02f, 1f);
    return false;
  }

  public virtual void draw(SpriteBatch b, int i, ref int heightUsed)
  {
    Rectangle titleSafeArea = Game1.graphics.GraphicsDevice.Viewport.GetTitleSafeArea();
    if (this.noIcon)
    {
      int overrideX = titleSafeArea.Left + 16 /*0x10*/;
      int num = (int) Game1.dialogueFont.MeasureString(this.message).Y + 64 /*0x40*/;
      int overrideY = (Game1.uiViewport.Width < 1400 ? -64 : 0) + titleSafeArea.Bottom - num - heightUsed - 64 /*0x40*/;
      heightUsed += num;
      IClickableMenu.drawHoverText(b, this.message, Game1.dialogueFont, overrideX: overrideX, overrideY: overrideY, alpha: this.transparency);
    }
    else
    {
      int num = 112 /*0x70*/;
      Vector2 vector2 = new Vector2((float) (titleSafeArea.Left + 16 /*0x10*/), (float) (titleSafeArea.Bottom - num - heightUsed - 64 /*0x40*/));
      heightUsed += num;
      if (Game1.isOutdoorMapSmallerThanViewport())
        vector2.X = (float) Math.Max(titleSafeArea.Left + 16 /*0x10*/, -Game1.uiViewport.X + 16 /*0x10*/);
      if (Game1.uiViewport.Width < 1400)
        vector2.Y -= 48f;
      b.Draw(Game1.mouseCursors, vector2, new Rectangle?(!(this.messageSubject is Object messageSubject) || messageSubject.sellToStorePrice(-1L) <= 500 ? new Rectangle(293, 360, 26, 24) : new Rectangle(163, 399, 26, 24)), Color.White * this.transparency, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
      float x = Game1.smallFont.MeasureString(this.message ?? "").X;
      b.Draw(Game1.mouseCursors, new Vector2(vector2.X + 104f, vector2.Y), new Rectangle?(new Rectangle(319, 360, 1, 24)), Color.White * this.transparency, 0.0f, Vector2.Zero, new Vector2(x, 4f), SpriteEffects.None, 1f);
      b.Draw(Game1.mouseCursors, new Vector2(vector2.X + 104f + x, vector2.Y), new Rectangle?(new Rectangle(323, 360, 6, 24)), Color.White * this.transparency, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
      vector2.X += 16f;
      vector2.Y += 16f;
      if (this.messageSubject == null)
      {
        switch (this.whatType)
        {
          case 1:
            b.Draw(Game1.mouseCursors, vector2 + new Vector2(8f, 8f) * 4f, new Rectangle?(new Rectangle(294, 392, 16 /*0x10*/, 16 /*0x10*/)), Color.White * this.transparency, 0.0f, new Vector2(8f, 8f), 4f + Math.Max(0.0f, (float) (((double) this.timeLeft - 3000.0) / 900.0)), SpriteEffects.None, 1f);
            break;
          case 2:
            b.Draw(Game1.mouseCursors, vector2 + new Vector2(8f, 8f) * 4f, new Rectangle?(new Rectangle(403, 496, 5, 14)), Color.White * this.transparency, 0.0f, new Vector2(3f, 7f), 4f + Math.Max(0.0f, (float) (((double) this.timeLeft - 3000.0) / 900.0)), SpriteEffects.None, 1f);
            break;
          case 3:
            b.Draw(Game1.mouseCursors, vector2 + new Vector2(8f, 8f) * 4f, new Rectangle?(new Rectangle(268, 470, 16 /*0x10*/, 16 /*0x10*/)), Color.White * this.transparency, 0.0f, new Vector2(8f, 8f), 4f + Math.Max(0.0f, (float) (((double) this.timeLeft - 3000.0) / 900.0)), SpriteEffects.None, 1f);
            break;
          case 4:
            b.Draw(Game1.mouseCursors, vector2 + new Vector2(8f, 8f) * 4f, new Rectangle?(new Rectangle(0, 411, 16 /*0x10*/, 16 /*0x10*/)), Color.White * this.transparency, 0.0f, new Vector2(8f, 8f), 4f + Math.Max(0.0f, (float) (((double) this.timeLeft - 3000.0) / 900.0)), SpriteEffects.None, 1f);
            break;
          case 5:
            b.Draw(Game1.mouseCursors, vector2 + new Vector2(8f, 8f) * 4f, new Rectangle?(new Rectangle(16 /*0x10*/, 411, 16 /*0x10*/, 16 /*0x10*/)), Color.White * this.transparency, 0.0f, new Vector2(8f, 8f), 4f + Math.Max(0.0f, (float) (((double) this.timeLeft - 3000.0) / 900.0)), SpriteEffects.None, 1f);
            break;
          case 6:
            b.Draw(Game1.mouseCursors2, vector2 + new Vector2(8f, 8f) * 4f, new Rectangle?(new Rectangle(96 /*0x60*/, 32 /*0x20*/, 16 /*0x10*/, 16 /*0x10*/)), Color.White * this.transparency, 0.0f, new Vector2(8f, 8f), 4f + Math.Max(0.0f, (float) (((double) this.timeLeft - 3000.0) / 900.0)), SpriteEffects.None, 1f);
            break;
        }
      }
      else
        this.messageSubject.drawInMenu(b, vector2, 1f + Math.Max(0.0f, (float) (((double) this.timeLeft - 3000.0) / 900.0)), this.transparency, 1f, StackDrawType.Hide);
      vector2.X += 51f;
      vector2.Y += 51f;
      if (this.number > 1)
        Utility.drawTinyDigits(this.number, b, vector2, 3f, 1f, Color.White * this.transparency);
      vector2.X += 32f;
      vector2.Y -= 33f;
      Utility.drawTextWithShadow(b, this.message ?? "", Game1.smallFont, vector2, Game1.textColor * this.transparency, layerDepth: 1f, shadowIntensity: this.transparency);
    }
  }

  public static void numbersEasterEgg(int number)
  {
    if (number > 100000 && !Game1.player.mailReceived.Contains("numbersEgg1"))
    {
      Game1.player.mailReceived.Add("numbersEgg1");
      Game1.chatBox.addMessage("...", new Color((int) byte.MaxValue, (int) byte.MaxValue, (int) byte.MaxValue));
    }
    if (number > 200000 && !Game1.player.mailReceived.Contains("numbersEgg2"))
    {
      Game1.player.mailReceived.Add("numbersEgg2");
      Game1.chatBox.addMessage("......", new Color((int) byte.MaxValue, (int) byte.MaxValue, (int) byte.MaxValue));
    }
    if (number > 250000 && !Game1.player.mailReceived.Contains("numbersEgg3"))
    {
      Game1.player.mailReceived.Add("numbersEgg3");
      Game1.chatBox.addMessage(Game1.content.GetCurrentLanguage() == LocalizedContentManager.LanguageCode.en ? "Shooting for a million?" : "...........???", new Color((int) byte.MaxValue, (int) byte.MaxValue, (int) byte.MaxValue));
    }
    if (number > 500000 && !Game1.player.mailReceived.Contains("numbersEgg1.5"))
    {
      Game1.player.mailReceived.Add("numbersEgg1.5");
      Game1.chatBox.addMessage(".......................", new Color((int) byte.MaxValue, (int) byte.MaxValue, (int) byte.MaxValue));
    }
    if (number <= 1000000 || Game1.player.mailReceived.Contains("numbersEgg7"))
      return;
    Game1.player.mailReceived.Add("numbersEgg7");
    Game1.chatBox.addMessage(Game1.content.GetCurrentLanguage() == LocalizedContentManager.LanguageCode.en ? "[196] Secret Iridium Stackmaster Trophy Achieved [196]" : "[196]", new Color(104, 214, (int) byte.MaxValue));
    Game1.playSound("discoverMineral");
    if (Game1.content.GetCurrentLanguage() != LocalizedContentManager.LanguageCode.en)
      return;
    DelayedAction.functionAfterDelay((Action) (() => Game1.chatBox.addMessage("Qi: *slow clap*... Congratulations, kid. Ya did it. Now, on to the next challenge!", new Color(100, 50, (int) byte.MaxValue))), 6000);
  }
}
