// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.TailoringMenu
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley.Extensions;
using StardewValley.GameData.Crafting;
using StardewValley.Objects;
using System;
using System.Collections.Generic;

#nullable disable
namespace StardewValley.Menus;

public class TailoringMenu : MenuWithInventory
{
  protected int _timeUntilCraft;
  public const int region_leftIngredient = 998;
  public const int region_rightIngredient = 997;
  public const int region_startButton = 996;
  public const int region_resultItem = 995;
  public ClickableTextureComponent needleSprite;
  public ClickableTextureComponent presserSprite;
  public ClickableTextureComponent craftResultDisplay;
  public Vector2 needlePosition;
  public Vector2 presserPosition;
  public Vector2 leftIngredientStartSpot;
  public Vector2 leftIngredientEndSpot;
  protected float _rightItemOffset;
  public ClickableTextureComponent leftIngredientSpot;
  public ClickableTextureComponent rightIngredientSpot;
  public ClickableTextureComponent blankLeftIngredientSpot;
  public ClickableTextureComponent blankRightIngredientSpot;
  public ClickableTextureComponent startTailoringButton;
  public const int region_shirt = 108;
  public const int region_pants = 109;
  public const int region_hat = 101;
  public List<ClickableComponent> equipmentIcons = new List<ClickableComponent>();
  public const int CRAFT_TIME = 1500;
  public Texture2D tailoringTextures;
  public List<TailorItemRecipe> _tailoringRecipes;
  private ICue _sewingSound;
  /// <summary>The slots in which each item can be placed.</summary>
  private readonly Dictionary<Item, TailoringMenu.TailorHighlight> ItemHighlightCache = new Dictionary<Item, TailoringMenu.TailorHighlight>();
  protected bool _shouldPrismaticDye;
  protected bool _isDyeCraft;
  protected bool _isMultipleResultCraft;
  protected string displayedDescription = "";
  protected TailoringMenu.CraftState _craftState;
  public Vector2 questionMarkOffset;

  public TailoringMenu()
    : base(okButton: true, trashCan: true, inventoryXOffset: 12, inventoryYOffset: 132)
  {
    Game1.playSound("bigSelect");
    if (this.yPositionOnScreen == IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder)
      this.movePosition(0, -IClickableMenu.spaceToClearTopBorder);
    this.inventory.highlightMethod = new InventoryMenu.highlightThisItem(this.HighlightItems);
    this.tailoringTextures = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\tailoring");
    this._tailoringRecipes = DataLoader.TailoringRecipes(Game1.temporaryContent);
    this._CreateButtons();
    if (this.trashCan != null)
      this.trashCan.myID = 106;
    if (this.okButton != null)
      this.okButton.leftNeighborID = 11;
    if (Game1.options.SnappyMenus)
    {
      this.populateClickableComponentList();
      this.snapToDefaultClickableComponent();
    }
    this._ValidateCraft();
  }

