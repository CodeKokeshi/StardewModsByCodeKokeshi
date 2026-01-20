// Decompiled with JetBrains decompiler
// Type: StardewValley.Tests.SyntaxAbstractor
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

#nullable disable
namespace StardewValley.Tests;

/// <summary>Converts raw text from dialogue, event, mail, and data assets into language-independent syntax representations, which can be compared between languages to make sure they have the same sequence of commands, portraits, unlocalized metadata and delimiters, etc.</summary>
/// <remarks>
///   <para><strong>This is highly specialized.</strong> It's meant for vanilla unit tests, so it may not correctly handle non-vanilla text and may change at any time.</para>
/// 
///   For example, this converts a dialogue string like this:
///   <code>$c 0.5#Wow... Thanks, @!$h#Thank you! It's so pretty.</code>
/// 
///   Into a language-independent representation like this:
///   <code>$c 0.5#text$h#text</code>
/// </remarks>
public class SyntaxAbstractor
{
  /// <summary>The placeholder in <see cref="M:StardewValley.Tests.SyntaxAbstractor.ExtractSyntaxFor(System.String,System.String,System.String)" /> for localizable text.</summary>
  public const string TextMarker = "text";
  /// <summary>The implementations which extract syntax from specific assets, indexed by exact match or prefix.</summary>
  public readonly Dictionary<string, ExtractSyntaxDelegate> SyntaxHandlers = new Dictionary<string, ExtractSyntaxDelegate>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
  {
    ["Characters/Dialogue/*"] = new ExtractSyntaxDelegate(SyntaxAbstractor.DialogueSyntaxHandler),
    ["Data/EngagementDialogue"] = new ExtractSyntaxDelegate(SyntaxAbstractor.DialogueSyntaxHandler),
    ["Data/ExtraDialogue"] = new ExtractSyntaxDelegate(SyntaxAbstractor.DialogueSyntaxHandler),
    ["Strings/animationDescriptions"] = new ExtractSyntaxDelegate(SyntaxAbstractor.DialogueSyntaxHandler),
    ["Strings/Buildings"] = new ExtractSyntaxDelegate(SyntaxAbstractor.DialogueSyntaxHandler),
    ["Strings/Characters"] = new ExtractSyntaxDelegate(SyntaxAbstractor.DialogueSyntaxHandler),
    ["Strings/Events"] = new ExtractSyntaxDelegate(SyntaxAbstractor.DialogueSyntaxHandler),
    ["Strings/Locations"] = new ExtractSyntaxDelegate(SyntaxAbstractor.DialogueSyntaxHandler),
    ["Strings/MovieReactions"] = new ExtractSyntaxDelegate(SyntaxAbstractor.DialogueSyntaxHandler),
    ["Strings/Objects"] = new ExtractSyntaxDelegate(SyntaxAbstractor.DialogueSyntaxHandler),
    ["Strings/Quests"] = new ExtractSyntaxDelegate(SyntaxAbstractor.DialogueSyntaxHandler),
    ["Strings/schedules/*"] = new ExtractSyntaxDelegate(SyntaxAbstractor.DialogueSyntaxHandler),
    ["Strings/SimpleNonVillagerDialogues"] = new ExtractSyntaxDelegate(SyntaxAbstractor.DialogueSyntaxHandler),
    ["Strings/SpecialOrderStrings"] = new ExtractSyntaxDelegate(SyntaxAbstractor.DialogueSyntaxHandler),
    ["Strings/SpeechBubbles"] = new ExtractSyntaxDelegate(SyntaxAbstractor.DialogueSyntaxHandler),
    ["Strings/StringsFromCSFiles"] = new ExtractSyntaxDelegate(SyntaxAbstractor.DialogueSyntaxHandler),
    ["Strings/StringsFromMaps"] = new ExtractSyntaxDelegate(SyntaxAbstractor.DialogueSyntaxHandler),
    ["Strings/BigCraftables"] = new ExtractSyntaxDelegate(SyntaxAbstractor.PlainTextSyntaxHandler),
    ["Strings/BundleNames"] = new ExtractSyntaxDelegate(SyntaxAbstractor.PlainTextSyntaxHandler),
    ["Strings/EnchantmentNames"] = new ExtractSyntaxDelegate(SyntaxAbstractor.PlainTextSyntaxHandler),
    ["Strings/FarmAnimals"] = new ExtractSyntaxDelegate(SyntaxAbstractor.PlainTextSyntaxHandler),
    ["Strings/Furniture"] = new ExtractSyntaxDelegate(SyntaxAbstractor.PlainTextSyntaxHandler),
    ["Strings/MovieConcessions"] = new ExtractSyntaxDelegate(SyntaxAbstractor.PlainTextSyntaxHandler),
    ["Strings/Movies"] = new ExtractSyntaxDelegate(SyntaxAbstractor.PlainTextSyntaxHandler),
    ["Strings/NPCNames"] = new ExtractSyntaxDelegate(SyntaxAbstractor.PlainTextSyntaxHandler),
    ["Strings/Pants"] = new ExtractSyntaxDelegate(SyntaxAbstractor.PlainTextSyntaxHandler),
    ["Strings/Shirts"] = new ExtractSyntaxDelegate(SyntaxAbstractor.PlainTextSyntaxHandler),
    ["Strings/Tools"] = new ExtractSyntaxDelegate(SyntaxAbstractor.PlainTextSyntaxHandler),
    ["Strings/TV/TipChannel"] = new ExtractSyntaxDelegate(SyntaxAbstractor.PlainTextSyntaxHandler),
    ["Strings/UI"] = new ExtractSyntaxDelegate(SyntaxAbstractor.PlainTextSyntaxHandler),
    ["Strings/Weapons"] = new ExtractSyntaxDelegate(SyntaxAbstractor.PlainTextSyntaxHandler),
    ["Strings/WorldMap"] = new ExtractSyntaxDelegate(SyntaxAbstractor.PlainTextSyntaxHandler),
    ["Data/Events/*"] = new ExtractSyntaxDelegate(SyntaxAbstractor.EventSyntaxHandler),
    ["Data/Festivals/*"] = new ExtractSyntaxDelegate(SyntaxAbstractor.FestivalSyntaxHandler),
    ["Data/Achievements"] = (ExtractSyntaxDelegate) ((syntaxBuilder, _, key, text) => syntaxBuilder.ExtractDelimitedDataSyntax(text, '^', 0, 1)),
    ["Data/Boots"] = (ExtractSyntaxDelegate) ((syntaxBuilder, _, key, text) => syntaxBuilder.ExtractDelimitedDataSyntax(text, '/', 1, 6)),
    ["Data/Bundles"] = (ExtractSyntaxDelegate) ((syntaxBuilder, _, key, text) => syntaxBuilder.ExtractDelimitedDataSyntax(text, '/', 6)),
    ["Data/hats"] = (ExtractSyntaxDelegate) ((syntaxBuilder, _, key, text) => syntaxBuilder.ExtractDelimitedDataSyntax(text, '/', 1, 5)),
    ["Data/Monsters"] = (ExtractSyntaxDelegate) ((syntaxBuilder, _, key, text) => syntaxBuilder.ExtractDelimitedDataSyntax(text, '/', 14)),
    ["Data/NPCGiftTastes"] = (ExtractSyntaxDelegate) ((syntaxBuilder, _, key, text) =>
    {
      if (key.StartsWith("Universal_"))
        return text;
      return syntaxBuilder.ExtractDelimitedDataSyntax(text, '/', 0, 2, 4, 6, 8);
    }),
    ["Data/Quests"] = (ExtractSyntaxDelegate) ((syntaxBuilder, _, key, text) => syntaxBuilder.ExtractDelimitedDataSyntax(text, '/', new int[3]
    {
      1,
      2,
      3
    }, new int[1]{ 9 })),
    ["Data/TV/CookingChannel"] = (ExtractSyntaxDelegate) ((syntaxBuilder, _, key, text) => syntaxBuilder.ExtractDelimitedDataSyntax(text, '/', 1)),
    ["Data/mail"] = (ExtractSyntaxDelegate) ((syntaxBuilder, _, key, text) => syntaxBuilder.ExtractMailSyntax(text)),
    ["Data/Notes"] = (ExtractSyntaxDelegate) ((syntaxBuilder, _, key, text) => syntaxBuilder.ExtractMailSyntax(text)),
    ["Data/SecretNotes"] = (ExtractSyntaxDelegate) ((syntaxBuilder, _, key, text) => syntaxBuilder.ExtractMailSyntax(text)),
    ["Strings/credits"] = (ExtractSyntaxDelegate) ((syntaxBuilder, _, key, text) => syntaxBuilder.ExtractCreditsSyntax(text)),
    ["Strings/1_6_Strings"] = (ExtractSyntaxDelegate) ((syntaxBuilder, _, key, text) => syntaxBuilder.Extract16StringsSyntax(key, text)),
    ["Strings/Lexicon"] = (ExtractSyntaxDelegate) ((syntaxBuilder, _, key, text) => syntaxBuilder.ExtractLexiconSyntax(key, text))
  };

