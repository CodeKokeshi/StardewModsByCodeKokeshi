// Decompiled with JetBrains decompiler
// Type: StardewValley.Internal.ItemQueryResolver
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Netcode;
using StardewValley.Delegates;
using StardewValley.Enchantments;
using StardewValley.Extensions;
using StardewValley.GameData;
using StardewValley.GameData.Objects;
using StardewValley.GameData.Pets;
using StardewValley.GameData.Tools;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

#nullable enable
namespace StardewValley.Internal;

/// <summary>Resolves item IDs like <samp>(O)128</samp> and item queries like <samp>RANDOM_ITEMS</samp> in data assets.</summary>
/// <remarks>This is an internal implementation class. Most code should use higher-level code like <see cref="M:StardewValley.Utility.TryOpenShopMenu(System.String,System.String,System.Boolean)" /> instead.</remarks>
/// <summary>Resolves item IDs like <samp>(O)128</samp> and item queries like <samp>RANDOM_ITEMS</samp> in data assets.</summary>
/// <remarks>This is an internal implementation class. Most code should use higher-level code like <see cref="M:StardewValley.Utility.TryOpenShopMenu(System.String,System.String,System.Boolean)" /> instead.</remarks>
public static class ItemQueryResolver
{
  /// <summary>The item query keys that can be used instead of an item ID in list data fields like <see cref="P:StardewValley.GameData.ISpawnItemData.ItemId" /> or <see cref="P:StardewValley.GameData.ISpawnItemData.RandomItemId" /> fields, and the methods which create the items for them.</summary>
  public static 
  #nullable disable
  Dictionary<string, ResolveItemQueryDelegate> ItemResolvers { get; } = new Dictionary<string, ResolveItemQueryDelegate>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

  /// <summary>Register the default item queries, defined as <see cref="T:StardewValley.Internal.ItemQueryResolver.DefaultResolvers" /> methods.</summary>
  static ItemQueryResolver()
  {
    foreach (MethodInfo method in typeof (ItemQueryResolver.DefaultResolvers).GetMethods(BindingFlags.Static | BindingFlags.Public))
    {
      ResolveItemQueryDelegate queryDelegate = (ResolveItemQueryDelegate) Delegate.CreateDelegate(typeof (ResolveItemQueryDelegate), method);
      ItemQueryResolver.Register(method.Name, queryDelegate);
    }
  }

  /// <summary>Register an item query resolver.</summary>
  /// <param name="queryKey">The item query key, like <c>ALL_ITEMS</c>. This should only contain alphanumeric, underscore, and dot characters. For custom queries, this should be prefixed with your mod ID like <c>Example.ModId_QueryName</c>.</param>
  /// <param name="queryDelegate">The resolver which returns the items produced by the item query.</param>
  /// <exception cref="T:System.ArgumentException">The <paramref name="queryKey" /> is null or whitespace-only.</exception>
  /// <exception cref="T:System.ArgumentNullException">The <paramref name="queryDelegate" /> is null.</exception>
  /// <exception cref="T:System.InvalidOperationException">The <paramref name="queryKey" /> is already registered.</exception>
  public static void Register(string queryKey, ResolveItemQueryDelegate queryDelegate)
  {
    if (string.IsNullOrWhiteSpace(queryKey))
      throw new ArgumentException("The query key can't be null or empty.", nameof (queryKey));
    Dictionary<string, ResolveItemQueryDelegate> dictionary = !ItemQueryResolver.ItemResolvers.ContainsKey(queryKey) ? ItemQueryResolver.ItemResolvers : throw new InvalidOperationException($"The query key '{queryKey}' is already registered.");
    string key = queryKey.Trim();
    dictionary[key] = queryDelegate ?? throw new ArgumentNullException(nameof (queryDelegate));
  }

  /// <summary>Get the items matching an item ID or query.</summary>
  /// <param name="query">The item ID or query to match.</param>
  /// <param name="context">The contextual info for item queries, or <c>null</c> to use the global context.</param>
  /// <param name="filter">The filter to apply to the search results.</param>
  /// <param name="perItemCondition">A game state query which indicates whether an item produced from the other fields should be returned. Defaults to always true.</param>
  /// <param name="maxItems">The maximum number of item stacks to produce, or <c>null</c> to include all stacks produced by the <paramref name="query" />.</param>
  /// <param name="avoidRepeat">Whether to avoid adding duplicate items.</param>
  /// <param name="avoidItemIds">The qualified item IDs which shouldn't be returned.</param>
  /// <param name="logError">Log an error message to the console, given the item query and error message.</param>
  public static ItemQueryResult[] TryResolve(
    string query,
    ItemQueryContext context,
    ItemQuerySearchMode filter = ItemQuerySearchMode.All,
    string perItemCondition = null,
    int? maxItems = null,
    bool avoidRepeat = false,
    HashSet<string> avoidItemIds = null,
    Action<string, string> logError = null)
  {
    if (string.IsNullOrWhiteSpace(query))
      return ItemQueryResolver.Helpers.ErrorResult(query, "", logError, "must specify an item ID or query");
    string key = query;
    string str = (string) null;
    int length = query.IndexOf(' ');
    if (length > -1)
    {
      key = query.Substring(0, length);
      str = query.Substring(length + 1);
    }
    if (context == null)
      context = new ItemQueryContext();
    context.QueryString = query;
    if (context.ParentContext != null)
    {
      List<string> values = new List<string>();
      for (ItemQueryContext itemQueryContext = context; itemQueryContext != null; itemQueryContext = itemQueryContext.ParentContext)
      {
        int num = values.Contains(itemQueryContext.QueryString) ? 1 : 0;
        values.Add(itemQueryContext.QueryString);
        if (num != 0)
        {
          if (logError != null)
            logError(query, "detected circular reference in item queries: " + string.Join(" -> ", (IEnumerable<string>) values));
          return LegacyShims.EmptyArray<ItemQueryResult>();
        }
      }
    }
    ResolveItemQueryDelegate itemQueryDelegate;
    if (ItemQueryResolver.ItemResolvers.TryGetValue(key, out itemQueryDelegate))
    {
      IEnumerable<ItemQueryResult> source = itemQueryDelegate(key, str ?? string.Empty, context, avoidRepeat, avoidItemIds, logError ?? new Action<string, string>(ItemQueryResolver.LogNothing));
      if (source is ItemQueryResult[] itemQueryResultArray1 && itemQueryResultArray1.Length == 0)
        return itemQueryResultArray1;
      HashSet<string> duplicates = avoidRepeat ? new HashSet<string>() : (HashSet<string>) null;
      if (!avoidRepeat)
      {
        HashSet<string> stringSet = avoidItemIds;
        // ISSUE: explicit non-virtual call
        if ((stringSet != null ? (__nonvirtual (stringSet.Count) > 0 ? 1 : 0) : 0) == 0 && GameStateQuery.IsImmutablyFalse(perItemCondition))
          goto label_20;
      }
      source = source.Where<ItemQueryResult>((Func<ItemQueryResult, bool>) (result =>
      {
        HashSet<string> stringSet1 = avoidItemIds;
        // ISSUE: explicit non-virtual call
        if ((stringSet1 != null ? (!__nonvirtual (stringSet1.Contains(result.Item.QualifiedItemId)) ? 1 : 0) : 1) != 0)
        {
          HashSet<string> stringSet2 = duplicates;
          // ISSUE: explicit non-virtual call
          if ((stringSet2 != null ? (__nonvirtual (stringSet2.Add(result.Item.QualifiedItemId)) ? 1 : 0) : 1) != 0)
            return GameStateQuery.CheckConditions(perItemCondition, targetItem: result.Item as Item);
        }
        return false;
      }));
label_20:
      switch (filter)
      {
        case ItemQuerySearchMode.AllOfTypeItem:
          source = source.Where<ItemQueryResult>((Func<ItemQueryResult, bool>) (result => result.Item is Item));
          break;
        case ItemQuerySearchMode.FirstOfTypeItem:
          ItemQueryResult itemQueryResult1 = source.FirstOrDefault<ItemQueryResult>((Func<ItemQueryResult, bool>) (p => p.Item is Item));
          ItemQueryResult[] itemQueryResultArray2;
          if (itemQueryResult1 == null)
            itemQueryResultArray2 = LegacyShims.EmptyArray<ItemQueryResult>();
          else
            itemQueryResultArray2 = new ItemQueryResult[1]
            {
              itemQueryResult1
            };
          source = (IEnumerable<ItemQueryResult>) itemQueryResultArray2;
          break;
        case ItemQuerySearchMode.RandomOfTypeItem:
          ItemQueryResult itemQueryResult2 = (context.Random ?? Game1.random).ChooseFrom<ItemQueryResult>((IList<ItemQueryResult>) source.Where<ItemQueryResult>((Func<ItemQueryResult, bool>) (p => p.Item is Item)).ToArray<ItemQueryResult>());
          ItemQueryResult[] itemQueryResultArray3;
          if (itemQueryResult2 == null)
            itemQueryResultArray3 = LegacyShims.EmptyArray<ItemQueryResult>();
          else
            itemQueryResultArray3 = new ItemQueryResult[1]
            {
              itemQueryResult2
            };
          source = (IEnumerable<ItemQueryResult>) itemQueryResultArray3;
          break;
      }
      if (maxItems.HasValue)
        source = source.Take<ItemQueryResult>(maxItems.Value);
      return source is ItemQueryResult[] itemQueryResultArray4 ? itemQueryResultArray4 : source.ToArray<ItemQueryResult>();
    }
    Item obj = ItemRegistry.Create(query);
    if (obj != null)
    {
      HashSet<string> stringSet = avoidItemIds;
      // ISSUE: explicit non-virtual call
      if ((stringSet != null ? (!__nonvirtual (stringSet.Contains(obj.QualifiedItemId)) ? 1 : 0) : 1) != 0)
        return new ItemQueryResult[1]
        {
          new ItemQueryResult((ISalable) obj)
        };
    }
    return LegacyShims.EmptyArray<ItemQueryResult>();
  }

