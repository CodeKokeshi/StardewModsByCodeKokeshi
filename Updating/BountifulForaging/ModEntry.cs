using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Extensions;
using StardewValley.GameData.Locations;
using StardewValley.Internal;

namespace BountifulForaging;

/// <summary>
/// Bountiful Foraging - A simple mod that boosts vanilla forage and artifact spot spawning.
/// 
/// HOW IT WORKS:
/// This mod uses the SAME spawning logic as vanilla, but with boosted values.
/// It runs AFTER vanilla spawning, adding extra items using vanilla-compatible methods.
/// 
/// VANILLA FORAGE SPAWNING (GameLocation.spawnObjects):
/// 1. Builds list of valid SpawnForageData for location + season
/// 2. Picks random count between MinDailyForageSpawn and MaxDailyForageSpawn
/// 3. For each spawn attempt, tries up to 11 random tiles
/// 4. Each forage item has a Chance value (0.0-1.0) that must pass
/// 5. Stops when numberOfSpawnedObjectsOnMap >= MaxSpawnedForageAtOnce
/// 
/// VANILLA ARTIFACT SPOT SPAWNING (same method, after forage):
/// 1. Starts with base chance of 1.0 (100%)
/// 2. For each successful spawn, multiplies chance by 0.75 (decay)
/// 3. In winter, adds 0.1 to chance
/// 4. Spawns on "Diggable" tiles (different from forage "Spawnable" tiles)
/// 5. 16.6% chance to be SeedSpot instead of ArtifactSpot
/// </summary>
public class ModEntry : Mod
{
    internal static ModConfig Config { get; private set; } = null!;
    internal static IMonitor ModMonitor { get; private set; } = null!;

