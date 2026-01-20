// Decompiled with JetBrains decompiler
// Type: StardewValley.TokenizableStrings.TokenParser
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework.Content;
using StardewValley.BellsAndWhistles;
using StardewValley.Extensions;
using StardewValley.GameData.Movies;
using StardewValley.GameData.SpecialOrders;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Locations;
using StardewValley.SpecialOrders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

#nullable disable
namespace StardewValley.TokenizableStrings;

/// <summary>Parses text containing tokens like "<c>It's a nice [Season] day</c>" into the resulting display text.</summary>
public class TokenParser
{
  /// <summary>The supported tokens and their resolvers.</summary>
  private static readonly Dictionary<string, TokenParserDelegate> Parsers = new Dictionary<string, TokenParserDelegate>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
  /// <summary>The character used to escape spaces in token arguments.</summary>
  private const char EscapedSpace = ' ';
  /// <summary>The character used to escape an empty argument.</summary>
  private const char EscapedEmpty = '\u200B';
  /// <summary>The character used to escape an empty argument.</summary>
  private static readonly string EscapedEmptyStr = '\u200B'.ToString();
  /// <summary>The character used to start a token.</summary>
  internal const char StartTokenChar = '[';
  /// <summary>The character used to end a token.</summary>
  internal const char EndTokenChar = ']';
  /// <summary>The characters which, when present in a tokenizable string, indicate that the string should be wrapped in [EscapedText] when used as an argument.</summary>
  internal static readonly char[] HeuristicCharactersForEscapableStrings = new char[2]
  {
    ' ',
    '['
  };

  /// <summary>Register the default game state queries, defined as <see cref="T:StardewValley.TokenizableStrings.TokenParser.DefaultResolvers" /> methods.</summary>
  static TokenParser()
  {
    foreach (MethodInfo method in typeof (TokenParser.DefaultResolvers).GetMethods(BindingFlags.Static | BindingFlags.Public))
    {
      TokenParserDelegate tokenParserDelegate = (TokenParserDelegate) Delegate.CreateDelegate(typeof (TokenParserDelegate), method);
      TokenParser.Parsers[method.Name] = tokenParserDelegate;
    }
  }

  /// <summary>Register a custom token parser.</summary>
  /// <param name="tokenKey">The token key. This should only contain alphanumeric, underscore, and dot characters. For custom queries, this should be prefixed with your mod ID like <c>Example.ModId_TokenName</c>.</param>
  /// <param name="parser">The parses which returns the text to use for a given token tag.</param>
  public static void RegisterParser(string tokenKey, TokenParserDelegate parser)
  {
    if (string.IsNullOrWhiteSpace(tokenKey))
      throw new ArgumentException("The token key can't be empty.", nameof (tokenKey));
    if (parser == null)
      throw new ArgumentException($"The parser callback for token key '{tokenKey}' can't be null.", nameof (parser));
    tokenKey = tokenKey.Trim();
    if (!TokenParser.Parsers.TryAdd(tokenKey, parser))
      throw new ArgumentException($"Can't add token parser for key '{tokenKey}' because one is already registered for it.");
  }

  /// <summary>Escape spaces within a tokenized string so it can be passed as an argument to tokens. The characters will automatically be converted back into spaces when parsed.</summary>
  /// <param name="text">The text to modify.</param>
  public static string EscapeSpaces(string text)
  {
    return text.Length <= 0 ? TokenParser.EscapedEmptyStr : text.Replace(' ', ' ');
  }

