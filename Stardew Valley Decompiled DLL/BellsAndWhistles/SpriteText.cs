// Decompiled with JetBrains decompiler
// Type: StardewValley.BellsAndWhistles.SpriteText
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using BmFont;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace StardewValley.BellsAndWhistles;

public class SpriteText
{
  public const int scrollStyle_scroll = 0;
  public const int scrollStyle_speechBubble = 1;
  public const int scrollStyle_darkMetal = 2;
  public const int scrollStyle_blueMetal = 3;
  public const int maxCharacter = 999999;
  public const int maxHeight = 999999;
  public const int characterWidth = 8;
  public const int characterHeight = 16 /*0x10*/;
  public const int horizontalSpaceBetweenCharacters = 0;
  public const int verticalSpaceBetweenCharacters = 2;
  public const char newLine = '^';
  public static float fontPixelZoom = 3f;
  public static float shadowAlpha = 0.15f;
  public static Dictionary<char, FontChar> characterMap;
  public static FontFile FontFile = (FontFile) null;
  public static List<Texture2D> fontPages = (List<Texture2D>) null;
  public static Texture2D spriteTexture;
  public static Texture2D coloredTexture;
  public const int color_index_Default = -1;
  public const int color_index_Black = 0;
  public const int color_index_Blue = 1;
  public const int color_index_Red = 2;
  public const int color_index_Purple = 3;
  public const int color_index_White = 4;
  public const int color_index_Orange = 5;
  public const int color_index_Green = 6;
  public const int color_index_Cyan = 7;
  public const int color_index_Gray = 8;
  public const int color_index_JojaBlue = 9;
  public static bool forceEnglishFont = false;

  public static float FontPixelZoom
  {
    get
    {
      return SpriteText.fontPixelZoom + (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.zh ? (float) (((double) Game1.options.dialogueFontScale - 1.0) / (Game1.options.useChineseSmoothFont ? 4.0 : 2.0)) : 0.0f);
    }
  }

  public static void drawStringHorizontallyCenteredAt(
    SpriteBatch b,
    string s,
    int x,
    int y,
    int characterPosition = 999999,
    int width = -1,
    int height = 999999,
    float alpha = 1f,
    float layerDepth = 0.88f,
    bool junimoText = false,
    Color? color = null,
    int maxWidth = 99999)
  {
    SpriteText.drawString(b, s, x - SpriteText.getWidthOfString(s, maxWidth) / 2, y, characterPosition, width, height, alpha, layerDepth, junimoText, color: color);
  }

  public static int getWidthOfString(string s, int widthConstraint = 999999)
  {
    SpriteText.setUpCharacterMap();
    int val1 = 0;
    int val2 = 0;
    for (int index = 0; index < s.Length; ++index)
    {
      if (SpriteText.isUsingNonSpriteSheetFont() && !SpriteText.forceEnglishFont)
      {
        FontChar fontChar;
        if (SpriteText.characterMap.TryGetValue(s[index], out fontChar))
          val1 += fontChar.XAdvance;
        val2 = Math.Max(val1, val2);
        if (s[index] == '^' || (double) val1 * (double) SpriteText.FontPixelZoom > (double) widthConstraint)
          val1 = 0;
      }
      else
      {
        val1 += 8 + SpriteText.getWidthOffsetForChar(s[index]);
        if (index > 0)
          val1 += SpriteText.getWidthOffsetForChar(s[Math.Max(0, index - 1)]);
        val2 = Math.Max(val1, val2);
        float num = (float) SpriteText.positionOfNextSpace(s, index, (int) ((double) val1 * (double) SpriteText.FontPixelZoom), 0);
        if (s[index] == '^' || (double) val1 * (double) SpriteText.FontPixelZoom >= (double) widthConstraint || (double) num >= (double) widthConstraint)
          val1 = 0;
      }
    }
    return (int) ((double) val2 * (double) SpriteText.FontPixelZoom);
  }

  public static bool IsMissingCharacters(string text)
  {
    SpriteText.setUpCharacterMap();
    if (!LocalizedContentManager.CurrentLanguageLatin && !SpriteText.forceEnglishFont)
    {
      for (int index = 0; index < text.Length; ++index)
      {
        if (!SpriteText.characterMap.ContainsKey(text[index]))
          return true;
      }
    }
    return false;
  }

