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
        // Don't affect non-object items (e.g. tools, rings, hats, weapons, etc).
        if (__instance is not StardewValley.Object obj)
            return true;

        // Never affect big craftables.
        if (obj.bigCraftable.Value)
            return true;

        // CRITICAL EDGE CASE: Hat Stand (BC)126 uses the quality field to store hat ID, not actual quality.
        if (obj.QualifiedItemId == "(BC)126")
            return true;

        // Only force quality for items which can reasonably have quality in vanilla.
        // This avoids affecting junk/resources and other non-quality objects.
        bool shouldForceQuality =
            obj.isForage()
            || obj.Category == StardewValley.Object.FishCategory
            || obj.Category == StardewValley.Object.EggCategory
            || obj.Category == StardewValley.Object.MilkCategory
            || obj.Category == StardewValley.Object.artisanGoodsCategory
            || obj.Category == StardewValley.Object.meatCategory;

        if (!shouldForceQuality)
            return true;

        value = StardewValley.Object.bestQuality;
        return true;
    }
}