  /// <summary>Parse text containing tokens like "<c>It's a nice [Season] day</c>" into the resulting display text.</summary>
  /// <param name="text">The text to parse.</param>
  /// <param name="random">The RNG to use for randomization, or <c>null</c> to use <see cref="F:StardewValley.Game1.random" />.</param>
  /// <param name="customParser">A custom token parser which will be given an opportunity to parse each token first, if any.</param>
  /// <param name="player">The player to use for any player-related checks, or <c>null</c> to use <see cref="P:StardewValley.Game1.player" />.</param>
  /// <returns>Returns the modified text.</returns>
  public static string ParseText(
    string text,
    Random random = null,
    TokenParserDelegate customParser = null,
    Farmer player = null)
  {
    if (text == null)
      return (string) null;
    int num = text.IndexOf('[');
    if (num == -1)
      return text;
    for (int index = num; index < text.Length; ++index)
    {
      if (text[index] == '[')
        index = TokenParser.ParseTagStartingAt(ref text, index, random ?? Game1.random, customParser, player ?? Game1.player);
    }
    return TokenParser.UnescapeText(text.Replace("\\n", "\n"));
  }

  /// <summary>Log an error indicating that a token could not be parsed.</summary>
  /// <param name="query">The full token string split by spaces, including the token name.</param>
  /// <param name="error">The error indicating why parsing failed.</param>
  /// <param name="replacement">The replacement value to set.</param>
  /// <returns>Returns <c>false</c> for convenience.</returns>
  public static bool LogTokenError(string[] query, string error, out string replacement)
  {
    Game1.log.Error($"Failed parsing [{string.Join(" ", query)}]: {error}.");
    replacement = (string) null;
    return false;
  }

  /// <summary>Log an error indicating that a token could not be parsed.</summary>
  /// <param name="query">The full token string split by spaces, including the token name.</param>
  /// <param name="error">The error indicating why parsing failed.</param>
  /// <param name="replacement">The replacement value to set.</param>
  /// <returns>Returns <c>false</c> for convenience.</returns>
  public static bool LogTokenError(string[] query, Exception error, out string replacement)
  {
    Game1.log.Error($"Failed parsing [{string.Join(" ", query)}].", error);
    replacement = (string) null;
    return false;
  }

  /// <summary>Parse a tag within a text starting at the given index.</summary>
  /// <param name="text">The full text being parsed.</param>
  /// <param name="startIndex">The index at which the token appears, including the <see cref="F:StardewValley.TokenizableStrings.TokenParser.StartTokenChar" />.</param>
  /// <param name="random">The RNG to use for randomization.</param>
  /// <param name="customParser">A custom token parser which will be given an opportunity to parse each token first, if any.</param>
  /// <param name="player">The player to use for any player-related checks.</param>
  /// <returns>Returns the index within the <paramref name="text" /> at which to resume parsing.</returns>
  private static int ParseTagStartingAt(
    ref string text,
    int startIndex,
    Random random,
    TokenParserDelegate customParser,
    Farmer player)
  {
    for (int tagStartingAt = startIndex + 1; tagStartingAt < text.Length; ++tagStartingAt)
    {
      switch (text[tagStartingAt])
      {
        case '[':
          tagStartingAt = TokenParser.ParseTagStartingAt(ref text, tagStartingAt, random, customParser, player);
          break;
        case ']':
          string replacement;
          if (!TokenParser.ParseTag(text.Substring(startIndex + 1, tagStartingAt - startIndex - 1), out replacement, random, customParser, player))
            return tagStartingAt;
          text = text.Remove(startIndex, tagStartingAt - startIndex + 1);
          text = text.Insert(startIndex, replacement);
          return startIndex + replacement.Length - 1;
      }
    }
    return text.Length - 1;
  }

  /// <summary>Parse a tag substring within a text.</summary>
  /// <param name="tag">The token tag to parse, excluding the <see cref="F:StardewValley.TokenizableStrings.TokenParser.StartTokenChar" /> and <see cref="F:StardewValley.TokenizableStrings.TokenParser.EndTokenChar" /> characters.</param>
  /// <param name="replacement">The output string with which to replace the token within the text being parsed.</param>
  /// <param name="random">The RNG to use for randomization.</param>
  /// <param name="customParser">A custom token parser which will be given an opportunity to parse each token first, if any.</param>
  /// <param name="player">The player to use for any player-related checks.</param>
  /// <returns>Returns whether the tag was successfully parsed.</returns>
  private static bool ParseTag(
    string tag,
    out string replacement,
    Random random,
    TokenParserDelegate customParser,
    Farmer player)
  {
    string[] query = ArgUtility.SplitBySpace(tag);
    for (int index = 0; index < query.Length; ++index)
      query[index] = TokenParser.UnescapeText(query[index]);
    TokenParserDelegate tokenParserDelegate;
    if (customParser != null && customParser(query, out replacement, random, player) || TokenParser.Parsers.TryGetValue(query[0], out tokenParserDelegate) && tokenParserDelegate(query, out replacement, random, player))
      return true;
    replacement = (string) null;
    return false;
  }

