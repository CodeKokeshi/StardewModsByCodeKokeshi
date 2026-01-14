using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Menus;
using System;
using System.Reflection;

namespace WorkingPets.Patches;

/// <summary>
/// Patches AnimalPage to add whistle functionality when clicking on pets.
/// Whistle = switch pet to follow mode (only if in same location as player).
/// </summary>
[HarmonyPatch]
public static class AnimalPagePatch
{
    /// <summary>
    /// Patch the AnimalPage.draw method to show whistle button.
    /// </summary>
    [HarmonyPatch(typeof(AnimalPage), "draw", typeof(SpriteBatch))]
    [HarmonyPostfix]
    public static void draw_Postfix(AnimalPage __instance, SpriteBatch b)
    {
        try
        {
            UI.WhistleButton.Draw(b, __instance);
        }
        catch (Exception ex)
        {
            ModEntry.Instance?.Monitor.Log($"Error drawing whistle button: {ex}", LogLevel.Error);
        }
    }

    /// <summary>
    /// Patch the AnimalPage.receiveLeftClick method to detect pet clicks.
    /// </summary>
    [HarmonyPatch(typeof(AnimalPage), "receiveLeftClick")]
    [HarmonyPostfix]
    public static void receiveLeftClick_Postfix(AnimalPage __instance, int x, int y, bool playSound)
    {
        try
        {
            // Get the clicked slot
            int clickedSlot = GetClickedSlot(__instance, x, y);
            if (clickedSlot < 0)
                return;

            // Get the animal entry for this slot
            var entries = GetAnimalEntries(__instance);
            if (entries == null || clickedSlot >= entries.Count)
                return;

            var entry = entries[clickedSlot];
            if (entry == null || entry.Animal is not Pet pet)
                return;

            // Whistle the pet!
            WhistlePet(pet);
        }
        catch (Exception ex)
        {
            ModEntry.Instance?.Monitor.Log($"Error in AnimalPage whistle: {ex}", LogLevel.Error);
        }
    }

    /// <summary>
    /// Whistle a pet to come to the player (switches to follow mode if in same location).
    /// </summary>
    private static void WhistlePet(Pet pet)
    {
        var player = Game1.player;
        
        // Check if pet is in the same location
        if (pet.currentLocation != player.currentLocation)
        {
            // Pet is in a different location - show message
            Game1.showRedMessage($"{pet.displayName} is too far away to hear!");
            Game1.playSound("cancel");
            return;
        }

        // Get pet's work manager
        var manager = ModEntry.PetManager?.GetManagerForPet(pet);
        if (manager == null)
        {
            ModEntry.Instance?.Monitor.Log($"No manager found for pet {pet.displayName}", LogLevel.Warn);
            return;
        }

        // Check if already following
        if (manager.IsFollowing && !manager.IsWorking)
        {
            Game1.showGlobalMessage($"{pet.displayName} is already following you!");
            Game1.playSound("breathin");
            return;
        }

        // Switch to follow mode
        if (manager.IsWorking)
        {
            manager.ToggleWork(); // Turn off working
        }
        
        if (!manager.IsFollowing)
        {
            manager.ToggleFollow(); // Turn on following
        }
        
        // Show success message
        Game1.showGlobalMessage($"{pet.displayName} is coming to you!");
        Game1.playSound("whistle"); // Nice whistle sound effect
        
        ModEntry.Instance?.Monitor.Log($"Whistled {pet.displayName} to follow mode", LogLevel.Debug);
    }

    /// <summary>
    /// Get the index of the slot that was clicked, or -1 if none.
    /// </summary>
    private static int GetClickedSlot(AnimalPage page, int x, int y)
    {
        try
        {
            // Use reflection to access characterSlots (List<ClickableTextureComponent>)
            var slotsField = typeof(AnimalPage).GetField("characterSlots", BindingFlags.NonPublic | BindingFlags.Instance);
            if (slotsField == null)
                return -1;

            var slots = slotsField.GetValue(page) as System.Collections.Generic.List<ClickableTextureComponent>;
            if (slots == null)
                return -1;

            // Check which slot was clicked
            for (int i = 0; i < slots.Count; i++)
            {
                if (slots[i].containsPoint(x, y))
                {
                    return i;
                }
            }
        }
        catch (Exception ex)
        {
            ModEntry.Instance?.Monitor.Log($"Error getting clicked slot: {ex}", LogLevel.Error);
        }

        return -1;
    }

    /// <summary>
    /// Get the AnimalEntries list from the AnimalPage.
    /// </summary>
    private static System.Collections.Generic.List<AnimalPage.AnimalEntry>? GetAnimalEntries(AnimalPage page)
    {
        try
        {
            var entriesField = typeof(AnimalPage).GetField("AnimalEntries", BindingFlags.Public | BindingFlags.Instance);
            if (entriesField == null)
                return null;

            return entriesField.GetValue(page) as System.Collections.Generic.List<AnimalPage.AnimalEntry>;
        }
        catch (Exception ex)
        {
            ModEntry.Instance?.Monitor.Log($"Error getting animal entries: {ex}", LogLevel.Error);
            return null;
        }
    }
}
