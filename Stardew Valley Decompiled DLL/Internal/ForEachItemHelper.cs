// Decompiled with JetBrains decompiler
// Type: StardewValley.Internal.ForEachItemHelper
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Netcode;
using StardewValley.Buildings;
using StardewValley.Characters;
using StardewValley.Delegates;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley.SpecialOrders;
using System;
using System.Collections.Generic;

#nullable disable
namespace StardewValley.Internal;

/// <summary>Iterates through every item in the game state and optionally edits, replaces, or removes instances.</summary>
/// <remarks>This is a low-level class. Most code should use a utility method like <see cref="M:StardewValley.Utility.ForEachItem(System.Func{StardewValley.Item,System.Boolean})" /> or <see cref="M:StardewValley.Utility.ForEachItemContext(StardewValley.Delegates.ForEachItemDelegate)" /> instead.</remarks>
public static class ForEachItemHelper
{
  /// <summary>Perform an action for each item in the game world, including items within items (e.g. in a chest or on a table), hats placed on children, items in player inventories, etc.</summary>
  /// <param name="handler">The action to perform for each item.</param>
  /// <returns>Returns whether to continue iterating if needed (i.e. returns false if the last <paramref name="handler" /> call did).</returns>
  public static bool ForEachItemInWorld(ForEachItemDelegate handler)
  {
    bool canContinue = true;
    Utility.ForEachLocation((Func<GameLocation, bool>) (location => canContinue = ForEachItemHelper.ForEachItemInLocation(location, handler)));
    if (!canContinue)
      return false;
    foreach (Farmer allFarmer in Game1.getAllFarmers())
    {
      Farmer farmer = allFarmer;
      int toolIndex = farmer.CurrentToolIndex;
      if ((!ForEachItemHelper.ApplyToList<Item>((IList<Item>) farmer.Items, handler, new GetForEachItemPathDelegate(GetParentPath), true, new Action<Item, Item, int>(OnChangedItemSlot)) || !ForEachItemHelper.ApplyToField<Clothing>(farmer.shirtItem, handler, new GetForEachItemPathDelegate(GetParentPath), new Action<Item, Item>(OnChangedEquipment)) || !ForEachItemHelper.ApplyToField<Clothing>(farmer.pantsItem, handler, new GetForEachItemPathDelegate(GetParentPath), new Action<Item, Item>(OnChangedEquipment)) || !ForEachItemHelper.ApplyToField<Boots>(farmer.boots, handler, new GetForEachItemPathDelegate(GetParentPath), new Action<Item, Item>(OnChangedEquipment)) || !ForEachItemHelper.ApplyToField<Hat>(farmer.hat, handler, new GetForEachItemPathDelegate(GetParentPath), new Action<Item, Item>(OnChangedEquipment)) || !ForEachItemHelper.ApplyToField<Ring>(farmer.leftRing, handler, new GetForEachItemPathDelegate(GetParentPath), new Action<Item, Item>(OnChangedEquipment)) || !ForEachItemHelper.ApplyToField<Ring>(farmer.rightRing, handler, new GetForEachItemPathDelegate(GetParentPath), new Action<Item, Item>(OnChangedEquipment)) || !ForEachItemHelper.ApplyToItem<Item>(farmer.recoveredItem, handler, (Action) (() => farmer.recoveredItem = (Item) null), (Action<Item>) (newItem => farmer.recoveredItem = ForEachItemHelper.PrepareForReplaceWith<Item>(farmer.recoveredItem, newItem)), new GetForEachItemPathDelegate(GetParentPath)) || !ForEachItemHelper.ApplyToField<Tool>(farmer.toolBeingUpgraded, handler, new GetForEachItemPathDelegate(GetParentPath)) ? 0 : (ForEachItemHelper.ApplyToList<Item>((IList<Item>) farmer.itemsLostLastDeath, handler, new GetForEachItemPathDelegate(GetParentPath)) ? 1 : 0)) == 0)
        return false;

      void OnChangedEquipment(Item oldItem, Item newItem)
      {
        oldItem?.onUnequip(farmer);
        newItem?.onEquip(farmer);
      }

      IList<object> GetParentPath()
      {
        return (IList<object>) new List<object>()
        {
          (object) farmer
        };
      }

      void OnChangedItemSlot(Item oldItem, Item newItem, int index)
      {
        if (index != toolIndex)
          return;
        if (oldItem is Tool tool1)
          tool1.onUnequip(farmer);
        if (!(newItem is Tool tool2))
          return;
        tool2.onEquip(farmer);
      }
    }
    if (!ForEachItemHelper.ApplyToList<Item>((IList<Item>) Game1.player.team.returnedDonations, handler, new GetForEachItemPathDelegate(GetParentPathForTeam)))
      return false;
    foreach (IList<Item> list in Game1.player.team.globalInventories.Values)
    {
      if (!ForEachItemHelper.ApplyToList<Item>(list, handler, new GetForEachItemPathDelegate(GetParentPathForTeam)))
        return false;
    }
    foreach (SpecialOrder specialOrder in Game1.player.team.specialOrders)
    {
      SpecialOrder order = specialOrder;
      if (!ForEachItemHelper.ApplyToList<Item>((IList<Item>) order.donatedItems, handler, (GetForEachItemPathDelegate) (() => ForEachItemHelper.CombinePath(new GetForEachItemPathDelegate(GetParentPathForTeam), (object) Game1.player.team.specialOrders, (object) order))))
        return false;
    }
    return true;

    static IList<object> GetParentPathForTeam()
    {
      return (IList<object>) new List<object>()
      {
        (object) Game1.player.team
      };
    }
  }