  /// <summary>Reverse replacements from <see cref="M:StardewValley.TokenizableStrings.TokenParser.EscapeSpaces(System.String)" />.</summary>
  /// <param name="text">The text to unescape.</param>
  private static string UnescapeText(string text)
  {
    return text.Replace(' ', ' ').Replace(TokenParser.EscapedEmptyStr, "");
  }

  /// <summary>The resolvers for vanilla token strings. Most code should call <see cref="M:StardewValley.TokenizableStrings.TokenParser.ParseText(System.String,System.Random,StardewValley.TokenizableStrings.TokenParserDelegate,StardewValley.Farmer)" /> instead of using these directly.</summary>
  public static class DefaultResolvers
  {
    /// <summary>The translated display name for an achievement ID.</summary>
    /// <remarks>For example, <c>[AchievementName 5]</c> will output something like "A Complete Collection".</remarks>
    /// <inheritdoc cref="T:StardewValley.TokenizableStrings.TokenParserDelegate" />
    public static bool AchievementName(
      string[] query,
      out string replacement,
      Random random,
      Farmer player)
    {
      int key;
      string error;
      if (!ArgUtility.TryGetInt(query, 1, out key, out error, "int achievementId"))
        return TokenParser.LogTokenError(query, error, out replacement);
      string str;
      if (!Game1.achievements.TryGetValue(key, out str))
        return TokenParser.LogTokenError(query, $"unknown achievement ID '{key}'", out replacement);
      replacement = str.Split('^', 2)[0];
      return true;
    }

    /// <summary>The grammatical article ('a' or 'an') for the given word when playing in English, else blank.</summary>
    /// <remarks>For example: <c>[ArticleFor apple]</c> will output <c>an</c>.</remarks>
    /// <inheritdoc cref="T:StardewValley.TokenizableStrings.TokenParserDelegate" />
    public static bool ArticleFor(
      string[] query,
      out string replacement,
      Random random,
      Farmer player)
    {
      string word;
      string error;
      if (!ArgUtility.TryGet(query, 1, out word, out error, name: "string word"))
        return TokenParser.LogTokenError(query, error, out replacement);
      replacement = Lexicon.getProperArticleForWord(word);
      return true;
    }

    /// <summary>Get the input text with the first letter capitalized.</summary>
    /// <remarks>For example: <c>[CapitalizeFirstLetter an apple]</c> will output <c>An apple</c>.</remarks>
    /// <inheritdoc cref="T:StardewValley.TokenizableStrings.TokenParserDelegate" />
    public static bool CapitalizeFirstLetter(
      string[] query,
      out string replacement,
      Random random,
      Farmer player)
    {
      string s;
      string error;
      if (!ArgUtility.TryGetRemainder(query, 1, out s, out error, name: "string text"))
        return TokenParser.LogTokenError(query, error, out replacement);
      replacement = Utility.capitalizeFirstLetter(s);
      return true;
    }

    /// <summary>Replaces spaces in the given text with a special character that lets you pass them into other space-delimited tokens. The characters are automatically turned back into spaces when displayed.</summary>
    /// <remarks>For example: <c>[EscapedText Some arbitrary text]</c>.</remarks>
    /// <inheritdoc cref="T:StardewValley.TokenizableStrings.TokenParserDelegate" />
    public static bool EscapedText(
      string[] query,
      out string replacement,
      Random random,
      Farmer player)
    {
      replacement = string.Join(" ", ((IEnumerable<string>) query).Skip<string>(1));
      replacement = TokenParser.EscapeSpaces(replacement);
      return true;
    }

