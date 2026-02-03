using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;

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
        /// Will match item quality to existing chest items if needed.
        /// </summary>
        /// <param name="item">The item to deposit.</param>
        /// <param name="chests">List of all chests to check.</param>
        /// <returns>True if the item was fully deposited, false otherwise.</returns>
        private bool TryDepositItem(Item item, List<Chest> chests)
        {
            // Find a chest that already contains this item type (any quality)
            var result = FindMatchingChestAndQuality(item, chests);

            if (result.Chest == null)
                return false;

            // If we need to change quality to match chest contents, do so
            if (result.MatchedQuality >= 0 && item is StardewValley.Object obj && obj.Quality != result.MatchedQuality)
            {
                _monitor?.Log($"[ChestStorageManager] Adjusting {item.DisplayName} quality from {obj.Quality} to {result.MatchedQuality} to match chest.", LogLevel.Trace);
                obj.Quality = result.MatchedQuality;
            }

            // Try to add the item to the chest
            // Chest.addItem returns null if fully added, or the remaining item if partial/failed
            Item? remaining = result.Chest.addItem(item);

            return remaining == null;
        }

        /// <summary>
        /// Find a chest that contains at least one item of the same type (ignoring quality).
        /// Returns the chest and the quality of the matching item so we can adjust.
        /// </summary>
        /// <param name="item">The item to find a matching chest for.</param>
        /// <param name="chests">List of all chests to search.</param>
        /// <returns>The matching chest and the quality to use, or null if none found.</returns>
        private (Chest? Chest, int MatchedQuality) FindMatchingChestAndQuality(Item item, List<Chest> chests)
        {
            int itemQuality = 0;
            if (item is StardewValley.Object obj)
                itemQuality = obj.Quality;

            foreach (Chest chest in chests)
            {
                // Skip special chest types that shouldn't be used for storage
                if (chest.SpecialChestType == Chest.SpecialChestTypes.MiniShippingBin ||
                    chest.SpecialChestType == Chest.SpecialChestTypes.Enricher)
                {
                    continue;
                }

                // Collect all qualities of this item type in the chest
                var qualitiesInChest = new List<int>();
                foreach (Item chestItem in chest.Items)
                {
                    if (chestItem == null) continue;

                    // Check if same item type by qualified ID
                    if (chestItem.QualifiedItemId == item.QualifiedItemId)
                    {
                        int chestItemQuality = 0;
                        if (chestItem is StardewValley.Object chestObj)
                            chestItemQuality = chestObj.Quality;
                        
                        if (!qualitiesInChest.Contains(chestItemQuality))
                            qualitiesInChest.Add(chestItemQuality);
                    }
                }

                if (qualitiesInChest.Count == 0)
                    continue;

                // Prefer exact quality match, otherwise choose highest quality
                int targetQuality = itemQuality;
                if (!qualitiesInChest.Contains(itemQuality))
                {
                    // No exact match - use highest quality in chest
                    targetQuality = qualitiesInChest.Max();
                }

                // Check if there's room (either stacking with adjusted quality or new slot)
                if (HasRoomForItemWithQuality(chest, item, targetQuality))
                {
                    return (chest, targetQuality);
                }
            }

            return (null, -1);
        }

        /// <summary>
        /// Check if a chest has room for an item with a specific quality.
        /// </summary>
        /// <param name="chest">The chest to check.</param>
        /// <param name="item">The item to check room for.</param>
        /// <param name="quality">The quality the item will have when deposited.</param>
        /// <returns>True if there's room, false otherwise.</returns>
        private bool HasRoomForItemWithQuality(Chest chest, Item item, int quality)
        {
            // Check if any existing stack of same item and target quality has room
            foreach (Item chestItem in chest.Items)
            {
                if (chestItem != null && 
                    chestItem.QualifiedItemId == item.QualifiedItemId)
                {
                    // Check if this item has the target quality
                    int chestItemQuality = 0;
                    if (chestItem is StardewValley.Object chestObj)
                    {
                        chestItemQuality = chestObj.Quality;
                    }

                    if (chestItemQuality == quality)
                    {
                        // Check if this stack has room
                        int maxStack = chestItem.maximumStackSize();
                        if (chestItem.Stack < maxStack)
                        {
                            return true;
                        }
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

                var result = FindMatchingChestAndQuality(item, allChests);
                
                if (result.Chest != null)
                {
                    string location = result.Chest.Location?.DisplayName ?? "Unknown";
                    string key = $"{item.DisplayName} (Q{item.Quality})";
                    preview[key] = location;
                }
            }

            return preview;
        }
    }
}