  public static int getHeightOfString(string s, int widthConstraint = 999999)
  {
    if (s.Length == 0)
      return 0;
    Vector2 vector2 = new Vector2();
    int accumulatedHorizontalSpaceBetweenCharacters = 0;
    s = s.Replace(Environment.NewLine, "");
    SpriteText.setUpCharacterMap();
    if (SpriteText.isUsingNonSpriteSheetFont() && !SpriteText.forceEnglishFont)
    {
      for (int index = 0; index < s.Length; ++index)
      {
        if (s[index] == '^')
        {
          vector2.Y += (float) (SpriteText.FontFile.Common.LineHeight + 2) * SpriteText.FontPixelZoom;
          vector2.X = 0.0f;
        }
        else
        {
          if (SpriteText.positionOfNextSpace(s, index, (int) vector2.X, accumulatedHorizontalSpaceBetweenCharacters) >= widthConstraint)
          {
            vector2.Y += (float) (SpriteText.FontFile.Common.LineHeight + 2) * SpriteText.FontPixelZoom;
            accumulatedHorizontalSpaceBetweenCharacters = 0;
            vector2.X = 0.0f;
          }
          FontChar fontChar;
          if (SpriteText.characterMap.TryGetValue(s[index], out fontChar))
            vector2.X += (float) fontChar.XAdvance * SpriteText.FontPixelZoom;
        }
      }
      return (int) ((double) vector2.Y + (double) (SpriteText.FontFile.Common.LineHeight + 2) * (double) SpriteText.FontPixelZoom);
    }
    for (int index = 0; index < s.Length; ++index)
    {
      if (s[index] == '^')
      {
        vector2.Y += 18f * SpriteText.FontPixelZoom;
        vector2.X = 0.0f;
        accumulatedHorizontalSpaceBetweenCharacters = 0;
      }
      else
      {
        if (SpriteText.positionOfNextSpace(s, index, (int) vector2.X, accumulatedHorizontalSpaceBetweenCharacters) >= widthConstraint)
        {
          vector2.Y += 18f * SpriteText.FontPixelZoom;
          accumulatedHorizontalSpaceBetweenCharacters = 0;
          vector2.X = 0.0f;
        }
        vector2.X += (float) (8.0 * (double) SpriteText.FontPixelZoom + (double) accumulatedHorizontalSpaceBetweenCharacters + (double) SpriteText.getWidthOffsetForChar(s[index]) * (double) SpriteText.FontPixelZoom);
        if (index > 0)
          vector2.X += (float) SpriteText.getWidthOffsetForChar(s[index - 1]) * SpriteText.FontPixelZoom;
        accumulatedHorizontalSpaceBetweenCharacters = (int) (0.0 * (double) SpriteText.FontPixelZoom);
      }
    }
    return (int) ((double) vector2.Y + 16.0 * (double) SpriteText.FontPixelZoom);
  }

  public static Color color_Default
  {
    get
    {
      return !LocalizedContentManager.CurrentLanguageLatin && (LocalizedContentManager.CurrentLanguageCode != LocalizedContentManager.LanguageCode.ru || Game1.options.useAlternateFont) ? new Color(86, 22, 12) : Color.White;
    }
  }

  public static Color color_Black { get; } = Color.Black;

  public static Color color_Blue { get; } = Color.SkyBlue;

  public static Color color_Red { get; } = Color.Red;

  public static Color color_Purple { get; } = new Color(110, 43, (int) byte.MaxValue);

  public static Color color_White { get; } = Color.White;

  public static Color color_Orange { get; } = Color.OrangeRed;

  public static Color color_Green { get; } = Color.LimeGreen;

  public static Color color_Cyan { get; } = Color.Cyan;

  public static Color color_Gray { get; } = new Color(60, 60, 60);

  public static Color color_JojaBlue { get; } = new Color(52, 50, 122);

  public static Color getColorFromIndex(int index)
  {
    switch (index)
    {
      case -1:
        return SpriteText.color_Default;
      case 1:
        return SpriteText.color_Blue;
      case 2:
        return SpriteText.color_Red;
      case 3:
        return SpriteText.color_Purple;
      case 4:
        return SpriteText.color_White;
      case 5:
        return SpriteText.color_Orange;
      case 6:
        return SpriteText.color_Green;
      case 7:
        return SpriteText.color_Cyan;
      case 8:
        return SpriteText.color_Gray;
      case 9:
        return SpriteText.color_JojaBlue;
      default:
        return Color.Black;
    }
  }

  public static string getSubstringBeyondHeight(string s, int width, int height)
  {
    Vector2 vector2 = new Vector2();
    int accumulatedHorizontalSpaceBetweenCharacters = 0;
    s = s.Replace(Environment.NewLine, "");
    SpriteText.setUpCharacterMap();
    if (SpriteText.isUsingNonSpriteSheetFont())
    {
      for (int index = 0; index < s.Length; ++index)
      {
        if (s[index] == '^')
        {
          vector2.Y += (float) (SpriteText.FontFile.Common.LineHeight + 2) * SpriteText.FontPixelZoom;
          vector2.X = 0.0f;
          accumulatedHorizontalSpaceBetweenCharacters = 0;
        }
        else
        {
          FontChar fontChar;
          if (SpriteText.characterMap.TryGetValue(s[index], out fontChar))
          {
            if (index > 0)
              vector2.X += (float) fontChar.XAdvance * SpriteText.FontPixelZoom;
            if (SpriteText.positionOfNextSpace(s, index, (int) vector2.X, accumulatedHorizontalSpaceBetweenCharacters) >= width)
            {
              vector2.Y += (float) (SpriteText.FontFile.Common.LineHeight + 2) * SpriteText.FontPixelZoom;
              accumulatedHorizontalSpaceBetweenCharacters = 0;
              vector2.X = 0.0f;
            }
          }
          if ((double) vector2.Y >= (double) height - (double) SpriteText.FontFile.Common.LineHeight * (double) SpriteText.FontPixelZoom * 2.0)
            return s.Substring(SpriteText.getLastSpace(s, index));
        }
      }
      return "";
    }
    for (int index = 0; index < s.Length; ++index)
    {
      if (s[index] == '^')
      {
        vector2.Y += 18f * SpriteText.FontPixelZoom;
        vector2.X = 0.0f;
        accumulatedHorizontalSpaceBetweenCharacters = 0;
      }
      else
      {
        if (index > 0)
          vector2.X += (float) (8.0 * (double) SpriteText.FontPixelZoom + (double) accumulatedHorizontalSpaceBetweenCharacters + (double) (SpriteText.getWidthOffsetForChar(s[index]) + SpriteText.getWidthOffsetForChar(s[index - 1])) * (double) SpriteText.FontPixelZoom);
        accumulatedHorizontalSpaceBetweenCharacters = (int) (0.0 * (double) SpriteText.FontPixelZoom);
        if (SpriteText.positionOfNextSpace(s, index, (int) vector2.X, accumulatedHorizontalSpaceBetweenCharacters) >= width)
        {
          vector2.Y += 18f * SpriteText.FontPixelZoom;
          accumulatedHorizontalSpaceBetweenCharacters = 0;
          vector2.X = 0.0f;
        }
        if ((double) vector2.Y >= (double) height - 16.0 * (double) SpriteText.FontPixelZoom * 2.0)
          return s.Substring(SpriteText.getLastSpace(s, index));
      }
    }
    return "";
  }

