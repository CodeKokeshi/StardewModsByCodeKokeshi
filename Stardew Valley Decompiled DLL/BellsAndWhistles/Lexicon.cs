// Decompiled with JetBrains decompiler
// Type: StardewValley.BellsAndWhistles.Lexicon
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using StardewValley.Extensions;
using StardewValley.TokenizableStrings;
using System;
using System.Linq;

#nullable disable
namespace StardewValley.BellsAndWhistles;

public class Lexicon
{
  /// <summary>
  /// 
  /// A noun to represent some kind of "bad" object. Kind of has connotations of it being disgusting or cheap. preface with "that" or "such"
  /// 
  /// </summary>
  /// <returns></returns>
  public static string getRandomNegativeItemSlanderNoun()
  {
    return Utility.CreateDaySaveRandom().Choose<string>(Game1.content.LoadString("Strings\\Lexicon:RandomNegativeItemNoun").Split('#'));
  }

  public static string getProperArticleForWord(string word)
  {
    if (LocalizedContentManager.CurrentLanguageCode != LocalizedContentManager.LanguageCode.en)
      return "";
    if (word != null && word.Length > 0)
    {
      switch (word.ToLower()[0])
      {
        case 'a':
        case 'e':
        case 'i':
        case 'o':
        case 'u':
          return "an";
      }
    }
    return "a";
  }

  public static string capitalize(string text)
  {
    if (string.IsNullOrEmpty(text) || LocalizedContentManager.CurrentLanguageCode != LocalizedContentManager.LanguageCode.en)
      return text;
    int num = 0;
    for (int index = 0; index < text.Length; ++index)
    {
      if (char.IsLetter(text[index]))
      {
        num = index;
        break;
      }
    }
    return num == 0 ? text[0].ToString().ToUpper() + text.Substring(1) : text.Substring(0, num) + text[num].ToString().ToUpper() + text.Substring(num + 1);
  }

  public static string makePlural(string word, bool ignore = false)
  {
    if (ignore || LocalizedContentManager.CurrentLanguageCode != LocalizedContentManager.LanguageCode.en || word == null)
      return word;
    if (word != null)
    {
      switch (word.Length)
      {
        case 3:
          if (word == "Hay")
            break;
          goto label_66;
        case 4:
          switch (word[3])
          {
            case 'b':
              if (word == "Chub")
                break;
              goto label_66;
            case 'l':
              if (word == "Coal")
                return "lumps of Coal";
              goto label_66;
            case 'p':
              if (word == "Carp")
                break;
              goto label_66;
            case 's':
              if (word == "Hops")
                break;
              goto label_66;
            case 't':
              if (word == "Salt")
                return "pieces of Salt";
              goto label_66;
            case 'y':
              if (word == "Clay")
                break;
              goto label_66;
            default:
              goto label_66;
          }
          break;
        case 5:
          switch (word[4])
          {
            case 'm':
              if (word == "Bream")
                break;
              goto label_66;
            case 's':
              if (word == "Weeds")
                break;
              goto label_66;
            case 't':
              if (word == "Wheat")
                return "bushels of Wheat";
              goto label_66;
            case 'y':
              if (word == "Jelly")
                return "Jellies";
              goto label_66;
            default:
              goto label_66;
          }
          break;
        case 6:
          switch (word[1])
          {
            case 'a':
              if (word == "Garlic")
                return "bulbs of Garlic";
              goto label_66;
            case 'i':
              if (word == "Ginger")
                return "pieces of Ginger";
              goto label_66;
            default:
              goto label_66;
          }
        case 7:
          if (word == "Pickles")
            break;
          goto label_66;
        case 8:
          switch (word[0])
          {
            case 'B':
              if (word == "Bok Choy")
                break;
              goto label_66;
            case 'P':
              if (word == "Pancakes")
                break;
              goto label_66;
            case 'S':
              if (word == "Sandfish")
                break;
              goto label_66;
            default:
              goto label_66;
          }
          break;
        case 9:
          switch (word[0])
          {
            case 'D':
              if (word == "Driftwood")
                break;
              goto label_66;
            case 'G':
              if (word == "Ghostfish")
                break;
              goto label_66;
            case 'R':
              if (word == "Red Canes")
                break;
              goto label_66;
            default:
              goto label_66;
          }
          break;
        case 10:
          switch (word[0])
          {
            case 'A':
              if (word == "Algae Soup")
                return "bowls of Algae Soup";
              goto label_66;
            case 'C':
              if (word == "Crab Cakes")
                break;
              goto label_66;
            case 'H':
              if (word == "Hashbrowns")
                break;
              goto label_66;
            case 'T':
              if (word == "Tea Leaves")
                break;
              goto label_66;
            default:
              goto label_66;
          }
          break;
        case 11:
          switch (word[4])
          {
            case ' ':
              if (word == "Star Shards")
                break;
              goto label_66;
            case 'b':
              if (word == "Cranberries")
                break;
              goto label_66;
            case 'd':
              if (word == "Mixed Seeds")
                break;
              goto label_66;
            case 'e':
              if (word == "Glazed Yams")
                break;
              goto label_66;
            case 'n':
              if (word == "Green Canes")
                break;
              goto label_66;
            default:
              goto label_66;
          }
          break;
        case 12:
          switch (word[0])
          {
            case 'D':
              if (word == "Dragon Tooth")
                return "Dragon Teeth";
              goto label_66;
            case 'G':
              if (word == "Glass Shards")
                break;
              goto label_66;
            case 'R':
              if (word == "Rice Pudding")
                return "bowls of Rice Pudding";
              goto label_66;
            default:
              goto label_66;
          }
          break;
        case 14:
          switch (word[0])
          {
            case 'B':
              if (word == "Broken Glasses")
                break;
              goto label_66;
            case 'P':
              if (word == "Pepper Poppers")
                break;
              goto label_66;
            default:
              goto label_66;
          }
          break;
        case 15:
          switch (word[0])
          {
            case 'F':
              if (word == "Fossilized Ribs")
                break;
              goto label_66;
            case 'L':
              if (word == "Largemouth Bass")
                break;
              goto label_66;
            case 'S':
              if (word == "Smallmouth Bass")
                break;
              goto label_66;
            default:
              goto label_66;
          }
          break;
        case 16 /*0x10*/:
          if (word == "Dried Sunflowers")
            break;
          goto label_66;
        case 17:
          switch (word[0])
          {
            case 'D':
              if (word == "Dried Cranberries")
                break;
              goto label_66;
            case 'R':
              if (word == "Roasted Hazelnuts")
                break;
              goto label_66;
            default:
              goto label_66;
          }
          break;
        case 21:
          if (word == "Warp Totem: Mountains")
            break;
          goto label_66;
        default:
          goto label_66;
      }
      return word;
    }
label_66:
    switch (word.Last<char>())
    {
      case 's':
        return !word.EndsWith(" Seeds") && !word.EndsWith(" Shorts") && !word.EndsWith(" Bass") && !word.EndsWith(" Flowers") && !word.EndsWith(" Peach") ? word + "es" : word;
      case 'x':
      case 'z':
        return word + "es";
      case 'y':
        return word.Substring(0, word.Length - 1) + "ies";
      default:
        if (word.Length > 2)
        {
          string str = word.Substring(word.Length - 2);
          if (str == "sh" || str == "ch")
            return word + "es";
        }
        return word + "s";
    }
  }

