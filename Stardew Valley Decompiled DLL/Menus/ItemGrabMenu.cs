// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.ItemGrabMenu
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley.Buildings;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace StardewValley.Menus;

public class ItemGrabMenu : MenuWithInventory
{
  public const int region_organizationButtons = 15923;
  public const int region_itemsToGrabMenuModifier = 53910;
  public const int region_fillStacksButton = 12952;
  public const int region_organizeButton = 106;
  public const int region_colorPickToggle = 27346;
  public const int region_specialButton = 12485;
  public const int region_lastShippedHolder = 12598;
  /// <summary>The <see cref="F:StardewValley.Menus.ItemGrabMenu.source" /> value when a specific value doesn't apply.</summary>
  public const int source_none = 0;
  /// <summary>The <see cref="F:StardewValley.Menus.ItemGrabMenu.source" /> value when collecting items from a chest.</summary>
  public const int source_chest = 1;
  /// <summary>The <see cref="F:StardewValley.Menus.ItemGrabMenu.source" /> value when collecting items which couldn't be added directly to the player's inventory (e.g. from NPC dialogue).</summary>
  public const int source_gift = 2;
  /// <summary>The <see cref="F:StardewValley.Menus.ItemGrabMenu.source" /> value when collecting treasure found while fishing.</summary>
  public const int source_fishingChest = 3;
  /// <summary>The <see cref="F:StardewValley.Menus.ItemGrabMenu.source" /> value when collecting items which couldn't be added directly to the player's inventory via <see cref="M:StardewValley.Farmer.addItemByMenuIfNecessary(StardewValley.Item,StardewValley.Menus.ItemGrabMenu.behaviorOnItemSelect,System.Boolean)" />.</summary>
  public const int source_overflow = 4;
  public const int specialButton_junimotoggle = 1;
  /// <summary>The inventory from which the player can collect items.</summary>
  public InventoryMenu ItemsToGrabMenu;
  public TemporaryAnimatedSprite poof;
  public bool reverseGrab;
  public bool showReceivingMenu = true;
  public bool drawBG = true;
  public bool destroyItemOnClick;
  public bool canExitOnKey;
  public bool playRightClickSound;
  public bool allowRightClick;
  public bool shippingBin;
  public string message;
  /// <summary>The callback invoked when taking something out of the player inventory (e.g. putting something in the Luau soup), if any.</summary>
  public ItemGrabMenu.behaviorOnItemSelect behaviorFunction;
  /// <summary>The callback invoked when taking something from the menu (e.g. to put in the player's inventory), if any.</summary>
  public ItemGrabMenu.behaviorOnItemSelect behaviorOnItemGrab;
  /// <summary>The item for which the item menu was opened (e.g. the chest or storage furniture item being checked), if applicable.</summary>
  public Item sourceItem;
  public ClickableTextureComponent fillStacksButton;
  public ClickableTextureComponent organizeButton;
  public ClickableTextureComponent colorPickerToggleButton;
  public ClickableTextureComponent specialButton;
  public ClickableTextureComponent lastShippedHolder;
  public List<ClickableComponent> discreteColorPickerCC;
  /// <summary>The reason this menu was opened, usually matching a constant like <see cref="F:StardewValley.Menus.ItemGrabMenu.source_chest" />.</summary>
  public int source;
  public int whichSpecialButton;
  /// <summary>A contextual value for what opened the menu. This may be a chest, event, fishing rod, location, etc.</summary>
  public object context;
  public bool snappedtoBottom;
  public DiscreteColorPicker chestColorPicker;
  public bool essential;
  public bool superEssential;
  public int storageSpaceTopBorderOffset;
  /// <summary>Whether <see cref="M:StardewValley.Menus.ItemGrabMenu.update(Microsoft.Xna.Framework.GameTime)" /> has run at least once yet.</summary>
  private bool HasUpdateTicked;
  public List<ItemGrabMenu.TransferredItemSprite> _transferredItemSprites = new List<ItemGrabMenu.TransferredItemSprite>();
  /// <summary>Whether the source item was placed in the current location when the menu is opened.</summary>
  public bool _sourceItemInCurrentLocation;
  public ClickableTextureComponent junimoNoteIcon;
  public int junimoNotePulser;

  /// <summary>Construct an instance.</summary>
  /// <param name="inventory">The items that can be collected by the player.</param>
  /// <param name="context">A contextual value for what opened the menu. This may be a chest, event, fishing rod, location, etc.</param>
  public ItemGrabMenu(IList<Item> inventory, object context = null)
    : base(okButton: true, trashCan: true)
  {
    this.context = context;
    this.ItemsToGrabMenu = new InventoryMenu(this.xPositionOnScreen + 32 /*0x20*/, this.yPositionOnScreen, false, inventory);
    this.trashCan.myID = 106;
    this.ItemsToGrabMenu.populateClickableComponentList();
    for (int index = 0; index < this.ItemsToGrabMenu.inventory.Count; ++index)
    {
      if (this.ItemsToGrabMenu.inventory[index] != null)
      {
        this.ItemsToGrabMenu.inventory[index].myID += 53910;
        this.ItemsToGrabMenu.inventory[index].upNeighborID += 53910;
        this.ItemsToGrabMenu.inventory[index].rightNeighborID += 53910;
        this.ItemsToGrabMenu.inventory[index].downNeighborID = -7777;
        this.ItemsToGrabMenu.inventory[index].leftNeighborID += 53910;
        this.ItemsToGrabMenu.inventory[index].fullyImmutable = true;
        if (index % (this.ItemsToGrabMenu.capacity / this.ItemsToGrabMenu.rows) == 0)
          this.ItemsToGrabMenu.inventory[index].leftNeighborID = this.dropItemInvisibleButton.myID;
        if (index % (this.ItemsToGrabMenu.capacity / this.ItemsToGrabMenu.rows) == this.ItemsToGrabMenu.capacity / this.ItemsToGrabMenu.rows - 1)
          this.ItemsToGrabMenu.inventory[index].rightNeighborID = this.trashCan.myID;
      }
    }
    int? count;
    for (int index = 0; index < this.GetColumnCount(); ++index)
    {
      count = this.inventory?.inventory?.Count;
      int columnCount = this.GetColumnCount();
      if (count.GetValueOrDefault() >= columnCount & count.HasValue)
        this.inventory.inventory[index].upNeighborID = this.shippingBin ? 12598 : -7777;
    }
    if (!this.shippingBin)
    {
      for (int index = 0; index < this.GetColumnCount() * 3; ++index)
      {
        InventoryMenu inventory1 = this.inventory;
        int num1;
        if (inventory1 == null)
        {
          num1 = 0;
        }
        else
        {
          count = inventory1.inventory?.Count;
          int num2 = index;
          num1 = count.GetValueOrDefault() > num2 & count.HasValue ? 1 : 0;
        }
        if (num1 != 0)
        {
          this.inventory.inventory[index].upNeighborID = -7777;
          this.inventory.inventory[index].upNeighborImmutable = true;
        }
      }
    }
    if (this.trashCan != null)
      this.trashCan.leftNeighborID = 11;
    if (this.okButton != null)
      this.okButton.leftNeighborID = 11;
    this.populateClickableComponentList();
    if (Game1.options.SnappyMenus)
      this.snapToDefaultClickableComponent();
    this.inventory.showGrayedOutSlots = true;
    this.SetupBorderNeighbors();
  }

  /// <summary>Drop any remaining items that weren't grabbed by the player onto the ground at their feet.</summary>
  public virtual void DropRemainingItems()
  {
    if (this.ItemsToGrabMenu?.actualInventory == null)
      return;
    foreach (Item obj in (IEnumerable<Item>) this.ItemsToGrabMenu.actualInventory)
    {
      if (obj != null)
        Game1.createItemDebris(obj, Game1.player.getStandingPosition(), Game1.player.FacingDirection);
    }
    this.ItemsToGrabMenu.actualInventory.Clear();
  }

