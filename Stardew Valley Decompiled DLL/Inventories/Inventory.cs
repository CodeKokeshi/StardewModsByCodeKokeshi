// Decompiled with JetBrains decompiler
// Type: StardewValley.Inventories.Inventory
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Netcode;
using StardewValley.SaveSerialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley.Inventories;

/// <summary>A managed list of items.</summary>
[XmlRoot("items")]
public class Inventory : 
  INetObject<NetFields>,
  IXmlSerializable,
  IInventory,
  IList<Item>,
  ICollection<Item>,
  IEnumerable<Item>,
  IEnumerable
{
  /// <summary>The underlying list of items.</summary>
  private readonly NetObjectList<Item> Items = new NetObjectList<Item>();
  /// <summary>The <see cref="F:StardewValley.Inventories.Inventory.Items" /> indexed by their qualified item ID.</summary>
  private InventoryIndex ItemsById;
  /// <summary>The backing field for <see cref="M:StardewValley.Inventories.Inventory.CountItemStacks" />.</summary>
  private int? CachedItemStackCount;

  /// <inheritdoc />
  public NetFields NetFields { get; } = new NetFields(nameof (Inventory));

  /// <summary>The number of items in the inventory, including <c>null</c> slots.</summary>
  public int Count => this.Items.Count;

  /// <inheritdoc />
  public bool IsReadOnly => this.Items.IsReadOnly;

  /// <inheritdoc />
  public Item this[int index]
  {
    get => this.Items[index];
    set => this.Items[index] = value;
  }

  /// <inheritdoc />
  public bool IsLocalPlayerInventory { get; set; }

  /// <summary>An event raised when an item stack is added or removed.</summary>
  public event OnSlotChangedDelegate OnSlotChanged;

  /// <summary>An event raised when the inventory is cleared or replaced.</summary>
  public event OnInventoryReplacedDelegate OnInventoryReplaced;

  /// <inheritdoc />
  public long LastTickSlotChanged { get; private set; }

  /// <summary>Construct an instance.</summary>
  public Inventory()
  {
    this.NetFields.SetOwner((INetObject<NetFields>) this).AddField((INetSerializable) this.Items, "this.Items");
    this.Items.OnElementChanged += new NetList<Item, NetRef<Item>>.ElementChangedEvent(this.HandleElementChanged);
    this.Items.OnArrayReplaced += new NetList<Item, NetRef<Item>>.ArrayReplacedEvent(this.HandleArrayReplaced);
  }

  /// <inheritdoc />
  public bool HasAny() => this.GetItemsById().CountKeys() > 0;

  /// <inheritdoc />
  public bool HasEmptySlots() => this.Count > this.CountItemStacks();

  /// <inheritdoc />
  public int CountItemStacks()
  {
    return this.CachedItemStackCount ?? (this.CachedItemStackCount = new int?(this.GetItemsById().CountItems())).Value;
  }

  /// <inheritdoc />
  public void OverwriteWith(IList<Item> list)
  {
    if (this == list || this.Items == list)
      return;
    this.ClearIndex();
    this.Items.CopyFrom(list);
  }

  /// <inheritdoc />
  public IList<Item> GetRange(int index, int count)
  {
    return (IList<Item>) this.Items.GetRange(index, count);
  }

  /// <inheritdoc />
  public void AddRange(ICollection<Item> collection)
  {
    this.Items.AddRange((IEnumerable<Item>) collection);
  }

  /// <inheritdoc />
  public void RemoveRange(int index, int count) => this.Items.RemoveRange(index, count);

  /// <inheritdoc />
  public void RemoveEmptySlots()
  {
    if (!this.HasEmptySlots())
      return;
    for (int index = this.Count - 1; index >= 0; --index)
    {
      if (this[index] == null)
        this.RemoveAt(index);
    }
  }

  /// <inheritdoc />
  public bool ContainsId(string itemId)
  {
    itemId = ItemRegistry.QualifyItemId(itemId);
    return itemId != null && this.GetItemsById().Contains(itemId);
  }

  /// <inheritdoc />
  public bool ContainsId(string itemId, int minimum)
  {
    itemId = ItemRegistry.QualifyItemId(itemId);
    IReadOnlyList<Item> items;
    if (itemId == null || !this.GetItemsById().TryGet(itemId, out items))
      return false;
    if (minimum <= 1)
      return true;
    int num = 0;
    foreach (Item obj in (IEnumerable<Item>) items)
    {
      if (obj.QualifiedItemId == itemId)
        num += obj.Stack;
      if (num >= minimum)
        return true;
    }
    return false;
  }

  /// <inheritdoc />
  public int CountId(string itemId)
  {
    itemId = ItemRegistry.QualifyItemId(itemId);
    IReadOnlyList<Item> items;
    if (itemId == null || !this.GetItemsById().TryGet(itemId, out items))
      return 0;
    int num = 0;
    foreach (Item obj in (IEnumerable<Item>) items)
    {
      if (obj.QualifiedItemId == itemId)
        num += obj.Stack;
    }
    return num;
  }

  /// <inheritdoc />
  public IEnumerable<Item> GetById(string itemId)
  {
    itemId = ItemRegistry.QualifyItemId(itemId);
    IReadOnlyList<Item> items;
    return itemId == null || !this.GetItemsById().TryGet(itemId, out items) ? (IEnumerable<Item>) LegacyShims.EmptyArray<Item>() : (IEnumerable<Item>) items;
  }

  /// <inheritdoc />
  public int Reduce(Item item, int count, bool reduceRemainderFromInventory = false)
  {
    int index = -1;
    if (this.IsLocalPlayerInventory)
    {
      index = Game1.player.CurrentToolIndex;
      if (index < 0 || index >= this.Count || this.Items[index] != item)
        index = -1;
    }
    if (index < 0)
      index = this.IndexOf(item);
    int count1 = count;
    if (index > -1)
    {
      count1 -= item.Stack;
      this.Items[index] = item.ConsumeStack(count);
    }
    else
      Game1.log.Warn($"Can't deduct item with ID '{item.QualifiedItemId}' from {(this.IsLocalPlayerInventory ? "the player's" : "this")} inventory because it's not in that inventory.");
    if (reduceRemainderFromInventory && count1 > 0)
      count1 -= this.ReduceId(item.QualifiedItemId, count1);
    return count1 > 0 ? count - count1 : count;
  }

  /// <inheritdoc />
  public int ReduceId(string itemId, int count)
  {
    itemId = ItemRegistry.QualifyItemId(itemId);
    if (itemId == null)
      return 0;
    InventoryIndex itemsById = this.GetItemsById();
    IList<Item> items;
    if (!itemsById.TryGetMutable(itemId, out items))
      return 0;
    bool flag = false;
    int val1 = count;
    for (int index = 0; index < items.Count && val1 > 0; ++index)
    {
      Item obj = items[index];
      int amount = Math.Min(val1, obj.Stack);
      items[index] = obj.ConsumeStack(amount);
      if (items[index] == null)
      {
        flag = true;
        items.RemoveAt(index);
        obj.SetTempData<string>("__Inventory_ReduceId_Remove", "");
        --index;
      }
      val1 -= amount;
    }
    if (items.Count == 0)
      itemsById.RemoveKey(itemId);
    if (flag)
    {
      for (int index = this.Items.Count - 1; index >= 0; --index)
      {
        bool? nullable = this.Items[index]?.tempData?.Remove("__Inventory_ReduceId_Remove");
        if (nullable.HasValue && nullable.GetValueOrDefault())
          this.Items[index] = (Item) null;
      }
    }
    return count - val1;
  }

  /// <inheritdoc />
  public bool RemoveButKeepEmptySlot(Item item)
  {
    if (item == null)
      return false;
    int index = this.Items.IndexOf(item);
    if (index == -1)
      return false;
    this.Items[index] = (Item) null;
    return true;
  }

  /// <inheritdoc />
  public IEnumerator<Item> GetEnumerator() => (IEnumerator<Item>) this.Items.GetEnumerator();

  /// <inheritdoc />
  IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.Items.GetEnumerator();

  /// <inheritdoc />
  public void Add(Item item) => this.Items.Add(item);

  /// <inheritdoc />
  public void Clear()
  {
    this.ClearIndex();
    this.Items.Clear();
  }

  /// <inheritdoc />
  public bool Contains(Item item)
  {
    IList<Item> items;
    return item != null && this.GetItemsById().TryGetMutable(item.QualifiedItemId, out items) && items.Contains(item);
  }

  /// <inheritdoc />
  public void CopyTo(Item[] array, int arrayIndex) => this.Items.CopyTo(array, arrayIndex);

  /// <inheritdoc />
  public bool Remove(Item item) => item != null && this.Items.Remove(item);

  /// <inheritdoc />
  public int IndexOf(Item item) => this.Items.IndexOf(item);

  /// <inheritdoc />
  public void Insert(int index, Item item) => this.Items.Insert(index, item);

  /// <inheritdoc />
  public void RemoveAt(int index) => this.Items.RemoveAt(index);

  /// <inheritdoc />
  public XmlSchema GetSchema() => (XmlSchema) null;

  /// <inheritdoc />
  public void ReadXml(XmlReader reader)
  {
    int num = reader.IsEmptyElement ? 1 : 0;
    reader.Read();
    if (num != 0)
      return;
    while (reader.NodeType != XmlNodeType.EndElement)
    {
      this.Items.Add(SaveSerializer.Deserialize<Item>(reader));
      int content = (int) reader.MoveToContent();
    }
    reader.ReadEndElement();
  }

  /// <inheritdoc />
  public void WriteXml(XmlWriter writer)
  {
    foreach (Item obj in (NetList<Item, NetRef<Item>>) this.Items)
      SaveSerializer.Serialize<Item>(writer, obj);
  }

  /// <summary>Get an index of items by ID.</summary>
  private InventoryIndex GetItemsById()
  {
    return this.ItemsById ?? (this.ItemsById = InventoryIndex.ById((IList<Item>) this.Items));
  }

  /// <summary>Handle the <see cref="F:StardewValley.Inventories.Inventory.Items" /> data getting replaced.</summary>
  /// <param name="list">The item list.</param>
  /// <param name="before">The previous item list.</param>
  /// <param name="after">The new item list.</param>
  private void HandleArrayReplaced(
    NetList<Item, NetRef<Item>> list,
    IList<Item> before,
    IList<Item> after)
  {
    if (before.Count == 0 && after.Count == 0)
      return;
    this.ClearIndex();
    this.CachedItemStackCount = new int?();
    this.LastTickSlotChanged = DateTime.UtcNow.Ticks;
    OnInventoryReplacedDelegate inventoryReplaced = this.OnInventoryReplaced;
    if (inventoryReplaced == null)
      return;
    inventoryReplaced(this, before, after);
  }

  /// <summary>Handle a slot in the <see cref="F:StardewValley.Inventories.Inventory.Items" /> data changing.</summary>
  /// <param name="list">The item list.</param>
  /// <param name="index">The item slot's index within the inventory.</param>
  /// <param name="before">The previous item value (which may be <c>null</c> when adding a stack).</param>
  /// <param name="after">The new item value (which may be <c>null</c> when removing a stack).</param>
  private void HandleElementChanged(
    NetList<Item, NetRef<Item>> list,
    int index,
    Item before,
    Item after)
  {
    if (before == after)
      return;
    this.ItemsById?.Remove(before);
    this.ItemsById?.Add(after);
    this.CachedItemStackCount = new int?();
    this.LastTickSlotChanged = DateTime.UtcNow.Ticks;
    OnSlotChangedDelegate onSlotChanged = this.OnSlotChanged;
    if (onSlotChanged == null)
      return;
    onSlotChanged(this, index, before, after);
  }

  /// <summary>Clear the item index, so it'll be rebuilt next time it's needed.</summary>
  private void ClearIndex() => this.ItemsById = (InventoryIndex) null;
}
