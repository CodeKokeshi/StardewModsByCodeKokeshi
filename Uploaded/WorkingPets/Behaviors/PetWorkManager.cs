using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Characters;
using StardewValley.BellsAndWhistles;

namespace WorkingPets.Behaviors
{
    /// <summary>Work type categories for priority system.</summary>
    public enum WorkType
    {
        Debris,
        StumpsAndLogs,
        Trees,
        Boulders
    }

    /// <summary>Manages pet work behavior.</summary>
    public class PetWorkManager
    {
        /*********
        ** Constants
        *********/
        private const int TREE_HEALTH = 10;
        private const int STUMP_HEALTH = 5;
        private const int BOULDER_HEALTH = 8;

        /*********
        ** Fields
        *********/
        private Pet? _pet;
        private bool _isWorking;
        private bool _isFollowing;
        private int _tickCounter;
        
        // Movement & Action State
        private Vector2? _targetTile;
        private Action? _pendingAction;
        private readonly float _moveSpeed = 3f;
        
        // Track unreachable targets to avoid trying them repeatedly
        private readonly HashSet<Vector2> _unreachableTiles = new();
        private int _unreachableClearTimer;

        // Stuck detection (fallback warp near target)
        private float _lastDistanceToTarget = float.MaxValue;
        private int _noProgressTicks;
        private Vector2? _lastTargetTile;

        // Follow-mode stuck detection
        private float _lastDistanceToPlayer = float.MaxValue;
        private int _noProgressFollowTicks;

        private readonly Random _random = new();

        // Track damage for multi-hit objects
        private readonly Dictionary<Vector2, int> _objectDamage = new();

        /*********
        ** Properties
        *********/
        public bool IsWorking => _isWorking;
        public bool IsFollowing => _isFollowing;
        public Pet? Pet => _pet;

        /*********
        ** Public methods
        *********/
        public void Initialize(Pet pet)
        {
            _pet = pet;
            LoadState(pet);
        }

        public void ToggleWork()
        {
            _isWorking = !_isWorking;
            _tickCounter = 0;
            _targetTile = null;
            _pendingAction = null;

            if (_isWorking)
            {
                ModEntry.Instance.Monitor.Log($"[WorkingPets] === {_pet?.Name} STARTED WORKING ===", LogLevel.Info);
                ModEntry.Instance.Monitor.Log($"[WorkingPets] Pet tile: {_pet?.Tile}, Location: {_pet?.currentLocation?.Name}", LogLevel.Info);
            }
            else
            {
                ModEntry.Instance.Monitor.Log($"[WorkingPets] === {_pet?.Name} STOPPED WORKING ===", LogLevel.Info);
                StopWorkImmediately();
            }
        }

        public void ToggleFollow()
        {
            _isFollowing = !_isFollowing;
            
            // Stop working if we start following
            if (_isFollowing)
            {
                _isWorking = false;
                _targetTile = null;
                _pendingAction = null;
                ModEntry.Instance.Monitor.Log($"[WorkingPets] === {_pet?.Name} FOLLOWING PLAYER ===", LogLevel.Info);
            }
            else
            {
                ModEntry.Instance.Monitor.Log($"[WorkingPets] === {_pet?.Name} STOPPED FOLLOWING ===", LogLevel.Info);
            }
        }

        public void StopFollowing()
        {
            if (_isFollowing)
            {
                _isFollowing = false;
                ModEntry.Instance.Monitor.Log($"[WorkingPets] === {_pet?.Name} STOPPED FOLLOWING ===", LogLevel.Info);
            }
        }

        public void Update()
        {
            if (_pet == null)
            {
                return;
            }

            // Let the pet rest if it's sleeping / showing the Zzz emote.
            // This avoids the mod force-moving the pet while the game wants it to be asleep.
            if (IsPetResting())
            {
                return;
            }

            // Handle following mode
            if (_isFollowing)
            {
                UpdateFollow();
                return;
            }

            if (!_isWorking)
            {
                return;
            }

            Farm? farm = Game1.getFarm();
            if (farm == null)
            {
                return;
            }

            // Ensure pet is on farm when working
            if (_pet.currentLocation?.Name != "Farm")
            {
                ModEntry.Instance.Monitor.Log($"[WorkingPets] Pet not on farm, warping. Current location: {_pet.currentLocation?.Name ?? "null"}", LogLevel.Debug);
                Game1.warpCharacter(_pet, "Farm", new Vector2(64, 15));
                return;
            }

            // If we have a target, move towards it
            if (_targetTile.HasValue)
            {
                ModEntry.Instance.Monitor.Log($"[WorkingPets] Moving to target: {_targetTile.Value}, HasController: {_pet.controller != null}", LogLevel.Trace);
                MoveTowardTarget();
                return;
            }

            // Otherwise, scan for work periodically
            _tickCounter++;
            if (_tickCounter < ModEntry.Config.TicksBetweenActions)
                return;
            
            ModEntry.Instance.Monitor.Log($"[WorkingPets] Scanning for work... (tick {_tickCounter})", LogLevel.Debug);
            _tickCounter = 0;

            ScanForWork(farm);
        }

