#nullable enable
using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Monsters;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;

namespace PlayerCheats
{
    /// <summary>Harmony patches for player cheats.</summary>
    public static class PlayerPatches
    {
        /// <summary>Patch to make player invincible (Infinite HP / block all damage).</summary>
        public static bool Farmer_TakeDamage_Prefix(Farmer __instance, ref int damage, Monster damager)
        {
            if (!Context.IsWorldReady || __instance != Game1.player || !ModEntry.Config.ModEnabled)
                return true;

            if (ModEntry.Config.InfiniteHealth)
            {
                damage = 0;
                return false; // Skip original method
            }

            return true;
        }

        /// <summary>Patch to modify movement speed.</summary>
        public static void Farmer_GetMovementSpeed_Postfix(Farmer __instance, ref float __result)
        {
            if (!Context.IsWorldReady || __instance != Game1.player || !ModEntry.Config.ModEnabled)
                return;

            // Apply speed multiplier
            if (ModEntry.Config.SpeedMultiplier != 1.0f)
            {
                __result *= ModEntry.Config.SpeedMultiplier;
            }

            // Apply added speed bonus
            if (ModEntry.Config.AddedSpeedBonus > 0)
            {
                __result += ModEntry.Config.AddedSpeedBonus;
            }
        }

        /// <summary>Patch to modify magnetic radius (pickup range).</summary>
        public static void Farmer_MagneticRadius_Postfix(Farmer __instance, ref int __result)
        {
            if (!Context.IsWorldReady || __instance != Game1.player || !ModEntry.Config.ModEnabled)
                return;

            // Apply multiplier
            if (ModEntry.Config.MagneticRadiusMultiplier != 1.0f)
            {
                __result = (int)(__result * ModEntry.Config.MagneticRadiusMultiplier);
            }

            // Apply flat bonus
            if (ModEntry.Config.AddedMagneticRadius > 0)
            {
                __result += ModEntry.Config.AddedMagneticRadius;
            }
        }

        /// <summary>Patch to prevent stamina reduction (infinite stamina).</summary>
        public static bool Farmer_Stamina_Prefix(Farmer __instance, ref float value)
        {
            if (!Context.IsWorldReady || __instance != Game1.player || !ModEntry.Config.ModEnabled)
                return true;

            // If infinite stamina, prevent any decrease
            if (ModEntry.Config.InfiniteStamina && value < __instance.stamina)
            {
                return false; // Don't set lower value
            }

            return true;
        }

        /// <summary>Patch to make monsters die in one hit and apply damage multiplier.</summary>
        public static void Monster_TakeDamage_Prefix(Monster __instance, ref int damage)
        {
            if (!Context.IsWorldReady || !ModEntry.Config.ModEnabled)
                return;

            if (ModEntry.Config.OneHitKill)
            {
                damage = 999999;
                return;
            }

            // Apply damage multiplier
            if (ModEntry.Config.DamageMultiplier != 1.0f)
            {
                damage = (int)(damage * ModEntry.Config.DamageMultiplier);
            }
        }

        /// <summary>Patch to force 100% crit chance on weapons.</summary>
        public static void MeleeWeapon_DoDamage_Prefix(MeleeWeapon __instance)
        {
            if (!Context.IsWorldReady || !ModEntry.Config.ModEnabled)
                return;

            if (ModEntry.Config.AlwaysCrit)
            {
                __instance.critChance.Value = 1.0f;
            }

            if (ModEntry.Config.CritDamageMultiplier != 1.0f)
            {
                __instance.critMultiplier.Value *= ModEntry.Config.CritDamageMultiplier;
            }
        }

        /// <summary>Patch to mark tools as efficient (no stamina cost).</summary>
        public static void Tool_DoFunction_Prefix(Tool __instance, Farmer who)
        {
            if (!Context.IsWorldReady || who != Game1.player || !ModEntry.Config.ModEnabled)
                return;

            if (ModEntry.Config.NoToolStaminaCost)
            {
                __instance.IsEfficient = true;
            }

            // Apply power bonus for axe
            if (__instance is Axe axe && ModEntry.Config.AxePowerBonus > 0)
            {
                var additionalPower = AccessTools.Field(typeof(Axe), "additionalPower");
                if (additionalPower?.GetValue(axe) is Netcode.NetInt axeNetInt)
                {
                    axeNetInt.Value = ModEntry.Config.AxePowerBonus;
                }
            }

            // Apply power bonus for pickaxe
            if (__instance is Pickaxe pick && ModEntry.Config.PickaxePowerBonus > 0)
            {
                var additionalPower = AccessTools.Field(typeof(Pickaxe), "additionalPower");
                if (additionalPower?.GetValue(pick) is Netcode.NetInt pickNetInt)
                {
                    pickNetInt.Value = ModEntry.Config.PickaxePowerBonus;
                }
            }
        }

