using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Network;
using HarmonyLib;
using System;

namespace FixMuseumInventory;

/// <summary>
/// Replaces the vanilla museum menu with a custom implementation that:
/// - Donation mode (talking to Gunther): Shows ONLY donatable items in a compact, draggable inventory
/// - Arrange mode (inspecting book): NO inventory at all, just move pieces around
/// </summary>
public class ModEntry : Mod
{
    public override void Entry(IModHelper helper)
    {
        var harmony = new Harmony(ModManifest.UniqueID);
        
        // Intercept when game tries to open donation menu (talking to Gunther)
        harmony.Patch(
            original: AccessTools.Method(typeof(LibraryMuseum), nameof(LibraryMuseum.OpenDonationMenu)),
            prefix: new HarmonyMethod(typeof(ModEntry), nameof(OpenDonationMenu_Prefix))
        );
        
        // Intercept when game tries to open rearrange menu (inspecting book)
        harmony.Patch(
            original: AccessTools.Method(typeof(LibraryMuseum), nameof(LibraryMuseum.OpenRearrangeMenu)),
            prefix: new HarmonyMethod(typeof(ModEntry), nameof(OpenRearrangeMenu_Prefix))
        );
    }
    
    /// <summary>
    /// Intercepts donation menu (Gunther) and replaces with our custom menu in donation mode
    /// </summary>
    private static bool OpenDonationMenu_Prefix(LibraryMuseum __instance)
    {
        // Get the mutex and lock it (same as vanilla)
        var mutexField = AccessTools.Field(typeof(LibraryMuseum), "mutex");
        var mutex = (NetMutex?)mutexField?.GetValue(__instance);
        
        mutex?.RequestLock(() =>
        {
            // Open our custom menu in DONATION mode
            Game1.activeClickableMenu = new CustomMuseumMenu(__instance, arrangeMode: false)
            {
                exitFunction = new IClickableMenu.onExit(__instance.OnDonationMenuClosed)
            };
        });
        
        return false; // Prevent vanilla method
    }
    
    /// <summary>
    /// Intercepts rearrange menu (book) and replaces with our custom menu in arrange mode
    /// </summary>
    private static bool OpenRearrangeMenu_Prefix(LibraryMuseum __instance)
    {
        // Get the mutex and check if locked (same as vanilla)
        var mutexField = AccessTools.Field(typeof(LibraryMuseum), "mutex");
        var mutex = (NetMutex?)mutexField?.GetValue(__instance);
        
        if (mutex?.IsLocked() ?? false)
            return false;
        
        mutex?.RequestLock(() =>
        {
            // Open our custom menu in ARRANGE mode
            Game1.activeClickableMenu = new CustomMuseumMenu(__instance, arrangeMode: true)
            {
                exitFunction = new IClickableMenu.onExit(mutex.ReleaseLock)
            };
        });
        
        return false; // Prevent vanilla method
    }
}
