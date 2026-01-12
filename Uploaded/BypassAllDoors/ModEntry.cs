using System;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using xTile.Dimensions;

namespace BypassAllDoors
{
    /// <summary>
    /// Bypass ALL Doors Mod - Bypasses ALL door restrictions including:
    /// - Friendship-locked doors (NPC bedrooms)
    /// - Time-locked doors (shop hours)
    /// - Festival-locked doors
    /// - Conditional doors
    /// Each bypass type can be toggled via config/GMCM.
    /// </summary>
    public class ModEntry : Mod
    {
        public static IMonitor ModMonitor = null!;
        public static ModConfig Config = null!;
        public static IModHelper ModHelper = null!;

        public override void Entry(IModHelper helper)
        {
            ModMonitor = this.Monitor;
            ModHelper = helper;
            Config = helper.ReadConfig<ModConfig>();

            var harmony = new Harmony(this.ModManifest.UniqueID);
            harmony.PatchAll();

            // Register GMCM on game launch
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;

            ModMonitor.Log("[BypassAllDoors] Loaded - Door restrictions can be bypassed based on config!", LogLevel.Info);
        }

        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            // Get Generic Mod Config Menu's API (if it's installed)
            var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
            {
                this.Monitor.Log("Generic Mod Config Menu not found. Config can only be edited via config.json", LogLevel.Info);
                return;
            }

            // Register mod
            configMenu.Register(
                mod: this.ModManifest,
                reset: () => Config = new ModConfig(),
                save: () => this.Helper.WriteConfig(Config),
                titleScreenOnly: true
            );

            // === Door Bypass Settings ===
            configMenu.AddSectionTitle(
                mod: this.ModManifest,
                text: () => "Door Bypass Settings"
            );

            configMenu.AddParagraph(
                mod: this.ModManifest,
                text: () => "Toggle which types of door restrictions to bypass. All are enabled by default."
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Bypass Friendship Doors",
                tooltip: () => "Bypass NPC bedroom doors that require 2+ hearts friendship (e.g., Maru's room, Sebastian's room).",
                getValue: () => Config.BypassFriendshipDoors,
                setValue: value => Config.BypassFriendshipDoors = value
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Bypass Time Restrictions",
                tooltip: () => "Bypass shop hours - enter buildings even when they're closed (e.g., Pierre's at night).",
                getValue: () => Config.BypassTimeRestrictions,
                setValue: value => Config.BypassTimeRestrictions = value
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Bypass Festival Closures",
                tooltip: () => "Bypass festival day closures - enter buildings during festivals when everything is normally closed.",
                getValue: () => Config.BypassFestivalClosures,
                setValue: value => Config.BypassFestivalClosures = value
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Bypass Special Closures",
                tooltip: () => "Bypass special closures like Pierre's Wednesday closure.",
                getValue: () => Config.BypassSpecialClosures,
                setValue: value => Config.BypassSpecialClosures = value
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Bypass Conditional Doors",
                tooltip: () => "Bypass conditional doors that use Game State Queries (GSQ) to determine access.",
                getValue: () => Config.BypassConditionalDoors,
                setValue: value => Config.BypassConditionalDoors = value
            );

