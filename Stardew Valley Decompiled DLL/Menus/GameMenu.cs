// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.GameMenu
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley.Locations;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace StardewValley.Menus;

public class GameMenu : IClickableMenu
{
  public static readonly int inventoryTab = 0;
  public static readonly int skillsTab = 1;
  public static readonly int socialTab = 2;
  public static readonly int mapTab = 3;
  public static readonly int craftingTab = 4;
  public static readonly int animalsTab = 5;
  public static readonly int powersTab = 6;
  public static readonly int collectionsTab = 7;
  public static readonly int optionsTab = 8;
  public static readonly int exitTab = 9;
  public const int region_inventoryTab = 12340;
  public const int region_skillsTab = 12341;
  public const int region_socialTab = 12342;
  public const int region_mapTab = 12343;
  public const int region_craftingTab = 12344;
  public const int region_animalsTab = 12345;
  public const int region_powersTab = 12346;
  public const int region_collectionsTab = 12347;
  public const int region_optionsTab = 12348;
  public const int region_exitTab = 12349;
  public static readonly int numberOfTabs = 9;
  public int currentTab;
  public int lastOpenedNonMapTab = GameMenu.inventoryTab;
  public string hoverText = "";
  public string descriptionText = "";
  public List<ClickableComponent> tabs = new List<ClickableComponent>();
  public List<IClickableMenu> pages = new List<IClickableMenu>();
  public bool invisible;
  public static bool forcePreventClose;
  public static bool bundleItemHovered;
  /// <summary>The translation keys for tab names.</summary>
  private static readonly Dictionary<int, string> TabTranslationKeys = new Dictionary<int, string>()
  {
    [GameMenu.inventoryTab] = "Strings\\UI:GameMenu_Inventory",
    [GameMenu.skillsTab] = "Strings\\UI:GameMenu_Skills",
    [GameMenu.socialTab] = "Strings\\UI:GameMenu_Social",
    [GameMenu.mapTab] = "Strings\\UI:GameMenu_Map",
    [GameMenu.craftingTab] = "Strings\\UI:GameMenu_Crafting",
    [GameMenu.powersTab] = "Strings\\1_6_Strings:GameMenu_Powers",
    [GameMenu.exitTab] = "Strings\\UI:GameMenu_Exit",
    [GameMenu.collectionsTab] = "Strings\\UI:GameMenu_Collections",
    [GameMenu.optionsTab] = "Strings\\UI:GameMenu_Options",
    [GameMenu.exitTab] = "Strings\\UI:GameMenu_Exit"
  };

