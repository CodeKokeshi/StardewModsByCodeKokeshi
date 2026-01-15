using System;
using System.Collections.Generic;
using System.Linq;
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
    // ============================================
    // CONFIGURATION - CHANGE THESE VALUES!
    // ============================================
    
    /// <summary>
    /// Multiplier for how much forage to spawn beyond the game's default max.
    /// 1 = game default, 2 = double, 5 = 5x more, 10 = 10x more
    /// </summary>
    public const int FORAGE_MULTIPLIER = 5;
    
    /// <summary>
    /// Minimum forage to spawn per location regardless of game data.
    /// This ensures even "empty" areas get forage.
    /// </summary>
    public const int MINIMUM_FORAGE_PER_LOCATION = 10;
    
    /// <summary>
    /// Maximum forage per location (to prevent lag in huge areas)
    /// </summary>
    public const int ABSOLUTE_MAX_FORAGE = 100;
    
    // ============================================

    public override void Entry(IModHelper helper)
    {
        
        var harmony = new Harmony(this.ModManifest.UniqueID);
        harmony.PatchAll();
        
        // Run our aggressive forage spawn after the day starts
        helper.Events.GameLoop.DayStarted += OnDayStarted;
    }

    private void OnDayStarted(object? sender, DayStartedEventArgs e)
    {
        if (!Context.IsWorldReady)
            return;
            
        Monitor?.Log($"Bountiful Foraging: Spawning forage with {FORAGE_MULTIPLIER}x multiplier...", LogLevel.Info);
        
        int totalSpawned = 0;
        
        // Process ALL locations
        foreach (GameLocation location in Game1.locations)
        {
            totalSpawned += ForceSpawnForage(location);
            
            // Also handle buildings within locations
            foreach (var building in location.buildings)
            {
                if (building.indoors.Value != null)
                {
                    totalSpawned += ForceSpawnForage(building.indoors.Value);
                }
            }
        }
        
        Monitor?.Log($"Bountiful Foraging: Spawned {totalSpawned} total forage items!", LogLevel.Info);
    }

    /// <summary>
    /// Aggressively spawns forage in a location, ignoring game limits
    /// </summary>
    public static int ForceSpawnForage(GameLocation location)
    {
        if (location?.map == null)
            return 0;
            
        // Only process outdoor locations or those with ForceSpawnForageables
        if (!location.IsOutdoors && !location.map.Properties.ContainsKey("ForceSpawnForageables"))
            return 0;

        var data = location.GetData();
        
        // Get possible forage for this location
        List<SpawnForageData> possibleForage = GetPossibleForage(location, data);
        
        if (possibleForage.Count == 0)
            return 0;

        // Clear ALL existing spawned forage first
        ClearExistingForage(location);
        
        // Calculate how much to spawn
        int gameMax = data?.MaxSpawnedForageAtOnce ?? 6;
        int targetAmount = Math.Max(gameMax * FORAGE_MULTIPLIER, MINIMUM_FORAGE_PER_LOCATION);
        targetAmount = Math.Min(targetAmount, ABSOLUTE_MAX_FORAGE);
        
        // Spawn the forage!
        int spawned = SpawnForageItems(location, possibleForage, targetAmount);
        
        return spawned;
    }

    /// <summary>
    /// Gets all valid forage items for the current location and season
    /// </summary>
    private static List<SpawnForageData> GetPossibleForage(GameLocation location, LocationData? data)
    {
        List<SpawnForageData> possibleForage = new List<SpawnForageData>();
        Random r = Utility.CreateDaySaveRandom();
        Season season = location.GetSeason();
        
        // Get default forage (applies to all locations)
        var defaultData = GameLocation.GetData("Default");
        if (defaultData?.Forage != null)
        {
            foreach (SpawnForageData spawn in defaultData.Forage)
            {
                if (IsForageValid(spawn, location, season, r))
                {
                    possibleForage.Add(spawn);
                }
            }
        }
        
        // Get location-specific forage
        if (data?.Forage != null)
        {
            foreach (SpawnForageData spawn in data.Forage)
            {
                if (IsForageValid(spawn, location, season, r))
                {
                    possibleForage.Add(spawn);
                }
            }
        }
        
        return possibleForage;
    }

    private static bool IsForageValid(SpawnForageData spawn, GameLocation location, Season season, Random r)
    {
        // Check season
        if (spawn.Season.HasValue && spawn.Season != season)
            return false;
            
        // Check condition (but be lenient)
        if (spawn.Condition != null)
        {
            try
            {
                if (!GameStateQuery.CheckConditions(spawn.Condition, location, null, null, null, r))
                    return false;
            }
            catch
            {
                // If condition check fails, allow it anyway
            }
        }
        
        return true;
    }

    /// <summary>
    /// Removes all existing spawned forage from a location
    /// </summary>
    private static void ClearExistingForage(GameLocation location)
    {
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
        
        location.numberOfSpawnedObjectsOnMap = 0;
    }

    /// <summary>
    /// Spawns forage items in valid locations
    /// </summary>
    private static int SpawnForageItems(GameLocation location, List<SpawnForageData> possibleForage, int targetAmount)
    {
        Random r = new Random((int)(Game1.uniqueIDForThisGame + (ulong)Game1.stats.DaysPlayed + (ulong)location.NameOrUniqueName.GetHashCode()));
        ItemQueryContext itemQueryContext = new ItemQueryContext(location, null, r, "BountifulForaging");
        
        int spawned = 0;
        int mapWidth = location.map.DisplayWidth / 64;
        int mapHeight = location.map.DisplayHeight / 64;
        
        // Collect all valid spawn tiles first for better distribution
        List<Vector2> validTiles = GetValidSpawnTiles(location, mapWidth, mapHeight);
        
        if (validTiles.Count == 0)
            return 0;
        
        // Shuffle for random distribution
        ShuffleList(validTiles, r);
        
        int tileIndex = 0;
        int maxAttempts = targetAmount * 3; // Allow extra attempts
        int attempts = 0;
        
        while (spawned < targetAmount && attempts < maxAttempts && tileIndex < validTiles.Count)
        {
            attempts++;
            Vector2 tile = validTiles[tileIndex % validTiles.Count];
            tileIndex++;
            
            // Skip if something is already there
            if (location.objects.ContainsKey(tile))
                continue;
            
            // Pick a random forage type
            SpawnForageData forage = possibleForage[r.Next(possibleForage.Count)];
            
            // Create the forage item
            Item? forageItem = null;
            try
            {
                forageItem = ItemQueryResolver.TryResolveRandomItem(forage, itemQueryContext, avoidRepeat: false, null, null, null, delegate { });
            }
            catch
            {
                continue;
            }
            
            if (forageItem is not StardewValley.Object forageObj)
                continue;
            
            forageObj.IsSpawnedObject = true;
            
            // Place the forage
            if (location.dropObject(forageObj, tile * 64f, Game1.viewport, initialPlacement: true))
            {
                location.numberOfSpawnedObjectsOnMap++;
                spawned++;
            }
        }
        
        return spawned;
    }

    /// <summary>
    /// Gets all tiles that are valid for forage spawning
    /// </summary>
    private static List<Vector2> GetValidSpawnTiles(GameLocation location, int mapWidth, int mapHeight)
    {
        List<Vector2> validTiles = new List<Vector2>();
        
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                Vector2 tile = new Vector2(x, y);
                
                if (IsTileValidForForage(location, x, y, tile))
                {
                    validTiles.Add(tile);
                }
            }
        }
        
        return validTiles;
    }

    /// <summary>
    /// Checks if a tile is valid for forage spawning (more lenient than vanilla)
    /// </summary>
    private static bool IsTileValidForForage(GameLocation location, int x, int y, Vector2 tile)
    {
        // Already has object
        if (location.objects.ContainsKey(tile))
            return false;
        
        // No spawn tile
        if (location.IsNoSpawnTile(tile))
            return false;
        
        // Check for spawnable OR diggable property (more lenient)
        bool isSpawnable = location.doesTileHaveProperty(x, y, "Spawnable", "Back") != null;
        bool isDiggable = location.doesTileHaveProperty(x, y, "Diggable", "Back") != null;
        bool isGrass = location.doesTileHaveProperty(x, y, "Type", "Back") == "Grass";
        
        if (!isSpawnable && !isDiggable && !isGrass)
            return false;
        
        // Explicitly marked as not spawnable
        if (location.doesEitherTileOrTileIndexPropertyEqual(x, y, "Spawnable", "Back", "F"))
            return false;
        
        // Can't place items here
        if (!location.CanItemBePlacedHere(tile))
            return false;
        
        // Behind bush (skip this check to allow more spawns)
        // if (location.isBehindBush(tile))
        //     return false;
        
        // Has front layer tile (building, etc)
        if (location.isTileOnMap(tile))
        {
            // Be more lenient - only skip if there's actually something blocking
            try
            {
                if (location.IsTileBlockedBy(tile))
                    return false;
            }
            catch
            {
                // Ignore errors
            }
        }
        
        return true;
    }

    private static void ShuffleList<T>(List<T> list, Random r)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = r.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}

/// <summary>
/// Patch to prevent vanilla spawn from interfering (we handle everything ourselves)
/// </summary>
[HarmonyPatch(typeof(GameLocation), nameof(GameLocation.spawnObjects))]
public static class GameLocationSpawnObjectsPatch
{
    /// <summary>
    /// Skip vanilla spawn logic - we handle forage spawning entirely in DayStarted
    /// </summary>
    public static bool Prefix(GameLocation __instance)
    {
        // Return false to skip the original method entirely
        // Our DayStarted handler will spawn all the forage we need
        return false;
    }
}
