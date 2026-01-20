// Decompiled with JetBrains decompiler
// Type: StardewValley.Inventories.OnSlotChangedDelegate
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

#nullable disable
namespace StardewValley.Inventories;

/// <summary>The delegate for <see cref="E:StardewValley.Inventories.Inventory.OnSlotChanged" />.</summary>
/// <param name="inventory">The inventory instance.</param>
/// <param name="index">The item slot's index within the inventory.</param>
/// <param name="before">The previous item value (which may be <c>null</c> when adding a stack).</param>
/// <param name="after">The new item value (which may be <c>null</c> when removing a stack).</param>
public delegate void OnSlotChangedDelegate(
  Inventory inventory,
  int index,
  Item before,
  Item after);