  public GameMenu(bool playOpeningSound = true)
    : base(Game1.uiViewport.Width / 2 - (800 + IClickableMenu.borderWidth * 2) / 2, Game1.uiViewport.Height / 2 - (600 + IClickableMenu.borderWidth * 2) / 2, 800 + IClickableMenu.borderWidth * 2, 600 + IClickableMenu.borderWidth * 2, true)
  {
    this.tabs.Add(new ClickableComponent(new Rectangle(this.xPositionOnScreen + 64 /*0x40*/, this.yPositionOnScreen + IClickableMenu.tabYPositionRelativeToMenuY + 64 /*0x40*/, 64 /*0x40*/, 64 /*0x40*/), "inventory", Game1.content.LoadString("Strings\\UI:GameMenu_Inventory"))
    {
      myID = 12340,
      downNeighborID = 0,
      rightNeighborID = 12341,
      tryDefaultIfNoDownNeighborExists = true,
      fullyImmutable = true
    });
    this.pages.Add((IClickableMenu) new InventoryPage(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height));
    this.tabs.Add(new ClickableComponent(new Rectangle(this.xPositionOnScreen + 128 /*0x80*/, this.yPositionOnScreen + IClickableMenu.tabYPositionRelativeToMenuY + 64 /*0x40*/, 64 /*0x40*/, 64 /*0x40*/), "skills", Game1.content.LoadString("Strings\\UI:GameMenu_Skills"))
    {
      myID = 12341,
      downNeighborID = 1,
      rightNeighborID = 12342,
      leftNeighborID = 12340,
      tryDefaultIfNoDownNeighborExists = true,
      fullyImmutable = true
    });
    this.pages.Add((IClickableMenu) new SkillsPage(this.xPositionOnScreen, this.yPositionOnScreen, this.width + (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ru || LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.it ? 64 /*0x40*/ : 0), this.height));
    this.tabs.Add(new ClickableComponent(new Rectangle(this.xPositionOnScreen + 192 /*0xC0*/, this.yPositionOnScreen + IClickableMenu.tabYPositionRelativeToMenuY + 64 /*0x40*/, 64 /*0x40*/, 64 /*0x40*/), "social", Game1.content.LoadString("Strings\\UI:GameMenu_Social"))
    {
      myID = 12342,
      downNeighborID = 2,
      rightNeighborID = 12343,
      leftNeighborID = 12341,
      tryDefaultIfNoDownNeighborExists = true,
      fullyImmutable = true
    });
    this.pages.Add((IClickableMenu) new SocialPage(this.xPositionOnScreen, this.yPositionOnScreen, this.width + 36, this.height));
    this.tabs.Add(new ClickableComponent(new Rectangle(this.xPositionOnScreen + 256 /*0x0100*/, this.yPositionOnScreen + IClickableMenu.tabYPositionRelativeToMenuY + 64 /*0x40*/, 64 /*0x40*/, 64 /*0x40*/), "map", Game1.content.LoadString("Strings\\UI:GameMenu_Map"))
    {
      myID = 12343,
      downNeighborID = 3,
      rightNeighborID = 12344,
      leftNeighborID = 12342,
      tryDefaultIfNoDownNeighborExists = true,
      fullyImmutable = true
    });
    this.pages.Add((IClickableMenu) new MapPage(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height));
    this.tabs.Add(new ClickableComponent(new Rectangle(this.xPositionOnScreen + 320, this.yPositionOnScreen + IClickableMenu.tabYPositionRelativeToMenuY + 64 /*0x40*/, 64 /*0x40*/, 64 /*0x40*/), "crafting", Game1.content.LoadString("Strings\\UI:GameMenu_Crafting"))
    {
      myID = 12344,
      downNeighborID = 4,
      rightNeighborID = 12345,
      leftNeighborID = 12343,
      tryDefaultIfNoDownNeighborExists = true,
      fullyImmutable = true
    });
    this.pages.Add((IClickableMenu) new CraftingPage(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height));
    this.tabs.Add(new ClickableComponent(new Rectangle(this.xPositionOnScreen + 384, this.yPositionOnScreen + IClickableMenu.tabYPositionRelativeToMenuY + 64 /*0x40*/, 64 /*0x40*/, 64 /*0x40*/), "animals", Game1.content.LoadString("Strings\\1_6_Strings:GameMenu_Animals"))
    {
      myID = 12345,
      downNeighborID = 5,
      rightNeighborID = 12346,
      leftNeighborID = 12344,
      tryDefaultIfNoDownNeighborExists = true,
      fullyImmutable = true
    });
    this.pages.Add((IClickableMenu) new AnimalPage(this.xPositionOnScreen, this.yPositionOnScreen, this.width - 64 /*0x40*/ - 16 /*0x10*/, this.height));
    this.tabs.Add(new ClickableComponent(new Rectangle(this.xPositionOnScreen + 448, this.yPositionOnScreen + IClickableMenu.tabYPositionRelativeToMenuY + 64 /*0x40*/, 64 /*0x40*/, 64 /*0x40*/), "powers", Game1.content.LoadString("Strings\\1_6_Strings:GameMenu_Powers"))
    {
      myID = 12346,
      downNeighborID = 6,
      rightNeighborID = 12347,
      leftNeighborID = 12345,
      tryDefaultIfNoDownNeighborExists = true,
      fullyImmutable = true
    });
    this.pages.Add((IClickableMenu) new PowersTab(this.xPositionOnScreen, this.yPositionOnScreen, this.width - 64 /*0x40*/ - 16 /*0x10*/, this.height));
    this.tabs.Add(new ClickableComponent(new Rectangle(this.xPositionOnScreen + 512 /*0x0200*/, this.yPositionOnScreen + IClickableMenu.tabYPositionRelativeToMenuY + 64 /*0x40*/, 64 /*0x40*/, 64 /*0x40*/), "collections", Game1.content.LoadString("Strings\\UI:GameMenu_Collections"))
    {
      myID = 12347,
      downNeighborID = 7,
      rightNeighborID = 12348,
      leftNeighborID = 12346,
      tryDefaultIfNoDownNeighborExists = true,
      fullyImmutable = true
    });
    this.pages.Add((IClickableMenu) new CollectionsPage(this.xPositionOnScreen, this.yPositionOnScreen, this.width - 64 /*0x40*/ - 16 /*0x10*/, this.height));
    this.tabs.Add(new ClickableComponent(new Rectangle(this.xPositionOnScreen + 576, this.yPositionOnScreen + IClickableMenu.tabYPositionRelativeToMenuY + 64 /*0x40*/, 64 /*0x40*/, 64 /*0x40*/), "options", Game1.content.LoadString("Strings\\UI:GameMenu_Options"))
    {
      myID = 12348,
      downNeighborID = 8,
      rightNeighborID = 12349,
      leftNeighborID = 12347,
      tryDefaultIfNoDownNeighborExists = true,
      fullyImmutable = true
    });
    int num;
    switch (LocalizedContentManager.CurrentLanguageCode)
    {
      case LocalizedContentManager.LanguageCode.ru:
        num = 96 /*0x60*/;
        break;
      case LocalizedContentManager.LanguageCode.fr:
      case LocalizedContentManager.LanguageCode.tr:
        num = 192 /*0xC0*/;
        break;
      default:
        num = 0;
        break;
    }
    this.pages.Add((IClickableMenu) new OptionsPage(this.xPositionOnScreen, this.yPositionOnScreen, this.width + num, this.height));
    this.tabs.Add(new ClickableComponent(new Rectangle(this.xPositionOnScreen + 640, this.yPositionOnScreen + IClickableMenu.tabYPositionRelativeToMenuY + 64 /*0x40*/, 64 /*0x40*/, 64 /*0x40*/), "exit", Game1.content.LoadString("Strings\\UI:GameMenu_Exit"))
    {
      myID = 12349,
      downNeighborID = 9,
      leftNeighborID = 12348,
      tryDefaultIfNoDownNeighborExists = true,
      fullyImmutable = true
    });
    this.pages.Add((IClickableMenu) new ExitPage(this.xPositionOnScreen, this.yPositionOnScreen, this.width - 64 /*0x40*/ - 16 /*0x10*/, this.height));
    if (Game1.activeClickableMenu == null & playOpeningSound)
      Game1.playSound("bigSelect");
    GameMenu.forcePreventClose = false;
    Game1.RequireLocation<CommunityCenter>("CommunityCenter").refreshBundlesIngredientsInfo();
    this.pages[this.currentTab].populateClickableComponentList();
    this.AddTabsToClickableComponents(this.pages[this.currentTab]);
    if (!Game1.options.SnappyMenus)
      return;
    this.snapToDefaultClickableComponent();
  }

