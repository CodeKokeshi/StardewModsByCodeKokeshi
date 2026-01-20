// Decompiled with JetBrains decompiler
// Type: StardewValley.Utility
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Netcode;
using StardewValley.Buildings;
using StardewValley.Characters;
using StardewValley.Constants;
using StardewValley.Delegates;
using StardewValley.Enchantments;
using StardewValley.Events;
using StardewValley.Extensions;
using StardewValley.GameData;
using StardewValley.GameData.Buildings;
using StardewValley.GameData.Characters;
using StardewValley.GameData.FarmAnimals;
using StardewValley.GameData.Objects;
using StardewValley.GameData.Shops;
using StardewValley.GameData.Weddings;
using StardewValley.Internal;
using StardewValley.Inventories;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Minigames;
using StardewValley.Monsters;
using StardewValley.Network;
using StardewValley.Network.NetEvents;
using StardewValley.Objects;
using StardewValley.Pathfinding;
using StardewValley.Quests;
using StardewValley.SpecialOrders;
using StardewValley.TerrainFeatures;
using StardewValley.TokenizableStrings;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using xTile.Dimensions;
using xTile.Layers;

#nullable enable
namespace StardewValley;

/// <summary>Provides general utility methods for the game code.</summary>
/// <remarks>See also <see cref="T:StardewValley.ItemRegistry" /> for working with item IDs.</remarks>
public class Utility
{
  public static 
  #nullable disable
  Color[] PRISMATIC_COLORS = new Color[6]
  {
    Color.Red,
    new Color((int) byte.MaxValue, 120, 0),
    new Color((int) byte.MaxValue, 217, 0),
    Color.Lime,
    Color.Cyan,
    Color.Violet
  };
  public static Item recentlyDiscoveredMissingBasicShippedItem;
  public static readonly Vector2[] DirectionsTileVectors = new Vector2[4]
  {
    new Vector2(0.0f, -1f),
    new Vector2(1f, 0.0f),
    new Vector2(0.0f, 1f),
    new Vector2(-1f, 0.0f)
  };
  public static readonly Vector2[] DirectionsTileVectorsWithDiagonals = new Vector2[8]
  {
    new Vector2(0.0f, -1f),
    new Vector2(1f, -1f),
    new Vector2(1f, 0.0f),
    new Vector2(1f, 1f),
    new Vector2(0.0f, 1f),
    new Vector2(-1f, 1f),
    new Vector2(-1f, 0.0f),
    new Vector2(-1f, -1f)
  };
  public static readonly RasterizerState ScissorEnabled = new RasterizerState()
  {
    ScissorTestEnable = true
  };

  public static Microsoft.Xna.Framework.Rectangle controllerMapSourceRect(Microsoft.Xna.Framework.Rectangle xboxSourceRect)
  {
    return xboxSourceRect;
  }

  public static List<Vector2> removeDuplicates(List<Vector2> list)
  {
    for (int index1 = 0; index1 < list.Count; ++index1)
    {
      for (int index2 = list.Count - 1; index2 >= 0; --index2)
      {
        if (index2 != index1 && list[index1].Equals(list[index2]))
          list.RemoveAt(index2);
      }
    }
    return list;
  }

  /// <summary>Get the reasons a horse can't be summoned to the player currently, if any.</summary>
  /// <param name="who">The player requesting a horse.</param>
  public static Utility.HorseWarpRestrictions GetHorseWarpRestrictionsForFarmer(Farmer who)
  {
    Utility.HorseWarpRestrictions restrictionsForFarmer = Utility.HorseWarpRestrictions.None;
    if (who.horseName.Value == null)
      restrictionsForFarmer |= Utility.HorseWarpRestrictions.NoOwnedHorse;
    GameLocation currentLocation = who.currentLocation;
    if (!currentLocation.IsOutdoors)
      restrictionsForFarmer |= Utility.HorseWarpRestrictions.Indoors;
    Point tilePoint = who.TilePoint;
    if (currentLocation.isCollidingPosition(new Microsoft.Xna.Framework.Rectangle(tilePoint.X * 64 /*0x40*/, tilePoint.Y * 64 /*0x40*/, 128 /*0x80*/, 64 /*0x40*/), Game1.viewport, true, 0, false, (Character) who))
      restrictionsForFarmer |= Utility.HorseWarpRestrictions.NoRoom;
    foreach (Farmer onlineFarmer in Game1.getOnlineFarmers())
    {
      if (onlineFarmer.mount != null && onlineFarmer.mount.getOwner() == who)
      {
        restrictionsForFarmer |= Utility.HorseWarpRestrictions.InUse;
        break;
      }
    }
    return restrictionsForFarmer;
  }

  /// <summary>Get the error message to show for a warp issue returned by <see cref="M:StardewValley.Utility.GetHorseWarpRestrictionsForFarmer(StardewValley.Farmer)" />.</summary>
  /// <param name="issue">The current issues preventing a warp, if any.</param>
  /// <returns>Returns the error message to display, or <c>null</c> if none apply.</returns>
  public static string GetHorseWarpErrorMessage(Utility.HorseWarpRestrictions issue)
  {
    if (issue.HasFlag((Enum) Utility.HorseWarpRestrictions.NoOwnedHorse))
      return Game1.content.LoadString("Strings\\StringsFromCSFiles:HorseFlute_NoHorse");
    if (issue.HasFlag((Enum) Utility.HorseWarpRestrictions.Indoors))
      return Game1.content.LoadString("Strings\\StringsFromCSFiles:HorseFlute_InvalidLocation");
    if (issue.HasFlag((Enum) Utility.HorseWarpRestrictions.NoRoom))
      return Game1.content.LoadString("Strings\\StringsFromCSFiles:HorseFlute_NoClearance");
    return issue.HasFlag((Enum) Utility.HorseWarpRestrictions.InUse) ? Game1.content.LoadString("Strings\\StringsFromCSFiles:HorseFlute_InUse") : (string) null;
  }

  public static Microsoft.Xna.Framework.Rectangle ConstrainScissorRectToScreen(
    Microsoft.Xna.Framework.Rectangle scissor_rect)
  {
    if (scissor_rect.Top < 0)
    {
      int num = -scissor_rect.Top;
      scissor_rect.Height -= num;
      scissor_rect.Y += num;
    }
    if (scissor_rect.Bottom > Game1.viewport.Height)
    {
      int num = scissor_rect.Bottom - Game1.viewport.Height;
      scissor_rect.Height -= num;
    }
    if (scissor_rect.Left < 0)
    {
      int num = -scissor_rect.Left;
      scissor_rect.Width -= num;
      scissor_rect.X += num;
    }
    if (scissor_rect.Right > Game1.viewport.Width)
    {
      int num = scissor_rect.Right - Game1.viewport.Width;
      scissor_rect.Width -= num;
    }
    return scissor_rect;
  }

  public static double getRandomDouble(double min, double max, Random random = null)
  {
    if (random == null)
      random = Game1.random;
    double num = max - min;
    return random.NextDouble() * num + min;
  }

  public static Vector2 getRandom360degreeVector(float speed)
  {
    Vector2 vector2 = Vector2.Transform(new Vector2(0.0f, -1f), Matrix.CreateRotationZ((float) Utility.getRandomDouble(0.0, 2.0 * Math.PI)));
    vector2.Normalize();
    return vector2 * speed;
  }

  public static Point Vector2ToPoint(Vector2 v) => new Point((int) v.X, (int) v.Y);

  public static Item getRaccoonSeedForCurrentTimeOfYear(Farmer who, Random r, int stackOverride = -1)
  {
    int num = r.Next(2, 4);
    while (r.NextDouble() < 0.1 + who.team.AverageDailyLuck())
      ++num;
    Item currentTimeOfYear = (Item) null;
    Season season = Game1.season;
    if (Game1.dayOfMonth > (season == Season.Spring ? 23 : 20))
      season = (Season) ((int) (season + 1) % 4);
    switch (season)
    {
      case Season.Spring:
        currentTimeOfYear = ItemRegistry.Create("(O)CarrotSeeds");
        break;
      case Season.Summer:
        currentTimeOfYear = ItemRegistry.Create("(O)SummerSquashSeeds");
        break;
      case Season.Fall:
        currentTimeOfYear = ItemRegistry.Create("(O)BroccoliSeeds");
        break;
      case Season.Winter:
        currentTimeOfYear = ItemRegistry.Create("(O)PowdermelonSeeds");
        break;
    }
    currentTimeOfYear.Stack = stackOverride == -1 ? num : stackOverride;
    return currentTimeOfYear;
  }

  public static Vector2 PointToVector2(Point p) => new Vector2((float) p.X, (float) p.Y);

  public static int getStartTimeOfFestival()
  {
    return Game1.weatherIcon == 1 ? Convert.ToInt32(ArgUtility.SplitBySpaceAndGet(Game1.temporaryContent.Load<Dictionary<string, string>>($"Data\\Festivals\\{Game1.currentSeason}{Game1.dayOfMonth.ToString()}")["conditions"].Split('/')[1], 0)) : -1;
  }

  public static bool doesMasterPlayerHaveMailReceivedButNotMailForTomorrow(string mailID)
  {
    return (Game1.MasterPlayer.mailReceived.Contains(mailID) || Game1.MasterPlayer.mailReceived.Contains(mailID + "%&NL&%")) && !Game1.MasterPlayer.mailForTomorrow.Contains(mailID) && !Game1.MasterPlayer.mailForTomorrow.Contains(mailID + "%&NL&%");
  }

  /// <summary>Get whether there's a festival scheduled for today in any location.</summary>
  /// <remarks>This doesn't match passive festivals like the Night Market; see <see cref="M:StardewValley.Utility.IsPassiveFestivalDay" /> for those.</remarks>
  public static bool isFestivalDay()
  {
    return Utility.isFestivalDay(Game1.dayOfMonth, Game1.season, (string) null);
  }

  /// <summary>Get whether there's a festival scheduled for today in the given location context.</summary>
  /// <param name="locationContext">The location context to check, usually matching a constant like <see cref="F:StardewValley.LocationContexts.DefaultId" />, or <c>null</c> for any context.</param>
  /// <inheritdoc cref="M:StardewValley.Utility.isFestivalDay" path="/remarks" />
  public static bool isFestivalDay(string locationContext)
  {
    return Utility.isFestivalDay(Game1.dayOfMonth, Game1.season, locationContext);
  }

  /// <summary>Get whether there's a festival scheduled on the given day in any location. This doesn't match passive festivals like the Night Market.</summary>
  /// <param name="day">The day of month to check.</param>
  /// <param name="season">The season key to check.</param>
  /// <inheritdoc cref="M:StardewValley.Utility.isFestivalDay" path="/remarks" />
  public static bool isFestivalDay(int day, Season season)
  {
    return Utility.isFestivalDay(day, season, (string) null);
  }

  /// <summary>Get whether there's a festival scheduled on the given day and in the given location context. This doesn't match passive festivals like the Night Market.</summary>
  /// <param name="day">The day of month to check.</param>
  /// <param name="season">The season key to check.</param>
  /// <param name="locationContext">The location context to check, usually matching a constant like <see cref="F:StardewValley.LocationContexts.DefaultId" />, or <c>null</c> for any context.</param>
  /// <inheritdoc cref="M:StardewValley.Utility.isFestivalDay" path="/remarks" />
  public static bool isFestivalDay(int day, Season season, string locationContext)
  {
    string str = $"{Utility.getSeasonKey(season)}{day}";
    if (!DataLoader.Festivals_FestivalDates(Game1.temporaryContent).ContainsKey(str))
      return false;
    if (locationContext != null)
    {
      string locationName;
      if (!Event.tryToLoadFestivalData(str, out string _, out Dictionary<string, string> _, out locationName, out int _, out int _))
        return false;
      GameLocation locationFromName = Game1.getLocationFromName(locationName);
      if (locationFromName == null || locationFromName.GetLocationContextId() != locationContext)
        return false;
    }
    return true;
  }

  /// <summary>Perform an action for each location in the game.</summary>
  /// <param name="action">The action to perform for each location. This should return true (continue iterating) or false (stop).</param>
  /// <param name="includeInteriors">Whether to include instanced building interiors that aren't in <see cref="P:StardewValley.Game1.locations" /> directly.</param>
  /// <param name="includeGenerated">Whether to include temporary generated locations like mine or volcano dungeon levels.</param>
  public static void ForEachLocation(
    Func<GameLocation, bool> action,
    bool includeInteriors = true,
    bool includeGenerated = false)
  {
    GameLocation currentLocation = Game1.currentLocation;
    string nameOrUniqueName = currentLocation?.NameOrUniqueName;
    foreach (GameLocation location in (IEnumerable<GameLocation>) Game1.locations)
    {
      GameLocation gameLocation = !(location.NameOrUniqueName == nameOrUniqueName) || currentLocation == null ? location : currentLocation;
      if (!action(gameLocation))
        return;
      if (includeInteriors)
      {
        bool shouldContinue = true;
        gameLocation.ForEachInstancedInterior((Func<GameLocation, bool>) (interior =>
        {
          if (action(interior))
            return true;
          shouldContinue = false;
          return false;
        }));
        if (!shouldContinue)
          return;
      }
    }
    if (!includeGenerated)
      return;
    foreach (MineShaft activeMine in MineShaft.activeMines)
    {
      GameLocation gameLocation = !(activeMine.NameOrUniqueName == nameOrUniqueName) || currentLocation == null ? (GameLocation) activeMine : currentLocation;
      if (!action(gameLocation))
        return;
    }
    foreach (VolcanoDungeon activeLevel in VolcanoDungeon.activeLevels)
    {
      GameLocation gameLocation = !(activeLevel.NameOrUniqueName == nameOrUniqueName) || currentLocation == null ? (GameLocation) activeLevel : currentLocation;
      if (!action(gameLocation))
        break;
    }
  }

  /// <summary>Perform an action for each building in the game.</summary>
  /// <param name="action">The action to perform for each building. This should return true (continue iterating) or false (stop).</param>
  /// <param name="ignoreUnderConstruction">Whether to ignore buildings which haven't been fully constructed yet.</param>
  public static void ForEachBuilding(Func<Building, bool> action, bool ignoreUnderConstruction = true)
  {
    Utility.ForEachLocation((Func<GameLocation, bool>) (location =>
    {
      foreach (Building building in location.buildings)
      {
        if ((!ignoreUnderConstruction || !building.isUnderConstruction()) && !action(building))
          return false;
      }
      return true;
    }), false);
  }

  public static List<Pet> getAllPets()
  {
    List<Pet> allPets = new List<Pet>();
    foreach (NPC character in Game1.getFarm().characters)
    {
      if (character is Pet pet)
        allPets.Add(pet);
    }
    foreach (Farmer allFarmer in Game1.getAllFarmers())
    {
      foreach (NPC character in Utility.getHomeOfFarmer(allFarmer).characters)
      {
        if (character is Pet pet)
          allPets.Add(pet);
      }
    }
    return allPets;
  }

  /// <summary>Perform an action for each non-playable character in the game (including villagers, horses, pets, monsters, player children, etc).</summary>
  /// <param name="action">The action to perform for each character. This should return true (continue iterating) or false (stop).</param>
  /// <param name="includeEventActors">Whether to match temporary event actors.</param>
  /// <remarks>See also <see cref="M:StardewValley.Utility.ForEachVillager(System.Func{StardewValley.NPC,System.Boolean},System.Boolean)" />.</remarks>
  public static void ForEachCharacter(Func<NPC, bool> action, bool includeEventActors = false)
  {
    Utility.ForEachLocation((Func<GameLocation, bool>) (location =>
    {
      foreach (NPC character in location.characters)
      {
        if ((includeEventActors || !character.EventActor) && !action(character))
          return false;
      }
      return true;
    }), includeGenerated: true);
  }

  /// <summary>Perform an action for each villager NPC in the game.</summary>
  /// <param name="action">The action to perform for each character. This should return true (continue iterating) or false (stop).</param>
  /// <param name="includeEventActors">Whether to match temporary event actors.</param>
  /// <remarks>See also <see cref="M:StardewValley.Utility.ForEachCharacter(System.Func{StardewValley.NPC,System.Boolean},System.Boolean)" />.</remarks>
  public static void ForEachVillager(Func<NPC, bool> action, bool includeEventActors = false)
  {
    Utility.ForEachLocation((Func<GameLocation, bool>) (location =>
    {
      foreach (NPC character in location.characters)
      {
        if ((includeEventActors || !character.EventActor) && character.IsVillager && !action(character))
          return false;
      }
      return true;
    }));
  }

  /// <summary>Perform an action for each building in the game.</summary>
  /// <typeparam name="TBuilding">The expected building type.</typeparam>
  /// <param name="action">The action to perform for each building. This should return true (continue iterating) or false (stop).</param>
  /// <param name="ignoreUnderConstruction">Whether to ignore buildings which haven't been fully constructed yet.</param>
  public static void ForEachBuilding<TBuilding>(
    Func<TBuilding, bool> action,
    bool ignoreUnderConstruction = true)
    where TBuilding : Building
  {
    Utility.ForEachLocation((Func<GameLocation, bool>) (location =>
    {
      foreach (Building building2 in location.buildings)
      {
        if (building2 is TBuilding building3 && (!ignoreUnderConstruction || !building3.isUnderConstruction(true)) && !action(building3))
          return false;
      }
      return true;
    }), false);
  }

  /// <summary>Perform an action for each planted crop in the game.</summary>
  /// <param name="action">The action to perform for each crop. This should return true (continue iterating) or false (stop).</param>
  public static void ForEachCrop(Func<Crop, bool> action)
  {
    Utility.ForEachLocation((Func<GameLocation, bool>) (location =>
    {
      foreach (TerrainFeature terrainFeature in location.terrainFeatures.Values)
      {
        Crop crop = terrainFeature is HoeDirt hoeDirt2 ? hoeDirt2.crop : (Crop) null;
        if (crop != null && !action(crop))
          return false;
      }
      foreach (Object @object in location.objects.Values)
      {
        Crop crop = @object is IndoorPot indoorPot2 ? indoorPot2.hoeDirt.Value?.crop : (Crop) null;
        if (crop != null && !action(crop))
          return false;
      }
      return true;
    }));
  }

  /// <summary>Perform an action for each item in the game world, including items within items (e.g. in a chest or on a table), hats placed on children, items in player inventories, etc.</summary>
  /// <param name="action">The action to perform for each item. This should return true (continue iterating) or false (stop).</param>
  /// <returns>Returns whether to continue iterating if needed (i.e. returns false if the last <paramref name="action" /> call did).</returns>
  /// <remarks>See also <see cref="M:StardewValley.Utility.ForEachItemContext(StardewValley.Delegates.ForEachItemDelegate)" /> for more advanced scenarios like replacing items.</remarks>
  public static bool ForEachItem(Func<Item, bool> action)
  {
    return ForEachItemHelper.ForEachItemInWorld(new ForEachItemDelegate(Handle));

    bool Handle(in StardewValley.Internal.ForEachItemContext context) => action(context.Item);
  }

  /// <summary>Perform an action for each item in the game world, including items within items (e.g. in a chest or on a table), hats placed on children, items in player inventories, etc.</summary>
  /// <param name="handler">The action to perform for each item.</param>
  /// <returns>Returns whether to continue iterating if needed (i.e. returns false if the last <paramref name="handler" /> call did).</returns>
  /// <remarks>See also <see cref="M:StardewValley.Utility.ForEachItem(System.Func{StardewValley.Item,System.Boolean})" /> if you only need to iterate items.</remarks>
  public static bool ForEachItemContext(ForEachItemDelegate handler)
  {
    return ForEachItemHelper.ForEachItemInWorld(handler);
  }

  /// <summary>Perform an action for each item within a location, including items within items (e.g. in a chest or on a table), hats placed on children, items in player inventories, etc.</summary>
  /// <param name="location">The location whose items to iterate.</param>
  /// <param name="action">The action to perform for each item. This should return true (continue iterating) or false (stop).</param>
  /// <returns>Returns whether to continue iterating if needed (i.e. returns false if the last <paramref name="action" /> call did).</returns>
  /// <remarks>See also <see cref="M:StardewValley.Utility.ForEachItemContextIn(StardewValley.GameLocation,StardewValley.Delegates.ForEachItemDelegate)" /> for more advanced scenarios like replacing items.</remarks>
  public static bool ForEachItemIn(GameLocation location, Func<Item, bool> action)
  {
    return ForEachItemHelper.ForEachItemInLocation(location, new ForEachItemDelegate(Handle));

    bool Handle(in StardewValley.Internal.ForEachItemContext context) => action(context.Item);
  }

  /// <summary>Perform an action for each item within a location, including items within items (e.g. in a chest or on a table), hats placed on children, items in player inventories, etc.</summary>
  /// <param name="location">The location whose items to iterate.</param>
  /// <param name="handler">The action to perform for each item.</param>
  /// <returns>Returns whether to continue iterating if needed (i.e. returns false if the last <paramref name="handler" /> call did).</returns>
  /// <remarks>See also <see cref="M:StardewValley.Utility.ForEachItemIn(StardewValley.GameLocation,System.Func{StardewValley.Item,System.Boolean})" /> if you only need to iterate items.</remarks>
  public static bool ForEachItemContextIn(GameLocation location, ForEachItemDelegate handler)
  {
    return ForEachItemHelper.ForEachItemInLocation(location, handler);
  }

  public static int getNumObjectsOfIndexWithinRectangle(
    Microsoft.Xna.Framework.Rectangle r,
    string[] indexes,
    GameLocation location)
  {
    int indexWithinRectangle = 0;
    Vector2 zero = Vector2.Zero;
    for (int y = r.Y; y < r.Bottom + 1; ++y)
    {
      zero.Y = (float) y;
      for (int x = r.X; x < r.Right + 1; ++x)
      {
        zero.X = (float) x;
        Object @object;
        if (location.objects.TryGetValue(zero, out @object))
        {
          for (int index1 = 0; index1 < indexes.Length; ++index1)
          {
            string index2 = indexes[index1];
            if (index2 == null || ItemRegistry.HasItemId((Item) @object, index2))
            {
              ++indexWithinRectangle;
              break;
            }
          }
        }
      }
    }
    return indexWithinRectangle;
  }

  /// <summary>Try to parse a string as a valid enum value.</summary>
  /// <typeparam name="TEnum">The enum type.</typeparam>
  /// <param name="value">The raw value to parse. This is not case-sensitive.</param>
  /// <param name="parsed">The parsed enum value, if valid.</param>
  /// <returns>Returns whether the value was successfully parsed as an enum.</returns>
  public static bool TryParseEnum<TEnum>(string value, out TEnum parsed) where TEnum : struct
  {
    if (Enum.TryParse<TEnum>(value, true, out parsed) && (typeof (TEnum).IsEnumDefined((object) parsed) || typeof (TEnum).GetCustomAttribute<FlagsAttribute>() != null && !long.TryParse(parsed.ToString(), out long _)))
      return true;
    parsed = default (TEnum);
    return false;
  }

  /// <summary>Get an enum value if it's valid, else get a default value.</summary>
  /// <typeparam name="TEnum">The enum type.</typeparam>
  /// <param name="value">The unvalidated enum value.</param>
  /// <param name="defaultValue">The value to return if invalid.</param>
  /// <returns>Returns <paramref name="value" /> if it matches one of the enum constants, else <paramref name="defaultValue" />.</returns>
  public static TEnum GetEnumOrDefault<TEnum>(TEnum value, TEnum defaultValue) where TEnum : struct
  {
    return !typeof (TEnum).IsEnumDefined((object) value) ? defaultValue : value;
  }

  /// <summary>Trim whitespace at the start and end of each line in the given text.</summary>
  /// <param name="text">The text whose lines to trim.</param>
  public static string TrimLines(string text)
  {
    text = text?.Trim();
    if (string.IsNullOrEmpty(text))
      return text;
    string[] strArray = LegacyShims.SplitAndTrim(text, '\n');
    return strArray.Length <= 1 ? text : string.Join("\n", strArray);
  }

  public static bool IsLegacyIdAbove(string itemId, int lowerBound)
  {
    int result;
    return int.TryParse(itemId, out result) && result > lowerBound;
  }

  public static bool IsLegacyIdBetween(string itemId, int lowerBound, int upperBound)
  {
    int result;
    return int.TryParse(itemId, out result) && result >= lowerBound && result <= upperBound;
  }

  /// <summary>Find the best match for a search term based on fuzzy compare rules.</summary>
  /// <param name="query">The fuzzy search query to match.</param>
  /// <param name="terms">The terms from which to choose a match.</param>
  /// <returns>Returns the best match for the query, or <c>null</c> if no match was found.</returns>
  public static string fuzzySearch(string query, ICollection<string> terms)
  {
    int? nullable1 = new int?();
    string str = (string) null;
    foreach (string term in (IEnumerable<string>) terms)
    {
      int? nullable2 = Utility.fuzzyCompare(query, term);
      if (nullable2.HasValue)
      {
        if (nullable1.HasValue)
        {
          int? nullable3 = nullable2;
          int? nullable4 = nullable1;
          if (!(nullable3.GetValueOrDefault() < nullable4.GetValueOrDefault() & nullable3.HasValue & nullable4.HasValue))
            continue;
        }
        nullable1 = nullable2;
        str = term;
      }
    }
    return str;
  }

  /// <summary>Find all matches for a search term based on fuzzy compare rules.</summary>
  /// <param name="query">The fuzzy search query to match.</param>
  /// <param name="terms">The terms from which to choose a match.</param>
  /// <param name="sortByScore">Whether to sort the matching terms by score, in addition to alphabetically.</param>
  /// <returns>Returns all matches for the query, ordered by fuzzy match score and then by name.</returns>
  public static IEnumerable<string> fuzzySearchAll(
    string query,
    ICollection<string> terms,
    bool sortByScore = true)
  {
    return !sortByScore ? (IEnumerable<string>) terms.Where<string>((Func<string, bool>) (term => Utility.fuzzyCompare(query, term).HasValue)).OrderBy<string, string>((Func<string, string>) (term => term.ToLowerInvariant())) : terms.Select(term => new
    {
      term = term,
      score = Utility.fuzzyCompare(query, term)
    }).Where(_param1 => _param1.score.HasValue).OrderBy(_param1 => _param1.score.Value).ThenBy(_param1 => _param1.term.ToLowerInvariant()).Select(_param1 => _param1.term);
  }

  /// <summary>Get whether a term is a fuzzy match for a search query.</summary>
  /// <param name="query">The fuzzy search query to match.</param>
  /// <param name="term">The actual value to compare against the query.</param>
  /// <returns>Returns the numeric match priority (where lower values are a better match), or <c>null</c> if the term doesn't match the query.</returns>
  public static int? fuzzyCompare(string query, string term)
  {
    if (query.Trim() == term.Trim())
      return new int?(0);
    string str1 = FormatForFuzzySearch(query);
    string str2 = FormatForFuzzySearch(term);
    if (str1 == str2)
      return new int?(1);
    if (str2.StartsWith(str1))
      return new int?(2);
    return str2.Contains(str1) ? new int?(3) : new int?();

    static string FormatForFuzzySearch(string value)
    {
      string str1 = value.Trim().ToLowerInvariant().Replace(" ", "");
      string str2 = str1.Replace("(", "").Replace(")", "").Replace("'", "").Replace(".", "").Replace("!", "").Replace("?", "").Replace("-", "");
      return str2.Length != 0 ? str2 : str1;
    }
  }

  public static Item fuzzyItemSearch(string query, int stack_count = 1, bool useLocalizedNames = false)
  {
    Dictionary<string, string> dictionary = new Dictionary<string, string>();
    foreach (IItemDataDefinition itemType in ItemRegistry.ItemTypes)
    {
      foreach (string allId in itemType.GetAllIds())
      {
        ParsedItemData data = itemType.GetData(allId);
        string key = useLocalizedNames ? data.DisplayName : data.InternalName;
        if (!dictionary.ContainsKey(key))
          dictionary[key] = itemType.Identifier + allId;
      }
    }
    ParsedItemData data1 = ItemRegistry.GetData("(O)390");
    if (data1 != null)
    {
      string key = useLocalizedNames ? data1.DisplayName : data1.InternalName;
      dictionary[key] = "(O)390";
    }
    string key1 = Utility.fuzzySearch(query, (ICollection<string>) dictionary.Keys);
    return key1 != null ? ItemRegistry.Create(dictionary[key1], stack_count) : (Item) null;
  }

  public static GameLocation fuzzyLocationSearch(string query)
  {
    Dictionary<string, GameLocation> name_bank = new Dictionary<string, GameLocation>();
    Utility.ForEachLocation((Func<GameLocation, bool>) (location =>
    {
      name_bank[location.NameOrUniqueName] = location;
      return true;
    }));
    string key = Utility.fuzzySearch(query, (ICollection<string>) name_bank.Keys);
    return key == null ? (GameLocation) null : name_bank[key];
  }

  public static string AOrAn(string text)
  {
    if (text != null && text.Length > 0)
    {
      switch (text.ToLowerInvariant()[0])
      {
        case 'a':
        case 'e':
        case 'i':
        case 'o':
        case 'u':
          return LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.hu ? "az" : "an";
      }
    }
    return "a";
  }

  /// <summary>Get the default tile position where the player should be placed when they arrive in a location, if arriving from a warp that didn't specify a tile position.</summary>
  /// <param name="locationName">The <see cref="P:StardewValley.GameLocation.Name" /> value for the target location.</param>
  /// <param name="x">The default X tile position.</param>
  /// <param name="y">The default Y tile position.</param>
  public static void getDefaultWarpLocation(string locationName, ref int x, ref int y)
  {
    GameLocation locationFromName = Game1.getLocationFromName(locationName);
    Point parsed;
    if (locationFromName != null && locationFromName.TryGetMapPropertyAs("DefaultWarpLocation", out parsed))
    {
      x = parsed.X;
      y = parsed.Y;
    }
    else
    {
      if (locationFromName is Farm farm)
      {
        Point mainFarmHouseEntry = farm.GetMainFarmHouseEntry();
        if (mainFarmHouseEntry != Point.Zero)
        {
          x = mainFarmHouseEntry.X;
          y = mainFarmHouseEntry.Y;
        }
      }
      Point? defaultArrivalTile = (Point?) GameLocation.GetData(locationName)?.DefaultArrivalTile;
      if (defaultArrivalTile.HasValue)
      {
        x = defaultArrivalTile.Value.X;
        y = defaultArrivalTile.Value.Y;
      }
      else
      {
        if (locationName != null)
        {
          switch (locationName.Length)
          {
            case 4:
              switch (locationName[0])
              {
                case 'B':
                  if (locationName == "Barn")
                    break;
                  goto label_22;
                case 'C':
                  if (locationName == "Coop")
                    goto label_19;
                  goto label_22;
                case 'F':
                  if (locationName == "Farm")
                  {
                    x = 64 /*0x40*/;
                    y = 15;
                    return;
                  }
                  goto label_22;
                default:
                  goto label_22;
              }
              break;
            case 5:
              switch (locationName[0])
              {
                case 'B':
                  if (locationName == "Barn2" || locationName == "Barn3")
                    break;
                  goto label_22;
                case 'C':
                  if (locationName == "Coop2" || locationName == "Coop3")
                    goto label_19;
                  goto label_22;
                default:
                  goto label_22;
              }
              break;
            case 10:
              if (locationName == "SlimeHutch")
              {
                x = 8;
                y = 18;
                return;
              }
              goto label_22;
            default:
              goto label_22;
          }
          x = 11;
          y = 13;
          return;
label_19:
          x = 2;
          y = 8;
          return;
        }
label_22:
        string propertyValue;
        if (locationFromName == null || !locationFromName.TryGetMapProperty("Warp", out propertyValue))
          return;
        string[] strArray = propertyValue.Split(' ');
        Vector2 tileLocation = new Vector2((float) Convert.ToInt32(strArray[0]), (float) Convert.ToInt32(strArray[1]));
        Vector2 tileForCharacter = Utility.recursiveFindOpenTileForCharacter((Character) Game1.player, Game1.getLocationFromName(locationName), tileLocation, 10, false);
        x = (int) tileForCharacter.X;
        y = (int) tileForCharacter.Y;
      }
    }
  }

  public static FarmAnimal fuzzyAnimalSearch(string query)
  {
    List<FarmAnimal> animals = new List<FarmAnimal>();
    Utility.ForEachLocation((Func<GameLocation, bool>) (location =>
    {
      animals.AddRange((IEnumerable<FarmAnimal>) location.Animals.Values);
      return true;
    }));
    Dictionary<string, FarmAnimal> dictionary = new Dictionary<string, FarmAnimal>();
    foreach (FarmAnimal farmAnimal in animals)
      dictionary[farmAnimal.Name] = farmAnimal;
    string key = Utility.fuzzySearch(query, (ICollection<string>) dictionary.Keys);
    return key == null ? (FarmAnimal) null : dictionary[key];
  }

  public static NPC fuzzyCharacterSearch(string query, bool must_be_villager = true)
  {
    Dictionary<string, NPC> name_bank = new Dictionary<string, NPC>();
    Utility.ForEachCharacter((Func<NPC, bool>) (character =>
    {
      if (!must_be_villager || character.IsVillager)
        name_bank[character.Name] = character;
      return true;
    }));
    string key = Utility.fuzzySearch(query, (ICollection<string>) name_bank.Keys);
    return key == null ? (NPC) null : name_bank[key];
  }

  public static Color GetPrismaticColor(int offset = 0, float speedMultiplier = 1f)
  {
    float num = 1500f;
    int index1 = ((int) (Game1.currentGameTime.TotalGameTime.TotalMilliseconds * (double) speedMultiplier / (double) num) + offset) % Utility.PRISMATIC_COLORS.Length;
    int index2 = (index1 + 1) % Utility.PRISMATIC_COLORS.Length;
    float t = (float) (Game1.currentGameTime.TotalGameTime.TotalMilliseconds * (double) speedMultiplier / (double) num % 1.0);
    return new Color()
    {
      R = (byte) ((double) Utility.Lerp((float) Utility.PRISMATIC_COLORS[index1].R / (float) byte.MaxValue, (float) Utility.PRISMATIC_COLORS[index2].R / (float) byte.MaxValue, t) * (double) byte.MaxValue),
      G = (byte) ((double) Utility.Lerp((float) Utility.PRISMATIC_COLORS[index1].G / (float) byte.MaxValue, (float) Utility.PRISMATIC_COLORS[index2].G / (float) byte.MaxValue, t) * (double) byte.MaxValue),
      B = (byte) ((double) Utility.Lerp((float) Utility.PRISMATIC_COLORS[index1].B / (float) byte.MaxValue, (float) Utility.PRISMATIC_COLORS[index2].B / (float) byte.MaxValue, t) * (double) byte.MaxValue),
      A = (byte) ((double) Utility.Lerp((float) Utility.PRISMATIC_COLORS[index1].A / (float) byte.MaxValue, (float) Utility.PRISMATIC_COLORS[index2].A / (float) byte.MaxValue, t) * (double) byte.MaxValue)
    };
  }

  public static Color Get2PhaseColor(
    Color color1,
    Color color2,
    int offset = 0,
    float speedMultiplier = 1f,
    float timeOffset = 0.0f)
  {
    float num1 = 1500f;
    TimeSpan totalGameTime = Game1.currentGameTime.TotalGameTime;
    int num2 = ((int) ((double) ((float) totalGameTime.TotalMilliseconds + timeOffset) * (double) speedMultiplier / (double) num1) + offset) % 2;
    totalGameTime = Game1.currentGameTime.TotalGameTime;
    float t = (float) ((double) ((float) totalGameTime.TotalMilliseconds + timeOffset) * (double) speedMultiplier / (double) num1 % 1.0);
    Color color3 = new Color();
    Color color4 = num2 == 0 ? color1 : color2;
    Color color5 = num2 == 0 ? color2 : color1;
    color3.R = (byte) ((double) Utility.Lerp((float) color4.R / (float) byte.MaxValue, (float) color5.R / (float) byte.MaxValue, t) * (double) byte.MaxValue);
    color3.G = (byte) ((double) Utility.Lerp((float) color4.G / (float) byte.MaxValue, (float) color5.G / (float) byte.MaxValue, t) * (double) byte.MaxValue);
    color3.B = (byte) ((double) Utility.Lerp((float) color4.B / (float) byte.MaxValue, (float) color5.B / (float) byte.MaxValue, t) * (double) byte.MaxValue);
    color3.A = (byte) ((double) Utility.Lerp((float) color4.A / (float) byte.MaxValue, (float) color5.A / (float) byte.MaxValue, t) * (double) byte.MaxValue);
    return color3;
  }

  public static bool IsNormalObjectAtParentSheetIndex(Item item, string itemId)
  {
    return item.HasTypeObject() && item.GetType() == typeof (Object) && item.ItemId == itemId;
  }

  public static Microsoft.Xna.Framework.Rectangle getSafeArea()
  {
    Microsoft.Xna.Framework.Rectangle titleSafeArea = Game1.game1.GraphicsDevice.Viewport.GetTitleSafeArea();
    if (Game1.game1.GraphicsDevice.GetRenderTargets().Length == 0)
    {
      float num = 1f / Game1.options.zoomLevel;
      if (Game1.uiMode)
        num = 1f / Game1.options.uiScale;
      titleSafeArea.X = (int) ((double) titleSafeArea.X * (double) num);
      titleSafeArea.Y = (int) ((double) titleSafeArea.Y * (double) num);
      titleSafeArea.Width = (int) ((double) titleSafeArea.Width * (double) num);
      titleSafeArea.Height = (int) ((double) titleSafeArea.Height * (double) num);
    }
    return titleSafeArea;
  }