  /// <summary>Get a handler which can extract syntactic representations for a given asset.</summary>
  /// <param name="baseAssetName">The asset name without the locale suffix, like <c>Data/Achievements</c>.</param>
  /// <remarks>Most code should use <see cref="M:StardewValley.Tests.SyntaxAbstractor.ExtractSyntaxFor(System.String,System.String,System.String)" /> or a specific method like <see cref="M:StardewValley.Tests.SyntaxAbstractor.ExtractDialogueSyntax(System.String)" /> instead.</remarks>
  public ExtractSyntaxDelegate GetSyntaxHandler(string baseAssetName)
  {
    ExtractSyntaxDelegate syntaxHandler;
    if (this.SyntaxHandlers.TryGetValue(baseAssetName, out syntaxHandler))
      return syntaxHandler;
    int length = baseAssetName.LastIndexOf('/');
    return length != -1 && this.SyntaxHandlers.TryGetValue(baseAssetName.Substring(0, length) + "/*", out syntaxHandler) ? syntaxHandler : (ExtractSyntaxDelegate) null;
  }

  /// <summary>Get a syntactic representation of an arbitrary asset entry, if it's a known asset.</summary>
  /// <param name="baseAssetName">The asset name without the locale suffix, like <c>Data/Achievements</c>.</param>
  /// <param name="key">The key within the asset for the text value.</param>
  /// <param name="value">The text to represent.</param>
  public string ExtractSyntaxFor(string baseAssetName, string key, string value)
  {
    if (value.Contains("${"))
      value = Regex.Replace(value, "\\$\\{.+?\\}\\$", "text");
    ExtractSyntaxDelegate syntaxHandler = this.GetSyntaxHandler(baseAssetName);
    return (syntaxHandler != null ? syntaxHandler(this, baseAssetName, key, value) : (string) null) ?? value;
  }

  /// <summary>Get a syntactic representation of plain text which has no special syntax.</summary>
  /// <param name="value">The text to represent.</param>
  public string ExtractPlainTextSyntax(string value)
  {
    return !string.IsNullOrWhiteSpace(value) ? "text" : string.Empty;
  }

  /// <summary>Get a syntactic representation of a dialogue string.</summary>
  /// <param name="value">The text to represent.</param>
  /// <remarks>This handles the general syntax format. For asset-specific formats, see <see cref="M:StardewValley.Tests.SyntaxAbstractor.ExtractDialogueSyntax(System.String,System.String,System.String)" /> instead.</remarks>
  public string ExtractDialogueSyntax(string value)
  {
    StringBuilder syntax = new StringBuilder();
    int index = 0;
    this.ExtractDialogueSyntaxImpl(value, '#', ref index, syntax);
    return syntax.ToString();
  }

