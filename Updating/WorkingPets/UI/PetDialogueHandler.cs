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
        ** Private methods
        *********/
        /// <summary>Play a sound appropriate for the pet type (dog barks, cat meows, turtle chirps, etc.).</summary>
        private static void PlayPetSound(Pet? pet, string soundType = "BARK")
        {
            if (pet == null) return;
            
            // Use the pet's built-in PlaySound method which handles type-specific sounds
            // "BARK" gets converted to the appropriate sound for each pet type
            pet.PlaySound(soundType, true, -1, -1);
        }

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
            
            string petName = pet.Name ?? ModEntry.I18n.Get("pet.genericName");
            bool isWorking = manager?.IsWorking ?? false;
            bool isFollowing = manager?.IsFollowing ?? false;
            bool isExploring = manager?.IsExploring ?? false;
            int totalItems = manager?.InventoryManager?.TotalItemCount ?? 0;

            // Build response options - natural dialogue style
            var responses = new List<Response>();

            // Follow option
            if (isFollowing)
            {
                responses.Add(new Response("ToggleFollow", ModEntry.I18n.Get("petMenu.option.follow.stop", new { petName })));
            }
            else
            {
                responses.Add(new Response("ToggleFollow", ModEntry.I18n.Get("petMenu.option.follow.start", new { petName })));
            }
            
            // Explore option - autonomous foraging across the valley
            if (isExploring)
            {
                responses.Add(new Response("ToggleExplore", ModEntry.I18n.Get("petMenu.option.explore.stop", new { petName })));
            }
            else if (manager?.CanExplore == true)
            {
                // Can explore (hasn't explored today OR exploration was paused)
                if (manager?.IsExplorePaused == true)
                {
                    responses.Add(new Response("ToggleExplore", ModEntry.I18n.Get("petMenu.option.explore.resume", new { petName })));
                }
                else
                {
                    responses.Add(new Response("ToggleExplore", ModEntry.I18n.Get("petMenu.option.explore.start", new { petName })));
                }
            }
            // If !CanExplore, don't show the explore option (already done for today)

            // Work option (always available - selecting it cancels follow mode)
            if (isWorking)
            {
                responses.Add(new Response("ToggleWork", ModEntry.I18n.Get("petMenu.option.work.stop", new { petName })));
            }
            else
            {
                responses.Add(new Response("ToggleWork", ModEntry.I18n.Get("petMenu.option.work.start", new { petName })));
            }

            if (totalItems > 0)
            {
                responses.Add(new Response("OpenInventory", ModEntry.I18n.Get("petMenu.option.inventory.withCount", new { petName, totalItems })));
            }
            else
            {
                responses.Add(new Response("OpenInventory", ModEntry.I18n.Get("petMenu.option.inventory.empty", new { petName })));
            }

            responses.Add(new Response("PetThem", ModEntry.I18n.Get("petMenu.option.pet", new { petName })));

            // One-time rename option - only show if not used yet
            if (!pet.modData.ContainsKey(RENAME_USED_KEY))
            {
                responses.Add(new Response("Rename", ModEntry.I18n.Get("petMenu.option.rename", new { petName })));
            }

            responses.Add(new Response("Cancel", ModEntry.I18n.Get("petMenu.option.cancel")));

            // Create question dialogue
            string greeting;
            if (isExploring)
                greeting = ModEntry.I18n.Get("petMenu.greeting.exploring", new { petName });
            else if (isFollowing)
                greeting = ModEntry.I18n.Get("petMenu.greeting.following", new { petName });
            else if (isWorking)
                greeting = ModEntry.I18n.Get("petMenu.greeting.working", new { petName });
            else
                greeting = ModEntry.I18n.Get("petMenu.greeting.idle", new { petName });

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
                    
                case "ToggleExplore":
                    HandleToggleExplore(pet, manager);
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
                PlayPetSound(pet, "BARK");
                if (ModEntry.Config.ShowStateNotifications)
                    Game1.addHUDMessage(new HUDMessage(ModEntry.I18n.Get("hud.follow.start", new { petName }), HUDMessage.newQuest_type));
            }
            else
            {
                PlayPetSound(pet, "BARK");
                if (ModEntry.Config.ShowStateNotifications)
                    Game1.addHUDMessage(new HUDMessage(ModEntry.I18n.Get("hud.follow.stop", new { petName }), HUDMessage.newQuest_type));
            }
        }
        
        private static void HandleToggleExplore(Pet? pet, PetWorkManager? manager)
        {
            if (manager == null) return;
            
            manager.ToggleExplore();

            string petName = pet?.Name ?? "Your pet";

            if (manager.IsExploring)
            {
                PlayPetSound(pet, "BARK");
                if (ModEntry.Config.ShowStateNotifications)
                    Game1.addHUDMessage(new HUDMessage(ModEntry.I18n.Get("hud.explore.start", new { petName }), HUDMessage.newQuest_type));
            }
            else
            {
                PlayPetSound(pet, "BARK");
                if (ModEntry.Config.ShowStateNotifications)
                    Game1.addHUDMessage(new HUDMessage(ModEntry.I18n.Get("hud.explore.stop", new { petName }), HUDMessage.newQuest_type));
            }
        }

        private static void HandleToggleWork(Pet? pet, PetWorkManager? manager)
        {
            if (manager == null) return;
            
            string petName = pet?.Name ?? "Your pet";
            
            // If trying to start work, do a quick pre-check
            if (!manager.IsWorking)
            {
                // Check if there's any work to do BEFORE starting
                if (!PetWorkManager.HasAnyWorkOnFarm())
                {
                    PlayPetSound(pet, "BARK");
                    Game1.addHUDMessage(new HUDMessage($"{petName} looked around but found nothing to do!", HUDMessage.newQuest_type));
                    return; // Don't start work mode
                }
            }
            
            // If currently following, stop following first
            if (manager.IsFollowing)
            {
                manager.StopFollowing();
            }
            
            manager.ToggleWork();

            // Play sound and show message
            if (manager.IsWorking)
            {
                PlayPetSound(pet, "BARK");
                if (ModEntry.Config.ShowStateNotifications)
                    Game1.addHUDMessage(new HUDMessage(ModEntry.I18n.Get("hud.work.start", new { petName }), HUDMessage.newQuest_type));
            }
            else
            {
                PlayPetSound(pet, "BARK");
                if (ModEntry.Config.ShowStateNotifications)
                    Game1.addHUDMessage(new HUDMessage(ModEntry.I18n.Get("hud.work.stop", new { petName }), HUDMessage.newQuest_type));
            }
        }

        private static void HandleOpenInventory(Pet? pet)
        {
            try
            {
                string petName = pet?.Name ?? ModEntry.I18n.Get("pet.genericName");
                
                // Get the manager for this specific pet
                var manager = ModEntry.PetManager?.GetManagerForPet(pet!);
                if (manager == null) return;

                // Create inventory list for ItemGrabMenu
                var inventoryList = manager.InventoryManager.Inventory;

                // Open chest-like menu
                Game1.activeClickableMenu = new ItemGrabMenu(
                    inventory: inventoryList,
                    reverseGrab: false,
                    showReceivingMenu: true,
                    highlightFunction: InventoryMenu.highlightAllItems,
                    behaviorOnItemSelectFunction: null,
                    message: ModEntry.I18n.Get("petMenu.inventory.title", new { petName }),
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
                Game1.addHUDMessage(new HUDMessage(ModEntry.I18n.Get("hud.genericError"), HUDMessage.error_type));
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
