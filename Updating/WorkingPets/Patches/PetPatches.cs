using System;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Characters;
using WorkingPets.Behaviors;
using WorkingPets.UI;

namespace WorkingPets.Patches
{
    /// <summary>Harmony patches for Pet class.</summary>
    public static class PetPatches
    {
        private static IMonitor Monitor = null!;

        /// <summary>Apply all pet patches.</summary>
        public static void Apply(Harmony harmony, IMonitor monitor)
        {
            Monitor = monitor;

            harmony.Patch(
                original: AccessTools.Method(typeof(Pet), nameof(Pet.checkAction)),
                prefix: new HarmonyMethod(typeof(PetPatches), nameof(CheckAction_Prefix))
            );

            // Patch Pet.update to suppress vanilla movement when following/working
            harmony.Patch(
                original: AccessTools.Method(typeof(Pet), "RunState"),
                prefix: new HarmonyMethod(typeof(PetPatches), nameof(RunState_Prefix))
            );
            
            // Patch Pet behavior changes that cause stuttering
            harmony.Patch(
                original: AccessTools.Method(typeof(Pet), "behaviorOnFarmerLocationEntry"),
                prefix: new HarmonyMethod(typeof(PetPatches), nameof(BehaviorOnFarmerLocationEntry_Prefix))
            );
        }

        /// <summary>Suppress vanilla RunState when pet is following or working (prevents movement interference).</summary>
        public static bool RunState_Prefix(Pet __instance)
        {
            try
            {
                if (!ModEntry.Config.ModEnabled)
                    return true;

                var manager = ModEntry.PetManager?.GetManagerForPet(__instance);
                if (manager == null)
                    return true;

                // If the pet is following or working, skip vanilla RunState entirely
                // This prevents the vanilla behavior from changing direction/movement
                if (manager.IsFollowing || manager.IsWorking)
                {
                    return false; // Skip original
                }

                return true; // Run original for idle pets
            }
            catch (Exception ex)
            {
                Monitor.Log($"Error in Pet.RunState patch: {ex}", LogLevel.Error);
                return true;
            }
        }

        /// <summary>Prevent vanilla sleep/behavior changes when pet is following.</summary>
        public static bool BehaviorOnFarmerLocationEntry_Prefix(Pet __instance, GameLocation location, Farmer who)
        {
            try
            {
                if (!ModEntry.Config.ModEnabled)
                    return true;

                var manager = ModEntry.PetManager?.GetManagerForPet(__instance);
                if (manager == null)
                    return true;

                // If following, don't let the vanilla code mess with the pet's behavior
                if (manager.IsFollowing)
                {
                    return false; // Skip original - don't randomly start sleeping
                }

                return true;
            }
            catch (Exception ex)
            {
                Monitor.Log($"Error in Pet.behaviorOnFarmerLocationEntry patch: {ex}", LogLevel.Error);
                return true;
            }
        }

        /// <summary>Intercept pet interaction to show custom dialogue.</summary>
        public static bool CheckAction_Prefix(Pet __instance, Farmer who, GameLocation l, ref bool __result)
        {
            try
            {
                if (!ModEntry.Config.ModEnabled)
                    return true; // Use original behavior

                // Only open the custom command menu when the player is stationary.
                // This prevents accidentally opening the menu while holding right-click to walk near the pet.
                // Use the isMoving() check which returns true when player is in motion.
                if (who.isMoving())
                    return true; // Use original behavior (player is moving)

                // Don't intercept during events (like the naming cutscene)
                if (Game1.CurrentEvent != null)
                    return true; // Use original behavior

                // Don't intercept if a menu is already open (prevents loops)
                if (Game1.activeClickableMenu != null)
                    return true; // Use original behavior

                // Don't intercept if dialogue is up
                if (Game1.dialogueUp)
                    return true; // Use original behavior

                // Show our custom dialogue menu
                PetDialogueHandler.ShowPetMenu(__instance, who, l);
                __result = true;
                return false; // Skip original method
            }
            catch (Exception ex)
            {
                Monitor.Log($"Error in Pet.checkAction patch: {ex}", LogLevel.Error);
                return true; // Fall back to original on error
            }
        }
    }
}
