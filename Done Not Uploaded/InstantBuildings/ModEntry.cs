using System;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using HarmonyLib;

namespace InstantBuildings
{
    public class ModEntry : Mod
    {
        private static IMonitor SMonitor;

        public override void Entry(IModHelper helper)
        {
            SMonitor = this.Monitor;

            // Initialize Harmony
            var harmony = new Harmony(this.ModManifest.UniqueID);
            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.Network.NetWorldState), nameof(StardewValley.Network.NetWorldState.MarkUnderConstruction)),
                prefix: new HarmonyMethod(typeof(ModEntry), nameof(MarkUnderConstructionPrefix))
            );

            helper.Events.GameLoop.DayStarted += OnDayStarted;
            helper.Events.World.BuildingListChanged += OnBuildingListChanged;
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
        }

        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            // Complete house upgrades and community upgrades on save load
            CompleteHouseUpgrades();
        }

        private static bool MarkUnderConstructionPrefix(string builderName, Building building)
        {
            try
            {
                // Finish construction immediately
                building.FinishConstruction();
                SMonitor?.Log($"Instant completed {building.buildingType.Value} (intercepted MarkUnderConstruction) by {builderName}", LogLevel.Info);

                // Free the builder
                FreeBuilder(builderName);

                // Release build lock if held
                if (Game1.player.team.buildLock.IsLocked())
                {
                    Game1.player.team.buildLock.ReleaseLock();
                }

                // Return false to skip the original method
                // This prevents the building from being added to the 'builders' list (under construction queue)
                return false; 
            }
            catch (Exception ex)
            {
                SMonitor?.Log($"Error in MarkUnderConstructionPrefix: {ex}", LogLevel.Error);
                return true; // Use original logic on error
            }
        }

        /*
        private static void AfterMarkUnderConstruction()
        {
             // Deprecated in favor of Prefix
        }
        */

        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            // Complete any buildings currently under construction
            CompleteAllBuildingsStatic();
            
            // Complete house and community upgrades
            CompleteHouseUpgrades();
        }

        private void OnBuildingListChanged(object sender, BuildingListChangedEventArgs e)
        {
            // Only trigger when buildings are added
            if (e.Added.Any())
            {
                CompleteAllBuildingsStatic();
            }
        }

        private static void CompleteAllBuildingsStatic()
        {
            Farm farm = Game1.getFarm();
            if (farm == null)
                return;

            bool foundConstruction = false;

            foreach (Building building in farm.buildings)
            {
                // Check for both construction AND upgrades
                if (building.isUnderConstruction(ignoreUpgrades: false))
                {
                    // Fix: Do not set days to 0 manually. FinishConstruction needs them > 0 to work.
                    building.FinishConstruction();
                    foundConstruction = true;
                    SMonitor?.Log($"Completed construction of {building.buildingType.Value}!", LogLevel.Info);
                }
            }

            // Clear Robin's task and send her home
            if (foundConstruction)
            {
                // Release the build lock if it's locked
                if (Game1.player.team.buildLock.IsLocked())
                {
                    Game1.player.team.buildLock.ReleaseLock();
                }
                
                FreeBuilder("Robin");
            }
        }

        private static void CompleteHouseUpgrades()
        {
            // Complete house upgrades for all players
            foreach (Farmer farmer in Game1.getAllFarmers())
            {
                if (farmer.daysUntilHouseUpgrade.Value > 0)
                {
                    // Complete the house upgrade instantly
                    FarmHouse homeOfFarmer = Utility.getHomeOfFarmer(farmer);
                    if (homeOfFarmer != null)
                    {
                        homeOfFarmer.moveObjectsForHouseUpgrade(farmer.HouseUpgradeLevel + 1);
                        farmer.HouseUpgradeLevel++;
                        farmer.daysUntilHouseUpgrade.Value = -1;
                        homeOfFarmer.setMapForUpgradeLevel(farmer.HouseUpgradeLevel);
                        Game1.stats.checkForBuildingUpgradeAchievements();
                        farmer.autoGenerateActiveDialogueEvent("houseUpgrade_" + farmer.HouseUpgradeLevel);
                        
                        SMonitor?.Log($"Completed house upgrade for {farmer.Name} to level {farmer.HouseUpgradeLevel}!", LogLevel.Info);
                        
                        // Free Robin after completing the upgrade
                        FreeBuilder("Robin");
                    }
                }
            }
            
            // Complete community upgrades
            Town town = Game1.getLocationFromName("Town") as Town;
            if (town != null && town.daysUntilCommunityUpgrade.Value > 0)
            {
                // Complete community upgrade instantly
                if (town.daysUntilCommunityUpgrade.Value == 3 && !Game1.MasterPlayer.mailReceived.Contains("pamHouseUpgrade"))
                {
                    Game1.MasterPlayer.mailReceived.Add("pamHouseUpgrade");
                    Game1.MasterPlayer.mailReceived.Add("pamHouseUpgradeAnonymous");
                }
                else if (town.daysUntilCommunityUpgrade.Value == 3 && !Game1.MasterPlayer.mailReceived.Contains("communityUpgradeShortcuts"))
                {
                    Game1.MasterPlayer.mailReceived.Add("communityUpgradeShortcuts");
                }
                
                town.daysUntilCommunityUpgrade.Value = 0;
                SMonitor?.Log($"Completed community upgrade!", LogLevel.Info);
                
                // Free Robin after completing the upgrade
                FreeBuilder("Robin");
            }
        }

        private static void FreeBuilder(string builderName)
        {
            NPC builder = Game1.getCharacterFromName(builderName);
            if (builder != null)
            {
                // Stop the hammer animation flag
                builder.shouldPlayRobinHammerAnimation.Value = false;
                
                // Reset her AI and schedule flags
                builder.ignoreScheduleToday = false;
                builder.controller = null;
                builder.Sprite.StopAnimation();
                
                // Clear her construction dialogue
                builder.CurrentDialogue.Clear();
                builder.resetCurrentDialogue();
                
                // Warp her home and make her follow her schedule
                // Only do this for Robin, as she has a shop we need to access.
                // Others (like Wizard) might be fine staying where they are.
                if (builderName == "Robin")
                {
                    Game1.warpCharacter(builder, "ScienceHouse", new Vector2(8, 18));
                    builder.checkSchedule(Game1.timeOfDay);
                }
                
                SMonitor?.Log($"Freed builder {builderName}, cleared dialogue, and reset schedule!", LogLevel.Debug);
            }
        }
    }
}
