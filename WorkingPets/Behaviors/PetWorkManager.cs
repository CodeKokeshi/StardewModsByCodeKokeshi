using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Characters;

namespace WorkingPets.Behaviors
{
    /// <summary>Manages pet work behavior.</summary>
    public class PetWorkManager
    {
        /*********
        ** Fields
        *********/
        private Pet? _pet;
        private bool _isWorking;
        private int _tickCounter;
        
        // Movement & Action State
        private Vector2? _targetTile;
        private Action? _pendingAction;
        private readonly float _moveSpeed = 3f; // Pixels per tick

        private readonly Random _random = new();

        // Track tree damage (trees need multiple hits)
        private readonly Dictionary<Vector2, int> _treeDamage = new();
        private const int TREE_HEALTH = 10;
        private const int STUMP_HEALTH = 5;

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
                Game1.warpCharacter(_pet, "Farm", new Vector2(64, 15)); // Default to front of house
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
            pet.modData["WorkingPets.IsWorking"] = _isWorking.ToString();
        }

        public void LoadState(Pet pet)
        {
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

            // Reset strict control
            _pet.Halt();
            _pet.controller = null;
            
            // Resume idle behavior (Sit or Sleep)
            _pet.CurrentBehavior = Pet.behavior_Sleep; 
            _pet.Sprite.StopAnimation();
            _pet.faceDirection(2);
        }

        private void MoveTowardTarget()
        {
            if (_pet == null || !_targetTile.HasValue) return;

            // Target position in pixels (center of tile)
            // Stardew tiles are 64x64. Center is +32, +32.
            Vector2 targetPos = _targetTile.Value * 64f + new Vector2(32, 32);
            
            // Adjust to touch the edge (pets shouldn't stand INSIDE the tree usually)
            // But for debris it's fine.
            
            Vector2 currentPos = _pet.Position + new Vector2(32, 32); // Center of pet (roughly)

            float distance = Vector2.Distance(currentPos, targetPos);

            // If close enough, execute action
            // 48 pixels = 3/4 tile distance.
            if (distance < 55f) 
            {
                _pet.Halt();
                _pet.controller = null;
                
                // Execute action
                _pendingAction?.Invoke();

                // Clear target
                _targetTile = null;
                _pendingAction = null;
                return;
            }

            // Move
            Vector2 direction = targetPos - currentPos;
            direction.Normalize();

            // Apply movement
            _pet.Position += direction * _moveSpeed;

            // Face direction
            if (Math.Abs(direction.X) > Math.Abs(direction.Y))
            {
                _pet.faceDirection(direction.X > 0 ? 1 : 3);
            }
            else
            {
                _pet.faceDirection(direction.Y > 0 ? 2 : 0);
            }

            // Animate
            _pet.animateInFacingDirection(Game1.currentGameTime);
            
            // Override vanilla AI for this frame to prevent fighting
            _pet.controller = null;
        }

        private void ScanForWork(Farm farm)
        {
            Vector2 petTile = _pet!.Tile;

            if (TryClearDebris(farm, petTile)) return;
            if (TryChopTree(farm, petTile)) return;
            if (TryRemoveStump(farm, petTile)) return;
        }

