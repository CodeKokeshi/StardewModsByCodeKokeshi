// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.IClickableMenu
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley.BellsAndWhistles;
using StardewValley.Buffs;
using StardewValley.Enchantments;
using StardewValley.GameData.Objects;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Objects;
using StardewValley.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

#nullable disable
namespace StardewValley.Menus;

[InstanceStatics]
public abstract class IClickableMenu
{
  protected IClickableMenu _childMenu;
  protected IClickableMenu _parentMenu;
  public const int upperRightCloseButton_ID = 9175502;
  public const int currency_g = 0;
  public const int currency_starTokens = 1;
  public const int currency_qiCoins = 2;
  public const int currency_qiGems = 4;
  public const int greyedOutSpotIndex = 57;
  public const int presentIconIndex = 58;
  public const int itemSpotIndex = 10;
  protected string closeSound = "bigDeSelect";
  public static int borderWidth = 40;
  public static int tabYPositionRelativeToMenuY = -48;
  public static int spaceToClearTopBorder = 96 /*0x60*/;
  public static int spaceToClearSideBorder = 16 /*0x10*/;
  public const int spaceBetweenTabs = 4;
  /// <summary>The top-left X pixel position at which the menu is drawn.</summary>
  public int xPositionOnScreen;
  /// <summary>The top-left Y pixel position at which the menu is drawn.</summary>
  public int yPositionOnScreen;
  /// <summary>The pixel width of the menu.</summary>
  public int width;
  /// <summary>The pixel height of the menu.</summary>
  public int height;
  /// <summary>A callback to invoke before the menu exits.</summary>
  public Action<IClickableMenu> behaviorBeforeCleanup;
  /// <summary>A callback to invoke after the menu exits.</summary>
  public IClickableMenu.onExit exitFunction;
  /// <summary>The 'X' button to close the menu.</summary>
  public ClickableTextureComponent upperRightCloseButton;
  public bool destroy;
  protected int _dependencies;
  public List<ClickableComponent> allClickableComponents;
  public ClickableComponent currentlySnappedComponent;
  public static StringBuilder HoverTextStringBuilder = new StringBuilder();

  public Vector2 Position
  {
    get => new Vector2((float) this.xPositionOnScreen, (float) this.yPositionOnScreen);
  }

  /// <summary>Construct an instance.</summary>
  public IClickableMenu()
  {
  }

  /// <summary>Construct an instance.</summary>
  /// <param name="x">The top-left X pixel position at which to position the menu.</param>
  /// <param name="y">The top-left Y pixel position at which to position the menu.</param>
  /// <param name="width">The pixel width of the menu.</param>
  /// <param name="height">The pixel height of the menu.</param>
  /// <param name="showUpperRightCloseButton">Whether the 'X' button to close the menu should be shown.</param>
  public IClickableMenu(int x, int y, int width, int height, bool showUpperRightCloseButton = false)
  {
    Game1.mouseCursorTransparency = 1f;
    this.initialize(x, y, width, height, showUpperRightCloseButton);
    if (Game1.gameMode != (byte) 3 || Game1.player == null || Game1.eventUp)
      return;
    Game1.player.Halt();
  }

