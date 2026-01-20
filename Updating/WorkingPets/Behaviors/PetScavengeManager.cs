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
            { "Hunter", new[] { "(O)684", "(O)137", "(O)145" } },     // Bug Meat, Smallmouth Bass, Sunfish
            { "Digger", new[] { "(O)330", "(O)770", "(O)273" } },     // Clay, Mixed Seeds, Rice Shoots
            { "Crow", new[] { "(O)80", "(O)86", "(O)109" } },         // Quartz, Earth Crystal, Rusty Spoon
            { "Gardener", new[] { "(O)771", "(O)16", "(O)18" } },     // Fiber, Horseradish, Daffodil
            { "Trash Cat", new[] { "(O)172", "(O)169" } }             // Soggy Newspaper, Driftwood
        };

        public void PerformDailyScavenge(Pet pet)
        {
            // Only scavenge if the pet was "working"
            var manager = ModEntry.PetManager.GetManagerForPet(pet);
            if (manager == null || !manager.IsWorking)
                return;

            // Limit to max 5 items per day as requested
            int itemsToFind = _random.Next(1, 6); // 1 to 5 random items

            int addedCount = 0;
            for (int i = 0; i < itemsToFind; i++)
            {
                string category = PickRandomCategory();
                string[] possibleItems = _lootTables[category];
                string itemId = possibleItems[_random.Next(possibleItems.Length)];

                Item newItem = ItemRegistry.Create(itemId, 1);
                
                // Add to inventory
                if (ModEntry.InventoryManager.AddItem(newItem))
                {
                    addedCount++;
                }
            }

            if (addedCount > 0)
            {
                // Simple notification in the morning
                Game1.addHUDMessage(new HUDMessage($"{pet.Name} found {addedCount} new items overnight!", HUDMessage.newQuest_type));
            }
        }

        private string PickRandomCategory()
        {
            // Uniformly pick a category
            return _lootTables.Keys.ElementAt(_random.Next(_lootTables.Count));
        }
    }
}