  /// <summary>Get a syntactic representation of a dialogue string.</summary>
  /// <param name="baseAssetName">The asset name without the locale suffix, like <c>Data/Achievements</c>.</param>
  /// <param name="key">The key within the asset for the text value.</param>
  /// <param name="value">The text to represent.</param>
  /// <remarks>This supports asset-specific dialogue formats. In particular, some translations are loaded via <see cref="M:StardewValley.Game1.LoadStringByGender(StardewValley.Gender,System.String)" /> which supports a special <c>male/female</c> format based on the NPC's gender (not the player's gender).</remarks>
  public string ExtractDialogueSyntax(string baseAssetName, string key, string value)
  {
    switch (baseAssetName)
    {
      case "Data/ExtraDialogue":
        if (key == "NewChild_Adoption" || key == "NewChild_FirstChild" || key == "NewChild_SecondChild1" || key == "NewChild_SecondChild2")
          return this.ExtractNpcGenderedDialogueSyntax(value);
        break;
      case "Strings/Locations":
        if (key == "FarmHouse_SpouseAttacked3")
          return "text";
        break;
      case "Strings/StringsFromCSFiles":
        switch (key)
        {
          case "Event.cs.1497":
          case "Event.cs.1498":
          case "Event.cs.1499":
          case "Event.cs.1500":
          case "Event.cs.1501":
          case "Event.cs.1504":
          case "NPC.cs.3957":
          case "NPC.cs.3959":
          case "NPC.cs.3962":
          case "NPC.cs.3963":
          case "NPC.cs.3965":
          case "NPC.cs.3966":
          case "NPC.cs.3968":
          case "NPC.cs.3974":
          case "NPC.cs.3975":
          case "NPC.cs.4079":
          case "NPC.cs.4080":
          case "NPC.cs.4088":
          case "NPC.cs.4089":
          case "NPC.cs.4091":
          case "NPC.cs.4113":
          case "NPC.cs.4115":
          case "NPC.cs.4141":
          case "NPC.cs.4144":
          case "NPC.cs.4146":
          case "NPC.cs.4147":
          case "NPC.cs.4149":
          case "NPC.cs.4152":
          case "NPC.cs.4153":
          case "NPC.cs.4154":
          case "NPC.cs.4274":
          case "NPC.cs.4276":
          case "NPC.cs.4277":
          case "NPC.cs.4278":
          case "NPC.cs.4279":
          case "NPC.cs.4293":
          case "NPC.cs.4422":
          case "NPC.cs.4446":
          case "NPC.cs.4447":
          case "NPC.cs.4449":
          case "NPC.cs.4452":
          case "NPC.cs.4455":
          case "NPC.cs.4462":
          case "NPC.cs.4470":
          case "NPC.cs.4474":
          case "NPC.cs.4481":
          case "NPC.cs.4488":
          case "NPC.cs.4498":
          case "NPC.cs.4500":
            return this.ExtractNpcGenderedDialogueSyntax(value);
          case "OptionsPage.cs.11289":
          case "OptionsPage.cs.11290":
          case "OptionsPage.cs.11291":
          case "OptionsPage.cs.11292":
          case "OptionsPage.cs.11293":
          case "OptionsPage.cs.11294":
          case "OptionsPage.cs.11295":
          case "OptionsPage.cs.11296":
          case "OptionsPage.cs.11297":
          case "OptionsPage.cs.11298":
          case "OptionsPage.cs.11299":
          case "OptionsPage.cs.11300":
            if (!string.IsNullOrWhiteSpace(value))
              return "text";
            break;
          case "Pipe":
            return "text";
        }
        break;
    }
    return this.ExtractDialogueSyntax(value);
  }

  /// <summary>Get a syntactic representation of a dialogue string.</summary>
  /// <param name="value">The text to represent.</param>
  public string ExtractEventSyntax(string value)
  {
    StringBuilder syntax = new StringBuilder();
    int index = 0;
    this.ExtractEventSyntaxImpl(value, ref index, syntax);
    return syntax.ToString();
  }

  /// <summary>Get a syntactic representation of a festival string.</summary>
  /// <param name="baseAssetName">The asset name without the locale suffix, like <c>Data/Achievements</c>.</param>
  /// <param name="key">The key within the asset for the text value.</param>
  /// <param name="value">The text to represent.</param>
  public string ExtractFestivalSyntax(string baseAssetName, string key, string value)
  {
    if (key != null)
    {
      switch (key.Length)
      {
        case 6:
          if (key == "set-up")
            break;
          goto label_25;
        case 7:
          if (key == "AbbyWin")
            goto label_21;
          goto label_25;
        case 9:
          switch (key[0])
          {
            case 'm':
              if (key == "mainEvent")
                break;
              goto label_25;
            case 's':
              if (key == "set-up_y2")
                break;
              goto label_25;
            default:
              goto label_25;
          }
          break;
        case 10:
          if (key == "conditions")
            break;
          goto label_25;
        case 12:
          switch (key[0])
          {
            case 'a':
              if (key == "afterEggHunt")
                goto label_21;
              goto label_25;
            case 'm':
              if (key == "mainEvent_y2")
                break;
              goto label_25;
            default:
              goto label_25;
          }
          break;
        case 15:
          if (key == "afterEggHunt_y2")
            goto label_21;
          goto label_25;
        case 17:
          switch (key[16 /*0x10*/])
          {
            case '0':
              if (key == "governorReaction0")
                break;
              goto label_25;
            case '1':
              if (key == "governorReaction1")
                break;
              goto label_25;
            case '2':
              if (key == "governorReaction2")
                break;
              goto label_25;
            case '3':
              if (key == "governorReaction3")
                break;
              goto label_25;
            case '4':
              if (key == "governorReaction4")
                break;
              goto label_25;
            case '5':
              if (key == "governorReaction5")
                break;
              goto label_25;
            case '6':
              if (key == "governorReaction6")
                break;
              goto label_25;
            default:
              goto label_25;
          }
          if (baseAssetName == "Data/Festivals/summer11")
            return this.ExtractEventSyntax(value);
          goto label_25;
        default:
          goto label_25;
      }
      return this.ExtractEventSyntax(value);
label_21:
      if (baseAssetName == "Data/Festivals/spring13")
        return this.ExtractEventSyntax(value);
    }
label_25:
    return this.ExtractDialogueSyntax(value);
  }

