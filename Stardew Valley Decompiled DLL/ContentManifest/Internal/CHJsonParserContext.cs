// Decompiled with JetBrains decompiler
// Type: ContentManifest.Internal.CHJsonParserContext
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using System;

#nullable disable
namespace ContentManifest.Internal;

internal class CHJsonParserContext
{
  public int ReadHead;
  public string JsonText = "";

  public CHJsonParserContext(string jsonText) => this.JsonText = jsonText;

  public void SkipWhitespace()
  {
    for (; this.ReadHead < this.JsonText.Length; ++this.ReadHead)
    {
      switch (this.JsonText[this.ReadHead])
      {
        case '\t':
        case '\n':
        case '\r':
        case ' ':
          continue;
        case '\v':
          return;
        case '\f':
          return;
        default:
          return;
      }
    }
  }

  public void AssertReadHeadIsValid()
  {
    if (this.ReadHead < 0 || this.ReadHead >= this.JsonText.Length)
      throw new InvalidOperationException();
  }
}