  /// <summary>Get the items matching spawn data from a content asset.</summary>
  /// <param name="data">The spawn data to match.</param>
  /// <param name="context">The contextual info for item queries, or <c>null</c> to use the global context.</param>
  /// <param name="filter">The filter to apply to the search results.</param>
  /// <param name="avoidRepeat">Whether to avoid adding duplicate items.</param>
  /// <param name="avoidItemIds">The qualified item IDs which shouldn't be returned.</param>
  /// <param name="formatItemId">Format the raw item ID before it's resolved. Note that this is applied after <paramref name="avoidRepeat" /> and <paramref name="avoidItemIds" /> are checked.</param>
  /// <param name="inputItem">The input item (e.g. machine input) for which to check queries, or <c>null</c> if not applicable.</param>
  /// <param name="logError">Log an error message to the console, given the item query and error message.</param>
  public static IList<ItemQueryResult> TryResolve(
    ISpawnItemData data,
    ItemQueryContext context,
    ItemQuerySearchMode filter = ItemQuerySearchMode.All,
    bool avoidRepeat = false,
    HashSet<string> avoidItemIds = null,
    Func<string, string> formatItemId = null,
    Action<string, string> logError = null,
    Item inputItem = null)
  {
    Random random = context?.Random ?? Game1.random;
    string selected = data.ItemId;
    List<string> randomItemId = data.RandomItemId;
    if ((randomItemId != null ? (randomItemId.Any<string>() ? 1 : 0) : 0) != 0)
    {
      if (avoidItemIds != null)
      {
        if (!Utility.TryGetRandomExcept<string>((IList<string>) data.RandomItemId, (ISet<string>) avoidItemIds, random, out selected))
          return (IList<ItemQueryResult>) LegacyShims.EmptyArray<ItemQueryResult>();
      }
      else
        selected = random.ChooseFrom<string>((IList<string>) data.RandomItemId);
    }
    if (string.IsNullOrWhiteSpace(selected))
    {
      Game1.log.Warn(ItemQueryResolver.FormatLogMessage("Item spawn fields for {0} produced a null or empty item ID.", data, context));
      return (IList<ItemQueryResult>) LegacyShims.EmptyArray<ItemQueryResult>();
    }
    if (formatItemId != null)
      selected = formatItemId(selected);
    ItemQueryResult[] itemQueryResultArray = ItemQueryResolver.TryResolve(selected, context, filter, data.PerItemCondition, data.MaxItems, avoidRepeat, avoidItemIds, logError);
    foreach (ItemQueryResult itemQueryResult in itemQueryResultArray)
      itemQueryResult.Item = ItemQueryResolver.ApplyItemFields(itemQueryResult.Item, data, context, inputItem);
    return (IList<ItemQueryResult>) itemQueryResultArray;
  }

  /// <summary>Get a random item matching an item ID or query.</summary>
  /// <param name="query">The item ID or query to match.</param>
  /// <param name="context">The contextual info for item queries, or <c>null</c> to use the global context.</param>
  /// <param name="avoidRepeat">Whether to avoid adding duplicate items.</param>
  /// <param name="avoidItemIds">The qualified item IDs which shouldn't be returned.</param>
  /// <param name="logError">Log an error message to the console, given the item query and error message.</param>
  public static Item TryResolveRandomItem(
    string query,
    ItemQueryContext context,
    bool avoidRepeat = false,
    HashSet<string> avoidItemIds = null,
    Action<string, string> logError = null)
  {
    return ((IEnumerable<ItemQueryResult>) ItemQueryResolver.TryResolve(query, context, ItemQuerySearchMode.RandomOfTypeItem, avoidRepeat: avoidRepeat, avoidItemIds: avoidItemIds, logError: logError)).FirstOrDefault<ItemQueryResult>()?.Item as Item;
  }

  /// <summary>Get the items matching spawn data from a content asset.</summary>
  /// <param name="data">The spawn data to match.</param>
  /// <param name="context">The contextual info for item queries, or <c>null</c> to use the global context.</param>
  /// <param name="avoidRepeat">Whether to avoid adding duplicate items.</param>
  /// <param name="avoidItemIds">The qualified item IDs which shouldn't be returned.</param>
  /// <param name="formatItemId">Format the selected item ID before it's resolved.</param>
  /// <param name="inputItem">The input item (e.g. machine input) for which to check queries, or <c>null</c> if not applicable.</param>
  /// <param name="logError">Log an error message to the console, given the item query and error message.</param>
  public static Item TryResolveRandomItem(
    ISpawnItemData data,
    ItemQueryContext context,
    bool avoidRepeat = false,
    HashSet<string> avoidItemIds = null,
    Func<string, string> formatItemId = null,
    Item inputItem = null,
    Action<string, string> logError = null)
  {
    return ItemQueryResolver.TryResolve(data, context, ItemQuerySearchMode.RandomOfTypeItem, avoidRepeat, avoidItemIds, formatItemId, logError, inputItem).FirstOrDefault<ItemQueryResult>()?.Item as Item;
  }

