using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Menus;
using WorkingPets.Behaviors;

namespace WorkingPets.UI
{
    /// <summary>Handles pet dialogue and menu interactions.</summary>
    public static class PetDialogueHandler
    {
        /*********
        ** Constants
        *********/
        private const string RENAME_USED_KEY = "WorkingPets.RenameUsed";
        
        /*********
        ** Fields
        *********/
        /// <summary>The pet currently being interacted with (for dialogue callbacks).</summary>
        private static Pet? _currentDialoguePet;

        /*********
        ** Public methods
        *********/
        /// <summary>Show the custom pet interaction menu.</summary>
        public static void ShowPetMenu(Pet pet, Farmer who, GameLocation location)
        {
            // Track which pet we're talking to
            _currentDialoguePet = pet;
            
            // Get this pet's work manager
            var manager = ModEntry.PetManager.GetManagerForPet(pet);
            
            // Pause THIS pet's movement while dialogue is open
            manager?.PauseForDialogue();
            
            string petName = pet.Name ?? "Your pet";
            bool isWorking = manager?.IsWorking ?? false;
            bool isFollowing = manager?.IsFollowing ?? false;
            int totalItems = ModEntry.InventoryManager.TotalItemCount;

            // Build response options - natural dialogue style
            var responses = new List<Response>();

            // Follow option
            if (isFollowing)
            {
                responses.Add(new Response("ToggleFollow", $"That's enough following for now, {petName}"));
            }
            else
            {
                responses.Add(new Response("ToggleFollow", $"Follow me, {petName}!"));
            }

            // Work option (always available - selecting it cancels follow mode)
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

            // One-time rename option - only show if not used yet
            if (!pet.modData.ContainsKey(RENAME_USED_KEY))
            {
                responses.Add(new Response("Rename", $"Give {petName} a new name"));
            }

            responses.Add(new Response("Cancel", "Never mind"));

            // Create question dialogue
            string greeting;
            if (isFollowing)
                greeting = $"{petName} is happily following you!";
            else if (isWorking)
                greeting = $"{petName} pauses and looks at you, tail wagging.";
            else
                greeting = $"{petName} looks up at you expectantly.";

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
            // Use the pet we were talking to, not just the first pet
            Pet? pet = _currentDialoguePet ?? ModEntry.GetPlayerPet();
            var manager = pet != null ? ModEntry.PetManager.GetManagerForPet(pet) : null;

            switch (answer)
            {
                case "ToggleFollow":
                    HandleToggleFollow(pet, manager);
                    break;

                case "ToggleWork":
                    HandleToggleWork(pet, manager);
                    break;

                case "OpenInventory":
                    HandleOpenInventory(pet);
                    break;

                case "PetThem":
                    HandlePetAction(pet, who);
                    break;

                case "Rename":
                    HandleRename(pet);
                    break;

                case "Cancel":
                default:
                    // Do nothing
                    break;
            }
            
            // Clear the tracked pet after handling response
            _currentDialoguePet = null;
        }

        private static void HandleToggleFollow(Pet? pet, PetWorkManager? manager)
        {
            if (manager == null) return;
            
            manager.ToggleFollow();

            string petName = pet?.Name ?? "Your pet";

            if (manager.IsFollowing)
            {
                Game1.playSound("dog_pant");
                Game1.addHUDMessage(new HUDMessage($"{petName} is now following you!", HUDMessage.newQuest_type));
            }
            else
            {
                Game1.playSound("cat");
                Game1.addHUDMessage(new HUDMessage($"{petName} is no longer following you.", HUDMessage.newQuest_type));
            }
        }

        private static void HandleToggleWork(Pet? pet, PetWorkManager? manager)
        {
            if (manager == null) return;
            
            // If currently following, stop following first
            if (manager.IsFollowing)
            {
                manager.StopFollowing();
            }
            
            manager.ToggleWork();

            string petName = pet?.Name ?? "Your pet";

            // Play sound and show message
            if (manager.IsWorking)
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

        private static void HandleRename(Pet? pet)
        {
            if (pet == null)
                return;

            string oldName = pet.Name ?? "Your pet";

            // Open the naming menu (same one used when first getting a pet)
            Game1.activeClickableMenu = new NamingMenu(
                (string newName) => 
                {
                    if (!string.IsNullOrWhiteSpace(newName))
                    {
                        // Set the new name exactly like vanilla does
                        pet.Name = newName;
                        pet.displayName = newName;

                        // Mark rename as used - this option won't appear in dialogue again
                        // but will now appear in settings menu instead
                        pet.modData[RENAME_USED_KEY] = "true";

                        Game1.playSound("newArtifact");
                        Game1.addHUDMessage(new HUDMessage($"{oldName} is now known as {newName}!", HUDMessage.newQuest_type));
                    }
                    
                    // Force close menu and ensure player can move
                    Game1.exitActiveMenu();
                    Game1.player.canMove = true;
                    Game1.dialogueUp = false;
                },
                Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1236"), // "Name your pet:"
                pet.Name // Default to current name
            );

            Game1.playSound("bigSelect");
        }
    }
}
