// Decompiled with JetBrains decompiler
// Type: StardewValley.DebugCommands
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Netcode;
using StardewValley.BellsAndWhistles;
using StardewValley.Buffs;
using StardewValley.Buildings;
using StardewValley.Characters;
using StardewValley.Delegates;
using StardewValley.Events;
using StardewValley.Extensions;
using StardewValley.GameData.Buildings;
using StardewValley.GameData.FarmAnimals;
using StardewValley.GameData.Movies;
using StardewValley.GameData.Pets;
using StardewValley.GameData.Shops;
using StardewValley.Internal;
using StardewValley.Inventories;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Locations;
using StardewValley.Logging;
using StardewValley.Menus;
using StardewValley.Minigames;
using StardewValley.Monsters;
using StardewValley.Network;
using StardewValley.Network.Compress;
using StardewValley.Objects;
using StardewValley.Pathfinding;
using StardewValley.Quests;
using StardewValley.SaveMigrations;
using StardewValley.SpecialOrders;
using StardewValley.SpecialOrders.Objectives;
using StardewValley.TerrainFeatures;
using StardewValley.TokenizableStrings;
using StardewValley.Tools;
using StardewValley.Triggers;
using StardewValley.Util;
using StardewValley.WorldMaps;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using xTile.Dimensions;
using xTile.Layers;

#nullable disable
namespace StardewValley;

/// <summary>The debug commands that can be executed through the console.</summary>
/// <remarks>See also <see cref="T:StardewValley.ChatCommands" />.</remarks>
/// <summary>The debug commands that can be executed through the console.</summary>
/// <remarks>See also <see cref="T:StardewValley.ChatCommands" />.</remarks>
public static class DebugCommands
{
  /// <summary>The supported commands and their resolvers.</summary>
  private static readonly Dictionary<string, DebugCommandHandlerDelegate> Handlers = new Dictionary<string, DebugCommandHandlerDelegate>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
  /// <summary>Alternate names for debug commands (e.g. shorthand or acronyms).</summary>
  private static readonly Dictionary<string, string> Aliases = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

  /// <summary>Register the default debug commands, defined as <see cref="T:StardewValley.DebugCommands.DefaultHandlers" /> methods.</summary>
  static DebugCommands()
  {
    MethodInfo[] methods = typeof (DebugCommands.DefaultHandlers).GetMethods(BindingFlags.Static | BindingFlags.Public);
    foreach (MethodInfo method in methods)
    {
      try
      {
        DebugCommands.Handlers[method.Name] = (DebugCommandHandlerDelegate) Delegate.CreateDelegate(typeof (DebugCommandHandlerDelegate), method);
      }
      catch (Exception ex)
      {
        Game1.log.Error($"Failed to initialize debug command {method.Name}.", ex);
      }
    }
    foreach (MethodInfo element in methods)
    {
      OtherNamesAttribute customAttribute = element.GetCustomAttribute<OtherNamesAttribute>();
      if (customAttribute != null)
      {
        foreach (string alias in customAttribute.Aliases)
        {
          if (DebugCommands.Handlers.ContainsKey(alias))
            Game1.log.Error($"Can't register alias '{alias}' for debug command '{element.Name}', because there's a command with that name.");
          string str;
          if (DebugCommands.Aliases.TryGetValue(alias, out str))
            Game1.log.Error($"Can't register alias '{alias}' for debug command '{element.Name}', because that's already an alias for '{str}'.");
          DebugCommands.Aliases[alias] = element.Name;
        }
      }
    }
  }

  /// <summary>Try to handle a debug command.</summary>
  /// <param name="command">The full debug command split by spaces, including the command name and arguments.</param>
  /// <param name="log">The log to which to write command output, or <c>null</c> to use <see cref="F:StardewValley.Game1.log" />.</param>
  /// <returns>Returns whether the command was found and executed, regardless of whether the command logic succeeded.</returns>
  public static bool TryHandle(string[] command, IGameLogger log = null)
  {
    if (log == null)
      log = Game1.log;
    string str1 = ArgUtility.Get(command, 0);
    if (string.IsNullOrWhiteSpace(str1))
    {
      log.Error("Can't parse an empty command.");
      return false;
    }
    string str2;
    if (DebugCommands.Aliases.TryGetValue(str1, out str2))
      str1 = str2;
    DebugCommandHandlerDelegate commandHandlerDelegate;
    if (!DebugCommands.Handlers.TryGetValue(str1, out commandHandlerDelegate))
    {
      log.Error($"Unknown debug command '{str1}'.");
      string[] array = DebugCommands.SearchCommandNames(str1).Take<string>(10).ToArray<string>();
      if (array.Length != 0)
        log.Info("Did you mean one of these?\n- " + string.Join("\n- ", array));
      return false;
    }
    try
    {
      commandHandlerDelegate(command, log);
      return true;
    }
    catch (Exception ex)
    {
      log.Error($"Error running debug command '{string.Join(" ", command)}'.", ex);
      return false;
    }
  }

  /// <summary>Get the list of commands which match the given search text.</summary>
  /// <param name="search">The text to match in command names, or <c>null</c> to list all command names.</param>
  /// <param name="displayAliases">Whether to append aliases in the results, like <c>"houseUpgrade (house, hu)"</c>.</param>
  public static List<string> SearchCommandNames(string search, bool displayAliases = true)
  {
    ILookup<string, string> lookup = DebugCommands.Aliases.ToLookup<KeyValuePair<string, string>, string, string>((Func<KeyValuePair<string, string>, string>) (p => p.Value), (Func<KeyValuePair<string, string>, string>) (p => p.Key));
    List<string> stringList = new List<string>();
    foreach (string key in (IEnumerable<string>) DebugCommands.Handlers.Keys.OrderBy<string, string>((Func<string, string>) (p => p), (IComparer<string>) StringComparer.OrdinalIgnoreCase))
    {
      string[] array = lookup[key].ToArray<string>();
      if (array.Length == 0)
        stringList.Add(key);
      else if (displayAliases)
        stringList.Add($"{key} ({string.Join(", ", (IEnumerable<string>) ((IEnumerable<string>) array).OrderBy<string, string>((Func<string, string>) (p => p), (IComparer<string>) StringComparer.OrdinalIgnoreCase))})");
      else
        stringList.Add($"###{key}###{string.Join(",", array)}");
    }
    if (search != null)
      stringList.RemoveAll((Predicate<string>) (line => !Utility.fuzzyCompare(search, line).HasValue));
    if (!displayAliases)
    {
      for (int index = 0; index < stringList.Count; ++index)
      {
        if (stringList[index].StartsWith("###"))
          stringList[index] = stringList[index].Split("###", 3)[1];
      }
    }
    return stringList;
  }

  /// <summary>Log an error indicating a command's arguments are invalid.</summary>
  /// <param name="log">The log to which to write debug command output.</param>
  /// <param name="command">The full debug command split by spaces, including the command name.</param>
  /// <param name="error">The error phrase to log.</param>
  private static void LogArgError(IGameLogger log, string[] command, string error)
  {
    string str1 = ArgUtility.Get(command, 0);
    string str2 = str1;
    if (!string.IsNullOrWhiteSpace(str1))
    {
      string str3;
      if (!DebugCommands.Aliases.TryGetValue(str1, out str3))
      {
        foreach (string key in DebugCommands.Handlers.Keys)
        {
          if (str1.EqualsIgnoreCase(key))
          {
            str3 = key;
            break;
          }
        }
      }
      str2 = str3 ?? str1;
      if (!str2.EqualsIgnoreCase(str1))
        str2 = $"{str1} ({str2})";
    }
    log.Error($"Failed parsing {str2} command: {error}.");
  }