    public override void Entry(IModHelper helper)
    {
        ModMonitor = this.Monitor;
        Config = helper.ReadConfig<ModConfig>();
        
        helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        helper.Events.GameLoop.DayStarted += OnDayStarted;
        
        Monitor.Log("[BountifulForaging] Loaded! Using vanilla-aligned spawning with boosted values.", LogLevel.Info);
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
        if (configMenu is null)
        {
            Monitor.Log("Generic Mod Config Menu not found. Edit config.json manually.", LogLevel.Info);
            return;
        }

        configMenu.Register(
            mod: this.ModManifest,
            reset: () => Config = new ModConfig(),
            save: () => this.Helper.WriteConfig(Config),
            titleScreenOnly: true
        );

        // ===========================================
        // FORAGE SETTINGS
        // ===========================================
        configMenu.AddSectionTitle(
            mod: this.ModManifest,
            text: () => "🌿 Forage Spawning"
        );

        configMenu.AddParagraph(
            mod: this.ModManifest,
            text: () => "These values replace vanilla's location-based spawn limits. Higher = more forage!"
        );

        configMenu.AddNumberOption(
            mod: this.ModManifest,
            name: () => "Min Daily Forage",
            tooltip: () => "Minimum forage spawn attempts per day. Vanilla is typically 1-2 per location.",
            getValue: () => Config.MinDailyForageSpawn,
            setValue: value => Config.MinDailyForageSpawn = value,
            min: 0,
            max: 100,
            interval: 5
        );

        configMenu.AddNumberOption(
            mod: this.ModManifest,
            name: () => "Max Daily Forage",
            tooltip: () => "Maximum forage spawn attempts per day. Vanilla is typically 4-6 per location.",
            getValue: () => Config.MaxDailyForageSpawn,
            setValue: value => Config.MaxDailyForageSpawn = value,
            min: 1,
            max: 200,
            interval: 5
        );

        configMenu.AddNumberOption(
            mod: this.ModManifest,
            name: () => "Max Forage At Once",
            tooltip: () => "Maximum forage items allowed in a location at once. Vanilla is typically 6.",
            getValue: () => Config.MaxSpawnedForageAtOnce,
            setValue: value => Config.MaxSpawnedForageAtOnce = value,
            min: 1,
            max: 500,
            interval: 10
        );

        configMenu.AddNumberOption(
            mod: this.ModManifest,
            name: () => "Forage Spawn Chance %",
            tooltip: () => "Multiplier for individual forage spawn chance. 100% = vanilla, 200% = double chance.",
            getValue: () => Config.ForageChancePercent,
            setValue: value => Config.ForageChancePercent = value,
            min: 50,
            max: 300,
            interval: 10,
            formatValue: value => $"{value}%"
        );

        // ===========================================
        // ARTIFACT SPOT (DIGGING SPOT) SETTINGS
        // ===========================================
        configMenu.AddSectionTitle(
            mod: this.ModManifest,
            text: () => "🪱 Artifact Spots (Digging Spots)"
        );

        configMenu.AddParagraph(
            mod: this.ModManifest,
            text: () => "Artifact spots are the worm/stick tiles you dig up for artifacts, clay, etc."
        );

        configMenu.AddBoolOption(
            mod: this.ModManifest,
            name: () => "Boost Artifact Spots",
            tooltip: () => "Enable extra artifact spot spawning beyond vanilla.",
            getValue: () => Config.BoostArtifactSpots,
            setValue: value => Config.BoostArtifactSpots = value
        );

        configMenu.AddNumberOption(
            mod: this.ModManifest,
            name: () => "Extra Spawn Attempts",
            tooltip: () => "Number of extra artifact spot spawn attempts. Vanilla spawns ~1-3 on average.",
            getValue: () => Config.ExtraArtifactSpotAttempts,
            setValue: value => Config.ExtraArtifactSpotAttempts = value,
            min: 0,
            max: 100,
            interval: 5
        );

        configMenu.AddNumberOption(
            mod: this.ModManifest,
            name: () => "Seed Spot Chance %",
            tooltip: () => "Chance that an artifact spot is a Seed Spot (4-leaf clover) instead. Vanilla is ~17%.",
            getValue: () => Config.SeedSpotChancePercent,
            setValue: value => Config.SeedSpotChancePercent = value,
            min: 0,
            max: 100,
            interval: 5,
            formatValue: value => $"{value}%"
        );

        // ===========================================
        // BEACH SPECIAL ITEMS (coral, sea urchin)
        // ===========================================
        configMenu.AddSectionTitle(
            mod: this.ModManifest,
            text: () => "🐚 Beach Items (Coral & Sea Urchins)"
        );

        configMenu.AddParagraph(
            mod: this.ModManifest,
            text: () => "Corals and sea urchins use special hardcoded spawning in vanilla, separate from normal forage."
        );

        configMenu.AddBoolOption(
            mod: this.ModManifest,
            name: () => "Boost Beach Items",
            tooltip: () => "Enable extra coral and sea urchin spawning on the beach.",
            getValue: () => Config.BoostBeachItems,
            setValue: value => Config.BoostBeachItems = value
        );

        configMenu.AddNumberOption(
            mod: this.ModManifest,
            name: () => "Extra Coral Spawns",
            tooltip: () => "Extra coral/nautilus shell spawns in the tidepool area. Vanilla spawns ~1-2.",
            getValue: () => Config.ExtraBeachCoralSpawns,
            setValue: value => Config.ExtraBeachCoralSpawns = value,
            min: 0,
            max: 50,
            interval: 5
        );

        configMenu.AddNumberOption(
            mod: this.ModManifest,
            name: () => "Extra Sea Urchin Spawns",
            tooltip: () => "Extra sea urchin spawns on the shore. Vanilla spawns ~0-1 (very rare).",
            getValue: () => Config.ExtraBeachUrchinSpawns,
            setValue: value => Config.ExtraBeachUrchinSpawns = value,
            min: 0,
            max: 30,
            interval: 1
        );

        Monitor.Log("GMCM registered successfully!", LogLevel.Debug);
    }

