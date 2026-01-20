// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.StorageContainer
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

#nullable disable
namespace StardewValley.Menus;

public class StorageContainer : MenuWithInventory
{
  public InventoryMenu ItemsToGrabMenu;
  private TemporaryAnimatedSprite poof;
  private StorageContainer.behaviorOnItemChange itemChangeBehavior;

  public StorageContainer(
    IList<Item> inventory,
    int capacity,
    int rows = 3,
    StorageContainer.behaviorOnItemChange itemChangeBehavior = null,
    InventoryMenu.highlightThisItem highlightMethod = null)
    : base(highlightMethod, true, true)
  {
    this.itemChangeBehavior = itemChangeBehavior;
    int num = 64 /*0x40*/ * (capacity / rows);
    this.ItemsToGrabMenu = new InventoryMenu(Game1.uiViewport.Width / 2 - num / 2, this.yPositionOnScreen + 64 /*0x40*/, false, inventory, capacity: capacity, rows: rows);
    for (int index = 0; index < this.ItemsToGrabMenu.actualInventory.Count; ++index)
    {
      if (index >= this.ItemsToGrabMenu.actualInventory.Count - this.ItemsToGrabMenu.capacity / this.ItemsToGrabMenu.rows)
        this.ItemsToGrabMenu.inventory[index].downNeighborID = index + 53910;
    }
    for (int index = 0; index < this.inventory.inventory.Count; ++index)
    {
      this.inventory.inventory[index].myID = index + 53910;
      if (this.inventory.inventory[index].downNeighborID != -1)
        this.inventory.inventory[index].downNeighborID += 53910;
      if (this.inventory.inventory[index].rightNeighborID != -1)
        this.inventory.inventory[index].rightNeighborID += 53910;
      if (this.inventory.inventory[index].leftNeighborID != -1)
        this.inventory.inventory[index].leftNeighborID += 53910;
      if (this.inventory.inventory[index].upNeighborID != -1)
        this.inventory.inventory[index].upNeighborID += 53910;
      if (index < 12)
        this.inventory.inventory[index].upNeighborID = this.ItemsToGrabMenu.actualInventory.Count - this.ItemsToGrabMenu.capacity / this.ItemsToGrabMenu.rows;
    }
    this.dropItemInvisibleButton.myID = -500;
    this.ItemsToGrabMenu.dropItemInvisibleButton.myID = -500;
    if (!Game1.options.SnappyMenus)
      return;
    this.populateClickableComponentList();
    this.setCurrentlySnappedComponentTo(53910);
    this.snapCursorToCurrentSnappedComponent();
  }

  /// <inheritdoc />
  public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
  {
    base.gameWindowSizeChanged(oldBounds, newBounds);
    int num = 64 /*0x40*/ * (this.ItemsToGrabMenu.capacity / this.ItemsToGrabMenu.rows);
    this.ItemsToGrabMenu = new InventoryMenu(Game1.uiViewport.Width / 2 - num / 2, this.yPositionOnScreen + 64 /*0x40*/, false, this.ItemsToGrabMenu.actualInventory, capacity: this.ItemsToGrabMenu.capacity, rows: this.ItemsToGrabMenu.rows);
  }

