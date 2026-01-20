// Decompiled with JetBrains decompiler
// Type: ContentManifest.Internal.CHNumber
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using System;
using System.Globalization;
using System.Text;

#nullable disable
namespace ContentManifest.Internal;

internal class CHNumber : CHParsable
{
  private static StringBuilder DoubleSb;
  public double RawDouble;

  public static bool IsValidPrefix(char prefixChar)
  {
    if (prefixChar == '-')
      return true;
    return '0' <= prefixChar && prefixChar <= '9';
  }

  public void Parse(CHJsonParserContext context)
  {
    CHNumber.EnsureStringBuilderInitialized();
    CHNumber.DoubleSb.Clear();
    if (context.JsonText[context.ReadHead] == '-')
    {
      ++context.ReadHead;
      CHNumber.DoubleSb.Append('-');
    }
    context.AssertReadHeadIsValid();
    char ch1 = context.JsonText[context.ReadHead];
    if (ch1 == '0')
    {
      ++context.ReadHead;
      if (context.ReadHead < context.JsonText.Length)
      {
        char ch2 = context.JsonText[context.ReadHead];
        if ('1' <= ch2 && ch2 <= '9')
          throw new InvalidOperationException();
      }
      CHNumber.DoubleSb.Append('0');
    }
    else
    {
      if ('1' > ch1 || ch1 > '9')
        throw new InvalidOperationException();
      ++context.ReadHead;
      CHNumber.DoubleSb.Append(ch1);
    }
    this.ParseDigits(context);
    if (context.ReadHead < context.JsonText.Length && context.JsonText[context.ReadHead] == '.')
    {
      ++context.ReadHead;
      context.AssertReadHeadIsValid();
      CHNumber.DoubleSb.Append('.');
      this.ParseDigits(context);
    }
    if (context.ReadHead < context.JsonText.Length)
    {
      switch (context.JsonText[context.ReadHead])
      {
        case 'E':
        case 'e':
          ++context.ReadHead;
          context.AssertReadHeadIsValid();
          CHNumber.DoubleSb.Append('E');
          char ch3 = context.JsonText[context.ReadHead];
          switch (ch3)
          {
            case '+':
            case '-':
              ++context.ReadHead;
              context.AssertReadHeadIsValid();
              CHNumber.DoubleSb.Append(ch3);
              break;
          }
          this.ParseDigits(context);
          break;
      }
    }
    this.RawDouble = double.Parse(CHNumber.DoubleSb.ToString(), (IFormatProvider) CultureInfo.InvariantCulture);
  }

  private void ParseDigits(CHJsonParserContext context)
  {
    string jsonText = context.JsonText;
    int readHead;
    for (readHead = context.ReadHead; readHead < jsonText.Length; ++readHead)
    {
      char ch = jsonText[readHead];
      switch (ch)
      {
        case '0':
        case '1':
        case '2':
        case '3':
        case '4':
        case '5':
        case '6':
        case '7':
        case '8':
        case '9':
          CHNumber.DoubleSb.Append(ch);
          continue;
        default:
          goto label_4;
      }
    }
label_4:
    context.ReadHead = readHead;
  }

  private static void EnsureStringBuilderInitialized()
  {
    string str = Convert.ToString(long.MaxValue);
    CHNumber.DoubleSb = new StringBuilder("-".Length + str.Length + ".".Length + str.Length + "E".Length + "+".Length + str.Length);
  }
}