  public static int getIndexOfSubstringBeyondHeight(string s, int width, int height)
  {
    Vector2 vector2 = new Vector2();
    int accumulatedHorizontalSpaceBetweenCharacters = 0;
    s = s.Replace(Environment.NewLine, "");
    SpriteText.setUpCharacterMap();
    if (!LocalizedContentManager.CurrentLanguageLatin)
    {
      for (int index = 0; index < s.Length; ++index)
      {
        if (s[index] == '^')
        {
          vector2.Y += (float) (SpriteText.FontFile.Common.LineHeight + 2) * SpriteText.FontPixelZoom;
          vector2.X = 0.0f;
          accumulatedHorizontalSpaceBetweenCharacters = 0;
        }
        else
        {
          FontChar fontChar;
          if (SpriteText.characterMap.TryGetValue(s[index], out fontChar))
          {
            if (index > 0)
              vector2.X += (float) fontChar.XAdvance * SpriteText.FontPixelZoom;
            if (SpriteText.positionOfNextSpace(s, index, (int) vector2.X, accumulatedHorizontalSpaceBetweenCharacters) >= width)
            {
              vector2.Y += (float) (SpriteText.FontFile.Common.LineHeight + 2) * SpriteText.FontPixelZoom;
              accumulatedHorizontalSpaceBetweenCharacters = 0;
              vector2.X = 0.0f;
            }
          }
          if ((double) vector2.Y >= (double) height - (double) SpriteText.FontFile.Common.LineHeight * (double) SpriteText.FontPixelZoom * 2.0)
            return index - 1;
        }
      }
      return s.Length - 1;
    }
    for (int index = 0; index < s.Length; ++index)
    {
      if (s[index] == '^')
      {
        vector2.Y += 18f * SpriteText.FontPixelZoom;
        vector2.X = 0.0f;
        accumulatedHorizontalSpaceBetweenCharacters = 0;
      }
      else
      {
        if (index > 0)
          vector2.X += (float) (8.0 * (double) SpriteText.FontPixelZoom + (double) accumulatedHorizontalSpaceBetweenCharacters + (double) (SpriteText.getWidthOffsetForChar(s[index]) + SpriteText.getWidthOffsetForChar(s[index - 1])) * (double) SpriteText.FontPixelZoom);
        accumulatedHorizontalSpaceBetweenCharacters = (int) (0.0 * (double) SpriteText.FontPixelZoom);
        if (SpriteText.positionOfNextSpace(s, index, (int) vector2.X, accumulatedHorizontalSpaceBetweenCharacters) >= width)
        {
          vector2.Y += 18f * SpriteText.FontPixelZoom;
          accumulatedHorizontalSpaceBetweenCharacters = 0;
          vector2.X = 0.0f;
        }
        if ((double) vector2.Y >= (double) height - 16.0 * (double) SpriteText.FontPixelZoom)
          return index - 1;
      }
    }
    return s.Length - 1;
  }

  public static List<string> getStringBrokenIntoSectionsOfHeight(string s, int width, int height)
  {
    List<string> source = new List<string>();
    for (; s.Length > 0; s = s.Substring(source.Last<string>().Length))
    {
      string thisHeightCutoff = SpriteText.getStringPreviousToThisHeightCutoff(s, width, height);
      if (thisHeightCutoff.Length > 0)
        source.Add(thisHeightCutoff);
      else
        break;
    }
    return source;
  }

  public static string getStringPreviousToThisHeightCutoff(string s, int width, int height)
  {
    return s.Substring(0, SpriteText.getIndexOfSubstringBeyondHeight(s, width, height) + 1);
  }

  private static int getLastSpace(string s, int startIndex)
  {
    if (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ja || LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.zh || LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.th)
      return startIndex;
    for (int index = startIndex; index >= 0; --index)
    {
      if (s[index] == ' ')
        return index;
    }
    return startIndex;
  }

  public static int getWidthOffsetForChar(char c)
  {
    switch (c)
    {
      case '!':
      case 'j':
      case 'l':
      case '¡':
        return -1;
      case '$':
        return 1;
      case ',':
      case '.':
        return -2;
      case '^':
        return -8;
      case 'i':
      case 'ì':
      case 'í':
      case 'î':
      case 'ï':
      case 'ı':
        return -1;
      case 'ş':
        return -1;
      default:
        return 0;
    }
  }