  /// <summary>In English only, prepend an article like 'a' or 'an' to a word.</summary>
  /// <param name="word">The word for which to prepend an article.</param>
  public static string prependArticle(string word)
  {
    return LocalizedContentManager.CurrentLanguageCode != LocalizedContentManager.LanguageCode.en ? word : $"{Lexicon.getProperArticleForWord(word)} {word}";
  }

  /// <summary>In English only, prepend an article like 'a' or 'an' to a word as a <see cref="T:StardewValley.TokenizableStrings.TokenParser">tokenizable string</see>.</summary>
  /// <param name="word">The tokenizable string which returns the word for which to prepend an article.</param>
  public static string prependTokenizedArticle(string word)
  {
    return LocalizedContentManager.CurrentLanguageCode != LocalizedContentManager.LanguageCode.en ? word : $"{TokenStringBuilder.ArticleFor(word)} {word}";
  }

  /// <summary>
  /// 
  /// Adjectives like "wonderful" "amazing" "excellent", prefaced with "are"  "is"  "was" "will be" "usually is", etc.
  /// these wouldn't really make sense for an object, more for a person,place, or event
  /// </summary>
  /// <returns></returns>
  public static string getRandomPositiveAdjectiveForEventOrPerson(NPC n = null)
  {
    Random daySaveRandom = Utility.CreateDaySaveRandom();
    string[] strArray;
    if (n != null && n.Age != 0)
    {
      strArray = Game1.content.LoadString("Strings\\Lexicon:RandomPositiveAdjective_Child").Split('#');
    }
    else
    {
      Gender? gender = n?.Gender;
      if (gender.HasValue)
      {
        switch (gender.GetValueOrDefault())
        {
          case Gender.Male:
            strArray = Game1.content.LoadString("Strings\\Lexicon:RandomPositiveAdjective_AdultMale").Split('#');
            goto label_7;
          case Gender.Female:
            strArray = Game1.content.LoadString("Strings\\Lexicon:RandomPositiveAdjective_AdultFemale").Split('#');
            goto label_7;
        }
      }
      strArray = Game1.content.LoadString("Strings\\Lexicon:RandomPositiveAdjective_PlaceOrEvent").Split('#');
    }
label_7:
    return daySaveRandom.Choose<string>(strArray);
  }

