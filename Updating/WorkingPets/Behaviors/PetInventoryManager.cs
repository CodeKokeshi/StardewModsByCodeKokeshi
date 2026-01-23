using System;
using System.Collections.Generic;
using System.Text.Json;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Inventories;

namespace WorkingPets.Behaviors
{
    /// <summary>Manages pet inventory storage.</summary>
    public class PetInventoryManager
    {
        /*********
        ** Fields
        *********/
        private readonly List<Item?> _inventory;
        private const string INVENTORY_KEY = "WorkingPets.Inventory";

        /*********
        ** Constructor
        *********/
        public PetInventoryManager()
        {
            // Initialize with configured size
            _inventory = new List<Item?>(ModEntry.Config.InventorySize);
            for (int i = 0; i < ModEntry.Config.InventorySize; i++)
            {
                _inventory.Add(null);
            }
        }

        /*********
        ** Properties
        *********/
        /// <summary>The pet's inventory as IInventory for menu compatibility.</summary>
        public IList<Item?> Inventory => _inventory;

        /// <summary>Number of items currently stored.</summary>
        public int ItemCount
        {
            get
            {
                int count = 0;
                foreach (var item in _inventory)
                {
                    if (item != null)
                        count++;
                }
                return count;
            }
        }

        /// <summary>Total number of individual items (counting stacks).</summary>
        public int TotalItemCount
        {
            get
            {
                int count = 0;
                foreach (var item in _inventory)
                {
                    if (item != null)
                        count += item.Stack;
                }
                return count;
            }
        }
        
        /// <summary>Check if the inventory is full (no empty slots).</summary>
        public bool IsFull
        {
            get
            {
                foreach (var item in _inventory)
                {
                    if (item == null)
                        return false;
                }
                return true;
            }
        }
        
        /// <summary>Check if inventory has space for more items.</summary>
        public bool HasSpace => !IsFull;

        /*********
        ** Public methods
        *********/
        /// <summary>Add an item to the pet's inventory.</summary>
        public bool AddItem(Item item)
        {
            if (item == null)
                return false;

            // First, try to stack with existing items
            for (int i = 0; i < _inventory.Count; i++)
            {
                if (_inventory[i] != null && _inventory[i]!.canStackWith(item))
                {
                    int remaining = _inventory[i]!.addToStack(item);
                    if (remaining <= 0)
                    {
                        ModEntry.Instance.Monitor.Log($"Added {item.Stack} {item.Name} to pet inventory (stacked)", LogLevel.Trace);
                        return true;
                    }
                    item.Stack = remaining;
                }
            }

            // Find empty slot
            for (int i = 0; i < _inventory.Count; i++)
            {
                if (_inventory[i] == null)
                {
                    _inventory[i] = item;
                    ModEntry.Instance.Monitor.Log($"Added {item.Stack} {item.Name} to pet inventory (new slot)", LogLevel.Trace);
                    return true;
                }
            }

            // Inventory full - drop on ground
            ModEntry.Instance.Monitor.Log($"Pet inventory full! Dropping {item.Name} on ground.", LogLevel.Debug);
            Pet? pet = ModEntry.GetPlayerPet();
            if (pet != null && pet.currentLocation != null)
            {
                Game1.createItemDebris(item, pet.Position, 2, pet.currentLocation);
            }

            return false;
        }

        /// <summary>Remove an item from the pet's inventory.</summary>
        public Item? RemoveItem(int slot)
        {
            if (slot < 0 || slot >= _inventory.Count)
                return null;

            Item? item = _inventory[slot];
            _inventory[slot] = null;
            return item;
        }

        /// <summary>Clear all items from inventory.</summary>
        public void Clear()
        {
            for (int i = 0; i < _inventory.Count; i++)
            {
                _inventory[i] = null;
            }
        }

        /// <summary>Save inventory to pet's modData.</summary>
        public void Save(Pet pet)
        {
            try
            {
                var saveData = new List<ItemSaveData>();

                foreach (var item in _inventory)
                {
                    if (item != null)
                    {
                        saveData.Add(new ItemSaveData
                        {
                            QualifiedItemId = item.QualifiedItemId,
                            Stack = item.Stack,
                            Quality = (item is StardewValley.Object obj) ? obj.Quality : 0
                        });
                    }
                    else
                    {
                        saveData.Add(null!);
                    }
                }

                string json = JsonSerializer.Serialize(saveData);
                pet.modData[INVENTORY_KEY] = json;

                ModEntry.Instance.Monitor.Log($"Saved {ItemCount} items to pet inventory.", LogLevel.Debug);
            }
            catch (Exception ex)
            {
                ModEntry.Instance.Monitor.Log($"Error saving pet inventory: {ex}", LogLevel.Error);
            }
        }

        /// <summary>Load inventory from pet's modData.</summary>
        public void Load(Pet pet)
        {
            try
            {
                Clear();

                if (!pet.modData.TryGetValue(INVENTORY_KEY, out string? json) || string.IsNullOrEmpty(json))
                {
                    ModEntry.Instance.Monitor.Log("No saved pet inventory found.", LogLevel.Debug);
                    return;
                }

                var saveData = JsonSerializer.Deserialize<List<ItemSaveData>>(json);
                if (saveData == null)
                    return;

                for (int i = 0; i < saveData.Count && i < _inventory.Count; i++)
                {
                    var data = saveData[i];
                    if (data != null && !string.IsNullOrEmpty(data.QualifiedItemId))
                    {
                        var item = ItemRegistry.Create(data.QualifiedItemId, data.Stack);
                        if (item is StardewValley.Object obj)
                        {
                            obj.Quality = data.Quality;
                        }
                        _inventory[i] = item;
                    }
                }

                ModEntry.Instance.Monitor.Log($"Loaded {ItemCount} items from pet inventory.", LogLevel.Debug);
            }
            catch (Exception ex)
            {
                ModEntry.Instance.Monitor.Log($"Error loading pet inventory: {ex}", LogLevel.Error);
            }
        }

        /*********
        ** Private classes
        *********/
        private class ItemSaveData
        {
            public string QualifiedItemId { get; set; } = "";
            public int Stack { get; set; }
            public int Quality { get; set; }
        }
    }
}
