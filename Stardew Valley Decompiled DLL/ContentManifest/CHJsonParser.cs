// Decompiled with JetBrains decompiler
// Type: ContentManifest.CHJsonParser
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using ContentManifest.Internal;

#nullable disable
namespace ContentManifest;

public class CHJsonParser
{
  public static object ParseJson(string text)
  {
    CHJsonParserContext context = new CHJsonParserContext(text);
    CHJson chJson = new CHJson();
    chJson.Parse(context);
    return chJson.Element.Value.GetManagedObject();
  }
}
