using System;
using System.Collections.Generic;
using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Characters;
using WorkingPets.Behaviors;
using WorkingPets.Patches;
using WorkingPets.UI;

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
        [Obsolete("Use PetManager.GetManagerForPet() instead for multi-pet support")]
        public static PetWorkManager WorkManager { get; private set; } = null!;
        
        /// <summary>Manages multiple pets and coordinates their work.</summary>
        public static MultiPetManager PetManager { get; private set; } = null!;

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
            #pragma warning disable CS0618 // Intentionally using legacy WorkManager for backwards compat
            WorkManager = new PetWorkManager(); // Legacy, kept for backwards compatibility
            #pragma warning restore CS0618
            PetManager = new MultiPetManager();
            InventoryManager = new PetInventoryManager();
            ScavengeManager = new PetScavengeManager();

            // Apply Harmony patches
            var harmony = new Harmony(this.ModManifest.UniqueID);
            PetPatches.Apply(harmony, this.Monitor);

            // Register events
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.GameLoop.Saving += OnSaving;
            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
            helper.Events.GameLoop.DayStarted += OnDayStarted;
            helper.Events.Display.MenuChanged += OnMenuChanged;
            helper.Events.Display.RenderingHud += OnRenderingHud; // Draw pet map icons
            helper.Events.Input.ButtonPressed += OnButtonPressed;
            helper.Events.Player.Warped += OnPlayerWarped; // Exit resting on location change

            this.Monitor.Log("Working Pets mod loaded! Talk to your pet to toggle work mode, or press V to whistle!", LogLevel.Info);
        }

        /*********
        ** Private methods
        *********/
        /// <summary>Raised when a button is pressed.</summary>
        private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady || Game1.activeClickableMenu != null)
                return;

            // Toggle whistle menu
            if (Config.WhistleKey.JustPressed())
            {
                if (Game1.activeClickableMenu is WhistleMenu)
                {
                    Game1.activeClickableMenu.exitThisMenu();
                    Game1.playSound("bigDeSelect");
                }
                else
                {
                    Game1.activeClickableMenu = new WhistleMenu();
                    Game1.playSound("bigSelect");
                }
            }
        }

        /// <summary>Raised after a player warps to a new location.</summary>
        private void OnPlayerWarped(object? sender, WarpedEventArgs e)
        {
            if (!e.IsLocalPlayer)
                return;

            // Exit resting state for all pets when player changes location
            Utility.ForEachLocation(location =>
            {
                foreach (var character in location.characters)
                {
                    if (character is Pet pet)
                    {
                        var manager = PetManager?.GetManagerForPet(pet);
                        if (manager != null && manager.IsFollowing)
                        {
                            // Wake up pet if sleeping on bed
                            if (pet.isSleepingOnFarmerBed?.Value == true)
                            {
                                pet.isSleepingOnFarmerBed.Value = false;
                                pet.CurrentBehavior = "Walk";
                            }
                            
                            // Clear sleep emote by setting doingEndOfRouteAnimation to false
                            if (pet.IsEmoting && (pet.CurrentEmote == Character.sleepEmote || pet.CurrentEmote == Character.sadEmote))
                            {
                                pet.doingEndOfRouteAnimation.Value = false;
                                pet.IsEmoting = false;
                            }
                            
                            // Exit sleep behavior
                            if (string.Equals(pet.CurrentBehavior, Pet.behavior_Sleep, StringComparison.Ordinal))
                            {
                                pet.CurrentBehavior = "Walk";
                            }
                        }
                    }
                }
                return true;
            });
        }
        /// <summary>Raised after the game is launched, right before the first update tick.</summary>
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
                save: () => 
                {
                    // Validate priorities before saving
                    ValidatePriorities();
                    this.Helper.WriteConfig(Config);
                },
                titleScreenOnly: false // Allow editing in-game!
            );

            // === General Settings ===
            configMenu.AddSectionTitle(
                mod: this.ModManifest,
                text: () => "General Settings"
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Mod Enabled",
                tooltip: () => "Turn the entire mod on or off.",
                getValue: () => Config.ModEnabled,
                setValue: value => Config.ModEnabled = value
            );

            configMenu.AddParagraph(
                mod: this.ModManifest,
                text: () => $"Whistle Key: {Config.WhistleKey}\n(Edit in config.json to change)"
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Mod Enabled",
                tooltip: () => "Enable or disable the Working Pets mod entirely.",
                getValue: () => Config.ModEnabled,
                setValue: value => Config.ModEnabled = value
            );

            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => "Work Interval (ticks)",
                tooltip: () => "Number of game ticks between each work scan. 60 ticks = 1 second. Lower = faster but more CPU.",
                getValue: () => Config.TicksBetweenActions,
                setValue: value => Config.TicksBetweenActions = value,
                min: 10,
                max: 300,
                interval: 10
            );

            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => "Work Radius (tiles)",
                tooltip: () => "How far (in tiles) your pet will search for work.",
                getValue: () => Config.WorkRadius,
                setValue: value => Config.WorkRadius = value,
                min: 5,
                max: 100
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Show Work Messages",
                tooltip: () => "Show HUD messages when your pet clears things.",
                getValue: () => Config.ShowWorkingMessages,
                setValue: value => Config.ShowWorkingMessages = value
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Follow Outside Farm (Experimental)",
                tooltip: () => "Allow your pet to follow you outside the farm. If disabled, pet stops following when you leave the farm.",
                getValue: () => Config.FollowOutsideFarm,
                setValue: value => Config.FollowOutsideFarm = value
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Forage While Following",
                tooltip: () => "When following you, your pet will pick up forageable items (spring onions, berries, mushrooms, etc.) and store them in their inventory.",
                getValue: () => Config.ForageWhileFollowing,
                setValue: value => Config.ForageWhileFollowing = value
            );

            // === Work Types ===
            configMenu.AddSectionTitle(
                mod: this.ModManifest,
                text: () => "What Your Pet Will Clear"
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Clear Debris",
                tooltip: () => "Clear weeds, small stones, and twigs.",
                getValue: () => Config.ClearDebris,
                setValue: value => Config.ClearDebris = value
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Clear Stumps & Logs",
                tooltip: () => "Clear tree stumps and large logs (gives hardwood).",
                getValue: () => Config.ClearStumpsAndLogs,
                setValue: value => Config.ClearStumpsAndLogs = value
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Chop Trees",
                tooltip: () => "Chop down fully grown trees. WARNING: This includes trees you planted!",
                getValue: () => Config.ChopTrees,
                setValue: value => Config.ChopTrees = value
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Break Boulders",
                tooltip: () => "Break large boulders (gives stone and ore).",
                getValue: () => Config.BreakBoulders,
                setValue: value => Config.BreakBoulders = value
            );

            // === Priority Settings ===
            configMenu.AddSectionTitle(
                mod: this.ModManifest,
                text: () => "Priority Settings"
            );

            configMenu.AddParagraph(
                mod: this.ModManifest,
                text: () => "Lower number = higher priority. Your pet will prioritize work types with lower numbers first. Use different numbers (1-99) for each type!"
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Ignore Priority (Target Nearest)",
                tooltip: () => "When enabled, your pet ignores priority and just targets the nearest thing.",
                getValue: () => Config.IgnorePriority,
                setValue: value => Config.IgnorePriority = value
            );

            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => "Debris Priority",
                tooltip: () => "Priority for clearing debris. Lower = higher priority.",
                getValue: () => Config.DebrisPriority,
                setValue: value => Config.DebrisPriority = value,
                min: 1,
                max: 99
            );

            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => "Stumps & Logs Priority",
                tooltip: () => "Priority for clearing stumps and logs. Lower = higher priority.",
                getValue: () => Config.StumpsAndLogsPriority,
                setValue: value => Config.StumpsAndLogsPriority = value,
                min: 1,
                max: 99
            );

            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => "Trees Priority",
                tooltip: () => "Priority for chopping trees. Lower = higher priority.",
                getValue: () => Config.TreesPriority,
                setValue: value => Config.TreesPriority = value,
                min: 1,
                max: 99
            );

            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => "Boulders Priority",
                tooltip: () => "Priority for breaking boulders. Lower = higher priority.",
                getValue: () => Config.BouldersPriority,
                setValue: value => Config.BouldersPriority = value,
                min: 1,
                max: 99
            );

            this.Monitor.Log("Generic Mod Config Menu integration complete!", LogLevel.Debug);
        }

        /// <summary>Validate that all priorities are unique. If not, auto-fix them.</summary>
        private void ValidatePriorities()
        {
            var priorities = new HashSet<int>();
            var usedPriorities = new List<(string name, int value)>
            {
                ("Debris", Config.DebrisPriority),
                ("StumpsAndLogs", Config.StumpsAndLogsPriority),
                ("Trees", Config.TreesPriority),
                ("Boulders", Config.BouldersPriority)
            };

            bool hasDuplicates = false;
            foreach (var (name, value) in usedPriorities)
            {
                if (!priorities.Add(value))
                {
                    hasDuplicates = true;
                    break;
                }
            }

            if (hasDuplicates)
            {
                this.Monitor.Log("Duplicate priorities detected! Auto-fixing to 1, 2, 3, 4...", LogLevel.Warn);
                Config.DebrisPriority = 1;
                Config.StumpsAndLogsPriority = 2;
                Config.TreesPriority = 3;
                Config.BouldersPriority = 4;
            }
        }

        /// <summary>Raised after the save file is loaded.</summary>
        private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
        {
            // Initialize all pets in the game
            PetManager.InitializeAllPets();
            
            // Load inventory (shared across all pets)
            var allPets = MultiPetManager.GetAllPets();
            if (allPets.Count > 0)
            {
                InventoryManager.Load(allPets[0]); // Load from first pet's modData
                this.Monitor.Log($"Found {allPets.Count} pet(s). Working Pets initialized!", LogLevel.Info);
                foreach (var pet in allPets)
                {
                    this.Monitor.Log($"  - {pet.Name} ({pet.petType.Value}, {pet.whichBreed.Value})", LogLevel.Info);
                }
            }
            else
            {
                this.Monitor.Log("No pets found. Get a pet to use Working Pets!", LogLevel.Info);
            }
            
            // Legacy: keep WorkManager synced with first pet for backwards compat
            if (allPets.Count > 0)
            {
                #pragma warning disable CS0618 // Intentionally using legacy WorkManager
                WorkManager.Initialize(allPets[0]);
                #pragma warning restore CS0618
            }
        }

        /// <summary>Raised before the game saves.</summary>
        private void OnSaving(object? sender, SavingEventArgs e)
        {
            // Save state for all pets
            PetManager.SaveAllStates();
            
            // Save inventory (shared across all pets, stored in first pet's modData)
            var allPets = MultiPetManager.GetAllPets();
            if (allPets.Count > 0)
            {
                InventoryManager.Save(allPets[0]);
            }
        }

        /// <summary>Raised every game tick (~60 times per second).</summary>
        private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady || !Config.ModEnabled)
                return;

            // Update all pets
            PetManager.UpdateAll();
        }

        /// <summary>Raised when a new day starts.</summary>
        private void OnDayStarted(object? sender, DayStartedEventArgs e)
        {
            var allPets = MultiPetManager.GetAllPets();
            
            // Trigger daily scavenge for each pet
            foreach (var pet in allPets)
            {
                ScavengeManager.PerformDailyScavenge(pet);
                
                // Ensure pet is on the farm if it was working
                var manager = PetManager.GetManagerForPet(pet);
                if (manager?.IsWorking == true && pet.currentLocation?.Name != "Farm")
                {
                    Game1.warpCharacter(pet, "Farm", new Microsoft.Xna.Framework.Vector2(54, 8));
                    this.Monitor.Log($"Warped {pet.Name} back to the farm.", LogLevel.Debug);
                }
            }
        }

        /// <summary>Raised when the active menu changes (detect menu close to resume pet).</summary>
        private void OnMenuChanged(object? sender, MenuChangedEventArgs e)
        {
            // When any menu closes, resume all pets
            if (e.NewMenu == null && e.OldMenu != null)
            {
                PetManager.ResumeAllFromDialogue();
            }
        }

        /// <summary>Raised before the HUD is drawn to screen - draw pet map icons.</summary>
        private void OnRenderingHud(object? sender, RenderingHudEventArgs e)
        {
            UI.PetMapTracker.DrawPetIcons(e.SpriteBatch);
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
