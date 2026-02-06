#nullable enable
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Monsters;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;

namespace PlayerCheats
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        /*********
        ** Public Fields
        *********/
        /// <summary>Static config instance accessible by the whole mod.</summary>
        internal static ModConfig Config { get; private set; } = null!;

        /// <summary>Static monitor for logging from static methods.</summary>
        internal static IMonitor ModMonitor { get; private set; } = null!;

        /// <summary>Static helper for accessing mod APIs.</summary>
        internal static IModHelper ModHelper { get; private set; } = null!;

        /// <summary>Harmony instance for patching.</summary>
        private Harmony? harmony;

        /// <summary>GMCM API for opening menu with hotkey.</summary>
        private IGenericModConfigMenuApi? gmcmApi;

        /*********
        ** Public Methods
        *********/
        public override void Entry(IModHelper helper)
        {
            ModMonitor = this.Monitor;
            ModHelper = helper;
            Config = helper.ReadConfig<ModConfig>();

            harmony = new Harmony(this.ModManifest.UniqueID);
            ApplyPatches();

            // Register events
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
            helper.Events.GameLoop.OneSecondUpdateTicked += OnOneSecondUpdateTicked;
            helper.Events.GameLoop.DayStarted += OnDayStarted;
            helper.Events.GameLoop.TimeChanged += OnTimeChanged;
            helper.Events.Input.ButtonPressed += OnButtonPressed;

            Monitor.Log("[PlayerCheats] Loaded! Configure in GMCM.", LogLevel.Info);
        }

        /*********
        ** Private Methods
        *********/
        private void ApplyPatches()
        {
            if (harmony == null) return;

            // Helper method for safe patching
            void TryPatch(MethodBase? original, HarmonyMethod? prefix = null, HarmonyMethod? postfix = null, string description = "")
            {
                if (original == null)
                {
                    Monitor.Log($"[PlayerCheats] WARNING: Could not find method to patch: {description}", LogLevel.Warn);
                    return;
                }
                try
                {
                    harmony.Patch(original, prefix: prefix, postfix: postfix);
                    Monitor.Log($"[PlayerCheats] Patched: {description}", LogLevel.Trace);
                }
                catch (Exception ex)
                {
                    Monitor.Log($"[PlayerCheats] ERROR patching {description}: {ex.Message}", LogLevel.Error);
                }
            }

            // === Player Damage (Invincibility) ===
            TryPatch(
                original: AccessTools.Method(typeof(Farmer), nameof(Farmer.takeDamage)),
                prefix: new HarmonyMethod(typeof(PlayerPatches), nameof(PlayerPatches.Farmer_TakeDamage_Prefix)),
                description: "Farmer.takeDamage"
            );

            // === Movement Speed ===
            TryPatch(
                original: AccessTools.Method(typeof(Farmer), nameof(Farmer.getMovementSpeed)),
                postfix: new HarmonyMethod(typeof(PlayerPatches), nameof(PlayerPatches.Farmer_GetMovementSpeed_Postfix)),
                description: "Farmer.getMovementSpeed"
            );

            // === Magnetic Radius ===
            TryPatch(
                original: AccessTools.PropertyGetter(typeof(Farmer), nameof(Farmer.MagneticRadius)),
                postfix: new HarmonyMethod(typeof(PlayerPatches), nameof(PlayerPatches.Farmer_MagneticRadius_Postfix)),
                description: "Farmer.MagneticRadius getter"
            );

            // === Stamina Consumption ===
            TryPatch(
                original: AccessTools.PropertySetter(typeof(Farmer), nameof(Farmer.stamina)),
                prefix: new HarmonyMethod(typeof(PlayerPatches), nameof(PlayerPatches.Farmer_Stamina_Prefix)),
                description: "Farmer.stamina setter"
            );

            // === Monster One Hit Kill ===
            TryPatch(
                original: AccessTools.Method(typeof(Monster), nameof(Monster.takeDamage), new Type[] { typeof(int), typeof(int), typeof(int), typeof(bool), typeof(double), typeof(Farmer) }),
                prefix: new HarmonyMethod(typeof(PlayerPatches), nameof(PlayerPatches.Monster_TakeDamage_Prefix)),
                description: "Monster.takeDamage"
            );

            // === Weapon Critical Chance ===
            TryPatch(
                original: AccessTools.Method(typeof(MeleeWeapon), nameof(MeleeWeapon.DoDamage)),
                prefix: new HarmonyMethod(typeof(PlayerPatches), nameof(PlayerPatches.MeleeWeapon_DoDamage_Prefix)),
                description: "MeleeWeapon.DoDamage"
            );

            // === Tool Stamina Cost ===
            TryPatch(
                original: AccessTools.Method(typeof(Tool), nameof(Tool.DoFunction)),
                prefix: new HarmonyMethod(typeof(PlayerPatches), nameof(PlayerPatches.Tool_DoFunction_Prefix)),
                description: "Tool.DoFunction"
            );

            // === Hoe/Watering Can Area ===
            TryPatch(
                original: AccessTools.Method(typeof(Tool), "tilesAffected", new Type[] { typeof(Vector2), typeof(int), typeof(Farmer) }),
                postfix: new HarmonyMethod(typeof(PlayerPatches), nameof(PlayerPatches.Tool_TilesAffected_Postfix)),
                description: "Tool.tilesAffected"
            );

            // === Watering Can Infinite Water ===
            TryPatch(
                original: AccessTools.PropertyGetter(typeof(WateringCan), nameof(WateringCan.WaterLeft)),
                postfix: new HarmonyMethod(typeof(PlayerPatches), nameof(PlayerPatches.WateringCan_WaterLeft_Postfix)),
                description: "WateringCan.WaterLeft getter"
            );

            // === Skill Levels ===
            TryPatch(
                original: AccessTools.PropertyGetter(typeof(Farmer), nameof(Farmer.FarmingLevel)),
                postfix: new HarmonyMethod(typeof(PlayerPatches), nameof(PlayerPatches.Farmer_FarmingLevel_Postfix)),
                description: "Farmer.FarmingLevel getter"
            );
            TryPatch(
                original: AccessTools.PropertyGetter(typeof(Farmer), nameof(Farmer.MiningLevel)),
                postfix: new HarmonyMethod(typeof(PlayerPatches), nameof(PlayerPatches.Farmer_MiningLevel_Postfix)),
                description: "Farmer.MiningLevel getter"
            );
            TryPatch(
                original: AccessTools.PropertyGetter(typeof(Farmer), nameof(Farmer.ForagingLevel)),
                postfix: new HarmonyMethod(typeof(PlayerPatches), nameof(PlayerPatches.Farmer_ForagingLevel_Postfix)),
                description: "Farmer.ForagingLevel getter"
            );
            TryPatch(
                original: AccessTools.PropertyGetter(typeof(Farmer), nameof(Farmer.FishingLevel)),
                postfix: new HarmonyMethod(typeof(PlayerPatches), nameof(PlayerPatches.Farmer_FishingLevel_Postfix)),
                description: "Farmer.FishingLevel getter"
            );
            TryPatch(
                original: AccessTools.PropertyGetter(typeof(Farmer), nameof(Farmer.CombatLevel)),
                postfix: new HarmonyMethod(typeof(PlayerPatches), nameof(PlayerPatches.Farmer_CombatLevel_Postfix)),
                description: "Farmer.CombatLevel getter"
            );

            // === XP Gain ===
            TryPatch(
                original: AccessTools.Method(typeof(Farmer), nameof(Farmer.gainExperience)),
                prefix: new HarmonyMethod(typeof(PlayerPatches), nameof(PlayerPatches.Farmer_GainExperience_Prefix)),
                description: "Farmer.gainExperience"
            );

            // === Time Freeze ===
            TryPatch(
                original: AccessTools.Method(typeof(Game1), nameof(Game1.performTenMinuteClockUpdate)),
                prefix: new HarmonyMethod(typeof(PlayerPatches), nameof(PlayerPatches.Game1_PerformTenMinuteClockUpdate_Prefix)),
                description: "Game1.performTenMinuteClockUpdate"
            );

            // === Passout Prevention (static method) ===
            TryPatch(
                original: AccessTools.Method(typeof(Farmer), nameof(Farmer.passOutFromTired)),
                prefix: new HarmonyMethod(typeof(PlayerPatches), nameof(PlayerPatches.Farmer_PassOutFromTired_Prefix)),
                description: "Farmer.passOutFromTired"
            );

            // === Friendship Gain ===
            TryPatch(
                original: AccessTools.Method(typeof(Farmer), nameof(Farmer.changeFriendship)),
                prefix: new HarmonyMethod(typeof(PlayerPatches), nameof(PlayerPatches.Farmer_ChangeFriendship_Prefix)),
                description: "Farmer.changeFriendship"
            );

            // === Harvest Quality ===
            TryPatch(
                original: AccessTools.Method(typeof(Crop), nameof(Crop.harvest)),
                prefix: new HarmonyMethod(typeof(PlayerPatches), nameof(PlayerPatches.Crop_Harvest_Prefix)),
                description: "Crop.harvest"
            );

            // === Max Fish Quality ===
            TryPatch(
                original: AccessTools.Method(typeof(FishingRod), nameof(FishingRod.pullFishFromWater)),
                prefix: new HarmonyMethod(typeof(PlayerPatches), nameof(PlayerPatches.FishingRod_PullFishFromWater_Prefix)),
                description: "FishingRod.pullFishFromWater"
            );

            // === Infinite Items ===
            TryPatch(
                original: AccessTools.Method(typeof(Farmer), nameof(Farmer.reduceActiveItemByOne)),
                prefix: new HarmonyMethod(typeof(PlayerPatches), nameof(PlayerPatches.Farmer_ReduceActiveItemByOne_Prefix)),
                description: "Farmer.reduceActiveItemByOne"
            );

            TryPatch(
                original: AccessTools.Method(typeof(Item), nameof(Item.ConsumeStack)),
                prefix: new HarmonyMethod(typeof(PlayerPatches), nameof(PlayerPatches.Item_ConsumeStack_Prefix)),
                description: "Item.ConsumeStack"
            );

            // === Fishing Rod (Instant Bite) ===
            TryPatch(
                original: AccessTools.Method(typeof(FishingRod), nameof(FishingRod.DoFunction)),
                postfix: new HarmonyMethod(typeof(PlayerPatches), nameof(PlayerPatches.FishingRod_DoFunction_Postfix)),
                description: "FishingRod.DoFunction"
            );

            // === Max Stamina Override ===
            TryPatch(
                original: AccessTools.PropertyGetter(typeof(Farmer), nameof(Farmer.MaxStamina)),
                postfix: new HarmonyMethod(typeof(PlayerPatches), nameof(PlayerPatches.Farmer_MaxStamina_Postfix)),
                description: "Farmer.MaxStamina getter"
            );

            // === Attack ===
            TryPatch(
                original: AccessTools.PropertyGetter(typeof(Farmer), nameof(Farmer.Attack)),
                postfix: new HarmonyMethod(typeof(PlayerPatches), nameof(PlayerPatches.Farmer_Attack_Postfix)),
                description: "Farmer.Attack getter"
            );

            // === Immunity ===
            TryPatch(
                original: AccessTools.PropertyGetter(typeof(Farmer), nameof(Farmer.Immunity)),
                postfix: new HarmonyMethod(typeof(PlayerPatches), nameof(PlayerPatches.Farmer_Immunity_Postfix)),
                description: "Farmer.Immunity getter"
            );

            // === Forage Quality Override ===
            TryPatch(
                original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.GetHarvestSpawnedObjectQuality)),
                postfix: new HarmonyMethod(typeof(PlayerPatches), nameof(PlayerPatches.GameLocation_GetHarvestSpawnedObjectQuality_Postfix)),
                description: "GameLocation.GetHarvestSpawnedObjectQuality"
            );

            // === Sell Price Multiplier ===
            TryPatch(
                original: AccessTools.Method(typeof(StardewValley.Object), nameof(StardewValley.Object.sellToStorePrice)),
                postfix: new HarmonyMethod(typeof(PlayerPatches), nameof(PlayerPatches.Object_SellToStorePrice_Postfix)),
                description: "Object.sellToStorePrice"
            );

            // === Buy Price Multiplier ===
            TryPatch(
                original: AccessTools.Method(typeof(StardewValley.Object), nameof(StardewValley.Object.salePrice)),
                postfix: new HarmonyMethod(typeof(PlayerPatches), nameof(PlayerPatches.Object_SalePrice_Postfix)),
                description: "Object.salePrice"
            );

            Monitor.Log("All Harmony patches applied!", LogLevel.Debug);
        }

        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            SetupGMCM();
        }

        private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady || !Config.ModEnabled)
                return;

            var player = Game1.player;
            if (player == null) return;

            // Infinite stamina
            if (Config.InfiniteStamina)
            {
                player.stamina = player.MaxStamina;
            }

            // Infinite health
            if (Config.InfiniteHealth)
            {
                player.health = player.maxHealth;
            }

            // Max health override
            if (Config.MaxHealthOverride > 0)
            {
                player.maxHealth = Config.MaxHealthOverride;
            }

            // Always run
            if (Config.AlwaysRun)
            {
                player.running = true;
            }

            // NoClip - set the field directly since it's a public field, not a property
            if (Config.NoClip)
            {
                player.ignoreCollisions = true;
            }
            else
            {
                player.ignoreCollisions = false;
            }

            // No monster spawns - remove all monsters
            if (Config.NoMonsterSpawns && e.IsMultipleOf(60))
            {
                var location = player.currentLocation;
                if (location != null)
                {
                    for (int i = location.characters.Count - 1; i >= 0; i--)
                    {
                        if (location.characters[i] is Monster)
                        {
                            location.characters.RemoveAt(i);
                        }
                    }
                }
            }
        }

        private void OnOneSecondUpdateTicked(object? sender, OneSecondUpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady || !Config.ModEnabled)
                return;

            var player = Game1.player;
            if (player == null) return;

            // Stamina regen
            if (Config.StaminaRegenPerSecond > 0 && player.stamina < player.MaxStamina)
            {
                player.stamina = Math.Min(player.MaxStamina, player.stamina + Config.StaminaRegenPerSecond);
            }

            // Health regen
            if (Config.HealthRegenPerSecond > 0 && player.health < player.maxHealth)
            {
                player.health = Math.Min(player.maxHealth, player.health + (int)Config.HealthRegenPerSecond);
            }

            // Max animal happiness
            if (Config.MaxAnimalHappiness)
            {
                var farm = Game1.getFarm();
                if (farm != null)
                {
                    foreach (var animal in farm.getAllFarmAnimals())
                    {
                        animal.happiness.Value = 255;
                        animal.friendshipTowardFarmer.Value = 1000;
                    }
                }
            }

            // Real-time luck setting
            if (Config.AlwaysMaxLuck)
            {
                Game1.player.team.sharedDailyLuck.Value = 0.12;
            }
            else if (Config.DailyLuckOverride >= -0.1f && Config.DailyLuckOverride <= 0.12f)
            {
                Game1.player.team.sharedDailyLuck.Value = Config.DailyLuckOverride;
            }

            // Real-time instant crop growth (all locations)
            if (Config.InstantCropGrowth)
            {
                foreach (var location in Game1.locations)
                {
                    foreach (var pair in location.terrainFeatures.Pairs)
                    {
                        if (pair.Value is HoeDirt dirt && dirt.crop != null && !dirt.crop.fullyGrown.Value)
                        {
                            dirt.crop.growCompletely();
                        }
                    }
                }
            }

            // Real-time instant tree growth (regular trees)
            if (Config.InstantTreeGrowth)
            {
                foreach (var location in Game1.locations)
                {
                    foreach (var pair in location.terrainFeatures.Pairs)
                    {
                        if (pair.Value is Tree tree && tree.growthStage.Value < Tree.treeStage)
                        {
                            tree.growthStage.Value = Tree.treeStage;
                        }
                    }
                }
            }

            // Real-time instant fruit tree growth
            if (Config.InstantFruitTreeGrowth)
            {
                foreach (var location in Game1.locations)
                {
                    foreach (var pair in location.terrainFeatures.Pairs)
                    {
                        if (pair.Value is FruitTree fruitTree && fruitTree.growthStage.Value < FruitTree.treeStage)
                        {
                            fruitTree.growthStage.Value = FruitTree.treeStage;
                            fruitTree.daysUntilMature.Value = 0;
                        }
                    }
                }
            }

            // Force forage quality on world objects
            if (Config.ForceForageQuality >= 0)
            {
                foreach (var location in Game1.locations)
                {
                    foreach (var obj in location.objects.Values)
                    {
                        if (obj.IsSpawnedObject && obj.isForage() && obj.Quality != Config.ForceForageQuality)
                        {
                            obj.Quality = Config.ForceForageQuality;
                        }
                    }
                }
            }
        }

        private void OnDayStarted(object? sender, DayStartedEventArgs e)
        {
            if (!Config.ModEnabled)
                return;

            var player = Game1.player;
            if (player == null) return;

            // Luck is now applied in real-time via OnOneSecondUpdateTicked

            // Crop growth is now applied in real-time via OnOneSecondUpdateTicked
        }

        private void OnTimeChanged(object? sender, TimeChangedEventArgs e)
        {
            if (!Context.IsWorldReady || !Config.ModEnabled)
                return;

            // Freeze time indoors
            if (Config.FreezeTimeIndoors && !Game1.currentLocation.IsOutdoors)
            {
                Game1.timeOfDay = e.OldTime;
            }
        }

        private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            // Open this mod's GMCM menu when hotkey is pressed
            if (Config.OpenMenuKey.JustPressed())
            {
                if (gmcmApi != null)
                {
                    gmcmApi.OpenModMenu(this.ModManifest);
                }
                else
                {
                    Monitor.Log("Cannot open config menu - GMCM not installed.", LogLevel.Warn);
                }
            }
        }

        private void SetupGMCM()
        {
            var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
            {
                Monitor.Log("Generic Mod Config Menu not found. Config can only be edited via config.json", LogLevel.Info);
                return;
            }

            // Store reference for hotkey use
            gmcmApi = configMenu;

            // Register mod
            configMenu.Register(
                mod: this.ModManifest,
                reset: () => Config = new ModConfig(),
                save: () => this.Helper.WriteConfig(Config),
                titleScreenOnly: false
            );

            // === Master Toggle ===
            configMenu.AddSectionTitle(
                mod: this.ModManifest,
                text: () => "Master Toggle"
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Mod Enabled",
                tooltip: () => "Enable or disable all cheats.",
                getValue: () => Config.ModEnabled,
                setValue: value => Config.ModEnabled = value
            );

            configMenu.AddKeybindList(
                mod: this.ModManifest,
                name: () => "Open Menu Hotkey",
                tooltip: () => "Press this key to open this config menu directly.",
                getValue: () => Config.OpenMenuKey,
                setValue: value => Config.OpenMenuKey = value
            );

            // === Movement & Speed ===
            configMenu.AddSectionTitle(
                mod: this.ModManifest,
                text: () => "Movement & Speed"
            );

            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => "Speed Multiplier",
                tooltip: () => "Movement speed multiplier. 1.0 = normal, 2.0 = 2x speed, 10.0 = 10x speed.",
                getValue: () => Config.SpeedMultiplier,
                setValue: value => Config.SpeedMultiplier = value,
                min: 0.5f,
                max: 20f,
                interval: 0.5f
            );

            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => "Added Speed Bonus",
                tooltip: () => "Flat speed bonus added to base movement. Like having permanent speed buff.",
                getValue: () => Config.AddedSpeedBonus,
                setValue: value => Config.AddedSpeedBonus = value,
                min: 0f,
                max: 20f,
                interval: 0.5f
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "No Clip",
                tooltip: () => "Walk through walls, buildings, and all obstacles.",
                getValue: () => Config.NoClip,
                setValue: value => Config.NoClip = value
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Always Run",
                tooltip: () => "Always running, never walking.",
                getValue: () => Config.AlwaysRun,
                setValue: value => Config.AlwaysRun = value
            );

            // === Health & Stamina ===
            configMenu.AddSectionTitle(
                mod: this.ModManifest,
                text: () => "Health & Stamina"
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Infinite Stamina",
                tooltip: () => "Never get tired. Stamina always stays at max.",
                getValue: () => Config.InfiniteStamina,
                setValue: value => Config.InfiniteStamina = value
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Infinite Health (Invincibility)",
                tooltip: () => "Never die. Health always stays at max. Complete invincibility!",
                getValue: () => Config.InfiniteHealth,
                setValue: value => Config.InfiniteHealth = value
            );

            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => "Max Stamina Override",
                tooltip: () => "Override max stamina. 0 = use default.",
                getValue: () => Config.MaxStaminaOverride,
                setValue: value => Config.MaxStaminaOverride = value,
                min: 0,
                max: 10000,
                interval: 50
            );

            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => "Max Health Override",
                tooltip: () => "Override max health. 0 = use default.",
                getValue: () => Config.MaxHealthOverride,
                setValue: value => Config.MaxHealthOverride = value,
                min: 0,
                max: 10000,
                interval: 10
            );

            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => "Stamina Regen Per Second",
                tooltip: () => "How much stamina regenerates every second. 0 = none.",
                getValue: () => Config.StaminaRegenPerSecond,
                setValue: value => Config.StaminaRegenPerSecond = value,
                min: 0f,
                max: 100f,
                interval: 1f
            );

            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => "Health Regen Per Second",
                tooltip: () => "How much health regenerates every second. 0 = none.",
                getValue: () => Config.HealthRegenPerSecond,
                setValue: value => Config.HealthRegenPerSecond = value,
                min: 0f,
                max: 100f,
                interval: 1f
            );

            // === Combat ===
            configMenu.AddSectionTitle(
                mod: this.ModManifest,
                text: () => "Combat"
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "One Hit Kill",
                tooltip: () => "All enemies die in one hit.",
                getValue: () => Config.OneHitKill,
                setValue: value => Config.OneHitKill = value
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "100% Critical Chance",
                tooltip: () => "All attacks are critical hits.",
                getValue: () => Config.AlwaysCrit,
                setValue: value => Config.AlwaysCrit = value
            );

            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => "Damage Multiplier",
                tooltip: () => "Multiply all weapon damage. 1.0 = normal, 5.0 = 5x damage.",
                getValue: () => Config.DamageMultiplier,
                setValue: value => Config.DamageMultiplier = value,
                min: 1f,
                max: 100f,
                interval: 1f
            );

            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => "Crit Damage Multiplier",
                tooltip: () => "Multiply critical hit damage. 1.0 = normal.",
                getValue: () => Config.CritDamageMultiplier,
                setValue: value => Config.CritDamageMultiplier = value,
                min: 1f,
                max: 100f,
                interval: 1f
            );

            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => "Added Attack",
                tooltip: () => "Flat bonus attack points.",
                getValue: () => Config.AddedAttack,
                setValue: value => Config.AddedAttack = value,
                min: 0,
                max: 500,
                interval: 5
            );

            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => "Added Defense",
                tooltip: () => "Flat bonus defense points.",
                getValue: () => Config.AddedDefense,
                setValue: value => Config.AddedDefense = value,
                min: 0,
                max: 500,
                interval: 5
            );

            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => "Added Immunity",
                tooltip: () => "Flat bonus immunity points.",
                getValue: () => Config.AddedImmunity,
                setValue: value => Config.AddedImmunity = value,
                min: 0,
                max: 100,
                interval: 1
            );

            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => "Invincibility Duration (ms)",
                tooltip: () => "How long you're invincible after getting hit. Default 1200ms.",
                getValue: () => Config.InvincibilityDuration,
                setValue: value => Config.InvincibilityDuration = value,
                min: 0,
                max: 10000,
                interval: 100
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "No Monster Spawns",
                tooltip: () => "Removes all monsters from the current location.",
                getValue: () => Config.NoMonsterSpawns,
                setValue: value => Config.NoMonsterSpawns = value
            );

            // === Tools & Farming ===
            configMenu.AddSectionTitle(
                mod: this.ModManifest,
                text: () => "Tools & Farming"
            );

            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => "Tool Area Multiplier",
                tooltip: () => "Multiply the area affected by hoe/watering can. 1 = normal, 3 = 3x3, 5 = 5x5.",
                getValue: () => Config.ToolAreaMultiplier,
                setValue: value => Config.ToolAreaMultiplier = value,
                min: 1,
                max: 11,
                interval: 2
            );

            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => "Tool Power Override",
                tooltip: () => "Force tool charge level. -1 = normal, 0-5 = power level.",
                getValue: () => Config.ToolPowerOverride,
                setValue: value => Config.ToolPowerOverride = value,
                min: -1,
                max: 5
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "No Tool Stamina Cost",
                tooltip: () => "Using tools doesn't cost stamina.",
                getValue: () => Config.NoToolStaminaCost,
                setValue: value => Config.NoToolStaminaCost = value
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Infinite Water",
                tooltip: () => "Watering can never runs out.",
                getValue: () => Config.InfiniteWater,
                setValue: value => Config.InfiniteWater = value
            );

            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => "Axe Power Bonus",
                tooltip: () => "Additional power for axe. Makes chopping easier.",
                getValue: () => Config.AxePowerBonus,
                setValue: value => Config.AxePowerBonus = value,
                min: 0,
                max: 10
            );

            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => "Pickaxe Power Bonus",
                tooltip: () => "Additional power for pickaxe. Makes mining easier.",
                getValue: () => Config.PickaxePowerBonus,
                setValue: value => Config.PickaxePowerBonus = value,
                min: 0,
                max: 10
            );

            // === Item Pickup & Inventory ===
            configMenu.AddSectionTitle(
                mod: this.ModManifest,
                text: () => "Items & Inventory"
            );

            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => "Magnetic Radius Multiplier",
                tooltip: () => "Item pickup range multiplier. 1.0 = normal, 5.0 = 5x range.",
                getValue: () => Config.MagneticRadiusMultiplier,
                setValue: value => Config.MagneticRadiusMultiplier = value,
                min: 1f,
                max: 50f,
                interval: 1f
            );

            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => "Added Magnetic Radius",
                tooltip: () => "Flat bonus to pickup range in pixels. 128 = 2 tiles.",
                getValue: () => Config.AddedMagneticRadius,
                setValue: value => Config.AddedMagneticRadius = value,
                min: 0,
                max: 2000,
                interval: 64
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Infinite Items",
                tooltip: () => "Items don't get consumed when used. (Experimental)",
                getValue: () => Config.InfiniteItems,
                setValue: value => Config.InfiniteItems = value
            );

            // === Skills & Levels ===
            configMenu.AddSectionTitle(
                mod: this.ModManifest,
                text: () => "Skills & Levels"
            );

            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => "XP Multiplier",
                tooltip: () => "Multiply all XP gains. 1.0 = normal, 100 = 100x XP, 1000 = instant level ups!",
                getValue: () => Config.XPMultiplier,
                setValue: value => Config.XPMultiplier = value,
                min: 1f,
                max: 1000f,
                interval: 10f
            );

            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => "Farming Level Override",
                tooltip: () => "Force farming level. -1 = normal.",
                getValue: () => Config.FarmingLevelOverride,
                setValue: value => Config.FarmingLevelOverride = value,
                min: -1,
                max: 20
            );

            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => "Mining Level Override",
                tooltip: () => "Force mining level. -1 = normal.",
                getValue: () => Config.MiningLevelOverride,
                setValue: value => Config.MiningLevelOverride = value,
                min: -1,
                max: 20
            );

            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => "Foraging Level Override",
                tooltip: () => "Force foraging level. -1 = normal.",
                getValue: () => Config.ForagingLevelOverride,
                setValue: value => Config.ForagingLevelOverride = value,
                min: -1,
                max: 20
            );

            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => "Fishing Level Override",
                tooltip: () => "Force fishing level. -1 = normal.",
                getValue: () => Config.FishingLevelOverride,
                setValue: value => Config.FishingLevelOverride = value,
                min: -1,
                max: 20
            );

            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => "Combat Level Override",
                tooltip: () => "Force combat level. -1 = normal.",
                getValue: () => Config.CombatLevelOverride,
                setValue: value => Config.CombatLevelOverride = value,
                min: -1,
                max: 20
            );

            // === Luck ===
            configMenu.AddSectionTitle(
                mod: this.ModManifest,
                text: () => "Luck"
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Always Max Luck",
                tooltip: () => "Every day is the luckiest day possible. Applies immediately.",
                getValue: () => Config.AlwaysMaxLuck,
                setValue: value => Config.AlwaysMaxLuck = value
            );

            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => "Daily Luck Override",
                tooltip: () => "Set exact daily luck. -1.0 = disabled, range -0.1 to 0.12. Applies immediately.",
                getValue: () => Config.DailyLuckOverride,
                setValue: value => Config.DailyLuckOverride = value,
                min: -1f,
                max: 0.12f,
                interval: 0.01f
            );

            // === Fishing ===
            configMenu.AddSectionTitle(
                mod: this.ModManifest,
                text: () => "Fishing"
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Instant Fish Bite",
                tooltip: () => "Fish bite immediately when you cast.",
                getValue: () => Config.InstantFishBite,
                setValue: value => Config.InstantFishBite = value
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Instant Catch",
                tooltip: () => "Skip the fishing minigame entirely.",
                getValue: () => Config.InstantCatch,
                setValue: value => Config.InstantCatch = value
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Max Fish Quality",
                tooltip: () => "All caught fish are iridium quality.",
                getValue: () => Config.MaxFishQuality,
                setValue: value => Config.MaxFishQuality = value
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Always Find Treasure",
                tooltip: () => "Always find treasure when fishing.",
                getValue: () => Config.AlwaysFindTreasure,
                setValue: value => Config.AlwaysFindTreasure = value
            );

            // === Quality & Prices ===
            configMenu.AddSectionTitle(
                mod: this.ModManifest,
                text: () => "Quality & Prices"
            );

            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => "Force Harvest Quality",
                tooltip: () => "Force quality of all harvested crops. -1=disabled, 0=normal, 1=silver, 2=gold, 4=iridium.",
                getValue: () => Config.ForceHarvestQuality,
                setValue: value => Config.ForceHarvestQuality = value,
                min: -1,
                max: 4
            );

            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => "Force Forage Quality",
                tooltip: () => "Force quality of all foraged items. -1=disabled, 0=normal, 1=silver, 2=gold, 4=iridium. Only affects forageables that naturally have quality.",
                getValue: () => Config.ForceForageQuality,
                setValue: value => Config.ForceForageQuality = value,
                min: -1,
                max: 4
            );

            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => "Sell Price Multiplier",
                tooltip: () => "Multiply prices when selling. 1.0 = normal, 2.0 = 2x profit.",
                getValue: () => Config.SellPriceMultiplier,
                setValue: value => Config.SellPriceMultiplier = value,
                min: 1f,
                max: 100f,
                interval: 0.5f
            );

            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => "Buy Price Multiplier",
                tooltip: () => "Multiply prices when buying. 1.0 = normal, 0.5 = half price, 0 = free.",
                getValue: () => Config.BuyPriceMultiplier,
                setValue: value => Config.BuyPriceMultiplier = value,
                min: 0f,
                max: 2f,
                interval: 0.1f
            );

            // === Relationships ===
            configMenu.AddSectionTitle(
                mod: this.ModManifest,
                text: () => "Relationships"
            );

            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => "Friendship Multiplier",
                tooltip: () => "Multiply friendship gains. 1.0 = normal, 10.0 = 10x faster.",
                getValue: () => Config.FriendshipMultiplier,
                setValue: value => Config.FriendshipMultiplier = value,
                min: 1f,
                max: 100f,
                interval: 1f
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "No Friendship Decay",
                tooltip: () => "Friendship never decreases.",
                getValue: () => Config.NoFriendshipDecay,
                setValue: value => Config.NoFriendshipDecay = value
            );

            // === Time ===
            configMenu.AddSectionTitle(
                mod: this.ModManifest,
                text: () => "Time"
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Freeze Time",
                tooltip: () => "Time never passes.",
                getValue: () => Config.FreezeTime,
                setValue: value => Config.FreezeTime = value
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Freeze Time Indoors",
                tooltip: () => "Time stops when you're inside buildings.",
                getValue: () => Config.FreezeTimeIndoors,
                setValue: value => Config.FreezeTimeIndoors = value
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Never Pass Out",
                tooltip: () => "Stay awake past 2am without passing out.",
                getValue: () => Config.NeverPassOut,
                setValue: value => Config.NeverPassOut = value
            );

            // === Misc ===
            configMenu.AddSectionTitle(
                mod: this.ModManifest,
                text: () => "Miscellaneous"
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Max Animal Happiness",
                tooltip: () => "All farm animals are always at max happiness and friendship.",
                getValue: () => Config.MaxAnimalHappiness,
                setValue: value => Config.MaxAnimalHappiness = value
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Crops Never Die",
                tooltip: () => "Crops survive season changes and lack of water.",
                getValue: () => Config.CropsNeverDie,
                setValue: value => Config.CropsNeverDie = value
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Instant Crop Growth",
                tooltip: () => "All crops across all locations grow to full harvest in real-time.",
                getValue: () => Config.InstantCropGrowth,
                setValue: value => Config.InstantCropGrowth = value
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Instant Tree Growth",
                tooltip: () => "All regular trees (oak, maple, pine, etc.) grow to full size in real-time.",
                getValue: () => Config.InstantTreeGrowth,
                setValue: value => Config.InstantTreeGrowth = value
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Instant Fruit Tree Growth",
                tooltip: () => "All fruit trees grow to full maturity in real-time.",
                getValue: () => Config.InstantFruitTreeGrowth,
                setValue: value => Config.InstantFruitTreeGrowth = value
            );

            Monitor.Log("Generic Mod Config Menu integration complete!", LogLevel.Debug);
        }
    }
}