        public void SaveState(Pet pet)
        {
            if (pet == null) return;
            pet.modData["WorkingPets.IsWorking"] = _isWorking.ToString();
        }

        public void LoadState(Pet pet)
        {
            if (pet == null) return;
            if (pet.modData.TryGetValue("WorkingPets.IsWorking", out string? value))
            {
                _isWorking = bool.TryParse(value, out bool result) && result;
            }
        }

        /*********
        ** Private methods
        *********/
        
        private void StopWorkImmediately()
        {
            if (_pet == null) return;

            _pet.Halt();
            _pet.controller = null;
            _targetTile = null;
            _pendingAction = null;
        }

        private bool IsPetResting()
        {
            if (_pet == null)
            {
                return false;
            }

            // Bed sleeping state.
            if (_pet.isSleepingOnFarmerBed?.Value == true)
            {
                return true;
            }

            // Emotes: Zzz (sleep) and (sometimes) sad.
            // Note: the backing fields are protected; use the public properties.
            if (_pet.IsEmoting && (_pet.CurrentEmote == Character.sleepEmote || _pet.CurrentEmote == Character.sadEmote))
            {
                return true;
            }

            // General sleep behavior.
            if (string.Equals(_pet.CurrentBehavior, Pet.behavior_Sleep, StringComparison.Ordinal))
            {
                return true;
            }

            return false;
        }

        private void MoveTowardTarget()
        {
            if (_pet == null || !_targetTile.HasValue || _pet.currentLocation == null) return;

            Vector2 targetPos = _targetTile.Value * 64f + new Vector2(32, 32);
            Vector2 currentPos = _pet.Position + new Vector2(32, 32);
            float distance = Vector2.Distance(currentPos, targetPos);

            // Detect if we're not making progress toward the target.
            // If we get "stuck" (e.g. pet AI/physics fighting us, odd collisions), warp near the target.
            if (!_lastTargetTile.HasValue || _lastTargetTile.Value != _targetTile.Value)
            {
                _lastTargetTile = _targetTile.Value;
                _lastDistanceToTarget = distance;
                _noProgressTicks = 0;
            }
            else
            {
                // Consider it progress only if we got meaningfully closer.
                if (distance < _lastDistanceToTarget - 1f)
                {
                    _lastDistanceToTarget = distance;
                    _noProgressTicks = 0;
                }
                else
                {
                    _noProgressTicks++;
                }

                // After ~2 seconds with no progress, warp to a safe tile adjacent to the target.
                if (_noProgressTicks >= 120)
                {
                    if (TryWarpPetNearTarget(_pet.currentLocation, _targetTile.Value))
                    {
                        _noProgressTicks = 0;
                        _lastDistanceToTarget = float.MaxValue;
                    }
                    else
                    {
                        // If we can't find a safe adjacent tile, treat as unreachable and bail.
                        ModEntry.Instance.Monitor.Log($"[WorkingPets] Couldn't warp near target {_targetTile.Value}; marking unreachable.", LogLevel.Debug);
                        _unreachableTiles.Add(_targetTile.Value);
                        _targetTile = null;
                        _pendingAction = null;
                        return;
                    }
                }
            }

            // Arrived!
            if (distance < 55f)
            {
                ModEntry.Instance.Monitor.Log($"[WorkingPets] ARRIVED at {_targetTile.Value}! Executing action.", LogLevel.Info);
                _pet.Halt();
                _pendingAction?.Invoke();
                _targetTile = null;
                _pendingAction = null;

                _lastTargetTile = null;
                _lastDistanceToTarget = float.MaxValue;
                _noProgressTicks = 0;
                return;
            }

            // Calculate next position
            Vector2 direction = targetPos - currentPos;
            direction.Normalize();
            Vector2 nextPos = _pet.Position + direction * _moveSpeed;
            
            // Check multiple points around the pet for water/obstacles
            var location = _pet.currentLocation;
            if (IsNearWaterOrBlocked(location, nextPos))
            {
                // If we can't take a step toward the target, try the "stuck" warp fallback first.
                // This matches the expected behavior: if stuck, teleport 1 tile away from the destination.
                if (TryWarpPetNearTarget(location, _targetTile.Value))
                {
                    _lastTargetTile = _targetTile.Value;
                    _lastDistanceToTarget = float.MaxValue;
                    _noProgressTicks = 0;
                    return;
                }

                ModEntry.Instance.Monitor.Log($"[WorkingPets] Blocked near water/obstacle and couldn't warp; skipping target {_targetTile.Value}", LogLevel.Debug);
                _unreachableTiles.Add(_targetTile.Value);
                _targetTile = null;
                _pendingAction = null;

                _lastTargetTile = null;
                _lastDistanceToTarget = float.MaxValue;
                _noProgressTicks = 0;
                return;
            }

            // Move the pet
            _pet.Position = nextPos;

            // Face the right direction
            if (Math.Abs(direction.X) > Math.Abs(direction.Y))
                _pet.faceDirection(direction.X > 0 ? 1 : 3);
            else
                _pet.faceDirection(direction.Y > 0 ? 2 : 0);

            _pet.animateInFacingDirection(Game1.currentGameTime);
        }

