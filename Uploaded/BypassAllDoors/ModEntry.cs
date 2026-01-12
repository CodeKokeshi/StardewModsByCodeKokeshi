using System;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using xTile.Dimensions;

namespace BypassAllDoors
{
    /// <summary>
    /// Bypass ALL Doors Mod - Bypasses ALL door restrictions including:
    /// - Friendship-locked doors (NPC bedrooms)
    /// - Time-locked doors (shop hours)
    /// - Festival-locked doors
    /// </summary>
    public class ModEntry : Mod
    {
        public static IMonitor ModMonitor;

        public override void Entry(IModHelper helper)
        {
            ModMonitor = this.Monitor;
            var harmony = new Harmony(this.ModManifest.UniqueID);
            harmony.PatchAll();
            ModMonitor.Log("[BypassAllDoors] Loaded - ALL door restrictions will be bypassed!", LogLevel.Info);
        }
    }

    /// <summary>
    /// Patch performAction to bypass ALL door types completely
    /// </summary>
    [HarmonyPatch(typeof(GameLocation), nameof(GameLocation.performAction), new Type[] { typeof(string[]), typeof(Farmer), typeof(Location) })]
    public static class GameLocationPerformActionPatch
    {
        public static bool Prefix(GameLocation __instance, string[] action, Farmer who, Location tileLocation, ref bool __result)
        {
            try
            {
                if (action == null || action.Length == 0) return true;

                string actionType = action[0];

                // --- Case 1: "Door NpcName" - Friendship-locked bedroom doors ---
                if (string.Equals(actionType, "Door", StringComparison.OrdinalIgnoreCase) && action.Length > 1)
                {
                    ModEntry.ModMonitor.Log($"[BypassAllDoors] Forcing open Door: {string.Join(" ", action)}", LogLevel.Debug);
                    
                    // Just open the door, no checks
                    Rumble.rumble(0.1f, 100f);
                    __instance.openDoor(tileLocation, playSound: true);
                    __result = true;
                    return false; // Skip vanilla (which has friendship check)
                }

                // --- Case 2: "LockedDoorWarp x y Location openTime closeTime [npcName] [minFriendship]" ---
                // Used for shops and houses with time + friendship requirements
                if (string.Equals(actionType, "LockedDoorWarp", StringComparison.OrdinalIgnoreCase))
                {
                    if (action.Length >= 4 &&
                        int.TryParse(action[1], out int x) &&
                        int.TryParse(action[2], out int y))
                    {
                        string locationName = action[3];
                        ModEntry.ModMonitor.Log($"[BypassAllDoors] Forcing LockedDoorWarp to {locationName} at ({x},{y})", LogLevel.Debug);

                        // Warp unconditionally - bypass time AND friendship
                        Rumble.rumble(0.15f, 200f);
                        who.completelyStopAnimatingOrDoingAction();
                        __instance.playSound("doorClose", who.Tile);
                        Game1.warpFarmer(locationName, x, y, flip: false);
                        __result = true;
                        return false;
                    }
                }

                // --- Case 3: "ConditionalDoor" - GSQ-based conditional doors ---
                if (string.Equals(actionType, "ConditionalDoor", StringComparison.OrdinalIgnoreCase))
                {
                    ModEntry.ModMonitor.Log($"[BypassAllDoors] Forcing open ConditionalDoor", LogLevel.Debug);
                    __instance.openDoor(tileLocation, playSound: true);
                    __result = true;
                    return false;
                }
            }
            catch (Exception ex)
            {
                ModEntry.ModMonitor.Log($"[BypassAllDoors] Error in performAction: {ex.Message}", LogLevel.Error);
            }

            return true; // Let vanilla handle everything else
        }
    }

    /// <summary>
    /// Patch lockedDoorWarp to bypass time and friendship checks
    /// This catches warps triggered by TouchAction tiles
    /// </summary>
    [HarmonyPatch(typeof(GameLocation), nameof(GameLocation.lockedDoorWarp))]
    public static class GameLocationLockedDoorWarpPatch
    {
        public static bool Prefix(GameLocation __instance, Point tile, string locationName, int openTime, int closeTime, string npcName, int minFriendship)
        {
            try
            {
                ModEntry.ModMonitor.Log($"[BypassAllDoors] Forcing lockedDoorWarp to {locationName} (ignoring time {openTime}-{closeTime}, friendship {minFriendship})", LogLevel.Debug);

                // Warp unconditionally
                Rumble.rumble(0.15f, 200f);
                Game1.player.completelyStopAnimatingOrDoingAction();
                __instance.playSound("doorClose", Game1.player.Tile);
                Game1.warpFarmer(locationName, tile.X, tile.Y, flip: false);
                
                return false; // Skip vanilla entirely
            }
            catch (Exception ex)
            {
                ModEntry.ModMonitor.Log($"[BypassAllDoors] Error in lockedDoorWarp: {ex.Message}", LogLevel.Error);
                return true; // Fallback to vanilla on error
            }
        }
    }

    /// <summary>
    /// Patch checkAction (TouchAction handler) for doors triggered by walking
    /// </summary>
    [HarmonyPatch(typeof(GameLocation), nameof(GameLocation.checkAction))]
    public static class GameLocationCheckActionPatch
    {
        public static bool Prefix(GameLocation __instance, Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who, ref bool __result)
        {
            try
            {
                string actionValue = __instance.doesTileHaveProperty(tileLocation.X, tileLocation.Y, "Action", "Buildings");
                if (string.IsNullOrEmpty(actionValue)) return true;

                string[] parts = actionValue.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 0) return true;

                string actionType = parts[0];

                // Handle "Door NpcName" type (walk-into doors)
                if (string.Equals(actionType, "Door", StringComparison.OrdinalIgnoreCase) && parts.Length > 1)
                {
                    ModEntry.ModMonitor.Log($"[BypassAllDoors] checkAction: Forcing open Door at ({tileLocation.X},{tileLocation.Y})", LogLevel.Debug);
                    __instance.openDoor(tileLocation, true);
                    __result = true;
                    return false;
                }
            }
            catch (Exception ex)
            {
                ModEntry.ModMonitor.Log($"[BypassAllDoors] Error in checkAction: {ex.Message}", LogLevel.Error);
            }

            return true;
        }
    }
}