  /// <summary>Apply data fields to an item instance.</summary>
  /// <param name="item">The item to modify.</param>
  /// <param name="data">The spawn data to apply.</param>
  /// <param name="context">The contextual info for item queries, or <c>null</c> to use the global context.</param>
  /// <remarks>This is applied automatically by methods which take an <see cref="T:StardewValley.GameData.ISpawnItemData" />, so it only needs to be called directly when creating an item from an item query string directly.</remarks>
  /// <param name="inputItem">The input item (e.g. machine input) for which to check queries, or <c>null</c> if not applicable.</param>
  /// <returns>Returns the modified item. This is usually the input <paramref name="item" />, but may be a new item instance in some cases.</returns>
  public static ISalable ApplyItemFields(
    ISalable item,
    ISpawnItemData data,
    ItemQueryContext context,
    Item inputItem = null)
  {
    return ItemQueryResolver.ApplyItemFields(item, data.MinStack, data.MaxStack, data.ToolUpgradeLevel, data.ObjectInternalName, data.ObjectDisplayName, data.ObjectColor, data.Quality, data.IsRecipe, data.StackModifiers, data.StackModifierMode, data.QualityModifiers, data.QualityModifierMode, data.ModData, context, inputItem);
  }

  /// <summary>Apply data fields to an item instance.</summary>
  /// <param name="item">The item to modify.</param>
  /// <param name="minStackSize">The minimum stack size for the item to create, or <c>-1</c> to keep it as-is.</param>
  /// <param name="maxStackSize">The maximum stack size for the item to create, or <c>-1</c> to match <paramref name="minStackSize" />.</param>
  /// <param name="toolUpgradeLevel"><inheritdoc cref="P:StardewValley.GameData.ISpawnItemData.ToolUpgradeLevel" path="/summary" /></param>
  /// <param name="objectInternalName"><inheritdoc cref="P:StardewValley.GameData.ISpawnItemData.ObjectInternalName" path="/summary" /></param>
  /// <param name="objectDisplayName"><inheritdoc cref="P:StardewValley.GameData.ISpawnItemData.ObjectDisplayName" path="/summary" /></param>
  /// <param name="objectColor"><inheritdoc cref="P:StardewValley.GameData.ISpawnItemData.ObjectColor" path="/summary" /></param>
  /// <param name="quality"><inheritdoc cref="P:StardewValley.GameData.ISpawnItemData.Quality" path="/summary" /></param>
  /// <param name="isRecipe"><inheritdoc cref="P:StardewValley.GameData.ISpawnItemData.IsRecipe" path="/summary" /></param>
  /// <param name="stackSizeModifiers">The modifiers to apply to the item's stack size.</param>
  /// <param name="stackSizeModifierMode">How multiple <paramref name="stackSizeModifiers" /> should be combined.</param>
  /// <param name="qualityModifiers">The modifiers to apply to the item's quality.</param>
  /// <param name="qualityModifierMode">How multiple <paramref name="qualityModifiers" /> should be combined.</param>
  /// <param name="modData"><inheritdoc cref="P:StardewValley.GameData.ISpawnItemData.ModData" path="/summary" /></param>
  /// <param name="context">The contextual info for item queries, or <c>null</c> to use the global context.</param>
  /// <param name="inputItem">The input item (e.g. machine input) for which to check queries, or <c>null</c> if not applicable.</param>
  /// <returns>Returns the modified item. This is usually the input <paramref name="item" />, but may be a new item instance in some cases.</returns>
  /// <remarks>This is applied automatically by methods which take an <see cref="T:StardewValley.GameData.ISpawnItemData" />, so it only needs to be called directly when creating an item from an item query string directly.</remarks>
  public static ISalable ApplyItemFields(
    ISalable item,
    int minStackSize,
    int maxStackSize,
    int toolUpgradeLevel,
    string objectInternalName,
    string objectDisplayName,
    string objectColor,
    int quality,
    bool isRecipe,
    List<QuantityModifier> stackSizeModifiers,
    QuantityModifier.QuantityModifierMode stackSizeModifierMode,
    List<QuantityModifier> qualityModifiers,
    QuantityModifier.QuantityModifierMode qualityModifierMode,
    Dictionary<string, string> modData,
    ItemQueryContext context,
    Item inputItem = null)
  {
    if (item == null)
      return (ISalable) null;
    Ring ring = item as Ring;
    if (ring != null & isRecipe)
      item = (ISalable) new StardewValley.Object(ring.ItemId, ring.Stack, true);
    int num = 1;
    if (!isRecipe)
    {
      if (minStackSize == -1 && maxStackSize == -1)
        num = item.Stack;
      else if (maxStackSize > 1)
      {
        minStackSize = Math.Max(minStackSize, 1);
        maxStackSize = Math.Max(maxStackSize, minStackSize);
        num = (context?.Random ?? Game1.random).Next(minStackSize, maxStackSize + 1);
      }
      else if (minStackSize > 1)
        num = minStackSize;
      num = (int) Utility.ApplyQuantityModifiers((float) num, (IList<QuantityModifier>) stackSizeModifiers, stackSizeModifierMode, context?.Location, context?.Player, item as Item, inputItem, context?.Random);
    }
    quality = quality >= 0 ? quality : item.Quality;
    quality = (int) Utility.ApplyQuantityModifiers((float) quality, (IList<QuantityModifier>) qualityModifiers, qualityModifierMode, context?.Location, context?.Player, item as Item, inputItem, context?.Random);
    if (isRecipe)
      item.IsRecipe = true;
    if (num > -1 && num != item.Stack)
    {
      item.Stack = num;
      item.FixStackSize();
    }
    if (quality >= 0 && quality != item.Quality)
    {
      item.Quality = quality;
      item.FixQuality();
    }
    // ISSUE: explicit non-virtual call
    if (modData != null && __nonvirtual (modData.Count) > 0 && item is Item obj)
    {
      // ISSUE: explicit non-virtual call
      __nonvirtual (obj.modData).CopyFrom((IEnumerable<KeyValuePair<string, string>>) modData);
    }
    switch (item)
    {
      case StardewValley.Object input:
        if (!string.IsNullOrWhiteSpace(objectInternalName))
          input.Name = objectInternalName;
        if (!string.IsNullOrWhiteSpace(objectDisplayName))
          input.displayNameFormat = objectDisplayName;
        if (!string.IsNullOrWhiteSpace(objectColor) && item.HasTypeObject())
        {
          Color? color = Utility.StringToColor(objectColor);
          ColoredObject coloredItem;
          if (color.HasValue && ColoredObject.TrySetColor((Item) input, color.Value, out coloredItem))
          {
            StardewValley.Object @object;
            item = (ISalable) (@object = (StardewValley.Object) coloredItem);
            break;
          }
          break;
        }
        break;
      case Tool tool when toolUpgradeLevel > -1 && toolUpgradeLevel != tool.UpgradeLevel:
        tool.UpgradeLevel = toolUpgradeLevel;
        break;
    }
    return item;
  }

