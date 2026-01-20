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
    public static void Prefix(ref int value)
    {
        // Always set quality to 4 (iridium)
        value = 4;
    }
}
