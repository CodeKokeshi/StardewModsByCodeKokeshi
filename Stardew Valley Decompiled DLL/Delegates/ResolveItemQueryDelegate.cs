// Decompiled with JetBrains decompiler
// Type: StardewValley.Delegates.ResolveItemQueryDelegate
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using StardewValley.Internal;
using System;
using System.Collections.Generic;

#nullable disable
namespace StardewValley.Delegates;

/// <summary>Creates the items for an item query key like <c>RANDOM_ITEMS</c>.</summary>
/// <param name="key">The query key like <c>RANDOM_ITEMS</c> specified in the item ID.</param>
/// <param name="arguments">Any text specified in the item ID after the <paramref name="key" />.</param>
/// <param name="avoidRepeat">Whether duplicate items will be stripped from the list. This is only a hint for cases where the resolver may want to return unique items (e.g. if a specific count is expected); any duplicates will be removed automatically at a higher level.</param>
/// <param name="avoidItemIds">The qualified item IDs which shouldn't be returned, or <c>null</c> for none.</param>
/// <param name="context">The contextual info for item queries.</param>
/// <param name="logError">Log an error message to the console, given the item query and error message.</param>
/// <returns>Returns the resolved items, if any.</returns>
public delegate IEnumerable<ItemQueryResult> ResolveItemQueryDelegate(
  string key,
  string arguments,
  ItemQueryContext context,
  bool avoidRepeat,
  HashSet<string> avoidItemIds,
  Action<string, string> logError);
