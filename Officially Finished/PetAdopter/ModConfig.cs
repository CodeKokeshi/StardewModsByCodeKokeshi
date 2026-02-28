using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace PetAdopter;

internal sealed class ModConfig
{
    /// <summary>Hotkey to open the pet adoption menu. Default: LeftShift + V.</summary>
    public KeybindList OpenMenuKey { get; set; } = new(new Keybind(SButton.LeftShift, SButton.V));
}