  /// <summary>
  /// Return the adjusted renderPos such that bounds implied by renderSize
  /// is within the TitleSafeArea.
  /// 
  /// If it already is, renderPos is returned unmodified.
  /// </summary>
  public static Vector2 makeSafe(Vector2 renderPos, Vector2 renderSize)
  {
    int x1 = (int) renderPos.X;
    int y1 = (int) renderPos.Y;
    int x2 = (int) renderSize.X;
    int y2 = (int) renderSize.Y;
    Utility.makeSafe(ref x1, ref y1, x2, y2);
    return new Vector2((float) x1, (float) y1);
  }

  public static void makeSafe(ref Vector2 position, int width, int height)
  {
    int x = (int) position.X;
    int y = (int) position.Y;
    Utility.makeSafe(ref x, ref y, width, height);
    position.X = (float) x;
    position.Y = (float) y;
  }

  public static void makeSafe(ref Microsoft.Xna.Framework.Rectangle bounds)
  {
    Utility.makeSafe(ref bounds.X, ref bounds.Y, bounds.Width, bounds.Height);
  }

  public static void makeSafe(ref int x, ref int y, int width, int height)
  {
    Microsoft.Xna.Framework.Rectangle safeArea = Utility.getSafeArea();
    if (x < safeArea.Left)
      x = safeArea.Left;
    if (y < safeArea.Top)
      y = safeArea.Top;
    if (x + width > safeArea.Right)
      x = safeArea.Right - width;
    if (y + height <= safeArea.Bottom)
      return;
    y = safeArea.Bottom - height;
  }

  public static int makeSafeMarginY(int marginy)
  {
    Viewport viewport = Game1.game1.GraphicsDevice.Viewport;
    Microsoft.Xna.Framework.Rectangle safeArea = Utility.getSafeArea();
    int num1 = safeArea.Top - viewport.Bounds.Top;
    if (num1 > marginy)
      marginy = num1;
    int num2 = viewport.Bounds.Bottom - safeArea.Bottom;
    if (num2 > marginy)
      marginy = num2;
    return marginy;
  }

  public static int CompareGameVersions(
    string version,
    string other_version,
    bool ignore_platform_specific = false)
  {
    string[] strArray1 = version.Split('.');
    string[] strArray2 = other_version.Split('.');
    for (int index = 0; index < Math.Max(strArray1.Length, strArray2.Length); ++index)
    {
      float result1 = 0.0f;
      float result2 = 0.0f;
      if (index < strArray1.Length)
        float.TryParse(strArray1[index], out result1);
      if (index < strArray2.Length)
        float.TryParse(strArray2[index], out result2);
      if ((double) result1 != (double) result2 || index == 2 & ignore_platform_specific)
        return result1.CompareTo(result2);
    }
    return 0;
  }

  public static float getFarmerItemsShippedPercent(Farmer who = null)
  {
    if (who == null)
      who = Game1.player;
    Utility.recentlyDiscoveredMissingBasicShippedItem = (Item) null;
    int num1 = 0;
    int num2 = 0;
    foreach (ParsedItemData parsedItemData in ItemRegistry.GetObjectTypeDefinition().GetAllData())
    {
      switch (parsedItemData.Category)
      {
        case -7:
        case -2:
          continue;
        default:
          if (Object.isPotentialBasicShipped(parsedItemData.ItemId, parsedItemData.Category, parsedItemData.ObjectType))
          {
            ++num2;
            if (who.basicShipped.ContainsKey(parsedItemData.ItemId))
            {
              ++num1;
              continue;
            }
            if (Utility.recentlyDiscoveredMissingBasicShippedItem == null)
            {
              Utility.recentlyDiscoveredMissingBasicShippedItem = ItemRegistry.Create(parsedItemData.QualifiedItemId);
              continue;
            }
            continue;
          }
          continue;
      }
    }
    return (float) num1 / (float) num2;
  }

  public static bool hasFarmerShippedAllItems()
  {
    return (double) Utility.getFarmerItemsShippedPercent() >= 1.0;
  }

  public static NPC getTodaysBirthdayNPC()
  {
    NPC match = (NPC) null;
    Utility.ForEachVillager((Func<NPC, bool>) (n =>
    {
      if (n.isBirthday())
        match = n;
      return match == null;
    }));
    return match;
  }

  /// <summary>Create a <see cref="T:System.Random" /> instance using the save ID and days played as a seed.</summary>
  /// <param name="seedA">The first extra value to add to the RNG seed, if any.</param>
  /// <param name="seedB">The second extra value to add to the RNG seed, if any.</param>
  /// <param name="seedC">The third extra value to add to the RNG seed, if any.</param>
  public static Random CreateDaySaveRandom(double seedA = 0.0, double seedB = 0.0, double seedC = 0.0)
  {
    return Utility.CreateRandom((double) Game1.stats.DaysPlayed, (double) (Game1.uniqueIDForThisGame / 2UL), seedA, seedB, seedC);
  }

  /// <summary>Get an RNG seeded with the same value when called within the specified period.</summary>
  /// <param name="interval">The time interval within which the random seed should be consistent.</param>
  /// <param name="key">A key which identifies the random instance being created, if any. Instances with a different key will have a different seed.</param>
  /// <param name="random">The created RNG, if valid.</param>
  /// <param name="error">An error indicating why the RNG could not be created, if applicable.</param>
  /// <returns>Returns whether the interval is valid and the RNG was created.</returns>
  public static bool TryCreateIntervalRandom(
    string interval,
    string key,
    out Random random,
    out string error)
  {
    int deterministicHashCode = key != null ? Game1.hash.GetDeterministicHashCode(key) : 0;
    error = (string) null;
    double seedC;
    switch (interval.ToLower())
    {
      case "tick":
        seedC = (double) Game1.ticks;
        break;
      case "day":
        seedC = (double) Game1.stats.DaysPlayed;
        break;
      case "season":
        seedC = (double) Game1.hash.GetDeterministicHashCode(Game1.currentSeason + Game1.year.ToString());
        break;
      case "year":
        seedC = (double) Game1.hash.GetDeterministicHashCode("year" + Game1.year.ToString());
        break;
      default:
        error = $"invalid interval '{interval}'; expected one of 'tick', 'day', 'season', or 'year'";
        random = (Random) null;
        return false;
    }
    random = Utility.CreateRandom((double) deterministicHashCode, (double) Game1.uniqueIDForThisGame, seedC);
    return true;
  }

  /// <summary>Create a <see cref="T:System.Random" /> instance which safely combines the given seed values.</summary>
  /// <param name="seedA">The first seed value to combine.</param>
  /// <param name="seedB">The second seed value to combine.</param>
  /// <param name="seedC">The third seed value to combine.</param>
  /// <param name="seedD">The fourth seed value to combine.</param>
  /// <param name="seedE">The fifth seed value to combine.</param>
  public static Random CreateRandom(
    double seedA,
    double seedB = 0.0,
    double seedC = 0.0,
    double seedD = 0.0,
    double seedE = 0.0)
  {
    return new Random(Utility.CreateRandomSeed(seedA, seedB, seedC, seedD, seedE));
  }

  /// <summary>Safely combine seed values for use as a <see cref="T:System.Random" /> seed.</summary>
  /// <param name="seedA">The first seed value to combine.</param>
  /// <param name="seedB">The second seed value to combine.</param>
  /// <param name="seedC">The third seed value to combine.</param>
  /// <param name="seedD">The fourth seed value to combine.</param>
  /// <param name="seedE">The fifth seed value to combine.</param>
  public static int CreateRandomSeed(
    double seedA,
    double seedB,
    double seedC = 0.0,
    double seedD = 0.0,
    double seedE = 0.0)
  {
    if (Game1.UseLegacyRandom)
      return (int) ((seedA % (double) int.MaxValue + seedB % (double) int.MaxValue + seedC % (double) int.MaxValue + seedD % (double) int.MaxValue + seedE % (double) int.MaxValue) % (double) int.MaxValue);
    return Game1.hash.GetDeterministicHashCode((int) (seedA % (double) int.MaxValue), (int) (seedB % (double) int.MaxValue), (int) (seedC % (double) int.MaxValue), (int) (seedD % (double) int.MaxValue), (int) (seedE % (double) int.MaxValue));
  }

  /// <summary>Get a random entry from a dictionary.</summary>
  /// <typeparam name="TKey">The dictionary key type.</typeparam>
  /// <typeparam name="TValue">The dictionary value type.</typeparam>
  /// <param name="dictionary">The list whose entries to get.</param>
  /// <param name="key">The random entry's key, if found.</param>
  /// <param name="value">The random entry's value, if found.</param>
  /// <param name="random">The RNG to use, or <c>null</c> for <see cref="F:StardewValley.Game1.random" />.</param>
  /// <returns>Returns whether an entry was found.</returns>
  public static bool TryGetRandom<TKey, TValue>(
    IDictionary<TKey, TValue> dictionary,
    out TKey key,
    out TValue value,
    Random random = null)
  {
    if (dictionary == null || dictionary.Count == 0)
    {
      key = default (TKey);
      value = default (TValue);
      return false;
    }
    if (random == null)
      random = Game1.random;
    KeyValuePair<TKey, TValue> keyValuePair = dictionary.ElementAt<KeyValuePair<TKey, TValue>>(random.Next(dictionary.Count));
    key = keyValuePair.Key;
    value = keyValuePair.Value;
    return true;
  }

  /// <inheritdoc cref="M:StardewValley.Utility.TryGetRandom``2(System.Collections.Generic.IDictionary{``0,``1},``0@,``1@,System.Random)" />
  public static bool TryGetRandom<TKey, TValue, TField, TSerialDict, TSelf>(
    NetDictionary<TKey, TValue, TField, TSerialDict, TSelf> dictionary,
    out TKey key,
    out TValue value,
    Random random = null)
    where TField : class, INetObject<INetSerializable>, new()
    where TSerialDict : IDictionary<TKey, TValue>, new()
    where TSelf : NetDictionary<TKey, TValue, TField, TSerialDict, TSelf>
  {
    if (dictionary == null || dictionary.Length == 0)
    {
      key = default (TKey);
      value = default (TValue);
      return false;
    }
    if (random == null)
      random = Game1.random;
    KeyValuePair<TKey, TValue> keyValuePair = dictionary.Pairs.ElementAt(random.Next(dictionary.Length));
    key = keyValuePair.Key;
    value = keyValuePair.Value;
    return true;
  }

  /// <inheritdoc cref="M:StardewValley.Utility.TryGetRandom``2(System.Collections.Generic.IDictionary{``0,``1},``0@,``1@,System.Random)" />
  public static bool TryGetRandom(
    OverlaidDictionary dictionary,
    out Vector2 key,
    out Object value,
    Random random = null)
  {
    if (dictionary == null || dictionary.Length == 0)
    {
      key = Vector2.Zero;
      value = (Object) null;
      return false;
    }
    if (random == null)
      random = Game1.random;
    KeyValuePair<Vector2, Object> keyValuePair = dictionary.Pairs.ElementAt<KeyValuePair<Vector2, Object>>(random.Next(dictionary.Length));
    key = keyValuePair.Key;
    value = keyValuePair.Value;
    return true;
  }

  /// <summary>Get a random entry from a list, ignoring specific values.</summary>
  /// <typeparam name="T">The list item type.</typeparam>
  /// <param name="list">The values to choose from.</param>
  /// <param name="except">The values to ignore in the <paramref name="list" />.</param>
  /// <param name="random">The random number generator to use.</param>
  /// <param name="selected">The selected value.</param>
  /// <returns>Returns whether a value was selected.</returns>
  public static bool TryGetRandomExcept<T>(
    IList<T> list,
    ISet<T> except,
    Random random,
    out T selected)
  {
    if (list == null || list.Count == 0)
    {
      selected = default (T);
      return false;
    }
    if (except == null || except.Count == 0)
    {
      selected = random.ChooseFrom<T>(list);
      return true;
    }
    T[] array = list.Except<T>((IEnumerable<T>) except).ToArray<T>();
    selected = random.ChooseFrom<T>((IList<T>) array);
    return true;
  }

  public static string getRandomSingleTileFurniture(Random r)
  {
    switch (r.Next(3))
    {
      case 0:
        return "(F)" + (r.Next(10) * 3).ToString();
      case 1:
        return "(F)" + r.Next(1376, 1391).ToString();
      default:
        return "(F)" + (r.Next(6) * 2 + 1391).ToString();
    }
  }

  public static void improveFriendshipWithEveryoneInRegion(Farmer who, int amount, string region)
  {
    Utility.ForEachLocation((Func<GameLocation, bool>) (l =>
    {
      foreach (NPC character in l.characters)
      {
        if (character.GetData()?.HomeRegion == region && who.friendshipData.ContainsKey(character.Name))
          who.changeFriendship(amount, character);
      }
      return true;
    }));
  }

  /// <summary>Get a random Winter Star gift which an NPC can give to players.</summary>
  /// <param name="who">The NPC giving the gift.</param>
  public static Item getGiftFromNPC(NPC who)
  {
    Random random = Utility.CreateRandom((double) (Game1.uniqueIDForThisGame / 2UL), (double) Game1.year, (double) Game1.dayOfMonth, (double) Game1.seasonIndex, (double) who.TilePoint.X);
    List<Item> options = new List<Item>();
    CharacterData data = who.GetData();
    List<GenericSpawnItemDataWithCondition> winterStarGifts = data.WinterStarGifts;
    // ISSUE: explicit non-virtual call
    if ((winterStarGifts != null ? (__nonvirtual (winterStarGifts.Count) > 0 ? 1 : 0) : 0) != 0)
    {
      ItemQueryContext context = new ItemQueryContext(Game1.currentLocation, Game1.player, random, $"character '{who.Name}' > winter star gifts");
      foreach (GenericSpawnItemDataWithCondition winterStarGift in data.WinterStarGifts)
      {
        GenericSpawnItemDataWithCondition entry = winterStarGift;
        if (GameStateQuery.CheckConditions(entry.Condition, random: random))
        {
          Item obj = ItemQueryResolver.TryResolveRandomItem((ISpawnItemData) entry, context, logError: (Action<string, string>) ((query, error) => Game1.log.Error($"{who.Name} failed parsing item query '{query}' for winter star gift entry '{entry.Id}': {error}")));
          if (obj != null)
            options.Add(obj);
        }
      }
    }
    if (options.Count == 0)
    {
      if (who.Age == 2)
        options.AddRange((IEnumerable<Item>) new Item[4]
        {
          ItemRegistry.Create("(O)330"),
          ItemRegistry.Create("(O)103"),
          ItemRegistry.Create("(O)394"),
          ItemRegistry.Create("(O)" + random.Next(535, 538).ToString())
        });
      else
        options.AddRange((IEnumerable<Item>) new Item[14]
        {
          ItemRegistry.Create("(O)608"),
          ItemRegistry.Create("(O)651"),
          ItemRegistry.Create("(O)611"),
          ItemRegistry.Create("(O)517"),
          ItemRegistry.Create("(O)466", 10),
          ItemRegistry.Create("(O)422"),
          ItemRegistry.Create("(O)392"),
          ItemRegistry.Create("(O)348"),
          ItemRegistry.Create("(O)346"),
          ItemRegistry.Create("(O)341"),
          ItemRegistry.Create("(O)221"),
          ItemRegistry.Create("(O)64"),
          ItemRegistry.Create("(O)60"),
          ItemRegistry.Create("(O)70")
        });
    }
    return random.ChooseFrom<Item>((IList<Item>) options);
  }

  public static NPC getTopRomanticInterest(Farmer who)
  {
    NPC topSpot = (NPC) null;
    int highestFriendPoints = -1;
    Utility.ForEachVillager((Func<NPC, bool>) (n =>
    {
      if (who.friendshipData.ContainsKey(n.Name) && n.datable.Value && who.getFriendshipLevelForNPC(n.Name) > highestFriendPoints)
      {
        topSpot = n;
        highestFriendPoints = who.getFriendshipLevelForNPC(n.Name);
      }
      return true;
    }));
    return topSpot;
  }

  public static Color getRandomRainbowColor(Random r = null)
  {
    switch (r == null ? Game1.random.Next(8) : r.Next(8))
    {
      case 0:
        return Color.Red;
      case 1:
        return Color.Orange;
      case 2:
        return Color.Yellow;
      case 3:
        return Color.Lime;
      case 4:
        return Color.Cyan;
      case 5:
        return new Color(0, 100, (int) byte.MaxValue);
      case 6:
        return new Color(152, 96 /*0x60*/, (int) byte.MaxValue);
      case 7:
        return new Color((int) byte.MaxValue, 100, (int) byte.MaxValue);
      default:
        return Color.White;
    }
  }

  public static NPC getTopNonRomanticInterest(Farmer who)
  {
    NPC topSpot = (NPC) null;
    int highestFriendPoints = -1;
    Utility.ForEachVillager((Func<NPC, bool>) (n =>
    {
      if (who.friendshipData.ContainsKey(n.Name) && !n.datable.Value && who.getFriendshipLevelForNPC(n.Name) > highestFriendPoints)
      {
        topSpot = n;
        highestFriendPoints = who.getFriendshipLevelForNPC(n.Name);
      }
      return true;
    }));
    return topSpot;
  }

  /// <summary>Get which of a player's skills has the highest number of experience points.</summary>
  /// <param name="who">The player whose skills to check.</param>
  public static int getHighestSkill(Farmer who)
  {
    int num = 0;
    int highestSkill = 0;
    for (int index = 0; index < who.experiencePoints.Length; ++index)
    {
      int experiencePoint = who.experiencePoints[index];
      if (who.experiencePoints[index] > num)
      {
        num = experiencePoint;
        highestSkill = index;
      }
    }
    return highestSkill;
  }

  public static int getNumberOfFriendsWithinThisRange(
    Farmer who,
    int minFriendshipPoints,
    int maxFriendshipPoints,
    bool romanceOnly = false)
  {
    int number = 0;
    Utility.ForEachVillager((Func<NPC, bool>) (n =>
    {
      int? friendshipLevelForNpc = who.tryGetFriendshipLevelForNPC(n.Name);
      int? nullable = friendshipLevelForNpc;
      int num = minFriendshipPoints;
      if (nullable.GetValueOrDefault() >= num & nullable.HasValue && friendshipLevelForNpc.Value <= maxFriendshipPoints && (!romanceOnly || n.datable.Value))
        ++number;
      return true;
    }));
    return number;
  }

  public static bool highlightLuauSoupItems(Item i)
  {
    if (!(i is Object @object))
      return false;
    return @object.edibility.Value != -300 && @object.Category != -7 || @object.QualifiedItemId == "(O)789" || @object.QualifiedItemId == "(O)71";
  }

  public static bool highlightSmallObjects(Item i)
  {
    return i is Object @object && !@object.bigCraftable.Value;
  }

  public static bool highlightSantaObjects(Item i)
  {
    return i.canBeTrashed() && i.canBeGivenAsGift() && Utility.highlightSmallObjects(i);
  }

  public static bool highlightShippableObjects(Item i) => i?.canBeShipped() ?? false;

  public static int getFarmerNumberFromFarmer(Farmer who)
  {
    if (who != null)
    {
      if (who.IsMainPlayer)
        return 1;
      int numberFromFarmer = 2;
      foreach (Farmer farmer in Game1.otherFarmers.Values.OrderBy<Farmer, long>((Func<Farmer, long>) (f => f.UniqueMultiplayerID)).Where<Farmer>((Func<Farmer, bool>) (f => !f.IsMainPlayer)))
      {
        if (farmer.UniqueMultiplayerID == who.UniqueMultiplayerID)
          return numberFromFarmer;
        ++numberFromFarmer;
      }
    }
    return -1;
  }

  public static Farmer getFarmerFromFarmerNumber(int number)
  {
    if (number <= 1)
      return Game1.MasterPlayer;
    int num = 2;
    foreach (Farmer fromFarmerNumber in Game1.otherFarmers.Values.OrderBy<Farmer, long>((Func<Farmer, long>) (f => f.UniqueMultiplayerID)).Where<Farmer>((Func<Farmer, bool>) (f => !f.IsMainPlayer)))
    {
      if (num == number)
        return fromFarmerNumber;
      ++num;
    }
    return (Farmer) null;
  }

  public static string getLoveInterest(string who)
  {
    if (who != null)
    {
      switch (who.Length)
      {
        case 3:
          if (who == "Sam")
            return "Penny";
          break;
        case 4:
          switch (who[0])
          {
            case 'A':
              if (who == "Alex")
                return "Haley";
              break;
            case 'L':
              if (who == "Leah")
                return "Elliott";
              break;
            case 'M':
              if (who == "Maru")
                return "Harvey";
              break;
          }
          break;
        case 5:
          switch (who[0])
          {
            case 'E':
              if (who == "Emily")
                return "Shane";
              break;
            case 'H':
              if (who == "Haley")
                return "Alex";
              break;
            case 'P':
              if (who == "Penny")
                return "Sam";
              break;
            case 'S':
              if (who == "Shane")
                return "Emily";
              break;
          }
          break;
        case 6:
          if (who == "Harvey")
            return "Maru";
          break;
        case 7:
          switch (who[0])
          {
            case 'A':
              if (who == "Abigail")
                return "Sebastian";
              break;
            case 'E':
              if (who == "Elliott")
                return "Leah";
              break;
          }
          break;
        case 9:
          if (who == "Sebastian")
            return "Abigail";
          break;
      }
    }
    return "";
  }

  public static string ParseGiftReveals(string str)
  {
    string str1 = str;
    try
    {
      while (true)
      {
        int startIndex1 = str.IndexOf("%revealtaste");
        if (startIndex1 >= 0)
        {
          int num = startIndex1 + "%revealtaste".Length;
          for (int index = num; index < str.Length; ++index)
          {
            char c = str[index];
            if (!char.IsWhiteSpace(c) && c != '#' && c != '%' && c != '$' && c != '{' && c != '^' && c != '*')
              num = index;
            else
              break;
          }
          string str2 = str.Substring(startIndex1, num - startIndex1 + 1);
          string[] strArray = str2.Split(':');
          if (strArray.Length == 3 && strArray[0] == "%revealtaste")
          {
            string name = strArray[1].Trim();
            NPC characterFromName = Game1.getCharacterFromName(name);
            ItemMetadata metadata = ItemRegistry.GetMetadata(strArray[2].Trim());
            if (metadata == null)
              Game1.log.Warn($"Failed to parse gift taste reveal '{str2}' in dialogue '{str}'. There is no item with that ID.");
            else
              Game1.player.revealGiftTaste(characterFromName?.Name ?? name, metadata.LocalItemId);
            str = str.Remove(startIndex1, str2.Length);
          }
          else
          {
            int startIndex2 = startIndex1 + "%revealtaste".Length;
            int index = startIndex1 + 1;
            if (index >= str.Length)
              index = str.Length - 1;
            while (index < str.Length && (str[index] < '0' || str[index] > '9'))
              ++index;
            string name = str.Substring(startIndex2, index - startIndex2);
            int startIndex3 = index;
            while (index < str.Length && str[index] >= '0' && str[index] <= '9')
              ++index;
            string itemId = str.Substring(startIndex3, index - startIndex3);
            str = str.Remove(startIndex1, index - startIndex1);
            Game1.player.revealGiftTaste(Game1.getCharacterFromName(name)?.Name ?? name, itemId);
          }
        }
        else
          break;
      }
    }
    catch (Exception ex)
    {
      Game1.log.Error($"Error parsing gift taste reveals in string '{str1}'.", ex);
    }
    return str;
  }

  public static void Shuffle<T>(Random rng, List<T> list)
  {
    int count = list.Count;
    while (count > 1)
    {
      int index = rng.Next(count--);
      T obj = list[count];
      list[count] = list[index];
      list[index] = obj;
    }
  }

  public static void Shuffle<T>(Random rng, T[] array)
  {
    int length = array.Length;
    while (length > 1)
    {
      int index = rng.Next(length--);
      T obj = array[length];
      array[length] = array[index];
      array[index] = obj;
    }
  }

  /// <summary>Get the unique key for a season (one of <c>spring</c>, <c>summer</c>, <c>fall</c>, or <c>winter</c>).</summary>
  /// <param name="season">The season value.</param>
  public static string getSeasonKey(Season season)
  {
    switch (season)
    {
      case Season.Spring:
        return "spring";
      case Season.Summer:
        return "summer";
      case Season.Fall:
        return "fall";
      case Season.Winter:
        return "winter";
      default:
        return season.ToString().ToLower();
    }
  }

  public static int getSeasonNumber(string whichSeason)
  {
    Season parsed;
    if (Utility.TryParseEnum<Season>(whichSeason, out parsed))
      return (int) parsed;
    return whichSeason.EqualsIgnoreCase("autumn") ? 2 : -1;
  }

  /// <summary>
  /// uses Game1.random so this will not be the same each time it's called in the same context.
  /// </summary>
  /// <param name="startTile"></param>
  /// <param name="number"></param>
  /// <returns></returns>
  public static List<Vector2> getPositionsInClusterAroundThisTile(Vector2 startTile, int number)
  {
    Queue<Vector2> vector2Queue = new Queue<Vector2>();
    List<Vector2> clusterAroundThisTile = new List<Vector2>();
    Vector2 vector2_1 = startTile;
    vector2Queue.Enqueue(vector2_1);
    while (clusterAroundThisTile.Count < number)
    {
      Vector2 vector2_2 = vector2Queue.Dequeue();
      clusterAroundThisTile.Add(vector2_2);
      if (!clusterAroundThisTile.Contains(new Vector2(vector2_2.X + 1f, vector2_2.Y)))
        vector2Queue.Enqueue(new Vector2(vector2_2.X + 1f, vector2_2.Y));
      if (!clusterAroundThisTile.Contains(new Vector2(vector2_2.X - 1f, vector2_2.Y)))
        vector2Queue.Enqueue(new Vector2(vector2_2.X - 1f, vector2_2.Y));
      if (!clusterAroundThisTile.Contains(new Vector2(vector2_2.X, vector2_2.Y + 1f)))
        vector2Queue.Enqueue(new Vector2(vector2_2.X, vector2_2.Y + 1f));
      if (!clusterAroundThisTile.Contains(new Vector2(vector2_2.X, vector2_2.Y - 1f)))
        vector2Queue.Enqueue(new Vector2(vector2_2.X, vector2_2.Y - 1f));
    }
    return clusterAroundThisTile;
  }

  public static bool doesPointHaveLineOfSightInMine(
    GameLocation mine,
    Vector2 start,
    Vector2 end,
    int visionDistance)
  {
    if ((double) Vector2.Distance(start, end) > (double) visionDistance)
      return false;
    foreach (Point tile in Utility.GetPointsOnLine((int) start.X, (int) start.Y, (int) end.X, (int) end.Y))
    {
      if (mine.hasTileAt(tile, "Buildings"))
        return false;
    }
    return true;
  }

  public static void addSprinklesToLocation(
    GameLocation l,
    int sourceXTile,
    int sourceYTile,
    int tilesWide,
    int tilesHigh,
    int totalSprinkleDuration,
    int millisecondsBetweenSprinkles,
    Color sprinkleColor,
    string sound = null,
    bool motionTowardCenter = false)
  {
    Microsoft.Xna.Framework.Rectangle r = new Microsoft.Xna.Framework.Rectangle(sourceXTile - tilesWide / 2, sourceYTile - tilesHigh / 2, tilesWide, tilesHigh);
    Random random = Game1.random;
    int num = totalSprinkleDuration / millisecondsBetweenSprinkles;
    for (int index = 0; index < num; ++index)
    {
      Vector2 vector2 = Utility.getRandomPositionInThisRectangle(r, random) * 64f;
      l.temporarySprites.Add(new TemporaryAnimatedSprite(random.Next(10, 12), vector2, sprinkleColor, animationInterval: 50f)
      {
        layerDepth = 1f,
        delayBeforeAnimationStart = millisecondsBetweenSprinkles * index,
        interval = 100f,
        startSound = sound,
        motion = motionTowardCenter ? Utility.getVelocityTowardPoint(vector2, new Vector2((float) sourceXTile, (float) sourceYTile) * 64f, Vector2.Distance(new Vector2((float) sourceXTile, (float) sourceYTile) * 64f, vector2) / 64f) : Vector2.Zero,
        xStopCoordinate = sourceXTile,
        yStopCoordinate = sourceYTile
      });
    }
  }