    /// <summary>Depending on the target player's gender, show either the male text or female text. To pass text containing spaces, wrap it in <c>EscapeText</c>.</summary>
    /// <inheritdoc cref="T:StardewValley.TokenizableStrings.TokenParserDelegate" />
    public static bool GenderedText(
      string[] query,
      out string replacement,
      Random random,
      Farmer player)
    {
      string str1;
      string error;
      string str2;
      string str3;
      if (!ArgUtility.TryGet(query, 1, out str1, out error, name: "string maleStr") || !ArgUtility.TryGet(query, 2, out str2, out error, name: "string femaleStr") || !ArgUtility.TryGetOptional(query, 3, out str3, out error, name: "string otherStr"))
        return TokenParser.LogTokenError(query, error, out replacement);
      switch (player.Gender)
      {
        case Gender.Male:
          replacement = str1;
          break;
        case Gender.Female:
          replacement = str2;
          break;
        default:
          replacement = str3 ?? str2;
          break;
      }
      return true;
    }

    /// <summary>The translated display name for a qualified item ID.</summary>
    /// <remarks>For example, <c>[ItemName (O)128]</c> returns a value like "Pufferfish".</remarks>
    /// <inheritdoc cref="T:StardewValley.TokenizableStrings.TokenParserDelegate" />
    public static bool ItemName(
      string[] query,
      out string replacement,
      Random random,
      Farmer player)
    {
      string itemId;
      string error;
      string str;
      if (!ArgUtility.TryGet(query, 1, out itemId, out error, name: "string itemId") || !ArgUtility.TryGetOptional(query, 2, out str, out error, name: "string fallbackItemName"))
        return TokenParser.LogTokenError(query, error, out replacement);
      replacement = ItemRegistry.GetData(itemId)?.DisplayName ?? str ?? ItemRegistry.GetErrorItemName(itemId);
      return true;
    }

    /// <summary>The translated display name for a qualified item ID which includes a preserved flavor.</summary>
    /// <remarks>For example, <c>[ItemNameWithFlavor Wine (O)258]</c> returns a value like "Blueberry Wine".</remarks>
    /// <inheritdoc cref="T:StardewValley.TokenizableStrings.TokenParserDelegate" />
    public static bool ItemNameWithFlavor(
      string[] query,
      out string replacement,
      Random random,
      Farmer player)
    {
      StardewValley.Object.PreserveType preserveType;
      string error;
      string str;
      string defaultBaseName;
      if (!ArgUtility.TryGetEnum<StardewValley.Object.PreserveType>(query, 1, out preserveType, out error, "Object.PreserveType preserveType") || !ArgUtility.TryGet(query, 2, out str, out error, name: "string preservedId") || !ArgUtility.TryGetOptional(query, 3, out defaultBaseName, out error, name: "string fallbackItemName"))
        return TokenParser.LogTokenError(query, error, out replacement);
      string idForFlavoredItem = ItemRegistry.GetObjectTypeDefinition().GetBaseItemIdForFlavoredItem(preserveType, str);
      replacement = StardewValley.Object.GetObjectDisplayName(idForFlavoredItem, new StardewValley.Object.PreserveType?(preserveType), str, defaultBaseName: defaultBaseName);
      return true;
    }

