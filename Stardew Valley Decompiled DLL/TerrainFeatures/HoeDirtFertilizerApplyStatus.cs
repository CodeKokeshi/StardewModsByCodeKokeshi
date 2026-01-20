// Decompiled with JetBrains decompiler
// Type: StardewValley.TerrainFeatures.HoeDirtFertilizerApplyStatus
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

#nullable disable
namespace StardewValley.TerrainFeatures;

/// <summary>Indicates whether fertilizer can be applied to a given <see cref="T:StardewValley.TerrainFeatures.HoeDirt" /> instance.</summary>
public enum HoeDirtFertilizerApplyStatus
{
  /// <summary>The fertilizer can be applied.</summary>
  Okay,
  /// <summary>The fertilizer can't be applied because the dirt already has the same fertilizer.</summary>
  HasThisFertilizer,
  /// <summary>The fertilizer can't be applied because the dirt already has a different fertilizer.</summary>
  HasAnotherFertilizer,
  /// <summary>The fertilizer can't be applied because the crop has already sprouted, and this fertilizer must be placed before that point.</summary>
  CropAlreadySprouted,
}
