// Decompiled with JetBrains decompiler
// Type: StardewValley.Extensions.ItemRegistryExtensions
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using StardewValley.ItemTypeDefinitions;
using System.Collections.Generic;

#nullable enable
namespace StardewValley.Extensions;

/// <summary>Provides utility extension methods on <see cref="T:StardewValley.ItemRegistry" /> and <see cref="T:StardewValley.ItemTypeDefinitions.IItemDataDefinition" /> types.</summary>
public static class ItemRegistryExtensions
{
  /// <summary>Get the parsed data for each item provided by this item data definition.</summary>
  /// <param name="definition">The item data definition to query.</param>
  public static 
  #nullable disable
  IEnumerable<ParsedItemData> GetAllData(this IItemDataDefinition definition)
  {
    foreach (string allId in definition.GetAllIds())
      yield return ItemRegistry.GetDataOrErrorItem(definition.Identifier + allId);
  }
}
