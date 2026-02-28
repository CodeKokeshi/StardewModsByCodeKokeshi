using System;
using System.Collections.Generic;
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

            try
            {
                // Use the pet's built-in PlaySound method which handles type-specific sounds
                // "BARK" gets converted to the appropriate sound for each pet type
                // Wrapped in try-catch because some pet types (e.g. turtle) may have
                // null or missing sound cues, causing ArgumentNullException in SoundBank
                pet.PlaySound(soundType, true, -1, -1);
            }
            catch (Exception)
            {
                // Sound not available for this pet type — silently ignore
            }
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
            bool isFollowing = manager?.IsFollowing ?? false;
            int totalItems = manager?.InventoryManager?.TotalItemCount ?? 0;

            // Build response options
            var responses = new List<Response>();

            // 1. Follow option
            if (isFollowing)
                responses.Add(new Response("ToggleFollow", ModEntry.I18n.Get("petMenu.option.follow.stop", new { petName })));
            else
                responses.Add(new Response("ToggleFollow", ModEntry.I18n.Get("petMenu.option.follow.start", new { petName })));

            // 2. Pouch (inventory)
            if (totalItems > 0)
                responses.Add(new Response("OpenInventory", ModEntry.I18n.Get("petMenu.option.inventory.withCount", new { petName, totalItems })));
            else
                responses.Add(new Response("OpenInventory", ModEntry.I18n.Get("petMenu.option.inventory.empty", new { petName })));

            // 3. Love
            responses.Add(new Response("PetThem", ModEntry.I18n.Get("petMenu.option.pet", new { petName })));

            // 4. Pet Manager
            responses.Add(new Response("OpenPetManager", ModEntry.I18n.Get("petMenu.option.petManager")));

            // 5. Never mind
            responses.Add(new Response("Cancel", ModEntry.I18n.Get("petMenu.option.cancel")));

            // Create question dialogue
            string greeting;
            if (isFollowing)
                greeting = ModEntry.I18n.Get("petMenu.greeting.following", new { petName });
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

                case "OpenInventory":
                    HandleOpenInventory(pet);
                    break;

                case "PetThem":
                    HandlePetAction(pet, who);
                    break;

                case "OpenPetManager":
                    // Close dialogue, then open the Pet Manager UI
                    Game1.dialogueUp = false;
                    Game1.player.canMove = true;
                    ModEntry.Instance.OpenPetManagerMenu();
                    break;

                case "Cancel":
                default:
                    break;
            }
            
            // Clear the tracked pet after handling response
            _currentDialoguePet = null;
        }

        private static void HandleToggleFollow(Pet? pet, PetWorkManager? manager)
        {
            if (manager == null) return;
            
            manager.ToggleFollow();

            string petName = pet?.Name ?? ModEntry.I18n.Get("pet.genericName");

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
                ModEntry.Instance.Monitor.Log(ModEntry.I18n.Get("log.petInventory.openError", new { error = ex }), LogLevel.Error);
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

            string petName = pet.Name ?? ModEntry.I18n.Get("pet.genericName");

            // Check if already petted today using lastPetDay (the vanilla way)
            if (pet.lastPetDay.TryGetValue(who.UniqueMultiplayerID, out int lastDay) && lastDay == Game1.Date.TotalDays)
            {
                // Already petted today
                pet.doEmote(32); // Happy emote
                Game1.drawObjectDialogue(ModEntry.I18n.Get("petMenu.petAction.alreadyPetted", new { petName }));
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

            Game1.drawObjectDialogue(ModEntry.I18n.Get("petMenu.petAction.love", new { petName }));
        }
    }
}