            this.Monitor.Log("Generic Mod Config Menu integration loaded!", LogLevel.Debug);
        }
    }

    /// <summary>
    /// Patch performAction to bypass door types based on config
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
                if (string.Equals(actionType, "Door", StringComparison.OrdinalIgnoreCase) && action.Length > 1 && !Game1.eventUp)
                {
                    if (!ModEntry.Config.BypassFriendshipDoors)
                        return true; // Let vanilla handle it

                    ModEntry.ModMonitor.Log($"[BypassAllDoors] Bypassing friendship Door: {string.Join(" ", action)}", LogLevel.Debug);
                    
                    Rumble.rumble(0.1f, 100f);
                    
                    // Add the unlock mail flags so the game remembers we "unlocked" them
                    // THIS IS CRITICAL - without this, the game's secondary checks will block us!
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
                    return false; // Skip vanilla
                }

                // --- Case 2: "LockedDoorWarp" is handled by lockedDoorWarp patch ---

                // --- Case 3: "ConditionalDoor" - GSQ-based conditional doors ---
                if (string.Equals(actionType, "ConditionalDoor", StringComparison.OrdinalIgnoreCase))
                {
                    if (!ModEntry.Config.BypassConditionalDoors)
                        return true; // Let vanilla handle it

                    ModEntry.ModMonitor.Log($"[BypassAllDoors] Bypassing ConditionalDoor", LogLevel.Debug);
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
    /// Patch lockedDoorWarp to bypass time, friendship, and festival checks based on config
    /// </summary>
    [HarmonyPatch(typeof(GameLocation), nameof(GameLocation.lockedDoorWarp))]
    public static class GameLocationLockedDoorWarpPatch
    {
        public static bool Prefix(GameLocation __instance, Point tile, string locationName, int openTime, int closeTime, string npcName, int minFriendship)
        {
            try
            {
                // Check if we should bypass anything at all
                bool bypassFestival = ModEntry.Config.BypassFestivalClosures;
                bool bypassSpecial = ModEntry.Config.BypassSpecialClosures;
                bool bypassTime = ModEntry.Config.BypassTimeRestrictions;
                bool bypassFriendship = ModEntry.Config.BypassFriendshipDoors;

                // If all bypasses are disabled, let vanilla handle it
                if (!bypassFestival && !bypassSpecial && !bypassTime && !bypassFriendship)
                    return true;

                // Festival closure check
                if (GameLocation.AreStoresClosedForFestival() && __instance.InValleyContext())
                {
                    if (!bypassFestival)
                    {
                        Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\Locations:FestivalDay_DoorLocked")));
                        return false;
                    }
                    ModEntry.ModMonitor.Log($"[BypassAllDoors] Bypassing festival closure for {locationName}", LogLevel.Debug);
                }

                // Special closure check (Pierre's Wednesday)
                bool town_key_applies = Game1.player.HasTownKey;
                if (locationName == "SeedShop" && Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth).Equals("Wed") && !Utility.HasAnyPlayerSeenEvent("191393") && !town_key_applies)
                {
                    if (!bypassSpecial)
                    {
                        Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\Locations:SeedShop_LockedWed")));
                        return false;
                    }
                    ModEntry.ModMonitor.Log($"[BypassAllDoors] Bypassing Pierre's Wednesday closure", LogLevel.Debug);
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
                        town_key_applies = false;
                    if (__instance is BeachNightMarket && locationName != "FishShop")
                        town_key_applies = false;
                }

                // Time check
                bool timeIsValid = town_key_applies || (Game1.timeOfDay >= actualOpenTime && Game1.timeOfDay < closeTime);

                // Green rain exception (same as vanilla)
                if (__instance.IsGreenRainingHere() && Game1.year == 1 && !(__instance is Beach) && !(__instance is Forest) && !locationName.Equals("AdventureGuild"))
                {
                    timeIsValid = true;
                }

                // Bypass time if configured
                if (!timeIsValid && bypassTime)
                {
                    ModEntry.ModMonitor.Log($"[BypassAllDoors] Bypassing time restriction for {locationName} (hours: {openTime}-{closeTime})", LogLevel.Debug);
                    timeIsValid = true;
                }

                // Friendship check - bypass if configured
                bool friendshipValid = minFriendship <= 0 || __instance.IsWinterHere();
                if (!friendshipValid)
                {
                    Friendship friendship;
                    friendshipValid = Game1.player.friendshipData.TryGetValue(npcName, out friendship) && friendship.Points >= minFriendship;
                }

                if (!friendshipValid && bypassFriendship)
                {
                    ModEntry.ModMonitor.Log($"[BypassAllDoors] Bypassing friendship requirement ({minFriendship} points with {npcName}) for {locationName}", LogLevel.Debug);
                    friendshipValid = true;
                }

                // Final check - can we open the door?
                if (timeIsValid && friendshipValid)
                {
                    Rumble.rumble(0.15f, 200f);
                    Game1.player.completelyStopAnimatingOrDoingAction();
                    __instance.playSound("doorClose", Game1.player.Tile);
                    Game1.warpFarmer(locationName, tile.X, tile.Y, flip: false);
                }
                else if (!timeIsValid)
                {
                    // Show time restriction message
                    string openTimeString = Game1.getTimeOfDayString(actualOpenTime).Replace(" ", "");
                    string closeTimeString = Game1.getTimeOfDayString(closeTime).Replace(" ", "");
                    Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:LockedDoor_OpenRange", openTimeString, closeTimeString));
                }
                else
                {
                    // Show friendship message
                    NPC character = Game1.getCharacterFromName(npcName);
                    if (character != null)
                        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:LockedDoor_FriendsOnly", character.displayName));
                    else
                        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:LockedDoor"));
                }

                return false; // We handled it completely
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
                if (!ModEntry.Config.BypassFriendshipDoors)
                    return true; // Let vanilla handle it

                string actionValue = __instance.doesTileHaveProperty(tileLocation.X, tileLocation.Y, "Action", "Buildings");
                if (string.IsNullOrEmpty(actionValue)) return true;

                string[] parts = actionValue.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 0) return true;

                string actionType = parts[0];

                // Handle "Door NpcName" type (walk-into doors)
                if (string.Equals(actionType, "Door", StringComparison.OrdinalIgnoreCase) && parts.Length > 1)
                {
                    ModEntry.ModMonitor.Log($"[BypassAllDoors] checkAction: Bypassing friendship for Door at ({tileLocation.X},{tileLocation.Y})", LogLevel.Debug);

                    // Add unlock mail flags - THIS IS CRITICAL!
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
                ModEntry.ModMonitor.Log($"[BypassAllDoors] Error in checkAction: {ex.Message}", LogLevel.Error);
            }

            return true;
        }
    }
}
