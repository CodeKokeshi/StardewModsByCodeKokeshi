using System;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Network.NetEvents;
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
            
            // Patch MarkUnderConstruction to intercept building construction
            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.Network.NetWorldState), nameof(StardewValley.Network.NetWorldState.MarkUnderConstruction)),
                prefix: new HarmonyMethod(typeof(ModEntry), nameof(MarkUnderConstructionPrefix))
            );

            helper.Events.GameLoop.DayStarted += OnDayStarted;
            helper.Events.World.BuildingListChanged += OnBuildingListChanged;
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
        }

        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            // Check every quarter second (15 ticks) for house/community upgrades
            if (!e.IsMultipleOf(15))
                return;

            // Complete house upgrades and community upgrades immediately after purchase
            CompleteHouseUpgrades();
            CompleteCommunityUpgrades();
        }

        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            // Complete any pending upgrades on save load
            CompleteHouseUpgrades();
            CompleteCommunityUpgrades();
        }

        private static bool MarkUnderConstructionPrefix(string builderName, Building building)
        {
            try
            {
                // EDGE CASE 1: Check for festival days (Robin doesn't work on festivals)
                if (Utility.isFestivalDay(Game1.dayOfMonth, Game1.season))
                {
                    SMonitor?.Log($"Festival day detected - allowing normal construction timing for {building.buildingType.Value}", LogLevel.Debug);
                    return true; // Use original logic on festival days
                }

                // EDGE CASE 2: Check for Green Rain in year 1 (Robin doesn't work during Green Rain year 1)
                if (Game1.isGreenRain && Game1.year == 1)
                {
                    SMonitor?.Log($"Green Rain year 1 detected - allowing normal construction timing for {building.buildingType.Value}", LogLevel.Debug);
                    return true; // Use original logic during Green Rain year 1
                }

                // EDGE CASE 3: Check if building is being moved (don't complete during moves)
                if (building.isMoving)
                {
                    SMonitor?.Log($"Building is moving - allowing normal construction timing for {building.buildingType.Value}", LogLevel.Debug);
                    return true; // Use original logic when building is being moved
                }

                // Finish construction immediately
                building.FinishConstruction();
                SMonitor?.Log($"Instant completed {building.buildingType.Value} (intercepted MarkUnderConstruction) by {builderName}", LogLevel.Info);

                // Free the builder NPC
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
            CompleteCommunityUpgrades();
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

            // EDGE CASE 1 & 2: Check for festival days and Green Rain year 1
            if (Utility.isFestivalDay(Game1.dayOfMonth, Game1.season))
            {
                SMonitor?.Log("Festival day - skipping building completion", LogLevel.Debug);
                return;
            }

            if (Game1.isGreenRain && Game1.year == 1)
            {
                SMonitor?.Log("Green Rain year 1 - skipping building completion", LogLevel.Debug);
                return;
            }

            bool foundConstruction = false;

            foreach (Building building in farm.buildings)
            {
                // EDGE CASE 3: Check if building is being moved
                if (building.isMoving)
                {
                    SMonitor?.Log($"Building {building.buildingType.Value} is moving - skipping completion", LogLevel.Debug);
                    continue;
                }

                // Check for both construction AND upgrades
                if (building.isUnderConstruction(ignoreUpgrades: false))
                {
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
            // EDGE CASE 1 & 2: Check for festival days and Green Rain year 1
            if (Utility.isFestivalDay(Game1.dayOfMonth, Game1.season))
            {
                SMonitor?.Log("Festival day - skipping house upgrade completion", LogLevel.Debug);
                return;
            }

            if (Game1.isGreenRain && Game1.year == 1)
            {
                SMonitor?.Log("Green Rain year 1 - skipping house upgrade completion", LogLevel.Debug);
                return;
            }

            // EDGE CASE 4: Use proper building upgrade system instead of manual manipulation
            // Complete house upgrades for all players by finding their house building
            foreach (Farmer farmer in Game1.getAllFarmers())
            {
                if (farmer.daysUntilHouseUpgrade.Value > 0)
                {
                    FarmHouse homeOfFarmer = Utility.getHomeOfFarmer(farmer);
                    if (homeOfFarmer == null)
                        continue;

                    // Find the building that contains this farmhouse
                    Building houseBuilding = null;
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
                        SMonitor?.Log($"Completed house upgrade for {farmer.Name} using building system!", LogLevel.Info);
                        
                        // Free Robin after completing the upgrade
                        FreeBuilder("Robin");
                    }
                    else
                    {
                        // Fallback: Manual upgrade if building system doesn't work (legacy saves?)
                        // This is the old approach but kept as fallback
                        homeOfFarmer.moveObjectsForHouseUpgrade(farmer.HouseUpgradeLevel + 1);
                        farmer.HouseUpgradeLevel++;
                        farmer.daysUntilHouseUpgrade.Value = -1;
                        homeOfFarmer.setMapForUpgradeLevel(farmer.HouseUpgradeLevel);
                        Game1.stats.checkForBuildingUpgradeAchievements();
                        farmer.autoGenerateActiveDialogueEvent("houseUpgrade_" + farmer.HouseUpgradeLevel);
                        
                        SMonitor?.Log($"Completed house upgrade for {farmer.Name} (fallback method) to level {farmer.HouseUpgradeLevel}!", LogLevel.Info);
                        
                        // Free Robin after completing the upgrade
                        FreeBuilder("Robin");
                    }
                }
            }
        }

        private static void CompleteCommunityUpgrades()
        {
            Town town = Game1.getLocationFromName("Town") as Town;
            if (town == null || town.daysUntilCommunityUpgrade.Value <= 0)
                return;

            // EDGE CASE 1 & 2: Check for festival days and Green Rain year 1
            if (Utility.isFestivalDay(Game1.dayOfMonth, Game1.season))
            {
                SMonitor?.Log("Festival day - skipping community upgrade completion", LogLevel.Debug);
                return;
            }

            if (Game1.isGreenRain && Game1.year == 1)
            {
                SMonitor?.Log("Green Rain year 1 - skipping community upgrade completion", LogLevel.Debug);
                return;
            }

            // EDGE CASE 5: Fix community upgrade mail logic
            // Determine which upgrade this is based on mail flags, not timer value
            bool isPamHouseUpgrade = !Game1.MasterPlayer.mailReceived.Contains("pamHouseUpgrade");
            
            if (isPamHouseUpgrade)
            {
                // Pam's house upgrade - use proper multiplayer-safe mail system
                Game1.player.team.RequestSetMail(PlayerActionTarget.Host, "pamHouseUpgrade", MailType.Received, add: true);
                
                // Add friendship with Pam
                NPC pam = Game1.getCharacterFromName("Pam");
                if (pam != null)
                {
                    Game1.player.changeFriendship(1000, pam);
                }
                
                SMonitor?.Log("Completed Pam's house upgrade! Added mail and friendship.", LogLevel.Info);
            }
            else
            {
                // Shortcuts upgrade - use proper multiplayer-safe mail system
                Game1.player.team.RequestSetMail(PlayerActionTarget.Host, "communityUpgradeShortcuts", MailType.Received, add: true);
                SMonitor?.Log("Completed shortcuts upgrade! Added mail.", LogLevel.Info);
            }
            
            // Set to 0 to complete the upgrade
            town.daysUntilCommunityUpgrade.Value = 0;
            
            // Free Robin after completing the upgrade
            FreeBuilder("Robin");
        }

        private static void FreeBuilder(string builderName)
        {
            NPC builder = Game1.getCharacterFromName(builderName);
            if (builder == null)
                return;

            // Stop the hammer animation flag
            builder.shouldPlayRobinHammerAnimation.Value = false;
            
            // Reset her AI and schedule flags
            builder.ignoreScheduleToday = false;
            builder.controller = null;
            builder.Sprite.StopAnimation();
            
            // Clear her construction dialogue
            builder.CurrentDialogue.Clear();
            builder.resetCurrentDialogue();
            
            // EDGE CASE 7: Add null checks before warping
            // Warp her home and make her follow her schedule
            // Only do this for Robin, as she has a shop we need to access.
            // Others (like Wizard) might be fine staying where they are.
            if (builderName == "Robin")
            {
                GameLocation scienceHouse = Game1.getLocationFromName("ScienceHouse");
                if (scienceHouse != null)
                {
                    Game1.warpCharacter(builder, "ScienceHouse", new Vector2(8, 18));
                    builder.checkSchedule(Game1.timeOfDay);
                    SMonitor?.Log($"Freed builder {builderName}, cleared dialogue, and warped to ScienceHouse!", LogLevel.Debug);
                }
                else
                {
                    // Location not loaded, just reset her state
                    SMonitor?.Log($"Freed builder {builderName}, cleared dialogue (ScienceHouse not loaded, skipped warp)!", LogLevel.Debug);
                }
            }
            else
            {
                SMonitor?.Log($"Freed builder {builderName}, cleared dialogue!", LogLevel.Debug);
            }
        }
    }
}
