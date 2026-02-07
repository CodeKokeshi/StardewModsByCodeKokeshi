#nullable enable
using System;
using System.Collections.Generic;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.GameData.Machines;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Network;
using StardewValley.Network.NetEvents;
using StardewValley.Quests;
using SObject = StardewValley.Object;

namespace PlayerCheats
{
    /// <summary>Harmony patches for world cheats.</summary>
    public static class WorldPatches
    {
        /*********
        ** Buildings & Construction
        *********/

        /// <summary>Intercept building construction to finish instantly.</summary>
        public static bool MarkUnderConstruction_Prefix(string builderName, Building building)
        {
            if (!ModEntry.Config.ModEnabled || !ModEntry.Config.InstantBuildConstruction)
                return true;

            try
            {
                // Skip on festival days (Robin doesn't work on festivals)
                if (Utility.isFestivalDay(Game1.dayOfMonth, Game1.season))
                {
                    ModEntry.ModMonitor.Log($"Festival day - allowing normal construction for {building.buildingType.Value}", LogLevel.Debug);
                    return true;
                }

                // Skip during Green Rain year 1
                if (Game1.isGreenRain && Game1.year == 1)
                {
                    ModEntry.ModMonitor.Log($"Green Rain year 1 - allowing normal construction for {building.buildingType.Value}", LogLevel.Debug);
                    return true;
                }

                // Skip if building is being moved
                if (building.isMoving)
                {
                    ModEntry.ModMonitor.Log($"Building is moving - skipping instant completion for {building.buildingType.Value}", LogLevel.Debug);
                    return true;
                }

                // Finish construction immediately
                building.FinishConstruction();
                ModEntry.ModMonitor.Log($"Instant completed {building.buildingType.Value} (intercepted MarkUnderConstruction) by {builderName}", LogLevel.Info);

                // Free the builder NPC
                FreeBuilder(builderName);

                // Release build lock if held
                if (Game1.player.team.buildLock.IsLocked())
                {
                    Game1.player.team.buildLock.ReleaseLock();
                }

                return false; // Skip original method
            }
            catch (Exception ex)
            {
                ModEntry.ModMonitor.Log($"Error in MarkUnderConstruction_Prefix: {ex}", LogLevel.Error);
                return true;
            }
        }

        /*********
        ** Free Building Costs
        *********/

        /// <summary>Skip consuming resources for building construction when free building is enabled.</summary>
        public static bool CarpenterMenu_ConsumeResources_Prefix()
        {
            if (!ModEntry.Config.ModEnabled || !ModEntry.Config.FreeBuildingConstruction)
                return true;

            ModEntry.ModMonitor.Log("Free building construction: skipping resource consumption.", LogLevel.Debug);
            return false; // Skip original method
        }

        /// <summary>Always report having enough resources when free building is enabled.</summary>
        public static void CarpenterMenu_DoesFarmerHaveEnoughResources_Postfix(ref bool __result)
        {
            if (!ModEntry.Config.ModEnabled || !ModEntry.Config.FreeBuildingConstruction)
                return;

            __result = true;
        }

        /*********
        ** Instant Machine Processing
        *********/

        /// <summary>Set machine output to ready instantly after OutputMachine runs.</summary>
        public static void Object_OutputMachine_Postfix(SObject __instance, ref bool __result, bool probe)
        {
            if (!ModEntry.Config.ModEnabled || !ModEntry.Config.InstantMachineProcessing)
                return;

            // Don't modify during probe checks (probe = checking if machine can accept item)
            if (probe)
                return;

            // Only act if the machine successfully produced output
            if (!__result)
                return;

            if (__instance.MinutesUntilReady > 0)
            {
                __instance.MinutesUntilReady = 0;
                __instance.readyForHarvest.Value = true;
                ModEntry.ModMonitor.Log($"Instant machine: {__instance.DisplayName} output ready immediately.", LogLevel.Trace);
            }
        }

        /*********
        ** Free Shopping
        *********/

