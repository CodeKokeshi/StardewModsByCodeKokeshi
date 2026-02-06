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
using SObject = StardewValley.Object;

namespace WorldCheats
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

        /// <summary>Prefix to restore money after geode crack charges the player.</summary>
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
    }
}
