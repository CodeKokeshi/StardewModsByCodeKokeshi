// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.FarmerBoxButton
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;

#nullable disable
namespace StardewValley.Menus;

internal class FarmerBoxButton(string name) : ClickableComponent(Rectangle.Empty, name)
{
  public bool Selected;
}