  /// <summary>Get a syntactic representation of a <c>Strings/credits</c> entry.</summary>
  /// <param name="text">The text to represent.</param>
  /// <remarks>See parsing logic in <see cref="M:StardewValley.Menus.AboutMenu.SetUpCredits" />.</remarks>
  public string ExtractCreditsSyntax(string text)
  {
    if (text.Length == 0)
      return text;
    if (text.StartsWith('['))
    {
      if (text.StartsWith("[image]"))
        return text;
      if (text.StartsWith("[link]"))
      {
        string[] strArray = text.Split(' ', 3);
        strArray[2] = nameof (text);
        return string.Join(" ", strArray);
      }
    }
    StringBuilder syntax = new StringBuilder();
    int index = 0;
    bool hasText = false;
    for (; index < text.Length; ++index)
    {
      if (text[index] == '[')
      {
        this.EndTextContext(ref hasText, syntax);
        this.ExtractTagSyntax(text, ref index, syntax);
      }
      else
        hasText = true;
    }
    this.EndTextContext(ref hasText, syntax);
    return syntax.ToString();
  }

  /// <summary>Get a syntactic representation of a mail string.</summary>
  /// <param name="text">The text to represent.</param>
  /// <remarks>This handles the general syntax format. For asset-specific formats, see <see cref="M:StardewValley.Tests.SyntaxAbstractor.ExtractDialogueSyntax(System.String,System.String,System.String)" /> instead.</remarks>
  public string ExtractMailSyntax(string text)
  {
    text = text.Replace("%secretsanta", nameof (text));
    StringBuilder syntax = new StringBuilder();
    int index = 0;
    bool hasText = false;
    for (; index < text.Length; ++index)
    {
      char ch = text[index];
      switch (ch)
      {
        case ' ':
          continue;
        case '%':
          if (index >= text.Length || char.IsWhiteSpace(text[index + 1]) || char.IsDigit(text[index + 1]))
          {
            hasText = true;
            continue;
          }
          this.EndTextContext(ref hasText, syntax);
          this.ExtractMailCommandSyntax(text, ref index, syntax);
          continue;
        case '[':
          this.EndTextContext(ref hasText, syntax);
          this.ExtractTagSyntax(text, ref index, syntax);
          continue;
        case '¦':
          this.EndTextContext(ref hasText, syntax);
          syntax.Append(ch);
          continue;
        default:
          if (!hasText)
          {
            hasText = true;
            continue;
          }
          continue;
      }
    }
    this.EndTextContext(ref hasText, syntax);
    return syntax.ToString();
  }

  /// <summary>Get a syntactic representation of a data entry containing delimited fields.</summary>
  /// <param name="text">The dialogue entry.</param>
  /// <param name="delimiter">The delimiter between fields.</param>
  /// <param name="textFields">The field indices containing localized text, which should be replaced by <see cref="F:StardewValley.Tests.SyntaxAbstractor.TextMarker" />.</param>
  public string ExtractDelimitedDataSyntax(string text, char delimiter, params int[] textFields)
  {
    return this.ExtractDelimitedDataSyntax(text, delimiter, textFields, (int[]) null);
  }

  /// <summary>Get a syntactic representation of a data entry containing delimited fields.</summary>
  /// <param name="text">The dialogue entry.</param>
  /// <param name="delimiter">The delimiter between fields.</param>
  /// <param name="textFields">The field indices containing localized text, which should be replaced by <see cref="F:StardewValley.Tests.SyntaxAbstractor.TextMarker" />.</param>
  /// <param name="dialogueFields">The field indices containing dialogue text.</param>
  public string ExtractDelimitedDataSyntax(
    string text,
    char delimiter,
    int[] textFields,
    int[] dialogueFields)
  {
    string[] array = text.Split(delimiter);
    foreach (int textField in textFields)
    {
      if (ArgUtility.HasIndex<string>(array, textField))
        array[textField] = nameof (text);
    }
    if (dialogueFields != null)
    {
      foreach (int dialogueField in dialogueFields)
      {
        if (ArgUtility.HasIndex<string>(array, dialogueField))
          array[dialogueField] = this.ExtractDialogueSyntax(array[dialogueField]);
      }
    }
    return string.Join(delimiter.ToString(), array);
  }

  /// <summary>Get a syntactic representation of a string from <c>Strings/1_6_Strings</c>.</summary>
  /// <param name="key">The key within the asset for the text value.</param>
  /// <param name="text">The text to represent.</param>
  public string Extract16StringsSyntax(string key, string text)
  {
    if (key.StartsWith("Renovation_"))
      return this.ExtractDelimitedDataSyntax(text, '/', LegacyShims.EmptyArray<int>(), new int[3]
      {
        0,
        1,
        2
      });
    switch (key)
    {
      case "ForestPylonEvent":
        return this.ExtractEventSyntax(text);
      case "StarterChicken_Names":
        string[] strArray = text.Split('|');
        StringBuilder stringBuilder1 = new StringBuilder();
        bool flag = false;
        foreach (string str in strArray)
        {
          if (str.Split(',', 3).Length == 2)
          {
            if (stringBuilder1.Length == 0)
              stringBuilder1.Append("name,name");
            else
              flag = true;
          }
          else
          {
            if (stringBuilder1.Length > 0)
              stringBuilder1.Append(" | ");
            StringBuilder stringBuilder2 = stringBuilder1;
            StringBuilder stringBuilder3 = stringBuilder2;
            StringBuilder.AppendInterpolatedStringHandler interpolatedStringHandler = new StringBuilder.AppendInterpolatedStringHandler(16 /*0x10*/, 1, stringBuilder2);
            interpolatedStringHandler.AppendLiteral("<invalid pair: ");
            interpolatedStringHandler.AppendFormatted(str.Trim());
            interpolatedStringHandler.AppendLiteral(">");
            ref StringBuilder.AppendInterpolatedStringHandler local = ref interpolatedStringHandler;
            stringBuilder3.Append(ref local);
          }
        }
        if (flag)
          return $"{stringBuilder1} | ...";
        return stringBuilder1.Length > 0 ? stringBuilder1.ToString() : string.Empty;
      default:
        return this.ExtractDialogueSyntax(text);
    }
  }

