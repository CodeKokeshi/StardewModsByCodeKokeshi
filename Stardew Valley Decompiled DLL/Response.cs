// Decompiled with JetBrains decompiler
// Type: StardewValley.Response
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework.Input;

#nullable disable
namespace StardewValley;

public class Response
{
  public string responseKey;
  public string responseText;
  public Keys hotkey;

  public Response(string responseKey, string responseText)
  {
    this.responseKey = responseKey;
    this.responseText = responseText;
  }

  public Response SetHotKey(Keys key)
  {
    this.hotkey = key;
    return this;
  }
}