  /// <inheritdoc />
  public override void receiveLeftClick(int x, int y, bool playSound = true)
  {
    Item heldItem1 = this.heldItem;
    int num = heldItem1 != null ? heldItem1.Stack : -1;
    if (this.isWithinBounds(x, y))
    {
      base.receiveLeftClick(x, y, false);
      if (this.itemChangeBehavior == null && heldItem1 == null && this.heldItem != null && Game1.oldKBState.IsKeyDown(Keys.LeftShift))
        this.heldItem = this.ItemsToGrabMenu.tryToAddItem(this.heldItem, "Ship");
    }
    bool flag = true;
    if (this.ItemsToGrabMenu.isWithinBounds(x, y))
    {
      this.heldItem = this.ItemsToGrabMenu.leftClick(x, y, this.heldItem, false);
      if (this.heldItem != null && heldItem1 == null || this.heldItem != null && heldItem1 != null && !this.heldItem.Equals((object) heldItem1))
      {
        if (this.itemChangeBehavior != null)
          flag = this.itemChangeBehavior(this.heldItem, this.ItemsToGrabMenu.getInventoryPositionOfClick(x, y), heldItem1, this, true);
        if (flag)
          Game1.playSound("dwop");
      }
      if (this.heldItem == null && heldItem1 != null || this.heldItem != null && heldItem1 != null && !this.heldItem.Equals((object) heldItem1))
      {
        Item old = this.heldItem;
        if (this.heldItem == null && this.ItemsToGrabMenu.getItemAt(x, y) != null && num < this.ItemsToGrabMenu.getItemAt(x, y).Stack)
        {
          old = heldItem1.getOne();
          old.Stack = num;
        }
        if (this.itemChangeBehavior != null)
          flag = this.itemChangeBehavior(heldItem1, this.ItemsToGrabMenu.getInventoryPositionOfClick(x, y), old, this);
        if (flag)
          Game1.playSound("Ship");
      }
      Item heldItem2 = this.heldItem;
      // ISSUE: explicit non-virtual call
      if ((heldItem2 != null ? (__nonvirtual (heldItem2.IsRecipe) ? 1 : 0) : 0) != 0)
      {
        this.heldItem.LearnRecipe();
        this.poof = new TemporaryAnimatedSprite("TileSheets\\animations", new Rectangle(0, 320, 64 /*0x40*/, 64 /*0x40*/), 50f, 8, 0, new Vector2((float) (x - x % 64 /*0x40*/ + 16 /*0x10*/), (float) (y - y % 64 /*0x40*/ + 16 /*0x10*/)), false, false);
        Game1.playSound("newRecipe");
        this.heldItem = (Item) null;
      }
      else if (Game1.oldKBState.IsKeyDown(Keys.LeftShift) && Game1.player.addItemToInventoryBool(this.heldItem))
      {
        this.heldItem = (Item) null;
        if (this.itemChangeBehavior != null)
          flag = this.itemChangeBehavior(this.heldItem, this.ItemsToGrabMenu.getInventoryPositionOfClick(x, y), heldItem1, this, true);
        if (flag)
          Game1.playSound("coin");
      }
    }
    if (this.okButton.containsPoint(x, y) && this.readyToClose())
    {
      Game1.playSound("bigDeSelect");
      Game1.exitActiveMenu();
    }
    if (!this.trashCan.containsPoint(x, y) || this.heldItem == null || !this.heldItem.canBeTrashed())
      return;
    Utility.trashItem(this.heldItem);
    this.heldItem = (Item) null;
  }

