// Decompiled with JetBrains decompiler
// Type: StardewValley.Internal.ShopBuilder
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using StardewValley.Extensions;
using StardewValley.GameData;
using StardewValley.GameData.Shops;
using StardewValley.GameData.Tools;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace StardewValley.Internal;

/// <summary>Handles building a shop menu from data in <c>Data/Shops</c>.</summary>
/// <remarks>This is an internal implementation class. Most code should use <see cref="M:StardewValley.Utility.TryOpenShopMenu(System.String,System.String,System.Boolean)" /> instead.</remarks>
public static class ShopBuilder
{
  /// <summary>Get the inventory to sell for a shop menu.</summary>
  /// <param name="shopId">The shop ID matching the entry in <c>Data/Shops</c>.</param>
  public static Dictionary<ISalable, ItemStockInformation> GetShopStock(string shopId)
  {
    ShopData shop;
    return DataLoader.Shops(Game1.content).TryGetValue(shopId, out shop) ? ShopBuilder.GetShopStock(shopId, shop) : new Dictionary<ISalable, ItemStockInformation>();
  }

  /// <summary>Get the inventory to sell for a shop menu.</summary>
  /// <param name="shopId">The shop ID in <c>Data\Shops</c>.</param>
  /// <param name="shop">The shop data from <c>Data\Shops</c>.</param>
  public static Dictionary<ISalable, ItemStockInformation> GetShopStock(
    string shopId,
    ShopData shop)
  {
    Dictionary<ISalable, ItemStockInformation> local_stock = new Dictionary<ISalable, ItemStockInformation>();
    List<ShopItemData> items = shop.Items;
    // ISSUE: explicit non-virtual call
    if ((items != null ? (__nonvirtual (items.Count) > 0 ? 1 : 0) : 0) != 0)
    {
      Random daySaveRandom = Utility.CreateDaySaveRandom();
      HashSet<string> stockedItems = new HashSet<string>();
      ItemQueryContext context = new ItemQueryContext(Game1.currentLocation, Game1.player, daySaveRandom, $"shop '{shopId}'");
      bool applyPierreMissingStockList = shopId == "SeedShop" && Game1.MasterPlayer.hasOrWillReceiveMail("PierreStocklist");
      HashSet<string> stringSet = new HashSet<string>();
      foreach (ShopItemData shopItemData in shop.Items)
      {
        if (!stringSet.Add(shopItemData.Id))
          Game1.log.Warn($"Shop {shopId} has multiple items with entry ID '{shopItemData.Id}'. This may cause unintended behavior.");
        bool isOutOfSeason;
        if (ShopBuilder.CheckItemCondition(shopItemData.Condition, applyPierreMissingStockList, out isOutOfSeason))
        {
          IList<ItemQueryResult> itemQueryResultList = ItemQueryResolver.TryResolve((ISpawnItemData) shopItemData, context, avoidRepeat: shopItemData.AvoidRepeat, avoidItemIds: shopItemData.AvoidRepeat ? stockedItems : (HashSet<string>) null, logError: (Action<string, string>) ((query, message) => Game1.log.Error($"Failed parsing shop item query '{query}' for the '{shopId}' shop: {message}.")));
          int num1 = 0;
          foreach (ItemQueryResult output in (IEnumerable<ItemQueryResult>) itemQueryResultList)
          {
            ISalable targetItem = output.Item;
            targetItem.Stack = output.OverrideStackSize ?? targetItem.Stack;
            float num2 = (float) ShopBuilder.GetBasePrice(output, shop, shopItemData, targetItem, isOutOfSeason, shopItemData.UseObjectDataPrice);
            int? nullable1 = output.OverrideShopAvailableStock;
            int num3 = nullable1 ?? shopItemData.AvailableStock;
            LimitedStockMode limitedStockMode = shopItemData.AvailableStockLimit;
            string str = output.OverrideTradeItemId ?? shopItemData.TradeItemId;
            nullable1 = output.OverrideTradeItemAmount;
            int num4 = 0;
            int? nullable2 = nullable1.GetValueOrDefault() > num4 & nullable1.HasValue ? output.OverrideTradeItemAmount : new int?(shopItemData.TradeItemAmount);
            if (str != null)
            {
              nullable1 = nullable2;
              int num5 = 0;
              if (!(nullable1.GetValueOrDefault() < num5 & nullable1.HasValue))
                goto label_11;
            }
            str = (string) null;
            nullable2 = new int?();
label_11:
            if (shopItemData.IsRecipe)
            {
              targetItem.Stack = 1;
              limitedStockMode = LimitedStockMode.None;
              num3 = 1;
            }
            if (!shopItemData.IgnoreShopPriceModifiers)
              num2 = Utility.ApplyQuantityModifiers(num2, (IList<QuantityModifier>) shop.PriceModifiers, shop.PriceModifierMode, targetItem: targetItem as Item, random: daySaveRandom);
            float num6 = Utility.ApplyQuantityModifiers(num2, (IList<QuantityModifier>) shopItemData.PriceModifiers, shopItemData.PriceModifierMode, targetItem: targetItem as Item, random: daySaveRandom);
            if (!shopItemData.IsRecipe)
              num3 = (int) Utility.ApplyQuantityModifiers((float) num3, (IList<QuantityModifier>) shopItemData.AvailableStockModifiers, shopItemData.AvailableStockModifierMode, targetItem: targetItem as Item, random: daySaveRandom);
            if (!ShopBuilder.TrackSeenItems(stockedItems, targetItem) || !shopItemData.AvoidRepeat)
            {
              if (num3 < 0)
                num3 = int.MaxValue;
              string id = shopItemData.Id;
              if (++num1 > 1)
                id += num1.ToString();
              Dictionary<ISalable, ItemStockInformation> dictionary = local_stock;
              ISalable key = targetItem;
              int price = (int) num6;
              int stock = num3;
              string tradeItem = str;
              int? tradeItemCount = nullable2;
              int stockMode = (int) limitedStockMode;
              string syncedKey = id;
              Item syncStacksWith = output.SyncStacksWith;
              List<string> actionsOnPurchase1 = shopItemData.ActionsOnPurchase;
              StackDrawType? stackDrawType = new StackDrawType?();
              List<string> actionsOnPurchase2 = actionsOnPurchase1;
              ItemStockInformation stockInformation = new ItemStockInformation(price, stock, tradeItem, tradeItemCount, (LimitedStockMode) stockMode, syncedKey, (ISalable) syncStacksWith, stackDrawType, actionsOnPurchase2);
              dictionary.Add(key, stockInformation);
            }
          }
        }
      }
    }
    Game1.player.team.synchronizedShopStock.UpdateLocalStockWithSyncedQuanitities(shopId, local_stock);
    return local_stock;
  }

