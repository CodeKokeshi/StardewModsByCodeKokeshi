using System;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using xTile.Dimensions;

namespace BypassFriendshipDoors
{
    /// <summary>
    /// Bypass Friendship Doors Mod - ONLY bypasses friendship requirements
    /// - Does NOT bypass shop hours (time restrictions still apply)
    /// - Does NOT bypass festival closures
    /// - Only affects NPC bedroom doors that require 2+ hearts
    /// </summary>
    public class ModEntry : Mod
    {
        public static IMonitor ModMonitor;

        public override void Entry(IModHelper helper)
        {
            ModMonitor = this.Monitor;
            var harmony = new Harmony(this.ModManifest.UniqueID);
            harmony.PatchAll();
            ModMonitor.Log("[BypassFriendshipDoors] Loaded - Friendship door requirements bypassed (time restrictions still apply)", LogLevel.Info);
        }
    }

    /// <summary>
    /// Patch the "Door" action in performAction - these are pure friendship doors (NPC bedrooms)
    /// Format: "Door NpcName [NpcName2]"
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

                // Only intercept "Door NpcName" - these are PURE friendship doors
                // They have NO time component, just require 2 hearts with an NPC
                if (string.Equals(actionType, "Door", StringComparison.OrdinalIgnoreCase) && action.Length > 1 && !Game1.eventUp)
                {
                    string npcNames = string.Join(", ", action, 1, action.Length - 1);
                    ModEntry.ModMonitor.Log($"[BypassFriendshipDoors] Bypassing friendship check for Door: {npcNames}", LogLevel.Debug);

                    // Open door without friendship check
                    Rumble.rumble(0.1f, 100f);
                    
                    // Add the unlock mail flags so the game remembers we "unlocked" them
                    for (int i = 1; i < action.Length; i++)
                    {
                        string mailKey = "doorUnlock" + action[i];
                        if (!Game1.player.mailReceived.Contains(mailKey))
                        {
                            Game1.player.mailReceived.Add(mailKey);
                        }
                    }

                    __instance.openDoor(tileLocation, playSound: true);
                    __result = true;
                    return false; // Skip vanilla friendship check
                }

                // NOTE: We do NOT intercept "LockedDoorWarp" here!
                // LockedDoorWarp handles both time AND friendship, and we want time to still apply.
                // We'll patch lockedDoorWarp separately to only bypass the friendship part.
            }
            catch (Exception ex)
            {
                ModEntry.ModMonitor.Log($"[BypassFriendshipDoors] Error in performAction: {ex.Message}", LogLevel.Error);
            }

            return true; // Let vanilla handle everything else
        }
    }

    /// <summary>
    /// Patch lockedDoorWarp to bypass ONLY the friendship requirement
    /// Time restrictions (openTime/closeTime) still apply!
    /// Festival closures still apply!
    /// 
    /// Original logic: canOpenDoor = (townKey OR timeInRange) AND (noFriendshipReq OR winterHere OR hasFriendship)
    /// Our change: canOpenDoor = (townKey OR timeInRange) AND TRUE  (friendship always passes)
    /// </summary>
    [HarmonyPatch(typeof(GameLocation), nameof(GameLocation.lockedDoorWarp))]
    public static class GameLocationLockedDoorWarpPatch
    {
        public static bool Prefix(GameLocation __instance, Point tile, string locationName, int openTime, int closeTime, string npcName, int minFriendship)
        {
            try
            {
                // Check if stores are closed for festival (we respect this!)
                if (GameLocation.AreStoresClosedForFestival() && __instance.InValleyContext())
                {
                    Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\Locations:FestivalDay_DoorLocked")));
                    return false;
                }

                // Check special SeedShop Wednesday closure (we respect this!)
                bool town_key_applies = Game1.player.HasTownKey;
                if (locationName == "SeedShop" && Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth).Equals("Wed") && !Utility.HasAnyPlayerSeenEvent("191393") && !town_key_applies)
                {
                    Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\Locations:SeedShop_LockedWed")));
                    return false;
                }

                // Adjust Willy's hours if player has the upgrade
                int actualOpenTime = openTime;
                if (locationName == "FishShop" && Game1.player.mailReceived.Contains("willyHours"))
                {
                    actualOpenTime = 800;
                }

                // Town key logic (same as vanilla)
                if (town_key_applies)
                {
                    if (!__instance.InValleyContext())
                    {
                        town_key_applies = false;
                    }
                    if (__instance is BeachNightMarket && locationName != "FishShop")
                    {
                        town_key_applies = false;
                    }
                }

                // TIME CHECK - We still respect time restrictions!
                bool timeIsValid = town_key_applies || (Game1.timeOfDay >= actualOpenTime && Game1.timeOfDay < closeTime);

                // Green rain exception (same as vanilla)
                if (__instance.IsGreenRainingHere() && Game1.year == 1 && !(__instance is Beach) && !(__instance is Forest) && !locationName.Equals("AdventureGuild"))
                {
                    timeIsValid = true;
                }

                // FRIENDSHIP CHECK - We BYPASS this! (always true)
                // Original: (minFriendship <= 0 || IsWinterHere() || hasFriendship)
                // Ours: always true

                if (timeIsValid)
                {
                    // Door opens!
                    if (minFriendship > 0)
                    {
                        ModEntry.ModMonitor.Log($"[BypassFriendshipDoors] Bypassing friendship requirement ({minFriendship} points) for {locationName}", LogLevel.Debug);
                    }
                    
                    Rumble.rumble(0.15f, 200f);
                    Game1.player.completelyStopAnimatingOrDoingAction();
                    __instance.playSound("doorClose", Game1.player.Tile);
                    Game1.warpFarmer(locationName, tile.X, tile.Y, flip: false);
                }
                else
                {
                    // Time restriction - show the "opens at X" message
                    string openTimeString = Game1.getTimeOfDayString(actualOpenTime).Replace(" ", "");
                    string closeTimeString = Game1.getTimeOfDayString(closeTime).Replace(" ", "");
                    Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:LockedDoor_OpenRange", openTimeString, closeTimeString));
                }

                return false; // We handled it completely
            }
            catch (Exception ex)
            {
                ModEntry.ModMonitor.Log($"[BypassFriendshipDoors] Error in lockedDoorWarp: {ex.Message}", LogLevel.Error);
                return true; // Fallback to vanilla on error
            }
        }
    }

    /// <summary>
    /// Patch checkAction (TouchAction handler) for walk-into doors
    /// Only handles "Door NpcName" type doors
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

                // Only handle pure "Door NpcName" friendship doors
                if (string.Equals(actionType, "Door", StringComparison.OrdinalIgnoreCase) && parts.Length > 1)
                {
                    string npcNames = string.Join(", ", parts, 1, parts.Length - 1);
                    ModEntry.ModMonitor.Log($"[BypassFriendshipDoors] checkAction: Bypassing friendship for Door ({npcNames})", LogLevel.Debug);

                    // Add unlock mail flags
                    for (int i = 1; i < parts.Length; i++)
                    {
                        string mailKey = "doorUnlock" + parts[i];
                        if (!Game1.player.mailReceived.Contains(mailKey))
                        {
                            Game1.player.mailReceived.Add(mailKey);
                        }
                    }

                    __instance.openDoor(tileLocation, true);
                    __result = true;
                    return false;
                }
            }
            catch (Exception ex)
            {
                ModEntry.ModMonitor.Log($"[BypassFriendshipDoors] Error in checkAction: {ex.Message}", LogLevel.Error);
            }

            return true;
        }
    }
}
