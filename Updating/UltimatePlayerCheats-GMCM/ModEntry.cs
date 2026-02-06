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

            // === Harvest Quality === (REMOVED - requires transpiler, too complex)
            // ForceHarvestQuality is not currently implemented

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

            // === Instant Catch & Always Find Treasure ===
            TryPatch(
                original: AccessTools.Method(typeof(FishingRod), nameof(FishingRod.startMinigameEndFunction)),
                prefix: new HarmonyMethod(typeof(PlayerPatches), nameof(PlayerPatches.FishingRod_StartMinigameEndFunction_Prefix)),
                description: "FishingRod.startMinigameEndFunction"
            );

            // === BobberBar Treasure Override ===
            TryPatch(
                original: AccessTools.Constructor(typeof(BobberBar), new Type[] { typeof(string), typeof(float), typeof(bool), typeof(List<string>), typeof(string), typeof(bool), typeof(string), typeof(bool) }),
                postfix: new HarmonyMethod(typeof(PlayerPatches), nameof(PlayerPatches.BobberBar_Constructor_Postfix)),
                description: "BobberBar constructor"
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

            // Helper for translations
            var i18n = this.Helper.Translation;

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
                text: () => i18n.Get("section.master")
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => i18n.Get("option.modEnabled.name"),
                tooltip: () => i18n.Get("option.modEnabled.tooltip"),
                getValue: () => Config.ModEnabled,
                setValue: value => Config.ModEnabled = value
            );

            configMenu.AddKeybindList(
                mod: this.ModManifest,
                name: () => i18n.Get("option.openMenuKey.name"),
                tooltip: () => i18n.Get("option.openMenuKey.tooltip"),
                getValue: () => Config.OpenMenuKey,
                setValue: value => Config.OpenMenuKey = value
            );

            // === Movement & Speed ===
            configMenu.AddSectionTitle(
                mod: this.ModManifest,
                text: () => i18n.Get("section.movement")
            );

            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => i18n.Get("option.speedMultiplier.name"),
                tooltip: () => i18n.Get("option.speedMultiplier.tooltip"),
                getValue: () => Config.SpeedMultiplier,
                setValue: value => Config.SpeedMultiplier = value,
                min: 0.5f,
                max: 20f,
                interval: 0.5f
            );

            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => i18n.Get("option.addedSpeedBonus.name"),
                tooltip: () => i18n.Get("option.addedSpeedBonus.tooltip"),
                getValue: () => Config.AddedSpeedBonus,
                setValue: value => Config.AddedSpeedBonus = value,
                min: 0f,
                max: 20f,
                interval: 0.5f
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => i18n.Get("option.noClip.name"),
                tooltip: () => i18n.Get("option.noClip.tooltip"),
                getValue: () => Config.NoClip,
                setValue: value => Config.NoClip = value
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => i18n.Get("option.alwaysRun.name"),
                tooltip: () => i18n.Get("option.alwaysRun.tooltip"),
                getValue: () => Config.AlwaysRun,
                setValue: value => Config.AlwaysRun = value
            );

            // === Health & Stamina ===
            configMenu.AddSectionTitle(
                mod: this.ModManifest,
                text: () => i18n.Get("section.healthStamina")
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => i18n.Get("option.infiniteStamina.name"),
                tooltip: () => i18n.Get("option.infiniteStamina.tooltip"),
                getValue: () => Config.InfiniteStamina,
                setValue: value => Config.InfiniteStamina = value
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => i18n.Get("option.infiniteHealth.name"),
                tooltip: () => i18n.Get("option.infiniteHealth.tooltip"),
                getValue: () => Config.InfiniteHealth,
                setValue: value => Config.InfiniteHealth = value
            );

            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => i18n.Get("option.maxStaminaOverride.name"),
                tooltip: () => i18n.Get("option.maxStaminaOverride.tooltip"),
                getValue: () => Config.MaxStaminaOverride,
                setValue: value => Config.MaxStaminaOverride = value,
                min: 0,
                max: 10000,
                interval: 50
            );

            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => i18n.Get("option.maxHealthOverride.name"),
                tooltip: () => i18n.Get("option.maxHealthOverride.tooltip"),
                getValue: () => Config.MaxHealthOverride,
                setValue: value => Config.MaxHealthOverride = value,
                min: 0,
                max: 10000,
                interval: 10
            );

            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => i18n.Get("option.staminaRegenPerSecond.name"),
                tooltip: () => i18n.Get("option.staminaRegenPerSecond.tooltip"),
                getValue: () => Config.StaminaRegenPerSecond,
                setValue: value => Config.StaminaRegenPerSecond = value,
                min: 0f,
                max: 100f,
                interval: 1f
            );

            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => i18n.Get("option.healthRegenPerSecond.name"),
                tooltip: () => i18n.Get("option.healthRegenPerSecond.tooltip"),
                getValue: () => Config.HealthRegenPerSecond,
                setValue: value => Config.HealthRegenPerSecond = value,
                min: 0f,
                max: 100f,
                interval: 1f
            );

            // === Combat ===
            configMenu.AddSectionTitle(
                mod: this.ModManifest,
                text: () => i18n.Get("section.combat")
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => i18n.Get("option.oneHitKill.name"),
                tooltip: () => i18n.Get("option.oneHitKill.tooltip"),
                getValue: () => Config.OneHitKill,
                setValue: value => Config.OneHitKill = value
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => i18n.Get("option.alwaysCrit.name"),
                tooltip: () => i18n.Get("option.alwaysCrit.tooltip"),
                getValue: () => Config.AlwaysCrit,
                setValue: value => Config.AlwaysCrit = value
            );

            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => i18n.Get("option.damageMultiplier.name"),
                tooltip: () => i18n.Get("option.damageMultiplier.tooltip"),
                getValue: () => Config.DamageMultiplier,
                setValue: value => Config.DamageMultiplier = value,
                min: 1f,
                max: 100f,
                interval: 1f
            );

            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => i18n.Get("option.critDamageMultiplier.name"),
                tooltip: () => i18n.Get("option.critDamageMultiplier.tooltip"),
                getValue: () => Config.CritDamageMultiplier,
                setValue: value => Config.CritDamageMultiplier = value,
                min: 1f,
                max: 100f,
                interval: 1f
            );

            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => i18n.Get("option.addedAttack.name"),
                tooltip: () => i18n.Get("option.addedAttack.tooltip"),
                getValue: () => Config.AddedAttack,
                setValue: value => Config.AddedAttack = value,
                min: 0,
                max: 500,
                interval: 5
            );

            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => i18n.Get("option.addedDefense.name"),
                tooltip: () => i18n.Get("option.addedDefense.tooltip"),
                getValue: () => Config.AddedDefense,
                setValue: value => Config.AddedDefense = value,
                min: 0,
                max: 500,
                interval: 5
            );

            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => i18n.Get("option.addedImmunity.name"),
                tooltip: () => i18n.Get("option.addedImmunity.tooltip"),
                getValue: () => Config.AddedImmunity,
                setValue: value => Config.AddedImmunity = value,
                min: 0,
                max: 100,
                interval: 1
            );

            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => i18n.Get("option.invincibilityDuration.name"),
                tooltip: () => i18n.Get("option.invincibilityDuration.tooltip"),
                getValue: () => Config.InvincibilityDuration,
                setValue: value => Config.InvincibilityDuration = value,
                min: 0,
                max: 10000,
                interval: 100
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => i18n.Get("option.noMonsterSpawns.name"),
                tooltip: () => i18n.Get("option.noMonsterSpawns.tooltip"),
                getValue: () => Config.NoMonsterSpawns,
                setValue: value => Config.NoMonsterSpawns = value
            );

            // === Tools & Farming ===
            configMenu.AddSectionTitle(
                mod: this.ModManifest,
                text: () => i18n.Get("section.tools")
            );

            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => i18n.Get("option.toolAreaMultiplier.name"),
                tooltip: () => i18n.Get("option.toolAreaMultiplier.tooltip"),
                getValue: () => Config.ToolAreaMultiplier,
                setValue: value => Config.ToolAreaMultiplier = value,
                min: 1,
                max: 11,
                interval: 2
            );

            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => i18n.Get("option.toolPowerOverride.name"),
                tooltip: () => i18n.Get("option.toolPowerOverride.tooltip"),
                getValue: () => Config.ToolPowerOverride,
                setValue: value => Config.ToolPowerOverride = value,
                min: -1,
                max: 5
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => i18n.Get("option.noToolStaminaCost.name"),
                tooltip: () => i18n.Get("option.noToolStaminaCost.tooltip"),
                getValue: () => Config.NoToolStaminaCost,
                setValue: value => Config.NoToolStaminaCost = value
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => i18n.Get("option.infiniteWater.name"),
                tooltip: () => i18n.Get("option.infiniteWater.tooltip"),
                getValue: () => Config.InfiniteWater,
                setValue: value => Config.InfiniteWater = value
            );

            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => i18n.Get("option.axePowerBonus.name"),
                tooltip: () => i18n.Get("option.axePowerBonus.tooltip"),
                getValue: () => Config.AxePowerBonus,
                setValue: value => Config.AxePowerBonus = value,
                min: 0,
                max: 10
            );

            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => i18n.Get("option.pickaxePowerBonus.name"),
                tooltip: () => i18n.Get("option.pickaxePowerBonus.tooltip"),
                getValue: () => Config.PickaxePowerBonus,
                setValue: value => Config.PickaxePowerBonus = value,
                min: 0,
                max: 10
            );

            // === Item Pickup & Inventory ===
            configMenu.AddSectionTitle(
                mod: this.ModManifest,
                text: () => i18n.Get("section.items")
            );

            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => i18n.Get("option.magneticRadiusMultiplier.name"),
                tooltip: () => i18n.Get("option.magneticRadiusMultiplier.tooltip"),
                getValue: () => Config.MagneticRadiusMultiplier,
                setValue: value => Config.MagneticRadiusMultiplier = value,
                min: 1f,
                max: 50f,
                interval: 1f
            );

            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => i18n.Get("option.addedMagneticRadius.name"),
                tooltip: () => i18n.Get("option.addedMagneticRadius.tooltip"),
                getValue: () => Config.AddedMagneticRadius,
                setValue: value => Config.AddedMagneticRadius = value,
                min: 0,
                max: 2000,
                interval: 64
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => i18n.Get("option.infiniteItems.name"),
                tooltip: () => i18n.Get("option.infiniteItems.tooltip"),
                getValue: () => Config.InfiniteItems,
                setValue: value => Config.InfiniteItems = value
            );

            // === Skills & Levels ===
            configMenu.AddSectionTitle(
                mod: this.ModManifest,
                text: () => i18n.Get("section.skills")
            );

            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => i18n.Get("option.xpMultiplier.name"),
                tooltip: () => i18n.Get("option.xpMultiplier.tooltip"),
                getValue: () => Config.XPMultiplier,
                setValue: value => Config.XPMultiplier = value,
                min: 1f,
                max: 1000f,
                interval: 10f
            );

            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => i18n.Get("option.farmingLevelOverride.name"),
                tooltip: () => i18n.Get("option.farmingLevelOverride.tooltip"),
                getValue: () => Config.FarmingLevelOverride,
                setValue: value => Config.FarmingLevelOverride = value,
                min: -1,
                max: 20
            );

            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => i18n.Get("option.miningLevelOverride.name"),
                tooltip: () => i18n.Get("option.miningLevelOverride.tooltip"),
                getValue: () => Config.MiningLevelOverride,
                setValue: value => Config.MiningLevelOverride = value,
                min: -1,
                max: 20
            );

            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => i18n.Get("option.foragingLevelOverride.name"),
                tooltip: () => i18n.Get("option.foragingLevelOverride.tooltip"),
                getValue: () => Config.ForagingLevelOverride,
                setValue: value => Config.ForagingLevelOverride = value,
                min: -1,
                max: 20
            );

            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => i18n.Get("option.fishingLevelOverride.name"),
                tooltip: () => i18n.Get("option.fishingLevelOverride.tooltip"),
                getValue: () => Config.FishingLevelOverride,
                setValue: value => Config.FishingLevelOverride = value,
                min: -1,
                max: 20
            );

            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => i18n.Get("option.combatLevelOverride.name"),
                tooltip: () => i18n.Get("option.combatLevelOverride.tooltip"),
                getValue: () => Config.CombatLevelOverride,
                setValue: value => Config.CombatLevelOverride = value,
                min: -1,
                max: 20
            );

            // === Luck ===
            configMenu.AddSectionTitle(
                mod: this.ModManifest,
                text: () => i18n.Get("section.luck")
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => i18n.Get("option.alwaysMaxLuck.name"),
                tooltip: () => i18n.Get("option.alwaysMaxLuck.tooltip"),
                getValue: () => Config.AlwaysMaxLuck,
                setValue: value => Config.AlwaysMaxLuck = value
            );

            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => i18n.Get("option.dailyLuckOverride.name"),
                tooltip: () => i18n.Get("option.dailyLuckOverride.tooltip"),
                getValue: () => Config.DailyLuckOverride,
                setValue: value => Config.DailyLuckOverride = value,
                min: -1f,
                max: 0.12f,
                interval: 0.01f
            );

            // === Fishing ===
            configMenu.AddSectionTitle(
                mod: this.ModManifest,
                text: () => i18n.Get("section.fishing")
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => i18n.Get("option.instantFishBite.name"),
                tooltip: () => i18n.Get("option.instantFishBite.tooltip"),
                getValue: () => Config.InstantFishBite,
                setValue: value => Config.InstantFishBite = value
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => i18n.Get("option.instantCatch.name"),
                tooltip: () => i18n.Get("option.instantCatch.tooltip"),
                getValue: () => Config.InstantCatch,
                setValue: value => Config.InstantCatch = value
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => i18n.Get("option.maxFishQuality.name"),
                tooltip: () => i18n.Get("option.maxFishQuality.tooltip"),
                getValue: () => Config.MaxFishQuality,
                setValue: value => Config.MaxFishQuality = value
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => i18n.Get("option.alwaysFindTreasure.name"),
                tooltip: () => i18n.Get("option.alwaysFindTreasure.tooltip"),
                getValue: () => Config.AlwaysFindTreasure,
                setValue: value => Config.AlwaysFindTreasure = value
            );

            // === Quality & Prices ===
            configMenu.AddSectionTitle(
                mod: this.ModManifest,
                text: () => i18n.Get("section.quality")
            );

            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => i18n.Get("option.forceForageQuality.name"),
                tooltip: () => i18n.Get("option.forceForageQuality.tooltip"),
                getValue: () => Config.ForceForageQuality,
                setValue: value => Config.ForceForageQuality = value,
                min: -1,
                max: 4
            );

            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => i18n.Get("option.sellPriceMultiplier.name"),
                tooltip: () => i18n.Get("option.sellPriceMultiplier.tooltip"),
                getValue: () => Config.SellPriceMultiplier,
                setValue: value => Config.SellPriceMultiplier = value,
                min: 1f,
                max: 100f,
                interval: 0.5f
            );

            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => i18n.Get("option.buyPriceMultiplier.name"),
                tooltip: () => i18n.Get("option.buyPriceMultiplier.tooltip"),
                getValue: () => Config.BuyPriceMultiplier,
                setValue: value => Config.BuyPriceMultiplier = value,
                min: 0f,
                max: 2f,
                interval: 0.1f
            );

            // === Relationships ===
            configMenu.AddSectionTitle(
                mod: this.ModManifest,
                text: () => i18n.Get("section.relationships")
            );

            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => i18n.Get("option.friendshipMultiplier.name"),
                tooltip: () => i18n.Get("option.friendshipMultiplier.tooltip"),
                getValue: () => Config.FriendshipMultiplier,
                setValue: value => Config.FriendshipMultiplier = value,
                min: 1f,
                max: 100f,
                interval: 1f
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => i18n.Get("option.noFriendshipDecay.name"),
                tooltip: () => i18n.Get("option.noFriendshipDecay.tooltip"),
                getValue: () => Config.NoFriendshipDecay,
                setValue: value => Config.NoFriendshipDecay = value
            );

            // === Time ===
            configMenu.AddSectionTitle(
                mod: this.ModManifest,
                text: () => i18n.Get("section.time")
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => i18n.Get("option.freezeTime.name"),
                tooltip: () => i18n.Get("option.freezeTime.tooltip"),
                getValue: () => Config.FreezeTime,
                setValue: value => Config.FreezeTime = value
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => i18n.Get("option.freezeTimeIndoors.name"),
                tooltip: () => i18n.Get("option.freezeTimeIndoors.tooltip"),
                getValue: () => Config.FreezeTimeIndoors,
                setValue: value => Config.FreezeTimeIndoors = value
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => i18n.Get("option.neverPassOut.name"),
                tooltip: () => i18n.Get("option.neverPassOut.tooltip"),
                getValue: () => Config.NeverPassOut,
                setValue: value => Config.NeverPassOut = value
            );

            // === Misc ===
            configMenu.AddSectionTitle(
                mod: this.ModManifest,
                text: () => i18n.Get("section.misc")
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => i18n.Get("option.maxAnimalHappiness.name"),
                tooltip: () => i18n.Get("option.maxAnimalHappiness.tooltip"),
                getValue: () => Config.MaxAnimalHappiness,
                setValue: value => Config.MaxAnimalHappiness = value
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => i18n.Get("option.cropsNeverDie.name"),
                tooltip: () => i18n.Get("option.cropsNeverDie.tooltip"),
                getValue: () => Config.CropsNeverDie,
                setValue: value => Config.CropsNeverDie = value
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => i18n.Get("option.instantCropGrowth.name"),
                tooltip: () => i18n.Get("option.instantCropGrowth.tooltip"),
                getValue: () => Config.InstantCropGrowth,
                setValue: value => Config.InstantCropGrowth = value
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => i18n.Get("option.instantTreeGrowth.name"),
                tooltip: () => i18n.Get("option.instantTreeGrowth.tooltip"),
                getValue: () => Config.InstantTreeGrowth,
                setValue: value => Config.InstantTreeGrowth = value
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => i18n.Get("option.instantFruitTreeGrowth.name"),
                tooltip: () => i18n.Get("option.instantFruitTreeGrowth.tooltip"),
                getValue: () => Config.InstantFruitTreeGrowth,
                setValue: value => Config.InstantFruitTreeGrowth = value
            );

            Monitor.Log("Generic Mod Config Menu integration complete!", LogLevel.Debug);
        }
    }
}
