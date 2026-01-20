// Decompiled with JetBrains decompiler
// Type: ContentManifest.Internal.CHString
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using System;
using System.Text;

#nullable disable
namespace ContentManifest.Internal;

internal class CHString : CHParsable
{
  public string RawString = "";

  public void Parse(CHJsonParserContext context)
  {
    if (context.JsonText[context.ReadHead] != '"')
      throw new InvalidOperationException();
    ++context.ReadHead;
    int readHead = context.ReadHead;
    string jsonText = context.JsonText;
    StringBuilder stringBuilder = new StringBuilder();
    for (; readHead < jsonText.Length; ++readHead)
    {
      char ch1 = jsonText[readHead];
      switch (ch1)
      {
        case '"':
          this.RawString = stringBuilder.ToString();
          context.ReadHead = readHead + 1;
          return;
        case '\\':
          ++readHead;
          if (readHead >= jsonText.Length)
            throw new InvalidOperationException();
          char ch2 = jsonText[readHead];
          switch (ch2)
          {
            case '"':
            case '/':
            case '\\':
              stringBuilder.Append(ch2);
              continue;
            case 'b':
              stringBuilder.Append('\b');
              continue;
            case 'f':
              stringBuilder.Append('\f');
              continue;
            case 'n':
              stringBuilder.Append('\n');
              continue;
            case 'r':
              stringBuilder.Append('\r');
              continue;
            case 't':
              stringBuilder.Append('\t');
              continue;
            case 'u':
              if (readHead + 4 >= jsonText.Length)
                throw new InvalidOperationException();
              string str = char.ConvertFromUtf32(0 | (this.ParseHexChar(jsonText[readHead + 1]) & 15) << 12 | (this.ParseHexChar(jsonText[readHead + 2]) & 15) << 8 | (this.ParseHexChar(jsonText[readHead + 3]) & 15) << 4 | this.ParseHexChar(jsonText[readHead + 4]) & 15);
              if (str.Length != 1)
                throw new InvalidOperationException();
              stringBuilder.Append(str[0]);
              readHead += 4;
              continue;
            default:
              continue;
          }
        default:
          stringBuilder.Append(ch1);
          break;
      }
    }
    throw new InvalidOperationException();
  }

  private int ParseHexChar(char hexChar)
  {
    if ('0' <= hexChar && hexChar < '9')
      return (int) hexChar - 48 /*0x30*/;
    if ('a' <= hexChar && hexChar <= 'z')
      return (int) hexChar - 97 + 10;
    if ('A' <= hexChar && hexChar <= 'Z')
      return (int) hexChar - 65 + 10;
    throw new InvalidOperationException();
  }
}