        private bool IsNearWaterOrBlocked(GameLocation location, Vector2 pixelPos)
        {
            // Check the corners and center of the pet's bounding box
            // Pet sprite is roughly 64x64, so check points around the position
            float[] offsetsX = { -24, 0, 24 };
            float[] offsetsY = { -24, 0, 24 };

            foreach (float ox in offsetsX)
            {
                foreach (float oy in offsetsY)
                {
                    int tileX = (int)((pixelPos.X + ox) / 64);
                    int tileY = (int)((pixelPos.Y + oy) / 64);

                    if (location.isWaterTile(tileX, tileY))
                        return true;
                }
            }

            // Also check if the main tile is impassable
            int mainTileX = (int)(pixelPos.X / 64);
            int mainTileY = (int)(pixelPos.Y / 64);
            if (!location.isTilePassable(new xTile.Dimensions.Location(mainTileX, mainTileY), Game1.viewport))
                return true;

            return false;
        }

        private void UpdateFollow()
        {
            if (_pet == null) return;

            Farmer player = Game1.player;
            
            // Check if player left the farm and following outside is disabled
            if (player.currentLocation?.Name != "Farm" && !ModEntry.Config.FollowOutsideFarm)
            {
                // Stop following and return pet to idle
                _isFollowing = false;
                ModEntry.Instance.Monitor.Log($"[WorkingPets] Player left farm, {_pet.Name} stopped following.", LogLevel.Debug);
                return;
            }
            
            // Warp pet to player's location if different (only if allowed outside farm)
            if (_pet.currentLocation != player.currentLocation)
            {
                if (player.currentLocation?.Name == "Farm" || ModEntry.Config.FollowOutsideFarm)
                {
                    Game1.warpCharacter(_pet, player.currentLocation, player.Tile);
                }
                return;
            }

            Vector2 playerPos = player.Position;
            Vector2 petPos = _pet.Position;
            float distance = Vector2.Distance(petPos, playerPos);

            // Stay close but not too close (follow at ~2 tiles behind)
            float followDistance = 96f; // 1.5 tiles
            float stopDistance = 64f;   // 1 tile
            
            if (distance <= stopDistance)
            {
                // Close enough, just face the player
                _pet.Halt();

                _lastDistanceToPlayer = float.MaxValue;
                _noProgressFollowTicks = 0;
                return;
            }

            if (distance > followDistance)
            {
                // Move toward player
                Vector2 direction = playerPos - petPos;
                direction.Normalize();

                // Catch-up speed scaling: if the player is far, move faster.
                // This keeps following responsive without permanently changing base speed.
                float speed = _moveSpeed;
                if (distance > 512f)
                    speed *= 2.0f;
                else if (distance > 256f)
                    speed *= 1.5f;

                Vector2 nextPos = _pet.Position + direction * speed;

                // Check for water/obstacles using improved detection
                var location = _pet.currentLocation;
                
                if (IsNearWaterOrBlocked(location, nextPos))
                {
                    // If blocked for too long while trying to follow, warp near the player.
                    _noProgressFollowTicks++;
                    if (_noProgressFollowTicks >= 120)
                    {
                        if (TryWarpPetNearTarget(location, player.Tile))
                        {
                            _noProgressFollowTicks = 0;
                            _lastDistanceToPlayer = float.MaxValue;
                        }
                    }
                    return;
                }

                // Progress tracking: if we aren't getting closer to the player, count it as stuck.
                if (distance < _lastDistanceToPlayer - 1f)
                {
                    _lastDistanceToPlayer = distance;
                    _noProgressFollowTicks = 0;
                }
                else
                {
                    _noProgressFollowTicks++;
                    if (_noProgressFollowTicks >= 120)
                    {
                        if (TryWarpPetNearTarget(location, player.Tile))
                        {
                            _noProgressFollowTicks = 0;
                            _lastDistanceToPlayer = float.MaxValue;
                        }
                    }
                }

                _pet.Position = nextPos;

                // Face the right direction
                if (Math.Abs(direction.X) > Math.Abs(direction.Y))
                    _pet.faceDirection(direction.X > 0 ? 1 : 3);
                else
                    _pet.faceDirection(direction.Y > 0 ? 2 : 0);

                _pet.animateInFacingDirection(Game1.currentGameTime);
            }
        }