    /// <summary>Translation text loaded from a string key. If the translation has placeholder tokens like {0}, you can add the values after the string key. To pass arguments containing spaces, wrap them in <c>EscapeText</c>.</summary>
    /// <remarks>For example: <c>[LocalizedText Strings\NPCNames:OldMariner]</c>.</remarks>
    /// <inheritdoc cref="T:StardewValley.TokenizableStrings.TokenParserDelegate" />
    public static bool LocalizedText(
      string[] query,
      out string replacement,
      Random random,
      Farmer player)
    {
      string path;
      string error;
      if (!ArgUtility.TryGet(query, 1, out path, out error, name: "string key"))
        return TokenParser.LogTokenError(query, error, out replacement);
      object[] objArray;
      if (query.Length > 2)
      {
        objArray = new object[query.Length - 2];
        for (int index = 2; index < query.Length; ++index)
          objArray[index - 2] = (object) query[index];
      }
      else
        objArray = LegacyShims.EmptyArray<object>();
      try
      {
        replacement = objArray.Length != 0 ? Game1.content.LoadString(path, objArray) : Game1.content.LoadString(path);
        return true;
      }
      catch (ContentLoadException ex)
      {
        return TokenParser.LogTokenError(query, $"the key '{path}' doesn't match an existing asset", out replacement);
      }
      catch (InvalidCastException ex)
      {
        return TokenParser.LogTokenError(query, $"the key '{path}' matches an asset, but it isn't of the required type 'Dictionary<string, string>'", out replacement);
      }
    }

    /// <summary>The translated display name for a monster.</summary>
    /// <inheritdoc cref="T:StardewValley.TokenizableStrings.TokenParserDelegate" />
    public static bool MonsterName(
      string[] query,
      out string replacement,
      Random random,
      Farmer player)
    {
      string key;
      string error;
      string str1;
      if (!ArgUtility.TryGet(query, 1, out key, out error, name: "string monsterId") || !ArgUtility.TryGetOptional(query, 2, out str1, out error, name: "string fallbackText"))
        return TokenParser.LogTokenError(query, error, out replacement);
      string str2;
      replacement = DataLoader.Monsters(Game1.content).TryGetValue(key, out str2) ? ArgUtility.Get(str2.Split('/'), 14) : (string) null;
      replacement = replacement ?? str1 ?? key;
      return true;
    }

    /// <summary>The translated title for a movie ID.</summary>
    /// <remarks>For example, <c>[MovieTitle spring_movie_0]</c> will output something like "The Brave Little Sapling".</remarks>
    /// <inheritdoc cref="T:StardewValley.TokenizableStrings.TokenParserDelegate" />
    public static bool MovieName(
      string[] query,
      out string replacement,
      Random random,
      Farmer player)
    {
      string id;
      string error;
      if (!ArgUtility.TryGet(query, 1, out id, out error, name: "string movieId"))
        return TokenParser.LogTokenError(query, error, out replacement);
      MovieData data;
      if (!MovieTheater.TryGetMovieData(id, out data))
        return TokenParser.LogTokenError(query, $"unknown movie ID '{id}'", out replacement);
      replacement = TokenParser.ParseText(data.Title);
      return true;
    }

    /// <summary>Format a number with commas based on the current language.</summary>
    /// <inheritdoc cref="T:StardewValley.TokenizableStrings.TokenParserDelegate" />
    public static bool NumberWithSeparators(
      string[] query,
      out string replacement,
      Random random,
      Farmer player)
    {
      int number;
      string error;
      if (!ArgUtility.TryGetInt(query, 1, out number, out error, "int number"))
        return TokenParser.LogTokenError(query, error, out replacement);
      replacement = Utility.getNumberWithCommas(number);
      return true;
    }

    /// <summary>A random adjective from the <c>Strings\Lexicon</c> data asset's <c>RandomPositiveAdjective_PlaceOrEvent</c> entry.</summary>
    /// <inheritdoc cref="T:StardewValley.TokenizableStrings.TokenParserDelegate" />
    public static bool PositiveAdjective(
      string[] query,
      out string replacement,
      Random random,
      Farmer player)
    {
      replacement = Lexicon.getRandomPositiveAdjectiveForEventOrPerson();
      return true;
    }

