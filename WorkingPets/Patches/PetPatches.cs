using System;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Characters;
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

            Monitor.Log("Pet patches applied successfully.", LogLevel.Debug);
        }

        /// <summary>Intercept pet interaction to show custom dialogue.</summary>
        public static bool CheckAction_Prefix(Pet __instance, Farmer who, GameLocation l, ref bool __result)
        {
            try
            {
                if (!ModEntry.Config.ModEnabled)
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
