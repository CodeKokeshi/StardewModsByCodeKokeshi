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
                postfix: new HarmonyMethod(typeof(ModEntry), nameof(AfterMarkUnderConstruction))
            );

            helper.Events.GameLoop.DayStarted += OnDayStarted;
            helper.Events.World.BuildingListChanged += OnBuildingListChanged;
        }

        private static void AfterMarkUnderConstruction()
        {
            // Trigger completion logic immediately when a building is marked under construction
            CompleteAllBuildingsStatic();
        }

        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            // Complete any buildings currently under construction
            CompleteAllBuildingsStatic();
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
                
                NPC robin = Game1.getCharacterFromName("Robin");
                if (robin != null)
                {
                    // Stop the hammer animation flag
                    robin.shouldPlayRobinHammerAnimation.Value = false;
                    
                    // Reset her AI and schedule flags
                    robin.ignoreScheduleToday = false;
                    robin.controller = null;
                    robin.Sprite.StopAnimation();
                    
                    // Clear her construction dialogue
                    robin.CurrentDialogue.Clear();
                    robin.resetCurrentDialogue();
                    
                    // Warp her home and make her follow her schedule
                    Game1.warpCharacter(robin, "ScienceHouse", new Vector2(8, 18));
                    robin.checkSchedule(Game1.timeOfDay);
                    
                    SMonitor?.Log("Sent Robin home, cleared dialogue, and reset her schedule!", LogLevel.Debug);
                }
            }
        }
    }
}
