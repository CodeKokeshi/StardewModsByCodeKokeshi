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
    /// <summary>Static config instance accessible by the whole mod.</summary>
    internal static ModConfig Config { get; private set; } = null!;
    
    /// <summary>Static monitor for logging from static methods.</summary>
    internal static IMonitor ModMonitor { get; private set; } = null!;

    public override void Entry(IModHelper helper)
    {
        ModMonitor = this.Monitor;
        Config = helper.ReadConfig<ModConfig>();
        
        var harmony = new Harmony(this.ModManifest.UniqueID);
        harmony.PatchAll();
        
        // Register GMCM on game launch
        helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        
        // Run our aggressive forage spawn after the day starts
        helper.Events.GameLoop.DayStarted += OnDayStarted;
        
        Monitor.Log("[BountifulForaging] Loaded! Forage will spawn bountifuly every day.", LogLevel.Info);
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        // Get Generic Mod Config Menu's API (if it's installed)
        var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
        if (configMenu is null)
        {
            Monitor.Log("Generic Mod Config Menu not found. Config can only be edited via config.json", LogLevel.Info);
            return;
        }

        // Register mod - TITLE SCREEN ONLY
        configMenu.Register(
            mod: this.ModManifest,
            reset: () => Config = new ModConfig(),
            save: () => this.Helper.WriteConfig(Config),
            titleScreenOnly: true
        );

        // === Forage Settings ===
        configMenu.AddSectionTitle(
            mod: this.ModManifest,
            text: () => "Forage Bounty Settings"
        );

        configMenu.AddParagraph(
            mod: this.ModManifest,
            text: () => "Configure how much forage spawns in each area every day. Higher values = more forageables!"
        );

        // Forage Multiplier (1x - 15x, default 4x)
        configMenu.AddNumberOption(
            mod: this.ModManifest,
            name: () => "Forage Multiplier",
            tooltip: () => "Multiplies the game's default max forage per area. 4x is recommended, 15x is maximum bounty!",
            getValue: () => Config.ForageMultiplier,
            setValue: value => Config.ForageMultiplier = value,
            min: 1,
            max: 15,
            interval: 1,
            formatValue: value => $"{value}x"
        );

        // Minimum Forage Per Location (1-20, default 4)
        configMenu.AddNumberOption(
            mod: this.ModManifest,
            name: () => "Minimum Per Area",
            tooltip: () => "Minimum forage to spawn even in areas with no default forage. Ensures no empty areas!",
            getValue: () => Config.MinimumForagePerLocation,
            setValue: value => Config.MinimumForagePerLocation = value,
            min: 1,
            max: 20,
            interval: 1
        );

        // Maximum Forage Per Location (10-50, default 50)
        configMenu.AddNumberOption(
            mod: this.ModManifest,
            name: () => "Maximum Per Area",
            tooltip: () => "Maximum forage per area to prevent performance issues. Lower this if you experience lag.",
            getValue: () => Config.MaxForagePerLocation,
            setValue: value => Config.MaxForagePerLocation = value,
            min: 10,
            max: 50,
            interval: 5
        );

        Monitor.Log("Generic Mod Config Menu integration complete!", LogLevel.Debug);
    }

    private void OnDayStarted(object? sender, DayStartedEventArgs e)
    {
        if (!Context.IsWorldReady)
            return;
            
        Monitor.Log($"Bountiful Foraging: Spawning forage with {Config.ForageMultiplier}x multiplier...", LogLevel.Debug);
        
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
        
        Monitor.Log($"Bountiful Foraging: Spawned {totalSpawned} total forage items!", LogLevel.Debug);
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

        // Calculate how much to spawn using config values.
        // Important: don't clear or replace existing forage, and don't block vanilla/other-mod spawns.
        int gameMax = data?.MaxSpawnedForageAtOnce ?? 6;
        int targetAmount = Math.Max(gameMax * Config.ForageMultiplier, Config.MinimumForagePerLocation);
        targetAmount = Math.Min(targetAmount, Config.MaxForagePerLocation);

        int existingSpawnedForage = 0;
        foreach (var kvp in location.objects.Pairs)
        {
            if (kvp.Value.IsSpawnedObject && kvp.Value.isForage())
                existingSpawnedForage++;
        }

        int extraToSpawn = Math.Max(0, targetAmount - existingSpawnedForage);
        if (extraToSpawn <= 0)
            return 0;
        
        // Spawn the forage!
        int spawned = SpawnForageItems(location, possibleForage, extraToSpawn);
        
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
            
        // Check condition
        if (spawn.Condition != null)
        {
            try
            {
                if (!GameStateQuery.CheckConditions(spawn.Condition, location, null, null, null, r))
                    return false;
            }
            catch
            {
                // If condition parsing fails, treat it as invalid rather than spawning unexpected items.
                ModMonitor?.Log($"Bountiful Foraging: ignoring invalid condition '{spawn.Condition}' for forage '{spawn.Id}' in '{location?.NameOrUniqueName}'.", LogLevel.Trace);
                return false;
            }
        }
        
        return true;
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

            // Respect the forage entry's chance (matches vanilla behavior more closely)
            if (!r.NextBool(forage.Chance))
                continue;
            
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
        
        // Forage should only spawn on the map's spawnable tiles.
        // Avoid treating diggable tiles as forage spawn tiles, otherwise we can crowd out artifact spots.
        if (location.doesTileHaveProperty(x, y, "Spawnable", "Back") == null)
            return false;
        
        // Explicitly marked as not spawnable
        if (location.doesEitherTileOrTileIndexPropertyEqual(x, y, "Spawnable", "Back", "F"))
            return false;
        
        // Can't place items here
        if (!location.CanItemBePlacedHere(tile))
            return false;

        // Avoid spawning under front-layer tiles (closer to vanilla)
        if (location.hasTileAt(x, y, "AlwaysFront") || location.hasTileAt(x, y, "AlwaysFront2") || location.hasTileAt(x, y, "AlwaysFront3") || location.hasTileAt(x, y, "Front"))
            return false;

        // Avoid spawning behind bushes (vanilla behavior)
        try
        {
            if (location.isBehindBush(tile))
                return false;
        }
        catch
        {
            // ignore if not supported
        }
        
        // If blocked, skip
        try
        {
            if (location.IsTileBlockedBy(tile))
                return false;
        }
        catch
        {
            // ignore if not supported
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