  /// <summary>Construct an instance.</summary>
  /// <param name="menu">The menu whose state to copy.</param>
  public ItemGrabMenu(ItemGrabMenu menu)
    : this(menu.ItemsToGrabMenu.actualInventory, menu.reverseGrab, menu.showReceivingMenu, menu.inventory.highlightMethod, menu.behaviorFunction, menu.message, menu.behaviorOnItemGrab, canBeExitedWithKey: menu.canExitOnKey, playRightClickSound: menu.playRightClickSound, allowRightClick: menu.allowRightClick, showOrganizeButton: menu.organizeButton != null, source: menu.source, sourceItem: menu.sourceItem, whichSpecialButton: menu.whichSpecialButton, context: menu.context, heldItemExitBehavior: menu.HeldItemExitBehavior, allowExitWithHeldItem: menu.AllowExitWithHeldItem)
  {
    this.setEssential(menu.essential);
    if (menu.currentlySnappedComponent != null)
    {
      this.setCurrentlySnappedComponentTo(menu.currentlySnappedComponent.myID);
      if (Game1.options.SnappyMenus)
        this.snapCursorToCurrentSnappedComponent();
    }
    this.heldItem = menu.heldItem;
  }

  public ItemGrabMenu(
    IList<Item> inventory,
    bool reverseGrab,
    bool showReceivingMenu,
    InventoryMenu.highlightThisItem highlightFunction,
    ItemGrabMenu.behaviorOnItemSelect behaviorOnItemSelectFunction,
    string message,
    ItemGrabMenu.behaviorOnItemSelect behaviorOnItemGrab = null,
    bool snapToBottom = false,
    bool canBeExitedWithKey = false,
    bool playRightClickSound = true,
    bool allowRightClick = true,
    bool showOrganizeButton = false,
    int source = 0,
    Item sourceItem = null,
    int whichSpecialButton = -1,
    object context = null,
    ItemExitBehavior heldItemExitBehavior = ItemExitBehavior.ReturnToPlayer,
    bool allowExitWithHeldItem = false)
    : base(highlightFunction, true, true, menuOffsetHack: 64 /*0x40*/, heldItemExitBehavior: heldItemExitBehavior, allowExitWithHeldItem: allowExitWithHeldItem)
  {
    this.source = source;
    this.message = message;
    this.reverseGrab = reverseGrab;
    this.showReceivingMenu = showReceivingMenu;
    this.playRightClickSound = playRightClickSound;
    this.allowRightClick = allowRightClick;
    this.inventory.showGrayedOutSlots = true;
    this.sourceItem = sourceItem;
    this.whichSpecialButton = whichSpecialButton;
    this.context = context;
    this._sourceItemInCurrentLocation = sourceItem != null && ((IEnumerable<Item>) Game1.currentLocation.objects.Values).Contains<Item>(sourceItem);
    if (sourceItem is Chest chest1)
    {
      if (this.CanHaveColorPicker())
      {
        Chest itemToDrawColored = new Chest(true, sourceItem.ItemId);
        this.chestColorPicker = new DiscreteColorPicker(this.xPositionOnScreen, this.yPositionOnScreen - 64 /*0x40*/ - IClickableMenu.borderWidth * 2, chest1.playerChoiceColor.Value, (Item) itemToDrawColored);
        itemToDrawColored.playerChoiceColor.Value = DiscreteColorPicker.getColorFromSelection(this.chestColorPicker.colorSelection);
        ClickableTextureComponent textureComponent = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + this.width, this.yPositionOnScreen + this.height / 3 - 64 /*0x40*/ - 160 /*0xA0*/, 64 /*0x40*/, 64 /*0x40*/), Game1.mouseCursors, new Rectangle(119, 469, 16 /*0x10*/, 16 /*0x10*/), 4f);
        textureComponent.hoverText = Game1.content.LoadString("Strings\\UI:Toggle_ColorPicker");
        textureComponent.myID = 27346;
        textureComponent.downNeighborID = -99998;
        textureComponent.leftNeighborID = 53921;
        textureComponent.region = 15923;
        this.colorPickerToggleButton = textureComponent;
      }
      if (source == 1 && (chest1.SpecialChestType == Chest.SpecialChestTypes.None || chest1.SpecialChestType == Chest.SpecialChestTypes.BigChest) && InventoryPage.ShouldShowJunimoNoteIcon())
      {
        ClickableTextureComponent textureComponent = new ClickableTextureComponent("", new Rectangle(this.xPositionOnScreen + this.width, this.yPositionOnScreen + this.height / 3 - 64 /*0x40*/ - 216, 64 /*0x40*/, 64 /*0x40*/), "", Game1.content.LoadString("Strings\\UI:GameMenu_JunimoNote_Hover"), Game1.mouseCursors, new Rectangle(331, 374, 15, 14), 4f);
        textureComponent.myID = 898;
        textureComponent.leftNeighborID = 11;
        textureComponent.downNeighborID = 106;
        this.junimoNoteIcon = textureComponent;
      }
    }
    if (whichSpecialButton == 1)
    {
      ClickableTextureComponent textureComponent = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + this.width, this.yPositionOnScreen + this.height / 3 - 64 /*0x40*/ - 160 /*0xA0*/, 64 /*0x40*/, 64 /*0x40*/), Game1.mouseCursors, new Rectangle(108, 491, 16 /*0x10*/, 16 /*0x10*/), 4f);
      textureComponent.myID = 12485;
      textureComponent.downNeighborID = showOrganizeButton ? 12952 : 5948;
      textureComponent.region = 15923;
      textureComponent.leftNeighborID = 53921;
      this.specialButton = textureComponent;
      if (context is JunimoHut junimoHut)
        this.specialButton.sourceRect.X = junimoHut.noHarvest.Value ? 124 : 108;
    }
    if (snapToBottom)
    {
      this.movePosition(0, Game1.uiViewport.Height - (this.yPositionOnScreen + this.height - IClickableMenu.spaceToClearTopBorder));
      this.snappedtoBottom = true;
    }
    if (source == 1 && sourceItem is Chest chest2 && chest2.GetActualCapacity() != 36)
    {
      int actualCapacity = chest2.GetActualCapacity();
      int rows = actualCapacity >= 70 ? 5 : 3;
      if (actualCapacity < 9)
        rows = 1;
      int num = 64 /*0x40*/ * (actualCapacity / rows);
      this.ItemsToGrabMenu = new InventoryMenu(Game1.uiViewport.Width / 2 - num / 2, this.yPositionOnScreen + (actualCapacity < 70 ? 64 /*0x40*/ : -21), false, inventory, highlightFunction, actualCapacity, rows);
      if (chest2.SpecialChestType == Chest.SpecialChestTypes.MiniShippingBin)
        this.inventory.moveItemSound = "Ship";
      if (rows > 3)
      {
        this.yPositionOnScreen += 42;
        this.inventory.SetPosition(this.inventory.xPositionOnScreen, this.inventory.yPositionOnScreen + 38 + 4);
        this.ItemsToGrabMenu.SetPosition(this.ItemsToGrabMenu.xPositionOnScreen - 32 /*0x20*/ + 8, this.ItemsToGrabMenu.yPositionOnScreen);
        this.storageSpaceTopBorderOffset = 20;
        this.trashCan.bounds.X = this.ItemsToGrabMenu.width + this.ItemsToGrabMenu.xPositionOnScreen + IClickableMenu.borderWidth * 2;
        this.okButton.bounds.X = this.ItemsToGrabMenu.width + this.ItemsToGrabMenu.xPositionOnScreen + IClickableMenu.borderWidth * 2;
      }
    }
    else
      this.ItemsToGrabMenu = new InventoryMenu(this.xPositionOnScreen + 32 /*0x20*/, this.yPositionOnScreen, false, inventory, highlightFunction);
    this.ItemsToGrabMenu.populateClickableComponentList();
    for (int index = 0; index < this.ItemsToGrabMenu.inventory.Count; ++index)
    {
      if (this.ItemsToGrabMenu.inventory[index] != null)
      {
        this.ItemsToGrabMenu.inventory[index].myID += 53910;
        this.ItemsToGrabMenu.inventory[index].upNeighborID += 53910;
        this.ItemsToGrabMenu.inventory[index].rightNeighborID += 53910;
        this.ItemsToGrabMenu.inventory[index].downNeighborID = -7777;
        this.ItemsToGrabMenu.inventory[index].leftNeighborID += 53910;
        this.ItemsToGrabMenu.inventory[index].fullyImmutable = true;
      }
    }
    this.behaviorFunction = behaviorOnItemSelectFunction;
    this.behaviorOnItemGrab = behaviorOnItemGrab;
    this.canExitOnKey = canBeExitedWithKey;
    if (showOrganizeButton)
    {
      ClickableTextureComponent textureComponent1 = new ClickableTextureComponent("", new Rectangle(this.xPositionOnScreen + this.width, this.yPositionOnScreen + this.height / 3 - 64 /*0x40*/ - 64 /*0x40*/ - 16 /*0x10*/, 64 /*0x40*/, 64 /*0x40*/), "", Game1.content.LoadString("Strings\\UI:ItemGrab_FillStacks"), Game1.mouseCursors, new Rectangle(103, 469, 16 /*0x10*/, 16 /*0x10*/), 4f);
      textureComponent1.myID = 12952;
      textureComponent1.upNeighborID = this.colorPickerToggleButton != null ? 27346 : (this.specialButton != null ? 12485 : -500);
      textureComponent1.downNeighborID = 106;
      textureComponent1.leftNeighborID = 53921;
      textureComponent1.region = 15923;
      this.fillStacksButton = textureComponent1;
      ClickableTextureComponent textureComponent2 = new ClickableTextureComponent("", new Rectangle(this.xPositionOnScreen + this.width, this.yPositionOnScreen + this.height / 3 - 64 /*0x40*/, 64 /*0x40*/, 64 /*0x40*/), "", Game1.content.LoadString("Strings\\UI:ItemGrab_Organize"), Game1.mouseCursors, new Rectangle(162, 440, 16 /*0x10*/, 16 /*0x10*/), 4f);
      textureComponent2.myID = 106;
      textureComponent2.upNeighborID = 12952;
      textureComponent2.downNeighborID = 5948;
      textureComponent2.leftNeighborID = 53921;
      textureComponent2.region = 15923;
      this.organizeButton = textureComponent2;
    }
    this.RepositionSideButtons();
    if (this.chestColorPicker != null)
    {
      this.discreteColorPickerCC = new List<ClickableComponent>();
      for (int index = 0; index < DiscreteColorPicker.totalColors; ++index)
      {
        List<ClickableComponent> discreteColorPickerCc = this.discreteColorPickerCC;
        ClickableComponent clickableComponent = new ClickableComponent(new Rectangle(this.chestColorPicker.xPositionOnScreen + IClickableMenu.borderWidth / 2 + index * 9 * 4, this.chestColorPicker.yPositionOnScreen + IClickableMenu.borderWidth / 2, 36, 28), "");
        clickableComponent.myID = index + 4343;
        clickableComponent.rightNeighborID = index < DiscreteColorPicker.totalColors - 1 ? index + 4343 + 1 : -1;
        clickableComponent.leftNeighborID = index > 0 ? index + 4343 - 1 : -1;
        InventoryMenu itemsToGrabMenu = this.ItemsToGrabMenu;
        clickableComponent.downNeighborID = (itemsToGrabMenu != null ? (itemsToGrabMenu.inventory.Count > 0 ? 1 : 0) : 0) != 0 ? 53910 : 0;
        discreteColorPickerCc.Add(clickableComponent);
      }
    }
    if (this.organizeButton != null)
    {
      foreach (ClickableComponent clickableComponent in this.ItemsToGrabMenu.GetBorder(InventoryMenu.BorderSide.Right))
        clickableComponent.rightNeighborID = this.organizeButton.myID;
    }
    if (this.trashCan != null && this.inventory.inventory.Count >= 12 && this.inventory.inventory[11] != null)
      this.inventory.inventory[11].rightNeighborID = 5948;
    if (this.trashCan != null)
      this.trashCan.leftNeighborID = 11;
    if (this.okButton != null)
      this.okButton.leftNeighborID = 11;
    ClickableComponent clickableComponent1 = this.ItemsToGrabMenu.GetBorder(InventoryMenu.BorderSide.Right).FirstOrDefault<ClickableComponent>();
    if (clickableComponent1 != null)
    {
      if (this.organizeButton != null)
        this.organizeButton.leftNeighborID = clickableComponent1.myID;
      if (this.specialButton != null)
        this.specialButton.leftNeighborID = clickableComponent1.myID;
      if (this.fillStacksButton != null)
        this.fillStacksButton.leftNeighborID = clickableComponent1.myID;
      if (this.junimoNoteIcon != null)
        this.junimoNoteIcon.leftNeighborID = clickableComponent1.myID;
    }
    this.populateClickableComponentList();
    if (Game1.options.SnappyMenus)
      this.snapToDefaultClickableComponent();
    this.SetupBorderNeighbors();
  }

  /// <summary>Create an item grab menu to collect items which couldn't be added to the player's inventory directly.</summary>
  /// <param name="items">The items to collect.</param>
  /// <param name="onCollectItem">The callback to invoke when an item is retrieved.</param>
  public static ItemGrabMenu CreateOverflowMenu(
    IList<Item> items,
    ItemGrabMenu.behaviorOnItemSelect onCollectItem = null)
  {
    ItemGrabMenu overflowMenu = new ItemGrabMenu(items).setEssential(true);
    overflowMenu.inventory.showGrayedOutSlots = true;
    overflowMenu.inventory.onAddItem = onCollectItem;
    overflowMenu.source = 4;
    return overflowMenu;
  }

  /// <summary>Position the buttons that appear on the right side of the screen (e.g. to organize or fill stacks), and update their neighbor IDs.</summary>
  public virtual void RepositionSideButtons()
  {
    List<ClickableComponent> clickableComponentList = new List<ClickableComponent>();
    int num1 = this.ItemsToGrabMenu.capacity / this.ItemsToGrabMenu.rows;
    if (this.organizeButton != null)
    {
      this.organizeButton.leftNeighborID = num1 - 1 + 53910;
      clickableComponentList.Add((ClickableComponent) this.organizeButton);
    }
    if (this.fillStacksButton != null)
    {
      this.fillStacksButton.leftNeighborID = num1 - 1 + 53910;
      clickableComponentList.Add((ClickableComponent) this.fillStacksButton);
    }
    if (this.colorPickerToggleButton != null)
    {
      this.colorPickerToggleButton.leftNeighborID = num1 - 1 + 53910;
      clickableComponentList.Add((ClickableComponent) this.colorPickerToggleButton);
    }
    if (this.specialButton != null)
      clickableComponentList.Add((ClickableComponent) this.specialButton);
    if (this.junimoNoteIcon != null)
    {
      this.junimoNoteIcon.leftNeighborID = num1 - 1;
      clickableComponentList.Add((ClickableComponent) this.junimoNoteIcon);
    }
    int num2 = 80 /*0x50*/;
    if (clickableComponentList.Count >= 4)
      num2 = 72;
    for (int index = 0; index < clickableComponentList.Count; ++index)
    {
      ClickableComponent clickableComponent = clickableComponentList[index];
      if (index > 0 && clickableComponentList.Count > 1)
        clickableComponent.downNeighborID = clickableComponentList[index - 1].myID;
      if (index < clickableComponentList.Count - 1 && clickableComponentList.Count > 1)
        clickableComponent.upNeighborID = clickableComponentList[index + 1].myID;
      clickableComponent.bounds.X = this.ItemsToGrabMenu.xPositionOnScreen + this.ItemsToGrabMenu.width + IClickableMenu.borderWidth * 2;
      clickableComponent.bounds.Y = this.ItemsToGrabMenu.yPositionOnScreen + this.height / 3 - 64 /*0x40*/ - num2 * index;
    }
  }

  public void SetupBorderNeighbors()
  {
    foreach (ClickableComponent clickableComponent in this.inventory.GetBorder(InventoryMenu.BorderSide.Right))
    {
      clickableComponent.rightNeighborID = -99998;
      clickableComponent.rightNeighborImmutable = true;
    }
    List<ClickableComponent> border = this.ItemsToGrabMenu.GetBorder(InventoryMenu.BorderSide.Right);
    bool flag = false;
    foreach (ClickableComponent clickableComponent in this.allClickableComponents)
    {
      if (clickableComponent.region == 15923)
      {
        flag = true;
        break;
      }
    }
    foreach (ClickableComponent clickableComponent in border)
    {
      if (flag)
      {
        clickableComponent.rightNeighborID = -99998;
        clickableComponent.rightNeighborImmutable = true;
      }
      else
        clickableComponent.rightNeighborID = -1;
    }
    int? count;
    for (int index = 0; index < this.GetColumnCount(); ++index)
    {
      InventoryMenu inventory = this.inventory;
      int num1;
      if (inventory == null)
      {
        num1 = 0;
      }
      else
      {
        count = inventory.inventory?.Count;
        int num2 = 12;
        num1 = count.GetValueOrDefault() >= num2 & count.HasValue ? 1 : 0;
      }
      if (num1 != 0)
      {
        ClickableComponent clickableComponent = this.inventory.inventory[index];
        int num3;
        if (!this.shippingBin)
        {
          if (this.discreteColorPickerCC != null)
          {
            InventoryMenu itemsToGrabMenu = this.ItemsToGrabMenu;
            if ((itemsToGrabMenu != null ? (itemsToGrabMenu.inventory.Count <= index ? 1 : 0) : 0) != 0 && Game1.player.showChestColorPicker)
            {
              num3 = 4343;
              goto label_31;
            }
          }
          num3 = this.ItemsToGrabMenu.inventory.Count > index ? 53910 + index : 53910;
        }
        else
          num3 = 12598;
label_31:
        clickableComponent.upNeighborID = num3;
      }
      if (this.discreteColorPickerCC != null)
      {
        InventoryMenu itemsToGrabMenu = this.ItemsToGrabMenu;
        if ((itemsToGrabMenu != null ? (itemsToGrabMenu.inventory.Count > index ? 1 : 0) : 0) != 0 && Game1.player.showChestColorPicker)
        {
          this.ItemsToGrabMenu.inventory[index].upNeighborID = 4343;
          continue;
        }
      }
      this.ItemsToGrabMenu.inventory[index].upNeighborID = -1;
    }
    if (this.shippingBin)
      return;
    for (int index = 0; index < 36; ++index)
    {
      InventoryMenu inventory = this.inventory;
      int num4;
      if (inventory == null)
      {
        num4 = 0;
      }
      else
      {
        count = inventory.inventory?.Count;
        int num5 = index;
        num4 = count.GetValueOrDefault() > num5 & count.HasValue ? 1 : 0;
      }
      if (num4 != 0)
      {
        this.inventory.inventory[index].upNeighborID = -7777;
        this.inventory.inventory[index].upNeighborImmutable = true;
      }
    }
  }

  /// <summary>Get whether the chest can display a color picker (regardless of whether it's currently shown).</summary>
  public virtual bool CanHaveColorPicker()
  {
    return this.source == 1 && this.sourceItem is Chest sourceItem && (sourceItem.SpecialChestType == Chest.SpecialChestTypes.None || sourceItem.SpecialChestType == Chest.SpecialChestTypes.BigChest) && !sourceItem.fridge.Value;
  }

  public virtual int GetColumnCount() => this.ItemsToGrabMenu.capacity / this.ItemsToGrabMenu.rows;

  /// <summary>Set whether to rescue items from the menu when it's force-closed (e.g. from passing out at 2am). Rescued items will be added to the player's inventory if possible, else dropped onto the ground at their feet.</summary>
  /// <param name="essential">Whether to rescue items on force-close.</param>
  /// <param name="superEssential">Whether to rescue items on normal close.</param>
  public ItemGrabMenu setEssential(bool essential, bool superEssential = false)
  {
    this.essential = essential | superEssential;
    this.superEssential = superEssential;
    return this;
  }

  public void initializeShippingBin()
  {
    this.shippingBin = true;
    ClickableTextureComponent textureComponent = new ClickableTextureComponent("", new Rectangle(this.xPositionOnScreen + this.width / 2 - 48 /*0x30*/, this.yPositionOnScreen + this.height / 2 - 80 /*0x50*/ - 64 /*0x40*/, 96 /*0x60*/, 96 /*0x60*/), "", Game1.content.LoadString("Strings\\UI:ShippingBin_LastItem"), Game1.mouseCursors, new Rectangle(293, 360, 24, 24), 4f);
    textureComponent.myID = 12598;
    textureComponent.region = 12598;
    this.lastShippedHolder = textureComponent;
    for (int index = 0; index < this.GetColumnCount(); ++index)
    {
      int? count = this.inventory?.inventory?.Count;
      int columnCount = this.GetColumnCount();
      if (count.GetValueOrDefault() >= columnCount & count.HasValue)
      {
        this.inventory.inventory[index].upNeighborID = -7777;
        if (index == 11)
          this.inventory.inventory[index].rightNeighborID = 5948;
      }
    }
    this.populateClickableComponentList();
    if (!Game1.options.SnappyMenus)
      return;
    this.snapToDefaultClickableComponent();
  }

  protected override void customSnapBehavior(int direction, int oldRegion, int oldID)
  {
    switch (direction)
    {
      case 0:
        if (this.shippingBin && Game1.getFarm().lastItemShipped != null && oldID < 12)
        {
          this.currentlySnappedComponent = this.getComponentWithID(12598);
          this.currentlySnappedComponent.downNeighborID = oldID;
          this.snapCursorToCurrentSnappedComponent();
          break;
        }
        if (oldID < 53910 && oldID >= 12)
        {
          this.currentlySnappedComponent = this.getComponentWithID(oldID - 12);
          break;
        }
        int num1 = oldID + this.GetColumnCount() * (this.ItemsToGrabMenu.rows - 1);
        for (int index = 0; index < 3 && this.ItemsToGrabMenu.inventory.Count <= num1; ++index)
          num1 -= this.GetColumnCount();
        if (this.showReceivingMenu)
        {
          if (num1 < 0)
          {
            if (this.ItemsToGrabMenu.inventory.Count > 0)
              this.currentlySnappedComponent = this.getComponentWithID(53910 + this.ItemsToGrabMenu.inventory.Count - 1);
            else if (this.discreteColorPickerCC != null)
              this.currentlySnappedComponent = this.getComponentWithID(4343);
          }
          else
          {
            int num2 = this.inventory.capacity / this.inventory.rows;
            int num3 = this.GetColumnCount() - num2;
            this.currentlySnappedComponent = this.getComponentWithID(num1 + 53910 + num3 / 2);
            if (this.currentlySnappedComponent == null)
              this.currentlySnappedComponent = this.getComponentWithID(53910);
          }
        }
        this.snapCursorToCurrentSnappedComponent();
        break;
      case 2:
        for (int index = 0; index < 12; ++index)
        {
          int? count = this.inventory?.inventory?.Count;
          int columnCount = this.GetColumnCount();
          if (count.GetValueOrDefault() >= columnCount & count.HasValue && this.shippingBin)
            this.inventory.inventory[index].upNeighborID = this.shippingBin ? 12598 : Math.Min(index, this.ItemsToGrabMenu.inventory.Count - 1) + 53910;
        }
        if (!this.shippingBin && oldID >= 53910)
        {
          int num4 = oldID - 53910;
          if (num4 + this.GetColumnCount() <= this.ItemsToGrabMenu.inventory.Count - 1)
          {
            this.currentlySnappedComponent = this.getComponentWithID(num4 + this.GetColumnCount() + 53910);
            this.snapCursorToCurrentSnappedComponent();
            break;
          }
        }
        if (this.inventory != null)
        {
          int num5 = this.inventory.capacity / this.inventory.rows;
          int num6 = this.GetColumnCount() - num5;
          this.currentlySnappedComponent = this.getComponentWithID(oldRegion == 12598 ? 0 : Math.Max(0, Math.Min((oldID - 53910) % this.GetColumnCount() - num6 / 2, this.inventory.capacity / this.inventory.rows - num6 / 2)));
        }
        else
          this.currentlySnappedComponent = this.getComponentWithID(oldRegion == 12598 ? 0 : (oldID - 53910) % this.GetColumnCount());
        this.snapCursorToCurrentSnappedComponent();
        break;
    }
  }

  public override void snapToDefaultClickableComponent()
  {
    if (this.shippingBin)
      this.currentlySnappedComponent = this.getComponentWithID(0);
    else if (this.source == 1 && this.sourceItem is Chest sourceItem && sourceItem.SpecialChestType == Chest.SpecialChestTypes.MiniShippingBin)
      this.currentlySnappedComponent = this.getComponentWithID(0);
    else
      this.currentlySnappedComponent = this.getComponentWithID(this.ItemsToGrabMenu.inventory.Count <= 0 || !this.showReceivingMenu ? 0 : 53910);
    this.snapCursorToCurrentSnappedComponent();
  }

  public void setSourceItem(Item item)
  {
    this.sourceItem = item;
    this.chestColorPicker = (DiscreteColorPicker) null;
    this.colorPickerToggleButton = (ClickableTextureComponent) null;
    if (this.CanHaveColorPicker() && this.sourceItem is Chest sourceItem)
    {
      Chest itemToDrawColored = new Chest(true, this.sourceItem.ItemId);
      this.chestColorPicker = new DiscreteColorPicker(this.xPositionOnScreen, this.yPositionOnScreen - 64 /*0x40*/ - IClickableMenu.borderWidth * 2, sourceItem.playerChoiceColor.Value, (Item) itemToDrawColored);
      if (sourceItem.SpecialChestType == Chest.SpecialChestTypes.BigChest)
        this.chestColorPicker.yPositionOnScreen -= 42;
      itemToDrawColored.playerChoiceColor.Value = DiscreteColorPicker.getColorFromSelection(this.chestColorPicker.colorSelection);
      this.colorPickerToggleButton = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + this.width, this.yPositionOnScreen + this.height / 3 - 64 /*0x40*/ - 160 /*0xA0*/, 64 /*0x40*/, 64 /*0x40*/), Game1.mouseCursors, new Rectangle(119, 469, 16 /*0x10*/, 16 /*0x10*/), 4f)
      {
        hoverText = Game1.content.LoadString("Strings\\UI:Toggle_ColorPicker")
      };
    }
    this.RepositionSideButtons();
  }

  public override bool IsAutomaticSnapValid(
    int direction,
    ClickableComponent a,
    ClickableComponent b)
  {
    return (direction != 1 || !this.ItemsToGrabMenu.inventory.Contains(a) || !this.inventory.inventory.Contains(b)) && base.IsAutomaticSnapValid(direction, a, b);
  }

  public void setBackgroundTransparency(bool b) => this.drawBG = b;

  public void setDestroyItemOnClick(bool b) => this.destroyItemOnClick = b;

  /// <inheritdoc />
  public override void receiveRightClick(int x, int y, bool playSound = true)
  {
    if (!this.allowRightClick)
    {
      this.receiveRightClickOnlyToolAttachments(x, y);
    }
    else
    {
      base.receiveRightClick(x, y, playSound && this.playRightClickSound);
      if (this.heldItem == null && this.showReceivingMenu)
      {
        this.heldItem = this.ItemsToGrabMenu.rightClick(x, y, this.heldItem, false);
        if (this.heldItem != null && this.behaviorOnItemGrab != null)
        {
          this.behaviorOnItemGrab(this.heldItem, Game1.player);
          if (Game1.activeClickableMenu is ItemGrabMenu activeClickableMenu)
          {
            activeClickableMenu.setSourceItem(this.sourceItem);
            if (Game1.options.SnappyMenus)
            {
              activeClickableMenu.currentlySnappedComponent = this.currentlySnappedComponent;
              activeClickableMenu.snapCursorToCurrentSnappedComponent();
            }
          }
        }
        if (this.heldItem?.QualifiedItemId == "(O)326")
        {
          this.heldItem = (Item) null;
          Game1.player.canUnderstandDwarves = true;
          this.poof = new TemporaryAnimatedSprite("TileSheets\\animations", new Rectangle(0, 320, 64 /*0x40*/, 64 /*0x40*/), 50f, 8, 0, new Vector2((float) (x - x % 64 /*0x40*/ + 16 /*0x10*/), (float) (y - y % 64 /*0x40*/ + 16 /*0x10*/)), false, false);
          Game1.playSound("fireball");
        }
        else if (this.heldItem is StardewValley.Object heldItem && heldItem?.QualifiedItemId == "(O)434")
        {
          this.heldItem = (Item) null;
          this.exitThisMenu(false);
          Game1.player.eatObject(heldItem, true);
        }
        else if (this.heldItem != null && this.heldItem.IsRecipe)
        {
          this.heldItem.LearnRecipe();
          this.poof = new TemporaryAnimatedSprite("TileSheets\\animations", new Rectangle(0, 320, 64 /*0x40*/, 64 /*0x40*/), 50f, 8, 0, new Vector2((float) (x - x % 64 /*0x40*/ + 16 /*0x10*/), (float) (y - y % 64 /*0x40*/ + 16 /*0x10*/)), false, false);
          Game1.playSound("newRecipe");
          this.heldItem = (Item) null;
        }
        else
        {
          if (!Game1.player.addItemToInventoryBool(this.heldItem))
            return;
          this.heldItem = (Item) null;
          Game1.playSound("coin");
        }
      }
      else
      {
        if (!this.reverseGrab && this.behaviorFunction == null)
          return;
        this.behaviorFunction(this.heldItem, Game1.player);
        if (Game1.activeClickableMenu is ItemGrabMenu activeClickableMenu)
          activeClickableMenu.setSourceItem(this.sourceItem);
        if (!this.destroyItemOnClick)
          return;
        this.heldItem = (Item) null;
      }
    }
  }

  /// <inheritdoc />
  public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
  {
    if (this.snappedtoBottom)
      this.movePosition((newBounds.Width - oldBounds.Width) / 2, Game1.uiViewport.Height - (this.yPositionOnScreen + this.height - IClickableMenu.spaceToClearTopBorder));
    else
      this.movePosition((newBounds.Width - oldBounds.Width) / 2, (newBounds.Height - oldBounds.Height) / 2);
    this.ItemsToGrabMenu?.gameWindowSizeChanged(oldBounds, newBounds);
    this.RepositionSideButtons();
    if (!this.CanHaveColorPicker() || !(this.sourceItem is Chest sourceItem))
      return;
    this.chestColorPicker = new DiscreteColorPicker(this.xPositionOnScreen, this.yPositionOnScreen - 64 /*0x40*/ - IClickableMenu.borderWidth * 2, sourceItem.playerChoiceColor.Value, (Item) new Chest(true, this.sourceItem.ItemId));
  }

  /// <inheritdoc />
  public override void receiveLeftClick(int x, int y, bool playSound = true)
  {
    base.receiveLeftClick(x, y, !this.destroyItemOnClick);
    if (this.shippingBin && this.lastShippedHolder.containsPoint(x, y))
    {
      if (Game1.getFarm().lastItemShipped == null)
        return;
      Game1.getFarm().getShippingBin(Game1.player).Remove(Game1.getFarm().lastItemShipped);
      if (Game1.player.addItemToInventoryBool(Game1.getFarm().lastItemShipped))
      {
        Game1.playSound("coin");
        Game1.getFarm().lastItemShipped = (Item) null;
        if (Game1.player.ActiveObject == null)
          return;
        Game1.player.showCarrying();
        Game1.player.Halt();
      }
      else
        Game1.getFarm().getShippingBin(Game1.player).Add(Game1.getFarm().lastItemShipped);
    }
    else
    {
      if (this.chestColorPicker != null)
      {
        this.chestColorPicker.receiveLeftClick(x, y, true);
        if (this.sourceItem is Chest sourceItem)
          sourceItem.playerChoiceColor.Value = DiscreteColorPicker.getColorFromSelection(this.chestColorPicker.colorSelection);
      }
      if (this.colorPickerToggleButton != null && this.colorPickerToggleButton.containsPoint(x, y))
      {
        Game1.player.showChestColorPicker = !Game1.player.showChestColorPicker;
        this.chestColorPicker.visible = Game1.player.showChestColorPicker;
        try
        {
          Game1.playSound("drumkit6");
        }
        catch (Exception ex)
        {
        }
        this.SetupBorderNeighbors();
      }
      else if (this.whichSpecialButton != -1 && this.specialButton != null && this.specialButton.containsPoint(x, y))
      {
        Game1.playSound("drumkit6");
        if (this.whichSpecialButton != 1 || !(this.context is JunimoHut context))
          return;
        context.noHarvest.Value = !context.noHarvest.Value;
        this.specialButton.sourceRect.X = context.noHarvest.Value ? 124 : 108;
      }
      else
      {
        if (this.heldItem == null && this.showReceivingMenu)
        {
          this.heldItem = this.ItemsToGrabMenu.leftClick(x, y, this.heldItem, false);
          if (this.heldItem != null && this.behaviorOnItemGrab != null)
          {
            this.behaviorOnItemGrab(this.heldItem, Game1.player);
            if (Game1.activeClickableMenu is ItemGrabMenu activeClickableMenu)
            {
              activeClickableMenu.setSourceItem(this.sourceItem);
              if (Game1.options.SnappyMenus)
              {
                activeClickableMenu.currentlySnappedComponent = this.currentlySnappedComponent;
                activeClickableMenu.snapCursorToCurrentSnappedComponent();
              }
            }
          }
          switch (this.heldItem?.QualifiedItemId)
          {
            case "(O)326":
              this.heldItem = (Item) null;
              Game1.player.canUnderstandDwarves = true;
              this.poof = new TemporaryAnimatedSprite("TileSheets\\animations", new Rectangle(0, 320, 64 /*0x40*/, 64 /*0x40*/), 50f, 8, 0, new Vector2((float) (x - x % 64 /*0x40*/ + 16 /*0x10*/), (float) (y - y % 64 /*0x40*/ + 16 /*0x10*/)), false, false);
              Game1.playSound("fireball");
              break;
            case "(O)102":
              this.heldItem = (Item) null;
              Game1.player.foundArtifact("102", 1);
              this.poof = new TemporaryAnimatedSprite("TileSheets\\animations", new Rectangle(0, 320, 64 /*0x40*/, 64 /*0x40*/), 50f, 8, 0, new Vector2((float) (x - x % 64 /*0x40*/ + 16 /*0x10*/), (float) (y - y % 64 /*0x40*/ + 16 /*0x10*/)), false, false);
              Game1.playSound("fireball");
              break;
          }
          if (this.heldItem is StardewValley.Object heldItem && heldItem?.QualifiedItemId == "(O)434")
          {
            this.heldItem = (Item) null;
            this.exitThisMenu(false);
            Game1.player.eatObject(heldItem, true);
          }
          else if (this.heldItem != null && this.heldItem.IsRecipe)
          {
            this.heldItem.LearnRecipe();
            this.poof = new TemporaryAnimatedSprite("TileSheets\\animations", new Rectangle(0, 320, 64 /*0x40*/, 64 /*0x40*/), 50f, 8, 0, new Vector2((float) (x - x % 64 /*0x40*/ + 16 /*0x10*/), (float) (y - y % 64 /*0x40*/ + 16 /*0x10*/)), false, false);
            Game1.playSound("newRecipe");
            this.heldItem = (Item) null;
          }
          else if (Game1.player.addItemToInventoryBool(this.heldItem))
          {
            this.heldItem = (Item) null;
            Game1.playSound("coin");
          }
        }
        else if ((this.reverseGrab || this.behaviorFunction != null) && this.isWithinBounds(x, y))
        {
          this.behaviorFunction(this.heldItem, Game1.player);
          if (Game1.activeClickableMenu is ItemGrabMenu activeClickableMenu)
          {
            activeClickableMenu.setSourceItem(this.sourceItem);
            if (Game1.options.SnappyMenus)
            {
              activeClickableMenu.currentlySnappedComponent = this.currentlySnappedComponent;
              activeClickableMenu.snapCursorToCurrentSnappedComponent();
            }
          }
          if (this.destroyItemOnClick)
          {
            this.heldItem = (Item) null;
            return;
          }
        }
        if (this.organizeButton != null && this.organizeButton.containsPoint(x, y))
        {
          ItemGrabMenu.organizeItemsInList(this.ItemsToGrabMenu.actualInventory);
          Game1.activeClickableMenu = (IClickableMenu) new ItemGrabMenu(this);
          Game1.playSound("Ship");
        }
        else if (this.fillStacksButton != null && this.fillStacksButton.containsPoint(x, y))
        {
          this.FillOutStacks();
          Game1.playSound("Ship");
        }
        else if (this.junimoNoteIcon != null && this.junimoNoteIcon.containsPoint(x, y))
        {
          if (!this.readyToClose())
            return;
          Game1.activeClickableMenu = (IClickableMenu) new JunimoNoteMenu(true)
          {
            menuToReturnTo = (IClickableMenu) this
          };
        }
        else
        {
          if (this.heldItem == null || this.isWithinBounds(x, y) || !this.heldItem.canBeTrashed())
            return;
          this.DropHeldItem();
        }
      }
    }
  }

  /// <summary>Merge any items from the player inventory into an equivalent stack in the chest where possible.</summary>
  public void FillOutStacks()
  {
    IList<Item> actualInventory1 = this.inventory.actualInventory;
    IList<Item> actualInventory2 = this.ItemsToGrabMenu.actualInventory;
    HashSet<int> intSet = new HashSet<int>();
    ILookup<string, Item> lookup = actualInventory2.Where<Item>((Func<Item, bool>) (item => item != null)).ToLookup<Item, string>((Func<Item, string>) (item => item.QualifiedItemId));
    if (lookup.Count == 0)
      return;
    for (int index1 = 0; index1 < actualInventory1.Count; ++index1)
    {
      Item obj1 = actualInventory1[index1];
      if (obj1 != null)
      {
        bool flag1 = false;
        foreach (Item obj2 in lookup[obj1.QualifiedItemId])
        {
          flag1 = obj2.canStackWith((ISalable) obj1);
          if (flag1)
            break;
        }
        if (flag1)
        {
          Item obj3 = obj1;
          bool flag2 = false;
          int index2 = -1;
          for (int index3 = 0; index3 < actualInventory2.Count; ++index3)
          {
            Item obj4 = actualInventory2[index3];
            if (obj4 == null)
            {
              if (index2 == -1)
                index2 = index3;
            }
            else if (obj4.canStackWith((ISalable) obj1))
            {
              int amount = obj1.Stack - obj4.addToStack(obj1);
              if (amount > 0)
              {
                flag2 = true;
                intSet.Add(index3);
                obj1 = obj1.ConsumeStack(amount);
                if (obj1 == null)
                {
                  actualInventory1[index1] = (Item) null;
                  break;
                }
              }
            }
          }
          if (obj1 != null)
          {
            if (index2 == -1 && actualInventory2.Count < this.ItemsToGrabMenu.capacity)
            {
              index2 = actualInventory2.Count;
              actualInventory2.Add((Item) null);
            }
            if (index2 > -1)
            {
              flag2 = true;
              intSet.Add(index2);
              obj1.onDetachedFromParent();
              actualInventory2[index2] = obj1;
              actualInventory1[index1] = (Item) null;
            }
          }
          if (flag2)
            this._transferredItemSprites.Add(new ItemGrabMenu.TransferredItemSprite(obj3.getOne(), this.inventory.inventory[index1].bounds.X, this.inventory.inventory[index1].bounds.Y));
        }
      }
    }
    foreach (int index in intSet)
      this.ItemsToGrabMenu.ShakeItem(index);
  }

  /// <summary>Consolidate and sort item stacks in an item list.</summary>
  /// <param name="items">The item list to change.</param>
  public static void organizeItemsInList(IList<Item> items)
  {
    List<Item> objList = new List<Item>((IEnumerable<Item>) items);
    List<Item> tools = new List<Item>();
    objList.RemoveAll((Predicate<Item>) (item =>
    {
      if (item == null)
        return true;
      if (!(item is Tool))
        return false;
      tools.Add(item);
      return true;
    }));
    for (int index1 = 0; index1 < objList.Count; ++index1)
    {
      Item obj1 = objList[index1];
      if (obj1.getRemainingStackSpace() > 0)
      {
        for (int index2 = index1 + 1; index2 < objList.Count; ++index2)
        {
          Item obj2 = objList[index2];
          if (obj1.canStackWith((ISalable) obj2))
          {
            obj2.Stack = obj1.addToStack(obj2);
            if (obj2.Stack == 0)
            {
              objList.RemoveAt(index2);
              --index2;
            }
          }
        }
      }
    }
    objList.Sort();
    objList.InsertRange(0, (IEnumerable<Item>) tools);
    for (int index = 0; index < items.Count; ++index)
      items[index] = (Item) null;
    for (int index = 0; index < objList.Count; ++index)
      items[index] = objList[index];
  }

  public bool areAllItemsTaken()
  {
    for (int index = 0; index < this.ItemsToGrabMenu.actualInventory.Count; ++index)
    {
      if (this.ItemsToGrabMenu.actualInventory[index] != null)
        return false;
    }
    return true;
  }

  /// <inheritdoc />
  public override void receiveGamePadButton(Buttons button)
  {
    base.receiveGamePadButton(button);
    switch (button)
    {
      case Buttons.Back:
        if (this.organizeButton == null)
          break;
        ItemGrabMenu.organizeItemsInList((IList<Item>) Game1.player.Items);
        Game1.playSound("Ship");
        break;
      case Buttons.LeftShoulder:
        if (this.shippingBin)
          break;
        ClickableComponent componentWithId1 = this.getComponentWithID(53910);
        if (componentWithId1 != null)
        {
          this.setCurrentlySnappedComponentTo(componentWithId1.myID);
          this.snapCursorToCurrentSnappedComponent();
          break;
        }
        if (this.getComponentWithID(0) == null)
          break;
        this.setCurrentlySnappedComponentTo(0);
        this.snapCursorToCurrentSnappedComponent();
        break;
      case Buttons.RightShoulder:
        ClickableComponent componentWithId2 = this.getComponentWithID(12952);
        if (componentWithId2 != null)
        {
          this.setCurrentlySnappedComponentTo(componentWithId2.myID);
          this.snapCursorToCurrentSnappedComponent();
          break;
        }
        int num = -1;
        ClickableComponent clickableComponent1 = (ClickableComponent) null;
        foreach (ClickableComponent clickableComponent2 in this.allClickableComponents)
        {
          if (clickableComponent2.region == 15923 && (num == -1 || clickableComponent2.bounds.Y < num))
          {
            num = clickableComponent2.bounds.Y;
            clickableComponent1 = clickableComponent2;
          }
        }
        if (clickableComponent1 == null)
          break;
        this.setCurrentlySnappedComponentTo(clickableComponent1.myID);
        this.snapCursorToCurrentSnappedComponent();
        break;
    }
  }

  /// <inheritdoc />
  public override void receiveKeyPress(Keys key)
  {
    if (Game1.options.snappyMenus && Game1.options.gamepadControls)
      this.applyMovementKey(key);
    if ((this.canExitOnKey || this.areAllItemsTaken()) && Game1.options.doesInputListContain(Game1.options.menuButton, key) && this.readyToClose())
    {
      this.exitThisMenu();
      Event currentEvent = Game1.currentLocation.currentEvent;
      if ((currentEvent != null ? (currentEvent.CurrentCommand > 0 ? 1 : 0) : 0) != 0)
        ++Game1.currentLocation.currentEvent.CurrentCommand;
    }
    else if (Game1.options.doesInputListContain(Game1.options.menuButton, key) && this.heldItem != null)
      Game1.setMousePosition(this.trashCan.bounds.Center);
    if (key != Keys.Delete || this.heldItem == null || !this.heldItem.canBeTrashed())
      return;
    Utility.trashItem(this.heldItem);
    this.heldItem = (Item) null;
  }

  /// <inheritdoc />
  public override void update(GameTime time)
  {
    base.update(time);
    if (!this.HasUpdateTicked)
    {
      this.HasUpdateTicked = true;
      if (this.source == 4)
      {
        IList<Item> actualInventory = this.ItemsToGrabMenu.actualInventory;
        for (int index = 0; index < actualInventory.Count; ++index)
        {
          if (actualInventory[index]?.QualifiedItemId == "(O)434")
          {
            List<Item> items = new List<Item>((IEnumerable<Item>) actualInventory);
            items.RemoveAt(index);
            items.RemoveAll((Predicate<Item>) (p => p == null));
            if (items.Count > 0)
              Game1.nextClickableMenu.Insert(0, (IClickableMenu) ItemGrabMenu.CreateOverflowMenu((IList<Item>) items, this.inventory.onAddItem));
            this.essential = false;
            this.superEssential = false;
            this.exitThisMenu(false);
            Game1.player.eatObject(actualInventory[index] as StardewValley.Object, true);
            return;
          }
        }
      }
    }
    if (this.poof != null && this.poof.update(time))
      this.poof = (TemporaryAnimatedSprite) null;
    this.chestColorPicker?.update(time);
    if (this.sourceItem is Chest sourceItem && this._sourceItemInCurrentLocation)
    {
      Vector2 key = sourceItem.tileLocation.Value;
      if (key != Vector2.Zero && !Game1.currentLocation.objects.ContainsKey(key))
      {
        if (Game1.activeClickableMenu != null)
          Game1.activeClickableMenu.emergencyShutDown();
        Game1.exitActiveMenu();
      }
    }
    this._transferredItemSprites.RemoveAll((Predicate<ItemGrabMenu.TransferredItemSprite>) (sprite => sprite.Update(time)));
  }

  /// <inheritdoc />
  public override void performHoverAction(int x, int y)
  {
    this.hoveredItem = (Item) null;
    this.hoverText = "";
    base.performHoverAction(x, y);
    if (this.colorPickerToggleButton != null)
    {
      this.colorPickerToggleButton.tryHover(x, y, 0.25f);
      if (this.colorPickerToggleButton.containsPoint(x, y))
        this.hoverText = this.colorPickerToggleButton.hoverText;
    }
    if (this.organizeButton != null)
    {
      this.organizeButton.tryHover(x, y, 0.25f);
      if (this.organizeButton.containsPoint(x, y))
        this.hoverText = this.organizeButton.hoverText;
    }
    if (this.fillStacksButton != null)
    {
      this.fillStacksButton.tryHover(x, y, 0.25f);
      if (this.fillStacksButton.containsPoint(x, y))
        this.hoverText = this.fillStacksButton.hoverText;
    }
    this.specialButton?.tryHover(x, y, 0.25f);
    if (this.showReceivingMenu)
    {
      Item obj = this.ItemsToGrabMenu.hover(x, y, this.heldItem);
      if (obj != null)
        this.hoveredItem = obj;
    }
    if (this.junimoNoteIcon != null)
    {
      this.junimoNoteIcon.tryHover(x, y);
      if (this.junimoNoteIcon.containsPoint(x, y))
        this.hoverText = this.junimoNoteIcon.hoverText;
      if (GameMenu.bundleItemHovered)
      {
        this.junimoNoteIcon.scale = this.junimoNoteIcon.baseScale + (float) Math.Sin((double) this.junimoNotePulser / 100.0) / 4f;
        this.junimoNotePulser += (int) Game1.currentGameTime.ElapsedGameTime.TotalMilliseconds;
      }
      else
      {
        this.junimoNotePulser = 0;
        this.junimoNoteIcon.scale = this.junimoNoteIcon.baseScale;
      }
    }
    if (this.hoverText != null)
      return;
    if (this.organizeButton != null)
    {
      this.hoverText = (string) null;
      this.organizeButton.tryHover(x, y);
      if (this.organizeButton.containsPoint(x, y))
        this.hoverText = this.organizeButton.hoverText;
    }
    if (this.shippingBin)
    {
      this.hoverText = (string) null;
      if (this.lastShippedHolder.containsPoint(x, y) && Game1.getFarm().lastItemShipped != null)
        this.hoverText = this.lastShippedHolder.hoverText;
    }
    this.chestColorPicker?.performHoverAction(x, y);
  }

  /// <inheritdoc />
  public override void draw(SpriteBatch b)
  {
    if (this.drawBG && !Game1.options.showClearBackgrounds)
      b.Draw(Game1.fadeToBlackRect, new Rectangle(0, 0, Game1.uiViewport.Width, Game1.uiViewport.Height), Color.Black * 0.5f);
    this.draw(b, false, false);
    if (this.showReceivingMenu)
    {
      b.Draw(Game1.mouseCursors, new Vector2((float) (this.xPositionOnScreen - 64 /*0x40*/), (float) (this.yPositionOnScreen + this.height / 2 + 64 /*0x40*/ + 16 /*0x10*/)), new Rectangle?(new Rectangle(16 /*0x10*/, 368, 12, 16 /*0x10*/)), Color.White, 4.712389f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
      b.Draw(Game1.mouseCursors, new Vector2((float) (this.xPositionOnScreen - 64 /*0x40*/), (float) (this.yPositionOnScreen + this.height / 2 + 64 /*0x40*/ - 16 /*0x10*/)), new Rectangle?(new Rectangle(21, 368, 11, 16 /*0x10*/)), Color.White, 4.712389f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
      b.Draw(Game1.mouseCursors, new Vector2((float) (this.xPositionOnScreen - 40), (float) (this.yPositionOnScreen + this.height / 2 + 64 /*0x40*/ - 44)), new Rectangle?(new Rectangle(4, 372, 8, 11)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
      Game1.drawDialogueBox(this.ItemsToGrabMenu.xPositionOnScreen - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder, this.ItemsToGrabMenu.yPositionOnScreen - IClickableMenu.borderWidth - IClickableMenu.spaceToClearTopBorder + this.storageSpaceTopBorderOffset, this.ItemsToGrabMenu.width + IClickableMenu.borderWidth * 2 + IClickableMenu.spaceToClearSideBorder * 2, this.ItemsToGrabMenu.height + IClickableMenu.spaceToClearTopBorder + IClickableMenu.borderWidth * 2 - this.storageSpaceTopBorderOffset, false, true);
      if ((this.source != 1 || !(this.sourceItem is Chest sourceItem) || sourceItem.SpecialChestType != Chest.SpecialChestTypes.MiniShippingBin && sourceItem.SpecialChestType != Chest.SpecialChestTypes.JunimoChest && sourceItem.SpecialChestType != Chest.SpecialChestTypes.Enricher) && this.source != 0)
      {
        b.Draw(Game1.mouseCursors, new Vector2((float) (this.ItemsToGrabMenu.xPositionOnScreen - 100), (float) (this.yPositionOnScreen + 64 /*0x40*/ + 16 /*0x10*/)), new Rectangle?(new Rectangle(16 /*0x10*/, 368, 12, 16 /*0x10*/)), Color.White, 4.712389f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
        b.Draw(Game1.mouseCursors, new Vector2((float) (this.ItemsToGrabMenu.xPositionOnScreen - 100), (float) (this.yPositionOnScreen + 64 /*0x40*/ - 16 /*0x10*/)), new Rectangle?(new Rectangle(21, 368, 11, 16 /*0x10*/)), Color.White, 4.712389f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
        Rectangle rectangle = new Rectangle((int) sbyte.MaxValue, 412, 10, 11);
        switch (this.source)
        {
          case 3:
            rectangle.X += 10;
            break;
          case 4:
            rectangle.X += 20;
            break;
        }
        b.Draw(Game1.mouseCursors, new Vector2((float) (this.ItemsToGrabMenu.xPositionOnScreen - 80 /*0x50*/), (float) (this.yPositionOnScreen + 64 /*0x40*/ - 44)), new Rectangle?(rectangle), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
      }
      this.ItemsToGrabMenu.draw(b);
    }
    else if (this.message != null)
      Game1.drawDialogueBox(Game1.uiViewport.Width / 2, this.ItemsToGrabMenu.yPositionOnScreen + this.ItemsToGrabMenu.height / 2, false, false, this.message);
    this.poof?.draw(b, true);
    foreach (ItemGrabMenu.TransferredItemSprite transferredItemSprite in this._transferredItemSprites)
      transferredItemSprite.Draw(b);
    if (this.shippingBin && Game1.getFarm().lastItemShipped != null)
    {
      this.lastShippedHolder.draw(b);
      Game1.getFarm().lastItemShipped.drawInMenu(b, new Vector2((float) (this.lastShippedHolder.bounds.X + 16 /*0x10*/), (float) (this.lastShippedHolder.bounds.Y + 16 /*0x10*/)), 1f);
      b.Draw(Game1.mouseCursors, new Vector2((float) (this.lastShippedHolder.bounds.X - 8), (float) (this.lastShippedHolder.bounds.Bottom - 100)), new Rectangle?(new Rectangle(325, 448, 5, 14)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
      b.Draw(Game1.mouseCursors, new Vector2((float) (this.lastShippedHolder.bounds.X + 84), (float) (this.lastShippedHolder.bounds.Bottom - 100)), new Rectangle?(new Rectangle(325, 448, 5, 14)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
      b.Draw(Game1.mouseCursors, new Vector2((float) (this.lastShippedHolder.bounds.X - 8), (float) (this.lastShippedHolder.bounds.Bottom - 44)), new Rectangle?(new Rectangle(325, 452, 5, 13)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
      b.Draw(Game1.mouseCursors, new Vector2((float) (this.lastShippedHolder.bounds.X + 84), (float) (this.lastShippedHolder.bounds.Bottom - 44)), new Rectangle?(new Rectangle(325, 452, 5, 13)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
    }
    if (this.colorPickerToggleButton != null)
      this.colorPickerToggleButton.draw(b);
    else
      this.specialButton?.draw(b);
    this.chestColorPicker?.draw(b);
    this.organizeButton?.draw(b);
    this.fillStacksButton?.draw(b);
    this.junimoNoteIcon?.draw(b);
    if (this.hoverText != null && (this.hoveredItem == null || this.ItemsToGrabMenu == null))
    {
      if (this.hoverAmount > 0)
        IClickableMenu.drawToolTip(b, this.hoverText, "", (Item) null, true, moneyAmountToShowAtBottom: this.hoverAmount);
      else
        IClickableMenu.drawHoverText(b, this.hoverText, Game1.smallFont);
    }
    if (this.hoveredItem != null)
      IClickableMenu.drawToolTip(b, this.hoveredItem.getDescription(), this.hoveredItem.DisplayName, this.hoveredItem, this.heldItem != null);
    else if (this.hoveredItem != null && this.ItemsToGrabMenu != null)
      IClickableMenu.drawToolTip(b, this.ItemsToGrabMenu.descriptionText, this.ItemsToGrabMenu.descriptionTitle, this.hoveredItem, this.heldItem != null);
    this.heldItem?.drawInMenu(b, new Vector2((float) (Game1.getOldMouseX() + 8), (float) (Game1.getOldMouseY() + 8)), 1f);
    Game1.mouseCursorTransparency = 1f;
    this.drawMouse(b);
  }

  /// <inheritdoc />
  protected override void cleanupBeforeExit()
  {
    base.cleanupBeforeExit();
    if (!this.superEssential)
      return;
    this.DropRemainingItems();
  }

  public override void emergencyShutDown()
  {
    base.emergencyShutDown();
    if (!this.essential)
      return;
    foreach (Item obj in (IEnumerable<Item>) this.ItemsToGrabMenu.actualInventory)
    {
      if (obj != null)
      {
        Item inventory = Game1.player.addItemToInventory(obj);
        if (inventory != null)
          Game1.createItemDebris(inventory, Game1.player.getStandingPosition(), Game1.player.FacingDirection);
      }
    }
  }

  public delegate void behaviorOnItemSelect(Item item, Farmer who);

  public class TransferredItemSprite
  {
    public Item item;
    public Vector2 position;
    public float age;
    public float alpha = 1f;

    public TransferredItemSprite(Item transferred_item, int start_x, int start_y)
    {
      this.item = transferred_item;
      this.position.X = (float) start_x;
      this.position.Y = (float) start_y;
    }

    public bool Update(GameTime time)
    {
      float num = 0.15f;
      this.position.Y -= (float) (time.ElapsedGameTime.TotalSeconds * 128.0);
      this.age += (float) time.ElapsedGameTime.TotalSeconds;
      this.alpha = (float) (1.0 - (double) this.age / (double) num);
      return (double) this.age >= (double) num;
    }

    public void Draw(SpriteBatch b)
    {
      this.item.drawInMenu(b, this.position, 1f, this.alpha, 0.9f, StackDrawType.Hide, Color.White, false);
    }
  }
}