        /// <summary>Make all shop purchases free by setting charge amount to 0.</summary>
        public static void ShopMenu_ChargePlayer_Prefix(ref int amount)
        {
            if (!ModEntry.Config.ModEnabled || !ModEntry.Config.FreeShopPurchases)
                return;

            amount = 0;
        }

        /// <summary>Report infinite currency when free shopping is enabled.</summary>
        public static void ShopMenu_GetPlayerCurrencyAmount_Postfix(ref int __result)
        {
            if (!ModEntry.Config.ModEnabled || !ModEntry.Config.FreeShopPurchases)
                return;

            __result = int.MaxValue;
        }

        /*********
        ** Free Crafting
        *********/

        /// <summary>Skip consuming ingredients when free crafting is enabled.</summary>
        public static bool CraftingRecipe_ConsumeIngredients_Prefix()
        {
            if (!ModEntry.Config.ModEnabled || !ModEntry.Config.FreeCrafting)
                return true;

            ModEntry.ModMonitor.Log("Free crafting: skipping ingredient consumption.", LogLevel.Trace);
            return false; // Skip original method
        }

        /// <summary>Always report having ingredients when free crafting is enabled.</summary>
        public static void CraftingRecipe_DoesHaveIngredients_Postfix(ref bool __result)
        {
            if (!ModEntry.Config.ModEnabled || !ModEntry.Config.FreeCrafting)
                return;

            __result = true;
        }

        /*********
        ** Free Geode Processing
        *********/

        /// <summary>Prefix to save money before geode crack charges the player.</summary>
        public static void GeodeMenu_StartGeodeCrack_Prefix(out int __state)
        {
            // Save current money before the method charges us
            __state = Game1.player.Money;
        }

        /// <summary>Postfix to restore money after geode processing if free geodes is enabled.</summary>
        public static void GeodeMenu_StartGeodeCrack_Postfix(int __state)
        {
            if (!ModEntry.Config.ModEnabled || !ModEntry.Config.FreeGeodeProcessing)
                return;

            Game1.player.Money = __state;
            ModEntry.ModMonitor.Log("Free geode processing: refunded 25g.", LogLevel.Trace);
        }

        /*********
        ** Helper Methods
        *********/

        /// <summary>Complete all buildings currently under construction on the farm.</summary>
        public static void CompleteAllBuildings()
        {
            Farm farm = Game1.getFarm();
            if (farm == null)
                return;

            // Skip on festival days
            if (Utility.isFestivalDay(Game1.dayOfMonth, Game1.season))
            {
                ModEntry.ModMonitor.Log("Festival day - skipping building completion.", LogLevel.Debug);
                return;
            }

            // Skip during Green Rain year 1
            if (Game1.isGreenRain && Game1.year == 1)
            {
                ModEntry.ModMonitor.Log("Green Rain year 1 - skipping building completion.", LogLevel.Debug);
                return;
            }

            bool foundConstruction = false;

            foreach (Building building in farm.buildings)
            {
                // Skip buildings being moved
                if (building.isMoving)
                {
                    ModEntry.ModMonitor.Log($"Building {building.buildingType.Value} is moving - skipping.", LogLevel.Debug);
                    continue;
                }

                // Check for both construction AND upgrades
                if (building.isUnderConstruction(ignoreUpgrades: false))
                {
                    building.FinishConstruction();
                    foundConstruction = true;
                    ModEntry.ModMonitor.Log($"Completed construction of {building.buildingType.Value}!", LogLevel.Info);
                }
            }

            if (foundConstruction)
            {
                if (Game1.player.team.buildLock.IsLocked())
                {
                    Game1.player.team.buildLock.ReleaseLock();
                }
                FreeBuilder("Robin");
            }
        }