        private void ScanForWork(Farm farm)
        {
            Vector2 petTile = _pet!.Tile;
            var config = ModEntry.Config;

            // Clear unreachable tiles periodically (every ~10 seconds)
            _unreachableClearTimer++;
            if (_unreachableClearTimer > 600)
            {
                _unreachableTiles.Clear();
                _unreachableClearTimer = 0;
            }

            ModEntry.Instance.Monitor.Log($"[WorkingPets] ScanForWork at tile {petTile}", LogLevel.Debug);
            ModEntry.Instance.Monitor.Log($"[WorkingPets] Config - Debris:{config.ClearDebris}, Stumps:{config.ClearStumpsAndLogs}, Trees:{config.ChopTrees}, Boulders:{config.BreakBoulders}", LogLevel.Debug);

            if (config.IgnorePriority)
            {
                ScanForNearestTarget(farm, petTile);
            }
            else
            {
                ScanByPriority(farm, petTile);
            }
        }

        private void ScanByPriority(Farm farm, Vector2 petTile)
        {
            // Get work types sorted by priority (lower number = higher priority)
            var enabledTypes = GetEnabledWorkTypesByPriority();

            foreach (var workType in enabledTypes)
            {
                bool found = workType switch
                {
                    WorkType.Debris => TryClearDebris(farm, petTile),
                    WorkType.StumpsAndLogs => TryRemoveStump(farm, petTile),
                    WorkType.Trees => TryChopTree(farm, petTile),
                    WorkType.Boulders => TryBreakBoulder(farm, petTile),
                    _ => false
                };

                if (found) return;
            }
        }

        private void ScanForNearestTarget(Farm farm, Vector2 petTile)
        {
            var config = ModEntry.Config;
            var candidates = new List<(Vector2 tile, float distance, Action action)>();
            int workRadius = config.WorkRadius;

            // Debris
            if (config.ClearDebris)
            {
                foreach (var pair in farm.objects.Pairs)
                {
                    if (IsDebris(pair.Value))
                    {
                        float dist = Vector2.Distance(petTile, pair.Key);
                        if (dist <= workRadius)
                        {
                            var tile = pair.Key;
                            var obj = pair.Value;
                            candidates.Add((tile, dist, () => ClearDebrisAt(farm, tile, obj)));
                        }
                    }
                }
            }

            // Trees
            if (config.ChopTrees)
            {
                foreach (var pair in farm.terrainFeatures.Pairs)
                {
                    if (pair.Value is Tree tree && tree.growthStage.Value >= 5 && !tree.stump.Value)
                    {
                        float dist = Vector2.Distance(petTile, pair.Key);
                        if (dist <= workRadius)
                        {
                            var tile = pair.Key;
                            candidates.Add((tile, dist, () => ChopTreeAt(farm, tile, tree)));
                        }
                    }
                }
            }

            // Stumps
            if (config.ClearStumpsAndLogs)
            {
                foreach (var pair in farm.terrainFeatures.Pairs)
                {
                    if (pair.Value is Tree tree && tree.stump.Value)
                    {
                        float dist = Vector2.Distance(petTile, pair.Key);
                        if (dist <= workRadius)
                        {
                            var tile = pair.Key;
                            candidates.Add((tile, dist, () => RemoveStumpAt(farm, tile, tree)));
                        }
                    }
                }

                // Large stumps/logs
                foreach (var clump in farm.resourceClumps)
                {
                    if (clump.parentSheetIndex.Value == 600 || clump.parentSheetIndex.Value == 602)
                    {
                        float dist = Vector2.Distance(petTile, clump.Tile);
                        if (dist <= workRadius)
                        {
                            var targetClump = clump;
                            candidates.Add((clump.Tile, dist, () => RemoveLargeStump(farm, targetClump)));
                        }
                    }
                }
            }

            // Boulders
            if (config.BreakBoulders)
            {
                foreach (var clump in farm.resourceClumps)
                {
                    if (IsBoulderClump(clump.parentSheetIndex.Value))
                    {
                        float dist = Vector2.Distance(petTile, clump.Tile);
                        if (dist <= workRadius)
                        {
                            var targetClump = clump;
                            candidates.Add((clump.Tile, dist, () => BreakBoulder(farm, targetClump)));
                        }
                    }
                }
            }

            // Pick the nearest
            if (candidates.Count > 0)
            {
                var nearest = candidates.OrderBy(c => c.distance).First();
                SetJob(nearest.tile, nearest.action);
            }
        }

