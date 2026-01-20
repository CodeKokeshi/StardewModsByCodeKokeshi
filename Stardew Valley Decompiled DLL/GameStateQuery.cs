// Decompiled with JetBrains decompiler
// Type: StardewValley.GameStateQuery
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using StardewValley.Delegates;
using StardewValley.Extensions;
using StardewValley.GameData.Locations;
using StardewValley.Internal;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Locations;
using StardewValley.Network;
using StardewValley.Objects.Trinkets;
using System;
using System.Collections.Generic;
using System.Reflection;

#nullable disable
namespace StardewValley;

/// <summary>Resolves game state queries like <c>SEASON spring</c> in data assets.</summary>
/// <summary>Resolves game state queries like <c>SEASON spring</c> in data assets.</summary>
public class GameStateQuery
{
  /// <summary>The supported game state queries and their resolvers.</summary>
  private static readonly Dictionary<string, GameStateQueryDelegate> QueryTypeLookup = new Dictionary<string, GameStateQueryDelegate>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
  /// <summary>Alternate names for game state queries (e.g. shorthand or acronyms).</summary>
  private static readonly Dictionary<string, string> Aliases = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
  /// <summary>The <see cref="F:StardewValley.Game1.ticks" /> value when the cache should be reset.</summary>
  private static int NextClearCacheTick;
  /// <summary>The cache of parsed game state queries.</summary>
  private static readonly Dictionary<string, GameStateQuery.ParsedGameStateQuery[]> ParseCache = new Dictionary<string, GameStateQuery.ParsedGameStateQuery[]>();
  /// <summary>The query keys which check the season, like <c>LOCATION_SEASON</c> or <c>SEASON</c>.</summary>
  public static HashSet<string> SeasonQueryKeys = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
  {
    "LOCATION_SEASON",
    "SEASON"
  };
  /// <summary>The query keys which are ignored when catching fish with the Magic Bait equipped.</summary>
  public static HashSet<string> MagicBaitIgnoreQueryKeys = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
  {
    "DAY_OF_MONTH",
    "DAY_OF_WEEK",
    "DAYS_PLAYED",
    "LOCATION_SEASON",
    "SEASON",
    "SEASON_DAY",
    "WEATHER",
    "TIME"
  };

  /// <summary>Register the default game state queries, defined as <see cref="T:StardewValley.GameStateQuery.DefaultResolvers" /> methods.</summary>
  static GameStateQuery()
  {
    MethodInfo[] methods = typeof (GameStateQuery.DefaultResolvers).GetMethods(BindingFlags.Static | BindingFlags.Public);
    foreach (MethodInfo method in methods)
    {
      GameStateQueryDelegate queryDelegate = (GameStateQueryDelegate) Delegate.CreateDelegate(typeof (GameStateQueryDelegate), method);
      GameStateQuery.Register(method.Name, queryDelegate);
    }
    foreach (MethodInfo element in methods)
    {
      OtherNamesAttribute customAttribute = element.GetCustomAttribute<OtherNamesAttribute>();
      if (customAttribute != null)
      {
        foreach (string alias in customAttribute.Aliases)
          GameStateQuery.RegisterAlias(alias, element.Name);
      }
    }
  }

  /// <summary>Update the game state query tracking.</summary>
  internal static void Update()
  {
    if (Game1.ticks < GameStateQuery.NextClearCacheTick)
      return;
    if (GameStateQuery.ParseCache.Count > 50)
      GameStateQuery.ParseCache.Clear();
    GameStateQuery.NextClearCacheTick = Game1.ticks + 3600;
  }

  /// <summary>Get whether a game state query exists.</summary>
  /// <param name="queryKey">The game state query key, like <c>SEASON</c>.</param>
  public static bool Exists(string queryKey)
  {
    if (queryKey == null)
      return false;
    return GameStateQuery.QueryTypeLookup.ContainsKey(queryKey) || GameStateQuery.Aliases.ContainsKey(queryKey);
  }

  /// <summary>Register a game state query resolver.</summary>
  /// <param name="queryKey">The game state query key, like <c>SEASON</c>. This should only contain alphanumeric, underscore, and dot characters. For custom queries, this should be prefixed with your mod ID like <c>Example.ModId_QueryName</c>.</param>
  /// <param name="queryDelegate">The resolver which returns whether a given query matches in the current context.</param>
  /// <exception cref="T:System.ArgumentException">The <paramref name="queryKey" /> is null or whitespace-only.</exception>
  /// <exception cref="T:System.ArgumentNullException">The <paramref name="queryDelegate" /> is null.</exception>
  /// <exception cref="T:System.InvalidOperationException">The <paramref name="queryKey" /> is already registered.</exception>
  public static void Register(string queryKey, GameStateQueryDelegate queryDelegate)
  {
    queryKey = queryKey?.Trim();
    if (string.IsNullOrWhiteSpace(queryKey))
      throw new ArgumentException("The query key can't be null or empty.", nameof (queryKey));
    if (GameStateQuery.QueryTypeLookup.ContainsKey(queryKey))
      throw new InvalidOperationException($"The query key '{queryKey}' is already registered.");
    string str;
    if (GameStateQuery.Aliases.TryGetValue(queryKey, out str))
      throw new InvalidOperationException($"The query key '{queryKey}' is already registered as an alias of '{str}'.");
    Dictionary<string, GameStateQueryDelegate> queryTypeLookup = GameStateQuery.QueryTypeLookup;
    string key = queryKey;
    queryTypeLookup[key] = queryDelegate ?? throw new ArgumentNullException(nameof (queryDelegate));
  }

  /// <summary>Register an alternate name for a game state query.</summary>
  /// <param name="alias">The alias to register. This should only contain alphanumeric, underscore, and dot characters. For custom queries, this should be prefixed with your mod ID like <c>Example.ModId_QueryName</c>.</param>
  /// <param name="queryKey">The game state query key to map it to, like <c>SEASON</c>. This should already be registered (e.g. via <see cref="M:StardewValley.GameStateQuery.Register(System.String,StardewValley.Delegates.GameStateQueryDelegate)" />).</param>
  /// <exception cref="T:System.ArgumentException">The <paramref name="alias" /> or <paramref name="queryKey" /> is null or whitespace-only.</exception>
  /// <exception cref="T:System.InvalidOperationException">The <paramref name="queryKey" /> is already registered.</exception>
  public static void RegisterAlias(string alias, string queryKey)
  {
    alias = alias?.Trim();
    if (string.IsNullOrWhiteSpace(alias))
      throw new ArgumentException("The alias can't be null or empty.", nameof (alias));
    if (GameStateQuery.QueryTypeLookup.ContainsKey(alias))
      throw new InvalidOperationException($"The alias '{alias}' is already registered as a game state query.");
    string str;
    if (GameStateQuery.Aliases.TryGetValue(alias, out str))
      throw new InvalidOperationException($"The alias '{alias}' is already registered for '{str}'.");
    if (string.IsNullOrWhiteSpace(queryKey))
      throw new ArgumentException("The query key can't be null or empty.", nameof (alias));
    GameStateQuery.Aliases[alias] = GameStateQuery.QueryTypeLookup.ContainsKey(queryKey) ? queryKey : throw new InvalidOperationException($"The alias '{alias}' can't be registered for '{queryKey}' because there's no game state query with that name.");
  }

  /// <summary>Get whether a set of game state queries matches in the current context.</summary>
  /// <param name="queryString">The game state queries to check as a comma-delimited string.</param>
  /// <param name="location">The location for which to check the query, or <c>null</c> to use the current location.</param>
  /// <param name="player">The player for which to check the query, or <c>null</c> to use the current player.</param>
  /// <param name="targetItem">The target item (e.g. machine output or tree fruit) for which to check queries, or <c>null</c> if not applicable.</param>
  /// <param name="inputItem">The input item (e.g. machine input) for which to check queries, or <c>null</c> if not applicable.</param>
  /// <param name="random">The RNG to use for randomization, or <c>null</c> to use <see cref="F:StardewValley.Game1.random" />.</param>
  /// <param name="ignoreQueryKeys">The query keys to ignore when checking conditions (like <c>LOCATION_SEASON</c>), or <c>null</c> to check all of them.</param>
  /// <returns>Returns whether the query matches.</returns>
  public static bool CheckConditions(
    string queryString,
    GameLocation location = null,
    Farmer player = null,
    Item targetItem = null,
    Item inputItem = null,
    Random random = null,
    HashSet<string> ignoreQueryKeys = null)
  {
    switch (queryString)
    {
      case null:
      case "":
      case "TRUE":
        return true;
      case "FALSE":
        return false;
      default:
        GameStateQueryContext context = new GameStateQueryContext(location, player, targetItem, inputItem, random, ignoreQueryKeys);
        return GameStateQuery.CheckConditionsImpl(queryString, context);
    }
  }

  /// <summary>Get whether a set of game state queries matches in the current context.</summary>
  /// <param name="queryString">The game state queries to check as a comma-delimited string.</param>
  /// <param name="context">The game state query context.</param>
  /// <returns>Returns whether the query matches.</returns>
  public static bool CheckConditions(string queryString, GameStateQueryContext context)
  {
    switch (queryString)
    {
      case null:
      case "":
      case "TRUE":
        return true;
      case "FALSE":
        return false;
      default:
        return GameStateQuery.CheckConditionsImpl(queryString, context);
    }
  }

  /// <summary>Get whether a game state query can never be true under any circumstance (e.g. <c>FALSE</c> or <c>!TRUE</c>).</summary>
  /// <param name="queryString">The game state queries to check as a comma-delimited string.</param>
  public static bool IsImmutablyFalse(string queryString)
  {
    switch (queryString)
    {
      case null:
      case "":
      case "TRUE":
        return false;
      case "FALSE":
        return true;
      default:
        foreach (GameStateQuery.ParsedGameStateQuery parsedGameStateQuery in GameStateQuery.Parse(queryString))
        {
          if (parsedGameStateQuery.Query.Length != 0)
          {
            string str = parsedGameStateQuery.Negated ? "TRUE" : "FALSE";
            if (parsedGameStateQuery.Query[0].EqualsIgnoreCase(str))
              return true;
          }
        }
        return false;
    }
  }

  /// <summary>Get whether a game state query can never be false under any circumstance (e.g. <c>TRUE</c>, <c>!FALSE</c>, or empty).</summary>
  /// <param name="queryString">The game state queries to check as a comma-delimited string.</param>
  public static bool IsImmutablyTrue(string queryString)
  {
    switch (queryString)
    {
      case null:
      case "":
      case "TRUE":
        return true;
      case "FALSE":
        return false;
      default:
        foreach (GameStateQuery.ParsedGameStateQuery parsedGameStateQuery in GameStateQuery.Parse(queryString))
        {
          if (parsedGameStateQuery.Query.Length != 0)
          {
            string str = parsedGameStateQuery.Negated ? "FALSE" : "TRUE";
            if (!parsedGameStateQuery.Query[0].EqualsIgnoreCase(str))
              return false;
          }
        }
        return true;
    }
  }

  /// <summary>Parse a raw query string into its component query data.</summary>
  /// <param name="queryString">The query string to parse.</param>
  /// <returns>Returns the parsed game state queries. This value is cached, so it should not be modified. If any part of the query string is invalid, this returns a single value containing the invalid query with the error property set.</returns>
  public static GameStateQuery.ParsedGameStateQuery[] Parse(string queryString)
  {
    GameStateQuery.ParsedGameStateQuery[] parsedGameStateQueryArray;
    if (!GameStateQuery.ParseCache.TryGetValue(queryString, out parsedGameStateQueryArray))
    {
      string[] strArray = GameStateQuery.SplitRaw(queryString);
      parsedGameStateQueryArray = new GameStateQuery.ParsedGameStateQuery[strArray.Length];
      for (int index = 0; index < strArray.Length; ++index)
      {
        string[] query = ArgUtility.SplitBySpaceQuoteAware(strArray[index]);
        string key = query[0];
        bool negated = key.StartsWith('!');
        if (negated)
          query[0] = key = key.Substring(1);
        string str;
        if (GameStateQuery.Aliases.TryGetValue(key, out str))
        {
          key = str;
          query[0] = str;
        }
        GameStateQueryDelegate resolver;
        if (!GameStateQuery.QueryTypeLookup.TryGetValue(key, out resolver))
        {
          if (parsedGameStateQueryArray.Length > 1)
            parsedGameStateQueryArray = new GameStateQuery.ParsedGameStateQuery[1];
          parsedGameStateQueryArray[0] = new GameStateQuery.ParsedGameStateQuery(false, query, (GameStateQueryDelegate) null, $"'{key}' isn't a known query or alias");
          break;
        }
        parsedGameStateQueryArray[index] = new GameStateQuery.ParsedGameStateQuery(negated, query, resolver, (string) null);
      }
      GameStateQuery.ParseCache[queryString] = parsedGameStateQueryArray;
    }
    return parsedGameStateQueryArray;
  }

