using System;
using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Characters;

namespace WorkingPets.Behaviors
{
    public class PetScavengeManager
    {
        private readonly Random _random = new();

        private readonly Dictionary<string, string[]> _lootTables = new()
        {
            // Common / safe finds
            { "CommonForage", new[] { "(O)16", "(O)18", "(O)20", "(O)22", "(O)396", "(O)398", "(O)404" } }, // Horseradish, Daffodil, Leek, Dandelion, Spice Berry, Grape, Common Mushroom
            { "SeedsAndFiber", new[] { "(O)771", "(O)770", "(O)330", "(O)92", "(O)297" } }, // Fiber, Mixed Seeds, Clay, Sap, Wild Seed (misc)
            { "RocksAndOre", new[] { "(O)390", "(O)378", "(O)380", "(O)382", "(O)384", "(O)386" } }, // Stone, Copper/iron/gold/iridium ore, coal
            { "CritterLoot", new[] { "(O)684", "(O)766", "(O)767", "(O)768", "(O)769" } }, // Bug Meat, Slime, Bat Wing, Solar Essence, Void Essence

            // A bit of variety (still mostly low value)
            { "ShellsAndBeach", new[] { "(O)372", "(O)392", "(O)393", "(O)394", "(O)397", "(O)718" } }, // Clam, Nautilus Shell, Coral, Rainbow Shell, Sea Urchin, Cockle
            { "ArtifactsAndJunk", new[] { "(O)102", "(O)103", "(O)109", "(O)110", "(O)111", "(O)112", "(O)113", "(O)114", "(O)115", "(O)116", "(O)117", "(O)118", "(O)119", "(O)120" } },
            { "Trash", new[] { "(O)168", "(O)169", "(O)170", "(O)171", "(O)172" } }, // Trash, Driftwood, Broken Glasses, Broken CD, Soggy Newspaper
            { "FoodSnacks", new[] { "(O)194", "(O)210", "(O)216" } }, // Fried Egg, Bread, Tortilla (small fun finds)
        };

        public void PerformDailyScavenge(Pet pet)
        {
            if (pet == null)
                return;

            // Always scavenge overnight (working/following/idle doesn't matter).
            // Keep quantity small, but allow a wide variety of possible items.
            int itemsToFind = _random.Next(1, 4); // 1 to 3 items

            int addedCount = 0;
            for (int i = 0; i < itemsToFind; i++)
            {
                string category = PickRandomCategory();
                string[] possibleItems = _lootTables[category];
                string itemId = possibleItems[_random.Next(possibleItems.Length)];

                Item? newItem = null;
                try
                {
                    newItem = ItemRegistry.Create(itemId, 1);
                }
                catch
                {
                    // Ignore invalid IDs (e.g. if game data changes).
                }
                if (newItem == null)
                    continue;
                
                // Add to inventory
                if (ModEntry.InventoryManager.AddItem(newItem))
                {
                    addedCount++;
                }
            }

            if (addedCount > 0)
            {
                // Simple notification in the morning
                Game1.addHUDMessage(new HUDMessage(ModEntry.I18n.Get("hud.scavenge.morning", new { petName = pet.Name, addedCount }), HUDMessage.newQuest_type));
            }
        }

        private string PickRandomCategory()
        {
            // Uniformly pick a category
            return _lootTables.Keys.ElementAt(_random.Next(_lootTables.Count));
        }
    }
}
