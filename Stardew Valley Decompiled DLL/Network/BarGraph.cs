// Decompiled with JetBrains decompiler
// Type: StardewValley.Network.BarGraph
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

#nullable disable
namespace StardewValley.Network;

public class BarGraph
{
  public static double DYNAMIC_SCALE_MAX = -1.0;
  public static double DYNAMIC_SCALE_AVG = -2.0;
  private Queue<double> elements;
  private int height;
  private int width;
  private int x;
  private int y;
  private double maxValue;
  private Color barColor;
  private int elementWidth;
  private Texture2D whiteTexture;

  public BarGraph(
    Queue<double> elements,
    int x,
    int y,
    int width,
    int height,
    int elementWidth,
    double maxValue,
    Color barColor,
    Texture2D whiteTexture)
  {
    this.elements = elements;
    this.width = width;
    this.height = height;
    this.x = x;
    this.y = y;
    this.maxValue = maxValue;
    this.barColor = barColor;
    this.elementWidth = elementWidth;
    this.whiteTexture = whiteTexture;
  }

  public void Draw(SpriteBatch sb)
  {
    double val2 = this.maxValue;
    if (val2 == BarGraph.DYNAMIC_SCALE_MAX)
    {
      foreach (double element in this.elements)
        val2 = Math.Max(element, val2);
    }
    else if (val2 == BarGraph.DYNAMIC_SCALE_AVG)
    {
      double num = 0.0;
      foreach (double element in this.elements)
        num += element;
      val2 = num / (double) Math.Max(1, this.elements.Count);
    }
    sb.Draw(this.whiteTexture, new Rectangle(this.x - 1, this.y, this.width, this.height), new Rectangle?(), Color.Black * 0.5f);
    int num1 = this.x + this.width - this.elementWidth * this.elements.Count;
    int num2 = 0;
    foreach (double element in this.elements)
    {
      int x = num1 + num2 * this.elementWidth;
      int y = this.y;
      int height = (int) (element / val2 * (double) this.height);
      sb.Draw(this.whiteTexture, new Rectangle(x, y + this.height - height, this.elementWidth, height), new Rectangle?(), this.barColor);
      ++num2;
    }
  }
}