  /// <summary>Perform an action for each item within a location, including items within items (e.g. in a chest or on a table), hats placed on children, items in player inventories, etc.</summary>
  /// <param name="location">The location whose items to iterate.</param>
  /// <param name="handler">The action to perform for each item.</param>
  /// <returns>Returns whether to continue iterating if needed (i.e. returns false if the last <paramref name="handler" /> call did).</returns>
  public static bool ForEachItemInLocation(GameLocation location, ForEachItemDelegate handler)
  {
    if (location == null)
      return true;
    if (!ForEachItemHelper.ApplyToList<Furniture>((IList<Furniture>) location.furniture, handler, new GetForEachItemPathDelegate(GetLocationPath)))
      return false;
    foreach (NPC character1 in location.characters)
    {
      NPC character = character1;
      if (!(character is Child child))
      {
        if (!(character is Horse horse))
        {
          if (character is Pet pet && !ForEachItemHelper.ApplyToField<Hat>(pet.hat, handler, new GetForEachItemPathDelegate(GetNpcPath)))
            return false;
        }
        else if (!ForEachItemHelper.ApplyToField<Hat>(horse.hat, handler, new GetForEachItemPathDelegate(GetNpcPath)))
          return false;
      }
      else if (!ForEachItemHelper.ApplyToField<Hat>(child.hat, handler, new GetForEachItemPathDelegate(GetNpcPath)))
        return false;
      // ISSUE: variable of a compiler-generated type
      ForEachItemHelper.\u003C\u003Ec__DisplayClass1_0 cDisplayClass10;

      IList<object> GetNpcPath()
      {
        // ISSUE: method pointer
        return ForEachItemHelper.CombinePath(new GetForEachItemPathDelegate((object) cDisplayClass10, __methodptr(\u003CForEachItemInLocation\u003Eg__GetLocationPath\u007C0)), (object) character);
      }
    }
    foreach (Building building in location.buildings)
    {
      if (!building.ForEachItemContextExcludingInterior(handler, new GetForEachItemPathDelegate(GetLocationPath)))
        return false;
    }
    bool? nullable = location.GetFridge(false)?.ForEachItem(handler, new GetForEachItemPathDelegate(GetLocationPath));
    if (nullable.HasValue && !nullable.GetValueOrDefault())
      return false;
    if (location.objects.Length > 0)
    {
      foreach (Vector2 key in location.objects.Keys)
      {
        Vector2 tile = key;
        StardewValley.Object obj = location.objects[tile];
        if (!ForEachItemHelper.ApplyToItem<StardewValley.Object>(obj, handler, (Action) (() => location.objects.Remove(tile)), (Action<Item>) (newItem => location.objects[tile] = ForEachItemHelper.PrepareForReplaceWith<StardewValley.Object>(obj, (StardewValley.Object) newItem)), (GetForEachItemPathDelegate) (() => ForEachItemHelper.CombinePath(new GetForEachItemPathDelegate(GetLocationPath), (object) location.objects))))
          return false;
      }
    }
    for (int i = location.debris.Count - 1; i >= 0; i--)
    {
      Debris d = location.debris[i];
      if (d.item != null && !ForEachItemHelper.ApplyToItem<Item>(d.item, handler, new Action(Remove), new Action<Item>(ReplaceWith), (GetForEachItemPathDelegate) (() => ForEachItemHelper.CombinePath(new GetForEachItemPathDelegate(GetLocationPath), (object) location.debris))))
        return false;

      void Remove()
      {
        if (d.itemId.Value == null || ItemRegistry.HasItemId(d.item, d.itemId.Value))
          location.debris.RemoveAt(i);
        else
          d.item = (Item) null;
      }

      void ReplaceWith(Item newItem)
      {
        if (ItemRegistry.HasItemId(newItem, d.itemId.Value))
          d.itemId.Value = newItem.QualifiedItemId;
        d.item = ForEachItemHelper.PrepareForReplaceWith<Item>(d.item, newItem);
      }
    }
    ShopLocation shopLocation = location as ShopLocation;
    return shopLocation == null || ForEachItemHelper.ApplyToList<Item>((IList<Item>) shopLocation.itemsFromPlayerToSell, handler, (GetForEachItemPathDelegate) (() => ForEachItemHelper.CombinePath(new GetForEachItemPathDelegate(GetLocationPath), (object) shopLocation.itemsFromPlayerToSell))) && ForEachItemHelper.ApplyToList<Item>((IList<Item>) shopLocation.itemsToStartSellingTomorrow, handler, (GetForEachItemPathDelegate) (() => ForEachItemHelper.CombinePath(new GetForEachItemPathDelegate(GetLocationPath), (object) shopLocation.itemsToStartSellingTomorrow)));

    IList<object> GetLocationPath()
    {
      return (IList<object>) new List<object>()
      {
        (object) location
      };
    }
  }

