using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace MaxQuality;

internal sealed class ModConfig
{
    /// <summary>Apply max quality to player inventory and the currently-open chest menu (if any).</summary>
    public KeybindList ApplyKey { get; set; } = new(SButton.F9);
}