  /// <summary>Get a syntactic representation of a string from <c>Strings/Lexicon</c>.</summary>
  /// <param name="key">The key within the asset for the text value.</param>
  /// <param name="text">The text to represent.</param>
  public string ExtractLexiconSyntax(string key, string text)
  {
    string[] strArray = text.Split('#');
    for (int index = 0; index < strArray.Length; ++index)
    {
      if (!string.IsNullOrWhiteSpace(strArray[index]))
      {
        string str = strArray[index];
        int totalWidth1 = str.Length - str.TrimStart().Length;
        int totalWidth2 = str.Length - str.TrimEnd().Length;
        strArray[index] = totalWidth1 > 0 || totalWidth2 > 0 ? $"{"".PadRight(totalWidth1)}text{"".PadRight(totalWidth2)}" : nameof (text);
      }
    }
    return key.StartsWith("Random") && strArray.Length > 2 ? $"{strArray[0]}#{strArray[1]}#..." : string.Join("#", strArray);
  }

  /// <summary>A shortcut for calling <see cref="M:StardewValley.Tests.SyntaxAbstractor.ExtractDialogueSyntax(System.String,System.String,System.String)" /> in <see cref="F:StardewValley.Tests.SyntaxAbstractor.SyntaxHandlers" />.</summary>
  /// <inheritdoc cref="T:StardewValley.Tests.ExtractSyntaxDelegate" />
  private static string DialogueSyntaxHandler(
    SyntaxAbstractor syntaxAbstractor,
    string baseAssetName,
    string key,
    string text)
  {
    return syntaxAbstractor.ExtractDialogueSyntax(baseAssetName, key, text);
  }

  /// <summary>A shortcut for calling <see cref="M:StardewValley.Tests.SyntaxAbstractor.ExtractPlainTextSyntax(System.String)" /> in <see cref="F:StardewValley.Tests.SyntaxAbstractor.SyntaxHandlers" />.</summary>
  /// <inheritdoc cref="T:StardewValley.Tests.ExtractSyntaxDelegate" />
  private static string PlainTextSyntaxHandler(
    SyntaxAbstractor syntaxAbstractor,
    string baseAssetName,
    string key,
    string text)
  {
    return syntaxAbstractor.ExtractPlainTextSyntax(text);
  }

  /// <summary>A shortcut for calling <see cref="M:StardewValley.Tests.SyntaxAbstractor.ExtractEventSyntax(System.String)" /> in <see cref="F:StardewValley.Tests.SyntaxAbstractor.SyntaxHandlers" />.</summary>
  /// <inheritdoc cref="T:StardewValley.Tests.ExtractSyntaxDelegate" />
  private static string EventSyntaxHandler(
    SyntaxAbstractor syntaxAbstractor,
    string baseAssetName,
    string key,
    string text)
  {
    return syntaxAbstractor.ExtractEventSyntax(text);
  }

  /// <summary>A shortcut for calling <see cref="M:StardewValley.Tests.SyntaxAbstractor.ExtractFestivalSyntax(System.String,System.String,System.String)" /> in <see cref="F:StardewValley.Tests.SyntaxAbstractor.SyntaxHandlers" />.</summary>
  /// <inheritdoc cref="T:StardewValley.Tests.ExtractSyntaxDelegate" />
  private static string FestivalSyntaxHandler(
    SyntaxAbstractor syntaxAbstractor,
    string baseAssetName,
    string key,
    string text)
  {
    return syntaxAbstractor.ExtractFestivalSyntax(baseAssetName, key, text);
  }