  public void AddTabsToClickableComponents(IClickableMenu menu)
  {
    menu.allClickableComponents.AddRange((IEnumerable<ClickableComponent>) this.tabs);
  }

  public GameMenu(int startingTab, int extra = -1, bool playOpeningSound = true)
    : this(playOpeningSound)
  {
    this.changeTab(startingTab, false);
    if (startingTab != GameMenu.optionsTab || extra == -1)
      return;
    (this.pages[GameMenu.optionsTab] as OptionsPage).currentItemIndex = extra;
  }

  public override void automaticSnapBehavior(int direction, int oldRegion, int oldID)
  {
    if (this.GetCurrentPage() != null)
      this.GetCurrentPage().automaticSnapBehavior(direction, oldRegion, oldID);
    else
      base.automaticSnapBehavior(direction, oldRegion, oldID);
  }

  public override void snapToDefaultClickableComponent()
  {
    if (this.currentTab >= this.pages.Count)
      return;
    this.pages[this.currentTab].snapToDefaultClickableComponent();
  }

  /// <inheritdoc />
  public override void receiveGamePadButton(Buttons button)
  {
    base.receiveGamePadButton(button);
    switch (button)
    {
      case Buttons.RightTrigger:
        if (this.currentTab == GameMenu.mapTab)
        {
          Game1.activeClickableMenu = (IClickableMenu) new GameMenu(GameMenu.mapTab + 1);
          Game1.playSound("smallSelect");
          break;
        }
        if (this.currentTab >= GameMenu.numberOfTabs || !this.pages[this.currentTab].readyToClose())
          break;
        this.changeTab(this.currentTab + 1);
        break;
      case Buttons.LeftTrigger:
        if (this.currentTab == GameMenu.mapTab)
        {
          Game1.activeClickableMenu = (IClickableMenu) new GameMenu(GameMenu.mapTab - 1);
          Game1.playSound("smallSelect");
          break;
        }
        if (this.currentTab <= 0 || !this.pages[this.currentTab].readyToClose())
          break;
        this.changeTab(this.currentTab - 1);
        break;
      default:
        this.pages[this.currentTab].receiveGamePadButton(button);
        break;
    }
  }