        /// <summary>Patch to expand tool area (hoe/watering can).</summary>
        public static void Tool_TilesAffected_Postfix(Tool __instance, ref List<Vector2> __result, Vector2 tileLocation, int power, Farmer who)
        {
            if (!Context.IsWorldReady || who != Game1.player || !ModEntry.Config.ModEnabled)
                return;

            if (ModEntry.Config.ToolAreaMultiplier <= 1)
                return;

            // Only apply to hoe and watering can
            if (!(__instance is Hoe || __instance is WateringCan))
                return;

            // Calculate expanded area
            int radius = (ModEntry.Config.ToolAreaMultiplier - 1) / 2;
            List<Vector2> expandedTiles = new List<Vector2>();

            for (int x = -radius; x <= radius; x++)
            {
                for (int y = -radius; y <= radius; y++)
                {
                    Vector2 tile = new Vector2(tileLocation.X + x, tileLocation.Y + y);
                    if (!__result.Contains(tile))
                    {
                        expandedTiles.Add(tile);
                    }
                }
            }

            __result.AddRange(expandedTiles);
        }

        /// <summary>Patch to give infinite water.</summary>
        public static void WateringCan_WaterLeft_Postfix(WateringCan __instance, ref int __result)
        {
            if (!Context.IsWorldReady || !ModEntry.Config.ModEnabled)
                return;

            if (ModEntry.Config.InfiniteWater)
            {
                __result = __instance.waterCanMax;
            }
        }

        /// <summary>Patch to override farming level.</summary>
        public static void Farmer_FarmingLevel_Postfix(Farmer __instance, ref int __result)
        {
            if (!Context.IsWorldReady || __instance != Game1.player || !ModEntry.Config.ModEnabled)
                return;

            if (ModEntry.Config.FarmingLevelOverride >= 0)
            {
                __result = ModEntry.Config.FarmingLevelOverride;
            }
        }

        /// <summary>Patch to override mining level.</summary>
        public static void Farmer_MiningLevel_Postfix(Farmer __instance, ref int __result)
        {
            if (!Context.IsWorldReady || __instance != Game1.player || !ModEntry.Config.ModEnabled)
                return;

            if (ModEntry.Config.MiningLevelOverride >= 0)
            {
                __result = ModEntry.Config.MiningLevelOverride;
            }
        }

        /// <summary>Patch to override foraging level.</summary>
        public static void Farmer_ForagingLevel_Postfix(Farmer __instance, ref int __result)
        {
            if (!Context.IsWorldReady || __instance != Game1.player || !ModEntry.Config.ModEnabled)
                return;

            if (ModEntry.Config.ForagingLevelOverride >= 0)
            {
                __result = ModEntry.Config.ForagingLevelOverride;
            }
        }

        /// <summary>Patch to override fishing level.</summary>
        public static void Farmer_FishingLevel_Postfix(Farmer __instance, ref int __result)
        {
            if (!Context.IsWorldReady || __instance != Game1.player || !ModEntry.Config.ModEnabled)
                return;

            if (ModEntry.Config.FishingLevelOverride >= 0)
            {
                __result = ModEntry.Config.FishingLevelOverride;
            }
        }

        /// <summary>Patch to override combat level.</summary>
        public static void Farmer_CombatLevel_Postfix(Farmer __instance, ref int __result)
        {
            if (!Context.IsWorldReady || __instance != Game1.player || !ModEntry.Config.ModEnabled)
                return;

            if (ModEntry.Config.CombatLevelOverride >= 0)
            {
                __result = ModEntry.Config.CombatLevelOverride;
            }
        }

        /// <summary>Patch to multiply XP gains.</summary>
        public static void Farmer_GainExperience_Prefix(Farmer __instance, ref int howMuch)
        {
            if (!Context.IsWorldReady || __instance != Game1.player || !ModEntry.Config.ModEnabled)
                return;

            if (ModEntry.Config.XPMultiplier != 1.0f)
            {
                howMuch = (int)(howMuch * ModEntry.Config.XPMultiplier);
            }
        }

        /// <summary>Patch to freeze time.</summary>
        public static bool Game1_PerformTenMinuteClockUpdate_Prefix()
        {
            if (!Context.IsWorldReady || !ModEntry.Config.ModEnabled)
                return true;

            if (ModEntry.Config.FreezeTime)
            {
                return false; // Skip time update
            }

            return true;
        }

        /// <summary>Patch to prevent passing out (static method).</summary>
        public static bool Farmer_PassOutFromTired_Prefix(Farmer who)
        {
            if (!Context.IsWorldReady || who != Game1.player || !ModEntry.Config.ModEnabled)
                return true;

            if (ModEntry.Config.NeverPassOut)
            {
                return false; // Don't pass out
            }

            return true;
        }

