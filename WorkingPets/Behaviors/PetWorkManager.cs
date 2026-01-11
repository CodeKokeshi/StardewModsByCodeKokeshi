using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Characters;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;

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
        private Vector2 _currentTarget = Vector2.Zero;
        private readonly Random _random = new();

        // Track tree damage (trees need multiple hits)
        private readonly Dictionary<Vector2, int> _treeDamage = new();
        private const int TREE_HEALTH = 10; // Hits needed to fell a tree
        private const int STUMP_HEALTH = 5; // Hits needed to remove stump

        /*********
        ** Properties
        *********/
        /// <summary>Whether the pet is currently working.</summary>
        public bool IsWorking => _isWorking;

        /// <summary>The pet being managed.</summary>
        public Pet? Pet => _pet;

        /*********
        ** Public methods
        *********/
        /// <summary>Initialize the work manager with a pet.</summary>
        public void Initialize(Pet pet)
        {
            _pet = pet;
            LoadState(pet);
        }

        /// <summary>Toggle work mode on/off.</summary>
        public void ToggleWork()
        {
            _isWorking = !_isWorking;
            _tickCounter = 0;
            _currentTarget = Vector2.Zero;

            if (_isWorking)
            {
                ModEntry.Instance.Monitor.Log($"{_pet?.Name} started working!", LogLevel.Info);
            }
            else
            {
                ModEntry.Instance.Monitor.Log($"{_pet?.Name} stopped working.", LogLevel.Info);
            }
        }

        /// <summary>Update work logic (called every tick).</summary>
        public void Update()
        {
            if (!_isWorking || _pet == null)
                return;

            // Always halt pet's default behavior when working
            _pet.Halt();
            _pet.controller = null;

            // Only act at configured intervals
            _tickCounter++;
            if (_tickCounter < ModEntry.Config.TicksBetweenActions)
                return;
            _tickCounter = 0;

            // Make sure pet is on farm
            Farm? farm = Game1.getFarm();
            if (farm == null)
                return;

            // Warp pet to farm if needed
            if (_pet.currentLocation?.Name != "Farm")
            {
                Game1.warpCharacter(_pet, "Farm", new Vector2(54, 8));
                return;
            }

            // Find and execute work
            ExecuteWork(farm);
        }

        /// <summary>Save work state to pet's modData.</summary>
        public void SaveState(Pet pet)
        {
            pet.modData["WorkingPets.IsWorking"] = _isWorking.ToString();
        }

        /// <summary>Load work state from pet's modData.</summary>
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
        private void ExecuteWork(Farm farm)
        {
            Vector2 petTile = _pet!.Tile;

            // Priority 1: Clear debris (weeds, stones, sticks)
            if (TryClearDebris(farm, petTile))
                return;

            // Priority 2: Chop trees
            if (TryChopTree(farm, petTile))
                return;

            // Priority 3: Remove stumps
            if (TryRemoveStump(farm, petTile))
                return;

            // Nothing to do - pet can idle
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

                // Check distance
                float dist = Vector2.Distance(petTile, tile);
                if (dist > config.WorkRadius || dist >= nearestDist)
                    continue;

                // Check if it's clearable debris
                bool isWeed = config.ClearWeeds && obj.Name.Contains("Weed");
                bool isStone = config.ClearStones && obj.Name.Contains("Stone");
                bool isStick = config.ClearSticks && (obj.Name.Contains("Twig") || obj.ParentSheetIndex == 294 || obj.ParentSheetIndex == 295);

                // Also check for generic debris
                if (!isWeed && !isStone && !isStick)
                {
                    // Check category for litter
                    if (obj.Category == StardewValley.Object.litterCategory)
                    {
                        isWeed = config.ClearWeeds;
                    }
                }

                if (isWeed || isStone || isStick)
                {
                    target = pair;
                    nearestDist = dist;
                }
            }

            if (target.HasValue)
            {
                ClearDebrisAt(farm, target.Value.Key, target.Value.Value);
                return true;
            }

            return false;
        }

        private void ClearDebrisAt(Farm farm, Vector2 tile, StardewValley.Object obj)
        {
            try
            {
                // Stop pet movement temporarily and teleport near target
                if (_pet != null)
                {
                    _pet.Halt();
                    _pet.controller = null; // Clear any pathfinding
                    
                    // Teleport pet near the debris (1 tile away)
                    Vector2 petPos = tile * 64f;
                    _pet.Position = petPos + new Vector2(64, 0); // 1 tile to the right
                    _pet.faceDirection(3); // Face left toward the debris
                }

                // Play appropriate sound
                if (obj.Name.Contains("Stone"))
                {
                    farm.localSound("hammer");
                }
                else
                {
                    farm.localSound("cut");
                }

                // Get drops BEFORE removing
                var drops = GetDropsForObject(obj);

                // Directly remove the object from the farm (no performToolAction)
                farm.objects.Remove(tile);

                // Add drops to pet inventory
                foreach (var item in drops)
                {
                    ModEntry.InventoryManager.AddItem(item);
                }

                // Show message
                if (ModEntry.Config.ShowWorkingMessages)
                {
                    ShowWorkMessage($"Cleared {obj.Name}!");
                }

                ModEntry.Instance.Monitor.Log($"Pet cleared {obj.Name} at {tile}", LogLevel.Debug);
            }
            catch (Exception ex)
            {
                ModEntry.Instance.Monitor.Log($"Error clearing debris: {ex.Message}", LogLevel.Error);
            }
        }

        private bool TryChopTree(Farm farm, Vector2 petTile)
        {
            if (!ModEntry.Config.ChopTrees)
                return false;

            foreach (var pair in farm.terrainFeatures.Pairs)
            {
                if (pair.Value is not Tree tree)
                    continue;

                // Only fully grown trees (growth stage 5)
                if (tree.growthStage.Value < 5 || tree.stump.Value)
                    continue;

                float dist = Vector2.Distance(petTile, pair.Key);
                if (dist > ModEntry.Config.WorkRadius)
                    continue;

                ChopTreeAt(farm, pair.Key, tree);
                return true;
            }

            return false;
        }

        private void ChopTreeAt(Farm farm, Vector2 tile, Tree tree)
        {
            try
            {
                // Stop pet and teleport near tree
                if (_pet != null)
                {
                    _pet.Halt();
                    _pet.controller = null;
                    _pet.Position = tile * 64f + new Vector2(64, 0);
                    _pet.faceDirection(3);
                }

                // Track damage
                if (!_treeDamage.ContainsKey(tile))
                    _treeDamage[tile] = 0;

                _treeDamage[tile]++;

                // Play sound
                farm.localSound("axchop");

                // Create shake effect on tree
                tree.shake(tile, doEvenIfStillShaking: true);

                if (_treeDamage[tile] >= TREE_HEALTH)
                {
                    // Tree falls! Remove it directly
                    farm.terrainFeatures.Remove(tile);
                    _treeDamage.Remove(tile);

                    // Play fall sound
                    farm.localSound("treecrack");

                    // Add wood to inventory
                    var wood = ItemRegistry.Create("(O)388", _random.Next(8, 15)); // 8-14 wood
                    ModEntry.InventoryManager.AddItem(wood);

                    // Sometimes get sap
                    if (_random.NextDouble() < 0.5)
                    {
                        var sap = ItemRegistry.Create("(O)92", _random.Next(1, 3));
                        ModEntry.InventoryManager.AddItem(sap);
                    }

                    // Sometimes get seeds
                    if (_random.NextDouble() < 0.25)
                    {
                        var seeds = ItemRegistry.Create($"(O){tree.treeType.Value}", _random.Next(1, 3));
                        ModEntry.InventoryManager.AddItem(seeds);
                    }

                    if (ModEntry.Config.ShowWorkingMessages)
                    {
                        ShowWorkMessage("Chopped down a tree!");
                    }

                    ModEntry.Instance.Monitor.Log($"Pet chopped tree at {tile}", LogLevel.Debug);
                }
            }
            catch (Exception ex)
            {
                ModEntry.Instance.Monitor.Log($"Error chopping tree: {ex.Message}", LogLevel.Error);
            }
        }

        private bool TryRemoveStump(Farm farm, Vector2 petTile)
        {
            if (!ModEntry.Config.ChopStumps)
                return false;

            foreach (var pair in farm.terrainFeatures.Pairs)
            {
                if (pair.Value is not Tree tree || !tree.stump.Value)
                    continue;

                float dist = Vector2.Distance(petTile, pair.Key);
                if (dist > ModEntry.Config.WorkRadius)
                    continue;

                RemoveStumpAt(farm, pair.Key, tree);
                return true;
            }

            // Also check resource clumps for large stumps
            for (int i = farm.resourceClumps.Count - 1; i >= 0; i--)
            {
                var clump = farm.resourceClumps[i];
                // 600 = large stump, 602 = hollow log
                if (clump.parentSheetIndex.Value != 600 && clump.parentSheetIndex.Value != 602)
                    continue;

                float dist = Vector2.Distance(petTile, clump.Tile);
                if (dist > ModEntry.Config.WorkRadius)
                    continue;

                RemoveLargeStump(farm, clump, i);
                return true;
            }

            return false;
        }

        private void RemoveStumpAt(Farm farm, Vector2 tile, Tree tree)
        {
            try
            {
                // Stop pet and teleport near stump
                if (_pet != null)
                {
                    _pet.Halt();
                    _pet.controller = null;
                    _pet.Position = tile * 64f + new Vector2(64, 0);
                    _pet.faceDirection(3);
                }

                if (!_treeDamage.ContainsKey(tile))
                    _treeDamage[tile] = 0;

                _treeDamage[tile]++;

                farm.localSound("axchop");

                if (_treeDamage[tile] >= STUMP_HEALTH)
                {
                    // Remove stump directly
                    farm.terrainFeatures.Remove(tile);
                    _treeDamage.Remove(tile);

                    var wood = ItemRegistry.Create("(O)388", _random.Next(3, 6));
                    ModEntry.InventoryManager.AddItem(wood);

                    if (ModEntry.Config.ShowWorkingMessages)
                    {
                        ShowWorkMessage("Removed a stump!");
                    }

                    ModEntry.Instance.Monitor.Log($"Pet removed stump at {tile}", LogLevel.Debug);
                }
            }
            catch (Exception ex)
            {
                ModEntry.Instance.Monitor.Log($"Error removing stump: {ex.Message}", LogLevel.Error);
            }
        }

        private void RemoveLargeStump(Farm farm, ResourceClump clump, int index)
        {
            try
            {
                Vector2 tile = clump.Tile;

                // Stop pet and teleport near large stump
                if (_pet != null)
                {
                    _pet.Halt();
                    _pet.controller = null;
                    _pet.Position = tile * 64f + new Vector2(128, 0); // 2 tiles away (large stumps are 2x2)
                    _pet.faceDirection(3);
                }

                if (!_treeDamage.ContainsKey(tile))
                    _treeDamage[tile] = 0;

                _treeDamage[tile]++;

                farm.localSound("axchop");

                if (_treeDamage[tile] >= STUMP_HEALTH * 2) // Large stumps need more hits
                {
                    // Remove large stump directly
                    farm.resourceClumps.RemoveAt(index);
                    _treeDamage.Remove(tile);

                    var hardwood = ItemRegistry.Create("(O)709", _random.Next(2, 5));
                    ModEntry.InventoryManager.AddItem(hardwood);

                    if (ModEntry.Config.ShowWorkingMessages)
                    {
                        ShowWorkMessage("Removed a large stump!");
                    }

                    ModEntry.Instance.Monitor.Log($"Pet removed large stump at {tile}", LogLevel.Debug);
                }
            }
            catch (Exception ex)
            {
                ModEntry.Instance.Monitor.Log($"Error removing large stump: {ex.Message}", LogLevel.Error);
            }
        }

        private void MovePetToward(Vector2 targetTile)
        {
            if (_pet == null)
                return;

            _currentTarget = targetTile;

            // Simple movement - just update position gradually
            Vector2 petPos = _pet.Position;
            Vector2 targetPos = targetTile * 64f; // Convert to pixels
            Vector2 direction = targetPos - petPos;

            if (direction.Length() > 64f) // More than 1 tile away
            {
                direction.Normalize();
                _pet.Position = petPos + direction * 4f; // Move 4 pixels per tick

                // Face the direction
                if (Math.Abs(direction.X) > Math.Abs(direction.Y))
                {
                    _pet.FacingDirection = direction.X > 0 ? 1 : 3; // Right or Left
                }
                else
                {
                    _pet.FacingDirection = direction.Y > 0 ? 2 : 0; // Down or Up
                }
            }
        }

        private List<Item> GetDropsForObject(StardewValley.Object obj)
        {
            var drops = new List<Item>();

            // Determine drops based on object type
            if (obj.Name.Contains("Stone"))
            {
                drops.Add(ItemRegistry.Create("(O)390", _random.Next(1, 3))); // Stone
                if (_random.NextDouble() < 0.05)
                    drops.Add(ItemRegistry.Create("(O)382", 1)); // Coal
            }
            else if (obj.Name.Contains("Twig") || obj.ParentSheetIndex == 294 || obj.ParentSheetIndex == 295)
            {
                drops.Add(ItemRegistry.Create("(O)388", _random.Next(1, 3))); // Wood
            }
            else if (obj.Name.Contains("Weed"))
            {
                // Weeds can drop fiber, mixed seeds, or nothing
                if (_random.NextDouble() < 0.5)
                    drops.Add(ItemRegistry.Create("(O)771", _random.Next(1, 3))); // Fiber
                if (_random.NextDouble() < 0.05)
                    drops.Add(ItemRegistry.Create("(O)770", 1)); // Mixed Seeds
            }

            return drops;
        }

        private void ShowWorkMessage(string message)
        {
            if (_pet != null)
            {
                Game1.addHUDMessage(new HUDMessage($"{_pet.Name}: {message}", HUDMessage.achievement_type));
            }
        }
    }
}
