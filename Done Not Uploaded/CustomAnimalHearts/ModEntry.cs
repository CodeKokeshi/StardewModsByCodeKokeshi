#nullable enable
using System;
using System.Linq;
using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Characters;

namespace CustomAnimalHearts
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        /// <summary>Static config instance accessible by the whole mod.</summary>
        internal static ModConfig Config { get; private set; } = null!;
        
        /// <summary>Static monitor for logging from static methods.</summary>
        internal static IMonitor ModMonitor { get; private set; } = null!;

        public override void Entry(IModHelper helper)
        {
            ModMonitor = this.Monitor;
            Config = helper.ReadConfig<ModConfig>();
            
            // Register GMCM on game launch
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            
            // Apply animal hearts on save loaded
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            
            Monitor.Log("[CustomAnimalHearts] Loaded! Configure in GMCM to override pet and farm animal hearts.", LogLevel.Info);
        }

        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            // Get Generic Mod Config Menu's API (if it's installed)
            var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
            {
                Monitor.Log("Generic Mod Config Menu not found. Config can only be edited via config.json", LogLevel.Info);
                return;
            }

            // Register mod - titleScreenOnly = true as specified
            configMenu.Register(
                mod: this.ModManifest,
                reset: () => Config = new ModConfig(),
                save: () => this.Helper.WriteConfig(Config),
                titleScreenOnly: true
            );

            // === Main Settings ===
            configMenu.AddSectionTitle(
                mod: this.ModManifest,
                text: () => "Custom Animal Hearts"
            );

            configMenu.AddParagraph(
                mod: this.ModManifest,
                text: () => "Override the friendship/heart levels for pets and farm animals. Changes apply when loading a save."
            );

            // Enabled Toggle
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Enabled",
                tooltip: () => "Enable or disable the entire mod. When disabled, no hearts will be overridden.",
                getValue: () => Config.Enabled,
                setValue: value => Config.Enabled = value
            );

            // === Pet Hearts Section ===
            configMenu.AddSectionTitle(
                mod: this.ModManifest,
                text: () => "Pet Hearts"
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Override Pet Hearts",
                tooltip: () => "When enabled, override your pet's friendship level to the specified hearts value.",
                getValue: () => Config.OverridePetHearts,
                setValue: value => Config.OverridePetHearts = value
            );

            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => "Pet Hearts",
                tooltip: () => "Number of hearts (0-10) your pet will have. Each heart = 100 friendship points.",
                getValue: () => Config.PetHearts,
                setValue: value => Config.PetHearts = value,
                min: 0,
                max: 10,
                interval: 1,
                formatValue: value => $"{value} ♥"
            );

            // === Farm Animal Hearts Section ===
            configMenu.AddSectionTitle(
                mod: this.ModManifest,
                text: () => "Farm Animal Hearts"
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Override Farm Animal Hearts",
                tooltip: () => "When enabled, override all farm animals' friendship level to the specified hearts value.",
                getValue: () => Config.OverrideFarmAnimalHearts,
                setValue: value => Config.OverrideFarmAnimalHearts = value
            );

            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => "Farm Animal Hearts",
                tooltip: () => "Number of hearts (0-10) your farm animals will have. Each heart = 100 friendship points.",
                getValue: () => Config.FarmAnimalHearts,
                setValue: value => Config.FarmAnimalHearts = value,
                min: 0,
                max: 10,
                interval: 1,
                formatValue: value => $"{value} ♥"
            );

            Monitor.Log("Generic Mod Config Menu integration complete!", LogLevel.Debug);
        }

        private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
        {
            if (!Config.Enabled)
            {
                Monitor.Log("Mod is disabled, skipping animal hearts override.", LogLevel.Debug);
                return;
            }

            // Override pet hearts
            if (Config.OverridePetHearts)
            {
                ApplyPetHearts();
            }

            // Override farm animal hearts
            if (Config.OverrideFarmAnimalHearts)
            {
                ApplyFarmAnimalHearts();
            }
        }

        /// <summary>Apply the configured hearts to all pets.</summary>
        private void ApplyPetHearts()
        {
            int targetFriendship = Config.PetHearts * 100; // Each heart = 100 friendship
            int petsModified = 0;

            // Find all pets in all locations
            foreach (var location in Game1.locations)
            {
                foreach (var character in location.characters)
                {
                    if (character is Pet pet)
                    {
                        pet.friendshipTowardFarmer.Value = targetFriendship;
                        petsModified++;
                        Monitor.Log($"Set {pet.Name}'s friendship to {targetFriendship} ({Config.PetHearts} hearts)", LogLevel.Debug);
                    }
                }
            }

            if (petsModified > 0)
            {
                Monitor.Log($"Applied {Config.PetHearts} hearts to {petsModified} pet(s).", LogLevel.Info);
            }
            else
            {
                Monitor.Log("No pets found to modify.", LogLevel.Debug);
            }
        }

        /// <summary>Apply the configured hearts to all farm animals.</summary>
        private void ApplyFarmAnimalHearts()
        {
            int targetFriendship = Config.FarmAnimalHearts * 100; // Each heart = 100 friendship
            int animalsModified = 0;

            // Find all farm animals in all locations
            foreach (var location in Game1.locations)
            {
                foreach (var animal in location.animals.Values)
                {
                    animal.friendshipTowardFarmer.Value = targetFriendship;
                    animalsModified++;
                    Monitor.Log($"Set {animal.Name} ({animal.type.Value})'s friendship to {targetFriendship} ({Config.FarmAnimalHearts} hearts)", LogLevel.Trace);
                }
            }

            if (animalsModified > 0)
            {
                Monitor.Log($"Applied {Config.FarmAnimalHearts} hearts to {animalsModified} farm animal(s).", LogLevel.Info);
            }
            else
            {
                Monitor.Log("No farm animals found to modify.", LogLevel.Debug);
            }
        }
    }
}
