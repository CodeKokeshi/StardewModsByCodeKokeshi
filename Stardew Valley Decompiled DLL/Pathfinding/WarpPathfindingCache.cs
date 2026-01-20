// Decompiled with JetBrains decompiler
// Type: StardewValley.Pathfinding.WarpPathfindingCache
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Netcode;
using StardewValley.Locations;
using System.Collections.Generic;

#nullable disable
namespace StardewValley.Pathfinding;

/// <summary>Handles pathfinding between locations.</summary>
public static class WarpPathfindingCache
{
  /// <summary>Every possible path through location names that NPCs can take while pathfinding, indexed by the start location.</summary>
  /// <remarks>For example, <c>"BusStop": [ "BusStop", "Town", "Mountain" ]</c> means that an NPC in the bus stop can warp to town and then to the mountain.</remarks>
  private static readonly Dictionary<string, List<LocationWarpRoute>> Routes = new Dictionary<string, List<LocationWarpRoute>>();
  /// <summary>The location names which NPCs aren't allowed to warp through.</summary>
  /// <remarks>The farmhand cellars are added automatically.</remarks>
  public static readonly HashSet<string> IgnoreLocationNames = new HashSet<string>()
  {
    "Backwoods",
    "Cellar",
    "Farm"
  };
  /// <summary>A map of warp targets to the actual location name NPCs should warp to.</summary>
  public static readonly Dictionary<string, string> OverrideTargetNames = new Dictionary<string, string>()
  {
    ["BoatTunnel"] = "IslandSouth"
  };
  /// <summary>The locations which can only be accessed by NPCs of one gender.</summary>
  public static readonly Dictionary<string, Gender> GenderRestrictions = new Dictionary<string, Gender>()
  {
    ["BathHouse_MensLocker"] = Gender.Male,
    ["BathHouse_WomensLocker"] = Gender.Female
  };

  /// <summary>Cache the possible pathfinding routes between game locations.</summary>
  public static void PopulateCache()
  {
    for (int index = 1; index <= Game1.netWorldState.Value.HighestPlayerLimit; ++index)
      WarpPathfindingCache.IgnoreLocationNames.Add("Cellar" + index.ToString());
    WarpPathfindingCache.Routes.Clear();
    foreach (GameLocation location in (IEnumerable<GameLocation>) Game1.locations)
    {
      if (!WarpPathfindingCache.IgnoreLocationNames.Contains(location.NameOrUniqueName))
        WarpPathfindingCache.ExploreWarpPoints(location, new List<string>(), new Gender?());
    }
  }

  /// <summary>Get a valid pathfinding route between a start and destination location.</summary>
  /// <param name="startingLocation">The name of the location the NPC is starting from.</param>
  /// <param name="endingLocation">The name of the destination location.</param>
  /// <param name="gender">The NPC's gender, used to choose gender-specific routes like the pool locker rooms.</param>
  /// <returns>If a valid route was found, returns a list of location names to transit through including the start and destination locations. For example, <c>[ "BusStop", "Town", "Mountain" ]</c> means that an NPC in the bus stop can warp to town and then to the mountain. If no valid route was found, returns null.</returns>
  public static string[] GetLocationRoute(
    string startingLocation,
    string endingLocation,
    Gender gender)
  {
    List<LocationWarpRoute> locationWarpRouteList;
    if (WarpPathfindingCache.Routes.TryGetValue(startingLocation, out locationWarpRouteList))
    {
      foreach (LocationWarpRoute locationWarpRoute in locationWarpRouteList)
      {
        if (locationWarpRoute.LocationNames[locationWarpRoute.LocationNames.Length - 1] == endingLocation)
        {
          if (locationWarpRoute.OnlyGender.HasValue)
          {
            Gender? onlyGender = locationWarpRoute.OnlyGender;
            Gender gender1 = gender;
            if (!(onlyGender.GetValueOrDefault() == gender1 & onlyGender.HasValue) && gender != Gender.Undefined)
              continue;
          }
          return locationWarpRoute.LocationNames;
        }
      }
    }
    return (string[]) null;
  }