    private void OnDayStarted(object? sender, DayStartedEventArgs e)
    {
        if (!Context.IsWorldReady)
            return;

        Monitor.Log($"=== Bountiful Foraging Starting ===", LogLevel.Info);
        Monitor.Log($"Config: MinDaily={Config.MinDailyForageSpawn}, MaxDaily={Config.MaxDailyForageSpawn}, MaxAtOnce={Config.MaxSpawnedForageAtOnce}, Chance={Config.ForageChancePercent}%", LogLevel.Info);

        int totalForage = 0;
        int totalArtifactSpots = 0;
        int totalBeachItems = 0;

        foreach (GameLocation location in Game1.locations)
        {
            int forageSpawned = SpawnForage(location);
            int artifactSpawned = SpawnArtifactSpots(location);
            int beachSpawned = SpawnBeachItems(location);
            
            if (forageSpawned > 0 || artifactSpawned > 0 || beachSpawned > 0)
                Monitor.Log($"  {location.Name}: +{forageSpawned} forage, +{artifactSpawned} artifact spots, +{beachSpawned} beach items", LogLevel.Info);
            
            totalForage += forageSpawned;
            totalArtifactSpots += artifactSpawned;
            totalBeachItems += beachSpawned;

            // Process buildings
            foreach (var building in location.buildings)
            {
                if (building.indoors.Value != null)
                {
                    totalForage += SpawnForage(building.indoors.Value);
                }
            }
        }

        Monitor.Log($"=== Bountiful Foraging Done: {totalForage} forage + {totalArtifactSpots} artifact spots + {totalBeachItems} beach items ===", LogLevel.Info);
    }

    /// <summary>
    /// Spawns forage using vanilla-like logic with boosted config values.
    /// Based on GameLocation.spawnObjects() - forage section.
    /// </summary>
    private int SpawnForage(GameLocation location)
    {
        if (location?.map == null)
            return 0;

        // Only outdoor locations (or those marked for forage)
        if (!location.IsOutdoors && !location.map.Properties.ContainsKey("ForceSpawnForageables"))
            return 0;

        // Count actual forage objects (don't trust vanilla counter)
        int currentForageCount = 0;
        foreach (var obj in location.objects.Values)
        {
            if (obj.IsSpawnedObject && obj.isForage())
                currentForageCount++;
        }

        // Check if we're at OUR capacity (not vanilla's)
        if (currentForageCount >= Config.MaxSpawnedForageAtOnce)
            return 0;

        // Get valid forage for this location/season (same as vanilla)
        var possibleForage = GetPossibleForage(location);
        if (possibleForage.Count == 0)
            return 0;

        // Determine spawn count (vanilla: random between min and max)
        Random r = Utility.CreateDaySaveRandom(location.NameOrUniqueName.GetHashCode());
        int spawnCount = r.Next(Config.MinDailyForageSpawn, Config.MaxDailyForageSpawn + 1);
        spawnCount = Math.Min(spawnCount, Config.MaxSpawnedForageAtOnce - currentForageCount);

        if (spawnCount <= 0)
            return 0;

        int mapWidth = location.map.DisplayWidth / 64;
        int mapHeight = location.map.DisplayHeight / 64;
        double chanceMultiplier = Config.ForageChancePercent / 100.0;

        ItemQueryContext context = new ItemQueryContext(location, null, r, "BountifulForaging forage");
        int spawned = 0;

        // Spawn loop - we try MORE attempts than vanilla (vanilla only does 11)
        // This ensures we actually hit valid tiles
        for (int i = 0; i < spawnCount; i++)
        {
            for (int attempt = 0; attempt < 50; attempt++)  // 50 attempts instead of vanilla's 11
            {
                int x = r.Next(mapWidth);
                int y = r.Next(mapHeight);
                Vector2 tile = new Vector2(x, y);

                // Vanilla tile checks (exact from decompiled code)
                if (location.objects.ContainsKey(tile))
                    continue;
                if (location.IsNoSpawnTile(tile))
                    continue;
                if (location.doesTileHaveProperty(x, y, "Spawnable", "Back") == null)
                    continue;
                if (location.doesEitherTileOrTileIndexPropertyEqual(x, y, "Spawnable", "Back", "F"))
                    continue;
                if (!location.CanItemBePlacedHere(tile))
                    continue;
                if (location.hasTileAt(x, y, "AlwaysFront") || location.hasTileAt(x, y, "AlwaysFront2") ||
                    location.hasTileAt(x, y, "AlwaysFront3") || location.hasTileAt(x, y, "Front"))
                    continue;
                if (location.isBehindBush(tile))
                    continue;

                // Skip behind tree check entirely for bountiful spawning
                // (vanilla has 90% chance to skip behind trees - we ignore this)

                // Pick random forage (vanilla method)
                SpawnForageData? forage = r.ChooseFrom(possibleForage);
                if (forage == null)
                    continue;

                // Check forage chance (with our multiplier)
                double adjustedChance = Math.Clamp(forage.Chance * chanceMultiplier, 0.0, 1.0);
                if (!r.NextBool(adjustedChance))
                    continue;

                // Create the item (vanilla method - supports modded items)
                Item? item = ItemQueryResolver.TryResolveRandomItem(forage, context, avoidRepeat: false);
                if (item is not StardewValley.Object obj)
                    continue;

                obj.IsSpawnedObject = true;

                if (location.dropObject(obj, tile * 64f, Game1.viewport, initialPlacement: true))
                {
                    location.numberOfSpawnedObjectsOnMap++;
                    spawned++;
                    break; // Success, move to next spawn
                }
            }
        }

        return spawned;
    }

