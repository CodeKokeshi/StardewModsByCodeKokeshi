// Decompiled with JetBrains decompiler
// Type: StardewValley.Util.ProfanityFilter
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

#nullable disable
namespace StardewValley.Util;

internal class ProfanityFilter
{
  private readonly List<Regex> _words;
  private readonly StringBuilder _cleanup;

  public ProfanityFilter()
    : this("Content/profanity.regex")
  {
  }

  public ProfanityFilter(string profanityFile)
  {
    this._cleanup = new StringBuilder(2048 /*0x0800*/);
    string[] strArray = File.ReadAllLines(profanityFile);
    this._words = new List<Regex>(strArray.Length);
    for (int index = 0; index < strArray.Length; ++index)
      this._words.Add(new Regex(strArray[index], RegexOptions.IgnoreCase | RegexOptions.Compiled));
  }

  public string Filter(string words)
  {
    if (string.IsNullOrWhiteSpace(words))
      return words;
    for (int index1 = 0; index1 < this._words.Count; ++index1)
    {
      MatchCollection matchCollection = this._words[index1].Matches(words);
      if (matchCollection.Count != 0)
      {
        this._cleanup.Clear();
        this._cleanup.Append(words);
        for (int i = 0; i < matchCollection.Count; ++i)
        {
          Match match = matchCollection[i];
          int num = match.Index + match.Length;
          for (int index2 = match.Index; index2 < num; ++index2)
          {
            if (!char.IsWhiteSpace(this._cleanup[index2]))
              this._cleanup[index2] = '*';
          }
        }
        words = this._cleanup.ToString();
      }
    }
    return words;
  }
}
