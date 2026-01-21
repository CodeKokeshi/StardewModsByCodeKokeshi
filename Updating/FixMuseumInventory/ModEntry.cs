using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using HarmonyLib;

namespace FixMuseumInventory;

/// <summary>
/// Replaces the vanilla museum menu with a custom implementation that:
/// - In donation mode: Shows ONLY donatable items in a compact, draggable inventory
/// - In arrange mode: NO inventory at all, just move pieces around
/// </summary>
public class ModEntry : Mod
{
    public override void Entry(IModHelper helper)
    {
        var harmony = new Harmony(ModManifest.UniqueID);
        
        // Intercept when vanilla tries to open MuseumMenu - replace it with ours
        harmony.Patch(
            original: AccessTools.Constructor(typeof(MuseumMenu), new[] { typeof(InventoryMenu.highlightThisItem) }),
            prefix: new HarmonyMethod(typeof(ModEntry), nameof(MuseumMenu_Constructor_Prefix))
        );
        
        // Hook input events to detect arrange mode trigger
        helper.Events.Input.ButtonPressed += OnButtonPressed;
    }
    
    /// <summary>
    /// Intercepts vanilla MuseumMenu creation and replaces it with our custom menu
    /// </summary>
    private static bool MuseumMenu_Constructor_Prefix(InventoryMenu.highlightThisItem highlighterMethod)
    {
        // The vanilla menu is being opened - stop it and open ours instead
        if (Game1.currentLocation is LibraryMuseum museum)
        {
            // Open in donation mode (default)
            Game1.activeClickableMenu = new CustomMuseumMenu(museum, arrangeMode: false);
            return false; // Prevent vanilla constructor
        }
        
        return true; // Let vanilla handle non-museum cases
    }
    
    /// <summary>
    /// Detects when player wants to enter arrange mode
    /// </summary>
    private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
    {
        // Check if we're in the museum with our custom menu open
        if (Game1.currentLocation is not LibraryMuseum museum)
            return;
        
        if (Game1.activeClickableMenu is not CustomMuseumMenu currentMenu)
            return;
        
        // Toggle between donation and arrange mode with 'R' key (Rearrange)
        if (e.Button == SButton.R)
        {
            // Close current menu and open in opposite mode
            bool newMode = !currentMenu.IsArrangeMode;
            currentMenu.exitThisMenu(false);
            
            Game1.activeClickableMenu = new CustomMuseumMenu(museum, arrangeMode: newMode);
            Helper.Input.Suppress(e.Button);
            
            string modeText = newMode ? "Arrange Mode" : "Donation Mode";
            Game1.addHUDMessage(new HUDMessage(modeText, HUDMessage.newQuest_type));
        }
    }
}
