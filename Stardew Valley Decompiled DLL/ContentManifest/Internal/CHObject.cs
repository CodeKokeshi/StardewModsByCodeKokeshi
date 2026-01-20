// Decompiled with JetBrains decompiler
// Type: ContentManifest.Internal.CHObject
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using System;
using System.Collections.Generic;

#nullable disable
namespace ContentManifest.Internal;

internal class CHObject : CHParsable
{
  public readonly Dictionary<string, object> Members = new Dictionary<string, object>();

  public void Parse(CHJsonParserContext context)
  {
    if (context.JsonText[context.ReadHead] != '{')
      throw new InvalidOperationException();
    ++context.ReadHead;
    bool flag = false;
    while (true)
    {
      do
      {
        context.SkipWhitespace();
        context.AssertReadHeadIsValid();
        switch (context.JsonText[context.ReadHead])
        {
          case '"':
            CHString chString = new CHString();
            chString.Parse(context);
            context.SkipWhitespace();
            context.AssertReadHeadIsValid();
            if (context.JsonText[context.ReadHead] != ':')
              throw new InvalidOperationException();
            ++context.ReadHead;
            CHElement chElement = new CHElement();
            chElement.Parse(context);
            this.Members[chString.RawString] = chElement.Value.GetManagedObject();
            flag = false;
            context.SkipWhitespace();
            context.AssertReadHeadIsValid();
            continue;
          case '}':
            if (flag)
              throw new InvalidOperationException();
            ++context.ReadHead;
            return;
          default:
            goto label_11;
        }
      }
      while (context.JsonText[context.ReadHead] != ',');
      ++context.ReadHead;
      flag = true;
    }
label_11:
    throw new InvalidOperationException();
  }
}