    /// <summary>
    /// Spawns beach-specific items (coral, nautilus shell, sea urchins).
    /// These use HARDCODED spawning in vanilla's Beach.DayUpdate(), separate from SpawnForageData.
    /// 
    /// VANILLA LOGIC (Beach.cs DayUpdate):
    /// - Coral/Nautilus: Rectangle(65,11,25,12) - tidepool area
    ///   - 80% chance Coral (393), 20% chance Nautilus Shell (397)
    ///   - Spawns with 100% initial chance, then halves each time
    /// - Sea Urchin: Rectangle(66,24,19,1) - narrow shore strip  
    ///   - Only 25% initial chance, halves each time, PLUS 10% per-spawn chance
    ///   - Item ID 152
    /// </summary>
    private int SpawnBeachItems(GameLocation location)
    {
        if (!Config.BoostBeachItems)
            return 0;

        // Only works on Beach and BeachNightMarket
        if (location?.Name != "Beach" && location?.Name != "BeachNightMarket")
            return 0;

        Random r = Utility.CreateDaySaveRandom(location.NameOrUniqueName.GetHashCode() + 9999);
        int spawned = 0;

        // ===========================================
        // CORAL / NAUTILUS SHELL - Tidepool area
        // ===========================================
        // Vanilla: Rectangle(65, 11, 25, 12) with items (O)393 (coral) and (O)397 (nautilus)
        Rectangle coralArea = new Rectangle(65, 11, 25, 12);
        
        for (int i = 0; i < Config.ExtraBeachCoralSpawns; i++)
        {
            // 80% coral, 20% nautilus shell (same as vanilla)
            string itemId = r.NextDouble() < 0.2 ? "(O)397" : "(O)393";
            
            // Try a few times to find a valid tile
            for (int attempt = 0; attempt < 10; attempt++)
            {
                int x = r.Next(coralArea.X, coralArea.Right);
                int y = r.Next(coralArea.Y, coralArea.Bottom);
                Vector2 tile = new Vector2(x, y);
                
                if (location.CanItemBePlacedHere(tile) && !location.objects.ContainsKey(tile))
                {
                    var obj = ItemRegistry.Create<StardewValley.Object>(itemId);
                    obj.IsSpawnedObject = true;
                    if (location.dropObject(obj, tile * 64f, Game1.viewport, initialPlacement: true))
                    {
                        spawned++;
                        break;
                    }
                }
            }
        }

        // ===========================================
        // SEA URCHIN - Shore strip
        // ===========================================
        // Vanilla: Rectangle(66, 24, 19, 1) with item (O)152 (sea urchin)
        // In vanilla this is EXTREMELY rare (25% initial * 10% per spawn)
        Rectangle urchinArea = new Rectangle(66, 24, 19, 1);
        
        for (int i = 0; i < Config.ExtraBeachUrchinSpawns; i++)
        {
            // Try a few times to find a valid tile
            for (int attempt = 0; attempt < 10; attempt++)
            {
                int x = r.Next(urchinArea.X, urchinArea.Right);
                int y = r.Next(urchinArea.Y, urchinArea.Bottom);
                Vector2 tile = new Vector2(x, y);
                
                if (location.CanItemBePlacedHere(tile) && !location.objects.ContainsKey(tile))
                {
                    var obj = ItemRegistry.Create<StardewValley.Object>("(O)152");
                    obj.IsSpawnedObject = true;
                    if (location.dropObject(obj, tile * 64f, Game1.viewport, initialPlacement: true))
                    {
                        spawned++;
                        break;
                    }
                }
            }
        }

        return spawned;
    }

