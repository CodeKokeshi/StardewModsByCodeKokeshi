// Decompiled with JetBrains decompiler
// Type: StardewValley.CharacterEventArgs
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using System;

#nullable disable
namespace StardewValley;

public class CharacterEventArgs : EventArgs
{
  private readonly char character;
  private readonly int lParam;

  public CharacterEventArgs(char character, int lParam)
  {
    this.character = character;
    this.lParam = lParam;
  }

  public char Character => this.character;

  public int Param => this.lParam;

  public int RepeatCount => this.lParam & (int) ushort.MaxValue;

  public bool PreviousState => (this.lParam & 1073741824 /*0x40000000*/) > 0;

  public bool TransitionState => (this.lParam & int.MinValue) > 0;
}