  /// <summary>Split a query string into its top-level component queries without parsing them.</summary>
  /// <param name="queryString">The query string to split.</param>
  public static string[] SplitRaw(string queryString)
  {
    return ArgUtility.SplitQuoteAware(queryString, ',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries, true);
  }

  /// <summary>Get whether a set of game state queries matches in the current context without short-circuiting immutable values like <c>TRUE</c>.</summary>
  /// <param name="queryString">The game state queries to check as a comma-delimited string.</param>
  /// <param name="context">The game state query context.</param>
  /// <returns>Returns whether the query matches.</returns>
  private static bool CheckConditionsImpl(string queryString, GameStateQueryContext context)
  {
    if (queryString == null)
      return true;
    GameStateQuery.ParsedGameStateQuery[] parsedGameStateQueryArray = GameStateQuery.Parse(queryString);
    if (parsedGameStateQueryArray.Length == 0)
      return true;
    if (parsedGameStateQueryArray[0].Error != null)
      return GameStateQuery.Helpers.ErrorResult(parsedGameStateQueryArray[0].Query, parsedGameStateQueryArray[0].Error);
    foreach (GameStateQuery.ParsedGameStateQuery parsedGameStateQuery in parsedGameStateQueryArray)
    {
      HashSet<string> ignoreQueryKeys = context.IgnoreQueryKeys;
      // ISSUE: explicit non-virtual call
      if ((ignoreQueryKeys != null ? (__nonvirtual (ignoreQueryKeys.Contains(parsedGameStateQuery.Query[0])) ? 1 : 0) : 0) == 0)
      {
        try
        {
          if (parsedGameStateQuery.Resolver(parsedGameStateQuery.Query, context) == parsedGameStateQuery.Negated)
            return false;
        }
        catch (Exception ex)
        {
          return GameStateQuery.Helpers.ErrorResult(parsedGameStateQuery.Query, "unhandled exception", ex);
        }
      }
    }
    return true;
  }

  /// <summary>The helper methods which simplify implementing custom game state query resolvers.</summary>
  public static class Helpers
  {
    /// <summary>Get the location matching a given name.</summary>
    /// <param name="locationName">The location to check. This can be <c>Here</c> (current location), <c>Target</c> (contextual location), or a location name.</param>
    /// <param name="contextualLocation">The location for which the query is being checked.</param>
    public static GameLocation GetLocation(string locationName, GameLocation contextualLocation)
    {
      if (locationName.EqualsIgnoreCase("Here"))
        return Game1.currentLocation;
      return locationName.EqualsIgnoreCase("Target") ? contextualLocation ?? Game1.currentLocation : Game1.getLocationFromName(locationName);
    }

    /// <summary>Get the location matching a given name, or throw an exception if it's not found.</summary>
    /// <param name="locationName">The location to check. This can be <c>Here</c> (current location), <c>Target</c> (contextual location), or a location name.</param>
    /// <param name="contextualLocation">The location for which the query is being checked.</param>
    public static GameLocation RequireLocation(string locationName, GameLocation contextualLocation)
    {
      return GameStateQuery.Helpers.GetLocation(locationName, contextualLocation) ?? throw new KeyNotFoundException($"Required location '{locationName}' not found.");
    }

    /// <summary>Try to get a location matching a target location argument.</summary>
    /// <param name="query">The game state query split by space, including the query key.</param>
    /// <param name="index">The argument index to read.</param>
    /// <param name="error">An error phrase indicating why getting the argument failed (like 'required index X not found'), if applicable.</param>
    /// <param name="location">The contextual location instance, which will be updated if the argument is valid.</param>
    public static bool TryGetLocationArg(
      string[] query,
      int index,
      ref GameLocation location,
      out string error)
    {
      string locationName;
      if (!ArgUtility.TryGet(query, index, out locationName, out error, name: "string locationTarget"))
      {
        location = (GameLocation) null;
        return false;
      }
      GameLocation location1 = GameStateQuery.Helpers.GetLocation(locationName, location);
      if (location1 == null)
      {
        error = $"no location found matching '{locationName}'";
        return false;
      }
      location = location1;
      return true;
    }

    /// <summary>Try to get an item matching an item type argument.</summary>
    /// <param name="query">The game state query split by space, including the query key.</param>
    /// <param name="index">The argument index to read.</param>
    /// <param name="targetItem">The target item (e.g. machine output or tree fruit), or <c>null</c> if not applicable.</param>
    /// <param name="inputItem">The input item (e.g. machine input), or <c>null</c> if not applicable.</param>
    /// <param name="error">An error phrase indicating why getting the argument failed (like 'required index X not found'), if applicable.</param>
    /// <param name="item">The item instance, if valid.</param>
    public static bool TryGetItemArg(
      string[] query,
      int index,
      Item targetItem,
      Item inputItem,
      out Item item,
      out string error)
    {
      string str;
      if (!ArgUtility.TryGet(query, index, out str, out error, name: "string itemType"))
      {
        item = (Item) null;
        return false;
      }
      if (str.EqualsIgnoreCase("Target"))
      {
        item = targetItem;
        return true;
      }
      if (str.EqualsIgnoreCase("Input"))
      {
        item = inputItem;
        return true;
      }
      item = (Item) null;
      error = $"invalid item type '{str}' (should be 'Input' or 'Target')";
      return false;
    }

    /// <summary>Get whether a check applies to the given player or players.</summary>
    /// <param name="contextualPlayer">The player for which the query is being checked.</param>
    /// <param name="playerKey">The players to check. This can be <c>Any</c> (at least one player matches), <c>All</c> (every player matches), <c>Current</c> (the current player), <c>Target</c> (the contextual player), <c>Host</c> (the main player), or a player ID.</param>
    /// <param name="check">The check to perform.</param>
    public static bool WithPlayer(
      Farmer contextualPlayer,
      string playerKey,
      Func<Farmer, bool> check)
    {
      if (playerKey.EqualsIgnoreCase("Any"))
      {
        foreach (Farmer allFarmer in Game1.getAllFarmers())
        {
          if (check(allFarmer))
            return true;
        }
        return false;
      }
      if (playerKey.EqualsIgnoreCase("All"))
      {
        foreach (Farmer allFarmer in Game1.getAllFarmers())
        {
          if (!check(allFarmer))
            return false;
        }
        return true;
      }
      if (playerKey.EqualsIgnoreCase("Current"))
        return check(Game1.player);
      if (playerKey.EqualsIgnoreCase("Target"))
        return check(contextualPlayer);
      if (playerKey.EqualsIgnoreCase("Host"))
        return check(Game1.MasterPlayer);
      long result;
      return long.TryParse(playerKey, out result) && check(Game1.GetPlayer(result));
    }

    /// <summary>Get whether any query argument matches a condition.</summary>
    /// <param name="query">The game state query split by space, including the query key.</param>
    /// <param name="startAt">The index within <paramref name="query" /> to start iterating.</param>
    /// <param name="check">Check whether a query argument matches. This should return true (argument matches), false (argument doesn't match, but we can try the remaining arguments), or null (argument caused an error so we should stop iterating).</param>
    public static bool AnyArgMatches(string[] query, int startAt, Func<string, bool?> check)
    {
      for (int index = startAt; index < query.Length; ++index)
      {
        bool? nullable = check(query[index]);
        if (!nullable.HasValue)
          return false;
        if (nullable.GetValueOrDefault())
          return true;
      }
      return false;
    }

    /// <summary>Log an error indicating that a query couldn't be parsed.</summary>
    /// <param name="query">The game state query split by space, including the query key.</param>
    /// <param name="reason">The human-readable reason why the query is invalid.</param>
    /// <param name="exception">The underlying exception, if applicable.</param>
    /// <returns>Returns false.</returns>
    public static bool ErrorResult(string[] query, string reason, Exception exception = null)
    {
      Game1.log.Error($"Failed parsing condition '{string.Join(" ", query)}': {reason}.", exception);
      return false;
    }

    /// <summary>The common implementation for <c>PLAYER_*_SKILL</c> game state queries.</summary>
    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool PlayerSkillLevelImpl(
      string[] query,
      Farmer player,
      Func<Farmer, int> getLevel)
    {
      string playerKey;
      string error;
      int minLevel;
      int maxLevel;
      return !ArgUtility.TryGet(query, 1, out playerKey, out error, name: "string playerKey") || !ArgUtility.TryGetInt(query, 2, out minLevel, out error, "int minLevel") || !ArgUtility.TryGetOptionalInt(query, 3, out maxLevel, out error, int.MaxValue, "int maxLevel") ? GameStateQuery.Helpers.ErrorResult(query, error) : GameStateQuery.Helpers.WithPlayer(player, playerKey, (Func<Farmer, bool>) (target =>
      {
        int num = getLevel(target);
        return num >= minLevel && num <= maxLevel;
      }));
    }

    /// <summary>The common implementation for most <c>RANDOM</c> game state queries.</summary>
    /// <param name="random">The random instance to use.</param>
    /// <param name="query">The condition arguments received by the query.</param>
    /// <param name="skipArguments">The number of arguments to skip. The next argument should be the chance value, followed by an optional <c>@addDailyLuck</c> argument.</param>
    public static bool RandomImpl(Random random, string[] query, int skipArguments)
    {
      float num;
      string error;
      if (!ArgUtility.TryGetFloat(query, skipArguments, out num, out error, "float chance"))
        return GameStateQuery.Helpers.ErrorResult(query, error);
      bool flag = false;
      for (int index = skipArguments + 1; index < query.Length; ++index)
      {
        if (query[index].EqualsIgnoreCase("@addDailyLuck"))
          flag = true;
      }
      if (flag)
        num += (float) Game1.player.DailyLuck;
      return random.NextDouble() < (double) num;
    }
  }

  /// <summary>The cached metadata for a single raw game state query.</summary>
  /// <summary>Construct an instance.</summary>
  /// <param name="negated">Whether the result should be negated.</param>
  /// <param name="query">The game state query split by space, including the query key.</param>
  /// <param name="resolver">The resolver which handles the game state query.</param>
  /// <param name="error">An error indicating why parsing the query failed, if applicable.</param>
  public readonly struct ParsedGameStateQuery(
    bool negated,
    string[] query,
    GameStateQueryDelegate resolver,
    string error)
  {
    /// <summary>Whether the result should be negated.</summary>
    public readonly bool Negated = negated;
    /// <summary>The game state query split by space, including the query key.</summary>
    public readonly string[] Query = query;
    /// <summary>The resolver which handles the game state query.</summary>
    public readonly GameStateQueryDelegate Resolver = resolver;
    /// <summary>An error indicating why the query is invalid, if applicable.</summary>
    public readonly string Error = error;
  }

  /// <summary>The resolvers for vanilla game state queries. Most code should call <see cref="M:StardewValley.GameStateQuery.CheckConditions(System.String,StardewValley.Delegates.GameStateQueryContext)" /> instead of using these directly.</summary>
  public static class DefaultResolvers
  {
    /// <summary>Get whether any of the given conditions match.</summary>
    /// <remarks>The query arguments must be passed as quoted arguments. For example, <c>ANY "SEASON Winter" "SEASON Spring, DAY_OF_WEEK Friday"</c> is true if (a) it's winter or (b) it's a spring Friday.</remarks>
    /// 
    ///             /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool ANY(string[] query, GameStateQueryContext context)
    {
      return GameStateQuery.Helpers.AnyArgMatches(query, 1, (Func<string, bool?>) (value => new bool?(GameStateQuery.CheckConditions(value, context))));
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool DATE_RANGE(string[] query, GameStateQueryContext context)
    {
      Season season1;
      string error;
      int dayOfMonth1;
      int year1;
      Season season2;
      int dayOfMonth2;
      int year2;
      if (!ArgUtility.TryGetEnum<Season>(query, 1, out season1, out error, "Season minSeason") || !ArgUtility.TryGetInt(query, 2, out dayOfMonth1, out error, "int minDayOfMonth") || !ArgUtility.TryGetInt(query, 3, out year1, out error, "int minYear") || !ArgUtility.TryGetOptionalEnum<Season>(query, 4, out season2, out error, Season.Winter, "Season maxSeason") || !ArgUtility.TryGetOptionalInt(query, 5, out dayOfMonth2, out error, 28, "int maxDayOfMonth") || !ArgUtility.TryGetOptionalInt(query, 6, out year2, out error, int.MaxValue, "int maxYear"))
        return GameStateQuery.Helpers.ErrorResult(query, error);
      int daysPlayed = WorldDate.GetDaysPlayed(year1, season1, dayOfMonth1);
      int num = year2 != int.MaxValue ? WorldDate.GetDaysPlayed(year2, season2, dayOfMonth2) : int.MaxValue;
      int totalDays = Game1.Date.TotalDays;
      return totalDays >= daysPlayed && totalDays <= num;
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool SEASON_DAY(string[] query, GameStateQueryContext context)
    {
      for (int index = 1; index < query.Length; index += 2)
      {
        Season season;
        string error;
        int num;
        if (!ArgUtility.TryGetEnum<Season>(query, index, out season, out error, "Season season") || !ArgUtility.TryGetInt(query, index + 1, out num, out error, "int day"))
          return GameStateQuery.Helpers.ErrorResult(query, error);
        if (Game1.season == season && Game1.dayOfMonth == num)
          return true;
      }
      return false;
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool DAY_OF_MONTH(string[] query, GameStateQueryContext context)
    {
      return GameStateQuery.Helpers.AnyArgMatches(query, 1, (Func<string, bool?>) (rawDay =>
      {
        int result;
        if (int.TryParse(rawDay, out result))
          return new bool?(Game1.dayOfMonth == result);
        if (rawDay.EqualsIgnoreCase("even"))
          return new bool?(Game1.dayOfMonth % 2 == 0);
        if (rawDay.EqualsIgnoreCase("odd"))
          return new bool?(Game1.dayOfMonth % 2 == 1);
        GameStateQuery.Helpers.ErrorResult(query, $"'{rawDay}' isn't a valid day of month");
        return new bool?();
      }));
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool DAY_OF_WEEK(string[] query, GameStateQueryContext context)
    {
      return GameStateQuery.Helpers.AnyArgMatches(query, 1, (Func<string, bool?>) (rawDay =>
      {
        DayOfWeek dayOfWeek;
        if (!WorldDate.TryGetDayOfWeekFor(rawDay, out dayOfWeek))
        {
          GameStateQuery.Helpers.ErrorResult(query, $"'{rawDay}' isn't a valid day of week");
          return new bool?();
        }
        return Game1.Date.DayOfWeek == dayOfWeek ? new bool?(true) : new bool?(false);
      }));
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool DAYS_PLAYED(string[] query, GameStateQueryContext context)
    {
      int num1;
      string error;
      int num2;
      if (!ArgUtility.TryGetInt(query, 1, out num1, out error, "int minDaysPlayed") || !ArgUtility.TryGetOptionalInt(query, 2, out num2, out error, int.MaxValue, "int maxDaysPlayed"))
        return GameStateQuery.Helpers.ErrorResult(query, error);
      uint daysPlayed = Game1.stats.DaysPlayed;
      return (long) daysPlayed >= (long) num1 && (long) daysPlayed <= (long) num2;
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool IS_GREEN_RAIN_DAY(string[] query, GameStateQueryContext context)
    {
      WorldDate worldDate = new WorldDate(Game1.Date);
      ++worldDate.TotalDays;
      return Utility.isGreenRainDay(worldDate.DayOfMonth, worldDate.Season);
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool IS_FESTIVAL_DAY(string[] query, GameStateQueryContext context)
    {
      string str;
      string error;
      int num1;
      if (!ArgUtility.TryGetOptional(query, 1, out str, out error, "any", false, "string locationContextId") || !ArgUtility.TryGetOptionalInt(query, 2, out num1, out error, name: "int dayOffset"))
        return GameStateQuery.Helpers.ErrorResult(query, error);
      switch (str?.ToLower())
      {
        case null:
        case "any":
          str = (string) null;
          break;
        case "here":
        case "target":
          str = GameStateQuery.Helpers.RequireLocation(str, context.Location).GetLocationContextId();
          break;
      }
      int num2 = (Game1.Date.TotalDays + num1) % 112 /*0x70*/;
      return Utility.isFestivalDay(num2 % 28 + 1, (Season) (num2 / 28), str);
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool IS_PASSIVE_FESTIVAL_OPEN(string[] query, GameStateQueryContext context)
    {
      string festivalId;
      string error;
      return !ArgUtility.TryGet(query, 1, out festivalId, out error, name: "string festivalId") ? GameStateQuery.Helpers.ErrorResult(query, error) : Utility.IsPassiveFestivalOpen(festivalId);
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool IS_PASSIVE_FESTIVAL_TODAY(string[] query, GameStateQueryContext context)
    {
      string festivalId;
      string error;
      return !ArgUtility.TryGet(query, 1, out festivalId, out error, name: "string festivalId") ? GameStateQuery.Helpers.ErrorResult(query, error) : Utility.IsPassiveFestivalDay(festivalId);
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool SEASON(string[] query, GameStateQueryContext context)
    {
      for (int index = 1; index < query.Length; ++index)
      {
        Season season;
        string error;
        if (!ArgUtility.TryGetEnum<Season>(query, index, out season, out error, "Season season"))
          return GameStateQuery.Helpers.ErrorResult(query, error);
        if (Game1.season == season)
          return true;
      }
      return false;
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool YEAR(string[] query, GameStateQueryContext context)
    {
      int num1;
      string error;
      int num2;
      if (!ArgUtility.TryGetInt(query, 1, out num1, out error, "int minYear") || !ArgUtility.TryGetOptionalInt(query, 2, out num2, out error, int.MaxValue, "int maxYear"))
        return GameStateQuery.Helpers.ErrorResult(query, error);
      int year = Game1.year;
      return year >= num1 && year <= num2;
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool TIME(string[] query, GameStateQueryContext context)
    {
      int num1;
      string error;
      int num2;
      if (!ArgUtility.TryGetInt(query, 1, out num1, out error, "int minTime") || !ArgUtility.TryGetOptionalInt(query, 2, out num2, out error, int.MaxValue, "int maxTime"))
        return GameStateQuery.Helpers.ErrorResult(query, error);
      int timeOfDay = Game1.timeOfDay;
      return timeOfDay >= num1 && timeOfDay <= num2;
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    [OtherNames(new string[] {"EVENT_ID"})]
    public static bool IS_EVENT(string[] query, GameStateQueryContext context)
    {
      Event @event = Game1.CurrentEvent;
      if (@event == null)
        return false;
      return query.Length == 1 || GameStateQuery.Helpers.AnyArgMatches(query, 1, (Func<string, bool?>) (eventId => new bool?(eventId == @event.id)));
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool CAN_BUILD_CABIN(string[] query, GameStateQueryContext context)
    {
      int buildingsConstructed = Game1.GetNumberBuildingsConstructed("Cabin");
      return Game1.IsMasterGame && buildingsConstructed < Game1.CurrentPlayerLimit - 1;
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool CAN_BUILD_FOR_CABINS(string[] query, GameStateQueryContext context)
    {
      string name;
      string error;
      if (!ArgUtility.TryGet(query, 1, out name, out error, name: "string buildingType"))
        return GameStateQuery.Helpers.ErrorResult(query, error);
      int buildingsConstructed = Game1.GetNumberBuildingsConstructed("Cabin");
      return Game1.GetNumberBuildingsConstructed(name) < buildingsConstructed + 1;
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool BUILDINGS_CONSTRUCTED(string[] query, GameStateQueryContext context)
    {
      string str1;
      string error;
      string str2;
      int num1;
      int num2;
      bool includeUnderConstruction;
      if (!ArgUtility.TryGet(query, 1, out str1, out error, name: "string locationFilter") || !ArgUtility.TryGetOptional(query, 2, out str2, out error, "All", name: "string buildingType") || !ArgUtility.TryGetOptionalInt(query, 3, out num1, out error, 1, "int minCount") || !ArgUtility.TryGetOptionalInt(query, 4, out num2, out error, int.MaxValue, "int maxCount") || !ArgUtility.TryGetOptionalBool(query, 5, out includeUnderConstruction, out error, name: "bool includeUnderConstruction"))
        return GameStateQuery.Helpers.ErrorResult(query, error);
      bool flag1 = str1.EqualsIgnoreCase("All");
      bool flag2 = str2.EqualsIgnoreCase("All");
      GameLocation location = context.Location;
      if (!flag1)
      {
        location = GameStateQuery.Helpers.GetLocation(str1, location);
        if (location == null)
          return GameStateQuery.Helpers.ErrorResult(query, $"required index 2 has value '{str1}', which doesn't match an existing location name or one of the special keys (All, Here, or Target)");
      }
      int num3 = !flag1 ? (flag2 ? location.getNumberBuildingsConstructed(includeUnderConstruction) : location.getNumberBuildingsConstructed(str2, includeUnderConstruction)) : (flag2 ? Game1.GetNumberBuildingsConstructed(includeUnderConstruction) : Game1.GetNumberBuildingsConstructed(str2, includeUnderConstruction));
      return num3 >= num1 && num3 <= num2;
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool FARM_CAVE(string[] query, GameStateQueryContext context)
    {
      string error;
      if (!ArgUtility.TryGet(query, 1, out string _, out error, name: "_"))
        return GameStateQuery.Helpers.ErrorResult(query, error);
      string caveType;
      switch (Game1.MasterPlayer.caveChoice.Value)
      {
        case 1:
          caveType = "Bats";
          break;
        case 2:
          caveType = "Mushrooms";
          break;
        default:
          caveType = "None";
          break;
      }
      return GameStateQuery.Helpers.AnyArgMatches(query, 1, (Func<string, bool?>) (rawCaveType => new bool?(rawCaveType.EqualsIgnoreCase(caveType))));
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool FARM_NAME(string[] query, GameStateQueryContext context)
    {
      string str;
      string error;
      return !ArgUtility.TryGetRemainder(query, 1, out str, out error, name: "string farmName") ? GameStateQuery.Helpers.ErrorResult(query, error) : context.Player.farmName.Value.EqualsIgnoreCase(str);
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool FARM_TYPE(string[] query, GameStateQueryContext context)
    {
      string error;
      if (!ArgUtility.TryGet(query, 1, out string _, out error, name: "_"))
        return GameStateQuery.Helpers.ErrorResult(query, error);
      string farmTypeId = Game1.GetFarmTypeID();
      string farmTypeKey = Game1.GetFarmTypeKey();
      return GameStateQuery.Helpers.AnyArgMatches(query, 1, (Func<string, bool?>) (rawFarmType => new bool?(rawFarmType.EqualsIgnoreCase(farmTypeId) || rawFarmType.EqualsIgnoreCase(farmTypeKey))));
    }

    /// <summary>Get whether all the Lost Books for the library have been found.</summary>
    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool FOUND_ALL_LOST_BOOKS(string[] query, GameStateQueryContext context)
    {
      return Game1.netWorldState.Value.LostBooksFound >= 21;
    }

    /// <summary>Get whether an explicit target location is set.</summary>
    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool HAS_TARGET_LOCATION(string[] query, GameStateQueryContext context)
    {
      return context.ExplicitTargetLocation != null;
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool IS_COMMUNITY_CENTER_COMPLETE(string[] query, GameStateQueryContext context)
    {
      return Game1.MasterPlayer.hasCompletedCommunityCenter() && !Game1.MasterPlayer.mailReceived.Contains("JojaMember");
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool IS_CUSTOM_FARM_TYPE(string[] query, GameStateQueryContext context)
    {
      return Game1.whichFarm == 7;
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool IS_HOST(string[] query, GameStateQueryContext context) => Game1.IsMasterGame;

    /// <summary>Get whether the <see cref="T:StardewValley.Locations.IslandNorth" /> bridge is fixed.</summary>
    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool IS_ISLAND_NORTH_BRIDGE_FIXED(string[] query, GameStateQueryContext context)
    {
      IslandNorth locationFromName = (IslandNorth) Game1.getLocationFromName("IslandNorth");
      return locationFromName != null && locationFromName.bridgeFixed.Value;
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool IS_JOJA_MART_COMPLETE(string[] query, GameStateQueryContext context)
    {
      return Utility.hasFinishedJojaRoute();
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool IS_MULTIPLAYER(string[] query, GameStateQueryContext context)
    {
      return Game1.IsMultiplayer;
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool IS_VISITING_ISLAND(string[] query, GameStateQueryContext context)
    {
      string npc_name;
      string error;
      return !ArgUtility.TryGet(query, 1, out npc_name, out error, name: "string npcName") ? GameStateQuery.Helpers.ErrorResult(query, error) : Game1.IsVisitingIslandToday(npc_name);
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool LOCATION_ACCESSIBLE(string[] query, GameStateQueryContext context)
    {
      GameLocation location = context.Location;
      string error;
      return !GameStateQuery.Helpers.TryGetLocationArg(query, 1, ref location, out error) ? GameStateQuery.Helpers.ErrorResult(query, error) : Game1.isLocationAccessible(location.NameOrUniqueName);
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool LOCATION_CONTEXT(string[] query, GameStateQueryContext context)
    {
      GameLocation location = context.Location;
      string error;
      if (!GameStateQuery.Helpers.TryGetLocationArg(query, 1, ref location, out error) || !ArgUtility.TryGet(query, 2, out string _, out error, name: "_"))
        return GameStateQuery.Helpers.ErrorResult(query, error);
      string contextId = location.GetLocationContextId();
      return GameStateQuery.Helpers.AnyArgMatches(query, 2, (Func<string, bool?>) (rawContextId => new bool?(rawContextId.EqualsIgnoreCase(contextId))));
    }

    /// <summary>Get whether a location has a given value in its <see cref="F:StardewValley.GameData.Locations.LocationData.CustomFields" /> data field. This expects <c>LOCATION_HAS_CUSTOM_FIELD &lt;location&gt; &lt;field key&gt; [expected value]</c>, where omitting the expected value will just check if the field is defined.</summary>
    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool LOCATION_HAS_CUSTOM_FIELD(string[] query, GameStateQueryContext context)
    {
      GameLocation location = context.Location;
      string error;
      string key;
      string str1;
      if (!GameStateQuery.Helpers.TryGetLocationArg(query, 1, ref location, out error) || !ArgUtility.TryGet(query, 2, out key, out error, false, "string fieldKey") || !ArgUtility.TryGetOptional(query, 3, out str1, out error, name: "string value"))
        return GameStateQuery.Helpers.ErrorResult(query, error);
      bool flag = ArgUtility.HasIndex<string>(query, 3);
      LocationData data = location?.GetData();
      string str2;
      if (data?.CustomFields == null || !data.CustomFields.TryGetValue(key, out str2))
        return false;
      return !flag || str2 == str1;
    }

    /// <summary>Get whether a location is indoors.</summary>
    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool LOCATION_IS_INDOORS(string[] query, GameStateQueryContext context)
    {
      GameLocation location = context.Location;
      string error;
      if (!GameStateQuery.Helpers.TryGetLocationArg(query, 1, ref location, out error))
        return GameStateQuery.Helpers.ErrorResult(query, error);
      bool? isOutdoors = location?.IsOutdoors;
      return isOutdoors.HasValue && !isOutdoors.GetValueOrDefault();
    }

    /// <summary>Get whether a location is outdoors.</summary>
    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool LOCATION_IS_OUTDOORS(string[] query, GameStateQueryContext context)
    {
      GameLocation location = context.Location;
      string error;
      return !GameStateQuery.Helpers.TryGetLocationArg(query, 1, ref location, out error) ? GameStateQuery.Helpers.ErrorResult(query, error) : location?.IsOutdoors ?? false;
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool LOCATION_IS_MINES(string[] query, GameStateQueryContext context)
    {
      GameLocation location = context.Location;
      string error;
      return !GameStateQuery.Helpers.TryGetLocationArg(query, 1, ref location, out error) ? GameStateQuery.Helpers.ErrorResult(query, error) : location is MineShaft;
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool LOCATION_IS_SKULL_CAVE(string[] query, GameStateQueryContext context)
    {
      GameLocation location = context.Location;
      string error;
      if (!GameStateQuery.Helpers.TryGetLocationArg(query, 1, ref location, out error))
        return GameStateQuery.Helpers.ErrorResult(query, error);
      return location is MineShaft mineShaft && mineShaft.mineLevel >= 121 && mineShaft.mineLevel != 77377;
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool LOCATION_NAME(string[] query, GameStateQueryContext context)
    {
      GameLocation location = context.Location;
      string error;
      if (!GameStateQuery.Helpers.TryGetLocationArg(query, 1, ref location, out error) || !ArgUtility.TryGet(query, 2, out string _, out error, name: "_"))
        return GameStateQuery.Helpers.ErrorResult(query, error);
      return location != null && GameStateQuery.Helpers.AnyArgMatches(query, 2, (Func<string, bool?>) (rawName => new bool?(rawName.EqualsIgnoreCase(location.Name))));
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool LOCATION_UNIQUE_NAME(string[] query, GameStateQueryContext context)
    {
      GameLocation location = context.Location;
      string error;
      if (!GameStateQuery.Helpers.TryGetLocationArg(query, 1, ref location, out error) || !ArgUtility.TryGet(query, 2, out string _, out error, name: "_"))
        return GameStateQuery.Helpers.ErrorResult(query, error);
      return location != null && GameStateQuery.Helpers.AnyArgMatches(query, 2, (Func<string, bool?>) (rawName => new bool?(rawName.EqualsIgnoreCase(location.NameOrUniqueName))));
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool LOCATION_SEASON(string[] query, GameStateQueryContext context)
    {
      GameLocation location = context.Location;
      string error;
      if (!GameStateQuery.Helpers.TryGetLocationArg(query, 1, ref location, out error))
        return GameStateQuery.Helpers.ErrorResult(query, error);
      string season = Game1.GetSeasonKeyForLocation(location);
      return GameStateQuery.Helpers.AnyArgMatches(query, 2, (Func<string, bool?>) (rawSeason => new bool?(rawSeason.EqualsIgnoreCase(season))));
    }

    /// <summary>Get whether a given number of items have been donated to the museum, optionally filtered by type. Format: <c>MUSEUM_DONATIONS &lt;min count&gt; [max count] [object type]+</c> or <c>MUSEUM_DONATIONS &lt;min count&gt; &lt;max count&gt; [object type]+</c>.</summary>
    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool MUSEUM_DONATIONS(string[] query, GameStateQueryContext context)
    {
      int num1 = 3;
      int num2;
      string error;
      if (!ArgUtility.TryGetInt(query, 1, out num2, out error, "int minCount"))
        return GameStateQuery.Helpers.ErrorResult(query, error);
      int maxValue;
      if (!ArgUtility.TryGetInt(query, 2, out maxValue, out string _, "int maxCount"))
      {
        num1 = 2;
        maxValue = int.MaxValue;
      }
      bool flag = query.Length > num1;
      int num3 = 0;
      foreach (string itemId in Game1.netWorldState.Value.MuseumPieces.Values)
      {
        if (flag)
        {
          ParsedItemData dataOrErrorItem = ItemRegistry.GetDataOrErrorItem(itemId);
          if (dataOrErrorItem.ObjectType != null)
          {
            for (int index = num1; index < query.Length; ++index)
            {
              if (dataOrErrorItem.ObjectType == query[index])
              {
                ++num3;
                break;
              }
            }
          }
        }
        else
          ++num3;
      }
      return num3 >= num2 && num3 <= maxValue;
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool WEATHER(string[] query, GameStateQueryContext context)
    {
      GameLocation location = context.Location;
      string error;
      if (!GameStateQuery.Helpers.TryGetLocationArg(query, 1, ref location, out error) || !ArgUtility.TryGet(query, 2, out string _, out error, name: "_"))
        return GameStateQuery.Helpers.ErrorResult(query, error);
      if (location == null)
        return false;
      string weather = location.GetWeather().Weather;
      return GameStateQuery.Helpers.AnyArgMatches(query, 2, (Func<string, bool?>) (rawWeather => new bool?(rawWeather.EqualsIgnoreCase(weather))));
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool WORLD_STATE_FIELD(string[] query, GameStateQueryContext context)
    {
      string name;
      string error;
      string str1;
      int num1;
      if (!ArgUtility.TryGet(query, 1, out name, out error, name: "string name") || !ArgUtility.TryGet(query, 2, out str1, out error, name: "string expectedValue") || !ArgUtility.TryGetOptionalInt(query, 3, out num1, out error, int.MaxValue, "int maxValue"))
        return GameStateQuery.Helpers.ErrorResult(query, error);
      PropertyInfo property = typeof (NetWorldState).GetProperty(name, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
      if ((object) property == null)
        return false;
      object obj = property.GetValue((object) Game1.netWorldState.Value, (object[]) null);
      switch (obj)
      {
        case null:
          return str1.EqualsIgnoreCase("null");
        case bool flag:
          bool result1;
          return bool.TryParse(str1, out result1) && flag == result1;
        case int num2:
          int result2;
          return int.TryParse(str1, out result2) && num2 >= result2 && num2 <= num1;
        case string str2:
          return str2.EqualsIgnoreCase(str1);
        default:
          return obj.ToString().EqualsIgnoreCase(str1);
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool WORLD_STATE_ID(string[] query, GameStateQueryContext context)
    {
      string error;
      return !ArgUtility.TryGet(query, 1, out string _, out error, name: "_") ? GameStateQuery.Helpers.ErrorResult(query, error) : GameStateQuery.Helpers.AnyArgMatches(query, 1, (Func<string, bool?>) (worldStateId => new bool?(NetWorldState.checkAnywhereForWorldStateID(worldStateId))));
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool MINE_LOWEST_LEVEL_REACHED(string[] query, GameStateQueryContext context)
    {
      int num1;
      string error;
      int num2;
      if (!ArgUtility.TryGetInt(query, 1, out num1, out error, "int minLevel") || !ArgUtility.TryGetOptionalInt(query, 2, out num2, out error, int.MaxValue, "int maxLevel"))
        return GameStateQuery.Helpers.ErrorResult(query, error);
      int lowestLevelReached = MineShaft.lowestLevelReached;
      return lowestLevelReached >= num1 && lowestLevelReached <= num2;
    }

    /// <summary>Get whether a player has the given combat level, excluding buffs.</summary>
    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool PLAYER_BASE_COMBAT_LEVEL(string[] query, GameStateQueryContext context)
    {
      return GameStateQuery.Helpers.PlayerSkillLevelImpl(query, context.Player, (Func<Farmer, int>) (target => target.combatLevel.Value));
    }

    /// <summary>Get whether a player has the given farming level, excluding buffs.</summary>
    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool PLAYER_BASE_FARMING_LEVEL(string[] query, GameStateQueryContext context)
    {
      return GameStateQuery.Helpers.PlayerSkillLevelImpl(query, context.Player, (Func<Farmer, int>) (target => target.farmingLevel.Value));
    }

    /// <summary>Get whether a player has the given fishing level, excluding buffs.</summary>
    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool PLAYER_BASE_FISHING_LEVEL(string[] query, GameStateQueryContext context)
    {
      return GameStateQuery.Helpers.PlayerSkillLevelImpl(query, context.Player, (Func<Farmer, int>) (target => target.fishingLevel.Value));
    }

    /// <summary>Get whether a player has the given foraging level, excluding buffs.</summary>
    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool PLAYER_BASE_FORAGING_LEVEL(string[] query, GameStateQueryContext context)
    {
      return GameStateQuery.Helpers.PlayerSkillLevelImpl(query, context.Player, (Func<Farmer, int>) (target => target.foragingLevel.Value));
    }

    /// <summary>Get whether a player has the given luck level, excluding buffs.</summary>
    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool PLAYER_BASE_LUCK_LEVEL(string[] query, GameStateQueryContext context)
    {
      return GameStateQuery.Helpers.PlayerSkillLevelImpl(query, context.Player, (Func<Farmer, int>) (target => target.luckLevel.Value));
    }

    /// <summary>Get whether a player has the given mining level, excluding buffs.</summary>
    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool PLAYER_BASE_MINING_LEVEL(string[] query, GameStateQueryContext context)
    {
      return GameStateQuery.Helpers.PlayerSkillLevelImpl(query, context.Player, (Func<Farmer, int>) (target => target.miningLevel.Value));
    }

    /// <summary>Get whether a player has the given combat level, including any buffs which increase it.</summary>
    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool PLAYER_COMBAT_LEVEL(string[] query, GameStateQueryContext context)
    {
      return GameStateQuery.Helpers.PlayerSkillLevelImpl(query, context.Player, (Func<Farmer, int>) (target => target.CombatLevel));
    }

    /// <summary>Get whether a player has the given farming level, including any buffs which increase it.</summary>
    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool PLAYER_FARMING_LEVEL(string[] query, GameStateQueryContext context)
    {
      return GameStateQuery.Helpers.PlayerSkillLevelImpl(query, context.Player, (Func<Farmer, int>) (target => target.FarmingLevel));
    }

    /// <summary>Get whether a player has the given fishing level, including any buffs which increase it.</summary>
    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool PLAYER_FISHING_LEVEL(string[] query, GameStateQueryContext context)
    {
      return GameStateQuery.Helpers.PlayerSkillLevelImpl(query, context.Player, (Func<Farmer, int>) (target => target.FishingLevel));
    }

    /// <summary>Get whether a player has the given foraging level, including any buffs which increase it.</summary>
    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool PLAYER_FORAGING_LEVEL(string[] query, GameStateQueryContext context)
    {
      return GameStateQuery.Helpers.PlayerSkillLevelImpl(query, context.Player, (Func<Farmer, int>) (target => target.ForagingLevel));
    }

    /// <summary>Get whether a player has the given luck level, including any buffs which increase it.</summary>
    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool PLAYER_LUCK_LEVEL(string[] query, GameStateQueryContext context)
    {
      return GameStateQuery.Helpers.PlayerSkillLevelImpl(query, context.Player, (Func<Farmer, int>) (target => target.LuckLevel));
    }

    /// <summary>Get whether a player has the given mining level, including any buffs which increase it.</summary>
    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool PLAYER_MINING_LEVEL(string[] query, GameStateQueryContext context)
    {
      return GameStateQuery.Helpers.PlayerSkillLevelImpl(query, context.Player, (Func<Farmer, int>) (target => target.MiningLevel));
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool PLAYER_CURRENT_MONEY(string[] query, GameStateQueryContext context)
    {
      string playerKey;
      string error;
      int minAmount;
      int maxAmount;
      return !ArgUtility.TryGet(query, 1, out playerKey, out error, name: "string playerKey") || !ArgUtility.TryGetInt(query, 2, out minAmount, out error, "int minAmount") || !ArgUtility.TryGetOptionalInt(query, 3, out maxAmount, out error, int.MaxValue, "int maxAmount") ? GameStateQuery.Helpers.ErrorResult(query, error) : GameStateQuery.Helpers.WithPlayer(context.Player, playerKey, (Func<Farmer, bool>) (target =>
      {
        int money = target.Money;
        return money >= minAmount && money <= maxAmount;
      }));
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool PLAYER_FARMHOUSE_UPGRADE(string[] query, GameStateQueryContext context)
    {
      string playerKey;
      string error;
      int minUpgradeLevel;
      int maxUpgradeLevel;
      return !ArgUtility.TryGet(query, 1, out playerKey, out error, name: "string playerKey") || !ArgUtility.TryGetInt(query, 2, out minUpgradeLevel, out error, "int minUpgradeLevel") || !ArgUtility.TryGetOptionalInt(query, 3, out maxUpgradeLevel, out error, int.MaxValue, "int maxUpgradeLevel") ? GameStateQuery.Helpers.ErrorResult(query, error) : GameStateQuery.Helpers.WithPlayer(context.Player, playerKey, (Func<Farmer, bool>) (target =>
      {
        int houseUpgradeLevel = target.HouseUpgradeLevel;
        return houseUpgradeLevel >= minUpgradeLevel && houseUpgradeLevel <= maxUpgradeLevel;
      }));
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool PLAYER_GENDER(string[] query, GameStateQueryContext context)
    {
      string playerKey;
      string error;
      string str;
      if (!ArgUtility.TryGet(query, 1, out playerKey, out error, name: "string playerKey") || !ArgUtility.TryGet(query, 2, out str, out error, name: "string genderName"))
        return GameStateQuery.Helpers.ErrorResult(query, error);
      bool isMale = str.EqualsIgnoreCase("Male");
      return GameStateQuery.Helpers.WithPlayer(context.Player, playerKey, (Func<Farmer, bool>) (target => target.IsMale == isMale));
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool PLAYER_HAS_ACHIEVEMENT(string[] query, GameStateQueryContext context)
    {
      string playerKey;
      string error;
      int achievementId;
      return !ArgUtility.TryGet(query, 1, out playerKey, out error, name: "string playerKey") || !ArgUtility.TryGetInt(query, 2, out achievementId, out error, "int achievementId") ? GameStateQuery.Helpers.ErrorResult(query, error) : GameStateQuery.Helpers.WithPlayer(context.Player, playerKey, (Func<Farmer, bool>) (target => target.achievements.Contains(achievementId)));
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool PLAYER_HAS_ALL_ACHIEVEMENTS(string[] query, GameStateQueryContext context)
    {
      string playerKey;
      string error;
      return !ArgUtility.TryGet(query, 1, out playerKey, out error, name: "string playerKey") ? GameStateQuery.Helpers.ErrorResult(query, error) : GameStateQuery.Helpers.WithPlayer(context.Player, playerKey, (Func<Farmer, bool>) (target =>
      {
        foreach (int key in Game1.achievements.Keys)
        {
          if (!target.achievements.Contains(key))
            return false;
        }
        return true;
      }));
    }

    /// <summary>Get whether a player has a given buff currently active.</summary>
    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool PLAYER_HAS_BUFF(string[] query, GameStateQueryContext context)
    {
      string playerKey;
      string error;
      return !ArgUtility.TryGet(query, 1, out playerKey, out error, name: "string playerKey") || !ArgUtility.TryGet(query, 2, out string _, out error, name: "string buffId") ? GameStateQuery.Helpers.ErrorResult(query, error) : GameStateQuery.Helpers.WithPlayer(context.Player, playerKey, (Func<Farmer, bool>) (target => GameStateQuery.Helpers.AnyArgMatches(query, 2, (Func<string, bool?>) (id => new bool?(target.buffs.IsApplied(id))))));
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool PLAYER_HAS_CAUGHT_FISH(string[] query, GameStateQueryContext context)
    {
      string playerKey;
      string error;
      string itemId;
      if (!ArgUtility.TryGet(query, 1, out playerKey, out error, name: "string playerKey") || !ArgUtility.TryGet(query, 2, out itemId, out error, name: "string fishId"))
        return GameStateQuery.Helpers.ErrorResult(query, error);
      return ItemRegistry.QualifyItemId(itemId) != null && GameStateQuery.Helpers.WithPlayer(context.Player, playerKey, (Func<Farmer, bool>) (target => GameStateQuery.Helpers.AnyArgMatches(query, 2, (Func<string, bool?>) (id => new bool?(target.fishCaught.ContainsKey(id))))));
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool PLAYER_HAS_CONVERSATION_TOPIC(string[] query, GameStateQueryContext context)
    {
      string playerKey;
      string error;
      return !ArgUtility.TryGet(query, 1, out playerKey, out error, name: "string playerKey") || !ArgUtility.TryGet(query, 2, out string _, out error, name: "string topic") ? GameStateQuery.Helpers.ErrorResult(query, error) : GameStateQuery.Helpers.WithPlayer(context.Player, playerKey, (Func<Farmer, bool>) (target => GameStateQuery.Helpers.AnyArgMatches(query, 2, (Func<string, bool?>) (id => new bool?(target.activeDialogueEvents.ContainsKey(id))))));
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool PLAYER_HAS_CRAFTING_RECIPE(string[] query, GameStateQueryContext context)
    {
      string playerKey;
      string error;
      string recipeName;
      return !ArgUtility.TryGet(query, 1, out playerKey, out error, name: "string playerKey") || !ArgUtility.TryGetRemainder(query, 2, out recipeName, out error, name: "string recipeName") ? GameStateQuery.Helpers.ErrorResult(query, error) : GameStateQuery.Helpers.WithPlayer(context.Player, playerKey, (Func<Farmer, bool>) (target => target.craftingRecipes.ContainsKey(recipeName)));
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool PLAYER_HAS_COOKING_RECIPE(string[] query, GameStateQueryContext context)
    {
      string playerKey;
      string error;
      string recipeName;
      return !ArgUtility.TryGet(query, 1, out playerKey, out error, name: "string playerKey") || !ArgUtility.TryGetRemainder(query, 2, out recipeName, out error, name: "string recipeName") ? GameStateQuery.Helpers.ErrorResult(query, error) : GameStateQuery.Helpers.WithPlayer(context.Player, playerKey, (Func<Farmer, bool>) (target => target.cookingRecipes.ContainsKey(recipeName)));
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool PLAYER_HAS_DIALOGUE_ANSWER(string[] query, GameStateQueryContext context)
    {
      string playerKey;
      string error;
      return !ArgUtility.TryGet(query, 1, out playerKey, out error, name: "string playerKey") || !ArgUtility.TryGet(query, 2, out string _, out error, name: "string responseId") ? GameStateQuery.Helpers.ErrorResult(query, error) : GameStateQuery.Helpers.WithPlayer(context.Player, playerKey, (Func<Farmer, bool>) (target => GameStateQuery.Helpers.AnyArgMatches(query, 2, (Func<string, bool?>) (id => new bool?(target.DialogueQuestionsAnswered.Contains(id))))));
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool PLAYER_HAS_HEARD_SONG(string[] query, GameStateQueryContext context)
    {
      string playerKey;
      string error;
      return !ArgUtility.TryGet(query, 1, out playerKey, out error, name: "string playerKey") || !ArgUtility.TryGet(query, 2, out string _, out error, name: "string songId") ? GameStateQuery.Helpers.ErrorResult(query, error) : GameStateQuery.Helpers.WithPlayer(context.Player, playerKey, (Func<Farmer, bool>) (target => GameStateQuery.Helpers.AnyArgMatches(query, 2, (Func<string, bool?>) (id => new bool?(target.songsHeard.Contains(id))))));
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool PLAYER_HAS_ITEM(string[] query, GameStateQueryContext context)
    {
      string playerKey;
      string error;
      string itemId;
      int minCount;
      int maxCount;
      return !ArgUtility.TryGet(query, 1, out playerKey, out error, name: "string playerKey") || !ArgUtility.TryGet(query, 2, out itemId, out error, name: "string itemId") || !ArgUtility.TryGetOptionalInt(query, 3, out minCount, out error, 1, "int minCount") || !ArgUtility.TryGetOptionalInt(query, 4, out maxCount, out error, int.MaxValue, "int maxCount") ? GameStateQuery.Helpers.ErrorResult(query, error) : GameStateQuery.Helpers.WithPlayer(context.Player, playerKey, (Func<Farmer, bool>) (target =>
      {
        switch (itemId)
        {
          case "73":
          case "(O)73":
            int goldenWalnuts = Game1.netWorldState.Value.GoldenWalnuts;
            return goldenWalnuts >= minCount && goldenWalnuts <= maxCount;
          case "858":
          case "(O)858":
            int qiGems = target.QiGems;
            return qiGems >= minCount && qiGems <= maxCount;
          default:
            if (maxCount == int.MaxValue)
              return target.Items.ContainsId(itemId, minCount);
            int num = target.Items.CountId(itemId);
            return num >= minCount && num <= maxCount;
        }
      }));
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool PLAYER_HAS_MAIL(string[] query, GameStateQueryContext context)
    {
      string playerKey;
      string error;
      string str1;
      string mailId;
      if (!ArgUtility.TryGet(query, 1, out playerKey, out error, name: "string playerKey") || !ArgUtility.TryGet(query, 2, out mailId, out error, name: "string mailId") || !ArgUtility.TryGetOptional(query, 3, out str1, out error, "any", name: "string rawType"))
        return GameStateQuery.Helpers.ErrorResult(query, error);
      string type = str1?.ToLower();
      string str2 = type;
      return !(str2 == "mailbox") && !(str2 == "tomorrow") && !(str2 == "received") && !(str2 == "any") ? GameStateQuery.Helpers.ErrorResult(query, $"unknown mail type '{type}'; expected 'Mailbox', 'Tomorrow', 'Received', or 'Any'") : GameStateQuery.Helpers.WithPlayer(context.Player, playerKey, (Func<Farmer, bool>) (target =>
      {
        switch (type)
        {
          case "mailbox":
            return target.mailbox.Contains(mailId);
          case "tomorrow":
            return target.mailForTomorrow.Contains(mailId);
          case "received":
            return target.mailReceived.Contains(mailId);
          default:
            return target.hasOrWillReceiveMail(mailId);
        }
      }));
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool PLAYER_HAS_PROFESSION(string[] query, GameStateQueryContext context)
    {
      string playerKey;
      string error;
      int professionId;
      return !ArgUtility.TryGet(query, 1, out playerKey, out error, name: "string playerKey") || !ArgUtility.TryGetInt(query, 2, out professionId, out error, "int professionId") ? GameStateQuery.Helpers.ErrorResult(query, error) : GameStateQuery.Helpers.WithPlayer(context.Player, playerKey, (Func<Farmer, bool>) (target => target.professions.Contains(professionId)));
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool PLAYER_HAS_RUN_TRIGGER_ACTION(string[] query, GameStateQueryContext context)
    {
      string playerKey;
      string error;
      return !ArgUtility.TryGet(query, 1, out playerKey, out error, name: "string playerKey") || !ArgUtility.TryGet(query, 2, out string _, out error, name: "string actionId") ? GameStateQuery.Helpers.ErrorResult(query, error) : GameStateQuery.Helpers.WithPlayer(context.Player, playerKey, (Func<Farmer, bool>) (target => GameStateQuery.Helpers.AnyArgMatches(query, 2, (Func<string, bool?>) (id => new bool?(target.triggerActionsRun.Contains(id))))));
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool PLAYER_HAS_SECRET_NOTE(string[] query, GameStateQueryContext context)
    {
      string playerKey;
      string error;
      int noteId;
      return !ArgUtility.TryGet(query, 1, out playerKey, out error, name: "string playerKey") || !ArgUtility.TryGetInt(query, 2, out noteId, out error, "int noteId") ? GameStateQuery.Helpers.ErrorResult(query, error) : GameStateQuery.Helpers.WithPlayer(context.Player, playerKey, (Func<Farmer, bool>) (target => target.secretNotesSeen.Contains(noteId)));
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool PLAYER_HAS_SEEN_EVENT(string[] query, GameStateQueryContext context)
    {
      string playerKey;
      string error;
      return !ArgUtility.TryGet(query, 1, out playerKey, out error, name: "string playerKey") || !ArgUtility.TryGet(query, 2, out string _, out error, name: "string eventId") ? GameStateQuery.Helpers.ErrorResult(query, error) : GameStateQuery.Helpers.WithPlayer(context.Player, playerKey, (Func<Farmer, bool>) (target => GameStateQuery.Helpers.AnyArgMatches(query, 2, (Func<string, bool?>) (id => new bool?(target.eventsSeen.Contains(id))))));
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool PLAYER_HAS_TOWN_KEY(string[] query, GameStateQueryContext context)
    {
      string playerKey;
      string error;
      return !ArgUtility.TryGet(query, 1, out playerKey, out error, name: "string playerKey") ? GameStateQuery.Helpers.ErrorResult(query, error) : GameStateQuery.Helpers.WithPlayer(context.Player, playerKey, (Func<Farmer, bool>) (target => target.HasTownKey));
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool PLAYER_HAS_TRASH_CAN_LEVEL(string[] query, GameStateQueryContext context)
    {
      string playerKey;
      string error;
      int minLevel;
      int maxLevel;
      return !ArgUtility.TryGet(query, 1, out playerKey, out error, name: "string playerKey") || !ArgUtility.TryGetInt(query, 2, out minLevel, out error, "int minLevel") || !ArgUtility.TryGetOptionalInt(query, 3, out maxLevel, out error, int.MaxValue, "int maxLevel") ? GameStateQuery.Helpers.ErrorResult(query, error) : GameStateQuery.Helpers.WithPlayer(context.Player, playerKey, (Func<Farmer, bool>) (target =>
      {
        int trashCanLevel = target.trashCanLevel;
        return trashCanLevel >= minLevel && trashCanLevel <= maxLevel;
      }));
    }

    /// <summary>Get whether a target player has a trinket equipped.</summary>
    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool PLAYER_HAS_TRINKET(string[] query, GameStateQueryContext context)
    {
      string playerKey;
      string error;
      return !ArgUtility.TryGet(query, 1, out playerKey, out error, name: "string playerKey") ? GameStateQuery.Helpers.ErrorResult(query, error) : GameStateQuery.Helpers.WithPlayer(context.Player, playerKey, (Func<Farmer, bool>) (target =>
      {
        foreach (Trinket trinketItem in target.trinketItems)
        {
          if (trinketItem != null)
          {
            for (int index = 2; index < query.Length; ++index)
            {
              if (trinketItem.QualifiedItemId == query[index] || trinketItem.ItemId == query[index])
                return true;
            }
          }
        }
        return false;
      }));
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool PLAYER_LOCATION_CONTEXT(string[] query, GameStateQueryContext context)
    {
      string playerKey;
      string error;
      return !ArgUtility.TryGet(query, 1, out playerKey, out error, name: "string playerKey") || !ArgUtility.TryGet(query, 2, out string _, out error, name: "_") ? GameStateQuery.Helpers.ErrorResult(query, error) : GameStateQuery.Helpers.WithPlayer(context.Player, playerKey, (Func<Farmer, bool>) (target =>
      {
        string contextId = target.currentLocation?.GetLocationContextId();
        return GameStateQuery.Helpers.AnyArgMatches(query, 2, (Func<string, bool?>) (rawContextId => new bool?(rawContextId.EqualsIgnoreCase(contextId))));
      }));
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool PLAYER_LOCATION_NAME(string[] query, GameStateQueryContext context)
    {
      string playerKey;
      string error;
      return !ArgUtility.TryGet(query, 1, out playerKey, out error, name: "string playerKey") || !ArgUtility.TryGet(query, 2, out string _, out error, name: "_") ? GameStateQuery.Helpers.ErrorResult(query, error) : GameStateQuery.Helpers.WithPlayer(context.Player, playerKey, (Func<Farmer, bool>) (target => GameStateQuery.Helpers.AnyArgMatches(query, 2, (Func<string, bool?>) (rawName => new bool?(rawName.EqualsIgnoreCase(target.currentLocation?.Name))))));
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool PLAYER_LOCATION_UNIQUE_NAME(string[] query, GameStateQueryContext context)
    {
      string playerKey;
      string error;
      return !ArgUtility.TryGet(query, 1, out playerKey, out error, name: "string playerKey") || !ArgUtility.TryGet(query, 2, out string _, out error, name: "_") ? GameStateQuery.Helpers.ErrorResult(query, error) : GameStateQuery.Helpers.WithPlayer(context.Player, playerKey, (Func<Farmer, bool>) (target => GameStateQuery.Helpers.AnyArgMatches(query, 2, (Func<string, bool?>) (rawName => new bool?(rawName.EqualsIgnoreCase(target.currentLocation?.NameOrUniqueName))))));
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool PLAYER_MOD_DATA(string[] query, GameStateQueryContext context)
    {
      string playerKey;
      string error;
      string key;
      string value;
      string str;
      return !ArgUtility.TryGet(query, 1, out playerKey, out error, name: "string playerKey") || !ArgUtility.TryGet(query, 2, out key, out error, name: "string key") || !ArgUtility.TryGet(query, 3, out value, out error, name: "string value") ? GameStateQuery.Helpers.ErrorResult(query, error) : GameStateQuery.Helpers.WithPlayer(context.Player, playerKey, (Func<Farmer, bool>) (target => target.modData.TryGetValue(key, out str) && str.EqualsIgnoreCase(value)));
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool PLAYER_MONEY_EARNED(string[] query, GameStateQueryContext context)
    {
      string playerKey;
      string error;
      int minAmount;
      int maxAmount;
      return !ArgUtility.TryGet(query, 1, out playerKey, out error, name: "string playerKey") || !ArgUtility.TryGetInt(query, 2, out minAmount, out error, "int minAmount") || !ArgUtility.TryGetOptionalInt(query, 3, out maxAmount, out error, int.MaxValue, "int maxAmount") ? GameStateQuery.Helpers.ErrorResult(query, error) : GameStateQuery.Helpers.WithPlayer(context.Player, playerKey, (Func<Farmer, bool>) (target =>
      {
        uint totalMoneyEarned = target.totalMoneyEarned;
        return (long) totalMoneyEarned >= (long) minAmount && (long) totalMoneyEarned <= (long) maxAmount;
      }));
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool PLAYER_SHIPPED_BASIC_ITEM(string[] query, GameStateQueryContext context)
    {
      string playerKey;
      string error;
      string itemId;
      int minShipped;
      int maxShipped;
      if (!ArgUtility.TryGet(query, 1, out playerKey, out error, name: "string playerKey") || !ArgUtility.TryGet(query, 2, out itemId, out error, name: "string itemId") || !ArgUtility.TryGetOptionalInt(query, 3, out minShipped, out error, 1, "int minShipped") || !ArgUtility.TryGetOptionalInt(query, 4, out maxShipped, out error, int.MaxValue, "int maxShipped"))
        return GameStateQuery.Helpers.ErrorResult(query, error);
      if (ItemRegistry.IsQualifiedItemId(itemId))
      {
        ItemMetadata metadata = ItemRegistry.GetMetadata(itemId);
        if (metadata?.TypeIdentifier != "(O)")
          return false;
        itemId = metadata.LocalItemId;
      }
      int num;
      return GameStateQuery.Helpers.WithPlayer(context.Player, playerKey, (Func<Farmer, bool>) (target => target.basicShipped.TryGetValue(itemId, out num) && num >= minShipped && num <= maxShipped));
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool PLAYER_SPECIAL_ORDER_ACTIVE(string[] query, GameStateQueryContext context)
    {
      string playerKey;
      string error;
      return !ArgUtility.TryGet(query, 1, out playerKey, out error, name: "string playerKey") || !ArgUtility.TryGet(query, 2, out string _, out error, name: "string orderId") ? GameStateQuery.Helpers.ErrorResult(query, error) : GameStateQuery.Helpers.WithPlayer(context.Player, playerKey, (Func<Farmer, bool>) (target => GameStateQuery.Helpers.AnyArgMatches(query, 2, (Func<string, bool?>) (id => new bool?(target.team.SpecialOrderActive(id))))));
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool PLAYER_SPECIAL_ORDER_RULE_ACTIVE(
      string[] query,
      GameStateQueryContext context)
    {
      string playerKey;
      string error;
      return !ArgUtility.TryGet(query, 1, out playerKey, out error, name: "string playerKey") || !ArgUtility.TryGet(query, 2, out string _, out error, name: "string ruleId") ? GameStateQuery.Helpers.ErrorResult(query, error) : GameStateQuery.Helpers.WithPlayer(context.Player, playerKey, (Func<Farmer, bool>) (target => GameStateQuery.Helpers.AnyArgMatches(query, 2, (Func<string, bool?>) (id => new bool?(target.team.SpecialOrderRuleActive(id))))));
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool PLAYER_SPECIAL_ORDER_COMPLETE(string[] query, GameStateQueryContext context)
    {
      string playerKey;
      string error;
      return !ArgUtility.TryGet(query, 1, out playerKey, out error, name: "string playerKey") || !ArgUtility.TryGet(query, 2, out string _, out error, name: "string orderId") ? GameStateQuery.Helpers.ErrorResult(query, error) : GameStateQuery.Helpers.WithPlayer(context.Player, playerKey, (Func<Farmer, bool>) (target => GameStateQuery.Helpers.AnyArgMatches(query, 2, (Func<string, bool?>) (id => new bool?(target.team.completedSpecialOrders.Contains(id))))));
    }

    /// <summary>Check the number of monsters killed by the player. Format: <c>PLAYER_KILLED_MONSTERS &lt;player&gt; &lt;monster name&gt;+ [min] [max]</c>.</summary>
    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool PLAYER_KILLED_MONSTERS(string[] query, GameStateQueryContext context)
    {
      List<string> monsterNames = new List<string>();
      int min = 1;
      string playerKey;
      string error;
      if (!ArgUtility.TryGet(query, 1, out playerKey, out error, name: "playerKey"))
        return GameStateQuery.Helpers.ErrorResult(query, error);
      int index = 2;
      while (index < query.Length)
      {
        string s = query[index];
        ++index;
        int result;
        if (int.TryParse(s, out result))
        {
          min = result;
          break;
        }
        monsterNames.Add(s);
      }
      int max;
      if (!ArgUtility.TryGetOptionalInt(query, index, out max, out error, int.MaxValue, "max"))
        return GameStateQuery.Helpers.ErrorResult(query, error);
      return monsterNames.Count == 0 ? GameStateQuery.Helpers.ErrorResult(query, "must specify at least one monster name to count") : GameStateQuery.Helpers.WithPlayer(context.Player, playerKey, (Func<Farmer, bool>) (target =>
      {
        int num = 0;
        foreach (string name in monsterNames)
          num += target.stats.getMonstersKilled(name);
        return num >= min && num <= max;
      }));
    }

    /// <summary>Get whether the given player has a minimum value for a <see cref="P:StardewValley.Game1.stats" /> field returned by <see cref="M:StardewValley.Stats.Get(System.String)" />.</summary>
    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool PLAYER_STAT(string[] query, GameStateQueryContext context)
    {
      string playerKey;
      string error;
      string statName;
      int minValue;
      int maxValue;
      return !ArgUtility.TryGet(query, 1, out playerKey, out error, name: "string playerKey") || !ArgUtility.TryGet(query, 2, out statName, out error, name: "string statName") || !ArgUtility.TryGetInt(query, 3, out minValue, out error, "int minValue") || !ArgUtility.TryGetOptionalInt(query, 4, out maxValue, out error, int.MaxValue, "int maxValue") ? GameStateQuery.Helpers.ErrorResult(query, error) : GameStateQuery.Helpers.WithPlayer(context.Player, playerKey, (Func<Farmer, bool>) (target =>
      {
        uint num = target.stats.Get(statName);
        return (long) num >= (long) minValue && (long) num <= (long) maxValue;
      }));
    }

    /// <summary>Get whether the given player has ever visited a location name.</summary>
    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool PLAYER_VISITED_LOCATION(string[] query, GameStateQueryContext context)
    {
      string playerKey;
      string error;
      return !ArgUtility.TryGet(query, 1, out playerKey, out error, name: "string playerKey") || !ArgUtility.TryGet(query, 2, out string _, out error, name: "_") ? GameStateQuery.Helpers.ErrorResult(query, error) : GameStateQuery.Helpers.WithPlayer(context.Player, playerKey, (Func<Farmer, bool>) (target => GameStateQuery.Helpers.AnyArgMatches(query, 2, (Func<string, bool?>) (locationName => new bool?(target.locationsVisited.Contains(locationName))))));
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool PLAYER_FRIENDSHIP_POINTS(string[] query, GameStateQueryContext context)
    {
      string playerKey;
      string error;
      int minPoints;
      int maxPoints;
      string npcName;
      if (!ArgUtility.TryGet(query, 1, out playerKey, out error, name: "string playerKey") || !ArgUtility.TryGet(query, 2, out npcName, out error, name: "string npcName") || !ArgUtility.TryGetInt(query, 3, out minPoints, out error, "int minPoints") || !ArgUtility.TryGetOptionalInt(query, 4, out maxPoints, out error, int.MaxValue, "int maxPoints"))
        return GameStateQuery.Helpers.ErrorResult(query, error);
      bool isAny = npcName.EqualsIgnoreCase("Any");
      bool isAnyDateable = !isAny && npcName.EqualsIgnoreCase("AnyDateable");
      return GameStateQuery.Helpers.WithPlayer(context.Player, playerKey, (Func<Farmer, bool>) (target =>
      {
        if (isAny)
          return target.hasAFriendWithFriendshipPoints(minPoints, false, maxPoints);
        if (isAnyDateable)
          return target.hasAFriendWithFriendshipPoints(minPoints, true, maxPoints);
        int friendshipLevelForNpc = target.getFriendshipLevelForNPC(npcName);
        return friendshipLevelForNpc >= minPoints && friendshipLevelForNpc <= maxPoints;
      }));
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool PLAYER_HAS_CHILDREN(string[] query, GameStateQueryContext context)
    {
      string playerKey;
      string error;
      int minCount;
      int maxCount;
      return !ArgUtility.TryGet(query, 1, out playerKey, out error, name: "string playerKey") || !ArgUtility.TryGetOptionalInt(query, 2, out minCount, out error, 1, "int minCount") || !ArgUtility.TryGetOptionalInt(query, 3, out maxCount, out error, int.MaxValue, "int maxCount") ? GameStateQuery.Helpers.ErrorResult(query, error) : GameStateQuery.Helpers.WithPlayer(context.Player, playerKey, (Func<Farmer, bool>) (target =>
      {
        int childrenCount = target.getChildrenCount();
        return childrenCount >= minCount && childrenCount <= maxCount;
      }));
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool PLAYER_HAS_PET(string[] query, GameStateQueryContext context)
    {
      string playerKey;
      string error;
      return !ArgUtility.TryGet(query, 1, out playerKey, out error, name: "string playerKey") ? GameStateQuery.Helpers.ErrorResult(query, error) : GameStateQuery.Helpers.WithPlayer(context.Player, playerKey, (Func<Farmer, bool>) (target => target.hasPet()));
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool PLAYER_HEARTS(string[] query, GameStateQueryContext context)
    {
      string playerKey;
      string error;
      int minHearts;
      int maxHearts;
      string npcName;
      if (!ArgUtility.TryGet(query, 1, out playerKey, out error, name: "string playerKey") || !ArgUtility.TryGet(query, 2, out npcName, out error, name: "string npcName") || !ArgUtility.TryGetInt(query, 3, out minHearts, out error, "int minHearts") || !ArgUtility.TryGetOptionalInt(query, 4, out maxHearts, out error, int.MaxValue, "int maxHearts"))
        return GameStateQuery.Helpers.ErrorResult(query, error);
      bool isAny = npcName.EqualsIgnoreCase("Any");
      bool isAnyDateable = !isAny && npcName.EqualsIgnoreCase("AnyDateable");
      return GameStateQuery.Helpers.WithPlayer(context.Player, playerKey, (Func<Farmer, bool>) (target =>
      {
        if (isAny)
          return target.hasAFriendWithHeartLevel(minHearts, false, maxHearts);
        if (isAnyDateable)
          return target.hasAFriendWithHeartLevel(minHearts, true, maxHearts);
        int heartLevelForNpc = target.getFriendshipHeartLevelForNPC(npcName);
        return heartLevelForNpc >= minHearts && heartLevelForNpc <= maxHearts;
      }));
    }

    /// <summary>Get whether a player has ever talked to an NPC. Format: <c>PLAYER_HAS_MET &lt;player&gt; &lt;npc&gt;</c>.</summary>
    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool PLAYER_HAS_MET(string[] query, GameStateQueryContext context)
    {
      string playerKey;
      string error;
      return !ArgUtility.TryGet(query, 1, out playerKey, out error, name: "string playerKey") || !ArgUtility.TryGet(query, 2, out string _, out error, name: "string npcName") ? GameStateQuery.Helpers.ErrorResult(query, error) : GameStateQuery.Helpers.WithPlayer(context.Player, playerKey, (Func<Farmer, bool>) (target => GameStateQuery.Helpers.AnyArgMatches(query, 2, (Func<string, bool?>) (name => new bool?(target.friendshipData.ContainsKey(name))))));
    }

    /// <summary>Get a player's relationship status with an NPC. Format: <c>PLAYER_NPC_RELATIONSHIP &lt;player&gt; &lt;npc&gt; &lt;type&gt;+</c>, where the type is any combination of 'Friendly', 'Dating', 'Engaged', 'Roommate', 'Married' or 'Divorced'.</summary>
    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool PLAYER_NPC_RELATIONSHIP(string[] query, GameStateQueryContext context)
    {
      string playerKey;
      string error;
      string npcName;
      if (!ArgUtility.TryGet(query, 1, out playerKey, out error, false, "string playerKey") || !ArgUtility.TryGet(query, 2, out npcName, out error, false, "string npcName"))
        return GameStateQuery.Helpers.ErrorResult(query, error);
      string[] relationships = new string[query.Length - 3];
      string lower;
      for (int index = 3; index < query.Length && ArgUtility.TryGet(query, index, out lower, out error, false, "string type"); ++index)
      {
        lower = lower.ToLower();
        relationships[index - 3] = lower;
        if (!(lower == "friendly") && !(lower == "roommate") && !(lower == "dating") && !(lower == "engaged") && !(lower == "married") && !(lower == "divorced"))
          return GameStateQuery.Helpers.ErrorResult(query, $"unknown relationship type '{lower}'; expected one of Friendly, Roommate, Dating, Engaged, Married, or Divorced");
      }
      if (relationships.Length == 0)
        return GameStateQuery.Helpers.ErrorResult(query, ArgUtility.GetMissingRequiredIndexError(query, 3, "type"));
      bool anyNpc = npcName.EqualsIgnoreCase("Any");
      return GameStateQuery.Helpers.WithPlayer(context.Player, playerKey, (Func<Farmer, bool>) (target =>
      {
        if (anyNpc)
        {
          foreach (Friendship friendship in target.friendshipData.Values)
          {
            if (IsMatch(friendship, relationships))
              return true;
          }
        }
        else
        {
          Friendship friendship;
          if (target.friendshipData.TryGetValue(npcName, out friendship) && IsMatch(friendship, relationships))
            return true;
        }
        return false;
      }));

      bool IsMatch(Friendship friendship, string[] relationshipTypes)
      {
        foreach (string relationshipType in relationshipTypes)
        {
          switch (relationshipType)
          {
            case "friendly":
              if (friendship.Status == FriendshipStatus.Friendly)
                return true;
              break;
            case "roommate":
              if (friendship.Status == FriendshipStatus.Married && friendship.RoommateMarriage)
                return true;
              break;
            case "dating":
              if (friendship.Status == FriendshipStatus.Dating)
                return true;
              break;
            case "engaged":
              if (friendship.Status == FriendshipStatus.Engaged)
                return true;
              break;
            case "married":
              if (friendship.Status == FriendshipStatus.Married && !friendship.RoommateMarriage)
                return true;
              break;
            case "divorced":
              if (friendship.Status == FriendshipStatus.Divorced && !friendship.RoommateMarriage)
                return true;
              break;
            default:
              return GameStateQuery.Helpers.ErrorResult(query, $"unhandled relationship type '{relationshipType}'");
          }
        }
        return false;
      }
    }

    /// <summary>Get a player's relationship status with another player. Format: <c>PLAYER_PLAYER_RELATIONSHIP &lt;player 1&gt; &lt;player 2&gt; &lt;type&gt;+</c>, where the type is 'Engaged' or 'Married'.</summary>
    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool PLAYER_PLAYER_RELATIONSHIP(string[] query, GameStateQueryContext context)
    {
      string playerKey;
      string error;
      string targetPlayerKey;
      string type;
      if (!ArgUtility.TryGet(query, 1, out playerKey, out error, false, "string playerKey") || !ArgUtility.TryGet(query, 2, out targetPlayerKey, out error, false, "string targetPlayerKey") || !ArgUtility.TryGet(query, 3, out type, out error, false, "string type"))
        return GameStateQuery.Helpers.ErrorResult(query, error);
      type = type.ToLower();
      string str = type;
      return !(str == "friendly") && !(str == "engaged") && !(str == "married") ? GameStateQuery.Helpers.ErrorResult(query, $"unknown relationship type '{type}'; expected one of Friendly, Engaged, or Married") : GameStateQuery.Helpers.WithPlayer(context.Player, playerKey, (Func<Farmer, bool>) (fromPlayer => GameStateQuery.Helpers.WithPlayer(context.Player, targetPlayerKey, (Func<Farmer, bool>) (toPlayer =>
      {
        FriendshipStatus status = fromPlayer.team.GetFriendship(fromPlayer.UniqueMultiplayerID, toPlayer.UniqueMultiplayerID).Status;
        switch (type)
        {
          case "friendly":
            return status != FriendshipStatus.Engaged && status != FriendshipStatus.Married;
          case "engaged":
            return status == FriendshipStatus.Engaged;
          case "married":
            return status == FriendshipStatus.Married;
          default:
            return GameStateQuery.Helpers.ErrorResult(query, $"unhandled relationship type '{type}'");
        }
      }))));
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool PLAYER_PREFERRED_PET(string[] query, GameStateQueryContext context)
    {
      string playerKey;
      string error;
      return !ArgUtility.TryGet(query, 1, out playerKey, out error, name: "string playerKey") || !ArgUtility.TryGet(query, 2, out string _, out error, name: "_") ? GameStateQuery.Helpers.ErrorResult(query, error) : GameStateQuery.Helpers.WithPlayer(context.Player, playerKey, (Func<Farmer, bool>) (target => GameStateQuery.Helpers.AnyArgMatches(query, 2, (Func<string, bool?>) (rawPetId => new bool?(rawPetId.EqualsIgnoreCase(target.whichPetType))))));
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool RANDOM(string[] query, GameStateQueryContext context)
    {
      return GameStateQuery.Helpers.RandomImpl(context.Random, query, 1);
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool SYNCED_CHOICE(string[] query, GameStateQueryContext context)
    {
      string interval;
      string error;
      string key;
      int minValue;
      int num;
      Random random;
      if (!ArgUtility.TryGet(query, 1, out interval, out error, name: "string interval") || !ArgUtility.TryGet(query, 2, out key, out error, name: "string key") || !ArgUtility.TryGetInt(query, 3, out minValue, out error, "int min") || !ArgUtility.TryGetInt(query, 4, out num, out error, "int max") || !Utility.TryCreateIntervalRandom(interval, key, out random, out error))
        return GameStateQuery.Helpers.ErrorResult(query, error);
      string str = random.Next(minValue, num + 1).ToString();
      for (int index = 5; index < query.Length; ++index)
      {
        if (query[index] == str)
          return true;
      }
      return false;
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool SYNCED_RANDOM(string[] query, GameStateQueryContext context)
    {
      string interval;
      string error;
      string key;
      Random random;
      return !ArgUtility.TryGet(query, 1, out interval, out error, name: "string interval") || !ArgUtility.TryGet(query, 2, out key, out error, name: "string key") || !Utility.TryCreateIntervalRandom(interval, key, out random, out error) ? GameStateQuery.Helpers.ErrorResult(query, error) : GameStateQuery.Helpers.RandomImpl(random, query, 3);
    }

    /// <summary>A custom variant of <see cref="M:StardewValley.GameStateQuery.DefaultResolvers.SYNCED_RANDOM(System.String[],StardewValley.Delegates.GameStateQueryContext)" /> with a set key and which applies a chance boost for each day after summer starts.</summary>
    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool SYNCED_SUMMER_RAIN_RANDOM(string[] query, GameStateQueryContext context)
    {
      return Utility.CreateDaySaveRandom((double) Game1.hash.GetDeterministicHashCode("summer_rain_chance")).NextBool((float) (0.11999999731779099 + (double) Game1.dayOfMonth * (3.0 / 1000.0)));
    }

    /// <summary>Get whether the target item has all of the given context tags.</summary>
    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool ITEM_CONTEXT_TAG(string[] query, GameStateQueryContext context)
    {
      Item obj;
      string error;
      if (!GameStateQuery.Helpers.TryGetItemArg(query, 1, context.TargetItem, context.InputItem, out obj, out error))
        return GameStateQuery.Helpers.ErrorResult(query, error);
      if (obj == null)
        return false;
      for (int index = 2; index < query.Length; ++index)
      {
        if (!obj.HasContextTag(query[index]))
          return false;
      }
      return true;
    }

    /// <summary>Get whether the target item has one of the given categories.</summary>
    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool ITEM_CATEGORY(string[] query, GameStateQueryContext context)
    {
      Item obj;
      string error;
      if (!GameStateQuery.Helpers.TryGetItemArg(query, 1, context.TargetItem, context.InputItem, out obj, out error))
        return GameStateQuery.Helpers.ErrorResult(query, error);
      if (obj != null)
      {
        if (query.Length == 2)
          return obj.Category < -1;
        for (int index = 2; index < query.Length; ++index)
        {
          int num;
          if (!ArgUtility.TryGetInt(query, index, out num, out error, "int category"))
            return GameStateQuery.Helpers.ErrorResult(query, error);
          if (obj.Category == num)
            return true;
        }
      }
      return false;
    }

    /// <summary>Get whether the item has an explicit category set in <c>Data/Objects</c>, ignoring categories assigned dynamically in code (e.g. for rings). These are often (but not always) special items like Secret Note or unimplemented items like Lumber. This is somewhat specialized.</summary>
    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool ITEM_HAS_EXPLICIT_OBJECT_CATEGORY(
      string[] query,
      GameStateQueryContext context)
    {
      Item obj;
      string error;
      return !GameStateQuery.Helpers.TryGetItemArg(query, 1, context.TargetItem, context.InputItem, out obj, out error) ? GameStateQuery.Helpers.ErrorResult(query, error) : ObjectDataDefinition.HasExplicitCategory(ItemRegistry.GetData(obj?.QualifiedItemId));
    }

    /// <summary>Get whether the target item has the given qualified or unqualified item ID.</summary>
    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool ITEM_ID(string[] query, GameStateQueryContext context)
    {
      string error;
      Item item;
      if (!GameStateQuery.Helpers.TryGetItemArg(query, 1, context.TargetItem, context.InputItem, out item, out error))
        return GameStateQuery.Helpers.ErrorResult(query, error);
      return item != null && GameStateQuery.Helpers.AnyArgMatches(query, 2, (Func<string, bool?>) (rawItemId => new bool?(rawItemId.EqualsIgnoreCase(item.QualifiedItemId) || rawItemId.EqualsIgnoreCase(item.ItemId))));
    }

    /// <summary>Get whether the target item's qualified or unqualified item ID starts with the given prefix.</summary>
    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool ITEM_ID_PREFIX(string[] query, GameStateQueryContext context)
    {
      string error;
      Item item;
      if (!GameStateQuery.Helpers.TryGetItemArg(query, 1, context.TargetItem, context.InputItem, out item, out error))
        return GameStateQuery.Helpers.ErrorResult(query, error);
      return item != null && GameStateQuery.Helpers.AnyArgMatches(query, 2, (Func<string, bool?>) (prefix => new bool?(item.ItemId.StartsWithIgnoreCase(prefix) || item.QualifiedItemId.StartsWithIgnoreCase(prefix))));
    }

    /// <summary>Get whether the target item has a numeric item ID within the given range.</summary>
    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool ITEM_NUMERIC_ID(string[] query, GameStateQueryContext context)
    {
      Item obj;
      string error;
      int num1;
      int num2;
      if (!GameStateQuery.Helpers.TryGetItemArg(query, 1, context.TargetItem, context.InputItem, out obj, out error) || !ArgUtility.TryGetInt(query, 2, out num1, out error, "int minId") || !ArgUtility.TryGetOptionalInt(query, 3, out num2, out error, int.MaxValue, "int maxId"))
        return GameStateQuery.Helpers.ErrorResult(query, error);
      int result;
      return int.TryParse(obj?.ItemId, out result) && result >= num1 && result <= num2;
    }

    /// <summary>Get whether the target item has one of the given object types.</summary>
    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool ITEM_OBJECT_TYPE(string[] query, GameStateQueryContext context)
    {
      Item obj1;
      string error;
      if (!GameStateQuery.Helpers.TryGetItemArg(query, 1, context.TargetItem, context.InputItem, out obj1, out error))
        return GameStateQuery.Helpers.ErrorResult(query, error);
      Object obj = obj1 as Object;
      return obj != null && GameStateQuery.Helpers.AnyArgMatches(query, 2, (Func<string, bool?>) (rawObjType => new bool?(rawObjType.EqualsIgnoreCase(obj.Type))));
    }

    /// <summary>Get whether the target item has a price within the given range.</summary>
    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool ITEM_PRICE(string[] query, GameStateQueryContext context)
    {
      Item obj;
      string error;
      int num1;
      int num2;
      if (!GameStateQuery.Helpers.TryGetItemArg(query, 1, context.TargetItem, context.InputItem, out obj, out error) || !ArgUtility.TryGetInt(query, 2, out num1, out error, "int minPrice") || !ArgUtility.TryGetOptionalInt(query, 3, out num2, out error, int.MaxValue, "int maxPrice"))
        return GameStateQuery.Helpers.ErrorResult(query, error);
      int? nullable1 = obj?.salePrice(false);
      int? nullable2 = nullable1;
      int num3 = num1;
      if (!(nullable2.GetValueOrDefault() >= num3 & nullable2.HasValue))
        return false;
      nullable2 = nullable1;
      int num4 = num2;
      return nullable2.GetValueOrDefault() <= num4 & nullable2.HasValue;
    }

    /// <summary>Get whether the target item has a min quality level.</summary>
    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool ITEM_QUALITY(string[] query, GameStateQueryContext context)
    {
      Item obj;
      string error;
      int num1;
      int num2;
      if (!GameStateQuery.Helpers.TryGetItemArg(query, 1, context.TargetItem, context.InputItem, out obj, out error) || !ArgUtility.TryGetInt(query, 2, out num1, out error, "int minQuality") || !ArgUtility.TryGetOptionalInt(query, 3, out num2, out error, int.MaxValue, "int maxQuality"))
        return GameStateQuery.Helpers.ErrorResult(query, error);
      int? quality = obj?.Quality;
      int? nullable = quality;
      int num3 = num1;
      if (!(nullable.GetValueOrDefault() >= num3 & nullable.HasValue))
        return false;
      nullable = quality;
      int num4 = num2;
      return nullable.GetValueOrDefault() <= num4 & nullable.HasValue;
    }

    /// <summary>Get whether the target item has a min stack size (ignoring other stacks in the inventory).</summary>
    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool ITEM_STACK(string[] query, GameStateQueryContext context)
    {
      Item obj;
      string error;
      int num1;
      int num2;
      if (!GameStateQuery.Helpers.TryGetItemArg(query, 1, context.TargetItem, context.InputItem, out obj, out error) || !ArgUtility.TryGetInt(query, 2, out num1, out error, "int minStack") || !ArgUtility.TryGetOptionalInt(query, 3, out num2, out error, int.MaxValue, "int maxStack"))
        return GameStateQuery.Helpers.ErrorResult(query, error);
      int? stack = obj?.Stack;
      int? nullable = stack;
      int num3 = num1;
      if (!(nullable.GetValueOrDefault() >= num3 & nullable.HasValue))
        return false;
      nullable = stack;
      int num4 = num2;
      return nullable.GetValueOrDefault() <= num4 & nullable.HasValue;
    }

    /// <summary>Get whether the target item has the given item type.</summary>
    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool ITEM_TYPE(string[] query, GameStateQueryContext context)
    {
      string error;
      Item item;
      if (!GameStateQuery.Helpers.TryGetItemArg(query, 1, context.TargetItem, context.InputItem, out item, out error))
        return GameStateQuery.Helpers.ErrorResult(query, error);
      return item != null && GameStateQuery.Helpers.AnyArgMatches(query, 2, (Func<string, bool?>) (rawItemType => new bool?(rawItemType.EqualsIgnoreCase(item.TypeDefinitionId))));
    }

    /// <summary>Get whether the target item has a min edibility level.</summary>
    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool ITEM_EDIBILITY(string[] query, GameStateQueryContext context)
    {
      Item obj;
      string error;
      int num1;
      int num2;
      if (!GameStateQuery.Helpers.TryGetItemArg(query, 1, context.TargetItem, context.InputItem, out obj, out error) || !ArgUtility.TryGetOptionalInt(query, 2, out num1, out error, -299, "int minEdibility") || !ArgUtility.TryGetOptionalInt(query, 3, out num2, out error, int.MaxValue, "int maxEdibility"))
        return GameStateQuery.Helpers.ErrorResult(query, error);
      return obj is Object @object && @object.Edibility >= num1 && @object.Edibility <= num2;
    }

    /// <summary>A condition that always passes.</summary>
    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool TRUE(string[] query, GameStateQueryContext context) => true;

    /// <summary>A condition that always fails.</summary>
    /// <inheritdoc cref="T:StardewValley.Delegates.GameStateQueryDelegate" />
    public static bool FALSE(string[] query, GameStateQueryContext context) => false;
  }
}