  public override void setUpForGamePadMode()
  {
    base.setUpForGamePadMode();
    if (this.pages.Count <= this.currentTab)
      return;
    this.pages[this.currentTab].setUpForGamePadMode();
  }

  public override ClickableComponent getCurrentlySnappedComponent()
  {
    return this.pages[this.currentTab].getCurrentlySnappedComponent();
  }

  public override void setCurrentlySnappedComponentTo(int id)
  {
    this.pages[this.currentTab].setCurrentlySnappedComponentTo(id);
  }

  /// <inheritdoc />
  public override void receiveLeftClick(int x, int y, bool playSound = true)
  {
    if ((this.pages[this.currentTab] is CollectionsPage page ? page.letterviewerSubMenu : (LetterViewerMenu) null) == null)
      base.receiveLeftClick(x, y, playSound);
    if (!this.invisible && !GameMenu.forcePreventClose)
    {
      for (int index = 0; index < this.tabs.Count; ++index)
      {
        if (this.tabs[index].containsPoint(x, y) && this.currentTab != index && this.pages[this.currentTab].readyToClose())
        {
          this.changeTab(this.getTabNumberFromName(this.tabs[index].name));
          return;
        }
      }
    }
    this.pages[this.currentTab].receiveLeftClick(x, y);
  }

  public static string getLabelOfTabFromIndex(int index)
  {
    string path;
    return !GameMenu.TabTranslationKeys.TryGetValue(index, out path) ? "" : Game1.content.LoadString(path);
  }

  /// <inheritdoc />
  public override void receiveRightClick(int x, int y, bool playSound = true)
  {
    this.pages[this.currentTab].receiveRightClick(x, y);
  }

  /// <inheritdoc />
  public override void receiveScrollWheelAction(int direction)
  {
    base.receiveScrollWheelAction(direction);
    this.pages[this.currentTab].receiveScrollWheelAction(direction);
  }

  /// <inheritdoc />
  public override void performHoverAction(int x, int y)
  {
    base.performHoverAction(x, y);
    this.hoverText = "";
    this.pages[this.currentTab].performHoverAction(x, y);
    foreach (ClickableComponent tab in this.tabs)
    {
      if (tab.containsPoint(x, y))
      {
        this.hoverText = tab.label;
        break;
      }
    }
  }