    /// <summary>
    /// Spawns artifact spots using vanilla-like logic with boosted attempts.
    /// Based on GameLocation.spawnObjects() - artifact spot section.
    /// </summary>
    private int SpawnArtifactSpots(GameLocation location)
    {
        if (!Config.BoostArtifactSpots || Config.ExtraArtifactSpotAttempts <= 0)
            return 0;

        if (location?.map == null)
            return 0;

        // Only Farm and IslandWest get artifact spots in vanilla (plus other outdoor locations via spawnWeedsAndStones)
        // We'll be more generous and allow any outdoor location
        if (!location.IsOutdoors)
            return 0;

        Random r = Utility.CreateDaySaveRandom(location.NameOrUniqueName.GetHashCode());
        int mapWidth = location.map.DisplayWidth / 64;
        int mapHeight = location.map.DisplayHeight / 64;
        double seedSpotChance = Config.SeedSpotChancePercent / 100.0;

        int spawned = 0;

        // Extra spawn attempts (vanilla uses a decay loop, we just do flat attempts)
        for (int i = 0; i < Config.ExtraArtifactSpotAttempts; i++)
        {
            int x = r.Next(mapWidth);
            int y = r.Next(mapHeight);
            Vector2 tile = new Vector2(x, y);

            // Vanilla artifact spot tile checks
            if (!location.CanItemBePlacedHere(tile))
                continue;
            if (location.IsTileOccupiedBy(tile))
                continue;
            if (location.hasTileAt(x, y, "AlwaysFront") || location.hasTileAt(x, y, "Front"))
                continue;
            if (location.isBehindBush(tile))
                continue;

            // Must be diggable OR grass-type in winter
            bool isDiggable = location.doesTileHaveProperty(x, y, "Diggable", "Back") != null;
            bool isWinterGrass = location.GetSeason() == Season.Winter &&
                                 location.doesTileHaveProperty(x, y, "Type", "Back") == "Grass";

            if (!isDiggable && !isWinterGrass)
                continue;

            // Already has an object
            if (location.objects.ContainsKey(tile))
                continue;

            // Spawn artifact spot or seed spot
            string itemId = r.NextBool(seedSpotChance) ? "(O)SeedSpot" : "(O)590";
            location.objects.Add(tile, ItemRegistry.Create<StardewValley.Object>(itemId));
            spawned++;
        }

        return spawned;
    }

    /// <summary>
    /// Gets valid forage items for this location and season.
    /// Exact vanilla logic from GameLocation.spawnObjects().
    /// </summary>
    private List<SpawnForageData> GetPossibleForage(GameLocation location)
    {
        List<SpawnForageData> result = new();
        Random r = Utility.CreateDaySaveRandom();
        Season season = location.GetSeason();

        // Default forage (applies to all locations)
        var defaultData = GameLocation.GetData("Default");
        if (defaultData?.Forage != null)
        {
            foreach (var forage in defaultData.Forage)
            {
                if (IsForageValidForSeason(forage, season, location, r))
                    result.Add(forage);
            }
        }

        // Location-specific forage
        var locationData = location.GetData();
        if (locationData?.Forage != null)
        {
            foreach (var forage in locationData.Forage)
            {
                if (IsForageValidForSeason(forage, season, location, r))
                    result.Add(forage);
            }
        }

        return result;
    }

    /// <summary>
    /// Checks if forage is valid for current season and conditions.
    /// Exact vanilla logic.
    /// </summary>
    private bool IsForageValidForSeason(SpawnForageData forage, Season season, GameLocation location, Random r)
    {
        // Check condition first (vanilla does this)
        if (forage.Condition != null && !GameStateQuery.CheckConditions(forage.Condition, location, random: r))
            return false;

        // Check season
        if (forage.Season.HasValue && forage.Season.Value != season)
            return false;

        return true;
    }
}