        /// <summary>Complete all pending house upgrades instantly.</summary>
        public static void CompleteHouseUpgrades()
        {
            // Skip on festival days
            if (Utility.isFestivalDay(Game1.dayOfMonth, Game1.season))
                return;

            if (Game1.isGreenRain && Game1.year == 1)
                return;

            foreach (Farmer farmer in Game1.getAllFarmers())
            {
                if (farmer.daysUntilHouseUpgrade.Value > 0)
                {
                    FarmHouse homeOfFarmer = Utility.getHomeOfFarmer(farmer);
                    if (homeOfFarmer == null)
                        continue;

                    // Find the building that contains this farmhouse
                    Building? houseBuilding = null;
                    foreach (Building building in Game1.getFarm().buildings)
                    {
                        if (building.GetIndoors() == homeOfFarmer)
                        {
                            houseBuilding = building;
                            break;
                        }
                    }

                    if (houseBuilding != null && houseBuilding.daysUntilUpgrade.Value > 0)
                    {
                        // Use the building's upgrade system (proper way)
                        houseBuilding.FinishConstruction();
                        ModEntry.ModMonitor.Log($"Completed house upgrade for {farmer.Name} using building system!", LogLevel.Info);
                        FreeBuilder("Robin");
                    }
                    else
                    {
                        // Fallback: Manual upgrade for legacy saves
                        homeOfFarmer.moveObjectsForHouseUpgrade(farmer.HouseUpgradeLevel + 1);
                        farmer.HouseUpgradeLevel++;
                        farmer.daysUntilHouseUpgrade.Value = -1;
                        homeOfFarmer.setMapForUpgradeLevel(farmer.HouseUpgradeLevel);
                        Game1.stats.checkForBuildingUpgradeAchievements();
                        farmer.autoGenerateActiveDialogueEvent("houseUpgrade_" + farmer.HouseUpgradeLevel);

                        ModEntry.ModMonitor.Log($"Completed house upgrade for {farmer.Name} (fallback method) to level {farmer.HouseUpgradeLevel}!", LogLevel.Info);
                        FreeBuilder("Robin");
                    }
                }
            }
        }

        /// <summary>Complete community upgrades (Pam's house, shortcuts) instantly.</summary>
        public static void CompleteCommunityUpgrades()
        {
            Town? town = Game1.getLocationFromName("Town") as Town;
            if (town == null || town.daysUntilCommunityUpgrade.Value <= 0)
                return;

            // Skip on festival days
            if (Utility.isFestivalDay(Game1.dayOfMonth, Game1.season))
                return;

            if (Game1.isGreenRain && Game1.year == 1)
                return;

            // Determine which upgrade based on mail flags
            bool isPamHouseUpgrade = !Game1.MasterPlayer.mailReceived.Contains("pamHouseUpgrade");

            if (isPamHouseUpgrade)
            {
                Game1.player.team.RequestSetMail(PlayerActionTarget.Host, "pamHouseUpgrade", MailType.Received, add: true);

                NPC? pam = Game1.getCharacterFromName("Pam");
                if (pam != null)
                {
                    Game1.player.changeFriendship(1000, pam);
                }

                ModEntry.ModMonitor.Log("Completed Pam's house upgrade!", LogLevel.Info);
            }
            else
            {
                Game1.player.team.RequestSetMail(PlayerActionTarget.Host, "communityUpgradeShortcuts", MailType.Received, add: true);
                ModEntry.ModMonitor.Log("Completed shortcuts upgrade!", LogLevel.Info);
            }

            town.daysUntilCommunityUpgrade.Value = 0;
            FreeBuilder("Robin");
        }

