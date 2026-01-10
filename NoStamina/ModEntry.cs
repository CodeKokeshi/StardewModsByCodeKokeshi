using System;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley;

namespace NoStamina;

public class ModEntry : Mod
{
    public override void Entry(IModHelper helper)
    {
        var harmony = new Harmony(this.ModManifest.UniqueID);
        harmony.PatchAll();
    }
}

[HarmonyPatch(typeof(Farmer), nameof(Farmer.Stamina), MethodType.Setter)]
public static class FarmerStaminaPatch
{
    public static void Prefix(Farmer __instance, ref float value)
    {
        // Only affect the local player
        if (__instance == Game1.player)
        {
            // Instead of allowing stamina to drop, force it to MaxStamina
            value = __instance.MaxStamina; 
        }
    }
}
