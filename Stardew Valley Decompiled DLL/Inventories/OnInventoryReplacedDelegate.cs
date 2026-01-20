// Decompiled with JetBrains decompiler
// Type: StardewValley.Inventories.OnInventoryReplacedDelegate
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using System.Collections.Generic;

#nullable disable
namespace StardewValley.Inventories;

/// <summary>The delegate for <see cref="E:StardewValley.Inventories.Inventory.OnInventoryReplaced" />.</summary>
/// <param name="inventory">The inventory instance.</param>
/// <param name="before">The previous item list.</param>
/// <param name="after">The new item list.</param>
public delegate void OnInventoryReplacedDelegate(
  Inventory inventory,
  IList<Item> before,
  IList<Item> after);