  /// <summary>Apply a for-each-item callback to an item.</summary>
  /// <typeparam name="TItem">The item type.</typeparam>
  /// <param name="item">The item instance to iterate.</param>
  /// <param name="handler">The action to perform for each item.</param>
  /// <param name="remove">Delete this item instance.</param>
  /// <param name="replaceWith">Replace this item with a new instance.</param>
  /// <param name="getParentPath">Get the contextual path leading to this item (excluding the item itself).</param>
  /// <returns>Returns whether to continue iterating if needed.</returns>
  public static bool ApplyToItem<TItem>(
    TItem item,
    ForEachItemDelegate handler,
    Action remove,
    Action<Item> replaceWith,
    GetForEachItemPathDelegate getParentPath)
    where TItem : Item
  {
    if ((object) item == null)
      return true;
    if (!handler(new ForEachItemContext((Item) item, new Action(Remove), new Action<Item>(ReplaceWith), getParentPath)))
      return false;
    // ISSUE: variable of a boxed type
    __Boxed<TItem> local = (object) item;
    return local == null || local.ForEachItem(handler, (GetForEachItemPathDelegate) (() => ForEachItemHelper.CombinePath(getParentPath, (object) (TItem) item)));

    void Remove()
    {
      remove();
      item = default (TItem);
    }

    void ReplaceWith(Item newItem)
    {
      if (newItem == null)
      {
        Remove();
      }
      else
      {
        item = ForEachItemHelper.PrepareForReplaceWith<TItem>(item, (TItem) newItem);
        replaceWith((Item) item);
      }
    }
  }

  /// <summary>Apply a for-each-item callback to an item.</summary>
  /// <typeparam name="TItem">The item type.</typeparam>
  /// <param name="field">The field instance to iterate.</param>
  /// <param name="handler">The action to perform for each item.</param>
  /// <param name="getParentPath">Get the contextual path leading to this field, excluding the field itself.</param>
  /// <param name="onChanged">A callback to invoke when the assigned value changes, which receives the old and new items.</param>
  /// <returns>Returns whether to continue iterating if needed.</returns>
  public static bool ApplyToField<TItem>(
    NetRef<TItem> field,
    ForEachItemDelegate handler,
    GetForEachItemPathDelegate getParentPath,
    Action<Item, Item> onChanged = null)
    where TItem : Item
  {
    Item oldValue = (Item) field.Value;
    return ForEachItemHelper.ApplyToItem<TItem>(field.Value, handler, new Action(Remove), new Action<Item>(ReplaceWith), new GetForEachItemPathDelegate(GetPath));

    void Remove()
    {
      ((NetFieldBase<TItem, NetRef<TItem>>) field).Value = default (TItem);
      Action<Item, Item> action = onChanged;
      if (action == null)
        return;
      action(oldValue, (Item) null);
    }

    void ReplaceWith(Item newItem)
    {
      ((NetFieldBase<TItem, NetRef<TItem>>) field).Value = ForEachItemHelper.PrepareForReplaceWith<TItem>(((NetFieldBase<TItem, NetRef<TItem>>) field).Value, (TItem) newItem);
      Action<Item, Item> action = onChanged;
      if (action == null)
        return;
      action(oldValue, newItem);
    }

    IList<object> GetPath() => ForEachItemHelper.CombinePath(getParentPath, (object) field);
  }

