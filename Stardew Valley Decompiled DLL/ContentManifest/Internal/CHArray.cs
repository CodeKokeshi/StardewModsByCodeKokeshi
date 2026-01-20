// Decompiled with JetBrains decompiler
// Type: ContentManifest.Internal.CHArray
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using System;
using System.Collections.Generic;

#nullable disable
namespace ContentManifest.Internal;

internal class CHArray : CHParsable
{
  private static readonly List<object> ElementList = new List<object>();
  public object[] Elements;

  public void Parse(CHJsonParserContext context)
  {
    if (context.JsonText[context.ReadHead] != '[')
      throw new InvalidOperationException();
    ++context.ReadHead;
    bool flag = false;
    CHArray.ElementList.Clear();
    while (true)
    {
      do
      {
        context.SkipWhitespace();
        context.AssertReadHeadIsValid();
        if (context.JsonText[context.ReadHead] == ']')
        {
          if (flag)
            throw new InvalidOperationException();
          this.Elements = CHArray.ElementList.ToArray();
          ++context.ReadHead;
          return;
        }
        CHElement chElement = new CHElement();
        chElement.Parse(context);
        CHArray.ElementList.Add(chElement.Value.GetManagedObject());
        flag = false;
        context.SkipWhitespace();
        context.AssertReadHeadIsValid();
      }
      while (context.JsonText[context.ReadHead] != ',');
      ++context.ReadHead;
      flag = true;
    }
  }
}
