using System;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley;

namespace MaxQuality;

public class ModEntry : Mod
{
    public override void Entry(IModHelper helper)
    {
        var harmony = new Harmony(this.ModManifest.UniqueID);
        harmony.PatchAll();
    }
}

/// <summary>
/// Patch to force all items to have maximum (iridium) quality.
/// Quality values: 0 = normal, 1 = silver, 2 = gold, 4 = iridium
/// </summary>
[HarmonyPatch(typeof(Item), nameof(Item.Quality), MethodType.Setter)]
public static class ItemQualityPatch
{
    public static bool Prefix(Item __instance, ref int value)
    {
        // CRITICAL EDGE CASE: Hat Stand (BC)126 uses quality field to store hat ID, not actual quality
        // When placing a hat on mannequin: quality = hatItemId + 1
        // When removing hat: hatItemId = quality - 1
        // Must NOT patch quality for hat stands to avoid corrupting hat storage
        if (__instance is StardewValley.Object obj && obj.QualifiedItemId == "(BC)126")
        {
            return true; // Use original setter (don't patch hat stand)
        }

        // All other items: force to iridium quality
        value = 4;
        return true; // Continue with modified value
    }
}
