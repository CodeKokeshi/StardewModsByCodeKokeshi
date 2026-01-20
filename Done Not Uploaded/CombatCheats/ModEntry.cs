#nullable enable
using System;
using System.Collections.Generic;
using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Monsters;
using StardewValley.Tools;

namespace CombatCheats
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
            
            var harmony = new Harmony(this.ModManifest.UniqueID);
            harmony.PatchAll();
            
            // Register GMCM on game launch
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            
            Monitor.Log("[CombatCheats] Loaded! All cheats are OFF by default. Configure in GMCM.", LogLevel.Info);
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

            // Register mod
            configMenu.Register(
                mod: this.ModManifest,
                reset: () => Config = new ModConfig(),
                save: () => this.Helper.WriteConfig(Config),
                titleScreenOnly: false
            );

            // === Combat Cheats Settings ===
            configMenu.AddSectionTitle(
                mod: this.ModManifest,
                text: () => "Combat Cheats"
            );

            configMenu.AddParagraph(
                mod: this.ModManifest,
                text: () => "Enable powerful combat cheats. All features are disabled by default."
            );

            // One Hit Kill
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "One Hit Kill",
                tooltip: () => "All enemies die in one hit. Makes all monsters have 1 HP when you attack them.",
                getValue: () => Config.OneHitKill,
                setValue: value => Config.OneHitKill = value
            );

            // Infinite HP
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Infinite HP",
                tooltip: () => "You take no damage from any source. Completely invincible!",
                getValue: () => Config.InfiniteHP,
                setValue: value => Config.InfiniteHP = value
            );

            // Max Drop Rate
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Max Drop Rate",
                tooltip: () => "Monsters always drop ALL their possible items. Guaranteed drops!",
                getValue: () => Config.MaxDropRate,
                setValue: value => Config.MaxDropRate = value
            );

            // 100% Crit Chance
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "100% Crit Chance",
                tooltip: () => "All weapon attacks are critical hits. Alternative to one-hit kill for more damage.",
                getValue: () => Config.HundredPercentCrit,
                setValue: value => Config.HundredPercentCrit = value
            );

            Monitor.Log("Generic Mod Config Menu integration complete!", LogLevel.Debug);
        }
    }

    /// <summary>Harmony patches for combat cheats.</summary>
    [HarmonyPatch]
    public static class CombatPatches
    {
        /// <summary>Patch to make player invincible (Infinite HP).</summary>
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Farmer), nameof(Farmer.takeDamage))]
        public static bool Farmer_takeDamage_Prefix(Farmer __instance, ref int damage)
        {
            if (!Context.IsWorldReady || __instance != Game1.player)
                return true;

            if (ModEntry.Config.InfiniteHP)
            {
                // Cancel damage completely
                damage = 0;
                ModEntry.ModMonitor.LogOnce("Infinite HP active - blocking damage", LogLevel.Trace);
                return false; // Skip original method
            }

            return true; // Run original method
        }

        /// <summary>Patch to make monsters die in one hit.</summary>
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Monster), nameof(Monster.takeDamage), new Type[] { typeof(int), typeof(int), typeof(int), typeof(bool), typeof(double), typeof(Farmer) })]
        public static void Monster_takeDamage_Prefix(Monster __instance, ref int damage)
        {
            if (!Context.IsWorldReady)
                return;

            if (ModEntry.Config.OneHitKill)
            {
                // Set monster health to 1 so any damage kills it
                if (__instance.Health > 1)
                {
                    __instance.Health = 1;
                    ModEntry.ModMonitor.LogOnce($"One Hit Kill active - set {__instance.Name} HP to 1", LogLevel.Trace);
                }
            }
        }

        /// <summary>Patch to guarantee 100% crit chance on weapons.</summary>
        [HarmonyPrefix]
        [HarmonyPatch(typeof(MeleeWeapon), nameof(MeleeWeapon.DoDamage))]
        public static void MeleeWeapon_DoDamage_Prefix(MeleeWeapon __instance)
        {
            if (!Context.IsWorldReady)
                return;

            if (ModEntry.Config.HundredPercentCrit)
            {
                // Set critChance to 1.0 (100%) before damage calculation
                __instance.critChance.Value = 1.0f;
                ModEntry.ModMonitor.LogOnce("100% Crit active - forcing crit chance to 100%", LogLevel.Trace);
            }
        }

        /// <summary>Patch to guarantee max drop rate from monsters - multiply all drops like trainers do!</summary>
        [HarmonyPrefix]
        [HarmonyPatch(typeof(GameLocation), nameof(GameLocation.monsterDrop))]
        public static void GameLocation_monsterDrop_Prefix(Monster monster, Farmer who)
        {
            if (!Context.IsWorldReady)
                return;

            if (ModEntry.Config.MaxDropRate)
            {
                try
                {
                    // Get the monster's data from DataLoader
                    var monsterData = DataLoader.Monsters(Game1.content);
                    if (monsterData.TryGetValue(monster.Name, out string? rawData))
                    {
                        // Parse monster data: format is split by '/' and drops are at index 6
                        string[] parts = rawData.Split('/');
                        if (parts.Length > 6)
                        {
                            // Drop data format: "itemId chance itemId chance ..."
                            string[] dropData = ArgUtility.SplitBySpace(parts[6]);
                            
                            // Add ALL possible drops MULTIPLE TIMES to objectsToDrop (like trainers do!)
                            // This makes monsters drop way more loot
                            for (int i = 0; i < dropData.Length; i += 2)
                            {
                                if (i + 1 < dropData.Length)
                                {
                                    string itemId = dropData[i];
                                    // Add the item 5 times for maximum loot drop!
                                    for (int j = 0; j < 5; j++)
                                    {
                                        monster.objectsToDrop.Add(itemId);
                                    }
                                }
                            }
                            
                            ModEntry.ModMonitor.LogOnce($"Max Drop Rate active - multiplied all drops 5x for {monster.Name}", LogLevel.Trace);
                        }
                    }
                }
                catch (Exception ex)
                {
                    ModEntry.ModMonitor.Log($"Error applying max drop rate: {ex.Message}", LogLevel.Error);
                }
            }
        }
    }
}
