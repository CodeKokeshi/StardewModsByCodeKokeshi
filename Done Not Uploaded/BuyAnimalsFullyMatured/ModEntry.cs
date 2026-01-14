using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace BuyAnimalsFullyMatured;

public sealed class ModEntry : Mod
{
    public override void Entry(IModHelper helper)
    {
        var harmony = new Harmony(ModManifest.UniqueID);
        harmony.PatchAll();
    }
}

[HarmonyPatch(typeof(PurchaseAnimalsMenu), nameof(PurchaseAnimalsMenu.receiveLeftClick))]
internal static class PurchaseAnimalsMenu_ReceiveLeftClick_Patch
{
    public static void Prefix(PurchaseAnimalsMenu __instance, ref FarmAnimal? __state)
    {
        __state = __instance.animalBeingPurchased;
    }

    public static void Postfix(PurchaseAnimalsMenu __instance, FarmAnimal? __state)
    {
        FarmAnimal? purchased = __instance.animalBeingPurchased;
        if (purchased == null)
            return;

        // Only apply when a new animal was just selected for purchase.
        if (ReferenceEquals(purchased, __state))
            return;

        purchased.growFully();
    }
}
