// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.AdvancedGameOptions
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

#nullable disable
namespace StardewValley.Menus;

public class AdvancedGameOptions : IClickableMenu
{
  public const int itemsPerPage = 7;
  private string hoverText = "";
  public List<ClickableComponent> optionSlots = new List<ClickableComponent>();
  public int currentItemIndex;
  private ClickableTextureComponent upArrow;
  private ClickableTextureComponent downArrow;
  private ClickableTextureComponent scrollBar;
  public ClickableTextureComponent okButton;
  public List<Action> applySettingCallbacks = new List<Action>();
  public Dictionary<OptionsElement, string> tooltips = new Dictionary<OptionsElement, string>();
  public int ID_okButton = 10000;
  private bool scrolling;
  public List<OptionsElement> options = new List<OptionsElement>();
  private Rectangle scrollBarBounds;
  protected static int _lastSelectedIndex;
  protected static int _lastCurrentItemIndex;
  protected int _lastHoveredIndex;
  protected int _hoverDuration;
  public const int WINDOW_WIDTH = 800;
  public const int WINDOW_HEIGHT = 500;
  public bool initialMonsterSpawnAtValue;
  private int optionsSlotHeld = -1;

  public AdvancedGameOptions()
    : base(Game1.uiViewport.Width / 2 - 400, Game1.uiViewport.Height / 2 - 250, 800, 500)
  {
    this.ResetComponents();
  }

  /// <inheritdoc />
  public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
  {
    base.gameWindowSizeChanged(oldBounds, newBounds);
    this.xPositionOnScreen = Game1.uiViewport.Width / 2 - 400;
    this.yPositionOnScreen = Game1.uiViewport.Height / 2 - 250;
    this.ResetComponents();
  }

