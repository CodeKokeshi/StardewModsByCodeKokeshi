// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.CraftingPage
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley.Buffs;
using StardewValley.Extensions;
using StardewValley.GameData.Objects;
using StardewValley.Inventories;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Quests;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace StardewValley.Menus;

public class CraftingPage : IClickableMenu
{
  public const int howManyRecipesFitOnPage = 40;
  public const int numInRow = 10;
  public const int numInCol = 4;
  public const int region_upArrow = 88;
  public const int region_downArrow = 89;
  public const int region_craftingSelectionArea = 8000;
  public const int region_craftingModifier = 200;
  public string hoverText = "";
  public Item hoverItem;
  public Item lastCookingHover;
  public InventoryMenu inventory;
  public Item heldItem;
  [SkipForClickableAggregation]
  public List<Dictionary<ClickableTextureComponent, CraftingRecipe>> pagesOfCraftingRecipes = new List<Dictionary<ClickableTextureComponent, CraftingRecipe>>();
  public int currentCraftingPage;
  public CraftingRecipe hoverRecipe;
  public ClickableTextureComponent upButton;
  public ClickableTextureComponent downButton;
  public bool cooking;
  public ClickableTextureComponent trashCan;
  public ClickableComponent dropItemInvisibleButton;
  public float trashCanLidRotation;
  public List<IInventory> _materialContainers;
  protected bool _standaloneMenu;
  public int hoverAmount;
  public List<ClickableComponent> currentPageClickableComponents = new List<ClickableComponent>();
  private string hoverTitle = "";

