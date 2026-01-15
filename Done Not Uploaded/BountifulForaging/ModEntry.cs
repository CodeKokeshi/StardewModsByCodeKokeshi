using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Extensions;
using StardewValley.GameData.Locations;
using StardewValley.Internal;

namespace BountifulForaging;

public class ModEntry : Mod
{
    public override void Entry(IModHelper helper)
    {
        var harmony = new Harmony(this.ModManifest.UniqueID);
        harmony.PatchAll();
        
        // Also run foraging spawn after day starts to ensure all locations are filled
        helper.Events.GameLoop.DayStarted += OnDayStarted;
    }

    private void OnDayStarted(object? sender, DayStartedEventArgs e)
    {
        // Force spawn forage in all outdoor locations at maximum capacity
        foreach (GameLocation location in Game1.locations)
        {
            ForceMaxForageSpawn(location);
            
            // Also handle buildings within locations (like farm buildings)
            foreach (var building in location.buildings)
            {
                if (building.indoors.Value != null)
                {
                    ForceMaxForageSpawn(building.indoors.Value);
                }
            }
        }
    }

    /// <summary>
    /// Forces a location to spawn forage at maximum capacity
    /// </summary>
    private static void ForceMaxForageSpawn(GameLocation location)
    {
        if (location?.map == null)
            return;
            
        // Only process outdoor locations or those with ForceSpawnForageables property
        if (!location.IsOutdoors && !location.map.Properties.ContainsKey("ForceSpawnForageables"))
            return;
            
        var data = location.GetData();
        if (data == null || data.MaxSpawnedForageAtOnce <= 0)
            return;

        // Clear existing spawned forage objects to make room for new ones
        var keysToRemove = new List<Vector2>();
        foreach (var kvp in location.objects.Pairs)
        {
            if (kvp.Value.IsSpawnedObject && kvp.Value.isForage())
            {
                keysToRemove.Add(kvp.Key);
            }
        }
        
        foreach (var key in keysToRemove)
        {
            location.objects.Remove(key);
        }
        
        // Reset the counter
        location.numberOfSpawnedObjectsOnMap = 0;
        
        // Force spawn maximum forage by calling spawnObjects multiple times
        // Each call spawns between MinDailyForageSpawn and MaxDailyForageSpawn
        // We call it enough times to reach MaxSpawnedForageAtOnce
        int maxAttempts = 20; // Prevent infinite loop
        int attempts = 0;
        
        while (location.numberOfSpawnedObjectsOnMap < data.MaxSpawnedForageAtOnce && attempts < maxAttempts)
        {
            int beforeCount = location.numberOfSpawnedObjectsOnMap;
            SpawnForageMaximized(location, data);
            attempts++;
            
            // If no new objects were spawned, there might not be valid spawn tiles
            if (location.numberOfSpawnedObjectsOnMap == beforeCount)
                break;
        }
    }

    /// <summary>
    /// Custom forage spawning that ignores chance checks and spawns at max capacity
    /// </summary>
    private static void SpawnForageMaximized(GameLocation location, LocationData data)
    {
        if (data == null || location.numberOfSpawnedObjectsOnMap >= data.MaxSpawnedForageAtOnce)
            return;
            
        Random r = Utility.CreateDaySaveRandom(Game1.uniqueIDForThisGame, Game1.stats.DaysPlayed, location.numberOfSpawnedObjectsOnMap);
        Season season = location.GetSeason();
        
        // Get all possible forage for this location and season
        List<SpawnForageData> possibleForage = new List<SpawnForageData>();
        
        var defaultData = GameLocation.GetData("Default");
        if (defaultData?.Forage != null)
        {
            foreach (SpawnForageData spawn in defaultData.Forage)
            {
                if ((spawn.Condition == null || GameStateQuery.CheckConditions(spawn.Condition, location, null, null, null, r)) 
                    && (!spawn.Season.HasValue || spawn.Season == season))
                {
                    possibleForage.Add(spawn);
                }
            }
        }
        
        if (data.Forage != null)
        {
            foreach (SpawnForageData spawn in data.Forage)
            {
                if ((spawn.Condition == null || GameStateQuery.CheckConditions(spawn.Condition, location, null, null, null, r)) 
                    && (!spawn.Season.HasValue || spawn.Season == season))
                {
                    possibleForage.Add(spawn);
                }
            }
        }
        
        if (!possibleForage.Any())
            return;
            
        // Spawn as many as possible up to max
        int numberToSpawn = data.MaxSpawnedForageAtOnce - location.numberOfSpawnedObjectsOnMap;
        ItemQueryContext itemQueryContext = new ItemQueryContext(location, null, r, "location '" + location.NameOrUniqueName + "' > forage");
        
        for (int i = 0; i < numberToSpawn; i++)
        {
            bool spawned = false;
            
            // More attempts per object to ensure we find a valid spot
            for (int attempt = 0; attempt < 50; attempt++)
            {
                int xCoord = r.Next(location.map.DisplayWidth / 64);
                int yCoord = r.Next(location.map.DisplayHeight / 64);
                Vector2 tileLocation = new Vector2(xCoord, yCoord);
                
                // Check if tile is valid for spawning (simplified checks for more spawning)
                if (location.objects.ContainsKey(tileLocation))
                    continue;
                    
                if (location.IsNoSpawnTile(tileLocation))
                    continue;
                    
                // Check if spawnable tile (but also allow diggable tiles as fallback)
                bool isSpawnable = location.doesTileHaveProperty(xCoord, yCoord, "Spawnable", "Back") != null;
                bool isDiggable = location.doesTileHaveProperty(xCoord, yCoord, "Diggable", "Back") != null;
                
                if (!isSpawnable && !isDiggable)
                    continue;
                    
                if (location.doesEitherTileOrTileIndexPropertyEqual(xCoord, yCoord, "Spawnable", "Back", "F"))
                    continue;
                    
                if (!location.CanItemBePlacedHere(tileLocation))
                    continue;
                    
                // Skip front layer checks for more spawn opportunities
                if (location.isBehindBush(tileLocation))
                    continue;
                
                // Choose a random forage from possibilities (skip chance check for guaranteed spawn)
                SpawnForageData forage = r.ChooseFrom(possibleForage);
                
                Item? forageItem = ItemQueryResolver.TryResolveRandomItem(forage, itemQueryContext, avoidRepeat: false, null, null, null, delegate(string query, string error)
                {
                    // Silent error handling
                });
                
                if (forageItem == null)
                    continue;
                    
                if (forageItem is not StardewValley.Object forageObj)
                    continue;
                    
                forageObj.IsSpawnedObject = true;
                
                if (location.dropObject(forageObj, tileLocation * 64f, Game1.viewport, initialPlacement: true))
                {
                    location.numberOfSpawnedObjectsOnMap++;
                    spawned = true;
                    break;
                }
            }
            
            // If we couldn't spawn after many attempts, there might not be enough valid tiles
            if (!spawned)
                break;
        }
    }
}