  /// <summary>Apply a for-each-item callback to an item.</summary>
  /// <typeparam name="TItem">The item type.</typeparam>
  /// <param name="list">The list of items to iterate.</param>
  /// <param name="handler">The action to perform for each item.</param>
  /// <param name="getParentPath">Get the contextual path leading to this list, excluding the list itself.</param>
  /// <param name="leaveNullSlotsOnRemoval">Whether to leave a null entry in the list when an item is removed. If <c>false</c>, the index is removed from the list instead.</param>
  /// <param name="onChanged">A callback to invoke when the assigned value changes, which receives the old and new items.</param>
  /// <returns>Returns whether to continue iterating if needed.</returns>
  public static bool ApplyToList<TItem>(
    IList<TItem> list,
    ForEachItemDelegate handler,
    GetForEachItemPathDelegate getParentPath,
    bool leaveNullSlotsOnRemoval = false,
    Action<Item, Item, int> onChanged = null)
    where TItem : Item
  {
    for (int i = list.Count - 1; i >= 0; i--)
    {
      Item oldValue = (Item) list[i];
      if (!ForEachItemHelper.ApplyToItem<TItem>(list[i], handler, new Action(Remove), new Action<Item>(ReplaceWith), new GetForEachItemPathDelegate(GetPath)))
        return false;

      void Remove()
      {
        if (leaveNullSlotsOnRemoval)
          list[i] = default (TItem);
        else
          list.RemoveAt(i);
        Action<Item, Item, int> action = onChanged;
        if (action == null)
          return;
        action(oldValue, (Item) null, i);
      }

      void ReplaceWith(Item newItem)
      {
        list[i] = ForEachItemHelper.PrepareForReplaceWith<TItem>(list[i], (TItem) newItem);
        Action<Item, Item, int> action = onChanged;
        if (action == null)
          return;
        action(oldValue, newItem, i);
      }
    }
    return true;

    IList<object> GetPath() => ForEachItemHelper.CombinePath(getParentPath, (object) list);
  }

  /// <summary>Combine the result of a <see cref="T:StardewValley.Delegates.GetForEachItemPathDelegate" /> parent path with child paths into a single path.</summary>
  /// <param name="parentPath">The parent path, or <c>null</c> to start the root at the first <paramref name="pathValues" /> value.</param>
  /// <param name="pathValues">The path segments to append.</param>
  /// <returns></returns>
  public static IList<object> CombinePath(
    GetForEachItemPathDelegate parentPath,
    params object[] pathValues)
  {
    IList<object> objectList = (parentPath != null ? parentPath() : (IList<object>) null) ?? (IList<object>) new List<object>();
    foreach (object pathValue in pathValues)
      objectList.Add(pathValue);
    return objectList;
  }

  /// <summary>Prepare a new item instance as a replacement for an existing item.</summary>
  /// <param name="previousItem">The existing item that's being replaced.</param>
  /// <param name="newItem">The new item that will replace <paramref name="previousItem" />.</param>
  /// <returns>Returns the <paramref name="newItem" /> for convenience.</returns>
  private static TItem PrepareForReplaceWith<TItem>(TItem previousItem, TItem newItem) where TItem : Item
  {
    StardewValley.Object object1 = (object) previousItem as StardewValley.Object;
    StardewValley.Object object2 = (object) newItem as StardewValley.Object;
    if (object1 != null && object2 != null)
      object2.TileLocation = object1.TileLocation;
    return newItem;
  }
}
