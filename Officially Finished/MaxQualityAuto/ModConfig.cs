namespace MaxQualityAuto;

internal sealed class ModConfig
{
    /// <summary>Master toggle — when false, the mod does nothing.</summary>
    public bool Enabled { get; set; } = true;

    /// <summary>Automatically upgrade items whenever your inventory changes (pick-up, craft, buy, etc.).</summary>
    public bool AutoUpgradeInventory { get; set; } = true;

    /// <summary>Automatically upgrade chest contents when you open a chest (and while moving items in/out).</summary>
    public bool AutoUpgradeChests { get; set; } = true;
}
