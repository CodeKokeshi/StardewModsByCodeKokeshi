// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.ShopMenu
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley.BellsAndWhistles;
using StardewValley.Extensions;
using StardewValley.GameData.Shops;
using StardewValley.Internal;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley.TokenizableStrings;
using StardewValley.Triggers;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace StardewValley.Menus;

public class ShopMenu : IClickableMenu
{
  public const int region_shopButtonModifier = 3546;
  public const int region_upArrow = 97865;
  public const int region_downArrow = 97866;
  public const int region_tabStartIndex = 99999;
  public const int infiniteStock = 2147483647 /*0x7FFFFFFF*/;
  public const int itemsPerPage = 4;
  public const int numberRequiredForExtraItemTrade = 5;
  public string hoverText = "";
  public string boldTitleText = "";
  /// <summary>The sound played when the shop menu is opened.</summary>
  public string openMenuSound = "dwop";
  /// <summary>The sound played when an item is purchased normally.</summary>
  public string purchaseSound = "purchaseClick";
  /// <summary>The repeating sound played when accumulating a stack to purchase (e.g. by holding right-click on PC).</summary>
  public string purchaseRepeatSound = "purchaseRepeat";
  /// <summary>A key which identifies the current shop. This may be the unique shop ID in <c>Data/Shops</c> for a standard shop, <c>Dresser</c> or <c>FishTank</c> for furniture, etc.</summary>
  public string ShopId;
  /// <summary>The underlying shop data, if this is a standard shop from <c>Data/Shops</c>.</summary>
  public ShopData ShopData;
  public InventoryMenu inventory;
  public ISalable heldItem;
  public ISalable hoveredItem;
  /// <summary>How to draw stack size numbers in the shop list by default. If set, this overrides <see cref="F:StardewValley.GameData.Shops.ShopData.StackSizeVisibility" />.</summary>
  public StackDrawType? DefaultStackDrawType;
  private TemporaryAnimatedSprite poof;
  private Rectangle scrollBarRunner;
  /// <summary>The items sold in the shop.</summary>
  public List<ISalable> forSale = new List<ISalable>();
  public List<ClickableComponent> forSaleButtons = new List<ClickableComponent>();
  public List<int> categoriesToSellHere = new List<int>();
  public List<List<string>> tagsToSellHere = new List<List<string>>();
  /// <summary>The stock info for each item in <see cref="F:StardewValley.Menus.ShopMenu.forSale" />.</summary>
  public Dictionary<ISalable, ItemStockInformation> itemPriceAndStock = new Dictionary<ISalable, ItemStockInformation>();
  private float sellPercentage = 1f;
  private TemporaryAnimatedSpriteList animations = new TemporaryAnimatedSpriteList();
  public int hoverPrice = -1;
  public int currentItemIndex;
  /// <summary>The currency in which all items in the shop should be priced. The valid values are 0 (money), 1 (star tokens), 2 (Qi coins), and 4 (Qi gems).</summary>
  public int currency;
  public ClickableTextureComponent upArrow;
  public ClickableTextureComponent downArrow;
  public ClickableTextureComponent scrollBar;
  public Texture2D portraitTexture;
  public string potraitPersonDialogue;
  public object source;
  private bool scrolling;
  /// <summary>A callback to invoke when the player purchases an item, if any.</summary>
  public ShopMenu.OnPurchaseDelegate onPurchase;
  /// <summary>A callback to invoke when the player sells an item, if any.</summary>
  public Func<ISalable, bool> onSell;
  public Func<int, bool> canPurchaseCheck;
  public List<ShopMenu.ShopTabClickableTextureComponent> tabButtons = new List<ShopMenu.ShopTabClickableTextureComponent>();
  protected int currentTab;
  protected bool _isStorageShop;
  public bool readOnly;
  public HashSet<ISalable> buyBackItems = new HashSet<ISalable>();
  public Dictionary<ISalable, ISalable> buyBackItemsToResellTomorrow = new Dictionary<ISalable, ISalable>();
  /// <summary>The number of milliseconds until the menu will allow buying or selling items, to help avoid doing so accidentally.</summary>
  public int safetyTimer = 250;

  /// <summary>The visual theme applied to the shop UI.</summary>
  /// <remarks>This can be set via <see cref="M:StardewValley.Menus.ShopMenu.SetVisualTheme(StardewValley.GameData.Shops.ShopThemeData)" />.</remarks>
  public ShopMenu.ShopCachedTheme VisualTheme { get; private set; }

  /// <summary>Construct an instance.</summary>
  /// <param name="shopId">The unique shop ID in <c>Data\Shops</c>.</param>
  /// <param name="shopData">The shop data from <c>Data/Shops</c>.</param>
  /// <param name="ownerData">The owner entry for the shop portrait and dialogue, or <c>null</c> to disable those.</param>
  /// <param name="owner">The NPC matching <paramref name="ownerData" /> whose portrait to show, if applicable.</param>
  /// <param name="onPurchase">A callback to invoke when the player purchases an item, if any.</param>
  /// <param name="onSell">A callback to invoke when the player sells an item, if any.</param>
  /// <param name="playOpenSound">Whether to play the open-menu sound.</param>
  public ShopMenu(
    string shopId,
    ShopData shopData,
    ShopOwnerData ownerData,
    NPC owner = null,
    ShopMenu.OnPurchaseDelegate onPurchase = null,
    Func<ISalable, bool> onSell = null,
    bool playOpenSound = true)
  {
    this.ShopId = shopId ?? throw new ArgumentNullException(nameof (shopId));
    foreach (KeyValuePair<ISalable, ItemStockInformation> keyValuePair in ShopBuilder.GetShopStock(shopId, shopData))
      this.AddForSale(keyValuePair.Key, keyValuePair.Value);
    this.ShopData = shopData;
    if (shopData.SalableItemTags != null)
    {
      foreach (string salableItemTag in shopData.SalableItemTags)
      {
        List<string> stringList = new List<string>();
        foreach (string str in salableItemTag.Split(','))
          stringList.Add(str.Trim());
        this.tagsToSellHere.Add(stringList);
      }
    }
    this.openMenuSound = shopData.OpenSound ?? this.openMenuSound;
    this.purchaseSound = shopData.PurchaseSound ?? this.purchaseSound;
    this.purchaseRepeatSound = shopData.PurchaseRepeatSound ?? this.purchaseRepeatSound;
    List<ShopThemeData> visualTheme = shopData.VisualTheme;
    this.SetVisualTheme(visualTheme != null ? visualTheme.FirstOrDefault<ShopThemeData>((Func<ShopThemeData, bool>) (theme => GameStateQuery.CheckConditions(theme.Condition))) : (ShopThemeData) null);
    this.SetUpShopOwner(ownerData, owner);
    this.Initialize(shopData.Currency, onPurchase, onSell, playOpenSound);
  }

  /// <summary>Construct an instance.</summary>
  /// <param name="shopId">A key which identifies the current shop.</param>
  /// <param name="itemPriceAndStock">The items to sell in the shop.</param>
  /// <param name="currency">The currency in which all items in the shop should be priced. The valid values are 0 (money), 1 (star tokens), 2 (Qi coins), and 4 (Qi gems).</param>
  /// <param name="who">The internal name for the NPC running the shop, if any.</param>
  /// <param name="on_purchase">A callback to invoke when the player purchases an item, if any.</param>
  /// <param name="on_sell">A callback to invoke when the player sells an item, if any.</param>
  /// <param name="playOpenSound">Whether to play the open-menu sound.</param>
  public ShopMenu(
    string shopId,
    Dictionary<ISalable, ItemStockInformation> itemPriceAndStock,
    int currency = 0,
    string who = null,
    ShopMenu.OnPurchaseDelegate on_purchase = null,
    Func<ISalable, bool> on_sell = null,
    bool playOpenSound = true)
  {
    this.ShopId = shopId ?? throw new ArgumentNullException(nameof (shopId));
    foreach (KeyValuePair<ISalable, ItemStockInformation> keyValuePair in itemPriceAndStock)
      this.AddForSale(keyValuePair.Key, keyValuePair.Value);
    this.SetVisualTheme((ShopThemeData) null);
    this.setUpShopOwner(who, shopId);
    this.Initialize(currency, on_purchase, on_sell, playOpenSound);
  }

  /// <summary>Construct an instance.</summary>
  /// <param name="shopId">A key which identifies the current shop.</param>
  /// <param name="itemsForSale">The items to sell in the shop.</param>
  /// <param name="currency">The currency in which all items in the shop should be priced. The valid values are 0 (money), 1 (star tokens), 2 (Qi coins), and 4 (Qi gems).</param>
  /// <param name="who">The internal name for the NPC running the shop, if any.</param>
  /// <param name="on_purchase">A callback to invoke when the player purchases an item, if any.</param>
  /// <param name="on_sell">A callback to invoke when the player sells an item, if any.</param>
  /// <param name="playOpenSound">Whether to play the open-menu sound.</param>
  public ShopMenu(
    string shopId,
    List<ISalable> itemsForSale,
    int currency = 0,
    string who = null,
    ShopMenu.OnPurchaseDelegate on_purchase = null,
    Func<ISalable, bool> on_sell = null,
    bool playOpenSound = true)
    : base(Game1.uiViewport.Width / 2 - (800 + IClickableMenu.borderWidth * 2) / 2, Game1.uiViewport.Height / 2 - (600 + IClickableMenu.borderWidth * 2) / 2, 1000 + IClickableMenu.borderWidth * 2, 600 + IClickableMenu.borderWidth * 2, true)
  {
    this.ShopId = shopId ?? throw new ArgumentNullException(nameof (shopId));
    foreach (ISalable salable in itemsForSale)
      this.AddForSale(salable);
    this.SetVisualTheme((ShopThemeData) null);
    this.setUpShopOwner(who, shopId);
    this.Initialize(currency, on_purchase, on_sell, playOpenSound);
  }

  /// <summary>Set the visual theme for the shop menu.</summary>
  /// <param name="theme">The visual theme to display, or <c>null</c> for the default theme.</param>
  /// <remarks>The visual theme is usually set in <c>Data/Shops</c> instead of calling this method directly.</remarks>
  public void SetVisualTheme(ShopThemeData theme)
  {
    this.VisualTheme = new ShopMenu.ShopCachedTheme(theme);
    if (this.upArrow == null)
      return;
    Rectangle rectangle = new Rectangle(Game1.uiViewport.X, Game1.uiViewport.Y, Game1.uiViewport.Width, Game1.uiViewport.Height);
    this.gameWindowSizeChanged(rectangle, rectangle);
  }