  /// <summary>As part of <see cref="M:StardewValley.Tests.SyntaxAbstractor.ExtractSyntaxFor(System.String,System.String,System.String)" />, read a syntax representation of an event script.</summary>
  /// <param name="text">The event script.</param>
  /// <param name="index">The index at which to read the next character. After this method runs, it will be set to the last character in the event script.</param>
  /// <param name="syntax">The string builder to extend with the current command's syntax.</param>
  /// <param name="maxIndex">If set, the index at which to stop reading the string.</param>
  private void ExtractEventSyntaxImpl(
    string text,
    ref int index,
    StringBuilder syntax,
    int maxIndex = -1)
  {
    string[] strArray1 = ArgUtility.SplitQuoteAware(index != 0 || maxIndex >= 0 ? text.Substring(index, maxIndex - index + 1) : text, '/', StringSplitOptions.TrimEntries, true);
    bool flag = true;
    foreach (string input in strArray1)
    {
      if (!flag)
        syntax.Append('/');
      if (!string.IsNullOrWhiteSpace(input))
      {
        string[] strArray2 = ArgUtility.SplitBySpaceQuoteAware(input);
        string name = strArray2[0];
        syntax.Append(name);
        int index1 = 1;
        string actualName;
        if (Event.TryResolveCommandName(name, out actualName) && actualName != null)
        {
          switch (actualName.Length)
          {
            case 3:
              if (actualName == "End")
              {
                string str = ArgUtility.Get(strArray2, 1);
                if (str == "dialogue" || str == "dialogueWarpOut")
                {
                  this.AppendEventCommandArg(syntax, strArray2, 1);
                  this.AppendEventCommandArg(syntax, strArray2, 2);
                  this.AppendEventCommandDialogueArg(syntax, strArray2, 3);
                  index1 = 4;
                  break;
                }
                break;
              }
              break;
            case 5:
              if (actualName == "Speak")
              {
                this.AppendEventCommandArg(syntax, strArray2, 1);
                this.AppendEventCommandDialogueArg(syntax, strArray2, 2);
                index1 = 3;
                break;
              }
              break;
            case 7:
              if (actualName == "Message")
              {
                this.AppendEventCommandDialogueArg(syntax, strArray2, 1);
                index1 = 2;
                break;
              }
              break;
            case 8:
              if (actualName == "Question")
              {
                this.AppendEventCommandArg(syntax, strArray2, 1);
                this.AppendEventCommandDialogueArg(syntax, strArray2, 2);
                index1 = 3;
                break;
              }
              break;
            case 10:
              switch (actualName[2])
              {
                case 'l':
                  if (actualName == "SplitSpeak")
                  {
                    string[] args = ArgUtility.Get(strArray2, 2)?.Split('~');
                    this.AppendEventCommandArg(syntax, strArray2, 1);
                    if (args != null)
                    {
                      syntax.Append(" \"");
                      for (int index2 = 0; index2 < args.Length; ++index2)
                      {
                        if (index2 > 0)
                          syntax.Append('~');
                        this.AppendEventCommandDialogueArg(syntax, args, index2, false, false);
                      }
                      syntax.Append('"');
                    }
                    index1 = 3;
                    break;
                  }
                  break;
                case 'r':
                  if (actualName == "SpriteText")
                  {
                    this.AppendEventCommandArg(syntax, strArray2, 1);
                    this.AppendEventCommandDialogueArg(syntax, strArray2, 2);
                    index1 = 3;
                    break;
                  }
                  break;
              }
              break;
            case 13:
              switch (actualName[0])
              {
                case 'Q':
                  if (actualName == "QuickQuestion")
                  {
                    string[] strArray3 = LegacyShims.SplitAndTrim(input.Substring(input.IndexOf(' ')), "(break)");
                    string[] args = LegacyShims.SplitAndTrim(strArray3[0], '#');
                    syntax.Append(" \"");
                    this.AppendEventCommandDialogueArg(syntax, args, 0, quote: false);
                    for (int index3 = 1; index3 < args.Length; ++index3)
                    {
                      syntax.Append('#');
                      this.AppendEventCommandDialogueArg(syntax, args, index3, false, false);
                    }
                    for (int index4 = 1; index4 < strArray3.Length; ++index4)
                    {
                      strArray3[index4] = strArray3[index4].Replace('\\', '/');
                      syntax.Append("(break)");
                      int index5 = 0;
                      this.ExtractEventSyntaxImpl(strArray3[index4], ref index5, syntax);
                    }
                    syntax.Append('"');
                    index1 = strArray2.Length;
                    break;
                  }
                  break;
                case 'T':
                  if (actualName == "TextAboveHead")
                  {
                    this.AppendEventCommandArg(syntax, strArray2, 1);
                    this.AppendEventCommandDialogueArg(syntax, strArray2, 2);
                    index1 = 3;
                    break;
                  }
                  break;
              }
              break;
          }
        }
        for (; index1 < strArray2.Length; ++index1)
          this.AppendEventCommandArg(syntax, strArray2, index1);
      }
      flag = false;
    }
    index = maxIndex > 0 ? maxIndex : text.Length - 1;
  }

  /// <summary>Append an event command argument to a syntax string being built, including the preceding space.</summary>
  /// <param name="syntax">The syntax string being built.</param>
  /// <param name="args">The command arguments.</param>
  /// <param name="index">The index of the argument in <paramref name="args" /> to append.</param>
  /// <param name="prependSpace">Whether to prepend a space before the argument.</param>
  private void AppendEventCommandArg(
    StringBuilder syntax,
    string[] args,
    int index,
    bool prependSpace = true)
  {
    if (!ArgUtility.HasIndex<string>(args, index))
      return;
    string str = args[index];
    int num = str.Contains(' ') ? 1 : 0;
    if (prependSpace)
      syntax.Append(' ');
    if (num != 0)
      syntax.Append('"');
    syntax.Append(str);
    if (num == 0)
      return;
    syntax.Append('"');
  }

  /// <summary>Append an event command argument containing dialogue syntax to a syntax string being built, including the preceding space.</summary>
  /// <param name="syntax">The syntax string being built.</param>
  /// <param name="args">The command arguments.</param>
  /// <param name="index">The index of the argument in <paramref name="args" /> to append.</param>
  /// <param name="prependSpace">Whether to prepend a space before the argument.</param>
  /// <param name="quote">Whether to quote the dialogue string.</param>
  private void AppendEventCommandDialogueArg(
    StringBuilder syntax,
    string[] args,
    int index,
    bool prependSpace = true,
    bool quote = true)
  {
    if (!ArgUtility.HasIndex<string>(args, index))
      return;
    string text = args[index];
    int index1 = 0;
    if (prependSpace)
      syntax.Append(' ');
    if (quote)
      syntax.Append('"');
    this.ExtractDialogueSyntaxImpl(text, '/', ref index1, syntax);
    if (!quote)
      return;
    syntax.Append('"');
  }

  /// <summary>As part of <see cref="M:StardewValley.Tests.SyntaxAbstractor.ExtractDialogueSyntax(System.String,System.String,System.String)" />, extract the syntax for a dialogue which is NPC-gendered via <see cref="M:StardewValley.Game1.LoadStringByGender(StardewValley.Gender,System.String)" />.</summary>
  /// <param name="text">The dialogue entry.</param>
  private string ExtractNpcGenderedDialogueSyntax(string text)
  {
    if (!text.Contains('/'))
      return this.ExtractDialogueSyntax(text);
    string[] strArray = text.Split('/');
    for (int index = 0; index < strArray.Length; ++index)
      strArray[index] = this.ExtractDialogueSyntax(strArray[index]);
    return strArray.Length != 2 || !(strArray[0] == strArray[1]) ? string.Join("/", strArray) : strArray[0];
  }