  /// <summary>Build a log message with the source context.</summary>
  /// <param name="template">The template message to log, where <c>{0}</c> is the source phrase. See remarks on <see cref="P:StardewValley.Internal.ItemQueryContext.SourcePhrase" />.</param>
  /// <param name="data">The item spawn data.</param>
  /// <param name="context">The contextual info for item queries, or <c>null</c> to use the global context.</param>
  public static string FormatLogMessage(
    string template,
    ISpawnItemData data,
    ItemQueryContext context)
  {
    string id = data is GenericSpawnItemData genericSpawnItemData ? genericSpawnItemData.Id : (string) null;
    string str = context == null || context.SourcePhrase == null ? (id == null ? "unknown context" : $"entry '{id}'") : (id != null ? $"{context.SourcePhrase} > entry '{id}'" : context.SourcePhrase);
    return string.Format(template, (object) str);
  }

  /// <summary>A default implementation for <c>logError</c> parameters which logs nothing.</summary>
  /// <param name="query">The item query which failed.</param>
  /// <param name="error">The error indicating why it failed.</param>
  private static void LogNothing(string query, string error)
  {
  }

  /// <summary>The helper methods which simplify implementing custom item queries.</summary>
  public static class Helpers
  {
    /// <summary>Split an argument list into individual arguments.</summary>
    /// <param name="arguments">The arguments to split.</param>
    public static string[] SplitArguments(string arguments)
    {
      return arguments.Length <= 0 ? LegacyShims.EmptyArray<string>() : ArgUtility.SplitBySpace(arguments);
    }

    /// <summary>Log an error for an invalid query, and return an empty list of items.</summary>
    /// <param name="key">The query key specified in the item ID.</param>
    /// <param name="arguments">Any text specified in the item ID after the <paramref name="key" />.</param>
    /// <param name="logError">Log an error message to the console, given the item query and error message.</param>
    /// <param name="message">A human-readable message indicating why the query is invalid.</param>
    public static ItemQueryResult[] ErrorResult(
      string key,
      string arguments,
      Action<string, string> logError,
      string message)
    {
      if (logError != null)
        logError($"{key} {arguments}".Trim(), message);
      return LegacyShims.EmptyArray<ItemQueryResult>();
    }

    /// <summary>Get whether to exclude this item from shops when selecting random items to sell, including catalogues.</summary>
    /// <param name="data">The parsed item data.</param>
    public static bool ExcludeFromRandomSale(ParsedItemData data)
    {
      if (data.ExcludeFromRandomSale)
        return true;
      switch (data.GetItemTypeId())
      {
        case "(WP)":
          if (Utility.isWallpaperOffLimitsForSale(data.ItemId))
            return true;
          break;
        case "(FL)":
          if (Utility.isFlooringOffLimitsForSale(data.ItemId))
            return true;
          break;
      }
      return false;
    }
  }

  /// <summary>The resolvers for vanilla item queries. Most code should call <c>TryResolve</c> instead of using these directly.</summary>
  public static class DefaultResolvers
  {
    /// <summary>Get every item in the game, optionally filtered by type. Format: <c>ALL_ITEMS [type]</c>.</summary>
    /// <inheritdoc cref="T:StardewValley.Delegates.ResolveItemQueryDelegate" />
    public static IEnumerable<ItemQueryResult> ALL_ITEMS(
      string key,
      string arguments,
      ItemQueryContext context,
      bool avoidRepeat,
      HashSet<string> avoidItemIds,
      Action<string, string> logError)
    {
      string onlyTypeId = (string) null;
      bool isRandomSale = false;
      bool requirePrice = false;
      string[] array = ItemQueryResolver.Helpers.SplitArguments(arguments);
      int num = 0;
      if (ArgUtility.HasIndex<string>(array, 0) && !array[0].StartsWith('@'))
      {
        onlyTypeId = array[0];
        ++num;
      }
      for (int index = num; index < array.Length; ++index)
      {
        string str = array[index];
        if (str.EqualsIgnoreCase("@isRandomSale"))
          isRandomSale = true;
        else if (str.EqualsIgnoreCase("@requirePrice"))
        {
          requirePrice = true;
        }
        else
        {
          if (str.StartsWith('@'))
          {
            ItemQueryResolver.Helpers.ErrorResult(key, arguments, logError, $"index {index} has unknown option flag '{str}'");
            yield break;
          }
          if (onlyTypeId != null && onlyTypeId != str)
          {
            ItemQueryResolver.Helpers.ErrorResult(key, arguments, logError, $"index {index} must be an option flag starting with '@'");
            yield break;
          }
          onlyTypeId = str;
        }
      }
      foreach (IItemDataDefinition itemType in ItemRegistry.ItemTypes)
      {
        string identifier = itemType.Identifier;
        if (onlyTypeId == null || !(identifier != onlyTypeId))
        {
          if (identifier == "(F)")
          {
            List<Furniture> furnitureList = new List<Furniture>();
            foreach (ParsedItemData data in itemType.GetAllData())
            {
              if (!isRandomSale || !ItemQueryResolver.Helpers.ExcludeFromRandomSale(data))
              {
                Furniture furniture = ItemRegistry.Create<Furniture>(data.QualifiedItemId);
                if (!requirePrice || furniture.salePrice(true) > 0)
                  furnitureList.Add(furniture);
              }
            }
            furnitureList.Sort(new Comparison<Furniture>(Utility.SortAllFurnitures));
            foreach (ISalable salable in furnitureList)
              yield return new ItemQueryResult(salable);
          }
          else
          {
            foreach (ParsedItemData data in itemType.GetAllData())
            {
              if (!isRandomSale || !ItemQueryResolver.Helpers.ExcludeFromRandomSale(data))
              {
                Item obj = ItemRegistry.Create(data.QualifiedItemId);
                if (!requirePrice || obj.salePrice(true) > 0)
                  yield return new ItemQueryResult((ISalable) obj);
              }
            }
          }
        }
      }
    }

    /// <summary>Get the dish of the day sold at the Saloon, if any. Format: <c>DISH_OF_THE_DAY</c> (no arguments).</summary>
    /// <inheritdoc cref="T:StardewValley.Delegates.ResolveItemQueryDelegate" />
    public static IEnumerable<ItemQueryResult> DISH_OF_THE_DAY(
      string key,
      string arguments,
      ItemQueryContext context,
      bool avoidRepeat,
      HashSet<string> avoidItemIds,
      Action<string, string> logError)
    {
      if (Game1.dishOfTheDay == null)
        return (IEnumerable<ItemQueryResult>) LegacyShims.EmptyArray<ItemQueryResult>();
      return (IEnumerable<ItemQueryResult>) new ItemQueryResult[1]
      {
        new ItemQueryResult((ISalable) Game1.dishOfTheDay.getOne())
        {
          OverrideShopAvailableStock = new int?(Game1.dishOfTheDay.Stack),
          SyncStacksWith = (Item) Game1.dishOfTheDay
        }
      };
    }

