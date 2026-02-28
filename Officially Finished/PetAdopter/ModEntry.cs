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
    internal static IModHelper? ModHelper { get; private set; }

    private IViewEngine? viewEngine;

    public override void Entry(IModHelper helper)
    {
        ModMonitor = this.Monitor;
        ModHelper = helper;
        Config = helper.ReadConfig<ModConfig>();

        helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
        helper.Events.Input.ButtonPressed += this.OnButtonPressed;

        this.Monitor.Log(helper.Translation.Get("log.loaded"), LogLevel.Info);
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        // ── StardewUI ──
        viewEngine = this.Helper.ModRegistry.GetApi<IViewEngine>("focustense.StardewUI");
        if (viewEngine == null)
        {
            this.Monitor.Log(this.Helper.Translation.Get("log.stardewui-missing"), LogLevel.Error);
            return;
        }

        viewEngine.RegisterViews("Mods/CodeKokeshi.PetAdopter/Views", "assets/views");
        this.Monitor.Log(this.Helper.Translation.Get("log.stardewui-ok"), LogLevel.Debug);

        // ── GMCM ──
        var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
        if (configMenu is null)
            return;

        configMenu.Register(
            mod: this.ModManifest,
            reset: () => Config = new ModConfig(),
            save: () => this.Helper.WriteConfig(Config),
            titleScreenOnly: false
        );

        configMenu.AddSectionTitle(
            mod: this.ModManifest,
            text: () => this.Helper.Translation.Get("gmcm.section-title")
        );

        configMenu.AddParagraph(
            mod: this.ModManifest,
            text: () => this.Helper.Translation.Get("gmcm.description")
        );

        configMenu.AddKeybindList(
            mod: this.ModManifest,
            name: () => this.Helper.Translation.Get("gmcm.open-menu-key.name"),
            tooltip: () => this.Helper.Translation.Get("gmcm.open-menu-key.tooltip"),
            getValue: () => Config.OpenMenuKey,
            setValue: value => Config.OpenMenuKey = value
        );
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
            this.Monitor.Log(this.Helper.Translation.Get("log.menu-no-stardewui"), LogLevel.Warn);
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
            this.Monitor.Log(this.Helper.Translation.Get("log.menu-error", new { error = ex.Message }), LogLevel.Error);
        }
    }
}