    /// <summary>The translated display name for a special order ID.</summary>
    /// <inheritdoc cref="T:StardewValley.TokenizableStrings.TokenParserDelegate" />
    public static bool SpecialOrderName(
      string[] query,
      out string replacement,
      Random random,
      Farmer player)
    {
      string id;
      string error;
      if (!ArgUtility.TryGet(query, 1, out id, out error, name: "string orderId"))
        return TokenParser.LogTokenError(query, error, out replacement);
      foreach (SpecialOrder specialOrder in Game1.player.team.specialOrders)
      {
        if (specialOrder.questKey.Value == id)
        {
          replacement = specialOrder.GetName();
          return true;
        }
      }
      SpecialOrderData data;
      if (!SpecialOrder.TryGetData(id, out data))
        return TokenParser.LogTokenError(query, $"unknown special order ID '{id}'", out replacement);
      replacement = SpecialOrder.MakeLocalizationReplacements(TokenParser.ParseText(data.Name));
      return true;
    }

    /// <summary>Show different text depending on whether the target player's spouse is a player (first argument) or NPC (second argument). To pass text containing spaces, wrap it in <c>EscapeText</c>.</summary>
    /// <inheritdoc cref="T:StardewValley.TokenizableStrings.TokenParserDelegate" />
    public static bool SpouseFarmerText(
      string[] query,
      out string replacement,
      Random random,
      Farmer player)
    {
      string str1;
      string error;
      string str2;
      if (!ArgUtility.TryGet(query, 1, out str1, out error, name: "string playerSpouse") || !ArgUtility.TryGet(query, 2, out str2, out error, name: "string npcSpouse"))
        return TokenParser.LogTokenError(query, error, out replacement);
      if (player.team.GetSpouse(player.UniqueMultiplayerID).HasValue)
      {
        replacement = str1;
        return true;
      }
      if (player.getSpouse() == null)
        return TokenParser.LogTokenError(query, $"the target player '{player.Name}' isn't married", out replacement);
      replacement = str2;
      return true;
    }

    /// <summary>Equivalent to <see cref="M:StardewValley.TokenizableStrings.TokenParser.DefaultResolvers.GenderedText(System.String[],System.String@,System.Random,StardewValley.Farmer)" />, but based on the gender of the player's NPC or player spouse.</summary>
    /// <inheritdoc cref="T:StardewValley.TokenizableStrings.TokenParserDelegate" />
    public static bool SpouseGenderedText(
      string[] query,
      out string replacement,
      Random random,
      Farmer player)
    {
      string str1;
      string error;
      string str2;
      string str3;
      if (!ArgUtility.TryGet(query, 1, out str1, out error, name: "string maleStr") || !ArgUtility.TryGet(query, 2, out str2, out error, name: "string femaleStr") || !ArgUtility.TryGetOptional(query, 3, out str3, out error, name: "string otherStr"))
        return TokenParser.LogTokenError(query, error, out replacement);
      Gender? nullable = new Gender?();
      long? spouse = player.team.GetSpouse(player.UniqueMultiplayerID);
      if (spouse.HasValue)
      {
        Farmer player1 = Game1.GetPlayer(spouse.Value);
        nullable = new Gender?(player1 != null ? player1.Gender : Gender.Male);
      }
      else
        nullable = player.getSpouse()?.Gender;
      if (!nullable.HasValue)
        return TokenParser.LogTokenError(query, $"the target player '{player.Name}' isn't married", out replacement);
      if (nullable.HasValue)
      {
        switch (nullable.GetValueOrDefault())
        {
          case Gender.Male:
            replacement = str1;
            goto label_11;
          case Gender.Female:
            replacement = str2;
            goto label_11;
        }
      }
      replacement = str3 ?? str2;
label_11:
      return true;
    }

    /// <summary>The translated display name for a qualified tool ID.</summary>
    /// <remarks>For example, <c>[ToolName (T)IridiumAxe]</c> returns a value like "Iridium Axe".</remarks>
    /// <inheritdoc cref="T:StardewValley.TokenizableStrings.TokenParserDelegate" />
    public static bool ToolName(
      string[] query,
      out string replacement,
      Random random,
      Farmer player)
    {
      string itemId;
      string error;
      if (!ArgUtility.TryGet(query, 1, out itemId, out error, name: "string itemId") || !ArgUtility.TryGetOptionalInt(query, 2, out int _, out error, -1, "int upgradeLevel"))
        return TokenParser.LogTokenError(query, error, out replacement);
      ParsedItemData dataOrErrorItem = ItemRegistry.GetDataOrErrorItem(itemId);
      if (!dataOrErrorItem.HasTypeId("(T)"))
        return TokenParser.LogTokenError(query, $"the item ID '{itemId}' matches a non-tool item", out replacement);
      replacement = dataOrErrorItem.DisplayName;
      return true;
    }