        private List<WorkType> GetEnabledWorkTypesByPriority()
        {
            var config = ModEntry.Config;
            var types = new List<(WorkType type, int priority, bool enabled)>
            {
                (WorkType.Debris, config.DebrisPriority, config.ClearDebris),
                (WorkType.StumpsAndLogs, config.StumpsAndLogsPriority, config.ClearStumpsAndLogs),
                (WorkType.Trees, config.TreesPriority, config.ChopTrees),
                (WorkType.Boulders, config.BouldersPriority, config.BreakBoulders)
            };

            return types
                .Where(t => t.enabled)
                .OrderBy(t => t.priority)
                .Select(t => t.type)
                .ToList();
        }

        private bool IsDebris(StardewValley.Object obj)
        {
            return obj.Name.Contains("Weed") || 
                   obj.Name.Contains("Stone") || 
                   obj.Name.Contains("Twig") || 
                   obj.ParentSheetIndex == 294 || 
                   obj.ParentSheetIndex == 295 ||
                   obj.Category == StardewValley.Object.litterCategory;
        }

        private bool IsBoulderClump(int parentSheetIndex)
        {
            return parentSheetIndex == 672 || 
                   parentSheetIndex == 752 || 
                   parentSheetIndex == 754 || 
                   parentSheetIndex == 756 || 
                   parentSheetIndex == 758;
        }

        private bool TryClearDebris(Farm farm, Vector2 petTile)
        {
            if (!ModEntry.Config.ClearDebris) return false;

            KeyValuePair<Vector2, StardewValley.Object>? target = null;
            float nearestDist = float.MaxValue;

            foreach (var pair in farm.objects.Pairs)
            {
                if (!IsDebris(pair.Value)) continue;

                float dist = Vector2.Distance(petTile, pair.Key);
                if (dist > ModEntry.Config.WorkRadius || dist >= nearestDist)
                    continue;

                target = pair;
                nearestDist = dist;
            }

            if (target.HasValue)
            {
                SetJob(target.Value.Key, () => ClearDebrisAt(farm, target.Value.Key, target.Value.Value));
                return true;
            }
            return false;
        }

        private bool TryChopTree(Farm farm, Vector2 petTile)
        {
            if (!ModEntry.Config.ChopTrees) return false;

            foreach (var pair in farm.terrainFeatures.Pairs)
            {
                if (pair.Value is not Tree tree) continue;
                if (tree.growthStage.Value < 5 || tree.stump.Value) continue;

                float dist = Vector2.Distance(petTile, pair.Key);
                if (dist > ModEntry.Config.WorkRadius) continue;

                SetJob(pair.Key, () => ChopTreeAt(farm, pair.Key, tree));
                return true;
            }
            return false;
        }

        private bool TryRemoveStump(Farm farm, Vector2 petTile)
        {
            if (!ModEntry.Config.ClearStumpsAndLogs) return false;

            // Normal stumps
            foreach (var pair in farm.terrainFeatures.Pairs)
            {
                if (pair.Value is not Tree tree) continue;
                if (!tree.stump.Value) continue;

                float dist = Vector2.Distance(petTile, pair.Key);
                if (dist > ModEntry.Config.WorkRadius) continue;

                SetJob(pair.Key, () => RemoveStumpAt(farm, pair.Key, tree));
                return true;
            }

            // Large Stumps/Logs (ResourceClumps)
            foreach (var clump in farm.resourceClumps)
            {
                if (clump.parentSheetIndex.Value != 600 && clump.parentSheetIndex.Value != 602)
                    continue;

                float dist = Vector2.Distance(petTile, clump.Tile);
                if (dist > ModEntry.Config.WorkRadius)
                    continue;

                ResourceClump targetClump = clump;
                SetJob(clump.Tile, () => RemoveLargeStump(farm, targetClump));
                return true;
            }

            return false;
        }

        private bool TryBreakBoulder(Farm farm, Vector2 petTile)
        {
            if (!ModEntry.Config.BreakBoulders) return false;

            foreach (var clump in farm.resourceClumps)
            {
                if (!IsBoulderClump(clump.parentSheetIndex.Value))
                    continue;

                float dist = Vector2.Distance(petTile, clump.Tile);
                if (dist > ModEntry.Config.WorkRadius)
                    continue;

                ResourceClump targetClump = clump;
                SetJob(clump.Tile, () => BreakBoulder(farm, targetClump));
                return true;
            }

            return false;
        }

