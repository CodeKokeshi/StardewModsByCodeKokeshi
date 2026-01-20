// Decompiled with JetBrains decompiler
// Type: StardewValley.Objects.Trinkets.CompanionTrinketEffect
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using StardewValley.Companions;
using StardewValley.Extensions;
using StardewValley.TokenizableStrings;
using System;

#nullable disable
namespace StardewValley.Objects.Trinkets;

/// <summary>Implements the special behavior for a <see cref="T:StardewValley.Objects.Trinkets.Trinket" /> which summons a hungry frog companion.</summary>
/// <inheritdoc />
public class CompanionTrinketEffect(Trinket trinket) : TrinketEffect(trinket)
{
  /// <summary>The frog variant to spawn.</summary>
  public int Variant;

  /// <inheritdoc />
  public override bool GenerateRandomStats(Trinket trinket)
  {
    Random random = Utility.CreateRandom((double) trinket.generationSeed.Value);
    this.Variant = !random.NextBool(0.2) ? (!random.NextBool(0.8) ? (!random.NextBool(0.8) ? random.Next(2) + 6 : random.Next(3) + 3) : random.Next(3)) : 0;
    trinket.displayNameOverrideTemplate.Value = TokenStringBuilder.LocalizedText("Strings\\1_6_Strings:frog_variant_" + this.Variant.ToString());
    return true;
  }

  /// <inheritdoc />
  public override void Apply(Farmer farmer)
  {
    this.Companion = (Companion) new HungryFrogCompanion(this.Variant);
    if (Game1.gameMode != (byte) 3)
      return;
    farmer.AddCompanion(this.Companion);
  }

  /// <inheritdoc />
  public override void Unapply(Farmer farmer) => farmer.RemoveCompanion(this.Companion);
}
