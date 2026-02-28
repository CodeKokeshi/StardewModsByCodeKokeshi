#nullable enable
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewUI.Framework;
using StardewValley;

namespace PetAdopter;

public class ModEntry : Mod
{
    internal static ModConfig Config { get; private set; } = new();
    internal static IMonitor? ModMonitor { get; private set; }

    private IViewEngine? viewEngine;

    public override void Entry(IModHelper helper)
    {
        ModMonitor = this.Monitor;
        Config = helper.ReadConfig<ModConfig>();

        helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
        helper.Events.Input.ButtonPressed += this.OnButtonPressed;

        this.Monitor.Log("[Pet Adopter] Loaded! Press LShift + V to open the adoption menu.", LogLevel.Info);
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        // Initialize StardewUI
        viewEngine = this.Helper.ModRegistry.GetApi<IViewEngine>("focustense.StardewUI");
        if (viewEngine == null)
        {
            this.Monitor.Log("StardewUI Framework not found! Cannot create adoption menu.", LogLevel.Error);
            return;
        }

        // Register our view assets
        viewEngine.RegisterViews("Mods/CodeKokeshi.PetAdopter/Views", "assets/views");

        this.Monitor.Log("StardewUI integration complete!", LogLevel.Debug);
    }

    private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
    {
        if (!Config.OpenMenuKey.JustPressed())
            return;
        if (!Context.IsWorldReady)
            return;

        OpenAdoptionMenu();
    }

    private void OpenAdoptionMenu()
    {
        if (viewEngine == null)
        {
            this.Monitor.Log("Cannot open adoption menu — StardewUI not loaded.", LogLevel.Warn);
            return;
        }

        try
        {
            var viewModel = new PetAdopterViewModel();
            var controller = viewEngine.CreateMenuControllerFromAsset(
                "Mods/CodeKokeshi.PetAdopter/Views/PetAdopterMenu",
                viewModel
            );

            controller.EnableCloseButton();
            controller.DimmingAmount = 0.75f;

            Game1.activeClickableMenu = controller.Menu;
        }
        catch (Exception ex)
        {
            this.Monitor.Log($"Error opening adoption menu: {ex.Message}\n{ex.StackTrace}", LogLevel.Error);
        }
    }
}
