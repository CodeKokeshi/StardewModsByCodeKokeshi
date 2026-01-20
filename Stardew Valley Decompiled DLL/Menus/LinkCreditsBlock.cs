// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.LinkCreditsBlock
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.BellsAndWhistles;
using System;
using System.Diagnostics;

#nullable disable
namespace StardewValley.Menus;

internal class LinkCreditsBlock : ICreditsBlock
{
  private string text;
  private string url;
  private bool currentlyHovered;

  public LinkCreditsBlock(string text, string url)
  {
    this.text = text;
    this.url = url;
  }

  public override void draw(int topLeftX, int topLeftY, int widthToOccupy, SpriteBatch b)
  {
    SpriteText.drawString(b, this.text, topLeftX, topLeftY, width: widthToOccupy, height: 99999, color: new Color?(this.currentlyHovered ? SpriteText.color_Green : SpriteText.color_Cyan));
    this.currentlyHovered = false;
  }

  public override int getHeight(int maxWidth)
  {
    return !(this.text == "") ? SpriteText.getHeightOfString(this.text, maxWidth) : 64 /*0x40*/;
  }

  public override void hovered() => this.currentlyHovered = true;

  private static void LaunchBrowser(string url)
  {
    Process.Start(new ProcessStartInfo(url)
    {
      UseShellExecute = true
    });
  }

  public override void clicked()
  {
    Game1.playSound("bigSelect");
    try
    {
      LinkCreditsBlock.LaunchBrowser(this.url);
    }
    catch (Exception ex)
    {
      Game1.log.Error("Could not open credit link.", ex);
    }
  }
}