  private void ResetComponents()
  {
    int x = this.xPositionOnScreen + this.width + 16 /*0x10*/;
    this.upArrow = new ClickableTextureComponent(new Rectangle(x, this.yPositionOnScreen, 44, 48 /*0x30*/), Game1.mouseCursors, new Rectangle(421, 459, 11, 12), 4f);
    this.downArrow = new ClickableTextureComponent(new Rectangle(x, this.yPositionOnScreen + this.height - 64 /*0x40*/, 44, 48 /*0x30*/), Game1.mouseCursors, new Rectangle(421, 472, 11, 12), 4f);
    this.scrollBarBounds = new Rectangle()
    {
      X = this.upArrow.bounds.X + 12,
      Y = this.upArrow.bounds.Y + this.upArrow.bounds.Height + 4,
      Width = 24
    };
    this.scrollBarBounds.Height = this.downArrow.bounds.Y - 4 - this.scrollBarBounds.Y;
    this.scrollBar = new ClickableTextureComponent(new Rectangle(this.scrollBarBounds.X, this.scrollBarBounds.Y, 24, 40), Game1.mouseCursors, new Rectangle(435, 463, 6, 10), 4f);
    this.optionSlots.Clear();
    for (int index = 0; index < 7; ++index)
      this.optionSlots.Add(new ClickableComponent(new Rectangle(this.xPositionOnScreen + 16 /*0x10*/, this.yPositionOnScreen + index * ((this.height - 16 /*0x10*/) / 7), this.width - 16 /*0x10*/, this.height / 7), index.ToString() ?? "")
      {
        myID = index,
        downNeighborID = index < 6 ? index + 1 : -7777,
        upNeighborID = index > 0 ? index - 1 : -7777,
        fullyImmutable = true
      });
    this.PopulateOptions();
    ClickableTextureComponent textureComponent = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen, this.yPositionOnScreen + this.height + 32 /*0x20*/, 64 /*0x40*/, 64 /*0x40*/), Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46), 1f);
    textureComponent.myID = this.ID_okButton;
    textureComponent.upNeighborID = -99998;
    this.okButton = textureComponent;
    this.populateClickableComponentList();
    if (!Game1.options.SnappyMenus)
      return;
    this.setCurrentlySnappedComponentTo(this.ID_okButton);
    this.snapCursorToCurrentSnappedComponent();
  }

  protected override void customSnapBehavior(int direction, int oldRegion, int oldID)
  {
    base.customSnapBehavior(direction, oldRegion, oldID);
    switch (oldID)
    {
      case 0:
        if (direction != 0)
          break;
        if (this.currentItemIndex > 0)
        {
          this.upArrowPressed();
          Game1.playSound("shiny4");
          break;
        }
        this.snapCursorToCurrentSnappedComponent();
        break;
      case 6:
        if (direction != 2)
          break;
        if (this.currentItemIndex < Math.Max(0, this.options.Count - 7))
        {
          this.downArrowPressed();
          Game1.playSound("shiny4");
          break;
        }
        this.currentlySnappedComponent = this.getComponentWithID(this.ID_okButton);
        if (this.currentlySnappedComponent == null)
          break;
        this.currentlySnappedComponent.upNeighborID = Math.Min(this.options.Count, 7) - 1;
        break;
    }
  }

  public virtual void PopulateOptions()
  {
    this.options.Clear();
    this.tooltips.Clear();
    this.applySettingCallbacks.Clear();
    this.AddHeader(Game1.content.LoadString("Strings\\UI:AGO_Label"));
    this.AddDropdown<Game1.BundleType>(Game1.content.LoadString("Strings\\UI:AGO_CCB"), Game1.content.LoadString("Strings\\UI:AGO_CCB_Tooltip"), true, (Func<Game1.BundleType>) (() => Game1.bundleType), (Action<Game1.BundleType>) (val => Game1.bundleType = val), new KeyValuePair<string, Game1.BundleType>(Game1.content.LoadString("Strings\\UI:AGO_CCB_Normal"), Game1.BundleType.Default), new KeyValuePair<string, Game1.BundleType>(Game1.content.LoadString("Strings\\UI:AGO_CCB_Remixed"), Game1.BundleType.Remixed));
    this.AddCheckbox(Game1.content.LoadString("Strings\\UI:AGO_Year1Completable"), Game1.content.LoadString("Strings\\UI:AGO_Year1Completable_Tooltip"), (Func<bool>) (() => Game1.game1.GetNewGameOption<bool>("YearOneCompletable")), (Action<bool>) (val => Game1.game1.SetNewGameOption<bool>("YearOneCompletable", val)));
    this.AddDropdown<Game1.MineChestType>(Game1.content.LoadString("Strings\\UI:AGO_MineTreasureShuffle"), Game1.content.LoadString("Strings\\UI:AGO_MineTreasureShuffle_Tooltip"), true, (Func<Game1.MineChestType>) (() => Game1.game1.GetNewGameOption<Game1.MineChestType>("MineChests")), (Action<Game1.MineChestType>) (val => Game1.game1.SetNewGameOption<Game1.MineChestType>("MineChests", val)), new KeyValuePair<string, Game1.MineChestType>(Game1.content.LoadString("Strings\\UI:AGO_CCB_Normal"), Game1.MineChestType.Default), new KeyValuePair<string, Game1.MineChestType>(Game1.content.LoadString("Strings\\UI:AGO_CCB_Remixed"), Game1.MineChestType.Remixed));
    this.AddCheckbox(Game1.content.LoadString("Strings\\UI:AGO_FarmMonsters"), Game1.content.LoadString("Strings\\UI:AGO_FarmMonsters_Tooltip"), (Func<bool>) (() =>
    {
      bool flag = Game1.spawnMonstersAtNight;
      if (Game1.game1.newGameSetupOptions.ContainsKey("SpawnMonstersAtNight"))
        flag = Game1.game1.GetNewGameOption<bool>("SpawnMonstersAtNight");
      this.initialMonsterSpawnAtValue = flag;
      return flag;
    }), (Action<bool>) (val =>
    {
      if (this.initialMonsterSpawnAtValue == val)
        return;
      Game1.game1.SetNewGameOption<bool>("SpawnMonstersAtNight", val);
    }));
    this.AddDropdown<float>(Game1.content.LoadString("Strings\\UI:Character_Difficulty"), Game1.content.LoadString("Strings\\UI:AGO_ProfitMargin_Tooltip"), false, (Func<float>) (() => Game1.player.difficultyModifier), (Action<float>) (val => Game1.player.difficultyModifier = val), new KeyValuePair<string, float>(Game1.content.LoadString("Strings\\UI:Character_Normal"), 1f), new KeyValuePair<string, float>("75%", 0.75f), new KeyValuePair<string, float>("50%", 0.5f), new KeyValuePair<string, float>("25%", 0.25f));
    this.AddHeader(Game1.content.LoadString("Strings\\UI:AGO_MPOptions_Label"));
    KeyValuePair<string, int>[] keyValuePairArray = new KeyValuePair<string, int>[Game1.multiplayer.playerLimit];
    keyValuePairArray[0] = new KeyValuePair<string, int>(Game1.content.LoadString("Strings\\UI:Character_none"), 0);
    for (int index = 1; index < Game1.multiplayer.playerLimit; ++index)
      keyValuePairArray[index] = new KeyValuePair<string, int>(index.ToString(), index);
    this.AddDropdown<int>(Game1.content.LoadString("Strings\\UI:Character_StartingCabins"), Game1.content.LoadString("Strings\\UI:AGO_StartingCabins_Tooltip"), false, (Func<int>) (() => Game1.startingCabins), (Action<int>) (val => Game1.startingCabins = val), keyValuePairArray);
    this.AddDropdown<bool>(Game1.content.LoadString("Strings\\UI:Character_CabinLayout"), Game1.content.LoadString("Strings\\UI:AGO_CabinLayout_Tooltip"), false, (Func<bool>) (() => Game1.cabinsSeparate), (Action<bool>) (val => Game1.cabinsSeparate = val), new KeyValuePair<string, bool>(Game1.content.LoadString("Strings\\UI:Character_Close"), false), new KeyValuePair<string, bool>(Game1.content.LoadString("Strings\\UI:Character_Separate"), true));
    this.AddHeader(Game1.content.LoadString("Strings\\UI:AGO_OtherOptions_Label"));
    this.AddTextEntry(Game1.content.LoadString("Strings\\UI:AGO_RandomSeed"), Game1.content.LoadString("Strings\\UI:AGO_RandomSeed_Tooltip"), true, (Func<string>) (() => !Game1.startingGameSeed.HasValue ? "" : Game1.startingGameSeed.Value.ToString()), (Action<string>) (val =>
    {
      val.Trim();
      if (string.IsNullOrEmpty(val))
      {
        Game1.startingGameSeed = new ulong?();
      }
      else
      {
        for (; val.Length > 0; val = val.Substring(0, val.Length - 1))
        {
          ulong result;
          if (ulong.TryParse(val, out result))
          {
            Game1.startingGameSeed = new ulong?(result);
            break;
          }
        }
      }
    }), (Action<OptionsTextEntry>) (textbox =>
    {
      textbox.textBox.numbersOnly = true;
      textbox.textBox.textLimit = 9;
    }));
    this.AddCheckbox(Game1.content.LoadString("Strings\\UI:AGO_LegacyRandomization"), Game1.content.LoadString("Strings\\UI:AGO_LegacyRandomization_Tooltip"), (Func<bool>) (() => Game1.UseLegacyRandom), (Action<bool>) (val => Game1.UseLegacyRandom = val));
    for (int count = this.options.Count; count < 7; ++count)
      this.options.Add(new OptionsElement(""));
  }

  public virtual void CloseAndApply()
  {
    foreach (Action applySettingCallback in this.applySettingCallbacks)
      applySettingCallback();
    this.applySettingCallbacks.Clear();
    this.exitThisMenu();
  }

  public virtual void AddHeader(string label) => this.options.Add(new OptionsElement(label));

  public virtual void AddTextEntry(
    string label,
    string tooltip,
    bool labelOnSeparateLine,
    Func<string> get,
    Action<string> set,
    Action<OptionsTextEntry> configure = null)
  {
    if (labelOnSeparateLine)
    {
      OptionsElement key = new OptionsElement(label)
      {
        style = OptionsElement.Style.OptionLabel
      };
      this.options.Add(key);
      this.tooltips[key] = tooltip;
    }
    OptionsTextEntry option_element = new OptionsTextEntry(labelOnSeparateLine ? string.Empty : label, -999);
    if (configure != null)
      configure(option_element);
    this.tooltips[(OptionsElement) option_element] = tooltip;
    option_element.textBox.Text = get();
    this.applySettingCallbacks.Add((Action) (() => set(option_element.textBox.Text)));
    this.options.Add((OptionsElement) option_element);
  }

  public virtual void AddDropdown<T>(
    string label,
    string tooltip,
    bool labelOnSeparateLine,
    Func<T> get,
    Action<T> set,
    params KeyValuePair<string, T>[] dropdown_options)
  {
    if (labelOnSeparateLine)
    {
      OptionsElement key = new OptionsElement(label)
      {
        style = OptionsElement.Style.OptionLabel
      };
      this.options.Add(key);
      this.tooltips[key] = tooltip;
    }
    OptionsDropDown option_element = new OptionsDropDown(labelOnSeparateLine ? string.Empty : label, -999);
    this.tooltips[(OptionsElement) option_element] = tooltip;
    foreach (KeyValuePair<string, T> dropdownOption in dropdown_options)
    {
      option_element.dropDownDisplayOptions.Add(dropdownOption.Key);
      option_element.dropDownOptions.Add(dropdownOption.Value.ToString());
    }
    option_element.RecalculateBounds();
    T obj = get();
    int num = 0;
    for (int index = 0; index < dropdown_options.Length; ++index)
    {
      KeyValuePair<string, T> dropdownOption = dropdown_options[index];
      if ((object) dropdownOption.Value == null && (object) obj == null || (object) dropdownOption.Value != null && (object) obj != null && dropdownOption.Value.Equals((object) obj))
      {
        num = index;
        break;
      }
    }
    option_element.selectedOption = num;
    this.applySettingCallbacks.Add((Action) (() => set(dropdown_options[option_element.selectedOption].Value)));
    this.options.Add((OptionsElement) option_element);
  }

  public virtual void AddCheckbox(string label, string tooltip, Func<bool> get, Action<bool> set)
  {
    OptionsCheckbox option_element = new OptionsCheckbox(label, -999);
    this.tooltips[(OptionsElement) option_element] = tooltip;
    option_element.isChecked = get();
    this.applySettingCallbacks.Add((Action) (() => set(option_element.isChecked)));
    this.options.Add((OptionsElement) option_element);
  }

  public override bool readyToClose() => false;

  public override void snapToDefaultClickableComponent()
  {
    base.snapToDefaultClickableComponent();
    this.currentlySnappedComponent = this.getComponentWithID(this.ID_okButton);
    this.snapCursorToCurrentSnappedComponent();
  }

  public override void applyMovementKey(int direction)
  {
    if (this.IsDropdownActive())
      return;
    base.applyMovementKey(direction);
  }

  private void setScrollBarToCurrentIndex()
  {
    if (this.options.Count <= 0)
      return;
    this.scrollBar.bounds.Y = this.scrollBarBounds.Y + this.scrollBarBounds.Height / Math.Max(1, this.options.Count - 7) * this.currentItemIndex;
    if (this.currentItemIndex != this.options.Count - 7)
      return;
    this.scrollBar.bounds.Y = this.downArrow.bounds.Y - this.scrollBar.bounds.Height - 4;
  }

  public override void snapCursorToCurrentSnappedComponent()
  {
    if (this.currentlySnappedComponent != null && this.currentlySnappedComponent.myID < this.options.Count)
    {
      switch (this.options[this.currentlySnappedComponent.myID + this.currentItemIndex])
      {
        case OptionsDropDown optionsDropDown:
          Game1.setMousePosition(this.currentlySnappedComponent.bounds.Left + optionsDropDown.bounds.Right - 32 /*0x20*/, this.currentlySnappedComponent.bounds.Center.Y - 4);
          break;
        case OptionsPlusMinusButton _:
          Game1.setMousePosition(this.currentlySnappedComponent.bounds.Left + 64 /*0x40*/, this.currentlySnappedComponent.bounds.Center.Y + 4);
          break;
        case OptionsInputListener _:
          Game1.setMousePosition(this.currentlySnappedComponent.bounds.Right - 48 /*0x30*/, this.currentlySnappedComponent.bounds.Center.Y - 12);
          break;
        default:
          Game1.setMousePosition(this.currentlySnappedComponent.bounds.Left + 48 /*0x30*/, this.currentlySnappedComponent.bounds.Center.Y - 12);
          break;
      }
    }
    else
    {
      if (this.currentlySnappedComponent == null)
        return;
      base.snapCursorToCurrentSnappedComponent();
    }
  }

  public virtual void SetScrollFromY(int y)
  {
    int y1 = this.scrollBar.bounds.Y;
    this.currentItemIndex = (int) Utility.Lerp(0.0f, (float) (this.options.Count - 7), Utility.Clamp((float) (y - this.scrollBarBounds.Y) / (float) this.scrollBarBounds.Height, 0.0f, 1f));
    this.setScrollBarToCurrentIndex();
    int y2 = this.scrollBar.bounds.Y;
    if (y1 == y2)
      return;
    Game1.playSound("shiny4");
  }

  /// <inheritdoc />
  public override void leftClickHeld(int x, int y)
  {
    if (GameMenu.forcePreventClose)
      return;
    base.leftClickHeld(x, y);
    if (this.scrolling)
    {
      this.SetScrollFromY(y);
    }
    else
    {
      if (this.optionsSlotHeld == -1 || this.optionsSlotHeld + this.currentItemIndex >= this.options.Count)
        return;
      this.options[this.currentItemIndex + this.optionsSlotHeld].leftClickHeld(x - this.optionSlots[this.optionsSlotHeld].bounds.X, y - this.optionSlots[this.optionsSlotHeld].bounds.Y);
    }
  }

  public override void setCurrentlySnappedComponentTo(int id)
  {
    this.currentlySnappedComponent = this.getComponentWithID(id);
    this.snapCursorToCurrentSnappedComponent();
  }

  /// <inheritdoc />
  public override void receiveKeyPress(Keys key)
  {
    if (this.optionsSlotHeld != -1 && this.optionsSlotHeld + this.currentItemIndex < this.options.Count || Game1.options.snappyMenus && Game1.options.gamepadControls)
    {
      if (this.currentlySnappedComponent != null && Game1.options.snappyMenus && Game1.options.gamepadControls && this.options.Count > this.currentItemIndex + this.currentlySnappedComponent.myID && this.currentItemIndex + this.currentlySnappedComponent.myID >= 0)
        this.options[this.currentItemIndex + this.currentlySnappedComponent.myID].receiveKeyPress(key);
      else if (this.options.Count > this.currentItemIndex + this.optionsSlotHeld && this.currentItemIndex + this.optionsSlotHeld >= 0)
        this.options[this.currentItemIndex + this.optionsSlotHeld].receiveKeyPress(key);
    }
    base.receiveKeyPress(key);
  }

  /// <inheritdoc />
  public override void receiveScrollWheelAction(int direction)
  {
    if (GameMenu.forcePreventClose || this.IsDropdownActive())
      return;
    base.receiveScrollWheelAction(direction);
    if (direction > 0 && this.currentItemIndex > 0)
    {
      this.upArrowPressed();
      Game1.playSound("shiny4");
    }
    else if (direction < 0 && this.currentItemIndex < Math.Max(0, this.options.Count - 7))
    {
      this.downArrowPressed();
      Game1.playSound("shiny4");
    }
    if (!Game1.options.SnappyMenus)
      return;
    this.snapCursorToCurrentSnappedComponent();
  }

  /// <inheritdoc />
  public override void releaseLeftClick(int x, int y)
  {
    if (GameMenu.forcePreventClose)
      return;
    base.releaseLeftClick(x, y);
    if (this.optionsSlotHeld != -1 && this.optionsSlotHeld + this.currentItemIndex < this.options.Count)
      this.options[this.currentItemIndex + this.optionsSlotHeld].leftClickReleased(x - this.optionSlots[this.optionsSlotHeld].bounds.X, y - this.optionSlots[this.optionsSlotHeld].bounds.Y);
    this.optionsSlotHeld = -1;
    this.scrolling = false;
  }

  public bool IsDropdownActive()
  {
    return this.optionsSlotHeld != -1 && this.optionsSlotHeld + this.currentItemIndex < this.options.Count && this.options[this.currentItemIndex + this.optionsSlotHeld] is OptionsDropDown;
  }

  private void downArrowPressed()
  {
    if (this.IsDropdownActive())
      return;
    this.downArrow.scale = this.downArrow.baseScale;
    ++this.currentItemIndex;
    this.UnsubscribeFromSelectedTextbox();
    this.setScrollBarToCurrentIndex();
  }

  public virtual void UnsubscribeFromSelectedTextbox()
  {
    if (Game1.keyboardDispatcher.Subscriber == null)
      return;
    foreach (OptionsElement option in this.options)
    {
      if (option is OptionsTextEntry optionsTextEntry && Game1.keyboardDispatcher.Subscriber == optionsTextEntry.textBox)
      {
        Game1.keyboardDispatcher.Subscriber = (IKeyboardSubscriber) null;
        break;
      }
    }
  }

  public void preWindowSizeChange()
  {
    AdvancedGameOptions._lastSelectedIndex = this.getCurrentlySnappedComponent() != null ? this.getCurrentlySnappedComponent().myID : -1;
    AdvancedGameOptions._lastCurrentItemIndex = this.currentItemIndex;
  }

  public void postWindowSizeChange()
  {
    if (Game1.options.SnappyMenus)
      Game1.activeClickableMenu.setCurrentlySnappedComponentTo(AdvancedGameOptions._lastSelectedIndex);
    this.currentItemIndex = AdvancedGameOptions._lastCurrentItemIndex;
    this.setScrollBarToCurrentIndex();
  }

  private void upArrowPressed()
  {
    if (this.IsDropdownActive())
      return;
    this.upArrow.scale = this.upArrow.baseScale;
    --this.currentItemIndex;
    this.UnsubscribeFromSelectedTextbox();
    this.setScrollBarToCurrentIndex();
  }

  /// <inheritdoc />
  public override void receiveLeftClick(int x, int y, bool playSound = true)
  {
    if (GameMenu.forcePreventClose)
      return;
    if (this.downArrow.containsPoint(x, y) && this.currentItemIndex < Math.Max(0, this.options.Count - 7))
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
    this.currentItemIndex = Math.Max(0, Math.Min(this.options.Count - 7, this.currentItemIndex));
    if (this.okButton.containsPoint(x, y))
    {
      this.CloseAndApply();
    }
    else
    {
      this.UnsubscribeFromSelectedTextbox();
      for (int index = 0; index < this.optionSlots.Count; ++index)
      {
        if (this.optionSlots[index].bounds.Contains(x, y) && this.currentItemIndex + index < this.options.Count && this.options[this.currentItemIndex + index].bounds.Contains(x - this.optionSlots[index].bounds.X, y - this.optionSlots[index].bounds.Y))
        {
          this.options[this.currentItemIndex + index].receiveLeftClick(x - this.optionSlots[index].bounds.X, y - this.optionSlots[index].bounds.Y);
          this.optionsSlotHeld = index;
          break;
        }
      }
    }
  }

  /// <inheritdoc />
  public override void performHoverAction(int x, int y)
  {
    this.okButton.tryHover(x, y);
    for (int index = 0; index < this.optionSlots.Count; ++index)
    {
      if (this.currentItemIndex >= 0 && this.currentItemIndex + index < this.options.Count && this.options[this.currentItemIndex + index].bounds.Contains(x - this.optionSlots[index].bounds.X, y - this.optionSlots[index].bounds.Y))
      {
        Game1.SetFreeCursorDrag();
        break;
      }
    }
    if (this.scrollBarBounds.Contains(x, y))
      Game1.SetFreeCursorDrag();
    if (GameMenu.forcePreventClose)
      return;
    this.hoverText = "";
    int num = -1;
    if (!this.IsDropdownActive())
    {
      for (int index = 0; index < this.optionSlots.Count; ++index)
      {
        if (this.optionSlots[index].containsPoint(x, y) && index + this.currentItemIndex < this.options.Count && this.hoverText == "")
          num = index + this.currentItemIndex;
      }
    }
    if (this._lastHoveredIndex != num)
    {
      this._lastHoveredIndex = num;
      this._hoverDuration = 0;
    }
    else
      this._hoverDuration += (int) Game1.currentGameTime.ElapsedGameTime.TotalMilliseconds;
    string text;
    if (this._lastHoveredIndex >= 0 && this._hoverDuration >= 500 && this.tooltips.TryGetValue(this.options[this._lastHoveredIndex], out text))
      this.hoverText = Game1.parseText(text);
    this.upArrow.tryHover(x, y);
    this.downArrow.tryHover(x, y);
    this.scrollBar.tryHover(x, y);
  }

  /// <inheritdoc />
  public override void draw(SpriteBatch b)
  {
    SpriteBatch spriteBatch = b;
    Texture2D staminaRect = Game1.staminaRect;
    Viewport viewport = Game1.graphics.GraphicsDevice.Viewport;
    int width = viewport.Width;
    viewport = Game1.graphics.GraphicsDevice.Viewport;
    int height = viewport.Height;
    Rectangle destinationRectangle = new Rectangle(0, 0, width, height);
    Color color = Color.Black * 0.75f;
    spriteBatch.Draw(staminaRect, destinationRectangle, color);
    Game1.DrawBox(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height);
    this.okButton.draw(b);
    b.End();
    b.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp);
    for (int index = 0; index < this.optionSlots.Count; ++index)
    {
      if (this.currentItemIndex >= 0 && this.currentItemIndex + index < this.options.Count)
        this.options[this.currentItemIndex + index].draw(b, this.optionSlots[index].bounds.X, this.optionSlots[index].bounds.Y, (IClickableMenu) this);
    }
    b.End();
    b.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
    if (this.options.Count > 7)
    {
      this.upArrow.draw(b);
      this.downArrow.draw(b);
      IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(403, 383, 6, 6), this.scrollBarBounds.X, this.scrollBarBounds.Y, this.scrollBarBounds.Width, this.scrollBarBounds.Height, Color.White, 4f, false);
      this.scrollBar.draw(b);
    }
    if (!this.hoverText.Equals(""))
      IClickableMenu.drawHoverText(b, this.hoverText, Game1.smallFont);
    this.drawMouse(b);
  }
}