        private void SetJob(Vector2 tile, Action action)
        {
            // Skip if we already know this tile is unreachable
            if (_unreachableTiles.Contains(tile))
            {
                return; // Silently skip, don't spam logs
            }

            ModEntry.Instance.Monitor.Log($"[WorkingPets] SetJob: Moving to tile {tile}", LogLevel.Debug);
            
            _targetTile = tile;
            _pendingAction = action;

            _lastTargetTile = tile;
            _lastDistanceToTarget = float.MaxValue;
            _noProgressTicks = 0;
        }

        private bool TryWarpPetNearTarget(GameLocation location, Vector2 targetTile)
        {
            if (_pet == null)
            {
                return false;
            }

            // Try the 8 neighboring tiles around the target (1 tile away).
            // Prefer tiles that are on-map, passable, and not water.
            var candidates = new List<Vector2>(8);
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (dx == 0 && dy == 0)
                        continue;
                    candidates.Add(targetTile + new Vector2(dx, dy));
                }
            }

            // Prefer closer-to-pet tiles.
            candidates = candidates
                .Where(t => location.isTileOnMap(t))
                .OrderBy(t => Vector2.Distance(_pet.Tile, t))
                .ToList();

            foreach (var tile in candidates)
            {
                int x = (int)tile.X;
                int y = (int)tile.Y;

                if (location.isWaterTile(x, y))
                    continue;

                if (!location.isTilePassable(new xTile.Dimensions.Location(x, y), Game1.viewport))
                    continue;

                ModEntry.Instance.Monitor.Log($"[WorkingPets] Stuck detected; warping {_pet.Name} near target {targetTile} -> {tile}", LogLevel.Debug);
                Game1.warpCharacter(_pet, location.NameOrUniqueName, tile);
                _pet.Halt();
                _pet.controller = null;
                return true;
            }