  /// <summary>Check a game state query which determines whether an item should be added to a shop menu.</summary>
  /// <param name="conditions">The conditions to check.</param>
  /// <param name="applyPierreMissingStockList">Whether to apply Pierre's Missing Stock List, which allows buying out-of-season crops.</param>
  /// <param name="isOutOfSeason">Whether this is an out-of-season item which is allowed (for a price) because the player found Pierre's Stock List.</param>
  public static bool CheckItemCondition(
    string conditions,
    bool applyPierreMissingStockList,
    out bool isOutOfSeason)
  {
    if (conditions == null || GameStateQuery.CheckConditions(conditions))
    {
      isOutOfSeason = false;
      return true;
    }
    if (applyPierreMissingStockList && GameStateQuery.CheckConditions(conditions, ignoreQueryKeys: GameStateQuery.SeasonQueryKeys))
    {
      isOutOfSeason = true;
      return true;
    }
    isOutOfSeason = false;
    return false;
  }

  /// <summary>Get the tool upgrade data to show in the blacksmith shop for a given tool, if any.</summary>
  /// <param name="tool">The tool data to show as an upgrade, if possible.</param>
  /// <param name="player">The player viewing the shop.</param>
  public static ToolUpgradeData GetToolUpgradeData(ToolData tool, Farmer player)
  {
    if (tool == null)
      return (ToolUpgradeData) null;
    IList<ToolUpgradeData> second = (IList<ToolUpgradeData>) tool.UpgradeFrom;
    if (tool.ConventionalUpgradeFrom != null)
    {
      IList<ToolUpgradeData> first = (IList<ToolUpgradeData>) new ToolUpgradeData[1]
      {
        new ToolUpgradeData()
        {
          RequireToolId = tool.ConventionalUpgradeFrom,
          Price = ShopBuilder.GetToolUpgradeConventionalPrice(tool.UpgradeLevel),
          TradeItemId = ShopBuilder.GetToolUpgradeConventionalTradeItem(tool.UpgradeLevel),
          TradeItemAmount = 5
        }
      };
      second = second == null || second.Count <= 0 ? first : (IList<ToolUpgradeData>) first.Concat<ToolUpgradeData>((IEnumerable<ToolUpgradeData>) second).ToList<ToolUpgradeData>();
    }
    if (second == null)
      return (ToolUpgradeData) null;
    foreach (ToolUpgradeData toolUpgradeData in (IEnumerable<ToolUpgradeData>) second)
    {
      if ((toolUpgradeData.Condition == null || GameStateQuery.CheckConditions(toolUpgradeData.Condition, player.currentLocation, player)) && (toolUpgradeData.RequireToolId == null || player.Items.ContainsId(toolUpgradeData.RequireToolId)))
        return toolUpgradeData;
    }
    return (ToolUpgradeData) null;
  }

