// Decompiled with JetBrains decompiler
// Type: StardewValley.Objects.Trinkets.RainbowHairTrinketEffect
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

#nullable disable
namespace StardewValley.Objects.Trinkets;

/// <summary>Implements the special behavior for a <see cref="T:StardewValley.Objects.Trinkets.Trinket" /> which makes the player's hair prismatic.</summary>
/// <inheritdoc />
public class RainbowHairTrinketEffect(Trinket trinket) : TrinketEffect(trinket)
{
  /// <inheritdoc />
  public override void Apply(Farmer farmer) => farmer.prismaticHair.Value = true;

  /// <inheritdoc />
  public override void Unapply(Farmer farmer) => farmer.prismaticHair.Value = false;
}