  /// <summary>As part of <see cref="M:StardewValley.Tests.SyntaxAbstractor.ExtractSyntaxFor(System.String,System.String,System.String)" />, read a syntax representation of a dialogue entry.</summary>
  /// <param name="text">The dialogue entry.</param>
  /// <param name="commandDelimiter">Within the larger asset, the character which delimits commands. This is usually <c>#</c> for dialogue, or <c>/</c> for event data. This is used in certain specialized cases like <see cref="F:StardewValley.Dialogue.dialogueQuickResponse" />, which extends to the end of the command.</param>
  /// <param name="index">The index at which to read the next character. After this method runs, it will be set to the last character in the dialogue string.</param>
  /// <param name="syntax">The string builder to extend with the current command's syntax.</param>
  /// <param name="maxIndex">If set, the index at which to stop reading the string.</param>
  private void ExtractDialogueSyntaxImpl(
    string text,
    char commandDelimiter,
    ref int index,
    StringBuilder syntax,
    int maxIndex = -1)
  {
    bool hasText = false;
    bool flag = false;
    if (maxIndex < 0 || maxIndex > text.Length - 1)
      maxIndex = text.Length - 1;
    while (index <= maxIndex)
    {
      char ch = text[index];
      if (ch <= '$')
      {
        if (ch != '#' && ch != '$')
          goto label_14;
      }
      else if (ch != '[')
      {
        if (ch != ']')
        {
          if (ch != '|')
            goto label_14;
        }
        else
        {
          this.EndTextContext(ref hasText, syntax);
          syntax.Append(']');
          flag = false;
          goto label_17;
        }
      }
      else
      {
        this.EndTextContext(ref hasText, syntax);
        this.ExtractDialogueItemSpawnSyntax(text, ref index, syntax);
        flag = false;
        goto label_17;
      }
      if (ch == '$' & flag && !hasText)
        syntax.Append(nameof (text));
      this.EndTextContext(ref hasText, syntax);
      flag = false;
      if (ch == '$')
      {
        this.ExtractDialogueCommandSyntax(text, ref index, syntax, commandDelimiter);
        goto label_17;
      }
      syntax.Append(ch);
      goto label_17;
label_14:
      if (ch == ' ')
        flag = true;
      else
        hasText = true;
label_17:
      ++index;
    }
    this.EndTextContext(ref hasText, syntax);
  }

  /// <summary>As part of <see cref="M:StardewValley.Tests.SyntaxAbstractor.ExtractDialogueSyntaxImpl(System.String,System.Char,System.Int32@,System.Text.StringBuilder,System.Int32)" />, read a syntax representation of a single command from the input string.</summary>
  /// <param name="text">The dialogue or event text.</param>
  /// <param name="index">The index at which to read the next character. After this method runs, it will be set to the last character in the command.</param>
  /// <param name="syntax">The string builder to extend with the current command's syntax.</param>
  /// <param name="commandDelimiter">Within the larger asset, the character which delimits commands. This is usually <c>#</c> for dialogue, or <c>/</c> for event data. This is used in certain specialized cases like <see cref="F:StardewValley.Dialogue.dialogueQuickResponse" />, which extends to the end of the command.</param>
  private void ExtractDialogueCommandSyntax(
    string text,
    ref int index,
    StringBuilder syntax,
    char commandDelimiter)
  {
    int startIndex1 = index;
    ++index;
    while (index < text.Length && (char.IsLetter(text[index]) || char.IsNumber(text[index])))
      ++index;
    string str = text.Substring(startIndex1, index - startIndex1);
    syntax.Append(str);
    if (str != null)
    {
      switch (str.Length)
      {
        case 2:
          switch (str[1])
          {
            case '1':
              if (str == "$1")
                break;
              goto label_53;
            case 'c':
              if (str == "$c")
                break;
              goto label_53;
            case 'd':
              if (str == "$d")
                goto label_20;
              goto label_53;
            case 'p':
              if (str == "$p")
                goto label_20;
              goto label_53;
            case 'q':
              if (str == "$q")
                break;
              goto label_53;
            case 'r':
              if (str == "$r")
                break;
              goto label_53;
            case 't':
              if (str == "$t")
                break;
              goto label_53;
            case 'y':
              if (str == "$y")
              {
                int startIndex2 = index;
                while (index < text.Length && text[index] == ' ')
                  ++index;
                if (text[index] == '\'')
                {
                  ++index;
                  syntax.Append(text.Substring(startIndex2, index - startIndex2).TrimEnd(' '));
                  int num1 = index;
                  int num2 = text.IndexOf(commandDelimiter, index);
                  if (num2 == -1)
                    num2 = text.Length;
                  while (true)
                  {
                    int num3 = text.IndexOf('\'', num1 + 1);
                    if (num3 != -1 && num3 <= num2)
                      num1 = num3;
                    else
                      break;
                  }
                  if (num1 <= index)
                    return;
                  bool flag = false;
                  while (index < num1 - 1)
                  {
                    char ch = text[index];
                    if (ch == '_')
                    {
                      if (flag)
                      {
                        syntax.Append(nameof (text));
                        flag = false;
                      }
                      syntax.Append(ch);
                    }
                    else
                      flag = true;
                    ++index;
                  }
                  if (flag)
                    syntax.Append(nameof (text));
                  ++index;
                  syntax.Append(text[index]);
                  ++index;
                  goto label_53;
                }
                index = startIndex2;
                return;
              }
              goto label_53;
            default:
              goto label_53;
          }
          int startIndex3 = index;
          while (index < text.Length && text[index] != '#')
            ++index;
          syntax.Append(text.Substring(startIndex3, index - startIndex3).TrimEnd(' '));
          goto label_53;
        case 6:
          if (str == "$query")
            break;
          goto label_53;
        default:
          goto label_53;
      }
label_20:
      int startIndex4 = index;
      while (index < text.Length && text[index] != '#')
        ++index;
      ++index;
      syntax.Append(text.Substring(startIndex4, index - startIndex4).TrimEnd(' '));
      int index1 = index;
      while (index1 < text.Length && text[index1] != '#' && text[index1] != '|')
        ++index1;
      this.ExtractDialogueSyntaxImpl(text, commandDelimiter, ref index, syntax, index1 - 1);
      if (index >= text.Length || text[index] != '|')
        return;
      syntax.Append(text[index]);
      ++index;
      int index2 = index;
      while (index2 < text.Length && text[index2] != '#' && text[index2] != '|')
        ++index2;
      this.ExtractDialogueSyntaxImpl(text, commandDelimiter, ref index, syntax, index2 - 1);
    }
label_53:
    --index;
  }