  public int getTabNumberFromName(string name)
  {
    int tabNumberFromName = -1;
    if (name != null)
    {
      switch (name.Length)
      {
        case 3:
          if (name == "map")
          {
            tabNumberFromName = GameMenu.mapTab;
            break;
          }
          break;
        case 4:
          if (name == "exit")
          {
            tabNumberFromName = GameMenu.exitTab;
            break;
          }
          break;
        case 6:
          switch (name[2])
          {
            case 'c':
              if (name == "social")
              {
                tabNumberFromName = GameMenu.socialTab;
                break;
              }
              break;
            case 'i':
              if (name == "skills")
              {
                tabNumberFromName = GameMenu.skillsTab;
                break;
              }
              break;
            case 'w':
              if (name == "powers")
              {
                tabNumberFromName = GameMenu.powersTab;
                break;
              }
              break;
          }
          break;
        case 7:
          switch (name[0])
          {
            case 'a':
              if (name == "animals")
              {
                tabNumberFromName = GameMenu.animalsTab;
                break;
              }
              break;
            case 'o':
              if (name == "options")
              {
                tabNumberFromName = GameMenu.optionsTab;
                break;
              }
              break;
          }
          break;
        case 8:
          if (name == "crafting")
          {
            tabNumberFromName = GameMenu.craftingTab;
            break;
          }
          break;
        case 9:
          if (name == "inventory")
          {
            tabNumberFromName = GameMenu.inventoryTab;
            break;
          }
          break;
        case 11:
          if (name == "collections")
          {
            tabNumberFromName = GameMenu.collectionsTab;
            break;
          }
          break;
      }
    }
    return tabNumberFromName;
  }

  /// <inheritdoc />
  public override void update(GameTime time)
  {
    base.update(time);
    this.pages[this.currentTab].update(time);
  }

  /// <inheritdoc />
  public override void releaseLeftClick(int x, int y)
  {
    base.releaseLeftClick(x, y);
    this.pages[this.currentTab].releaseLeftClick(x, y);
  }

  /// <inheritdoc />
  public override void leftClickHeld(int x, int y)
  {
    base.leftClickHeld(x, y);
    this.pages[this.currentTab].leftClickHeld(x, y);
  }

  public override bool readyToClose()
  {
    return !GameMenu.forcePreventClose && this.pages[this.currentTab].readyToClose();
  }

  public void changeTab(int whichTab, bool playSound = true)
  {
    this.currentTab = this.getTabNumberFromName(this.tabs[whichTab].name);
    if (this.currentTab == GameMenu.mapTab)
    {
      this.invisible = true;
      this.width += 128 /*0x80*/;
      this.initializeUpperRightCloseButton();
    }
    else
    {
      this.lastOpenedNonMapTab = this.currentTab;
      this.width = 800 + IClickableMenu.borderWidth * 2;
      this.initializeUpperRightCloseButton();
      this.invisible = false;
    }
    if (playSound)
      Game1.playSound("smallSelect");
    this.pages[this.currentTab].populateClickableComponentList();
    this.AddTabsToClickableComponents(this.pages[this.currentTab]);
    this.setTabNeighborsForCurrentPage();
    if (!Game1.options.SnappyMenus)
      return;
    this.snapToDefaultClickableComponent();
  }

  public IClickableMenu GetCurrentPage()
  {
    return this.currentTab >= this.pages.Count || this.currentTab < 0 ? (IClickableMenu) null : this.pages[this.currentTab];
  }

  public void setTabNeighborsForCurrentPage()
  {
    if (this.currentTab == GameMenu.inventoryTab)
    {
      for (int index = 0; index < this.tabs.Count; ++index)
        this.tabs[index].downNeighborID = index;
    }
    else if (this.currentTab == GameMenu.exitTab)
    {
      for (int index = 0; index < this.tabs.Count; ++index)
        this.tabs[index].downNeighborID = 535;
    }
    else
    {
      for (int index = 0; index < this.tabs.Count; ++index)
        this.tabs[index].downNeighborID = -99999;
    }
  }

