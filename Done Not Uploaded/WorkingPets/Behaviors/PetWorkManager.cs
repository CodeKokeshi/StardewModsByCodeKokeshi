using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Characters;
using StardewValley.Pathfinding;

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
        private int _tickCounter;
        
        // Movement & Action State
        private Vector2? _targetTile;
        private Action? _pendingAction;
        private bool _isMovingToTarget;

        private readonly Random _random = new();

        // Track damage for multi-hit objects
        private readonly Dictionary<Vector2, int> _objectDamage = new();

        /*********
        ** Properties
        *********/
        public bool IsWorking => _isWorking;
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
                ModEntry.Instance.Monitor.Log($"{_pet?.Name} started working!", LogLevel.Info);
            }
            else
            {
                ModEntry.Instance.Monitor.Log($"{_pet?.Name} stopped working.", LogLevel.Info);
                StopWorkImmediately();
            }
        }

        public void Update()
        {
            if (!_isWorking || _pet == null)
                return;

            Farm? farm = Game1.getFarm();
            if (farm == null) return;

            // Ensure pet is on farm
            if (_pet.currentLocation?.Name != "Farm")
            {
                Game1.warpCharacter(_pet, "Farm", new Vector2(64, 15));
                return;
            }

            // If we have a target, move towards it
            if (_targetTile.HasValue)
            {
                MoveTowardTarget();
                return;
            }

            // Otherwise, scan for work periodically
            _tickCounter++;
            if (_tickCounter < ModEntry.Config.TicksBetweenActions)
                return;
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
            _isMovingToTarget = false;
            _targetTile = null;
            _pendingAction = null;
        }

        private void MoveTowardTarget()
        {
            if (_pet == null || !_targetTile.HasValue) return;

            // Check if we've arrived (close enough to target)
            float distance = Vector2.Distance(_pet.Tile, _targetTile.Value);
            if (distance < 2f)
            {
                // Arrived! Execute the action
                _pet.Halt();
                _pet.controller = null;
                _pendingAction?.Invoke();
                _targetTile = null;
                _pendingAction = null;
                _isMovingToTarget = false;
                return;
            }

            // If we have a pathfinding controller, update it manually (pets don't auto-update like NPCs)
            if (_pet.controller != null)
            {
                // Call the controller's update to actually move the pet
                bool finished = _pet.controller.update(Game1.currentGameTime);
                
                if (finished)
                {
                    // Pathfinding completed (arrived or gave up)
                    _pet.controller = null;
                    
                    // Check if we're close enough to do the action
                    distance = Vector2.Distance(_pet.Tile, _targetTile.Value);
                    if (distance < 3f)
                    {
                        _pendingAction?.Invoke();
                    }
                    
                    _targetTile = null;
                    _pendingAction = null;
                    _isMovingToTarget = false;
                }
                return;
            }

            // No controller but we should be moving - pathfinding failed, skip target
            if (_isMovingToTarget)
            {
                ModEntry.Instance.Monitor.Log($"Pathfinding failed to reach target at {_targetTile.Value}, skipping.", LogLevel.Trace);
                _targetTile = null;
                _pendingAction = null;
                _isMovingToTarget = false;
            }
        }

        private void StartPathfindingToTarget()
        {
            if (_pet == null || !_targetTile.HasValue || _pet.currentLocation == null) return;

            // Find a walkable tile adjacent to the target
            Vector2 destination = FindWalkableTileNear(_targetTile.Value);
            
            _pet.controller = new PathFindController(
                _pet,
                _pet.currentLocation,
                new Point((int)destination.X, (int)destination.Y),
                _pet.FacingDirection,
                OnPathfindingComplete
            );

            _isMovingToTarget = true;
        }

        private Vector2 FindWalkableTileNear(Vector2 target)
        {
            if (_pet?.currentLocation == null) return target;

            var location = _pet.currentLocation;
            
            // Check adjacent tiles for a walkable one
            Vector2[] offsets = { Vector2.Zero, new(-1, 0), new(1, 0), new(0, -1), new(0, 1), new(-1, -1), new(1, -1), new(-1, 1), new(1, 1) };
            
            foreach (var offset in offsets)
            {
                Vector2 checkTile = target + offset;
                if (IsTileWalkable(location, checkTile))
                    return checkTile;
            }

            return target; // Fallback
        }

        private bool IsTileWalkable(GameLocation location, Vector2 tile)
        {
            // Check if tile is passable for the pet
            return location.isTileOnMap(tile) &&
                   !location.isWaterTile((int)tile.X, (int)tile.Y) &&
                   location.isTilePassable(new xTile.Dimensions.Location((int)tile.X, (int)tile.Y), Game1.viewport);
        }

        private void OnPathfindingComplete(Character c, GameLocation location)
        {
            // Pathfinding finished - the action will be executed in MoveTowardTarget
        }

        private void ScanForWork(Farm farm)
        {
            Vector2 petTile = _pet!.Tile;
            var config = ModEntry.Config;

            if (config.IgnorePriority)
            {
                // Find the nearest target regardless of type
                ScanForNearestTarget(farm, petTile);
            }
            else
            {
                // Scan by priority order
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
            // Check if target is reachable (not across water)
            if (_pet?.currentLocation != null && !CanReachTarget(tile))
            {
                ModEntry.Instance.Monitor.Log($"Target at {tile} is unreachable, skipping.", LogLevel.Trace);
                return;
            }

            _targetTile = tile;
            _pendingAction = action;
            _isMovingToTarget = false;
            
            // Start pathfinding immediately
            StartPathfindingToTarget();
        }

        private bool CanReachTarget(Vector2 target)
        {
            if (_pet?.currentLocation == null) return false;

            // Quick check: if a walkable tile exists near the target, we might be able to reach it
            Vector2 destination = FindWalkableTileNear(target);
            
            // Try to create a path - if it fails, the target is unreachable
            var testController = new PathFindController(
                _pet,
                _pet.currentLocation,
                new Point((int)destination.X, (int)destination.Y),
                _pet.FacingDirection
            );

            return testController.pathToEndPoint != null && testController.pathToEndPoint.Count > 0;
        }

        private void ClearDebrisAt(Farm farm, Vector2 tile, StardewValley.Object obj)
        {
            if (!farm.objects.ContainsKey(tile)) return;

            if (obj.Name.Contains("Stone")) farm.localSound("hammer");
            else farm.localSound("cut");

            var drops = GetDropsForObject(obj);
            farm.objects.Remove(tile);

            foreach (var item in drops) ModEntry.InventoryManager.AddItem(item);
        }

        private void ChopTreeAt(Farm farm, Vector2 tile, Tree tree)
        {
            if (!farm.terrainFeatures.ContainsKey(tile)) return;

            if (!_objectDamage.ContainsKey(tile)) _objectDamage[tile] = 0;
            _objectDamage[tile]++;

            farm.localSound("axchop");
            tree.shake(tile, doEvenIfStillShaking: true);

            if (_objectDamage[tile] >= TREE_HEALTH)
            {
                farm.terrainFeatures.Remove(tile);
                _objectDamage.Remove(tile);
                farm.localSound("treecrack");

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

            farm.localSound("axchop");

            if (_objectDamage[tile] >= STUMP_HEALTH)
            {
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

            farm.localSound("axchop");

            if (_objectDamage[tile] >= STUMP_HEALTH * 2)
            {
                farm.resourceClumps.RemoveAt(index);
                _objectDamage.Remove(tile);

                ModEntry.InventoryManager.AddItem(ItemRegistry.Create("(O)709", _random.Next(2, 5))); // Hardwood
            }
        }

        private void BreakBoulder(Farm farm, ResourceClump clump)
        {
            Vector2 tile = clump.Tile;
            int index = farm.resourceClumps.IndexOf(clump);
            if (index == -1) return;

            if (!_objectDamage.ContainsKey(tile)) _objectDamage[tile] = 0;
            _objectDamage[tile]++;

            farm.localSound("hammer");

            if (_objectDamage[tile] >= BOULDER_HEALTH)
            {
                farm.resourceClumps.RemoveAt(index);
                _objectDamage.Remove(tile);
                farm.localSound("boulderBreak");

                // Drop stone and chance for ores
                ModEntry.InventoryManager.AddItem(ItemRegistry.Create("(O)390", _random.Next(10, 20))); // Stone
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
