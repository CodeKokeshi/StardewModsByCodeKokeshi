// Decompiled with JetBrains decompiler
// Type: StardewValley.Internal.ItemQueryResult
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

#nullable disable
namespace StardewValley.Internal;

/// <summary>An item resolved by the <see cref="T:StardewValley.Internal.ItemQueryResolver" />.</summary>
public class ItemQueryResult
{
  /// <summary>The resolved item instance.</summary>
  public ISalable Item;
  /// <summary>The base gold price for the resolved item (i.e. <c>Item.Price</c>), or <c>null</c> to get it from the item data.</summary>
  public int? OverrideBasePrice;
  /// <summary>If set, the number of the item purchased on each click.</summary>
  public int? OverrideStackSize;
  /// <summary>The maximum number of sets of this item which can be purchased in one day, or <see cref="F:StardewValley.Menus.ShopMenu.infiniteStock" />, or <c>null</c> to get it from the shop data. This has no effect when used outside shops.</summary>
  public int? OverrideShopAvailableStock;
  /// <summary>If set, overrides the qualified or unqualified item ID which must be traded to purchase this item.</summary>
  public string OverrideTradeItemId;
  /// <summary>If set, overrides the number of <see cref="F:StardewValley.Internal.ItemQueryResult.OverrideTradeItemId" /> needed to purchase this item.</summary>
  public int? OverrideTradeItemAmount;
  /// <summary>If set, the stack count will be synchronized with the given item's. This is very specialized and only used for objects whose available stock are tracked separately from the normal shop stock tracking.</summary>
  public StardewValley.Item SyncStacksWith;

  /// <summary>Construct an instance.</summary>
  /// <param name="item">The resolved item instance.</param>
  public ItemQueryResult(ISalable item) => this.Item = item;
}
