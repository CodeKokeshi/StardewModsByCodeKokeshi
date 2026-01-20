// Decompiled with JetBrains decompiler
// Type: StardewValley.RainDrop
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;

#nullable disable
namespace StardewValley;

public struct RainDrop(int x, int y, int frame, int accumulator)
{
  public int frame = frame;
  public int accumulator = accumulator;
  public Vector2 position = new Vector2((float) x, (float) y);
}