  /// <inheritdoc />
  public override void draw(SpriteBatch b)
  {
    if (!this.invisible)
    {
      if (!Game1.options.showMenuBackground && !Game1.options.showClearBackgrounds)
        b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.4f);
      Game1.drawDialogueBox(this.xPositionOnScreen, this.yPositionOnScreen, this.pages[this.currentTab].width, this.pages[this.currentTab].height, false, true);
      b.End();
      b.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp);
      foreach (ClickableComponent tab in this.tabs)
      {
        int num = -1;
        string name = tab.name;
        if (name != null)
        {
          switch (name.Length)
          {
            case 3:
              if (name == "map")
              {
                num = 3;
                break;
              }
              break;
            case 4:
              switch (name[0])
              {
                case 'c':
                  if (name == "coop")
                  {
                    num = 1;
                    break;
                  }
                  break;
                case 'e':
                  if (name == "exit")
                  {
                    num = 7;
                    break;
                  }
                  break;
              }
              break;
            case 6:
              switch (name[2])
              {
                case 'c':
                  if (name == "social")
                  {
                    num = 2;
                    break;
                  }
                  break;
                case 'i':
                  if (name == "skills")
                  {
                    num = 1;
                    break;
                  }
                  break;
                case 'w':
                  if (name == "powers")
                  {
                    b.Draw(Game1.mouseCursors_1_6, new Vector2((float) tab.bounds.X, (float) (tab.bounds.Y + (this.currentTab == this.getTabNumberFromName(tab.name) ? 8 : 0))), new Rectangle?(new Rectangle(216, 494, 16 /*0x10*/, 16 /*0x10*/)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.0001f);
                    break;
                  }
                  break;
              }
              break;
            case 7:
              switch (name[0])
              {
                case 'a':
                  if (name == "animals")
                  {
                    b.Draw(Game1.mouseCursors_1_6, new Vector2((float) tab.bounds.X, (float) (tab.bounds.Y + (this.currentTab == this.getTabNumberFromName(tab.name) ? 8 : 0))), new Rectangle?(new Rectangle(257, 246, 16 /*0x10*/, 16 /*0x10*/)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.0001f);
                    break;
                  }
                  break;
                case 'o':
                  if (name == "options")
                  {
                    num = 6;
                    break;
                  }
                  break;
              }
              break;
            case 8:
              if (name == "crafting")
              {
                num = 4;
                break;
              }
              break;
            case 9:
              switch (name[0])
              {
                case 'c':
                  if (name == "catalogue")
                  {
                    num = 7;
                    break;
                  }
                  break;
                case 'i':
                  if (name == "inventory")
                  {
                    num = 0;
                    break;
                  }
                  break;
              }
              break;
            case 11:
              if (name == "collections")
              {
                num = 5;
                break;
              }
              break;
          }
        }
        if (num != -1)
          b.Draw(Game1.mouseCursors, new Vector2((float) tab.bounds.X, (float) (tab.bounds.Y + (this.currentTab == this.getTabNumberFromName(tab.name) ? 8 : 0))), new Rectangle?(new Rectangle(num * 16 /*0x10*/, 368, 16 /*0x10*/, 16 /*0x10*/)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.0001f);
        if (tab.name.Equals("skills"))
          Game1.player.FarmerRenderer.drawMiniPortrat(b, new Vector2((float) (tab.bounds.X + 8), (float) (tab.bounds.Y + 12 + (this.currentTab == this.getTabNumberFromName(tab.name) ? 8 : 0))), 0.00011f, 3f, 2, Game1.player);
      }
      b.End();
      b.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
      this.pages[this.currentTab].draw(b);
      if (!this.hoverText.Equals(""))
        IClickableMenu.drawHoverText(b, this.hoverText, Game1.smallFont);
    }
    else
      this.pages[this.currentTab].draw(b);
    if (!GameMenu.forcePreventClose && this.pages[this.currentTab].shouldDrawCloseButton())
      base.draw(b);
    if (Game1.options.SnappyMenus && (this.pages[this.currentTab] is CollectionsPage page ? page.letterviewerSubMenu : (LetterViewerMenu) null) != null || Game1.options.hardwareCursor)
      return;
    this.drawMouse(b, true);
  }

  public override bool areGamePadControlsImplemented() => false;

  /// <inheritdoc />
  public override void receiveKeyPress(Keys key)
  {
    if (((IEnumerable<InputButton>) Game1.options.menuButton).Contains<InputButton>(new InputButton(key)) && this.readyToClose())
    {
      Game1.exitActiveMenu();
      Game1.playSound("bigDeSelect");
    }
    this.pages[this.currentTab].receiveKeyPress(key);
  }

  public override void emergencyShutDown()
  {
    base.emergencyShutDown();
    this.pages[this.currentTab].emergencyShutDown();
  }

  /// <inheritdoc />
  protected override void cleanupBeforeExit()
  {
    base.cleanupBeforeExit();
    if (!Game1.options.optionsDirty)
      return;
    Game1.options.SaveDefaultOptions();
  }
}