  /// <summary>As part of <see cref="M:StardewValley.Tests.SyntaxAbstractor.ExtractDialogueSyntaxImpl(System.String,System.Char,System.Int32@,System.Text.StringBuilder,System.Int32)" />, read a syntax representation of an item spawn list like <c>[128 129]</c>.</summary>
  /// <param name="text">The dialogue or event text.</param>
  /// <param name="index">The index at which to read the next character. After this method runs, it will be set to the last character in the command.</param>
  /// <param name="syntax">The string builder to extend with the current command's syntax.</param>
  private void ExtractDialogueItemSpawnSyntax(string text, ref int index, StringBuilder syntax)
  {
    int startIndex = index;
    int index1 = index + 1;
    bool flag = false;
    for (; index1 < text.Length; ++index1)
    {
      char c = text[index1];
      switch (c)
      {
        case ' ':
        case '.':
          continue;
        default:
          if (!char.IsLetter(c) && !char.IsNumber(c))
          {
            if (c == ']')
            {
              flag = true;
              goto label_7;
            }
            goto label_7;
          }
          continue;
      }
    }
label_7:
    if (flag)
    {
      syntax.Append(text.Substring(startIndex, index1 - startIndex + 1).TrimEnd(' '));
      index = index1;
    }
    else
    {
      syntax.Append(text[index]);
      ++index;
    }
  }

  /// <summary>As part of <see cref="M:StardewValley.Tests.SyntaxAbstractor.ExtractMailSyntax(System.String)" />, read a syntax representation of a single <c>%</c> mail command from the input string.</summary>
  /// <param name="text">The dialogue or event text.</param>
  /// <param name="index">The index at which to read the next character. After this method runs, it will be set to the last character in the command.</param>
  /// <param name="syntax">The string builder to extend with the current command's syntax.</param>
  private void ExtractMailCommandSyntax(string text, ref int index, StringBuilder syntax)
  {
    int startIndex1 = index;
    ++index;
    while (index < text.Length && (char.IsLetter(text[index]) || char.IsNumber(text[index])))
      ++index;
    string str1 = text.Substring(startIndex1, index - startIndex1);
    switch (str1)
    {
      case "%item":
        syntax.Append(str1);
        int startIndex2 = index;
        while (index < text.Length)
        {
          ++index;
          if (index > 1 && text[index] == '%' && text[index - 1] == '%')
            break;
        }
        string str2 = text[index] != '%' || text[index - 1] != '%' || !char.IsWhiteSpace(text[index - 2]) ? text.Substring(startIndex2, index - startIndex2 + 1) : text.Substring(startIndex2, index - startIndex2 - 1).TrimEnd() + "%%";
        syntax.Append(str2);
        break;
      case "%revealtaste":
        index -= "%revealtaste".Length;
        this.ExtractRevealTasteCommandSyntax(text, ref index, syntax);
        break;
      default:
        syntax.Append(str1);
        --index;
        break;
    }
  }

  /// <summary>As part of <see cref="M:StardewValley.Tests.SyntaxAbstractor.ExtractMailSyntax(System.String)" /> or <see cref="M:StardewValley.Tests.SyntaxAbstractor.ExtractCreditsSyntax(System.String)" />, read a syntax representation of a single <c>[...]</c> tag from the input string.</summary>
  /// <param name="text">The dialogue or event text.</param>
  /// <param name="index">The index at which to read the next character. After this method runs, it will be set to the last character in the command.</param>
  /// <param name="syntax">The string builder to extend with the current command's syntax.</param>
  private void ExtractTagSyntax(string text, ref int index, StringBuilder syntax)
  {
    int startIndex = index;
    ++index;
    while (index < text.Length - 1 && text[index] != ']')
      ++index;
    syntax.Append(text.Substring(startIndex, index - startIndex + 1));
  }

  /// <summary>As part of <see cref="M:StardewValley.Tests.SyntaxAbstractor.ExtractMailSyntax(System.String)" />, read a syntax representation of a single <c>%</c> mail command from the input string.</summary>
  /// <param name="text">The dialogue or event text.</param>
  /// <param name="index">The index at which to read the next character. After this method runs, it will be set to the last character in the command.</param>
  /// <param name="syntax">The string builder to extend with the current command's syntax.</param>
  /// <remarks>Derived from <see cref="M:StardewValley.Utility.ParseGiftReveals(System.String)" />.</remarks>
  private void ExtractRevealTasteCommandSyntax(string text, ref int index, StringBuilder syntax)
  {
    int startIndex = index;
    while (index < text.Length - 1)
    {
      char c = text[index + 1];
      if (!char.IsWhiteSpace(c) && c != '#' && c != '%' && c != '$' && c != '{' && c != '^' && c != '*' && c != '[')
        ++index;
      else
        break;
    }
    syntax.Append(text.Substring(startIndex, index - startIndex + 1));
  }

  /// <summary>If we're in the text portion of a dialogue/data string, output a <c>text</c> token now and end the text portion.</summary>
  /// <param name="hasText">Whether we're in a text portion of the input string. This will be set to false.</param>
  /// <param name="syntax">The syntax string being compiled.</param>
  private void EndTextContext(ref bool hasText, StringBuilder syntax)
  {
    if (!hasText)
      return;
    syntax.Append("text");
    hasText = false;
  }
}
