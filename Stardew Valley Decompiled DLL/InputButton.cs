// Decompiled with JetBrains decompiler
// Type: StardewValley.InputButton
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework.Input;

#nullable disable
namespace StardewValley;

public struct InputButton
{
  public Keys key;
  public bool mouseLeft;
  public bool mouseRight;

  public InputButton(Keys key)
  {
    this.key = key;
    this.mouseLeft = false;
    this.mouseRight = false;
  }

  public InputButton(bool mouseLeft)
  {
    this.key = Keys.None;
    this.mouseLeft = mouseLeft;
    this.mouseRight = !mouseLeft;
  }

  public override string ToString()
  {
    if (this.mouseLeft)
      return Game1.content.LoadString("Strings\\StringsFromCSFiles:Left-Click");
    if (this.mouseRight)
      return Game1.content.LoadString("Strings\\StringsFromCSFiles:Right-Click");
    switch (this.key)
    {
      case Keys.D0:
        return "0";
      case Keys.D1:
        return "1";
      case Keys.D2:
        return "2";
      case Keys.D3:
        return "3";
      case Keys.D4:
        return "4";
      case Keys.D5:
        return "5";
      case Keys.D6:
        return "6";
      case Keys.D7:
        return "7";
      case Keys.D8:
        return "8";
      case Keys.D9:
        return "9";
      default:
        string str = this.key.ToString().Replace("Oem", "");
        if (Game1.content.LoadString("Strings\\StringsFromCSFiles:" + this.key.ToString().Replace("Oem", "")) != "Strings\\StringsFromCSFiles:" + this.key.ToString().Replace("Oem", ""))
          str = Game1.content.LoadString("Strings\\StringsFromCSFiles:" + this.key.ToString().Replace("Oem", ""));
        return str;
    }
  }
}