  /// <inheritdoc />
  public override void receiveRightClick(int x, int y, bool playSound = true)
  {
    int stack = this.heldItem != null ? this.heldItem.Stack : 0;
    Item heldItem1 = this.heldItem;
    if (this.isWithinBounds(x, y))
    {
      base.receiveRightClick(x, y, true);
      if (this.itemChangeBehavior == null && heldItem1 == null && this.heldItem != null && Game1.oldKBState.IsKeyDown(Keys.LeftShift))
        this.heldItem = this.ItemsToGrabMenu.tryToAddItem(this.heldItem, "Ship");
    }
    if (!this.ItemsToGrabMenu.isWithinBounds(x, y))
      return;
    this.heldItem = this.ItemsToGrabMenu.rightClick(x, y, this.heldItem, false);
    if (this.heldItem != null && heldItem1 == null || this.heldItem != null && heldItem1 != null && !this.heldItem.Equals((object) heldItem1) || this.heldItem != null && heldItem1 != null && this.heldItem.Equals((object) heldItem1) && this.heldItem.Stack != stack)
    {
      StorageContainer.behaviorOnItemChange itemChangeBehavior = this.itemChangeBehavior;
      if (itemChangeBehavior != null)
      {
        int num = itemChangeBehavior(this.heldItem, this.ItemsToGrabMenu.getInventoryPositionOfClick(x, y), heldItem1, this, true) ? 1 : 0;
      }
      Game1.playSound("dwop");
    }
    if (this.heldItem == null && heldItem1 != null || this.heldItem != null && heldItem1 != null && !this.heldItem.Equals((object) heldItem1))
    {
      StorageContainer.behaviorOnItemChange itemChangeBehavior = this.itemChangeBehavior;
      if (itemChangeBehavior != null)
      {
        int num = itemChangeBehavior(heldItem1, this.ItemsToGrabMenu.getInventoryPositionOfClick(x, y), this.heldItem, this) ? 1 : 0;
      }
      Game1.playSound("Ship");
    }
    Item heldItem2 = this.heldItem;
    // ISSUE: explicit non-virtual call
    if ((heldItem2 != null ? (__nonvirtual (heldItem2.IsRecipe) ? 1 : 0) : 0) != 0)
    {
      this.heldItem.LearnRecipe();
      this.poof = new TemporaryAnimatedSprite("TileSheets\\animations", new Rectangle(0, 320, 64 /*0x40*/, 64 /*0x40*/), 50f, 8, 0, new Vector2((float) (x - x % 64 /*0x40*/ + 16 /*0x10*/), (float) (y - y % 64 /*0x40*/ + 16 /*0x10*/)), false, false);
      Game1.playSound("newRecipe");
      this.heldItem = (Item) null;
    }
    else
    {
      if (!Game1.oldKBState.IsKeyDown(Keys.LeftShift) || !Game1.player.addItemToInventoryBool(this.heldItem))
        return;
      this.heldItem = (Item) null;
      Game1.playSound("coin");
      StorageContainer.behaviorOnItemChange itemChangeBehavior = this.itemChangeBehavior;
      if (itemChangeBehavior == null)
        return;
      int num = itemChangeBehavior(this.heldItem, this.ItemsToGrabMenu.getInventoryPositionOfClick(x, y), heldItem1, this, true) ? 1 : 0;
    }
  }

  /// <inheritdoc />
  public override void update(GameTime time)
  {
    base.update(time);
    if (this.poof == null || !this.poof.update(time))
      return;
    this.poof = (TemporaryAnimatedSprite) null;
  }

  /// <inheritdoc />
  public override void performHoverAction(int x, int y)
  {
    base.performHoverAction(x, y);
    this.ItemsToGrabMenu.hover(x, y, this.heldItem);
  }

  /// <inheritdoc />
  public override void draw(SpriteBatch b)
  {
    b.Draw(Game1.fadeToBlackRect, new Rectangle(0, 0, Game1.uiViewport.Width, Game1.uiViewport.Height), Color.Black * 0.5f);
    this.draw(b, false, false);
    Game1.drawDialogueBox(this.ItemsToGrabMenu.xPositionOnScreen - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder, this.ItemsToGrabMenu.yPositionOnScreen - IClickableMenu.borderWidth - IClickableMenu.spaceToClearTopBorder, this.ItemsToGrabMenu.width + IClickableMenu.borderWidth * 2 + IClickableMenu.spaceToClearSideBorder * 2, this.ItemsToGrabMenu.height + IClickableMenu.spaceToClearTopBorder + IClickableMenu.borderWidth * 2, false, true);
    this.ItemsToGrabMenu.draw(b);
    this.poof?.draw(b, true);
    if (!this.hoverText.Equals(""))
      IClickableMenu.drawHoverText(b, this.hoverText, Game1.smallFont);
    this.heldItem?.drawInMenu(b, new Vector2((float) (Game1.getOldMouseX() + 16 /*0x10*/), (float) (Game1.getOldMouseY() + 16 /*0x10*/)), 1f);
    this.drawMouse(b);
    string descriptionTitle = this.ItemsToGrabMenu.descriptionTitle;
    if ((descriptionTitle != null ? (descriptionTitle.Length > 1 ? 1 : 0) : 0) == 0)
      return;
    IClickableMenu.drawHoverText(b, this.ItemsToGrabMenu.descriptionTitle, Game1.smallFont, 32 /*0x20*/ + (this.heldItem != null ? 16 /*0x10*/ : -21), 32 /*0x20*/ + (this.heldItem != null ? 16 /*0x10*/ : -21));
  }

  public delegate bool behaviorOnItemChange(
    Item i,
    int position,
    Item old,
    StorageContainer container,
    bool onRemoval = false);
}
