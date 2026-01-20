// Decompiled with JetBrains decompiler
// Type: StardewValley.KeyEventArgs
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework.Input;
using System;

#nullable disable
namespace StardewValley;

public class KeyEventArgs : EventArgs
{
  private Keys keyCode;

  public KeyEventArgs(Keys keyCode) => this.keyCode = keyCode;

  public Keys KeyCode => this.keyCode;
}
