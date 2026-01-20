// Decompiled with JetBrains decompiler
// Type: StardewValley.Extensions.GameExtensions
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Netcode;
using StardewValley.Network;
using System.Collections.Generic;

#nullable disable
namespace StardewValley.Extensions;

/// <summary>Provides utility extension methods on specific game types.</summary>
public static class GameExtensions
{
  /// <summary>Add a light source to the dictionary.</summary>
  /// <param name="dictionary">The dictionary of light sources to update.</param>
  /// <param name="lightSource">The light source to add.</param>
  public static void Add(this IDictionary<string, LightSource> dictionary, LightSource lightSource)
  {
    if (lightSource == null)
      return;
    if (string.IsNullOrWhiteSpace(lightSource.Id))
    {
      lightSource.Id = $"LightSource_TempId_{Game1.random.Next()}";
      Game1.log.Warn($"Light source has no ID; assigning ID '{lightSource.Id}'.");
    }
    dictionary[lightSource.Id] = lightSource;
  }

  /// <summary>Add or overwrite a light source to the dictionary.</summary>
  /// <param name="dictionary">The dictionary of light sources to update.</param>
  /// <param name="lightSource">The light source to add.</param>
  public static void AddLight(
    this NetStringDictionary<LightSource, NetRef<LightSource>> dictionary,
    LightSource lightSource)
  {
    if (lightSource == null)
      return;
    if (string.IsNullOrWhiteSpace(lightSource.Id))
    {
      lightSource.Id = $"LightSource_TempId_{Game1.random.Next()}";
      Game1.log.Warn($"Light source has no ID; assigning ID '{lightSource.Id}'.");
    }
    dictionary[lightSource.Id] = lightSource;
  }
}
