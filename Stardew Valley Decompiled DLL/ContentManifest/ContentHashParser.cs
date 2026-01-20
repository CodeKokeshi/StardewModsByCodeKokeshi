// Decompiled with JetBrains decompiler
// Type: ContentManifest.ContentHashParser
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using System.Collections.Generic;
using System.IO;

#nullable disable
namespace ContentManifest;

public class ContentHashParser
{
  public static Dictionary<string, object> ParseFromFile(string contentHashPath)
  {
    return CHJsonParser.ParseJson(File.ReadAllText(contentHashPath)) as Dictionary<string, object>;
  }
}