            return false;
        }

        private void ClearDebrisAt(Farm farm, Vector2 tile, StardewValley.Object obj)
        {
            ModEntry.Instance.Monitor.Log($"[WorkingPets] ClearDebrisAt called: tile={tile}, object={obj.Name}", LogLevel.Info);
            
            if (!farm.objects.ContainsKey(tile))
            {
                ModEntry.Instance.Monitor.Log($"[WorkingPets] Object no longer exists at {tile}!", LogLevel.Warn);
                return;
            }

            // Play appropriate sound and create destruction animation
            if (obj.Name.Contains("Stone"))
            {
                // Stone destruction - hammer sound + stone debris
                farm.localSound("hammer");
                Game1.createRadialDebris(farm, 14, (int)tile.X, (int)tile.Y, Game1.random.Next(2, 5), resource: false);
                // Stone break animation
                farm.temporarySprites.Add(new TemporaryAnimatedSprite(47, tile * 64f, Color.Gray, 8, Game1.random.NextDouble() < 0.5, 50f));
            }
            else if (obj.Name.Contains("Twig") || obj.ParentSheetIndex == 294 || obj.ParentSheetIndex == 295)
            {
                // Twig destruction - axe chop + wood debris animation
                farm.localSound("axchop");
                Game1.createRadialDebris(farm, 12, (int)tile.X, (int)tile.Y, Game1.random.Next(4, 10), resource: false);
                // Wood break animation (sprite 12 = wood poof)
                farm.temporarySprites.Add(new TemporaryAnimatedSprite(12, tile * 64f, Color.White, 8, Game1.random.NextDouble() < 0.5, 50f));
            }
            else if (obj.Name.Contains("Weed"))
            {
                // Weed cutting - cut sound + grass/weed animation
                farm.localSound("weed_cut");
                // Weed destruction animation (sprite 50 = green poof for weeds)
                Color weedColor = Color.Green;
                farm.temporarySprites.Add(new TemporaryAnimatedSprite(50, tile * 64f, weedColor));
                farm.temporarySprites.Add(new TemporaryAnimatedSprite(50, tile * 64f + new Vector2(Game1.random.Next(-16, 16), Game1.random.Next(-48, 48)), weedColor * 0.75f)
                {
                    scale = 0.75f,
                    flipped = true
                });
                farm.temporarySprites.Add(new TemporaryAnimatedSprite(50, tile * 64f + new Vector2(Game1.random.Next(-16, 16), Game1.random.Next(-48, 48)), weedColor * 0.75f)
                {
                    scale = 0.75f,
                    delayBeforeAnimationStart = 50
                });
            }
            else
            {
                // Generic debris - cut sound
                farm.localSound("cut");
                Game1.createRadialDebris(farm, 12, (int)tile.X, (int)tile.Y, Game1.random.Next(2, 5), resource: false);
            }

            var drops = GetDropsForObject(obj);
            farm.objects.Remove(tile);
            
            ModEntry.Instance.Monitor.Log($"[WorkingPets] Removed {obj.Name} at {tile}, got {drops.Count} drops", LogLevel.Info);

            foreach (var item in drops) ModEntry.InventoryManager.AddItem(item);
        }

        private void ChopTreeAt(Farm farm, Vector2 tile, Tree tree)
        {
            if (!farm.terrainFeatures.ContainsKey(tile)) return;

            if (!_objectDamage.ContainsKey(tile)) _objectDamage[tile] = 0;
            _objectDamage[tile]++;

            // Play axe chop sound and create wood chip debris
            farm.localSound("axchop");
            
            // Create wood chip debris flying from the chop point
            Vector2 debrisOrigin = new Vector2(tile.X * 64f + 32f, tile.Y * 64f);
            farm.debris.Add(new Debris(12, Game1.random.Next(1, 3), debrisOrigin, debrisOrigin, 0));
            
            // Shake the tree
            tree.shake(tile, doEvenIfStillShaking: true);

            if (_objectDamage[tile] >= TREE_HEALTH)
            {
                // IMPORTANT: Don't use the falling-tree animation here.
                // It causes weird sequencing (stump/top flicker) when we're not letting the game handle drops.
                // Instead: play a crack + burst, and instantly convert the tree into a stump.
                // Visual burst (looks like the chop finished)
                Game1.createRadialDebris(farm, 12, (int)tile.X, (int)tile.Y, Game1.random.Next(12, 18), resource: false);
                farm.temporarySprites.Add(new TemporaryAnimatedSprite(12, tile * 64f, Color.White, 8, Game1.random.NextDouble() < 0.5, 50f));

                // Force a clean non-falling state and leave a stump behind
                tree.falling.Value = false;
                tree.shakeRotation = 0f;
                tree.maxShake = 0f;
                tree.stump.Value = true;
                tree.health.Value = 5f;

                // Reset our hit tracking for the stump phase
                _objectDamage.Remove(tile);

                // Add drops to pet inventory
                ModEntry.InventoryManager.AddItem(ItemRegistry.Create("(O)388", _random.Next(8, 15)));
                if (_random.NextDouble() < 0.5)
                    ModEntry.InventoryManager.AddItem(ItemRegistry.Create("(O)92", _random.Next(1, 3)));
            }
        }

        private void RemoveStumpAt(Farm farm, Vector2 tile, Tree tree)
        {
            if (!farm.terrainFeatures.ContainsKey(tile)) return;

            if (!_objectDamage.ContainsKey(tile)) _objectDamage[tile] = 0;
            _objectDamage[tile]++;

            // Play axe chop sound and create wood debris
            farm.localSound("axchop");
            Vector2 debrisOrigin = new Vector2(tile.X * 64f + 32f, tile.Y * 64f);
            farm.debris.Add(new Debris(12, Game1.random.Next(1, 3), debrisOrigin, debrisOrigin, 0));

            if (_objectDamage[tile] >= STUMP_HEALTH)
            {
                // Final destruction - use the same VFX as breaking twigs/wood (universal wood break look)
                farm.localSound("stumpCrack");

                // Wood chips burst
                Game1.createRadialDebris(farm, 12, (int)tile.X, (int)tile.Y, Game1.random.Next(10, 16), resource: false);

                // Wood poof (same as twig break)
                farm.temporarySprites.Add(new TemporaryAnimatedSprite(12, tile * 64f, Color.White, 8, Game1.random.NextDouble() < 0.5, 50f));
                
                farm.terrainFeatures.Remove(tile);
                _objectDamage.Remove(tile);

                ModEntry.InventoryManager.AddItem(ItemRegistry.Create("(O)388", _random.Next(3, 6)));
            }
        }

        private void RemoveLargeStump(Farm farm, ResourceClump clump)
        {
            Vector2 tile = clump.Tile;
            int index = farm.resourceClumps.IndexOf(clump);
            if (index == -1) return;

            if (!_objectDamage.ContainsKey(tile)) _objectDamage[tile] = 0;
            _objectDamage[tile]++;

            // Play axe chop sound and create wood debris on each hit
            farm.localSound("axchop");
            Vector2 debrisOrigin = new Vector2(tile.X * 64f + 32f, tile.Y * 64f + 32f);
            farm.debris.Add(new Debris(12, Game1.random.Next(1, 3), debrisOrigin, debrisOrigin, 0));

            if (_objectDamage[tile] >= STUMP_HEALTH * 2)
            {
                // Final destruction - use the same VFX as breaking twigs/wood (universal wood break look)
                farm.localSound("stumpCrack");

                // Wood chips burst across the clump footprint
                int bursts = 3;
                for (int i = 0; i < bursts; i++)
                {
                    int x = (int)tile.X + Game1.random.Next(clump.width.Value);
                    int y = (int)tile.Y + Game1.random.Next(clump.height.Value);
                    Game1.createRadialDebris(farm, 12, x, y, Game1.random.Next(8, 14), resource: false);
                }

                // A couple wood poofs to sell the breakup (covers 2x2-ish area)
                farm.temporarySprites.Add(new TemporaryAnimatedSprite(12, tile * 64f, Color.White, 8, Game1.random.NextDouble() < 0.5, 50f));
                farm.temporarySprites.Add(new TemporaryAnimatedSprite(12, (tile + new Vector2(1f, 0f)) * 64f, Color.White, 8, Game1.random.NextDouble() < 0.5, 60f));
                farm.temporarySprites.Add(new TemporaryAnimatedSprite(12, (tile + new Vector2(0f, 1f)) * 64f, Color.White, 8, Game1.random.NextDouble() < 0.5, 70f));
                
                farm.resourceClumps.RemoveAt(index);
                _objectDamage.Remove(tile);

                // Hardwood drops (game gives 2-8 for stump, 8-10 for log)
                int hardwoodAmount = (clump.parentSheetIndex.Value == 602) ? _random.Next(6, 10) : _random.Next(2, 4);
                ModEntry.InventoryManager.AddItem(ItemRegistry.Create("(O)709", hardwoodAmount));
            }
        }

        private void BreakBoulder(Farm farm, ResourceClump clump)
        {
            Vector2 tile = clump.Tile;
            int index = farm.resourceClumps.IndexOf(clump);
            if (index == -1) return;

            if (!_objectDamage.ContainsKey(tile)) _objectDamage[tile] = 0;
            _objectDamage[tile]++;

            // Play hammer sound and create stone debris on each hit
            farm.localSound("hammer");
            
            // Stone chip debris flying from impact (14 = stoneDebris)
            Game1.createRadialDebris(farm, 14, (int)tile.X + Game1.random.Next(clump.width.Value / 2 + 1), 
                (int)tile.Y + Game1.random.Next(clump.height.Value / 2 + 1), Game1.random.Next(4, 9), resource: false);

            if (_objectDamage[tile] >= BOULDER_HEALTH)
            {
                // Final destruction - use the normal stone break VFX, just bigger/more of it
                farm.localSound("hammer");

                // Big burst of regular stone debris across the clump footprint (14 = stoneDebris)
                int bursts = 4;
                for (int i = 0; i < bursts; i++)
                {
                    int x = (int)tile.X + Game1.random.Next(clump.width.Value);
                    int y = (int)tile.Y + Game1.random.Next(clump.height.Value);
                    Game1.createRadialDebris(farm, 14, x, y, Game1.random.Next(6, 12), resource: false);
                }

                // Extra stone break sprites (47) to sell the impact
                for (int i = 0; i < 3; i++)
                {
                    Vector2 pos = (tile + new Vector2(Game1.random.Next(clump.width.Value), Game1.random.Next(clump.height.Value))) * 64f;
                    farm.temporarySprites.Add(new TemporaryAnimatedSprite(47, pos, Color.Gray, 8, Game1.random.NextDouble() < 0.5, 50f)
                    {
                        scale = 1.25f
                    });
                }
                
                farm.resourceClumps.RemoveAt(index);
                _objectDamage.Remove(tile);

                // Drop stone - game gives 15 for regular boulder, 10 for mine boulders
                int stoneAmount = (clump.parentSheetIndex.Value == 672) ? _random.Next(12, 18) : _random.Next(8, 12);
                ModEntry.InventoryManager.AddItem(ItemRegistry.Create("(O)390", stoneAmount));
                
                // Chance for ores
                if (_random.NextDouble() < 0.25)
                    ModEntry.InventoryManager.AddItem(ItemRegistry.Create("(O)380", _random.Next(1, 3))); // Copper
                if (_random.NextDouble() < 0.1)
                    ModEntry.InventoryManager.AddItem(ItemRegistry.Create("(O)384", _random.Next(1, 2))); // Gold
            }
        }

        private List<Item> GetDropsForObject(StardewValley.Object obj)
        {
            var drops = new List<Item>();
            if (obj.Name.Contains("Stone"))
            {
                drops.Add(ItemRegistry.Create("(O)390", 1));
                if (_random.NextDouble() < 0.05) drops.Add(ItemRegistry.Create("(O)382", 1));
            }
            else if (obj.Name.Contains("Twig") || obj.ParentSheetIndex == 294 || obj.ParentSheetIndex == 295)
            {
                drops.Add(ItemRegistry.Create("(O)388", 1));
            }
            else if (obj.Name.Contains("Weed"))
            {
                if (_random.NextDouble() < 0.5) drops.Add(ItemRegistry.Create("(O)771", 1));
                if (_random.NextDouble() < 0.05) drops.Add(ItemRegistry.Create("(O)770", 1));
            }
            return drops;
        }
    }
}