  public static void drawStringWithScrollCenteredAt(
    SpriteBatch b,
    string s,
    int x,
    int y,
    int width,
    float alpha = 1f,
    Color? color = null,
    int scrollType = 0,
    float layerDepth = 0.88f,
    bool junimoText = false)
  {
    SpriteText.drawString(b, s, x - width / 2, y, width: width, alpha: alpha, layerDepth: layerDepth, junimoText: junimoText, drawBGScroll: scrollType, color: color, scroll_text_alignment: SpriteText.ScrollTextAlignment.Center);
  }

  public static void drawSmallTextBubble(
    SpriteBatch b,
    string s,
    Vector2 positionOfBottomCenter,
    int maxWidth = -1,
    float layerDepth = -1f,
    bool drawPointerOnTop = false)
  {
    if (maxWidth != -1)
      s = Game1.parseText(s, Game1.smallFont, maxWidth - 16 /*0x10*/);
    s = s.Trim();
    Vector2 vector2 = Game1.smallFont.MeasureString(s);
    IClickableMenu.drawTextureBox(b, Game1.mouseCursors_1_6, new Rectangle(241, 503, 9, 9), (int) ((double) positionOfBottomCenter.X - (double) vector2.X / 2.0 - 4.0), (int) ((double) positionOfBottomCenter.Y - (double) vector2.Y), (int) vector2.X + 16 /*0x10*/, (int) vector2.Y + 12, Color.White, 4f, false, layerDepth);
    if (drawPointerOnTop)
      b.Draw(Game1.mouseCursors_1_6, positionOfBottomCenter + new Vector2(-4f, -3f) * 4f + new Vector2(vector2.X / 2f, -vector2.Y), new Rectangle?(new Rectangle(251, 506, 5, 5)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.FlipVertically, layerDepth + 1E-05f);
    else
      b.Draw(Game1.mouseCursors_1_6, positionOfBottomCenter + new Vector2(-2.5f, 1f) * 4f, new Rectangle?(new Rectangle(251, 506, 5, 5)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth + 1E-05f);
    Utility.drawTextWithShadow(b, s, Game1.smallFont, positionOfBottomCenter - vector2 + new Vector2((float) (4.0 + (double) vector2.X / 2.0), 8f), Game1.textColor, layerDepth: layerDepth + 2E-05f, shadowIntensity: 0.5f);
  }

  public static void drawStringWithScrollCenteredAt(
    SpriteBatch b,
    string s,
    int x,
    int y,
    string placeHolderWidthText = "",
    float alpha = 1f,
    Color? color = null,
    int scrollType = 0,
    float layerDepth = 0.88f,
    bool junimoText = false)
  {
    SpriteText.drawString(b, s, x - SpriteText.getWidthOfString(placeHolderWidthText.Length > 0 ? placeHolderWidthText : s) / 2, y, alpha: alpha, layerDepth: layerDepth, junimoText: junimoText, drawBGScroll: scrollType, placeHolderScrollWidthText: placeHolderWidthText, color: color, scroll_text_alignment: SpriteText.ScrollTextAlignment.Center);
  }

  public static void drawStringWithScrollBackground(
    SpriteBatch b,
    string s,
    int x,
    int y,
    string placeHolderWidthText = "",
    float alpha = 1f,
    Color? color = null,
    SpriteText.ScrollTextAlignment scroll_text_alignment = SpriteText.ScrollTextAlignment.Left)
  {
    SpriteText.drawString(b, s, x, y, alpha: alpha, drawBGScroll: 0, placeHolderScrollWidthText: placeHolderWidthText, color: color, scroll_text_alignment: scroll_text_alignment);
  }

  private static FontFile loadFont(string assetName)
  {
    return FontLoader.Parse(Game1.content.Load<XmlSource>(assetName).Source);
  }

  private static void setUpCharacterMap()
  {
    if (LocalizedContentManager.CurrentLanguageLatin || SpriteText.characterMap != null)
      return;
    LocalizedContentManager.OnLanguageChange += new LocalizedContentManager.LanguageChangedHandler(SpriteText.OnLanguageChange);
    SpriteText.LoadFontData(LocalizedContentManager.CurrentLanguageCode);
  }

  public static void drawString(
    SpriteBatch b,
    string s,
    int x,
    int y,
    int characterPosition = 999999,
    int width = -1,
    int height = 999999,
    float alpha = 1f,
    float layerDepth = 0.88f,
    bool junimoText = false,
    int drawBGScroll = -1,
    string placeHolderScrollWidthText = "",
    Color? color = null,
    SpriteText.ScrollTextAlignment scroll_text_alignment = SpriteText.ScrollTextAlignment.Left)
  {
    SpriteText.setUpCharacterMap();
    bool hasValue = color.HasValue;
    color = new Color?(color ?? SpriteText.color_Default);
    bool flag1 = width != -1;
    Viewport viewport;
    if (!flag1)
    {
      viewport = Game1.graphics.GraphicsDevice.Viewport;
      width = viewport.Width - x;
      if (drawBGScroll == 1)
        width = SpriteText.getWidthOfString(s) * 2;
    }
    if ((double) SpriteText.FontPixelZoom < 4.0 && LocalizedContentManager.CurrentLanguageCode != LocalizedContentManager.LanguageCode.ko && LocalizedContentManager.CurrentLanguageCode != LocalizedContentManager.LanguageCode.zh)
      y += (int) ((4.0 - (double) SpriteText.FontPixelZoom) * 4.0);
    Vector2 vector2_1 = new Vector2((float) x, (float) y);
    int accumulatedHorizontalSpaceBetweenCharacters = 0;
    if (drawBGScroll != 1)
    {
      double num1 = (double) vector2_1.X + (double) width;
      viewport = Game1.graphics.GraphicsDevice.Viewport;
      double num2 = (double) (viewport.Width - 4);
      if (num1 > num2)
      {
        ref Vector2 local = ref vector2_1;
        viewport = Game1.graphics.GraphicsDevice.Viewport;
        double num3 = (double) (viewport.Width - width - 4);
        local.X = (float) num3;
      }
      if ((double) vector2_1.X < 0.0)
        vector2_1.X = 0.0f;
    }
    switch (drawBGScroll)
    {
      case 0:
      case 2:
      case 3:
        int x1 = SpriteText.getWidthOfString(placeHolderScrollWidthText.Length > 0 ? placeHolderScrollWidthText : s);
        if (flag1)
          x1 = width;
        switch (drawBGScroll)
        {
          case 0:
            b.Draw(Game1.mouseCursors, vector2_1 + new Vector2(-12f, -3f) * 4f, new Rectangle?(new Rectangle(325, 318, 12, 18)), Color.White * alpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth - 1f / 1000f);
            b.Draw(Game1.mouseCursors, vector2_1 + new Vector2(0.0f, -3f) * 4f, new Rectangle?(new Rectangle(337, 318, 1, 18)), Color.White * alpha, 0.0f, Vector2.Zero, new Vector2((float) x1, 4f), SpriteEffects.None, layerDepth - 1f / 1000f);
            b.Draw(Game1.mouseCursors, vector2_1 + new Vector2((float) x1, -12f), new Rectangle?(new Rectangle(338, 318, 12, 18)), Color.White * alpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth - 1f / 1000f);
            break;
          case 2:
            b.Draw(Game1.mouseCursors, vector2_1 + new Vector2(-3f, -3f) * 4f, new Rectangle?(new Rectangle(327, 281, 3, 17)), Color.White * alpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth - 1f / 1000f);
            b.Draw(Game1.mouseCursors, vector2_1 + new Vector2(0.0f, -3f) * 4f, new Rectangle?(new Rectangle(330, 281, 1, 17)), Color.White * alpha, 0.0f, Vector2.Zero, new Vector2((float) (x1 + 4), 4f), SpriteEffects.None, layerDepth - 1f / 1000f);
            b.Draw(Game1.mouseCursors, vector2_1 + new Vector2((float) (x1 + 4), -12f), new Rectangle?(new Rectangle(333, 281, 3, 17)), Color.White * alpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth - 1f / 1000f);
            break;
          case 3:
            b.Draw(Game1.mouseCursors_1_6, vector2_1 + new Vector2(-3f, -3f) * 4f, new Rectangle?(new Rectangle(86, 145, 3, 17)), Color.White * alpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth - 1f / 1000f);
            b.Draw(Game1.mouseCursors_1_6, vector2_1 + new Vector2(0.0f, -3f) * 4f, new Rectangle?(new Rectangle(89, 145, 1, 17)), Color.White * alpha, 0.0f, Vector2.Zero, new Vector2((float) (x1 + 4), 4f), SpriteEffects.None, layerDepth - 1f / 1000f);
            b.Draw(Game1.mouseCursors_1_6, vector2_1 + new Vector2((float) (x1 + 4), -12f), new Rectangle?(new Rectangle(92, 145, 3, 17)), Color.White * alpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth - 1f / 1000f);
            break;
        }
        switch (scroll_text_alignment)
        {
          case SpriteText.ScrollTextAlignment.Center:
            x += (x1 - SpriteText.getWidthOfString(s)) / 2;
            vector2_1.X = (float) x;
            break;
          case SpriteText.ScrollTextAlignment.Right:
            x += x1 - SpriteText.getWidthOfString(s);
            vector2_1.X = (float) x;
            break;
        }
        vector2_1.Y += (float) ((4.0 - (double) SpriteText.FontPixelZoom) * 4.0);
        break;
      case 1:
        int widthOfString = SpriteText.getWidthOfString(placeHolderScrollWidthText.Length > 0 ? placeHolderScrollWidthText : s);
        Vector2 vector2_2 = vector2_1;
        if (Game1.currentLocation?.map?.Layers[0] != null)
        {
          int num4 = -Game1.viewport.X + 28;
          int num5 = -Game1.viewport.X + Game1.currentLocation.map.Layers[0].LayerWidth * 64 /*0x40*/ - 28;
          if ((double) vector2_1.X < (double) num4)
            vector2_1.X = (float) num4;
          if ((double) vector2_1.X + (double) widthOfString > (double) num5)
            vector2_1.X = (float) (num5 - widthOfString);
          vector2_2.X += (float) (widthOfString / 2);
          if ((double) vector2_2.X < (double) vector2_1.X)
            vector2_1.X += vector2_2.X - vector2_1.X;
          if ((double) vector2_2.X > (double) vector2_1.X + (double) widthOfString - 24.0)
            vector2_1.X += vector2_2.X - (float) ((double) vector2_1.X + (double) widthOfString - 24.0);
          vector2_2.X = Utility.Clamp(vector2_2.X, vector2_1.X, (float) ((double) vector2_1.X + (double) widthOfString - 24.0));
        }
        b.Draw(Game1.mouseCursors, vector2_1 + new Vector2(-7f, -3f) * 4f, new Rectangle?(new Rectangle(324, 299, 7, 17)), Color.White * alpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth - 1f / 1000f);
        b.Draw(Game1.mouseCursors, vector2_1 + new Vector2(0.0f, -3f) * 4f, new Rectangle?(new Rectangle(331, 299, 1, 17)), Color.White * alpha, 0.0f, Vector2.Zero, new Vector2((float) SpriteText.getWidthOfString(placeHolderScrollWidthText.Length > 0 ? placeHolderScrollWidthText : s), 4f), SpriteEffects.None, layerDepth - 1f / 1000f);
        b.Draw(Game1.mouseCursors, vector2_1 + new Vector2((float) widthOfString, -12f), new Rectangle?(new Rectangle(332, 299, 7, 17)), Color.White * alpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth - 1f / 1000f);
        b.Draw(Game1.mouseCursors, vector2_2 + new Vector2(0.0f, 52f), new Rectangle?(new Rectangle(341, 308, 6, 5)), Color.White * alpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth - 0.0001f);
        x = (int) vector2_1.X;
        if (placeHolderScrollWidthText.Length > 0)
        {
          x += SpriteText.getWidthOfString(placeHolderScrollWidthText) / 2 - SpriteText.getWidthOfString(s) / 2;
          vector2_1.X = (float) x;
        }
        vector2_1.Y += (float) ((4.0 - (double) SpriteText.FontPixelZoom) * 4.0);
        break;
    }
    if (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ko)
      vector2_1.Y -= 8f;
    if (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.zh)
    {
      if (drawBGScroll != -1)
      {
        float num = 3.5f;
        if (Game1.options.useChineseSmoothFont)
        {
          vector2_1.Y -= 2f;
          num = 3.8f;
        }
        else
          vector2_1.Y += 4f;
        vector2_1.Y -= (float) (((double) SpriteText.FontPixelZoom - 0.75) * 4.0) * num;
      }
      else
        vector2_1.Y += 4f;
    }
    s = s.Replace(Environment.NewLine, "");
    if (!junimoText)
    {
      switch (LocalizedContentManager.CurrentLanguageCode)
      {
        case LocalizedContentManager.LanguageCode.ja:
        case LocalizedContentManager.LanguageCode.zh:
        case LocalizedContentManager.LanguageCode.th:
          vector2_1.Y -= (float) ((4.0 - (double) SpriteText.FontPixelZoom) * 4.0);
          break;
        case LocalizedContentManager.LanguageCode.mod:
          if (!LocalizedContentManager.CurrentModLanguage.FontApplyYOffset)
            break;
          goto case LocalizedContentManager.LanguageCode.ja;
      }
    }
    s = s.Replace('♡', '<');
    for (int index = 0; index < Math.Min(s.Length, characterPosition); ++index)
    {
      if (((LocalizedContentManager.CurrentLanguageLatin || LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ru && !Game1.options.useAlternateFont ? 1 : (SpriteText.IsSpecialCharacter(s[index]) ? 1 : 0)) | (junimoText ? 1 : 0)) != 0 || SpriteText.forceEnglishFont)
      {
        float fontPixelZoom = SpriteText.fontPixelZoom;
        if (SpriteText.IsSpecialCharacter(s[index]) | junimoText || SpriteText.forceEnglishFont)
          SpriteText.fontPixelZoom = 3f;
        if (s[index] == '^')
        {
          vector2_1.Y += 18f * SpriteText.FontPixelZoom;
          vector2_1.X = (float) x;
          accumulatedHorizontalSpaceBetweenCharacters = 0;
          SpriteText.fontPixelZoom = fontPixelZoom;
        }
        else
        {
          accumulatedHorizontalSpaceBetweenCharacters = (int) (0.0 * (double) SpriteText.FontPixelZoom);
          bool flag2 = char.IsUpper(s[index]) || s[index] == 'ß';
          Vector2 vector2_3 = new Vector2(0.0f, (float) ((!junimoText & flag2 ? -3 : 0) - 1));
          if (s[index] == 'Ç')
            vector2_3.Y += 2f;
          if (SpriteText.positionOfNextSpace(s, index, (int) vector2_1.X - x, accumulatedHorizontalSpaceBetweenCharacters) >= width)
          {
            vector2_1.Y += 18f * SpriteText.FontPixelZoom;
            accumulatedHorizontalSpaceBetweenCharacters = 0;
            vector2_1.X = (float) x;
            if (s[index] == ' ')
            {
              SpriteText.fontPixelZoom = fontPixelZoom;
              continue;
            }
          }
          Rectangle sourceRectForChar = SpriteText.getSourceRectForChar(s[index], junimoText);
          b.Draw(hasValue ? SpriteText.coloredTexture : SpriteText.spriteTexture, vector2_1 + vector2_3 * SpriteText.FontPixelZoom, new Rectangle?(sourceRectForChar), (SpriteText.IsSpecialCharacter(s[index]) | junimoText ? Color.White : color.Value) * alpha, 0.0f, Vector2.Zero, SpriteText.FontPixelZoom, SpriteEffects.None, layerDepth);
          if (index < s.Length - 1)
            vector2_1.X += (float) (8.0 * (double) SpriteText.FontPixelZoom + (double) accumulatedHorizontalSpaceBetweenCharacters + (double) SpriteText.getWidthOffsetForChar(s[index + 1]) * (double) SpriteText.FontPixelZoom);
          if (s[index] != '^')
            vector2_1.X += (float) SpriteText.getWidthOffsetForChar(s[index]) * SpriteText.FontPixelZoom;
          SpriteText.fontPixelZoom = fontPixelZoom;
        }
      }
      else if (s[index] == '^')
      {
        vector2_1.Y += (float) (SpriteText.FontFile.Common.LineHeight + 2) * SpriteText.FontPixelZoom;
        vector2_1.X = (float) x;
        accumulatedHorizontalSpaceBetweenCharacters = 0;
      }
      else
      {
        if (index > 0 && SpriteText.IsSpecialCharacter(s[index - 1]))
          vector2_1.X += 24f;
        FontChar fontChar;
        if (SpriteText.characterMap.TryGetValue(s[index], out fontChar))
        {
          Rectangle rectangle = new Rectangle(fontChar.X, fontChar.Y, fontChar.Width, fontChar.Height);
          Texture2D fontPage = SpriteText.fontPages[fontChar.Page];
          if (SpriteText.positionOfNextSpace(s, index, (int) vector2_1.X, accumulatedHorizontalSpaceBetweenCharacters) >= x + width - 4)
          {
            vector2_1.Y += (float) (SpriteText.FontFile.Common.LineHeight + 2) * SpriteText.FontPixelZoom;
            accumulatedHorizontalSpaceBetweenCharacters = 0;
            vector2_1.X = (float) x;
          }
          Vector2 position = new Vector2(vector2_1.X + (float) fontChar.XOffset * SpriteText.FontPixelZoom, vector2_1.Y + (float) fontChar.YOffset * SpriteText.FontPixelZoom);
          if (drawBGScroll != -1 && LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ko)
            position.Y -= 8f;
          if (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ru)
          {
            Vector2 vector2_4 = new Vector2(-1f, 1f) * SpriteText.FontPixelZoom;
            b.Draw(fontPage, position + vector2_4, new Rectangle?(rectangle), color.Value * alpha * SpriteText.shadowAlpha, 0.0f, Vector2.Zero, SpriteText.FontPixelZoom, SpriteEffects.None, layerDepth);
            b.Draw(fontPage, position + new Vector2(0.0f, vector2_4.Y), new Rectangle?(rectangle), color.Value * alpha * SpriteText.shadowAlpha, 0.0f, Vector2.Zero, SpriteText.FontPixelZoom, SpriteEffects.None, layerDepth);
            b.Draw(fontPage, position + new Vector2(vector2_4.X, 0.0f), new Rectangle?(rectangle), color.Value * alpha * SpriteText.shadowAlpha, 0.0f, Vector2.Zero, SpriteText.FontPixelZoom, SpriteEffects.None, layerDepth);
          }
          b.Draw(fontPage, position, new Rectangle?(rectangle), color.Value * alpha, 0.0f, Vector2.Zero, SpriteText.FontPixelZoom, SpriteEffects.None, layerDepth);
          vector2_1.X += (float) fontChar.XAdvance * SpriteText.FontPixelZoom;
        }
      }
    }
  }

  private static bool IsSpecialCharacter(char c)
  {
    return c.Equals('<') || c.Equals('=') || c.Equals('>') || c.Equals('@') || c.Equals('$') || c.Equals('`') || c.Equals('+');
  }

  private static void OnLanguageChange(LocalizedContentManager.LanguageCode code)
  {
    SpriteText.LoadFontData(code);
  }

  public static void LoadFontData(LocalizedContentManager.LanguageCode code)
  {
    if (SpriteText.characterMap != null)
      SpriteText.characterMap.Clear();
    else
      SpriteText.characterMap = new Dictionary<char, FontChar>();
    if (SpriteText.fontPages != null)
      SpriteText.fontPages.Clear();
    else
      SpriteText.fontPages = new List<Texture2D>();
    string str = "Fonts\\";
    switch (code)
    {
      case LocalizedContentManager.LanguageCode.ja:
        SpriteText.FontFile = SpriteText.loadFont(str + "Japanese");
        SpriteText.fontPixelZoom = 1.75f;
        break;
      case LocalizedContentManager.LanguageCode.ru:
        SpriteText.FontFile = SpriteText.loadFont(str + "Russian");
        SpriteText.fontPixelZoom = 3f;
        break;
      case LocalizedContentManager.LanguageCode.zh:
        if (Game1.options.useChineseSmoothFont)
        {
          str += "Chinese_round\\";
          SpriteText.fontPixelZoom = 1f;
        }
        else
          SpriteText.fontPixelZoom = 1.5f;
        SpriteText.FontFile = SpriteText.loadFont(str + "Chinese");
        break;
      case LocalizedContentManager.LanguageCode.th:
        SpriteText.FontFile = SpriteText.loadFont(str + "Thai");
        SpriteText.fontPixelZoom = 1.5f;
        break;
      case LocalizedContentManager.LanguageCode.ko:
        SpriteText.FontFile = SpriteText.loadFont(str + "Korean");
        SpriteText.fontPixelZoom = 1.5f;
        break;
      case LocalizedContentManager.LanguageCode.mod:
        SpriteText.FontFile = SpriteText.loadFont(LocalizedContentManager.CurrentModLanguage.FontFile);
        SpriteText.fontPixelZoom = LocalizedContentManager.CurrentModLanguage.FontPixelZoom;
        break;
      default:
        SpriteText.FontFile = (FontFile) null;
        SpriteText.fontPixelZoom = 3f;
        break;
    }
    if (SpriteText.FontFile == null)
      return;
    foreach (FontChar fontChar in SpriteText.FontFile.Chars)
    {
      char id = (char) fontChar.ID;
      SpriteText.characterMap.Add(id, fontChar);
    }
    foreach (FontPage page in SpriteText.FontFile.Pages)
      SpriteText.fontPages.Add(Game1.content.Load<Texture2D>(str + page.File));
  }

  public static int positionOfNextSpace(
    string s,
    int index,
    int currentXPosition,
    int accumulatedHorizontalSpaceBetweenCharacters)
  {
    SpriteText.setUpCharacterMap();
    switch (LocalizedContentManager.CurrentLanguageCode)
    {
      case LocalizedContentManager.LanguageCode.ja:
      case LocalizedContentManager.LanguageCode.zh:
      case LocalizedContentManager.LanguageCode.th:
        float num = (float) currentXPosition;
        foreach (char key in Game1.asianSpacingRegex.Match(s, index).Value)
        {
          FontChar fontChar;
          if (SpriteText.characterMap.TryGetValue(key, out fontChar))
            num += (float) fontChar.XAdvance * SpriteText.FontPixelZoom;
        }
        return (int) num;
      default:
        for (int index1 = index; index1 < s.Length; ++index1)
        {
          if (SpriteText.isUsingNonSpriteSheetFont())
          {
            if (s[index1] == ' ' || s[index1] == '^')
              return currentXPosition;
            FontChar fontChar;
            if (SpriteText.characterMap.TryGetValue(s[index1], out fontChar))
              currentXPosition += (int) ((double) fontChar.XAdvance * (double) SpriteText.FontPixelZoom);
            else
              currentXPosition += (int) ((double) SpriteText.FontFile.Common.LineHeight * (double) SpriteText.FontPixelZoom);
          }
          else
          {
            if (s[index1] == ' ' || s[index1] == '^')
              return currentXPosition;
            currentXPosition += (int) (8.0 * (double) SpriteText.FontPixelZoom + (double) accumulatedHorizontalSpaceBetweenCharacters + (double) (SpriteText.getWidthOffsetForChar(s[index1]) + SpriteText.getWidthOffsetForChar(s[Math.Max(0, index1 - 1)])) * (double) SpriteText.FontPixelZoom);
            accumulatedHorizontalSpaceBetweenCharacters = (int) (0.0 * (double) SpriteText.FontPixelZoom);
          }
        }
        return currentXPosition;
    }
  }

  private static bool isUsingNonSpriteSheetFont()
  {
    if (LocalizedContentManager.CurrentLanguageLatin)
      return false;
    return LocalizedContentManager.CurrentLanguageCode != LocalizedContentManager.LanguageCode.ru || Game1.options.useAlternateFont;
  }

  private static Rectangle getSourceRectForChar(char c, bool junimoText)
  {
    int num = (int) c - 32 /*0x20*/;
    switch (c)
    {
      case 'Ą':
        num = 576;
        break;
      case 'ą':
        num = 578;
        break;
      case 'Ć':
        num = 579;
        break;
      case 'ć':
        num = 580;
        break;
      case 'Ę':
        num = 581;
        break;
      case 'ę':
        num = 582;
        break;
      case 'Ğ':
        num = 102;
        break;
      case 'ğ':
        num = 103;
        break;
      case 'İ':
        num = 98;
        break;
      case 'ı':
        num = 99;
        break;
      case 'Ł':
        num = 583;
        break;
      case 'ł':
        num = 584;
        break;
      case 'Ń':
        num = 585;
        break;
      case 'ń':
        num = 586;
        break;
      case 'Ő':
        num = 105;
        break;
      case 'ő':
        num = 106;
        break;
      case 'Œ':
        num = 96 /*0x60*/;
        break;
      case 'œ':
        num = 97;
        break;
      case 'Ś':
        num = 574;
        break;
      case 'ś':
        num = 575;
        break;
      case 'Ş':
        num = 100;
        break;
      case 'ş':
        num = 101;
        break;
      case 'Ű':
        num = 107;
        break;
      case 'ű':
        num = 108;
        break;
      case 'Ź':
        num = 587;
        break;
      case 'ź':
        num = 588;
        break;
      case 'Ż':
        num = 589;
        break;
      case 'ż':
        num = 590;
        break;
      case 'Ё':
        num = 512 /*0x0200*/;
        break;
      case 'Є':
        num = 514;
        break;
      case 'І':
        num = 515;
        break;
      case 'Ї':
        num = 516;
        break;
      case 'Ў':
        num = 517;
        break;
      case 'ё':
        num = 560;
        break;
      case 'є':
        num = 562;
        break;
      case 'і':
        num = 563;
        break;
      case 'ї':
        num = 564;
        break;
      case 'ў':
        num = 565;
        break;
      case 'Ґ':
        num = 513;
        break;
      case 'ґ':
        num = 561;
        break;
      case '–':
        num = 464;
        break;
      case '—':
        num = 465;
        break;
      case '’':
        num = 104;
        break;
      case '№':
        num = 466;
        break;
      default:
        if (num >= 1008 && num < 1040)
        {
          num -= 528;
          break;
        }
        if (num >= 1040 && num < 1072)
        {
          num -= 512 /*0x0200*/;
          break;
        }
        break;
    }
    return new Rectangle(num * 8 % SpriteText.spriteTexture.Width, num * 8 / SpriteText.spriteTexture.Width * 16 /*0x10*/ + (junimoText ? 224 /*0xE0*/ : 0), 8, 16 /*0x10*/);
  }

  public enum ScrollTextAlignment
  {
    Left,
    Center,
    Right,
  }
}