  /// <summary>Initialize the menu.</summary>
  /// <param name="x">The top-left X pixel position at which to position the menu.</param>
  /// <param name="y">The top-left Y pixel position at which to position the menu.</param>
  /// <param name="width">The pixel width of the menu.</param>
  /// <param name="height">The pixel height of the menu.</param>
  /// <param name="showUpperRightCloseButton">Whether the 'X' button to close the menu should be shown.</param>
  public void initialize(int x, int y, int width, int height, bool showUpperRightCloseButton = false)
  {
    if (Game1.player != null && !Game1.player.UsingTool && !Game1.eventUp)
      Game1.player.forceCanMove();
    this.xPositionOnScreen = x;
    this.yPositionOnScreen = y;
    this.width = width;
    this.height = height;
    if (showUpperRightCloseButton)
    {
      ClickableTextureComponent textureComponent = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + width - 36, this.yPositionOnScreen - 8, 48 /*0x30*/, 48 /*0x30*/), Game1.mouseCursors, new Rectangle(337, 494, 12, 12), 4f);
      textureComponent.myID = 9175502;
      this.upperRightCloseButton = textureComponent;
    }
    for (int index = 0; index < 4; ++index)
      Game1.directionKeyPolling[index] = 250;
  }

  public IClickableMenu GetChildMenu() => this._childMenu;

  public IClickableMenu GetParentMenu() => this._parentMenu;

  public void SetChildMenu(IClickableMenu menu)
  {
    this._childMenu = menu;
    if (this._childMenu == null)
      return;
    this._childMenu._parentMenu = this;
  }

  public void AddDependency() => ++this._dependencies;

  public void RemoveDependency()
  {
    --this._dependencies;
    if (this._dependencies > 0 || Game1.activeClickableMenu == this || TitleMenu.subMenu == this || !(this is IDisposable disposable))
      return;
    disposable.Dispose();
  }

  public bool HasDependencies() => this._dependencies > 0;

  public virtual bool areGamePadControlsImplemented() => false;

  /// <summary>Handle the player pressing a game pad button.</summary>
  /// <param name="button">The game pad button that was pressed.</param>
  public virtual void receiveGamePadButton(Buttons button)
  {
  }

  public void drawMouse(SpriteBatch b, bool ignore_transparency = false, int cursor = -1)
  {
    if (Game1.options.hardwareCursor)
      return;
    float num = Game1.mouseCursorTransparency;
    if (ignore_transparency)
      num = 1f;
    if (cursor < 0)
      cursor = !Game1.options.snappyMenus || !Game1.options.gamepadControls ? 0 : 44;
    b.Draw(Game1.mouseCursors, new Vector2((float) Game1.getMouseX(), (float) Game1.getMouseY()), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, cursor, 16 /*0x10*/, 16 /*0x10*/)), Color.White * num, 0.0f, Vector2.Zero, (float) (4.0 + (double) Game1.dialogueButtonScale / 150.0), SpriteEffects.None, 1f);
  }

  public virtual void populateClickableComponentList()
  {
    this.allClickableComponents = new List<ClickableComponent>();
    foreach (FieldInfo field in this.GetType().GetFields())
    {
      Type fieldType = field.FieldType;
      if (!fieldType.IsPrimitive && !(fieldType == typeof (string)) && field.GetCustomAttribute<SkipForClickableAggregation>() == null && !(field.DeclaringType == typeof (IClickableMenu)))
      {
        switch (field.GetValue((object) this))
        {
          case ClickableComponent clickableComponent1:
            this.allClickableComponents.Add(clickableComponent1);
            continue;
          case List<List<ClickableTextureComponent>> textureComponentListList:
            using (List<List<ClickableTextureComponent>>.Enumerator enumerator = textureComponentListList.GetEnumerator())
            {
              while (enumerator.MoveNext())
              {
                foreach (ClickableTextureComponent textureComponent in enumerator.Current)
                {
                  if (textureComponent != null)
                    this.allClickableComponents.Add((ClickableComponent) textureComponent);
                }
              }
              continue;
            }
          case InventoryMenu inventoryMenu:
            this.allClickableComponents.AddRange((IEnumerable<ClickableComponent>) inventoryMenu.inventory);
            this.allClickableComponents.Add(inventoryMenu.dropItemInvisibleButton);
            continue;
          case List<Dictionary<ClickableTextureComponent, CraftingRecipe>> dictionaryList:
            using (List<Dictionary<ClickableTextureComponent, CraftingRecipe>>.Enumerator enumerator = dictionaryList.GetEnumerator())
            {
              while (enumerator.MoveNext())
                this.allClickableComponents.AddRange((IEnumerable<ClickableComponent>) enumerator.Current.Keys);
              continue;
            }
          case Dictionary<int, List<List<ClickableTextureComponent>>> dictionary1:
            using (Dictionary<int, List<List<ClickableTextureComponent>>>.ValueCollection.Enumerator enumerator = dictionary1.Values.GetEnumerator())
            {
              while (enumerator.MoveNext())
              {
                foreach (IEnumerable<ClickableComponent> collection in enumerator.Current)
                  this.allClickableComponents.AddRange(collection);
              }
              continue;
            }
          case IDictionary dictionary2:
            if (fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof (Dictionary<,>))
            {
              Type type = typeof (ClickableComponent);
              Type[] genericArguments = fieldType.GetGenericArguments();
              Type c1 = genericArguments[0];
              Type c2 = genericArguments[1];
              if (type.IsAssignableFrom(c1) || type.IsAssignableFrom(c2))
              {
                IDictionaryEnumerator enumerator = dictionary2.GetEnumerator();
                try
                {
                  while (enumerator.MoveNext())
                  {
                    DictionaryEntry current = (DictionaryEntry) enumerator.Current;
                    if (current.Key is ClickableComponent key)
                      this.allClickableComponents.Add(key);
                    if (current.Value is ClickableComponent clickableComponent)
                      this.allClickableComponents.Add(clickableComponent);
                  }
                  continue;
                }
                finally
                {
                  if (enumerator is IDisposable disposable)
                    disposable.Dispose();
                }
              }
              else
                continue;
            }
            else
              continue;
          case IEnumerable enumerable:
            if (fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof (List<>) && typeof (ClickableComponent).IsAssignableFrom(fieldType.GetGenericArguments()[0]))
            {
              IEnumerator enumerator = enumerable.GetEnumerator();
              try
              {
                while (enumerator.MoveNext())
                {
                  if (enumerator.Current is ClickableComponent current)
                    this.allClickableComponents.Add(current);
                }
                continue;
              }
              finally
              {
                if (enumerator is IDisposable disposable)
                  disposable.Dispose();
              }
            }
            else
              continue;
          default:
            continue;
        }
      }
    }
    if (Game1.activeClickableMenu is GameMenu activeClickableMenu && this == activeClickableMenu.GetCurrentPage())
      activeClickableMenu.AddTabsToClickableComponents(this);
    if (this.upperRightCloseButton == null)
      return;
    this.allClickableComponents.Add((ClickableComponent) this.upperRightCloseButton);
  }

  public virtual void applyMovementKey(int direction)
  {
    if (this.allClickableComponents == null)
      this.populateClickableComponentList();
    this.moveCursorInDirection(direction);
  }

  /// <summary>
  /// return true if this method is overriden and a default clickablecomponent is snapped to.
  /// </summary>
  public virtual void snapToDefaultClickableComponent()
  {
  }

  public void applyMovementKey(Keys key)
  {
    if (Game1.options.doesInputListContain(Game1.options.moveUpButton, key))
      this.applyMovementKey(0);
    else if (Game1.options.doesInputListContain(Game1.options.moveRightButton, key))
      this.applyMovementKey(1);
    else if (Game1.options.doesInputListContain(Game1.options.moveDownButton, key))
    {
      this.applyMovementKey(2);
    }
    else
    {
      if (!Game1.options.doesInputListContain(Game1.options.moveLeftButton, key))
        return;
      this.applyMovementKey(3);
    }
  }

  /// <summary>Only use this if the child class overrides</summary>
  /// <param name="id"></param>
  public virtual void setCurrentlySnappedComponentTo(int id)
  {
    this.currentlySnappedComponent = this.getComponentWithID(id);
  }

  public void moveCursorInDirection(int direction)
  {
    if (this.currentlySnappedComponent == null)
    {
      List<ClickableComponent> clickableComponents = this.allClickableComponents;
      // ISSUE: explicit non-virtual call
      if ((clickableComponents != null ? (__nonvirtual (clickableComponents.Count) > 0 ? 1 : 0) : 0) != 0)
      {
        this.snapToDefaultClickableComponent();
        if (this.currentlySnappedComponent == null)
          this.currentlySnappedComponent = this.allClickableComponents[0];
      }
    }
    if (this.currentlySnappedComponent == null)
      return;
    ClickableComponent snappedComponent = this.currentlySnappedComponent;
    switch (direction)
    {
      case 0:
        switch (this.currentlySnappedComponent.upNeighborID)
        {
          case -99999:
            this.snapToDefaultClickableComponent();
            break;
          case -99998:
            this.automaticSnapBehavior(0, this.currentlySnappedComponent.region, this.currentlySnappedComponent.myID);
            break;
          case -7777:
            this.customSnapBehavior(0, this.currentlySnappedComponent.region, this.currentlySnappedComponent.myID);
            break;
          default:
            this.currentlySnappedComponent = this.getComponentWithID(this.currentlySnappedComponent.upNeighborID);
            break;
        }
        if (this.currentlySnappedComponent != null && (snappedComponent == null || snappedComponent.upNeighborID != -7777 && snappedComponent.upNeighborID != -99998) && !this.currentlySnappedComponent.downNeighborImmutable && !this.currentlySnappedComponent.fullyImmutable)
          this.currentlySnappedComponent.downNeighborID = snappedComponent.myID;
        if (this.currentlySnappedComponent == null)
        {
          this.noSnappedComponentFound(0, snappedComponent.region, snappedComponent.myID);
          break;
        }
        break;
      case 1:
        switch (this.currentlySnappedComponent.rightNeighborID)
        {
          case -99999:
            this.snapToDefaultClickableComponent();
            break;
          case -99998:
            this.automaticSnapBehavior(1, this.currentlySnappedComponent.region, this.currentlySnappedComponent.myID);
            break;
          case -7777:
            this.customSnapBehavior(1, this.currentlySnappedComponent.region, this.currentlySnappedComponent.myID);
            break;
          default:
            this.currentlySnappedComponent = this.getComponentWithID(this.currentlySnappedComponent.rightNeighborID);
            break;
        }
        if (this.currentlySnappedComponent != null && (snappedComponent == null || snappedComponent.rightNeighborID != -7777 && snappedComponent.rightNeighborID != -99998) && !this.currentlySnappedComponent.leftNeighborImmutable && !this.currentlySnappedComponent.fullyImmutable)
          this.currentlySnappedComponent.leftNeighborID = snappedComponent.myID;
        if (this.currentlySnappedComponent == null && snappedComponent.tryDefaultIfNoRightNeighborExists)
        {
          this.snapToDefaultClickableComponent();
          break;
        }
        if (this.currentlySnappedComponent == null)
        {
          this.noSnappedComponentFound(1, snappedComponent.region, snappedComponent.myID);
          break;
        }
        break;
      case 2:
        switch (this.currentlySnappedComponent.downNeighborID)
        {
          case -99999:
            this.snapToDefaultClickableComponent();
            break;
          case -99998:
            this.automaticSnapBehavior(2, this.currentlySnappedComponent.region, this.currentlySnappedComponent.myID);
            break;
          case -7777:
            this.customSnapBehavior(2, this.currentlySnappedComponent.region, this.currentlySnappedComponent.myID);
            break;
          default:
            this.currentlySnappedComponent = this.getComponentWithID(this.currentlySnappedComponent.downNeighborID);
            break;
        }
        if (this.currentlySnappedComponent != null && (snappedComponent == null || snappedComponent.downNeighborID != -7777 && snappedComponent.downNeighborID != -99998) && !this.currentlySnappedComponent.upNeighborImmutable && !this.currentlySnappedComponent.fullyImmutable)
          this.currentlySnappedComponent.upNeighborID = snappedComponent.myID;
        if (this.currentlySnappedComponent == null && snappedComponent.tryDefaultIfNoDownNeighborExists)
        {
          this.snapToDefaultClickableComponent();
          break;
        }
        if (this.currentlySnappedComponent == null)
        {
          this.noSnappedComponentFound(2, snappedComponent.region, snappedComponent.myID);
          break;
        }
        break;
      case 3:
        switch (this.currentlySnappedComponent.leftNeighborID)
        {
          case -99999:
            this.snapToDefaultClickableComponent();
            break;
          case -99998:
            this.automaticSnapBehavior(3, this.currentlySnappedComponent.region, this.currentlySnappedComponent.myID);
            break;
          case -7777:
            this.customSnapBehavior(3, this.currentlySnappedComponent.region, this.currentlySnappedComponent.myID);
            break;
          default:
            this.currentlySnappedComponent = this.getComponentWithID(this.currentlySnappedComponent.leftNeighborID);
            break;
        }
        if (this.currentlySnappedComponent != null && (snappedComponent == null || snappedComponent.leftNeighborID != -7777 && snappedComponent.leftNeighborID != -99998) && !this.currentlySnappedComponent.rightNeighborImmutable && !this.currentlySnappedComponent.fullyImmutable)
          this.currentlySnappedComponent.rightNeighborID = snappedComponent.myID;
        if (this.currentlySnappedComponent == null)
        {
          this.noSnappedComponentFound(3, snappedComponent.region, snappedComponent.myID);
          break;
        }
        break;
    }
    if (this.currentlySnappedComponent != null && snappedComponent != null && this.currentlySnappedComponent.region != snappedComponent.region)
      this.actionOnRegionChange(snappedComponent.region, this.currentlySnappedComponent.region);
    if (this.currentlySnappedComponent == null)
      this.currentlySnappedComponent = snappedComponent;
    this.snapCursorToCurrentSnappedComponent();
    if (this.currentlySnappedComponent == snappedComponent)
      return;
    Game1.playSound("shiny4");
  }

  public virtual void snapCursorToCurrentSnappedComponent()
  {
    if (this.currentlySnappedComponent == null)
      return;
    Game1.setMousePosition(this.currentlySnappedComponent.bounds.Right - this.currentlySnappedComponent.bounds.Width / 4, this.currentlySnappedComponent.bounds.Bottom - this.currentlySnappedComponent.bounds.Height / 4, true);
  }

  protected virtual void noSnappedComponentFound(int direction, int oldRegion, int oldID)
  {
  }

  protected virtual void customSnapBehavior(int direction, int oldRegion, int oldID)
  {
  }

  public virtual bool IsActive()
  {
    if (this._parentMenu == null)
      return this == Game1.activeClickableMenu;
    IClickableMenu parentMenu = this._parentMenu;
    while (parentMenu?._parentMenu != null)
      parentMenu = parentMenu._parentMenu;
    return parentMenu == Game1.activeClickableMenu;
  }

  public virtual void automaticSnapBehavior(int direction, int oldRegion, int oldID)
  {
    if (this.currentlySnappedComponent == null)
    {
      this.snapToDefaultClickableComponent();
    }
    else
    {
      Vector2 zero = Vector2.Zero;
      switch (direction)
      {
        case 0:
          zero.X = 0.0f;
          zero.Y = -1f;
          break;
        case 1:
          zero.X = 1f;
          zero.Y = 0.0f;
          break;
        case 2:
          zero.X = 0.0f;
          zero.Y = 1f;
          break;
        case 3:
          zero.X = -1f;
          zero.Y = 0.0f;
          break;
      }
      float num1 = -1f;
      ClickableComponent clickableComponent1 = (ClickableComponent) null;
      for (int index = 0; index < this.allClickableComponents.Count; ++index)
      {
        ClickableComponent clickableComponent2 = this.allClickableComponents[index];
        if ((clickableComponent2.leftNeighborID != -1 || clickableComponent2.rightNeighborID != -1 || clickableComponent2.upNeighborID != -1 || clickableComponent2.downNeighborID != -1) && clickableComponent2.myID != -500 && this.IsAutomaticSnapValid(direction, this.currentlySnappedComponent, clickableComponent2) && clickableComponent2.visible && clickableComponent2 != this.upperRightCloseButton && clickableComponent2 != this.currentlySnappedComponent)
        {
          Vector2 vector2_1 = new Vector2((float) (clickableComponent2.bounds.Center.X - this.currentlySnappedComponent.bounds.Center.X), (float) (clickableComponent2.bounds.Center.Y - this.currentlySnappedComponent.bounds.Center.Y));
          Vector2 vector2_2 = new Vector2(vector2_1.X, vector2_1.Y);
          vector2_2.Normalize();
          float num2 = Vector2.Dot(zero, vector2_2);
          if ((double) num2 > 0.0099999997764825821)
          {
            float num3 = Vector2.DistanceSquared(Vector2.Zero, vector2_1);
            bool flag = false;
            switch (direction)
            {
              case 0:
              case 2:
                if ((double) Math.Abs(vector2_1.X) < 32.0)
                {
                  flag = true;
                  break;
                }
                break;
              case 1:
              case 3:
                if ((double) Math.Abs(vector2_1.Y) < 32.0)
                {
                  flag = true;
                  break;
                }
                break;
            }
            if (this._ShouldAutoSnapPrioritizeAlignedElements() && (double) num2 > 0.99998998641967773 | flag)
              num3 *= 0.01f;
            if ((double) num1 == -1.0 || (double) num3 < (double) num1)
            {
              num1 = num3;
              clickableComponent1 = clickableComponent2;
            }
          }
        }
      }
      if (clickableComponent1 == null)
        return;
      this.currentlySnappedComponent = clickableComponent1;
    }
  }

  protected virtual bool _ShouldAutoSnapPrioritizeAlignedElements() => true;

  public virtual bool IsAutomaticSnapValid(
    int direction,
    ClickableComponent a,
    ClickableComponent b)
  {
    return true;
  }

  /// <summary>Handle the <see cref="F:StardewValley.Menus.IClickableMenu.currentlySnappedComponent" /> region changing.</summary>
  /// <param name="oldRegion">The previous value.</param>
  /// <param name="newRegion">The new value.</param>
  protected virtual void actionOnRegionChange(int oldRegion, int newRegion)
  {
  }

  public ClickableComponent getComponentWithID(int id)
  {
    if (id == -500)
      return (ClickableComponent) null;
    if (this.allClickableComponents != null)
    {
      for (int index = 0; index < this.allClickableComponents.Count; ++index)
      {
        if (this.allClickableComponents[index] != null && this.allClickableComponents[index].myID == id && this.allClickableComponents[index].visible)
          return this.allClickableComponents[index];
      }
      for (int index = 0; index < this.allClickableComponents.Count; ++index)
      {
        if (this.allClickableComponents[index] != null && this.allClickableComponents[index].myAlternateID == id && this.allClickableComponents[index].visible)
          return this.allClickableComponents[index];
      }
    }
    return (ClickableComponent) null;
  }

  public void initializeUpperRightCloseButton()
  {
    this.upperRightCloseButton = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + this.width - 36, this.yPositionOnScreen - 8, 48 /*0x30*/, 48 /*0x30*/), Game1.mouseCursors, new Rectangle(337, 494, 12, 12), 4f);
  }

  public virtual void drawBackground(SpriteBatch b)
  {
    if (this is ShopMenu)
    {
      for (int x = 0; x < Game1.uiViewport.Width; x += 400)
      {
        for (int y = 0; y < Game1.uiViewport.Height; y += 384)
          b.Draw(Game1.mouseCursors, new Vector2((float) x, (float) y), new Rectangle?(new Rectangle(527, 0, 100, 96 /*0x60*/)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.08f);
      }
    }
    else
    {
      if (Game1.isDarkOut(Game1.currentLocation))
        b.Draw(Game1.mouseCursors, new Rectangle(0, 0, Game1.uiViewport.Width, Game1.uiViewport.Height), new Rectangle?(new Rectangle(639, 858, 1, 144 /*0x90*/)), Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 0.9f);
      else if (Game1.IsRainingHere())
        b.Draw(Game1.mouseCursors, new Rectangle(0, 0, Game1.uiViewport.Width, Game1.uiViewport.Height), new Rectangle?(new Rectangle(640, 858, 1, 184)), Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 0.9f);
      else
        b.Draw(Game1.mouseCursors, new Rectangle(0, 0, Game1.uiViewport.Width, Game1.uiViewport.Height), new Rectangle?(new Rectangle(639 + Game1.seasonIndex, 1051, 1, 400)), Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 0.9f);
      b.Draw(Game1.mouseCursors, new Vector2(-120f, (float) (Game1.uiViewport.Height - 592)), new Rectangle?(new Rectangle(0, Game1.season == Season.Winter ? 1035 : (Game1.isRaining || Game1.isDarkOut(Game1.currentLocation) ? 886 : 737), 639, 148)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.08f);
      b.Draw(Game1.mouseCursors, new Vector2(2436f, (float) (Game1.uiViewport.Height - 592)), new Rectangle?(new Rectangle(0, Game1.season == Season.Winter ? 1035 : (Game1.isRaining || Game1.isDarkOut(Game1.currentLocation) ? 886 : 737), 639, 148)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.08f);
      if (!Game1.isRaining)
        return;
      b.Draw(Game1.staminaRect, new Rectangle(0, 0, Game1.uiViewport.Width, Game1.uiViewport.Height), Color.Blue * 0.2f);
    }
  }

  public virtual bool showWithoutTransparencyIfOptionIsSet()
  {
    switch (this)
    {
      case GameMenu _:
      case ShopMenu _:
      case WheelSpinGame _:
      case ItemGrabMenu _:
        return true;
      default:
        return false;
    }
  }

  public virtual void clickAway()
  {
  }

  /// <summary>Update the menu when the game window is resized.</summary>
  /// <param name="oldBounds">The window's previous pixel size.</param>
  /// <param name="newBounds">The window's new pixel size.</param>
  public virtual void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
  {
    this.xPositionOnScreen = (int) ((double) (newBounds.Width - this.width) * ((double) this.xPositionOnScreen / (double) (oldBounds.Width - this.width)));
    this.yPositionOnScreen = (int) ((double) (newBounds.Height - this.height) * ((double) this.yPositionOnScreen / (double) (oldBounds.Height - this.height)));
  }

  public virtual void setUpForGamePadMode()
  {
  }

  public virtual bool shouldClampGamePadCursor() => false;

  /// <summary>Handle the left-click button being released (including a button resulting in a 'click' through controller selection).</summary>
  /// <param name="x">The cursor's current pixel X coordinate.</param>
  /// <param name="y">The cursor's current pixel Y coordinate.</param>
  public virtual void releaseLeftClick(int x, int y)
  {
  }

  /// <summary>Handle the left-click button being held down (including a button resulting in a 'click' through controller selection). This is called each tick that it's held.</summary>
  /// <param name="x">The cursor's current pixel X coordinate.</param>
  /// <param name="y">The cursor's current pixel Y coordinate.</param>
  public virtual void leftClickHeld(int x, int y)
  {
  }

  /// <summary>Handle a user left-click in the UI (including a 'click' through controller selection).</summary>
  /// <param name="x">The pixel X coordinate that was clicked.</param>
  /// <param name="y">The pixel Y coordinate that was clicked.</param>
  /// <param name="playSound">Whether to play sounds in response to the click, if applicable.</param>
  public virtual void receiveLeftClick(int x, int y, bool playSound = true)
  {
    if (this.upperRightCloseButton == null || !this.readyToClose() || !this.upperRightCloseButton.containsPoint(x, y))
      return;
    if (playSound)
      Game1.playSound(this.closeSound);
    this.exitThisMenu();
  }

  /// <summary>Get whether controller-style menus should be disabled for this menu.</summary>
  public virtual bool overrideSnappyMenuCursorMovementBan() => false;

  /// <summary>Handle a user right-click in the UI (including a 'click' through a controller <see cref="F:Microsoft.Xna.Framework.Input.Buttons.X" /> button).</summary>
  /// <param name="x">The pixel X coordinate that was clicked.</param>
  /// <param name="y">The pixel Y coordinate that was clicked.</param>
  /// <param name="playSound">Whether to play sounds in response to the click, if applicable.</param>
  public virtual void receiveRightClick(int x, int y, bool playSound = true)
  {
  }

  /// <summary>Handle a keyboard button pressed while the menu is open.</summary>
  /// <param name="key">The keyboard button that was pressed.</param>
  public virtual void receiveKeyPress(Keys key)
  {
    if (key == Keys.None)
      return;
    if (Game1.options.doesInputListContain(Game1.options.menuButton, key) && this.readyToClose())
    {
      this.exitThisMenu();
    }
    else
    {
      if (!Game1.options.snappyMenus || !Game1.options.gamepadControls || this.overrideSnappyMenuCursorMovementBan())
        return;
      this.applyMovementKey(key);
    }
  }

  /// <summary>Handle a controller button held down while the menu is open. This is called each tick that it's held.</summary>
  /// <param name="b">The button being held.</param>
  public virtual void gamePadButtonHeld(Buttons b)
  {
  }

  public virtual ClickableComponent getCurrentlySnappedComponent()
  {
    return this.currentlySnappedComponent;
  }

  /// <summary>Handle the scroll wheel being spun while the menu is open. This is called each time the scroll wheel value changes.</summary>
  /// <param name="direction">The change relative to the previous value.</param>
  public virtual void receiveScrollWheelAction(int direction)
  {
  }

  /// <summary>Handle the cursor hovering over the menu. This is called each tick, sometimes regardless of whether the cursor is within the menu's bounds.</summary>
  /// <param name="x">The pixel X coordinate being hovered by the cursor.</param>
  /// <param name="y">The pixel Y coordinate being hovered by the cursor.</param>
  public virtual void performHoverAction(int x, int y)
  {
    this.upperRightCloseButton?.tryHover(x, y, 0.5f);
  }

  /// <summary>Render the UI.</summary>
  /// <param name="b">The sprite batch being drawn.</param>
  /// <param name="red">If the menu can be tinted, a red tint to apply (as a value between 0 and 255) or -1 for no tint.</param>
  /// <param name="green">If the menu can be tinted, a green tint to apply (as a value between 0 and 255) or -1 for no tint.</param>
  /// <param name="blue">If the menu can be tinted, a blue tint to apply (as a value between 0 and 255) or -1 for no tint.</param>
  public virtual void draw(SpriteBatch b, int red, int green, int blue)
  {
    if (this.upperRightCloseButton == null || !this.shouldDrawCloseButton())
      return;
    this.upperRightCloseButton.draw(b);
  }

  /// <summary>Render the UI.</summary>
  /// <param name="b">The sprite batch being drawn.</param>
  public virtual void draw(SpriteBatch b)
  {
    if (this.upperRightCloseButton == null || !this.shouldDrawCloseButton())
      return;
    this.upperRightCloseButton.draw(b);
  }

  public virtual bool isWithinBounds(int x, int y)
  {
    return x - this.xPositionOnScreen < this.width && x - this.xPositionOnScreen >= 0 && y - this.yPositionOnScreen < this.height && y - this.yPositionOnScreen >= 0;
  }

  /// <summary>Update the menu state if needed.</summary>
  /// <param name="time">The elapsed game time.</param>
  public virtual void update(GameTime time)
  {
  }

  /// <summary>Perform any cleanup needed when the menu exits.</summary>
  protected virtual void cleanupBeforeExit()
  {
  }

  public virtual bool shouldDrawCloseButton() => true;

  public void exitThisMenuNoSound() => this.exitThisMenu(false);

  public void exitThisMenu(bool playSound = true)
  {
    Action<IClickableMenu> behaviorBeforeCleanup = this.behaviorBeforeCleanup;
    if (behaviorBeforeCleanup != null)
      behaviorBeforeCleanup(this);
    this.cleanupBeforeExit();
    if (playSound)
      Game1.playSound(this.closeSound);
    if (this == Game1.activeClickableMenu)
      Game1.exitActiveMenu();
    else if (Game1.activeClickableMenu is GameMenu activeClickableMenu && activeClickableMenu.GetCurrentPage() == this)
      Game1.exitActiveMenu();
    if (this._parentMenu != null)
    {
      IClickableMenu parentMenu = this._parentMenu;
      this._parentMenu = (IClickableMenu) null;
      parentMenu.SetChildMenu((IClickableMenu) null);
    }
    if (this.exitFunction == null)
      return;
    IClickableMenu.onExit exitFunction = this.exitFunction;
    this.exitFunction = (IClickableMenu.onExit) null;
    exitFunction();
  }

  public virtual void emergencyShutDown()
  {
  }

  public virtual bool readyToClose() => true;

  protected void drawHorizontalPartition(
    SpriteBatch b,
    int yPosition,
    bool small = false,
    int red = -1,
    int green = -1,
    int blue = -1)
  {
    Color color = red == -1 ? Color.White : new Color(red, green, blue);
    Texture2D texture = red == -1 ? Game1.menuTexture : Game1.uncoloredMenuTexture;
    if (small)
    {
      b.Draw(texture, new Rectangle(this.xPositionOnScreen + 32 /*0x20*/, yPosition, this.width - 64 /*0x40*/, 64 /*0x40*/), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 25)), color);
    }
    else
    {
      b.Draw(texture, new Vector2((float) this.xPositionOnScreen, (float) yPosition), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 4)), color);
      b.Draw(texture, new Rectangle(this.xPositionOnScreen + 64 /*0x40*/, yPosition, this.width - 128 /*0x80*/, 64 /*0x40*/), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 6)), color);
      b.Draw(texture, new Vector2((float) (this.xPositionOnScreen + this.width - 64 /*0x40*/), (float) yPosition), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 7)), color);
    }
  }

  protected void drawVerticalPartition(
    SpriteBatch b,
    int xPosition,
    bool small = false,
    int red = -1,
    int green = -1,
    int blue = -1,
    int heightOverride = -1)
  {
    Color color = red == -1 ? Color.White : new Color(red, green, blue);
    Texture2D texture = red == -1 ? Game1.menuTexture : Game1.uncoloredMenuTexture;
    if (small)
    {
      b.Draw(texture, new Rectangle(xPosition, this.yPositionOnScreen + 64 /*0x40*/ + 32 /*0x20*/, 64 /*0x40*/, heightOverride != -1 ? heightOverride : this.height - 128 /*0x80*/), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 26)), color);
    }
    else
    {
      b.Draw(texture, new Vector2((float) xPosition, (float) (this.yPositionOnScreen + 64 /*0x40*/)), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 1)), color);
      b.Draw(texture, new Rectangle(xPosition, this.yPositionOnScreen + 128 /*0x80*/, 64 /*0x40*/, heightOverride != -1 ? heightOverride : this.height - 192 /*0xC0*/), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 5)), color);
      b.Draw(texture, new Vector2((float) xPosition, (float) (this.yPositionOnScreen + (heightOverride != -1 ? heightOverride : this.height - 64 /*0x40*/))), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 13)), color);
    }
  }

  protected void drawVerticalIntersectingPartition(
    SpriteBatch b,
    int xPosition,
    int yPosition,
    int red = -1,
    int green = -1,
    int blue = -1)
  {
    Color color = red == -1 ? Color.White : new Color(red, green, blue);
    Texture2D texture = red == -1 ? Game1.menuTexture : Game1.uncoloredMenuTexture;
    b.Draw(texture, new Vector2((float) xPosition, (float) yPosition), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 59)), color);
    b.Draw(texture, new Rectangle(xPosition, yPosition + 64 /*0x40*/, 64 /*0x40*/, this.yPositionOnScreen + this.height - 64 /*0x40*/ - yPosition - 64 /*0x40*/), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 63 /*0x3F*/)), color);
    b.Draw(texture, new Vector2((float) xPosition, (float) (this.yPositionOnScreen + this.height - 64 /*0x40*/)), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 62)), color);
  }

  protected void drawVerticalUpperIntersectingPartition(
    SpriteBatch b,
    int xPosition,
    int partitionHeight,
    int red = -1,
    int green = -1,
    int blue = -1)
  {
    Color color = red == -1 ? Color.White : new Color(red, green, blue);
    Texture2D texture = red == -1 ? Game1.menuTexture : Game1.uncoloredMenuTexture;
    b.Draw(texture, new Vector2((float) xPosition, (float) (this.yPositionOnScreen + 64 /*0x40*/)), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 44)), color);
    b.Draw(texture, new Rectangle(xPosition, this.yPositionOnScreen + 128 /*0x80*/, 64 /*0x40*/, partitionHeight - 32 /*0x20*/), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 63 /*0x3F*/)), color);
    b.Draw(texture, new Vector2((float) xPosition, (float) (this.yPositionOnScreen + partitionHeight + 64 /*0x40*/)), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 39)), color);
  }

  public static void drawTextureBox(
    SpriteBatch b,
    int x,
    int y,
    int width,
    int height,
    Color color)
  {
    IClickableMenu.drawTextureBox(b, Game1.menuTexture, new Rectangle(0, 256 /*0x0100*/, 60, 60), x, y, width, height, color);
  }

  public static void drawTextureBox(
    SpriteBatch b,
    Texture2D texture,
    Rectangle sourceRect,
    int x,
    int y,
    int width,
    int height,
    Color color,
    float scale = 1f,
    bool drawShadow = true,
    float draw_layer = -1f)
  {
    int num = sourceRect.Width / 3;
    float layerDepth = draw_layer - 0.03f;
    if ((double) draw_layer < 0.0)
    {
      draw_layer = (float) (0.800000011920929 - (double) y * 9.9999999747524271E-07);
      layerDepth = 0.77f;
    }
    if (drawShadow)
    {
      b.Draw(texture, new Vector2((float) (x + width - (int) ((double) num * (double) scale) - 8), (float) (y + 8)), new Rectangle?(new Rectangle(sourceRect.X + num * 2, sourceRect.Y, num, num)), Color.Black * 0.4f, 0.0f, Vector2.Zero, scale, SpriteEffects.None, layerDepth);
      b.Draw(texture, new Vector2((float) (x - 8), (float) (y + height - (int) ((double) num * (double) scale) + 8)), new Rectangle?(new Rectangle(sourceRect.X, num * 2 + sourceRect.Y, num, num)), Color.Black * 0.4f, 0.0f, Vector2.Zero, scale, SpriteEffects.None, layerDepth);
      b.Draw(texture, new Vector2((float) (x + width - (int) ((double) num * (double) scale) - 8), (float) (y + height - (int) ((double) num * (double) scale) + 8)), new Rectangle?(new Rectangle(sourceRect.X + num * 2, num * 2 + sourceRect.Y, num, num)), Color.Black * 0.4f, 0.0f, Vector2.Zero, scale, SpriteEffects.None, layerDepth);
      b.Draw(texture, new Rectangle(x + (int) ((double) num * (double) scale) - 8, y + 8, width - (int) ((double) num * (double) scale) * 2, (int) ((double) num * (double) scale)), new Rectangle?(new Rectangle(sourceRect.X + num, sourceRect.Y, num, num)), Color.Black * 0.4f, 0.0f, Vector2.Zero, SpriteEffects.None, layerDepth);
      b.Draw(texture, new Rectangle(x + (int) ((double) num * (double) scale) - 8, y + height - (int) ((double) num * (double) scale) + 8, width - (int) ((double) num * (double) scale) * 2, (int) ((double) num * (double) scale)), new Rectangle?(new Rectangle(sourceRect.X + num, num * 2 + sourceRect.Y, num, num)), Color.Black * 0.4f, 0.0f, Vector2.Zero, SpriteEffects.None, layerDepth);
      b.Draw(texture, new Rectangle(x - 8, y + (int) ((double) num * (double) scale) + 8, (int) ((double) num * (double) scale), height - (int) ((double) num * (double) scale) * 2), new Rectangle?(new Rectangle(sourceRect.X, num + sourceRect.Y, num, num)), Color.Black * 0.4f, 0.0f, Vector2.Zero, SpriteEffects.None, layerDepth);
      b.Draw(texture, new Rectangle(x + width - (int) ((double) num * (double) scale) - 8, y + (int) ((double) num * (double) scale) + 8, (int) ((double) num * (double) scale), height - (int) ((double) num * (double) scale) * 2), new Rectangle?(new Rectangle(sourceRect.X + num * 2, num + sourceRect.Y, num, num)), Color.Black * 0.4f, 0.0f, Vector2.Zero, SpriteEffects.None, layerDepth);
      b.Draw(texture, new Rectangle((int) ((double) num * (double) scale / 2.0) + x - 8, (int) ((double) num * (double) scale / 2.0) + y + 8, width - (int) ((double) num * (double) scale), height - (int) ((double) num * (double) scale)), new Rectangle?(new Rectangle(num + sourceRect.X, num + sourceRect.Y, num, num)), Color.Black * 0.4f, 0.0f, Vector2.Zero, SpriteEffects.None, layerDepth);
    }
    b.Draw(texture, new Rectangle((int) ((double) num * (double) scale) + x, (int) ((double) num * (double) scale) + y, width - (int) ((double) num * (double) scale * 2.0), height - (int) ((double) num * (double) scale * 2.0)), new Rectangle?(new Rectangle(num + sourceRect.X, num + sourceRect.Y, num, num)), color, 0.0f, Vector2.Zero, SpriteEffects.None, draw_layer);
    b.Draw(texture, new Vector2((float) x, (float) y), new Rectangle?(new Rectangle(sourceRect.X, sourceRect.Y, num, num)), color, 0.0f, Vector2.Zero, scale, SpriteEffects.None, draw_layer);
    b.Draw(texture, new Vector2((float) (x + width - (int) ((double) num * (double) scale)), (float) y), new Rectangle?(new Rectangle(sourceRect.X + num * 2, sourceRect.Y, num, num)), color, 0.0f, Vector2.Zero, scale, SpriteEffects.None, draw_layer);
    b.Draw(texture, new Vector2((float) x, (float) (y + height - (int) ((double) num * (double) scale))), new Rectangle?(new Rectangle(sourceRect.X, num * 2 + sourceRect.Y, num, num)), color, 0.0f, Vector2.Zero, scale, SpriteEffects.None, draw_layer);
    b.Draw(texture, new Vector2((float) (x + width - (int) ((double) num * (double) scale)), (float) (y + height - (int) ((double) num * (double) scale))), new Rectangle?(new Rectangle(sourceRect.X + num * 2, num * 2 + sourceRect.Y, num, num)), color, 0.0f, Vector2.Zero, scale, SpriteEffects.None, draw_layer);
    b.Draw(texture, new Rectangle(x + (int) ((double) num * (double) scale), y, width - (int) ((double) num * (double) scale) * 2, (int) ((double) num * (double) scale)), new Rectangle?(new Rectangle(sourceRect.X + num, sourceRect.Y, num, num)), color, 0.0f, Vector2.Zero, SpriteEffects.None, draw_layer);
    b.Draw(texture, new Rectangle(x + (int) ((double) num * (double) scale), y + height - (int) ((double) num * (double) scale), width - (int) ((double) num * (double) scale) * 2, (int) ((double) num * (double) scale)), new Rectangle?(new Rectangle(sourceRect.X + num, num * 2 + sourceRect.Y, num, num)), color, 0.0f, Vector2.Zero, SpriteEffects.None, draw_layer);
    b.Draw(texture, new Rectangle(x, y + (int) ((double) num * (double) scale), (int) ((double) num * (double) scale), height - (int) ((double) num * (double) scale) * 2), new Rectangle?(new Rectangle(sourceRect.X, num + sourceRect.Y, num, num)), color, 0.0f, Vector2.Zero, SpriteEffects.None, draw_layer);
    b.Draw(texture, new Rectangle(x + width - (int) ((double) num * (double) scale), y + (int) ((double) num * (double) scale), (int) ((double) num * (double) scale), height - (int) ((double) num * (double) scale) * 2), new Rectangle?(new Rectangle(sourceRect.X + num * 2, num + sourceRect.Y, num, num)), color, 0.0f, Vector2.Zero, SpriteEffects.None, draw_layer);
  }

  public void drawBorderLabel(SpriteBatch b, string text, SpriteFont font, int x, int y)
  {
    int x1 = (int) font.MeasureString(text).X;
    y += 52;
    b.Draw(Game1.mouseCursors, new Vector2((float) x, (float) y), new Rectangle?(new Rectangle(256 /*0x0100*/, 267, 6, 16 /*0x10*/)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.87f);
    b.Draw(Game1.mouseCursors, new Vector2((float) (x + 24), (float) y), new Rectangle?(new Rectangle(262, 267, 1, 16 /*0x10*/)), Color.White, 0.0f, Vector2.Zero, new Vector2((float) x1, 4f), SpriteEffects.None, 0.87f);
    b.Draw(Game1.mouseCursors, new Vector2((float) (x + 24 + x1), (float) y), new Rectangle?(new Rectangle(263, 267, 6, 16 /*0x10*/)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.87f);
    Utility.drawTextWithShadow(b, text, font, new Vector2((float) (x + 24), (float) (y + 20)), Game1.textColor);
  }

  public static void drawToolTip(
    SpriteBatch b,
    string hoverText,
    string hoverTitle,
    Item hoveredItem,
    bool heldItem = false,
    int healAmountToDisplay = -1,
    int currencySymbol = 0,
    string extraItemToShowIndex = null,
    int extraItemToShowAmount = -1,
    CraftingRecipe craftingIngredients = null,
    int moneyAmountToShowAtBottom = -1,
    IList<Item> additionalCraftMaterials = null)
  {
    bool flag = hoveredItem is StardewValley.Object @object && @object.edibility.Value != -300;
    string[] buffIconsToDisplay = (string[]) null;
    ObjectData objectData;
    if (flag && Game1.objectData.TryGetValue(hoveredItem.ItemId, out objectData))
    {
      BuffEffects buffEffects = new BuffEffects();
      int milliseconds = int.MinValue;
      foreach (Buff buff in StardewValley.Object.TryCreateBuffsFromData(objectData, hoveredItem.Name, hoveredItem.DisplayName, adjustEffects: new Action<BuffEffects>(hoveredItem.ModifyItemBuffs)))
      {
        buffEffects.Add(buff.effects);
        if (buff.millisecondsDuration == -2 || buff.millisecondsDuration > milliseconds && milliseconds != -2)
          milliseconds = buff.millisecondsDuration;
      }
      if (buffEffects.HasAnyValue())
      {
        buffIconsToDisplay = buffEffects.ToLegacyAttributeFormat();
        if (milliseconds != -2)
          buffIconsToDisplay[12] = " " + Utility.getMinutesSecondsStringFromMilliseconds(milliseconds);
      }
    }
    IClickableMenu.drawHoverText(b, hoverText, Game1.smallFont, heldItem ? 40 : 0, heldItem ? 40 : 0, moneyAmountToShowAtBottom, hoverTitle, flag ? (hoveredItem as StardewValley.Object).edibility.Value : -1, buffIconsToDisplay, hoveredItem, currencySymbol, extraItemToShowIndex, extraItemToShowAmount, craftingIngredients: craftingIngredients, additional_craft_materials: additionalCraftMaterials);
  }

  public static void drawHoverText(
    SpriteBatch b,
    string text,
    SpriteFont font,
    int xOffset = 0,
    int yOffset = 0,
    int moneyAmountToDisplayAtBottom = -1,
    string boldTitleText = null,
    int healAmountToDisplay = -1,
    string[] buffIconsToDisplay = null,
    Item hoveredItem = null,
    int currencySymbol = 0,
    string extraItemToShowIndex = null,
    int extraItemToShowAmount = -1,
    int overrideX = -1,
    int overrideY = -1,
    float alpha = 1f,
    CraftingRecipe craftingIngredients = null,
    IList<Item> additional_craft_materials = null,
    Texture2D boxTexture = null,
    Rectangle? boxSourceRect = null,
    Color? textColor = null,
    Color? textShadowColor = null,
    float boxScale = 1f,
    int boxWidthOverride = -1,
    int boxHeightOverride = -1)
  {
    IClickableMenu.HoverTextStringBuilder.Clear();
    IClickableMenu.HoverTextStringBuilder.Append(text);
    IClickableMenu.drawHoverText(b, IClickableMenu.HoverTextStringBuilder, font, xOffset, yOffset, moneyAmountToDisplayAtBottom, boldTitleText, healAmountToDisplay, buffIconsToDisplay, hoveredItem, currencySymbol, extraItemToShowIndex, extraItemToShowAmount, overrideX, overrideY, alpha, craftingIngredients, additional_craft_materials, boxTexture, boxSourceRect, textColor, textShadowColor, boxScale, boxWidthOverride, boxHeightOverride);
  }

  public static void drawHoverText(
    SpriteBatch b,
    StringBuilder text,
    SpriteFont font,
    int xOffset = 0,
    int yOffset = 0,
    int moneyAmountToDisplayAtBottom = -1,
    string boldTitleText = null,
    int healAmountToDisplay = -1,
    string[] buffIconsToDisplay = null,
    Item hoveredItem = null,
    int currencySymbol = 0,
    string extraItemToShowIndex = null,
    int extraItemToShowAmount = -1,
    int overrideX = -1,
    int overrideY = -1,
    float alpha = 1f,
    CraftingRecipe craftingIngredients = null,
    IList<Item> additional_craft_materials = null,
    Texture2D boxTexture = null,
    Rectangle? boxSourceRect = null,
    Color? textColor = null,
    Color? textShadowColor = null,
    float boxScale = 1f,
    int boxWidthOverride = -1,
    int boxHeightOverride = -1)
  {
    boxTexture = boxTexture ?? Game1.menuTexture;
    boxSourceRect = new Rectangle?(boxSourceRect ?? new Rectangle(0, 256 /*0x0100*/, 60, 60));
    ref Color? local1 = ref textColor;
    Color? nullable = textColor;
    Color color1 = nullable ?? Game1.textColor;
    local1 = new Color?(color1);
    ref Color? local2 = ref textShadowColor;
    nullable = textShadowColor;
    Color color2 = nullable ?? Game1.textShadowColor;
    local2 = new Color?(color2);
    if (text == null || text.Length == 0)
      return;
    if (hoveredItem != null && craftingIngredients != null && hoveredItem.getDescription().Equals(text.ToString()))
      text = new StringBuilder(" ");
    if (moneyAmountToDisplayAtBottom <= -1 && currencySymbol == 0 && hoveredItem != null && Game1.player.stats.Get("Book_PriceCatalogue") > 0U && !(hoveredItem is Furniture) && hoveredItem.CanBeLostOnDeath())
    {
      switch (hoveredItem)
      {
        case Clothing _:
        case Wallpaper _:
          goto label_9;
        case StardewValley.Object _:
          if ((hoveredItem as StardewValley.Object).bigCraftable.Value)
            goto label_9;
          break;
      }
      if (hoveredItem.sellToStorePrice(-1L) > 0)
        moneyAmountToDisplayAtBottom = hoveredItem.sellToStorePrice(-1L) * hoveredItem.Stack;
    }
label_9:
    string text1 = (string) null;
    if (boldTitleText != null && boldTitleText.Length == 0)
      boldTitleText = (string) null;
    int index1;
    int val1;
    if (healAmountToDisplay == -1)
    {
      val1 = 0;
    }
    else
    {
      SpriteFont spriteFont = font;
      string str1 = healAmountToDisplay.ToString();
      index1 = 32 /*0x20*/;
      string str2 = index1.ToString();
      string text2 = $"{str1}+ Energy{str2}";
      val1 = (int) spriteFont.MeasureString(text2).X;
    }
    int val2 = Math.Max((int) font.MeasureString(text).X, boldTitleText != null ? (int) Game1.dialogueFont.MeasureString(boldTitleText).X : 0);
    int num1 = Math.Max(val1, val2) + 32 /*0x20*/;
    int startingHeight = Math.Max(20 * 3, (int) font.MeasureString(text).Y + 32 /*0x20*/ + (moneyAmountToDisplayAtBottom > -1 ? (int) Math.Max(font.MeasureString(moneyAmountToDisplayAtBottom.ToString() ?? "").Y + 4f, 44f) : 0) + (boldTitleText != null ? (int) ((double) Game1.dialogueFont.MeasureString(boldTitleText).Y + 16.0) : 0));
    if (extraItemToShowIndex != null)
    {
      ParsedItemData dataOrErrorItem = ItemRegistry.GetDataOrErrorItem("(O)" + extraItemToShowIndex);
      string displayName = dataOrErrorItem.DisplayName;
      Rectangle sourceRect = dataOrErrorItem.GetSourceRect();
      string text3 = Game1.content.LoadString("Strings\\UI:ItemHover_Requirements", (object) extraItemToShowAmount, extraItemToShowAmount > 1 ? (object) Lexicon.makePlural(displayName) : (object) displayName);
      int num2 = sourceRect.Width * 2 * 4;
      num1 = Math.Max(num1, num2 + (int) font.MeasureString(text3).X);
    }
    if (buffIconsToDisplay != null)
    {
      string[] strArray = buffIconsToDisplay;
      for (index1 = 0; index1 < strArray.Length; ++index1)
      {
        string str = strArray[index1];
        if (!str.Equals("0") && str != "")
          startingHeight += 39;
      }
      startingHeight += 4;
    }
    if (craftingIngredients != null && Game1.options.showAdvancedCraftingInformation && craftingIngredients.getCraftCountText() != null)
      startingHeight += (int) font.MeasureString("T").Y + 2;
    string text4 = (string) null;
    if (hoveredItem != null)
    {
      if (hoveredItem is FishingRod)
      {
        if (hoveredItem.attachmentSlots() == 1)
          startingHeight += 68;
        else if (hoveredItem.attachmentSlots() > 1)
          startingHeight += 144 /*0x90*/;
      }
      else
        startingHeight += 68 * hoveredItem.attachmentSlots();
      text4 = hoveredItem.getCategoryName();
      if (text4.Length > 0)
      {
        num1 = Math.Max(num1, (int) font.MeasureString(text4).X + 32 /*0x20*/);
        startingHeight += (int) font.MeasureString("T").Y;
      }
      int sub1 = 9999;
      int horizontalBuffer = 92;
      Point tooltipSpecialIcons = hoveredItem.getExtraSpaceNeededForTooltipSpecialIcons(font, num1, horizontalBuffer, startingHeight, text, boldTitleText, moneyAmountToDisplayAtBottom);
      num1 = tooltipSpecialIcons.X != 0 ? tooltipSpecialIcons.X : num1;
      startingHeight = tooltipSpecialIcons.Y != 0 ? tooltipSpecialIcons.Y : startingHeight;
      if (!(hoveredItem is MeleeWeapon meleeWeapon))
      {
        if (hoveredItem is StardewValley.Object @object && @object.edibility.Value != -300 && @object.edibility.Value != 0)
        {
          healAmountToDisplay = @object.staminaRecoveredOnConsumption();
          if (healAmountToDisplay != -1)
            startingHeight += 40 * (healAmountToDisplay <= 0 || @object.healthRecoveredOnConsumption() <= 0 ? 1 : 2);
          else
            startingHeight += 40;
          if (Game1.content.GetCurrentLanguage() == LocalizedContentManager.LanguageCode.zh && Game1.options.useChineseSmoothFont)
            startingHeight += 16 /*0x10*/;
          num1 = (int) Math.Max((float) num1, Math.Max(font.MeasureString(Game1.content.LoadString("Strings\\UI:ItemHover_Energy", (object) sub1)).X + (float) horizontalBuffer, font.MeasureString(Game1.content.LoadString("Strings\\UI:ItemHover_Health", (object) sub1)).X + (float) horizontalBuffer));
        }
      }
      else
      {
        if (meleeWeapon.GetTotalForgeLevels() > 0)
          startingHeight += (int) font.MeasureString("T").Y;
        if (meleeWeapon.GetEnchantmentLevel<GalaxySoulEnchantment>() > 0)
          startingHeight += (int) font.MeasureString("T").Y;
      }
      if (buffIconsToDisplay != null)
      {
        for (int index2 = 0; index2 < buffIconsToDisplay.Length; ++index2)
        {
          if (!buffIconsToDisplay[index2].Equals("0") && index2 <= 12)
            num1 = (int) Math.Max((float) num1, font.MeasureString(Game1.content.LoadString("Strings\\UI:ItemHover_Buff" + index2.ToString(), (object) sub1)).X + (float) horizontalBuffer);
        }
      }
    }
    Vector2 vector2_1 = Vector2.Zero;
    if (craftingIngredients != null)
    {
      if (Game1.options.showAdvancedCraftingInformation)
      {
        int craftableCount = craftingIngredients.getCraftableCount(additional_craft_materials);
        if (craftableCount > 1)
        {
          text1 = $" ({craftableCount.ToString()})";
          vector2_1 = Game1.smallFont.MeasureString(text1);
        }
      }
      num1 = (int) Math.Max((float) ((double) Game1.dialogueFont.MeasureString(boldTitleText).X + (double) vector2_1.X + 12.0), 384f);
      startingHeight += craftingIngredients.getDescriptionHeight(num1 + 4 - 8) - 32 /*0x20*/;
      if (craftingIngredients != null && hoveredItem != null && hoveredItem.getDescription().Equals(text.ToString()))
        startingHeight -= (int) font.MeasureString(text.ToString()).Y;
      if (craftingIngredients != null && Game1.content.GetCurrentLanguage() == LocalizedContentManager.LanguageCode.zh)
        startingHeight += 8;
    }
    else if (text1 != null && boldTitleText != null)
    {
      vector2_1 = Game1.smallFont.MeasureString(text1);
      num1 = (int) Math.Max((float) num1, (float) ((double) Game1.dialogueFont.MeasureString(boldTitleText).X + (double) vector2_1.X + 12.0));
    }
    int x = Game1.getOldMouseX() + 32 /*0x20*/ + xOffset;
    int y1 = Game1.getOldMouseY() + 32 /*0x20*/ + yOffset;
    if (overrideX != -1)
      x = overrideX;
    if (overrideY != -1)
      y1 = overrideY;
    int num3 = x + num1;
    Rectangle safeArea = Utility.getSafeArea();
    int right1 = safeArea.Right;
    if (num3 > right1)
    {
      safeArea = Utility.getSafeArea();
      x = safeArea.Right - num1;
      y1 += 16 /*0x10*/;
    }
    int num4 = y1 + startingHeight;
    safeArea = Utility.getSafeArea();
    int bottom = safeArea.Bottom;
    if (num4 > bottom)
    {
      x += 16 /*0x10*/;
      int num5 = x + num1;
      safeArea = Utility.getSafeArea();
      int right2 = safeArea.Right;
      if (num5 > right2)
      {
        safeArea = Utility.getSafeArea();
        x = safeArea.Right - num1;
      }
      safeArea = Utility.getSafeArea();
      y1 = safeArea.Bottom - startingHeight;
    }
    int width1 = num1 + 4;
    int width2 = boxWidthOverride != -1 ? boxWidthOverride : width1 + (craftingIngredients != null ? 21 : 0);
    int height1 = boxHeightOverride != -1 ? boxHeightOverride : startingHeight;
    IClickableMenu.drawTextureBox(b, boxTexture, boxSourceRect.Value, x, y1, width2, height1, Color.White * alpha, boxScale);
    if (boldTitleText != null)
    {
      Vector2 vector2_2 = Game1.dialogueFont.MeasureString(boldTitleText);
      IClickableMenu.drawTextureBox(b, boxTexture, boxSourceRect.Value, x, y1, width1 + (craftingIngredients != null ? 21 : 0), (int) Game1.dialogueFont.MeasureString(boldTitleText).Y + 32 /*0x20*/ + (hoveredItem == null || text4.Length <= 0 ? 0 : (int) font.MeasureString("asd").Y) - 4, Color.White * alpha, drawShadow: false);
      b.Draw(Game1.menuTexture, new Rectangle(x + 12, y1 + (int) Game1.dialogueFont.MeasureString(boldTitleText).Y + 32 /*0x20*/ + (hoveredItem == null || text4.Length <= 0 ? 0 : (int) font.MeasureString("asd").Y) - 4, width1 - 4 * (craftingIngredients == null ? 6 : 1), 4), new Rectangle?(new Rectangle(44, 300, 4, 4)), Color.White);
      b.DrawString(Game1.dialogueFont, boldTitleText, new Vector2((float) (x + 16 /*0x10*/), (float) (y1 + 16 /*0x10*/ + 4)) + new Vector2(2f, 2f), textShadowColor.Value);
      b.DrawString(Game1.dialogueFont, boldTitleText, new Vector2((float) (x + 16 /*0x10*/), (float) (y1 + 16 /*0x10*/ + 4)) + new Vector2(0.0f, 2f), textShadowColor.Value);
      b.DrawString(Game1.dialogueFont, boldTitleText, new Vector2((float) (x + 16 /*0x10*/), (float) (y1 + 16 /*0x10*/ + 4)), textColor.Value);
      if (text1 != null)
        Utility.drawTextWithShadow(b, text1, Game1.smallFont, new Vector2((float) (x + 16 /*0x10*/) + vector2_2.X, (float) (int) ((double) (y1 + 16 /*0x10*/ + 4) + (double) vector2_2.Y / 2.0 - (double) vector2_1.Y / 2.0)), Game1.textColor);
      y1 += (int) Game1.dialogueFont.MeasureString(boldTitleText).Y;
    }
    int y2;
    if (hoveredItem != null && text4.Length > 0)
    {
      int num6 = y1 - 4;
      Utility.drawTextWithShadow(b, text4, font, new Vector2((float) (x + 16 /*0x10*/), (float) (num6 + 16 /*0x10*/ + 4)), hoveredItem.getCategoryColor(), horizontalShadowOffset: 2, verticalShadowOffset: 2);
      y2 = num6 + ((int) font.MeasureString("T").Y + (boldTitleText != null ? 16 /*0x10*/ : 0) + 4);
      if (hoveredItem is Tool tool && tool.GetTotalForgeLevels() > 0)
      {
        string text5 = Game1.content.LoadString("Strings\\UI:Item_Tooltip_Forged");
        Utility.drawTextWithShadow(b, text5, font, new Vector2((float) (x + 16 /*0x10*/), (float) (y2 + 16 /*0x10*/ + 4)), Color.DarkRed, horizontalShadowOffset: 2, verticalShadowOffset: 2);
        int totalForgeLevels = tool.GetTotalForgeLevels();
        if (totalForgeLevels < tool.GetMaxForges() && !tool.hasEnchantmentOfType<DiamondEnchantment>())
        {
          SpriteBatch b1 = b;
          string[] strArray = new string[5]
          {
            " (",
            totalForgeLevels.ToString(),
            "/",
            null,
            null
          };
          index1 = tool.GetMaxForges();
          strArray[3] = index1.ToString();
          strArray[4] = ")";
          string text6 = string.Concat(strArray);
          SpriteFont font1 = font;
          Vector2 position = new Vector2((float) (x + 16 /*0x10*/) + font.MeasureString(text5).X, (float) (y2 + 16 /*0x10*/ + 4));
          Color dimGray = Color.DimGray;
          Utility.drawTextWithShadow(b1, text6, font1, position, dimGray, horizontalShadowOffset: 2, verticalShadowOffset: 2);
        }
        y2 += (int) font.MeasureString("T").Y;
      }
      if (hoveredItem is MeleeWeapon meleeWeapon && meleeWeapon.GetEnchantmentLevel<GalaxySoulEnchantment>() > 0)
      {
        GalaxySoulEnchantment enchantmentOfType = meleeWeapon.GetEnchantmentOfType<GalaxySoulEnchantment>();
        string text7 = Game1.content.LoadString("Strings\\UI:Item_Tooltip_GalaxyForged");
        Utility.drawTextWithShadow(b, text7, font, new Vector2((float) (x + 16 /*0x10*/), (float) (y2 + 16 /*0x10*/ + 4)), Color.DarkRed, horizontalShadowOffset: 2, verticalShadowOffset: 2);
        int level = enchantmentOfType.GetLevel();
        if (level < enchantmentOfType.GetMaximumLevel())
        {
          SpriteBatch b2 = b;
          string[] strArray = new string[5]
          {
            " (",
            level.ToString(),
            "/",
            null,
            null
          };
          index1 = enchantmentOfType.GetMaximumLevel();
          strArray[3] = index1.ToString();
          strArray[4] = ")";
          string text8 = string.Concat(strArray);
          SpriteFont font2 = font;
          Vector2 position = new Vector2((float) (x + 16 /*0x10*/) + font.MeasureString(text7).X, (float) (y2 + 16 /*0x10*/ + 4));
          Color dimGray = Color.DimGray;
          Utility.drawTextWithShadow(b2, text8, font2, position, dimGray, horizontalShadowOffset: 2, verticalShadowOffset: 2);
        }
        y2 += (int) font.MeasureString("T").Y;
      }
    }
    else
      y2 = y1 + (boldTitleText != null ? 16 /*0x10*/ : 0);
    if (hoveredItem != null && craftingIngredients == null)
      hoveredItem.drawTooltip(b, ref x, ref y2, font, alpha, text);
    else if (text != null && text.Length != 0 && (text.Length != 1 || text[0] != ' ') && (craftingIngredients == null || hoveredItem == null || !hoveredItem.getDescription().Equals(text.ToString())))
    {
      if (text.ToString().Contains("[line]"))
      {
        string[] strArray = text.ToString().Split("[line]");
        b.DrawString(font, strArray[0], new Vector2((float) (x + 16 /*0x10*/), (float) (y2 + 16 /*0x10*/ + 4)) + new Vector2(2f, 2f), textShadowColor.Value * alpha);
        b.DrawString(font, strArray[0], new Vector2((float) (x + 16 /*0x10*/), (float) (y2 + 16 /*0x10*/ + 4)) + new Vector2(0.0f, 2f), textShadowColor.Value * alpha);
        b.DrawString(font, strArray[0], new Vector2((float) (x + 16 /*0x10*/), (float) (y2 + 16 /*0x10*/ + 4)) + new Vector2(2f, 0.0f), textShadowColor.Value * alpha);
        b.DrawString(font, strArray[0], new Vector2((float) (x + 16 /*0x10*/), (float) (y2 + 16 /*0x10*/ + 4)), textColor.Value * 0.9f * alpha);
        int num7 = y2 + ((int) font.MeasureString(strArray[0]).Y - 16 /*0x10*/);
        Utility.drawLineWithScreenCoordinates(x + 16 /*0x10*/ - 4, num7 + 16 /*0x10*/ + 4, x + 16 /*0x10*/ + width1 - 28, num7 + 16 /*0x10*/ + 4, b, textShadowColor.Value);
        Utility.drawLineWithScreenCoordinates(x + 16 /*0x10*/ - 4, num7 + 16 /*0x10*/ + 5, x + 16 /*0x10*/ + width1 - 28, num7 + 16 /*0x10*/ + 5, b, textShadowColor.Value);
        if (strArray.Length > 1)
        {
          int num8 = num7 - 16 /*0x10*/;
          b.DrawString(font, strArray[1], new Vector2((float) (x + 16 /*0x10*/), (float) (num8 + 16 /*0x10*/ + 4)) + new Vector2(2f, 2f), textShadowColor.Value * alpha);
          b.DrawString(font, strArray[1], new Vector2((float) (x + 16 /*0x10*/), (float) (num8 + 16 /*0x10*/ + 4)) + new Vector2(0.0f, 2f), textShadowColor.Value * alpha);
          b.DrawString(font, strArray[1], new Vector2((float) (x + 16 /*0x10*/), (float) (num8 + 16 /*0x10*/ + 4)) + new Vector2(2f, 0.0f), textShadowColor.Value * alpha);
          b.DrawString(font, strArray[1], new Vector2((float) (x + 16 /*0x10*/), (float) (num8 + 16 /*0x10*/ + 4)), textColor.Value * 0.9f * alpha);
          num7 = num8 + (int) font.MeasureString(strArray[1]).Y;
        }
        y2 = num7 + 4;
      }
      else
      {
        b.DrawString(font, text, new Vector2((float) (x + 16 /*0x10*/), (float) (y2 + 16 /*0x10*/ + 4)) + new Vector2(2f, 2f), textShadowColor.Value * alpha);
        b.DrawString(font, text, new Vector2((float) (x + 16 /*0x10*/), (float) (y2 + 16 /*0x10*/ + 4)) + new Vector2(0.0f, 2f), textShadowColor.Value * alpha);
        b.DrawString(font, text, new Vector2((float) (x + 16 /*0x10*/), (float) (y2 + 16 /*0x10*/ + 4)) + new Vector2(2f, 0.0f), textShadowColor.Value * alpha);
        b.DrawString(font, text, new Vector2((float) (x + 16 /*0x10*/), (float) (y2 + 16 /*0x10*/ + 4)), textColor.Value * 0.9f * alpha);
        y2 += (int) font.MeasureString(text).Y + 4;
      }
    }
    if (craftingIngredients != null)
    {
      craftingIngredients.drawRecipeDescription(b, new Vector2((float) (x + 16 /*0x10*/), (float) (y2 - 8)), width1, additional_craft_materials);
      y2 += craftingIngredients.getDescriptionHeight(width1 - 8);
    }
    if (healAmountToDisplay != -1)
    {
      int num9 = (hoveredItem as StardewValley.Object).staminaRecoveredOnConsumption();
      if (Game1.content.GetCurrentLanguage() == LocalizedContentManager.LanguageCode.zh)
        y2 += 8;
      if (num9 >= 0)
      {
        int num10 = (hoveredItem as StardewValley.Object).healthRecoveredOnConsumption();
        if (num9 > 0)
        {
          Utility.drawWithShadow(b, Game1.mouseCursors, new Vector2((float) (x + 16 /*0x10*/ + 4), (float) (y2 + 16 /*0x10*/)), new Rectangle(0, 428, 10, 10), Color.White, 0.0f, Vector2.Zero, 3f, layerDepth: 0.95f);
          Utility.drawTextWithShadow(b, num9 >= 999 ? " 100%" : Game1.content.LoadString("Strings\\UI:ItemHover_Energy", (object) ("+" + num9.ToString())), font, new Vector2((float) (x + 16 /*0x10*/ + 34 + 4), (float) (y2 + 16 /*0x10*/)), Game1.textColor);
          y2 += 34;
        }
        if (num10 > 0)
        {
          Utility.drawWithShadow(b, Game1.mouseCursors, new Vector2((float) (x + 16 /*0x10*/ + 4), (float) (y2 + 16 /*0x10*/)), new Rectangle(0, 438, 10, 10), Color.White, 0.0f, Vector2.Zero, 3f, layerDepth: 0.95f);
          Utility.drawTextWithShadow(b, num10 >= 999 ? " 100%" : Game1.content.LoadString("Strings\\UI:ItemHover_Health", (object) ("+" + num10.ToString())), font, new Vector2((float) (x + 16 /*0x10*/ + 34 + 4), (float) (y2 + 16 /*0x10*/)), Game1.textColor);
          y2 += 34;
        }
      }
      else if (num9 != -300)
      {
        Utility.drawWithShadow(b, Game1.mouseCursors, new Vector2((float) (x + 16 /*0x10*/ + 4), (float) (y2 + 16 /*0x10*/)), new Rectangle(140, 428, 10, 10), Color.White, 0.0f, Vector2.Zero, 3f, layerDepth: 0.95f);
        Utility.drawTextWithShadow(b, Game1.content.LoadString("Strings\\UI:ItemHover_Energy", (object) (num9.ToString() ?? "")), font, new Vector2((float) (x + 16 /*0x10*/ + 34 + 4), (float) (y2 + 16 /*0x10*/)), Game1.textColor);
        y2 += 34;
      }
    }
    if (buffIconsToDisplay != null)
    {
      y2 += 16 /*0x10*/;
      b.Draw(Game1.staminaRect, new Rectangle(x + 12, y2 + 6, width1 - (craftingIngredients != null ? 4 : 24), 2), new Color(207, 147, 103) * 0.8f);
      for (int index3 = 0; index3 < buffIconsToDisplay.Length; ++index3)
      {
        if (!buffIconsToDisplay[index3].Equals("0") && buffIconsToDisplay[index3] != "")
        {
          if (index3 == 12)
          {
            Utility.drawWithShadow(b, Game1.mouseCursors, new Vector2((float) (x + 16 /*0x10*/ + 4), (float) (y2 + 16 /*0x10*/)), new Rectangle(410, 501, 9, 9), Color.White, 0.0f, Vector2.Zero, 3f, layerDepth: 0.95f);
            Utility.drawTextWithShadow(b, buffIconsToDisplay[index3], font, new Vector2((float) (x + 16 /*0x10*/ + 34 + 4), (float) (y2 + 16 /*0x10*/)), Game1.textColor);
          }
          else
          {
            Utility.drawWithShadow(b, Game1.mouseCursors, new Vector2((float) (x + 16 /*0x10*/ + 4), (float) (y2 + 16 /*0x10*/)), new Rectangle(10 + index3 * 10, 428, 10, 10), Color.White, 0.0f, Vector2.Zero, 3f, layerDepth: 0.95f);
            string str = (Convert.ToDouble(buffIconsToDisplay[index3]) > 0.0 ? "+" : "") + buffIconsToDisplay[index3];
            if (index3 <= 11)
              str = Game1.content.LoadString("Strings\\UI:ItemHover_Buff" + index3.ToString(), (object) str);
            Utility.drawTextWithShadow(b, str, font, new Vector2((float) (x + 16 /*0x10*/ + 34 + 4), (float) (y2 + 16 /*0x10*/)), Game1.textColor);
          }
          y2 += 39;
        }
      }
      y2 -= 8;
    }
    if (hoveredItem != null && hoveredItem.attachmentSlots() > 0)
    {
      hoveredItem.drawAttachments(b, x + 16 /*0x10*/, y2 + 16 /*0x10*/);
      if (moneyAmountToDisplayAtBottom > -1)
        y2 += 68 * hoveredItem.attachmentSlots();
    }
    if (moneyAmountToDisplayAtBottom > -1)
    {
      b.Draw(Game1.staminaRect, new Rectangle(x + 12, y2 + 22 - (healAmountToDisplay <= 0 ? 6 : 0), width1 - (craftingIngredients != null ? 4 : 24), 2), new Color(207, 147, 103) * 0.5f);
      string text9 = moneyAmountToDisplayAtBottom.ToString();
      int num11 = 0;
      if (buffIconsToDisplay != null && buffIconsToDisplay.Length > 1 || healAmountToDisplay > 0 || craftingIngredients != null)
        num11 = 8;
      b.DrawString(font, text9, new Vector2((float) (x + 16 /*0x10*/), (float) (y2 + 16 /*0x10*/ + 4 + num11)) + new Vector2(2f, 2f), textShadowColor.Value);
      b.DrawString(font, text9, new Vector2((float) (x + 16 /*0x10*/), (float) (y2 + 16 /*0x10*/ + 4 + num11)) + new Vector2(0.0f, 2f), textShadowColor.Value);
      b.DrawString(font, text9, new Vector2((float) (x + 16 /*0x10*/), (float) (y2 + 16 /*0x10*/ + 4 + num11)) + new Vector2(2f, 0.0f), textShadowColor.Value);
      b.DrawString(font, text9, new Vector2((float) (x + 16 /*0x10*/), (float) (y2 + 16 /*0x10*/ + 4 + num11)), textColor.Value);
      switch (currencySymbol)
      {
        case 0:
          b.Draw(Game1.debrisSpriteSheet, new Vector2((float) ((double) (x + 16 /*0x10*/) + (double) font.MeasureString(text9).X + 20.0), (float) (y2 + 16 /*0x10*/ + 20 + num11)), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.debrisSpriteSheet, 8, 16 /*0x10*/, 16 /*0x10*/)), Color.White, 0.0f, new Vector2(8f, 8f), 4f, SpriteEffects.None, 0.95f);
          break;
        case 1:
          b.Draw(Game1.mouseCursors, new Vector2((float) ((double) (x + 8) + (double) font.MeasureString(text9).X + 20.0), (float) (y2 + 16 /*0x10*/ - 5 + num11)), new Rectangle?(new Rectangle(338, 400, 8, 8)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
          break;
        case 2:
          b.Draw(Game1.mouseCursors, new Vector2((float) ((double) (x + 8) + (double) font.MeasureString(text9).X + 20.0), (float) (y2 + 16 /*0x10*/ - 7 + num11)), new Rectangle?(new Rectangle(211, 373, 9, 10)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
          break;
        case 4:
          b.Draw(Game1.objectSpriteSheet, new Vector2((float) ((double) (x + 8) + (double) font.MeasureString(text9).X + 20.0), (float) (y2 + 16 /*0x10*/ - 7 + num11)), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, 858, 16 /*0x10*/, 16 /*0x10*/)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
          break;
      }
      y2 += 48 /*0x30*/;
      if (extraItemToShowIndex != null)
        y2 += num11;
    }
    if (extraItemToShowIndex != null)
    {
      if (moneyAmountToDisplayAtBottom == -1)
        y2 += 8;
      ParsedItemData dataOrErrorItem = ItemRegistry.GetDataOrErrorItem(extraItemToShowIndex);
      string displayName = dataOrErrorItem.DisplayName;
      Texture2D texture = dataOrErrorItem.GetTexture();
      Rectangle sourceRect = dataOrErrorItem.GetSourceRect();
      string text10 = Game1.content.LoadString("Strings\\UI:ItemHover_Requirements", (object) extraItemToShowAmount, (object) displayName);
      float height2 = Math.Max(font.MeasureString(text10).Y + 21f, 96f);
      IClickableMenu.drawTextureBox(b, Game1.menuTexture, new Rectangle(0, 256 /*0x0100*/, 60, 60), x, y2 + 4, width1 + (craftingIngredients != null ? 21 : 0), (int) height2, Color.White);
      y2 += 20;
      b.DrawString(font, text10, new Vector2((float) (x + 16 /*0x10*/), (float) (y2 + 4)) + new Vector2(2f, 2f), textShadowColor.Value);
      b.DrawString(font, text10, new Vector2((float) (x + 16 /*0x10*/), (float) (y2 + 4)) + new Vector2(0.0f, 2f), textShadowColor.Value);
      b.DrawString(font, text10, new Vector2((float) (x + 16 /*0x10*/), (float) (y2 + 4)) + new Vector2(2f, 0.0f), textShadowColor.Value);
      b.DrawString(Game1.smallFont, text10, new Vector2((float) (x + 16 /*0x10*/), (float) (y2 + 4)), textColor.Value);
      b.Draw(texture, new Vector2((float) (x + 16 /*0x10*/ + (int) font.MeasureString(text10).X + 21), (float) y2), new Rectangle?(sourceRect), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
    }
    if (craftingIngredients == null || !Game1.options.showAdvancedCraftingInformation)
      return;
    Utility.drawTextWithShadow(b, craftingIngredients.getCraftCountText(), font, new Vector2((float) (x + 16 /*0x10*/), (float) (y2 + 16 /*0x10*/ + 4)), Game1.textColor, horizontalShadowOffset: 2, verticalShadowOffset: 2);
    y2 += (int) font.MeasureString("T").Y + 4;
  }

  public delegate void onExit();
}