  /// <summary>Initialize the shop menu after the stock has been constructed.</summary>
  /// <param name="currency">The currency in which all items in the shop should be priced. The valid values are 0 (money), 1 (star tokens), 2 (Qi coins), and 4 (Qi gems).</param>
  /// <param name="onPurchase">A callback to invoke when the player purchases an item, if any.</param>
  /// <param name="onSell">A callback to invoke when the player sells an item, if any.</param>
  /// <param name="playOpenSound">Whether to play the open-menu sound.</param>
  private void Initialize(
    int currency,
    ShopMenu.OnPurchaseDelegate onPurchase,
    Func<ISalable, bool> onSell,
    bool playOpenSound)
  {
    ShopMenu.ShopCachedTheme visualTheme = this.VisualTheme;
    this.updatePosition();
    this.upperRightCloseButton = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + this.width - 36, this.yPositionOnScreen - 8, 48 /*0x30*/, 48 /*0x30*/), Game1.mouseCursors, new Rectangle(337, 494, 12, 12), 4f);
    this.currency = currency;
    this.onPurchase = onPurchase;
    this.onSell = onSell;
    Game1.player.forceCanMove();
    if (playOpenSound)
      this.PlayOpenSound();
    this.inventory = new InventoryMenu(this.xPositionOnScreen + this.width, this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + IClickableMenu.borderWidth + 320 + 40, false, highlightMethod: new InventoryMenu.highlightThisItem(this.highlightItemToSell))
    {
      showGrayedOutSlots = true
    };
    this.inventory.movePosition(-this.inventory.width - 32 /*0x20*/, 0);
    ClickableTextureComponent textureComponent1 = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + this.width + 16 /*0x10*/, this.yPositionOnScreen + 16 /*0x10*/, 44, 48 /*0x30*/), visualTheme.ScrollUpTexture, visualTheme.ScrollUpSourceRect, 4f);
    textureComponent1.myID = 97865;
    textureComponent1.downNeighborID = 106;
    textureComponent1.leftNeighborID = 3546;
    this.upArrow = textureComponent1;
    ClickableTextureComponent textureComponent2 = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + this.width + 16 /*0x10*/, this.yPositionOnScreen + this.height - 64 /*0x40*/, 44, 48 /*0x30*/), visualTheme.ScrollDownTexture, visualTheme.ScrollDownSourceRect, 4f);
    textureComponent2.myID = 106;
    textureComponent2.upNeighborID = 97865;
    textureComponent2.leftNeighborID = 3546;
    this.downArrow = textureComponent2;
    this.scrollBar = new ClickableTextureComponent(new Rectangle(this.upArrow.bounds.X + 12, this.upArrow.bounds.Y + this.upArrow.bounds.Height + 4, 24, 40), visualTheme.ScrollBarFrontTexture, visualTheme.ScrollBarFrontSourceRect, 4f);
    this.scrollBarRunner = new Rectangle(this.scrollBar.bounds.X, this.upArrow.bounds.Y + this.upArrow.bounds.Height + 4, this.scrollBar.bounds.Width, this.height - 64 /*0x40*/ - this.upArrow.bounds.Height - 28);
    for (int index = 0; index < 4; ++index)
      this.forSaleButtons.Add(new ClickableComponent(new Rectangle(this.xPositionOnScreen + 16 /*0x10*/, this.yPositionOnScreen + 16 /*0x10*/ + index * ((this.height - 256 /*0x0100*/) / 4), this.width - 32 /*0x20*/, (this.height - 256 /*0x0100*/) / 4 + 4), index.ToString() ?? "")
      {
        myID = index + 3546,
        rightNeighborID = 97865,
        fullyImmutable = true
      });
    this.updateSaleButtonNeighbors();
    this.setUpStoreForContext();
    if (this.tabButtons.Count > 0)
    {
      foreach (ClickableComponent forSaleButton in this.forSaleButtons)
        forSaleButton.leftNeighborID = -99998;
    }
    this.applyTab();
    foreach (ClickableComponent clickableComponent in this.inventory.GetBorder(InventoryMenu.BorderSide.Top))
      clickableComponent.upNeighborID = -99998;
    if (Game1.options.snappyMenus && Game1.options.gamepadControls)
    {
      this.populateClickableComponentList();
      this.snapToDefaultClickableComponent();
    }
    if (currency != 4)
      return;
    int tickOpened = Game1.ticks;
    Game1.specialCurrencyDisplay.ShowCurrency("qiGems", (Func<bool>) (() =>
    {
      if (Game1.ticks == tickOpened)
        return true;
      return Game1.activeClickableMenu == this && this.currency == 4;
    }), 0.0f);
  }

  /// <summary>Add an item to sell in the menu.</summary>
  /// <param name="item">The item instance to sell.</param>
  /// <param name="stock">The stock information, or <c>null</c> to create it automatically.</param>
  public void AddForSale(ISalable item, ItemStockInformation stock = null)
  {
    if (item.IsRecipe)
    {
      if (Game1.player.knowsRecipe(item.Name))
        return;
      item.Stack = 1;
    }
    this.forSale.Add(item);
    this.itemPriceAndStock.Add(item, stock ?? new ItemStockInformation(item.salePrice(), item.Stack));
  }

  public void updateSaleButtonNeighbors()
  {
    ClickableComponent clickableComponent = this.forSaleButtons[0];
    for (int index = 0; index < this.forSaleButtons.Count; ++index)
    {
      ClickableComponent forSaleButton = this.forSaleButtons[index];
      forSaleButton.upNeighborImmutable = true;
      forSaleButton.downNeighborImmutable = true;
      forSaleButton.upNeighborID = index > 0 ? index + 3546 - 1 : -7777;
      forSaleButton.downNeighborID = index >= 3 || index >= this.forSale.Count - 1 ? -7777 : index + 3546 + 1;
      if (index >= this.forSale.Count)
      {
        if (forSaleButton == this.currentlySnappedComponent)
        {
          this.currentlySnappedComponent = clickableComponent;
          if (Game1.options.SnappyMenus)
            this.snapCursorToCurrentSnappedComponent();
        }
      }
      else
        clickableComponent = forSaleButton;
    }
  }

  public virtual void setUpStoreForContext()
  {
    this.tabButtons = (List<ShopMenu.ShopTabClickableTextureComponent>) null;
    string shopId = this.ShopId;
    if (shopId != null)
    {
      switch (shopId.Length)
      {
        case 7:
          if (shopId == "Dresser")
          {
            this.categoriesToSellHere.AddRange((IEnumerable<int>) new int[4]
            {
              -95,
              -100,
              -97,
              -96
            });
            this.UseDresserTabs();
            this._isStorageShop = true;
            goto label_21;
          }
          goto label_20;
        case 8:
          if (shopId == "FishTank")
          {
            this.UseNoTabs();
            this._isStorageShop = true;
            goto label_21;
          }
          goto label_20;
        case 9:
          if (shopId == "Catalogue")
          {
            this.UseCatalogueTabs();
            goto label_21;
          }
          goto label_20;
        case 17:
          if (shopId == "ReturnedDonations")
          {
            this.UseNoTabs();
            this._isStorageShop = true;
            goto label_21;
          }
          goto label_20;
        case 19:
          if (shopId == "Furniture Catalogue")
            break;
          goto label_20;
        case 22:
          if (shopId == "JojaFurnitureCatalogue")
            break;
          goto label_20;
        case 23:
          switch (shopId[0])
          {
            case 'R':
              if (shopId == "RetroFurnitureCatalogue")
                break;
              goto label_20;
            case 'T':
              if (shopId == "TrashFurnitureCatalogue")
                break;
              goto label_20;
            default:
              goto label_20;
          }
          break;
        case 24:
          switch (shopId[0])
          {
            case 'J':
              if (shopId == "JunimoFurnitureCatalogue")
                break;
              goto label_20;
            case 'W':
              if (shopId == "WizardFurnitureCatalogue")
                break;
              goto label_20;
            default:
              goto label_20;
          }
          break;
        default:
          goto label_20;
      }
      this.UseFurnitureCatalogueTabs();
      goto label_21;
    }
label_20:
    this.UseNoTabs();
label_21:
    if (!this._isStorageShop)
      return;
    this.purchaseSound = (string) null;
    this.purchaseRepeatSound = (string) null;
  }

  /// <summary>Remove the filter tabs, if any.</summary>
  public void UseNoTabs()
  {
    this.tabButtons = new List<ShopMenu.ShopTabClickableTextureComponent>();
    this.repositionTabs();
  }

  /// <summary>Add the filter tabs for a furniture catalogue (e.g. tables, seats, paintings, etc).</summary>
  public void UseFurnitureCatalogueTabs()
  {
    List<ShopMenu.ShopTabClickableTextureComponent> textureComponentList = new List<ShopMenu.ShopTabClickableTextureComponent>();
    ShopMenu.ShopTabClickableTextureComponent textureComponent1 = new ShopMenu.ShopTabClickableTextureComponent(new Rectangle(0, 0, 64 /*0x40*/, 64 /*0x40*/), Game1.mouseCursors2, new Rectangle(96 /*0x60*/, 48 /*0x30*/, 16 /*0x10*/, 16 /*0x10*/), 4f);
    textureComponent1.myID = 99999;
    textureComponent1.upNeighborID = -99998;
    textureComponent1.downNeighborID = -99998;
    textureComponent1.rightNeighborID = 3546;
    textureComponent1.Filter = (Func<ISalable, bool>) (_ => true);
    textureComponentList.Add(textureComponent1);
    ShopMenu.ShopTabClickableTextureComponent textureComponent2 = new ShopMenu.ShopTabClickableTextureComponent(new Rectangle(0, 0, 64 /*0x40*/, 64 /*0x40*/), Game1.mouseCursors2, new Rectangle(80 /*0x50*/, 48 /*0x30*/, 16 /*0x10*/, 16 /*0x10*/), 4f);
    textureComponent2.myID = 100000;
    textureComponent2.upNeighborID = -99998;
    textureComponent2.downNeighborID = -99998;
    textureComponent2.rightNeighborID = 3546;
    textureComponent2.Filter = (Func<ISalable, bool>) (item =>
    {
      if (!(item is Furniture furniture2))
        return false;
      return furniture2.IsTable() || furniture2.furniture_type.Value == 4;
    });
    textureComponentList.Add(textureComponent2);
    ShopMenu.ShopTabClickableTextureComponent textureComponent3 = new ShopMenu.ShopTabClickableTextureComponent(new Rectangle(0, 0, 64 /*0x40*/, 64 /*0x40*/), Game1.mouseCursors2, new Rectangle(64 /*0x40*/, 48 /*0x30*/, 16 /*0x10*/, 16 /*0x10*/), 4f);
    textureComponent3.myID = 100001;
    textureComponent3.upNeighborID = -99998;
    textureComponent3.downNeighborID = -99998;
    textureComponent3.rightNeighborID = 3546;
    textureComponent3.Filter = (Func<ISalable, bool>) (item =>
    {
      if (!(item is Furniture furniture4))
        return false;
      return furniture4.furniture_type.Value == 0 || furniture4.furniture_type.Value == 1 || furniture4.furniture_type.Value == 2 || furniture4.furniture_type.Value == 3;
    });
    textureComponentList.Add(textureComponent3);
    ShopMenu.ShopTabClickableTextureComponent textureComponent4 = new ShopMenu.ShopTabClickableTextureComponent(new Rectangle(0, 0, 64 /*0x40*/, 64 /*0x40*/), Game1.mouseCursors2, new Rectangle(64 /*0x40*/, 64 /*0x40*/, 16 /*0x10*/, 16 /*0x10*/), 4f);
    textureComponent4.myID = 100002;
    textureComponent4.upNeighborID = -99998;
    textureComponent4.downNeighborID = -99998;
    textureComponent4.rightNeighborID = 3546;
    textureComponent4.Filter = (Func<ISalable, bool>) (item =>
    {
      if (!(item is Furniture furniture6))
        return false;
      return furniture6.furniture_type.Value == 6 || furniture6.furniture_type.Value == 13;
    });
    textureComponentList.Add(textureComponent4);
    ShopMenu.ShopTabClickableTextureComponent textureComponent5 = new ShopMenu.ShopTabClickableTextureComponent(new Rectangle(0, 0, 64 /*0x40*/, 64 /*0x40*/), Game1.mouseCursors2, new Rectangle(96 /*0x60*/, 64 /*0x40*/, 16 /*0x10*/, 16 /*0x10*/), 4f);
    textureComponent5.myID = 100003;
    textureComponent5.upNeighborID = -99998;
    textureComponent5.downNeighborID = -99998;
    textureComponent5.rightNeighborID = 3546;
    textureComponent5.Filter = (Func<ISalable, bool>) (item => item is Furniture furniture7 && furniture7.furniture_type.Value == 12);
    textureComponentList.Add(textureComponent5);
    ShopMenu.ShopTabClickableTextureComponent textureComponent6 = new ShopMenu.ShopTabClickableTextureComponent(new Rectangle(0, 0, 64 /*0x40*/, 64 /*0x40*/), Game1.mouseCursors2, new Rectangle(80 /*0x50*/, 64 /*0x40*/, 16 /*0x10*/, 16 /*0x10*/), 4f);
    textureComponent6.myID = 100004;
    textureComponent6.upNeighborID = -99998;
    textureComponent6.downNeighborID = -99998;
    textureComponent6.rightNeighborID = 3546;
    textureComponent6.Filter = (Func<ISalable, bool>) (item =>
    {
      if (!(item is Furniture furniture9))
        return false;
      return furniture9.furniture_type.Value == 7 || furniture9.furniture_type.Value == 17 || furniture9.furniture_type.Value == 10 || furniture9.furniture_type.Value == 8 || furniture9.furniture_type.Value == 9 || furniture9.furniture_type.Value == 14;
    });
    textureComponentList.Add(textureComponent6);
    this.tabButtons = textureComponentList;
    this.repositionTabs();
  }

  /// <summary>Add the filter tabs for a catalogue (e.g. flooring and wallpaper).</summary>
  public void UseCatalogueTabs()
  {
    List<ShopMenu.ShopTabClickableTextureComponent> textureComponentList = new List<ShopMenu.ShopTabClickableTextureComponent>();
    ShopMenu.ShopTabClickableTextureComponent textureComponent1 = new ShopMenu.ShopTabClickableTextureComponent(new Rectangle(0, 0, 64 /*0x40*/, 64 /*0x40*/), Game1.mouseCursors2, new Rectangle(96 /*0x60*/, 48 /*0x30*/, 16 /*0x10*/, 16 /*0x10*/), 4f);
    textureComponent1.myID = 99999;
    textureComponent1.upNeighborID = -99998;
    textureComponent1.downNeighborID = -99998;
    textureComponent1.rightNeighborID = 3546;
    textureComponent1.Filter = (Func<ISalable, bool>) (item => true);
    textureComponentList.Add(textureComponent1);
    ShopMenu.ShopTabClickableTextureComponent textureComponent2 = new ShopMenu.ShopTabClickableTextureComponent(new Rectangle(0, 0, 64 /*0x40*/, 64 /*0x40*/), Game1.mouseCursors2, new Rectangle(48 /*0x30*/, 64 /*0x40*/, 16 /*0x10*/, 16 /*0x10*/), 4f);
    textureComponent2.myID = 100000;
    textureComponent2.upNeighborID = -99998;
    textureComponent2.downNeighborID = -99998;
    textureComponent2.rightNeighborID = 3546;
    textureComponent2.Filter = (Func<ISalable, bool>) (item => item is Wallpaper wallpaper1 && wallpaper1.isFloor.Value);
    textureComponentList.Add(textureComponent2);
    ShopMenu.ShopTabClickableTextureComponent textureComponent3 = new ShopMenu.ShopTabClickableTextureComponent(new Rectangle(0, 0, 64 /*0x40*/, 64 /*0x40*/), Game1.mouseCursors2, new Rectangle(32 /*0x20*/, 64 /*0x40*/, 16 /*0x10*/, 16 /*0x10*/), 4f);
    textureComponent3.myID = 100001;
    textureComponent3.upNeighborID = -99998;
    textureComponent3.downNeighborID = -99998;
    textureComponent3.rightNeighborID = 3546;
    textureComponent3.Filter = (Func<ISalable, bool>) (item => item is Wallpaper wallpaper2 && !wallpaper2.isFloor.Value);
    textureComponentList.Add(textureComponent3);
    this.tabButtons = textureComponentList;
    this.repositionTabs();
  }

  /// <summary>Add the filter tabs for a dresser (e.g. hats, shirts, pants, etc).</summary>
  public void UseDresserTabs()
  {
    List<ShopMenu.ShopTabClickableTextureComponent> textureComponentList = new List<ShopMenu.ShopTabClickableTextureComponent>();
    ShopMenu.ShopTabClickableTextureComponent textureComponent1 = new ShopMenu.ShopTabClickableTextureComponent(new Rectangle(0, 0, 64 /*0x40*/, 64 /*0x40*/), Game1.mouseCursors2, new Rectangle(0, 48 /*0x30*/, 16 /*0x10*/, 16 /*0x10*/), 4f);
    textureComponent1.myID = 99999;
    textureComponent1.upNeighborID = -99998;
    textureComponent1.downNeighborID = -99998;
    textureComponent1.rightNeighborID = 3546;
    textureComponent1.Filter = (Func<ISalable, bool>) (item => true);
    textureComponentList.Add(textureComponent1);
    ShopMenu.ShopTabClickableTextureComponent textureComponent2 = new ShopMenu.ShopTabClickableTextureComponent(new Rectangle(0, 0, 64 /*0x40*/, 64 /*0x40*/), Game1.mouseCursors2, new Rectangle(16 /*0x10*/, 48 /*0x30*/, 16 /*0x10*/, 16 /*0x10*/), 4f);
    textureComponent2.myID = 100000;
    textureComponent2.upNeighborID = -99998;
    textureComponent2.downNeighborID = -99998;
    textureComponent2.rightNeighborID = 3546;
    textureComponent2.Filter = (Func<ISalable, bool>) (salable => salable is Item obj1 && obj1.Category == -95);
    textureComponentList.Add(textureComponent2);
    ShopMenu.ShopTabClickableTextureComponent textureComponent3 = new ShopMenu.ShopTabClickableTextureComponent(new Rectangle(0, 0, 64 /*0x40*/, 64 /*0x40*/), Game1.mouseCursors2, new Rectangle(32 /*0x20*/, 48 /*0x30*/, 16 /*0x10*/, 16 /*0x10*/), 4f);
    textureComponent3.myID = 100001;
    textureComponent3.upNeighborID = -99998;
    textureComponent3.downNeighborID = -99998;
    textureComponent3.rightNeighborID = 3546;
    textureComponent3.Filter = (Func<ISalable, bool>) (salable => salable is Clothing clothing1 && clothing1.clothesType.Value == Clothing.ClothesType.SHIRT);
    textureComponentList.Add(textureComponent3);
    ShopMenu.ShopTabClickableTextureComponent textureComponent4 = new ShopMenu.ShopTabClickableTextureComponent(new Rectangle(0, 0, 64 /*0x40*/, 64 /*0x40*/), Game1.mouseCursors2, new Rectangle(48 /*0x30*/, 48 /*0x30*/, 16 /*0x10*/, 16 /*0x10*/), 4f);
    textureComponent4.myID = 100002;
    textureComponent4.upNeighborID = -99998;
    textureComponent4.downNeighborID = -99998;
    textureComponent4.rightNeighborID = 3546;
    textureComponent4.Filter = (Func<ISalable, bool>) (salable => salable is Clothing clothing2 && clothing2.clothesType.Value == Clothing.ClothesType.PANTS);
    textureComponentList.Add(textureComponent4);
    ShopMenu.ShopTabClickableTextureComponent textureComponent5 = new ShopMenu.ShopTabClickableTextureComponent(new Rectangle(0, 0, 64 /*0x40*/, 64 /*0x40*/), Game1.mouseCursors2, new Rectangle(0, 64 /*0x40*/, 16 /*0x10*/, 16 /*0x10*/), 4f);
    textureComponent5.myID = 100003;
    textureComponent5.upNeighborID = -99998;
    textureComponent5.downNeighborID = -99998;
    textureComponent5.rightNeighborID = 3546;
    textureComponent5.Filter = (Func<ISalable, bool>) (salable => salable is Item obj2 && obj2.Category == -97);
    textureComponentList.Add(textureComponent5);
    ShopMenu.ShopTabClickableTextureComponent textureComponent6 = new ShopMenu.ShopTabClickableTextureComponent(new Rectangle(0, 0, 64 /*0x40*/, 64 /*0x40*/), Game1.mouseCursors2, new Rectangle(16 /*0x10*/, 64 /*0x40*/, 16 /*0x10*/, 16 /*0x10*/), 4f);
    textureComponent6.myID = 100004;
    textureComponent6.upNeighborID = -99998;
    textureComponent6.downNeighborID = -99998;
    textureComponent6.rightNeighborID = 3546;
    textureComponent6.Filter = (Func<ISalable, bool>) (salable => salable is Item obj3 && obj3.Category == -96);
    textureComponentList.Add(textureComponent6);
    this.tabButtons = textureComponentList;
    this.repositionTabs();
  }

  public void repositionTabs()
  {
    for (int index = 0; index < this.tabButtons.Count; ++index)
    {
      if (index == this.currentTab)
        this.tabButtons[index].bounds.X = this.xPositionOnScreen - 56;
      else
        this.tabButtons[index].bounds.X = this.xPositionOnScreen - 64 /*0x40*/;
      this.tabButtons[index].bounds.Y = this.yPositionOnScreen + index * 16 /*0x10*/ * 4 + 16 /*0x10*/;
    }
  }

  protected override void customSnapBehavior(int direction, int oldRegion, int oldID)
  {
    switch (direction)
    {
      case 0:
        if (this.currentItemIndex <= 0)
          break;
        this.upArrowPressed();
        this.currentlySnappedComponent = this.getComponentWithID(3546);
        this.snapCursorToCurrentSnappedComponent();
        break;
      case 2:
        if (this.currentItemIndex < Math.Max(0, this.forSale.Count - 4))
        {
          this.downArrowPressed();
          break;
        }
        int num = -1;
        for (int index = 0; index < 12; ++index)
        {
          this.inventory.inventory[index].upNeighborID = oldID;
          if (num == -1 && this.heldItem != null)
          {
            IList<Item> actualInventory = this.inventory.actualInventory;
            if ((actualInventory != null ? (actualInventory.Count > index ? 1 : 0) : 0) != 0 && this.inventory.actualInventory[index] == null)
              num = index;
          }
        }
        this.currentlySnappedComponent = this.getComponentWithID(num != -1 ? num : 0);
        this.snapCursorToCurrentSnappedComponent();
        break;
    }
  }

  public override void snapToDefaultClickableComponent()
  {
    this.currentlySnappedComponent = this.getComponentWithID(3546);
    this.snapCursorToCurrentSnappedComponent();
  }

  public void setUpShopOwner(string who, string shopId)
  {
    ShopData shop;
    if (!DataLoader.Shops(Game1.content).TryGetValue(shopId, out shop))
      return;
    foreach (ShopOwnerData currentOwner in ShopBuilder.GetCurrentOwners(shop))
    {
      if (currentOwner.IsValid(who))
      {
        this.SetUpShopOwner(currentOwner);
        break;
      }
    }
  }

  /// <summary>Set the shop portrait and dialogue.</summary>
  /// <param name="ownerData">The owner entry in the shop data.</param>
  /// <param name="owner">The specific NPC which matches the <paramref name="ownerData" />, if set.</param>
  public void SetUpShopOwner(ShopOwnerData ownerData, NPC owner = null)
  {
    if (ownerData == null)
    {
      this.portraitTexture = (Texture2D) null;
      this.potraitPersonDialogue = (string) null;
    }
    else
    {
      string str = (string) null;
      bool flag = false;
      if (ownerData.Dialogues != null)
      {
        Random random = ownerData.RandomizeDialogueOnOpen ? Game1.random : Utility.CreateRandom((double) Game1.uniqueIDForThisGame, (double) Game1.stats.DaysPlayed);
        foreach (ShopDialogueData dialogue in ownerData.Dialogues)
        {
          if (GameStateQuery.CheckConditions(dialogue.Condition))
          {
            string text = dialogue.Dialogue;
            List<string> randomDialogue = dialogue.RandomDialogue;
            if ((randomDialogue != null ? (randomDialogue.Any<string>() ? 1 : 0) : 0) != 0)
              text = random.ChooseFrom<string>((IList<string>) dialogue.RandomDialogue);
            str = TokenParser.ParseText(text, random, new TokenParserDelegate(this.ParseDialogueSubstitution));
            break;
          }
        }
        if (string.IsNullOrWhiteSpace(str))
          flag = true;
      }
      this.portraitTexture = this.TryLoadPortrait(ownerData, owner);
      if (flag)
        return;
      this.potraitPersonDialogue = Game1.parseText(str ?? Game1.content.LoadString("Strings\\StringsFromCSFiles:ShopMenu.cs.11457"), Game1.dialogueFont, 304);
    }
  }

  /// <summary>Get the portrait to show for the selected NPC, if any.</summary>
  /// <param name="ownerData">The shop owner data.</param>
  /// <param name="owner">The specific NPC which matches the <paramref name="ownerData" />, if set.</param>
  public Texture2D TryLoadPortrait(ShopOwnerData ownerData, NPC owner)
  {
    if (ownerData.Type == ShopOwnerType.None)
      return (Texture2D) null;
    if (ownerData.Portrait != null)
    {
      if (!string.IsNullOrWhiteSpace(ownerData.Portrait))
      {
        if (Game1.content.DoesAssetExist<Texture2D>(ownerData.Portrait))
          return Game1.content.Load<Texture2D>(ownerData.Portrait);
        NPC characterFromName = Game1.getCharacterFromName(ownerData.Portrait);
        if (characterFromName?.Portrait != null)
          return characterFromName.Portrait;
      }
      return (Texture2D) null;
    }
    if (owner?.Portrait != null)
      return owner.Portrait;
    if (ownerData.Type == ShopOwnerType.NamedNpc && !string.IsNullOrWhiteSpace(ownerData.Name))
    {
      NPC characterFromName = Game1.getCharacterFromName(ownerData.Name);
      if (characterFromName?.Portrait != null)
        return characterFromName.Portrait;
    }
    return (Texture2D) null;
  }

  public bool ParseDialogueSubstitution(
    string[] query,
    out string replacement,
    Random random,
    Farmer player)
  {
    if (query[0] == "SuggestedItem")
    {
      string error;
      if (!Utility.TryCreateIntervalRandom(ArgUtility.Get(query, 1, "day"), ArgUtility.Get(query, 2, this.ShopId), out random, out error))
      {
        Game1.log.Error($"Failed parsing [SuggestedItem {string.Join(" ", query)}] in dialogue shop '{this.ShopId}': {error}");
        random = Utility.CreateRandom((double) Game1.ticks);
      }
      ISalable key;
      if (Utility.TryGetRandom<ISalable, ItemStockInformation>((IDictionary<ISalable, ItemStockInformation>) this.itemPriceAndStock, out key, out ItemStockInformation _, random))
      {
        replacement = key.DisplayName;
        return true;
      }
    }
    replacement = (string) null;
    return false;
  }

  public bool highlightItemToSell(Item i)
  {
    if (this.heldItem != null)
      return this.heldItem.canStackWith((ISalable) i);
    if (this.categoriesToSellHere.Contains(i.Category))
      return true;
    foreach (List<string> stringList in this.tagsToSellHere)
    {
      bool flag = false;
      foreach (string tag in stringList)
      {
        if (!i.HasContextTag(tag))
        {
          flag = true;
          break;
        }
      }
      if (!flag)
        return true;
    }
    return false;
  }

  public static int getPlayerCurrencyAmount(Farmer who, int currencyType)
  {
    switch (currencyType)
    {
      case 0:
        return who.Money;
      case 1:
        return who.festivalScore;
      case 2:
        return who.clubCoins;
      case 4:
        return who.QiGems;
      default:
        return 0;
    }
  }

  /// <inheritdoc />
  public override void leftClickHeld(int x, int y)
  {
    base.leftClickHeld(x, y);
    if (!this.scrolling)
      return;
    int y1 = this.scrollBar.bounds.Y;
    this.scrollBar.bounds.Y = Math.Min(this.yPositionOnScreen + this.height - 64 /*0x40*/ - 12 - this.scrollBar.bounds.Height, Math.Max(y, this.yPositionOnScreen + this.upArrow.bounds.Height + 20));
    float num = (float) (y - this.scrollBarRunner.Y) / (float) this.scrollBarRunner.Height;
    this.currentItemIndex = Math.Min(Math.Max(0, this.forSale.Count - 4), Math.Max(0, (int) ((double) this.forSale.Count * (double) num)));
    this.setScrollBarToCurrentIndex();
    this.updateSaleButtonNeighbors();
    int y2 = this.scrollBar.bounds.Y;
    if (y1 == y2)
      return;
    Game1.playSound("shiny4");
  }

  /// <inheritdoc />
  public override void releaseLeftClick(int x, int y)
  {
    base.releaseLeftClick(x, y);
    this.scrolling = false;
  }

  private void setScrollBarToCurrentIndex()
  {
    if (this.forSale.Count <= 0)
      return;
    this.scrollBar.bounds.Y = (int) ((double) ((float) this.scrollBarRunner.Height / (float) Math.Max(1, this.forSale.Count - 4 + 1)) * (double) this.currentItemIndex + (double) this.upArrow.bounds.Bottom + 4.0);
    if (this.currentItemIndex != this.forSale.Count - 4)
      return;
    this.scrollBar.bounds.Y = this.downArrow.bounds.Y - this.scrollBar.bounds.Height - 4;
  }

  /// <inheritdoc />
  public override void receiveScrollWheelAction(int direction)
  {
    base.receiveScrollWheelAction(direction);
    if (direction > 0 && this.currentItemIndex > 0)
    {
      this.upArrowPressed();
      Game1.playSound("shiny4");
    }
    else
    {
      if (direction >= 0 || this.currentItemIndex >= Math.Max(0, this.forSale.Count - 4))
        return;
      this.downArrowPressed();
      Game1.playSound("shiny4");
    }
  }

  private void downArrowPressed()
  {
    this.downArrow.scale = this.downArrow.baseScale;
    ++this.currentItemIndex;
    this.setScrollBarToCurrentIndex();
    this.updateSaleButtonNeighbors();
  }

  private void upArrowPressed()
  {
    this.upArrow.scale = this.upArrow.baseScale;
    --this.currentItemIndex;
    this.setScrollBarToCurrentIndex();
    this.updateSaleButtonNeighbors();
  }

  /// <inheritdoc />
  public override void receiveKeyPress(Keys key)
  {
    if (Game1.options.doesInputListContain(Game1.options.menuButton, key) && this.heldItem is Item heldItem)
    {
      this.heldItem = (ISalable) null;
      if (Utility.CollectOrDrop(heldItem))
        Game1.playSound("stoneStep");
      else
        Game1.playSound("throwDownITem");
    }
    else
      base.receiveKeyPress(key);
  }

  /// <inheritdoc />
  public override void receiveLeftClick(int x, int y, bool playSound = true)
  {
    base.receiveLeftClick(x, y);
    if (Game1.activeClickableMenu == null)
      return;
    Vector2 clickableComponent = this.inventory.snapToClickableComponent(x, y);
    if (this.downArrow.containsPoint(x, y) && this.currentItemIndex < Math.Max(0, this.forSale.Count - 4))
    {
      this.downArrowPressed();
      Game1.playSound("shwip");
    }
    else if (this.upArrow.containsPoint(x, y) && this.currentItemIndex > 0)
    {
      this.upArrowPressed();
      Game1.playSound("shwip");
    }
    else if (this.scrollBar.containsPoint(x, y))
      this.scrolling = true;
    else if (!this.downArrow.containsPoint(x, y) && x > this.xPositionOnScreen + this.width && x < this.xPositionOnScreen + this.width + 128 /*0x80*/ && y > this.yPositionOnScreen && y < this.yPositionOnScreen + this.height)
    {
      this.scrolling = true;
      this.leftClickHeld(x, y);
      this.releaseLeftClick(x, y);
    }
    for (int index = 0; index < this.tabButtons.Count; ++index)
    {
      if (this.tabButtons[index].containsPoint(x, y))
        this.switchTab(index);
    }
    this.currentItemIndex = Math.Max(0, Math.Min(this.forSale.Count - 4, this.currentItemIndex));
    if (this.safetyTimer <= 0)
    {
      if (this.heldItem == null && !this.readOnly)
      {
        Item sold_item = this.inventory.leftClick(x, y, (Item) null, false);
        if (sold_item != null)
        {
          if (this.onSell != null)
          {
            int num1 = this.onSell((ISalable) sold_item) ? 1 : 0;
          }
          else
          {
            int sell_unit_price = (int) ((double) sold_item.sellToStorePrice(-1L) * (double) this.sellPercentage);
            ShopMenu.chargePlayer(Game1.player, this.currency, -sell_unit_price * sold_item.Stack);
            int num2 = sold_item.Stack / 8 + 2;
            for (int index = 0; index < num2; ++index)
            {
              this.animations.Add(new TemporaryAnimatedSprite("TileSheets\\debris", new Rectangle(Game1.random.Next(2) * 16 /*0x10*/, 64 /*0x40*/, 16 /*0x10*/, 16 /*0x10*/), 9999f, 1, 999, clickableComponent + new Vector2(32f, 32f), false, false)
              {
                alphaFade = 0.025f,
                motion = new Vector2((float) Game1.random.Next(-3, 4), -4f),
                acceleration = new Vector2(0.0f, 0.5f),
                delayBeforeAnimationStart = index * 25,
                scale = 2f
              });
              this.animations.Add(new TemporaryAnimatedSprite("TileSheets\\debris", new Rectangle(Game1.random.Next(2) * 16 /*0x10*/, 64 /*0x40*/, 16 /*0x10*/, 16 /*0x10*/), 9999f, 1, 999, clickableComponent + new Vector2(32f, 32f), false, false)
              {
                scale = 4f,
                alphaFade = 0.025f,
                delayBeforeAnimationStart = index * 50,
                motion = Utility.getVelocityTowardPoint(new Point((int) clickableComponent.X + 32 /*0x20*/, (int) clickableComponent.Y + 32 /*0x20*/), new Vector2((float) (this.xPositionOnScreen - 36), (float) (this.yPositionOnScreen + this.height - this.inventory.height - 16 /*0x10*/)), 8f),
                acceleration = Utility.getVelocityTowardPoint(new Point((int) clickableComponent.X + 32 /*0x20*/, (int) clickableComponent.Y + 32 /*0x20*/), new Vector2((float) (this.xPositionOnScreen - 36), (float) (this.yPositionOnScreen + this.height - this.inventory.height - 16 /*0x10*/)), 0.5f)
              });
            }
            ISalable key = (ISalable) null;
            if (this.CanBuyback())
              key = this.AddBuybackItem((ISalable) sold_item, sell_unit_price, sold_item.Stack);
            if (sold_item is StardewValley.Object @object && @object.edibility.Value != -300)
            {
              Item one = @object.getOne();
              one.Stack = @object.Stack;
              ISalable salable;
              if (key != null && this.buyBackItemsToResellTomorrow.TryGetValue(key, out salable))
                salable.Stack += @object.Stack;
              else if (Game1.currentLocation is ShopLocation currentLocation)
              {
                if (key != null)
                  this.buyBackItemsToResellTomorrow[key] = (ISalable) one;
                currentLocation.itemsToStartSellingTomorrow.Add(one);
              }
            }
            Game1.playSound("sell");
            Game1.playSound("purchase");
            if (this.inventory.getItemAt(x, y) == null)
              this.animations.Add(new TemporaryAnimatedSprite(5, clickableComponent + new Vector2(32f, 32f), Color.White)
              {
                motion = new Vector2(0.0f, -0.5f)
              });
          }
          this.updateSaleButtonNeighbors();
        }
      }
      else
        this.heldItem = (ISalable) this.inventory.leftClick(x, y, this.heldItem as Item);
      for (int index1 = 0; index1 < this.forSaleButtons.Count; ++index1)
      {
        if (this.currentItemIndex + index1 < this.forSale.Count && this.forSaleButtons[index1].containsPoint(x, y))
        {
          int index2 = this.currentItemIndex + index1;
          if (this.forSale[index2] != null)
          {
            int val1 = Game1.oldKBState.IsKeyDown(Keys.LeftShift) ? Math.Min(Math.Min(Game1.oldKBState.IsKeyDown(Keys.LeftControl) ? (Game1.oldKBState.IsKeyDown(Keys.D1) ? 999 : 25) : 5, ShopMenu.getPlayerCurrencyAmount(Game1.player, this.currency) / Math.Max(1, this.itemPriceAndStock[this.forSale[index2]].Price)), Math.Max(1, this.itemPriceAndStock[this.forSale[index2]].Stock)) : 1;
            if (this.ShopId == "ReturnedDonations")
              val1 = this.itemPriceAndStock[this.forSale[index2]].Stock;
            int stockToBuy = Math.Min(val1, this.forSale[index2].maximumStackSize());
            if (stockToBuy == -1)
              stockToBuy = 1;
            if (this.canPurchaseCheck != null && !this.canPurchaseCheck(index2))
              return;
            if (stockToBuy > 0 && this.tryToPurchaseItem(this.forSale[index2], this.heldItem, stockToBuy, x, y))
            {
              this.itemPriceAndStock.Remove(this.forSale[index2]);
              this.forSale.RemoveAt(index2);
            }
            else if (stockToBuy <= 0)
            {
              if (this.itemPriceAndStock[this.forSale[index2]].Price > 0)
                Game1.dayTimeMoneyBox.moneyShakeTimer = 1000;
              Game1.playSound("cancel");
            }
            if (this.heldItem != null && (this._isStorageShop || Game1.options.SnappyMenus || Game1.oldKBState.IsKeyDown(Keys.LeftShift) && (this.heldItem.maximumStackSize() == 1 || this.heldItem.Stack == 999)) && Game1.activeClickableMenu is ShopMenu && Game1.player.addItemToInventoryBool(this.heldItem as Item))
            {
              this.heldItem = (ISalable) null;
              DelayedAction.playSoundAfterDelay("coin", 100);
            }
          }
          this.currentItemIndex = Math.Max(0, Math.Min(this.forSale.Count - 4, this.currentItemIndex));
          this.updateSaleButtonNeighbors();
          this.setScrollBarToCurrentIndex();
          return;
        }
      }
    }
    if (!this.readyToClose() || x >= this.xPositionOnScreen - 64 /*0x40*/ && y >= this.yPositionOnScreen - 64 /*0x40*/ && x <= this.xPositionOnScreen + this.width + 128 /*0x80*/ && y <= this.yPositionOnScreen + this.height + 64 /*0x40*/)
      return;
    this.exitThisMenu();
  }

  public virtual bool CanBuyback() => true;

  public virtual void BuyBuybackItem(ISalable bought_item, int price, int stack)
  {
    Game1.player.totalMoneyEarned -= (uint) price;
    if (Game1.player.useSeparateWallets)
      Game1.player.stats.IndividualMoneyEarned -= (uint) price;
    ISalable salable;
    if (!this.buyBackItemsToResellTomorrow.TryGetValue(bought_item, out salable))
      return;
    salable.Stack -= stack;
    if (salable.Stack > 0)
      return;
    this.buyBackItemsToResellTomorrow.Remove(bought_item);
    (Game1.currentLocation as ShopLocation).itemsToStartSellingTomorrow.Remove(salable as Item);
  }

  public virtual ISalable AddBuybackItem(ISalable sold_item, int sell_unit_price, int stack)
  {
    ISalable key = (ISalable) null;
    while (stack > 0)
    {
      key = (ISalable) null;
      foreach (ISalable buyBackItem in this.buyBackItems)
      {
        if (buyBackItem.canStackWith(sold_item) && buyBackItem.Stack < buyBackItem.maximumStackSize())
        {
          key = buyBackItem;
          break;
        }
      }
      if (key == null)
      {
        key = sold_item.GetSalableInstance();
        int stock = Math.Min(stack, key.maximumStackSize());
        this.buyBackItems.Add(key);
        this.itemPriceAndStock.Add(key, new ItemStockInformation(sell_unit_price, stock));
        key.Stack = 1;
        stack -= stock;
      }
      else
      {
        int num = Math.Min(stack, key.maximumStackSize() - key.Stack);
        ItemStockInformation stockInformation = this.itemPriceAndStock[key];
        stockInformation.Stock += num;
        this.itemPriceAndStock[key] = stockInformation;
        key.Stack = 1;
        stack -= num;
      }
    }
    this.forSale = this.itemPriceAndStock.Keys.ToList<ISalable>();
    return key;
  }

  public override bool IsAutomaticSnapValid(
    int direction,
    ClickableComponent a,
    ClickableComponent b)
  {
    return (direction != 1 || !((IEnumerable<ClickableComponent>) this.tabButtons).Contains<ClickableComponent>(a) || !((IEnumerable<ClickableComponent>) this.tabButtons).Contains<ClickableComponent>(b)) && base.IsAutomaticSnapValid(direction, a, b);
  }

  public virtual void switchTab(int new_tab)
  {
    this.currentTab = new_tab;
    Game1.playSound("shwip");
    this.applyTab();
    if (!Game1.options.snappyMenus || !Game1.options.gamepadControls)
      return;
    this.snapCursorToCurrentSnappedComponent();
  }

  public virtual void applyTab()
  {
    if (this.currentTab < 0 || this.currentTab >= this.tabButtons.Count)
    {
      this.forSale = this.itemPriceAndStock.Keys.ToList<ISalable>();
    }
    else
    {
      ShopMenu.ShopTabClickableTextureComponent tabButton = this.tabButtons[this.currentTab];
      if (tabButton.Filter == null)
        tabButton.Filter = (Func<ISalable, bool>) (_ => true);
      this.forSale.Clear();
      foreach (ISalable key in this.itemPriceAndStock.Keys)
      {
        if (tabButton.Filter(key))
          this.forSale.Add(key);
      }
      this.currentItemIndex = 0;
      this.setScrollBarToCurrentIndex();
      this.updateSaleButtonNeighbors();
    }
  }

  public override bool readyToClose() => this.heldItem == null && this.animations.Count == 0;

  public override void emergencyShutDown()
  {
    base.emergencyShutDown();
    if (this.heldItem == null)
      return;
    Game1.player.addItemToInventoryBool(this.heldItem as Item);
    Game1.playSound("coin");
  }

  /// <summary>Play the open-menu sound.</summary>
  public void PlayOpenSound() => Game1.playSound(this.openMenuSound);

  /// <summary>Get whether all items in the shop have been purchased.</summary>
  public bool IsOutOfStock() => !this._isStorageShop && this.forSale.Count == 0;

  public static void chargePlayer(Farmer who, int currencyType, int amount)
  {
    switch (currencyType)
    {
      case 0:
        who.Money -= amount;
        break;
      case 1:
        who.festivalScore -= amount;
        break;
      case 2:
        who.clubCoins -= amount;
        break;
      case 4:
        who.QiGems -= amount;
        break;
    }
  }

  public virtual void HandleSynchedItemPurchase(ISalable item, Farmer who, int number_purchased)
  {
    if (!this.itemPriceAndStock.ContainsKey(item))
      return;
    who.team.synchronizedShopStock.OnItemPurchased(this.ShopId, item, this.itemPriceAndStock, number_purchased);
  }

  private bool tryToPurchaseItem(ISalable item, ISalable held_item, int stockToBuy, int x, int y)
  {
    if (this.readOnly)
      return false;
    ItemStockInformation stock = this.itemPriceAndStock[item];
    if (held_item == null)
    {
      if (stock.Stock == 0)
      {
        this.hoveredItem = (ISalable) null;
        return true;
      }
      if (stockToBuy > item.GetSalableInstance().maximumStackSize())
        stockToBuy = Math.Max(1, item.GetSalableInstance().maximumStackSize());
      int num = stock.Price * stockToBuy;
      string itemId = (string) null;
      int count = 5;
      int stack = stockToBuy * item.Stack;
      if (stock.TradeItem != null)
      {
        itemId = stock.TradeItem;
        if (stock.TradeItemCount.HasValue)
          count = stock.TradeItemCount.Value;
        count *= stockToBuy;
      }
      if (ShopMenu.getPlayerCurrencyAmount(Game1.player, this.currency) >= num && (itemId == null || this.HasTradeItem(itemId, count)))
      {
        this.heldItem = item.GetSalableInstance();
        this.heldItem.Stack = stack;
        if (!this.heldItem.CanBuyItem(Game1.player) && !item.IsInfiniteStock() && !item.IsRecipe)
        {
          Game1.playSound("smallSelect");
          this.heldItem = (ISalable) null;
          return false;
        }
        if (this.CanBuyback() && this.buyBackItems.Contains(item))
          this.BuyBuybackItem(item, num, stack);
        ShopMenu.chargePlayer(Game1.player, this.currency, num);
        if (!string.IsNullOrEmpty(itemId))
          this.ConsumeTradeItem(itemId, count);
        if (!this._isStorageShop && item.actionWhenPurchased(this.ShopId))
        {
          if (item.IsRecipe)
          {
            if (item is Item obj)
              obj.LearnRecipe();
            Game1.playSound("newRecipe");
          }
          held_item = (ISalable) null;
          this.heldItem = (ISalable) null;
        }
        else
        {
          // ISSUE: explicit non-virtual call
          if ((this.heldItem is Item heldItem ? __nonvirtual (heldItem.QualifiedItemId) : (string) null) == "(O)858")
          {
            Game1.player.team.addQiGemsToTeam.Fire(this.heldItem.Stack);
            this.heldItem = (ISalable) null;
          }
          if (Game1.mouseClickPolling > 300)
          {
            if (this.purchaseRepeatSound != null)
              Game1.playSound(this.purchaseRepeatSound);
          }
          else if (this.purchaseSound != null)
            Game1.playSound(this.purchaseSound);
        }
        if (stock.Stock != int.MaxValue && !item.IsInfiniteStock())
        {
          this.HandleSynchedItemPurchase(item, Game1.player, stockToBuy);
          if (stock.ItemToSyncStack != null)
            stock.ItemToSyncStack.Stack = stock.Stock;
        }
        List<string> actionsOnPurchase = stock.ActionsOnPurchase;
        // ISSUE: explicit non-virtual call
        if ((actionsOnPurchase != null ? (__nonvirtual (actionsOnPurchase.Count) > 0 ? 1 : 0) : 0) != 0)
        {
          foreach (string action in stock.ActionsOnPurchase)
          {
            string error;
            Exception exception;
            if (!TriggerActionManager.TryRunAction(action, out error, out exception))
              Game1.log.Error($"Shop {this.ShopId} ignored invalid action '{action}' on purchase of item '{item.QualifiedItemId}': {error}", exception);
          }
        }
        if (this.onPurchase != null && this.onPurchase(item, Game1.player, stockToBuy, stock))
          this.exitThisMenu();
      }
      else
      {
        if (num > 0)
          Game1.dayTimeMoneyBox.moneyShakeTimer = 1000;
        Game1.playSound("cancel");
      }
    }
    else if (held_item.canStackWith(item))
    {
      stockToBuy = Math.Min(stockToBuy, (held_item.maximumStackSize() - held_item.Stack) / item.Stack);
      int stack = stockToBuy * item.Stack;
      if (stockToBuy > 0)
      {
        int num = stock.Price * stockToBuy;
        string itemId = (string) null;
        int count = 5;
        if (stock.TradeItem != null)
        {
          itemId = stock.TradeItem;
          if (stock.TradeItemCount.HasValue)
            count = stock.TradeItemCount.Value;
          count *= stockToBuy;
        }
        ISalable salableInstance = item.GetSalableInstance();
        salableInstance.Stack = stack;
        if (!salableInstance.CanBuyItem(Game1.player))
        {
          Game1.playSound("cancel");
          return false;
        }
        if (ShopMenu.getPlayerCurrencyAmount(Game1.player, this.currency) >= num && (itemId == null || this.HasTradeItem(itemId, count)))
        {
          this.heldItem.Stack += stack;
          if (this.CanBuyback() && this.buyBackItems.Contains(item))
            this.BuyBuybackItem(item, num, stack);
          ShopMenu.chargePlayer(Game1.player, this.currency, num);
          if (Game1.mouseClickPolling > 300)
          {
            if (this.purchaseRepeatSound != null)
              Game1.playSound(this.purchaseRepeatSound);
          }
          else if (this.purchaseSound != null)
            Game1.playSound(this.purchaseSound);
          if (itemId != null)
            this.ConsumeTradeItem(itemId, count);
          if (!this._isStorageShop && item.actionWhenPurchased(this.ShopId))
            this.heldItem = (ISalable) null;
          if (stock.Stock != int.MaxValue && !item.IsInfiniteStock())
          {
            this.HandleSynchedItemPurchase(item, Game1.player, stockToBuy);
            if (stock.ItemToSyncStack != null)
              stock.ItemToSyncStack.Stack = stock.Stock;
          }
          if (this.onPurchase != null && this.onPurchase(item, Game1.player, stockToBuy, stock))
            this.exitThisMenu();
        }
        else
        {
          if (num > 0)
            Game1.dayTimeMoneyBox.moneyShakeTimer = 1000;
          Game1.playSound("cancel");
        }
      }
    }
    if (stock.Stock > 0)
      return false;
    this.buyBackItems.Remove(item);
    this.hoveredItem = (ISalable) null;
    return true;
  }

  /// <summary>Get whether the player's inventory contains a minimum number of a trade item.</summary>
  /// <param name="itemId">The qualified or unqualified item ID to find.</param>
  /// <param name="count">The number needed.</param>
  public bool HasTradeItem(string itemId, int count)
  {
    itemId = ItemRegistry.QualifyItemId(itemId);
    switch (itemId)
    {
      case "(O)858":
        return Game1.player.QiGems >= count;
      case "(O)73":
        return Game1.netWorldState.Value.GoldenWalnuts >= count;
      default:
        return Game1.player.Items.ContainsId(itemId, count);
    }
  }

  /// <summary>Reduce the number of an item held by the player.</summary>
  /// <param name="itemId">The qualified or unqualified item ID.</param>
  /// <param name="count">The number to remove.</param>
  public void ConsumeTradeItem(string itemId, int count)
  {
    itemId = ItemRegistry.QualifyItemId(itemId);
    switch (itemId)
    {
      case "(O)858":
        Game1.player.QiGems = Math.Max(0, Game1.player.QiGems - count);
        break;
      case "(O)73":
        Game1.netWorldState.Value.GoldenWalnuts = Math.Max(0, Game1.netWorldState.Value.GoldenWalnuts - count);
        break;
      default:
        Game1.player.Items.ReduceId(itemId, count);
        break;
    }
  }

  /// <inheritdoc />
  public override void receiveRightClick(int x, int y, bool playSound = true)
  {
    Vector2 clickableComponent = this.inventory.snapToClickableComponent(x, y);
    if (this.safetyTimer > 0)
      return;
    if (this.heldItem == null && !this.readOnly)
    {
      ISalable sold_item = (ISalable) this.inventory.rightClick(x, y, (Item) null, false);
      if (sold_item != null)
      {
        if (this.onSell != null)
        {
          int num1 = this.onSell(sold_item) ? 1 : 0;
        }
        else
        {
          int sell_unit_price = (int) ((double) sold_item.sellToStorePrice() * (double) this.sellPercentage);
          int stack = sold_item.Stack;
          ISalable salable1 = sold_item;
          ShopMenu.chargePlayer(Game1.player, this.currency, -sell_unit_price * stack);
          ISalable key = (ISalable) null;
          if (this.CanBuyback())
            key = this.AddBuybackItem(sold_item, sell_unit_price, stack);
          if (Game1.mouseClickPolling > 300)
          {
            if (this.purchaseRepeatSound != null)
              Game1.playSound(this.purchaseRepeatSound);
          }
          else if (this.purchaseSound != null)
            Game1.playSound(this.purchaseSound);
          int num2 = 2;
          for (int index = 0; index < num2; ++index)
          {
            this.animations.Add(new TemporaryAnimatedSprite("TileSheets\\debris", new Rectangle(Game1.random.Next(2) * 16 /*0x10*/, 64 /*0x40*/, 16 /*0x10*/, 16 /*0x10*/), 9999f, 1, 999, clickableComponent + new Vector2(32f, 32f), false, false)
            {
              alphaFade = 0.025f,
              motion = new Vector2((float) Game1.random.Next(-3, 4), -4f),
              acceleration = new Vector2(0.0f, 0.5f),
              delayBeforeAnimationStart = index * 25,
              scale = 2f
            });
            this.animations.Add(new TemporaryAnimatedSprite("TileSheets\\debris", new Rectangle(Game1.random.Next(2) * 16 /*0x10*/, 64 /*0x40*/, 16 /*0x10*/, 16 /*0x10*/), 9999f, 1, 999, clickableComponent + new Vector2(32f, 32f), false, false)
            {
              scale = 4f,
              alphaFade = 0.025f,
              delayBeforeAnimationStart = index * 50,
              motion = Utility.getVelocityTowardPoint(new Point((int) clickableComponent.X + 32 /*0x20*/, (int) clickableComponent.Y + 32 /*0x20*/), new Vector2((float) (this.xPositionOnScreen - 36), (float) (this.yPositionOnScreen + this.height - this.inventory.height - 16 /*0x10*/)), 8f),
              acceleration = Utility.getVelocityTowardPoint(new Point((int) clickableComponent.X + 32 /*0x20*/, (int) clickableComponent.Y + 32 /*0x20*/), new Vector2((float) (this.xPositionOnScreen - 36), (float) (this.yPositionOnScreen + this.height - this.inventory.height - 16 /*0x10*/)), 0.5f)
            });
          }
          ISalable salable2;
          if (key != null && this.buyBackItemsToResellTomorrow.TryGetValue(key, out salable2))
            salable2.Stack += stack;
          else if (salable1 is StardewValley.Object @object && @object.edibility.Value != -300 && Game1.random.NextDouble() < 0.039999999105930328 && Game1.currentLocation is ShopLocation currentLocation)
          {
            ISalable salableInstance = salable1.GetSalableInstance();
            if (key != null)
              this.buyBackItemsToResellTomorrow[key] = salableInstance;
            currentLocation.itemsToStartSellingTomorrow.Add(salableInstance as Item);
          }
          if (this.inventory.getItemAt(x, y) == null)
          {
            Game1.playSound("sell");
            this.animations.Add(new TemporaryAnimatedSprite(5, clickableComponent + new Vector2(32f, 32f), Color.White)
            {
              motion = new Vector2(0.0f, -0.5f)
            });
          }
        }
      }
    }
    else
      this.heldItem = (ISalable) this.inventory.rightClick(x, y, this.heldItem as Item);
    for (int index1 = 0; index1 < this.forSaleButtons.Count; ++index1)
    {
      if (this.currentItemIndex + index1 < this.forSale.Count && this.forSaleButtons[index1].containsPoint(x, y))
      {
        int index2 = this.currentItemIndex + index1;
        if (this.forSale[index2] == null)
          break;
        int stockToBuy = 1;
        if (this.itemPriceAndStock[this.forSale[index2]].Price > 0)
          stockToBuy = Game1.oldKBState.IsKeyDown(Keys.LeftShift) ? Math.Min(Math.Min(Game1.oldKBState.IsKeyDown(Keys.LeftControl) ? (Game1.oldKBState.IsKeyDown(Keys.OemTilde) ? 999 : 25) : 5, ShopMenu.getPlayerCurrencyAmount(Game1.player, this.currency) / this.itemPriceAndStock[this.forSale[index2]].Price), this.itemPriceAndStock[this.forSale[index2]].Stock) : 1;
        if (this.canPurchaseCheck != null && !this.canPurchaseCheck(index2))
          break;
        if (stockToBuy > 0 && this.tryToPurchaseItem(this.forSale[index2], this.heldItem, stockToBuy, x, y))
        {
          this.itemPriceAndStock.Remove(this.forSale[index2]);
          this.forSale.RemoveAt(index2);
        }
        if (this.heldItem != null && (this._isStorageShop || Game1.options.SnappyMenus) && Game1.activeClickableMenu is ShopMenu && Game1.player.addItemToInventoryBool(this.heldItem as Item))
        {
          this.heldItem = (ISalable) null;
          DelayedAction.playSoundAfterDelay("coin", 100);
        }
        this.setScrollBarToCurrentIndex();
        break;
      }
    }
  }

  /// <inheritdoc />
  public override void performHoverAction(int x, int y)
  {
    base.performHoverAction(x, y);
    this.hoverText = "";
    this.hoveredItem = (ISalable) null;
    this.hoverPrice = -1;
    this.boldTitleText = "";
    this.upArrow.tryHover(x, y);
    this.downArrow.tryHover(x, y);
    this.scrollBar.tryHover(x, y);
    if (this.scrolling)
      return;
    for (int index = 0; index < this.forSaleButtons.Count; ++index)
    {
      if (this.currentItemIndex + index < this.forSale.Count && this.forSaleButtons[index].containsPoint(x, y))
      {
        ISalable key = this.forSale[this.currentItemIndex + index];
        if (this.canPurchaseCheck == null || this.canPurchaseCheck(this.currentItemIndex + index))
        {
          this.hoverText = key.getDescription();
          this.boldTitleText = key.DisplayName;
          if (!this._isStorageShop)
          {
            ItemStockInformation stockInformation;
            this.hoverPrice = this.itemPriceAndStock == null || !this.itemPriceAndStock.TryGetValue(key, out stockInformation) ? key.salePrice() : stockInformation.Price;
          }
          this.hoveredItem = key;
          this.forSaleButtons[index].scale = Math.Min(this.forSaleButtons[index].scale + 0.03f, 1.1f);
        }
      }
      else
        this.forSaleButtons[index].scale = Math.Max(1f, this.forSaleButtons[index].scale - 0.03f);
    }
    if (this.heldItem != null)
      return;
    foreach (ClickableComponent c in this.inventory.inventory)
    {
      if (c.containsPoint(x, y))
      {
        Item clickableComponent = this.inventory.getItemFromClickableComponent(c);
        if (clickableComponent != null && (this.inventory.highlightMethod == null || this.inventory.highlightMethod(clickableComponent)))
        {
          if (this._isStorageShop)
          {
            this.hoverText = clickableComponent.getDescription();
            this.boldTitleText = clickableComponent.DisplayName;
            this.hoveredItem = (ISalable) clickableComponent;
          }
          else
          {
            this.hoverText = $"{clickableComponent.DisplayName} x{clickableComponent.Stack.ToString()}";
            if (clickableComponent is StardewValley.Object @object && @object.needsToBeDonated())
              this.hoverText = $"{this.hoverText}\n\n{clickableComponent.getDescription()}\n";
            this.hoverPrice = (int) ((double) clickableComponent.sellToStorePrice(-1L) * (double) this.sellPercentage) * clickableComponent.Stack;
          }
        }
      }
    }
  }

  /// <inheritdoc />
  public override void update(GameTime time)
  {
    base.update(time);
    if (this.safetyTimer > 0)
      this.safetyTimer -= time.ElapsedGameTime.Milliseconds;
    if (this.poof != null && this.poof.update(time))
      this.poof = (TemporaryAnimatedSprite) null;
    this.repositionTabs();
  }

  public void drawCurrency(SpriteBatch b)
  {
    if (this._isStorageShop || this.currency != 0)
      return;
    Game1.dayTimeMoneyBox.drawMoneyBox(b, this.xPositionOnScreen - 36, this.yPositionOnScreen + this.height - this.inventory.height - 12);
  }

  /// <inheritdoc />
  public override void receiveGamePadButton(Buttons button)
  {
    base.receiveGamePadButton(button);
    if (button != Buttons.RightTrigger && button != Buttons.LeftTrigger)
      return;
    ClickableComponent snappedComponent = this.currentlySnappedComponent;
    if ((snappedComponent != null ? (snappedComponent.myID >= 3546 ? 1 : 0) : 0) != 0)
    {
      int num = -1;
      for (int index = 0; index < 12; ++index)
      {
        this.inventory.inventory[index].upNeighborID = 3546 + this.forSaleButtons.Count - 1;
        if (num == -1 && this.heldItem != null)
        {
          IList<Item> actualInventory = this.inventory.actualInventory;
          if ((actualInventory != null ? (actualInventory.Count > index ? 1 : 0) : 0) != 0 && this.inventory.actualInventory[index] == null)
            num = index;
        }
      }
      this.currentlySnappedComponent = this.getComponentWithID(num != -1 ? num : 0);
      this.snapCursorToCurrentSnappedComponent();
    }
    else
      this.snapToDefaultClickableComponent();
    Game1.playSound("shiny4");
  }

  private string getHoveredItemExtraItemIndex()
  {
    ItemStockInformation stockInformation;
    return this.hoveredItem != null && this.itemPriceAndStock != null && this.itemPriceAndStock.TryGetValue(this.hoveredItem, out stockInformation) && stockInformation.TradeItem != null ? stockInformation.TradeItem : (string) null;
  }

  private int getHoveredItemExtraItemAmount()
  {
    ItemStockInformation stockInformation;
    return this.hoveredItem != null && this.itemPriceAndStock != null && this.itemPriceAndStock.TryGetValue(this.hoveredItem, out stockInformation) && stockInformation.TradeItem != null && stockInformation.TradeItemCount.HasValue ? stockInformation.TradeItemCount.Value : 5;
  }

  public void updatePosition()
  {
    this.width = 1000 + IClickableMenu.borderWidth * 2;
    this.height = 600 + IClickableMenu.borderWidth * 2;
    this.xPositionOnScreen = Game1.uiViewport.Width / 2 - (800 + IClickableMenu.borderWidth * 2) / 2;
    this.yPositionOnScreen = Game1.uiViewport.Height / 2 - (600 + IClickableMenu.borderWidth * 2) / 2;
    int num = this.xPositionOnScreen - 320;
    if ((this.portraitTexture != null ? 1 : (!string.IsNullOrEmpty(this.potraitPersonDialogue) ? 1 : 0)) != 0 && num > 0 && Game1.options.showMerchantPortraits)
      return;
    this.xPositionOnScreen = Game1.uiViewport.Width / 2 - (1000 + IClickableMenu.borderWidth * 2) / 2;
    this.yPositionOnScreen = Game1.uiViewport.Height / 2 - (600 + IClickableMenu.borderWidth * 2) / 2;
  }

  /// <inheritdoc />
  public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
  {
    ShopMenu.ShopCachedTheme visualTheme = this.VisualTheme;
    this.updatePosition();
    this.initializeUpperRightCloseButton();
    Game1.player.forceCanMove();
    this.inventory = new InventoryMenu(this.xPositionOnScreen + this.width, this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + IClickableMenu.borderWidth + 320 + 40, false, highlightMethod: new InventoryMenu.highlightThisItem(this.highlightItemToSell))
    {
      showGrayedOutSlots = true
    };
    this.inventory.movePosition(-this.inventory.width - 32 /*0x20*/, 0);
    this.upArrow = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + this.width + 16 /*0x10*/, this.yPositionOnScreen + 16 /*0x10*/, 44, 48 /*0x30*/), visualTheme.ScrollUpTexture, visualTheme.ScrollUpSourceRect, 4f);
    this.downArrow = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + this.width + 16 /*0x10*/, this.yPositionOnScreen + this.height - 64 /*0x40*/, 44, 48 /*0x30*/), visualTheme.ScrollDownTexture, visualTheme.ScrollDownSourceRect, 4f);
    this.scrollBar = new ClickableTextureComponent(new Rectangle(this.upArrow.bounds.X + 12, this.upArrow.bounds.Y + this.upArrow.bounds.Height + 4, 24, 40), visualTheme.ScrollBarFrontTexture, visualTheme.ScrollBarFrontSourceRect, 4f);
    this.scrollBarRunner = new Rectangle(this.scrollBar.bounds.X, this.upArrow.bounds.Y + this.upArrow.bounds.Height + 4, this.scrollBar.bounds.Width, this.height - 64 /*0x40*/ - this.upArrow.bounds.Height - 28);
    this.forSaleButtons.Clear();
    for (int index = 0; index < 4; ++index)
      this.forSaleButtons.Add(new ClickableComponent(new Rectangle(this.xPositionOnScreen + 16 /*0x10*/, this.yPositionOnScreen + 16 /*0x10*/ + index * ((this.height - 256 /*0x0100*/) / 4), this.width - 32 /*0x20*/, (this.height - 256 /*0x0100*/) / 4 + 4), index.ToString() ?? ""));
    if (this.tabButtons.Count > 0)
    {
      foreach (ClickableComponent forSaleButton in this.forSaleButtons)
        forSaleButton.leftNeighborID = -99998;
    }
    this.repositionTabs();
    foreach (ClickableComponent clickableComponent in this.inventory.GetBorder(InventoryMenu.BorderSide.Top))
      clickableComponent.upNeighborID = -99998;
  }

  public void setItemPriceAndStock(
    Dictionary<ISalable, ItemStockInformation> new_stock)
  {
    this.itemPriceAndStock = new_stock;
    this.forSale = this.itemPriceAndStock.Keys.ToList<ISalable>();
    this.applyTab();
  }

  /// <inheritdoc />
  public override void draw(SpriteBatch b)
  {
    if (!Game1.options.showMenuBackground && !Game1.options.showClearBackgrounds)
      b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
    ShopMenu.ShopCachedTheme visualTheme = this.VisualTheme;
    IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(384, 373, 18, 18), this.xPositionOnScreen + this.width - this.inventory.width - 32 /*0x20*/ - 24, this.yPositionOnScreen + this.height - 256 /*0x0100*/ + 40, this.inventory.width + 56, this.height - 448 + 20, Color.White, 4f);
    IClickableMenu.drawTextureBox(b, visualTheme.WindowBorderTexture, visualTheme.WindowBorderSourceRect, this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height - 256 /*0x0100*/ + 32 /*0x20*/ + 4, Color.White, 4f);
    this.drawCurrency(b);
    for (int index = 0; index < this.forSaleButtons.Count; ++index)
    {
      ClickableComponent forSaleButton = this.forSaleButtons[index];
      if (this.currentItemIndex + index < this.forSale.Count)
      {
        bool flag1 = this.canPurchaseCheck != null && !this.canPurchaseCheck(this.currentItemIndex + index);
        ISalable key = this.forSale[this.currentItemIndex + index];
        ItemStockInformation stockInfo = this.itemPriceAndStock[key];
        StackDrawType stackDrawType = this.GetStackDrawType(stockInfo, key);
        string s1 = key.DisplayName;
        IClickableMenu.drawTextureBox(b, visualTheme.ItemRowBackgroundTexture, visualTheme.ItemRowBackgroundSourceRect, forSaleButton.bounds.X, forSaleButton.bounds.Y, forSaleButton.bounds.Width, forSaleButton.bounds.Height, !forSaleButton.containsPoint(Game1.getOldMouseX(), Game1.getOldMouseY()) || this.scrolling ? Color.White : visualTheme.ItemRowBackgroundHoverColor, 4f, false);
        if (key.Stack > 1)
          s1 = $"{s1} x{key.Stack.ToString()}";
        if (key.ShouldDrawIcon())
        {
          b.Draw(visualTheme.ItemIconBackgroundTexture, new Vector2((float) (forSaleButton.bounds.X + 32 /*0x20*/ - 12), (float) (forSaleButton.bounds.Y + 24 - 4)), new Rectangle?(visualTheme.ItemIconBackgroundSourceRect), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
          Vector2 location = new Vector2((float) (forSaleButton.bounds.X + 32 /*0x20*/ - 8), (float) (forSaleButton.bounds.Y + 24));
          Color color = Color.White * (!flag1 ? 1f : 0.25f);
          int stock = stockInfo.Stock;
          key.drawInMenu(b, location, 1f, 1f, 0.9f, StackDrawType.HideButShowQuality, color, true);
          if (stock != int.MaxValue && this.ShopId != "ClintUpgrade" && (stackDrawType == StackDrawType.Draw && stock > 1 || stackDrawType == StackDrawType.Draw_OneInclusive))
            Utility.drawTinyDigits(stock, b, location + new Vector2((float) (64 /*0x40*/ - Utility.getWidthOfTinyDigitString(stock, 3f) + 3), 47f), 3f, 1f, color);
          if (this.buyBackItems.Contains(key))
            b.Draw(Game1.mouseCursors2, new Vector2((float) (forSaleButton.bounds.X + 32 /*0x20*/ - 8), (float) (forSaleButton.bounds.Y + 24)), new Rectangle?(new Rectangle(64 /*0x40*/, 240 /*0xF0*/, 16 /*0x10*/, 16 /*0x10*/)), Color.White * (!flag1 ? 1f : 0.25f), 0.0f, new Vector2(8f, 8f), 4f, SpriteEffects.None, 1f);
          string s2 = s1;
          bool flag2 = stockInfo.Price > 0;
          if (SpriteText.getWidthOfString(s2) > this.width - (flag2 ? 150 + SpriteText.getWidthOfString(stockInfo.Price.ToString() + " ") : 100) && s2.Length > (flag2 ? 27 : 37))
            s2 = s2.Substring(0, flag2 ? 27 : 37) + "...";
          SpriteText.drawString(b, s2, forSaleButton.bounds.X + 96 /*0x60*/ + 8, forSaleButton.bounds.Y + 28, alpha: flag1 ? 0.5f : 1f, color: visualTheme.ItemRowTextColor);
        }
        else
          SpriteText.drawString(b, s1, forSaleButton.bounds.X + 32 /*0x20*/ + 8, forSaleButton.bounds.Y + 28, alpha: flag1 ? 0.5f : 1f, color: visualTheme.ItemRowTextColor);
        int right = forSaleButton.bounds.Right;
        int y1 = forSaleButton.bounds.Y + 28 - 4;
        int y2 = forSaleButton.bounds.Y + 44;
        if (stockInfo.Price > 0)
        {
          SpriteText.drawString(b, stockInfo.Price.ToString() + " ", right - SpriteText.getWidthOfString(stockInfo.Price.ToString() + " ") - 60, forSaleButton.bounds.Y + 28, alpha: ShopMenu.getPlayerCurrencyAmount(Game1.player, this.currency) < stockInfo.Price || flag1 ? 0.5f : 1f, color: visualTheme.ItemRowTextColor);
          Utility.drawWithShadow(b, Game1.mouseCursors, new Vector2((float) (forSaleButton.bounds.Right - 52), (float) (forSaleButton.bounds.Y + 40 - 4)), new Rectangle(193 + this.currency * 9, 373, 9, 10), Color.White * (!flag1 ? 1f : 0.25f), 0.0f, Vector2.Zero, 4f, shadowIntensity: !flag1 ? 0.35f : 0.0f);
          right -= SpriteText.getWidthOfString(stockInfo.Price.ToString() + " ") + 96 /*0x60*/;
          y1 = forSaleButton.bounds.Y + 20;
          y2 = forSaleButton.bounds.Y + 28;
        }
        if (stockInfo.TradeItem != null)
        {
          int count = 5;
          string tradeItem = stockInfo.TradeItem;
          if (tradeItem != null && stockInfo.TradeItemCount.HasValue)
            count = stockInfo.TradeItemCount.Value;
          bool flag3 = this.HasTradeItem(tradeItem, count);
          if (this.canPurchaseCheck != null && !this.canPurchaseCheck(this.currentItemIndex + index))
            flag3 = false;
          float widthOfString = (float) SpriteText.getWidthOfString("x" + count.ToString());
          ParsedItemData dataOrErrorItem = ItemRegistry.GetDataOrErrorItem(tradeItem);
          Texture2D texture = dataOrErrorItem.GetTexture();
          Rectangle sourceRect = dataOrErrorItem.GetSourceRect();
          Utility.drawWithShadow(b, texture, new Vector2((float) (right - 88) - widthOfString, (float) y1), sourceRect, Color.White * (flag3 ? 1f : 0.25f), 0.0f, Vector2.Zero, shadowIntensity: flag3 ? 0.35f : 0.0f);
          SpriteText.drawString(b, "x" + count.ToString(), right - (int) widthOfString - 16 /*0x10*/, y2, alpha: flag3 ? 1f : 0.5f, color: visualTheme.ItemRowTextColor);
        }
      }
    }
    if (this.IsOutOfStock())
    {
      string s = Game1.content.LoadString("Strings\\StringsFromCSFiles:ShopMenu.cs.11583");
      SpriteText.drawString(b, s, this.xPositionOnScreen + this.width / 2 - SpriteText.getWidthOfString(s) / 2, this.yPositionOnScreen + this.height / 2 - 128 /*0x80*/, color: (Color?) visualTheme?.ItemRowTextColor);
    }
    this.inventory.draw(b);
    for (int index = this.animations.Count - 1; index >= 0; --index)
    {
      if (this.animations[index].update(Game1.currentGameTime))
        this.animations.RemoveAt(index);
      else
        this.animations[index].draw(b, true);
    }
    this.poof?.draw(b);
    this.upArrow.draw(b);
    this.downArrow.draw(b);
    foreach (ClickableTextureComponent tabButton in this.tabButtons)
      tabButton.draw(b);
    if (this.forSale.Count > 4)
    {
      IClickableMenu.drawTextureBox(b, visualTheme.ScrollBarBackTexture, visualTheme.ScrollBarBackSourceRect, this.scrollBarRunner.X, this.scrollBarRunner.Y, this.scrollBarRunner.Width, this.scrollBarRunner.Height, Color.White, 4f);
      this.scrollBar.draw(b);
    }
    if (this.hoverText != "")
    {
      Item hoveredItem1 = this.hoveredItem as Item;
      ISalable hoveredItem2 = this.hoveredItem;
      if ((hoveredItem2 != null ? (hoveredItem2.IsRecipe ? 1 : 0) : 0) != 0)
        IClickableMenu.drawToolTip(b, " ", this.boldTitleText, hoveredItem1, this.heldItem != null, currencySymbol: this.currency, extraItemToShowIndex: this.getHoveredItemExtraItemIndex(), extraItemToShowAmount: this.getHoveredItemExtraItemAmount(), craftingIngredients: new CraftingRecipe(hoveredItem1?.BaseName ?? this.hoveredItem.Name), moneyAmountToShowAtBottom: this.hoverPrice > 0 ? this.hoverPrice : -1);
      else
        IClickableMenu.drawToolTip(b, this.hoverText, this.boldTitleText, hoveredItem1, this.heldItem != null, currencySymbol: this.currency, extraItemToShowIndex: this.getHoveredItemExtraItemIndex(), extraItemToShowAmount: this.getHoveredItemExtraItemAmount(), moneyAmountToShowAtBottom: this.hoverPrice > 0 ? this.hoverPrice : -1);
    }
    this.heldItem?.drawInMenu(b, new Vector2((float) (Game1.getOldMouseX() + 8), (float) (Game1.getOldMouseY() + 8)), 1f, 1f, 0.9f, StackDrawType.Draw, Color.White, true);
    base.draw(b);
    int x = this.xPositionOnScreen - 320;
    if (x > 0 && Game1.options.showMerchantPortraits)
    {
      if (this.portraitTexture != null)
      {
        Utility.drawWithShadow(b, visualTheme.PortraitBackgroundTexture, new Vector2((float) x, (float) this.yPositionOnScreen), visualTheme.PortraitBackgroundSourceRect, Color.White, 0.0f, Vector2.Zero, 4f, layerDepth: 0.91f);
        if (this.portraitTexture != null)
          b.Draw(this.portraitTexture, new Vector2((float) (x + 20), (float) (this.yPositionOnScreen + 20)), new Rectangle?(new Rectangle(0, 0, 64 /*0x40*/, 64 /*0x40*/)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.92f);
      }
      if (this.potraitPersonDialogue != null)
      {
        int overrideX = this.xPositionOnScreen - (int) Game1.dialogueFont.MeasureString(this.potraitPersonDialogue).X - 64 /*0x40*/;
        if (overrideX > 0)
          IClickableMenu.drawHoverText(b, this.potraitPersonDialogue, Game1.dialogueFont, overrideX: overrideX, overrideY: this.yPositionOnScreen + (this.portraitTexture != null ? 312 : 0), boxTexture: visualTheme.DialogueBackgroundTexture, boxSourceRect: new Rectangle?(visualTheme.DialogueBackgroundSourceRect), textColor: visualTheme.DialogueColor, textShadowColor: visualTheme.DialogueShadowColor);
      }
    }
    this.drawMouse(b);
  }

  /// <summary>Get how the stack size for a shop entry should be drawn.</summary>
  /// <param name="stockInfo">The shop entry's stock information.</param>
  /// <param name="item">The spawned item instance.</param>
  public StackDrawType GetStackDrawType(ItemStockInformation stockInfo, ISalable item)
  {
    if (item.IsRecipe)
      return StackDrawType.Hide;
    if (stockInfo.StackDrawType.HasValue)
      return stockInfo.StackDrawType.Value;
    if (stockInfo.Stock == int.MaxValue)
      return StackDrawType.HideButShowQuality;
    if (this.DefaultStackDrawType.HasValue)
      return this.DefaultStackDrawType.Value;
    ShopData shopData = this.ShopData;
    if ((shopData != null ? (shopData.StackSizeVisibility.HasValue ? 1 : 0) : 0) != 0)
    {
      StackSizeVisibility? stackSizeVisibility = this.ShopData.StackSizeVisibility;
      if (stackSizeVisibility.HasValue)
      {
        switch (stackSizeVisibility.GetValueOrDefault())
        {
          case StackSizeVisibility.Hide:
            return StackDrawType.HideButShowQuality;
          case StackSizeVisibility.ShowIfMultiple:
            return StackDrawType.Draw;
        }
      }
      return StackDrawType.Draw_OneInclusive;
    }
    return !this._isStorageShop ? StackDrawType.Draw_OneInclusive : StackDrawType.Draw;
  }

  /// <summary>A callback to invoke when the player purchases an item.</summary>
  /// <param name="salable">The shop entry that was purchased. The entry's <see cref="P:StardewValley.Item.Stack" /> represents the number purchased per click, *not* the number bought (which is <paramref name="countTaken" />) or remaining stock (which is <paramref name="stock" />).</param>
  /// <param name="who">The player who purchased the item.</param>
  /// <param name="countTaken">The stack size that was taken out of the shop.</param>
  /// <param name="stock">The remaining stock in the shop. The <paramref name="countTaken" /> is already deducted from this value.</param>
  /// <returns>Returns whether to immediately exit the menu.</returns>
  public delegate bool OnPurchaseDelegate(
    ISalable salable,
    Farmer who,
    int countTaken,
    ItemStockInformation stock);

  /// <summary>A cached visual theme for the <see cref="T:StardewValley.Menus.ShopMenu" />.</summary>
  public class ShopCachedTheme
  {
    /// <summary>The visual theme data from <c>Data/Shops</c>, if applicable.</summary>
    public ShopThemeData ThemeData { get; }

    /// <summary>The texture for the shop window border.</summary>
    public Texture2D WindowBorderTexture { get; }

    /// <summary>The pixel area within the <see cref="P:StardewValley.Menus.ShopMenu.ShopCachedTheme.WindowBorderSourceRect" /> for the shop window border. This should be an 18x18 pixel area.</summary>
    public Rectangle WindowBorderSourceRect { get; }

    /// <summary>The texture for the NPC portrait background.</summary>
    public Texture2D PortraitBackgroundTexture { get; }

    /// <summary>The pixel area within the <see cref="P:StardewValley.Menus.ShopMenu.ShopCachedTheme.PortraitBackgroundTexture" /> for the NPC portrait background. This should be a 74x47 pixel area.</summary>
    public Rectangle PortraitBackgroundSourceRect { get; }

    /// <summary>The texture for the NPC dialogue background.</summary>
    public Texture2D DialogueBackgroundTexture { get; }

    /// <summary>The pixel area within the <see cref="P:StardewValley.Menus.ShopMenu.ShopCachedTheme.DialogueBackgroundTexture" /> for the NPC dialogue background. This should be a 60x60 pixel area.</summary>
    public Rectangle DialogueBackgroundSourceRect { get; }

    /// <summary>The sprite text color for the dialogue text, or <c>null</c> for the default color.</summary>
    public Color? DialogueColor { get; }

    /// <summary>The sprite text shadow color for the dialogue text, or <c>null</c> for the default color.</summary>
    public Color? DialogueShadowColor { get; }

    /// <summary>The texture for the item row background.</summary>
    public Texture2D ItemRowBackgroundTexture { get; }

    /// <summary>The pixel area within the <see cref="P:StardewValley.Menus.ShopMenu.ShopCachedTheme.ItemRowBackgroundTexture" /> for the item row background. This should be a 15x15 pixel area.</summary>
    public Rectangle ItemRowBackgroundSourceRect { get; }

    /// <summary>The color tint to apply to the item row background when the cursor is hovering over it</summary>
    public Color ItemRowBackgroundHoverColor { get; }

    /// <summary>The sprite text color for the item text, or <c>null</c> for the default color.</summary>
    public Color? ItemRowTextColor { get; }

    /// <summary>The texture for the box behind the item icons.</summary>
    public Texture2D ItemIconBackgroundTexture { get; }

    /// <summary>The pixel area within the <see cref="P:StardewValley.Menus.ShopMenu.ShopCachedTheme.ItemIconBackgroundTexture" /> for the item icon background. This should be an 18x18 pixel area.</summary>
    public Rectangle ItemIconBackgroundSourceRect { get; }

    /// <summary>The texture for the scroll up icon.</summary>
    public Texture2D ScrollUpTexture { get; }

    /// <summary>The pixel area within the <see cref="P:StardewValley.Menus.ShopMenu.ShopCachedTheme.ScrollUpTexture" /> for the scroll up icon. This should be an 11x12 pixel area.</summary>
    public Rectangle ScrollUpSourceRect { get; }

    /// <summary>The texture for the scroll down icon.</summary>
    public Texture2D ScrollDownTexture { get; }

    /// <summary>The pixel area within the <see cref="P:StardewValley.Menus.ShopMenu.ShopCachedTheme.ScrollDownTexture" /> for the scroll down icon. This should be an 11x12 pixel area.</summary>
    public Rectangle ScrollDownSourceRect { get; }

    /// <summary>The texture for the scrollbar foreground texture.</summary>
    public Texture2D ScrollBarFrontTexture { get; }

    /// <summary>The pixel area within the <see cref="P:StardewValley.Menus.ShopMenu.ShopCachedTheme.ScrollBarFrontTexture" /> for the scroll foreground. This should be a 6x10 pixel area.</summary>
    public Rectangle ScrollBarFrontSourceRect { get; }

    /// <summary>The texture for the scrollbar background texture.</summary>
    public Texture2D ScrollBarBackTexture { get; }

    /// <summary>The pixel area within the <see cref="P:StardewValley.Menus.ShopMenu.ShopCachedTheme.ScrollBarBackTexture" /> for the scroll background. This should be a 6x6 pixel area.</summary>
    public Rectangle ScrollBarBackSourceRect { get; }

    /// <summary>Construct an instance.</summary>
    /// <param name="theme">The visual theme data, or <c>null</c> for the default shop theme.</param>
    public ShopCachedTheme(ShopThemeData theme)
    {
      this.ThemeData = theme;
      this.WindowBorderTexture = this.LoadThemeTexture(theme?.WindowBorderTexture, Game1.mouseCursors);
      Rectangle? nullable = (Rectangle?) theme?.WindowBorderSourceRect;
      this.WindowBorderSourceRect = nullable ?? new Rectangle(384, 373, 18, 18);
      this.PortraitBackgroundTexture = this.LoadThemeTexture(theme?.PortraitBackgroundTexture, Game1.mouseCursors);
      nullable = (Rectangle?) theme?.PortraitBackgroundSourceRect;
      this.PortraitBackgroundSourceRect = nullable ?? new Rectangle(603, 414, 74, 74);
      this.DialogueBackgroundTexture = this.LoadThemeTexture(theme?.DialogueBackgroundTexture, Game1.menuTexture);
      nullable = (Rectangle?) theme?.DialogueBackgroundSourceRect;
      this.DialogueBackgroundSourceRect = nullable ?? new Rectangle(0, 256 /*0x0100*/, 60, 60);
      this.DialogueColor = Utility.StringToColor(theme?.DialogueColor);
      this.DialogueShadowColor = Utility.StringToColor(theme?.DialogueShadowColor);
      this.ItemRowBackgroundTexture = this.LoadThemeTexture(theme?.ItemRowBackgroundTexture, Game1.mouseCursors);
      nullable = (Rectangle?) theme?.ItemRowBackgroundSourceRect;
      this.ItemRowBackgroundSourceRect = nullable ?? new Rectangle(384, 396, 15, 15);
      this.ItemRowBackgroundHoverColor = Utility.StringToColor(theme?.ItemRowBackgroundHoverColor) ?? Color.Wheat;
      this.ItemRowTextColor = Utility.StringToColor(theme?.ItemRowTextColor);
      this.ItemIconBackgroundTexture = this.LoadThemeTexture(theme?.ItemIconBackgroundTexture, Game1.mouseCursors);
      nullable = (Rectangle?) theme?.ItemIconBackgroundSourceRect;
      this.ItemIconBackgroundSourceRect = nullable ?? new Rectangle(296, 363, 18, 18);
      this.ScrollUpTexture = this.LoadThemeTexture(theme?.ScrollUpTexture, Game1.mouseCursors);
      nullable = (Rectangle?) theme?.ScrollUpSourceRect;
      this.ScrollUpSourceRect = nullable ?? new Rectangle(421, 459, 11, 12);
      this.ScrollDownTexture = this.LoadThemeTexture(theme?.ScrollDownTexture, Game1.mouseCursors);
      nullable = (Rectangle?) theme?.ScrollDownSourceRect;
      this.ScrollDownSourceRect = nullable ?? new Rectangle(421, 472, 11, 12);
      this.ScrollBarFrontTexture = this.LoadThemeTexture(theme?.ScrollBarFrontTexture, Game1.mouseCursors);
      nullable = (Rectangle?) theme?.ScrollBarFrontSourceRect;
      this.ScrollBarFrontSourceRect = nullable ?? new Rectangle(435, 463, 6, 10);
      this.ScrollBarBackTexture = this.LoadThemeTexture(theme?.ScrollBarBackTexture, Game1.mouseCursors);
      nullable = (Rectangle?) theme?.ScrollBarBackSourceRect;
      this.ScrollBarBackSourceRect = nullable ?? new Rectangle(403, 383, 6, 6);
    }

    /// <summary>Load a theme texture if it's non-null and exists, else get the default texture.</summary>
    /// <param name="customTextureName">The custom texture asset name to load.</param>
    /// <param name="defaultTexture">The default texture.</param>
    private Texture2D LoadThemeTexture(string customTextureName, Texture2D defaultTexture)
    {
      return customTextureName == null || !Game1.content.DoesAssetExist<Texture2D>(customTextureName) ? defaultTexture : Game1.content.Load<Texture2D>(customTextureName);
    }
  }

  /// <summary>A clickable component representing a shop tab, which applies a filter to the list of displayed shop items when clicked.</summary>
  public class ShopTabClickableTextureComponent : ClickableTextureComponent
  {
    /// <summary>Matches items to show when this tab is selected.</summary>
    public Func<ISalable, bool> Filter;

    public ShopTabClickableTextureComponent(
      string name,
      Rectangle bounds,
      string label,
      string hoverText,
      Texture2D texture,
      Rectangle sourceRect,
      float scale,
      bool drawShadow = false)
      : base(name, bounds, label, hoverText, texture, sourceRect, scale, drawShadow)
    {
    }

    public ShopTabClickableTextureComponent(
      Rectangle bounds,
      Texture2D texture,
      Rectangle sourceRect,
      float scale,
      bool drawShadow = false)
      : base(bounds, texture, sourceRect, scale, drawShadow)
    {
    }
  }
}
