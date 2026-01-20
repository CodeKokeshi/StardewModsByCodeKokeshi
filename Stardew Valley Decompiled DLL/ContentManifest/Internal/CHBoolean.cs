// Decompiled with JetBrains decompiler
// Type: ContentManifest.Internal.CHBoolean
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using System;

#nullable disable
namespace ContentManifest.Internal;

internal class CHBoolean : CHParsable
{
  public bool RawBoolean;

  public void Parse(CHJsonParserContext context)
  {
    string jsonText = context.JsonText;
    int readHead = context.ReadHead;
    switch (jsonText[readHead])
    {
      case 'f':
        if (readHead + 4 >= jsonText.Length)
          throw new InvalidOperationException();
        if (jsonText[readHead + 1] != 'a' || jsonText[readHead + 2] != 'l' || jsonText[readHead + 3] != 's' || jsonText[readHead + 4] != 'e')
          throw new InvalidOperationException();
        context.ReadHead += 5;
        this.RawBoolean = false;
        break;
      case 't':
        if (readHead + 3 >= jsonText.Length)
          throw new InvalidOperationException();
        if (jsonText[readHead + 1] != 'r' || jsonText[readHead + 2] != 'u' || jsonText[readHead + 3] != 'e')
          throw new InvalidOperationException();
        context.ReadHead += 4;
        this.RawBoolean = true;
        break;
      default:
        throw new NotImplementedException();
    }
  }
}