    /// <summary>Get a flavored item for a given type and ingredient (like Wine + Blueberry = Blueberry Wine). Format: <c>FLAVORED_ITEM &lt;type&gt; &lt;ingredient item ID&gt; [ingredient preserved ID]</c>.</summary>
    /// <inheritdoc cref="T:StardewValley.Delegates.ResolveItemQueryDelegate" />
    public static IEnumerable<ItemQueryResult> FLAVORED_ITEM(
      string key,
      string arguments,
      ItemQueryContext context,
      bool avoidRepeat,
      HashSet<string> avoidItemIds,
      Action<string, string> logError)
    {
      int num = 0;
      bool flag = false;
      string[] array = ItemQueryResolver.Helpers.SplitArguments(arguments);
      StardewValley.Object.PreserveType parsed;
      if (!Utility.TryParseEnum<StardewValley.Object.PreserveType>(array[0], out parsed))
        return (IEnumerable<ItemQueryResult>) ItemQueryResolver.Helpers.ErrorResult(key, arguments, logError, $"invalid flavored item type (must be one of {string.Join(", ", Enum.GetNames(typeof (StardewValley.Object.PreserveType)))})");
      string itemId1 = ArgUtility.Get(array, 1);
      string itemId2;
      if (parsed == StardewValley.Object.PreserveType.Honey && itemId1 == "-1")
      {
        flag = true;
        itemId2 = (string) null;
      }
      else
      {
        itemId2 = ItemRegistry.QualifyItemId(itemId1);
        if (itemId2 == null)
          return (IEnumerable<ItemQueryResult>) ItemQueryResolver.Helpers.ErrorResult(key, arguments, logError, "must specify a valid flavor ingredient ID");
      }
      string itemId3 = ArgUtility.Get(array, 2);
      if (itemId3 == "0")
        itemId3 = (string) null;
      ArgUtility.TryGetOptionalInt(array, 2, out num, out string _, name: "quality");
      ObjectDataDefinition objectTypeDefinition = ItemRegistry.GetObjectTypeDefinition();
      StardewValley.Object ingredient = (StardewValley.Object) null;
      if (!flag)
      {
        try
        {
          ingredient = parsed != StardewValley.Object.PreserveType.AgedRoe || !(itemId2 == "(O)812") || itemId3 == null ? ItemRegistry.Create(itemId2) as StardewValley.Object : objectTypeDefinition.CreateFlavoredItem(StardewValley.Object.PreserveType.Roe, ItemRegistry.Create<StardewValley.Object>(itemId3));
        }
        catch (Exception ex)
        {
          return (IEnumerable<ItemQueryResult>) ItemQueryResolver.Helpers.ErrorResult(key, arguments, logError, ex.Message);
        }
        if (ingredient != null)
          ingredient.Quality = num;
      }
      StardewValley.Object flavoredItem = objectTypeDefinition.CreateFlavoredItem(parsed, ingredient);
      if (flavoredItem == null)
        return (IEnumerable<ItemQueryResult>) ItemQueryResolver.Helpers.ErrorResult(key, arguments, logError, $"unsupported flavor type '{parsed}'.");
      return (IEnumerable<ItemQueryResult>) new ItemQueryResult[1]
      {
        new ItemQueryResult((ISalable) flavoredItem)
      };
    }

    /// <summary>Get the items lost when the player collapsed in the mines, which can be recovered from Marlon's shop. Format: <c>ITEMS_LOST_ON_DEATH</c> (no arguments).</summary>
    /// <inheritdoc cref="T:StardewValley.Delegates.ResolveItemQueryDelegate" />
    public static IEnumerable<ItemQueryResult> ITEMS_LOST_ON_DEATH(
      string key,
      string arguments,
      ItemQueryContext context,
      bool avoidRepeat,
      HashSet<string> avoidItemIds,
      Action<string, string> logError)
    {
      List<ItemQueryResult> itemQueryResultList = new List<ItemQueryResult>();
      foreach (Item i in (NetList<Item, NetRef<Item>>) Game1.player.itemsLostLastDeath)
      {
        if (i != null)
        {
          i.isLostItem = true;
          itemQueryResultList.Add(new ItemQueryResult((ISalable) i)
          {
            OverrideStackSize = new int?(i.Stack),
            OverrideBasePrice = new int?(Game1.player.stats.Get("Book_Marlon") > 0U ? (int) ((double) Utility.getSellToStorePriceOfItem(i) * 0.5) : Utility.getSellToStorePriceOfItem(i))
          });
        }
      }
      return (IEnumerable<ItemQueryResult>) itemQueryResultList;
    }

    /// <summary>Get items the player has recently sold to a given shop. Format: <c>ITEMS_SOLD_BY_PLAYER &lt;shop location ID&gt;</c>.</summary>
    /// <inheritdoc cref="T:StardewValley.Delegates.ResolveItemQueryDelegate" />
    public static IEnumerable<ItemQueryResult> ITEMS_SOLD_BY_PLAYER(
      string key,
      string arguments,
      ItemQueryContext context,
      bool avoidRepeat,
      HashSet<string> avoidItemIds,
      Action<string, string> logError)
    {
      string name = arguments;
      if (string.IsNullOrWhiteSpace(name))
      {
        ItemQueryResolver.Helpers.ErrorResult(key, arguments, logError, "must specify a location ID");
      }
      else
      {
        GameLocation locationFromName = Game1.getLocationFromName(name);
        if (locationFromName == null)
          ItemQueryResolver.Helpers.ErrorResult(key, arguments, logError, "the specified location ID didn't match any location");
        else if (!(locationFromName is ShopLocation shopLocation))
        {
          ItemQueryResolver.Helpers.ErrorResult(key, arguments, logError, "the specified location ID matched a location which isn't a ShopLocation instance");
        }
        else
        {
          foreach (Item obj in (NetList<Item, NetRef<Item>>) shopLocation.itemsFromPlayerToSell)
          {
            if (obj.Stack > 0)
            {
              int num = obj is StardewValley.Object @object ? @object.sellToStorePrice(-1L) : obj.salePrice(false);
              yield return new ItemQueryResult((ISalable) obj.getOne())
              {
                OverrideBasePrice = new int?(num),
                OverrideShopAvailableStock = new int?(obj.Stack),
                SyncStacksWith = obj
              };
            }
          }
        }
      }
    }

    /// <summary>Get a fish which can be caught in a location based on its <c>Data/Locations</c> entry. Format: <c>LOCATION_FISH &lt;location name&gt; &lt;bobber x&gt; &lt;bobber y&gt; &lt;water depth&gt;</c>.</summary>
    /// <inheritdoc cref="T:StardewValley.Delegates.ResolveItemQueryDelegate" />
    public static IEnumerable<ItemQueryResult> LOCATION_FISH(
      string key,
      string arguments,
      ItemQueryContext context,
      bool avoidRepeat,
      HashSet<string> avoidItemIds,
      Action<string, string> logError)
    {
      string[] strArray = ItemQueryResolver.Helpers.SplitArguments(arguments);
      if (strArray.Length != 4)
        return (IEnumerable<ItemQueryResult>) ItemQueryResolver.Helpers.ErrorResult(key, arguments, logError, "expected four arguments in the form <location name> <bobber x> <bobber y> <depth>");
      string locationName = strArray[0];
      string s1 = strArray[1];
      string s2 = strArray[2];
      string s3 = strArray[3];
      int result1;
      int result2;
      if (!int.TryParse(s1, out result1) || !int.TryParse(s2, out result2))
        return (IEnumerable<ItemQueryResult>) ItemQueryResolver.Helpers.ErrorResult(key, arguments, logError, $"can't parse '{s1} {s2}' as numeric 'x y' values");
      int result3;
      if (!int.TryParse(s3, out result3))
        return (IEnumerable<ItemQueryResult>) ItemQueryResolver.Helpers.ErrorResult(key, arguments, logError, $"can't parse '{s3}' as a numeric depth value");
      Item fromLocationData = GameLocation.GetFishFromLocationData(locationName, new Vector2((float) result1, (float) result2), result3, context?.Player, false, true);
      if (fromLocationData == null)
        return (IEnumerable<ItemQueryResult>) LegacyShims.EmptyArray<ItemQueryResult>();
      return (IEnumerable<ItemQueryResult>) new ItemQueryResult[1]
      {
        new ItemQueryResult((ISalable) fromLocationData)
      };
    }

