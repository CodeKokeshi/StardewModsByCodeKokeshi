// Decompiled with JetBrains decompiler
// Type: StardewValley.SDKs.GogGalaxy.Internal.Base36
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using System;

#nullable disable
namespace StardewValley.SDKs.GogGalaxy.Internal;

public class Base36
{
  private const string Chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
  private const ulong Base = 36;

  public static string Encode(ulong value)
  {
    string str = "";
    if (value == 0UL)
      return "0";
    while (value != 0UL)
    {
      int index = (int) (value % 36UL);
      value /= 36UL;
      str = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"[index].ToString() + str;
    }
    return str;
  }

  public static ulong Decode(string value)
  {
    value = value.ToUpper();
    ulong num1 = 0;
    foreach (char ch in value)
    {
      ulong num2 = num1 * 36UL;
      int num3 = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ".IndexOf(ch);
      if (num3 == -1)
        throw new FormatException(value);
      num1 = num2 + (ulong) num3;
    }
    return num1;
  }
}
