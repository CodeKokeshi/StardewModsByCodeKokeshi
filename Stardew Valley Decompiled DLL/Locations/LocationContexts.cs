// Decompiled with JetBrains decompiler
// Type: StardewValley.LocationContexts
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using StardewValley.GameData.LocationContexts;
using System.Collections.Generic;

#nullable disable
namespace StardewValley;

/// <summary>Manages data about the game's location contexts.</summary>
public static class LocationContexts
{
  /// <summary>The location context ID for the valley in <c>Data/LocationContexts</c>.</summary>
  public const string DefaultId = "Default";
  /// <summary>The location context ID for the desert in <c>Data/LocationContexts</c>.</summary>
  public const string DesertId = "Desert";
  /// <summary>The location context ID for Ginger Island in <c>Data/LocationContexts</c>.</summary>
  public const string IslandId = "Island";

  /// <summary>The location context data for Ginger Island.</summary>
  public static LocationContextData Island => StardewValley.LocationContexts.Require(nameof (Island));

  /// <summary>The location context data for the valley.</summary>
  public static LocationContextData Default => StardewValley.LocationContexts.Require(nameof (Default));

  /// <summary>Get a location context by ID.</summary>
  /// <param name="id">The location context's ID in <c>Data/LocationContext</c>.</param>
  /// <exception cref="T:System.Collections.Generic.KeyNotFoundException">There's no location context with the given <paramref name="id" />.</exception>
  public static LocationContextData Require(string id)
  {
    LocationContextData locationContextData;
    if (id == null || !Game1.locationContextData.TryGetValue(id, out locationContextData))
      throw new KeyNotFoundException($"There's no entry in Data/LocationContexts with the required ID '{id}'.");
    return locationContextData;
  }
}