  /// <summary>Recursively populate the cache based on every location reachable through warps starting from this location.</summary>
  /// <param name="location">The location to start from.</param>
  /// <param name="route">The location names explored up to this point for the current route, excluding the <paramref name="location" />.</param>
  /// <param name="genderRestriction">The gender restriction for the route up to this point, if any. For example, a route which passes through the men's locker room is restricted to male NPCs.</param>
  private static void ExploreWarpPoints(
    GameLocation location,
    List<string> route,
    Gender? genderRestriction)
  {
    string key = location?.name.Value;
    if (key == null || location.ShouldExcludeFromNpcPathfinding() || route.Contains(key))
      return;
    Gender gender;
    if (WarpPathfindingCache.GenderRestrictions.TryGetValue(key, out gender))
    {
      if (genderRestriction.HasValue && genderRestriction.Value != gender)
        return;
      genderRestriction = new Gender?(gender);
    }
    route.Add(key);
    if (route.Count > 1)
      WarpPathfindingCache.AddRoute(route, genderRestriction);
    bool flag1 = location.warps.Count > 0;
    bool flag2 = location.doors.Length > 0;
    if (flag1 | flag2)
    {
      HashSet<string> seenTargets = new HashSet<string>()
      {
        key
      };
      if (route.Count > 1)
        seenTargets.Add(route[route.Count - 2]);
      if (flag1)
      {
        foreach (Warp warp in (NetList<Warp, NetRef<Warp>>) location.warps)
          WarpPathfindingCache.ExploreWarpPoints(warp.TargetName, route, genderRestriction, seenTargets);
      }
      if (flag2)
      {
        foreach (string locationName in location.doors.Values)
          WarpPathfindingCache.ExploreWarpPoints(locationName, route, genderRestriction, seenTargets);
      }
    }
    if (route.Count <= 0)
      return;
    route.RemoveAt(route.Count - 1);
  }

  /// <summary>Recursively populate the cache based on every location reachable through warps starting from this location.</summary>
  /// <param name="locationName">The location name to start from.</param>
  /// <param name="route">The location names explored up to this point for the current route, excluding the <paramref name="locationName" />.</param>
  /// <param name="genderRestriction">The gender restriction for the route up to this point, if any. For example, a route which passes through the men's locker room is restricted to male NPCs.</param>
  /// <param name="seenTargets">The warp target names which have already been explored from this location.</param>
  /// <returns>Returns whether any routes were added.</returns>
  private static void ExploreWarpPoints(
    string locationName,
    List<string> route,
    Gender? genderRestriction,
    HashSet<string> seenTargets)
  {
    string str;
    if (WarpPathfindingCache.OverrideTargetNames.TryGetValue(locationName, out str))
      locationName = str;
    if (!seenTargets.Add(locationName) || WarpPathfindingCache.IgnoreLocationNames.Contains(locationName) || MineShaft.IsGeneratedLevel(locationName) || VolcanoDungeon.IsGeneratedLevel(locationName))
      return;
    WarpPathfindingCache.ExploreWarpPoints(Game1.getLocationFromName(locationName), route, genderRestriction);
  }

  /// <summary>Add a route to the <see cref="F:StardewValley.Pathfinding.WarpPathfindingCache.Routes" /> cache.</summary>
  /// <param name="route">The location names in the route.</param>
  /// <param name="onlyGender">If set, this route can only be used by NPCs of the given gender.</param>
  private static void AddRoute(List<string> route, Gender? onlyGender)
  {
    List<LocationWarpRoute> locationWarpRouteList;
    if (!WarpPathfindingCache.Routes.TryGetValue(route[0], out locationWarpRouteList))
      WarpPathfindingCache.Routes[route[0]] = locationWarpRouteList = new List<LocationWarpRoute>();
    locationWarpRouteList.Add(new LocationWarpRoute(route.ToArray(), onlyGender));
  }
}
