using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace PetAdopter;

internal sealed class ModConfig
{
    /// <summary>Hotkey to open the pet adoption menu. Default: LeftShift + V.</summary>
    public KeybindList OpenMenuKey { get; set; } = new(new Keybind(SButton.LeftShift, SButton.V));

    /// <summary>If true, adoption is blocked when there are no empty pet bowls. Default: false (vanilla behavior).</summary>
    public bool RequireFreeBowl { get; set; } = false;
}