  /// <summary>Get the conventional price for a tool upgrade.</summary>
  /// <param name="level">The level to which the tool is being upgraded.</param>
  public static int GetToolUpgradeConventionalPrice(int level)
  {
    switch (level)
    {
      case 1:
        return 2000;
      case 2:
        return 5000;
      case 3:
        return 10000;
      case 4:
        return 25000;
      default:
        return 2000;
    }
  }

  /// <summary>Get the unqualified item ID for the conventional material that must be provided for a tool upgrade.</summary>
  /// <param name="level">The level to which the tool is being upgraded.</param>
  private static string GetToolUpgradeConventionalTradeItem(int level)
  {
    switch (level)
    {
      case 1:
        return "334";
      case 2:
        return "335";
      case 3:
        return "336";
      case 4:
        return "337";
      default:
        return "334";
    }
  }

  /// <summary>Get the owner entries for a shop whose conditions currently match.</summary>
  /// <param name="shop">The shop data to check.</param>
  public static IEnumerable<ShopOwnerData> GetCurrentOwners(ShopData shop)
  {
    IEnumerable<ShopOwnerData> shopOwnerDatas;
    if (shop == null)
    {
      shopOwnerDatas = (IEnumerable<ShopOwnerData>) null;
    }
    else
    {
      List<ShopOwnerData> owners = shop.Owners;
      shopOwnerDatas = owners != null ? owners.Where<ShopOwnerData>((Func<ShopOwnerData, bool>) (owner => GameStateQuery.CheckConditions(owner.Condition))) : (IEnumerable<ShopOwnerData>) null;
    }
    return shopOwnerDatas ?? (IEnumerable<ShopOwnerData>) LegacyShims.EmptyArray<ShopOwnerData>();
  }

  /// <summary>Get the sell price for a shop item, excluding quantity modifiers.</summary>
  /// <param name="output">The shop item for which to get the base price.</param>
  /// <param name="shopData">The shop data.</param>
  /// <param name="itemData">The shop item's data.</param>
  /// <param name="item">The item instance.</param>
  /// <param name="outOfSeasonPrice">Whether to apply the out-of-season pricing for Pierre's Missing Stock List.</param>
  /// <param name="useObjectDataPrice">If <paramref name="item" /> has type <see cref="F:StardewValley.ItemRegistry.type_object" />, whether to use the raw price in <c>Data/Objects</c> instead of the calculated sell-to-player price.</param>
  public static int GetBasePrice(
    ItemQueryResult output,
    ShopData shopData,
    ShopItemData itemData,
    ISalable item,
    bool outOfSeasonPrice,
    bool useObjectDataPrice = false)
  {
    float basePrice = (float) (output.OverrideBasePrice ?? itemData.Price);
    if ((double) basePrice < 0.0)
      basePrice = itemData.TradeItemId == null ? (!useObjectDataPrice || !item.HasTypeObject() || !(item is StardewValley.Object @object) ? (float) item.salePrice(true) : (float) @object.Price) : 0.0f;
    if (((int) itemData.ApplyProfitMargins ?? (int) shopData.ApplyProfitMargins ?? (item.appliesProfitMargins() ? 1 : 0)) != 0)
      basePrice *= Game1.MasterPlayer.difficultyModifier;
    if (outOfSeasonPrice)
      basePrice *= 1.5f;
    return (int) basePrice;
  }

  /// <summary>Add an item to the list of items already in the shop.</summary>
  /// <param name="stockedItems">The item IDs in the shop.</param>
  /// <param name="item">The item to track.</param>
  /// <returns>Returns whether the item was already in the shop.</returns>
  public static bool TrackSeenItems(HashSet<string> stockedItems, ISalable item)
  {
    string str = item.QualifiedItemId;
    if (item is Tool tool && tool.UpgradeLevel > 0)
      str = $"{str}#{tool.UpgradeLevel.ToString()}";
    if (item.IsRecipe)
      str += "#Recipe";
    return !stockedItems.Add(str);
  }
}