        /// <summary>Patch to multiply friendship gains.</summary>
        public static void Farmer_ChangeFriendship_Prefix(Farmer __instance, ref int amount, NPC n)
        {
            if (!Context.IsWorldReady || __instance != Game1.player || !ModEntry.Config.ModEnabled)
                return;

            // Prevent friendship decay
            if (ModEntry.Config.NoFriendshipDecay && amount < 0)
            {
                amount = 0;
                return;
            }

            // Multiply friendship gains
            if (amount > 0 && ModEntry.Config.FriendshipMultiplier != 1.0f)
            {
                amount = (int)(amount * ModEntry.Config.FriendshipMultiplier);
            }
        }

        /// <summary>Patch to force harvest quality - placeholder, actual implementation complex.</summary>
        public static void Crop_Harvest_Prefix(Crop __instance)
        {
            // Note: Crop quality is determined by complex factors including
            // fertilizer and farming level - this would require more extensive patching
        }

        /// <summary>Patch for max fish quality - override fishQuality parameter.</summary>
        public static void FishingRod_PullFishFromWater_Prefix(ref int fishQuality)
        {
            if (!Context.IsWorldReady || !ModEntry.Config.ModEnabled)
                return;

            if (ModEntry.Config.MaxFishQuality)
            {
                fishQuality = 4; // Iridium quality
            }
        }

        /// <summary>Patch to prevent item consumption (infinite items).</summary>
        public static bool Farmer_ReduceActiveItemByOne_Prefix(Farmer __instance)
        {
            if (!Context.IsWorldReady || __instance != Game1.player || !ModEntry.Config.ModEnabled)
                return true;

            if (ModEntry.Config.InfiniteItems)
            {
                return false; // Skip the original method - don't reduce item
            }

            return true;
        }

        /// <summary>Patch to prevent item stack consumption (infinite items).</summary>
        public static bool Item_ConsumeStack_Prefix(Item __instance, ref Item? __result, int amount)
        {
            if (!Context.IsWorldReady || !ModEntry.Config.ModEnabled)
                return true;

            if (ModEntry.Config.InfiniteItems)
            {
                // Return the item unchanged instead of consuming it
                __result = __instance;
                return false;
            }

            return true;
        }

        /// <summary>Patch for instant fish bite.</summary>
        public static void FishingRod_DoFunction_Postfix(FishingRod __instance, Farmer who)
        {
            if (!Context.IsWorldReady || who != Game1.player || !ModEntry.Config.ModEnabled)
                return;

            if (ModEntry.Config.InstantFishBite && __instance.isFishing)
            {
                // Force immediate bite
                var timeUntilFishingBite = AccessTools.Field(typeof(FishingRod), "timeUntilFishingBite");
                if (timeUntilFishingBite != null)
                {
                    timeUntilFishingBite.SetValue(__instance, 1f);
                }
            }
        }

        /// <summary>Patch to override max stamina.</summary>
        public static void Farmer_MaxStamina_Postfix(Farmer __instance, ref int __result)
        {
            if (!Context.IsWorldReady || __instance != Game1.player || !ModEntry.Config.ModEnabled)
                return;

            if (ModEntry.Config.MaxStaminaOverride > 0)
            {
                __result = ModEntry.Config.MaxStaminaOverride;
            }
        }

        /// <summary>Patch to add bonus attack.</summary>
        public static void Farmer_Attack_Postfix(Farmer __instance, ref int __result)
        {
            if (!Context.IsWorldReady || __instance != Game1.player || !ModEntry.Config.ModEnabled)
                return;

            if (ModEntry.Config.AddedAttack > 0)
            {
                __result += ModEntry.Config.AddedAttack;
            }
        }

        /// <summary>Patch to add bonus immunity.</summary>
        public static void Farmer_Immunity_Postfix(Farmer __instance, ref int __result)
        {
            if (!Context.IsWorldReady || __instance != Game1.player || !ModEntry.Config.ModEnabled)
                return;

            if (ModEntry.Config.AddedImmunity > 0)
            {
                __result += ModEntry.Config.AddedImmunity;
            }
        }

        /// <summary>Patch to multiply sell prices (Object.sellToStorePrice postfix).</summary>
        public static void Object_SellToStorePrice_Postfix(StardewValley.Object __instance, ref int __result)
        {
            if (!Context.IsWorldReady || !ModEntry.Config.ModEnabled)
                return;

            if (ModEntry.Config.SellPriceMultiplier != 1.0f)
            {
                __result = (int)(__result * ModEntry.Config.SellPriceMultiplier);
            }
        }

        /// <summary>Patch to multiply buy prices via salePrice (Object.salePrice postfix).</summary>
        public static void Object_SalePrice_Postfix(StardewValley.Object __instance, ref int __result)
        {
            if (!Context.IsWorldReady || !ModEntry.Config.ModEnabled)
                return;

            if (ModEntry.Config.BuyPriceMultiplier != 1.0f)
            {
                __result = (int)(__result * ModEntry.Config.BuyPriceMultiplier);
            }
        }
    }
}