  /// <summary>
  /// 
  /// An adjective to represent something tasty, like "delicious", "tasty", "wonderful", "satisfying"
  /// 
  /// </summary>
  /// <returns></returns>
  public static string getRandomDeliciousAdjective(NPC n = null)
  {
    return Utility.CreateDaySaveRandom().Choose<string>(n == null || n.Age != 2 ? Game1.content.LoadString("Strings\\Lexicon:RandomDeliciousAdjective").Split('#') : Game1.content.LoadString("Strings\\Lexicon:RandomDeliciousAdjective_Child").Split('#'));
  }

  /// <summary>
  /// 
  /// Adjective to describe something that is not tasty. "gross", "disgusting", "nasty"
  /// </summary>
  /// <returns></returns>
  public static string getRandomNegativeFoodAdjective(NPC n = null)
  {
    return Utility.CreateDaySaveRandom().Choose<string>(n == null || n.Age != 2 ? (n == null || n.Manners != 1 ? Game1.content.LoadString("Strings\\Lexicon:RandomNegativeFoodAdjective").Split('#') : Game1.content.LoadString("Strings\\Lexicon:RandomNegativeFoodAdjective_Polite").Split('#')) : Game1.content.LoadString("Strings\\Lexicon:RandomNegativeFoodAdjective_Child").Split('#'));
  }

  /// <summary>Adjectives like "decent" "good"</summary>
  /// <returns></returns>
  public static string getRandomSlightlyPositiveAdjectiveForEdibleNoun(NPC n = null)
  {
    return Utility.CreateDaySaveRandom().Choose<string>(Game1.content.LoadString("Strings\\Lexicon:RandomSlightlyPositiveFoodAdjective").Split('#'));
  }

  /// <summary>Get a generic term for a child of a given gender (i.e. "boy" or "girl").</summary>
  /// <param name="isMale">Whether the child is male.</param>
  public static string getGenderedChildTerm(bool isMale)
  {
    return Game1.content.LoadString(isMale ? "Strings\\Lexicon:ChildTerm_Male" : "Strings\\Lexicon:ChildTerm_Female");
  }

  /// <summary>Get a generic term for a child of a given gender (i.e. "boy" or "girl"), as a <see cref="T:StardewValley.TokenizableStrings.TokenParser">tokenizable string</see>.</summary>
  /// <param name="isMale">Whether the child is male.</param>
  public static string getTokenizedGenderedChildTerm(bool isMale)
  {
    return TokenStringBuilder.LocalizedText(isMale ? "Strings\\Lexicon:ChildTerm_Male" : "Strings\\Lexicon:ChildTerm_Female");
  }

  /// <summary>Get a gendered pronoun (i.e. "him" or "her").</summary>
  /// <param name="isMale">Whether to get a male pronoun.</param>
  public static string getPronoun(bool isMale)
  {
    return Game1.content.LoadString(isMale ? "Strings\\Lexicon:Pronoun_Male" : "Strings\\Lexicon:Pronoun_Female");
  }

  /// <summary>Get a gendered pronoun (i.e. "him" or "her"), as a <see cref="T:StardewValley.TokenizableStrings.TokenParser">tokenizable string</see>.</summary>
  /// <param name="isMale">Whether to get a male pronoun.</param>
  public static string getTokenizedPronoun(bool isMale)
  {
    return TokenStringBuilder.LocalizedText(isMale ? "Strings\\Lexicon:Pronoun_Male" : "Strings\\Lexicon:Pronoun_Female");
  }

  /// <summary>Get a possessive gendered pronoun (i.e. "his" or "her").</summary>
  /// <param name="isMale">Whether to get a male pronoun.</param>
  public static string getPossessivePronoun(bool isMale)
  {
    return Game1.content.LoadString(isMale ? "Strings\\Lexicon:Possessive_Pronoun_Male" : "Strings\\Lexicon:Possessive_Pronoun_Female");
  }

  /// <summary>Get a possessive gendered pronoun (i.e. "his" or "her"), as a <see cref="T:StardewValley.TokenizableStrings.TokenParser">tokenizable string</see>.</summary>
  /// <param name="isMale">Whether to get a male pronoun.</param>
  public static string getTokenizedPossessivePronoun(bool isMale)
  {
    return TokenStringBuilder.LocalizedText(isMale ? "Strings\\Lexicon:Possessive_Pronoun_Male" : "Strings\\Lexicon:Possessive_Pronoun_Female");
  }
}