  /// <summary>The low-level handlers for vanilla debug commands. Most code should call <see cref="M:StardewValley.DebugCommands.TryHandle(System.String[],StardewValley.Logging.IGameLogger)" /> instead, which adds error-handling.</summary>
  public static class DefaultHandlers
  {
    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void GrowWildTrees(string[] command, IGameLogger log)
    {
      foreach (TerrainFeature terrainFeature in Game1.currentLocation.terrainFeatures.Values.ToArray<TerrainFeature>())
      {
        if (terrainFeature is Tree tree)
        {
          tree.growthStage.Value = 4;
          tree.fertilized.Value = true;
          tree.dayUpdate();
          tree.fertilized.Value = false;
        }
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void Emote(string[] command, IGameLogger log)
    {
      for (int index = 1; index < command.Length; index += 2)
      {
        string query;
        string error;
        int whichEmote;
        if (!ArgUtility.TryGet(command, index, out query, out error, false, "string npcName") || !ArgUtility.TryGetInt(command, index + 1, out whichEmote, out error, "int emoteId"))
        {
          log.Warn(error);
        }
        else
        {
          NPC npc = Utility.fuzzyCharacterSearch(query, false);
          if (npc == null)
            log.Error("Couldn't find character named " + query);
          else
            npc.doEmote(whichEmote);
        }
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void EventTestSpecific(string[] command, IGameLogger log)
    {
      Game1.eventTest = new EventTest(command);
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void EventTest(string[] command, IGameLogger log)
    {
      string str;
      string error;
      int startingEventIndex;
      if (!ArgUtility.TryGetOptional(command, 1, out str, out error, name: "string locationName") || !ArgUtility.TryGetOptionalInt(command, 2, out startingEventIndex, out error, name: "int startingEventIndex"))
        DebugCommands.LogArgError(log, command, error);
      else
        Game1.eventTest = new EventTest(str ?? "", startingEventIndex);
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void GetAllQuests(string[] command, IGameLogger log)
    {
      foreach (KeyValuePair<string, string> quest in DataLoader.Quests(Game1.content))
        Game1.player.addQuest(quest.Key);
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void Movie(string[] command, IGameLogger log)
    {
      string id;
      string error;
      string query;
      if (!ArgUtility.TryGetOptional(command, 1, out id, out error, allowBlank: false, name: "string movieId") || !ArgUtility.TryGetOptional(command, 2, out query, out error, allowBlank: false, name: "string invitedNpcName"))
        DebugCommands.LogArgError(log, command, error);
      else if (id != null && !MovieTheater.TryGetMovieData(id, out MovieData _))
      {
        log.Error($"No movie found with ID '{id}'.");
      }
      else
      {
        if (query != null)
        {
          NPC invited_npc = Utility.fuzzyCharacterSearch(query);
          if (invited_npc != null)
            MovieTheater.Invite(Game1.player, invited_npc);
          else
            log.Error($"No NPC found matching '{query}'.");
        }
        if (id != null)
          MovieTheater.forceMovieId = id;
        LocationRequest locationRequest = Game1.getLocationRequest("MovieTheater");
        locationRequest.OnWarp += (LocationRequest.Callback) (() => Game1.currentLocation.performAction("Theater_Doors", Game1.player, Location.Origin));
        Game1.warpFarmer(locationRequest, 10, 10, 0);
      }
    }

    /// <summary>Print the movie schedule for a specified year.</summary>
    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void MovieSchedule(string[] command, IGameLogger log)
    {
      int year;
      string error;
      if (!ArgUtility.TryGetOptionalInt(command, 1, out year, out error, Game1.year, "int year"))
      {
        DebugCommands.LogArgError(log, command, error);
      }
      else
      {
        StringBuilder stringBuilder1 = new StringBuilder();
        StringBuilder stringBuilder2 = stringBuilder1;
        StringBuilder.AppendInterpolatedStringHandler interpolatedStringHandler = new StringBuilder.AppendInterpolatedStringHandler(20, 1, stringBuilder1);
        interpolatedStringHandler.AppendLiteral("Movie schedule for ");
        ref StringBuilder.AppendInterpolatedStringHandler local1 = ref interpolatedStringHandler;
        string str1;
        if (year != Game1.year)
          str1 = $"year {year}";
        else
          str1 = $"this year (year {year})";
        local1.AppendFormatted(str1);
        interpolatedStringHandler.AppendLiteral(":");
        ref StringBuilder.AppendInterpolatedStringHandler local2 = ref interpolatedStringHandler;
        StringBuilder stringBuilder3 = stringBuilder2.AppendLine(ref local2).AppendLine();
        Season[] seasonArray = new Season[4]
        {
          Season.Spring,
          Season.Summer,
          Season.Fall,
          Season.Winter
        };
        foreach (Season season in seasonArray)
        {
          List<Tuple<MovieData, int>> tupleList = new List<Tuple<MovieData, int>>();
          string str2 = (string) null;
          for (int dayOfMonth = 1; dayOfMonth <= 28; ++dayOfMonth)
          {
            MovieData movieForDate = MovieTheater.GetMovieForDate(new WorldDate(year, season, dayOfMonth));
            if (movieForDate.Id != str2)
            {
              tupleList.Add(Tuple.Create<MovieData, int>(movieForDate, dayOfMonth));
              str2 = movieForDate.Id;
            }
          }
          for (int index = 0; index < tupleList.Count; ++index)
          {
            MovieData movieData = tupleList[index].Item1;
            int num1 = tupleList[index].Item2;
            int num2 = tupleList.Count > index + 1 ? tupleList[index + 1].Item2 - 1 : 28;
            string text = TokenParser.ParseText(movieData.Title);
            stringBuilder3.Append((object) season).Append(' ').Append(num1);
            if (num2 != num1)
              stringBuilder3.Append("-").Append(num2);
            stringBuilder3.Append(": ").AppendLine(text);
          }
        }
        log.Info(stringBuilder3.ToString());
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void Shop(string[] command, IGameLogger log)
    {
      string query;
      string error;
      string ownerName;
      if (!ArgUtility.TryGet(command, 1, out query, out error, false, "string shopId") || !ArgUtility.TryGetOptional(command, 2, out ownerName, out error, allowBlank: false, name: "string ownerName"))
      {
        DebugCommands.LogArgError(log, command, error);
      }
      else
      {
        string str = Utility.fuzzySearch(query, (ICollection<string>) DataLoader.Shops(Game1.content).Keys.ToArray<string>());
        if (str == null)
        {
          log.Error($"Couldn't find any shop in Data/Shops matching ID '{query}'.");
        }
        else
        {
          string shopId = str;
          if ((ownerName != null ? (Utility.TryOpenShopMenu(shopId, ownerName) ? 1 : 0) : (Utility.TryOpenShopMenu(shopId, Game1.player.currentLocation, forceOpen: true) ? 1 : 0)) != 0)
            log.Info($"Opened shop with ID '{shopId}'.");
          else
            log.Error($"Failed to open shop with ID '{shopId}'. Is the data in Data/Shops valid?");
        }
      }
    }

    /// <summary>Export a summary of every shop's current inventory.</summary>
    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void ExportShops(string[] command, IGameLogger log)
    {
      StringBuilder stringBuilder1 = new StringBuilder();
      string[] command1 = new string[2]{ "Shop", null };
      foreach (string key1 in DataLoader.Shops(Game1.content).Keys)
      {
        stringBuilder1.AppendLine(key1);
        stringBuilder1.AppendLine("".PadRight(Math.Max(50, key1.Length), '-'));
        StringBuilder.AppendInterpolatedStringHandler interpolatedStringHandler;
        try
        {
          command1[1] = key1;
          DebugCommands.DefaultHandlers.Shop(command1, log);
        }
        catch (Exception ex)
        {
          StringBuilder stringBuilder2 = stringBuilder1.Append("    ");
          StringBuilder stringBuilder3 = stringBuilder2;
          interpolatedStringHandler = new StringBuilder.AppendInterpolatedStringHandler(23, 1, stringBuilder2);
          interpolatedStringHandler.AppendLiteral("Failed to open shop '");
          interpolatedStringHandler.AppendFormatted(key1);
          interpolatedStringHandler.AppendLiteral("'.");
          ref StringBuilder.AppendInterpolatedStringHandler local = ref interpolatedStringHandler;
          stringBuilder3.AppendLine(ref local);
          stringBuilder1.AppendLine("    " + string.Join("\n    ", ex.ToString().Split('\n')));
          continue;
        }
        if (Game1.activeClickableMenu is ShopMenu activeClickableMenu)
        {
          switch (activeClickableMenu.currency)
          {
            case 0:
              stringBuilder1.AppendLine("    Currency: gold");
              break;
            case 1:
              stringBuilder1.AppendLine("    Currency: star tokens");
              break;
            case 2:
              stringBuilder1.AppendLine("    Currency: Qi coins");
              break;
            case 4:
              stringBuilder1.AppendLine("    Currency: Qi gems");
              break;
            default:
              StringBuilder stringBuilder4 = stringBuilder1;
              StringBuilder stringBuilder5 = stringBuilder4;
              interpolatedStringHandler = new StringBuilder.AppendInterpolatedStringHandler(20, 2, stringBuilder4);
              interpolatedStringHandler.AppendFormatted("    ");
              interpolatedStringHandler.AppendLiteral("Currency: unknown (");
              interpolatedStringHandler.AppendFormatted<int>(activeClickableMenu.currency);
              interpolatedStringHandler.AppendLiteral(")");
              ref StringBuilder.AppendInterpolatedStringHandler local = ref interpolatedStringHandler;
              stringBuilder5.AppendLine(ref local);
              break;
          }
          stringBuilder1.AppendLine();
          \u003C\u003Ef__AnonymousType0<string, string, int, string, string>[] array = activeClickableMenu.itemPriceAndStock.Select(entry =>
          {
            ISalable key2 = entry.Key;
            ItemStockInformation stockInformation = entry.Value;
            string qualifiedItemId = key2.QualifiedItemId;
            string displayName = key2.DisplayName;
            int price = stockInformation.Price;
            string str1 = stockInformation.TradeItem != null ? $"{stockInformation.TradeItem} x{(stockInformation.TradeItemCount ?? 1).ToString()}" : (string) null;
            string str2;
            if (stockInformation.Stock == int.MaxValue || stockInformation.LimitedStockMode == LimitedStockMode.None)
              str2 = (string) null;
            else
              str2 = $"{stockInformation.LimitedStockMode} {stockInformation.Stock}";
            return new
            {
              Id = qualifiedItemId,
              Name = displayName,
              Price = price,
              Trade = str1,
              StockLimit = str2
            };
          }).ToArray();
          int num1 = "id".Length;
          int num2 = "name".Length;
          int num3 = "price".Length;
          int num4 = "trade".Length;
          int length = "stock limit".Length;
          foreach (var data in array)
          {
            num1 = Math.Max(num1, data.Id.Length);
            num2 = Math.Max(num2, data.Name.Length);
            num3 = Math.Max(num3, data.Price.ToString().Length);
            if (data.Trade != null)
              num4 = Math.Max(num4, data.Trade.Length);
            if (data.StockLimit != null)
              num4 = Math.Max(num4, data.StockLimit.Length);
          }
          stringBuilder1.Append("    ").Append("id".PadRight(num1)).Append(" | ").Append("name".PadRight(num2)).Append(" | ").Append("price".PadRight(num3)).Append(" | ").Append("trade".PadRight(num4)).AppendLine(" | stock limit");
          stringBuilder1.Append("    ").Append("".PadRight(num1, '-')).Append(" | ").Append("".PadRight(num2, '-')).Append(" | ").Append("".PadRight(num3, '-')).Append(" | ").Append("".PadRight(num4, '-')).Append(" | ").AppendLine("".PadRight(length, '-'));
          foreach (var data in array)
            stringBuilder1.Append("    ").Append(data.Id.PadRight(num1)).Append(" | ").Append(data.Name.PadRight(num2)).Append(" | ").Append(data.Price.ToString().PadRight(num3)).Append(" | ").Append((data.Trade ?? "").PadRight(num4)).Append(" | ").AppendLine(data.StockLimit);
        }
        else
        {
          StringBuilder stringBuilder6 = stringBuilder1.Append("    ");
          StringBuilder stringBuilder7 = stringBuilder6;
          interpolatedStringHandler = new StringBuilder.AppendInterpolatedStringHandler(60, 1, stringBuilder6);
          interpolatedStringHandler.AppendLiteral("Failed to open shop '");
          interpolatedStringHandler.AppendFormatted(key1);
          interpolatedStringHandler.AppendLiteral("': shop menu unexpected failed to open.");
          ref StringBuilder.AppendInterpolatedStringHandler local = ref interpolatedStringHandler;
          stringBuilder7.AppendLine(ref local);
        }
        stringBuilder1.AppendLine();
        stringBuilder1.AppendLine();
      }
      string path = Path.Combine(Program.GetLocalAppDataFolder("Exports"), $"{DateTime.Now:yyyy-MM-dd} shop export.txt");
      File.WriteAllText(path, stringBuilder1.ToString());
      log.Info($"Exported shop data to {path}.");
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void Dating(string[] command, IGameLogger log)
    {
      string key;
      string error;
      if (!ArgUtility.TryGet(command, 1, out key, out error, false, "string npcName"))
        DebugCommands.LogArgError(log, command, error);
      else
        Game1.player.friendshipData[key].Status = FriendshipStatus.Dating;
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void ClearActiveDialogueEvents(string[] command, IGameLogger log)
    {
      Game1.player.activeDialogueEvents.Clear();
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void Buff(string[] command, IGameLogger log)
    {
      string id;
      string error;
      if (!ArgUtility.TryGet(command, 1, out id, out error, false, "string buffId"))
        DebugCommands.LogArgError(log, command, error);
      else
        Game1.player.applyBuff(id);
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void ClearBuffs(string[] command, IGameLogger log) => Game1.player.ClearBuffs();

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void PauseTime(string[] command, IGameLogger log)
    {
      Game1.isTimePaused = !Game1.isTimePaused;
      Game1.playSound(Game1.isTimePaused ? "bigSelect" : "bigDeSelect");
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    [OtherNames(new string[] {"fbf"})]
    public static void FrameByFrame(string[] command, IGameLogger log)
    {
      Game1.frameByFrame = !Game1.frameByFrame;
      Game1.playSound(Game1.frameByFrame ? "bigSelect" : "bigDeSelect");
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    [OtherNames(new string[] {"fbp", "fill", "fillbp"})]
    public static void FillBackpack(string[] command, IGameLogger log)
    {
      for (int index = 0; index < Game1.player.Items.Count; ++index)
      {
        if (Game1.player.Items[index] == null)
        {
          ItemMetadata itemMetadata = (ItemMetadata) null;
          while (itemMetadata == null)
          {
            itemMetadata = ItemRegistry.ResolveMetadata(Game1.random.Next(1000).ToString());
            ParsedItemData parsedData = itemMetadata?.GetParsedData();
            if (parsedData == null || parsedData.Category == -999 || parsedData.ObjectType == "Crafting" || parsedData.ObjectType == "Seeds")
              itemMetadata = (ItemMetadata) null;
          }
          Game1.player.Items[index] = itemMetadata.CreateItem();
        }
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void Bobber(string[] command, IGameLogger log)
    {
      int num;
      string error;
      if (!ArgUtility.TryGetInt(command, 1, out num, out error, "int bobberStyle"))
        DebugCommands.LogArgError(log, command, error);
      else
        Game1.player.bobberStyle.Value = num;
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    [OtherNames(new string[] {"sl"})]
    public static void ShiftToolbarLeft(string[] command, IGameLogger log)
    {
      Game1.player.shiftToolbar(false);
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    [OtherNames(new string[] {"sr"})]
    public static void ShiftToolbarRight(string[] command, IGameLogger log)
    {
      Game1.player.shiftToolbar(true);
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void CharacterInfo(string[] command, IGameLogger log)
    {
      Game1.showGlobalMessage(Game1.currentLocation.characters.Count.ToString() + " characters on this map");
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void DoesItemExist(string[] command, IGameLogger log)
    {
      string itemId;
      string error;
      if (!ArgUtility.TryGet(command, 1, out itemId, out error, false, "string itemId"))
        DebugCommands.LogArgError(log, command, error);
      else
        Game1.showGlobalMessage(Utility.doesItemExistAnywhere(itemId) ? "Yes" : "No");
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void SpecialItem(string[] command, IGameLogger log)
    {
      string str;
      string error;
      if (!ArgUtility.TryGet(command, 1, out str, out error, false, "string itemId"))
        DebugCommands.LogArgError(log, command, error);
      else
        Game1.player.specialItems.Add(str);
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void AnimalInfo(string[] command, IGameLogger log)
    {
      int animalCount = 0;
      int locationCount = 0;
      Utility.ForEachLocation((Func<GameLocation, bool>) (location =>
      {
        int length = location.animals.Length;
        if (length > 0)
        {
          animalCount += length;
          ++locationCount;
        }
        return true;
      }));
      Game1.showGlobalMessage($"{animalCount} animals in {locationCount} locations");
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void ClearChildren(string[] command, IGameLogger log)
    {
      Game1.player.getRidOfChildren();
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void CreateSplash(string[] command, IGameLogger log)
    {
      Point point = new Point();
      switch (Game1.player.FacingDirection)
      {
        case 0:
          point.Y = 4;
          break;
        case 1:
          point.X = 4;
          break;
        case 2:
          point.Y = -4;
          break;
        case 3:
          point.X = -4;
          break;
      }
      Game1.player.currentLocation.fishSplashPoint.Set(new Point(Game1.player.TilePoint.X + point.X, Game1.player.TilePoint.Y + point.Y));
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void Pregnant(string[] command, IGameLogger log)
    {
      WorldDate date = Game1.Date;
      ++date.TotalDays;
      Game1.player.GetSpouseFriendship().NextBirthingDate = date;
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void SpreadSeeds(string[] command, IGameLogger log)
    {
      string error;
      string cropId;
      if (!ArgUtility.TryGet(command, 1, out cropId, out error, false, "string cropId"))
        DebugCommands.LogArgError(log, command, error);
      else
        Game1.currentLocation?.ForEachDirt((Func<HoeDirt, bool>) (dirt =>
        {
          dirt.crop = new Crop(cropId, (int) dirt.Tile.X, (int) dirt.Tile.Y, dirt.Location);
          return true;
        }));
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void SpreadDirt(string[] command, IGameLogger log)
    {
      GameLocation currentLocation = Game1.currentLocation;
      if (currentLocation == null)
        return;
      for (int index1 = 0; index1 < currentLocation.map.Layers[0].LayerWidth; ++index1)
      {
        for (int index2 = 0; index2 < currentLocation.map.Layers[0].LayerHeight; ++index2)
        {
          if (currentLocation.doesTileHaveProperty(index1, index2, "Diggable", "Back") != null && currentLocation.CanItemBePlacedHere(new Vector2((float) index1, (float) index2), true, ignorePassables: CollisionMask.None))
            currentLocation.terrainFeatures.Add(new Vector2((float) index1, (float) index2), (TerrainFeature) new HoeDirt());
        }
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void RemoveFurniture(string[] command, IGameLogger log)
    {
      Game1.currentLocation.furniture.Clear();
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void MakeEx(string[] command, IGameLogger log)
    {
      string key;
      string error;
      if (!ArgUtility.TryGet(command, 1, out key, out error, false, "string npcName"))
      {
        DebugCommands.LogArgError(log, command, error);
      }
      else
      {
        Game1.player.friendshipData[key].RoommateMarriage = false;
        Game1.player.friendshipData[key].Status = FriendshipStatus.Divorced;
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void DarkTalisman(string[] command, IGameLogger log)
    {
      GameLocation gameLocation1 = Game1.RequireLocation("Railroad");
      GameLocation gameLocation2 = Game1.RequireLocation("WitchHut");
      gameLocation1.setMapTile(54, 35, 287, "Buildings", "untitled tile sheet", "");
      gameLocation1.setMapTile(54, 34, 262, "Front", "untitled tile sheet", "");
      gameLocation2.setMapTile(4, 11, 114, "Buildings", "untitled tile sheet", "MagicInk");
      Game1.player.hasDarkTalisman = true;
      Game1.player.hasMagicInk = false;
      Game1.player.mailReceived.Clear();
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void ConventionMode(string[] command, IGameLogger log)
    {
      Game1.conventionMode = !Game1.conventionMode;
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void FarmMap(string[] command, IGameLogger log)
    {
      int num;
      string error;
      if (!ArgUtility.TryGetInt(command, 1, out num, out error, "int farmType"))
      {
        DebugCommands.LogArgError(log, command, error);
      }
      else
      {
        Game1.locations.RemoveWhere<GameLocation>((Predicate<GameLocation>) (location => location is Farm || location is FarmHouse));
        Game1.whichFarm = num;
        Game1.locations.Add((GameLocation) new Farm("Maps\\" + Farm.getMapNameFromTypeInt(Game1.whichFarm), "Farm"));
        Game1.locations.Add((GameLocation) new FarmHouse("Maps\\FarmHouse", "FarmHouse"));
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void ClearMuseum(string[] command, IGameLogger log)
    {
      Game1.RequireLocation<LibraryMuseum>("ArchaeologyHouse").museumPieces.Clear();
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void Clone(string[] command, IGameLogger log)
    {
      string query;
      string error;
      if (!ArgUtility.TryGet(command, 1, out query, out error, false, "string npcName"))
        DebugCommands.LogArgError(log, command, error);
      else
        Game1.currentLocation.characters.Add(Utility.fuzzyCharacterSearch(query));
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    [OtherNames(new string[] {"zl"})]
    public static void ZoomLevel(string[] command, IGameLogger log)
    {
      int num;
      string error;
      if (!ArgUtility.TryGetInt(command, 1, out num, out error, "int zoomLevel"))
        DebugCommands.LogArgError(log, command, error);
      else
        Game1.options.desiredBaseZoomLevel = (float) num / 100f;
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    [OtherNames(new string[] {"us"})]
    public static void UiScale(string[] command, IGameLogger log)
    {
      int num;
      string error;
      if (!ArgUtility.TryGetInt(command, 1, out num, out error, "int uiScale"))
        DebugCommands.LogArgError(log, command, error);
      else
        Game1.options.desiredUIScale = (float) num / 100f;
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void DeleteArch(string[] command, IGameLogger log)
    {
      Game1.player.archaeologyFound.Clear();
      Game1.player.fishCaught.Clear();
      Game1.player.mineralsFound.Clear();
      Game1.player.mailReceived.Clear();
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void Save(string[] command, IGameLogger log)
    {
      Game1.saveOnNewDay = !Game1.saveOnNewDay;
      Game1.playSound(Game1.saveOnNewDay ? "bigSelect" : "bigDeSelect");
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    [OtherNames(new string[] {"removeLargeTf"})]
    public static void RemoveLargeTerrainFeature(string[] command, IGameLogger log)
    {
      Game1.currentLocation.largeTerrainFeatures.Clear();
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void Test(string[] command, IGameLogger log)
    {
      Game1.currentMinigame = (IMinigame) new Test();
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void FenceDecay(string[] command, IGameLogger log)
    {
      int num;
      string error;
      if (!ArgUtility.TryGetInt(command, 1, out num, out error, "int decayAmount"))
      {
        DebugCommands.LogArgError(log, command, error);
      }
      else
      {
        foreach (Object @object in Game1.currentLocation.objects.Values)
        {
          if (@object is Fence fence)
            fence.health.Value -= (float) num;
        }
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    [OtherNames(new string[] {"sb"})]
    public static void ShowTextAboveHead(string[] command, IGameLogger log)
    {
      string query;
      string error;
      if (!ArgUtility.TryGet(command, 1, out query, out error, false, "string npcName"))
        DebugCommands.LogArgError(log, command, error);
      else
        Utility.fuzzyCharacterSearch(query).showTextAboveHead(Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3206"));
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void Gamepad(string[] command, IGameLogger log)
    {
      Game1.options.gamepadControls = !Game1.options.gamepadControls;
      Game1.options.mouseControls = !Game1.options.gamepadControls;
      Game1.showGlobalMessage(Game1.options.gamepadControls ? Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3209") : Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3210"));
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void Slimecraft(string[] command, IGameLogger log)
    {
      Game1.player.craftingRecipes.Add("Slime Incubator", 0);
      Game1.player.craftingRecipes.Add("Slime Egg-Press", 0);
      Game1.playSound("crystal", new int?(0));
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    [OtherNames(new string[] {"kms"})]
    public static void KillMonsterStat(string[] command, IGameLogger log)
    {
      string str;
      string error;
      int sub2;
      if (!ArgUtility.TryGet(command, 1, out str, out error, false, "string monsterId") || !ArgUtility.TryGetInt(command, 2, out sub2, out error, "int kills"))
      {
        DebugCommands.LogArgError(log, command, error);
      }
      else
      {
        Game1.stats.specificMonstersKilled[str] = sub2;
        log.Info(Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3159", (object) str, (object) sub2));
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void RemoveAnimals(string[] command, IGameLogger log)
    {
      Utility.ForEachLocation((Func<GameLocation, bool>) (location =>
      {
        location.Animals.Clear();
        foreach (Building building in location.buildings)
        {
          if (building.GetIndoors() is AnimalHouse indoors2)
            indoors2.Animals.Clear();
        }
        return true;
      }), false);
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void FixAnimals(string[] command, IGameLogger log)
    {
      bool fixedAny = false;
      Utility.ForEachLocation((Func<GameLocation, bool>) (location =>
      {
        int num = 0;
        foreach (Building building1 in location.buildings)
        {
          if (building1.GetIndoors() is AnimalHouse indoors4)
          {
            foreach (FarmAnimal farmAnimal in indoors4.animals.Values)
            {
              FarmAnimal animal = farmAnimal;
              foreach (Building building2 in location.buildings)
              {
                if (building2.GetIndoors() is AnimalHouse indoors3 && indoors3.animalsThatLiveHere.Contains(animal.myID.Value) && !building2.Equals((object) animal.home))
                  num += indoors3.animalsThatLiveHere.RemoveWhere((Func<long, bool>) (id => id == animal.myID.Value));
              }
            }
            num += indoors4.animalsThatLiveHere.RemoveWhere((Func<long, bool>) (id => Utility.getAnimal(id) == null));
          }
        }
        if (num > 0)
        {
          Game1.playSound("crystal", new int?(0));
          log.Info($"Fixed {num} animals in the '{location.NameOrUniqueName}' location.");
          fixedAny = true;
        }
        return true;
      }), false);
      if (!fixedAny)
        log.Info("No animal issues found.");
      Utility.fixAllAnimals();
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void DisplaceAnimals(string[] command, IGameLogger log)
    {
      Utility.ForEachLocation((Func<GameLocation, bool>) (location =>
      {
        if (location.animals.Length == 0 && location.buildings.Count == 0)
          return true;
        Utility.fixAllAnimals();
        foreach (Building building in location.buildings)
        {
          if (building.GetIndoors() is AnimalHouse indoors2)
          {
            foreach (FarmAnimal c in indoors2.animals.Values)
            {
              c.homeInterior = (GameLocation) null;
              c.Position = Utility.recursiveFindOpenTileForCharacter((Character) c, location, new Vector2(40f, 40f), 200) * 64f;
              location.animals.TryAdd(c.myID.Value, c);
            }
            indoors2.animals.Clear();
            indoors2.animalsThatLiveHere.Clear();
          }
        }
        return true;
      }));
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    [OtherNames(new string[] {"sdkInfo"})]
    public static void SteamInfo(string[] command, IGameLogger log) => Program.sdk.DebugInfo();

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void Achieve(string[] command, IGameLogger log)
    {
      string which;
      string error;
      if (!ArgUtility.TryGet(command, 1, out which, out error, false, "string achievementId"))
        DebugCommands.LogArgError(log, command, error);
      else
        Game1.getSteamAchievement(which);
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void ResetAchievements(string[] command, IGameLogger log)
    {
      Program.sdk.ResetAchievements();
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void Divorce(string[] command, IGameLogger log)
    {
      Game1.player.divorceTonight.Value = true;
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void BefriendAnimals(string[] command, IGameLogger log)
    {
      int num;
      string error;
      if (!ArgUtility.TryGetOptionalInt(command, 1, out num, out error, 1000, "int friendship"))
      {
        DebugCommands.LogArgError(log, command, error);
      }
      else
      {
        foreach (FarmAnimal farmAnimal in Game1.currentLocation.animals.Values)
          farmAnimal.friendshipTowardFarmer.Value = num;
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void PetToFarm(string[] command, IGameLogger log)
    {
      Game1.RequireCharacter<Pet>(Game1.player.getPetName(), false).setAtFarmPosition();
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void BefriendPets(string[] command, IGameLogger log)
    {
      foreach (NPC allCharacter in Utility.getAllCharacters())
      {
        if (allCharacter is Pet pet)
          pet.friendshipTowardFarmer.Value = 1000;
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void Version(string[] command, IGameLogger log)
    {
      log.Info(typeof (Game1).Assembly.GetName().Version?.ToString() ?? "");
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    [OtherNames(new string[] {"sdlv"})]
    public static void SdlVersion(string[] command, IGameLogger log)
    {
      Type type = Assembly.GetAssembly(GameRunner.instance.Window.GetType())?.GetType("Sdl");
      if ((object) type == null)
      {
        log.Error("Could not find type 'Sdl'");
      }
      else
      {
        FieldInfo field1 = type.GetField("version", BindingFlags.Static | BindingFlags.Public);
        if ((object) field1 == null)
        {
          log.Error("SDL does not have field 'version'");
        }
        else
        {
          Type fieldType = field1.FieldType;
          object obj1 = field1.GetValue((object) null);
          if ((object) fieldType == null)
            log.Error("Could not find type 'Sdl::Type'");
          else if (obj1 == null)
          {
            log.Error("The obtained from from SDL was null");
          }
          else
          {
            byte[] numArray = new byte[3];
            string[] strArray = new string[3]
            {
              "Major",
              "Minor",
              "Patch"
            };
            for (int index = 0; index < 3; ++index)
            {
              string name = strArray[index];
              FieldInfo field2 = fieldType.GetField(name, BindingFlags.Instance | BindingFlags.Public);
              if ((object) field2 == null)
              {
                log.Error($"SDL::Version does not have field '{name}'");
                return;
              }
              object obj2 = field2.GetValue(obj1);
              if (obj2 is byte)
              {
                int num = (int) (byte) obj2;
                numArray[index] = (byte) obj2;
              }
              else
              {
                log.Error($"SDL::Version field '{name}' is not a byte");
                return;
              }
            }
            log.Info($"SDL Version: {(int) numArray[0]}.{(int) numArray[1]}.{(int) numArray[2]}");
          }
        }
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    [OtherNames(new string[] {"ns"})]
    public static void NoSave(string[] command, IGameLogger log)
    {
      Game1.saveOnNewDay = !Game1.saveOnNewDay;
      if (!Game1.saveOnNewDay)
        Game1.playSound("bigDeSelect");
      else
        Game1.playSound("bigSelect");
      log.Info("Saving is now " + (Game1.saveOnNewDay ? "enabled" : "disabled"));
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    [OtherNames(new string[] {"rfh"})]
    public static void ReadyForHarvest(string[] command, IGameLogger log)
    {
      Vector2 key;
      string error;
      if (!ArgUtility.TryGetVector2(command, 1, out key, out error, true, "Vector2 tile"))
        DebugCommands.LogArgError(log, command, error);
      else
        Game1.currentLocation.objects[key].minutesUntilReady.Value = 1;
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void BeachBridge(string[] command, IGameLogger log)
    {
      Beach beach = Game1.RequireLocation<Beach>("Beach");
      beach.bridgeFixed.Value = !beach.bridgeFixed.Value;
      if (beach.bridgeFixed.Value)
        return;
      beach.setMapTile(58, 13, 284, "Buildings", "untitled tile sheet");
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    /// <remarks>See also <see cref="M:StardewValley.DebugCommands.DefaultHandlers.DaysPlayed(System.String[],StardewValley.Logging.IGameLogger)" />.</remarks>
    public static void Dp(string[] command, IGameLogger log)
    {
      int num;
      string error;
      if (!ArgUtility.TryGetInt(command, 1, out num, out error, "int daysPlayed"))
        DebugCommands.LogArgError(log, command, error);
      else
        Game1.stats.DaysPlayed = (uint) num;
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    [OtherNames(new string[] {"fo"})]
    public static void FrameOffset(string[] command, IGameLogger log)
    {
      int index;
      string error;
      int num1;
      int num2;
      if (!ArgUtility.TryGetInt(command, 1, out index, out error, "int frame") || !ArgUtility.TryGetInt(command, 2, out num1, out error, "int offsetX") || !ArgUtility.TryGetInt(command, 3, out num2, out error, "int offsetY"))
      {
        DebugCommands.LogArgError(log, command, error);
      }
      else
      {
        FarmerRenderer.featureXOffsetPerFrame[index] = (int) (short) num1;
        FarmerRenderer.featureYOffsetPerFrame[index] = (int) (short) num2;
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void Horse(string[] command, IGameLogger log)
    {
      int xTile;
      string error;
      int yTile;
      if (!ArgUtility.TryGetOptionalInt(command, 1, out xTile, out error, Game1.player.TilePoint.X, "int tileX") || !ArgUtility.TryGetOptionalInt(command, 1, out yTile, out error, Game1.player.TilePoint.Y, "int tileY"))
        DebugCommands.LogArgError(log, command, error);
      else
        Game1.currentLocation.characters.Add((NPC) new Horse(GuidHelper.NewGuid(), xTile, yTile));
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void Owl(string[] command, IGameLogger log) => Game1.currentLocation.addOwl();

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void Pole(string[] command, IGameLogger log)
    {
      int num;
      string error;
      if (!ArgUtility.TryGetOptionalInt(command, 1, out num, out error, name: "int rodLevel"))
      {
        DebugCommands.LogArgError(log, command, error);
      }
      else
      {
        Item obj;
        switch (num)
        {
          case 1:
            obj = ItemRegistry.Create("(T)TrainingRod");
            break;
          case 2:
            obj = ItemRegistry.Create("(T)FiberglassRod");
            break;
          case 3:
            obj = ItemRegistry.Create("(T)IridiumRod");
            break;
          default:
            obj = ItemRegistry.Create("(T)BambooRod");
            break;
        }
        Game1.player.addItemToInventoryBool(obj);
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void RemoveQuest(string[] command, IGameLogger log)
    {
      string questID;
      string error;
      if (!ArgUtility.TryGet(command, 1, out questID, out error, false, "string questId"))
        DebugCommands.LogArgError(log, command, error);
      else
        Game1.player.removeQuest(questID);
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void CompleteQuest(string[] command, IGameLogger log)
    {
      string questID;
      string error;
      if (!ArgUtility.TryGet(command, 1, out questID, out error, false, "string questId"))
        DebugCommands.LogArgError(log, command, error);
      else
        Game1.player.completeQuest(questID);
    }

    /// <summary>Set the current player's preferred pet type and breed. This doesn't change any existing pets; see <see cref="M:StardewValley.DebugCommands.DefaultHandlers.ChangePet(System.String[],StardewValley.Logging.IGameLogger)" /> for that.</summary>
    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void SetPreferredPet(string[] command, IGameLogger log)
    {
      string petType;
      string error;
      string breedId;
      if (!ArgUtility.TryGet(command, 1, out petType, out error, false, "string typeId") || !ArgUtility.TryGetOptional(command, 2, out breedId, out error, allowBlank: false, name: "string breedId"))
      {
        DebugCommands.LogArgError(log, command, error);
      }
      else
      {
        PetData data;
        if (!Pet.TryGetData(petType, out data))
          log.Error($"Can't set the player's preferred pet type to '{petType}': no such pet type found. Expected one of ['{string.Join("', '", (IEnumerable<string>) Game1.petData.Keys)}'].");
        else if (breedId != null && data.Breeds.All<PetBreed>((Func<PetBreed, bool>) (p => p.Id != breedId)))
        {
          log.Error($"Can't set the player's preferred pet breed to '{breedId}': no such breed found. Expected one of ['{string.Join("', '", data.Breeds.Select<PetBreed, string>((Func<PetBreed, string>) (p => p.Id)))}'].");
        }
        else
        {
          bool flag = false;
          if (Game1.player.whichPetType != petType)
          {
            log.Info($"Changed preferred pet type from '{Game1.player.whichPetType}' to '{petType}'.");
            Game1.player.whichPetType = petType;
            flag = true;
            if (breedId == null)
              breedId = data.Breeds.FirstOrDefault<PetBreed>()?.Id;
          }
          if (breedId != null && Game1.player.whichPetBreed != breedId)
          {
            log.Info($"Changed preferred pet breed from '{Game1.player.whichPetBreed}' to '{breedId}'.");
            Game1.player.whichPetBreed = breedId;
            flag = true;
          }
          if (flag)
            return;
          log.Info("The player's pet type and breed already match those values.");
        }
      }
    }

    /// <summary>Change the pet type and/or breed for a specific pet. This doesn't change the player's preferred pet type/breed; see <see cref="M:StardewValley.DebugCommands.DefaultHandlers.SetPreferredPet(System.String[],StardewValley.Logging.IGameLogger)" /> for that.</summary>
    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void ChangePet(string[] command, IGameLogger log)
    {
      string name;
      string error;
      string petType;
      string breedId;
      if (!ArgUtility.TryGet(command, 1, out name, out error, false, "string petName") || !ArgUtility.TryGet(command, 2, out petType, out error, false, "string typeId") || !ArgUtility.TryGetOptional(command, 3, out breedId, out error, allowBlank: false, name: "string breedId"))
      {
        DebugCommands.LogArgError(log, command, error);
      }
      else
      {
        PetData data;
        if (!Pet.TryGetData(petType, out data))
          log.Error($"Can't set the pet type to '{petType}': no such pet type found. Expected one of ['{string.Join("', '", (IEnumerable<string>) Game1.petData.Keys)}'].");
        else if (breedId != null && data.Breeds.All<PetBreed>((Func<PetBreed, bool>) (p => p.Id != breedId)))
        {
          log.Error($"Can't set the pet breed to '{breedId}': no such breed found. Expected one of ['{string.Join("', '", data.Breeds.Select<PetBreed, string>((Func<PetBreed, string>) (p => p.Id)))}'].");
        }
        else
        {
          Pet characterFromName = Game1.getCharacterFromName<Pet>(name, false);
          if (characterFromName == null)
          {
            log.Error($"No pet found with name '{name}'.");
          }
          else
          {
            bool flag = false;
            if (characterFromName.petType.Value != petType)
            {
              log.Info($"Changed {characterFromName.Name}'s type from '{characterFromName.petType.Value}' to '{petType}'.");
              characterFromName.petType.Value = petType;
              flag = true;
              if (breedId == null)
                breedId = data.Breeds.FirstOrDefault<PetBreed>()?.Id;
            }
            if (breedId != null && characterFromName.whichBreed.Value != breedId)
            {
              log.Info($"Changed {characterFromName.Name}'s breed from '{characterFromName.whichBreed.Value}' to '{breedId}'.");
              characterFromName.whichBreed.Value = breedId;
              flag = true;
            }
            if (flag)
              return;
            log.Info(characterFromName.Name + "'s type and breed already match those values.");
          }
        }
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void ClearCharacters(string[] command, IGameLogger log)
    {
      Game1.currentLocation.characters.Clear();
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void Cat(string[] command, IGameLogger log)
    {
      Point point;
      string error;
      string petBreed;
      if (!ArgUtility.TryGetPoint(command, 1, out point, out error, "Point tile") || !ArgUtility.TryGetOptional(command, 3, out petBreed, out error, "0", false, "string breedId"))
        DebugCommands.LogArgError(log, command, error);
      else
        Game1.currentLocation.characters.Add((NPC) new Pet(point.X, point.Y, petBreed, nameof (Cat)));
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void Dog(string[] command, IGameLogger log)
    {
      Point point;
      string error;
      string petBreed;
      if (!ArgUtility.TryGetPoint(command, 1, out point, out error, "Point tile") || !ArgUtility.TryGetOptional(command, 3, out petBreed, out error, "0", false, "string breedId"))
        DebugCommands.LogArgError(log, command, error);
      else
        Game1.currentLocation.characters.Add((NPC) new Pet(point.X, point.Y, petBreed, nameof (Dog)));
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void Quest(string[] command, IGameLogger log)
    {
      string questId;
      string error;
      if (!ArgUtility.TryGet(command, 1, out questId, out error, false, "string questId"))
        DebugCommands.LogArgError(log, command, error);
      else
        Game1.player.addQuest(questId);
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void DeliveryQuest(string[] command, IGameLogger log)
    {
      Game1.player.questLog.Add((Quest) new ItemDeliveryQuest());
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void CollectQuest(string[] command, IGameLogger log)
    {
      Game1.player.questLog.Add((Quest) new ResourceCollectionQuest());
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void SlayQuest(string[] command, IGameLogger log)
    {
      bool flag;
      string error;
      if (!ArgUtility.TryGetOptionalBool(command, 1, out flag, out error, true, "bool ignoreFarmMonsters"))
      {
        DebugCommands.LogArgError(log, command, error);
      }
      else
      {
        NetObjectList<Quest> questLog = Game1.player.questLog;
        SlayMonsterQuest slayMonsterQuest = new SlayMonsterQuest();
        slayMonsterQuest.ignoreFarmMonsters.Add(flag);
        questLog.Add((Quest) slayMonsterQuest);
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void Quests(string[] command, IGameLogger log)
    {
      foreach (string key in DataLoader.Quests(Game1.content).Keys)
      {
        if (!Game1.player.hasQuest(key))
          Game1.player.addQuest(key);
      }
      Game1.player.questLog.Add((Quest) new ItemDeliveryQuest());
      Game1.player.questLog.Add((Quest) new SlayMonsterQuest());
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void ClearQuests(string[] command, IGameLogger log)
    {
      Game1.player.questLog.Clear();
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    [OtherNames(new string[] {"fb"})]
    public static void FillBin(string[] command, IGameLogger log)
    {
      IInventory shippingBin = Game1.getFarm().getShippingBin(Game1.player);
      shippingBin.Add(ItemRegistry.Create("(O)24"));
      shippingBin.Add(ItemRegistry.Create("(O)82"));
      shippingBin.Add(ItemRegistry.Create("(O)136"));
      shippingBin.Add(ItemRegistry.Create("(O)16"));
      shippingBin.Add(ItemRegistry.Create("(O)388"));
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void Gold(string[] command, IGameLogger log) => Game1.player.Money += 1000000;

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void ClearFarm(string[] command, IGameLogger log)
    {
      Farm farm = Game1.getFarm();
      Layer layer = farm.map.Layers[0];
      farm.removeObjectsAndSpawned(0, 0, layer.LayerWidth, layer.LayerHeight);
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void SetupFarm(string[] command, IGameLogger log)
    {
      bool flag;
      string error;
      if (!ArgUtility.TryGetOptionalBool(command, 1, out flag, out error, name: "bool clearMore"))
      {
        DebugCommands.LogArgError(log, command, error);
      }
      else
      {
        Farm farm = Game1.getFarm();
        Layer layer = farm.map.Layers[0];
        farm.buildings.Clear();
        farm.AddDefaultBuildings(true);
        farm.removeObjectsAndSpawned(0, 0, layer.LayerWidth, 16 /*0x10*/ + (flag ? 32 /*0x20*/ : 0));
        farm.removeObjectsAndSpawned(56, 17, 16 /*0x10*/, 18);
        for (int x = 58; x < 70; ++x)
        {
          for (int y = 19; y < 29; ++y)
            farm.terrainFeatures.Add(new Vector2((float) x, (float) y), (TerrainFeature) new HoeDirt());
        }
        Building constructed1;
        if (farm.buildStructure("Coop", new Vector2(52f, 11f), Game1.player, out constructed1))
          constructed1.daysOfConstructionLeft.Value = 0;
        Building constructed2;
        if (farm.buildStructure("Silo", new Vector2(36f, 9f), Game1.player, out constructed2))
          constructed2.daysOfConstructionLeft.Value = 0;
        Building constructed3;
        if (farm.buildStructure("Barn", new Vector2(42f, 10f), Game1.player, out constructed3))
          constructed3.daysOfConstructionLeft.Value = 0;
        for (int index = 0; index < Game1.player.Items.Count; ++index)
        {
          if (Game1.player.Items[index] is Tool tool)
          {
            string itemId = (string) null;
            string qualifiedItemId = tool.QualifiedItemId;
            if (qualifiedItemId != null)
            {
              switch (qualifiedItemId.Length)
              {
                case 6:
                  switch (qualifiedItemId[3])
                  {
                    case 'A':
                      if (qualifiedItemId == "(T)Axe")
                        break;
                      goto label_43;
                    case 'H':
                      if (qualifiedItemId == "(T)Hoe")
                        goto label_40;
                      goto label_43;
                    default:
                      goto label_43;
                  }
                  break;
                case 10:
                  switch (qualifiedItemId[7])
                  {
                    case 'A':
                      if (qualifiedItemId == "(T)GoldAxe")
                        break;
                      goto label_43;
                    case 'H':
                      if (qualifiedItemId == "(T)GoldHoe")
                        goto label_40;
                      goto label_43;
                    case 'a':
                      if (qualifiedItemId == "(T)Pickaxe")
                        goto label_41;
                      goto label_43;
                    default:
                      goto label_43;
                  }
                  break;
                case 11:
                  switch (qualifiedItemId[8])
                  {
                    case 'A':
                      if (qualifiedItemId == "(T)SteelAxe")
                        break;
                      goto label_43;
                    case 'H':
                      if (qualifiedItemId == "(T)SteelHoe")
                        goto label_40;
                      goto label_43;
                    default:
                      goto label_43;
                  }
                  break;
                case 12:
                  switch (qualifiedItemId[9])
                  {
                    case 'A':
                      if (qualifiedItemId == "(T)CopperAxe")
                        break;
                      goto label_43;
                    case 'H':
                      if (qualifiedItemId == "(T)CopperHoe")
                        goto label_40;
                      goto label_43;
                    default:
                      goto label_43;
                  }
                  break;
                case 14:
                  switch (qualifiedItemId[3])
                  {
                    case 'G':
                      if (qualifiedItemId == "(T)GoldPickaxe")
                        goto label_41;
                      goto label_43;
                    case 'W':
                      if (qualifiedItemId == "(T)WateringCan")
                        goto label_42;
                      goto label_43;
                    default:
                      goto label_43;
                  }
                case 15:
                  if (qualifiedItemId == "(T)SteelPickaxe")
                    goto label_41;
                  goto label_43;
                case 16 /*0x10*/:
                  if (qualifiedItemId == "(T)CopperPickaxe")
                    goto label_41;
                  goto label_43;
                case 18:
                  if (qualifiedItemId == "(T)GoldWateringCan")
                    goto label_42;
                  goto label_43;
                case 19:
                  if (qualifiedItemId == "(T)SteelWateringCan")
                    goto label_42;
                  goto label_43;
                case 20:
                  if (qualifiedItemId == "(T)CopperWateringCan")
                    goto label_42;
                  goto label_43;
                default:
                  goto label_43;
              }
              itemId = "(T)IridiumAxe";
              goto label_43;
label_40:
              itemId = "(T)IridiumHoe";
              goto label_43;
label_41:
              itemId = "(T)IridiumPickaxe";
              goto label_43;
label_42:
              itemId = "(T)IridiumWateringCan";
            }
label_43:
            if (itemId != null)
            {
              Tool other = ItemRegistry.Create<Tool>(itemId);
              other.UpgradeFrom(other);
              Game1.player.Items[index] = (Item) other;
            }
          }
        }
        Game1.player.Money += 20000;
        Game1.player.addItemToInventoryBool(ItemRegistry.Create("(T)Shears"));
        Game1.player.addItemToInventoryBool(ItemRegistry.Create("(T)MilkPail"));
        Game1.player.addItemToInventoryBool(ItemRegistry.Create("(O)472", 999));
        Game1.player.addItemToInventoryBool(ItemRegistry.Create("(O)473", 999));
        Game1.player.addItemToInventoryBool(ItemRegistry.Create("(O)322", 999));
        Game1.player.addItemToInventoryBool(ItemRegistry.Create("(O)388", 999));
        Game1.player.addItemToInventoryBool(ItemRegistry.Create("(O)390", 999));
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void RemoveBuildings(string[] command, IGameLogger log)
    {
      Game1.currentLocation.buildings.Clear();
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void Build(string[] command, IGameLogger log)
    {
      string error;
      int x;
      int y;
      bool skipSafetyChecks;
      string buildingType;
      if (!ArgUtility.TryGet(command, 1, out buildingType, out error, false, "string buildingType") || !ArgUtility.TryGetOptionalInt(command, 2, out x, out error, Game1.player.TilePoint.X + 1, "int x") || !ArgUtility.TryGetOptionalInt(command, 3, out y, out error, Game1.player.TilePoint.Y, "int y") || !ArgUtility.TryGetOptionalBool(command, 4, out skipSafetyChecks, out error, ArgUtility.Get(command, 0) == "ForceBuild", "bool forceBuild"))
      {
        DebugCommands.LogArgError(log, command, error);
      }
      else
      {
        if (!Game1.buildingData.ContainsKey(buildingType))
          buildingType = Game1.buildingData.Keys.FirstOrDefault<string>((Func<string, bool>) (key => buildingType.EqualsIgnoreCase(key))) ?? buildingType;
        if (!Game1.buildingData.ContainsKey(buildingType))
        {
          string[] array = Utility.fuzzySearchAll(buildingType, Game1.buildingData.Keys, false).ToArray<string>();
          log.Warn(array.Length == 0 ? $"There's no building with type '{buildingType}'." : $"There's no building with type '{buildingType}'. Did you mean one of these?\n- {string.Join("\n- ", array)}");
        }
        else
        {
          Building constructed;
          if (!Game1.currentLocation.buildStructure(buildingType, new Vector2((float) x, (float) y), Game1.player, out constructed, skipSafetyChecks: skipSafetyChecks))
          {
            log.Warn($"Couldn't place a '{buildingType}' building at position ({x}, {y}).");
          }
          else
          {
            constructed.daysOfConstructionLeft.Value = 0;
            log.Info($"Placed '{buildingType}' at position ({x}, {y}).");
          }
        }
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void ForceBuild(string[] command, IGameLogger log)
    {
      if (ArgUtility.HasIndex<string>(command, 0))
        command[0] = nameof (ForceBuild);
      DebugCommands.DefaultHandlers.Build(command, log);
    }

    [OtherNames(new string[] {"fab"})]
    public static void FinishAllBuilds(string[] command, IGameLogger log)
    {
      if (!Game1.IsMasterGame)
      {
        log.Error("Only the host can use this command.");
      }
      else
      {
        int count = 0;
        Utility.ForEachLocation((Func<GameLocation, bool>) (location =>
        {
          foreach (Building building in location.buildings)
          {
            if (building.daysOfConstructionLeft.Value > 0 || building.daysUntilUpgrade.Value > 0)
            {
              building.FinishConstruction();
              ++count;
            }
          }
          return true;
        }));
        log.Info($"Finished constructing {count} building(s).");
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void LocalInfo(string[] command, IGameLogger log)
    {
      int num1 = 0;
      int num2 = 0;
      int num3 = 0;
      foreach (TerrainFeature terrainFeature in Game1.currentLocation.terrainFeatures.Values)
      {
        switch (terrainFeature)
        {
          case Grass _:
            ++num1;
            continue;
          case Tree _:
            ++num2;
            continue;
          default:
            ++num3;
            continue;
        }
      }
      string str = $"Grass:{(object) num1},  Trees:{(object) num2},  Other Terrain Features:{(object) num3},  Objects: {(object) Game1.currentLocation.objects.Length},  temporarySprites: {(object) Game1.currentLocation.temporarySprites.Count},  ";
      log.Info(str);
      Game1.drawObjectDialogue(str);
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    [OtherNames(new string[] {"al"})]
    public static void AmbientLight(string[] command, IGameLogger log)
    {
      int r;
      string error;
      int g;
      int b;
      if (!ArgUtility.TryGetInt(command, 1, out r, out error, "int red") || !ArgUtility.TryGetInt(command, 2, out g, out error, "int green") || !ArgUtility.TryGetInt(command, 3, out b, out error, "int blue"))
        DebugCommands.LogArgError(log, command, error);
      else
        Game1.ambientLight = new Color(r, g, b);
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void ResetMines(string[] command, IGameLogger log)
    {
      MineShaft.permanentMineChanges.Clear();
      Game1.playSound("jingle1");
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    [OtherNames(new string[] {"db"})]
    public static void SpeakTo(string[] command, IGameLogger log)
    {
      string query;
      string error;
      if (!ArgUtility.TryGetOptional(command, 1, out query, out error, "Pierre", false, "string npcName"))
        DebugCommands.LogArgError(log, command, error);
      else
        Game1.activeClickableMenu = (IClickableMenu) new DialogueBox(Utility.fuzzyCharacterSearch(query).CurrentDialogue.Peek());
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void SkullKey(string[] command, IGameLogger log)
    {
      Game1.player.hasSkullKey = true;
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void TownKey(string[] command, IGameLogger log) => Game1.player.HasTownKey = true;

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void Specials(string[] command, IGameLogger log)
    {
      Game1.player.hasRustyKey = true;
      Game1.player.hasSkullKey = true;
      Game1.player.hasSpecialCharm = true;
      Game1.player.hasDarkTalisman = true;
      Game1.player.hasMagicInk = true;
      Game1.player.hasClubCard = true;
      Game1.player.canUnderstandDwarves = true;
      Game1.player.hasMagnifyingGlass = true;
      Game1.player.eventsSeen.Add("2120303");
      Game1.player.eventsSeen.Add("3910979");
      Game1.player.HasTownKey = true;
      Game1.player.stats.Set("trinketSlots", 1);
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void SkullGear(string[] command, IGameLogger log)
    {
      int howMuch = 36 - Game1.player.MaxItems;
      if (howMuch > 0)
        Game1.player.increaseBackpackSize(howMuch);
      Game1.player.hasSkullKey = true;
      Game1.player.Equip<Ring>(ItemRegistry.Create<Ring>("(O)527"), Game1.player.leftRing);
      Game1.player.Equip<Ring>(ItemRegistry.Create<Ring>("(O)523"), Game1.player.rightRing);
      Game1.player.Equip<Boots>(ItemRegistry.Create<Boots>("(B)514"), Game1.player.boots);
      Game1.player.clearBackpack();
      Game1.player.addItemToInventory(ItemRegistry.Create("(T)IridiumPickaxe"));
      Game1.player.addItemToInventory(ItemRegistry.Create("(W)4"));
      Game1.player.addItemToInventory(ItemRegistry.Create("(O)226", 20));
      Game1.player.addItemToInventory(ItemRegistry.Create("(O)288", 20));
      Game1.player.professions.Add(24);
      Game1.player.maxHealth = 75;
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void ClearSpecials(string[] command, IGameLogger log)
    {
      Game1.player.hasRustyKey = false;
      Game1.player.hasSkullKey = false;
      Game1.player.hasSpecialCharm = false;
      Game1.player.hasDarkTalisman = false;
      Game1.player.hasMagicInk = false;
      Game1.player.hasClubCard = false;
      Game1.player.canUnderstandDwarves = false;
      Game1.player.hasMagnifyingGlass = false;
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void Tv(string[] command, IGameLogger log)
    {
      Game1.player.addItemToInventoryBool(ItemRegistry.Create(Game1.random.Choose<string>("(F)1466", "(F)1468")));
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    [OtherNames(new string[] {"sn"})]
    public static void SecretNote(string[] command, IGameLogger log)
    {
      int num1;
      string error;
      if (!ArgUtility.TryGetOptionalInt(command, 1, out num1, out error, -1, "int noteId"))
      {
        DebugCommands.LogArgError(log, command, error);
      }
      else
      {
        Game1.player.hasMagnifyingGlass = true;
        if (num1 > -1)
        {
          int num2 = num1;
          Object object1 = ItemRegistry.Create<Object>("(O)79");
          Object object2 = object1;
          object2.name = $"{object2.name} #{num2.ToString()}";
          Game1.player.addItemToInventory((Item) object1);
        }
        else
          Game1.player.addItemToInventory((Item) Game1.currentLocation.tryToCreateUnseenSecretNote(Game1.player));
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void Child2(string[] command, IGameLogger log)
    {
      Farmer player = Game1.player;
      List<Child> children = player.getChildren();
      if (children.Count > 1)
      {
        ++children[1].Age;
        children[1].reloadSprite(false);
      }
      else
        Utility.getHomeOfFarmer(player).characters.Add((NPC) new Child("Baby2", Game1.random.NextBool(), Game1.random.NextBool(), player));
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    [OtherNames(new string[] {"kid"})]
    public static void Child(string[] command, IGameLogger log)
    {
      Farmer player = Game1.player;
      List<Child> children = player.getChildren();
      if (children.Count > 0)
      {
        ++children[0].Age;
        children[0].reloadSprite(false);
      }
      else
        Utility.getHomeOfFarmer(player).characters.Add((NPC) new Child("Baby", Game1.random.NextBool(), Game1.random.NextBool(), player));
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void KillAll(string[] command, IGameLogger log)
    {
      string error;
      string safeNpcName;
      if (!ArgUtility.TryGet(command, 1, out safeNpcName, out error, false, "string safeNpcName"))
        DebugCommands.LogArgError(log, command, error);
      else
        Utility.ForEachLocation((Func<GameLocation, bool>) (location =>
        {
          if (!location.Equals(Game1.currentLocation))
            location.characters.Clear();
          else
            location.characters.RemoveWhere((Func<NPC, bool>) (npc => npc.Name != safeNpcName));
          return true;
        }));
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void ResetWorldState(string[] command, IGameLogger log)
    {
      Game1.worldStateIDs.Clear();
      Game1.netWorldState.Value = new NetWorldState();
      Game1.game1.parseDebugInput("DeleteArch", log);
      Game1.player.mailReceived.Clear();
      Game1.player.eventsSeen.Clear();
      Game1.eventsSeenSinceLastLocationChange.Clear();
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void KillAllHorses(string[] command, IGameLogger log)
    {
      Utility.ForEachLocation((Func<GameLocation, bool>) (location =>
      {
        if (location.characters.RemoveWhere((Func<NPC, bool>) (npc => npc is Horse)) > 0)
          Game1.playSound("drumkit0");
        return true;
      }));
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void DatePlayer(string[] command, IGameLogger log)
    {
      foreach (Farmer allFarmer in Game1.getAllFarmers())
      {
        if (allFarmer != Game1.player && allFarmer.isCustomized.Value)
        {
          Game1.player.team.GetFriendship(Game1.player.UniqueMultiplayerID, allFarmer.UniqueMultiplayerID).Status = FriendshipStatus.Dating;
          break;
        }
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void EngagePlayer(string[] command, IGameLogger log)
    {
      foreach (Farmer allFarmer in Game1.getAllFarmers())
      {
        if (allFarmer != Game1.player && allFarmer.isCustomized.Value)
        {
          Friendship friendship = Game1.player.team.GetFriendship(Game1.player.UniqueMultiplayerID, allFarmer.UniqueMultiplayerID);
          friendship.Status = FriendshipStatus.Engaged;
          friendship.WeddingDate = Game1.Date;
          ++friendship.WeddingDate.TotalDays;
          break;
        }
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void MarryPlayer(string[] command, IGameLogger log)
    {
      foreach (Farmer onlineFarmer in Game1.getOnlineFarmers())
      {
        if (onlineFarmer != Game1.player && onlineFarmer.isCustomized.Value)
        {
          Friendship friendship = Game1.player.team.GetFriendship(Game1.player.UniqueMultiplayerID, onlineFarmer.UniqueMultiplayerID);
          friendship.Status = FriendshipStatus.Married;
          friendship.WeddingDate = Game1.Date;
          break;
        }
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void Marry(string[] command, IGameLogger log)
    {
      string query;
      string error;
      if (!ArgUtility.TryGet(command, 1, out query, out error, false, "string npcName"))
      {
        DebugCommands.LogArgError(log, command, error);
      }
      else
      {
        NPC n = Utility.fuzzyCharacterSearch(query);
        if (n == null)
        {
          log.Error($"No character found matching '{query}'.");
        }
        else
        {
          Friendship friendship;
          if (!Game1.player.friendshipData.TryGetValue(n.Name, out friendship))
            Game1.player.friendshipData[n.Name] = friendship = new Friendship();
          Game1.player.changeFriendship(2500, n);
          Game1.player.spouse = n.Name;
          friendship.WeddingDate = new WorldDate(Game1.Date);
          friendship.Status = FriendshipStatus.Married;
          Game1.prepareSpouseForWedding(Game1.player);
        }
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void Engaged(string[] command, IGameLogger log)
    {
      string query;
      string error;
      if (!ArgUtility.TryGet(command, 1, out query, out error, false, "string npcName"))
      {
        DebugCommands.LogArgError(log, command, error);
      }
      else
      {
        NPC n = Utility.fuzzyCharacterSearch(query);
        if (n == null)
        {
          log.Error($"No character found matching '{query}'.");
        }
        else
        {
          Friendship friendship;
          if (!Game1.player.friendshipData.TryGetValue(n.Name, out friendship))
            Game1.player.friendshipData[n.Name] = friendship = new Friendship();
          Game1.player.changeFriendship(2500, n);
          Game1.player.spouse = n.Name;
          friendship.Status = FriendshipStatus.Engaged;
          WorldDate date = Game1.Date;
          ++date.TotalDays;
          friendship.WeddingDate = date;
        }
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void ClearLightGlows(string[] command, IGameLogger log)
    {
      Game1.currentLocation.lightGlows.Clear();
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    [OtherNames(new string[] {"wp"})]
    public static void Wallpaper(string[] command, IGameLogger log)
    {
      int which;
      string error;
      if (!ArgUtility.TryGetOptionalInt(command, 1, out which, out error, -1, "int wallpaperId"))
        DebugCommands.LogArgError(log, command, error);
      else if (which > -1)
      {
        Game1.player.addItemToInventoryBool((Item) new Wallpaper(which));
      }
      else
      {
        bool isFloor = Game1.random.NextBool();
        Game1.player.addItemToInventoryBool((Item) new Wallpaper(isFloor ? Game1.random.Next(40) : Game1.random.Next(112 /*0x70*/), isFloor));
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void ClearFurniture(string[] command, IGameLogger log)
    {
      Game1.currentLocation.furniture.Clear();
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    [OtherNames(new string[] {"ff"})]
    public static void Furniture(string[] command, IGameLogger log)
    {
      string str;
      string error;
      if (!ArgUtility.TryGetOptional(command, 1, out str, out error, allowBlank: false, name: "string furnitureId"))
        DebugCommands.LogArgError(log, command, error);
      else if (str == null)
      {
        Item obj = (Item) null;
        while (obj == null)
        {
          try
          {
            obj = ItemRegistry.Create("(F)" + Game1.random.Next(1613).ToString());
          }
          catch
          {
          }
        }
        Game1.player.addItemToInventoryBool(obj);
      }
      else
        Game1.player.addItemToInventoryBool(ItemRegistry.Create("(F)" + str));
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void SpawnCoopsAndBarns(string[] command, IGameLogger log)
    {
      int num;
      string error;
      if (!ArgUtility.TryGetInt(command, 1, out num, out error, "int count"))
      {
        DebugCommands.LogArgError(log, command, error);
      }
      else
      {
        if (!(Game1.currentLocation is Farm currentLocation))
          return;
        for (int index1 = 0; index1 < num; ++index1)
        {
          for (int index2 = 0; index2 < 20; ++index2)
          {
            bool flag = Game1.random.NextBool();
            Building constructed;
            if (currentLocation.buildStructure(flag ? "Deluxe Coop" : "Deluxe Barn", currentLocation.getRandomTile(), Game1.player, out constructed))
            {
              constructed.daysOfConstructionLeft.Value = 0;
              constructed.doAction(Utility.PointToVector2(constructed.animalDoor.Value) + new Vector2((float) constructed.tileX.Value, (float) constructed.tileY.Value), Game1.player);
              for (int index3 = 0; index3 < 16 /*0x10*/; ++index3)
                Utility.addAnimalToFarm(new FarmAnimal(flag ? "White Chicken" : "Cow", (long) Game1.random.Next(int.MaxValue), Game1.player.UniqueMultiplayerID));
              break;
            }
          }
        }
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void SetupFishPondFarm(string[] command, IGameLogger log)
    {
      int num1;
      string error;
      if (!ArgUtility.TryGetOptionalInt(command, 1, out num1, out error, 10, "int population"))
      {
        DebugCommands.LogArgError(log, command, error);
      }
      else
      {
        Game1.game1.parseDebugInput("ClearFarm", log);
        for (int index1 = 4; index1 < 77; index1 += 6)
        {
          for (int index2 = 9; index2 < 60; index2 += 6)
            Game1.game1.parseDebugInput($"{"Build"} \"Fish Pond\" {index1} {index2}", log);
        }
        foreach (Building building in Game1.getFarm().buildings)
        {
          if (building is FishPond fishPond)
          {
            int num2 = Game1.random.Next(128 /*0x80*/, 159);
            if (Game1.random.NextDouble() < 0.15)
              num2 = Game1.random.Next(698, 724);
            if (Game1.random.NextDouble() < 0.05)
              num2 = Game1.random.Next(796, 801);
            ParsedItemData data = ItemRegistry.GetData(num2.ToString());
            if ((data != null ? (data.Category == -4 ? 1 : 0) : 0) != 0)
              fishPond.fishType.Value = num2.ToString();
            else
              fishPond.fishType.Value = Game1.random.Choose<string>("393", "397");
            fishPond.maxOccupants.Value = 10;
            fishPond.currentOccupants.Value = num1;
            fishPond.GetFishObject();
          }
        }
        Game1.game1.parseDebugInput("DayUpdate 1", log);
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void Grass(string[] command, IGameLogger log)
    {
      GameLocation currentLocation = Game1.currentLocation;
      if (currentLocation == null)
        return;
      for (int x = 0; x < currentLocation.Map.Layers[0].LayerWidth; ++x)
      {
        for (int y = 0; y < currentLocation.Map.Layers[0].LayerHeight; ++y)
        {
          if (currentLocation.CanItemBePlacedHere(new Vector2((float) x, (float) y), true, ignorePassables: CollisionMask.None))
            currentLocation.terrainFeatures.Add(new Vector2((float) x, (float) y), (TerrainFeature) new Grass(1, 4));
        }
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void SetupBigFarm(string[] command, IGameLogger log)
    {
      Farm farm = Game1.getFarm();
      Game1.game1.parseDebugInput("ClearFarm", log);
      Game1.game1.parseDebugInput("Build \"Deluxe Coop\" 4 9", log);
      Game1.game1.parseDebugInput("Build \"Deluxe Coop\" 10 9", log);
      Game1.game1.parseDebugInput("Build \"Deluxe Coop\" 36 11", log);
      Game1.game1.parseDebugInput("Build \"Deluxe Barn\" 16 9", log);
      Game1.game1.parseDebugInput("Build \"Deluxe Barn\" 3 16", log);
      Game1.game1.parseDebugInput("Build Mill 30 20", log);
      Game1.game1.parseDebugInput("Build Stable 46 10", log);
      Game1.game1.parseDebugInput("Build Silo 54 14", log);
      Game1.game1.parseDebugInput("Build \"Junimo Hut\" 48 52", log);
      Game1.game1.parseDebugInput("Build \"Junimo Hut\" 55 52", log);
      Game1.game1.parseDebugInput("Build \"Junimo Hut\" 59 52", log);
      Game1.game1.parseDebugInput("Build \"Junimo Hut\" 65 52", log);
      using (List<Building>.Enumerator enumerator = farm.buildings.GetEnumerator())
      {
label_8:
        while (enumerator.MoveNext())
        {
          Building current = enumerator.Current;
          if (current.GetIndoors() is AnimalHouse indoors)
          {
            BuildingData buildingData = current.GetData();
            string[] array = Game1.farmAnimalData.Where<KeyValuePair<string, FarmAnimalData>>((Func<KeyValuePair<string, FarmAnimalData>, bool>) (p => p.Value.House != null && buildingData.ValidOccupantTypes.Contains(p.Value.House))).Select<KeyValuePair<string, FarmAnimalData>, string>((Func<KeyValuePair<string, FarmAnimalData>, string>) (p => p.Key)).ToArray<string>();
            int num = 0;
            while (true)
            {
              if (num < indoors.animalLimit.Value && !indoors.isFull())
              {
                FarmAnimal animal = new FarmAnimal(Game1.random.ChooseFrom<string>((IList<string>) array), (long) Game1.random.Next(int.MaxValue), Game1.player.UniqueMultiplayerID);
                if (Game1.random.NextBool())
                  animal.growFully();
                indoors.adoptAnimal(animal);
                ++num;
              }
              else
                goto label_8;
            }
          }
        }
      }
      foreach (Building building in farm.buildings)
        building.doAction(Utility.PointToVector2(building.animalDoor.Value) + new Vector2((float) building.tileX.Value, (float) building.tileY.Value), Game1.player);
      for (int x = 11; x < 23; ++x)
      {
        for (int y = 14; y < 25; ++y)
          farm.terrainFeatures.Add(new Vector2((float) x, (float) y), (TerrainFeature) new Grass(1, 4));
      }
      for (int x = 3; x < 23; ++x)
      {
        for (int y = 57; y < 61; ++y)
          farm.terrainFeatures.Add(new Vector2((float) x, (float) y), (TerrainFeature) new Grass(1, 4));
      }
      for (int y = 17; y < 25; ++y)
        farm.terrainFeatures.Add(new Vector2(64f, (float) y), (TerrainFeature) new Flooring("6"));
      for (int x = 35; x < 64 /*0x40*/; ++x)
        farm.terrainFeatures.Add(new Vector2((float) x, 24f), (TerrainFeature) new Flooring("6"));
      for (int x = 38; x < 76; ++x)
      {
        for (int y = 18; y < 52; ++y)
        {
          if (farm.CanItemBePlacedHere(new Vector2((float) x, (float) y), true, ignorePassables: CollisionMask.None))
          {
            HoeDirt hoeDirt = new HoeDirt();
            farm.terrainFeatures.Add(new Vector2((float) x, (float) y), (TerrainFeature) hoeDirt);
            hoeDirt.plant((472 + Game1.random.Next(5)).ToString(), Game1.player, false);
          }
        }
      }
      Game1.game1.parseDebugInput("GrowCrops 8", log);
      Vector2[] vector2Array = new Vector2[18]
      {
        new Vector2(8f, 25f),
        new Vector2(11f, 25f),
        new Vector2(14f, 25f),
        new Vector2(17f, 25f),
        new Vector2(20f, 25f),
        new Vector2(23f, 25f),
        new Vector2(8f, 28f),
        new Vector2(11f, 28f),
        new Vector2(14f, 28f),
        new Vector2(17f, 28f),
        new Vector2(20f, 28f),
        new Vector2(23f, 28f),
        new Vector2(8f, 31f),
        new Vector2(11f, 31f),
        new Vector2(14f, 31f),
        new Vector2(17f, 31f),
        new Vector2(20f, 31f),
        new Vector2(23f, 31f)
      };
      NetVector2Dictionary<TerrainFeature, NetRef<TerrainFeature>> terrainFeatures = farm.terrainFeatures;
      foreach (Vector2 key in vector2Array)
        terrainFeatures.Add(key, (TerrainFeature) new FruitTree((628 + Game1.random.Next(2)).ToString(), 4));
      for (int x = 3; x < 15; ++x)
      {
        for (int y = 36; y < 45; ++y)
        {
          if (farm.CanItemBePlacedHere(new Vector2((float) x, (float) y)))
          {
            Object @object = ItemRegistry.Create<Object>("(BC)12");
            farm.objects.Add(new Vector2((float) x, (float) y), @object);
            @object.performObjectDropInAction((Item) ItemRegistry.Create<Object>("(O)454"), false, Game1.player);
          }
        }
      }
      for (int x = 16 /*0x10*/; x < 26; ++x)
      {
        for (int y = 36; y < 45; ++y)
        {
          if (farm.CanItemBePlacedHere(new Vector2((float) x, (float) y)))
            farm.objects.Add(new Vector2((float) x, (float) y), ItemRegistry.Create<Object>("(BC)13"));
        }
      }
      for (int x = 3; x < 15; ++x)
      {
        for (int y = 47; y < 57; ++y)
        {
          if (farm.CanItemBePlacedHere(new Vector2((float) x, (float) y)))
            farm.objects.Add(new Vector2((float) x, (float) y), ItemRegistry.Create<Object>("(BC)16"));
        }
      }
      for (int x = 16 /*0x10*/; x < 26; ++x)
      {
        for (int y = 47; y < 57; ++y)
        {
          if (farm.CanItemBePlacedHere(new Vector2((float) x, (float) y)))
            farm.objects.Add(new Vector2((float) x, (float) y), ItemRegistry.Create<Object>("(BC)15"));
        }
      }
      for (int x = 28; x < 38; ++x)
      {
        for (int y = 26; y < 46; ++y)
        {
          if (farm.CanItemBePlacedHere(new Vector2((float) x, (float) y)))
            new Torch().placementAction((GameLocation) farm, x * 64 /*0x40*/, y * 64 /*0x40*/);
        }
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    [OtherNames(new string[] {"hu", "house"})]
    public static void HouseUpgrade(string[] command, IGameLogger log)
    {
      int num;
      string error;
      if (!ArgUtility.TryGetInt(command, 1, out num, out error, "int upgradeLevel"))
      {
        DebugCommands.LogArgError(log, command, error);
      }
      else
      {
        Utility.getHomeOfFarmer(Game1.player).moveObjectsForHouseUpgrade(num);
        Utility.getHomeOfFarmer(Game1.player).setMapForUpgradeLevel(num);
        Game1.player.HouseUpgradeLevel = num;
        Game1.addNewFarmBuildingMaps();
        Utility.getHomeOfFarmer(Game1.player).ReadWallpaperAndFloorTileData();
        Utility.getHomeOfFarmer(Game1.player).RefreshFloorObjectNeighbors();
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    [OtherNames(new string[] {"thu", "thishouse"})]
    public static void ThisHouseUpgrade(string[] command, IGameLogger log)
    {
      int num;
      string error;
      if (!ArgUtility.TryGetInt(command, 1, out num, out error, "int upgradeLevel"))
      {
        DebugCommands.LogArgError(log, command, error);
      }
      else
      {
        if (!(Game1.currentLocation?.getBuildingAt(Game1.player.Tile + new Vector2(0.0f, -1f))?.GetIndoors() is FarmHouse farmHouse1))
          farmHouse1 = Game1.currentLocation as FarmHouse;
        FarmHouse farmHouse2 = farmHouse1;
        if (farmHouse2 == null)
          return;
        farmHouse2.moveObjectsForHouseUpgrade(num);
        farmHouse2.setMapForUpgradeLevel(num);
        farmHouse2.upgradeLevel = num;
        Game1.addNewFarmBuildingMaps();
        farmHouse2.ReadWallpaperAndFloorTileData();
        farmHouse2.RefreshFloorObjectNeighbors();
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    [OtherNames(new string[] {"ci"})]
    public static void Clear(string[] command, IGameLogger log) => Game1.player.clearBackpack();

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    [OtherNames(new string[] {"w"})]
    public static void Wall(string[] command, IGameLogger log)
    {
      string which;
      string error;
      if (!ArgUtility.TryGet(command, 1, out which, out error, false, "string wallpaperId"))
        DebugCommands.LogArgError(log, command, error);
      else
        Game1.RequireLocation<FarmHouse>("FarmHouse").SetWallpaper(which, (string) null);
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void Floor(string[] command, IGameLogger log)
    {
      string which;
      string error;
      if (!ArgUtility.TryGet(command, 1, out which, out error, false, "string floorId"))
        DebugCommands.LogArgError(log, command, error);
      else
        Game1.RequireLocation<FarmHouse>("FarmHouse").SetFloor(which, (string) null);
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void Sprinkle(string[] command, IGameLogger log)
    {
      Utility.addSprinklesToLocation(Game1.currentLocation, Game1.player.TilePoint.X, Game1.player.TilePoint.Y, 7, 7, 2000, 100, Color.White);
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void ClearMail(string[] command, IGameLogger log)
    {
      Game1.player.mailReceived.Clear();
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void BroadcastMailbox(string[] command, IGameLogger log)
    {
      string mailName;
      string error;
      if (!ArgUtility.TryGet(command, 1, out mailName, out error, false, "string mailId"))
        DebugCommands.LogArgError(log, command, error);
      else
        Game1.addMail(mailName, sendToEveryone: true);
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    [OtherNames(new string[] {"mft"})]
    public static void MailForTomorrow(string[] command, IGameLogger log)
    {
      string mailName;
      string error;
      if (!ArgUtility.TryGet(command, 1, out mailName, out error, false, "string mailId"))
        DebugCommands.LogArgError(log, command, error);
      else
        Game1.addMailForTomorrow(mailName, command.Length > 2);
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void AllMail(string[] command, IGameLogger log)
    {
      foreach (string key in DataLoader.Mail(Game1.content).Keys)
        Game1.addMailForTomorrow(key);
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void AllMailRead(string[] command, IGameLogger log)
    {
      foreach (string key in DataLoader.Mail(Game1.content).Keys)
        Game1.player.mailReceived.Add(key);
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void ShowMail(string[] command, IGameLogger log)
    {
      string str;
      string error;
      if (!ArgUtility.TryGet(command, 1, out str, out error, false, "string mailId"))
        DebugCommands.LogArgError(log, command, error);
      else
        Game1.activeClickableMenu = (IClickableMenu) new LetterViewerMenu(DataLoader.Mail(Game1.content).GetValueOrDefault<string, string>(str, ""), str);
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    [OtherNames(new string[] {"where"})]
    public static void WhereIs(string[] command, IGameLogger log)
    {
      string error;
      string npcName;
      if (!ArgUtility.TryGet(command, 1, out npcName, out error, false, "string npcName"))
      {
        DebugCommands.LogArgError(log, command, error);
      }
      else
      {
        List<string> lines = new List<string>();
        if (Game1.CurrentEvent != null)
        {
          foreach (NPC actor in Game1.CurrentEvent.actors)
          {
            if (Utility.fuzzyCompare(npcName, actor.Name).HasValue)
              lines.Add($"{actor.Name} is in this event at ({actor.TilePoint.X}, {actor.TilePoint.Y})");
          }
        }
        Utility.ForEachCharacter((Func<NPC, bool>) (character =>
        {
          if (Utility.fuzzyCompare(npcName, character.Name).HasValue)
            lines.Add($"'{character.Name}'{(character.EventActor ? " (event actor)" : "")} is at {character.currentLocation.NameOrUniqueName} ({character.TilePoint.X}, {character.TilePoint.Y})");
          return true;
        }), true);
        if (lines.Any<string>())
          log.Info(string.Join("\n", (IEnumerable<string>) lines));
        else
          log.Error($"No NPC found matching '{npcName}'.");
      }
    }

    /// <summary>List the locations of every item in the game state matching a given item ID or name.</summary>
    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    [OtherNames(new string[] {"whereItem"})]
    public static void WhereIsItem(string[] command, IGameLogger log)
    {
      string error;
      string itemNameOrId;
      if (!ArgUtility.TryGet(command, 1, out itemNameOrId, out error, false, "string itemNameOrId"))
      {
        DebugCommands.LogArgError(log, command, error);
      }
      else
      {
        string itemId = ItemRegistry.GetData(itemNameOrId)?.QualifiedItemId;
        List<string> lines = new List<string>();
        long count = 0;
        Utility.ForEachItemContext((ForEachItemDelegate) ((in ForEachItemContext context) =>
        {
          Item obj = context.Item;
          int num;
          if (itemId == null)
          {
            int? nullable = Utility.fuzzyCompare(itemNameOrId, obj.Name);
            if (!nullable.HasValue)
            {
              nullable = Utility.fuzzyCompare(itemNameOrId, obj.DisplayName);
              num = nullable.HasValue ? 1 : 0;
            }
            else
              num = 1;
          }
          else
            num = obj.QualifiedItemId == itemId ? 1 : 0;
          if (num != 0)
          {
            count += (long) Math.Min(obj.Stack, 1);
            List<string> stringList = lines;
            DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(7, 3);
            interpolatedStringHandler.AppendLiteral("  - ");
            interpolatedStringHandler.AppendFormatted(string.Join(" > ", (IEnumerable<string>) context.GetDisplayPath(true)));
            interpolatedStringHandler.AppendLiteral(" (");
            interpolatedStringHandler.AppendFormatted(obj.QualifiedItemId);
            ref DefaultInterpolatedStringHandler local = ref interpolatedStringHandler;
            string str;
            if (obj.Stack <= 1)
              str = "";
            else
              str = $" x {obj.Stack}";
            local.AppendFormatted(str);
            interpolatedStringHandler.AppendLiteral(")");
            string stringAndClear = interpolatedStringHandler.ToStringAndClear();
            stringList.Add(stringAndClear);
          }
          return true;
        }));
        string str1 = itemId != null ? $"ID '{itemId}'" : $"name '{itemNameOrId}'";
        if (lines.Any<string>())
          log.Info($"Found {count} item{(count > 1L ? "s" : "")} matching {str1}:\n{string.Join("\n", (IEnumerable<string>) lines)}");
        else
          log.Error($"No item found matching {str1}.");
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    [OtherNames(new string[] {"pm"})]
    public static void PanMode(string[] command, IGameLogger log)
    {
      string str;
      string error;
      if (!ArgUtility.TryGetOptional(command, 1, out str, out error, allowBlank: false, name: "string option"))
        DebugCommands.LogArgError(log, command, error);
      else if (str == null)
      {
        if (!Game1.panMode)
        {
          Game1.panMode = true;
          Game1.viewportFreeze = true;
          Game1.debugMode = true;
          Game1.game1.panFacingDirectionWait = false;
          Game1.game1.panModeString = "";
          log.Info("Screen pan mode enabled.");
        }
        else
        {
          Game1.panMode = false;
          Game1.viewportFreeze = false;
          Game1.game1.panModeString = "";
          Game1.debugMode = false;
          Game1.game1.panFacingDirectionWait = false;
          Game1.inputSimulator = (IInputSimulator) null;
          log.Info("Screen pan mode disabled.");
        }
      }
      else if (Game1.panMode)
      {
        if (str == "clear")
        {
          Game1.game1.panModeString = "";
          Game1.game1.panFacingDirectionWait = false;
        }
        else
        {
          int num;
          if (ArgUtility.TryGetInt(command, 1, out num, out string _, "int time"))
          {
            if (Game1.game1.panFacingDirectionWait)
              return;
            Game1 game1 = Game1.game1;
            game1.panModeString = $"{game1.panModeString}{(Game1.game1.panModeString.Length > 0 ? "/" : "")}{num.ToString()} ";
            log.Info(Game1.game1.panModeString + Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3191"));
          }
          else
            DebugCommands.LogArgError(log, command, "the first argument must be omitted (to toggle pan mode), 'clear', or a numeric time");
        }
      }
      else
        log.Error("Screen pan mode isn't enabled. You can enable it by using this command without arguments.");
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    [OtherNames(new string[] {"is"})]
    public static void InputSim(string[] command, IGameLogger log)
    {
      string str;
      string error;
      if (!ArgUtility.TryGet(command, 1, out str, out error, false, "string option"))
      {
        DebugCommands.LogArgError(log, command, error);
      }
      else
      {
        Game1.inputSimulator = (IInputSimulator) null;
        switch (str.ToLower())
        {
          case "spamtool":
            Game1.inputSimulator = (IInputSimulator) new ToolSpamInputSimulator();
            break;
          case "spamlr":
            Game1.inputSimulator = (IInputSimulator) new LeftRightClickSpamInputSimulator();
            break;
          default:
            log.Error("No input simulator found for " + str);
            break;
        }
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void Hurry(string[] command, IGameLogger log)
    {
      string query;
      string error;
      if (!ArgUtility.TryGet(command, 1, out query, out error, false, "string npcName"))
        DebugCommands.LogArgError(log, command, error);
      else
        Utility.fuzzyCharacterSearch(query).warpToPathControllerDestination();
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void MorePollen(string[] command, IGameLogger log)
    {
      int num;
      string error;
      if (!ArgUtility.TryGetInt(command, 1, out num, out error, "int amount"))
      {
        DebugCommands.LogArgError(log, command, error);
      }
      else
      {
        for (int index = 0; index < num; ++index)
          Game1.debrisWeather.Add(new WeatherDebris(new Vector2((float) Game1.random.Next(0, Game1.graphics.GraphicsDevice.Viewport.Width), (float) Game1.random.Next(0, Game1.graphics.GraphicsDevice.Viewport.Height)), 0, (float) Game1.random.Next(15) / 500f, (float) Game1.random.Next(-10, 0) / 50f, (float) Game1.random.Next(10) / 50f));
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void FillWithObject(string[] command, IGameLogger log)
    {
      string str1;
      string error;
      bool flag;
      if (!ArgUtility.TryGet(command, 1, out str1, out error, false, "string id") || !ArgUtility.TryGetOptionalBool(command, 2, out flag, out error, name: "bool bigCraftable"))
      {
        DebugCommands.LogArgError(log, command, error);
      }
      else
      {
        for (int y = 0; y < Game1.currentLocation.map.Layers[0].LayerHeight; ++y)
        {
          for (int x = 0; x < Game1.currentLocation.map.Layers[0].LayerWidth; ++x)
          {
            Vector2 vector2 = new Vector2((float) x, (float) y);
            if (Game1.currentLocation.CanItemBePlacedHere(vector2))
            {
              string str2 = flag ? "(BC)" : "(O)";
              Game1.currentLocation.setObject(vector2, ItemRegistry.Create<Object>(str2 + str1));
            }
          }
        }
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void SpawnWeeds(string[] command, IGameLogger log)
    {
      int num;
      string error;
      if (!ArgUtility.TryGetInt(command, 1, out num, out error, "int spawnPasses"))
      {
        DebugCommands.LogArgError(log, command, error);
      }
      else
      {
        for (int index = 0; index < num; ++index)
          Game1.currentLocation.spawnWeedsAndStones(1);
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void BusDriveBack(string[] command, IGameLogger log)
    {
      Game1.RequireLocation<BusStop>("BusStop").busDriveBack();
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void BusDriveOff(string[] command, IGameLogger log)
    {
      Game1.RequireLocation<BusStop>("BusStop").busDriveOff();
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void CompleteJoja(string[] command, IGameLogger log)
    {
      Game1.player.mailReceived.Add("ccCraftsRoom");
      Game1.player.mailReceived.Add("ccVault");
      Game1.player.mailReceived.Add("ccFishTank");
      Game1.player.mailReceived.Add("ccBoilerRoom");
      Game1.player.mailReceived.Add("ccPantry");
      Game1.player.mailReceived.Add("jojaCraftsRoom");
      Game1.player.mailReceived.Add("jojaVault");
      Game1.player.mailReceived.Add("jojaFishTank");
      Game1.player.mailReceived.Add("jojaBoilerRoom");
      Game1.player.mailReceived.Add("jojaPantry");
      Game1.player.mailReceived.Add("JojaMember");
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void CompleteCc(string[] command, IGameLogger log)
    {
      Game1.player.mailReceived.Add("ccCraftsRoom");
      Game1.player.mailReceived.Add("ccVault");
      Game1.player.mailReceived.Add("ccFishTank");
      Game1.player.mailReceived.Add("ccBoilerRoom");
      Game1.player.mailReceived.Add("ccPantry");
      Game1.player.mailReceived.Add("ccBulletin");
      Game1.player.mailReceived.Add("ccBoilerRoom");
      Game1.player.mailReceived.Add("ccPantry");
      Game1.player.mailReceived.Add("ccBulletin");
      CommunityCenter communityCenter = Game1.RequireLocation<CommunityCenter>("CommunityCenter");
      for (int index = 0; index < communityCenter.areasComplete.Count; ++index)
      {
        communityCenter.markAreaAsComplete(index);
        communityCenter.areasComplete[index] = true;
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void Break(string[] command, IGameLogger log)
    {
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void WhereOre(string[] command, IGameLogger log)
    {
      log.Info(Convert.ToString((object) Game1.currentLocation.orePanPoint.Value));
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void AllBundles(string[] command, IGameLogger log)
    {
      foreach (KeyValuePair<int, NetArray<bool, NetBool>> keyValuePair in Game1.RequireLocation<CommunityCenter>("CommunityCenter").bundles.FieldDict)
      {
        for (int index = 0; index < keyValuePair.Value.Count; ++index)
          keyValuePair.Value[index] = true;
      }
      Game1.playSound("crystal", new int?(0));
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void JunimoGoodbye(string[] command, IGameLogger log)
    {
      if (!(Game1.currentLocation is CommunityCenter currentLocation))
        log.Error("The JunimoGoodbye command must be run while inside the community center.");
      else
        currentLocation.junimoGoodbyeDance();
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void Bundle(string[] command, IGameLogger log)
    {
      int num;
      string error;
      if (!ArgUtility.TryGetInt(command, 1, out num, out error, "int bundleKey"))
      {
        DebugCommands.LogArgError(log, command, error);
      }
      else
      {
        foreach (KeyValuePair<int, NetArray<bool, NetBool>> keyValuePair in Game1.RequireLocation<CommunityCenter>("CommunityCenter").bundles.FieldDict)
        {
          if (keyValuePair.Key == num)
          {
            for (int index = 0; index < keyValuePair.Value.Count; ++index)
              keyValuePair.Value[index] = true;
          }
        }
        Game1.playSound("crystal", new int?(0));
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    [OtherNames(new string[] {"lu"})]
    public static void Lookup(string[] command, IGameLogger log)
    {
      string str;
      string error;
      if (!ArgUtility.TryGetRemainder(command, 1, out str, out error, name: "string search"))
      {
        DebugCommands.LogArgError(log, command, error);
      }
      else
      {
        foreach (ParsedItemData parsedItemData in ItemRegistry.GetObjectTypeDefinition().GetAllData())
        {
          if (parsedItemData.InternalName.EqualsIgnoreCase(str))
            log.Info($"{parsedItemData.InternalName} {parsedItemData.ItemId}");
        }
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void CcLoadCutscene(string[] command, IGameLogger log)
    {
      int whichArea;
      string error;
      if (!ArgUtility.TryGetInt(command, 1, out whichArea, out error, "int areaId"))
        DebugCommands.LogArgError(log, command, error);
      else
        Game1.RequireLocation<CommunityCenter>("CommunityCenter").restoreAreaCutscene(whichArea);
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void CcLoad(string[] command, IGameLogger log)
    {
      int area;
      string error;
      if (!ArgUtility.TryGetInt(command, 1, out area, out error, "int areaId"))
      {
        DebugCommands.LogArgError(log, command, error);
      }
      else
      {
        Game1.RequireLocation<CommunityCenter>("CommunityCenter").loadArea(area);
        Game1.RequireLocation<CommunityCenter>("CommunityCenter").markAreaAsComplete(area);
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void Plaque(string[] command, IGameLogger log)
    {
      Game1.RequireLocation<CommunityCenter>("CommunityCenter").addStarToPlaque();
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void JunimoStar(string[] command, IGameLogger log)
    {
      CommunityCenter location = Game1.RequireLocation<CommunityCenter>("CommunityCenter");
      Junimo junimo = location.characters.OfType<Junimo>().FirstOrDefault<Junimo>();
      if (junimo == null)
        log.Error("No Junimo found in the community center.");
      else
        junimo.returnToJunimoHutToFetchStar((GameLocation) location);
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    [OtherNames(new string[] {"j", "aj"})]
    public static void AddJunimo(string[] command, IGameLogger log)
    {
      Vector2 vector2;
      string error;
      int whichArea;
      if (!ArgUtility.TryGetVector2(command, 1, out vector2, out error, true, "Vector2 tile") || !ArgUtility.TryGetInt(command, 3, out whichArea, out error, "int areaId"))
        DebugCommands.LogArgError(log, command, error);
      else
        Game1.RequireLocation<CommunityCenter>("CommunityCenter").addCharacter((NPC) new Junimo(vector2 * 64f, whichArea));
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void ResetJunimoNotes(string[] command, IGameLogger log)
    {
      foreach (NetArray<bool, NetBool> netArray in Game1.RequireLocation<CommunityCenter>("CommunityCenter").bundles.FieldDict.Values)
      {
        for (int index = 0; index < netArray.Count; ++index)
          netArray[index] = false;
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    [OtherNames(new string[] {"jn"})]
    public static void JunimoNote(string[] command, IGameLogger log)
    {
      int area;
      string error;
      if (!ArgUtility.TryGetInt(command, 1, out area, out error, "int areaId"))
        DebugCommands.LogArgError(log, command, error);
      else
        Game1.RequireLocation<CommunityCenter>("CommunityCenter").addJunimoNote(area);
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void WaterColor(string[] command, IGameLogger log)
    {
      int r;
      string error;
      int g;
      int b;
      if (!ArgUtility.TryGetInt(command, 1, out r, out error, "int red") || !ArgUtility.TryGetInt(command, 2, out g, out error, "int green") || !ArgUtility.TryGetInt(command, 3, out b, out error, "int blue"))
        DebugCommands.LogArgError(log, command, error);
      else
        Game1.currentLocation.waterColor.Value = new Color(r, g, b) * 0.5f;
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void FestivalScore(string[] command, IGameLogger log)
    {
      int num;
      string error;
      if (!ArgUtility.TryGetInt(command, 1, out num, out error, "int score"))
        DebugCommands.LogArgError(log, command, error);
      else
        Game1.player.festivalScore += num;
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void AddOtherFarmer(string[] command, IGameLogger log)
    {
      Farmer farmer = new Farmer(new FarmerSprite("Characters\\Farmer\\farmer_base"), new Vector2(Game1.player.Position.X - 64f, Game1.player.Position.Y), 2, Dialogue.randomName(), (List<Item>) null, true);
      farmer.changeShirt(Game1.random.Next(1000, 1040).ToString());
      farmer.changePantsColor(new Color(Game1.random.Next((int) byte.MaxValue), Game1.random.Next((int) byte.MaxValue), Game1.random.Next((int) byte.MaxValue)));
      farmer.changeHairStyle(Game1.random.Next(FarmerRenderer.hairStylesTexture.Height / 96 /*0x60*/ * 8));
      if (Game1.random.NextBool())
        farmer.changeHat(Game1.random.Next(-1, FarmerRenderer.hatsTexture.Height / 80 /*0x50*/ * 12));
      else
        Game1.player.changeHat(-1);
      farmer.changeHairColor(new Color(Game1.random.Next((int) byte.MaxValue), Game1.random.Next((int) byte.MaxValue), Game1.random.Next((int) byte.MaxValue)));
      farmer.changeSkinColor(Game1.random.Next(16 /*0x10*/));
      farmer.currentLocation = Game1.currentLocation;
      Game1.otherFarmers.Add((long) Game1.random.Next(), farmer);
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void PlayMusic(string[] command, IGameLogger log)
    {
      string newTrackName;
      string error;
      if (!ArgUtility.TryGet(command, 1, out newTrackName, out error, false, "string trackName"))
        DebugCommands.LogArgError(log, command, error);
      else
        Game1.changeMusicTrack(newTrackName);
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void Jump(string[] command, IGameLogger log)
    {
      string query;
      string error;
      float jumpVelocity;
      if (!ArgUtility.TryGet(command, 1, out query, out error, false, "string target") || !ArgUtility.TryGetOptionalFloat(command, 2, out jumpVelocity, out error, 8f, "float jumpVelocity"))
        DebugCommands.LogArgError(log, command, error);
      else if (query == "farmer")
        Game1.player.jump(jumpVelocity);
      else
        Utility.fuzzyCharacterSearch(query).jump(jumpVelocity);
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void Toss(string[] command, IGameLogger log)
    {
      Game1.currentLocation.TemporarySprites.Add(new TemporaryAnimatedSprite(738, 2700f, 1, 0, Game1.player.Tile * 64f, false, false)
      {
        rotationChange = (float) Math.PI / 32f,
        motion = new Vector2(0.0f, -6f),
        acceleration = new Vector2(0.0f, 0.08f)
      });
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void Rain(string[] command, IGameLogger log)
    {
      string locationContextId = Game1.player.currentLocation.GetLocationContextId();
      LocationWeather weatherForLocation = Game1.netWorldState.Value.GetWeatherForLocation(locationContextId);
      weatherForLocation.IsRaining = !weatherForLocation.IsRaining;
      weatherForLocation.IsDebrisWeather = false;
      if (!(locationContextId == "Default"))
        return;
      Game1.isRaining = weatherForLocation.IsRaining;
      Game1.isDebrisWeather = false;
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void GreenRain(string[] command, IGameLogger log)
    {
      string locationContextId = Game1.player.currentLocation.GetLocationContextId();
      LocationWeather weatherForLocation = Game1.netWorldState.Value.GetWeatherForLocation(locationContextId);
      weatherForLocation.IsGreenRain = !weatherForLocation.IsGreenRain;
      weatherForLocation.IsDebrisWeather = false;
      if (!(locationContextId == "Default"))
        return;
      Game1.isRaining = weatherForLocation.IsRaining;
      Game1.isGreenRain = weatherForLocation.IsGreenRain;
      Game1.isDebrisWeather = false;
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    [OtherNames(new string[] {"sf"})]
    public static void SetFrame(string[] command, IGameLogger log)
    {
      int which;
      string error;
      if (!ArgUtility.TryGetInt(command, 1, out which, out error, "int animationId"))
      {
        DebugCommands.LogArgError(log, command, error);
      }
      else
      {
        Game1.player.FarmerSprite.PauseForSingleAnimation = true;
        Game1.player.FarmerSprite.setCurrentSingleAnimation(which);
      }
    }

    /// <summary>Immediately end the current event.</summary>
    [OtherNames(new string[] {"ee"})]
    public static void EndEvent(string[] command, IGameLogger log)
    {
      Event currentEvent = Game1.CurrentEvent;
      if (currentEvent == null)
      {
        log.Warn("Can't end an event because there's none playing.");
      }
      else
      {
        if (currentEvent.id == "1590166")
          Game1.player.mailReceived.Add("rejectedPet");
        currentEvent.skipped = true;
        currentEvent.skipEvent();
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void Language(string[] command, IGameLogger log)
    {
      Game1.activeClickableMenu = (IClickableMenu) new LanguageSelectionMenu();
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    [OtherNames(new string[] {"rte"})]
    public static void RunTestEvent(string[] command, IGameLogger log) => Game1.runTestEvent();

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    [OtherNames(new string[] {"qb"})]
    public static void QiBoard(string[] command, IGameLogger log)
    {
      Game1.activeClickableMenu = (IClickableMenu) new SpecialOrdersBoard("Qi");
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    [OtherNames(new string[] {"ob"})]
    public static void OrdersBoard(string[] command, IGameLogger log)
    {
      Game1.activeClickableMenu = (IClickableMenu) new SpecialOrdersBoard();
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void ReturnedDonations(string[] command, IGameLogger log)
    {
      Game1.player.team.CheckReturnedDonations();
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    [OtherNames(new string[] {"cso"})]
    public static void CompleteSpecialOrders(string[] command, IGameLogger log)
    {
      foreach (SpecialOrder specialOrder in Game1.player.team.specialOrders)
      {
        foreach (OrderObjective objective in specialOrder.objectives)
          objective.SetCount(objective.maxCount.Value);
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void SpecialOrder(string[] command, IGameLogger log)
    {
      string id;
      string error;
      if (!ArgUtility.TryGet(command, 1, out id, out error, false, "string orderId"))
        DebugCommands.LogArgError(log, command, error);
      else
        Game1.player.team.AddSpecialOrder(id);
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void BoatJourney(string[] command, IGameLogger log)
    {
      Game1.currentMinigame = (IMinigame) new BoatJourney();
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void Minigame(string[] command, IGameLogger log)
    {
      string str;
      string error;
      if (!ArgUtility.TryGet(command, 1, out str, out error, false, "string minigame"))
      {
        DebugCommands.LogArgError(log, command, error);
      }
      else
      {
        if (str == null)
          return;
        switch (str.Length)
        {
          case 5:
            switch (str[0])
            {
              case 'i':
                if (!(str == "intro"))
                  return;
                Game1.currentMinigame = (IMinigame) new Intro();
                return;
              case 'p':
                if (!(str == "plane"))
                  return;
                Game1.currentMinigame = (IMinigame) new PlaneFlyBy();
                return;
              case 's':
                if (!(str == "slots"))
                  return;
                Game1.currentMinigame = (IMinigame) new Slots();
                return;
              default:
                return;
            }
          case 6:
            switch (str[0])
            {
              case 'c':
                if (!(str == "cowboy"))
                  return;
                Game1.updateViewportForScreenSizeChange(false, Game1.graphics.PreferredBackBufferWidth, Game1.graphics.PreferredBackBufferHeight);
                Game1.currentMinigame = (IMinigame) new AbigailGame();
                return;
              case 't':
                if (!(str == "target"))
                  return;
                Game1.currentMinigame = (IMinigame) new TargetGame();
                return;
              default:
                return;
            }
          case 7:
            switch (str[0])
            {
              case 'f':
                if (!(str == "fishing"))
                  return;
                Game1.currentMinigame = (IMinigame) new FishingGame();
                return;
              case 'g':
                if (!(str == "grandpa"))
                  return;
                Game1.currentMinigame = (IMinigame) new GrandpaStory();
                return;
              default:
                return;
            }
          case 8:
            switch (str[0])
            {
              case 'b':
                if (!(str == "blastoff"))
                  return;
                Game1.currentMinigame = (IMinigame) new RobotBlastoff();
                return;
              case 'm':
                if (!(str == "minecart"))
                  return;
                Game1.currentMinigame = (IMinigame) new MineCart(0, 3);
                return;
              default:
                return;
            }
          case 9:
            switch (str[0])
            {
              case 'h':
                if (!(str == "haleyCows"))
                  return;
                Game1.currentMinigame = (IMinigame) new HaleyCowPictures();
                return;
              case 'm':
                if (!(str == "marucomet"))
                  return;
                Game1.currentMinigame = (IMinigame) new MaruComet();
                return;
              default:
                return;
            }
        }
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void Event(string[] command, IGameLogger log)
    {
      string query;
      string error;
      int index;
      bool flag;
      if (!ArgUtility.TryGet(command, 1, out query, out error, false, "string locationName") || !ArgUtility.TryGetInt(command, 2, out index, out error, "int eventIndex") || !ArgUtility.TryGetOptionalBool(command, 3, out flag, out error, true, "bool clearEventsSeen"))
      {
        DebugCommands.LogArgError(log, command, error);
      }
      else
      {
        GameLocation gameLocation = Utility.fuzzyLocationSearch(query);
        if (gameLocation == null)
        {
          log.Error("No location with name " + query);
        }
        else
        {
          string locationName = gameLocation.Name;
          if (locationName == "Pool")
            locationName = "BathHouse_Pool";
          if (flag)
            Game1.player.eventsSeen.Clear();
          string assetName = "Data\\Events\\" + locationName;
          KeyValuePair<string, string> entry = Game1.content.Load<Dictionary<string, string>>(assetName).ElementAt<KeyValuePair<string, string>>(index);
          if (!entry.Key.Contains('/'))
            return;
          LocationRequest locationRequest = Game1.getLocationRequest(locationName);
          locationRequest.OnLoad += (LocationRequest.Callback) (() => Game1.currentLocation.currentEvent = new Event(entry.Value, assetName, Event.SplitPreconditions(entry.Key)[0]));
          Game1.warpFarmer(locationRequest, 8, 8, Game1.player.FacingDirection);
        }
      }
    }

    /// <summary>Find an event by ID and play it.</summary>
    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    [OtherNames(new string[] {"ebi"})]
    public static void EventById(string[] command, IGameLogger log)
    {
      string eventId;
      string error;
      if (!ArgUtility.TryGet(command, 1, out eventId, out error, false, "string eventId"))
      {
        DebugCommands.LogArgError(log, command, error);
      }
      else
      {
        Game1.player.eventsSeen.Remove(eventId);
        Game1.eventsSeenSinceLastLocationChange.Remove(eventId);
        if (Game1.PlayEvent(eventId, false, false))
          log.Info("Starting event " + eventId);
        else
          log.Error($"Event '{eventId}' not found.");
      }
    }

    public static void EventScript(string[] command, IGameLogger log)
    {
      string locationName;
      string error;
      string script;
      if (!ArgUtility.TryGet(command, 1, out locationName, out error, name: "string location") || !ArgUtility.TryGetRemainder(command, 2, out script, out error, name: "string script"))
        DebugCommands.LogArgError(log, command, error);
      else if (locationName != Game1.currentLocation.Name)
      {
        LocationRequest locationRequest = Game1.getLocationRequest(locationName);
        locationRequest.OnLoad += (LocationRequest.Callback) (() => Game1.currentLocation.currentEvent = new Event(script));
        int x = 8;
        int y = 8;
        Utility.getDefaultWarpLocation(locationRequest.Name, ref x, ref y);
        Game1.warpFarmer(locationRequest, x, y, Game1.player.FacingDirection);
      }
      else
        Game1.globalFadeToBlack((Game1.afterFadeFunction) (() =>
        {
          Game1.forceSnapOnNextViewportUpdate = true;
          Game1.currentLocation.startEvent(new Event(script));
          Game1.globalFadeToClear();
        }));
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    [OtherNames(new string[] {"sfe"})]
    public static void SetFarmEvent(string[] command, IGameLogger log)
    {
      string key;
      string error;
      if (!ArgUtility.TryGet(command, 1, out key, out error, false, "string eventName"))
      {
        DebugCommands.LogArgError(log, command, error);
      }
      else
      {
        Dictionary<string, Func<FarmEvent>> dictionary = new Dictionary<string, Func<FarmEvent>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
        {
          ["dogs"] = (Func<FarmEvent>) (() => (FarmEvent) new SoundInTheNightEvent(2)),
          ["earthquake"] = (Func<FarmEvent>) (() => (FarmEvent) new SoundInTheNightEvent(4)),
          ["fairy"] = (Func<FarmEvent>) (() => (FarmEvent) new FairyEvent()),
          ["meteorite"] = (Func<FarmEvent>) (() => (FarmEvent) new SoundInTheNightEvent(1)),
          ["owl"] = (Func<FarmEvent>) (() => (FarmEvent) new SoundInTheNightEvent(3)),
          ["racoon"] = (Func<FarmEvent>) (() => (FarmEvent) new SoundInTheNightEvent(5)),
          ["ufo"] = (Func<FarmEvent>) (() => (FarmEvent) new SoundInTheNightEvent(0)),
          ["witch"] = (Func<FarmEvent>) (() => (FarmEvent) new WitchEvent())
        };
        Func<FarmEvent> func;
        if (dictionary.TryGetValue(key, out func))
        {
          Game1.farmEventOverride = func();
          log.Info($"Set farm event to '{key}'! The event will play if no other nightly event plays normally.");
        }
        else
          log.Error($"Unknown event type; expected one of '{string.Join("', '", (IEnumerable<string>) dictionary.Keys)}'.");
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void TestWedding(string[] command, IGameLogger log)
    {
      Event weddingEvent = Utility.getWeddingEvent(Game1.player);
      LocationRequest locationRequest = Game1.getLocationRequest("Town");
      locationRequest.OnLoad += (LocationRequest.Callback) (() => Game1.currentLocation.currentEvent = weddingEvent);
      int x = 8;
      int y = 8;
      Utility.getDefaultWarpLocation(locationRequest.Name, ref x, ref y);
      Game1.warpFarmer(locationRequest, x, y, Game1.player.FacingDirection);
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void Festival(string[] command, IGameLogger log)
    {
      string source;
      string error;
      if (!ArgUtility.TryGet(command, 1, out source, out error, false, "string festivalId"))
      {
        DebugCommands.LogArgError(log, command, error);
      }
      else
      {
        Dictionary<string, string> dictionary = Game1.temporaryContent.Load<Dictionary<string, string>>("Data\\Festivals\\" + source);
        if (dictionary == null)
          return;
        string str1 = new string(source.Where<char>(new Func<char, bool>(char.IsLetter)).ToArray<char>());
        int int32_1 = Convert.ToInt32(new string(source.Where<char>(new Func<char, bool>(char.IsDigit)).ToArray<char>()));
        Game1.game1.parseDebugInput("Season " + str1, log);
        Game1.game1.parseDebugInput($"{"Day"} {int32_1}", log);
        string[] strArray = dictionary["conditions"].Split('/');
        int int32_2 = Convert.ToInt32(ArgUtility.SplitBySpaceAndGet(strArray[1], 0));
        Game1.game1.parseDebugInput($"{"Time"} {int32_2}", log);
        string str2 = strArray[0];
        Game1.game1.parseDebugInput($"Warp {str2} 1 1", log);
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    [OtherNames(new string[] {"ps"})]
    public static void PlaySound(string[] command, IGameLogger log)
    {
      string cueName;
      string error;
      int num;
      if (!ArgUtility.TryGet(command, 1, out cueName, out error, false, "string soundId") || !ArgUtility.TryGetOptionalInt(command, 2, out num, out error, -1, "int pitch"))
        DebugCommands.LogArgError(log, command, error);
      else if (num > -1)
        Game1.playSound(cueName, new int?(num));
      else
        Game1.playSound(cueName);
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void LogSounds(string[] command, IGameLogger log)
    {
      Game1.sounds.LogSounds = !Game1.sounds.LogSounds;
      log.Info((Game1.sounds.LogSounds ? "Enabled" : "Disabled") + " sound logging.");
    }

    [OtherNames(new string[] {"poali"})]
    public static void PrintOpenAlInfo(string[] command, IGameLogger log)
    {
      Type oalType = Assembly.GetAssembly(Game1.staminaRect.GetType())?.GetType("Microsoft.Xna.Framework.Audio.OpenALSoundController");
      if ((object) oalType == null)
      {
        log.Error("Could not find type 'OpenALSoundController'");
      }
      else
      {
        FieldInfo destField1;
        FieldInfo destField2;
        FieldInfo destField3;
        if (!TryGetField("_instance", BindingFlags.Static | BindingFlags.NonPublic, out destField1) || !TryGetField("availableSourcesCollection", BindingFlags.Instance | BindingFlags.NonPublic, out destField2) || !TryGetField("inUseSourcesCollection", BindingFlags.Instance | BindingFlags.NonPublic, out destField3))
          return;
        object obj1 = destField1.GetValue((object) null);
        if (obj1 == null)
          log.Error("OpenALSoundController._instance is null");
        else if (obj1.GetType() != oalType)
        {
          log.Error("OpenALSoundController._instance is not an instance of " + oalType.ToString());
        }
        else
        {
          object obj2 = destField2.GetValue(obj1);
          object obj3 = destField3.GetValue(obj1);
          List<int> intList1 = obj2 as List<int>;
          List<int> intList2 = obj3 as List<int>;
          if (intList1 == null)
            log.Error("OpenALSoundController._instance.availableSourcesCollection is not an instance of List<int>");
          else if (intList2 == null)
            log.Error("OpenALSoundController._instance.inUseSourcesCollection is not an instance of List<int>");
          else
            log.Info($"Available: {intList1.Count}\nIn Use: {intList2.Count}");
        }
      }

      bool TryGetField(string fieldName, BindingFlags fieldFlags, out FieldInfo destField)
      {
        destField = oalType.GetField(fieldName, fieldFlags);
        if ((object) destField != null)
          return true;
        log.Error($"OpenALSoundController does not have field '{fieldName}'");
        return false;
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void Crafting(string[] command, IGameLogger log)
    {
      foreach (string key in CraftingRecipe.craftingRecipes.Keys)
        Game1.player.craftingRecipes.TryAdd(key, 0);
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void Cooking(string[] command, IGameLogger log)
    {
      foreach (string key in CraftingRecipe.cookingRecipes.Keys)
        Game1.player.cookingRecipes.TryAdd(key, 0);
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void Experience(string[] command, IGameLogger log)
    {
      string s;
      string error;
      int howMuch;
      if (!ArgUtility.TryGet(command, 1, out s, out error, false, "string skill") | !ArgUtility.TryGetInt(command, 2, out howMuch, out error, "int experiencePoints"))
      {
        DebugCommands.LogArgError(log, command, error);
      }
      else
      {
        switch (s.ToLower())
        {
          case "all":
            Game1.player.gainExperience(0, howMuch);
            Game1.player.gainExperience(1, howMuch);
            Game1.player.gainExperience(3, howMuch);
            Game1.player.gainExperience(2, howMuch);
            Game1.player.gainExperience(4, howMuch);
            break;
          case "farming":
            Game1.player.gainExperience(0, howMuch);
            break;
          case "fishing":
            Game1.player.gainExperience(1, howMuch);
            break;
          case "mining":
            Game1.player.gainExperience(3, howMuch);
            break;
          case "foraging":
            Game1.player.gainExperience(2, howMuch);
            break;
          case "combat":
            Game1.player.gainExperience(4, howMuch);
            break;
          default:
            int result;
            if (int.TryParse(s, out result))
            {
              Game1.player.gainExperience(result, howMuch);
              break;
            }
            DebugCommands.LogArgError(log, command, $"unknown skill ID '{s}'");
            break;
        }
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void ShowExperience(string[] command, IGameLogger log)
    {
      int index;
      string error;
      if (!ArgUtility.TryGetInt(command, 1, out index, out error, "int skillId"))
      {
        DebugCommands.LogArgError(log, command, error);
      }
      else
      {
        int experiencePoint = Game1.player.experiencePoints[index];
        log.Info(experiencePoint.ToString());
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void Profession(string[] command, IGameLogger log)
    {
      int num;
      string error;
      if (!ArgUtility.TryGetInt(command, 1, out num, out error, "int professionId"))
        DebugCommands.LogArgError(log, command, error);
      else
        Game1.player.professions.Add(num);
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void ClearFishCaught(string[] command, IGameLogger log)
    {
      Game1.player.fishCaught.Clear();
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    [OtherNames(new string[] {"caughtFish"})]
    public static void FishCaught(string[] command, IGameLogger log)
    {
      int num;
      string error;
      if (!ArgUtility.TryGetInt(command, 1, out num, out error, "int count"))
        DebugCommands.LogArgError(log, command, error);
      else
        Game1.stats.FishCaught = (uint) num;
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    [OtherNames(new string[] {"r"})]
    public static void ResetForPlayerEntry(string[] command, IGameLogger log)
    {
      Game1.currentLocation.cleanupBeforePlayerExit();
      Game1.currentLocation.resetForPlayerEntry();
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void Fish(string[] command, IGameLogger log)
    {
      string whichFish;
      string error;
      if (!ArgUtility.TryGet(command, 1, out whichFish, out error, false, "string fishId"))
        DebugCommands.LogArgError(log, command, error);
      else if (Game1.player.CurrentTool is FishingRod currentTool)
      {
        List<string> qualifiedItemIds = currentTool.GetTackleQualifiedItemIDs();
        Game1.activeClickableMenu = (IClickableMenu) new BobberBar(whichFish, 0.5f, true, qualifiedItemIds, (string) null, false);
      }
      else
        log.Error("The player must have a fishing rod equipped to use this command.");
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void GrowAnimals(string[] command, IGameLogger log)
    {
      foreach (FarmAnimal farmAnimal in Game1.currentLocation.animals.Values)
        farmAnimal.growFully();
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void PauseAnimals(string[] command, IGameLogger log)
    {
      foreach (FarmAnimal farmAnimal in Game1.currentLocation.Animals.Values)
        farmAnimal.pauseTimer = int.MaxValue;
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void UnpauseAnimals(string[] command, IGameLogger log)
    {
      foreach (FarmAnimal farmAnimal in Game1.currentLocation.Animals.Values)
        farmAnimal.pauseTimer = 0;
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    [OtherNames(new string[] {"removetf"})]
    public static void RemoveTerrainFeatures(string[] command, IGameLogger log)
    {
      Game1.currentLocation.terrainFeatures.Clear();
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void MushroomTrees(string[] command, IGameLogger log)
    {
      foreach (TerrainFeature terrainFeature in Game1.currentLocation.terrainFeatures.Values)
      {
        if (terrainFeature is Tree tree)
          tree.treeType.Value = "7";
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void TrashCan(string[] command, IGameLogger log)
    {
      int num;
      string error;
      if (!ArgUtility.TryGetInt(command, 1, out num, out error, "int trashCanLevel"))
        DebugCommands.LogArgError(log, command, error);
      else
        Game1.player.trashCanLevel = num;
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void FruitTrees(string[] command, IGameLogger log)
    {
      foreach (KeyValuePair<Vector2, TerrainFeature> pair in Game1.currentLocation.terrainFeatures.Pairs)
      {
        if (pair.Value is FruitTree fruitTree)
        {
          fruitTree.daysUntilMature.Value -= 27;
          fruitTree.dayUpdate();
        }
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void Train(string[] command, IGameLogger log)
    {
      Game1.RequireLocation<Railroad>("Railroad").setTrainComing(7500);
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void DebrisWeather(string[] command, IGameLogger log)
    {
      string locationContextId = Game1.player.currentLocation.GetLocationContextId();
      LocationWeather weatherForLocation = Game1.netWorldState.Value.GetWeatherForLocation(locationContextId);
      weatherForLocation.IsDebrisWeather = !weatherForLocation.IsDebrisWeather;
      if (locationContextId == "Default")
        Game1.isDebrisWeather = weatherForLocation.isDebrisWeather.Value;
      Game1.debrisWeather.Clear();
      if (!weatherForLocation.IsDebrisWeather)
        return;
      Game1.populateDebrisWeatherArray();
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void Speed(string[] command, IGameLogger log)
    {
      int num1;
      string error;
      int num2;
      if (!ArgUtility.TryGetInt(command, 1, out num1, out error, "int speed") || !ArgUtility.TryGetOptionalInt(command, 2, out num2, out error, 30, "int minutes"))
      {
        DebugCommands.LogArgError(log, command, error);
      }
      else
      {
        BuffEffects effects = new BuffEffects();
        effects.Speed.Value = (float) num1;
        Game1.player.applyBuff(new Buff("debug_speed", "Debug Speed", "Debug Speed", num2 * Game1.realMilliSecondsPerGameMinute, iconSheetIndex: 0, effects: effects));
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void DayUpdate(string[] command, IGameLogger log)
    {
      int num;
      string error;
      if (!ArgUtility.TryGetInt(command, 1, out num, out error, "int days"))
      {
        DebugCommands.LogArgError(log, command, error);
      }
      else
      {
        for (int index = 0; index < num; ++index)
          Game1.currentLocation.DayUpdate(Game1.dayOfMonth);
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void FarmerDayUpdate(string[] command, IGameLogger log)
    {
      int num;
      string error;
      if (!ArgUtility.TryGetInt(command, 1, out num, out error, "int days"))
      {
        DebugCommands.LogArgError(log, command, error);
      }
      else
      {
        for (int index = 0; index < num; ++index)
          Game1.player.dayupdate(Game1.timeOfDay);
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void MuseumLoot(string[] command, IGameLogger log)
    {
      foreach (ParsedItemData parsedItemData in ItemRegistry.GetObjectTypeDefinition().GetAllData())
      {
        string itemId = parsedItemData.ItemId;
        string objectType = parsedItemData.ObjectType;
        if ((objectType == "Arch" || objectType == "Minerals") && !Game1.player.mineralsFound.ContainsKey(itemId) && !Game1.player.archaeologyFound.ContainsKey(itemId))
        {
          if (objectType == "Arch")
            Game1.player.foundArtifact(itemId, 1);
          else
            Game1.player.addItemToInventoryBool((Item) new Object(itemId, 1));
        }
        if (Game1.player.freeSpotsInInventory() == 0)
          break;
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void NewMuseumLoot(string[] command, IGameLogger log)
    {
      foreach (ParsedItemData parsedItemData in ItemRegistry.GetObjectTypeDefinition().GetAllData())
      {
        string qualifiedItemId = parsedItemData.QualifiedItemId;
        if (LibraryMuseum.IsItemSuitableForDonation(qualifiedItemId) && !LibraryMuseum.HasDonatedArtifact(qualifiedItemId))
          Game1.player.addItemToInventoryBool(ItemRegistry.Create(qualifiedItemId));
        if (Game1.player.freeSpotsInInventory() == 0)
          break;
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void CreateDebris(string[] command, IGameLogger log)
    {
      string id;
      string error;
      if (!ArgUtility.TryGet(command, 1, out id, out error, false, "string itemId"))
        DebugCommands.LogArgError(log, command, error);
      else
        Game1.createObjectDebris(id, Game1.player.TilePoint.X, Game1.player.TilePoint.Y);
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void RemoveDebris(string[] command, IGameLogger log)
    {
      Game1.currentLocation.debris.Clear();
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void RemoveDirt(string[] command, IGameLogger log)
    {
      Game1.currentLocation.terrainFeatures.RemoveWhere((Func<KeyValuePair<Vector2, TerrainFeature>, bool>) (pair => pair.Value is HoeDirt));
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void DyeAll(string[] command, IGameLogger log)
    {
      Game1.activeClickableMenu = (IClickableMenu) new CharacterCustomization(CharacterCustomization.Source.DyePots);
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void DyeShirt(string[] command, IGameLogger log)
    {
      Game1.activeClickableMenu = (IClickableMenu) new CharacterCustomization(Game1.player.shirtItem.Value);
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void DyePants(string[] command, IGameLogger log)
    {
      Game1.activeClickableMenu = (IClickableMenu) new CharacterCustomization(Game1.player.pantsItem.Value);
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    [OtherNames(new string[] {"cmenu", "customize"})]
    public static void CustomizeMenu(string[] command, IGameLogger log)
    {
      Game1.activeClickableMenu = (IClickableMenu) new CharacterCustomization(CharacterCustomization.Source.NewGame);
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void CopyOutfit(string[] command, IGameLogger log)
    {
      StringBuilder stringBuilder1 = new StringBuilder();
      stringBuilder1.Append("<Item><OutfitParts>");
      if (Game1.player.hat.Value != null)
        stringBuilder1.Append($"<Item><ItemId>{Game1.player.hat.Value.QualifiedItemId}</ItemId></Item>");
      Color color;
      if (Game1.player.pantsItem.Value != null)
      {
        StringBuilder stringBuilder2 = stringBuilder1;
        string[] strArray = new string[9];
        strArray[0] = "<Item><ItemId>";
        strArray[1] = Game1.player.pantsItem.Value.QualifiedItemId;
        strArray[2] = "</ItemId><Color>";
        color = Game1.player.pantsItem.Value.clothesColor.Value;
        strArray[3] = color.R.ToString();
        strArray[4] = " ";
        color = Game1.player.pantsItem.Value.clothesColor.Value;
        strArray[5] = color.G.ToString();
        strArray[6] = " ";
        color = Game1.player.pantsItem.Value.clothesColor.Value;
        strArray[7] = color.B.ToString();
        strArray[8] = "</Color></Item>";
        string str = string.Concat(strArray);
        stringBuilder2.Append(str);
      }
      if (Game1.player.shirtItem.Value != null)
      {
        StringBuilder stringBuilder3 = stringBuilder1;
        string[] strArray = new string[9];
        strArray[0] = "<Item><ItemId>";
        strArray[1] = Game1.player.shirtItem.Value.QualifiedItemId;
        strArray[2] = "</ItemId><Color>";
        color = Game1.player.shirtItem.Value.clothesColor.Value;
        strArray[3] = color.R.ToString();
        strArray[4] = " ";
        color = Game1.player.shirtItem.Value.clothesColor.Value;
        strArray[5] = color.G.ToString();
        strArray[6] = " ";
        color = Game1.player.shirtItem.Value.clothesColor.Value;
        strArray[7] = color.B.ToString();
        strArray[8] = "</Color></Item>";
        string str = string.Concat(strArray);
        stringBuilder3.Append(str);
      }
      stringBuilder1.Append("</OutfitParts></Item>");
      string text = stringBuilder1.ToString();
      DesktopClipboard.SetText(text);
      Game1.debugOutput = text;
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void SkinColor(string[] command, IGameLogger log)
    {
      int which;
      string error;
      if (!ArgUtility.TryGetInt(command, 1, out which, out error, "int skinColor"))
        DebugCommands.LogArgError(log, command, error);
      else
        Game1.player.changeSkinColor(which);
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void Hat(string[] command, IGameLogger log)
    {
      int newHat;
      string error;
      if (!ArgUtility.TryGetInt(command, 1, out newHat, out error, "int hatId"))
      {
        DebugCommands.LogArgError(log, command, error);
      }
      else
      {
        Game1.player.changeHat(newHat);
        Game1.playSound("coin");
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void Pants(string[] command, IGameLogger log)
    {
      int r;
      string error;
      int g;
      int b;
      if (!ArgUtility.TryGetInt(command, 1, out r, out error, "int red") || !ArgUtility.TryGetInt(command, 2, out g, out error, "int green") || !ArgUtility.TryGetInt(command, 3, out b, out error, "int blue"))
        DebugCommands.LogArgError(log, command, error);
      else
        Game1.player.changePantsColor(new Color(r, g, b));
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void HairStyle(string[] command, IGameLogger log)
    {
      int whichHair;
      string error;
      if (!ArgUtility.TryGetInt(command, 1, out whichHair, out error, "int hairStyle"))
        DebugCommands.LogArgError(log, command, error);
      else
        Game1.player.changeHairStyle(whichHair);
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void HairColor(string[] command, IGameLogger log)
    {
      int r;
      string error;
      int g;
      int b;
      if (!ArgUtility.TryGetInt(command, 1, out r, out error, "int red") || !ArgUtility.TryGetInt(command, 2, out g, out error, "int green") || !ArgUtility.TryGetInt(command, 3, out b, out error, "int blue"))
        DebugCommands.LogArgError(log, command, error);
      else
        Game1.player.changeHairColor(new Color(r, g, b));
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void Shirt(string[] command, IGameLogger log)
    {
      string itemId;
      string error;
      if (!ArgUtility.TryGet(command, 1, out itemId, out error, false, "string shirtId"))
        DebugCommands.LogArgError(log, command, error);
      else
        Game1.player.changeShirt(itemId);
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    [OtherNames(new string[] {"m", "mv"})]
    public static void MusicVolume(string[] command, IGameLogger log)
    {
      float num;
      string error;
      if (!ArgUtility.TryGetFloat(command, 1, out num, out error, "float volume"))
      {
        DebugCommands.LogArgError(log, command, error);
      }
      else
      {
        Game1.musicPlayerVolume = num;
        Game1.options.musicVolumeLevel = num;
        Game1.musicCategory.SetVolume(Game1.options.musicVolumeLevel);
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void RemoveObjects(string[] command, IGameLogger log)
    {
      Game1.currentLocation.objects.Clear();
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void ListLights(string[] command, IGameLogger log)
    {
      StringBuilder stringBuilder1 = new StringBuilder();
      StringBuilder stringBuilder2 = stringBuilder1;
      StringBuilder stringBuilder3 = stringBuilder2;
      StringBuilder.AppendInterpolatedStringHandler interpolatedStringHandler1 = new StringBuilder.AppendInterpolatedStringHandler(69, 6, stringBuilder2);
      interpolatedStringHandler1.AppendLiteral("The viewport covers tiles (");
      interpolatedStringHandler1.AppendFormatted<int>(Game1.viewport.X / 64 /*0x40*/);
      interpolatedStringHandler1.AppendLiteral(", ");
      interpolatedStringHandler1.AppendFormatted<int>(Game1.viewport.Y / 64 /*0x40*/);
      interpolatedStringHandler1.AppendLiteral(") through (");
      interpolatedStringHandler1.AppendFormatted<int>(Game1.viewport.MaxCorner.X / 64 /*0x40*/);
      interpolatedStringHandler1.AppendLiteral(", ");
      interpolatedStringHandler1.AppendFormatted<int>(Game1.viewport.MaxCorner.Y / 64 /*0x40*/);
      interpolatedStringHandler1.AppendLiteral("), with the player at (");
      interpolatedStringHandler1.AppendFormatted<int>(Game1.player.TilePoint.X);
      interpolatedStringHandler1.AppendLiteral(", ");
      interpolatedStringHandler1.AppendFormatted<int>(Game1.player.TilePoint.Y);
      interpolatedStringHandler1.AppendLiteral(").");
      ref StringBuilder.AppendInterpolatedStringHandler local1 = ref interpolatedStringHandler1;
      stringBuilder3.AppendLine(ref local1);
      stringBuilder1.AppendLine();
      if (Game1.currentLightSources.Count > 0)
      {
        foreach (IGrouping<bool, KeyValuePair<string, LightSource>> source in (IEnumerable<IGrouping<bool, KeyValuePair<string, LightSource>>>) Game1.currentLightSources.ToLookup<KeyValuePair<string, LightSource>, bool>((Func<KeyValuePair<string, LightSource>, bool>) (light => light.Value.IsOnScreen())).OrderByDescending<IGrouping<bool, KeyValuePair<string, LightSource>>, bool>((Func<IGrouping<bool, KeyValuePair<string, LightSource>>, bool>) (p => p.Key)))
        {
          bool key = source.Key;
          KeyValuePair<string, LightSource>[] array = source.ToArray<KeyValuePair<string, LightSource>>();
          if (array.Length != 0)
          {
            StringBuilder stringBuilder4 = stringBuilder1;
            StringBuilder stringBuilder5 = stringBuilder4;
            StringBuilder.AppendInterpolatedStringHandler interpolatedStringHandler2 = new StringBuilder.AppendInterpolatedStringHandler(8, 1, stringBuilder4);
            interpolatedStringHandler2.AppendLiteral("Lights ");
            interpolatedStringHandler2.AppendFormatted(key ? "in view" : "out of view");
            interpolatedStringHandler2.AppendLiteral(":");
            ref StringBuilder.AppendInterpolatedStringHandler local2 = ref interpolatedStringHandler2;
            stringBuilder5.AppendLine(ref local2);
            int num = 1;
            foreach (KeyValuePair<string, LightSource> keyValuePair in array)
            {
              LightSource lightSource = keyValuePair.Value;
              Vector2 vector2 = new Vector2(lightSource.position.X / 64f, lightSource.position.Y / 64f);
              StringBuilder stringBuilder6 = stringBuilder1;
              StringBuilder stringBuilder7 = stringBuilder6;
              interpolatedStringHandler2 = new StringBuilder.AppendInterpolatedStringHandler(32 /*0x20*/, 5, stringBuilder6);
              interpolatedStringHandler2.AppendLiteral("  ");
              interpolatedStringHandler2.AppendFormatted<int>(num++);
              interpolatedStringHandler2.AppendLiteral(". '");
              interpolatedStringHandler2.AppendFormatted(lightSource.Id);
              interpolatedStringHandler2.AppendLiteral("' at tile (");
              interpolatedStringHandler2.AppendFormatted<float>(vector2.X);
              interpolatedStringHandler2.AppendLiteral(", ");
              interpolatedStringHandler2.AppendFormatted<float>(vector2.Y);
              interpolatedStringHandler2.AppendLiteral(") with radius ");
              interpolatedStringHandler2.AppendFormatted<float>(lightSource.radius.Value);
              ref StringBuilder.AppendInterpolatedStringHandler local3 = ref interpolatedStringHandler2;
              stringBuilder7.Append(ref local3);
              if (lightSource.onlyLocation.Value != null && lightSource.onlyLocation.Value != Game1.currentLocation?.NameOrUniqueName)
              {
                StringBuilder stringBuilder8 = stringBuilder1;
                StringBuilder stringBuilder9 = stringBuilder8;
                interpolatedStringHandler2 = new StringBuilder.AppendInterpolatedStringHandler(28, 1, stringBuilder8);
                interpolatedStringHandler2.AppendLiteral(" [only shown in location '");
                interpolatedStringHandler2.AppendFormatted(lightSource.onlyLocation.Value);
                interpolatedStringHandler2.AppendLiteral("']");
                ref StringBuilder.AppendInterpolatedStringHandler local4 = ref interpolatedStringHandler2;
                stringBuilder9.Append(ref local4);
              }
              if (lightSource.Id != keyValuePair.Key)
              {
                StringBuilder stringBuilder10 = stringBuilder1;
                StringBuilder stringBuilder11 = stringBuilder10;
                interpolatedStringHandler2 = new StringBuilder.AppendInterpolatedStringHandler(74, 2, stringBuilder10);
                interpolatedStringHandler2.AppendLiteral(" [WARNING: ID mismatch between dictionary lookup (");
                interpolatedStringHandler2.AppendFormatted(keyValuePair.Key);
                interpolatedStringHandler2.AppendLiteral(") and light instance (");
                interpolatedStringHandler2.AppendFormatted(lightSource.Id);
                interpolatedStringHandler2.AppendLiteral(")]");
                ref StringBuilder.AppendInterpolatedStringHandler local5 = ref interpolatedStringHandler2;
                stringBuilder11.Append(ref local5);
              }
              stringBuilder1.AppendLine(".");
            }
            stringBuilder1.AppendLine();
          }
        }
      }
      else
        stringBuilder1.AppendLine("There are no current light sources.");
      log.Info(stringBuilder1.ToString().TrimEnd());
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void RemoveLights(string[] command, IGameLogger log)
    {
      Game1.currentLightSources.Clear();
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    [OtherNames(new string[] {"i"})]
    public static void Item(string[] command, IGameLogger log)
    {
      string itemId;
      string error;
      int amount;
      int quality;
      if (!ArgUtility.TryGet(command, 1, out itemId, out error, false, "string itemId") || !ArgUtility.TryGetOptionalInt(command, 2, out amount, out error, 1, "int count") || !ArgUtility.TryGetOptionalInt(command, 3, out quality, out error, name: "int quality"))
      {
        DebugCommands.LogArgError(log, command, error);
      }
      else
      {
        Item obj = ItemRegistry.Create(itemId, amount, quality);
        Game1.playSound("coin");
        Game1.player.addItemToInventoryBool(obj);
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    [OtherNames(new string[] {"iq"})]
    public static void ItemQuery(string[] command, IGameLogger log)
    {
      string query;
      string error;
      if (!ArgUtility.TryGetRemainder(command, 1, out query, out error, name: "string query"))
      {
        DebugCommands.LogArgError(log, command, error);
      }
      else
      {
        ItemQueryResult[] itemQueryResultArray = ItemQueryResolver.TryResolve(query, (ItemQueryContext) null, logError: (Action<string, string>) ((_, queryError) => log.Error("Failed parsing that query: " + queryError)));
        if (itemQueryResultArray.Length == 0)
        {
          log.Info("That query did not match any items.");
        }
        else
        {
          ShopMenu shopMenu = new ShopMenu("DebugItemQuery", new Dictionary<ISalable, ItemStockInformation>());
          foreach (ItemQueryResult itemQueryResult in itemQueryResultArray)
            shopMenu.AddForSale(itemQueryResult.Item, new ItemStockInformation(0, int.MaxValue));
          Game1.activeClickableMenu = (IClickableMenu) shopMenu;
        }
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    [OtherNames(new string[] {"gq"})]
    public static void GameQuery(string[] command, IGameLogger log)
    {
      string queryString;
      string error;
      if (!ArgUtility.TryGetRemainder(command, 1, out queryString, out error, name: "string query"))
      {
        DebugCommands.LogArgError(log, command, error);
      }
      else
      {
        \u003C\u003Ef__AnonymousType1<string, bool>[] array = ((IEnumerable<string>) GameStateQuery.SplitRaw(queryString)).Select(rawQuery => new
        {
          Query = rawQuery,
          Result = GameStateQuery.CheckConditions(rawQuery)
        }).ToArray();
        int totalWidth = Math.Max("Query".Length, array.Max(p => p.Query.Length));
        StringBuilder stringBuilder = new StringBuilder().AppendLine().Append("   ").Append("Query".PadRight(totalWidth, ' ')).AppendLine(" | Result").Append("   ").Append("".PadRight(totalWidth, '-')).AppendLine(" | ------");
        bool flag = true;
        foreach (var data in array)
        {
          flag = flag && data.Result;
          stringBuilder.Append("   ").Append(data.Query.PadRight(totalWidth, ' ')).Append(" | ").AppendLine(data.Result.ToString().ToLower());
        }
        stringBuilder.AppendLine().Append("Overall result: ").Append(flag.ToString().ToLower()).AppendLine(".");
        log.Info(stringBuilder.ToString());
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void Tokens(string[] command, IGameLogger log)
    {
      string text1;
      string error;
      if (!ArgUtility.TryGetRemainder(command, 1, out text1, out error, name: "string input"))
      {
        DebugCommands.LogArgError(log, command, error);
      }
      else
      {
        string text2 = TokenParser.ParseText(text1);
        log.Info($"Result: \"{text2}\".");
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void DyeMenu(string[] command, IGameLogger log)
    {
      Game1.activeClickableMenu = (IClickableMenu) new DyeMenu();
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void Tailor(string[] command, IGameLogger log)
    {
      Game1.activeClickableMenu = (IClickableMenu) new TailoringMenu();
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void Forge(string[] command, IGameLogger log)
    {
      Game1.activeClickableMenu = (IClickableMenu) new ForgeMenu();
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void ListTags(string[] command, IGameLogger log)
    {
      if (Game1.player.CurrentItem == null)
        return;
      string str = $"Tags on {Game1.player.CurrentItem.DisplayName}: ";
      foreach (string contextTag in Game1.player.CurrentItem.GetContextTags())
        str = $"{str}{contextTag} ";
      log.Info(str.Trim());
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void QualifiedId(string[] command, IGameLogger log)
    {
      if (Game1.player.CurrentItem == null)
        return;
      string str = $"Qualified ID of {Game1.player.CurrentItem.DisplayName}: {Game1.player.CurrentItem.QualifiedItemId}";
      log.Info(str.Trim());
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void Dye(string[] command, IGameLogger log)
    {
      string str1;
      string error;
      string str2;
      float strength;
      if (!ArgUtility.TryGet(command, 1, out str1, out error, false, "string slot") || !ArgUtility.TryGet(command, 2, out str2, out error, false, "string color") || !ArgUtility.TryGetOptionalFloat(command, 3, out strength, out error, 1f, "float dyeStrength"))
      {
        DebugCommands.LogArgError(log, command, error);
      }
      else
      {
        Color color = Color.White;
        switch (str2.ToLower().Trim())
        {
          case "black":
            color = Color.Black;
            break;
          case "red":
            color = new Color(220, 0, 0);
            break;
          case "blue":
            color = new Color(0, 100, 220);
            break;
          case "yellow":
            color = new Color((int) byte.MaxValue, 230, 0);
            break;
          case "white":
            color = Color.White;
            break;
          case "green":
            color = new Color(10, 143, 0);
            break;
        }
        switch (str1.ToLower().Trim())
        {
          case "shirt":
            Game1.player.shirtItem.Value?.Dye(color, strength);
            break;
          case "pants":
            Game1.player.pantsItem.Value?.Dye(color, strength);
            break;
        }
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void GetIndex(string[] command, IGameLogger log)
    {
      string query;
      string error;
      if (!ArgUtility.TryGet(command, 1, out query, out error, false, "string itemName"))
      {
        DebugCommands.LogArgError(log, command, error);
      }
      else
      {
        Item obj = Utility.fuzzyItemSearch(query);
        if (obj != null)
          log.Info($"{obj.DisplayName}'s qualified ID is {obj.QualifiedItemId}");
        else
          log.Error("No item found with name " + query);
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    [OtherNames(new string[] {"f", "fin"})]
    public static void FuzzyItemNamed(string[] command, IGameLogger log)
    {
      string query;
      string error;
      int stack_count;
      int num;
      if (!ArgUtility.TryGet(command, 1, out query, out error, name: "string itemId") || !ArgUtility.TryGetOptionalInt(command, 2, out stack_count, out error, name: "int count") || !ArgUtility.TryGetOptionalInt(command, 3, out num, out error, name: "int quality"))
      {
        DebugCommands.LogArgError(log, command, error);
      }
      else
      {
        Item obj = Utility.fuzzyItemSearch(query, stack_count);
        if (obj == null)
        {
          log.Error($"No item found with name '{query}'");
        }
        else
        {
          obj.quality.Value = num;
          MeleeWeapon.attemptAddRandomInnateEnchantment(obj, (Random) null);
          Game1.player.addItemToInventory(obj);
          Game1.playSound("coin");
          log.Info($"Added {obj.DisplayName} ({obj.QualifiedItemId})");
        }
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    [OtherNames(new string[] {"in"})]
    public static void ItemNamed(string[] command, IGameLogger log)
    {
      string str;
      string error;
      int amount;
      int quality;
      if (!ArgUtility.TryGet(command, 1, out str, out error, false, "string itemName") || !ArgUtility.TryGetOptionalInt(command, 2, out amount, out error, 1, "int count") || !ArgUtility.TryGetOptionalInt(command, 3, out quality, out error, name: "int quality"))
      {
        DebugCommands.LogArgError(log, command, error);
      }
      else
      {
        foreach (ParsedItemData parsedItemData in ItemRegistry.GetObjectTypeDefinition().GetAllData())
        {
          if (parsedItemData.InternalName.EqualsIgnoreCase(str))
          {
            Game1.player.addItemToInventory(ItemRegistry.Create("(O)" + parsedItemData.ItemId, amount, quality));
            Game1.playSound("coin");
          }
        }
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void Achievement(string[] command, IGameLogger log)
    {
      int which;
      string error;
      if (!ArgUtility.TryGetInt(command, 1, out which, out error, "int achievementId"))
        DebugCommands.LogArgError(log, command, error);
      else
        Game1.getAchievement(which);
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void Heal(string[] command, IGameLogger log)
    {
      Game1.player.health = Game1.player.maxHealth;
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void Die(string[] command, IGameLogger log) => Game1.player.health = 0;

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void Energize(string[] command, IGameLogger log)
    {
      int num;
      string error;
      if (!ArgUtility.TryGetOptionalInt(command, 1, out num, out error, Game1.player.MaxStamina, "int stamina"))
        DebugCommands.LogArgError(log, command, error);
      else
        Game1.player.Stamina = (float) num;
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void Exhaust(string[] command, IGameLogger log) => Game1.player.Stamina = -15f;

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void Warp(string[] command, IGameLogger log)
    {
      string query;
      string error;
      int x;
      int y;
      if (!ArgUtility.TryGet(command, 1, out query, out error, false, "string locationName") || !ArgUtility.TryGetOptionalInt(command, 2, out x, out error, -1, "int tileX") || !ArgUtility.TryGetOptionalInt(command, 3, out y, out error, -1, "int tileY"))
        DebugCommands.LogArgError(log, command, error);
      else if (x > -1 && y <= -1)
      {
        DebugCommands.LogArgError(log, command, "must specify both X and Y positions, or neither");
      }
      else
      {
        GameLocation location = Utility.fuzzyLocationSearch(query);
        if (location == null)
        {
          log.Error("No location with name " + query);
        }
        else
        {
          if (x < 0)
          {
            x = 0;
            y = 0;
            Utility.getDefaultWarpLocation(location.Name, ref x, ref y);
          }
          Game1.warpFarmer(new LocationRequest(location.NameOrUniqueName, location.uniqueName.Value != null, location), x, y, 2);
          log.Info($"Warping Game1.player to {location.NameOrUniqueName} at {x}, {y}");
        }
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    [OtherNames(new string[] {"wh"})]
    public static void WarpHome(string[] command, IGameLogger log) => Game1.warpHome();

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void Money(string[] command, IGameLogger log)
    {
      int num;
      string error;
      if (!ArgUtility.TryGetInt(command, 1, out num, out error, "int amount"))
        DebugCommands.LogArgError(log, command, error);
      else
        Game1.player.Money = num;
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void CatchAllFish(string[] command, IGameLogger log)
    {
      foreach (ParsedItemData parsedItemData in ItemRegistry.GetObjectTypeDefinition().GetAllData())
      {
        if (parsedItemData.ObjectType == "Fish")
          Game1.player.caughtFish(parsedItemData.ItemId, 9);
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void ActivateCalicoStatue(string[] command, IGameLogger log)
    {
      Game1.mine.calicoStatueSpot.Value = new Point(8, 8);
      Game1.mine.calicoStatueActivated(new NetPoint(new Point(8, 8)), Point.Zero, new Point(8, 8));
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void Perfection(string[] command, IGameLogger log)
    {
      Game1.game1.parseDebugInput("CompleteCc", log);
      Game1.game1.parseDebugInput("Specials", log);
      Game1.game1.parseDebugInput("FriendAll", log);
      Game1.game1.parseDebugInput("Cooking", log);
      Game1.game1.parseDebugInput("Crafting", log);
      foreach (string key in Game1.player.craftingRecipes.Keys)
        Game1.player.craftingRecipes[key] = 1;
      foreach (ParsedItemData parsedItemData in ItemRegistry.GetObjectTypeDefinition().GetAllData())
      {
        string itemId = parsedItemData.ItemId;
        if (parsedItemData.ObjectType == "Fish")
          Game1.player.fishCaught.Add(parsedItemData.QualifiedItemId, new int[3]);
        if (Object.isPotentialBasicShipped(itemId, parsedItemData.Category, parsedItemData.ObjectType))
          Game1.player.basicShipped.Add(itemId, 1);
        Game1.player.recipesCooked.Add(itemId, 1);
      }
      Game1.game1.parseDebugInput("Walnut 130", log);
      Game1.player.mailReceived.Add("CF_Fair");
      Game1.player.mailReceived.Add("CF_Fish");
      Game1.player.mailReceived.Add("CF_Sewer");
      Game1.player.mailReceived.Add("CF_Mines");
      Game1.player.mailReceived.Add("CF_Spouse");
      Game1.player.mailReceived.Add("CF_Statue");
      Game1.player.mailReceived.Add("museumComplete");
      Game1.player.miningLevel.Value = 10;
      Game1.player.fishingLevel.Value = 10;
      Game1.player.foragingLevel.Value = 10;
      Game1.player.combatLevel.Value = 10;
      Game1.player.farmingLevel.Value = 10;
      Farm farm = Game1.getFarm();
      Building constructed;
      farm.buildStructure("Water Obelisk", new Vector2(0.0f, 0.0f), Game1.player, out constructed, true, true);
      farm.buildStructure("Earth Obelisk", new Vector2(4f, 0.0f), Game1.player, out constructed, true, true);
      farm.buildStructure("Desert Obelisk", new Vector2(8f, 0.0f), Game1.player, out constructed, true, true);
      farm.buildStructure("Island Obelisk", new Vector2(12f, 0.0f), Game1.player, out constructed, true, true);
      farm.buildStructure("Gold Clock", new Vector2(16f, 0.0f), Game1.player, out constructed, true, true);
      foreach (KeyValuePair<string, string> monster in DataLoader.Monsters(Game1.content))
      {
        for (int index = 0; index < 500; ++index)
          Game1.stats.monsterKilled(monster.Key);
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void Walnut(string[] command, IGameLogger log)
    {
      int num;
      string error;
      if (!ArgUtility.TryGetInt(command, 1, out num, out error, "int count"))
      {
        DebugCommands.LogArgError(log, command, error);
      }
      else
      {
        Game1.netWorldState.Value.GoldenWalnuts += num;
        Game1.netWorldState.Value.GoldenWalnutsFound += num;
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void Gem(string[] command, IGameLogger log)
    {
      int num;
      string error;
      if (!ArgUtility.TryGetInt(command, 1, out num, out error, "int count"))
        DebugCommands.LogArgError(log, command, error);
      else
        Game1.player.QiGems += num;
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    [OtherNames(new string[] {"removeNpc"})]
    public static void KillNpc(string[] command, IGameLogger log)
    {
      string error;
      string npcName;
      if (!ArgUtility.TryGet(command, 1, out npcName, out error, false, "string npcName"))
      {
        DebugCommands.LogArgError(log, command, error);
      }
      else
      {
        bool anyFound = false;
        Utility.ForEachLocation((Func<GameLocation, bool>) (location =>
        {
          location.characters.RemoveWhere((Func<NPC, bool>) (npc =>
          {
            if (!(npc.Name == npcName))
              return false;
            log.Info($"Removed {npc.Name} from {location.NameOrUniqueName}");
            anyFound = true;
            return true;
          }));
          return true;
        }));
        if (anyFound)
          return;
        log.Error($"Couldn't find {npcName} in any locations.");
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    /// <remarks>See also <see cref="M:StardewValley.DebugCommands.DefaultHandlers.Dp(System.String[],StardewValley.Logging.IGameLogger)" />.</remarks>
    [OtherNames(new string[] {"dap"})]
    public static void DaysPlayed(string[] command, IGameLogger log)
    {
      Game1.showGlobalMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3332", (object) (int) Game1.stats.DaysPlayed));
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void FriendAll(string[] command, IGameLogger log)
    {
      string error;
      int friendship;
      if (!ArgUtility.TryGetOptionalInt(command, 1, out friendship, out error, 2500, "int friendship"))
      {
        DebugCommands.LogArgError(log, command, error);
      }
      else
      {
        if (Game1.year == 1)
        {
          Game1.AddCharacterIfNecessary("Kent", true);
          Game1.AddCharacterIfNecessary("Leo", true);
        }
        Utility.ForEachVillager((Func<NPC, bool>) (n =>
        {
          if (!n.CanSocialize && n.Name != "Sandy" && n.Name == "Krobus" || n.Name == "Marlon")
            return true;
          if (!Game1.player.friendshipData.ContainsKey(n.Name))
            Game1.player.friendshipData.Add(n.Name, new Friendship());
          Game1.player.changeFriendship(friendship, n);
          return true;
        }));
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    [OtherNames(new string[] {"friend"})]
    public static void Friendship(string[] command, IGameLogger log)
    {
      string query;
      string error;
      int num;
      if (!ArgUtility.TryGet(command, 1, out query, out error, false, "string npcName") || !ArgUtility.TryGetInt(command, 2, out num, out error, "int friendshipPoints"))
      {
        DebugCommands.LogArgError(log, command, error);
      }
      else
      {
        NPC npc = Utility.fuzzyCharacterSearch(query);
        if (npc == null)
        {
          log.Error($"No character found matching '{query}'.");
        }
        else
        {
          Friendship friendship;
          if (!Game1.player.friendshipData.TryGetValue(npc.Name, out friendship))
            Game1.player.friendshipData[npc.Name] = friendship = new Friendship();
          friendship.Points = num;
        }
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void GetStat(string[] command, IGameLogger log)
    {
      string key;
      string error;
      if (!ArgUtility.TryGet(command, 1, out key, out error, false, "string statName"))
      {
        DebugCommands.LogArgError(log, command, error);
      }
      else
      {
        uint num = Game1.stats.Get(key);
        log.Info($"The '{key}' stat is set to {num}.");
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void SetStat(string[] command, IGameLogger log)
    {
      string key;
      string error;
      int num;
      if (!ArgUtility.TryGet(command, 1, out key, out error, false, "string statName") || !ArgUtility.TryGetInt(command, 2, out num, out error, "int newValue"))
      {
        DebugCommands.LogArgError(log, command, error);
      }
      else
      {
        Game1.stats.Set(key, num);
        log.Info($"Set '{key}' stat to {Game1.stats.Get(key)}.");
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    [OtherNames(new string[] {"eventSeen"})]
    public static void SeenEvent(string[] command, IGameLogger log)
    {
      string str;
      string error;
      bool add;
      if (!ArgUtility.TryGet(command, 1, out str, out error, false, "string eventId") || !ArgUtility.TryGetOptionalBool(command, 2, out add, out error, true, "bool seen"))
      {
        DebugCommands.LogArgError(log, command, error);
      }
      else
      {
        Game1.player.eventsSeen.Toggle<string>(str, add);
        if (add)
          return;
        Game1.eventsSeenSinceLastLocationChange.Remove(str);
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void SeenMail(string[] command, IGameLogger log)
    {
      string str;
      string error;
      bool add;
      if (!ArgUtility.TryGet(command, 1, out str, out error, false, "string mailId") || !ArgUtility.TryGetOptionalBool(command, 2, out add, out error, true, "bool seen"))
        DebugCommands.LogArgError(log, command, error);
      else
        Game1.player.mailReceived.Toggle<string>(str, add);
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void CookingRecipe(string[] command, IGameLogger log)
    {
      string str;
      string error;
      if (!ArgUtility.TryGetRemainder(command, 1, out str, out error, name: "string recipeName"))
        DebugCommands.LogArgError(log, command, error);
      else
        Game1.player.cookingRecipes.Add(str.Trim(), 0);
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    [OtherNames(new string[] {"craftingRecipe"})]
    public static void AddCraftingRecipe(string[] command, IGameLogger log)
    {
      string str;
      string error;
      if (!ArgUtility.TryGetRemainder(command, 1, out str, out error, name: "string recipeName"))
        DebugCommands.LogArgError(log, command, error);
      else
        Game1.player.craftingRecipes.Add(str.Trim(), 0);
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void UpgradeHouse(string[] command, IGameLogger log)
    {
      Game1.player.HouseUpgradeLevel = Math.Min(3, Game1.player.HouseUpgradeLevel + 1);
      Game1.addNewFarmBuildingMaps();
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void StopRafting(string[] command, IGameLogger log)
    {
      Game1.player.isRafting = false;
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void Time(string[] command, IGameLogger log)
    {
      int num;
      string error;
      if (!ArgUtility.TryGetInt(command, 1, out num, out error, "int time"))
      {
        DebugCommands.LogArgError(log, command, error);
      }
      else
      {
        Game1.timeOfDay = num;
        Game1.outdoorLight = Color.White;
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void AddMinute(string[] command, IGameLogger log) => Game1.addMinute();

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void AddHour(string[] command, IGameLogger log) => Game1.addHour();

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void Water(string[] command, IGameLogger log)
    {
      Game1.currentLocation?.ForEachDirt((Func<HoeDirt, bool>) (dirt =>
      {
        if (dirt.Pot != null)
          dirt.Pot.Water();
        else
          dirt.state.Value = 1;
        return true;
      }));
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void GrowCrops(string[] command, IGameLogger log)
    {
      string error;
      int days;
      if (!ArgUtility.TryGetInt(command, 1, out days, out error, "int days"))
        DebugCommands.LogArgError(log, command, error);
      else
        Game1.currentLocation?.ForEachDirt((Func<HoeDirt, bool>) (dirt =>
        {
          if (dirt?.crop != null)
          {
            for (int index = 0; index < days; ++index)
            {
              dirt.crop.newDay(1);
              if (dirt.crop == null)
                break;
            }
          }
          return true;
        }));
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    [OtherNames(new string[] {"c", "cm"})]
    public static void CanMove(string[] command, IGameLogger log)
    {
      Game1.player.isEating = false;
      Game1.player.CanMove = true;
      Game1.player.UsingTool = false;
      Game1.player.usingSlingshot = false;
      Game1.player.FarmerSprite.PauseForSingleAnimation = false;
      if (Game1.player.CurrentTool is FishingRod currentTool)
        currentTool.isFishing = false;
      Game1.player.mount?.dismount();
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void Backpack(string[] command, IGameLogger log)
    {
      int val2;
      string error;
      if (!ArgUtility.TryGetInt(command, 1, out val2, out error, "int increaseBy"))
        DebugCommands.LogArgError(log, command, error);
      else
        Game1.player.increaseBackpackSize(Math.Min(36 - Game1.player.Items.Count, val2));
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void Question(string[] command, IGameLogger log)
    {
      string str;
      string error;
      bool add;
      if (!ArgUtility.TryGet(command, 1, out str, out error, false, "string questionId") || !ArgUtility.TryGetOptionalBool(command, 2, out add, out error, true, "bool seen"))
        DebugCommands.LogArgError(log, command, error);
      else
        Game1.player.dialogueQuestionsAnswered.Toggle<string>(str, add);
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void Year(string[] command, IGameLogger log)
    {
      int num;
      string error;
      if (!ArgUtility.TryGetInt(command, 1, out num, out error, "int year"))
        DebugCommands.LogArgError(log, command, error);
      else
        Game1.year = num;
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void Day(string[] command, IGameLogger log)
    {
      int num;
      string error;
      if (!ArgUtility.TryGetInt(command, 1, out num, out error, "int day"))
      {
        DebugCommands.LogArgError(log, command, error);
      }
      else
      {
        Game1.stats.DaysPlayed = (uint) (Game1.seasonIndex * 28 + num + (Game1.year - 1) * 4 * 28);
        Game1.dayOfMonth = num;
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void Season(string[] command, IGameLogger log)
    {
      Season season;
      string error;
      if (!ArgUtility.TryGetEnum<Season>(command, 1, out season, out error, "Season season"))
      {
        DebugCommands.LogArgError(log, command, error);
      }
      else
      {
        Game1.season = season;
        Game1.setGraphicsForSeason();
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    [OtherNames(new string[] {"dialogue"})]
    public static void AddDialogue(string[] command, IGameLogger log)
    {
      string query;
      string error;
      string dialogueText;
      if (!ArgUtility.TryGet(command, 1, out query, out error, false, "string search") || !ArgUtility.TryGetRemainder(command, 2, out dialogueText, out error, name: "string dialogueText"))
      {
        DebugCommands.LogArgError(log, command, error);
      }
      else
      {
        NPC speaker = Utility.fuzzyCharacterSearch(query);
        if (speaker == null)
          log.Error($"No NPC found matching search '{query}'.");
        else
          Game1.DrawDialogue(new Dialogue(speaker, (string) null, dialogueText));
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void Speech(string[] command, IGameLogger log)
    {
      string query;
      string error;
      string dialogueText;
      if (!ArgUtility.TryGet(command, 1, out query, out error, false, "string search") || !ArgUtility.TryGetRemainder(command, 2, out dialogueText, out error, name: "string dialogueText"))
      {
        DebugCommands.LogArgError(log, command, error);
      }
      else
      {
        NPC speaker = Utility.fuzzyCharacterSearch(query);
        if (speaker == null)
          log.Error($"No NPC found matching search '{query}'.");
        else
          Game1.DrawDialogue(new Dialogue(speaker, (string) null, dialogueText));
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void LoadDialogue(string[] command, IGameLogger log)
    {
      string query;
      string error;
      string str;
      if (!ArgUtility.TryGet(command, 1, out query, out error, false, "string npcName") || !ArgUtility.TryGet(command, 2, out str, out error, false, "string translationKey"))
      {
        DebugCommands.LogArgError(log, command, error);
      }
      else
      {
        NPC speaker = Utility.fuzzyCharacterSearch(query);
        string dialogueText = Game1.content.LoadString(str).Replace("{", "<").Replace("}", ">");
        speaker.CurrentDialogue.Push(new Dialogue(speaker, str, dialogueText));
        Game1.drawDialogue(speaker);
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void Wedding(string[] command, IGameLogger log)
    {
      string str;
      string error;
      if (!ArgUtility.TryGet(command, 1, out str, out error, false, "string npcName"))
      {
        DebugCommands.LogArgError(log, command, error);
      }
      else
      {
        Game1.player.spouse = str;
        Game1.weddingsToday.Add(Game1.player.UniqueMultiplayerID);
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void GameMode(string[] command, IGameLogger log)
    {
      int mode;
      string error;
      if (!ArgUtility.TryGetInt(command, 1, out mode, out error, "int gameMode"))
        DebugCommands.LogArgError(log, command, error);
      else
        Game1.setGameMode((byte) mode);
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void Volcano(string[] command, IGameLogger log)
    {
      int level;
      string error;
      if (!ArgUtility.TryGetInt(command, 1, out level, out error, "int level"))
        DebugCommands.LogArgError(log, command, error);
      else
        Game1.warpFarmer(VolcanoDungeon.GetLevelName(level), 0, 1, 2);
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void MineLevel(string[] command, IGameLogger log)
    {
      int whatLevel;
      string error;
      int num1;
      if (!ArgUtility.TryGetInt(command, 1, out whatLevel, out error, "int level") || !ArgUtility.TryGetOptionalInt(command, 2, out num1, out error, -1, "int layout"))
      {
        DebugCommands.LogArgError(log, command, error);
      }
      else
      {
        int? forceLayout = new int?(num1);
        int? nullable = forceLayout;
        int num2 = 0;
        if (nullable.GetValueOrDefault() < num2 & nullable.HasValue)
          forceLayout = new int?();
        Game1.enterMine(whatLevel, forceLayout);
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void MineInfo(string[] command, IGameLogger log)
    {
      log.Info($"MineShaft.lowestLevelReached = {MineShaft.lowestLevelReached}\nplayer.deepestMineLevel = {Game1.player.deepestMineLevel}");
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void Viewport(string[] command, IGameLogger log)
    {
      Point point;
      string error;
      if (!ArgUtility.TryGetPoint(command, 1, out point, out error, "Point tilePosition"))
      {
        DebugCommands.LogArgError(log, command, error);
      }
      else
      {
        Game1.viewport.X = point.X * 64 /*0x40*/;
        Game1.viewport.Y = point.Y * 64 /*0x40*/;
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void MakeInedible(string[] command, IGameLogger log)
    {
      if (Game1.player.ActiveObject == null)
        return;
      Game1.player.ActiveObject.edibility.Value = -300;
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    [OtherNames(new string[] {"watm"})]
    public static void WarpAnimalToMe(string[] command, IGameLogger log)
    {
      string query;
      string error;
      if (!ArgUtility.TryGet(command, 1, out query, out error, false, "string animalName"))
      {
        DebugCommands.LogArgError(log, command, error);
      }
      else
      {
        FarmAnimal farmAnimal = Utility.fuzzyAnimalSearch(query);
        if (farmAnimal == null)
        {
          log.Info("Couldn't find character named " + query);
        }
        else
        {
          log.Info("Warping " + farmAnimal.displayName);
          farmAnimal.currentLocation.Animals.Remove(farmAnimal.myID.Value);
          Game1.currentLocation.Animals.Add(farmAnimal.myID.Value, farmAnimal);
          farmAnimal.Position = Game1.player.Position;
          farmAnimal.controller = (PathFindController) null;
        }
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    [OtherNames(new string[] {"wctm"})]
    public static void WarpCharacterToMe(string[] command, IGameLogger log)
    {
      string query;
      string error;
      if (!ArgUtility.TryGet(command, 1, out query, out error, false, "string npcName"))
      {
        DebugCommands.LogArgError(log, command, error);
      }
      else
      {
        NPC character = Utility.fuzzyCharacterSearch(query, false);
        if (character == null)
        {
          log.Error("Couldn't find character named " + query);
        }
        else
        {
          log.Info("Warping " + character.displayName);
          Game1.warpCharacter(character, Game1.currentLocation.Name, new Vector2((float) Game1.player.TilePoint.X, (float) Game1.player.TilePoint.Y));
          character.controller = (PathFindController) null;
          character.Halt();
        }
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    [OtherNames(new string[] {"wc"})]
    public static void WarpCharacter(string[] command, IGameLogger log)
    {
      string query;
      string error;
      Point position;
      int direction;
      if (!ArgUtility.TryGet(command, 1, out query, out error, false, "string npcName") || !ArgUtility.TryGetPoint(command, 2, out position, out error, "Point tile") || !ArgUtility.TryGetOptionalInt(command, 4, out direction, out error, 2, "int facingDirection"))
      {
        DebugCommands.LogArgError(log, command, error);
      }
      else
      {
        NPC character = Utility.fuzzyCharacterSearch(query, false);
        if (character == null)
        {
          log.Error("Couldn't find character named " + query);
        }
        else
        {
          Game1.warpCharacter(character, Game1.currentLocation.Name, position);
          character.faceDirection(direction);
          character.controller = (PathFindController) null;
          character.Halt();
        }
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    [OtherNames(new string[] {"wtp"})]
    public static void WarpToPlayer(string[] command, IGameLogger log)
    {
      string error;
      string playerName;
      if (!ArgUtility.TryGet(command, 1, out playerName, out error, false, "string playerName"))
      {
        DebugCommands.LogArgError(log, command, error);
      }
      else
      {
        Farmer farmer = Game1.getOnlineFarmers().FirstOrDefault<Farmer>((Func<Farmer, bool>) (other => other.displayName.EqualsIgnoreCase(playerName)));
        if (farmer == null)
          log.Error("Could not find other farmer " + playerName);
        else
          Game1.game1.parseDebugInput($"{"Warp"} {farmer.currentLocation.NameOrUniqueName} {farmer.TilePoint.X} {farmer.TilePoint.Y}", log);
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    [OtherNames(new string[] {"wtc"})]
    public static void WarpToCharacter(string[] command, IGameLogger log)
    {
      string query;
      string error;
      if (!ArgUtility.TryGet(command, 1, out query, out error, false, "string npcName"))
      {
        DebugCommands.LogArgError(log, command, error);
      }
      else
      {
        NPC n = Utility.fuzzyCharacterSearch(query);
        if (n == null)
          log.Error("Could not find valid character " + query);
        else
          Game1.game1.parseDebugInput($"{"Warp"} {Utility.getGameLocationOfCharacter(n).Name} {n.TilePoint.X} {n.TilePoint.Y}", log);
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    [OtherNames(new string[] {"wct"})]
    public static void WarpCharacterTo(string[] command, IGameLogger log)
    {
      string query;
      string error;
      string targetLocationName;
      Point position;
      int direction;
      if (!ArgUtility.TryGet(command, 1, out query, out error, false, "string npcName") || !ArgUtility.TryGet(command, 2, out targetLocationName, out error, false, "string locationName") || !ArgUtility.TryGetPoint(command, 3, out position, out error, "Point tile") || !ArgUtility.TryGetOptionalInt(command, 5, out direction, out error, 2, "int facingDirection"))
      {
        DebugCommands.LogArgError(log, command, error);
      }
      else
      {
        NPC character = Utility.fuzzyCharacterSearch(query);
        if (character == null)
        {
          log.Error("Could not find valid character " + query);
        }
        else
        {
          Game1.warpCharacter(character, targetLocationName, position);
          character.faceDirection(direction);
          character.controller = (PathFindController) null;
          character.Halt();
        }
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    [OtherNames(new string[] {"ws"})]
    public static void WarpShop(string[] command, IGameLogger log)
    {
      string str;
      string error;
      if (!ArgUtility.TryGet(command, 1, out str, out error, false, "string shopKey"))
      {
        DebugCommands.LogArgError(log, command, error);
      }
      else
      {
        string lower = str.ToLower();
        if (lower != null)
        {
          switch (lower.Length)
          {
            case 3:
              switch (lower[0])
              {
                case 'g':
                  if (lower == "gus")
                  {
                    Game1.game1.parseDebugInput("Warp Saloon 10 20", log);
                    Game1.game1.parseDebugInput("WarpCharacterTo Gus Saloon 10 18", log);
                    return;
                  }
                  break;
                case 'p':
                  if (lower == "pam")
                  {
                    Game1.game1.parseDebugInput("Warp BusStop 7 12", log);
                    Game1.game1.parseDebugInput("WarpCharacterTo Pam BusStop 11 10", log);
                    return;
                  }
                  break;
              }
              break;
            case 5:
              switch (lower[0])
              {
                case 'c':
                  if (lower == "clint")
                  {
                    Game1.game1.parseDebugInput("Warp Blacksmith 3 15", log);
                    Game1.game1.parseDebugInput("WarpCharacterTo Clint Blacksmith 3 13", log);
                    return;
                  }
                  break;
                case 'd':
                  if (lower == "dwarf")
                  {
                    Game1.game1.parseDebugInput("Warp Mine 43 7", log);
                    return;
                  }
                  break;
                case 'r':
                  if (lower == "robin")
                  {
                    Game1.game1.parseDebugInput("Warp ScienceHouse 8 20", log);
                    Game1.game1.parseDebugInput("WarpCharacterTo Robin ScienceHouse 8 18", log);
                    return;
                  }
                  break;
                case 's':
                  if (lower == "sandy")
                  {
                    Game1.game1.parseDebugInput("Warp SandyHouse 2 7", log);
                    Game1.game1.parseDebugInput("WarpCharacterTo Sandy SandyHouse 2 5", log);
                    return;
                  }
                  break;
                case 'w':
                  if (lower == "willy")
                  {
                    Game1.game1.parseDebugInput("Warp FishShop 6 6", log);
                    Game1.game1.parseDebugInput("WarpCharacterTo Willy FishShop 6 4", log);
                    return;
                  }
                  break;
              }
              break;
            case 6:
              switch (lower[0])
              {
                case 'k':
                  if (lower == "krobus")
                  {
                    Game1.game1.parseDebugInput("Warp Sewer 31 19", log);
                    return;
                  }
                  break;
                case 'm':
                  if (lower == "marnie")
                  {
                    Game1.game1.parseDebugInput("Warp AnimalShop 12 16", log);
                    Game1.game1.parseDebugInput("WarpCharacterTo Marnie AnimalShop 12 14", log);
                    return;
                  }
                  break;
                case 'p':
                  if (lower == "pierre")
                  {
                    Game1.game1.parseDebugInput("Warp SeedShop 4 19", log);
                    Game1.game1.parseDebugInput("WarpCharacterTo Pierre SeedShop 4 17", log);
                    return;
                  }
                  break;
                case 'w':
                  if (lower == "wizard")
                  {
                    Game1.player.eventsSeen.Add("418172");
                    Game1.player.hasMagicInk = true;
                    Game1.game1.parseDebugInput("Warp WizardHouse 2 14", log);
                    return;
                  }
                  break;
              }
              break;
          }
        }
        log.Error("That npc doesn't have a shop or it isn't handled by this command");
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void FacePlayer(string[] command, IGameLogger log)
    {
      string query;
      string error;
      if (!ArgUtility.TryGet(command, 1, out query, out error, false, "string npcName"))
      {
        DebugCommands.LogArgError(log, command, error);
      }
      else
      {
        NPC npc = Utility.fuzzyCharacterSearch(query);
        if (npc == null)
          log.Error($"Can't find NPC '{query}'.");
        else
          npc.faceTowardFarmer = true;
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void Refuel(string[] command, IGameLogger log)
    {
      if (!(Game1.player.getToolFromName("Lantern") is Lantern toolFromName))
        return;
      toolFromName.fuelLeft = 100;
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void Lantern(string[] command, IGameLogger log)
    {
      Game1.player.Items.Add(ItemRegistry.Create("(T)Lantern"));
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void GrowGrass(string[] command, IGameLogger log)
    {
      int iterations;
      string error;
      if (!ArgUtility.TryGetInt(command, 1, out iterations, out error, "int iterations"))
      {
        DebugCommands.LogArgError(log, command, error);
      }
      else
      {
        Game1.currentLocation.spawnWeeds(false);
        Game1.currentLocation.growWeedGrass(iterations);
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void AddAllCrafting(string[] command, IGameLogger log)
    {
      foreach (string key in CraftingRecipe.craftingRecipes.Keys)
        Game1.player.craftingRecipes.Add(key, 0);
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void Animal(string[] command, IGameLogger log)
    {
      string str;
      string error;
      if (!ArgUtility.TryGetRemainder(command, 1, out str, out error, name: "string animalName"))
        DebugCommands.LogArgError(log, command, error);
      else
        Utility.addAnimalToFarm(new FarmAnimal(str.Trim(), Game1.multiplayer.getNewID(), Game1.player.UniqueMultiplayerID));
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void MoveBuilding(string[] command, IGameLogger log)
    {
      Vector2 tile;
      string error;
      Point point;
      if (!ArgUtility.TryGetVector2(command, 1, out tile, out error, true, "Vector2 fromTile") || !ArgUtility.TryGetPoint(command, 3, out point, out error, "Point toTile"))
      {
        DebugCommands.LogArgError(log, command, error);
      }
      else
      {
        GameLocation currentLocation = Game1.currentLocation;
        if (currentLocation == null)
          return;
        Building buildingAt = currentLocation.getBuildingAt(tile);
        if (buildingAt == null)
          return;
        buildingAt.tileX.Value = point.X;
        buildingAt.tileY.Value = point.Y;
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void Fishing(string[] command, IGameLogger log)
    {
      int num;
      string error;
      if (!ArgUtility.TryGetInt(command, 1, out num, out error, "int level"))
        DebugCommands.LogArgError(log, command, error);
      else
        Game1.player.fishingLevel.Value = num;
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    [OtherNames(new string[] {"fd", "face"})]
    public static void FaceDirection(string[] command, IGameLogger log)
    {
      string query;
      string error;
      int direction;
      if (!ArgUtility.TryGet(command, 1, out query, out error, false, "string targetName") || !ArgUtility.TryGetInt(command, 2, out direction, out error, "int facingDirection"))
        DebugCommands.LogArgError(log, command, error);
      else if (query == "farmer")
      {
        Game1.player.Halt();
        Game1.player.completelyStopAnimatingOrDoingAction();
        Game1.player.faceDirection(direction);
      }
      else
        Utility.fuzzyCharacterSearch(query).faceDirection(direction);
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void Note(string[] command, IGameLogger log)
    {
      int which;
      string error;
      if (!ArgUtility.TryGetInt(command, 1, out which, out error, "int noteId"))
      {
        DebugCommands.LogArgError(log, command, error);
      }
      else
      {
        int[] numArray;
        if (!Game1.player.archaeologyFound.TryGetValue("102", out numArray))
          Game1.player.archaeologyFound["102"] = numArray = new int[2];
        numArray[0] = 18;
        Game1.netWorldState.Value.LostBooksFound = 18;
        Game1.currentLocation.readNote(which);
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void NetHost(string[] command, IGameLogger log)
    {
      Game1.multiplayer.StartServer();
    }

    /// <summary>Connect to a specified IP address.</summary>
    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void NetJoin(string[] command, IGameLogger log)
    {
      string address;
      string error;
      if (!ArgUtility.TryGet(command, 1, out address, out error, false, "string address"))
      {
        DebugCommands.LogArgError(log, command, error);
      }
      else
      {
        FarmhandMenu farmhandMenu = new FarmhandMenu(Game1.multiplayer.InitClient((Client) new LidgrenClient(address)));
        if (Game1.activeClickableMenu is TitleMenu)
          TitleMenu.subMenu = (IClickableMenu) farmhandMenu;
        else
          Game1.ExitToTitle((Action) (() =>
          {
            (Game1.activeClickableMenu as TitleMenu).skipToTitleButtons();
            TitleMenu.subMenu = (IClickableMenu) farmhandMenu;
          }));
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void ToggleNetCompression(string[] command, IGameLogger log)
    {
      if (Program.defaultCompression.GetType() == typeof (NullNetCompression))
        log.Error("This command can only be used on platforms that support compression.");
      else if (Game1.activeClickableMenu is TitleMenu)
        ToggleCompression();
      else
        Game1.ExitToTitle((Action) (() =>
        {
          (Game1.activeClickableMenu as TitleMenu).skipToTitleButtons();
          ToggleCompression();
        }));

      void ToggleCompression()
      {
        bool flag = Program.netCompression.GetType() == typeof (NullNetCompression);
        Program.netCompression = flag ? Program.defaultCompression : (INetCompression) new NullNetCompression();
        log.Info((flag ? "Enabled" : "Disabled") + " net compression.");
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void LevelUp(string[] command, IGameLogger log)
    {
      int skill;
      string error;
      int level;
      if (!ArgUtility.TryGetInt(command, 1, out skill, out error, "int skill") || !ArgUtility.TryGetInt(command, 2, out level, out error, "int level"))
        DebugCommands.LogArgError(log, command, error);
      else
        Game1.activeClickableMenu = (IClickableMenu) new LevelUpMenu(skill, level);
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void Darts(string[] command, IGameLogger log)
    {
      Game1.currentMinigame = (IMinigame) new Darts();
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void MineGame(string[] command, IGameLogger log)
    {
      string str;
      string error;
      if (!ArgUtility.TryGetOptional(command, 1, out str, out error, allowBlank: false, name: "string mode"))
        DebugCommands.LogArgError(log, command, error);
      else
        Game1.currentMinigame = (IMinigame) new MineCart(0, str == "infinite" ? 2 : 3);
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void Crane(string[] command, IGameLogger log)
    {
      Game1.currentMinigame = (IMinigame) new CraneGame();
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    [OtherNames(new string[] {"trlt"})]
    public static void TailorRecipeListTool(string[] command, IGameLogger log)
    {
      Game1.activeClickableMenu = (IClickableMenu) new TailorRecipeListTool();
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    [OtherNames(new string[] {"apt"})]
    public static void AnimationPreviewTool(string[] command, IGameLogger log)
    {
      Game1.activeClickableMenu = (IClickableMenu) new AnimationPreviewTool();
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void CreateDino(string[] command, IGameLogger log)
    {
      Game1.currentLocation.characters.Add((NPC) new DinoMonster(Game1.player.position.Value + new Vector2(100f, 0.0f)));
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    [OtherNames(new string[] {"pta"})]
    public static void PerformTitleAction(string[] command, IGameLogger log)
    {
      string error;
      string titleAction;
      if (!ArgUtility.TryGet(command, 1, out titleAction, out error, false, "string titleAction"))
        DebugCommands.LogArgError(log, command, error);
      else if (Game1.activeClickableMenu is TitleMenu activeClickableMenu3)
        activeClickableMenu3.performButtonAction(titleAction);
      else
        Game1.ExitToTitle((Action) (() =>
        {
          if (!(Game1.activeClickableMenu is TitleMenu activeClickableMenu2))
            return;
          activeClickableMenu2.skipToTitleButtons();
          activeClickableMenu2.performButtonAction(titleAction);
        }));
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void Action(string[] command, IGameLogger log)
    {
      string action;
      string error;
      if (!ArgUtility.TryGetRemainder(command, 1, out action, out error, name: "string action"))
      {
        DebugCommands.LogArgError(log, command, error);
      }
      else
      {
        Exception exception;
        if (TriggerActionManager.TryRunAction(action, out error, out exception))
          log.Info($"Applied action '{action}'.");
        else
          log.Error($"Couldn't apply action '{action}': {error}", exception);
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void BroadcastMail(string[] command, IGameLogger log)
    {
      string mailName;
      string error;
      if (!ArgUtility.TryGetRemainder(command, 1, out mailName, out error, name: "string mailId"))
        DebugCommands.LogArgError(log, command, error);
      else
        Game1.addMailForTomorrow(mailName, sendToEveryone: true);
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void Phone(string[] command, IGameLogger log) => Game1.game1.ShowTelephoneMenu();

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void Renovate(string[] command, IGameLogger log)
    {
      HouseRenovation.ShowRenovationMenu();
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void Crib(string[] command, IGameLogger log)
    {
      int num;
      string error;
      if (!ArgUtility.TryGetInt(command, 1, out num, out error, "int style"))
      {
        DebugCommands.LogArgError(log, command, error);
      }
      else
      {
        if (!(Game1.getLocationFromName(Game1.player.homeLocation.Value) is FarmHouse locationFromName))
          return;
        locationFromName.cribStyle.Value = num;
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void TestNut(string[] command, IGameLogger log)
    {
      Game1.createItemDebris(ItemRegistry.Create("(O)73"), Vector2.Zero, 2);
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void ShuffleBundles(string[] command, IGameLogger log)
    {
      Game1.GenerateBundles(Game1.BundleType.Remixed, false);
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void Split(string[] command, IGameLogger log)
    {
      int player_index;
      string error;
      if (!ArgUtility.TryGetOptionalInt(command, 1, out player_index, out error, -1, "int playerIndex"))
        DebugCommands.LogArgError(log, command, error);
      else if (player_index > -1)
        GameRunner.instance.AddGameInstance((PlayerIndex) player_index);
      else
        Game1.game1.ShowLocalCoopJoinMenu();
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    [OtherNames(new string[] {"bsm"})]
    public static void SkinBuilding(string[] command, IGameLogger log)
    {
      Building buildingAt = Game1.currentLocation?.getBuildingAt(Game1.player.Tile + new Vector2(0.0f, -1f));
      if (buildingAt != null)
      {
        if (buildingAt.CanBeReskinned())
          Game1.activeClickableMenu = (IClickableMenu) new BuildingSkinMenu(buildingAt);
        else
          log.Error($"The '{buildingAt.buildingType.Value}' building in front of the player can't be skinned.");
      }
      else
        log.Error("No building found in front of player.");
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    [OtherNames(new string[] {"bpm"})]
    public static void PaintBuilding(string[] command, IGameLogger log)
    {
      Building buildingAt = Game1.currentLocation?.getBuildingAt(Game1.player.Tile + new Vector2(0.0f, -1f));
      if (buildingAt != null)
      {
        if (buildingAt.CanBePainted())
        {
          Game1.activeClickableMenu = (IClickableMenu) new BuildingPaintMenu(buildingAt);
          return;
        }
        log.Error($"The '{buildingAt.buildingType.Value}' building in front of the player can't be painted. Defaulting to main farmhouse.");
      }
      Building mainFarmHouse = Game1.getFarm().GetMainFarmHouse();
      if (mainFarmHouse == null)
        log.Error("The main farmhouse wasn't found.");
      else if (!mainFarmHouse.CanBePainted())
        log.Error("The main farmhouse can't be painted.");
      else
        Game1.activeClickableMenu = (IClickableMenu) new BuildingPaintMenu(mainFarmHouse);
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    [OtherNames(new string[] {"md"})]
    public static void MineDifficulty(string[] command, IGameLogger log)
    {
      int num;
      string error;
      if (!ArgUtility.TryGetOptionalInt(command, 1, out num, out error, -1, "int difficulty"))
      {
        DebugCommands.LogArgError(log, command, error);
      }
      else
      {
        if (num > -1)
          Game1.netWorldState.Value.MinesDifficulty = num;
        log.Info($"Mine difficulty: {Game1.netWorldState.Value.MinesDifficulty}");
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    [OtherNames(new string[] {"scd"})]
    public static void SkullCaveDifficulty(string[] command, IGameLogger log)
    {
      int num;
      string error;
      if (!ArgUtility.TryGetOptionalInt(command, 1, out num, out error, -1, "int difficulty"))
      {
        DebugCommands.LogArgError(log, command, error);
      }
      else
      {
        if (num > -1)
          Game1.netWorldState.Value.SkullCavesDifficulty = num;
        log.Info($"Skull Cave difficulty: {Game1.netWorldState.Value.SkullCavesDifficulty}");
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    [OtherNames(new string[] {"tls"})]
    public static void ToggleLightingScale(string[] command, IGameLogger log)
    {
      Game1.game1.useUnscaledLighting = !Game1.game1.useUnscaledLighting;
      log.Info($"Toggled Lighting Scale: useUnscaledLighting: {Game1.game1.useUnscaledLighting}");
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void FixWeapons(string[] command, IGameLogger log)
    {
      SaveMigrator_1_5.ResetForges();
      log.Info("Reset forged weapon attributes.");
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    [OtherNames(new string[] {"plsf"})]
    public static void PrintLatestSaveFix(string[] command, IGameLogger log)
    {
      SaveFixes saveFixes = SaveFixes.FixDuplicateMissedMail;
      log.Info($"The latest save fix is '{saveFixes.ToString()}' (ID: {(int) saveFixes})");
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    [OtherNames(new string[] {"pdb"})]
    public static void PrintGemBirds(string[] command, IGameLogger log)
    {
      log.Info($"Gem birds: North {IslandGemBird.GetBirdTypeForLocation("IslandNorth")} South {IslandGemBird.GetBirdTypeForLocation("IslandSouth")} East {IslandGemBird.GetBirdTypeForLocation("IslandEast")} West {IslandGemBird.GetBirdTypeForLocation("IslandWest")}");
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    [OtherNames(new string[] {"ppp"})]
    public static void PrintPlayerPos(string[] command, IGameLogger log)
    {
      log.Info($"Player tile position is {Game1.player.Tile} (World position: {Game1.player.Position})");
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void ShowPlurals(string[] command, IGameLogger log)
    {
      List<string> stringList = new List<string>();
      foreach (ParsedItemData parsedItemData in ItemRegistry.GetObjectTypeDefinition().GetAllData())
        stringList.Add(parsedItemData.InternalName);
      foreach (ParsedItemData parsedItemData in ItemRegistry.RequireTypeDefinition("(BC)").GetAllData())
        stringList.Add(parsedItemData.InternalName);
      stringList.Sort();
      foreach (string word in stringList)
        log.Info(Lexicon.makePlural(word));
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void HoldItem(string[] command, IGameLogger log)
    {
      bool showMessage;
      string error;
      if (!ArgUtility.TryGetOptionalBool(command, 1, out showMessage, out error, name: "bool showMessage"))
        DebugCommands.LogArgError(log, command, error);
      else
        Game1.player.holdUpItemThenMessage(Game1.player.CurrentItem, showMessage);
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    [OtherNames(new string[] {"rm"})]
    public static void RunMacro(string[] command, IGameLogger log)
    {
      string path;
      string error;
      if (!ArgUtility.TryGetOptional(command, 1, out path, out error, "macro.txt", false, "string fileName"))
        DebugCommands.LogArgError(log, command, error);
      else if (Game1.isRunningMacro)
      {
        log.Error("You cannot run a macro from within a macro.");
      }
      else
      {
        Game1.isRunningMacro = true;
        try
        {
          StreamReader streamReader = new StreamReader(path);
          string text_to_send;
          while ((text_to_send = streamReader.ReadLine()) != null)
            Game1.chatBox.textBoxEnter(text_to_send);
          log.Info("Executed macro file " + path);
          streamReader.Close();
        }
        catch (Exception ex)
        {
          log.Error($"Error running macro file {path}.", ex);
        }
        Game1.isRunningMacro = false;
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void InviteMovie(string[] command, IGameLogger log)
    {
      string query;
      string error;
      if (!ArgUtility.TryGet(command, 1, out query, out error, false, "string npcName"))
      {
        DebugCommands.LogArgError(log, command, error);
      }
      else
      {
        NPC invited_npc = Utility.fuzzyCharacterSearch(query);
        if (invited_npc == null)
          log.Error("Invalid NPC");
        else
          MovieTheater.Invite(Game1.player, invited_npc);
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void Monster(string[] command, IGameLogger log)
    {
      string str;
      string error;
      Point point;
      string s;
      if (!ArgUtility.TryGet(command, 1, out str, out error, false, "string typeName") || !ArgUtility.TryGetPoint(command, 2, out point, out error, "Point tile") || !ArgUtility.TryGetOptionalRemainder(command, 4, out s))
      {
        DebugCommands.LogArgError(log, command, error);
      }
      else
      {
        string typeName = "StardewValley.Monsters." + str;
        Type type = Type.GetType(typeName);
        if ((object) type == null)
        {
          log.Error($"There's no monster with type '{typeName}'.");
        }
        else
        {
          Vector2 vector2 = new Vector2((float) (point.X * 64 /*0x40*/), (float) (point.Y * 64 /*0x40*/));
          object[] objArray;
          if (string.IsNullOrWhiteSpace(s))
          {
            objArray = new object[1]{ (object) vector2 };
          }
          else
          {
            int result;
            if (int.TryParse(s, out result))
              objArray = new object[2]
              {
                (object) vector2,
                (object) result
              };
            else
              objArray = new object[2]
              {
                (object) vector2,
                (object) s
              };
          }
          Game1.currentLocation.characters.Add((NPC) (Activator.CreateInstance(type, objArray) as Monster));
        }
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    [OtherNames(new string[] {"shaft"})]
    public static void Ladder(string[] command, IGameLogger log)
    {
      int x;
      string error;
      int y;
      if (!ArgUtility.TryGetOptionalInt(command, 1, out x, out error, Game1.player.TilePoint.X, "int tileX") || !ArgUtility.TryGetOptionalInt(command, 2, out y, out error, Game1.player.TilePoint.Y + 1, "int tileY"))
      {
        DebugCommands.LogArgError(log, command, error);
      }
      else
      {
        bool forceShaft = command[0].EqualsIgnoreCase("shaft");
        Game1.mine.createLadderDown(x, y, forceShaft);
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void NetLog(string[] command, IGameLogger log)
    {
      Game1.multiplayer.logging.IsLogging = !Game1.multiplayer.logging.IsLogging;
      log.Info($"Turned {(Game1.multiplayer.logging.IsLogging ? "on" : "off")} network write logging");
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void NetClear(string[] command, IGameLogger log)
    {
      Game1.multiplayer.logging.Clear();
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void NetDump(string[] command, IGameLogger log)
    {
      log.Info("Wrote log to " + Game1.multiplayer.logging.Dump());
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    [OtherNames(new string[] {"tto"})]
    public static void ToggleTimingOverlay(string[] command, IGameLogger log)
    {
      bool? isMainInstance = Game1.game1?.IsMainInstance;
      if (!isMainInstance.HasValue || !isMainInstance.GetValueOrDefault())
      {
        log.Error("Cannot toggle timing overlay as a splitscreen instance.");
      }
      else
      {
        bool flag = Game1.debugTimings.Toggle();
        log.Info((flag ? "Enabled" : "Disabled") + " in-game timing overlay.");
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void LogBandwidth(string[] command, IGameLogger log)
    {
      if (Game1.IsServer)
      {
        Game1.server.LogBandwidth = !Game1.server.LogBandwidth;
        log.Info($"Turned {(Game1.server.LogBandwidth ? "on" : "off")} server bandwidth logging");
      }
      else if (Game1.IsClient)
      {
        Game1.client.LogBandwidth = !Game1.client.LogBandwidth;
        log.Info($"Turned {(Game1.client.LogBandwidth ? "on" : "off")} client bandwidth logging");
      }
      else
        log.Error("Cannot toggle bandwidth logging in non-multiplayer games");
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void LogWallAndFloorWarnings(string[] command, IGameLogger log)
    {
      DecoratableLocation.LogTroubleshootingInfo = !DecoratableLocation.LogTroubleshootingInfo;
      log.Info((DecoratableLocation.LogTroubleshootingInfo ? "Enabled" : "Disabled") + " wall and floor warning logs.");
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void ChangeWallet(string[] command, IGameLogger log)
    {
      if (!Game1.IsMasterGame)
        return;
      Game1.player.changeWalletTypeTonight.Value = true;
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void SeparateWallets(string[] command, IGameLogger log)
    {
      if (!Game1.IsMasterGame)
        return;
      ManorHouse.SeparateWallets();
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void MergeWallets(string[] command, IGameLogger log)
    {
      if (!Game1.IsMasterGame)
        return;
      ManorHouse.MergeWallets();
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    [OtherNames(new string[] {"nd", "newDay", "s"})]
    public static void Sleep(string[] command, IGameLogger log)
    {
      Game1.player.isInBed.Value = true;
      Game1.player.sleptInTemporaryBed.Value = true;
      Game1.currentLocation.answerDialogueAction("Sleep_Yes", (string[]) null);
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    [OtherNames(new string[] {"gm", "inv"})]
    public static void Invincible(string[] command, IGameLogger log)
    {
      if (Game1.player.temporarilyInvincible)
      {
        Game1.player.temporaryInvincibilityTimer = 0;
        Game1.playSound("bigDeSelect");
      }
      else
      {
        Game1.player.temporarilyInvincible = true;
        Game1.player.temporaryInvincibilityTimer = -1000000000;
        Game1.playSound("bigSelect");
      }
    }

    /// <summary>Toggle whether multiplayer sync fields should run detailed validation to detect possible bugs. See remarks on <see cref="F:Netcode.NetFields.ShouldValidateNetFields" />.</summary>
    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void ValidateNetFields(string[] command, IGameLogger log)
    {
      NetFields.ShouldValidateNetFields = !NetFields.ShouldValidateNetFields;
      log.Info(NetFields.ShouldValidateNetFields ? "Enabled net field validation, which may impact performance. This only affects new net fields created after it's enabled." : "Disabled net field validation.");
    }

    /// <summary>Filter the saves shown in the current load or co-op menu based on a search term.</summary>
    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    [OtherNames(new string[] {"flm"})]
    public static void FilterLoadMenu(string[] command, IGameLogger log)
    {
      string filter;
      string error;
      if (!ArgUtility.TryGetRemainder(command, 1, out filter, out error, name: "string filter"))
      {
        DebugCommands.LogArgError(log, command, error);
      }
      else
      {
        if (Game1.activeClickableMenu is TitleMenu)
        {
          switch (TitleMenu.subMenu)
          {
            case CoopMenu coopMenu:
              TitleMenu.subMenu = (IClickableMenu) new CoopMenu(coopMenu.tooManyFarms, initialTab: coopMenu.currentTab, filter: filter);
              return;
            case LoadGameMenu _:
              TitleMenu.subMenu = (IClickableMenu) new LoadGameMenu(filter);
              return;
          }
        }
        log.Error("The FilterLoadMenu debug command must be run while the list of saved games is open.");
      }
    }

    /// <summary>Toggle the <see cref="F:StardewValley.Menus.MapPage.EnableDebugLines" /> option.</summary>
    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void WorldMapLines(string[] command, IGameLogger log)
    {
      MapPage.WorldMapDebugLineType parsed;
      if (command.Length > 1)
      {
        if (!Utility.TryParseEnum<MapPage.WorldMapDebugLineType>(string.Join(", ", ((IEnumerable<string>) command).Skip<string>(1)), out parsed))
        {
          DebugCommands.LogArgError(log, command, $"unknown type '{string.Join(" ", ((IEnumerable<string>) command).Skip<string>(1))}', expected space-delimited list of {string.Join(", ", Enum.GetNames(typeof (MapPage.WorldMapDebugLineType)))}");
          return;
        }
      }
      else
        parsed = MapPage.EnableDebugLines == MapPage.WorldMapDebugLineType.None ? MapPage.WorldMapDebugLineType.All : MapPage.WorldMapDebugLineType.None;
      MapPage.EnableDebugLines = parsed;
      IGameLogger gameLogger = log;
      string message;
      if (parsed != MapPage.WorldMapDebugLineType.None)
        message = $"World map debug lines enabled for types {parsed}.";
      else
        message = "World map debug lines disabled.";
      gameLogger.Info(message);
    }

    /// <summary>Print info about the player's current position for <c>Data/WorldMaps</c> data.</summary>
    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    /// <remarks>This is derived from <see cref="M:StardewValley.WorldMaps.WorldMapManager.GetPositionData(StardewValley.GameLocation,Microsoft.Xna.Framework.Point)" />.</remarks>
    public static void WorldMapPosition(string[] command, IGameLogger log)
    {
      bool flag;
      string error;
      if (!ArgUtility.TryGetOptionalBool(command, 1, out flag, out error, name: "bool includeLog"))
      {
        DebugCommands.LogArgError(log, command, error);
      }
      else
      {
        GameLocation currentLocation = Game1.currentLocation;
        Point tilePoint = Game1.player.TilePoint;
        LogBuilder log1 = flag ? new LogBuilder(3) : (LogBuilder) null;
        MapAreaPositionWithContext? positionData = WorldMapManager.GetPositionData(currentLocation, tilePoint, log1);
        StringBuilder stringBuilder1 = new StringBuilder();
        if (!positionData.HasValue)
        {
          stringBuilder1.AppendLine("The player's current position didn't match any entry in Data/WorldMaps.");
        }
        else
        {
          MapAreaPositionWithContext positionWithContext = positionData.Value;
          MapAreaPosition data = positionData.Value.Data;
          StringBuilder stringBuilder2 = stringBuilder1;
          StringBuilder stringBuilder3 = stringBuilder2;
          StringBuilder.AppendInterpolatedStringHandler interpolatedStringHandler = new StringBuilder.AppendInterpolatedStringHandler(33, 3, stringBuilder2);
          interpolatedStringHandler.AppendLiteral("The player is currently at ");
          interpolatedStringHandler.AppendFormatted(currentLocation.NameOrUniqueName);
          interpolatedStringHandler.AppendLiteral(" (");
          interpolatedStringHandler.AppendFormatted<int>(tilePoint.X);
          interpolatedStringHandler.AppendLiteral(", ");
          interpolatedStringHandler.AppendFormatted<int>(tilePoint.Y);
          interpolatedStringHandler.AppendLiteral(").");
          ref StringBuilder.AppendInterpolatedStringHandler local1 = ref interpolatedStringHandler;
          stringBuilder3.AppendLine(ref local1);
          if (currentLocation.NameOrUniqueName != positionWithContext.Location.NameOrUniqueName || tilePoint != positionWithContext.Tile)
          {
            StringBuilder stringBuilder4 = stringBuilder1;
            StringBuilder stringBuilder5 = stringBuilder4;
            interpolatedStringHandler = new StringBuilder.AppendInterpolatedStringHandler(31 /*0x1F*/, 3, stringBuilder4);
            interpolatedStringHandler.AppendLiteral("That was translated to '");
            interpolatedStringHandler.AppendFormatted(positionWithContext.Location.NameOrUniqueName);
            interpolatedStringHandler.AppendLiteral("' (");
            interpolatedStringHandler.AppendFormatted<int>(positionWithContext.Tile.X);
            interpolatedStringHandler.AppendLiteral(", ");
            interpolatedStringHandler.AppendFormatted<int>(positionWithContext.Tile.Y);
            interpolatedStringHandler.AppendLiteral(").");
            ref StringBuilder.AppendInterpolatedStringHandler local2 = ref interpolatedStringHandler;
            stringBuilder5.AppendLine(ref local2);
          }
          StringBuilder stringBuilder6 = stringBuilder1;
          StringBuilder stringBuilder7 = stringBuilder6;
          interpolatedStringHandler = new StringBuilder.AppendInterpolatedStringHandler(53, 3, stringBuilder6);
          interpolatedStringHandler.AppendLiteral("This matches region '");
          interpolatedStringHandler.AppendFormatted(data.Region.Id);
          interpolatedStringHandler.AppendLiteral("', area '");
          interpolatedStringHandler.AppendFormatted(data.Area.Id);
          interpolatedStringHandler.AppendLiteral("', and map position '");
          interpolatedStringHandler.AppendFormatted(data.Data.Id);
          interpolatedStringHandler.AppendLiteral("'.");
          ref StringBuilder.AppendInterpolatedStringHandler local3 = ref interpolatedStringHandler;
          stringBuilder7.AppendLine(ref local3);
          StringBuilder stringBuilder8 = stringBuilder1;
          StringBuilder stringBuilder9 = stringBuilder8;
          interpolatedStringHandler = new StringBuilder.AppendInterpolatedStringHandler(79, 3, stringBuilder8);
          interpolatedStringHandler.AppendLiteral("The position's pixel area is ");
          interpolatedStringHandler.AppendFormatted<Microsoft.Xna.Framework.Rectangle>(data.GetPixelArea());
          interpolatedStringHandler.AppendLiteral(", with the player at position ");
          interpolatedStringHandler.AppendFormatted<Vector2>(positionWithContext.GetMapPixelPosition());
          interpolatedStringHandler.AppendLiteral(" (position ratio: ");
          interpolatedStringHandler.AppendFormatted<Vector2?>(positionWithContext.GetPositionRatioIfValid());
          interpolatedStringHandler.AppendLiteral(").");
          ref StringBuilder.AppendInterpolatedStringHandler local4 = ref interpolatedStringHandler;
          stringBuilder9.AppendLine(ref local4);
          StringBuilder stringBuilder10 = stringBuilder1;
          StringBuilder stringBuilder11 = stringBuilder10;
          interpolatedStringHandler = new StringBuilder.AppendInterpolatedStringHandler(14, 1, stringBuilder10);
          interpolatedStringHandler.AppendLiteral("Scroll text: ");
          interpolatedStringHandler.AppendFormatted(positionWithContext.GetScrollText() ?? "none");
          interpolatedStringHandler.AppendLiteral(".");
          ref StringBuilder.AppendInterpolatedStringHandler local5 = ref interpolatedStringHandler;
          stringBuilder11.AppendLine(ref local5);
        }
        stringBuilder1.AppendLine();
        stringBuilder1.AppendLine("Log:");
        if (log1 != null)
          stringBuilder1.Append(log1.Log);
        else
          stringBuilder1.AppendLine("   Run `debug WorldMapPosition true` to show the detailed log.");
        log.Info(stringBuilder1.ToString());
      }
    }

    /// <summary>List debug commands in the game.</summary>
    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void Search(string[] command, IGameLogger log)
    {
      string search;
      string error;
      if (!ArgUtility.TryGetOptional(command, 1, out search, out error, allowBlank: false, name: "string search"))
      {
        DebugCommands.LogArgError(log, command, error);
      }
      else
      {
        List<string> values = DebugCommands.SearchCommandNames(search);
        if (values.Count == 0)
        {
          log.Info($"No debug commands found matching '{search}'.");
        }
        else
        {
          IGameLogger gameLogger = log;
          string str1;
          if (search == null)
            str1 = $"{values.Count} debug commands registered:\n";
          else
            str1 = $"Found {values.Count} debug commands matching search term '{search}':\n";
          string str2 = string.Join("\n  - ", (IEnumerable<string>) values);
          string str3 = search == null ? "\n\nTip: you can search debug commands like 'debug Search searchTermHere'." : "";
          string message = $"{str1}  - {str2}{str3}";
          gameLogger.Info(message);
        }
      }
    }

    /// <summary>Add artifact spots in every available spot in a 9x9 grid around the player.</summary>
    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void ArtifactSpots(string[] command, IGameLogger log)
    {
      GameLocation currentLocation = Game1.player.currentLocation;
      Vector2 tile = Game1.player.Tile;
      if (currentLocation == null)
      {
        log.Info("You must be in a location to use this command.");
      }
      else
      {
        int num = 0;
        foreach (Vector2 surroundingTileLocations in Utility.getSurroundingTileLocationsArray(tile))
        {
          TerrainFeature terrainFeature;
          if (currentLocation.terrainFeatures.TryGetValue(surroundingTileLocations, out terrainFeature) && terrainFeature is HoeDirt hoeDirt && hoeDirt.crop == null)
            currentLocation.terrainFeatures.Remove(surroundingTileLocations);
          if (currentLocation.isTilePassable(surroundingTileLocations) && !currentLocation.IsTileOccupiedBy(surroundingTileLocations, CollisionMask.Buildings | CollisionMask.Flooring | CollisionMask.Furniture | CollisionMask.Objects | CollisionMask.LocationSpecific))
          {
            currentLocation.objects.Add(surroundingTileLocations, ItemRegistry.Create<Object>("(O)590"));
            ++num;
          }
        }
        if (num == 0)
          log.Info("No unoccupied tiles found around the player.");
        else
          log.Info($"Spawned {num} artifact spots around the player.");
      }
    }

    /// <summary>Enable or disable writing messages to the debug log file.</summary>
    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void LogFile(string[] command, IGameLogger log)
    {
      if (Game1.log is DefaultLogger log1)
      {
        Game1.log = (IGameLogger) new DefaultLogger(log1.ShouldWriteToConsole, !log1.ShouldWriteToLogFile);
        log.Info($"{(log1.ShouldWriteToLogFile ? "Disabled" : "Enabled")} the game log file at {Program.GetDebugLogPath()}.");
      }
      else
      {
        bool? nullable = Game1.log?.GetType().FullName?.StartsWith("StardewModdingAPI.");
        if (nullable.HasValue && nullable.GetValueOrDefault())
          log.Error("The debug log can't be enabled when SMAPI is installed. SMAPI already includes log messages in its own log file.");
        else
          log.Error($"The debug log can't be enabled: the game logger has been replaced with unknown implementation '{Game1.log?.GetType()?.FullName}'.");
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.DebugCommandHandlerDelegate" />
    public static void ToggleCheats(string[] command, IGameLogger log)
    {
      Program.enableCheats = !Program.enableCheats;
      log.Info((Program.enableCheats ? "Enabled" : "Disabled") + " in-game cheats.");
    }
  }
}
