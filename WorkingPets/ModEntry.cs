using System;
using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Characters;
using WorkingPets.Behaviors;
using WorkingPets.Patches;

namespace WorkingPets
{
    /// <summary>Main entry point for Working Pets mod.</summary>
    public class ModEntry : Mod
    {
        /*********
        ** Public fields
        *********/
        /// <summary>Singleton instance of the mod.</summary>
        public static ModEntry Instance { get; private set; } = null!;

        /// <summary>The mod configuration.</summary>
        public static ModConfig Config { get; private set; } = null!;

        /// <summary>Manages pet work behavior.</summary>
        public static PetWorkManager WorkManager { get; private set; } = null!;

        /// <summary>Manages pet inventory.</summary>
        public static PetInventoryManager InventoryManager { get; private set; } = null!;

        /// <summary>Manages daily scavenging.</summary>
        public static PetScavengeManager ScavengeManager { get; private set; } = null!;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            Instance = this;
            Config = helper.ReadConfig<ModConfig>();

            // Initialize managers
            WorkManager = new PetWorkManager();
            InventoryManager = new PetInventoryManager();
            ScavengeManager = new PetScavengeManager();

            // Apply Harmony patches
            var harmony = new Harmony(this.ModManifest.UniqueID);
            PetPatches.Apply(harmony, this.Monitor);

            // Register events
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.GameLoop.Saving += OnSaving;
            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
            helper.Events.GameLoop.DayStarted += OnDayStarted;

            this.Monitor.Log("Working Pets mod loaded! Talk to your pet to toggle work mode.", LogLevel.Info);
        }

        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the save file is loaded.</summary>
        private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
        {
            // Find the player's pet and initialize
            Pet? pet = GetPlayerPet();
            if (pet != null)
            {
                WorkManager.Initialize(pet);
                InventoryManager.Load(pet);
                this.Monitor.Log($"Found pet: {pet.Name}. Working Pets initialized!", LogLevel.Info);
            }
            else
            {
                this.Monitor.Log("No pet found. Get a pet to use Working Pets!", LogLevel.Info);
            }
        }

        /// <summary>Raised before the game saves.</summary>
        private void OnSaving(object? sender, SavingEventArgs e)
        {
            Pet? pet = GetPlayerPet();
            if (pet != null)
            {
                InventoryManager.Save(pet);
                WorkManager.SaveState(pet);
            }
        }

        /// <summary>Raised every game tick (~60 times per second).</summary>
        private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady || !Config.ModEnabled)
                return;

            WorkManager.Update();
        }

        /// <summary>Raised when a new day starts.</summary>
        private void OnDayStarted(object? sender, DayStartedEventArgs e)
        {
            Pet? pet = GetPlayerPet();
            if (pet == null)
                return;

            // Trigger daily scavengehunt
            ScavengeManager.PerformDailyScavenge(pet);

            // Ensure pet is on the farm
            if (pet.currentLocation?.Name != "Farm")
            {
                // Warp pet to farm
                Game1.warpCharacter(pet, "Farm", new Microsoft.Xna.Framework.Vector2(54, 8));
                this.Monitor.Log($"Warped {pet.Name} back to the farm.", LogLevel.Debug);
            }
        }

        /// <summary>Gets the player's pet.</summary>
        public static Pet? GetPlayerPet()
        {
            foreach (NPC npc in Utility.getAllCharacters())
            {
                if (npc is Pet pet)
                    return pet;
            }
            return null;
        }
    }
}