/// <summary>
/// Harmony patch to make spawnObjects() always spawn maximum forage
/// This patches the random number generation for number to spawn
/// </summary>
[HarmonyPatch(typeof(GameLocation), nameof(GameLocation.spawnObjects))]
public static class GameLocationSpawnObjectsPatch
{
    /// <summary>
    /// Prefix: Reset numberOfSpawnedObjectsOnMap to 0 before spawning to ensure max spawns
    /// </summary>
    public static void Prefix(GameLocation __instance)
    {
        // Clear existing forage to make room for fresh spawns
        if (__instance?.map == null)
            return;
            
        if (!__instance.IsOutdoors && !__instance.map.Properties.ContainsKey("ForceSpawnForageables"))
            return;
            
        var data = __instance.GetData();
        if (data == null)
            return;
            
        // Remove existing spawned forage objects
        var keysToRemove = new List<Vector2>();
        foreach (var kvp in __instance.objects.Pairs)
        {
            if (kvp.Value.IsSpawnedObject && kvp.Value.isForage())
            {
                keysToRemove.Add(kvp.Key);
            }
        }
        
        foreach (var key in keysToRemove)
        {
            __instance.objects.Remove(key);
        }
        
        // Reset counter so new spawns can fill up to max
        __instance.numberOfSpawnedObjectsOnMap = 0;
    }
}

/// <summary>
/// Harmony patch to override the random spawn count to always use maximum
/// </summary>
[HarmonyPatch(typeof(Random), nameof(Random.Next), typeof(int), typeof(int))]
public static class RandomNextPatch
{
    private static bool inSpawnObjects = false;
    
    public static void SetInSpawnObjects(bool value) => inSpawnObjects = value;
    
    /// <summary>
    /// When called from spawnObjects for determining number to spawn,
    /// always return the maximum value
    /// </summary>
    public static void Postfix(int minValue, int maxValue, ref int __result)
    {
        // This is a broad patch, so we only modify when we know it's for spawn count
        // The spawn count call uses minValue=MinDailyForageSpawn, maxValue=MaxDailyForageSpawn+1
        // We want to always return maxValue-1 (the maximum spawn count)
        if (inSpawnObjects && minValue >= 0 && maxValue > minValue)
        {
            __result = maxValue - 1;
        }
    }
}

/// <summary>
/// Alternative approach: Patch DayUpdate to call spawnObjects multiple times
/// </summary>
[HarmonyPatch(typeof(GameLocation), nameof(GameLocation.DayUpdate))]
public static class GameLocationDayUpdatePatch
{
    /// <summary>
    /// After DayUpdate, ensure all locations have maximum forage
    /// </summary>
    public static void Postfix(GameLocation __instance)
    {
        if (__instance?.map == null)
            return;
            
        if (!__instance.IsOutdoors && !__instance.map.Properties.ContainsKey("ForceSpawnForageables"))
            return;
            
        var data = __instance.GetData();
        if (data == null || data.MaxSpawnedForageAtOnce <= 0)
            return;
            
        // Keep spawning until we reach max capacity
        int maxAttempts = 10;
        int attempts = 0;
        
        while (__instance.numberOfSpawnedObjectsOnMap < data.MaxSpawnedForageAtOnce && attempts < maxAttempts)
        {
            int beforeCount = __instance.numberOfSpawnedObjectsOnMap;
            __instance.spawnObjects();
            attempts++;
            
            if (__instance.numberOfSpawnedObjectsOnMap == beforeCount)
                break;
        }
    }
}
