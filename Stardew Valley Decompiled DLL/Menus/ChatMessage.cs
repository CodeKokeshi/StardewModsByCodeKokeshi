// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.ChatMessage
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

#nullable disable
namespace StardewValley.Menus;

public class ChatMessage
{
  public List<ChatSnippet> message = new List<ChatSnippet>();
  public int timeLeftToDisplay;
  public int verticalSize;
  public float alpha = 1f;
  public Color color;
  public LocalizedContentManager.LanguageCode language;

  public void parseMessageForEmoji(string messagePlaintext)
  {
    if (messagePlaintext == null)
      return;
    StringBuilder sb = new StringBuilder();
    for (int index = 0; index < messagePlaintext.Length; ++index)
    {
      if (messagePlaintext[index] == '[')
      {
        if (sb.Length > 0)
          this.breakNewLines(sb);
        sb.Clear();
        int num1 = messagePlaintext.IndexOf(']', index);
        int num2 = -1;
        if (index + 1 < messagePlaintext.Length)
          num2 = messagePlaintext.IndexOf('[', index + 1);
        if (num1 != -1 && (num2 == -1 || num2 > num1))
        {
          string str = messagePlaintext.Substring(index + 1, num1 - index - 1);
          int result;
          if (int.TryParse(str, out result))
          {
            if (result < EmojiMenu.totalEmojis)
              this.message.Add(new ChatSnippet(result));
          }
          else
          {
            if (str != null)
            {
              switch (str.Length)
              {
                case 3:
                  if (str == "red")
                    break;
                  goto label_36;
                case 4:
                  switch (str[0])
                  {
                    case 'a':
                      if (str == "aqua")
                        break;
                      goto label_36;
                    case 'b':
                      if (str == "blue")
                        break;
                      goto label_36;
                    case 'g':
                      if (str == "gray")
                        break;
                      goto label_36;
                    case 'j':
                      if (str == "jade")
                        break;
                      goto label_36;
                    case 'p':
                      if (str == "pink" || str == "plum")
                        break;
                      goto label_36;
                    default:
                      goto label_36;
                  }
                  break;
                case 5:
                  switch (str[0])
                  {
                    case 'b':
                      if (str == "brown")
                        break;
                      goto label_36;
                    case 'c':
                      if (str == "cream")
                        break;
                      goto label_36;
                    case 'g':
                      if (str == "green")
                        break;
                      goto label_36;
                    case 'p':
                      if (str == "peach")
                        break;
                      goto label_36;
                    default:
                      goto label_36;
                  }
                  break;
                case 6:
                  switch (str[0])
                  {
                    case 'j':
                      if (str == "jungle")
                        break;
                      goto label_36;
                    case 'o':
                      if (str == "orange")
                        break;
                      goto label_36;
                    case 'p':
                      if (str == "purple")
                        break;
                      goto label_36;
                    case 's':
                      if (str == "salmon")
                        break;
                      goto label_36;
                    case 'y':
                      if (str == "yellow")
                        break;
                      goto label_36;
                    default:
                      goto label_36;
                  }
                  break;
                case 11:
                  if (!(str == "yellowgreen"))
                    goto label_36;
                  break;
                default:
                  goto label_36;
              }
              if (this.color.Equals(Color.White))
              {
                this.color = ChatMessage.getColorFromName(str);
                goto label_37;
              }
              goto label_37;
            }
label_36:
            sb.Append("[");
            sb.Append(str);
            sb.Append("]");
          }
label_37:
          index = num1;
        }
        else
          sb.Append("[");
      }
      else
        sb.Append(messagePlaintext[index]);
    }
    if (sb.Length <= 0)
      return;
    this.breakNewLines(sb);
  }

