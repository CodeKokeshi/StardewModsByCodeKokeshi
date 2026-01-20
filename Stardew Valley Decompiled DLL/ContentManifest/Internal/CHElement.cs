// Decompiled with JetBrains decompiler
// Type: ContentManifest.Internal.CHElement
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

#nullable disable
namespace ContentManifest.Internal;

internal class CHElement : CHParsable
{
  public CHValue Value;

  public void Parse(CHJsonParserContext context)
  {
    context.SkipWhitespace();
    this.Value = new CHValue();
    this.Value.Parse(context);
    context.SkipWhitespace();
  }
}