        /// <summary>Complete any pending machines in all locations.</summary>
        public static void CompleteAllMachines()
        {
            foreach (GameLocation location in Game1.locations)
            {
                foreach (SObject obj in location.objects.Values)
                {
                    if (obj.MinutesUntilReady > 0 && obj.heldObject.Value != null)
                    {
                        obj.MinutesUntilReady = 0;
                        obj.readyForHarvest.Value = true;
                    }
                }

                // Also check buildings (e.g., barns, coops have indoor locations)
                foreach (Building building in location.buildings)
                {
                    GameLocation? indoors = building.GetIndoors();
                    if (indoors != null)
                    {
                        foreach (SObject obj in indoors.objects.Values)
                        {
                            if (obj.MinutesUntilReady > 0 && obj.heldObject.Value != null)
                            {
                                obj.MinutesUntilReady = 0;
                                obj.readyForHarvest.Value = true;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>Free a builder NPC from construction duty.</summary>
        public static void FreeBuilder(string builderName)
        {
            NPC? builder = Game1.getCharacterFromName(builderName);
            if (builder == null)
                return;

            builder.shouldPlayRobinHammerAnimation.Value = false;
            builder.ignoreScheduleToday = false;
            builder.controller = null;
            builder.Sprite.StopAnimation();
            builder.CurrentDialogue.Clear();
            builder.resetCurrentDialogue();

            if (builderName == "Robin")
            {
                GameLocation? scienceHouse = Game1.getLocationFromName("ScienceHouse");
                if (scienceHouse != null)
                {
                    Game1.warpCharacter(builder, "ScienceHouse", new Vector2(8, 18));
                    builder.checkSchedule(Game1.timeOfDay);
                    ModEntry.ModMonitor.Log($"Freed {builderName} and warped to ScienceHouse.", LogLevel.Debug);
                }
                else
                {
                    ModEntry.ModMonitor.Log($"Freed {builderName} (ScienceHouse not loaded, skipped warp).", LogLevel.Debug);
                }
            }
            else
            {
                ModEntry.ModMonitor.Log($"Freed builder {builderName}.", LogLevel.Debug);
            }
        }

        /// <summary>Apply weather override for tomorrow.</summary>
        public static void ApplyWeatherOverride()
        {
            string weatherChoice = ModEntry.Config.WeatherForTomorrow;

            if (string.IsNullOrEmpty(weatherChoice) || weatherChoice == "Default")
                return;

            // Map config value to game weather constant
            string gameWeather = weatherChoice switch
            {
                "Sun" => "Sun",
                "Rain" => "Rain",
                "Storm" => "Storm",
                "Snow" => "Snow",
                "Wind" => "Wind",
                _ => ""
            };

            if (string.IsNullOrEmpty(gameWeather))
                return;

            Game1.weatherForTomorrow = gameWeather;

            LocationWeather? locWeather = Game1.netWorldState.Value.GetWeatherForLocation("Default");
            if (locWeather != null)
            {
                locWeather.WeatherForTomorrow = gameWeather;
                ModEntry.ModMonitor.Log($"Weather for tomorrow set to: {gameWeather}", LogLevel.Info);
            }
        }

        /*********
        ** Mining
        *********/

        /// <summary>Force ladder spawn when breaking rocks in mines based on configured chance.</summary>
        public static void MineShaft_CheckStoneForItems_Postfix(MineShaft __instance, string stoneId, int x, int y, Farmer who)
        {
            if (!ModEntry.Config.ModEnabled || ModEntry.Config.ForceLadderChance <= 0)
                return;

            // Skip if ladder already spawned on this level
            if (__instance.ladderHasSpawned)
                return;

            // Skip if player must kill all monsters first
            if (__instance.mustKillAllMonstersToAdvance())
                return;

            // Check against configured chance
            int chance = ModEntry.Config.ForceLadderChance;
            if (chance >= 100 || Game1.random.Next(100) < chance)
            {
                // Create ladder at the stone's position
                __instance.createLadderDown(x, y);
                ModEntry.ModMonitor.Log($"[Mining] Forced ladder spawn at ({x}, {y}) on level {__instance.mineLevel}.", LogLevel.Trace);
            }
        }

        /*********
        ** Farming - Field Protection
        *********/

        /// <summary>Prevent weeds, stones, and twigs from spawning when enabled.</summary>
        public static bool GameLocation_SpawnWeedsAndStones_Prefix(GameLocation __instance)
        {
            if (!ModEntry.Config.ModEnabled || !ModEntry.Config.PreventDebrisSpawn)
                return true;

            // Skip spawning on farm and farm buildings
            if (__instance is Farm || __instance.GetParentLocation() is Farm)
            {
                return false;
            }

            return true;
        }

        /// <summary>Prevent tilled soil from decaying by returning 0% decay chance.</summary>
        public static void GameLocation_GetDirtDecayChance_Postfix(ref double __result)
        {
            if (!ModEntry.Config.ModEnabled || !ModEntry.Config.TilledSoilDontDecay)
                return;

            __result = 0.0;
        }

        /*********
        ** Animals & Pets
        *********/

        /// <summary>Track the animal being purchased before receiveLeftClick.</summary>
        public static void PurchaseAnimalsMenu_ReceiveLeftClick_Prefix(PurchaseAnimalsMenu __instance, ref FarmAnimal? __state)
        {
            __state = __instance.animalBeingPurchased;
        }

        /// <summary>Make purchased animals fully grown immediately.</summary>
        public static void PurchaseAnimalsMenu_ReceiveLeftClick_Postfix(PurchaseAnimalsMenu __instance, FarmAnimal? __state)
        {
            if (!ModEntry.Config.ModEnabled || !ModEntry.Config.BuyAnimalsFullyMatured)
                return;

            FarmAnimal? purchased = __instance.animalBeingPurchased;
            if (purchased == null)
                return;

            // Only apply when a new animal was just selected for purchase
            if (ReferenceEquals(purchased, __state))
                return;

            purchased.growFully();
            ModEntry.ModMonitor.Log($"[Animals] Made {purchased.Name} ({purchased.type.Value}) fully matured.", LogLevel.Trace);
        }

        /*********
        ** Bypass All Doors
        *********/

        /// <summary>Bypass door restrictions in performAction (friendship doors, conditional doors).</summary>
        public static bool GameLocation_PerformAction_Prefix(GameLocation __instance, string[] action, Farmer who, xTile.Dimensions.Location tileLocation, ref bool __result)
        {
            if (!ModEntry.Config.ModEnabled)
                return true;

            try
            {
                if (action == null || action.Length == 0) return true;

                string actionType = action[0];

                // Case 1: "Door NpcName" - Friendship-locked bedroom doors
                if (string.Equals(actionType, "Door", StringComparison.OrdinalIgnoreCase) && action.Length > 1 && !Game1.eventUp)
                {
                    if (!ModEntry.Config.BypassFriendshipDoors)
                        return true;

                    ModEntry.ModMonitor.Log($"[BypassAllDoors] Bypassing friendship Door: {string.Join(" ", action)}", LogLevel.Debug);
                    Rumble.rumble(0.1f, 100f);

                    for (int i = 1; i < action.Length; i++)
                    {
                        string mailKey = "doorUnlock" + action[i];
                        if (!Game1.player.mailReceived.Contains(mailKey))
                            Game1.player.mailReceived.Add(mailKey);
                    }

                    __instance.openDoor(tileLocation, playSound: true);
                    __result = true;
                    return false;
                }

                // Case 2: "ConditionalDoor" - GSQ-based conditional doors
                if (string.Equals(actionType, "ConditionalDoor", StringComparison.OrdinalIgnoreCase))
                {
                    if (!ModEntry.Config.BypassConditionalDoors)
                        return true;

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

            return true;
        }

        /// <summary>Bypass lockedDoorWarp restrictions (time, friendship, festival, special closures).</summary>
        public static bool GameLocation_LockedDoorWarp_Prefix(GameLocation __instance, Point tile, string locationName, int openTime, int closeTime, string npcName, int minFriendship)
        {
            if (!ModEntry.Config.ModEnabled)
                return true;

            try
            {
                bool bypassFestival = ModEntry.Config.BypassFestivalClosures;
                bool bypassSpecial = ModEntry.Config.BypassSpecialClosures;
                bool bypassTime = ModEntry.Config.BypassTimeRestrictions;
                bool bypassFriendship = ModEntry.Config.BypassFriendshipDoors;

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

                // Pierre's Wednesday special closure
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

                // Adjust Willy's hours
                int actualOpenTime = openTime;
                if (locationName == "FishShop" && Game1.player.mailReceived.Contains("willyHours"))
                    actualOpenTime = 800;

                // Town key logic
                if (town_key_applies)
                {
                    if (!__instance.InValleyContext())
                        town_key_applies = false;
                    if (__instance is BeachNightMarket && locationName != "FishShop")
                        town_key_applies = false;
                }

                // Time check
                bool timeIsValid = town_key_applies || (Game1.timeOfDay >= actualOpenTime && Game1.timeOfDay < closeTime);

                // Green rain exception
                if (__instance.IsGreenRainingHere() && Game1.year == 1 && !(__instance is Beach) && !(__instance is Forest) && !locationName.Equals("AdventureGuild"))
                    timeIsValid = true;

                if (!timeIsValid && bypassTime)
                {
                    ModEntry.ModMonitor.Log($"[BypassAllDoors] Bypassing time restriction for {locationName} (hours: {openTime}-{closeTime})", LogLevel.Debug);
                    timeIsValid = true;
                }

                // Friendship check
                bool friendshipValid = minFriendship <= 0 || __instance.IsWinterHere();
                if (!friendshipValid)
                {
                    Friendship friendship;
                    friendshipValid = Game1.player.friendshipData.TryGetValue(npcName, out friendship) && friendship.Points >= minFriendship;
                }

                if (!friendshipValid && bypassFriendship)
                {
                    ModEntry.ModMonitor.Log($"[BypassAllDoors] Bypassing friendship requirement for {locationName}", LogLevel.Debug);
                    friendshipValid = true;
                }

                if (timeIsValid && friendshipValid)
                {
                    Rumble.rumble(0.15f, 200f);
                    Game1.player.completelyStopAnimatingOrDoingAction();
                    __instance.playSound("doorClose", Game1.player.Tile);
                    Game1.warpFarmer(locationName, tile.X, tile.Y, flip: false);
                }
                else if (!timeIsValid)
                {
                    string openTimeString = Game1.getTimeOfDayString(actualOpenTime).Replace(" ", "");
                    string closeTimeString = Game1.getTimeOfDayString(closeTime).Replace(" ", "");
                    Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:LockedDoor_OpenRange", openTimeString, closeTimeString));
                }
                else
                {
                    NPC character = Game1.getCharacterFromName(npcName);
                    if (character != null)
                        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:LockedDoor_FriendsOnly", character.displayName));
                    else
                        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:LockedDoor"));
                }

                return false;
            }
            catch (Exception ex)
            {
                ModEntry.ModMonitor.Log($"[BypassAllDoors] Error in lockedDoorWarp: {ex.Message}", LogLevel.Error);
                return true;
            }
        }

        /// <summary>Bypass friendship doors via checkAction (walk-into doors).</summary>
        public static bool GameLocation_CheckAction_Prefix(GameLocation __instance, xTile.Dimensions.Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who, ref bool __result)
        {
            if (!ModEntry.Config.ModEnabled || !ModEntry.Config.BypassFriendshipDoors)
                return true;

            try
            {
                string actionValue = __instance.doesTileHaveProperty(tileLocation.X, tileLocation.Y, "Action", "Buildings");
                if (string.IsNullOrEmpty(actionValue)) return true;

                string[] parts = actionValue.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 0) return true;

                if (string.Equals(parts[0], "Door", StringComparison.OrdinalIgnoreCase) && parts.Length > 1)
                {
                    ModEntry.ModMonitor.Log($"[BypassAllDoors] checkAction: Bypassing friendship for Door at ({tileLocation.X},{tileLocation.Y})", LogLevel.Debug);

                    for (int i = 1; i < parts.Length; i++)
                    {
                        string mailKey = "doorUnlock" + parts[i];
                        if (!Game1.player.mailReceived.Contains(mailKey))
                            Game1.player.mailReceived.Add(mailKey);
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

        /*********
        ** Freeze Time in Mines
        *********/

        /// <summary>Freeze time when in mines/skull cavern/volcano.</summary>
        public static bool Game1_PerformTenMinuteClockUpdate_MinesPrefix()
        {
            if (!Context.IsWorldReady || !ModEntry.Config.ModEnabled)
                return true;

            if (ModEntry.Config.FreezeTimeMines && Game1.currentLocation is MineShaft)
            {
                return false;
            }

            // Also check for volcano dungeon
            if (ModEntry.Config.FreezeTimeMines && Game1.currentLocation is VolcanoDungeon)
            {
                return false;
            }

            return true;
        }
    }
}