    /// <summary>Get a lost book (if they haven't all been found), else the given item query (if provided), else nothing. Format: <c>LOST_BOOK_OR_ITEM [alternate item query]</c>.</summary>
    /// <inheritdoc cref="T:StardewValley.Delegates.ResolveItemQueryDelegate" />
    public static IEnumerable<ItemQueryResult> LOST_BOOK_OR_ITEM(
      string key,
      string arguments,
      ItemQueryContext context,
      bool avoidRepeat,
      HashSet<string> avoidItemIds,
      Action<string, string> logError)
    {
      return Game1.netWorldState.Value.LostBooksFound < 21 ? (IEnumerable<ItemQueryResult>) new ItemQueryResult[1]
      {
        new ItemQueryResult((ISalable) ItemRegistry.Create("(O)102"))
      } : (string.IsNullOrWhiteSpace(arguments) ? (IEnumerable<ItemQueryResult>) LegacyShims.EmptyArray<ItemQueryResult>() : (IEnumerable<ItemQueryResult>) ItemQueryResolver.TryResolve(arguments, new ItemQueryContext(context, "query 'LOST_BOOK_OR_ITEM'")));
    }

    /// <summary>Get the unique items which no longer exist anywhere in the world, which are shown in the crow lost-items shop. Format: <c>LOST_UNIQUE_ITEMS</c> (no arguments).</summary>
    /// <inheritdoc cref="T:StardewValley.Delegates.ResolveItemQueryDelegate" />
    public static IEnumerable<ItemQueryResult> LOST_UNIQUE_ITEMS(
      string key,
      string arguments,
      ItemQueryContext context,
      bool avoidRepeat,
      HashSet<string> avoidItemIds,
      Action<string, string> logError)
    {
      List<ItemQueryResult> itemQueryResultList = new List<ItemQueryResult>();
      foreach (Item obj in (IEnumerable<Item>) Woods.GetLostItemsShopInventory())
      {
        if (obj != null && obj.Stack > 0)
          itemQueryResultList.Add(new ItemQueryResult((ISalable) obj)
          {
            OverrideStackSize = new int?(obj.Stack),
            SyncStacksWith = obj
          });
      }
      return (IEnumerable<ItemQueryResult>) itemQueryResultList;
    }

    /// <summary>Get the rewards that can currently be collected from Gil in the Adventurer's Guild. Format: <c>MONSTER_SLAYER_REWARDS</c> (no arguments).</summary>
    /// <inheritdoc cref="T:StardewValley.Delegates.ResolveItemQueryDelegate" />
    public static IEnumerable<ItemQueryResult> MONSTER_SLAYER_REWARDS(
      string key,
      string arguments,
      ItemQueryContext context,
      bool avoidRepeat,
      HashSet<string> avoidItemIds,
      Action<string, string> logError)
    {
      KeyValuePair<string, MonsterSlayerQuestData>[] monsterSlayerQuestData = DataLoader.MonsterSlayerQuests(Game1.content).Where<KeyValuePair<string, MonsterSlayerQuestData>>((Func<KeyValuePair<string, MonsterSlayerQuestData>, bool>) (p => AdventureGuild.HasCollectedReward(context.Player, p.Key))).ToArray<KeyValuePair<string, MonsterSlayerQuestData>>();
      HashSet<string> questIds = new HashSet<string>();
      KeyValuePair<string, MonsterSlayerQuestData>[] keyValuePairArray = monsterSlayerQuestData;
      int index;
      string id;
      for (index = 0; index < keyValuePairArray.Length; ++index)
      {
        KeyValuePair<string, MonsterSlayerQuestData> keyValuePair = keyValuePairArray[index];
        id = keyValuePair.Key;
        MonsterSlayerQuestData monsterSlayerQuestData1 = keyValuePair.Value;
        if (!questIds.Contains(id))
        {
          if (monsterSlayerQuestData1.RewardItemId != null && monsterSlayerQuestData1.RewardItemPrice != -1)
          {
            if (ItemContextTagManager.HasBaseTag(monsterSlayerQuestData1.RewardItemId, "item_type_ring"))
            {
              yield return new ItemQueryResult((ISalable) ItemRegistry.Create(monsterSlayerQuestData1.RewardItemId))
              {
                OverrideBasePrice = new int?(monsterSlayerQuestData1.RewardItemPrice),
                OverrideShopAvailableStock = new int?(int.MaxValue)
              };
              questIds.Add(id);
            }
            else
              continue;
          }
          id = (string) null;
        }
      }
      keyValuePairArray = (KeyValuePair<string, MonsterSlayerQuestData>[]) null;
      keyValuePairArray = monsterSlayerQuestData;
      for (index = 0; index < keyValuePairArray.Length; ++index)
      {
        KeyValuePair<string, MonsterSlayerQuestData> keyValuePair = keyValuePairArray[index];
        id = keyValuePair.Key;
        MonsterSlayerQuestData monsterSlayerQuestData2 = keyValuePair.Value;
        if (!questIds.Contains(id))
        {
          if (monsterSlayerQuestData2.RewardItemId != null && monsterSlayerQuestData2.RewardItemPrice != -1)
          {
            if (!(ItemRegistry.ResolveMetadata(monsterSlayerQuestData2.RewardItemId)?.GetTypeDefinition()?.Identifier != "(H)"))
            {
              yield return new ItemQueryResult((ISalable) ItemRegistry.Create(monsterSlayerQuestData2.RewardItemId))
              {
                OverrideBasePrice = new int?(monsterSlayerQuestData2.RewardItemPrice),
                OverrideShopAvailableStock = new int?(int.MaxValue)
              };
              questIds.Add(id);
            }
            else
              continue;
          }
          id = (string) null;
        }
      }
      keyValuePairArray = (KeyValuePair<string, MonsterSlayerQuestData>[]) null;
      keyValuePairArray = monsterSlayerQuestData;
      for (index = 0; index < keyValuePairArray.Length; ++index)
      {
        KeyValuePair<string, MonsterSlayerQuestData> keyValuePair = keyValuePairArray[index];
        id = keyValuePair.Key;
        MonsterSlayerQuestData monsterSlayerQuestData3 = keyValuePair.Value;
        if (!questIds.Contains(id))
        {
          if (monsterSlayerQuestData3.RewardItemId != null && monsterSlayerQuestData3.RewardItemPrice != -1)
          {
            if (!(ItemRegistry.ResolveMetadata(monsterSlayerQuestData3.RewardItemId)?.GetTypeDefinition()?.Identifier != "(W)"))
            {
              yield return new ItemQueryResult((ISalable) ItemRegistry.Create(monsterSlayerQuestData3.RewardItemId))
              {
                OverrideBasePrice = new int?(monsterSlayerQuestData3.RewardItemPrice),
                OverrideShopAvailableStock = new int?(int.MaxValue)
              };
              questIds.Add(id);
            }
            else
              continue;
          }
          id = (string) null;
        }
      }
      keyValuePairArray = (KeyValuePair<string, MonsterSlayerQuestData>[]) null;
      keyValuePairArray = monsterSlayerQuestData;
      for (index = 0; index < keyValuePairArray.Length; ++index)
      {
        KeyValuePair<string, MonsterSlayerQuestData> keyValuePair = keyValuePairArray[index];
        id = keyValuePair.Key;
        MonsterSlayerQuestData monsterSlayerQuestData4 = keyValuePair.Value;
        if (!questIds.Contains(id))
        {
          if (monsterSlayerQuestData4.RewardItemId != null && monsterSlayerQuestData4.RewardItemPrice != -1)
          {
            yield return new ItemQueryResult((ISalable) ItemRegistry.Create(monsterSlayerQuestData4.RewardItemId))
            {
              OverrideBasePrice = new int?(monsterSlayerQuestData4.RewardItemPrice),
              OverrideShopAvailableStock = new int?(int.MaxValue)
            };
            questIds.Add(id);
          }
          id = (string) null;
        }
      }
      keyValuePairArray = (KeyValuePair<string, MonsterSlayerQuestData>[]) null;
    }

