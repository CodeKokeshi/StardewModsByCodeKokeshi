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
            bool isWorking = ModEntry.WorkManager.IsWorking;
            int totalItems = ModEntry.InventoryManager.TotalItemCount;

            // Build response options - natural dialogue style
            var responses = new List<Response>();

            if (isWorking)
            {
                responses.Add(new Response("ToggleWork", $"Let {petName} rest"));
            }
            else
            {
                responses.Add(new Response("ToggleWork", $"Ask {petName} to help around the farm"));
            }

            if (totalItems > 0)
            {
                responses.Add(new Response("OpenInventory", $"Check what {petName} has found ({totalItems} items)"));
            }
            else
            {
                responses.Add(new Response("OpenInventory", $"Check {petName}'s pouch"));
            }

            responses.Add(new Response("PetThem", $"Give {petName} some love"));
            responses.Add(new Response("Cancel", "Never mind"));

            // Create question dialogue
            string greeting = isWorking 
                ? $"{petName} pauses and looks at you, tail wagging."
                : $"{petName} looks up at you expectantly.";

            location.createQuestionDialogue(
                greeting,
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

            // Play sound and show message
            if (ModEntry.WorkManager.IsWorking)
            {
                Game1.playSound("questcomplete");
                Game1.addHUDMessage(new HUDMessage($"{petName} is now helping on the farm!", HUDMessage.newQuest_type));
            }
            else
            {
                Game1.playSound("breathout");
                Game1.addHUDMessage(new HUDMessage($"{petName} is taking a break.", HUDMessage.newQuest_type));
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
                    message: $"{petName}'s Finds",
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
                Game1.addHUDMessage(new HUDMessage("Something went wrong!", HUDMessage.error_type));
            }
        }

        private static void OnItemGrabbed(Item item, Farmer who)
        {
            // Item was taken from pet inventory
            Game1.playSound("coin");
        }

        private static void HandlePetAction(Pet? pet, Farmer who)
        {
            if (pet == null || pet.currentLocation == null)
                return;

            string petName = pet.Name ?? "Your pet";

            // Check if already petted today using lastPetDay (the vanilla way)
            if (pet.lastPetDay.TryGetValue(who.UniqueMultiplayerID, out int lastDay) && lastDay == Game1.Date.TotalDays)
            {
                // Already petted today
                pet.doEmote(32); // Happy emote
                Game1.drawObjectDialogue($"{petName} purrs contentedly. You've already given them attention today.");
                return;
            }

            // Mark as petted today
            pet.lastPetDay[who.UniqueMultiplayerID] = Game1.Date.TotalDays;

            // Grant friendship if not already granted
            if (!pet.grantedFriendshipForPet.Value)
            {
                pet.grantedFriendshipForPet.Value = true;
                pet.friendshipTowardFarmer.Value = Math.Min(Pet.maxFriendship, pet.friendshipTowardFarmer.Value + 12);
            }

            // Visual and audio feedback
            pet.doEmote(20); // Heart emote
            pet.playContentSound();

            Game1.drawObjectDialogue($"{petName} loves the attention!");
        }
    }
}