    /// <summary>The numeric day of month, like <c>5</c> on spring 5.</summary>
    /// <inheritdoc cref="T:StardewValley.TokenizableStrings.TokenParserDelegate" />
    public static bool DayOfMonth(
      string[] query,
      out string replacement,
      Random random,
      Farmer player)
    {
      replacement = Game1.dayOfMonth.ToString();
      return true;
    }

    /// <summary>The current season name, like <c>spring</c>.</summary>
    /// <inheritdoc cref="T:StardewValley.TokenizableStrings.TokenParserDelegate" />
    public static bool Season(
      string[] query,
      out string replacement,
      Random random,
      Farmer player)
    {
      replacement = Game1.CurrentSeasonDisplayName;
      return true;
    }

    /// <summary>The translated display name for an NPC, given their internal name.</summary>
    /// <inheritdoc cref="T:StardewValley.TokenizableStrings.TokenParserDelegate" />
    public static bool CharacterName(
      string[] query,
      out string replacement,
      Random random,
      Farmer player)
    {
      string name;
      string error;
      if (!ArgUtility.TryGet(query, 1, out name, out error, name: "string npcName"))
        return TokenParser.LogTokenError(query, error, out replacement);
      NPC characterFromName = Game1.getCharacterFromName(name);
      if (characterFromName == null)
        return TokenParser.LogTokenError(query, $"no character found with name '{name}'", out replacement);
      replacement = characterFromName.displayName;
      return true;
    }

    /// <summary>The farm name for the current save (without the injected "Farm" text).</summary>
    /// <inheritdoc cref="T:StardewValley.TokenizableStrings.TokenParserDelegate" />
    public static bool FarmName(
      string[] query,
      out string replacement,
      Random random,
      Farmer player)
    {
      replacement = player.farmName.Value;
      return true;
    }

    /// <summary>The target player's unique internal multiplayer ID.</summary>
    /// <inheritdoc cref="T:StardewValley.TokenizableStrings.TokenParserDelegate" />
    public static bool FarmerUniqueId(
      string[] query,
      out string replacement,
      Random random,
      Farmer player)
    {
      replacement = player.UniqueMultiplayerID.ToString();
      return true;
    }

    /// <summary>The translated display name for a location given its ID in <c>Data/Locations</c>.</summary>
    /// <inheritdoc cref="T:StardewValley.TokenizableStrings.TokenParserDelegate" />
    public static bool LocationName(
      string[] query,
      out string replacement,
      Random random,
      Farmer player)
    {
      string name;
      string error;
      if (!ArgUtility.TryGet(query, 1, out name, out error, name: "string locationKey"))
        return TokenParser.LogTokenError(query, error, out replacement);
      GameLocation locationFromName = Game1.getLocationFromName(name);
      if (locationFromName == null)
        return TokenParser.LogTokenError(query, $"no location found with name '{name}'", out replacement);
      replacement = locationFromName.DisplayName;
      return true;
    }

    /// <summary>The value of a tracked player stat.</summary>
    /// <inheritdoc cref="T:StardewValley.TokenizableStrings.TokenParserDelegate" />
    public static bool FarmerStat(
      string[] query,
      out string replacement,
      Random random,
      Farmer player)
    {
      string key;
      string error;
      if (!ArgUtility.TryGet(query, 1, out key, out error, name: "string statName"))
        return TokenParser.LogTokenError(query, error, out replacement);
      replacement = player.stats.Get(key).ToString();
      return true;
    }
  }
}