  protected void _CreateButtons()
  {
    ClickableTextureComponent textureComponent1 = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth / 2 + 4, this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + 8 + 192 /*0xC0*/, 96 /*0x60*/, 96 /*0x60*/), this.tailoringTextures, new Rectangle(0, 156, 24, 24), 4f);
    textureComponent1.myID = 998;
    textureComponent1.downNeighborID = -99998;
    textureComponent1.leftNeighborID = 109;
    textureComponent1.rightNeighborID = 996;
    textureComponent1.upNeighborID = 997;
    textureComponent1.item = this.leftIngredientSpot != null ? this.leftIngredientSpot.item : (Item) null;
    this.leftIngredientSpot = textureComponent1;
    this.leftIngredientStartSpot = new Vector2((float) this.leftIngredientSpot.bounds.X, (float) this.leftIngredientSpot.bounds.Y);
    this.leftIngredientEndSpot = this.leftIngredientStartSpot + new Vector2(256f, 0.0f);
    this.needleSprite = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth / 2 + 4 + 116, this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + 8 + 128 /*0x80*/, 96 /*0x60*/, 96 /*0x60*/), this.tailoringTextures, new Rectangle(64 /*0x40*/, 80 /*0x50*/, 16 /*0x10*/, 32 /*0x20*/), 4f);
    this.presserSprite = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth / 2 + 4 + 116, this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + 8 + 128 /*0x80*/, 96 /*0x60*/, 96 /*0x60*/), this.tailoringTextures, new Rectangle(48 /*0x30*/, 80 /*0x50*/, 16 /*0x10*/, 32 /*0x20*/), 4f);
    this.needlePosition = new Vector2((float) this.needleSprite.bounds.X, (float) this.needleSprite.bounds.Y);
    this.presserPosition = new Vector2((float) this.presserSprite.bounds.X, (float) this.presserSprite.bounds.Y);
    ClickableTextureComponent textureComponent2 = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth / 2 + 4 + 400, this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + 8, 96 /*0x60*/, 96 /*0x60*/), this.tailoringTextures, new Rectangle(0, 180, 24, 24), 4f);
    textureComponent2.myID = 997;
    textureComponent2.downNeighborID = 996;
    textureComponent2.leftNeighborID = 998;
    textureComponent2.rightNeighborID = -99998;
    textureComponent2.upNeighborID = -99998;
    textureComponent2.item = this.rightIngredientSpot != null ? this.rightIngredientSpot.item : (Item) null;
    textureComponent2.fullyImmutable = true;
    this.rightIngredientSpot = textureComponent2;
    this.blankRightIngredientSpot = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth / 2 + 4 + 400, this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + 8, 96 /*0x60*/, 96 /*0x60*/), this.tailoringTextures, new Rectangle(0, 128 /*0x80*/, 24, 24), 4f);
    this.blankLeftIngredientSpot = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth / 2 + 4, this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + 8 + 192 /*0xC0*/, 96 /*0x60*/, 96 /*0x60*/), this.tailoringTextures, new Rectangle(0, 128 /*0x80*/, 24, 24), 4f);
    ClickableTextureComponent textureComponent3 = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth / 2 + 4 + 448, this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + 8 + 128 /*0x80*/, 96 /*0x60*/, 96 /*0x60*/), this.tailoringTextures, new Rectangle(24, 80 /*0x50*/, 24, 24), 4f);
    textureComponent3.myID = 996;
    textureComponent3.downNeighborID = -99998;
    textureComponent3.leftNeighborID = 998;
    textureComponent3.rightNeighborID = 995;
    textureComponent3.upNeighborID = 997;
    textureComponent3.item = this.startTailoringButton != null ? this.startTailoringButton.item : (Item) null;
    textureComponent3.fullyImmutable = true;
    this.startTailoringButton = textureComponent3;
    List<ClickableComponent> inventory = this.inventory.inventory;
    // ISSUE: explicit non-virtual call
    if ((inventory != null ? (__nonvirtual (inventory.Count) >= 12 ? 1 : 0) : 0) != 0)
    {
      for (int index = 0; index < 12; ++index)
      {
        if (this.inventory.inventory[index] != null)
          this.inventory.inventory[index].upNeighborID = -99998;
      }
    }
    this.equipmentIcons = new List<ClickableComponent>()
    {
      new ClickableComponent(new Rectangle(0, 0, 64 /*0x40*/, 64 /*0x40*/), "Hat")
      {
        myID = 101,
        leftNeighborID = -99998,
        downNeighborID = -99998,
        upNeighborID = -99998,
        rightNeighborID = -99998
      },
      new ClickableComponent(new Rectangle(0, 0, 64 /*0x40*/, 64 /*0x40*/), "Shirt")
      {
        myID = 108,
        upNeighborID = -99998,
        downNeighborID = -99998,
        rightNeighborID = -99998,
        leftNeighborID = -99998
      },
      new ClickableComponent(new Rectangle(0, 0, 64 /*0x40*/, 64 /*0x40*/), "Pants")
      {
        myID = 109,
        upNeighborID = -99998,
        rightNeighborID = -99998,
        leftNeighborID = -99998,
        downNeighborID = -99998
      }
    };
    for (int index = 0; index < this.equipmentIcons.Count; ++index)
    {
      this.equipmentIcons[index].bounds.X = this.xPositionOnScreen - 64 /*0x40*/ + 9;
      this.equipmentIcons[index].bounds.Y = this.yPositionOnScreen + 192 /*0xC0*/ + index * 64 /*0x40*/;
    }
    ClickableTextureComponent textureComponent4 = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth / 2 + 4 + 660, this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + 8 + 232, 64 /*0x40*/, 64 /*0x40*/), this.tailoringTextures, new Rectangle(0, 208 /*0xD0*/, 16 /*0x10*/, 16 /*0x10*/), 4f);
    textureComponent4.myID = 995;
    textureComponent4.downNeighborID = -99998;
    textureComponent4.leftNeighborID = 996;
    textureComponent4.upNeighborID = 997;
    textureComponent4.item = this.craftResultDisplay?.item;
    this.craftResultDisplay = textureComponent4;
  }

  public override void snapToDefaultClickableComponent()
  {
    this.currentlySnappedComponent = this.getComponentWithID(0);
    this.snapCursorToCurrentSnappedComponent();
  }

  public bool IsBusy() => this._timeUntilCraft > 0;

  public override bool readyToClose()
  {
    return base.readyToClose() && this.heldItem == null && !this.IsBusy();
  }

  /// <summary>Get whether an item can be put into one of the empty tailoring slots, or (if all tailoring slots are empty) swapped with an equipment slot.</summary>
  /// <param name="i">The item to check.</param>
  public bool HighlightItems(Item i)
  {
    if (i == null)
      return false;
    if (!this.ItemHighlightCache.ContainsKey(i))
      this.BuildHighlightCache();
    return this.ItemHighlightCache[i].AnySlot;
  }

  public void BuildHighlightCache()
  {
    this.ItemHighlightCache.Clear();
    List<Item> objList = new List<Item>((IEnumerable<Item>) this.inventory.actualInventory);
    objList.Add((Item) Game1.player.pantsItem.Value);
    objList.Add((Item) Game1.player.shirtItem.Value);
    objList.Add((Item) Game1.player.hat.Value);
    Item left_item = this.leftIngredientSpot.item;
    Item right_item = this.rightIngredientSpot.item;
    bool flag1 = left_item == null;
    bool flag2 = right_item == null;
    foreach (Item obj in objList)
    {
      if (obj != null)
      {
        if (!flag1 && !flag2 || !this.IsValidCraftIngredient(obj))
          this.ItemHighlightCache[obj] = new TailoringMenu.TailorHighlight();
        else if (!this.IsValidCraftIngredient(obj))
          this.ItemHighlightCache[obj] = new TailoringMenu.TailorHighlight(false, false, obj is Hat || obj is Clothing);
        else if (flag1 != flag2)
        {
          this.ItemHighlightCache[obj] = new TailoringMenu.TailorHighlight(flag1 && this.IsValidCraft(obj, right_item), flag2 && this.IsValidCraft(left_item, obj), false);
        }
        else
        {
          bool leftSlot = false;
          bool rightSlot = false;
          if (obj is Boots)
          {
            leftSlot = true;
            rightSlot = true;
          }
          else if (obj is Clothing clothing && clothing.dyeable.Value)
            leftSlot = true;
          else if (obj.HasContextTag("color_prismatic") || TailoringMenu.GetDyeColor(obj).HasValue)
            rightSlot = true;
          foreach (TailorItemRecipe tailoringRecipe in this._tailoringRecipes)
          {
            if (!(leftSlot & rightSlot))
            {
              leftSlot = leftSlot || this.HasRequiredTags(obj, tailoringRecipe.FirstItemTags);
              rightSlot = rightSlot || this.HasRequiredTags(obj, tailoringRecipe.SecondItemTags);
            }
            else
              break;
          }
          this.ItemHighlightCache[obj] = new TailoringMenu.TailorHighlight(leftSlot, rightSlot, obj is Hat || obj is Clothing);
        }
      }
    }
  }

  private void _leftIngredientSpotClicked()
  {
    if (this.heldItem != null)
    {
      bool? leftSlot = this.ItemHighlightCache.GetValueOrDefault<Item, TailoringMenu.TailorHighlight>(this.heldItem)?.LeftSlot;
      if (leftSlot.HasValue && !leftSlot.GetValueOrDefault())
        return;
    }
    Item obj = this.leftIngredientSpot.item;
    if (this.heldItem != null && !this.IsValidCraftIngredient(this.heldItem))
      return;
    Game1.playSound("stoneStep");
    this.leftIngredientSpot.item = this.heldItem;
    this.heldItem = obj;
    this.ItemHighlightCache.Clear();
    this._ValidateCraft();
  }

  public bool IsValidCraftIngredient(Item item)
  {
    return item.HasContextTag("item_lucky_purple_shorts") || item.canBeTrashed();
  }

  private void _rightIngredientSpotClicked()
  {
    if (this.heldItem != null)
    {
      bool? rightSlot = this.ItemHighlightCache.GetValueOrDefault<Item, TailoringMenu.TailorHighlight>(this.heldItem)?.RightSlot;
      if (rightSlot.HasValue && !rightSlot.GetValueOrDefault())
        return;
    }
    Item obj = this.rightIngredientSpot.item;
    if (this.heldItem != null && !this.IsValidCraftIngredient(this.heldItem))
      return;
    Game1.playSound("stoneStep");
    this.rightIngredientSpot.item = this.heldItem;
    this.heldItem = obj;
    this.ItemHighlightCache.Clear();
    this._ValidateCraft();
  }

  /// <inheritdoc />
  public override void receiveKeyPress(Keys key)
  {
    if (key == Keys.Delete)
    {
      bool? nullable = this.heldItem?.canBeTrashed();
      if (!nullable.HasValue || !nullable.GetValueOrDefault())
        return;
      Utility.trashItem(this.heldItem);
      this.heldItem = (Item) null;
    }
    else
      base.receiveKeyPress(key);
  }

  /// <inheritdoc />
  public override void receiveLeftClick(int x, int y, bool playSound = true)
  {
    Item heldItem1 = this.heldItem;
    int num = Game1.player.IsEquippedItem(heldItem1) ? 1 : 0;
    base.receiveLeftClick(x, y, true);
    if (num != 0 && this.heldItem != heldItem1)
    {
      if (heldItem1 == Game1.player.hat.Value)
      {
        Game1.player.Equip<Hat>((Hat) null, Game1.player.hat);
        this.ItemHighlightCache.Clear();
      }
      else if (heldItem1 == Game1.player.shirtItem.Value)
      {
        Game1.player.Equip<Clothing>((Clothing) null, Game1.player.shirtItem);
        this.ItemHighlightCache.Clear();
      }
      else if (heldItem1 == Game1.player.pantsItem.Value)
      {
        Game1.player.Equip<Clothing>((Clothing) null, Game1.player.pantsItem);
        this.ItemHighlightCache.Clear();
      }
    }
    foreach (ClickableComponent equipmentIcon in this.equipmentIcons)
    {
      if (equipmentIcon.containsPoint(x, y))
      {
        switch (equipmentIcon.name)
        {
          case "Hat":
            Item obj1 = Utility.PerformSpecialItemPlaceReplacement(this.heldItem);
            if (this.heldItem == null)
            {
              if (!this.HighlightItems((Item) Game1.player.hat.Value))
                return;
              this.heldItem = Utility.PerformSpecialItemGrabReplacement((Item) Game1.player.hat.Value);
              Game1.playSound("dwop");
              if (!(this.heldItem is Hat))
                Game1.player.Equip<Hat>((Hat) null, Game1.player.hat);
              this.ItemHighlightCache.Clear();
              this._ValidateCraft();
              return;
            }
            if (!(obj1 is Hat newItem1))
              return;
            Item obj2 = Utility.PerformSpecialItemGrabReplacement((Item) Game1.player.hat.Value);
            if (obj2 == this.heldItem)
              obj2 = (Item) null;
            Game1.player.Equip<Hat>(newItem1, Game1.player.hat);
            this.heldItem = obj2;
            Game1.playSound("grassyStep");
            this.ItemHighlightCache.Clear();
            this._ValidateCraft();
            return;
          case "Shirt":
            Item obj3 = Utility.PerformSpecialItemPlaceReplacement(this.heldItem);
            if (this.heldItem == null)
            {
              if (!this.HighlightItems((Item) Game1.player.shirtItem.Value))
                return;
              this.heldItem = Utility.PerformSpecialItemGrabReplacement((Item) Game1.player.shirtItem.Value);
              Game1.playSound("dwop");
              if (!(this.heldItem is Clothing))
                Game1.player.Equip<Clothing>((Clothing) null, Game1.player.shirtItem);
              this.ItemHighlightCache.Clear();
              this._ValidateCraft();
              return;
            }
            if (!(obj3 is Clothing newItem2) || newItem2.clothesType.Value != Clothing.ClothesType.SHIRT)
              return;
            Item obj4 = Utility.PerformSpecialItemGrabReplacement((Item) Game1.player.shirtItem.Value);
            if (obj4 == this.heldItem)
              obj4 = (Item) null;
            Game1.player.Equip<Clothing>(newItem2, Game1.player.shirtItem);
            this.heldItem = obj4;
            Game1.playSound("sandyStep");
            this.ItemHighlightCache.Clear();
            this._ValidateCraft();
            return;
          case "Pants":
            Item obj5 = Utility.PerformSpecialItemPlaceReplacement(this.heldItem);
            if (this.heldItem == null)
            {
              if (!this.HighlightItems((Item) Game1.player.pantsItem.Value))
                return;
              this.heldItem = Utility.PerformSpecialItemGrabReplacement((Item) Game1.player.pantsItem.Value);
              if (!(this.heldItem is Clothing))
                Game1.player.Equip<Clothing>((Clothing) null, Game1.player.pantsItem);
              Game1.playSound("dwop");
              this.ItemHighlightCache.Clear();
              this._ValidateCraft();
              return;
            }
            if (!(obj5 is Clothing newItem3) || newItem3.clothesType.Value != Clothing.ClothesType.PANTS)
              return;
            Item obj6 = Utility.PerformSpecialItemGrabReplacement((Item) Game1.player.pantsItem.Value);
            if (obj6 == this.heldItem)
              obj6 = (Item) null;
            Game1.player.Equip<Clothing>(newItem3, Game1.player.pantsItem);
            this.heldItem = obj6;
            Game1.playSound("sandyStep");
            this.ItemHighlightCache.Clear();
            this._ValidateCraft();
            return;
          default:
            return;
        }
      }
    }
    KeyboardState keyboardState = Game1.GetKeyboardState();
    if (keyboardState.IsKeyDown(Keys.LeftShift) && heldItem1 != this.heldItem && this.heldItem != null)
    {
      if (this.heldItem.QualifiedItemId == "(O)428" || this.heldItem is Clothing heldItem2 && heldItem2.dyeable.Value)
        this._leftIngredientSpotClicked();
      else
        this._rightIngredientSpotClicked();
    }
    if (this.IsBusy())
      return;
    if (this.leftIngredientSpot.containsPoint(x, y))
    {
      this._leftIngredientSpotClicked();
      keyboardState = Game1.GetKeyboardState();
      if (keyboardState.IsKeyDown(Keys.LeftShift) && this.heldItem != null)
      {
        if (Game1.player.IsEquippedItem(this.heldItem))
          this.heldItem = (Item) null;
        else
          this.heldItem = this.inventory.tryToAddItem(this.heldItem, "");
      }
    }
    else if (this.rightIngredientSpot.containsPoint(x, y))
    {
      this._rightIngredientSpotClicked();
      keyboardState = Game1.GetKeyboardState();
      if (keyboardState.IsKeyDown(Keys.LeftShift) && this.heldItem != null)
      {
        if (Game1.player.IsEquippedItem(this.heldItem))
          this.heldItem = (Item) null;
        else
          this.heldItem = this.inventory.tryToAddItem(this.heldItem, "");
      }
    }
    else if (this.startTailoringButton.containsPoint(x, y))
    {
      if (this.heldItem == null)
      {
        bool flag = false;
        if (!this.CanFitCraftedItem())
        {
          Game1.playSound("cancel");
          Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Crop.cs.588"));
          this._timeUntilCraft = 0;
          flag = true;
        }
        if (!flag && this.IsValidCraft(this.leftIngredientSpot.item, this.rightIngredientSpot.item))
        {
          Game1.playSound("bigSelect");
          Game1.playSound("sewing_loop", out this._sewingSound);
          this.startTailoringButton.scale = this.startTailoringButton.baseScale;
          this._timeUntilCraft = 1500;
          this._UpdateDescriptionText();
        }
        else
          Game1.playSound("sell");
      }
      else
        Game1.playSound("sell");
    }
    if (this.heldItem == null || this.isWithinBounds(x, y) || !this.heldItem.canBeTrashed())
      return;
    if (Game1.player.IsEquippedItem(this.heldItem))
    {
      if (this.heldItem == Game1.player.hat.Value)
        Game1.player.Equip<Hat>((Hat) null, Game1.player.hat);
      else if (this.heldItem == Game1.player.shirtItem.Value)
        Game1.player.Equip<Clothing>((Clothing) null, Game1.player.shirtItem);
      else if (this.heldItem == Game1.player.pantsItem.Value)
        Game1.player.Equip<Clothing>((Clothing) null, Game1.player.pantsItem);
    }
    Game1.playSound("throwDownITem");
    Game1.createItemDebris(this.heldItem, Game1.player.getStandingPosition(), Game1.player.FacingDirection);
    this.heldItem = (Item) null;
  }

  protected void _ValidateCraft()
  {
    Item left_item = this.leftIngredientSpot.item;
    Item right_item = this.rightIngredientSpot.item;
    if (left_item == null || right_item == null)
      this._craftState = TailoringMenu.CraftState.MissingIngredients;
    else if (left_item is Clothing clothing && !clothing.dyeable.Value)
      this._craftState = TailoringMenu.CraftState.NotDyeable;
    else if (this.IsValidCraft(left_item, right_item))
    {
      this._craftState = TailoringMenu.CraftState.Valid;
      bool shouldPrismaticDye = this._shouldPrismaticDye;
      Item one = left_item.getOne();
      this._isMultipleResultCraft = this.IsMultipleResultCraft(left_item, right_item);
      this.craftResultDisplay.item = this.CraftItem(one, right_item.getOne());
      this._isDyeCraft = this.craftResultDisplay.item == one;
      this._shouldPrismaticDye = shouldPrismaticDye;
    }
    else
      this._craftState = TailoringMenu.CraftState.InvalidRecipe;
    this._UpdateDescriptionText();
  }

  protected void _UpdateDescriptionText()
  {
    if (this.IsBusy())
    {
      this.displayedDescription = Game1.content.LoadString("Strings\\UI:Tailor_Busy");
    }
    else
    {
      switch (this._craftState)
      {
        case TailoringMenu.CraftState.MissingIngredients:
          this.displayedDescription = Game1.content.LoadString("Strings\\UI:Tailor_MissingIngredients");
          break;
        case TailoringMenu.CraftState.Valid:
          this.displayedDescription = !this.CanFitCraftedItem() ? Game1.content.LoadString("Strings\\StringsFromCSFiles:Crop.cs.588") : Game1.content.LoadString("Strings\\UI:Tailor_Valid");
          break;
        case TailoringMenu.CraftState.InvalidRecipe:
          this.displayedDescription = Game1.content.LoadString("Strings\\UI:Tailor_InvalidRecipe");
          break;
        case TailoringMenu.CraftState.NotDyeable:
          this.displayedDescription = Game1.content.LoadString("Strings\\UI:Tailor_NotDyeable");
          break;
        default:
          this.displayedDescription = "";
          break;
      }
    }
  }

  public static Color? GetDyeColor(Item dye_object)
  {
    if (dye_object == null)
      return new Color?();
    if (dye_object.QualifiedItemId == "(O)74")
      return new Color?(Color.White);
    return dye_object is ColoredObject coloredObject ? new Color?(coloredObject.color.Value) : ItemContextTagManager.GetColorFromTags(dye_object);
  }

  public bool DyeItems(Clothing clothing, Item dye_object, float dye_strength_override = -1f)
  {
    if (dye_object.QualifiedItemId == "(O)74")
    {
      clothing.Dye(Color.White, 1f);
      clothing.isPrismatic.Set(true);
      return true;
    }
    Color? dyeColor = TailoringMenu.GetDyeColor(dye_object);
    if (!dyeColor.HasValue)
      return false;
    float strength = 0.25f;
    if (dye_object.HasContextTag("dye_medium"))
      strength = 0.5f;
    if (dye_object.HasContextTag("dye_strong"))
      strength = 1f;
    if ((double) dye_strength_override >= 0.0)
      strength = dye_strength_override;
    clothing.Dye(dyeColor.Value, strength);
    if (clothing == Game1.player.shirtItem.Value || clothing == Game1.player.pantsItem.Value)
      Game1.player.FarmerRenderer.MarkSpriteDirty();
    return true;
  }

  /// <summary>Get the recipe which accepts the given items.</summary>
  /// <param name="leftItem">The item in the left slot (usually cloth).</param>
  /// <param name="rightItem">The item in the right slot.</param>
  /// <returns>Returns the matching recipe, or <c>null</c> if none was found.</returns>
  public TailorItemRecipe GetRecipeForItems(Item leftItem, Item rightItem)
  {
    if (leftItem != null && rightItem != null)
    {
      foreach (TailorItemRecipe tailoringRecipe in this._tailoringRecipes)
      {
        if (this.HasRequiredTags(leftItem, tailoringRecipe.FirstItemTags) && this.HasRequiredTags(rightItem, tailoringRecipe.SecondItemTags))
          return tailoringRecipe;
      }
    }
    return (TailorItemRecipe) null;
  }

  /// <summary>Get whether an item matches every given tag.</summary>
  /// <param name="item">The item to check.</param>
  /// <param name="requiredTags">The context tags which must all match the item.</param>
  private bool HasRequiredTags(Item item, List<string> requiredTags)
  {
    // ISSUE: explicit non-virtual call
    if (item == null || requiredTags == null || __nonvirtual (requiredTags.Count) <= 0)
      return false;
    foreach (string requiredTag in requiredTags)
    {
      if (!item.HasContextTag(requiredTag))
        return false;
    }
    return true;
  }

  public bool IsValidCraft(Item left_item, Item right_item)
  {
    if (left_item == null || right_item == null)
      return false;
    switch (left_item)
    {
      case Boots _ when right_item is Boots:
        return true;
      case Clothing clothing when clothing.dyeable.Value:
        if (right_item.HasContextTag("color_prismatic") || TailoringMenu.GetDyeColor(right_item).HasValue)
          return true;
        break;
    }
    return this.GetRecipeForItems(left_item, right_item) != null;
  }

  public bool IsMultipleResultCraft(Item left_item, Item right_item)
  {
    TailorItemRecipe recipeForItems = this.GetRecipeForItems(left_item, right_item);
    if (recipeForItems == null)
      return false;
    int? count = recipeForItems.CraftedItemIds?.Count;
    int num = 0;
    return count.GetValueOrDefault() > num & count.HasValue;
  }

  public Item CraftItem(Item left_item, Item right_item)
  {
    if (left_item == null || right_item == null)
      return (Item) null;
    switch (left_item)
    {
      case Boots boots when right_item is Boots applied_boots:
        boots.applyStats(applied_boots);
        return (Item) boots;
      case Clothing clothing1 when clothing1.dyeable.Value:
        if (right_item.HasContextTag("color_prismatic"))
        {
          this._shouldPrismaticDye = true;
          return (Item) clothing1;
        }
        if (this.DyeItems(clothing1, right_item))
          return (Item) clothing1;
        break;
    }
    TailorItemRecipe recipeForItems = this.GetRecipeForItems(left_item, right_item);
    if (recipeForItems == null)
      return (Item) null;
    string id;
    if (recipeForItems.CraftedItemIdFeminine != null && !Game1.player.IsMale)
    {
      id = recipeForItems.CraftedItemIdFeminine;
    }
    else
    {
      List<string> craftedItemIds = recipeForItems.CraftedItemIds;
      // ISSUE: explicit non-virtual call
      id = (craftedItemIds != null ? (__nonvirtual (craftedItemIds.Count) > 0 ? 1 : 0) : 0) == 0 ? recipeForItems.CraftedItemId : Game1.random.ChooseFrom<string>((IList<string>) recipeForItems.CraftedItemIds);
    }
    Item obj = ItemRegistry.Create(TailoringMenu.ConvertLegacyItemId(id));
    if (obj is Clothing clothing2)
      this.DyeItems(clothing2, right_item, 1f);
    if (!(obj is StardewValley.Object object1) || (!(left_item is StardewValley.Object object2) || !object2.questItem.Value) && (!(right_item is StardewValley.Object object3) || !object3.questItem.Value))
      return obj;
    object1.questItem.Value = true;
    return obj;
  }

  /// <summary>Get an item ID for a legacy output from Stardew Valley 1.5.5 and earlier.</summary>
  /// <param name="id">The legacy item ID.</param>
  public static string ConvertLegacyItemId(string id)
  {
    int result;
    if (!int.TryParse(id, out result))
      return id;
    if (result < 0)
      return "(O)" + (-result).ToString();
    if (result >= 2000 && result < 3000)
      return "(H)" + (result - 2000).ToString();
    return result >= 1000 ? "(S)" + result.ToString() : "(P)" + result.ToString();
  }

  public void SpendRightItem()
  {
    this.rightIngredientSpot.item = this.rightIngredientSpot.item?.ConsumeStack(1);
  }

  public void SpendLeftItem()
  {
    this.leftIngredientSpot.item = this.leftIngredientSpot.item?.ConsumeStack(1);
  }

  /// <inheritdoc />
  public override void receiveRightClick(int x, int y, bool playSound = true)
  {
    if (this.IsBusy())
      return;
    base.receiveRightClick(x, y, true);
  }

  /// <inheritdoc />
  public override void performHoverAction(int x, int y)
  {
    if (this.IsBusy())
      return;
    this.hoveredItem = (Item) null;
    base.performHoverAction(x, y);
    this.hoverText = "";
    for (int index = 0; index < this.equipmentIcons.Count; ++index)
    {
      if (this.equipmentIcons[index].containsPoint(x, y))
      {
        switch (this.equipmentIcons[index].name)
        {
          case "Shirt":
            this.hoveredItem = (Item) Game1.player.shirtItem.Value;
            continue;
          case "Hat":
            this.hoveredItem = (Item) Game1.player.hat.Value;
            continue;
          case "Pants":
            this.hoveredItem = (Item) Game1.player.pantsItem.Value;
            continue;
          default:
            continue;
        }
      }
    }
    if (this.craftResultDisplay.visible && this.craftResultDisplay.containsPoint(x, y) && this.craftResultDisplay.item != null)
    {
      if (this._isDyeCraft || Game1.player.HasTailoredThisItem(this.craftResultDisplay.item))
        this.hoveredItem = this.craftResultDisplay.item;
      else
        this.hoverText = Game1.content.LoadString("Strings\\UI:Tailor_MakeResultUnknown");
    }
    if (this.leftIngredientSpot.containsPoint(x, y))
    {
      if (this.leftIngredientSpot.item != null)
        this.hoveredItem = this.leftIngredientSpot.item;
      else
        this.hoverText = Game1.content.LoadString("Strings\\UI:Tailor_Feed");
    }
    if (this.rightIngredientSpot.containsPoint(x, y) && this.rightIngredientSpot.item == null)
      this.hoverText = Game1.content.LoadString("Strings\\UI:Tailor_Spool");
    this.rightIngredientSpot.tryHover(x, y);
    this.leftIngredientSpot.tryHover(x, y);
    if (this._craftState == TailoringMenu.CraftState.Valid && this.CanFitCraftedItem())
      this.startTailoringButton.tryHover(x, y, 0.33f);
    else
      this.startTailoringButton.tryHover(-999, -999);
  }

  public bool CanFitCraftedItem()
  {
    return this.craftResultDisplay.item == null || Utility.canItemBeAddedToThisInventoryList(this.craftResultDisplay.item, this.inventory.actualInventory);
  }

  /// <inheritdoc />
  public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
  {
    base.gameWindowSizeChanged(oldBounds, newBounds);
    int yPosition = this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + IClickableMenu.borderWidth + 192 /*0xC0*/ - 16 /*0x10*/ + 128 /*0x80*/ + 4;
    this.inventory = new InventoryMenu(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth / 2 + 12, yPosition, false, highlightMethod: this.inventory.highlightMethod);
    this._CreateButtons();
  }

  public override void emergencyShutDown()
  {
    this._OnCloseMenu();
    base.emergencyShutDown();
  }

  /// <inheritdoc />
  public override void update(GameTime time)
  {
    base.update(time);
    this.descriptionText = this.displayedDescription;
    this.questionMarkOffset.X = (float) (Math.Sin(time.TotalGameTime.TotalSeconds * 2.5) * 4.0);
    this.questionMarkOffset.Y = (float) (Math.Cos(time.TotalGameTime.TotalSeconds * 5.0) * -4.0);
    bool flag = this.CanFitCraftedItem();
    this.startTailoringButton.sourceRect.Y = this._craftState == TailoringMenu.CraftState.Valid & flag ? 104 : 80 /*0x50*/;
    this.craftResultDisplay.visible = ((this._craftState != TailoringMenu.CraftState.Valid ? 0 : (!this.IsBusy() ? 1 : 0)) & (flag ? 1 : 0)) != 0;
    if (this._timeUntilCraft > 0)
    {
      this.startTailoringButton.tryHover(this.startTailoringButton.bounds.Center.X, this.startTailoringButton.bounds.Center.Y, 0.33f);
      this.leftIngredientSpot.bounds.X = (int) Utility.Lerp(this.leftIngredientEndSpot.X, this.leftIngredientStartSpot.X, (float) this._timeUntilCraft / 1500f);
      this.leftIngredientSpot.bounds.Y = (int) Utility.Lerp(this.leftIngredientEndSpot.Y, this.leftIngredientStartSpot.Y, (float) this._timeUntilCraft / 1500f);
      this._timeUntilCraft -= time.ElapsedGameTime.Milliseconds;
      this.needleSprite.bounds.Location = new Point((int) this.needlePosition.X, (int) ((double) this.needlePosition.Y - 2.0 * ((double) this._timeUntilCraft % 25.0) / 25.0 * 4.0));
      this.presserSprite.bounds.Location = new Point((int) this.presserPosition.X, (int) ((double) this.presserPosition.Y - 1.0 * ((double) this._timeUntilCraft % 50.0) / 50.0 * 4.0));
      this._rightItemOffset = (float) Math.Sin(time.TotalGameTime.TotalMilliseconds * 2.0 * Math.PI / 180.0) * 2f;
      if (this._timeUntilCraft > 0)
        return;
      TailorItemRecipe recipeForItems = this.GetRecipeForItems(this.leftIngredientSpot.item, this.rightIngredientSpot.item);
      this._shouldPrismaticDye = false;
      Item i = this.CraftItem(this.leftIngredientSpot.item, this.rightIngredientSpot.item);
      if (this._sewingSound != null && this._sewingSound.IsPlaying)
        this._sewingSound.Stop(AudioStopOptions.Immediate);
      if (!Utility.canItemBeAddedToThisInventoryList(i, this.inventory.actualInventory))
      {
        Game1.playSound("cancel");
        Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Crop.cs.588"));
        this._timeUntilCraft = 0;
        return;
      }
      if (this.leftIngredientSpot.item == i)
        this.leftIngredientSpot.item = (Item) null;
      else
        this.SpendLeftItem();
      if ((recipeForItems == null || recipeForItems.SpendRightItem) && (this.readyToClose() || !this._shouldPrismaticDye))
        this.SpendRightItem();
      if (recipeForItems != null)
        Game1.player.MarkItemAsTailored(i);
      Game1.playSound("coin");
      this.heldItem = i;
      this._timeUntilCraft = 0;
      this._ValidateCraft();
      if (this._shouldPrismaticDye)
      {
        Item heldItem = this.heldItem;
        this.heldItem = (Item) null;
        if (this.readyToClose())
        {
          this.exitThisMenuNoSound();
          Game1.activeClickableMenu = (IClickableMenu) new CharacterCustomization(i as Clothing);
          return;
        }
        this.heldItem = heldItem;
      }
    }
    this._rightItemOffset = 0.0f;
    this.leftIngredientSpot.bounds.X = (int) this.leftIngredientStartSpot.X;
    this.leftIngredientSpot.bounds.Y = (int) this.leftIngredientStartSpot.Y;
    this.needleSprite.bounds.Location = new Point((int) this.needlePosition.X, (int) this.needlePosition.Y);
    this.presserSprite.bounds.Location = new Point((int) this.presserPosition.X, (int) this.presserPosition.Y);
  }

  /// <inheritdoc />
  public override void draw(SpriteBatch b)
  {
    if (!Game1.options.showClearBackgrounds)
      b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.6f);
    b.Draw(this.tailoringTextures, new Vector2((float) this.xPositionOnScreen + 96f, (float) (this.yPositionOnScreen - 64 /*0x40*/)), new Rectangle?(new Rectangle(101, 80 /*0x50*/, 41, 36)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.FlipHorizontally, 0.87f);
    b.Draw(this.tailoringTextures, new Vector2((float) this.xPositionOnScreen + 352f, (float) (this.yPositionOnScreen - 64 /*0x40*/)), new Rectangle?(new Rectangle(101, 80 /*0x50*/, 41, 36)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.87f);
    b.Draw(this.tailoringTextures, new Vector2((float) this.xPositionOnScreen + 608f, (float) (this.yPositionOnScreen - 64 /*0x40*/)), new Rectangle?(new Rectangle(101, 80 /*0x50*/, 41, 36)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.87f);
    b.Draw(this.tailoringTextures, new Vector2((float) this.xPositionOnScreen + 256f, (float) this.yPositionOnScreen), new Rectangle?(new Rectangle(79, 97, 22, 20)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.87f);
    b.Draw(this.tailoringTextures, new Vector2((float) this.xPositionOnScreen + 512f, (float) this.yPositionOnScreen), new Rectangle?(new Rectangle(79, 97, 22, 20)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.87f);
    b.Draw(this.tailoringTextures, new Vector2((float) this.xPositionOnScreen + 32f, (float) (this.yPositionOnScreen + 44)), new Rectangle?(new Rectangle(81, 81, 16 /*0x10*/, 9)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.87f);
    b.Draw(this.tailoringTextures, new Vector2((float) this.xPositionOnScreen + 768f, (float) (this.yPositionOnScreen + 44)), new Rectangle?(new Rectangle(81, 81, 16 /*0x10*/, 9)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.87f);
    Game1.DrawBox(this.xPositionOnScreen - 64 /*0x40*/, this.yPositionOnScreen + 128 /*0x80*/, 128 /*0x80*/, 265, new Color?(new Color(50, 160 /*0xA0*/, (int) byte.MaxValue)));
    Game1.player.FarmerRenderer.drawMiniPortrat(b, new Vector2((float) (this.xPositionOnScreen - 64 /*0x40*/) + 9.6f, (float) (this.yPositionOnScreen + 128 /*0x80*/)), 0.87f, 4f, 2, Game1.player);
    this.draw(b, true, true, 50, 160 /*0xA0*/, (int) byte.MaxValue);
    b.Draw(this.tailoringTextures, new Vector2((float) (this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth / 2 - 4), (float) (this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder)), new Rectangle?(new Rectangle(0, 0, 142, 80 /*0x50*/)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.87f);
    this.startTailoringButton.draw(b, Color.White, 0.96f);
    this.startTailoringButton.drawItem(b, 16 /*0x10*/, 16 /*0x10*/);
    this.presserSprite.draw(b, Color.White, 0.99f);
    this.needleSprite.draw(b, Color.White, 0.97f);
    Point point = new Point(0, 0);
    if (!this.IsBusy())
    {
      Color color;
      if (this.heldItem != null)
      {
        bool? leftSlot = this.ItemHighlightCache.GetValueOrDefault<Item, TailoringMenu.TailorHighlight>(this.heldItem)?.LeftSlot;
        if (!leftSlot.HasValue || !leftSlot.GetValueOrDefault())
        {
          color = Color.White * 0.5f;
          goto label_7;
        }
      }
      color = Color.White;
label_7:
      Color c = color;
      if (this.leftIngredientSpot.item != null)
        this.blankLeftIngredientSpot.draw(b, c, 0.87f);
      else
        this.leftIngredientSpot.draw(b, c, 0.87f, (int) Game1.currentGameTime.TotalGameTime.TotalMilliseconds % 1000 / 200);
    }
    else
    {
      point.X = Game1.random.Next(-1, 2);
      point.Y = Game1.random.Next(-1, 2);
    }
    this.leftIngredientSpot.drawItem(b, (4 + point.X) * 4, (4 + point.Y) * 4);
    if (this.craftResultDisplay.visible)
    {
      string text = Game1.content.LoadString("Strings\\UI:Tailor_MakeResult");
      Vector2 position = new Vector2((float) this.craftResultDisplay.bounds.Center.X - Game1.smallFont.MeasureString(text).X / 2f, (float) this.craftResultDisplay.bounds.Top - Game1.smallFont.MeasureString(text).Y);
      Utility.drawTextWithColoredShadow(b, text, Game1.smallFont, position, Game1.textColor * 0.75f, Color.Black * 0.2f);
      this.craftResultDisplay.draw(b);
      if (this.craftResultDisplay.item != null)
      {
        if (this._isMultipleResultCraft)
        {
          Rectangle bounds = this.craftResultDisplay.bounds;
          bounds.X += 6;
          bounds.Y -= 8 + (int) this.questionMarkOffset.Y;
          b.Draw(this.tailoringTextures, bounds, new Rectangle?(new Rectangle(112 /*0x70*/, 208 /*0xD0*/, 16 /*0x10*/, 16 /*0x10*/)), Color.White);
        }
        else if (this._isDyeCraft || Game1.player.HasTailoredThisItem(this.craftResultDisplay.item))
        {
          this.craftResultDisplay.drawItem(b);
        }
        else
        {
          switch (this.craftResultDisplay.item)
          {
            case Hat _:
              b.Draw(this.tailoringTextures, this.craftResultDisplay.bounds, new Rectangle?(new Rectangle(96 /*0x60*/, 208 /*0xD0*/, 16 /*0x10*/, 16 /*0x10*/)), Color.White);
              break;
            case Clothing clothing:
              switch (clothing.clothesType.Value)
              {
                case Clothing.ClothesType.SHIRT:
                  b.Draw(this.tailoringTextures, this.craftResultDisplay.bounds, new Rectangle?(new Rectangle(80 /*0x50*/, 208 /*0xD0*/, 16 /*0x10*/, 16 /*0x10*/)), Color.White);
                  break;
                case Clothing.ClothesType.PANTS:
                  b.Draw(this.tailoringTextures, this.craftResultDisplay.bounds, new Rectangle?(new Rectangle(64 /*0x40*/, 208 /*0xD0*/, 16 /*0x10*/, 16 /*0x10*/)), Color.White);
                  break;
              }
              break;
            case StardewValley.Object @object:
              if (@object.QualifiedItemId == "(O)71")
              {
                b.Draw(this.tailoringTextures, this.craftResultDisplay.bounds, new Rectangle?(new Rectangle(64 /*0x40*/, 208 /*0xD0*/, 16 /*0x10*/, 16 /*0x10*/)), Color.White);
                break;
              }
              break;
          }
          Rectangle bounds = this.craftResultDisplay.bounds;
          bounds.X += 24;
          bounds.Y += 12 + (int) this.questionMarkOffset.Y;
          b.Draw(this.tailoringTextures, bounds, new Rectangle?(new Rectangle(112 /*0x70*/, 208 /*0xD0*/, 16 /*0x10*/, 16 /*0x10*/)), Color.White);
        }
      }
    }
    foreach (ClickableComponent equipmentIcon in this.equipmentIcons)
    {
      switch (equipmentIcon.name)
      {
        case "Hat":
          if (Game1.player.hat.Value != null)
          {
            b.Draw(this.tailoringTextures, equipmentIcon.bounds, new Rectangle?(new Rectangle(0, 208 /*0xD0*/, 16 /*0x10*/, 16 /*0x10*/)), Color.White);
            float transparency = !this.HighlightItems((Item) Game1.player.hat.Value) || Game1.player.hat.Value == this.heldItem || this.heldItem != null && !(this.heldItem is Hat) ? 0.5f : 1f;
            Game1.player.hat.Value.drawInMenu(b, new Vector2((float) equipmentIcon.bounds.X, (float) equipmentIcon.bounds.Y), equipmentIcon.scale, transparency, 0.866f, StackDrawType.Hide);
            continue;
          }
          b.Draw(this.tailoringTextures, equipmentIcon.bounds, new Rectangle?(new Rectangle(48 /*0x30*/, 208 /*0xD0*/, 16 /*0x10*/, 16 /*0x10*/)), Color.White);
          continue;
        case "Shirt":
          if (Game1.player.shirtItem.Value != null)
          {
            b.Draw(this.tailoringTextures, equipmentIcon.bounds, new Rectangle?(new Rectangle(0, 208 /*0xD0*/, 16 /*0x10*/, 16 /*0x10*/)), Color.White);
            float transparency = !this.HighlightItems((Item) Game1.player.shirtItem.Value) || Game1.player.shirtItem.Value == this.heldItem || this.heldItem != null && (this.heldItem is Clothing heldItem ? (heldItem.clothesType.Value != 0 ? 1 : 0) : 1) != 0 ? 0.5f : 1f;
            Game1.player.shirtItem.Value.drawInMenu(b, new Vector2((float) equipmentIcon.bounds.X, (float) equipmentIcon.bounds.Y), equipmentIcon.scale, transparency, 0.866f);
            continue;
          }
          b.Draw(this.tailoringTextures, equipmentIcon.bounds, new Rectangle?(new Rectangle(32 /*0x20*/, 208 /*0xD0*/, 16 /*0x10*/, 16 /*0x10*/)), Color.White);
          continue;
        case "Pants":
          if (Game1.player.pantsItem.Value != null)
          {
            b.Draw(this.tailoringTextures, equipmentIcon.bounds, new Rectangle?(new Rectangle(0, 208 /*0xD0*/, 16 /*0x10*/, 16 /*0x10*/)), Color.White);
            float transparency = !this.HighlightItems((Item) Game1.player.pantsItem.Value) || Game1.player.pantsItem.Value == this.heldItem || this.heldItem != null && (this.heldItem is Clothing heldItem ? (heldItem.clothesType.Value != Clothing.ClothesType.PANTS ? 1 : 0) : 1) != 0 ? 0.5f : 1f;
            Game1.player.pantsItem.Value.drawInMenu(b, new Vector2((float) equipmentIcon.bounds.X, (float) equipmentIcon.bounds.Y), equipmentIcon.scale, transparency, 0.866f);
            continue;
          }
          b.Draw(this.tailoringTextures, equipmentIcon.bounds, new Rectangle?(new Rectangle(16 /*0x10*/, 208 /*0xD0*/, 16 /*0x10*/, 16 /*0x10*/)), Color.White);
          continue;
        default:
          continue;
      }
    }
    if (!this.IsBusy())
    {
      Color color;
      if (this.heldItem != null)
      {
        bool? rightSlot = this.ItemHighlightCache.GetValueOrDefault<Item, TailoringMenu.TailorHighlight>(this.heldItem)?.RightSlot;
        if (!rightSlot.HasValue || !rightSlot.GetValueOrDefault())
        {
          color = Color.White * 0.5f;
          goto label_45;
        }
      }
      color = Color.White;
label_45:
      Color c = color;
      if (this.rightIngredientSpot.item != null)
        this.blankRightIngredientSpot.draw(b, c, 0.87f);
      else
        this.rightIngredientSpot.draw(b, c, 0.87f, (int) Game1.currentGameTime.TotalGameTime.TotalMilliseconds % 1000 / 200);
    }
    this.rightIngredientSpot.drawItem(b, 16 /*0x10*/, (4 + (int) this._rightItemOffset) * 4);
    if (!this.hoverText.Equals(""))
      IClickableMenu.drawHoverText(b, this.hoverText, Game1.smallFont, this.heldItem != null ? 32 /*0x20*/ : 0, this.heldItem != null ? 32 /*0x20*/ : 0);
    else if (this.hoveredItem != null)
      IClickableMenu.drawToolTip(b, this.hoveredItem.getDescription(), this.hoveredItem.DisplayName, this.hoveredItem, this.heldItem != null);
    this.heldItem?.drawInMenu(b, new Vector2((float) (Game1.getOldMouseX() + 8), (float) (Game1.getOldMouseY() + 8)), 1f);
    if (Game1.options.hardwareCursor)
      return;
    this.drawMouse(b);
  }

  /// <inheritdoc />
  protected override void cleanupBeforeExit() => this._OnCloseMenu();

  protected void _OnCloseMenu()
  {
    if (!Game1.player.IsEquippedItem(this.heldItem))
      Utility.CollectOrDrop(this.heldItem);
    if (!Game1.player.IsEquippedItem(this.leftIngredientSpot.item))
      Utility.CollectOrDrop(this.leftIngredientSpot.item);
    if (!Game1.player.IsEquippedItem(this.rightIngredientSpot.item))
      Utility.CollectOrDrop(this.rightIngredientSpot.item);
    if (!Game1.player.IsEquippedItem(this.startTailoringButton.item))
      Utility.CollectOrDrop(this.startTailoringButton.item);
    this.heldItem = (Item) null;
    this.leftIngredientSpot.item = (Item) null;
    this.rightIngredientSpot.item = (Item) null;
    this.startTailoringButton.item = (Item) null;
  }

  protected enum CraftState
  {
    MissingIngredients,
    Valid,
    InvalidRecipe,
    NotDyeable,
  }

  /// <summary>The slots which can currently accept an item.</summary>
  public class TailorHighlight
  {
    /// <summary>Whether the item can be placed in the left slot.</summary>
    public readonly bool LeftSlot;
    /// <summary>Whether the item can be placed in the right slot.</summary>
    public readonly bool RightSlot;
    /// <summary>Whether the item can be placed in an equipment slot.</summary>
    public readonly bool EquipmentSlot;
    /// <summary>Whether the item can be placed in any of these slots.</summary>
    public readonly bool AnySlot;

    /// <summary>Construct an instance.</summary>
    public TailorHighlight()
    {
    }

    /// <summary>Construct an instance.</summary>
    /// <param name="leftSlot"><inheritdoc cref="F:StardewValley.Menus.TailoringMenu.TailorHighlight.LeftSlot" path="/summary" /></param>
    /// <param name="rightSlot"><inheritdoc cref="F:StardewValley.Menus.TailoringMenu.TailorHighlight.RightSlot" path="/summary" /></param>
    /// <param name="equipmentSlot"><inheritdoc cref="F:StardewValley.Menus.TailoringMenu.TailorHighlight.EquipmentSlot" path="/summary" /></param>
    public TailorHighlight(bool leftSlot, bool rightSlot, bool equipmentSlot)
    {
      this.LeftSlot = leftSlot;
      this.RightSlot = rightSlot;
      this.EquipmentSlot = equipmentSlot;
      this.AnySlot = leftSlot | rightSlot | equipmentSlot;
    }
  }
}