        private bool TryClearDebris(Farm farm, Vector2 petTile)
        {
            var config = ModEntry.Config;
            KeyValuePair<Vector2, StardewValley.Object>? target = null;
            float nearestDist = float.MaxValue;

            foreach (var pair in farm.objects.Pairs)
            {
                var obj = pair.Value;
                var tile = pair.Key;

                float dist = Vector2.Distance(petTile, tile);
                if (dist > config.WorkRadius || dist >= nearestDist)
                    continue;

                bool isWeed = config.ClearWeeds && obj.Name.Contains("Weed");
                bool isStone = config.ClearStones && obj.Name.Contains("Stone");
                bool isStick = config.ClearSticks && (obj.Name.Contains("Twig") || obj.ParentSheetIndex == 294 || obj.ParentSheetIndex == 295);

                if (!isWeed && !isStone && !isStick)
                {
                    if (obj.Category == StardewValley.Object.litterCategory)
                        isWeed = config.ClearWeeds;
                }

                if (isWeed || isStone || isStick)
                {
                    target = pair;
                    nearestDist = dist;
                }
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

            // Search normal trees
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
            if (!ModEntry.Config.ChopStumps) return false;

            // Search normal stumps
            foreach (var pair in farm.terrainFeatures.Pairs)
            {
                if (pair.Value is not Tree tree) continue;
                if (!tree.stump.Value) continue;

                float dist = Vector2.Distance(petTile, pair.Key);
                if (dist > ModEntry.Config.WorkRadius) continue;

                SetJob(pair.Key, () => RemoveStumpAt(farm, pair.Key, tree));
                return true;
            }

            // Search Resource Clumps (Large Stumps/Logs)
            for (int i = 0; i < farm.resourceClumps.Count; i++)
            {
                var clump = farm.resourceClumps[i];
                // 600 = large stump, 602 = hollow log
                if (clump.parentSheetIndex.Value != 600 && clump.parentSheetIndex.Value != 602)
                    continue;

                float dist = Vector2.Distance(petTile, clump.Tile);
                if (dist > ModEntry.Config.WorkRadius)
                    continue;
                
                // Need to capture index safely or just use the object reference if possible. 
                // But list indexing can change if we remove. 
                // However, we only do one job at a time.
                // Better: Pass the clump itself and find its index at runtime.
                ResourceClump targetClump = clump;
                SetJob(clump.Tile, () => RemoveLargeStump(farm, targetClump));
                return true;
            }

            return false;
        }

        private void SetJob(Vector2 tile, Action action)
        {
            _targetTile = tile;
            _pendingAction = action;
        }

        private void ClearDebrisAt(Farm farm, Vector2 tile, StardewValley.Object obj)
        {
            if (!farm.objects.ContainsKey(tile)) return; // Already gone?

            if (obj.Name.Contains("Stone")) farm.localSound("hammer");
            else farm.localSound("cut");

            var drops = GetDropsForObject(obj);
            farm.objects.Remove(tile);

            foreach (var item in drops) ModEntry.InventoryManager.AddItem(item);
            
            if (ModEntry.Config.ShowWorkingMessages) 
                ShowWorkMessage($"Cleared {obj.Name}!");
        }

        private void ChopTreeAt(Farm farm, Vector2 tile, Tree tree)
        {
            if (!farm.terrainFeatures.ContainsKey(tile)) return;

            if (!_treeDamage.ContainsKey(tile)) _treeDamage[tile] = 0;
            _treeDamage[tile]++;

            farm.localSound("axchop");
            tree.shake(tile, doEvenIfStillShaking: true);

            if (_treeDamage[tile] >= TREE_HEALTH)
            {
                farm.terrainFeatures.Remove(tile);
                _treeDamage.Remove(tile);
                farm.localSound("treecrack");

                ModEntry.InventoryManager.AddItem(ItemRegistry.Create("(O)388", _random.Next(8, 15)));
                if (_random.NextDouble() < 0.5)
                    ModEntry.InventoryManager.AddItem(ItemRegistry.Create("(O)92", _random.Next(1, 3)));
                
                ShowWorkMessage("Chopped down tree!");
            }
        }

        private void RemoveStumpAt(Farm farm, Vector2 tile, Tree tree)
        {
            if (!farm.terrainFeatures.ContainsKey(tile)) return;

            if (!_treeDamage.ContainsKey(tile)) _treeDamage[tile] = 0;
            _treeDamage[tile]++;

            farm.localSound("axchop");

            if (_treeDamage[tile] >= STUMP_HEALTH)
            {
                farm.terrainFeatures.Remove(tile);
                _treeDamage.Remove(tile);

                ModEntry.InventoryManager.AddItem(ItemRegistry.Create("(O)388", _random.Next(3, 6)));
                ShowWorkMessage("Removed stump!");
            }
        }

        private void RemoveLargeStump(Farm farm, ResourceClump clump)
        {
            Vector2 tile = clump.Tile;
            // Find index again just in case
            int index = farm.resourceClumps.IndexOf(clump);
            if (index == -1) return;

            if (!_treeDamage.ContainsKey(tile)) _treeDamage[tile] = 0;
            _treeDamage[tile]++;

            farm.localSound("axchop");

            if (_treeDamage[tile] >= STUMP_HEALTH * 2)
            {
                farm.resourceClumps.RemoveAt(index);
                _treeDamage.Remove(tile);

                ModEntry.InventoryManager.AddItem(ItemRegistry.Create("(O)709", _random.Next(2, 5))); // Hardwood
                ShowWorkMessage("Removed large stump!");
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

        private void ShowWorkMessage(string message)
        {
            if (_pet != null && ModEntry.Config.ShowWorkingMessages)
            {
                // Simple HUD message with pet face (type 2) if possible, or just standard type 1
                Game1.addHUDMessage(new HUDMessage($"{_pet.Name}: {message}", 2));
            }
        }
    }
}
