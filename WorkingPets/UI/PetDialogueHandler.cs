using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Menus;

namespace WorkingPets.UI
{
    /// <summary>Handles pet dialogue and menu interactions.</summary>
    public static class PetDialogueHandler
    {
        /*********
        ** Public methods
        *********/
        /// <summary>Show the custom pet interaction menu.</summary>
        public static void ShowPetMenu(Pet pet, Farmer who, GameLocation location)
        {
            string petName = pet.Name ?? "Your pet";
            string workStatus = ModEntry.WorkManager.IsWorking ? "ON" : "OFF";
            int itemCount = ModEntry.InventoryManager.ItemCount;
            int totalItems = ModEntry.InventoryManager.TotalItemCount;

            // Build response options
            var responses = new List<Response>
            {
                new Response("ToggleWork", $"Toggle Work Mode [Currently: {workStatus}]"),
                new Response("OpenInventory", $"Check Inventory ({itemCount} slots used, {totalItems} items)"),
                new Response("PetThem", "Pet them"),
                new Response("Cancel", "Cancel")
            };

            // Create question dialogue
            location.createQuestionDialogue(
                $"{petName} looks at you expectantly!",
                responses.ToArray(),
                HandlePetResponse
            );
        }

        /*********
        ** Private methods
        *********/
        private static void HandlePetResponse(Farmer who, string answer)
        {
            Pet? pet = ModEntry.GetPlayerPet();

            switch (answer)
            {
                case "ToggleWork":
                    HandleToggleWork(pet);
                    break;

                case "OpenInventory":
                    HandleOpenInventory(pet);
                    break;

                case "PetThem":
                    HandlePetAction(pet, who);
                    break;

                case "Cancel":
                default:
                    // Do nothing
                    break;
            }
        }

        private static void HandleToggleWork(Pet? pet)
        {
            ModEntry.WorkManager.ToggleWork();

            string petName = pet?.Name ?? "Your pet";
            string status = ModEntry.WorkManager.IsWorking ? "started working" : "stopped working";
            string statusColor = ModEntry.WorkManager.IsWorking ? "green" : "red";

            Game1.addHUDMessage(new HUDMessage($"{petName} has {status}!", HUDMessage.newQuest_type));

            // Play sound
            if (ModEntry.WorkManager.IsWorking)
            {
                Game1.playSound("questcomplete");
            }
            else
            {
                Game1.playSound("breathout");
            }
        }

        private static void HandleOpenInventory(Pet? pet)
        {
            try
            {
                string petName = pet?.Name ?? "Pet";

                // Create inventory list for ItemGrabMenu
                var inventoryList = ModEntry.InventoryManager.Inventory;

                // Open chest-like menu
                Game1.activeClickableMenu = new ItemGrabMenu(
                    inventory: inventoryList,
                    reverseGrab: false,
                    showReceivingMenu: true,
                    highlightFunction: InventoryMenu.highlightAllItems,
                    behaviorOnItemSelectFunction: null,
                    message: $"{petName}'s Inventory",
                    behaviorOnItemGrab: OnItemGrabbed,
                    snapToBottom: false,
                    canBeExitedWithKey: true,
                    playRightClickSound: true,
                    allowRightClick: true,
                    showOrganizeButton: true,
                    source: ItemGrabMenu.source_chest
                );

                Game1.playSound("openBox");
            }
            catch (Exception ex)
            {
                ModEntry.Instance.Monitor.Log($"Error opening pet inventory: {ex}", LogLevel.Error);
                Game1.addHUDMessage(new HUDMessage("Error opening inventory!", HUDMessage.error_type));
            }
        }

        private static void OnItemGrabbed(Item item, Farmer who)
        {
            // Item was taken from pet inventory
            Game1.playSound("coin");
        }

        private static void HandlePetAction(Pet? pet, Farmer who)
        {
            if (pet == null)
                return;

            // Use the vanilla checkAction method which handles lastPetDay, mutex, and grantedFriendshipForPet properly
            // This ensures compatibility with mods that track pet status
            bool result = pet.checkAction(who, pet.currentLocation);
            
            if (!result)
            {
                // Already petted today - show a soft acknowledgement
                Game1.drawObjectDialogue($"{pet.Name} seems happy today.");
            }
        }
    }
}
