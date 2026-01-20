// Decompiled with JetBrains decompiler
// Type: StardewValley.Objects.StorageFurniture
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Netcode;
using StardewValley.Delegates;
using StardewValley.GameData.Shops;
using StardewValley.Internal;
using StardewValley.Menus;
using StardewValley.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley.Objects;

[XmlInclude(typeof (FishTankFurniture))]
public class StorageFurniture : Furniture
{
  [XmlElement("heldItems")]
  public readonly NetObjectList<Item> heldItems = new NetObjectList<Item>();
  [XmlIgnore]
  public readonly NetMutex mutex = new NetMutex();

  public StorageFurniture()
  {
  }

  public StorageFurniture(string itemId, Vector2 tile, int initialRotations)
    : base(itemId, tile, initialRotations)
  {
  }

  public StorageFurniture(string itemId, Vector2 tile)
    : base(itemId, tile)
  {
  }

  /// <inheritdoc />
  protected override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.heldItems, "heldItems").AddField((INetSerializable) this.mutex.NetFields, "mutex.NetFields");
  }

  public override bool canBeRemoved(Farmer who) => !this.mutex.IsLocked() && base.canBeRemoved(who);

  /// <inheritdoc />
  public override bool checkForAction(Farmer who, bool justCheckingForActivity = false)
  {
    if (justCheckingForActivity)
      return true;
    this.mutex.RequestLock(new Action(this.ShowMenu));
    return true;
  }

  public virtual void ShowMenu() => this.ShowShopMenu();

  public virtual void ShowChestMenu()
  {
    ItemGrabMenu itemGrabMenu = new ItemGrabMenu((IList<Item>) this.heldItems, false, true, new InventoryMenu.highlightThisItem(InventoryMenu.highlightAllItems), new ItemGrabMenu.behaviorOnItemSelect(this.GrabItemFromInventory), (string) null, new ItemGrabMenu.behaviorOnItemSelect(this.GrabItemFromChest), canBeExitedWithKey: true, showOrganizeButton: true, source: 1, sourceItem: (Item) this, context: (object) this);
    itemGrabMenu.behaviorBeforeCleanup = (Action<IClickableMenu>) (menu =>
    {
      this.mutex.ReleaseLock();
      this.OnMenuClose();
    });
    Game1.activeClickableMenu = (IClickableMenu) itemGrabMenu;
    Game1.playSound("dwop");
  }

  public virtual void GrabItemFromInventory(Item item, Farmer who)
  {
    if (item.Stack == 0)
      item.Stack = 1;
    Item obj = this.AddItem(item);
    if (obj == null)
      who.removeItemFromInventory(item);
    else
      obj = who.addItemToInventory(obj);
    this.ClearNulls();
    int id = Game1.activeClickableMenu.currentlySnappedComponent != null ? Game1.activeClickableMenu.currentlySnappedComponent.myID : -1;
    this.ShowChestMenu();
    (Game1.activeClickableMenu as ItemGrabMenu).heldItem = obj;
    if (id == -1)
      return;
    Game1.activeClickableMenu.currentlySnappedComponent = Game1.activeClickableMenu.getComponentWithID(id);
    Game1.activeClickableMenu.snapCursorToCurrentSnappedComponent();
  }

  public virtual bool HighlightItems(Item item) => InventoryMenu.highlightAllItems(item);

  public virtual void GrabItemFromChest(Item item, Farmer who)
  {
    if (!who.couldInventoryAcceptThisItem(item))
      return;
    this.heldItems.Remove(item);
    this.ClearNulls();
    this.ShowChestMenu();
  }

  public virtual void ClearNulls()
  {
    this.heldItems.RemoveWhere((Func<Item, bool>) (slot => slot == null));
  }

  public virtual Item AddItem(Item item)
  {
    item.resetState();
    this.ClearNulls();
    for (int index = 0; index < this.heldItems.Count; ++index)
    {
      if (this.heldItems[index] != null && this.heldItems[index].canStackWith((ISalable) item))
      {
        int amount = item.Stack - this.heldItems[index].addToStack(item);
        if (item.ConsumeStack(amount) == null)
          return (Item) null;
      }
    }
    if (this.heldItems.Count >= 36)
      return item;
    this.heldItems.Add(item);
    return (Item) null;
  }

  public virtual void ShowShopMenu()
  {
    List<Item> list = this.heldItems.ToList<Item>();
    list.Sort(new Comparison<Item>(this.SortItems));
    Dictionary<ISalable, ItemStockInformation> itemPriceAndStock = new Dictionary<ISalable, ItemStockInformation>();
    foreach (Item key in list)
      itemPriceAndStock[(ISalable) key] = new ItemStockInformation(0, 1, stockMode: LimitedStockMode.None);
    ShopMenu shopMenu = new ShopMenu(this.GetShopMenuContext(), itemPriceAndStock, on_purchase: new ShopMenu.OnPurchaseDelegate(this.onDresserItemWithdrawn), on_sell: new Func<ISalable, bool>(this.onDresserItemDeposited));
    shopMenu.source = (object) this;
    shopMenu.behaviorBeforeCleanup = (Action<IClickableMenu>) (menu =>
    {
      this.mutex.ReleaseLock();
      this.OnMenuClose();
    });
    Game1.activeClickableMenu = (IClickableMenu) shopMenu;
  }

  public virtual void OnMenuClose()
  {
  }

  public virtual string GetShopMenuContext() => "Dresser";

  /// <inheritdoc />
  public override bool canBeTrashed() => this.heldItems.Count <= 0 && base.canBeTrashed();

  public override void DayUpdate()
  {
    base.DayUpdate();
    this.mutex.ReleaseLock();
  }

  /// <inheritdoc />
  protected override Item GetOneNew()
  {
    return (Item) new StorageFurniture(this.ItemId, this.tileLocation.Value);
  }

  public virtual int SortItems(Item a, Item b)
  {
    if (a.Category != b.Category)
      return a.Category.CompareTo(b.Category);
    return a is Clothing clothing1 && b is Clothing clothing2 && clothing1.clothesType.Value != clothing2.clothesType.Value ? clothing1.clothesType.Value.CompareTo((object) clothing2.clothesType.Value) : a.ParentSheetIndex.CompareTo(b.ParentSheetIndex);
  }

  /// <summary>Handle an item being taken from the storage furniture.</summary>
  /// <inheritdoc cref="T:StardewValley.Menus.ShopMenu.OnPurchaseDelegate" />
  public virtual bool onDresserItemWithdrawn(
    ISalable salable,
    Farmer who,
    int countTaken,
    ItemStockInformation stock)
  {
    if (salable is Item obj)
      this.heldItems.Remove(obj);
    return false;
  }

  public override void updateWhenCurrentLocation(GameTime time)
  {
    GameLocation location = this.Location;
    if (location != null)
      this.mutex.Update(location);
    base.updateWhenCurrentLocation(time);
  }

  public virtual bool onDresserItemDeposited(ISalable deposited_salable)
  {
    if (deposited_salable is Item obj)
    {
      this.heldItems.Add(obj);
      if (Game1.activeClickableMenu is ShopMenu)
      {
        Dictionary<ISalable, ItemStockInformation> new_stock = new Dictionary<ISalable, ItemStockInformation>();
        List<Item> list = this.heldItems.ToList<Item>();
        list.Sort(new Comparison<Item>(this.SortItems));
        foreach (Item key in list)
          new_stock[(ISalable) key] = new ItemStockInformation(0, 1, stockMode: LimitedStockMode.None);
        (Game1.activeClickableMenu as ShopMenu).setItemPriceAndStock(new_stock);
        Game1.playSound("dwop");
        return true;
      }
    }
    return false;
  }

  /// <inheritdoc />
  public override bool ForEachItem(ForEachItemDelegate handler, GetForEachItemPathDelegate getPath)
  {
    return base.ForEachItem(handler, getPath) && ForEachItemHelper.ApplyToList<Item>((IList<Item>) this.heldItems, handler, getPath);
  }
}
