using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;

namespace InstantBuildings
{
    public class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.DayStarted += OnDayStarted;
            helper.Events.World.BuildingListChanged += OnBuildingListChanged;
        }

        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            // Complete any buildings currently under construction
            CompleteAllBuildings();
        }

        private void OnBuildingListChanged(object sender, BuildingListChangedEventArgs e)
        {
            // Only trigger when buildings are added
            if (e.Added.Any())
            {
                CompleteAllBuildings();
            }
        }

        private void CompleteAllBuildings()
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
                    building.daysOfConstructionLeft.Value = 0;
                    building.daysUntilUpgrade.Value = 0;
                    building.FinishConstruction();
                    foundConstruction = true;
                    this.Monitor.Log($"Completed construction of {building.buildingType.Value}!", LogLevel.Info);
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
                    // Stop the hammer animation flag (from extracted NPC.cs)
                    robin.shouldPlayRobinHammerAnimation.Value = false;
                    
                    // Reset her AI and schedule flags
                    robin.ignoreScheduleToday = false;
                    robin.controller = null;
                    robin.Sprite.StopAnimation();
                    
                    // Clear her construction dialogue
                    robin.CurrentDialogue.Clear();
                    robin.resetCurrentDialogue();
                    
                    // Warp her home and make her follow her schedule
                    Game1.warpCharacter(robin, "ScienceHouse", new Vector2(27, 10));
                    robin.checkSchedule(Game1.timeOfDay);
                    
                    this.Monitor.Log("Sent Robin home, cleared dialogue, and reset her schedule!", LogLevel.Debug);
                }
            }
        }
    }
}
