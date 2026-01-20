// Decompiled with JetBrains decompiler
// Type: StardewValley.Inventories.InventoryIndex
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using System;
using System.Collections.Generic;

#nullable disable
namespace StardewValley.Inventories;

/// <summary>Manages a lookup of items in an inventory by key.</summary>
public class InventoryIndex
{
  /// <summary>A cache of inventory items by key.</summary>
  private readonly Dictionary<string, List<Item>> Index = new Dictionary<string, List<Item>>();
  /// <summary>Adds an item to the index by key.</summary>
  private readonly Action<InventoryIndex, Item> AddImpl;
  /// <summary>Removes an item from the index by key.</summary>
  private readonly Action<InventoryIndex, Item> RemoveImpl;

  /// <summary>Construct an instance.</summary>
  /// <param name="addImpl">Adds an item to the index by key.</param>
  /// <param name="removeImpl">Removes an item from the index by key.</param>
  public InventoryIndex(
    Action<InventoryIndex, Item> addImpl,
    Action<InventoryIndex, Item> removeImpl)
  {
    this.AddImpl = addImpl;
    this.RemoveImpl = removeImpl;
  }

  /// <summary>Construct an index which caches items by their qualified ID.</summary>
  /// <param name="items">The items to index.</param>
  public static InventoryIndex ById(IList<Item> items)
  {
    InventoryIndex inventoryIndex = new InventoryIndex((Action<InventoryIndex, Item>) ((index, item) => index.AddWithKey(item.QualifiedItemId, item)), (Action<InventoryIndex, Item>) ((index, item) => index.RemoveItem(item.QualifiedItemId, item)));
    foreach (Item obj in (IEnumerable<Item>) items)
      inventoryIndex.Add(obj);
    return inventoryIndex;
  }

  /// <summary>The number of unique keys used to index items.</summary>
  public int CountKeys() => this.Index.Count;

  /// <summary>The number of items in the inventory.</summary>
  public int CountItems()
  {
    int num = 0;
    foreach (List<Item> objList in this.Index.Values)
      num += objList.Count;
    return num;
  }

  /// <summary>Get whether any items match a given key.</summary>
  /// <param name="key">The index key.</param>
  public bool Contains(string key) => key != null && this.Index.ContainsKey(key);

  /// <summary>Get a read-only list of items which match a given key, if any.</summary>
  /// <param name="key">The index key.</param>
  /// <param name="items">The matching items.</param>
  public bool TryGet(string key, out IReadOnlyList<Item> items)
  {
    List<Item> objList;
    if (key != null && this.Index.TryGetValue(key, out objList))
    {
      items = (IReadOnlyList<Item>) objList;
      return true;
    }
    items = (IReadOnlyList<Item>) null;
    return false;
  }

  /// <summary>Get an editable list of items which match a given key, if any.</summary>
  /// <param name="key">The index key.</param>
  /// <param name="items">The matching items.</param>
  /// <remarks>Most code should use <see cref="M:StardewValley.Inventories.InventoryIndex.TryGet(System.String,System.Collections.Generic.IReadOnlyList{StardewValley.Item}@)" /> instead. Changes to the list will only affect the index, not the underlying inventory. This method is only provided for cases where you're directly changing both at once. If you clear the list, make sure to call <see cref="M:StardewValley.Inventories.InventoryIndex.RemoveKey(System.String)" /> too.</remarks>
  public bool TryGetMutable(string key, out IList<Item> items)
  {
    List<Item> objList;
    if (key != null && this.Index.TryGetValue(key, out objList))
    {
      items = (IList<Item>) objList;
      return true;
    }
    items = (IList<Item>) null;
    return false;
  }

  /// <summary>Add an item to the index.</summary>
  /// <param name="item">The item to add.</param>
  public void Add(Item item)
  {
    if (item == null)
      return;
    this.AddImpl(this, item);
  }

  /// <summary>Add an item to the index.</summary>
  /// <param name="key">The key to index by.</param>
  /// <param name="item">The item to add.</param>
  /// <exception cref="T:System.ArgumentNullException">The <paramref name="key" /> is null.</exception>
  public void AddWithKey(string key, Item item)
  {
    if (key == null)
      throw new ArgumentNullException(nameof (key));
    if (item == null)
      return;
    List<Item> objList;
    if (!this.Index.TryGetValue(key, out objList))
      this.Index[key] = objList = new List<Item>();
    objList.Add(item);
  }

  /// <summary>Remove an item from the index.</summary>
  /// <param name="item">The item to remove.</param>
  public void Remove(Item item)
  {
    if (item == null)
      return;
    this.RemoveImpl(this, item);
  }

  /// <summary>Remove a key from the index.</summary>
  /// <param name="key">The key to remove.</param>
  /// <exception cref="T:System.ArgumentNullException">The <paramref name="key" /> is null.</exception>
  public void RemoveKey(string key)
  {
    if (key == null)
      throw new ArgumentNullException(nameof (key));
    this.Index.Remove(key);
  }

  /// <summary>Remove an item from the index.</summary>
  /// <param name="key">The key for which to remove the item.</param>
  /// <param name="item">The item to remove.</param>
  /// <exception cref="T:System.ArgumentNullException">The <paramref name="key" /> is null.</exception>
  public void RemoveItem(string key, Item item)
  {
    if (key == null)
      throw new ArgumentNullException(nameof (key));
    List<Item> objList;
    if (item == null || !this.Index.TryGetValue(key, out objList))
      return;
    objList.Remove(item);
    if (objList.Count != 0)
      return;
    this.Index.Remove(key);
  }
}