  public CraftingPage(
    int x,
    int y,
    int width,
    int height,
    bool cooking = false,
    bool standaloneMenu = false,
    List<IInventory> materialContainers = null)
    : base(x, y, width, height)
  {
    this._standaloneMenu = standaloneMenu;
    this.cooking = cooking;
    this.inventory = new InventoryMenu(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth, this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + IClickableMenu.borderWidth + 320 - 16 /*0x10*/, false);
    this.inventory.showGrayedOutSlots = true;
    this.currentPageClickableComponents = new List<ClickableComponent>();
    foreach (ClickableComponent clickableComponent in this.inventory.GetBorder(InventoryMenu.BorderSide.Top))
      clickableComponent.upNeighborID = -99998;
    this._materialContainers = materialContainers;
    if (this._standaloneMenu)
      this.initializeUpperRightCloseButton();
    ClickableTextureComponent textureComponent = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + width + 4, this.yPositionOnScreen + height - 192 /*0xC0*/ - 32 /*0x20*/ - IClickableMenu.borderWidth - 104, 64 /*0x40*/, 104), Game1.mouseCursors, new Rectangle(564 + Game1.player.trashCanLevel * 18, 102, 18, 26), 4f);
    textureComponent.myID = 106;
    this.trashCan = textureComponent;
    this.dropItemInvisibleButton = new ClickableComponent(new Rectangle(this.xPositionOnScreen - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder - 64 /*0x40*/, this.trashCan.bounds.Y, 64 /*0x40*/, 64 /*0x40*/), "")
    {
      myID = 107,
      rightNeighborID = 0
    };
    if (this._standaloneMenu)
      Game1.playSound("bigSelect");
    this.RepositionElements();
    if (!Game1.options.SnappyMenus)
      return;
    this.snapToDefaultClickableComponent();
  }

  /// <summary>Get the recipes to display in the menu.</summary>
  protected virtual List<string> GetRecipesToDisplay()
  {
    List<string> recipesToDisplay = new List<string>();
    if (!this.cooking)
    {
      foreach (string key in CraftingRecipe.craftingRecipes.Keys)
      {
        if (Game1.player.craftingRecipes.ContainsKey(key))
          recipesToDisplay.Add(key);
      }
    }
    else
    {
      foreach (string key in CraftingRecipe.cookingRecipes.Keys)
      {
        if (!key.Equals("Moss Soup"))
          recipesToDisplay.Add(key);
      }
      recipesToDisplay.Sort((Comparison<string>) ((a, b) =>
      {
        int num1 = -1;
        int num2 = -1;
        string str1;
        if (a != null && CraftingRecipe.cookingRecipes.TryGetValue(a, out str1))
          num1 = ArgUtility.GetInt(str1.Split('/'), 2, -1);
        string str2;
        if (b != null && CraftingRecipe.cookingRecipes.TryGetValue(b, out str2))
          num2 = ArgUtility.GetInt(str2.Split('/'), 2, -1);
        return num1.CompareTo(num2);
      }));
      recipesToDisplay.Add("Moss Soup");
    }
    return recipesToDisplay;
  }

  protected virtual IList<Item> getContainerContents()
  {
    if (this._materialContainers == null)
      return (IList<Item>) null;
    List<Item> containerContents = new List<Item>();
    foreach (IInventory materialContainer in this._materialContainers)
      containerContents.AddRange((IEnumerable<Item>) materialContainer);
    return (IList<Item>) containerContents;
  }

  private int craftingPageY()
  {
    return this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + IClickableMenu.borderWidth - 16 /*0x10*/;
  }

  private ClickableTextureComponent[,] createNewPageLayout()
  {
    return new ClickableTextureComponent[10, 4];
  }

  private Dictionary<ClickableTextureComponent, CraftingRecipe> createNewPage()
  {
    Dictionary<ClickableTextureComponent, CraftingRecipe> newPage = new Dictionary<ClickableTextureComponent, CraftingRecipe>();
    this.pagesOfCraftingRecipes.Add(newPage);
    return newPage;
  }

  private bool spaceOccupied(
    ClickableTextureComponent[,] pageLayout,
    int x,
    int y,
    CraftingRecipe recipe)
  {
    if (pageLayout[x, y] != null)
      return true;
    if (!recipe.bigCraftable)
      return false;
    return y + 1 >= 4 || pageLayout[x, y + 1] != null;
  }

  private void layoutRecipes(List<string> playerRecipes)
  {
    int num1 = this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth - 16 /*0x10*/;
    int num2 = 8;
    Dictionary<ClickableTextureComponent, CraftingRecipe> newPage = this.createNewPage();
    int x = 0;
    int y = 0;
    int num3 = 0;
    ClickableTextureComponent[,] newPageLayout = this.createNewPageLayout();
    List<ClickableTextureComponent[,]> textureComponentArrayList = new List<ClickableTextureComponent[,]>();
    textureComponentArrayList.Add(newPageLayout);
    foreach (string playerRecipe in playerRecipes)
    {
      ++num3;
      int num4 = this.cooking ? 1 : 0;
      CraftingRecipe recipe = new CraftingRecipe(playerRecipe, num4 != 0);
      while (this.spaceOccupied(newPageLayout, x, y, recipe))
      {
        ++x;
        if (x >= 10)
        {
          x = 0;
          ++y;
          if (y >= 4)
          {
            newPage = this.createNewPage();
            newPageLayout = this.createNewPageLayout();
            textureComponentArrayList.Add(newPageLayout);
            x = 0;
            y = 0;
          }
        }
      }
      int num5 = 200 + num3;
      ParsedItemData itemData = recipe.GetItemData(true);
      Texture2D texture = itemData.GetTexture();
      Rectangle sourceRect = itemData.GetSourceRect();
      ClickableTextureComponent textureComponent = new ClickableTextureComponent("", new Rectangle(num1 + x * (64 /*0x40*/ + num2), this.craftingPageY() + y * 72, 64 /*0x40*/, recipe.bigCraftable ? 128 /*0x80*/ : 64 /*0x40*/), (string) null, !this.cooking || Game1.player.cookingRecipes.ContainsKey(recipe.name) ? "" : "ghosted", texture, sourceRect, 4f);
      textureComponent.myID = num5;
      textureComponent.rightNeighborID = -99998;
      textureComponent.leftNeighborID = -99998;
      textureComponent.upNeighborID = -99998;
      textureComponent.downNeighborID = -99998;
      textureComponent.fullyImmutable = true;
      textureComponent.region = 8000;
      ClickableTextureComponent key = textureComponent;
      newPage.Add(key, recipe);
      newPageLayout[x, y] = key;
      if (recipe.bigCraftable)
        newPageLayout[x, y + 1] = key;
    }
  }

  protected override void noSnappedComponentFound(int direction, int oldRegion, int oldID)
  {
    base.noSnappedComponentFound(direction, oldRegion, oldID);
    if (oldRegion != 8000 || direction != 2)
      return;
    this.currentlySnappedComponent = this.getComponentWithID(oldID % 10);
    this.currentlySnappedComponent.upNeighborID = oldID;
  }

  public override void snapToDefaultClickableComponent()
  {
    this.currentlySnappedComponent = this.currentCraftingPage < this.pagesOfCraftingRecipes.Count ? (ClickableComponent) this.pagesOfCraftingRecipes[this.currentCraftingPage].First<KeyValuePair<ClickableTextureComponent, CraftingRecipe>>().Key : (ClickableComponent) null;
    this.snapCursorToCurrentSnappedComponent();
  }

  /// <inheritdoc />
  protected override void actionOnRegionChange(int oldRegion, int newRegion)
  {
    base.actionOnRegionChange(oldRegion, newRegion);
    if (newRegion != 9000 || oldRegion == 0)
      return;
    for (int index = 0; index < 10; ++index)
    {
      if (this.inventory.inventory.Count > index)
        this.inventory.inventory[index].upNeighborID = this.currentlySnappedComponent.upNeighborID;
    }
  }

  /// <inheritdoc />
  public override void receiveKeyPress(Keys key)
  {
    base.receiveKeyPress(key);
    if (key == Keys.Delete && this.heldItem != null && this.heldItem.canBeTrashed())
    {
      Utility.trashItem(this.heldItem);
      this.heldItem = (Item) null;
    }
    if (!Game1.isAnyGamePadButtonBeingPressed() || !Game1.options.doesInputListContain(Game1.options.menuButton, key) || this.heldItem == null)
      return;
    Game1.setMousePosition(this.trashCan.bounds.Center);
  }

  /// <inheritdoc />
  public override void receiveScrollWheelAction(int direction)
  {
    base.receiveScrollWheelAction(direction);
    if (direction > 0 && this.currentCraftingPage > 0)
    {
      --this.currentCraftingPage;
      this._UpdateCurrentPageButtons();
      Game1.playSound("shwip");
      if (!Game1.options.SnappyMenus)
        return;
      this.setCurrentlySnappedComponentTo(88);
      this.snapCursorToCurrentSnappedComponent();
    }
    else
    {
      if (direction >= 0 || this.currentCraftingPage >= this.pagesOfCraftingRecipes.Count - 1)
        return;
      ++this.currentCraftingPage;
      this._UpdateCurrentPageButtons();
      Game1.playSound("shwip");
      if (!Game1.options.SnappyMenus)
        return;
      this.setCurrentlySnappedComponentTo(89);
      this.snapCursorToCurrentSnappedComponent();
    }
  }

  /// <inheritdoc />
  public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
  {
    base.gameWindowSizeChanged(oldBounds, newBounds);
    this.RepositionElements();
  }

  public virtual void RepositionElements()
  {
    List<string> recipesToDisplay = this.GetRecipesToDisplay();
    this.pagesOfCraftingRecipes.Clear();
    this.layoutRecipes(recipesToDisplay);
    if (this.pagesOfCraftingRecipes.Count > 1)
    {
      ClickableTextureComponent textureComponent1 = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + 768 /*0x0300*/ + 32 /*0x20*/, this.craftingPageY(), 64 /*0x40*/, 64 /*0x40*/), Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 12), 0.8f);
      textureComponent1.myID = 88;
      textureComponent1.downNeighborID = 89;
      textureComponent1.rightNeighborID = 106;
      textureComponent1.leftNeighborID = -99998;
      this.upButton = textureComponent1;
      ClickableTextureComponent textureComponent2 = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + 768 /*0x0300*/ + 32 /*0x20*/, this.craftingPageY() + 192 /*0xC0*/ + 32 /*0x20*/, 64 /*0x40*/, 64 /*0x40*/), Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 11), 0.8f);
      textureComponent2.myID = 89;
      textureComponent2.upNeighborID = 88;
      textureComponent2.rightNeighborID = 106;
      textureComponent2.leftNeighborID = -99998;
      this.downButton = textureComponent2;
    }
    this.inventory.SetPosition(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth, this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + IClickableMenu.borderWidth + 320 - 16 /*0x10*/);
    this.trashCan.bounds.X = this.xPositionOnScreen + this.width + 4;
    this.trashCan.bounds.Y = this.yPositionOnScreen + this.height - 192 /*0xC0*/ - 32 /*0x20*/ - IClickableMenu.borderWidth - 104;
    this.dropItemInvisibleButton.bounds.X = this.xPositionOnScreen - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder - 64 /*0x40*/;
    this.dropItemInvisibleButton.bounds.Y = this.trashCan.bounds.Y;
    if (this.upButton != null)
    {
      this.upButton.bounds.X = this.xPositionOnScreen + 768 /*0x0300*/ + 32 /*0x20*/;
      this.upButton.bounds.Y = this.craftingPageY();
    }
    if (this.downButton != null)
    {
      this.downButton.bounds.X = this.xPositionOnScreen + 768 /*0x0300*/ + 32 /*0x20*/;
      this.downButton.bounds.Y = this.craftingPageY() + 192 /*0xC0*/ + 32 /*0x20*/;
    }
    this._UpdateCurrentPageButtons();
  }

  /// <inheritdoc />
  public override void receiveLeftClick(int x, int y, bool playSound = true)
  {
    base.receiveLeftClick(x, y);
    this.heldItem = this.inventory.leftClick(x, y, this.heldItem);
    if (this.upButton != null && this.upButton.containsPoint(x, y) && this.currentCraftingPage > 0)
    {
      Game1.playSound("coin");
      this.currentCraftingPage = Math.Max(0, this.currentCraftingPage - 1);
      this._UpdateCurrentPageButtons();
      this.upButton.scale = this.upButton.baseScale;
    }
    if (this.downButton != null && this.downButton.containsPoint(x, y) && this.currentCraftingPage < this.pagesOfCraftingRecipes.Count - 1)
    {
      Game1.playSound("coin");
      this.currentCraftingPage = Math.Min(this.pagesOfCraftingRecipes.Count - 1, this.currentCraftingPage + 1);
      this._UpdateCurrentPageButtons();
      this.downButton.scale = this.downButton.baseScale;
    }
    foreach (ClickableTextureComponent key in this.pagesOfCraftingRecipes[this.currentCraftingPage].Keys)
    {
      int num = Game1.oldKBState.IsKeyDown(Keys.LeftShift) ? (Game1.oldKBState.IsKeyDown(Keys.LeftControl) ? 25 : 5) : 1;
      for (int index = 0; index < num; ++index)
      {
        if (key.containsPoint(x, y, 4) && !key.hoverText.Equals("ghosted") && this.pagesOfCraftingRecipes[this.currentCraftingPage][key].doesFarmerHaveIngredientsInInventory(this.getContainerContents()))
          this.clickCraftingRecipe(key, index == 0);
      }
      if (this.heldItem != null && Game1.oldKBState.IsKeyDown(Keys.LeftShift) && this.heldItem.maximumStackSize() == 1 && Game1.player.couldInventoryAcceptThisItem(this.heldItem))
      {
        Game1.player.addItemToInventoryBool(this.heldItem);
        this.heldItem = (Item) null;
      }
    }
    if (this.trashCan != null && this.trashCan.containsPoint(x, y) && this.heldItem != null && this.heldItem.canBeTrashed())
    {
      Utility.trashItem(this.heldItem);
      this.heldItem = (Item) null;
    }
    else
    {
      if (this.heldItem == null || this.isWithinBounds(x, y) || !this.heldItem.canBeTrashed())
        return;
      Game1.playSound("throwDownITem");
      Game1.createItemDebris(this.heldItem, Game1.player.getStandingPosition(), Game1.player.FacingDirection);
      this.heldItem = (Item) null;
    }
  }

  protected void _UpdateCurrentPageButtons()
  {
    this.currentPageClickableComponents.Clear();
    foreach (ClickableComponent key in this.pagesOfCraftingRecipes[this.currentCraftingPage].Keys)
      this.currentPageClickableComponents.Add(key);
    this.populateClickableComponentList();
  }

  private void clickCraftingRecipe(ClickableTextureComponent c, bool playSound = true)
  {
    CraftingRecipe recipe = this.pagesOfCraftingRecipes[this.currentCraftingPage][c];
    Item crafted = recipe.createItem();
    List<KeyValuePair<string, int>> keyValuePairList = (List<KeyValuePair<string, int>>) null;
    if (this.cooking && crafted.Quality == 0)
    {
      keyValuePairList = new List<KeyValuePair<string, int>>();
      keyValuePairList.Add(new KeyValuePair<string, int>("917", 1));
      if (CraftingRecipe.DoesFarmerHaveAdditionalIngredientsInInventory(keyValuePairList, this.getContainerContents()))
        crafted.Quality = 2;
      else
        keyValuePairList = (List<KeyValuePair<string, int>>) null;
    }
    if (this.heldItem == null)
    {
      recipe.consumeIngredients(this._materialContainers);
      this.heldItem = crafted;
      if (playSound)
        Game1.playSound("coin");
    }
    else
    {
      if (!(this.heldItem.Name == crafted.Name) || !this.heldItem.getOne().canStackWith((ISalable) crafted.getOne()) || this.heldItem.Stack + recipe.numberProducedPerCraft - 1 >= this.heldItem.maximumStackSize())
        return;
      this.heldItem.Stack += recipe.numberProducedPerCraft;
      recipe.consumeIngredients(this._materialContainers);
      if (playSound)
        Game1.playSound("coin");
    }
    if (keyValuePairList != null)
    {
      if (playSound)
        Game1.playSound("breathin");
      CraftingRecipe.ConsumeAdditionalIngredients(keyValuePairList, this._materialContainers);
      if (!CraftingRecipe.DoesFarmerHaveAdditionalIngredientsInInventory(keyValuePairList, this.getContainerContents()))
        Game1.showGlobalMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Seasoning_UsedLast"));
    }
    Game1.player.NotifyQuests((Func<Quest, bool>) (quest => quest.OnRecipeCrafted(recipe, crafted)));
    if (!this.cooking && Game1.player.craftingRecipes.ContainsKey(recipe.name))
      Game1.player.craftingRecipes[recipe.name] += recipe.numberProducedPerCraft;
    if (this.cooking)
    {
      Game1.player.cookedRecipe(this.heldItem.ItemId);
      Game1.stats.checkForCookingAchievements();
    }
    else
      Game1.stats.checkForCraftingAchievements();
    if (!Game1.options.gamepadControls || this.heldItem == null || !Game1.player.couldInventoryAcceptThisItem(this.heldItem))
      return;
    Game1.player.addItemToInventoryBool(this.heldItem);
    this.heldItem = (Item) null;
  }

  /// <inheritdoc />
  public override void receiveRightClick(int x, int y, bool playSound = true)
  {
    this.heldItem = this.inventory.rightClick(x, y, this.heldItem);
    foreach (ClickableTextureComponent key in this.pagesOfCraftingRecipes[this.currentCraftingPage].Keys)
    {
      if (key.containsPoint(x, y) && !key.hoverText.Equals("ghosted") && this.pagesOfCraftingRecipes[this.currentCraftingPage][key].doesFarmerHaveIngredientsInInventory(this.getContainerContents()))
        this.clickCraftingRecipe(key);
    }
  }

  /// <inheritdoc />
  public override void performHoverAction(int x, int y)
  {
    CraftingRecipe hoverRecipe = this.hoverRecipe;
    base.performHoverAction(x, y);
    this.hoverTitle = "";
    this.hoverText = "";
    this.hoverRecipe = (CraftingRecipe) null;
    this.hoverItem = this.inventory.hover(x, y, this.hoverItem);
    this.hoverAmount = -1;
    if (this.hoverItem != null)
    {
      this.hoverTitle = this.inventory.hoverTitle;
      this.hoverText = this.inventory.hoverText;
    }
    foreach (ClickableTextureComponent key in this.pagesOfCraftingRecipes[this.currentCraftingPage].Keys)
    {
      if (key.containsPoint(x, y, 4))
      {
        if (key.hoverText.Equals("ghosted"))
        {
          this.hoverText = "???";
        }
        else
        {
          this.hoverRecipe = this.pagesOfCraftingRecipes[this.currentCraftingPage][key];
          if ((hoverRecipe == null ? 1 : (hoverRecipe.name != this.hoverRecipe.name ? 1 : 0)) != 0)
            this.lastCookingHover = this.hoverRecipe.createItem();
          key.scale = Math.Min(key.scale + 0.02f, key.baseScale + 0.1f);
        }
      }
      else
        key.scale = Math.Max(key.scale - 0.02f, key.baseScale);
    }
    if (this.upButton != null)
    {
      if (this.upButton.containsPoint(x, y))
        this.upButton.scale = Math.Min(this.upButton.scale + 0.02f, this.upButton.baseScale + 0.1f);
      else
        this.upButton.scale = Math.Max(this.upButton.scale - 0.02f, this.upButton.baseScale);
    }
    if (this.downButton != null)
    {
      if (this.downButton.containsPoint(x, y))
        this.downButton.scale = Math.Min(this.downButton.scale + 0.02f, this.downButton.baseScale + 0.1f);
      else
        this.downButton.scale = Math.Max(this.downButton.scale - 0.02f, this.downButton.baseScale);
    }
    if (this.trashCan == null)
      return;
    if (this.trashCan.containsPoint(x, y))
    {
      if ((double) this.trashCanLidRotation <= 0.0)
        Game1.playSound("trashcanlid");
      this.trashCanLidRotation = Math.Min(this.trashCanLidRotation + (float) Math.PI / 48f, 1.57079637f);
      if (this.heldItem == null || Utility.getTrashReclamationPrice(this.heldItem, Game1.player) <= 0)
        return;
      this.hoverText = Game1.content.LoadString("Strings\\UI:TrashCanSale");
      this.hoverAmount = Utility.getTrashReclamationPrice(this.heldItem, Game1.player);
    }
    else
      this.trashCanLidRotation = Math.Max(this.trashCanLidRotation - (float) Math.PI / 48f, 0.0f);
  }

  public override bool readyToClose() => this.heldItem == null;

  /// <inheritdoc />
  public override void draw(SpriteBatch b)
  {
    if (this._standaloneMenu)
      Game1.drawDialogueBox(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height, false, true);
    this.drawHorizontalPartition(b, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + 256 /*0x0100*/);
    this.inventory.draw(b);
    if (this.trashCan != null)
    {
      this.trashCan.draw(b);
      b.Draw(Game1.mouseCursors, new Vector2((float) (this.trashCan.bounds.X + 60), (float) (this.trashCan.bounds.Y + 40)), new Rectangle?(new Rectangle(564 + Game1.player.trashCanLevel * 18, 129, 18, 10)), Color.White, this.trashCanLidRotation, new Vector2(16f, 10f), 4f, SpriteEffects.None, 0.86f);
    }
    b.End();
    b.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp);
    foreach (ClickableTextureComponent key in this.pagesOfCraftingRecipes[this.currentCraftingPage].Keys)
    {
      if (key.hoverText.Equals("ghosted"))
        key.draw(b, Color.Black * 0.35f, 0.89f);
      else if (!this.pagesOfCraftingRecipes[this.currentCraftingPage][key].doesFarmerHaveIngredientsInInventory(this.getContainerContents()))
      {
        key.draw(b, Color.DimGray * 0.4f, 0.89f);
        if (this.pagesOfCraftingRecipes[this.currentCraftingPage][key].numberProducedPerCraft > 1)
          NumberSprite.draw(this.pagesOfCraftingRecipes[this.currentCraftingPage][key].numberProducedPerCraft, b, new Vector2((float) (key.bounds.X + 64 /*0x40*/ - 2), (float) (key.bounds.Y + 64 /*0x40*/ - 2)), Color.LightGray * 0.75f, (float) (0.5 * ((double) key.scale / 4.0)), 0.97f, 1f, 0);
      }
      else
      {
        key.draw(b);
        if (this.pagesOfCraftingRecipes[this.currentCraftingPage][key].numberProducedPerCraft > 1)
          NumberSprite.draw(this.pagesOfCraftingRecipes[this.currentCraftingPage][key].numberProducedPerCraft, b, new Vector2((float) (key.bounds.X + 64 /*0x40*/ - 2), (float) (key.bounds.Y + 64 /*0x40*/ - 2)), Color.White, (float) (0.5 * ((double) key.scale / 4.0)), 0.97f, 1f, 0);
      }
    }
    b.End();
    b.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
    if (this.hoverItem != null)
      IClickableMenu.drawToolTip(b, this.hoverText, this.hoverTitle, this.hoverItem, this.heldItem != null);
    else if (!string.IsNullOrEmpty(this.hoverText))
    {
      if (this.hoverAmount > 0)
        IClickableMenu.drawToolTip(b, this.hoverText, this.hoverTitle, (Item) null, true, moneyAmountToShowAtBottom: this.hoverAmount);
      else
        IClickableMenu.drawHoverText(b, this.hoverText, Game1.smallFont, this.heldItem != null ? 64 /*0x40*/ : 0, this.heldItem != null ? 64 /*0x40*/ : 0);
    }
    this.heldItem?.drawInMenu(b, new Vector2((float) (Game1.getOldMouseX() + 16 /*0x10*/), (float) (Game1.getOldMouseY() + 16 /*0x10*/)), 1f);
    base.draw(b);
    if (this.downButton != null && this.currentCraftingPage < this.pagesOfCraftingRecipes.Count - 1)
      this.downButton.draw(b);
    if (this.upButton != null && this.currentCraftingPage > 0)
      this.upButton.draw(b);
    if (this._standaloneMenu)
    {
      Game1.mouseCursorTransparency = 1f;
      this.drawMouse(b);
    }
    if (this.hoverRecipe == null)
      return;
    ObjectData objectData;
    if (this.cooking && this.lastCookingHover.HasTypeObject() && Game1.objectData.TryGetValue(this.lastCookingHover.ItemId, out objectData))
    {
      BuffEffects buffEffects = new BuffEffects();
      foreach (Buff buff in StardewValley.Object.TryCreateBuffsFromData(objectData, this.lastCookingHover.Name, this.lastCookingHover.DisplayName, adjustEffects: new Action<BuffEffects>(this.lastCookingHover.ModifyItemBuffs)))
        buffEffects.Add(buff.effects);
      if (buffEffects.HasAnyValue())
        buffEffects.ToLegacyAttributeFormat();
    }
    IClickableMenu.drawToolTip(b, " ", this.hoverRecipe.DisplayName + (this.hoverRecipe.numberProducedPerCraft > 1 ? " x" + this.hoverRecipe.numberProducedPerCraft.ToString() : ""), this.lastCookingHover, this.heldItem != null, craftingIngredients: this.hoverRecipe, additionalCraftMaterials: this.getContainerContents());
  }

  protected override bool _ShouldAutoSnapPrioritizeAlignedElements() => false;

  public override bool IsAutomaticSnapValid(
    int direction,
    ClickableComponent a,
    ClickableComponent b)
  {
    return (a != this.downButton && a != this.upButton || direction != 3 || b.region == 8000) && (a.region != 8000 || direction != 3 && direction != 1 || b.region != 9000) && (a.region != 8000 || direction != 2 || b != this.upButton && b != this.downButton) && base.IsAutomaticSnapValid(direction, a, b);
  }

  public override void emergencyShutDown()
  {
    base.emergencyShutDown();
    if (this.heldItem == null)
      return;
    Item heldItem = this.heldItem;
    this.heldItem = (Item) null;
    Utility.CollectOrDrop(heldItem);
  }
}
