using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using System;
using System.Collections.Generic;

namespace WorkingPets.Behaviors
{
    /// <summary>
    /// Manages automatic storage of pet-collected items into matching chests.
    /// Scans all chests in the game and deposits items that match existing chest contents.
    /// </summary>
    public class ChestStorageManager
    {
        private readonly IMonitor _monitor;

        public ChestStorageManager(IMonitor monitor)
        {
            _monitor = monitor;
        }

        /// <summary>
        /// Deposit pet inventory items into matching chests across all game locations.
        /// Items are only deposited into chests that already contain the same item with matching quality.
        /// </summary>
        /// <param name="petInventory">The pet's inventory items to deposit.</param>
        /// <returns>List of items that could not be deposited (no matching chest found).</returns>
        public List<Item> DepositItemsToMatchingChests(IList<Item> petInventory)
        {
            if (petInventory == null || petInventory.Count == 0)
                return new List<Item>();

            // Collect all chests from all locations
            List<Chest> allChests = GetAllChests();

            if (allChests.Count == 0)
            {
                _monitor?.Log("[ChestStorageManager] No chests found in game.", LogLevel.Trace);
                return new List<Item>(petInventory);
            }

            _monitor?.Log($"[ChestStorageManager] Found {allChests.Count} chests to scan.", LogLevel.Trace);

            List<Item> remainingItems = new List<Item>();
            int depositedCount = 0;

            foreach (Item item in petInventory)
            {
                if (item == null) continue;

                bool deposited = TryDepositItem(item, allChests);
                
                if (deposited)
                {
                    depositedCount++;
                    _monitor?.Log($"[ChestStorageManager] Deposited {item.Stack}x {item.DisplayName} (Quality: {item.Quality}) to matching chest.", LogLevel.Debug);
                }
                else
                {
                    remainingItems.Add(item);
                    _monitor?.Log($"[ChestStorageManager] No matching chest found for {item.DisplayName} (Quality: {item.Quality}).", LogLevel.Trace);
                }
            }

            _monitor?.Log($"[ChestStorageManager] Deposited {depositedCount} items, {remainingItems.Count} items remain.", LogLevel.Debug);

            return remainingItems;
        }

        /// <summary>
        /// Try to deposit an item into a matching chest.
        /// </summary>
        /// <param name="item">The item to deposit.</param>
        /// <param name="chests">List of all chests to check.</param>
        /// <returns>True if the item was fully deposited, false otherwise.</returns>
        private bool TryDepositItem(Item item, List<Chest> chests)
        {
            // Find a chest that already contains this item with matching quality
            Chest? matchingChest = FindMatchingChest(item, chests);

            if (matchingChest == null)
                return false;

            // Try to add the item to the chest
            // Chest.addItem returns null if fully added, or the remaining item if partial/failed
            Item remaining = matchingChest.addItem(item);

            return remaining == null;
        }

        /// <summary>
        /// Find a chest that contains at least one item that can stack with the given item.
        /// This ensures items only go to chests that already have the same item type and quality.
        /// </summary>
        /// <param name="item">The item to find a matching chest for.</param>
        /// <param name="chests">List of all chests to search.</param>
        /// <returns>The first matching chest, or null if none found.</returns>
        private Chest? FindMatchingChest(Item item, List<Chest> chests)
        {
            foreach (Chest chest in chests)
            {
                // Skip special chest types that shouldn't be used for storage
                if (chest.SpecialChestType == Chest.SpecialChestTypes.MiniShippingBin ||
                    chest.SpecialChestType == Chest.SpecialChestTypes.Enricher)
                {
                    continue;
                }

                // Check if this chest contains an item that can stack with our item
                foreach (Item chestItem in chest.Items)
                {
                    if (chestItem == null) continue;

                    // canStackWith checks: type, quality, itemId, name, etc.
                    if (chestItem.canStackWith(item))
                    {
                        // Found a chest with matching item - check if there's room
                        if (HasRoomForItem(chest, item))
                        {
                            return chest;
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Check if a chest has room for an item (either stacking or new slot).
        /// </summary>
        /// <param name="chest">The chest to check.</param>
        /// <param name="item">The item to check room for.</param>
        /// <returns>True if there's room, false otherwise.</returns>
        private bool HasRoomForItem(Chest chest, Item item)
        {
            // Check if any existing stack has room
            foreach (Item chestItem in chest.Items)
            {
                if (chestItem != null && chestItem.canStackWith(item))
                {
                    // Check if this stack has room
                    int maxStack = chestItem.maximumStackSize();
                    if (chestItem.Stack < maxStack)
                    {
                        return true;
                    }
                }
            }

            // Check if there's an empty slot
            int capacity = chest.GetActualCapacity();
            int usedSlots = 0;
            foreach (Item chestItem in chest.Items)
            {
                if (chestItem != null)
                    usedSlots++;
            }

            return usedSlots < capacity;
        }

        /// <summary>
        /// Collect all player chests from all game locations.
        /// </summary>
        /// <returns>List of all accessible chests.</returns>
        private List<Chest> GetAllChests()
        {
            List<Chest> chests = new List<Chest>();

            try
            {
                // Use Utility.ForEachLocation to iterate all locations including building interiors
                Utility.ForEachLocation((location) =>
                {
                    if (location == null) return true;

                    // Check all objects in the location
                    foreach (StardewValley.Object obj in location.objects.Values)
                    {
                        if (obj is Chest chest && chest.playerChest.Value)
                        {
                            chests.Add(chest);
                        }
                    }

                    // Check furniture for held objects that might be chests (like storage furniture)
                    foreach (var furniture in location.furniture)
                    {
                        // Check held objects in furniture
                        if (furniture.heldObject.Value is Chest heldChest && heldChest.playerChest.Value)
                        {
                            chests.Add(heldChest);
                        }
                    }

                    return true; // Continue iterating
                }, includeInteriors: true, includeGenerated: false);

                // Also check the fridge in farmhouse
                var farmhouse = Game1.getLocationFromName("FarmHouse") as StardewValley.Locations.FarmHouse;
                if (farmhouse?.fridge.Value is Chest fridgeChest)
                {
                    chests.Add(fridgeChest);
                }

                // Check fridges in cabins for multiplayer
                Utility.ForEachLocation((location) =>
                {
                    if (location is StardewValley.Locations.Cabin cabin && cabin.fridge.Value is Chest cabinFridge)
                    {
                        chests.Add(cabinFridge);
                    }
                    return true;
                }, includeInteriors: true, includeGenerated: false);
            }
            catch (Exception ex)
            {
                _monitor?.Log($"[ChestStorageManager] Error collecting chests: {ex.Message}", LogLevel.Warn);
            }

            return chests;
        }

        /// <summary>
        /// Get a summary of where items would be deposited (for debugging/display).
        /// </summary>
        /// <param name="petInventory">The pet's inventory items.</param>
        /// <returns>Dictionary of item display names to chest locations.</returns>
        public Dictionary<string, string> PreviewDeposits(IList<Item> petInventory)
        {
            var preview = new Dictionary<string, string>();
            var allChests = GetAllChests();

            foreach (Item item in petInventory)
            {
                if (item == null) continue;

                Chest? matchingChest = FindMatchingChest(item, allChests);
                
                if (matchingChest != null)
                {
                    string location = matchingChest.Location?.DisplayName ?? "Unknown";
                    string key = $"{item.DisplayName} (Q{item.Quality})";
                    preview[key] = location;
                }
            }

            return preview;
        }
    }
}