    /// <summary>Get the movie concessions to show for an invited NPC. Format <c>MOVIE_CONCESSIONS_FOR_GUEST [npcName]</c>.</summary>
    /// <inheritdoc cref="T:StardewValley.Delegates.ResolveItemQueryDelegate" />
    public static IEnumerable<ItemQueryResult> MOVIE_CONCESSIONS_FOR_GUEST(
      string key,
      string arguments,
      ItemQueryContext context,
      bool avoidRepeat,
      HashSet<string> avoidItemIds,
      Action<string, string> logError)
    {
      string npc_name = ArgUtility.SplitBySpaceAndGet(arguments, 0);
      foreach (ISalable salable in npc_name != null ? MovieTheater.GetConcessionsForGuest(npc_name) : MovieTheater.GetConcessionsForGuest())
        yield return new ItemQueryResult(salable);
    }

    /// <summary>Get the first artifact in <c>Data/Objects</c> which lists the current location as a spawn location and whose chance matches. Format <c>RANDOM_ARTIFACT_FOR_DIG_SPOT</c> (no arguments).</summary>
    /// <inheritdoc cref="T:StardewValley.Delegates.ResolveItemQueryDelegate" />
    public static IEnumerable<ItemQueryResult> RANDOM_ARTIFACT_FOR_DIG_SPOT(
      string key,
      string arguments,
      ItemQueryContext context,
      bool avoidRepeat,
      HashSet<string> avoidItemIds,
      Action<string, string> logError)
    {
      Random random = context.Random ?? Game1.random;
      Farmer player = context.Player;
      string name = context.Location.Name;
      int num1 = (player.CurrentTool is Hoe currentTool ? (currentTool.hasEnchantmentOfType<ArchaeologistEnchantment>() ? 1 : 0) : 0) != 0 ? 2 : 1;
      foreach (ParsedItemData parsedItemData in ItemRegistry.GetObjectTypeDefinition().GetAllData())
      {
        if (!(parsedItemData.ObjectType != "Arch"))
        {
          Dictionary<string, float> artifactSpotChances = parsedItemData.RawData is ObjectData rawData ? rawData.ArtifactSpotChances : (Dictionary<string, float>) null;
          float num2;
          if (artifactSpotChances != null && artifactSpotChances.TryGetValue(name, out num2) && random.NextBool((float) num1 * num2))
            return (IEnumerable<ItemQueryResult>) new ItemQueryResult[1]
            {
              new ItemQueryResult((ISalable) ItemRegistry.Create(parsedItemData.QualifiedItemId))
            };
        }
      }
      return (IEnumerable<ItemQueryResult>) LegacyShims.EmptyArray<ItemQueryResult>();
    }

    /// <summary>Get a random seasonal vanilla item which can be found by searching garbage cans, breaking containers in the mines, etc. Format: <c>RANDOM_BASE_SEASON_ITEM</c> (no arguments).</summary>
    /// <inheritdoc cref="T:StardewValley.Delegates.ResolveItemQueryDelegate" />
    public static IEnumerable<ItemQueryResult> RANDOM_BASE_SEASON_ITEM(
      string key,
      string arguments,
      ItemQueryContext context,
      bool avoidRepeat,
      HashSet<string> avoidItemIds,
      Action<string, string> logError)
    {
      GameLocation location = context.Location;
      Random random = context.Random ?? Utility.CreateDaySaveRandom((double) Game1.hash.GetDeterministicHashCode(key + arguments));
      return (IEnumerable<ItemQueryResult>) new ItemQueryResult[1]
      {
        new ItemQueryResult((ISalable) ItemRegistry.Create(Utility.getRandomItemFromSeason(location.GetSeason(), false, random)))
      };
    }

    /// <summary>Get random items for a given type, optionally within a numeric ID range. Format: <c>RANDOM_ITEMS &lt;item data definition ID&gt; [min numeric id] [max numeric id]</c>.</summary>
    /// <inheritdoc cref="T:StardewValley.Delegates.ResolveItemQueryDelegate" />
    public static IEnumerable<ItemQueryResult> RANDOM_ITEMS(
      string key,
      string arguments,
      ItemQueryContext context,
      bool avoidRepeat,
      HashSet<string> avoidItemIds,
      Action<string, string> logError)
    {
      int minId = int.MinValue;
      int maxId = int.MaxValue;
      bool isRandomSale = false;
      bool requirePrice = false;
      string[] array = ItemQueryResolver.Helpers.SplitArguments(arguments);
      string identifier;
      string error;
      if (!ArgUtility.TryGet(array, 0, out identifier, out error, false, "typeId"))
      {
        ItemQueryResolver.Helpers.ErrorResult(key, arguments, logError, error);
      }
      else
      {
        int num = 1;
        int result1;
        if (ArgUtility.HasIndex<string>(array, 1) && int.TryParse(array[1], out result1))
        {
          minId = result1;
          ++num;
          if (ArgUtility.HasIndex<string>(array, 2) && int.TryParse(array[2], out result1))
          {
            maxId = result1;
            ++num;
          }
        }
        for (int index = num; index < array.Length; ++index)
        {
          string str = array[index];
          if (str.EqualsIgnoreCase("@isRandomSale"))
            isRandomSale = true;
          else if (str.EqualsIgnoreCase("@requirePrice"))
          {
            requirePrice = true;
          }
          else
          {
            if (str.StartsWith('@'))
            {
              ItemQueryResolver.Helpers.ErrorResult(key, arguments, logError, $"index {index} has unknown flag argument '{str}'");
              yield break;
            }
            if (index == 1 || index == 2)
            {
              ItemQueryResolver.Helpers.ErrorResult(key, arguments, logError, $"index {index} must a numeric {(index == 1 ? "min" : "max")} ID, or an option flag starting with '@'.");
              yield break;
            }
            ItemQueryResolver.Helpers.ErrorResult(key, arguments, logError, $"index {index} must be an option flag starting with '@'.");
            yield break;
          }
        }
        IItemDataDefinition typeDefinition = ItemRegistry.GetTypeDefinition(identifier);
        if (typeDefinition == null)
        {
          ItemQueryResolver.Helpers.ErrorResult(key, arguments, logError, $"there's no item data definition with ID '{identifier}'");
        }
        else
        {
          bool hasRange = minId != int.MinValue || maxId != int.MaxValue;
          Random random = context.Random ?? Game1.random;
          foreach (ParsedItemData data in (IEnumerable<ParsedItemData>) typeDefinition.GetAllData().OrderBy<ParsedItemData, int>((Func<ParsedItemData, int>) (p => random.Next())))
          {
            int result2;
            if ((!isRandomSale || !ItemQueryResolver.Helpers.ExcludeFromRandomSale(data)) && (!hasRange || int.TryParse(data.ItemId, out result2) && result2 >= minId && result2 <= maxId))
            {
              Item obj = ItemRegistry.Create(data.QualifiedItemId);
              if (!requirePrice || obj.salePrice(true) > 0)
                yield return new ItemQueryResult((ISalable) obj);
            }
          }
        }
      }
    }

