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

            Monitor.Log("[PlayerCheats] Loaded! Configure in GMCM.", LogLevel.Info);
        }

        /*********
        ** Private Methods
        *********/
        private void ApplyPatches()
        {
            if (harmony == null) return;

            // === Player Damage (Invincibility) ===
            harmony.Patch(
                original: AccessTools.Method(typeof(Farmer), nameof(Farmer.takeDamage)),
                prefix: new HarmonyMethod(typeof(PlayerPatches), nameof(PlayerPatches.Farmer_TakeDamage_Prefix))
            );

            // === Movement Speed ===
            harmony.Patch(
                original: AccessTools.Method(typeof(Farmer), nameof(Farmer.getMovementSpeed)),
                postfix: new HarmonyMethod(typeof(PlayerPatches), nameof(PlayerPatches.Farmer_GetMovementSpeed_Postfix))
            );

            // === Magnetic Radius ===
            harmony.Patch(
                original: AccessTools.PropertyGetter(typeof(Farmer), nameof(Farmer.MagneticRadius)),
                postfix: new HarmonyMethod(typeof(PlayerPatches), nameof(PlayerPatches.Farmer_MagneticRadius_Postfix))
            );

            // === Stamina Consumption ===
            harmony.Patch(
                original: AccessTools.PropertySetter(typeof(Farmer), nameof(Farmer.stamina)),
                prefix: new HarmonyMethod(typeof(PlayerPatches), nameof(PlayerPatches.Farmer_Stamina_Prefix))
            );

            // === Monster One Hit Kill ===
            harmony.Patch(
                original: AccessTools.Method(typeof(Monster), nameof(Monster.takeDamage), new Type[] { typeof(int), typeof(int), typeof(int), typeof(bool), typeof(double), typeof(Farmer) }),
                prefix: new HarmonyMethod(typeof(PlayerPatches), nameof(PlayerPatches.Monster_TakeDamage_Prefix))
            );

            // === Weapon Critical Chance ===
            harmony.Patch(
                original: AccessTools.Method(typeof(MeleeWeapon), nameof(MeleeWeapon.DoDamage)),
                prefix: new HarmonyMethod(typeof(PlayerPatches), nameof(PlayerPatches.MeleeWeapon_DoDamage_Prefix))
            );

            // === Tool Stamina Cost ===
            harmony.Patch(
                original: AccessTools.Method(typeof(Tool), nameof(Tool.DoFunction)),
                prefix: new HarmonyMethod(typeof(PlayerPatches), nameof(PlayerPatches.Tool_DoFunction_Prefix))
            );

            // === Hoe/Watering Can Area ===
            harmony.Patch(
                original: AccessTools.Method(typeof(Tool), "tilesAffected", new Type[] { typeof(Vector2), typeof(int), typeof(Farmer) }),
                postfix: new HarmonyMethod(typeof(PlayerPatches), nameof(PlayerPatches.Tool_TilesAffected_Postfix))
            );

            // === Watering Can Infinite Water ===
            harmony.Patch(
                original: AccessTools.PropertyGetter(typeof(WateringCan), nameof(WateringCan.WaterLeft)),
                postfix: new HarmonyMethod(typeof(PlayerPatches), nameof(PlayerPatches.WateringCan_WaterLeft_Postfix))
            );

            // === Skill Levels ===
            harmony.Patch(
                original: AccessTools.PropertyGetter(typeof(Farmer), nameof(Farmer.FarmingLevel)),
                postfix: new HarmonyMethod(typeof(PlayerPatches), nameof(PlayerPatches.Farmer_FarmingLevel_Postfix))
            );
            harmony.Patch(
                original: AccessTools.PropertyGetter(typeof(Farmer), nameof(Farmer.MiningLevel)),
                postfix: new HarmonyMethod(typeof(PlayerPatches), nameof(PlayerPatches.Farmer_MiningLevel_Postfix))
            );
            harmony.Patch(
                original: AccessTools.PropertyGetter(typeof(Farmer), nameof(Farmer.ForagingLevel)),
                postfix: new HarmonyMethod(typeof(PlayerPatches), nameof(PlayerPatches.Farmer_ForagingLevel_Postfix))
            );
            harmony.Patch(
                original: AccessTools.PropertyGetter(typeof(Farmer), nameof(Farmer.FishingLevel)),
                postfix: new HarmonyMethod(typeof(PlayerPatches), nameof(PlayerPatches.Farmer_FishingLevel_Postfix))
            );
            harmony.Patch(
                original: AccessTools.PropertyGetter(typeof(Farmer), nameof(Farmer.CombatLevel)),
                postfix: new HarmonyMethod(typeof(PlayerPatches), nameof(PlayerPatches.Farmer_CombatLevel_Postfix))
            );

            // === XP Gain ===
            harmony.Patch(
                original: AccessTools.Method(typeof(Farmer), nameof(Farmer.gainExperience)),
                prefix: new HarmonyMethod(typeof(PlayerPatches), nameof(PlayerPatches.Farmer_GainExperience_Prefix))
            );

            // === Time Freeze ===
            harmony.Patch(
                original: AccessTools.Method(typeof(Game1), nameof(Game1.performTenMinuteClockUpdate)),
                prefix: new HarmonyMethod(typeof(PlayerPatches), nameof(PlayerPatches.Game1_PerformTenMinuteClockUpdate_Prefix))
            );

            // === Passout Prevention ===
            harmony.Patch(
                original: AccessTools.Method(typeof(Farmer), nameof(Farmer.passOutFromTired)),
                prefix: new HarmonyMethod(typeof(PlayerPatches), nameof(PlayerPatches.Farmer_PassOutFromTired_Prefix))
            );

            // === NoClip (Collision) ===
            harmony.Patch(
                original: AccessTools.PropertyGetter(typeof(Farmer), nameof(Farmer.ignoreCollisions)),
                postfix: new HarmonyMethod(typeof(PlayerPatches), nameof(PlayerPatches.Farmer_IgnoreCollisions_Postfix))
            );

            // === Friendship Gain ===
            harmony.Patch(
                original: AccessTools.Method(typeof(Farmer), nameof(Farmer.changeFriendship)),
                prefix: new HarmonyMethod(typeof(PlayerPatches), nameof(PlayerPatches.Farmer_ChangeFriendship_Prefix))
            );

            // === Harvest Quality ===
            harmony.Patch(
                original: AccessTools.Method(typeof(Crop), nameof(Crop.harvest)),
                prefix: new HarmonyMethod(typeof(PlayerPatches), nameof(PlayerPatches.Crop_Harvest_Prefix))
            );

            // === Fishing Rod (Instant Bite) ===
            harmony.Patch(
                original: AccessTools.Method(typeof(FishingRod), nameof(FishingRod.DoFunction)),
                postfix: new HarmonyMethod(typeof(PlayerPatches), nameof(PlayerPatches.FishingRod_DoFunction_Postfix))
            );

            // === Max Health/Stamina Override ===
            harmony.Patch(
                original: AccessTools.PropertyGetter(typeof(Farmer), nameof(Farmer.MaxStamina)),
                postfix: new HarmonyMethod(typeof(PlayerPatches), nameof(PlayerPatches.Farmer_MaxStamina_Postfix))
            );

            // === Defense/Attack/Immunity ===
            harmony.Patch(
                original: AccessTools.PropertyGetter(typeof(Farmer), nameof(Farmer.Attack)),
                postfix: new HarmonyMethod(typeof(PlayerPatches), nameof(PlayerPatches.Farmer_Attack_Postfix))
            );
            harmony.Patch(
                original: AccessTools.PropertyGetter(typeof(Farmer), "Immunity"),
                postfix: new HarmonyMethod(typeof(PlayerPatches), nameof(PlayerPatches.Farmer_Immunity_Postfix))
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
        }

        private void OnDayStarted(object? sender, DayStartedEventArgs e)
        {
            if (!Config.ModEnabled)
                return;

            var player = Game1.player;
            if (player == null) return;

            // Max luck day
            if (Config.AlwaysMaxLuck)
            {
                Game1.player.team.sharedDailyLuck.Value = 0.12f; // Max luck
            }
            else if (Config.DailyLuckOverride >= -0.1f && Config.DailyLuckOverride <= 0.1f)
            {
                Game1.player.team.sharedDailyLuck.Value = Config.DailyLuckOverride;
            }

            // Instant crop growth
            if (Config.InstantCropGrowth)
            {
                var farm = Game1.getFarm();
                if (farm != null)
                {
                    foreach (var pair in farm.terrainFeatures.Pairs)
                    {
                        if (pair.Value is HoeDirt dirt && dirt.crop != null)
                        {
                            dirt.crop.growCompletely();
                        }
                    }
                }
            }
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

        private void SetupGMCM()
        {
            var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
            {
                Monitor.Log("Generic Mod Config Menu not found. Config can only be edited via config.json", LogLevel.Info);
                return;
            }

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
                text: () => "⚙️ Master Toggle"
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Mod Enabled",
                tooltip: () => "Enable or disable all cheats.",
                getValue: () => Config.ModEnabled,
                setValue: value => Config.ModEnabled = value
            );

            // === Movement & Speed ===
            configMenu.AddSectionTitle(
                mod: this.ModManifest,
                text: () => "🏃 Movement & Speed"
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
                text: () => "❤️ Health & Stamina"
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
                text: () => "⚔️ Combat"
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
                text: () => "🔧 Tools & Farming"
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
                text: () => "🎒 Items & Inventory"
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
                text: () => "📊 Skills & Levels"
            );

            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => "XP Multiplier",
                tooltip: () => "Multiply all XP gains. 1.0 = normal, 10.0 = 10x XP.",
                getValue: () => Config.XPMultiplier,
                setValue: value => Config.XPMultiplier = value,
                min: 1f,
                max: 100f,
                interval: 1f
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
                text: () => "🍀 Luck"
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Always Max Luck",
                tooltip: () => "Every day is the luckiest day possible.",
                getValue: () => Config.AlwaysMaxLuck,
                setValue: value => Config.AlwaysMaxLuck = value
            );

            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => "Daily Luck Override",
                tooltip: () => "Set exact daily luck. -1.0 = disabled, range -0.1 to 0.1",
                getValue: () => Config.DailyLuckOverride,
                setValue: value => Config.DailyLuckOverride = value,
                min: -1f,
                max: 0.12f,
                interval: 0.01f
            );

            // === Fishing ===
            configMenu.AddSectionTitle(
                mod: this.ModManifest,
                text: () => "🎣 Fishing"
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
                text: () => "💰 Quality & Prices"
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
                text: () => "💕 Relationships"
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
                text: () => "⏰ Time"
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
                text: () => "✨ Miscellaneous"
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
                tooltip: () => "All crops grow to full harvest at the start of each day.",
                getValue: () => Config.InstantCropGrowth,
                setValue: value => Config.InstantCropGrowth = value
            );

            Monitor.Log("Generic Mod Config Menu integration complete!", LogLevel.Debug);
        }
    }
}