  public static void addRainbowStarExplosion(GameLocation l, Vector2 origin, int numStars)
  {
    List<TemporaryAnimatedSprite> values = new List<TemporaryAnimatedSprite>();
    float num1 = 6.28318548f / (float) Math.Max(1, numStars - 1);
    Vector2 vector2 = new Vector2(0.0f, -4f);
    double num2 = Game1.random.NextDouble() * Math.PI * 2.0;
    for (int index = 0; index < numStars; ++index)
    {
      values.Add(new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(0, 640, 64 /*0x40*/, 64 /*0x40*/), origin + vector2, false, 0.03f, Utility.GetPrismaticColor(Game1.random.Next(99999)))
      {
        motion = Utility.getVectorDirection(origin, origin + vector2, true) * 0.06f * 150f,
        acceleration = -Utility.getVectorDirection(origin, origin + vector2, true) * 0.06f * 6f,
        totalNumberOfLoops = 1,
        animationLength = 8,
        interval = 50f,
        drawAboveAlwaysFront = true,
        rotation = (float) (-1.5707963705062866 - (double) num1 * (double) index)
      });
      vector2.X = 4f * (float) Math.Sin((double) num1 * (double) (index + 1) + num2);
      vector2.Y = 4f * (float) Math.Cos((double) num1 * (double) (index + 1) + num2);
    }
    values.Add(new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(0, 320, 64 /*0x40*/, 64 /*0x40*/), origin + vector2, false, 0.03f, Color.White)
    {
      totalNumberOfLoops = 1,
      animationLength = 8,
      interval = 60f,
      drawAboveAlwaysFront = true
    });
    l.temporarySprites.AddRange((IEnumerable<TemporaryAnimatedSprite>) values);
  }

  public static Vector2 getVectorDirection(Vector2 start, Vector2 finish, bool normalize = false)
  {
    Vector2 vectorDirection = new Vector2(finish.X - start.X, finish.Y - start.Y);
    if (normalize)
      vectorDirection.Normalize();
    return vectorDirection;
  }

  public static TemporaryAnimatedSpriteList getStarsAndSpirals(
    GameLocation l,
    int sourceXTile,
    int sourceYTile,
    int tilesWide,
    int tilesHigh,
    int totalSprinkleDuration,
    int millisecondsBetweenSprinkles,
    Color sprinkleColor,
    string sound = null,
    bool motionTowardCenter = false)
  {
    Microsoft.Xna.Framework.Rectangle r = new Microsoft.Xna.Framework.Rectangle(sourceXTile - tilesWide / 2, sourceYTile - tilesHigh / 2, tilesWide, tilesHigh);
    Random random = Utility.CreateRandom((double) (sourceXTile * 7), (double) (sourceYTile * 77), Game1.currentGameTime.TotalGameTime.TotalSeconds);
    int num = totalSprinkleDuration / millisecondsBetweenSprinkles;
    TemporaryAnimatedSpriteList starsAndSpirals = new TemporaryAnimatedSpriteList();
    for (int index = 0; index < num; ++index)
    {
      Vector2 position = Utility.getRandomPositionInThisRectangle(r, random) * 64f;
      starsAndSpirals.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", random.NextBool() ? new Microsoft.Xna.Framework.Rectangle(359, 1437, 14, 14) : new Microsoft.Xna.Framework.Rectangle(377, 1438, 9, 9), position, false, 0.01f, sprinkleColor)
      {
        xPeriodic = true,
        xPeriodicLoopTime = (float) random.Next(2000, 3000),
        xPeriodicRange = (float) random.Next(-64, 64 /*0x40*/),
        motion = new Vector2(0.0f, -2f),
        rotationChange = 3.14159274f / (float) random.Next(4, 64 /*0x40*/),
        delayBeforeAnimationStart = millisecondsBetweenSprinkles * index,
        layerDepth = 1f,
        scaleChange = 0.04f,
        scaleChangeChange = -0.0008f,
        scale = 4f
      });
    }
    return starsAndSpirals;
  }

  public static void addStarsAndSpirals(
    GameLocation l,
    int sourceXTile,
    int sourceYTile,
    int tilesWide,
    int tilesHigh,
    int totalSprinkleDuration,
    int millisecondsBetweenSprinkles,
    Color sprinkleColor,
    string sound = null,
    bool motionTowardCenter = false)
  {
    l.temporarySprites.AddRange((IEnumerable<TemporaryAnimatedSprite>) Utility.getStarsAndSpirals(l, sourceXTile, sourceYTile, tilesWide, tilesHigh, totalSprinkleDuration, millisecondsBetweenSprinkles, sprinkleColor, sound, motionTowardCenter));
  }

  public static Vector2 snapDrawPosition(Vector2 draw_position)
  {
    return new Vector2((float) (int) draw_position.X, (float) (int) draw_position.Y);
  }

  public static Vector2 clampToTile(Vector2 nonTileLocation)
  {
    nonTileLocation.X -= nonTileLocation.X % 64f;
    nonTileLocation.Y -= nonTileLocation.Y % 64f;
    return nonTileLocation;
  }

  public static float distance(float x1, float x2, float y1, float y2)
  {
    return (float) Math.Sqrt(((double) x2 - (double) x1) * ((double) x2 - (double) x1) + ((double) y2 - (double) y1) * ((double) y2 - (double) y1));
  }

  public static bool couldSeePlayerInPeripheralVision(Farmer player, Character c)
  {
    Point standingPixel1 = player.StandingPixel;
    Point standingPixel2 = c.StandingPixel;
    switch (c.FacingDirection)
    {
      case 0:
        if (standingPixel1.Y < standingPixel2.Y + 32 /*0x20*/)
          return true;
        break;
      case 1:
        if (standingPixel1.X > standingPixel2.X - 32 /*0x20*/)
          return true;
        break;
      case 2:
        if (standingPixel1.Y > standingPixel2.Y - 32 /*0x20*/)
          return true;
        break;
      case 3:
        if (standingPixel1.X < standingPixel2.X + 32 /*0x20*/)
          return true;
        break;
    }
    return false;
  }

  public static IEnumerable<Point> GetPointsOnLine(int x0, int y0, int x1, int y1)
  {
    return Utility.GetPointsOnLine(x0, y0, x1, y1, false);
  }

  public static List<Vector2> getBorderOfThisRectangle(Microsoft.Xna.Framework.Rectangle r)
  {
    List<Vector2> borderOfThisRectangle = new List<Vector2>();
    for (int x = r.X; x < r.Right; ++x)
      borderOfThisRectangle.Add(new Vector2((float) x, (float) r.Y));
    for (int y = r.Y + 1; y < r.Bottom; ++y)
      borderOfThisRectangle.Add(new Vector2((float) (r.Right - 1), (float) y));
    for (int x = r.Right - 2; x >= r.X; --x)
      borderOfThisRectangle.Add(new Vector2((float) x, (float) (r.Bottom - 1)));
    for (int y = r.Bottom - 2; y >= r.Y + 1; --y)
      borderOfThisRectangle.Add(new Vector2((float) r.X, (float) y));
    return borderOfThisRectangle;
  }

  /// <summary>Get the closest valid monster within range of a pixel position, if any.</summary>
  /// <param name="location">The location whose monsters to search.</param>
  /// <param name="originPoint">The pixel position from which to find a nearby monster.</param>
  /// <param name="range">The maximum pixel distance from the <paramref name="originPoint" /> within which to match monsters.</param>
  /// <param name="ignoreUntargetables">Whether to ignore monsters which can't normally be targeted by the player.</param>
  /// <param name="match">If set, a callback which returns whether a matched monster is valid.</param>
  public static Monster findClosestMonsterWithinRange(
    GameLocation location,
    Vector2 originPoint,
    int range,
    bool ignoreUntargetables = false,
    Func<Monster, bool> match = null)
  {
    Monster monsterWithinRange = (Monster) null;
    float num1 = (float) (range + 1);
    foreach (NPC character in location.characters)
    {
      if (character is Monster monster && (!ignoreUntargetables || !(character is Spiker)) && (match == null || match(monster)))
      {
        float num2 = Vector2.Distance(originPoint, character.getStandingPosition());
        if ((double) num2 <= (double) range && (double) num2 < (double) num1 && !monster.IsInvisible)
        {
          monsterWithinRange = monster;
          num1 = num2;
        }
      }
    }
    return monsterWithinRange;
  }

  public static Microsoft.Xna.Framework.Rectangle getTranslatedRectangle(
    Microsoft.Xna.Framework.Rectangle r,
    int xTranslate,
    int yTranslate = 0)
  {
    return Utility.translateRect(r, xTranslate, yTranslate);
  }

  public static Microsoft.Xna.Framework.Rectangle translateRect(
    Microsoft.Xna.Framework.Rectangle r,
    int xTranslate,
    int yTranslate = 0)
  {
    r.X += xTranslate;
    r.Y += yTranslate;
    return r;
  }

  public static Point getTranslatedPoint(Point p, int direction, int movementAmount)
  {
    switch (direction)
    {
      case 0:
        return new Point(p.X, p.Y - movementAmount);
      case 1:
        return new Point(p.X + movementAmount, p.Y);
      case 2:
        return new Point(p.X, p.Y + movementAmount);
      case 3:
        return new Point(p.X - movementAmount, p.Y);
      default:
        return p;
    }
  }

  public static Vector2 getTranslatedVector2(Vector2 p, int direction, float movementAmount)
  {
    switch (direction)
    {
      case 0:
        return new Vector2(p.X, p.Y - movementAmount);
      case 1:
        return new Vector2(p.X + movementAmount, p.Y);
      case 2:
        return new Vector2(p.X, p.Y + movementAmount);
      case 3:
        return new Vector2(p.X - movementAmount, p.Y);
      default:
        return p;
    }
  }

  public static IEnumerable<Point> GetPointsOnLine(
    int x0,
    int y0,
    int x1,
    int y1,
    bool ignoreSwap)
  {
    bool steep = Math.Abs(y1 - y0) > Math.Abs(x1 - x0);
    if (steep)
    {
      int num1 = x0;
      x0 = y0;
      y0 = num1;
      int num2 = x1;
      x1 = y1;
      y1 = num2;
    }
    if (!ignoreSwap && x0 > x1)
    {
      int num3 = x0;
      x0 = x1;
      x1 = num3;
      int num4 = y0;
      y0 = y1;
      y1 = num4;
    }
    int dx = x1 - x0;
    int dy = Math.Abs(y1 - y0);
    int error = dx / 2;
    int ystep = y0 < y1 ? 1 : -1;
    int y = y0;
    for (int x = x0; x <= x1; ++x)
    {
      yield return new Point(steep ? y : x, steep ? x : y);
      error -= dy;
      if (error < 0)
      {
        y += ystep;
        error += dx;
      }
    }
  }

  public static Vector2 getRandomAdjacentOpenTile(Vector2 tile, GameLocation location)
  {
    List<Vector2> adjacentTileLocations = Utility.getAdjacentTileLocations(tile);
    int num = 0;
    int index = Game1.random.Next(adjacentTileLocations.Count);
    Vector2 tile1;
    for (tile1 = adjacentTileLocations[index]; num < 4 && location.IsTileBlockedBy(tile1); ++num)
    {
      index = (index + 1) % adjacentTileLocations.Count;
      tile1 = adjacentTileLocations[index];
    }
    return num >= 4 ? Vector2.Zero : tile1;
  }

  public static void CollectSingleItemOrShowChestMenu(Chest chest, object context = null)
  {
    int num = 0;
    Item obj = (Item) null;
    IInventory items = (IInventory) chest.Items;
    for (int index = 0; index < items.Count; ++index)
    {
      if (items[index] != null)
      {
        ++num;
        if (num == 1)
          obj = items[index];
        if (num == 2)
        {
          obj = (Item) null;
          break;
        }
      }
    }
    if (num == 0)
      return;
    if (obj != null)
    {
      int stack = obj.Stack;
      if (Game1.player.addItemToInventory(obj) == null)
      {
        Game1.playSound("coin");
        items.Remove(obj);
        chest.clearNulls();
        return;
      }
      if (obj.Stack != stack)
        Game1.playSound("coin");
    }
    Game1.activeClickableMenu = (IClickableMenu) new ItemGrabMenu((IList<Item>) items, false, true, new InventoryMenu.highlightThisItem(InventoryMenu.highlightAllItems), new ItemGrabMenu.behaviorOnItemSelect(chest.grabItemFromInventory), (string) null, new ItemGrabMenu.behaviorOnItemSelect(chest.grabItemFromChest), canBeExitedWithKey: true, showOrganizeButton: true, source: 1, context: context);
  }

  /// <summary>Add the item to the player's inventory if there's room, and drop any remainder at their feet.</summary>
  /// <param name="item">The item to collect or drop.</param>
  /// <param name="direction">The direction in which to drop the item relative to the player, or <c>-1</c> to use their facing direction.</param>
  /// <returns>Returns <c>true</c> if the item item was fully added to their inventory, or <c>false</c> if any were dropped.</returns>
  public static bool CollectOrDrop(Item item, int direction)
  {
    if (item == null)
      return true;
    item = Game1.player.addItemToInventory(item);
    if (item == null)
      return true;
    if (direction != -1)
      Game1.createItemDebris(item, Game1.player.getStandingPosition(), direction);
    else
      Game1.createItemDebris(item, Game1.player.getStandingPosition(), Game1.player.FacingDirection);
    return false;
  }

  public static bool CollectOrDrop(Item item) => Utility.CollectOrDrop(item, -1);

  public static List<string> getExes(Farmer farmer)
  {
    List<string> exes = new List<string>();
    foreach (string key in farmer.friendshipData.Keys)
    {
      if (farmer.friendshipData[key].IsDivorced())
        exes.Add(key);
    }
    return exes;
  }

  public static void fixAllAnimals()
  {
    if (!Game1.IsMasterGame)
      return;
    List<GameLocation> animalLocations = new List<GameLocation>();
    HashSet<long> uniqueAnimals = new HashSet<long>();
    List<long> animalsToRemove = new List<long>();
    Utility.ForEachLocation((Func<GameLocation, bool>) (f =>
    {
      if (f.animals.Length == 0 && f.buildings.Count == 0)
        return true;
      animalLocations.Clear();
      animalLocations.Add(f);
      foreach (Building building in f.buildings)
      {
        GameLocation indoors4 = building.GetIndoors();
        if (indoors4 != null && indoors4.animals.Length > 0)
          animalLocations.Add(indoors4);
      }
      bool flag1 = false;
      bool flag2 = false;
      foreach (GameLocation gameLocation in animalLocations)
      {
        AnimalHouse animalHouse = gameLocation as AnimalHouse;
        animalsToRemove.Clear();
        foreach (KeyValuePair<long, NetRef<FarmAnimal>> keyValuePair in gameLocation.animals.FieldDict)
        {
          if (keyValuePair.Value?.Value == null)
          {
            animalsToRemove.Add(keyValuePair.Key);
          }
          else
          {
            if (keyValuePair.Value.Value.home == null)
              flag1 = true;
            if (!uniqueAnimals.Add(keyValuePair.Value.Value.myID.Value))
              animalsToRemove.Add(keyValuePair.Key);
          }
        }
        flag2 = flag2 || animalsToRemove.Count > 0;
        foreach (long key in animalsToRemove)
        {
          long animalId = gameLocation.animals[key].myID.Value;
          gameLocation.animals.Remove(key);
          animalHouse?.animalsThatLiveHere.RemoveWhere((Func<long, bool>) (id => id == animalId));
        }
      }
      foreach (Building building in f.buildings)
      {
        if (building.GetIndoors() is AnimalHouse indoors5)
        {
          foreach (long id in (NetList<long, NetLong>) indoors5.animalsThatLiveHere)
          {
            FarmAnimal animal = Utility.getAnimal(id);
            if (animal != null)
            {
              if (animal.home == null)
                flag1 = true;
              animal.homeInterior = (GameLocation) indoors5;
            }
          }
        }
      }
      if (!flag1 && !flag2)
        return true;
      List<FarmAnimal> allFarmAnimals = f.getAllFarmAnimals();
      allFarmAnimals.RemoveAll((Predicate<FarmAnimal>) (a => a.home != null));
      foreach (FarmAnimal farmAnimal in allFarmAnimals)
      {
        FarmAnimal a = farmAnimal;
        foreach (Building building in f.buildings)
          building.GetIndoors()?.animals.RemoveWhere((Func<KeyValuePair<long, FarmAnimal>, bool>) (pair => pair.Value.Equals((object) a)));
        f.animals.RemoveWhere((Func<KeyValuePair<long, FarmAnimal>, bool>) (pair => pair.Value.Equals((object) a)));
      }
      foreach (Building building in f.buildings)
      {
        Building b = building;
        if (b.GetIndoors() is AnimalHouse indoors6)
          indoors6.animalsThatLiveHere.RemoveWhere((Func<long, bool>) (id => Utility.getAnimal(id)?.home != b));
      }
      foreach (FarmAnimal animal in allFarmAnimals)
      {
        foreach (Building building in f.buildings)
        {
          if (animal.CanLiveIn(building) && building.GetIndoors() is AnimalHouse indoors7 && !indoors7.isFull())
          {
            indoors7.adoptAnimal(animal);
            break;
          }
        }
      }
      foreach (FarmAnimal c in allFarmAnimals)
      {
        if (c.home == null)
        {
          c.Position = Utility.recursiveFindOpenTileForCharacter((Character) c, f, new Vector2(40f, 40f), 200) * 64f;
          f.animals.TryAdd(c.myID.Value, c);
        }
      }
      return true;
    }), false);
  }

  /// <summary>Create a generated event to marry a player's current NPC or player spouse.</summary>
  /// <param name="farmer">The player getting married.</param>
  public static Event getWeddingEvent(Farmer farmer)
  {
    Farmer who = (Farmer) null;
    long? spouse = farmer.team.GetSpouse(farmer.UniqueMultiplayerID);
    if (spouse.HasValue)
      who = Game1.GetPlayer(spouse.Value);
    string spouseActor = who != null ? nameof (farmer) + Utility.getFarmerNumberFromFarmer(who).ToString() : farmer.spouse;
    WeddingData weddingData = DataLoader.Weddings(Game1.content);
    List<WeddingAttendeeData> contextualAttendees = new List<WeddingAttendeeData>();
    if (weddingData.Attendees != null)
    {
      List<string> exes = Utility.getExes(farmer);
      foreach (WeddingAttendeeData weddingAttendeeData in weddingData.Attendees.Values)
      {
        CharacterData data;
        if (!exes.Contains(weddingAttendeeData.Id) && !(weddingAttendeeData.Id == farmer.spouse) && GameStateQuery.CheckConditions(weddingAttendeeData.Condition, player: farmer) && (weddingAttendeeData.IgnoreUnlockConditions || !NPC.TryGetData(weddingAttendeeData.Id, out data) || GameStateQuery.CheckConditions(data.UnlockConditions, player: farmer)))
          contextualAttendees.Add(weddingAttendeeData);
      }
    }
    string text;
    if (!weddingData.EventScript.TryGetValue(spouse?.ToString() ?? farmer.spouse, out text) && !weddingData.EventScript.TryGetValue("default", out text))
      throw new InvalidOperationException("The Data/Weddings asset has no wedding script with the 'default' script key.");
    text = TokenParser.ParseText(text, customParser: new TokenParserDelegate(ParseWeddingToken), player: farmer);
    return new Event(text, (string) null, "-2", farmer);

    bool ParseWeddingToken(string[] query, out string replacement, Random random, Farmer player)
    {
      switch (ArgUtility.Get(query, 0)?.ToLower())
      {
        case "spouseactor":
          replacement = spouseActor;
          return true;
        case "setupcontextualweddingattendees":
          StringBuilder stringBuilder1 = new StringBuilder();
          foreach (WeddingAttendeeData weddingAttendeeData in contextualAttendees)
          {
            stringBuilder1.Append(" ");
            stringBuilder1.Append(weddingAttendeeData.Setup);
          }
          replacement = stringBuilder1.ToString();
          return true;
        case "contextualweddingcelebrations":
          StringBuilder stringBuilder2 = new StringBuilder();
          foreach (WeddingAttendeeData weddingAttendeeData in contextualAttendees)
          {
            if (weddingAttendeeData.Celebration != null)
            {
              stringBuilder2.Append(weddingAttendeeData.Celebration);
              stringBuilder2.Append("/");
            }
          }
          replacement = stringBuilder2.ToString();
          return true;
        default:
          replacement = (string) null;
          return false;
      }
    }
  }

  /// <summary>Draw a box to the screen.</summary>
  /// <param name="b">The sprite batch being drawn.</param>
  /// <param name="pixelArea">The pixel area of the box to draw.</param>
  /// <param name="borderWidth">The width of the border to draw.</param>
  /// <param name="borderColor">The color of the border to draw, or <c>null</c> for black.</param>
  /// <param name="backgroundColor">The background color to draw, or <c>null</c> for none.</param>
  public static void DrawSquare(
    SpriteBatch b,
    Microsoft.Xna.Framework.Rectangle pixelArea,
    int borderWidth,
    Color? borderColor = null,
    Color? backgroundColor = null)
  {
    if (backgroundColor.HasValue)
      b.Draw(Game1.staminaRect, pixelArea, backgroundColor.Value);
    if (borderWidth <= 0)
      return;
    Color color = borderColor ?? Color.Black;
    b.Draw(Game1.staminaRect, new Microsoft.Xna.Framework.Rectangle(pixelArea.X, pixelArea.Y, pixelArea.Width, borderWidth), color);
    b.Draw(Game1.staminaRect, new Microsoft.Xna.Framework.Rectangle(pixelArea.X, pixelArea.Y + pixelArea.Height - borderWidth, pixelArea.Width, borderWidth), color);
    b.Draw(Game1.staminaRect, new Microsoft.Xna.Framework.Rectangle(pixelArea.X, pixelArea.Y, borderWidth, pixelArea.Height), color);
    b.Draw(Game1.staminaRect, new Microsoft.Xna.Framework.Rectangle(pixelArea.X + pixelArea.Width - borderWidth, pixelArea.Y, borderWidth, pixelArea.Height), color);
  }

  /// <summary>Draw a missing-texture image to the screen.</summary>
  /// <param name="spriteBatch">The sprite batch being drawn.</param>
  /// <param name="screenArea">The pixel area within the <see cref="F:StardewValley.Game1.viewport" /> to cover with the error texture.</param>
  /// <param name="layerDepth">The layer depth at which to draw the error texture in the <paramref name="spriteBatch" />.</param>
  public static void DrawErrorTexture(
    SpriteBatch spriteBatch,
    Microsoft.Xna.Framework.Rectangle screenArea,
    float layerDepth)
  {
    spriteBatch.Draw(Game1.mouseCursors, screenArea, new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(320, 496, 16 /*0x10*/, 16 /*0x10*/)), Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, layerDepth);
  }

  public static void drawTinyDigits(
    int toDraw,
    SpriteBatch b,
    Vector2 position,
    float scale,
    float layerDepth,
    Color c)
  {
    int x = 0;
    int num1 = toDraw;
    int num2 = 0;
    do
    {
      ++num2;
    }
    while ((toDraw /= 10) >= 1);
    int num3 = (int) Math.Pow(10.0, (double) (num2 - 1));
    bool flag = false;
    for (int index = 0; index < num2; ++index)
    {
      int num4 = num1 / num3 % 10;
      if (num4 > 0 || index == num2 - 1)
        flag = true;
      if (flag)
        b.Draw(Game1.mouseCursors, position + new Vector2((float) x, 0.0f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(368 + num4 * 5, 56, 5, 7)), c, 0.0f, Vector2.Zero, scale, SpriteEffects.None, layerDepth);
      x += (int) (5.0 * (double) scale) - 1;
      num3 /= 10;
    }
  }

  public static int getWidthOfTinyDigitString(int toDraw, float scale)
  {
    int num = 0;
    do
    {
      ++num;
    }
    while ((toDraw /= 10) >= 1);
    return (int) ((double) (num * 5) * (double) scale);
  }

  public static bool isMale(string who)
  {
    CharacterData data;
    return !NPC.TryGetData(who, out data) || data.Gender == Gender.Male;
  }

  public static int GetMaximumHeartsForCharacter(Character character)
  {
    if (character == null)
      return 0;
    int heartsForCharacter = 10;
    if (character is NPC npc && npc.datable.Value)
      heartsForCharacter = 8;
    Friendship friendship;
    if (Game1.player.friendshipData.TryGetValue(character.Name, out friendship))
    {
      if (friendship.IsMarried())
        heartsForCharacter = 14;
      else if (friendship.IsDating())
        heartsForCharacter = 10;
    }
    return heartsForCharacter;
  }

  /// <summary>Get whether an item exists anywhere in the world.</summary>
  /// <param name="itemId">The qualified or unqualified item ID.</param>
  public static bool doesItemExistAnywhere(string itemId)
  {
    itemId = ItemRegistry.QualifyItemId(itemId);
    if (itemId == null)
      return false;
    bool itemFound = false;
    Utility.ForEachItem((Func<Item, bool>) (item =>
    {
      if (item.QualifiedItemId == itemId)
        itemFound = true;
      return !itemFound;
    }));
    return itemFound;
  }

  internal static void CollectGarbage(string filePath = "", int lineNumber = 0)
  {
    GC.Collect(0, GCCollectionMode.Forced);
  }

  public static List<string> possibleCropsAtThisTime(Season season, bool firstWeek)
  {
    List<string> collection = (List<string>) null;
    List<string> stringList = (List<string>) null;
    switch (season)
    {
      case Season.Spring:
        collection = new List<string>() { "24", "192" };
        if (Game1.year > 1)
          collection.Add("250");
        if (Utility.doesAnyFarmerHaveMail("ccVault"))
          collection.Add("248");
        stringList = new List<string>() { "190", "188" };
        if (Utility.doesAnyFarmerHaveMail("ccVault"))
          stringList.Add("252");
        stringList.AddRange((IEnumerable<string>) collection);
        break;
      case Season.Summer:
        collection = new List<string>()
        {
          "264",
          "262",
          "260"
        };
        stringList = new List<string>() { "254", "256" };
        if (Game1.year > 1)
          collection.Add("266");
        if (Utility.doesAnyFarmerHaveMail("ccVault"))
          stringList.AddRange((IEnumerable<string>) new string[2]
          {
            "258",
            "268"
          });
        stringList.AddRange((IEnumerable<string>) collection);
        break;
      case Season.Fall:
        collection = new List<string>() { "272", "278" };
        stringList = new List<string>()
        {
          "270",
          "276",
          "280"
        };
        if (Game1.year > 1)
          stringList.Add("274");
        if (Utility.doesAnyFarmerHaveMail("ccVault"))
        {
          collection.Add("284");
          stringList.Add("282");
        }
        stringList.AddRange((IEnumerable<string>) collection);
        break;
    }
    return !firstWeek ? stringList : collection;
  }

  public static float RandomFloat(float min, float max, Random random = null)
  {
    if (random == null)
      random = Game1.random;
    return Utility.Lerp(min, max, (float) random.NextDouble());
  }

  public static float Clamp(float value, float min, float max)
  {
    if ((double) max < (double) min)
    {
      double num = (double) min;
      min = max;
      max = (float) num;
    }
    if ((double) value < (double) min)
      value = min;
    if ((double) value > (double) max)
      value = max;
    return value;
  }

  public static Color MakeCompletelyOpaque(Color color)
  {
    if (color.A >= byte.MaxValue)
      return color;
    color.A = byte.MaxValue;
    return color;
  }

  public static int Clamp(int value, int min, int max)
  {
    if (max < min)
    {
      int num = min;
      min = max;
      max = num;
    }
    if (value < min)
      value = min;
    if (value > max)
      value = max;
    return value;
  }

  public static float Lerp(float a, float b, float t) => a + t * (b - a);

  public static float MoveTowards(float from, float to, float delta)
  {
    return (double) Math.Abs(to - from) <= (double) delta ? to : from + (float) Math.Sign(to - from) * delta;
  }

  public static Color MultiplyColor(Color a, Color b)
  {
    return new Color((float) ((double) a.R / (double) byte.MaxValue * ((double) b.R / (double) byte.MaxValue)), (float) ((double) a.G / (double) byte.MaxValue * ((double) b.G / (double) byte.MaxValue)), (float) ((double) a.B / (double) byte.MaxValue * ((double) b.B / (double) byte.MaxValue)), (float) ((double) a.A / (double) byte.MaxValue * ((double) b.A / (double) byte.MaxValue)));
  }

  /// <summary>Get the number of minutes until 6am tomorrow.</summary>
  /// <param name="currentTime">The starting time of day, in 26-hour format.</param>
  public static int CalculateMinutesUntilMorning(int currentTime)
  {
    return Utility.CalculateMinutesUntilMorning(currentTime, 1);
  }

  /// <summary>Get the number of minutes until 6am on a given day.</summary>
  /// <param name="currentTime">The starting time of day, in 26-hour format.</param>
  /// <param name="daysElapsed">The day offset (e.g. 1 for tomorrow).</param>
  public static int CalculateMinutesUntilMorning(int currentTime, int daysElapsed)
  {
    return daysElapsed < 1 ? 0 : Utility.ConvertTimeToMinutes(2600) - Utility.ConvertTimeToMinutes(currentTime) + 400 + (daysElapsed - 1) * 1600;
  }

  /// <summary>Get the number of minutes between two times.</summary>
  /// <param name="startTime">The starting time of day, in 26-hour format.</param>
  /// <param name="endTime">The ending time of day, in 26-hour format.</param>
  public static int CalculateMinutesBetweenTimes(int startTime, int endTime)
  {
    return Utility.ConvertTimeToMinutes(endTime) - Utility.ConvertTimeToMinutes(startTime);
  }

  /// <summary>Apply a minute offset to a time of day.</summary>
  /// <param name="timestamp">The initial time of day, in 26-hour format.</param>
  /// <param name="minutes_to_add">The number of minutes to add to the time.</param>
  public static int ModifyTime(int timestamp, int minutes_to_add)
  {
    timestamp = Utility.ConvertTimeToMinutes(timestamp);
    timestamp += minutes_to_add;
    return Utility.ConvertMinutesToTime(timestamp);
  }

  /// <summary>Get the time of day given the number of minutes since midnight.</summary>
  /// <param name="minutes">The number of minutes since midnight.</param>
  public static int ConvertMinutesToTime(int minutes) => minutes / 60 * 100 + minutes % 60;

  /// <summary>Get the number of minutes since midnight for a time.</summary>
  /// <param name="time_stamp">The time of day, in 26-hour format.</param>
  public static int ConvertTimeToMinutes(int time_stamp)
  {
    return time_stamp / 100 * 60 + time_stamp % 100;
  }

  public static int getSellToStorePriceOfItem(Item i, bool countStack = true)
  {
    return i != null ? i.sellToStorePrice(-1L) * (countStack ? i.Stack : 1) : 0;
  }

  /// <summary>Get a list of secret notes or journal scraps that have not been seen.</summary>
  /// <param name="who">The farmer to check for unseen secret notes or journal scraps.</param>
  /// <param name="journal">Whether to get journal scraps (true) or secret notes (false).</param>
  /// <param name="totalNotes">The total number of secret notes or journal scraps (depending on <paramref name="journal" />), including seen ones.</param>
  public static int[] GetUnseenSecretNotes(Farmer who, bool journal, out int totalNotes)
  {
    Func<int, bool> predicate = !journal ? (Func<int, bool>) (id => id < GameLocation.JOURNAL_INDEX) : (Func<int, bool>) (id => id >= GameLocation.JOURNAL_INDEX);
    int[] array = DataLoader.SecretNotes(Game1.content).Keys.Where<int>(predicate).ToArray<int>();
    totalNotes = array.Length;
    return ((IEnumerable<int>) array).Except<int>(who.secretNotesSeen.Where<int>(predicate)).ToArray<int>();
  }

  public static bool HasAnyPlayerSeenSecretNote(int note_number)
  {
    foreach (Farmer allFarmer in Game1.getAllFarmers())
    {
      if (allFarmer.secretNotesSeen.Contains(note_number))
        return true;
    }
    return false;
  }

  public static bool HasAnyPlayerSeenEvent(string eventId)
  {
    foreach (Farmer allFarmer in Game1.getAllFarmers())
    {
      if (allFarmer.eventsSeen.Contains(eventId))
        return true;
    }
    return false;
  }

  public static bool HaveAllPlayersSeenEvent(string eventId)
  {
    foreach (Farmer allFarmer in Game1.getAllFarmers())
    {
      if (!allFarmer.eventsSeen.Contains(eventId))
        return false;
    }
    return true;
  }

  public static List<string> GetAllPlayerUnlockedCookingRecipes()
  {
    List<string> unlockedCookingRecipes = new List<string>();
    foreach (Farmer allFarmer in Game1.getAllFarmers())
    {
      foreach (string key in allFarmer.cookingRecipes.Keys)
      {
        if (!unlockedCookingRecipes.Contains(key))
          unlockedCookingRecipes.Add(key);
      }
    }
    return unlockedCookingRecipes;
  }

  public static List<string> GetAllPlayerUnlockedCraftingRecipes()
  {
    List<string> unlockedCraftingRecipes = new List<string>();
    foreach (Farmer allFarmer in Game1.getAllFarmers())
    {
      foreach (string key in allFarmer.craftingRecipes.Keys)
      {
        if (!unlockedCraftingRecipes.Contains(key))
          unlockedCraftingRecipes.Add(key);
      }
    }
    return unlockedCraftingRecipes;
  }

  public static int GetAllPlayerFriendshipLevel(NPC npc)
  {
    int playerFriendshipLevel = -1;
    foreach (Farmer allFarmer in Game1.getAllFarmers())
    {
      Friendship friendship;
      if (allFarmer.friendshipData.TryGetValue(npc.Name, out friendship) && friendship.Points > playerFriendshipLevel)
        playerFriendshipLevel = friendship.Points;
    }
    return playerFriendshipLevel;
  }

  public static int GetAllPlayerReachedBottomOfMines()
  {
    int reachedBottomOfMines = 0;
    foreach (Farmer allFarmer in Game1.getAllFarmers())
    {
      if (allFarmer.timesReachedMineBottom > reachedBottomOfMines)
        reachedBottomOfMines = allFarmer.timesReachedMineBottom;
    }
    return reachedBottomOfMines;
  }

  public static int GetAllPlayerDeepestMineLevel()
  {
    int deepestMineLevel = 0;
    foreach (Farmer allFarmer in Game1.getAllFarmers())
    {
      if (allFarmer.deepestMineLevel > deepestMineLevel)
        deepestMineLevel = allFarmer.deepestMineLevel;
    }
    return deepestMineLevel;
  }

  public static string LegacyWeatherToWeather(int legacyWeather)
  {
    switch (legacyWeather)
    {
      case 1:
        return "Rain";
      case 2:
        return "Wind";
      case 3:
        return "Storm";
      case 4:
        return "Festival";
      case 5:
        return "Snow";
      case 6:
        return "Wedding";
      default:
        return "Sun";
    }
  }

  public static string getRandomBasicSeasonalForageItem(Season season, int randomSeedAddition = -1)
  {
    Random random = Utility.CreateRandom((double) Game1.uniqueIDForThisGame, (double) randomSeedAddition);
    string[] options = LegacyShims.EmptyArray<string>();
    switch (season)
    {
      case Season.Spring:
        options = new string[4]{ "16", "18", "20", "22" };
        break;
      case Season.Summer:
        options = new string[3]{ "396", "398", "402" };
        break;
      case Season.Fall:
        options = new string[4]
        {
          "404",
          "406",
          "408",
          "410"
        };
        break;
      case Season.Winter:
        options = new string[4]
        {
          "412",
          "414",
          "416",
          "418"
        };
        break;
    }
    return random.ChooseFrom<string>((IList<string>) options) ?? "0";
  }

  public static string getRandomPureSeasonalItem(Season season, int randomSeedAddition)
  {
    Random random = Utility.CreateRandom((double) Game1.uniqueIDForThisGame, (double) randomSeedAddition);
    string[] options = LegacyShims.EmptyArray<string>();
    switch (season)
    {
      case Season.Spring:
        options = new string[15]
        {
          "16",
          "18",
          "20",
          "22",
          "129",
          "131",
          "132",
          "136",
          "137",
          "142",
          "143",
          "145",
          "147",
          "148",
          "152"
        };
        break;
      case Season.Summer:
        options = new string[16 /*0x10*/]
        {
          "128",
          "130",
          "131",
          "132",
          "136",
          "138",
          "142",
          "144",
          "145",
          "146",
          "149",
          "150",
          "155",
          "396",
          "398",
          "402"
        };
        break;
      case Season.Fall:
        options = new string[17]
        {
          "404",
          "406",
          "408",
          "410",
          "129",
          "131",
          "132",
          "136",
          "137",
          "139",
          "140",
          "142",
          "143",
          "148",
          "150",
          "154",
          "155"
        };
        break;
      case Season.Winter:
        options = new string[17]
        {
          "412",
          "414",
          "416",
          "418",
          "130",
          "131",
          "132",
          "136",
          "140",
          "141",
          "143",
          "144",
          "146",
          "147",
          "150",
          "151",
          "154"
        };
        break;
    }
    return random.ChooseFrom<string>((IList<string>) options) ?? "0";
  }

  public static Item CreateFlavoredItem(string baseID, string preservesID, int quality = 0, int stack = 1)
  {
    ItemQueryContext context = new ItemQueryContext(Game1.currentLocation, Game1.player, Game1.random, "FLAVORED_ITEM query");
    if (!(((IEnumerable<ItemQueryResult>) ItemQueryResolver.TryResolve($"FLAVORED_ITEM {baseID} {preservesID}", context)).FirstOrDefault<ItemQueryResult>()?.Item is Item flavoredItem))
      return (Item) null;
    flavoredItem.Quality = quality;
    flavoredItem.Stack = stack;
    return flavoredItem;
  }

  public static string getRandomItemFromSeason(Season season, bool forQuest, Random random)
  {
    Random random1 = random;
    List<string> options = new List<string>()
    {
      "68",
      "66",
      "78",
      "80",
      "86",
      "152",
      "167",
      "153",
      "420"
    };
    List<string> stringList1 = new List<string>((IEnumerable<string>) Game1.player.craftingRecipes.Keys);
    List<string> stringList2 = new List<string>((IEnumerable<string>) Game1.player.cookingRecipes.Keys);
    if (forQuest)
    {
      stringList1 = Utility.GetAllPlayerUnlockedCraftingRecipes();
      stringList2 = Utility.GetAllPlayerUnlockedCookingRecipes();
    }
    if (forQuest && (MineShaft.lowestLevelReached > 40 || Utility.GetAllPlayerReachedBottomOfMines() >= 1) || !forQuest && (Game1.player.deepestMineLevel > 40 || Game1.player.timesReachedMineBottom >= 1))
      options.AddRange((IEnumerable<string>) new string[5]
      {
        "62",
        "70",
        "72",
        "84",
        "422"
      });
    if (forQuest && (MineShaft.lowestLevelReached > 80 /*0x50*/ || Utility.GetAllPlayerReachedBottomOfMines() >= 1) || !forQuest && (Game1.player.deepestMineLevel > 80 /*0x50*/ || Game1.player.timesReachedMineBottom >= 1))
      options.AddRange((IEnumerable<string>) new string[3]
      {
        "64",
        "60",
        "82"
      });
    if (Utility.doesAnyFarmerHaveMail("ccVault"))
      options.AddRange((IEnumerable<string>) new string[4]
      {
        "88",
        "90",
        "164",
        "165"
      });
    if (stringList1.Contains("Furnace"))
      options.AddRange((IEnumerable<string>) new string[4]
      {
        "334",
        "335",
        "336",
        "338"
      });
    if (stringList1.Contains("Quartz Globe"))
      options.Add("339");
    switch (season)
    {
      case Season.Spring:
        options.AddRange((IEnumerable<string>) new string[17]
        {
          "16",
          "18",
          "20",
          "22",
          "129",
          "131",
          "132",
          "136",
          "137",
          "142",
          "143",
          "145",
          "147",
          "148",
          "152",
          "167",
          "267"
        });
        break;
      case Season.Summer:
        options.AddRange((IEnumerable<string>) new string[16 /*0x10*/]
        {
          "128",
          "130",
          "132",
          "136",
          "138",
          "142",
          "144",
          "145",
          "146",
          "149",
          "150",
          "155",
          "396",
          "398",
          "402",
          "267"
        });
        break;
      case Season.Fall:
        options.AddRange((IEnumerable<string>) new string[18]
        {
          "404",
          "406",
          "408",
          "410",
          "129",
          "131",
          "132",
          "136",
          "137",
          "139",
          "140",
          "142",
          "143",
          "148",
          "150",
          "154",
          "155",
          "269"
        });
        break;
      case Season.Winter:
        options.AddRange((IEnumerable<string>) new string[17]
        {
          "412",
          "414",
          "416",
          "418",
          "130",
          "131",
          "132",
          "136",
          "140",
          "141",
          "144",
          "146",
          "147",
          "150",
          "151",
          "154",
          "269"
        });
        break;
    }
    if (forQuest)
    {
      foreach (string key in stringList2)
      {
        if (random1.NextDouble() >= 0.4)
        {
          List<string> stringList3 = Utility.possibleCropsAtThisTime(Game1.season, Game1.dayOfMonth <= 7);
          string str1;
          if (CraftingRecipe.cookingRecipes.TryGetValue(key, out str1))
          {
            string[] array = str1.Split('/');
            string[] strArray = ArgUtility.SplitBySpace(ArgUtility.Get(array, 0));
            bool flag = true;
            for (int index = 0; index < strArray.Length; ++index)
            {
              if (!options.Contains(strArray[index]) && !Utility.isCategoryIngredientAvailable(strArray[index]) && (stringList3 == null || !stringList3.Contains(strArray[index])))
              {
                flag = false;
                break;
              }
            }
            if (flag)
            {
              string str2 = ArgUtility.Get(array, 2);
              if (str2 != null)
                options.Add(str2);
            }
          }
        }
      }
    }
    return random1.ChooseFrom<string>((IList<string>) options);
  }

  public static string getRandomItemFromSeason(
    Season season,
    int randomSeedAddition,
    bool forQuest,
    bool changeDaily = true)
  {
    Random random = Utility.CreateRandom((double) Game1.uniqueIDForThisGame, changeDaily ? (double) Game1.stats.DaysPlayed : 0.0, (double) randomSeedAddition);
    return Utility.getRandomItemFromSeason(season, forQuest, random);
  }

  private static bool isCategoryIngredientAvailable(string category)
  {
    return category != null && category.StartsWith('-') && !(category == "-5") && !(category == "-6");
  }

  public static void farmerHeardSong(string trackName)
  {
    if (string.IsNullOrWhiteSpace(trackName))
      return;
    HashSet<string> songsHeard = Game1.player.songsHeard;
    switch (trackName)
    {
      case "EarthMine":
        songsHeard.Add("Crystal Bells");
        songsHeard.Add("Cavern");
        songsHeard.Add("Secret Gnomes");
        break;
      case "FrostMine":
        songsHeard.Add("Cloth");
        songsHeard.Add("Icicles");
        songsHeard.Add("XOR");
        break;
      case "LavaMine":
        songsHeard.Add("Of Dwarves");
        songsHeard.Add("Near The Planet Core");
        songsHeard.Add("Overcast");
        songsHeard.Add("tribal");
        break;
      case "VolcanoMines":
        songsHeard.Add("VolcanoMines1");
        songsHeard.Add("VolcanoMines2");
        break;
      default:
        if (!(trackName != "none") || !(trackName != "rain") || !(trackName != "silence"))
          break;
        songsHeard.Add(trackName);
        break;
    }
  }

  public static float getMaxedFriendshipPercent(Farmer who = null)
  {
    if (who == null)
      who = Game1.player;
    int num1 = 0;
    int num2 = 0;
    foreach (KeyValuePair<string, CharacterData> keyValuePair in (IEnumerable<KeyValuePair<string, CharacterData>>) Game1.characterData)
    {
      string key = keyValuePair.Key;
      CharacterData characterData = keyValuePair.Value;
      if (characterData.PerfectionScore && !GameStateQuery.IsImmutablyFalse(characterData.CanSocialize))
      {
        ++num2;
        Friendship friendship;
        if (who.friendshipData.TryGetValue(key, out friendship))
        {
          int num3 = (characterData.CanBeRomanced ? 8 : 10) * 250;
          if (friendship != null && friendship.Points >= num3)
            ++num1;
        }
      }
    }
    return (float) num1 / ((float) num2 * 1f);
  }

  public static float getCookedRecipesPercent(Farmer who = null)
  {
    if (who == null)
      who = Game1.player;
    Dictionary<string, string> cookingRecipes = CraftingRecipe.cookingRecipes;
    float num = 0.0f;
    foreach (KeyValuePair<string, string> keyValuePair in cookingRecipes)
    {
      string key1 = keyValuePair.Key;
      if (who.cookingRecipes.ContainsKey(key1))
      {
        string key2 = ArgUtility.SplitBySpaceAndGet(ArgUtility.Get(keyValuePair.Value.Split('/'), 2), 0);
        if (who.recipesCooked.ContainsKey(key2))
          ++num;
      }
    }
    return num / (float) cookingRecipes.Count;
  }

  public static float getCraftedRecipesPercent(Farmer who = null)
  {
    if (who == null)
      who = Game1.player;
    Dictionary<string, string> craftingRecipes = CraftingRecipe.craftingRecipes;
    float num1 = 0.0f;
    foreach (string key in craftingRecipes.Keys)
    {
      int num2;
      if (!(key == "Wedding Ring") && who.craftingRecipes.TryGetValue(key, out num2) && num2 > 0)
        ++num1;
    }
    return num1 / ((float) craftingRecipes.Count - 1f);
  }

  public static float getFishCaughtPercent(Farmer who = null)
  {
    if (who == null)
      who = Game1.player;
    float num1 = 0.0f;
    float num2 = 0.0f;
    foreach (ParsedItemData parsedItemData in ItemRegistry.GetObjectTypeDefinition().GetAllData())
    {
      if (parsedItemData.ObjectType == "Fish" && (!(parsedItemData.RawData is ObjectData rawData) || !rawData.ExcludeFromFishingCollection))
      {
        ++num2;
        if (who.fishCaught.ContainsKey(parsedItemData.QualifiedItemId))
          ++num1;
      }
    }
    return num1 / num2;
  }

  public static KeyValuePair<Farmer, bool> GetFarmCompletion(Func<Farmer, bool> check)
  {
    if (check(Game1.player))
      return new KeyValuePair<Farmer, bool>(Game1.player, true);
    foreach (Farmer allFarmer in Game1.getAllFarmers())
    {
      if (allFarmer != Game1.player && allFarmer.isCustomized.Value && check(allFarmer))
        return new KeyValuePair<Farmer, bool>(allFarmer, true);
    }
    return new KeyValuePair<Farmer, bool>(Game1.player, false);
  }

  public static KeyValuePair<Farmer, float> GetFarmCompletion(Func<Farmer, float> check)
  {
    Farmer key = Game1.player;
    float num1 = check(Game1.player);
    foreach (Farmer allFarmer in Game1.getAllFarmers())
    {
      if (allFarmer != Game1.player && allFarmer.isCustomized.Value)
      {
        float num2 = check(allFarmer);
        if ((double) num2 > (double) num1)
        {
          key = allFarmer;
          num1 = num2;
        }
      }
    }
    return new KeyValuePair<Farmer, float>(key, num1);
  }

  /// <summary>Get the overall perfection score for this save, accounting for all players.</summary>
  /// <remarks>See also <see cref="M:StardewValley.Game1.UpdateFarmPerfection" /> for the overnight changes if perfection was reached.</remarks>
  /// <returns>Returns a number between 0 (no perfection requirements met) and 1 (all requirements met).</returns>
  public static float percentGameComplete()
  {
    float num1 = 0.0f;
    KeyValuePair<Farmer, float> farmCompletion1 = Utility.GetFarmCompletion((Func<Farmer, float>) (farmer => Utility.getFarmerItemsShippedPercent(farmer)));
    double num2 = 0.0 + (double) farmCompletion1.Value * 15.0;
    float num3 = num1 + 15f;
    double num4 = (double) Math.Min((float) Utility.GetObeliskTypesBuilt(), 4f);
    double num5 = num2 + num4;
    float num6 = num3 + 4f;
    double num7 = Game1.IsBuildingConstructed("Gold Clock") ? 10.0 : 0.0;
    double num8 = num5 + num7;
    float num9 = num6 + 10f;
    KeyValuePair<Farmer, bool> farmCompletion2 = Utility.GetFarmCompletion((Func<Farmer, bool>) (farmer => farmer.hasCompletedAllMonsterSlayerQuests.Value));
    double num10 = farmCompletion2.Value ? 10.0 : 0.0;
    double num11 = num8 + num10;
    float num12 = num9 + 10f;
    farmCompletion1 = Utility.GetFarmCompletion((Func<Farmer, float>) (farmer => Utility.getMaxedFriendshipPercent(farmer)));
    double num13 = (double) farmCompletion1.Value * 11.0;
    double num14 = num11 + num13;
    float num15 = num12 + 11f;
    farmCompletion1 = Utility.GetFarmCompletion((Func<Farmer, float>) (farmer => Math.Min((float) farmer.Level, 25f) / 25f));
    double num16 = (double) farmCompletion1.Value * 5.0;
    double num17 = num14 + num16;
    float num18 = num15 + 5f;
    farmCompletion2 = Utility.GetFarmCompletion((Func<Farmer, bool>) (farmer => Utility.foundAllStardrops(farmer)));
    double num19 = farmCompletion2.Value ? 10.0 : 0.0;
    double num20 = num17 + num19;
    float num21 = num18 + 10f;
    farmCompletion1 = Utility.GetFarmCompletion((Func<Farmer, float>) (farmer => Utility.getCookedRecipesPercent(farmer)));
    double num22 = (double) farmCompletion1.Value * 10.0;
    double num23 = num20 + num22;
    float num24 = num21 + 10f;
    farmCompletion1 = Utility.GetFarmCompletion((Func<Farmer, float>) (farmer => Utility.getCraftedRecipesPercent(farmer)));
    double num25 = (double) farmCompletion1.Value * 10.0;
    double num26 = num23 + num25;
    float num27 = num24 + 10f;
    farmCompletion1 = Utility.GetFarmCompletion((Func<Farmer, float>) (farmer => Utility.getFishCaughtPercent(farmer)));
    double num28 = (double) farmCompletion1.Value * 10.0;
    double num29 = num26 + num28;
    float num30 = num27 + 10f;
    float val2 = 130f;
    double num31 = (double) Math.Min((float) Game1.netWorldState.Value.GoldenWalnutsFound, val2) / (double) val2 * 5.0;
    return (float) (num29 + num31) / (num30 + 5f);
  }

  /// <summary>Get the number of unique obelisk building types constructed anywhere in the world.</summary>
  public static int GetObeliskTypesBuilt()
  {
    return (Game1.IsBuildingConstructed("Water Obelisk") ? 1 : 0) + (Game1.IsBuildingConstructed("Earth Obelisk") ? 1 : 0) + (Game1.IsBuildingConstructed("Desert Obelisk") ? 1 : 0) + (Game1.IsBuildingConstructed("Island Obelisk") ? 1 : 0);
  }

  private static int itemsShippedPercent()
  {
    return (int) ((double) Game1.player.basicShipped.Length / 92.0 * 5.0);
  }

  public static int getTrashReclamationPrice(Item i, Farmer f)
  {
    float num = 0.15f * (float) f.trashCanLevel;
    return i.canBeTrashed() && !(i is Wallpaper) && !(i is Furniture) && (i is Object @object && !@object.bigCraftable.Value || i is MeleeWeapon || i is Ring || i is Boots) ? (int) ((double) i.Stack * ((double) i.sellToStorePrice(-1L) * (double) num)) : -1;
  }

  /// <summary>Get the help-wanted quest to show on Pierre's bulletin board today, if any.</summary>
  public static Quest getQuestOfTheDay()
  {
    if (Game1.stats.DaysPlayed <= 1U)
      return (Quest) null;
    double num = Utility.CreateDaySaveRandom(100.0, (double) (Game1.stats.DaysPlayed * 777U)).NextDouble();
    Quest questOfTheDay;
    if (num < 0.08)
      questOfTheDay = (Quest) new ResourceCollectionQuest();
    else if (num < 0.2 && MineShaft.lowestLevelReached > 0 && Game1.stats.DaysPlayed > 5U)
    {
      SlayMonsterQuest slayMonsterQuest = new SlayMonsterQuest();
      slayMonsterQuest.ignoreFarmMonsters.Add(true);
      questOfTheDay = (Quest) slayMonsterQuest;
    }
    else if (num < 0.5)
      questOfTheDay = (Quest) null;
    else if (num < 0.6)
      questOfTheDay = (Quest) new FishingQuest();
    else if (num < 0.66 && Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth).Equals("Mon"))
    {
      bool flag = false;
      foreach (Farmer allFarmer in Game1.getAllFarmers())
      {
        foreach (Quest quest in (NetList<Quest, NetRef<Quest>>) allFarmer.questLog)
        {
          if (quest is SocializeQuest)
          {
            flag = true;
            break;
          }
        }
        if (flag)
          break;
      }
      questOfTheDay = flag ? (Quest) new ItemDeliveryQuest() : (Quest) new SocializeQuest();
    }
    else
      questOfTheDay = (Quest) new ItemDeliveryQuest();
    return questOfTheDay;
  }

  /// <summary>Get a MonoGame color from a string representation.</summary>
  /// <param name="rawColor">The raw color value to parse. This can be a <see cref="T:Microsoft.Xna.Framework.Color" /> property name (like <c>SkyBlue</c>), RGB or RGBA hex code (like <c>#AABBCC</c> or <c>#AABBCCDD</c>), or 8-bit RGB or RGBA code (like <c>34 139 34</c> or <c>34 139 34 255</c>).</param>
  /// <returns>Returns the matching color (if any), else <c>null</c>.</returns>
  public static Color? StringToColor(string rawColor)
  {
    rawColor = rawColor?.Trim();
    if (string.IsNullOrEmpty(rawColor))
      return new Color?();
    if (rawColor.StartsWith('#'))
    {
      byte result1 = byte.MaxValue;
      byte result2;
      byte result3;
      byte result4;
      if ((rawColor.Length == 7 || rawColor.Length == 9) && byte.TryParse(rawColor.Substring(1, 2), NumberStyles.HexNumber, (IFormatProvider) null, out result2) && byte.TryParse(rawColor.Substring(3, 2), NumberStyles.HexNumber, (IFormatProvider) null, out result3) && byte.TryParse(rawColor.Substring(5, 2), NumberStyles.HexNumber, (IFormatProvider) null, out result4) && (rawColor.Length == 7 || byte.TryParse(rawColor.Substring(7, 2), NumberStyles.HexNumber, (IFormatProvider) null, out result1)))
        return new Color?(new Color(result2, result3, result4, result1));
    }
    else if (rawColor.Contains(' '))
    {
      string[] array = ArgUtility.SplitBySpace(rawColor);
      int r;
      string error;
      int g;
      int b;
      int alpha;
      if ((array.Length == 3 || array.Length == 4) && ArgUtility.TryGetInt(array, 0, out r, out error, "int red") && ArgUtility.TryGetInt(array, 1, out g, out error, "int green") && ArgUtility.TryGetInt(array, 2, out b, out error, "int blue") && ArgUtility.TryGetOptionalInt(array, 3, out alpha, out error, (int) byte.MaxValue, "int alpha"))
        return new Color?(new Color(r, g, b, alpha));
    }
    else
    {
      PropertyInfo property = typeof (Color).GetProperty(rawColor, BindingFlags.IgnoreCase | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
      if (property != (PropertyInfo) null)
        return new Color?((Color) property.GetValue((object) null, (object[]) null));
    }
    Game1.log.Warn($"Can't parse '{rawColor}' as a color because it's not a hexadecimal code, RGB code, or color name.");
    return new Color?();
  }

  public static Color getOppositeColor(Color color)
  {
    return new Color((int) byte.MaxValue - (int) color.R, (int) byte.MaxValue - (int) color.G, (int) byte.MaxValue - (int) color.B);
  }

  public static void drawLightningBolt(Vector2 strikePosition, GameLocation l)
  {
    Microsoft.Xna.Framework.Rectangle sourceRect = new Microsoft.Xna.Framework.Rectangle(644, 1078, 37, 57);
    for (Vector2 position = strikePosition + new Vector2((float) (-sourceRect.Width * 4 / 2), (float) (-sourceRect.Height * 4)); (double) position.Y > (double) (-sourceRect.Height * 4); position.Y -= (float) (sourceRect.Height * 4))
    {
      TemporaryAnimatedSpriteList temporarySprites = l.temporarySprites;
      TemporaryAnimatedSprite temporaryAnimatedSprite = new TemporaryAnimatedSprite("LooseSprites\\Cursors", sourceRect, 9999f, 1, 999, position, false, Game1.random.NextBool(), (float) (((double) strikePosition.Y + 32.0) / 10000.0 + 1.0 / 1000.0), 0.025f, Color.White, 4f, 0.0f, 0.0f, 0.0f);
      temporaryAnimatedSprite.lightId = $"{l.NameOrUniqueName}_LightningBolt_{strikePosition.X}_{strikePosition.Y}_{Game1.random.Next()}";
      temporaryAnimatedSprite.lightRadius = 2f;
      temporaryAnimatedSprite.delayBeforeAnimationStart = 200;
      temporaryAnimatedSprite.lightcolor = Color.Black;
      temporarySprites.Add(temporaryAnimatedSprite);
    }
  }

  /// <summary>Get a translated display text for a calendar date.</summary>
  /// <param name="day">The calendar day of month.</param>
  /// <param name="season">The calendar season.</param>
  /// <param name="year">The calendar year.</param>
  public static string getDateStringFor(int day, int season, int year)
  {
    if (day <= 0)
    {
      day += 28;
      --season;
      if (season < 0)
      {
        season = 3;
        --year;
      }
    }
    else if (day > 28)
    {
      day -= 28;
      ++season;
      if (season > 3)
      {
        season = 0;
        ++year;
      }
    }
    return year == 0 ? Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5677") : Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5678", (object) day, LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.es ? (object) Utility.getSeasonNameFromNumber(season).ToLower() : (object) Utility.getSeasonNameFromNumber(season), (object) year);
  }

  public static string getDateString(int offset = 0)
  {
    int dayOfMonth = Game1.dayOfMonth;
    int seasonIndex = Game1.seasonIndex;
    int year = Game1.year;
    int num = offset;
    return Utility.getDateStringFor(dayOfMonth + num, seasonIndex, year);
  }

  public static string getYesterdaysDate() => Utility.getDateString(-1);

  public static string getSeasonNameFromNumber(int number)
  {
    switch (number)
    {
      case 0:
        return Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5680");
      case 1:
        return Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5681");
      case 2:
        return Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5682");
      case 3:
        return Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5683");
      default:
        return "";
    }
  }

  public static string getNumberEnding(int number)
  {
    if (number % 100 > 10 && number % 100 < 20)
      return "th";
    switch (number % 10)
    {
      case 0:
      case 4:
      case 5:
      case 6:
      case 7:
      case 8:
      case 9:
        return "th";
      case 1:
        return "st";
      case 2:
        return "nd";
      case 3:
        return "rd";
      default:
        return "";
    }
  }

  public static void killAllStaticLoopingSoundCues()
  {
    Intro.roadNoise?.Stop(AudioStopOptions.Immediate);
    Fly.buzz?.Stop(AudioStopOptions.Immediate);
    Railroad.trainLoop?.Stop(AudioStopOptions.Immediate);
    BobberBar.reelSound?.Stop(AudioStopOptions.Immediate);
    BobberBar.unReelSound?.Stop(AudioStopOptions.Immediate);
    FishingRod.reelSound?.Stop(AudioStopOptions.Immediate);
    Game1.loopingLocationCues.StopAll();
  }

  public static void consolidateStacks(IList<Item> objects)
  {
    for (int index1 = 0; index1 < objects.Count; ++index1)
    {
      if (objects[index1] is Object otherStack)
      {
        for (int index2 = index1 + 1; index2 < objects.Count; ++index2)
        {
          if (objects[index2] != null && otherStack.canStackWith((ISalable) objects[index2]))
          {
            int amount = otherStack.Stack - objects[index2].addToStack((Item) otherStack);
            if (otherStack.ConsumeStack(amount) == null)
            {
              objects[index1] = (Item) null;
              break;
            }
          }
        }
      }
    }
  }

  public static void performLightningUpdate(int time_of_day)
  {
    Random random = Utility.CreateRandom((double) Game1.uniqueIDForThisGame, (double) Game1.stats.DaysPlayed, (double) time_of_day);
    if (random.NextDouble() < 0.125 + Game1.player.team.AverageDailyLuck() + Game1.player.team.AverageLuckLevel() / 100.0)
    {
      Farm.LightningStrikeEvent lightningStrikeEvent = new Farm.LightningStrikeEvent();
      lightningStrikeEvent.bigFlash = true;
      Farm farm = Game1.getFarm();
      List<Vector2> options = new List<Vector2>();
      foreach (KeyValuePair<Vector2, Object> pair in farm.objects.Pairs)
      {
        if (pair.Value.QualifiedItemId == "(BC)9")
          options.Add(pair.Key);
      }
      if (options.Count > 0)
      {
        for (int index = 0; index < 2; ++index)
        {
          Vector2 key = random.ChooseFrom<Vector2>((IList<Vector2>) options);
          if (farm.objects[key].heldObject.Value == null)
          {
            farm.objects[key].heldObject.Value = ItemRegistry.Create<Object>("(O)787");
            farm.objects[key].minutesUntilReady.Value = Utility.CalculateMinutesUntilMorning(Game1.timeOfDay);
            farm.objects[key].shakeTimer = 1000;
            lightningStrikeEvent.createBolt = true;
            lightningStrikeEvent.boltPosition = key * 64f + new Vector2(32f, 0.0f);
            farm.lightningStrikeEvent.Fire(lightningStrikeEvent);
            return;
          }
        }
      }
      if (random.NextDouble() < 0.25 - Game1.player.team.AverageDailyLuck() - Game1.player.team.AverageLuckLevel() / 100.0)
      {
        try
        {
          Vector2 key;
          TerrainFeature terrainFeature;
          if (Utility.TryGetRandom<Vector2, TerrainFeature, NetRef<TerrainFeature>, SerializableDictionary<Vector2, TerrainFeature>, NetVector2Dictionary<TerrainFeature, NetRef<TerrainFeature>>>((NetDictionary<Vector2, TerrainFeature, NetRef<TerrainFeature>, SerializableDictionary<Vector2, TerrainFeature>, NetVector2Dictionary<TerrainFeature, NetRef<TerrainFeature>>>) farm.terrainFeatures, out key, out terrainFeature))
          {
            Crop crop1;
            switch (terrainFeature)
            {
              case FruitTree fruitTree:
                fruitTree.struckByLightningCountdown.Value = 4;
                fruitTree.shake(key, true);
                lightningStrikeEvent.createBolt = true;
                lightningStrikeEvent.boltPosition = key * 64f + new Vector2(32f, (float) sbyte.MinValue);
                goto label_28;
              case HoeDirt hoeDirt:
                crop1 = hoeDirt.crop;
                break;
              default:
                crop1 = (Crop) null;
                break;
            }
            Crop crop2 = crop1;
            int num = crop2 == null ? 0 : (!crop2.dead.Value ? 1 : 0);
            if (terrainFeature.performToolAction((Tool) null, 50, key))
            {
              lightningStrikeEvent.destroyedTerrainFeature = true;
              lightningStrikeEvent.createBolt = true;
              farm.terrainFeatures.Remove(key);
              lightningStrikeEvent.boltPosition = key * 64f + new Vector2(32f, (float) sbyte.MinValue);
            }
            if (num != 0)
            {
              if (crop2.dead.Value)
              {
                lightningStrikeEvent.createBolt = true;
                lightningStrikeEvent.boltPosition = key * 64f + new Vector2(32f, 0.0f);
              }
            }
          }
        }
        catch (Exception ex)
        {
        }
      }
label_28:
      farm.lightningStrikeEvent.Fire(lightningStrikeEvent);
    }
    else
    {
      if (random.NextDouble() >= 0.1)
        return;
      Game1.getFarm().lightningStrikeEvent.Fire(new Farm.LightningStrikeEvent()
      {
        smallFlash = true
      });
    }
  }

  /// <summary>Apply overnight lightning strikes after the player goes to sleep.</summary>
  /// <param name="timeWentToSleep">The time of day when the player went to sleep, in 26-hour format.</param>
  public static void overnightLightning(int timeWentToSleep)
  {
    if (!Game1.IsMasterGame)
      return;
    int num = (2300 - timeWentToSleep) / 100;
    for (int index = 1; index <= num; ++index)
      Utility.performLightningUpdate(timeWentToSleep + index * 100);
  }

  public static List<Vector2> getAdjacentTileLocations(Vector2 tileLocation)
  {
    return new List<Vector2>()
    {
      new Vector2(-1f, 0.0f) + tileLocation,
      new Vector2(1f, 0.0f) + tileLocation,
      new Vector2(0.0f, 1f) + tileLocation,
      new Vector2(0.0f, -1f) + tileLocation
    };
  }

  public static Vector2[] getAdjacentTileLocationsArray(Vector2 tileLocation)
  {
    return new Vector2[4]
    {
      new Vector2(-1f, 0.0f) + tileLocation,
      new Vector2(1f, 0.0f) + tileLocation,
      new Vector2(0.0f, 1f) + tileLocation,
      new Vector2(0.0f, -1f) + tileLocation
    };
  }

  public static Vector2[] getSurroundingTileLocationsArray(Vector2 tileLocation)
  {
    return new Vector2[8]
    {
      new Vector2(-1f, 0.0f) + tileLocation,
      new Vector2(1f, 0.0f) + tileLocation,
      new Vector2(0.0f, 1f) + tileLocation,
      new Vector2(0.0f, -1f) + tileLocation,
      new Vector2(-1f, -1f) + tileLocation,
      new Vector2(1f, -1f) + tileLocation,
      new Vector2(1f, 1f) + tileLocation,
      new Vector2(-1f, 1f) + tileLocation
    };
  }

  public static Crop findCloseFlower(
    GameLocation location,
    Vector2 startTileLocation,
    int range = -1,
    Func<Crop, bool> additional_check = null)
  {
    Queue<Vector2> vector2Queue = new Queue<Vector2>();
    HashSet<Vector2> vector2Set = new HashSet<Vector2>();
    vector2Queue.Enqueue(startTileLocation);
    for (int index = 0; (range >= 0 || range < 0 && index <= 150) && vector2Queue.Count > 0; ++index)
    {
      Vector2 vector2 = vector2Queue.Dequeue();
      HoeDirt hoeDirtAtTile = location.GetHoeDirtAtTile(vector2);
      if (hoeDirtAtTile?.crop != null)
      {
        ParsedItemData data = ItemRegistry.GetData(hoeDirtAtTile.crop.indexOfHarvest.Value);
        if ((data != null ? (data.Category == -80 ? 1 : 0) : 0) != 0 && hoeDirtAtTile.crop.currentPhase.Value >= hoeDirtAtTile.crop.phaseDays.Count - 1 && !hoeDirtAtTile.crop.dead.Value && (additional_check == null || additional_check(hoeDirtAtTile.crop)))
          return hoeDirtAtTile.crop;
      }
      foreach (Vector2 adjacentTileLocation in Utility.getAdjacentTileLocations(vector2))
      {
        if (!vector2Set.Contains(adjacentTileLocation) && (range < 0 || (double) Math.Abs(adjacentTileLocation.X - startTileLocation.X) + (double) Math.Abs(adjacentTileLocation.Y - startTileLocation.Y) <= (double) range))
          vector2Queue.Enqueue(adjacentTileLocation);
      }
      vector2Set.Add(vector2);
    }
    return (Crop) null;
  }

  public static void recursiveFenceBuild(
    Vector2 position,
    int direction,
    GameLocation location,
    Random r)
  {
    if (r.NextDouble() < 0.04 || location.objects.ContainsKey(position) || !location.isTileLocationOpen(new Location((int) position.X, (int) position.Y)))
      return;
    location.objects.Add(position, (Object) new Fence(position, "322", false));
    int direction1 = direction;
    if (r.NextDouble() < 0.16)
      direction1 = r.Next(4);
    if (direction1 == (direction + 2) % 4)
      direction1 = (direction1 + 1) % 4;
    switch (direction)
    {
      case 0:
        Utility.recursiveFenceBuild(position + new Vector2(0.0f, -1f), direction1, location, r);
        break;
      case 1:
        Utility.recursiveFenceBuild(position + new Vector2(1f, 0.0f), direction1, location, r);
        break;
      case 2:
        Utility.recursiveFenceBuild(position + new Vector2(0.0f, 1f), direction1, location, r);
        break;
      case 3:
        Utility.recursiveFenceBuild(position + new Vector2(-1f, 0.0f), direction1, location, r);
        break;
    }
  }

  public static bool addAnimalToFarm(FarmAnimal animal)
  {
    if (animal?.Sprite == null)
      return false;
    foreach (Building building in Game1.currentLocation.buildings)
    {
      if (animal.CanLiveIn(building) && building.GetIndoors() is AnimalHouse indoors && !indoors.isFull())
      {
        indoors.adoptAnimal(animal);
        return true;
      }
    }
    return false;
  }

  /// <summary>
  /// "Standard" description is as follows:
  /// (Item type [Object (O), BigObject (BO), Weapon (W), Ring (R), Hat (H), Boot (B), Blueprint (BL), Big Object Blueprint(BBL)], follwed by item index, then stack amount)
  /// </summary>
  /// <returns>the described Item object</returns>
  [Obsolete("This is only intended for backwards compatibility with older data. Most code should use ItemRegistry instead.")]
  public static Item getItemFromStandardTextDescription(
    string description,
    Farmer who,
    char delimiter = ' ')
  {
    string[] strArray = description.Split(delimiter);
    return Utility.getItemFromStandardTextDescription(strArray[0], strArray[1], Convert.ToInt32(strArray[2]), who);
  }

  /// <summary>
  /// "Standard" description is as follows:
  /// (Item type [Object (O), BigObject (BO), Weapon (W), Ring (R), Hat (H), Boot (B), Blueprint (BL), Big Object Blueprint(BBL)], follwed by item index, then stack amount)
  /// </summary>
  /// <returns>the described Item object</returns>
  [Obsolete("This is only intended for backwards compatibility with older data. Most code should use ItemRegistry instead.")]
  public static Item getItemFromStandardTextDescription(
    string type,
    string itemId,
    int stock,
    Farmer who)
  {
    Item obj = (Item) null;
    if (type != null)
    {
      switch (type.Length)
      {
        case 1:
          switch (type[0])
          {
            case 'B':
              goto label_24;
            case 'C':
              int result;
              obj = int.TryParse(itemId, out result) ? ItemRegistry.Create((result >= 1000 ? "(S)" : "(P)") + itemId) : ItemRegistry.Create(itemId);
              goto label_30;
            case 'F':
              break;
            case 'H':
              goto label_27;
            case 'O':
            case 'R':
              goto label_22;
            case 'W':
              goto label_25;
            default:
              goto label_30;
          }
          break;
        case 2:
          switch (type[1])
          {
            case 'L':
              if (type == "BL")
                goto label_26;
              goto label_30;
            case 'O':
              if (type == "BO")
                goto label_23;
              goto label_30;
            default:
              goto label_30;
          }
        case 3:
          switch (type[2])
          {
            case 'L':
              if (type == "BBL")
                goto label_28;
              goto label_30;
            case 'l':
              if (type == "BBl")
                goto label_28;
              goto label_30;
            case 't':
              if (type == "Hat")
                goto label_27;
              goto label_30;
            default:
              goto label_30;
          }
        case 4:
          switch (type[0])
          {
            case 'B':
              if (type == "Boot")
                goto label_24;
              goto label_30;
            case 'R':
              if (type == "Ring")
                goto label_22;
              goto label_30;
            default:
              goto label_30;
          }
        case 6:
          switch (type[0])
          {
            case 'O':
              if (type == "Object")
                goto label_22;
              goto label_30;
            case 'W':
              if (type == "Weapon")
                goto label_25;
              goto label_30;
            default:
              goto label_30;
          }
        case 9:
          switch (type[1])
          {
            case 'i':
              if (type == "BigObject")
                goto label_23;
              goto label_30;
            case 'l':
              if (type == "Blueprint")
                goto label_26;
              goto label_30;
            case 'u':
              if (type == "Furniture")
                break;
              goto label_30;
            default:
              goto label_30;
          }
          break;
        case 12:
          if (type == "BigBlueprint")
            goto label_28;
          goto label_30;
        default:
          goto label_30;
      }
      obj = ItemRegistry.Create("(F)" + itemId);
      goto label_30;
label_22:
      obj = ItemRegistry.Create("(O)" + itemId);
      goto label_30;
label_23:
      obj = ItemRegistry.Create("(BC)" + itemId);
      goto label_30;
label_24:
      obj = ItemRegistry.Create("(B)" + itemId);
      goto label_30;
label_25:
      obj = ItemRegistry.Create("(W)" + itemId);
      goto label_30;
label_26:
      obj = ItemRegistry.Create("(O)" + itemId);
      obj.IsRecipe = true;
      goto label_30;
label_27:
      obj = ItemRegistry.Create("(H)" + itemId);
      goto label_30;
label_28:
      obj = ItemRegistry.Create("(BC)" + itemId);
      obj.IsRecipe = true;
    }
label_30:
    obj.Stack = stock;
    return who != null && obj.IsRecipe && who.knowsRecipe(obj.Name) ? (Item) null : obj;
  }

  [Obsolete("This is only intended for backwards compatibility with older data. Most code should use ItemRegistry instead.")]
  public static string getStandardDescriptionFromItem(Item item, int stack, char delimiter = ' ')
  {
    return Utility.getStandardDescriptionFromItem(item.TypeDefinitionId, item.ItemId, item.isRecipe.Value, item is Ring, stack, delimiter);
  }

  [Obsolete("This is only intended for backwards compatibility with older data. Most code should use ItemRegistry instead.")]
  public static string getStandardDescriptionFromItem(
    string typeDefinitionId,
    string itemId,
    bool isRecipe,
    bool isRing,
    int stack,
    char delimiter = ' ')
  {
    string str;
    if (typeDefinitionId != null)
    {
      switch (typeDefinitionId.Length)
      {
        case 3:
          switch (typeDefinitionId[1])
          {
            case 'B':
              if (typeDefinitionId == "(B)")
              {
                str = "B";
                goto label_19;
              }
              goto label_18;
            case 'F':
              if (typeDefinitionId == "(F)")
              {
                str = "F";
                goto label_19;
              }
              goto label_18;
            case 'H':
              if (typeDefinitionId == "(H)")
              {
                str = "H";
                goto label_19;
              }
              goto label_18;
            case 'O':
              if (typeDefinitionId == "(O)")
              {
                str = !isRing ? (isRecipe ? "BL" : "O") : "R";
                goto label_19;
              }
              goto label_18;
            case 'P':
              if (typeDefinitionId == "(P)")
                break;
              goto label_18;
            case 'S':
              if (typeDefinitionId == "(S)")
                break;
              goto label_18;
            case 'W':
              if (typeDefinitionId == "(W)")
              {
                str = "W";
                goto label_19;
              }
              goto label_18;
            default:
              goto label_18;
          }
          str = "C";
          goto label_19;
        case 4:
          if (typeDefinitionId == "(BC)")
          {
            str = isRecipe ? "BBL" : "BO";
            goto label_19;
          }
          break;
      }
    }
label_18:
    str = "";
label_19:
    return str + delimiter.ToString() + itemId + delimiter.ToString() + stack.ToString();
  }

  public static TemporaryAnimatedSpriteList sparkleWithinArea(
    Microsoft.Xna.Framework.Rectangle bounds,
    int numberOfSparkles,
    Color sparkleColor,
    int delayBetweenSparkles = 100,
    int delayBeforeStarting = 0,
    string sparkleSound = "")
  {
    return Utility.getTemporarySpritesWithinArea(new int[2]
    {
      10,
      11
    }, bounds, numberOfSparkles, sparkleColor, delayBetweenSparkles, delayBeforeStarting, sparkleSound);
  }

  public static TemporaryAnimatedSpriteList getTemporarySpritesWithinArea(
    int[] temporarySpriteRowNumbers,
    Microsoft.Xna.Framework.Rectangle bounds,
    int numberOfsprites,
    Color color,
    int delayBetweenSprites = 100,
    int delayBeforeStarting = 0,
    string sound = "")
  {
    TemporaryAnimatedSpriteList spritesWithinArea = new TemporaryAnimatedSpriteList();
    for (int index = 0; index < numberOfsprites; ++index)
      spritesWithinArea.Add(new TemporaryAnimatedSprite(Game1.random.Choose<int>(temporarySpriteRowNumbers), new Vector2((float) Game1.random.Next(bounds.X, bounds.Right), (float) Game1.random.Next(bounds.Y, bounds.Bottom)), color)
      {
        delayBeforeAnimationStart = delayBeforeStarting + delayBetweenSprites * index,
        startSound = sound.Length > 0 ? sound : (string) null
      });
    return spritesWithinArea;
  }

  public static Vector2 getAwayFromPlayerTrajectory(Microsoft.Xna.Framework.Rectangle monsterBox, Farmer who)
  {
    Point center = monsterBox.Center;
    Point standingPixel = who.StandingPixel;
    Vector2 playerTrajectory = new Vector2((float) -(standingPixel.X - center.X), (float) (standingPixel.Y - center.Y));
    if ((double) playerTrajectory.Length() <= 0.0)
    {
      switch (who.FacingDirection)
      {
        case 0:
          playerTrajectory = new Vector2(0.0f, 1f);
          break;
        case 1:
          playerTrajectory = new Vector2(1f, 0.0f);
          break;
        case 2:
          playerTrajectory = new Vector2(0.0f, -1f);
          break;
        case 3:
          playerTrajectory = new Vector2(-1f, 0.0f);
          break;
      }
    }
    playerTrajectory.Normalize();
    playerTrajectory.X *= (float) (50 + Game1.random.Next(-20, 20));
    playerTrajectory.Y *= (float) (50 + Game1.random.Next(-20, 20));
    return playerTrajectory;
  }

  /// <summary>Get the cue names that can be played from a jukebox for the current player.</summary>
  /// <param name="player">The player for whom to get music.</param>
  /// <param name="location">The location for whom to get music.</param>
  /// <remarks>See also <see cref="M:StardewValley.Utility.getSongTitleFromCueName(System.String)" />.</remarks>
  public static List<string> GetJukeboxTracks(Farmer player, GameLocation location)
  {
    Dictionary<string, string> dictionary = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    foreach (KeyValuePair<string, JukeboxTrackData> keyValuePair in (IEnumerable<KeyValuePair<string, JukeboxTrackData>>) Game1.jukeboxTrackData)
    {
      List<string> alternativeTrackIds = keyValuePair.Value.AlternativeTrackIds;
      // ISSUE: explicit non-virtual call
      if ((alternativeTrackIds != null ? (__nonvirtual (alternativeTrackIds.Count) > 0 ? 1 : 0) : 0) != 0)
      {
        foreach (string alternativeTrackId in keyValuePair.Value.AlternativeTrackIds)
        {
          if (alternativeTrackId != null)
            dictionary[alternativeTrackId] = keyValuePair.Key;
        }
      }
    }
    List<string> jukeboxTracks = new List<string>();
    HashSet<string> stringSet = new HashSet<string>();
    foreach (KeyValuePair<string, JukeboxTrackData> keyValuePair in (IEnumerable<KeyValuePair<string, JukeboxTrackData>>) Game1.jukeboxTrackData)
    {
      bool? available = keyValuePair.Value.Available;
      if (available.HasValue && available.GetValueOrDefault())
      {
        jukeboxTracks.Add(keyValuePair.Key);
        stringSet.Add(keyValuePair.Key);
      }
    }
    foreach (string key in player.songsHeard)
    {
      string str = dictionary.GetValueOrDefault<string, string>(key) ?? key;
      if (Utility.IsValidTrackName(str) && stringSet.Add(str))
      {
        JukeboxTrackData jukeboxTrackData;
        if (Game1.jukeboxTrackData.TryGetValue(str, out jukeboxTrackData))
        {
          bool? available = jukeboxTrackData.Available;
          if (available.HasValue && !available.GetValueOrDefault())
            continue;
        }
        jukeboxTracks.Add(str);
      }
    }
    return jukeboxTracks;
  }

  /// <summary>Get whether an audio cue name is valid for the jukebox, regardless of whether it's disabled in <see cref="F:StardewValley.Game1.jukeboxTrackData" />.</summary>
  /// <param name="name">The audio cue name to check.</param>
  /// <remarks>This only checks whether the cue *could* be played by the jukebox. To check whether it's actually available, see <see cref="M:StardewValley.Utility.GetJukeboxTracks(StardewValley.Farmer,StardewValley.GameLocation)" />.</remarks>
  public static bool IsValidTrackName(string name)
  {
    if (string.IsNullOrWhiteSpace(name))
      return false;
    string lower = name.ToLower();
    return !lower.Contains("ambience") && !lower.Contains("ambient") && !lower.Contains("bigdrums") && !lower.Contains("clubloop") && Game1.soundBank.Exists(name);
  }

  /// <summary>Get the jukebox display name for a cue name.</summary>
  /// <param name="cueName">The cue name being played.</param>
  /// <remarks>See also <see cref="M:StardewValley.Utility.GetJukeboxTracks(StardewValley.Farmer,StardewValley.GameLocation)" />.</remarks>
  public static string getSongTitleFromCueName(string cueName)
  {
    if (!string.IsNullOrWhiteSpace(cueName))
    {
      switch (cueName.ToLowerInvariant())
      {
        case "turn_off":
          return Game1.content.LoadString("Strings\\UI:Mini_JukeBox_Off");
        case "random":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:JukeboxRandomTrack");
        default:
          JukeboxTrackData jukeboxTrackData;
          if (Game1.jukeboxTrackData.TryGetValue(cueName, out jukeboxTrackData))
            return TokenParser.ParseText(jukeboxTrackData.Name) ?? cueName;
          using (IEnumerator<JukeboxTrackData> enumerator = Game1.jukeboxTrackData.Values.GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              JukeboxTrackData current = enumerator.Current;
              List<string> alternativeTrackIds = current.AlternativeTrackIds;
              bool? nullable = alternativeTrackIds != null ? new bool?(alternativeTrackIds.Contains<string>(cueName, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)) : new bool?();
              if (nullable.HasValue && nullable.GetValueOrDefault())
                return TokenParser.ParseText(current.Name) ?? cueName;
            }
            break;
          }
      }
    }
    return cueName;
  }

  public static bool isOffScreenEndFunction(
    PathNode currentNode,
    Point endPoint,
    GameLocation location,
    Character c)
  {
    return !Utility.isOnScreen(new Vector2((float) (currentNode.x * 64 /*0x40*/), (float) (currentNode.y * 64 /*0x40*/)), 32 /*0x20*/);
  }

  public static Vector2 getAwayFromPositionTrajectory(Microsoft.Xna.Framework.Rectangle monsterBox, Vector2 position)
  {
    double num1 = -((double) position.X - (double) monsterBox.Center.X);
    float num2 = position.Y - (float) monsterBox.Center.Y;
    float num3 = Math.Abs((float) num1) + Math.Abs(num2);
    if ((double) num3 < 1.0)
      num3 = 5f;
    return new Vector2((float) (num1 / (double) num3 * 20.0), (float) ((double) num2 / (double) num3 * 20.0));
  }

  public static bool tileWithinRadiusOfPlayer(int xTile, int yTile, int tileRadius, Farmer f)
  {
    Point point = new Point(xTile, yTile);
    Vector2 tile = f.Tile;
    return (double) Math.Abs((float) point.X - tile.X) <= (double) tileRadius && (double) Math.Abs((float) point.Y - tile.Y) <= (double) tileRadius;
  }

  public static bool withinRadiusOfPlayer(int x, int y, int tileRadius, Farmer f)
  {
    Point point = new Point(x / 64 /*0x40*/, y / 64 /*0x40*/);
    Vector2 tile = f.Tile;
    return (double) Math.Abs((float) point.X - tile.X) <= (double) tileRadius && (double) Math.Abs((float) point.Y - tile.Y) <= (double) tileRadius;
  }

  public static bool isThereAnObjectHereWhichAcceptsThisItem(
    GameLocation location,
    Item item,
    int x,
    int y)
  {
    if (item is Tool)
      return false;
    Vector2 vector2 = new Vector2((float) (x / 64 /*0x40*/), (float) (y / 64 /*0x40*/));
    foreach (Building building in location.buildings)
    {
      if (building.occupiesTile(vector2) && building.performActiveObjectDropInAction(Game1.player, true))
        return true;
    }
    Object @object;
    return location.Objects.TryGetValue(vector2, out @object) && @object.heldObject.Value == null && @object.performObjectDropInAction(item, true, Game1.player);
  }

  public static FarmAnimal getAnimal(long id)
  {
    FarmAnimal match = (FarmAnimal) null;
    Utility.ForEachLocation((Func<GameLocation, bool>) (location =>
    {
      FarmAnimal farmAnimal;
      if (!location.animals.TryGetValue(id, out farmAnimal))
        return true;
      match = farmAnimal;
      return false;
    }));
    return match;
  }

  public static bool isWallpaperOffLimitsForSale(string index) => index.StartsWith("MoreWalls");

  public static bool isFlooringOffLimitsForSale(string index) => false;

  /// <summary>Open a menu to buy items from a shop, if it exists, using the specified NPC regardless of whether they're present.</summary>
  /// <param name="shopId">The shop ID matching the entry in <c>Data/Shops</c>.</param>
  /// <param name="ownerName">The internal name of the NPC running the shop, or <c>null</c> to open the shop with no NPC portrait/dialogue.</param>
  /// <param name="playOpenSound">Whether to play the open-menu sound.</param>
  /// <returns>Returns whether the shop menu was opened.</returns>
  public static bool TryOpenShopMenu(string shopId, string ownerName, bool playOpenSound = true)
  {
    ShopData shopData;
    if (!DataLoader.Shops(Game1.content).TryGetValue(shopId, out shopData))
      return false;
    ShopOwnerType ownerType;
    if (!Utility.TryParseEnum<ShopOwnerType>(ownerName, out ownerType))
      ownerType = ShopOwnerType.NamedNpc;
    ShopOwnerData[] array = ShopBuilder.GetCurrentOwners(shopData).ToArray<ShopOwnerData>();
    NPC owner;
    ShopOwnerData ownerData;
    switch (ownerType)
    {
      case ShopOwnerType.Any:
        owner = (NPC) null;
        ownerData = ((IEnumerable<ShopOwnerData>) array).FirstOrDefault<ShopOwnerData>((Func<ShopOwnerData, bool>) (p => p.Type == ownerType)) ?? ((IEnumerable<ShopOwnerData>) array).FirstOrDefault<ShopOwnerData>((Func<ShopOwnerData, bool>) (p => p.Type != ShopOwnerType.None));
        break;
      case ShopOwnerType.AnyOrNone:
        owner = (NPC) null;
        ownerData = ((IEnumerable<ShopOwnerData>) array).FirstOrDefault<ShopOwnerData>((Func<ShopOwnerData, bool>) (p => p.Type == ownerType)) ?? ((IEnumerable<ShopOwnerData>) array).FirstOrDefault<ShopOwnerData>();
        break;
      case ShopOwnerType.None:
        owner = (NPC) null;
        ownerData = ((IEnumerable<ShopOwnerData>) array).FirstOrDefault<ShopOwnerData>((Func<ShopOwnerData, bool>) (p => p.Type == ownerType)) ?? ((IEnumerable<ShopOwnerData>) array).FirstOrDefault<ShopOwnerData>((Func<ShopOwnerData, bool>) (p => p.Type == ShopOwnerType.AnyOrNone));
        break;
      default:
        if (ownerName == null)
        {
          owner = (NPC) null;
          ownerData = ((IEnumerable<ShopOwnerData>) array).FirstOrDefault<ShopOwnerData>((Func<ShopOwnerData, bool>) (p => p.Type == ShopOwnerType.AnyOrNone || p.Type == ShopOwnerType.None));
          break;
        }
        owner = Game1.getCharacterFromName(ownerName);
        ownerData = ((IEnumerable<ShopOwnerData>) array).OrderByDescending<ShopOwnerData, bool>((Func<ShopOwnerData, bool>) (p => p.Type == ShopOwnerType.NamedNpc)).ThenByDescending<ShopOwnerData, bool>((Func<ShopOwnerData, bool>) (p => p.Type != ShopOwnerType.None)).FirstOrDefault<ShopOwnerData>((Func<ShopOwnerData, bool>) (p => p.IsValid(ownerName)));
        break;
    }
    Game1.activeClickableMenu = (IClickableMenu) new ShopMenu(shopId, shopData, ownerData, owner, playOpenSound: playOpenSound);
    return true;
  }

  /// <summary>Open a menu to buy items from a shop, if it exists and an NPC who can run it is within the specified range.</summary>
  /// <param name="shopId">The shop ID matching the entry in <c>Data/Shops</c>.</param>
  /// <param name="location">The location in which to open the shop menu.</param>
  /// <param name="ownerArea">The tile area to search for an NPC who can run the shop (or <c>null</c> to search the entire location). If no NPC within the area matches the shop's <see cref="F:StardewValley.GameData.Shops.ShopData.Owners" />, the shop won't be opened (unless <paramref name="forceOpen" /> is <c>true</c>).</param>
  /// <param name="maxOwnerY">The maximum Y tile position for an owner NPC, or <c>null</c> for no maximum. This is used for shops that only work if the NPC is behind the counter.</param>
  /// <param name="forceOpen">Whether to open the menu regardless of whether an owner NPC was found.</param>
  /// <param name="playOpenSound">Whether to play the open-menu sound.</param>
  /// <param name="showClosedMessage">Custom logic to handle the closed message if it shouldn't be shown directly.</param>
  /// <returns>Returns whether the shop menu was opened.</returns>
  public static bool TryOpenShopMenu(
    string shopId,
    GameLocation location,
    Microsoft.Xna.Framework.Rectangle? ownerArea = null,
    int? maxOwnerY = null,
    bool forceOpen = false,
    bool playOpenSound = true,
    Action<string> showClosedMessage = null)
  {
    ShopData shopData;
    if (!DataLoader.Shops(Game1.content).TryGetValue(shopId, out shopData))
      return false;
    IList<NPC> source = (IList<NPC>) location.currentEvent?.actors ?? (IList<NPC>) location.characters;
    NPC owner = (NPC) null;
    ShopOwnerData ownerData = (ShopOwnerData) null;
    ShopOwnerData[] array = ShopBuilder.GetCurrentOwners(shopData).ToArray<ShopOwnerData>();
    foreach (ShopOwnerData shopOwnerData in array)
    {
      if (!forceOpen || shopOwnerData.ClosedMessage == null)
      {
        foreach (NPC npc in (IEnumerable<NPC>) source)
        {
          if (shopOwnerData.IsValid(npc.Name))
          {
            Point tilePoint = npc.TilePoint;
            int num;
            if (!ownerArea.HasValue || ownerArea.Value.Contains(tilePoint))
            {
              if (maxOwnerY.HasValue)
              {
                int y = tilePoint.Y;
                int? nullable = maxOwnerY;
                int valueOrDefault = nullable.GetValueOrDefault();
                num = y <= valueOrDefault & nullable.HasValue ? 1 : 0;
              }
              else
                num = 1;
            }
            else
              num = 0;
            if (num != 0)
            {
              owner = npc;
              ownerData = shopOwnerData;
              break;
            }
          }
        }
        if (ownerData != null)
          break;
      }
    }
    if (ownerData == null)
      ownerData = ((IEnumerable<ShopOwnerData>) array).FirstOrDefault<ShopOwnerData>((Func<ShopOwnerData, bool>) (p =>
      {
        if (p.Type != ShopOwnerType.AnyOrNone && p.Type != ShopOwnerType.None)
          return false;
        return !forceOpen || p.ClosedMessage == null;
      }));
    if (forceOpen && ownerData == null)
    {
      foreach (ShopOwnerData shopOwnerData in array)
      {
        if (shopOwnerData.Type == ShopOwnerType.Any)
        {
          ownerData = shopOwnerData;
          owner = source.FirstOrDefault<NPC>((Func<NPC, bool>) (p => p.IsVillager));
          if (owner == null)
            Utility.ForEachVillager((Func<NPC, bool>) (npc =>
            {
              owner = npc;
              return false;
            }));
        }
        else
        {
          owner = Game1.getCharacterFromName(shopOwnerData.Name);
          if (owner != null)
            ownerData = shopOwnerData;
        }
        if (ownerData != null)
          break;
      }
    }
    if (ownerData != null && ownerData.ClosedMessage != null)
    {
      string text = TokenParser.ParseText(ownerData.ClosedMessage);
      if (showClosedMessage != null)
        showClosedMessage(text);
      else
        Game1.drawObjectDialogue(text);
      return false;
    }
    if (!(ownerData != null | forceOpen))
      return false;
    Game1.activeClickableMenu = (IClickableMenu) new ShopMenu(shopId, shopData, ownerData, owner);
    return true;
  }

  /// <summary>Apply a set of modifiers to a value.</summary>
  /// <param name="value">The base value to which to apply modifiers.</param>
  /// <param name="modifiers">The modifiers to apply.</param>
  /// <param name="mode">How multiple quantity modifiers should be combined.</param>
  /// <param name="location">The location for which to check queries, or <c>null</c> for the current location.</param>
  /// <param name="player">The player for which to check queries, or <c>null</c> for the current player.</param>
  /// <param name="targetItem">The target item (e.g. machine output or tree fruit) for which to check queries, or <c>null</c> if not applicable.</param>
  /// <param name="inputItem">The input item (e.g. machine input) for which to check queries, or <c>null</c> if not applicable.</param>
  /// <param name="random">The random number generator to use, or <c>null</c> for <see cref="F:StardewValley.Game1.random" />.</param>
  public static float ApplyQuantityModifiers(
    float value,
    IList<QuantityModifier> modifiers,
    QuantityModifier.QuantityModifierMode mode = QuantityModifier.QuantityModifierMode.Stack,
    GameLocation location = null,
    Farmer player = null,
    Item targetItem = null,
    Item inputItem = null,
    Random random = null)
  {
    if ((modifiers != null ? (!modifiers.Any<QuantityModifier>() ? 1 : 0) : 1) != 0)
      return value;
    if (random == null)
      random = Game1.random;
    float? nullable1 = new float?();
    foreach (QuantityModifier modifier in (IEnumerable<QuantityModifier>) modifiers)
    {
      float amount = modifier.Amount;
      List<float> randomAmount = modifier.RandomAmount;
      if ((randomAmount != null ? (randomAmount.Any<float>() ? 1 : 0) : 0) != 0)
        amount = random.ChooseFrom<float>((IList<float>) modifier.RandomAmount);
      float? nullable2;
      if (GameStateQuery.CheckConditions(modifier.Condition, location, player, targetItem, inputItem, random))
      {
        switch (mode)
        {
          case QuantityModifier.QuantityModifierMode.Minimum:
            float num1 = QuantityModifier.Apply(value, modifier.Modification, amount);
            if (nullable1.HasValue)
            {
              double num2 = (double) num1;
              nullable2 = nullable1;
              double valueOrDefault = (double) nullable2.GetValueOrDefault();
              if (!(num2 < valueOrDefault & nullable2.HasValue))
                continue;
            }
            nullable1 = new float?(num1);
            continue;
          case QuantityModifier.QuantityModifierMode.Maximum:
            float num3 = QuantityModifier.Apply(value, modifier.Modification, amount);
            if (nullable1.HasValue)
            {
              double num4 = (double) num3;
              nullable2 = nullable1;
              double valueOrDefault = (double) nullable2.GetValueOrDefault();
              if (!(num4 > valueOrDefault & nullable2.HasValue))
                continue;
            }
            nullable1 = new float?(num3);
            continue;
          default:
            nullable1 = new float?(QuantityModifier.Apply(nullable1 ?? value, modifier.Modification, amount));
            continue;
        }
      }
    }
    return nullable1 ?? value;
  }

  public static bool IsForbiddenDishOfTheDay(string id)
  {
    return id == "346" || id == "196" || id == "216" || id == "224" || id == "206" || id == "395" || !ItemRegistry.Exists(id);
  }

  public static bool removeLightSource([NotNullWhen(true)] string identifier)
  {
    return identifier != null && Game1.currentLightSources.Remove(identifier);
  }

  public static Horse findHorseForPlayer(long uid)
  {
    Horse match = (Horse) null;
    Utility.ForEachLocation((Func<GameLocation, bool>) (location =>
    {
      foreach (NPC character in location.characters)
      {
        if (character is Horse horse2 && horse2.ownerId.Value == uid)
        {
          match = horse2;
          return false;
        }
      }
      return true;
    }), includeGenerated: true);
    return match;
  }

  public static Horse findHorse(Guid horseId)
  {
    Horse match = (Horse) null;
    Utility.ForEachLocation((Func<GameLocation, bool>) (location =>
    {
      foreach (NPC character in location.characters)
      {
        if (character is Horse horse2 && horse2.HorseId == horseId)
        {
          match = horse2;
          return false;
        }
      }
      return true;
    }), includeGenerated: true);
    return match;
  }

  public static void addDirtPuffs(
    GameLocation location,
    int tileX,
    int tileY,
    int tilesWide,
    int tilesHigh,
    int number = 5)
  {
    for (int x = tileX; x < tileX + tilesWide; ++x)
    {
      for (int y = tileY; y < tileY + tilesHigh; ++y)
      {
        for (int index = 0; index < number; ++index)
          location.temporarySprites.Add(new TemporaryAnimatedSprite(Game1.random.Choose<int>(46, 12), new Vector2((float) x, (float) y) * 64f + new Vector2((float) Game1.random.Next(-16, 32 /*0x20*/), (float) Game1.random.Next(-16, 32 /*0x20*/)), Color.White, 10, Game1.random.NextBool())
          {
            delayBeforeAnimationStart = Math.Max(0, Game1.random.Next(-200, 400)),
            motion = new Vector2(0.0f, -1f),
            interval = (float) Game1.random.Next(50, 80 /*0x50*/)
          });
        location.temporarySprites.Add(new TemporaryAnimatedSprite(14, new Vector2((float) x, (float) y) * 64f + new Vector2((float) Game1.random.Next(-16, 32 /*0x20*/), (float) Game1.random.Next(-16, 32 /*0x20*/)), Color.White, 10, Game1.random.NextBool()));
      }
    }
  }

  public static void addSmokePuff(
    GameLocation l,
    Vector2 v,
    int delay = 0,
    float baseScale = 2f,
    float scaleChange = 0.02f,
    float alpha = 0.75f,
    float alphaFade = 0.002f)
  {
    TemporaryAnimatedSprite temporaryAnimatedSprite = TemporaryAnimatedSprite.GetTemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(372, 1956, 10, 10), v, false, alphaFade, Color.Gray);
    temporaryAnimatedSprite.alpha = alpha;
    temporaryAnimatedSprite.motion = new Vector2(0.0f, -0.5f);
    temporaryAnimatedSprite.acceleration = new Vector2(1f / 500f, 0.0f);
    temporaryAnimatedSprite.interval = 99999f;
    temporaryAnimatedSprite.layerDepth = 1f;
    temporaryAnimatedSprite.scale = baseScale;
    temporaryAnimatedSprite.scaleChange = scaleChange;
    temporaryAnimatedSprite.rotationChange = (float) ((double) Game1.random.Next(-5, 6) * 3.1415927410125732 / 256.0);
    temporaryAnimatedSprite.delayBeforeAnimationStart = delay;
    l.temporarySprites.Add(temporaryAnimatedSprite);
  }

  public static LightSource getLightSource([NotNullWhen(true)] string identifier)
  {
    LightSource lightSource;
    return identifier == null || !Game1.currentLightSources.TryGetValue(identifier, out lightSource) ? (LightSource) null : lightSource;
  }

  public static int SortAllFurnitures(Furniture a, Furniture b)
  {
    string qualifiedItemId1 = a.QualifiedItemId;
    string qualifiedItemId2 = b.QualifiedItemId;
    if (qualifiedItemId1 != qualifiedItemId2)
    {
      if (qualifiedItemId1 == "(F)1226" || qualifiedItemId1 == "(F)1308")
        return -1;
      if (qualifiedItemId2 == "(F)1226" || qualifiedItemId2 == "(F)1308")
        return 1;
    }
    if (a.furniture_type.Value != b.furniture_type.Value)
      return a.furniture_type.Value.CompareTo(b.furniture_type.Value);
    if (a.furniture_type.Value == 12 && b.furniture_type.Value == 12)
    {
      int num1 = a.Name.StartsWith("Floor Divider ") ? 1 : 0;
      bool flag = b.Name.StartsWith("Floor Divider ");
      int num2 = flag ? 1 : 0;
      if (num1 != num2)
        return flag ? -1 : 1;
    }
    return a.ItemId.CompareTo(b.ItemId);
  }

  public static bool doesAnyFarmerHaveOrWillReceiveMail(string id)
  {
    foreach (Farmer allFarmer in Game1.getAllFarmers())
    {
      if (allFarmer.hasOrWillReceiveMail(id))
        return true;
    }
    return false;
  }

  public static string loadStringShort(string fileWithinStringsFolder, string key)
  {
    return Game1.content.LoadString($"Strings\\{fileWithinStringsFolder}:{key}");
  }

  public static bool doesAnyFarmerHaveMail(string id)
  {
    if (Game1.player.mailReceived.Contains(id))
      return true;
    foreach (Farmer farmer in (IEnumerable<Farmer>) Game1.otherFarmers.Values)
    {
      if (farmer.mailReceived.Contains(id))
        return true;
    }
    return false;
  }

  public static FarmEvent pickFarmEvent()
  {
    return Game1.hooks.OnUtility_PickFarmEvent((Func<FarmEvent>) (() =>
    {
      Random daySaveRandom = Utility.CreateDaySaveRandom();
      for (int index = 0; index < 10; ++index)
        daySaveRandom.NextDouble();
      if (Game1.weddingToday)
        return (FarmEvent) null;
      foreach (Farmer onlineFarmer in Game1.getOnlineFarmers())
      {
        Friendship spouseFriendship = onlineFarmer.GetSpouseFriendship();
        if (spouseFriendship != null && spouseFriendship.IsMarried() && spouseFriendship.WeddingDate == Game1.Date)
          return (FarmEvent) null;
      }
      if (Game1.stats.DaysPlayed == 31U /*0x1F*/)
        return (FarmEvent) new SoundInTheNightEvent(4);
      if (Game1.MasterPlayer.mailForTomorrow.Contains("leoMoved%&NL&%") || Game1.MasterPlayer.mailForTomorrow.Contains("leoMoved"))
        return (FarmEvent) new WorldChangeEvent(14);
      if (Game1.player.mailForTomorrow.Contains("jojaPantry%&NL&%") || Game1.player.mailForTomorrow.Contains("jojaPantry"))
        return (FarmEvent) new WorldChangeEvent(0);
      if (Game1.player.mailForTomorrow.Contains("ccPantry%&NL&%") || Game1.player.mailForTomorrow.Contains("ccPantry"))
        return (FarmEvent) new WorldChangeEvent(1);
      if (Game1.player.mailForTomorrow.Contains("jojaVault%&NL&%") || Game1.player.mailForTomorrow.Contains("jojaVault"))
        return (FarmEvent) new WorldChangeEvent(6);
      if (Game1.player.mailForTomorrow.Contains("ccVault%&NL&%") || Game1.player.mailForTomorrow.Contains("ccVault"))
        return (FarmEvent) new WorldChangeEvent(7);
      if (Game1.player.mailForTomorrow.Contains("jojaBoilerRoom%&NL&%") || Game1.player.mailForTomorrow.Contains("jojaBoilerRoom"))
        return (FarmEvent) new WorldChangeEvent(2);
      if (Game1.player.mailForTomorrow.Contains("ccBoilerRoom%&NL&%") || Game1.player.mailForTomorrow.Contains("ccBoilerRoom"))
        return (FarmEvent) new WorldChangeEvent(3);
      if (Game1.player.mailForTomorrow.Contains("jojaCraftsRoom%&NL&%") || Game1.player.mailForTomorrow.Contains("jojaCraftsRoom"))
        return (FarmEvent) new WorldChangeEvent(4);
      if (Game1.player.mailForTomorrow.Contains("ccCraftsRoom%&NL&%") || Game1.player.mailForTomorrow.Contains("ccCraftsRoom"))
        return (FarmEvent) new WorldChangeEvent(5);
      if (Game1.player.mailForTomorrow.Contains("jojaFishTank%&NL&%") || Game1.player.mailForTomorrow.Contains("jojaFishTank"))
        return (FarmEvent) new WorldChangeEvent(8);
      if (Game1.player.mailForTomorrow.Contains("ccFishTank%&NL&%") || Game1.player.mailForTomorrow.Contains("ccFishTank"))
        return (FarmEvent) new WorldChangeEvent(9);
      if (Game1.player.mailForTomorrow.Contains("ccMovieTheaterJoja%&NL&%") || Game1.player.mailForTomorrow.Contains("jojaMovieTheater"))
        return (FarmEvent) new WorldChangeEvent(10);
      if (Game1.player.mailForTomorrow.Contains("ccMovieTheater%&NL&%") || Game1.player.mailForTomorrow.Contains("ccMovieTheater"))
        return (FarmEvent) new WorldChangeEvent(11);
      if (Game1.MasterPlayer.eventsSeen.Contains("191393") && (Game1.isRaining || Game1.isLightning) && !Game1.MasterPlayer.mailReceived.Contains("abandonedJojaMartAccessible") && !Game1.MasterPlayer.mailReceived.Contains("ccMovieTheater"))
        return (FarmEvent) new WorldChangeEvent(12);
      if (Game1.MasterPlayer.hasOrWillReceiveMail("willyBoatTicketMachine") && Game1.MasterPlayer.hasOrWillReceiveMail("willyBoatHull") && Game1.MasterPlayer.hasOrWillReceiveMail("willyBoatAnchor") && !Game1.MasterPlayer.hasOrWillReceiveMail("willyBoatFixed"))
        return (FarmEvent) new WorldChangeEvent(13);
      if (Game1.MasterPlayer.hasOrWillReceiveMail("activateGoldenParrotsTonight") && !Game1.netWorldState.Value.ActivatedGoldenParrot)
        return (FarmEvent) new WorldChangeEvent(15);
      if (Game1.player.mailReceived.Contains("ccPantry") && daySaveRandom.NextDouble() < 0.1 && !Game1.MasterPlayer.mailReceived.Contains("raccoonTreeFallen"))
        return (FarmEvent) new SoundInTheNightEvent(5);
      if (!Game1.player.mailReceived.Contains("sawQiPlane"))
      {
        foreach (Farmer onlineFarmer in Game1.getOnlineFarmers())
        {
          if (onlineFarmer.mailReceived.Contains("gotFirstBillboardPrizeTicket") || Game1.stats.DaysPlayed > 50U)
            return (FarmEvent) new QiPlaneEvent();
        }
      }
      double num = Game1.getFarm().hasMatureFairyRoseTonight ? 0.007 : 0.0;
      Game1.getFarm().hasMatureFairyRoseTonight = false;
      if (daySaveRandom.NextDouble() < 0.01 + num && !Game1.IsWinter && Game1.dayOfMonth != 1)
        return (FarmEvent) new FairyEvent();
      if (daySaveRandom.NextDouble() < 0.01 && Game1.stats.DaysPlayed > 20U)
        return (FarmEvent) new WitchEvent();
      if (daySaveRandom.NextDouble() < 0.01 && Game1.stats.DaysPlayed > 5U)
        return (FarmEvent) new SoundInTheNightEvent(1);
      if (daySaveRandom.NextDouble() < 0.005)
        return (FarmEvent) new SoundInTheNightEvent(3);
      if (daySaveRandom.NextDouble() >= 0.008 || Game1.year <= 1 || Game1.MasterPlayer.mailReceived.Contains("Got_Capsule"))
        return (FarmEvent) null;
      Game1.player.team.RequestSetMail(PlayerActionTarget.Host, "Got_Capsule", MailType.Received, true);
      return (FarmEvent) new SoundInTheNightEvent(0);
    }));
  }

  public static bool hasFinishedJojaRoute()
  {
    bool flag = false;
    if (Game1.MasterPlayer.mailReceived.Contains("jojaVault"))
      flag = true;
    else if (!Game1.MasterPlayer.mailReceived.Contains("ccVault"))
      return false;
    if (Game1.MasterPlayer.mailReceived.Contains("jojaPantry"))
      flag = true;
    else if (!Game1.MasterPlayer.mailReceived.Contains("ccPantry"))
      return false;
    if (Game1.MasterPlayer.mailReceived.Contains("jojaBoilerRoom"))
      flag = true;
    else if (!Game1.MasterPlayer.mailReceived.Contains("ccBoilerRoom"))
      return false;
    if (Game1.MasterPlayer.mailReceived.Contains("jojaCraftsRoom"))
      flag = true;
    else if (!Game1.MasterPlayer.mailReceived.Contains("ccCraftsRoom"))
      return false;
    if (Game1.MasterPlayer.mailReceived.Contains("jojaFishTank"))
      flag = true;
    else if (!Game1.MasterPlayer.mailReceived.Contains("ccFishTank"))
      return false;
    return flag || Game1.MasterPlayer.mailReceived.Contains("JojaMember");
  }

  public static FarmEvent pickPersonalFarmEvent()
  {
    Random random = Utility.CreateRandom((double) Game1.stats.DaysPlayed, (double) (Game1.uniqueIDForThisGame / 2UL), 470124797.0, (double) Game1.player.UniqueMultiplayerID);
    if (Game1.weddingToday)
      return (FarmEvent) null;
    NPC spouse = Game1.player.getSpouse();
    bool flag = Game1.player.isMarriedOrRoommates();
    if (flag && Game1.player.GetSpouseFriendship().DaysUntilBirthing <= 0 && Game1.player.GetSpouseFriendship().NextBirthingDate != (WorldDate) null)
    {
      if (spouse != null)
        return (FarmEvent) new BirthingEvent();
      long key = Game1.player.team.GetSpouse(Game1.player.UniqueMultiplayerID).Value;
      if (Game1.otherFarmers.ContainsKey(key))
        return (FarmEvent) new PlayerCoupleBirthingEvent();
    }
    else
    {
      if (flag)
      {
        bool? pregnant = spouse?.canGetPregnant();
        if (pregnant.HasValue && pregnant.GetValueOrDefault() && Game1.player.currentLocation == Game1.getLocationFromName(Game1.player.homeLocation.Value) && random.NextDouble() < 0.05 && GameStateQuery.CheckConditions(spouse.GetData()?.SpouseWantsChildren))
          return (FarmEvent) new QuestionEvent(1);
      }
      if (flag && Game1.player.team.GetSpouse(Game1.player.UniqueMultiplayerID).HasValue && Game1.player.GetSpouseFriendship().NextBirthingDate == (WorldDate) null && random.NextDouble() < 0.05)
      {
        long key = Game1.player.team.GetSpouse(Game1.player.UniqueMultiplayerID).Value;
        Farmer farmer1;
        if (Game1.otherFarmers.TryGetValue(key, out farmer1))
        {
          Farmer farmer2 = farmer1;
          if (farmer2.currentLocation == Game1.player.currentLocation && (farmer2.currentLocation == Game1.getLocationFromName(farmer2.homeLocation.Value) || farmer2.currentLocation == Game1.getLocationFromName(Game1.player.homeLocation.Value)) && Utility.playersCanGetPregnantHere(farmer2.currentLocation as FarmHouse))
            return (FarmEvent) new QuestionEvent(3);
        }
      }
    }
    return random.NextBool() ? (FarmEvent) new QuestionEvent(2) : (FarmEvent) new SoundInTheNightEvent(2);
  }

  public static bool playersCanGetPregnantHere(FarmHouse farmHouse)
  {
    List<Child> children = farmHouse.getChildren();
    if (farmHouse.cribStyle.Value <= 0 || farmHouse.getChildrenCount() >= 2 || farmHouse.upgradeLevel < 2 || children.Count >= 2)
      return false;
    return children.Count == 0 || children[0].Age > 2;
  }

  public static string capitalizeFirstLetter(string s)
  {
    return string.IsNullOrEmpty(s) ? "" : s[0].ToString().ToUpper() + (s.Length > 1 ? s.Substring(1) : "");
  }

  public static void repositionLightSource([NotNullWhen(true)] string identifier, Vector2 position)
  {
    LightSource lightSource;
    if (identifier == null || !Game1.currentLightSources.TryGetValue(identifier, out lightSource))
      return;
    lightSource.position.Value = position;
  }

  public static bool areThereAnyOtherAnimalsWithThisName(string name)
  {
    bool found = false;
    if (name != null)
      Utility.ForEachLocation((Func<GameLocation, bool>) (location =>
      {
        foreach (Character character in location.animals.Values)
        {
          if (character.displayName == name)
          {
            found = true;
            return false;
          }
        }
        return true;
      }));
    return found;
  }

  public static string getNumberWithCommas(int number)
  {
    StringBuilder stringBuilder = new StringBuilder(number.ToString() ?? "");
    string str;
    switch (LocalizedContentManager.CurrentLanguageCode)
    {
      case LocalizedContentManager.LanguageCode.ru:
        str = " ";
        break;
      case LocalizedContentManager.LanguageCode.pt:
      case LocalizedContentManager.LanguageCode.es:
      case LocalizedContentManager.LanguageCode.de:
      case LocalizedContentManager.LanguageCode.hu:
        str = ".";
        break;
      case LocalizedContentManager.LanguageCode.mod:
        str = LocalizedContentManager.CurrentModLanguage?.NumberComma ?? ",";
        break;
      default:
        str = ",";
        break;
    }
    for (int index = stringBuilder.Length - 4; index >= 0; index -= 3)
      stringBuilder.Insert(index + 1, str);
    return stringBuilder.ToString();
  }

  protected static bool _HasBuildingOrUpgrade(GameLocation location, string buildingId)
  {
    if (location.getNumberBuildingsConstructed(buildingId) > 0)
      return true;
    foreach (KeyValuePair<string, BuildingData> keyValuePair in (IEnumerable<KeyValuePair<string, BuildingData>>) Game1.buildingData)
    {
      string key = keyValuePair.Key;
      BuildingData buildingData = keyValuePair.Value;
      if (!(key == buildingId) && buildingData.BuildingToUpgrade == buildingId && Utility._HasBuildingOrUpgrade(location, key))
        return true;
    }
    return false;
  }

  public static List<int> getDaysOfBooksellerThisSeason()
  {
    Random random = Utility.CreateRandom((double) (Game1.year * 11), (double) Game1.uniqueIDForThisGame, (double) Game1.seasonIndex);
    int[] numArray = (int[]) null;
    List<int> booksellerThisSeason = new List<int>();
    switch (Game1.season)
    {
      case Season.Spring:
        numArray = new int[5]{ 11, 12, 21, 22, 25 };
        break;
      case Season.Summer:
        numArray = new int[5]{ 9, 12, 18, 25, 27 };
        break;
      case Season.Fall:
        numArray = new int[8]{ 4, 7, 8, 9, 12, 19, 22, 25 };
        break;
      case Season.Winter:
        numArray = new int[6]{ 5, 11, 12, 19, 22, 24 };
        break;
    }
    int index = random.Next(numArray.Length);
    booksellerThisSeason.Add(numArray[index]);
    booksellerThisSeason.Add(numArray[(index + numArray.Length / 2) % numArray.Length]);
    return booksellerThisSeason;
  }

  /// <summary>Get whether there's green rain scheduled for today.</summary>
  public static bool isGreenRainDay() => Utility.isGreenRainDay(Game1.dayOfMonth, Game1.season);

  /// <summary>Get whether there's green rain scheduled on the given day.</summary>
  /// <param name="day">The day of month to check.</param>
  /// <param name="season">The season key to check.</param>
  public static bool isGreenRainDay(int day, Season season)
  {
    if (season != Season.Summer)
      return false;
    Random random = Utility.CreateRandom((double) (Game1.year * 777), (double) Game1.uniqueIDForThisGame);
    int[] options = new int[8]
    {
      5,
      6,
      7,
      14,
      15,
      16 /*0x10*/,
      18,
      23
    };
    return day == random.ChooseFrom<int>((IList<int>) options);
  }

  public static List<Object> getPurchaseAnimalStock(GameLocation location)
  {
    List<Object> purchaseAnimalStock = new List<Object>();
    foreach (KeyValuePair<string, FarmAnimalData> keyValuePair in (IEnumerable<KeyValuePair<string, FarmAnimalData>>) Game1.farmAnimalData)
    {
      FarmAnimalData farmAnimalData = keyValuePair.Value;
      if (farmAnimalData.PurchasePrice >= 0 && GameStateQuery.CheckConditions(farmAnimalData.UnlockCondition))
      {
        Object object1 = new Object("100", 1, price: farmAnimalData.PurchasePrice);
        object1.Name = keyValuePair.Key;
        object1.Type = (string) null;
        Object object2 = object1;
        if (farmAnimalData.RequiredBuilding != null && !Utility._HasBuildingOrUpgrade(location, farmAnimalData.RequiredBuilding))
          object2.Type = farmAnimalData.ShopMissingBuildingDescription == null ? "" : TokenParser.ParseText(farmAnimalData.ShopMissingBuildingDescription);
        object2.displayNameFormat = farmAnimalData.ShopDisplayName;
        purchaseAnimalStock.Add(object2);
      }
    }
    return purchaseAnimalStock;
  }

  public static string SanitizeName(string name)
  {
    return Regex.Replace(name, "[^a-zA-Z0-9]", string.Empty);
  }

  public static void FixChildNameCollisions()
  {
    List<NPC> allCharacters = Utility.getAllCharacters();
    foreach (NPC npc1 in allCharacters)
    {
      if (npc1 is Child)
      {
        string name1 = npc1.Name;
        string name2 = npc1.Name;
        bool flag;
        do
        {
          flag = false;
          if (Game1.characterData.ContainsKey(name2))
          {
            name2 += " ";
            flag = true;
          }
          else
          {
            foreach (NPC npc2 in allCharacters)
            {
              if (npc2 != npc1 && npc2.name.Equals((object) name2))
              {
                name2 += " ";
                flag = true;
              }
            }
          }
        }
        while (flag);
        if (name2 != npc1.Name)
        {
          npc1.Name = name2;
          npc1.displayName = (string) null;
          foreach (Farmer allFarmer in Game1.getAllFarmers())
          {
            Friendship friendship;
            if (allFarmer.friendshipData != null && allFarmer.friendshipData.TryGetValue(name1, out friendship))
            {
              allFarmer.friendshipData[name2] = friendship;
              allFarmer.friendshipData.Remove(name1);
            }
          }
        }
      }
    }
  }

  public static Vector2 getCornersOfThisRectangle(ref Microsoft.Xna.Framework.Rectangle r, int corner)
  {
    switch (corner)
    {
      case 1:
        return new Vector2((float) (r.Right - 1), (float) r.Y);
      case 2:
        return new Vector2((float) (r.Right - 1), (float) (r.Bottom - 1));
      case 3:
        return new Vector2((float) r.X, (float) (r.Bottom - 1));
      default:
        return new Vector2((float) r.X, (float) r.Y);
    }
  }

  public static Vector2 GetCurvePoint(float t, Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
  {
    float num1 = (float) (3.0 * ((double) p1.X - (double) p0.X));
    float num2 = (float) (3.0 * ((double) p1.Y - (double) p0.Y));
    float num3 = (float) (3.0 * ((double) p2.X - (double) p1.X)) - num1;
    float num4 = (float) (3.0 * ((double) p2.Y - (double) p1.Y)) - num2;
    double num5 = (double) p3.X - (double) p0.X - (double) num1 - (double) num3;
    float num6 = p3.Y - p0.Y - num2 - num4;
    float num7 = t * t * t;
    float num8 = t * t;
    double num9 = (double) num7;
    return new Vector2((float) (num5 * num9 + (double) num3 * (double) num8 + (double) num1 * (double) t) + p0.X, (float) ((double) num6 * (double) num7 + (double) num4 * (double) num8 + (double) num2 * (double) t) + p0.Y);
  }

  public static GameLocation getGameLocationOfCharacter(NPC n) => n.currentLocation;

  public static int[] parseStringToIntArray(string s, char delimiter = ' ')
  {
    string[] strArray = s.Split(delimiter);
    int[] stringToIntArray = new int[strArray.Length];
    for (int index = 0; index < strArray.Length; ++index)
      stringToIntArray[index] = Convert.ToInt32(strArray[index]);
    return stringToIntArray;
  }

  public static void drawLineWithScreenCoordinates(
    int x1,
    int y1,
    int x2,
    int y2,
    SpriteBatch b,
    Color color1,
    float layerDepth = 1f,
    int thickness = 1)
  {
    Vector2 vector2_1 = new Vector2((float) x2, (float) y2);
    Vector2 vector2_2 = new Vector2((float) x1, (float) y1);
    Vector2 vector2_3 = vector2_2;
    Vector2 vector2_4 = vector2_1 - vector2_3;
    float rotation = (float) Math.Atan2((double) vector2_4.Y, (double) vector2_4.X);
    b.Draw(Game1.fadeToBlackRect, new Microsoft.Xna.Framework.Rectangle((int) vector2_2.X, (int) vector2_2.Y, (int) vector2_4.Length(), thickness), new Microsoft.Xna.Framework.Rectangle?(), color1, rotation, new Vector2(0.0f, 0.0f), SpriteEffects.None, layerDepth);
    b.Draw(Game1.fadeToBlackRect, new Microsoft.Xna.Framework.Rectangle((int) vector2_2.X, (int) vector2_2.Y + 1, (int) vector2_4.Length(), thickness), new Microsoft.Xna.Framework.Rectangle?(), color1, rotation, new Vector2(0.0f, 0.0f), SpriteEffects.None, layerDepth);
  }

  public static Farmer isThereAFarmerWithinDistance(
    Vector2 tileLocation,
    int tilesAway,
    GameLocation location)
  {
    return Utility.GetPlayersWithinDistance(tileLocation, tilesAway, location).FirstOrDefault<Farmer>();
  }

  public static Character isThereAFarmerOrCharacterWithinDistance(
    Vector2 tileLocation,
    int tilesAway,
    GameLocation environment)
  {
    return (Character) Utility.GetNpcsWithinDistance(tileLocation, tilesAway, environment).FirstOrDefault<NPC>() ?? (Character) Utility.GetPlayersWithinDistance(tileLocation, tilesAway, environment).FirstOrDefault<Farmer>();
  }

  /// <summary>Get all NPCs within a given distance of a tile.</summary>
  /// <param name="centerTile">The tile location around which to find NPCs.</param>
  /// <param name="tilesAway">The maximum tile distance (including diagonal) within which to match NPCs.</param>
  /// <param name="location">The location to search.</param>
  public static IEnumerable<NPC> GetNpcsWithinDistance(
    Vector2 centerTile,
    int tilesAway,
    GameLocation location)
  {
    foreach (NPC character in location.characters)
    {
      if ((double) Vector2.Distance(character.Tile, centerTile) <= (double) tilesAway)
        yield return character;
    }
  }

  /// <summary>Get all players within a given distance of a tile.</summary>
  /// <param name="centerTile">The tile location around which to find NPCs.</param>
  /// <param name="tilesAway">The maximum tile distance (including diagonal) within which to match NPCs.</param>
  /// <param name="location">The location to search.</param>
  public static IEnumerable<Farmer> GetPlayersWithinDistance(
    Vector2 centerTile,
    int tilesAway,
    GameLocation location)
  {
    foreach (Farmer farmer in location.farmers)
    {
      if ((double) Vector2.Distance(farmer.Tile, centerTile) <= (double) tilesAway)
        yield return farmer;
    }
  }

  public static Color getRedToGreenLerpColor(float power)
  {
    return new Color((double) power <= 0.5 ? (int) byte.MaxValue : (int) ((1.0 - (double) power) * 2.0 * (double) byte.MaxValue), (int) Math.Min((float) byte.MaxValue, (float) ((double) power * 2.0 * (double) byte.MaxValue)), 0);
  }

  public static FarmHouse getHomeOfFarmer(Farmer who)
  {
    return Game1.RequireLocation<FarmHouse>(who.homeLocation.Value);
  }

  public static Vector2 getRandomPositionOnScreen()
  {
    return new Vector2((float) Game1.random.Next(Game1.viewport.Width), (float) Game1.random.Next(Game1.viewport.Height));
  }

  public static Vector2 getRandomPositionOnScreenNotOnMap()
  {
    Vector2 vector2 = Vector2.Zero;
    int num;
    for (num = 0; num < 30 && (vector2.Equals(Vector2.Zero) || Game1.currentLocation.isTileOnMap((vector2 + new Vector2((float) Game1.viewport.X, (float) Game1.viewport.Y)) / 64f)); ++num)
      vector2 = Utility.getRandomPositionOnScreen();
    return num >= 30 ? new Vector2(-1000f, -1000f) : vector2;
  }

  public static Microsoft.Xna.Framework.Rectangle getRectangleCenteredAt(Vector2 v, int size)
  {
    return new Microsoft.Xna.Framework.Rectangle((int) v.X - size / 2, (int) v.Y - size / 2, size, size);
  }

  public static bool checkForCharacterInteractionAtTile(Vector2 tileLocation, Farmer who)
  {
    NPC npc = Game1.currentLocation.isCharacterAtTile(tileLocation);
    if (npc == null || npc.IsMonster || npc.IsInvisible)
      return false;
    if (npc.SimpleNonVillagerNPC && npc.nonVillagerNPCTimesTalked != -1)
      Game1.mouseCursor = Game1.cursor_talk;
    else if (Game1.currentLocation is MovieTheater)
      Game1.mouseCursor = Game1.cursor_talk;
    else if (npc.Name == "Pierre" && who.ActiveObject?.QualifiedItemId == "(O)897" && npc.tryToReceiveActiveObject(who, true))
    {
      Game1.mouseCursor = Game1.cursor_gift;
    }
    else
    {
      bool? nullable = who.ActiveItem?.canBeGivenAsGift();
      if (nullable.HasValue && nullable.GetValueOrDefault() && npc.CanReceiveGifts() && !who.isRidingHorse() && who.friendshipData.ContainsKey(npc.Name) && !Game1.eventUp)
        Game1.mouseCursor = npc.tryToReceiveActiveObject(who, true) ? Game1.cursor_gift : Game1.cursor_default;
      else if (npc.canTalk())
      {
        if (npc.CurrentDialogue == null || npc.CurrentDialogue.Count <= 0)
        {
          if (Game1.player.spouse != null && npc.Name != null && npc.Name == Game1.player.spouse && npc.shouldSayMarriageDialogue.Value)
          {
            NetList<MarriageDialogueReference, NetRef<MarriageDialogueReference>> marriageDialogue = npc.currentMarriageDialogue;
            // ISSUE: explicit non-virtual call
            if ((marriageDialogue != null ? (__nonvirtual (marriageDialogue.Count) > 0 ? 1 : 0) : 0) != 0)
              goto label_14;
          }
          if (!npc.hasTemporaryMessageAvailable() && (!who.hasClubCard || !npc.Name.Equals("Bouncer") || !who.IsLocalPlayer) && (!npc.Name.Equals("Henchman") || !npc.currentLocation.Name.Equals("WitchSwamp") || who.hasOrWillReceiveMail("henchmanGone")))
            goto label_16;
        }
label_14:
        if (!npc.isOnSilentTemporaryMessage())
          Game1.mouseCursor = Game1.cursor_talk;
      }
    }
label_16:
    if (Game1.eventUp && Game1.CurrentEvent != null && !Game1.CurrentEvent.playerControlSequence)
      Game1.mouseCursor = Game1.cursor_default;
    Game1.currentLocation.checkForSpecialCharacterIconAtThisTile(tileLocation);
    if (Game1.mouseCursor == Game1.cursor_gift || Game1.mouseCursor == Game1.cursor_talk)
      Game1.mouseCursorTransparency = !Utility.tileWithinRadiusOfPlayer((int) tileLocation.X, (int) tileLocation.Y, 1, who) ? 0.5f : 1f;
    return true;
  }

  public static bool canGrabSomethingFromHere(int x, int y, Farmer who)
  {
    if (Game1.currentLocation == null)
      return false;
    Vector2 vector2 = new Vector2((float) (x / 64 /*0x40*/), (float) (y / 64 /*0x40*/));
    if (Game1.currentLocation.isObjectAt(x, y))
      Game1.currentLocation.getObjectAt(x, y).hoverAction();
    if (Utility.checkForCharacterInteractionAtTile(vector2, who) || Utility.checkForCharacterInteractionAtTile(vector2 + new Vector2(0.0f, 1f), who) || !who.IsLocalPlayer || who.onBridge.Value)
      return false;
    if (Game1.currentLocation != null)
    {
      foreach (Furniture furniture in Game1.currentLocation.furniture)
      {
        if (furniture.GetBoundingBox().Contains(Utility.Vector2ToPoint(vector2 * 64f)) && furniture.IsTable() && furniture.heldObject.Value != null)
          return true;
      }
    }
    Object @object;
    if (Game1.currentLocation.Objects.TryGetValue(vector2, out @object))
    {
      if (@object.readyForHarvest.Value || @object.isSpawnedObject.Value || @object is IndoorPot indoorPot && indoorPot.hoeDirt.Value.readyForHarvest())
      {
        Game1.mouseCursor = Game1.cursor_harvest;
        if (Utility.withinRadiusOfPlayer(x, y, 1, who))
          return true;
        Game1.mouseCursorTransparency = 0.5f;
        return false;
      }
    }
    else
    {
      TerrainFeature terrainFeature;
      if (Game1.currentLocation.terrainFeatures.TryGetValue(vector2, out terrainFeature) && terrainFeature is HoeDirt hoeDirt && hoeDirt.readyForHarvest())
      {
        Game1.mouseCursor = Game1.cursor_harvest;
        if (Utility.withinRadiusOfPlayer(x, y, 1, who))
          return true;
        Game1.mouseCursorTransparency = 0.5f;
        return false;
      }
    }
    return false;
  }

  public static int getStringCountInList(List<string> strings, string whichStringToCheck)
  {
    int stringCountInList = 0;
    if (strings != null)
    {
      foreach (string str in strings)
      {
        if (str == whichStringToCheck)
          ++stringCountInList;
      }
    }
    return stringCountInList;
  }

  public static Microsoft.Xna.Framework.Rectangle getSourceRectWithinRectangularRegion(
    int regionX,
    int regionY,
    int regionWidth,
    int sourceIndex,
    int sourceWidth,
    int sourceHeight)
  {
    int num = regionWidth / sourceWidth;
    return new Microsoft.Xna.Framework.Rectangle(regionX + sourceIndex % num * sourceWidth, regionY + sourceIndex / num * sourceHeight, sourceWidth, sourceHeight);
  }

  public static void drawWithShadow(
    SpriteBatch b,
    Texture2D texture,
    Vector2 position,
    Microsoft.Xna.Framework.Rectangle sourceRect,
    Color color,
    float rotation,
    Vector2 origin,
    float scale = -1f,
    bool flipped = false,
    float layerDepth = -1f,
    int horizontalShadowOffset = -1,
    int verticalShadowOffset = -1,
    float shadowIntensity = 0.35f)
  {
    if ((double) scale == -1.0)
      scale = 4f;
    if ((double) layerDepth == -1.0)
      layerDepth = position.Y / 10000f;
    if (horizontalShadowOffset == -1)
      horizontalShadowOffset = -4;
    if (verticalShadowOffset == -1)
      verticalShadowOffset = 4;
    b.Draw(texture, position + new Vector2((float) horizontalShadowOffset, (float) verticalShadowOffset), new Microsoft.Xna.Framework.Rectangle?(sourceRect), Color.Black * shadowIntensity * ((float) color.A / (float) byte.MaxValue), rotation, origin, scale, flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, layerDepth - 0.0001f);
    b.Draw(texture, position, new Microsoft.Xna.Framework.Rectangle?(sourceRect), color, rotation, origin, scale, flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, layerDepth);
  }

  public static void drawTextWithShadow(
    SpriteBatch b,
    StringBuilder text,
    SpriteFont font,
    Vector2 position,
    Color color,
    float scale = 1f,
    float layerDepth = -1f,
    int horizontalShadowOffset = -1,
    int verticalShadowOffset = -1,
    float shadowIntensity = 1f,
    int numShadows = 3)
  {
    if ((double) layerDepth == -1.0)
      layerDepth = position.Y / 10000f;
    bool flag = Game1.content.GetCurrentLanguage() == LocalizedContentManager.LanguageCode.ru || Game1.content.GetCurrentLanguage() == LocalizedContentManager.LanguageCode.de;
    if (horizontalShadowOffset == -1)
      horizontalShadowOffset = font.Equals((object) Game1.smallFont) | flag ? -2 : -3;
    if (verticalShadowOffset == -1)
      verticalShadowOffset = font.Equals((object) Game1.smallFont) | flag ? 2 : 3;
    if (text == null)
      throw new ArgumentNullException(nameof (text));
    b.DrawString(font, text, position + new Vector2((float) horizontalShadowOffset, (float) verticalShadowOffset), Game1.textShadowDarkerColor * shadowIntensity, 0.0f, Vector2.Zero, scale, SpriteEffects.None, layerDepth - 0.0001f);
    switch (numShadows)
    {
      case 2:
        b.DrawString(font, text, position + new Vector2((float) horizontalShadowOffset, 0.0f), Game1.textShadowDarkerColor * shadowIntensity, 0.0f, Vector2.Zero, scale, SpriteEffects.None, layerDepth - 0.0002f);
        break;
      case 3:
        b.DrawString(font, text, position + new Vector2(0.0f, (float) verticalShadowOffset), Game1.textShadowDarkerColor * shadowIntensity, 0.0f, Vector2.Zero, scale, SpriteEffects.None, layerDepth - 0.0003f);
        break;
    }
    b.DrawString(font, text, position, color, 0.0f, Vector2.Zero, scale, SpriteEffects.None, layerDepth);
  }

  public static void drawTextWithShadow(
    SpriteBatch b,
    string text,
    SpriteFont font,
    Vector2 position,
    Color color,
    float scale = 1f,
    float layerDepth = -1f,
    int horizontalShadowOffset = -1,
    int verticalShadowOffset = -1,
    float shadowIntensity = 1f,
    int numShadows = 3)
  {
    if ((double) layerDepth == -1.0)
      layerDepth = position.Y / 10000f;
    bool flag = Game1.content.GetCurrentLanguage() == LocalizedContentManager.LanguageCode.ru || Game1.content.GetCurrentLanguage() == LocalizedContentManager.LanguageCode.de || Game1.content.GetCurrentLanguage() == LocalizedContentManager.LanguageCode.ko;
    if (horizontalShadowOffset == -1)
      horizontalShadowOffset = font.Equals((object) Game1.smallFont) | flag ? -2 : -3;
    if (verticalShadowOffset == -1)
      verticalShadowOffset = font.Equals((object) Game1.smallFont) | flag ? 2 : 3;
    if (text == null)
      text = "";
    b.DrawString(font, text, position + new Vector2((float) horizontalShadowOffset, (float) verticalShadowOffset), Game1.textShadowDarkerColor * shadowIntensity, 0.0f, Vector2.Zero, scale, SpriteEffects.None, layerDepth - 0.0001f);
    switch (numShadows)
    {
      case 2:
        b.DrawString(font, text, position + new Vector2((float) horizontalShadowOffset, 0.0f), Game1.textShadowDarkerColor * shadowIntensity, 0.0f, Vector2.Zero, scale, SpriteEffects.None, layerDepth - 0.0002f);
        break;
      case 3:
        b.DrawString(font, text, position + new Vector2(0.0f, (float) verticalShadowOffset), Game1.textShadowDarkerColor * shadowIntensity, 0.0f, Vector2.Zero, scale, SpriteEffects.None, layerDepth - 0.0003f);
        break;
    }
    b.DrawString(font, text, position, color, 0.0f, Vector2.Zero, scale, SpriteEffects.None, layerDepth);
  }

  public static void drawTextWithColoredShadow(
    SpriteBatch b,
    string text,
    SpriteFont font,
    Vector2 position,
    Color color,
    Color shadowColor,
    float scale = 1f,
    float layerDepth = -1f,
    int horizontalShadowOffset = -1,
    int verticalShadowOffset = -1,
    int numShadows = 3)
  {
    if ((double) layerDepth == -1.0)
      layerDepth = position.Y / 10000f;
    bool flag = Game1.content.GetCurrentLanguage() == LocalizedContentManager.LanguageCode.ru || Game1.content.GetCurrentLanguage() == LocalizedContentManager.LanguageCode.de;
    if (horizontalShadowOffset == -1)
      horizontalShadowOffset = font.Equals((object) Game1.smallFont) | flag ? -2 : -3;
    if (verticalShadowOffset == -1)
      verticalShadowOffset = font.Equals((object) Game1.smallFont) | flag ? 2 : 3;
    if (text == null)
      text = "";
    b.DrawString(font, text, position + new Vector2((float) horizontalShadowOffset, (float) verticalShadowOffset), shadowColor, 0.0f, Vector2.Zero, scale, SpriteEffects.None, layerDepth - 0.0001f);
    switch (numShadows)
    {
      case 2:
        b.DrawString(font, text, position + new Vector2((float) horizontalShadowOffset, 0.0f), shadowColor, 0.0f, Vector2.Zero, scale, SpriteEffects.None, layerDepth - 0.0002f);
        break;
      case 3:
        b.DrawString(font, text, position + new Vector2(0.0f, (float) verticalShadowOffset), shadowColor, 0.0f, Vector2.Zero, scale, SpriteEffects.None, layerDepth - 0.0003f);
        break;
    }
    b.DrawString(font, text, position, color, 0.0f, Vector2.Zero, scale, SpriteEffects.None, layerDepth);
  }

  public static void drawBoldText(
    SpriteBatch b,
    string text,
    SpriteFont font,
    Vector2 position,
    Color color,
    float scale = 1f,
    float layerDepth = -1f,
    int boldnessOffset = 1)
  {
    if ((double) layerDepth == -1.0)
      layerDepth = position.Y / 10000f;
    b.DrawString(font, text, position, color, 0.0f, Vector2.Zero, scale, SpriteEffects.None, layerDepth);
    b.DrawString(font, text, position + new Vector2((float) boldnessOffset, 0.0f), color, 0.0f, Vector2.Zero, scale, SpriteEffects.None, layerDepth);
    b.DrawString(font, text, position + new Vector2((float) boldnessOffset, (float) boldnessOffset), color, 0.0f, Vector2.Zero, scale, SpriteEffects.None, layerDepth);
    b.DrawString(font, text, position + new Vector2(0.0f, (float) boldnessOffset), color, 0.0f, Vector2.Zero, scale, SpriteEffects.None, layerDepth);
  }

  protected static bool _HasNonMousePlacementLeeway(int x, int y, Item item, Farmer f)
  {
    if (!Game1.isCheckingNonMousePlacement)
      return false;
    Point tilePoint = f.TilePoint;
    if (!Utility.withinRadiusOfPlayer(x, y, 2, f))
      return false;
    if (item.Category == -74)
      return true;
    foreach (Point point in Utility.GetPointsOnLine(tilePoint.X, tilePoint.Y, x / 64 /*0x40*/, y / 64 /*0x40*/))
    {
      if (!(point == tilePoint) && !item.canBePlacedHere(f.currentLocation, new Vector2((float) point.X, (float) point.Y), CollisionMask.Buildings | CollisionMask.Flooring | CollisionMask.Furniture | CollisionMask.Objects | CollisionMask.TerrainFeatures | CollisionMask.LocationSpecific))
        return false;
    }
    return true;
  }

  public static bool isPlacementForbiddenHere(GameLocation location)
  {
    return location == null || Utility.isPlacementForbiddenHere(location.name.Value);
  }

  public static bool TryGetPassiveFestivalData(string festivalId, out PassiveFestivalData data)
  {
    if (festivalId != null)
      return DataLoader.PassiveFestivals(Game1.content).TryGetValue(festivalId, out data);
    data = (PassiveFestivalData) null;
    return false;
  }

  /// <summary>Get the passive festival which is active on a given date.</summary>
  /// <param name="dayOfMonth">The day of month to check.</param>
  /// <param name="season">The season to check.</param>
  /// <param name="locationContextId">The location context to check, or <c>null</c> for any location context.</param>
  /// <param name="id">The passive festival ID, if found.</param>
  /// <param name="data">The passive festival data, if found.</param>
  /// <param name="ignoreConditionsCheck">Whether to ignore the custom passive festival conditions, if any.</param>
  public static bool TryGetPassiveFestivalDataForDay(
    int dayOfMonth,
    Season season,
    string locationContextId,
    out string id,
    out PassiveFestivalData data,
    bool ignoreConditionsCheck = false)
  {
    bool flag = true;
    ICollection<string> strings;
    if (dayOfMonth == Game1.dayOfMonth && season == Game1.season)
    {
      strings = (ICollection<string>) Game1.netWorldState.Value.ActivePassiveFestivals;
      flag = false;
    }
    else
      strings = (ICollection<string>) DataLoader.PassiveFestivals(Game1.content).Keys;
    foreach (string str in (IEnumerable<string>) strings)
    {
      id = str;
      if (Utility.TryGetPassiveFestivalData(id, out data) && (!flag || dayOfMonth >= data.StartDay && dayOfMonth <= data.EndDay && season == data.Season && (ignoreConditionsCheck || GameStateQuery.CheckConditions(data.Condition))))
      {
        if (locationContextId == null)
          return true;
        if (data.MapReplacements != null)
        {
          foreach (string key in data.MapReplacements.Keys)
          {
            if (Game1.getLocationFromName(key)?.GetLocationContextId() == locationContextId)
              return true;
          }
        }
      }
    }
    id = (string) null;
    data = (PassiveFestivalData) null;
    return false;
  }

  /// <summary>Get whether there's a passive festival scheduled for today.</summary>
  /// <remarks>This doesn't match active festivals like the Flower Dance; see <see cref="M:StardewValley.Utility.isFestivalDay" /> for those.</remarks>
  public static bool IsPassiveFestivalDay()
  {
    return Utility.TryGetPassiveFestivalDataForDay(Game1.dayOfMonth, Game1.season, (string) null, out string _, out PassiveFestivalData _);
  }

  /// <summary>Get whether there's a passive festival scheduled for the given day.</summary>
  /// <param name="day">The day of month to check.</param>
  /// <param name="season">The season to check.</param>
  /// <param name="locationContextId">The location context to check, or <c>null</c> for any location context.</param>
  /// <remarks>This doesn't match active festivals like the Flower Dance; see <see cref="M:StardewValley.Utility.isFestivalDay(System.Int32,StardewValley.Season)" /> for those.</remarks>
  public static bool IsPassiveFestivalDay(int dayOfMonth, Season season, string locationContextId)
  {
    return Utility.TryGetPassiveFestivalDataForDay(dayOfMonth, season, locationContextId, out string _, out PassiveFestivalData _);
  }

  /// <summary>Get whether a given passive festival is scheduled for today.</summary>
  /// <param name="festivalId">The passive festival ID.</param>
  /// <remarks>This doesn't match active festivals like the Flower Dance; see <see cref="M:StardewValley.Utility.isFestivalDay" /> for those.</remarks>
  public static bool IsPassiveFestivalDay(string festivalId)
  {
    return Game1.netWorldState.Value.ActivePassiveFestivals.Contains(festivalId);
  }

  public static bool IsPassiveFestivalOpen(string festivalId)
  {
    PassiveFestivalData data;
    return Utility.IsPassiveFestivalDay(festivalId) && Utility.TryGetPassiveFestivalData(festivalId, out data) && Game1.timeOfDay >= data.StartTime;
  }

  public static int GetDayOfPassiveFestival(string festivalId)
  {
    PassiveFestivalData data;
    return !Utility.IsPassiveFestivalDay(festivalId) || !Utility.TryGetPassiveFestivalData(festivalId, out data) ? -1 : Game1.dayOfMonth - data.StartDay + 1;
  }

  public static bool isPlacementForbiddenHere(string location_name)
  {
    if (location_name == "AbandonedJojaMart")
      return true;
    foreach (string activePassiveFestival in (IEnumerable<string>) Game1.netWorldState.Value.ActivePassiveFestivals)
    {
      PassiveFestivalData data;
      if (Utility.TryGetPassiveFestivalData(activePassiveFestival, out data) && data.MapReplacements != null)
      {
        foreach (string str in data.MapReplacements.Values)
        {
          if (location_name == str)
            return true;
        }
      }
    }
    return false;
  }

  public static void transferPlacedObjectsFromOneLocationToAnother(
    GameLocation source,
    GameLocation destination,
    Vector2? overflow_chest_position = null,
    GameLocation overflow_chest_location = null)
  {
    if (source == null)
      return;
    List<Item> overflow_items = new List<Item>();
    foreach (Vector2 vector2 in new List<Vector2>((IEnumerable<Vector2>) source.objects.Keys))
    {
      if (source.objects[vector2] != null)
      {
        Object @object = source.objects[vector2];
        int num = destination == null || destination.objects.ContainsKey(vector2) ? 0 : (destination.CanItemBePlacedHere(vector2) ? 1 : 0);
        source.objects.Remove(vector2);
        if (num != 0 && destination != null)
        {
          destination.objects[vector2] = @object;
        }
        else
        {
          overflow_items.Add((Item) @object);
          if (@object is Chest chest)
          {
            List<Item> objList = new List<Item>((IEnumerable<Item>) chest.Items);
            chest.Items.Clear();
            foreach (Item obj in objList)
            {
              if (obj != null)
                overflow_items.Add(obj);
            }
          }
        }
      }
    }
    if (!overflow_chest_position.HasValue)
      return;
    if (overflow_chest_location != null)
    {
      Utility.createOverflowChest(overflow_chest_location, overflow_chest_position.Value, overflow_items);
    }
    else
    {
      if (destination == null)
        return;
      Utility.createOverflowChest(destination, overflow_chest_position.Value, overflow_items);
    }
  }

  public static void createOverflowChest(
    GameLocation destination,
    Vector2 overflow_chest_location,
    List<Item> overflow_items)
  {
    List<Chest> chestList = new List<Chest>();
    foreach (Item overflowItem in overflow_items)
    {
      if (chestList.Count == 0)
        chestList.Add(new Chest(true));
      bool flag = false;
      foreach (Chest chest in chestList)
      {
        if (chest.addItem(overflowItem) == null)
        {
          flag = true;
          break;
        }
      }
      if (!flag)
      {
        Chest chest = new Chest(true);
        chest.addItem(overflowItem);
        chestList.Add(chest);
      }
    }
    for (int index = 0; index < chestList.Count; ++index)
    {
      Chest o = chestList[index];
      Vector2 tileLocation = overflow_chest_location;
      Utility._placeOverflowChestInNearbySpace(destination, tileLocation, (Object) o);
    }
  }

  protected static void _placeOverflowChestInNearbySpace(
    GameLocation location,
    Vector2 tileLocation,
    Object o)
  {
    if (o == null || tileLocation.Equals(Vector2.Zero))
      return;
    int num = 0;
    Queue<Vector2> vector2Queue = new Queue<Vector2>();
    HashSet<Vector2> vector2Set = new HashSet<Vector2>();
    vector2Queue.Enqueue(tileLocation);
    Vector2 vector2 = Vector2.Zero;
    for (; num < 100; ++num)
    {
      vector2 = vector2Queue.Dequeue();
      if (!location.CanItemBePlacedHere(vector2))
      {
        vector2Set.Add(vector2);
        foreach (Vector2 adjacentTileLocation in Utility.getAdjacentTileLocations(vector2))
        {
          if (!vector2Set.Contains(adjacentTileLocation))
            vector2Queue.Enqueue(adjacentTileLocation);
        }
      }
      else
        break;
    }
    if (vector2.Equals(Vector2.Zero) || !location.CanItemBePlacedHere(vector2))
      return;
    o.TileLocation = vector2;
    location.objects.Add(vector2, o);
  }

  public static bool isWithinTileWithLeeway(int x, int y, Item item, Farmer f)
  {
    return Utility.withinRadiusOfPlayer(x, y, 1, f) || Utility._HasNonMousePlacementLeeway(x, y, item, f);
  }

  public static bool playerCanPlaceItemHere(
    GameLocation location,
    Item item,
    int x,
    int y,
    Farmer f,
    bool show_error = false)
  {
    if (Utility.isPlacementForbiddenHere(location) || item == null || item is Tool || Game1.eventUp || f.bathingClothes.Value || f.onBridge.Value || !Utility.isWithinTileWithLeeway(x, y, item, f) && (!(item is Wallpaper) || !(location is DecoratableLocation)) && (!(item is Furniture furniture1) || !location.CanPlaceThisFurnitureHere(furniture1)) || item is Furniture furniture2 && !location.CanFreePlaceFurniture() && !furniture2.IsCloseEnoughToFarmer(f, new int?(x / 64 /*0x40*/), new int?(y / 64 /*0x40*/)))
      return false;
    Vector2 tile = new Vector2((float) (x / 64 /*0x40*/), (float) (y / 64 /*0x40*/));
    return item.canBePlacedHere(location, tile, showError: show_error) && item.isPlaceable();
  }

  public static string GetDoubleWideVersionOfBed(string bedId)
  {
    int result;
    if (int.TryParse(bedId, out result))
      return (result + 4).ToString();
    return bedId == "BluePinstripeBed" ? "BluePinstripeDoubleBed" : BedFurniture.DOUBLE_BED_INDEX;
  }

  public static int getDirectionFromChange(Vector2 current, Vector2 previous)
  {
    if ((double) current.X > (double) previous.X)
      return 1;
    if ((double) current.X < (double) previous.X)
      return 3;
    if ((double) current.Y > (double) previous.Y)
      return 2;
    return (double) current.Y < (double) previous.Y ? 0 : -1;
  }

  public static bool doesRectangleIntersectTile(Microsoft.Xna.Framework.Rectangle r, int tileX, int tileY)
  {
    Microsoft.Xna.Framework.Rectangle rectangle = new Microsoft.Xna.Framework.Rectangle(tileX * 64 /*0x40*/, tileY * 64 /*0x40*/, 64 /*0x40*/, 64 /*0x40*/);
    return r.Intersects(rectangle);
  }

  public static bool IsHospitalVisitDay(string character_name)
  {
    try
    {
      string str;
      if (Game1.content.Load<Dictionary<string, string>>("Characters\\schedules\\" + character_name).TryGetValue($"{Game1.currentSeason}_{Game1.dayOfMonth.ToString()}", out str))
      {
        if (str.Contains("Hospital"))
          return true;
      }
    }
    catch (Exception ex)
    {
    }
    return false;
  }

  /// <summary>Get all characters of any type (including villagers, horses, pets, monsters, player children, etc).</summary>
  /// <remarks>This creates a new list each time it's called, which is inefficient for hot paths. Consider using <see cref="M:StardewValley.Utility.ForEachCharacter(System.Func{StardewValley.NPC,System.Boolean},System.Boolean)" /> if you don't need an actual list (e.g. you just need to iterate them once).</remarks>
  public static List<NPC> getAllCharacters()
  {
    List<NPC> list = new List<NPC>();
    Utility.ForEachCharacter((Func<NPC, bool>) (npc =>
    {
      list.Add(npc);
      return true;
    }));
    return list;
  }

  /// <summary>Get all villager NPCs (excluding horses, pets, monsters, player children, etc).</summary>
  /// <remarks>This creates a new list each time it's called, which is inefficient for hot paths. Consider using <see cref="M:StardewValley.Utility.ForEachVillager(System.Func{StardewValley.NPC,System.Boolean},System.Boolean)" /> if you don't need an actual list (e.g. you just need to iterate them once).</remarks>
  public static List<NPC> getAllVillagers()
  {
    List<NPC> list = new List<NPC>();
    Utility.ForEachVillager((Func<NPC, bool>) (npc =>
    {
      list.Add(npc);
      return true;
    }));
    return list;
  }

  /// <summary>Apply special conversion rules when equipping an item. For example, this is used to convert a Copper Pan tool into a hat.</summary>
  /// <param name="placedItem">The item being equipped.</param>
  public static Item PerformSpecialItemPlaceReplacement(Item placedItem)
  {
    Item obj;
    switch (placedItem?.QualifiedItemId)
    {
      case "(T)Pan":
        obj = ItemRegistry.Create("(H)71");
        break;
      case "(T)SteelPan":
        obj = ItemRegistry.Create("(H)SteelPanHat");
        break;
      case "(T)GoldPan":
        obj = ItemRegistry.Create("(H)GoldPanHat");
        break;
      case "(T)IridiumPan":
        obj = ItemRegistry.Create("(H)IridiumPanHat");
        break;
      case "(O)71":
        obj = ItemRegistry.Create("(P)15");
        break;
      default:
        return placedItem;
    }
    obj.modData.CopyFrom(placedItem.modData);
    if (obj is Hat hat && placedItem is Tool tool)
    {
      hat.enchantments.AddRange((IEnumerable<BaseEnchantment>) tool.enchantments);
      hat.previousEnchantments.AddRange((IEnumerable<string>) tool.previousEnchantments);
    }
    return obj;
  }

  /// <summary>Apply special conversion rules when un-equipping an item. For example, this is used to convert a Copper Pan hat back into a tool.</summary>
  /// <param name="placedItem">The item being equipped.</param>
  public static Item PerformSpecialItemGrabReplacement(Item heldItem)
  {
    Item obj;
    switch (heldItem?.QualifiedItemId)
    {
      case "(P)15":
        Object @object = ItemRegistry.Create<Object>("(O)71");
        @object.questItem.Value = true;
        @object.questId.Value = "102";
        obj = (Item) @object;
        break;
      case "(H)71":
        obj = ItemRegistry.Create("(T)Pan");
        break;
      case "(H)SteelPanHat":
        obj = ItemRegistry.Create("(T)SteelPan");
        break;
      case "(H)GoldPanHat":
        obj = ItemRegistry.Create("(T)GoldPan");
        break;
      case "(H)IridiumPanHat":
        obj = ItemRegistry.Create("(T)IridiumPan");
        break;
      default:
        return heldItem;
    }
    obj.modData.CopyFrom(heldItem.modData);
    if (obj is Pan pan && heldItem is Hat hat)
    {
      pan.enchantments.AddRange((IEnumerable<BaseEnchantment>) hat.enchantments);
      pan.previousEnchantments.AddRange((IEnumerable<string>) hat.previousEnchantments);
    }
    return obj;
  }

  /// <summary>Perform an action for every item stored in chests or storage furniture, or placed on furniture.</summary>
  /// <param name="action">The action to perform.</param>
  /// <remarks>See also <see cref="M:StardewValley.Utility.ForEachItem(System.Func{StardewValley.Item,System.Boolean})" /> to iterate all items, regardless of where they are.</remarks>
  public static void iterateChestsAndStorage(Action<Item> action)
  {
    Utility.ForEachLocation((Func<GameLocation, bool>) (l =>
    {
      Chest fridge = l.GetFridge(false);
      fridge?.ForEachItem(new ForEachItemDelegate(Handle), (GetForEachItemPathDelegate) null);
      foreach (Object @object in l.objects.Values)
      {
        if (@object != fridge)
        {
          if (@object is Chest)
            @object.ForEachItem(new ForEachItemDelegate(Handle), (GetForEachItemPathDelegate) null);
          else if (@object.heldObject.Value is Chest chest2)
            chest2.ForEachItem(new ForEachItemDelegate(Handle), (GetForEachItemPathDelegate) null);
        }
      }
      foreach (Item obj in l.furniture)
        obj.ForEachItem(new ForEachItemDelegate(Handle), (GetForEachItemPathDelegate) null);
      foreach (Building building in l.buildings)
      {
        foreach (Item buildingChest in building.buildingChests)
          buildingChest.ForEachItem(new ForEachItemDelegate(Handle), (GetForEachItemPathDelegate) null);
      }
      return true;
    }));
    foreach (Item returnedDonation in Game1.player.team.returnedDonations)
    {
      if (returnedDonation != null)
        action(returnedDonation);
    }
    foreach (IEnumerable<Item> objs in Game1.player.team.globalInventories.Values)
    {
      foreach (Item obj in objs)
      {
        if (obj != null)
          action(obj);
      }
    }
    foreach (SpecialOrder specialOrder in Game1.player.team.specialOrders)
    {
      foreach (Item donatedItem in specialOrder.donatedItems)
      {
        if (donatedItem != null)
          action(donatedItem);
      }
    }

    bool Handle(in StardewValley.Internal.ForEachItemContext context)
    {
      action(context.Item);
      return true;
    }
  }

  public static Item removeItemFromInventory(int whichItemIndex, IList<Item> items)
  {
    if (whichItemIndex < 0 || whichItemIndex >= items.Count || items[whichItemIndex] == null)
      return (Item) null;
    Item obj = items[whichItemIndex];
    if (whichItemIndex == Game1.player.CurrentToolIndex && items.Equals((object) Game1.player.Items) && obj != null)
      obj.actionWhenStopBeingHeld(Game1.player);
    items[whichItemIndex] = (Item) null;
    return obj;
  }

  /// <summary>Get a random available NPC listed in <c>Data/Characters</c> whose <see cref="F:StardewValley.GameData.Characters.CharacterData.HomeRegion" /> is <see cref="F:StardewValley.NPC.region_town" />.</summary>
  /// <param name="random">The RNG with which to choose an NPC.</param>
  /// <remarks>See also <see cref="M:StardewValley.Utility.getRandomNpcFromHomeRegion(System.String,System.Random)" />.</remarks>
  public static NPC getRandomTownNPC(Random random = null)
  {
    return Utility.getRandomNpcFromHomeRegion("Town", random);
  }

  /// <summary>Get a random available NPC listed in <c>Data/Characters</c> with a given <see cref="F:StardewValley.GameData.Characters.CharacterData.HomeRegion" />.</summary>
  /// <param name="region">The region to match.</param>
  /// <param name="random">The RNG with which to choose an NPC.</param>
  public static NPC getRandomNpcFromHomeRegion(string region, Random random = null)
  {
    return Utility.GetRandomNpc((Func<string, CharacterData, bool>) ((name, data) => data.HomeRegion == region), random);
  }

  /// <summary>Get a random available NPC listed in <c>Data/Characters</c> which can give or receive gifts at the Feast of the Winter Star.</summary>
  /// <param name="ignoreNpc">Whether to exclude an NPC from the selection.</param>
  public static NPC GetRandomWinterStarParticipant(Func<string, bool> ignoreNpc = null)
  {
    return Utility.GetRandomNpc((Func<string, CharacterData, bool>) ((name, data) =>
    {
      Func<string, bool> func = ignoreNpc;
      if ((func != null ? (!func(name) ? 1 : 0) : 1) == 0)
        return false;
      return data.WinterStarParticipant == null ? data.HomeRegion == "Town" : GameStateQuery.CheckConditions(data.WinterStarParticipant);
    }), Utility.CreateRandom((double) (Game1.uniqueIDForThisGame / 2UL), (double) Game1.year, (double) Game1.player.UniqueMultiplayerID));
  }

  /// <summary>Get a random available NPC listed in <c>Data/Characters</c>.</summary>
  /// <param name="match">A predicate matching the NPCs to include, or <c>null</c> to allow any valid match.</param>
  /// <param name="random">The RNG with which to choose an NPC.</param>
  /// <param name="mustBeSocial">Whether to only include NPCs whose <see cref="P:StardewValley.NPC.CanSocialize" /> property is true.</param>
  public static NPC GetRandomNpc(
    Func<string, CharacterData, bool> match = null,
    Random random = null,
    bool mustBeSocial = true)
  {
    List<string> stringList = new List<string>();
    foreach (KeyValuePair<string, CharacterData> keyValuePair in (IEnumerable<KeyValuePair<string, CharacterData>>) Game1.characterData)
    {
      if (match == null || match(keyValuePair.Key, keyValuePair.Value))
        stringList.Add(keyValuePair.Key);
    }
    random = random ?? Game1.random;
    while (stringList.Count > 0)
    {
      int index = random.Next(stringList.Count);
      NPC characterFromName = Game1.getCharacterFromName(stringList[index]);
      if (characterFromName != null && (!mustBeSocial || characterFromName.CanSocialize))
        return characterFromName;
      stringList.RemoveAt(index);
    }
    return (NPC) null;
  }

  public static bool foundAllStardrops(Farmer who = null)
  {
    if (who == null)
      who = Game1.player;
    if (who.mailReceived.Contains("gotMaxStamina"))
      return true;
    return who.hasOrWillReceiveMail("CF_Fair") && who.hasOrWillReceiveMail("CF_Fish") && (who.hasOrWillReceiveMail("CF_Mines") || who.chestConsumedMineLevels.GetValueOrDefault(100)) && who.hasOrWillReceiveMail("CF_Sewer") && who.hasOrWillReceiveMail("museumComplete") && who.hasOrWillReceiveMail("CF_Spouse") && who.hasOrWillReceiveMail("CF_Statue");
  }

  public static int numStardropsFound(Farmer who = null)
  {
    if (who == null)
      who = Game1.player;
    int num = 0;
    if (who.hasOrWillReceiveMail("CF_Fair"))
      ++num;
    if (who.hasOrWillReceiveMail("CF_Fish"))
      ++num;
    if (who.hasOrWillReceiveMail("CF_Mines") || who.chestConsumedMineLevels.GetValueOrDefault(100))
      ++num;
    if (who.hasOrWillReceiveMail("CF_Sewer"))
      ++num;
    if (who.hasOrWillReceiveMail("museumComplete"))
      ++num;
    if (who.hasOrWillReceiveMail("CF_Spouse"))
      ++num;
    if (who.hasOrWillReceiveMail("CF_Statue"))
      ++num;
    return num;
  }

  /// <summary>
  /// Can range from 0 to 21.
  /// 
  ///    if (points &gt;= 12) 4
  ///     if (points &gt;= 8) 3
  ///   if (points &gt;= 4)  2
  ///    else 1
  /// those are the number of candles that will be light on grandpa's shrine.
  /// </summary>
  /// <returns></returns>
  public static int getGrandpaScore()
  {
    int grandpaScore = 0;
    if (Game1.player.totalMoneyEarned >= 50000U)
      ++grandpaScore;
    if (Game1.player.totalMoneyEarned >= 100000U)
      ++grandpaScore;
    if (Game1.player.totalMoneyEarned >= 200000U)
      ++grandpaScore;
    if (Game1.player.totalMoneyEarned >= 300000U)
      ++grandpaScore;
    if (Game1.player.totalMoneyEarned >= 500000U)
      ++grandpaScore;
    if (Game1.player.totalMoneyEarned >= 1000000U)
      grandpaScore += 2;
    if (Game1.player.achievements.Contains(5))
      ++grandpaScore;
    if (Game1.player.hasSkullKey)
      ++grandpaScore;
    int num = Game1.isLocationAccessible("CommunityCenter") ? 1 : 0;
    if (num != 0 || Game1.player.hasCompletedCommunityCenter())
      ++grandpaScore;
    if (num != 0)
      grandpaScore += 2;
    if (Game1.player.isMarriedOrRoommates() && Utility.getHomeOfFarmer(Game1.player).upgradeLevel >= 2)
      ++grandpaScore;
    if (Game1.player.hasRustyKey)
      ++grandpaScore;
    if (Game1.player.achievements.Contains(26))
      ++grandpaScore;
    if (Game1.player.achievements.Contains(34))
      ++grandpaScore;
    int friendsWithinThisRange = Utility.getNumberOfFriendsWithinThisRange(Game1.player, 1975, 999999);
    if (friendsWithinThisRange >= 5)
      ++grandpaScore;
    if (friendsWithinThisRange >= 10)
      ++grandpaScore;
    int level = Game1.player.Level;
    if (level >= 15)
      ++grandpaScore;
    if (level >= 25)
      ++grandpaScore;
    if (Game1.player.mailReceived.Contains("petLoveMessage"))
      ++grandpaScore;
    return grandpaScore;
  }

  public static int getGrandpaCandlesFromScore(int score)
  {
    if (score >= 12)
      return 4;
    if (score >= 8)
      return 3;
    return score >= 4 ? 2 : 1;
  }

  public static bool canItemBeAddedToThisInventoryList(Item i, IList<Item> list, int listMaxSpace = -1)
  {
    if (listMaxSpace != -1 && list.Count < listMaxSpace)
      return true;
    int stack = i.Stack;
    foreach (Item obj in (IEnumerable<Item>) list)
    {
      if (obj == null)
        return true;
      if (obj.canStackWith((ISalable) i) && obj.getRemainingStackSpace() > 0)
      {
        stack -= obj.getRemainingStackSpace();
        if (stack <= 0)
          return true;
      }
    }
    return false;
  }

  /// <summary>Parse a raw direction string into a number matching one of the constants like <see cref="F:StardewValley.Game1.up" />.</summary>
  /// <param name="direction">The raw direction value. This can be a case-insensitive name (<c>up</c>, <c>down</c>, <c>left</c>, or <c>right</c>) or a numeric value matching a contant like <see cref="F:StardewValley.Game1.up" />.</param>
  /// <param name="parsed">The parsed value matching a constant like <see cref="F:StardewValley.Game1.up" />, or <c>-1</c> if not valid.</param>
  /// <returns>Returns whether the value was successfully parsed.</returns>
  public static bool TryParseDirection(string direction, out int parsed)
  {
    if (string.IsNullOrWhiteSpace(direction))
    {
      parsed = -1;
      return false;
    }
    if (direction.EqualsIgnoreCase("up"))
    {
      parsed = 0;
      return true;
    }
    if (direction.EqualsIgnoreCase("down"))
    {
      parsed = 2;
      return true;
    }
    if (direction.EqualsIgnoreCase("left"))
    {
      parsed = 3;
      return true;
    }
    if (direction.EqualsIgnoreCase("right"))
    {
      parsed = 1;
      return true;
    }
    if (int.TryParse(direction, out parsed))
    {
      switch (parsed)
      {
        case 0:
        case 1:
        case 2:
        case 3:
          return true;
      }
    }
    parsed = -1;
    return false;
  }

  public static int GetNumberOfItemThatCanBeAddedToThisInventoryList(
    Item item,
    IList<Item> list,
    int listMaxItems)
  {
    int thisInventoryList = 0;
    foreach (Item obj in (IEnumerable<Item>) list)
    {
      if (obj == null)
        thisInventoryList += item.maximumStackSize();
      else if (obj != null && obj.canStackWith((ISalable) item) && obj.getRemainingStackSpace() > 0)
        thisInventoryList += obj.getRemainingStackSpace();
    }
    for (int index = 0; index < listMaxItems - list.Count; ++index)
      thisInventoryList += item.maximumStackSize();
    return thisInventoryList;
  }

  /// <summary>Add an item to an inventory list if there's room for it.</summary>
  /// <param name="i">The item to add.</param>
  /// <param name="list">The inventory list to add it to.</param>
  /// <param name="listMaxSpace">The maximum number of item slots allowed in the <paramref name="list" />, or <c>-1</c> for no limit.</param>
  /// <returns>If the item was fully added to the inventory, returns <c>null</c>. Else returns the input item with its stack reduced to the amount that couldn't be added.</returns>
  public static Item addItemToThisInventoryList(Item i, IList<Item> list, int listMaxSpace = -1)
  {
    i.FixStackSize();
    foreach (Item obj in (IEnumerable<Item>) list)
    {
      if (obj != null && obj.canStackWith((ISalable) i) && obj.getRemainingStackSpace() > 0)
      {
        int amount = i.Stack - obj.addToStack(i);
        if (i.ConsumeStack(amount) == null)
          return (Item) null;
      }
    }
    for (int index = list.Count - 1; index >= 0; --index)
    {
      if (list[index] == null)
      {
        if (i.Stack > i.maximumStackSize())
        {
          list[index] = i.getOne();
          list[index].Stack = i.maximumStackSize();
          if (i is Object @object)
            @object.stack.Value -= i.maximumStackSize();
          else
            i.Stack -= i.maximumStackSize();
        }
        else
        {
          list[index] = i;
          return (Item) null;
        }
      }
    }
    while (listMaxSpace != -1 && list.Count < listMaxSpace)
    {
      if (i.Stack > i.maximumStackSize())
      {
        Item one = i.getOne();
        one.Stack = i.maximumStackSize();
        if (i is Object @object)
          @object.stack.Value -= i.maximumStackSize();
        else
          i.Stack -= i.maximumStackSize();
        list.Add(one);
      }
      else
      {
        list.Add(i);
        return (Item) null;
      }
    }
    return i;
  }

  /// <summary>Add an item to an inventory list at a specific index position. If there's already an item at that position, the stacks are merged (if possible) else they're swapped.</summary>
  /// <param name="item">The item to add.</param>
  /// <param name="position">The index position within the list at which to add the item.</param>
  /// <param name="items">The inventory list to add it to.</param>
  /// <param name="onAddFunction">The callback to invoke when an item is added to the inventory.</param>
  /// <returns>If the item was fully added to the inventory, returns <c>null</c>. If it replaced an item stack previously at that position, returns the replaced item stack. Else returns the input item with its stack reduced to the amount that couldn't be added.</returns>
  public static Item addItemToInventory(
    Item item,
    int position,
    IList<Item> items,
    ItemGrabMenu.behaviorOnItemSelect onAddFunction = null)
  {
    bool flag = items.Equals((object) Game1.player.Items);
    if (flag)
    {
      bool needsInventorySpace;
      Game1.player.GetItemReceiveBehavior(item, out needsInventorySpace, out bool _);
      if (!needsInventorySpace)
      {
        Game1.player.OnItemReceived(item, item.Stack, (Item) null);
        return (Item) null;
      }
    }
    if (position < 0 || position >= items.Count)
      return item;
    if (items[position] == null)
    {
      items[position] = item;
      if (flag)
        Game1.player.OnItemReceived(item, item.Stack, (Item) null);
      if (onAddFunction != null)
        onAddFunction(item, (Farmer) null);
      return (Item) null;
    }
    if (item.canStackWith((ISalable) items[position]))
    {
      int stack1 = item.Stack;
      int stack2 = items[position].addToStack(item);
      if (flag)
        Game1.player.OnItemReceived(item, stack1 - stack2, items[position]);
      if (stack2 <= 0)
        return (Item) null;
      item.Stack = stack2;
      if (onAddFunction != null)
        onAddFunction(item, (Farmer) null);
      return item;
    }
    Item inventory = items[position];
    if (position == Game1.player.CurrentToolIndex && items.Equals((object) Game1.player.Items) && inventory != null)
    {
      inventory.actionWhenStopBeingHeld(Game1.player);
      item.actionWhenBeingHeld(Game1.player);
    }
    items[position] = item;
    if (flag)
      Game1.player.OnItemReceived(item, item.Stack, (Item) null);
    if (onAddFunction != null)
      onAddFunction(item, (Farmer) null);
    return inventory;
  }

  /// <summary>
  /// called on monster kill, breakable container open, tree chop, tree shake w/ seed, diggable spots. ChanceModifier is adjusted per each source to account for the frequency of source hits
  /// </summary>
  public static bool trySpawnRareObject(
    Farmer who,
    Vector2 position,
    GameLocation location,
    double chanceModifier = 1.0,
    double dailyLuckWeight = 1.0,
    int groundLevel = -1,
    Random random = null)
  {
    if (random == null)
      random = Game1.random;
    double num1 = 1.0;
    if (who != null)
      num1 = 1.0 + who.team.AverageDailyLuck() * dailyLuckWeight;
    int num2 = 0;
    if (who != null && who.stats.Get(StatKeys.Mastery(0)) > 0U && random.NextDouble() < 0.001 * chanceModifier * num1)
      Game1.createItemDebris(ItemRegistry.Create("(O)GoldenAnimalCracker"), position, -1, location, groundLevel);
    if (Game1.stats.DaysPlayed > 2U && random.NextDouble() < 0.002 * chanceModifier)
      Game1.createItemDebris(Utility.getRandomCosmeticItem(random), position, -1, location, groundLevel);
    if (Game1.stats.DaysPlayed <= 2U)
      return num2 != 0;
    if (random.NextDouble() >= 0.0006 * chanceModifier)
      return num2 != 0;
    Game1.createItemDebris(ItemRegistry.Create("(O)SkillBook_" + random.Next(5).ToString()), position, -1, location, groundLevel);
    return num2 != 0;
  }

  public static bool spawnObjectAround(
    Vector2 tileLocation,
    Object o,
    GameLocation l,
    bool playSound = true,
    Action<Object> modifyObject = null)
  {
    if (o == null || l == null || tileLocation.Equals(Vector2.Zero))
      return false;
    int num = 0;
    Queue<Vector2> vector2Queue = new Queue<Vector2>();
    HashSet<Vector2> vector2Set = new HashSet<Vector2>();
    vector2Queue.Enqueue(tileLocation);
    Vector2 vector2_1 = Vector2.Zero;
    for (; num < 100; ++num)
    {
      vector2_1 = vector2Queue.Dequeue();
      if (!l.CanItemBePlacedHere(vector2_1))
      {
        vector2Set.Add(vector2_1);
        foreach (Vector2 vector2_2 in Utility.getAdjacentTileLocations(vector2_1).OrderBy<Vector2, Guid>((Func<Vector2, Guid>) (a => Guid.NewGuid())).ToArray<Vector2>())
        {
          if (!vector2Set.Contains(vector2_2))
            vector2Queue.Enqueue(vector2_2);
        }
      }
      else
        break;
    }
    o.isSpawnedObject.Value = true;
    o.canBeGrabbed.Value = true;
    o.TileLocation = vector2_1;
    if (modifyObject != null)
      modifyObject(o);
    if (vector2_1.Equals(Vector2.Zero) || !l.CanItemBePlacedHere(vector2_1))
      return false;
    l.objects.Add(vector2_1, o);
    if (playSound)
      l.playSound("coin");
    if (l.Equals(Game1.currentLocation))
      l.temporarySprites.Add(new TemporaryAnimatedSprite(5, vector2_1 * 64f, Color.White));
    return true;
  }

  public static bool IsGeode(Item item, bool disallow_special_geodes = false)
  {
    if (!item.HasTypeObject() || disallow_special_geodes && item.HasContextTag("geode_crusher_ignored"))
      return false;
    if (item.QualifiedItemId.Contains("MysteryBox"))
      return true;
    ObjectData objectData;
    if (!Game1.objectData.TryGetValue(item.ItemId, out objectData))
      return false;
    if (objectData.GeodeDropsDefaultItems)
      return true;
    List<ObjectGeodeDropData> geodeDrops = objectData.GeodeDrops;
    // ISSUE: explicit non-virtual call
    return geodeDrops != null && __nonvirtual (geodeDrops.Count) > 0;
  }

  public static Item getRandomCosmeticItem(Random r)
  {
    if (r.NextDouble() < 0.2)
    {
      if (r.NextDouble() < 0.05)
        return ItemRegistry.Create("(F)1369");
      Item randomCosmeticItem = (Item) null;
      switch (r.Next(3))
      {
        case 0:
          randomCosmeticItem = ItemRegistry.Create(Utility.getRandomSingleTileFurniture(r));
          break;
        case 1:
          randomCosmeticItem = ItemRegistry.Create("(F)" + r.Next(1362, 1370).ToString());
          break;
        case 2:
          randomCosmeticItem = ItemRegistry.Create("(F)" + r.Next(1376, 1391).ToString());
          break;
      }
      if (randomCosmeticItem == null || randomCosmeticItem.Name.Contains("Error"))
        randomCosmeticItem = ItemRegistry.Create("(F)1369");
      return randomCosmeticItem;
    }
    if (r.NextDouble() < 0.25)
    {
      List<string> stringList = new List<string>()
      {
        "(H)45",
        "(H)46",
        "(H)47",
        "(H)49",
        "(H)52",
        "(H)53",
        "(H)54",
        "(H)55",
        "(H)57",
        "(H)58",
        "(H)59",
        "(H)62",
        "(H)63",
        "(H)68",
        "(H)69",
        "(H)70",
        "(H)84",
        "(H)85",
        "(H)87",
        "(H)88",
        "(H)89",
        "(H)90"
      };
      return ItemRegistry.Create(stringList[r.Next(stringList.Count)]);
    }
    return ItemRegistry.Create("(S)" + Utility.getRandomIntWithExceptions(r, 1112, 1291, new List<int>()
    {
      1038,
      1041,
      1129,
      1130,
      1132,
      1133,
      1136,
      1152,
      1176,
      1177,
      1201,
      1202,
      1127
    }).ToString());
  }

  public static int getRandomIntWithExceptions(
    Random r,
    int minValue,
    int maxValueExclusive,
    List<int> exceptions)
  {
    if (r == null)
      r = Game1.random;
    int intWithExceptions = r.Next(minValue, maxValueExclusive);
    while (exceptions != null && exceptions.Contains(intWithExceptions))
      intWithExceptions = r.Next(minValue, maxValueExclusive);
    return intWithExceptions;
  }

  public static bool tryRollMysteryBox(double baseChance, Random r = null)
  {
    if (!Game1.MasterPlayer.mailReceived.Contains("sawQiPlane"))
      return false;
    if (r == null)
      r = Game1.random;
    if (Game1.player.stats.Get("Book_Mystery") > 0U)
      baseChance *= 0.88;
    else
      baseChance *= 0.66;
    return r.NextDouble() < baseChance;
  }

  public static Item getTreasureFromGeode(Item geode)
  {
    if (!Utility.IsGeode(geode))
      return (Item) null;
    try
    {
      string qualifiedItemId = geode.QualifiedItemId;
      Random random = Utility.CreateRandom(qualifiedItemId.Contains("MysteryBox") ? (double) Game1.stats.Get("MysteryBoxesOpened") : (double) Game1.stats.GeodesCracked, (double) (Game1.uniqueIDForThisGame / 2UL), (double) ((int) Game1.player.uniqueMultiplayerID.Value / 2));
      int num1 = random.Next(1, 10);
      for (int index = 0; index < num1; ++index)
        random.NextDouble();
      int num2 = random.Next(1, 10);
      for (int index = 0; index < num2; ++index)
        random.NextDouble();
      if (qualifiedItemId.Contains("MysteryBox"))
      {
        if (Game1.stats.Get("MysteryBoxesOpened") > 10U || qualifiedItemId == "(O)GoldenMysteryBox")
        {
          double num3 = qualifiedItemId == "(O)GoldenMysteryBox" ? 2.0 : 1.0;
          if (qualifiedItemId == "(O)GoldenMysteryBox")
          {
            if (Game1.player.stats.Get(StatKeys.Mastery(0)) > 0U && random.NextBool(0.005))
              return ItemRegistry.Create("(O)GoldenAnimalCracker");
            if (random.NextBool(0.005))
              return ItemRegistry.Create("(BC)272");
          }
          if (random.NextBool(0.002 * num3))
            return ItemRegistry.Create("(O)279");
          if (random.NextBool(0.004 * num3))
            return ItemRegistry.Create("(O)74");
          if (random.NextBool(0.008 * num3))
            return ItemRegistry.Create("(O)166");
          if (random.NextBool(0.01 * num3 + (Game1.player.mailReceived.Contains("GotMysteryBook") ? 0.0 : 0.0004 * (double) Game1.stats.Get("MysteryBoxesOpened"))))
          {
            if (Game1.player.mailReceived.Contains("GotMysteryBook"))
              return ItemRegistry.Create(random.Choose<string>("(O)PurpleBook", "(O)Book_Mystery"));
            Game1.player.mailReceived.Add("GotMysteryBook");
            return ItemRegistry.Create("(O)Book_Mystery");
          }
          if (random.NextBool(0.01 * num3))
            return ItemRegistry.Create(random.Choose<string>("(O)797", "(O)373"));
          if (random.NextBool(0.01 * num3))
            return ItemRegistry.Create("(H)MysteryHat");
          if (random.NextBool(0.01 * num3))
            return ItemRegistry.Create("(S)MysteryShirt");
          if (random.NextBool(0.01 * num3))
            return ItemRegistry.Create("(WP)MoreWalls:11");
          if (random.NextBool(0.1) || qualifiedItemId == "(O)GoldenMysteryBox")
          {
            switch (random.Next(15))
            {
              case 0:
                return ItemRegistry.Create("(O)288", 5);
              case 1:
                return ItemRegistry.Create("(O)253", 3);
              case 2:
                return Game1.player.GetUnmodifiedSkillLevel(1) >= 6 && random.NextBool() ? ItemRegistry.Create(random.Choose<string>("(O)687", "(O)695")) : ItemRegistry.Create("(O)242", 2);
              case 3:
                return ItemRegistry.Create("(O)204", 2);
              case 4:
                return ItemRegistry.Create("(O)369", 20);
              case 5:
                return ItemRegistry.Create("(O)466", 20);
              case 6:
                return ItemRegistry.Create("(O)773", 2);
              case 7:
                return ItemRegistry.Create("(O)688", 3);
              case 8:
                return ItemRegistry.Create("(O)" + random.Next(628, 634).ToString());
              case 9:
                return ItemRegistry.Create("(O)" + Crop.getRandomLowGradeCropForThisSeason(Game1.season), 20);
              case 10:
                return random.NextBool() ? ItemRegistry.Create("(W)60") : ItemRegistry.Create(random.Choose<string>("(O)533", "(O)534"));
              case 11:
                return ItemRegistry.Create("(O)621");
              case 12:
                return ItemRegistry.Create("(O)MysteryBox", random.Next(3, 5));
              case 13:
                return ItemRegistry.Create("(O)SkillBook_" + random.Next(5).ToString());
              case 14:
                return Utility.getRaccoonSeedForCurrentTimeOfYear(Game1.player, random, 8);
            }
          }
        }
        switch (random.Next(14))
        {
          case 0:
            return ItemRegistry.Create("(O)395", 3);
          case 1:
            return ItemRegistry.Create("(O)287", 5);
          case 2:
            return ItemRegistry.Create("(O)" + Crop.getRandomLowGradeCropForThisSeason(Game1.season), 8);
          case 3:
            return ItemRegistry.Create("(O)" + random.Next(727, 734).ToString());
          case 4:
            return ItemRegistry.Create("(O)" + Utility.getRandomIntWithExceptions(random, 194, 240 /*0xF0*/, new List<int>()
            {
              217
            }).ToString());
          case 5:
            return ItemRegistry.Create("(O)709", 10);
          case 6:
            return ItemRegistry.Create("(O)369", 10);
          case 7:
            return ItemRegistry.Create("(O)466", 10);
          case 8:
            return ItemRegistry.Create("(O)688");
          case 9:
            return ItemRegistry.Create("(O)689");
          case 10:
            return ItemRegistry.Create("(O)770", 10);
          case 11:
            return ItemRegistry.Create("(O)MixedFlowerSeeds", 10);
          case 12:
            if (!random.NextBool(0.4))
              return ItemRegistry.Create("(O)MysteryBox", 2);
            switch (random.Next(4))
            {
              case 0:
                return (Item) ItemRegistry.Create<Ring>("(O)525");
              case 1:
                return (Item) ItemRegistry.Create<Ring>("(O)529");
              case 2:
                return (Item) ItemRegistry.Create<Ring>("(O)888");
              default:
                return (Item) ItemRegistry.Create<Ring>("(O)" + random.Next(531, 533).ToString());
            }
          case 13:
            return ItemRegistry.Create("(O)690");
          default:
            return ItemRegistry.Create("(O)382");
        }
      }
      else
      {
        if (random.NextBool(0.1) && Game1.player.team.SpecialOrderRuleActive("DROP_QI_BEANS"))
          return ItemRegistry.Create("(O)890", random.NextBool(0.25) ? 5 : 1);
        ObjectData objectData;
        if (Game1.objectData.TryGetValue(geode.ItemId, out objectData))
        {
          List<ObjectGeodeDropData> geodeDrops = objectData.GeodeDrops;
          // ISSUE: explicit non-virtual call
          if ((geodeDrops != null ? (__nonvirtual (geodeDrops.Count) > 0 ? 1 : 0) : 0) != 0 && (!objectData.GeodeDropsDefaultItems || random.NextBool()))
          {
            foreach (ObjectGeodeDropData objectGeodeDropData in (IEnumerable<ObjectGeodeDropData>) objectData.GeodeDrops.OrderBy<ObjectGeodeDropData, int>((Func<ObjectGeodeDropData, int>) (p => p.Precedence)))
            {
              ObjectGeodeDropData drop = objectGeodeDropData;
              if (random.NextBool(drop.Chance) && (drop.Condition == null || GameStateQuery.CheckConditions(drop.Condition, random: random)))
              {
                Item treasureFromGeode = ItemQueryResolver.TryResolveRandomItem((ISpawnItemData) drop, new ItemQueryContext((GameLocation) null, (Farmer) null, random, $"object '{geode.ItemId}' > geode drop '{drop.Id}'"), logError: (Action<string, string>) ((query, error) => Game1.log.Error($"Geode item '{geode.QualifiedItemId}' failed parsing item query '{query}' for {"GeodeDrops"} entry '{drop.Id}': {error}")));
                if (treasureFromGeode != null)
                {
                  if (drop.SetFlagOnPickup != null)
                    treasureFromGeode.SetFlagOnPickup = drop.SetFlagOnPickup;
                  return treasureFromGeode;
                }
              }
            }
          }
        }
        int amount = random.Next(3) * 2 + 1;
        if (random.NextBool(0.1))
          amount = 10;
        if (random.NextBool(0.01))
          amount = 20;
        if (random.NextBool())
        {
          switch (random.Next(4))
          {
            case 0:
            case 1:
              return ItemRegistry.Create("(O)390", amount);
            case 2:
              return ItemRegistry.Create("(O)330");
            default:
              switch (qualifiedItemId)
              {
                case "(O)749":
                  return ItemRegistry.Create("(O)" + (82 + random.Next(3) * 2).ToString());
                case "(O)535":
                  return ItemRegistry.Create("(O)86");
                case "(O)536":
                  return ItemRegistry.Create("(O)84");
                default:
                  return ItemRegistry.Create("(O)82");
              }
          }
        }
        else
        {
          switch (qualifiedItemId)
          {
            case "(O)535":
              switch (random.Next(3))
              {
                case 0:
                  return ItemRegistry.Create("(O)378", amount);
                case 1:
                  return ItemRegistry.Create(Game1.player.deepestMineLevel > 25 ? "(O)380" : "(O)378", amount);
                default:
                  return ItemRegistry.Create("(O)382", amount);
              }
            case "(O)536":
              switch (random.Next(4))
              {
                case 0:
                  return ItemRegistry.Create("(O)378", amount);
                case 1:
                  return ItemRegistry.Create("(O)380", amount);
                case 2:
                  return ItemRegistry.Create("(O)382", amount);
                default:
                  return ItemRegistry.Create(Game1.player.deepestMineLevel > 75 ? "(O)384" : "(O)380", amount);
              }
            default:
              switch (random.Next(5))
              {
                case 0:
                  return ItemRegistry.Create("(O)378", amount);
                case 1:
                  return ItemRegistry.Create("(O)380", amount);
                case 2:
                  return ItemRegistry.Create("(O)382", amount);
                case 3:
                  return ItemRegistry.Create("(O)384", amount);
                default:
                  return ItemRegistry.Create("(O)386", amount / 2 + 1);
              }
          }
        }
      }
    }
    catch (Exception ex)
    {
      Game1.log.Error($"Geode '{geode?.QualifiedItemId}' failed creating treasure.", ex);
    }
    return ItemRegistry.Create("(O)390");
  }

  public static Vector2 snapToInt(Vector2 v)
  {
    v.X = (float) (int) v.X;
    v.Y = (float) (int) v.Y;
    return v;
  }

  public static Vector2 GetNearbyValidPlacementPosition(
    Farmer who,
    GameLocation location,
    Item item,
    int x,
    int y)
  {
    if (!Game1.isCheckingNonMousePlacement)
      return new Vector2((float) x, (float) y);
    int num1 = 1;
    int num2 = 1;
    Point point = new Point();
    Microsoft.Xna.Framework.Rectangle rectangle = new Microsoft.Xna.Framework.Rectangle(0, 0, num1 * 64 /*0x40*/, num2 * 64 /*0x40*/);
    if (item is Furniture furniture)
    {
      num1 = furniture.getTilesWide();
      num2 = furniture.getTilesHigh();
      rectangle.Width = furniture.boundingBox.Value.Width;
      rectangle.Height = furniture.boundingBox.Value.Height;
    }
    switch (who.FacingDirection)
    {
      case 0:
        point.X = 0;
        point.Y = -1;
        y -= (num2 - 1) * 64 /*0x40*/;
        break;
      case 1:
        point.X = 1;
        point.Y = 0;
        break;
      case 2:
        point.X = 0;
        point.Y = 1;
        break;
      case 3:
        point.X = -1;
        point.Y = 0;
        x -= (num1 - 1) * 64 /*0x40*/;
        break;
    }
    int num3 = 2;
    if (item is Object object1 && object1.isPassable() && (object1.Category == -74 || object1.isSapling() || object1.Category == -19))
    {
      x = (int) who.GetToolLocation().X / 64 /*0x40*/ * 64 /*0x40*/;
      y = (int) who.GetToolLocation().Y / 64 /*0x40*/ * 64 /*0x40*/;
      point.X = who.TilePoint.X - x / 64 /*0x40*/;
      point.Y = who.TilePoint.Y - y / 64 /*0x40*/;
      int num4 = (int) Math.Sqrt(Math.Pow((double) point.X, 2.0) + Math.Pow((double) point.Y, 2.0));
      if (num4 > 0)
      {
        point.X /= num4;
        point.Y /= num4;
      }
      num3 = num4 + 1;
    }
    bool flag = item is Object object2 && object2.isPassable();
    x = x / 64 /*0x40*/ * 64 /*0x40*/;
    y = y / 64 /*0x40*/ * 64 /*0x40*/;
    Microsoft.Xna.Framework.Rectangle boundingBox = who.GetBoundingBox();
    for (int index = 0; index < num3; ++index)
    {
      int x1 = x + point.X * index * 64 /*0x40*/;
      int y1 = y + point.Y * index * 64 /*0x40*/;
      rectangle.X = x1;
      rectangle.Y = y1;
      if (!boundingBox.Intersects(rectangle) && !flag || Utility.playerCanPlaceItemHere(location, item, x1, y1, who))
        return new Vector2((float) x1, (float) y1);
    }
    return new Vector2((float) x, (float) y);
  }

  public static bool tryToPlaceItem(GameLocation location, Object item, int x, int y)
  {
    if (item == null)
      return false;
    Vector2 key = new Vector2((float) (x / 64 /*0x40*/), (float) (y / 64 /*0x40*/));
    if (Utility.playerCanPlaceItemHere(location, (Item) item, x, y, Game1.player))
    {
      if (item is Furniture)
        Game1.player.ActiveObject = (Object) null;
      if (item.placementAction(location, x, y, Game1.player))
      {
        Game1.player.reduceActiveItemByOne();
      }
      else
      {
        switch (item)
        {
          case Furniture furniture:
            Game1.player.ActiveObject = (Object) furniture;
            break;
          case Wallpaper _:
            return false;
        }
      }
      return true;
    }
    if (Utility.isPlacementForbiddenHere(location) && item != null && item.isPlaceable())
    {
      if (Game1.didPlayerJustClickAtAll(true))
        Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.13053"));
    }
    else if (item is Furniture furniture1 && Game1.didPlayerJustLeftClick(true))
    {
      switch (furniture1.GetAdditionalFurniturePlacementStatus(location, x, y, Game1.player))
      {
        case 1:
          Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Furniture.cs.12629"));
          break;
        case 2:
          Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Furniture.cs.12632"));
          break;
        case 3:
          Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Furniture.cs.12633"));
          break;
        case 4:
          Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Furniture.cs.12632"));
          break;
      }
    }
    TerrainFeature terrainFeature;
    if (item.Category == -19 && location.terrainFeatures.TryGetValue(key, out terrainFeature) && terrainFeature is HoeDirt hoeDirt)
    {
      switch (hoeDirt.CheckApplyFertilizerRules(item.QualifiedItemId))
      {
        case HoeDirtFertilizerApplyStatus.HasThisFertilizer:
          return false;
        case HoeDirtFertilizerApplyStatus.HasAnotherFertilizer:
          if (Game1.didPlayerJustClickAtAll(true))
            Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:HoeDirt.cs.13916-2"));
          return false;
        case HoeDirtFertilizerApplyStatus.CropAlreadySprouted:
          if (Game1.didPlayerJustClickAtAll(true))
            Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:HoeDirt.cs.13916"));
          return false;
      }
    }
    Utility.playerCanPlaceItemHere(location, (Item) item, x, y, Game1.player, true);
    return false;
  }

  public static bool pointInRectangles(List<Microsoft.Xna.Framework.Rectangle> rectangles, int x, int y)
  {
    foreach (Microsoft.Xna.Framework.Rectangle rectangle in rectangles)
    {
      if (rectangle.Contains(x, y))
        return true;
    }
    return false;
  }

  public static Keys mapGamePadButtonToKey(Buttons b)
  {
    switch (b)
    {
      case Buttons.DPadUp:
        return Game1.options.getFirstKeyboardKeyFromInputButtonList(Game1.options.moveUpButton);
      case Buttons.DPadDown:
        return Game1.options.getFirstKeyboardKeyFromInputButtonList(Game1.options.moveDownButton);
      case Buttons.DPadLeft:
        return Game1.options.getFirstKeyboardKeyFromInputButtonList(Game1.options.moveLeftButton);
      case Buttons.DPadRight:
        return Game1.options.getFirstKeyboardKeyFromInputButtonList(Game1.options.moveRightButton);
      case Buttons.Start:
        return Game1.options.getFirstKeyboardKeyFromInputButtonList(Game1.options.menuButton);
      case Buttons.Back:
        return Game1.options.getFirstKeyboardKeyFromInputButtonList(Game1.options.journalButton);
      case Buttons.A:
        return Game1.options.getFirstKeyboardKeyFromInputButtonList(Game1.options.actionButton);
      case Buttons.B:
        return Game1.options.getFirstKeyboardKeyFromInputButtonList(Game1.options.menuButton);
      case Buttons.X:
        return Game1.options.getFirstKeyboardKeyFromInputButtonList(Game1.options.useToolButton);
      case Buttons.Y:
        return Game1.options.getFirstKeyboardKeyFromInputButtonList(Game1.options.menuButton);
      case Buttons.LeftThumbstickLeft:
        return Game1.options.getFirstKeyboardKeyFromInputButtonList(Game1.options.moveLeftButton);
      case Buttons.LeftThumbstickUp:
        return Game1.options.getFirstKeyboardKeyFromInputButtonList(Game1.options.moveUpButton);
      case Buttons.LeftThumbstickDown:
        return Game1.options.getFirstKeyboardKeyFromInputButtonList(Game1.options.moveDownButton);
      case Buttons.LeftThumbstickRight:
        return Game1.options.getFirstKeyboardKeyFromInputButtonList(Game1.options.moveRightButton);
      default:
        return Keys.None;
    }
  }

  public static ButtonCollection getPressedButtons(GamePadState padState, GamePadState oldPadState)
  {
    return new ButtonCollection(ref padState, ref oldPadState);
  }

  public static bool thumbstickIsInDirection(int direction, GamePadState padState)
  {
    if (Game1.currentMinigame != null)
      return true;
    switch (direction)
    {
      case 0:
        GamePadThumbSticks thumbSticks1 = padState.ThumbSticks;
        double num1 = (double) Math.Abs(thumbSticks1.Left.X);
        thumbSticks1 = padState.ThumbSticks;
        double y = (double) thumbSticks1.Left.Y;
        return num1 < y;
      case 1:
        GamePadThumbSticks thumbSticks2 = padState.ThumbSticks;
        double x = (double) thumbSticks2.Left.X;
        thumbSticks2 = padState.ThumbSticks;
        double num2 = (double) Math.Abs(thumbSticks2.Left.Y);
        return x > num2;
      case 2:
        GamePadThumbSticks thumbSticks3 = padState.ThumbSticks;
        double num3 = (double) Math.Abs(thumbSticks3.Left.X);
        thumbSticks3 = padState.ThumbSticks;
        double num4 = (double) Math.Abs(thumbSticks3.Left.Y);
        return num3 < num4;
      case 3:
        GamePadThumbSticks thumbSticks4 = padState.ThumbSticks;
        double num5 = (double) Math.Abs(thumbSticks4.Left.X);
        thumbSticks4 = padState.ThumbSticks;
        double num6 = (double) Math.Abs(thumbSticks4.Left.Y);
        return num5 > num6;
      default:
        return false;
    }
  }

  public static ButtonCollection getHeldButtons(GamePadState padState)
  {
    return new ButtonCollection(ref padState);
  }

  /// <summary>return true if music becomes muted</summary>
  /// <returns></returns>
  public static bool toggleMuteMusic()
  {
    if ((double) Game1.options.musicVolumeLevel != 0.0)
    {
      Utility.disableMusic();
      return true;
    }
    Utility.enableMusic();
    return false;
  }

  public static void enableMusic()
  {
    Game1.options.musicVolumeLevel = 0.75f;
    Game1.musicCategory.SetVolume(0.75f);
    Game1.musicPlayerVolume = 0.75f;
    Game1.options.ambientVolumeLevel = 0.75f;
    Game1.ambientCategory.SetVolume(0.75f);
    Game1.ambientPlayerVolume = 0.75f;
  }

  public static void disableMusic()
  {
    Game1.options.musicVolumeLevel = 0.0f;
    Game1.musicCategory.SetVolume(0.0f);
    Game1.options.ambientVolumeLevel = 0.0f;
    Game1.ambientCategory.SetVolume(0.0f);
    Game1.ambientPlayerVolume = 0.0f;
    Game1.musicPlayerVolume = 0.0f;
  }

  public static Vector2 getVelocityTowardPlayer(Point startingPoint, float speed, Farmer f)
  {
    Microsoft.Xna.Framework.Rectangle boundingBox = f.GetBoundingBox();
    return Utility.getVelocityTowardPoint(startingPoint, new Vector2((float) boundingBox.X, (float) boundingBox.Y), speed);
  }

  /// <summary>Get a timestamp with hours and minutes from a milliseconds count, like <c>27:46</c> for 100,000,000 milliseconds.</summary>
  /// <param name="milliseconds">The number of milliseconds.</param>
  public static string getHoursMinutesStringFromMilliseconds(ulong milliseconds)
  {
    ulong num = milliseconds / 3600000UL;
    string str1 = num.ToString();
    string str2 = milliseconds % 3600000UL / 60000UL < 10UL ? "0" : "";
    num = milliseconds % 3600000UL / 60000UL;
    string str3 = num.ToString();
    return $"{str1}:{str2}{str3}";
  }

  /// <summary>Get a timestamp with minutes and seconds from a milliseconds count, like <c>1:40</c> for 100,000 milliseconds.</summary>
  /// <param name="milliseconds">The number of milliseconds.</param>
  public static string getMinutesSecondsStringFromMilliseconds(int milliseconds)
  {
    int num = milliseconds / 60000;
    string str1 = num.ToString();
    string str2 = milliseconds % 60000 / 1000 < 10 ? "0" : "";
    num = milliseconds % 60000 / 1000;
    string str3 = num.ToString();
    return $"{str1}:{str2}{str3}";
  }

  public static Vector2 getVelocityTowardPoint(
    Vector2 startingPoint,
    Vector2 endingPoint,
    float speed)
  {
    double x1 = (double) endingPoint.X - (double) startingPoint.X;
    double x2 = (double) endingPoint.Y - (double) startingPoint.Y;
    if (Math.Abs(x1) < 0.1 && Math.Abs(x2) < 0.1)
      return new Vector2(0.0f, 0.0f);
    double num1 = Math.Sqrt(Math.Pow(x1, 2.0) + Math.Pow(x2, 2.0));
    double num2 = x1 / num1;
    double num3 = x2 / num1;
    return new Vector2((float) num2 * speed, (float) num3 * speed);
  }

  public static Vector2 getVelocityTowardPoint(
    Point startingPoint,
    Vector2 endingPoint,
    float speed)
  {
    return Utility.getVelocityTowardPoint(new Vector2((float) startingPoint.X, (float) startingPoint.Y), endingPoint, speed);
  }

  public static Vector2 getRandomPositionInThisRectangle(Microsoft.Xna.Framework.Rectangle r, Random random)
  {
    return new Vector2((float) random.Next(r.X, r.X + r.Width), (float) random.Next(r.Y, r.Y + r.Height));
  }

  public static Vector2 getTopLeftPositionForCenteringOnScreen(
    xTile.Dimensions.Rectangle viewport,
    int width,
    int height,
    int xOffset = 0,
    int yOffset = 0)
  {
    return new Vector2((float) (viewport.Width / 2 - width / 2 + xOffset), (float) (viewport.Height / 2 - height / 2 + yOffset));
  }

  public static Vector2 getTopLeftPositionForCenteringOnScreen(
    int width,
    int height,
    int xOffset = 0,
    int yOffset = 0)
  {
    return Utility.getTopLeftPositionForCenteringOnScreen(Game1.uiViewport, width, height, xOffset, yOffset);
  }

  public static void recursiveFindPositionForCharacter(
    NPC c,
    GameLocation l,
    Vector2 tileLocation,
    int maxIterations)
  {
    int num = 0;
    Queue<Vector2> vector2Queue = new Queue<Vector2>();
    vector2Queue.Enqueue(tileLocation);
    List<Vector2> vector2List = new List<Vector2>();
    Microsoft.Xna.Framework.Rectangle boundingBox = c.GetBoundingBox();
    for (; num < maxIterations && vector2Queue.Count > 0; ++num)
    {
      Vector2 vector2 = vector2Queue.Dequeue();
      vector2List.Add(vector2);
      c.Position = new Vector2((float) ((double) vector2.X * 64.0 + 32.0) - (float) (boundingBox.Width / 2), vector2.Y * 64f - (float) boundingBox.Height);
      if (!l.isCollidingPosition(c.GetBoundingBox(), Game1.viewport, false, 0, false, (Character) c, true))
      {
        if (l.characters.Contains(c))
          break;
        l.characters.Add(c);
        c.currentLocation = l;
        break;
      }
      foreach (Vector2 directionsTileVector in Utility.DirectionsTileVectors)
      {
        if (!vector2List.Contains(vector2 + directionsTileVector))
          vector2Queue.Enqueue(vector2 + directionsTileVector);
      }
    }
  }

  public static Pet findPet(Guid guid)
  {
    foreach (NPC character in Game1.getFarm().characters)
    {
      if (character is Pet pet && pet.petId.Value.Equals(guid))
        return pet;
    }
    foreach (Farmer allFarmer in Game1.getAllFarmers())
    {
      foreach (NPC character in Utility.getHomeOfFarmer(allFarmer).characters)
      {
        if (character is Pet pet && pet.petId.Value.Equals(guid))
          return pet;
      }
    }
    return (Pet) null;
  }

  public static Vector2 recursiveFindOpenTileForCharacter(
    Character c,
    GameLocation l,
    Vector2 tileLocation,
    int maxIterations,
    bool allowOffMap = true)
  {
    int num = 0;
    Queue<Vector2> vector2Queue = new Queue<Vector2>();
    vector2Queue.Enqueue(tileLocation);
    List<Vector2> vector2List = new List<Vector2>();
    Vector2 position1 = c.Position;
    int width = c.GetBoundingBox().Width;
    for (; num < maxIterations && vector2Queue.Count > 0; ++num)
    {
      Vector2 position2 = vector2Queue.Dequeue();
      vector2List.Add(position2);
      c.Position = new Vector2((float) ((double) position2.X * 64.0 + 32.0) - (float) (width / 2), (float) ((double) position2.Y * 64.0 + 4.0));
      Microsoft.Xna.Framework.Rectangle boundingBox = c.GetBoundingBox();
      c.Position = position1;
      if (!l.isCollidingPosition(boundingBox, Game1.viewport, c is Farmer, 0, false, c, false, skipCollisionEffects: true) && (allowOffMap || l.isTileOnMap(position2)))
        return position2;
      foreach (Vector2 directionsTileVector in Utility.DirectionsTileVectors)
      {
        if (!vector2List.Contains(position2 + directionsTileVector) && l.isTilePlaceable(position2 + directionsTileVector) && (!(l is DecoratableLocation) || !(l as DecoratableLocation).isTileOnWall((int) ((double) directionsTileVector.X + (double) position2.X), (int) ((double) directionsTileVector.Y + (double) position2.Y))))
          vector2Queue.Enqueue(position2 + directionsTileVector);
      }
    }
    return Vector2.Zero;
  }

  public static List<Vector2> recursiveFindOpenTiles(
    GameLocation l,
    Vector2 tileLocation,
    int maxOpenTilesToFind = 24,
    int maxIterations = 50)
  {
    int num = 0;
    Queue<Vector2> vector2Queue = new Queue<Vector2>();
    vector2Queue.Enqueue(tileLocation);
    List<Vector2> vector2List = new List<Vector2>();
    List<Vector2> openTiles;
    for (openTiles = new List<Vector2>(); num < maxIterations && vector2Queue.Count > 0 && openTiles.Count < maxOpenTilesToFind; ++num)
    {
      Vector2 tile = vector2Queue.Dequeue();
      vector2List.Add(tile);
      if (l.CanItemBePlacedHere(tile))
        openTiles.Add(tile);
      foreach (Vector2 directionsTileVector in Utility.DirectionsTileVectors)
      {
        if (!vector2List.Contains(tile + directionsTileVector))
          vector2Queue.Enqueue(tile + directionsTileVector);
      }
    }
    return openTiles;
  }

  public static void spreadAnimalsAround(Building b, GameLocation environment)
  {
    try
    {
      GameLocation indoors = b.GetIndoors();
      if (indoors == null)
        return;
      Utility.spreadAnimalsAround(b, environment, (IEnumerable<FarmAnimal>) indoors.animals.Values);
    }
    catch (Exception ex)
    {
    }
  }

  public static void spreadAnimalsAround(
    Building b,
    GameLocation environment,
    IEnumerable<FarmAnimal> animalsList)
  {
    if (!b.HasIndoors())
      return;
    Queue<FarmAnimal> farmAnimalQueue = new Queue<FarmAnimal>(animalsList);
    int num = 0;
    Queue<Vector2> vector2Queue = new Queue<Vector2>();
    vector2Queue.Enqueue(new Vector2((float) (b.tileX.Value + b.animalDoor.X), (float) (b.tileY.Value + b.animalDoor.Y + 1)));
    for (; farmAnimalQueue.Count > 0 && num < 40 && vector2Queue.Count > 0; ++num)
    {
      Vector2 vector2 = vector2Queue.Dequeue();
      FarmAnimal farmAnimal1 = farmAnimalQueue.Peek();
      Microsoft.Xna.Framework.Rectangle boundingBox1 = farmAnimal1.GetBoundingBox();
      farmAnimal1.Position = new Vector2((float) ((double) vector2.X * 64.0 + 32.0) - (float) (boundingBox1.Width / 2), (float) ((double) vector2.Y * 64.0 - 32.0) - (float) (boundingBox1.Height / 2));
      if (!environment.isCollidingPosition(farmAnimal1.GetBoundingBox(), Game1.viewport, false, 0, false, (Character) farmAnimal1, true))
      {
        environment.animals.Add(farmAnimal1.myID.Value, farmAnimal1);
        farmAnimalQueue.Dequeue();
      }
      if (farmAnimalQueue.Count > 0)
      {
        FarmAnimal farmAnimal2 = farmAnimalQueue.Peek();
        Microsoft.Xna.Framework.Rectangle boundingBox2 = farmAnimal2.GetBoundingBox();
        foreach (Vector2 directionsTileVector in Utility.DirectionsTileVectors)
        {
          farmAnimal2.Position = new Vector2((float) (((double) vector2.X + (double) directionsTileVector.X) * 64.0 + 32.0) - (float) (boundingBox2.Width / 2), (float) (((double) vector2.Y + (double) directionsTileVector.Y) * 64.0 - 32.0) - (float) (boundingBox2.Height / 2));
          if (!environment.isCollidingPosition(farmAnimal2.GetBoundingBox(), Game1.viewport, false, 0, false, (Character) farmAnimal2, true))
            vector2Queue.Enqueue(vector2 + directionsTileVector);
        }
      }
    }
  }

  /// <summary>Get the tile position which contains a tile index.</summary>
  /// <param name="location">The location whose map to search.</param>
  /// <param name="tileIndex">The tile index to find.</param>
  /// <param name="layerId">The layer whose tiles to check.</param>
  /// <param name="tilesheet">The tilesheet ID containing the <paramref name="tileIndex" />, or <c>null</c> for any tilesheet. If a tile doesn't use this tilesheet, it'll be ignored.</param>
  /// <returns>Returns the first match found, or (-1, -1) if none was found.</returns>
  public static Point findTile(
    GameLocation location,
    int tileIndex,
    string layerId,
    string tilesheet = null)
  {
    Layer layer = location.map.RequireLayer(layerId);
    for (int y = 0; y < layer.LayerHeight; ++y)
    {
      for (int x = 0; x < layer.LayerWidth; ++x)
      {
        if (location.getTileIndexAt(x, y, layerId, tilesheet) == tileIndex)
          return new Point(x, y);
      }
    }
    return new Point(-1, -1);
  }

  public static bool[] horizontalOrVerticalCollisionDirections(
    Microsoft.Xna.Framework.Rectangle boundingBox,
    Character c,
    bool projectile = false)
  {
    bool[] flagArray = new bool[2];
    Microsoft.Xna.Framework.Rectangle position = new Microsoft.Xna.Framework.Rectangle(boundingBox.X, boundingBox.Y, boundingBox.Width, boundingBox.Height);
    position.Width = 1;
    position.X = boundingBox.Center.X;
    if (c != null)
    {
      if (Game1.currentLocation.isCollidingPosition(position, Game1.viewport, false, -1, projectile, c, false, projectile))
        flagArray[1] = true;
    }
    else if (Game1.currentLocation.isCollidingPosition(position, Game1.viewport, false, -1, projectile, c, false, projectile))
      flagArray[1] = true;
    position.Width = boundingBox.Width;
    position.X = boundingBox.X;
    position.Height = 1;
    position.Y = boundingBox.Center.Y;
    if (c != null)
    {
      if (Game1.currentLocation.isCollidingPosition(position, Game1.viewport, false, -1, projectile, c, false, projectile))
        flagArray[0] = true;
    }
    else if (Game1.currentLocation.isCollidingPosition(position, Game1.viewport, false, -1, projectile, c, false, projectile))
      flagArray[0] = true;
    return flagArray;
  }

  public static Color getBlendedColor(Color c1, Color c2)
  {
    return new Color(Game1.random.NextBool() ? (int) Math.Max(c1.R, c2.R) : ((int) c1.R + (int) c2.R) / 2, Game1.random.NextBool() ? (int) Math.Max(c1.G, c2.G) : ((int) c1.G + (int) c2.G) / 2, Game1.random.NextBool() ? (int) Math.Max(c1.B, c2.B) : ((int) c1.B + (int) c2.B) / 2);
  }

  public static Character checkForCharacterWithinArea(
    Type kindOfCharacter,
    Vector2 positionToAvoid,
    GameLocation location,
    Microsoft.Xna.Framework.Rectangle area)
  {
    foreach (NPC character in location.characters)
    {
      if (character.GetType().Equals(kindOfCharacter) && character.GetBoundingBox().Intersects(area) && !character.Position.Equals(positionToAvoid))
        return (Character) character;
    }
    return (Character) null;
  }

  public static int getNumberOfCharactersInRadius(GameLocation l, Point position, int tileRadius)
  {
    Microsoft.Xna.Framework.Rectangle rectangle = new Microsoft.Xna.Framework.Rectangle(position.X - tileRadius * 64 /*0x40*/, position.Y - tileRadius * 64 /*0x40*/, (tileRadius * 2 + 1) * 64 /*0x40*/, (tileRadius * 2 + 1) * 64 /*0x40*/);
    int charactersInRadius = 0;
    foreach (NPC character in l.characters)
    {
      if (rectangle.Contains(Utility.Vector2ToPoint(character.Position)))
        ++charactersInRadius;
    }
    return charactersInRadius;
  }

  public static List<Vector2> getListOfTileLocationsForBordersOfNonTileRectangle(Microsoft.Xna.Framework.Rectangle rectangle)
  {
    return new List<Vector2>()
    {
      new Vector2((float) (rectangle.Left / 64 /*0x40*/), (float) (rectangle.Top / 64 /*0x40*/)),
      new Vector2((float) (rectangle.Right / 64 /*0x40*/), (float) (rectangle.Top / 64 /*0x40*/)),
      new Vector2((float) (rectangle.Left / 64 /*0x40*/), (float) (rectangle.Bottom / 64 /*0x40*/)),
      new Vector2((float) (rectangle.Right / 64 /*0x40*/), (float) (rectangle.Bottom / 64 /*0x40*/)),
      new Vector2((float) (rectangle.Left / 64 /*0x40*/), (float) (rectangle.Center.Y / 64 /*0x40*/)),
      new Vector2((float) (rectangle.Right / 64 /*0x40*/), (float) (rectangle.Center.Y / 64 /*0x40*/)),
      new Vector2((float) (rectangle.Center.X / 64 /*0x40*/), (float) (rectangle.Bottom / 64 /*0x40*/)),
      new Vector2((float) (rectangle.Center.X / 64 /*0x40*/), (float) (rectangle.Top / 64 /*0x40*/)),
      new Vector2((float) (rectangle.Center.X / 64 /*0x40*/), (float) (rectangle.Center.Y / 64 /*0x40*/))
    };
  }

  public static void makeTemporarySpriteJuicier(
    TemporaryAnimatedSprite t,
    GameLocation l,
    int numAddOns = 4,
    int xRange = 64 /*0x40*/,
    int yRange = 64 /*0x40*/)
  {
    t.position.Y -= 8f;
    l.temporarySprites.Add(t);
    for (int index = 0; index < numAddOns; ++index)
    {
      TemporaryAnimatedSprite clone = t.getClone();
      clone.delayBeforeAnimationStart = index * 100;
      clone.position += new Vector2((float) Game1.random.Next(-xRange / 2, xRange / 2 + 1), (float) Game1.random.Next(-yRange / 2, yRange / 2 + 1));
      clone.layerDepth += 1E-06f;
      l.temporarySprites.Add(clone);
    }
  }

  public static void recursiveObjectPlacement(
    Object o,
    int tileX,
    int tileY,
    double growthRate,
    double decay,
    GameLocation location,
    string terrainToExclude = "",
    int objectIndexAddRange = 0,
    double failChance = 0.0,
    int objectIndeAddRangeMultiplier = 1,
    List<string> itemIDVariations = null)
  {
    if (o == null)
      return;
    int result;
    if (!int.TryParse(o.ItemId, out result))
      result = -1;
    if (!location.isTileLocationOpen(new Location(tileX, tileY)) || location.IsTileOccupiedBy(new Vector2((float) tileX, (float) tileY)) || !location.hasTileAt(tileX, tileY, "Back") || !terrainToExclude.Equals("") && (location.doesTileHaveProperty(tileX, tileY, "Type", "Back") == null || location.doesTileHaveProperty(tileX, tileY, "Type", "Back").Equals(terrainToExclude)))
      return;
    Vector2 key1 = new Vector2((float) tileX, (float) tileY);
    if (!Game1.random.NextBool(failChance * 2.0))
    {
      string itemId = o.ItemId;
      if (result >= 0)
        itemId = (result + Game1.random.Next(objectIndexAddRange + 1) * objectIndeAddRangeMultiplier).ToString();
      if (o is ColoredObject coloredObject1)
      {
        OverlaidDictionary objects = location.objects;
        Vector2 key2 = key1;
        ColoredObject coloredObject = new ColoredObject(itemId, 1, coloredObject1.color.Value);
        coloredObject.Fragility = o.fragility.Value;
        coloredObject.MinutesUntilReady = o.MinutesUntilReady;
        coloredObject.Name = o.name;
        coloredObject.CanBeSetDown = o.CanBeSetDown;
        coloredObject.CanBeGrabbed = o.CanBeGrabbed;
        coloredObject.IsSpawnedObject = o.IsSpawnedObject;
        coloredObject.TileLocation = key1;
        coloredObject.ColorSameIndexAsParentSheetIndex = coloredObject1.ColorSameIndexAsParentSheetIndex;
        objects.Add(key2, (Object) coloredObject);
      }
      else
        location.objects.Add(key1, new Object(itemId, 1)
        {
          Fragility = o.fragility.Value,
          MinutesUntilReady = o.MinutesUntilReady,
          CanBeSetDown = o.canBeSetDown.Value,
          CanBeGrabbed = o.canBeGrabbed.Value,
          IsSpawnedObject = o.isSpawnedObject.Value
        });
    }
    growthRate -= decay;
    if (Game1.random.NextDouble() < growthRate)
      Utility.recursiveObjectPlacement(o, tileX + 1, tileY, growthRate, decay, location, terrainToExclude, objectIndexAddRange, failChance, objectIndeAddRangeMultiplier, itemIDVariations);
    if (Game1.random.NextDouble() < growthRate)
      Utility.recursiveObjectPlacement(o, tileX - 1, tileY, growthRate, decay, location, terrainToExclude, objectIndexAddRange, failChance, objectIndeAddRangeMultiplier, itemIDVariations);
    if (Game1.random.NextDouble() < growthRate)
      Utility.recursiveObjectPlacement(o, tileX, tileY + 1, growthRate, decay, location, terrainToExclude, objectIndexAddRange, failChance, objectIndeAddRangeMultiplier, itemIDVariations);
    if (Game1.random.NextDouble() >= growthRate)
      return;
    Utility.recursiveObjectPlacement(o, tileX, tileY - 1, growthRate, decay, location, terrainToExclude, objectIndexAddRange, failChance, objectIndeAddRangeMultiplier, itemIDVariations);
  }

  public static void recursiveFarmGrassPlacement(
    int tileX,
    int tileY,
    double growthRate,
    double decay,
    GameLocation farm)
  {
    if (!farm.isTileLocationOpen(new Location(tileX, tileY)) || farm.IsTileOccupiedBy(new Vector2((float) tileX, (float) tileY)) || farm.doesTileHaveProperty(tileX, tileY, "Diggable", "Back") == null)
      return;
    Vector2 key = new Vector2((float) tileX, (float) tileY);
    if (Game1.random.NextDouble() < 0.05)
      farm.objects.Add(new Vector2((float) tileX, (float) tileY), ItemRegistry.Create<Object>(Game1.random.Choose<string>("(O)674", "(O)675")));
    else
      farm.terrainFeatures.Add(key, (TerrainFeature) new Grass(1, 4 - (int) ((1.0 - growthRate) * 4.0)));
    growthRate -= decay;
    if (Game1.random.NextDouble() < growthRate)
      Utility.recursiveFarmGrassPlacement(tileX + 1, tileY, growthRate, decay, farm);
    if (Game1.random.NextDouble() < growthRate)
      Utility.recursiveFarmGrassPlacement(tileX - 1, tileY, growthRate, decay, farm);
    if (Game1.random.NextDouble() < growthRate)
      Utility.recursiveFarmGrassPlacement(tileX, tileY + 1, growthRate, decay, farm);
    if (Game1.random.NextDouble() >= growthRate)
      return;
    Utility.recursiveFarmGrassPlacement(tileX, tileY - 1, growthRate, decay, farm);
  }

  public static void recursiveTreePlacement(
    int tileX,
    int tileY,
    double growthRate,
    int growthStage,
    double skipChance,
    GameLocation l,
    Microsoft.Xna.Framework.Rectangle clearPatch,
    bool sparse)
  {
    if (clearPatch.Contains(tileX, tileY))
      return;
    Vector2 vector2 = new Vector2((float) tileX, (float) tileY);
    if (l.doesTileHaveProperty((int) vector2.X, (int) vector2.Y, "Diggable", "Back") == null || l.IsNoSpawnTile(vector2) || !l.isTileLocationOpen(new Location((int) vector2.X, (int) vector2.Y)) || l.IsTileOccupiedBy(vector2) || sparse && (l.IsTileOccupiedBy(new Vector2((float) tileX, (float) (tileY - 1))) || l.IsTileOccupiedBy(new Vector2((float) tileX, (float) (tileY + 1))) || l.IsTileOccupiedBy(new Vector2((float) (tileX + 1), (float) tileY)) || l.IsTileOccupiedBy(new Vector2((float) (tileX - 1), (float) tileY)) || l.IsTileOccupiedBy(new Vector2((float) (tileX + 1), (float) (tileY + 1)))))
      return;
    if (!Game1.random.NextBool(skipChance))
    {
      if (sparse && (double) vector2.X < 70.0 && ((double) vector2.X < 48.0 || (double) vector2.Y > 26.0) && Game1.random.NextDouble() < 0.07)
        (l as Farm).resourceClumps.Add(new ResourceClump(Game1.random.Choose<int>(672, 600, 602), 2, 2, vector2));
      else
        l.terrainFeatures.Add(vector2, (TerrainFeature) new Tree(Game1.random.Next(1, 4).ToString(), growthStage < 5 ? Game1.random.Next(5) : 5));
      growthRate -= 0.05;
    }
    if (Game1.random.NextDouble() < growthRate)
      Utility.recursiveTreePlacement(tileX + Game1.random.Next(1, 3), tileY, growthRate, growthStage, skipChance, l, clearPatch, sparse);
    if (Game1.random.NextDouble() < growthRate)
      Utility.recursiveTreePlacement(tileX - Game1.random.Next(1, 3), tileY, growthRate, growthStage, skipChance, l, clearPatch, sparse);
    if (Game1.random.NextDouble() < growthRate)
      Utility.recursiveTreePlacement(tileX, tileY + Game1.random.Next(1, 3), growthRate, growthStage, skipChance, l, clearPatch, sparse);
    if (Game1.random.NextDouble() >= growthRate)
      return;
    Utility.recursiveTreePlacement(tileX, tileY - Game1.random.Next(1, 3), growthRate, growthStage, skipChance, l, clearPatch, sparse);
  }

  public static void recursiveRemoveTerrainFeatures(
    int tileX,
    int tileY,
    double growthRate,
    double decay,
    GameLocation l)
  {
    Vector2 key = new Vector2((float) tileX, (float) tileY);
    l.terrainFeatures.Remove(key);
    growthRate -= decay;
    if (Game1.random.NextDouble() < growthRate)
      Utility.recursiveRemoveTerrainFeatures(tileX + 1, tileY, growthRate, decay, l);
    if (Game1.random.NextDouble() < growthRate)
      Utility.recursiveRemoveTerrainFeatures(tileX - 1, tileY, growthRate, decay, l);
    if (Game1.random.NextDouble() < growthRate)
      Utility.recursiveRemoveTerrainFeatures(tileX, tileY + 1, growthRate, decay, l);
    if (Game1.random.NextDouble() >= growthRate)
      return;
    Utility.recursiveRemoveTerrainFeatures(tileX, tileY - 1, growthRate, decay, l);
  }

  public static IEnumerator<int> generateNewFarm(bool skipFarmGeneration)
  {
    return Utility.generateNewFarm(skipFarmGeneration, true);
  }

  public static IEnumerator<int> generateNewFarm(bool skipFarmGeneration, bool loadForNewGame)
  {
    Game1.fadeToBlack = false;
    Game1.fadeToBlackAlpha = 1f;
    Game1.debrisWeather.Clear();
    Game1.viewport.X = -9999;
    Game1.changeMusicTrack("none");
    if (loadForNewGame)
      Game1.game1.loadForNewGame();
    Game1.currentLocation = Game1.RequireLocation("Farmhouse");
    Game1.currentLocation.currentEvent = new Event("none/-600 -600/farmer 4 8 2/warp farmer 4 8/end beginGame");
    Game1.gameMode = (byte) 2;
    yield return 100;
  }

  /// <summary>Get the pixel distance between a position in the world and the player's screen viewport, where 0 is within the viewport.</summary>
  /// <param name="pixelPosition">The pixel position.</param>
  public static float distanceFromScreen(Vector2 pixelPosition)
  {
    float num1 = pixelPosition.X - (float) Game1.viewport.X;
    float num2 = pixelPosition.Y - (float) Game1.viewport.Y;
    double x1 = (double) MathHelper.Clamp(num1, 0.0f, (float) (Game1.viewport.Width - 1));
    float num3 = MathHelper.Clamp(num2, 0.0f, (float) (Game1.viewport.Height - 1));
    double x2 = (double) num1;
    double y1 = (double) num3;
    double y2 = (double) num2;
    return Utility.distance((float) x1, (float) x2, (float) y1, (float) y2);
  }

  /// <summary>Get whether a pixel position is within the current player's screen viewport.</summary>
  /// <param name="positionNonTile">The pixel position.</param>
  /// <param name="acceptableDistanceFromScreen">The maximum pixel distance outside the screen viewport to allow.</param>
  public static bool isOnScreen(Vector2 positionNonTile, int acceptableDistanceFromScreen)
  {
    positionNonTile.X -= (float) Game1.viewport.X;
    positionNonTile.Y -= (float) Game1.viewport.Y;
    return (double) positionNonTile.X > (double) -acceptableDistanceFromScreen && (double) positionNonTile.X < (double) (Game1.viewport.Width + acceptableDistanceFromScreen) && (double) positionNonTile.Y > (double) -acceptableDistanceFromScreen && (double) positionNonTile.Y < (double) (Game1.viewport.Height + acceptableDistanceFromScreen);
  }

  /// <summary>Get whether a tile position is within the current player's screen viewport.</summary>
  /// <param name="positionTile">The tile position.</param>
  /// <param name="acceptableDistanceFromScreenNonTile">The maximum tile distance outside the screen viewport to allow.</param>
  /// <param name="location">The location whose position to check.</param>
  public static bool isOnScreen(
    Point positionTile,
    int acceptableDistanceFromScreenNonTile,
    GameLocation location = null)
  {
    return (location == null || location.Equals(Game1.currentLocation)) && positionTile.X * 64 /*0x40*/ > Game1.viewport.X - acceptableDistanceFromScreenNonTile && positionTile.X * 64 /*0x40*/ < Game1.viewport.X + Game1.viewport.Width + acceptableDistanceFromScreenNonTile && positionTile.Y * 64 /*0x40*/ > Game1.viewport.Y - acceptableDistanceFromScreenNonTile && positionTile.Y * 64 /*0x40*/ < Game1.viewport.Y + Game1.viewport.Height + acceptableDistanceFromScreenNonTile;
  }

  public static void clearObjectsInArea(Microsoft.Xna.Framework.Rectangle r, GameLocation l)
  {
    for (int left = r.Left; left < r.Right; left += 64 /*0x40*/)
    {
      for (int top = r.Top; top < r.Bottom; top += 64 /*0x40*/)
        l.removeEverythingFromThisTile(left / 64 /*0x40*/, top / 64 /*0x40*/);
    }
  }

  public static void trashItem(Item item)
  {
    if (item is Object && Game1.player.specialItems.Contains(item.ItemId))
      Game1.player.specialItems.Remove(item.ItemId);
    if (Utility.getTrashReclamationPrice(item, Game1.player) > 0)
      Game1.player.Money += Utility.getTrashReclamationPrice(item, Game1.player);
    Game1.playSound("trashcan");
  }

  public static FarmAnimal GetBestHarvestableFarmAnimal(
    IEnumerable<FarmAnimal> animals,
    Tool tool,
    Microsoft.Xna.Framework.Rectangle toolRect)
  {
    FarmAnimal harvestableFarmAnimal = (FarmAnimal) null;
    foreach (FarmAnimal animal in animals)
    {
      if (animal.GetHarvestBoundingBox().Intersects(toolRect))
      {
        if (animal.CanGetProduceWithTool(tool) && animal.currentProduce.Value != null && animal.isAdult())
          return animal;
        harvestableFarmAnimal = animal;
      }
    }
    return harvestableFarmAnimal;
  }

  public static long RandomLong(Random r = null)
  {
    if (r == null)
      r = Game1.random;
    byte[] buffer = new byte[8];
    r.NextBytes(buffer);
    return BitConverter.ToInt64(buffer, 0);
  }

  public static ulong NewUniqueIdForThisGame()
  {
    return (ulong) (long) (DateTime.UtcNow - new DateTime(2012, 6, 22)).TotalSeconds;
  }

  /// <summary>Apply platform-specific bad word filtering to a given text.</summary>
  /// <param name="words">The text to filter.</param>
  /// <returns>Returns the text with any bad words removed or censored.</returns>
  public static string FilterDirtyWords(string words) => Program.sdk.FilterDirtyWords(words);

  /// <summary>Apply platform-specific bad word filtering to a given text, but only if the current platform requires strict filtering of all text inputs (including text only shown locally).</summary>
  /// <param name="words">The text to filter.</param>
  /// <returns>Returns the text with any bad words removed or censored if applicable.</returns>
  public static string FilterDirtyWordsIfStrictPlatform(string words) => words;

  /// <summary>
  /// This is used to filter out special characters from user entered
  /// names to avoid crashes and other bugs in Dialogue.cs parsing.
  /// 
  /// The characters are replaced with spaces.
  /// </summary>
  public static string FilterUserName(string name) => name;

  public static bool IsHorizontalDirection(int direction) => direction == 3 || direction == 1;

  public static bool IsVerticalDirection(int direction) => direction == 0 || direction == 2;

  public static Microsoft.Xna.Framework.Rectangle ExpandRectangle(
    Microsoft.Xna.Framework.Rectangle rect,
    int facingDirection,
    int pixels)
  {
    switch (facingDirection)
    {
      case 0:
        rect.Height += pixels;
        rect.Y -= pixels;
        break;
      case 1:
        rect.Width += pixels;
        break;
      case 2:
        rect.Height += pixels;
        break;
      case 3:
        rect.Width += pixels;
        rect.X -= pixels;
        break;
    }
    return rect;
  }

  public static int GetOppositeFacingDirection(int facingDirection)
  {
    switch (facingDirection)
    {
      case 0:
        return 2;
      case 1:
        return 3;
      case 2:
        return 0;
      case 3:
        return 1;
      default:
        return 0;
    }
  }

  /// <summary>Convert an RGB value into an HLS value.</summary>
  /// <param name="r">The RGB red channel value.</param>
  /// <param name="g">The RGB green channel value.</param>
  /// <param name="b">The RGB blue channel value.</param>
  /// <param name="h">The equivalent hue value. This is a value between 0 and 360, indicating the angle on the HSL color wheel.</param>
  /// <param name="s">The equivalent saturation value, which indicates the amount of color added. This is a value between 0 (pure gray) and 1 (pure color).</param>
  /// <param name="l">The equivalent lightness value, which indicates how much light is in the color. This is a value between 0 (black) and 1 (white).</param>
  /// <remarks>Adapted from <a href="http://csharphelper.com/howtos/howto_rgb_to_hls.html">code by Rod Stephens</a>.</remarks>
  public static void RGBtoHSL(int r, int g, int b, out double h, out double s, out double l)
  {
    double num1 = (double) r / (double) byte.MaxValue;
    double num2 = (double) g / (double) byte.MaxValue;
    double num3 = (double) b / (double) byte.MaxValue;
    double num4 = num1;
    if (num4 < num2)
      num4 = num2;
    if (num4 < num3)
      num4 = num3;
    double num5 = num1;
    if (num5 > num2)
      num5 = num2;
    if (num5 > num3)
      num5 = num3;
    double num6 = num4 - num5;
    l = (num4 + num5) / 2.0;
    if (Math.Abs(num6) < 1E-05)
    {
      s = 0.0;
      h = 0.0;
    }
    else
    {
      s = l > 0.5 ? num6 / (2.0 - num4 - num5) : num6 / (num4 + num5);
      double num7 = (num4 - num1) / num6;
      double num8 = (num4 - num2) / num6;
      double num9 = (num4 - num3) / num6;
      h = num1 != num4 ? (num2 != num4 ? 4.0 + num8 - num7 : 2.0 + num7 - num9) : num9 - num8;
      h *= 60.0;
      if (h >= 0.0)
        return;
      h += 360.0;
    }
  }

  /// <summary>Convert an HSL value into an RGB value.</summary>
  /// <param name="h">The hue value.</param>
  /// <param name="s">The saturation value.</param>
  /// <param name="l">The lightness value.</param>
  /// <param name="r">The equivalent RGB red channel value.</param>
  /// <param name="g">The equivalent RGB green channel value.</param>
  /// <param name="b">The equivalent RGB blue channel value.</param>
  /// <remarks>Adapted from <a href="http://csharphelper.com/howtos/howto_rgb_to_hls.html">code by Rod Stephens</a>.</remarks>
  public static void HSLtoRGB(double h, double s, double l, out int r, out int g, out int b)
  {
    double q2 = l > 0.5 ? l + s - l * s : l * (1.0 + s);
    double q1 = 2.0 * l - q2;
    double num1;
    double num2;
    double num3;
    if (s == 0.0)
    {
      num1 = l;
      num2 = l;
      num3 = l;
    }
    else
    {
      num1 = Utility.QQHtoRGB(q1, q2, h + 120.0);
      num2 = Utility.QQHtoRGB(q1, q2, h);
      num3 = Utility.QQHtoRGB(q1, q2, h - 120.0);
    }
    r = (int) (num1 * (double) byte.MaxValue);
    g = (int) (num2 * (double) byte.MaxValue);
    b = (int) (num3 * (double) byte.MaxValue);
  }

  private static double QQHtoRGB(double q1, double q2, double hue)
  {
    if (hue > 360.0)
      hue -= 360.0;
    else if (hue < 0.0)
      hue += 360.0;
    if (hue < 60.0)
      return q1 + (q2 - q1) * hue / 60.0;
    if (hue < 180.0)
      return q2;
    return hue < 240.0 ? q1 + (q2 - q1) * (240.0 - hue) / 60.0 : q1;
  }

  public static float ModifyCoordinateFromUIScale(float coordinate)
  {
    return coordinate * Game1.options.uiScale / Game1.options.zoomLevel;
  }

  public static Vector2 ModifyCoordinatesFromUIScale(Vector2 coordinates)
  {
    return coordinates * Game1.options.uiScale / Game1.options.zoomLevel;
  }

  public static float ModifyCoordinateForUIScale(float coordinate)
  {
    return coordinate / Game1.options.uiScale * Game1.options.zoomLevel;
  }

  public static Vector2 ModifyCoordinatesForUIScale(Vector2 coordinates)
  {
    return coordinates / Game1.options.uiScale * Game1.options.zoomLevel;
  }

  public static bool ShouldIgnoreValueChangeCallback()
  {
    return Game1.gameMode != (byte) 3 || Game1.client != null && !Game1.client.readyToPlay || Game1.client != null && Game1.locationRequest != null;
  }

  /// <summary>Constrain an index to a range by wrapping out-of-bounds values to the other side (e.g. last index + 1 is the first index).</summary>
  /// <param name="index">The index to constrain.</param>
  /// <param name="count">The number of values in the range.</param>
  public static int WrapIndex(int index, int count) => (index + count) % count;

  /// <summary>Indicates the reasons a horse can't be summoned by a player.</summary>
  [Flags]
  public enum HorseWarpRestrictions
  {
    /// <summary>No reasons apply.</summary>
    None = 0,
    /// <summary>The player doesn't own a horse.</summary>
    NoOwnedHorse = 1,
    /// <summary>The player is indoors (horses can't be summoned to an indoors location).</summary>
    Indoors = 2,
    /// <summary>There's no room near the player to place the horse.</summary>
    NoRoom = 4,
    /// <summary>The player's horse is currently in use by another player.</summary>
    InUse = 8,
  }
}