  public static Color getColorFromName(string name)
  {
    if (name != null)
    {
      switch (name.Length)
      {
        case 3:
          if (name == "red")
            return new Color(220, 20, 20);
          break;
        case 4:
          switch (name[0])
          {
            case 'a':
              if (name == "aqua")
                return Color.MediumTurquoise;
              break;
            case 'b':
              if (name == "blue")
                return Color.DodgerBlue;
              break;
            case 'g':
              if (name == "gray")
                return Color.Gray;
              break;
            case 'j':
              if (name == "jade")
                return new Color(50, 230, 150);
              break;
            case 'p':
              switch (name)
              {
                case "pink":
                  return Color.HotPink;
                case "plum":
                  return new Color(190, 0, 190);
              }
              break;
          }
          break;
        case 5:
          switch (name[0])
          {
            case 'b':
              if (name == "brown")
                return new Color(160 /*0xA0*/, 80 /*0x50*/, 30);
              break;
            case 'c':
              if (name == "cream")
                return new Color((int) byte.MaxValue, (int) byte.MaxValue, 180);
              break;
            case 'g':
              if (name == "green")
                return new Color(0, 180, 10);
              break;
            case 'p':
              if (name == "peach")
                return new Color((int) byte.MaxValue, 180, 120);
              break;
          }
          break;
        case 6:
          switch (name[0])
          {
            case 'j':
              if (name == "jungle")
                return Color.SeaGreen;
              break;
            case 'o':
              if (name == "orange")
                return new Color((int) byte.MaxValue, 100, 0);
              break;
            case 'p':
              if (name == "purple")
                return new Color(138, 43, 250);
              break;
            case 's':
              if (name == "salmon")
                return Color.Salmon;
              break;
            case 'y':
              if (name == "yellow")
                return new Color(240 /*0xF0*/, 200, 0);
              break;
          }
          break;
        case 11:
          if (name == "yellowgreen")
            return new Color(182, 214, 0);
          break;
      }
    }
    return Color.White;
  }

  private void breakNewLines(StringBuilder sb)
  {
    string[] strArray = sb.ToString().Split(Environment.NewLine);
    for (int index = 0; index < strArray.Length; ++index)
    {
      this.message.Add(new ChatSnippet(strArray[index], this.language));
      if (index != strArray.Length - 1)
        this.message.Add(new ChatSnippet(Environment.NewLine, this.language));
    }
  }

  public static string makeMessagePlaintext(
    List<ChatSnippet> message,
    bool include_color_information)
  {
    StringBuilder stringBuilder = new StringBuilder();
    foreach (ChatSnippet chatSnippet in message)
    {
      if (chatSnippet.message != null)
        stringBuilder.Append(chatSnippet.message);
      else if (chatSnippet.emojiIndex != -1)
        stringBuilder.Append($"[{chatSnippet.emojiIndex.ToString()}]");
    }
    if (include_color_information && Game1.player.defaultChatColor != null && !ChatMessage.getColorFromName(Game1.player.defaultChatColor).Equals(Color.White))
    {
      stringBuilder.Append(" [");
      stringBuilder.Append(Game1.player.defaultChatColor);
      stringBuilder.Append("]");
    }
    return stringBuilder.ToString();
  }

  public void draw(SpriteBatch b, int x, int y)
  {
    float num1 = 0.0f;
    float num2 = 0.0f;
    for (int index = 0; index < this.message.Count; ++index)
    {
      if (this.message[index].emojiIndex != -1)
        b.Draw(ChatBox.emojiTexture, new Vector2((float) ((double) x + (double) num1 + 1.0), (float) ((double) y + (double) num2 - 4.0)), new Rectangle?(new Rectangle(this.message[index].emojiIndex * 9 % ChatBox.emojiTexture.Width, this.message[index].emojiIndex * 9 / ChatBox.emojiTexture.Width * 9, 9, 9)), Color.White * this.alpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.99f);
      else if (this.message[index].message != null)
      {
        if (this.message[index].message.Equals(Environment.NewLine))
        {
          num1 = 0.0f;
          num2 += ChatBox.messageFont(this.language).MeasureString("(").Y;
        }
        else
          b.DrawString(ChatBox.messageFont(this.language), this.message[index].message, new Vector2((float) x + num1, (float) y + num2), this.color * this.alpha, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.99f);
      }
      num1 += this.message[index].myLength;
      if ((double) num1 >= 888.0)
      {
        num1 = 0.0f;
        num2 += ChatBox.messageFont(this.language).MeasureString("(").Y;
        if (this.message.Count > index + 1 && this.message[index + 1].message != null && this.message[index + 1].message.Equals(Environment.NewLine))
          ++index;
      }
    }
  }
}