    /// <summary>Get a secret note (if the player unlocked them and hasn't found them all), else the given item query (if provided), else nothing. Format: <c>SECRET_NOTE_OR_ITEM [alternate item query]</c>.</summary>
    /// <inheritdoc cref="T:StardewValley.Delegates.ResolveItemQueryDelegate" />
    public static IEnumerable<ItemQueryResult> SECRET_NOTE_OR_ITEM(
      string key,
      string arguments,
      ItemQueryContext context,
      bool avoidRepeat,
      HashSet<string> avoidItemIds,
      Action<string, string> logError)
    {
      GameLocation location = context.Location;
      Farmer player = context.Player;
      if (location != null && location.HasUnlockedAreaSecretNotes(player))
      {
        StardewValley.Object unseenSecretNote = location.tryToCreateUnseenSecretNote(player);
        if (unseenSecretNote != null)
          return (IEnumerable<ItemQueryResult>) new ItemQueryResult[1]
          {
            new ItemQueryResult((ISalable) unseenSecretNote)
          };
      }
      return string.IsNullOrWhiteSpace(arguments) ? (IEnumerable<ItemQueryResult>) LegacyShims.EmptyArray<ItemQueryResult>() : (IEnumerable<ItemQueryResult>) ItemQueryResolver.TryResolve(arguments, new ItemQueryContext(context, "query 'SECRET_NOTE_OR_ITEM'"));
    }

    /// <summary>Get a special 'key to the town' shop item. This returns an <see cref="T:StardewValley.ISalable" /> instance which may be ignored or invalid outside shops. Format: <c>SHOP_TOWN_KEY</c> (no arguments).</summary>
    /// <inheritdoc cref="T:StardewValley.Delegates.ResolveItemQueryDelegate" />
    public static IEnumerable<ItemQueryResult> SHOP_TOWN_KEY(
      string key,
      string arguments,
      ItemQueryContext context,
      bool avoidRepeat,
      HashSet<string> avoidItemIds,
      Action<string, string> logError)
    {
      return (IEnumerable<ItemQueryResult>) new ItemQueryResult[1]
      {
        new ItemQueryResult((ISalable) new PurchaseableKeyItem(Game1.content.LoadString("Strings\\StringsFromCSFiles:KeyToTheTown"), Game1.content.LoadString("Strings\\StringsFromCSFiles:KeyToTheTown_desc"), 912, (Action<Farmer>) (farmer => farmer.HasTownKey = true)))
        {
          OverrideShopAvailableStock = new int?(1)
        }
      };
    }

    /// <summary>Get the tool upgrades listed in <c>Data/Shops</c> for the given tool ID (or all tool upgrades if <c>[tool ID]</c> is omitted). Format: <c>TOOL_UPGRADES [tool ID]</c>.</summary>
    /// <inheritdoc cref="T:StardewValley.Delegates.ResolveItemQueryDelegate" />
    public static IEnumerable<ItemQueryResult> TOOL_UPGRADES(
      string key,
      string arguments,
      ItemQueryContext context,
      bool avoidRepeat,
      HashSet<string> avoidItemIds,
      Action<string, string> logError)
    {
      string str = (string) null;
      if (!string.IsNullOrWhiteSpace(arguments))
      {
        ParsedItemData dataOrErrorItem = ItemRegistry.GetDataOrErrorItem(arguments);
        if (dataOrErrorItem.HasTypeId("(T)"))
          return (IEnumerable<ItemQueryResult>) ItemQueryResolver.Helpers.ErrorResult(key, arguments, logError, $"can't filter for ID '{arguments}' because that isn't a tool item ID");
        str = dataOrErrorItem.ItemId;
      }
      List<ItemQueryResult> itemQueryResultList = new List<ItemQueryResult>();
      foreach (KeyValuePair<string, ToolData> keyValuePair in (IEnumerable<KeyValuePair<string, ToolData>>) Game1.toolData)
      {
        string key1 = keyValuePair.Key;
        ToolData tool = keyValuePair.Value;
        if (str == null || !(key1 != str))
        {
          ToolUpgradeData toolUpgradeData = ShopBuilder.GetToolUpgradeData(tool, Game1.player);
          if (toolUpgradeData != null)
          {
            Item obj = ItemRegistry.Create("(T)" + key1);
            int num = toolUpgradeData.Price > -1 ? toolUpgradeData.Price : Math.Max(0, obj.salePrice(false));
            itemQueryResultList.Add(new ItemQueryResult((ISalable) obj)
            {
              OverrideBasePrice = new int?(num),
              OverrideShopAvailableStock = new int?(1),
              OverrideTradeItemId = toolUpgradeData.TradeItemId,
              OverrideTradeItemAmount = new int?(toolUpgradeData.TradeItemAmount)
            });
          }
        }
      }
      return (IEnumerable<ItemQueryResult>) itemQueryResultList;
    }

    public static IEnumerable<ItemQueryResult> PET_ADOPTION(
      string key,
      string arguments,
      ItemQueryContext context,
      bool avoidRepeat,
      HashSet<string> avoidItemIds,
      Action<string, string> logError)
    {
      List<ItemQueryResult> itemQueryResultList1 = new List<ItemQueryResult>();
      foreach (KeyValuePair<string, PetData> keyValuePair in (IEnumerable<KeyValuePair<string, PetData>>) Game1.petData)
      {
        foreach (PetBreed breed in keyValuePair.Value.Breeds)
        {
          if (breed.CanBeAdoptedFromMarnie)
          {
            List<ItemQueryResult> itemQueryResultList2 = itemQueryResultList1;
            PetLicense petLicense = new PetLicense();
            petLicense.Name = $"{keyValuePair.Key}|{breed.Id}";
            itemQueryResultList2.Add(new ItemQueryResult((ISalable) petLicense)
            {
              OverrideBasePrice = new int?(breed.AdoptionPrice)
            });
          }
        }
      }
      return (IEnumerable<ItemQueryResult>) itemQueryResultList1;
    }
  }
}
