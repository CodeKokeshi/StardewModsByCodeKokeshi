// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.TextCreditsBlock
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.BellsAndWhistles;
using System;

#nullable disable
namespace StardewValley.Menus;

internal class TextCreditsBlock : ICreditsBlock
{
  private string text;
  private Color color;
  private bool renderNameInEnglish;

  public TextCreditsBlock(string rawtext)
  {
    string[] strArray = rawtext.Split(']');
    if (strArray.Length > 1)
    {
      this.text = strArray[1];
      this.color = SpriteText.getColorFromIndex(Convert.ToInt32(strArray[0].Substring(1)));
    }
    else
    {
      this.text = strArray[0];
      this.color = SpriteText.color_White;
    }
    if (!SpriteText.IsMissingCharacters(rawtext))
      return;
    this.renderNameInEnglish = true;
  }

  public override void draw(int topLeftX, int topLeftY, int widthToOccupy, SpriteBatch b)
  {
    if (this.renderNameInEnglish)
    {
      int num1 = this.text.IndexOf('(');
      if (num1 != -1 && num1 > 0)
      {
        string s1 = this.text.Substring(0, num1);
        string s2 = this.text.Substring(num1);
        SpriteText.forceEnglishFont = true;
        int num2 = (int) ((double) SpriteText.getWidthOfString(s1) / (double) SpriteText.FontPixelZoom * 3.0);
        SpriteText.drawString(b, s1, topLeftX, topLeftY, width: widthToOccupy, height: 99999, color: new Color?(this.color));
        SpriteText.forceEnglishFont = false;
        SpriteText.drawString(b, s2, topLeftX + num2, topLeftY, height: 99999, color: new Color?(this.color));
      }
      else
      {
        SpriteText.forceEnglishFont = true;
        SpriteText.drawString(b, this.text, topLeftX, topLeftY, width: widthToOccupy, height: 99999, color: new Color?(this.color));
        SpriteText.forceEnglishFont = false;
      }
    }
    else
      SpriteText.drawString(b, this.text, topLeftX, topLeftY, width: widthToOccupy, height: 99999, color: new Color?(this.color));
  }

  public override int getHeight(int maxWidth)
  {
    return !(this.text == "") ? SpriteText.getHeightOfString(this.text, maxWidth) : 64 /*0x40*/;
  }
}
