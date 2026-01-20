// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.ChatSnippet
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

#nullable disable
namespace StardewValley.Menus;

public class ChatSnippet
{
  public string message;
  public int emojiIndex = -1;
  public float myLength;

  public ChatSnippet(string message, LocalizedContentManager.LanguageCode language)
  {
    this.message = message;
    this.myLength = ChatBox.messageFont(language).MeasureString(message).X;
  }

  public ChatSnippet(int emojiIndex)
  {
    this.emojiIndex = emojiIndex;
    this.myLength = 40f;
  }
}
