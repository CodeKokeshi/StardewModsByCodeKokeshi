using System;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using xTile.Dimensions;

namespace BypassFriendshipLockedDoors
{
    public class ModEntry : Mod
    {
        public static IMonitor ModMonitor;

        public override void Entry(IModHelper helper)
        {
            ModMonitor = this.Monitor;
            var harmony = new Harmony(this.ModManifest.UniqueID);
            harmony.PatchAll();
        }
    }

    // Patch to bypass friendship checks in event preconditions
    [HarmonyPatch(typeof(GameLocation), nameof(GameLocation.checkEventPrecondition), new Type[] { typeof(string) })]
    public static class GameLocationCheckEventPreconditionPatch
    {
        public static bool Prefix(GameLocation __instance, string precondition, ref string __result)
        {
            try
            {
                // If the precondition is checking friendship level (contains 'f' command)
                // we'll bypass it by always returning success
                if (!string.IsNullOrEmpty(precondition) && precondition.Contains("/f "))
                {
                    ModEntry.ModMonitor.Log($"[BypassDoors] Bypassing friendship precondition: {precondition}", LogLevel.Debug);
                    __result = "-1"; // Return string "-1" which means precondition passed
                    return false;     // Skip original method
                }
            }
            catch (Exception ex)
            {
                ModEntry.ModMonitor.Log($"[BypassDoors] Error in checkEventPrecondition: {ex.Message}", LogLevel.Error);
            }

            return true; // Run original method for non-friendship checks
        }
    }

    // Patch to completely bypass the locked door message
    [HarmonyPatch(typeof(GameLocation), nameof(GameLocation.ShowLockedDoorMessage))]
    public static class GameLocationShowLockedDoorMessagePatch
    {
        public static bool Prefix(GameLocation __instance, string[] action)
        {
            ModEntry.ModMonitor.Log($"[BypassDoors] Blocking ShowLockedDoorMessage for: {string.Join(" ", action)}", LogLevel.Debug);
            return false; // Skip showing the message entirely
        }
    }

    [HarmonyPatch(typeof(GameLocation), nameof(GameLocation.checkAction))]
    public static class GameLocationCheckActionPatch
    {
        public static bool Prefix(GameLocation __instance, Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who, ref bool __result)
        {
            try
            {
                // Retrieve "Action" property
                // Game usually checks "Buildings" layer
                string actionValue = __instance.doesTileHaveProperty(tileLocation.X, tileLocation.Y, "Action", "Buildings");

                // Also check if text notification action triggers are present
                if (string.IsNullOrEmpty(actionValue)) return true;

                string[] parts = actionValue.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 0) return true;

                string actionType = parts[0];

                // --- Case 1: Standard NPC Bedroom Door (e.g., "Door Maru") ---
                if (string.Equals(actionType, "Door", StringComparison.OrdinalIgnoreCase))
                {
                    ModEntry.ModMonitor.Log($"[BypassDoors] Intercepting Door: '{actionValue}' at {tileLocation.X},{tileLocation.Y}", LogLevel.Debug);

                    // 1. Force Open Animation (visuals + sound)
                    // This swaps the tiles to the "Open" state, making them passable.
                    __instance.openDoor(tileLocation, true);
                    
                    // 2. Do NOT try to warp manually.
                    // Standard doors work by making the tile passable. The warp happens when the player
                    // walks onto the "Warp" tile (which is usually behind the door).
                    // By returning false, we suppress the vanilla friendship check.
                    // The door is now open, and the player can walk through.
                    
                    __result = true; // Mark as handled (success)
                    return false;    // Suppress vanilla logic (which contains the friendship check)
                }
                
                // --- Case 2: LockedDoorWarp ---
                else if (string.Equals(actionType, "LockedDoorWarp", StringComparison.OrdinalIgnoreCase))
                {
                    ModEntry.ModMonitor.Log($"[BypassDoors] Bypassing LockedDoorWarp: {actionValue}", LogLevel.Debug);
                    if (parts.Length >= 4 && int.TryParse(parts[1], out int x) && int.TryParse(parts[2], out int y))
                    {
                         string locationName = parts[3];
                         Game1.warpFarmer(locationName, x, y, false);
                         __result = true;
                         return false;
                    }
                }
            }
            catch (Exception ex)
            {
                ModEntry.ModMonitor.Log($"[BypassDoors] Error: {ex.Message}", LogLevel.Error);
            }

            return true; // Run original method if not handled
        }

        private static string GetWarpProperty(GameLocation location, int x, int y)
        {
            // Check 'Back' layer (standard)
            string warp = location.doesTileHaveProperty(x, y, "Warp", "Back");
            if (!string.IsNullOrEmpty(warp)) return warp;

            // Check 'Buildings' layer
            return location.doesTileHaveProperty(x, y, "Warp", "Buildings");
        }
    }
}
